using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Inventory
{
    /// <summary>
    /// Published by <c>HaulSystem</c> when an item is selected for a specific
    /// task. Marked <see cref="DeferredAttribute"/>: the reservation is
    /// recorded by <c>InventorySystem</c> on the next phase boundary so that
    /// future Phase 5+ multi-tick haul logic can consult the reservation
    /// table before picking a stack.
    /// <para>
    /// Phase 4 same-tick double-allocation (two idle pawns selecting the
    /// same stack within a single <c>HaulSystem.Update</c> call) is closed by
    /// HaulSystem's own per-Update reservation set, since the deferred event
    /// only reaches InventorySystem at the next phase boundary.
    /// </para>
    /// </summary>
    [Deferred]
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
