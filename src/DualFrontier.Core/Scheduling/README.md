# Scheduling — Parallel scheduler

## Purpose
Builds the READ/WRITE dependency graph between systems based on `[SystemAccess]`
declarations, topologically sorts it into phases, and runs the systems of a
single phase in parallel. `TickScheduler` on top of this manages different tick
frequencies: REALTIME / FAST / NORMAL / SLOW / RARE.

## Dependencies
- `DualFrontier.Contracts` (the `SystemAccessAttribute`, `TickRateAttribute` attributes).
- `DualFrontier.Core.ECS` (`SystemBase`).

## Contents
- `DependencyGraph.cs` — builds the conflict graph from system declarations.
- `ParallelSystemScheduler.cs` — runs phases through `Parallel.ForEach` or a
  task pool.
- `SystemPhase.cs` — immutable list of systems in one phase.
- `TickScheduler.cs` — decides which systems to run on which ticks.
- `TickRates.cs` — frequency constants (duplicates `DualFrontier.Contracts.Attributes.TickRates`).

## Rules
- The graph is built once at startup, after every mod has loaded. Late system
  registration ⇒ graph rebuild (expensive).
- A system cannot live in multiple phases at once.
- Two threads never write to the same `ComponentStore`: this is the graph's invariant.

## Usage examples
```csharp
var graph = new DependencyGraph();
graph.AddSystem(new CombatSystem());
graph.AddSystem(new ManaSystem());
graph.Build();

var ticks = new TickScheduler();
var scheduler = new ParallelSystemScheduler(
    graph.GetPhases(), ticks, world,
    new Dictionary<SystemBase, SystemMetadata>(),
    new NullModFaultSink(),
    services);
scheduler.ExecuteTick(delta: 1f / 30f);
```

## TODO
- [x] Phase 1 — implement topological sort of the graph.
- [x] Phase 1 — implement parallel phase execution.
- [x] Phase 1 — implement `TickScheduler` with multiple frequencies.
- [x] Phase 1 — add cycle detection in the graph with diagnostics.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CORE-SCHEDULING
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CORE-SCHEDULING
---
