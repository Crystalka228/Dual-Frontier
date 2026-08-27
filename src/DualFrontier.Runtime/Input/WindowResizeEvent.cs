namespace DualFrontier.Runtime.Input;

/// <summary>
/// Window resize event emitted by the windowing backend when the client area changes to
/// non-zero, unchanged-rejecting dimensions (Win32 WM_SIZE / XCB CONFIGURE_NOTIFY — the shared
/// law is in <c>Window.WindowEventDecode</c>).
///
/// <para>Published for parity across backends, but it has no consumer: the Launcher drains the
/// input queue and discards every event. Swapchain recreation is driven by out-of-date acquire
/// and present results reading <c>IWindow.Width</c>/<c>Height</c>, which the pump keeps current
/// — those properties, not this event, are the load-bearing resize output.</para>
/// </summary>
public sealed record WindowResizeEvent(int NewWidth, int NewHeight) : IInputEvent;
