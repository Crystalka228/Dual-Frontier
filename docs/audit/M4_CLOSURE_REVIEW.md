---
title: M4 closure verification report
nav_order: 96
---

# M4 — Shared ALC + shared mod kind closure verification report

**Date:** 2026-04-29
**Branch:** `main` (commits `0a3a858..2a707f3`, eighteen commits inclusive)
**Scope:** Verification only. No new architectural work. Surgical fixes
applied only for typos, broken cross-references, or clearly-wrong facts in
the new documents. Any structural finding is recorded in §10 as a
follow-up item rather than remediated in this session.

---

## §0 Executive summary

| # | Check | Status | Notes |
|---|---|---|---|
| 1 | Build & test integrity | **PASSED** | `dotnet build`: 0 warnings, 0 errors. `dotnet test` at HEAD: 281/281 (Persistence 4, Systems 7, Modding 210, Core 60). Three-commit invariant verified at every commit in the M4 closure batch. Test progression: 260 → 267 (M4.1) → 273 (M4.2) → 281 (M4.3). |
| 2 | Spec ↔ code ↔ test triple consistency | **PASSED** | All five `MOD_OS_ARCHITECTURE` §11.1 acceptance bullets for M4 have all three legs (spec section, file:line, test name) present. The CENTRAL cross-ALC pub/sub roundtrip is `CrossAlcTypeIdentityTests.CrossAlcPubSub_DeliversEvent`. |
| 3 | Cross-document consistency | **PASSED** | `MOD_OS_ARCHITECTURE` v1.3 LOCKED (status line + version-history v1.3 entry + §2.2 reworded), `ROADMAP` header `Updated: 2026-04-29` with M4 ✅ Closed and `281/281`, `docs/README` v1.3 LOCKED — three documents in coherent state. |
| 4 | Stale-reference sweep | **PASSED** | All five forbidden patterns return zero hits in active-navigation context. `v1.0`, `v1.1`, `v1.2 LOCKED` survive only as legitimate historical-attribution (M0 row in ROADMAP, M3 closure review). `260` survives only inside `docs/M3_CLOSURE_REVIEW.md` (frozen audit trail). |
| 5 | Methodology compliance | **PASSED** | All 18 commits have scope prefixes per METHODOLOGY §7.3; every commit carries a substantive body. v1.3 changelog matches the v1.1/v1.2 four-rule pattern. The §12 LOCKED decisions D-1 through D-7 are byte-identical between v1.2 and v1.3 — verified by diff (zero changes inside §12 region). |
| 6 | Sub-phase acceptance criteria coverage | **PASSED** | Every acceptance bullet for M4.1, M4.2, M4.3 maps to an identifiable artifact (commit, file:line, test name). |
| 7 | Carried debts forward | **PASSED** | Phase 2 WeakReference unload tests still tracked in M7; Phase 3 `SocialSystem`/`SkillSystem` stubs still tracked in M10.C; M3.4 (CI Roslyn analyzer) remains `⏸ Deferred`. v1.3 §2.2/§5.2 ratification is registered in version history (not latent). M4 establishes the `AssemblyName` rename precedent (Case B from M4.3) for shared-mod fixtures. |
| 8 | Ready-for-M5 readiness | **PASSED** | `VersionConstraint` (M1) and `ModDependency.Version: VersionConstraint?` (M1) are in place. `TopoSortSharedMods` is `internal static` and reusable; M5 can introduce a regular-mod analogue without touching pass 1/2 structure. `ModIntegrationPipeline.Apply` has clear hook points between `[1]` shared-load and `[2]` regular-load for inserting regular-mod topological sort + version check. No M4 surface change blocks M5. |

**Result:** All 8 checks PASSED. Zero findings. Zero surgical fixes
applied. M4 phase closes cleanly; M5 (Inter-mod dependency resolution
with caret syntax) is unblocked.

---

## §1 Build & test integrity

**`dotnet build DualFrontier.sln`** at HEAD (`2a707f3`):

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**`dotnet test DualFrontier.sln`** at HEAD:

| Project | Pass | Skip | Total |
|---|---|---|---|
| `DualFrontier.Persistence.Tests` | 4 | 0 | 4 |
| `DualFrontier.Systems.Tests` | 7 | 0 | 7 |
| `DualFrontier.Modding.Tests` | 210 | 0 | 210 |
| `DualFrontier.Core.Tests` | 60 | 0 | 60 |
| **Total** | **281** | **0** | **281** |

### Three-commit invariant (METHODOLOGY §7.3)

Each of the 18 commits in the M4 closure batch was checked out
independently against a clean working tree; `dotnet build` ran
`0 Warning(s)` / `0 Error(s)` at every checkout, and `dotnet test`
exit code was zero at every checkout. Per-commit total test counts:

