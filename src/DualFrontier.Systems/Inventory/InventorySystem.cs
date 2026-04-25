using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Components.Building;
using DualFrontier.Core.ECS;
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
    /// Phase: Фаза 4. Tick: FAST.
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
            if (storage.Items.ContainsKey(e.ItemId))
                storage.Items[e.ItemId] += e.Quantity;
            else
                storage.Items[e.ItemId] = e.Quantity;
            SetComponent(e.StorageId, storage);
            _cacheDirty = true;
        }

        private void OnItemRemoved(ItemRemovedEvent e)
        {
            var storage = GetComponent<StorageComponent>(e.StorageId);
            if (!storage.Items.ContainsKey(e.ItemId)) return;
            storage.Items[e.ItemId] -= e.Quantity;
            if (storage.Items[e.ItemId] <= 0)
                storage.Items.Remove(e.ItemId);
            SetComponent(e.StorageId, storage);
            _cacheDirty = true;

            var key = (e.StorageId, e.ItemId);
            if (_reservedQuantities.TryGetValue(key, out int reserved))
            {
                int remaining = reserved - e.Quantity;
                if (remaining <= 0) _reservedQuantities.Remove(key);
                else _reservedQuantities[key] = remaining;
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
                if (!storage.IsFull)
                {
                    _freeSlotCache[entity.Index] =
                        new List<string>(storage.Items.Keys);
                }
            }
        }
    }
}
