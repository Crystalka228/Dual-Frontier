# Дорожная карта

Реализация Dual Frontier разделена на восемь фаз. Каждая фаза имеет чёткий выходной артефакт, критерии приёмки и список систем, которые она разблокирует. Фазы не пересекаются по владению кодом: сначала один пласт, потом следующий. Это даёт возможность вести интеграционные тесты на каждом шаге и не накапливать риски.

## Фаза 0 — Контракты

Цель: зафиксировать все публичные интерфейсы и атрибуты в сборке `DualFrontier.Contracts`.

### Что реализуем

Файлы в `src/DualFrontier.Contracts/`:

- `Core/IEntity.cs`, `Core/EntityId.cs`, `Core/IComponent.cs`, `Core/IEvent.cs`, `Core/IQuery.cs`, `Core/IQueryResult.cs`, `Core/ICommand.cs`.
- `Bus/IEventBus.cs`, `Bus/IGameServices.cs`, `Bus/ICombatBus.cs`, `Bus/IInventoryBus.cs`, `Bus/IMagicBus.cs`, `Bus/IPawnBus.cs`, `Bus/IWorldBus.cs`.
- `Modding/IMod.cs`, `Modding/IModApi.cs`, `Modding/IModContract.cs`, `Modding/ModManifest.cs`.
- `Attributes/SystemAccessAttribute.cs`, `Attributes/DeferredAttribute.cs`, `Attributes/ImmediateAttribute.cs`, `Attributes/TickRateAttribute.cs`.

### Критерии приёмки

- `dotnet build src/DualFrontier.Contracts` проходит без ошибок и предупреждений.
- Каждый публичный тип имеет XML-документацию на английском.
- Зависимости: только `System.*`. Ни одного project reference.

### Разблокирует

Всё остальное — ни одна другая сборка без `Contracts` не собирается.

## Фаза 1 — Ядро

Цель: работающее ECS-ядро с многопоточным планировщиком и доменными шинами.

> Дополнено в v0.3 (закрытие долга Phase 4):
> `[Deferred]` и `[Immediate]` семантика реализованы в `DomainEventBus` —
> `[Deferred]`-события накапливаются в per-bus очереди и доставляются
> `ParallelSystemScheduler` на границе каждой фазы внутри захваченного
> при Subscribe `SystemExecutionContext` подписчика;
> `[Immediate]` гарантированно доставляется синхронно. См.
> [EVENT_BUS](./EVENT_BUS.md).

### Что реализуем

Файлы в `src/DualFrontier.Core/`:

- `ECS/World.cs`, `ECS/ComponentStore.cs`, `ECS/SystemBase.cs`, `ECS/SystemExecutionContext.cs`, `ECS/IsolationViolationException.cs`.
- `Scheduling/DependencyGraph.cs`, `Scheduling/ParallelSystemScheduler.cs`, `Scheduling/SystemPhase.cs`, `Scheduling/TickScheduler.cs`, `Scheduling/TickRates.cs`.
- `Bus/DomainEventBus.cs`, `Bus/GameServices.cs`, `Bus/IntentBatcher.cs`.
- `Math/SpatialGrid.cs` (инфраструктура; примитив `GridVector` живёт в `DualFrontier.Contracts.Math`).
- `Registry/ComponentRegistry.cs`, `Registry/SystemRegistry.cs`.

Главный цикл симуляции (`src/DualFrontier.Application/Loop/`):

- [x] `FrameClock` — источник `delta` на базе `Stopwatch`, метод `Update()`.
- [x] `GameLoop` — accumulator-based tick (30 Hz), пауза, speed x1/x2/x3.

### Критерии приёмки

- Unit-тесты ComponentStore, DependencyGraph, DomainEventBus проходят.
- Тест планировщика на сценарии "3 системы, 2 фазы" с реальным параллелизмом.
- `SystemExecutionContext` в DEBUG ловит обращение к незадекларированному компоненту.
- `IntentBatcher` группирует события за фазу, доставляет батч в следующей.

### Разблокирует

Все игровые системы. Начиная с этой фазы `DualFrontier.Systems` собирается.

## Фаза 2 — Верификация

Цель: доказать, что архитектурные гарантии работают, а не только объявлены.

### Что реализуем

Тесты в `tests/`:

