---
register_id: DOC-E-EQ_A3_CHECKED_DESTROY_CLOSURE_REPORT
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
title: EQ_A3_CHECKED_DESTROY closure report (Cascade C -- the checked-destroy ABI pair df_world_active_span_count + df_world_destroy_checked under D7, F-49/F-50 native riders, F-51 seed)
---

# EQ_A3_CHECKED_DESTROY -- Closure Report

Cascade C of the EQ-a shutdown-law family: the NATIVE members, landing last per
ROL section 4.4 step 4. Native + managed-interop scope; zero gameplay code.

## 1. HEAD before / after

- **Before:** `3972c9b` (main, EQ_A2 closure).
- **Branch:** `claude/eq-a3-checked-destroy` off that HEAD.
- **After:** the C7 closure-append commit (this report + AUDIT_TRAIL EVT + brief
  EXECUTED). NOT pushed -- the operator pushes `claude/eq-a3-checked-destroy` + merges.

## 2. Per-commit hashes

| Commit | Hash | What |
|---|---|---|
| C1 enroll | `3314684` | Brief (Draft) enrolled; REGISTER +1 doc; validate --armed exit 0. |
| C2 native (M5) | `7bc4e07` | `df_status` typedef + `DF_OK`/`DF_ERR_INVALID_HANDLE`/`DF_COND_WORLD_BUSY`; the two exports; both configs 0W/0E; exports 205 -> 207. |
| C3 selftest | `e38e471` | 4 checked-destroy scenarios; selftest 99 -> 103, ALL PASSED both configs. |
| C4 F-50/F-49 | `934e339` | `mod_unload` `SingletonGuard` + concurrency scenario (103 -> 104); `bus_native.h:155` comment corrected. |
| C5 managed | `31dfb26` | Interop decls + `NativeWorld.DisposeChecked`/`ActiveSpanCount` + EngineSession S7 checked switch + busy->Abort; 5 new tests; F-50 test-isolation (`[Collection]` x6); full-sln 1186 -> 1191. |
| C6 governance | `3b7ce64` | IAC 1.1.0 / ROL 1.0.1 / KERNEL 1.2.1; ROADMAP EQ-a Cascade C + F-49/F-50 CLOSED + F-51; sync; validate --armed exit 0. |
| C7 closure | (this commit) | AUDIT_TRAIL EVT; brief -> EXECUTED; this report; sync; validate --armed. |

## 3. ABI: export count + df_status constants

- **Export count: 205 -> 207** (measured `^\s*DF_API` across `native/DualFrontier.Core.Native/include/`; `df_capi.h` alone 151 -> 153). The brief F1 estimate (174 -> 176) was stale; the measured baseline was 205, recorded per the brief's "re-verify at Phase 0" clause.
- **df_status materialized on disk** (`df_capi.h`, the first realized subset; anchor IAC section 4): `DF_OK = 0`, `DF_ERR_INVALID_HANDLE = -1`, `DF_COND_WORLD_BUSY = 6`. The remaining sketch constants stay proposed (F-43).
- **H-CONV resolution (operator-ratified):** `DF_COND_WORLD_BUSY` is positive/retryable, matching IAC section 4's "a negative status ... not that the world is busy"; EngineSession layers the K-L20 fatal-after-fence policy on top (the "never retry" is managed policy, not the ABI class).
- **Refuse scope (operator-ratified):** `df_world_destroy_checked` refuses on `active_spans_ > 0 OR active_batches_ > 0`, reporting both via two out-params; the boundary `catch(...)` returns a NEGATIVE status (never 0 = DF_OK). Unchecked `df_world_destroy` REMAINS as the finalizer backstop. Grandfathered 1/0 block (`df_capi.h:37-40`) byte-unchanged.
- **Skeleton revision:** the export out-param shape is `(out_active_spans, out_active_batches)` (the brief named a single `out_active_spans`) -- driven by the spans+batches refuse-scope; conforms to IAC section 3.3.

## 4. Selftest totals (both configs)

- **99 -> 104 scenarios** (99 baseline + 4 checked-destroy at C3 + 1 concurrency-guard at C4); Release + Debug both `ALL PASSED`, exit 0. The brief F1 estimate (107) was stale; measured baseline was 99.

## 5. Busy-refusal test evidence (three layers)

