using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Сущность умерла (HP ≤ 0). Помечено <see cref="DeferredAttribute"/>:
/// удаление entity и связанная очистка компонентов не должны прерывать
/// текущую фазу — другие системы этой же фазы могут ещё ссылаться на
/// entity (например, статистика, графика, аудио).
///
/// Эффекты смерти (реакции пешек — см. <c>DeathReactionEvent</c>, раздача
/// лута и т.п.) подписываются уже на deferred-доставку.
/// </summary>
[Deferred]
public sealed record DeathEvent : IEvent
{
    // TODO: public required EntityId VictimId { get; init; }
    // TODO: public EntityId? KillerId { get; init; }
    // TODO: public DamageType LastDamageType { get; init; }
}
