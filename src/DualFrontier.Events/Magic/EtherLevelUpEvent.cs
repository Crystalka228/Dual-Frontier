using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Уровень восприятия эфира у мага повышен (см. GDD 4.1, 4.2).
/// Помечено <see cref="DeferredAttribute"/>: повышение уровня меняет
/// максимум маны, регенерацию, доступные школы — это производные значения,
/// пересчёт должен идти в следующей фазе, чтобы не конфликтовать с
/// одновременной правкой <c>ManaComponent</c> (см. DualFrontier.Components.Magic).
/// </summary>
[Deferred]
public sealed record EtherLevelUpEvent : IEvent
{
    // TODO: public required EntityId MageId { get; init; }
    // TODO: public required int OldLevel { get; init; }
    // TODO: public required int NewLevel { get; init; }  // 1..5 — см. EtherComponent
}
