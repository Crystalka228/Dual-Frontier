---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-M6_CLOSURE_REVIEW
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-M6_CLOSURE_REVIEW
---
---
title: M6 closure verification report
nav_order: 98
---

# M6 — Bridge replacement closure verification report

**Date:** 2026-05-01
**Branch:** `feat/m4-shared-alc` (commits `1af73ad..e643011`, seven commits inclusive; branch is forty-one commits ahead of `origin/main` and not yet pushed)
**Scope:** Verification only. No new architectural work. Surgical fixes
applied only for typos, broken cross-references, or clearly-wrong facts in
the new documents. Any structural finding is recorded in §10 as a
follow-up item rather than remediated in this session.

---

## §0 Executive summary

| # | Check | Status | Notes |
|---|---|---|---|
| 1 | Build & test integrity | **PASSED** | `dotnet build`: 0 warnings, 0 errors. `dotnet test` at HEAD: 338/338 (Persistence 4, Systems 16, Modding 258, Core 60). Three-commit invariant verified at every commit in the M6 closure batch. Test progression: 311 (M5 baseline) → 311 (M6.1 Phase 5 annotations) → 311 (M6.1 Phase H) → 328 (M6.1 tests) → 328 (M6.2 skip wiring) → 333 (M6.2 helper tests) → 338 (M6.2 integration tests) → 338 (M6.3 docs). |
| 2 | Spec ↔ code ↔ test triple consistency | **PASSED** | All five `MOD_OS_ARCHITECTURE` §7.5 / §11.1 acceptance bullets reachable inside a single load batch have all three legs (spec section, file:line, test name) present. The CENTRAL bridge-replacement demonstration is `Apply_WithReplaceableBridge_SkipsBridgeAndUsesModSystem`. Conflict / Protected / Unknown semantics demonstrated at validator and pipeline level independently. The §7.5 fifth bullet (mod-unloaded re-registration) is hot-reload territory and is registered as the M6→M7 hand-off. |
| 3 | Cross-document consistency | **PASSED** | `MOD_OS_ARCHITECTURE` v1.3 LOCKED unchanged through M6 (byte-identical from `32f6f04..HEAD`). `ROADMAP` header `Updated: 2026-05-01` with M6 ✅ Closed, M6.1/M6.2 sub-sections, and `338/338`. `docs/README` v1.3 LOCKED unchanged. |
| 4 | Stale-reference sweep | **PASSED** | All forbidden patterns return zero hits in active-navigation context. `311`, `328`, `333` survive only inside `M5_CLOSURE_REVIEW.md` (frozen audit trail) or as line-number citations in `NORMALIZATION_REPORT.md` (column header, not a test count). `v1.0`/`v1.1`/`v1.2 LOCKED` survive only as legitimate historical-attribution (M0 row, M3/M4/M5 reviews). No `M6 in progress` / `🔨 Current` markers active. |
| 5 | Methodology compliance | **PASSED** | All 7 commits have scope prefixes per METHODOLOGY §7.3; every commit carries a substantive body. The §12 LOCKED decisions D-1 through D-7 are byte-identical between v1.3 (M5 closure point) and HEAD — verified by `git diff 32f6f04..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md` returning zero output. **M6 introduces no spec change.** This is the strongest possible falsifiable signal that v1.3 LOCKED specification correctly drove implementation through entire M6 batch — same headline result as the M5 closure review. |
| 6 | Sub-phase acceptance criteria coverage | **PASSED** | Every acceptance bullet for M6.1 and M6.2 maps to an identifiable artifact (commit, file:line, test name). M6.3 is itself the closure mechanism (the ROADMAP-sync commit `e643011`), not a verifiable sub-phase. |
| 7 | Carried debts forward | **PASSED** | Phase 2 WeakReference unload tests still tracked in M7; Phase 3 `SocialSystem`/`SkillSystem` stubs still tracked in M10.C; M3.4 (CI Roslyn analyzer) remains `⏸ Deferred`. M6 introduces one new forward-tracked carried hand-off — §7.5 fifth test scenario (mod unloaded → replacement skip reverted, kernel bridge re-registers) is registered as M7's territory in the M6 closure note `ROADMAP.md:278`. Two non-blocking M9 architectural considerations registered in §10 (Phase B vs. Replaces ordering; three-way HealthComponent write overlap among Phase 5 stubs). |
| 8 | Ready-for-M7 readiness | **PASSED** | `ModIntegrationPipeline.Apply` already rebuilds graph from current mod set on every call — the unload-revert path reduces to "re-`Apply` without the unloaded mod" once M7's Pause/Resume + per-mod unload chain lands. `CollectReplacedFqns(loaded)` is parameter-driven and naturally regenerates the skip set when a mod is removed. `ModLoader._loaded`, `ModLoader.UnloadMod`, `ModRegistry.RemoveMod`, `IModContractStore.RevokeAll` infrastructure is all present (most from M0–M2). The 8-phase validator does not require a new phase for M7 — hot reload runs the existing pipeline against the new mod set. No M6 surface change blocks M7. |

**Result:** All 8 checks PASSED. Zero blocking findings. Zero surgical fixes
applied. M6 phase closes cleanly; M7 (Hot reload from menu) is unblocked.
Three non-blocking observations registered in §10 (one empirical
contradiction-rate datapoint; two M9 architectural considerations).

---

## §1 Build & test integrity

**`dotnet build DualFrontier.sln`** at HEAD (`e643011`):

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**`dotnet test DualFrontier.sln`** at HEAD:

| Project | Pass | Skip | Total |
|---|---|---|---|
| `DualFrontier.Persistence.Tests` | 4 | 0 | 4 |
| `DualFrontier.Systems.Tests` | 16 | 0 | 16 |
| `DualFrontier.Modding.Tests` | 258 | 0 | 258 |
| `DualFrontier.Core.Tests` | 60 | 0 | 60 |
| **Total** | **338** | **0** | **338** |

### Three-commit invariant (METHODOLOGY §7.3)

Each of the 7 commits in the M6 closure batch was checked out
independently against a clean working tree; `dotnet build` ran
`0 Warning(s)` / `0 Error(s)` at every checkout, and `dotnet test`
exit code was zero at every checkout. Per-commit total test counts:

| # | Commit | Sub-phase | Subject (truncated) | Build | Tests |
|---|---|---|---|---|---|
| — | `32f6f04` | (M5 closure) | docs(review): M5 closure verification report | 0 W / 0 E | **311** |
| 1 | `1af73ad` | M6.1 | feat(systems): annotate Phase 5 combat stubs as Replaceable bridges | 0 W / 0 E | **311** |
| 2 | `a408f44` | M6.1 | feat(modding): add Phase H bridge replacement validation to ContractValidator | 0 W / 0 E | **311** |
| 3 | `b0f1ee5` | M6.1 | test(modding): Phase H scenarios + Replaceable bridge annotation guards | 0 W / 0 E | **328** |
| 4 | `23f2933` | M6.2 | feat(modding): wire bridge replacement skip into ModIntegrationPipeline graph build | 0 W / 0 E | **328** |
| 5 | `602a84e` | M6.2 | test(modding): helper tests for CollectReplacedFqns | 0 W / 0 E | **333** |
| 6 | `adad506` | M6.2 | test(modding): M6.2 integration tests + replacement fixtures | 0 W / 0 E | **338** |
| 7 | `e643011` | M6.3 | docs(roadmap): close M6 — sync with M6.1 + M6.2 implementation | 0 W / 0 E | **338** |

