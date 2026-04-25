using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
namespace DualFrontier.Events.Inventory
{
    /// <summary>
    /// Published when a haul or craft removes an item stack from a storage.
    /// Marked <see cref="DeferredAttribute"/>: the actual mutation of
    /// <c>StorageComponent</c> happens on the next phase boundary inside
    /// <c>InventorySystem</c>'s captured execution context — same rationale as
    /// <see cref="ItemAddedEvent"/>.
    /// </summary>
    [Deferred]
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
