namespace DualFrontier.Runtime.Input;

/// <summary>Keyboard key press event. Emitted on WM_KEYDOWN / WM_SYSKEYDOWN.</summary>
public sealed record KeyPressedEvent(Key Key) : IInputEvent;