- `DualFrontier.Core.Tests/Isolation/` — полный набор тестов сторожа: незадекларированный компонент, прямой доступ к системе, запрос `GetSystem`, async в `Update`, запись в чужую шину.
- `DualFrontier.Core.Tests/Scheduling/` — параллельные системы без конфликтов действительно выполняются на разных потоках (проверка через `Thread.CurrentThread.ManagedThreadId`).
- `DualFrontier.Modding.Tests/` — мод не грузит `DualFrontier.Core.dll` (проверка `ModLoadContext`).

Плюс скриптовые fixture-моды в `tests/fixtures/` — как легальные, так и "злые" (пытаются нарушить).

Исходники в `src/`:

- `DualFrontier.Application/Modding/` — `ModIntegrationPipeline`,
  `ContractValidator`, `ModRegistry`, `ModContractStore`, `RestrictedModApi` (полная реализация).

Компоненты мира (`src/DualFrontier.Components/World/`):

- [x] `TerrainKind` enum (Grass, Rock, Sand, Water, Ice, Swamp, Arcane, Unknown).
- [x] `TileComponent` — `Terrain`, `Passable`, `Default`.

### Технический долг

- [ ] Modding-тесты `AssemblyLoadContext` (WeakReference unload, попытка
      загрузить `DualFrontier.Core.dll`) — не реализованы.

### Критерии приёмки

- 100% тестов изоляции проходят как в DEBUG, так и в RELEASE (для критических).
- Mod-тест: попытка загрузить сборку с `using DualFrontier.Core;` возвращает `ModIsolationException`.
- Документированный отчёт о покрытии сторожа — какие типы нарушений протестированы.
- `ModIntegrationPipeline.Apply` атомарна: ошибка на любом шаге не ломает текущий планировщик.
- `ContractValidator` возвращает точное сообщение при write-write конфликте между модами.
- WeakReference тест: `ModLoadContext.Unload` физически освобождает сборку.

### Разблокирует

Безопасную разработку всего последующего. Теперь любая регрессия в изоляции будет пойматься CI.

## Фаза 3 — Пешки

Цель: живая пешка, которая гуляет по карте, ест, спит и имеет задачи.

### ✅ Фаза 3 — Живая колония (завершена)

Результат: пешки ходят по карте через A* pathfinding,
нужды деградируют, настроение пересчитывается,
джобы назначаются по приоритетам через шины событий.
61/61 тестов. Godot визуал подключён.

### Что реализуем

- `DualFrontier.Components/Shared/PositionComponent`, `FactionComponent`, `RaceComponent`.
- `DualFrontier.Components/Pawn/NeedsComponent`, `MindComponent`, `JobComponent`, `SkillsComponent`.
- [x] `DualFrontier.Systems/Pawn/NeedsSystem` (SLOW) — Hunger/Thirst/Rest/Comfort decay.
- [x] `DualFrontier.Systems/Pawn/MoodSystem` — формула `mood = f(needs)`,
      переход в MoodBreak.
- [x] `DualFrontier.Systems/Pawn/JobSystem` (NORMAL) — приоритеты по нуждам,
      `JobKind.Eat/Sleep/Idle`.
- [ ] `DualFrontier.Systems/Pawn/SocialSystem` — стаб.
- [ ] `DualFrontier.Systems/Pawn/SkillSystem` — стаб.
- [x] `DualFrontier.AI/Pathfinding/IPathfindingService`, `AStarPathfinding`
      (A* с лимитом 2000 итераций, без кэша), `NavGrid` (passability bitmap,
      cost map, SetTile).
- [x] `DualFrontier.Events/Pawn/DeathEvent`, `PawnSpawnedEvent`,
      `PawnMovedEvent`, `MoodBreakEvent`, `SkillGainEvent`.
- [x] `DualFrontier.Application/Scenario/ScenarioDef`, `ScenarioLoader`
      (JSON + `LoadDefault()`).
- [x] `MovementComponent` + `MovementSystem` — движение пешек по A*-маршруту,
      публикация `PawnMovedEvent`.
- [x] `NavGrid` инициализация из `GameBootstrap` (50×50, 50 препятствий).
- [x] Godot визуал — пешки двигаются по карте, `PawnMovedEvent`
      публикуется и обрабатывается.
- `DualFrontier.AI/Jobs/IJob`, `JobHaul`, `JobMeditate`.
- `DualFrontier.AI/BehaviourTree/BTNode`, `Selector`, `Sequence`, `Leaf`.

