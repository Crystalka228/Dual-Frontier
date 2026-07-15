---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-THREADING_V2
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: "0.1.0"
next_review_due: post-ratification closure
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-THREADING_V2
---
# Multithreading

How Dual Frontier schedules and dispatches systems across the native kernel and the managed runtime — the К-L12 dependency graph, the managed dispatch facade that executes it today, execution contexts, tick rates, the feedback-cycle rule, and the async ban.

> **Document class: authored-rework (current-truth candidate).** Successor of `docs/architecture/historical/THREADING.md` (DOC-A-THREADING, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. Becomes the LOCKED authority upon Crystalka ratification per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7; until then the predecessor remains the last-ratified reference and prevails on conflict.
> **Ratification checklist:** [ ] content spot-audit at ratification HEAD · [ ] lifecycle AUTHORED → LOCKED, version → 1.0.0 · [ ] `next_review_due` set · [ ] predecessor register rationale updated.

## Status

| Field | Value |
|---|---|
| Role | normative-current-candidate |
| Successor of | `docs/architecture/historical/THREADING.md` (DOC-A-THREADING) |
| Scope | Native `SystemGraph`/wake-registry/thread-pool; the managed dispatch facade (`DependencyGraph` + `ParallelSystemScheduler`); `[SystemAccess]`; execution contexts; tick rates; the feedback-cycle rule; the async ban |
| Non-goals | К-L invariant text (KERNEL_ARCHITECTURE.md Part 0); happens-before/lock-order model (target draft: CONCURRENCY_AND_MEMORY_MODEL.md); shutdown law (target draft: RESOURCE_OWNERSHIP_AND_LIFETIME.md); gameplay feedback-loop catalogue (FEEDBACK_LOOPS.md, Category J) |
| Authority domains | threading (dispatch mechanics, execution contexts, phase barrier, tick rates, async ban); descriptive authority over current wiring state only — the К-L12 text stays [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md)'s |
| Defers to | [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) К-L text · [EVENT_BUS.md](./EVENT_BUS.md) flush/tiers · [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) enforcement/fault lifecycle · [ECS.md](./ECS.md) storage · [CONCURRENCY_AND_MEMORY_MODEL.md](./CONCURRENCY_AND_MEMORY_MODEL.md)/[RESOURCE_OWNERSHIP_AND_LIFETIME.md](./RESOURCE_OWNERSHIP_AND_LIFETIME.md) (AUTHORED drafts) target law |

## §1 Scheduling model overview

Scheduling is split across the kernel boundary the way an OS splits it: **kernel scheduling decisions are made natively** — dependency-graph construction, runqueue maintenance, wake dispatch, phase composition — per К-L12 (KERNEL_ARCHITECTURE.md Part 0), while system execution bodies stay managed and dispatch from the managed side. On-demand activation (К-L13) means only runnable systems enter phase dispatch. Invariant wording is owned by KERNEL_ARCHITECTURE.md Part 0; this document describes the mechanism as it exists in code.

> **FENCED (target / planned — not current truth):** every "Planned" marker below describes scheduled work, not running code.

## §2 The native scheduler (К-L12)

The authoritative scheduler graph lives in the C++ kernel — `native/DualFrontier.Core.Native/src/system_graph.cpp` (`SystemGraph`, process-global `default_scheduler_graph()` singleton), exercised by `df_native_selftest` (`test/selftest.cpp:scenario_system_graph_*`). Each system registers a numeric id, FQN, read/write component-type id sets, priority class, and wake type, in two modes: **static graph** — Kahn's sort over all registered systems (`compute_static_graph`, `:90-102`); **per-tick graph** — Kahn restricted to the runnable subset (К-L13, `compute_per_tick_graph`, `:104-135`).

Edge semantics mirror the managed `DependencyGraph`: `A → B` means "B reads what A writes"; write-write conflicts return `-1` (`:269`); cycles return `-2` (`:348`); each BFS layer (`:303-335`) is a phase of mutually independent systems; `last_error()` carries diagnostics. Per-phase barrier type is configurable (`system_graph.h:47`): `Full` (0, default), `Partial` (1), `None` (2).

**Current-state note.** In production both modes sort an *empty* edge set — every system registers today with empty read/write id sets (§3) — so both collapse into **one phase containing every registered/runnable system**, with no native conflict detection or ordering. The scheduler correctly computes the graph it was given; the graph is empty. Phase composition and conflict detection are, in production, entirely the managed `DependencyGraph`'s job (§3).

The **wake registry** (`wake_registry.cpp`) holds per-type subscription tables for the five К-L13 wake types — Timer (subsumes `[TickRate]`), Event (bus publication), StateChange (write-through hook), Init (one-shot post-`SignalEngineReady`), Explicit (API-driven) — plus a runqueue of fired ids; `drain_runqueue` feeds `compute_per_tick_graph`. Worker threads come from the kernel **thread pool** (`thread_pool.cpp`): modes `Bootstrap`/`Scheduler`, batched submission (`submit_batch`), a phase barrier (`wait_idle`), a work-stealing flag present but per-thread deques unimplemented; built at `hardware_concurrency()` threads, fallback 4 (`:7-11`).

Pipeline-slot transitions (`pipeline_slot.cpp`, К-L16) feed the wake machinery as a separate signal, not a sixth wake type: `FenceCompleted → ReadableAsTail` fires a wake hook with an observable fire counter (`df_pipeline_get_wake_fire_count`, `:328`). The `[WakeOnSlotTransition]` consumer attribute exists (`WakeOnSlotTransitionAttribute.cs`).

> **FENCED (target / planned):** subscriber-registry integration for `[WakeOnSlotTransition]` — [ROADMAP.md](../ROADMAP.md) §Native foundation tracks.

## §3 The managed layer — dispatch facade

`GameBootstrap` (`src/DualFrontier.Application/Loop/GameBootstrap.cs`) registers every Core system **twice** at startup:

1. With the **native graph**, via `SystemGraphInterop.RegisterSystem` + `WakeRegistryInterop.SubscribeTimer(id, 1)`, then `ComputeStaticGraph()` (`:160-181`) — empty read/write id sets (`:173-174`), priority class Normal (`2`), TimerWake rate 1 (`:179`): every system wakes every tick. This is the empty-set registration §2 describes.
2. With the **managed `DependencyGraph`** (`src/DualFrontier.Core/Scheduling/DependencyGraph.cs`), which reads `[SystemAccess]` via reflection once at registration (`AddSystem`, `:36-63`), builds write-to-read edges over exact `System.Type` identity (`Build`, `:73-176`), rejects write-write conflicts and cycles, and groups systems into ordered `SystemPhase` lists.

> **FENCED (target / planned):** marshalling `[SystemAccess]` component ids into the native registry, so §2's graph carries real edges — [ROADMAP.md](../ROADMAP.md) §Native foundation tracks.

The production tick path runs through `ParallelSystemScheduler` (`src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs`) — the **managed dispatch facade** retained at К10.1 closure when К-L6 ("game tick scheduler: managed") was superseded by К-L12. Per phase it runs due systems via `Parallel.ForEach` at `MaxDegreeOfParallelism = Math.Max(1, ProcessorCount − 2)` (`:88-91`); the blocking join is the implicit phase barrier (`ExecutePhase`, `:144-168`); after it, deferred events flush via `IDeferredFlush.FlushDeferred` (`:166-167`; [EVENT_BUS.md](./EVENT_BUS.md)). `GameLoop` drives `ExecuteTick` on a `"SimulationLoop"` thread at a fixed 30 Hz step (`GameLoop.cs:29,64-69`).

The cross-layer **batched callback ABI** for native-driven managed dispatch (the К-L12 bridge) is on disk and test-exercised: `SchedulerAdapter.Register` (`SchedulerAdapter.cs:22`) registers `ManagedSystemDispatcher.OnBatch`, an `[UnmanagedCallersOnly]` reverse-P/Invoke entry point (`ManagedSystemDispatcher.cs:75`) — the single sanctioned reverse-callback path under Rule 5 (KERNEL_ARCHITECTURE.md §1.5). Its only callers today are `BatchedCallbackTests.cs`.

> **FENCED (target / planned):** production dispatch is planned to route through this adapter, gated per [EXECUTION_AUTHORITY_MATRIX.md](./EXECUTION_AUTHORITY_MATRIX.md) §3 (AUTHORED draft) cutover conditions; today dispatch runs exclusively through `Parallel.ForEach` above, and no deletion trigger for the managed `DependencyGraph` exists either.

Mod systems are tracked per-mod by `ModSubScheduler` (`ModSubScheduler.cs`): each mod ALC owns a sub-scheduler holding its registered systems, torn down on unload alongside native per-mod state (К-L12 kernel/user split).

## §4 `[SystemAccess]` declarations

Every system declares which components it reads and writes and which bus it publishes to:

```csharp
// From src/DualFrontier.Systems/Pawn/ConsumeSystem.cs (verified at HEAD).
[SystemAccess(
    reads: new[]
    {
        typeof(NeedsComponent), typeof(JobComponent), typeof(MovementComponent),
        typeof(PositionComponent), typeof(WaterSourceComponent),
    },
    writes: new[] { typeof(ConsumableComponent) },
    bus: nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
public sealed class ConsumeSystem : SystemBase { /* ... */ }
```

Conflict rules (identical in both graphs, per the shared contract in `system_graph.h`): two systems conflict if one writes what the other reads or writes; two systems writing **different** components don't conflict; two systems reading the same component don't conflict.

The attribute is read once via reflection at registration — no runtime reparse (`DependencyGraph.AddSystem`/`Build`; `ParallelSystemScheduler.BuildContext`, `:231-265`).

## §5 Execution contexts (К8.3+К8.4)

Each system runs under its own `SystemExecutionContext`, pushed before `Update` and popped in `finally` (`ExecutePhase`, `:149-164`), held in a `ThreadLocal<T>` slot per scheduler thread. It carries the active `NativeWorld`, the `IGameServices` aggregator, the system's `SystemOrigin`/`ModId`, the fault sink, and the Path β managed-storage resolver (MOD_OS_ARCHITECTURE.md, Path β section).

Enforcement here is narrower than it sounds: `[SystemAccess]` gates edge-building in `DependencyGraph` at registration — real and current. No shipped analyzer rule cross-checks a system's *actual* `NativeWorld` calls against its *declared* reads/writes — the 17 shipped rules cover other invariants entirely (§8) — so `[SystemAccess]` completeness is unenforced. The runtime guard that once gated each call site was removed in К8.3+К8.4 (A'.5 closure 2026-05-14) with the entire `GetComponent`/`SetComponent` surface — systems read/write through `NativeWorld.AcquireSpan<T>()` / `BeginBatch<T>()` directly, and nothing at the call site checks that against the declaration.

