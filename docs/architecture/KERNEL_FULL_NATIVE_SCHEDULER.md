# Kernel Full Native Scheduler — Architectural Specification

**Version: 2.0 (2026-05-17, this version)** — K10 architectural deliberation complete (all 9 S surfaces ratified). Major amendment per S deliberation arc: К-Lxx invariant series extended 11 → 20 (К-L6 SUPERSEDED + К-L7.1 sub + К-L12-К-L19); К10 scope extended 25 → 46 items; performance predictions restructured §5.1.A К10 + §5.1.B К11+; Q-N surface extended Q-N-1 through Q-N-49+; cross-document amendment list 9 documents; risk register extended с R-K10-6 through R-K10-9; Tier 1 LOCKED status (post-amendment).

**Version: 1.0 (2026-05-16)** — Initial К10 specification authored (deliberation-grade, pre-ratification).

**Document type**: Architectural specification (Tier 1 LOCKED, post-К10 deliberation closure)
**Scope**: К10 native kernel scheduler extension; К-series completion logic; new К-Lxx invariants
**Status**: LOCKED (Tier 1) — К10 deliberation arc 2026-05-16..2026-05-17 closed, all 9 S surfaces ratified
**Companion documents**: `KERNEL_ARCHITECTURE.md` (existing kernel authority — extended by this doc), `VULKAN_SUBSTRATE.md` (V substrate — interlocks with K10 scheduler for cross-layer dispatch), `MOD_OS_ARCHITECTURE.md` (Mod-OS metaphor — this doc realizes the kernel-space half), `K10_DELIBERATION_STATE.md` (Project file sister — deliberation rationale for all 9 S surfaces)
**Authority**: Crystalka direction 2026-05-16 «делаем как настоящий OS, производительность растёт от сложной архитектуры и чистоты»; К10 deliberation arc closure 2026-05-17

---

## Part 0 — Document positioning and authority

### 0.1 Why this document exists

`KERNEL_ARCHITECTURE.md` v1.6 LOCKED records the K-series as it exists today, with **K-L6 invariant** committing to «managed scheduler because all systems are mods». This invariant served the project through K0-K9 closure — managed scheduler was appropriate while data plane migration к native (K0-K8.4 storage cutover) was the primary architectural concern.

**Post K8.4 closure** (2026-05-14), the data plane is native: `NativeWorld` is sole production component-storage backbone per K-L11. The control plane (scheduler + dependency graph) remains managed — this is the **last managed component in the kernel**. K-L6 is no longer architecturally correct; it is **accidental complexity preserved from managed-first development era**.

This document specifies the architectural extension that closes that gap: **K10 native kernel scheduler with full OS-faithful semantics**. K10 supersedes K-L6, introduces K-L12 / K-L13 / K-L14, and properly closes the K-series as **complete native OS-faithful kernel implementation**.

### 0.2 Project framing — load-bearing context

This document operates under explicit project framing:

> «Dual Frontier = research framework, not game. Claim under test: solo dev as contract architect builds non-trivial systems-software via LLM pipeline. M-series vanilla content = demonstration surface на substrate'е.»
>
> «Производительность растёт от сложной архитектуры и чистоты, как OS.»
>
> «Без костылей, у меня много времени, а также требуется архитектурная чистота, чтобы проект жил десятилетиями.»

These statements are **architectural inputs**, not aspirational rhetoric. They constrain design space:

- «Research framework» — quality metric is architectural integrity, not shipping speed
- «Производительность от сложной чистой архитектуры» — performance is **derived** from clean complex architecture, not traded against simplicity
- «Десятилетние горизонты» — distributed-perpetual rent of architectural compromise outweighs front-loaded migration costs

**Inference for К10 scope**: complexity is positive signal when architecturally principled. Tactical heuristics («overengineered», «YAGNI», «premature optimization») are category errors in this framing — they belong in product engineering, not research framework. Default-inclusion bias for architectural items; burden of proof on **exclusion**, not inclusion.

This framing is itself a Lesson candidate (#20: «Tactical heuristics inapplicable to research framework architectural decisions; default-inclusion bias on architectural completeness»). Formalization deferred to A'.8 K-closure report per established lesson formalization timing.

### 0.3 Document authority hierarchy

```
LOCKED (existing):
├── KERNEL_ARCHITECTURE.md v1.6 — kernel architecture + K0-K9 roadmap
│   └── K-L6: «Game tick scheduler: Managed» ← SUPERSEDED by this document
├── VULKAN_SUBSTRATE.md v1.0 — V substrate primitives (V0/V1/V2)
└── MOD_OS_ARCHITECTURE.md v1.7 — Mod-OS metaphor + capability declarations

AUTHORED (this document):
├── KERNEL_FULL_NATIVE_SCHEDULER.md (this) — К10 specification + new К-Lxx invariants
│   ├── K-L12 (new): Full native kernel scheduling (supersedes К-L6)
│   ├── K-L13 (new): On-demand system activation (OS-faithful semantics)
│   └── K-L14 (new): Performance derives from architectural cleanliness (project invariant)
└── (post-Q-N-locks promotion to LOCKED)
```

Upon Q-N deliberation closure + Crystalka ratification, this document promotes к Tier 1 LOCKED. `KERNEL_ARCHITECTURE.md` amends к v2.0 (К-L6 superseded, К-L12/13/14 added, Part 2 К10 row inserted, Part 8 cross-references updated).

### 0.4 Sister deliberation artifacts

This specification is the **authoring product** of К10 deliberation, in symmetry to `COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` precedent (composite namespace deliberation, 2026-05-15, 10 Q-locks across multiple sessions).

Sister artifacts expected to accompany this document through К10 deliberation arc:
- **Q-N deliberation state document** (forthcoming): Q-N-1 through Q-N-30 (or higher) tracking, per-Q lock status, lesson candidates surfaced
- **К10 execution brief** (forthcoming, post-locks): atomic commit cascade, halt triggers, verification gates
- **К10 TLA+ formal specification** (К10 deliverable): scheduler invariants encoded in formal model

### 0.5 К10 deliberation arc completion (2026-05-17)

**К10 architectural deliberation complete** (2026-05-17). All 9 S surfaces ratified. К-Lxx invariant series expanded 11 → 20 (К-L6 SUPERSEDED + К-L12 through К-L19 + К-L7.1 sub-invariant). К10 scope expanded 25 → 46 items. Performance predictions restructured §5.1.A (К10 architecture) + §5.1.B (К11+ measurement). TLA+ specification authoring included as К10 deliverable per S-TLA Option (e) + Option (c) targeted verification scope.

Deliberation state preserved in `K10_DELIBERATION_STATE.md` sister document (Project file, not committed). Full lock rationale and Lesson candidates documented therein.

---

## Part 1 — К-L6 supersession rationale

### 1.1 Why K-L6 served, and why it stops serving

K-L6 text (from KERNEL_ARCHITECTURE.md Part 0):

> «K-L6 | Game tick scheduler | Managed (because all systems are mods) | «Vanilla = mods» principle; AssemblyLoadContext mandates managed code path для systems»

K-L6 was correct for two specific architectural moments:

**Moment 1 — pre-K-L3.1 era** (before 2026-05-10):
- All components were managed (`class` based)
- Component storage was managed (`World` registry)
- Scheduler operating on managed components in managed storage = consistent layer
- Cross-cutting K-L6 «scheduler managed» was the **simplest** invariant given the rest of the kernel was managed

**Moment 2 — К-L3.1 transition** (2026-05-10 through 2026-05-14):
- К-L3.1 introduced Path α (unmanaged struct in NativeWorld) + Path β (managed class in mod-side ManagedStore)
- К8.3+K8.4 cutover (2026-05-14) made NativeWorld sole production storage
- During transition, scheduler-managed simplified migration risk — one variable changed at a time

**Post K8.4 era** (current):
- Data plane native (`NativeWorld`)
- Control plane managed (current scheduler)
- **Layer split is now between control and data**, not within kernel
- К-L6 preserves accidental complexity from migration sequencing, not architectural intent

The «AssemblyLoadContext mandates managed code path для systems» rationale conflates two distinct concerns:
1. **Mod systems** (user-space in OS metaphor) — managed code in ALC, correct
2. **Core systems** (kernel-space in OS metaphor) — managed code because not yet migrated, **not** architecturally mandated

К-L9 «Vanilla = mods» principle still holds — but it constrains **interface uniformity** (same IModApi for vanilla and third-party), not **execution layer** (vanilla systems can be native code while still registering through IModApi).

### 1.2 What K-L6 supersession enables

Replacing K-L6 unlocks the kernel-space / user-space distinction central to Mod-OS metaphor:

| Layer | Pre-supersession | Post-supersession |
|---|---|---|
| Kernel scheduling | Managed (К-L6) | Native (К-L12) |
| Core systems | Managed | Native (К-L12 / К11+ migration) |
| Mod systems | Managed | Managed (preserved — user-space per Mod-OS) |
| Component storage | Native (К-L11) | Native (К-L11 unchanged) |
| Data plane | Native | Native |
| Control plane | Managed | Native (kernel) + Managed (user) |

This is **architecturally cleaner**:
- **OS metaphor consistent**: kernel-space (native) + user-space (managed), as real OS
- **No dual-layer scheduler authority**: one scheduler per layer (kernel scheduler dispatches Core, mod scheduler dispatches mod systems within mod ALCs)
- **К-L11 «NativeWorld single source of truth» extends к scheduler**: native control plane operates on native data plane, no boundary crossing in hot loop
- **К-L9 «Vanilla = mods» preserved**: vanilla mods still register through IModApi (which still produces managed RestrictedModApi for ALC-isolated mod code); separate concern from execution layer of native Core systems

### 1.3 К-L6 amendment text

К-L6 is **superseded but not deleted** — KERNEL_ARCHITECTURE.md preserves it as historical record with explicit supersession note. Amendment shape:

```
| K-L6 | Game tick scheduler | SUPERSEDED by K-L12 (see KERNEL_FULL_NATIVE_SCHEDULER.md) | Original rationale «Vanilla = mods» preserved as K-L9; execution layer concern factored out to K-L12 |
```

К-L12 / К-L13 / К-L14 added to К-Lxx series with full rationale in Part 0 of KERNEL_ARCHITECTURE.md v2.0.

---

## Part 2 — New invariants

### 2.1 K-L12 — Full native kernel scheduling

**Text**:

> «Native kernel owns sovereign per-tick scheduling for kernel-space systems (Core). Managed scheduler exists only for user-space (mod) systems within mod ALCs. Kernel scheduling decisions are made natively: dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement. Cross-layer communication uses C ABI with batched callbacks (managed adapter receives batched managed-system dispatches from kernel scheduler).»

**Rationale**:

- Kernel sovereignty: kernel decisions affecting all systems happen at kernel layer, not user layer
- Layer cleanness: kernel scheduling = kernel-space concern, mirrors real OS process scheduling
- Performance derivation: native control plane operates on native data plane in hot loop without boundary crossing per system
- К-L9 preserved: mods register through IModApi; vanilla mods still use same API; execution layer (where Core systems run) is orthogonal к registration uniformity

**Implication**: К-L6 superseded. Production Core systems execute through native kernel scheduler. Mod systems continue to execute through managed scheduler within mod ALCs. Batched callback ABI bridges layers for transitional managed-Core-systems (until К11+ Core migration к native) and for mod-system dispatch.

### 2.2 K-L13 — On-demand system activation

**Text**:

> «Kernel scheduler activates systems on-demand based on wake-up registration, not by global tick clock. Five wake-up types supported: Timer (periodic by [TickRate]), Event (bus publication subscription), StateChange (component value condition crossing), Init (one-shot at startup), Explicit (API-driven wake by another system). Systems not satisfying any wake condition for current tick remain blocked; only runnable systems enter phase dispatch. Per-tick dependency graph computed on runnable subset.»

**Rationale**:

- OS-faithful: real OS processes block on events / I/O / timers / semaphores, not run every quantum
- Performance derivation: idle systems skip dispatch entirely; CPU cycles preserved для actually-runnable work
- Cache locality: dispatched set is smaller, fits in cache better
- Dependency graph dynamic: per-tick Kahn on runnable subset produces tighter parallelism than static phase ordering
- Architectural completeness: real OS scheduler treats time-driven as one wake type among many; replicating that here is principled, not feature creep

**Implication**: TickScheduler refactored к WakeRegistry. [TickRate] attribute preserved as TimerWake sugar. New attributes: [WakeOnEvent<T>], [WakeOnState<T>(condition)], [WakeOnInit], [WakeOnExplicit(WakeId)]. NativeWorld component writes trigger StateChange wake dispatch (write-through hook). Bus publications trigger Event wake dispatch. Scheduler runqueue / blocked queue maintained natively.

### 2.3 K-L14 — Performance derives from architectural cleanliness

**Text**:

> «Performance is causally derived from clean complex architecture, not traded against simplicity. Each architectural addition that is principled (matches real OS pattern, serves cleanness, satisfies invariant cohesion) increases performance ceiling. Tactical reasoning («overengineered», «YAGNI», «premature optimization») does not argue against strategic architectural completeness in research framework context. Burden of proof on exclusion of architectural items, not inclusion.»

**Rationale**:

- Empirical evidence: Linux kernel, FreeBSD, Solaris, ZFS, Postgres — all dominant in their domains while being most architecturally complex
- Counter-evidence absent: «simple» systems consistently lose performance race against well-architected complex systems
- Project invariant: stated explicitly by Crystalka 2026-05-16: «производительность растёт от сложной архитектуры и чистоты»
- Decade horizon: distributed-perpetual rent of architectural shortcuts dominates front-loaded complexity cost
- Research framework: complexity is the **research subject**, not the cost — falsifies tactical-engineering bias

**Implication**: All К10 architectural items default-included unless specific architectural reason against (invariant violation, principle conflict, cross-cutting concern in single Q deliberation). Items dismissed on complexity / effort / simplicity grounds = methodology violation. Future К-series milestones (К11+) follow same default-inclusion bias.

### 2.4 К-L invariant table (consolidated, post-К10)

К-L invariant series final state after К10 deliberation closure (2026-05-17):

| ID | Status | Subject |
|---|---|---|
| К-L1 | LOCKED (pre-К10) | C++20 native language |
| К-L2 | LOCKED (pre-К10) | Pure P/Invoke к DualFrontier.Core.Native.dll |
| К-L3 (+К-L3.1) | LOCKED | Component storage paths (Path α default, Path β opt-in; К-L3.1 sub-invariant) |
| К-L4 | LOCKED (pre-К10) | Explicit type ID registry |
| К-L5 | LOCKED (pre-К10) | Declarative bootstrap graph |
| **~~К-L6~~** | **SUPERSEDED (К10)** | Game tick scheduler — superseded by К-L12 (was «Managed», now native) |
| К-L7 (+К-L7.1) | LOCKED (К-L7.1 added S8-Q2) | Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1) |
| К-L8 | LOCKED (pre-К10) | Component lifetime (native owns, managed holds IntPtr) |
| К-L9 | LOCKED (pre-К10) | Mod parity (vanilla = mods through same IModApi) |
| К-L10 | LOCKED (pre-К10) | Decision rule §8 metrics |
| К-L11 | LOCKED (pre-К10) | NativeWorld single source of truth |
| **К-L12** | **AUTHORED (К10)** | Full native kernel scheduling |
| **К-L13** | **AUTHORED (К10)** | On-demand system activation (5 wake types) |
| **К-L14** | **AUTHORED (К10)** | Performance derives from architectural cleanliness |
| **К-L15** | **AUTHORED (К10, S1 anchor)** | Native bus authority + three-tier event dispatch |
| **К-L16** | **AUTHORED (К10, S8-Q1 anchor)** | Simulation tick pipeline depth |
| **К-L17** | **AUTHORED (К10, S8-Q3 anchor)** | Display composition multi-layer |
| **К-L18** | **AUTHORED (К10, S8-Q4 anchor)** | Mod lifecycle quiescent state |
| **К-L19** | **AUTHORED (К10, S8-Q5 anchor)** | Hardware tier commitment |

