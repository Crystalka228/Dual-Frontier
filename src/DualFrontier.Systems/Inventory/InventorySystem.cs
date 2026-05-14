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
    /// Single writer of <see cref="StorageComponent"/> — all inventory changes
    /// go through this system's deferred subscription to <see cref="ItemAddedEvent"/>
    /// and <see cref="ItemRemovedEvent"/>; mutation runs at the next phase
    /// boundary inside this system's captured execution context, so the
    /// publisher (e.g. <c>HaulSystem</c>) does not need to declare
    /// <c>StorageComponent</c> as a write target.
    /// <para>
    /// <see cref="ItemReservedEvent"/> populates a persistent reservation table
    /// so future Phase 5+ multi-tick haul / craft can deduct reserved
    /// quantities before allocating.
    /// </para>
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

        /// <summary>
        /// Reservation table keyed by (storage entity, item id) → reserved
        /// quantity. Phase 4 exposes this for tests; Phase 5+ multi-tick haul
        /// will consult it before picking source stacks.
        /// </summary>
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
            var storage = GetComponent<StorageComponent>(e.StorageId);
            InternedString itemKey = NativeWorld.InternString(e.ItemId);
            int existing = 0;
            storage.Items.TryGet(itemKey, out existing);
            storage.Items.Set(itemKey, existing + e.Quantity);
            // K8.3+K8.4 Phase 4 dual-write — legacy mirror removed Phase 5 commit 21.
            using (var batch = NativeWorld.BeginBatch<StorageComponent>())
                batch.Update(e.StorageId, storage);
            SetComponent(e.StorageId, storage);
            _cacheDirty = true;
        }

        private void OnItemRemoved(ItemRemovedEvent e)
        {
            var storage = GetComponent<StorageComponent>(e.StorageId);
            InternedString itemKey = NativeWorld.InternString(e.ItemId);
            if (!storage.Items.TryGet(itemKey, out int currentQty)) return;
            int remaining = currentQty - e.Quantity;
            if (remaining <= 0)
                storage.Items.Remove(itemKey);
            else
                storage.Items.Set(itemKey, remaining);
            using (var batch = NativeWorld.BeginBatch<StorageComponent>())
                batch.Update(e.StorageId, storage);
            SetComponent(e.StorageId, storage);
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
            foreach (var entity in Query<StorageComponent>())
            {
                var storage = GetComponent<StorageComponent>(entity);
                if (!storage.IsFull && storage.Items.IsValid)
                {
                    int count = storage.Items.Count;
                    var keysBuf = new InternedString[count];
                    var valuesBuf = new int[count];
                    storage.Items.Iterate(keysBuf, valuesBuf);

                    var keyStrings = new List<string>(count);
                    for (int i = 0; i < count; i++)
                    {
                        string? resolved = keysBuf[i].Resolve(NativeWorld);
                        if (resolved is not null) keyStrings.Add(resolved);
                    }
                    _freeSlotCache[entity.Index] = keyStrings;
                }
            }
        }
    }
}
