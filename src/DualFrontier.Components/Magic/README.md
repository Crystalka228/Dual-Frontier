# Magic

## Назначение
Компоненты магической подсистемы: мана, уровни школ магии, восприятие эфира
и связь голем-маг. Реализует ключевые механики GDD 4 («Магическая колония»)
и GDD 5 («Система Големов»).

## Зависимости
- `DualFrontier.Contracts` — `IComponent`, `EntityId`.

## Что внутри
- `ManaComponent.cs` — текущая/максимальная мана + регенерация.
- `SchoolComponent.cs` — уровни магии по школам (`Dictionary<MagicSchool,int>`).
- `EtherComponent.cs` — уровень восприятия эфира (1–5, GDD 4.1).
- `GolemBondComponent.cs` — связь голема с магом-хозяином, уровень голема (1–5).

## Правила
- Маг без `ManaComponent` заклинаний не кастует — проверка в SpellCastSystem.
- `EtherComponent.Level` жёстко ограничен 1..5. Повышение — через
  `EtherLevelUpEvent` (deferred).
- Голем ОБЯЗАН иметь `GolemBondComponent.OwnerId`. Смерть/истощение хозяина
  → голем останавливается (GDD 5.2).

## Примеры использования
```csharp
[SystemAccess(reads: new[] { typeof(ManaComponent), typeof(SchoolComponent) })]
public class SpellCastSystem : SystemBase { /* ... */ }
```

## TODO
- [ ] Определить `MagicSchool` enum (Fire, Ice, Storm, Earth, Wind, Water,
      Dark, Light, Mind, Void) — GDD 4.3.
- [ ] Добавить `ManaRegenModifierComponent` для эффектов окружения (узлы,
      зелья, ритуалы) — Фаза 6.
- [ ] `GolemTier` 1..5 — валидатор в Фазе 5.

## v02 Addendum additions
Расширение `GolemBondComponent` (v02 §12.5): поля режима владения и механики оспаривания/перехвата.

- `GolemBondComponent.BondedMage` — `EntityId?`, ссылка на текущего мага-хозяина (`null` при `Abandoned`).
- `GolemBondComponent.Mode` — `OwnershipMode`, режим владения, по умолчанию `Bonded`.
- `GolemBondComponent.TicksSinceContested` — `int`, счётчик тиков в состоянии `Contested` для таймаута смены хозяина.
- `GolemBondComponent.BondStrength` — `int`, прочность связи, участвует в разрешении споров.

Примечание: enum `OwnershipMode` определён в `DualFrontier.Contracts.Enums` — Components не зависят от Events, поэтому общий тип поднят в Contracts.