## §6 Tick rates and wakes

`[TickRate]` sets the update period; constants per `src/DualFrontier.Core/Scheduling/TickRates.cs`:

| Tick     | Period      | Sample, verified against `src/DualFrontier.Systems` |
|----------|-------------|--------------------------------------------------------|
| REALTIME | every tick  | `ProjectileSystem`; also the default with no attribute |
| FAST     | 3 ticks     | `CombatSystem`, `DamageSystem`, `InventorySystem`, `SpellSystem` |
| NORMAL   | 15 ticks    | `ConsumeSystem`, `HaulSystem`, `ManaSystem`, `GolemSystem` |
| SLOW     | 60 ticks    | `NeedsSystem`, `MoodSystem`, `EtherGrowthSystem` |
| RARE     | 3600 ticks  | `WeatherSystem`, `RaidSystem`, `TradeSystem`, `MapSystem` |

`TickScheduler` (`src/DualFrontier.Core/Scheduling/TickScheduler.cs`) keeps the monotonic tick counter and answers `ShouldRun` by `tick % ticksPerUpdate == 0` (`:54-63`), memoised in a `ConcurrentDictionary`. Native-side, the same period is the TimerWake type. The full К-L13 wake surface is declared by `[WakeOnEvent]`/`[WakeOnState]`/`[WakeOnInit]`/`[WakeOnExplicit]` (`WakeAttributes.cs`); production bootstrap currently subscribes every Core system as TimerWake rate 1 regardless (§3).

