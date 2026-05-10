# K6 — Second-graph rebuild on mod change (Full Brief)

**Status**: AUTHORED — closure-shaped implementation brief, executes against existing M7-era code with explicit gap identification
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K6
**Specification source**: `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.6 §9 (lifecycle), §9.5 (ALC unload protocol), §9.5.1 (failure semantics), §11.1 (migration phases M7.1–M7.3)
**Companion**: `docs/MIGRATION_PROGRESS.md` (live tracker — K6 row promotes from NOT STARTED → DONE on closure)
**Methodology lineage**: `tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md` (Anthropic `Edit` literal-mode semantics, atomic commit discipline, read-first/brief-second/execute-third pivot)

---

## Executable contract notice

This brief is a deterministic instruction set for a Claude Code execution session. The K6 milestone has a unique shape compared to K1–K5/K9: most deliverables already exist on disk via the parallel MOD_OS migration M7 work track (M7.1 pause/resume, M7.2 unload chain, M7.3 step 7). The brief describes the **complete K6 contract** as if starting from zero; the executor performs a verification pass at Phase 1 and skips already-fulfilled deliverables, executing only the genuine gaps identified in Phase 0.4 inventory.

The executor reads this entire brief before any tool call. Anthropic `Edit` tool semantics assumed (literal string matching, not regex — per `MOD_OS_V16_AMENDMENT_CLOSURE.md` lessons learned). When the executor encounters an underspecified situation, the stop condition is "halt and escalate", not "improvise".

Time estimate: **3-5 days at hobby pace (~1h/day)** if all deliverables required new code. **1-2 days at hobby pace** for the as-found state where most code exists and gaps are localized to: (a) drift reconciliation between KERNEL §K6 wording and reality, (b) `HandleModFault` TODO closure, (c) verification test additions. Auto-mode estimate: **3-6 hours wall time** for the as-found path.

Scope follows "no compromises" — no shortcuts in drift reconciliation, no skipped verification, no postponed test gaps. `HandleModFault` gets a real implementation, not a `throw new NotImplementedException` retention.

---

## Phase 0 — Pre-flight verification

Before any edit, the executor verifies the working tree state, prerequisite milestones, and the assumptions this brief makes about the code state.

### 0.1 — Working tree clean

```
git status
```

**Expected output**: `nothing to commit, working tree clean` on branch `main` or `feat/k6-mod-rebuild`.

**Halt condition**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K6-WIP"` and re-verify, or commit them on the current branch before starting K6 work.

### 0.2 — Prerequisite milestones closed

```
git log --oneline -50
```

**Expected**: K0–K5 closure commits visible (`89a4b24`, `e2c50b8`, `129a0a0`, `7629f57`, `2fc59d1`, `547c919`).

Additional expected: M3 closure, M4 closure, M5.1, M5.2, M6.x, M7.1, M7.2, M7.3 closures all visible (commits SHA TBD per local log; the executor records them in Phase 5 when updating MIGRATION_PROGRESS.md).

**Halt condition**: K0–K5 not all closed. K6 sits in the K-series chain after K5; without K0–K5, the working state contradicts the brief's foundational assumptions.

**Soft halt** (advisory, not blocking): if M7.1–M7.3 are not closed, K6 needs full implementation rather than verification. The executor records this in Phase 0.4 inventory and reverts to from-scratch implementation per the K6 deliverable list in §K6 of `KERNEL_ARCHITECTURE.md`.

### 0.3 — Prerequisite documents at expected versions

```
head -10 docs/architecture/KERNEL_ARCHITECTURE.md
head -10 docs/architecture/MOD_OS_ARCHITECTURE.md
head -3  docs/MIGRATION_PROGRESS.md
```

**Expected**:

- `KERNEL_ARCHITECTURE.md` Status: AUTHORITATIVE LOCKED v1.0
- `MOD_OS_ARCHITECTURE.md` Status: LOCKED v1.6
- `MIGRATION_PROGRESS.md` Last updated: post-K5 (date TBD, K6 row should still read NOT STARTED at brief execution start)

**Halt condition**: any spec at unexpected version. K6 implements against these specs verbatim; version mismatch means the spec contract has shifted under the brief.

### 0.4 — Code state inventory (gap detection)

The executor performs the following verification reads to determine which K6 deliverables are already fulfilled and which require new implementation.

**Inventory table** — fill each row by reading the named file:

| K6 deliverable (per `KERNEL_ARCHITECTURE.md` §K6) | Expected file | Expected method | Verify presence |
|---|---|---|---|
| Pause/resume primitives | `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` | `Pause()`, `Resume()`, `IsRunning` | `grep -n "public void Pause\|public void Resume\|public bool IsRunning" <file>` |
| Apply (mod change → graph rebuild) | same | `Apply(IReadOnlyList<string>)` | `grep -n "public PipelineResult Apply" <file>` |
| Unload single mod (full §9.5 chain) | same | `UnloadMod(string)` | `grep -n "public IReadOnlyList<ValidationWarning> UnloadMod" <file>` |
| Bulk unload | same | `UnloadAll()` | `grep -n "public IReadOnlyList<ValidationWarning> UnloadAll" <file>` |
| ALC unload + WeakReference spin | same | `TryStep7AlcVerification`, `CaptureAlcWeakReference` | `grep -n "TryStep7AlcVerification\|CaptureAlcWeakReference" <file>` |
| Per-mod registry cleanup | `src/DualFrontier.Application/Modding/ModRegistry.cs` | `RemoveMod(string)` | `grep -n "public void RemoveMod" <file>` |
| ModLoader unload primitive | `src/DualFrontier.Application/Modding/ModLoader.cs` | `UnloadMod(string)` | `grep -n "public void UnloadMod" <file>` |
| Bus subscription cleanup | `src/DualFrontier.Application/Modding/RestrictedModApi.cs` | `UnsubscribeAll()` | `grep -n "public void UnsubscribeAll\|internal void UnsubscribeAll" <file>` |
| Contract store cleanup | `src/DualFrontier.Application/Modding/ModContractStore.cs` | `RevokeAll(string)` | `grep -n "public void RevokeAll\|internal void RevokeAll" <file>` |
| Graph reset/rebuild primitive | `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` | `Reset()`, `Build()` | `grep -n "public void Reset\|public void Build" <file>` |
| Scheduler swap primitive | `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` | `Rebuild(IReadOnlyList<SystemPhase>)` | `grep -n "public void Rebuild" <file>` |
| Mod fault sink interface | `src/DualFrontier.Core/ECS/IModFaultSink.cs` | `IModFaultSink`, `NullModFaultSink` | `grep -n "interface IModFaultSink\|class NullModFaultSink" <file>` |
| **Mod fault handler (Phase 3 gap)** | `src/DualFrontier.Application/Modding/ModFaultHandler.cs` | `ModFaultHandler` class | `ls <file>` (expected: file does NOT exist) |
| **`ModLoader.HandleModFault` impl** | `src/DualFrontier.Application/Modding/ModLoader.cs` | `HandleModFault(string, ModIsolationException)` body | `grep -A 3 "public void HandleModFault" <file>` (expected body: `throw new NotImplementedException`) |
| Pause/resume tests | `tests/DualFrontier.Modding.Tests/Pipeline/M71PauseResumeTests.cs` | — | `ls <file>` |
| Unload chain tests | `tests/DualFrontier.Modding.Tests/Pipeline/M72UnloadChainTests.cs` | — | `ls <file>` |
| ALC verification tests | `tests/DualFrontier.Modding.Tests/Pipeline/M73Step7Tests.cs` | — | `ls <file>` |
| Phase 2 carried-debt tests | `tests/DualFrontier.Modding.Tests/Pipeline/M73Phase2DebtTests.cs` | — | `ls <file>` |
| Topological sort tests | `tests/DualFrontier.Modding.Tests/Pipeline/RegularModTopologicalSortTests.cs` | — | `ls <file>` |

