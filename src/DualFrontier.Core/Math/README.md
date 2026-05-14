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
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CORE-MATH
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CORE-MATH
---
