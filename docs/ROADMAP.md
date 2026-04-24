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

## 🔨 Фаза 4 — Экономика (текущая)

Цель: производство, склад, энергосеть.

### Что реализуем

- `DualFrontier.Components/Building/StorageComponent`, `WorkbenchComponent`, `PowerConsumerComponent`, `PowerProducerComponent`.
- `DualFrontier.Systems/Inventory/InventorySystem` (кэш + батчинг), `HaulSystem`, `CraftSystem`.
- `DualFrontier.Systems/Power/ElectricGridSystem`, `ConverterSystem` (КПД 30% по GDD 9).
- `DualFrontier.Events/Inventory/*`, `Events/Power/*`.

### Критерии приёмки

- 100 пешек × 60 тиков × патрон-запрос: ≤100 сканов/сек (цель из 11.11).
- Крафт цепочка: ресурс → верстак → продукт → склад.
- Электросеть: генератор → провод → потребитель; overload на перегрузе.
- Конвертор: 100W → 30 манны и обратно; работает точечно, невыгоден в масштабе.

### Разблокирует

Бой и магию — обеим системам нужен инвентарь (патроны, кристаллы).

## Фаза 5 — Бой

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

## Phase 5 — Магия и Phase 6 — Мир: мост между фазами (v02 §12.6)

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

После завершения Phase 7 и релиза Dual Frontier проект разветвляется. Игровая ветка продолжает жить контент-обновлениями и модами. Параллельно создаётся форк движковой части как самостоятельный продукт для внешнего использования. Эта развилка — не «возможно, когда-нибудь», а **запланированное** следствие того, как проект строился с Phase 0: граница движок/игра поддерживалась явно (см. [ARCHITECTURE §«Граница движок / игра»](./ARCHITECTURE.md#граница-движок--игра)), scope-префиксы в коммитах (см. [CODING_STANDARDS §«Сообщения коммитов»](./CODING_STANDARDS.md#сообщения-коммитов)) позволяют одной командой выделить движковую историю.

### Что уходит в движковый форк

Исключительно движковые сборки:

- `DualFrontier.Contracts`
- `DualFrontier.Core`
- `DualFrontier.Core.Interop` + `native/DualFrontier.Core.Native/`
- `DualFrontier.Presentation.Native`
- Модинг-секция `DualFrontier.Application` (`ModIntegrationPipeline`, `ContractValidator`, `ModRegistry`, `ModLoader`, `RestrictedModApi`, `PresentationBridge`)
- Все тесты на эти сборки

Переименование: `DualFrontier.*` → неймспейс форка (имя выбирается при развилке, не сейчас).

### Что остаётся в игровой ветке

- `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Systems`, `DualFrontier.AI`
- Игровая часть `Application` (`GameLoop`, `FrameClock`, `ScenarioLoader`, сценарии)
- `DualFrontier.Presentation` (Godot DevKit)
- Весь контент Phase 4–7, ассеты, моды

Игровая ветка после форка зависит от движкового форка как от внешнего пакета — тот же самый API, но через nuget/submodule, а не через `ProjectReference`.

### Что нужно добавить в движок для «продукта»

Анализ на текущей ветке показал пробелы, которые во время разработки игры закрывать нет смысла (они не нужны для релиза Dual Frontier), но которые обязательны для внешнего использования движка:

- Asset pipeline (конвертеры sprite atlas / звука / анимации; `.dfscene` сейчас — вручную собранный JSON).
- Build & packaging для Windows/Linux/macOS.
- Hot-reload модов (сейчас `ModIntegrationPipeline` требует рестарта сессии).
- Replay/determinism tools (упомянуты в [THREADING](./THREADING.md), но не реализованы).
- Cross-platform CI для `DualFrontier.Core.Native` (сейчас CMake на одной платформе разработчика).
- Документация «как сделать свою игру на этом движке» (есть [MODDING](./MODDING.md) для модера — это другое).

Эти задачи — **не** часть текущего roadmap Dual Frontier. Они появляются только **после** форка, в движковом репозитории.

### Критерий готовности к форку

Форк имеет смысл делать тогда, когда:

- Phase 7 закрыта, игра в релизе и прошла стадию «первых багфиксов» (1–3 месяца после релиза).
- Движковая часть выдержала все Phase 4–7 без появления `using DualFrontier.{Components,Systems,Events,AI}` в `Core`/`Contracts`/`Interop`/модинге. Это проверяется на каждом PR как обычная архитектурная гигиена — см. [ARCHITECTURE §«Граница движок / игра»](./ARCHITECTURE.md#граница-движок--игра).
- Моддинг-инфраструктура (`ModIntegrationPipeline`, `ContractValidator`) имеет хотя бы 2–3 реальных мода на ней, а не только `mods/DualFrontier.Mod.Example`.

До этого момента движковой ветки как отдельного репозитория не существует — параллельное обсуждение ведётся здесь, на ветке `claude/cpp-core-experiment-cEsyH`, и в документах, помеченных как движковые ([NATIVE_CORE_EXPERIMENT](./NATIVE_CORE_EXPERIMENT.md), [VISUAL_ENGINE](./VISUAL_ENGINE.md), [MODDING](./MODDING.md), [MOD_PIPELINE](./MOD_PIPELINE.md)).

## См. также

- [ARCHITECTURE](./ARCHITECTURE.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
- [PERFORMANCE](./PERFORMANCE.md)
