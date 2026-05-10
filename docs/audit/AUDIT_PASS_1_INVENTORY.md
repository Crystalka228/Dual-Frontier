---
title: Audit Pass 1 — Inventory & Baseline
nav_order: 104
---

# Audit Pass 1 — Inventory & Baseline

**Date:** 2026-05-01
**Branch:** `main` (per `.git/HEAD` line 1)
**HEAD:** `1d43858a36c17b956a345e9bfe07a9ccf82daddb` (per `.git/refs/heads/main` line 1)
**Scope:** Inventory only. No interpretation, no classification, no
comparison between sources. Pure facts with source attribution.

---

## §0 Executive summary

| # | Check | Status | Notes |
|---|---|---|---|
| 1 | Repo baseline established | PASSED | HEAD `1d43858`, branch `main`, last commit subject "docs(roadmap): close M7.3 — UnloadMod step 7 + Phase 2 carried-debt closure" at 2026-05-02 01:05 UTC |
| 2 | Test inventory complete | PASSED | 357 `[Fact]` attributes counted across 48 files + 2 `[Theory]` attributes counted in 1 file = 359 source-level attributes; 0 `[Fact(Skip` / `[Theory(Skip` matches |
| 3 | Spec version captured | PASSED | `MOD_OS_ARCHITECTURE.md:8` status line reads `LOCKED v1.4`; v1.4 changelog entry on lines 26–29 |
| 4 | Document inventory complete | PASSED | 29 .md files in `docs/`, 10 .md files in `docs/audit/`, 1 .md in `docs/learning/`, 2 root-level READMEs (`README.md`, `mods/README.md`), 58 sub-folder READMEs under `src/`, `tests/`, `mods/` |
| 5 | Symbol inventory complete | PASSED | 38 public types catalogued across 32 source files in `src/DualFrontier.Contracts/` |
| 6 | Manifest schema inventory complete | PASSED | 19 `mod.manifest.json` files catalogued at source paths (18 fixtures under `tests/Fixture.*/` + 1 mod under `mods/DualFrontier.Mod.Example/`) |
| 7 | Closure review inventory complete | PASSED | 4 M-phase closure reviews + 1 Phase 4 session log + 3 translation-campaign reports + 1 campaign plan + 1 pass prompt — all 10 files present under `docs/audit/` |
| 8 | Cyrillic inventory complete | PASSED | 0 `.cs` files with characters in U+0400–U+04FF range across `src/`, `tests/`, `mods/` |
| 9 | Sequence catalogue complete | PASSED | 53 numbered/lettered sequences catalogued across spec, roadmap, code, and closure reviews |

`PASSED` = data successfully gathered. `FAILED` = filesystem access failed,
file missing, parse error, or other technical blocker (none recorded).

---

## §1 Repo baseline

- **Branch:** `main` (source: `.git/HEAD` line 1 contains `ref: refs/heads/main`).
- **HEAD commit:** `1d43858a36c17b956a345e9bfe07a9ccf82daddb` (source: `.git/refs/heads/main` line 1).
- **Last commit subject:** `docs(roadmap): close M7.3 — UnloadMod step 7 + Phase 2 carried-debt closure` (source: `.git/logs/HEAD` last commit-event line).
- **Last commit author:** `Crystalka <vovavovilav@hotmail.com>` (source: `.git/logs/HEAD`).
- **Last commit timestamp:** `1777683908 -0400` Unix epoch → `2026-05-02 01:05 UTC` (`2026-05-01 21:05 EDT` local).
- **Recent commit log (last 30 commit-events, newest first; checkouts/pulls excluded):**

| # | SHA (short) | Author | Timestamp (UTC) | Subject |
|---|---|---|---|---|
| 1 | `1d43858` | Crystalka | 2026-05-02 01:05 | docs(roadmap): close M7.3 — UnloadMod step 7 + Phase 2 carried-debt closure |
| 2 | `46b4f33` | Crystalka | 2026-05-02 01:02 | test(modding): §9.5 step 7 protocol + Phase 2 carried-debt closure |
| 3 | `9bed1a4` | Crystalka | 2026-05-02 00:56 | feat(modding): UnloadMod step 7 — WeakReference + GC pump + ModUnloadTimeout |
| 4 | `c3f5251` | Crystalka | 2026-05-01 06:40 | docs(roadmap): close M7.2 — ALC unload chain steps 1-6 |
| 5 | `d68ba93` | Crystalka | 2026-05-01 06:32 | test(modding): M7.2 ALC unload chain tests |
| 6 | `2531ed7` | Crystalka | 2026-05-01 06:32 | feat(modding): retain RestrictedModApi on LoadedMod and add UnloadMod chain to pipeline |
| 7 | `f3e92fb` | Crystalka | 2026-05-01 06:20 | docs(roadmap): sync stale v1.3 ref to v1.4 in M-Section preamble |
| 8 | `0606c43` | Crystalka | 2026-05-01 06:11 | docs(roadmap): close M7.1 — Pause/Resume on ModIntegrationPipeline |
| 9 | `c964475` | Crystalka | 2026-05-01 06:08 | test(modding): M7.1 Pause/Resume guard tests |
| 10 | `a2ab761` | Crystalka | 2026-05-01 06:00 | feat(modding): add Pause/Resume + IsRunning state to ModIntegrationPipeline |
| 11 | `b504813` | Crystalka | 2026-05-01 05:49 | docs(architecture): ratify v1.4 — pre-flight clarifications to §9.5 for M7 |
| 12 | `c7210ca` | Crystalka | 2026-05-01 04:32 | docs(review): M6 closure verification report |
| 13 | `e643011` | Crystalka | 2026-05-01 04:12 | docs(roadmap): close M6 — sync with M6.1 + M6.2 implementation |
| 14 | `adad506` | Crystalka | 2026-05-01 04:03 | test(modding): M6.2 integration tests + replacement fixtures |
| 15 | `602a84e` | Crystalka | 2026-05-01 03:55 | test(modding): helper tests for CollectReplacedFqns |
| 16 | `23f2933` | Crystalka | 2026-05-01 03:53 | feat(modding): wire bridge replacement skip into ModIntegrationPipeline graph build |
| 17 | `b0f1ee5` | Crystalka | 2026-05-01 03:28 | test(modding): Phase H scenarios + Replaceable bridge annotation guards |
| 18 | `a408f44` | Crystalka | 2026-05-01 03:24 | feat(modding): add Phase H bridge replacement validation to ContractValidator |
| 19 | `1af73ad` | Crystalka | 2026-05-01 03:22 | feat(systems): annotate Phase 5 combat stubs as Replaceable bridges |
| 20 | `32f6f04` | Crystalka | 2026-05-01 02:48 | docs(review): M5 closure verification report |
| 21 | `5c0d1b5` | Crystalka | 2026-05-01 02:23 | docs(roadmap): close M5 — sync with M5.1 + M5.2 implementation |
| 22 | `376be7e` | Crystalka | 2026-05-01 01:53 | test(modding): integration tests for M5.2 validator-level cascade behavior |
| 23 | `f8f18ee` | Crystalka | 2026-05-01 01:49 | feat(modding): add Phase G inter-mod dependency version check |
| 24 | `50efe9d` | Crystalka | 2026-05-01 01:47 | feat(modding): modernize Phase A to use VersionConstraint pipeline for v2 manifests |
| 25 | `bab4d85` | Crystalka | 2026-05-01 01:27 | test(modding): integration tests for M5.1 pipeline behavior |
| 26 | `a3968f4` | Crystalka | 2026-05-01 01:22 | feat(modding): wire regular-mod toposort and dep presence into pipeline |
| 27 | `13400bb` | Crystalka | 2026-05-01 01:17 | feat(modding): add TopoSortRegularMods + CheckDependencyPresence helpers |
| 28 | `fffd785` | Crystalka | 2026-05-01 01:14 | refactor(modding): extract TopoSortByPredicate from TopoSortSharedMods |
| 29 | `dba17c7` | Crystalka | 2026-04-30 02:57 | docs(review): M4 closure verification report |
| 30 | `2a707f3` | Crystalka | 2026-04-30 02:32 | docs: update MOD_OS_ARCHITECTURE version reference (v1.2 → v1.3) |

Source: `.git/logs/HEAD`, filtered for `commit:` event-type lines (checkout and pull events excluded). Timestamps converted from Unix epoch to UTC.

