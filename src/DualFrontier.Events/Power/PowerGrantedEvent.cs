using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Power;

/// <summary>
/// PowerSystem подтверждает выдачу запрошенной мощности. Потребитель
/// может работать в текущем тике. Отсутствие этого события в тике =
/// постройка не получила энергию.
/// </summary>
public sealed record PowerGrantedEvent : IEvent
{
    // TODO: public required EntityId ConsumerId { get; init; }
    // TODO: public required PowerType Type { get; init; }
    // TODO: public required float Watts { get; init; }
}
