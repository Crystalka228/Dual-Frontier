---
register_id: DOC-E-EQ_A1_FAULT_SYMMETRY_CLOSURE_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'EQ_A1_FAULT_SYMMETRY CLOSURE REPORT — 2026-07-18 — M4 bus + M1 scheduler fault-crossing parity under the ratified D2 origin-asymmetric policy; ModQuarantine skip-set (ELT 2.3 immediate quarantine); CMM section 7 PATCH recording D2; all gates green, full-sln 1166 -> 1177/0/5'
special_case_rationale: 'Closure report enrolled DOC-E Tier 3 per the docs/reports/ durable-report convention (precedents: DOC-E-STACK_UPDATE_CLOSURE_REPORT, DOC-E-A_PRIME_9_1_ARC_CLOSURE_REPORT). Companion of DOC-D-EQ_A1_FAULT_SYMMETRY_BRIEF section 9; the recon ground truth lives in DOC-E-EQ_A_SHUTDOWN_RECON_REPORT.'
---

# EQ_A1_FAULT_SYMMETRY — Closure Report (2026-07-18)

First engineering cascade off the ratified contracts (EQ-a family, **Cascade A**). Brief:
`tools/briefs/EQ_A1_FAULT_SYMMETRY_BRIEF.md` (DOC-D, EXECUTED at this closure). Recon:
`docs/reports/EQ_A_SHUTDOWN_RECON_REPORT.md` (DOC-E). Executor: Claude Code (Opus 4.8),
LOCAL, on branch `claude/eq-a1-fault-symmetry` off `main` (= `da63a93`). The brief named the
executor as Fable 5; this session ran Opus 4.8 — the operator's choice, recorded, no effect on
execution.

**Done means** (all met): no mod fault can kill the process through either bus delivery mode or
phase execution; every fault is recorded with origin context; core faults still fail fast but are
RECORDED before propagating; regression tests pin all of it; gates green.

## 1. Commits

| # | Hash | Subject |
|---|------|---------|
| C1 | `4d8b8cd` | `governance(eq-a): enroll EQ_A1 brief + EQ-a recon report` |
| C2 | `c72d3b9` | `fix(bus): deferred/sync fault-crossing parity under the D2 origin policy (CMM section 7)` |
| C3 | `4dae07b` | `fix(scheduler): ExecutePhase per-system catch, origin dispatch, ELT 2.3 quarantine` |
| C4 | `df97639` | `docs(cmm): record the ratified D2 core-origin policy (PATCH)` |
| C5 | *(this)* | `governance(closure): EQ_A1 EVT append + ROADMAP EQ-a annotation` |

## 2. What landed (the D2 policy)