- **Repository size approximation:**
  - 11 sub-directories under `src/` (DualFrontier.AI, .Application, .Components, .Contracts, .Core, .Core.Interop, .Events, .Persistence, .Presentation, .Presentation.Native, .Systems).
  - 23 sub-directories under `tests/` (5 test projects: DualFrontier.Core.Benchmarks, .Core.Tests, .Modding.Tests, .Persistence.Tests, .Systems.Tests; 18 fixture mod assemblies under `Fixture.*/`).
  - 1 sub-directory under `mods/` (DualFrontier.Mod.Example).
  - 2 sub-directories under `docs/` (audit, learning) plus 29 root-level `.md` files.

---

## §2 Test inventory

`[Fact]` and `[Theory]` attributes counted by reading test files directly via Grep over `tests/**/*.cs`. Pass 1 does not run `dotnet test`. The count is the source-level number of attribute occurrences (each `[Theory]` attribute may expand to multiple runtime test cases through `[InlineData]`).

| Project | Test files | `[Fact]` count | `[Theory]` count | Total | ROADMAP-stated count |
|---|---|---|---|---|---|
| `tests/DualFrontier.Persistence.Tests/` | 4 | 4 | 0 | 4 | 4 (ROADMAP.md:21 — Persistence row "Tests 4/4"; ROADMAP.md:36 engine snapshot lists "Persistence 4") |
| `tests/DualFrontier.Systems.Tests/` | 6 | 16 | 0 | 16 | 16 (M6_CLOSURE_REVIEW.md:55 table row "DualFrontier.Systems.Tests 16"; consistent with ROADMAP M6 closure) |
| `tests/DualFrontier.Modding.Tests/` | 31 (.cs files under non-bin/non-obj paths; 28 of these contain `[Fact]`) | 277 | 2 | 279 | 289 implied (369 total per ROADMAP.md:36 minus Persistence 4, Systems 16, Core 60) |
| `tests/DualFrontier.Core.Tests/` | 10 (10 of 10 contain `[Fact]`) | 60 | 0 | 60 | 60 (M6_CLOSURE_REVIEW.md:54 table row "DualFrontier.Core.Tests 60") |
| **Total** | **51** | **357** | **2** | **359** | **369** (per ROADMAP.md:36 engine snapshot line: "Total at M7.3 closure: 369/369 passed") |

**Per-file attribute counts (Modding.Tests, source: Grep `\[Fact` over `tests/DualFrontier.Modding.Tests/**/*.cs`):**

| File | `[Fact]` count | `[Theory]` count |
|---|---|---|
| `Capability/ProductionComponentCapabilityTests.cs` | 1 | 2 |
| `Capability/KernelCapabilityRegistryTests.cs` | 11 | 0 |
| `Capability/CapabilityValidationTests.cs` | 11 | 0 |
| `Manifest/VersionConstraintTests.cs` | 35 | 0 |
| `Manifest/ModManifestV2Tests.cs` | 18 | 0 |
| `Manifest/ModDependencyTests.cs` | 18 | 0 |
| `Manifest/ManifestCapabilitiesTests.cs` | 31 | 0 |
| `Validator/PhaseHBridgeReplacementTests.cs` | 8 | 0 |
| `Validator/PhaseGInterModVersionTests.cs` | 7 | 0 |
| `Validator/PhaseAModernizationTests.cs` | 6 | 0 |
| `Api/RestrictedModApiV2Tests.cs` | 22 | 0 |
| `Parser/ManifestParserTests.cs` | 19 | 0 |
| `Sharing/ContractTypeInRegularModTests.cs` | 6 | 0 |
| `Sharing/CrossAlcTypeIdentityTests.cs` | 3 | 0 |
| `Sharing/SharedAssemblyResolutionTests.cs` | 4 | 0 |
| `Sharing/SharedModComplianceTests.cs` | 8 | 0 |
| `Pipeline/CollectReplacedFqnsTests.cs` | 5 | 0 |
| `Pipeline/ContractValidatorTests.cs` | 6 | 0 |
| `Pipeline/DependencyPresenceTests.cs` | 4 | 0 |
| `Pipeline/M51PipelineIntegrationTests.cs` | 4 | 0 |
| `Pipeline/M52IntegrationTests.cs` | 3 | 0 |
| `Pipeline/M62IntegrationTests.cs` | 5 | 0 |
| `Pipeline/M71PauseResumeTests.cs` | 11 | 0 |
| `Pipeline/M72UnloadChainTests.cs` | 13 | 0 |
| `Pipeline/M73Step7Tests.cs` | 5 | 0 |
| `Pipeline/M73Phase2DebtTests.cs` | 2 | 0 |
| `Pipeline/ModIntegrationPipelineTests.cs` | 5 | 0 |
| `Pipeline/RegularModTopologicalSortTests.cs` | 6 | 0 |

**Skipped tests inventory:**

| File:line | Test name | Skip reason |
|---|---|---|
| (none) | (none) | (none — Grep for `\[Fact\(Skip` and `\[Theory\(Skip` over `tests/**/*.cs` returned zero matches) |

---

## §3 Spec version

- **File:** `docs/architecture/MOD_OS_ARCHITECTURE.md`
- **Status line (line 8, verbatim):**
  ```
  **Status:** LOCKED v1.4 — Phase 0 closed; non-semantic corrections from M1–M3.1 audit (v1.1), M3 closure review (v1.2), M4.3 implementation review (v1.3), and M7 pre-flight readiness review (v1.4) applied. Every architectural decision in this document is final input to all subsequent migration phases (M1–M10, see §11). Items marked **✓ LOCKED** reflect decisions taken during Phase 0 deliberation; deviation in implementation requires reopening this document, not improvisation in code.
  ```
- **Last ratification entry (lines 26–29, verbatim):**
  ```
  - v1.4 (this version) — non-semantic clarifications from the M7 pre-flight readiness review:
    - §9.5 step 7: explicit GC pump protocol added. Each iteration of the `WeakReference` spin loop performs `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect()` before re-checking `WeakReference.IsAlive`. The double-collect bracket is required because `WaitForPendingFinalizers` can resurrect finalizable graph nodes the first collect would have removed; the second collect picks those up, restoring monotonic progress. Default cadence: 100 iterations × 100 ms = 10 s timeout (matches the §9.5 step 7 v1.0 wording). The cadence is implementation-tunable; the GC pump bracket is mandatory. Without this clarification the v1.0 wording «spins on `WeakReference`» admits flaky implementations, and the §11.4 stop condition («WeakReference unload tests are flaky — any failure rate above 0%») would trigger spuriously. v1.4 brings §9.5 step 7 in line with the only stable implementation pattern.
    - §9.5: new sub-section §9.5.1 «Failure semantics» added, locking the best-effort discipline already implicit in the chain. Steps 1–6 are sequential and best-effort: if any step throws, the loader logs the exception with `(modId, stepNumber)`, surfaces a non-blocking `ValidationWarning`, and continues to the next step. The `ModLoader.UnloadMod` swallowed `try/catch` around `mod.Instance.Unload()` (in place since M0) is consistent with this discipline. After step 6, if step 7 times out, the existing `ModUnloadTimeout` warning fires; the mod is removed from the active set regardless. There is no atomic-unload guarantee — `Unload` is conceptually irreversible (subscriptions removed cannot be re-attached without re-running `Subscribe`); the chain is structured so each step is a no-op if its predecessor failed (e.g. `RemoveSystems` on a mod with no registered systems is harmless). This formalises a discipline the M0–M6 implementation already follows; no new state is introduced to §9.1.
    - No semantic changes. No locked decision (D-1 through D-7) is altered. No state added to §9.1. M0–M6 implementations continue to comply.
  ```
- **File size:** 65,037 bytes (source: `ls -la docs/architecture/MOD_OS_ARCHITECTURE.md`).
- **Total line count:** 982 (source: `wc -l docs/architecture/MOD_OS_ARCHITECTURE.md`).
- **Top-level section count (lines starting with `## `):** 15 (Preamble + 13 numbered sections §0–§12 + See also; source: Grep `^## ` over file).

---

## §4 Document inventory

### Root and `docs/`

