namespace DualFrontier.Runtime.Input;

/// <summary>
/// Window resize event emitted by Window.WindowProcedure on WM_SIZE messages с non-zero
/// dimensions. Consumed by swapchain code (V0.B Commit 7+) to trigger swapchain recreation.
/// </summary>
public sealed record WindowResizeEvent(int NewWidth, int NewHeight) : IInputEvent;
