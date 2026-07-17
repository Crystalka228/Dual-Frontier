# Math — Grid math

## Purpose
Math primitives for tile-grid work and spatial indexing. Used by almost every
system: pathfinding, neighbor lookup, projectile spawning, building placement.

## Dependencies
- `System` (BCL)

## Contents
- `SpatialGrid.cs` — partitioning the world into cells for O(1) "everything in
  radius R" queries. Used by CombatSystem (target search), RaidSystem.

> `GridVector` lives in `DualFrontier.Contracts.Math` — a shared primitive
> available to every layer including `AI` and mods that do not depend on `Core`.

## Rules
- `SpatialGrid` is NOT thread-safe — protection comes through the scheduler graph.
- Godot's `Vector2I` is forbidden — that is a Godot namespace. Conversion
  happens only in Presentation.

## Usage examples
```csharp
var a = new GridVector(3, 4);
var b = new GridVector(0, 0);
int dist = a.Manhattan(b); // 7

var grid = new SpatialGrid<EntityId>(cellSize: 16);
grid.Insert(id, position);
foreach (var near in grid.Query(position, radius: 5)) { /* ... */ }
```

## TODO
- [ ] Phase 3 — implement `SpatialGrid.Insert/Remove/Update/Query`.
- [ ] Phase 3 — cover `SpatialGrid` with a benchmark (BenchmarkDotNet).

---
register_id: DOC-F-SRC-CORE-MATH
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
title: Core Math submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
