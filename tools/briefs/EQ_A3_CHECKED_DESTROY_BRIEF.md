---
register_id: DOC-D-EQ_A3_CHECKED_DESTROY_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
title: EQ_A3_CHECKED_DESTROY execution brief (Cascade C of the EQ-a shutdown-law family -- M5 native ABI pair df_world_active_span_count + df_world_destroy_checked under ratified D7, + F-49 comment fix, F-50 SingletonGuard, F-51 ledger seed)
---

# EQ_A3_CHECKED_DESTROY -- Execution Brief

Cascade C of the EQ-a shutdown-law family: the NATIVE members, landing last per ROL 4.4
step 4. Implements the ratified D7 checked-destroy ABI pair, discharges the two native
riders EQ_A2 fenced out (F-49, F-50), and seeds the chat-only Smoke-F02 finding as F-51.
Native + managed-interop scope; ZERO gameplay/harness code. Anti-pattern rule: a conflict
between this brief and any LOCKED law means THE BRIEF IS WRONG -- halt H-LAW, quote both.

## 1. Ratified decisions (operator, chat, 2026-07-18)

- **D7.** `df_world_destroy_checked` is REFUSE-NOT-FORCE: with active spans outstanding it
  refuses destruction and reports the live count; it is the ABI-level enforcement of ROL
  section 6.2 wait-for-zero. New status constants live in the IAC section 4 df_status
  space (working names: `DF_STATUS_WORLD_BUSY` for live-spans refusal; invalid handle
  follows the EXISTING convention -- reuse, do not invent). The shipped 0/1 status block
  stays grandfathered byte-unchanged. `df_world_destroy` (unchecked) REMAINS: it is the
  finalizer backstop primitive, not a transitional bridge -- both entry points are
  legitimate law (checked = production path; unchecked = backstop), no deletion trigger.
- **EngineSession S7 semantics (D7 consequence).** Production teardown switches to the
  checked variant. A `WORLD_BUSY` refusal AFTER a passed fence is a K-L20 invariant
  violation (live spans despite proven quiescence): treat as transaction failure -- record
  structured diagnostics (include the reported span count) and route through the existing
  Abort path (fail-fast; no further teardown). Never loop-retry into the refusal: a spin
  here would be a kostyl hiding the invariant breach.
- **F-51 seed.** The Smoke-F02 finding (post-closure GPU verification at EQ_A2) enters the
  F-ledger verbatim-in-substance: `Runtime.SmokeTest` TileMap multi-cycle scenario trips
  the `VertexBufferRing` guard -- a PRE-EXISTING Runtime defect, NOT EQ_A2-caused, NOT the
  Defender interaction; disposition: its own dedicated session (operator-ruled 2026-07-18).
  Status OPEN, architect-owned.

## 2. Established facts (EQ_A recon @ da63a93 lineage + EQ_A2 closure; re-verify at Phase 0)

- F1. The active-spans counter EXISTS native-side, UNEXPORTED. `df_capi.h` carries 174
  exports at the recon count (176 expected after this cascade); `catch(...)` entry-point
  hygiene is the standing convention (72 sites at recon).
- F2. IAC section 4 defines the df_status conventions; the shipped 0/1 block is
  grandfathered. F-43 (full ABI-hardening protocol) is a SEPARATE ledger item riding
  EQ-b/c -- this cascade adds two conforming entries, it does NOT start F-43.
- F3. F-49: `native/DualFrontier.Core.Native/include/bus_native.h:155` carries a
  `// test-only` marker on `df_bus_clear` that EQ_A2's M8 promotion made factually stale.
- F4. F-50: `mod_unload.cpp` globals (`g_sim_paused`; entry
  `df_scheduler_unload_mod_native_state`) lack a `SingletonGuard`; the F-29 D2/D3
  fail-loud pattern + `SingletonGuard(busy_)` form is the ratified remedy shape.
- F5. `EngineSession.Dispose` S7 currently calls `_world.Dispose()` ->
  `df_world_destroy` (unchecked). The `NativeWorld` finalizer is backstop-only (EQ_A2).
- F6. Gates baseline: build 0W/0E both configs; full-sln 1186/0/5; native selftest ALL
  PASSED (107 scenarios); `validate --armed` exit 0; DFK-WAIVER 2=2; Console.WriteLine
  src=2; BoundaryRatchet 4+1; K-L20 seated (KERNEL 1.2.0).

## 3. Phase 0 (stop on any failure)

1. `main` HEAD recorded (3972c9b or descendant); clean tree; baseline gates re-run (F6).
2. MANDATORY LAW READS: IAC sections 3-4 IN FULL (ABI + df_status conventions -- the new
   constants must conform to the letter; if the convention is ambiguous for a refusal-
   with-payload shape, H-CONV: quote the text, propose, stop), ROL sections 4 + 6.2,
   K-L20 canon (KERNEL Part 0), K-L1 (C++23 preview constraints), CODING_STANDARDS native
   sections, EAM 3.3 (surface rules).