### Критерии приёмки

- Симуляция на 30 пешек, 100×100 тайлов, 60 FPS без заиканий.
- Пешка получает задачу через `JobComponent`, `JobSystem` выполняет её через behaviour tree.
- `NeedsSystem` снижает голод/сон, пешка ищет еду через `JobSystem`.
- Pathfinding работает, кэширует маршруты.

### Открытые задачи

- [ ] `SocialSystem` — полная реализация (сейчас стаб).
- [ ] `SkillSystem` — полная реализация (сейчас стаб).
- [ ] Доступ систем к `IGameServices` для публикации событий в шины
      (`MoodSystem` сейчас содержит заглушку вместо публикации
      `MoodBreakEvent`).

### Разблокирует

Производственную цепочку: без ходячей пешки нет крафта.

## Фаза 3.5 — Godot DevKit (короткая промежуточная)

Цель: Godot полностью работает как редактор + временный рантайм для тестирования.
Запускать игру можно через F5 в Godot и через `dotnet run` в Native — обе
реализации потребляют один и тот же `.dfscene`.
Архитектурный контекст: [VISUAL_ENGINE](./VISUAL_ENGINE.md).

### Что реализуем

- `src/DualFrontier.Presentation/addons/df_devkit/` — полноценный плагин:
  `DfDevKitPlugin`, `DFEntityMeta` с UI в инспекторе, `SceneExporter` с реальным
  обходом SceneTree, `TilemapExporter`, `EntityExporter`.
- `src/DualFrontier.Presentation/GodotRenderer.cs` — реализация `IRenderer`
  через стандартный Godot `_Process`.
- `src/DualFrontier.Presentation/GodotSceneLoader.cs` — реализация `ISceneLoader`
  через `FileAccess.Open(res://...)` и `System.Text.Json`.
- `src/DualFrontier.Presentation/GodotInputRouter.cs` — реализация `IInputSource`.
- Меню редактора: "Tools → Export .dfscene → выбрать путь".

### Критерии приёмки

- Godot Editor открывает `main.tscn`, редактирует, экспортирует в `.dfscene`.
- `GodotSceneLoader.Load("sample.dfscene")` возвращает `SceneDef` с тайлмапом
  и тремя сущностями (пешка + спаунер + маркер).
- F5 в Godot запускает игру с загруженной сценой, пешка живёт.
- `dotnet test` проходит все 43+ тестов плюс новые `SceneDefSerializationTests`.

### Разблокирует

Производственный пайплайн Godot → `.dfscene` → Native. Дальше Фаза 4
работает с реальными сценами, а не с хардкодом.

## ✅ Фаза 4 — Экономика (завершена)

Результат: склады, крафт-верстаки, электросеть, конвертор.
InventorySystem с кэшем, HaulSystem, ElectricGridSystem
с приоритетным распределением, ConverterSystem 30% КПД (зарегистрирована
в `GameBootstrap`, цикл с ElectricGrid разорван через `[Deferred]` событие).
Grimdark HUD с панелями ColonyPanel и PawnDetail.
82/82 тестов (включая Phase 4 покрытие).

### Что реализовано

- [x] `StorageComponent`, `WorkbenchComponent`
- [x] `PowerConsumerComponent`, `PowerProducerComponent`
- [x] `InventorySystem` (кэш + батчинг через `_freeSlotCache`/`_cacheDirty`,
      резервации через `[Deferred] ItemReservedEvent`)
- [x] `HaulSystem` — поиск source→dest по предмету (Phase 4: телепорт без
      pathfinding); per-Update reservation set предотвращает same-tick
      double-allocation
- [x] `ElectricGridSystem` — приоритетное распределение ватт, шина `IPowerBus`
- [x] `ConverterSystem` (КПД 30%) — зарегистрирована, публикует
      `[Deferred] ConverterPowerOutputEvent` на `IPowerBus`, разрывая
      компонентный цикл с ElectricGrid
- [x] `Events/Inventory/*` — все четыре события (`ItemAdded`, `ItemRemoved`,
      `ItemReserved`, `CraftRequest`); первые три помечены `[Deferred]`
- [x] `Events/Power/*` — `PowerRequest`, `PowerGranted`, `GridOverload`,
      `ConverterPowerOutputEvent` (`[Deferred]`)
