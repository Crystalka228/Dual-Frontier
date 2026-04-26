# Архитектура Dual Frontier

Проект строится вокруг одного жёсткого принципа: система не имеет права обращаться к чужим данным напрямую. Взаимодействие происходит только через контракты. Любое отклонение ловится сторожем изоляции и приводит к немедленному крашу с диагностикой, а не к тихому повреждению состояния.

## История изменений

- **v0.3 (2026-04):** Закрытие архитектурного долга Phase 4. `[Deferred]`/`[Immediate]` доставка реализована в `DomainEventBus` (per-bus очередь, дренаж между фазами, захват `SystemExecutionContext` подписчика); добавлена шестая доменная шина `IPowerBus` для `ElectricGridSystem` + `ConverterSystem`; разорван компонентный цикл ElectricGrid↔Converter через `[Deferred] ConverterPowerOutputEvent`; `ItemAddedEvent`/`ItemRemovedEvent`/`ItemReservedEvent` помечены `[Deferred]` — мутация `StorageComponent` идёт в контексте `InventorySystem`, изоляция `HaulSystem.writes=[]` сохранена; введён `BridgeImplementationAttribute(Phase = N)` и применён ко всем системам со стаб-`OnInitialize`.
- **v0.2 (2026-04):** Добавлены модели Lease ([RESOURCE_MODELS](./RESOURCE_MODELS.md), [EVENT_BUS](./EVENT_BUS.md)), двухфазный commit для multi-bus запросов ([COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md)), feedback-loop resolution через tick lag ([FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md)), детерминированная резолюция урона ([COMBO_RESOLUTION](./COMBO_RESOLUTION.md)), состояния владения големом ([OWNERSHIP_TRANSITION](./OWNERSHIP_TRANSITION.md)), bridge-pattern между фазами 5 и 6 ([ROADMAP](./ROADMAP.md)).
- **v0.1 (2026-03):** Исходный каркас: четыре слоя, пять доменных шин, декларативная изоляция, параллельный планировщик.

## Проблемы RimWorld которые решаются

RimWorld стал популярной отправной точкой для колониальных симуляторов, но его архитектура несёт три хронические проблемы, и Dual Frontier исправляет каждую из них.

### Производительность

Боевая система RimWorld сама лезет на склад проверять патроны. Сто пешек, шестьдесят тиков в секунду, обход O(n) предметов — итог десятки тысяч сканов за секунду даже на маленькой колонии. Dual Frontier заменяет прямые сканы контрактной шиной и кэшами инвалидации: боевая система публикует `AmmoIntent`, складская система отвечает из кэша за O(1), батч из сотни намерений обрабатывается одним проходом.

### Многопоточность

RimWorld однопоточный: все системы бегут по очереди, даже если конфликтов между ними нет. Dual Frontier заставляет каждую систему декларировать читаемые и записываемые компоненты через `[SystemAccess]`. Планировщик строит граф зависимостей один раз при старте, топологически сортирует его в фазы и исполняет несвязанные системы параллельно — на 8 ядрах фазы идут до 6–7 потоков одновременно.

### Модифицируемость

В RimWorld мод через Harmony патчит любой приватный метод и ломает его. Dual Frontier грузит каждый мод в отдельный `AssemblyLoadContext`: мод физически не видит `DualFrontier.Core`, у него нет ссылки ни на `World`, ни на конкретную систему. Моды взаимодействуют между собой через `IModContract` — декларацию публичного API, а не через рефлексию.

## Четыре слоя

Архитектура делится на четыре слоя. Каждый слой знает только о слоях ниже себя.

```
┌─────────────────────────────────────────────────────┐
│                   PRESENTATION                      │
│         Godot SceneTree, UI, Рендер, Ввод           │
│         Только main thread. Только визуал.          │
├─────────────────────────────────────────────────────┤
│                  APPLICATION                        │
│      GameLoop, SaveSystem, ScenarioManager          │
│      Очередь команд Domain → Presentation           │
├─────────────────────────────────────────────────────┤
│                    DOMAIN                           │
│   Systems, Entities, Components, Contracts          │
│   Многопоточно. Не знает о Godot.                   │
├─────────────────────────────────────────────────────┤
│                 INFRASTRUCTURE                      │
│   EventBus (доменные шины), Pathfinding,            │
│   SpatialGrid, ParallelScheduler                    │
└─────────────────────────────────────────────────────┘
```

### Presentation

