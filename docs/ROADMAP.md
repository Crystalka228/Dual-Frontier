---
register_id: DOC-C-ROADMAP
project: Dual Frontier
category: C
tier: 2
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: 2026-Q4
title: Roadmap
supersedes:
- DOC-A-MIGRATION_PLAN
last_modified_commit: 9b4968a
review_cadence: on-closure+quarterly
last_review_date: 2026-07-17
last_review_event: "Standing-Law cascade C6+C8 (4a67a22 + 5ebc24b) — received the NEW «⏭ Analyzer track (A'.9.1 Phase β/γ/δ + deferred rule families)» section (analyzer forward-sequencing extracted from ANALYZER_RULES v0.1 §4.1-4.2/§5/§6/§10.5/§11 per the Standing-Law spec/roadmap separation; Phase β detection + Q-L-1 adaptive gate, Phase γ severity promotion, Phase δ governance, К-L20 LOCK rule family, hardware tier expansion, PublicApiAnalyzers activation conditions) + the NEW «Findings ledger (F-series)» section seeded F-1..F-12 (brief §13 seed + session findings F-10 pre-existing test failures / F-11 stale-branch pruning candidates / F-12 DFK019.A Phase-γ severity discrepancy). Prior context: DD-2 spec/roadmap separation increment 1 (2026-06-02) — K-series roadmap relocated from KERNEL Part 2/3 into «Native foundation tracks»; status-overview K-series + K9 rows retired to Closed; remaining M8/V status rows pending full DD-1 re-sync. docs/ROADMAP.md is the canonical roadmap-layer consolidation home per accepted Option ε target architecture."
reviewer: Crystalka
---

# Roadmap

The Dual Frontier implementation has reorganised after the closure of Phase 4. The original Phase 5 (Combat), Phase 6 (Magic), and Phase 7 (World) are dissolved into a broader **Mod-OS Migration** (M1–M10) that simultaneously builds the modding kernel and ships gameplay content as vanilla mods. The architecture for this migration is specified in [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) (LOCKED; the register owns its version); this roadmap is the execution sequence derived from it.

The reorganisation follows the project's central methodological claim: **engine and methodology are the main research result, the game is a test case for the hypothesis** ([METHODOLOGY](/docs/methodology/METHODOLOGY.md)). By implementing combat, magic, and world content through the modding system rather than alongside it, we make every gameplay feature also a test of the modding architecture. A combat system that ships as a vanilla mod is the strongest possible falsifiable claim that the contract surface for mods is complete.

Phases do not overlap in code ownership. Closed phases retain their entries here as historical record; current work is the Mod-OS Migration; Phase 9 (Native Runtime) remains the post-launch endpoint.

## Status overview