## §7 Feedback cycles and previous-tick snapshots

The dependency graph forbids cycles over the same components, or neither graph (§2, §3) can build phases. Game logic sometimes needs feedback anyway — one system writes a resource another reads to decide something, closing a cycle. The motivating case (a golem draining a mage's mana) is worked through in [FEEDBACK_LOOPS.md](../mechanics/FEEDBACK_LOOPS.md) (Category J); this section states the engine rule and mechanism the mechanic depends on.

**The rule** (carried over unchanged): any read that closes a cycle must go through a `_Previous` snapshot of the previous tick — the reader declares `reads: FooSnapshot` instead of `reads: FooComponent`, and something copies `FooComponent` into `FooSnapshot` once per tick. Both graphs build edges from exact `System.Type`/type-id identity (`DependencyGraph.Build`, `:79-96,202-224`; native `system_graph.cpp:217-222`), so a `FooSnapshot` reader simply cannot form an edge with `FooComponent`'s writer — no "Snapshot" name-matching exists in either graph. This is a modeling technique, not a scheduler feature. **Cost, if applied:** +1 tick latency (reader sees last tick's value); +1 O(N) copy pass per tick per participating component.

**DD-1, resolved false.** The predecessor self-flagged an unverified claim: `DependencyGraph` "marks every cycle and requires at least one side to use a `*Snapshot` component," throwing `IsolationViolationException` at registration otherwise. Checked against `DependencyGraph.cs` at HEAD: **false as stated.** `Build()` reports a cycle once, via `BuildCycleException` (`:239-260`) — a plain `InvalidOperationException` with no component-name inspection anywhere in the file. `IsolationViolationException` is thrown by nothing in the current tree; the type no longer exists as a throwable class — its only surviving mentions are past-tense doc comments (`SystemExecutionContext.cs:30`, `IModFaultSink.cs:12`, `SystemOrigin.cs:12`) describing the runtime guard deleted at К8.3+К8.4 (§5). The rule holds only because a distinct type structurally prevents the cycle, never because the graph recognizes "Snapshot" cycles specially — a real cycle fails the build like any other, regardless of naming.