Сборки `DualFrontier.Presentation` (Godot DevKit) и `DualFrontier.Presentation.Native`
(production-рантайм на Silk.NET). Обе реализуют контракты `IRenderer`, `ISceneLoader`,
`IInputSource` из Application. Godot — инструмент разработки и редактор сцен;
Native — то, что запускают игроки. Работают только в главном потоке своего бэкенда.
Не вызывают Domain напрямую — читают команды из `PresentationBridge` и
отправляют ввод в шины через свою реализацию `IInputSource`.
Подробно: [VISUAL_ENGINE](./VISUAL_ENGINE.md).

### Application

Сборка `DualFrontier.Application`. Связующий слой: главный игровой цикл `GameLoop`, система сохранений, загрузчик сценариев, `ModLoader`. Содержит `PresentationBridge` — однонаправленную очередь команд из Domain в Presentation.

### Domain

Сборки `DualFrontier.Systems`, `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.AI`. Все игровые правила. Многопоточно: системы исполняются параллельно. Не импортирует `using Godot;` никогда.

### Infrastructure

Сборка `DualFrontier.Core`. Инфраструктура ECS: `World`, `ComponentStore`, `DomainEventBus`, `ParallelSystemScheduler`, `DependencyGraph`, `SpatialGrid`. Всё `internal` — снаружи видны только контракты. Сборке `DualFrontier.Systems` открыт доступ через `InternalsVisibleTo`.

## Состав решения

Полный перечень сборок, входящих в `DualFrontier.sln`, плюс нативный
side-car проект.

| Сборка | Слой | Назначение |
|---|---|---|
| `DualFrontier.Contracts` | All | Публичные интерфейсы, атрибуты, маркер-типы. Единственное, что видит мод. |
| `DualFrontier.Core` | Infrastructure | ECS-ядро, планировщик, доменные шины. `internal` — снаружи недоступно. |
| `DualFrontier.Core.Interop` | Infrastructure | P/Invoke-обёртки над `native/DualFrontier.Core.Native/`. См. [NATIVE_CORE_EXPERIMENT](./NATIVE_CORE_EXPERIMENT.md). |
| `native/DualFrontier.Core.Native/` | Infrastructure | C++-исследовательская реализация ECS-ядра. Решение по batch-API отложено до Phase 9. |
| `DualFrontier.Components` | Domain | Чистые POCO-компоненты, никакой логики. |
| `DualFrontier.Events` | Domain | `record`-события, запросы, команды, intents. |
| `DualFrontier.Systems` | Domain | Игровые системы. `[SystemAccess]` декларирует R/W. |
| `DualFrontier.AI` | Domain | Behavior Tree, Job-ы, Pathfinding. |
| `DualFrontier.Persistence` | Infrastructure | Snapshot-кодеры, RLE, range encoding, StringPool. Не зависит от Godot. |
| `DualFrontier.Application` | Application | GameLoop, Save/Load, ModLoader, PresentationBridge. |
| `DualFrontier.Presentation` | Presentation | Godot DevKit: редактор сцен, визуальная отладка. Единственная сборка, которой разрешён `using Godot`. |
| `DualFrontier.Presentation.Native` | Presentation | Production-рантайм на Silk.NET + OpenGL. Собирается без зависимости от Godot. |

## Правила зависимостей

Направление стрелок зависимостей — строго сверху вниз. Нарушение = ошибка архитектурного ревью.

- `Contracts` не зависит ни от чего кроме `System.*`.
- `Components`, `Events` зависят только от `Contracts`.
- `Core` зависит от `Contracts`.
- `Core.Interop` зависит от `Contracts` и от нативной библиотеки `DualFrontier.Core.Native` через P/Invoke. Не ссылается на игровые сборки.
- `Persistence` зависит от `Contracts`. Не знает о Godot.
- `Systems` зависит от `Contracts`, `Components`, `Events` и от `Core` через `InternalsVisibleTo`.
- `AI` зависит от `Contracts` и `Components`.
- `Application` зависит от `Core` и `Systems`.
- `Presentation` (Godot DevKit) зависит от `Application` и `Godot`.
- `Presentation.Native` зависит от `Application` и `Silk.NET` (не от `Godot`).
- Моды зависят **только** от `Contracts`. Ссылка на `Core` из мода блокируется `AssemblyLoadContext`.

## Граница движок / игра

Кодовая база развивается двумя параллельными треками: **игра** Dual Frontier
(Phase 0–7, основная ветка) и **движок** — обобщённое ECS-ядро, которое
после релиза игры форкается в отдельный продукт. Граница между этими треками
проходит по сборкам и проверяется на каждом PR.

