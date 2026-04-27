# Inventory Systems

## Purpose
Storage, hauling, and crafting. See the GDD "Resources and production" section
and TechArch 11.5 (storage request cache/batching).

## Dependencies
- `DualFrontier.Contracts` — attributes, `IInventoryBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Building` — `StorageComponent`,
  `WorkbenchComponent`.
- `DualFrontier.Components.Pawn` — `JobComponent`, `SkillsComponent`.
- `DualFrontier.Components.Shared` — `PositionComponent`.
- `DualFrontier.Events.Inventory` — `AmmoIntentEvent`,
  `AmmoGrantedEvent`, `AmmoRefusedEvent`, `ItemAddedEvent`.

## Contents
- `InventorySystem.cs` — FAST: storage request handler (cache + batching).
- `HaulSystem.cs` — NORMAL: pawns move stacks between storages.
- `CraftSystem.cs` — NORMAL: recipe execution at workbenches.

## Rules
- Domain bus — `nameof(IGameServices.Inventory)`.
- `InventorySystem` is the only system that writes `StorageComponent`. Every
  storage change goes through requests on its bus, otherwise batching breaks
  (TechArch 11.5).
- `HaulSystem` does not write to storage directly — it only publishes
  `ItemPickupEvent` / `ItemDropEvent`.

## Usage examples
```csharp
// Inside CombatSystem:
inventoryBus.Publish(new AmmoIntentEvent(shooter, weaponId, amount: 1));
// Later InventorySystem responds with AmmoGranted or AmmoRefused.
```

## TODO
- [x] Implement `InventorySystem` with a free-slot cache
      (`_freeSlotCache` + `_cacheDirty`).
- [x] Implement `HaulSystem`: "from → to" item lookup
      (Phase 4: teleport without pathfinding, real haul — Phase 6).
- [ ] Implement `CraftSystem` — Phase 6 (currently a stub that throws
      `NotImplementedException`).
- [ ] Move `ItemAddedEvent` / `ItemRemovedEvent` to `[Deferred]` delivery —
      otherwise `InventorySystem.OnItemAdded.SetComponent` runs in the
      publisher's context and trips the DEBUG isolation guard as soon as
      real `StorageComponent` entities appear.
