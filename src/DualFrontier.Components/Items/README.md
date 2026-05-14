# Items

## Purpose
Components for world-placed item entities — consumables (food, drink),
beds, decorations, plus the generic reservation primitive used to
coordinate pawn approaches к items. M8.3 establishes the vocabulary;
ItemFactory (M8.4 Vanilla.World) creates entities, consume / sleep /
comfort systems (M8.5–M8.7 Vanilla.Pawn) consume them.

## Dependencies
- `DualFrontier.Contracts` — `IComponent`, `EntityId`, `[ModAccessible]`.
- `DualFrontier.Components.Pawn` — `NeedKind` (referenced by ConsumableComponent.RestoresKind).

## Contents
- `ConsumableComponent.cs` — single-tick consume (food, packaged drinks).
  Closes `Satiety` or `Hydration` per `RestoresKind`. Charges decrement
  on consume; entity destroyed when 0.
- `WaterSourceComponent.cs` — persistent infinite hydration source. No
  charges; sources never deplete.
- `BedComponent.cs` — multi-tick sleep restoration. Tracks Occupant +
  per-tick rate. Single-occupant for now.
- `DecorativeAuraComponent.cs` — passive ambient Comfort. Radius +
  per-tick rate; M8.7 ComfortAuraSystem queries SpatialGrid.GetInRadius.
- `ReservationComponent.cs` — generic reservation primitive. Component
  existence = reservation active. Multiple systems use it — pawn
  approaches food, bed, work site.

## Rules
- All components annotated `[ModAccessible(...)]` per architecture §3 D-1
  LOCKED. Vanilla mods declare `kernel.read:{FQN}` / `kernel.write:{FQN}`
  in `mod.manifest.json` capabilities.required when content lands.
- ConsumableComponent.Charges decrement is the ConsumeSystem's
  responsibility; component is pure data. When Charges reaches 0,
  ConsumeSystem destroys the entity.
- BedComponent.Occupant is set by SleepSystem on claim, cleared on wake
  or interrupt. No two pawns share a bed in M8 (single-occupant only).
- DecorativeAuraComponent emissions are static — auras don't change
  state at runtime в M8. Future fluctuation (e.g. broken decoration)
  via separate component or Charges-style field.
- ReservationComponent existence = reservation active. Removing the
  component releases the reservation. Multiple systems may reserve/
  release; no central manager. Timeout cleanup via ReservedAtTick.

## Usage examples
```csharp
// M8.4 ItemFactory creating a piece of bread:
var bread = world.CreateEntity();
world.AddComponent(bread, new PositionComponent { X = 50, Y = 50 });
world.AddComponent(bread, new ConsumableComponent {
    RestoresKind      = NeedKind.Satiety,
    RestorationAmount = 0.4f,
    Charges           = 4,                  // 4 bites before destroyed
});

// M8.5 ConsumeSystem applying a consume:
var consumable = world.GetComponent<ConsumableComponent>(target);
needs.Satiety = Math.Min(1f, needs.Satiety + consumable.RestorationAmount);
consumable.Charges--;
if (consumable.Charges <= 0)
    world.DestroyEntity(target);

// M8.7 ComfortAuraSystem applying passive ambient:
foreach (var decoration in Query<DecorativeAuraComponent>())
{
    var pos = world.GetComponent<PositionComponent>(decoration);
    var aura = world.GetComponent<DecorativeAuraComponent>(decoration);
    var nearbyPawns = spatialGrid.GetInRadius(new GridVector(pos.X, pos.Y), aura.Radius)
                                 .Where(e => world.HasComponent<NeedsComponent>(e));
    foreach (var p in nearbyPawns)
    {
        var n = world.GetComponent<NeedsComponent>(p);
        n.Comfort = Math.Min(1f, n.Comfort + aura.ComfortPerTick);
    }
}
```

## TODO
- [ ] Quality tier modifiers — luxury beds restore Sleep faster, masterwork decorations larger Radius. Currently per-instance в ItemFactory.
- [ ] Multi-occupant beds (bunk beds) — defer; single-occupant в M8.
- [ ] Aura decay over time (e.g. flowers wilt) — defer to spoilage system.
- [ ] Item carrying / inventory — separate component family когда `Inventory` mod has content (M10).

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-ITEMS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-ITEMS
---
