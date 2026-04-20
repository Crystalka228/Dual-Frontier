using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Пешка заметила смерть сородича и эмоционально на это отреагировала.
/// Публикуется MoodSystem при обработке deferred <c>DeathEvent</c>:
/// добавляет мудовый штраф, может триггерить <see cref="MoodBreakEvent"/>.
/// </summary>
public sealed record DeathReactionEvent : IEvent
{
    // TODO: public required EntityId ObserverId { get; init; }
    // TODO: public required EntityId VictimId { get; init; }
    // TODO: public required int Relationship { get; init; }  // из SocialComponent — для силы эффекта
}
