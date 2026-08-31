---
register_id: DOC-D-W4_COMPOSITION_ROOT_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft
owner: Volodymyr (Crystalka)
version: '1.1'
first_authored: '2026-08-30'
last_modified: '2026-08-30'
content_language: en
next_review_due: null
title: 'W4_COMPOSITION_ROOT -- wave W4 (BD-4 + BD-8): the distribution manifest ships and the boot-time mod-load path is CREATED (production loads zero mods today); GameBootstrap dissolves into a neutral engine composer + a new vanilla.scenario mod that owns component registration, factories, seeds, pathfinding provision, and the 10 real system registrations in place; the presentation bridge goes mod-side over a neutralized G2-widened command channel; the 4 engine->game ProjectReference edges are CUT so the gate is compiler-enforced (ratchet baseline 4->0); the extended BD-8 orphan cluster (344 lines / 11 files re-measured) dies; executor runs a mandated Wave R subagent survey before writing. v1.1 (operator ratification 2026-08-30, post-Wave-R H4/H6 halt): section 2.7 REFUTED five ways, so the wave additionally GROWS THE SDK -- an IModApi factory registration overload (closing both the parameterised-ctor and the unreachable-provision blockers), INavGridService, typed presentation effects, ContractsVersion 2.1.1->2.2.0 MINOR; identity/movement/skills RE-TYPE onto Contracts-side storage (managed-only, native byte-untouched); SystemBase.Initialize gains a once-guard; the runtime mods root anchors at AppContext.BaseDirectory; ItemFactory gains the test floor it never had; the gameplay fault-policy inversion to quarantine is ACCEPTED; the vanilla skeletons are ledgered rather than repaired'
authored_by: Claude Fable (architect session, W4 charter)
basis: 'DOC-E-W4_COMPOSITION_ROOT_RECON_REPORT (Opus subagent, read-only, 2026-08-30 at 34bc221, sections A-H + 9 discrepancies + 10 surprises) + ratified lean set 2026-08-30 (L1 assembly-scoped gate + typed strict manifest + boot Apply fail-fast; L2 vanilla.scenario mod as content receiver + SystemBase-path registration in place; L3 presentation-command neutralization + G2-widened mod-side channel; L4 extended BD-8 deletion + doc riders + fences) + operator additions 2026-08-30 (Wave R subagent survey mandated in-session; the UI design handoff MAY be unpacked OUTSIDE the repo for consultation) + VANILLA_SEPARATION_MIGRATION_PLAN.md W4 law (:173-177) + boundary law B-5 (GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md:75-77)'
---

# W4_COMPOSITION_ROOT -- Execution Brief

Wave W4 makes the composition honest. Today `GameBootstrap.CreateSession` hardwires
the whole vanilla game into the engine assembly (21 component registrations, 10
system registrations, two factories, 11 scenario consts, five bridge subscriptions)
-- and, the recon's central surprise, **never loads a single mod**: the pipeline is
built and idle, `Apply` sits behind a menu the Launcher never opens. This cascade
ships the distribution manifest (BD-4), CREATES the boot-time mod-load path,
dissolves GameBootstrap into a neutral engine composer plus a new
`dualfrontier.vanilla.scenario` mod that owns the vanilla content, moves the
presentation bridge mod-side over a neutralized command channel, deletes the
extended BD-8 orphan cluster, and cuts the 4 engine->game `ProjectReference` edges
so the wave gate becomes compiler-enforced. Done = the Launcher boots FROM
`game.manifest.json`, the vanilla colony runs as before (50 pawns, items, movement,
render) with the content owned by mods, `DualFrontier.Application.csproj` carries
ZERO references to Components/Events/Systems/AI, the ratchet baseline reads 0, and
the full gate set is green with the live Launcher smoke passing.

Executor: fresh Claude Code session (flagship model), LOCAL, repository
`/mnt/Work/Colony_Simulator` (GitHub `Crystalka228/Dual-Frontier`). MANAGED-ONLY:
`native/` byte-untouched (H7).

Brief-integration notice: this brief CITES standing law and does not restate it --
commit-body structure per `CODING_STANDARDS.md`; push law per section 8.4 (v3.0.0);
census pins per `TESTING_STRATEGY.md`; mutability license per
`RESERVED_SURFACE_MUTABILITY.md`; session closure per `METHODOLOGY.md`. A conflict
between this brief and any standing doc means THE BRIEF IS WRONG -- halt and
escalate. A conflict with the live code means THE CODE IS THE TRUTH -- record and
proceed per section 2's re-verify discipline.

## 1. Mission [CORE]

| #  | Deliverable | Action |
| -- | ----------- | ------ |
| D1 | This brief + the recon report | enroll (frontmatter + sync); brief Draft -> EXECUTED at closure |
| D2 | `game.manifest.json` + typed STRICT loader | BD-4 full schema; engine-side loader (Application), fail-fast on unknown keys and on any invalid value; the manifest file ships at the distribution root |
| D3 | Neutral engine composer + boot-time mod load | GameBootstrap dissolves; a composer builds EngineSession from ENGINE parts only and calls `Apply(rootMods)` at session start, fail-fast on refusal |
| D4 | `dualfrontier.vanilla.scenario` mod | NEW regular mod (game-side csproj): 21 component registrations, factory FILES relocated, seeds from the manifest scenario section (re-entry-safe read-then-mint), pathfinding provision, the 10 real SystemBase system registrations IN PLACE |
| D5 | Presentation channel | the two enum-carrying command records neutralized (primitives); command records + sink surface exposed to mods (G2 widening); the five bridge subscriptions become the scenario mod's presentation translation |
| D6 | The gate cut | 4 `ProjectReference` edges deleted from `Application.csproj`; `BoundaryRatchetTests` baseline 4 -> 0 (shrink protocol + census-delta); test blast radius rewritten to the composer+manifest world |
| D7 | Extended BD-8 deletion | `Scene/` folder (9 files), `ScenarioDef` + `ScenarioLoader`, the 11 consts (values move to the manifest); census-deltas recorded |
| D8 | Doc + ledger truth | MOD_OS :570 staleness fixed; plan/EAM anchor drift corrected; dead `Systems->AI` edge cut; EAM R13 ownership seated; plan W4 write-back; F-rows; closure EVT |
| D9 | Wave R survey (operator-mandated) | four read-only subagents run IN-SESSION before any edit; inventories are the writers' substrate |

