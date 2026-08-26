namespace DualFrontier.Runtime.Native.Xcb;

/// <summary>
/// XCB protocol constants used by the windowing backend. Values are the X11 protocol's own and
/// are fixed by the wire format, not by any library version.
/// </summary>
internal static class XcbConstants
{
    // Event codes, as delivered in xcb_generic_event_t.response_type. The high bit of that byte
    // flags a SendEvent-synthesised event, so it MUST be masked off before comparing.
    internal const byte XCB_RESPONSE_TYPE_MASK = 0x7F;

    internal const byte XCB_KEY_PRESS = 2;
    internal const byte XCB_KEY_RELEASE = 3;
    internal const byte XCB_BUTTON_PRESS = 4;
    internal const byte XCB_BUTTON_RELEASE = 5;
    internal const byte XCB_MOTION_NOTIFY = 6;
    internal const byte XCB_FOCUS_IN = 9;
    internal const byte XCB_FOCUS_OUT = 10;
    internal const byte XCB_CONFIGURE_NOTIFY = 22;
    internal const byte XCB_CLIENT_MESSAGE = 33;

    // Pointer button numbers arrive in the event's detail field. 1/2/3 are the physical
    // buttons; 4/5 are vertical wheel notches, which X11 reports as button presses.
    internal const byte XCB_BUTTON_LEFT = 1;
    internal const byte XCB_BUTTON_MIDDLE = 2;
    internal const byte XCB_BUTTON_RIGHT = 3;
    internal const byte XCB_BUTTON_WHEEL_UP = 4;
    internal const byte XCB_BUTTON_WHEEL_DOWN = 5;

    // xcb_cw_t — the value_mask bits for xcb_create_window, and the order the value_list
    // entries must appear in (ascending bit order, per the X11 protocol).
    internal const uint XCB_CW_BACK_PIXEL = 0x00000002;
    internal const uint XCB_CW_EVENT_MASK = 0x00000800;

    // xcb_event_mask_t
    internal const uint XCB_EVENT_MASK_KEY_PRESS = 0x00000001;
    internal const uint XCB_EVENT_MASK_KEY_RELEASE = 0x00000002;
    internal const uint XCB_EVENT_MASK_BUTTON_PRESS = 0x00000004;
    internal const uint XCB_EVENT_MASK_BUTTON_RELEASE = 0x00000008;
    internal const uint XCB_EVENT_MASK_POINTER_MOTION = 0x00000040;
    internal const uint XCB_EVENT_MASK_STRUCTURE_NOTIFY = 0x00020000;
    internal const uint XCB_EVENT_MASK_FOCUS_CHANGE = 0x00200000;

    internal const ushort XCB_WINDOW_CLASS_INPUT_OUTPUT = 1;
    internal const byte XCB_COPY_FROM_PARENT = 0;

    // xcb_config_window_t — value_mask bits for xcb_configure_window.
    internal const ushort XCB_CONFIG_WINDOW_WIDTH = 0x0004;
    internal const ushort XCB_CONFIG_WINDOW_HEIGHT = 0x0008;

    internal const byte XCB_PROP_MODE_REPLACE = 0;

    // Predefined atoms (X11 protocol, fixed ids — no intern round-trip needed).
    internal const uint XCB_ATOM_NONE = 0;
    internal const uint XCB_ATOM_ATOM = 4;
    internal const uint XCB_ATOM_STRING = 31;
    internal const uint XCB_ATOM_WM_NAME = 39;
}
