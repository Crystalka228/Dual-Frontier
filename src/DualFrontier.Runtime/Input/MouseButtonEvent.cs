namespace DualFrontier.Runtime.Input;

/// <summary>
/// Mouse button press / release event. <see cref="Pressed"/> = true on WM_*BUTTONDOWN,
/// false on WM_*BUTTONUP.
/// </summary>
public sealed record MouseButtonEvent(MouseButton Button, bool Pressed) : IInputEvent;
