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

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-SHARED
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-SHARED
---
