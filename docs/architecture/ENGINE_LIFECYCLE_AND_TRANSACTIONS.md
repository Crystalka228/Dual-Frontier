---
register_id: DOC-A-ENGINE_LIFECYCLE_AND_TRANSACTIONS
project: Dual Frontier
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: 0.1.1
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: post-ratification closure
title: Engine Lifecycle & Transactions — prepare/validate/quiesce/commit/reclaim/recover vocabulary, transition inventory, fault taxonomy (A3+A8 draft)
last_modified_commit: 8960085
review_cadence: on-status-transition
last_review_date: 2026-07-17
last_review_event: 'DRAFTS_RATIFICATION Phase B (C3): HALT-1-ratified retargets ELT-1..ELT-6 — historical/ prefixes + verified offsets for ISOLATION/MOD_PIPELINE/KFNS/MIGRATION_PLAN/OWNERSHIP_TRANSITION cites (R4-11); §9.1/§9.5-resolved re-anchoring (the successor adopted this draft''s §1 law); §9.6 quiescence §-fix; §6 conflicts 1/2/6 re-marked RESOLVED.'
reviewer: Crystalka
special_case_rationale: 'Tier 1 AUTHORED override (forbidden pair; precedent DOC-A-K_CLOSURE_REPORT): authored-proposal draft of the missing A3 (+A8 recovery) cross-cutting contract per ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715 §7. Tier 1 per FRAMEWORK §3.4; AUTHORED because unratified — preamble marks normative-target, NOT current truth; LOCKED docs prevail until ratification per FRAMEWORK §7.'
---

# Engine Lifecycle and Transactions

**The A3 transition contract, plus the fault-and-recovery portion of A8.** One vocabulary for every state transition the engine performs, one law reconciling "atomic" with "best-effort", one fault taxonomy with named owners.

> **Status: AUTHORED PROPOSAL — a normative target, NOT current truth.** Produced by the 2026-07-15 architecture decomposition session (report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md), HEAD `6f39903`). Nothing in this document is binding until ratified per [FRAMEWORK.md §7](../governance/FRAMEWORK.md) (amendment milestone protocol, §7.2). Intended ratification path: the §1 vocabulary and §3 state machines fold into `MOD_OS_ARCHITECTURE.md` §9 as the amendment that resolves the §9 / §9.5.1 contradiction, plus a new MOD_OS shutdown section; the §4 fault taxonomy could fold into `ISOLATION.md`. Until that ratification, wherever this document contradicts a LOCKED document, the LOCKED document wins.

Scope: runtime state transitions of the engine process — mod apply/unload/fault, scheduler rebuild, swapchain recreation, world shutdown, save boundary. Register/governance lifecycle transitions (REGISTER.yaml states per FRAMEWORK.md §3.3) live on a different substrate (git) and are out of scope.

---

## 1. The transition vocabulary — seven stages

Every engine transition is described in terms of the same seven stages. A transition MAY omit stages, but must say which ones and why; it MUST NOT rename them or merge their outcome variables.

| # | Stage | Normative definition |
|---|---|---|
| 1 | **Prepare** | Build the *candidate* state off to the side. Live state is untouched; the candidate is invisible to every observer. Failure here is free: discard the candidate. |
| 2 | **Validate** | Prove the candidate: dependencies resolve, capacity exists, invariants hold. Produces an accept/reject verdict *before* anything live changes. |
| 3 | **Quiesce** | Reach an **acknowledged** boundary: simulation paused *and acknowledged*, pipeline slots quiescent (all slots `Empty` or `ReadableAsTail` — `PipelineSlotInterop.cs:35-51`, MOD_OS_ARCHITECTURE §9.6 (К-L18 quiescence)), no active spans or in-flight batches. A flag set is a *request*; quiescence is the *acknowledgment* (К-L18: "Mod load/unload operations require simulation paused state + pipeline slots quiescent"). Bounded by timeout; timeout aborts the transition before commit. |
| 4 | **Commit** | Single atomic publication of the *desired* state — one reference swap, one flag, one native call inside one critical section. Either the old desired state or the new one is observable; never a mixture. |
| 5 | **Reclaim** | Irreversible, best-effort cleanup of the *old* state (free memory, unload ALCs, destroy handles, drain queues). Reclaim failure NEVER blocks or reverts commit — the desired state is already published. |
| 6 | **Recover** | Response to reclaim failure: bounded retry of the failed reclaim step, or enter **Degraded** with a recorded reason. Never un-commits. |
| 7 | **Resume** | Release the quiescent boundary; simulation continues against the new desired state. |

### 1.1 The key law: atomic commit, best-effort reclaim

