# Performance

Performance is an architectural property, not a late-stage optimization. Dual Frontier aims at specific numbers from day one; the dependency graph, domain buses, and invalidating caches are the direct tools for hitting them.

## Target metrics

Comparison with RimWorld shows which operations specifically must be cheaper and how that is achieved.

| Operation                          | RimWorld             | Dual Frontier target  | How it is achieved                 |
|------------------------------------|----------------------|-----------------------|------------------------------------|
| Storage item lookup                | O(n) per tick        | O(1)                  | Invalidating cache                 |
| 100 pawns × ammo request           | 6000 scans/sec       | ≤100 scans/sec        | Intent batching                    |
| Parallel systems                   | 1 thread             | N-2 threads           | Dependency graph                   |
| Mod loading                        | Restart              | Hot reload            | `AssemblyLoadContext`              |
| Isolation violation                | Silent bug           | Crash + diagnostics   | `SystemExecutionContext`           |
| Mood-system tick                   | Every tick           | 1× per second         | `TickScheduler` SLOW               |
| `InventorySystem` cache for 100 storages | —              | ≤1 scan/tick after warm-up | `_freeSlotCache` with a `_cacheDirty` flag |

Targets are formulated per operation. The frame budget is 16.6 ms at 60 FPS on a 30-pawn / 10,000-tile scene; 33 ms at 30 FPS on a 100-pawn scene. These values are pinned in CI through `PerformanceGate` — tests fail when regression exceeds 10%.

## dotTrace and BenchmarkDotNet

### dotTrace

The primary in-game profiling tool. Used in two modes:

- **Sampling** (`--profiling-mode=Sampling`) — low overhead, suitable for a full 60-second run. Result: exactly where CPU is spent.
- **Timeline** — bus events and scheduler phases pinned to a time axis. For Dual Frontier this is the primary mode: it shows which phases finish too late, which bus is holding the thread, and where the `[Deferred]` queue grows.

Every scheduler phase is tagged via `Activity` / `EventSource`; each domain bus emits `BusPublished` markers. dotTrace builds a clear map: "phase 2, 8 ms, 6 systems in parallel, 2 idle".

### BenchmarkDotNet

