using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Пешка пытается произвести выстрел. Публикуется AI или игроком.
/// CombatSystem проверяет наличие оружия/патронов и публикует
/// соответствующий <see cref="AmmoIntent"/>.
/// </summary>
public sealed record ShootAttemptEvent : IEvent
{
    // TODO: public required EntityId AttackerId { get; init; }
    // TODO: public required EntityId TargetId { get; init; }
}
