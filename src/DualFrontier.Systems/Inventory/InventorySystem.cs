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
    /// Manages item storage. Caches free storage slots to avoid
    /// full scan every tick. Cache invalidated on ItemAdded/Removed.
    /// Single writer of StorageComponent — all inventory changes
    /// go through this system's bus.
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
        private bool _cacheDirty = true;

        protected override void OnInitialize()
        {
            // TODO: Фаза 6 — после реализации [Deferred] семантики
            // в DomainEventBus перевести ItemAddedEvent/ItemRemovedEvent
            // на [Deferred] доставку чтобы избежать cross-context mutation.
            // Сейчас безопасно пока нет реальных StorageComponent entity.
            Services.Inventory.Subscribe<ItemAddedEvent>(OnItemAdded);
            Services.Inventory.Subscribe<ItemRemovedEvent>(OnItemRemoved);
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
