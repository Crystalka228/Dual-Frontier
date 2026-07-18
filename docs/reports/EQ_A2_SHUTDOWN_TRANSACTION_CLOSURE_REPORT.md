---
register_id: DOC-E-EQ_A2_SHUTDOWN_TRANSACTION_CLOSURE_REPORT
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
title: EQ_A2_SHUTDOWN_TRANSACTION closure report (Cascade B — EngineSession + world-shutdown transaction + Degraded/EngineHealth + df_bus_clear promotion + К-L20 seating)
---

# EQ_A2_SHUTDOWN_TRANSACTION — Closure Report

Cascade B of the EQ-a shutdown-law family. MANAGED-ONLY; no native files touched.

## 1. HEAD before / after

- **Before:** `100384d224c4b1aba7d29282266fa7033f96b50c` (main).
- **Branch:** `claude/eq-a2-shutdown-transaction` off that HEAD.
- **After:** the C7 closure-append commit (this report + AUDIT_TRAIL EVT + ROADMAP + brief EXECUTED). Not pushed — the operator pushes + merges.

## 2. Per-commit hashes + deltas

| Commit | Hash | What |
|---|---|---|
| C1 enroll | `41a78fe` | Brief committed (Draft) to match its already-enrolled register entry; Option-B H-LAW correction folded into D5/C6a. Register unchanged (a prior sync had enrolled the frontmatter); validate --armed exit 0. |
| C2+C3 | `f207f02` | `EngineSession : IDisposable` + the shutdown transaction (quiesce→fence→teardown+abort); `GameLoop.TryStop` / `DomainEventBus.DropDeferred` / `ManagedBusBridge.Shutdown` primitives; `GameContext` deleted; Launcher rewired fence-first. Census `deferred` 82→86/51→52 (same-commit pin update). |
| C4 | `1e07c2d` | Degraded/EngineHealth surface (ELT §4.1) + the `ModQuarantine`→Degraded link (scheduler `OnModQuarantined` observer, wired in `GameBootstrap`). |
| C5 | `3e63416` | 7 transaction/Degraded tests (order, abort-not-teardown, unload-once, bus-clear, idempotent, Degraded entry/exit, dedupe); the `CreateSession` hooks seam; D9 serialising `[Collection]` in Interop.Tests. |
| C6 | `e733797` | KERNEL Option-B (К-L20 seated shutdown-quiescence, Mod-API→К-L21); EVENT_BUS D6; ELT/CMM corrections + D10; IModFaultSink REC-A1. sync written, validate --armed exit 0. |
| C7 closure | (this commit) | AUDIT_TRAIL EVT; ROADMAP EQ-a Cascade-B note + F-40 CLOSED + F-49/F-50; brief → EXECUTED; this report; sync; validate. |

**Executor split ruling (recorded):** the brief's C2 (skeleton) / C3 (transaction) split was FOLDED into one commit — under `TreatWarningsAsErrors` a ref-carrying skeleton whose `Dispose` does not yet consume the refs is meaningless and risks unused-field diagnostics; the composition root and the transaction it exists to run are one atomic unit (the brief C2 permits this — "executor's call, record it").

## 3. S1–S8 verdict table (recon BEFORE → EQ_A2 AFTER)

| Step | Before (EQ_A recon) | After | Realized by |
|---|---|---|---|
| S1 stop intake | PARTIAL | REALIZED | session Dispose stops admitting ticks (input drain already vacuous) |
| S2 finish tick, bounded CHECKED join | PARTIAL | REALIZED | `GameLoop.TryStop(deadline)` (checked) + К-L18 pipeline quiescence; abort on miss |
| S3 flush/drop deferred, counted | ABSENT | REALIZED | `DomainEventBus.DropDeferred` / `GameServices.DropDeferred` (returns a count) |
| S4 unload mods | ABSENT | REALIZED | `ModIntegrationPipeline.UnloadAll` — its FIRST production caller (M2) |
| S5 native scheduler/bus teardown + free GCHandles | ABSENT | REALIZED | `SystemGraphInterop.Clear`/`WakeRegistryInterop.Clear`/`SchedulerAdapter.ClearCallback` + `ManagedBusBridge.Shutdown` (`df_bus_clear` + GCHandle free) |
| S6 GPU waitIdle + Vulkan dispose | EXISTS | EXISTS | `renderer.Shutdown` (unchanged) — now correctly sequenced AFTER the session fence |
| S7 deterministic `NativeWorld.Dispose` | ABSENT | REALIZED | `world.Dispose()` in the transaction; the finalizer is now a leak-reporter backstop only |
| S8 window last + exit log | PARTIAL | PARTIAL | window-last order REALIZED (session disposes before renderer/runtime); the pending-reclamation EXIT LOG + failed-step exit code remain Launcher-side (S8 exit-log wiring deferred) |