| Path | Type | Status (from header or content) | File size (bytes) |
|---|---|---|---|
| `README.md` (root) | nav / research framing | active (v1.0 framing) | 6,561 |
| `docs/README.md` | nav | active | 5,834 |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | spec | LOCKED v1.4 (line 8) | 65,037 |
| `docs/ROADMAP.md` | active roadmap | active (M7.3 closed; M7.4–M7.5 pending per status overview line 11) | 52,535 |
| `docs/methodology/METHODOLOGY.md` | methodology | active v1.0 (line 6 says "Version: 1.0 (2026-04-25)") | 33,275 |
| `docs/architecture/ARCHITECTURE.md` | architecture | active v0.3 (line 7 changelog `v0.3 (2026-04)`) | 11,712 |
| `docs/architecture/CONTRACTS.md` | architecture | active | 5,933 |
| `docs/architecture/ECS.md` | architecture | active | 7,493 |
| `docs/architecture/EVENT_BUS.md` | architecture | active | 10,139 |
| `docs/architecture/THREADING.md` | architecture | active | 9,579 |
| `docs/architecture/ISOLATION.md` | architecture | active | 8,952 |
| `docs/architecture/MODDING.md` | architecture/dev | active | 12,695 |
| `docs/architecture/MOD_PIPELINE.md` | architecture/dev | active v0.2 (line 4 says `Architecture version: v0.2  \|  Implementation phase: 2`) | 13,394 |
| `docs/methodology/CODING_STANDARDS.md` | development | active | 6,319 |
| `docs/methodology/DEVELOPMENT_HYGIENE.md` | development | active | 9,966 |
| `docs/methodology/TESTING_STRATEGY.md` | development | active | 6,347 |
| `docs/architecture/PERFORMANCE.md` | development | active | 7,266 |
| `docs/methodology/PIPELINE_METRICS.md` | methodology empirics | active v0.1 (line 13 says "Status: v0.1 (2026-04-28)") | 10,568 |
| `docs/architecture/GPU_COMPUTE.md` | research deferred | active (deferred decision) | 2,990 |
| `docs/reports/NATIVE_CORE_EXPERIMENT.md` | research / experiment | "Experiment — awaiting benchmark results" (line 13) | 18,914 |
| `docs/architecture/VISUAL_ENGINE.md` | architecture | active | 5,932 |
| `docs/architecture/GODOT_INTEGRATION.md` | architecture | active | 9,615 |
| `docs/architecture/RESOURCE_MODELS.md` | v0.2 addendum | active | 4,412 |
| `docs/architecture/COMPOSITE_REQUESTS.md` | v0.2 addendum | active | 6,626 |
| `docs/FEEDBACK_LOOPS.md` | v0.2 addendum | active | 5,079 |
| `docs/architecture/COMBO_RESOLUTION.md` | v0.2 addendum | active | 4,983 |
| `docs/architecture/OWNERSHIP_TRANSITION.md` | v0.2 addendum | active | 6,340 |
| `docs/TRANSLATION_GLOSSARY.md` | translation | locked v1.0 (line 8 says "Status: v1.0 (locked)") | 49,589 |
| `docs/TRANSLATION_PLAN.md` | translation | active v0.1 draft (line 10 says "Version: 0.1 (draft, 2026-04-26)") | 17,643 |
| `docs/reports/NORMALIZATION_REPORT.md` | translation Pass 1 artifact | "Status: complete. Pass 2 may begin once the human resolves the two escalations recorded in §6." (line 8) | 31,442 |

### `docs/audit/`

| Path | Type | Status (from header or content) | File size (bytes) |
|---|---|---|---|
| `docs/audit/AUDIT_CAMPAIGN_PLAN.md` | audit-campaign plan | LOCKED v1.0 (line 10 says "Status: LOCKED v1.0 — Phase 0 кампании закрыт") | 49,908 |
| `docs/audit/AUDIT_PASS_1_PROMPT.md` | audit prompt (this Pass) | "Версия v1.0, выпущена 2026-05-01" (line 8) | 40,092 |
| `docs/audit/M3_CLOSURE_REVIEW.md` | audit-trail | historical/frozen — Date: 2026-04-29, branch `main` (line 9) | 29,639 |
| `docs/audit/M4_CLOSURE_REVIEW.md` | audit-trail | historical/frozen — Date: 2026-04-29, branch `main` (line 9) | 40,419 |
| `docs/audit/M5_CLOSURE_REVIEW.md` | audit-trail | historical/frozen — Date: 2026-04-30, branch `feat/m4-shared-alc` (line 9) | 47,172 |
| `docs/audit/M6_CLOSURE_REVIEW.md` | audit-trail | historical/frozen — Date: 2026-05-01, branch `feat/m4-shared-alc` (line 9) | 52,549 |
| `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md` | audit-trail (Russian) | preserved Russian audit trail (per TRANSLATION_PLAN whitelist) | 31,372 |
| `docs/audit/PASS_2_NOTES.md` | translation Pass 2 notes | "Status: open — Pass 2 in progress, started 2026-04-26." (line 8) | 11,137 |
| `docs/audit/PASS_3_NOTES.md` | translation Pass 3 notes | "Status: complete. Cyrillic-grep clean across `src/`, `tests/`, `mods/`." (line 8) | 5,996 |
| `docs/audit/PASS_4_REPORT.md` | translation Pass 4 verification | Date: 2026-04-27, branch `chore/translation-pass-3` (line 9) | 26,846 |

### `docs/learning/`

| Path | Type | Status | File size (bytes) |
|---|---|---|---|
| `docs/learning/PHASE_1.md` | learning ritual artifact | "Version 1.0, 2026-04-25" (line 9), translated to English on 2026-04-27 (line 5) | 29,683 |

### Sub-folder READMEs

