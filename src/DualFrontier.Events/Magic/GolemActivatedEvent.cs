using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Маг активировал своего голема. С этого момента PowerSystem начинает
/// списывать ману у мага-хозяина на содержание голема (GDD 5.3 «Экономика
/// Манны»). При истощении мага публикуется событие деактивации (Фаза 5).
/// </summary>
public sealed record GolemActivatedEvent : IEvent
{
    // TODO: public required EntityId GolemId { get; init; }
    // TODO: public required EntityId OwnerId { get; init; }
    // TODO: public required int GolemTier { get; init; }  // 1..5 — GDD 5.1
}
