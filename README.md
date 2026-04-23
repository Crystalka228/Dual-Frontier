# Dual Frontier

**Dual Frontier** — колониальный симулятор на Godot 4 + C# с двумя параллельными технологическими ветками (индустриальная и аркано-магическая), обусловленными **биологией** рас, а не идеологическим выбором игрока. Люди-инженеры строят RimWorld-подобные колонии на 10–20 пешек; фэнтезийные расы растят ~10 магов, каждый из которых — невосполнимая сила.

Игра построена на переработанной архитектуре которая решает три проблемы оригинала: медленное сканирование мира системами, однопоточность, и полную отсутствующую изоляцию модов.

---

## Три архитектурных принципа

1. **Контрактные шины.** Система никогда не лезет в чужие данные напрямую — только через маркер-события (`IEvent`, `IQuery`, `ICommand`) на доменных шинах. Поиск «патронов на складе» — это `AmmoIntent` в `InventoryBus`, а не `O(n)` скан инвентаря.
2. **Многопоточность по декларации.** Каждая система декларирует `[SystemAccess(reads, writes, bus)]`. Планировщик строит граф зависимостей один раз и исполняет несвязанные системы параллельно в одной фазе.
3. **Изоляция модов.** Каждый мод загружается в отдельный `AssemblyLoadContext`. Мод физически не имеет доступа к `DualFrontier.Core` — только к `DualFrontier.Contracts`. Моды взаимодействуют через публикуемые `IModContract`.

Главный закон: **нарушение изоляции = немедленный краш с диагностикой**, а не тихая порча состояния.

---

## Слои

```
┌─────────────────────────────────────────────────────┐
│ PRESENTATION    Godot SceneTree, UI, ввод           │
│                 ТОЛЬКО main thread                  │
├─────────────────────────────────────────────────────┤
│ APPLICATION     GameLoop, Save, ScenarioLoader      │
│                 Очередь команд Domain → Presentation│
├─────────────────────────────────────────────────────┤
│ DOMAIN          Systems / Components / Entities     │
│                 Многопоточно. Godot не знает.       │
├─────────────────────────────────────────────────────┤
│ INFRASTRUCTURE  EventBus, Scheduler, SpatialGrid    │
└─────────────────────────────────────────────────────┘
```

Каждый слой знает только о слое ниже. `Presentation` получает данные только через очередь из `Application` — никогда не вызывает `Systems` напрямую.

---

## Состав решения

| Сборка | Слой | Назначение |
|---|---|---|
| `DualFrontier.Contracts` | All | Публичные интерфейсы, атрибуты, маркер-типы. Единственное, что видит мод. |
| `DualFrontier.Core` | Infra/Domain | ECS ядро, планировщик, доменные шины. `internal` — снаружи недоступно. |
| `DualFrontier.Components` | Domain | Чистые POCO-компоненты, никакой логики. |
| `DualFrontier.Events` | Domain | `record`-события, запросы, команды, intents. |
| `DualFrontier.Systems` | Domain | Игровые системы. `[SystemAccess]` декларирует R/W. |
| `DualFrontier.AI` | Domain | Behavior Tree, Job-ы, Pathfinding. |
| `DualFrontier.Application` | App | GameLoop, Save/Load, ModLoader, PresentationBridge. |
| `DualFrontier.Presentation` | Godot | Godot-specific код. Единственная сборка, которой разрешён `using Godot`. |
| `DualFrontier.WorldFields` | Infra | Изолированный модуль GPU/CPU pipeline для полей мира. Виден Domain только через `IWorldFieldCompute`. |

---

## Статус

**Фаза 0 — Scaffolding завершён.** Проект компилируется, все интерфейсы и стабы на месте. Реализация тел методов идёт по фазам согласно [docs/ROADMAP.md](docs/ROADMAP.md).

Следующие фазы:
- **Фаза 1 — Core.** World, ComponentStore, SystemExecutionContext, DomainEventBus, ParallelSystemScheduler.
- **Фаза 2 — Верификация.** Тесты изоляции: сторож ловит нарушения, мод не видит внутренности, параллельные системы не конфликтуют.
- **Фаза 3–7.** Пешки, экономика, бой, магия, мир.

---

## Документация

- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) — слои, принципы, правила зависимостей.
- [docs/CONTRACTS.md](docs/CONTRACTS.md) — шины событий, маркер-интерфейсы, эволюция контрактов.
- [docs/ECS.md](docs/ECS.md) — World, Entity, Component, System.
- [docs/EVENT_BUS.md](docs/EVENT_BUS.md) — синхронная/[Deferred]/[Immediate] доставка, Intent→Granted/Refused.
- [docs/THREADING.md](docs/THREADING.md) — граф зависимостей, фазы, TickRates.
- [docs/ISOLATION.md](docs/ISOLATION.md) — `SystemExecutionContext`, сторож, типы нарушений.
- [docs/MODDING.md](docs/MODDING.md) — `IMod`, `AssemblyLoadContext`, `IModContract`.
- [docs/GODOT_INTEGRATION.md](docs/GODOT_INTEGRATION.md) — `PresentationBridge`, правила main thread.
- [docs/PERFORMANCE.md](docs/PERFORMANCE.md) — целевые метрики, профилирование.
- [docs/CODING_STANDARDS.md](docs/CODING_STANDARDS.md) — naming, комментарии, структура файлов.
- [docs/TESTING_STRATEGY.md](docs/TESTING_STRATEGY.md) — unit/integration/isolation/modding.
- [docs/ROADMAP.md](docs/ROADMAP.md) — порядок реализации по фазам.
- [docs/WORLDFIELDS.md](docs/WORLDFIELDS.md) — GPU pipeline полей мира, модуль `DualFrontier.WorldFields`.
- [docs/GPU_COMPUTE.md](docs/GPU_COMPUTE.md) — массовая физика на GPU: снаряды, коллизии, flood-fill.

---

## Требования

- **.NET 8.0 SDK** (C# 12, `Nullable` enabled, `ImplicitUsings`, `TreatWarningsAsErrors`).
- **Godot 4.3+** с поддержкой C# (mono / .NET build).
- **JetBrains Rider** 2024.1+ или **Visual Studio 2022** 17.8+.

---

## Билд

```bash
dotnet restore
dotnet build DualFrontier.sln
```

Godot-проект открывается через `src/DualFrontier.Presentation/project.godot` после успешного `dotnet build` всех ссылочных сборок.

---

## Лицензия

TBD (placeholder — лицензия будет выбрана до публичного релиза).
