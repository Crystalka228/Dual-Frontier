using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Win32;

namespace DualFrontier.Runtime.Window;

/// <summary>
/// Win32 window implementation. Lifecycle owns: class registration, HWND, WindowProc delegate
/// pinning, message pump. V0.A scope: lifecycle + close handling. V0.C extends с input event
/// dispatch from WM_KEYDOWN/WM_MOUSEMOVE/etc.
/// </summary>
public sealed class Window : IWindow
{
    private readonly WindowOptions _options;
    private IntPtr _hwnd;
    private IntPtr _hinstance;
    private string _className = string.Empty;
    private bool _isOpen;
    private GCHandle _wndProcHandle;
    private WindowProc? _wndProc;

    public IntPtr Handle => _hwnd;
    public int Width => _options.Width;
    public int Height => _options.Height;
    public bool IsOpen => _isOpen;

    internal InputEventQueue InputQueue { get; }

    public Window(WindowOptions options, InputEventQueue inputQueue)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(inputQueue);

        _options = options;
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
            // V0.C: WM_SIZE, WM_KILLFOCUS, WM_SETFOCUS, WM_KEYDOWN/UP, WM_MOUSE* dispatch.
            default:
                return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);
        }
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
