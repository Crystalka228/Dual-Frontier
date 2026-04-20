using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Power;

/// <summary>
/// Постройка запрашивает мощность на текущий тик. PowerSystem агрегирует
/// все запросы и отвечает <see cref="PowerGrantedEvent"/> при успехе,
/// либо публикует <see cref="GridOverloadEvent"/> при дефиците.
/// </summary>
public sealed record PowerRequestEvent : IEvent
{
    // TODO: public required EntityId ConsumerId { get; init; }
    // TODO: public required PowerType Type { get; init; }  // enum — см. Components/Building
    // TODO: public required float Watts { get; init; }
    // TODO: public int Priority { get; init; }             // для отключения при перегрузке
}