**Gap classification** — based on inventory results:

- **Gap class A** (drift reconciliation): K6 deliverable is fulfilled but with naming or organization different from §K6 wording. Examples observed during brief authoring: "SystemGraph.Rebuild" (skeleton wording) vs. "DependencyGraph.Reset() + Build()" (reality), "ReloadMod" (skeleton wording) vs. "UnloadMod + Apply" (reality), "PhaseCoordinator.OnModChanged" (skeleton wording) vs. caller composition pattern (reality). Resolution: Phase 2.
- **Gap class B** (true implementation gap, K6 scope): K6 deliverable absent or stubbed. Examples observed during brief authoring: `ModFaultHandler` class does not exist; `ModLoader.HandleModFault` is `throw new NotImplementedException("TODO: Phase 2 (part 2) — ModFaultHandler")`. Resolution: Phase 3.
- **Gap class C** (test coverage gap): K6 contract semantically met by code but test coverage incomplete for some scenario. Resolution: Phase 4.

**Halt condition**: inventory diverges substantially from the table above (e.g. `ModIntegrationPipeline.cs` does not exist, or `RemoveMod` is missing, or `IModFaultSink` is not present). This indicates either a regression since the brief was authored or a different working tree than expected. Halt and escalate; do not improvise an implementation that re-creates from-scratch what should already exist.

### 0.5 — Managed build clean

```
dotnet build
```

**Expected**: build succeeds without warnings or errors.

**Halt condition**: build failure on baseline. K6 starts from a known-good managed state; regressions are not the executor's problem to debug.

### 0.6 — Managed test baseline

```
dotnet test
```

**Expected**: 538 tests passing minimum (post-K5; M7.1–M7.3 may have added more — record the actual baseline before continuing).

**Halt condition**: any test failing. Same reasoning as 0.5.

---

## Phase 1 — Verification of fulfilled K6 deliverables

For each deliverable identified in Phase 0.4 as already-fulfilled, the executor performs a structural and semantic verification pass. The goal is not just to confirm the API exists but that it satisfies the K6 contract in `KERNEL_ARCHITECTURE.md` §K6 and `MOD_OS_ARCHITECTURE.md` §9.5/§9.5.1.

Each verification block produces a one-line entry in a verification log written to `tools/briefs/K6_VERIFICATION_LOG.md` (created in this phase, committed at Phase 5). The log format:

```
| Deliverable | File | Method/Class | Verified | Evidence |
|---|---|---|---|---|
| ... | ... | ... | ✓ / partial / fail | <commit SHA, line ref, or test ref> |
```

### 1.1 — Pause/Resume primitives (MOD_OS §9.2 / §9.3)

**File**: `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`

**Contract**:
- `Pause()` sets `_isRunning = false`, idempotent
- `Resume()` sets `_isRunning = true`, idempotent
- `IsRunning` property returns current state
- `Apply()` and `UnloadMod()` throw `InvalidOperationException` when `IsRunning` is true

**Verify**:
1. `grep -A 1 "public void Pause"` returns method body assigning `_isRunning = false`
2. `grep -A 1 "public void Resume"` returns method body assigning `_isRunning = true`
3. `grep "throw new InvalidOperationException.*Pause the scheduler"` returns at least 2 matches (one in `Apply`, one in `UnloadMod`)
4. `M71PauseResumeTests.cs` covers: pause idempotent, resume idempotent, Apply-while-running throws, UnloadMod-while-running throws

**Log entry**: ✓ if all 4 verifications pass; partial otherwise with specifics.

### 1.2 — Apply (mod load → graph rebuild → scheduler swap)

**File**: `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`

**Contract** (per MOD_OS §9.2 + §11.1 M5–M6 reference):
- `Apply(IReadOnlyList<string>)` is the atomic mod batch entry point
- Steps follow numbered comments [0]–[8] in the implementation
- Atomicity: graph build runs on local variable; scheduler phase list replaced only on success
- Rollback: any exception earlier in the chain reverts loaded mods
- Replaces handling: kernel bridge systems whose FQN appears in any mod's `Replaces` are skipped per §7.1 step 3

**Verify**:
1. Method signature matches `public PipelineResult Apply(IReadOnlyList<string> modPaths)`
2. Comment markers `[0]`, `[1]`, `[2]`, `[3]`, `[4]`, `[5-7]`, `[8]` present in method body
3. `localGraph.Build()` called before `_scheduler.Rebuild()` (atomicity)
4. `RollbackLoaded(loaded)` called from at least 3 error paths (validation, init failure, graph build failure)
5. `CollectReplacedFqns` invoked before graph build
6. `M51PipelineIntegrationTests.cs`, `M52IntegrationTests.cs`, `M62IntegrationTests.cs` cover Apply happy paths and rollback paths

**Log entry**: ✓ / partial.

### 1.3 — UnloadMod (single-mod §9.5 chain)

**File**: `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`

**Contract** (per MOD_OS §9.5 steps 1–7 + §9.5.1 best-effort):
- Step 1: `RestrictedModApi.UnsubscribeAll`
- Step 2: `IModContractStore.RevokeAll(modId)`
- Step 3: `ModRegistry.RemoveMod(modId)`
- Step 4: graph rebuild (DependencyGraph reset + AddSystem + Build)
- Step 5: `ParallelSystemScheduler.Rebuild`
- Step 6: `ModLoader.UnloadMod(modId)` (calls `mod.Instance.Unload()` + `mod.Context.Unload()`)
- Step 7: WeakReference spin with GC pump bracket, 100 × 100 ms = 10 s timeout
- §9.5.1: each step wrapped in try/catch; failures emit `ValidationWarning`; chain continues
- Idempotent: unknown modId returns empty warnings, no throw
- JIT inlining defense: WeakReference capture and step 7 spin in non-inlined helpers per the doc comments

