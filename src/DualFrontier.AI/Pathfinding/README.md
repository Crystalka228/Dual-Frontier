# Pathfinding

## Purpose
A* pathfinding over the passability grid. Strictly synchronous: no
`async/await`, otherwise the `ThreadLocal` system isolation guard breaks
(see THREADING).

## Dependencies
- `DualFrontier.Contracts` — `GridVector` (path coordinates), `EntityId`.
- `DualFrontier.Components.World` — `TileComponent` (used by the upper layer
  to build `NavGrid` from the world — not by the AI itself).

There is **NO** dependency on `DualFrontier.Core` — `SpatialGrid` lives there,
but it is infrastructure, not a primitive. `GridVector` is a primitive and
lives in `Contracts.Math`.

## Contents
- `IPathfindingService.cs` — interface with the synchronous `TryFindPath`.
- `AStarPathfinding.cs` — A* implementation.
- `NavGrid.cs` — passability bitmap for queries.

## Rules
- Synchronous API. Long A* is sliced into a per-tick iteration cap and returns
  False on overflow (the pawn retries on the next tick).
- `NavGrid` is immutable for one query; updates from the outside happen in
  batches whenever something is built or destroyed in the world.
- No static singletons — the service is injected.

## Usage examples
```csharp
IPathfindingService pf = /* resolve */;
if (pf.TryFindPath(from, to, out var path)) { /* use it */ }
```

## TODO
- [x] Implement `AStarPathfinding` with an iteration cap (2000 per call,
      `PriorityQueue<GridVector, float>`).
- [x] Implement `NavGrid` as a bitmap (passability + cost map, `SetTile`).
- [ ] Add hierarchical pathfinding for distant goals.
- [ ] Path cache between frequently used pairs of points (invalidation on
      `BuildingPlacedEvent` / `TileChangedEvent`, see PERFORMANCE).
