using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
namespace DualFrontier.Events.Inventory
{
    /// <summary>
    /// Published when a haul or craft adds an item stack to a storage.
    /// Marked <see cref="DeferredAttribute"/>: the actual mutation of
    /// <c>StorageComponent</c> happens on the next phase boundary inside
    /// <c>InventorySystem</c>'s captured execution context — that keeps the
    /// publisher (e.g. <c>HaulSystem</c>) outside StorageComponent's writers
    /// while still letting <c>InventorySystem</c> apply the change without
    /// breaking isolation.
    /// </summary>
    [Deferred]
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
