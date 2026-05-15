using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Components.Building;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Inventory;

namespace DualFrontier.Systems.Inventory
{
    /// <summary>
    /// Manages item storage. Caches free storage slots to avoid full scan
    /// every tick. Cache invalidated on ItemAdded/Removed/Reserved.
    /// Single writer of <see cref="StorageComponent"/>.
    /// Phase: Phase 4. Tick: FAST.
    /// </summary>
    [SystemAccess(
        reads:  new Type[0],
        writes: new[] { typeof(StorageComponent) },
        bus:    nameof(IGameServices.Inventory)
    )]
    [TickRate(TickRates.FAST)]
    public sealed class InventorySystem : SystemBase
    {
        private readonly Dictionary<int, List<string>> _freeSlotCache = new();
        private readonly Dictionary<(EntityId Storage, string ItemId), int> _reservedQuantities = new();
        private bool _cacheDirty = true;

        internal IReadOnlyDictionary<(EntityId Storage, string ItemId), int> ReservedQuantities
            => _reservedQuantities;

        protected override void OnInitialize()
        {
            Services.Inventory.Subscribe<ItemAddedEvent>(OnItemAdded);
            Services.Inventory.Subscribe<ItemRemovedEvent>(OnItemRemoved);
            Services.Inventory.Subscribe<ItemReservedEvent>(OnItemReserved);
        }

        public override void Update(float delta)
        {
            if (_cacheDirty)
            {
                RebuildCache();
                _cacheDirty = false;
            }
        }

        private void OnItemAdded(ItemAddedEvent e)
        {
            if (!NativeWorld.TryGetComponent<StorageComponent>(e.StorageId, out StorageComponent storage)) return;
            InternedString itemKey = NativeWorld.InternString(e.ItemId);
            int existing = 0;
            storage.Items.TryGet(itemKey, out existing);
            storage.Items.Set(itemKey, existing + e.Quantity);
            using (WriteBatch<StorageComponent> batch = NativeWorld.BeginBatch<StorageComponent>())
                batch.Update(e.StorageId, storage);
            _cacheDirty = true;
        }

        private void OnItemRemoved(ItemRemovedEvent e)
        {
            if (!NativeWorld.TryGetComponent<StorageComponent>(e.StorageId, out StorageComponent storage)) return;
            InternedString itemKey = NativeWorld.InternString(e.ItemId);
            if (!storage.Items.TryGet(itemKey, out int currentQty)) return;
            int remaining = currentQty - e.Quantity;
            if (remaining <= 0)
                storage.Items.Remove(itemKey);
            else
                storage.Items.Set(itemKey, remaining);
            using (WriteBatch<StorageComponent> batch = NativeWorld.BeginBatch<StorageComponent>())
                batch.Update(e.StorageId, storage);
            _cacheDirty = true;

            var key = (e.StorageId, e.ItemId);
            if (_reservedQuantities.TryGetValue(key, out int reserved))
            {
                int remainingReserve = reserved - e.Quantity;
                if (remainingReserve <= 0) _reservedQuantities.Remove(key);
                else _reservedQuantities[key] = remainingReserve;
            }
        }

        private void OnItemReserved(ItemReservedEvent e)
        {
            var key = (e.StorageId, e.ItemId);
            _reservedQuantities.TryGetValue(key, out int current);
            _reservedQuantities[key] = current + e.Quantity;
            _cacheDirty = true;
        }

        private void RebuildCache()
        {
            _freeSlotCache.Clear();
            using SpanLease<StorageComponent> storages = NativeWorld.AcquireSpan<StorageComponent>();
            ReadOnlySpan<StorageComponent> storageSpan = storages.Span;
            ReadOnlySpan<int> storageIndices = storages.Indices;
            for (int i = 0; i < storages.Count; i++)
            {
                StorageComponent storage = storageSpan[i];
                if (!storage.IsFull && storage.Items.IsValid)
                {
                    int count = storage.Items.Count;
                    var keysBuf = new InternedString[count];
                    var valuesBuf = new int[count];
                    storage.Items.Iterate(keysBuf, valuesBuf);

                    var keyStrings = new List<string>(count);
                    for (int k = 0; k < count; k++)
                    {
                        string? resolved = keysBuf[k].Resolve(NativeWorld);
                        if (resolved is not null) keyStrings.Add(resolved);
                    }
                    _freeSlotCache[storageIndices[i]] = keyStrings;
                }
            }
        }
    }
}
