---
register_id: DOC-A-TIME_AND_CONSISTENCY_MODEL
project: Dual Frontier
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: 0.1.3
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: post-ratification closure
title: Time & Consistency Model — canonical time vocabulary, visibility table, determinism classes D0/D1/D2, RNG law (A4 draft)
last_modified_commit: 0145f1b
review_cadence: on-status-transition
last_review_date: 2026-07-17
last_review_event: 'DRAFTS_RATIFICATION Phase B (C4): HALT-1-ratified retargets TCM-1..TCM-4 — R4-7 dangling-phase cites re-homed (THREADING §7 tick-boundary; COMBO §2 footnote; FEEDBACK_LOOPS §2); R4-10 RNG census corrected (FOUR fixed-seed sites, semantic-analyzer nuance added); §7.1/§7.5 re-marked RESOLVED by rework, §7.2 reframed (both LOCKED texts now defer to this contract); §6.1 cite reform.'
reviewer: Crystalka
special_case_rationale: 'Tier 1 AUTHORED override (forbidden pair; precedent DOC-A-K_CLOSURE_REPORT): authored-proposal draft of the missing A4 cross-cutting contract per ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715 §7. Tier 1 per FRAMEWORK §3.4; AUTHORED because unratified — preamble marks normative-target, NOT current truth; LOCKED docs prevail until ratification per FRAMEWORK §7.'
---

# Time and Consistency Model (the A4 contract)

> **Document class: authored-proposal (normative-target). NOT current truth, NOT enforceable law.** This is the missing "A4" contract identified by the Architecture Decomposition & Contracts session 2026-07-15 (report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md), HEAD `6f39903`): a single time/visibility model for semantics the corpus currently ships scattered across six-plus documents in local vocabularies, with at least one direct contradiction (§7.1). It becomes normative only upon Crystalka ratification per [FRAMEWORK](../governance/FRAMEWORK.md) §7 (amendment milestone protocol, §7.2). **Until each named amendment lands, any conflict between this draft and a LOCKED document resolves in favor of the LOCKED document** (conflicts are enumerated honestly in §7).

**Ratification path** (per FRAMEWORK.md §7.2; each destination bumps per its own amendment protocol):

