namespace DualFrontier.Application.Display;

/// <summary>
/// К-L17 display composition taxonomy (К10.3 v2 Item 38).
///
/// Layer type drives composition order in <see cref="CompositionFramework"/> и
/// declares the latency contract that the layer's <see cref="Layer.Render"/>
/// implementation must respect.
///
/// Per KERNEL_ARCHITECTURE.md К-L17 row: «sim state + intent overlay + combat
/// feedback layers с independent latency contracts; composition order: sim
/// state first, overlays composited on top».
/// </summary>
public enum LayerType
{
    /// <summary>
    /// Pipeline slot tail (К-L16) for pipeline-managed display state, либо
    /// current sim state for К-L7 sync default (V1 path). Latency =
    /// <c>D × tick_period</c> for pipeline-managed; sub-tick for К-L7 sync.
    /// Rendered first per К-L17 composition order.
    /// </summary>
    SimState = 0,

    /// <summary>
    /// Sub-pipeline-latency overlay reading current input state. Latency
    /// ≤16ms (1 display frame at 60 FPS) per К-L17 invariant. Composited
    /// on top of <see cref="SimState"/>.
    /// </summary>
    Intent = 1,

    /// <summary>
    /// Fast tier consumer (К-L15) overlay — damage numbers, hit sparks,
    /// weapon glints. Latency ≤1ms К-L15 + ≤16ms display ≈ ≤17ms event-к-
    /// visible per К-L17 invariant. Composited on top of <see cref="SimState"/>
    /// (alongside <see cref="Intent"/>).
    /// </summary>
    CombatFeedback = 2,

    /// <summary>
    /// Loaded static assets — backgrounds, UI chrome. No latency mandate.
    /// Rendered last per К-L17 composition order (above overlays).
    /// </summary>
    Static = 3,
}