**К-L6 SUPERSEDED rationale**:

> К-L6 «Game tick scheduler: Managed» served pre-K-L3.1 era + K8.3+K8.4 transition (managed scheduler simplified one-variable-at-a-time migration). Post-K8.4 (data plane native per К-L11, control plane managed), К-L6 preserves accidental complexity from migration sequencing, не architectural intent. К-L12 (full native kernel scheduling) replaces К-L6.

**К-L7.1 sub-invariant text** (full):

> «К-L7.1: GPU compute writes to RawTileField storage are bound к pipeline slot (К-L16). Sim-thread reads of compute-managed fields see slot-tail state — sim tick T+D reads dispatched-at-(T+D-1) state. One-tick lag from sim-perspective bounded и deterministic. К-L7 atomic-from-observer invariant preserved within pipeline slot boundary; cross-slot reads see different snapshots.»

**К-L15 text** (full):

> «К-L15: Native kernel owns sovereign event routing for kernel-space and cross-layer events. Bus implementation native: type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics all native authority. Bus supports three-tier dispatch (fast / normal / background) with tier declared per event type. Managed bus facade preserved as IModApi-exposed surface (К-L9 uniformity); implementation routes through C ABI bridge к native bus. Fast/background event publish/subscribe requires per-FQN per-tier capability declarations. Cross-layer event delivery uses batched callback ABI (normal/background tiers) or immediate synchronous callback (fast tier).»

**К-L16 text** (full):

> «К-L16: Simulation tick pipeline depth D ≥ 1 (configurable 1-3, default 2). Simulation thread runs D ticks ahead of display thread. Cross-layer async operations (GPU compute, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread. Display thread reads results from simulation tick (CurrentSimTick - D). Pipeline drain orderly at save/pause; pipeline refill orderly at load/resume. К-L16 establishes display latency invariant (D × tick_period); К-L15 fast tier latency invariant (subscriber response ≤1ms) preserved independently — К-L15 governs publish-to-subscribe response time, К-L16 governs sim-to-display visibility latency.»

**К-L17 text** (full):

> «К-L17: Display tick T composites multi-layer state where layers carry independent latency contracts. Sim state layers read from pipeline slot tail (К-L16 governed, D-tick lag). Intent overlay layer reads from current input state (sub-tick latency, не sim-mutating). Combat feedback layer reads from Fast tier event consumers (К-L15 latency). Composition order: sim state layers rendered first, intent and combat overlays composited on top. Layer separation maintains К-L16 invariant (sim state pipeline-bound) while enabling sub-pipeline-latency feedback channels для player intent surfaces. К-L15 fast tier subscriber latency, К-L16 sim-to-display latency preserved as independent invariants; К-L17 establishes display-composition latency invariant (intent layer ≤1 display frame ≈16ms at 60 FPS).»

**К-L18 text** (full):

> «К-L18: Mod load/unload operations require simulation paused state. Simulation thread не active during mod lifecycle changes; pipeline slots quiescent (all fences completed); no concurrent compute dispatches in-flight. Mod management UI (settings menu, hot reload tooling) enforces pause precondition. Hot reload preserves game state through managed dependency graph swap mechanism, но simulation pause required для consistent mod transition. К-L16 pipeline depth invariant naturally drains under pause; К-L17 multi-layer composition pauses with sim state. Mod lifecycle coordination protocol simplified by quiescent state precondition.»

**К-L19 text** (full):

> «К-L19: Dual Frontier requires Vulkan 1.3 hardware с async compute queue family support. Target hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20xx и newer) / AMD RDNA 1+ (RX 5000 и newer) / Intel Arc Alchemist+. Async compute queue used для К-L16 pipeline depth dispatches. Graphics queue used для display rendering. Copy/transfer queue used для asset transfers. К10 mandates queue family availability at startup; failure to detect async compute queue is fail-fast condition с clear hardware requirement diagnostic message. Hardware exclusion of pre-Turing NVIDIA, pre-RDNA AMD, pre-Arc Intel, и most integrated GPUs accepted as architectural choice supporting clean implementation. Project release timeline accounts for hardware proliferation — by Dual Frontier release, target hardware tier represents majority of gaming hardware.»

---

## Part 3 — К10 scope (46 items post-deliberation, full kernel extension)

