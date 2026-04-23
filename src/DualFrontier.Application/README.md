# DualFrontier.Application

## Назначение
Связующий слой между доменом (`Core`, `Systems`, `Components`, `Events`, `AI`) и
презентацией (`Presentation`). Здесь живёт главный цикл (`GameLoop`), сохранения
(`SaveSystem`), загрузка сценариев (`ScenarioLoader`), загрузчик модов (`ModLoader`)
и мост в Godot-слой (`PresentationBridge`). Application — единственная сборка,
которая знает и о домене, и о том, что есть «вышестоящий» слой рендеринга.

## Зависимости
- `DualFrontier.Contracts` — интерфейсы (`IMod`, `IModApi`, `IEvent`, `EntityId`, ...)
- `DualFrontier.Core` — `World`, `SystemBase`, `DomainEventBus`, `GameServices`
- `DualFrontier.Components` — стандартные компоненты
- `DualFrontier.Events` — доменные события
- `DualFrontier.Systems` — игровые системы
- `DualFrontier.AI` — поведенческие деревья / утилиты для AI

## Что внутри
- `Loop/` — главный цикл игры (`GameLoop`, `FrameClock`).
- `Save/` — сериализация мира (`ISaveSystem`, `SaveSystem`, `SaveFormat`).
- `Scenario/` — загрузка стартовых сценариев (`ScenarioLoader`, `ScenarioDef`).
- `Modding/` — загрузчик модов и изолированный `IModApi`
  (`ModLoader`, `ModLoadContext`, `RestrictedModApi`, `ModIsolationException`).
- `Bridge/` — мост Domain → Presentation через очередь команд
  (`PresentationBridge`, `IRenderCommand`, `Commands/`).

## Правила
- Application **может** знать о `Core` и `Systems` — это его работа склеивать.
- Application **не должен** знать о Godot или напрямую вызывать `Presentation`.
  Связь строго однонаправленная: Domain/Application → `PresentationBridge`
  (очередь команд) → Presentation читает в главном потоке.
- Загрузка мода **всегда** идёт через собственный `AssemblyLoadContext`
  (`ModLoadContext`) с `isCollectible: true`, чтобы обеспечить горячую выгрузку
  (TechArch 11.8).
- `SaveSystem.Save/Load` — синхронные. Асинхронные операции I/O выполняет
  вышестоящий уровень, передающий готовый путь/поток. Это правило из
  THREADING (см. `docs/THREADING.md`).

## Примеры использования
```csharp
// Обычный старт игры: сценарий + цикл + мост.
var services = new GameServices(); // из Core
var bridge   = new PresentationBridge();
var loop     = new GameLoop(services, bridge);

var scenario = new ScenarioLoader().Load("scenarios/default.json");
// TODO: создать World по ScenarioDef.

loop.Start();
```

## TODO
- [x] Фаза 1 — `GameLoop` с accumulator-based фиксированным шагом
      (30 Hz, пауза, speed x1/x2/x3).
- [ ] Фаза 1 — `SaveSystem.Save/Load` (binary + header `SaveFormat`).
- [ ] Фаза 2 — `ModLoader` (`AssemblyLoadContext`, реестр модов, горячая выгрузка).
- [ ] Фаза 2 — `RestrictedModApi` проксирует вызовы в `Core.GameServices`.
- [ ] Фаза 3 — подключить `PresentationBridge.DrainCommands` к Godot `_Process`
      (`PresentationBridge.SetScene` / `EnqueueInput` пока не существует,
      `GameBootstrap` не реализован).
- [x] Фаза 3 — `ScenarioLoader` парсит JSON через `System.Text.Json`.
