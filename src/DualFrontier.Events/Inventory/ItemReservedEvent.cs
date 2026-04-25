using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Inventory
{
    /// <summary>
    /// Published by InventorySystem when an item is reserved
    /// for a specific task. Prevents double-allocation:
    /// while reserved, HaulSystem will not pick this item.
    /// </summary>
    public sealed record ItemReservedEvent : IEvent
    {
        /// <summary>Storage entity holding the reserved item.</summary>
        public required EntityId StorageId { get; init; }

        /// <summary>Item template identifier.</summary>
        public required string ItemId { get; init; }

        /// <summary>Quantity reserved.</summary>
        public required int Quantity { get; init; }

        /// <summary>Entity that reserved the item (pawn or building).</summary>
        public required EntityId ReservedBy { get; init; }
    }
}