| Path | File size (bytes) | Notes |
|---|---|---|
| `mods/README.md` | 638 | mods/ root nav |
| `mods/DualFrontier.Mod.Example/README.md` | (not measured this pass; present per `find`) | example mod |
| `tests/README.md` | (not measured this pass; present per `find`) | tests/ root nav |
| `tests/DualFrontier.Core.Tests/README.md` | (not measured this pass; present per `find`) | core unit tests |
| `tests/DualFrontier.Modding.Tests/README.md` | (not measured this pass; present per `find`) | mod loader tests |
| `tests/DualFrontier.Systems.Tests/README.md` | (not measured this pass; present per `find`) | game-system tests |
| `src/DualFrontier.AI/README.md` | (not measured this pass; present per `find`) | AI assembly |
| `src/DualFrontier.AI/BehaviourTree/README.md` | (not measured this pass; present per `find`) | behaviour tree |
| `src/DualFrontier.AI/Jobs/README.md` | (not measured this pass; present per `find`) | jobs |
| `src/DualFrontier.AI/Pathfinding/README.md` | (not measured this pass; present per `find`) | pathfinding |
| `src/DualFrontier.Application/README.md` | (not measured this pass; present per `find`) | application root |
| `src/DualFrontier.Application/Bridge/README.md` | (not measured this pass; present per `find`) | bridge |
| `src/DualFrontier.Application/Bridge/Commands/README.md` | (not measured this pass; present per `find`) | bridge commands |
| `src/DualFrontier.Application/Input/README.md` | (not measured this pass; present per `find`) | input |
| `src/DualFrontier.Application/Loop/README.md` | (not measured this pass; present per `find`) | loop |
| `src/DualFrontier.Application/Modding/README.md` | (not measured this pass; present per `find`) | modding |
| `src/DualFrontier.Application/Rendering/README.md` | (not measured this pass; present per `find`) | rendering |
| `src/DualFrontier.Application/Save/README.md` | (not measured this pass; present per `find`) | save |
| `src/DualFrontier.Application/Scenario/README.md` | (not measured this pass; present per `find`) | scenario |
| `src/DualFrontier.Application/Scene/README.md` | (not measured this pass; present per `find`) | scene |
| `src/DualFrontier.Components/README.md` | (not measured this pass; present per `find`) | components root |
| `src/DualFrontier.Components/Building/README.md` | (not measured this pass; present per `find`) | building |
| `src/DualFrontier.Components/Combat/README.md` | (not measured this pass; present per `find`) | combat components |
| `src/DualFrontier.Components/Magic/README.md` | (not measured this pass; present per `find`) | magic components |
| `src/DualFrontier.Components/Pawn/README.md` | (not measured this pass; present per `find`) | pawn components |
| `src/DualFrontier.Components/Shared/README.md` | (not measured this pass; present per `find`) | shared components |
| `src/DualFrontier.Components/World/README.md` | (not measured this pass; present per `find`) | world components |
| `src/DualFrontier.Contracts/README.md` | (not measured this pass; present per `find`) | contracts root |
| `src/DualFrontier.Contracts/Attributes/README.md` | (not measured this pass; present per `find`) | attributes |
| `src/DualFrontier.Contracts/Bus/README.md` | (not measured this pass; present per `find`) | bus contracts |
| `src/DualFrontier.Contracts/Core/README.md` | (not measured this pass; present per `find`) | core markers |
| `src/DualFrontier.Contracts/Modding/README.md` | (not measured this pass; present per `find`) | modding contracts |
| `src/DualFrontier.Core/README.md` | (not measured this pass; present per `find`) | core root |
| `src/DualFrontier.Core/Bus/README.md` | (not measured this pass; present per `find`) | bus implementation |
| `src/DualFrontier.Core/ECS/README.md` | (not measured this pass; present per `find`) | ECS internals |
| `src/DualFrontier.Core/Math/README.md` | (not measured this pass; present per `find`) | math helpers |
| `src/DualFrontier.Core/Registry/README.md` | (not measured this pass; present per `find`) | registry |
| `src/DualFrontier.Core/Scheduling/README.md` | (not measured this pass; present per `find`) | scheduling |
| `src/DualFrontier.Events/README.md` | (not measured this pass; present per `find`) | events root |
| `src/DualFrontier.Events/Combat/README.md` | (not measured this pass; present per `find`) | combat events |
| `src/DualFrontier.Events/Inventory/README.md` | (not measured this pass; present per `find`) | inventory events |
| `src/DualFrontier.Events/Magic/README.md` | (not measured this pass; present per `find`) | magic events |
| `src/DualFrontier.Events/Pawn/README.md` | (not measured this pass; present per `find`) | pawn events |
| `src/DualFrontier.Events/Power/README.md` | (not measured this pass; present per `find`) | power events |
| `src/DualFrontier.Events/World/README.md` | (not measured this pass; present per `find`) | world events |
| `src/DualFrontier.Persistence/README.md` | (not measured this pass; present per `find`) | persistence |
| `src/DualFrontier.Presentation/README.md` | (not measured this pass; present per `find`) | presentation root |
| `src/DualFrontier.Presentation/addons/df_devkit/README.md` | (not measured this pass; present per `find`) | df_devkit plugin |
| `src/DualFrontier.Presentation/Input/README.md` | (not measured this pass; present per `find`) | presentation input |
| `src/DualFrontier.Presentation/Nodes/README.md` | (not measured this pass; present per `find`) | presentation nodes |
| `src/DualFrontier.Presentation/Scenes/README.md` | (not measured this pass; present per `find`) | presentation scenes |
| `src/DualFrontier.Presentation/UI/README.md` | (not measured this pass; present per `find`) | presentation UI |
| `src/DualFrontier.Presentation.Native/README.md` | (not measured this pass; present per `find`) | native presentation |
| `src/DualFrontier.Systems/README.md` | (not measured this pass; present per `find`) | systems root |
| `src/DualFrontier.Systems/Combat/README.md` | (not measured this pass; present per `find`) | combat systems |
| `src/DualFrontier.Systems/Faction/README.md` | (not measured this pass; present per `find`) | faction systems |
| `src/DualFrontier.Systems/Inventory/README.md` | (not measured this pass; present per `find`) | inventory systems |
| `src/DualFrontier.Systems/Magic/README.md` | (not measured this pass; present per `find`) | magic systems |
| `src/DualFrontier.Systems/Magic/Internal/README.md` | (not measured this pass; present per `find`) | magic systems internal |
| `src/DualFrontier.Systems/Pawn/README.md` | (not measured this pass; present per `find`) | pawn systems |
| `src/DualFrontier.Systems/Power/README.md` | (not measured this pass; present per `find`) | power systems |
| `src/DualFrontier.Systems/World/README.md` | (not measured this pass; present per `find`) | world systems |

Source for sub-folder list: `find D:/Colony_Simulator/Colony_Simulator/{src,tests,mods} -name README.md` returned 60 paths total (including the two root-level `tests/README.md` and `mods/README.md`).

---

## §5 Symbol inventory

Public surface of `DualFrontier.Contracts` enumerated by reading every `.cs` file under `src/DualFrontier.Contracts/` (excluding `bin/`, `obj/`). Sources verified from file contents.

| Namespace | Type | Kind | File:line |
|---|---|---|---|
| `DualFrontier.Contracts.Attributes` | `BridgeImplementationAttribute` | sealed class (attribute) | `BridgeImplementationAttribute.cs:20` |
| `DualFrontier.Contracts.Attributes` | `DeferredAttribute` | sealed class (attribute) | `DeferredAttribute.cs:13` |
| `DualFrontier.Contracts.Attributes` | `ImmediateAttribute` | sealed class (attribute) | `ImmediateAttribute.cs:12` |
| `DualFrontier.Contracts.Attributes` | `ModAccessibleAttribute` | sealed class (attribute) | `ModAccessibleAttribute.cs:16` |
| `DualFrontier.Contracts.Attributes` | `ModCapabilitiesAttribute` | sealed class (attribute) | `ModCapabilitiesAttribute.cs:24` |
| `DualFrontier.Contracts.Attributes` | `SystemAccessAttribute` | sealed class (attribute) | `SystemAccessAttribute.cs:17` |
| `DualFrontier.Contracts.Attributes` | `TickRateAttribute` | sealed class (attribute) | `TickRateAttribute.cs:13` |
| `DualFrontier.Contracts.Attributes` | `TickRates` | static class (constants) | `TickRateAttribute.cs:37` |
| `DualFrontier.Contracts.Bus` | `EventBusAttribute` | sealed class (attribute) | `EventBusAttribute.cs:20` |
| `DualFrontier.Contracts.Bus` | `IEventBus` | interface | `IEventBus.cs:14` |
| `DualFrontier.Contracts.Bus` | `IGameServices` | interface | `IGameServices.cs:13` |
| `DualFrontier.Contracts.Bus` | `ICombatBus` | interface | `ICombatBus.cs:11` |
| `DualFrontier.Contracts.Bus` | `IInventoryBus` | interface | `IInventoryBus.cs:11` |
| `DualFrontier.Contracts.Bus` | `IMagicBus` | interface | `IMagicBus.cs:11` |
| `DualFrontier.Contracts.Bus` | `IPawnBus` | interface | `IPawnBus.cs:10` |
| `DualFrontier.Contracts.Bus` | `IPowerBus` | interface | `IPowerBus.cs:17` |
| `DualFrontier.Contracts.Bus` | `IWorldBus` | interface | `IWorldBus.cs:11` |
| `DualFrontier.Contracts.Core` | `EntityId` | readonly record struct | `EntityId.cs:21` |
| `DualFrontier.Contracts.Core` | `ICommand` | interface (marker) | `ICommand.cs:10` |
| `DualFrontier.Contracts.Core` | `IComponent` | interface (marker) | `IComponent.cs:10` |
| `DualFrontier.Contracts.Core` | `IEntity` | interface (marker) | `IEntity.cs:11` |
| `DualFrontier.Contracts.Core` | `IEvent` | interface (marker) | `IEvent.cs:11` |
| `DualFrontier.Contracts.Core` | `IQuery` | interface (marker) | `IQuery.cs:11` |
| `DualFrontier.Contracts.Core` | `IQueryResult` | interface (marker) | `IQueryResult.cs:9` |
| `DualFrontier.Contracts.Enums` | `OwnershipMode` | enum | `OwnershipMode.cs:10` |
| `DualFrontier.Contracts.Math` | `GridVector` | readonly record struct | `GridVector.cs:6` |
| `DualFrontier.Contracts.Modding` | `CapabilityViolationException` | sealed class (exception) | `CapabilityViolationException.cs:9` |
| `DualFrontier.Contracts.Modding` | `ContractsVersion` | readonly struct | `ContractsVersion.cs:11` |
| `DualFrontier.Contracts.Modding` | `IMod` | interface | `IMod.cs:11` |
| `DualFrontier.Contracts.Modding` | `IModApi` | interface | `IModApi.cs:16` |
| `DualFrontier.Contracts.Modding` | `IModContract` | interface (marker) | `IModContract.cs:12` |
| `DualFrontier.Contracts.Modding` | `ManifestCapabilities` | readonly record struct | `ManifestCapabilities.cs:10` |
| `DualFrontier.Contracts.Modding` | `ModDependency` | sealed record | `ModDependency.cs:9` |
| `DualFrontier.Contracts.Modding` | `ModKind` | enum | `ModManifest.cs:7` |
| `DualFrontier.Contracts.Modding` | `ModLogLevel` | enum | `ModLogLevel.cs:7` |
| `DualFrontier.Contracts.Modding` | `ModManifest` | sealed class | `ModManifest.cs:24` |
| `DualFrontier.Contracts.Modding` | `VersionConstraint` | readonly struct | `VersionConstraint.cs:21` |
| `DualFrontier.Contracts.Modding` | `VersionConstraintKind` | enum | `VersionConstraint.cs:7` |

