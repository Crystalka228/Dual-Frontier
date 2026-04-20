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
var phases = graph.Build();

var scheduler = new ParallelSystemScheduler(phases);
scheduler.ExecuteTick(deltaSeconds: 0.016f);
```

## TODO
- [ ] Фаза 1 — реализовать топологическую сортировку графа.
- [ ] Фаза 1 — реализовать параллельное исполнение фазы.
- [ ] Фаза 1 — реализовать TickScheduler с разными частотами.
- [ ] Фаза 2 — добавить детекцию циклов в графе с диагностикой.
