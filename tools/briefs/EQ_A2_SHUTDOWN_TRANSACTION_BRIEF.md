---
register_id: DOC-D-EQ_A2_SHUTDOWN_TRANSACTION_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
title: EQ_A2_SHUTDOWN_TRANSACTION execution brief (Cascade B of the EQ-a shutdown-law family -- M3 EngineSession + M2 UnloadAll production caller + M7 Degraded/EngineHealth + M8 df_bus_clear promotion, under ratified D3/D4/D5/D6/D8 + riders D9/D10/REC-A1)
---

# EQ_A2_SHUTDOWN_TRANSACTION -- Execution Brief

Cascade B of the EQ-a shutdown-law family. Implements the world-shutdown TRANSACTION the
ratified contracts demand (ROL section 4, CMM sections 6-7, ELT), seats the reserved K-L20
slot as the shutdown-quiescence invariant (operator-ratified D5 option B), and builds the
engine composition root the boundary law B-5 requires. MANAGED-ONLY: if any step appears to
require a native (C++/ABI) change, HALT H-ABI -- native members (M5) belong to Cascade C by
ROL 4.4 step 4 (native lands last). Anti-pattern rule: a conflict between this brief and any
LOCKED law means THE BRIEF IS WRONG -- halt H-LAW and quote both texts.

## 1. Ratified decisions (operator, chat, 2026-07-18 -- transcribe into law/code faithfully)

- **D3.** `EngineSession : IDisposable` lives in `DualFrontier.Application` as the ENGINE
  composition root per boundary law B-5: owns kernel/world, scheduler, bus, mod pipeline
  lifecycle end-to-end; ZERO knowledge of game types (the ratchet enforces the assembly
  edge; keep the class itself game-vocabulary-free). `Launcher` consumes it. `GameContext`
  DIES (delete). `GameBootstrap` survives only as a thin SACRIFICIAL harness caller (the
  2026-07-18 scaffolding ruling: harness gameplay logic may be freely cut, no equivalence
  obligation to it -- equivalence binds ENGINE behavior only).
- **D4.** Shutdown is a transaction. S2 fence = bounded `Join` with a NAMED deadline
  constant; on expiry the transaction ABORTS: record structured diagnostics, then fail-fast
  process exit WITHOUT native teardown. Never dismantle native state under a live sim
  thread -- leak-on-abort is safer than racing world finalization.
- **D5 (governance act, option B ratified; refined 2026-07-18 per Phase-0 H-LAW).** SEAT the
  reserved K-L20 slot as the shutdown-transaction quiescence invariant (canon text in section
  5). Phase-0 line-read found the slot was NOT free: it was pre-reserved for "Mod API
  forward-compatibility," with DFK009/DFK012/DFK015/DFK018 + the DFK020 family deferred to
  "the K-L20 cascade." Operator ruling (informed): seat K-L20 = shutdown AND RELOCATE the
  Mod-API-forward-compatibility reservation to a new K-L21 slot -- re-pointing the four DF-rule
  deferrals K-L20 -> K-L21 and renumbering DFK020 -> DFK021; the four LOCKED invariant canons
  stay byte-unchanged (only their DF-rule surface pointer moves). K-L18 stays mod-unload-scoped
  and untouched.
- **D6.** `df_bus_clear` is PROMOTED from test-only to a production teardown step.
  EVENT_BUS + the K-L15 companion text gain the amendment recording
  Fast-tier-on-clearing-thread semantics. Managed callers only -- the export exists.
- **D8.** The bootstrap bridge lambdas are harness-side and sacrificial: EngineSession's
  disposal contract governs; no preservation machinery for harness wiring.
- **D9 (rider).** F-40: add the serializing collection to Interop.Tests (cross-CLASS
  parallelism is the measured exposure) AND a SingletonGuard on the mod_unload globals this
  transaction touches.
- **D10 (rider).** The abnormal-process-exit paragraph is OWNED by ELT; CMM carries a
  cross-reference only.