**Not implemented.** No `*Snapshot` component exists anywhere in `src/DualFrontier.Components` (zero matches, repo-wide). The illustrating example is itself unbuilt — `GolemSystem.Update` is a `// TODO` no-op and `ReadPreviousTickManaState` throws `NotImplementedException` (`GolemSystem.cs:36-39,60-77`); `ManaSystem.Update` is likewise a stub — and no real cycle exists between them today: `ManaSystem` reads only `ManaLeaseOpenRequest`, so `GolemSystem` reading `ManaComponent` is a plain write→read edge, not a cycle. No generic snapshot-copy primitive exists in `NativeWorld` or the scheduler; a system using this rule must write its own copy. The predecessor's "Phase 5 — Feedback snapshot" anchor is stale — 2.0.0 removed the fixed five-phase scaffold (§3), and CONCURRENCY_AND_MEMORY_MODEL.md (AUTHORED draft) independently flags this as dangling (§9 item 4). If built, the correct anchor is the **tick boundary**, not a numbered phase — phase count is data-dependent (§2). Applicability, per the predecessor and unverified against running code: `GolemSystem`/`ManaSnapshot`, `EtherSurgeSystem`/`EtherSnapshot`, `ShieldBreakSystem`/`ManaSnapshot`. Gameplay detail: [FEEDBACK_LOOPS.md](../mechanics/FEEDBACK_LOOPS.md) (Category J).

## §8 Rule: async is forbidden

`async`/`await` are forbidden inside systems. `SystemExecutionContext` lives in `ThreadLocal`, bound to the current thread; `await` resumes on another thread where `SystemExecutionContext.Current` is `null`, so `SystemBase.NativeWorld`/`Services` throw `InvalidOperationException` (§5). Even with a context present, the write would happen outside the dependency graph's synchronization.

- **Law by convention** — [CODING_STANDARDS](../methodology/CODING_STANDARDS.md) §2.7 (no LINQ, no async in system/hot-path code); one named exception, `SimulationStateController.WaitForQuiescenceAsync`, a lifecycle controller, not a per-tick system.
- **Analyzer infrastructure is real, no longer non-detecting.** `DualFrontier.Analyzers` wires into all 12 managed `src/` projects (`Directory.Build.props:31-32`) and ships 17 rule classes (9 Architecture + 3 Discipline + 5 NativeBoundary, `tools/DualFrontier.Analyzers/Rules/`). Since А'.9.1 (Phase β detection + Phase γ promotion, both 2026-07-01) all 17 carry real detection and fire at shipped severities — 16 build-breaking (11 Error + 5 Warning), 1 IDE-only (`DFL025_B`), 2 census-pinned `DFK001` waivers ([ANALYZER_RULES.md](./ANALYZER_RULES.md) §4). This corrects the predecessor (2026-06-12, pre-Phase β/γ), which called the 17 "non-detecting stubs" reporting zero diagnostics.
- **None of the 17 is the async-ban rule**, though — they police native-boundary discipline, wake-type/pipeline-depth usage, and test discipline, not `async`/LINQ. CODING_STANDARDS.md §2.7 still says plainly: "no analyzer enforces it."

