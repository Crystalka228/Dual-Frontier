using System;
using System.Collections.Generic;
using DualFrontier.Components.Building;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Inventory;

namespace DualFrontier.Systems.Inventory;

/// <summary>
/// Phase 4 haul: for each idle pawn, finds a storage with items and a
/// storage with free space, and "teleports" one stack between them via
/// Reserve/Remove/Add events. No pathfinding or per-tick travel yet —
/// real multi-tick hauling arrives in a later phase.
/// <para>
/// Same-tick double-allocation is prevented by an in-call reservation set:
/// once a pawn picks a (storage, item) pair this tick, subsequent pawns in
/// the same <c>Update</c> skip it. The reservation set is local to a single
/// <c>Update</c> invocation — across ticks the persistent reservation table
/// lives in <c>InventorySystem</c>, populated via deferred
/// <see cref="ItemReservedEvent"/>.
/// </para>
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
    private readonly HashSet<(EntityId Storage, string ItemId)> _inCallReservations = new();

    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        _inCallReservations.Clear();

        using SpanLease<JobComponent> jobs = NativeWorld.AcquireSpan<JobComponent>();
        ReadOnlySpan<JobComponent> jobSpan = jobs.Span;
        ReadOnlySpan<int> jobIndices = jobs.Indices;

        for (int i = 0; i < jobs.Count; i++)
        {
            JobComponent job = jobSpan[i];
            if (job.Current != JobKind.Idle) continue;
            var pawn = new EntityId(jobIndices[i], 0);

            if (!TryFindHaul(out var sourceId, out var destId, out var itemId, out var quantity))
                continue;

            _inCallReservations.Add((sourceId, itemId));

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

        using (SpanLease<StorageComponent> storages = NativeWorld.AcquireSpan<StorageComponent>())
        {
            ReadOnlySpan<StorageComponent> storageSpan = storages.Span;
            ReadOnlySpan<int> storageIndices = storages.Indices;

            for (int s = 0; s < storages.Count; s++)
            {
                StorageComponent storage = storageSpan[s];
                if (!storage.Items.IsValid || storage.Items.Count == 0) continue;
                var storageId = new EntityId(storageIndices[s], 0);

                int count = storage.Items.Count;
                var keysBuf = new InternedString[count];
                var valuesBuf = new int[count];
                storage.Items.Iterate(keysBuf, valuesBuf);

                for (int i = 0; i < count; i++)
                {
                    string? resolved = keysBuf[i].Resolve(NativeWorld);
                    if (resolved is null) continue;
                    if (_inCallReservations.Contains((storageId, resolved)))
                        continue;
                    src      = storageId;
                    srcItem  = resolved;
                    srcQty   = valuesBuf[i];
                    srcFound = true;
                    break;
                }
                if (srcFound) break;
            }
        }

        if (!srcFound) return false;

        EntityId dst      = default;
        bool     dstFound = false;
        using (SpanLease<StorageComponent> storages = NativeWorld.AcquireSpan<StorageComponent>())
        {
            ReadOnlySpan<StorageComponent> storageSpan = storages.Span;
            ReadOnlySpan<int> storageIndices = storages.Indices;

            for (int s = 0; s < storages.Count; s++)
            {
                var storageId = new EntityId(storageIndices[s], 0);
                if (storageId.Equals(src)) continue;
                StorageComponent storage = storageSpan[s];
                if (!storage.Items.IsValid || storage.IsFull) continue;
                dst      = storageId;
                dstFound = true;
                break;
            }
        }

        if (!dstFound) return false;

        sourceId = src;
        destId   = dst;
        itemId   = srcItem;
        quantity = srcQty;
        return true;
    }
}
