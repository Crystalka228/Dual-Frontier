# Attributes — Декларативные атрибуты

## Назначение
Атрибуты, которыми системы декларируют свои зависимости и поведение во
время исполнения. Планировщик строит граф параллелизма и тиков, читая эти
атрибуты рефлексией при старте.

## Зависимости
- `System` (BCL)

## Что внутри
- `SystemAccessAttribute.cs` — декларация READ/WRITE компонентов и имени шины.
  Основа графа зависимостей и сторожа изоляции.
- `DeferredAttribute.cs` — событие доставляется в следующей фазе планировщика.
- `ImmediateAttribute.cs` — событие прерывает текущую фазу ради мгновенной доставки.
- `TickRateAttribute.cs` — частота вызова `Update` системы (REALTIME/FAST/NORMAL/SLOW/RARE).

## Правила
- Если на системе нет `[SystemAccess]` — планировщик поднимает её как
  "пишет во все" и блокирует параллелизм. Это намеренный fail-safe.
- `[Deferred]` и `[Immediate]` — взаимоисключающие. Если не указан ни один —
  событие доставляется синхронно в текущей фазе.
- `TickRateAttribute` применяется только к системам (не к компонентам или событиям).

## Примеры использования
```csharp
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat))]
[TickRate(TickRates.FAST)]
public sealed class CombatSystem : SystemBase { /* ... */ }

[Deferred]
public sealed record DeathEvent(EntityId Who) : IEvent;
```

## TODO
- [ ] Фаза 1 — добавить анализатор Roslyn, кидающий CS-warning если у системы
      нет `[SystemAccess]`.
- [ ] Фаза 2 — добавить `[Phase(int)]` для ручного оверрайда фазы в
      диагностических целях.
