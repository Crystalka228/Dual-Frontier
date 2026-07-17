---
register_id: DOC-A-TIME_AND_CONSISTENCY_MODEL
project: Dual Frontier
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: 0.1.2
first_authored: 2026-07-15
last_modified: 2026-07-15
content_language: en
next_review_due: post-ratification closure
title: Time & Consistency Model — canonical time vocabulary, visibility table, determinism classes D0/D1/D2, RNG law (A4 draft)
last_modified_commit: 0145f1b
review_cadence: on-status-transition
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

File:line anchors below are as of HEAD `6f39903` (2026-07-15); doc anchors cite section + line of the current LOCKED/Live texts.

## 1. Canonical time vocabulary (normative)

Seven terms. Every statement about time or visibility in the architecture corpus MUST be expressible in these terms.

**SimTick** — the monotonic simulation tick counter, advanced once per fixed step on the dedicated simulation thread at 30 Hz.

- Owner: `TickScheduler.CurrentTick` (`src/DualFrontier.Core/Scheduling/TickScheduler.cs:36`); driver: `GameLoop` (`src/DualFrontier.Application/Loop/GameLoop.cs:29-31` — `TargetTps = 30f`, `FixedDelta = 1/30`); [THREADING](./THREADING.md):41,86.
- `[TickRate]` periods (REALTIME=1, FAST=3, NORMAL=15, SLOW=60, RARE=3600; [THREADING](./THREADING.md):78-84) stretch a *producer's cadence* in SimTicks; they do not change visibility semantics.

**PhaseId** — the ordinal of a dependency-graph phase within one SimTick.

- Managed phases are the ordered `SystemPhase` lists computed by `DependencyGraph`; the count is data-dependent, not fixed — the historical five-phase scaffold was removed from THREADING at v2.0.0 ([THREADING](./THREADING.md):141, change history).
- Native scheduling adds the per-tick macro-phases `Phase_Update = 0`, `Phase_Compute = 1`, `Phase_Display = 2` (`native/DualFrontier.Core.Native/include/phase_compute.h:49-51`).
- The *tick-finalization phase* is the last PhaseId of a tick, where previous-tick snapshot copies happen. [FEEDBACK_LOOPS](../mechanics/FEEDBACK_LOOPS.md):40 calls it "Phase 5 — Feedback snapshot" — a dangling reference, see §7.5.

**DisplayTick** — `SimTick − D` for pipeline-managed state.

