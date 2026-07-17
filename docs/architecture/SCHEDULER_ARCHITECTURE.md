---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-SCHEDULER_ARCHITECTURE
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0.0"
next_review_due: 2027-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-SCHEDULER_ARCHITECTURE
---
# Scheduler Architecture (К10 substrate)

The native kernel scheduler: its law, its mechanical model, and the honest wiring truth of who plans and who dispatches production ticks today.

> **Document class: authored-rework (current-truth candidate).** Successor of `docs/architecture/historical/KERNEL_FULL_NATIVE_SCHEDULER.md` (DOC-A-KERNEL_FULL_NATIVE_SCHEDULER, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. Becomes the LOCKED authority upon Crystalka ratification per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7; until then the predecessor remains the last-ratified reference and prevails on conflict.
> **Ratification checklist:** [ ] content spot-audit at ratification HEAD · [ ] lifecycle AUTHORED → LOCKED, version → 1.0.0 · [ ] `next_review_due` set · [ ] predecessor register rationale updated.

**What this document is NOT.** The predecessor was a deliberation record: 46 work items, ~56 Q-N open questions, 18 performance predictions, a risk register, and the К10 deliberation-arc narrative. None of that is reproduced here — it remains readable in the historical original, and forward scheduling lives in [docs/ROADMAP.md](../ROADMAP.md) («Native foundation tracks»). This document carries exactly three things: the scheduling **law** (by pointer), the scheduler **model** (what the substrate on disk is), and the **wiring truth** (what actually runs production ticks, versus what К-L12 targets).

## Status

| Field | Value |
|---|---|
| Role | normative-current-candidate |
| Successor of | `docs/architecture/historical/KERNEL_FULL_NATIVE_SCHEDULER.md` (DOC-A-KERNEL_FULL_NATIVE_SCHEDULER) |
| Scope | Scheduling mechanics and wiring: the native scheduler substrate (system graph, wake registry, phases, barriers, priority classes, thread pool, intrinsics, batched callback ABI), the current-vs-target dispatch wiring, the mod↔scheduler interaction surface, and the scheduler verification surface |
| Non-goals | Invariant **text** ownership (К-L canon is KERNEL_ARCHITECTURE.md Part 0 — this doc quotes and elaborates, never redefines); bus mechanics and tier semantics (EVENT_BUS.md); thread census and memory-ordering law (THREADING.md); deliberation history, Q-N questions, performance predictions, risk register (historical original); cutover sequencing (ROADMAP) |
| Authority domains | Scheduler **mechanics** (what the native scheduling substrate contains and how its pieces compose) and scheduler **wiring truth** (which plane plans phases, which plane dispatches, what is installed versus deciding) |
| Defers to | KERNEL_ARCHITECTURE.md → К-L12/К-L13 (and К-L14, К-L18) canonical invariant text, Part 0; THREADING.md → managed dispatch facade, thread model, execution contexts; EVENT_BUS.md → bus tiers, EventWake traffic reality, background drain; EXECUTION_AUTHORITY_MATRIX.md (AUTHORED draft) → cutover gates and deletion triggers for the split scheduling rows |

---

## §1 Scheduling law summary

This section summarizes the law; it owns none of it. Canonical text for every invariant named here: **KERNEL_ARCHITECTURE.md Part 0**.

**К-L12 — Full native kernel scheduling** (canonical text: KERNEL_ARCHITECTURE.md Part 0). The load-bearing sentences:

> Sovereign per-tick scheduling for kernel-space systems (Core) native; managed scheduler scope reduced to user-space (mod) systems within mod ALCs. Kernel scheduling decisions are made natively: dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement. Cross-layer communication uses C ABI with batched callbacks.

К-L12 superseded К-L6 («game tick scheduler: managed») at К10.1 closure. The supersession rationale — К-L6 was migration sequencing, not architectural intent — is preserved in the historical original and in the К-L6 SUPERSEDED row of KERNEL_ARCHITECTURE.md Part 0. К-L12's falsifiability clauses bind the **post-cutover end state**; the pre-cutover wiring in §3 is the sanctioned migration state, not a falsification (see the current/target wiring annotations in KERNEL_ARCHITECTURE.md Part 0).

**К-L13 — On-demand system activation** (canonical text: KERNEL_ARCHITECTURE.md Part 0):

> Five wake types (Timer / Event / StateChange / Init / Explicit); only runnable systems enter phase dispatch; per-tick dependency graph computed on runnable subset.

**The single sanctioned reverse path.** К-L12's batched callbacks are the one legal native→managed invocation: the registered `[UnmanagedCallersOnly]` trampoline described in §2.7. Ad-hoc reverse P/Invoke, synchronous per-system callbacks, and unregistered function pointers remain forbidden — the reconciled Rule 5 wording is owned by KERNEL_ARCHITECTURE.md (dependency rules); this document describes only the mechanism.

**Adjacent law this document operates under, but does not own:** К-L14 (performance derives from architectural cleanliness — the framing under which the §2 model was default-included; canonical text pointer chain in KERNEL_ARCHITECTURE.md Part 0), К-L15 (native bus authority — EventWake's source; per К-L15 (KERNEL_ARCHITECTURE.md Part 0), traffic reality in EVENT_BUS.md), К-L16 (pipeline depth — supplies the Phase.Compute slot and the slot-transition wake signal), and К-L18 (mod lifecycle quiescent state — precondition of every scheduler-graph mutation, §4).

## §2 The native scheduler model

The substrate lives in `native/DualFrontier.Core.Native/` and is exposed managed-side through `src/DualFrontier.Core.Interop/`. Everything in this section is **on disk and test-exercised at HEAD `35364c2`**; what portion of it decides production ticks is §3's subject, and §3 wins on any tension.

### §2.1 System graph

`SystemGraph` (`native/DualFrontier.Core.Native/src/system_graph.cpp`, process-global `default_scheduler_graph()` singleton — one kernel scheduler per process) is the dependency-graph and phase-composition engine. Systems register through the C ABI — `df_scheduler_register_system` (`native/DualFrontier.Core.Native/include/df_capi.h:593`) — with a numeric id, fully-qualified name, read/write component-type-id sets, a priority class, and a wake type.

The graph operates in two modes:

- **Static graph** — Kahn's topological sort over all registered systems (`system_graph.cpp:101`), recomputed on registration change (system add/remove, mod load/unload) via `df_scheduler_compute_static_graph` (`df_capi.h:609`) and cached.
- **Per-tick graph** — the same Kahn restricted to the **runnable subset** for the current tick (`system_graph.cpp:134`; К-L13). Reusing the registration-time edges, the sort over only-woken systems produces tighter parallelism than static phase ordering.

Edge semantics deliberately transcribe the managed `DependencyGraph`: an edge `A → B` means "B reads what A writes"; a write-write conflict on one component type is an error (compute returns `-1`); a cycle is an error (`-2`, detected after Kahn drains — `native/DualFrontier.Core.Native/include/system_graph.h:23`); each phase contains mutually independent systems; `last_error()` carries diagnostics. Phase layering is a BFS Kahn — one phase per layer, ids sorted ascending for determinism (`system_graph.cpp:303`).

### §2.2 Wake registry — the five wake types

The wake registry (`native/DualFrontier.Core.Native/src/wake_registry.cpp`) is К-L13's bookkeeping: per-type subscription tables plus a runqueue of system ids whose wake fired since the last drain. The five wake types (`native/DualFrontier.Core.Native/include/wake_registry.h:34-40`, values C-ABI-stable):

| Wake type | Value | Fires when | Declaration surface (managed) |
|---|---|---|---|
| Timer | 0 | Tick counter reaches the system's declared rate; subsumes `[TickRate]` | `[TickRate]` (`TickRateAttribute`) as TimerWake sugar |
| Event | 1 | A subscribed bus event type is published (wake firing is native bus authority per К-L15 (KERNEL_ARCHITECTURE.md Part 0)) | `[WakeOnEvent(typeof(T))]` (`src/DualFrontier.Contracts/Scheduling/WakeAttributes.cs:30`) |
| StateChange | 2 | A component-value condition crossing is detected at write-batch commit (see filter below) | `[WakeOnState(typeof(T))]` (`WakeAttributes.cs:52`) |
| Init | 3 | Once, after `SignalEngineReady`; never again until next bootstrap | `[WakeOnInit]` (`WakeAttributes.cs:70`) |
| Explicit | 4 | Another system requests the wake by id | `[WakeOnExplicit(wakeId)]` (`WakeAttributes.cs:80`) |

`drain_runqueue` feeds the runnable subset into `compute_per_tick_graph`; the managed mirrors are `WakeRegistryInterop.DrainRunqueue` (`src/DualFrontier.Core.Interop/WakeRegistryInterop.cs:74`) and `SystemGraphInterop.ComputePerTickGraph` (`src/DualFrontier.Core.Interop/SystemGraphInterop.cs:139`).

**StateChange detection — the write-through hook.** The commit-time hook `df_native_world_commit_hook` routes through a two-level hybrid filter (`native/DualFrontier.Core.Native/include/state_change_filter.h`): Level 1 is a per-component-type atomic bitset (256 slots; cold-path bypass ≈ one atomic load + bit test), Level 2 a per-(type, entity) sparse hint; only on filter hit does the hook fire a type-wide StateChange wake into the registry (`native/DualFrontier.Core.Native/src/state_change_filter.cpp:106-115`). The hook fires at **batch-commit boundary**, not per write, preserving К-L7 atomic-from-observer. Per-entity condition-expression evaluation across the ABI is not implemented — the fired wake is type-wide.

**Slot-transition wake.** Pipeline-slot transitions (К-L16) feed the wake machinery as a separate lifecycle signal, not a sixth wake type: the `FenceCompleted → ReadableAsTail` transition fires a wake hook with an observable fire counter; the `[WakeOnSlotTransition]` consumer attribute exists (`src/DualFrontier.Contracts/Scheduling/WakeOnSlotTransitionAttribute.cs`) with subscriber-registry integration still forward-scheduled. Mechanism detail: THREADING.md (native scheduler section).

### §2.3 Phases and barriers

Two orthogonal phase notions coexist:

- **Graph phases** — the ordered layers the Kahn sort emits (§2.1); "phase" in dispatch discussions means these.
- **Lifecycle phases** — the named tick-lifecycle slots `Phase_Update = 0 / Phase_Compute = 1 / Phase_Display = 2` (`native/DualFrontier.Core.Native/include/phase_compute.h:48-52`). `Phase.Compute` sits between sim writes and display reads: compute dispatches registered during Update accumulate and are submitted as a single batch at the Compute boundary (`phase_compute.h:59-61`), targeting the async compute queue per К-L19. GPU-side semantics: VULKAN_SUBSTRATE.md.

Per-phase **barrier classes** are configurable on the graph (`native/DualFrontier.Core.Native/include/phase_barrier.h:12-21`; set/get via `df_scheduler_set_phase_barrier`, `native/DualFrontier.Core.Native/src/capi.cpp:1483-1488`; storage `system_graph.cpp:60-66`):

| Barrier | Semantics |
|---|---|
| `Full` (default) | All systems in phase N complete before any system in phase N+1 starts |
| `Partial` | Phase N+1 systems depending only on a subset of phase N may start when that subset completes |
| `None` | Phases may overlap; intended for non-mutating diagnostic/observability phases |

The default is `Full` — correctness-preserving; `Partial`/`None` are opt-in optimizations.

### §2.4 Priority classes, quotas, preemption, affinity

Five scheduling classes, lower value = higher priority (`native/DualFrontier.Core.Native/include/scheduling_policies.h:15-20`; managed mirror `src/DualFrontier.Contracts/Scheduling/SchedulingClasses.cs:10`):

| Class | Value | Intent |
|---|---|---|
| RealTime | 0 | Strict latency; preempts other classes; bounded execution |
| High | 1 | Interactive / input handling |
| Normal | 2 | Default; most systems |
| Low | 3 | Non-critical; deferred to phase end |
| Background | 4 | Idle-time work; may be skipped |

Declaration attributes live in Contracts: `[Priority(SchedulingClass, MaxLatencyMicros, MaxJitterMicros)]` (`SchedulingClasses.cs:39`), `[CpuQuota(MaxMicrosPerTick)]` (`SchedulingClasses.cs:65`), `[Preempt(PreemptionMode)]` (`SchedulingClasses.cs:82`; `Cooperative` default, `Forced` reserved for the RT class), and `[CpuAffinity]` (`src/DualFrontier.Contracts/Scheduling/AffinityAndBarrier.cs:11`).

The native side keeps a per-system policy table with execution accounting: `record_execution` accumulates per-tick micros and counts quota violations when a `cpu_quota_micros_per_tick` budget is exceeded (`native/DualFrontier.Core.Native/src/scheduling_policies.cpp:23-34`). This is **bookkeeping, not enforcement**: a violation increments a counter and returns a flag; no fault handler, throttle, or forced preemption is wired to it anywhere in production (the interop wrapper `SchedulingPoliciesInterop` has test/benchmark consumers only). Forced preemption itself — stack-unwind-and-restart at quota boundary — exists only as the declared `PreemptionMode.Forced` enum value.

### §2.5 Thread pool

The kernel `ThreadPool` (`native/DualFrontier.Core.Native/src/thread_pool.cpp`) is a task-queue pool with two lifecycle modes (`native/DualFrontier.Core.Native/include/thread_pool.h:45-48`): `Bootstrap` (K3-era one-shot orchestration of the native bootstrap graph) and `Scheduler` (per-tick dispatch after `SignalEngineReady`) — the transition is an atomic flag flip; worker semantics are identical in both modes. Dispatch-relevant surface: `submit_batch` (single queue-lock acquisition per batch, `thread_pool.h:69`) and `wait_phase_barrier` (`thread_pool.h:78`).

Sizing: the engine pool is constructed during the native bootstrap graph at **full `hardware_concurrency()`** threads, fallback 4 (`capi.cpp:280-282`); the short-lived bootstrap pool caps at 4 (`capi.cpp:307-310`). The N−2 core-reservation rule is a **managed-side** policy — it lives in `ParallelSystemScheduler` (`src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:90`), not in the native pool.

Work stealing: a policy **flag** exists and defaults to enabled (`thread_pool.h:94-102`, member default `thread_pool.h:122`), but per-thread deques are **not implemented** — the header itself defers the actual stealing implementation to К11+. Today the flag toggles nothing observable; all workers drain one shared queue.

### §2.6 Intrinsics, tracing, diagnostics

Emergency-path intrinsics (`native/DualFrontier.Core.Native/include/scheduler_intrinsics.h`): `suspend` (:23) / `resume` (:26) pause and restore dispatch admission; `panic_halt(message)` (:34) is the emergency stop; `snapshot` (:51) captures scheduler state into a caller buffer. C ABI: `df_scheduler_suspend` / `df_scheduler_resume` / `df_scheduler_panic_halt` / `df_scheduler_snapshot` (`capi.cpp:1227-1247`). No managed wrapper for the intrinsics exists in `src/` — they are a native-and-test surface today.

Observability: a lock-free trace ring of scheduler events with sampling **off by default** (`native/DualFrontier.Core.Native/include/scheduler_trace.h:11`), and a diagnostics API for wake-state introspection — `SchedulerDiagnostics.GetRunnableSystems` / `GetWakeSubscriptions` (`src/DualFrontier.Core.Interop/SchedulerDiagnostics.cs:23,42`) over `df_scheduler_query_runnable` and wake-subscription mask queries.

Adjacent kernel primitives registered alongside the scheduler but outside its decision surface: shared-memory regions (`native/DualFrontier.Core.Native/src/shm_region.cpp`, `src/DualFrontier.Core.Interop/ShmRegionInterop.cs` — single-writer/multi-reader IPC, no production consumers) — noted here only so their existence is not mistaken for scheduling authority. NUMA awareness from the old scope was never implemented (deferred by design).

### §2.7 Batched callback ABI

The cross-layer bridge that lets the native scheduler dispatch managed system batches — К-L12's "C ABI with batched callbacks" — is one boundary crossing per phase per origin, not per system:

- **C ABI**: `df_scheduler_register_managed_callback(cb, user_data)` and `df_scheduler_dispatch_managed_batch(batch)` (`df_capi.h:823-824`).
- **Batch struct**: `NativeManagedBatch { uint* SystemIds; uint Count; float Delta; void* UserData; }` — blittable, pointer + primitives only (`src/DualFrontier.Core.Interop/Marshalling/NativeManagedBatch.cs:11-21`).
- **Managed entry point**: `ManagedSystemDispatcher.OnBatch`, an `[UnmanagedCallersOnly(CallConvCdecl)]` static method (`src/DualFrontier.Application/Scheduler/ManagedSystemDispatcher.cs:74-75`). It resolves the dispatcher instance from the `GCHandle` passed as `UserData`, wraps the id pointer in a zero-copy `ReadOnlySpan<uint>`, and invokes the pluggable `BatchExecutor` delegate.
- **Registration**: `SchedulerAdapter.Register(dispatcher)` passes the unmanaged function pointer plus the dispatcher's GCHandle to the native registry (`src/DualFrontier.Application/Scheduler/SchedulerAdapter.cs:22-29`).

Hard constraints, encoded in the implementation (`ManagedSystemDispatcher.cs:33-42`): callback static; all arguments blittable; no generics; **no managed exception may cross the boundary** — the entry point absorbs everything in try/catch (`ManagedSystemDispatcher.cs:85-90`); `SuppressGCTransition` is forbidden for reverse P/Invoke, so the GC transition cost is accepted and amortized across the batch; the GCHandle is allocated at construction and freed at `Dispose`.

This trampoline is the **single sanctioned reverse path** of §1. Its production wiring status — currently none — is §3.

## §3 Current vs target wiring

This is the honesty core of the document. The predecessor's self-status («К10.1–К10.3 shipped, native scheduler live») described **installation**; this section describes **decision authority**. Installed is not deciding.

### §3.1 CURRENT (2026-07-15, HEAD `35364c2`)

**Every Core system is registered twice** at startup (`src/DualFrontier.Application/Loop/GameBootstrap.cs:145-181`):

1. **Managed plane — the one that decides.** The 10-system Core set is added to the managed `DependencyGraph`, which reflects over `[SystemAccess]`, builds write-to-read edges, and emits ordered `SystemPhase` lists (`GameBootstrap.cs:145-148`).
2. **Native plane — installed, not deciding.** The same systems register with the native graph via `SystemGraphInterop.RegisterSystem` — passing **empty read/write component-id sets** (`GameBootstrap.cs:173-174`), constant priority class Normal and wake type Timer (`GameBootstrap.cs:175-176`) — followed by a blanket `WakeRegistryInterop.SubscribeTimer(i, 1)` (`GameBootstrap.cs:179`) and one `SystemGraphInterop.ComputeStaticGraph()` (`GameBootstrap.cs:181`).

Consequences, stated plainly:

- With empty access sets the native graph **has no edges**, so its phase composition carries no dependency information.
- Timer-rate-1 for every system reduces К-L13's five wake types to "wake everything every tick"; the wake attributes of §2.2 exist in Contracts but **no production reader marshals them** to the native registry (`GameBootstrap.cs:177-178` names them as future overrides; repo-wide, their only mention outside Contracts is that comment).
- The per-tick machinery is production-idle: `ComputePerTickGraph` and `DrainRunqueue` have **zero production call sites** (registration in `GameBootstrap` is the only production consumer of the scheduler interop; the per-tick paths are exercised by stress/extreme suites and `df_native_selftest`).
- The StateChange write-through hook is not wired into the production write path: `df_native_world_commit_hook`'s only callers are selftest scenarios (`native/DualFrontier.Core.Native/test/selftest.cpp:1445`).

**Production dispatch is managed end-to-end.** `GameLoop` drives `ExecuteTick` on the dedicated simulation thread at a fixed 30 Hz step (`src/DualFrontier.Application/Loop/GameLoop.cs:115`); per phase, `ParallelSystemScheduler.ExecutePhase` runs due systems via `Parallel.ForEach` with `MaxDegreeOfParallelism = max(1, ProcessorCount − 2)` (`ParallelSystemScheduler.cs:149`, `:90`); the runnable filter is the managed `TickScheduler.ShouldRun` `[TickRate]` check consulted inside the loop (`ParallelSystemScheduler.cs:151`; `src/DualFrontier.Core/Scheduling/TickScheduler.cs`); the blocking join of `Parallel.ForEach` forms the phase barrier; deferred events flush after it (`ParallelSystemScheduler.cs:166-167`; semantics in EVENT_BUS.md). THREADING.md names `ParallelSystemScheduler` the **managed dispatch facade** — with the honest caveat (recorded in EXECUTION_AUTHORITY_MATRIX.md (AUTHORED draft), row notes) that a facade selecting the runnable subset is performing a К-L13 scheduling decision, which is precisely what the cutover retires.

**The batched callback ABI is on disk and test-exercised only.** The sole call site of `SchedulerAdapter.Register` in the repository is the test fixture (`tests/DualFrontier.Core.Tests/Scheduler/BatchedCallbackTests.cs:30`). Production per-phase dispatch does not route through `OnBatch`; the switch is forward-scheduled (docs/ROADMAP.md, «Native foundation tracks»).

**Where the cutover condition lived until this rework**: a code comment (`GameBootstrap.cs:150-159` — "К10.2 mechanical dispatch switch routes through the batched callback ABI … once mod ALC lifecycle context surrounds the call sites"). A comment is not a gate; the named gates are §3.3.

Summary table (mirrors the scheduling rows of EXECUTION_AUTHORITY_MATRIX.md (AUTHORED draft) §2):

| Decision | К-L12 target owner | De-facto owner today |
|---|---|---|
| Dependency edges / phase composition | Native `SystemGraph` | Managed `DependencyGraph` (`GameBootstrap.cs:145-148`) |
| Runnable-subset selection (wakes) | Native wake registry | Managed `TickScheduler.ShouldRun` (`ParallelSystemScheduler.cs:151`) |
| Parallelism + phase barrier | Native pool `submit_batch` / `wait_phase_barrier` | `Parallel.ForEach`, MaxDoP N−2 (`ParallelSystemScheduler.cs:149`, `:90`) |
| Priority arbitration / quota enforcement | Native policy table | Nobody (constants registered; native accounting unconsumed — §2.4) |
| Managed system invocation | Batched callback ABI (§2.7) | Direct managed calls inside `Parallel.ForEach` |

### §3.2 TARGET

> **FENCED (target / planned — not current truth):** Native sovereign dispatch per К-L12 (KERNEL_ARCHITECTURE.md Part 0). The native `SystemGraph` receives real `[SystemAccess]`-derived component-type ids and owns phase composition; the wake registry receives the real `[TickRate]` census plus `[WakeOn*]` declarations and selects the per-tick runnable subset; per-tick Kahn on that subset composes phases; the native pool dispatches them, invoking managed system bodies in batches through `SchedulerAdapter`/`ManagedSystemDispatcher.OnBatch` (§2.7) with mod-ALC lifecycle context surrounding the call sites; priority arbitration and quota accounting act on declared `[Priority]`/`[CpuQuota]` values. The managed `DependencyGraph` leaves production planning and `ParallelSystemScheduler` either reduces to a pure batch executor (no `ShouldRun` consultation, no phase-list ownership) or is deleted — per its own К10.1 retention clause ("may remain as managed scheduler adapter facade or be deleted", `GameBootstrap.cs:49-52`). Sequencing is ROADMAP territory; this document never says *when*.

### §3.3 Cutover gates and the deletion trigger

The gate set is specified per EXECUTION_AUTHORITY_MATRIX.md §3 (AUTHORED draft) — advisory until ratified, cited here because it is the only named cutover contract in the corpus. A split scheduling plane is tolerable only under three elements: named falsifiable gate conditions, an equivalence-proof obligation on the production workload, and a deletion trigger named in advance. Condensed:

- **GATE-S1 — real access sets.** Native registration receives `[SystemAccess]`-derived ids instead of `ReadOnlySpan<uint>.Empty`. Open while any production `RegisterSystem` call passes empty spans (`GameBootstrap.cs:173-174`).
- **GATE-S2 — phase-composition equivalence on the production set.** Managed `GetPhases()` and native static + per-tick graphs produce equivalent partitions for the real Core set over N ≥ 1000 ticks. Open while no test under `tests/` compares the two planes on the production system set (today's only native-graph exercise is `df_native_selftest` with synthetic systems).
- **GATE-S3 — dispatch switch.** Production per-phase dispatch routes through `ManagedSystemDispatcher.OnBatch` via `SchedulerAdapter`. Open while `SchedulerAdapter` has zero production call sites.
- **GATE-S4 — wake surface used beyond Timer-1.** The `[TickRate]` census maps to real TimerWake rates and at least one non-Timer wake type carries a production system — otherwise К-L13 is dead law. Open while every production wake subscription is `SubscribeTimer(i, 1)` (`GameBootstrap.cs:179`).

**Equivalence-proof obligation:** an N-tick lockstep harness asserting equal phase partitions and equal per-tick runnable subsets, recorded as К-L14 evidence rows (the evidence dashboard today carries bus-cutover rows and zero scheduler-cutover rows — the asymmetry is the measurable gap).

**DELETION TRIGGER:** when S1–S4 have held for one full release cycle, the managed `DependencyGraph` leaves production planning — removed from `GameBootstrap.cs:145-148`, demoted to test oracle or deleted — and `ParallelSystemScheduler` reduces or is deleted per §3.2. The equivalence tests then retire in the same cascade (a zombie oracle comparing against a deleted implementation is its own debt).

**Interim coexistence rules** (in force while the rows stay split, per EXECUTION_AUTHORITY_MATRIX.md §3 (AUTHORED draft)): dual registration stays mandatory for every production system; `[SystemAccess]`/`[TickRate]`/`[WakeOn*]`/`[Priority]` remain the single declarer set both planes derive from — neither plane may grow a private side-channel declaration; no new consumers of the losing plane's internals (`DependencyGraph` phase lists); semantics changes land on both planes in one commit or not at all.

## §4 Mod ↔ scheduler interaction

### §4.1 Kernel-space / user-space split

Per К-L12 (KERNEL_ARCHITECTURE.md Part 0), kernel-space (Core) systems belong to the native scheduler; user-space (mod) systems execute managed inside their ALCs, dispatched to as opaque batches. Per К-L9, mods and vanilla register through the same `IModApi` — execution layer is orthogonal to registration uniformity. The registration path: `IModApi.RegisterSystem<T>()` (`src/DualFrontier.Contracts/Modding/IModApi.cs:66`) → `RestrictedModApi.RegisterSystem` (`src/DualFrontier.Application/Modding/RestrictedModApi.cs:153-154`) → `ModRegistry.RegisterSystem(modId, type)` (`src/DualFrontier.Application/Modding/ModRegistry.cs:103`). Mod systems then enter the (today: managed) phase lists through the pipeline's graph rebuild.

Each mod ALC owns a `ModSubScheduler` (`src/DualFrontier.Application/Modding/ModSubScheduler.cs`; per-mod instances tracked by `ModRegistry.GetOrCreateSubScheduler`, `ModRegistry.cs:234`) — the user-space "process scheduler" of the OS metaphor: the kernel dispatches to the mod, not into it. Today the sub-scheduler is a per-mod system roster with teardown; within-mod scheduling policy is the mod's own concern.

### §4.2 Graph rebuild under quiescence (К-L18)

Scheduler-graph mutation happens only through the mod pipeline's prepare/commit transaction: `ModIntegrationPipeline.Apply` builds a new graph on a local, and only after `Build()` succeeds swaps the scheduler's phase list via `ParallelSystemScheduler.Rebuild` (`src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:493`; `ParallelSystemScheduler.cs:191`) — the graph is always the old consistent one or the new fully-built one. Transaction vocabulary for this pattern: per ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft).

The precondition is К-L18 (canonical text: KERNEL_ARCHITECTURE.md Part 0): mod load/unload requires **simulation paused + pipeline slots quiescent**. Enforcement is layered:

- Managed guard: the pipeline refuses `Apply`/`UnloadAll` while running (M7.1 `_isRunning` guard with canonical messages).
- Programmatic helper: `SimulationStateController` — `PauseAsync` → `WaitForQuiescenceAsync(timeout)` → mod operation → `ResumeAsync` (`src/DualFrontier.Application/Loop/SimulationStateController.cs:71,110,82`; default quiescence timeout 5 s, `:38`). Pause state reaches the native side through `df_scheduler_set_sim_paused` (`native/DualFrontier.Core.Native/src/mod_unload.cpp:36`).
- Native precondition (below): the unload primitive verifies both conditions itself and refuses to act otherwise.

Honesty note: the native paused flag defaults to **paused** (`mod_unload.cpp:17`) as a К10.2-era stub suitable for the pipeline's own default-paused discipline; wiring it to live sim-thread state end-to-end is К10.3-annotated wire-up, not completed law.

### §4.3 Native unload primitive

`df_scheduler_unload_mod_native_state(mod_id, out ModUnloadResult)` (`mod_unload.cpp:45`; contract `native/DualFrontier.Core.Native/include/mod_unload.h:85`) encapsulates per-mod native scheduler/bus teardown as **one primitive with a structured result** (per-tier counts + up to 8 error messages). Behavior at HEAD `35364c2`:

- **К-L18 precondition first, no partial teardown**: if the sim is not paused (`mod_unload.cpp:52`) or pipeline slots are not quiescent (`mod_unload.cpp:66-72`, via `df_pipeline_is_quiescent`; an uninitialized pipeline counts as quiescent), the primitive writes an error into the result and returns **before touching any state**.
- **T-sequence** (T0–T7 per `mod_unload.h:6-16`): fast-tier unsubscribe + drop (fast events are never stored, so in-flight-dropped is structurally 0), normal-tier drain-to-commit-boundary then unsubscribe, background-tier unsubscribe with queue contents preserved (untargeted persistence — events outlive their publisher's subscribers), then capability revocation / shm cleanup / wake-registry teardown / access-declaration unregistration.
- **Honest deltas from the old Item 32 text**: (a) the "single cross-tier critical section" no longer exists — after the 2026-05-21 bus state split each tier owns its own mutex, each unsubscribe acquires only its tier's lock, and the in-code comment explicitly records that the cross-tier critical-section concept is gone and that cross-tier atomicity should be reconsidered at wire-up (`mod_unload.cpp:74-79`); (b) T4–T7 are **stubs** — capability revocation, shm/affinity cleanup, wake-registry teardown (Q-N-48 order: Explicit → Init → StateChange → Event → Timer), and access-declaration unregistration all report zero pending К10.3 wire-up (`mod_unload.cpp:108-117`).

Production call site: unload chain **Step 3.5** (`ModIntegrationPipeline.cs:651-693`) — invokes the primitive via `ModUnloadInterop.UnloadModNativeState` (`:668`), surfaces its error messages as pipeline warnings, then tears down the managed `ModSubScheduler` (`:690`). Because production registers zero native subscribers today (per К-L15 wiring truth in EVENT_BUS.md), the primitive's teardown work is currently vacuous — it becomes load-bearing at the bus/scheduler cutovers. Step 3.6 is the analogous V-resource cleanup placeholder (vacuous success today; chain law in MOD_OS_ARCHITECTURE.md).

## §5 Verification surface

What can catch this document's claims — and the invariants behind them — being violated:

- **Native selftest.** `df_native_selftest` exercises the graph, wake registry, pool, intrinsics, and filter through `scenario_system_graph_*` and neighbors (`native/DualFrontier.Core.Native/test/selftest.cpp:1034` ff.). Synthetic systems only — it proves the substrate's mechanics, not managed/native equivalence on the production set (that absence is exactly GATE-S2's open condition, §3.3).
- **Managed suites.** `BatchedCallbackTests` (the ABI round-trip, sole `SchedulerAdapter.Register` exerciser — `tests/DualFrontier.Core.Tests/Scheduler/BatchedCallbackTests.cs:30`); `SchedulerStressTests` / `SchedulerExtremeTests` (native-path stress across graph/wake/policy interop); the `ManagedTestScheduler` fixture for managed-plane tests.
- **S10 cross-tier re-entrancy probe.** `S10_Bus_CrossTier_FastCallback_PublishesNormal_NoDeadlock` (`tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs:1010`) — the standing regression guard for К-L15.1's re-entrancy claim at the scheduler/bus seam.
- **Analyzer rules** (shipped, per ANALYZER_RULES.md): **DFK013** — К-L13 wake-discipline (flags `SystemBase` subclasses with neither `[WakeOn*]` nor `[TickRate]`, and eager init) — Warning, enforcing; **DFK016** — К-L16 pipeline-depth constant discipline — Warning, enforcing. The К-L12 rule **DFK012 is deferred** to the К-L20 LOCK cascade — today no tooling detects the managed planner deciding, which is the enforcement gap behind §3's split rows (pattern noted in EXECUTION_AUTHORITY_MATRIX.md (AUTHORED draft) §5).

> **FENCED (target / planned — not current truth):** **TLA+ formal verification** (old Item 18 / К10.4) — specifications for the scheduler-relevant invariant set (К-L12 progress, К-L13 wake dispatch, filter consistency, К-L16 drain, К-L18 quiescence, among the 12 targeted), a safety CI gate, and targeted liveness models. Pending per docs/ROADMAP.md («Native foundation tracks», K10.4 row). No `.tla` artifact exists in the repository at HEAD `35364c2` — and note there is currently no CI of any kind in-repo to host such a gate (session report §6.4 N-25).

## §6 Provenance note

The predecessor's lock rationale (all 9 S-surface deliberations, Q-by-Q) lives in `K10_DELIBERATION_STATE`, an **external archive that is not in the repository** — the provenance of this document's model is therefore unverifiable in-repo (session report N-15); this successor deliberately carries only what code and in-repo docs corroborate. One known stale assumption from that archive: Q-N-3 recorded "bus stays managed authority (status quo)" as the working assumption — superseded by К-L15 (KERNEL_ARCHITECTURE.md Part 0) and never reconciled in the archive.

## Cross-references

| Document | Relation | Note |
|---|---|---|
| [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) | defers-to | Canonical text of К-L12/К-L13 (and К-L6-SUPERSEDED, К-L14, К-L15, К-L16, К-L18) — Part 0; Rule 5 reconciled wording; current/target wiring annex on the К-L12 row |
| [THREADING.md](./THREADING.md) | defers-to | Thread model, execution contexts, managed dispatch facade description, slot-transition wake mechanism |
| [EVENT_BUS.md](./EVENT_BUS.md) | defers-to | Bus tiers, deferred-flush semantics at the phase barrier, EventWake/background-drain traffic reality |
| [EXECUTION_AUTHORITY_MATRIX.md](./EXECUTION_AUTHORITY_MATRIX.md) | cites (AUTHORED draft) | Cutover gates GATE-S1..S4, deletion trigger, interim coexistence rules, authority-row definitions — advisory until ratified |
| [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](./ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) | cites (AUTHORED draft) | Transaction vocabulary for the Apply/Rebuild prepare-commit and unload chain |
| [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) | cites | Mod lifecycle state machine and the unload chain that hosts Steps 3.5/3.6 |
| [ANALYZER_RULES.md](./ANALYZER_RULES.md) | cites | DFK013/DFK016 shipped status; DFK012 deferral to the К-L20 cascade |
| [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) | cites | GPU-side semantics of Phase.Compute, pipeline slots, async compute queue (К-L19) |
| [docs/ROADMAP.md](../ROADMAP.md) | cites | Forward state for all Planned markers: dispatch switch, access-set marshalling, wake wire-up, TLA+ (K10.4) |
| [FRAMEWORK.md](../governance/FRAMEWORK.md) | governance | Ratification path (§7) and authority predicate for this document's standing |

## Amendment protocol

PATCH for anchor refresh and wiring-truth corrections (every `file:line` claim re-verified at the amending HEAD); MINOR for model additions (new substrate surface). §3's CURRENT/TARGET split **must** be re-validated whenever any GATE-S row closes — a closed gate moves content across the fence, it does not get annotated around. Invariant text changes never happen here: К-L wording amendments go through KERNEL_ARCHITECTURE.md Part 0 per its own protocol, and this document follows.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 | 2026-07-17 | HALT-1-ratified review correction (CORPUS_CLOSURE_INVERSION_B, D1 R1-11): three anchor nano-drifts — `NativeManagedBatch.cs:11-19`→`:11-21` (UserData at :20); `SimulationStateController.cs:71,82,95`→`:71,110,82` (WaitForQuiescenceAsync at :110, narrative order); sub-scheduler teardown `:692`→`:690`. |
| 0.1.0 | 2026-07-15 | Initial authored rework: law/model/wiring successor of KERNEL_FULL_NATIVE_SCHEDULER.md; deliberation record (46 items, Q-N surface, predictions, risk register) retired to `historical/`; §3 corrects the predecessor's "native scheduler live" self-status to installed-not-deciding |
