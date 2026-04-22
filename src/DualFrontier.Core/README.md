# DualFrontier.Core

## Назначение
Ядро проекта: ECS (World + ComponentStore + SystemBase + сторож изоляции),
параллельный планировщик (граф READ/WRITE зависимостей, фазы, tick rates),
реализации доменных шин событий, математика сетки и реестры компонентов и
систем. Вся сборка — `internal`-first: снаружи доступны только типы явно
помеченные `public`, и то через `InternalsVisibleTo` для `Systems`,
`Application` и `Core.Tests`.

## Зависимости
- `DualFrontier.Contracts` — контракты ECS, шин, атрибуты.

## Что внутри
- `ECS/` — `World`, `ComponentStore`, `SystemBase`, `SystemExecutionContext`
  (сторож), `IsolationViolationException`.
- `Scheduling/` — `DependencyGraph`, `ParallelSystemScheduler`, `SystemPhase`,
  `TickScheduler`, `TickRates`.
- `Bus/` — `DomainEventBus`, `GameServices`, `IntentBatcher`.
- `Math/` — `SpatialGrid` (инфраструктурное разбиение мира; `GridVector`-примитив
  живёт в `DualFrontier.Contracts.Math`).
- `Registry/` — `ComponentRegistry`, `SystemRegistry`.

## Правила
- Никаких ссылок на Godot.
- Никакой игровой логики — она живёт в `DualFrontier.Systems`.
- Все типы сделаны `internal` кроме `SystemBase` и публичных контрактов
  (которые на самом деле определены в `Contracts`) — системам Core даётся
  через `InternalsVisibleTo`.
- Параллельность обеспечивается планировщиком, не самими структурами. Если
  тип предполагается использовать напрямую из множества потоков — он обязан
  быть потокобезопасным и документировать это.

## Примеры использования
```csharp
// Из DualFrontier.Application (через InternalsVisibleTo)
var world = new World();
var services = new GameServices();
var scheduler = new ParallelSystemScheduler(world, services);
scheduler.ExecuteTick(deltaSeconds: 0.016f);
```

## TODO
- [ ] Фаза 1 — реализовать `World`/`ComponentStore` со SparseSet.
- [ ] Фаза 1 — реализовать `SystemExecutionContext` (ThreadLocal сторож).
- [ ] Фаза 1 — реализовать `DependencyGraph` и `ParallelSystemScheduler`.
- [ ] Фаза 1 — реализовать `DomainEventBus` и `GameServices`.
- [ ] Фаза 2 — написать isolation-тесты, подтверждающие краш сторожа.