Microbenchmarks for hot paths: `ComponentStore` (Add/Remove/Query), `DependencyGraph` (BuildGraph), `SpatialGrid` (QueryRadius). Results are stored in `tests/DualFrontier.Core.Benchmarks/Results` and compared across versions.

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class ComponentStoreBenchmark
{
    private ComponentStore<HealthComponent> _store = null!;
    [GlobalSetup] public void Setup() { /* ... */ }

    [Benchmark] public HealthComponent Get10k()
    {
        // typical hot path.
    }
}
```

Benchmarks run in CI once a week, plus ad-hoc locally when the core is edited.

## Hot paths

Workload analysis identifies the following hot paths — in descending order of call frequency.

### ComponentStore.Get / Query

Called thousands of times per phase. Implemented on SparseSet: one array for lookup, one for dense storage. Target: 5 ns per `Get`, 30 ns per `Query<T1, T2>` per-entity.

### DomainEventBus.Publish

Publishing an event walks the subscriber list synchronously. The implementation uses a delegate array (not `List<T>`), allocation-free. Target: 50 ns per publication with 3 subscribers.

### SpatialGrid.QueryRadius

Find entities within radius R from a point. Implementation: a cell grid, each cell holds `List<EntityId>`. Target: a walk over ≤9 cells instead of all 10,000 entities.

### SystemExecutionContext.GetComponent (DEBUG)

In DEBUG the full declaration check adds +1–2 ns per call. In RELEASE the check is removed.

### PathfindingService

A\* through the navigation graph. The most expensive operation (1–5 ms per path).
The current implementation (`AStarPathfinding`) is synchronous A* with a
2000-iteration cap per call; on overflow, `TryFindPath` returns `false` and
the pawn re-requests on the next tick. A path cache between frequently used
pairs of points is not yet implemented (TODO — would give 10×).

Long-term: pathfinding migrates to GPU flow fields under the K9 + G6/G7
roadmap ([GPU_COMPUTE](./GPU_COMPUTE.md) Domain A extension). Per-pawn cost
collapses to one field read + arithmetic; pathfinding cost decouples from
pawn count. A\* preserved as fallback for unique destinations only.

### GPU compute — Domain A (fields) and Domain B (entity-keyed bulk)

GPU compute is now a foundational architectural capability with two workload domains
([GPU_COMPUTE](./GPU_COMPUTE.md) v2.0 LOCKED).

**Domain A — fields** (mana, electricity, water, heat, sound, scent). Dense 2D grids
updated by 4-neighbor stencil compute shaders. No CPU/GPU crossover threshold — field
math is GPU-suitable from the first cell. Per-tick budget for 3 fields × 200×200 ×
5–10 iterations: <1 ms on mid-range GPU vs several ms CPU. Performance benchmarks
land per G-milestone (G1 mana, G2 electricity, G3 storage, G4 multi-field) and pin
CPU-fallback ratios for environments without Vulkan 1.3 compute.

**Domain B — entity-keyed bulk compute** (`ProjectileSystem` and similar). Phase 3
deferral preserved as a special case: still threshold-driven, still benchmarked
against CPU baseline. "Battle of the Gods" stress test (500 mages × spell-spam =
~5,000 simultaneous projectiles, ~50,000 collisions/sec) remains the calibration
scenario, but native kernel + Vulkan rendering layer pivots collapse the dispatch
overhead from 0.5–2 ms (managed) to microseconds (native), so the threshold may
shift downward in practice.

TODO (G5+): BenchmarkDotNet scenario `ProjectileStressBenchmark` with parameter
`[Params(100, 500, 1000, 5000)]` for projectile count. Compare `CpuProjectileCompute`
vs `GpuProjectileCompute` on the post-pivot architecture. Pin the switchover
threshold in [GPU_COMPUTE](./GPU_COMPUTE.md) Domain B timing budget.

## Caches and invalidation

A cache without invalidation is worse than no cache: the game starts to lie. In Dual Frontier every cache explicitly declares its invalidation source.

### `InventorySystem` cache

A `Dictionary<ItemType, List<EntityId>>`. Invalidated by `ItemAddedEvent` and `ItemRemovedEvent`. Correctness check in DEBUG: every 100 ticks a full scan is compared against the cache.

### `PathfindingService` cache

Two-layer: terrain blocks (do NOT change between builds) and the path between anchor points. Invalidated by `BuildingPlacedEvent`, `BuildingDestroyedEvent`, `TileChangedEvent`.

### `SpatialGrid` cache

Updated on every `PositionChangedEvent`. Not invalidated but updated incrementally — the entity is removed from the old cell and added to the new one.

### System graph cache

`DependencyGraph` is built once and does not change. Invalidated only on mod load/unload — at that moment the graph is rebuilt and the scheduler recomputes phases.

Rule: every cache is documented with the table `(key → value, invalidation sources, DEBUG check)`. Without that table the cache review does not pass.

## How to add a benchmark

1. **Define what is measured.** A benchmark without a target is noise. A good formulation: "`ComponentStore.Get` on 10k entities must hold ≤10 ns per call".
2. **Create a class in `tests/DualFrontier.Core.Benchmarks`.**

    ```csharp
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class MyBenchmark
    {
        [Params(1000, 10000, 100000)] public int N;
        // ...

        [GlobalSetup] public void Setup() { /* ... */ }

        [Benchmark] public void HotPath() { /* ... */ }
    }
    ```

3. **Run it locally.** `dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks`.
4. **Pin the baseline.** Results are stored in `Results/MyBenchmark.json`.
5. **Add a CI gate.** In `tests/DualFrontier.Core.Benchmarks/PerformanceGates.cs` add a rule: "method X must be no slower than baseline + 10%". CI fails on regression.
6. **Document.** Add a row to the "Hot paths" table with the expected number.

A benchmark without a pinned baseline does not land in main — otherwise anyone could quietly degrade performance.

## See also

- [THREADING](./THREADING.md)
- [EVENT_BUS](./EVENT_BUS.md)
- [TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md)
