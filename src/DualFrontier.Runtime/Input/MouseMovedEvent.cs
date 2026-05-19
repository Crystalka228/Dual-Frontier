namespace DualFrontier.Runtime.Input;

/// <summary>Mouse move event. Coordinates in client area pixel space. Emitted on WM_MOUSEMOVE.</summary>
public sealed record MouseMovedEvent(int X, int Y) : IInputEvent;
