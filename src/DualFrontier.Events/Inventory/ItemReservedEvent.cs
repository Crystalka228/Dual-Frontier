using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Inventory;

/// <summary>
/// Предмет зарезервирован под работу (крафт, постройку, перенос).
/// Пока резерв активен — другой haul/craft-запрос этот предмет не возьмёт.
/// Снятие резерва — по событию завершения работы (`ItemRemovedEvent` или
/// отмене).
/// </summary>
public sealed record ItemReservedEvent : IEvent
{
    // TODO: public required EntityId ItemId { get; init; }
    // TODO: public required EntityId ReservedBy { get; init; }     // pawn / job
    // TODO: public required string Purpose { get; init; } = string.Empty;
}