**Verify**:
1. `RunUnloadSteps1Through6AndCaptureAlc` exists and is `[MethodImpl(MethodImplOptions.NoInlining)]`
2. `CaptureAlcWeakReference` exists and is `[MethodImpl(MethodImplOptions.NoInlining)]`
3. `TryStep7AlcVerification` exists and is `[MethodImpl(MethodImplOptions.NoInlining)]`
4. `TryUnloadStep` wraps each step in try/catch; on exception adds `ValidationWarning`
5. Step 7 timeout constants: `Step7TimeoutMs = 10_000`, `Step7PollIntervalMs = 100`
6. GC pump bracket present: `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();` in spin loop
7. `ModUnloadTimeout` warning text contains `modId`, `"§9.5 step 7"`, `"10000 ms"` substrings
8. `M72UnloadChainTests.cs` covers: step ordering, step 1–6 best-effort, missing modId no-op
9. `M73Step7Tests.cs` covers: WeakReference release within timeout, timeout fires `ModUnloadTimeout` warning
10. `M73Phase2DebtTests.cs` covers Phase 2 carried debt (real-world unload scenarios)

**Log entry**: ✓ / partial / fail with which sub-check failed.

### 1.4 — UnloadAll (bulk unload)

**File**: `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`

**Contract**:
- Snapshots active mod ids in non-inlined helper (`SnapshotActiveModIds`)
- Iterates ids and calls `UnloadMod` per id
- Accumulates warnings across all per-mod unload chains
- Final scheduler rebuild for empty-active-set case (kernel-only graph reinstalled)
- Throws if `IsRunning`

**Verify**:
1. Method signature matches `public IReadOnlyList<ValidationWarning> UnloadAll()`
2. `SnapshotActiveModIds` is `[MethodImpl(MethodImplOptions.NoInlining)]`
3. `UnloadAll` calls `UnloadMod` per id, accumulates warnings
4. Empty-set path runs final scheduler rebuild
5. `M72UnloadChainTests.cs` or adjacent covers UnloadAll on multi-mod set

**Log entry**: ✓ / partial.

### 1.5 — ModRegistry.RemoveMod (§9.5 step 3 backing primitive)

**File**: `src/DualFrontier.Application/Modding/ModRegistry.cs`

