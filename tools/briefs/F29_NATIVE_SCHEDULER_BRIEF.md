---
register_id: DOC-D-F29_NATIVE_SCHEDULER_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-07-04
last_modified: 2026-07-04
content_language: en
next_review_due: null
title: F29_NATIVE_SCHEDULER_BRIEF — F-29 native-scheduler cascade execution brief (FIX defect (b) O(N^2) -> O(N+E) index-keyed graph rebuild + FIX defect (a) fail-loud native concurrency detector on the lock-free scheduler-graph + wake-registry singletons + a shared xUnit collection over the singleton-touching classes; un-quarantine S1/S7; re-point S2 -> F-30; S3/S4/S5 bus-load artifact -> F-31; TESTING_STRATEGY 2.1.0 -> 2.2.0 §2.8 isolation law; F-29 CLOSED / F-30 + F-31 seeded; REGISTER 2.22 -> 2.23)
last_modified_commit: c0ab964
review_cadence: on-cascade-execution
last_review_date: 2026-07-04
last_review_event: 'F-29 native-scheduler brief authored by Claude Opus from the durable recon docs/reports/F29_NATIVE_SCHEDULER_RECON_REPORT.md (2026-07-04, R1-R7); ratified by Crystalka 2026-07-04 (AskUserQuestion -> Execute now; Draft -> LOCKED). Executed 2026-07-04 by Claude Code (Opus 4.8, LOCAL Skarlet), single-orchestrator serial C1-C10; H1/H2/H3 gates clear; zero hard halts (one in-flight reclassification: the S3/S4/S5 extreme-bus-load probes root-caused a .NET-runtime-stress artifact -> new finding F-31, +1 commit vs the 9-commit intended form). Enrolled EXECUTED at the F-29 REGISTER closure.'
reviewer: Crystalka
risks_referenced: []
capa_entries_referenced: []
special_case_rationale: Category D Tier 3 -- the clean (non-forbidden) Category-D combo, per the design/decision-tier brief precedent (the K-series + F-10 DOC-D Tier-3 briefs). This brief authors standing law (the TESTING_STRATEGY §2.8 shared-native-singleton isolation law), so it enrolls at the authoritative design tier rather than the Tier-4 execution-brief convention of the A'.9.1 phase briefs. Entry id follows the brief's own frontmatter register_id.
---

