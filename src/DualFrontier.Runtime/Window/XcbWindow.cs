using System.Runtime.InteropServices;
using System.Text;
using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Native.Vulkan;
using DualFrontier.Runtime.Native.Xcb;

namespace DualFrontier.Runtime.Window;

/// <summary>
/// XCB windowing backend. On a Wayland session this runs through XWayland, which is a fully
/// functional path for VK_KHR_xcb_surface; a Wayland-native backend is separate future work.
///
/// <para>Lifecycle owns: the xcb connection, the window id, the WM_DELETE_WINDOW protocol
/// registration, the xkbcommon keyboard state, and the VK_KHR_xcb_surface creation for its own
/// window. The event table mirrors the Win32 backend's message table one-for-one, and both share
/// the resize law in <see cref="WindowEventDecode"/>.</para>
///
/// <para>Thread contract: the connection is used only from the thread that created it — the same
/// law the Win32 backend's HWND imposes. Construct through <see cref="PlatformWindow.Create"/>.</para>
/// </summary>
public sealed class XcbWindow : IWindow
{
    private readonly WindowOptions _options;
    private IntPtr _connection;
    private uint _window;
    private uint _wmDeleteWindowAtom;
    private uint _wmProtocolsAtom;
    private IntPtr _xkbContext;
    private IntPtr _xkbKeymap;
    private IntPtr _xkbState;
    private bool _isOpen;
    private int _currentWidth;
    private int _currentHeight;

    public int Width => _currentWidth;
    public int Height => _currentHeight;
    public bool IsOpen => _isOpen;

    internal InputEventQueue InputQueue { get; }

    /// <summary>The live xcb connection. Test-visible so display-gated pins can drive the server.</summary>
    internal IntPtr Connection => _connection;

    /// <summary>The X11 window id. Test-visible for the same reason as <see cref="Connection"/>.</summary>
    internal uint WindowId => _window;

    /// <summary>The interned WM_DELETE_WINDOW atom, so a pin can synthesise the close protocol.</summary>
    internal uint WmDeleteWindowAtom => _wmDeleteWindowAtom;

    public XcbWindow(WindowOptions options, InputEventQueue inputQueue)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(inputQueue);

        // The X11 CreateWindow request carries width/height as 16-bit unsigned values, so an
        // out-of-range dimension would WRAP rather than fail: 65536 narrows to 0 and 70000 to
        // 4464. The request is unchecked and X reports protocol errors asynchronously, so the
        // constructor would go on to set IsOpen and keep the original ints while the server had
        // rejected the window or built it at a different size. Reject it here instead.
        ArgumentOutOfRangeException.ThrowIfLessThan(options.Width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.Height, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(options.Width, ushort.MaxValue);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(options.Height, ushort.MaxValue);

        _options = options;
        _currentWidth = options.Width;
        _currentHeight = options.Height;
        InputQueue = inputQueue;
        InitializeXcb();
    }

