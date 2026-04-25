using DualFrontier.Contracts.Core;
namespace DualFrontier.Events.Inventory
{
    /// <summary>
    /// Published by InventorySystem when an item stack is removed
    /// from a storage entity.
    /// </summary>
    public sealed record ItemRemovedEvent : IEvent
    {
        /// <summary>Storage entity that lost the item.</summary>
        public required EntityId StorageId { get; init; }

        /// <summary>Item template identifier.</summary>
        public required string ItemId { get; init; }

        /// <summary>Quantity removed.</summary>
        public required int Quantity { get; init; }
    }
}