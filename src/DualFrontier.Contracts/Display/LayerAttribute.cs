using System;

namespace DualFrontier.Contracts.Display;

/// <summary>
/// К-L17 layer declaration attribute (К10.3 v2 Items 39+40).
///
/// Applied к concrete layer types в mod assemblies (and vanilla layer classes
/// в Application) к declare which К-L17 tier the layer belongs к. Read via
/// reflection by <c>KernelCapabilityRegistry</c> к emit capability tokens
/// (<c>kernel.layer.intent:{FQN}</c>, <c>kernel.layer.combat_feedback:{FQN}</c>)
/// per S3-Q5 + S8-Q3 granular FQN pattern.
///
/// Mismatch between this attribute's <see cref="LayerType"/> и the layer
/// instance's <c>Type</c> property surfaces as <c>LayerCapabilityMismatch</c>
/// validation error (К10.3 v2 MOD_OS §11.2 amendment).
///
/// SimState и Static tiers use existing renderer-level capabilities (V
/// substrate primitives) — typically не declared via <see cref="LayerAttribute"/>
/// в mod assemblies. Intent and CombatFeedback are the primary tiers где
/// modders register layers per Item 39/40 spec.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class LayerAttribute : Attribute
{
    /// <summary>К-L17 latency tier для this layer.</summary>
    public LayerType LayerType { get; }

    /// <summary>
    /// Secondary composition order within the same <see cref="LayerType"/>.
    /// Lower values rendered first. Defaults к 0.
    /// </summary>
    public int CompositionOrder { get; init; } = 0;

    public LayerAttribute(LayerType layerType)
    {
        LayerType = layerType;
    }
}
