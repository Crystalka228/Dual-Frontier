# Переходы владения големом

Голем принадлежит магу через `GolemBondComponent`. Связь не бинарная «есть/нет» — она проходит через несколько состояний: активная, оспариваемая, брошенная, передаваемая. v0.2 фиксирует полный список состояний, разрешённые переходы и правила их выполнения.

## Состояния

- **Bonded** — нормальное состояние. У голема есть живой маг-владелец, мана капает из его пула, голем выполняет команды.
- **Contested** — связь оспаривается: другой маг проводит ритуал перехвата. Владелец текущий, но таймер `TicksSinceContested` тикает.
- **Abandoned** — владелец мёртв или снял связь намеренно. Голем стоит бездействующим и доступен для нового bond'а.
- **Transferred** — промежуточное состояние в течение одного тика: связь уже разорвана с прежним владельцем, но новый ещё не принял её формально. Используется только для корректного порядка `[Deferred]` событий.

## Таблица переходов

| Из            | В             | Триггер                                                         |
|---------------|---------------|------------------------------------------------------------------|
| `Bonded`      | `Contested`   | Другой маг начал ритуал перехвата — атакует bond.               |
| `Contested`   | `Bonded`      | Таймер `TicksSinceContested` истёк, исходный владелец удержал.  |
| `Contested`   | `Transferred` | Атакующий маг довёл ритуал до конца, bond переходит к нему.      |
| `Transferred` | `Bonded`      | Новый владелец принял bond (один тик на оформление).             |
| `Bonded`      | `Abandoned`   | Владелец умер (`DeathEvent`) или добровольно разорвал связь.    |
| `Contested`   | `Abandoned`   | Владелец умер во время оспаривания — bond освобождается.         |
| `Abandoned`   | `Bonded`      | Новый маг выполнил ритуал привязки.                              |

Все остальные переходы запрещены — попытка выполнить такой переход кидает `InvalidOwnershipTransitionException` в DEBUG и тихо игнорируется в RELEASE (с инкрементом счётчика ошибок для диагностики).

## Диаграмма состояний

```
                  ritual succeeds
   ┌──────────────────────────────────────────────┐
   │                                              │
   ▼                                              │
┌──────────┐   contender starts ritual   ┌─────────────┐
│ Bonded   │ ───────────────────────────►│ Contested   │
│          │                             │             │
│          │◄────── timer expires ───────│             │
└────┬─────┘   (owner held)              └──────┬──────┘
     │                                          │
     │ owner dies            contender wins     │
     │ or releases           ritual             │
     ▼                                          ▼
┌────────────┐                          ┌──────────────┐
│ Abandoned  │                          │ Transferred  │
│            │                          │ (one tick)   │
└────┬───────┘                          └──────┬───────┘
     │                                         │
     │ new mage runs bond ritual               │ new owner accepts
     │                                         │
     └───────────────► Bonded ◄────────────────┘
```

## События

Любой переход публикует `GolemOwnershipChanged` (`[Deferred]`). Поля события:

- `GolemId` — `EntityId` голема.
- `PreviousOwnerId` — `EntityId?` прежнего владельца (null, если из `Abandoned`).
- `NewOwnerId` — `EntityId?` нового владельца (null, если в `Abandoned`).
- `PreviousMode` — состояние до перехода (`OwnershipMode`).
- `NewMode` — состояние после перехода (`OwnershipMode`).

Отдельно существует входящее событие `GolemOwnershipTransferRequest` — запрос на инициацию ритуала перехвата. Обработка: `GolemSystem` проверяет допустимость (голем не в `Transferred`, атакующий действительно маг, цель в радиусе) и либо переводит в `Contested`, либо отвечает отказом через `GolemOwnershipRefused` (TODO Фаза 6).

## Компонент `GolemBondComponent`

```csharp
public sealed class GolemBondComponent : IComponent
{
    public EntityId? BondedMage;       // null, если Abandoned
    public OwnershipMode Mode;         // Bonded | Contested | Abandoned | Transferred
    public int TicksSinceContested;    // счётчик таймера; 0, если не Contested
    public float BondStrength;         // 0..1, учитывается в ритуале перехвата
}
```

`BondStrength` растёт с временем привязки и успешными общими боями; уменьшается при длительном простое и после каждого оспаривания. Формула — в `DualFrontier.Systems/Magic/GolemBondStrength.cs` (TODO Фаза 6).

## Правило мутации

**Только `GolemSystem` имеет право писать `GolemBondComponent`.** Другие системы наблюдают за событиями `GolemOwnershipChanged`, но не мутируют компонент напрямую. Декларация:

```csharp
[SystemAccess(
    reads:  new[] { typeof(ManaSnapshotComponent), typeof(PositionComponent) },
    writes: new[] { typeof(GolemBondComponent) },
    bus:    nameof(IGameServices.Magic)
)]
public sealed class GolemSystem : SystemBase { /* ... */ }
```

Попытка другой системы декларировать `writes: GolemBondComponent` валит планировщик на старте — write-конфликт, см. [THREADING](./THREADING.md).

Чтение маны мага в `GolemSystem` идёт через снимок предыдущего тика — иначе возникает петля с `ManaSystem`. Подробности в [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md).

## См. также

- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md) — почему `GolemSystem` читает `ManaSnapshot`.
- [EVENT_BUS](./EVENT_BUS.md) — `[Deferred]`-режим для `GolemOwnershipChanged`.
- [THREADING](./THREADING.md) — write-конфликты и декларация доступа.
- [ROADMAP](./ROADMAP.md) — Фаза 6 (Магия) как место реализации.
