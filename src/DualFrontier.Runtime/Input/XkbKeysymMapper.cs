namespace DualFrontier.Runtime.Input;

/// <summary>
/// Maps X11 keysyms (as produced by <c>xkb_state_key_get_one_sym</c>) к <see cref="Key"/>.
/// Symbols outside the supported set return <see cref="Key.Unknown"/>. Covers the same ground as
/// <see cref="VirtualKeyMapper"/> does for Win32: arrows, modifiers, special keys, F1-F12,
/// letters A-Z, digits 0-9.
///
/// <para>This is a SEPARATE mapper rather than an extension of <see cref="VirtualKeyMapper"/>,
/// because the two input domains are genuinely different. A Win32 virtual-key code is
/// layout-independent and identifies a physical key; an X11 keysym is the character the key
/// produces under the active layout and modifier level, so <c>a</c> and <c>A</c> are distinct
/// keysyms for one physical key and both must fold к <see cref="Key.A"/>.</para>
///
/// <para>Consequence worth stating plainly: under a non-Latin layout the letter keysyms are not
/// Latin and fold к <see cref="Key.Unknown"/>. That is a property of resolving through the
/// layout, not a gap in this table. It is inert today — the Launcher drains and discards input —
/// and the fix (resolving letters through the keymap's Latin level) belongs with the cascade
/// that gives input a consumer.</para>
/// </summary>
public static class XkbKeysymMapper
{
    // X11 keysymdef.h. The 0xFF00 block is the function/control set; the low range is Latin-1,
    // where the keysym equals the ASCII code point.
    private const uint XK_BackSpace = 0xFF08;
    private const uint XK_Tab = 0xFF09;
    private const uint XK_Return = 0xFF0D;
    private const uint XK_Escape = 0xFF1B;
    private const uint XK_Home = 0xFF50;
    private const uint XK_Left = 0xFF51;
    private const uint XK_Up = 0xFF52;
    private const uint XK_Right = 0xFF53;
    private const uint XK_Down = 0xFF54;
    private const uint XK_Prior = 0xFF55;   // Page Up
    private const uint XK_Next = 0xFF56;    // Page Down
    private const uint XK_End = 0xFF57;
    private const uint XK_KP_Enter = 0xFF8D;
    private const uint XK_F1 = 0xFFBE;
    private const uint XK_F12 = 0xFFC9;
    private const uint XK_Shift_L = 0xFFE1;
    private const uint XK_Shift_R = 0xFFE2;
    private const uint XK_Control_L = 0xFFE3;
    private const uint XK_Control_R = 0xFFE4;
    private const uint XK_Alt_L = 0xFFE9;
    private const uint XK_Alt_R = 0xFFEA;
    private const uint XK_Delete = 0xFFFF;

    private const uint XK_space = 0x0020;
    private const uint XK_0 = 0x0030;
    private const uint XK_9 = 0x0039;
    private const uint XK_A = 0x0041;
    private const uint XK_Z = 0x005A;
    private const uint XK_a = 0x0061;
    private const uint XK_z = 0x007A;

    public static Key Map(uint keysym) => keysym switch
    {
        XK_Left => Key.Left,
        XK_Right => Key.Right,
        XK_Up => Key.Up,
        XK_Down => Key.Down,
        XK_Escape => Key.Escape,
        XK_space => Key.Space,
        XK_Return or XK_KP_Enter => Key.Enter,
        XK_Tab => Key.Tab,
        XK_BackSpace => Key.Backspace,
        XK_Delete => Key.Delete,
        XK_Home => Key.Home,
        XK_End => Key.End,
        XK_Prior => Key.PageUp,
        XK_Next => Key.PageDown,
        // X11 reports left and right modifiers as distinct keysyms; Key has one of each, matching
        // the Win32 arm where VK_SHIFT/VK_CONTROL/VK_MENU are already side-agnostic.
        XK_Shift_L or XK_Shift_R => Key.Shift,
        XK_Control_L or XK_Control_R => Key.Control,
        XK_Alt_L or XK_Alt_R => Key.Alt,
        // F1..F12 are contiguous in keysymdef.h, as F1..F12 are in Key.
        >= XK_F1 and <= XK_F12 => (Key)((int)Key.F1 + (int)(keysym - XK_F1)),
        // Both letter cases fold к the same Key: one physical key, two keysyms by shift level.
        >= XK_A and <= XK_Z => (Key)((int)Key.A + (int)(keysym - XK_A)),
        >= XK_a and <= XK_z => (Key)((int)Key.A + (int)(keysym - XK_a)),
        >= XK_0 and <= XK_9 => (Key)((int)Key.Digit0 + (int)(keysym - XK_0)),
        _ => Key.Unknown,
    };
}
