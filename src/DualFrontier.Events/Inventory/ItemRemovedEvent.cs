using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Inventory;

/// <summary>
/// Предмет изъят из хранилища или выпал из рук пешки. Подписчики: UI,
/// статистика, анимация дропа.
/// </summary>
public sealed record ItemRemovedEvent : IEvent
{
    // TODO: public required EntityId ItemId { get; init; }
    // TODO: public required EntityId ContainerId { get; init; }
    // TODO: public int Count { get; init; } = 1;
}