**Per-namespace count:**

| Namespace | Type count |
|---|---|
| `DualFrontier.Contracts.Attributes` | 8 |
| `DualFrontier.Contracts.Bus` | 9 |
| `DualFrontier.Contracts.Core` | 7 |
| `DualFrontier.Contracts.Enums` | 1 |
| `DualFrontier.Contracts.Math` | 1 |
| `DualFrontier.Contracts.Modding` | 12 |
| **Total public types in Contracts** | **38** |

**Note on `ValidationErrorKind`:** The prompt template lists `ValidationErrorKind` under `DualFrontier.Contracts.Modding`. Filesystem location of this enum is `src/DualFrontier.Application/Modding/ValidationError.cs:9` (namespace `DualFrontier.Application.Modding`), not under Contracts. Enum members are catalogued in §9 below. This is recorded as an inventory observation in §11.

---

## §6 Manifest schema inventory

`mod.manifest.json` files at source paths (excluding `bin/Debug/`, `bin/Release/` build-output copies). Source: `find D:/Colony_Simulator/Colony_Simulator/{tests,mods} -name mod.manifest.json` filtered to non-bin.

| Path | Fields present | Kind | Capabilities required (count) | Replaces (count) |
|---|---|---|---|---|
| `tests/Fixture.RegularMod_DependedOn/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType | regular | 0 | 0 |
| `tests/Fixture.RegularMod_DependsOnAnother/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, dependencies (1 entry) | regular | 0 | 0 |
| `tests/Fixture.RegularMod_DependsOnBadApi/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, dependencies (1 entry with version `^99.0.0`) | regular | 0 | 0 |
| `tests/Fixture.RegularMod_DepsBadVersion/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, dependencies (1 entry with version `^99.0.0`) | regular | 0 | 0 |
| `tests/Fixture.RegularMod_BadApiVersion/mod.manifest.json` | id, name, version, author, kind, apiVersion (`^99.0.0`), entryAssembly, entryType | regular | 0 | 0 |
| `tests/Fixture.RegularMod_CyclicA/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, dependencies (1 entry: `tests.regular.cycb`) | regular | 0 | 0 |
| `tests/Fixture.RegularMod_CyclicB/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, dependencies (1 entry: `tests.regular.cyca`) | regular | 0 | 0 |
| `tests/Fixture.RegularMod_MissingOptional/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, dependencies (1 entry with `optional: true`) | regular | 0 | 0 |
| `tests/Fixture.RegularMod_MissingRequired/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, dependencies (1 entry without optional) | regular | 0 | 0 |
| `tests/Fixture.RegularMod_ReplacesCombat/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, replaces (1 FQN) | regular | 0 | 1 (`DualFrontier.Systems.Combat.CombatSystem`) |
| `tests/Fixture.RegularMod_ReplacesCombat_Alt/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, replaces (1 FQN) | regular | 0 | 1 (`DualFrontier.Systems.Combat.CombatSystem`) |
| `tests/Fixture.RegularMod_ReplacesProtected/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, replaces (1 FQN) | regular | 0 | 1 (`DualFrontier.Systems.Pawn.SocialSystem`) |
| `tests/Fixture.RegularMod_ReplacesUnknown/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType, replaces (1 FQN) | regular | 0 | 1 (`DualFrontier.Phantom.NonExistentSystem`) |
| `tests/Fixture.SharedEvents/mod.manifest.json` | id, name, version, author, kind | shared | 0 | 0 |
| `tests/Fixture.SubscriberMod/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType | regular | 0 | 0 |
| `tests/Fixture.PublisherMod/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType | regular | 0 | 0 |
| `tests/Fixture.BadRegularMod/mod.manifest.json` | id, name, version, author, kind, entryAssembly, entryType | regular | 0 | 0 |
| `tests/Fixture.BadSharedMod_WithIMod/mod.manifest.json` | id, name, version, author, kind | shared | 0 | 0 |
| `mods/DualFrontier.Mod.Example/mod.manifest.json` | id, name, version, author, description, entryAssembly, entryType, dependencies (empty) | (kind absent — defaults to regular per ModManifest.cs:82) | 0 | 0 |

**Per-manifest detail records:**

- None of the catalogued manifests declare `apiVersion` other than `Fixture.RegularMod_BadApiVersion` (which sets `^99.0.0`). All others rely on the default behaviour of `ModManifest.EffectiveApiVersion` (per ModManifest.cs:119 — falls back to parsing `RequiresContractsVersion`, which is itself defaulted to `1.0.0`).
- None of the catalogued manifests declare `hotReload` explicitly. Default per ModManifest.cs:96 is `false`.
- None of the catalogued manifests declare `capabilities`. Default per ModManifest.cs:112 is `ManifestCapabilities.Empty`.
- `mods/DualFrontier.Mod.Example/mod.manifest.json` is the only catalogued manifest that contains a `description` field. `description` is not a recognised field in the v2 schema per `MOD_OS_ARCHITECTURE.md` §2.2 field reference table.

---

## §7 Closure review inventory

| File | Date (from frontmatter or §0) | Reviewed phase | Mentions HEAD/upper-bound commit | Status |
|---|---|---|---|---|
| `docs/audit/M3_CLOSURE_REVIEW.md` | 2026-04-29 | M3 (M3.1, M3.2, M3.3 closed; M3.4 deferred) | `a73669f..95935d7` (six commits inclusive — line 9) | historical/frozen |
| `docs/audit/M4_CLOSURE_REVIEW.md` | 2026-04-29 | M4 (M4.1, M4.2, M4.3) | `0a3a858..2a707f3` (eighteen commits inclusive — line 9) | historical/frozen |
| `docs/audit/M5_CLOSURE_REVIEW.md` | 2026-04-30 | M5 (M5.1, M5.2) | `fffd785..5c0d1b5` (eight commits inclusive — line 9) | historical/frozen |
| `docs/audit/M6_CLOSURE_REVIEW.md` | 2026-05-01 | M6 (M6.1, M6.2; M6.3 closure-sync mechanism) | `1af73ad..e643011` (seven commits inclusive — line 9) | historical/frozen |
| `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md` | 2026-04-25 (per body field "Дата" line 11) | Phase 4 | not stated as range; documents single Opus session | preserved Russian audit trail |

Translation-campaign audit-trail artifacts also present in `docs/audit/`:

| File | Date | Reviewed pass | Status |
|---|---|---|---|
| `docs/audit/PASS_2_NOTES.md` | started 2026-04-26 | Pass 2 (documentation translation) | "open — Pass 2 in progress" (line 8) |
| `docs/audit/PASS_3_NOTES.md` | 2026-04-27 | Pass 3 (code translation) | "complete. Cyrillic-grep clean across `src/`, `tests/`, `mods/`" (line 8) |
| `docs/audit/PASS_4_REPORT.md` | 2026-04-27 | Pass 4 (verification) | branch `chore/translation-pass-3`; surgical fix `352ff0f` (line 9) |

**Ratification entries in `docs/architecture/MOD_OS_ARCHITECTURE.md` Version history (lines 10–29):**

| Version | Source review | Entry summary (verbatim quote of headline) |
|---|---|---|
| v0.1 | initial draft | "initial specification of the mod-as-process model. Five strategic decisions locked; seven detail decisions (D-1 through D-7) collected in §12 as ⚠ DECISION pending human resolution." |
| v1.0 | Phase 0 closure | "Phase 0 closed. All seven open decisions resolved and locked. Implementation phases M1–M10 may begin." |
| v1.1 | M1–M3.1 audit | "non-semantic corrections from the first independent audit (M1–M3.1)" — §4.1 `Log()` parameter type → `ModLogLevel`; §2.2 `dependencies[i].optional` documented |
| v1.2 | M3 closure review | "non-semantic corrections from the M3 closure review" — §3.6 hybrid enforcement formulation; §3.5+§2.1 example manifest; §11.1 M3.4 deferred |
| v1.3 | M4.3 implementation review | "non-semantic correction from the M4.3 implementation review" — §2.2 `entryAssembly`/`entryType` "must be empty for `kind=shared`" |
| v1.4 | M7 pre-flight readiness review | "non-semantic clarifications from the M7 pre-flight readiness review" — §9.5 step 7 GC pump bracket; §9.5.1 failure semantics |