Two predecessor LOCKED sentences contradicted each other inside one document — **resolved: MOD_OS_ARCHITECTURE §9.1 (commit/reclaim split) now reconciles them, adopting this draft's §1 law by name**:

- predecessor MOD_OS §9 (quoted inside the resolved MOD_OS_ARCHITECTURE §9.1): "Transitions between states are atomic; failure mid-transition rolls back to the previous state."
- predecessor MOD_OS §9.5.1 (superseded; reconciled at §9.1): "There is no atomic-unload guarantee. `Unload` is conceptually irreversible".

Both sentences are *correct* — about **different stages**:

> **LAW.** "Atomic" applies to the **commit** of the desired state. "Best-effort" applies to the **reclaim** of the old state. They are different stages with **different state variables**: `DesiredState` (changed only by commit, atomically, roll-back-able before commit and never after) and `ReclamationState` (advanced only by reclaim/recover, monotonically, never atomically). Any display of a transition's status — UI, log, diagnostic API — MUST show both variables, e.g. "Disabled (reclaiming…)" / "Disabled (leaked — restart advised)", never a single word that conflates them.

§9's sentence is the commit-stage law; §9.5.1's sentence is the reclaim-stage law. The ratifying amendment rewrites §9's opening to say "state *commits* are atomic; *reclamation* of the previous state is best-effort per §9.5.1" — after which both LOCKED sentences survive verbatim, one per stage. (This amendment landed in the corpus rework: MOD_OS_ARCHITECTURE §9.1 carries exactly this split, citing this draft.)

Corollaries:

