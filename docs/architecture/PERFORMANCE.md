---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-PERFORMANCE_V2
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: "0.1.0"
next_review_due: post-ratification closure
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-PERFORMANCE_V2
---
# Performance

Where Dual Frontier's frame and tick budgets attach on the current native substrate — and an honest accounting of what enforces them today: nothing does, yet.

> **Document class: authored-rework (current-truth candidate).** Successor of `docs/architecture/historical/PERFORMANCE.md` (DOC-A-PERFORMANCE, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. Becomes the LOCKED authority upon Crystalka ratification per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7; until then the predecessor remains the last-ratified reference and prevails on conflict.
> **Ratification checklist:** [ ] content spot-audit at ratification HEAD · [ ] lifecycle AUTHORED → LOCKED, version → 1.0.0 · [ ] `next_review_due` set · [ ] predecessor register rationale updated.

## Status block

| Field | Value |
|---|---|
| Role | **normative-current-candidate.** Budget numbers below are the intended law; §4 states plainly that no enforcement exists yet — the numbers bind engineering judgment, not CI. |
| Successor of | `docs/architecture/historical/PERFORMANCE.md` (DOC-A-PERFORMANCE) |
| Scope | Frame/tick budgets on the native substrate; cache-invalidation discipline; the real benchmark inventory. |
| Non-goals | Scheduling law, bus-tier semantics (KERNEL_ARCHITECTURE), the GPU substrate itself (VULKAN_SUBSTRATE) — only the budgets attaching to them. |
| Authority domains | Performance budget numbers; cache-invalidation discipline; benchmark-authoring practice. |
| Defers to | KERNEL_ARCHITECTURE.md — К-L15 bus-tier law. VULKAN_SUBSTRATE.md — GPU substrate, own timing budgets. ECS.md / THREADING.md — phase-boundary write-visibility. |

## 1. Philosophy

Performance is architectural, not a late-stage pass: К-L14 names it directly — "Performance derives from cleanness" (KERNEL_ARCHITECTURE.md Part 0, abbreviated row). The dependency graph, native storage, and invalidating caches are the tools; a budget without a documented invalidation source or an "unenforced" label is not a budget, it's a wish.

## 2. Frame budget

`GameLoop` ticks the simulation at a fixed `TargetTps = 30f` (`GameLoop.cs:29`), independent of render FPS — a 33 ms/tick budget. Display-side latency invariants (intent-overlay ≤16 ms, combat-feedback event-to-visible ≈≤17 ms) are К-L15/К-L16/К-L17 law living in KERNEL_ARCHITECTURE.md/VULKAN_SUBSTRATE.md; this doc only inherits the 33 ms simulation figure that every row in §3 divides.

## 3. Budgets on the current substrate

> **FENCED (target / planned — not current truth):** the timing numbers in this table are design targets — none is pinned to a measured baseline or gated by CI (§4). Treat each as engineering intent, not a passing test. The mechanisms in the third column, and the A* iteration cap (a literal code constant), are current truth.

| Layer | Budget | Mechanism (current, verified) |
|---|---|---|
| Native storage (`RawComponentStore`) | O(1) add/remove/get | Sparse-set, swap-remove on delete (`native/DualFrontier.Core.Native/include/component_store.h:39-116`); no per-op ns target is pinned anywhere in the corpus — none is claimed here either. |
| Span/batch protocol | Zero-copy reads; atomic batched writes | `SpanLease<T>` hands out a read-only span, rejects mutation while any lease is open (`SpanLease.cs:16-21`); `WriteBatch<T>` stages writes, flushes atomically (`WriteBatch.cs:9-14`). Recording is currently **one P/Invoke per command** (`Update`/`Add`/`Remove`); bulk transmission is explicitly deferred pending measurement (`WriteBatch.cs:31-32`). Lease pooling is likewise deferred (`SpanLease.cs:13-14`). |
| Phase commit | Writes visible at next phase boundary | `WriteBatch` flush and entity destruction both defer to the next scheduler phase boundary (ECS.md); native `BarrierType` (Full/Partial/None, `phase_barrier.h:12-22`) governs how strictly one phase waits on the last. No numeric commit-latency budget exists anywhere in the corpus — an honest gap. |
| Bus — fast tier | Subscriber response ≤1 ms | К-L15 law (KERNEL_ARCHITECTURE.md Part 0: "К-L15 fast tier latency invariant (subscriber response ≤1ms)"), reused by VULKAN_SUBSTRATE.md's CombatFeedbackLayer budget. Bus semantics are KERNEL_ARCHITECTURE.md's domain; this row only inherits the number. |
| GPU dispatch — V1 diffusion | <1 ms for 3 fields × 200×200 × 5–10 iterations, mid-range GPU | Shipped: `V1DiffusionPipeline.ExecuteIteration` → native `df_world_field_dispatch_compute` (`src/DualFrontier.Runtime/Compute/V1DiffusionPipeline.cs`), synchronous К-L7 dispatch, fence-gated. No benchmark project measures this number; carried forward as a target, not a result. |
| A* pathfinding | Cap 2000 iterations/call | `AStarPathfinding.cs:14`: `private const int MaxIterations = 2000;` — verified exact. Overflow returns `false` and the caller retries next tick. No path cache exists (§6). |

Dropped without replacement, mechanisms gone: `ComponentStore<T>`/SparseSet managed-era targets (5 ns `Get`, 30 ns `Query<T1,T2>`) — the managed store and its benchmark no longer exist; `DomainEventBus.Publish` 50 ns — managed-era, never re-baselined against the native bus; `SpatialGrid.QueryRadius` — renamed `GetInRadius` (`SpatialGrid.cs:127`) and, verified this pass, **not instantiated anywhere in production** (`grep "new SpatialGrid"` matches only the constructor) — `ComfortAuraSystem` runs an explicit O(N×M) brute-force scan instead, comment and all: "SpatialGrid integration deferred to backlog" (`ComfortAuraSystem.cs:26-29`); the "Isolation violation → crash via `SystemExecutionContext`" row — that runtime guard was deleted at the K8.3+K8.4 cutover, so the row asserted a mechanism that no longer exists.

Also worth naming: the managed `DependencyGraph` facade is the only scheduling structure with any budget-shaped treatment in this corpus; the authoritative native `system_graph.cpp` graph has none — a documentation gap, not a claim resolved here.

## 4. Enforcement honesty

> **FENCED (target / planned — not current truth):** a CI-backed enforcement layer for the budgets in §3 is an **open obligation** (session report AD-5), not a shipped mechanism. Nothing below the line is aspirational; it is what exists today.

The predecessor doc stated: "these values are pinned in CI through `PerformanceGate` — tests fail when regression exceeds 10%," and told authors to add a rule "in `tests/DualFrontier.Core.Benchmarks/PerformanceGates.cs`." Neither symbol exists anywhere in the repository — `grep -r PerformanceGate` outside documentation returns zero code matches. There is no `.github/` directory and no CI configuration of any kind, anywhere in the tree. The benchmark suite says so about itself, `SchedulerStressBenchmarks.cs:15-16`: "xUnit tests give pass/fail invariants; these benchmarks give numbers — per-op latency, Gen0/1/2 collections, lock contentions, completed work items." Every budget in §3 is, today, an unenforced engineering obligation, not a gate anything currently fails.

## 5. Benchmark reality census

What exists, verified against `tests/DualFrontier.Core.Benchmarks/` this pass (`DualFrontier.Core.Benchmarks.csproj`, BenchmarkDotNet-based, `dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks`):

| Class | File | Measures |
|---|---|---|
| `BenchHealthComponent` | `BenchHealthComponent.cs` | Benchmark-only blittable struct — like-for-like stand-in for reference-type production `HealthComponent`, which cannot cross P/Invoke without GCHandle pinning. |
| `BootstrapTimeBenchmark` | `BootstrapTimeBenchmark.cs` | `Bootstrap.Run(useRegistry: false)` end-to-end; target 5–15 ms per KERNEL_ARCHITECTURE §K3 (comment-documented, not gated). |
| `NativeBulkAddBenchmark` | `NativeBulkAddBenchmark.cs` | `NativeAdd10k_PerEntity` (10,000 P/Invoke crossings) vs. `NativeBulkAdd10k_SinglePInvoke` (one crossing) — the bulk-add hypothesis from `CPP_KERNEL_BRANCH_REPORT.md` §10.4. |
| `SchedulerStressBenchmarks` + `ModDependencyGraphBenchmarks` | `Stress/SchedulerStressBenchmarks.cs` | Native scheduler one-tick cost, static-graph rebuild, priority ordering (500/2,000/5,000 systems); managed mod-dependency topo-sort/presence-check throughput (500/2,000/5,000 mods) — same file, two classes. |
| `TickLoopBenchmark` + scenarios | `TickLoop/TickLoopBenchmark.cs`, `V3NativeBatchedScenario.cs`, `TickLoopScenarioBase.cs`, `MetricsCollector.cs` | Per-tick wall-clock, allocation rate, gen0/1/2 collections for the V3 native-batched scenario (50 pawns, 30 Hz). V2's managed-structs scenario was removed with the managed `World` at K8.3+K8.4; the class comment says so directly. |

Results land in BenchmarkDotNet's default artifact directory (`tests/DualFrontier.Core.Benchmarks/BenchmarkDotNet.Artifacts/results/`, untracked), with curated K7 result sets committed under `docs/benchmarks/` (`k7-bdn-tick.csv`, `k7-long-run-V*.csv`); compared by hand across runs — no automated regression comparison (§4). Census completeness note: `TickLoop/V1ManagedCurrentScenario.cs` is a worktree-placeholder stub (`NotSupportedException` on main, K7 brief §1.6 Option c) and `TickLoop/LongRunDriftRunner.cs` drives the long-run CSV path — both live in the same tree, neither is a BenchmarkDotNet class.

## 6. Cache invalidation discipline

A cache without a documented invalidation source is worse than no cache. Verified examples only — the predecessor's `PathfindingService` cache subsection is dropped: it describes a cache that was never built. `AStarPathfinding` is a stateless synchronous A* (`IPathfindingService.TryFindPath`) with no cache field anywhere in `DualFrontier.AI`, a fact the predecessor's own Hot Paths section already admitted ("code-confirmed absent... 2026-06-02") while its Caches section contradicted that by describing invalidation events for a structure that does not exist.

### `InventorySystem._freeSlotCache`

`Dictionary<int, List<string>>` — exact verified type (`InventorySystem.cs:27`), keyed by storage entity index, valued by free item-slot keys. Invalidated by a `_cacheDirty` flag (`:29`) set from three handlers — `ItemAddedEvent`, `ItemRemovedEvent`, `ItemReservedEvent` (`:36-38`, `:59`, `:74`, `:90`) — rebuilt lazily on next `Update` (`:41-48`). Single writer of `StorageComponent`, `[TickRate(TickRates.FAST)]`.

### System graph cache (managed facade only)

The managed `DependencyGraph` used by the mod-integration pipeline is built on a local instance, swapped in only after `Build()` succeeds (`ModIntegrationPipeline.cs:444`, `:726`, `:811`) — rebuilt on mod apply, not per tick. This is the managed facade's cache only; per §3, the authoritative native `system_graph.cpp` graph carries no documented budget or invalidation table of its own.

Rule, unchanged because it is sound: every cache is documented as `(key → value, invalidation sources, verified against code)`. An entry without that table does not belong in this section.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) | defers-to | К-L15 bus-tier law (§3); К-L14 performance-from-cleanliness framing (§1). |
| [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) | defers-to | GPU substrate, V1 diffusion internals, display-latency invariants (§2, §3). |
| [ECS](./ECS.md) | cites | Phase-boundary write-visibility semantics the "phase commit" row (§3) inherits. |
| [THREADING](./THREADING.md) | cites | `Parallel.ForEach` phase barrier that phase-commit visibility depends on. |
| [TESTING_STRATEGY](../methodology/TESTING_STRATEGY.md) | cites | Test/benchmark authoring practice generally. |

## Amendment protocol

Budget numbers change by direct edit plus re-verification against the cited code anchors; no ratification required pre-LOCK. Post-ratification, a budget change is a version bump citing the new anchor; adding or removing enforcement (§4) should trigger a fresh reading of AD-5's status in the session report.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 (this doc) | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R3-18..R3-21): §5 results location corrected (untracked `BenchmarkDotNet.Artifacts/` + curated sets in `docs/benchmarks/`; the cited `Results/` directory does not exist) and census completeness note added (V1 placeholder stub + LongRunDriftRunner); §1 К-L14 quote aligned to the Part 0 abbreviated row's actual wording; §3 P/Invoke-per-command attribution moved from Flush to recording. |
| 0.1.0 (this doc) | 2026-07-15 | Corpus rework: budgets re-attached to the native substrate in place of deleted managed-era rows; added §4 enforcement-honesty (no CI, no `PerformanceGate` anywhere); added §5 benchmark census; dropped the fictional `PathfindingService` cache; corrected `SpatialGrid` to its real unwired production status. |
| 1.1.1 | pre-rework | Last state of predecessor `DOC-A-PERFORMANCE` (see historical/). |
