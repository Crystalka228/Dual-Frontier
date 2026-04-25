# Inventory Systems

## Назначение
Склад, хаулы (переноска) и крафт. См. GDD раздел "Ресурсы и
производство" и TechArch 11.5 (кэш/батчинг запросов склада).

## Зависимости
- `DualFrontier.Contracts` — атрибуты, `IInventoryBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Building` — `StorageComponent`,
  `WorkbenchComponent`.
- `DualFrontier.Components.Pawn` — `JobComponent`, `SkillsComponent`.
- `DualFrontier.Components.Shared` — `PositionComponent`.
- `DualFrontier.Events.Inventory` — `AmmoIntentEvent`,
  `AmmoGrantedEvent`, `AmmoRefusedEvent`, `ItemAddedEvent`.

## Что внутри
- `InventorySystem.cs` — FAST: обработчик запросов склада (кэш + батчинг).
- `HaulSystem.cs` — NORMAL: пешки переносят стаки между складами.
- `CraftSystem.cs` — NORMAL: выполнение рецептов на верстаках.

## Правила
- Шина домена — `nameof(IGameServices.Inventory)`.
- `InventorySystem` — единственная система, которая пишет
  `StorageComponent`. Все изменения склада проходят через
  запросы в её шину, иначе ломается батчинг (TechArch 11.5).
- `HaulSystem` ничего не пишет напрямую в склад — только
  публикует `ItemPickupEvent` / `ItemDropEvent`.

## Примеры использования
```csharp
// Внутри CombatSystem:
inventoryBus.Publish(new AmmoIntentEvent(shooter, weaponId, amount: 1));
// Позже InventorySystem ответит AmmoGranted или AmmoRefused.
```

## TODO
- [x] Реализовать `InventorySystem` с кэшем свободных слотов
      (`_freeSlotCache` + `_cacheDirty`).
- [x] Реализовать `HaulSystem`: поиск "откуда → куда" по предмету
      (Phase 4: телепорт без pathfinding, real haul — Phase 6).
- [ ] Реализовать `CraftSystem` — Фаза 6 (сейчас стаб бросает
      `NotImplementedException`).
- [ ] Перевести `ItemAddedEvent`/`ItemRemovedEvent` на `[Deferred]`
      доставку — иначе `InventorySystem.OnItemAdded.SetComponent`
      исполняется в контексте публикующей системы и сорвёт DEBUG
      isolation guard, как только появятся реальные `StorageComponent`
      сущности.