**Honest scope notes:** (a) the ELT §2.6 stage-4 *Recover* per-step best-effort-continue is not implemented — teardown steps run in order and an unexpected step exception would propagate; post-fence steps are expected to succeed, and the safety property (no teardown before a passed fence) holds. (b) ELT §4.1 transition-REFUSAL depends on the §3.2 session state machine (not built); this cascade lands the queryable annotation + quarantine link + lifecycle event only.

## 4. D-set implementation map

| Decision | Code / doc anchor |
|---|---|
| D3 EngineSession composition root, B-5, GameContext dies, GameBootstrap sacrificial | `EngineSession.cs`; `GameBootstrap.CreateSession`; `GameContext.cs` deleted; `Program.cs` |
| D4 transaction; bounded Join named deadline; abort = diagnostics + fail-fast, no native teardown | `EngineSession.DefaultFenceDeadline`, `Dispose` fence + `Abort` → `Environment.FailFast` |
| D5 seat К-L20 (Option B: relocate Mod-API → К-L21) | `KERNEL_ARCHITECTURE.md` Part 0 (C6a) |
| D6 df_bus_clear production promotion + Fast-tier-on-clearing-thread | `ManagedBusBridge.Shutdown`; `EVENT_BUS.md` §3/§4 (C6b) |
| D8 bootstrap bridge lambdas sacrificial | `GameBootstrap` (harness; no preservation machinery) |
| D9 serialising collection + native SingletonGuard | `[Collection("SharedNativeSingleton")]` in Interop.Tests (done); native SingletonGuard → F-50 (Cascade C, per Q2) |
| D10 abnormal-exit paragraph owned by ELT + CMM cross-ref | `ELT §2.6`; `CMM §6.2` (C6c) |
| REC-A1 ELT finalizer + NativeWorld doc + IModFaultSink header | `ELT §2.6`; `NativeWorld.cs:25-26` (C3); `IModFaultSink.cs` (C6d) |

## 5. F-40 outcome under the D9 collection

**F-40 CLOSED.** The serialising `[Collection("SharedNativeSingleton")]` on the three cross-class native-state Interop.Tests classes (`ModUnloadInteropTests`, `BackgroundQueueInteropTests`, `PipelineSlotInteropTests`) ELIMINATED the flake: `UnloadModNativeState_VacuousUnload_Succeeds` was intermittently 201/1 before (full-sln and isolated-assembly), and is 202/0/0 across three post-D9 runs (two isolated + the full-sln) with the single test isolated 1/1. The fail-loud native SingletonGuard (the other F-29 D2/D3 option the F-40 row named) is deferred to Cascade C as **F-50** (native scope, Q2). The stale `bus_native.h:155` `// test-only` comment is **F-49** (native scope).

## 6. Final gates

- **Build:** 0W/0E both configs (Debug + Release).
- **Full-sln tests** (Release, §8 no-pipe harness): **1179 → 1186 / 0 / 5** (total 1191). +7 = the C5 tests; 5 skips are the unchanged F-30/F-31 Extreme guards; F-40 no longer recurs.
- **validate --armed:** exit 0 after every frontmatter-touching commit (C1, C6, C7).
- **Census:** `deferred` 82→86/51→52 (test-pinned, same-commit); DFK-WAIVER **2 = 2** (test-enforced); Console.WriteLine src **= 2** (the abort uses `Environment.FailFast`, never console); BoundaryRatchet **4 edges + 1 IVT UNMOVED** (EngineSession game-vocabulary-free by discipline; GameBootstrap keeps the game refs).

## 7. Register / EVT / surface deltas

- REGISTER: 4 architecture-doc metadata bumps (KERNEL 1.1.0→1.2.0, EVENT_BUS 1.0.1→1.1.0, ELT 1.0.0→1.0.1, CMM 1.0.1→1.0.2) — no document added or removed. `last_modified_commit` dropped from KERNEL/EVENT_BUS/ELT (pre-commit hash unknowable; the EQ_A1-on-CMM precedent).
- New enrollments: this report (DOC-E). The brief (DOC-D) flips Draft → EXECUTED.
- AUDIT_TRAIL: `EVT-2026-07-18-EQ_A2_SHUTDOWN_TRANSACTION` appended (prior entries byte-unchanged).
- ROADMAP: EQ-a Cascade-B progress note (M2/M3/M7/M8 DONE; M5/M6/M9 OPEN); F-40 CLOSED; F-49/F-50 added; REC-A1 rider DISCHARGED.

## 8. Attestation

No push (executor). sync run + validate --armed exit 0 on every frontmatter-touching commit. Derived headers (REGISTER.yaml, CURRENT_AUTHORITY_SURFACE.yaml) regenerated via `sync` only, never hand-edited. AUDIT_TRAIL append-only (prior entries byte-unchanged). **No native (C++/ABI) files touched** — the promotion of `df_bus_clear` is entirely managed-side (the export was pre-existing); the native ABI pair (M5), swapchain (M6), device-lost (M9), and the native mod_unload SingletonGuard (F-50) are fenced to Cascades C/D. `historical/` untouched.