The `311 → 328` jump at `b0f1ee5` matches the +17 delta declared in that
commit's body (eight tests in `PhaseHBridgeReplacementTests` plus nine
tests in `Phase5BridgeAnnotationsTests` = 7 Replaceable bridges + 2
protected-system guards). Distribution: Modding +8 (240→248),
Systems +9 (7→16). The `328 → 333` jump at `602a84e` matches the +5
delta from `CollectReplacedFqnsTests`. The `333 → 338` jump at
`adad506` matches the +5 delta from `M62IntegrationTests` (4 §7.5
scenarios + 1 regression guard) — the four new fixture projects
(Fixture.RegularMod_ReplacesCombat, _ReplacesCombat_Alt,
_ReplacesProtected, _ReplacesUnknown) ship in the same commit but
contribute zero direct tests; their assemblies are loaded by the
integration tests in the DualFrontier.Modding.Tests project.

The intermediate commits (`1af73ad` annotations, `a408f44` Phase H,
`23f2933` skip wiring) preserve the test count by not introducing
dependent assertions yet — the invariant is preserved by the absence of
dependent tests, not by accident. The annotation commit `1af73ad`
introduces no test because the guards for the seven annotated stubs are
themselves added in `b0f1ee5`; the validator commit `a408f44` likewise
adds Phase H code but defers its scenarios to `b0f1ee5`; the pipeline
skip wiring `23f2933` is the helper plumbing exercised by `602a84e` and
`adad506`. The `e643011` docs-only commit also preserves test count.

**Verdict:** PASSED. The three-commit invariant from METHODOLOGY §7.3
holds across the entire batch. No commit ships a broken build or a
failing test.

---

## §2 Spec ↔ code ↔ test triple consistency

