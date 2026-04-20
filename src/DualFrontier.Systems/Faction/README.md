# Faction Systems

## Назначение
Метаигровые отношения между фракциями: дипломатия, торговля и
рейды. См. GDD раздел "Фракции".

## Зависимости
- `DualFrontier.Contracts` — атрибуты, `IWorldBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Shared` — `FactionComponent`.
- `DualFrontier.Components.Pawn` — `SocialComponent` (для влияния отдельных пешек на репутацию).
- `DualFrontier.Events.World` — `RaidIncomingEvent`, `TradeCaravanEvent`.

## Что внутри
- `RelationSystem.cs` — RARE: эволюция отношений между фракциями.
- `TradeSystem.cs` — RARE: прилёт торговых караванов.
- `RaidSystem.cs` — RARE: спавн рейдов, публикует `RaidIncomingEvent`.

## Правила
- Шина домена — `nameof(IGameServices.World)`.
- Все три системы — RARE, метаигра редкая.
- `RaidSystem` единственная решает, когда идёт налёт; остальной
  мир реагирует на `RaidIncomingEvent`.

## Примеры использования
```csharp
// Внутри RaidSystem:
worldBus.Publish(new RaidIncomingEvent(fromFaction: "raiders", strength: 12));
```

## TODO
- [ ] Реализовать `RelationSystem`: матрица отношений [-100..100].
- [ ] Реализовать `TradeSystem`: таблица товаров по фракциям.
- [ ] Реализовать `RaidSystem`: формула силы рейда от богатства колонии.
