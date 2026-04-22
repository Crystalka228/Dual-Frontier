# Math — Математика сетки

## Назначение
Математические примитивы для работы с сеткой тайлов и пространственным
индексированием. Используется почти всеми системами: путь, поиск соседей,
спавн снарядов, размещение зданий.

## Зависимости
- `System` (BCL)

## Что внутри
- `SpatialGrid.cs` — разбиение мира на ячейки для O(1) запроса "всё в радиусе R".
  Используется CombatSystem (поиск целей), RaidSystem, BiomeSystem.

> `GridVector` живёт в `DualFrontier.Contracts.Math` — это общий примитив,
> доступный всем слоям включая `AI` и моды, которые не зависят от `Core`.

## Правила
- `SpatialGrid` НЕ потокобезопасный — защита через граф планировщика.
- Godot `Vector2I` использовать нельзя — это Godot namespace. Конвертация
  происходит только в Presentation.

## Примеры использования
```csharp
var a = new GridVector(3, 4);
var b = new GridVector(0, 0);
int dist = a.Manhattan(b); // 7

var grid = new SpatialGrid<EntityId>(cellSize: 16);
grid.Insert(id, position);
foreach (var near in grid.Query(position, radius: 5)) { /* ... */ }
```

## TODO
- [ ] Фаза 3 — реализовать `SpatialGrid.Insert/Remove/Update/Query`.
- [ ] Фаза 3 — покрыть `SpatialGrid` бенчмарком (BenchmarkDotNet).