---

## §8 Cyrillic inventory

Search executed via Grep over `.cs` files using regex `[А-Яа-яЁё]` (covers Unicode range U+0400–U+04FF including the Ё/ё characters which sit at U+0401/U+0451).

| Scope | Files scanned | `.cs` files with any Cyrillic |
|---|---|---|
| `src/**/*.cs` | (Grep returned `No files found` for matches) | 0 |
| `tests/**/*.cs` | (Grep returned `No files found` for matches) | 0 |
| `mods/**/*.cs` | (Grep returned `No files found` for matches) | 0 |

**Summary:**

- Total `.cs` files with any Cyrillic across `src/`, `tests/`, `mods/`: **0**.
- Per-area breakdown: every Grep run reported `No files found` for the regex.

(Translation-campaign Pass 3 closure note in `docs/audit/PASS_3_NOTES.md:11–12` reports the same condition: "`grep -rE --include='*.cs' --include='*.md' '[А-Яа-я]' src/ tests/ mods/` returns zero matches.")

---

## §9 Sequence catalogue

Catalogue of every numbered/lettered sequence observed in spec, roadmap, code, and closure reviews. Source attribution included for each entry.

| # | Source | Location | Sequence label | Observed numbers/identifiers | Stated total (if any) |
|---|---|---|---|---|---|
| 1 | `MOD_OS_ARCHITECTURE.md` | Preamble (lines 56–62) | strategic locked decisions | 1, 2, 3, 4, 5 | "Five top-level decisions" (line 56) |
| 2 | `MOD_OS_ARCHITECTURE.md` | §0 OS mapping table (lines 73–84) | OS-concept rows | 11 rows (Kernel, Process, Process isolation, Syscall, IPC, Device driver, Register new syscall, Shared library, dlopen/hot reload, Capability model, Package manager) | — |
| 3 | `MOD_OS_ARCHITECTURE.md` | §1 (lines 90–122) | mod kinds | §1.1 Regular, §1.2 Shared, §1.3 Vanilla | "three mod kinds" (§1 header, line 90); "three categories" (line 92) |
| 4 | `MOD_OS_ARCHITECTURE.md` | §1.4 (lines 152–157) | load-graph invariants | 4 bullets (Shared ALC singleton; Each regular mod own ALC; Regular ALC may resolve from shared; Cycles forbidden) | — |
| 5 | `MOD_OS_ARCHITECTURE.md` | §2.2 (lines 209–224) | manifest field reference | id, name, version, author, kind, apiVersion, entryAssembly, entryType, hotReload, dependencies, replaces, capabilities.required, capabilities.provided | 13 rows |
| 6 | `MOD_OS_ARCHITECTURE.md` | §2.3 (lines 227–233) | parse-time validation steps | 1, 2, 3, 4, 5, 6 | 6 numbered |
| 7 | `MOD_OS_ARCHITECTURE.md` | §3.6 (lines 307–311) | runtime cases the load-time gate cannot reach | 1, 2, 3 (Reflection bypass; Runtime-constructed events; v1 manifest grace period) | "three cases" (line 307) |
| 8 | `MOD_OS_ARCHITECTURE.md` | §4.1 (lines 343–378) | IModApi v2 surface methods | RegisterComponent, RegisterSystem, Publish, Subscribe, PublishContract, TryGetContract, GetKernelCapabilities, GetOwnManifest, Log | 9 method declarations (v1 had 6, v2 adds 3 per line 340) |
| 9 | `MOD_OS_ARCHITECTURE.md` | §5.2 (lines 432–438) | shared mod loader steps | 1, 2, 3, 4, 5, 6 | 6 numbered |
| 10 | `MOD_OS_ARCHITECTURE.md` | §5.3 (lines 442–448) | regular mod (with shared dep) loader steps | 1, 2, 3, 4, 5 | 5 numbered |
| 11 | `MOD_OS_ARCHITECTURE.md` | §5.4 (lines 451–455) | shared mod restrictions | 4 bullets (only types; cannot reference Core/Application; no static ctor with mutable state; no env/file/network at type-load) | — |
| 12 | `MOD_OS_ARCHITECTURE.md` | §7.1 (lines 559–565) | bridge-replacement mechanism steps | 1, 2, 3, 4 | 4 numbered |
| 13 | `MOD_OS_ARCHITECTURE.md` | §7.5 (lines 593–599) | bridge-replacement test scenarios | 5 bullets (Replaceable=true success; Two mods → BridgeReplacementConflict; Replaceable=false → ProtectedSystemReplacement; Non-existent FQN → UnknownSystemReplacement; Mod is unloaded — replacement skip is reverted) | — (5 bullets observed; no "of N" wording in §7.5 itself) |
| 14 | `MOD_OS_ARCHITECTURE.md` | §8.4 (lines 632–636) | constraint syntaxes | Exact, Caret, Tilde (rejected) | "Three syntaxes are supported" (line 632) — observed: 2 supported + 1 rejected |
| 15 | `MOD_OS_ARCHITECTURE.md` | §8.6 (lines 668–672) | where each axis applies | apiVersion (Axis 1); version (Axis 2); dependencies[i].version (Axis 3) | 3 rows |
| 16 | `MOD_OS_ARCHITECTURE.md` | §8.7 (lines 678–684) | resolution algorithm | 1, 2, 3 (with sub-steps a, b, c), 4 | 4 numbered top-level |
| 17 | `MOD_OS_ARCHITECTURE.md` | §9.1 (lines 696–724) | lifecycle states | Disabled, Pending, Loaded, Active, Stopping, Disabled (terminal) | "six well-defined states" (line 692); diagram contains 6 boxes; unique state-name labels: 5 (Disabled appears twice — initial and terminal) |
| 18 | `MOD_OS_ARCHITECTURE.md` | §9.2 (lines 730–737) | hot reload menu flow steps | 1, 2, 3, 4 | 4 numbered |
| 19 | `MOD_OS_ARCHITECTURE.md` | §9.5 (lines 757–765) | unload chain steps | 1, 2, 3, 4, 5, 6, 7 | 7 numbered (the v1.4 ratification of §9.5 step 7 explicitly references "step 7" by number) |
| 20 | `MOD_OS_ARCHITECTURE.md` | §10.1 (lines 787–796) | architectural threats caught | 8 rows (undeclared component access; bus publish; GetSystem; cast IModApi; system-conflict; replace-conflict; missing capability; publish without capability) | — |
| 21 | `MOD_OS_ARCHITECTURE.md` | §10.2 (lines 802–806) | architectural threats not caught | 5 bullets (Process.Kill / Environment.Exit; network/files/shell; unbounded memory/CPU; mutating IComponent after GetComponent; reflection on internals) | — |
| 22 | `MOD_OS_ARCHITECTURE.md` | §10.4 (lines 822–828) | required test categories | 7 bullets (Isolation; Capability violation; Bridge replacement; Type-sharing; WeakReference unload; Cross-mod cycle; Version constraint) | — |
| 23 | `MOD_OS_ARCHITECTURE.md` | §11.1 (lines 838–851) | migration phases | M0, M1, M2, M3, M3.4 (deferred), M4, M5, M6, M7, M8, M9, M10 | 12 rows (11 sequential M0..M10 + 1 deferred M3.4) |
| 24 | `MOD_OS_ARCHITECTURE.md` | §11.2 (lines 855–864) | new ValidationErrorKind entries (added by migration) | MissingCapability (M3); BridgeReplacementConflict (M6); ProtectedSystemReplacement (M6); UnknownSystemReplacement (M6); IncompatibleVersion (M5); SharedModWithEntryPoint (M4); ContractTypeInRegularMod (M4); CapabilityViolation (M3, runtime, "not part of the validation set, but listed here for completeness") | 8 bullets; opening line 854 says "The current enum has `MissingDependency` and `CyclicDependency`" (2 baseline) |
| 25 | `MOD_OS_ARCHITECTURE.md` | §11.4 (lines 873–875) | stop conditions | 3 bullets (capability cross-check >5s/mod; WR unload tests flaky; capability bypass via documented .NET features) | — |
| 26 | `MOD_OS_ARCHITECTURE.md` | §12 (lines 886–969) | detail decisions | D-1, D-2, D-3, D-4, D-5, D-6, D-7 | 7 entries (header text "seven detail decisions"; line 12 changelog says "seven detail decisions (D-1 through D-7)") |
| 27 | `ROADMAP.md` | Status overview (lines 13–34) | phase rows | Phase 0, Phase 1, Phase 2, Phase 3, Phase 3.5, Phase 4, Persistence (scaffold), M0, M1, M2, M3, M3.4, M4, M5, M6, M7, M8, M9, M10, Phase 9 | 20 rows |
| 28 | `ROADMAP.md` | Closed phases (lines 40–98) | closed phase headings | Phase 0, Phase 1, Phase 2, Phase 3, Phase 3.5, Phase 4, Persistence | 7 closed-phase headings |
| 29 | `ROADMAP.md` | M3 row (lines 25, 169–197) | sub-phases | M3.1, M3.2, M3.3, M3.4 (deferred) | — |
| 30 | `ROADMAP.md` | M4 row (lines 27, 201–221) | sub-phases | M4.1, M4.2, M4.3 | — |
| 31 | `ROADMAP.md` | M5 row (lines 28, 226–254) | sub-phases | M5.1, M5.2 | — |
| 32 | `ROADMAP.md` | M6 row (lines 29, 258–278) | sub-phases | M6.1, M6.2 (M6.3 documented as closure-sync mechanism, line 318) | — |
| 33 | `ROADMAP.md` | M7 row (lines 30, 282–329) | sub-phases | M7.1, M7.2, M7.3, M7.4, M7.5, M7-closure | "five implementation sub-phases (M7.1 – M7.5) plus a closure session" (line 284) |
| 34 | `ROADMAP.md` | M10 row (lines 33, 387–408) | vanilla slices | M10.A, M10.B, M10.C, M10.D | — |
| 35 | `ROADMAP.md` | Phase 4 v0.3 architectural fixes block (lines 82–89) | architectural-fix bullets | 6 bullets (`[Deferred]`/`[Immediate]`; `IPowerBus` added; Inventory events `[Deferred]`; `ConverterSystem.writes=[]`; `[BridgeImplementation]` replacing NotImplementedException; `HaulSystem` `continue` + DomainEventBus TOCTOU) | — |
| 36 | `ROADMAP.md` | Engine snapshot (line 36) | progressive test counts | 60, 82, 247, 260, 281, 311, 328, 333, 338, 369 | "Total at M7.3 closure: 369/369 passed" |
| 37 | `src/DualFrontier.Application/Modding/ContractValidator.cs` | class XML-doc (lines 12–48) | validator phases (alphabetical) | A, B, C, D, E, F, G, H | "Eight-phase validator" (line 13) |
| 38 | `src/DualFrontier.Application/Modding/ContractValidator.cs` | `Validate()` method body (lines 88–103) | validator-phase invocation order | A → B → E → G → H → [C, D if kernelCapabilities] → [F if sharedMods] | — (phases run unconditionally: A, B, E, G, H per class doc line 41) |
| 39 | `src/DualFrontier.Application/Modding/ValidationError.cs` | `enum ValidationErrorKind` (lines 9–83) | enum members | IncompatibleContractsVersion, WriteWriteConflict, CyclicDependency, MissingDependency, IncompatibleVersion, MissingCapability, SharedModWithEntryPoint, ContractTypeInRegularMod, BridgeReplacementConflict, ProtectedSystemReplacement, UnknownSystemReplacement | 11 members |
| 40 | `src/DualFrontier.Contracts/Modding/IModApi.cs` | interface (lines 16–75) | API methods (in declaration order) | RegisterComponent, RegisterSystem, Publish, Subscribe, PublishContract, TryGetContract, GetKernelCapabilities, GetOwnManifest, Log | 9 declarations |
| 41 | `src/DualFrontier.Contracts/Modding/ModLogLevel.cs` | `enum ModLogLevel` (lines 7–20) | severity levels | Debug, Info, Warning, Error | 4 members |
| 42 | `src/DualFrontier.Contracts/Modding/VersionConstraint.cs` | `enum VersionConstraintKind` (lines 7–14) | constraint kinds | Exact, Caret | 2 members |
| 43 | `src/DualFrontier.Contracts/Modding/ModManifest.cs` | `enum ModKind` (lines 7–17) | mod kinds | Regular, Shared | 2 members |
| 44 | `src/DualFrontier.Contracts/Enums/OwnershipMode.cs` | `enum OwnershipMode` (lines 10–37) | ownership modes | Bonded, Contested, Abandoned, Transferred | 4 members |
| 45 | `src/DualFrontier.Contracts/Attributes/TickRateAttribute.cs` | `static class TickRates` (lines 37–53) | tick-rate constants | REALTIME, FAST, NORMAL, SLOW, RARE | 5 constants |
| 46 | `src/DualFrontier.Contracts/Bus/IGameServices.cs` | interface properties (lines 13–57) | bus accessors | Combat, Inventory, Magic, Pawns, World, Power | 6 properties |
| 47 | `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` | violation-message methods (lines 270–319) | violation paths | BuildReadViolationMessage, BuildWriteViolationMessage, GetSystem (always-throws path) | 3 paths |
| 48 | `docs/audit/M3_CLOSURE_REVIEW.md` | §0 Executive summary | review checks | rows 1..8 | 8 rows (counted by `awk` over §0 region) |
| 49 | `docs/audit/M4_CLOSURE_REVIEW.md` | §0 Executive summary | review checks | rows 1..8 | 8 rows |
| 50 | `docs/audit/M5_CLOSURE_REVIEW.md` | §0 Executive summary | review checks | rows 1..8 | 8 rows |
| 51 | `docs/audit/M6_CLOSURE_REVIEW.md` | §0 Executive summary | review checks | rows 1..8 | 8 rows ("All 8 checks PASSED" — line 30) |
| 52 | `docs/audit/AUDIT_CAMPAIGN_PLAN.md` | §2 Структура кампании (line 78) | campaign passes | Pass 1, Pass 2, Pass 3, Pass 4, Pass 5 | "пять последовательных проходов" (line 29) |
| 53 | `docs/audit/AUDIT_CAMPAIGN_PLAN.md` | §7 ratified decisions (lines 381–419) | locked decisions | §7.1, §7.2, §7.3, §7.4, §7.5, §7.6 | 6 ratified decisions (header line 12 says "шесть открытых решений §7 ratified") |

