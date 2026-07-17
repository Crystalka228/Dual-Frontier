---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-THREADING
category: A
tier: 1
lifecycle: SUPERSEDED
owner: Crystalka
version: "2.0.0"
next_review_due: null
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-THREADING
---
# Multithreading

Scheduling in Dual Frontier is split across the kernel boundary the way an OS splits it: **kernel scheduling decisions are made natively** — dependency-graph construction, runqueue maintenance, wake dispatch, phase composition — per К-L12, while system execution bodies remain managed and are dispatched from the managed side. On-demand activation (К-L13) means only runnable systems enter phase dispatch. The invariant wording is owned by [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 (К-L invariants); this document describes the mechanism as it exists in code.

## The native scheduler (К-L12)

The authoritative scheduler graph lives in the C++ kernel — `native/DualFrontier.Core.Native/src/system_graph.cpp` (`SystemGraph`, with a process-global `default_scheduler_graph()` singleton matching the "one kernel scheduler per process" model). Each system registers with a numeric id, its fully-qualified name, read/write component-type id sets, a priority class, and a wake type. The graph operates in two modes:

- **Static graph** — Kahn's topological sort over all registered systems, recomputed on registration changes (system add/remove, mod load/unload) and cached.
- **Per-tick graph** — Kahn restricted to the **runnable subset** for the current tick (К-L13 on-demand activation). The same `[SystemAccess]`-derived edges are reused; restricting the sort to woken systems produces tighter parallelism than static phase ordering.

Edge semantics mirror the managed `DependencyGraph` (transcribed deliberately, per the header comment in `system_graph.h`): an edge `A → B` means "B reads what A writes"; write-write conflicts on the same component type are errors (compute returns `-1`); cycles are errors (`-2`); each computed phase contains mutually independent systems. `last_error()` carries the diagnostic text. Per-phase barrier semantics are configurable (`Full` — the default, `Partial`, `None`).

Wake bookkeeping is the **wake registry** (`wake_registry.cpp`): per-type subscription tables for the five К-L13 wake types — Timer (subsumes `[TickRate]`), Event (bus publication), StateChange (component-value condition, fired from the write-through commit hook), Init (one-shot after `SignalEngineReady`), Explicit (API-driven wake by another system) — plus a runqueue of system ids whose wake fired since the last drain. `drain_runqueue` feeds the runnable subset into `compute_per_tick_graph`.

Worker threads come from the kernel **thread pool** (`thread_pool.cpp`): a task-queue pool with two lifecycle modes (`Bootstrap` — one-shot orchestration of the native bootstrap graph; `Scheduler` — per-tick dispatch after `SignalEngineReady`), batched submission (`submit_batch`, single queue-lock acquisition), a phase barrier (`wait_phase_barrier`), and a work-stealing policy flag (toggle present; per-thread deques are not implemented). The engine pool is constructed during the native bootstrap graph at `hardware_concurrency()` threads (fallback 4).

Pipeline-slot transitions (`pipeline_slot.cpp`, К-L16 depth-D GPU pipeline) feed the wake machinery as a separate lifecycle signal, not a sixth wake type: the `FenceCompleted → ReadableAsTail` transition fires a wake hook whose fire counter is observable (`df_pipeline_get_wake_fire_count`; managed mirror `PipelineSlotInterop`). The `[WakeOnSlotTransition]` consumer attribute exists (`src/DualFrontier.Contracts/Scheduling/WakeOnSlotTransitionAttribute.cs`); subscriber-registry integration for it is `Planned — see docs/ROADMAP.md §Native foundation tracks`.

The native graph, wake registry, and pool are exercised by `df_native_selftest` scenarios (`native/DualFrontier.Core.Native/test/selftest.cpp`, `scenario_system_graph_*`).

## The managed layer — dispatch facade

`GameBootstrap` (`src/DualFrontier.Application/Loop/GameBootstrap.cs`) registers every Core system **twice** at startup:

1. With the **native graph** via `SystemGraphInterop.RegisterSystem` + `WakeRegistryInterop.SubscribeTimer(id, 1)`, then `SystemGraphInterop.ComputeStaticGraph()`. Today this registration passes empty read/write component-id sets (marshalling `[SystemAccess]` type ids to the native registry is `Planned — see docs/ROADMAP.md §Native foundation tracks`), default priority class Normal, and a TimerWake at rate 1 — every system is woken every tick.
2. With the **managed `DependencyGraph`** (`src/DualFrontier.Core/Scheduling/DependencyGraph.cs`), which reads `[SystemAccess]` declarations via reflection once at registration, builds write-to-read edges, rejects write-write conflicts and cycles, and groups systems into ordered `SystemPhase` lists.

The production tick path then runs through `ParallelSystemScheduler` (`src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs`) — the **managed dispatch facade** retained at К10.1 closure when К-L6 ("game tick scheduler: managed") was superseded by К-L12. Per phase it executes the due systems via `Parallel.ForEach` with `MaxDegreeOfParallelism = max(1, ProcessorCount − 2)`; the blocking semantics of `Parallel.ForEach` form the implicit phase barrier; after the barrier, deferred events are flushed (see [EVENT_BUS](./EVENT_BUS.md)). `GameLoop` drives `ExecuteTick` on a dedicated simulation thread at a fixed 30 Hz step.

The cross-layer **batched callback ABI** that lets the native scheduler dispatch managed-system batches (К-L12 bridge) is on disk and test-exercised: `SchedulerAdapter` registers `ManagedSystemDispatcher.OnBatch` (an `[UnmanagedCallersOnly]` reverse-P/Invoke entry point) with the native registry (`src/DualFrontier.Application/Scheduler/`). Production per-phase dispatch does not yet route through it — the switch is `Planned — see docs/ROADMAP.md §Native foundation tracks`.

Mod (user-space) systems are tracked per-mod by `ModSubScheduler` (`src/DualFrontier.Application/Modding/ModSubScheduler.cs`): each mod ALC owns a sub-scheduler holding its registered systems, torn down on unload alongside the native per-mod state primitive (К-L12 kernel/user split).

## `[SystemAccess]` declarations

Every system declares which components it reads and writes and which bus it publishes to:

```csharp
// Abridged from src/DualFrontier.Systems/Pawn/ConsumeSystem.cs
[SystemAccess(
    reads:  new[] { typeof(NeedsComponent), typeof(JobComponent), typeof(PositionComponent) },
    writes: new[] { typeof(ConsumableComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
public sealed class ConsumeSystem : SystemBase { /* ... */ }
```

Conflict rules (identical in the managed and native graphs):

- Two systems conflict if one writes a component the other reads or writes.
- Two systems writing **different** components do not conflict.
- Two systems reading the same component do not conflict.

The attribute is read once via reflection when the system is registered — no runtime reparse. Declaration changes require a restart, which C# compilation forces anyway.

## Execution contexts (К8.3+К8.4)

Each system runs under its own `SystemExecutionContext`, pushed before `Update` and popped in `finally`, held in a `ThreadLocal<T>` slot per scheduler thread. The context carries the active `NativeWorld`, the `IGameServices` aggregator, the system's `SystemOrigin` and optional `ModId`, the fault sink, and the Path β managed-storage resolver (see [ISOLATION](./ISOLATION.md)). Isolation enforcement is compile-time: `[SystemAccess]` declarations are consumed by `DependencyGraph` for edge-building; the Roslyn analyzer's call-site layer is infrastructure-only today (see the async-ban section below). The runtime guard `HashSet` lookup that historically gated each component access was removed in К8.3+К8.4 (A'.5 closure 2026-05-14) along with the entire `GetComponent`/`SetComponent` access surface — systems read and write through `NativeWorld.AcquireSpan<T>()` / `BeginBatch<T>()` directly.

## Tick rates and wakes

Not every system needs to run every tick. `[TickRate]` sets the period; constants per `src/DualFrontier.Core/Scheduling/TickRates.cs`:

| Tick     | Period      | Systems carrying it today (sample)                      |
|----------|-------------|--------------------------------------------------------|
| REALTIME | every tick  | `ProjectileSystem`; also the default with no attribute |
| FAST     | 3 ticks     | `CombatSystem`, `DamageSystem`, `InventorySystem`, `SpellSystem` |
| NORMAL   | 15 ticks    | `ConsumeSystem`, `HaulSystem`, `ManaSystem`, `GolemSystem` |
| SLOW     | 60 ticks    | `NeedsSystem`, `MoodSystem`, `EtherGrowthSystem`       |
| RARE     | 3600 ticks  | `WeatherSystem`, `RaidSystem`, `TradeSystem`, `MapSystem` |

Managed-side, `TickScheduler` (`src/DualFrontier.Core/Scheduling/TickScheduler.cs`) keeps the monotonic tick counter and answers `ShouldRun` by `tick % ticksPerUpdate == 0`, with the per-type attribute lookup memoised; `ParallelSystemScheduler` consults it before dispatching each system inside a phase. Native-side, the same periodic semantics are the TimerWake type in the wake registry — `[TickRate]` is the timer subscription's rate. The full К-L13 wake surface is declared managed-side by the `[WakeOnEvent]` / `[WakeOnState]` / `[WakeOnInit]` / `[WakeOnExplicit]` attributes (`src/DualFrontier.Contracts/Scheduling/WakeAttributes.cs`); production bootstrap currently subscribes every Core system as TimerWake rate 1.

## Rule: async is forbidden

Inside systems, `async`/`await` are forbidden. The reason lies in `SystemExecutionContext`: it lives in `ThreadLocal`, bound to the current thread, and `await` resumes on another thread where `SystemExecutionContext.Current` is `null` — any access to `SystemBase.NativeWorld` / `SystemBase.Services` throws `InvalidOperationException` (see [ISOLATION](./ISOLATION.md)). Even with a context present, the write would happen outside the dependency graph's synchronization.

Enforcement state, stated precisely:

- **The ban is law by convention** — [CODING_STANDARDS](../methodology/CODING_STANDARDS.md) §2.7 (no LINQ, no async in system/hot-path code), with one named observed exception: `SimulationStateController.WaitForQuiescenceAsync`, a lifecycle controller, not a per-tick system.
- **Analyzer infrastructure is shipped**: `DualFrontier.Analyzers` is wired to all 12 managed src projects via `src/Directory.Build.props` and registers 17 rule stubs ([ANALYZER_RULES](./ANALYZER_RULES.md) §4 (rule registry)). The stubs are **non-detecting** — the build reports zero DFK/DFL diagnostics today; no automated enforcement of this rule exists.
- **Detection logic** is `Planned — see docs/ROADMAP.md §Analyzer track`.

What to do instead of `async`: long-running work (pathfinding, serialization) goes through the Application layer on a separate thread with results returned via an event or command; I/O happens only in Application; waiting is unnecessary — the scheduler calls the system again next phase or next tick.

## Debugging conflicts

The managed `DependencyGraph` throws `InvalidOperationException` with formatted diagnostics at `Build()` time (the native graph returns the equivalent through `last_error()`):

```
[SCHEDULER ERROR] Write conflict:
  DualFrontier.Systems.Combat.StatusEffectSystem writes HealthComponent
  DualFrontier.Systems.Combat.DamageSystem writes HealthComponent
Resolve: one of the systems must read instead of write, or be moved to a separate phase via a [Deferred] event.
```

```
[SCHEDULER ERROR] Cyclic dependency detected:
  CombatSystem → DamageSystem
  DamageSystem → CombatSystem
Resolve: break the cycle via a [Deferred] event.
```

A system registered without its declaration fails immediately: `[SCHEDULER ERROR] System '…' has no [SystemAccess] attribute.` with the attribute template to add.

Runtime faults follow the origin split (see [ISOLATION](./ISOLATION.md)): an unhandled exception from a **Core** system propagates through `Parallel.ForEach` and crashes the process — a developer bug to fix. A **mod-origin** fault is routed as `ModIsolationException` into `ModLoader.HandleModFault`, which reports through the `IModFaultSink` implementation `ModFaultHandler` (`src/DualFrontier.Application/Modding/ModFaultHandler.cs`); the mod is queued for deferred unload at the next menu open — the graph is not rebuilt mid-tick. The core does not crash; the game continues.

## See also

- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — Part 0 (К-L invariants), Part 1 (threading model, two-phase bootstrap).
- [ECS](./ECS.md) — `NativeWorld`, span/batch access protocol.
- [ISOLATION](./ISOLATION.md) — enforcement model, execution contexts, mod fault lifecycle.
- [EVENT_BUS](./EVENT_BUS.md) — deferred flush at phase boundaries; native bus tiers.
- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md) — resolving write/read loops through tick-lagged snapshots.
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md) — multi-bus responses across phases.
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md) — deterministic ordering of `DamageIntent`.
- [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) — fence-based sync between the CPU tick loop and GPU compute dispatch.

