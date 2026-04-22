# Shared

## Назначение
Компоненты, потенциально применимые к любой entity: позиция в мире,
здоровье, принадлежность к фракции, раса. Это фундамент, на который
опираются все остальные домены.

## Зависимости
- `DualFrontier.Contracts` — `IComponent`, `EntityId`, `GridVector`.

## Что внутри
- `PositionComponent.cs` — координаты в тайловой сетке мира.
- `HealthComponent.cs` — текущее/максимальное HP, `IsDead`.
- `FactionComponent.cs` — идентификатор фракции (колония, налётчики и т. д.).
- `RaceComponent.cs` — раса (люди / нежить / синтеты) — см. GDD раздел «Расы».

## Правила
- Почти у каждой боевой entity будет `HealthComponent` и `PositionComponent`.
- `FactionComponent` обязателен для всех участников боя — иначе системы
  не смогут определить «свой/чужой».
- Никаких float-флагов состояния вида `isAlive` — используем `IsDead`
  через `HealthComponent.Current`.

## Примеры использования
```csharp
var pawn = world.CreateEntity();
world.AddComponent(pawn, new PositionComponent { Position = new GridVector(10, 5) });
world.AddComponent(pawn, new HealthComponent { /* Current = 100, Maximum = 100 */ });
world.AddComponent(pawn, new FactionComponent { /* FactionId = "colony" */ });
```

## TODO
- [ ] Определить `RaceKind` enum (Human, Undead, Synthetic …) по GDD.
- [ ] Решить: `FactionId` — `string` или `int` (ID в таблице фракций).
