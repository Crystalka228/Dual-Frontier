---
register_id: DOC-D-EQ_A1_FAULT_SYMMETRY_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: 'post-EQ_A1 closure'
title: EQ_A1_FAULT_SYMMETRY execution brief (Cascade A of the EQ-a shutdown-law family -- M4 deferred-catch parity + M1 ExecutePhase per-system catch, under the ratified D2 origin-asymmetric policy)
---

# EQ_A1_FAULT_SYMMETRY -- Execution Brief

First engineering cascade off the ratified contracts. Scope: the two managed fault-crossing
points the EQ-a work order names as the lead edge -- M4 (deferred-handler catch symmetry +
fault-sink routing in BOTH delivery modes, `DomainEventBus`) and M1 (`ExecutePhase`
per-system catch + origin dispatch + ELT section 2.3 quarantine, `ParallelSystemScheduler`)
-- implemented under the operator-ratified **D2 origin-asymmetric policy** (section 2).
Done means: no mod fault can kill the process through either bus delivery mode or phase
execution; every fault is recorded with origin context; core faults still fail fast but are
RECORDED before propagating; regression tests pin all of it; gates green.

**Basis**: `docs/reports/EQ_A_SHUTDOWN_RECON_REPORT.md` (2026-07-18, R2/R6/R7 -- cited as
REC-*) + operator ratification of the split and D2 (2026-07-18).
**Executor**: Claude Code (Fable 5), LOCAL. **Branch**: off `main` (= `da63a93` at
authoring). **Register regime**: schema 2.0 (frontmatter SoT; mutation = frontmatter +
`dotnet run --project tools/DualFrontier.Governance -- sync` same commit; gate =
`validate --armed` exit 0; closure EVT = AUDIT_TRAIL.yaml append; PENDING-* outlawed).

**Brief-integration notice**: standing law (CMM, ELT, ROL, TESTING_STRATEGY,
CODING_STANDARDS) cited by anchor, never restated. Brief-vs-law conflict = brief is wrong,
halt. Brief-vs-code conflict = code is truth, record.

## 1. Mission [CORE]

| # | Deliverable | Action |
|---|---|---|
| D1 | `DomainEventBus` fault-crossing parity (M4) | deferred catch added; sync catch re-routed; D2 policy both modes |
| D2 | `ParallelSystemScheduler.ExecutePhase` per-system catch + origin dispatch + quarantine (M1) | first real feeder of the dormant mod-fault route |
| D3 | Regression tests pinning the new law (bus + scheduling suites) | REC-R6.4 placement |
| D4 | The D2 decision durably recorded (EVT; CMM section 7 PATCH if its text carries a pending-decision clause) | governance |
| D5 | Recon report enrolled; closure EVT appended; ROADMAP EQ-a row annotated (Cascade A done) | governance |

## 2. The ratified D2 policy [CORE -- the law this cascade implements]

Origin-asymmetric, uniform across ALL managed fault-crossing points touched here
(`InvokeDeferred`, the sync per-subscriber catch, the new `ExecutePhase` catch):

- **Mod-origin fault** (`SystemOrigin`/captured context identifies a mod): CATCH; route to
  the mod-fault surface (`ModLoader.HandleModFault` -> `ModFaultHandler` -- REC-A8: this
  cascade creates its FIRST real invocation sites); execution CONTINUES (remaining
  subscribers deliver; remaining systems run; the phase completes and `FlushDeferred`
  runs); phase-execution faults additionally engage the ELT section 2.3 quarantine
  semantics TO THE LETTER of that section (ground it at Phase 0; its shape, not this
  brief's paraphrase, is the law).
- **Core-origin fault**: CATCH; RECORD with full context (origin, system/subscriber
  identity, exception) in a structured line on the existing logging surface -- do NOT
  build a new sink abstraction or the ELT section 5 event stream (Cascade B territory);
  then RETHROW -- fail-fast preserved. A half-executed core handler leaves undefined sim
  state; masking it would be a kostyl and would corrupt the defect-rate falsifiability
  signal.
- Consequence acknowledged and ratified: the sync path's current swallow-everything
  behavior CHANGES for core faults (today `Console.WriteLine` + continue -> now record +
  rethrow). This is CMM section 7's own "even the good mode under-reports" clause being
  executed, not a regression.

Fault-party identification: the faulting party is the SUBSCRIBER/SYSTEM whose code threw
-- for deferred delivery the subscription's captured context (REC-R2.4: origin/modId/sink
already in hand); for the sync path and ExecutePhase, ground the identification against
CMM section 7's exact wording at Phase 0. If the law's letter conflicts with this policy
as stated -> HALT H-LAW (never improvise a reconciliation).