    private void InitializeXcb()
    {
        // Passing null uses $DISPLAY, which is what every other X client does.
        _connection = XcbApi.xcb_connect(null, out int screenNumber);
        if (_connection == IntPtr.Zero || XcbApi.xcb_connection_has_error(_connection) != 0)
        {
            // xcb_connect never returns null; it returns a connection already in the error
            // state, which still has to be disconnected to release it.
            int code = _connection == IntPtr.Zero ? -1 : XcbApi.xcb_connection_has_error(_connection);
            if (_connection != IntPtr.Zero)
            {
                XcbApi.xcb_disconnect(_connection);
                _connection = IntPtr.Zero;
            }
            throw new InvalidOperationException(
                $"xcb_connect failed (error {code}). Verify DISPLAY is set and the X server " +
                "(or XWayland) is reachable.");
        }

        // Everything after the connection may throw. A throwing constructor never hands the
        // instance to the caller, so Dispose is unreachable for it and every resource acquired
        // here must be rolled back on the way out — the same F08 rationale the Win32 backend
        // carries, with the connection released last.
        try
        {
            XcbScreen screen = FindScreen(screenNumber);

            _window = XcbApi.xcb_generate_id(_connection);

            // value_list entries follow the value_mask bits in ascending order: BACK_PIXEL
            // (0x02) then EVENT_MASK (0x800).
            var values = new uint[2];
            values[0] = screen.black_pixel;
            values[1] = XcbConstants.XCB_EVENT_MASK_STRUCTURE_NOTIFY
                | XcbConstants.XCB_EVENT_MASK_KEY_PRESS
                | XcbConstants.XCB_EVENT_MASK_KEY_RELEASE
                | XcbConstants.XCB_EVENT_MASK_BUTTON_PRESS
                | XcbConstants.XCB_EVENT_MASK_BUTTON_RELEASE
                | XcbConstants.XCB_EVENT_MASK_POINTER_MOTION
                | XcbConstants.XCB_EVENT_MASK_FOCUS_CHANGE;

            XcbApi.xcb_create_window(
                _connection,
                depth: XcbConstants.XCB_COPY_FROM_PARENT,
                wid: _window,
                parent: screen.root,
                x: 0,
                y: 0,
                width: (ushort)_options.Width,
                height: (ushort)_options.Height,
                border_width: 0,
                @class: XcbConstants.XCB_WINDOW_CLASS_INPUT_OUTPUT,
                visual: screen.root_visual,
                value_mask: XcbConstants.XCB_CW_BACK_PIXEL | XcbConstants.XCB_CW_EVENT_MASK,
                value_list: ref values[0]);

            RegisterCloseProtocol();
            SetTitle(_options.Title);
            InitializeKeyboard();

            XcbApi.xcb_flush(_connection);
            _isOpen = true;
        }
        catch
        {
            RollbackXcbInitialization();
            throw;
        }
    }

    /// <summary>
    /// Walks the roots iterator к the screen <c>xcb_connect</c> reported. Almost always index 0,
    /// but a multi-seat server can hand back another, and reading the wrong screen would create
    /// the window on the wrong root with an incompatible visual.
    /// </summary>
    private XcbScreen FindScreen(int screenNumber)
    {
        IntPtr setup = XcbApi.xcb_get_setup(_connection);
        if (setup == IntPtr.Zero)
        {
            throw new InvalidOperationException("xcb_get_setup returned null; the X connection is unusable.");
        }

        XcbScreenIterator iterator = XcbApi.xcb_setup_roots_iterator(setup);
        for (int i = 0; i < screenNumber && iterator.rem > 0; i++)
        {
            XcbApi.xcb_screen_next(ref iterator);
        }
        if (iterator.rem <= 0 || iterator.data == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                $"X screen {screenNumber} not present in the server setup.");
        }

