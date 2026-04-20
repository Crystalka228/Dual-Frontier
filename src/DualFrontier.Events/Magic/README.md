# Magic Events

## Назначение
События магической подсистемы: запросы маны (двухшаговая модель),
заклинания, эфирные срывы, активация големов, повышение уровня эфира.

## Зависимости
- `DualFrontier.Contracts` — `IEvent`, `EntityId`, `[Deferred]`.

## Что внутри
- `ManaIntent.cs` — шаг 1: намерение потратить ману.
- `ManaGranted.cs` — шаг 2: мана списана, можно кастовать.
- `ManaRefused.cs` — шаг 2: маны не хватает, каст отменяется.
- `SpellCastEvent.cs` — заклинание успешно произнесено.
- `EtherSurgeEvent.cs` — «эфирный срыв» при работе с кристаллом / перегрузке (GDD 4.2).
- `GolemActivatedEvent.cs` — маг активировал своего голема.
- `EtherLevelUpEvent.cs` — `[Deferred]`: уровень восприятия эфира повышен.

## Правила
- Стоимость заклинания и содержание голема всегда идут через
  `ManaIntent` — никогда не вычитать ману напрямую из компонента.
- `EtherLevelUpEvent` — `[Deferred]`: повышение уровня влияет на максимум
  маны и другие производные, обработка в следующей фазе избегает гонок.
- `EtherSurgeEvent` может опубликовать StatusApplied (горение, оглушение и т. п.)
  — см. GDD 4.2.

## Примеры использования
```csharp
_bus.Publish(new ManaIntent { /* CasterId = mage, Amount = spell.Cost */ });
// → ManaSystem публикует ManaGranted → SpellCastSystem публикует SpellCastEvent
//   (или ManaRefused — каст отменён, AI выбирает другое действие)
```

## TODO
- [ ] Заполнить поля, когда появятся `MagicSchool`, `SpellId`, `GolemId`.
- [ ] Добавить `SpellInterruptedEvent` — если каст прерван (урон, дизель, и т.п.).

## v02 Addendum additions
Расширение подсистемы магии: непрерывная аренда маны (mana lease) и смена владения големом.

- `LeaseId.cs` — идентификатор аренды маны (`readonly record struct`), фабрика `New()` — TODO Фаза 5.
- `RefusalReason.cs` — причины отказа в открытии аренды (`InsufficientMana`, `LeaseCapExceeded`, `NoActiveBond`, `SchoolMismatch`).
- `CloseReason.cs` — причины закрытия аренды (`Completed`, `SpellInterrupted`, `GolemDeactivated`, `PawnDied`, `ManaExhausted`).
- `ManaLeaseOpenRequest.cs` — `ICommand`: открыть аренду маны на кастера с указанным дренажом и окном длительности.
- `ManaLeaseOpened.cs` — `IEvent`: подтверждение, что аренда открыта и первый тик списан.
- `ManaLeaseRefused.cs` — `IEvent`: в открытии аренды отказано с причиной и доступной маной.
- `ManaLeaseClosed.cs` — `[Deferred]` `IEvent`: терминальное событие жизненного цикла аренды.
- `GolemOwnershipTransferRequest.cs` — `ICommand`: передать/оспорить/покинуть владение големом.
- `GolemOwnershipChanged.cs` — `[Deferred]` `IEvent`: владение големом изменено.

Примечание: `OwnershipMode` живёт в `DualFrontier.Contracts.Enums`, так как используется и компонентами (`GolemBondComponent`), и событиями магии.