Why now: ratified queue (W4 after LINUX_PRESENT_1). The UI program and W5 slice
moves both land on this composition.

## 2. Established facts [CORE]

(RV) facts re-verified at Phase 0; mismatch -> H1. Anchors at HEAD
`34bc221c569c95645b81a2cf136e04487752b9b2`. Source: the recon report (sections A-H,
S# = SURPRISES, D# = DISCREPANCIES).

**The composition today:**

1. (RV) `GameBootstrap.cs` (284 lines, `Loop/`): the 25-step `CreateSession`
   sequence (recon A.3 is the authoritative map); 11 scenario consts at `:65-75`
   (BD-8's `:58-68` anchor is stale -- D1); the ONLY production caller is
   `Launcher/Program.cs:55`; 18 integration tests + 8 transaction tests are the
   blast radius (recon H.2).
2. (RV) **Production loads ZERO mods** (S1): `CreateSession` never calls
   `pipeline.Apply`; the sole `Apply` site in src/ is `ModMenuController.cs:273`
   behind `BeginEditing()->Commit()`; the Launcher never opens the menu. The
   boot-time load path DOES NOT EXIST and must be created.
3. (RV) The forbidden-namespace footprint in `DualFrontier.Application` is exactly
   **6 files / 24 using-sites / 0 fully-qualified** (S3): `Loop/GameBootstrap.cs`
   (7), `Bootstrap/VanillaComponentRegistration.cs` (7), `Scenario/RandomPawnFactory.cs`
   (4), `Scenario/ItemFactory.cs` (4), `Bridge/Commands/PawnStateCommand.cs` (1 --
   `SkillKind` only), `Bridge/Commands/ItemSpawnedCommand.cs` (1 -- `ItemKind`
   only). The 4 edges live at `Application.csproj:12,14,15,16` (v1.1 correction: the
   brief and recon both said `:11`; line `:11` is `DualFrontier.Core`, an ENGINE
   edge -- re-measured at Phase 0).
4. `EngineSession.cs` ALREADY has zero game-namespace references (D2, B-5
   discipline since EQ_A2) -- the falsifiable W4 gate is the ASSEMBLY-scoped cut of
   the 4 edges; `BoundaryRatchetTests` (`ProjectReferenceBaseline` 4 edges,
   `:55-61`) measures exactly this and its shrink protocol (:27-29) sanctions 4 -> 0
   with a census-delta. The B-6 analyzer remains the endpoint (its own F-row; the
   ratchet class deletes only when the analyzer ships, :23-25).

**Lifecycle + SDK surfaces the design rests on:**

5. `ISimulationSystem.Initialize(ISystemContext)` runs with a LIVE context (world
   access legal) but fires PER GRAPH BUILD -- scheduler ctor + every Rebuild, i.e.
   every `Apply` (S2). Seeding must be re-entry-safe: the ratified pattern is the
   Weather lazy read-then-mint in `Tick` (`WeatherSystem.cs:49-53`, `:119-130`).
6. `IModApi` has NO world access (D.2 -- 12 members, none touch entities);
   `IMod.Initialize` is registration-only. `ISystemContext` has the full 18-member
   world surface (D.4).
7. (RV) `ModRegistry` ALREADY supports mod registration of `SystemBase` types --
   the measured bridge path (`RegisterSystem(modId, systemType)` dispatch at
   `ModRegistry.cs:263-265`, direct-instance arm `:460-479`, F60A-verified). A mod
   whose csproj references the `Systems` assembly (game->game -- LEGAL, the ratchet
   fences engine->game only, and the Example Contracts-only ratchet binds Example
   alone) can therefore register the 10 real systems WITHOUT moving their files.
   File moves are W5 territory.
8. `ISystemServices` has exactly ONE member (`IPathfindingService Pathfinding`);
   today the engine constructs `NavGrid` + `AStarPathfinding` (AI assembly) and
   feeds `SetSystemServices` (`GameBootstrap.cs:105-113,:145`) -- after the cut the
   ENGINE CANNOT construct these; provision must come from the game side (7.4).
9. The five bridge subscriptions (`GameBootstrap.cs:92-103`) translate 5 game
   events into render commands; the bootstrap lambdas are ratified SACRIFICIAL
   (EQ_A2 D8). `WindowResizeEvent`-style caution does not apply here, but the
   translation itself is load-bearing for the vanilla render.

**Manifest + discovery surfaces:**

10. `ModManifest`/`ManifestParser`: manifestVersion strict `"3"`, unknown keys
    SILENTLY IGNORED (S8 -- the game manifest loader must be STRICT by ratified
    lean); `description` is already an ignored stray in 3 shipped manifests.
11. `DefaultModDiscoverer`: filesystem-order, one level, silent parse-failure
    swallow (S9) -- NO ordering semantics exist, so the manifest's ordered
    `rootMods` is ADDITIVE, replacing nothing.
12. (RV) mods/ inventory: 9 discoverable ids; 6 vanilla skeletons own NOTHING
    (Vanilla.Core is shared and empty; 5 regular genre skeletons have empty
    Initialize/Unload); only Weather + Example register anything (C.6).
13. EAM R13 is VACANT (config ownerless; the row demands an ownership decision
    before features land in it) -- this cascade SEATS the owner: the distribution
    manifest (L4) + owning mods (L5).

**The extended orphan cluster (BD-8, re-measured -- S5):**

14. (RV) `Scenario/ScenarioDef.cs` (31) + `Scenario/ScenarioLoader.cs` (50, ZERO
    callers) + the WHOLE `Scene/` folder (9 files, 214 lines, sole consumer = one
    serialization test) = ~295 lines / 11 files. `ScenarioDef.WorldSeed` is the
    only modelled world seed in the repo (S10 -- F-42 coordination: the manifest
    CARRIES the seed; the RNG service is NOT built here).

**Known doc drift the cascade corrects (D-series + S6):** BD-8 anchors `:58-68`
-> `:65-75`; plan stock counts (27 systems not 29-as-classes; 51 event types not
52; 19 AI types not 20); plan `:70-71` stale "mods absent from sln"; EAM anchors
(`ParallelSystemScheduler.cs:90`->`:112`, `GameBootstrap.cs:145-148`->`:167-170`,
`:170-179`->`:192-201`, R11's `:241` now fixed by ID-B); F-42's `:96`->`:106`;
`MOD_OS_ARCHITECTURE.md:570` claims F-60 open and cites two renamed tests.

**Adjacent truths fenced OUT (section 15):** `GameServices` genre nouns live in
engine `Core.Bus` (S7 -- invisible to the namespace gate; F-row, not W4); dead
`Systems->AI` csproj edge (S4 -- cut here as a one-line rider, it is game->game and
outside the ratchet); BT/Jobs zero consumers (BD-5/W5).

## 3. Phase 0 -- preconditions and checkpoint [CORE]

1. **Verify recon facts**: HEAD `34bc221` (or a fast-forward not touching
   section-2 anchors); every (RV) fact re-measured. Mismatch -> H1.
2. **Native kernel present** (the Launcher smoke needs it): canonical Linux build
   if the `.so` is absent; selftest 110 ALL PASSED.
3. **Baseline gates** (regression anchor, verbatim): build 0W/0E; full suite --
   expected shape 1374 / 0 / 6 (5 F-10 + 1 validation-layer); the live Launcher
   smoke ONCE (timeout-15s protocol, exit 124, clean log) -- W4 must not regress
   what LP1 paid for. Never pipe `dotnet test`.
4. **Validation checkpoint**: `dotnet run --project tools/DualFrontier.Governance
   -- validate --armed` exit 0 -> else H3.
5. **Frontmatter-shape read** (Lesson #N14) + verify the recon report's
   register_id is absent (collision -> H5) and its frontmatter PARSES (the F60A
   lesson -- hold untracked files aside to prove which artifact fails).
6. **Mandatory reads**: the recon report FULL; this brief FULL; `GameBootstrap.cs`,
   `EngineSession.cs`, `GameLoop.cs`, `ModIntegrationPipeline.cs` `Apply` (:228-575)
   + unload chain, `ModRegistry.cs` (registration paths incl. :460-479),
   `ContractValidator.cs` phase docs, `ManifestParser.cs`, `DefaultModDiscoverer.cs`,
   `IModApi.cs`, `ISystemContext.cs`, `ISystemServices.cs`, `SystemContextView.cs`,
   the six coupled files, `WeatherMod.cs` + `WeatherSystem.cs` (the seed pattern),
   `BoundaryRatchetTests.cs`, `CensusMetaTests.cs`, `GameBootstrapIntegrationTests.cs`,
   `EngineSessionTransactionTests.cs`, `VANILLA_SEPARATION_MIGRATION_PLAN.md`,
   `GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md` (B-5, B-6),
   `EXECUTION_AUTHORITY_MATRIX.md` (R2/R9/R13), `METHODOLOGY.md` closure,
   `CODING_STANDARDS.md` 8.4.

`sync` in EVERY frontmatter-touching commit; derived registers never hand-edited;
`AUDIT_TRAIL.yaml` append-only. Closure: push the WORK BRANCH + PR; never `main`.

## 4. Topology [CORE]

**Orchestrator + Wave R (operator-mandated), then serial writes.** The four
read-only survey subagents run AFTER Phase 0, BEFORE any edit, in parallel; each
returns a structured inventory; the C-R checkpoint reconciles them against section
2. Writers are then SERIAL (single orchestrator; only the orchestrator runs
`git add/commit` -- atomic discipline is incompatible with parallel committers).
Single-writer files: `ROADMAP.md`, `AUDIT_TRAIL.yaml`, all governance surfaces.

## 5. Wave R -- survey agents [CORE for this cascade]

Each agent is READ-ONLY (no edits, no builds, no git mutations), returns an
inventory on a fixed schema (findings with file:line anchors + counts with
commands + an explicit DISCREPANCIES-vs-brief list). A writer may not state a
claim absent from its inventory.

- **R1 -- manifest + boot-load front.** The full `Apply` semantics a boot call
  must satisfy (fault handling, rollback, warnings vs errors, the menu's
  Begin/Commit interplay so boot-load and menu-editing do not fight); discoverer
  interaction; where the manifest file must live so the Launcher, tests, and
  `dotnet run` cwd all resolve it; how `RuntimeOptions.AssetsDirectory` and the
  window options thread through (BD-4 fields the Launcher consumes at once vs
  reserves).
- **R2 -- dissolution front.** Per-line disposition map of `GameBootstrap`
  (every one of the 25 steps -> composer | scenario mod | dies); the six coupled
  files' full consumer graphs; the COMPLETE test blast radius (name every test
  that reddens, per deliverable) including `Core.Benchmarks` IVT uses of the
  factories.
- **R3 -- presentation-channel front.** Command records + `RenderCommandDispatcher`
  + `LauncherRenderer` consumption map; the exact minimal command-type set the
  vanilla render needs; `IPresentationSink`/`SetAmbientTint` (G2) wiring end to
  end; AND the UI handoff consultation: **unpack
  `Special/Game engine interface design.zip` OUTSIDE the repository** (session
  scratchpad ONLY -- never into the tree, perimeter law) and read `HANDOFF.md` +
  `tokens.json` to report which command-channel shapes the UI program will need,
  so the neutralized records are not re-broken at UI-0. Report facts, no design.
- **R4 -- mod-host front.** Verify the SystemBase mod-registration path end to end
  against a live trace (what `SystemMetadataBuilder`/quarantine/identity do with a
  mod-registered engine-assembly type); `SetSystemServices` consumers and timing;
  the ContractValidator phases a Systems-referencing mod must survive (Phase E
  exports check -- the scenario mod must not EXPORT IEvent types; verify
  referencing the Events assembly does not trip it); vanilla mod csproj/manifest
  shapes; where `ProvideService`-style SDK additions would land (IModApi ->
  RestrictedModApi -> registry plumbing).

## 6. Checkpoints [CORE]

- **C-R** (after Wave R): reconcile inventories against section 2; material
  contradiction beyond explained measurement deltas -> H4. Re-derive every
  subtotal from its own rows (the LP1 lesson -- prose totals are not evidence).
  Hand each writer phase its inventory slice.
- **C-manifest** (after C3): the Launcher boots from the manifest with the
  UNCHANGED composition (content still engine-side) -- proves the loader + boot
  Apply path before the content moves. Deliberate-break: a manifest with an
  unknown key and one with a missing root mod must each fail fast, loudly, named.
- **C-content** (after C5): the vanilla colony runs from the scenario mod --
  50 pawns spawn, items seed, movement/needs run, render shows sprites; the
  GameBootstrap integration suite (rewritten) is green; unload/reload of the
  scenario mod through the menu leaves the engine healthy (the W3 gate lens).
- **C-cut** (after C6): `Application.csproj` has zero game `ProjectReference`
  edges; ratchet baseline 0 green; full suite green; deliberate-break: re-add one
  edge -> ratchet reddens -> remove.
- **C-smoke** (before closure): live Launcher smoke, LP1 protocol, on the FINAL
  composition. Fail -> H8.
- **C-audit**: truth-law self-audit; census-deltas all recorded; citation-form.

## 7. Execution / writer specifications [CORE]

Intended forms are mutable surface; deviations -> `Skeleton revisions`;
architectural forks -> H6. No LINQ/no async in src/. All new P/Invoke-free.

**7.1 D2 -- the manifest (C2).** `game.manifest.json` at the DISTRIBUTION ROOT
(repo root beside `mods/` and `assets/`; the loader takes an explicit path with
that default). Full BD-4 typed schema, all fields present from day one, consumers
minimal: `manifestVersion` (strict "1"), `product` {id, name, version},
`rootMods` [ordered ids], `scenario` {id, worldSeed, mapWidth, mapHeight,
obstacleCount, obstacleSeed, factorySeed, itemFactorySeed, counts {pawns, food,
water, beds, decorations}}, `assetRoots` [paths], `saveNamespace` (reserved
consumer: W7), `minEngineCapabilities` [] (reserved). Loader: engine-side
(`Application`, intended name `DistributionManifestLoader`), System.Text.Json,
**STRICT**: unknown key -> throw naming the key; missing required -> throw; the
contrast with the forgiving mod parser is deliberate and documented in the class
doc. The typed scenario record lands in `Contracts` (intended:
`Contracts/Distribution/ScenarioConfig.cs`) so mods can consume it;
`IModApi` gains a `ScenarioConfig? Scenario { get; }` (null when the host has
none -- tests unaffected). ContractsVersion MINOR bump.

**7.2 D3 -- the composer + boot load (C3).** GameBootstrap DISSOLVES (the EQ_A2
sacrificial ruling licenses cutting harness logic freely). A neutral composer
(intended: `Loop/EngineComposer.CreateSession(PresentationBridge, DistributionManifest, hooks?)`)
builds ONLY engine parts: world (`Bootstrap.Run(useRegistry: true)`), services,
ticks, registry (+`SetTickSource`, `SetPresentationSink`), EMPTY core-system set,
graph, native-graph clear+compute, loader/fault-handler, metadata, scheduler,
validator/store/pipeline, discoverer (root from manifest? the discoverer root
stays `"mods"` -- rootMods selects WITHIN discovery), menu controller, bus bridge,
loop, session, quarantine observer -- and then **`pipeline.Apply(rootModPaths)`**
resolved from `rootMods` ids via the discoverer, in manifest order. Any root mod
missing, failing validation, or refused -> the composer THROWS (fail-fast; a
distribution without its root set is not viable) -- deliberate contrast with the
menu path's warning semantics, stated in the doc. Launcher: reads the manifest,
passes window/assets values through to `RuntimeOptions` (title/width/height stay
Launcher-consumed defaults unless the manifest carries them -- intended: manifest
does NOT model the window in v1; record as a reserved field decision), calls the
composer. `Initialize`-re-entry note (S2) recorded in the composer doc.

**7.3 D4 -- the scenario mod (C4).** NEW `mods/DualFrontier.Mod.Vanilla.Scenario`
(regular kind, `dualfrontier.vanilla.scenario`, depends on `vanilla.core`),
csproj references: `Contracts` + `Components` + `AI` + `Systems` + `Events`
(game->game; the Example Contracts-only ratchet binds Example alone -- do NOT
extend it here). Owns:
- **Component registration**: the 21 `RegisterComponent<T>` calls (the
  `VanillaComponentRegistration` content moves here; the Application file dies).
  Explicit-id parity is NOT required -- ids are owner-scoped since ID-A; Wave R4
  verifies the identity story for engine-assembly types registered by a mod and
  the brief's H6 catches a fork.
- **Factories**: `RandomPawnFactory` + `ItemFactory` FILES relocate into the mod
  (they are game logic); reworked to the SDK world surface where their current
  `NativeWorld` internals do not travel -- Wave R2/R4 inventories decide the exact
  seam; equivalence obligation: same counts, same seeds, same component sets.
- **Seeding**: a scenario system (intended `ScenarioSeedSystem`, SystemBase or
  ISimulationSystem per R4's finding) seeds ONCE, re-entry-safe (read-then-mint on
  a marker the way Weather does), consuming `api.Scenario`; obstacles feed the
  NavGrid it builds.
- **Pathfinding provision**: the mod constructs `NavGrid` + `AStarPathfinding`
  and provides the service. Intended SDK form: `IModApi.ProvidePathfinding(IPathfindingService)`
  routed to `ModRegistry.SetSystemServices` (single-provider, second provision =
  loud error); H6 if Wave R4 shows a cleaner ratified seam (e.g. the contract
  store).
- **The 10 system registrations** via `api.RegisterSystem<T>` on the measured
  SystemBase path -- files stay in `src/DualFrontier.Systems` until W5.
- **Presentation translation** (with 7.4's channel): subscribes the 5 events,
  emits neutral commands.
- The 17 stubs: NOT registered by the scenario mod (they were never registered by
  GameBootstrap either -- parity holds).

**7.4 D5 -- the presentation channel (C5, coupled with 7.3).**
- Neutralize the two records: `PawnStateCommand.SkillKind` -> the primitive the
  UI consumes (R3 reports; intended `byte`/`int` + the mapping owned by the
  publisher), `ItemSpawnedCommand.ItemKind` -> `int`. Renderer arms updated.
- Widen G2: the command records move to a mods-visible surface (intended:
  `Contracts/Presentation/` records of primitives + `ISystemContext.EnqueueCommand(...)`
  or a typed-per-command set mirroring `SetAmbientTint` -- R3's inventory decides
  the minimal honest set; MINOR ContractsVersion bump rides 7.1's).
- The five subscriptions die with GameBootstrap; the scenario mod re-creates the
  translation mod-side. Behavioural equivalence: the same five event flows reach
  the renderer (C-content proves via the existing PawnState/TickAdvanced tests
  rewritten).

**7.5 D6 -- the cut (C6).** Delete the 4 edges (`Application.csproj:11,14,15,16`);
`BoundaryRatchetTests.ProjectReferenceBaseline` -> empty (shrink + census-delta in
the commit body); rewrite the test blast radius (R2's inventory is the work
order): the 18 GameBootstrap tests become composer+manifest tests (same
behaviours: 50 pawns, menu flows, discoverer pin, loop round-trip), the 8
transaction tests re-harness, `WeatherHarness` untouched (it does not use
GameBootstrap -- verified F60A). `Core.Benchmarks` factory IVT: R2 reports; the
benchmarks follow the factories or pin against the mod -- intended: follow.

**7.6 D7 -- deletions (C7).** The extended orphan cluster: `Scenario/ScenarioDef.cs`,
`Scenario/ScenarioLoader.cs`, `Scenario/README.md` staleness, the whole `Scene/`
folder + its serialization test, `assets/scenes/` sample + README if orphaned with
it (R2 confirms), `Bootstrap/VanillaComponentRegistration.cs` (content moved by
7.3). Census-deltas for every `CensusMetaTests` pin the deletions move (S5 list;
`git ls-files src/**/*.cs` count 421 changes). Build-green-as-inertness proof per
deletion commit; HISTORICAL keep-set: none of these is a historical record --
state the explicit-none.

**7.7 D8 -- docs + ledger (C8/C9).** `VANILLA_SEPARATION_MIGRATION_PLAN.md`: W4
write-back + stock-count corrections + stale-prose fixes (D3-D5 list) -- PATCH/
MINOR per its change-history convention; `EXECUTION_AUTHORITY_MATRIX.md`: R13
seated (owner = distribution manifest + owning mods), R9 updated (composer is the
code owner now), anchor drift fixed; `MOD_OS_ARCHITECTURE.md:570` corrected to
the F-60-closed truth with the renamed test names; `GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md`
scenario rows updated to shipped reality. Dead `Systems->AI` edge cut (one line,
game->game, outside ratchet -- census-delta only in prose). F-rows at closure:
B-6 boundary analyzer (Planned, ROADMAP pointer -- the ratchet deletes only when
it ships); `GameServices` genre-noun vocabulary in engine Core (S7, honest-record);
F-42 coordination note appended to its row (the manifest now carries the seeds;
the service remains unbuilt); the `description`-key silent-ignore in ManifestParser
(honest-record, ties to the strict-loader contrast). ROADMAP: W4 DONE with hashes.

## 8. Kind-specific machinery [KIND: phase-execution]

Wave R (section 5) is this cascade's survey machinery. Per-behaviour test
obligation: every moved behaviour carries its test at the new owner (seed counts,
registration parity, translation flows); every deleted behaviour's test dies WITH
it in the same commit (no orphaned green).

## 9. S-LOCK invariants [CORE]

The compiler-enforced edge cut IS the wave's structural lock (a `ProjectReference`
cannot silently return -- the build fails without it only if code needs it, and the
ratchet pins the baseline at 0 against re-addition). No new analyzer this wave;
B-6 is the ledgered endpoint. Claiming more would breach the truth law.

## 10. Census discipline [CORE]

- HARD pins unchanged: DFK-WAIVER **3**; native exports **209**; `native/` diff
  **0** (H7); `Console.Write` in src **2**.
- Ratchet baselines MOVE BY DESIGN: ProjectReference 4 -> **0**;
  InternalsVisibleTo baseline unchanged (1: Core -> Systems -- W5 territory).
- `CensusMetaTests` pins move with the deletions -- each movement carried by a
  census-delta in the deleting commit's body (reserved-surface sites/files, stub,
  deferred, TODO, Phase 6, tracked-file count). Record before/after per pin.
- New censuses recorded at closure: game.manifest loader strict-key list count;
  scenario-mod registration counts (21 components / 10 systems).

## 11. Commit plan [CORE]

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(enroll): W4_COMPOSITION_ROOT brief + recon enrolled` | brief + recon + sync + validate |
| C2 | `feat(distribution): game.manifest.json ships with a strict typed loader` | 7.1 + ScenarioConfig in Contracts + IModApi.Scenario |
| C3 | `feat(composition): neutral engine composer; the boot-time mod-load path exists` | 7.2; content still engine-side; C-manifest proof |
| C4 | `feat(vanilla): the scenario mod owns registration, factories, seeds, pathfinding, systems` | 7.3 (+ manifest rootMods gains the mod) |
| C5 | `feat(presentation): neutral command records; the bridge translation goes mod-side` | 7.4; C-content proof |
| C6 | `refactor(boundary)!: the four engine->game edges are CUT; ratchet baseline 4 -> 0` | 7.5 + test rewrites; C-cut proof |
| C7 | `chore(orphans): the extended BD-8 cluster dies` | 7.6 + census-deltas |
| C8 | `docs(architecture): W4 truth -- plan/EAM/MOD_OS/boundary write-backs` | 7.7 docs |
| C9 | `governance(closure): W4_COMPOSITION_ROOT EVT + ROADMAP write-back -- W4 DONE` | EVT + F-rows + brief EXECUTED + sync + validate |

Intended-form count; splits recorded, history never compressed. C3 before C4 is
load-bearing (the boot path is proven on the unchanged composition first).

## 12. REGISTER cascade [CORE]

Schema-2.0, Phase 0 verbatim shapes. C1 enrolls brief (D/3/Draft) + recon
(E/3/EXECUTED, frontmatter pre-written -- shape-verify). C9 flips the brief to
EXECUTED, appends ONE closure EVT with real C1-C8 hashes, re-syncs. Doc version
bumps per 7.7 follow each doc's own change-history convention. `validate --armed`
exit 0 at every frontmatter-touching commit. Vocabulary gap -> H5.

## 13. Halt conditions (H-series) [CORE]

- **H1** Phase 0 mismatch. **H2** gate regression vs baseline (incl. the smoke).
- **H3** validate nonzero. **H4** Wave-R material contradiction with section 2.
- **H5** REGISTER vocabulary gap. **H6** an intended form proves architectural --
  named triggers: the SystemBase mod-registration path cannot carry an
  engine-assembly type cleanly (identity/quarantine/metadata fork); the
  pathfinding provision needs more than a single-provider setter; Phase E rejects
  the scenario mod for referencing Events; the presentation widening demands more
  than additive Contracts changes. STOP and surface -- these are ratification
  questions, not improvisation.
- **H7** any `native/` modification. **H8** the C-smoke or a deliberate-break
  proof fails (incl. a break that does NOT redden -- a vacuous gate).
- **H9** executor display absent (LP1 rule).
- Standing rails: push law 8.4 v3.0.0; derived registers never hand-edited;
  AUDIT_TRAIL append-only; no history rewrite; `historical/` read-only; never
  pipe `dotnet test`; Wave R agents are READ-ONLY and never commit; the design
  zip is unpacked OUTSIDE the repo only; single-writer files honored.

On halt: stop, report verbatim, await the operator.

## 14. Closure protocol and report [CORE]

`METHODOLOGY` closure: (a) ROADMAP write-back; (b) frontmatter+sync folded;
(c) single EVT append; (d) F-rows never chat-only; (e) the closure report (chat):
commits table; versions table (docs + ContractsVersion); census before/after
(section 10, every moved pin with its delta record); gates table baseline vs
closure -- build, full suite (state the exact arithmetic; test count will move
with the rewrites: name every retired/added test), native selftest 110, validate
exit 0, ratchet 0, THE SMOKE EVIDENCE on the final composition, deliberate-break
records (manifest strict-fail x2, edge re-add redden); Wave R inventory summary
(what each front found, discrepancies raised at C-R); consolidated `Skeleton
revisions`; self-attestation (8.4 v3.0.0: branch pushed + PR opened, main
untouched, no self-merge; sync every governance commit; single EVT append; no
rewrites; native/ + historical/ untouched; design zip never entered the tree);
operator checklist (merge the PR; confirm the visual smoke; forward queue: UI
charter / W5 / F-61 -- operator re-ranks).

## 15. Out of scope [CORE]

W5 slice moves (system/component/event FILE relocation; the Core->Systems IVT);
the B-6 analyzer (F-row); `GameServices` genre-noun vocabulary (F-row);
F-42 RNG service (manifest carries seeds; service unbuilt); F-58 reclaim;
window/title fields in the manifest (reserved-field decision recorded);
Weather/Example in the root mod set (optional, menu-loaded); the UI program
(R3 consults its handoff, builds nothing for it); BT/Jobs disposal (BD-5/W5);
`Persistence`/`Crypto.Future` (BD-5/W7); any `native/` change; pushes to `main`
and merging (operator's act).

## 16. v1.1 -- ratified amendments (operator, 2026-08-30, post-Wave-R) [CORE]

Wave R (six read-only agents: R1 manifest/boot-load, R2 dissolution, R3 presentation,
R4 mod-host, R5 re-typing/SDK, R6 graph/deploy) reconciled against section 2 at the C-R
checkpoint and triggered **H4 + H6**. The executor halted with zero edits. The operator
ratified six rulings; this section is the resulting delta. Where 16.x contradicts an
earlier section, **16.x wins** -- the earlier text is the pre-Wave-R intent.

### 16.1 What Wave R REFUTED in section 2

- **2.7 is wrong.** "A mod can register the 10 real systems WITHOUT moving their files"
  fails five independent ways: (a) `IModApi.RegisterSystem<T>()` is parameterless and
  `ModRegistry.CreateSystemInstance` (`:462-479`) is a parameterless `Activator` -- the
  recon's "direct-instance arm" reading is wrong -- so `MovementSystem`'s
  `IPathfindingService` ctor throws `MissingMethodException`, which propagates out of
  `IMod.Initialize` and rolls back the WHOLE batch (all ten fail together);
  (b) `ModRegistry._systemServices` is read ONLY by the CORE overload at `:131`, so a
  mod-provided service reaches nothing -- a WIRING gap, not a timing one;
  (c) the relocating factories name `NativeWorld` and `GameServices` in their signatures,
  which `ISystemContext.cs:11-13` forbids crossing the SDK;
  (d) `IdentityComponent.Name` and `MovementComponent.Path` carry Core.Interop types with
  no Contracts equivalent in scope; (e) `SkillsComponent` needs a map surface the SDK lacks.
- **Fault policy inverts.** `SystemExecutionContext.RouteFault:135-148` keys purely on
  `SystemOrigin`; the ten move from `RethrowCore` (fail-fast) to `ContainedMod`.
- **Section 10's census expectation is retired.** Every `CensusMetaTests` pin measures
  exactly its asserted value and ALL 16 deletion/relocation candidates measure ZERO on
  EVERY family. **No pin moves; no census-delta is owed for D4/D7.** The risk is inverted:
  a `deferred`/`stub`/`TODO`/`not yet`/`Phase 6` word in the prose of any NEW `src/` file
  IS a pin movement. Every writer must be told.
- **Measurement corrections.** Orphan cluster = **344 lines / 11 files**, not ~295;
  `Scene/` = **262 lines / 9 files**, not 214; blast radius = **46 distinct tests**, not 26
  (adds 8 `RandomPawnFactoryTests`, 6 `VanillaComponentRoundTripTests` in a DIFFERENT
  assembly, 2 `Systems.Tests` smoke). D6 additionally stops `Modding.Tests` COMPILING --
  it references only `Application` and reaches the game namespaces transitively.

### 16.2 Ratified rulings

| # | Ruling |
| - | ------ |
| R1 | The SDK grows; the brief rises to v1.1 and gains what the cascade lacked |
| R2 | The fault-policy inversion is **ACCEPTED** -- there is no game yet, so quarantine semantics are tolerable for gameplay systems |
| R3 | Engine first, then the game: **growing the SDK is justified**. Game components conform to Contracts-side types; `ISystemContext.cs:11-13` is PRESERVED, not waived |
| R4 | Mods live in a `mods/` folder; where the systems assembly lives is not important |
| R5 | The `Initialize` re-entry defect is fixed IN this wave |
| R6 | The small defects are fixed IN this wave. Subagents permitted on a Kahn schedule with one integrator; only the orchestrator commits |

### 16.3 New deliverables (D10-D15)

| #   | Deliverable | Action |
| --- | ----------- | ------ |
| D10 | SDK growth | `IModApi.RegisterSystem<T>(Func<ISystemServices,T>)` (closes BOTH the parameterised-ctor and the unreachable-provision blockers); new `Contracts/Services/INavGridService.cs`; `ISystemServices` gains its member; N typed presentation effects mirroring `SetAmbientTint`; `ContractsVersion` **2.1.1 -> 2.2.0 MINOR** |
| D11 | Component re-typing | `IdentityComponent.Name` `InternedString` -> `StringHandle`; `MovementComponent.Path` `NativeComposite<GridVector>` -> `CompositeHandle<GridVector>`; `SkillsComponent` maps -> fixed 13-slot storage. MANAGED-ONLY: `native/` byte-untouched |
| D12 | `Initialize` once-guard | `SystemBase.Initialize` gains the guard; `ParallelSystemScheduler` `:129-135` doc corrected (it claims "exactly once" and is false) |
| D13 | Runtime mods root | anchored at `AppContext.BaseDirectory` (the distribution root), derived from the resolved manifest's own directory; every shippable mod gains a deploy target |
| D14 | Vanilla skeleton defects LEDGERED, not fixed (operator, 2026-08-30: *the mods do not exist yet -- they only promise mechanics and distribution slices*). W5 rewrites them clean and deletes the originals, so repairing them now is work W5 discards. The NEW scenario mod copies the **Weather** template, which is correct, never the skeleton template. The scenario mod does NOT depend on `dualfrontier.vanilla.core` -- that mod vends nothing (zero `.cs`), so the dependency would be decorative and would drag its unloadable assembly name into W4's boot path |
| D15 | `ItemFactory` test floor | authored BEFORE the relocation -- it has ZERO coverage today, so a move would have no falsifiable floor |

### 16.4 Locked design decisions (deviations -> `Skeleton revisions`; forks -> H6)

1. **Composer is `EngineComposer`, never `*Bootstrap`.** `DFK005` (Error) hard-codes
   `CanonicalBootstrap = "GameBootstrap"` and forbids every other `*Bootstrap` outside
   `DualFrontier.Core.Interop*`. К-L5 (LOCKED, К0) is about the declarative graph and
   forbids "managed-side bootstrap FRAGMENTS" -- one composer keeps the invariant. Its
   **Implementation artifacts** line (`KERNEL_ARCHITECTURE.md:157`) names `GameBootstrap.cs`
   and becomes FALSE on dissolution: it receives its PATCH in this cascade per
   `METHODOLOGY.md` 12.7 step 9 (SYNTH-2). DFK005's now-dead exemption -> F-row.
2. **Re-typing is MANAGED-ONLY and proven so.** `component_store.h:14-17` is type-erased
   (tracks only a byte count, all ops `memcpy`); `grep native/` for either component or any
   of the four wrapper types returns ZERO; the `IntPtr` inside `NativeComposite<T>` is a
   managed convenience minted FROM the id by `df_world_get_composite`. `MovementComponent`
   shrinks 40 -> 24 bytes; sizes are derived via `Unsafe.SizeOf<T>()` and pinned nowhere.
3. **IVT: grant `Contracts -> Core.Interop`.** Both handle ctors are `internal` and Contracts
   grants IVT to `Application` only, so `NativeWorld` cannot mint. The grant is ENGINE->ENGINE,
   the class the csproj comment already blesses, and does not move the ratchet. Forge
   discipline survives; public ctors would destroy it.
4. **Re-typing scope fence: exactly TWO fields** -- the ones the relocating factories touch.
   The other four `InternedString`-family fields (`FactionComponent`, `WorkbenchComponent`,
   `StorageComponent` x2) are W5. `Components -> Core.Interop` is a GAME->ENGINE edge, legal
   and outside the ratchet, so the family migration buys no W4 gate progress. Record the
   partial boundary honestly.
5. **`SkillsComponent`: fixed 13-slot storage, NOT a `MapHandle<K,V>` family.** A Contracts
   primitive family whose only W4 consumer is one component, with its second consumer
   (`StorageComponent`) in W5, is scope the wave does not need -- and the repo's own precedent
   is against it (the `IModApi` factory overload was deferred at W1 for "no consumer yet").
   Cost 6 files / ~40 sites and +72 bytes per pawn; gain: the map-SDK requirement disappears
   and two native allocations per pawn die (100 at boot). `SkillsComponent.IsInitialized`
   changes semantics and has tests -- handle deliberately. `MapHandle`/`SetHandle` -> F-row.
6. **`INavGridService`, do NOT widen `IPathfindingService`.** The latter has TWO implementers
   (`AStarPathfinding` + a test double), is public, and is marked engine-only nowhere, so
   `CONTRACTS.md` 4.2's MINOR argument does not apply and widening pushes toward MAJOR --
   which would strand all 8 `apiVersion "^2.0.0"` manifests. A NEW type is non-breaking per 4.
7. **Presentation mirrors the typed-per-effect precedent; `IPresentationSink` stays
   `internal`.** Its doc (`:10-15`) records the mods-visible-record shape as the thing it
   exists to prevent. 7.4's "records move to `Contracts/Presentation/`" is SUPERSEDED --
   7.4 delegated the choice to R3's inventory and R3 measured against it.
   **Gate test owed:** `RenderCommandDispatcher.cs:78-81` THROWS on an unknown command, so a
   record shipped without its dispatch arm is a runtime Launcher crash, not a compile error.
8. **Neutralization values are `int`.** `.Kind` is asserted by nobody;
   `PawnStateReporterSystem.cs:130` already casts `(int)a.Kind - (int)b.Kind`. Nothing measured
   distinguishes `byte` from `int`. The ordinal->name table is OWED to the UI program (its
   honesty rule forbids rendering an unresolvable ordinal); the codebase already answers the
   identical question publisher-side for jobs (`TranslateJob(JobKind) -> string`, delivered as
   `JobLabel : string`). Record as a UI-0 handoff note; build no UI surface here (section 15).
9. **The graph needs no W4 change.** `ModIntegrationPipeline` `:503-528` builds from
   `GetAllSystems()` -- core AND mod -- so a boot graph from an EMPTY core set is superseded by
   `Apply`'s rebuild. Already shipping: `WeatherHarness.cs:60` uses exactly this shape.
   The NATIVE graph does not mirror mod systems and it does not matter: `ExecuteTick` is purely
   managed and the per-tick native entry points have ZERO production call sites. -> F-row.
10. **`Initialize` fix = a once-guard on `SystemBase`** (1 file, ~4 lines). It is the only
    option that also covers `SystemAdapter.cs:72`, the unbounded arm forwarding to arbitrary
    third-party `ISimulationSystem.Initialize`. ZERO tests move, so the executor MUST break it
    once deliberately and observe the failure before restoring. A SECOND consequence is NOT
    fixed by the guard and gets its own F-row: `DomainEventBus` returns before `list.Add` on a
    duplicate, so a surviving subscription keeps its ORIGINAL `CapturedContext` while `Rebuild`
    installs fresh contexts -- after any rebuild those subscriptions dispatch under a stale one.
11. **Runtime mods root = `AppContext.BaseDirectory`.** The measured cwd is the REPO ROOT, whose
    `mods/` holds 9 manifests and ZERO assemblies, while the deploy targets ship to the Launcher
    OUTPUT tree -- so nothing loadable is reachable today and the csproj comment asserting
    `<cwd>/mods/` is an assumption refuted by measurement. Deploying into the SOURCE tree is
    rejected: `.gitignore` has no `*.dll` rule (proven by `git check-ignore`), so every build
    would dirty `git status`, and a published distribution has no source tree.
    **Vocabulary the docs must state:** source-tree `mods/` holds mod PROJECTS; runtime `mods/`
    sits beside the binary.
12. **The 6 vanilla skeletons are NOT repaired here.** Three measured defects keep them
    unloadable -- `Vanilla.Core`'s `AssemblyName` misses the manifest-derived
    `dualfrontier.vanilla.core.dll` by the `.Mod` segment (a mismatch on ANY filesystem, not a
    case-sensitivity issue); none of the six sets `CopyLocalLockFileAssemblies=false` or
    `<Private>false</Private>`, so `DualFrontier.Contracts.dll` sits in every one of their bins
    and would give the contract types TWO identities inside a collectible ALC; and
    `Vanilla.Core` has zero `.cs` files, so it vends nothing. They are harmless while nothing
    loads them: the discoverer merely lists them, and `Apply` only ever receives the root set.
    All three -> F-rows. The two-line csproj fix is ~12 lines total if a later wave wants it
    before W5's rewrite.

### 16.5 Revised commit plan (supersedes section 11)

| #  | Subject |
| -- | ------- |
| C1 | `governance(enroll): W4_COMPOSITION_ROOT brief v1.1 + recon enrolled` |
| C2 | `fix(sdk): SystemBase.Initialize is once-only; the scheduler doc stops claiming what it did not do` |
| C3 | `test(scenario): ItemFactory gains the behavioural floor it never had` |
| C4 | `feat(sdk): factory registration for mods, INavGridService, presentation effects -- ContractsVersion 2.2.0` |
| C5 | `refactor(components)!: identity, movement and skills re-type onto Contracts-side storage` |
| C6 | `feat(distribution): game.manifest.json ships with a strict typed loader` |
| C7 | `feat(composition): EngineComposer; the boot-time mod-load path exists` |
| C8 | `feat(vanilla): the scenario mod owns registration, factories, seeds, pathfinding, systems` |
| C9 | `feat(presentation): neutral command records; the bridge translation goes mod-side` |
| C10 | `refactor(boundary)!: the four engine->game edges are CUT; ratchet baseline 4 -> 0` |
| C11 | `chore(orphans): the extended BD-8 cluster dies` |
| C12 | `docs(architecture): W4 truth -- К-L5 artifact, plan/EAM/MOD_OS/boundary/CONTRACTS write-backs` |
| C13 | `governance(closure): W4_COMPOSITION_ROOT EVT + ROADMAP write-back -- W4 DONE` |

C2, C3 and C6 are independent and may be produced in parallel; C4 and C5 need only disjoint
file sets; C7 before C8 stays load-bearing. Only the orchestrator runs `git add/commit`.

### 16.6 Additional halt triggers

- **H10** `native/` would need a change for the re-typing (contradicts 16.4-2 -- STOP).
- **H11** RETIRED at v1.1. It asked whether Phase F accepts a shared mod exporting zero
  types; with the scenario mod no longer depending on `dualfrontier.vanilla.core`, that mod is
  never loaded and the question gates nothing. It remains UNMEASURED and rides an F-row.
- **H12** the once-guard's deliberate break does NOT redden -- a vacuous gate.

### 16.7 F-rows owed at closure (new, beyond section 7.7)

Native graph does not mirror the managed set after `Apply`; stale `SystemExecutionContext`
after `Rebuild`; `MapHandle<K,V>`/`SetHandle<T>` deferred to their second consumer;
DFK005's dead `GameBootstrap` exemption; `SystemBase.Context` never assigned (dead property);
the false `Application.csproj:21-25` IVT comment about `Core.Benchmarks` re-using the
factories (measured: zero uses); `Components -> Core.Interop` survives W4 (partial boundary);
the three vanilla-skeleton load defects (16.4-12); whether Phase F accepts a zero-export
shared mod (unmeasured).

---

**End of W4_COMPOSITION_ROOT_BRIEF.md v1.1**
