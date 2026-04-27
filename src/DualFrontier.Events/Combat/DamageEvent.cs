using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Damage application. Published by CombatSystem after hit and penetration
/// calculations. DamageSystem applies damage: first to the shield (GDD 6.4),
/// then to armour, then to HP.
/// </summary>
public sealed record DamageEvent : IEvent
{
    // TODO: public required EntityId AttackerId { get; init; }
    // TODO: public required EntityId TargetId { get; init; }
    // TODO: public required float Amount { get; init; }
    // TODO: public required DamageType Type { get; init; }
    // TODO: public required float Penetration { get; init; }
}