## 3. Established facts (REC-*; (R) = re-verify at Phase 0) [CORE]

- (R) `main` = `da63a93`, clean; register 2.0, 346 docs, surface 134; AUDIT_TRAIL 57 EVTs;
  armed validate exit 0; tests 1166/0/5 (F-40 flake shape carved out per its ledger row --
  a single recurrence of exactly `UnloadModNativeState_VacuousUnload_Succeeds` in a full
  run is KNOWN, not H2); live invocable suite count = 10 (REC-A2).
- REC-R2.2: `ExecutePhase` has NO catch (`ParallelSystemScheduler.cs:149-164`); the class
  doc `:30-35` states the deliberate dev-era propagate policy -- that doc MUST be updated
  by M1 to the new law. AggregateException today unwinds to `GameLoop.RunLoop` (no catch)
  = process death; `FlushDeferred` (`:166-167`) is skipped, stranding events.
- REC-R2.2: `BuildContext` already bakes `origin`/`modId`/`_faultSink` into each
  `SystemExecutionContext` (`:248-264`) -- zero new plumbing at the insertion point.
- REC-A8: `ModLoader.HandleModFault` (`ModLoader.cs:303-309`) has ZERO invocation sites
  repo-wide; drain side works and is tested (`ModIntegrationPipeline.Apply`,
  `ModFaultHandlerTests`).
- REC-R2.4: `InvokeDeferred` (`DomainEventBus.cs:169-187`) -- no catch; sync path
  (`:156-166`) -- per-subscriber catch swallowing to `Console.WriteLine` (one of exactly 3
  Console sites in src/). The captured context is pushed around the deferred invoke
  already.
- REC-R2.6: NO existing test throws from a handler in either delivery mode; no test
  exercises a system throwing in `ExecutePhase`. The behaviors being changed are
  UNASSERTED today -- the new tests are first coverage, not rewrites.
- REC-R2.3: `ManagedSystemDispatcher.OnBatch` absorb-to-Debug path is production-dormant
  (0 `SchedulerAdapter.Register` production callers) -- OUT OF SCOPE (section 10).
- REC-R6.4/R6.5: test placement -- bus tests in `Core.Tests/Bus`, scheduler tests in
  `Core.Tests/Scheduling`; trait law (only Stress/Extreme/Integration values); a new test
  touching process-global NATIVE state must join `[Collection("SharedNativeSingleton")]`
  -- expected NOT needed (managed-only surfaces), verify at authoring time.

## 4. Phase 0 -- preconditions [CORE]

1. Verify (R) facts; mismatch -> H1. 2. Baseline gates: full build, full test run (record
shapes; F-40 carve-out), governance suite. 3. `validate --armed` exit 0 -> else H3.
4. Frontmatter-shape read (FRAMEWORK 14.3/14.4 + one LOCKED doc + one AUDIT_TRAIL entry).
5. Mandatory reads: this brief; the recon report IN FULL; **CMM sections 6-7 and ELT
sections 2.3/3/4 IN FULL** (the quarantine and policy letter live there); ROL section 4
(context); TESTING_STRATEGY sections 2.8/3/8; METHODOLOGY closure protocol. 6. Ground the
CMM section 7 wording: does it carry an explicit pending-decision clause for core-origin
policy? YES -> D4 includes a CMM PATCH amendment recording the ratified D2 (pre-authorized,
PATCH-only, exact replacement text presented in the commit body); NO -> the EVT record
suffices; state which branch was taken.

## 5. Execution specification [CORE]

**C2 -- M4 (`DomainEventBus.cs`, single-file src diff + tests).** `InvokeDeferred` gains
the D2 catch (context already pushed); the sync per-subscriber catch is re-routed from
`Console.WriteLine`-swallow to the D2 policy. Tests (new, `Core.Tests/Bus`): deferred
mod-fault -> remaining subscribers deliver + fault reported + no propagation; deferred
core-fault -> recorded + propagates; sync mod-fault -> continue + reported; sync
core-fault -> recorded + propagates (the behavior-change pin); delivery-order/other-
subscriber integrity in both modes. 3-5 tests per REC estimate; assert the RECORDING
surface, not just the propagation shape.

