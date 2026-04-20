using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Inventory;

/// <summary>
/// Запрос на крафт. Публикуется игроком (через UI) или AI-приоритизатором.
/// Это **заявка**, а не начало работы: JobSystem назначает подходящую
/// пешку и верстак, InventorySystem резервирует компоненты (<see cref="ItemReservedEvent"/>).
/// </summary>
public sealed record CraftRequestEvent : IEvent
{
    // TODO: public required string RecipeId { get; init; } = string.Empty;
    // TODO: public EntityId? RequesterId { get; init; }   // кто заказал (для UI/истории)
    // TODO: public int Count { get; init; } = 1;
    // TODO: public int Priority { get; init; }            // очередь приоритетов
}