**One policy, three catch sites, one definition.** The D2 origin-asymmetric decision lives in a
single method — `SystemExecutionContext.RouteFault(Exception, out string?)` (its `_faultSink`
field's first read site since the K8.3+K8.4 cutover): a mod-origin fault is reported to the
`IModFaultSink` and CONTAINED; a core-origin fault (or a context with no mod identity) is recorded
on the existing logging surface (`System.Diagnostics.Debug.WriteLine`, the ManagedSystemDispatcher
precedent) and RETHROWN — fail-fast preserved in both DEBUG and RELEASE (ELT §4, class 1).

- **M4 — `DomainEventBus` (C2).** `InvokeDeferred` gained the catch it never had (a faulting
  `[Deferred]` handler previously unwound to `RunLoop` and killed the sim thread); `DeliverSync`
  re-routed off its `Console.WriteLine` swallow. Symmetric per-subscriber isolation across both
  delivery modes — the recon's stated top priority.
- **M1 — `ParallelSystemScheduler.ExecutePhase` (C3).** The currently-missing per-system catch with
  origin dispatch, plus the new `ModQuarantine` skip-set (thread-safe, lock-guarded — the
  `ModFaultHandler` pattern) consulted before each `Update`: a faulted mod's systems are skipped on
  subsequent ticks this session (the immediate half of the ELT §2.3 two-stage split). The queued
  reclamation half (`ModIntegrationPipeline.Apply` draining the faulted set at the next menu open) is
  unchanged. This is the first RUNTIME feeder of the mod-fault route.
- **D2 recorded (C4).** CMM §7's "one explicit recorded decision" clause replaced with the recorded
  D2; a "Realized — EQ_A1" bullet closes the "today it is asymmetric" state; version 1.0.0 → 1.0.1,
  LOCKED preserved.

## 3. Ratified decisions (the three forks)

| # | Fork | Ratified | As-built |
|---|------|----------|----------|
| Q1 | ELT §2.3 quarantine depth | **Build the dispatch skip-set now (1 file)** | `ModQuarantine.cs` — mechanism ONLY. The `LogicallyDisabled` desired-state annotation + EngineHealth/Degraded surface (ELT §3.1/§4.1) deliberately NOT built (Cascade B). |
| Q2 | `SystemExecutionContext` edit | **In-scope enabling change** | `RouteFault` internal method + `FaultDisposition` enum. Recorded as a Skeleton revision (C2 body); one file beyond the two named surfaces. |
| Q3 | CMM §7 C4 PATCH branch | **Add the C4 PATCH** | §7 records D2; commit plan C1–C5. Phase-0-step-6 determination: §7 stated a requirement, not a literal pending marker, and D2 does not conflict with its letter (no H-LAW). |

## 4. Gates — baseline vs closure

| Gate | Baseline (`da63a93`) | Closure |
|------|----------------------|---------|
| Build (Release, analyzers on) | 0W / 0E | 0W / 0E |
| Full solution (`dotnet test`, Release) | 1166 / 0 / 5 | **1177 / 0 / 5** (+11 = the 11 new fault tests) |
| Native selftest | ALL PASSED | **ALL PASSED** (exit 0) |
| `validate --armed` | exit 0 | exit 0 (after C1, C4, C5 — every frontmatter-touching commit) |
| CensusMetaTests | 8 / 0 / 0 | 8 / 0 / 0 |

F-40 carve-out: `UnloadModNativeState_VacuousUnload_Succeeds` did **not** recur (0 failures);
5 skips are the unchanged F-30/F-31 Extreme guards (§2.6).

## 5. Census delta (TESTING_STRATEGY §4)

- **`Console.WriteLine` src 3 → 2** — by design; the bus site re-routed. Both survivors in
  `RestrictedModApi.cs`. (One raw grep hit is a comment in a README markdown block, excluded.)
- **`Debug.WriteLine` src 1 → 2** — the core-fault record surface (`SystemExecutionContext`). Not a
  pinned marker; recorded for transparency.
- **DFK-WAIVER 2 = 2** (HARD pin). **`[ReservedStub` 34 / 13 UNCHANGED.** The five marker families
  UNCHANGED (`deferred` 82 / `stub` 51 / `TODO` 136 / `Phase 6` 23 / `not yet` 10). During C2 the
  `deferred` family drifted 82 → 84 on a comment; the comment was reworded back to baseline (the
  `CensusMetaTests` live in `Analyzers.Tests`, not `Core.Tests`, so the exact-pin assertion is the
  guard). No reserved-stub surface touched; `ModQuarantine` is live mechanism, not a stub.

Register: 346 → **348** documents (C1 +2: brief DOC-D Draft + recon DOC-E; C5 +1: this report
DOC-E → **349** post-C5). Authority surface: **134**, unchanged in count (C4 bumped the CMM entry's
version 1.0.0 → 1.0.1). Derived registers regenerated by `sync`, never hand-edited.

## 6. Recorded discrepancies (code-truth wins)

1. **No `ModFault` struct exists.** `ModLoader.HandleModFault(string, ModIsolationException)` forwards
   only `.Message`; routing uses the existing `IModFaultSink.ReportFault(modId, message)` — no new
   payload type was needed.
2. **REC-A8 correction.** The recon states `HandleModFault` has "zero invocation sites repo-wide …
   no test calls it either." True count is zero **production** callers — one test
   (`ModFaultHandlerTests`) already calls it. This cascade adds the first **runtime** feeder; the
   "no test calls it" clause was stale.
3. **`Console.WriteLine` = 3 in compiled `.cs`** (brief correct); the 4th raw hit is a README code
   block.

## 7. Out-of-scope observation (flagged, not acted on)

Two untracked files — `docs/methodology/BRIEF_TEMPLATE.md` and
`docs/methodology/RECON_KICKOFF_TEMPLATE.md` — materialized mid-session (mtime 08:08–08:09; the
df-cascade-authoring skill's bundled scaffolds). They are **not** part of EQ_A1, are excluded from
the register (in-scope count unchanged; `validate --armed` exit 0), and were left **untracked** — no
EQ_A1 commit includes them. Left for the operator to enroll or remove separately.

## 8. Self-attestation (standard 2.0 set)

- Every normative enforcement claim names its on-disk artifact (Truth law): the D2 policy →
  `SystemExecutionContext.RouteFault`; the quarantine → `ModQuarantine` + `ExecutePhase`; the tests →
  `BusFaultIsolationTests` + `SchedulerFaultDispatchTests`; the census → `CensusMetaTests`.
- Code-truth wins: the three §6 discrepancies recorded, not smoothed over.
- No LOCKED invariant weakened — the C4 PATCH STRENGTHENS §7 (fills in a required decision); lifecycle
  LOCKED / tier 1 preserved.
- History append-only; no force-push, no history rewrite; `historical/` untouched; derived registers
  machine-generated only; AUDIT_TRAIL appended, prior entries byte-unchanged.
- H6 respected: changes confined to the two named surfaces + `SystemExecutionContext` (Q2) +
  `ModQuarantine` (Q1) + the C4 CMM PATCH + tests + governance. No native code, no other schedulers,
  no sink-abstraction redesign, no ELT §5 event stream, no EngineHealth/Degraded/desired-state types.
- The executor did not push.

## 9. Operator checklist

- [ ] Push `claude/eq-a1-fault-symmetry` and merge to `main` (executor never pushes).
- [ ] Next: **Cascade B chartering** — the D3–D8 decisions deliberation (EngineSession, shutdown
  transaction, `df_bus_clear`, Degraded/EngineHealth, the two ABI exports, swapchain, device-lost).
- [ ] **Cascade-B rider (REC-A1):** the ELT §2.6 finalizer misstatement + the stale
  `NativeWorld.cs:25-26` "no finalizer backstop" doc line — record for correction in Cascade B.
- [ ] Decide on the two untracked methodology templates (§7).
- [ ] **G-RATIO standing:** the corpus monitor reads 154/344 (44.8%) vs the FRAMEWORK §10 #5 20%
  threshold — report-only, pre-existing, unchanged by this cascade.

---

**End of EQ_A1_FAULT_SYMMETRY_CLOSURE_REPORT.md v1.0**