- [x] Grimdark HUD — `GameHUD` (CanvasLayer), `ColonyPanel`, `PawnDetail`,
      `PawnStateReporterSystem`, `PawnStateCommand`
- [x] `IPowerBus` добавлена в `IGameServices` (non-breaking, см.
      [CONTRACTS](./CONTRACTS.md) §«Не ломающее → Добавление новой шины»)
- [x] `BridgeImplementationAttribute` введён в `DualFrontier.Contracts.Attributes`
      и применён ко всем системам со стаб-`OnInitialize` — Phase 5+
      регистрация теперь не падает

### Открытые задачи

Закрыто в v0.3 (см. истории коммитов раздела «Архитектурные правки v0.3» ниже).

### Критерии приёмки

- 100 пешек × 60 тиков × патрон-запрос: ≤100 сканов/сек (цель из 11.11).
- Крафт цепочка: ресурс → верстак → продукт → склад. *(Фаза 6 — `CraftSystem`)*
- Электросеть: генератор → провод → потребитель; overload на перегрузе. ✅
  (см. `tests/DualFrontier.Systems.Tests/Power/ElectricGridOverloadTests.cs`)
- Конвертор: 100W → 30W (магическая ветка получит ману в Phase 6); работает
  точечно, невыгоден в масштабе. ✅ (см.
  `tests/DualFrontier.Systems.Tests/Power/ConverterEfficiencyTests.cs`)

### Архитектурные правки v0.3 (закрытие технического долга)

- `[Deferred]`/`[Immediate]` доставка реализована в `DomainEventBus` (Q1).
- `IPowerBus` добавлена в `IGameServices`; `ElectricGridSystem` и
  `ConverterSystem` переведены на эту шину (Q2).
- `ItemAddedEvent`/`ItemRemovedEvent`/`ItemReservedEvent` помечены
  `[Deferred]`; `InventorySystem` подписывается на все три, мутации
  `StorageComponent` идут на следующей фазе через захваченный контекст —
  изоляция HaulSystem.writes=`[]` сохранена (Q3, Q4).
- `ConverterSystem.writes` ужат до `[]`; выход публикуется через
  `[Deferred] ConverterPowerOutputEvent`, цикл DAG разорван (Q6).
- Все стабы с `throw new NotImplementedException` в `OnInitialize` получили
  пустое тело + `[BridgeImplementation(Phase = N)]` (Q5) — разблокирует
  Phase 5/6/7 регистрацию.
- `HaulSystem` `return` после неуспешного `TryFindHaul` заменён на `continue`;
  `DomainEventBus.Subscribe` TOCTOU исправлен через `GetOrAdd`.

### Разблокирует

Бой и магию — обеим системам нужен инвентарь (патроны, кристаллы).
Phase 5 (бой) теперь имеет работающий `[Deferred] DeathEvent` и bridge-стабы
для Magic-систем, не падающие при регистрации.

## 🔨 Фаза 5 — Бой (текущая)

Цель: Combat Extended на базе шины.

### Что реализуем

- `DualFrontier.Components/Combat/WeaponComponent`, `ArmorComponent`, `AmmoComponent`, `ShieldComponent`, `HealthComponent`.
- `DualFrontier.Systems/Combat/CombatSystem` (FAST, фаза 1), `ProjectileSystem` (REALTIME), `DamageSystem` (FAST, фаза 2), `StatusEffectSystem`, `ShieldSystem`.
- `DualFrontier.Events/Combat/ShootAttemptEvent`, `AmmoIntent`, `AmmoGranted`, `AmmoRefused`, `DamageEvent`, `DeathEvent`, `StatusAppliedEvent`.

### Критерии приёмки

- Двухшаговая модель AmmoIntent → AmmoGranted работает через `IntentBatcher`.
- Damage model: типы урона (Heat, Sharp, Blunt, EMP, Toxic, Psychic, Stagger) применяются с учётом брони.
- Щиты: пул HP, регенерация, слабости.
- `DeathEvent` помечен `[Deferred]` — `MoodSystem` и `SocialSystem` получают его в следующей фазе.

### Параллельно: Native Runtime Bootstrap

Начинается разработка `DualFrontier.Presentation.Native`:
- `PackageReference` на Silk.NET.
- `NativeRenderer.Initialize` создаёт окно и GL контекст.
- `SpriteBatch`, `TilemapRenderer` — базовый рендер.
- `NativeSceneLoader` парсит тот же `.dfscene`.

