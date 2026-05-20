using System;

namespace DualFrontier.Application.Display;

/// <summary>
/// Abstract base for every К-L17 display composition layer (К10.3 v2 Item 38).
///
/// Layer authors override <see cref="Render"/> with substrate-specific
/// rendering calls; the <see cref="Type"/>, <see cref="Fqn"/>, и
/// <see cref="CompositionOrder"/> properties drive registration и composition
/// ordering in <see cref="CompositionFramework"/>.
///
/// Per К10.3 v2 §1.5 (S-LOCK-11): layers live в Application/Display/ above
/// the renderer abstraction. Concrete backends provide the rendering surface
/// via <see cref="ILayerRenderContext"/>.
/// </summary>
public abstract class Layer
{
    /// <summary>К-L17 latency tier — drives primary composition order.</summary>
    public abstract LayerType Type { get; }

    /// <summary>
    /// Fully-qualified name used for capability tokens (К10.3 v2 Items 39+40
    /// extend MOD_OS §3.2 с <c>kernel.layer.intent:{FQN}</c> и
    /// <c>kernel.layer.combat_feedback:{FQN}</c>) и dictionary keying в
    /// <see cref="CompositionFramework"/>.
    /// </summary>
    public abstract string Fqn { get; }

    /// <summary>
    /// Secondary composition order within the same <see cref="LayerType"/>.
    /// Lower values rendered first; default 0. Layers with identical
    /// <see cref="CompositionOrder"/> render в deterministic FQN order per
    /// К-L17 «composition order: sim state first, overlays composited on top».
    /// </summary>
    public virtual int CompositionOrder => 0;

    /// <summary>
    /// Renders this layer for the current display frame. К-L17 latency
    /// contract for the layer's <see cref="Type"/> applies — implementers
    /// must respect bounds per <see cref="LayerType"/> documentation.
    /// </summary>
    /// <param name="context">Per-frame render context.</param>
    public abstract void Render(ILayerRenderContext context);

    /// <summary>
    /// Optional disposal hook called when the layer is unregistered (e.g.
    /// during К-L18 mod unload, К10.3 v2 Item 42). Default no-op.
    /// </summary>
    public virtual void OnUnregistered() { }
}