| This document's section | Folds into, on ratification |
|---|---|
| §1 canonical vocabulary + §2 visibility table | [THREADING](./THREADING.md) — additive sections (MINOR per its amendment protocol) |
| §3 consistency rules | [THREADING](./THREADING.md), same amendment |
| §5 determinism classes | [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 (a determinism-class note against the K-L rows) or [PERFORMANCE](./PERFORMANCE.md) |
| §7.1 FIELDS wording fix | [FIELDS](./FIELDS.md) — named amendment (PATCH: correction) |
| §5.2 / §7.2 COMBO re-scoping | [COMBO_RESOLUTION](../mechanics/COMBO_RESOLUTION.md) (+ [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) §7.2.1 if Option B) — named amendment |
| §4, §6 | Remain here; document then transitions authored-proposal → AUTHORED → LOCKED |

Every subsystem already answers "when does my write become visible, and to whom?" — but each answers in its own dialect: K-L16 speaks in display lag, [FEEDBACK_LOOPS](../mechanics/FEEDBACK_LOOPS.md) in previous-tick snapshots, [ECS](./ECS.md) in phase-boundary batch flushes, [EVENT_BUS](./EVENT_BUS.md) in flushes and drains, [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) in fences and slot tails. This document normalizes the vocabulary (§1), tabulates every publication boundary in one place (§2), states which observer-consistency guarantees hold across those boundaries (§3), pins the pause/save/load boundaries (§4), and resolves the determinism vocabulary mess by defining explicit determinism classes (§5) plus the RNG law they require (§6).

File:line anchors below were authored against HEAD `6f39903` (2026-07-15); code anchors re-verified unchanged at ratification HEAD `48983c4` (EVT-2026-07-17-DRAFTS_RATIFICATION); doc anchors retargeted to the LOCKED v1.0.0 successors (which superseded the pre-rework texts in place) per CODING_STANDARDS §6.1 form.

## 1. Canonical time vocabulary (normative)

Seven terms. Every statement about time or visibility in the architecture corpus MUST be expressible in these terms.

**SimTick** — the monotonic simulation tick counter, advanced once per fixed step on the dedicated simulation thread at 30 Hz.

- Owner: `TickScheduler.CurrentTick` (`src/DualFrontier.Core/Scheduling/TickScheduler.cs:36`); driver: `GameLoop` (`src/DualFrontier.Application/Loop/GameLoop.cs:29-31` — `TargetTps = 30f`, `FixedDelta = 1/30`); [THREADING](./THREADING.md) §3 (phase barrier), §6 (tick rates and wakes).
- `[TickRate]` periods (REALTIME=1, FAST=3, NORMAL=15, SLOW=60, RARE=3600; [THREADING](./THREADING.md) §6) stretch a *producer's cadence* in SimTicks; they do not change visibility semantics.

**PhaseId** — the ordinal of a dependency-graph phase within one SimTick.

- Managed phases are the ordered `SystemPhase` lists computed by `DependencyGraph`; the count is data-dependent, not fixed — the fixed five-phase scaffold was retired in the THREADING rework ([THREADING](./THREADING.md) §2 (native scheduler — BFS-layer phases) and §7 (feedback cycles — the correct anchor is the tick boundary, not a numbered phase)).
- Native scheduling adds the per-tick macro-phases `Phase_Update = 0`, `Phase_Compute = 1`, `Phase_Display = 2` (`native/DualFrontier.Core.Native/include/phase_compute.h:49-51`).
- The *tick-finalization phase* is the last PhaseId of a tick, where previous-tick snapshot copies happen ([FEEDBACK_LOOPS](../mechanics/FEEDBACK_LOOPS.md) §2 (snapshot-pattern shape)); the predecessor's "Phase 5 — Feedback snapshot" anchor was retired with the five-phase scaffold — see §7.5 (closed by the rework).

**DisplayTick** — `SimTick − D` for pipeline-managed state.

- D is the K-L16 pipeline depth: configurable 1–3, default 2 (К-L16, [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 — "Display thread reads results from simulation tick (CurrentSimTick − D)").
- Display latency invariant: D × tick period ≈ 66.7 ms at D=2, 30 Hz (K-L16 row).

**PipelineGeneration** — the `sim_tick` stamped into a `PipelineSlot` at dispatch.

- Source: `native/DualFrontier.Core.Native/include/pipeline_slot.h`; [KERNEL_FULL_NATIVE_SCHEDULER](./historical/KERNEL_FULL_NATIVE_SCHEDULER.md) §3.10 Item 33 (:969-977).
- Slot lifecycle: `Empty → Dispatched → FenceCompleted → ReadableAsTail` (`pipeline_slot.h:10-11,47-50`).
- The *declared tail* differs per consumer: a sim-thread read of a pipeline-managed field at tick T+D sees generation T+D−1 (К-L7.1, [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0); the display thread sees generation SimTick−D (K-L16).

**FlushGeneration** — the per-bus counter of deferred-queue drains, one per phase boundary per domain bus.

- A specification device: no such counter exists in code today ([EVENT_BUS](./EVENT_BUS.md) §7 (observability is inverted) — "The managed `DomainEventBus` keeps no counters").
- The physical mechanism is the snapshot drain in `DomainEventBus.FlushDeferred` (`src/DualFrontier.Core/Bus/DomainEventBus.cs:115-135`: the queue is emptied into a local batch *before* any handler runs).
- An event deferred during flush generation F is delivered at F+1, never F ([EVENT_BUS](./EVENT_BUS.md) §2 (snapshot re-entrancy)).

**FrameIndex** — the render-frame counter on the presentation main thread (`src/DualFrontier.Launcher/Program.cs:70-92` main loop).

- Deliberately unsynchronized to SimTick: the sim thread enqueues one `TickAdvancedCommand` per SimTick (`GameLoop.cs:116`); the renderer drains the bridge once per frame. A frame may observe zero, one, or several ticks' worth of commands.

**Catch-up window** — the bounded backlog of fixed steps the sim thread may execute back-to-back after a stall.

- The accumulator is clamped to `MaxAccumulator = FixedDelta * 5` (`GameLoop.cs:31`, applied at `:110`), so at most 5 consecutive steps run; wall time beyond the clamp is discarded — under sustained overload sim time slips relative to wall time, it never bursts more than 5 steps.
- Pause accumulates no debt: paused iterations consume the clock (`GameLoop.cs:101-107`), so resume replays nothing.

Three deliberately overloaded words, to be qualified on every use:

- **"slot"** — a K-L16 `PipelineSlot` is unrelated to the Background tier's "idle-slot dispatch", which is an *intra-tick CPU budget window* (`df_background_queue_dispatch_idle_slot(budget_micros)`, [EVENT_BUS](./EVENT_BUS.md):59; budget computed per tick at `GameLoop.cs:118-128`).
- **"snapshot"** — four distinct things today: the `_Previous` component copy (THREADING §7 / FEEDBACK_LOOPS), the consistent phase view after a `WriteBatch` flush ([ECS](./ECS.md) §4), the subscriber-list array copy taken during dispatch (EVENT_BUS), and the per-slot world/fields snapshot (historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 33).
- **"tick"** — always SimTick below; "display tick" is DisplayTick; `[TickRate]` multiples are cadences, not a second clock.

### 1.1 Term mapping (existing doc term → canonical)

| Doc term | Where | Canonical term |
|---|---|---|
| `CurrentSimTick` | К-L16 (KERNEL_ARCHITECTURE Part 0) | SimTick |
| "monotonic tick counter" | THREADING §6 (`TickScheduler.CurrentTick`) | SimTick |
| "tick N / N+1", "tick boundary" | FEEDBACK_LOOPS §2 (snapshot pattern) | SimTick; tick-finalization PhaseId |
| `sim_tick` (slot field) | `pipeline_slot.h`; historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 33 | PipelineGeneration |
| "slot tail", "slot-tail state" | К-L7.1; VULKAN §2.5 (pipeline depth and slots) | PipelineGeneration G−1 (sim reader) / G−D (display reader) |
| "Display thread reads CurrentSimTick − D" | К-L16; VULKAN §2.5 | DisplayTick |
| "Phase 5 — Feedback snapshot" (retired predecessor anchor) | historical/FEEDBACK_LOOPS.md; rule now THREADING §7 | tick-finalization PhaseId (§7.5, closed) |
| "Phase 4 (Apply & Damage)" (retired predecessor anchor) | historical/COMBO_RESOLUTION.md; see COMBO_RESOLUTION §2 phase-numbering footnote | PhaseId (§7.5, closed) |
| "phase boundary" | ECS §4/§6; EVENT_BUS §2 | PhaseId boundary (phase commit) |
| "delivered at the next phase boundary" / "the next flush" | EVENT_BUS §2 | FlushGeneration F+1 |
| `Phase_Update` / `Phase_Compute` / `Phase_Display` | `phase_compute.h:49-51` | native macro-PhaseId values |
| "frame", per-frame bridge drain | `PresentationBridge.cs`; `Program.cs:70-92` | FrameIndex |
| "one-tick stale data is invisible" | VULKAN §5 (sync semantics) | sync-path staleness ≤ 1 SimTick |
| "idle-slot dispatch", `budget_micros` | EVENT_BUS §3 (Background tier); `GameLoop.cs:118-128` | intra-tick budget window (NOT a PipelineSlot) |
| `MaxAccumulator` clamp | `GameLoop.cs:31` | catch-up window |

## 2. The visibility table

One row per producer class. "Publication boundary" is the moment after which the write can be observed by anyone other than the producer; "consumer-visible state" says who sees what, and when, in §1 vocabulary.

| # | Producer | Publication boundary | Consumer-visible state |
|---|---|---|---|
| 1 | Component write via `WriteBatch<T>` | Phase commit — batch flushed at the next PhaseId boundary (ECS §4 (span/batch access pattern)) | Span readers in PhaseId+1 (same SimTick) observe a consistent post-flush snapshot; readers within the writing phase never see it |
| 2 | Entity creation / component add | Same `WriteBatch` flush at PhaseId boundary (ECS §4/§6) | New entity invisible to parallel readers until PhaseId+1; "create + write this phase, read next phase" is the documented split |
| 3 | Entity destroy | Deferred to PhaseId boundary; version bump (ECS §6 (entity lifecycle)) | From PhaseId+1: `TryGetComponent` false, `IsAlive` false; writes staged against the dead `EntityId` are **silently dropped at flush** (ECS §6) |
| 4 | Managed sync event (default) | None — handlers run inside `Publish`, publisher's thread, current PhaseId (EVENT_BUS §2 (managed delivery)) | Immediate; handler exceptions caught and logged, publication continues to remaining subscribers |
| 5 | Managed `[Deferred]` event | Queue-snapshot drain at the phase barrier: after `Parallel.ForEach` completes, `IDeferredFlush.FlushDeferred()` (EVENT_BUS §2; THREADING §3; `DomainEventBus.cs:115-135`) | Handlers run at FlushGeneration F+1 under the *subscriber's* captured context; a deferred event published from inside a deferred handler lands at F+2 (EVENT_BUS §2) |
| 6 | Previous-tick snapshot component (`*Snapshot`, e.g. `ManaSnapshot`) | Tick finalization: scheduler copies `ManaComponent → ManaSnapshot` at the tick boundary (THREADING §7 (normative rule); FEEDBACK_LOOPS §2 (mechanic application)) | Readers on SimTick N+1 get `ManaSnapshot[N]` — the post-drain state of tick N; +1 SimTick latency by design (FEEDBACK_LOOPS §2/§4) |
| 7 | Pipeline-managed field (K-L7.1 opt-in) | Fence completion + slot publish: `FenceCompleted → ReadableAsTail` (`pipeline_slot.h:107`; historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 33) | Declared slot tail: sim tick T+D reads dispatched-at-(T+D−1) state (К-L7.1); display reads generation SimTick−D (К-L16). Lag bounded and deterministic in depth |
| 8 | Sync-path field dispatch (K-L7, V1 default) | Fence wait *inside the dispatch call* — "consumer call returns after fence signals" (VULKAN §5.1 (К-L7 sync path)) | Immediate: a subsequent `ReadCell` sees the dispatched result. **The call BLOCKS the sim thread** — the predecessor FIELDS "non-blocking" wording was corrected by the successor (FIELDS §10 (CPU/GPU exclusion): "Sync dispatch blocks; it does not return early") — §7.1, closed |
| 9 | Native Normal-tier event | Batch drain: `df_bus_drain_normal_batch` at a phase boundary (EVENT_BUS §3 (native tiers)) | Post-drain, atomic-from-observer within the drained batch (K-L7-within-batch, EVENT_BUS §3). Dormant: no production call site today — production events travel the managed path (EVENT_BUS §4 (division of labor)) |
| 10 | Native Background-tier event | Coalesce by `(type_id, coalesce_key)` + idle-slot dispatch within remaining tick budget (`df_background_queue_dispatch_idle_slot(budget_micros)`, EVENT_BUS §3; drained after every fixed step, `GameLoop.cs:118-128`; EVENT_BUS §4) | Eventually, coalesced; no per-event deadline; drop-oldest with warning counter at the size cap (default 10 MB, warn at 80%) |
| 11 | Presentation command | `PresentationBridge.Enqueue` into an **unbounded** `ConcurrentQueue` (`src/DualFrontier.Application/Bridge/PresentationBridge.cs:21`); one `TickAdvancedCommand` per SimTick (`GameLoop.cs:116`) | Next FrameIndex drain on the render main thread (`DrainCommands`); strictly one-way; queue depth observable but uncapped |
| 12 | Player input | **TODAY: never.** The Launcher pumps and drains `InputEventQueue`, then discards every event — "Future cascade will forward" (`Program.cs:81-84`; VULKAN §2.2 (window and input truth)) | Nothing reaches the simulation. The K-L17 intent overlay reads input at display time (≤16 ms, VULKAN §2.6 (display composition)) but no input→sim boundary exists; marked explicitly as a hole, §7.3 |

Reading rules for the table:

1. Rows 1–3 are one physical mechanism (batch flush at phase commit); they are split because their *failure modes* differ — row 3's silent drop is the only one that discards data.
2. Rows 5 and 9 are two implementations of one idea (deliver after the barrier, atomically as a batch); only row 5 carries production traffic today.
3. Rows 7 and 8 are per-field author choices (K-L9 "Vanilla = mods"; S-LOCK-10/13 coexistence, VULKAN §2.5). A single system may read both kinds in one tick — see §3 for the resulting permitted skew.
4. Latency contracts stack on top of visibility, they do not replace it: K-L17 gives the intent overlay ≤1 frame (~16 ms) and combat feedback ≤17 ms event-to-visible via the Fast tier (≤1 ms subscriber budget, EVENT_BUS §3) + display (К-L17, KERNEL_ARCHITECTURE Part 0; VULKAN §2.6). These bound *when pixels change*, not when state publishes.
5. A producer's `[TickRate]` delays *production*, not publication: a NORMAL system's writes still publish at the phase commit of the tick it actually ran in.

## 3. Cross-boundary consistency rules

**Atomic-from-observer holds within, and only within:**

- **A phase snapshot.** During a phase, span walks observe the world as of the last phase commit; no reader sees a half-applied `WriteBatch` (ECS §4). Write-write conflicts within a phase are structurally excluded by the dependency graph (THREADING §2 (write-write exclusion)).
- **A pipeline slot.** All reads served from one `PipelineSlot` see one generation's world+fields snapshot — the K-L7 atomic-from-observer invariant is preserved *within the slot boundary* (К-L7.1; VULKAN §2.5).
- **A drained batch.** Deferred-flush (row 5) and native Normal drain (row 9) deliver their batch against a stable subscriber snapshot; new deferrals wait for the next generation (EVENT_BUS §2/§3).
- **A span lease.** While any field span is acquired, point writes, conductivity/flag updates, and ping-pong swaps are rejected (`active_spans_` atomic counter, FIELDS §5 (span-lifetime law)).

**Cross-boundary inconsistency is PERMITTED (by design) between:**

- **Slots.** "Cross-slot reads see different snapshots" — K-L7.1 says so verbatim. Two reads of pipeline-managed state served by different slots may disagree by one generation.
- **Render tail and sim head.** The display composites DisplayTick (SimTick−D) sim state under a current-input intent overlay (К-L17 composition order: SimState first, intent + combat overlays on top, static last — KERNEL_ARCHITECTURE Part 0). The screen intentionally shows two times at once.
- **K-L7 and K-L7.1 fields in one system.** A sync-path read returns tick-current state (row 8); a pipeline-managed read returns generation-lagged state (row 7). Skew ≤ 1 generation, permitted; systems mixing both must not equate the two timestamps.
- **Sync-path reads and an in-flight dispatch.** A `ReadCell` racing a dispatch may see last-tick or new state; "one-tick stale data is invisible" is the accepted bound (VULKAN §5 (sync semantics)).
- **Frames and ticks.** No frame is guaranteed to observe every tick individually (row 11); presentation consumers must tolerate command bursts, including catch-up bursts of up to 5 ticks (§1, catch-up window).

**Forbidden (a bug wherever observed):**

- Torn reads inside a span or slot; observing a partially applied batch; observing an entity between destroy-staging and version bump.
- A deferred handler observing pre-barrier partial writes at FlushGeneration F (the flush runs strictly after the `Parallel.ForEach` barrier, THREADING §3).
- Staleness exceeding the declared bound: >1 generation on a slot tail, >1 SimTick on a `*Snapshot` component, unbounded delay on Normal-tier delivery once the native path carries traffic.

## 4. Pause, save, load boundaries

### 4.1 Pause — two operations, one word

- **Flag pause**: `GameLoop.SetPaused(true)` sets a volatile flag; the in-flight tick completes; the loop then sleeps without advancing (`GameLoop.cs:80,103-107`). Fire-and-forget — the caller gets no acknowledgement that the tick finished.
- **Acknowledged quiesce** (K-L18): `SimulationStateController.PauseAsync → WaitForQuiescenceAsync(timeout)` (К-L18; THREADING §8). The native predicate is `df_pipeline_is_quiescent` — quiescent ⇔ every slot is `Empty` or `ReadableAsTail`, none `Dispatched`/`FenceCompleted` (`pipeline_slot.h:112-117`; consumed by the mod-unload primitive, `native/DualFrontier.Core.Native/src/mod_unload.cpp:65-67`; historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 41).
- Pause converges naturally: sim completes the current tick, dispatches nothing new, pipeline depth absorbs already-dispatched work (VULKAN §7.1 (GPU save protocol); historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 34).
- Mod load/unload requires acknowledged quiesce, never just the flag (K-L18 precondition: `sim_state == Paused && pipeline_slots_quiescent()`).

### 4.2 Save — which SimTick, which queues, no drain

- The saved world is the **DisplayTick state (CurrentSimTick − D)** — the coherent tail the display already sees; **pipeline drain is NOT required at save** (VULKAN §7.1 (GPU save protocol — forward-references PERSISTENCE_SNAPSHOT_CONTRACT §1); historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 34 — "pipeline drain не required").
- Tension to reconcile: the K-L16 row's own wording says "Pipeline drain orderly at save/pause" (К-L16, KERNEL_ARCHITECTURE Part 0). This draft reads *drain* as the pause-path convergence and *save* as tail-snapshot-without-drain, per the more specific Item 34 / VULKAN §7.1 text; the K-L16 row wording should be aligned at ratification — reconciliation now delegated to PERSISTENCE_SNAPSHOT_CONTRACT §1 (PS-2) by VULKAN §7.1 itself (§7.4).
- A save must also capture the native Background queue: Item 31 save-integrated persist (historical/KERNEL_FULL_NATIVE_SCHEDULER.md Items 31/32, `df_scheduler_serialize_background_queue`; wire format `background_queue.cpp:209` — Item 31, К10.2 schema v1; live successor pointer EVENT_BUS §6 (capacity truths)).
- GPU-derived state: per the VULKAN §7.2 design mitigation (**not implemented** — no field save path exists), the save serializes canonical CPU-computed field state, not GPU-resident bytes. `waitIdle` policy per VULKAN §5.4.
- Managed `[Deferred]` queues are normally empty at tick end (flushed every phase), but deferred-from-deferred residue published during a tick's final flush survives into the next tick and has **no persistence path** — flagged §7.6.

### 4.3 Load and resume

- Fields restore from serialized primary buffers; back buffers restore to zero — in-flight ping-pong state is lost by contract (FIELDS §12 (save/load) — now a FENCED target deferring to PERSISTENCE_SNAPSHOT_CONTRACT).
- The pipeline **refills incrementally**: sim starts at the saved SimTick, display unblocks once D slots are populated (historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 34); resume-from-pause refills from the pause point.
- RNG state is **unspecified today** — production RNG is ad-hoc across four fixed-seed sites — `MovementSystem.cs:35` (`new(42)`), `GameBootstrap.cs:96` (`ObstacleSeed`), `RandomPawnFactory.cs:58` and `ItemFactory.cs:39` (constructor-injected seeds) — none world-derived, none persisted, so a loaded game does not reproduce the pre-save trajectory (§6 fixes this; flagged §7.7).
- Catch-up after resume: none from the pause itself (paused iterations consume the clock, `GameLoop.cs:101-107`); stall-induced backlog is bounded by the catch-up window to 5 back-to-back steps (`GameLoop.cs:31,110-131`).

## 5. Determinism classes

The corpus asserts three incompatible strengths of "deterministic" without naming them. This section defines the classes; every future claim MUST cite one.

**D0 — best-effort.** No cross-run guarantees.

- Within-phase system execution order is unordered (`Parallel.ForEach`, THREADING §3; MaxDoP = N−2, `ParallelSystemScheduler.cs:90`; admitted as the nondeterminism source by COMBO_RESOLUTION §5 (why sort and not a queue)).
- RNG is ad-hoc: four fixed-seed `Random` instances (canonical worst example `MovementSystem.cs:35`, `new(42)`), each with consumption order coupled to iteration/tick interleaving and position never persisted. There is no RNG service.
- GPU float results vary across hardware/driver — "floating-point ordering, parallel reduction differences, and driver optimizations" (VULKAN §7.2 (determinism posture)).
- Structural invariants still hold under D0 — e.g. K-L7.1's "one-tick lag bounded и deterministic" is *lag-depth* determinism, not bit determinism.
- **This is the engine's current class, everywhere.**

**D1 — save-reproducible.** Loading a save yields identical observable state: same component values, same field cells, same persisted Background queue, same SimTick.

- This is VULKAN's own requirement — "Save/load must produce reproducible state on load" (VULKAN §7.2).
- D1 constrains *state at the save boundary*, not future evolution: two sessions resumed from one save may diverge.
- Needs (none currently met): canonical CPU field state at save per the VULKAN §7.2 design mitigation; a seeded RNG service whose state is derivable or serialized (§6); no GPU-derived bytes in the saved state; Item 31 background persist wired; a save path at all (Persistence is orphaned from production today).

**D2 — replay / byte-identical.** Identical initial state + identical input stream ⇒ byte-identical event stream across runs (and across hardware, if claimed).

- This is COMBO_RESOLUTION's promise: "the resulting `DamageEvent` stream is byte-identical between runs" (COMBO_RESOLUTION §4 (determinism guarantee — design intent, hedged by the successor)), with the replay suite and replay-from-command-log consequence in the same section.
- Needs: fixed system order or order-independent semantics, deterministic RNG (§6), event-stream capture, bit-stable arithmetic.
- **Not currently scoped**: VULKAN declares "single-player, no replays" (VULKAN §7.2).

### 5.1 Claim assignment

| Claim | Source | Class asserted | Class held today |
|---|---|---|---|
| "byte-identical `DamageEvent` stream between runs" | COMBO_RESOLUTION §4 (LOCKED; design-intent hedge + "aspirational D2, effective D0" note citing this contract) | D2 | D0 (`ComboResolutionSystem` throws `NotImplementedException`) |
| "reproducible combat, correct replay, and stable tests" | FEEDBACK_LOOPS §4 (cost) | D2 vocabulary | snapshot rule supports D1/D2; delivers neither alone |
| "Save/load must produce reproducible state on load" | VULKAN §7.2 | D1 | unmet — no field save path, RNG unspecified |
| "single-player, no replays"; cross-hardware GPU variance accepted | VULKAN §7.2 (cedes class vocabulary to this contract) | caps engine at D1 | consistent with D0 reality |
| "One-tick lag bounded и deterministic" | К-L7.1 (KERNEL_ARCHITECTURE Part 0) | structural (lag depth) | holds; orthogonal to D1/D2 |
| "Determinism >> latency" (successor wording: "determinism is worth a tick of delay") | FEEDBACK_LOOPS §4 | design priority | priority honored; class undefined |
| Replay test "same seed and the same event sequence produce the same result" | COMBO_RESOLUTION §4 (suite does not exist — successor-verified) | D2 evidence | absent |

### 5.2 Resolving COMBO vs VULKAN

Two LOCKED documents conflict at the class level: COMBO promises D2 for the damage path; VULKAN denies D2 engine-wide and accepts non-bit-exact GPU state feeding the world. Exactly one of the following must be ratified:

**Option A — downgrade COMBO to D1 scope (recommended default).** Re-word the guarantee to *stable intra-tick ordering*: given one world state and one `DamageIntent` multiset within a tick, the applied order and resulting `DamageEvent` set are identical — independent of thread schedule. This keeps every stated benefit except replay-from-log, which is deleted. Cheap: it is what the sort-key design (COMBO_RESOLUTION §3) actually buys.

**Option B — commit to D2 as a bounded determinism island (damage path only).** The engine stays D1; the island `DamageIntent → ComboResolutionSystem → DamageEvent` is D2 under named conditions:

- **C1 — total sort key.** The current `(EntityId, DamageKind ordinal)` key (COMBO_RESOLUTION §3) is not total: two same-kind intents on one target tie. Extend with a deterministic tiebreaker (publisher SystemId + per-publisher intent sequence).
- **C2 — no GPU-derived inputs.** Any field value entering damage math must come from a CPU-canonical source (CPU reference kernels, VULKAN §7.2 (CPU-canonical mitigation), or the K-L7 sync CPU path) — slot-tail reads are deterministic in *depth* but not bit-stable across hardware.
- **C3 — RNG per §6 only.**
- **C4 — bit-stable arithmetic** inside the island (strict IEEE-754; no FMA-variant intrinsics).
- **C5 — `DamageIntent` stream capture** — which requires amending VULKAN §7.2 "no replays" to "no replays outside the damage island".
- **C6 — island inputs restricted to D1-stable state**, so a loaded save re-enters the island identically.

Until one option is ratified, COMBO's determinism guarantee is to be read as **aspirational D2, effective D0** — the COMBO successor §4 now states exactly this reading itself, citing this section's C1–C6; FEEDBACK_LOOPS "correct replay" (§4) inherits the same downgrade.

## 6. RNG law (proposal)

One rule kills the ad-hoc pattern: **no system constructs its own RNG.** `new Random(42)` at `MovementSystem.cs:35` is the canonical worst example of four fixed-seed instances today (with `GameBootstrap.cs:96`, `RandomPawnFactory.cs:58`, `ItemFactory.cs:39`) and exhibits every failure at once: fixed seed regardless of world, consumption order coupled to iteration order, position lost on load, stream unshareable and uncapturable.

Proposed service (Contracts surface, injected like other `IGameServices` members; mods get the same surface per K-L9):

1. `IRandomService.Stream(SystemId, SimTick)` returns a small counter-based generator seeded by `hash(WorldSeed, SystemId, SimTick)` (SplitMix64/PCG-class mix).
2. **Keying by SimTick makes stream position stateless**: nothing to serialize but `WorldSeed`. D1 save-reproducibility of RNG comes for free, and no cross-tick position drift can occur.
3. `WorldSeed` is created at new-game and stored in the save header. No config/seed source exists today — a gap in its own right (no config contract anywhere in the repo).
4. Per-(system, tick) streams are independent by construction, so within-phase parallelism cannot interleave draws across systems. *Within* one system's tick, draw order must itself be deterministic — companion rule: iterate entities in index order when drawing.
5. Enforcement: an analyzer rule banning system-constructed RNG in `src/DualFrontier.Systems/**` (DFL discipline family, [ANALYZER_RULES](./ANALYZER_RULES.md) §1.1 registry / §4 deferred registry) plus migration of the four existing call sites. The ban must be **semantic (type-based), not textual** — a `new System.Random` regex misses target-typed construction (`MovementSystem.cs:35` is `new(42)`).

D1 requires exactly this much. Option B of §5.2 additionally requires the damage island to draw only from streams keyed to captured inputs — satisfied by the same `(WorldSeed, SystemId, SimTick)` derivation.

## 7. Open questions and LOCKED-doc conflicts

1. **FIELDS "non-blocking" wording — RESOLVED by the rework.** The predecessor's "field dispatches are non-blocking" (retired; historical/FIELDS.md) was corrected by the FIELDS successor §10 (CPU/GPU exclusion — sync-dispatch fence semantics): "Sync dispatch blocks; it does not return early" — exactly this contract's proposed amendment, landed. Rows 7/8 of §2 and the successor agree; no live conflict remains.
2. **COMBO vs VULKAN determinism class conflict — REFRAMED.** Both LOCKED texts now defer to this contract: the COMBO successor §4 hedges to design-intent and states "aspirational D2, effective D0" citing §5.2 C1–C6; VULKAN §7.2 keeps "single-player, no replays" but cedes the class vocabulary here. The Option A/B decision is owned by this contract — a live decision, no longer a LOCKED-vs-LOCKED contradiction. FEEDBACK_LOOPS §4 ("correct replay") amends in the same stroke as the chosen option.
3. **Input path absent.** Row 12: input is drained and discarded (`Program.cs:81-84`); no input→sim visibility contract exists, while K-L17 already grants input a *display*-side latency contract (≤16 ms). When the input bridge lands (ROADMAP §Native foundation tracks), its row in §2 must state the boundary. Proposal: enqueue to sim, visible at the next SimTick start, before its first PhaseId.
4. **K-L16 row vs Item 34 save-drain wording — STILL LIVE, reconciliation delegated.** "Pipeline drain orderly at save/pause" (К-L16, KERNEL_ARCHITECTURE Part 0) vs "pipeline drain не required" at save (historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 34; VULKAN §7.1). §4.2 resolves by specificity; the K-L16 row should say "drain at pause; tail snapshot without drain at save" — and VULKAN §7.1 now explicitly delegates the reconciliation to PERSISTENCE_SNAPSHOT_CONTRACT §1 (PS-2).
5. **Dangling phase vocabulary — RESOLVED by the rework.** The predecessor "Phase 5"/"Phase 4" anchors are retired (historical/FEEDBACK_LOOPS.md, historical/COMBO_RESOLUTION.md); THREADING §7 now owns the tick-boundary anchor for the snapshot copy and the COMBO successor §2 carries the phase-numbering disambiguation footnote. The copy-pass mechanism itself is still unbuilt (THREADING §7 "Not implemented", citing CONCURRENCY_AND_MEMORY_MODEL §9 item 4) — a build item, not a vocabulary dangle.
6. **Deferred-queue residue at save.** Deferred-from-deferred events pending at tick end are unpersisted (managed queues have no save path; Item 31 covers only the native Background tier). Either flush-to-empty becomes a save precondition (drain FlushGenerations until quiescent) or the residue must serialize.
7. **RNG state at load unspecified** (§4.3, §6) — until the RNG law lands, D1 is unreachable in principle, not merely unimplemented.
8. **Native Normal tier has no production driver.** `df_bus_drain_normal_batch` (row 9) is fully specified but never called in production (EVENT_BUS §4 (division of labor)); when the sovereign switch lands, its drain point must be pinned to a named PhaseId boundary so rows 5 and 9 stay equivalent.
9. **"Slot" and "snapshot" overloading** (§1) — low-cost wording hazard; fold the disambiguation list into THREADING together with the vocabulary.

## See also

- [THREADING](./THREADING.md) — scheduler, phase barriers, deferred flush; fold-in target for §1–§3.
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 — the K-L7/L7.1/L16/L17/L18 invariant rows this contract composes.
- [KERNEL_FULL_NATIVE_SCHEDULER](./historical/KERNEL_FULL_NATIVE_SCHEDULER.md) — Items 31–35, 41 (slots, drain/refill, quiescence).
- [EVENT_BUS](./EVENT_BUS.md), [ECS](./ECS.md), [FEEDBACK_LOOPS](../mechanics/FEEDBACK_LOOPS.md) — per-mechanism visibility sources for §2.
- [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) §2.0, §7 — pipeline depth, save/drain semantics, determinism notes.
- [FIELDS](./FIELDS.md), [COMBO_RESOLUTION](../mechanics/COMBO_RESOLUTION.md) — the two named amendment targets (§7.1, §7.2).