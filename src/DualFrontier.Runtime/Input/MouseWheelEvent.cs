namespace DualFrontier.Runtime.Input;

/// <summary>
/// Mouse wheel scroll event. <see cref="Delta"/> is normalized to ±1 per notch (positive =
/// scroll up, negative = scroll down). Each backend normalizes from its own units: Win32
/// divides the WM_MOUSEWHEEL delta by WHEEL_DELTA (120); XCB reports notches as press/release
/// pairs on buttons 4 and 5, and publishes on the press only.
/// </summary>
public sealed record MouseWheelEvent(int Delta) : IInputEvent;
