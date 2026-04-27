# DualFrontier.AI

## Purpose
The behavioral layer for pawns and units: behaviour trees, jobs (tasks the pawn
executes step by step), and A* pathfinding. Systems from `DualFrontier.Systems`
call AI as pure utilities (no world state of their own) — and apply effects
through their own WRITE components.

## Dependencies
- `DualFrontier.Contracts` — `EntityId`, `GridVector`, base types.
- `DualFrontier.Components` — for reading pawn data (skills, position,
  inventory) — through `BTContext` / `IJob`.

**Does NOT depend** on `DualFrontier.Core` (otherwise the graph cycles —
Core contains the scheduler, which calls Systems, which call AI). Every
primitive AI shares with Systems and Components (e.g., `GridVector`)
lives in `Contracts` — the same layer mods depend on.

## Contents
- `BehaviourTree/` — generic BT (Selector / Sequence / Leaf).
- `Jobs/` — `IJob` + concrete jobs (haul, craft, cast, meditate,
  golem-command).
- `Pathfinding/` — `IPathfindingService`, A* implementation, passability grid.

## Rules
- AI has NO access to `World` or to the buses — data is fed to it through
  `BTContext` / `IJob.Tick` arguments.
- AI MUST NOT cache state between system ticks unless the job is marked
  stateful (a stateful job stores local progress in its own fields).
- No `async` / `await` — pathfinding is synchronous (see THREADING).
  Long A* is sliced across frames via `TryFindPath` with an iteration cap.
- BT nodes are pure functions over `BTContext`, no global singletons.

## Usage examples
```csharp
// In Pawn/JobSystem:
var job = new JobHaul(/* args */);
job.Start();
if (job.Tick(delta) == JobStatus.Done) { /* ... */ }
```

## TODO
- [x] Implement `AStarPathfinding` with a per-tick iteration cap
      (2000 iterations per call, no path cache — see
      `Pathfinding/README.md`).
- [ ] Write a BT JSON parser for mods.
- [ ] Cover `Selector` / `Sequence` / `Leaf` with unit tests.
- [ ] Implement `JobCast` (integration with `SpellSystem` through the bus).
