# Combat Events

## Назначение
События боевой подсистемы: попытки выстрела, запросы патронов по
двухшаговой модели, события урона, смерти и наложения статусов.

## Зависимости
- `DualFrontier.Contracts` — `IEvent`, `EntityId`, `[Deferred]`.

## Что внутри
- `ShootAttemptEvent.cs` — пешка пытается произвести выстрел.
- `AmmoIntent.cs` — **шаг 1** двухшаговой модели: намерение получить патрон (TechArch 11.5).
- `AmmoGranted.cs` — **шаг 2**: патрон выдан, можно стрелять.
- `AmmoRefused.cs` — **шаг 2**: патрона нет, выстрел отменяется.
- `DamageEvent.cs` — нанесение урона рассчитано DamageSystem.
- `DeathEvent.cs` — `[Deferred]`: сущность умерла, обработка в следующей фазе.
- `StatusAppliedEvent.cs` — на сущность наложен статус-эффект.

## Правила
- `AmmoIntent` НЕ блокирует — ответ приходит отдельным событием.
- `DeathEvent` помечен `[Deferred]`: удаление entity не должно прерывать
  текущую фазу (ссылки на entity ещё используются другими системами).
- `ShootAttemptEvent` публикуется AI/игроком; CombatSystem решает,
  возможен ли выстрел (достаёт `WeaponComponent`/`AmmoComponent`).

## Примеры использования
```csharp
_bus.Publish(new ShootAttemptEvent { /* AttackerId = pawn, TargetId = enemy */ });
// → CombatSystem публикует AmmoIntent
// → InventorySystem отвечает AmmoGranted или AmmoRefused
// → CombatSystem, получив AmmoGranted, публикует DamageEvent
// → DamageSystem при HP ≤ 0 публикует DeathEvent ([Deferred])
```

## TODO
- [ ] Заполнить поля, когда появятся `AmmoType`, `DamageType`, `GridVector`, `StatusKind`.
- [ ] Добавить `MissEvent` / `CritEvent` — опционально, Фаза 6.

## v02 Addendum additions
Расширение боевой подсистемы: двухфазный коммит «составного выстрела» (патрон + мана) и явная команда на урон.

- `TransactionId.cs` — идентификатор составной транзакции выстрела (`readonly record struct`), фабрика `New()` — TODO Фаза 4.
- `ShotRefusalReason.cs` — причины отказа (`NoAmmo`, `NoMana`, `WeaponOnCooldown`, `OutOfRange`, `TargetInvalid`).
- `CompoundShotIntent.cs` — `IQuery`: намерение произвести составной выстрел (опрос Inventory + Magic).
- `ShootGranted.cs` — `IEvent`: обе шины подтвердили, выстрел разрешён.
- `ShootRefused.cs` — `IEvent`: хотя бы одна шина отказала, выстрел отменён.
- `DamageIntent.cs` — `ICommand`: запрос на нанесение урона для `DamageSystem` (перед публикацией `DamageEvent`).
