using System;
using DualFrontier.Contracts.Display;

namespace DualFrontier.Application.Display;

/// <summary>
/// К-L17 intent overlay layer base (К10.3 v2 Item 39).
///
/// Renders cursor / hover / drag-and-drop / construction-placement preview
/// reading directly от current input state — bypasses pipeline slot tail.
/// Latency contract: ≤16ms (1 display frame at 60 FPS) per К-L17 invariant.
///
/// Concrete consumers (mod-registered или vanilla) inject input-state reader
/// и rendering delegate via constructor; this base ensures the layer plugs
/// в the К-L17 composition order at <see cref="LayerType.Intent"/> tier.
///
/// К10.3 v2 boundary: layer slot established с the latency contract; concrete
/// input event queue integration lives в presentation backends per К10.3 v2
/// §1.5 S-LOCK-11 — Application layer stays substrate-agnostic. (The era
/// examples — Silk.NET input poll, Godot Input singleton — were retired with
/// К-extensions cascade #2; the Launcher Win32 InputQueue is the current
/// backend seam.)
/// </summary>
public class IntentOverlayLayer : Layer
{
    /// <summary>Default FQN для the vanilla intent overlay slot.</summary>
    public const string DefaultFqn = "DualFrontier.Application.Display.IntentOverlayLayer";

    private readonly string _fqn;
    private readonly int _compositionOrder;
    private readonly Action<ILayerRenderContext>? _renderDelegate;

    /// <summary>
    /// Constructs an intent overlay layer slot. Pass <paramref name="renderDelegate"/>
    /// = <see langword="null"/> для a no-op placeholder; pass а delegate
    /// reading current input state for full operational integration.
    /// </summary>
    public IntentOverlayLayer(
        string? fqn = null,
        int compositionOrder = 0,
        Action<ILayerRenderContext>? renderDelegate = null)
    {
        _fqn = fqn ?? DefaultFqn;
        _compositionOrder = compositionOrder;
        _renderDelegate = renderDelegate;
    }

    /// <inheritdoc/>
    public override LayerType Type => LayerType.Intent;

    /// <inheritdoc/>
    public override string Fqn => _fqn;

    /// <inheritdoc/>
    public override int CompositionOrder => _compositionOrder;

    /// <inheritdoc/>
    public override void Render(ILayerRenderContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        // К-L17 contract: read current input state, не pipeline slot tail.
        // Concrete backend's renderDelegate reads from InputEventQueue (V0.C.1
        // surface) at display tick time.
        _renderDelegate?.Invoke(context);
    }
}