- **Native selftest:** refuse-on-span (`DF_COND_WORLD_BUSY`, spans==1, world intact + usable, success after release); refuse-on-batch (batches==1, success after `df_batch_destroy`); success-at-zero; unchecked backstop unchanged; span-count accuracy + invalid-handle negative status.
- **Managed real-native** (`CheckedDestroyTests`): the same refuse/report/release/succeed arc through `NativeWorld.DisposeChecked` + `ActiveSpanCount`, plus the batch variant.
- **Managed routing** (`EngineSessionTransactionTests.Dispose_WhenCheckedDestroyReportsBusy_AbortsAfterFence`): an injected busy `WorldTeardownResult` after a passed fence routes through `Abort` (WorldBusy report carrying spans=2/batches=1; steps show `Aborted`, NOT `WorldDisposed`) -- the H-SEAM seam.

## 6. F-49 / F-50 / F-51 dispositions

- **F-49 CLOSED** (C4 `934e339`): `bus_native.h:155` `// test-only` -> `// production teardown (EQ_A2 M8/D6) + test reset`.
- **F-50 CLOSED** (C4 `934e339` guard + C5 `31dfb26` test-isolation): `SingletonGuard` on `df_scheduler_unload_mod_native_state`; the guard exposed a Modding.Tests pipeline-unload race (the suites raced the process-global unload in parallel; green in isolation), closed via `[Collection("GameLoopSerial")]` on the six unload-driving classes -- the F-29 D2+D3 pattern, sibling to EQ_A2 D9's SharedNativeSingleton for the Interop.Tests. Operator-ratified in-session.
- **F-51 SEEDED** (OPEN, architect-owned): Smoke-F02 -- `Runtime.SmokeTest` TileMap multi-cycle trips the `VertexBufferRing` guard; a pre-existing Runtime defect, NOT EQ_A2-caused, NOT the Defender interaction; its own dedicated session (operator-ruled 2026-07-18).

## 7. Final gates (F6 shape + deltas)

- Native build Release + Debug: **0W/0E** (C4; current -- no native change since).
- Native selftest Release + Debug: **ALL PASSED, 104 scenarios**, exit 0 (C4).
- Managed build Release (analyzers armed): **0W/0E** (C5).
- Full-sln `dotnet test -c Release` (TRX harness, no pipe per TESTING_STRATEGY section 8): **1186 -> 1191 pass / 0 fail / 5 skip** (C5; +5 = the 5 new tests; the 2 formerly-failing M73 suites green via serialisation, deterministic across the run). C6/C7 are docs-only, so the code tree is byte-identical to C5's tested state.
- `validate --armed`: **exit 0** after every frontmatter-touching commit (C1, C6, C7).
- Census UNMOVED: DFK-WAIVER src **2=2**; Console.WriteLine src .cs **2=2** (Abort uses `Environment.FailFast`, no console); BoundaryRatchet **4+1** (verified by the 0-fail full-sln). Grandfathered `df_capi.h:37-40` byte-unchanged.

## 8. Register / EVT / ROADMAP deltas

- **REGISTER:** net **+2 docs** across the cascade (the brief at C1, this report at C7); IAC 1.0.0 -> 1.1.0, ROL 1.0.0 -> 1.0.1, KERNEL 1.2.0 -> 1.2.1; `last_modified_commit` dropped from IAC/ROL (real-hashes-only; the pre-commit C6 hash was unknowable; KERNEL precedent).
- **AUDIT_TRAIL:** `EVT-2026-07-18-EQ_A3_CHECKED_DESTROY` appended (prior entries byte-unchanged).
- **ROADMAP:** EQ-a Cascade C progress block; M5 DONE (remaining EQ-a = M6/M9); F-49/F-50 CLOSED; F-51 SEEDED.

## 9. Attestation

- No push (executor); the operator pushes `claude/eq-a3-checked-destroy` + merges.
- No history rewrite; AUDIT_TRAIL append-only; derived REGISTER/CURRENT_AUTHORITY_SURFACE via `sync` only; grandfathered 1/0 block byte-unchanged; K-L20 canon byte-unchanged (artifacts line only -- H-LAW did not fire); C++23preview flag (K-L1) untouched; `historical/` untouched.
- Operator ratifications recorded (this session): H-CONV (`DF_COND_WORLD_BUSY` = 6, positive/retryable), refuse-scope (spans + write-batches), H-SEAM (the `ShutdownTransactionHooks` world-teardown seam), and the F-50 test-isolation (serialise via `GameLoopSerial`).
