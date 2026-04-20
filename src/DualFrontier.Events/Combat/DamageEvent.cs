using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Нанесение урона. Публикуется CombatSystem после расчёта попадания
/// и пробития. DamageSystem применяет урон: сначала к щиту (GDD 6.4),
/// затем к броне, затем к HP.
/// </summary>
public sealed record DamageEvent : IEvent
{
    // TODO: public required EntityId AttackerId { get; init; }
    // TODO: public required EntityId TargetId { get; init; }
    // TODO: public required float Amount { get; init; }
    // TODO: public required DamageType Type { get; init; }
    // TODO: public required float Penetration { get; init; }
}