**Contract**:
- Removes mod systems where `SystemRegistration.ModId == modId`
- Removes component owners where `_componentOwners[type] == modId`
- Reverse-pass on `_modSystems` (indices don't shift)
- Distinct from `ResetModSystems` which clears all mod state

**Verify**:
1. Method signature matches `public void RemoveMod(string modId)`
2. Reverse iteration on `_modSystems` (`for (int i = _modSystems.Count - 1; i >= 0; i--)`)
3. Two-pass on `_componentOwners` (collect-then-remove to avoid concurrent modification)
4. Argument null check throws `ArgumentNullException`

**Log entry**: ✓ / partial.

### 1.6 — DependencyGraph.Reset (graph rebuild primitive)

**File**: `src/DualFrontier.Core/Scheduling/DependencyGraph.cs`

**Contract**:
- `Reset()` clears `_systems`, `_edges`, `_phases`, sets `_built = false`
- After Reset, `AddSystem` and `Build` callable again
- Documented as "Used by tests and future mod hot-reload"

**Verify**:
1. Method signature matches `public void Reset()`
2. Clears all three internal collections
3. Resets `_built` flag
4. Doc comment mentions "mod hot-reload"

**Log entry**: ✓ / partial.

### 1.7 — ParallelSystemScheduler.Rebuild (scheduler swap primitive)

**File**: `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs`

**Contract**:
- `Rebuild(IReadOnlyList<SystemPhase>)` replaces internal phase list atomically
- Caller (pipeline) is responsible for ensuring no tick is in flight
- Used by `Apply` step [8] and `UnloadMod` step 5

**Verify**:
1. Method signature matches `public void Rebuild(IReadOnlyList<SystemPhase> phases)`
2. Argument null check
3. Internal field replacement atomic (single assignment, no partial state)

**Log entry**: ✓ / partial.

### 1.8 — IModFaultSink interface (Core-side fault contract)

**File**: `src/DualFrontier.Core/ECS/IModFaultSink.cs`

**Contract**:
- `internal interface IModFaultSink` with `ReportFault(string modId, string message)` method
- `internal sealed class NullModFaultSink : IModFaultSink` provides no-op default
- Used by `SystemExecutionContext` to surface mod faults without crashing the host process

**Verify**:
1. Interface declared `internal`
2. `ReportFault` signature matches
3. `NullModFaultSink.ReportFault` body is a no-op
4. Doc comments reference "ModFaultHandler (Phase 2 completion is scheduled later)" — flags adjacent debt for Phase 3

**Log entry**: ✓ — interface intact; `ModFaultHandler` is the missing implementation (Phase 3 gap).

### 1.9 — Verification log writeback

After Phases 1.1–1.8, the executor writes the verification log to `tools/briefs/K6_VERIFICATION_LOG.md`. Format:

```markdown
# K6 verification log

**Generated**: 2026-MM-DD by K6 brief execution session
**Source brief**: `tools/briefs/K6_MOD_REBUILD_BRIEF.md`
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §K6, `docs/architecture/MOD_OS_ARCHITECTURE.md` §9.5

| Deliverable | File | Method/Class | Verified | Evidence |
|---|---|---|---|---|
| Pause/Resume | ModIntegrationPipeline.cs | Pause/Resume/IsRunning | ✓ | M7.1 closure (commit SHA TBD); M71PauseResumeTests.cs |
| Apply | ModIntegrationPipeline.cs | Apply | ✓ | M5.x/M6.x closures; M51/M52/M62 integration tests |
| UnloadMod | ModIntegrationPipeline.cs | UnloadMod + helpers | ✓ | M7.2/M7.3 closures; M72/M73 tests |
| UnloadAll | ModIntegrationPipeline.cs | UnloadAll + SnapshotActiveModIds | ✓ | M7.2 closure |
| ModRegistry.RemoveMod | ModRegistry.cs | RemoveMod | ✓ | M7.2 closure |
| DependencyGraph.Reset | DependencyGraph.cs | Reset, Build | ✓ | Pre-K6 (designed for hot-reload per doc comment) |
| Scheduler.Rebuild | ParallelSystemScheduler.cs | Rebuild | ✓ | Pre-K6 |
| IModFaultSink | IModFaultSink.cs | interface + NullModFaultSink | ✓ | Pre-K6 (interface intact, ModFaultHandler is Phase 3 gap) |
```

**Atomic commit at end of Phase 1**:

```
docs(briefs): K6 verification log — confirm M7-era code fulfills K6 deliverables
```

---

## Phase 2 — Drift reconciliation (Gap class A)

The following naming and organization drifts exist between `KERNEL_ARCHITECTURE.md` §K6 wording and the realized implementation. The brief presents both reconciliation options with trade-offs; Crystalka selects one before the executor proceeds.

### 2.1 — Drift inventory

| KERNEL §K6 wording (v1.0) | Implementation reality | Drift kind |
|---|---|---|
| `SystemGraph.Rebuild(modRegistry)` | `DependencyGraph.Reset() + foreach AddSystem + Build()` (in `ModIntegrationPipeline.UnloadMod` step 4 inline; in `Apply` steps [5-7]) | Naming + composition (no single Rebuild method, the operation is inline) |
| `ModLoader.UnloadMod + ReloadMod` | `ModLoader.UnloadMod(modId)` exists; `ReloadMod` does not exist as a single method | Naming (Reload achieved via `Pause + UnloadMod + Apply([newPath]) + Resume`) |
| `PhaseCoordinator.OnModChanged()` event handler | No `PhaseCoordinator` class exists. Equivalent surface: `GameLoop.SetPaused(true) + ModIntegrationPipeline.Apply(...) + GameLoop.SetPaused(false)`, with `ModIntegrationPipeline.IsRunning` as the gate | Architectural (skeleton presupposed a coordinator class that the project never produced; behavior is composed across `GameLoop` and `ModIntegrationPipeline`) |

These drifts are functional — every K6 contract requirement is met semantically. The drifts are about wording and decomposition, not about missing capability.

### 2.2 — Reconciliation Option A: KERNEL_ARCHITECTURE.md v1.1 surgical amendment

**Approach**: Update `KERNEL_ARCHITECTURE.md` §K6 wording to match implementation reality. Bumps spec to v1.1 (non-semantic correction, parallel to MOD_OS v1.1–v1.5 pattern).

**Edits required** (executor performs via Anthropic `Edit` tool, literal-mode):

1. **Status line**: `**Version**: 1.0` → `**Version**: 1.1` (bumped status line per LOCKED amendment convention).
2. **Status line continuation**: append `, K6 wording reconciled with implementation reality` to the status note.
3. **Add v1.1 entry to Version history** (if KERNEL maintains one parallel to MOD_OS — verify in pre-flight read; if absent, no entry added, status line bump alone documents the change).
4. **§K6 Deliverables section**: replace
   ```
   - `SystemGraph.Rebuild(modRegistry)` method
   - `ModLoader.UnloadMod(modId)` + `ReloadMod(modId)`
   - `PhaseCoordinator.OnModChanged()` (pause/rebuild/resume tick)
   - Tests: rebuild correctness, unload+reload cycle, topological invariants
   ```
   with
   ```
   - Graph rebuild primitive: `DependencyGraph.Reset() + AddSystem + Build()` invoked from `ModIntegrationPipeline.UnloadMod` step 4 and `Apply` steps [5-7]
   - `ModLoader.UnloadMod(modId)` (per MOD_OS §9.5 step 6); reload composition: `Pause + UnloadMod + Apply([newPath]) + Resume`
   - Pause-rebuild-resume pattern composed across `GameLoop.SetPaused` and `ModIntegrationPipeline.Pause/Resume/Apply`; gate via `ModIntegrationPipeline.IsRunning` per MOD_OS §9.3
   - Tests: M71PauseResumeTests, M72UnloadChainTests, M73Phase2DebtTests, M73Step7Tests, RegularModTopologicalSortTests, M51/M52/M62 integration tests
   ```

**Trade-offs**:

- ✓ KERNEL spec reflects reality; future readers don't see drift
- ✓ Parallel to MOD_OS v1.x pattern (small surgical amendments accumulate without reopening Phase 0)
- ✗ KERNEL_ARCHITECTURE.md is LOCKED v1.0 by §0 of itself; amendments need explicit decision-log entry
- ✗ Cross-references in other docs (`PERFORMANCE.md`, `ROADMAP.md`, `MIGRATION_PROGRESS.md`) may quote the old wording; they need follow-up scan

**Atomic commits** (if Option A chosen):
1. `docs(kernel): bump status to v1.1`
2. `docs(kernel): reconcile §K6 wording with M7-era implementation`
3. `docs(kernel): add v1.1 entry to version history if applicable`

### 2.3 — Reconciliation Option B: Decision-log entry in MIGRATION_PROGRESS.md

**Approach**: Leave KERNEL_ARCHITECTURE.md LOCKED v1.0 unchanged. Document the drift as a Decision in MIGRATION_PROGRESS.md "Decisions log (operational)" section, parallel to D1/D2/D3.

**Edits required**:

1. Add `D4 — K6 deliverables fulfilled differently than KERNEL §K6 v1.0 wording suggested` to MIGRATION_PROGRESS.md "Decisions log".
2. Body of D4: enumerate the three drift items from §2.1 above, explicit acknowledgement that KERNEL spec wording is "speculative pre-implementation", reality is authoritative going forward, no spec amendment needed because functional contract is met.

**Trade-offs**:

- ✓ KERNEL_ARCHITECTURE.md stays LOCKED v1.0 (no ratchet effect on amendment count)
- ✓ Drift is recorded; future readers can see why §K6 wording disagrees with code
- ✓ MIGRATION_PROGRESS.md is the live tracker, expected to accumulate operational decisions
- ✗ Drift lives in two places (spec wording and decision log) instead of one (updated spec wording)
- ✗ Future reader of KERNEL §K6 alone (not reading MIGRATION_PROGRESS) sees stale wording

**Atomic commits** (if Option B chosen):
1. `docs(migration): add D4 — K6 wording vs M7 implementation drift acknowledgement`

### 2.4 — Reconciliation Option C: Both

**Approach**: Apply Option A (spec amendment) AND Option B (decision log entry, but pointing to v1.1). Strongest documentation, highest cost.

**Trade-offs**:

- ✓ Spec accurate AND drift rationale captured separately for audit trail
- ✗ Most edits, most commits; maximum "no compromises" but possibly overengineered for a wording-only drift

**Atomic commits**: union of A and B above.

### 2.5 — Selection prompt

The executor halts at this point and prompts Crystalka:

```
Phase 2 reconciliation — choose one:
  [A] KERNEL_ARCHITECTURE.md v1.1 surgical amendment (spec reflects reality)
  [B] MIGRATION_PROGRESS.md D4 decision log entry (spec stays v1.0)
  [C] Both A and B (maximum documentation)

Default: [B] — minimal ratchet on KERNEL_ARCHITECTURE.md, drift documented operationally.
```

Crystalka responds with letter; executor proceeds with selected option.

**Stop condition**: ambiguous response or new option proposed. Halt and escalate to brief authoring session.

---

## Phase 3 — HandleModFault implementation (Gap class B)

The Phase 0.4 inventory identified `ModLoader.HandleModFault` as `throw new NotImplementedException("TODO: Phase 2 (part 2) — ModFaultHandler")`. The corresponding `ModFaultHandler` class does not exist. K6 closure requires the mod fault path to be operational because:

1. K6 deliverables include "managed dependency graph rebuilds when mods load/unload" — a faulting mod is a special case of unload triggered by the isolation guard.
2. `IModFaultSink` (Core-side interface) is intact; the Application-side default implementation is the gap.
3. Without `ModFaultHandler`, a real mod isolation violation produces `IsolationViolationException` from `SystemExecutionContext` and `NotImplementedException` from `ModLoader.HandleModFault` if the pipeline routes to it — the second exception masks the first and the user sees a generic crash, not a localized mod-disabled state.

### 3.1 — ModFaultHandler design

**File**: `src/DualFrontier.Application/Modding/ModFaultHandler.cs` (NEW)

**Contract**:

```csharp
namespace DualFrontier.Application.Modding;

/// <summary>
/// Application-side handler for mod faults reported through the Core
/// <see cref="DualFrontier.Core.ECS.IModFaultSink"/> interface. Implements
/// the "fault → unload" pipeline per MOD_OS_ARCHITECTURE §10.3 (architectural
/// threats — caught) and TechArch 11.8 (the documented "core does not crash;
/// the offending mod is unloaded" behaviour).
///
/// Owned by ModIntegrationPipeline; constructed during pipeline startup and
/// installed into SystemExecutionContext via DI before the first tick. Not
/// reentrant: a fault during fault handling is logged but not re-routed
/// (the second handle attempt swallows itself per defensive try/catch).
///
/// The handler does NOT rebuild the dependency graph immediately. Per
/// TechArch 11.8 and the existing comment in ModLoader.HandleModFault,
/// graph rebuild is deferred to the next menu open. Rationale: the fault
/// arrives mid-tick on a worker thread; rebuilding the graph synchronously
/// would race with other workers. The mod is marked for removal; the next
/// time the user opens the mod menu, the pipeline observes the marked mod
/// is no longer in the active set and the rebuild happens through normal
/// Apply flow.
/// </summary>
internal sealed class ModFaultHandler : IModFaultSink
{
    private readonly ModIntegrationPipeline _pipeline;
    private readonly object _lock = new();
    private readonly HashSet<string> _faultedMods = new(StringComparer.Ordinal);

    public ModFaultHandler(ModIntegrationPipeline pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    /// <summary>
    /// Called by SystemExecutionContext (via the IModFaultSink contract) when
    /// a mod system breaks isolation. Records the fault in an internal set
    /// for deferred unload at the next menu open. Idempotent: faulting the
    /// same mod twice during a single tick is harmless.
    /// </summary>
    public void ReportFault(string modId, string message)
    {
        if (modId is null) return; // defensive — sink contract is non-throwing
        lock (_lock)
        {
            _faultedMods.Add(modId);
        }
        // Logging delegated to the calling system's log channel; the sink
        // itself is intentionally silent per IModFaultSink doc — the
        // IsolationViolationException carries the diagnostic message.
    }

    /// <summary>
    /// Snapshot of mods marked for unload due to faults. Read by
    /// ModIntegrationPipeline at menu-open time to determine which mods to
    /// drop from the active set during the next Apply.
    /// </summary>
    public IReadOnlyList<string> GetFaultedMods()
    {
        lock (_lock)
        {
            return _faultedMods.Count == 0
                ? Array.Empty<string>()
                : new List<string>(_faultedMods);
        }
    }

    /// <summary>
    /// Clears the faulted-mods set. Called by ModIntegrationPipeline after
    /// the deferred unload completes during an Apply or UnloadMod cycle.
    /// </summary>
    public void ClearFault(string modId)
    {
        if (modId is null) return;
        lock (_lock)
        {
            _faultedMods.Remove(modId);
        }
    }
}
```

### 3.2 — ModLoader.HandleModFault rewrite

**File**: `src/DualFrontier.Application/Modding/ModLoader.cs`

**Edit**: replace the existing `HandleModFault` body:

```csharp
public void HandleModFault(string modId, ModIsolationException exception)
{
    // TODO: Phase 2 (part 2) — extracted into a dedicated ModFaultHandler.
    throw new NotImplementedException("TODO: Phase 2 (part 2) — ModFaultHandler");
}
```

with:

```csharp
public void HandleModFault(string modId, ModIsolationException exception)
{
    // Per MOD_OS_ARCHITECTURE §10.3 + TechArch 11.8: the core does not
    // crash on a mod isolation violation. The fault is reported through
    // ModFaultHandler (an IModFaultSink implementation owned by
    // ModIntegrationPipeline); the offending mod is queued for deferred
    // unload at the next menu open per the design comment retained from
    // the original Phase 2 (part 2) plan.
    //
    // This method is the public-surface entry point for callers that hold
    // a ModLoader reference but not a ModFaultHandler reference (e.g.
    // legacy SystemExecutionContext wiring). For new code, route faults
    // through IModFaultSink directly.
    //
    // Idempotent: handling the same fault twice is harmless. The handler
    // de-duplicates internally.
    if (modId is null) throw new ArgumentNullException(nameof(modId));
    if (exception is null) throw new ArgumentNullException(nameof(exception));

    _faultHandler?.ReportFault(modId, exception.Message);
}
```

Add field `private ModFaultHandler? _faultHandler;` and constructor parameter or setter to inject it. The pipeline owns the handler instance; ModLoader receives the reference during pipeline construction.

### 3.3 — Wiring through ModIntegrationPipeline

**File**: `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`

**Edit 1**: Add `private readonly ModFaultHandler _faultHandler;` field.

**Edit 2**: Construct `ModFaultHandler` in pipeline ctor, pass to `ModLoader` via setter:

```csharp
public ModIntegrationPipeline(
    ModLoader loader,
    ModRegistry registry,
    ContractValidator validator,
    IModContractStore contractStore,
    IGameServices services,
    ParallelSystemScheduler scheduler)
{
    _loader = loader ?? throw new ArgumentNullException(nameof(loader));
    _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _contractStore = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
    _services = services ?? throw new ArgumentNullException(nameof(services));
    _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    _faultHandler = new ModFaultHandler(this);
    _loader.SetFaultHandler(_faultHandler); // NEW — wires deferred unload path
}
```

**Edit 3**: At the start of `Apply`, drain `_faultHandler.GetFaultedMods()` and prepend their UnloadMod calls before processing the new mod set:

```csharp
public PipelineResult Apply(IReadOnlyList<string> modPaths)
{
    if (modPaths is null) throw new ArgumentNullException(nameof(modPaths));
    if (_isRunning)
        throw new InvalidOperationException(
            "Pause the scheduler before applying mods");

    // [-1] Drain faulted mods from the previous tick(s). Each is unloaded
    // through the standard §9.5 chain; warnings are accumulated.
    var faultedWarnings = new List<ValidationWarning>();
    foreach (string faultedId in _faultHandler.GetFaultedMods())
    {
        IReadOnlyList<ValidationWarning> ws = UnloadMod(faultedId);
        foreach (ValidationWarning w in ws) faultedWarnings.Add(w);
        _faultHandler.ClearFault(faultedId);
    }

    // [0] ... existing Apply body continues here ...
}
```

**Edit 4**: Merge `faultedWarnings` into final `PipelineResult.Warnings` on every return path.

### 3.4 — SystemExecutionContext wiring

**File**: `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` (verify exists)

**Verify**: `SystemExecutionContext` already accepts `IModFaultSink` per the `IModFaultSink.cs` doc comment. If yes, no edit needed in Core; the Application-side wiring routes the real `ModFaultHandler` instance during pipeline startup, replacing the `NullModFaultSink` default.

**If `SystemExecutionContext` does not yet accept `IModFaultSink`**: this is a deeper Phase 3 expansion; halt and escalate (the brief assumed M3-era Phase 2 work landed the Core-side hookup; if not, K6 closure depends on Phase 2 completion).

### 3.5 — ModFaultHandler tests

**File**: `tests/DualFrontier.Modding.Tests/Pipeline/ModFaultHandlerTests.cs` (NEW)

**Test cases** (≥8):

1. `ReportFault_AddsModToFaultedSet` — call ReportFault, assert GetFaultedMods returns the id.
2. `ReportFault_IsIdempotent` — call ReportFault twice with same id, assert GetFaultedMods returns the id once.
3. `ReportFault_NullModId_NoThrow` — defensive contract check.
4. `GetFaultedMods_EmptyByDefault` — fresh handler returns empty list.
5. `ClearFault_RemovesFromSet` — ReportFault then ClearFault then GetFaultedMods returns empty.
6. `ClearFault_UnknownMod_NoThrow` — ClearFault on never-faulted id is a no-op.
7. `ConcurrentReportFault_ThreadSafe` — N parallel ReportFault calls from M threads, assert final set size = unique ids reported.
8. `ApplyAfterFault_DrainsFaultedMods` — pipeline integration test: load mod, manually call ReportFault, call Apply with empty modPaths, assert mod is no longer in active set.

### 3.6 — Atomic commits for Phase 3

1. `feat(application): add ModFaultHandler implementing IModFaultSink`
2. `feat(application): wire ModFaultHandler into ModLoader.HandleModFault`
3. `feat(application): wire ModFaultHandler through ModIntegrationPipeline (drain on Apply)`
4. `test(modding): add ModFaultHandlerTests covering report/get/clear/concurrent/integration`

### 3.7 — Build + test gate

After Phase 3 commits:

```
dotnet build
dotnet test
```

**Expected**: 0 errors, 0 warnings, all tests pass (538 baseline + 8 ModFaultHandler tests = 546 minimum; M7-era count may already exceed 538).

**Stop condition**: any test failure. The new tests are deterministic; failures indicate either a test bug or a wiring bug — both halt for review.

---

## Phase 4 — Test coverage audit (Gap class C)

The M7.x test set covers the M7-specific scope (pause/resume/unload chain). K6 introduces additional contract requirements that may or may not have explicit test coverage. The executor performs a coverage audit and adds tests for any gaps.

### 4.1 — Coverage matrix

| K6 contract requirement | Existing test coverage | Gap |
|---|---|---|
| Apply succeeds → graph rebuilt → scheduler swapped | M51/M52/M62 integration tests | None |
| Apply fails on validation → mods rolled back → scheduler untouched | M52IntegrationTests | None |
| Apply fails on graph build → all loaded mods rolled back | Implicit in M52 | **Possible gap**: explicit test for Build() throwing CyclicDependency mid-Apply |
| UnloadMod on absent modId → no-op, empty warnings | Verify in M72UnloadChainTests | If absent, add `UnloadMod_UnknownId_ReturnsEmptyWarnings` |
| UnloadMod step 1 throws → step 2-6 still run, warning recorded | M72UnloadChainTests | None |
| UnloadMod step 7 timeout → ModUnloadTimeout warning + mod removed from active set | M73Step7Tests | None |
| UnloadAll on N>1 mods → all unloaded, warnings accumulated, scheduler rebuilt | M72UnloadChainTests | Verify presence of multi-mod scenario |
| UnloadAll on empty active set → final scheduler rebuild runs (kernel-only graph) | Possible gap | If absent, add `UnloadAll_EmptyActiveSet_RebuildsKernelOnlyGraph` |
| Concurrent ApplyAttempt while IsRunning → throws InvalidOperationException | M71PauseResumeTests | None |
| **Mod fault → fault recorded → next Apply drains the faulted mod** | None (Phase 3 introduces) | Phase 3 tests cover |
| Topological sort of regular mods produces deterministic order | RegularModTopologicalSortTests | None |
| Bridge replacement (mod replaces kernel system FQN) → kernel system skipped during graph build | M62IntegrationTests + CollectReplacedFqnsTests | None |

### 4.2 — Gap-fill tests

For each "Possible gap" or "If absent" entry, the executor first verifies presence of equivalent coverage in the existing test set; if confirmed absent, adds a focused test.

**File**: `tests/DualFrontier.Modding.Tests/Pipeline/K6CoverageGapTests.cs` (NEW, conditional — created only if any gaps confirmed)

**Test names** (added only for confirmed gaps):

1. `Apply_GraphBuildFailsWithCyclicDependency_AllLoadedModsRolledBack`
2. `UnloadMod_UnknownModId_ReturnsEmptyWarningsList`
3. `UnloadAll_EmptyActiveSet_RebuildsKernelOnlyGraph`

### 4.3 — Atomic commit for Phase 4

```
test(modding): add K6 coverage gap tests for Apply/UnloadMod/UnloadAll edge cases
```

(Skipped if coverage audit finds no gaps.)

---

## Phase 5 — Closure documentation

K6 milestone closure is recorded across three documents.

### 5.1 — MIGRATION_PROGRESS.md update

**File**: `docs/MIGRATION_PROGRESS.md`

**Edit 1**: K-series Overview table, K6 row:

`| K6 | Second-graph rebuild on mod change | NOT STARTED | 3–5 days | — | — |`

→

`| K6 | Second-graph rebuild on mod change | DONE | <commit SHA range> | <date> |`

**Edit 2**: Add K6 closure section after K5:

```markdown
### K6 — Second-graph rebuild on mod change

- **Status**: DONE (`<commit SHA range>`, <date>)
- **Brief**: `tools/briefs/K6_MOD_REBUILD_BRIEF.md` (FULL EXECUTED)
- **Closure shape**: Most deliverables fulfilled by parallel MOD_OS migration M7.1–M7.3 work (pause/resume primitives, full §9.5 unload chain, ALC verification). K6 brief executed three additional phases:
  1. **Verification** of M7-era code against K6 contract — log at `tools/briefs/K6_VERIFICATION_LOG.md`.
  2. **Drift reconciliation** between KERNEL §K6 v1.0 wording and implementation reality — Crystalka selected Option <A/B/C> at Phase 2.5.
  3. **Adjacent debt fill** — `ModFaultHandler` implementation closing the Phase 2 part 2 TODO in `ModLoader.HandleModFault`.
- **Test count**: 538 baseline → <new count after Phase 3 + 4> passing (+<delta>).
- **Lessons learned**:
  - M-series (mod migration) and K-series (kernel migration) have meaningful overlap. Future skeleton briefs should cross-check overlapping migration phases before being authored as full implementation briefs — the K6 case shows that a deliverable nominally in the kernel track may already be fulfilled by mod-track work.
  - "Closure-shaped implementation brief" is a third brief type alongside "implementation" and "skeleton". Used when the milestone's deliverables exist but verification, drift reconciliation, and adjacent debt are needed for closure.
  - `IModFaultSink` interface (Core-side) was authored during M3-era work but the Application-side `ModFaultHandler` was deferred. K6 closure exposed the deferred work as a real gap (mod isolation violations would crash with `NotImplementedException`). The fix lands as part of K6 because K6 closure semantically requires the fault → unload path to function.
```

**Edit 3**: Update `Current state snapshot` table:

- `Active phase`: K6 → K7 (planned)
- `Last completed milestone`: K5 → K6
- `Tests passing`: 538 → <new count>

**Edit 4** (if Phase 2 Option B or C selected): Add D4 entry to Decisions log per Phase 2.3 wording.

### 5.2 — KERNEL_ARCHITECTURE.md update (if Phase 2 Option A or C selected)

Already covered in Phase 2.2 edit list. If Option B selected at Phase 2.5, this sub-phase is skipped.

### 5.3 — Atomic commits for Phase 5

1. `docs(migration): K6 closure recorded — promote K6 row to DONE` (always)
2. `docs(migration): add D4 decision log — K6 wording vs M7 implementation drift` (if Option B or C)
3. (Phase 2 KERNEL amendment commits already enumerated in 2.2 — listed under Phase 5 only if execution order places them after the closure recording)

### 5.4 — Final verification

```
dotnet build
dotnet test
```

**Expected**: 0 errors, 0 warnings, all tests pass.

**Final pre-commit grep** (AD #4 discipline):

```
grep -rn "TODO: Phase 2 (part 2)" src/ tests/
grep -rn "ModFaultHandler" src/ tests/
```

**Expected**:
- First grep returns 0 matches (the only TODO of this kind was in ModLoader.HandleModFault, fixed in Phase 3)
- Second grep returns matches in ModFaultHandler.cs (definition), ModLoader.cs (field + setter + use), ModIntegrationPipeline.cs (field + ctor + Apply drain), ModFaultHandlerTests.cs (test class)

---

## Atomic commit log expected

Approximate commit count: **8-14**, range depends on Phase 2 selection and Phase 4 gap detection:

**Always present** (5):
1. `docs(briefs): K6 verification log — confirm M7-era code fulfills K6 deliverables` (Phase 1)
2. `feat(application): add ModFaultHandler implementing IModFaultSink` (Phase 3.1)
3. `feat(application): wire ModFaultHandler into ModLoader.HandleModFault` (Phase 3.2)
4. `feat(application): wire ModFaultHandler through ModIntegrationPipeline (drain on Apply)` (Phase 3.3)
5. `test(modding): add ModFaultHandlerTests covering report/get/clear/concurrent/integration` (Phase 3.5)

**Phase 2 conditional** (1-3):
- Option A: `docs(kernel): bump status to v1.1` + `docs(kernel): reconcile §K6 wording with M7-era implementation` + `docs(kernel): add v1.1 entry to version history if applicable` (3 commits)
- Option B: `docs(migration): add D4 — K6 wording vs M7 implementation drift acknowledgement` (1 commit)
- Option C: union of A and B (4 commits)

**Phase 4 conditional** (0-1):
- If gaps found: `test(modding): add K6 coverage gap tests for Apply/UnloadMod/UnloadAll edge cases` (1 commit)

**Phase 5 always** (1-2):
- `docs(migration): K6 closure recorded — promote K6 row to DONE` (always)
- `docs(migration): add D4 decision log — K6 wording vs M7 implementation drift` (only if Phase 2 Option B or C and not already committed in Phase 2)

A merge commit on `main` is **not** in this list — the merge is fast-forward, no commit produced.

---

## Cross-cutting design constraints

This brief explicitly enforces the following architectural invariants. The executor checks each at the relevant phase and halts on violation.

1. **Verification before implementation** (closure-shaped brief discipline). Phase 0.4 inventory must complete and produce evidence of which deliverables are fulfilled before any new code is written. Phase 1 verification log must exist before Phase 3 implementation begins. This is the methodology pivot from `MOD_OS_V16_AMENDMENT_CLOSURE.md` applied to a closure brief: "data exists or it doesn't" → "if the deliverable exists, document evidence; if not, fill the gap explicitly".

2. **Native code untouched** (per K-L7 direction discipline + KERNEL §K6 wording). K6 is a managed-only milestone. The native kernel is not modified. `IModFaultSink` lives in `DualFrontier.Core` (managed), not in the C++ side. Any edit to `native/` files during K6 execution is a brief violation — halt and escalate.

3. **Atomic commits per logical change** (per project standing rule). One commit per Phase sub-step. The commit log above is the contract; deviations halt for review. Each commit must build and test cleanly (no commits with intermediate broken state).

4. **No regex metacharacters in `Edit` tool boundaries** (per `MOD_OS_V16_AMENDMENT_CLOSURE.md` lessons learned). All `oldText` / `newText` payloads in this brief are plain prose / code without `$ ^ \b \d \w \s [ ] ( | ) * + ?` at the boundary positions. Where regex content appears as content (e.g. capability syntax, regex strings in comments), it lives inside fenced code blocks where it is interior, not boundary. Anthropic `Edit` tool uses literal string matching; this rule is a defense-in-depth across both literal-mode and regex-mode editors.

5. **Pre-flight grep discipline** (AD #4 from project memory). Phase 0.4 inventory is a structured grep-based verification. Phase 5.4 final pre-commit grep verifies no TODO leaks. Every Phase that touches existing identifiers checks for collisions or stale references via grep before introducing changes.

6. **Triple binding awareness** (Russian error message strings caveat). K6 work touches files that contain user-facing diagnostic strings (`ModUnloadTimeout` warning text, `Unload step N failed for mod 'X'` warning text). These strings appear in:
   - Source code (`ModIntegrationPipeline.cs`)
   - Test assertions (`M72UnloadChainTests.cs`, `M73Step7Tests.cs`)
   - Spec wording (`MOD_OS_ARCHITECTURE.md` §9.5 step 7)
   K6 must NOT modify any of these strings. If a Phase 3 wiring change incidentally touches one, halt and escalate — three-way atomic commit required, outside K6 scope.

7. **Drift reconciliation requires explicit selection** (per Phase 2.5). The executor does NOT silently choose Option A, B, or C. The executor halts at Phase 2.5, prompts Crystalka, and proceeds only after explicit selection. Default is Option B but is NOT auto-applied — the prompt makes the choice visible.

8. **`HandleModFault` is K6 closure-blocking** (Phase 3 gate). The brief executor does NOT skip Phase 3 even if the verification log shows everything else green. K6 closure semantically requires the fault → unload path operational; closing K6 with `HandleModFault` still throwing `NotImplementedException` would be a false closure.

9. **«Data exists or it doesn't»** (METHODOLOGY §7.1). `ModFaultHandler.ReportFault(null modId, ...)` is a no-op (defensive); `ReportFault("nonempty", message)` records the fault. There is no third state. `GetFaultedMods()` returns the actual set or empty; never null, never partial.

10. **Single ownership boundary** (per K-L8). `ModFaultHandler` owns the `_faultedMods` HashSet; access is gated through `lock (_lock)`. No external code mutates the set directly. `ModIntegrationPipeline` queries via `GetFaultedMods()` (read snapshot) and clears via `ClearFault(modId)` (single-id write); never reaches into the field.

---

## Stop conditions

The executor halts and escalates the brief authoring session if any of the following:

1. Any pre-flight check (Phase 0) fails — working tree dirty, prerequisites missing, specs at unexpected version, baseline build/test fails.
2. Phase 0.4 inventory diverges substantially from the table — not just a missing test file, but missing core methods (e.g. `ModIntegrationPipeline.UnloadMod` does not exist). This indicates regression or different working tree.
3. Phase 1 verification finds a "fail" entry — a deliverable nominally fulfilled actually missing key contract requirements (e.g. `UnloadMod` exists but does not implement the §9.5 chain steps in order, or skips the GC pump bracket).
4. Phase 2.5 receives an ambiguous selection or new option not in [A, B, C].
5. Phase 3.4 finds `SystemExecutionContext` does not accept `IModFaultSink` — Core-side wiring missing, K6 cannot close.
6. Any phase produces unexpected `dotnet build` warnings or errors not covered by the deterministic edits.
7. Any test fails after a phase's edits — edits are deterministic, so failures indicate either a brief bug (escalate) or an environment issue (escalate with environment details).
8. The `Edit` tool reports unexpected behavior on any oldText/newText pair.
9. The executor encounters a project structure that contradicts this brief's assumptions (e.g. `RestrictedModApi` lives in a different file, `IModApi` interface has a different shape, `ModFaultHandler.cs` already exists with different content).
10. Native kernel files are modified during execution — K6 is managed-only; any `native/` edit halts the session.

The fallback in every halt case is `git stash push -m "k6-WIP-halt-$(date +%s)"` and report to the brief author. Partial K6 work is recoverable; an ad-hoc continuation on a corrupted state is not.

---

## Brief authoring lineage

- **2026-05-08** — Skeleton committed alongside MOD_OS v1.6 amendment closure (commit `d5dcdde` per closure record extension to K-series briefs, OR earlier — verify in pre-flight read).
- **2026-05-09** — Skeleton expanded to closure-shaped implementation brief in this revision. Author: Opus architect session per «доки сначала, миграция потом» pivot continuation. K6 identified as overlap candidate with M7.x work; gap analysis performed read-first; brief authored to verify-then-fill rather than rewrite-from-scratch.
- **(date TBD)** — Executed and closed at K6 milestone closure.

The closure-shaped brief format was authored read-first / brief-second per the methodology pivot recorded in `MOD_OS_V16_AMENDMENT_CLOSURE.md`. Source documents read during authoring: `KERNEL_ARCHITECTURE.md` v1.0 LOCKED §K6, `MOD_OS_ARCHITECTURE.md` v1.6 LOCKED §9 + §10.3, `MIGRATION_PROGRESS.md` (live), existing managed code (`ModIntegrationPipeline.cs`, `ModLoader.cs`, `ModRegistry.cs`, `DependencyGraph.cs`, `IModFaultSink.cs`, `ModIsolationException.cs`), existing test set inventory (`M51/M52/M62/M71/M72/M73*` + `RegularModTopologicalSortTests.cs`). Authored against the contract surfaces in those documents verbatim; any deviation between this brief and those specs is a brief bug, not a design choice.

---

## Methodology note on closure-shaped briefs

This brief introduces a third brief type to the project's pipeline:

| Type | Purpose | Examples |
|---|---|---|
| **Implementation brief** | Detailed instructions to write new code | K1, K2, K3, K5, K9 (full) |
| **Skeleton** | Place-holder pending authoring | G0–G9, K6 (pre-this-brief) |
| **Closure-shaped implementation brief** | Verify existing fulfillment + reconcile drift + fill adjacent gaps | K6 (this brief) |

The closure-shaped variant is appropriate when:

1. The milestone deliverables exist or substantially exist on disk via parallel migration tracks.
2. Drift between specification wording and implementation reality requires explicit documentation.
3. Adjacent debt blocks meaningful closure (the milestone cannot be honestly marked DONE while `HandleModFault` throws `NotImplementedException`).

Future briefs (K7, K8) may take this shape if the M-series work or other parallel tracks have substantially fulfilled their deliverables. Future K9 brief execution remains a pure implementation brief because K9 has no parallel track.

The «no compromises» rule applies to closure-shaped briefs as strictly as to implementation briefs: verification produces evidence, drift is reconciled (not silently accepted), adjacent debt is filled (not deferred again).

---

**Brief end.** Awaits Crystalka's review and feed to Claude Code session for execution.
