using System.Runtime.InteropServices;
using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Native.Win32;

namespace DualFrontier.Runtime.Window;

/// <summary>
/// Win32 windowing backend. Lifecycle owns: class registration, HWND, WindowProc delegate
/// pinning, message pump, and the VK_KHR_win32_surface creation for its own HWND. V0.A scope:
/// lifecycle + close handling. V0.B adds the WM_SIZE handler emitting WindowResizeEvent for
/// swapchain recreation. V0.C extends с input event dispatch from WM_KEYDOWN/WM_MOUSEMOVE/etc.
///
/// <para>Thread contract: the HWND and its message queue belong to the creating thread; Show,
/// Hide and PumpMessages must be called from it. Construct through
/// <see cref="PlatformWindow.Create"/> rather than directly, so the host picks the backend.</para>
/// </summary>
public sealed class Win32Window : IWindow
{
    private readonly WindowOptions _options;
    private IntPtr _hwnd;
    private IntPtr _hinstance;
    private string _className = string.Empty;
    private bool _isOpen;
    private GCHandle _wndProcHandle;
    private WindowProc? _wndProc;
    private int _currentWidth;
    private int _currentHeight;

    public int Width => _currentWidth;
    public int Height => _currentHeight;
    public bool IsOpen => _isOpen;

    internal InputEventQueue InputQueue { get; }

    public Win32Window(WindowOptions options, InputEventQueue inputQueue)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(inputQueue);

