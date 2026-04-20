# Композитные запросы

Одношаговый Intent решает задачу, когда требуется ровно один ресурс с одной шины. Но уже выстрел манной стрелкой требует двух подтверждений сразу: патрон на `IInventoryBus` и мана на `IMagicBus`. Прямой ask «получи оба ответа и действуй» порождает race conditions в многопоточной фазе. v0.2 вводит двухфазный commit через одного посредника.

## Проблема

Инициатор (например, `CombatSystem`) публикует два независимых намерения: `AmmoIntent` и `ManaIntent`. Оба обрабатываются батч-системами в одной фазе параллельно. Возможны четыре исхода: (granted, granted), (granted, refused), (refused, granted), (refused, refused). В трёх из четырёх случаев действие невозможно, но частично потраченные ресурсы уже списаны — патрон зарезервирован, а маны нет, и откатывать нужно руками на стороне каждой шины. Любая ошибка — постоянная утечка.

Плюс сторона: инициатор должен сам собрать два ответа и понять, что они про одно действие. В батчах с сотнями намерений корреляция по `RequesterId` ломается — один пешке за тик приходит и ammo, и mana от совсем разных событий.

## Решение

Запрос, задевающий N шин, публикуется **один раз** как `CompoundShotIntent` (или другой композитный тип) с общим `TransactionId`. Специальная система-посредник `CompositeResolutionSystem` подписана на все задействованные шины, собирает по `TransactionId` частичные ответы и публикует итоговый `ShootGranted` либо `ShootRefused` (с `ShotRefusalReason`).

### Последовательность

```
 Shooter                 Combat              Inventory           Magic           CompositeResolution
    │                       │                    │                 │                      │
    │── CompoundShotIntent ─┤                    │                 │                      │
    │    (TransactionId)    │                    │                 │                      │
    │                       ├───── AmmoIntent ──►│                 │                      │
    │                       ├────────────── ManaIntent ───────────►│                      │
    │                       │                    │                 │                      │
    │                       │                 (batch phase)                                │
    │                       │                    │                 │                      │
    │                       │◄── AmmoGranted ────┤                 │                      │
    │                       │                    │                 │                      │
    │                       │◄──────────────── ManaGranted ────────┤                      │
    │                       │                    │                 │                      │
    │                       │                                                              │
    │                       ├── forwards partials by TransactionId ────────────────────►  │
    │                       │                                                              │
    │                       │                                   (collects pair)            │
    │                       │                                                              │
    │                       │◄──────────── ShootGranted ──────────────────────────────────┤
    │◄── ShootGranted ──────┤                                                              │
    │                       │                                                              │
```

`CompositeResolutionSystem` живёт в отдельной фазе между сбором намерений и применением — см. [THREADING](./THREADING.md), Phase 2 (Intent Collection).

### Правила определённости

- `TransactionId` монотонен в рамках одного тика: генератор — атомарный счётчик, хранящийся в `CompositeResolutionSystem`.
- Все частичные ответы помечаются своим `TransactionId`. Без него ответ считается частью обычного Intent/Granted и не участвует в composite-резолюции.
- `CompositeResolutionSystem` хранит словарь `TransactionId → PartialState` с числом ожидаемых ответов. Как только все ответы собраны, публикуется итог и запись удаляется.
- Если ответов меньше ожидаемого к концу фазы — транзакция помечается как провисшая, публикуется `ShootRefused` с причиной `PartialTimeout`.

### Откат при частичном отказе

Если одна шина ответила Granted, а вторая — Refused, уже списанный ресурс возвращается Refund-событием на соответствующей шине. Например, `AmmoGranted` означает, что патрон уже зарезервирован в `InventorySystem`. При итоговом `ShootRefused` `CompositeResolutionSystem` публикует `AmmoRefunded` (TODO Фаза 4), и `InventorySystem` отдаёт патрон обратно в склад.

Refund-события помечаются `[Deferred]` — их доставка не должна конкурировать с новыми намерениями в той же фазе. Шина, получающая Refund, обрабатывает его в следующей фазе вместе с новыми Intent'ами.

## Обобщение

Любой запрос, затрагивающий более одной шины, идёт через `CompositeResolutionSystem`. Список поддерживаемых композитных намерений:

- `CompoundShotIntent` — патрон + мана (стрелка с чарами).
- `CompoundCraftIntent` — материалы + энергия + рабочее место (TODO Фаза 4).
- `CompoundRitualIntent` — эфир + мана + предмет-якорь (TODO Фаза 6).

Каждое из них описывается своей парой `...Granted`/`...Refused` с соответствующим Refusal-enum'ом.

## Родственная задача: объединение урона

Тот же паттерн «собрать множество намерений и выдать один детерминированный результат» используется для урона — но с другим посредником: `ComboResolutionSystem`. См. [COMBO_RESOLUTION](./COMBO_RESOLUTION.md). Разница: composite ждёт ответы с разных шин по одной транзакции, combo собирает однородные `DamageIntent` на одну цель и упорядочивает их.

## См. также

- [EVENT_BUS](./EVENT_BUS.md) — двухшаговая модель Intent → Granted.
- [RESOURCE_MODELS](./RESOURCE_MODELS.md) — выбор между Intent и Lease.
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md) — `ComboResolutionSystem` для урона.
- [THREADING](./THREADING.md) — фаза Intent Collection.
