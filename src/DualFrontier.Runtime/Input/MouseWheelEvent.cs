namespace DualFrontier.Runtime.Input;

/// <summary>
/// Mouse wheel scroll event. <see cref="Delta"/> is in WHEEL_DELTA units (positive = scroll up,
/// negative = scroll down); normalized to ±1 per notch via division by Win32 WHEEL_DELTA (120).
/// </summary>
public sealed record MouseWheelEvent(int Delta) : IInputEvent;