`MOD_OS_ARCHITECTURE` §7 LOCKED + §11.1 (M6 row) declares M6 acceptance
through the §7.5 test scenarios plus the §11.1 acceptance bullets. The
fifth §7.5 scenario ("Mod is unloaded — replacement skip is reverted,
kernel bridge re-registers, dependency graph rebuilds") is hot-reload
territory and is registered as the M6→M7 hand-off (`ROADMAP.md:278`).
Each of the in-batch acceptance items is verified through three legs
(spec section, implementation file:line, dedicated test):

| # | Acceptance | Spec leg | Code leg | Test leg |
|---|---|---|---|---|
| 1 | Mod's `replaces` field declares FQN of system to supersede | §7.1 step 1 | [ModManifest.cs:98–105](../src/DualFrontier.Contracts/Modding/ModManifest.cs:98) `Replaces : IReadOnlyList<string>` (M1 plumbing); [ManifestParser](../src/DualFrontier.Application/Modding/ManifestParser.cs) (M1 plumbing) | Existing M1 manifest tests; M6 fixture manifests at [Fixture.RegularMod_ReplacesCombat/mod.manifest.json](../tests/Fixture.RegularMod_ReplacesCombat/mod.manifest.json) and three siblings exercise the field end-to-end |
| 2 | Loader reads all replaces, builds replacedSystems set | §7.1 step 1–2 | [ModIntegrationPipeline.cs:479–488](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:479) `CollectReplacedFqns` (private static); [ModIntegrationPipeline.cs:497–498](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:497) `CollectReplacedFqnsForTests` internal seam | [CollectReplacedFqnsTests.cs:28](../tests/DualFrontier.Modding.Tests/Pipeline/CollectReplacedFqnsTests.cs:28) `EmptyLoadedList_ReturnsEmptySet`; [CollectReplacedFqnsTests.cs:40](../tests/DualFrontier.Modding.Tests/Pipeline/CollectReplacedFqnsTests.cs:40) `SingleModWithMultipleReplaces_AllCollected`; [CollectReplacedFqnsTests.cs:61](../tests/DualFrontier.Modding.Tests/Pipeline/CollectReplacedFqnsTests.cs:61) `MultipleModsWithDistinctReplaces_AllCollected`; [CollectReplacedFqnsTests.cs:81](../tests/DualFrontier.Modding.Tests/Pipeline/CollectReplacedFqnsTests.cs:81) `MultipleModsWithSameFqn_DeduplicatedToSingle`; [CollectReplacedFqnsTests.cs:100](../tests/DualFrontier.Modding.Tests/Pipeline/CollectReplacedFqnsTests.cs:100) `ModWithEmptyReplaces_ContributesNothing` |
| 3 | Bridge skipped when its FQN in replacedSystems | §7.1 step 3 | [ModIntegrationPipeline.cs:300–324](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:300) graph-build skip loop in step `[5-7]` (filters `SystemOrigin.Core` whose `FQN` is in `replacedFqns`) | [M62IntegrationTests.cs:35](../tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs:35) `Apply_WithReplaceableBridge_SkipsBridgeAndUsesModSystem` asserts kernel `CombatSystem` is omitted from the rebuilt scheduler |
| 4 | Mod's replacement system registered in its place | §7.1 step 4 | Existing `RegisterSystem` flow (M2 plumbing), no M6 change required — replacement systems flow through `IModApi.RegisterSystem` like any other mod-supplied system, tagged `SystemOrigin.Mod` by `ModRegistry`, and reach graph build naturally | [M62IntegrationTests.cs:35](../tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs:35) `Apply_WithReplaceableBridge_SkipsBridgeAndUsesModSystem` asserts `Fixture.RegularMod_ReplacesCombat.ReplacementCombatSystem` lives in the rebuilt scheduler |
| 5 | Two mods replacing same FQN → `BridgeReplacementConflict` | §7.2 | [ContractValidator.cs:583–622](../src/DualFrontier.Application/Modding/ContractValidator.cs:583) Phase H step 1 with symmetric attribution (both mods get an error, each cross-referencing the other via `ConflictingModId`) | [PhaseHBridgeReplacementTests.cs:124](../tests/DualFrontier.Modding.Tests/Validator/PhaseHBridgeReplacementTests.cs:124) `TwoMods_ReplacingSameFqn_ProducesBridgeReplacementConflictBothSides`; [M62IntegrationTests.cs:120](../tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs:120) `Apply_WithTwoModsReplacingSameSystem_RejectsBatchWithConflict` (validator-level + pipeline-level) |
| 6 | `Replaceable=false` rejection | §7.4 | [ContractValidator.cs:626–666](../src/DualFrontier.Application/Modding/ContractValidator.cs:626) Phase H steps 2–3 emit `ProtectedSystemReplacement`; diagnostic distinguishes "Replaceable=false" from "attribute missing" cases | [PhaseHBridgeReplacementTests.cs:56](../tests/DualFrontier.Modding.Tests/Validator/PhaseHBridgeReplacementTests.cs:56) `Mod_WithProtectedSystemReplacement_ProducesError` asserts §7.4 substring; [PhaseHBridgeReplacementTests.cs:80](../tests/DualFrontier.Modding.Tests/Validator/PhaseHBridgeReplacementTests.cs:80) `Mod_WithSystemMissingBridgeAttribute_ProducesProtectedError`; [M62IntegrationTests.cs:67](../tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs:67) `Apply_WithProtectedSystemReplacement_RejectsBatch` (validator-level + pipeline-level) |
| 7 | Unknown FQN rejection | §7.5 line 4 | [ContractValidator.cs:636–647](../src/DualFrontier.Application/Modding/ContractValidator.cs:636) Phase H emits `UnknownSystemReplacement` when [ContractValidator.cs:676–685](../src/DualFrontier.Application/Modding/ContractValidator.cs:676) `ResolveTypeAcrossAssemblies` returns `null`; sweep covers every `AppDomain.CurrentDomain.GetAssemblies()` to handle kernel default-ALC + shared-ALC + collectible-ALC types | [PhaseHBridgeReplacementTests.cs:102](../tests/DualFrontier.Modding.Tests/Validator/PhaseHBridgeReplacementTests.cs:102) `Mod_WithUnknownFqn_ProducesUnknownSystemReplacementError` asserts §7.2 substring; [M62IntegrationTests.cs:96](../tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs:96) `Apply_WithUnknownFqn_RejectsBatch` (validator-level + pipeline-level) |
| 8 | Phase 5 combat stubs `Replaceable=true` | §7.4, §11.1 M6 row | All seven stubs in [src/DualFrontier.Systems/Combat/](../src/DualFrontier.Systems/Combat) annotated `[BridgeImplementation(Phase = 5, Replaceable = true)]`: `CombatSystem.cs:30`, `DamageSystem.cs:24`, `ProjectileSystem.cs:23`, `ShieldSystem.cs:22`, `StatusEffectSystem.cs:23`, `ComboResolutionSystem.cs:28`, `CompositeResolutionSystem.cs:30` | [Phase5BridgeAnnotationsTests.cs:20–60](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs:20) — seven `…_HasReplaceableBridgeAttribute` tests, one per Phase 5 system |
| 9 | Phase 3 stubs remain `Replaceable=false` (M10.C boundary) | implicit (carried debt in ROADMAP M10.C) | [SocialSystem.cs:22](../src/DualFrontier.Systems/Pawn/SocialSystem.cs:22) `[BridgeImplementation(Phase = 3)]` (default `Replaceable = false`); [SkillSystem.cs:21](../src/DualFrontier.Systems/Pawn/SkillSystem.cs:21) same — both files unchanged through M6 | [Phase5BridgeAnnotationsTests.cs:62](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs:62) `SocialSystem_RemainsProtected`; [Phase5BridgeAnnotationsTests.cs:75](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs:75) `SkillSystem_RemainsProtected` — M10.C boundary guards |

### §7.5 fifth scenario (M6→M7 hand-off)

§7.5 line 5 ("Mod is unloaded — replacement skip is reverted, kernel
bridge re-registers, dependency graph rebuilds") is **deliberately
out of scope for M6**. The closure note at `ROADMAP.md:278` records the
hand-off explicitly: "On-unload re-registration of skipped kernel
bridges is handled by M7's hot-reload path — at M6 closure, `Apply`
rebuilds the graph from the surviving mod set on every call, so the
unload case reduces to 're-`Apply` without the unloaded mod' once M7
lands." See §8 below for the architectural readiness analysis showing
why this hand-off is structurally clean.

### Regression guards

The M3–M5 surface continues to function unchanged after the M6.1
validator addition and the M6.2 pipeline wiring:

- [PhaseHBridgeReplacementTests.cs:154](../tests/DualFrontier.Modding.Tests/Validator/PhaseHBridgeReplacementTests.cs:154)
  `Mod_WithEmptyReplaces_PhaseHEarlyOuts` is the validator-level
  early-out guard: when no mod in the batch declares any replacement,
  Phase H must produce zero errors. Backed in code by the
  `anyReplacements` short-circuit at
  [ContractValidator.cs:570–581](../src/DualFrontier.Application/Modding/ContractValidator.cs:570).
- [M62IntegrationTests.cs:159](../tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs:159)
  `Apply_WithoutAnyReplaces_BridgesAllRegistered` is the pipeline-level
  regression guard: the skip path must NOT fire when no mod declares
  replaces. Five non-conflicting Phase 5 stubs are registered as Core
  and survive the rebuild. (Why a subset and not all seven: see §10
  M9 consideration A about the kernel-only HealthComponent
  three-stub overlap; the regression guard is meaningful with the
  non-conflicting subset and would be misleading if it tried to
  exercise an in-kernel state that vanilla.combat is the intended
  disambiguator for.)
- All 17 M6.1 tests (8 Phase H + 9 annotation guards) still pass after
  the M6.2 pipeline skip integration.
- All 311 tests from M3–M5 still pass after both M6.1 and M6.2.

**Verdict:** PASSED. All in-batch §7.5 scenarios + §11.1 M6 acceptance
bullets have all three legs present and verified, at validator-level
and at pipeline-level independently. The §7.5 fifth scenario (mod
unloaded) is correctly registered as M7 territory and not artificially
forced into M6.

---

## §3 Cross-document consistency

Three documents must agree on the v1.3 / M6-closed / 338-tests state:

| Document | Field | Expected | Found | Status |
|---|---|---|---|---|
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | Status line (line 8) | `LOCKED v1.3` | `LOCKED v1.3 — Phase 0 closed; non-semantic corrections from M1–M3.1 audit (v1.1), M3 closure review (v1.2), and M4.3 implementation review (v1.3) applied.` | ✓ |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | Byte-identity through M6 | zero changes since `32f6f04` | `git diff 32f6f04..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md` returns zero output | ✓ |
| `docs/ROADMAP.md` | Header date (line 11) | `2026-05-01` | `*Updated: 2026-05-01 (M6 closed — M6.1, M6.2 done; M7 next).*` | ✓ |
| `docs/ROADMAP.md` | M6 row (line 29) | `✅ Closed` | `✅ Closed` with the four new M6 test classes (`PhaseHBridgeReplacementTests`, `Phase5BridgeAnnotationsTests`, `CollectReplacedFqnsTests`, `M62IntegrationTests`) and a one-line summary covering M6.1 (Phase H + Phase 5 annotations) and M6.2 (pipeline skip + integration tests) | ✓ |
| `docs/ROADMAP.md` | M6 sub-phase block (lines 258–278) | M6.1 and M6.2 each `✅ Closed` with commit list and test classes; M7 hand-off note | M6.1 marked closed with `1af73ad, a408f44, b0f1ee5` and the two M6.1 test classes; M6.2 marked closed with `23f2933, 602a84e, adad506` and the two M6.2 test classes; M7 hand-off note at line 278 records §7.5 fifth scenario as hot-reload territory | ✓ |
| `docs/ROADMAP.md` | Engine snapshot (line 36) | `338/338` | `Total at M6 closure: 338/338 passed (verify with dotnet test against the current solution).` | ✓ |
| `docs/ROADMAP.md` | M7 row (line 30) | `⏭ Pending` | `M7 — Hot reload \| ⏭ Pending \| — \| Menu-driven, paused-only` | ✓ |
| `docs/ROADMAP.md` | v1.3 LOCKED references | header + see-also + migration prelude | line 3 `v1.3 LOCKED`; line 406 `v1.3 LOCKED specification driving M1–M10` | ✓ |
| `docs/README.md` | Architecture list entry (line 28) | `v1.3 LOCKED` | `**v1.3 LOCKED.** Mod system as a small operating system…` | ✓ |

The `v1.0 LOCKED` references at `ROADMAP.md:22` (M0 row Output column)
and `ROADMAP.md:109` (M0 phase Output prose) describe the artifact M0
**closed** — historically accurate, since v1.1, v1.2, and v1.3 are
non-semantic ratifications that did not reopen M0. Same handling as the
M3, M4, and M5 closure reviews: not navigation references.

The `v1.2` reference at `ROADMAP.md:25` (M3 row Notes column,
"hybrid per `MOD_OS_ARCHITECTURE` §3.6 v1.2") is similarly historical:
it attributes the §3.6 hybrid-enforcement formulation to the version of
the spec that ratified it (M3 closure). The row's status (`✅ Closed`)
is the live information; the v1.2 anchor is a closure timestamp.

The full `git diff 32f6f04..HEAD` against `docs/` shows changes to
`ROADMAP.md` only (the engine-snapshot update and the M6 sub-phase
section). `MOD_OS_ARCHITECTURE.md` and `README.md` are byte-identical
to the M5 closure point.

**Verdict:** PASSED. No version, date, status, or test-count drift among
the three primary documents.

---

## §4 Stale-reference sweep

Patterns checked across `docs/`:

| Pattern | Hits | Disposition |
|---|---|---|
| `v1.0 LOCKED` (active navigation) | 2 | Both at `ROADMAP.md:22` (M0 row Output column) and `ROADMAP.md:109` (M0 prose) — historical attribution of what M0 closed; not navigation references. Same handling as the M3/M4/M5 closure reviews. |
| `v1.1 LOCKED` (active navigation) | 0 | Clean. |
| `v1.2 LOCKED` (active navigation) | 0 in current docs; matches survive in `M3_CLOSURE_REVIEW.md`, `M4_CLOSURE_REVIEW.md`, and `M5_CLOSURE_REVIEW.md` as historical audit-trail (frozen by definition). |
| `311` (active "current state" test count) | many in `M5_CLOSURE_REVIEW.md`; 0 elsewhere as a current-state count | Clean. The `M5_CLOSURE_REVIEW.md` matches are historical (M5 closure was 311/311) and frozen. The `311` in this document's own three-commit invariant table (§1) cites the per-commit baseline, not the current state. |
| `328` / `333` (intermediate M6 test counts) | 0 outside historical context | Clean. The intermediate counts appear only in the present document's three-commit invariant table (§1) and the empirical commit-progression text — the canonical state pointer is `338`. |
| `M6 in progress` / `M6.1 current` / `M6.2 pending` / `M6.3 pending` | 0 | Clean. M6 is closed by `e643011`; no doc carries an in-progress marker for it or any of its sub-phases. |
| `🔨 Current` (active status) | 0 | Clean. The 🔨 glyph survives only in the `ROADMAP.md:101` section heading `## 🔨 Mod-OS Migration (M0–M10)` — a category label, not a status marker (preserved from M3/M4/M5 closures). |
| `M7 ... ⏭ Pending` (positive: required pattern) | 1 | Present at `ROADMAP.md:30` as expected — M7 is the next phase, marked Pending. |
| `M6 next` (positive: required absence — M6 is closed) | 0 in active state | Clean. The current header line 11 says "M7 next". |
| `Fixture.SharedEvents.dll` (post-v1.3 forbidden literal) | 0 | Clean (preserved from M4 / M5 closures). |
| `ignored for kind=shared` (post-v1.3 forbidden wording) | 0 | Clean (preserved from M4 / M5 closures). |

One tangential match for `338` outside the active engine-snapshot
context was verified as non-test-count usage:

- `NORMALIZATION_REPORT.md:338` references `BuildWriteViolationMessage`
  with `301` as a source-line column header — not a test count. The
  `338` here is a different cell column, also a source-line citation.

**Verdict:** PASSED. No active document carries a stale status, an
incorrect test count, or a stale version pointer. The handful of
historical-context occurrences are by design.

---

## §5 Methodology compliance

### §5.1 Commit scope prefixes (METHODOLOGY §7.3)

| # | Commit | Subject prefix | Scope | Body present | Verdict |
|---|---|---|---|---|---|
| 1 | `1af73ad` | `feat` | `systems` | yes (rationale + Phase 3 carry-over note + spec citation §7 LOCKED) | ✓ |
| 2 | `a408f44` | `feat` | `modding` | yes (eight-phase validator promotion + three error-kind enumeration + M6.2 hand-off) | ✓ |
| 3 | `b0f1ee5` | `test` | `modding` | yes (eight Phase H scenarios + nine annotation guards + layout note) | ✓ |
| 4 | `23f2933` | `feat` | `modding` | yes (skip path rationale + Phase H precondition citation §7.1 step 3 + internal seam note) | ✓ |
| 5 | `602a84e` | `test` | `modding` | yes (five boundary cases + delta declaration) | ✓ |
| 6 | `adad506` | `test` | `modding` | yes (five §7.5 scenarios + fixture layout + HealthComponent-overlap rationale + delta declaration) | ✓ |
| 7 | `e643011` | `docs` | `roadmap` | yes (M6.1 + M6.2 sync rationale + engine-snapshot bump + status overview update) | ✓ |

All 7 commits also carry substantive bodies — explanation, rationale,
and (where applicable) commit-cross-reference. None ship a one-line
subject without a body.

### §5.2 LOCKED decision sanctity — headline result

`MOD_OS_ARCHITECTURE.md` is **byte-identical** between `32f6f04` (M5
closure verification report commit) and `HEAD` (M6.3 ROADMAP-sync
commit). Verified directly:

```
$ git diff 32f6f04..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md | wc -l
0
```

Since the entire document is unchanged, §12 (D-1 through D-7
declarations) is necessarily byte-identical — and so is every §1–§11
section, every changelog entry through v1.3, every locked strategic
decision, and the entire §7 LOCKED specification that M6 implements.

This is the structural meaning of M6 as a "non-spec phase": M6 only
implements §7 acceptance criteria that have been locked since v1.0
(strategic lock 2 — explicit `replaces` — was ratified in v1.0; the
seven §11.2 error kinds for M6 were enumerated in v1.0; the
`Replaceable` flag mechanism was specified in v1.0 §7.4). M6 added no
contradiction that required a v1.4 ratification — **the v1.3 LOCKED
specification correctly drove implementation through the entire batch.**
This is the same headline result as the M5 closure review and the
strongest possible falsifiable signal for the empirical hypothesis
registered in §10.

### §5.3 No deliberate-interpretation registrations

Unlike M5 (which deliberately registered the cascade-failure
"accumulation, not skip" interpretation via ROADMAP §M5.3), M6 contains
no implementation-driven semantic refinement. Every §7 mechanism reads
directly out of the LOCKED specification:

- §7.1 step 3 ("when the kernel's bootstrap system list is being added
  to the dependency graph, every entry in `replacedSystems` is
  **skipped**") → `ModIntegrationPipeline.cs:300–324`.
- §7.2 ("two mods replacing the same FQN → batch rejected with
  `BridgeReplacementConflict`") → `ContractValidator.cs:583–622`,
  symmetric attribution per "the user is presented with the conflict
  in the mod menu and asked to disable one of the conflicting mods."
- §7.4 ("a bridge with `Replaceable = false` cannot be replaced") →
  `ContractValidator.cs:649–665`, with diagnostic distinguishing
  `Replaceable=false` from the no-attribute-at-all case (the latter is
  noted in §7.4 implicitly — non-bridge kernel systems are
  authoritative; the M6.1 implementation surfaces both as
  `ProtectedSystemReplacement` with explicit reason in the message).

No ROADMAP "deliberate interpretation" block was needed because no
ambiguity surfaced.

**Verdict:** PASSED. Commit-prefix discipline holds, the v1.3 LOCKED
specification is byte-identical through M6, the seven LOCKED decisions
are untouched, and M6 implementation reads literally off §7.

---

## §6 Sub-phase acceptance criteria coverage

`ROADMAP.md` M6 section (lines 258–278) declares M6.1 and M6.2 closed.
Each acceptance bullet maps to an identifiable artifact. M6.3 is itself
the closure mechanism (the ROADMAP-sync commit `e643011`), not a
verifiable sub-phase in the same sense as M6.1 / M6.2.

### M6.1 — Phase H validator + Replaceable bridge annotations

| Acceptance bullet | Artifact |
|---|---|
| `[BridgeImplementation]` extended with `Replaceable` bool (per §7.4) | M1 plumbing, preserved through M6: [BridgeImplementationAttribute.cs:35](../src/DualFrontier.Contracts/Attributes/BridgeImplementationAttribute.cs:35) `Replaceable { get; set; }` with v1.0 §7.4 XML-doc citing `ValidationErrorKind.ProtectedSystemReplacement` as the rejection mechanism. |
| Phase 5 combat stubs annotated `[BridgeImplementation(Phase = 5, Replaceable = true)]` | Commit `1af73ad`. Seven files in [src/DualFrontier.Systems/Combat/](../src/DualFrontier.Systems/Combat): `CombatSystem.cs:30`, `DamageSystem.cs:24`, `ProjectileSystem.cs:23`, `ShieldSystem.cs:22`, `StatusEffectSystem.cs:23`, `ComboResolutionSystem.cs:28`, `CompositeResolutionSystem.cs:30`. Each verified by a one-liner test in [Phase5BridgeAnnotationsTests.cs:20–60](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs:20). |
| `ContractValidator` Phase H emits `BridgeReplacementConflict`, `ProtectedSystemReplacement`, `UnknownSystemReplacement` | Commit `a408f44`. [ContractValidator.cs:566–667](../src/DualFrontier.Application/Modding/ContractValidator.cs:566) `ValidateBridgeReplacements` (private static), wired into `Validate` at [ContractValidator.cs:92](../src/DualFrontier.Application/Modding/ContractValidator.cs:92) (unconditional after Phase G). [ContractValidator.cs:676–685](../src/DualFrontier.Application/Modding/ContractValidator.cs:676) `ResolveTypeAcrossAssemblies` helper sweeps `AppDomain.CurrentDomain.GetAssemblies()`. The `anyReplacements` early-out at [ContractValidator.cs:570–581](../src/DualFrontier.Application/Modding/ContractValidator.cs:570) keeps Phase H pure overhead-free for typical batches. |
| Class XML-doc updated to "Eight-phase validator" | [ContractValidator.cs:12–48](../src/DualFrontier.Application/Modding/ContractValidator.cs:12) class summary opens with "Eight-phase validator" and enumerates Phases A, B, E, G, H (unconditional) plus C, D (capability-conditional) plus F (shared-mod-conditional). The phase split matches the conditional branches in `Validate` at [ContractValidator.cs:88–103](../src/DualFrontier.Application/Modding/ContractValidator.cs:88). |
| Phase 3 carry-over stubs explicitly verified `Replaceable = false` until M10.C | [SocialSystem.cs:22](../src/DualFrontier.Systems/Pawn/SocialSystem.cs:22) `[BridgeImplementation(Phase = 3)]` (unchanged through M6 — verified default `Replaceable = false`); [SkillSystem.cs:21](../src/DualFrontier.Systems/Pawn/SkillSystem.cs:21) same. Boundary locked by [Phase5BridgeAnnotationsTests.cs:62](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs:62) `SocialSystem_RemainsProtected` and [Phase5BridgeAnnotationsTests.cs:75](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs:75) `SkillSystem_RemainsProtected` — both assert `Phase = 3` AND `Replaceable.Should().BeFalse()`. |
| Eight Phase H scenarios + nine annotation guards | Commit `b0f1ee5`. [PhaseHBridgeReplacementTests.cs](../tests/DualFrontier.Modding.Tests/Validator/PhaseHBridgeReplacementTests.cs) (8 tests): accepts Replaceable target (line 40); rejects Replaceable=false with §7.4 + Phase value (line 56); rejects missing-attribute target with "attribute missing" wording (line 80); rejects unknown FQN with §7.2 (line 102); symmetric BridgeReplacementConflict (line 124); early-out when no mod declares replaces (line 154); independent verdicts for mixed valid/protected/unknown entries (line 170); disjoint FQNs across two mods produce no conflict (line 209). [Phase5BridgeAnnotationsTests.cs](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs) (9 tests): seven `…_HasReplaceableBridgeAttribute` (lines 20–60); two protected-guard tests for SocialSystem/SkillSystem (lines 62, 75). |

### M6.2 — Pipeline skip-on-replace graph build + integration tests

| Acceptance bullet | Artifact |
|---|---|
| `ModIntegrationPipeline.CollectReplacedFqns` helper | Commit `23f2933`. [ModIntegrationPipeline.cs:479–488](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:479) `CollectReplacedFqns` (private static, returns `HashSet<string>` keyed `StringComparer.Ordinal`). [ModIntegrationPipeline.cs:497–498](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:497) `CollectReplacedFqnsForTests` is the internal seam over the private static helper, available via `InternalsVisibleTo`. Verified by [CollectReplacedFqnsTests.cs](../tests/DualFrontier.Modding.Tests/Pipeline/CollectReplacedFqnsTests.cs) (5 tests). |
| Pipeline graph build skips `SystemOrigin.Core` whose FQN is in replaced set | Commit `23f2933`. [ModIntegrationPipeline.cs:300–324](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:300) — step `[5-7]` modification: `replacedFqns` is computed from `loaded`, then the `_registry.GetAllSystems()` loop skips entries where `reg.Origin == SystemOrigin.Core` AND the FQN appears in `replacedFqns`. The XML-doc cites §7.1 step 3 explicitly. |
| Mod-supplied replacement systems register normally per existing flow | No change required. Mods register replacement systems through `IModApi.RegisterSystem` during step `[4]` `IMod.Initialize`; ModRegistry tags them `SystemOrigin.Mod`; the loop at line 315 adds them to the local graph because the skip condition at line 317 only fires for `SystemOrigin.Core`. Verified by [M62IntegrationTests.cs:60–64](../tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs:60) — `Fixture.RegularMod_ReplacesCombat.ReplacementCombatSystem` is asserted present in the rebuilt scheduler alongside `CombatSystem` being absent. |
| All four §7.5 scenarios at pipeline level | Commit `adad506`. [M62IntegrationTests.cs](../tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs) (5 tests = 4 §7.5 scenarios + 1 regression guard): Replaceable success (line 35); Protected reject (line 67); Unknown reject (line 96); Conflict reject (line 120); regression guard for empty-replaces (line 159). |
| Four M6.2 fixture projects | Commit `adad506`. `tests/Fixture.RegularMod_ReplacesCombat`, `tests/Fixture.RegularMod_ReplacesCombat_Alt`, `tests/Fixture.RegularMod_ReplacesProtected`, `tests/Fixture.RegularMod_ReplacesUnknown`. Each follows the M4.3 layout (top-level `tests/Fixture.X/`, `AssemblyName` matches manifest `id`, MSBuild self-deploy target). The protected/unknown/alt fixtures throw on `IMod.Initialize` precisely so any order-of-operations regression (Phase H not running before pass [4]) surfaces as a loud test failure rather than silent pass. |

**Verdict:** PASSED. Every acceptance bullet for M6.1 and M6.2 maps to
an identifiable file:line and test. M6.3 is the closure-sync mechanism
itself (commit `e643011`) — verified by §3 cross-document consistency
checks above.

---

## §7 Carried debts forward

The M6 closure does not absorb earlier-phase debts; rather, it confirms
they remain tracked forward to the milestones that need them. M6
introduces **one** new forward-tracked carried hand-off (the §7.5
fifth scenario) and **two** non-blocking M9 architectural
considerations (registered in §10), all explicitly registered rather
than left latent.

| Debt / forward reference | Origin | Forward target | Documentation |
|---|---|---|---|
| WeakReference unload tests | Phase 2 (closed at 11/11 isolation tests, with the unload tests on backlog) | M7 (Hot reload from menu) — hard requirement | `ROADMAP.md:60` (Phase 2 carried-debt note); `ROADMAP.md:299` (M7 implementation: WeakReference unload tests are mandatory); `ROADMAP.md:307` (M7 acceptance: every regular mod under test passes the WeakReference unload check). |
| `SocialSystem`, `SkillSystem` `[BridgeImplementation(Phase = 3)]` stubs | Phase 3 (closed at 1/1 integration test, with social/skill stubs on backlog) | M10.C (`Vanilla.Pawn` mod) | `ROADMAP.md:68` (Phase 3 carried-debt note); `ROADMAP.md:327` (M10.C `Vanilla.Pawn` will absorb `SocialSystem`, `SkillSystem`); `ROADMAP.md:379` (M10.C consumes Phase 3 backlog with real implementations). M6 reinforces the boundary: [Phase5BridgeAnnotationsTests.cs:62](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs:62) and [Phase5BridgeAnnotationsTests.cs:75](../tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs:75) explicitly fail if `Replaceable` flips before M10.C. |
| M3.4 — CI Roslyn analyzer (D-2 hybrid completion) | M3 closure (deferred sub-phase) | First external (non-vanilla) mod author | `ROADMAP.md:26` (deferred row with rationale); `ROADMAP.md:177` (M3.4 sub-phase status); `MOD_OS_ARCHITECTURE.md:21` (v1.2 §11.1 changelog entry). M6 does not touch M3.4. |

### New in-batch carried item (M6 → forward)

#### §7.5 fifth scenario — mod-unloaded replacement-revert (M6 → M7 hand-off)

`MOD_OS_ARCHITECTURE.md` §7.5 line 5 reads: *"Mod is unloaded —
replacement skip is reverted, kernel bridge re-registers, dependency
graph rebuilds."* The first four §7.5 scenarios are exercisable inside a
single load batch; the fifth requires the unload chain (M7 territory).

The hand-off is registered at `ROADMAP.md:278`: *"On-unload
re-registration of skipped kernel bridges is handled by M7's hot-reload
path — at M6 closure, `Apply` rebuilds the graph from the surviving mod
set on every call, so the unload case reduces to 're-`Apply` without
the unloaded mod' once M7 lands."* The structural point: because
[ModIntegrationPipeline.cs:479–488](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:479)
`CollectReplacedFqns` is parameter-driven (it operates on its `loaded`
argument, not on persistent state), removing a mod from the active set
and re-`Apply`-ing naturally recomputes `replacedFqns` *without that
mod's contribution* — and the kernel bridge re-registers because its
FQN is no longer in the skip set. This is **acceptable carried hand-off,
not debt** — closes naturally when M7's per-mod unload path lands. No
spec amendment required.

**Verdict:** PASSED. Phase 2 and Phase 3 debts remain tracked forward
to their absorbing M-phases. M3.4 is unaltered. The §7.5 fifth scenario
is registered as the M6→M7 hand-off; the implementation is structurally
ready (see §8) so M7 absorbs it without M6 surface change.

---

## §8 Ready-for-M7 readiness

M7 is "Hot reload from menu" (`ROADMAP.md:282–311`). Per
`MOD_OS_ARCHITECTURE.md` §11.1, M7 consumes strategic lock 3
(menu-driven, paused-only) and D-7 (vanilla `hotReload` flag,
build-pipeline override). M7 needs:

| M7 dependency | M6/earlier surface | Status |
|---|---|---|
| `ModIntegrationPipeline.Pause()` / `Resume()` + `Apply` paused-only guard | Not present at M6 closure. This is M7's surface change, not blocked: a private bool flag on the pipeline plus a guard at the top of `Apply` is a localised addition. The `_scheduler` reference is already private, so the run-flag setter has a clean target. | ✓ ready (M7 surface change, expected) |
| ALC unload chain step 1 — `RestrictedModApi.UnsubscribeAll` | Present from M2 — `RestrictedModApi.UnsubscribeAll` removes every wrapper from the bus dispatcher. | ✓ ready |
| ALC unload chain step 2 — `IModContractStore.RevokeAll(modId)` | Present (M2 plumbing). Used today by [ModIntegrationPipeline.cs:286–288](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:286) (validation-failure rollback) and by [ModIntegrationPipeline.cs:330–332](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:330) (init-failure rollback). | ✓ ready |
| ALC unload chain step 3 — `ModRegistry.RemoveSystems(modId)` | [ModRegistry.cs:149](../src/DualFrontier.Application/Modding/ModRegistry.cs:149) `RemoveMod(modId)` is present (per-mod removal). [ModRegistry.cs:138](../src/DualFrontier.Application/Modding/ModRegistry.cs:138) `ResetModSystems` is the bulk variant used by `UnloadAll`. M7 will use the per-mod variant in the partial-unload path. | ✓ ready |
| ALC unload chain step 4 — Dependency graph rebuilt without the mod | Naturally handled: `Apply` constructs a fresh `DependencyGraph` from `_registry.GetAllSystems()` every call, so any `_activeMods` modification + re-`Apply` produces the correct rebuild. | ✓ ready |
| ALC unload chain step 5 — Scheduler swap | `_scheduler.Rebuild(localGraph.GetPhases())` is the existing atomic-swap call ([ModIntegrationPipeline.cs:351](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:351)). | ✓ ready |
| ALC unload chain step 6 — `ALC.Unload()` | [ModLoader.cs:214](../src/DualFrontier.Application/Modding/ModLoader.cs:214) `mod.Context.Unload()` is invoked by `UnloadMod`. Per-mod context unload primitive is in place. | ✓ ready |
| ALC unload chain step 7 — WeakReference spin loop | Not present at M6 closure. Phase 2 carried backlog as documented. M7 surface change. | ✓ ready (Phase 2 carried debt, expected) |
| Replacement-revert path (§7.5 fifth scenario) | `CollectReplacedFqns(loaded)` is parameter-driven — no persistent skip-set state to invalidate. M7's per-mod unload removes the mod from `_activeMods` (or equivalent) before re-`Apply`; `replacedFqns` recomputes naturally, the unloaded mod's `Replaces` entries vanish from the skip set, and the kernel bridge re-registers via the unmodified Core-systems loop. | ✓ ready |
| `ModLoader._loaded` / `ModLoader.UnloadMod` | [ModLoader.cs:21](../src/DualFrontier.Application/Modding/ModLoader.cs:21) `_loaded` dictionary present from M0; [ModLoader.cs:197](../src/DualFrontier.Application/Modding/ModLoader.cs:197) `UnloadMod(id)` per-mod unload present (calls `mod.Instance.Unload()`, then `mod.Context.Unload()`, then removes from the dict). | ✓ ready |
| `ContractValidator` does not need new phase for M7 | The 8-phase pipeline is run on the post-unload mod set as if it were a fresh `Apply` — Phase H sees a smaller `Replaces` aggregate, Phase G sees a smaller version-constraint graph, etc. No new phase is needed; M7 reuses the validator unchanged. | ✓ ready |
| `_activeMods` / `_activeShared` infrastructure | [ModIntegrationPipeline.cs:65–66](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:65) — both lists are populated on success path at line 352–353; M7's partial-unload path will need a `Remove` against `_activeMods`. | ✓ ready (existing field, M7 adds a removal call) |

The full M6 surface change list (one source file in `Application`,
seven systems in `Systems/Combat/` annotated, 32 test/fixture files in
`tests/`) is contained: nothing in `DualFrontier.Core`,
`DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Contracts`
(other than the M1 plumbing already in place), or `DualFrontier.Persistence`
is touched by M6. M7 inherits the same boundary-respecting M-phase
pattern.

**Verdict:** PASSED. No M6 surface change blocks M7. The pipeline's
parameter-driven skip-set design, the per-mod unload primitive in
ModLoader, the per-mod registry removal in ModRegistry, the contract
revocation primitive in ModContractStore, and the every-call
graph-rebuild discipline in `Apply` are the templates M7 will reuse.
The §7.5 fifth scenario reduces structurally to "re-`Apply` without
the unloaded mod" once M7's Pause/Resume + per-mod unload chain lands.

---

## §9 Surgical fixes applied

None.

---

## §10 Items requiring follow-up

No items requiring code or documentation change in this session. Three
non-blocking observations registered for visibility, none of which
gate M6 closure:

### Empirical observation — contradiction discovery rate across spec ratifications

The Mod-OS specification ratification chain shows a falsifiable
asymptotic decrease in latent-contradiction discovery rate per major
implementation pass:

| Implementation phase | Spec version at start | Spec version at end | Latent contradictions discovered |
|---|---|---|---|
| M3 closure review | v1.1 | v1.2 | **1** — §3.6 hybrid enforcement formulation contradicted §4.2/§4.3 implementation; ratified in v1.2 changelog |
| M4 closure review | v1.2 | v1.3 | **1** — §2.2 entryAssembly/entryType "ignored for kind=shared" contradicted §5.2 step 1; ratified in v1.3 changelog |
| M5 closure review | v1.3 | v1.3 (unchanged) | **0** — `git diff dba17c7..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md` returns zero output; M5 implementation surfaced no latent contradictions |
| M6 closure review | v1.3 | v1.3 (unchanged) | **0** — `git diff 32f6f04..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md` returns zero output; M6 implementation surfaced no latent contradictions |

The hypothesis is: **contradiction discovery rate decreases
asymptotically with each major implementation pass that exercises a
disjoint section of the specification**. M3 exercised §3 (capability
model). M4 exercised §1, §2, §5, §6.5 (mod kinds + shared ALC + D-4
contract types + D-5 cycles). M5 exercised §8 (versioning). M6 exercised
**§7 — an entirely new section relative to v1.1/v1.2/v1.3 ratifications,
since neither the v1.1 audit nor the v1.2 M3 review nor the v1.3 M4
review had any cause to inspect bridge replacement** — and discovered
zero latent contradictions, so the v1.3 LOCKED specification correctly
drove implementation across the entire batch.

This is the **fourth datapoint** in a four-element sequence (M3=1,
M4=1, M5=0, M6=0) and the **third consecutive zero**. The cleanest
empirical signal possible: M6 exercised a §7 section that was untouched
by every prior closure review, was specified at v1.0 and has not
required amendment since, and produced zero contradictions. The
asymptotic-decrease hypothesis is now strongly supported across two
independent disjoint section sweeps (§8 by M5, §7 by M6).

Falsifiable forward claim: M7 (which exercises §9 lifecycle + §10.4
WeakReference unload) discovers ≤ 1 latent contradiction; M8–M10
(vanilla mods exercising every consumer-facing manifest and capability
field) discover ≤ 1 cumulatively. The hypothesis fails if M7 alone
produces spec-amendment count ≥ 2 or if the cumulative count across
M3–M10 exceeds 4.

The observation belongs in a future `PIPELINE_METRICS.md` entry
(`docs/methodology/PIPELINE_METRICS.md` already exists per `docs/README.md` —
M-phase contradiction-discovery rate would slot into the empirical
metrics section). This review records the fourth datapoint as audit
trail; the metrics-section update is out of scope for a closure
verification report and would be a separate PR.

### M9 architectural consideration A — three Phase 5 stubs writing HealthComponent

`Apply_WithoutAnyReplaces_BridgesAllRegistered` (M62IntegrationTests
line 159) registers only **five** of the seven Phase 5 combat stubs as
Core systems (Combat, Projectile, Shield, ComboResolution,
CompositeResolution). The two excluded stubs — `DamageSystem` and
`StatusEffectSystem` — share a `HealthComponent` write declaration with
`CombatSystem`, which the regression-guard test scope (no replacements,
all stubs registered) would trip in Phase B (write-write conflict)
before exercising the actual skip-path regression invariant.

The commit body for `adad506` records this explicitly: *"DamageSystem
+ StatusEffectSystem are excluded because they share HealthComponent
writes with CombatSystem and would trip Phase B before the skip path is
even reachable. The future vanilla.combat mod is the intended
disambiguator; the regression guard is meaningful with the
non-conflicting subset."*

This is **NOT a §7 contradiction, NOT an M6 issue.** It is a future
M9 architectural consideration: when `Vanilla.Combat` ships
(`ROADMAP.md:342–365`), it will replace all conflicting Phase 5 stubs
through `replaces`, removing them from the kernel-only graph entirely.
Until M9 lands, the in-kernel-only configuration is not a production
target — kernel boots include the bridges as compiled stubs but never
register all seven into a single `DependencyGraph` together. M6
correctly does not "fix" this in M6.1/M6.2 because the resolution path
(vanilla.combat replacing the conflicting subset) is precisely what
M9 lands. Documented here for forward visibility.

### M9 architectural consideration B — Phase B ↔ Replaces ordering

`ContractValidator.Validate` runs Phase B (write-write conflict
detection) before Phase H (bridge replacement). For a kernel-only graph
where stubs conflict on `HealthComponent` (Consideration A above), this
ordering means a hypothetical "load this mod that replaces the
conflicting stub" scenario today does NOT short-circuit through the
write-write check — Phase B fires first and reports the kernel-vs-kernel
conflict, even when the user has supplied a replacement mod that would
make the conflict moot.

In practice, vanilla.combat will replace ALL three conflicting stubs
together (Combat, Damage, StatusEffect → vanilla.combat's replacement
systems), so the Phase B check on the post-replacement set sees no
remaining write-write conflicts. The current ordering is therefore
**correct for the realistic load batches M9 will ship.** A more
conservative future design might compute the post-replacement Core
system set first (effectively running Phase H's skip-set construction
before Phase B), so that hypothetical partial replacements do not
spuriously fail Phase B. M9's load-batch test polygon will reveal
whether this is needed.

This is **NOT a §7 contradiction, NOT an M6 issue.** It is a future M9
architectural consideration that may or may not surface depending on
the actual load patterns vanilla.combat exercises. Documented here for
forward visibility — M9 implementation review will revisit if the
ordering needs revision.

---

## Verification end-state

- **Build:** 0 warnings, 0 errors.
- **Tests:** 338/338 passing across all four test projects (Persistence
  4 / Systems 16 / Modding 258 / Core 60).
- **Three-commit invariant:** holds at every commit `1af73ad..e643011`.
  Per-commit progression: 311 (M5 baseline) → 311 → 311 → 328 (M6.1
  tests) → 328 → 333 (M6.2 helper tests) → 338 (M6.2 integration tests)
  → 338 (M6.3 docs).
- **Spec ↔ code ↔ test triple consistency:** 9/9 in-batch acceptance
  items (§7 LOCKED + §11.1 M6 row + Phase 3 boundary guard); CENTRAL
  bridge-replacement demonstration succeeds at validator and pipeline
  level; §7.5 fifth scenario registered as M6→M7 hand-off.
- **Cross-document consistency:** `MOD_OS_ARCHITECTURE` v1.3 LOCKED
  byte-identical through M6 ↔ `ROADMAP` `2026-05-01` `338/338` M6 ✅ ↔
  `docs/README` v1.3 LOCKED.
- **Stale-reference sweep:** zero hits on every active-navigation
  forbidden pattern.
- **Methodology compliance:** scope prefixes 7/7, all bodies
  substantive, LOCKED decisions D-1..D-7 byte-identical, **entire spec
  document byte-identical between M5 closure point and HEAD**.
- **Sub-phase acceptance:** M6.1 and M6.2 fully mapped; every bullet
  has identifiable file:line and test. M6.3 is the closure-sync
  mechanism itself.
- **Carried debts forward:** Phase 2 → M7, Phase 3 → M10.C, M3.4 →
  first external mod author. §7.5 fifth scenario registered as M6→M7
  hand-off; implementation structurally ready.
- **Ready-for-M7:** no surface blocker; pipeline parameter-driven
  skip-set, per-mod unload primitives in ModLoader, per-mod registry
  removal in ModRegistry, contract revocation primitive, and
  every-call graph-rebuild discipline all in place.
- **Surgical fixes applied this pass:** 0.
- **Items needing follow-up:** 0 blocking. Three observations
  registered in §10: one empirical contradiction-rate datapoint
  (fourth in sequence, third consecutive zero); two non-blocking M9
  architectural considerations (HealthComponent overlap among Phase 5
  stubs; Phase B ↔ Replaces ordering).

M6 closes cleanly. M7 (Hot reload from menu) is unblocked.

---

## See also

- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.3 LOCKED — the
  specification this review verifies. Byte-identical to its M5 closure
  state; M6 exercised §7 (entirely new section relative to all prior
  ratifications) without surfacing any latent contradiction.
- [ROADMAP](./ROADMAP.md) — M6 closure status, M6.1/M6.2 sub-phase
  detail, M7 pre-conditions and hand-off note.
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — §2.4 atomic phase review, §7.3
  process discipline; the verification cycle this report instantiates.
- [M3_CLOSURE_REVIEW](./M3_CLOSURE_REVIEW.md) — closure-report format
  origin (eight-check structure).
- [M4_CLOSURE_REVIEW](./M4_CLOSURE_REVIEW.md) — closure-report format
  model.
- [M5_CLOSURE_REVIEW](./M5_CLOSURE_REVIEW.md) — closure-report format
  model; this document mirrors its eight-check structure and extends
  the empirical contradiction-discovery rate observation registered
  in M5_CLOSURE_REVIEW §10 with its third consecutive zero datapoint.
