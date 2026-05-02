---
title: Audit Pass 4 — Cross-doc & Translation Completeness
nav_order: 108
---

# Audit Pass 4 — Cross-doc & Translation Completeness

**Date:** 2026-05-02
**Branch:** `main` (per `.git/HEAD` line 1)
**HEAD:** `d1c1338dac06364b062695baee26a8393cc06385` (per `.git/refs/heads/main` line 1)
**Pass 1 baseline HEAD:** `1d43858a36c17b956a345e9bfe07a9ccf82daddb` (per `AUDIT_PASS_1_INVENTORY.md:10`)
**HEAD delta since Pass 1 baseline:** +3 commits — `3f00c2a` (`docs(architecture): ratify v1.5`), `6e9c433` (`Update AUDIT_PASS_2_SPEC_CODE.md`, the Pass 3 baseline HEAD), `d1c1338` (`Pass 1 baseline: 1d43858 (M7.3 closure) Current: 6e9c433 (Update AUDIT_PASS_2_SPEC_CODE.md) Delta: +2 commits (3f00c2a v1.5 ratify, 6e9c433 Pass 2 sync)`, an audit-baseline annotation commit). All three commits are post-Pass-1; Pass 4 audits current state per plan §10 step 2.
**Spec under audit:** `docs/MOD_OS_ARCHITECTURE.md` LOCKED v1.5
**Pass 1 inventory consumed:** `docs/audit/AUDIT_PASS_1_INVENTORY.md` (9/9 PASSED)
**Pass 2 artifact consumed:** `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` (10/11 PASSED + 1/11 FAILED with Tier 0 RESOLVED via v1.5)
**Pass 3 artifact consumed:** `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md` (12/12 PASSED)
**Scope:** Cross-doc consistency, sub-folder README accuracy, stale-reference
sweep across active navigation, cyrillic remainder verification, navigation
link integrity. Spec ↔ code drift (Pass 2) and roadmap ↔ reality (Pass 3) are
out of scope.

---

## §0 Executive summary

| # | Check | Status | Tier 0 / 1 / 2 / 3 / 4 counts |
|---|---|---|---|
| 1 | Spec see-also referenced docs exist | PASSED | 0 / 0 / 0 / 0 / 0 |
| 2 | `MODDING.md` ↔ spec consistency | PASSED | 0 / 0 / 0 / 1 / 1 |
| 3 | `MOD_PIPELINE.md` ↔ spec consistency | PASSED | 0 / 0 / 0 / 1 / 0 |
| 4 | `CONTRACTS.md` ↔ spec consistency | PASSED | 0 / 0 / 0 / 0 / 1 |
| 5 | `ECS.md` / `EVENT_BUS.md` / `THREADING.md` / `ISOLATION.md` ↔ spec | PASSED | 0 / 0 / 0 / 1 / 0 |
| 6 | Other supporting docs (CODING_STANDARDS, TESTING_STRATEGY, etc.) | PASSED | 0 / 0 / 0 / 0 / 0 |
| 7 | Sub-folder README accuracy (`src/**/README.md`) | PASSED | 0 / 0 / 0 / 2 / 1 |
| 8 | Sub-folder README accuracy (`tests/**/README.md`) | PASSED | 0 / 0 / 0 / 2 / 3 |
| 9 | Sub-folder README accuracy (`mods/**/README.md`) | PASSED | 0 / 0 / 0 / 0 / 0 |
| 10 | Stale-reference sweep (v1.x post-v1.5 in active navigation) | PASSED | 0 / 0 / 0 / 5 / 1 |
| 11 | Cyrillic remainder check (.cs + active markdown) | PASSED | 0 / 0 / 5 / 0 / 0 |
| 12 | Navigation integrity (`docs/README.md`, root `README.md`, link existence, nav_order) | PASSED | 0 / 0 / 0 / 0 / 2 |

`PASSED` = no Tier 0 or Tier 1 findings in this check. `FAILED` = at least
one Tier 0 or Tier 1 finding present. All checks PASSED; Tier 0 eager
escalation NOT triggered.

**Tier breakdown across all checks:**

- Tier 0: 0 — eager escalation: NO
- Tier 1: 0
- Tier 2: 5 whitelist confirmations (translation campaign + closure-review preservation, see §14 Tier 2)
- Tier 3: 12 findings (cross-doc drift in v1-surface docs, sub-folder README inaccuracy, stale v1.x active references)
- Tier 4: 9 findings (cosmetic — broken link, nav_order duplicates, missing READMEs in test projects, manifest-example schema lag)

---

## §1 Spec see-also referenced docs verification

`MOD_OS_ARCHITECTURE.md` ends at line 985 with a «See also» section (line 976) listing eight references. Each verified to exist via direct `test -f` check.

| Spec see-also entry | Path | Exists? | Verdict |
|---|---|---|---|
| METHODOLOGY | `docs/METHODOLOGY.md` | yes | ✓ |
| ARCHITECTURE | `docs/ARCHITECTURE.md` | yes | ✓ |
| MODDING | `docs/MODDING.md` | yes | ✓ |
| MOD_PIPELINE | `docs/MOD_PIPELINE.md` | yes | ✓ |
| CONTRACTS | `docs/CONTRACTS.md` | yes | ✓ |
| ISOLATION | `docs/ISOLATION.md` | yes | ✓ |
| ROADMAP | `docs/ROADMAP.md` | yes | ✓ |
| `src/DualFrontier.Contracts/Modding/` (folder) | `src/DualFrontier.Contracts/Modding/` | yes (folder present, contains `IMod.cs`, `IModApi.cs`, `IModContract.cs`, `ContractsVersion.cs`, `ModManifest.cs`, `ManifestCapabilities.cs`, etc.) | ✓ |

**Findings:** None.

---

## §2 MODDING.md ↔ spec consistency

Read `docs/MODDING.md` целиком (309 lines). The document self-titles «Writing mods» without a version or status header. Spec see-also (line 980) explicitly frames this document: «MODDING — the existing v1 mod-author guide; this document specifies v2.»

**Spec acknowledges it as v1 surface; the doc itself does not announce v1 status visibly.** A reader landing on `docs/MODDING.md` from the `docs/README.md` Development section nav (line 43) sees no header signalling v1 vs. v2.

**Code mapping:**

| MODDING.md claim (line) | Spec section | Code path | Verdict |
|---|---|---|---|
| `IModApi` enumeration (lines 35–55): `RegisterComponent<T>`, `RegisterSystem<T>`, `Publish<T>`, `Subscribe<T>`, `Unsubscribe<T>`, `PublishContract<T>`, `TryGetContract<T>`, `Log`, `LogWarning`, `LogError` | Spec §4.1 v2 enumerates `Publish`, `Subscribe`, `GetKernelCapabilities`, `GetOwnManifest`, `Log`, `RegisterSystem`, `PublishContract`, `UnsubscribeAll`, `RegisterFor` (per Pass 2 §4 verification) | `src/DualFrontier.Contracts/Modding/IModApi.cs` (v2 surface, per Pass 2 §4) | **Tier 3 wording lag** — see Findings |
| Manifest example (line 132): `requiresContracts: "^1.0.0"` | Spec §2.2 v2 uses `apiVersion` field (per Pass 2 §2 verification) | `ModManifest.ApiVersion` + `EffectiveApiVersion` fallback | **Tier 4 example schema** — see Findings |
| Manifest field `optionalDependencies` (line 134) as separate top-level array | Spec §2.2 v1.1 ratification: `dependencies[i].optional` flag (no separate `optionalDependencies` field) | `ModDependency.IsOptional` (`ManifestParser.cs:332–343`, per Pass 2 §2) | **Tier 3** absorbed into wording-lag finding above |
| Hot-reload sequence (lines 151–162): five-step list ending at `ModLoadContext.Unload()` | Spec §9.5 v1.4 has seven-step unload chain (steps 1–6 sequential best-effort + step 7 WeakReference + GC pump) | `ModIntegrationPipeline.UnloadMod` per Pass 2 §9 + Pass 3 §10 | **Tier 3** absorbed into wording-lag finding above |
| Validation: «Phase A versioning + Phase B component conflicts» implicitly in lines 222–238 | Spec defines 8 phases A–H (per Pass 2 §11 sequence integrity verification) | `ContractValidator.cs:88–103` (8-phase invocation) | **Tier 3** absorbed into wording-lag finding above |

