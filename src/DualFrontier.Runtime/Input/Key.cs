namespace DualFrontier.Runtime.Input;

/// <summary>
/// Keyboard key enum per S-LOCK-10. V0.C.1 covers commonly-used keys (arrows, modifiers,
/// special keys, function keys F1-F12, letters A-Z, digits 0-9). Mapping from Win32 virtual
/// key codes happens в <see cref="VirtualKeyMapper"/>. Keys not в this enum produce
/// <see cref="Key.Unknown"/> events.
/// </summary>
public enum Key
{
    Unknown,

    // Arrow keys
    Left, Right, Up, Down,

    // Modifier keys
    Shift, Control, Alt,

    // Special keys
    Escape, Space, Enter, Tab, Backspace, Delete, Home, End, PageUp, PageDown,

    // Function keys
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

    // Letter keys (simplified — caller normalizes case if needed)
    A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

    // Digit keys
    Digit0, Digit1, Digit2, Digit3, Digit4, Digit5, Digit6, Digit7, Digit8, Digit9,
}