3. Re-verify F1/F3/F4/F5 by line-read; record the exact current export count and the
   exact spans-counter data structure + its thread-safety story (the export must read it
   race-free -- if the counter is not safely readable from the destroying thread without
   new synchronization DESIGN, H-SYNC: propose, stop).
4. Confirm the selftest harness pattern for new scenarios (mirror an existing
   world-lifecycle scenario's registration form).

## 4. Commit plan (atomic; sync folded into frontmatter-touching commits)

- **C1 -- enroll.** This brief (Draft) committed; sync; validate.
- **C2 -- native exports (M5).** `df_world_active_span_count(world)` read-only export;
  `df_world_destroy_checked(world, out_active_spans)` refuse-not-force per D7 (exact
  signature shape conforms to IAC section 3 conventions -- ground it, do not invent);
  status constants added in the IAC section 4 space; `catch(...)` hygiene matching the
  72-site convention; header docs state the checked/unchecked relation (production vs
  backstop). Both configs build; export count 174 -> 176 recorded.
- **C3 -- native selftest.** New scenarios: (a) span-count export accuracy across
  acquire/release; (b) destroy_checked REFUSES under a live span, reports the count,
  world remains valid and destroyable after release; (c) destroy_checked succeeds at
  zero and the handle dies; (d) unchecked destroy behavior unchanged (backstop
  regression pin). Selftest total grows from 107; ALL PASSED both configs.
- **C4 -- F-50 SingletonGuard.** `SingletonGuard(busy_)` (the F-29 pattern verbatim) on
  `df_scheduler_unload_mod_native_state`; a selftest or existing-scenario assertion that
  concurrent entry now fails LOUD (fail-open doctrine: absence of the guard was the
  silent path). F-49 in the same native sweep: bus_native.h:155 comment rewritten to the
  production-teardown truth (cite EQ_A2 M8/D6).
- **C5 -- managed half.** Interop declarations for both exports; `NativeWorld.Dispose`
  production path switches to checked-destroy with the D7-consequence semantics (busy
  after fence -> structured record incl. span count -> EngineSession Abort route); the
  finalizer STAYS on unchecked (backstop must never fail-fast the finalizer thread --
  record this asymmetry in the class doc); EngineSession/NativeWorld doc updates. Managed
  tests: checked-path success; the busy-refusal route asserted through the ShutdownHooks
  recorder seam (managed fake or interop scenario -- executor picks the clean seam,
  H-SEAM if none exists without redesign).
- **C6 -- governance.** IAC MINOR (new ABI members + status constants recorded in its
  sections 3-4 tables); ROL PATCH (6.2 Realized note w/ hashes); KERNEL PATCH (K-L20
  artifacts line gains the checked-destroy pair -- canon text untouched, H-LAW if more
  than the artifacts line needs edits); ROADMAP: M5 DONE w/ hashes (EQ-a remaining =
  M6/M9 only), F-49 CLOSED, F-50 CLOSED, F-51 SEEDED (section-1 text); sync; validate.
- **C7 -- closure.** EVT append (prior entries byte-unchanged); brief -> EXECUTED;
  closure report enrolled; final gates recorded.

## 5. Halt catalog

H-LAW / H-CONV (df_status convention ambiguous for refusal-with-payload) / H-SYNC
(span-counter read needs new synchronization design) / H-SEAM (no clean busy-path test
seam) / H-GATE (validate nonzero; build/test/selftest regression vs F6; DFK-WAIVER moves;
Console.WriteLine census moves; ratchet moves; grandfathered 0/1 status block edited).
Standing rails: never push; no history rewrite; AUDIT_TRAIL append-only; derived via sync
only; historical/ read-only; C++23 preview flag discipline (K-L1) untouched.

## 6. Closure report schema

HEAD before/after; per-commit hashes; export count 174 -> 176 (or measured actual with
delta explained); new status constant values + the IAC anchor; selftest totals
before/after both configs; the busy-refusal test evidence; F-49/F-50/F-51 dispositions;
final gates (F6 shape + deltas); register/EVT/surface deltas; attestation (no push, sync
per-commit, derived headers intact, grandfathered block byte-unchanged).

## 7. Out of scope (fenced)

F-43 full ABI-hardening protocol; M6 swapchain + M9 device-lost (Cascade D, D1); the
Smoke-F02 FIX itself (F-51 is seeded, its session is separate); any W1+ boundary work;
EngineSession behavior beyond the S7 checked-destroy switch; PSC.
