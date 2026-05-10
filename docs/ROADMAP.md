# Roadmap

The Dual Frontier implementation has reorganised after the closure of Phase 4. The original Phase 5 (Combat), Phase 6 (Magic), and Phase 7 (World) are dissolved into a broader **Mod-OS Migration** (M1–M10) that simultaneously builds the modding kernel and ships gameplay content as vanilla mods. The architecture for this migration is specified in [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.5 LOCKED; this roadmap is the execution sequence derived from it.

The reorganisation follows the project's central methodological claim: **engine and methodology are the main research result, the game is a test case for the hypothesis** ([METHODOLOGY](./METHODOLOGY.md)). By implementing combat, magic, and world content through the modding system rather than alongside it, we make every gameplay feature also a test of the modding architecture. A combat system that ships as a vanilla mod is the strongest possible falsifiable claim that the contract surface for mods is complete.

Phases do not overlap in code ownership. Closed phases retain their entries here as historical record; current work is the Mod-OS Migration; Phase 9 (Native Runtime) remains the post-launch endpoint.

## Status overview

*Updated: 2026-05-03 (M7 closed; M8 (Vanilla skeletons) is the next phase; §9.2 v1.6 ratification candidate registered; UI redesign with Kenney+Cinzel pending; .sln gap fix pending; Phase 5 backlog tracked).*

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
| **M4 — Shared ALC** | ✅ Closed | added (`CrossAlcTypeIdentityTests`, `SharedAssemblyResolutionTests`, `ContractTypeInRegularModTests`, `SharedModComplianceTests`) | M4.1 `SharedModLoadContext` + two-pass loader + cross-ALC type identity; M4.2 `ContractValidator` Phase E enforces D-4 (no contract types in regular mods); M4.3 D-5 LOCKED shared-mod cycle detection + Phase F enforces §5.2 shared-mod compliance |
| **M5 — Version constraints** | ✅ Closed | added (`RegularModTopologicalSortTests`, `DependencyPresenceTests`, `M51PipelineIntegrationTests`, `PhaseAModernizationTests`, `PhaseGInterModVersionTests`, `M52IntegrationTests`) | M5.1 pipeline regular-mod toposort + dependency presence (`MissingDependency` / optional warning); M5.2 `ContractValidator` Phase A v1/v2 dual-path modernization + Phase G inter-mod version check; cascade-failure semantics ratified as deliberate accumulation (per §8.7) |
| **M6 — Bridge replacement** | ✅ Closed | added (`PhaseHBridgeReplacementTests`, `Phase5BridgeAnnotationsTests`, `CollectReplacedFqnsTests`, `M62IntegrationTests`) | M6.1 `[BridgeImplementation(Replaceable)]` + Phase 5 combat stubs annotated + `ContractValidator` Phase H bridge replacement validation; M6.2 `ModIntegrationPipeline` skip-on-replace graph build + integration tests across all §7.5 scenarios |
| **M7 — Hot reload** | ✅ Closed | 437/437 (50 commits) | M7.1 ✅ Pause/Resume + `IsRunning`; M7.2 ✅ ALC unload chain steps 1–6 + §9.5.1 best-effort failure semantics; M7.3 ✅ step 7 `WeakReference` + GC pump + `ModUnloadTimeout` + Phase 2 carried-debt closure; M7.4 ✅ D-7 build-pipeline `hotReload` override; M7.5.A ✅ `ModMenuController` editing-session lifecycle + `IModDiscoverer` + `Pipeline.GetActiveMods`; M7.5.B.1 ✅ production `GameBootstrap` integration via `GameContext`; M7.5.B.2 ✅ Godot `ModMenuPanel` modal overlay + F10 hotkey + menu-lifecycle pauses simulation. 6 housekeeping passes ✅ (TICK display, TickScheduler.ShouldRun race, real pawn data, NeedsSystem decay direction, ModMenuPanel position + assets gitignore, menu-pauses-simulation). Closure verified — see [M7_CLOSURE_REVIEW](./audit/M7_CLOSURE_REVIEW.md). §9.2 v1.6 ratification candidate registered for future cycle. |
| M8 — Vanilla skeletons | ⏭ Pending | — | Five empty mod assemblies |
| M9 — Vanilla.Combat | ⏭ Pending | — | Absorbs original Phase 5 scope |
| M10 — Remaining vanilla | ⏭ Pending | — | Magic, Inventory, Pawn, World — incremental |
| **K-series — Native ECS kernel** | ⏭ Pending | — | K0–K8 per [KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md) v1.0 LOCKED, 5–8 weeks |
| **K9 — Field storage abstraction** | ⏭ Pending | — | `RawTileField<T>` + conductivity / storage flags, CPU functional first; gates G-series ([GPU_COMPUTE](/docs/architecture/GPU_COMPUTE.md) v2.0) |
| **G-series — GPU compute** | ⏭ Pending | — | G0 plumbing → G1 mana → G2 electricity → G3 storage → G4 multi-field → G5 `ProjectileSystem` (Domain B) → G6–G9 flow-field pathfinding ([GPU_COMPUTE](/docs/architecture/GPU_COMPUTE.md) v2.0 LOCKED) |
| **M9.0–M9.8 — Vulkan + Win32 runtime** | ⏭ Pending | — | Per [RUNTIME_ARCHITECTURE](/docs/architecture/RUNTIME_ARCHITECTURE.md) v1.0 LOCKED, parallel-development cutover at M9.5, Godot deletion at M9.8 |
| Phase 9 — Native Runtime | ⏭ Post-launch | — | Separate large project (now decomposed into K-series + M9.x runtime above) |

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

- **Stub UI re-creation.** `BuildMenu`, `AlertPanel`, `ManaBar`, `PawnInspector`, and `ProjectileVisual` were deleted as dead code in housekeeping `9141bd6`. Phase 5 introduces them as real `Godot.Control` / `Godot.Node2D` subclasses with real data sources when the corresponding gameplay systems land (placement mode, alert/notification system, magic system, expanded pawn inspector). **Addressed by:** Phase 5.

- **`UIUpdateCommand` (re-create).** Generic notification surface; deleted in housekeeping `9141bd6` because no event subscriber published it. Reintroduce when the alert/notification system lands. **Addressed by:** Phase 5.

- **Pawn sprite replacement.** Current `PawnVisual` uses solid blue squares; Kenney roguelike packs (`assets/kenney_roguelike-rpg-pack/`, `assets/kenney_tiny-dungeon/`) have pixel-art sprites. Tied to `PawnVisual` + render command extensions; belongs with combat/magic system landing. **Addressed by:** Phase 5 / M9.

### Resolved

- **NeedsSystem decay direction.** Was decay-toward-0 (falsely implying automatic recovery via the `Math.Clamp(needs.Hunger - HungerDecayPerTick, 0f, 1f)` line and three siblings); flipped to deficit accumulation in housekeeping commit `ee12108`. Class-level XML doc rewritten to describe accumulating-deficit semantics with the explicit no-recovery caveat. Constants (`HungerDecayPerTick = 0.002f`, `ThirstDecayPerTick = 0.0015f`, `SleepDecayPerTick = 0.001f`, `ComfortDecayPerTick = 0.0005f`) unchanged — they were already calibrated for "deficit accumulation" pacing semantically. Regression guard `NeedsAccumulationTests` (3 facts) added in commit `7ea038c`. Recovery loop (food / water / bed entities + Eat/Drink/Sleep job execution) remains pending in Phase 5 — until then pawns visibly degrade indefinitely, which is the honest behaviour for an incomplete simulation.

- **Pre-Phase-1 latent race in `TickScheduler.ShouldRun` cache.** `Dictionary<Type, int>` populated from inside `Parallel.ForEach` via `ParallelSystemScheduler.ExecutePhase`, against the class's own "not thread-safe" claim. Surfaced by housekeeping commit `21921887`'s integration test (`CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`, ~60 % flake rate on a 5-run sample). Resolved by housekeeping commit `e0b0ecf`: `Dictionary` → `ConcurrentDictionary`, `TryGetValue + indexer` → `GetOrAdd(systemType, ResolveTicksPerUpdate)`. Class-level XML doc rewritten to truthfully document thread-safety guarantees. New regression guard `TickSchedulerThreadSafetyTests.ShouldRun_InvokedFromManyThreadsWithEmptyCache_DoesNotThrowAndReturnsConsistentResults` added in commit `52d6d3f` (32 distinct synthetic systems × 500 iterations; pre-fix fails 9/10 invocations; post-fix passes 10/10). Integration test fixed-flake guard: 100 consecutive runs of `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` pass 100/100 post-fix.

- **UI lies removed via real pawn data (housekeeping `9141bd6` feat + `659a64a` test).** Pawn names were a hardcoded Warhammer-flavoured array indexed by `EntityId.Index` in `PawnStateReporterSystem`; the role label was a hash-derived fabrication in `PawnDetail.MakeRole` against a 5-element role array (no role component existed); the skill bars were hash-derived placeholders in `PawnDetail.DemoSkills` while `SkillsComponent` was completely ignored at the display surface. Resolved by introducing `IdentityComponent` (single `Name` field) and `RandomPawnFactory` (deterministic seed-42 generation, 10 colonists, all 13 `SkillKind` values populated per pawn), extending `PawnStateChangedEvent` / `PawnStateCommand` with `TopSkills: IReadOnlyList<(SkillKind Kind, int Level)>`, rewriting `PawnStateReporterSystem` to read the real components, and re-driving `PawnDetail`'s SKILLS section from real top-3 data while removing the role label entirely. Eight dead files deleted in the same pass: `BuildMenu.cs`, `AlertPanel.cs`, `ManaBar.cs`, `PawnInspector.cs`, `ProjectileVisual.cs`, `ProjectileSpawnedCommand.cs`, `SpellCastCommand.cs`, `UIUpdateCommand.cs` (all confirmed unused by `grep -r` before deletion). Test count: 417 → 428 (+11). M-phase boundary preserved: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.

### Maximum engineering refactor (parallel track)

A three-track discipline escalation proposed and ratified in
[MAXIMUM_ENGINEERING_REFACTOR](./MAXIMUM_ENGINEERING_REFACTOR.md) v1.0.
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

Result: Godot works as both editor and temporary runtime. `DfDevKitPlugin`, `SceneExporter`, `EntityExporter` enable scene authoring and `.dfscene` export. `IRenderer`, `ISceneLoader`, `IInputSource` contracts exist with both Godot and Native backends. F5 in Godot starts the game with the loaded scene.

Architectural context: [VISUAL_ENGINE](/docs/architecture/VISUAL_ENGINE.md). The decoupling work here unblocks Phase 9 (Native Runtime) without committing to its timeline.

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

This is a deliberate interpretation of §8.7 wording "cascade-fail," registered here per [METHODOLOGY](./METHODOLOGY.md)'s "no improvisation" rule. If the interpretation needs revision, escalate via §12 ratification process.

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

Goal achieved: hot-reload path through `ModIntegrationPipeline` complete, including `AssemblyLoadContext` unload with WeakReference verification. Decomposed per [METHODOLOGY](./METHODOLOGY.md) §2.4 into seven implementation sub-phases (M7.1, M7.2, M7.3, M7.4, M7.5.A, M7.5.B.1, M7.5.B.2) plus a closure session, with 6 housekeeping passes interleaved.

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

**Deliberate interpretation of §9.2 / §9.3 — flag location on the pipeline, not the scheduler.** §9.2 step 1 reads "menu sets the scheduler's run flag to false"; §9.3 reads "enforced by `ModIntegrationPipeline` checking the scheduler's run flag." M7.1 locates the flag itself on `ModIntegrationPipeline` (private `_isRunning` bool) rather than introducing one inside `ParallelSystemScheduler`. Adding a flag to the scheduler would require modifying `DualFrontier.Core`, which would break the M-phase boundary discipline that M3–M6 maintained (no `DualFrontier.Core` touched by any Mod-OS Migration phase) and which the [M6 closure review](./audit/M6_CLOSURE_REVIEW.md) §8 footer explicitly carries forward to M7. The pipeline-mediated reading is consistent with §9.3's "`ModIntegrationPipeline` checking" wording and treats §9.2's "scheduler's run flag" as the run state observable to the scheduler from the outside (via the pipeline), rather than as state the scheduler itself owns. This is a deliberate interpretation registered here per [METHODOLOGY](./METHODOLOGY.md)'s "no improvisation" rule and the M5.2 cascade-failure precedent above. If a future M7 closure review finds this materially incompatible with §9 wording, the resolution is a v1.6 ratification rather than relocating the flag into the scheduler (v1.5 was consumed by audit campaign Pass 2).

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

**M10.B — Vanilla.Inventory.** The Phase 4 `InventorySystem`, `HaulSystem`, and the power-grid systems (`ElectricGridSystem`, `ConverterSystem`) migrate from kernel to mod. The kernel keeps the bus contracts and the `[Deferred]` event types; the systems move. This is a **refactor migration**, not new gameplay; covered by existing tests.

**M10.C — Vanilla.Pawn.** Consumes Phase 3 backlog: `SocialSystem` and `SkillSystem` get real implementations inside the mod. The kernel `JobSystem`, `MoodSystem`, `MovementSystem`, `NeedsSystem`, `PawnStateReporterSystem` migrate as part of the same refactor.

**M10.D — Vanilla.World.** Consumes original Phase 7 scope. Biomes affect resources, weather, passability. Ether nodes form coverage radii. Inter-faction relations per [GDD] §3.3. Raids by colony level. Trade caravans by season. Replaces kernel `BiomeSystem`, `WeatherSystem`, `MapSystem`, `RaidSystem`, `RelationSystem`, `TradeSystem` bridges.

**Acceptance criteria** (per slice, applied uniformly)

- All gameplay scope from the original Phase 5/6/7 spec delivered as mod content.
- Disabling the slice mod: the kernel runs with that domain silent; no crashes.
- The slice's `replaces` list covers every kernel bridge it supersedes; capability cross-check passes; test count grows with each slice.
- Inter-slice dependencies (e.g. Magic depends on Combat for damage types) declared via `dependencies` with caret-versioned constraints.

**Unblocks:** the full game. Subsequent expansions are content mods (community or first-party), shipped as additional regular mods that depend on the vanilla slices.

---

## ⏭ Native foundation tracks (K-series + G-series + Runtime M9.x)

The post-Phase-4 architectural pivots committed in [KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md) v1.0, [RUNTIME_ARCHITECTURE](/docs/architecture/RUNTIME_ARCHITECTURE.md) v1.0, and [GPU_COMPUTE](/docs/architecture/GPU_COMPUTE.md) v2.0 decompose the original "Phase 9 — Native Runtime" endpoint into three coordinated tracks. Each track has its own brief; this section is the execution-sequence summary.

### K-series — Native ECS kernel (5–8 weeks)

Per [KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md) Part 2:

- **K0** — cherry-pick + cleanup from `claude/cpp-core-experiment-cEsyH` branch.
- **K1** — batching primitive (bulk Add/Get + `Span<T>` access).
- **K2** — type-id registry + bridge tests (`DualFrontier.Core.Interop.Tests`).
- **K3** — native bootstrap graph + thread pool.
- **K4** — component struct refactor (Path α, ~50–80 components).
- **K5** — `Span<T>` protocol + write command batching.
- **K6** — second-graph rebuild on mod change.
- **K7** — performance measurement (tick-loop), report → `PERFORMANCE_REPORT_K7.md`.
- **K8** — decision step + production cutover (managed `World` retired or retained per §8 metrics).

### K9 — Field storage abstraction (1–2 weeks)

Prerequisite for all GPU compute work; ships CPU functional path first. Per [GPU_COMPUTE](/docs/architecture/GPU_COMPUTE.md) "K9 — Field storage abstraction".

- `RawTileField<T>` (C++): conductivity map, storage flags, ping-pong back buffer.
- C ABI: `df_world_register_field`, `field_read_cell`, `field_acquire_span`, `field_set_conductivity`, `field_set_storage_flag`.
- Managed bridge: `FieldRegistry`, `FieldHandle<T>`.
- CPU reference diffusion (used both as functional fallback and as G1+ shader equivalence oracle).

Exit criteria: any field type registrable / readable / writeable from managed; CPU diffusion correct on test grids; no GPU dependency.

### G-series — Vulkan compute integration (~6–8 weeks)

Per [GPU_COMPUTE](/docs/architecture/GPU_COMPUTE.md) Roadmap:

- **G0** — Vulkan compute pipeline plumbing (~1 week). `VkBuffer` / descriptor sets / pipeline registration / dispatch C ABI / fence-based sync / build-time SPIR-V for compute shaders (extends [RUNTIME_ARCHITECTURE](/docs/architecture/RUNTIME_ARCHITECTURE.md) §1.7).
- **G1** — first field compute shader, mana diffusion (~1 week). `Vanilla.Magic` ships `ManaField` + isotropic diffusion shader; CPU/GPU equivalence verified.
- **G2** — anisotropic diffusion, electricity (~1 week). `Vanilla.Electricity` + conductivity map + cliff-threshold consumer effectiveness.
- **G3** — storage cells / capacitance (~3–5 days). Batteries, water tanks; α/β retention and release.
- **G4** — multi-field coexistence (~3–5 days). Mana + Electricity + Water concurrent; GPU memory budget verified.
- **G5+** — Domain B integration: `ProjectileSystem` reactivation (~1 week). Phase 3 deferral unwound on the post-pivot architecture; threshold pinned per measurement.
- **G6** — flow field infrastructure (~3–5 days). Distance + direction field shaders, `Vector2` field type.
- **G7** — `Vanilla.Movement` integration (~1 week). Flow field lifecycle, hybrid with A\* fallback for unique destinations.
- **G8** — local avoidance layer (~3–5 days). RVO/boids steering on top of flow field gradient.
- **G9** — eikonal upgrade (optional, ~1 week, evidence-gated). Fast Sweeping Method for geodesic-accurate distances.
- **Future G milestones** — climate, fire spread, sound, scent, pollution, radiation, alarm propagation, terrain affordances, modder fields. ~3–5 days each, compositional with the established infrastructure.

### M9.0–M9.8 — Vulkan + Win32 runtime (~3–6 weeks)

Per [RUNTIME_ARCHITECTURE](/docs/architecture/RUNTIME_ARCHITECTURE.md). Replaces Godot DevKit production path with own Vulkan rendering + Win32 windowing. Parallel-development cutover at M9.5; Godot deletion at M9.8. Domain layer preserved verbatim — `Vanilla.{Combat, Magic, ...}` runs identically under DevKit and under the new runtime because both consume the same `IGameServices` contracts.

### Combined timeline

K0–K8 (5–8w) + K9 (1–2w) + G0–G5 (~5–6w) + G6–G9 (~3–4w) + M9.0–M9.8 (~3–6w) ≈ 17–26 weeks for the full architectural foundation, runnable in parallel where dependencies allow ([KERNEL_ARCHITECTURE](/docs/architecture/KERNEL_ARCHITECTURE.md) §8 governs cross-track sequencing). Ordering: K-series gates K9; K9 gates G-series; M9.x is independent of the K/G tracks once Vulkan handles are exposed (G0 prerequisite is M9.0–M9.4 having loaded `vulkan-1.dll` into the runtime).

### When

Original "post-launch" framing is preserved as the outer envelope, but the K/G/M9.x tracks may begin during the M-cycle wherever non-blocking — particularly K9 (CPU-only, no GPU dependency) which can land alongside the M-cycle without disrupting it. Cutover decisions remain at K8 and M9.5/M9.8.

**Why this is possible:** the architecture is already ready. The Mod-OS Migration does not change the relationship between Application and Presentation; `Vanilla.Combat` runs identically under Godot and under the future native runtime, because both consume the same `IGameServices` contracts. Field-based mechanics ship as additional vanilla mods registering through the same `IModApi` ([GPU_COMPUTE](/docs/architecture/GPU_COMPUTE.md) "Mod parity (KERNEL K-L9)").

---

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

## See also

- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) — v1.5 LOCKED specification driving M1–M10.
- [METHODOLOGY](./METHODOLOGY.md) — the four-agent pipeline; M1–M10 are exercised through it.
- [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md) — the four layers; the Mod-OS migration touches only Application and below.
- [CONTRACTS](/docs/architecture/CONTRACTS.md) — bus and marker conventions; capability syntax mirrors bus naming.
- [MODDING](/docs/architecture/MODDING.md) — v1 mod-author guide; M-phase outputs supersede this document for v2.
- [MOD_PIPELINE](/docs/architecture/MOD_PIPELINE.md) — `ModIntegrationPipeline` mechanics; M2, M5, M6, M7 extend it.
- [TESTING_STRATEGY](./TESTING_STRATEGY.md) — test discipline; M-phase acceptance criteria slot into the existing isolation/modding/integration tiers.
- [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) — PR hygiene; the Mod-OS migration is exercised through the same checklist.
- [IDEAS_RESERVOIR](./IDEAS_RESERVOIR.md) — post-release ideas reservoir; populated, not scheduled. Read after Phase 7 closure.
- [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md) — architectural contract for fully homomorphic encryption multiplayer; ratified at v1.0, activation deferred per its §D1.
- [MAXIMUM_ENGINEERING_REFACTOR](./MAXIMUM_ENGINEERING_REFACTOR.md) — three-track discipline escalation; ratified v1.0, activation deferred per-track.