Не блокирует основную работу Фазы 5 — Native живёт в отдельной сборке.

### Разблокирует

Магический бой: единая физика урона уже есть, магия будет просто использовать её.

## Фаза 6 — Магия

Цель: магическая колония с големами, ростом мага и эфирными срывами.

### Что реализуем

- `DualFrontier.Components/Magic/ManaComponent`, `SchoolComponent`, `EtherComponent`, `GolemBondComponent`.
- `DualFrontier.Systems/Magic/ManaSystem` (NORMAL, фаза 1), `SpellSystem` (FAST), `GolemSystem` (NORMAL), `EtherGrowthSystem` (SLOW, фаза 2), `RitualSystem` (RARE).
- `DualFrontier.Events/Magic/ManaIntent`, `ManaGranted`, `ManaRefused`, `SpellCastEvent`, `EtherSurgeEvent`, `GolemActivatedEvent`, `EtherLevelUpEvent`.
- `DualFrontier.AI/Jobs/JobCast`, `JobGolemCommand`, `JobMeditate`.

### Критерии приёмки

- 8 школ магии (Огонь/Лёд/Гром/Земля/Ветер/Вода/Тьма/Свет) с разными профилями урона по GDD 6.1.
- 5 уровней восприятия эфира по GDD 4.1, масштабирование урона по 6.2.
- Големы 5 типов по 5.1, расход манны постоянный, истощение мага останавливает голема.
- Эфирный срыв при работе с кристаллом выше уровня мага.
- Комбо-механики: Лёд→Земля, Молния→Металл, Вода→Молния (по 6.3).

### Разблокирует

Финальный контент — мир, фракции, события.

## Фаза 7 — Мир

Цель: живой мир вокруг колонии.

### Что реализуем

- `DualFrontier.Components/World/TileComponent`, `BiomeComponent`, `EtherNodeComponent`.
- `DualFrontier.Systems/World/BiomeSystem`, `WeatherSystem`, `MapSystem`.
- `DualFrontier.Systems/Faction/RelationSystem` (RARE), `TradeSystem`, `RaidSystem` (RARE).
- `DualFrontier.Events/World/EtherNodeChangedEvent`, `WeatherChangedEvent`, `RaidIncomingEvent`.

### Критерии приёмки

- Биомы влияют на ресурсы, погоду, проходимость.
- Эфирные узлы формируют радиусы покрытия для магической энергосети.
- Межфракционные отношения по матрице GDD 3.3; динамическое изменение через события.
- Рейды приходят с разной периодичностью и составом по уровню колонии.
- Торговля: караваны, обмен ресурсами, сезонность.

### Разблокирует

Полную игру. Дальнейшие итерации — контент-расширения через моды.

## Phase 6 (Магия) и Phase 7 (Мир): мост между фазами (v0.2 §12.6)

### Проблема

Фаза 5 (Бой) уже ссылается на магические компоненты: `CombatSystem` знает про `ManaComponent`, чтобы обрабатывать выстрелы с чарами через композитный запрос (см. [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md)). Но сама магия реализуется только в Фазе 6. Если ждать полной реализации магии, прежде чем тестировать магические стрелки — фаза 5 блокируется. Если частично вклинить магию в фазу 5 — нарушается принцип «фазы не пересекаются по владению кодом».

### Решение: bridge-реализация

В Фазе 5 подключается **bridge-реализация** магических систем — stub-granter, возвращающий успешный ответ без настоящей логики. `CombatSystem` видит полноценную `ManaSystem` через шину, успешно получает `ManaGranted` и проводит свои тесты магических стрелок. В Фазе 6 stub заменяется на полную реализацию без изменений в `CombatSystem`.

- **`ManaSystem` (bridge).** Всегда возвращает `ManaGranted` (или `ManaLeaseOpened` с фиктивным `LeaseId`), считая уровень маны равным 100%. `ManaComponent` не обновляется.
- **`GolemSystem` (bridge).** Возвращает бесконечный bond: любой `GolemOwnershipTransferRequest` принимается, `GolemBondComponent` не мутируется. Достаточно, чтобы `CombatSystem` корректно распознавал голема как союзника/противника.
- **`EtherSystem` (bridge).** Статический уровень эфира 50% — стабильная константа, позволяющая `EtherSurgeEvent` корректно формироваться на грани допуска в тестах.