        _options = options;
        _currentWidth = options.Width;
        _currentHeight = options.Height;
        InputQueue = inputQueue;
        InitializeWin32();
    }

    private void InitializeWin32()
    {
        _hinstance = Win32Api.GetModuleHandle(null);
        if (_hinstance == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                $"GetModuleHandle failed: Win32 error {Win32Api.GetLastError()}");
        }

        _className = $"DualFrontierWindow_{Guid.NewGuid():N}";

        // Pin delegate so GC does not collect it while Win32 holds the function pointer.
        _wndProc = WindowProcedure;
        _wndProcHandle = GCHandle.Alloc(_wndProc);

        // Everything after the GCHandle allocation may throw (class registration, window
        // creation). Because a throwing constructor never hands the instance to the caller,
        // Dispose() is unreachable for it — so we must roll back every resource acquired so
        // far here, or the pinned delegate + registered class leak for the process lifetime (F08).
        try
        {
            var wndClass = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                style = Win32Constants.CS_OWNDC | Win32Constants.CS_HREDRAW | Win32Constants.CS_VREDRAW,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProc),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = _hinstance,
                hIcon = Win32Api.LoadIcon(IntPtr.Zero, Win32Constants.IDI_APPLICATION),
                hCursor = Win32Api.LoadCursor(IntPtr.Zero, Win32Constants.IDC_ARROW),
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = _className,
                hIconSm = IntPtr.Zero,
            };

            if (Win32Api.RegisterClassEx(in wndClass) == 0)
            {
                throw new InvalidOperationException(
                    $"RegisterClassEx failed: Win32 error {Win32Api.GetLastError()}");
            }

            _hwnd = Win32Api.CreateWindowEx(
                dwExStyle: 0,
                lpClassName: _className,
                lpWindowName: _options.Title,
                dwStyle: Win32Constants.WS_OVERLAPPEDWINDOW,
                X: Win32Constants.CW_USEDEFAULT,
                Y: Win32Constants.CW_USEDEFAULT,
                nWidth: _options.Width,
                nHeight: _options.Height,
                hWndParent: IntPtr.Zero,
                hMenu: IntPtr.Zero,
                hInstance: _hinstance,
                lpParam: IntPtr.Zero);

            if (_hwnd == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    $"CreateWindowEx failed: Win32 error {Win32Api.GetLastError()}");
            }

            _isOpen = true;
        }
        catch
        {
            RollbackWin32Initialization();
            throw;
        }
    }

    /// <summary>
    /// Frees every Win32 resource acquired by <see cref="InitializeWin32"/> when initialization
    /// fails partway. Mirrors <see cref="Dispose"/> but is reachable from the failing constructor,
    /// which never exposes the instance to the caller.
    /// </summary>
    private void RollbackWin32Initialization()
    {
        if (_hwnd != IntPtr.Zero)
        {
            Win32Api.DestroyWindow(_hwnd);
            _hwnd = IntPtr.Zero;
        }
        if (!string.IsNullOrEmpty(_className) && _hinstance != IntPtr.Zero)
        {
            Win32Api.UnregisterClass(_className, _hinstance);
            _className = string.Empty;
        }
        if (_wndProcHandle.IsAllocated)
        {
            _wndProcHandle.Free();
        }
        _wndProc = null;
        _isOpen = false;
    }

    private IntPtr WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case Win32Constants.WM_CLOSE:
                _isOpen = false;
                Win32Api.DestroyWindow(hWnd);
                return IntPtr.Zero;
            case Win32Constants.WM_DESTROY:
                Win32Api.PostQuitMessage(0);
                _isOpen = false;
                return IntPtr.Zero;
            case Win32Constants.WM_SIZE:
                WindowEventDecode.UnpackSizeLParam(
                    lParam.ToInt64(), out int reportedWidth, out int reportedHeight);
                // Skip-0×0 and skip-unchanged live in WindowEventDecode so the XCB
                // CONFIGURE_NOTIFY arm obeys the identical law (and both are pinned ungated).
                if (WindowEventDecode.TrySize(
                        reportedWidth, reportedHeight, _currentWidth, _currentHeight,
                        out int newWidth, out int newHeight))
                {
                    _currentWidth = newWidth;
                    _currentHeight = newHeight;
                    InputQueue.Enqueue(new WindowResizeEvent(newWidth, newHeight));
                }
                return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);

            // V0.C.1: keyboard input
            case Win32Constants.WM_KEYDOWN:
            {
                int vk = (int)wParam.ToInt64();
                Key key = VirtualKeyMapper.Map(vk);
                if (key != Key.Unknown)
                {
                    InputQueue.Enqueue(new KeyPressedEvent(key));
                }
                return IntPtr.Zero;
            }
            case Win32Constants.WM_KEYUP:
            {
                int vk = (int)wParam.ToInt64();
                Key key = VirtualKeyMapper.Map(vk);
                if (key != Key.Unknown)
                {
                    InputQueue.Enqueue(new KeyReleasedEvent(key));
                }
                return IntPtr.Zero;
            }

            // System keys (Alt-combinations, F10) are recorded for gameplay but MUST fall
            // through to DefWindowProc — otherwise Windows never processes Alt+F4 (→ WM_CLOSE),
            // Alt+Space (system menu), or F10 (menu activation). Returning zero here swallowed
            // them (F05); returning DefWindowProc preserves system behaviour.
            case Win32Constants.WM_SYSKEYDOWN:
            {
                int vk = (int)wParam.ToInt64();
                Key key = VirtualKeyMapper.Map(vk);
                if (key != Key.Unknown)
                {
                    InputQueue.Enqueue(new KeyPressedEvent(key));
                }
                return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);
            }
            case Win32Constants.WM_SYSKEYUP:
            {
                int vk = (int)wParam.ToInt64();
                Key key = VirtualKeyMapper.Map(vk);
                if (key != Key.Unknown)
                {
                    InputQueue.Enqueue(new KeyReleasedEvent(key));
                }
                return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);
            }

            // V0.C.1: mouse input
            case Win32Constants.WM_MOUSEMOVE:
            {
                long movePacked = lParam.ToInt64();
                // LOWORD = cursor X (signed), HIWORD = cursor Y (signed); cast through short for sign.
                int x = (short)(movePacked & 0xFFFF);
                int y = (short)((movePacked >> 16) & 0xFFFF);
                InputQueue.Enqueue(new MouseMovedEvent(x, y));
                return IntPtr.Zero;
            }
            case Win32Constants.WM_LBUTTONDOWN:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Left, Pressed: true));
                return IntPtr.Zero;
            case Win32Constants.WM_LBUTTONUP:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Left, Pressed: false));
                return IntPtr.Zero;
            case Win32Constants.WM_RBUTTONDOWN:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Right, Pressed: true));
                return IntPtr.Zero;
            case Win32Constants.WM_RBUTTONUP:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Right, Pressed: false));
                return IntPtr.Zero;
            case Win32Constants.WM_MBUTTONDOWN:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Middle, Pressed: true));
                return IntPtr.Zero;
            case Win32Constants.WM_MBUTTONUP:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Middle, Pressed: false));
                return IntPtr.Zero;
            case Win32Constants.WM_MOUSEWHEEL:
            {
                // HIWORD of wParam = wheel delta, signed (Win32 docs); WHEEL_DELTA=120 per notch.
                int rawDelta = (short)((wParam.ToInt64() >> 16) & 0xFFFF);
                int normalizedDelta = rawDelta / Win32Constants.WHEEL_DELTA;
                InputQueue.Enqueue(new MouseWheelEvent(normalizedDelta));
                return IntPtr.Zero;
            }

            // V0.C.1: focus events
            case Win32Constants.WM_SETFOCUS:
                InputQueue.Enqueue(new WindowFocusEvent(Focused: true));
                return IntPtr.Zero;
            case Win32Constants.WM_KILLFOCUS:
                InputQueue.Enqueue(new WindowFocusEvent(Focused: false));
                return IntPtr.Zero;

            default:
                return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }

    /// <summary>
    /// Creates a VkSurfaceKHR for this HWND through VK_KHR_win32_surface. The hinstance is the
    /// process module handle, exactly as at class-registration time — the surface belongs to
    /// this window, so the call that needs both values lives where both are already known.
    /// </summary>
    public IntPtr CreateVulkanSurface(IntPtr instanceHandle)
    {
        if (instanceHandle == IntPtr.Zero)
        {
            throw new ArgumentException("VkInstance handle must be non-zero.", nameof(instanceHandle));
        }
        if (_hwnd == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "Cannot create a Vulkan surface for a window that is not open.");
        }

        IntPtr hinstance = Win32Api.GetModuleHandle(null);
        if (hinstance == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                $"GetModuleHandle (Win32 surface foundation) failed: error {Win32Api.GetLastError()}");
        }

        var createInfo = new VkWin32SurfaceCreateInfoKHR
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
            pNext = IntPtr.Zero,
            flags = 0,
            hinstance = hinstance,
            hwnd = _hwnd,
        };

        VkResult result = VkApi.vkCreateWin32SurfaceKHR(
            instanceHandle, in createInfo, IntPtr.Zero, out IntPtr surface);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException(
                $"vkCreateWin32SurfaceKHR failed: {result}. Verify VK_KHR_win32_surface instance " +
                "extension was activated by VulkanInstance (it selects the surface extension by " +
                "platform).");
        }

        return surface;
    }

    public void Show() => Win32Api.ShowWindow(_hwnd, Win32Constants.SW_SHOW);

    public void Hide() => Win32Api.ShowWindow(_hwnd, Win32Constants.SW_HIDE);

    public void PumpMessages()
    {
        while (Win32Api.PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, Win32Constants.PM_REMOVE))
        {
            if (msg.message == Win32Constants.WM_QUIT)
            {
                _isOpen = false;
                break;
            }
            Win32Api.TranslateMessage(in msg);
            Win32Api.DispatchMessage(in msg);
        }
    }

    public void Dispose()
    {
        if (_hwnd != IntPtr.Zero)
        {
            Win32Api.DestroyWindow(_hwnd);
            _hwnd = IntPtr.Zero;
        }
        if (!string.IsNullOrEmpty(_className) && _hinstance != IntPtr.Zero)
        {
            Win32Api.UnregisterClass(_className, _hinstance);
            _className = string.Empty;
        }
        if (_wndProcHandle.IsAllocated)
        {
            _wndProcHandle.Free();
        }
        _wndProc = null;
        _isOpen = false;
    }
}
