using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Xcb;

/// <summary>
/// libxcb bindings — the X11 protocol surface the XCB windowing backend needs, and nothing more.
///
/// <para>Deliberately core libxcb only. The convenience helpers <c>libxcb-icccm</c> and
/// <c>libxcb-keysyms</c> are NOT installed on the target host, and neither is needed:
/// <c>xcb_intern_atom</c> covers the WM_PROTOCOLS/WM_DELETE_WINDOW close contract that icccm
/// would wrap, and keycode translation goes through libxkbcommon-x11 (see <see cref="XkbApi"/>),
/// which is the modern path and strictly better than xcb-keysyms anyway.</para>
///
/// <para>Thread contract: an <c>xcb_connection_t</c> is used from the thread that created it.
/// libxcb is itself thread-safe, but the backend's window state is not, so the pump owns it.</para>
/// </summary>
internal static partial class XcbApi
{
    private const string XcbLib = "libxcb.so.1";

    /// <summary>
    /// glibc's soname. The event and reply buffers libxcb hands back are malloc'd and must be
    /// released with the matching free; declared here rather than pulled in as a dependency.
    /// </summary>
    private const string LibC = "libc.so.6";

    // --- connection lifecycle -------------------------------------------------------------

    [LibraryImport(XcbLib, EntryPoint = "xcb_connect", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr xcb_connect(string? displayname, out int screenp);

    [LibraryImport(XcbLib, EntryPoint = "xcb_disconnect")]
    internal static partial void xcb_disconnect(IntPtr connection);

    /// <summary>Non-zero once the connection is in an unrecoverable error state.</summary>
    [LibraryImport(XcbLib, EntryPoint = "xcb_connection_has_error")]
    internal static partial int xcb_connection_has_error(IntPtr connection);

    [LibraryImport(XcbLib, EntryPoint = "xcb_flush")]
    internal static partial int xcb_flush(IntPtr connection);

    // --- setup / screen -------------------------------------------------------------------

    [LibraryImport(XcbLib, EntryPoint = "xcb_get_setup")]
    internal static partial IntPtr xcb_get_setup(IntPtr connection);

    [LibraryImport(XcbLib, EntryPoint = "xcb_setup_roots_iterator")]
    internal static partial XcbScreenIterator xcb_setup_roots_iterator(IntPtr setup);

    [LibraryImport(XcbLib, EntryPoint = "xcb_screen_next")]
    internal static partial void xcb_screen_next(ref XcbScreenIterator iterator);

    // --- window lifecycle -----------------------------------------------------------------

    [LibraryImport(XcbLib, EntryPoint = "xcb_generate_id")]
    internal static partial uint xcb_generate_id(IntPtr connection);

    [LibraryImport(XcbLib, EntryPoint = "xcb_create_window")]
    internal static partial XcbVoidCookie xcb_create_window(
        IntPtr connection,
        byte depth,
        uint wid,
        uint parent,
        short x,
        short y,
        ushort width,
        ushort height,
        ushort border_width,
        ushort @class,
        uint visual,
        uint value_mask,
        ref uint value_list);

    [LibraryImport(XcbLib, EntryPoint = "xcb_map_window")]
    internal static partial XcbVoidCookie xcb_map_window(IntPtr connection, uint window);

    [LibraryImport(XcbLib, EntryPoint = "xcb_unmap_window")]
    internal static partial XcbVoidCookie xcb_unmap_window(IntPtr connection, uint window);

    [LibraryImport(XcbLib, EntryPoint = "xcb_destroy_window")]
    internal static partial XcbVoidCookie xcb_destroy_window(IntPtr connection, uint window);

    [LibraryImport(XcbLib, EntryPoint = "xcb_configure_window")]
    internal static partial XcbVoidCookie xcb_configure_window(
        IntPtr connection, uint window, ushort value_mask, ref uint value_list);

    // --- atoms + properties ---------------------------------------------------------------

    [LibraryImport(XcbLib, EntryPoint = "xcb_intern_atom", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial XcbInternAtomCookie xcb_intern_atom(
        IntPtr connection,
        [MarshalAs(UnmanagedType.U1)] bool only_if_exists,
        ushort name_len,
        string name);

    /// <summary>Blocks for the reply. Returns a malloc'd buffer the caller frees with <see cref="free"/>.</summary>
    [LibraryImport(XcbLib, EntryPoint = "xcb_intern_atom_reply")]
    internal static partial IntPtr xcb_intern_atom_reply(
        IntPtr connection, XcbInternAtomCookie cookie, IntPtr error);

    [LibraryImport(XcbLib, EntryPoint = "xcb_change_property")]
    internal static partial XcbVoidCookie xcb_change_property(
        IntPtr connection,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint data_len,
        IntPtr data);

    // --- events -------------------------------------------------------------------------

    /// <summary>
    /// Non-blocking. Returns a malloc'd event buffer, or <see cref="IntPtr.Zero"/> when the queue
    /// is empty. Every non-null return must be released with <see cref="free"/>.
    /// </summary>
    [LibraryImport(XcbLib, EntryPoint = "xcb_poll_for_event")]
    internal static partial IntPtr xcb_poll_for_event(IntPtr connection);

    [LibraryImport(XcbLib, EntryPoint = "xcb_send_event")]
    internal static partial XcbVoidCookie xcb_send_event(
        IntPtr connection,
        [MarshalAs(UnmanagedType.U1)] bool propagate,
        uint destination,
        uint event_mask,
        IntPtr @event);

    // --- libc ---------------------------------------------------------------------------

    [LibraryImport(LibC, EntryPoint = "free")]
    internal static partial void free(IntPtr ptr);
}
