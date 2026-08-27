using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Xcb;

// ===========================================================================
// XCB wire structures. Every layout below is fixed by the X11 protocol, so the
// byte offsets are stable across libxcb versions. All events are delivered in a
// 32-byte buffer; the smaller structs simply leave the tail unread.
// ===========================================================================

/// <summary>
/// xcb_screen_t (40 bytes). Only <c>root</c>, <c>root_visual</c> and <c>root_depth</c> are read,
/// but the whole layout is declared so the pointer arithmetic in the iterator stays honest.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbScreen
{
    internal uint root;
    internal uint default_colormap;
    internal uint white_pixel;
    internal uint black_pixel;
    internal uint current_input_masks;
    internal ushort width_in_pixels;
    internal ushort height_in_pixels;
    internal ushort width_in_millimeters;
    internal ushort height_in_millimeters;
    internal ushort min_installed_maps;
    internal ushort max_installed_maps;
    internal uint root_visual;
    internal byte backing_stores;
    internal byte save_unders;
    internal byte root_depth;
    internal byte allowed_depths_len;
}

/// <summary>xcb_screen_iterator_t (16 bytes) — returned by value from xcb_setup_roots_iterator.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbScreenIterator
{
    internal IntPtr data;
    internal int rem;
    internal int index;
}

/// <summary>
/// xcb_intern_atom_cookie_t / xcb_void_cookie_t — a single sequence number passed by value.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbInternAtomCookie
{
    internal uint sequence;
}

/// <summary>xcb_void_cookie_t. Distinct type from the atom cookie so the two cannot be swapped.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbVoidCookie
{
    internal uint sequence;
}

/// <summary>xcb_intern_atom_reply_t (12 bytes). Caller frees the reply.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbInternAtomReply
{
    internal byte response_type;
    internal byte pad0;
    internal ushort sequence;
    internal uint length;
    internal uint atom;
}

/// <summary>xcb_generic_event_t (32 bytes) — the common head every event shares.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbGenericEvent
{
    internal byte response_type;
    internal byte pad0;
    internal ushort sequence;
    internal uint pad1;
    internal uint pad2;
    internal uint pad3;
    internal uint pad4;
    internal uint pad5;
    internal uint pad6;
    internal uint pad7;
    internal uint full_sequence;
}

/// <summary>xcb_configure_notify_event_t (28 bytes).</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbConfigureNotifyEvent
{
    internal byte response_type;
    internal byte pad0;
    internal ushort sequence;
    internal uint @event;
    internal uint window;
    internal uint above_sibling;
    internal short x;
    internal short y;
    internal ushort width;
    internal ushort height;
    internal ushort border_width;
    internal byte override_redirect;
    internal byte pad1;
}

/// <summary>
/// xcb_client_message_event_t (32 bytes). <c>data</c> is a 20-byte union; the WM_PROTOCOLS
/// convention puts the protocol atom in its first 32-bit word, which is all this backend reads.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbClientMessageEvent
{
    internal byte response_type;
    internal byte format;
    internal ushort sequence;
    internal uint window;
    internal uint type;
    internal uint data0;
    internal uint data1;
    internal uint data2;
    internal uint data3;
    internal uint data4;
}

/// <summary>
/// xcb_key_press_event_t (32 bytes). xcb_key_release_event_t, xcb_button_press_event_t,
/// xcb_button_release_event_t and xcb_motion_notify_event_t are byte-identical in layout —
/// only the meaning of <c>detail</c> differs (keycode / button number / motion hint).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbInputEvent
{
    internal byte response_type;
    internal byte detail;
    internal ushort sequence;
    internal uint time;
    internal uint root;
    internal uint @event;
    internal uint child;
    internal short root_x;
    internal short root_y;
    internal short event_x;
    internal short event_y;
    internal ushort state;
    internal byte same_screen;
    internal byte pad0;
}

/// <summary>xcb_focus_in_event_t / xcb_focus_out_event_t (12 bytes).</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct XcbFocusEvent
{
    internal byte response_type;
    internal byte detail;
    internal ushort sequence;
    internal uint @event;
    internal byte mode;
    internal byte pad0;
    internal byte pad1;
    internal byte pad2;
}
