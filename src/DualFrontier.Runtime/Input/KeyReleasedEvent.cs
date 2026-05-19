namespace DualFrontier.Runtime.Input;

/// <summary>Keyboard key release event. Emitted on WM_KEYUP / WM_SYSKEYUP.</summary>
public sealed record KeyReleasedEvent(Key Key) : IInputEvent;