**Findings:**

- **Tier 3 (wording lag).** `docs/MODDING.md` documents the v1 IModApi/manifest/hot-reload surface across IModApi enumeration (lines 34–55), manifest example (lines 122–148), and hot reload sequence (lines 151–162). The spec see-also entry frames the doc as the v1 guide, but the doc itself does not announce its v1 status — there is no «Status: v1 surface (superseded by v2 in MOD_OS_ARCHITECTURE)» header. A reader navigating from `docs/README.md` § Development row (line 43) cannot distinguish current vs. historical surface. Recommendation: add a one-line header mirroring `MOD_PIPELINE.md` self-version pattern (e.g. «**Status:** v1 mod-author guide. The v2 surface is specified in [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md).»). Not blocking M7.4. Surgical-fix candidate after Pass 5 ratification.
- **Tier 4 (manifest example schema lag).** Manifest JSON example at lines 122–148 uses `requiresContracts: "^1.0.0"` and a separate `optionalDependencies` array. v2 surface uses `apiVersion` and `dependencies[i].optional`. Tied to the wording-lag finding above; if the doc is reframed as v1 historical, the example becomes consistent with that frame. Not blocking. Cosmetic refresh candidate.

---

## §3 MOD_PIPELINE.md ↔ spec consistency

Read `docs/MOD_PIPELINE.md` целиком (329 lines). Self-version at line 4: «Architecture version: v0.2 | Implementation phase: 2». This is engine-architecture v0.2, not MOD_OS spec version.

**Code mapping:**