**Note on diapason 4–6 (per AUDIT_CAMPAIGN_PLAN §5.2 "особое внимание"):** sequences with positions in the 4–6 range have been recorded verbatim above. Particular cases:
- §9.5 unload chain (entry 19): step 4 = "dependency graph rebuilt"; step 5 = "scheduler swaps to new phase list"; step 6 = "ALC.Unload()". All three present and in order in spec text (lines 760–764).
- §7.5 test scenarios (entry 13): bullets in source order — Replaceable success (1), BridgeReplacementConflict (2), ProtectedSystemReplacement (3), UnknownSystemReplacement (4), unload-revert (5). All five present.
- §12 detail decisions (entry 26): D-4 (loader scan for IModContract/IEvent in regular mods), D-5 (shared-mod cycle detection), D-6 (save-game compatibility policy). All three present and in order.
- ContractValidator phases (entry 37): D, E, F all present in class XML-doc enumeration A, B, C, D, E, F, G, H.
- ValidationErrorKind enum (entry 39): positions 4 (MissingDependency), 5 (IncompatibleVersion), 6 (MissingCapability) all present in source order.
- Closure review §0 checks 4–6 (entries 48–51): row 4 "Stale-reference sweep", row 5 "Methodology compliance", row 6 "Sub-phase acceptance criteria coverage" — uniform across M3, M4, M5, M6 reviews.

---

## §10 Surgical fixes applied this pass

None. Pass 1 is read-only by contract.

---

## §11 Items requiring follow-up

