using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Shared;

/// <summary>
/// Entity health. Pure data — no logic.
/// Mutated only via DamageSystem / HealSystem.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct HealthComponent : IComponent
{
    public float Current;
    public float Maximum;
    public bool IsDead => Current <= 0;
}
