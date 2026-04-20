using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Shared;

/// <summary>
/// Здоровье entity. Pure data — никакой логики.
/// Модификация только через DamageSystem / HealSystem.
/// </summary>
public sealed class HealthComponent : IComponent
{
    // TODO: public float Current;
    // TODO: public float Maximum;
    // TODO: public bool IsDead => Current <= 0;
}
