# Shared

## Purpose
Components potentially applicable to any entity: world position, health, faction
membership, species. The foundation that every other domain builds on.

## Dependencies
- `DualFrontier.Contracts` — `IComponent`, `EntityId`, `GridVector`.

## Contents
- `PositionComponent.cs` — coordinates on the world tile grid.
- `HealthComponent.cs` — current/maximum HP, `IsDead`.
- `FactionComponent.cs` — faction identifier (colony, raiders, etc.).
- `RaceComponent.cs` — species (humans / undead / synthetics) — see the GDD "Species" section.

## Rules
- Almost every combat entity has both `HealthComponent` and `PositionComponent`.
- `FactionComponent` is mandatory for every combat participant — otherwise
  systems cannot tell friend from foe.
- No float state flags like `isAlive` — use `IsDead` through
  `HealthComponent.Current`.

## Usage examples
```csharp
var pawn = world.CreateEntity();
world.AddComponent(pawn, new PositionComponent { Position = new GridVector(10, 5) });
world.AddComponent(pawn, new HealthComponent { /* Current = 100, Maximum = 100 */ });
world.AddComponent(pawn, new FactionComponent { /* FactionId = "colony" */ });
```

## TODO
- [ ] Define the `RaceKind` enum (Human, Undead, Synthetic …) per the GDD.
- [ ] Decide: `FactionId` — `string` or `int` (ID in the faction table).
