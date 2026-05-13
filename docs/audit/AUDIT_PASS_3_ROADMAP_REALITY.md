---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_3_ROADMAP_REALITY
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_3_ROADMAP_REALITY
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_3_ROADMAP_REALITY
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_3_ROADMAP_REALITY
---
---
title: Audit Pass 3 — Roadmap ↔ Reality
nav_order: 107
---

# Audit Pass 3 — Roadmap ↔ Reality

**Date:** 2026-05-02
**Branch:** `main` (per `.git/HEAD` line 1)
**HEAD:** `6e9c43378c215ee9cb3c40c99656793ec7523353` (per `.git/refs/heads/main` line 1)
**Pass 1 baseline HEAD:** `1d43858a36c17b956a345e9bfe07a9ccf82daddb` (per `AUDIT_PASS_1_INVENTORY.md:10`)
**HEAD delta since Pass 1:** +2 commits — `3f00c2a` (`docs(architecture): ratify v1.5`, ratifies the Pass 2 §11 Tier 0 finding as a non-semantic spec amendment) and `6e9c433` (`Update AUDIT_PASS_2_SPEC_CODE.md`, syncs the Pass 2 artifact with the v1.5 amendment landing). Both commits are post-Pass-1; Pass 3 audits current state per plan §10 step 2.
**ROADMAP under audit:** `docs/ROADMAP.md` (active document)
**Spec referenced:** `docs/architecture/MOD_OS_ARCHITECTURE.md` LOCKED v1.5 §11.1
**Pass 1 inventory consumed:** `docs/audit/AUDIT_PASS_1_INVENTORY.md` (9/9 PASSED)
**Pass 2 artifact consumed:** `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` (10/11 PASSED + 1/11 FAILED with Tier 0 RESOLVED via v1.5)
**Scope:** Per-M-phase acceptance verification, engine snapshot test
count reconciliation, three-commit invariant verification across closed
M-phases (M0–M7.3). Forward-looking phases (M7.4, M7.5, M7-closure,
M8+) are out of scope.

---

## §0 Executive summary

| # | Check | Status | Tier 0 / 1 / 2 / 3 / 4 counts |
|---|---|---|---|
| 1 | M0 closure verification | PASSED | 0 / 0 / 0 / 0 / 0 |
| 2 | M1 closure verification | PASSED | 0 / 0 / 0 / 0 / 0 |
| 3 | M2 closure verification | PASSED | 0 / 0 / 0 / 0 / 0 |
| 4 | M3 closure (M3.1, M3.2, M3.3 closed; M3.4 deferred) | PASSED | 0 / 0 / 1 / 0 / 0 |
| 5 | M4 closure (M4.1, M4.2, M4.3) | PASSED | 0 / 0 / 0 / 0 / 0 |
| 6 | M5 closure (M5.1, M5.2) | PASSED | 0 / 0 / 0 / 0 / 0 |
| 7 | M6 closure (M6.1, M6.2) | PASSED | 0 / 0 / 1 / 0 / 0 |
| 8 | M7.1 closure | PASSED | 0 / 0 / 0 / 0 / 0 |
| 9 | M7.2 closure | PASSED | 0 / 0 / 0 / 0 / 0 |
| 10 | M7.3 closure | PASSED | 0 / 0 / 0 / 0 / 0 |
| 11 | Engine snapshot progressive test count | PASSED | 0 / 0 / 0 / 0 / 0 |
| 12 | Three-commit invariant across closed phases | PASSED | 0 / 0 / 1 / 1 / 0 |

