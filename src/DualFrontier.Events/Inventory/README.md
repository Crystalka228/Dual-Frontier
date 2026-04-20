# Inventory Events

## Назначение
События инвентаря и крафта: добавление/удаление/резервирование
предметов и запросы на крафт.

## Зависимости
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Что внутри
- `ItemAddedEvent.cs` — предмет положен в хранилище / в руки пешке.
- `ItemRemovedEvent.cs` — предмет изъят из хранилища.
- `ItemReservedEvent.cs` — предмет зарезервирован под работу (крафт, постройку, перенос).
- `CraftRequestEvent.cs` — пешка запросила крафт (AI или игрок).

## Правила
- Все изменения `StorageComponent.Items` проходят через эти события,
  чтобы остальные системы могли отреагировать (UI, сигналы, лог).
- `ItemReservedEvent` предотвращает двойное использование: пока предмет
  зарезервирован, другой haul-запрос его не возьмёт.
- `CraftRequestEvent` — **не** начало крафта, а заявка: JobSystem
  приоритизирует и назначает пешку.

## Примеры использования
```csharp
_bus.Publish(new CraftRequestEvent { /* RecipeId = "rifle", RequesterId = player */ });
// → JobSystem назначает пешку → ItemReservedEvent для компонентов → WorkbenchSystem крафтит
// → ItemRemovedEvent (компоненты) + ItemAddedEvent (готовое оружие)
```

## TODO
- [ ] Добавить `CraftCompletedEvent` / `CraftFailedEvent` — Фаза 4.
- [ ] Решить, хранить ли `ItemTemplateId` в событии или брать из компонента предмета.