- D is the K-L16 pipeline depth: configurable 1–3, default 2 ([KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0, K-L16 row :65 — "Display thread reads results from simulation tick (CurrentSimTick − D)").
- Display latency invariant: D × tick period ≈ 66.7 ms at D=2, 30 Hz (K-L16 row).

**PipelineGeneration** — the `sim_tick` stamped into a `PipelineSlot` at dispatch.

- Source: `native/DualFrontier.Core.Native/include/pipeline_slot.h`; [KERNEL_FULL_NATIVE_SCHEDULER](./historical/KERNEL_FULL_NATIVE_SCHEDULER.md) §3.10 Item 33 (:969-977).
- Slot lifecycle: `Empty → Dispatched → FenceCompleted → ReadableAsTail` (`pipeline_slot.h:10-11,47-50`).
- The *declared tail* differs per consumer: a sim-thread read of a pipeline-managed field at tick T+D sees generation T+D−1 (K-L7.1 row, [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md):56); the display thread sees generation SimTick−D (K-L16).

**FlushGeneration** — the per-bus counter of deferred-queue drains, one per phase boundary per domain bus.

- A specification device: no such counter exists in code today ([EVENT_BUS](./EVENT_BUS.md):123 — "The managed `DomainEventBus` keeps no counters").
- The physical mechanism is the snapshot drain in `DomainEventBus.FlushDeferred` (`src/DualFrontier.Core/Bus/DomainEventBus.cs:115-135`: the queue is emptied into a local batch *before* any handler runs).
- An event deferred during flush generation F is delivered at F+1, never F ([EVENT_BUS](./EVENT_BUS.md):49).

**FrameIndex** — the render-frame counter on the presentation main thread (`src/DualFrontier.Launcher/Program.cs:70-92` main loop).

- Deliberately unsynchronized to SimTick: the sim thread enqueues one `TickAdvancedCommand` per SimTick (`GameLoop.cs:116`); the renderer drains the bridge once per frame. A frame may observe zero, one, or several ticks' worth of commands.

**Catch-up window** — the bounded backlog of fixed steps the sim thread may execute back-to-back after a stall.

- The accumulator is clamped to `MaxAccumulator = FixedDelta * 5` (`GameLoop.cs:31`, applied at `:110`), so at most 5 consecutive steps run; wall time beyond the clamp is discarded — under sustained overload sim time slips relative to wall time, it never bursts more than 5 steps.
- Pause accumulates no debt: paused iterations consume the clock (`GameLoop.cs:101-107`), so resume replays nothing.

Three deliberately overloaded words, to be qualified on every use:

- **"slot"** — a K-L16 `PipelineSlot` is unrelated to the Background tier's "idle-slot dispatch", which is an *intra-tick CPU budget window* (`df_background_queue_dispatch_idle_slot(budget_micros)`, [EVENT_BUS](./EVENT_BUS.md):59; budget computed per tick at `GameLoop.cs:118-128`).
- **"snapshot"** — four distinct things today: the `_Previous` component copy (FEEDBACK_LOOPS), the consistent phase view after a `WriteBatch` flush ([ECS](./ECS.md):90), the subscriber-list array copy taken during dispatch (EVENT_BUS), and the per-slot world/fields snapshot (KFNS Item 33).
- **"tick"** — always SimTick below; "display tick" is DisplayTick; `[TickRate]` multiples are cadences, not a second clock.

### 1.1 Term mapping (existing doc term → canonical)

| Doc term | Where | Canonical term |
|---|---|---|
| `CurrentSimTick` | KERNEL_ARCHITECTURE Part 0, K-L16 row (:65) | SimTick |
| "monotonic tick counter" | THREADING:86 (`TickScheduler.CurrentTick`) | SimTick |
| "tick N / N+1", "tick boundary" | FEEDBACK_LOOPS:37-47 | SimTick; tick-finalization PhaseId |
| `sim_tick` (slot field) | `pipeline_slot.h`; KFNS Item 33 (:971) | PipelineGeneration |
| "slot tail", "slot-tail state" | K-L7.1 row (:56); VULKAN §7.3.0 (:1301-1305) | PipelineGeneration G−1 (sim reader) / G−D (display reader) |
| "Display thread reads CurrentSimTick − D" | K-L16 row (:65); VULKAN §2.0 (:331) | DisplayTick |
| "Phase 5 — Feedback snapshot" | FEEDBACK_LOOPS:40 | tick-finalization PhaseId (dangling, §7.5) |
| "Phase 4 (Apply & Damage)" | COMBO_RESOLUTION:39,104 | PhaseId (dangling, §7.5) |
| "phase boundary" | ECS:90,100; EVENT_BUS:44 | PhaseId boundary (phase commit) |
| "delivered at the next phase boundary" / "the next flush" | EVENT_BUS:44,49 | FlushGeneration F+1 |
| `Phase_Update` / `Phase_Compute` / `Phase_Display` | `phase_compute.h:49-51` | native macro-PhaseId values |
| "frame", per-frame bridge drain | `PresentationBridge.cs`; `Program.cs:70-92` | FrameIndex |
| "one-tick stale data is invisible" | VULKAN §7.3.1 (:1311-1314) | sync-path staleness ≤ 1 SimTick |
| "idle-slot dispatch", `budget_micros` | EVENT_BUS:59; `GameLoop.cs:118-128` | intra-tick budget window (NOT a PipelineSlot) |
| `MaxAccumulator` clamp | `GameLoop.cs:31` | catch-up window |

## 2. The visibility table

One row per producer class. "Publication boundary" is the moment after which the write can be observed by anyone other than the producer; "consumer-visible state" says who sees what, and when, in §1 vocabulary.

| # | Producer | Publication boundary | Consumer-visible state |
|---|---|---|---|
| 1 | Component write via `WriteBatch<T>` | Phase commit — batch flushed at the next PhaseId boundary (ECS:67,90) | Span readers in PhaseId+1 (same SimTick) observe a consistent post-flush snapshot; readers within the writing phase never see it |
| 2 | Entity creation / component add | Same `WriteBatch` flush at PhaseId boundary (ECS:96,180-188) | New entity invisible to parallel readers until PhaseId+1; "create + write this phase, read next phase" is the documented split |
| 3 | Entity destroy | Deferred to PhaseId boundary; version bump (ECS:100) | From PhaseId+1: `TryGetComponent` false, `IsAlive` false; writes staged against the dead `EntityId` are **silently dropped at flush** (ECS:100) |
| 4 | Managed sync event (default) | None — handlers run inside `Publish`, publisher's thread, current PhaseId (EVENT_BUS:43) | Immediate; handler exceptions caught and logged, publication continues to remaining subscribers |
| 5 | Managed `[Deferred]` event | Queue-snapshot drain at the phase barrier: after `Parallel.ForEach` completes, `IDeferredFlush.FlushDeferred()` (EVENT_BUS:44,47; THREADING:41; `DomainEventBus.cs:115-135`) | Handlers run at FlushGeneration F+1 under the *subscriber's* captured context; a deferred event published from inside a deferred handler lands at F+2 (EVENT_BUS:49) |
| 6 | Previous-tick snapshot component (`*Snapshot`, e.g. `ManaSnapshot`) | Tick finalization: scheduler copies `ManaComponent → ManaSnapshot` at the tick boundary (FEEDBACK_LOOPS:40) | Readers on SimTick N+1 get `ManaSnapshot[N]` — the post-drain state of tick N; +1 SimTick latency by design (FEEDBACK_LOOPS:41,54) |
| 7 | Pipeline-managed field (K-L7.1 opt-in) | Fence completion + slot publish: `FenceCompleted → ReadableAsTail` (`pipeline_slot.h:107`; KFNS Item 33) | Declared slot tail: sim tick T+D reads dispatched-at-(T+D−1) state (K-L7.1 row :56); display reads generation SimTick−D (K-L16 row :65). Lag bounded and deterministic in depth |
| 8 | Sync-path field dispatch (K-L7, V1 default) | Fence wait *inside the dispatch call* — "consumer call returns after fence signals" (VULKAN §2.3.1:549, :600) | Immediate: a subsequent `ReadCell` sees the dispatched result. **The call BLOCKS the sim thread** — FIELDS:372 "field dispatches are non-blocking" is wrong for this path and must be amended (§7.1) |
| 9 | Native Normal-tier event | Batch drain: `df_bus_drain_normal_batch` at a phase boundary (EVENT_BUS:57) | Post-drain, atomic-from-observer within the drained batch (K-L7-within-batch, EVENT_BUS:57). Dormant: no production call site today — production events travel the managed path (EVENT_BUS:76-78) |
| 10 | Native Background-tier event | Coalesce by `(type_id, coalesce_key)` + idle-slot dispatch within remaining tick budget (`df_background_queue_dispatch_idle_slot(budget_micros)`, EVENT_BUS:59; drained after every fixed step, `GameLoop.cs:118-128`; EVENT_BUS:77) | Eventually, coalesced; no per-event deadline; drop-oldest with warning counter at the size cap (default 10 MB, warn at 80%) |
| 11 | Presentation command | `PresentationBridge.Enqueue` into an **unbounded** `ConcurrentQueue` (`src/DualFrontier.Application/Bridge/PresentationBridge.cs:21`); one `TickAdvancedCommand` per SimTick (`GameLoop.cs:116`) | Next FrameIndex drain on the render main thread (`DrainCommands`); strictly one-way; queue depth observable but uncapped |
| 12 | Player input | **TODAY: never.** The Launcher pumps and drains `InputEventQueue`, then discards every event — "Future cascade will forward" (`Program.cs:81-84`; VULKAN §2.2:476, §6.2 R.4 row :957) | Nothing reaches the simulation. The K-L17 intent overlay reads input at display time (≤16 ms, VULKAN :923) but no input→sim boundary exists; marked explicitly as a hole, §7.3 |

Reading rules for the table:

1. Rows 1–3 are one physical mechanism (batch flush at phase commit); they are split because their *failure modes* differ — row 3's silent drop is the only one that discards data.
2. Rows 5 and 9 are two implementations of one idea (deliver after the barrier, atomically as a batch); only row 5 carries production traffic today.
3. Rows 7 and 8 are per-field author choices (K-L9 "Vanilla = mods"; S-LOCK-10/13 coexistence, VULKAN §2.0:333). A single system may read both kinds in one tick — see §3 for the resulting permitted skew.
4. Latency contracts stack on top of visibility, they do not replace it: K-L17 gives the intent overlay ≤1 frame (~16 ms) and combat feedback ≤17 ms event-to-visible via the Fast tier (≤1 ms subscriber budget, EVENT_BUS:55) + display (KERNEL_ARCHITECTURE:66; VULKAN :1182). These bound *when pixels change*, not when state publishes.
5. A producer's `[TickRate]` delays *production*, not publication: a NORMAL system's writes still publish at the phase commit of the tick it actually ran in.

## 3. Cross-boundary consistency rules

**Atomic-from-observer holds within, and only within:**

- **A phase snapshot.** During a phase, span walks observe the world as of the last phase commit; no reader sees a half-applied `WriteBatch` (ECS:90). Write-write conflicts within a phase are structurally excluded by the dependency graph (THREADING:62-66).
- **A pipeline slot.** All reads served from one `PipelineSlot` see one generation's world+fields snapshot — the K-L7 atomic-from-observer invariant is preserved *within the slot boundary* (K-L7.1 row :56; VULKAN §7.3.0:1305).
- **A drained batch.** Deferred-flush (row 5) and native Normal drain (row 9) deliver their batch against a stable subscriber snapshot; new deferrals wait for the next generation (EVENT_BUS:49,57).
- **A span lease.** While any field span is acquired, point writes, conductivity/flag updates, and ping-pong swaps are rejected (`active_spans_` atomic counter, FIELDS:144).

**Cross-boundary inconsistency is PERMITTED (by design) between:**

- **Slots.** "Cross-slot reads see different snapshots" — K-L7.1 says so verbatim (:56). Two reads of pipeline-managed state served by different slots may disagree by one generation.
- **Render tail and sim head.** The display composites DisplayTick (SimTick−D) sim state under a current-input intent overlay (K-L17 composition order: SimState first, intent + combat overlays on top, static last — KERNEL_ARCHITECTURE:66). The screen intentionally shows two times at once.
- **K-L7 and K-L7.1 fields in one system.** A sync-path read returns tick-current state (row 8); a pipeline-managed read returns generation-lagged state (row 7). Skew ≤ 1 generation, permitted; systems mixing both must not equate the two timestamps.
- **Sync-path reads and an in-flight dispatch.** A `ReadCell` racing a dispatch may see last-tick or new state; "one-tick stale data is invisible" is the accepted bound (VULKAN §7.3.1:1311-1315).
- **Frames and ticks.** No frame is guaranteed to observe every tick individually (row 11); presentation consumers must tolerate command bursts, including catch-up bursts of up to 5 ticks (§1, catch-up window).

**Forbidden (a bug wherever observed):**

- Torn reads inside a span or slot; observing a partially applied batch; observing an entity between destroy-staging and version bump.
- A deferred handler observing pre-barrier partial writes at FlushGeneration F (the flush runs strictly after the `Parallel.ForEach` barrier, THREADING:41).
- Staleness exceeding the declared bound: >1 generation on a slot tail, >1 SimTick on a `*Snapshot` component, unbounded delay on Normal-tier delivery once the native path carries traffic.

## 4. Pause, save, load boundaries

### 4.1 Pause — two operations, one word

- **Flag pause**: `GameLoop.SetPaused(true)` sets a volatile flag; the in-flight tick completes; the loop then sleeps without advancing (`GameLoop.cs:80,103-107`). Fire-and-forget — the caller gets no acknowledgement that the tick finished.
- **Acknowledged quiesce** (K-L18): `SimulationStateController.PauseAsync → WaitForQuiescenceAsync(timeout)` (KERNEL_ARCHITECTURE:67; THREADING:94). The native predicate is `df_pipeline_is_quiescent` — quiescent ⇔ every slot is `Empty` or `ReadableAsTail`, none `Dispatched`/`FenceCompleted` (`pipeline_slot.h:112-117`; consumed by the mod-unload primitive, `native/DualFrontier.Core.Native/src/mod_unload.cpp:65-67`; KFNS Item 41 :1099-1104).
- Pause converges naturally: sim completes the current tick, dispatches nothing new, pipeline depth absorbs already-dispatched work (VULKAN §7.2.0:1283; KFNS Item 34 :996).
- Mod load/unload requires acknowledged quiesce, never just the flag (K-L18 precondition: `sim_state == Paused && pipeline_slots_quiescent()`).

### 4.2 Save — which SimTick, which queues, no drain

- The saved world is the **DisplayTick state (CurrentSimTick − D)** — the coherent tail the display already sees; **pipeline drain is NOT required at save** (VULKAN §7.2.0:1281; KFNS Item 34 :995 — "pipeline drain не required").
- Tension to reconcile: the K-L16 row's own wording says "Pipeline drain orderly at save/pause" (KERNEL_ARCHITECTURE:65). This draft reads *drain* as the pause-path convergence and *save* as tail-snapshot-without-drain, per the more specific Item 34 / §7.2.0 text; the K-L16 row wording should be aligned at ratification (§7.4).
- A save must also capture the native Background queue: Item 31 save-integrated persist (KFNS :924-937), versioned wire format schema v1 (EVENT_BUS:59); the mod-unload T1 policy already names "background persist" (KFNS Item 32 :947).
- GPU-derived state: per the VULKAN §7.2.1 design mitigation (**not implemented** — no field save path exists), the save serializes canonical CPU-computed field state, not GPU-resident bytes (:1295). Hard sync (`waitIdle`) exists only for save snapshots and shutdown (VULKAN §7.3.1:1317).
- Managed `[Deferred]` queues are normally empty at tick end (flushed every phase), but deferred-from-deferred residue published during a tick's final flush survives into the next tick and has **no persistence path** — flagged §7.6.

### 4.3 Load and resume

- Fields restore from serialized primary buffers; back buffers restore to zero — in-flight ping-pong state is lost by contract (FIELDS:300; the whole Save/load section of FIELDS is **TBD**, :298).
- The pipeline **refills incrementally**: sim starts at the saved SimTick, display unblocks once D slots are populated (KFNS Item 34 :997); resume-from-pause refills from the pause point (:998).
- RNG state is **unspecified today** — the only production RNG is `new Random(42)` at `src/DualFrontier.Systems/Pawn/MovementSystem.cs:35`, whose sequence position silently resets on every process start, so a loaded game does not reproduce the pre-save trajectory (§6 fixes this; flagged §7.7).
- Catch-up after resume: none from the pause itself (paused iterations consume the clock, `GameLoop.cs:101-107`); stall-induced backlog is bounded by the catch-up window to 5 back-to-back steps (`GameLoop.cs:31,110-131`).

## 5. Determinism classes

The corpus asserts three incompatible strengths of "deterministic" without naming them. This section defines the classes; every future claim MUST cite one.

**D0 — best-effort.** No cross-run guarantees.

- Within-phase system execution order is unordered (`Parallel.ForEach`, THREADING:41; MaxDoP = N−2, `ParallelSystemScheduler.cs:90`; admitted as the nondeterminism source by COMBO_RESOLUTION:32,85-87).
- RNG is ad-hoc: `MovementSystem.cs:35` — a single seeded `Random(42)` whose consumption order tracks entity iteration and tick interleaving, and whose position is never persisted. There is no RNG service.
- GPU float results vary across hardware/driver — "floating-point ordering, parallel reduction differences, and driver optimizations" (VULKAN §7.2.1:1289).
- Structural invariants still hold under D0 — e.g. K-L7.1's "one-tick lag bounded и deterministic" is *lag-depth* determinism, not bit determinism.
- **This is the engine's current class, everywhere.**

**D1 — save-reproducible.** Loading a save yields identical observable state: same component values, same field cells, same persisted Background queue, same SimTick.

- This is VULKAN's own requirement — "Save/load must produce reproducible state on load" (§7.2.1:1292).
- D1 constrains *state at the save boundary*, not future evolution: two sessions resumed from one save may diverge.
- Needs (none currently met): canonical CPU field state at save per the §7.2.1 design mitigation (:1295); a seeded RNG service whose state is derivable or serialized (§6); no GPU-derived bytes in the saved state; Item 31 background persist wired; a save path at all (Persistence is orphaned from production today).

**D2 — replay / byte-identical.** Identical initial state + identical input stream ⇒ byte-identical event stream across runs (and across hardware, if claimed).

- This is COMBO_RESOLUTION's promise: "the resulting `DamageEvent` stream is byte-identical between runs" (:73), with a replay suite (:73, TODO Phase 5) and replay-from-command-log consequence (:77).
- Needs: fixed system order or order-independent semantics, deterministic RNG (§6), event-stream capture, bit-stable arithmetic.
- **Not currently scoped**: VULKAN declares "single-player, no replays" (§7.2.1:1291).

### 5.1 Claim assignment

| Claim | Source | Class asserted | Class held today |
|---|---|---|---|
| "byte-identical `DamageEvent` stream between runs" | COMBO_RESOLUTION:73 (LOCKED; self-declared NON-NORMATIVE stub, :17-22) | D2 | D0 (`ComboResolutionSystem` throws `NotImplementedException`) |
| "reproducible combat, correct replay, and stable tests" | FEEDBACK_LOOPS:57 | D2 vocabulary | snapshot rule supports D1/D2; delivers neither alone |
| "Save/load must produce reproducible state on load" | VULKAN §7.2.1:1292 | D1 | unmet — no field save path, RNG unspecified |
| "single-player, no replays"; cross-hardware GPU variance accepted | VULKAN §7.2.1:1289-1291 | caps engine at D1 | consistent with D0 reality |
| "One-tick lag bounded и deterministic" | K-L7.1 row (KERNEL_ARCHITECTURE:56) | structural (lag depth) | holds; orthogonal to D1/D2 |
| "Determinism >> latency" | FEEDBACK_LOOPS:57 | design priority | priority honored; class undefined |
| Replay test "same seed and the same event sequence produce the same result" | COMBO_RESOLUTION:73 (TODO Phase 5) | D2 evidence | absent |

### 5.2 Resolving COMBO vs VULKAN

Two LOCKED documents conflict at the class level: COMBO promises D2 for the damage path; VULKAN denies D2 engine-wide and accepts non-bit-exact GPU state feeding the world. Exactly one of the following must be ratified:

**Option A — downgrade COMBO to D1 scope (recommended default).** Re-word the guarantee to *stable intra-tick ordering*: given one world state and one `DamageIntent` multiset within a tick, the applied order and resulting `DamageEvent` set are identical — independent of thread schedule. This keeps every stated benefit except replay-from-log (:77), which is deleted. Cheap: it is what the sort-key design (:55-58) actually buys.

**Option B — commit to D2 as a bounded determinism island (damage path only).** The engine stays D1; the island `DamageIntent → ComboResolutionSystem → DamageEvent` is D2 under named conditions:

- **C1 — total sort key.** The current `(EntityId, DamageKind ordinal)` key (:55-58) is not total: two same-kind intents on one target tie. Extend with a deterministic tiebreaker (publisher SystemId + per-publisher intent sequence).
- **C2 — no GPU-derived inputs.** Any field value entering damage math must come from a CPU-canonical source (CPU reference kernels, VULKAN §7.1:1274, or the K-L7 sync CPU path) — slot-tail reads are deterministic in *depth* but not bit-stable across hardware.
- **C3 — RNG per §6 only.**
- **C4 — bit-stable arithmetic** inside the island (strict IEEE-754; no FMA-variant intrinsics).
- **C5 — `DamageIntent` stream capture** — which requires amending VULKAN §7.2.1 "no replays" to "no replays outside the damage island".
- **C6 — island inputs restricted to D1-stable state**, so a loaded save re-enters the island identically.

Until one option is ratified, COMBO's determinism guarantee is to be read as **aspirational D2, effective D0** — its own NON-NORMATIVE banner (:17-22) already licenses this reading; FEEDBACK_LOOPS:57 "correct replay" inherits the same downgrade.

## 6. RNG law (proposal)

One rule kills the ad-hoc pattern: **no system constructs its own RNG.** `new Random(42)` at `MovementSystem.cs:35` is the sole production instance today and exhibits every failure at once: fixed seed regardless of world, consumption order coupled to iteration order, position lost on load, stream unshareable and uncapturable.

Proposed service (Contracts surface, injected like other `IGameServices` members; mods get the same surface per K-L9):

1. `IRandomService.Stream(SystemId, SimTick)` returns a small counter-based generator seeded by `hash(WorldSeed, SystemId, SimTick)` (SplitMix64/PCG-class mix).
2. **Keying by SimTick makes stream position stateless**: nothing to serialize but `WorldSeed`. D1 save-reproducibility of RNG comes for free, and no cross-tick position drift can occur.
3. `WorldSeed` is created at new-game and stored in the save header. No config/seed source exists today — a gap in its own right (no config contract anywhere in the repo).
4. Per-(system, tick) streams are independent by construction, so within-phase parallelism cannot interleave draws across systems. *Within* one system's tick, draw order must itself be deterministic — companion rule: iterate entities in index order when drawing.
5. Enforcement: an analyzer rule banning `new System.Random` in `src/DualFrontier.Systems/**` (DFL discipline family, [ANALYZER_RULES](./ANALYZER_RULES.md) §4 registry) plus migration of the one existing call site.

D1 requires exactly this much. Option B of §5.2 additionally requires the damage island to draw only from streams keyed to captured inputs — satisfied by the same `(WorldSeed, SystemId, SimTick)` derivation.

## 7. Open questions and LOCKED-doc conflicts

1. **FIELDS "non-blocking" wording (direct contradiction).** FIELDS:372 — "fence-based GPU sync; field dispatches are non-blocking" — vs VULKAN §2.3.1:549/:600 — the K-L7 sync path "returns after fence signals", i.e. blocks the caller. FIELDS (Live, v0.1.1) predates K-L7.1/K-L16 slot semantics and describes neither path correctly. Named amendment: re-word FIELDS to "K-L7 sync dispatch blocks until the fence signals; K-L7.1 pipeline-managed dispatch is non-blocking with declared slot-tail visibility" (rows 7/8 of §2).
2. **COMBO vs VULKAN determinism class conflict.** LOCKED-vs-LOCKED (D2 promise vs "no replays" D1 cap), §5.2. Named amendment: COMBO Option A wording, or the VULKAN §7.2.1 island carve-out under Option B. FEEDBACK_LOOPS:57 ("correct replay") amends in the same stroke.
3. **Input path absent.** Row 12: input is drained and discarded (`Program.cs:81-84`); no input→sim visibility contract exists, while K-L17 already grants input a *display*-side latency contract (≤16 ms). When the input bridge lands (ROADMAP §Native foundation tracks), its row in §2 must state the boundary. Proposal: enqueue to sim, visible at the next SimTick start, before its first PhaseId.
4. **K-L16 row vs Item 34 save-drain wording.** "Pipeline drain orderly at save/pause" (KERNEL_ARCHITECTURE:65) vs "pipeline drain не required" at save (KFNS Item 34 :995; VULKAN §7.2.0:1281). §4.2 resolves by specificity; the K-L16 row should say "drain at pause; tail snapshot without drain at save".
5. **Dangling phase vocabulary.** FEEDBACK_LOOPS:40 ("Phase 5") and COMBO_RESOLUTION:39,104 ("Phase 4") cite the five-phase scaffold THREADING v2.0.0 removed (:141). The tick-finalization snapshot copy needs a home in the current phase model (managed final PhaseId, or native `Phase_Update` tail) — it is currently specified nowhere.
6. **Deferred-queue residue at save.** Deferred-from-deferred events pending at tick end are unpersisted (managed queues have no save path; Item 31 covers only the native Background tier). Either flush-to-empty becomes a save precondition (drain FlushGenerations until quiescent) or the residue must serialize.
7. **RNG state at load unspecified** (§4.3, §6) — until the RNG law lands, D1 is unreachable in principle, not merely unimplemented.
8. **Native Normal tier has no production driver.** `df_bus_drain_normal_batch` (row 9) is fully specified but never called in production (EVENT_BUS:76-78); when the sovereign switch lands, its drain point must be pinned to a named PhaseId boundary so rows 5 and 9 stay equivalent.
9. **"Slot" and "snapshot" overloading** (§1) — low-cost wording hazard; fold the disambiguation list into THREADING together with the vocabulary.

## See also

- [THREADING](./THREADING.md) — scheduler, phase barriers, deferred flush; fold-in target for §1–§3.
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 — the K-L7/L7.1/L16/L17/L18 invariant rows this contract composes.
- [KERNEL_FULL_NATIVE_SCHEDULER](./historical/KERNEL_FULL_NATIVE_SCHEDULER.md) — Items 31–35, 41 (slots, drain/refill, quiescence).
- [EVENT_BUS](./EVENT_BUS.md), [ECS](./ECS.md), [FEEDBACK_LOOPS](../mechanics/FEEDBACK_LOOPS.md) — per-mechanism visibility sources for §2.
- [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) §2.0, §7 — pipeline depth, save/drain semantics, determinism notes.
- [FIELDS](./FIELDS.md), [COMBO_RESOLUTION](../mechanics/COMBO_RESOLUTION.md) — the two named amendment targets (§7.1, §7.2).