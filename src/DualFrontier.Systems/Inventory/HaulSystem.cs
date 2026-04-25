using System;
using DualFrontier.Components.Building;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Inventory;

namespace DualFrontier.Systems.Inventory;

/// <summary>
/// Phase 4 haul: for each idle pawn, finds a storage with items and a
/// storage with free space, and "teleports" one stack between them via
/// Reserve/Remove/Add events. No pathfinding or per-tick travel yet —
/// real multi-tick hauling arrives in a later phase.
/// Phase: 4. Tick: NORMAL.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(StorageComponent), typeof(PositionComponent), typeof(JobComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Inventory)
)]
[TickRate(TickRates.NORMAL)]
public sealed class HaulSystem : SystemBase
{
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        foreach (var pawn in Query<JobComponent>())
        {
            var job = GetComponent<JobComponent>(pawn);
            if (job.Current != JobKind.Idle) continue;

            if (!TryFindHaul(out var sourceId, out var destId, out var itemId, out var quantity))
                return;

            Services.Inventory.Publish(new ItemReservedEvent
            {
                StorageId  = sourceId,
                ItemId     = itemId,
                Quantity   = quantity,
                ReservedBy = pawn
            });

            Services.Inventory.Publish(new ItemRemovedEvent
            {
                StorageId = sourceId,
                ItemId    = itemId,
                Quantity  = quantity
            });

            Services.Inventory.Publish(new ItemAddedEvent
            {
                StorageId = destId,
                ItemId    = itemId,
                Quantity  = quantity
            });
        }
    }

    private bool TryFindHaul(
        out EntityId sourceId,
        out EntityId destId,
        out string itemId,
        out int quantity)
    {
        sourceId = default;
        destId   = default;
        itemId   = string.Empty;
        quantity = 0;

        EntityId src      = default;
        string   srcItem  = string.Empty;
        int      srcQty   = 0;
        bool     srcFound = false;

        foreach (var storage in Query<StorageComponent>())
        {
            var s = GetComponent<StorageComponent>(storage);
            if (s.Items.Count == 0) continue;
            foreach (var kv in s.Items)
            {
                src      = storage;
                srcItem  = kv.Key;
                srcQty   = kv.Value;
                srcFound = true;
                break;
            }
            if (srcFound) break;
        }

        if (!srcFound) return false;

        EntityId dst      = default;
        bool     dstFound = false;
        foreach (var storage in Query<StorageComponent>())
        {
            if (storage.Equals(src)) continue;
            var s = GetComponent<StorageComponent>(storage);
            if (s.IsFull) continue;
            dst      = storage;
            dstFound = true;
            break;
        }

        if (!dstFound) return false;

        sourceId = src;
        destId   = dst;
        itemId   = srcItem;
        quantity = srcQty;
        return true;
    }
}