| # | Commit | Sub-phase | Subject (truncated) | Build | Tests |
|---|---|---|---|---|---|
| 1 | `0a3a858` | M4.1 | feat(modding): add SharedModLoadContext for cross-ALC type identity | 0 W / 0 E | **260** |
| 2 | `cf14edb` | M4.1 | feat(modding): add LoadedSharedMod record | 0 W / 0 E | **260** |
| 3 | `56772fc` | M4.1 | refactor(modding): split ModLoader into LoadRegularMod and LoadSharedMod | 0 W / 0 E | **260** |
| 4 | `e5e0e30` | M4.1 | feat(modding): wire ModLoadContext to delegate to shared ALC | 0 W / 0 E | **260** |
| 5 | `1ec1354` | M4.1 | feat(modding): two-pass mod loading in ModIntegrationPipeline | 0 W / 0 E | **260** |
| 6 | `cdb48f0` | M4.1 | test(modding): cross-ALC type identity and shared assembly resolution | 0 W / 0 E | **267** |
| 7 | `68cb693` | M4.2 | feat(modding): add Phase E contract-type scan to ContractValidator (D-4) | 0 W / 0 E | **267** |
| 8 | `14e1dd0` | M4.2 | test(modding): add Fixture.BadRegularMod for D-4 enforcement testing | 0 W / 0 E | **267** |
| 9 | `c410add` | M4.2 | test(modding): D-4 enforcement scenarios for regular-mod contract type rejection | 0 W / 0 E | **273** |
| 10 | `df582d3` | M4.3 | feat(modding): add shared-mod cycle detection to ModIntegrationPipeline (D-5) | 0 W / 0 E | **273** |
| 11 | `e0151d8` | M4.3 | feat(modding): add Phase F shared-mod compliance to ContractValidator | 0 W / 0 E | **273** |
| 12 | `d628692` | M4.3 | refactor(modding): remove M4.1 defensive IMod guard from ModLoader.LoadSharedMod | 0 W / 0 E | **273** |
| 13 | `b71e9e2` | M4.3 | test(modding): add Fixture.BadSharedMod_WithIMod for Phase F testing | 0 W / 0 E | **273** |
| 14 | `90f8012` | M4.3 | test(modding): D-5 cycle detection and Phase F shared-mod compliance scenarios | 0 W / 0 E | **281** |
| 15 | `1b35d51` | M4.3 | docs(roadmap): close M4 — sync with MOD_OS_ARCHITECTURE v1.2 §11.1 | 0 W / 0 E | **281** |
| 16 | `6e2e09c` | v1.3 | docs(arch): MOD_OS_ARCHITECTURE v1.3 — ratify §2.2 entryAssembly/entryType wording per §5.2 | 0 W / 0 E | **281** |
| 17 | `77f2aed` | v1.3 | test(modding): clean up Fixture.SharedEvents to comply with §5.2 | 0 W / 0 E | **281** |
| 18 | `2a707f3` | v1.3 | docs: update MOD_OS_ARCHITECTURE version reference (v1.2 → v1.3) | 0 W / 0 E | **281** |

The `260 → 267` jump at `cdb48f0` matches the +7 delta declared in that
commit's body (six tests in `CrossAlcTypeIdentityTests` /
`SharedAssemblyResolutionTests` plus one cross-class fact). The
`267 → 273` jump at `c410add` matches the +6 delta declared in that
commit's body (six Phase E enforcement scenarios in
`ContractTypeInRegularModTests`). The `273 → 281` jump at `90f8012`
matches the +8 delta declared in that commit's body (four D-5 cycle
scenarios + four Phase F shared-mod compliance scenarios in
`SharedModComplianceTests`).

The intermediate commits (feat / refactor without an associated test
commit) preserve the test count by not introducing dependent assertions
yet — the invariant is preserved by the absence of dependent tests, not
by accident.

**Verdict:** PASSED. The three-commit invariant from METHODOLOGY §7.3
holds across the entire batch. No commit ships a broken build or a
failing test.

---

## §2 Spec ↔ code ↔ test triple consistency

`MOD_OS_ARCHITECTURE` §11.1 declares M4 acceptance through five
acceptance bullets. Each is verified through three legs (spec section,
implementation file:line, dedicated test):