К10 is the K-series capstone milestone — establishes native kernel scheduler with full OS-faithful semantics. К-series properly closes upon К10 + К-closure report (A'.8). К11+ optional Core systems migration к native code (deferred from К10 scope, made viable by batched callback ABI absorbing transitional period without performance regression).

Items grouped by architectural concern:

### 3.1 — Kernel scheduling primitives (items 1-5)

#### Item 1 — Native dependency graph

**Purpose**: Replace managed `DependencyGraph.cs` with native C++ implementation. Reads `[SystemAccess]` declarations marshalled from managed at startup, performs Kahn topological sort, produces phase composition.

**OS analog**: Linux dependency tracking in `linux/init/main.c` and module-init dependency resolution.

**Implementation surface**:
- Native `system_graph.h` / `system_graph.cpp` — Kahn topological sort over system access declarations
- C ABI: `df_scheduler_register_system(id, reads[], writes[], read_count, write_count, priority, wake_type, ...)` — managed side calls this at startup for each Core system
- Native side stores access declarations + computes phase composition
- Phase composition cached until system registration changes (Rebuild on mod load/unload)

**Dependencies**: Item 2 (native thread pool extension); Item 5 (dynamic graph computation).

**К-L invariant alignment**: К-L12 (kernel scheduling native).

#### Item 2 — Native thread pool extension

**Purpose**: Extend existing `thread_pool.cpp` (bootstrap-only per K-L6 era) к support per-tick system dispatch. Pool persists post-`SignalEngineReady`, transitions from bootstrap orchestration к per-tick scheduling.

**OS analog**: Linux kernel worker threads (`kworker`), pool persistence across phases of kernel lifecycle.

**Implementation surface**:
- Existing `ThreadPool` class extended: `submit_batch(tasks[], count)`, `wait_phase_barrier()`
- Worker thread loop adapted: bootstrap mode → scheduler mode transition signal
- Pool sizing follows N-2 rule (preserved from managed `ParallelSystemScheduler`)
- Per-thread affinity hints (Item 13) consumed by pool

**Dependencies**: Item 1 (graph produces task batches); Item 12 (work stealing).

**К-L invariant alignment**: К-L12.

#### Item 3 — Wake-up registry (5 wake types)

**Purpose**: Central registry mapping wake sources к subscribed systems. Native scheduler reads registry to determine runnable subset per tick.

**Five wake types** (per К-L13):

1. **TimerWake** — periodic by [TickRate]. Migration path from current TickScheduler. Native side maintains tick counter, fires TimerWake at appropriate ticks per system's declared rate.

2. **EventWake** — bus publication subscription. System declares `[WakeOnEvent<DamageEvent>]`; native scheduler tracks subscription; bus publication (managed adapter call into native) triggers wake dispatch к subscribed systems.

3. **StateChangeWake** — component value condition. System declares `[WakeOnState<Health>(condition: h => h.Current < 10)]`; NativeWorld write-through hook (Item 17) checks subscribed conditions, fires wake when crossing detected.

4. **InitWake** — one-shot at startup. System runs once after `SignalEngineReady`, never again until next bootstrap. Useful for initialization-only systems.

5. **ExplicitWake** — API-driven wake. System A calls `scheduler.WakeSystem<SystemB>(wakeId)`; native scheduler marks SystemB runnable for next phase. Used for explicit producer-consumer relationships.

**Implementation surface**:
- Native `wake_registry.h` / `wake_registry.cpp` — wake source → subscribed systems mapping
- Wake source dispatcher (5 implementations for 5 types)
- Subscription lifecycle (register at system registration; unsubscribe at unregister / mod unload)
- Runqueue / blocked queue maintenance — runqueue = systems with active wake; blocked queue = systems awaiting wake

**Dependencies**: Item 1 (system registration provides wake declarations); Item 4 (runqueue feeds dispatch); Item 17 (write-through hook for StateChangeWake).

**К-L invariant alignment**: К-L13.

#### Item 4 — Wake registry diagnostic API

**Purpose**: Observability into wake state. Real OS provides `top`, `ps`, `/proc/<pid>/wchan` for blocked process inspection. К10 provides equivalent for scheduler state.

**OS analog**: `/proc/<pid>/status` reporting process state (running, sleeping, disk-sleep, etc.); `ftrace` scheduler events.

**Implementation surface**:
- C ABI: `df_scheduler_query_runnable(out system_ids[], out count)`, `df_scheduler_query_blocked(out blocked_entries[])` where each entry includes system ID + wake type + wake source descriptor
- Managed wrapper: `SchedulerDiagnostics.GetRunnableSystems()`, `GetBlockedSystems()`, `GetWakeSubscriptions(systemId)`
- Used by future debugging tools, performance profilers, integration tests
- Roslyn analyzer (A'.9) may consume diagnostics к verify wake declarations match `[SystemAccess]` (system can't wake on event it never reads)

**Dependencies**: Item 3 (registry data); Item 19 (observability hooks).

**К-L invariant alignment**: К-L13 (system state introspection essential for on-demand model debuggability).

#### Item 5 — Dynamic per-tick graph computation

**Purpose**: Per-tick Kahn topological sort over **runnable subset** (not static graph over all registered systems). Produces tightest possible parallelism for current tick's actual work.

**OS analog**: Modern OS schedulers compute per-quantum runqueue ordering, not pre-baked schedule. CFS (Completely Fair Scheduler) computes virtual runtime per quantum.

**Implementation surface**:
- Native scheduler per tick:
  1. Read runqueue from wake registry (which systems woke this tick)
  2. Build per-tick dependency graph: edges only between runnable systems
  3. Kahn topological sort produces per-tick phase composition
  4. Dispatch phases to thread pool
- Edge construction reuses static `[SystemAccess]` data (read once at registration, cached)
- Per-tick computation O(N log N) for runnable subset; typically tens of µs for ~100 systems
- Phase composition may differ per tick — if SystemB blocks on event not fired, SystemA→SystemB→SystemC chain dispatches with A and C in parallel

**Dependencies**: Item 1 (static graph data); Item 3 (runnable subset); Item 2 (thread pool dispatch).

**К-L invariant alignment**: К-L12 + К-L13.

### 3.2 — Scheduling policies (items 6-8)

#### Item 6 — Priority-based dispatch

**Purpose**: Real-time / high / normal / low / background priority levels. Higher priority systems dispatch first within phase; resource arbitration favors higher priority.

**OS analog**: Linux SCHED_FIFO / SCHED_RR / SCHED_NORMAL / SCHED_IDLE; Windows priority classes.

**Five scheduling classes**:

1. **RealTime** — strict latency requirement; preempts other classes; bounded max execution time
2. **High** — interactive / input handling; dispatched first in phase
3. **Normal** — default; most systems
4. **Low** — non-critical work; deferred to phase end
5. **Background** — runs in idle time; may be skipped if scheduler busy

**Implementation surface**:
- Native priority queues (one per scheduling class)
- Within phase: classes dispatched in priority order; same-class systems dispatched in parallel
- System attribute: `[Priority(SchedulingClass.RealTime, MaxLatencyMicros = 100)]`
- Priority class declarations consumed by scheduler at system registration
- Diagnostic API exposes per-system priority + actual latency

**Dependencies**: Item 1 (registration); Item 7 (quotas interact with RT class); Item 5 (phase dispatch).

**К-L invariant alignment**: К-L12 (full kernel scheduling = priority arbitration is part of kernel concern).

#### Item 7 — Resource quotas (CPU time per tick per system)

**Purpose**: Per-system CPU time budget per tick. System exceeding budget triggers fault (ModFaultHandler for mod systems; configurable for Core systems).

**OS analog**: Linux cgroups CPU shares + quotas; Windows job objects CPU limits.

**Implementation surface**:
- System attribute: `[CpuQuota(MaxMicrosPerTick = 200)]`
- Native scheduler instruments system execution: timestamp begin / end / delta
- Exceedance triggers configured fault handler:
  - Mod systems: ModFaultHandler invoked, system unloaded per existing pipeline
  - Core systems: fault logged, optionally throttled (skip next N ticks), optionally degraded к lower priority
- Per-system quota accounting exposed via diagnostic API
- Aggregate quota per priority class (RT class systems share RT quota budget)

**Dependencies**: Item 6 (priority class quotas); Item 19 (observability records quota events).

**К-L invariant alignment**: К-L12 + К-L14 (clean architecture includes resource boundary enforcement).

#### Item 8 — Preemption semantics

**Purpose**: Formalize when system execution can be interrupted vs runs к completion. Required for cross-layer coordination (V substrate dispatch during long-running CPU system).

**OS analog**: Linux preemption models (CONFIG_PREEMPT_NONE / VOLUNTARY / FULL / RT).

**Two-tier model**:

1. **Cooperative preemption (default)**: systems run to completion; long-running systems should yield voluntarily via `scheduler.Yield()` API
2. **Forced preemption (RT class only)**: scheduler may interrupt RT system at quota boundary, restart on next tick

**Implementation surface**:
- Cooperative path: systems implement long-running work as multi-tick state machines; `[Preempt(Cooperative)]` attribute marks preemption points
- Forced path (RT class): native scheduler instruments execution с safe-point checks; quota exceedance triggers stack unwind + restart marker
- Preemption interacts с V substrate: V command submission can occur at cooperative preemption points within CPU system without blocking GPU pipeline
- Diagnostic API exposes preemption events

**Dependencies**: Item 6 (RT class drives forced preemption); Item 7 (quotas trigger preemption).

**К-L invariant alignment**: К-L12. К-L14: preemption is OS-faithful architectural primitive, default-include.

### 3.3 — Memory and IPC (items 9-13)

#### Item 9 — Shared memory regions

**Purpose**: High-frequency data IPC beyond bus events. Bus events have serialization overhead; for hot-path data (positions, velocities, animation state) shared memory region is faster and more cache-friendly.

**OS analog**: Linux shared memory segments (shmget/shmat), `mmap` shared mappings, kernel ringbuffers (`perf_event` ring buffer).

**Implementation surface**:
- Native side allocates region (typed by component / data structure)
- Multiple systems map к region via NativeWorld primitives
- Single-writer / multi-reader pattern (writer system declared via `[ShmWriter<T>]`)
- Lock-free reads (atomic versioning or seqlock pattern)
- C ABI: `df_shm_create(region_id, size_bytes)`, `df_shm_map(region_id)`, `df_shm_unmap(region_id)`
- Distinct from NativeWorld component storage — shm regions for cross-system data flow that doesn't fit ECS model

**Dependencies**: Item 17 (NativeWorld write-through interaction); Item 11 (CPU affinity influences region placement on NUMA systems).

**К-L invariant alignment**: К-L14 (architectural completeness — real OS provides shared memory IPC).

#### Item 10 — NUMA awareness (deferred к К-extensions)

**Purpose**: On multi-socket NUMA systems, scheduler dispatches systems к threads on cores topologically close к their data. Linux scheduler does this automatically; explicit support enables more aggressive optimization.

**OS analog**: Linux NUMA-aware scheduling, `numactl`, libnuma.

**Implementation surface**:
- Defer to К-series-2 (post-К10 extensions)
- К10 establishes infrastructure (CPU affinity, Item 11) that NUMA awareness extends
- К10 brief notes NUMA awareness as ratified-but-dormant primitive (FHE-pattern precedent)
- Development machine («Skarlet» — single-socket Ryzen 7) makes NUMA exercise impossible without remote development environment
- Decision: include NUMA in К-L12 invariant text but no implementation in К10 scope

**Dependencies**: Item 11 (CPU affinity foundation).

**К-L invariant alignment**: К-L12 (kernel scheduling includes NUMA when applicable).

#### Item 11 — CPU affinity hints

**Purpose**: Systems may declare preferred CPU core (or core set) for execution. Scheduler respects hint when scheduling.

**OS analog**: Linux `sched_setaffinity`, Windows SetThreadAffinityMask.

**Implementation surface**:
- System attribute: `[CpuAffinity(CoreSet.PerformanceCores)]` или `[CpuAffinity(CoreId = 4)]`
- Native scheduler thread pool extension: per-thread core binding
- Affinity hints consumed by graph dispatch — system prefers thread on declared core
- Useful for cache locality (system reading large dataset stays on core with that data in L1)
- Diagnostic API exposes actual vs requested affinity

**Dependencies**: Item 2 (thread pool extension); Item 10 (NUMA extension consumes affinity).

**К-L invariant alignment**: К-L12.

#### Item 12 — Work stealing within phase

**Purpose**: When phase dispatch имеет imbalanced work distribution (some threads finish early, others still busy), idle threads steal pending tasks from busy threads' queues.

**OS analog**: Linux work-stealing scheduler in `kernel/sched/`; TPL work stealing (which we'd replace).

**Implementation surface**:
- Native thread pool extension: per-thread task queues with deque structure
- Idle thread pop from own queue (LIFO); steal from neighbor queue (FIFO)
- Lock-free dequeue (atomic ops) — Cilk-style work stealing
- Scheduler dispatches phase tasks к queues; threads autonomously steal
- Reduces phase barrier wait time

**Dependencies**: Item 2 (thread pool); Item 1 (task definition from graph).

**К-L invariant alignment**: К-L12.

#### Item 13 — Phase barrier semantics formalization

**Purpose**: Explicit formalization of when full barrier between phases vs partial barrier vs no barrier.

**Three barrier semantics**:

1. **Full barrier (default)**: all systems in phase N complete before any system in phase N+1 starts
2. **Partial barrier**: phase N+1 systems that depend only on subset of phase N can start as soon as that subset completes (data-flow-driven, not phase-boundary-driven)
3. **No barrier**: phases independent enough that overlap permitted; primarily for diagnostic / observability phases that don't write component state

**Implementation surface**:
- Phase descriptor includes barrier type: `Phase(BarrierType.Full)` / `Phase(BarrierType.Partial(dependsOn: [...]))` / `Phase(BarrierType.None)`
- Scheduler honors barrier type during dispatch
- Default Full — preserves correctness; Partial/None opt-in for optimization
- Diagnostic API records actual barrier events

**Dependencies**: Item 1 (graph); Item 5 (dynamic phase composition).

**К-L invariant alignment**: К-L12 + К-L14.

### 3.4 — Native execution layer (items 14-16)

#### Item 14 — Core systems migration к native code (deferred к К11+)

**Purpose**: Migrate currently-managed Core systems (HealthSystem, MovementSystem, etc.) к native C++ implementations. Native scheduler dispatches native systems directly through function pointer table without C ABI boundary crossing.

**OS analog**: Linux kernel modules implemented in C; not user-space C# equivalents.

**Implementation surface (К11+ scope, not К10)**:
- Per-system migration: managed → native code rewrite
- Component access through native NativeWorld primitives (already exists per К-L11)
- System lifecycle (Initialize / Update / Shutdown) re-implemented native
- Native systems registered via native registration API (separate from IModApi which is for mod-loaded managed code)
- Tests parallel: managed test fixture preserved через batched callback ABI; native test fixture (catch2 / gtest) added

**К10 scope decision**: deferred to К11+. К10 batched callback ABI (item 15) absorbs transitional state without performance regression, making staged migration viable. Crystalka 2026-05-16 direction «закончить серию K» interpreted as **К10 + К-closure report = K-series complete**; К11+ optional future work outside К-series formal closure.

**Dependencies**: Item 15 (batched callback ABI for transitional Core systems); Item 16 (К-L6 supersession enables native Core).

**К-L invariant alignment**: К-L12 (kernel-space systems are native code, when migrated).

#### Item 15 — Batched callback ABI

**Purpose**: Native scheduler dispatches managed systems (mod systems + transitional Core systems) via batched reverse-P/Invoke. One C ABI boundary crossing per phase per origin, not per system.

**Technical surface** (verified through .NET 10 research):

```c
// C ABI
typedef struct {
    const uint32_t* system_ids;     // pointer to array of managed system IDs
    uint32_t count;
    float delta;
    void* user_data;                // opaque managed context handle (GCHandle)
} ManagedSystemBatch;

typedef void (*managed_batch_fn)(const ManagedSystemBatch* batch);

void df_scheduler_register_managed_callback(managed_batch_fn cb, void* user_data);
void df_scheduler_dispatch_managed_batch(const ManagedSystemBatch* batch);
```

```csharp
// Managed adapter
public static class ManagedSystemDispatcher
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void OnBatch(ManagedSystemBatch* batch)
    {
        // GC transition happens automatically on entry (cannot suppress for reverse P/Invoke)
        // Now in cooperative mode
        try
        {
            var ids = new ReadOnlySpan<uint>(batch->system_ids, (int)batch->count);
            float delta = batch->delta;
            var handle = GCHandle.FromIntPtr((IntPtr)batch->user_data);
            var dispatcher = (ManagedSystemDispatcher)handle.Target!;
            dispatcher.ExecuteBatch(ids, delta);
        }
        catch (Exception ex)
        {
            // Exceptions cannot cross to native — absorb here
            FaultLog.RecordKernelBatchFault(ex);
        }
    }

    private void ExecuteBatch(ReadOnlySpan<uint> ids, float delta)
    {
        // In-batch parallelism via TPL (existing pattern from ParallelSystemScheduler)
        // Or sequential within batch if managed-side parallelism better dispatched at native level
        foreach (uint id in ids)
        {
            var system = _systems[id];
            SystemExecutionContext.PushContext(/* ... */);
            try { system.Update(delta); }
            finally { SystemExecutionContext.PopContext(); }
        }
    }
}
```

**Performance characteristics**:
- One reverse-P/Invoke per phase per managed-system-batch (typically 1 batch per phase)
- GC transition cost (~10-50ns) amortized across N systems in batch
- ReadOnlySpan from native pointer = zero-copy
- Per-tick cost: ~10 phases × ~30ns transition = ~300ns/tick = ~9µs/sec at 30Hz — negligible
- Compared к current managed scheduler context push/pop per system: net positive (~140µs/sec savings)

**Constraints (per .NET 10 research)**:
- Callback method must be `static` ✓
- All args blittable (pointer + primitives only) ✓
- No generics ✓
- No managed exceptions across boundary ✓
- `SuppressGCTransition` forbidden for reverse P/Invoke — accept transition cost
- GCHandle for managed instance state — `Alloc()` at registration, `Free()` at shutdown

**Dependencies**: Item 1 (scheduler dispatches batches); Item 14 (transitional state where Core systems still managed).

**К-L invariant alignment**: К-L12 (cross-layer ABI bridge enabling kernel-managed dispatch).

#### Item 16 — К-L6 supersession + К-L12/13/14 amendment

**Purpose**: Formal К-Lxx series amendment. К-L6 marked SUPERSEDED. К-L12 / К-L13 / К-L14 added with full text + rationale.

**Implementation surface**:
- KERNEL_ARCHITECTURE.md v1.6 → v2.0 amendment
- Part 0 table updated: К-L6 supersession row + 3 new rows (К-L12 / К-L13 / К-L14)
- Part 2 master plan: К10 row added between К9 and К-closure
- Part 8 cross-references updated с companion citation к this document
- ROADMAP.md amendment: К-series timeline extends к include К10
- MIGRATION_PROGRESS.md amendment: post-К10 entry added at closure time

**Dependencies**: All other К10 items (К-L invariants describe what К10 establishes).

**К-L invariant alignment**: meta-item — establishes the invariants themselves.

### 3.5 — State and observability (items 17-20)

#### Item 17 — Write-through hook for NativeWorld (Amended S2)

**Purpose**: NativeWorld component writes trigger callback к check StateChangeWake subscribers, enabling on-demand activation.

**Amendment (S2 lock)**: Option C (Hybrid filter + check) + C2 batch hook timing (at commit) + Q3b hybrid filter granularity (per-type Level 1 + per-entity Level 2). Cost: cold path ~2ns Level 1 atomic load + bit test; hot path Level 1 hit ~2ns + bool check, fall through к O(K) enumeration. К-L7 invariant preserved (commit-time hook, atomic-from-observer).

**Implementation surface**:
- NativeWorld write path extended: after component value updated at **commit time** (batch commit boundary, not per-write), invoke `wake_registry.check_state_subscribers(component_type, entity_id, new_value)`
- **Hybrid filter primitive**:
  - **Level 1**: per-component-type bitset (256 bits = 32 bytes), `std::atomic<uint64_t> wake_subscriber_type_filter[4]`. Cold-path bit test eliminates check when no subscribers exist for component type.
  - **Level 2**: per-(type, entity) sparse hint, `EntityFilter { std::unordered_set<EntityId> subscribed_entities; bool has_type_wide_subscribers; }`, `std::array<EntityFilter, 256> per_type_filters`. Hot-path sparse hash lookup eliminates check when no entity-specific subscribers exist for written entity.
- Check enumerates StateChangeWake subscribers for this component type only when filter indicates presence
- Each subscriber's condition evaluated against new value
- If condition crosses (was false, now true), system added to runqueue for next phase
- Cost: cold path ~2ns; hot path Level 1 hit ~2ns + bool check; fall-through O(K) where K = subscribers for this component type, typically small
- Lock-free where possible (atomic queue insertion); filter updates atomic on subscribe/unsubscribe

**Dependencies**: Item 3 (wake registry).

**К-L invariant alignment**: К-L13 (on-demand model requires state change detection mechanism); К-L7 atomic-from-observer preserved (commit-time hook = no mid-transaction observer state).

#### Item 18 — Formal verification (TLA+ specification) (Amended S2 + S-TLA)

**Purpose**: Formal model of scheduler invariants in TLA+ or similar. Verifies absence of deadlock, starvation, priority inversion, race conditions in scheduler logic itself.

**OS analog**: seL4 microkernel formally verified в Isabelle/HOL. Linux scheduler subjected к Spin model checking research.

**Amendment (S-TLA lock)**: TLA+ specification authoring covers **12 invariants** (К-L3, К-L4, К-L5, К-L7, К-L7.1, К-L8, К-L11, К-L12, К-L13, К-L15, К-L16, К-L18). Safety verification CI gate (all 12 invariants, ~1-2 hour runtime). Liveness verification deeper models для К-L7.1 (fence completion), К-L12 (scheduler progress), К-L16 (pipeline drain). Spec authoring ~2-3 months parallel с К10 implementation; safety CI integration ~2 weeks; liveness verification authoring ~1-2 months post-implementation.

**Amendment (S2 lock)**: Specification includes **filter consistency invariants** (subscribe/unsubscribe updates filter atomically), batch-commit semantics для state change detection.

**Implementation surface**:
- TLA+ specifications at `docs/formal/SCHEDULER_TLA_PLUS.tla` (and per-invariant modular files when scope demands)
- Model encodes: system states (blocked / runnable / executing), wake events, phase boundaries, dependency edges, pipeline slots (К-L16), filter Level 1/2 (S2), bus tier dispatch (К-L15), quiescent state (К-L18)
- Invariants checked (12 total):
  - Safety: no two systems writing same component concurrently (К-L11); filter consistency (S2); batch-commit semantics (К-L7); pipeline slot binding (К-L7.1); scheduler progress (К-L12); on-demand wake dispatch (К-L13); event routing tier preservation (К-L15); pipeline depth invariant (К-L16); quiescent-state mod lifecycle (К-L18)
  - Liveness: every wake event eventually dispatches к subscribed systems (К-L13); fence completion within pipeline window (К-L7.1); pipeline drain orderly at save/pause (К-L16)
  - Priority: high-priority systems dispatch before lower-priority in same phase (К-L12)
  - No deadlock: dependency graph acyclic (verified at registration); no resource lock cycles
- TLC model checker run against representative state space (bounded); safety CI gate runs as test
- Document explains TLA+ specification and verification approach
- Future К-series amendments may extend specification

**Dependencies**: Item 1 (graph definitions); Item 3 (wake semantics); Item 6 (priority semantics); Items 26, 33, 41 (К-L15/16/18 mechanisms).

**К-L invariant alignment**: К-L14 (architectural completeness — formal verification of scheduler invariants befits research-grade systems software).

#### Item 19 — Observability hooks (perf/ftrace-like tracing) (Amended S2)

**Purpose**: Per-tick trace events for scheduler decisions. Enables post-deployment profiling, debugging, performance analysis at scale.

**OS analog**: Linux `ftrace`, eBPF tracing, perf_event subsystem.

**Amendment (S2 lock)**: Additional metrics for filter primitive — filter hit/miss ratio (Level 1 cold-path bypass rate, Level 2 entity-specific hit rate), average subscriber count per state change (validates К-L13 wake dispatch cost model).

**Implementation surface**:
- Native trace ring buffer (lock-free, per-thread or shared)
- Trace events emitted by scheduler:
  - `system_woken(system_id, wake_type, source)`
  - `system_dispatched(system_id, thread_id, phase, priority)`
  - `system_completed(system_id, duration_micros)`
  - `phase_started(phase_id, runnable_count)`
  - `phase_completed(phase_id, duration_micros)`
  - `quota_violation(system_id, budget_micros, actual_micros)`
  - `filter_hit(component_type, level, subscriber_count)` (S2)
  - `filter_miss(component_type, level)` (S2)
- C ABI: `df_scheduler_trace_dump(out_buffer, size)`, `df_scheduler_trace_clear()`
- Managed wrapper consumes traces для analysis tools
- Default sampling rate configurable (full / 1-in-N / off)
- Aggregated metrics: filter hit/miss ratio, average subscriber count per state change (S2)
- Replaces ad-hoc logging in current scheduler

**Dependencies**: Item 1 (scheduler instrumentation points); Item 4 (diagnostic API consumes traces).

**К-L invariant alignment**: К-L14 (observability is architectural completeness for sovereign kernel).

#### Item 20 — Scheduler intrinsics for emergency paths

**Purpose**: Atomic scheduler self-modification primitives. Used during hot reload, state migration, debugging snapshots, panic conditions.

**OS analog**: Linux `cli`/`sti` (interrupt enable/disable), `local_irq_disable`, `stop_machine` (suspend all cores for global state change).

**Implementation surface**:
- C ABI: `df_scheduler_suspend()` — pause all dispatch; running systems complete, no new dispatch
- `df_scheduler_resume()` — resume normal operation
- `df_scheduler_panic_halt(message)` — emergency stop; flush traces, dump state
- `df_scheduler_snapshot(out_state_buffer, size)` — atomic snapshot of runqueue + blocked queue + wake subscriptions for debugging
- Suspend/resume used during `Rebuild` (mod load/unload causing graph changes)
- Panic path for unrecoverable invariant violations (cycle in graph despite Kahn check, quota violation in RT class, etc.)
- Managed wrapper provides `using var pause = scheduler.SuspendDispatching()` IDisposable pattern

**Dependencies**: Item 1 (registry suspension); Item 19 (panic flushes traces).

**К-L invariant alignment**: К-L12 + К-L14.

### 3.6 — Mod integration (items 21-23)

#### Item 21 — Mod scheduler authority (per-mod sub-schedulers) (Amended S3)

**Purpose**: Each mod has own scheduler for its systems within mod ALC. Kernel scheduler treats mod-system batch as opaque dispatch к mod-side scheduler.

**Mod-OS metaphor**: real OS processes have own event loops, async runtimes, internal scheduling. Kernel doesn't arbitrate inside processes.

**Amendment (S3 lock)**: Per-mod sub-scheduler **teardown** encapsulated в native primitive (single critical section). Teardown flow is L3 layering (S3-Q1): encapsulated atomicity, single critical section semantics, T0-T7 internal sequence (S3-Q6 single primitive с structured `ModUnloadResult` return). Insertion point: NEW step 3.5 within existing `ModIntegrationPipeline.UnloadMod` 7-step chain (see Item 32).

**Implementation surface**:
- Each mod ALC contains mod-scheduler instance
- ModRegistry tracks per-mod system ownership; kernel scheduler dispatches к mod-scheduler, not individual mod systems
- Mod-scheduler internal: small managed scheduler (TPL-based, similar к current ParallelSystemScheduler но scoped к single mod)
- Cross-mod communication: via buses (existing) + capability-declared kernel calls
- Within-mod: mod owns scheduling decisions (priority, ordering, parallelism)
- **Teardown primitive (S3-Q1 L3 + S3-Q6 single primitive)**: `df_scheduler_unload_mod_native_state(mod_id) → ModUnloadResult` (see Item 32). Single native C ABI call encapsulates T0-T7 sequence (subscription cleanup + wake registry teardown + per-mod queue handling per S3-Q2 per-tier policy + capability revocation).

**К10 vs К8.5 boundary**: К10 establishes infrastructure (kernel scheduler dispatches managed batches per Item 15); К8.5 documents mod authoring practice (how mod-scheduler relates к [SystemAccess], mod system ordering within mod, etc.).

**Dependencies**: Item 15 (batched callback ABI to dispatch mod-system batch); Item 32 (native unload primitive); Item 41 (К-L18 quiescent state enforcement).

**К-L invariant alignment**: К-L12 (kernel and mod scheduling separation = layer cleanness); К-L18 (mod lifecycle changes require quiescent state).

#### Item 22 — Hot-patching schedulable code

**Purpose**: Decade-horizon dev workflow benefits from Core system hot replacement (not just mod hot reload).

**OS analog**: Linux kernel `kpatch` / `kgraft` for live kernel patching.

**Implementation surface (К10 establishes primitives, К-extensions implement full)**:
- К10 includes function-pointer-based system dispatch (Item 2 + Item 14)
- Function pointer table mutation primitive: `df_scheduler_replace_system(system_id, new_function_ptr)`
- Suspend-replace-resume pattern via Item 20 intrinsics
- Full hot-patching workflow (build new code, link к running process, swap function pointers) deferred к К-extensions
- К10 ratifies primitive, К-extensions implement workflow

**Dependencies**: Item 20 (suspend/resume); Item 2 (function pointer dispatch); Item 14 (К11+ context).

**К-L invariant alignment**: К-L14.

#### Item 23 — Real-time scheduling guarantees

**Purpose**: RT class systems (Item 6) provide guaranteed latency bounds. Useful for input handling, audio, V substrate dispatch coordination.

**OS analog**: Linux SCHED_DEADLINE / PREEMPT_RT patchset.

**Implementation surface**:
- RT class attribute extended: `[Priority(SchedulingClass.RealTime, MaxLatencyMicros = 100, MaxJitterMicros = 20)]`
- Native scheduler enforces:
  - RT systems dispatched first in phase (preempts other classes)
  - Forced preemption (Item 8) on quota exceedance
  - Jitter monitoring via observability hooks (Item 19)
  - Best-effort initially; «strict» variant K-extensions
- Diagnostic API exposes actual latency / jitter per RT system
- Documentation explicitly states «best-effort» vs «hard real-time» distinction (game engine context = best-effort sufficient)

**Dependencies**: Item 6 (priority class); Item 7 (quotas); Item 8 (preemption); Item 19 (jitter measurement).

**К-L invariant alignment**: К-L12 + К-L14.

### 3.7 — Test infrastructure and migration (items 24-25)

#### Item 24 — Test infrastructure migration

**Purpose**: Existing managed scheduler tests (DependencyGraphTests, ParallelSystemSchedulerTests, etc.) migrate к native equivalents or preserve via managed test fixture.

**Pattern precedent**: К8.3+К8.4 cutover handled World retirement via `ManagedTestWorld` test fixture. Same pattern applies к scheduler.

**Implementation surface**:
- Native test framework decision: catch2 (header-only, modern C++17+) or gtest (more mature, more boilerplate)
- Native scheduler tests in `native/DualFrontier.Core.Native/test/scheduler_test.cpp`
- Managed test surface preserved где applicable: `ManagedTestScheduler` fixture for tests that don't go through native path (similar к `ManagedTestWorld`)
- Existing test count baseline (~620 post-A'.5) maintained; new native tests additive
- Integration tests bridge managed-side к native-side (via batched callback ABI) for end-to-end coverage

**Dependencies**: All К10 items (tests cover scheduler functionality).

**К-L invariant alignment**: К-L14 (architectural completeness includes test infrastructure).

#### Item 25 — К-closure report (A'.8) — К-series formal closure

**Purpose**: К-closure report documents К-series complete state. Dual purpose:
1. Historical record: enumeration of K-L1 through K-L14 (final invariant set) с rationale for each
2. Roslyn analyzer rule specification surface: each К-Lxx invariant maps к analyzer rules in А'.9

**Implementation surface (А'.8 milestone, post-К10)**:
- Document at `docs/architecture/K_SERIES_CLOSURE_REPORT.md` (Tier 1 LOCKED)
- Per-invariant format:
  - Formal statement (verbatim from KERNEL_ARCHITECTURE.md v2.0 Part 0)
  - Rationale (why this invariant exists)
  - Violation example (code that breaks it)
  - Compliance example (code that respects it)
  - Analyzer rule specification (what А'.9 Roslyn rule encodes this invariant)
- Phase A' sequencing: К10 (this doc closes) → А'.8 К-closure report → А'.9 Roslyn analyzer → Phase B M-series

**Dependencies**: К10 closure (all items 1-46 land); analyzer milestone consumes closure report.

**К-L invariant alignment**: meta-item — captures all K-Lxx invariants for posterity.

### 3.8 — Native bus three-tier dispatch (items 26-30, S1 lock)

#### Item 26 — Native bus implementation (three-tier dispatcher)

**Purpose**: Native authority for kernel-space and cross-layer event routing per К-L15. Type registry, normal/fast/background subscriber registries, three dispatch paths.

**Three-tier dispatch semantics** (per К-L15):

| Tier | Latency | Dispatch path | Subscriber contract | Use cases |
|---|---|---|---|---|
| **Fast** | Immediate (≤1ms target) | Synchronous bypass, preemption-aware | Bounded exec, no blocking, no GC alloc, RT class | Combat hits, player input, emergency |
| **Normal** | Within tick | Batched callback per-phase | Standard | AI signals, lifecycle, bulk |
| **Background** | Multi-tick acceptable (idle slots) | Coalesce'инг + idle-slot dispatch | Long-running ok, interruptible | Off-screen simulation, climate, quest gen |

**Implementation surface**:
- Native `bus_native.h` / `bus_native.cpp` — three subscriber registries (per-tier), event type registry (tier-annotated per event)
- Per-tier dispatch paths: Fast (synchronous bypass), Normal (batched callback per-phase), Background (coalesce + idle-slot)
- Native authority: type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics
- C ABI: `df_bus_publish_fast(type_id, payload)`, `df_bus_publish_normal(type_id, payload)`, `df_bus_publish_background(type_id, payload, coalesce_key)`, `df_bus_subscribe(type_id, tier, callback_fn, context)`

**Dependencies**: Item 27 (managed facade); Item 28 (type registry); Item 30 (background queue).

**К-L invariant alignment**: К-L15.

#### Item 27 — Managed bus facade + C ABI bridge

**Purpose**: Preserve IModApi-exposed surface (К-L9 uniformity) while routing through C ABI bridge к native bus.

**Implementation surface**:
- `DualFrontier.Application.BusFacade` (or similar managed assembly) — IModApi-exposed bus API
- Forward P/Invoke per tier: `df_bus_publish_fast`, `df_bus_publish_normal`, `df_bus_publish_background`
- Reverse callback per tier: Fast tier uses immediate synchronous callback ABI; Normal/Background tiers use batched callback ABI (similar к Item 15 batched system dispatch)
- Marshalling: payload structs blittable; mod-defined event types resolved through type registry (Item 28)
- К-L9 uniformity preserved: vanilla and mod systems use same managed bus API

**Dependencies**: Item 26 (native bus); Item 28 (type registry); Item 15 (batched callback ABI pattern).

**К-L invariant alignment**: К-L9 + К-L15.

#### Item 28 — Event type registry (tier-annotated)

**Purpose**: Per-event-type metadata including tier field + payload size constraints.

**Implementation surface**:
- Native `event_type_registry.cpp` — type_id → (tier, payload_size, coalesce_function, capability_token) mapping
- Event types declared at registration time (mod or core); tier field mandatory per event
- Payload size constraints per tier (Q-N-32 open Q): fast ≤ 64 bytes cache-line target, normal unlimited, background coalesce-friendly
- Capability token format: `kernel.{tier}.publish:{FQN}`, `kernel.{tier}.subscribe:{FQN}` (S3-Q5 lock)
- Type registry mirrored к managed adapter для marshalling

**Dependencies**: Item 26 (native bus consumes registry); Item 29 (contract enforcement reads tier).

**К-L invariant alignment**: К-L15 (tier declared per event type).

#### Item 29 — Subscriber contract enforcement (per-tier validation)

**Purpose**: Registration validation per tier subscriber contract + runtime instrumentation.

**Implementation surface**:
- Registration validation: Fast tier subscribers verified bounded exec contract (no blocking calls, no GC alloc — analyzer rule deferred к A'.9 Roslyn); Background tier subscribers verified interruptible
- Runtime instrumentation: Fast tier subscriber latency measured (deadline ≤1ms per К-L15); violation triggers fault handler (similar к Item 7 quota)
- Per-tier diagnostic API: report subscriber-contract-violation events
- Capability token enforcement: subscribe call requires `kernel.{tier}.subscribe:{FQN}` capability (per Item 28)

**Dependencies**: Item 26 (subscriber registration); Item 28 (capability tokens); Item 7 (quota enforcement pattern); Item 19 (instrumentation).

**К-L invariant alignment**: К-L15.

#### Item 30 — Background work queue + idle-slot scheduling

**Purpose**: Background tier work queue management + idle-slot detection + saturation prevention.

**Implementation surface**:
- Native `background_queue.cpp` — per-event-type background queue с coalesce function applied at publish time
- Coalesce'инг: per-type coalesce function declared by event author (default LIFO, per-key replace, etc.); 1000 WorldStateChanged events → ~100 batched dispatches
- Multi-tick spread: 1/N of queue per tick во время idle slots (idle slot = scheduler CPU budget measurement, Q-N-35 open Q)
- Saturation handling (Q-N-36 open Q): drop oldest / backpressure publisher / expand budget — deferred к brief authoring
- Persistence integration (S3-Q3 save-integrated): see Item 31

**Dependencies**: Item 26 (native bus); Item 31 (save-integrated storage).

**К-L invariant alignment**: К-L15 (background tier dispatch semantics).

### 3.9 — Mod ALC lifecycle native primitives (items 31-32, S3 lock)

#### Item 31 — Background queue save-integrated storage (S3-Q3)

**Purpose**: Persist background tier event queue с save file (game-world state continuity across save/load).

**Implementation surface**:
- Native serialization protocol: `df_scheduler_serialize_background_queue(out_buffer, size)` / `df_scheduler_deserialize_background_queue(buffer, size)`
- DualFrontier.Persistence integration: background queue blob included в save-game format
- Versioning concerns (Q-N-43 / Q-N-44 open): blittable struct array + type_id table; schema migration на event types changing between game versions (R-K10-6 risk)
- Untargeted persistence per S3-Q4: queue persists для future subscribers (mod replacement pattern)
- Size cap (Q-N-45 open): memory boundary; OOM prevention — deferred к brief authoring

**Dependencies**: Item 30 (background queue); DualFrontier.Persistence amendment.

**К-L invariant alignment**: К-L15.

#### Item 32 — Native unload primitive + ModUnloadResult (S3-Q1/Q6)

**Purpose**: Single native C ABI primitive encapsulating per-mod scheduler teardown (per S3-Q1 L3 layering + S3-Q6 single primitive).

**Implementation surface**:
- C ABI: `df_scheduler_unload_mod_native_state(mod_id) → ModUnloadResult`
- Internal T0-T7 sequence:
  - T0: Acquire scheduler critical section
  - T1: Per-tier in-flight policy application (S3-Q2): fast drop / normal drain / background persist
  - T2: Wake registry teardown for mod's systems (Q-N-48 open: 5 wake types ordering)
  - T3: Subscription cleanup for mod's bus subscriptions (per-tier)
  - T4: Capability revocation for mod's per-FQN per-tier capabilities (S3-Q5)
  - T5: Shared memory region destruction (Q-N-49 open: ShmWriter atomicity)
  - T6: Mod's scheduler instance teardown (small managed scheduler within mod ALC)
  - T7: Release scheduler critical section
- Structured `ModUnloadResult` return: success status, per-tier outcome (fast events dropped count, normal events drained count, background events persisted count), error diagnostics
- Insertion point: NEW step 3.5 calling primitive within existing `ModIntegrationPipeline.UnloadMod` 7-step chain

**Dependencies**: Item 21 (mod sub-scheduler); Item 26 (native bus subscription cleanup); Item 30 (background queue handling per-tier policy); Item 41 (К-L18 quiescent state precondition).

**К-L invariant alignment**: К-L12 + К-L18.

### 3.10 — Tick pipeline depth (items 33-35, S8-Q1 lock + Item 33 extended S8-Q2)

#### Item 33 — Tick pipeline depth mechanism (S8-Q1 + S8-Q2)

**Purpose**: Snapshot storage per pipeline slot, slot management, fence orchestration per slot.

**Pipeline slot data model** (S8-Q2 amendment):

```c++
struct PipelineSlot {
    uint64_t sim_tick;
    NativeWorld snapshot;
    FieldStorageSnapshot fields;        // S8-Q2: K-L7.1 binding
    FenceHandle compute_fence;
    enum { Dispatched, FenceCompleted, ReadableAsTail } state;
};
```

**Implementation surface**:
- Configurable depth D = 1-3 (default 2) per К-L16
- Simulation thread runs D ticks ahead of display thread
- Cross-layer async operations (GPU compute, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread
- Display thread reads from `CurrentSimTick - D` (slot tail)
- К-L7.1 binding (S8-Q2): GPU compute writes к RawTileField storage bound к pipeline slot; sim-thread reads of compute-managed fields see slot-tail state; one-tick lag from sim-perspective bounded и deterministic

**Dependencies**: Item 35 (Phase.Compute integration); Item 36 (slot read API); Item 43 (async compute queue).

**К-L invariant alignment**: К-L16 + К-L7.1.

#### Item 34 — Pipeline drain/refill protocols

**Purpose**: Orderly pipeline drain at save/pause; orderly pipeline refill at load/resume.

**Implementation surface**:
- Save protocol (per Q1.5): snapshot display tick state (CurrentSimTick - D); pipeline drain не required (display tick state captures coherent world)
- Pause protocol (per Q1.6): natural convergence — sim thread completes current tick, no new dispatch, pipeline depth absorbs already-dispatched work
- Load protocol: orderly refill — sim thread starts at saved tick, refills pipeline incrementally; display unblocks once D slots populated
- Resume protocol: refill from pause point; sim thread resumes от last saved sim_tick

**Dependencies**: Item 33 (pipeline slot mechanism); Persistence integration.

**К-L invariant alignment**: К-L16.

#### Item 35 — Phase.Compute scheduler integration

**Purpose**: VkQueueSubmit batching (single submit per tick для all compute dispatches) within scheduler tick lifecycle.

**Implementation surface**:
- New scheduler phase: `Phase.Compute` — runs after Phase.Update (sim writes), before Phase.Display (display reads slot tail)
- VkQueueSubmit batching: all compute dispatches from active dispatch systems coalesced into single submit per tick (predicted ~50-100μs savings at ~10 active dispatch systems per Prediction 12)
- К10 scheduler coordinates с background tier (Item 30): background tier compute dispatches may coalesce dispatches within К10 tick (Q-N-55 open)
- Native scheduler dispatches к dedicated async compute queue (per К-L19 Item 43)

**Dependencies**: Item 33 (pipeline slot for fence binding); Item 43 (async compute queue mandate); Item 30 (background tier coordination).

**К-L invariant alignment**: К-L16 + К-L19.

#### Item 36 — Pipeline slot read API (S8-Q2 Pattern C)

**Purpose**: Sim-thread reads from slot tail (`SimTick - 1` reads dispatched-at-`(SimTick - 1) - 1` results); display thread reads from slot tail (`SimTick - D`).

**Implementation surface**:
- C ABI: `df_pipeline_read_slot_tail(slot_offset, out_field_snapshot)` — slot_offset is `0` (current) к `-D` (display tail)
- К-L7 atomic-from-observer preserved within pipeline slot boundary; cross-slot reads see different snapshots (К-L7.1 sub-invariant)
- Sim-thread one-tick lag from sim-perspective bounded и deterministic
- Slot transition state machine: Dispatched → FenceCompleted → ReadableAsTail (each transition fires StateChangeWake per Item 37)

**Dependencies**: Item 33 (pipeline slot mechanism); Item 17 (filter primitive for slot transition wake).

**К-L invariant alignment**: К-L7.1 + К-L16.

#### Item 37 — Filter primitive integration с pipeline slot transitions

**Purpose**: StateChangeWake fires when fence completes на slot tail, providing wake-driven downstream read systems.

**Implementation surface**:
- Slot transition `FenceCompleted → ReadableAsTail` triggers per-component-type filter check (Item 17 hybrid filter)
- Downstream read systems subscribed via `[WakeOnState<RawTileField>(SlotTransition)]` (or similar slot transition wake type)
- Atomic slot transition + filter check (S2 batch-commit semantics): no observer sees torn slot state

**Dependencies**: Item 17 (filter primitive); Item 33 (slot transitions); Item 36 (slot read API).

**К-L invariant alignment**: К-L7 + К-L7.1 + К-L13 + К-L16.

### 3.11 — Display composition multi-layer (items 38-40, S8-Q3 lock)

#### Item 38 — Display composition framework

**Purpose**: Layer registry + layer latency contracts per К-L17 + composition order (sim state first, overlays composited on top).

**Layer taxonomy** (per К-L17):

| Layer | Source | Latency | Mutating? |
|---|---|---|---|
| Sim state | Pipeline slot tail | D × tick_period | Yes |
| Intent overlay | Current input state | ≤16ms (60 FPS) | No |
| Combat feedback | Fast tier consumers | ≤1ms + ≤16ms | No |
| Static | Loaded assets | N/A | No |

**Implementation surface**:
- Layer registry tracks per-layer latency contracts + composition order
- Composition order: sim state layers rendered first, intent and combat overlays composited on top
- К-L17 invariant: display-composition latency ≤1 display frame ≈16ms at 60 FPS для intent overlays
- К-L15 (fast tier) + К-L16 (sim-to-display) preserved as independent invariants

**Dependencies**: Item 36 (pipeline slot read); Item 39 (intent overlay); Item 40 (combat feedback).

**К-L invariant alignment**: К-L17.

#### Item 39 — Intent overlay layer infrastructure

**Purpose**: Current input state surface + sub-pipeline-latency rendering path (≤1 display frame).

**Implementation surface**:
- Current input state surface accessible at display tick time (not sim-mutating)
- Sub-pipeline-latency rendering: intent overlay reads directly from input state, не from pipeline slot
- Layer registration capabilities: `kernel.layer.intent:{FQN}` per mod (S8-Q3 + S3-Q5 pattern)
- Use cases: cursor / hover / preview / drag-and-drop / construction-placement preview

**Dependencies**: Item 38 (composition framework); MOD_OS_ARCHITECTURE capability extension.

**К-L invariant alignment**: К-L17.

#### Item 40 — Combat feedback layer infrastructure

**Purpose**: Fast tier event consumers rendering directly (К-L15 latency + К-L17 composition latency).

**Implementation surface**:
- Fast tier event consumers (e.g., CombatHit events) render directly к combat feedback layer (≤1ms К-L15 + ≤16ms К-L17 ≈ ≤17ms event-to-visible per Prediction 15)
- Layer registration capabilities: `kernel.layer.combat_feedback:{FQN}` per mod (S8-Q3 + S3-Q5 pattern)
- Use cases: damage numbers, hit sparks, weapon glints, immediate UI flash on event

**Dependencies**: Item 38 (composition framework); Item 26 (fast tier bus subscribers); MOD_OS_ARCHITECTURE capability extension.

**К-L invariant alignment**: К-L15 + К-L17.

### 3.12 — Mod lifecycle quiescent state (items 41-42, S8-Q4 lock)

#### Item 41 — К-L18 quiescent state enforcement

**Purpose**: К10 verifies sim paused before mod unload primitive accepts call; returns error code if violated.

**Implementation surface**:
- `df_scheduler_unload_mod_native_state` (Item 32) precondition check: `sim_state == Paused && pipeline_slots_quiescent()` 
- Returns error code if violated (does not perform partial teardown)
- Pipeline slot quiescence verified via fence completion check on all D slots
- Mod load primitive (mirror): same quiescent state precondition before accepting mod registration

**Dependencies**: Item 32 (unload primitive); Item 33 (pipeline slot state); Item 42 (UI integration).

**К-L invariant alignment**: К-L18.

#### Item 42 — Settings menu / mod management UI integration с К10

**Purpose**: UI workflow enforces К-L18 pause precondition before triggering mod load/unload.

**Implementation surface**:
- Mod management UI (settings menu) enforces simulation pause precondition: user triggers «Disable Mod» → UI pauses sim → unload flow proceeds
- Hot reload tooling (development workflow) similarly pauses sim before triggering reload
- UI surface: pause indicator + «mod management requires pause» tooltip / hint
- V substrate scope (V brief amendment, не К10 items): `df_vulkan_unload_mod_resources(mod_id) → VulkanModUnloadResult` C ABI primitive — V layer releases resources after К10 unload (called as Step 3.6 in `ModIntegrationPipeline.UnloadMod` chain)

**Dependencies**: Item 41 (quiescent state enforcement); MOD_OS_ARCHITECTURE.md §9.5 mod unload chain amendment; VULKAN_SUBSTRATE.md V resource cleanup primitive amendment.

**К-L invariant alignment**: К-L18.

### 3.13 — Hardware tier commitment (items 43-44, S8-Q5 lock)

#### Item 43 — Async compute queue mandate

**Purpose**: К10 dispatches к dedicated async compute queue per К-L19. Graphics queue handles display rendering. Copy/transfer queue handles asset transfers.

**Implementation surface**:
- Native scheduler Phase.Compute (Item 35) submits all compute dispatches к dedicated async compute queue family
- Queue family selection at V substrate initialization: async compute queue identified during physical device enumeration
- Async compute queue + pipeline depth (К-L16) combined achieve full GPU parallelism (Prediction K11-4): pipeline depth D=2 compute throughput = ~2x single-queue baseline когда 4+ active dispatch systems
- К-L19 mandate: failure to detect async compute queue is fail-fast condition

**Dependencies**: Item 44 (hardware capability check); Item 35 (Phase.Compute integration); VULKAN_SUBSTRATE async compute queue integration.

**К-L invariant alignment**: К-L19.

#### Item 44 — Hardware capability check at startup

**Purpose**: V layer verifies async compute queue available; fails fast otherwise с К-L19 diagnostic message.

**Implementation surface**:
- Startup capability check via `vkGetPhysicalDeviceQueueFamilyProperties` (Vulkan 1.3)
- Detected target hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20xx и newer) / AMD RDNA 1+ (RX 5000 и newer) / Intel Arc Alchemist+
- Fail-fast diagnostic message clearly states К-L19 requirement when async compute queue absent
- Hardware tier published в README.md hardware requirements section

**Dependencies**: VULKAN_SUBSTRATE async compute queue feature detection; README.md hardware requirements amendment.

**К-L invariant alignment**: К-L19.

### 3.14 — TLA+ verification CI (items 45-46, S-TLA lock)

#### Item 45 — Safety verification CI gate

**Purpose**: TLC model checker runs as test, all 12 invariants safety-checked. Target runtime ~1-2 hours.

**Implementation surface**:
- CI test script invokes TLC against all 12 invariant specifications (К-L3, К-L4, К-L5, К-L7, К-L7.1, К-L8, К-L11, К-L12, К-L13, К-L15, К-L16, К-L18)
- Bounded state space configured per invariant (representative test space)
- Test runs as part of full test suite at К10 closure gate
- Target runtime ~1-2 hours total; runs nightly or per-commit к scheduler / bus / pipeline-touching code

**Dependencies**: Item 18 (TLA+ specifications); CI infrastructure.

**К-L invariant alignment**: К-L14 (formal verification of architectural invariants).

#### Item 46 — Liveness verification targeted

**Purpose**: Deeper models для К-L7.1 (fence completion), К-L12 (scheduler progress), К-L16 (pipeline drain).

**Implementation surface**:
- Per-invariant liveness verification model (deeper state space)
- К-L7.1 fence completion: GPU compute fence eventually completes within pipeline window
- К-L12 scheduler progress: scheduler eventually dispatches all runnable systems
- К-L16 pipeline drain: pipeline drains within bounded steps on save/pause
- Targeted scope per S-TLA Option (c): independent verification when possible, combined models only when cross-invariant interaction matters (R-K10-9 state space explosion mitigation)
- Authoring effort ~1-2 months post-К10 implementation

**Dependencies**: Item 18 (TLA+ specifications); Item 45 (safety CI gate as prerequisite).

**К-L invariant alignment**: К-L14.

---

## Part 4 — К-series completion logic

### 4.1 K-series arc retrospective

К-series total scope (К0 through К10 + К-closure):

| Milestone | Title | Status | К-L invariants established |
|---|---|---|---|
| K0 | Cherry-pick + cleanup | CLOSED (pre-A') | — |
| K1 | Batching primitive | CLOSED | К-L7 |
| K2 | Type-id registry + bridge tests | CLOSED | К-L4 |
| K3 | Native bootstrap graph + thread pool | CLOSED | К-L5, К-L6 (now superseded) |
| K4 | Component struct refactor (Path α) | CLOSED | К-L3 |
| K5 | Span<T> protocol + write batching | CLOSED | К-L7, К-L8 |
| K6 | Second-graph rebuild on mod change | CLOSED | (operational primitive) |
| K7 | Performance measurement (tick-loop) | CLOSED | К-L10 |
| K8.0 | Solution A architectural commitment | CLOSED | К-L11 |
| K8.1 | Native reference handling primitives | CLOSED | (K-L11 prerequisite) |
| K8.2 v2 | К-L3 selective per-component closure | CLOSED | (K-L3 application) |
| K-L3.1 | Bridge formalization (Path α + Path β) | CLOSED 2026-05-10 | К-L3 amended |
| K9 | Field storage abstraction | CLOSED 2026-05-11 | (V substrate prerequisite) |
| K8.3+K8.4 (combined A'.5) | Storage cutover | CLOSED 2026-05-14 | К-L11 realized |
| K8.5 | Mod ecosystem migration prep | PENDING A'.6/A'.7 | (mod ecosystem guide) |
| **К10** | **Native kernel scheduler** | **AUTHORED (this doc)** | **К-L12, К-L13, К-L14 + К-L6 supersession** |
| K-closure report | К-series formal closure | PENDING A'.8 | (closure record) |

### 4.2 Why К10 properly closes К-series

К-series stated purpose (per KERNEL_ARCHITECTURE.md): «ECS kernel migrates от managed C# к pure C++ via P/Invoke».

К0-К9 achieved:
- Data plane native (NativeWorld, fields, primitives)
- Path α (struct components) + Path β (managed bridge) per К-L3.1
- C ABI bridge layer (DualFrontier.Core.Interop)
- Test infrastructure parallel (selftest native + bridge tests managed)

К10 achieves:
- Control plane native (scheduler, graph, runqueue, dispatch)
- On-demand activation (К-L13)
- Full OS-faithful semantics (priority, quotas, preemption, shared memory, observability)
- Cross-layer ABI bridge (batched callback for managed mod systems)
- К-L6 (last managed-in-kernel invariant) superseded

**Post К10 + К-closure**: К-series is **complete** as full native OS-faithful kernel. Both data plane AND control plane native. К-L9 «Vanilla = mods» preserved (mods register through same IModApi). К-L11 «NativeWorld single source of truth» extends к scheduler authority.

К11+ Core systems migration to native code (Item 14) is **optional optimization**, не К-series closure requirement. Batched callback ABI (Item 15) absorbs transitional state. К-series can formally close at К10 with managed Core systems via callback; subsequent migration к native Core systems is post-К-series performance work.

### 4.3 Crystalka «закончить серию K» interpretation

Crystalka 2026-05-16: «можем закончить серию миграций K».

Two valid interpretations were on the table:
- **(a)** К10 extends K-series — К-series doesn't end at К8.5
- **(b)** К-series closes at К8.5 + К10 is post-K-series work

This document commits к **interpretation (a)** with К10 + К-closure as К-series capstone. Rationale:
- К10 finishes what К0-К9 started: full native kernel
- К-L6 supersession is intrinsically К-series concern (modifies К-Lxx series)
- Phase A' sequencing supports: А'.7 К8.5 → А'.8 К-closure presupposes К10 between (К-closure enumerates К-L1..К-L14, which requires К10 having shipped к define К-L12/13/14)
- Crystalka direction «как настоящий OS» logically requires К10 for proper OS metaphor (currently К-L6 prevents OS-faithful kernel scheduling)

Phase A' sequencing amendment:
- А'.6 = К8.5 (current pending) — preserved
- А'.7 = К10 (NEW slot — was Roslyn analyzer per closure-note intent)
- А'.8 = К-closure report — preserved
- А'.9 = Roslyn analyzer — shifted from А'.7 closure-note к А'.9 (or А'.10 if А'.8 K-closure is substantial)

This **resolves Q-K-1 retroactive lock** (from composite namespace deliberation, deferred to «K8.5 brief authoring approaches»): К8.5 still happens at А'.6; К10 inserts as А'.7; А'.8 К-closure subsumes the closure note's A'.8 mention. Q-K-1 reconciliation note in PHASE_A_PRIME_SEQUENCING.md updates accordingly upon К10 brief authoring.

### 4.4 К10 deliverables (Extension per S-TLA lock)

К10 deliverables at К10 closure gate:

**Architectural artifacts**:
- К10 native scheduler implementation (Items 1-25 + 26-46)
- TLA+ specifications для 12 К-L invariants (К-L3, К-L4, К-L5, К-L7, К-L7.1, К-L8, К-L11, К-L12, К-L13, К-L15, К-L16, К-L18)
- TLA+ safety verification CI integration script (Item 45)
- TLA+ liveness verification scripts для К-L7.1, К-L12, К-L16 (Item 46)

**Verification artifacts**:
- Safety verification CI test results (all 12 invariants passing)
- Liveness verification preliminary results (targeted invariants К-L7.1, К-L12, К-L16)
- К10 closure benchmarks per §5.1.A 18 predictions

**Documentation artifacts**:
- К-closure report contributions (А'.8 deferred — invariant enumeration source material)
- Cross-document amendment landings (9 documents per Part 7)
- К10 execution brief (forthcoming, separate session) translating К10 spec к Claude Code execution material

---

## Part 5 — Performance derivation argument (К-L14 foundational claim)

### 5.1 Falsifiable performance claim — split per S4 lock

К-L14 commits to «performance derives from clean complex architecture». This is **falsifiable** — measurable predictions follow. Per S4 lock (2026-05-17), predictions split between §5.1.A (К10 closure gate predictions — measurable at К10 closure) and §5.1.B (К11+ deferred predictions — measurable after Core systems migration к native, post-К-series).

### §5.1.A — К10 architecture realization predictions (К10 closure gate)

К10 closure benchmarks structured per §5.1.A predictions. Each prediction measurable в К10 closure:

1. **Prediction 1** — On-demand activation 30-70% CPU reduction в sparse worlds. Mechanism: К10 wake registry + dynamic graph delivers; measurable в К10 closure benchmarks.

2. **Prediction 2** — Dynamic graph 10-30% parallelism improvement. Mechanism: К10 native scheduler delivers.

3. **Prediction 3** — Priority dispatch 50-90% p99 RT latency reduction. Mechanism: К10 priority queues delivers.

4. **Prediction 4 REFRAMED** — К10 primitives measurable improvement в targeted scenarios + baseline parity overall. Honest framing per S4 lock: parity overall + specific scenario gains. (Replaces earlier «doesn't regress vs current managed scheduler» framing which was binary-pass/fail; new framing acknowledges that К10 architectural primitives confer gains only in scenarios where their architectural shape applies, and К11+ Core migration delivers full performance realization.)

5. **Prediction 5** — Composition compounds within К10 features. Mechanism: each clean architectural addition (on-demand + dynamic + priority + quotas + observability + pipeline) contributes к performance independently; composition >sum-of-individual-benefits.

6. **Prediction 6** — Native bus eliminates per-publish boundary crossing для kernel-internal events. Predicted: 80-95% reduction в bus-related P/Invoke volume. Mechanism: К-L15 native bus implementation (Item 26). Measurement: P/Invoke crossing counter, K10-current vs K8-baseline.

7. **Prediction 7** — Fast event contract provides bounded delivery latency. Predicted: p99 latency для fast events ≤ 1ms. Mechanism: К-L15 fast tier synchronous bypass. Measurement: fast tier subscriber latency histogram.

8. **Prediction 8** — Background tier enables off-screen simulation без impact на foreground tick. Predicted: 5-20% CPU budget utilization для background work; foreground unchanged when background saturated. Mechanism: К-L15 background tier idle-slot dispatch (Item 30).

9. **Prediction 9** — Coalesce'инг reduces background processing cost. Predicted: typical coalesce ratio 2-10x. Mechanism: per-type coalesce function (Item 30).

10. **Prediction 10** — Background event persistence preserves game-world state across mod unload без re-simulation. Predicted: <5% save-game size overhead для typical background queue (~1000 events × ~100 bytes = ~100KB). Mechanism: S3-Q3 save-integrated storage (Item 31).

11. **Prediction 11** — Pipeline depth eliminates simulation thread GPU fence wait. Predicted: zero CPU-side fence waits per tick (excluding save/load drain). Mechanism: К-L16 simulation tick pipeline depth (Item 33).

12. **Prediction 12** — Phase.Compute submit batching reduces VkQueueSubmit overhead. Predicted: ~5-10μs × N submits → single submit per tick = ~50-100μs savings at ~10 active dispatch systems. Mechanism: Item 35 Phase.Compute integration.

13. **Prediction 13** — Pipeline slot tail read eliminates per-read fence query overhead. Predicted: ~30-50% reduction в FieldHandle.ReadCell latency vs Pattern B (fence-async per-read). Mechanism: S8-Q2 Pattern C + К-L7.1 (Items 33 + 36).

14. **Prediction 14** — К-L17 layer composition eliminates perceived latency для player intent surfaces. Predicted: ≤16ms input-to-display latency для intent overlays. Mechanism: S8-Q3 multi-layer composition (Items 38 + 39).

15. **Prediction 15** — Combat feedback layer renders Fast tier event consequences at К-L15 latency. Predicted: ≤1ms К-L15 + ≤16ms display ≈ ≤17ms event-to-visible. Mechanism: Item 40 combat feedback layer infrastructure.

16. **Prediction 16** — Async compute queue capability detection functional на all target hardware. Predicted: 100% startup success на RDNA 1+/Turing+/Arc+ hardware. Mechanism: К-L19 + Items 43-44.

17. **Prediction 17** — Pipeline drain on save coordinated с background queue serialization. Predicted: <100ms total save initiation latency. Mechanism: Item 34 drain/refill protocols + Item 31 background queue serialization.

18. **Prediction 18** — Safety verification CI gate functional. Predicted: ~1-2 hour runtime для all 12 invariants safety check. Mechanism: S-TLA Item 45 safety verification CI gate.

### §5.1.B — К11+ performance realization predictions (deferred к К11+ post-К-series)

Per S4 lock: К-series closes на К10 architecture; К11+ measurement deferred. §5.1.B authoring time = К11+ brief authoring time. Predictions stated here for chronological record:

1. **Prediction K11-1** — Core systems native execution eliminates per-phase boundary crossing overhead. Predicted: 100% removal batched callback ABI cost для Core systems (~9μs/sec savings minimum). Mechanism: К11+ Core systems migration к native code (Item 14 deferred scope).

2. **Prediction K11-2** — Native Core systems benefit from SIMD/auto-vectorization, no GC pauses. Predicted: 2-10x performance improvement для compute-heavy systems (MovementSystem, NeedsSystem, AISystem candidates).

3. **Prediction K11-3** — К-L14 measurably confirmed (or falsified): if К11+ shows no improvement над К10-with-managed-Core baseline, К-L14 *causality* claim weakened.

4. **Prediction K11-4** — Async compute queue + К-L16 pipeline depth combined achieve full GPU parallelism. Predicted: pipeline depth D=2 compute throughput = ~2x single-queue baseline когда 4+ active dispatch systems.

5. **Prediction K11-5** — Liveness verification catches deadlock/starvation bugs invisible к testing. Predicted: 0-3 verification-detected bugs surfaced per К-L invariant on first run.

### 5.2 What disproves К-L14

К-L14 is falsifiable. Disproof scenarios:

1. **К10 full implementation produces no performance improvement (or regression) vs К-L6 managed scheduler** in any workload — invalidates «performance from architecture» claim
2. **Architectural items contribute negative-sum к performance** (e.g., observability hooks cost more than they save through profiling-informed optimization) — invalidates «complexity buys performance»
3. **«Simpler» alternative scheduler matches or exceeds К10 performance** — invalidates «clean complex architecture causes performance»

К-L14 is **research framework hypothesis**, not assumption. К10 + measurements either confirm or falsify. К-closure report (А'.8) documents results — both confirming and disconfirming evidence committed to record.

### 5.3 Causality vs correlation

«Performance derives from architecture» is causal claim:
- **Architectural cleanness** enables specific optimizations (dynamic dispatch, priority, quotas)
- **Specific optimizations** produce measurable performance gains
- **Performance gains** are not coincidental к architectural cleanness — they are derived

Without architectural extensions (single static graph, no on-demand, no priority), the optimizations are **inexpressible** in code. К-L6 managed scheduler **cannot have** these features without becoming К10 native scheduler. Therefore performance ceiling tied to architectural choice; architectural choice causes performance ceiling.

Linux kernel evidence: 30+ years of architectural additions, each principled, each producing performance gain (CFS displacing O(1) scheduler, NUMA awareness, cgroups, BPF). Net architectural complexity AND net performance monotonically increasing. Causal link inductively supported.

---

## Part 6 — Q-N deliberation surface (open architectural questions)

К10 deliberation expected к surface ~20-30 Q-questions. Pre-authored Q-N list below; expansion / refinement during deliberation sessions.

### 6.1 Class B (architectural)

**Q-N-1** — К10 scope shape (atomic cutover vs staged):
- (a) Scheduler only, Core systems remain managed via batched callback (this doc's default)
- (b) Scheduler + first Core system migration (proof-of-concept)
- (c) Scheduler + all Core systems к native (К10 absorbs К11-К14)
- Locked: deferred (this doc assumes (a) based on batching cost analysis; revisit if deliberation surfaces preference)

**Q-N-2** — Mod scheduler authority (per-mod vs shared):
- (a) Per-mod sub-scheduler (this doc Item 21 default)
- (b) Shared managed scheduler descoped к mods-only
- Locked: deferred к К10 deliberation

**Q-N-3** — Bus implementation authority (managed vs native):
- (a) Bus stays managed authority (status quo)
- (b) Bus migrates к native
- (c) Bus splits — kernel-internal events native, managed-published bridge
- Locked: deferred (this doc assumes (a) initially; (c) potential post-К10 work)

**Q-N-4** — К-L6 supersession scope:
- К-L6 SUPERSEDED + К-L12/13/14 added (this doc default)
- Alternative: К-L6 amended in place vs added as К-L12+ — same outcome, different formality
- Locked: SUPERSEDED + new К-Lxx added (preserves К-L6 as historical record)

**Q-N-5** — Hot reload preservation:
- Rebuild semantics extend к native scheduler (suspend / replace / resume per Items 20 + 22)
- Cross-boundary atomic rebuild non-trivial (graph state + wake registry + thread pool state all need atomic transition)
- Locked: native rebuild via Item 20 intrinsics; deliberation may refine semantics

### 6.2 Class A (formal)

**Q-N-6** — K-series milestone identifier:
- К10 (this doc default — fits К-series numbering)
- Alternative: К8.6 (sub-К8 namespace — but К8.x is post-K-L3.1 bridge work, scheduler is broader)
- Alternative: new namespace (N-series, S-series) — but К-series is appropriate per Crystalka «закончить серию K»
- Locked: К10

**Q-N-7** — Phase A' or Phase B placement:
- Phase A' (this doc default — extends Phase A' sequencing)
- Phase B = M-series only; К10 belongs Phase A'
- Locked: Phase A'.7

### 6.3 Class C (organizational)

**Q-N-8** — Brief sequencing (single milestone or split):
- (a) Single К10 brief, atomic cutover (similar к К8.3+К8.4 v2 precedent)
- (b) Split into К10.1 / К10.2 / ... sub-milestones
- Locked: deferred — likely (a) per atomic-cutover precedent

**Q-N-9** — Test strategy (native test framework + managed fixture):
- catch2 vs gtest decision
- ManagedTestScheduler fixture для backward-compatible managed tests
- Locked: deferred к brief authoring time

**Q-N-10** — К-L invariant text final wording:
- К-L12 / К-L13 / К-L14 text in this doc is **draft**
- Final wording resolved at brief authoring + KERNEL_ARCHITECTURE.md v2.0 amendment
- Locked: deferred к KERNEL_ARCHITECTURE.md amendment

### 6.4 Implementation-specific Q-N

**Q-N-11** — Batching granularity (per-phase vs per-tick):
- Per-phase per-origin (this doc default — Item 15)
- Per-tick (whole tick = one boundary crossing)
- Per-system (defeats batching purpose)
- Locked: per-phase per-origin

**Q-N-12** — Callback signature design:
- ManagedSystemBatch struct (this doc default — Item 15)
- Per-system specialized callbacks
- Locked: ManagedSystemBatch — uniform surface

**Q-N-13** — Managed system identity across boundary:
- Integer ID + managed-side dictionary (this doc default)
- GCHandle per system
- String name with lookup
- Locked: integer ID

**Q-N-14** — Per-system context responsibility:
- Native scheduler doesn't know contexts (managed concern, this doc default)
- Native owns contexts (managed object reference back — square one)
- Refactored context concept across boundary
- Locked: managed adapter handles contexts

**Q-N-15** — Wake-up mechanism completeness:
- All 5 wake types in К10 (this doc default per К-L13)
- Subset, defer some к К-extensions
- Locked: all 5 (К-L14 default-inclusion)

**Q-N-16** — Dependency graph + runqueue interaction:
- Static phase with runnable subset filter
- Dynamic phase composition per tick (this doc default — Item 5)
- Locked: dynamic per-tick

**Q-N-17** — State change detection:
- Polling (defeats purpose)
- Write-through hook (this doc default — Item 17)
- Dirty bit
- Locked: write-through hook

**Q-N-18** — Mod system on-demand integration:
- Cross-mod wake-up routing through bus / state-change infrastructure
- Capability declaration extension (mod manifest declares wake sources)
- Locked: deferred к К8.5

**Q-N-19** — TickScheduler fate:
- Retained as-is, on-demand added on top
- Subsumed into WakeRegistry; [TickRate] becomes [WakeOn(Timer)] sugar (this doc default)
- Deleted
- Locked: subsumed into WakeRegistry with [TickRate] preserved as TimerWake sugar

**Q-N-20** — Performance measurement methodology:
- Benchmark scenarios per §5.1 predictions
- Locked: methodology specified at brief authoring

**Q-N-21** — Native test framework choice:
- catch2 (header-only, modern)
- gtest (mature, more boilerplate)
- Locked: deferred к brief authoring

**Q-N-22** — Formal verification scope:
- TLA+ specification at К10 closure (this doc default — Item 18)
- Defer formal verification к К-extensions
- Locked: TLA+ at К10 (К-L14 default-inclusion)

**Q-N-23** — Observability hook overhead acceptable:
- Default sampling rate decision (full / 1-in-N / off)
- Lock-free trace buffer implementation complexity
- Locked: deferred к brief authoring

**Q-N-24** — Priority class default mapping:
- Existing systems default к Normal priority
- Critical systems (input, V substrate dispatch) opt-in to RT / High
- Audit existing system attributes for natural priority class
- Locked: deferred к brief authoring

**Q-N-25** — Resource quota default budgets:
- Per-system [CpuQuota] not declared = no quota enforcement (legacy behavior)
- Or default quota per priority class (Normal = 500µs per tick, etc.)
- Locked: deferred к brief authoring

**Q-N-26** — Preemption point declaration:
- Cooperative preemption opt-in via [Preempt] attribute (this doc default Item 8)
- Implicit preemption points at common operations (component access, bus publish)
- Locked: explicit opt-in

**Q-N-27** — Shared memory region typing:
- Compile-time typed via generic (`ShmRegion<T>`)
- Runtime typed via descriptor + cast
- Locked: deferred к brief authoring

**Q-N-28** — Scheduler intrinsics access control:
- Public API for diagnostics; restricted API for self-modification (suspend / resume / panic)
- Locked: deferred к brief authoring

**Q-N-29** — Native AOT compatibility:
- Managed adapter as regular .NET assembly (full reflection — this doc default)
- Managed adapter as Native AOT DLL (limited reflection, AOT compatibility)
- Hybrid (graph build assembly + AOT runtime adapter)
- Locked: regular .NET assembly initially; AOT optional future

**Q-N-30** — .NET version target:
- .NET 10 (production-ready, K10 execution can start immediately)
- .NET 11 (late 2026, runtime-async matures, longer LTS)
- Locked: deferred к brief authoring (likely .NET 10 if К10 starts soon; .NET 11 if К10 deliberation extends)

### 6.5 Q-N surface extensions from S deliberation arc (Q-N-31 through Q-N-56)

Q-N-22 status update (resolved by S-TLA): TLA+ specifications в К10 deliverable scope; verification gated separately (safety CI gate К10, liveness targeted К10/К11+).

#### From S1 lock (Q-N-31 through Q-N-37)

- **Q-N-31** — Mod fast event subscribe authority (RESOLVED: S3-Q5 capability shape per-FQN per-tier)
- **Q-N-32** — Event payload size constraints per tier (fast ≤ 64 bytes cache-line? normal unlimited? background coalesce-friendly?)
- **Q-N-33** — Cross-tier event ordering guarantees (formal spec для TLA+ Item 18 — does fast preempt normal mid-phase? Background defers behind both?)
- **Q-N-34** — Background coalesce'инг strategy (per-type coalesce function declared by event author? framework-provided patterns? mod-declared?)
- **Q-N-35** — Idle-slot detection mechanism (how scheduler measures «idle CPU budget»)
- **Q-N-36** — Background queue saturation handling (drop oldest? backpressure publisher? expand budget?)
- **Q-N-37** — Capability granularity (RESOLVED: S3-Q5 per-FQN per-tier)

#### From S2 lock (Q-N-38 through Q-N-42)

- **Q-N-38** — Filter unsubscribe timing (atomic Level 1 bit clear when last subscriber for type unsubscribes; race condition class)
- **Q-N-39** — Filter rebuild on mod load/unload (atomic filter reconstruction; related к Item 22 hot-patching; S3 partially addresses)
- **Q-N-40** — К-L14 wording refinement к include effort-cost clause (deferred к К-closure report А'.8)
- **Q-N-41** — Batch commit hook ordering vs scheduler dispatch boundary (synchronous в writer thread, or enqueue для scheduler dispatch?)
- **Q-N-42** — Filter Level 2 entity removal lifecycle (entity destroyed but had entity-specific subscriber → cleanup path)

#### From S3 lock (Q-N-43 through Q-N-49)

- **Q-N-43** — Background queue serialization format (blittable struct array + type_id table? Versioning concerns)
- **Q-N-44** — Save-game compatibility versioning (background event types changing between game versions — schema migration)
- **Q-N-45** — Background queue size cap (memory boundary; OOM prevention)
- **Q-N-46** — Mid-batch unload race condition resolution (RESOLVED: К-L18 quiescent state precondition eliminates concern)
- **Q-N-47** — Capability per FQN scaling (если 1000+ events, registry size concern)
- **Q-N-48** — WakeRegistry teardown ordering (5 wake types — какой порядок очистки внутри T6?)
- **Q-N-49** — ShmWriter destruction атомарность (mod-sole-writer destruction semantics)

#### From S8 sub-deliberation (Q-N-50 through Q-N-56)

- **Q-N-50** — Compute dispatch phase в К10 tick lifecycle (RESOLVED: S8-Q1 Phase.Compute)
- **Q-N-51** — RT subscribers reading V fields one-tick lag contract (RESOLVED: S8-Q2 Pattern C + К-L7.1)
- **Q-N-52** — V Vulkan resource cleanup в mod unload T-sequence (RESOLVED: S8-Q4 К-L18 + Pattern (b) delegate)
- **Q-N-53** — Capability granularity для V compute dispatch (RESOLVED: S8-Q4 per-pipeline-FQN per-tier)
- **Q-N-54** — Async compute queue requirement (RESOLVED: S8-Q5 К-L19 mandate)
- **Q-N-55** — Background tier compute dispatch separate VkQueue priority (deferred к К-L19 implementation: К10 dispatches к async compute queue; background tier coalesces dispatches within К10)
- **Q-N-56** — Save/load sequence К10 background drain vs V GPU pause ordering (RESOLVED: К-L18 quiescent state; save pauses sim → all in-flight drains → save coordinates background queue + pipeline slot serialization)

### 6.6 Open Q-N count

After all S resolutions: ~42 open Q-N candidates. Most are implementation-specific и address during К10 execution brief authoring or implementation.

---

## Part 7 — Cross-document amendment list

Upon К10 deliberation closure (achieved 2026-05-17), the following documents require amendment. Status indicates whether amendment landed (✅) at this К10 amendment landing, or pending (⏳) in separate brief / session.

### 7.1 KERNEL_ARCHITECTURE.md v1.6 → v2.0 ⏳

- К-L invariant table fully updated (20 invariants final)
- К-L6 SUPERSEDED note added
- К-L7.1 sub-invariant note (mirrors К-L3.1 sub pattern)
- К-L12-К-L19 sequential additions
- Cross-reference к К10 specification (this document)

### 7.2 PHASE_A_PRIME_SEQUENCING.md (Tier 2 Live) ⏳

- §2 sequencing wording cleanup (S5)
- «А'.10 if А'.8 substantial» wording removed
- К9.5 not inserted (cancellation note)
- Q-K-1 retroactive lock resolved через К10 specification (К10 inserted at А'.7 per session log)

### 7.3 MIGRATION_PLAN_KERNEL_TO_VANILLA.md ⏳

- §0.1 sequence diagram extends с К10 between К-series and М-series
- Section listing К-series milestones extends к include К10
- К-series closure note clarifies «К10 + К-closure = К-series complete»

### 7.4 MOD_OS_ARCHITECTURE.md ⏳

- §3.2 capability section extended с tier-prefixed tokens (S3-Q5) + V resource capability tokens (S8-Q4) + К-L17 layer tokens (S8-Q3)
- §4 IModApi extended с К-L17 layer registration capabilities
- §9.5 mod unload chain — Steps 3.5 + 3.6 documented под К-L18 precondition
- §11 hot reload section — К-L18 compliance note (pause required during ALC swap)

### 7.5 VULKAN_SUBSTRATE.md ⏳

- §0 L1 GPU API decision extended с async compute requirement (К-L19)
- §0 L7 «Initial platform: Windows-only» implication note: matches К-L19 hardware tier
- §2 architecture extended с display composition (К-L17)
- §2.3 threading model extended с pipeline depth (К-L16) + async compute queue (К-L19) + sim-thread read pattern (К-L7.1)
- §3.4 native compute dispatch extended с `df_vulkan_unload_mod_resources` C ABI (S8-Q4) + К-L19 mandate
- §4 rendering use case extended с display composition section (К-L17)
- §5.5 Mode C navigation extended с visibility latency через К-L17 composition (no special-case mechanism)
- §7.2 determinism extended с pipeline drain semantics (К-L16): save snapshots display tick state, pipeline drain не required
- §7.3 async sync hazards reworded для pipeline slot tail read (К-L7.1)

### 7.6 DualFrontier.Persistence (project) ⏳

- Save/load integration для background queue (S3-Q3)
- Pipeline slot snapshot serialization (К-L16, save-integrated per S8-Q1.5)
- TLA+ specification для save/load atomicity (S-TLA Item 18)

### 7.7 KernelCapabilityRegistry.cs (source) ⏳

- Tier-prefixed token generation (S3-Q5): scanning logic extends к include tier attribute reading
- К-L17 layer capability tokens (S8-Q3): `kernel.layer.intent:{FQN}`, `kernel.layer.combat_feedback:{FQN}`
- V resource capability tokens (S8-Q4): `vulkan.compute.dispatch:{pipeline_FQN}`, `vulkan.field.register:{field_FQN}`

### 7.8 METHODOLOGY.md v1.7 → v1.8 ✅ (landed 2026-05-17)

- v1.7 → v1.8 version increment
- NEW Lessons #11, #20, #22 full formalization (S6 lock)
- NEW «Provisional Lessons» section с 9 candidates documented (S6 lock)
- TLA+ specification authoring methodology section (S-TLA)

### 7.9 README.md ⏳

- Hardware requirements section (К-L19)
- Target hardware tier: NVIDIA Turing+ / AMD RDNA 1+ / Intel Arc Alchemist+

### 7.10 К-closure report (А'.8) — pending creation

- К-series closure narrative
- К-L invariant series formalization (20 invariants)
- Provisional Lessons promotion (per S6 lock)
- TLA+ specification authoring status
- К-L14 evidence state framing (architecturally established, measurably pending)
- Roslyn analyzer rule specification (cascade input к А'.9)

### 7.11 New documents К10 produces

- `KERNEL_FULL_NATIVE_SCHEDULER.md` (this — promoted к Tier 1 LOCKED post-amendment) ✅
- `K10_DELIBERATION_STATE.md` (Project file, sister к this doc; not register-tracked) ✅
- `K10_EXECUTION_BRIEF.md` (forthcoming, separate session — execution brief for Claude Code) ⏳
- `SCHEDULER_TLA_PLUS.tla` (forthcoming, К10 deliverable — Item 18) ⏳
- `K_SERIES_CLOSURE_REPORT.md` (А'.8, after К10 implementation closure) ⏳

---

## Part 8 — Risk register

### 8.1 K10 implementation risks

**R-K10-1**: Reverse P/Invoke GC transition cost exceeds estimate
- Probability: Low
- Impact: К10 performance regression vs К-L6 managed scheduler
- Mitigation: Batching amortizes (Item 15); SuppressGCTransition forbidden (verified); .NET 10 transition cost benchmarked; predictions §5.1 falsifiable
- Recovery: Adjust batching granularity (Q-N-11 revisit) or accept overhead as cost of architectural cleanness

**R-K10-2**: Native scheduler thread synchronization bugs
- Probability: Medium
- Impact: Race conditions, deadlocks, data corruption in scheduler hot path
- Mitigation: TLA+ formal verification (Item 18); extensive testing (Item 24); selftest-based development; native side already proves ThreadPool patterns (К3)
- Recovery: Stop-the-world debugging via Item 20 intrinsics

**R-K10-3**: Mod hot reload across native + managed boundary fails
- Probability: Medium
- Impact: Mods cannot be loaded / unloaded cleanly post-К10
- Mitigation: Suspend-replace-resume pattern (Item 20); existing managed Rebuild semantics preserved within managed adapter; clear separation of native scheduler state vs mod-side state
- Recovery: Fallback к pause-and-restart for development; production polish in К-extensions

**R-K10-4**: Wake-up registry overhead dominates idle systems
- Probability: Low
- Impact: On-demand model net negative if registry maintenance > savings from skipped systems
- Mitigation: Lock-free registry implementation; O(1) wake dispatch where possible; benchmark §5.1 Prediction 1 falsifies
- Recovery: Adjust wake type implementations; possibly defer some wake types к К-extensions

**R-K10-5**: К-L14 falsified by measurements
- Probability: Low
- Impact: Project invariant (performance from architecture) requires re-examination
- Mitigation: К-L14 is **falsifiable hypothesis**; project framing accepts that hypothesis may be wrong; К-closure report records both confirming and disconfirming evidence
- Recovery: Examine which architectural items contributed positively vs negatively; refine К-L14 wording; possibly demote к К-L14 «architecture enables performance» (weaker claim)

**R-K10-6**: Background queue save compatibility versioning
- Probability: Medium
- Impact: Medium (save-game compatibility critical for player retention) — Background event types changing between game versions could break save-game compatibility (older save с event types removed в newer game version → load fails или silently drops events)
- Mitigation: Save versioning schema integrated с background queue serialization (S3-Q3); migration mechanism для type_id changes между versions. Defer detailed migration spec к К10 implementation.
- Recovery: Schema migration patch on detection; user-facing diagnostic if migration impossible

**R-K10-7**: Filter consistency races
- Probability: Low (но subtle)
- Impact: High (К-L7 invariant violation potential) — Filter primitive (S2 Q3b hybrid) Level 1 atomic + Level 2 sparse hash interactions could race under high subscribe/unsubscribe churn
- Mitigation: TLA+ specification (Item 18 extended per S-TLA) includes filter consistency invariants; safety CI gate verifies. Atomic subscribe/unsubscribe pattern documented в К10 implementation brief.
- Recovery: Race fix patch; TLA+ specification extended с failing case; safety CI gate enforces

**R-K10-8**: Hardware tier exclusion
- Probability: Medium (hardware tier evolution favorable; by Dual Frontier release, target hardware tier represents majority)
- Impact: Low (research framework, не mass-market product) — К-L19 hardware tier excludes pre-Turing NVIDIA, pre-RDNA AMD, pre-Arc Intel, и most integrated GPUs. Some potential players may have older hardware.
- Mitigation: Hardware requirements published в README.md (К-L19 cross-document amendment). Fail-fast diagnostic message clearly states requirement when async compute queue absent.
- Recovery: Accept exclusion as architectural choice (К-L19 commitment); document hardware requirement upfront

**R-K10-9**: TLA+ state space explosion
- Probability: Medium
- Impact: Medium (verification scope reduction may be required) — Liveness verification для К-L12 (scheduler progress) + К-L16 (pipeline drain) combined could explode state space
- Mitigation: Per S-TLA Option (c) targeted scope — verify invariants **independently** when possible. Each К-L gets own spec model. Combined models only when cross-invariant interaction matters (rare). К-extension milestone available if full liveness coverage requires more time.
- Recovery: Reduce liveness scope; accept safety-only verification for affected invariants

### 8.2 К10 deliberation risks

**R-K10-DEL-1**: Q-N deliberation extends substantially (15+ sessions)
- Probability: Medium-High
- Impact: К10 execution delayed
- Mitigation: Multi-session deliberation expected (К10 scope substantial); К10 deliberation framing document (this doc) accelerates by pre-authoring Q-N list; lessons from composite namespace deliberation (10 Q over multiple sessions) inform pacing
- Recovery: Accept timeline; «без костылей» commitment supports time investment

**R-K10-DEL-2**: Q-N decisions reveal architectural conflicts not anticipated
- Probability: Medium
- Impact: К10 scope changes mid-deliberation
- Mitigation: Q-N classification (Class B architectural first) surfaces architectural concerns early; deliberation discipline (one Q at a time, ratification before next) prevents cascading rework
- Recovery: К10 brief amendment; lessons captured

**R-K10-DEL-3**: Performance predictions §5.1 inaccurate, undermining К-L14
- Probability: Medium
- Impact: К-L14 falsified or weakened
- Mitigation: Predictions explicitly falsifiable; benchmarking methodology pre-specified; «research framework» framing accepts disconfirming evidence
- Recovery: К-L14 amendment к weaker form; К-closure report records evidence

### 8.3 Cross-cutting risks

**R-K10-X-1**: К10 + V substrate interlock complexity
- Probability: Medium
- Impact: К10 scheduler dispatching V substrate commands has surface area beyond К10 brief
- Mitigation: К10 brief explicitly excludes V substrate detailed integration; ratifies dispatch primitives (V substrate command submission can occur at cooperative preemption points Item 8); detailed integration in V0/V1/V2 briefs
- Recovery: Amendment cycle through К-extensions or V substrate briefs

**R-K10-X-2**: К8.5 ambiguity (was К8.5 absorbed into A'.5 or pending?)
- Probability: Low (resolved by this doc's interpretation)
- Impact: А'.6/А'.7 sequencing reshuffles
- Mitigation: This doc commits к К8.5 = А'.6 pending; К10 = А'.7 NEW; Q-K-1 reconciliation resolves through К10 brief authoring
- Recovery: PHASE_A_PRIME_SEQUENCING.md amendment within К10 brief scope

---

## Part 9 — Operational considerations

### 9.1 Build pipeline impact

К10 introduces:
- Native scheduler subsystem in `DualFrontier.Core.Native` C++ codebase
- Native test framework (catch2 / gtest) integration into CMake build
- TLA+ specification artifact (`docs/formal/SCHEDULER_TLA_PLUS.tla`) — outside build but tracked in REGISTER.yaml
- Managed adapter assembly (`DualFrontier.Application.SchedulerAdapter` or similar) for batched callback bridge

Build verification additions:
- `cmake --build` includes native scheduler compilation + native tests
- `dotnet build` includes managed adapter
- `dotnet test` includes integration tests (managed-side к native scheduler via batched callback ABI)
- TLA+ specification verified via TLC model checker (separate verification step, periodic, not per-commit)

### 9.2 Development workflow

Per-К10-commit verification:
- `cmake --build` clean
- Native tests passing (catch2 / gtest suites)
- `dotnet build` clean (0 warnings, 0 errors)
- `dotnet test` 620+ passing (current baseline + new К10 integration tests)
- `sync_register.ps1 --validate` exit 0
- TLA+ verification: periodic, not per-commit (model checking is multi-minute)

Hot-path debugging:
- Mixed-mode debugging (C++ + C#) required для К10 work — Visual Studio Pro+ or VS Code with mixed-mode extension
- Native scheduler trace output (Item 19) consulted alongside managed call stacks
- Diagnostic API (Item 4) запросы scheduler state at debugger breakpoints

### 9.3 Performance measurement infrastructure

К10 adds к existing benchmarking suite:
- `TickLoopBenchmark` variants:
  - **К10-on-demand-sparse**: sparse workload, on-demand model maximum benefit
  - **К10-on-demand-dense**: dense workload, on-demand parity with tick-driven
  - **К10-vs-K-L6-baseline**: К10-with-managed-Core-systems vs current К-L6 managed scheduler
  - **К10-ablation-suite**: К10 with subset of features (priority off / quotas off / shared memory off) vs full set
- Metrics:
  - p50 / p95 / p99 tick time
  - Per-phase parallelism (utilized cores / wall-clock)
  - GC pause count + duration (managed adapter side)
  - Total native scheduler overhead (trace timestamps via Item 19)
  - On-demand benefit (systems skipped / total systems registered)
- Report file: `docs/reports/PERFORMANCE_REPORT_K10.md`

---

## Part 10 — Glossary (OS terms applied к нашему context)

| OS term | Dual Frontier context |
|---|---|
| Kernel space | Native: NativeWorld + scheduler + Core systems (К11+ migrated) + V substrate |
| User space | Managed: mods (ALC-isolated) + IModApi capability surface |
| Process | Mod assembly (per-mod ALC, per-mod scheduler, capability declarations) |
| Process scheduler | Per-mod sub-scheduler within mod ALC (Item 21) |
| Kernel scheduler | Native scheduler in `DualFrontier.Core.Native` (К10 — this doc) |
| System call | C ABI function dispatched from managed к native (P/Invoke forward direction) |
| Kernel/user transition | Native ↔ managed boundary crossing (P/Invoke + reverse-P/Invoke batched callback) |
| Runqueue | Set of runnable systems on current tick (К-L13) |
| Blocked queue | Set of systems awaiting wake signal (К-L13) |
| Wake-up | System transition from blocked к runnable triggered by wake source (К-L13) |
| Quantum | Per-tick scheduling decision boundary |
| Priority class | Scheduling priority level (RealTime / High / Normal / Low / Background — Item 6) |
| Preemption | Forced suspension of executing system (RT class only — Item 8) |
| Quota | Per-system CPU time budget (Item 7) |
| Cooperative preemption | System voluntarily yields at marked points (default — Item 8) |
| IPC | Inter-system communication: buses (events) + shared memory regions (high-frequency data — Item 9) |
| Hot patching | Live code replacement during execution (К-extensions — Item 22) |
| ftrace / perf | Observability tracing infrastructure (Item 19) |
| /proc/ introspection | Scheduler diagnostic API (Item 4) |
| cli / sti | Scheduler suspend / resume intrinsics (Item 20) |
| stop_machine | Scheduler panic_halt intrinsic (Item 20) |
| SCHED_FIFO / SCHED_NORMAL | Scheduling class attributes (Item 6) |
| cgroups CPU shares | Resource quotas (Item 7) |
| Shared memory segment | Shared memory regions (Item 9) |
| Lock-free | К10 scheduler implementation pattern (TLA+ verified — Item 18) |
| Work stealing | Idle thread steals от busy thread queue (Item 12) |
| NUMA awareness | CPU topology-aware scheduling (Item 10 — К-extensions) |
| Real-time guarantees | Bounded latency for RT class (Item 23) |

---

## Part 11 — Closing notes

К10 architectural deliberation complete (2026-05-17). All 9 S surfaces ratified. К-Lxx invariant series 11 → 20 (К-L6 SUPERSEDED + К-L7.1 sub + К-L12 through К-L19). К10 scope 25 → 46 items.

**К-L14 evidence state at К-series closure** (per S4 lock): architecturally established through К10 architectural completeness; measurably confirmed-or-falsified through К11+ measurements. К-L14 *architecturally established* at К10 closure (А'.7); К-L14 *measurably confirmed-or-falsified* deferred к К11+ post-К-series measurements.

**К-L19 hardware tier commitment** (per S8-Q5 lock): Dual Frontier requires Vulkan 1.3 hardware с async compute queue family support. Target hardware tier baseline для performance prediction measurements. Project release timeline accounts for hardware proliferation.

**TLA+ specification authoring** (per S-TLA lock): 12 invariants targeted, safety CI gate, liveness verification deeper models для К-L7.1/К-L12/К-L16. Architectural artifact deliverable в К10 scope.

**Pipeline architecture** (post-К10 reality):
- Data plane: native (К-L11 NativeWorld single source of truth)
- Control plane: native (К-L12 full native kernel scheduling)
- Event routing: native (К-L15 three-tier bus)
- Tick lifecycle: pipelined (К-L16 simulation pipeline depth)
- Display composition: multi-layer (К-L17 latency separation)
- Mod lifecycle: quiescent (К-L18 simulation paused during load/unload)
- Hardware: target tier committed (К-L19 Vulkan 1.3 + async compute)

**Tier 1 LOCKED status**: К10 specification promoted к Tier 1 LOCKED at this amendment landing (2026-05-17). Lock gates К10 execution brief authoring, which translates spec к Claude Code execution material.

**Reading order для К10 implementation brief authoring**:
1. This document (KERNEL_FULL_NATIVE_SCHEDULER.md, post-amendment v2.0)
2. K10_DELIBERATION_STATE.md (Project file, deliberation rationale)
3. SESSION_LOG_2026_05_17_K10_DELIBERATION.md (Project file, session transfer log)
4. METHODOLOGY.md v1.8 (post-amendment, Lessons #11/#20/#22 + Provisional Lessons)
5. VULKAN_SUBSTRATE.md (post-amendment, V-side К-L7.1/К-L16/К-L17/К-L18/К-L19 integration)
6. MOD_OS_ARCHITECTURE.md (post-amendment, capability + lifecycle amendments)

К-series complete arc:
- К0-К9: Data plane native, K-L3.1 bridge
- К8.3+К8.4 (A'.5): Storage cutover, K-L11 realized
- К8.5 (A'.6): Mod ecosystem migration prep
- **К10 (A'.7 — this document)**: Control plane native, К-L6 superseded, К-L7.1 sub-invariant + К-L12 through К-L19 added (8 new invariants + 1 sub), OS-faithful semantics complete
- К-closure report (А'.8): Formal К-series closure, K-L invariant enumeration (20 invariants), analyzer rule specification surface
- Roslyn analyzer (А'.9): Encodes К-Lxx invariants, gates Phase B M-series

Post-К-series state: Dual Frontier kernel is **architecturally complete** as native OS-faithful microkernel. Subsequent work (К-extensions, M-series, V substrate, К11+ Core systems native migration) builds on stable kernel foundation. Decade-horizon project has its **kernel** as canonical artifact.

The «no shortcuts» commitment applied к К-series: 11 K-Lxx invariants pre-К10 → 20 K-Lxx invariants post-К10 (К-L6 SUPERSEDED + К-L7.1 sub + К-L12 through К-L19). Each invariant principled. Each architectural decision recorded with rationale. К-series serves as **case study** для research framework claim that solo dev + LLM pipeline can produce non-trivial systems software through architectural deliberation discipline.

«Без костылей.» Each invariant principled. Each architectural decision recorded с rationale. К-series serves as case study для research framework claim about LLM pipeline + solo developer + non-trivial systems software.

**Document end. Tier 1 LOCKED post-amendment landing 2026-05-17. Companion deliberation state document (`K10_DELIBERATION_STATE.md`, Project file) carries Q-by-Q lock rationale and Lesson candidates.**
