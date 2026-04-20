using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Inventory;

/// <summary>
/// Предмет положен в хранилище или в руки пешке. Подписчики: UI, сигналы,
/// статистика, save/load.
/// </summary>
public sealed record ItemAddedEvent : IEvent
{
    // TODO: public required EntityId ItemId { get; init; }
    // TODO: public required EntityId ContainerId { get; init; }
    // TODO: public int Count { get; init; } = 1;  // для stackable предметов
}