| # | Acceptance | Spec leg | Code leg | Test leg |
|---|---|---|---|---|
| 1 | A shared mod with `record FooEvent : IEvent` loads into the shared ALC | §1.2, §5.1, §5.2 | [SharedModLoadContext.cs:21](../src/DualFrontier.Application/Modding/SharedModLoadContext.cs:21), [LoadedSharedMod.cs:32](../src/DualFrontier.Application/Modding/LoadedSharedMod.cs:32), [ModLoader.cs:107](../src/DualFrontier.Application/Modding/ModLoader.cs:107) `LoadSharedMod` | [CrossAlcTypeIdentityTests.cs:23](../tests/DualFrontier.Modding.Tests/Sharing/CrossAlcTypeIdentityTests.cs:23) `SharedMod_DefiningEvent_LoadsIntoSharedAlc` |
| 2 | Cross-ALC type identity preserved (CENTRAL TEST) | §5, §5.3 | [ModLoadContext.cs:29](../src/DualFrontier.Application/Modding/ModLoadContext.cs:29) constructor accepts `SharedModLoadContext?`; [ModLoadContext.cs:45–54](../src/DualFrontier.Application/Modding/ModLoadContext.cs:45) `Load` delegates to `_sharedAlc.TryGetCachedAssembly` | [CrossAlcTypeIdentityTests.cs:43](../tests/DualFrontier.Modding.Tests/Sharing/CrossAlcTypeIdentityTests.cs:43) `RegularMod_DependingOnShared_ResolvesToSameTypeInstance` (Type-instance equality assertion); [CrossAlcTypeIdentityTests.cs:65](../tests/DualFrontier.Modding.Tests/Sharing/CrossAlcTypeIdentityTests.cs:65) `CrossAlcPubSub_DeliversEvent` (full pub/sub roundtrip across separate ALCs) |
| 3 | Regular mod with `IModContract` / `IEvent` types → `ContractTypeInRegularMod` | §6.5 D-4 LOCKED | [ContractValidator.cs:280–325](../src/DualFrontier.Application/Modding/ContractValidator.cs:280) `ValidateRegularModContractTypes`; emits via [ContractValidator.cs:327–340](../src/DualFrontier.Application/Modding/ContractValidator.cs:327) `BuildContractTypeError`; [ValidationError.cs:57–62](../src/DualFrontier.Application/Modding/ValidationError.cs:57) `ContractTypeInRegularMod` enum entry | [ContractTypeInRegularModTests.cs:23](../tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs:23) `RegularMod_WithIEventType_ProducesValidationError`; [ContractTypeInRegularModTests.cs:44](../tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs:44) `RegularMod_WithIModContractType_ProducesValidationError`; [ContractTypeInRegularModTests.cs:64](../tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs:64) `RegularMod_WithMultipleBadTypes_AccumulatesErrors`; [ContractTypeInRegularModTests.cs:118](../tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs:118) `Phase_E_RunsBeforeInitialize` (timing invariant) |
| 4 | Shared-mod cycle → `CyclicDependency` | §1.4, §12 D-5 LOCKED | [ModIntegrationPipeline.cs:131–144](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:131) cycle detection in `Apply` step `[0.5]`; [ModIntegrationPipeline.cs:377–466](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:377) `TopoSortSharedMods` (Kahn's algorithm) | [SharedModComplianceTests.cs:127](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:127) `SharedModCycle_TwoMods_ProducesCyclicDependencyErrors`; [SharedModComplianceTests.cs:161](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:161) `SharedModCycle_ThreeMods_ProducesCyclicDependencyErrors`; [SharedModComplianceTests.cs:200](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:200) `SharedMods_Acyclic_LoadsInTopologicalOrder` (positive baseline); [SharedModComplianceTests.cs:242](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:242) `SharedMod_WithRegularModDependency_NotConsideredForCycleDetection` (scope-of-D-5 negative) |
| 5 | Shared mod manifest with non-empty `entryAssembly` / `entryType` / `replaces` or `IMod` in assembly → `SharedModWithEntryPoint` | §5.2 step 1 (post-v1.3 §2.2 aligned); §11.2 enum list | [ContractValidator.cs:443–521](../src/DualFrontier.Application/Modding/ContractValidator.cs:443) `ValidateSharedModCompliance` (Phase F); [ValidationError.cs:49–55](../src/DualFrontier.Application/Modding/ValidationError.cs:49) `SharedModWithEntryPoint` enum entry | [SharedModComplianceTests.cs:19](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:19) `SharedMod_WithEntryAssemblyInManifest_ProducesSharedModWithEntryPointError`; [SharedModComplianceTests.cs:47](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:47) `SharedMod_WithEntryTypeInManifest_ProducesSharedModWithEntryPointError`; [SharedModComplianceTests.cs:74](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:74) `SharedMod_WithReplacesInManifest_ProducesSharedModWithEntryPointError`; [SharedModComplianceTests.cs:98](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:98) `SharedMod_WithIModImplementation_ProducesSharedModWithEntryPointError` |

### Regression guards

The M4.1 fixture set is reused as the positive baseline for M4.2 and the
v1.3 cleanup demonstrates §5.2 compliance at the fixture level itself:

- [ContractTypeInRegularModTests.cs:79–116](../tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs:79)
  re-uses `Fixture.SharedEvents` (M4.1) and `Fixture.PublisherMod` (M4.1)
  to assert that **legitimate** regular mods (publisher referencing a
  shared event) and **legitimate** shared mods (defining `IEvent` types)
  do not trip Phase E.
- [SharedModComplianceTests.cs:80–96](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:80)
  asserts that
  `Fixture.SharedEvents` (now compliant with §5.2 after `77f2aed`'s
  AssemblyName rename) does not trip Phase F when used as a positive
  shared-mod baseline.
- [ContractTypeInRegularModTests.cs:118–151](../tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs:118)
  `Phase_E_RunsBeforeInitialize` asserts that `BadMod.Initialize` (which
  throws on call) is never invoked when Phase E rejects the mod — the
  validation-precedes-initialization timing invariant.
- [SharedModComplianceTests.cs:98–124](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:98)
  `SharedMod_WithIModImplementation_ProducesSharedModWithEntryPointError`
  exercises `Fixture.BadSharedMod_WithIMod`, whose `WronglyHere.Initialize`
  similarly throws on call — Phase F surfacing the typed error before
  any IMod entry point runs is the architectural invariant.

**Verdict:** PASSED. All five §11.1 acceptance bullets have all three
legs present and verified. The CENTRAL cross-ALC pub/sub demonstration
(`CrossAlcPubSub_DeliversEvent`) succeeds: a publisher in one regular
ALC and a subscriber in another regular ALC, both referencing
`SharedTestEvent` from the shared ALC, exchange a synchronous event
through `RestrictedModApi.Publish`/`Subscribe`. Cross-ALC type identity
holds.

---

## §3 Cross-document consistency

Three documents must agree on the v1.3 / M4-closed / 281-tests state:

| Document | Field | Expected | Found | Status |
|---|---|---|---|---|
| `docs/MOD_OS_ARCHITECTURE.md` | Status line (line 8) | `LOCKED v1.3` | `LOCKED v1.3 — Phase 0 closed; non-semantic corrections from M1–M3.1 audit (v1.1), M3 closure review (v1.2), and M4.3 implementation review (v1.3) applied.` | ✓ |
| `docs/MOD_OS_ARCHITECTURE.md` | Version history v1.3 entry (lines 23–25) | v1.3 entry with §2.2 reference + final no-semantic statement | Two bullets present: §2.2 wording correction + "No semantic changes. No locked decision (D-1 through D-7) is altered. M4 implementations continue to comply." | ✓ |
| `docs/MOD_OS_ARCHITECTURE.md` | §2.2 entryAssembly row (line 214) | "must be empty for `kind=shared`" | `Required for kind=regular; **must be empty for kind=shared** (per §5.2 step 1).` | ✓ |
| `docs/MOD_OS_ARCHITECTURE.md` | §2.2 entryType row (line 215) | "must be empty for `kind=shared`" | `Required for kind=regular; **must be empty for kind=shared** (per §5.2 step 1).` | ✓ |
| `docs/ROADMAP.md` | Header date (line 11) | `2026-04-29` | `*Updated: 2026-04-29 (M4 closed — M4.1, M4.2, M4.3 done; M5 next).*` | ✓ |
| `docs/ROADMAP.md` | M4 row (line 27) | `✅ Closed` | `✅ Closed` with the four added M4 test classes (`CrossAlcTypeIdentityTests`, `SharedAssemblyResolutionTests`, `ContractTypeInRegularModTests`, `SharedModComplianceTests`) and a one-line summary covering M4.1/M4.2/M4.3 | ✓ |
| `docs/ROADMAP.md` | M4 sub-phase block (lines 207–209) | M4.1, M4.2, M4.3 each `✅ Closed` with commit list and test class | All three sub-phases marked closed with the exact commit SHAs from the batch and the right test class per sub-phase | ✓ |
| `docs/ROADMAP.md` | Engine snapshot (line 36) | `281/281` | `Total at M4 closure: 281/281 passed (verify with dotnet test against the current solution).` | ✓ |
| `docs/ROADMAP.md` | v1.3 LOCKED references | header + see-also + migration prelude | line 3 `v1.3 LOCKED`; line 103 `v1.3 §11`; line 412 `v1.3 LOCKED specification driving M1–M10` | ✓ |
| `docs/README.md` | Architecture list entry (line 28) | `v1.3 LOCKED` | `**v1.3 LOCKED.** Mod system as a small operating system…` | ✓ |

The `v1.0 LOCKED` references at `ROADMAP.md:22` (M0 row Output column)
and `ROADMAP.md:109` (M0 phase Output prose) describe the artifact M0
**closed** — historically accurate, since v1.1, v1.2, and v1.3 are
non-semantic ratifications that did not reopen M0. They do not function
as navigation references and therefore do not collide with the rule that
"navigation references must point to v1.3."

The `v1.2` reference at `ROADMAP.md:25` (M3 row Notes column,
"hybrid per `MOD_OS_ARCHITECTURE` §3.6 v1.2") is similarly historical:
it attributes the §3.6 hybrid-enforcement formulation to the version of
the spec that ratified it (M3 closure). The row's status (`✅ Closed`)
is the live information; the v1.2 anchor is a closure timestamp.

**Verdict:** PASSED. No version, date, status, or test-count drift among
the three primary documents.

---

## §4 Stale-reference sweep

Patterns checked across `docs/`:

| Pattern | Hits | Disposition |
|---|---|---|
| `v1.0 LOCKED` (active navigation) | 2 | Both at `ROADMAP.md:22` (M0 row Output column) and `ROADMAP.md:109` (M0 prose) — historical attribution of what M0 closed; not navigation references. Same handling as the M3 closure review. |
| `v1.1 LOCKED` (active navigation) | 0 | Clean. |
| `v1.2 LOCKED` (active navigation) | 0 in current docs; ten matches survive in `M3_CLOSURE_REVIEW.md` as historical audit-trail (frozen by definition — the M3 closure happened against v1.2). |
| `ignored for kind=shared` (post-v1.3 forbidden wording) | 0 | Clean. v1.3 ratification rewrites both rows in §2.2 to "must be empty for `kind=shared`". |
| `Fixture.SharedEvents.dll` (literal pre-v1.3 hardcoded name) | 0 | Clean. The fixture's `AssemblyName` is `tests.shared.events` after `77f2aed`; no other doc or code path references the literal `.dll` name. |
| `M4 in progress` / `M4.1 current` / `M4.2 current` / `M4.3 current` | 0 | Clean. M4 is closed by `1b35d51`; no doc carries an in-progress marker for it or any of its sub-phases. |
| `🔨 Current` (active status) | 0 | Clean. The 🔨 glyph survives only in the `ROADMAP.md:101` section heading `## 🔨 Mod-OS Migration (M0–M10)` — a category label, not a status marker (preserved from M3 closure). |
| `260` (active "current state" test count) | many in `M3_CLOSURE_REVIEW.md`; 0 elsewhere | Clean. The `M3_CLOSURE_REVIEW.md` matches are historical (M3 closure was 260/260). One hit at `NORMALIZATION_REPORT.md:326` is a source-code line-number citation, not a test count. |
| `267` / `273` (intermediate M4 test counts) | 0 | Clean. The intermediate counts appear only in the present document's three-commit invariant table (§1) and the empirical commit-progression text — the canonical state pointer is `281`. |

**Verdict:** PASSED. No active document carries a stale status, an
incorrect test count, or a stale version pointer. The handful of
historical-context occurrences are by design.

---

## §5 Methodology compliance

### §5.1 Commit scope prefixes (METHODOLOGY §7.3)

| # | Commit | Subject prefix | Scope | Verdict |
|---|---|---|---|---|
| 1 | `0a3a858` | `feat` | `modding` | ✓ |
| 2 | `cf14edb` | `feat` | `modding` | ✓ |
| 3 | `56772fc` | `refactor` | `modding` | ✓ |
| 4 | `e5e0e30` | `feat` | `modding` | ✓ |
| 5 | `1ec1354` | `feat` | `modding` | ✓ |
| 6 | `cdb48f0` | `test` | `modding` | ✓ |
| 7 | `68cb693` | `feat` | `modding` | ✓ |
| 8 | `14e1dd0` | `test` | `modding` | ✓ |
| 9 | `c410add` | `test` | `modding` | ✓ |
| 10 | `df582d3` | `feat` | `modding` | ✓ |
| 11 | `e0151d8` | `feat` | `modding` | ✓ |
| 12 | `d628692` | `refactor` | `modding` | ✓ |
| 13 | `b71e9e2` | `test` | `modding` | ✓ |
| 14 | `90f8012` | `test` | `modding` | ✓ |
| 15 | `1b35d51` | `docs` | `roadmap` | ✓ |
| 16 | `6e2e09c` | `docs` | `arch` | ✓ |
| 17 | `77f2aed` | `test` | `modding` | ✓ |
| 18 | `2a707f3` | `docs` | (parenthetical scope omitted; permitted by METHODOLOGY §7.3 which lists `docs:` as an acceptable form) | ✓ |

All 18 commits also carry substantive bodies — explanation, rationale,
and (where applicable) commit-cross-reference. None ship a one-line
subject without a body.

### §5.2 v1.3 ratification pattern parity

The v1.3 changelog at `MOD_OS_ARCHITECTURE.md:23–25` follows the same
shape as v1.1 (`MOD_OS_ARCHITECTURE.md:14–17`) and v1.2
(`MOD_OS_ARCHITECTURE.md:18–22`):

1. The changed section is identified by §-anchor (v1.1: §4.1, §2.2;
   v1.2: §3.6, §3.5+§2.1, §11.1; v1.3: §2.2).
2. The cause is described in implementation-vs-spec terms ("brings spec
   in line with implementation" / "non-semantic correction" /
   "contradicted §5.2 step 1").
3. An explicit "no semantic changes" clause appears.
4. An explicit "no locked decision (D-1 through D-7) is altered" clause
   appears, identical to v1.2's wording.

### §5.3 LOCKED decision sanctity

`git diff 7e44eb2..HEAD -- docs/MOD_OS_ARCHITECTURE.md` (the v1.2
ratification through the v1.3 ratification) confined to:

- Status line (v1.2 → v1.3 with extended explanation; "M4.3
  implementation review (v1.3)" appended).
- Version history (v1.2 entry stripped of "(this version)"; v1.3 entry
  added with 2 bullets).
- §2.2 manifest field reference table (entryAssembly and entryType
  Notes columns reworded — only the cell text on those two rows changes).

Comparing the bytes of §12 (D-1 through D-7 declarations) directly:

```
$ diff <(git show 7e44eb2:docs/MOD_OS_ARCHITECTURE.md | sed -n '871,$p') \
       <(git show 2a707f3:docs/MOD_OS_ARCHITECTURE.md | sed -n '874,$p')
$ echo $?
0
```

The §12 declaration text for D-1, D-2, D-3, D-4, D-5, D-6, D-7 is
**byte-identical** between v1.2 and v1.3. (Line numbers shift by three
because the v1.3 entry adds two bullets in the version history block;
the actual text inside the section is identical.)

**Verdict:** PASSED. Commit-prefix discipline holds, the v1.3 ratification
pattern matches v1.1 and v1.2, and all seven LOCKED decisions are
untouched.

---

## §6 Sub-phase acceptance criteria coverage

`ROADMAP.md` M4 section (lines 201–222) declares M4.1, M4.2, M4.3
closed. Each acceptance bullet maps to an identifiable artifact:

### M4.1 — `SharedModLoadContext` + two-pass loader + cross-ALC type identity

| Acceptance bullet | Artifact |
|---|---|
| `SharedModLoadContext` (singleton, non-collectible) | Commit `0a3a858`. [SharedModLoadContext.cs:21–34](../src/DualFrontier.Application/Modding/SharedModLoadContext.cs:21) `internal sealed class SharedModLoadContext : AssemblyLoadContext` with `base("shared", isCollectible: false)`. The `IsCollectible == false` invariant is asserted at [CrossAlcTypeIdentityTests.cs:34–35](../tests/DualFrontier.Modding.Tests/Sharing/CrossAlcTypeIdentityTests.cs:34). |
| `LoadedSharedMod` record | Commit `cf14edb`. [LoadedSharedMod.cs:32–37](../src/DualFrontier.Application/Modding/LoadedSharedMod.cs:32) `internal sealed record LoadedSharedMod(string ModId, ModManifest Manifest, AssemblyLoadContext Context, Assembly Assembly, IReadOnlyList<Type> ExportedTypes)`. |
| `ModLoader.LoadRegularMod` and `ModLoader.LoadSharedMod` separate methods | Commit `56772fc`. [ModLoader.cs:49–87](../src/DualFrontier.Application/Modding/ModLoader.cs:49) `LoadRegularMod`; [ModLoader.cs:107–161](../src/DualFrontier.Application/Modding/ModLoader.cs:107) `LoadSharedMod`. The original `LoadMod(path)` is preserved as [ModLoader.cs:32](../src/DualFrontier.Application/Modding/ModLoader.cs:32) backward-compat alias. |
| `ModLoadContext` delegates cross-mod resolves to shared ALC | Commit `e5e0e30`. [ModLoadContext.cs:29–33](../src/DualFrontier.Application/Modding/ModLoadContext.cs:29) constructor accepts `SharedModLoadContext? sharedAlc`; [ModLoadContext.cs:45–54](../src/DualFrontier.Application/Modding/ModLoadContext.cs:45) `Load` consults `_sharedAlc.TryGetCachedAssembly` first. Verified by [SharedAssemblyResolutionTests.cs:43–57](../tests/DualFrontier.Modding.Tests/Sharing/SharedAssemblyResolutionTests.cs:43) `ModLoadContext_with_shared_alc_resolves_cached_assembly` (asserts `BeSameAs`). |
| `ModIntegrationPipeline.Apply` two-pass | Commit `1ec1354`. [ModIntegrationPipeline.cs:88–284](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:88) `Apply` with explicit step labels `[0]` classify, `[0.5]` cycle detection, `[1]` shared load, `[2]` regular load, `[3]` validation, `[4]` `IMod.Initialize`, `[5-7]` graph build, `[8]` swap. Singleton shared ALC at [ModIntegrationPipeline.cs:53](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:53) `private readonly SharedModLoadContext _sharedAlc = new();`. |
| Cross-ALC type identity (CENTRAL) | All of the above plus the test fixture stack: [Fixture.SharedEvents/SharedTestEvent.cs:14](../tests/Fixture.SharedEvents/SharedTestEvent.cs:14) (record), [Fixture.PublisherMod/Publisher.cs:14–47](../tests/Fixture.PublisherMod/Publisher.cs:14) (publishes), [Fixture.SubscriberMod/Subscriber.cs:15–45](../tests/Fixture.SubscriberMod/Subscriber.cs:15) (subscribes). Demonstrated by [CrossAlcTypeIdentityTests.cs:65–101](../tests/DualFrontier.Modding.Tests/Sharing/CrossAlcTypeIdentityTests.cs:65) `CrossAlcPubSub_DeliversEvent`. |

### M4.2 — `ContractValidator` Phase E + D-4 enforcement

| Acceptance bullet | Artifact |
|---|---|
| `ContractValidator` runs five phases (post-Phase E) | Commit `68cb693`. [ContractValidator.cs:11–37](../src/DualFrontier.Application/Modding/ContractValidator.cs:11) class XML-doc lists Phases A, B, C, D, E (Phase F is added later in `e0151d8`). [ContractValidator.cs:79](../src/DualFrontier.Application/Modding/ContractValidator.cs:79) `ValidateRegularModContractTypes(mods, errors);` invocation. |
| Phase E unconditional, runs before `IMod.Initialize` | [ContractValidator.cs:30–32](../src/DualFrontier.Application/Modding/ContractValidator.cs:30) XML-doc explicitly: "Phases A, B and E run unconditionally". The pipeline ordering is enforced in [ModIntegrationPipeline.cs:195–211](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:195) (`_validator.Validate(...)` precedes the `IMod.Initialize` loop at [ModIntegrationPipeline.cs:215–230](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:215)). Asserted by [ContractTypeInRegularModTests.cs:118–151](../tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs:118) `Phase_E_RunsBeforeInitialize` — `BadMod.Initialize` throws on call; the test asserts the resulting error is `ContractTypeInRegularMod`, not `"threw during Initialize"`. |
| `ContractTypeInRegularMod` accumulation | [ContractValidator.cs:308–322](../src/DualFrontier.Application/Modding/ContractValidator.cs:308) iterates exported types and emits one error per offender (separate emit for `IEvent` and `IModContract` even on the same type). Asserted by [ContractTypeInRegularModTests.cs:64–77](../tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs:64) `RegularMod_WithMultipleBadTypes_AccumulatesErrors` (count == 2 for `BadEvent` + `BadContract`). |
| Test fixture | Commit `14e1dd0`. [tests/Fixture.BadRegularMod/BadMod.cs:14–42](../tests/Fixture.BadRegularMod/BadMod.cs:14) (BadMod + BadEvent + BadContract). [tests/Fixture.BadRegularMod/mod.manifest.json](../tests/Fixture.BadRegularMod/mod.manifest.json) declares `kind: regular`. Test scenarios commit `c410add`. |

### M4.3 — D-5 + Phase F + ROADMAP closure

| Acceptance bullet | Artifact |
|---|---|
| `ContractValidator` runs six phases (post-Phase F) | Commit `e0151d8`. [ContractValidator.cs:26–33](../src/DualFrontier.Application/Modding/ContractValidator.cs:26) XML-doc updated to enumerate Phases A, B, C, D, E, F. [ContractValidator.cs:87–90](../src/DualFrontier.Application/Modding/ContractValidator.cs:87) Phase F branch (`if (sharedMods is not null)`). |
| `ModLoader.LoadSharedMod` no longer throws on `IMod` presence | Commit `d628692`. [ModLoader.cs:117–128](../src/DualFrontier.Application/Modding/ModLoader.cs:117) keeps the `manifest.Kind != ModKind.Shared` guard (a programmer-error guard) but removes the `IMod` scan entirely. The XML-doc at [ModLoader.cs:89–100](../src/DualFrontier.Application/Modding/ModLoader.cs:89) explicitly redirects compliance to "ContractValidator Phase F". |
| Pipeline detects shared-mod cycles between pass 0 and pass 1 | Commit `df582d3`. [ModIntegrationPipeline.cs:131–144](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:131) cycle detection runs at step `[0.5]`, *between* manifest parse `[0]` and shared load `[1]`. The helper [ModIntegrationPipeline.cs:377–466](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:377) `TopoSortSharedMods` is `internal static` (reusable in M5). Cyclic mods never reach `LoadSharedMod` because their ids are excluded from `sortedShared`. |
| `SharedModWithEntryPoint` covers manifest fields AND `IMod` assembly scan | Commit `e0151d8`. [ContractValidator.cs:443–521](../src/DualFrontier.Application/Modding/ContractValidator.cs:443) `ValidateSharedModCompliance` checks: (a) `EntryAssembly` non-empty → error; (b) `EntryType` non-empty → error; (c) `Replaces.Count > 0` → error; (d) any exported type assignable to `IMod` → error. Each error names the offender (manifest value or type FQN). Verified by [SharedModComplianceTests.cs:19–124](../tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs:19) — four scenarios, one per case. |
| Test fixture for IMod-presence case | Commit `b71e9e2`. [tests/Fixture.BadSharedMod_WithIMod/WronglyHere.cs:16–39](../tests/Fixture.BadSharedMod_WithIMod/WronglyHere.cs:16) (`WronglyHere : IMod` + `SomeSharedType` co-resident). [tests/Fixture.BadSharedMod_WithIMod/mod.manifest.json](../tests/Fixture.BadSharedMod_WithIMod/mod.manifest.json) declares `kind: shared`. The `.csproj` sets `AssemblyName=tests.bad-shared-imod` so the loader's default `{id}.dll` lookup works without an `entryAssembly` field — establishing the §5.2-compliant pattern that `Fixture.SharedEvents` adopts in `77f2aed`. |
| ROADMAP M4 section closes with full sub-phase status | Commit `1b35d51`. [ROADMAP.md:201–222](./ROADMAP.md:201) — goal, sub-phase status, consumed decisions, acceptance criteria met, unblocks. |

**Verdict:** PASSED. Every acceptance bullet for M4.1, M4.2, M4.3 maps
to an identifiable file:line and test. No bullet is unbacked.

---

## §7 Carried debts forward

The M4 closure does not absorb earlier-phase debts; rather, it confirms
they are tracked forward to the milestones that need them. M4
additionally establishes one new in-batch precedent (the AssemblyName
rename for shared-mod fixtures), which is registered explicitly via the
v1.3 ratification.

| Debt | Origin | Forward target | Documentation |
|---|---|---|---|
| WeakReference unload tests | Phase 2 (closed at 11/11 isolation tests, with the unload tests on backlog) | M7 (Hot reload from menu) — hard requirement | `ROADMAP.md:60` ("Carried debt (now part of M7): AssemblyLoadContext WeakReference unload tests are not yet implemented…they become a hard requirement when M7 lands hot reload"); `ROADMAP.md:313` (M7 acceptance: "every regular mod under test passes the WeakReference unload check within 10 seconds"); `MOD_OS_ARCHITECTURE.md:816` (§10.4 "WeakReference unload tests… now hard-required"). |
| `SocialSystem`, `SkillSystem` `[BridgeImplementation(Phase = 3)]` stubs | Phase 3 (closed at 1/1 integration test, with social/skill stubs on backlog) | M10.C (`Vanilla.Pawn` mod) | `ROADMAP.md:68` ("Carried debt (now part of M10): SocialSystem and SkillSystem exist as `[BridgeImplementation(Phase = 3)]` stubs"); `ROADMAP.md:385` (M10.C acceptance: "Consumes Phase 3 backlog: SocialSystem and SkillSystem get real implementations inside the mod"). |
| M3.4 — CI Roslyn analyzer (D-2 hybrid completion) | M3 closure (deferred sub-phase) | First external (non-vanilla) mod author | `ROADMAP.md:26` (deferred row with rationale); `ROADMAP.md:177` (M3.4 sub-phase status); `MOD_OS_ARCHITECTURE.md:21` (v1.2 §11.1 changelog entry); `MOD_OS_ARCHITECTURE.md:834` (M3.4 row in migration table with deferred unblock condition). M4 does not touch M3.4. |
| §2.2 ↔ §5.2 wording contradiction | M4.3 implementation review (discovered while implementing Phase F) | v1.3 ratification (this batch) | `MOD_OS_ARCHITECTURE.md:23–25` (v1.3 version-history bullet) — registered, not latent. The contradiction is closed in-batch by `6e2e09c`. The implementation in `e0151d8` was correct against §5.2 from the start; v1.3 brings §2.2 in line. No follow-up work. |

### New in-batch precedent (M4.3 → v1.3): `AssemblyName` rename for §5.2-compliant fixtures

The v1.3 cleanup commit `77f2aed` renamed the `Fixture.SharedEvents`
project's `AssemblyName` from `Fixture.SharedEvents` (default) to
`tests.shared.events` (the manifest id). This pattern was first
introduced by `b71e9e2` for `Fixture.BadSharedMod_WithIMod` and is now
applied uniformly. Mechanism:

- Manifest `id` matches assembly base name (`tests.shared.events`).
- Manifest `entryAssembly` and `entryType` are empty (compliant with
  §5.2 step 1 / Phase F).
- `ModLoader.LoadSharedMod` falls through to its default `{id}.dll`
  lookup at [ModLoader.cs:134–137](../src/DualFrontier.Application/Modding/ModLoader.cs:134).
- The fixture deploy script in the `.csproj` `DeployToTestFixtures`
  target uses `$(AssemblyName)` for the destination directory, so the
  on-disk fixture path becomes `Fixtures/tests.shared.events/`.

Future shared-mod fixtures should follow this pattern. M5+ work
introducing additional shared mods will reuse it without further spec
amendments.

**Verdict:** PASSED. Phase 2 and Phase 3 debts remain tracked forward to
their absorbing M-phases. M3.4 is unaltered. The v1.3 §2.2/§5.2
ratification and the AssemblyName precedent are both registered in
documentation rather than left latent.

---

## §8 Ready-for-M5 readiness

M5 is "Inter-mod dependency resolution with caret syntax"
(`ROADMAP.md:226–254`). Per `MOD_OS_ARCHITECTURE` §11.1, M5 consumes
strategic lock 5 (caret syntax) — already locked and untouched by M4.

The M4 surface that M5 will exercise:

| M5 dependency | M4 surface | Status |
|---|---|---|
| `VersionConstraint` struct (`Exact`, `Caret`) | [VersionConstraint.cs:21–132](../src/DualFrontier.Contracts/Modding/VersionConstraint.cs:21) — full struct with `Parse` (handles `^X.Y.Z`, rejects tilde with directive to caret), `IsSatisfiedBy(ContractsVersion)`, `Floor`, `Kind`. Implemented in M1, untouched in M4. | ✓ ready |
| `ModDependency.Version: VersionConstraint?` | [ModDependency.cs:9](../src/DualFrontier.Contracts/Modding/ModDependency.cs:9) `record ModDependency(string ModId, VersionConstraint? Version, bool IsOptional)`; [ModDependency.cs:31–40](../src/DualFrontier.Contracts/Modding/ModDependency.cs:31) `IsSatisfiedBy(string availableModId, ContractsVersion? availableVersion)`. Implemented in M1, untouched in M4. | ✓ ready |
| Reusable topological sort | [ModIntegrationPipeline.cs:377–466](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:377) `TopoSortSharedMods` is `internal static` — Kahn's algorithm with `inDegree` and `dependents` dictionaries, returning `(sortedShared, cycleErrors)`. M5 can either extract the algorithm to a generic helper (parameterising on a kind filter) or introduce `TopoSortRegularMods` with the same shape. The `ModKind.Shared` filter at [ModIntegrationPipeline.cs:402](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:402) is the only kind-specific line. | ✓ ready |
| Hook point in `ModIntegrationPipeline.Apply` for regular-mod dep resolution | [ModIntegrationPipeline.cs:170–190](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:170) (step `[2]` regular load) is the natural insertion point: M5 can add a step `[1.5]` after pre-injected/parsed manifests are collected and before assembly load, running regular-mod topological sort + `apiVersion` check + `dependencies[i].version` check. The resulting failed-mod set feeds [ModIntegrationPipeline.cs:170–190](../src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:170) cascade logic without rewriting pass 1/2. | ✓ ready |
| `ValidationErrorKind.IncompatibleVersion` enum entry | [ValidationError.cs:35–40](../src/DualFrontier.Application/Modding/ValidationError.cs:35) — enum entry already present, declared in M1 in anticipation. M5 only needs to emit it. | ✓ ready |
| `ValidationErrorKind.MissingDependency` and `CyclicDependency` enum entries | [ValidationError.cs:25–32](../src/DualFrontier.Application/Modding/ValidationError.cs:25) — both present and exercised by M4 (M4.3 cycle detection emits `CyclicDependency`). | ✓ ready |
| No M4 coupling between shared-mod cycle code and regular-mod resolution | The cycle detector at `Apply [0.5]` operates on `sharedEntries` only; the regular-mod path at `Apply [2]` does no version checking and no toposort yet. M5 inserts orthogonally without disturbing M4 behaviour. | ✓ ready |

**Verdict:** PASSED. No M4 surface change blocks M5. The dependency
resolution layer (VersionConstraint + ModDependency.Version) is fully
specified from M1, and the M4-introduced topological-sort pattern is
the template M5 will reuse for the regular-mod graph.

---

## §9 Surgical fixes applied

None.

---

## §10 Items requiring follow-up

None.

All eight checks PASSED with no findings, no notes, and no surgical
fixes. The M4 closure batch is consistent with itself, with the v1.3
specification, with the methodology, and with the M5 entry surface.

---

## Verification end-state

- **Build:** 0 warnings, 0 errors.
- **Tests:** 281/281 passing across all four test projects (Persistence
  4 / Systems 7 / Modding 210 / Core 60).
- **Three-commit invariant:** holds at every commit `0a3a858..2a707f3`.
  Per-commit progression: 260 → 267 (M4.1) → 273 (M4.2) → 281 (M4.3) →
  281 (v1.3 ratification).
- **Spec ↔ code ↔ test triple consistency:** 5/5 §11.1 acceptance
  bullets; CENTRAL `CrossAlcPubSub_DeliversEvent` succeeds; cross-ALC
  type identity preserved.
- **Cross-document consistency:** `MOD_OS_ARCHITECTURE` v1.3 LOCKED ↔
  `ROADMAP` `2026-04-29` `281/281` M4 ✅ ↔ `docs/README` v1.3 LOCKED.
- **Stale-reference sweep:** zero hits on every active-navigation
  forbidden pattern.
- **Methodology compliance:** scope prefixes 18/18, ratification pattern
  v1.3 ≡ v1.2 ≡ v1.1, LOCKED decisions D-1..D-7 byte-identical between
  v1.2 and v1.3.
- **Sub-phase acceptance:** M4.1, M4.2, M4.3 fully mapped; every bullet
  has identifiable file:line and test.
- **Carried debts forward:** Phase 2 → M7, Phase 3 → M10.C, M3.4 → first
  external mod author. v1.3 §2.2 contradiction closed in-batch; the
  AssemblyName rename precedent is established.
- **Ready-for-M5:** no surface blocker; `VersionConstraint`,
  `ModDependency.Version`, reusable `TopoSortSharedMods`, clear hook
  points in `Apply` all in place.
- **Surgical fixes applied this pass:** 0.
- **Items needing follow-up:** 0.

M4 closes cleanly. M5 (Inter-mod dependency resolution with caret
syntax) is unblocked.

---

## See also

- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) v1.3 LOCKED — the
  specification this review verifies.
- [ROADMAP](./ROADMAP.md) — M4 closure status, M4.1/M4.2/M4.3 sub-phase
  detail, M5 pre-conditions.
- [METHODOLOGY](./METHODOLOGY.md) — §2.4 atomic phase review, §7.3
  process discipline; the verification cycle this report instantiates.
- [M3_CLOSURE_REVIEW](./M3_CLOSURE_REVIEW.md) — closure-report format
  model; this document mirrors its eight-check structure.
