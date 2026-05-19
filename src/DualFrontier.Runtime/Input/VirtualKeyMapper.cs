using DualFrontier.Runtime.Native.Win32;

namespace DualFrontier.Runtime.Input;

/// <summary>
/// Maps Win32 virtual key codes к <see cref="Key"/> enum values. Codes outside the supported
/// set return <see cref="Key.Unknown"/>. Per S-LOCK-10: covers arrow keys, modifiers, special
/// keys, function keys F1-F12, letters A-Z, digits 0-9.
/// </summary>
public static class VirtualKeyMapper
{
    public static Key Map(int vkCode) => vkCode switch
    {
        Win32Constants.VK_LEFT => Key.Left,
        Win32Constants.VK_RIGHT => Key.Right,
        Win32Constants.VK_UP => Key.Up,
        Win32Constants.VK_DOWN => Key.Down,
        Win32Constants.VK_ESCAPE => Key.Escape,
        Win32Constants.VK_SPACE => Key.Space,
        Win32Constants.VK_RETURN => Key.Enter,
        Win32Constants.VK_TAB => Key.Tab,
        Win32Constants.VK_BACK => Key.Backspace,
        Win32Constants.VK_DELETE => Key.Delete,
        Win32Constants.VK_HOME => Key.Home,
        Win32Constants.VK_END => Key.End,
        Win32Constants.VK_PRIOR => Key.PageUp,
        Win32Constants.VK_NEXT => Key.PageDown,
        Win32Constants.VK_SHIFT => Key.Shift,
        Win32Constants.VK_CONTROL => Key.Control,
        Win32Constants.VK_MENU => Key.Alt,
        Win32Constants.VK_F1 => Key.F1,
        Win32Constants.VK_F2 => Key.F2,
        Win32Constants.VK_F3 => Key.F3,
        Win32Constants.VK_F4 => Key.F4,
        Win32Constants.VK_F5 => Key.F5,
        Win32Constants.VK_F6 => Key.F6,
        Win32Constants.VK_F7 => Key.F7,
        Win32Constants.VK_F8 => Key.F8,
        Win32Constants.VK_F9 => Key.F9,
        Win32Constants.VK_F10 => Key.F10,
        Win32Constants.VK_F11 => Key.F11,
        Win32Constants.VK_F12 => Key.F12,
        // Letter keys: VK_A = 0x41 .. VK_Z = 0x5A (ASCII uppercase).
        >= 0x41 and <= 0x5A => (Key)((int)Key.A + (vkCode - 0x41)),
        // Digit keys: VK_0 = 0x30 .. VK_9 = 0x39.
        >= 0x30 and <= 0x39 => (Key)((int)Key.Digit0 + (vkCode - 0x30)),
        _ => Key.Unknown,
    };
}
