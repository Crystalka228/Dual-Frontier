---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CORE
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CORE
---
# DualFrontier.Core

## Purpose
The project's core: ECS (World + ComponentStore + SystemBase + isolation guard),
the parallel scheduler (READ/WRITE dependency graph, phases, tick rates),
domain-event-bus implementations, grid math, and component/system registries.
The whole assembly is `internal`-first: only types explicitly marked `public` are
visible from outside, and even those reach `Systems`, `Application`, and
`Core.Tests` only through `InternalsVisibleTo`.

## Dependencies
- `DualFrontier.Contracts` — ECS contracts, bus contracts, attributes.

## Contents
- `ECS/` — `World`, `ComponentStore`, `SystemBase`, `SystemExecutionContext`
  (the isolation guard), `IsolationViolationException`.
- `Scheduling/` — `DependencyGraph`, `ParallelSystemScheduler`, `SystemPhase`,
  `TickScheduler`, `TickRates`.
- `Bus/` — `DomainEventBus`, `GameServices`, `IntentBatcher`.
- `Math/` — `SpatialGrid` (infrastructure-level world partitioning; the
  `GridVector` primitive lives in `DualFrontier.Contracts.Math`).
- `Registry/` — `ComponentRegistry`, `SystemRegistry`.

## Rules
- No references to Godot.
- No game logic — that lives in `DualFrontier.Systems`.
- Every type is `internal` except `SystemBase` and the public contracts (which
  are actually defined in `Contracts`); Core systems get access through
  `InternalsVisibleTo`.
- Parallelism is provided by the scheduler, not by the data structures themselves.
  If a type is intended to be used directly from many threads, it MUST be
  thread-safe and document that.

## Usage examples
```csharp
// From DualFrontier.Application (via InternalsVisibleTo)
var world    = new World();
var services = new GameServices();
var ticks    = new TickScheduler();

var graph = new DependencyGraph();
graph.AddSystem(new NeedsSystem());
graph.AddSystem(new JobSystem());
graph.Build();

var scheduler = new ParallelSystemScheduler(
    graph.GetPhases(), ticks, world,
    new Dictionary<SystemBase, SystemMetadata>(),
    new NullModFaultSink(),
    services);
scheduler.ExecuteTick(delta: 1f / 30f);
```

## TODO
- [x] Phase 1 — implement `World` / `ComponentStore` on top of SparseSet.
- [x] Phase 1 — implement `SystemExecutionContext` (`ThreadLocal` guard).
- [x] Phase 1 — implement `DependencyGraph` and `ParallelSystemScheduler`.
- [x] Phase 1 — implement `DomainEventBus` and `GameServices`.
- [x] Phase 2 — write isolation tests confirming the guard crashes.