**C3 -- M1 (`ParallelSystemScheduler.cs` + quarantine surface + tests).** Per-system catch
inside the `Parallel.ForEach` body implementing D2: mod-origin -> report through the
`HandleModFault` route + ELT section 2.3 quarantine (its letter governs the skip semantics
and any skip-set artifact -- REC estimates 0-1 new file); core-origin -> record + rethrow
(process still dies, now with a recorded cause; `FlushDeferred` still runs for completed
mod-fault phases). The class doc `:30-35` rewritten to the new law. Tests (new,
`Core.Tests/Scheduling`): mod system throws -> phase completes, siblings ran, fault in
sink, quarantine engaged per the section-2.3 letter, flush ran; core system throws ->
propagates + recorded; quarantine release/persistence semantics as section 2.3 defines
them. 4-6 tests per REC estimate.

**Test discipline**: managed-only fixtures expected; if ANY new test touches a
process-global native singleton it joins the SharedNativeSingleton collection (verify,
state in the report). No new trait values. Invocation per TESTING_STRATEGY section 8.

## 6. Commit plan [CORE]

| # | Subject | Content |
|---|---|---|
| C1 | `governance(eq-a): enroll EQ_A1 brief + EQ-a recon report` | both frontmatter-enrolled + sync + validate --armed |
| C2 | `fix(bus): deferred/sync fault-crossing parity under the D2 origin policy (CMM section 7)` | DomainEventBus + bus tests |
| C3 | `fix(scheduler): ExecutePhase per-system catch, origin dispatch, ELT 2.3 quarantine` | scheduler + tests + class-doc |
| C4 | *(conditional per Phase 0 step 6)* `docs(cmm): record the ratified D2 core-origin policy (PATCH)` | CMM amendment + frontmatter + sync |
| C5 | `governance(closure): EQ_A1 EVT append + ROADMAP EQ-a annotation` | AUDIT_TRAIL + ROADMAP + brief -> EXECUTED + sync + validate |

## 7. Halt catalog [CORE]

H1 precondition mismatch. H2 gate regression beyond the F-40 carve-out. H3 armed validate
nonzero. **H-LAW** CMM section 7 / ELT section 2.3 letter conflicts with the D2 policy or
this brief's paraphrase -- stop, quote both texts, await the operator. H5 frontmatter/
sentinel outside 14.3/14.4. H6 any change beyond the two named surfaces + their tests +
the conditional CMM PATCH (native code, other schedulers, sink-abstraction redesign, ELT
section 5 event stream = all H6). Standing rails: no pushes; no history rewrite;
historical/ read-only; derived registers never hand-edited; AUDIT_TRAIL append-only; no
DFK-WAIVER additions (pin 2 = 2).

## 8. Census discipline [CORE]

DFK-WAIVER HARD pin `rg -n "DFK-WAIVER" src/` = 2 unchanged. `Console.WriteLine` src count
moves 3 -> 2 by design (the bus site re-routed) -- record as census-delta with the verbatim
expression, not a finding. No reserved-stub surface touched.

## 9. Closure [CORE]

METHODOLOGY closure protocol. ROADMAP: EQ-a row annotated (M1+M4 DONE with hashes; M2/M3/
M5-M9 remain; next = Cascade B chartering with D3-D8 deliberation); REC-A1 (ELT 2.6
finalizer misstatement + stale `NativeWorld.cs:25-26` doc) recorded as a Cascade-B rider
note -- NOT fixed here. Closure report: commits, gates baseline-vs-closure, the D2
recording branch taken, census delta, test count delta, self-attestation (standard 2.0
set), operator checklist (push + merge; Cascade B deliberation D3-D8 next; G-RATIO
standing).

## 10. Out of scope [CORE]

Everything in Cascades B/C/D (EngineSession, shutdown transaction, `df_bus_clear`,
Degraded/EngineHealth, the two ABI exports, swapchain, device-lost); the dormant
`ManagedSystemDispatcher.OnBatch` path (0 production callers -- EQ-c/S3 territory; its
comment untouched); F-40 collection treatment (D9, Cascade B-adjacent); `NativeWorld` doc
fix (A1, Cascade B); any sink-abstraction redesign; the NIH tree; pushes/merges.

---

**End of EQ_A1_FAULT_SYMMETRY_BRIEF.md v1.0**