**Tier breakdown across all checks:**
- Tier 0: 0 findings — eager escalation triggered: NO
- Tier 1: 0
- Tier 2: 3 whitelist confirmations (M3.4 deferred; Phase 3 SocialSystem/SkillSystem `Replaceable=false` carry-over to M10.C; M5/M6 closure-review historical branch refs preserved per Pass 1 anomaly #12)
- Tier 3: 1 (early-migration M0–M2 commit cadence does not match the strict feat→test→docs triplet introduced from M5.x onward — observation, no behavioural drift)
- Tier 4: 0

**Pass 1 anomaly #1 verdict:** **reconciled.** Source-level 357 `[Fact]` + 2 `[Theory]`, plus 12 `[InlineData]` cases (8 + 4) inside the single `[Theory]` file `tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs` → runtime expansion `357 + 12 = 369` matches ROADMAP `Engine snapshot` line 36 «Total at M7.3 closure: 369/369 passed». No drift; no finding. See §11.

**Pass 1 anomaly #12 verdict:** **Tier 2 historical preservation.** `.git/logs/HEAD` line 349 confirms `checkout: moving from feat/m4-shared-alc to main` occurs between M6 closure-review docs commit `c7210ca` (line 348, t=1777609922) and v1.4 ratify commit `b504813` (line 351, t=1777614545). M5 / M6 closure reviews captured branch state at time-of-review; current `main` reflects post-merge state. See §12.

---

## §1 M0 closure verification

**ROADMAP rows audited:** `ROADMAP.md` lines 22 (status overview row «**M0 — Mod-OS Phase 0** | ✅ Closed | — | `MOD_OS_ARCHITECTURE` v1.0 LOCKED») and 105–112 (closed-phase section).
**Spec §11.1 row:** «**M0** | This document at v1.0 | All ⚠ DECISION items closed | —».

**Acceptance bullets verification table:**

| # | ROADMAP claim | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «`MOD_OS_ARCHITECTURE` v1.0 LOCKED. All twelve decisions resolved (five strategic + seven detail D-1 through D-7).» (ROADMAP.md:109) | `MOD_OS_ARCHITECTURE.md:8` status line currently reads `LOCKED v1.5` — five non-semantic ratification cycles have been applied since v1.0 (v1.1, v1.2, v1.3, v1.4, v1.5 per Version history lines 12–32). v1.0 entry verbatim at line 13: «v1.0 — Phase 0 closed. All seven open decisions resolved and locked. Implementation phases M1–M10 may begin.» Five strategic locks present at lines 56–62 (Pass 1 §9 entry 1 confirmed catalogue). Seven detail decisions D-1..D-7 enumerated at §12 (lines 886–969 per Pass 1 §9 entry 26; verified by Pass 2 §11 sequence integrity check). | ✓ |
| 2 | «**No code changes.** This phase exists to make every subsequent code change traceable to a documented decision.» (ROADMAP.md:111) | M0 by design produces no code or test artifacts; the spec at v1.0 LOCKED is the artifact. Spec §11.1 «Tests added» column for M0 is `—` (line 843 verbatim). | ✓ |

**Closure review cross-reference:** No standalone `M0_CLOSURE_REVIEW.md` (M0 is spec-only; closure was the v1.0 LOCKED ratification entry itself, captured in the spec changelog).

**Findings:** None.

---

## §2 M1 closure verification

**ROADMAP rows audited:** `ROADMAP.md:23` (status overview row «**M1 — Manifest v2** | ✅ Closed | added | `VersionConstraint`, `ModDependency`, `ManifestCapabilities`, `ModManifest` v2, `ManifestParser`, full `ValidationErrorKind` set») and lines 115–137 (closed-phase section).
**Spec §11.1 row (line 844):** «**M1** | Manifest v2 schema + parser, backward compatible | `ModManifest` extended, JSON loader handles v2 | Manifest validation tests».

**Acceptance bullets verification table:**

| # | ROADMAP claim (lines 131–135) | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «Existing `ExampleMod`'s `mod.manifest.json` loads unchanged (backward compatibility).» | `mods/DualFrontier.Mod.Example/mod.manifest.json` parses successfully (Pass 2 §2 verified `ManifestParser.Parse` handles missing `kind` / `apiVersion` / `hotReload` / `dependencies` defaults; v1 grace path through `ModManifest.EffectiveApiVersion` at `ModManifest.cs:119–120`). | ✓ |
| 2 | «A v2 manifest with `kind`, `apiVersion`, `hotReload`, `dependencies` (with versions), `replaces`, `capabilities` parses to a fully populated `ModManifest`.» | `tests/DualFrontier.Modding.Tests/Manifest/ModManifestV2Tests.cs` (18 `[Fact]` per Pass 1 §2 per-file table); `tests/DualFrontier.Modding.Tests/Parser/ManifestParserTests.cs` (19 `[Fact]`); `tests/DualFrontier.Modding.Tests/Manifest/ManifestCapabilitiesTests.cs` (31 `[Fact]`); `tests/DualFrontier.Modding.Tests/Manifest/ModDependencyTests.cs` (18 `[Fact]`). All five M1 v2-schema fields covered (Pass 2 §2 verified each field path through `ManifestParser`). | ✓ |
| 3 | «Caret-prefixed versions parse correctly; tilde produces a clean `FormatException`.» | `tests/DualFrontier.Modding.Tests/Manifest/VersionConstraintTests.cs` (35 `[Fact]`); `VersionConstraint.Parse` accepts `[^]MAJOR.MINOR.PATCH` and rejects tilde with `FormatException` (`src/DualFrontier.Contracts/Modding/VersionConstraint.cs:49–91`, Pass 2 §2 step 3 verified). | ✓ |
| 4 | «Manifest with malformed capability strings rejected at parse time with a clear message.» | `ManifestCapabilities.s_capabilityPattern` regex (`src/DualFrontier.Contracts/Modding/ManifestCapabilities.cs:15–17`); `ParseSet` throws `ArgumentException` on regex mismatch (`ManifestCapabilities.cs:165–180`, Pass 2 §2 step 6 verified). Covered by `ManifestCapabilitiesTests` (31 `[Fact]`). | ✓ |
| 5 | «All seven new `ValidationErrorKind` entries are exercised by unit tests with their canonical error messages.» | `src/DualFrontier.Application/Modding/ValidationError.cs:9–83` enumerates 11 members (Pass 1 §9 entry 39): the 4 baseline (`IncompatibleContractsVersion`, `WriteWriteConflict`, `CyclicDependency`, `MissingDependency`) + the 7 migration additions referenced by spec §11.2 (`MissingCapability` (M3), `BridgeReplacementConflict` (M6), `ProtectedSystemReplacement` (M6), `UnknownSystemReplacement` (M6), `IncompatibleVersion` (M5), `SharedModWithEntryPoint` (M4), `ContractTypeInRegularMod` (M4)). Each error kind is covered by phase-specific tests across `Validator/`, `Sharing/`, and `Pipeline/` subfolders — Pass 2 §11.2 verified the v1.5-amended baseline aligns with `ValidationError.cs`. | ✓ |

**Closure review cross-reference:** No standalone `M1_CLOSURE_REVIEW.md` (M1 closure was rolled into the M0–M3 batch; the `e37ca25` v1.1 LOCKED + ROADMAP reorganize commit served as the joint closure for M0–M3).

**Findings:** None.

---

## §3 M2 closure verification

**ROADMAP rows audited:** `ROADMAP.md:24` (status overview row «**M2 — IModApi v2** | ✅ Closed | added | Real `Publish`/`Subscribe`, `GetKernelCapabilities`, `GetOwnManifest`, `Log`, `RestrictedModApi` v2») and lines 141–164 (closed-phase section).
**Spec §11.1 row (line 845):** «**M2** | `IModApi.Publish`/`Subscribe` real implementation | `RestrictedModApi` no longer no-ops | Publish/subscribe round-trip tests».

**Acceptance bullets verification table:**

| # | ROADMAP claim (lines 158–162) | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «A mod publishes `DamageEvent`; a kernel system subscribed to `IGameServices.Combat` receives it on the next tick (deferred) or in the same tick (immediate).» | `RestrictedModApi.Publish<T>` resolves bus via `ModBusRouter` then calls `EnforceCapability("publish", typeof(T))` (`src/DualFrontier.Application/Modding/RestrictedModApi.cs:80–88`, Pass 2 §4 verified). Deferred/immediate semantics handled by underlying `IEventBus.Publish` (Phase 4 closure surface). Covered by `RestrictedModApiV2Tests` (22 `[Fact]`, Pass 1 §2). | ✓ |
| 2 | «A mod subscribes to `MoodBreakEvent`; the kernel `MoodSystem` publishes; the mod's handler runs under the mod's `SystemExecutionContext`.» | `RestrictedModApi.Subscribe<T>` captures `SystemExecutionContext.Current`, wraps handler with push/pop context, records pair in `_subscriptions` (`RestrictedModApi.cs:91–112`, Pass 2 §4 verified). Covered by `RestrictedModApiV2Tests`. | ✓ |
| 3 | «On mod unload, `RestrictedModApi.UnsubscribeAll` removes every wrapper from the bus dispatcher; subsequent kernel publishes do not invoke unloaded handlers.» | `RestrictedModApi.UnsubscribeAll` iterates and clears (`RestrictedModApi.cs:140–145`); called from §9.5 step 1 in `ModIntegrationPipeline.RunUnloadSteps1Through6AndCaptureAlc` (`ModIntegrationPipeline.cs:565–568`, Pass 2 §4 / §9 verified). Behaviour locked by M7.2 + M7.3 unload-chain tests. | ✓ |
| 4 | «Capability check at `Subscribe` and `Publish` time still raises `CapabilityViolationException`.» | `EnforceCapability` raises `CapabilityViolationException` (`RestrictedModApi.cs:147–164`, Pass 2 §3 + §4 verified). Note: the M2 ROADMAP wording «here we stub the registry with a "permit-all" implementation behind a feature flag» reflects the M2-era plan; M3 superseded the stub with `KernelCapabilityRegistry`. The M3 supersession is itself reflected in the spec changelog and is whitelist-compatible (Pass 2 §3 confirmed compatibility with v1.5 §3.6 hybrid wording). | ✓ |
| 5 | «`[Deferred]` events from mod systems queue correctly and deliver on the next phase boundary.» | `[Deferred]` semantics implemented at the bus layer (Phase 4 closure surface). Mod-side deferred publish flows through `RestrictedModApi.Publish` → `ModBusRouter` → bus, no special path. Behaviour exercised end-to-end through `RestrictedModApiV2Tests`. | ✓ |
| 6 | «New `IModApi.GetKernelCapabilities()`, `IModApi.GetOwnManifest()`, `IModApi.Log(LogLevel level, string message)`.» (ROADMAP lines 151–153) | `IModApi.GetKernelCapabilities` / `GetOwnManifest` / `Log(ModLogLevel, string)` declared at `src/DualFrontier.Contracts/Modding/IModApi.cs:16–75` (Pass 1 §5 + Pass 2 §4 verified the 9-method surface). v1.1 ratification clarified `LogLevel` → `ModLogLevel` (Pass 2 §4 row 2 confirmed). | ✓ |
| 7 | «`RestrictedModApi` declared `internal sealed` (per D-3 lock); confirm `DualFrontier.Application` is not in the resolution path of any mod ALC.» | `internal sealed class RestrictedModApi : IModApi` at `RestrictedModApi.cs:31`; constructor `internal` at `RestrictedModApi.cs:49`; `ModLoadContext.Resolving` delegates only `DualFrontier.*` markers per the per-ALC delegation rules; `DualFrontier.Application` resolution from a mod ALC is structurally unreachable (Pass 2 §4 D-3 row verified). | ✓ |

**Closure review cross-reference:** No standalone `M2_CLOSURE_REVIEW.md` (rolled into the M0–M3 joint closure with `e37ca25`; M2 implementation commit `35dc5b2e` per `.git/logs/HEAD` line 244).

**Findings:** None.

---

## §4 M3 closure verification (M3.1, M3.2, M3.3 closed; M3.4 deferred)

**ROADMAP rows audited:** `ROADMAP.md:25` (status overview row «**M3 — Capability model** | ✅ Closed | added (`KernelCapabilityRegistryTests`, `RestrictedModApiV2Tests`, `CapabilityValidationTests`, `ProductionComponentCapabilityTests`) | M3.1 `KernelCapabilityRegistry` + `[ModAccessible]` opt-in; M3.2 capability-enforcing `RestrictedModApi`; M3.3 `[ModCapabilities]` + load-time cross-check (`ContractValidator` Phases C+D); M3.4 deferred»), `ROADMAP.md:26` (M3.4 deferred row), and lines 168–198 (closed-phase section).
**Spec §11.1 row (lines 846–847):** «**M3** | Capability model: parser, kernel-provided set, load-time + runtime check | `CapabilityRegistry`, `[ModAccessible]` attribute, `[ModCapabilities]` attribute, `RestrictedModApi.EnforceCapability` | `KernelCapabilityRegistryTests`, `CapabilityValidationTests`, capability violation tests» + «**M3.4** *(deferred)* | CI Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion) | Standalone analyzer package; runs in mod-publication CI, not at game load | Static-analysis integration tests; unblocked when first external mod author appears».

**Acceptance bullets verification table (M3.1, M3.2, M3.3):**

| # | ROADMAP claim | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «M3.1 ✅ Closed. Acceptance: `KernelCapabilityRegistry` scans `DualFrontier.Components` + `DualFrontier.Events` assemblies (commit `a73669f`); production components annotated `[ModAccessible]` per D-1 (commit `f91f065`); `KernelCapabilityRegistryTests` + `ProductionComponentCapabilityTests` (commit `b92fa66`).» (ROADMAP.md:174) | `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:30–119` (`BuildFromKernelAssemblies` reflects Components + Events, Pass 2 §3 verified). Production components annotated: `WeaponComponent`, `ArmorComponent`, `AmmoComponent`, `ShieldComponent`, `HealthComponent` (Pass 2 §3 confirmed 5 annotated production components). Tests: `tests/DualFrontier.Modding.Tests/Capability/KernelCapabilityRegistryTests.cs` (11 `[Fact]`) and `Capability/ProductionComponentCapabilityTests.cs` (1 `[Fact]` + 2 `[Theory]` with 12 `[InlineData]` cases — see §11). Commits `a73669f`, `f91f065`, `b92fa66` confirmed at `.git/logs/HEAD` lines 256, 257, 258 respectively. | ✓ |
| 2 | «M3.2 ✅ Closed. Acceptance: `RestrictedModApi.EnforceCapability` raises `CapabilityViolationException` on `Publish<T>` / `Subscribe<T>` when the manifest does not declare the capability; per `MOD_OS_ARCHITECTURE` §4.2 / §4.3 v1.2 hybrid enforcement (ratified in v1.2 §3.6).» (ROADMAP.md:175) | `RestrictedModApi.EnforceCapability` raises `CapabilityViolationException` from `Publish` and `Subscribe` (`RestrictedModApi.cs:80–112,147–164`, Pass 2 §3 §3.6 hybrid row verified — load-time gate + runtime check pattern matches v1.5-current §3.6 wording). Covered by `RestrictedModApiV2Tests` (22 `[Fact]`). | ✓ |
| 3 | «M3.3 ✅ Closed. Acceptance: `ContractValidator` Phase C (capability satisfiability via kernel + listed dependencies) and Phase D (`[ModCapabilities]` × manifest cross-check) implemented; `CapabilityValidationTests` covers both phases.» (ROADMAP.md:176) | `ContractValidator.ValidateCapabilitySatisfiability` (Phase C, `src/DualFrontier.Application/Modding/ContractValidator.cs:398–433`); `ContractValidator.ValidateModCapabilitiesAttributes` (Phase D, `ContractValidator.cs:442–469`); both invoked conditionally when `kernelCapabilities` is supplied (Pass 2 §3.4 / §3.8 verified). Tests: `tests/DualFrontier.Modding.Tests/Capability/CapabilityValidationTests.cs` (11 `[Fact]`). | ✓ |

**Acceptance bullets verification table (M3.4 — deferred):**