---
register_id: DOC-D-F29_NATIVE_SCHEDULER_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft (-> LOCKED on Crystalka ratification -> EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-04'
content_language: en
authored_by: Claude Opus (deliberation session, F29_NATIVE_SCHEDULER prep)
basis: F29_NATIVE_SCHEDULER RECON REPORT 2026-07-04 (R1-R7)
---

# F29_NATIVE_SCHEDULER -- Execution Brief

This cascade repairs the two independent defects behind finding F-29 in the
native kernel system scheduler. Defect (b) is a scale wall: the graph rebuild in
`system_graph.cpp` is O(N^2) in the number of systems (two quadratic scans in
`kahn_topological_sort` plus an O(N) duplicate scan per `register_system` call),
which crosses a ~130 s compute budget near 90k systems. Defect (a) is a data
race: the process-global scheduler / wake / bus singletons are lock-free, and the
test suite runs two classes that both mutate them in parallel, corrupting the
heap (reproduced three independent ways, including a literal testhost crash at
`coreclr.dll+0x1d45aa`). The design contract is single-threaded registration and
compute; production honours it, the tests violate it. The fix keeps the clean
single-threaded no-lock kernel design, serialises the offending tests, adds a
cheap fail-loud concurrency detector so the contract can never again fail
silently, replaces the O(N^2) rebuild with an index-keyed O(N+E) one that
preserves observable behaviour exactly, un-quarantines the two Extreme scenarios
that defect (b) was blocking, and closes the finding in governance.

**Done** when: the native graph rebuild is O(N+E) with byte-identical output; the
scheduler / wake / bus singletons reject concurrent access with a loud violation
code instead of corrupting memory; the singleton-touching test classes share one
xUnit collection so the full suite is green and repeatable; two new native
selftest scenarios prove both the concurrency detector and the scale fix; S1 and
S7 are un-quarantined and pass; S2 is re-pointed to a new managed-marathon
finding; `TESTING_STRATEGY.md` records the isolation law and the refreshed
honesty register; and F-29 is CLOSED in the F-ledger with F-30 seeded.

Executor: Claude Code (flagship model), running LOCAL on the operator's machine
`SKARLET` against `D:\Colony_Simulator\Colony_Simulator`.

This brief is the authority for this cascade and is STANDALONE: it carries the
Phase 0 preconditions, the halt catalog, and the closure protocol inline. It
cites standing law by anchor rather than restating it. Where this brief and the
live code differ, the code wins and the conflict is recorded in the closure
report. Where this brief and a standing doc differ, the brief is wrong -- HALT
and escalate.

## 1. Mission [CORE]

Deliverable milestone: **F-29 resolved and closed** -- both the scale defect (b)
and the concurrency defect (a) fixed at the root, the design invariant made
self-enforcing, and the quarantined scenarios that (b) blocked returned to the
live suite.

| ID | Artifact | Action | Version |
| -- | -------- | ------ | ------- |
| D1 | `native/DualFrontier.Core.Native/src/system_graph.cpp` + `include/system_graph.h` | Index-keyed graph rebuild (defect b): O(N^2) -> O(N+E), behaviour-preserving | native, no version |
| D2 | `SystemGraph` + `WakeRegistry` + the native bus singleton (native) and their managed interops (`src/DualFrontier.Core.Interop/*`) | Fail-loud concurrency detector (defect a, native half) + additive violation code / managed enum value | native + src, no version |
| D3 | shared xUnit collection across every test class touching a shared native singleton (`tests/DualFrontier.Core.Tests/**`) | Serialise singleton-touching classes (defect a, test half) | tests, no version |
| D4 | `native/DualFrontier.Core.Native/test/selftest.cpp` | Two new DF_CHECK scenarios: concurrency-detection + scale | native, no version |
| D5 | `tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs` | Un-quarantine S1 + S7 (remove Skip); re-point S2 Skip reason to F-30 | tests, no version |
| D6 | `docs/methodology/TESTING_STRATEGY.md` | Honesty-register refresh + codified shared-singleton isolation law | 2.1.0 -> 2.2.0 |
| D7 | `docs/ROADMAP.md` (F-ledger) + `REGISTER.yaml` | F-29 -> CLOSED with real hashes, F-30 seeded; REGISTER closure | register_version 2.22 -> 2.23 |

Nothing downstream is gated on this cascade; it stands on its own. It does,
however, retire the S1/S7 quarantine and correct the S2 mis-grouping that the
F-10 cascade and the F-ledger carried, so the Extreme suite regains two real
scenarios and stops mislabelling a managed marathon as a native-scale pathology.

## 2. Established facts [CORE]

The facts below are the recon digest (F29_NATIVE_SCHEDULER RECON REPORT, R1-R7),
each verified by the architect against the live tree by direct line-read, except
the dynamic measurements (crash reproductions, N->time curves, WER records),
which are accepted by-report as internally coherent and cross-validated. A
re-verify symbol `[RV]` marks the facts the executor must re-confirm at Phase 0;
any mismatch is HALT H1.

**Base state.**
- `[RV]` HEAD is `e628e16` on `main`; working tree clean; `origin/main` == local.
- `[RV]` `REGISTER.yaml` `register_version` is `"2.22"`.
- `[RV]` `docs/methodology/TESTING_STRATEGY.md` is at version `2.1.0`.
- `[RV]` F-ledger: F-29 OPEN (S2); F-10 CLOSED (S1); F-28(a) folded / F-28(b) OPEN.

**Defect (b) -- the O(N^2) rebuild (all in `system_graph.cpp`), verified.**
- `[RV]` `register_system` does a linear duplicate scan `for (const SystemEntry& s : systems_) if (s.id == system_id) return false;` on EVERY call -> O(N^2) across N registrations. This is a second, independent quadratic site (not the Kahn scan).
- `[RV]` `kahn_topological_sort` step 1 (write-write conflict) is a nested `for i` x `for j = i+1` over the subset, each iteration building a `writes_a` set and scanning `b->writes` -> O(N^2 * W).
- `[RV]` `kahn_topological_sort` step 2 (edge build) is a full nested `for i` x `for j` (with `if (i == j) continue`), scanning `b->reads` against `writes_a`, `break` after the first match -> O(N^2 * R), at most one edge per ordered (i,j).
- `kahn_topological_sort` step 3 (Kahn layering) rescans all N each phase -> O(P * N); P (phase count) plateaus at 27 in the measured data, so it is not the dominant term.
- Measured (by-report): `compute_static_graph` fits O(N^2.05-2.10); the knee where it crosses the 130 s watchdog is ~90k systems; the curve is smooth (no cliff) -> compute-bound, NOT a deadlock. The "native mutex above 90k" hypothesis in the S1 comment is REFUTED: there is no lock on the path.

**Defect (a) -- the concurrency race, verified.**
- `[RV]` `default_scheduler_graph()` is a lock-free Meyers singleton (`static SystemGraph instance; return instance;`); `SystemGraph` has NO mutex member; `clear()` frees the backing store (`systems_.clear()`).
- `[RV]` The header documents the design contract: single-threaded registration / compute; concurrent reads of already-computed phase composition are safe (immutable post-compute); one kernel scheduler per process. The header also notes tests MAY create their own `SystemGraph` instance to bypass the singleton -- but the C ABI always routes to the singleton and the managed interop exposes no instance creation, so managed tests can only isolate by serialisation.
- `[RV]` `SchedulerExtremeTests` carries `[Collection("ExtremeSerial")]` with a comment claiming it forces "isolation from other classes". That is the misconception: an xUnit collection serialises only WITHIN itself; against a different collection its members still run in parallel. `SchedulerStressTests` is in the default (unnamed) collection -> the two classes run concurrently.
- `[RV]` `SchedulerExtremeTests.ResetAllGlobals()` (called in BOTH the ctor and Dispose) calls `SystemGraphInterop.Clear()`, `WakeRegistryInterop.Clear()`, `SchedulingPoliciesInterop.Clear()`, `EventTypeRegistryInterop.ClearForTesting()` -- so every Extreme ctor/Dispose mutates the process-global singletons concurrently with `SchedulerStressTests` iterating `systems_` in its tick loop. That is the race. (The author added `ResetAllGlobals` to close cross-test pollution and thereby created it.)
- `df_scheduler_tick_begin` touches `default_wake_registry()` and `default_scheduler_graph()` with no synchronisation, wrapped in `catch(...)` (which cannot catch a Windows access violation). It does NOT use the thread pool -> the thread pool and phase barrier are NOT on the crash path.
- Measured (by-report): reproduced three ways -- a deterministic out-of-repo harness (3/3 crash, `0xC0000005` access violation at `coreclr.dll+0x1d45aa` and `0xC0000374` heap corruption at `ntdll.dll`); an in-suite soft manifestation (`SystemCount` 5000 -> 1758; bus count 5000000 -> 4375000); and an in-suite HARD testhost crash 3/4 attempts, even with zero external load, at the identical `coreclr.dll+0x1d45aa`. Baseline: `SchedulerStressTests` alone is 1/1 green (isolation confirmed).

**S2 mis-grouping, verified.**
- `[RV]` S2 (`S2_ParallelSystemScheduler_TwoHundredThousandTicks_SteadyStateStable`) is a MANAGED `ParallelSystemScheduler` marathon: 80 systems built once in setup, then a long tick loop that walks a pre-built phase list. Its own comment states it does NOT stress graph rebuild. It probes TPL phase-dispatch stability, not native scale. Defect (b) does not unblock it. (Minor internal inconsistency noted: the method name says 200k ticks, one comment says 50k -- immaterial to the disposition.)
- `[RV]` S1 (`:132`), S2 (`:240`), S7 (`:796`) each carry `[Fact(Skip = "F-29(b): ...")]`.

**Census pins (canonical -- govern over any stale figure).**
- Reserved-surface census on `src/**/*.cs`: reserved-stub sites 34, marker families 13 (the "34/13" pin); five marker families; DFK-WAIVER 2. These are the regression anchor. The native (b) rewrite is in `native/`, not `src/`, so it moves no `src/` pin. The (a) managed enum addition is the only `src/` touch and must move no reserved-surface pin.

## 3. Phase 0 -- preconditions and checkpoint [CORE]

Run serially by the orchestrator before any commit.

1. **Verify recon facts** -- the section 2 `[RV]` set: HEAD `e628e16`, clean tree,
   `register_version` "2.22", `TESTING_STRATEGY` 2.1.0, the F-ledger states, the
   current O(N^2) code shapes in `system_graph.cpp`, the lock-free singleton and
   absent mutex member, the `[Collection("ExtremeSerial")]` on `SchedulerExtremeTests`
   and `ResetAllGlobals` contents, S1/S2/S7 Skip attributes, and the S2 managed
   identity. Any mismatch -> HALT H1.

2. **Baseline gates** -- full managed + native build and the full test run (commands
   per `DEVELOPMENT_HYGIENE`; every `dotnet test` via the section 8 no-pipe harness
   with a watchdog, TRX as truth). **Expected baseline shape:** S1/S2/S7 SKIP; the
   rest of the scheduler suite is subject to the LIVE defect (a) race and is
   therefore known-flaky -- a normal full-suite run is usually green but may show a
   soft failure or a testhost crash when `SchedulerStressTests` collides with a
   `SchedulerExtremeTests` ctor/Dispose `Clear()`. This is the defect under repair,
   NOT a halt. Run the suite 2-3 times to characterise the flake and record it as
   the regression anchor. Closure must produce a GREEN, REPEATABLE suite with S1
   and S7 passing -- strictly better than a flaky baseline. A genuinely NEW failure
   unrelated to F-29 -> HALT H2.

3. **Enumerate the shared-singleton test classes** -- grep `tests/` for every class
   that calls a mutation or `Clear`/`ClearForTesting` on a shared native singleton
   interop: `SystemGraphInterop`, `WakeRegistryInterop`, `SchedulingPoliciesInterop`,
   `EventTypeRegistryInterop`, and the native bus interop / `ManagedBusBridge`. The
   resulting set is the exact membership of the D3 shared collection. This
   enumeration IS the (a-i) work order; record it in the closure report. Note the
   related pollution gap the recon surfaced: `SchedulerStressTests.Dispose` does not
   call `ManagedBusBridge.ClearForTesting` -- fold that into the D3 isolation work if
   it affects membership.

4. **Validation checkpoint**:
   `powershell -NoProfile -ExecutionPolicy Bypass -File tools\governance\sync_register.ps1 -Validate`.
   Exit code != 0 -> HALT H3. The refreshed `VALIDATION_REPORT.md` lands inside C1.

5. **REGISTER enum read** (Lesson #N14): extract the empirically-used values for
   category / tier / lifecycle and the exact `DOC-` and `EVT-` entry shapes from
   `REGISTER.yaml`. These verbatim shapes are the ONLY sanctioned templates for the
   section 12 REGISTER cascade. Never invent an enum value (HALT H5). Recall the
   standing convention: a DOC-D brief and a DOC-E recon report enroll Draft ->
   EXECUTED, and a tier-3 + LOCKED/EXECUTED combination requires
   `special_case_rationale`.

6. **Mandatory reads** before any commit: the recon report (full); `system_graph.cpp`
   + `system_graph.h`; `SchedulerExtremeTests.cs` + `SchedulerStressTests.cs`;
   `wake_registry.cpp`, the native bus singleton source, and the `df_scheduler_*`
   wrappers in `capi.cpp` (to place the D2 guard and translate its code);
   `SystemGraphInterop.cs` and the sibling interops (for the `ComputeResult` enum and
   the additive violation value); `selftest.cpp` (for the DF_CHECK style);
   `TESTING_STRATEGY.md` sections 2.6, 8, 9.2; the `METHODOLOGY` session-closure
   protocol. Confirm each was read.

NEVER run `-Sync` outside the ratified REGISTER cascade. `render_register.ps1` runs
exactly once, at the render commit. The executor NEVER pushes -- pushes are the
operator's manual step after closure.

## 4. Topology [CORE]

**Single orchestrator, no wave.** The recon report is the survey substrate -- the
code surface is already mapped and the root causes are already found and
verified, so no read-only survey wave is needed. The file set is bounded and
interdependent (the native rewrite, the native guard, the test collection, the
selftest, the quarantine flip, and the governance closure form one dependency
chain), which suits serial execution. Only the orchestrator runs `git add`/`commit`
(atomic-commit discipline). `docs/ROADMAP.md` and `REGISTER.yaml` are
orchestrator-only single-writer files. No agent touches any out-of-scope or
reference tree (see section 15).

## 5. Wave R -- survey agents [KIND: phase-execution]

None. The F29_NATIVE_SCHEDULER RECON REPORT already discharged the survey; its
verified facts (section 2) are the code-truth substrate for execution. The
executor still re-verifies against the live files at Phase 0 and before each
rewrite (code is truth), but spawns no survey agents.

## 6. Checkpoints [CORE]

Serial self-audits the orchestrator runs (no multi-agent checkpoints):

- **After C2 (shared collection):** run the full suite at least 5 times. Expected:
  green and stable every run (serialisation removes the concurrency). Any residual
  soft failure or crash means the section-3.3 enumeration missed a singleton-touching
  class -> add it to the collection and re-run. This is the completeness gate for the
  test half of defect (a).
- **After C3 (native guard):** run the full suite again. Because the classes are now
  serialised, the guard must stay dormant (never fire) and the suite must be green.
  If the guard fires (a test reports the concurrency-violation code), that positively
  names a still-concurrent singleton-touching path the collection missed -> add it.
  Green-under-the-guard is the POSITIVE proof the collection is complete (absence of
  a flake, made observable).
- **After C4 (index-keyed rebuild):** behaviour-preservation audit -- every
  pre-existing scheduler test that exercises phase composition, write-conflict
  detection, or cycle detection must produce identical results (same phases, same
  deterministic sorted ids, same error codes, same `last_error` text). A behaviour
  delta that is not the sanctioned first-conflict-determinism note (D1) -> HALT
  H(preserve).
- **Governance self-audit (C7):** truth law -- no enforcement claim in the
  `TESTING_STRATEGY` edit without an on-disk enforcer (the shared collection and the
  native guard ARE the enforcers of the isolation law); citation-form compliance (cite
  by anchor and stable ID, no living-doc version pins, no URL anchors); the change
  stays within the ratified scope (section 8). Violations unresolvable without an
  architectural decision -> HALT H6.

## 7. Execution / writer specifications [CORE]

Global laws by reference: truth law (no enforcement verb without an on-disk
enforcer; future capability only as `Planned -- see ROADMAP.md` pointers);
citation-form (cite by anchor and stable ID, no version pins, no URL anchors);
recon classifications are the work order, code is the truth (re-verify against the
live file before writing). The `TESTING_STRATEGY` edit carries its frontmatter per
the REGISTER mirror shape and ends with its Amendment-protocol and Change-history
sections.

### D1 -- Index-keyed graph rebuild (defect b), native-only, behaviour-preserving

Target: `native/DualFrontier.Core.Native/src/system_graph.cpp` (+ any member
declarations in `include/system_graph.h`). Replace the three quadratic sites with
index-keyed / work-queue equivalents that produce byte-identical output.

**(1) O(1) duplicate check in `register_system`.** Add a private
`std::unordered_set<uint32_t> system_ids_` member kept in lock-step with
`systems_`: insert on a successful `register_system`, erase in `unregister_system`,
clear in `clear()`. Replace the linear `for (const SystemEntry& s : systems_) ...`
duplicate scan with `if (system_ids_.count(system_id) != 0) return false;`. Result:
registration amortised O(1); N registrations O(N).

**(2) Component-indexed conflict + edge detection in `kahn_topological_sort`.**
Build once, in a single pass over `subset`, a
`std::unordered_map<uint32_t, std::vector<std::size_t>> writer_of` mapping each
written component id to the subset indices that write it.
- Write-write conflict: any component whose `writer_of` entry has size > 1 is a
  conflict -> return -1 with a `last_error` in the exact current format
  (`[SCHEDULER ERROR] Write conflict: <fqn_a> and <fqn_b> both write component type <w>`).
  PRESERVATION: the current nested `i < j` scan reports the FIRST conflict in
  (i, j, first-w) order. Preserve deterministic selection: report the conflict whose
  (lower writer index, higher writer index, component id) tuple is smallest, matching
  what the old loop would surface. See the preservation note below.
- Edges A -> B (B reads what A writes): for each subset system B at index j, for
  each component r in `B->reads`, look up `writer_of[r]`; each writer index i != j
  contributes edge i -> j. Match the current "at most one edge per ordered (i,j)"
  semantic (the old code `break`s after the first matching read): use a per-j
  `visited writers` set so a given (i,j) increments `in_degree[j]` exactly once.
  This keeps every `in_degree` value identical to the current build, which keeps the
  Kahn layering identical. Complexity ~O(N + E + total reads/writes).

**(3) Layer-wise BFS Kahn.** Replace the O(P * N) per-phase full rescan with a
layer-wise BFS that preserves the current per-layer phase emission: the current
layer is every not-yet-visited index with `in_degree == 0`; emit it as one phase
(ids sorted ascending, as now); decrement `in_degree` of each neighbour; collect
the newly-zero indices into the next layer; repeat. If a layer is empty while
unvisited systems remain -> cycle, return -2 with the current `last_error` format
(`[SCHEDULER ERROR] Cyclic dependency detected -- remaining systems: <fqns>`) over
the remaining set. Each node and edge is processed once -> O(N + E). Because phases
are still emitted per BFS layer with the same ascending id sort, the phase
composition is byte-identical to the current output.

**Preservation contract (the acceptance for D1):** identical static and per-tick
phase composition, identical ascending id sort within each phase, identical return
codes (1 / 0 / -1 / -2), identical `last_error` text for conflict and cycle. The
ONE sanctioned behaviour caveat is the specific pair named in a write-conflict
message when multiple conflicts exist: Phase 0 must grep the scheduler tests for
any assertion pinning the exact conflicting pair or the full conflict message; if
one exists, preserve the old first-conflict selection so it still passes; if none
does, the deterministic-smallest-tuple selection above is acceptable. Do NOT edit a
test to accommodate a behaviour change -- the rewrite is behaviour-preserving by
mandate (HALT H(preserve) on any other delta). No native locking is added in D1;
`system_graph` stays single-threaded by design.

### D2 -- Fail-loud concurrency detector (defect a, native half)

Targets: `SystemGraph`, `WakeRegistry`, and the native bus singleton (native
source + headers), plus the managed interops in `src/DualFrontier.Core.Interop/`.

This is a DETECTOR, not a lock. It does not serialise legitimate sequential
cross-thread use (register on one thread at load, tick on another thread later is
fine); it only rejects genuinely CONCURRENT entry, turning the silent
heap-corruption of the current contract violation into an immediate, visible
failure. It preserves the "concurrent reads of already-computed phases are safe"
property by guarding only the mutation / compute entry points, never the read-only
phase accessors.

**Mechanism.** Give each guarded singleton a private `std::atomic<bool> busy_`
(default false) and a small RAII scope guard whose constructor does an
acquire-or-fail compare-exchange (`false -> true`) and whose destructor stores
`false` on the path where it acquired. At the top of each guarded method,
construct the guard; if it did NOT acquire (another thread is inside), the method
must NOT touch shared state -- it sets `last_error` to a clear concurrency-violation
message and returns a new distinct violation code. Apply to the state-touching
entry points: `SystemGraph::register_system` / `unregister_system` / `clear` /
`compute_static_graph` / `compute_per_tick_graph`; the equivalent mutation /
drain / fire entry points on `WakeRegistry`; the equivalent mutation / publish /
reset entry points on the bus singleton. Leave the pure read accessors
(`static_phase_*`, `per_tick_phase_*`, `last_error`, `get_phase_barrier`)
unguarded. The `busy_` exchange is a single atomic op per call -- negligible even
on the per-tick path, which already does O(N+E) work.

**ABI + managed surface (additive).** Introduce one new negative return code for
"concurrency violation" distinct from -1 (conflict) and -2 (cycle); pick the next
free negative value and document it in the `system_graph.h` return-code comment and
wherever the sibling singletons document their codes. Translate it through the
`capi.cpp` `df_scheduler_*` wrappers unchanged. On the managed side add one new
`SystemGraphInterop.ComputeResult` enum value (and the equivalent on the wake / bus
interops if they surface a result enum) mapping the new code; the interop must
surface it as a distinct, non-swallowed outcome so a real production violation is
visible. This is a strictly additive ABI change (a new return value + a new enum
member) -- append-only, no existing value renumbered.

**Acceptance:** with the D3 collection in place the guard stays dormant and the
suite is green; a deliberate concurrent-entry selftest (D4) shows the loser
receives the violation code and shared state is untouched (no corruption).

### D3 -- Shared xUnit collection for singleton-touching classes (defect a, test half)

Target: `tests/DualFrontier.Core.Tests/**`. Put every class enumerated at Phase 0
(section 3.3) into ONE shared xUnit collection (a single `[Collection("...")]`
name, e.g. `SharedNativeSingleton`, backed by a collection-definition class) so no
two of them run in parallel. Remove or correct the misleading comment on the
existing `[Collection("ExtremeSerial")]` that claims cross-class isolation; if the
Extreme class joins the shared collection, retire `ExtremeSerial` in favour of the
shared name. Fold the `SchedulerStressTests.Dispose` / `ManagedBusBridge.ClearForTesting`
gap from section 3.3 if it bears on correctness.

**Acceptance:** the full suite is green and repeatable (checkpoint after C2: >= 5
clean runs); under the guard (checkpoint after C3) it stays green, positively
proving the collection is complete. This is the fix that makes the suite pass; the
D2 guard is the durable net that makes an incomplete collection fail loud rather
than silently.

### D4 -- Native selftest scenarios (concurrency-detection + scale)

Target: `native/DualFrontier.Core.Native/test/selftest.cpp`, in the existing
DF_CHECK style.
- **Concurrency-detection scenario:** drive two threads into a guarded entry point
  of one singleton concurrently (e.g. one thread in a long `compute_static_graph`
  over a non-trivial graph while another calls `clear`); assert that the concurrent
  caller receives the new violation code (detected) and that shared state is not
  corrupted -- i.e. the guard converts the former crash into a clean, observable
  rejection. The scenario must be deterministic enough to pass reliably in CI (bias
  toward forcing the overlap, not relying on chance).
- **Scale scenario:** register 50k+ systems in a forward-only DAG and run
  `compute_static_graph` (and a bounded per-tick recompute), asserting completion
  within a generous wall-clock budget that the old O(N^2) rebuild could not meet.
  This is the empirical proof of D1; keep the budget loose enough to tolerate machine
  noise while still failing on a quadratic regression.

### D5 -- Un-quarantine S1 + S7; re-point S2 to F-30

Target: `tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs`.
- Remove the `[Fact(Skip = "F-29(b): ...")]` attribute from S1 and S7 (leaving them
  as plain `[Fact]`). Defect (b) blocked them; with D1 they complete, and under the
  D3 collection + D2 guard they run serialised without racing. Keep their
  `[Trait("Category", "Extreme")]`. Update the stale O(N^2)/mutex hypothesis text in
  the S1 leading comment to state the resolved cause (O(N+E) index-keyed rebuild;
  the "mutex above 90k" hypothesis was refuted).
- S2 stays skipped but its Skip reason is re-pointed from F-29(b) to the new finding
  F-30, since S2 is a managed `ParallelSystemScheduler` marathon, not a native-scale
  scenario. Use a reason string that names F-30 and states the true cause, for
  example: `Skip = "F-30: managed ParallelSystemScheduler marathon (200k-tick TPL steady-state) -- not native-scale; disposition tick-trim vs opt-in marathon pending. See docs/ROADMAP.md F-30."`

**Acceptance:** S1 and S7 pass in the Extreme pass; S2 remains skipped under F-30;
the Extreme suite moves from 3 skipped to 1 skipped with no new failure.

### D6 -- TESTING_STRATEGY: honesty-register refresh + isolation law

Target: `docs/methodology/TESTING_STRATEGY.md`, 2.1.0 -> 2.2.0 (MINOR: additive law
+ register refresh, no breaking change). Ground the CURRENT live 2.1.0 content
before editing (do not assume from memory). Two edits, exact text in Appendix A:
- **Section 2.6 honesty register:** update the Skip census -- S1 and S7 are no
  longer skipped (defect b resolved); S2 is skipped under F-30 (managed marathon),
  not F-29(b). Remove the refuted "native mutex above 90k" framing and record the
  true cause (O(N^2) compute, now O(N+E)) and that defect (a) was a scheduler + wake
  + bus singleton race, not a "bus TickBegin path".
- **A new shared-native-singleton isolation law** (a subsection under section 8, the
  invocation / isolation laws, or its own numbered subsection): state that any test
  class touching a shared native singleton (scheduler graph, wake registry,
  scheduling policies, event-type registry, native bus) MUST join the single shared
  xUnit collection so it cannot run in parallel with another such class; note that
  the native fail-loud concurrency detector enforces this structurally (a violation
  returns the violation code rather than corrupting memory). Cite the shared
  collection and the guard as the on-disk enforcers (truth law).
- Add the 2.2.0 row to the section 9.2 version history.

### D7 -- Governance closure

Covered in sections 8, 11, 12. F-ledger write-back (F-29 -> CLOSED with real
hashes, F-30 seeded) and the REGISTER closure (2.22 -> 2.23) are orchestrator-only.

## 8. Governance-closure machinery [KIND: governance]

The load-bearing exact text. Ground each target's CURRENT live content before
computing a bump; never assume from memory.

**F-ledger (`docs/ROADMAP.md`).**
- **F-29 -> CLOSED.** Fill its resolution with the real commit hashes of the code
  commits (C2-C6) once they exist. The resolution states: defect (b) fixed --
  `system_graph` rebuild re-expressed index-keyed / work-queue, O(N^2) -> O(N+E),
  behaviour-preserving, proven by the selftest scale scenario and the un-quarantined
  S1/S7; defect (a) fixed -- the single-threaded contract is now self-enforcing via a
  native fail-loud concurrency detector on the scheduler / wake / bus singletons plus
  a shared xUnit collection serialising the singleton-touching test classes; the
  refuted "native mutex above 90k" hypothesis and the "bus TickBegin path"
  mis-attribution are corrected. Reference the lesson candidate: an xUnit
  `[Collection]` serialises only within itself, so cross-class isolation of a shared
  global requires one shared collection -- now codified in `TESTING_STRATEGY` and
  enforced by the guard.
- **F-30 -> NEW OPEN.** Seed a new finding row: S2 (`S2_ParallelSystemScheduler_...`)
  is a managed `ParallelSystemScheduler` steady-state marathon (a long TPL tick loop
  over a pre-built 80-system phase list), NOT a native-scale scenario; it does not
  complete within the CI budget for reasons unrelated to the native graph. Status
  OPEN; disposition (tick-trim to a CI-affordable count vs accept as an opt-in
  marathon excluded from the default sweep) is architect-owned. Skipped meanwhile
  under F-30.

**`TESTING_STRATEGY.md` 2.1.0 -> 2.2.0.** The section 2.6 refresh and the new
isolation-law subsection, verbatim, are in Appendix A. Add the section 9.2 history
row: `2.2.0 -- F-29 closure: shared-native-singleton test-isolation law codified;
honesty register refreshed (S1/S7 un-quarantined, S2 re-pointed to F-30, refuted
mutex hypothesis removed).`

## 9. S-LOCK invariants [CORE]

**New S-LOCK: single-threaded scheduler-singleton access is now structurally
enforced.** The invariant -- no concurrent access to the process-global scheduler /
wake / bus singletons -- is enforced by three coupled mechanisms: (1) the native
fail-loud concurrency detector (code: any concurrent entry returns the violation
code, never corrupts); (2) the shared xUnit collection (test: singleton-touching
classes cannot run in parallel); (3) the codified `TESTING_STRATEGY` isolation law
(governance). The guard makes the invariant self-checking: an incomplete collection
surfaces as a loud violation, not a silent flake.

**Analyzer-rule candidate (deferred, K-L20 family):** a Roslyn rule that flags a
test class calling a shared-native-singleton interop mutation without membership in
the shared collection would enforce (2) at compile time. This is a candidate for the
analyzer queue, NOT built in this cascade -- recorded here so the invariant has a
future structural home.

## 10. Census discipline [CORE]

The reserved-surface census runs on `src/**/*.cs`. HARD pins (syntax-anchored,
exact count is the invariant): reserved-stub application sites = 34; marker
families = 13 (the "34/13" pin); DFK-WAIVER = 2. The `CensusMetaTests` assert these
exactly.

**What this cascade touches in `src/`:** only the D2 additive managed enum value
(`ComputeResult` and any sibling result enum) in `src/DualFrontier.Core.Interop/`.
An added enum member is neither a reserved-stub site nor a marker family, so it
moves NO HARD pin. Verify empirically: run the census grep expression over
`src/**/*.cs` before and after the enum edit and confirm 34/13 and DFK-WAIVER 2 are
unchanged, and that `CensusMetaTests` stay green. If a pin moves unexpectedly
(e.g. a coincidental marker word in a new comment) -> investigate and correct
before committing; do not adjust a pin to match. The D1 native rewrite, the D4
selftest, and all test-file edits (D3, D5) are outside `src/` and move no `src/`
pin. No SOFT pin is expected to move; if a vocabulary count shifts, record it as a
census-delta in the closure report, not as a finding.

## 11. Commit plan [CORE]

One row per atomic commit in dependency order; each passes the gates. Commit count
is intended-form -- a defect-iteration or a needed split may add a commit (record
the deviation in the closure report; do not compress history to match the table).
The ordering lands the test-isolation fix FIRST so every intermediate suite state
is green: serialising the classes (C2) stabilises the suite before the guard (C3)
and the rewrite (C4) land on a stable base, and before the quarantine flip (C6)
depends on the rebuild.

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(register): enroll F29_NATIVE_SCHEDULER brief + recon report + validation checkpoint` | brief + recon report file + refreshed VALIDATION_REPORT |
| C2 | `test(scheduler): shared xUnit collection for singleton-touching classes` | D3 -- Phase-0-enumerated classes into one collection; suite green + stable |
| C3 | `fix(native-scheduler): fail-loud concurrency detector on shared singletons` | D2 -- native atomic busy-flag guard on scheduler/wake/bus + additive code + managed enum |
| C4 | `perf(native-scheduler): index-keyed graph rebuild (O(N^2) -> O(N+E))` | D1 -- writer_of index + O(1) register dedup + layer-wise BFS Kahn, behaviour-preserving |
| C5 | `test(native-scheduler): selftest concurrency-detection + scale scenarios` | D4 -- two DF_CHECK scenarios proving the guard and the rebuild |
| C6 | `test(scheduler): un-quarantine S1 + S7; re-point S2 to F-30` | D5 -- remove S1/S7 Skip; update S1 comment; S2 Skip -> F-30 |
| C7 | `docs(testing-strategy): isolation law + honesty-register refresh (2.1.0 -> 2.2.0)` | D6 -- section 2.6 refresh + new isolation law + section 9.2 row |
| C8 | `governance(register): F29_NATIVE_SCHEDULER REGISTER closure (2.22 -> 2.23) + F-ledger write-back` | REGISTER mutations + F-29 CLOSED / F-30 seeded + validate folded |
| C9 | `governance(register): render regeneration + header backfill` | render_register.ps1 run once + Option-B header self-reference backfill |

## 12. REGISTER cascade [CORE]

Using ONLY the Phase 0 verbatim enum shapes (never invent -- HALT H5).
- **Enroll the recon report** `docs/reports/F29_NATIVE_SCHEDULER_RECON_REPORT.md`
  as DOC-E, tier 3, lifecycle EXECUTED, with `special_case_rationale` (the standing
  recon-report convention: a tier-3 EXECUTED artifact requires the rationale).
- **Enroll this brief** `DOC-D-F29_NATIVE_SCHEDULER_BRIEF`, category D, tier 3,
  lifecycle Draft -> EXECUTED at closure (the brief FILE frontmatter is hand-authored,
  not register-synced; the register entry follows the D-brief convention).
- **Version bump mirror:** `TESTING_STRATEGY.md` 2.1.0 -> 2.2.0 recorded in its
  REGISTER entry and its frontmatter mirror.
- **Document-count delta:** +2 enrolled documents (recon report + brief) unless the
  recon report was already counted at recon time -- reconcile against the live
  `REGISTER.yaml` count, do not assume.
- **One audit-trail EVT** entry carrying the real hashes of C1-C8 (the code +
  governance commits), authored at C8 after those hashes exist.
- `register_version` 2.22 -> 2.23.
- Validate exit 0 is mandatory (HALT H3); fix only within the empirical enum
  vocabulary (HALT H5). `TESTING_STRATEGY` is the only standing doc whose version
  moves; no other doc's lifecycle or version changes.

## 13. Halt conditions (H-series) [CORE]

Stop and surface rather than improvise:
- **H1** precondition mismatch (section 3.1 `[RV]` set).
- **H2** build/test regression vs the Phase 0 baseline -- a NEW failure unrelated to
  F-29 (the known-flaky defect-(a) baseline is not itself a halt; see section 3.2).
- **H3** `-Validate` nonzero.
- **H4** recon material contradiction -- a live-code fact contradicts a section-2
  `[RV]` fact the rewrite depends on (beyond an explained measurement-method delta).
- **H5** a REGISTER enum value is needed that the Phase 0 vocabulary lacks --
  escalate, never invent.
- **H6** truth-law unsatisfiable without an architectural decision (e.g. the
  isolation law cannot be stated with a real on-disk enforcer).
- **H(preserve)** the D1 rewrite produces any observable behaviour delta other than
  the single sanctioned first-conflict-determinism caveat -- do not "fix" a test to
  match; the rebuild is behaviour-preserving by mandate.
- **H(governance)** any semantic change to `TESTING_STRATEGY.md` beyond the ratified
  scope (section 2.6 refresh + the new isolation-law subsection + the 9.2 row).
- Standing rails: no pushes to origin; no `-Sync` outside the REGISTER cascade; no
  history rewrite / force-push / squash; `docs/ROADMAP.md` and `REGISTER.yaml` are
  single-writer (orchestrator only).

On halt: stop, report state verbatim, await the operator. In-session
re-confirmation before resuming is expected behaviour (the auto-mode push-to-main
block is expected, not a fault).

## 14. Closure protocol and report [CORE]

Execute the `METHODOLOGY` session-closure protocol: (a) F-ledger write-back
(F-29 CLOSED, F-30 seeded -- never chat-only); (b) REGISTER mutations + `-Validate`
folded into C8; (c) `render_register.ps1` once + Option-B header self-reference
backfill at C9; (d) the closure report.

The closure report (chat) carries:
- **Commits table:** hash | subject, for C1-C9 (real hashes read from
  `.git/logs/HEAD` after the executor commits; the operator pushes).
- **Versions table:** before -> after, including `register_version` 2.22 -> 2.23 and
  `TESTING_STRATEGY` 2.1.0 -> 2.2.0.
- **Census pins:** the HARD pins re-attested exact (34 / 13, DFK-WAIVER 2) with the
  grep recipe; any SOFT delta recorded.
- **F-ledger final-state table:** F-29 CLOSED (with the resolution hashes), F-30 NEW
  OPEN.
- **Shared-singleton collection membership:** the section-3.3 enumeration as
  executed (the classes placed in the shared collection).
- **`Skeleton revisions` list:** every deviation from this brief's intended forms
  (e.g. the chosen violation code value, the collection name, any split commit, the
  first-conflict-determinism decision).
- **Gates table:** baseline (known-flaky) vs closure (green + stable, S1/S7 passing)
  -- must match-or-better.
- **Self-attestation:** no pushes; single render run; no `-Sync` outside the
  cascade; no history rewrites; reference / HISTORICAL trees untouched; only the
  enumerated files changed.
- **Operator manual checklist:** push C1-C9; ratify the brief and recon-report
  lifecycle states in REGISTER; the standing F-queue items that remain
  operator-owned (F-30 disposition, and the register-tooling and K-L20 cascades).

## 15. Out of scope [CORE]

Named so the executor does not wander in:
- **S2's actual fix.** This cascade only re-points S2's Skip to F-30. Whether to
  tick-trim S2 or accept it as an opt-in marathon is architect-owned (F-30).
- **The register-tooling cascade** (F-2 PENDING-COMMIT backfill, F-13 render-script
  defect) -- a separate cascade; F-13's render behaviour is taken as-generated here.
- **The K-L20 analyzer-rule family.** The shared-singleton collection rule is
  recorded as a candidate (section 9) but is NOT built in this cascade.
- **F-28(b)** analyzer-stub sweep -- separate.
- **A full native stack capture of the defect-(a) crash.** The recon left it as a
  documented tooling gap; the D2 guard makes the crash non-reproducible (it becomes a
  clean rejection), so a native debugger session is unnecessary. Do not add debugger
  tooling here.
- **`TESTING_STRATEGY.md` changes beyond the ratified scope**; any other standing
  doc's version or lifecycle.
- **Pushes to origin**, the reference tree, and any EXECUTED-doc content beyond the
  sanctioned governance edits.

## Appendix A -- TESTING_STRATEGY 2.2.0 edit content [KIND: governance]

The executor MUST ground the current live 2.1.0 content before applying these
(section 3.6 mandatory read). A2 is a wholly-new subsection given verbatim; A1 is an
edit spec against text the executor reads first (the current 2.6 wording is not
reproduced here because it must be grounded live, not from memory).

=== A1: section 2.6 honesty-register refresh (edit spec) ===
Apply these targeted changes to the current section 2.6 Skip / quarantine census,
preserving its existing structure and voice:
- S1 (`S1_NativeGraph_FiftyThousandSystems_...`): REMOVE from the skipped list --
  it is now a passing `[Fact]` (defect b resolved).
- S7 (the 250k-system scenario): REMOVE from the skipped list -- now passing.
- S2 (`S2_ParallelSystemScheduler_...`): MOVE its entry from "skipped under F-29(b)"
  to "skipped under F-30", and change its stated cause to: a managed
  `ParallelSystemScheduler` steady-state TPL marathon over a pre-built phase list --
  not a native-scale scenario.
- Any statement attributing the F-29 scale wall to a "native mutex above 90k" or
  the crash to a "bus TickBegin path": REPLACE with the verified causes -- an O(N^2)
  compute in the graph rebuild (now O(N+E)); and a data race across the scheduler,
  wake-registry, and bus process-global singletons.
Intended final state: section 2.6 lists exactly the still-skipped scenarios with
accurate causes; no refuted hypothesis remains; F-30 appears as the S2 owner.

=== A2: new subsection -- shared-native-singleton test-isolation law (verbatim) ===
BEGIN
Shared-native-singleton test isolation. The native kernel exposes several
process-global singletons -- the system-scheduler graph, the wake registry, the
scheduling-policies registry, the event-type registry, and the native event bus.
Their design contract is single-threaded registration and compute; concurrent reads
of already-computed, immutable state are safe, but concurrent mutation or compute is
undefined behaviour. Any test class that mutates one of these singletons (including
calling its Clear / reset for isolation) MUST join the single shared xUnit
collection reserved for them, so that no two such classes ever run in parallel. An
xUnit [Collection] serialises only its own members; placing two singleton-touching
classes in different collections lets them run concurrently and corrupt shared
native state. This law is enforced structurally on two fronts: the shared collection
prevents the parallel schedule, and the native fail-loud concurrency detector on
each singleton returns a distinct violation code (rather than corrupting memory) if
concurrent entry ever occurs -- so an incomplete collection surfaces as a loud,
localised test failure naming the offending path, not a silent heap corruption.
END

---

**End of F29_NATIVE_SCHEDULER_BRIEF.md v1.0**