        return Marshal.PtrToStructure<XcbScreen>(iterator.data);
    }

    /// <summary>
    /// Registers WM_DELETE_WINDOW on WM_PROTOCOLS. Without it the window manager closes the
    /// window by killing the client outright instead of sending a client message, so the
    /// application never gets to shut down in order.
    /// </summary>
    private void RegisterCloseProtocol()
    {
        _wmProtocolsAtom = InternAtom("WM_PROTOCOLS");
        _wmDeleteWindowAtom = InternAtom("WM_DELETE_WINDOW");

        uint deleteAtom = _wmDeleteWindowAtom;
        IntPtr buffer = Marshal.AllocHGlobal(sizeof(uint));
        try
        {
            Marshal.WriteInt32(buffer, (int)deleteAtom);
            XcbApi.xcb_change_property(
                _connection,
                XcbConstants.XCB_PROP_MODE_REPLACE,
                _window,
                _wmProtocolsAtom,
                XcbConstants.XCB_ATOM_ATOM,
                format: 32,
                data_len: 1,
                data: buffer);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private uint InternAtom(string name)
    {
        byte[] utf8 = Encoding.UTF8.GetBytes(name);
        XcbInternAtomCookie cookie = XcbApi.xcb_intern_atom(
            _connection, only_if_exists: false, (ushort)utf8.Length, name);

        IntPtr reply = XcbApi.xcb_intern_atom_reply(_connection, cookie, IntPtr.Zero);
        if (reply == IntPtr.Zero)
        {
            throw new InvalidOperationException($"xcb_intern_atom failed for '{name}'.");
        }
        try
        {
            return Marshal.PtrToStructure<XcbInternAtomReply>(reply).atom;
        }
        finally
        {
            XcbApi.free(reply);
        }
    }

    /// <summary>
    /// Sets the window title on both _NET_WM_NAME (UTF-8, what modern window managers read) and
    /// WM_NAME (Latin-1, the ICCCM fallback), so the title shows regardless of WM vintage.
    /// </summary>
    private void SetTitle(string title)
    {
        byte[] utf8 = Encoding.UTF8.GetBytes(title);
        uint netWmName = InternAtom("_NET_WM_NAME");
        uint utf8String = InternAtom("UTF8_STRING");

        IntPtr buffer = Marshal.AllocHGlobal(utf8.Length == 0 ? 1 : utf8.Length);
        try
        {
            Marshal.Copy(utf8, 0, buffer, utf8.Length);
            XcbApi.xcb_change_property(
                _connection, XcbConstants.XCB_PROP_MODE_REPLACE, _window,
                netWmName, utf8String, format: 8, data_len: (uint)utf8.Length, data: buffer);
            XcbApi.xcb_change_property(
                _connection, XcbConstants.XCB_PROP_MODE_REPLACE, _window,
                XcbConstants.XCB_ATOM_WM_NAME, XcbConstants.XCB_ATOM_STRING,
                format: 8, data_len: (uint)utf8.Length, data: buffer);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Brings up xkbcommon over this connection. Fails fast rather than degrading silently: a
    /// window that opens but cannot report a keystroke is a defect that would surface much later
    /// and much less legibly than a throw here.
    /// </summary>
    private void InitializeKeyboard()
    {
        if (XkbApi.xkb_x11_setup_xkb_extension(
                _connection,
                XkbApi.XKB_X11_MIN_MAJOR_XKB_VERSION,
                XkbApi.XKB_X11_MIN_MINOR_XKB_VERSION,
                XkbApi.XKB_X11_SETUP_XKB_EXTENSION_NO_FLAGS,
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) == 0)
        {
            throw new InvalidOperationException(
                "xkb_x11_setup_xkb_extension failed; the X server does not offer the XKB extension.");
        }

        _xkbContext = XkbApi.xkb_context_new(XkbApi.XKB_CONTEXT_NO_FLAGS);
        if (_xkbContext == IntPtr.Zero)
        {
            throw new InvalidOperationException("xkb_context_new failed.");
        }

        int deviceId = XkbApi.xkb_x11_get_core_keyboard_device_id(_connection);
        if (deviceId == XkbApi.XKB_INVALID_DEVICE_ID)
        {
            throw new InvalidOperationException("xkb_x11_get_core_keyboard_device_id failed.");
        }

        _xkbKeymap = XkbApi.xkb_x11_keymap_new_from_device(
            _xkbContext, _connection, deviceId, XkbApi.XKB_KEYMAP_COMPILE_NO_FLAGS);
        if (_xkbKeymap == IntPtr.Zero)
        {
            throw new InvalidOperationException("xkb_x11_keymap_new_from_device failed.");
        }

        _xkbState = XkbApi.xkb_x11_state_new_from_device(_xkbKeymap, _connection, deviceId);
        if (_xkbState == IntPtr.Zero)
        {
            throw new InvalidOperationException("xkb_x11_state_new_from_device failed.");
        }
    }

    /// <summary>
    /// Frees every X and xkb resource acquired by <see cref="InitializeXcb"/> when initialization
    /// fails partway. Mirrors <see cref="Dispose"/> but is reachable from the failing constructor,
    /// which never exposes the instance to the caller.
    /// </summary>
    private void RollbackXcbInitialization()
    {
        ReleaseKeyboard();
        if (_window != 0 && _connection != IntPtr.Zero)
        {
            XcbApi.xcb_destroy_window(_connection, _window);
            XcbApi.xcb_flush(_connection);
            _window = 0;
        }
        if (_connection != IntPtr.Zero)
        {
            XcbApi.xcb_disconnect(_connection);
            _connection = IntPtr.Zero;
        }
        _isOpen = false;
    }

    private void ReleaseKeyboard()
    {
        if (_xkbState != IntPtr.Zero)
        {
            XkbApi.xkb_state_unref(_xkbState);
            _xkbState = IntPtr.Zero;
        }
        if (_xkbKeymap != IntPtr.Zero)
        {
            XkbApi.xkb_keymap_unref(_xkbKeymap);
            _xkbKeymap = IntPtr.Zero;
        }
        if (_xkbContext != IntPtr.Zero)
        {
            XkbApi.xkb_context_unref(_xkbContext);
            _xkbContext = IntPtr.Zero;
        }
    }

    public void Show()
    {
        XcbApi.xcb_map_window(_connection, _window);
        XcbApi.xcb_flush(_connection);
    }

    public void Hide()
    {
        XcbApi.xcb_unmap_window(_connection, _window);
        XcbApi.xcb_flush(_connection);
    }

    public void PumpMessages()
    {
        if (_connection == IntPtr.Zero)
        {
            return;
        }

        // A dead connection is the XCB analogue of WM_QUIT: nothing further will ever arrive,
        // and the main loop must be allowed to leave.
        if (XcbApi.xcb_connection_has_error(_connection) != 0)
        {
            _isOpen = false;
            return;
        }

        IntPtr eventPtr;
        while ((eventPtr = XcbApi.xcb_poll_for_event(_connection)) != IntPtr.Zero)
        {
            try
            {
                DispatchEvent(eventPtr);
            }
            finally
            {
                // xcb_poll_for_event hands back a malloc'd buffer per event; the caller owns it.
                XcbApi.free(eventPtr);
            }
        }
    }

    private void DispatchEvent(IntPtr eventPtr)
    {
        byte rawType = Marshal.ReadByte(eventPtr);
        // The high bit marks an event synthesised through xcb_send_event — WM_DELETE_WINDOW
        // arrives that way, so it must be masked off rather than treated as a distinct type.
        byte type = (byte)(rawType & XcbConstants.XCB_RESPONSE_TYPE_MASK);

        switch (type)
        {
            case XcbConstants.XCB_CLIENT_MESSAGE:
            {
                var message = Marshal.PtrToStructure<XcbClientMessageEvent>(eventPtr);
                // The WM_PROTOCOLS convention: the protocol atom is the first data word.
                if (message.type == _wmProtocolsAtom && message.data0 == _wmDeleteWindowAtom)
                {
                    _isOpen = false;
                }
                break;
            }

            case XcbConstants.XCB_CONFIGURE_NOTIFY:
            {
                var configure = Marshal.PtrToStructure<XcbConfigureNotifyEvent>(eventPtr);
                // Width/Height are the load-bearing outputs of the pump: the swapchain recreate
                // path reads them on an out-of-date result. The resize EVENT below is published
                // for parity with the Win32 arm but has no consumer today.
                if (WindowEventDecode.TrySize(
                        configure.width, configure.height, _currentWidth, _currentHeight,
                        out int newWidth, out int newHeight))
                {
                    _currentWidth = newWidth;
                    _currentHeight = newHeight;
                    InputQueue.Enqueue(new WindowResizeEvent(newWidth, newHeight));
                }
                break;
            }

            case XcbConstants.XCB_KEY_PRESS:
            {
                var keyEvent = Marshal.PtrToStructure<XcbInputEvent>(eventPtr);
                Key key = MapKeycode(keyEvent.detail);
                XkbApi.xkb_state_update_key(_xkbState, keyEvent.detail, XkbApi.XKB_KEY_DOWN);
                if (key != Key.Unknown)
                {
                    InputQueue.Enqueue(new KeyPressedEvent(key));
                }
                break;
            }

            case XcbConstants.XCB_KEY_RELEASE:
            {
                var keyEvent = Marshal.PtrToStructure<XcbInputEvent>(eventPtr);
                Key key = MapKeycode(keyEvent.detail);
                XkbApi.xkb_state_update_key(_xkbState, keyEvent.detail, XkbApi.XKB_KEY_UP);
                if (key != Key.Unknown)
                {
                    InputQueue.Enqueue(new KeyReleasedEvent(key));
                }
                break;
            }

            case XcbConstants.XCB_BUTTON_PRESS:
                DispatchButton(eventPtr, pressed: true);
                break;

            case XcbConstants.XCB_BUTTON_RELEASE:
                DispatchButton(eventPtr, pressed: false);
                break;

            case XcbConstants.XCB_MOTION_NOTIFY:
            {
                var motion = Marshal.PtrToStructure<XcbInputEvent>(eventPtr);
                InputQueue.Enqueue(new MouseMovedEvent(motion.event_x, motion.event_y));
                break;
            }

            case XcbConstants.XCB_FOCUS_IN:
                InputQueue.Enqueue(new WindowFocusEvent(Focused: true));
                break;

            case XcbConstants.XCB_FOCUS_OUT:
                InputQueue.Enqueue(new WindowFocusEvent(Focused: false));
                break;

            default:
                // Every other event is unsubscribed or uninteresting; X sends some regardless.
                break;
        }
    }

    private void DispatchButton(IntPtr eventPtr, bool pressed)
    {
        var button = Marshal.PtrToStructure<XcbInputEvent>(eventPtr);
        switch (button.detail)
        {
            // Mapped by MEANING, not by the Win32 numbering: X11 calls the middle button 2 and
            // the right button 3, the opposite order to WM_?BUTTON's Left/Right/Middle grouping.
            case XcbConstants.XCB_BUTTON_LEFT:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Left, pressed));
                break;
            case XcbConstants.XCB_BUTTON_MIDDLE:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Middle, pressed));
                break;
            case XcbConstants.XCB_BUTTON_RIGHT:
                InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Right, pressed));
                break;
            // X11 reports wheel notches as press/release pairs on buttons 4 and 5. Only the press
            // is published, so one notch produces one event — matching WM_MOUSEWHEEL's normalised
            // +/-1 per notch rather than double-counting the release.
            case XcbConstants.XCB_BUTTON_WHEEL_UP:
                if (pressed)
                {
                    InputQueue.Enqueue(new MouseWheelEvent(1));
                }
                break;
            case XcbConstants.XCB_BUTTON_WHEEL_DOWN:
                if (pressed)
                {
                    InputQueue.Enqueue(new MouseWheelEvent(-1));
                }
                break;
            default:
                break;
        }
    }

    private Key MapKeycode(byte keycode)
    {
        if (_xkbState == IntPtr.Zero)
        {
            return Key.Unknown;
        }
        return XkbKeysymMapper.Map(XkbApi.xkb_state_key_get_one_sym(_xkbState, keycode));
    }

    /// <summary>
    /// Creates a VkSurfaceKHR for this window through VK_KHR_xcb_surface. The connection and the
    /// window id are both this object's own state — which is exactly why surface creation lives
    /// behind <see cref="IWindow"/> rather than behind a single exported handle.
    /// </summary>
    public IntPtr CreateVulkanSurface(IntPtr instanceHandle)
    {
        if (instanceHandle == IntPtr.Zero)
        {
            throw new ArgumentException("VkInstance handle must be non-zero.", nameof(instanceHandle));
        }
        if (_connection == IntPtr.Zero || _window == 0)
        {
            throw new InvalidOperationException(
                "Cannot create a Vulkan surface for a window that is not open.");
        }

        var createInfo = new VkXcbSurfaceCreateInfoKHR
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_XCB_SURFACE_CREATE_INFO_KHR,
            pNext = IntPtr.Zero,
            flags = 0,
            connection = _connection,
            window = _window,
        };

        VkResult result = VkApi.vkCreateXcbSurfaceKHR(
            instanceHandle, in createInfo, IntPtr.Zero, out IntPtr surface);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException(
                $"vkCreateXcbSurfaceKHR failed: {result}. Verify VK_KHR_xcb_surface instance " +
                "extension was activated by VulkanInstance (it selects the surface extension by " +
                "platform).");
        }

        return surface;
    }

    public void Dispose()
    {
        ReleaseKeyboard();
        if (_window != 0 && _connection != IntPtr.Zero)
        {
            XcbApi.xcb_destroy_window(_connection, _window);
            XcbApi.xcb_flush(_connection);
            _window = 0;
        }
        if (_connection != IntPtr.Zero)
        {
            XcbApi.xcb_disconnect(_connection);
            _connection = IntPtr.Zero;
        }
        _isOpen = false;
    }
}
