# DualFrontier.Events

## Назначение
Сборка всех доменных событий, интентов и запросов. Каждое событие —
неизменяемый `record`, реализующий `IEvent` из `DualFrontier.Contracts.Core`.
Разбиты по доменам (Combat / Magic / Inventory / Power / Pawn / World),
по одной папке на домен — под каждую доменную `IEventBus`.

Сборка реализует два ключевых паттерна архитектуры (TechArch 11):
- **Intent vs Event**: двухшаговые механики (AmmoIntent → AmmoGranted/Refused,
  ManaIntent → ManaGranted/Refused) вместо блокирующего request/response.
- **Deferred**: события, отложенные до следующей фазы планировщика
  (`DeathEvent`, `EtherLevelUpEvent`) — помечены атрибутом `[Deferred]`.

## Зависимости
- `DualFrontier.Contracts` — `IEvent`, `EntityId`, `[Deferred]`, `[Immediate]`.

События НЕ зависят от `Components` (общие типы — `EntityId`, перечисления —
должны жить либо в `Contracts`, либо в `Components`; в этом scaffold
типы полей оставлены в TODO).

## Что внутри
- `Combat/` — ShootAttempt, Ammo Intent/Granted/Refused, Damage, Death, StatusApplied.
- `Magic/` — Mana Intent/Granted/Refused, SpellCast, EtherSurge, GolemActivated, EtherLevelUp.
- `Inventory/` — ItemAdded, ItemRemoved, ItemReserved, CraftRequest.
- `Power/` — PowerRequest, PowerGranted, GridOverload.
- `Pawn/` — MoodBreak, DeathReaction, SkillGain.
- `World/` — EtherNodeChanged, WeatherChanged, RaidIncoming.

## Правила
- Только `public sealed record XxxEvent : IEvent` — никаких классов.
- Все поля через `init` или `required init` — события неизменяемы после создания.
- `[Deferred]` для событий, на которые нельзя реагировать мгновенно
  (напр. удаление сущности — `DeathEvent`).
- `[Immediate]` — только для критичных прерываний фазы (крайне редко).

## Примеры использования
```csharp
// Шаг 1 двухшаговой модели — CombatSystem публикует намерение.
_bus.Publish(new AmmoIntent { /* RequesterId = pawn, AmmoType = ..., Position = ... */ });

// InventorySystem собирает пачку AmmoIntent, в следующей фазе публикует
// AmmoGranted / AmmoRefused по каждому запросу.
```

## TODO
- [ ] Заполнить поля событий, когда появятся базовые типы
      (`GridVector`, `AmmoType`, `DamageType`, `MagicSchool`, `PowerType`).
- [ ] Проверить корректность `[Deferred]` разметки после того, как
      EventBus реализует обработку отложенной доставки (Фаза 1).
- [ ] Написать генератор диаграмм "кто publish / кто subscribe" по
      атрибутам и именам событий (Фаза 3, инструментарий).
