namespace DualFrontier.Application.Bridge;

/// <summary>
/// Marker interface для render commands enqueued onto the
/// <see cref="PresentationBridge"/>. Commands are pure immutable data records
/// carrying the information needed для visual effects. Dispatch is handled
/// by the active renderer (e.g. Launcher's <c>RenderCommandDispatcher</c>),
/// не by the command itself — per Lesson #25 refined: empty <c>Execute()</c>
/// bodies were lying-test surface, structurally eliminated в К-extensions
/// cascade #2 (2026-05-23).
/// </summary>
public interface IRenderCommand
{
}