> **FENCED (target / planned):** detection of forbidden `async`/LINQ surfaces — [ROADMAP.md](../ROADMAP.md) §Analyzer track.

Instead of `async`: long-running work (pathfinding, serialization) goes through Application on a separate thread, results returned via event or command; waiting is unnecessary — the scheduler calls the system again next phase or tick.

## §9 Debugging conflicts

`DependencyGraph` throws `InvalidOperationException` with formatted diagnostics at `Build()` time (native: `last_error()`):

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

An undeclared system fails immediately: `[SCHEDULER ERROR] System '…' has no [SystemAccess] attribute.` with the template to add (`DependencyGraph.AddSystem`, `:52-59`).

Runtime faults follow the origin split (MOD_OS_ARCHITECTURE.md, mod fault lifecycle section): an unhandled **Core**-system exception propagates through `Parallel.ForEach` and crashes the process — a bug to fix. A **mod-origin** fault routes as `ModIsolationException` into `ModLoader.HandleModFault`, reported through `IModFaultSink` implementation `ModFaultHandler` (`ModFaultHandler.cs`); the mod is queued for deferred unload at the next menu open. The core does not crash.

## §10 Known gap: shutdown

`GameLoop.Stop` cancels and joins the sim thread with a 2-second bound whose result is discarded (`GameLoop.cs:73-77`); on timeout the thread is abandoned, possibly mid-tick, while `NativeWorld` and the native scheduler/bus are never deterministically torn down in production. No system's `OnDispose` runs at process exit; no production code calls native teardown outside tests.

> **FENCED (target / planned):** the quiesce → fence → teardown law and eight-step shutdown order are specified in [CONCURRENCY_AND_MEMORY_MODEL.md](./CONCURRENCY_AND_MEMORY_MODEL.md) (AUTHORED draft) §6-7 and [RESOURCE_OWNERSHIP_AND_LIFETIME.md](./RESOURCE_OWNERSHIP_AND_LIFETIME.md) (AUTHORED draft) §4 — neither ratified nor implemented.

## Cross-references

| Document | Relation | Note |
|---|---|---|
| [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) | defers-to | К-L invariant text; bootstrap; Rule 5 |
| [ECS.md](./ECS.md) | cites | `NativeWorld` span/batch protocol used inside a `SystemExecutionContext` |
| [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) | defers-to | Enforcement model, Path β resolver, mod fault lifecycle |
| [EVENT_BUS.md](./EVENT_BUS.md) · [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) | cites | Deferred flush/bus tiers; fence-based GPU sync |
| [ANALYZER_RULES.md](./ANALYZER_RULES.md) | cites | Shipped 17-rule registry and severities (§8) |
| [CONCURRENCY_AND_MEMORY_MODEL.md](./CONCURRENCY_AND_MEMORY_MODEL.md) / [RESOURCE_OWNERSHIP_AND_LIFETIME.md](./RESOURCE_OWNERSHIP_AND_LIFETIME.md) / [EXECUTION_AUTHORITY_MATRIX.md](./EXECUTION_AUTHORITY_MATRIX.md) (AUTHORED drafts) | defers-to | Target concurrency/shutdown law (§10); cutover gates (§3) |
| [FEEDBACK_LOOPS.md](../mechanics/FEEDBACK_LOOPS.md) / [COMPOSITE_REQUESTS.md](../mechanics/COMPOSITE_REQUESTS.md) / [COMBO_RESOLUTION.md](../mechanics/COMBO_RESOLUTION.md) (Category J) | cites | Gameplay applications for §7; multi-bus responses; `DamageIntent` ordering |

## Amendment protocol

Amendments surface to the owner (Crystalka) with rationale before landing — no default amendments to standing law. Semver: PATCH for correction, MINOR for additive sections, MAJOR for inverting described architecture; propagate to citing documents in the same change.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.0 (unreleased, AUTHORED) | 2026-07-15 | Successor of DOC-A-THREADING v2.0.0: new §7 absorbs the FEEDBACK_LOOPS cycle/snapshot rule (DD-1 resolved false); §8 corrected for reverse-stale analyzer claim; §2/§3 self-inconsistency fixed (native graph has zero edges in production); §10 adds the shutdown-gap pointer to the A1/A2 AUTHORED drafts. |
