using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Combat;

/// <summary>
/// Magical shield. Defence layer 2 (GDD 6.4 "Magical Shields"):
/// the <c>HpPool</c> absorbs damage first and recovers at <c>Regen</c>
/// (units/tick). Shield kind (<c>Kind</c>) determines absorption modifiers
/// for different <c>DamageType</c>s.
/// </summary>
[ModAccessible(Read = true)]
public sealed class ShieldComponent : IComponent
{
    // TODO: public float HpPool;
    // TODO: public float Regen;
    // TODO: introduce DualFrontier.Components.Combat.ShieldKind enum (Arcane, Kinetic, Void …) — GDD 6.4, Phase 6.
    // TODO: public ShieldKind Kind;
}
