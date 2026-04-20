# Pathfinding

## Назначение
Поиск пути A* по сетке проходимости. Сугубо синхронный:
никакого `async/await`, иначе ломается `ThreadLocal` сторож
изоляции систем (см. THREADING).

## Зависимости
- `DualFrontier.Contracts` — `EntityId` (не используется напрямую сейчас).
- `DualFrontier.Components.World` — `TileComponent` (для постройки
  `NavGrid` из мира — это делает верхний слой, не сама AI).

**НЕТ** зависимости на `DualFrontier.Core`, где живёт
`GridVector` (`DualFrontier.Core.Math`). Пока `GridVector` не
переедет в `Contracts`, сигнатуры интерфейсов используют
`object` с TODO-пометкой.

## Что внутри
- `IPathfindingService.cs` — интерфейс с синхронным `TryFindPath`.
- `AStarPathfinding.cs` — реализация A*.
- `NavGrid.cs` — битмап проходимости для запросов.

## Правила
- Синхронный API. Длинный A* бьём на лимит итераций за тик и
  возвращаем False при превышении (пешка попробует на
  следующем тике).
- `NavGrid` — иммутабельна для одного запроса; обновляется
  снаружи батчами, когда в мире что-то построили/снесли.
- Никаких статических синглтонов — сервис инъектируется.

## Примеры использования
```csharp
IPathfindingService pf = /* resolve */;
if (pf.TryFindPath(from, to, out var path)) { /* использовать */ }
```

## TODO
- [ ] Заменить `object` на `GridVector`, когда тип переедет
      в `Contracts` (или в новую сборку `DualFrontier.Primitives`).
- [ ] Реализовать `AStarPathfinding` с бинарной кучей и лимитом итераций.
- [ ] Реализовать `NavGrid` как bitset по тайлам.
- [ ] Добавить hierarchical pathfinding для дальних целей.
