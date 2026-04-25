# Dual Frontier — исследовательский проект

**Dual Frontier** — методический эксперимент: проверка гипотезы, что один разработчик может построить сложный игровой движок (многопоточный ECS, моддинг через формальные контракты, native interop) через структурированную работу с LLM. Артефакт принимает форму колониального симулятора с двумя параллельными технологическими ветками — индустриальной и аркано-магической, обусловленными биологией рас, а не идеологическим выбором игрока. Но **движок и методика** — основной результат исследования. Игра — test case, на котором проверяется гипотеза.

---

## Гипотеза эксперимента

Публичный дискурс про LLM-разработку колеблется между двумя полюсами: «vibe coding для прототипов» и «AI заменит сеньоров». Этот проект проверяет третью модель: **человек как архитектор контрактов, LLM как исполнитель в строгих контрактных рамках**.

Falsifiable утверждение: **рабочая игра**, построенная соло через эту методику, с измеренной производительностью pipeline, частотой дефектов и архитектурной целостностью на длинной дистанции.

Полное описание подхода — в [docs/METHODOLOGY.md](docs/METHODOLOGY.md) *(в работе)*.

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
| `DualFrontier.Persistence` | Infra | Snapshot-кодеры, RLE, range encoding, StringPool. Не зависит от Godot. |

---

## Состояние эксперимента

*Обновлено: 2026-04-25*

| Фаза | Статус | Тесты | Заметки |
|---|---|---|---|
| Core ECS | ✅ Завершена | 49/49 | SparseSet, параллельный планировщик |
| Верификация | ✅ Завершена | — | Isolation guard, ContractValidator |
| Пешки | ✅ Завершена | 11/11 | A* pathfinding, Godot-bridge |
| Экономика + HUD | ✅ Завершена | 5/5 | Inventory, jobs, needs, Grimdark HUD |
| Persistence (scaffold) | 🔨 Phase 5 | 4/4 | TileEncoder/RLE, ComponentEncoder, EntityEncoder, StringPool |
| Бой | ⏭ Phase 5 | — | Следующая |
| Магия + мир | ⏭ Phases 6–7 | — | Запланировано |

**Снимок движка:** 65/65 тестов проходит, 0 известных production-багов, 168 коммитов за 5 дней, 296k токенов израсходовано (≈30% месячного бюджета Pro-подписки).

Полный план — в [docs/ROADMAP.md](docs/ROADMAP.md).

---

## Опубликованные артефакты исследования

### Методика

- [docs/METHODOLOGY.md](docs/METHODOLOGY.md) — трёхагентный pipeline (Sonnet-исполнитель, Opus-ревьюер, человек-архитектор), контракты как IPC между агентами, цикл верификации. *(в работе)*
- [docs/DEVELOPMENT_HYGIENE.md](docs/DEVELOPMENT_HYGIENE.md) — правила дисциплины и машинно-проверяемые инварианты.
- [docs/CODING_STANDARDS.md](docs/CODING_STANDARDS.md) — naming, комментарии, структура файлов, scope-префиксы коммитов.
- [docs/TESTING_STRATEGY.md](docs/TESTING_STRATEGY.md) — unit / integration / isolation / modding.

### Архитектура (положительные результаты)

- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) — слои, принципы, правила зависимостей.
- [docs/ECS.md](docs/ECS.md) — World, Entity, Component, System.
- [docs/EVENT_BUS.md](docs/EVENT_BUS.md) — синхронная/[Deferred]/[Immediate] доставка, Intent → Granted/Refused.
- [docs/THREADING.md](docs/THREADING.md) — граф зависимостей, фазы, TickRates.
- [docs/ISOLATION.md](docs/ISOLATION.md) — `SystemExecutionContext`, сторож, типы нарушений.
- [docs/MODDING.md](docs/MODDING.md) — `IMod`, `AssemblyLoadContext`, `IModContract`.
- [docs/MOD_PIPELINE.md](docs/MOD_PIPELINE.md) — двухфазная валидация, атомарная пересборка графа.
- [docs/CONTRACTS.md](docs/CONTRACTS.md) — шины событий, маркер-интерфейсы, эволюция контрактов.
- [docs/GODOT_INTEGRATION.md](docs/GODOT_INTEGRATION.md) — `PresentationBridge`, правила main thread.
- [docs/PERFORMANCE.md](docs/PERFORMANCE.md) — целевые метрики, профилирование.

### Эксперименты (с честно зафиксированными исходами)

- [docs/NATIVE_CORE_EXPERIMENT.md](docs/NATIVE_CORE_EXPERIMENT.md) — **отрицательный результат:** native C++ ядро через per-element P/Invoke проиграло managed-реализации (NativeAdd10k: ratio 3.92×). Критерий переформулирован с mean latency на p99 / GC pause / long-run drift; решение по batch-API отложено до Phase 9.
- [docs/GPU_COMPUTE.md](docs/GPU_COMPUTE.md) — **отложенное решение:** CPU-реализация `ProjectileSystem` справляется на текущих нагрузках; порог переключения на GPU зафиксирован в stress-сценарии «Битва богов» (5 000 одновременных снарядов).

### Расширения v0.2 (контракты для боя и магии)

- [docs/RESOURCE_MODELS.md](docs/RESOURCE_MODELS.md) — Intent vs Lease.
- [docs/COMPOSITE_REQUESTS.md](docs/COMPOSITE_REQUESTS.md) — двухфазный commit для составных запросов.
- [docs/FEEDBACK_LOOPS.md](docs/FEEDBACK_LOOPS.md) — Mana[N-1] snapshot для разрыва циклов.
- [docs/COMBO_RESOLUTION.md](docs/COMBO_RESOLUTION.md) — детерминированная сортировка по EntityId + DamageKind ordinal.
- [docs/OWNERSHIP_TRANSITION.md](docs/OWNERSHIP_TRANSITION.md) — состояния голема, переходы владения.

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

Godot-проект открывается через `src/DualFrontier.Presentation/project.godot` после успешной сборки всех ссылочных сборок.

---

## Лицензия

Этот проект распространяется под лицензией **[PolyForm Noncommercial 1.0.0](LICENSE)**.

На практике:

- ✅ **Изучение, форк, эксперименты, личное использование** — разрешены свободно.
- ✅ **Написание модов и их распространение** — разрешено свободно.
- ✅ **Использование в исследовательских, образовательных, некоммерческих целях** — разрешено свободно.
- ❌ **Коммерческое использование кода движка** в собственных продуктах — требует отдельного соглашения с автором.

Игровой контент (геймплейные системы, баланс, нарратив, арт), создаваемый поверх движка, не входит в этот публичный репозиторий и распространяется отдельно.

Для вопросов о коммерческом лицензировании или сотрудничестве — открыть issue в этом репозитории.
