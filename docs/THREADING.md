# Многопоточность

RimWorld однопоточный: все системы бегут по очереди. На мультиядерном железе 7 из 8 ядер простаивают. Dual Frontier строит граф зависимостей один раз при старте и запускает несвязанные системы параллельно. Без изменения игрового кода — только через декларацию `[SystemAccess]`.

## DependencyGraph

Каждая система объявляет, какие компоненты она читает и какие пишет. `DependencyGraph` при старте собирает все декларации и строит граф:

- Две системы конфликтуют, если одна пишет компонент, который другая читает или пишет.
- Две системы, пишущие разные компоненты, не конфликтуют.
- Две системы, читающие один и тот же компонент, не конфликтуют.

```csharp
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)
)]
public sealed class CombatSystem : SystemBase { /* ... */ }
```

Атрибут читается один раз рефлексией при регистрации системы — во время игры повторного парсинга нет, декларация становится частью графа в памяти.

## Топологическая сортировка в фазы (v0.2: 5 фаз)

Планировщик применяет топологическую сортировку к графу и группирует несвязанные системы в фазы. Каждая фаза — набор систем, исполняемых параллельно. В v0.2 фиксированный каркас — пять фаз, через которые проходит каждый тик.

```
┌───────────────────────────────────────────────────────────────┐
│ Phase 1 — Input & Sensors                                      │
│   Читают мир, публикуют Intent'ы.                              │
│   CombatSystem, SpellSystem, SensorSystem, JobSystem.          │
└──────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ Phase 2 — Intent Collection                                    │
│   CompositeResolutionSystem ждёт multi-bus ответы по           │
│   TransactionId; формирует итоговые ShootGranted/ShootRefused. │
└──────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ Phase 3 — Resolution                                           │
│   Resolution-системы решают granted/refused:                   │
│   InventorySystem (AmmoIntent), ManaSystem (ManaIntent и       │
│   lease-открытия/дрен), TargetResolutionSystem.                │
└──────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ Phase 4 — Apply & Damage                                       │
│   ComboResolutionSystem объединяет DamageIntent[] → DamageEvent│
│   в стабильном порядке. Применение статусов, смерть.           │
└──────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ Phase 5 — Feedback snapshot                                    │
│   Mana[N] → ManaSnapshot[N]; EtherSnapshot, HealthSnapshot.    │
│   Читаются в Phase 1 следующего тика системами-наблюдателями.  │
└───────────────────────────────────────────────────────────────┘
```

Снимок `Mana[N-1]` читается `GolemSystem`'ом на следующем тике — это разрешает петлю «ManaSystem пишет, GolemSystem читает» без конфликта по компоненту. Подробности — в [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md).

Внутри каждой фазы несвязанные системы исполняются параллельно через `Parallel.ForEach` (см. ниже). Граф строится один раз при старте игры, поэтому накладные расходы планирования нулевые в рантайме. Изменения декларации требуют перезапуска — но компиляция C# и так заставляет перезапуск при правке кода.

## Параллельное исполнение

`ParallelSystemScheduler` запускает все системы одной фазы через `Parallel.ForEach` с ограничением по числу ядер: `N-2` для игры, чтобы оставить ядра под Godot main thread и системный планировщик ОС.

```csharp
// Упрощённый скелет — полная версия в Scheduling/ParallelSystemScheduler.cs
void RunPhase(SystemPhase phase, float delta)
{
    Parallel.ForEach(phase.Systems, system =>
    {
        SystemExecutionContext.PushContext(system);
        try { system.Update(delta); }
        finally { SystemExecutionContext.PopContext(); }
    });
    // Доставка [Deferred] событий между фазами.
    FlushDeferredEvents();
}
```

Каждая система получает свой `SystemExecutionContext` через `ThreadLocal<T>`. Контекст хранит список разрешённых компонентов и имя активной шины. Доступ сторожа — O(1) по HashSet.

## TickRates

Не все системы должны тикать каждый кадр. Реальный кадр — около 16 мс при 60 FPS. `[TickRate]` задаёт частоту.

| Tick     | Период       | Применение                                          |
|----------|--------------|-----------------------------------------------------|
| REALTIME | каждый кадр  | Физика снарядов, ProjectileSystem.                  |
| FAST     | 3 кадра      | CombatSystem, DamageSystem — отзывчивость боя.      |
| NORMAL   | 15 кадров    | JobSystem, ManaSystem, SkillSystem.                  |
| SLOW     | 60 кадров    | NeedsSystem, MoodSystem, EtherGrowthSystem.          |
| RARE     | 3600 кадров  | SocialSystem, RaidSystem, WeatherSystem.             |

```csharp
[SystemAccess(reads: [...], writes: [...])]
[TickRate(TickRates.SLOW)]
public sealed class NeedsSystem : SystemBase { /* ... */ }
```

`TickScheduler` ведёт счётчик фреймов и вызывает `Update` только у систем, чей счётчик обнулился. Разные системы в одной фазе могут иметь разный тик — планировщик учитывает это, не нарушая граф.

## Правило: async запрещён

В системах `async`/`await` запрещены. Причина в `SystemExecutionContext`: он живёт в `ThreadLocal`, привязан к текущему потоку, и `await` переключает выполнение на другой поток после возврата — на новом потоке контекст другой, сторож не увидит декларации, и даже если не увидит — запись в компонент произойдёт без синхронизации с графом зависимостей.

Что делать вместо `async`:

- Долгая работа (pathfinding, serialization) — через `Application` слой, в отдельном потоке, с результатом, возвращённым через событие или команду.
- I/O — только в Application, никогда в системах.
- Ожидание — не нужно: планировщик сам вызовет систему в следующей фазе или следующем тике.

Сторож при DEBUG ловит `Task`, `ValueTask`, `await` через анализ stack-trace и кидает `IsolationViolationException` с сообщением: `"Система 'XXX' использует async. Вынеси работу в Application."`.

## Отладка конфликтов

Частая ошибка при добавлении системы — цикл или конфликт декларации. Сообщения планировщика:

### Цикл в графе

```
[SCHEDULER ERROR] Cyclic dependency detected:
  CombatSystem (W:Health) → DamageSystem (R:Health, W:Health)
  DamageSystem (W:Health) → CombatSystem (R:Health)
Resolve: разорви цикл через [Deferred] событие.
```

Решение: одна из систем публикует `[Deferred]` событие вместо прямой записи, доставка на границе фаз разрывает цикл.

### Нарушение декларации в рантайме

```
[IsolationViolationException]
Система 'PoisonSystem'
обратилась к 'BloodComponent'
без декларации доступа.
Добавь: [SystemAccess(reads: new[]{typeof(BloodComponent)})]
```

Это не падение планировщика, а падение сторожа — смотри [ISOLATION](./ISOLATION.md).

### Две системы пишут один компонент в одной фазе

```
[SCHEDULER ERROR] Write conflict in phase 2:
  PoisonSystem writes HealthComponent
  DamageSystem writes HealthComponent
Resolve: одна из систем должна читать, не писать, или разнести по фазам.
```

Решение: либо одна система становится читателем, либо одна из них уходит в более позднюю фазу через `[Deferred]` событие.

## См. также

- [ECS](./ECS.md)
- [ISOLATION](./ISOLATION.md)
- [EVENT_BUS](./EVENT_BUS.md)
- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md) — Phase 5 и чтение `Mana[N-1]`.
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md) — Phase 2, ожидание multi-bus ответов.
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md) — Phase 4, упорядочивание `DamageIntent`.