| # | Anomaly | Source | Note |
|---|---|---|---|
| 1 | Source-level test-attribute count: 357 `[Fact]` + 2 `[Theory]` = 359 attributes across 48 files. ROADMAP-stated runtime test count: 369. Difference: 359 − 369 = −10. | `tests/**/*.cs` (Grep `\[Fact`, `\[Theory`); `ROADMAP.md:36` engine-snapshot line "Total at M7.3 closure: 369/369 passed" | Pass 2/3 will classify. Pass 1 records both counts without comparison commentary. |
| 2 | Closure-review and translation-campaign artifacts present at `docs/audit/` rather than `docs/`. Working-tree state at session start: `git status` shows `D docs/M3_CLOSURE_REVIEW.md`, `D docs/M4_CLOSURE_REVIEW.md`, `D docs/M5_CLOSURE_REVIEW.md`, `D docs/M6_CLOSURE_REVIEW.md`, `D docs/PASS_2_NOTES.md`, `D docs/PASS_3_NOTES.md`, `D docs/PASS_4_REPORT.md`, `D docs/SESSION_PHASE_4_CLOSURE_REVIEW.md`, plus `?? docs/audit/`. | session-start gitStatus block; filesystem listing of `docs/audit/` returns these 8 files plus `AUDIT_CAMPAIGN_PLAN.md` and `AUDIT_PASS_1_PROMPT.md` | Pass 4 territory. |
| 3 | `MOD_OS_ARCHITECTURE.md` §9.1 line 692 reads "The mod lifecycle has six well-defined states." Diagram (lines 696–724) labels boxes with state names: Disabled, Pending, Loaded, Active, Stopping, Disabled. Unique state-name count: 5 (Disabled appears as both initial and terminal box). | `MOD_OS_ARCHITECTURE.md:692` and `MOD_OS_ARCHITECTURE.md:696–724` | Pass 2 sequence-integrity territory (per AUDIT_CAMPAIGN_PLAN §5). |
| 4 | `ContractValidator` class XML-doc (lines 12–48) names "Eight-phase validator" with phases A, B, C, D, E, F, G, H. `Validate()` method invocation order at lines 88–103 calls phases in non-alphabetical sequence: A, B, E, G, H, then conditionally C, D, then conditionally F. | `src/DualFrontier.Application/Modding/ContractValidator.cs` | Recorded for Pass 2 spec↔code consistency review. |
| 5 | `ValidationErrorKind` enum at `src/DualFrontier.Application/Modding/ValidationError.cs:9–83` has 11 members: IncompatibleContractsVersion, WriteWriteConflict, CyclicDependency, MissingDependency, IncompatibleVersion, MissingCapability, SharedModWithEntryPoint, ContractTypeInRegularMod, BridgeReplacementConflict, ProtectedSystemReplacement, UnknownSystemReplacement. `MOD_OS_ARCHITECTURE.md` §11.2 (lines 853–864) opens with "The current enum has `MissingDependency` and `CyclicDependency`. The migration adds:" followed by 8 bullets including `CapabilityViolation` flagged as runtime exception ("not part of the validation set, but listed here for completeness"). | `src/DualFrontier.Application/Modding/ValidationError.cs:9–83`; `MOD_OS_ARCHITECTURE.md:853–864` | Pass 2 territory. |
| 6 | `tests/DualFrontier.Modding.Tests/README.md:14` contents include "`.gitkeep` — placeholder. Real tests will arrive in Phase 2." Filesystem under `tests/DualFrontier.Modding.Tests/` (excluding `bin/`, `obj/`) contains 31 `.cs` files. | `tests/DualFrontier.Modding.Tests/README.md`; `find` count | Pass 4 territory (sub-folder README accuracy). |
| 7 | `tests/DualFrontier.Systems.Tests/README.md:13` contents include "`.gitkeep` — placeholder. Real tests will arrive in Phase 2+." Filesystem under `tests/DualFrontier.Systems.Tests/` contains 6 test `.cs` files. | `tests/DualFrontier.Systems.Tests/README.md`; `find` count | Pass 4 territory. |
| 8 | `src/DualFrontier.Contracts/README.md:17` lists five domain buses in the Bus/ contents description: `ICombatBus`, `IInventoryBus`, `IMagicBus`, `IPawnBus`, `IWorldBus`. `src/DualFrontier.Contracts/Bus/IGameServices.cs:13–57` declares six bus properties: Combat, Inventory, Magic, Pawns, World, Power. `src/DualFrontier.Contracts/Bus/README.md:5` describes "six domain buses (Combat, Inventory, Magic, Pawn, Power, World)". | `src/DualFrontier.Contracts/README.md:17`; `src/DualFrontier.Contracts/Bus/IGameServices.cs:13–57`; `src/DualFrontier.Contracts/Bus/README.md:5` | Pass 4 territory (sub-folder README accuracy). |
| 9 | `AUDIT_PASS_1_PROMPT.md` §4 contract table (lines 84–88) lists closure reviews under `docs/M3_CLOSURE_REVIEW.md` etc. Filesystem location: `docs/audit/M*_CLOSURE_REVIEW.md`. | `docs/audit/AUDIT_PASS_1_PROMPT.md:84–88`; `find` over `docs/audit/` | Inventory observation; Pass 1 recorded actual paths in §4 / §7 above. |
| 10 | `AUDIT_PASS_1_PROMPT.md` Appendix A (lines 691–711) example uses branch `feat/m4-shared-alc`. `.git/HEAD` line 1 reads `ref: refs/heads/main`. | `docs/audit/AUDIT_PASS_1_PROMPT.md:691–711`; `.git/HEAD:1` | Inventory observation only; Pass 1 recorded actual branch in §1. |
| 11 | `mods/DualFrontier.Mod.Example/mod.manifest.json` contains a `description` field. The v2 manifest schema in `MOD_OS_ARCHITECTURE.md` §2.2 (lines 209–224) field reference table does not list `description`. The manifest also omits the `kind` field (defaults to `regular` per `ModManifest.cs:82`). | `mods/DualFrontier.Mod.Example/mod.manifest.json`; `MOD_OS_ARCHITECTURE.md:209–224`; `src/DualFrontier.Contracts/Modding/ModManifest.cs:82` | Pass 2 territory. |
| 12 | M5 closure review header line 9 reports branch `feat/m4-shared-alc`; M6 closure review header line 9 reports branch `feat/m4-shared-alc`. `.git/HEAD` at session start: `ref: refs/heads/main`. `.git/logs/HEAD` shows `checkout: moving from feat/m4-shared-alc to main` event between commit `c7210ca` (M6 closure review docs commit) and commit `b504813` (v1.4 ratify on main, parent `06a9ff8` from `pull --ff origin`). | `docs/audit/M5_CLOSURE_REVIEW.md:9`, `docs/audit/M6_CLOSURE_REVIEW.md:9`, `.git/HEAD`, `.git/logs/HEAD` | Pass 3 territory (three-commit invariant / branch state consistency). |

(All 12 items recorded for Pass 2/3/4 attention. No assignment of priority, severity, or significance is made in this pass.)

---

## §12 Verification end-state

- **§0 Executive summary:** 9/9 PASSED.
- **Total facts captured:**
  - Test counts: 4 (Persistence) + 16 (Systems) + 60 (Core) + 279 (Modding) = 359 source-level attributes (357 `[Fact]` + 2 `[Theory]`); 0 skipped.
  - Spec version: `LOCKED v1.4`; v1.4 ratification entry 4-line block captured verbatim.
  - .md files catalogued: 29 in `docs/`, 10 in `docs/audit/`, 1 in `docs/learning/`, 2 root-level + 58 sub-folder = 100 markdown files (count of paths from `find`).
  - Public types in `DualFrontier.Contracts`: 38 across 6 namespaces.
  - Manifests catalogued: 19 source-path manifests (18 fixtures under `tests/Fixture.*/` + 1 mod under `mods/DualFrontier.Mod.Example/`).
  - Closure reviews and translation reports catalogued: 4 M-phase + 1 Phase 4 session + 3 translation passes + 1 campaign plan + 1 Pass 1 prompt = 10 audit-trail files in `docs/audit/`.
  - `.cs` files with Cyrillic content across `src/`, `tests/`, `mods/`: 0.
  - Sequences catalogued in §9: 53.
- **Anomalies in §11:** 12 items registered for Pass 2/3/4 attention.
- **Surgical fixes applied:** 0 (per contract).
- **Pass 1 status:** complete, ready for human ratification.
