# DualFrontier.Systems

## Назначение
Вся игровая логика Dual Frontier. Каждая система — это потомок
`SystemBase`, декларирующий свои READ/WRITE зависимости через
`[SystemAccess]` и частоту обновления через `[TickRate]`. Шедулер
Core использует эти декларации для построения графа зависимостей и
параллельного исполнения фаз (см. TechArch 11.4–11.6).

## Зависимости
- `DualFrontier.Contracts` — `SystemAccessAttribute`, `TickRateAttribute`,
  `IGameServices` и доменные шины (`ICombatBus`, `IMagicBus`, …),
  `EntityId`, `IEvent`, `IComponent`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`, исполнительный
  контекст и планировщик.
- `DualFrontier.Components` — все доменные компоненты, которые
  системы читают и пишут.
- `DualFrontier.Events` — события, которые системы публикуют и
  на которые подписываются.

## Что внутри
- `Pawn/` — нужды, работа, настроение, социалка, навыки.
- `Magic/` — мана, заклинания, големы, эфирные поля, ритуалы.
- `Combat/` — инициация боя, снаряды, урон, щиты, эффекты.
- `Inventory/` — склад, хаулы, крафт.
- `Power/` — электросеть, эфирная сеть, конвертеры.
- `World/` — карта, погода, биомы.
- `Faction/` — отношения, торговля, рейды.

## Правила
- Каждая система — `public sealed class XSystem : SystemBase`.
- Каждая система ОБЯЗАНА иметь `[SystemAccess(reads, writes, bus)]`
  и `[TickRate(TickRates.X)]`.
- Запрещено обращаться к `World` напрямую — только через
  `SystemExecutionContext` в `SystemBase`.
- Запрещено асинхронное API (`async`/`await`, `Task.Run`) —
  ломает `ThreadLocal` сторож изоляции (см. THREADING).
- Межсистемное общение — только через шины (`IGameServices.X`)
  и запросы компонентов у своих READ/WRITE множеств.
- Имя свойства шины в `bus:` задаётся через
  `nameof(IGameServices.Combat)` — не строкой.

## Примеры использования
```csharp
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.FAST)]
public sealed class CombatSystem : SystemBase { /* ... */ }
```

## TODO
- [ ] Реализовать тела `Update` во всех системах после того,
      как `SystemBase` и `SystemExecutionContext` будут готовы.
- [ ] Подписаться на нужные события в `Subscribe()` каждой системы.
- [ ] Покрыть тестами графы зависимостей (конфликт READ/WRITE → ошибка).
- [ ] Добавить интеграционные тесты "одна фаза — один проход шины".