1. Reclaim MUST NOT begin before commit succeeds. (A transition that destroys old state before publishing new state has no rollback — see §2.5 swapchain, today's counter-example.)
2. Quiesce failure aborts the transition *entirely* — no stage after quiesce may run "best-effort anyway" (today's counter-example: MOD_OS_ARCHITECTURE §9.4 (unload chain — steps 3.6 onward proceed best-effort after a step-3.5 quiescence rejection); see §6 conflict 5).
3. Genuine prepare/commit already exists in the codebase and is the model: MOD_OS_ARCHITECTURE §8.3 (graph-rebuild atomicity — the commit guarantee: "`DependencyGraph.Reset()` + `Build()` are performed on a **local variable**. The scheduler is replaced only after `Build()` succeeds. On error, the old scheduler stays active.") and `historical/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` §(atomic commit set / revert-as-unit) (delete lands "in the same milestone's atomic commit set"; "relocation commits and delete commits revert as a unit"; "Buildable at any commit"). This contract generalizes what those two places already do.

### 1.2 Stage ordering and omission rules

- **Order is fixed:** prepare → validate → quiesce → commit → reclaim → recover → resume. No stage begins until its predecessor's outcome is known: commit does not start until quiesce is *acknowledged*; reclaim does not start until commit has returned.
- **Prepare and validate run outside the boundary.** They touch only candidate state, so they SHOULD execute *before* quiesce, keeping the paused window as short as possible. (Mod Apply already does this: the graph is built and proven on a local variable while only a menu flag is held.)
- **Omission must be declared.** A read-only transition (save, §2.7) uses quiesce → observe → resume and omits the rest. A destruction-only transition (unload, shutdown) has vacuous prepare/validate. But **no transition may omit commit**: even pure teardown publishes a desired state first (mod leaves the active set; session enters `ShuttingDown`) — that publication is what the reclamation axis hangs off.
- **Quiesce is held minimally:** across commit and only those reclaim steps that touch shared live state (e.g. the native T0-T7 primitive). Long-running reclamation (the 10 s ALC pump) may outlive the boundary — its outcome lands in the reclamation state variable, not in the commit.
- **Recover never re-enters commit.** Its only moves are: retry a reclaim step (bounded), or record `ReclaimFailed` and raise a §4 class-4 Degraded reason.

---

## 2. Transition inventory

| Transition | Stages present today | Missing stages | Today's contradictions |
|---|---|---|---|
| Mod Apply (menu) | prepare, validate, quiesce (crude flag), commit, reclaim (departing mods), resume | acknowledged quiesce; recover | none — the conformant example (§2.1) |
| Mod unload / disable | quiesce (К-L18 at step 3.5), commit (fused into chain), reclaim | prepare/validate (n/a); recover; *separated* commit vs reclaim state variables | §9 vs §9.5.1 "atomic" vs "irreversible" (§1.1); chain continues after quiesce rejection (§6-5) |
| Mod fault handling | commit-ish (queue for unload), reclaim (deferred) | quarantine commit at fault time; recover | specified **three different ways** (§2.3); none implemented — scheduler has no catch |
| Scheduler graph rebuild | prepare, validate, commit | acknowledged quiesce (three unfused pause booleans) | none doc-side; enforcement is a flag check |
| Swapchain recreation | quiesce (WaitIdle), reclaim, prepare | commit as swap; reclaim ordered *after* commit; recover | shipped with **no documented protocol**; reclaims old state before building new (§2.5) |
| World shutdown | none — ad-hoc | all seven | **no transaction at all**: abandoned thread, leaked native world (§2.6) |
| Save snapshot | quiesce (К-L16 drain) | everything else | deferred to PERSISTENCE_SNAPSHOT_CONTRACT.md draft (§2.7) |
| Register/governance transitions | — | — | out of scope (git substrate) |

### 2.1 Mod Apply — the conformant transition

`ModIntegrationPipeline.Apply` (`src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:209`) already implements the vocabulary. Mapping its five steps (MOD_OS_ARCHITECTURE §8.1 (the Apply pipeline); predecessor record at `historical/MOD_PIPELINE.md`):

| MOD_PIPELINE step | Stage |
|---|---|
| [1] `ModLoader` reads manifest, creates collectible ALC | prepare |
| [2] `ContractValidator.Validate` (version + write-write) | validate |
| [3] `IMod.Initialize(RestrictedModApi)` — registration | prepare (see caveat) |
| [4] `DependencyGraph.Reset()+AddSystems()+Build()` on a **local** graph | prepare + final validate (`Build` proves acyclicity) |
| [5] `ParallelSystemScheduler.Rebuild(phases)` | **commit** — phases, metadata and context cache swap together (`src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:191-222`) |

Quiesce is the `_isRunning` guard — `Apply` throws `InvalidOperationException("Pause the scheduler before applying mods")` (`ModIntegrationPipeline.cs:213-214`, MOD_OS_ARCHITECTURE §9.3 (pause-before-apply)). Reclaim is `ALC.Unload()` on every departing mod (MOD_OS_ARCHITECTURE §9.2) plus the faulted-set drain (`ModIntegrationPipeline.cs:216-229`). Resume is `ModIntegrationPipeline.Resume()`. Per-mod partial success is first-class: `PipelineResult(..., LoadedModIds, FailedModIds)` (MOD_OS_ARCHITECTURE §8.4 (pipeline result)). The commit guarantee is stated (MOD_OS_ARCHITECTURE §8.3: "the graph is always consistent — either the old working one, or the new fully built one") and tested (`Pipeline_build_failure_leaves_old_scheduler_intact`, `historical/MOD_PIPELINE.md` §tests).

Caveat (residual): step [3] mutates the *live* `ModRegistry` before commit; rollback relies on `ResetModSystems`/`RemoveMod` compensation rather than a candidate registry. Acceptable today; a candidate-scoped registry is the eventual clean shape. Quiesce is a flag, not an acknowledged boundary — Apply should adopt the К-L18 helper (`PauseAsync` → `WaitForQuiescenceAsync(timeout)` → op → `ResumeAsync`, К-L18; `src/DualFrontier.Application/Loop/SimulationStateController.cs:71,110,82`).

### 2.2 Mod unload / disable — commit vs reclamation, separated

Today the unload chain (MOD_OS_ARCHITECTURE §9.4 (the canonical chain); the predecessor's variant step order at `historical/MOD_PIPELINE.md`) fuses commit and reclaim into one linear sequence, and the successor keeps the fusion's consequence: "the mod is removed from the active set regardless of whether the assembly actually unloaded" (MOD_OS_ARCHITECTURE §9.4). That sentence *is* the desired-state commit — it just has no name.

Under this contract, unload is two machines:

- **Desired-state commit (atomic):** mod leaves the active set; graph rebuilt without it; scheduler swaps. Mod's desired state: `Active → LogicallyDisabled`. This either happens or it doesn't.
- **Reclamation (best-effort, monotonic):** the chain steps 1-3.6 (unsubscribe, revoke contracts, remove systems, native T0-T7 primitive, V resource cleanup), then `ALC.Unload()` + the WeakReference GC pump (100 × 100 ms = 10 s, MOD_OS_ARCHITECTURE §9.4 (reclamation pump); `ModIntegrationPipeline.cs:581,931`). Reclamation state: `LogicallyDisabled → Reclaiming → Reclaimed | ReclaimFailed(leaked)`.

`ReclaimFailed(leaked)` is the named terminal for today's "the mod is marked as a leaked reference and the user is advised to restart" (MOD_OS_ARCHITECTURE §9.4). Recover stage today: none (no pump retry, no Degraded); §3.1 adds it. The native side already obeys the commit law: `df_scheduler_unload_mod_native_state` runs T0-T7 inside one critical section (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 32 (T0-T7 single critical section)) and "returns error code if violated (does not perform partial teardown)" (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 41; live restatement at MOD_OS_ARCHITECTURE §9.4 step 3.5).

### 2.3 Mod fault handling — resolving the three-way timing conflict

The same event — "a mod system throws at runtime" — was specified three incompatible ways by the predecessor corpus. **Resolved: MOD_OS_ARCHITECTURE §9.5 (single code-verified fault path, was session finding N-8)** — code queues at fault time and runs the chain at next `Apply`; the MOD_PIPELINE and ISOLATION variants are retired to `historical/`:

1. **predecessor MOD_OS §10.1** (survives as the code-verified path, MOD_OS_ARCHITECTURE §9.5/§10.3): "the mod is queued and unloaded via the §9.5 chain **at the next menu open** (`ModIntegrationPipeline.Apply` drains the faulted set)".
2. **`historical/MOD_PIPELINE.md` §ModFaultHandler** (retired): the chain runs **now**, but "Step [7] (DependencyGraph rebuild) is deferred… the mod's systems are marked `Disabled` in the scheduler, and the graph is rebuilt the next time the mod menu is opened."
3. **`historical/ISOLATION.md` §"ModFaultHandler — lifecycle on a mod fault"** (retired): an **immediate** "strict sequence" of all six steps — log, unsubscribe, evict from scheduler, `IMod.Unload` with timeout ("the call is aborted when the timeout elapses"), ALC unload, publish `ModDisabledEvent`.

Code truth (now also the LOCKED text — MOD_OS_ARCHITECTURE §9.5 documents exactly this): the queue-at-fault half of reading 1 without the detection half. `ModFaultHandler.ReportFault` only adds the modId to a set (`src/DualFrontier.Application/Modding/ModFaultHandler.cs:65-72`); the set is drained by `Apply` at the next menu open (`ModIntegrationPipeline.cs:216-229`) — closest to reading 1. But `ParallelSystemScheduler.ExecutePhase` has **no catch at all** (`src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:149-164`; exceptions "propagate through `Parallel.ForEach` as an `AggregateException`", `:31`), `GameLoop.RunLoop` has no catch either (`src/DualFrontier.Application/Loop/GameLoop.cs:94-139`), and the only production `ReportFault` route is `ModLoader.HandleModFault` (`src/DualFrontier.Application/Modding/ModLoader.cs:303-309`) — which no runtime path invokes. A faulting mod today kills the `SimulationLoop` background thread, mod and core alike.

**Proposed resolution** (stage-split, like §2.2):

- **Fault → immediate quarantine = commit.** At the tick boundary following the fault, the mod's systems are evicted from the schedule and its desired state commits `Active → LogicallyDisabled(faulted)`. This is cheap (skip-set consulted by dispatch; no graph rebuild mid-tick — the rationale in `ModFaultHandler.cs:29-37` stands) and atomic. Requires the currently missing per-system catch with origin dispatch (core → rethrow, mod → quarantine) in `ExecutePhase`.
- **Queued reclamation at the next menu open = reclaim.** The §9.5 chain (including `IMod.Unload` and the ALC pump) runs when `Apply` drains the faulted set under the К-L18 boundary — exactly today's `ModIntegrationPipeline.cs:216-229` behavior.

**Which doc text must change when the quarantine-commit stage ships** (the predecessor variants are already retired to `historical/` — never edited there; the surviving live target is MOD_OS_ARCHITECTURE):

- the six-step immediate sequence (`historical/ISOLATION.md` §"ModFaultHandler — lifecycle on a mod fault") stays retired; its steps 1-3 (log, unsubscribe, evict) become the immediate quarantine, steps 4-6 the deferred reclamation. Step 4's "the call is aborted when the timeout elapses" is rewritten to "bounded wait, then proceed and mark `ReclaimFailed`" — .NET offers no safe mid-call abort, and the swallowed-`try/catch` discipline (MOD_OS_ARCHITECTURE §9.4) is the real behavior.
- MOD_OS_ARCHITECTURE §10.3 (two response modes): "queued and unloaded… at the next menu open" → "quarantined immediately (desired-state commit); reclaimed via the §9.4 chain at the next menu open."
- the `historical/MOD_PIPELINE.md` "systems are marked `Disabled` in the scheduler" claim stays retired — today no such marking exists; the quarantine mechanism above is what would make it true.

### 2.4 Scheduler graph rebuild

The shared sub-transition of Apply, unload, and fault-drain. Prepare/validate/commit are exemplary (§2.1; MOD_OS_ARCHITECTURE §8.3 (graph-rebuild atomicity)). Gap: quiesce. Three pause surfaces exist and are not fused — `ModIntegrationPipeline._isRunning` (menu flag), `GameLoop._paused` (`GameLoop.cs:45,80`, a volatile bool the sim thread checks between ticks, set without waiting for the in-flight tick), and the К-L18 `SimulationStateController` sim-paused flag + slot poll. Only the third is an *acknowledged* boundary. Contract: rebuild's quiesce = К-L18 helper; the boolean flags become implementation details behind it.

### 2.5 Swapchain recreation — shipped, protocol unstated

Shipped code: on `outOfDate` acquire/present, `LauncherRenderer` calls `VulkanDevice.WaitIdle()` → `Swapchain.Recreate(w,h)` → `Runtime.RecreateFramebuffersForSwapchain()` (`src/DualFrontier.Launcher/LauncherRenderer.cs:125-139,181-196`). The only protocol statement is a doc comment: "Caller must invoke after `Swapchain.Recreate` + `VulkanDevice.WaitIdle`" (`src/DualFrontier.Runtime/Runtime.cs:188-192`). No architecture document specifies the transaction; VULKAN_SUBSTRATE is silent.

Today's stage order is quiesce → **reclaim → prepare** → implicit commit: `RecreateFramebuffersForSwapchain` disposes all old framebuffers, clears the list, *then* constructs new ones (`Runtime.cs:193-206`). A constructor failure mid-loop leaves zero framebuffers — old state already destroyed, new state absent, no rollback. This violates §1.1 corollary 1.

Proposed protocol: **prepare** (build new swapchain + image views + framebuffers alongside, where the surface API permits; retire into a candidate list) → **quiesce** (fence-based: wait per-frame fences or `WaitIdle` — the acknowledged GPU boundary) → **commit** (swap the framebuffer list and swapchain reference in one assignment) → **reclaim** (destroy old views/framebuffers/swapchain, best-effort with `ReclaimFailed` logging) → **resume**. Vulkan's `oldSwapchain` parameter exists precisely for prepare-before-reclaim.

### 2.6 World shutdown — no transaction at all

Today (`src/DualFrontier.Launcher/Program.cs:95-97`): window closes → `gameContext.Loop.Stop()` → `renderer.Shutdown()` → return. `GameLoop.Stop` is `_cts.Cancel(); _thread?.Join(2000);` with the `Join` result **ignored** (`GameLoop.cs:73-77`) — on timeout the background sim thread (`IsBackground = true`, `GameLoop.cs:64-69`) is abandoned, possibly inside `ExecuteTick`, racing teardown. Nothing calls `ModIntegrationPipeline.UnloadAll` (defined `ModIntegrationPipeline.cs:780`; **zero production call sites** — only tests). Nothing disposes the `NativeWorld` — it is created inside `GameBootstrap` (`src/DualFrontier.Application/Loop/GameBootstrap.cs:76`), never surfaced on `GameContext` (no `Dispose` exists there), and "Disposal is mandatory. Dropping a `NativeWorld` without calling `Dispose` leaks the underlying C++ world" (`src/DualFrontier.Core.Interop/NativeWorld.cs:25-26`) — there is no finalizer backstop. No native bus/scheduler teardown runs either.

Proposed shutdown transaction (full seven-stage; prepare/validate vacuous):

1. **Commit** — session state `Running/Paused → ShuttingDown` (one-way; §3.2). Input and new transitions refused from here.
2. **Quiesce (bounded, acknowledged)** — cancel the loop token, `Join` the sim thread with a deadline, and **check the result**. On acknowledgment, proceed. On timeout: the thread is wedged inside a tick — this is a class-1 fault (§4); fail fast with diagnostics rather than tear down state under a live thread. Never abandon silently.
3. **Reclaim (ordered, best-effort)** — mods (`UnloadAll`, giving the method its first production caller), then managed↔native bridges (bus drain, GC handles), then `NativeWorld.Dispose()`, then renderer/runtime (`WaitIdle` before Vulkan handle destruction), then window.
4. **Recover** — a failed reclaim step logs `(step, reason)` and continues; every wait is bounded. The process is terminating; Degraded is meaningless here, hangs are the only failure that matters.
5. **Terminal commit** — session state `ShuttingDown → Terminated`; process exit code reflects reclaim outcome (0 clean, nonzero with count of failed steps).

### 2.7 Save snapshot

A save is a *read* transaction: **quiesce** (К-L16: "Pipeline drain orderly at save/pause") → snapshot at the acknowledged boundary → **resume**. No commit/reclaim of engine state. Snapshot content, `(modId, modVersion)` recording (MOD_OS_ARCHITECTURE §9.8 (save-game implications)), and versioning are deferred to the companion draft [PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (A7); this document only fixes that a save boundary IS a quiesce in the §1 sense.

---

## 3. State machines

### 3.1 Mod lifecycle — desired-state axis + reclamation axis

The §9.1 states are preserved exactly (MOD_OS_ARCHITECTURE §9.1 (mod lifecycle states)): `Disabled → Pending → Loaded → Active → Stopping → Disabled`. They become the **desired-state axis**, whose transitions are commits — atomic, per §9.1's commit law, now true by construction. A second, orthogonal **reclamation axis** carries what the §9.4 chain (predecessor §9.5/§9.5.1) describes:

```
Desired axis   (atomic commits):   Disabled → Pending → Loaded → Active → Stopping → Disabled
                                                                    │ fault (quarantine commit)
                                                                    ▼
                                                          LogicallyDisabled(faulted)

Reclamation axis (monotonic, best-effort), entered on any leave-Active commit:

        ┌───────────────┐   §9.4 steps 1-3.6 + ALC pump   ┌───────────┐
        │ NotReclaiming │ ───────► Reclaiming ───────────► │ Reclaimed │
        └───────────────┘              │                   └───────────┘
                                       │ pump timeout / step failure exhausts recover
                                       ▼
                              ReclaimFailed(leaked)  → engine session gains Degraded(reason)
```

Rules: the desired axis never waits for the reclamation axis ("removed from the active set regardless", MOD_OS_ARCHITECTURE §9.4). `Recover` may retry the ALC pump a bounded number of times before `ReclaimFailed`. A mod may not re-enter `Pending` (re-enable) while its reclamation state is `Reclaiming`; re-enable from `ReclaimFailed` is refused with "restart required" (same user surface as today's `ModUnloadTimeout` advice, MOD_OS_ARCHITECTURE §9.4). **Degraded** is not a sixth desired state — it is the session-level annotation produced by `ReclaimFailed` (and by §4 class 4 generally). Today no Degraded representation exists in code at all (zero hits for `degraded` in `src/`; state is scattered boolean flags such as `GameLoop._paused`); §4 proposes making it real.

### 3.2 Engine session lifecycle

Today this machine is implicit in `Program.Main` + `GameBootstrap.CreateLoop` — no type represents it. Proposed:

```
  Boot ──ok──► Running ◄──────► Paused ──shutdown request──► ShuttingDown ──► Terminated
   │                 (pause/resume)  │                            ▲
   └─boot failure──► fail-fast       └───shutdown request─────────┘ (from Running too)
```

| Transition | Trigger | Notes |
|---|---|---|
| `Boot → Running` | bootstrap + `Loop.Start()` succeed | boot failure = fail-fast with diagnostic (К-L19 posture) |
| `Running → Paused` | user/menu via К-L18 helper | *acknowledged*: `PauseAsync` + `WaitForQuiescenceAsync(timeout)`; a set flag alone is `PauseRequested`, not `Paused` |
| `Paused → Running` | `ResumeAsync` | resume stage of any enclosing transition |
| `Running/Paused → ShuttingDown` | window close / quit / fatal fault escalation | one-way; refuses new transitions |
| `ShuttingDown → Terminated` | §2.6 reclaim sequence completes (or bounded-timeout fail-fast) | never reached today — the process just returns |

Mod transitions (§2.1-§2.3 reclaim phase) are legal **only** in `Paused` (acknowledged) — this is К-L18 restated at session level. `Degraded` is an orthogonal annotation (`Running + Degraded{reasons}`), not a state — the machine stays five-state.

---

## 4. Fault taxonomy and recovery owners

| # | Class | Example | Decision owner | Response | User-visible effect |
|---|---|---|---|---|---|
| 1 | Contract violation (a bug) | core system throws; illegal lifecycle transition; teardown under a live thread | origin dispatch (MOD_OS_ARCHITECTURE §10 (fault origin split); predecessor table at `historical/ISOLATION.md`) | core origin → **crash** (fail-fast); mod origin → class 3 | crash report / mod banner |
| 2 | Transient resource failure | swapchain out-of-date; file lock; allocation hiccup | owning subsystem | declared bounded **retry policy** (count + backoff), then escalate to class 4; out-of-date swapchain retries via §2.5 protocol | none if retry wins; Degraded reason otherwise |
| 3 | Mod fault | unhandled exception from a mod ALC (MOD_OS_ARCHITECTURE §10); isolation breach report | `ModFaultHandler` | **quarantine (commit) + queued reclaim** per §2.3 | banner (MOD_OS_ARCHITECTURE §10.3; predecessor form at `historical/ISOLATION.md`); disabled on next start |
| 4 | Subsystem degradation | `ReclaimFailed(leaked)` ALC; background drain starved; retry budget exhausted | engine session | enter **Degraded(reason)** — proposed as a real, queryable session annotation (today: nothing) | persistent status indicator; restart advice |
| 5 | Process-fatal kernel corruption | native invariant breach — cycle despite Kahn check, RT-class quota violation (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` §panic path) | native kernel | **fail-fast** — no best-effort continuation over corrupt state | crash with native diagnostic |
| 6 | Device-lost / platform | `VK_ERROR_DEVICE_LOST` (defined `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs:14`; **zero handlers repo-wide**) | **TODAY UNSPECIFIED — open (§6 OQ-3)** | proposed v1: fail-fast with diagnostic, consistent with К-L19 startup posture; device re-creation deferred | crash with diagnostic (v1) |
| 7 | Quota exceedance | `[CpuQuota]` budget exceeded (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 7 (CpuQuota)) | native scheduler → configured handler | mod system → class 3 ("ModFaultHandler invoked, system unloaded per existing pipeline"); core system → logged/throttled = a class-4 Degraded reason, never silent | mod banner / Degraded indicator |

> **LAW.** Every `#if DEBUG`-throw / RELEASE-silent fork MUST be justified against this table, per class. Silent RELEASE handling is legal only for a class whose row already names an owner, a counter, and a user-visible surface. Counter-example to eliminate: `historical/OWNERSHIP_TRANSITION.md` §"Transition table" closing rule — "attempting one throws `InvalidOwnershipTransitionException` in DEBUG and is silently ignored in RELEASE (with an error counter increment for diagnostics)". (Content relocated: the doc is superseded by DOC-J-GOLEM_OWNERSHIP; the pattern's canonical statement is preserved in the historical file.) An illegal transition is class 1 (a bug); class-1 faults get the same response in both configurations — a bug does not become acceptable because the build is RELEASE.

### 4.1 Making Degraded real

Classes 2, 4 and 7 all terminate in "enter Degraded" — a state that exists nowhere in code. Today's substitutes are scattered: volatile booleans (`GameLoop._paused`, `ModIntegrationPipeline._isRunning`), warning lists returned from unload calls, and a log line. Proposal:

- A session-scoped `EngineHealth` value owned by the §3.2 session machine: `Normal | Degraded(reasons)`, where each reason is structured — `(source, faultClass, tickId, advice)`.
- Entered only through the recover stage or a §4 class-2/4/7 escalation; **exited only by the action named in the advice** (usually restart; a successful retry removes its own reason).
- Queryable by UI and diagnostics; every entry/exit emits a §5 lifecycle event. A `Degraded` session refuses transitions whose safety assumptions the reason invalidates (e.g. `ReclaimFailed(leaked)` refuses re-enabling that mod, §3.1).

This is deliberately *not* a sixth mod state or a sixth session state — it is an annotation, so the LOCKED five-state diagrams survive ratification untouched.

---

## 5. Enforcement and observability

**Lifecycle event stream.** Every transition emits structured events: `(TickId, transition, stage, outcome)` — one event at stage entry, one at stage exit; `outcome ∈ {ok, failed(reason), timeout}`. `TickId` comes from the tick counter already threaded through the loop (`GameLoop.cs:116`). Reclamation MUST emit a terminal event (`Reclaimed` or `ReclaimFailed`) — the 10-second ALC pump today ends in a warning string (`ModIntegrationPipeline.cs:931`); it becomes a first-class event. Destination: diagnostics log now; the native observability surface (KFNS Item 19) later.

**Test law.** Each transition ships three tests: (a) *commit atomicity* — induced failure before/at commit leaves old state observable (existing model: `Pipeline_build_failure_leaves_old_scheduler_intact`, `historical/MOD_PIPELINE.md` §tests); (b) *reclaim failure isolation* — injected reclaim-step failure does not disturb the committed state (existing partial model: `tests/DualFrontier.Modding.Tests/Pipeline/M72UnloadChainTests.cs`); (c) *quiesce refusal* — the operation is rejected when the boundary is not acknowledged (existing model: `M71PauseResumeTests.cs`).

Coverage today, per transition:

| Transition | (a) commit atomicity | (b) reclaim-failure isolation | (c) quiesce refusal |
|---|---|---|---|
| Mod Apply | **exists** (`Pipeline_build_failure_leaves_old_scheduler_intact`) | partial (fault-drain path untested) | **exists** (`M71PauseResumeTests.cs`; throw at `ModIntegrationPipeline.cs:213`) |
| Mod unload | **exists** (`Pipeline_unload_removes_mod_systems_from_scheduler`) | **exists** (`M72UnloadChainTests.cs` step-throw seams) | **exists** (throw at `ModIntegrationPipeline.cs:566`) |
| Mod fault | missing — no catch seam exists to test | missing | n/a (fault arrives mid-tick by definition; quarantine commits at the boundary) |
| Graph rebuild | **exists** (shared with Apply; `ParallelSystemScheduler.Phases` exposed for it, `ParallelSystemScheduler.cs:224-229`) | n/a | flag-check only |
| Swapchain recreation | missing | missing | missing |
| World shutdown | missing | missing | missing |
| Save snapshot | deferred with A7 | — | — |

**Today's gaps**, in test-law terms:

- `UnloadAll` has zero production call sites (`ModIntegrationPipeline.cs:780`; only tests invoke it) — shutdown never exercises mod reclamation at all.
- No shutdown tests exist: nothing asserts `Join` success, `NativeWorld` disposal, or teardown ordering (§2.6). The world-shutdown transition currently has zero of its three required tests.
- No per-system catch exists in `ExecutePhase` (`ParallelSystemScheduler.cs:149-164`), so the ISOLATION origin-dispatch table has no enforcement point and no testable seam — the fault transition fails test (a) trivially.
- Swapchain recreation has no failure-injection test; a framebuffer-constructor throw mid-recreate is unobserved (§2.5).

---

## 6. Open questions and LOCKED-doc conflicts

**Open questions:**

- **OQ-1** Quarantine granularity: evict a faulted mod's systems at the next tick boundary (proposed) or the next phase barrier (needs scheduler skip-set support mid-tick)?
- **OQ-2** Does `ReclaimFailed(leaked)` force session-level `Degraded`, or remain per-mod with only the restart advisory? (Proposed: session-level, since a leaked ALC affects the whole process.)
- **OQ-3** Device-lost: is fail-fast v1 (§4 class 6) acceptable to ratify, with device re-creation as a separate epic?
- **OQ-4** Retry policies for class 2: per-subsystem constants or a config surface? (No config layer exists anywhere today.)
- **OQ-5** Shutdown quiesce timeout: fail-fast in both configurations (proposed — never abandon a live sim thread silently), or DEBUG-fail-fast / RELEASE-log-and-exit-nonzero? Must be justified under the §4 law either way.
- **OQ-6** Where do lifecycle events live long-term — managed log only, or the native observability API (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 19)?
- **OQ-7** Should Apply's step [3] (`IMod.Initialize`, §2.1 caveat) move to a candidate-scoped `ModRegistry` so prepare never touches live state, or is compensation-based rollback (`ResetModSystems`) acceptable permanently?

**Explicit LOCKED-doc conflicts this contract resolves (or exposes):**

1. **RESOLVED in-corpus.** The predecessor's "atomic / rolls back" vs "no atomic-unload guarantee" contradiction is reconciled at MOD_OS_ARCHITECTURE §9.1 (commit/reclaim split), which adopts this draft's §1.1 law by name: atomic = commit, best-effort = reclaim.
2. **RESOLVED in-corpus (was session finding N-8).** Mod fault-unload timing is canonicalized at MOD_OS_ARCHITECTURE §9.5 (queue at fault, chain at next `Apply`); the MOD_PIPELINE and ISOLATION variants are retired to `historical/`. The remaining §2.3 proposal (quarantine-now commit) is a forward tightening, and the detection gap stays open — MOD_OS §9.5 fences to this draft.
3. `historical/ISOLATION.md` step-4 abort-on-timeout ("the call is aborted when the timeout elapses") vs the swallowed-`try/catch` best-effort discipline (MOD_OS_ARCHITECTURE §9.4) — no safe abort primitive exists; the quarantine amendment rewrites it to bounded-wait-then-`ReclaimFailed`. **(Open — the abort wording survives only in the historical record; the live §9.4 discipline governs.)**
4. MOD_OS_ARCHITECTURE §9.1 has no fault state and no Degraded — the fault path of §9.5/§10.3 is unrepresentable in the lifecycle diagram of the same document. Resolved by the §3.1 quarantine commit + reclamation axis **(open until that amendment lands)**.
5. `historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 41: the native primitive "returns error code if violated (does not perform partial teardown)" vs MOD_OS_ARCHITECTURE §9.4: after step 3.5 rejects for quiescence violation, steps 3.6 onward proceed best-effort — the managed chain keeps reclaiming after the native side refused the transition. Under §1.1 corollary 2, a quiesce rejection aborts the whole transition before any reclaim step runs; §9.4's sentence must change when that corollary lands (MOD_OS §9.6 fences to this draft). **(Open.)**
6. **RESOLVED in-corpus (was session finding N-9).** MOD_OS_ARCHITECTURE §9.4 canonicalizes one ordered, code-verified chain; `ModDisabledEvent` was proven nonexistent; the MOD_PIPELINE 8-step variant is retired to `historical/`.

Until each item above is ratified per the preamble, the LOCKED texts stand as written and this document yields.