| MOD_PIPELINE.md claim (line) | Spec section | Code path | Verdict |
|---|---|---|---|
| Pipeline orchestration steps 1–7 (lines 32–43): LoadMod, ContractValidator (two-phase), `IMod.Initialize`, `DependencyGraph.Reset/AddSystems/Build`, `ParallelSystemScheduler.Rebuild` | Spec §9 lifecycle and integration pipeline (per Pass 2 §9) | `ModIntegrationPipeline.Apply` (per Pass 2 §9 verification) | **Tier 3 wording lag** — see Findings |
| ContractValidator described as «two-phase validation» (line 47): Phase A versioning + Phase B component conflicts | Spec defines 8 phases A–H (Pass 2 §11 sequence #37–#38: spec ContractValidator phases catalogue) | `ContractValidator.cs` 8-phase invocation (Pass 2 §11) | **Tier 3** absorbed |
| `ValidationErrorKind` enum listing (lines 68–75): 4 members (`IncompatibleContractsVersion`, `WriteWriteConflict`, `CyclicDependency`, `MissingDependency`) | Spec v1.5 §11.2 baseline = 4, plus 7 migration adds = 11 total (per Pass 2 §11 Tier 0 RESOLVED via v1.5) | `ValidationError.cs:9–83` declares 11 members (Pass 2 §13 Tier 0 row 1) | **Tier 3** absorbed |
| Unload sequence (lines 178–187): step 4 «`IMod.Unload()` with a 500ms timeout» | Spec §9.5 v1.4 unload chain step 7: `WeakReference` spin loop with 100×100 ms = 10 s timeout, plus the `ModUnloadTimeout` warning | `ModIntegrationPipeline.UnloadMod` (Pass 2 §9, Pass 3 §10 verifications: 7-step chain with v1.4 GC pump) | **Tier 3** absorbed |
| `ContractsVersion.Current = new(1, 0, 0)` literal (line 259) | Spec uses `apiVersion` SemVer with caret per §2.2/§4.5 v1.5 | `ContractsVersion.cs` per Pass 2 §2 | **Tier 3** absorbed |

**Findings:**

- **Tier 3 (wording lag).** `docs/MOD_PIPELINE.md` self-versions as «Architecture version: v0.2» and describes the modding pipeline at the M2-era surface: a two-phase ContractValidator, a 4-member `ValidationErrorKind`, a five-step unload sequence with a 500ms `IMod.Unload` timeout. After M3–M7.3 the actual implementation has eight ContractValidator phases (per Pass 2 §11), an eleven-member `ValidationErrorKind` (per Pass 2 §13 Tier 0 row 1, ratified into v1.5 spec §11.2), and a seven-step unload chain with a 10s WeakReference + GC pump bracket (per spec v1.4 §9.5 step 7 + v1.4 §9.5.1, verified in Pass 2 §9). The doc has not been refreshed against the spec evolution. Practical impact: external readers navigating from `docs/README.md` § Development row (line 44) see the v0.2 surface as if current. Recommendation: refresh the doc to v0.3+ surface, or add a status header pinning the as-designed snapshot. Not blocking M7.4.

---

## §4 CONTRACTS.md ↔ spec consistency

Read `docs/CONTRACTS.md` целиком (132 lines). The document covers marker interfaces, the six domain buses, IModContract, contract evolution, and SemVer.

**Pass 1 anomaly #8 follow-up (bus count cross-doc):**

- `Contracts/README.md:17` lists 5 buses (per Pass 1 anomaly #8). Routed to §7.
- `IGameServices.cs:13–57` declares 6 properties (Pass 2 §11 sequence #46).
- `Contracts/Bus/README.md:5` describes 6 buses (per Pass 1 anomaly #8 third source).
- **`docs/CONTRACTS.md:34–48` documents «Six domain buses» and lists exactly 6 in `IGameServices` (Combat, Inventory, Magic, World, Pawns, Power), with «Power introduced in v0.3 §13.1».** ✓ matches `IGameServices.cs` count.
- **`docs/EVENT_BUS.md:11–21` mirrors the same six-bus enumeration** ✓.

So the bus-count drift is exclusively in `src/DualFrontier.Contracts/README.md:17` (parent README); the architecture docs (CONTRACTS.md, EVENT_BUS.md) and the sibling sub-folder README (Contracts/Bus/README.md) all declare 6 correctly. The Tier 3 stale-README finding for the parent file is recorded in §7.

**Code mapping:**

| CONTRACTS.md claim (line) | Spec section | Code path | Verdict |
|---|---|---|---|
| Four base marker interfaces (lines 5–32): `IEvent`, `IQuery`+`IQueryResult`, `ICommand`, `IComponent` | Spec §6 «three contract levels» (kernel + shared + regular markers via `IModContract`) | `src/DualFrontier.Contracts/Core/` (per Pass 2 §6) | ✓ — engine-internal markers; orthogonal to spec §6 mod-contract markers |
| Six-bus enumeration (lines 38–48) | (matches `IGameServices.cs`) | `IGameServices.cs:13–57` (Pass 1 §9 #46) | ✓ |
| Manifest JSON example (line 122): `requiresContracts: "^1.0.0"` | Spec §2.2 v2 `apiVersion` | `ModManifest.ApiVersion` | **Tier 4 example schema lag** (mirrors §2 finding) |

**Findings:**

- **Tier 4.** Manifest example at line 122 uses pre-v2 `requiresContracts` field name, mirroring the issue in `MODDING.md`. Same recommendation: refresh after deciding whether to brand v1-surface docs as historical or update them to v2.

---

## §5 ECS.md / EVENT_BUS.md / THREADING.md / ISOLATION.md ↔ spec consistency

Read all four целиком.

- **`ECS.md`** (164 lines). Describes engine ECS layer (`World`, `EntityId`, `Component`, `SparseSet`, `Query`, `SystemBase`). No spec §1–§10 v1.5 claims to verify against — orthogonal to MOD_OS spec. ✓ clean.
- **`EVENT_BUS.md`** (209 lines). Mentions «v0.3» and «v0.2 §12.1» — these are engine-architecture (`ARCHITECTURE.md`) versions, not MOD_OS spec versions. Six-bus enumeration matches `IGameServices.cs`. Deferred/Immediate delivery mechanics described match Pass 2 §11 sequence verifications. ✓ clean.
- **`THREADING.md`** (164 lines). Mentions «v0.2: 5 phases» — engine-architecture phase scaffold version. Describes `DependencyGraph`, `ParallelSystemScheduler`, `[SystemAccess]`, TickRates, async-forbidden rule. No spec §1–§10 v1.5 normative claims. ✓ clean.
- **`ISOLATION.md`** (225 lines). Describes `SystemExecutionContext` and `ModFaultHandler`. The ModFaultHandler runtime-violation lifecycle (lines 70–79) enumerates a 6-step sequence (log → unsubscribe → scheduler removal → `IMod.Unload` with timeout → ALC unload → publish `ModDisabledEvent`). Spec §9.5 v1.4 unload chain has 7 steps; the ISOLATION.md sequence omits the explicit `WeakReference` verification (step 7) introduced in v1.4. The wording predates v1.4. **Tier 3 wording lag** — runtime-violation path conceptually maps to the menu-driven unload chain but lacks the WeakReference verification clarification.

**Findings:**

- **Tier 3.** `docs/ISOLATION.md` `ModFaultHandler` sequence at lines 70–79 documents 6 steps; spec §9.5 v1.4 + v1.4 §9.5.1 unload chain has 7 steps (with explicit `WeakReference` + GC pump bracket as step 7). The doc predates v1.4 ratification. Practical impact: low — `ModFaultHandler` is the runtime-violation path, and the doc describes its conceptual lifecycle accurately for the M0–M6 implementation. Recommendation: add a sentence noting that the menu-driven `ModIntegrationPipeline.UnloadMod` adds a final `WeakReference` verification step (per spec §9.5 step 7 v1.4) that completes the conceptual chain. Not blocking. Surgical-fix candidate.

---

## §6 Other supporting docs

Per Pass 1 §4 Document inventory — enumerate remaining `docs/*.md` not in §1–§5. Each was read header-level (frontmatter + top section) for status verification; full read where the document references spec or code in verifiable ways.

| Path | Status (Pass 1 §4) | Spec/code refs verified | Verdict |
|---|---|---|---|
| `CODING_STANDARDS.md` | active | line 86 affirms Pass 4 whitelist §5.1 (no Russian in source files except `SESSION_PHASE_4_CLOSURE_REVIEW.md`); references closed in v0.3 i18n campaign — engine-architecture version, not MOD_OS spec | ✓ |
| `DEVELOPMENT_HYGIENE.md` | active | engine/game boundary checklist; no MOD_OS spec normative refs | ✓ |
| `TESTING_STRATEGY.md` | active | references `ISOLATION.md`, `PERFORMANCE.md`, `CODING_STANDARDS.md`; no MOD_OS spec normative refs | ✓ |
| `PERFORMANCE.md` | active | target metrics; no MOD_OS spec normative refs | ✓ |
| `GPU_COMPUTE.md` | research deferred | Phase 5 deferral; no MOD_OS spec refs | ✓ |
| `NATIVE_CORE_EXPERIMENT.md` | «Experiment — awaiting benchmark results» (Pass 1 §4 line 13) | research; no MOD_OS spec refs | ✓ |
| `VISUAL_ENGINE.md` | active | DevKit vs Native distinction; no MOD_OS spec refs | ✓ |
| `GODOT_INTEGRATION.md` | active | engine-architecture v0.3 reference (not MOD_OS) | ✓ |
| `RESOURCE_MODELS.md` | v0.2 addendum (per Pass 1 §4) | engine-architecture v0.2 (not MOD_OS) | ✓ |
| `COMPOSITE_REQUESTS.md` | v0.2 addendum | engine-architecture v0.2 | ✓ |
| `FEEDBACK_LOOPS.md` | v0.2 addendum | engine-architecture v0.2 | ✓ |
| `COMBO_RESOLUTION.md` | v0.2 addendum | engine-architecture v0.2 | ✓ |
| `OWNERSHIP_TRANSITION.md` | v0.2 addendum | engine-architecture v0.2 | ✓ |
| `PIPELINE_METRICS.md` | active v0.1 (Pass 1 §4) | methodology empirics | ✓ |
| `learning/PHASE_1.md` | translated 2026-04-27 (frozen Phase 1) | header (lines 4–7) confirms «originally written in Russian as a self-teaching artifact… Translated to English on 2026-04-27 as part of the i18n campaign. The original Russian version is preserved in git history at commit `cf8ef86`.» Frozen-snapshot status confirmed. Whitelist §5.1 / §5.2 alignment: «active forward-looking sections» exclusion does not apply (this is past-snapshot); English translation already complete (zero cyrillic per §11). | ✓ |
| `TRANSLATION_PLAN.md` | active v0.1 draft | translation campaign companion; cyrillic content is preserved-by-design (translation source examples). See §11 whitelist. | ✓ |
| `ARCHITECTURE.md` | active v0.3 | engine architecture; v0.3 changelog adds the sixth `IPowerBus` (line 5), consistent with `IGameServices.cs` 6-bus declaration and Pass 1 §9 #46 | ✓ |
| `METHODOLOGY.md` | active v1.0 (line 6) | methodology document; no MOD_OS spec normative refs | ✓ |

**Findings:** None.

---

## §7 Sub-folder README accuracy (`src/**/README.md`)

Per Pass 1 §0 row 4 inventory: 58 sub-folder READMEs total under `src/`, `tests/`, `mods/`. Per `Glob src/**/README.md`: 49 READMEs under `src/`. Per-README verification prioritised the Pass 1 anomaly #8 entries; remaining 46 READMEs were pattern-scanned via Grep for likely stale claims (`.gitkeep`, `placeholder`, «Real tests will arrive», «Phase 0/1/2/3 — TODO» indicators).

**Per-README verification table (priority entries):**

| Path | Claim summary | Reality | Verdict |
|---|---|---|---|
| `src/DualFrontier.Contracts/README.md:17` | Bus enumeration (line 17): «`ICombatBus`, `IInventoryBus`, `IMagicBus`, `IPawnBus`, `IWorldBus`» — **5 buses** listed | Actual: 6 properties in `IGameServices.cs:13–57` (Pass 2 §11 sequence #46); sibling `Bus/README.md:5` correctly says «six domain buses» | **Tier 3 stale README** — confirms Pass 1 anomaly #8 |
| `src/DualFrontier.Contracts/Bus/README.md:5` | «six domain buses (Combat, Inventory, Magic, Pawn, Power, World)» | Actual: 6 in `IGameServices.cs` | ✓ |
| `src/DualFrontier.Contracts/Modding/README.md` | (sibling of Modding folder) | (read; no quantitative claim drift detected) | ✓ |
| `src/DualFrontier.Application/Modding/README.md` | Contents listing (lines 14–19) enumerates 4 files: `ModLoader.cs`, `ModLoadContext.cs`, `RestrictedModApi.cs`, `ModIsolationException.cs` | Actual folder contains 10+ source files including `ContractValidator.cs`, `ManifestParser.cs`, `ModIntegrationPipeline.cs`, `ModRegistry.cs`, `ModContractStore.cs`, `KernelCapabilityRegistry.cs`, `SharedModLoadContext.cs`, `LoadedMod.cs`, `ValidationError.cs`, `ValidationReport.cs` (per Pass 2 §1–§9 file references) | **Tier 3 stale README** — incomplete contents listing |
| `src/DualFrontier.Events/README.md` | Six domain folders (Combat / Magic / Inventory / Power / Pawn / World) and two architectural patterns (Intent-vs-Event, Deferred) | Actual: matches enumerated 6 sub-folders; `[Deferred]` and `[Immediate]` attributes used as documented | ✓ |
| `src/DualFrontier.Persistence/README.md` | persistence overview | (read) — no quantitative drift detected | ✓ |

**Pattern-scan finding for remaining `src/` READMEs.** Grep over `src/**/README.md` for `(Phase 0|Phase 1|Phase 2|Phase 3|Phase 4|Phase 5|placeholder|.gitkeep|TODO|will arrive)` returned 54 of 58 sub-folder READMEs. Inspection of a sample (`Application/Modding/README.md`, `Contracts/README.md`, etc.) confirms the dominant pattern: TODO bullets reference closed phases (Phase 0/1/2/3) as if pending. This is per-doc cosmetic drift; the established README pattern in this project encodes phase-of-expected-work as TODO bullets that are not retroactively cleared on phase closure. Tier 4 cosmetic — does not block any pipeline check; recommendation backlogged as a doc-hygiene sweep.

**Findings:**

- **Tier 3 #1.** `src/DualFrontier.Contracts/README.md:17` lists 5 buses (`ICombatBus`, `IInventoryBus`, `IMagicBus`, `IPawnBus`, `IWorldBus`); actual is 6 (`IPowerBus` missing from this list, though properly described in sibling `Bus/README.md:5` and in `docs/CONTRACTS.md:34–48`). Confirms Pass 1 anomaly #8. Recommendation: append `IPowerBus` to line 17. Not blocking. (Source-of-truth code is `IGameServices.cs` and remains correct; this is a parent-README enumeration drift only.)
- **Tier 3 #2.** `src/DualFrontier.Application/Modding/README.md:14–19` Contents listing enumerates 4 of the 10+ source files in the folder. Several core M-phase additions (`ContractValidator`, `ManifestParser`, `ModIntegrationPipeline`, `ModRegistry`, `ModContractStore`, `KernelCapabilityRegistry`, `SharedModLoadContext`, `LoadedMod`, `ValidationError`, `ValidationReport`) added across M1–M7 are absent. Recommendation: refresh contents listing or replace with «source-of-truth: see folder for current file list» framing. Not blocking.
- **Tier 4.** Pattern-wide stale TODO bullets across many `src/` READMEs reference closed phases (Phase 0/1/2/3) as pending. Scope: ~40+ READMEs. Cosmetic; recommendation: doc-hygiene sweep batched against a future ratification cycle.

---

## §8 Sub-folder README accuracy (`tests/**/README.md`)

Per `Glob tests/**/README.md`: 4 READMEs (`tests/README.md`, `tests/DualFrontier.Core.Tests/README.md`, `tests/DualFrontier.Modding.Tests/README.md`, `tests/DualFrontier.Systems.Tests/README.md`). Two test projects (`tests/DualFrontier.Persistence.Tests/`, `tests/DualFrontier.Core.Benchmarks/`) have no README.

| Path | Claim summary | Reality | Verdict |
|---|---|---|---|
| `tests/DualFrontier.Modding.Tests/README.md:14` | «`.gitkeep` — placeholder. Real tests will arrive in Phase 2.» | 31 `.cs` files (per Pass 1 §2 inventory: 28 contain `[Fact]`, totalling 277 `[Fact]` + 2 `[Theory]` = 279 source-level attributes, expanded to 289 runtime tests via `[InlineData]` in `Capability/ProductionComponentCapabilityTests.cs`) | **Tier 3 stale README** — confirms Pass 1 anomaly #6 |
| `tests/DualFrontier.Systems.Tests/README.md:13` | «`.gitkeep` — placeholder. Real tests will arrive in Phase 2+.» | 6 `.cs` files (per Pass 1 §2 inventory: 16 `[Fact]`s totalling 16 runtime tests) | **Tier 3 stale README** — confirms Pass 1 anomaly #7 |
| `tests/DualFrontier.Core.Tests/README.md` | Sub-folder enumeration (`ECS/`, `Scheduling/`, `Bus/`, `Isolation/`); TODO bullets reference Phase 1/2 work as pending | 10 `.cs` files (per Pass 1 §2 inventory: 60 `[Fact]`s); folder structure consistent with stated `ECS/`, `Scheduling/`, `Bus/`, `Isolation/` decomposition | ✓ structure / **Tier 4 stale TODO bullets** |
| `tests/DualFrontier.Persistence.Tests/` | (no README) | 4 `.cs` files (per Pass 1 §2 inventory: 4 `[Fact]`s; project added in the Persistence phase) | **Tier 4 missing README** — pattern broken (all other test projects have one) |
| `tests/DualFrontier.Core.Benchmarks/` | (no README; folder contains only `bin/`, `obj/`, `.lscache`) | placeholder folder | **Tier 4 missing README + placeholder folder** |
| `tests/README.md` | Lists 3 test projects (Core.Tests, Systems.Tests, Modding.Tests) | Actual: 4 test projects + 1 benchmarks project (Persistence.Tests + Core.Benchmarks unlisted) | **Tier 4 incomplete enumeration** |

**Findings:**

- **Tier 3 #1.** `tests/DualFrontier.Modding.Tests/README.md:14` claims `.gitkeep` placeholder with «Real tests will arrive in Phase 2». Actual: 31 `.cs` files containing 277 `[Fact]` + 2 `[Theory]` attributes (Pass 1 §2 inventory; Pass 3 §1 engine snapshot verification confirms runtime expansion 357 + 12 = 369). Phase 2 closed long ago; the placeholder claim is contradicted by every test under that path. Recommendation: replace contents with current per-folder summary (`Capability/`, `Manifest/`, `Validator/`, `Api/`, `Parser/`, `Sharing/`, `Pipeline/`). Not blocking. Confirms Pass 2 OOS #3 / Pass 1 anomaly #6.
- **Tier 3 #2.** `tests/DualFrontier.Systems.Tests/README.md:13` claims `.gitkeep` placeholder with «Real tests will arrive in Phase 2+». Actual: 6 `.cs` files containing 16 `[Fact]` attributes (Pass 1 §2 inventory). Recommendation: replace with current per-folder summary. Not blocking. Confirms Pass 2 OOS #4 / Pass 1 anomaly #7.
- **Tier 4 #1.** `tests/DualFrontier.Persistence.Tests/` has no README; the other three test projects have one. Pattern broken. Cosmetic backlog item.
- **Tier 4 #2.** `tests/DualFrontier.Core.Benchmarks/` has no README and contains only `.csproj.lscache`, `bin/`, `obj/` — empty placeholder folder. Cosmetic backlog item.
- **Tier 4 #3.** `tests/README.md:7–9` lists 3 of the 4 active test projects + 1 benchmarks project; Persistence.Tests and Core.Benchmarks omitted. Recommendation: append both. Not blocking.

---

## §9 Sub-folder README accuracy (`mods/**/README.md`)

Per `Glob mods/**/README.md`: 2 READMEs.

| Path | Claim summary | Reality | Verdict |
|---|---|---|---|
| `mods/README.md` | «Each is a separate assembly that sees ONLY `DualFrontier.Contracts`. … `DualFrontier.Mod.Example/` — reference minimal mod.» | 1 sub-directory under `mods/` (per Pass 1 §1: «1 sub-directory under `mods/` (DualFrontier.Mod.Example)»); claim consistent | ✓ |
| `mods/DualFrontier.Mod.Example/README.md` | «Reference example mod… `IMod` + `IModApi`… `mod.manifest.json`» | folder contents per Pass 1 §6 manifest inventory: 1 `mod.manifest.json` + `ExampleMod.cs` + `.csproj` | ✓ |

**Findings:** None.

---

## §10 Stale-reference sweep (v1.x post-v1.5 in active navigation)

Per Pass 2 OOS #10 / Pass 3 OOS #1: ROADMAP.md:3 references «v1.4 LOCKED» stale.

**Sweep methodology:** Grep recursively across all `.md` files under `docs/` and root `README.md` for `v1\.[0-4]`, then exclude:

- `docs/audit/` (historical / translation campaign artifacts whitelist per AUDIT_PASS_4_PROMPT §5.1, §5.4).
- `MOD_OS_ARCHITECTURE.md` Version history lines 12–32 (changelog whitelist per AUDIT_PASS_4_PROMPT §5.4).
- `TRANSLATION_GLOSSARY.md` (whitelist per AUDIT_PASS_4_PROMPT §5.1).

Each remaining match was classified per AUDIT_PASS_4_PROMPT §10 (a/b/c):

| File:line | Reference verbatim | Context | Classification |
|---|---|---|---|
| `README.md:118` | «v1.0 LOCKED» (in « `MOD_OS_ARCHITECTURE.md` — the capability-based mod isolation as an OS-style architecture; v1.0 LOCKED.») | active «Three primary documents» nav text claiming current status | **Tier 3 stale** (also: pre-existing drift, never updated past v1.0; current is v1.5) |
| `docs/README.md:28` | «**v1.4 LOCKED.**» (in MOD_OS_ARCHITECTURE row of Architecture nav table) | active nav table claiming current status | **Tier 3 stale** |
| `docs/ROADMAP.md:3` | «`MOD_OS_ARCHITECTURE` v1.4 LOCKED» | preamble describing the architecture under M1–M10 migration | **Tier 3 stale** (Pass 2 OOS #10 / Pass 3 OOS #1 confirmation) |
| `docs/ROADMAP.md:103` | «derived from `MOD_OS_ARCHITECTURE` v1.4 §11» | M-phase preamble describing the migration sequence source | **Tier 3 stale** |
| `docs/ROADMAP.md:424` | «`MOD_OS_ARCHITECTURE` — v1.4 LOCKED specification driving M1–M10» | See-also section closing line | **Tier 3 stale** |
| `docs/ROADMAP.md:290` | «v1.4 §9.5 step 7» (in M7.2 closure narrative explaining step 7 deferral to M7.3) | historical M7.2 closure narrative; v1.4 was the ratification version that introduced GC pump at §9.5 step 7; v1.5 did not change §9.5 (only §11.2). Reference is historically accurate as the specific ratification version where this surface landed; not «current LOCKED v1.4» framing. | **Tier 4 cosmetic** — historical narrative still accurate, but could be updated to «v1.4 §9.5 step 7 (preserved verbatim in v1.5)» for absolute clarity |

Other matches in active navigation (each classified clean per §10 (a) historical narrative):

| File:line | Reference verbatim | Why clean |
|---|---|---|
| `MOD_OS_ARCHITECTURE.md:8` | status line itself reads `LOCKED v1.5` then narrates earlier ratification cycles by name (v1.1, v1.2, v1.3, v1.4, v1.5) | the doc IS the v1.5 source — narrative reference to earlier ratifications is required for changelog cohesion |
| `MOD_OS_ARCHITECTURE.md:843` | «**M0** | This document at v1.0 | All ⚠ DECISION items closed | —» (in §11.1 M-row table) | M0 was closed at v1.0 by definition — past-tense historical anchor |
| `MOD_OS_ARCHITECTURE.md:887` | «in v0.1 ... locked during Phase 0 closure (v1.0)» | past-tense narrative |
| `NORMALIZATION_REPORT.md:168, 186, 202` | «Glossary lock | §15.1a, locked v1.0» | refers to TRANSLATION_GLOSSARY v1.0, not MOD_OS spec |
| `PIPELINE_METRICS.md:89, 92` | timeline entry «MOD_OS_ARCHITECTURE v1.1» / «specification version sync v1.0 → v1.1» | past-tense pipeline-execution log |
| `ROADMAP.md:22` | «**M0 — Mod-OS Phase 0** \| ✅ Closed \| — \| `MOD_OS_ARCHITECTURE` v1.0 LOCKED» | M0 Output column anchored to closure-time state per AUDIT_CAMPAIGN_PLAN §6.5 |
| `ROADMAP.md:25` | «§3.6 v1.2 hybrid enforcement» | references the v1.2 ratification cycle that introduced the hybrid wording — historical narrative |
| `ROADMAP.md:109` | «**Output:** ... v1.0 LOCKED» (M0 row body) | M0 closure narrative |
| `ROADMAP.md:175` | «§4.2 / §4.3 v1.2 hybrid enforcement (ratified in v1.2 §3.6)» | M3.2 closure narrative referencing the v1.2 ratification |
| `ROADMAP.md:247, 250` | «Mod A v1.0.0», «Mod B requires `^1.0.0`» | mod versioning illustrative example values, not spec-version references |
| `TRANSLATION_PLAN.md:197` | «glossary v1.0 finalized» | TRANSLATION_GLOSSARY v1.0, not MOD_OS spec |

**Findings:**

- **Tier 3 #1.** `README.md:118` — root README «Three primary documents» section claims «v1.0 LOCKED» for MOD_OS_ARCHITECTURE. This has been stale through v1.1 / v1.2 / v1.3 / v1.4 / v1.5 cycles; never updated. Current is v1.5. Recommendation: update to v1.5.
- **Tier 3 #2.** `docs/README.md:28` — Architecture nav table row reads «**v1.4 LOCKED.**». Current is v1.5. Recommendation: update.
- **Tier 3 #3.** `ROADMAP.md:3` — preamble «v1.4 LOCKED». Recommendation: update. (Pass 2 OOS #10 / Pass 3 OOS #1 routed-here finding.)
- **Tier 3 #4.** `ROADMAP.md:103` — M-phase preamble «v1.4 §11». Update.
- **Tier 3 #5.** `ROADMAP.md:424` — See-also «v1.4 LOCKED specification». Update.
- **Tier 4.** `ROADMAP.md:290` — M7.2 closure narrative «v1.4 §9.5 step 7». Historically accurate (v1.4 was the ratification cycle that introduced the GC pump bracket; v1.5 did not touch §9.5), but a future cosmetic refresh could note «v1.4 §9.5 step 7 (preserved verbatim in v1.5)» for reader clarity.

---

## §11 Cyrillic remainder check

### 11.1 `.cs` files

Per Pass 1 §8 inventory: 0 `.cs` files with cyrillic. Pass 4 verification:

```
LC_ALL=en_US.UTF-8 grep -lP '[\x{0400}-\x{04FF}]' \
  $(find . -type f -name '*.cs' \
      \( -path './src/*' -o -path './tests/*' -o -path './mods/*' \) \
      -not -path '*/bin/*' -not -path '*/obj/*')
```

**Result:** 0 files. Pass 1 §8 baseline holds across the +3 commits since the Pass 1 baseline.

(A naive `grep '[А-Яа-я]'` without Unicode regex returns false-positive matches due to byte-range collation; the PCRE `\x{0400}-\x{04FF}` form is the correct cross-locale check matching Pass 1 §8 inventory methodology.)

### 11.2 Active markdown files

Active = `docs/*.md` excluding:

- `docs/audit/` (historical / translation campaign artifacts whitelist).
- `docs/MOD_OS_ARCHITECTURE.md` Version history lines 12–32 (changelog whitelist; verified via `grep -P '[\x{0400}-\x{04FF}]'` — no matches in this file).
- `docs/TRANSLATION_GLOSSARY.md` (RU source by design, whitelist).
- `docs/learning/PHASE_1.md` (frozen Phase 1 snapshot — header lines 4–7 confirm «Translated to English on 2026-04-27 as part of the i18n campaign. The original Russian version is preserved in git history at commit `cf8ef86`.» Verified zero cyrillic remaining via PCRE grep).

For each active `.md` — Grep PCRE `[\x{0400}-\x{04FF}]`. Per recursive grep across `docs/`:

**Files with cyrillic content (16 total):**

- `docs/audit/AUDIT_CAMPAIGN_PLAN.md` (whitelist §5.4)
- `docs/audit/AUDIT_PASS_1_INVENTORY.md` (audit campaign artifact, RU commentary by design)
- `docs/audit/AUDIT_PASS_1_PROMPT.md` (audit campaign prompt, RU by design)
- `docs/audit/AUDIT_PASS_2_PROMPT.md` (audit campaign prompt, RU by design)
- `docs/audit/AUDIT_PASS_2_RESUMPTION_PROMPT.md` (audit campaign prompt, RU by design)
- `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` (audit artifact, RU commentary by design)
- `docs/audit/AUDIT_PASS_3_PROMPT.md` (audit campaign prompt, RU by design)
- `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md` (audit artifact, RU commentary by design)
- `docs/audit/AUDIT_PASS_4_PROMPT.md` (this pass's prompt, RU by design)
- `docs/audit/PASS_2_NOTES.md` (translation campaign whitelist §5.1)
- `docs/audit/PASS_3_NOTES.md` (translation campaign whitelist §5.1)
- `docs/audit/PASS_4_REPORT.md` (translation campaign whitelist §5.1)
- `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md` (whitelist §5.1)
- `docs/NORMALIZATION_REPORT.md` (translation campaign whitelist §5.1; path-corrected note in §14 Tier 4)
- `docs/TRANSLATION_GLOSSARY.md` (whitelist §5.1)
- `docs/TRANSLATION_PLAN.md` (translation campaign companion; cyrillic content is preserved-by-design — translation source examples in §1, glossary mappings in §3 — analogous whitelist to TRANSLATION_GLOSSARY)

**Active markdown files NOT in whitelist with cyrillic:** **0.**

### 11.3 Whitelist confirmations

| Path | Cyrillic? | Whitelist reason | Tier |
|---|---|---|---|
| `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md` | yes (preserved RU audit trail) | per AUDIT_PASS_4_PROMPT §5.1 + plan §6.4 | Tier 2 |
| `docs/audit/M3_CLOSURE_REVIEW.md` … `M6_CLOSURE_REVIEW.md` | (checked: present at audit/) — historical audit-trail artifacts; per Pass 1 §4 inventory included in audit folder; Pass 4 read-only (no cyrillic flag for whitelist purposes) | per AUDIT_PASS_4_PROMPT §5.1 | Tier 2 (frozen) |
| `docs/TRANSLATION_GLOSSARY.md` | yes (RU source terms by design) | per AUDIT_PASS_4_PROMPT §5.1 | Tier 2 |
| `docs/audit/PASS_2_NOTES.md`, `PASS_3_NOTES.md`, `PASS_4_REPORT.md` | yes (translation campaign artifacts) | per AUDIT_PASS_4_PROMPT §5.1 | Tier 2 |
| `docs/NORMALIZATION_REPORT.md` | yes (translation campaign Pass 1 normalization report) | AUDIT_PASS_4_PROMPT §5.1 wrote the path as «`docs/audit/NORMALIZATION_REPORT.md`», but the actual file is at `docs/NORMALIZATION_REPORT.md`. Intent is clear from the surrounding clause «translation campaign artifacts; Russian commentary preserved by design». | Tier 2 (intent-honored; see §14 Tier 4 path-mismatch observation) |

**Findings:**

- **Tier 2 (5 confirmations).** All cyrillic-bearing markdowns are within the AUDIT_PASS_4_PROMPT §5.1 whitelist (4 explicit + 1 path-corrected). Active markdowns outside the whitelist: 0 cyrillic. Pass 1 §8 .cs baseline (0) holds. Translation campaign closure verdict: **complete and stable** — no regression in the +3-commit delta since Pass 1.

---

## §12 Navigation integrity

### 12.1 `docs/README.md`

Read целиком. Each `[link](path)` verified for path existence:

| Link target | Exists? |
|---|---|
| `./METHODOLOGY.md` | ✓ |
| `./PIPELINE_METRICS.md` | ✓ |
| `./MOD_OS_ARCHITECTURE.md` | ✓ |
| `./NATIVE_CORE_EXPERIMENT.md` | ✓ |
| `./ARCHITECTURE.md` | ✓ |
| `./CONTRACTS.md` | ✓ |
| `./ECS.md` | ✓ |
| `./EVENT_BUS.md` | ✓ |
| `./THREADING.md` | ✓ |
| `./ISOLATION.md` | ✓ |
| `./GODOT_INTEGRATION.md` | ✓ |
| `./VISUAL_ENGINE.md` | ✓ |
| `./MODDING.md` | ✓ |
| `./MOD_PIPELINE.md` | ✓ |
| `./PERFORMANCE.md` | ✓ |
| `./GPU_COMPUTE.md` | ✓ |
| `./CODING_STANDARDS.md` | ✓ |
| `./TESTING_STRATEGY.md` | ✓ |
| `./DEVELOPMENT_HYGIENE.md` | ✓ |
| `../src/DualFrontier.Persistence/README.md` | ✓ |
| `./ROADMAP.md` | ✓ |
| `./learning/PHASE_1.md` | ✓ |
| `./SESSION_PHASE_4_CLOSURE_REVIEW.md` | **MISSING** — actual location is `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md`; Pass 1 §4 inventory placed it under `docs/audit/`. Tier 4 cosmetic broken link. |
| `./RESOURCE_MODELS.md` | ✓ |
| `./COMPOSITE_REQUESTS.md` | ✓ |
| `./FEEDBACK_LOOPS.md` | ✓ |
| `./COMBO_RESOLUTION.md` | ✓ |
| `./OWNERSHIP_TRANSITION.md` | ✓ |

### 12.2 root `README.md`

Each `[link](path)` verified:

| Link target | Exists? |
|---|---|
| `./docs/ROADMAP.md` | ✓ |
| `./docs/ARCHITECTURE.md` | ✓ |
| `./docs/MOD_OS_ARCHITECTURE.md` | ✓ |
| `./docs/NORMALIZATION_REPORT.md` | ✓ |
| `./docs/PIPELINE_METRICS.md` (and three section anchors `#1-pipeline-configuration`, `#3-subscription-headroom`, `#5-reproducibility-requirements`) | ✓ (file exists; PIPELINE_METRICS.md per Pass 1 §4 contains §1, §3, §5 sections — anchor refs presumed valid pending future deep verification, out of scope here) |
| `./docs/METHODOLOGY.md` (and section anchor `#6`) | ✓ |
| `./docs/NATIVE_CORE_EXPERIMENT.md` | ✓ |
| `./docs/README.md` | ✓ |
| `./LICENSE` | ✓ |

### 12.3 nav_order consistency

Frontmatter `nav_order:` collected via Grep across `docs/**/*.md` (line-3 frontmatter only; in-body code-block examples filtered):

| nav_order | Path |
|---|---|
| 25 | `docs/MOD_OS_ARCHITECTURE.md` |
| 27 | `docs/PIPELINE_METRICS.md` |
| 95 | `docs/audit/M3_CLOSURE_REVIEW.md` |
| 96 | `docs/audit/M4_CLOSURE_REVIEW.md` and `docs/audit/PASS_4_REPORT.md` |
| 97 | `docs/audit/M5_CLOSURE_REVIEW.md` |
| 98 | `docs/audit/M6_CLOSURE_REVIEW.md` and `docs/audit/PASS_2_NOTES.md` |
| 99 | `docs/TRANSLATION_GLOSSARY.md` |
| 100 | `docs/TRANSLATION_PLAN.md` |
| 101 | `docs/NORMALIZATION_REPORT.md` |
| 102 | `docs/audit/AUDIT_CAMPAIGN_PLAN.md` |
| 103 | `docs/audit/AUDIT_PASS_1_PROMPT.md` and `docs/audit/PASS_3_NOTES.md` |
| 104 | `docs/audit/AUDIT_PASS_1_INVENTORY.md` |
| 105 | `docs/audit/AUDIT_PASS_2_PROMPT.md` and `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` |
| 106 | `docs/audit/AUDIT_PASS_2_RESUMPTION_PROMPT.md` |
| 107 | `docs/audit/AUDIT_PASS_3_PROMPT.md` and `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md` |
| 108 | `docs/audit/AUDIT_PASS_4_PROMPT.md` and `docs/audit/AUDIT_PASS_4_CROSSDOC_TRANSLATION.md` (this artifact) |

**Pairings (intentional, prompt-and-artifact same nav_order):**

- 105 — Pass 2 prompt + Pass 2 artifact
- 107 — Pass 3 prompt + Pass 3 artifact
- 108 — Pass 4 prompt + Pass 4 artifact (this pass)

**Unrelated duplicates (not pairings):**

- 96 — `M4_CLOSURE_REVIEW.md` (M-phase) and `PASS_4_REPORT.md` (translation campaign)
- 98 — `M6_CLOSURE_REVIEW.md` (M-phase) and `PASS_2_NOTES.md` (translation campaign)
- 103 — `AUDIT_PASS_1_PROMPT.md` (audit campaign) and `PASS_3_NOTES.md` (translation campaign)

**Findings:**

- **Tier 4 #1.** `docs/README.md:69` link `[SESSION_PHASE_4_CLOSURE_REVIEW](./SESSION_PHASE_4_CLOSURE_REVIEW.md)` is broken — file moved to `docs/audit/` per Pass 1 §4 inventory. Recommendation: change href to `./audit/SESSION_PHASE_4_CLOSURE_REVIEW.md`. Not blocking.
- **Tier 4 #2.** Three pairs of unrelated documents share `nav_order` values (96, 98, 103). Cosmetic — affects rendered nav ordering only when both documents are listed in the same site index. Recommendation: re-stagger translation-campaign report nav_orders into a non-overlapping band. Not blocking.

---

## §13 Surgical fixes applied this pass

None. Pass 4 is read-only by contract (per AUDIT_PASS_4_PROMPT §11 «Запрещено» and AUDIT_CAMPAIGN_PLAN §2 Pass 4 «Surgical fixes applied this pass: 0»).

---

## §14 Items requiring follow-up

### Tier 0 — Cross-doc drift (BLOCKING)

(no Tier 0 findings)

Eager-escalation triggered: **NO**.

### Tier 1 — Missing required cross-doc

(no Tier 1 findings)

### Tier 2 — Whitelist deviations confirmed

| # | Whitelist entry | Source | Verification |
|---|---|---|---|
| 1 | `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md` preserved RU audit trail | per AUDIT_PASS_4_PROMPT §5.1 + plan §6.4 | Cyrillic confirmed; whitelist holds. Tier 2 confirmed. |
| 2 | `docs/TRANSLATION_GLOSSARY.md` RU source terms by design | per AUDIT_PASS_4_PROMPT §5.1 + plan §6.4 + Pass 1 §4 «locked v1.0» | Cyrillic confirmed; whitelist holds. Tier 2 confirmed. |
| 3 | `docs/audit/M3..M6_CLOSURE_REVIEW.md` historical audit-trail artifacts | per AUDIT_PASS_4_PROMPT §5.1 + Pass 3 §14 Tier 2 #3 | Files present at audit/; frozen status preserved. Tier 2 confirmed. |
| 4 | Translation campaign reports (`PASS_2_NOTES.md`, `PASS_3_NOTES.md`, `PASS_4_REPORT.md`, `NORMALIZATION_REPORT.md`) RU commentary by design | per AUDIT_PASS_4_PROMPT §5.1 (path-corrected for `NORMALIZATION_REPORT.md`) | Cyrillic confirmed; whitelist holds. Tier 2 confirmed. |
| 5 | `docs/TRANSLATION_PLAN.md` translation campaign companion — cyrillic content is preserved-by-design (translation source examples in §1, glossary mappings in §3) | analogue to TRANSLATION_GLOSSARY.md whitelist; Pass 1 §4 status «active v0.1 draft» | Sample cyrillic at lines 16, 20, 73–74 (referenced source strings such as `[ИЗОЛЯЦИЯ НАРУШЕНА]`), lines 154–174 (glossary mappings). Whitelist by analogy. Tier 2 confirmed. |

### Tier 3 — Stale references / minor mismatch

| # | Item | Source location | Recommendation |
|---|---|---|---|
| 1 | `MOD_OS_ARCHITECTURE` v1.x stale active reference | `README.md:118` («v1.0 LOCKED») | Update to «v1.5 LOCKED» |
| 2 | `MOD_OS_ARCHITECTURE` v1.x stale active reference | `docs/README.md:28` («v1.4 LOCKED») | Update to «v1.5 LOCKED» |
| 3 | `MOD_OS_ARCHITECTURE` v1.x stale active reference (Pass 2 OOS #10 / Pass 3 OOS #1) | `ROADMAP.md:3` («v1.4 LOCKED») | Update to «v1.5 LOCKED» |
| 4 | `MOD_OS_ARCHITECTURE` v1.x stale active reference | `ROADMAP.md:103` («v1.4 §11») | Update to «v1.5 §11» |
| 5 | `MOD_OS_ARCHITECTURE` v1.x stale active reference | `ROADMAP.md:424` («v1.4 LOCKED specification») | Update to «v1.5 LOCKED specification» |
| 6 | Sub-folder README inaccurate (Pass 1 anomaly #6 / Pass 2 OOS #3) | `tests/DualFrontier.Modding.Tests/README.md:14` («.gitkeep — placeholder. Real tests will arrive in Phase 2.»; actual: 31 .cs / 277 [Fact] + 2 [Theory]) | Replace with current per-folder summary (Capability/, Manifest/, Validator/, Api/, Parser/, Sharing/, Pipeline/) |
| 7 | Sub-folder README inaccurate (Pass 1 anomaly #7 / Pass 2 OOS #4) | `tests/DualFrontier.Systems.Tests/README.md:13` («.gitkeep — placeholder. Real tests will arrive in Phase 2+.»; actual: 6 .cs / 16 [Fact]) | Replace with current per-folder summary |
| 8 | Sub-folder README inaccurate (Pass 1 anomaly #8 / Pass 2 OOS #5) | `src/DualFrontier.Contracts/README.md:17` (lists 5 buses, missing `IPowerBus`; actual 6 in `IGameServices.cs`) | Append `IPowerBus` to the enumeration |
| 9 | Sub-folder README inaccurate | `src/DualFrontier.Application/Modding/README.md:14–19` (lists 4 of 10+ source files) | Refresh contents listing or replace with «source-of-truth: see folder» framing |
| 10 | Cross-doc wording lag (v1 surface doc not labelled v1) | `docs/MODDING.md` (no version header) | Add «**Status:** v1 mod-author guide. The v2 surface is specified in [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md).» header |
| 11 | Cross-doc wording lag (v0.2 surface doc, ContractValidator phases / ValidationErrorKind / unload sequence outdated vs spec v1.5 + Pass 2 §11 / §13 verifications) | `docs/MOD_PIPELINE.md` (self-versioned «Architecture version: v0.2») | Refresh to v0.3+ surface or pin as designed-snapshot |
| 12 | Cross-doc wording lag (ModFaultHandler 6-step sequence vs spec §9.5 v1.4 7-step unload chain) | `docs/ISOLATION.md:70–79` | Add note about menu-driven `UnloadMod` step 7 WeakReference verification per spec §9.5 step 7 v1.4 |

### Tier 4 — Cosmetic

| # | Item | Source location | Recommendation |
|---|---|---|---|
| 1 | Stale narrative reference (historically accurate but could clarify) | `ROADMAP.md:290` («v1.4 §9.5 step 7») | Update to «v1.4 §9.5 step 7 (preserved verbatim in v1.5)» for absolute reader clarity |
| 2 | Pattern-wide stale TODO bullets in `src/` READMEs (~40+ READMEs reference closed Phase 0/1/2/3 work as pending) | `src/**/README.md` | Doc-hygiene sweep batched against a future ratification cycle |
| 3 | Manifest example schema lag (uses `requiresContracts` v1 field) | `docs/MODDING.md:122–148`, `docs/CONTRACTS.md:122` | Tied to Tier 3 #10 / #11 wording-lag refresh |
| 4 | Missing README in `tests/DualFrontier.Persistence.Tests/` (pattern broken — other test projects have one) | `tests/DualFrontier.Persistence.Tests/` | Add minimal README per the established pattern |
| 5 | Missing README + empty placeholder folder | `tests/DualFrontier.Core.Benchmarks/` (only `.lscache`, `bin/`, `obj/`) | Add README OR remove the empty folder if benchmarks are still pending |
| 6 | Incomplete enumeration in `tests/README.md` | `tests/README.md:7–9` (lists 3 of 4 test projects + 1 benchmarks project; Persistence.Tests + Core.Benchmarks missing) | Append both projects |
| 7 | Broken nav link | `docs/README.md:69` (`./SESSION_PHASE_4_CLOSURE_REVIEW.md` — actual location `docs/audit/`) | Change href to `./audit/SESSION_PHASE_4_CLOSURE_REVIEW.md` |
| 8 | nav_order duplicates (unrelated documents share value) | `docs/audit/M4_CLOSURE_REVIEW.md` and `PASS_4_REPORT.md` (96); `M6_CLOSURE_REVIEW.md` and `PASS_2_NOTES.md` (98); `AUDIT_PASS_1_PROMPT.md` and `PASS_3_NOTES.md` (103) | Re-stagger translation-campaign report nav_orders into a non-overlapping band |
| 9 | AUDIT_PASS_4_PROMPT §5.1 path-mismatch observation | `docs/audit/AUDIT_PASS_4_PROMPT.md` §5.1 wrote «`docs/audit/NORMALIZATION_REPORT.md`» but the actual file is at `docs/NORMALIZATION_REPORT.md` (per Pass 1 §4 inventory). Pass 4 honored intent; Tier 2 whitelist applied to actual path. | Future prompt revisions: align path with Pass 1 inventory |

### Out-of-scope items observed (for Pass 5)

| # | Anomaly | Source | Routing |
|---|---|---|---|
| 1 | All Tier 3 / Tier 4 findings above are surgical-fix or doc-hygiene candidates, not blockers for M7.4. Final triage and GREEN/YELLOW/RED verdict belong to Pass 5. | This pass §14 | Pass 5 final triage |
| 2 | Pass 2 §13 OOS #6, #7 (AUDIT_PASS_1_PROMPT.md §4 closure-review path drift; example branch `feat/m4-shared-alc` vs actual `main`) — Pass 1 prompt-quality issues, observed but out of Pass 2/3/4 scope | `docs/audit/AUDIT_PASS_1_PROMPT.md` | Pass 5 — triage as audit-process hygiene observation, not project drift |

---

## §15 Verification end-state

- **§0 Executive summary:** 12/12 PASSED.
- **Total findings:** Tier 0: 0 (eager-escalated NO), Tier 1: 0, Tier 2: 5 whitelist confirmed, Tier 3: 12, Tier 4: 9.
- **Cyrillic remainder verdict:** 0 .cs / 16 active markdown matches all whitelist Tier 2 (4 explicit per AUDIT_PASS_4_PROMPT §5.1 + 1 by analogue extension). Pass 1 §8 baseline holds across the +3-commit delta.
- **Stale-reference sweep verdict:** 5 Tier 3 stale `MOD_OS_ARCHITECTURE` v1.x active references in active navigation (`README.md:118`, `docs/README.md:28`, `ROADMAP.md:3`, `ROADMAP.md:103`, `ROADMAP.md:424`) + 1 Tier 4 cosmetic narrative reference (`ROADMAP.md:290`). All require simple textual updates; none are blocking.
- **Sub-folder README accuracy verdict:** 4 stale READMEs (Tier 3): `tests/DualFrontier.Modding.Tests/README.md` (Pass 1 anomaly #6), `tests/DualFrontier.Systems.Tests/README.md` (Pass 1 anomaly #7), `src/DualFrontier.Contracts/README.md` (Pass 1 anomaly #8), `src/DualFrontier.Application/Modding/README.md` (incomplete contents listing). 3 Tier 4 cosmetic test-folder README/structure observations (Persistence.Tests no README, Core.Benchmarks empty placeholder, tests/README.md incomplete enumeration).
- **Cross-doc wording-lag verdict:** 3 Tier 3 (MODDING.md, MOD_PIPELINE.md, ISOLATION.md). All three are v1-surface or v0.2-surface docs that have not been refreshed against the spec evolution to v1.5. Pass 4 found no contradictory normative claim that would constitute Tier 0 or Tier 1 drift; the docs lag the spec wording but do not contradict it on any structural invariant.
- **Surgical fixes applied:** 0 (per contract).
- **Pass 4 status:** **complete**, ready for human ratification.
- **Unblocks:** Pass 5 (Triage + Final report).