| Движковые (generic, переиспользуемые) | Игровые (специфичны для Dual Frontier) |
|---------------------------------------|----------------------------------------|
| `DualFrontier.Contracts` | `DualFrontier.Components` |
| `DualFrontier.Core` | `DualFrontier.Events` |
| `DualFrontier.Core.Interop` | `DualFrontier.Systems` |
| `native/DualFrontier.Core.Native/` | `DualFrontier.AI` |
| `DualFrontier.Presentation.Native` | `DualFrontier.Presentation` (Godot DevKit) |
| Модинг-секция `DualFrontier.Application` | Игровой цикл `DualFrontier.Application` |

Главный инвариант: **движковые сборки никогда не ссылаются на игровые**.
Прикладной чек-лист на каждый PR, который этот инвариант проверяет, —
в [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md). Развилка форка после
релиза описана в [ROADMAP §«Пост-релиз — развилка на движок»](./ROADMAP.md#пост-релиз--развилка-на-движок).

## Зачем так: сценарии

### Сценарий 1 — маг тратит манну

`SpellSystem` публикует `ManaIntent` в `IMagicBus`. `ManaSystem` собирает все намерения в следующей фазе, проверяет запасы по `ManaComponent`, отвечает `ManaGranted` или `ManaRefused`. Между двумя шагами планировщик успевает прокрутить параллельно `WeatherSystem` и `NeedsSystem`. Это ровно то, что невозможно в однопоточном RimWorld.

### Сценарий 2 — мод добавляет школу Пустоты

Мод `VoidMagic` загружается в отдельный `AssemblyLoadContext`. Он регистрирует компонент `VoidAffinityComponent` через `IModApi`, регистрирует `VoidSpellSystem` с декларацией `[SystemAccess(reads: [VoidAffinityComponent])]` и публикует контракт `IVoidMagicContract`. Другой мод видит контракт через `api.TryGetContract<IVoidMagicContract>` и строит свою логику поверх — при выгрузке `VoidMagic` второй мод просто не подписывается, без крашей.

### Сценарий 3 — пешка умирает

`DamageSystem` снижает `HealthComponent.Current` до нуля, публикует `DeathEvent` (помечен `[Deferred]`) в `IPawnBus`. В следующей фазе `MoodSystem` реагирует на смерть, `SocialSystem` корректирует отношения, `Application` через `PresentationBridge` кладёт `PawnDiedCommand` в очередь — Godot подхватит команду в `_Process()` и проиграет анимацию. Domain и Presentation ни разу не встретились.

## Диаграмма зависимостей сборок

```
                     ┌────────────────────────────┐
                     │   DualFrontier.Contracts    │
                     │  (интерфейсы, атрибуты)     │
                     └──────────────┬──────────────┘
                                    │
           ┌────────────────────────┼────────────────────────┐
           │                        │                        │
           ▼                        ▼                        ▼
 ┌─────────────────┐   ┌─────────────────────┐   ┌────────────────────┐
 │   Components    │   │       Events        │   │       Core         │
 │   (POCO only)   │   │    (records only)   │   │  (ECS internal)    │
 └────────┬────────┘   └──────────┬──────────┘   └──────────┬─────────┘
          │                       │                         │
          │                       │                         │ InternalsVisibleTo
          ▼                       ▼                         ▼
       ┌────────────────────────────────────────────────────────────┐
       │                   DualFrontier.Systems                      │
       │   Combat / Magic / Pawn / Inventory / Power / World / ...  │
       └──────────────────────────────┬──────────────────────────────┘
                                      │
                                      ▼
                         ┌────────────────────────────┐
                         │   DualFrontier.Application  │
                         │   GameLoop / Save / Mods    │
                         └──────────────┬──────────────┘
                                        │
                                        ▼
                         ┌────────────────────────────┐
                         │  DualFrontier.Presentation  │
                         │   Godot nodes / UI / Input  │
                         └────────────────────────────┘

    ┌──────────────────────┐
    │  Mods/AnyMod.dll      │ ──► зависят ТОЛЬКО от Contracts
    └──────────────────────┘
```

## См. также

- [CONTRACTS](./CONTRACTS.md)
- [ECS](./ECS.md)
- [THREADING](./THREADING.md)
- [RESOURCE_MODELS](./RESOURCE_MODELS.md)
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md)
- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md)
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md)
- [OWNERSHIP_TRANSITION](./OWNERSHIP_TRANSITION.md)