Bridge-системы находятся в `DualFrontier.Systems/Magic/Bridge/` и помечены `[BridgeImplementation(Phase = 6)]` — анализатор предупреждает, если bridge-реализация остаётся в релизной сборке после Фазы 6.

### Критерии закрытия bridge'а

В Фазе 6 каждая bridge-система заменяется полной реализацией. Критерии:

- Все тесты, проходящие на bridge-версии, продолжают проходить на полной реализации.
- `[BridgeImplementation]`-пометка снимается, анализатор больше не предупреждает.
- Ни один публичный контракт (`ManaGranted`, `GolemOwnershipChanged`, `EtherSurgeEvent`) не меняет сигнатуру — `CombatSystem` и прочие потребители не требуют правок.

## Пост-релиз — развилка на движок

После закрытия Phase 7 и выхода игры в Steam кодовая база достигает точки
развилки. Игра продолжает развиваться в основной ветке как контент-проект.
Движок — generic ECS-ядро, накопленное за Phase 0–7, — форкается в
отдельный репозиторий и продолжает развиваться независимо как обобщённое
решение для других потенциальных потребителей.

### Что уходит в форк

Движковые сборки по определению из
[ARCHITECTURE §«Граница движок / игра»](./ARCHITECTURE.md#граница-движок--игра):

- `DualFrontier.Contracts` (с возможным переименованием — без префикса проекта).
- `DualFrontier.Core`.
- `DualFrontier.Core.Interop` + `native/DualFrontier.Core.Native/`.
- `DualFrontier.Presentation.Native`.
- Модинг-секция `DualFrontier.Application`.

История этих сборок реконструируется через scope-префиксы коммитов
(`core:`, `contracts:`, `interop:`, `native:`, `modding:`,
`presentation-native:`) одной командой `git log --grep`. Дисциплина
scope-префиксов на Phase 0–7 — прямая инвестиция в дешевизну этой
операции; см. [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md).

### Что остаётся в игре

Игровые сборки (`Components`, `Events`, `Systems`, `AI`, `Presentation`
Godot DevKit, игровой цикл `Application`) остаются в основном репозитории
и продолжают зависеть от движка как от внешней библиотеки.

### Условие осуществимости

Развилка проходит дёшево, только если граница движок/игра не деградировала
за Phase 4–7. Каждый коммит в этих фазах либо трогает движок, либо трогает
игру, но не оба сразу — это проверяется PR-чек-листом
([DEVELOPMENT_HYGIENE §«Чек-лист перед каждым PR»](./DEVELOPMENT_HYGIENE.md#чек-лист-перед-каждым-pr)).
Без этой дисциплины форк превращается в недели разбора смешанной истории.

### Связь с Phase 9 — Native Runtime

Phase 9 (см. ниже) технически завершает движок: своя точка входа, свой
рантайм, Godot отделяется от исполнения. Развилка на движок —
организационное событие, формально-параллельное Phase 9. Технически они
независимы: можно форкнуть движок до Phase 9 (с Godot DevKit как
зависимостью) и довести Native Runtime уже в форкнутом репозитории.

## Фаза 9 — Native Runtime

### Что реализуем

- `DualFrontier.Runtime` — собственный entry point,
  запускает GameLoop напрямую без Godot в цепочке.
- `IRenderer` — абстрактный интерфейс рендера.
- `IInputProvider` — абстрактный интерфейс ввода.
- `GodotBackend` — реализация через Godot GDExtension
  (рендер + ввод без Godot runtime в исполнении).
- Godot плагин — читает `.tscn` файлы и транслирует
  их в вызовы native runtime без Godot SceneTree.

### Почему это возможно

Архитектура уже готова к этому:
- Domain полностью отвязан от Godot (нет using Godot)
- GameLoop работает на чистом .NET потоке
- PresentationBridge абстрагирует рендер за IRenderCommand
- Simulation живёт без Godot — тесты это доказывают

### Разблокирует

- Полный контроль над runtime и производительностью
- Возможность портирования на любой рендер бэкенд
- Godot как инструмент контента, не как движок

### Когда

После завершения Фазы 7 и выхода в Steam.
Это отдельный большой проект, не блокирует релиз.

## См. также

- [ARCHITECTURE](./ARCHITECTURE.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
- [PERFORMANCE](./PERFORMANCE.md)