*Updated: 2026-05-03 (M7 closed; M8 next). Partial re-sync 2026-06-02 (DD-2): K-series + K9 rows retired to Closed and the KERNEL_ARCHITECTURE.md Part 2/3 roadmap relocated into «Native foundation tracks» below. Full re-sync 2026-06-12 (Architecture Truth Cascade): M8 row verified to honest state (build criterion attempted in-session), V-row split into V0 ✅ / V1 ✅ / V2 ⏭ / M-V ⏭ with evidence, К-extensions cascade block added (#0–#5), M3.5 relocated in from MOD_OS §11, stale version pins removed per the citation-form rule (CODING_STANDARDS §6). Governance re-sync 2026-07-17 (CORPUS_CLOSURE_INVERSION_B): the 14 corpus-rework successors ratified LOCKED v1.0.0 after the D1 full-corpus review (register 2.32, PR #43); the register inverted to schema 2.0 — F-34 executed (frontmatter SoT, derived archive + authority surface, gates armed, PS writers retired), F-2 dissolved, F-9 closed. Governance re-sync 2026-07-17 (DRAFTS_RATIFICATION, first cascade under register schema 2.0): EVT-R4 item [6] EXECUTED — the seven 2026-07-15 cross-cutting drafts re-verified claim-by-claim at `48983c4` (code anchors EXACT wholesale; doc anchors retargeted onto the post-rework LOCKED successors per CODING_STANDARDS §6.1), SIX ratified AUTHORED → LOCKED v1.0.0 (EXECUTION_AUTHORITY_MATRIX, CONCURRENCY_AND_MEMORY_MODEL, RESOURCE_OWNERSHIP_AND_LIFETIME, ENGINE_LIFECYCLE_AND_TRANSACTIONS, TIME_AND_CONSISTENCY_MODEL, IDENTITY_AND_ABI_CONTRACT; EVT-2026-07-17-DRAFTS_RATIFICATION), PERSISTENCE_SNAPSHOT_CONTRACT retargeted and HELD AUTHORED until the save milestone; both mechanical staleness classes cleared (candidate banners 14 → 0; "(AUTHORED draft)" qualifiers → PSC-only); authority surface 24 → 30 LOCKED. The ratified contracts are the standing work orders — see «⏭ Engineering queue (post-DRAFTS_RATIFICATION)». Remaining governance queue: the G-RATIO matrix deliberation (ruling (e), unblocked by `docs/reports/G_RATIO_PER_RULE_BREAKDOWN.md`).*

| Phase | Status | Tests | Notes |
|---|---|---|---|
| Phase 0 — Contracts | ✅ Closed | — | Public surface in `DualFrontier.Contracts` |
| Phase 1 — Core | ✅ Closed | 60/60 | ECS core, scheduler, domain buses, `[Deferred]`/`[Immediate]` |
| Phase 2 — Verification | ✅ Closed | 11/11 | Isolation guard, ContractValidator |
| Phase 3 — Pawns | ✅ Closed | 1/1 | A* pathfinding, Godot bridge, MoodSystem publishing |
| Phase 3.5 — Godot DevKit | ✅ Closed | — | DevKit plugin, `.dfscene` export, F5 launch |
| Phase 4 — Economy | ✅ Closed | 6/6 | Inventory, ElectricGrid, Converter at 30%, HUD |
| Persistence (scaffold) | ✅ Closed | 4/4 | TileEncoder/RLE, ComponentEncoder, EntityEncoder, StringPool |
| **M0 — Mod-OS Phase 0** | ✅ Closed | — | `MOD_OS_ARCHITECTURE` v1.0 LOCKED |
| **M1 — Manifest v2** | ✅ Closed | added | `VersionConstraint`, `ModDependency`, `ManifestCapabilities`, `ModManifest` v2, `ManifestParser`, full `ValidationErrorKind` set |
| **M2 — IModApi v2** | ✅ Closed | added | Real `Publish`/`Subscribe`, `GetKernelCapabilities`, `GetOwnManifest`, `Log`, `RestrictedModApi` v2 |
| **M3 — Capability model** | ✅ Closed | added (`KernelCapabilityRegistryTests`, `RestrictedModApiV2Tests`, `CapabilityValidationTests`, `ProductionComponentCapabilityTests`) | M3.1 `KernelCapabilityRegistry` + `[ModAccessible]` opt-in; M3.2 capability-enforcing `RestrictedModApi` (Publish/Subscribe runtime check, hybrid per `MOD_OS_ARCHITECTURE` §3.6 v1.2); M3.3 `[ModCapabilities]` + load-time cross-check (`ContractValidator` Phases C+D); M3.4 deferred |
| M3.4 — CI capability analyzer | ⏸ Deferred | — | Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion); unblocked when first external mod author appears |
| M3.5 — Field capability + Path α/β consistency analyzer | ⏸ Deferred | — | `[FieldAccessible]` registry extension + Path α/β consistency analyzer (К-L3.1 Q5.b deferred enforcement). Relocated from MOD_OS_ARCHITECTURE §11 at its v1.12.0 code-truth rewrite; see M3 section below. |
| **M4 — Shared ALC** | ✅ Closed | added (`CrossAlcTypeIdentityTests`, `SharedAssemblyResolutionTests`, `ContractTypeInRegularModTests`, `SharedModComplianceTests`) | M4.1 `SharedModLoadContext` + two-pass loader + cross-ALC type identity; M4.2 `ContractValidator` Phase E enforces D-4 (no contract types in regular mods); M4.3 D-5 LOCKED shared-mod cycle detection + Phase F enforces §5.2 shared-mod compliance |
| **M5 — Version constraints** | ✅ Closed | added (`RegularModTopologicalSortTests`, `DependencyPresenceTests`, `M51PipelineIntegrationTests`, `PhaseAModernizationTests`, `PhaseGInterModVersionTests`, `M52IntegrationTests`) | M5.1 pipeline regular-mod toposort + dependency presence (`MissingDependency` / optional warning); M5.2 `ContractValidator` Phase A v1/v2 dual-path modernization + Phase G inter-mod version check; cascade-failure semantics ratified as deliberate accumulation (per §8.7) |
| **M6 — Bridge replacement** | ✅ Closed | added (`PhaseHBridgeReplacementTests`, `Phase5BridgeAnnotationsTests`, `CollectReplacedFqnsTests`, `M62IntegrationTests`) | M6.1 `[BridgeImplementation(Replaceable)]` + Phase 5 combat stubs annotated + `ContractValidator` Phase H bridge replacement validation; M6.2 `ModIntegrationPipeline` skip-on-replace graph build + integration tests across all §7.5 scenarios |
| **M7 — Hot reload** | ✅ Closed | 437/437 (50 commits) | M7.1 ✅ Pause/Resume + `IsRunning`; M7.2 ✅ ALC unload chain steps 1–6 + §9.5.1 best-effort failure semantics; M7.3 ✅ step 7 `WeakReference` + GC pump + `ModUnloadTimeout` + Phase 2 carried-debt closure; M7.4 ✅ D-7 build-pipeline `hotReload` override; M7.5.A ✅ `ModMenuController` editing-session lifecycle + `IModDiscoverer` + `Pipeline.GetActiveMods`; M7.5.B.1 ✅ production `GameBootstrap` integration via `GameContext`; M7.5.B.2 ✅ Godot `ModMenuPanel` modal overlay + F10 hotkey + menu-lifecycle pauses simulation. 6 housekeeping passes ✅ (TICK display, TickScheduler.ShouldRun race, real pawn data, NeedsSystem decay direction, ModMenuPanel position + assets gitignore, menu-pauses-simulation). Closure verified — see [M7_CLOSURE_REVIEW](./audit/M7_CLOSURE_REVIEW.md). §9.2 v1.6 ratification candidate registered for future cycle. |
| **— pre-namespace boundary —** | | | **Composite namespace applies M8.x forward only.** Closed M0..M7 phases preserved under pre-composite-namespace nomenclature per Q-M-1 LOCK (`docs/architecture/COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` §3.7). Closure reviews `docs/audit/M*_CLOSURE_REVIEW.md`, audit passes `docs/audit/AUDIT_PASS_*.md`, and historical prompts `docs/prompts/M*.md` retain original names as shipped (not renamed retroactively). |
| M8 — Vanilla skeletons (M-K bucket) | ⏭ Pending | — | Honest state (verified 2026-06-12, Architecture Truth Cascade): six mod projects on disk (5 regular + `Vanilla.Core` shared) as strict-v3 skeletons (`manifestVersion: "3"`, empty `Initialize`); all six build clean (`dotnet build -c Release` per project, exit 0, zero warnings under `TreatWarningsAsErrors`). The build-pipeline `hotReload` override criterion is covered green by `M74BuildPipelineTests`. REMAINING for closure: the full-set pipeline smoke-load criterion (all vanilla mods loaded into `ModIntegrationPipeline` + one kernel tick, no exceptions) — no harness exists today; the menu-driven load path needs a manual session or a dedicated integration test. No faked flip. Composite namespace: M-K bucket per Q-M-2 LOCK; specific identifiers within bucket deferred to per-mod authoring per FHE-style reserved pattern. |
| M9 — Vanilla.Combat — DEFERRED | ⏭ Excluded from cascade | — | Per Q-V-2 LOCK + deliberation §4 (deferred items registry): Combat is a consumer mod, identity discussion deferred until V substrate ready. Original Phase 5 scope preserved as historical context. M9 namespace freed under composite namespace (Q-R-2 LOCK — no remaining collision with runtime M9.x). |
| M10 — Remaining vanilla (M-K bucket; V-side reserved for multi-substrate mods) | ⏭ Pending | — | Magic, Inventory, Pawn, World — incremental. K-side under M-K bucket per Q-M-2. Multi-substrate mods (Magic, Electricity, Water, Movement) carry compound marker `M-K{N} / M-V` with V-side identifier deferred to V substrate authoring time per Q-V-2 FHE-pattern. |
| **K-series — Native ECS kernel** | ✅ Closed | — | K0–K10.3 shipped (native scheduler К10.1, native bus К10.2, pipeline/display/quiescent К10.3 v2); K8.5 + K10.4 (TLA+) pending. Detail relocated to «Native foundation tracks» below per DD-2. |
| **K9 — Field storage abstraction** | ✅ Closed | — | `RawTileField<T>` shipped 2026-05-11 (conductivity / storage flags, CPU functional path); unblocks V substrate compute primitives V1/V2. |
| **V substrate — V0 foundation** | ✅ Closed | — | Realized 2026-05-18/19 (Q8 ratification; closure `1b8f2ea`): V0.A window/instance/device; V0.B swapchain + compute plumbing + SPIR-V toolchain (К-L19 LOCKED with `HardwareCapabilityCheck` fail-fast); V0.C.1 PNG decoder + textured sprites + input event types; V0.C.2 batched `SpriteRenderer` + Camera2D + TileMap. Rendering cutover completed at К-ext #2 (Godot purged 2026-05-23). Honest residual scope under this track: text/UI primitives (former R.6), DebugOverlay/frame-timer, focus→pause coupling, Domain input forwarding (Launcher drains + discards). Track consolidation per Q-G-1/Q-G-2 — see [VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md). |
| **V substrate — V1 diffusion primitive** | ✅ Closed | — | Realized 2026-05-19 (PR #40 merge `88aebf2`, closure `7ad0560`): `V1DiffusionPipeline.cs` + `DiffusionPushConstants.cs`; `diffusion.comp(.spv)`; CPU oracles (`IsotropicDiffusionKernel`/`AnisotropicDiffusionKernel`) + equivalence, insulator, mass-conservation suites; 200×200 iso/aniso smoke scenes; dispatch latency benchmark. M-V1 demo NOT included (see M-V row). |
| **V substrate — V2 wave primitive** | ⏭ Pending | — | Genuinely pending: `wave.comp` absent (no shader, no `.spv`, no managed wrapper, no distance/direction side products, no oracle/tests). Sequencing: V2 amendment authoring → V2 execution; design held in [VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md) §1.3. |
| **V substrate — M-V demonstrations** | ⏭ Pending | — | All pending: vanilla mods are strict-v3 skeletons (empty `Initialize`); mod-facing compute surface unwired (`RestrictedModApi.ComputePipelines => null`). M-V1 mana + M-V2 electricity gate on mod content + ComputePipelines wiring; M-V7/M-V8 movement additionally gate on V2; M-V5 deferred (Domain B disposition TBD at amendment authoring); M-V3/M-V4/M-V6/M-V9 reserved identifier gaps per Q-G-2. |
| Phase 9 — Native Runtime | ⏭ Post-launch | — | Separate large project (now decomposed into K-series + V substrate above per Q-G-1 LOCK). |

**Engine snapshot:** Phases 0–4 closed at 82/82 tests. M1 added Manifest/Parser test suites (`VersionConstraintTests`, `ModDependencyTests`, `ManifestCapabilitiesTests`, `ModManifestV2Tests`, `ManifestParserTests`). M2 added `RestrictedModApiV2Tests`. M3 added `KernelCapabilityRegistryTests`, `CapabilityValidationTests`, and `ProductionComponentCapabilityTests` (260/260 at M3 closure). M4 added `CrossAlcTypeIdentityTests` and `SharedAssemblyResolutionTests` (M4.1), `ContractTypeInRegularModTests` (M4.2), and `SharedModComplianceTests` (M4.3). M5 added `RegularModTopologicalSortTests`, `DependencyPresenceTests`, and `M51PipelineIntegrationTests` (M5.1) plus `PhaseAModernizationTests`, `PhaseGInterModVersionTests`, and `M52IntegrationTests` (M5.2). M6 added `PhaseHBridgeReplacementTests` and `Phase5BridgeAnnotationsTests` (M6.1) plus `CollectReplacedFqnsTests` and `M62IntegrationTests` (M6.2). M7.1 added `M71PauseResumeTests` (11 — default-state, idempotent setters, Apply/UnloadAll guards with verbatim §9.3 messages, default-paused regression guard). M7.2 added `M72UnloadChainTests` (13 — run-flag guard parity, idempotency for non-active mods, per-step verification of the §9.5 chain steps 1–6, best-effort failure discipline via a step-2 throwing seam, UnloadAll refactor regression + bulk-unload preservation + M7.1 guard preservation). M7.3 added `M73Step7Tests` (5 — happy-path step 7 release, ALC-retainer timeout path, canonical warning shape, AD #7 step-7-after-upstream-failure invariant, mod-removed-from-active-set after timeout), `M73Phase2DebtTests` (2 real-mod fixtures — `Fixture.RegularMod_DependedOn` minimal surface + `Fixture.RegularMod_ReplacesCombat` with system registration and bridge replacement; both close §10.4 hard-required `WeakReference.IsAlive == false` within timeout), and the `ModUnloadAssertions` helper mirroring the production spin pattern (non-inlined + GC pump bracket per §9.5 step 7) for reuse by M8+ fixture tests. M7.4 added `ManifestRewriterTests` (7 — flips `hotReload: true` → `false` and returns `Rewritten`, byte-identical no-ops for `AlreadyFalse` / `FieldAbsent` cases, full v2 manifest field-preservation round-trip, `NotFound` / `ParseError` failure semantics, idempotency under repeat invocation) and `M74BuildPipelineTests` (2 integration tests via `dotnet build` subprocess against `Fixture.VanillaMod_HotReloadOverride` — Release build rewrites the bin manifest while leaving the source unchanged; Debug build leaves both source and bin manifests at `hotReload: true`). M7.5.A added `ModMenuControllerTests` (22 — Begin/Cancel pause-resume + AD #6 idempotency, Toggle pending-set mutation + §9.6 RejectedHotReloadDisabled + AD #5 first-load-is-not-reload + NoSession/UnknownMod gates, CanToggle UI hint mirror, GetEditableState combined active+discovered rows with flags + throws-without-session, Commit no-op-success + add-only + remove-only + add-and-remove + AD #4 failure-stays-paused + retry-recovery), `DefaultModDiscovererTests` (4 — nonexistent-root empty for first-launch safety, manifest-less subdir skipped, valid manifest parsed, per-manifest parse failure swallowed for best-effort enumeration), and `PipelineGetActiveModsTests` (4 — empty-pipeline empty list, post-Apply contents, post-UnloadMod removal, fresh-list snapshot not live view). M7.5.B.1 added `GameBootstrapIntegrationTests` (7 — production-side smoke coverage of `GameBootstrap.CreateLoop`: returns `GameContext` with both `Loop` + `Controller`, controller's `BeginEditing` succeeds and flips `IsEditing`, empty-`modsRoot` returns empty editing state, fixture-`modsRoot` returns the discovered row with `IsCurrentlyActive`/`IsPendingActive`/`CanToggle` flags set correctly, non-existent-`modsRoot` succeeds with no throw, default-`modsRoot` parameter is the literal string `"mods"` via reflection on `MethodInfo.GetParameters()[1].DefaultValue`, loop start/stop round-trips cleanly through the new `GameContext` shape). **Total at M7.5.B.1 closure: 415/415 passed** (verify with `dotnet test` against the current solution). A subsequent Phase-4 housekeeping commit added one integration test (`CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`) for the TICK display fix, taking the total to **416/416** at the time of the Backlog section establishment. A second housekeeping commit (`e0b0ecf` fix + `52d6d3f` stress test) resolved a pre-Phase-1 latent race in `TickScheduler.ShouldRun`'s tick-rate cache (surfaced by the new integration test at ~60 % flake rate over a 5-run sample) by swapping the underlying storage from `Dictionary<Type, int>` to `ConcurrentDictionary<Type, int>` and adding `TickSchedulerThreadSafetyTests` as a unit-level regression guard, taking the total to **417/417**. A third housekeeping pass (`9141bd6` feat + `659a64a` test) replaced the 3-pawn hardcoded list and the Warhammer-flavored placeholder name/role/skill UI fabrications with real end-to-end data: new `IdentityComponent` (single `Name` field), new `RandomPawnFactory` (deterministic by seed, 10 colonists, all 13 SkillKind values populated per pawn), `PawnStateChangedEvent` + `PawnStateCommand` extended with `TopSkills`, `PawnStateReporterSystem` rewritten to read `IdentityComponent.Name` and compute top-3 skills (no LINQ), `PawnDetail` re-adding the SKILLS section driven by real component data and removing the role label entirely, plus 8 dead files deleted (4 Phase-3 stub UI classes, 1 Phase-3 stub node, 3 undispatched bridge commands). New `RandomPawnFactoryTests` (8) + `GameBootstrapIntegrationTests` (+3 facts: 10-pawn baseline, real Name in PawnStateCommand, descending TopSkills in PawnStateCommand) take the total to **428/428**. A fourth housekeeping pass (`ee12108` fix + `7ea038c` test) flipped the four `NeedsSystem` decay-line signs from `-` to `+` so the implementation finally matches the field semantic in `NeedsComponent` (`0 = full, 1 = starving`). Decay-toward-0 was a placeholder lie: it implied automatic recovery while no module currently closes needs in the simulation — there are no food / water / bed entities, no `EatSystem` / `DrinkSystem` / `SleepSystem`, only a `JobSystem` that already assigns `JobKind.Eat` / `Sleep` on `NeedsCriticalEvent` but has no execution layer to consume the assignment. Per the project's operating principle, the honest behaviour for an incomplete simulation is accumulating deficit until Phase 5 lands the recovery loop. New `NeedsAccumulationTests` (3 — single-need growth at initial 0.5, all-four-needs symmetric growth at initial 0.3, ceiling clamp at 1.0) lock the post-honesty-pass contract so a future refactor cannot silently revert the direction; existing `NeedsJobIntegrationTests.Starving_pawn_receives_Eat_job_after_NeedsCritical_fires_on_the_Pawns_bus` continues to pass without modification (its fixture seeds `Hunger = 0.9f`, above `CriticalThreshold = 0.8f` under either decay direction). Test count: 428 → **431/431**. M7.5.B.2 added `MenuFlow_OpenCommitClose_LeavesEditingFalse`, `MenuFlow_OpenCancelClose_LeavesEditingFalse`, and `MenuFlow_OpenWithoutCommitOrCancel_StaysEditing` to `GameBootstrapIntegrationTests` — three controller-level integration tests that lock the menu-flow sequences `ModMenuPanel` invokes (Commit closes the session, Cancel closes the session, opening without explicit close keeps the session live). Test count: 431 → **434/434**. A second M7.5.B.2 follow-up housekeeping pass added `MenuFlow_BeginEditing_PausesGameLoop`, `MenuFlow_Cancel_ResumesGameLoop`, and `MenuFlow_CommitSuccess_ResumesGameLoop` to the same suite — three integration tests at the `GameContext` level locking the menu lifecycle's effect on the simulation thread (BeginEditing pauses, Cancel resumes, successful Commit resumes; failed-commit-stays-paused covered indirectly by the controller's success-path-only RaiseHook plus M7.5.A's existing failure-path coverage). Test count: 434 → **437/437**. The structural foundation laid in Phases 0–4 is the entire prerequisite for the Mod-OS Migration; nothing in M1–M7 requires touching the ECS core, the scheduler, or the bus contracts (`IGameServices`).

---

## Backlog

Phase 3.5 / Phase 4 / Phase 5 carry-forward items surfaced during housekeeping passes that are not on the active M-cycle critical path. Each entry names the originating phase, a one-line description of the debt, and the phase that will properly address it. New entries land here as additional housekeeping commits surface debts; closure of an entry happens in the addressing phase, not by editing this list inside the housekeeping commit that flagged it.

### Phase 5 — gameplay completeness

- **Recovery loop for accumulating needs (Phase 3 / Phase 4 carry-forward).** Needs now grow as deficit per honesty-pass commit `ee12108`, but no module closes them: no food / water / bed entities exist; no `EatSystem` / `DrinkSystem` / `SleepSystem` exists to consume them and reset the corresponding `NeedsComponent` field; pawns therefore degrade indefinitely once spawned. `JobSystem` already assigns `JobKind.Eat` / `Sleep` on `NeedsCriticalEvent` (subscription wired in `OnInitialize`, `PickJob` selection by `CriticalThreshold = 0.8f`); the missing layer is execution. **Addressed by:** Phase 5 / M10.C — once the recovery side lands, `NeedsAccumulationTests` will need adjustment (or supplemental tests covering the recovery transitions).

- **`NeedsComponent` field semantic rename (Phase 3 / Phase 4 carry-forward).** The HUD label "Hunger 100%" alongside internal `needs.Hunger = 0` was confusing in the pre-honesty-pass world; the decay-direction flip in `ee12108` realigns reality with the existing field semantic (`0 = full, 1 = starving`), so the rename is no longer load-bearing. Still optional for clarity if the design later prefers wellness-named fields. **Addressed by:** Phase 5 / M10.C — bundle with the recovery-loop work above if pursued at all.

- **Job loop (Phase 3 carry-forward).** `JobSystem` already assigns `JobKind.Eat` / `Sleep` on `NeedsCriticalEvent` (since the `OnNeedsCritical` + `PickJob` wiring landed earlier); food / water / bed entities don't exist; pawns wander randomly via `MovementSystem` regardless of `JobComponent.Current`. Needs full Phase 5 implementation: spawn food / water / bed entities, extend `JobKind` enum (Wander), pathing + interaction handlers that actually consume the assigned job and reset the deficit. **Addressed by:** Phase 5 / M10.C — same workstream as the recovery-loop entry above; together they close the deficit → urgent-job → execution → reset cycle.

- **`MoodBreakEvent` handlers (Phase 3 carry-forward).** Published by `MoodSystem` on threshold transition; no subscribers. Pawns reach low mood with no observable consequence. **Addressed by:** Phase 5 — break-behaviour systems (run-away, fight-others, breakdown animation) when the corresponding gameplay lands.

- **`MovementSystem` job-aware wandering / `JobLabel` honesty (Phase 3 carry-forward).** `MovementSystem` picks random wander targets when `MovementComponent.Path` is empty, regardless of `JobComponent.Current`. The HUD then truthfully shows `JobLabel = "Idle"` for wandering pawns — technically correct, conceptually wrong. Either teach `MovementSystem` to honour `JobKind.Idle` (hold position) vs follow job target, or surface the wander state via a new `JobKind.Wander` so the HUD label tells the truth. **Addressed by:** Phase 5 — bundled with the job-loop work above.

### Phase 5 — UI ↔ data wiring

- **HealthComponent display.** `HealthComponent` exists in `DualFrontier.Components.Pawn` but is not added to spawned pawns and not displayed. **Addressed by:** Phase 5 — `RandomPawnFactory` populates it; `PawnStateChangedEvent` / `PawnStateCommand` carry the value; `PawnDetail` adds a HEALTH bar.

- **FactionComponent display.** Faction tag / colour in roster + detail. **Addressed by:** Phase 5 — same component-data + UI-section pattern as HealthComponent.

- **RaceComponent display.** Race / species tag in detail. **Addressed by:** Phase 5 — same pattern.

- **SocialComponent display (relationships).** Relationship graph or list of recent interactions. **Addressed by:** Phase 5 — depends on `SocialSystem` migration to `Vanilla.Pawn` (M10).

- **ManaComponent / EtherComponent display (magic system).** Mana / ether bars in `PawnDetail` for arcane-class pawns. **Addressed by:** Phase 5 / M10 — magic system migration.

- **Role / class concept (no component yet).** No role/class concept exists in any component; `PawnDetail` displayed a hash-derived role label which has been removed. If a role concept is introduced (sergeant, magus, etc.), publish via `PawnStateChangedEvent` and re-add the role line under the name. **Addressed by:** Phase 5 — when the design decides to introduce roles.

- **Full skills display (currently top-3).** `PawnDetail` shows the top-3 skills by level; the remaining 10 SkillKinds are not displayed. **Addressed by:** Phase 5 — an "Inspector" mode that shows the full skill grid.

- **UI redesign with Kenney UI pack + Cinzel font.** Replace the placeholder Palette-driven minimal styling with proper themed UI using extracted assets at `assets/kenney_ui-pack/`, `assets/kenney_ui-pack-rpg-expansion/`, `assets/cinzel/`. Scope decisions pending (which Kenney pack as base, theme system breadth, font application rules). Separate brief required.

### Phase 5 — combat / magic / scaffolding

- **Combat command dispatch (re-create).** `ProjectileSpawnedCommand` and `SpellCastCommand` were deleted as dead code in housekeeping `9141bd6` (no dispatcher case, no event subscriber). They will be re-created with real fire-and-forget rendering when the projectile / spell systems land. **Addressed by:** Phase 5 / M9 (combat) and Phase 5 / M10.B (magic).

- **Stub UI re-creation.** `BuildMenu`, `AlertPanel`, `ManaBar`, `PawnInspector`, and `ProjectileVisual` were deleted as dead code in housekeeping `9141bd6`. Phase 5 introduces them as real Launcher UI elements (Vulkan-backed presentation layer) with real data sources when the corresponding gameplay systems land (placement mode, alert/notification system, magic system, expanded pawn inspector). **Addressed by:** Phase 5.

- **`UIUpdateCommand` (re-create).** Generic notification surface; deleted in housekeeping `9141bd6` because no event subscriber published it. Reintroduce when the alert/notification system lands. **Addressed by:** Phase 5.

- **Pawn sprite replacement.** Current `PawnVisual` uses solid blue squares; Kenney roguelike packs (`assets/kenney_roguelike-rpg-pack/`, `assets/kenney_tiny-dungeon/`) have pixel-art sprites. Tied to `PawnVisual` + render command extensions; belongs with combat/magic system landing. **Addressed by:** Phase 5 / M9.

### Resolved

- **NeedsSystem decay direction.** Was decay-toward-0 (falsely implying automatic recovery via the `Math.Clamp(needs.Hunger - HungerDecayPerTick, 0f, 1f)` line and three siblings); flipped to deficit accumulation in housekeeping commit `ee12108`. Class-level XML doc rewritten to describe accumulating-deficit semantics with the explicit no-recovery caveat. Constants (`HungerDecayPerTick = 0.002f`, `ThirstDecayPerTick = 0.0015f`, `SleepDecayPerTick = 0.001f`, `ComfortDecayPerTick = 0.0005f`) unchanged — they were already calibrated for "deficit accumulation" pacing semantically. Regression guard `NeedsAccumulationTests` (3 facts) added in commit `7ea038c`. Recovery loop (food / water / bed entities + Eat/Drink/Sleep job execution) remains pending in Phase 5 — until then pawns visibly degrade indefinitely, which is the honest behaviour for an incomplete simulation.

- **Pre-Phase-1 latent race in `TickScheduler.ShouldRun` cache.** `Dictionary<Type, int>` populated from inside `Parallel.ForEach` via `ParallelSystemScheduler.ExecutePhase`, against the class's own "not thread-safe" claim. Surfaced by housekeeping commit `21921887`'s integration test (`CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`, ~60 % flake rate on a 5-run sample). Resolved by housekeeping commit `e0b0ecf`: `Dictionary` → `ConcurrentDictionary`, `TryGetValue + indexer` → `GetOrAdd(systemType, ResolveTicksPerUpdate)`. Class-level XML doc rewritten to truthfully document thread-safety guarantees. New regression guard `TickSchedulerThreadSafetyTests.ShouldRun_InvokedFromManyThreadsWithEmptyCache_DoesNotThrowAndReturnsConsistentResults` added in commit `52d6d3f` (32 distinct synthetic systems × 500 iterations; pre-fix fails 9/10 invocations; post-fix passes 10/10). Integration test fixed-flake guard: 100 consecutive runs of `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` pass 100/100 post-fix.

- **UI lies removed via real pawn data (housekeeping `9141bd6` feat + `659a64a` test).** Pawn names were a hardcoded Warhammer-flavoured array indexed by `EntityId.Index` in `PawnStateReporterSystem`; the role label was a hash-derived fabrication in `PawnDetail.MakeRole` against a 5-element role array (no role component existed); the skill bars were hash-derived placeholders in `PawnDetail.DemoSkills` while `SkillsComponent` was completely ignored at the display surface. Resolved by introducing `IdentityComponent` (single `Name` field) and `RandomPawnFactory` (deterministic seed-42 generation, 10 colonists, all 13 `SkillKind` values populated per pawn), extending `PawnStateChangedEvent` / `PawnStateCommand` with `TopSkills: IReadOnlyList<(SkillKind Kind, int Level)>`, rewriting `PawnStateReporterSystem` to read the real components, and re-driving `PawnDetail`'s SKILLS section from real top-3 data while removing the role label entirely. Eight dead files deleted in the same pass: `BuildMenu.cs`, `AlertPanel.cs`, `ManaBar.cs`, `PawnInspector.cs`, `ProjectileVisual.cs`, `ProjectileSpawnedCommand.cs`, `SpellCastCommand.cs`, `UIUpdateCommand.cs` (all confirmed unused by `grep -r` before deletion). Test count: 417 → 428 (+11). M-phase boundary preserved: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.

### Maximum engineering refactor (parallel track)

A three-track discipline escalation proposed and ratified in
[MAXIMUM_ENGINEERING_REFACTOR](/docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md) v1.0.
Tracks adopt incrementally as Phase 6+ sidecar work, parallel to the
M0–M10 sequence and the post-shipping reservoir entries.

- **Track A — Formal verification.** F* proof of one isolation guard
  property. Pilot scope; falsifiable via 2-week effort budget.
  See brief §2 for property selection (Candidate A2 recommended pilot).
- **Track B — Type-theoretic architecture.** Roslyn analyzers enforcing
  architectural invariants currently expressed in prose. Pilot scope;
  B1 layer dependency analyzer recommended first. See brief §3.
- **Track C — Methodology replication kit.** Executable replication
  protocol enabling external developers to reproduce the methodology
  measurements. Post-Phase-7 (requires stable baseline). See brief §4.

Adoption is opt-in per track. Tracks do not block shipping path.
Per-track briefs authored at activation time, not now.

---

## Closed phases

The following phases are retained for historical record. Each has been verified by its acceptance tests and is no longer subject to active edits except for translation, refactor, or i18n work.

### ✅ Phase 0 — Contracts (closed)

Goal achieved: every public interface and attribute is pinned in `DualFrontier.Contracts`. The assembly references only `System.*` and serves as the only contract surface a mod sees. Build is clean; every public type carries English XML documentation.

### ✅ Phase 1 — Core (closed)

Result: a working ECS core with multithreaded scheduler and domain buses. `[Deferred]` and `[Immediate]` semantics implemented in `DomainEventBus` (closed in v0.3 as Phase 4 debt). `IntentBatcher`, `DependencyGraph`, `ParallelSystemScheduler`, `TickScheduler` all operational. Main simulation loop (`FrameClock`, `GameLoop`) supports pause and 1×/2×/3× speed.

Tests: 60/60 covering `ComponentStore`, `DependencyGraph`, `DomainEventBus`, parallel execution.

### ✅ Phase 2 — Verification (closed)

Result: structural guarantees provably enforced. Isolation guard catches undeclared component access, direct system access, `GetSystem` calls. `ModIsolationException` raised when a mod assembly tries to load `DualFrontier.Core.dll`. `ContractValidator` returns precise messages on write-write conflicts between mods. `ModIntegrationPipeline.Apply` is atomic — failure at any step rolls back to the previous scheduler phase list.

Tests: 11/11 isolation tests pass in DEBUG; critical subset passes in RELEASE.

**Carried debt (closed in M7.3):** `AssemblyLoadContext` WeakReference unload tests landed with M7.3. Originally a Phase 2 backlog item, they became a hard requirement when M7 landed hot reload — the unload chain could not ship without proven ALC release. Closure shape: `M73Phase2DebtTests` exercises the §9.5 unload chain end-to-end against two representative real-mod fixtures (`Fixture.RegularMod_DependedOn` minimal surface; `Fixture.RegularMod_ReplacesCombat` with mod-system registration + bridge replacement) and asserts `WeakReference.IsAlive == false` within the §9.5 step 7 timeout via the `ModUnloadAssertions` helper. §11.4 zero-flake invariant verified across 3 consecutive runs at closure time.

### ✅ Phase 3 — Pawns (closed)

Result: a living colony. Pawns walk the map via A* pathfinding (2000-iteration cap, no cache). Needs decay, mood recomputes, jobs assigned by priority through event buses. `JobKind.Eat/Sleep/Idle` works through `JobSystem`. `MovementSystem` publishes `PawnMovedEvent` on every step. `NavGrid` initialises from `GameBootstrap` (50×50, 50 obstacles). `MoodSystem` publishes `MoodBreakEvent` via `Services.Pawns.Publish` on threshold transition (originally a Phase 3 backlog item, closed silently and now ratified here).

Tests: 1/1 (high-level integration); the system-level coverage moves to M9/M10 when the systems migrate to vanilla mods.

**Carried debt (now part of M10):** `SocialSystem` and `SkillSystem` exist as `[BridgeImplementation(Phase = 3)]` stubs in `DualFrontier.Systems.Pawn`. They will move to `Vanilla.Pawn` mod where they get real implementations.

### ✅ Phase 3.5 — Godot DevKit (closed)

Result: Phase 3.5 used Godot as both editor and temporary runtime — `DfDevKitPlugin`, `SceneExporter`, `EntityExporter` enabled scene authoring and `.dfscene` export, and F5 in Godot started the game with the loaded scene. The `IRenderer` / `ISceneLoader` / `IInputSource` contracts remain as reserved seams (their Godot backends retired at cascade #2, 2026-05-23; rendering is now the in-house Vulkan substrate via the Launcher).

Architectural context: [VISUAL_ENGINE (historical)](/docs/architecture/historical/VISUAL_ENGINE.md), superseded by [VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md) per Q-G-1 LOCK. The decoupling work here unblocks Phase 9 (Native Runtime) without committing to its timeline.

### ✅ Phase 4 — Economy (closed)

Result: storages, crafting workbenches, the power grid, the converter. `InventorySystem` with `_freeSlotCache` and `_cacheDirty`-driven batching. `HaulSystem` uses per-Update reservation set to prevent same-tick double-allocation. `ElectricGridSystem` distributes watts by priority on `IPowerBus`. `ConverterSystem` runs at 30 % efficiency, publishing `[Deferred] ConverterPowerOutputEvent` to break the component cycle with ElectricGrid. Grimdark HUD via `GameHUD`, `ColonyPanel`, `PawnDetail`.

Tests: 6/6 covering inventory deferred mutation, ElectricGrid overload, Converter efficiency.

**v0.3 architectural fixes (Phase 4 debt closure):** all items closed.

- `[Deferred]`/`[Immediate]` delivery in `DomainEventBus`.
- `IPowerBus` added to `IGameServices` (non-breaking).
- Inventory events marked `[Deferred]`; `HaulSystem.writes=[]` isolation preserved through subscriber context capture.
- `ConverterSystem.writes=[]`; output via `[Deferred] ConverterPowerOutputEvent`.
- All `OnInitialize` `NotImplementedException` stubs replaced with empty body + `[BridgeImplementation(Phase = N)]`.
- `HaulSystem` `return` after failed `TryFindHaul` replaced with `continue`; `DomainEventBus.Subscribe` TOCTOU fixed via `GetOrAdd`.

### ✅ Persistence scaffold (closed)

Result: save-compression scaffold complete. `TileEncoder` (RLE), `ComponentEncoder` (quantisation), `EntityEncoder` (range encoding), `StringPool`. Snapshot types in `DualFrontier.Persistence.Snapshots`. No Godot dependency.

Tests: 4/4 covering encoder round-trips and string pool semantics.

The save-game compatibility policy when a mod is missing (mod-OS decision D-6) targets this layer for the component-stripping logic; that work is out of M0–M10 scope and tracked separately.

---

## 🔨 Mod-OS Migration (M0–M10)

The migration sequence is derived from `MOD_OS_ARCHITECTURE` v1.5 §11. Each M-phase has a clear output artifact, acceptance criteria, and the set of architectural decisions (D-N) it consumes. Phases run in strict order — M(N+1) depends on M(N) — except where noted.

### ✅ M0 — Mod-OS Phase 0 (closed)

Goal achieved: architectural specification produced, reviewed, and locked.

**Output:** [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.0 LOCKED. All twelve decisions resolved (five strategic + seven detail D-1 through D-7). Implementation phases unblocked.

**No code changes.** This phase exists to make every subsequent code change traceable to a documented decision.

---

### ✅ M1 — Manifest v2 schema and parser (closed)

Goal achieved: `ModManifest` extended to the v2 schema specified in `MOD_OS_ARCHITECTURE` §2, with a backward-compatible JSON parser that accepts existing v1 manifests unchanged.

**Consumes decisions:** structural locks 1–5 (manifest fields for `replaces`, `hotReload`, three-tier versioning).

**What we implement**

- `ModManifest` v2 fields: `Kind` (`regular` | `shared`), `ApiVersion` (string with caret support), `HotReload` (bool, default `false`), `Replaces` (`IReadOnlyList<string>`), `Capabilities` (`ManifestCapabilities` record with `Required` and `Provided` lists).
- `ModDependency` record replacing the current `IReadOnlyList<string> Dependencies` — each entry carries `Id` and `Version` (constraint string).
- JSON parser that reads v2 fields, falls back to v1 defaults when fields are absent.
- New `ValidationErrorKind` entries: `MissingCapability`, `BridgeReplacementConflict`, `ProtectedSystemReplacement`, `UnknownSystemReplacement`, `IncompatibleVersion`, `SharedModWithEntryPoint`, `ContractTypeInRegularMod`.
- `ContractsVersion.Parse` extension (or new `VersionConstraint` struct) to handle `"^X.Y.Z"`. Tilde explicitly rejected with a `FormatException` directing the author to caret.

**Acceptance criteria**

- Existing `ExampleMod`'s `mod.manifest.json` loads unchanged (backward compatibility).
- A v2 manifest with `kind`, `apiVersion`, `hotReload`, `dependencies` (with versions), `replaces`, `capabilities` parses to a fully populated `ModManifest`.
- Caret-prefixed versions parse correctly; tilde produces a clean `FormatException`.
- Manifest with malformed capability strings rejected at parse time with a clear message.
- All seven new `ValidationErrorKind` entries are exercised by unit tests with their canonical error messages.

**Unblocks:** every subsequent M-phase. The manifest is the single input every loader stage reads from.

---

### ✅ M2 — IModApi v2: Publish and Subscribe (closed)

Goal achieved: replaced the no-op semantics of `RestrictedModApi.Publish` and `Subscribe` with real bus routing through `ModBusRouter`, while preserving the v1 `IModApi` signature for backward compatibility.

**Consumes decisions:** D-3 (cast prevention via structural barrier — no analyzer, no runtime check).

**What we implement**

- `RestrictedModApi.Publish<T>` resolves the target bus from the event type (via a marker attribute on the event record — `[Combat]`, `[Magic]`, etc.) and dispatches through the kernel's `IGameServices`. `[Deferred]` and `[Immediate]` semantics honored.
- `RestrictedModApi.Subscribe<T>` registers the handler on the appropriate bus, wrapping it to capture the calling mod's `SystemExecutionContext`. The wrapper is recorded for `UnsubscribeAll`.
- New `IModApi.GetKernelCapabilities()` accessor returning the kernel's frozen capability set (used by mods for self-introspection).
- New `IModApi.GetOwnManifest()` accessor.
- New `IModApi.Log(LogLevel level, string message)` — replaces ad-hoc `Console.WriteLine` in mod code, prefixes log lines with the mod id.
- `RestrictedModApi` declared `internal sealed` (per D-3 lock); confirm `DualFrontier.Application` is not in the resolution path of any mod ALC.

**Acceptance criteria**

- A mod publishes `DamageEvent`; a kernel system subscribed to `IGameServices.Combat` receives it on the next tick (deferred) or in the same tick (immediate).
- A mod subscribes to `MoodBreakEvent`; the kernel `MoodSystem` publishes; the mod's handler runs under the mod's `SystemExecutionContext`.
- On mod unload, `RestrictedModApi.UnsubscribeAll` removes every wrapper from the bus dispatcher; subsequent kernel publishes do not invoke unloaded handlers.
- Capability check at `Subscribe` and `Publish` time still raises `CapabilityViolationException` (the actual capability registry lands in M3; here we stub the registry with a "permit-all" implementation behind a feature flag).
- `[Deferred]` events from mod systems queue correctly and deliver on the next phase boundary.

**Unblocks:** M3 (capability model needs working publish/subscribe to enforce against), M9, M10 (vanilla mods need real bus operations).

---

### ✅ M3 — Capability model (closed)

Goal: implement the capability registry, the `[ModAccessible]` and `[ModCapabilities]` attributes, and the load-time cross-check between manifest, code, and kernel-provided capabilities.

**Sub-phase status:**

- **M3.1 ✅ Closed.** Acceptance: `KernelCapabilityRegistry` scans `DualFrontier.Components` + `DualFrontier.Events` assemblies (commit `a73669f`); production components annotated `[ModAccessible]` per D-1 (commit `f91f065`); `KernelCapabilityRegistryTests` + `ProductionComponentCapabilityTests` (commit `b92fa66`) verify both registry mechanics and the §2.1 example manifest token resolution.
- **M3.2 ✅ Closed.** Acceptance: `RestrictedModApi.EnforceCapability` raises `CapabilityViolationException` on `Publish<T>` / `Subscribe<T>` when the manifest does not declare the capability; per `MOD_OS_ARCHITECTURE` §4.2 / §4.3 v1.2 hybrid enforcement (ratified in v1.2 §3.6). `RegisterSystem` is locked as v1 semantics unchanged per §4.1 — no capability check applies.
- **M3.3 ✅ Closed.** Acceptance: `ContractValidator` Phase C (capability satisfiability via kernel + listed dependencies) and Phase D (`[ModCapabilities]` × manifest cross-check) implemented; `CapabilityValidationTests` covers both phases.
- **M3.4 ⏸ Deferred.** CI Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion, see `MOD_OS_ARCHITECTURE` §3.8). Unblocked when the first external (non-vanilla) mod author appears. Rationale: runtime `CapabilityViolationException` already catches dishonest attribute declarations; the analyzer is developer-experience tooling for early feedback before publication, not a runtime safety boundary.

- **M3.5 ⏸ Deferred.** Capability registry refresh for field types via `[FieldAccessible]` scan extension, plus a Path α/β consistency analyzer (`[ManagedStorage]` shape↔attribute consistency, `RegisterComponent`/`RegisterManagedComponent` constraint match, dual-API access pattern enforcement per К-L3.1 Q5.b deferred enforcement). Output: extended `KernelCapabilityRegistry` recognising `[FieldAccessible]`; Roslyn analyzer covering the Path α/β consistency rules. Tests: `FieldCapabilityRegistryTests` + `PathConsistencyAnalyzerTests`. Unblock state: the originally stated field-side gate (K9) is met — K9 closed 2026-05-11; `[FieldAccessible]` does not exist in `src/` (verified 2026-06-12), so the milestone is genuinely pending in full. Candidate for Analyzer-track consolidation (the path-consistency rules overlap the A'.9.x rule families — see «Analyzer track»). *Relocated here from MOD_OS_ARCHITECTURE §11 at the v1.12.0 code-truth rewrite (Architecture Truth Cascade D7); MOD_OS no longer carries migration rows.*

**Consumes decisions:** D-1 (curated opt-in `[ModAccessible]`), D-2 (hybrid attribute + CI static analysis).

**What we implement**

- `[ModAccessible(Read = bool, Write = bool)]` attribute applied to public components in `DualFrontier.Components`. Per D-1, every component is invisible to mods until annotated. Components touched by Phase 5 — `WeaponComponent`, `ArmorComponent`, `AmmoComponent`, `ShieldComponent`, `HealthComponent` — get annotated as part of M3 in preparation for M9.
- `[ModCapabilities("publish:DualFrontier.Events.Combat.DamageEvent", "subscribe:..." )]` attribute on mod-supplied systems. Per D-2, this is the load-time declaration; the loader cross-checks it against the manifest's `capabilities.required`.
- `KernelCapabilityRegistry` — built at startup by reflecting over `[ModAccessible]` types and over `IEvent` types in `DualFrontier.Events.*`. Frozen after build; exposed via `IModApi.GetKernelCapabilities`.
- Capability-enforcing `RestrictedModApi`: `Publish` and `Subscribe` consult a per-mod capability set (built from manifest + provided-capabilities of dependency mods) and throw `CapabilityViolationException` on mismatch.
- CI-level static analyzer (Roslyn, separate package): scans mod assemblies for actual `Publish`/`Subscribe` call sites and verifies `[ModCapabilities]` declarations are honest. Runs in mod-publication CI, not at game load time. **(Deferred — see M3.4 above.)**

**Acceptance criteria**

- A mod declares `kernel.read:HealthComponent` in manifest; HealthComponent is annotated `[ModAccessible(Read = true)]`; load succeeds.
- A mod declares `kernel.read:MindComponent` in manifest; MindComponent is **not** annotated; load fails with `MissingCapability` naming the component.
- A mod's system has `[ModCapabilities("publish:DamageEvent")]` but the manifest does not list `kernel.publish:DualFrontier.Events.Combat.DamageEvent` in `capabilities.required`; load fails with `MissingCapability`.
- A mod's system publishes an event for which the system declares `[ModCapabilities]` but the manifest is correctly populated; load succeeds, publish reaches the bus.
- *(M3.4 deferred)* The CI analyzer detects a `Services.Combat.Publish<DamageEvent>` call in code that has `[ModCapabilities]` not listing `publish:DamageEvent`; CI fails.

**Unblocks:** M6 (bridge replacement validates capability of replacing system), M9, M10 (vanilla mods declare capabilities honestly).

---

### ✅ M4 — Shared ALC and shared mod kind (closed)

Goal achieved: shared `AssemblyLoadContext`, two-pass mod loading, cross-ALC type identity, and the three M4 enforcement points (D-4 contract types in regular mods, D-5 shared-mod cycles, §5.2 shared-mod compliance) are operational. Migration unblocked through M5.

**Sub-phase status:**

- **M4.1 ✅ Closed.** Acceptance: `SharedModLoadContext` (singleton, non-collectible, `Resolving` delegates `DualFrontier.*` to default ALC) and the two-pass loader (shared mods first into the shared ALC, then regular mods whose `ModLoadContext` delegates to that shared ALC for cross-mod type references) implemented. Cross-ALC type identity verified by `CrossAlcTypeIdentityTests`; assembly resolution semantics verified by `SharedAssemblyResolutionTests` (commits `0a3a858`, `cf14edb`, `56772fc`, `e5e0e30`, `1ec1354`, `cdb48f0`).
- **M4.2 ✅ Closed.** Acceptance: `ContractValidator` Phase E enforces D-4 — every regular mod's assemblies are scanned, and any exported type implementing `IEvent` or `IModContract` produces a typed `ContractTypeInRegularMod` error before `IMod.Initialize` runs (commits `68cb693`, `14e1dd0`, `c410add`); covered by `ContractTypeInRegularModTests`.
- **M4.3 ✅ Closed.** Acceptance: `ModIntegrationPipeline.TopoSortSharedMods` (Kahn's algorithm) detects D-5 LOCKED shared-mod cycles between manifest parse and shared-mod load — cyclic mods never reach assembly load and surface as `CyclicDependency` errors naming the affected mod set. `ContractValidator` Phase F enforces §5.2 shared-mod compliance — non-empty `entryAssembly`/`entryType`/`replaces` and any `IMod` implementation in the assembly each produce a typed `SharedModWithEntryPoint` error; `ModLoader.LoadSharedMod`'s M4.1 defensive IMod throw is removed in favour of the validator's typed accumulation (commits `df582d3`, `e0151d8`, `d628692`, `b71e9e2`, `90f8012`); covered by `SharedModComplianceTests`.

**Consumes decisions:** D-4 (active scan, reject contract types in regular mods — M4.2), D-5 (forbid shared-mod cycles — M4.3), §5.2 shared-mod compliance (M4.3).

**Acceptance criteria met (per `MOD_OS_ARCHITECTURE` §11.1):**

- A shared mod with `record FooEvent : IEvent` loads into the shared ALC (M4.1).
- A regular mod (different ALC) `Subscribe<FooEvent>(handler)`; another regular mod `Publish(new FooEvent(...))`; the subscriber's handler runs — cross-ALC type identity preserved (M4.1).
- A regular mod containing `record BadEvent : IEvent` is rejected at load with `ContractTypeInRegularMod` (M4.2).
- A shared mod with circular dependency on another shared mod is rejected with `CyclicDependency` (M4.3).
- A shared mod manifest with non-empty `EntryAssembly` is rejected with `SharedModWithEntryPoint` (M4.3).
- A shared mod assembly containing an `IMod` implementation is rejected with `SharedModWithEntryPoint` (M4.3).

**Unblocks:** M5 (dependency resolution with versions needs the shared/regular distinction), M9, M10 (vanilla.core shared mod precedes the slice mods).

---

### ✅ M5 — Inter-mod dependency resolution with caret syntax (closed)

Goal achieved: `ModIntegrationPipeline` resolves regular-mod dependencies using the three-tier SemVer model from `MOD_OS_ARCHITECTURE` §8. Regular-mod topological sort, dependency presence check, and validator-level inter-mod version check are operational; cascade-failure semantics ratified as deliberate accumulation per §8.7.

**Sub-phase status:**

- **M5.1 ✅ Closed.** Acceptance: regular-mod topological sort via `TopoSortByPredicate` (extracted from `TopoSortSharedMods`) detects regular-mod cycles before assembly load; dependency presence check produces `MissingDependency` for required deps and a `ValidationWarning` for optional deps; pass `[0.6]` in `ModIntegrationPipeline.Apply` runs between shared-mod cycle detection and shared-mod load; `PipelineResult.Warnings` field flows through every return path. Commits: `fffd785` (extract `TopoSortByPredicate` from `TopoSortSharedMods`), `13400bb` (add `TopoSortRegularMods` + `CheckDependencyPresence` helpers), `a3968f4` (wire regular-mod toposort and dep presence into pipeline), `bab4d85` (integration tests). Tests: `RegularModTopologicalSortTests` (6), `DependencyPresenceTests` (4), `M51PipelineIntegrationTests` (4).

- **M5.2 ✅ Closed.** Acceptance: `ContractValidator` Phase A modernized for v1/v2 dual-path — legacy `IncompatibleContractsVersion` retained for v1 manifests, new `IncompatibleVersion` emitted for v2 manifests through the full `VersionConstraint` pipeline; `ContractValidator` Phase G inter-mod dependency version check produces `IncompatibleVersion` when a regular mod's `dependencies[i].version` constraint is unsatisfied by the depended-on mod's actual version; `ContractValidator` class XML-doc updated to "seven-phase validator" (Phases A–G). Commits: `50efe9d` (Phase A modernization for v2 manifests via `VersionConstraint` pipeline), `f8f18ee` (Phase G inter-mod dependency version check), `376be7e` (integration tests). Tests: `PhaseAModernizationTests` (6), `PhaseGInterModVersionTests` (7), `M52IntegrationTests` (3).

**Cascade-failure semantics — accumulation, not skip.** Per §8.7 of [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md), "the failed set is presented to the user; the success set proceeds to load." M5 implementation interprets this as: when mod A depends on mod B and B fails its own validation (any phase), A is **not** silently dropped — A's own validation runs to completion, and any independent errors A produces also surface. Both errors appear in `result.Errors`. This matches the existing pipeline accumulation pattern (Phases B / C / D / E / F / G all accumulate without short-circuit) and gives mod authors maximum diagnostic information per `Apply` call. Demonstrated by:

- Validator-level: `Mod_WithCascadeFailure_BothErrorsReportedNotSkipped` (`PhaseGInterModVersionTests`).
- Pipeline-level: `Apply_WithCascadeFailure_SurfacesBothErrors` (`M52IntegrationTests`).

This is a deliberate interpretation of §8.7 wording "cascade-fail," registered here per [METHODOLOGY](/docs/methodology/METHODOLOGY.md)'s "no improvisation" rule. If the interpretation needs revision, escalate via §12 ratification process.

**Consumes decisions:** strategic lock 5 (caret syntax for inter-mod deps).

**Acceptance criteria met (per `MOD_OS_ARCHITECTURE` §11.1):**

- Mod A v1.0.0, Mod B requires `^1.0.0` of A: load succeeds.
- Mod A v2.0.0, Mod B requires `^1.0.0` of A: Mod B rejected with `IncompatibleVersion`.
- Mod A v1.5.3, Mod B requires `^1.0.0` of A: load succeeds (caret matches any 1.x).
- Mod A v1.2.0, Mod B requires `1.0.0` (exact): Mod B rejected with `IncompatibleVersion`.
- Topological sort detects circular dependency between regular mods; both rejected with `CyclicDependency`.
- A mod requires kernel `apiVersion: "^2.0.0"` against `ContractsVersion.Current = 1.0.0`: rejected with `IncompatibleVersion`.

**Unblocks:** M6 (vanilla mods have inter-slice dependencies), M8 onwards.

---

### ✅ M6 — Bridge replacement via `replaces` (closed)

Goal achieved: explicit bridge replacement is operational. Vanilla mods can now supersede `[BridgeImplementation]` kernel systems without write-write conflict; the validator catches every §7.5 misuse before assembly load; the pipeline graph build skips replaced kernel systems while still registering mod-supplied replacements.

**Sub-phase status:**

- **M6.1 ✅ Closed.** Acceptance: `[BridgeImplementation]` extended with `Replaceable` bool (per `MOD_OS_ARCHITECTURE` §7.4); Phase 5 combat stubs annotated `[BridgeImplementation(Phase = 5, Replaceable = true)]` (`CombatSystem`, `DamageSystem`, `ProjectileSystem`, `ShieldSystem`, `StatusEffectSystem`, `ComboResolutionSystem`, `CompositeResolutionSystem`); `ContractValidator` Phase H bridge replacement validation emits `BridgeReplacementConflict` (two mods replace same FQN), `ProtectedSystemReplacement` (mod replaces `Replaceable = false` system), and `UnknownSystemReplacement` (mod replaces non-existent FQN); class XML-doc updated to "eight-phase validator" (Phases A–H). Phase 3 carry-over stubs (`SocialSystem`, `SkillSystem`) explicitly verified to remain `Replaceable = false` until M10.C — `Phase5BridgeAnnotationsTests` includes two protected-guard tests that lock this until the Phase 3 carry-over migrates. Commits: `1af73ad` (Phase 5 annotations), `a408f44` (Phase H), `b0f1ee5` (tests). Tests: `PhaseHBridgeReplacementTests` (8), `Phase5BridgeAnnotationsTests` (9 — 7 Replaceable bridges + 2 protected-system guards).

- **M6.2 ✅ Closed.** Acceptance: `ModIntegrationPipeline.CollectReplacedFqns` helper builds the set of replaced kernel-system FQNs from all loaded mods' `Manifest.Replaces` lists; pipeline graph build skips kernel `SystemOrigin.Core` systems whose FQN is in the replaced set; mod-supplied replacement systems register normally per the existing flow (no special path); integration tests cover all four §7.5 scenarios end-to-end at the pipeline level (Replaceable success, Protected reject, Unknown reject, Conflict reject). Commits: `23f2933` (pipeline skip wiring), `602a84e` (helper unit tests), `adad506` (integration scenarios + replacement fixtures). Tests: `CollectReplacedFqnsTests` (5), `M62IntegrationTests` (5).

**Consumes decisions:** strategic lock 2 (explicit `replaces`), D-2 (capability cross-check — Phase H reuses M3 capability resolution semantics for replacement systems).

**Acceptance criteria met (per `MOD_OS_ARCHITECTURE` §11.1):**

- A mod replacing a `Replaceable = true` bridge: bridge skipped at graph build, mod system runs (M6.1 + M6.2).
- Two mods replace the same bridge: batch rejected with `BridgeReplacementConflict` (M6.1 validator-level; M6.2 pipeline-level).
- A mod replaces a `Replaceable = false` system: rejected with `ProtectedSystemReplacement` (M6.1 validator-level; M6.2 pipeline-level).
- A mod replaces a non-existent FQN: rejected with `UnknownSystemReplacement` (M6.1 validator-level; M6.2 pipeline-level).
- Replacement system passes the M3 capability cross-check unchanged (M6 introduces no capability-layer change; replacement systems flow through Phase C/D as ordinary mod systems).

**Unblocks:** M9, M10 (vanilla mods declare `replaces` for their slice). On-unload re-registration of skipped kernel bridges is handled by M7's hot-reload path — at M6 closure, `Apply` rebuilds the graph from the surviving mod set on every call, so the unload case reduces to "re-`Apply` without the unloaded mod" once M7 lands.

---

### ✅ M7 — Hot reload from menu (closed)

Goal achieved: hot-reload path through `ModIntegrationPipeline` complete, including `AssemblyLoadContext` unload with WeakReference verification. Decomposed per [METHODOLOGY](/docs/methodology/METHODOLOGY.md) §2.4 into seven implementation sub-phases (M7.1, M7.2, M7.3, M7.4, M7.5.A, M7.5.B.1, M7.5.B.2) plus a closure session, with 6 housekeeping passes interleaved.

**Sub-phase status:**

- **M7.1 ✅ Closed.** Acceptance: `ModIntegrationPipeline._isRunning` private bool (default `false`, "paused"); public `IsRunning` getter; idempotent `Pause()` and `Resume()` setters; `Apply` and `UnloadAll` guard against running invocation by throwing `InvalidOperationException` with the canonical §9.3 messages "Pause the scheduler before applying mods" / "Pause the scheduler before unloading mods" (verbatim — asserted as exact-string matches at the test level so any paraphrase trips the suite). Default-paused construction is load-bearing: every M0–M6 test constructs a fresh pipeline and calls `Apply` without ever touching `Pause`/`Resume`, and the new guard must be a no-op for that pre-existing path. Commits: `a2ab761` (pipeline state additions + guards + class XML-doc paragraph), `c964475` (`M71PauseResumeTests` × 11). Tests: `M71PauseResumeTests` (11 — default state, Pause/Resume transitions, idempotency, Apply guard with verbatim canonical message, UnloadAll guard with verbatim canonical message, paused-Apply / paused-UnloadAll happy paths, Resume→Pause→Apply round-trip, default-paused regression guard).

- **M7.2 ✅ Closed.** Acceptance: `LoadedMod.Api` retains the `RestrictedModApi` issued by `Apply` step [4] (added as a non-positional record member so every M0–M6 `new LoadedMod(...)` call site keeps compiling); `ModIntegrationPipeline.UnloadMod(string modId): IReadOnlyList<ValidationWarning>` implements §9.5 steps 1–6 (UnsubscribeAll → RevokeAll → RemoveMod → graph-rebuild-and-swap → ALC.Unload); each step is wrapped in a private `TryUnloadStep` helper that catches exceptions, records a `ValidationWarning` with `(modId, stepNumber)` and continues to the next step per §9.5.1 best-effort failure discipline; the mod is removed from the active set regardless of step outcome. `UnloadMod` mirrors M7.1's run-flag guard verbatim (`"Pause the scheduler before unloading mods"`) and is idempotent for non-active mods (returns empty warnings, no throw). `UnloadAll` is refactored to delegate to `UnloadMod` per active mod and accumulate the per-mod warnings; bulk-unload semantics, the empty-active-set kernel-only rebuild, and the M7.1 run-flag guard are all preserved (verified by regression assertions in the test class). Step 7 (`WeakReference` spin loop with GC pump per v1.4 §9.5 step 7) is deferred to M7.3 alongside the Phase 2 carried-debt unload tests; M7.2's chain stops at step 6. Commits: `2531ed7` (LoadedMod.Api + Apply step [4] retention + UnloadMod + TryUnloadStep + UnloadAll refactor + class XML-doc paragraph), `d68ba93` (`M72UnloadChainTests` × 13). Tests: `M72UnloadChainTests` (13 — run-flag guard, idempotent non-active mod, per-step verification of steps 1–6, step-2-throws seam via `ThrowingRevokeAllContractStore` decorator, mod-removed-from-active-set-on-failure, warning shape, `UnloadAll` accumulator, empty-active-set kernel-only rebuild, M7.1 guard preservation).

- **M7.3 ✅ Closed.** Acceptance: `ModIntegrationPipeline.UnloadMod` extends the §9.5 chain past M7.2's step 6 with the §9.5 step 7 protocol — captures a `WeakReference` to the mod's `ModLoadContext` via a non-inlined `CaptureAlcWeakReference` helper, removes the mod from `_activeMods` per §9.5.1, then spins on `WeakReference.IsAlive` for up to 10 s (100 × 100 ms cadence) inside the non-inlined `TryStep7AlcVerification` helper which runs the mandatory `GC.Collect → WaitForPendingFinalizers → Collect` double-collect bracket each iteration. On timeout the chain appends a `ValidationWarning` whose text contains `"ModUnloadTimeout"`, the modId, `"§9.5 step 7"`, and `"10000 ms"` (substring contract for the future menu UI). Steps 1–6 + WR capture + active-set removal are extracted into a dedicated non-inlined `RunUnloadSteps1Through6AndCaptureAlc` method so the `LoadedMod` local — and the compiler-generated display class hoisting it for the step-1 lambda's `mod.Api?.UnsubscribeAll()` capture — live only inside that helper's stack frame; without this split, in DEBUG the lifted display class persists until `UnloadMod` returns and the spin times out (verified empirically against the M7.2 step-2-throws regression suite). `UnloadAll`'s snapshot loop moves to a non-inlined `SnapshotActiveModIds` helper for the same JIT-stack-frame reason: the `foreach` iteration variable that holds each `LoadedMod` retains the last value through the remainder of `UnloadAll`'s frame in DEBUG. Adds an internal `GetActiveModForTests` test seam — mirroring `CollectReplacedFqnsForTests` — for Phase 2 carried-debt closure tests to capture a `WeakReference` against real-mod fixtures. Step 7 always runs after the step 6 attempt regardless of step 6 outcome (per AD #7); when an upstream step throws, both warnings (step failure + step 7 timeout) accumulate in the returned list. Phase 2 carried debt closed via `M73Phase2DebtTests` against `Fixture.RegularMod_DependedOn` + `Fixture.RegularMod_ReplacesCombat` — both pass within timeout, zero flakes across 3 consecutive runs (§11.4 invariant). Commits: `9bed1a4` (step 7 implementation + helper extraction + `GetActiveModForTests` test seam + class XML-doc + UnloadMod method docstring), `46b4f33` (`M73Step7Tests` × 5 + `M73Phase2DebtTests` × 2 + `ModUnloadAssertions` helper). Tests: `M73Step7Tests` (5 — happy-path step 7 release on empty in-memory ALC, ALC-retainer timeout path emits ModUnloadTimeout, canonical warning shape locked at substring level, AD #7 step-7-after-upstream-failure invariant via the existing M7.2 step-2 throwing seam, mod-removed-from-active-set after timeout) and `M73Phase2DebtTests` (2 real-mod fixtures via `pipeline.Apply` — `Fixture.RegularMod_DependedOn` minimal surface, `Fixture.RegularMod_ReplacesCombat` with system registration + bridge replacement; both assert `ModUnloadAssertions.AssertAlcReleasedWithin` after `UnloadMod` returns).

- **M7.4 ✅ Closed.** Acceptance: D-7 build-pipeline override is implemented as a hybrid of a standalone .NET console tool plus an MSBuild target. `tools/DualFrontier.Mod.ManifestRewriter/` exposes a `public static class ManifestRewriter` with `Rewrite(string manifestPath): Result` (`Rewritten` / `AlreadyFalse` / `FieldAbsent` / `NotFound` / `ParseError` / `WriteError`) and a thin `Program.Main` CLI wrapper (`dotnet ManifestRewriter.dll --path <m.json>`; exit 0 on the three success values, exit 1/2/3 for the failure values; one-line stderr diagnostic on failure). JSON tooling on `System.Text.Json.Nodes` (BCL only — matches `ManifestParser`'s existing dependency surface; no Newtonsoft.Json introduced). `mods/Directory.Build.targets` defines the override target gated on `<IsVanillaMod>true</IsVanillaMod>` (default `false`, so the existing `DualFrontier.Mod.Example` is unaffected) plus `$(Configuration)=='Release'` for the rewrite step itself; the conditional `<None CopyToOutputDirectory="PreserveNewest">` for `mod.manifest.json` is gated on `IsVanillaMod` only, so Debug builds also receive a manifest copy in bin (just never rewritten). Source preservation discipline locked: the rewriter operates on `bin/{Configuration}/{TFM}/mod.manifest.json` after `CopyToOutputDirectory`; the source manifest at the project root is never modified. Idempotency contract per AD #7: `hotReload: true` → flipped to `false`; `hotReload: false` → byte-identical no-op; field absent → byte-identical no-op (the rewriter does NOT add the field — adding it would silently change document shape and break round-trip equality for non-vanilla cases relying on the §2.2 default). Build-order discipline via `<ProjectReference Include="...ManifestRewriter.csproj" ReferenceOutputAssembly="false" Private="false">` on every vanilla mod project: the tool builds before the consuming mod, but its assembly does not leak into the mod's bin output (verified in fixture). Test fixture `Fixture.VanillaMod_HotReloadOverride/` ships a minimal vanilla-mod-shaped artifact (trivial `IMod`, v2 manifest with `hotReload: true`, `<IsVanillaMod>true</IsVanillaMod>`, import of `mods/Directory.Build.targets`) and is exercised by the integration suite via `dotnet build -c {Release,Debug}` subprocess; integration tests resolve the repo root by walking up from `AppContext.BaseDirectory` until `DualFrontier.sln` is seen, then locate the fixture csproj. §11.4 zero-flake invariant verified across 3 consecutive runs at closure time. Commits: `5385fe5` (rewriter tool + `mods/Directory.Build.targets` + solution wiring + manual CLI verification), `8d51a9d` (`ManifestRewriterTests` × 7 + `Fixture.VanillaMod_HotReloadOverride` + `M74BuildPipelineTests` × 2). Tests: `ManifestRewriterTests` (7 — Rewritten happy path + all-fields-preserved round-trip, AlreadyFalse / FieldAbsent byte-identical no-ops, NotFound / ParseError failure semantics, idempotency under repeat invocation) + `M74BuildPipelineTests` (2 — Release build flips bin manifest to `hotReload: false` and preserves source; Debug build leaves both source and bin at `hotReload: true`).

- **M7.5.A ✅ Closed.** Acceptance: `ModMenuController` (internal sealed) encapsulates the menu-side editing-session lifecycle per §9.2 — `BeginEditing` snapshots the active set + discovered set and calls `Pipeline.Pause`; `Toggle(modId)` mutates only the pending set with §9.6 enforcement (currently-active mods with `hotReload: false` rejected via `ToggleResult.RejectedHotReloadDisabled`, defensive guard mirrored by the `CanToggle` UI hint); `Commit` computes the diff (added paths, removed ids), runs per-removed `Pipeline.UnloadMod` then a single `Pipeline.Apply` for added paths, calls `Pipeline.Resume` only on full success, returns aggregated `CommitResult`; `Cancel` discards pending state and resumes the simulation. Idempotency contract per AD #6: `BeginEditing` while editing → silent no-op; `Cancel` while not editing → silent no-op (does NOT call `Resume`, so a stray UI Cancel cannot accidentally start the scheduler); `Commit` / `GetEditableState` while not editing → throw `InvalidOperationException`; `Toggle` while not editing → `ToggleResult.NoSession`. `IModDiscoverer` abstraction with `DefaultModDiscoverer` production implementation (best-effort directory scan — non-existent root returns empty for first-launch safety, missing manifests skipped, per-manifest parse failure swallowed silently); tests inject a `FakeModDiscoverer` returning a hand-rolled list. New `ModIntegrationPipeline.GetActiveMods()` public read API returning `IReadOnlyList<ActiveModInfo>` — fresh-list snapshot per call, lets the controller build the editing-session view without exposing internal `LoadedMod`. Public DTOs (`ActiveModInfo`, `DiscoveredModInfo`, `EditableModInfo`, `CommitResult`, `ToggleResult`) live in `src/DualFrontier.Application/Modding/`, one class per file, English XML doc throughout. `DualFrontier.Core` and `DualFrontier.Contracts` untouched (verified via `git diff` returning empty). Bootstrap wiring (exposing controller from `GameBootstrap` to `GameRoot`) is M7.5.B territory. Commits: `9c895fe` (8 new files in `src/DualFrontier.Application/Modding/` + `Pipeline.GetActiveMods` extension), `4c648c6` (`ModMenuControllerTests` × 22 + `DefaultModDiscovererTests` × 4 + `PipelineGetActiveModsTests` × 4). Tests: `ModMenuControllerTests` (22 — Begin pause behavior + AD #6 idempotency, Cancel resume + idempotency, Toggle pending-set mutation + §9.6 RejectedHotReloadDisabled + AD #5 first-load-is-not-reload + NoSession + UnknownMod, CanToggle UI hint mirror + defensive-default-without-session, GetEditableState combined active+discovered rows with flags + throws-without-session, Commit no-op-success + add-only + remove-only + add-and-remove + AD #4 failure-stays-paused-session-open + retry-recovery + throws-without-session) + `DefaultModDiscovererTests` (4 — non-existent root empty, manifest-less subdir skipped, valid manifest parsed, broken manifest swallowed) + `PipelineGetActiveModsTests` (4 — empty-pipeline empty list, post-Apply contents with manifest preserved verbatim, post-UnloadMod removal, fresh-list snapshot not live view).

- **M7.5.B.1 ✅ Closed.** Acceptance: `GameBootstrap.CreateLoop` signature is now `public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")` — returns a new `internal sealed record GameContext(GameLoop Loop, ModMenuController Controller)` carrying both the simulation loop and the menu-side controller. The bootstrap body constructs the full modding stack (`ModLoader`, `ModRegistry` seeded with the nine kernel `SystemBase` instances, `ContractValidator`, `ModContractStore`, `ModIntegrationPipeline`, `DefaultModDiscoverer`, `ModMenuController`) atop the same kernel scheduler / `IGameServices` aggregator. The hard-coded inline `graph.AddSystem(new NeedsSystem())` etc. calls are refactored into a local `SystemBase[] coreSystems` array first so the same instances flow into both `graph.AddSystem` (kernel scheduling) and `ModRegistry.SetCoreSystems` (mod-validation/replacement visibility per AD #5 of the M7.5.B.1 prompt, matching the M7.2 test pattern). Per AD #7 the pipeline is left in its default paused state (M7.1 load-bearing default) — bootstrap does NOT call `Pause`/`Resume`; the menu (M7.5.B.2) drives the §9.2 Pause-Toggle-Apply-Resume sequence. Per AD #6 the same `IGameServices` instance is threaded through both the kernel scheduler and the modding pipeline. `GameRoot._Ready` consumes the new return shape via `GameContext context = GameBootstrap.CreateLoop(_bridge); _loop = context.Loop; _modMenuController = context.Controller;` — the `_modMenuController` field is held for M7.5.B.2 to bind to the Godot UI scene without re-constructing the bootstrap context. M-phase boundary discipline preserved: `git diff` against `src/DualFrontier.Core` and `src/DualFrontier.Contracts` returns empty. `dotnet sln list` unchanged from the M7.5.A baseline. Commits: `4956a13` (`GameContext.cs` + `GameBootstrap.cs` refactor + `GameRoot.cs` consumer adjustment + new `using` directives + XML docs), `94128be` (`GameBootstrapIntegrationTests` × 7). Tests: `GameBootstrapIntegrationTests` (7 — `ReturnsContextWithLoopAndController` smoke test of the new return shape, `ReturnedController_BeginEditingSucceedsAndPauses` controller-surface functional test, `WithEmptyModsRoot_GetEditableStateReturnsEmpty` happy-path empty case, `WithModsRootContainingFixture_GetEditableStateReturnsFixture` end-to-end discovery through production wiring with a single valid manifest, `WithNonExistentModsRoot_GetEditableStateReturnsEmpty_NoThrow` first-launch safety lock, `DefaultModsRoot_IsLiteralStringMods` AD #3 reflection lock on the default parameter value, `ReturnedLoop_StartStopRoundTripsCleanly` regression floor on the new `GameContext` shape).

- **M7.5.B.2 ✅ Closed.** Acceptance: new `src/DualFrontier.Presentation/UI/ModMenuPanel.cs` is a code-only `Godot.Control` subclass mirroring the Phase 4 UI precedent (`ColonyPanel` / `PawnDetail` / `GameHUD` — all layout in `_Ready()`, no `.tscn` editor authoring) per AD #1. F10 toggles modal visibility via `GameRoot._UnhandledInput` per AD #2 (open path calls `controller.BeginEditing` then `RebuildList` then `Visible = true`; close-while-open path calls `controller.Cancel` then `Visible = false`); `InputRouter.cs` ESC handling stays unchanged (AD #2). Modal full-screen overlay (`MouseFilter = Stop`, `ZIndex = 100`, `Visible = false` on construction) covers the viewport with a 60 %-alpha dim background plus a 500 × 420 centered `Panel` containing the actual UI per AD #4. Empty mods/ → "No mods found in mods/" muted Label per AD #5 (honest signal — discoverer ran, found nothing, no fabrication). Per-mod row layout per AD #6: `HBoxContainer` with `Name v{Version}` Label (`SizeFlagsHorizontal = ExpandFill`) on the left and a `CheckBox` on the right (`ButtonPressed = info.IsPendingActive`); when `info.CanToggle == false` (per §9.6 hot-reload disabled), `CheckBox.Disabled = true` and `TooltipText = "Hot-reload disabled — restart required to load/unload this mod"`. CheckBox `Toggled` handler routes through `controller.Toggle(modId)` per AD #7 — `RejectedHotReloadDisabled` / `UnknownMod` / `NoSession` revert the checkbox via `SetPressedNoSignal(priorState)` and surface the rejection message in the status label. Apply button calls `controller.Commit()` per AD #8 — success branch hides the panel; failure branch joins `result.Errors[*].Message` via `\n` into the status label and leaves the panel open per the M7.5.A AD #4 deliberate-interpretation contract (failed Commit leaves session open + simulation paused for retry). Cancel button calls `controller.Cancel()` per AD #9 then hides. `Setup(ModMenuController)` is `internal` (controller is `internal sealed`; both projects expose internals to `DualFrontier.Presentation` via existing `InternalsVisibleTo` declarations) per AD #10; null-check on `_controller` makes `OpenAndBegin` / `CloseAndCancel` / button handlers safe between construction and `Setup`. `GameRoot` adds `_modMenuPanel` field, constructs the panel in `_Ready` after `_modMenuController` is obtained from `GameContext`, calls `AddChild` then `Setup(_modMenuController)`, then handles F10 in `_UnhandledInput` (`GetViewport().SetInputAsHandled()` after dispatch) per AD #11. Visual styling reuses `Palette` / `MakePanelStyle` / `ColonyPanel.MakeLabel` per AD #12 (no new theming work). No automated UI test surface per AD #13 (Phase 4 UI widgets have no automated tests in this project — manual F5 verification is the convention); M7.5.B.2's test commit adds **integration tests at controller level** verifying menu-flow sequences. Apply has no enabled-state logic per AD #14 (Commit on a clean session is a no-op success per M7.5.A test 16). M-phase boundary discipline preserved: `git diff` against `src/DualFrontier.Core` and `src/DualFrontier.Contracts` returns empty. `dotnet sln list` count unchanged from the M7.5.B.1 baseline. Commits: `9a75b75` (`ModMenuPanel.cs` + `GameRoot.cs` field + `_Ready` extension + `_UnhandledInput` override), `6ecee53` (3 menu-flow integration tests). Tests: `MenuFlow_OpenCommitClose_LeavesEditingFalse` (Apply success path — `IsEditing` flips true → false on `Commit`), `MenuFlow_OpenCancelClose_LeavesEditingFalse` (Cancel button + `CloseAndCancel` path — `IsEditing` flips true → false on `Cancel`), `MenuFlow_OpenWithoutCommitOrCancel_StaysEditing` (session persists across `GetEditableState` reads when neither Commit nor Cancel is invoked). Manual F5 verification deferred to user — predicted observation: F10 opens the centered modal, F10 closes it, "No mods found in mods/" empty state visible (no vanilla mods discovered until M8), Apply on the unchanged session closes without errors, Cancel closes, simulation continues underneath via the dim overlay, ESC continues to quit (InputRouter unchanged).

  - **Follow-up housekeeping** (commits `5f0b4f5`, `805b882`, this docs commit):
    - `ModMenuPanel` converted from `Control` to `CanvasLayer` (Layer 20) to fix the modal misposition surfaced during F5 verification — Phase 4 UI pattern conformance (`GameHUD` Layer 10 + Control children pattern). Internal `_root` Control wraps the dim overlay + centered panel + interactive widgets and supplies the full-screen anchor space the modal needs. `GameRoot.cs` unchanged (CanvasLayer-vs-Control parent type difference is invisible from `GameRoot`'s `AddChild` / `Visible` access). Public method signatures (`Setup`, `OpenAndBegin`, `CloseAndCancel`) unchanged.
    - Extracted Kenney UI packs (`assets/kenney_*/`) and Cinzel font (`assets/cinzel/`) added to `.gitignore`; source `.zip` files remain the in-git SoT (already covered by the existing `*.zip` rule). Foundation prep for the upcoming UI redesign brief.

  - **Second follow-up housekeeping** (commits `9f87536`, `d8a448f`, this docs commit):
    - Menu open now actually pauses the background simulation thread.
      Surface gap diagnosed during F5 verification: TICK counter advanced
      ~250 ticks while menu held open across two screenshots; root cause
      was two independent pause flags (`ModIntegrationPipeline._isRunning`
      for Apply-mutation safety, `GameLoop._paused` for tick advance) where
      `ModMenuController.BeginEditing` only toggled the former. Fix wires
      `OnEditingBegan` / `OnEditingEnded` hooks on the controller from
      `GameBootstrap.CreateLoop` so the orchestration layer calls
      `GameLoop.SetPaused` in lockstep with `pipeline.Pause`/`Resume`.
      Failed-commit stays paused per M7.5.A AD #4 (success-path branch
      fires `OnEditingEnded`; failure-path branch does not). Three new
      integration tests (`MenuFlow_BeginEditing_PausesGameLoop`,
      `MenuFlow_Cancel_ResumesGameLoop`, `MenuFlow_CommitSuccess_ResumesGameLoop`)
      lock the contract; test count 434 → 437/437.
    - **§9.2 v1.6 ratification candidate flagged.** Spec wording «menu
      pauses the scheduler» reads as a single entity; implementation has
      two distinct pause surfaces (`ModIntegrationPipeline._isRunning` for
      Apply mutation safety, `GameLoop._paused` for tick advance). The
      orchestration-layer wiring fix is correct given current architecture,
      but the spec section should either explicitly enumerate the two
      surfaces or refactor toward a unified pause contract — to be
      addressed in M7-closure or its own ratification cycle. Hypothesis-
      falsification result: M7.5.B.2 closure claimed «§9 contradiction
      status: zero», F5 manual verification falsified that — datapoint
      sequence revised to M7.5.B.2 = 1 (§9.2 wording vs. implementation),
      M7-closure formalizes the v1.6 cycle.

- **M7-closure ✅ Closed.** Sub-phase closure session — ROADMAP M7 row updated to ✅ Closed, engine snapshot at 437/437, M7 closure verification report at [`docs/audit/M7_CLOSURE_REVIEW.md`](./audit/M7_CLOSURE_REVIEW.md). All 8 closure checks PASSED. §7.5 fifth scenario M6→M7 hand-off resolved via outcome (B) Structural verification (M7.2 `UnloadMod` + M6.2 `CollectReplacedFqns` parameter-driven design close the property via disjoint tests). Phase 2 WeakReference unload-test carried debt CLOSED via M7.3. §9.2 v1.6 ratification candidate registered in §10 of the closure review for future cycle (the «menu pauses the scheduler» wording-vs-implementation gap surfaced via F5 manual verification). Six surgical fixes applied to sync stale `v1.4 LOCKED` and stale `v1.5 ratification` references after mid-batch v1.5 was consumed by audit campaign Pass 2. M7 closes; M8 (Vanilla skeletons) is unblocked.

**Deliberate interpretation of §9.2 / §9.3 — flag location on the pipeline, not the scheduler.** §9.2 step 1 reads "menu sets the scheduler's run flag to false"; §9.3 reads "enforced by `ModIntegrationPipeline` checking the scheduler's run flag." M7.1 locates the flag itself on `ModIntegrationPipeline` (private `_isRunning` bool) rather than introducing one inside `ParallelSystemScheduler`. Adding a flag to the scheduler would require modifying `DualFrontier.Core`, which would break the M-phase boundary discipline that M3–M6 maintained (no `DualFrontier.Core` touched by any Mod-OS Migration phase) and which the [M6 closure review](./audit/M6_CLOSURE_REVIEW.md) §8 footer explicitly carries forward to M7. The pipeline-mediated reading is consistent with §9.3's "`ModIntegrationPipeline` checking" wording and treats §9.2's "scheduler's run flag" as the run state observable to the scheduler from the outside (via the pipeline), rather than as state the scheduler itself owns. This is a deliberate interpretation registered here per [METHODOLOGY](/docs/methodology/METHODOLOGY.md)'s "no improvisation" rule and the M5.2 cascade-failure precedent above. If a future M7 closure review finds this materially incompatible with §9 wording, the resolution is a v1.6 ratification rather than relocating the flag into the scheduler (v1.5 was consumed by audit campaign Pass 2).

**Deliberate interpretation of §9.5 / §9.5.1 — step 7 ordering: capture WR → remove from active set → spin.** §9.5 step 7 specifies the spin protocol but does not pin the order of `_activeMods` removal relative to WR capture and spin entry; §9.5.1 says "the mod is removed from the active set regardless of whether the assembly actually unloaded." M7.3 wires the order as `CaptureAlcWeakReference(mod) → _activeMods.Remove(mod) → TryStep7AlcVerification(modId, wr, warnings) → return`. The capture must precede the removal so the WR is bound to the same `ModLoadContext` instance the active-set removal then helps release; the spin must follow the removal so the pipeline-side strong reference (`_activeMods`) is gone before the spin's GC pumps run. This is consistent with §9.5.1's "removed regardless" wording (the removal does not wait for spin success) and with §9.5 step 7's spin running on a captured WR with no requirement to keep the mod in the active set. Registered here per the M7.1 §9.2/§9.3 footer interpretation precedent. If a future M7 closure review finds this materially incompatible with §9 wording, the resolution is a v1.6 ratification rather than silent reordering (v1.5 was consumed by audit campaign Pass 2).

**Deliberate interpretation of §9.2 — failed `Commit` leaves the simulation paused and the editing session open (M7.5.A AD #4).** §9.2 specifies the success path of the menu-driven hot-reload flow (Pause → Toggle → Apply → Resume) but does not pin behavior when `Pipeline.Apply` returns `Success: false`. M7.5.A interprets this as: when validation fails inside `Commit`, `ModMenuController` does NOT call `Pipeline.Resume` and does NOT close the editing session — the controller leaves `IsRunning == false` and `IsEditing == true` so the user can fix the pending state (un-toggle the offending mod, edit a version) and re-invoke `Commit` against the same session. Only successful `Commit` and any `Cancel` resume the simulation. This is consistent with §9.2's success-path wording (Resume is the menu's step 4, not a step that runs unconditionally) and with the menu-driven model where the user, not the controller, decides whether to abandon or retry a failed apply. A companion AD #5 records that §9.6 ("cannot be reloaded mid-session") plus §2.2 ("loads only at session start") combine to allow a discovered-only `hotReload: false` mod to be added inside the editing session — first-load is not a reload. Registered here parallel to the M7.1 §9.2/§9.3 footer and the M7.3 §9.5 step 7 ordering footer. If a future M7 closure review finds either reading materially incompatible with §9 wording, the resolution is a v1.6 ratification rather than silent semantic drift (v1.5 was consumed by audit campaign Pass 2).

**Consumes decisions:** strategic lock 3 (menu-driven, paused-only), D-7 (vanilla `hotReload` flag, build-pipeline override).

**What we implement (M7 in full — landed across M7.1–M7.5)**

- `ModIntegrationPipeline.Pause()` and `Resume()` setting the pipeline-mediated run flag (M7.1 ✅). `Apply` checks the flag and throws `InvalidOperationException("Pause the scheduler before applying mods")` if invoked while running (M7.1 ✅); `UnloadAll` mirrors the guard with the parallel "Pause the scheduler before unloading mods" message (M7.1 ✅).
- ALC unload chain:
  1. `RestrictedModApi.UnsubscribeAll` (drops bus subscriptions).
  2. `IModContractStore.RevokeAll(modId)`.
  3. `ModRegistry.RemoveSystems(modId)`.
  4. Dependency graph rebuilt without the mod.
  5. Scheduler swap.
  6. `ALC.Unload()`.
  7. WeakReference spin loop: poll `WeakReference.IsAlive` until `false` or 10-second timeout; timeout escalates to `ModUnloadTimeout` warning.
- WeakReference unload tests (originally Phase 2 backlog, now hard requirement): every regular mod loaded in tests must unload cleanly. Test fixture provides the `WeakReference` and asserts `false` within timeout.
- Mod-menu UI: list active mods, toggle, version display, "Apply" button. Hot-reload button disabled for mods with `hotReload: false`.
- Build-pipeline override (per D-7): a build-time tool rewrites `hotReload: true` to `hotReload: false` for every vanilla manifest in shipped builds. Single flag in build script, no code branching.

**Acceptance criteria**

- User pauses simulation via menu, toggles a mod, hits Apply: target mod loads/unloads, scheduler rebuilds, simulation resumes from the same world state.
- A mod with `hotReload: false` cannot be reloaded mid-session; menu shows tooltip directing user to restart the game.
- Every regular mod under test passes the WeakReference unload check within 10 seconds; failures fail the test (no flakiness tolerated).
- Calling `Apply` while the scheduler is running throws `InvalidOperationException` with the canonical message.
- A mod that retains a static reference to a kernel object after `Unload` triggers `ModUnloadTimeout` and is reported by id.

**Unblocks:** M8 onwards. Vanilla mods authored with `hotReload: true` for development; shipped builds override.

---

### M8 — Vanilla mod skeletons

Goal: create the directory structure and empty manifests for the five vanilla mods so subsequent M-phases have load targets.

**Consumes decisions:** strategic lock 4 (vanilla split into slices).

**What we implement**

- `mods/DualFrontier.Mod.Vanilla.Core/` — shared mod (`kind: "shared"`) for cross-slice types: `WeaponDef`, `RecipeDef`, common enums, helper records. Initial set is small; types are added as M9/M10 demand.
- `mods/DualFrontier.Mod.Vanilla.Combat/` — regular mod, depends on `Vanilla.Core`. Empty `IMod.Initialize`. `replaces` lists `CombatSystem`, `DamageSystem`, `ProjectileSystem`, `ShieldSystem`, `StatusEffectSystem` (eventually).
- `mods/DualFrontier.Mod.Vanilla.Magic/` — same shape.
- `mods/DualFrontier.Mod.Vanilla.Inventory/` — same shape (reuses Phase 4 `InventorySystem` once migrated).
- `mods/DualFrontier.Mod.Vanilla.Pawn/` — same shape; will absorb `SocialSystem`, `SkillSystem`.
- `mods/DualFrontier.Mod.Vanilla.World/` — same shape.
- Each mod's `.csproj` references only `DualFrontier.Contracts` and any shared-mod dependencies (typically `Vanilla.Core`).

**Acceptance criteria**

- All five mods build without warnings.
- Loading the full vanilla mod set into `ModIntegrationPipeline` succeeds: all manifests parse, dependency graph builds, scheduler rebuilds with the mods registered (still empty `Initialize`).
- Smoke test: kernel runs one tick with vanilla mods loaded; no exceptions; no system actually does anything yet (empty `Initialize`).
- Vanilla manifest `hotReload: true` in source; build-pipeline override correctness verified by a CI test.

**Unblocks:** M9, M10. From this point, new gameplay code lands inside vanilla mod assemblies.

---

### M9 — Vanilla.Combat real implementation

Goal: implement the gameplay scope originally planned for Phase 5 — but inside `Vanilla.Combat` rather than `DualFrontier.Systems.Combat`. The kernel `[BridgeImplementation]` stubs become bypassed at scheduler build via `replaces`.

**Consumes decisions:** strategic locks 1, 2, D-1, D-2.

**What we implement**

- Move the scope of `CombatSystem`, `DamageSystem`, `ProjectileSystem`, `ShieldSystem`, `StatusEffectSystem` from kernel bridges to `Vanilla.Combat` mod systems. Public surface (event types, component types) stays in kernel; only the system implementations move.
- Two-step `AmmoIntent → AmmoGranted` model through `IntentBatcher`. The composite shot pattern (TechArch v0.2 §12.4) — `CompoundShotIntent` → `CompositeResolutionSystem` → `ShootGranted`/`ShootRefused` — implemented end-to-end.
- Damage model: damage types (Heat, Sharp, Blunt, EMP, Toxic, Psychic, Stagger) with armour accounting per [GDD] §6.1.
- Shields: HP pool, regeneration, weaknesses.
- `DeathEvent` published `[Deferred]`; subscribers (mood, social, eventually faction) receive on next tick.
- `Vanilla.Core` shared mod populated with `WeaponDef`, `AmmoDef`, `DamageProfile` records used by combat. Vanilla weapons (rifle, pistol, sword, ammo types) registered via `PublishContract` in `IMod.Initialize`.
- Bridge stubs in `DualFrontier.Systems.Combat` retain their `[BridgeImplementation(Phase = 5, Replaceable = true)]` annotations; `Vanilla.Combat` manifest lists them in `replaces`.

**Acceptance criteria**

- Loading game with `Vanilla.Combat` enabled: combat works, pawns shoot, take damage, die. `DeathEvent` propagates.
- Disabling `Vanilla.Combat`: kernel bridges re-register, combat goes silent (pawns equipped with weapons do not shoot — bridges are no-op). The game does not crash; this is the architectural baseline for "an engine that ships without combat."
- All capability checks pass: `Vanilla.Combat` declares every capability honestly in its manifest.
- 100 pawns × 60 ticks × ammo request: ≤ 100 scans/sec (the original Phase 5 target from §11.11 of architecture; preserved here unchanged).

**Unblocks:** M10. Magic and other slices follow the same pattern with confidence the migration path works.

---

### M10 — Remaining vanilla slices: Magic, Inventory, Pawn, World

Goal: incremental delivery of the four remaining vanilla mods, in any order the development pipeline finds useful. Each follows the M9 template.

**Consumes decisions:** locks already in effect from M0–M9.

**M10.A — Vanilla.Magic.** Consumes original Phase 6 scope. Eight schools of magic per [GDD] §6.1. Five levels of ether perception per §4.1. Five golem types per §5.1. Combo mechanics per §6.3. Replaces kernel `ManaSystem`, `SpellSystem`, `GolemSystem`, `EtherGrowthSystem`, `RitualSystem` bridges. Cross-slice: depends on `Vanilla.Combat` for damage profiles, on `Vanilla.Inventory` for crystal storage.

**M10.B — Vanilla.Inventory.** The Phase 4 `InventorySystem` and `HaulSystem` migrate from kernel to mod. This is a **refactor migration**, not new gameplay; covered by existing tests. *(The v0.3 power-grid CPU subsystems — `ElectricGridSystem`, `ConverterSystem`, `IPowerBus`, `PowerConsumerComponent`, `PowerProducerComponent`, and the four power events — were deleted in A'.5 K8.3+K8.4 cutover 2026-05-14. Electricity-like mechanics are routed toward V substrate field/compute work per [VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md) §1.2 + §5.1; see M-V2 reservation.)*

**M10.C — Vanilla.Pawn.** Consumes Phase 3 backlog: `SocialSystem` and `SkillSystem` get real implementations inside the mod. The kernel `JobSystem`, `MoodSystem`, `MovementSystem`, `NeedsSystem`, `PawnStateReporterSystem` migrate as part of the same refactor.

**M10.D — Vanilla.World.** Consumes original Phase 7 scope. Biomes affect resources, weather, passability. Ether nodes form coverage radii. Inter-faction relations per [GDD] §3.3. Raids by colony level. Trade caravans by season. Replaces kernel `BiomeSystem`, `WeatherSystem`, `MapSystem`, `RaidSystem`, `RelationSystem`, `TradeSystem` bridges.

**Acceptance criteria** (per slice, applied uniformly)

- All gameplay scope from the original Phase 5/6/7 spec delivered as mod content.
- Disabling the slice mod: the kernel runs with that domain silent; no crashes.
- The slice's `replaces` list covers every kernel bridge it supersedes; capability cross-check passes; test count grows with each slice.
- Inter-slice dependencies (e.g. Magic depends on Combat for damage types) declared via `dependencies` with caret-versioned constraints.

**Unblocks:** the full game. Subsequent expansions are content mods (community or first-party), shipped as additional regular mods that depend on the vanilla slices.

---

## ⏭ Native foundation tracks (K-series + V substrate)

**Composite namespace amendment 2026-05-16:** Per Q-G-1 LOCK (`docs/architecture/COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` §3.1), the former runtime substrate (R) and GPU compute substrate (G) merged into unified Vulkan substrate (V). The original three-track decomposition (K-series + G-series + Runtime M9.x) consolidates to **two tracks** (K-series + V substrate), with rendering and compute as use cases of one Vulkan layer.

The post-Phase-4 architectural pivots committed in [KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md) v1.0 and [VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md) v1.0 (supersedes prior RUNTIME_ARCHITECTURE.md v1.0 + GPU_COMPUTE.md v2.0) decompose the original "Phase 9 — Native Runtime" endpoint into two coordinated tracks. Each track has its own brief; this section is the execution-sequence summary.

### K-series — Native ECS kernel — CLOSED (K0–K10.3 shipped; K8.5 + K10.4 pending)

> **Relocated here 2026-06-02 (DD-2 spec/roadmap separation).** The detailed K-series
> milestone roadmap + migration strategy below were moved verbatim from
> KERNEL_ARCHITECTURE.md Part 2/3, so that kernel document is now pure architecture
> specification. **Status: K0–K10.3 CLOSED** (shipped — native scheduler К10.1, native
> bus К10.2, pipeline/display/quiescent К10.3 v2 all landed). **Pending: K8.5** (mod
> ecosystem migration prep) **+ K10.4** (TLA+ formal verification). The time/LOC
> estimates in the relocated detail are historical pre-execution planning figures
> (most milestones have since shipped); the К-L invariants these milestones produced
> live — and stay authoritative — in KERNEL_ARCHITECTURE.md Part 0.

#### Relocated — Part 2: Roadmap (K-series)

##### Master plan

| Milestone | Title | Estimated time | LOC delta |
|---|---|---|---|
| K0 | Cherry-pick + cleanup от branch | 1-2 days | +1637 (existing) + cleanup |
| K1 | Batching primitive (bulk Add/Get + Span<T>) | 3-5 days | +500-800 |
| K2 | Type-id registry + bridge tests | 2-3 days | +400-600 |
| K3 | Native bootstrap graph + thread pool | 5-7 days | +600-900 |
| K4 | Component struct refactor (Path α) | 2-3 weeks | +/- (mostly conversion) |
| K5 | Span<T> protocol + write command batching | 1 week | +500-700 |
| K6 | Second-graph rebuild on mod change | 3-5 days | +200-400 |
| K7 | Performance measurement (tick-loop) | 3-5 days | +200-400 |
| K8.0 | Architectural decision recording (Solution A) | 1-2 days | +/- (docs only) |
| K8.1 | Native-side reference handling primitives | 1-2 weeks | +600-1000 |
| K8.2 | K-L3 selective per-component closure (post-K-L3.1 reframing): K8.1 wrapper value-type refactor (NativeMap/NativeSet/NativeComposite to readonly struct + IComparable<InternedString> + per-instance id allocation) + 6 class→struct conversions on Path α via K8.1 primitives (Identity/Workbench/Faction/Skills/Storage/Movement) + 6 empty TODO stub deletions per METHODOLOGY §7.1 (Ammo/Shield/Weapon/School/Social/Biome — content deferred to M-series, authored on appropriate path per K-L3.1) + 12 ModAccessible annotation completeness pass on already-struct components | 6-12 hours auto-mode (3-5 days hobby) | +800/-1500 |
| K8.3 | **CLOSED 2026-05-14** (combined w/ K8.4 in A'.5): 10 vanilla systems migrated to SpanLease/WriteBatch (Power 2 deleted as disposable CPU systems — electricity moves to GPU compute) | combined w/ K8.4 | actual -4481/+1211 |
| K8.4 | **CLOSED 2026-05-14** (combined w/ K8.3 in A'.5): ManagedWorld retired from production as `ManagedTestWorld` (test-project fixture); Mod API v3 ships (Path β bridge: `RegisterManagedComponent<T>` + `ManagedStore<T>`); compile-time isolation enforcement (runtime guard removed) | combined w/ K8.3 | (see K8.3 row) |
| K8.5 | Mod ecosystem migration prep | 3-5 days | +500 (docs) |
| K9 | Field storage abstraction (`RawTileField<T>`) | 1-2 weeks | +600-900 |
| K10 | Native kernel scheduler (К-L6 SUPERSEDED + К-L12/L13/L14 AUTHORED in K10.1; K-L15 AUTHORED in K10.2; К-L19 AUTHORED V0.B inheritance; К-L7.1/L16/L17/L18 AUTHORED in K10.3 v2) | 4 sub-milestones (K10.1 = kernel scheduler core CLOSED 2026-05-18; K10.2 = native bus + mod ALC lifecycle CLOSED 2026-05-18; K10.3 v2 = pipeline + display composition + quiescent CLOSED 2026-05-20; K10.4 = TLA+ formal verification pending) | K10.1 ~16 commits +3000-4000 (kernel core); K10.2 ~14 commits +4000-5000 (native bus + ALC); К10.3 v2 ~14 commits (pipeline + display + quiescent); К10.4 future brief |

**Cumulative K0-K8**: 5-8 weeks at hobby pace.
**Cumulative K0-K9**: 6-10 weeks at hobby pace (K9 prerequisite for V substrate primitives per Q-G-2 LOCK).
**Cumulative K0-K10.1**: K10.1 execution closure 2026-05-18 — native scheduler core landed (17 of 46 K10 items). K10 master closure waits for K10.4 (TLA+).
**Cumulative K0-K10.2**: K10.2 execution closure 2026-05-18 — native bus three-tier dispatch + mod ALC lifecycle native primitives landed (25 of 46 K10 items cumulative). K-L15 AUTHORED architecturally; sovereign authority switch deferred к K10.4 / А'.8 per managed-facade-preserved strategy.

**Combined с VULKAN_SUBSTRATE.md R.0-R.8 rendering migration + V0/V1/V2 substrate primitives**: 16-25 weeks total для full architectural foundation. K-series gates K9; K9 gates V substrate primitives. See [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) Roadmap for V0/V1/V2 detail и combined timeline (per Q-G-1/Q-G-2 LOCKs unifying former R-bucket + G-bucket into V substrate).

##### K0 — Cherry-pick + cleanup от branch

**Goal**: experimental branch contents preserved on current main as fresh feature branch. Hygiene fixes applied.

**Source**: `claude/cpp-core-experiment-cEsyH` per `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` Section 11.6.

**Cherry-pick sequence** (7 substantive commits, skipping 4 doc commits):
````
7b5cf78 — CMake scaffold
a8d235e — SparseSet template
cf0eed3 — World + C API
6eac732 — Interop project
80178c2 — Benchmark
f59492a — build files (cleaned)
e2bc2d9 — DLL loading fix
````

**Cleanup commits после cherry-pick**:
- `.gitignore` widening (exclude `native/*/out/`, `BenchmarkDotNet.Artifacts/`, committed `.dll`)
- Dead code removal (`SparseSet<T>` unused — delete OR wire к `RawComponentStore`)
- `.vscode/settings.json` → relative paths или remove
- `NATIVE_CORE.md` superseded marker + reference to KERNEL_ARCHITECTURE.md
- `NATIVE_CORE_EXPERIMENT.md` superseded marker

**Time**: 1-2 days. **LOC**: net +1500 (cherry-pick) - 50 (dead code) = +1450.

##### K1 — Batching primitive

**Goal**: bulk operations + Span<T> access. Validates batching hypothesis quantitatively.

**Deliverables**:
- C ABI extension: `df_world_add_components_bulk`, `df_world_acquire_span`, `df_world_release_span`, `df_world_get_components_bulk`
- Managed bridge: `AddComponents<T>(ReadOnlySpan<EntityId>, ReadOnlySpan<T>)`, `AcquireSpan<T>()`, `WriteCommandBuffer` skeleton
- Native side: span acquisition с atomic counter
- Selftest extension: bulk add scenario, span access scenario
- Benchmark: `NativeBulkAddBenchmark` (target ≤200μs vs current 400μs unbatched, vs managed 218μs)

**Success criteria**:
- Bulk add 10k components в single P/Invoke
- Span<T> iteration zero P/Invoke per element
- Mutation rejection during active span verified
- Validation: zero memory leaks (selftest)

**Time**: 3-5 days. **LOC**: +500-800.

##### K2 — Type-id registry + bridge tests

**Goal**: replace FNV-1a hash с explicit deterministic registry. Comprehensive bridge test coverage.

**Deliverables**:
- `ComponentTypeRegistry` class (sequential IDs, idempotent registration)
- `df_world_register_component_type(type_id, size)` C ABI function
- `DualFrontier.Core.Interop.Tests` project (xUnit, ~30-40 tests)
- Tests cover: NativeWorld CRUD equivalence, packing roundtrip, registry idempotency, span lease invariants, write buffer serialization

**Success criteria**:
- All bridge tests passing
- Hash collision risk eliminated (deterministic IDs)
- 472 + ~30 = ~500 tests passing

**Time**: 2-3 days. **LOC**: +400-600.

##### K3 — Native bootstrap graph + thread pool

**Goal**: declarative startup task graph executed parallel где deps allow. Native scheduler used ONLY for bootstrap.

**Deliverables**:
- `bootstrap_graph.h/cpp` — declarative `kStartupTasks` array, Kahn topological sort
- `thread_pool.h/cpp` — std::thread pool (N cores), work-stealing OR fixed-partitioned
- `df_engine_bootstrap()` C ABI entry point (replaces direct `df_world_create`)
- Selftest extension: bootstrap graph correctness
- Benchmark: `BootstrapTimeBenchmark` (parallel vs sequential)

**Success criteria**:
- Engine bootstraps в ~5-15ms typical hardware
- Parallel tasks demonstrably parallel (e.g. AllocateMemoryPools then InitWorld + InitThreadPool в parallel)
- All bootstrap tasks complete before SignalEngineReady
- Validation clean

**Time**: 5-7 days. **LOC**: +600-900.

##### K4 — Component struct refactor (Path α)

**Goal**: convert all class-based components к `unmanaged` structs.

**Scope**: ~50-80 components в `DualFrontier.Components/`. Each conversion:
- `class XComponent : IComponent` → `struct XComponent : IComponent`
- Update field access patterns
- Update systems that mutate (struct semantics — must use `ref` для mutation)
- Update tests

**Some components may need refactor**:
- Components с complex behavior (methods) — split к pure data + separate behavior class
- Components с reference fields — replace с EntityId references или separate storage

**Time**: 2-3 weeks (substantial scope, mostly mechanical).
**LOC**: +/- (conversion, не net additive).

**Success criteria**:
- All components are `unmanaged` structs
- All systems compile and tests pass
- Allocation profile: zero managed heap allocations during component access
- Existing 472 tests still passing

##### K5 — Span<T> protocol + write command batching

**Goal**: production-grade span lifetime + write batching infrastructure.

**Deliverables**:
- `SpanLease<T>` (`IDisposable`) wrapping native span pointer
- `WriteCommandBuffer` full implementation (Add/Remove/Destroy/Spawn commands)
- Native side: command buffer parser в C++ (parses byte stream from managed)
- `df_world_flush_write_batch(world, buffer, size)` C ABI function
- Native side: mutation rejection during active spans (atomic counter)
- Tests: span lifetime, write batch round-trip, mutation rejection

**Time**: 1 week. **LOC**: +500-700.

##### K6 — Second-graph rebuild on mod change

**Goal**: managed dependency graph rebuilds when mods load/unload. AssemblyLoadContext integration.

**Deliverables** (v1.1 — reconciled with M7-era implementation; pre-M7 wording kept in git history under v1.0):
- Graph rebuild primitive: `DependencyGraph.Reset() + AddSystem + Build()` invoked from `ModIntegrationPipeline.UnloadMod` step 4 and `Apply` steps [5-7]
- `ModLoader.UnloadMod(modId)` per `MOD_OS_ARCHITECTURE.md` §9.5 step 6; reload composition: `Pause + UnloadMod + Apply([newPath]) + Resume`
- Pause-rebuild-resume pattern composed across `GameLoop.SetPaused` and `ModIntegrationPipeline.Pause/Resume/Apply`; gate via `ModIntegrationPipeline.IsRunning` per `MOD_OS_ARCHITECTURE.md` §9.3
- Tests: `M71PauseResumeTests`, `M72UnloadChainTests`, `M73Step7Tests`, `M73Phase2DebtTests`, `RegularModTopologicalSortTests`, plus `M51`/`M52`/`M62` integration tests
- Adjacent debt closed during K6: `ModFaultHandler` implementing `IModFaultSink` (Application-side), wired through `ModLoader.HandleModFault` and `ModIntegrationPipeline` deferred drain

**Time**: 3-5 days. **LOC**: +200-400.

##### K7 — Performance measurement (tick-loop)

**Goal**: representative-load benchmark applying §8 metrics rule.

**Deliverables**:
- `TickLoopBenchmark` — 50 pawns × full component set × 10k ticks
- Variants: managed-current, managed-with-structs (validates К7 conversion value), native-with-batching
- Metrics: p50/p95/p99 tick time, GC pause count + duration, total allocations, drift over time
- Run on weak hardware (Docker cpu-limit container OR secondary machine)
- Report file `docs/reports/PERFORMANCE_REPORT_K7.md` documenting findings

**Time**: 3-5 days. **LOC**: +200-400 (mostly benchmark code).

##### K8 — Production storage cutover (Solution A: NativeWorld backbone)

**Decision** (recorded by K8.0 closure, 2026-05-09; see `docs/MIGRATION_PROGRESS.md` K8.0 closure section): Solution A — single NativeWorld backbone (per K-L11). Choice rationale in Part 4 Decisions log.

**Sub-milestone series**:
- **K8.0** — Architectural decision recording (this milestone; LOCKED v1.2)
- **K8.1** — Native-side reference handling primitives (string interning, keyed maps, composite components)
- **K8.2** — Per-component redesign + K8.1 wrapper value-type refactor + empty TODO stub deletions; K-L3 selective per-component closure achieved (v2 brief, single milestone; post-K-L3.1 reframing)
- **K8.3** — Production system migration (12 vanilla systems → SpanLease/WriteBatch)
- **K8.4** — ManagedWorld retired as production; Mod API v3 ships
- **K8.5** — Mod ecosystem migration prep (documentation + migration guide)

**Cumulative time**: 4-8 weeks at hobby pace.

**LOC delta**: substantial — K8.1 adds ~600-1000 LOC (native + bridge); K8.2 modifies 7 component files plus their consumers; K8.3 modifies 12 system files plus tests; K8.4 deletes managed World production path; K8.5 adds documentation.

##### K9 — Field storage abstraction

**Goal**: native `RawTileField<T>` storage as a parallel abstraction alongside `RawComponentStore`. Prerequisite for the V substrate primitives roadmap ([VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) v1.0 LOCKED — V0/V1/V2 per Q-G-2). Ships CPU functional path first; no Vulkan compute dependency.

**Authoritative spec**: [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) "Architectural integration → Native kernel (KERNEL_ARCHITECTURE.md K9)" + "Roadmap → K9 — Field storage abstraction".

**Deliverables**:
- `RawTileField<T>` C++ class (data + back buffer + conductivity map + storage flags)
- C ABI: `df_world_register_field`, `df_world_field_read_cell`, `df_world_field_acquire_span`, `df_world_field_set_conductivity`, `df_world_field_set_storage_flag`
- Managed bridge: `FieldRegistry`, `FieldHandle<T>` в `DualFrontier.Core.Interop`
- CPU-side reference implementation of basic diffusion (also serves as G1+ shader equivalence oracle and as CPU fallback per [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) "Failure modes → CPU fallback")
- Selftest: round-trip, span access, mutation, conductivity update, storage flag toggle

**Success criteria**:
- Any field type registrable / readable / writeable from managed
- CPU diffusion produces correct results on test grids
- No GPU dependency (G-series can take over later without API churn)

**Time**: 1-2 weeks. **LOC**: +600-900.

---

#### Relocated — Part 3: Migration strategy

**Approach: parallel development through K7; LOCKED commitment from K8.0 onward.**

Managed `World` stayed functional throughout K0-K7. K8.0 closure (2026-05-09) recorded the architectural decision per K-L11: **Solution A — single NativeWorld backbone**. Migration executes via the K8.1-K8.5 sub-milestone series:
- K8.1 — native-side reference handling primitives (string interning, keyed maps, composite components)
- K8.2 — K-L3 selective per-component closure (post-K-L3.1 reframing): K8.1 wrapper value-type refactor + 6 class→struct conversions using K8.1 primitives + 6 empty TODO stub deletions per METHODOLOGY §7.1
- K8.3 — 12 vanilla systems migrated to `SpanLease<T>` reads + `WriteBatch<T>` writes
- K8.4 — managed `World` retired as production path; Mod API v3 ships
- K8.5 — mod ecosystem migration prep (documentation + migration guide)

**Operating principle**: «honest state always available» — managed World stayed working through K0-K7 so the K7 evidence base could be collected; the K-L11 commitment then settles the decision permanently. Reversal trigger documented in `docs/MIGRATION_PROGRESS.md` D5 (Solution A rationale and reversal trigger).

Mirrors VULKAN_SUBSTRATE.md historical migration approach (parallel Godot + Vulkan until R.8 cutover; cutover completed К-extensions cascade #2, 2026-05-23 — Godot paths fully retired).

---

### К-extensions cascades (#0–#5)

Post-К-closure architectural cascades extending К-series invariants beyond the А'.8 closure boundary. Detail authority: [K_EXTENSIONS_LEDGER](/docs/architecture/K_EXTENSIONS_LEDGER.md) (cascade narratives) + K_L14_EVIDENCE_DASHBOARD (К-L14 verification log). Representation added 2026-06-12 (Architecture Truth Cascade — these cascades previously had no roadmap rows):

| # | Cascade | Date | Commits | Outcome |
|---|---|---|---|---|
| #0 | А'.7.x bus architecture amendment | 2026-05-21 | 13 (closure `ad3ff4f`) | Per-tier mutex split + O(N) coalesce + S10 cross-tier probe + 5 bug fixes; К-L15.1 LOCKED (2-layer); К-L14 #8 clean |
| #1 | А'.7.5 bus source split | 2026-05-22 | 5 (`c1d10b0..fe5a871`) | К-L15.1 compile-time layer (bus_native.cpp → 4-TU split); К-L14 #9 clean |
| #2 | Godot full deprecation + Launcher formalization | 2026-05-23 | ~16 (`2022bc1..21a1054`) | Presentation physically purged; `DualFrontier.Launcher` scaffold (Lesson #N12 first application); К-L14 #11 first removal-type evidence |
| #3 | Launcher visual implementation | 2026-05-23/24 | ~12 (`e1bbc6a..8ea0d03`) | Pawn-3 dispatch arms real + 3 silent stubs per S-LOCK-4 amended; К-L14 #12 first clean additive evidence |
| #4 | A'.9.0 reconnaissance | 2026-05-24 | 8 (`a233639..8a0ec32`) | 7-domain analyzer milestone recon (A_PRIME_9_RECONNAISSANCE_REPORT, ~3340 lines); К-L14 #13 first observational evidence |
| #5 | A'.9.1 analyzer infrastructure | 2026-05-25 | Phase 0 `bb6807c`; α `5030fa2..a23556f`; β-prep `588c667..a213954` | 17 rule stubs + CPM + PROJECT_AXIOMS v1.0 LOCKED; Phase β/γ/δ pending — see «Analyzer track» below |

К-L count unchanged across all six cascades: **21 final**.

### K9 — Field storage abstraction — CLOSED (RawTileField shipped 2026-05-11)

Prerequisite for V substrate compute primitives V1/V2; ships CPU functional path first. Per [VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md) "Storage interaction" + [KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md) K9.

- `RawTileField<T>` (C++): conductivity map, storage flags, ping-pong back buffer.
- C ABI: `df_world_register_field`, `field_read_cell`, `field_acquire_span`, `field_set_conductivity`, `field_set_storage_flag`.
- Managed bridge: `FieldRegistry`, `FieldHandle<T>`.
- CPU reference diffusion (used both as functional fallback and as V1+ shader equivalence oracle).

Exit criteria: any field type registrable / readable / writeable from managed; CPU diffusion correct on test grids; no GPU dependency.

### V substrate — Vulkan rendering + compute — V0 ✅ / V1 ✅ / V2 ⏭ / M-V ⏭

Per [VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md) (consolidates former RUNTIME_ARCHITECTURE.md + GPU_COMPUTE.md per Q-G-1 LOCK; substrate primitives reduced from G0..G6+G9 to V0/V1/V2 per Q-G-2). Status authority is the V-rows in the Status overview above; design authority stays VULKAN_SUBSTRATE. Realized/pending record (reconciled 2026-06-12, Architecture Truth Cascade):

- **V0 — foundation: ✅ realized** (2026-05-18/19, Q8 ratification; closure `1b8f2ea`; sub-closures V0.A/V0.B/V0.C.1/V0.C.2 chronicled in MIGRATION_PROGRESS V-series). Rendering-migration R.0–R.8 outcomes: R.0–R.3 realized via V0.A–V0.C.2; R.4 split — input infrastructure shipped, Domain forwarding unwired (Launcher drains + discards events); R.5 realized in altered form — `DualFrontier.Launcher` host per Q-G-6 (b1), not a Presentation rewrite (infrastructure К-ext #2, pawn visuals К-ext #3 `97f4573`); R.6 NOT shipped (no text/UI primitives, no fonts); R.7 partial (swapchain/framebuffer recreation shipped; focus→pause coupling + DebugOverlay open); R.8 done at К-ext #2 (Godot purge `2ba8130` branch-point, cascade closed 2026-05-23; deliberate residuals: root `project.godot` per F-5 + `assets/**/*.import` files; the R.8 deliverable's `tools/build-all.ps1` never existed — real wiring is the `CompileShaders` MSBuild target).
- **V1 — diffusion: ✅ realized** (2026-05-19; PR #40 `88aebf2`). Evidence: `V1DiffusionPipeline.cs`, `diffusion.comp(.spv)`, CPU oracles + equivalence/insulator/mass-conservation suites, smoke scenes, latency benchmark.
- **V2 — wave: ⏭ pending** — `wave.comp` absent; no managed wrapper, no distance/direction side products (former G6 fold-in), no oracle. V2 amendment authoring precedes execution (chronicle context: «А'.7.5 → А'.8 → V2 amendment → V2 → А'.9»; amendment not started). M-V7/M-V8 movement demos gate on this primitive.
- **V close — multi-field coexistence: ⏭ pending** (former G4, V close gate not separate primitive).

**Reductions per Q-G-2** (unchanged): G3 storage cells/capacitance → gameplay-level node config. G4 multi-field → V close criterion. G6 flow field → V2 side products.

**Deferred (TBD notes preserved in VULKAN_SUBSTRATE.md):** G5 projectile Domain B (substrate disposition TBD at M-V5 amendment authoring). G9 eikonal upgrade (V2 tunable or V3 — evidence-gated).

**M-V demonstrations** (per Q-R-1 format): all pending — M-V1 mana, M-V2 electricity, M-V5 projectile (deferred), M-V7 movement, M-V8 local avoidance; gaps M-V3/M-V4/M-V6/M-V9 reflect Q-G-2 reductions. Gates: mod content (vanilla skeletons have empty `Initialize`) + mod-facing compute surface (`RestrictedModApi.ComputePipelines => null` today).

**Future V-N primitives** reserved for post-substrate compute needs (G5 Domain B disposition, G9 eikonal upgrade if evidence justifies, modder-driven primitives).

### Combined timeline

*Historical planning estimate, recorded pre-execution — K0–K10.3, K9, V0 and V1 have since shipped (see rows above); only V2 + M-V demos remain from this envelope.* K0–K8 (5–8w) + K9 (1–2w) + V0 (~4–6w) + V1 (~1–2w) + V2 (~2–3w) + M-V demos (~3–5w) ≈ 16–26 weeks for the full architectural foundation, runnable in parallel where dependencies allow ([KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md) §8 governs cross-track sequencing). Ordering: K-series gates K9; K9 gates V1/V2 compute primitives; V0 rendering side is independent of K/V1/V2 tracks once Vulkan handles are exposed (V0 compute side gates on K9).

### When

Original "post-launch" framing is preserved as the outer envelope, but K/V tracks may begin during the M-cycle wherever non-blocking — particularly K9 (CPU-only, no GPU dependency) and V0 rendering side which can land alongside the M-cycle without disrupting it. Cutover decisions remain at K8 and V0 rendering-side R.5/R.8.

**Why this is possible:** the architecture is already ready. The Mod-OS Migration does not change the relationship between Application and the presentation host; vanilla mods run identically regardless of host — today `DualFrontier.Launcher` over the Vulkan substrate (Godot was purged at К-ext #2, 2026-05-23) — because the host and the simulation both consume the same `IGameServices` contracts. Field-based mechanics ship as additional vanilla mods registering through the same `IModApi` ([VULKAN_SUBSTRATE](/docs/architecture/VULKAN_SUBSTRATE.md) "Mod parity (KERNEL K-L9)").

---

## ⏭ Analyzer track (A'.9.1 Phase β/γ/δ + deferred rule families)

> **Relocated here 2026-06-11 (Standing-Law cascade spec/roadmap separation).** The
> analyzer forward-sequencing load was moved from [ANALYZER_RULES](/docs/architecture/ANALYZER_RULES.md)
> v0.1 — the §4.1/§4.2 severity-promotion columns + implementation order, §5 (К-L20 LOCK
> cascade deferral family), §6 (hardware tier expansion), §10.5 (forward implementation
> plan), §11 (lifecycle forward), and the frontmatter «Forward sequencing» line — so that
> document is now pure rule specification (registry, namespaces, templates, realized
> decision records). ANALYZER_RULES.md stays the rule-spec authority; **this section owns
> analyzer sequencing and futures.**

**Ground state (2026-07-02, A'.9.1 arc closed):** the analyzer ships **17 detecting rules** at Release 1.0 severities — 11 Error + 5 Warning, build-breaking under `TreatWarningsAsErrors`, plus DFL025_B at descriptor Info (`.editorconfig` `suggestion`, IDE-only) — at `tools/DualFrontier.Analyzers/Rules/{Architecture,Discipline,NativeBoundary}/`, wired into all 12 `src/` projects via `src/Directory.Build.props` (`ProjectReference` with `OutputItemType="Analyzer"`). Enforcement is live: a DFK/DFL/DF999 violation fails the build; suppression only per the CODING_STANDARDS DFK-WAIVER law (census pin 2). Registry + anatomy: [ANALYZER_RULES](/docs/architecture/ANALYZER_RULES.md) §4. The β/γ/δ blocks below are executed phase records (DONE with hashes); the deferral families further below remain scheduled, not shipped.

### A'.9.1 Phase β — detection implementation — DONE `1bc0df2..b116727` (12 commits, 2026-07-01)

Populated detection logic into the 17 stubs (canonical per-rule narratives: [K_CLOSURE_REPORT](/docs/architecture/K_CLOSURE_REPORT.md) §7.2):

- **Rule set by category** — Architecture (9): DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK011, DFK013, DFK016, DFK017. NativeBoundary (5): DFK001, DFK002, DFK007_1, DFK015_1, DFK019_A. Discipline (3): DFL025_A, DFL025_B, DF999.
- **Implementation order** (relocated v0.1 §4.1/§4.2 column): P0 first — 1 DFK001, 2 DFK002, 3 DFK003, 4 DFK003_1, 5 DFK004, 6 DFK005, 7 DFK007, 8 DFK011; then P1 — 9 DFK007_1, 10 DFK015_1, 12 DFK017, 13 DFK019_A (slot 11 vacated by the Q-L-9 DFK010 drop; v0.1 numbering preserved verbatim; rule IDs in the post-adjudication underscore forms); β-secondary (DFK013, DFK016) + Discipline (DFL025_A/B) + self-policing (DF999) follow without pinned integers.
- **Per-rule tests**: positive + negative cases via `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` into `tests/DualFrontier.Analyzers.Tests` (currently a single placeholder Fact).
- **Working severity baseline**: per-rule `.editorconfig` baseline at `suggestion` for the cleanup phase (triage mode), ahead of Phase γ promotion.
- **Violation triage** (cleanup phase): (a) fix in code; (b) suppress per the DFK-WAIVER form — [CODING_STANDARDS](/docs/methodology/CODING_STANDARDS.md) §5.3 (supersedes the v0.1 `DFK###-SUPPRESS` sketch); (c) rule refinement on false positive.
- **Adaptive gate per Q-L-1**: total first-run violation count ≤ 80 → continue in a single cascade; > 150 → split into sub-cascades; 81–150 → Crystalka decision.
- **Cleanup discipline** (relocated v0.1 §10.5): one rule's violations resolved at a time (Lesson #8 atomic discipline); cumulative debt resolution across all 17 rules; architectural debt unrelated к a rule handled per Lesson #14 as a separate cleanup cascade.

### A'.9.1 Phase γ — severity promotion — DONE `524dd31..cc2f71a` (8 commits, 2026-07-01) + execution residue `4cc5e7e`

- Per-rule severity lines landed in `.editorconfig` at γ C3 `18a4dac` — the repo file had been charset-only (`root = true` + `[*] charset = utf-8`); the `[*.cs]` block with all 17 descriptor-identical keys was created there.
- **Promotion map** (relocated v0.1 §4.1/§4.2 «post-promotion» columns, reconciled к canonical K_CLOSURE §7.2): `error` for the correctness rules — P0 (DFK001, DFK002, DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK011) + P1 (DFK007_1, DFK015_1, DFK017). DFK019_A → `warning` (K_CLOSURE §7.2 DF019 row; v0.1 §4.2 grouped it under Error by carryover — resolved at ANALYZER_RULES v0.2.0; F-12 ratified). DFK013 / DFK016 → `warning` per К-L13/К-L16 efficiency-class (efficiency-not-correctness). DFL025_A → `warning`; DFL025_B → `suggestion`; DF999 → `warning`. (Executed 2026-07-01 from descriptor Info — the v0.1 `suggestion` baseline wording presupposed an editorconfig prime that never existed; promotion flipped the descriptors themselves, with `.editorconfig` restating.)
- **Exit gate**: `dotnet build` exit 0 with every correctness DFK### at `severity = error` — the moment К-Lxx compile-time enforcement goes live — **satisfied at γ C2 `3272d74`** (post-flip full-solution `--no-incremental` Release rebuild 0W/0E; enforcement claims are legal doc-wide since).
- `AnalyzerReleases.Shipped.md` received its first release entry (Release 1.0) at γ C2 `3272d74`; `AnalyzerReleases.Unshipped.md` emptied to the RS-tracked shipped state.

### A'.9.1 Phase δ — closure + governance — DONE (2026-07-02, PHASE_DELTA_BRIEF cascade; the arc closes)

- K_EXTENSIONS_LEDGER §3.6 cascade #5 entry (realization + Phase β/γ outcomes) — executed `59c1802`.
- K_L14_EVIDENCE_DASHBOARD verification #14 (A'.9.1) recorded; dashboard promoted AUTHORED-SKELETON → Live (§6 gate satisfied at #14) — executed `5c34946`.
- KERNEL_ARCHITECTURE chronicle entry (2.6.1 → 2.6.2) — executed `11f82bb`.
- METHODOLOGY Lessons: the timing-locked evaluation executed — #N17/#N18/#N19/#N20 FORMALIZED + #N14 PROMOTED + the F-7 lesson-number registry note, METHODOLOGY 1.14.0 — executed `5a5bf75`; the F-27 rider set (CODING_STANDARDS 2.1.2 + PIPELINE_METRICS 0.2.1 + RESERVED_SURFACE_MUTABILITY 1.0.1 + TESTING_STRATEGY 2.0.2 + ANALYZER_RULES 0.4.1 + the 17 descriptor `Description` strings) — executed `0411bb0`.
- REGISTER governance cascade (3 enrollments + version sync + arc-brief EXECUTED transition; 2.20 → 2.21) — executed at this cascade's closure commit (EVT-2026-07-02-A_PRIME_9_1-CASCADE-CLOSURE). **ANALYZER_RULES.md lifecycle promotion AUTHORED-SKELETON → Tier 1 LOCKED — re-gated at Phase δ (honest deferral)**: of the relocated v0.1 §11 criteria, per-rule test coverage ✓ (Phase β 54-test suite), Phase γ promotion executed ✓, cleanup outcomes recorded ✓ (2 census-pinned DFK-WAIVERs; ANALYZER_RULES §12); the remaining criterion — per-rule §2 templates populated — is open by ANALYZER_RULES §10's own declaration («Empty at v0.4.0 … Phase δ+ item») and is the promotion gate of a future dedicated §10-population cascade.

### К-L20 LOCK cascade rule family (deferred — post-A'.9 milestone)

Relocated v0.1 §5. Per amendments log §3 (5-rule deferral) + Crystalka direction §1.1 batch 2 (Brief A'.9.1 deliberation 2026-05-24) + Q-L-11: pre-emptive enforcement against a moving Mod API target violates PA-002 («без костылей»).

| Rule | К-L | Deferral rationale |
|---|---|---|
| DFK009 | К-L9 Vanilla=mods | IModApi surface volatile pre-К-L20 LOCK; mod parity surface defines what «IModApi» means — undefined pre-LOCK. |
| DFK012 | К-L12 native scheduler sovereignty | Managed scheduler facade boundary not finalized pre-К-L20 LOCK; facade contract (К-L9 «facade preserves Vanilla = mods») depends on К-L20 LOCK for definition. |
| DFK015 | К-L15 bus capability declaration | Capability vocabulary (К-L15 tier registration, capability tokens, FQN scoping) ties tightly к К-L20 mod API surface; pre-LOCK не finalized. |
| DFK018 | К-L18 mod unload quiescence | Lifecycle sequence (PauseAsync → WaitForQuiescenceAsync → operation → ResumeAsync) part of the К-L20 Mod API contract surface; refinement at К-L20 LOCK pending (К10.3 v2 §9.5 8-step → 9-step с V resource cleanup placeholder per K_CLOSURE §2.21). |
| DFK020 family | К-L20 | 20 candidate sub-rules per recon §6.2: namespace/type restrictions, API usage restrictions, manifest field static cross-check, forward-compatibility grace period semantics. К-L20 canonical text post-LOCK. |
| DFC001.A | Bridge IRenderCommand marker purity | Bridge surface = Mod API-coupled per Q-L-11. К-L20 LOCK clarifies Bridge contract; pre-LOCK marker enforcement = pre-emptive kostyl. |
| DFC001.B | Bridge Command record purity | Same rationale as DFC001.A. Record purity (no mutable members) part of К-L20 Mod API immutability contract surface. |

**Activation**: К-L20 LOCK cascade post-A'.9 milestone per K_CLOSURE §9.5 Q1–Q8 deliberation timing. Cascade likely multi-stage decomposition (5 К-L-specific rules + ~20 DFK020 sub-rules per amendments log §3.4 / recon §6.2). The reserved `DFC###` namespace and the reserved `DualFrontier.ModSurface` category ([ANALYZER_RULES](/docs/architecture/ANALYZER_RULES.md) §9) activate here.

**`MOD_API_CONTRACT.md`** — authored **within this cascade** (Tier 1 LOCKED per K_CLOSURE §2.23 + §9.5 Q8); the document does not exist yet. References to it anywhere in the doc set are forward references until then.

### Hardware tier expansion cascade (deferred — audience-driven)

Relocated v0.1 §6. Per Q-L-8 split + Crystalka direction §1.6 batch 2 + recon Q-K-4 — multi-hardware-tier audience absent at current cascade; audience-driven deferral per Lesson #N17 Provisional.

| Rule | К-L | Deferral rationale |
|---|---|---|
| DFK019.B | К-L19 hardware tier capability runtime check | Requires runtime hardware capability probe (Vulkan extension query, GPU memory tier detection). Multi-tier hardware audience absent — single tier (T1 high-end Vulkan 1.3) is current substrate baseline. |
| DFK016 threshold customization API | К-L16 configurable depth | Optional follow-on if the DFK016 retain-α surface needs a runtime customization API beyond compile-time `PipelineSlotInterop` constants. Not activated — Phase 0 retain α is sufficient (compile-time constants stable). |

**Activation**: when a multi-tier hardware audience materializes (Crystalka direction §1.6 batch 2 «расширять V» — a post-A'.9 V-extension cascade may surface this).

### PublicApiAnalyzers deferral — activation conditions (Q-L-13)

The deferral decision record (Q-L-13 + PA-001 rationale) stays in [ANALYZER_RULES](/docs/architecture/ANALYZER_RULES.md) §8.1; the re-activation triggers live here (any of):

- Community emergence (third-party developers consuming the public API surface).
- A public-API stability commitment к external consumers.
- A specific cascade brief explicitly requesting PublicApiAnalyzers adoption (re-triggers Q-L deliberation).

---

## ⏭ Engineering queue (post-DRAFTS_RATIFICATION — the ratified contracts as work orders)

*Seeded 2026-07-17 at the DRAFTS_RATIFICATION closure (EVT-2026-07-17-DRAFTS_RATIFICATION). The six LOCKED cross-cutting contracts are the work orders; sequencing is the architect's next deliberation. Nothing here is scheduled by this cascade.*

| # | Family | Work order (contract anchor) | Members |
|---|---|---|---|
| EQ-a | **Shutdown-law implementation** | ROL §4 (S1–S8) + §6.2 checked-destroy + CMM §6–§7 + ELT §2.6/§3/§4 — the N-19 gap family; adjacent to F-40's mod_unload-globals guard | `ExecutePhase` per-system catch + origin dispatch; `UnloadAll` first production caller; world-shutdown transaction (Join checked, deterministic `NativeWorld.Dispose`, teardown order S2→S8); deferred-handler catch symmetry + regression test (CMM's stated top priority); `df_world_active_span_count` export + `df_world_destroy_checked` ABI; `EngineSession : IDisposable` adoption (ROL §4.4 sketch); swapchain prepare-before-reclaim + failure-injection test (ELT §2.5); Degraded surface (ELT §4.1); production native teardown (`df_bus_clear` promotion); device-lost v1 decision (ELT OQ-3 / IAC §7-7) |
| EQ-b | **Identity versions surface** | IAC §2 (the version-0 resolution — "highest-value single fix"; ECS §5 defers here) + EAM Q4 | `df_world_acquire_versions`/`release` additive ABI; `SpanLease<T>` versions view; `IsValid → Index > 0` both sides (`EntityId.cs:38`, `entity_id.h:14-16`); DFK-entity-identity analyzer rule (Error, NativeBoundary); ≈20-site atomic migration across nine systems + `GameBootstrap.cs:241`; `EntityEncoder` census-pinned waiver until PS-5; ABA regression + collision + negotiation tests |
| EQ-c | **GATE-S1..S4 / GATE-B1..B4 preparatory work** | EAM §3 (all 8 gates confirmed OPEN at ratification; split rows R2/R5 = the deferred DFK012/DFK015 rules) | Real access sets (S1); managed-vs-native phase-partition lockstep harness (S2); dispatch switch via `SchedulerAdapter` (S3); wake surface beyond Timer-1 (S4); `BusFacade` production construction (B1); flag-on/off dispatch-equivalence harness (B2); capability enforcement population (B3); `DomainEventBus` counters (B4) |
| EQ-d | **Wiring/comment queue** | F-35..F-38 (standing ledger rows) + smaller records from the ratification review | `EffectiveApiVersion` wire-or-delete; `RegisterRestrictedModApi` wiring gap; `TickRates.SLOW` comment class; bounded-queue law owner (CMM §9.6); no-config-contract gap (EAM R13 / TCM §6 rule 3); input-bridge (EAM Q5 / TCM §7.3, existing Native-foundation track) |

New F-entries seeded with this queue: F-42 (RNG service), F-43 (ABI protocol hardening) — see the Findings ledger.

## Beyond ship

After Phase 7 closure — game shipped, baseline performance measured, full vanilla mod set live — development continues in the form of post-release updates. These updates are **not** part of the M0–M10 milestone sequence and do not have a formal plan inside this roadmap. The active scope through Phase 7 is the Mod-OS Migration plus dynamic map expansion (tracked in [Backlog](#backlog)); everything else is deferred.

Candidate ideas for post-release updates live in a dedicated reservoir document: [IDEAS_RESERVOIR](./IDEAS_RESERVOIR.md). The reservoir holds entries that are architecturally compatible with the foundation laid by M0–M10 but are deliberately not scheduled. Inclusion is not a commitment; the reservoir's discipline is intentionally lighter than this roadmap's. Among the entries currently recorded:

- AI mod assistant (local LLM generates mods validated through the existing capability system).
- Voronoi-driven faction borders (computed from weighted geometry, GPU-determinism dependent).
- Topological data analysis for social dynamics (persistence homology over the colony's social network).
- Symbolic regression on simulation data (the game discovers its own laws and mod effects).
- Causal inference for player decision analysis (counterfactual replay made cheap by determinism).
- AI opponents through behavior cloning (challenging-but-legible adversaries from anonymized replays).
- Lambda calculus REPL for power users (minimal Turing-complete scripting under the capability system).
- Player model for adaptive personalization (local-only, transparent, player-controlled).
- FHE integration (architectural commitment ratified separately as [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md) v1.0; activation deferred per its §D1).

The reservoir is read after Phase 7 closure, not before. Reading it during the active M-cycle introduces decision fatigue: every idea there looks plausible, and the temptation to begin "just one" before shipping is real. The roadmap is the active surface; the reservoir is dormant context.

The single exception is FHE: because its contract is already ratified, [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md) v1.0 is binding from now even though activation is conditional. The contract specifies architectural boundaries that the multiplayer-related code in M0–M10 does not currently produce — this is the contract's specified resting state, not a defect.

---

## Findings ledger (F-series)

> Seeded 2026-06-11 (Standing-Law cascade, brief §13; shape adapted from the News
> Intelligence Hub findings-ledger mechanism). One row per finding; findings land
> here at the closure that surfaces them — **never chat-only** (METHODOLOGY §12.9
> (d)). Severity: S0 critical / S1 high / S2 medium / S3 low / N note. State:
> OPEN / CLOSED / ACCEPTED-NO-ACTION. Closing a finding records the closing
> commit hash in its resolution cell.

| F-# | Finding | Severity | State | Resolution / owner |
|---|---|---|---|---|
| F-1 | `REGISTER_RENDER.md` self-declares register v2.0 — 15+ versions stale vs SoT | S2 | CLOSED | Render regenerated at C10 (this cascade's final commit) against register 2.16 / C9 `fe1fa61`; header self-reference backfilled in the same commit. The render-script defect observed in the stale render persists in the fresh output → split off as F-13 |
| F-13 | `render_register.ps1` variable-expansion defect: every document row in the regenerated render emits the literal `$(System.Collections.Hashtable.last_modified_commit)` (272 occurrences in the 2026-06-11 render) instead of the commit value — the script bug the drift report §6.3.4 flagged; regeneration alone does not cure it | S3 | CLOSED | Root cause `render_register.ps1:105` — a backtick-escaped `$` prevented the `$(...)` subexpression from evaluating, so `$d` interpolated as its type name and every document row rendered the literal `$(System.Collections.Hashtable.last_modified_commit)` (286 in the pre-fix render, 272 historically). Fixed at CODEX_CLOSURE C3 `e025086` by doubling the backticks; the C3 dry-run render showed 0 `System.Collections.Hashtable` with real hashes / `PENDING-*` in markdown backticks. Final acceptance at the C8 closure render: 0 `System.Collections.Hashtable` across all 288 document rows. Postscript 2026-07-17: mooted permanently — `render_register.ps1` retired at the schema-2.0 inversion (CORPUS_CLOSURE_INVERSION_B CD4 `820abf9`); REGISTER_RENDER.md is DEPRECATED/historical |
| F-2 | `PENDING-COMMIT` placeholders across REGISTER.yaml; backfill discipline systemically open. Count: 124 at seeding → **123** measured 2026-06-11 post-C10 (the Standing-Law C10 render backfill resolved one; recon R3 re-measure) | S2 | **CLOSED** | Future register-tooling cascade (hybrid reverse-register, Q-T-2) — **dissolves at F-34 execution**: the REGISTER_INVERSION inversion (FRAMEWORK 2.0 §14.3) outlaws the `PENDING-*` vocabulary via G-SCHEMA and drops it at migration, so the placeholder class ceases structurally when Cascade B (F-34) migrates the live corpus. **CLOSED 2026-07-17, dissolved-by-inversion at F-34 execution** (CD3 `eca38b9`): the migrator dropped every `PENDING-*` `last_modified_commit` at frontmatter injection (real hashes retained where real); the two supplement-carried residues hand-dropped at CD4; G-SCHEMA rejects the vocabulary corpus-wide from the armed flip on |
| F-3 | METHODOLOGY v1.12.1 lacked a body changelog entry (frontmatter↔changelog desync class) | S3 | CLOSED | Backfilled at C7 `b58eed4` (METHODOLOGY v1.13.0) |
| F-4 | KERNEL Part 0 invariant-ID script inconsistency: table rows Latin "K-L", prose Cyrillic «К-L» — any census/grep must match BOTH scripts (code shows the same duality: 140 Cyrillic / 38 Latin citation sites in src/) | S3 | OPEN | KERNEL amendment (architect); interim mitigation: bi-script rule pinned in RESERVED_SURFACE_MUTABILITY §4 + CODING_STANDARDS §6 |
| F-5 | `project.godot` still tracked at repo root post-Godot-deprecation (cascade #2); contradicts VULKAN_SUBSTRATE R.8 closure criterion. Scope note 2026-06-12: `assets/**/*.import` Godot-era files also survive the R.8 sweep — same decision class | S3 | CLOSED | Deletion authorized by Crystalka 2026-06-29 (engine already removed from Skarlet); executed C4 `be7d4c2` (Godot Eradication Cascade). Scope-closure extended to the full inert Godot file surface — `project.godot` + `icon.svg`/`icon.svg.import` + 11 `assets/**/*.import` + 204 `.cs.uid` sidecars + the `.gitignore` Godot block + `.godot/**`/`.gdignore` SCOPE_EXCLUSIONS entries — all deleted, build stayed green (inertness proven); DEVELOPMENT_HYGIENE §7 flipped to fully-eradicated (C5 `27807f7`). The `.uid` class surfaced via the pre-ratification pressure-test (F-1 / F-26), folded by PATCH 1 |
| F-6 | Tooling reality undocumented: pwsh absent (PS 5.1 + powershell-yaml 0.4.12 is the actual substrate); `sync_register.ps1 -Validate` unconditionally rewrites VALIDATION_REPORT.md (line 380) → commit-folding protocol required | S2 | CLOSED | Documented at C5 `9da4760` (DEVELOPMENT_HYGIENE v2.0.0 §4) |
| F-7 | METHODOLOGY lesson numbering gap: **#N11/#N15/#N16 absent** in `#N1x` form (pool: N10, N12, N13, N14, N17 — recon R3 re-measure 2026-06-11 widened the original #N15/#N16 finding); #N18 referenced by the DD recon brief but never codified | N | CLOSED | Adjudicated at A'.9.1 Phase δ: the lesson-number registry note landed in the METHODOLOGY 1.14.0 Provisional-Lessons preamble `5a5bf75` — #N1/#N11 declared never-assigned (free for future candidates); #N4/#N15/#N16 declared reserved-with-semantics (execution-tier assignments per recon R2.3; #N16 absorbed into #N14 per its own K-ext-#3 record); #N18 FORMALIZED (the timing lock unlocked here); the `#Nxx` citation form mandatory against the old-series #17/#18/#19 collision |
| F-8 | S-LOCK coverage audit (TESTING_STRATEGY §4.4 obligation) — A'.9.1 S-LOCK set audited 2026-06-11 | N | CLOSED | Audit empty: all artifact-checkable S-LOCKs verified on disk (S-LOCK-4 = 17 rule files; S-LOCK-10 = no DFK010 file; 6/7/8/9/11/12 confirmed); no gap entries required. Analyzers test gate is placeholder-level by Phase α design (Phase β scope) |
| F-9 | KERNEL Part 0 counting-convention divergence risk (table composition vs doc-stated «21 final») | S3 | **CLOSED** | **Resolved 2026-07-17 at CORPUS_CLOSURE_INVERSION_B HALT-1, variant (b)** (commit `d8f1db3`): the rework had inverted K_CLOSURE's "− 1 SUPERSEDED" convention into "(К-L6 SUPERSEDED counted)"; the Part 0 headline recomposed keeping the ratified count 21 (К-L6 listed for traceability, excluded from the count; 22-row table note added). Census "21 (invariant\|final)" delta: 0; aligned with the RESERVED_SURFACE_MUTABILITY §4 pin. F-4 (bi-script ID duality) deliberately NOT resolved by this — remains its own KERNEL amendment |
| F-10 | Two pre-existing test failures on `main`, observed 2026-06-11 (full-suite run, 1034/1036): `Core.Tests SchedulerStressTests.NativeGraph_FiveThousandSystems_RandomDag_ComputesAndTicksWithoutError` (TickBegin failure at tick 2692, testhost crash; reproduced twice under survey load, Category=Stress) + `Modding.Tests GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PawnStateCommandCarriesRealName` (timing-sensitive, empty stateCommands). Production code untouched by this cascade — recorded, not absorbed (TESTING_STRATEGY §2.6) | S1 | CLOSED | F-10 isolation cascade 2026-07-02: recon proved zero deterministic failures. #2 (RunningLoop) FIXED (serial GameLoopSerial collection + poll-until-condition replacing fixed sleeps) at C2 fc8f6b9. #1 (SchedulerStressTests) is green in isolation; the native TickBegin crash-under-load reassigned to F-29(a); kept Category=Stress under the serial isolation pass. The "zombie testhost wedge" root-caused as a stdout-pipe deadlock (not a test hang) and codified in TESTING_STRATEGY §8 (C7 87ceb90). Extreme scale non-completers S1/S2/S7 reassigned to F-29(b), Skip-quarantined (C6 d8fc56c). Lesson candidate (not formalized here): the stdout-pipe / TRX-is-truth invocation law -- an executor-level lesson (#N-series analog) sourced from the F-10 recon; formalize at the next methodology cascade (owner: architect). |
| F-11 | 41 of 44 local branches besides `main` are fully merged — pruning candidates (3 unmerged keepers: `claude/cpp-core-experiment-cEsyH` К-L source-of-truth, `claude/godot-removal-deliberation-Vfg2R`, `feat/m0-m3-mod-os-migration`) | N | OPEN | Crystalka — prune at leisure; no pruning performed this cascade (DEVELOPMENT_HYGIENE §5) |
| F-12 | DFK019_A Phase-γ severity target discrepancy: K_CLOSURE §7.2 + ANALYZER_RULES v0.1 §10.1 say Warning; A'.9.1 brief §8.1 blanket promotion reads Error. v0.2.0 registry records Warning per the canonical Tier-1 source, footnoted in ANALYZER_RULES §4.1 + ROADMAP «Analyzer track» | S2 | CLOSED | Ratified by Crystalka 2026-07-01: DFK019_A = Warning per the canonical K_CLOSURE §7.2 (PHASE_GAMMA_BRIEF f12_authorization). Promotion executed at Phase γ C2 `3272d74`: descriptor Warning + AnalyzerReleases.Shipped Release 1.0 row + `.editorconfig` key agree (H-integrity clean) |
| F-14 | Un-ledgered version-bump class: MOD_OS changelog had no v1.9–v1.11 entries (frontmatter advanced silently at К10.3-v2 era); VULKAN changelog had no v1.1.2 entry (DD-1) | S3 | CLOSED | Backfilled empirically from git history at C2 `9676f54` (MOD_OS) + C3 `60175a1` (VULKAN), Architecture Truth Cascade |
| F-15 | MIGRATION tracker desyncs: MIGRATION_PLAN frontmatter 1.4 vs body "1.3 LOCKED" + stale authority pins; MIGRATION_PROGRESS chronicle ended at А'.8 (6 cascades unrecorded) + stale snapshot cells (620-test count, A'.6-next recommendation) | S2 | CLOSED | C7 `f2e6df2` (PLAN rider: 1.4.1 + unpins) + C9 `061fbc0` (PROGRESS catch-up + snapshot truth) |
| F-16 | FRAMEWORK §8.1 said "Four meta-entries"; REGISTER carries five (`is_meta_entry: true` = 5; PROJECT_AXIOMS enrolled A'.9.1 was uncounted) | S3 | CLOSED | C11 `7a074e4` (FRAMEWORK 1.1.2) |
| F-17 | Citation fragility classes: version pins of living docs (code + docs) and URL-fragment anchors (both ReservedStubAttribute anchors broke at ANALYZER_RULES v0.2.0 — recon Anomaly 4) | S2 | CLOSED | Citation-form law landed at C14 `bdb5283` (CODING_STANDARDS §6.1); code instances fixed C12 `89d34f6`; doc instances C2–C11 |
| F-18 | `NATIVE_CORE.md` cited from native headers (component_store.h, df_capi.h) + NativeWorld.cs — file absent repo-wide | S3 | CLOSED | C13 `476757b` + C12 `89d34f6`: repointed to KERNEL_ARCHITECTURE Part 0 (К-L3/К-L3.1); coverage verified, no architectural contradiction (H4 not triggered) |
| F-19 | A_PRIME_9_RECONNAISSANCE_REPORT registered Live though its recon input is consumed (largest file in docs/architecture) | S3 | CLOSED | Lifecycle Live → EXECUTED ratified by ARCHITECTURE_TRUTH_CASCADE_BRIEF (Д-5); executed in the C16 REGISTER closure of the same cascade |
| F-20 | Stale text inside string literals (out of comment-pass grant): RestrictedModApi.cs:233-236 grace-period console message (v1 manifests cannot reach that code); ModIntegrationPipeline.cs:864 "v1.4 §9.5.1" pin inside a ValidationWarning message | S3 | OPEN | Next code cascade — string changes are code-token changes; pair with a test sweep |
| F-21 | Doc-citation staleness outside the cascade grant: src READMEs claim IsolationViolationException semantics (Core/README.md:16, Core/ECS/README.md:17+:43, Application/Modding/README.md:18); all 5 vanilla mod sources pin "MOD_OS_ARCHITECTURE v1.5 §1.3" (CombatMod/MagicMod/PawnMod/InventoryMod/WorldMod) | S3 | OPEN | Next hygiene pass (README + mods comment sweep per CODING_STANDARDS §6.1) |
| F-22 | Dead managed surface flagged during CF audit: `SystemBase.Context` property never assigned anywhere (incl. tests); `SystemRegistry` class unreferenced in src | N | OPEN | Architect review — delete or annotate `[ReservedStub]` per Lesson #N12 |
| F-23 | ISOLATION.md internals stale: cites "THREADING §11.7" (no such section in any THREADING version); §"fault handling" 6-step sequence describes surfaces not found in code (logs/mods/<id>.log, ModDisabledEvent, user banner) — ModFaultHandler is an accumulator with deferred unload | S3 | OPEN | Next doc pass (ISOLATION was outside this cascade's grant) |
| F-24 | Lifecycle-vs-body mismatch: ARCHITECTURE_TYPE_SYSTEM + MAX_ENG_REFACTOR_TRACK_B carry `lifecycle: Draft` while bodies self-describe "LOCKED v0.1"/"RATIFIED v0.1" (D6 design-draft banners now counteract at the reader surface) | N | OPEN | Architect — align lifecycle or body at next touch |
| F-25 | Marker-family census delta unrecorded at source: commit C12 `89d34f6` (comment citation pass) moved two TESTING_STRATEGY §4.2 / CODING_STANDARDS §5.2 doc-tag baselines without the same-commit census-delta record — `stub` 48 matches / 18 files → **51 / 20** (truthful "reserved silent stub" phrasing in GameLoop/TickAdvancedCommand class docs), `deferred` 79 / 48 → **81 / 51** ("deferred unload" fault-path truth in SystemOrigin/IModFaultSink/ModIsolationException). Measured at the 2026-06-12 closure audit; the other three families (TODO 136/53, Phase 6 23/11, not yet 8/7), the reserved-surface pin (34/13) and the DFK-WAIVER baseline (0) are unchanged-exact. This row is the retroactive census-delta record per RESERVED_SURFACE_MUTABILITY §5 | S3 | CLOSED | Folded at Phase γ C5 `d5d6fe2` (TESTING_STRATEGY 2.0.1 §4.2 + CODING_STANDARDS 2.1.1 §5.2): `stub` 48/18 → 51/20, `deferred` 79/48 → 82/51. The ledger's 81/51 was the 2026-06-12 measure; 82/51 is the value the Phase β compiled meta-test pinned and this cascade re-measured (unchanged end-to-end). The pre-measure discipline lesson stands — Phase δ lesson-candidate alongside #N20 |
| F-26 | Eradication/cleanup recon swept the Godot file surface by an enumerated extension list (`.import`, `project.godot`, `icon.svg`) and missed the `.uid` Godot residue class — 204 source `.cs.uid` sidecars (+ 33 in `bin/obj`); two recon passes (R2 + the brief digest) inherited the gap. Surfaced by the Godot Eradication pre-ratification pressure-test (F-1) | S2 | CLOSED | Godot-specific: all `.uid` deleted at C4 `be7d4c2` (F-1 folded via PATCH 1; F-5 scope-closure). Method lesson — derive the eradication class from the repo's own `.gitignore`/config classification, not a fixed extension list — is the **Lesson #N20 candidate** (owner: architect; formalize at next closure), so the next cleanup cascade inherits the corrected method |
| F-27 | Post-Phase-γ doc-staleness residue (execution sweep, 2026-07-01) — living-prose sites still carrying pre-β/pre-γ state outside the Phase γ grant: (a) `RESERVED_SURFACE_MUTABILITY` §3 item 1 lines 57–58 assert the dot/hyphen descriptor-ID duality (`DiagnosticId = "DFK003.1"` / `"DFL025-A"`) — false since the β underscore adjudication (recon Anomaly 2); (b) `TESTING_STRATEGY` §5.3 item 2 says DFL025-A «analyzer detection is Phase β scope — today the convention binds by review» — detection landed β, binds at Warning since γ; (c) `ANALYZER_RULES` §9 DFL### note («DFL025-A + DFL025-B shipped as non-detecting stubs», hyphen forms) — past-tense provenance without the realized tail; (d) the 17 descriptor `Description` strings still say «Phase β cleanup-phase will populate detection patterns» (C2 grant was severity-only by brief letter); (e) `METHODOLOGY` Lesson #N17 narrative carries dotted DFK019.A/.B forms (living doc; natural fix at #N17 formalization) | S3 | CLOSED | Folded at Phase δ as PATCH riders: (a) RESERVED_SURFACE_MUTABILITY 1.0.1 + (b) TESTING_STRATEGY 2.0.2 + (c) ANALYZER_RULES 0.4.1 + (d) all 17 descriptor `Description` strings under the explicit file grant (analyzer rebuilt exit 0; 54/54 suite = pre-change baseline; string-only — no logic/IDs/severities) — all `0411bb0`; (e) dotted DFK019.A/.B → underscore forms inside the #N17 formalization at METHODOLOGY 1.14.0 `5a5bf75`. Residual staleness outside this grant surfaced during execution → F-28 |
| F-28 | Post-arc residual staleness surfaced at Phase δ execution (outside the F-27 grant): (a) `TESTING_STRATEGY` §3.8 still declares «Current state: NONE exist» for meta (repo-discipline) tests — false since Phase β (`CensusMetaTests` in `tests/DualFrontier.Analyzers.Tests` asserts the §4 census pins from inside the compiled suite; the v2.0.1 PATCH realized the §4.1/§4.2 wording but §3.8 was missed); (b) the 17 analyzer rule files' XML `<remarks>` provenance blocks still read «Phase β-prep stub — … Detection logic populated at Phase β cleanup-phase» as forward-looking prose (the F-27(d) grant covered descriptor `Description` strings only) | S3 | OPEN | (a) TESTING_STRATEGY §3.8 "NONE exist" CLOSED -- corrected to the realized meta layer (CensusMetaTests + FixtureDeploymentTests) at the F-10 cascade C7 87ceb90. (b) the 17 analyzer-rule XML <remarks> "Phase beta-prep stub" sweep remains OPEN -- next hygiene pass (comment-only analyzer-file class). |
| F-29 | Native/managed scheduler pathology surfaced by the F-10 recon (2026-07-02): (a) SchedulerStressTests.NativeGraph_FiveThousandSystems_RandomDag_... produces a native TickBegin crash (testhost crash) under concurrent build/test load -- green in per-project isolation, so a contention/concurrency defect in the native SystemGraphInterop/bus path, not a deterministic assertion failure; (b) SchedulerExtremeTests S1 (50k systems x 3k ticks), S2 (200k ticks), S7 (250k systems) do not complete within a 120-180s watchdog -- compute-bound at scale; the class's own comment hypothesizes an O(N^2) register-conflict scan or a native mutex above ~90k entries. Mitigated this cascade: (a) kept Category=Stress under the serial isolation pass; (b) Skip-quarantined so no sweep hangs. | S2 | CLOSED | F-29 native-scheduler cascade 2026-07-04 (brief `F29_NATIVE_SCHEDULER_BRIEF` v1.0 from recon `F29_NATIVE_SCHEDULER_RECON_REPORT` R1-R7; ratified + executed same day, LOCAL Skarlet): both defects FIXED. (a) root cause was NOT the bus (per-tier `std::mutex`, concurrent-publish by design) but the *lock-free* process-global `SystemGraph` + `WakeRegistry` Meyers singletons racing under xUnit parallel load -> managed-heap corruption; fixed two orthogonal ways -- D3 a shared `SharedNativeSingleton` xUnit collection (`DisableParallelization`) over the 3 singleton-touching classes (C2 `3c63b1f`) + D2 a fail-loud native `SingletonGuard` detector on graph + wake (`kConcurrencyViolation = -3` / `ComputeResult.ConcurrencyViolation`; C4 `e1f6485`; selftest hammer surfaces 2889 reader-violations + 1.02M writer-rejections, graph usable after). (b) the O(N^2) register-conflict scan replaced by an index-keyed O(N+E) rebuild (`writer_of` component->writer index + `system_ids_` O(1) dedup + layer-BFS Kahn; byte-identical phases/codes/last_error; C5 `c990755`) -- the selftest builds a 50k-system DAG in ~21 ms (was 120-180 s non-completion), so S1 (50k x 3k) + S7 (250k register+build) un-quarantined to plain `[Fact]` and pass (C7 `47252e7`); D4 selftest scenarios added (C6 `997bcb4`, 99 total). Two residuals seeded not absorbed: S2 -> F-30 (a *managed* `ParallelSystemScheduler` marathon, not native scale) + S3/S4/S5 -> F-31 (extreme-bus-load runtime-stress artifact, root-caused NOT a memory bug). TESTING_STRATEGY 2.1.0 -> 2.2.0 §2.8 isolation law (C8 `73954ef`). Closure gate all 9 suites green (1097 pass / 0 fail / 5 skip). Closure at C9 (REGISTER 2.22 -> 2.23); render C10 |
| F-30 | `SchedulerExtremeTests` S2 (`ManagedScheduler_TwoHundredThousandTicks_*`) is a *managed* `ParallelSystemScheduler` 200k-tick TPL steady-state marathon over a pre-built 80-system phase list -- not a native-scale scenario (the native graph is trivial and built once), so the F-29(b) O(N+E) rebuild does not reach its cost. Surfaced 2026-07-04 by the F-29 cascade: when the rebuild un-quarantined S1/S7, S2 still did not complete because its cost is per-tick managed TPL scheduling. Mis-grouped under F-29(b) by the F-10 recon | S3 | OPEN | Architect -- disposition pending: tick-count trim into the default sweep vs opt-in marathon excluded from the fast sweep (parallel to the S3/S4/S5 F-31 quarantine). Skip-quarantined + re-pointed from F-29(b) (C7 `47252e7`) so no sweep hangs |
| F-31 | `SchedulerExtremeTests` extreme-bus-load ceiling-probes -- S3 (5M events), S4 (12.8M events x 64 threads), S5a/S5b (1.6M latency samples) -- crash the testhost (`ExecutionEngineException` / `ThreadLocal.set_Value` AV inside `ParallelSystemScheduler.ExecutePhase`) when co-resident with `SchedulerStressTests` in one testhost. Root-caused this cascade as NOT a *native* memory-safety bug: all native paths + the interop marshalling audited clean; a post-closure standalone native ASan selftest (`df_native_selftest` under `/fsanitize=address`, dumpbin-confirmed ASan-linked) runs all 99 scenarios across the scheduler + bus tiers with zero AddressSanitizer errors; and each probe passes in isolation. The crash site is *managed* (`ParallelSystemScheduler.ExecutePhase` / `ThreadLocal`), outside native ASan's reach -- consistent with a *cumulative* .NET-runtime over-stress (thread-pool / GC pressure from co-resident multi-million-event probes) that tips the managed heap at teardown. Distinct from F-29(a) (the native lock-free-singleton race, fixed) | S2 | OPEN | Architect -- runtime-load ceiling, not a defect in project code; disposition = keep quarantined from the default sweep or gate behind an opt-in heavy-load lane. Skip-quarantined S3/S4/S5 (C3 `d9915ce`) so no sweep crashes; the D3 collection + D2 detector + O(N+E) rebuild ship independently |
| F-32 | GPU-validated Runtime follow-ups from the Codex remediation (CX-02 + CX-06). CX-02 — the sprite `VertexBufferRing` fix is a fail-fast guard (reusing a ring slot for a second unsubmitted batch now throws); the capacity redesign for genuine `>maxSpritesPerFrame` scenes needs a GPU-validated session. CX-06 — Vulkan present support is now VALIDATED on the graphics family (fail-fast); selecting a distinct present-capable family remains unimplemented. Both require Skarlet GPU work; neither blocks anything today | S3 | OPEN | Architect-owned — GPU-validated session (CODEX_CLOSURE C6 seed) |
| F-33 | Marker-family census-delta discipline gap at Codex `61f08ef`: CX-06 (`Runtime.cs:110` "not yet selected") + CX-21 (`TerrainKind.cs:13` "not yet set") added two `not yet` markers to `src/` without the same-commit census-delta record, moving the pin 8/7 → 10/9; `CensusMetaTests.MarkerFamilyCensus_MatchesPin("not yet")` was therefore red at HEAD (uncaught on the Codex Linux host — Roslyn 5.3 would not load `Analyzers.Tests`). Surfaced by the CODEX_CLOSURE Phase-0 Skarlet gate (H2b) | S3 | CLOSED | Operator-ruled FOLD; census-pin refreshed 8/7 → 10/9 at C2 `673f815` (CensusMetaTests.cs + CODING_STANDARDS 2.1.3 §5.2 + TESTING_STRATEGY 2.2.1 §4.2, same-commit census-delta per RESERVED_SURFACE_MUTABILITY §5); Analyzers.Tests 54/54 green post-fix. The six dated historical records preserve 8/7 |
| F-34 | REGISTER_INVERSION Cascade B — apply the inverted-register migration to the live corpus. Cascade A built the instrument (`tools/DualFrontier.Governance` net10.0; FRAMEWORK 2.0 §14 law) and dry-run-proved the round-trip (288/288 documents, globals 41/14/17/47, idempotent; zero reconciliation deltas beyond the ratified drops — see `docs/reports/REGISTER_INVERSION_A_MEASURE_REPORT.md`). B executes the live switch per the measure report's work order: migrate the corpus to frontmatter-as-SoT; align the mechanical surface (5 report-only gate fixes + orphan triage — 12 MODULE.md enroll F/4, 5 UNCLEAR enroll, 8 exclude); resolve the 5 architect-ruling classes (2 `.cs` non-.md artifacts, the 5 UNCLEAR category/tier, the EXECUTED-terminal question, the `'TBD —'` sentinel, and the 33% `special_case_rationale` override reading vs the §10 #5 20% threshold); **arm** the gates (flip `SemanticGatesEnforcing`, prove each red-once-then-green on the live corpus); retire the forward-regime PowerShell writers (`sync_register.ps1` + `render_register.ps1`); dissolve F-2. The derived-register integrity invariant (`REGISTER.yaml` + `CURRENT_AUTHORITY_SURFACE.yaml` byte-reproducible from the corpus) is armed as a closure-gate step. | S2 | **CLOSED** | **Executed 2026-07-17 as CORPUS_CLOSURE_INVERSION_B Phase D** (branch `claude/register-inversion-cascade-b` off post-merge `main` 959d7ea): CD1 measure `007473b` (re-proof at 321 + D5 `G_RATIO_PER_RULE_BREAKDOWN.md` — 41.5% decomposes to 22 genuine exemptions / 110 provenance notes); CD2 align `820758f` + CD2b `ecc1517` (7 gate fixes, 17 enrollments, 8 exclusions; control dry-run caught the scratch-glob and CD1-report seams); CD3 live migration `eca38b9` (337 docs to frontmatter-SoT; derived archive + authority surface 129 = 105 Live + 24 LOCKED; globals split; flag typed once); CD4 arm + retire (armed validate exit 0; PS writers deleted). Ruling (e) G-RATIO matrix deliberation remains OPEN separately, fed by D5 |
| F-35 | `ModManifest.EffectiveApiVersion` (`ModManifest.cs:145`) has zero call sites in `src/` — Phase A of the validator hand-rolls the fallback with different bare-string semantics (`ContractValidator.cs:128-176`: legacy path = floor-within-major; the property would produce an Exact pin). Doc truth corrected at `4a36abe` (MOD_OS R2-4); the CODE divergence is the open item | S3 | OPEN | Wire-or-delete decision (engineering, semantics choice for bare `requiresContractsVersion` strings); seeded from D1 R5.6 at CORPUS_CLOSURE_INVERSION_B closure |
| F-36 | `ModRegistry.RegisterRestrictedModApi` (`ModRegistry.cs:208`) has zero call sites repo-wide — the documented §9.4 step-3 Path β reclamation branch and `ModRegistry.Resolve<T>` dispatch are structurally unreachable in production (reclamation rides ALC collection at steps 6-7). Doc honesty clause landed at `4a36abe` (R2-6); the wiring gap is the open item | S3 | OPEN | Wire the registration at mod Apply, or retire the branch; seeded from D1 R5.6 |
| F-37 | Thread-pool Scheduler-mode arming is selftest-only: `transition_to_scheduler_mode` (`thread_pool.cpp:63`) has its sole caller at `selftest.cpp:1699`; the production bootstrap graph never flips the mode. Doc qualifier landed at `6497ed5` (R1-9) | S3 | OPEN | Belongs to the К-L12 cutover-gate family (EXECUTION_AUTHORITY_MATRIX §3); wire at the scheduler cutover; seeded from D1 R5.6 |
| F-38 | Stale 60 Hz-era assumptions in code comments: `TickRates.SLOW = 60` commented "(~1/sec)" (`TickRateAttribute.cs`) — at the 30 Hz sim rate it is ~2 s. Same class as the FEEDBACK_LOOPS ~250 ms figure fixed at `dff0f82` (R4-2). Comment-truth sweep candidate (zero `src/` changes were legal in the closing cascade) | S3 | OPEN | One-line comment fixes + a grep sweep for other rate-era residues; seeded from D1 R5.6 |
| F-39 | Dangling "METHODOLOGY §7.1 determinism invariant" citations OUTSIDE the rework corpus: `IDEAS_RESERVOIR.md` + `MAXIMUM_ENGINEERING_REFACTOR.md` still cite a determinism invariant at METHODOLOGY §7.1 that does not exist there (the live §7.1 is "Data exists or it doesn't"). The FHE in-corpus instances were re-anchored at `d66e02e` (R3-22) | S3 | CLOSED (2026-07-17) | Ridden as DRAFTS_RATIFICATION C8 per HALT-1 OD-8: all four sites re-anchored to TIME_AND_CONSISTENCY_MODEL §5 (now LOCKED law, no longer a draft citation) — IDEAS_RESERVOIR ×3 (GPU-determinism, runtime-LLM replay, see-also) + MAX_ENG:125 (PATCH 1.1.2 → 1.1.3) |
| F-40 | Cross-suite native-state flake: `ModUnloadInteropTests.UnloadModNativeState_VacuousUnload_Succeeds` failed once in a full-sln run (C-B gate, 2026-07-17) and passed 202/202 in the isolated suite re-run; zero `src/`/`native/` delta vs the passing baseline run. Mod-unload native globals (`g_sim_paused`, quiescence preconditions) are process-wide state exposed to cross-suite ordering — the F-29/F-31 family (bus was mutex-excluded; mod_unload state was not under the F-29 SingletonGuard umbrella) | S3 | OPEN | Reproduce under controlled ordering; consider SingletonGuard/collection treatment for `mod_unload` globals per the F-29 D2/D3 pattern |
| F-41 | Governance-tool follow-ups surfaced at the inversion: (a) the DOC-G-REGISTER self-entry hardcodes `next_review_due: 'post-Cascade B closure'` (`Validators.cs:432`) — stale as of this closure, needs a real onward value (tool PATCH; the instrument was deliberately not mutated mid-arm); (b) supplement-enrolled ids are not in the G-XREF resolvable set (pointers can only target .md-frontmatter docs — REGISTER_RENDER's deprecated_by had to target DOC-A-FRAMEWORK instead of the functional successors); (c) `VALIDATION_REPORT.md` (DOC-G-VALIDATION_REPORT, G/2/Live) lost its generator with the PS retirement — lifecycle disposition pending (historical vs delete) | S3 | OPEN | One small tool PATCH cascade + one register disposition; seeded at CORPUS_CLOSURE_INVERSION_B closure |
| F-42 | No RNG service: production RNG is ad-hoc across FOUR fixed-seed sites — `MovementSystem.cs:35` (target-typed `new(42)` — grep-resistant), `GameBootstrap.cs:96` (`ObstacleSeed`), `RandomPawnFactory.cs:58`, `ItemFactory.cs:39` — none world-derived, none persisted; blocks reproducibility class D1 in principle (TCM §4.3/§6, PS-14 precondition) | S3 | OPEN | Seeded `IRandomService.Stream(SystemId, SimTick)` on Contracts + `WorldSeed` in the save header + 4-site migration + a SEMANTIC (type-based) analyzer ban — a textual `new Random` regex misses the target-typed site (DRAFTS_RATIFICATION R4-10 lesson); rides EQ-b-adjacent |
| F-43 | ABI protocol hardening absent: no `df_abi_version` negotiation (native binary uncommitted, no CI → live skew risk, verdict N13); no per-entry-point thread-affinity declarations (IAC §3.6 — nothing stops `AcquireSpan` from the render thread); no DFK-abi-struct/DFK-abi-thread rules | S3 | OPEN | IAC §3 is the ratified work order: `df_abi_version` + affinity tag lines in `df_capi.h` + `NativeMethods` attributes + analyzer rules; sequenced with EQ-b/EQ-c per IAC §7 item 11 (renumbering-vs-cutover ordering) |

---

## See also

- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) — the specification driving M1–M10 (the register owns its version and lifecycle).
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — the pipeline; M1–M10 are exercised through it.
- [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md) — the four layers; the Mod-OS migration touches only Application and below.
- [CONTRACTS](/docs/architecture/CONTRACTS.md) — bus and marker conventions; capability syntax mirrors bus naming.
- [MODDING](/docs/architecture/MODDING.md) — v1 mod-author guide; M-phase outputs supersede this document for v2.
- [MOD_OS_ARCHITECTURE §8](/docs/architecture/MOD_OS_ARCHITECTURE.md) — `ModIntegrationPipeline` mechanics (absorbed the former MOD_PIPELINE document); M2, M5, M6, M7 extend it.
- [TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md) — test discipline; M-phase acceptance criteria slot into the existing isolation/modding/integration tiers.
- [DEVELOPMENT_HYGIENE](/docs/methodology/DEVELOPMENT_HYGIENE.md) — PR hygiene; the Mod-OS migration is exercised through the same checklist.
- [IDEAS_RESERVOIR](./IDEAS_RESERVOIR.md) — post-release ideas reservoir; populated, not scheduled. Read after Phase 7 closure.
- [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md) — architectural contract for fully homomorphic encryption multiplayer; ratified at v1.0, activation deferred per its §D1.
- [MAXIMUM_ENGINEERING_REFACTOR](/docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md) — three-track discipline escalation; ratified v1.0, activation deferred per-track.