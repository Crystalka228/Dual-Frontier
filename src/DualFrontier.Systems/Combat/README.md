# Combat Systems

## Назначение
Бой: инициация огня, полёт снарядов, расчёт урона, щиты и
статус-эффекты. См. GDD раздел "Бой".

## Зависимости
- `DualFrontier.Contracts` — атрибуты, `ICombatBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Combat` — `WeaponComponent`,
  `ArmorComponent`, `ShieldComponent`, `ProjectileComponent`.
- `DualFrontier.Components.Shared` — `HealthComponent`, `PositionComponent`.
- `DualFrontier.Events.Combat` — `ShootAttemptEvent`, `DamageEvent`,
  `DeathEvent`, `StatusAppliedEvent`.

## Что внутри
- `CombatSystem.cs` — FAST: инициирует атаки, запрашивает патроны (AmmoIntent).
- `ProjectileSystem.cs` — REALTIME: полёт снарядов, столкновения.
- `DamageSystem.cs` — FAST: применение урона с учётом брони и щитов.
- `ShieldSystem.cs` — FAST: регенерация/поглощение щитов.
- `StatusEffectSystem.cs` — FAST: тики статус-эффектов (горение, отравление).

## Правила
- Шина домена — `nameof(IGameServices.Combat)`.
- `CombatSystem` НЕ пишет урон напрямую — публикует
  `DamageEvent`, `DamageSystem` его обрабатывает.
- Патроны — через `AmmoIntent` / `AmmoGranted` / `AmmoRefused` в
  `Inventory` шину, не блокируясь.
- `ProjectileSystem` тикает на REALTIME (каждый кадр), потому
  что от него зависит визуальная корректность.

## Примеры использования
```csharp
// Внутри CombatSystem:
combatBus.Publish(new ShootAttemptEvent(shooterId, targetId));
// Позже — при AmmoGranted — публикуем DamageEvent в DamageSystem.
```

## TODO
- [ ] Реализовать `CombatSystem`: машина состояний выстрела.
- [ ] Реализовать `ProjectileSystem`: движение снарядов по прямой.
- [ ] Реализовать `DamageSystem`: формула урона = atk - armor + crit.
- [ ] Реализовать `ShieldSystem`: магическое поглощение.
- [ ] Реализовать `StatusEffectSystem`: очередь активных эффектов.

## v02 Addendum (TechArch §12.4)

- `CompositeResolutionSystem.cs` — FAST, multi-bus (Combat + Inventory + Magic):
  двухфазный коммит по `TransactionId`. Подписан на `CompoundShotIntent`,
  `AmmoGranted`/`AmmoRefused`, `ManaGranted`/`ManaRefused`. Когда оба
  частичных ответа собраны — публикует итоговый `ShootGranted` либо
  `ShootRefused` в Combat шину.
- `ComboResolutionSystem.cs` — NORMAL, multi-bus (Combat + Magic):
  собирает `DamageIntent` от Physical/Magic/Status-систем и применяет
  их в детерминированном порядке (сортировка по
  `(EntityId, DamageKind ordinal)`), публикуя итоговые `DamageEvent`.
- `CombatSystem` обновлён: теперь объявляет шины `Combat` и `Magic`
  (манна стрелка — часть составного выстрела) и делегирует проверку
  ресурсов через `CompoundShotIntent` вместо прямого `AmmoIntent`.
