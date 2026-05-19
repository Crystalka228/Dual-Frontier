namespace DualFrontier.Runtime.Input;

/// <summary>
/// Window focus change event. <see cref="Focused"/> = true on WM_SETFOCUS, false on WM_KILLFOCUS.
/// V0.C.2/V0.D will couple WindowFocusEvent(Focused: false) к simulation loop pause per
/// VULKAN_SUBSTRATE §2.3 threading model — V0.C.1 lands только event types.
/// </summary>
public sealed record WindowFocusEvent(bool Focused) : IInputEvent;
