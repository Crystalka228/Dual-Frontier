using System;
using DualFrontier.Contracts.Display;

namespace DualFrontier.Application.Display;

/// <summary>
/// К-L17 default SimState layer (К10.3 v2 Item 38).
///
/// Wraps the existing V0.C.2 batched sprite rendering path (or any future
/// SimState rendering pipeline) as the default <see cref="LayerType.SimState"/>
/// layer. Concrete presentation backends construct this с a render delegate
/// that drives their substrate-specific pipeline (Vulkan sprite renderer +
/// Camera2D + TileMap in <c>DualFrontier.Presentation.Native</c>; Godot canvas
/// в <c>DualFrontier.Presentation</c>).
///
/// Per К10.3 v2 §1 strategy «managed-facade-preserved + Lesson #20 scope
/// discipline»: К10.3 v2 lands the architectural commitment (К-L17 invariant
/// + composition framework + default SimState layer slot). Full integration
/// of the V substrate sprite path как SimState layer's render delegate lands
/// в presentation-backend cycles либо К-extensions per managed-facade-preserved
/// strategy — <c>SimStateLayer</c> sits ready to receive that delegate без
/// further К10.3 v2 changes.
///
/// Per К-L7/К-L7.1 coexistence (S-LOCK-10): the render delegate reads from
/// either current sim state (К-L7 sync default, V1 pattern) или pipeline slot
/// tail (К-L7.1 opt-in, К-L16 pipeline-managed). The choice belongs к the
/// concrete backend, не к this layer slot.
/// </summary>
public class SimStateLayer : Layer
{
    /// <summary>Default FQN для the vanilla SimState layer slot.</summary>
    public const string DefaultFqn = "DualFrontier.Application.Display.SimStateLayer";

    private readonly string _fqn;
    private readonly int _compositionOrder;
    private readonly Action<ILayerRenderContext>? _renderDelegate;

    /// <summary>
    /// Constructs a SimState layer slot. Pass <paramref name="renderDelegate"/>
    /// = <see langword="null"/> для a no-op placeholder (К10.3 v2 default
    /// before integration); pass a delegate driving the existing V0.C.2 sprite
    /// pipeline for full operational integration.
    /// </summary>
    /// <param name="fqn">Layer FQN; defaults to <see cref="DefaultFqn"/>.</param>
    /// <param name="compositionOrder">Secondary order within SimState tier.</param>
    /// <param name="renderDelegate">Optional render delegate; null = no-op.</param>
    public SimStateLayer(
        string? fqn = null,
        int compositionOrder = 0,
        Action<ILayerRenderContext>? renderDelegate = null)
    {
        _fqn = fqn ?? DefaultFqn;
        _compositionOrder = compositionOrder;
        _renderDelegate = renderDelegate;
    }

    /// <inheritdoc/>
    public override LayerType Type => LayerType.SimState;

    /// <inheritdoc/>
    public override string Fqn => _fqn;

    /// <inheritdoc/>
    public override int CompositionOrder => _compositionOrder;

    /// <inheritdoc/>
    public override void Render(ILayerRenderContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        _renderDelegate?.Invoke(context);
    }
}
