# Magic Systems

## Назначение
Магическая подсистема Dual Frontier: регенерация маны, каст
заклинаний, управление големами, рост эфирных узлов и
ритуалы. См. GDD разделы "Магия", "Школы магии", "Големы".

## Зависимости
- `DualFrontier.Contracts` — атрибуты, `IMagicBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Magic` — `ManaComponent`, `SchoolComponent`,
  `GolemBondComponent`, `EtherComponent`.
- `DualFrontier.Events.Magic` — `ManaRequest`, `SpellCastEvent`,
  `EtherSurgeEvent`, `GolemActivatedEvent`.

## Что внутри
- `ManaSystem.cs` — NORMAL: регенерация маны и её расход.
- `SpellSystem.cs` — FAST: исполнение заклинаний (публикует результаты).
- `GolemSystem.cs` — NORMAL: поддержание связи голема с хозяином.
- `EtherGrowthSystem.cs` — SLOW: рост эфирных полей от школы.
- `RitualSystem.cs` — RARE: долгие коллективные ритуалы.

## Правила
- Шина домена — `nameof(IGameServices.Magic)`.
- `SpellSystem` ничего не пишет в компоненты — только публикует
  события; урон накладывает `DamageSystem` из `Combat/`.
- `ManaSystem` — единственная система, которая пишет
  `ManaComponent` (кроме ритуалов).
- Эфир и мана — разные пулы: эфир в мире (узлы), мана у пешки.

## Примеры использования
```csharp
// Внутри SpellSystem:
magicBus.Publish(new SpellCastEvent(casterId, spellId, target));
```

## TODO
- [ ] Реализовать `ManaSystem`: формула регена от школы и стат.
- [ ] Реализовать `SpellSystem`: пул активных кастов.
- [ ] Реализовать `GolemSystem`: распад связи при нехватке маны.
- [ ] Реализовать `EtherGrowthSystem`: диффузия эфира по сетке.
- [ ] Реализовать `RitualSystem`: многопешечные ритуалы.

## v02 Addendum (TechArch §12.2, §12.3, §12.5)

### ManaSystem — mana-lease

`ManaSystem` теперь обрабатывает непрерывные аренды маны (lease):
- `OnManaLeaseOpenRequest(ManaLeaseOpenRequest)` — проверяет `Mana.Current[N-1]`,
  открывает аренду через `Internal/ManaLeaseRegistry` и публикует
  `ManaLeaseOpened` либо `ManaLeaseRefused` с соответствующим `RefusalReason`.
- `DrainActiveLeases()` — тиковое списание маны со всех активных аренд,
  публикует `ManaLeaseClosed` по истекшим.
- `OnManaLeaseCloseRequest(LeaseId, CloseReason)` — явное закрытие аренды
  (например, каст прерван извне).

### GolemSystem — смена владения

`GolemSystem` подписывается на `GolemOwnershipTransferRequest` и публикует
отложенное (`[Deferred]`) событие `GolemOwnershipChanged`. Для
предотвращения feedback loop (§12.3) чтение состояния манны хозяина идёт
через `ReadPreviousTickManaState()` — `Mana[N-1]` snapshot.

### Internal/ подкаталог

`Magic/Internal/` — приватные типы подсистемы магии, не пересекающие
границу сборки:
- `ManaLease.cs` — запись об одной активной аренде.
- `ManaLeaseRegistry.cs` — коллекция активных аренд, выдача `LeaseId`,
  тиковое списание.
- `README.md` — правила пакета.