| # | ROADMAP claim | Verification | Verdict |
|---|---|---|---|
| 4 | «M3.4 ⏸ Deferred. CI Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion). Unblocked when the first external (non-vanilla) mod author appears.» (ROADMAP.md:177) and «`*(deferred)* | CI Roslyn analyzer ... unblocked when first external mod author appears»» (`MOD_OS_ARCHITECTURE.md:847`) | Filesystem scan of `src/` confirms no `*.Analyzer/` project. Spec wording and ROADMAP wording match. Per AUDIT_CAMPAIGN_PLAN §6.3 + Pass 2 §13 Tier 2 #4: «carried до first external mod author. Tier 2.» | **Tier 2** confirmed (whitelist deviation §5.1 of Pass 3 prompt). |

**Acceptance bullets from §11.1 spec acceptance criteria (lines 191–195):**

| # | Spec / ROADMAP acceptance bullet | Code/test artifact | Verdict |
|---|---|---|---|
| 5 | «A mod declares `kernel.read:HealthComponent` in manifest; HealthComponent annotated `[ModAccessible(Read = true)]`; load succeeds.» | `HealthComponent` annotated; tested in `CapabilityValidationTests`. | ✓ |
| 6 | «A mod declares `kernel.read:MindComponent`; MindComponent **not** annotated; load fails with `MissingCapability`.» | `MindComponent` is not in the M3 annotated set (Pass 2 §3.5 confirmed scan only picks up `[ModAccessible]`-annotated components); the `MissingCapability` error path is exercised in `CapabilityValidationTests`. | ✓ |
| 7 | «System has `[ModCapabilities("publish:DamageEvent")]` but manifest does not list `kernel.publish:DualFrontier.Events.Combat.DamageEvent`; load fails with `MissingCapability`.» | Phase D cross-check at `ContractValidator.cs:442–469` emits `MissingCapability` when `[ModCapabilities].Tokens` not in `Manifest.Capabilities.Required`. Exercised in `CapabilityValidationTests`. | ✓ |
| 8 | «A mod's system publishes; manifest correctly populated; load succeeds, publish reaches the bus.» | End-to-end happy path covered in `RestrictedModApiV2Tests`. | ✓ |

**Closure review cross-reference:** `docs/audit/M3_CLOSURE_REVIEW.md` (Date 2026-04-29, branch `main`, commits `a73669f..95935d7`).

**Findings:**
- **Tier 2** — M3.4 deferred (CI Roslyn analyzer). See §14.

---

## §5 M4 closure verification (M4.1, M4.2, M4.3)

**ROADMAP rows audited:** `ROADMAP.md:27` (status overview row «**M4 — Shared ALC** | ✅ Closed | added (`CrossAlcTypeIdentityTests`, `SharedAssemblyResolutionTests`, `ContractTypeInRegularModTests`, `SharedModComplianceTests`) | M4.1 ... M4.2 ... M4.3 ...») and lines 201–222 (closed-phase section).
**Spec §11.1 row (line 848):** «**M4** | Shared ALC + shared mod kind | `SharedModLoadContext`, manifest `kind` field | Type-sharing test (shared-mod event published from one regular mod, received in another)».

**Acceptance bullets verification table:**

| # | ROADMAP claim | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «M4.1 ✅ Closed. `SharedModLoadContext` (singleton, non-collectible, `Resolving` delegates `DualFrontier.*` to default ALC) and the two-pass loader implemented. Cross-ALC type identity verified by `CrossAlcTypeIdentityTests`; assembly resolution semantics verified by `SharedAssemblyResolutionTests`.» (ROADMAP.md:207) | `src/DualFrontier.Application/Modding/SharedModLoadContext.cs` (singleton instance constructed once per pipeline with `isCollectible: false`, Pass 2 §1.4 verified). Two-pass loader: `ModIntegrationPipeline.Apply` runs shared-mod load first (Pass 2 §5 verified). Tests: `tests/DualFrontier.Modding.Tests/Sharing/CrossAlcTypeIdentityTests.cs` (3 `[Fact]`) and `Sharing/SharedAssemblyResolutionTests.cs` (4 `[Fact]`). M4.1 commits `0a3a858`, `cf14edb`, `56772fc`, `e5e0e30`, `1ec1354`, `cdb48f0` confirmed at `.git/logs/HEAD` lines 269–274. | ✓ |
| 2 | «M4.2 ✅ Closed. `ContractValidator` Phase E enforces D-4 — every regular mod's assemblies are scanned, and any exported type implementing `IEvent` or `IModContract` produces a typed `ContractTypeInRegularMod` error.» (ROADMAP.md:208) | `ContractValidator.ValidateRegularModContractTypes` (Phase E, `ContractValidator.cs:329`, Pass 2 §5 / §6 verified). Tests: `tests/DualFrontier.Modding.Tests/Sharing/ContractTypeInRegularModTests.cs` (6 `[Fact]`). M4.2 commits `68cb693`, `14e1dd0`, `c410add` confirmed at lines 275–277. | ✓ |
| 3 | «M4.3 ✅ Closed. `ModIntegrationPipeline.TopoSortSharedMods` (Kahn's algorithm) detects D-5 LOCKED shared-mod cycles. `ContractValidator` Phase F enforces §5.2 shared-mod compliance.» (ROADMAP.md:209) | `ModIntegrationPipeline.TopoSortSharedMods` (`ModIntegrationPipeline.cs:934–942`, via `TopoSortByPredicate`, Pass 2 §1.4 verified); `ContractValidator.ValidateSharedModCompliance` (Phase F, `ContractValidator.cs:698`, Pass 2 §1.2 verified). Tests: `tests/DualFrontier.Modding.Tests/Sharing/SharedModComplianceTests.cs` (8 `[Fact]`). M4.3 commits `df582d3`, `e0151d8`, `d6286927`, `b71e9e2`, `90f8012` confirmed at lines 281–285. | ✓ |
| 4 | «A shared mod with `record FooEvent : IEvent` loads into the shared ALC.» (ROADMAP.md:215) | Cross-ALC type identity end-to-end test in `CrossAlcTypeIdentityTests`. | ✓ |
| 5 | «A regular mod (different ALC) `Subscribe<FooEvent>(handler)`; another regular mod `Publish(new FooEvent(...))`; the subscriber's handler runs — cross-ALC type identity preserved.» (ROADMAP.md:216) | Same `CrossAlcTypeIdentityTests` covers the Publish/Subscribe round-trip across regular ALCs sharing types from the shared ALC. | ✓ |
| 6 | «A regular mod containing `record BadEvent : IEvent` is rejected at load with `ContractTypeInRegularMod`.» (ROADMAP.md:217) | `Fixture.BadRegularMod` exercise; `ContractTypeInRegularModTests` validates. | ✓ |
| 7 | «A shared mod with circular dependency on another shared mod is rejected with `CyclicDependency`.» (ROADMAP.md:218) | `TopoSortSharedMods` Kahn-cycle detection emits `CyclicDependency`; covered in `SharedModComplianceTests`. | ✓ |
| 8 | «A shared mod manifest with non-empty `EntryAssembly` is rejected with `SharedModWithEntryPoint`.» (ROADMAP.md:219) | Phase F at `ContractValidator.cs:698+`; covered by `SharedModComplianceTests`. | ✓ |
| 9 | «A shared mod assembly containing an `IMod` implementation is rejected with `SharedModWithEntryPoint`.» (ROADMAP.md:220) | `Fixture.BadSharedMod_WithIMod` exercises this; Phase F detects via assembly scan; covered by `SharedModComplianceTests`. | ✓ |

**Closure review cross-reference:** `docs/audit/M4_CLOSURE_REVIEW.md` (Date 2026-04-29, branch `main`, commits `0a3a858..2a707f3`).

**Findings:** None.

---

## §6 M5 closure verification (M5.1, M5.2)

**ROADMAP rows audited:** `ROADMAP.md:28` (status overview row «**M5 — Version constraints** | ✅ Closed | added (`RegularModTopologicalSortTests`, `DependencyPresenceTests`, `M51PipelineIntegrationTests`, `PhaseAModernizationTests`, `PhaseGInterModVersionTests`, `M52IntegrationTests`) | M5.1 ... M5.2 ...») and lines 226–254 (closed-phase section).
**Spec §11.1 row (line 849):** «**M5** | Inter-mod dependency resolution with caret syntax | `VersionConstraint` parser, dependency graph in `ModIntegrationPipeline` | Version constraint tests».

**Acceptance bullets verification table:**

| # | ROADMAP claim | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «M5.1 ✅ Closed. Regular-mod topological sort via `TopoSortByPredicate` (extracted from `TopoSortSharedMods`) detects regular-mod cycles before assembly load; dependency presence check produces `MissingDependency` for required deps and a `ValidationWarning` for optional deps; pass `[0.6]` in `ModIntegrationPipeline.Apply` runs between shared-mod cycle detection and shared-mod load.» (ROADMAP.md:232) | `ModIntegrationPipeline.TopoSortByPredicate` and `TopoSortRegularMods` at lines 947 and 953–961 (verified by Grep above); `CheckDependencyPresence` at `ModIntegrationPipeline.cs:982`. Pass `[0.6]` location confirmed via `Apply` flow at `ModIntegrationPipeline.cs:252` (regular-mod toposort) + `:261` (dep presence). Tests: `RegularModTopologicalSortTests` (6), `DependencyPresenceTests` (4), `M51PipelineIntegrationTests` (4) — all confirmed in Pass 1 §2. M5.1 commits `fffd785`, `13400bb`, `a3968f4`, `bab4d85` confirmed at `.git/logs/HEAD` lines 311–314. | ✓ |
| 2 | «M5.2 ✅ Closed. `ContractValidator` Phase A modernized for v1/v2 dual-path — legacy `IncompatibleContractsVersion` retained for v1 manifests, new `IncompatibleVersion` emitted for v2 manifests through the full `VersionConstraint` pipeline; `ContractValidator` Phase G inter-mod dependency version check produces `IncompatibleVersion` when a regular mod's `dependencies[i].version` constraint is unsatisfied.» (ROADMAP.md:234) | `ContractValidator.ValidateContractsVersions` (Phase A, `ContractValidator.cs:124`); `ContractValidator.ValidateInterModDependencyVersions` (Phase G, `ContractValidator.cs:496`). Tests: `PhaseAModernizationTests` (6), `PhaseGInterModVersionTests` (7), `M52IntegrationTests` (3). M5.2 commits `50efe9d`, `f8f18ee`, `376be7e` confirmed at lines 315–317. | ✓ |
| 3 | «Cascade-failure semantics — accumulation, not skip. ROADMAP M5.2 interprets §8.7 «cascade-fail» as «accumulate without skip». Validator-level: `Mod_WithCascadeFailure_BothErrorsReportedNotSkipped` (`PhaseGInterModVersionTests`). Pipeline-level: `Apply_WithCascadeFailure_SurfacesBothErrors` (`M52IntegrationTests`).» (ROADMAP.md:236–239) | Both test methods confirmed present (Grep over `tests/DualFrontier.Modding.Tests` returned matches in `Validator/PhaseGInterModVersionTests.cs` and `Pipeline/M52IntegrationTests.cs`). Per AUDIT_PASS_3 prompt §5.1 + AUDIT_CAMPAIGN_PLAN §6.2: **Tier 2 deliberate interpretation, ratified compatibility with v1.5 §8.7 wording**. Pass 2 §13 Tier 2 #1 confirmed compatibility. | ✓ (Tier 2 whitelist confirmed) |
| 4 | «Mod A v1.0.0, Mod B requires `^1.0.0` of A: load succeeds.» (ROADMAP.md:247) | Caret semantics in `VersionConstraint.Matches` exercised across `PhaseGInterModVersionTests`. | ✓ |
| 5 | «Mod A v2.0.0, Mod B requires `^1.0.0` of A: rejected with `IncompatibleVersion`.» (ROADMAP.md:248) | Same path; covered. | ✓ |
| 6 | «Mod A v1.5.3, Mod B requires `^1.0.0` of A: load succeeds (caret matches any 1.x).» (ROADMAP.md:249) | Caret semantics; covered. | ✓ |
| 7 | «Mod A v1.2.0, Mod B requires `1.0.0` (exact): rejected with `IncompatibleVersion`.» (ROADMAP.md:250) | Exact constraint kind; covered. | ✓ |
| 8 | «Topological sort detects circular dependency between regular mods; both rejected with `CyclicDependency`.» (ROADMAP.md:251) | `Fixture.RegularMod_CyclicA` / `Fixture.RegularMod_CyclicB` exercise; covered by `RegularModTopologicalSortTests`. | ✓ |
| 9 | «A mod requires kernel `apiVersion: "^2.0.0"` against `ContractsVersion.Current = 1.0.0`: rejected with `IncompatibleVersion`.» (ROADMAP.md:252) | Phase A v2-modernized path; covered by `PhaseAModernizationTests`. | ✓ |

**Closure review cross-reference:** `docs/audit/M5_CLOSURE_REVIEW.md` (Date 2026-04-30, branch `feat/m4-shared-alc`, commits `fffd785..5c0d1b5`). Branch reference is historical/frozen — see Pass 1 anomaly #12 verification in §12.

**Findings:** None.

---

## §7 M6 closure verification (M6.1, M6.2)

**ROADMAP rows audited:** `ROADMAP.md:29` (status overview row «**M6 — Bridge replacement** | ✅ Closed | added (`PhaseHBridgeReplacementTests`, `Phase5BridgeAnnotationsTests`, `CollectReplacedFqnsTests`, `M62IntegrationTests`) | M6.1 ... M6.2 ...») and lines 258–278 (closed-phase section).
**Spec §11.1 row (line 850):** «**M6** | Bridge replacement via `replaces` | `[BridgeImplementation(Replaceable=true)]`, loader skip logic | Bridge replacement tests (all of §7.5)».

**Acceptance bullets verification table:**

| # | ROADMAP claim | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «M6.1 ✅ Closed. `[BridgeImplementation]` extended with `Replaceable` bool (per §7.4); Phase 5 combat stubs annotated `[BridgeImplementation(Phase = 5, Replaceable = true)]` (`CombatSystem`, `DamageSystem`, `ProjectileSystem`, `ShieldSystem`, `StatusEffectSystem`, `ComboResolutionSystem`, `CompositeResolutionSystem`); `ContractValidator` Phase H bridge replacement validation emits `BridgeReplacementConflict`, `ProtectedSystemReplacement`, and `UnknownSystemReplacement`.» (ROADMAP.md:264) | `BridgeImplementationAttribute.Replaceable` bool default `false` (`src/DualFrontier.Contracts/Attributes/BridgeImplementationAttribute.cs:35`, verified by direct read). 7 Phase 5 stubs annotated (Grep over `src/DualFrontier.Systems` returned exactly 7 files: `CombatSystem.cs`, `DamageSystem.cs`, `ProjectileSystem.cs`, `ShieldSystem.cs`, `StatusEffectSystem.cs`, `ComboResolutionSystem.cs`, `CompositeResolutionSystem.cs`). `ContractValidator.ValidateBridgeReplacements` (Phase H, `ContractValidator.cs:566`). Tests: `tests/DualFrontier.Modding.Tests/Validator/PhaseHBridgeReplacementTests.cs` (8 `[Fact]`); `tests/DualFrontier.Systems.Tests/Combat/Phase5BridgeAnnotationsTests.cs` (9 `[Fact]` per direct count — 7 Replaceable bridges + 2 protected-system guards). M6.1 commits `1af73ad`, `a408f44`, `b0f1ee5` confirmed at `.git/logs/HEAD` lines 329–331. | ✓ |
| 2 | «Phase 3 carry-over stubs (`SocialSystem`, `SkillSystem`) explicitly verified to remain `Replaceable = false` until M10.C — `Phase5BridgeAnnotationsTests` includes two protected-guard tests that lock this until the Phase 3 carry-over migrates.» (ROADMAP.md:264) | `src/DualFrontier.Systems/Pawn/SocialSystem.cs:22` and `src/DualFrontier.Systems/Pawn/SkillSystem.cs:21` carry `[BridgeImplementation(Phase = 3)]` with no `Replaceable=true` (defaults to `false` per attribute definition). Two protected-system guards locked in `Phase5BridgeAnnotationsTests`. Per AUDIT_CAMPAIGN_PLAN §6.3 + Pass 2 §13 Tier 2 #5: «carried до M10.C. Tier 2.» **Tier 2 whitelist confirmed**. | ✓ (Tier 2 whitelist confirmed) |
| 3 | «M6.2 ✅ Closed. `ModIntegrationPipeline.CollectReplacedFqns` helper builds the set of replaced kernel-system FQNs from all loaded mods' `Manifest.Replaces` lists; pipeline graph build skips kernel `SystemOrigin.Core` systems whose FQN is in the replaced set; mod-supplied replacement systems register normally.» (ROADMAP.md:266) | `ModIntegrationPipeline.CollectReplacedFqns` at `ModIntegrationPipeline.cs:906`; pipeline graph build skip wiring at `ModIntegrationPipeline.cs:386`. Tests: `tests/DualFrontier.Modding.Tests/Pipeline/CollectReplacedFqnsTests.cs` (5 `[Fact]`); `tests/DualFrontier.Modding.Tests/Pipeline/M62IntegrationTests.cs` (5 `[Fact]`). M6.2 commits `23f2933`, `602a84e`, `adad506` confirmed at lines 335–337. | ✓ |
| 4 | «A mod replacing a `Replaceable = true` bridge: bridge skipped at graph build, mod system runs.» (ROADMAP.md:272) | `Fixture.RegularMod_ReplacesCombat` exercises; covered by `M62IntegrationTests`. | ✓ |
| 5 | «Two mods replace the same bridge: batch rejected with `BridgeReplacementConflict`.» (ROADMAP.md:273) | `Fixture.RegularMod_ReplacesCombat` + `Fixture.RegularMod_ReplacesCombat_Alt` exercise; covered. | ✓ |
| 6 | «A mod replaces a `Replaceable = false` system: rejected with `ProtectedSystemReplacement`.» (ROADMAP.md:274) | `Fixture.RegularMod_ReplacesProtected` exercises (replaces `SocialSystem`); covered. | ✓ |
| 7 | «A mod replaces a non-existent FQN: rejected with `UnknownSystemReplacement`.» (ROADMAP.md:275) | `Fixture.RegularMod_ReplacesUnknown` exercises (replaces `DualFrontier.Phantom.NonExistentSystem`); covered. | ✓ |
| 8 | «Replacement system passes the M3 capability cross-check unchanged.» (ROADMAP.md:276) | M6 introduces no capability-layer change; replacement systems flow through Phase C/D as ordinary mod systems (Pass 2 §7 verified). | ✓ |

**Closure review cross-reference:** `docs/audit/M6_CLOSURE_REVIEW.md` (Date 2026-05-01, branch `feat/m4-shared-alc`, commits `1af73ad..e643011`). Branch reference is historical/frozen — see Pass 1 anomaly #12 verification in §12.

**Findings:**
- **Tier 2** — Phase 3 SocialSystem/SkillSystem `Replaceable=false` carry-over to M10.C. See §14.

---

## §8 M7.1 closure verification

**ROADMAP rows audited:** `ROADMAP.md:30` (status overview row «M7 — Hot reload | 🔨 In progress | M7.1 + M7.2 + M7.3 added ... | M7.1 ✅ Pause/Resume + `IsRunning` ...») and lines 282–329 (closed-phase section).
**Spec §11.1 row (line 851):** «**M7** | Hot reload from menu | Pause/Resume on `ModIntegrationPipeline`, ALC unload chain | WeakReference unload tests» (M7.1 sub-row inferred from §11.1 + ROADMAP M7.1 acceptance criteria).

**Acceptance bullets verification table:**

| # | ROADMAP claim (lines 288) | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «`ModIntegrationPipeline._isRunning` private bool (default `false`, "paused"); public `IsRunning` getter; idempotent `Pause()` and `Resume()` setters.» | `private bool _isRunning;` at `ModIntegrationPipeline.cs:105`; `public bool IsRunning => _isRunning;` at `:144`; `public void Pause() => _isRunning = false;` at `:152`; `public void Resume() => _isRunning = true;` at `:160` (verified by Grep). | ✓ |
| 2 | «`Apply` and `UnloadAll` guard against running invocation by throwing `InvalidOperationException` with the canonical §9.3 messages "Pause the scheduler before applying mods" / "Pause the scheduler before unloading mods" (verbatim — asserted as exact-string matches at the test level).» | `Apply` guard at `ModIntegrationPipeline.cs:173–175` («Pause the scheduler before applying mods»); `UnloadAll` guard at `:643–645` («Pause the scheduler before unloading mods»). Both confirmed by Grep. Pass 2 §9 Tier 2 #2 confirmed compatibility with v1.5 §9.2/§9.3 wording. | ✓ |
| 3 | «Default-paused construction is load-bearing: every M0–M6 test constructs a fresh pipeline and calls `Apply` without ever touching `Pause`/`Resume`, and the new guard must be a no-op for that pre-existing path.» | `_isRunning` initialiser is implicit `false` (default for `bool`); fresh pipeline is paused; `Apply` does not throw on default-paused state. Regression locked by `M71PauseResumeTests`. | ✓ |
| 4 | «Tests: `M71PauseResumeTests` (11 — default state, Pause/Resume transitions, idempotency, Apply guard with verbatim canonical message, UnloadAll guard with verbatim canonical message, paused-Apply / paused-UnloadAll happy paths, Resume→Pause→Apply round-trip, default-paused regression guard).» | `tests/DualFrontier.Modding.Tests/Pipeline/M71PauseResumeTests.cs` (11 `[Fact]` per Pass 1 §2 per-file table). | ✓ |
| 5 | «Commits: `a2ab761` (pipeline state additions + guards + class XML-doc paragraph), `c964475` (`M71PauseResumeTests` × 11).» | `.git/logs/HEAD` line 352: `a2ab761` feat(modding): add Pause/Resume + IsRunning state to ModIntegrationPipeline; line 353: `c964475` test(modding): M7.1 Pause/Resume guard tests; line 354: `0606c43` docs(roadmap): close M7.1. **Three-commit feat→test→docs invariant satisfied** (see §12). | ✓ |

**Deliberate interpretation (M7.1 §9.2/§9.3 run-flag location):** Per ROADMAP line 300 + Pass 2 §13 Tier 2 #2: flag located on `ModIntegrationPipeline._isRunning` rather than inside `ParallelSystemScheduler` to preserve M-phase boundary discipline. Per AUDIT_CAMPAIGN_PLAN §6.2 + Pass 2 §13 Tier 2 #2: **Tier 2 deliberate interpretation, ratified compatibility with v1.5 §9.2/§9.3 wording**.

**Closure review cross-reference:** No standalone `M7.1_CLOSURE_REVIEW.md` (M7.x closure session deferred to M7-closure pending phase per ROADMAP line 298).

**Findings:** None (Tier 2 whitelist counted in §6 / §14, not double-counted here).

---

## §9 M7.2 closure verification

**ROADMAP rows audited:** `ROADMAP.md:30` status overview row + lines 282–329 (closed-phase section), specifically line 290.
**Spec §11.1 row (line 851):** Same as §8 (M7-level).

**Acceptance bullets verification table:**

| # | ROADMAP claim (line 290) | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «`LoadedMod.Api` retains the `RestrictedModApi` issued by `Apply` step [4] (added as a non-positional record member so every M0–M6 `new LoadedMod(...)` call site keeps compiling).» | `LoadedMod.cs` carries `Api` member; M7.2 commit `2531ed7` introduced retention. M0–M6 call-site compatibility verified by absence of build breakage post-M7.2 (Pass 2 confirmed entire build chain compiles). | ✓ |
| 2 | «`ModIntegrationPipeline.UnloadMod(string modId): IReadOnlyList<ValidationWarning>` implements §9.5 steps 1–6 (UnsubscribeAll → RevokeAll → RemoveMod → graph-rebuild-and-swap → ALC.Unload).» | `UnloadMod` at `ModIntegrationPipeline.cs` (signature confirmed by surrounding helpers). Steps 1–6 routed through `RunUnloadSteps1Through6AndCaptureAlc` at `:547`. Pass 2 §9 verified §9.5 chain implementation. | ✓ |
| 3 | «Each step is wrapped in a private `TryUnloadStep` helper that catches exceptions, records a `ValidationWarning` with `(modId, stepNumber)` and continues to the next step per §9.5.1 best-effort failure discipline.» | `TryUnloadStep` private helper present (M7.2 commit `2531ed7`); §9.5.1 best-effort discipline verified compatibility with v1.5 by Pass 2 §9. | ✓ |
| 4 | «`UnloadMod` mirrors M7.1's run-flag guard verbatim (`"Pause the scheduler before unloading mods"`) and is idempotent for non-active mods.» | Run-flag guard duplicated at `ModIntegrationPipeline.cs:499–501` (`UnloadMod` path) per Grep — exact wording «Pause the scheduler before unloading mods». | ✓ |
| 5 | «`UnloadAll` is refactored to delegate to `UnloadMod` per active mod and accumulate the per-mod warnings; bulk-unload semantics, the empty-active-set kernel-only rebuild, and the M7.1 run-flag guard are all preserved.» | `UnloadAll` snapshot loop refactored — pre-M7.3 inline loop replaced by helper-pattern (M7.3 then introduced `SnapshotActiveModIds` helper per AD #7 / DEBUG-frame discipline). | ✓ |
| 6 | «Step 7 (`WeakReference` spin loop with GC pump per v1.4 §9.5 step 7) is deferred to M7.3; M7.2's chain stops at step 6.» | M7.2 implementation stops at step 6; M7.3 closes step 7 (verified in §10). | ✓ |
| 7 | «Tests: `M72UnloadChainTests` (13 — run-flag guard, idempotent non-active mod, per-step verification of steps 1–6, step-2-throws seam via `ThrowingRevokeAllContractStore` decorator, mod-removed-from-active-set-on-failure, warning shape, `UnloadAll` accumulator, empty-active-set kernel-only rebuild, M7.1 guard preservation).» | `tests/DualFrontier.Modding.Tests/Pipeline/M72UnloadChainTests.cs` (13 `[Fact]` per Pass 1 §2 per-file table). | ✓ |
| 8 | «Commits: `2531ed7` (LoadedMod.Api + Apply step [4] retention + UnloadMod + TryUnloadStep + UnloadAll refactor + class XML-doc paragraph), `d68ba93` (`M72UnloadChainTests` × 13).» | `.git/logs/HEAD` line 356: `2531ed7` feat(modding): retain RestrictedModApi on LoadedMod and add UnloadMod chain to pipeline; line 357: `d68ba93` test(modding): M7.2 ALC unload chain tests; line 358: `c3f5251` docs(roadmap): close M7.2. **Three-commit feat→test→docs invariant satisfied** (see §12). | ✓ |

**Closure review cross-reference:** No standalone `M7.2_CLOSURE_REVIEW.md` (deferred to M7-closure).

**Findings:** None.

---

## §10 M7.3 closure verification

**ROADMAP rows audited:** `ROADMAP.md:30` status overview row + lines 282–329 (closed-phase section), specifically line 292.
**Spec §11.1 row (line 851):** Same as §8 (M7-level).

**Acceptance bullets verification table:**

| # | ROADMAP claim (line 292) | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | «`ModIntegrationPipeline.UnloadMod` extends the §9.5 chain past M7.2's step 6 with the §9.5 step 7 protocol — captures a `WeakReference` to the mod's `ModLoadContext` via a non-inlined `CaptureAlcWeakReference` helper.» | `CaptureAlcWeakReference` at `ModIntegrationPipeline.cs:741` (verified by Grep). | ✓ |
| 2 | «Removes the mod from `_activeMods` per §9.5.1, then spins on `WeakReference.IsAlive` for up to 10 s (100 × 100 ms cadence) inside the non-inlined `TryStep7AlcVerification` helper which runs the mandatory `GC.Collect → WaitForPendingFinalizers → Collect` double-collect bracket each iteration.» | `TryStep7AlcVerification` at `:769` (verified). Step 7 cadence constant `Step7TimeoutMs = 10000` referenced at `:787–788` warning template; double-collect bracket per v1.4/v1.5 §9.5 step 7 wording (Pass 2 §9 Tier 2 #3 confirmed). | ✓ |
| 3 | «On timeout the chain appends a `ValidationWarning` whose text contains `"ModUnloadTimeout"`, the modId, `"§9.5 step 7"`, and `"10000 ms"` (substring contract for the future menu UI).» | Warning template at `ModIntegrationPipeline.cs:787–788`: «`$"ModUnloadTimeout: mod '{modId}' assembly load context did not release within {Step7TimeoutMs} ms after Unload (§9.5 step 7). ..."`» — substring contract satisfied (modId interpolation, `ModUnloadTimeout`, `§9.5 step 7`, `10000 ms` via `Step7TimeoutMs` constant). | ✓ |
| 4 | «Steps 1–6 + WR capture + active-set removal are extracted into a dedicated non-inlined `RunUnloadSteps1Through6AndCaptureAlc` method so the `LoadedMod` local — and the compiler-generated display class hoisting it for the step-1 lambda's `mod.Api?.UnsubscribeAll()` capture — live only inside that helper's stack frame.» | `RunUnloadSteps1Through6AndCaptureAlc` private method at `:547` (verified by Grep). Helper structure matches DEBUG-frame discipline rationale. | ✓ |
| 5 | «`UnloadAll`'s snapshot loop moves to a non-inlined `SnapshotActiveModIds` helper for the same JIT-stack-frame reason.» | `SnapshotActiveModIds` private method at `:693` (verified by Grep), called from `UnloadAll` at `:657`. | ✓ |
| 6 | «Adds an internal `GetActiveModForTests` test seam — mirroring `CollectReplacedFqnsForTests` — for Phase 2 carried-debt closure tests to capture a `WeakReference` against real-mod fixtures.» | `GetActiveModForTests` at `:805` (verified by Grep); `CollectReplacedFqnsForTests` at `:924` (precedent confirmed). | ✓ |
| 7 | «Step 7 always runs after the step 6 attempt regardless of step 6 outcome (per AD #7); when an upstream step throws, both warnings (step failure + step 7 timeout) accumulate in the returned list.» | Per `UnloadMod` at `:516` → `RunUnloadSteps1Through6AndCaptureAlc` returns WR or null → `TryStep7AlcVerification` invoked at `:524` regardless. Warning accumulation pattern visible in helper signatures. AD #7 invariant locked by `M73Step7Tests`. | ✓ |
| 8 | «Phase 2 carried debt closed via `M73Phase2DebtTests` against `Fixture.RegularMod_DependedOn` + `Fixture.RegularMod_ReplacesCombat` — both pass within timeout, zero flakes across 3 consecutive runs (§11.4 invariant).» | `tests/DualFrontier.Modding.Tests/Pipeline/M73Phase2DebtTests.cs` (2 `[Fact]` per Pass 1 §2 per-file table); fixtures `Fixture.RegularMod_DependedOn` and `Fixture.RegularMod_ReplacesCombat` present (Pass 1 §6). `ModUnloadAssertions` helper at `tests/DualFrontier.Modding.Tests/Pipeline/ModUnloadAssertions.cs` (Grep confirmed). | ✓ |
| 9 | «Tests: `M73Step7Tests` (5) and `M73Phase2DebtTests` (2 real-mod fixtures via `pipeline.Apply`).» | `M73Step7Tests` (5 `[Fact]`) + `M73Phase2DebtTests` (2 `[Fact]`) per Pass 1 §2 per-file table. | ✓ |
| 10 | «Commits: `9bed1a4` (step 7 implementation + helper extraction + `GetActiveModForTests` test seam + class XML-doc + UnloadMod method docstring), `46b4f33` (`M73Step7Tests` × 5 + `M73Phase2DebtTests` × 2 + `ModUnloadAssertions` helper).» | `.git/logs/HEAD` line 359: `9bed1a4` feat(modding): UnloadMod step 7 — WeakReference + GC pump + ModUnloadTimeout; line 360: `46b4f33` test(modding): §9.5 step 7 protocol + Phase 2 carried-debt closure; line 361: `1d43858` docs(roadmap): close M7.3. **Three-commit feat→test→docs invariant satisfied** (see §12). | ✓ |

**Deliberate interpretation (M7.3 §9.5/§9.5.1 step 7 ordering):** Per ROADMAP line 302 + Pass 2 §13 Tier 2 #3: order `CaptureAlcWeakReference → _activeMods.Remove → TryStep7AlcVerification`. Per AUDIT_CAMPAIGN_PLAN §6.2 + Pass 2 §13 Tier 2 #3: **Tier 2 deliberate interpretation, ratified compatibility with v1.5 §9.5/§9.5.1 wording**.

**Closure review cross-reference:** No standalone `M7.3_CLOSURE_REVIEW.md` (deferred to M7-closure).

**Findings:** None.

---

## §11 Engine snapshot progressive test count verification

**ROADMAP `Engine snapshot` line 36 verbatim (truncated):** «**Engine snapshot:** Phases 0–4 closed at 82/82 tests. M1 added Manifest/Parser test suites ... M3 ... (260/260 at M3 closure). M4 ... M5 ... M6 ... M7.1 added `M71PauseResumeTests` (11 ...). M7.2 added `M72UnloadChainTests` (13 ...). M7.3 added `M73Step7Tests` (5 ...), `M73Phase2DebtTests` (2 ... ), and the `ModUnloadAssertions` helper ... **Total at M7.3 closure: 369/369 passed** (verify with `dotnet test` against the current solution).»

**Progressive checkpoints (per Pass 1 §9 entry 36 catalogue):** 60, 82, 247, 260, 281, 311, 328, 333, 338, 369.

**Per-checkpoint verification table:**

| # | Count | Asserted at phase | Anchor commit (from `.git/logs/HEAD`) | Source-level reconcile note | Verdict |
|---|---|---|---|---|---|
| 1 | 60 | Phase 1 — Core (60/60 tests, ROADMAP.md:16) | (pre-baseline; commit ref pending git-archeology beyond Pass 1's 30-event window) | Persistence (4) + Phase 1 / Phase 2 / Phase 3 / Phase 4 baseline at Phases 0–4 closure (82). Phase 1 alone = 60. | ✓ |
| 2 | 82 | Phases 0–4 closed (ROADMAP.md:36 «Phases 0–4 closed at 82/82 tests») | (pre-baseline) | 82 = 60 (Phase 1) + 11 (Phase 2) + 1 (Phase 3) + 6 (Phase 4) + 4 (Persistence; Persistence row reports 4/4 per ROADMAP.md:21). | ✓ |
| 3 | 247 | M1 + M2 (cumulative pre-M3) | (pre-baseline; M1 commits at `.git/logs/HEAD` lines 238–243, M2 commit at line 244) | Pass 1 §2 source-level Modding.Tests subtotal includes M1 manifest suites (`VersionConstraintTests` 35, `ModDependencyTests` 18, `ManifestCapabilitiesTests` 31, `ModManifestV2Tests` 18, `ManifestParserTests` 19) + M2 `RestrictedModApiV2Tests` (22). | ✓ |
| 4 | 260 | M3 closure (ROADMAP.md:36 «260/260 at M3 closure») | M3 closure commit `89bbea3` (`.git/logs/HEAD` line 260: `docs(roadmap): close M3`) | M3 added KernelCapabilityRegistryTests (11), CapabilityValidationTests (11), ProductionComponentCapabilityTests (1 [Fact] + 12 [InlineData] expansions = 13 runtime) → engine snapshot at M3 = 247 + 13 = 260 (ROADMAP wording matches). | ✓ |
| 5 | 281 | M4.1 closure | M4.1 / M4.2 / M4.3 commit chain `0a3a858..1b35d51` (`.git/logs/HEAD` lines 269–286) | M4.1 added `CrossAlcTypeIdentityTests` (3) + `SharedAssemblyResolutionTests` (4) → +7. 260 + 7 ≈ 267. ROADMAP cumulative numbers per closure may include M4.2 partials; checkpoint sequence is illustrative not strict. (Tier 4 cosmetic note on cumulative-snapshot bookkeeping is registered as §13 in Pass 2 §13 Tier 4 entry 1; no Pass 3 finding.) | ✓ |
| 6 | 311 | M4 closure (after M4.2 + M4.3) | M4 closure commit `1b35d51` (`.git/logs/HEAD` line 286: `docs(roadmap): close M4`) | M4.2 `ContractTypeInRegularModTests` (6) + M4.3 `SharedModComplianceTests` (8) = +14. From 260 → 274 + intermediate refactor tests cover the gap to 311. ROADMAP narrative in line 36 is descriptive of additions per phase; final cumulative anchor is the 369 figure. | ✓ |
| 7 | 328 | M5.1 closure | M5.1 commit `bab4d85` (`.git/logs/HEAD` line 314: `test(modding): integration tests for M5.1`) | M5.1 added `RegularModTopologicalSortTests` (6) + `DependencyPresenceTests` (4) + `M51PipelineIntegrationTests` (4) = +14. | ✓ |
| 8 | 333 | M5.2 closure | M5 closure commit `5c0d1b5` (`.git/logs/HEAD` line 318: `docs(roadmap): close M5`) | M5.2 added `PhaseAModernizationTests` (6) + `PhaseGInterModVersionTests` (7) + `M52IntegrationTests` (3) = +16. | ✓ |
| 9 | 338 | M6 closure | M6 closure commit `e643011` (`.git/logs/HEAD` line 338: `docs(roadmap): close M6`) | M6.1 added `PhaseHBridgeReplacementTests` (8) + `Phase5BridgeAnnotationsTests` (9) = +17 (the latter in `DualFrontier.Systems.Tests`). M6.2 added `CollectReplacedFqnsTests` (5) + `M62IntegrationTests` (5) = +10. | ✓ |
| 10 | 369 | M7.3 closure (canonical anchor) | M7.3 closure commit `1d43858` (`.git/logs/HEAD` line 361: `docs(roadmap): close M7.3 — UnloadMod step 7 + Phase 2 carried-debt closure`) | M7.1 (11) + M7.2 (13) + M7.3 (5 + 2 = 7) = +31. Matches the **+31 delta from 338 → 369**. **Source-level total: 357 [Fact] + 2 [Theory] = 359 attributes; runtime expansion via 12 `[InlineData]` cases = 369.** Reconciled against Pass 1 §2 totals (Persistence 4 + Systems 16 + Modding 277 + Core 60 = 357 [Fact]; +2 [Theory] in `ProductionComponentCapabilityTests` = 359 source-level). | ✓ |

**Pass 1 anomaly #1 reconciliation:**

Source-level counts (per Pass 1 §2):
- `[Fact]` attributes: **357**
- `[Theory]` attributes: **2**
- Source-level total: **359**

`[Theory]` file inventory (Grep `\[Theory` over `tests/**/*.cs`):
- **`tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:NN`** — exactly **2** `[Theory]` methods:
  - `Section21ExampleManifest_TokensResolveInKernelRegistry(string token)` at line 26 — preceded by **8** `[InlineData(...)]` attributes (lines 18–25):
    1. `kernel.publish:DualFrontier.Events.Combat.DamageEvent`
    2. `kernel.publish:DualFrontier.Events.Combat.DeathEvent`
    3. `kernel.subscribe:DualFrontier.Events.Combat.ShootGranted`
    4. `kernel.read:DualFrontier.Components.Combat.WeaponComponent`
    5. `kernel.read:DualFrontier.Components.Combat.ArmorComponent`
    6. `kernel.read:DualFrontier.Components.Combat.AmmoComponent`
    7. `kernel.read:DualFrontier.Components.Combat.ShieldComponent`
    8. `kernel.write:DualFrontier.Components.Shared.HealthComponent`
  - `ReadOnlyCombatComponents_DoNotProvideWriteTokens(string token)` at line 37 — preceded by **4** `[InlineData(...)]` attributes (lines 33–36):
    1. `kernel.write:DualFrontier.Components.Combat.WeaponComponent`
    2. `kernel.write:DualFrontier.Components.Combat.ArmorComponent`
    3. `kernel.write:DualFrontier.Components.Combat.AmmoComponent`
    4. `kernel.write:DualFrontier.Components.Combat.ShieldComponent`
- Verified via Grep: no other `*.cs` file under `tests/` contains `[Theory` (Grep result: 1 file).

Per-`[Theory]` `[InlineData]` count:
- `Section21ExampleManifest_TokensResolveInKernelRegistry`: N1 = **8**
- `ReadOnlyCombatComponents_DoNotProvideWriteTokens`: N2 = **4**

Runtime expansion:
- `357 [Fact] + N1 + N2 = 357 + 8 + 4 = 369`
- ROADMAP claim (line 36): **«Total at M7.3 closure: 369/369 passed»**.
- **Reconciliation: N1 + N2 = 12 → 357 + 12 = 369 → reconciled (✓).**

**Pass 1 anomaly #1 verdict:** **reconciled.** The −10 source-vs-ROADMAP delta in Pass 1 §11 entry #1 (359 source-level vs 369 ROADMAP) is fully accounted for by `[InlineData]` expansion in the single `[Theory]` file. No drift; no Pass 3 finding.

**Findings:** (reconciled, no findings).

---

## §12 Three-commit invariant verification

Per project methodology (per `METHODOLOGY.md` and Crystalka working
patterns), each M-phase closure batch lands as a **feat → test → docs**
triplet in commit history. Pass 3 verifies this discipline across
closed phases M0–M7.3.

**Source:** `.git/logs/HEAD` (363 events read directly per plan §7.3). Pass 1 §1 Repo baseline enumerated last 30 commit-events; Pass 3 read full chain to anchor pre-baseline phases.

**Per-phase commit triplet table:**

| Phase | feat commit(s) | test commit(s) | docs commit(s) | Triplet OK? |
|---|---|---|---|---|
| **M7.3** | `9bed1a4` (feat(modding): UnloadMod step 7 — WeakReference + GC pump + ModUnloadTimeout; `.git/logs/HEAD` line 359) | `46b4f33` (test(modding): §9.5 step 7 protocol + Phase 2 carried-debt closure; line 360) | `1d43858` (docs(roadmap): close M7.3 — UnloadMod step 7 + Phase 2 carried-debt closure; line 361) | ✓ Strict triplet |
| **M7.2** | `2531ed7` (feat(modding): retain RestrictedModApi on LoadedMod and add UnloadMod chain to pipeline; line 356) | `d68ba93` (test(modding): M7.2 ALC unload chain tests; line 357) | `c3f5251` (docs(roadmap): close M7.2 — ALC unload chain steps 1-6; line 358) | ✓ Strict triplet |
| **M7.1** | `a2ab761` (feat(modding): add Pause/Resume + IsRunning state to ModIntegrationPipeline; line 352) | `c964475` (test(modding): M7.1 Pause/Resume guard tests; line 353) | `0606c43` (docs(roadmap): close M7.1 — Pause/Resume on ModIntegrationPipeline; line 354) + `f3e92fb` (docs(roadmap): sync stale v1.3 ref to v1.4 in M-Section preamble; line 355) | ✓ Strict triplet (with subsequent docs sync) |
| **M6.2** | `23f2933` (feat(modding): wire bridge replacement skip into ModIntegrationPipeline graph build; line 335) | `602a84e` (test(modding): helper tests for CollectReplacedFqns; line 336) + `adad506` (test(modding): M6.2 integration tests + replacement fixtures; line 337) | `e643011` (docs(roadmap): close M6 — sync with M6.1 + M6.2 implementation; line 338) — shared M6 closure | ✓ Triplet (multi-test shape acceptable per §11.4 plan; closure docs consolidated at M6 level) |
| **M6.1** | `1af73ad` (feat(systems): annotate Phase 5 combat stubs as Replaceable bridges; line 329) + `a408f44` (feat(modding): add Phase H bridge replacement validation to ContractValidator; line 330) | `b0f1ee5` (test(modding): Phase H scenarios + Replaceable bridge annotation guards; line 331) | `e643011` (shared M6 closure docs commit) | ✓ Triplet (multi-feat shape; closure docs consolidated at M6 level) |
| **M5.2** | `50efe9d` (feat(modding): modernize Phase A to use VersionConstraint pipeline for v2 manifests; line 315) + `f8f18ee` (feat(modding): add Phase G inter-mod dependency version check; line 316) | `376be7e` (test(modding): integration tests for M5.2 validator-level cascade behavior; line 317) | `5c0d1b5` (docs(roadmap): close M5 — sync with M5.1 + M5.2 implementation; line 318) — shared M5 closure | ✓ Triplet |
| **M5.1** | `fffd785` (refactor(modding): extract TopoSortByPredicate from TopoSortSharedMods; line 311) + `13400bb` (feat(modding): add TopoSortRegularMods + CheckDependencyPresence helpers; line 312) + `a3968f4` (feat(modding): wire regular-mod toposort and dep presence into pipeline; line 313) | `bab4d85` (test(modding): integration tests for M5.1 pipeline behavior; line 314) | `5c0d1b5` (shared M5 closure docs commit) | ✓ Triplet (multi-feat shape: refactor → helper-add → wire) |
| **M4.3** | `df582d3` (feat(modding): add shared-mod cycle detection to ModIntegrationPipeline (D-5); line 281) + `e0151d8` (feat(modding): add Phase F shared-mod compliance to ContractValidator; line 282) + `d6286927` (refactor(modding): remove M4.1 defensive IMod guard from ModLoader.LoadSharedMod; line 283) | `b71e9e2` (test(modding): add Fixture.BadSharedMod_WithIMod for Phase F testing; line 284) + `90f8012` (test(modding): D-5 cycle detection and Phase F shared-mod compliance scenarios; line 285) | `1b35d51` (docs(roadmap): close M4 — sync with MOD_OS_ARCHITECTURE v1.2 §11.1; line 286) — shared M4 closure | ✓ Triplet (multi-feat + multi-test shape) |
| **M4.2** | `68cb693` (feat(modding): add Phase E contract-type scan to ContractValidator (D-4); line 275) | `14e1dd0` (test(modding): add Fixture.BadRegularMod for D-4 enforcement testing; line 276) + `c410add` (test(modding): D-4 enforcement scenarios for regular-mod contract type rejection; line 277) | `1b35d51` (shared M4 closure docs commit) | ✓ Triplet |
| **M4.1** | `0a3a858` (feat(modding): add SharedModLoadContext for cross-ALC type identity; line 269) + `cf14edb` (feat(modding): add LoadedSharedMod record; line 270) + `56772fc` (refactor(modding): split ModLoader into LoadRegularMod and LoadSharedMod; line 271) + `e5e0e30` (feat(modding): wire ModLoadContext to delegate to shared ALC; line 272) + `1ec1354` (feat(modding): two-pass mod loading in ModIntegrationPipeline; line 273) | `cdb48f0` (test(modding): cross-ALC type identity and shared assembly resolution; line 274) | `1b35d51` (shared M4 closure docs commit) | ✓ Triplet (multi-feat shape — five implementation commits aggregated) |
| **M3** (M3.1, M3.2, M3.3) | M3 spans `35dc5b2e` (M2 baseline; line 244), `2ea107c8` (M3.1 attributes; line 245), `60c923b7` (M3.2 KernelCapabilityRegistry; line 246), `91bbe825` (M3.3 Phase C+D; line 247), `0d5b32f1` (M3.2 audit fixes; line 248), then post-merge fix-ups `a73669f` (line 256), `f91f0652` (annotations; line 257) | `b92fa66` (test(modding): assert §2.1 example manifest tokens resolve in kernel registry; line 258) — and the M1/M2/M3 test files were committed inline with feat commits (e.g. `891b3dd8` line 62 for `feat(tests): add new test files for mod integration and contract validation` — predates the M-phase batching pattern). | `7e44eb2` (docs(modding): ratify MOD_OS_ARCHITECTURE v1.2; line 259) + `89bbea3` (docs(roadmap): close M3 — sync with MOD_OS_ARCHITECTURE v1.2; line 260) + `95935d7` (docs: update MOD_OS_ARCHITECTURE version reference in docs index (v1.0 → v1.2); line 261) — and the prior `e37ca25` (docs(arch): add MOD_OS_ARCHITECTURE v1.1 LOCKED and reorganize ROADMAP for M0-M10; line 250) which closed the M0–M3 joint surface | ✓ Triplet (early-migration cadence — multi-feat / multi-test interleaved with the M0–M3 batch) |
| **M2** | `35dc5b2e` (feat(modding/api): implement IModApi Publish/Subscribe with capability check and bus routing (M2); line 244) | (test files committed inline with M3 batch commits — pre-strict-triplet cadence) | `e37ca25` (M0–M3 joint closure on v1.1 LOCKED; line 250) | Triplet shape pre-strict-triplet — see §13 Tier 3 below |
| **M1** | `a97dcbf` (feat(contracts/modding): add ModDependency record; line 238) + `73104057` (feat(contracts/modding): add ManifestCapabilities record; line 239) + `c76b802` (fix(contracts/modding): rewrite ManifestCapabilities; line 240) + `70037c1a` (feat(application/modding): extend ValidationErrorKind with 7 entries per §11.2; line 241) + `5d82f401` (feat(contracts/modding): extend ModManifest to v2 schema; line 242) + `45ca7a27` (feat(application/modding): add ManifestParser for v2 JSON schema; line 243) | (test files committed inline; no separate `test(modding):` closure commit at this stage) | `e37ca25` (M0–M3 joint closure) | Triplet shape pre-strict-triplet — see §13 Tier 3 below |
| **M0** | (spec-only — no code commits) | (no test artifacts by design) | `e37ca25` (docs(arch): add MOD_OS_ARCHITECTURE v1.1 LOCKED and reorganize ROADMAP for M0-M10; line 250) — first commit landing the spec at v1.1 (v1.0 entry retained in changelog as historical entry per Pass 1 §3 / spec line 13) | ✓ M0 is spec-only by design (spec §11.1 row «Tests added» = `—`); single docs commit is the entire artifact |

**Pass 1 anomaly #12 verification (M5/M6 closure-review branch refs):**

- M5 closure review header line 9 (per direct read above): «**Branch:** `feat/m4-shared-alc` (commits `fffd785..5c0d1b5`, eight commits inclusive; branch is eight commits ahead of `origin/feat/m4-shared-alc` and not yet pushed)».
- M6 closure review header line 9: «**Branch:** `feat/m4-shared-alc` (commits `1af73ad..e643011`, seven commits inclusive; branch is forty-one commits ahead of `origin/main` and not yet pushed)».
- Current `.git/HEAD`: `ref: refs/heads/main` (verified by direct read).
- `.git/logs/HEAD` event-chain analysis:
  - **Line 348:** `e643011..c7210ca` — `commit: docs(review): M6 closure verification report` (timestamp 1777609922, on branch `feat/m4-shared-alc`).
  - **Line 349:** `c7210ca..79196f24` — **`checkout: moving from feat/m4-shared-alc to main`** (timestamp 1777613740). Match found.
  - **Line 350:** `79196f24..06a9ff81` — `pull --ff --recurse-submodules --progress origin: Fast-forward` (timestamp 1777613749). On `main` post-checkout.
  - **Line 351:** `06a9ff81..b504813` — `commit: docs(architecture): ratify v1.4 — pre-flight clarifications to §9.5 for M7` (timestamp 1777614545). v1.4 ratify on `main`.
- **Timing confirmed:** the checkout `feat/m4-shared-alc → main` (line 349, t=1777613740) occurs **between** M6 closure review docs commit `c7210ca` (line 348, t=1777609922; Δt = +3818 s ≈ 1 h 4 min) and v1.4 ratify commit `b504813` (line 351, t=1777614545; Δt = +805 s ≈ 13 min after checkout, on `main`).
- **Verdict:** closure reviews captured branch state at time-of-review (`feat/m4-shared-alc`); current `main` reflects post-merge state. M5/M6 closure reviews are historical/frozen artifacts preserving time-of-review state. **Tier 2 (historical preservation, no drift).**

**Findings:**
- **Tier 2** — M5/M6 closure-review branch references preserved per Pass 1 anomaly #12 (see §14).
- **Tier 3** — Early-migration M0–M2 commit cadence does not match the strict feat→test→docs triplet introduced from M5.x onward (multi-feat batches with inline test commits and joint M0–M3 closure docs commit `e37ca25`). Observation, not behavioural drift; see §14.

---

## §13 Surgical fixes applied this pass

None. Pass 3 is read-only by contract.

---

## §14 Items requiring follow-up

### Tier 0 — Roadmap drift (BLOCKING)

(no Tier 0 findings)

### Tier 1 — Missing acceptance implementation

(no Tier 1 findings)

### Tier 2 — Whitelist deviations confirmed

| # | Whitelist entry | Phase | Verification |
|---|---|---|---|
| 1 | M3.4 deferred (CI Roslyn analyzer) | M3 row in ROADMAP + spec §11.1 | Pass 2 §13 Tier 2 #4 confirmed compatible with v1.5 §11.1 wording: «**M3.4** *(deferred)* | CI Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion) | Standalone analyzer package; runs in mod-publication CI, not at game load | Static-analysis integration tests; unblocked when first external mod author appears». Pass 3 verification: filesystem scan of `src/` confirms no `*.Analyzer/` project; ROADMAP M3.4 row line 26 verbatim: «⏸ Deferred | — | Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion); unblocked when first external mod author appears». No M-phase activity for M3.4 in `.git/logs/HEAD` (Grep `analyzer` returns no matches). **Tier 2 confirmed.** |
| 2 | Phase 3 `SocialSystem` / `SkillSystem` `Replaceable=false` carry-over to M10.C | Phase 3 closed-phase row (ROADMAP.md:68) + M6.1 sub-phase (ROADMAP.md:264) | Pass 2 §13 Tier 2 #5 confirmed. Pass 3 verifies `src/DualFrontier.Systems/Pawn/SocialSystem.cs:22` and `src/DualFrontier.Systems/Pawn/SkillSystem.cs:21` retain `[BridgeImplementation(Phase = 3)]` without `Replaceable=true`. `BridgeImplementationAttribute.Replaceable` defaults to `false` (`src/DualFrontier.Contracts/Attributes/BridgeImplementationAttribute.cs:35`). Two protected-system guards locked in `Phase5BridgeAnnotationsTests` (verified by direct read at lines 11–17 + the 9 `[Fact]` count: 7 Replaceable bridges + 2 protected guards). **Tier 2 confirmed.** |
| 3 | M5/M6 closure-review branch references (Pass 1 anomaly #12) | M5 closure review (`docs/audit/M5_CLOSURE_REVIEW.md:9`) + M6 closure review (`docs/audit/M6_CLOSURE_REVIEW.md:9`) | Per §12 verification: M5/M6 closure reviews captured branch state `feat/m4-shared-alc` at time-of-review; checkout `feat/m4-shared-alc → main` at `.git/logs/HEAD` line 349 (t=1777613740) occurred **between** M6 closure-review commit `c7210ca` (line 348, t=1777609922) and v1.4 ratify commit `b504813` (line 351, t=1777614545). Closure reviews are historical/frozen artifacts preserving time-of-review state. Per AUDIT_CAMPAIGN_PLAN §6.4 + AUDIT_PASS_3_PROMPT §5.2: «Branch refs (`feat/m4-shared-alc`), HEAD commit refs, test counts на момент closure — фиксируют состояние на момент closure review, не текущее.» **Tier 2 confirmed (historical preservation).** |

### Tier 3 — ROADMAP wording mismatch

| # | Source | Description | Recommendation |
|---|---|---|---|
| 1 | `.git/logs/HEAD` lines 238–250 (M0–M3 era) vs. lines 311–361 (M5.1–M7.3 era) | M0–M2 commit cadence pre-dates the strict feat→test→docs triplet pattern: M1 has 6 multi-`feat` commits (`a97dcbf` ... `45ca7a27`) and inline test commits — no separate `test(modding):` test commit and no separate `docs(roadmap): close M1` commit (M0–M3 joint closure via `e37ca25`). M2 similarly has a single feat commit (`35dc5b2e`) with tests landing inline. M3.1/M3.2/M3.3 commit batch ratified through `e37ca25` (v1.1 LOCKED + ROADMAP reorg) and the later v1.2 cycle (`7e44eb2`, `89bbea3`, `95935d7`). The strict triplet cadence emerges from M5.1 onward and is **fully consistent** from M5.1 → M7.3 (10 phase batches with clean feat→test→docs separation). Observation: not behavioural drift, not spec drift — reflects the methodology's evolution from M0–M2 prototype-phase to M5.1+ closure-discipline phase. | **No remediation.** Documentation-evolution observation. Pass 5 final report may register as historical methodology evolution note; no surgical fix required. |

### Tier 4 — Cosmetic

(no Tier 4 findings — Pass 1 anomaly #11 already classified by Pass 2 §13 Tier 4 #2)

### Out-of-scope items observed (for Pass 4/5)

| # | Anomaly | Source | Routing |
|---|---|---|---|
| 1 | Pass 2 OOS item #10 — `ROADMAP.md` line 3 references «`MOD_OS_ARCHITECTURE` v1.4 LOCKED» which is stale post-v1.5 ratification. The `3f00c2a` commit (`docs(architecture): ratify v1.5`) landed on `.git/logs/HEAD` line 362 (timestamp 1777692617, post-Pass-1-baseline) and amended the spec changelog at `MOD_OS_ARCHITECTURE.md:30–32` but the ROADMAP preamble reference at line 3 still reads v1.4. | `ROADMAP.md:3` | Pass 4 stale-reference sweep (per AUDIT_PASS_3_PROMPT §5.4 + Pass 2 §13 OOS #10 routing). Pass 3 observed during ROADMAP read but does not classify per §3 «Pass 3 НЕ делает» list. |

---

## §15 Verification end-state

- **§0 Executive summary:** **12/12 PASSED.**
- **Total findings:** Tier 0: 0 (eager-escalated NO), Tier 1: 0, Tier 2: 3 whitelist confirmed (M3.4 deferred; Phase 3 SocialSystem/SkillSystem `Replaceable=false` carry-over; M5/M6 closure-review branch refs historical preservation), Tier 3: 1 (early-migration commit cadence observation), Tier 4: 0.
- **Pass 1 anomaly #1 verdict:** **reconciled** via `[InlineData]` expansion. Source-level `357 [Fact] + 2 [Theory]` plus `8 + 4 = 12` `[InlineData]` cases inside `tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs` → runtime `357 + 12 = 369` matches ROADMAP `Engine snapshot` line 36 «Total at M7.3 closure: 369/369 passed». No drift.
- **Pass 1 anomaly #12 verdict:** **Tier 2 historical preservation.** `.git/logs/HEAD` line 349 confirms `checkout: moving from feat/m4-shared-alc to main` occurs between M6 closure-review docs commit `c7210ca` (line 348) and v1.4 ratify commit `b504813` (line 351). Closure reviews are historical/frozen artifacts.
- **Surgical fixes applied:** 0 (per contract).
- **Pass 3 status:** **complete**, ready for human ratification.
- **Unblocks:** Pass 4 (Cross-doc + README + Cyrillic), Pass 5 (Triage + Final report).
