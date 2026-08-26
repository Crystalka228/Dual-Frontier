using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Xcb;

/// <summary>
/// libxkbcommon + libxkbcommon-x11 bindings: X11 keycode to keysym translation with the
/// keyboard's real layout and modifier state applied.
///
/// <para>This is the path <c>libxcb-keysyms</c> would otherwise serve, and it is the better one:
/// xkbcommon tracks modifier state properly across press/release, so a shifted key yields the
/// shifted keysym rather than requiring the caller to re-derive it. The host also does not ship
/// xcb-keysyms, so this is the only path available here.</para>
/// </summary>
internal static partial class XkbApi
{
    private const string XkbLib = "libxkbcommon.so.0";
    private const string XkbX11Lib = "libxkbcommon-x11.so.0";

    internal const int XKB_CONTEXT_NO_FLAGS = 0;
    internal const int XKB_KEYMAP_COMPILE_NO_FLAGS = 0;
    internal const int XKB_X11_SETUP_XKB_EXTENSION_NO_FLAGS = 0;

    /// <summary>Minimum XKB protocol version libxkbcommon-x11 requires of the server.</summary>
    internal const ushort XKB_X11_MIN_MAJOR_XKB_VERSION = 1;
    internal const ushort XKB_X11_MIN_MINOR_XKB_VERSION = 0;

    /// <summary>xkb_key_direction — argument to <see cref="xkb_state_update_key"/>.</summary>
    internal const int XKB_KEY_UP = 0;
    internal const int XKB_KEY_DOWN = 1;

    /// <summary>Returned by <see cref="xkb_x11_get_core_keyboard_device_id"/> on failure.</summary>
    internal const int XKB_INVALID_DEVICE_ID = -1;

    [LibraryImport(XkbLib, EntryPoint = "xkb_context_new")]
    internal static partial IntPtr xkb_context_new(int flags);

    [LibraryImport(XkbLib, EntryPoint = "xkb_context_unref")]
    internal static partial void xkb_context_unref(IntPtr context);

    [LibraryImport(XkbLib, EntryPoint = "xkb_keymap_unref")]
    internal static partial void xkb_keymap_unref(IntPtr keymap);

    [LibraryImport(XkbLib, EntryPoint = "xkb_state_unref")]
    internal static partial void xkb_state_unref(IntPtr state);

    /// <summary>Resolves the keycode to the keysym for the state's current modifier level.</summary>
    [LibraryImport(XkbLib, EntryPoint = "xkb_state_key_get_one_sym")]
    internal static partial uint xkb_state_key_get_one_sym(IntPtr state, uint key);

    [LibraryImport(XkbLib, EntryPoint = "xkb_state_update_key")]
    internal static partial uint xkb_state_update_key(IntPtr state, uint key, int direction);

    /// <summary>Negotiates the XKB extension on the connection. Non-zero on success.</summary>
    [LibraryImport(XkbX11Lib, EntryPoint = "xkb_x11_setup_xkb_extension")]
    internal static partial int xkb_x11_setup_xkb_extension(
        IntPtr connection,
        ushort major_xkb_version,
        ushort minor_xkb_version,
        int flags,
        IntPtr major_xkb_version_out,
        IntPtr minor_xkb_version_out,
        IntPtr base_event_out,
        IntPtr base_error_out);

    [LibraryImport(XkbX11Lib, EntryPoint = "xkb_x11_get_core_keyboard_device_id")]
    internal static partial int xkb_x11_get_core_keyboard_device_id(IntPtr connection);

    [LibraryImport(XkbX11Lib, EntryPoint = "xkb_x11_keymap_new_from_device")]
    internal static partial IntPtr xkb_x11_keymap_new_from_device(
        IntPtr context, IntPtr connection, int device_id, int flags);

    [LibraryImport(XkbX11Lib, EntryPoint = "xkb_x11_state_new_from_device")]
    internal static partial IntPtr xkb_x11_state_new_from_device(
        IntPtr keymap, IntPtr connection, int device_id);
}