- **Carried riders:** REC-A1 (ELT 2.6 "no finalizer backstop" misstatement corrected --
  NativeWorld HAS a live finalizer at NativeWorld.cs:496-503 -- plus the stale in-code doc
  at NativeWorld.cs:25-26); the stale IModFaultSink.cs header ("faults arrive through
  ModLoader.HandleModFault") corrected to the RouteFault/ReportFault reality (EQ_A1).

## 2. Established facts (EQ_A_SHUTDOWN_RECON_REPORT @ da63a93 lineage; re-verify at Phase 0)

- F1. S1-S8 verdicts: S6 EXISTS; S1/S2/S8 PARTIAL; S3/S4/S5/S7 ABSENT.
  `GameLoop.Stop` uses `Join(2000)` and can abandon the sim thread mid-`ExecuteTick`.
- F2. `ModIntegrationPipeline.UnloadAll` has ZERO production callers.
- F3. `NativeWorld` is not deterministically disposed by any production owner; a LIVE
  finalizer calls `df_world_destroy` (the ELT 2.6 text claiming otherwise is WRONG --
  REC-A1). `EngineSession` does not exist; ownership today is scattered
  (GameBootstrap/GameContext/Launcher).
- F4. `df_bus_clear` exists as an export with 37 selftest sites, zero production callers.
- F5. EQ_A1 landed: `SystemExecutionContext.RouteFault` (D2 policy), `ModQuarantine`
  skip-set (mechanism only -- Degraded/EngineHealth surface explicitly deferred to THIS
  cascade), `ModFaultHandler` production sink drained by `ModIntegrationPipeline.Apply`.
- F6. Gates baseline: build 0W/0E both configs; full-sln 1179/0/5 (post-W0; F-40 carve-out:
  one recurrence of exactly `UnloadModNativeState_VacuousUnload_Succeeds` is KNOWN --
  NOTE: the D9 serializing collection may eliminate the flake; either outcome is clean,
  record which). DFK-WAIVER pin 2 = 2 HARD. `Console.WriteLine` census in src/ = 2 HARD.
- F7. BoundaryRatchetTests freeze the engine-to-game census (4 edges + 1 IVT) -- this
  cascade must not move it.

## 3. Phase 0 (stop on any failure)

1. `main` HEAD recorded; clean tree. If the BOUNDARY_BANNER_PATCH micro-cascade has not
   yet run, it does NOT block this brief (independent surfaces).
2. Baseline gates re-run and recorded (F6 shape).
3. MANDATORY LAW READS, in full: ROL section 4 (the shutdown transaction + 4.4 order),
   CMM sections 6-7, ELT sections 2.5/2.6/4.1 + its transaction vocabulary,
   EVENT_BUS + K-L15 canon (KERNEL_ARCHITECTURE Part 0), THREADING section 10,
   GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY B-5, EAM section 3 (any transitional structure
   needs gate + equivalence + deletion trigger). The LOCKED letter of ROL/CMM/ELT is the
   work order; the recon's S-verdict table is the gap map. On any brief-vs-law conflict:
   H-LAW.
4. Re-verify F1-F5 by line-read at HEAD; deltas -> report, and H-RECON if load-bearing.
5. Grep KERNEL_ARCHITECTURE Part 0 for the K-L20 reserved-slot row and K-L18's canon form
   (the K-L20 canon in section 5 must be transcribed in the SAME structural form).

## 4. Commit plan (atomic; sync folded into every frontmatter-touching commit)

- **C1 -- enroll.** This brief at `tools/briefs/` (Draft) committed; sync; validate.
- **C2 -- M3 EngineSession.** New `EngineSession : IDisposable` in Application: constructor
  acquires in documented order (world/kernel -> scheduler -> bus wiring -> mod pipeline);
  `Dispose` runs the shutdown TRANSACTION (C3's logic; land the skeleton + acquisition
  here, transaction in C3 if the split is cleaner -- executor's call, record it).
  `GameContext` DELETED; `Launcher` consumes EngineSession; `GameBootstrap` thinned to a
  sacrificial harness caller (cut freely per the ruling; keep the ratchet census
  untouched). Class doc cites B-5 + ROL 4.4.
- **C3 -- the transaction.** Quiesce (stop admitting ticks; deferred work drained or
  cleared) -> fence (D4: bounded Join with named deadline; scheduler phase barrier passed)
  -> teardown (reverse acquisition order: mod pipeline `UnloadAll` (M2, first production
  caller) -> bus teardown incl. promoted `df_bus_clear` (M8/D6) -> scheduler stop ->
  `NativeWorld.Dispose` deterministic; the finalizer remains as backstop-only and its
  stale :25-26 doc is rewritten -- REC-A1 code half). Abort path per D4: structured record
  + fail-fast, NO native teardown. Every step ordering-asserted in C5's tests.
- **C4 -- M7 Degraded/EngineHealth.** ELT 4.1 implemented to its letter; ModQuarantine
  (EQ_A1) linked into the surface (quarantined mod -> reported state), replacing the
  "mechanism only" note. Managed surface; no UI obligations.
- **C5 -- tests.** New coverage: transaction step ORDER (observable seam or recording
  fake); join-timeout abort path (injection seam -- if no clean seam exists without
  redesign, H-SEAM with a proposal, do not bolt a kostyl); UnloadAll invoked exactly once
  per dispose; df_bus_clear step present; double-Dispose idempotent; Degraded surface
  transitions. D9: `[Collection]` serialization for the exposed Interop.Tests classes +
  SingletonGuard on the touched mod_unload globals; record whether F-40 stops recurring.
- **C6 -- governance.** (a) K-L20 SEATED (Option B, per the D5 refinement): KERNEL_ARCHITECTURE
  MINOR (1.1.0 -> 1.2.0). (i) Rewrite the K-L20 row to "Shutdown-transaction quiescence" = the
  section-5 canon in K-L18's structural form (Status AUTHORED at EQ_A2; DF rule = no analyzer
  rule on disk, falsified by the C5 order/abort tests). (ii) Add a new K-L21 row = the RELOCATED
  "Mod API forward-compatibility [RESERVED]" (current K-L20 body moved verbatim; DFK020 family
  -> DFK021 family). (iii) Re-point the four DF-rule deferrals "the K-L20 cascade" -> "the K-L21
  cascade" (K-L9/K-L12/K-L15/K-L18 DF rows + summary table) -- DF-rule surface only; the four
  canons stay byte-unchanged. (iv) Correct the count headline: K-L20 moves reserved -> AUTHORED
  (21 -> 22 active), K-L21 now "reserved separately," canonical-text-rows parenthetical adjusted;
  H-LAW if the recomposition needs more than mechanical wording. Amendment record cites D5 + the
  H-LAW resolution + this EVT. (b) EVENT_BUS + K-L15 companion:
  D6 amendment (df_bus_clear production role; Fast-tier-on-clearing-thread semantics;
  MINOR). (c) ELT: 2.6 finalizer correction (REC-A1); 4.1 Realized note w/ hashes; the
  abnormal-exit paragraph added under ELT ownership (D10) + CMM cross-reference line
  (PATCH each). (d) IModFaultSink.cs header rewritten to RouteFault reality. All
  frontmatter bumps + last_review_event provenance; sync; validate --armed exit 0.
- **C7 -- closure.** EVT append (AUDIT_TRAIL; prior entries byte-unchanged) with lifecycle
  transitions + D-set record; ROADMAP EQ-a row: M2/M3/M7/M8 DONE w/ hashes (M5/M6/M9 stay
  OPEN for Cascades C/D); brief -> EXECUTED; closure report enrolled; sync; validate.

## 5. K-L20 canon (ratified law text -- transcribe verbatim in Part 0's structural form)

K-L20 (shutdown-transaction quiescence): Process shutdown is a TRANSACTION. No native
teardown step may execute while simulation work can still run. Order: QUIESCE (no new
ticks admitted; deferred work drained or deliberately cleared) -> FENCE (simulation thread
joined within a named deadline; scheduler phase barrier passed) -> TEARDOWN (steps in
reverse acquisition order, ending with df_world_destroy). A failed fence ABORTS the
transaction: record structured diagnostics and fail-fast WITHOUT native teardown.
Generalizes K-L18's unload-quiescence principle from mod scope to process scope; K-L18
itself is unchanged. Falsifiability: (a) an injected fence timeout must produce
abort-not-teardown; (b) a teardown-order assertion must fail if any step runs before the
fence passes. Artifacts: EngineSession.Dispose, the C5 order/abort tests.

## 6. Halt catalog

H-LAW (brief-vs-LOCKED conflict: quote both, stop) / H-ABI (native change appears
required: stop, it is Cascade C scope) / H-RECON (load-bearing recon fact no longer holds)
/ H-SEAM (timeout-injection needs redesign: propose, stop) / H-GATE (validate nonzero,
build/test regression beyond F6's F-40 note, DFK-WAIVER pin moves, Console.WriteLine
census moves, ratchet census moves). Standing rails: never push; no history rewrite;
AUDIT_TRAIL append-only; derived artifacts via sync only; historical/ read-only.

## 7. Closure report schema

HEAD before/after; per-commit hashes + deltas; S1-S8 verdict table BEFORE -> AFTER;
the D-set implementation map (decision -> code/doc anchor); F-40 outcome under the D9
collection; final gates (build both configs, tests vs 1179/0/5 baseline, validate exit 0,
DFK-WAIVER 2=2, Console.WriteLine 2, ratchet green); register/EVT/surface deltas;
attestation (no push, sync per-commit, derived headers intact, no native files touched).

## 8. Out of scope (fenced)

M5 native ABI pair (df_world_active_span_count / df_world_destroy_checked -- Cascade C,
D7); M6 swapchain + M9 device-lost (Cascade D, D1); any W1+ boundary work (SDK
abstraction, bus taxonomy); OnBatch dormant path; ELT 5 stream; sink redesign; the
Fixture.RegularMod_ReplacesCombat leak; PSC.
