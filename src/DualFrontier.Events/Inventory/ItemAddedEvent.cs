using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Inventory
{
    /// <summary>
    /// Published by InventorySystem when an item stack is added
    /// to a storage entity. UI and log systems subscribe to this.
    /// </summary>
    public sealed record ItemAddedEvent : IEvent
    {
        /// <summary>Storage entity that received the item.</summary>
        public required EntityId StorageId { get; init; }

        /// <summary>Item template identifier.</summary>
        public required string ItemId { get; init; }

        /// <summary>Quantity added.</summary>
        public required int Quantity { get; init; }
    }
}