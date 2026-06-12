namespace DualFrontier.Application.Display;

/// <summary>
/// Per-frame render context handed to every <see cref="Layer.Render"/>
/// invocation by <see cref="CompositionFramework.RenderFrame"/>.
///
/// К10.3 v2 К-L17 lands the minimal abstraction (delta + frame index) that
/// concrete presentation backends extend with их own rendering surfaces
/// (e.g. a Vulkan command buffer). Application layer stays
/// substrate-agnostic per S-LOCK-11 — composition operates above renderer
/// abstraction, не extending <see cref="Rendering.IRenderer"/>.
/// </summary>
public interface ILayerRenderContext
{
    /// <summary>Wall-clock time since the previous display frame.</summary>
    double DeltaSeconds { get; }

    /// <summary>
    /// Monotonically increasing display frame index. Used by ring-buffered
    /// resources (vertex rings, descriptor pools) per V0.C.2 precedent.
    /// </summary>
    uint FrameIndex { get; }
}
