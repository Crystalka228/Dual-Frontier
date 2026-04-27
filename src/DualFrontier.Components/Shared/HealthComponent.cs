using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Shared;

/// <summary>
/// Entity health. Pure data — no logic.
/// Mutated only via DamageSystem / HealSystem.
/// </summary>
public sealed class HealthComponent : IComponent
{
    // TODO: public float Current;
    // TODO: public float Maximum;
    // TODO: public bool IsDead => Current <= 0;
}