## Amendment protocol

This document is Tier 1 LOCKED. An amendment proceeds: (1) surface the change to the owner (Crystalka) — no default amendments to standing law; (2) record rationale; (3) semver — PATCH for correction, MINOR for additive sections, MAJOR for inverting described architecture; (4) bump `version` and `next_review_due` in the register mirror via a governance commit with a validate run folded in; (5) propagate to documents citing this one ([ISOLATION](./ISOLATION.md), [EVENT_BUS](./EVENT_BUS.md), [ARCHITECTURE](./ARCHITECTURE.md)).

## Change history

| Version | Date | Change |
|---|---|---|
| **2.0.0** | 2026-06-12 | Native-truth rewrite (Architecture Truth Cascade): scheduling spec re-anchored on the native `SystemGraph`/wake-registry/thread-pool (К-L12/К-L13); managed `ParallelSystemScheduler` described as the dispatch facade it is; fixed five-phase scaffold and the deleted runtime-guard exception sample removed (guard retired at К8.3+К8.4); tick rates grounded in TimerWake + verified `[TickRate]` census; async-ban section re-grounded on the realized/pending analyzer split. **MAJOR.** |
| 1.1.1 | 2026-06-02 (era) | DD-1 code-truth notice banner fencing the stale managed-scheduler body; minor pin fixes. Superseded by 2.0.0 (banner removed — the body is now code-truth). |
| 1.0 | 2026-04 (era) | Initial managed-scheduler spec: `DependencyGraph`, five-phase scaffold, `Parallel.ForEach` execution, tick rates, async ban. |
