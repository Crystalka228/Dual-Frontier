---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-WORLD
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-WORLD
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-WORLD
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-WORLD
---
# World

## Purpose
Components for entities that represent the world: tiles, ether nodes, biomes.
The tile grid is the basis of the map; ether nodes are sources of accelerated
mana regeneration (GDD 4).

## Dependencies
- `DualFrontier.Contracts` — `IComponent`.

## Contents
- `TileComponent.cs` — terrain kind + passability.
- `EtherNodeComponent.cs` — node tier + influence radius.

## Rules
- Only one entity with `TileComponent` lives at any single grid position.
- `EtherNodeComponent.Radius` is in tiles. Overlapping radii are summed by
  EtherFieldSystem.

## Usage examples
```csharp
var node = world.CreateEntity();
world.AddComponent(node, new EtherNodeComponent { /* Tier = 2, Radius = 5 */ });
world.AddComponent(node, new PositionComponent { Position = new GridVector(42, 17) });
```

## TODO
- [x] Define the `TerrainKind` enum (Grass, Rock, Sand, Water, Ice, Swamp,
      Arcane, Unknown).
- [ ] Plan layers (floor / wall / decorative) — possibly through separate
      components rather than through `TileComponent`.
