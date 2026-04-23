# DualFrontier.AI

## Назначение
Поведенческая прослойка пешек и юнитов: behaviour trees, джобы
(задачи, которые пешка исполняет по шагам) и поиск пути A*.
Системы из `DualFrontier.Systems` вызывают AI как чистые
утилиты (без состояния на мир) — а сами эффекты прикладывают
через свои WRITE-компоненты.

## Зависимости
- `DualFrontier.Contracts` — `EntityId`, `GridVector`, базовые типы.
- `DualFrontier.Components` — для чтения данных пешки (навыки,
  позиция, инвентарь) — через `BTContext`/`IJob`.

**НЕ зависит** от `DualFrontier.Core` (иначе закольцуем граф —
Core содержит шедулер, который вызывает Systems, которые
вызывают AI). Все примитивы, которые AI разделяет с Systems и
Components (например `GridVector`), живут в `Contracts` — это
тот же слой, от которого зависят моды.

## Что внутри
- `BehaviourTree/` — универсальный BT (Selector / Sequence / Leaf).
- `Jobs/` — `IJob` + конкретные джобы (хаул, крафт, каст, медитация,
  приказ голему).
- `Pathfinding/` — `IPathfindingService`, A*-реализация, сетка
  проходимости.

## Правила
- AI НЕ имеет доступа к `World` и шинам — ему скармливают данные
  через `BTContext` / аргументы `IJob.Tick`.
- AI НЕ должен кешировать состояние между тиками системы, если
  джоб не помечен как stateful (stateful джоб хранит локальный
  прогресс в своих полях).
- Никакого `async`/`await` — поиск пути синхронный (см. THREADING).
  Длинный A* режем на кадры через `TryFindPath` с лимитом итераций.
- BT-ноды — чистые функции по `BTContext`, никаких глобальных
  синглтонов.

## Примеры использования
```csharp
// В системе Pawn/JobSystem:
var job = new JobHaul(/* args */);
job.Start();
if (job.Tick(delta) == JobStatus.Done) { /* ... */ }
```

## TODO
- [x] Реализовать `AStarPathfinding` с лимитом итераций за тик
      (2000 итераций за вызов, без кэша путей — см.
      `Pathfinding/README.md`).
- [ ] Написать BT-парсер из JSON для модов.
- [ ] Покрыть юнит-тестами `Selector` / `Sequence` / `Leaf`.
- [ ] Реализовать `JobCast` (интеграция со `SpellSystem` через шину).
