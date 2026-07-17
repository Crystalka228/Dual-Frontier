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

---
register_id: DOC-F-SRC-COMPONENTS-WORLD
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: Components World submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
