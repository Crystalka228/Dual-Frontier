# Scheduling — Параллельный планировщик

## Назначение
Строит граф READ/WRITE зависимостей между системами на основе декларации
`[SystemAccess]`, топологически сортирует его в фазы и параллельно исполняет
системы одной фазы. `TickScheduler` поверх этого управляет разными частотами
тиков: REALTIME/FAST/NORMAL/SLOW/RARE.

## Зависимости
- `DualFrontier.Contracts` (атрибуты `SystemAccessAttribute`, `TickRateAttribute`).
- `DualFrontier.Core.ECS` (`SystemBase`).

## Что внутри
- `DependencyGraph.cs` — построение графа конфликтов по декларациям систем.
- `ParallelSystemScheduler.cs` — исполнение фаз через `Parallel.ForEach` или
  пул задач.
- `SystemPhase.cs` — неизменяемый список систем одной фазы.
- `TickScheduler.cs` — решает, на каких тиках запускать какую систему.
- `TickRates.cs` — константы частот (дублирует `DualFrontier.Contracts.Attributes.TickRates`).

## Правила
- Граф строится один раз при старте, после загрузки всех модов. Поздняя
  регистрация системы => перестройка графа (дорого).
- Система не может быть в нескольких фазах одновременно.
- Два потока не пишут в один `ComponentStore`: это инвариант графа.

## Примеры использования
```csharp
var graph = new DependencyGraph();
graph.AddSystem(new CombatSystem());
graph.AddSystem(new ManaSystem());
graph.Build();

var ticks = new TickScheduler();
var scheduler = new ParallelSystemScheduler(
    graph.GetPhases(), ticks, world, faultSink: null, services: services);
scheduler.ExecuteTick(delta: 1f / 30f);
```

## TODO
- [x] Фаза 1 — реализовать топологическую сортировку графа.
- [x] Фаза 1 — реализовать параллельное исполнение фазы.
- [x] Фаза 1 — реализовать TickScheduler с разными частотами.
- [x] Фаза 1 — добавить детекцию циклов в графе с диагностикой.
