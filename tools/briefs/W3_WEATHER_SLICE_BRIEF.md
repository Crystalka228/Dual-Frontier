---
register_id: DOC-D-W3_WEATHER_SLICE_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft
owner: Crystalka
version: '1.0'
first_authored: '2026-07-19'
last_modified: '2026-08-20'
content_language: en
next_review_due: null
title: W3 Weather vertical slice -- first real mechanic as a mod pair (shared contracts + regular), SDK gaps G1/G2 closed, RegisterOwner live wiring, src stub deletion
authored_by: Claude Opus (architect session, W3 chartering)
basis: Architect grounding line-reads 2026-07-19 at main 8c2ec2e (ratified direct-brief deviation -- no separate recon session; W1/W2 recon corpus + live reads substitute)
---

# W3_WEATHER_SLICE -- Execution Brief

This cascade delivers Wave 3 of the vanilla-separation program (VANILLA_SEPARATION_MIGRATION_PLAN
section 4, W3): ONE small mechanic -- Weather -- implemented end-to-end as a REAL mod pair, WRITTEN
FRESH per the scaffolding ruling (plan section 1.1). The wave's purpose is to surface and close the
missing SDK surface BEFORE mass migration; the architect grounding has already measured the gaps
(G1 entity lifecycle, G2 presentation, G4 Phase C shared-provider satisfiability) and this brief
closes G1/G2/G4 and wires the W2 RegisterOwner mechanism live. Done means: the mod pair loads
through the ordinary pipeline, weather transitions tick deterministically, a mod-authored event
round-trips cross-owner under the strict capability gate, the scene visibly reacts, unload removes
the mechanic entirely with the engine healthy, reload restores it, and the two src/ Weather stubs
are deleted with the build green.

Executor: Claude Code (flagship model), LOCAL on the operator's machine, repository
`D:\Colony_Simulator\Colony_Simulator` (GitHub `Crystalka228/Dual-Frontier`), branch off `main`.

Brief-integration notice: this brief CITES standing law by anchor and does not restate it --
commit-body structure and marker law per CODING_STANDARDS; census pin law per TESTING_STRATEGY;
mutability license and `Skeleton revisions` form per RESERVED_SURFACE_MUTABILITY; session closure
per METHODOLOGY. Anti-pattern rule: a conflict between this brief and any standing document, or
between this brief and the live code, means THE BRIEF IS WRONG -- halt and escalate; code-truth wins.

## 1. Mission [CORE]

Deliverables:

| #  | Artifact | Action | Version |
|----|----------|--------|---------|
| D1 | `DualFrontier.Contracts` SDK surface | ADD `ISystemContext.CreateEntity()`, `DestroyEntity(EntityId)`, `IsEntityAlive(EntityId)`, `SetAmbientTint(float r, float g, float b, float strength)`; `ContractsVersion.Current` 2.0.0 -> 2.1.0 (MINOR, additive) | Contracts 2.1.0 |
| D2 | Presentation primitive | NEW engine-generic `AmbientTintCommand` (IRenderCommand) + Launcher dispatcher handling + ModRegistry presentation sink (loud-throw when unset) | -- |
| D3 | Owner ledger live wiring | `KernelCapabilityRegistry.RemoveOwner(owner)` NEW surface; `RegisterOwner` wired at pipeline pass [1] (shared) and pass [2] (regular); removal symmetry on rollback + unload chain; Phase C shared/ledger satisfiability extension | -- |
| D4 | Weather mod pair | NEW `mods/DualFrontier.Mod.Weather.Contracts` (kind=shared: `WeatherKind`, `WeatherChangedEvent`) + NEW `mods/DualFrontier.Mod.Weather` (kind=regular: `WeatherStateComponent`, `WeatherSystem`, `WeatherPresentationSystem`), sln-enrolled, fixture-deployed, capabilities DECLARED (first strict-gate manifests) | mods 0.1.0 |
| D5 | Wave-gate + F-series proof tests | end-to-end pipeline tests: load -> tick -> cross-owner event -> tint -> negative capability test -> re-entrant publish (F-56) -> unload -> reload | -- |
| D6 | src stub deletion | DELETE `src/DualFrontier.Systems/World/WeatherSystem.cs` + `src/DualFrontier.Events/World/WeatherChangedEvent.cs` (zero consumers; build-green-as-inertness-proof) | -- |
| D7 | Doc amendments | CONTRACTS.md, ECS.md, MOD_OS_ARCHITECTURE.md, MODDING.md (MINOR each); VANILLA_SEPARATION_MIGRATION_PLAN W3 DONE row | per section 12 |
| D8 | Closure | EVT append, ROADMAP write-back (F-55 CLOSED, F-56 closed-if-green, NEW F-row for G3), brief -> EXECUTED | -- |

W3 precedes W4 (composition root): the SDK gaps it closes are prerequisites for dissolving
GameBootstrap, and the mod-pair pattern it proves is the W5 slice-replacement rehearsal.

## 2. Established facts [CORE]

All facts measured by architect line-reads at `main = 8c2ec2e` (post-W2 + Codex fix `722bbca`).
Facts marked (RV) must be re-verified by the executor at Phase 0; mismatch -> HALT H1.

1. (RV) `main` HEAD = `8c2ec2e`, tree clean. Managed baseline gates: full-sln
   **1220 passed / 0 failed / 5 skipped** (the 5 skips are the known F-10 stress/extreme family);
   `validate --armed` exit 0; ratchet census 4+1 engine->game edges; DFK-WAIVER count 2;
   `Console.WriteLine` in `src/` count 2 (both in `RestrictedModApi.cs`: the grace warning and
   `Log`). Native tree untouched by this cascade -- selftest count not re-run unless native gates
   are part of the operator's standard closure verify.
2. (RV) `src/DualFrontier.Systems/World/WeatherSystem.cs` is a `[BridgeImplementation(Phase = 7)]`
   stub with an empty `Update`; it is NOT among the 10 systems registered in
   `GameBootstrap.CreateSession`. `src/DualFrontier.Events/World/WeatherChangedEvent.cs` is an
   empty record with TODO fields. Filename census found no other `*Weather*` file in `src/` or
   `tests/`. Content references beyond these two files are UNKNOWN -- Phase 0 runs the content
   grep (section 3.7); hits outside the two files and their own doc comments -> HALT H7.
3. `ISystemContext` (Contracts/Sdk) has NO entity-lifecycle members and NO presentation surface;
   `WriteScope<T>` has `Add/Update/Remove` (attach possible given an id, no way to mint one).
   `NativeWorld` already exposes `CreateEntity()`, `DestroyEntity(EntityId)`, `IsAlive(EntityId)`
   over `df_world_create_entity` / `df_world_destroy_entity` / `df_world_is_alive` -- D1 is a pure
   Contracts promotion, ZERO native changes.
4. `SystemContextView` (Application/Modding) implements `ISystemContext`, reaching the world via
   `SystemExecutionContext.Current.NativeWorld`; events route through
   `_registry.GetModApi(_modId)` (the live capability gate). New members implement here.
5. Shared-mod loading is FULLY LIVE in `ModIntegrationPipeline.Apply`: manifest `kind` classifies
   (`ModKind.Shared`), D-5 topo sort at [0.5], `LoadSharedMod` into the singleton
   `SharedModLoadContext` at pass [1] (never unloaded, MOD_OS section 5.1), regular ALCs delegate to
   it at pass [2]. `ContractValidator` Phase E rejects regular mods exporting `IEvent` /
   `IModContract` -- the weather event MUST live in the shared mod (structure forced by law).
   Phase F enforces shared-mod purity (empty entryAssembly/entryType/replaces, no IMod).
6. `KernelCapabilityRegistry` (Core/Modding): `RegisterOwner(ownerNamespace, assembly)` scans
   public concrete `IEvent` (tier-token emission incl. Normal legacy aliases), `[ModAccessible]`
   `IComponent`, `[Layer]` types; records ownership for `Owns`; `OwnerOf(fqn)` resolves; there is
   NO removal surface. The pipeline holds the ledger as `_kernelCapabilities` and passes it to
   validation and to each `RestrictedModApi`.
7. `RestrictedModApi.EnforceCapability`: (a) `Owns("mod."+modId, fqn)` self-access auto-grant;
   (b) token = `{OwnerOf(fqn) ?? "kernel"}.{verb}:{FQN}`; (c) empty manifest capabilities -> grace
   path with `Console.WriteLine` warning; (d) declared-set check -> `CapabilityViolationException`.
   A manifest that DECLARES capabilities exits grace and is enforced strictly.
8. (G4, measured defect-in-waiting) `ContractValidator.ValidateCapabilitySatisfiability` (Phase C)
   satisfies a non-kernel token ONLY via a dependency found in the REGULAR `mods` list whose
   MANIFEST declares the token in `capabilities.provided` -- a shared-mod provider is invisible
   (`FindMod` never sees `sharedMods`). Unextended, Phase C would reject the weather mod.
9. Capability grammar (`ManifestCapabilities.s_capabilityPattern`) accepts
   `mod.[a-z0-9.]+` owner ids -- `mod.dualfrontier.weather.contracts.publish:<FQN>` parses
   (greedy-backtrack resolves owner vs verb). (RV) One `ManifestCapabilities.Parse` smoke against
   the exact production tokens at Phase 0; rejection -> HALT H9.
10. Manifest JSON shape (live exemplar `mods/DualFrontier.Mod.Vanilla.World/mod.manifest.json`):
    `manifestVersion: "3"`, `kind: "regular"|"shared"`, `apiVersion: "^2.0.0"`, `dependencies`
    `[{id, version}]`, `capabilities: {required: [], provided: []}`. `ManifestParser` rejects
    non-"3".
11. Presentation path: `IRenderCommand` (marker) -> `PresentationBridge.Enqueue` (any thread) ->
    Launcher drains via `RenderCommandDispatcher` on the render main thread. Six game command
    records exist (W6 dissolves them); the tint command is engine-generic and joins the marker set.
12. `mods/Directory.Build.targets`: `<IsVanillaMod>` gates manifest deploy + Release hotReload
    rewrite; Mod.Example pattern: `Private=false` ProjectReference to Contracts ONLY,
    `CopyLocalLockFileAssemblies=false`, manifest CopyToOutputDirectory, `DeployToTestFixtures`
    target into `tests/DualFrontier.Modding.Tests/bin/.../Fixtures/<AssemblyName>/`.
13. `ModIntegrationPipeline` rollback paths: validation failure -> `RollbackLoaded(loaded)`
    (regular only; shared persists by design); Initialize failure -> registry reset + contract
    revoke + rollback. `UnloadMod` runs the section 9.5 chain steps 1/2/3/3.5/3.6/4+5/6/7.
14. `NativeWorld` also exposes `BeginModScope/EndModScope/ClearModScope` (context for the G3
    F-row -- NOT W3 scope; see section 14 F-ledger text).
15. B-2 freeze (no new gameplay nouns in `src/`) is ACTIVE -- every W3 gameplay noun lives under
    `mods/`; the only `src/` changes are SDK/engine-generic (D1-D3) and the D6 deletion.

## 3. Phase 0 -- preconditions and checkpoint [CORE]

Run serially before any code change.

1. **Verify the (RV) set** from section 2 by direct reads. Mismatch -> HALT H1.
2. **Baseline gates** (regression anchor): full managed build + full test run per
   DEVELOPMENT_HYGIENE. Expect 1220/0/5 (5 skips = F-10 family); record actuals. Closure must
   match-or-improve modulo the new tests this cascade adds -> HALT H2 on regression.
3. **Branch prep**: create work branch from `main` (`claude/w3-weather-slice` or session-assigned
   name). No pushes ever (operator act).
4. **Validation checkpoint**: `dotnet run --project tools/DualFrontier.Governance -- validate
   --armed` exit 0 -> else HALT H3. Governance test suite green.
5. **Frontmatter-shape read** (Lesson #N14): FRAMEWORK.md 14.3/14.4/14.5 + live frontmatter of one
   LOCKED tier-2 doc + one existing `AUDIT_TRAIL.yaml` EVT entry as the verbatim append template.
   Armed G-CATLIFE: this brief persists `lifecycle: Draft` and flips to `EXECUTED` at closure --
   never LOCKED in frontmatter.
6. **Mandatory reads** before any edit: this brief in full; `ISystemContext.cs`, `WriteScope.cs`,
   `SystemContextView.cs`, `SystemAdapter.cs`, `ModRegistry.cs`, `RestrictedModApi.cs`,
   `ContractValidator.cs` (Phases C/E/F), `ModIntegrationPipeline.cs` (Apply + UnloadMod),
   `KernelCapabilityRegistry.cs`, `ManifestParser.cs` + `ManifestCapabilities.cs`,
   `ModLoader.cs` (LoadSharedMod/LoadRegularMod), `mods/DualFrontier.Mod.Example/*`,
   `mods/Directory.Build.targets`, `PresentationBridge.cs` + `IRenderCommand.cs` + the Launcher
   `RenderCommandDispatcher` and renderer entry (locate under the Launcher project; read before
   designing the tint application), `GameBootstrap.cs`, `ContractsVersion` type,
   MOD_OS_ARCHITECTURE sections 3.2-3.6 / 5 / 9.5, CODING_STANDARDS commit-body law,
   METHODOLOGY closure protocol.
7. **Weather content grep** (D6 precondition):
   `rg -n "Weather" src/ tests/ mods/ --glob '!**/bin/**' --glob '!**/obj/**'`
   Expected hits: the two D6 files themselves; their own doc comments; NOTHING else in compiled
   code. Doc-comment mentions of WeatherSystem inside OTHER files (e.g. a subscriber list in a
   comment) are mutable surface -- clean them in D6's commit. Any COMPILED reference outside the
   two files -> HALT H7.
8. **Derived-fold protocol**: `sync` runs in EVERY frontmatter-touching commit; derived registers
   never hand-edited; `AUDIT_TRAIL.yaml` append-only.

## 4. Topology [CORE]

Single orchestrator, serial execution, no wave agents. Rationale: the cascade is deep but narrow --
every commit builds on the previous surface (SDK -> wiring -> mods -> tests -> deletion -> docs),
so parallel writers would serialize anyway. Only the orchestrator runs `git add/commit`.

## 5. Wave R -- survey agents

None (serial cascade; Phase 0 mandatory reads are the survey).

## 6. Checkpoints [CORE]

Serial self-audit at C-W equivalent (before C8 docs commit): re-run the section 3.7 grep (zero
Weather residue in src/); confirm every enforcement claim written into docs names its on-disk
enforcer (truth law); confirm no roadmap load landed outside `docs/ROADMAP.md`; citation-form
compliance (anchors, no version pins, no URL anchors).

## 7. Execution / writer specifications [CORE]

### 7.1 D1 -- SDK entity lifecycle + presentation members (commit C2, C3)

`ISystemContext` gains, with full XML docs in the established Contracts voice:

- `EntityId CreateEntity();` -- mints a live entity in the simulation world. Engine-generic.
- `void DestroyEntity(EntityId id);` -- deferred-destroy semantics identical to the engine path
  (document that destruction follows the native deferred-destroy + flush model; do NOT add a flush
  member to the SDK -- flushing stays an engine concern).
- `bool IsEntityAlive(EntityId id);` -- liveness probe (promotes `NativeWorld.IsAlive`).
- `void SetAmbientTint(float r, float g, float b, float strength);` -- engine-generic whole-scene
  color modulation, `strength` in [0,1], 0 = no tint. Document: this is the W3 minimal
  presentation primitive; the BD-9 wave (W6) delivers the full layer/slot model and ABSORBS this
  member (absorption trigger recorded in the doc comment AND in ROADMAP -- roadmap load lives in
  ROADMAP.md, the doc comment carries only the `Planned -- see ROADMAP.md` pointer form).

`SystemContextView` implements: the three entity members delegate to `World.CreateEntity` /
`DestroyEntity` / `IsAlive`; `SetAmbientTint` routes to the presentation sink resolved from
`ModRegistry` (7.2). Freshness law unchanged: values are valid this tick; an `EntityId` MAY be
persisted by the mod across ticks (it is world identity, not an engine reference -- state this
distinction explicitly in the XML docs; the ECS section 8 anti-pattern binds engine OBJECT
references, not ids).

`ContractsVersion.Current` 2.0.0 -> 2.1.0 in C2 (first additive member lands there); C3 adds
`SetAmbientTint` under the same 2.1.0 (one version per wave). Existing manifests pinning `^2.0.0`
remain satisfied -- assert with one test.

Unit tests (C2): create -> alive -> attach component via batch `Add` -> read back; destroy ->
not-alive after engine flush (drive the flush through the engine path the test controls); the
out-of-context loud failure (`ISystemContext` world access outside scheduler context still throws).

### 7.2 D2 -- presentation primitive plumbing (commit C3)

- NEW `AmbientTintCommand(float R, float G, float B, float Strength) : IRenderCommand` in
  `Application/Bridge/Commands` -- an engine-generic record; XML docs state it carries NO game
  semantics (a color, not "weather").
- `ModRegistry` gains a presentation sink slot: `SetPresentationSink(IPresentationSink sink)` with
  `internal interface IPresentationSink { void SetAmbientTint(float r, float g, float b, float strength); }`
  (Application-internal). UNSET sink + a mod calling `SetAmbientTint` -> throw
  `InvalidOperationException` LOUDLY (fail-open doctrine: a silent no-op presentation call is the
  forbidden shape). Tests that exercise tint install a recording test sink.
- `GameBootstrap.CreateSession` wires the sink to `bridge.Enqueue(new AmbientTintCommand(...))`.
- Launcher: `RenderCommandDispatcher` handles `AmbientTintCommand` by storing the current tint;
  the renderer applies a whole-scene modulation. Implementation freedom WITHIN this observable
  criterion: with strength > 0 the entire rendered scene is visibly color-modulated; strength 0
  restores the untinted scene. Choose the smallest mechanism the existing renderer architecture
  offers (full-screen overlay quad or global color modulation). If the renderer architecture
  cannot express it within a small, clean change (guideline: roughly <= 200 renderer-side LOC and
  no pipeline-object redesign) -> HALT H8 with a written option set; do not force.
- Dispatcher unit test: command dispatch updates the stored tint; strength 0 resets.

### 7.3 D3 -- owner ledger live wiring (commit C4)

`KernelCapabilityRegistry`:

- NEW `public void RemoveOwner(string ownerNamespace)` -- removes every capability token and every
  ownership record registered under the namespace; idempotent (removing an unknown owner is a
  no-op). Implementation note: removal must subtract exactly the tokens that owner's registration
  added (track per-owner token sets; do not re-derive by prefix string-matching -- `kernel.` vs
  `mod.` prefixes make prefix-removal a latent cross-owner deletion bug).
- `RegisterOwner` stays as-is (idempotency already documented -- verify it holds per-owner when
  the same assembly re-registers on reload; add a test).

`ModIntegrationPipeline.Apply`:

- Pass [1]: immediately after each successful `LoadSharedMod`, call
  `_kernelCapabilities.RegisterOwner("mod." + manifest.Id, <loaded shared assembly>)`. Shared
  ownership PERSISTS for the session (mirrors the non-collectible shared ALC; MOD_OS section 5.1) --
  including across a failed batch. State this in the code comment.
- Pass [2]: immediately after each successful `LoadRegularMod`, register the regular mod's own
  assemblies under `"mod." + manifest.Id`.
- Every failure path that rolls back regular mods (validation failure, Initialize failure, graph
  build failure) calls `RemoveOwner("mod." + modId)` for each rolled-back REGULAR mod (symmetry
  with `RollbackLoaded`); shared registrations are never rolled back.
- `UnloadMod` chain: add a `TryUnloadStep` invoking `RemoveOwner("mod." + modId)` adjacent to the
  step-2/step-3 revocations (best-effort per section 9.5.1). The MOD_OS section 9.5 amendment in D7
  records the new step.

`ContractValidator` Phase C extension (G4):

- Keep the existing kernel fast path (`ProvidesKernel`) and the existing regular-provider manifest
  path UNCHANGED (additive OR).
- NEW satisfiability arm: a token of the owner-namespaced form is satisfied when BOTH hold:
  (a) the owner id parsed from the token (`mod.<ownerId>.` prefix; parse against the registered
  owner set, not by naive dot-splitting -- owner ids contain dots) matches the `id` of a mod listed
  in the requiring mod's `dependencies`; AND (b) `kernelCapabilities.Provides(token)` (the ledger
  actually registered it, i.e. the provider assembly truly exports the type). Manifest `provided`
  declarations are NOT required for owner-scanned types -- the ledger (assembly scan) is the single
  source of truth. `Validate` gains the shared manifests it needs for (a) via the already-supplied
  `sharedMods` list (their `Manifest.Id` values).
- Tests: satisfied (declared dep + ledger-registered) passes; missing dependency declaration
  fails with `MissingCapability` even when the ledger has the token (implicit satisfaction stays
  rejected per MOD_OS section 3.4); ledger-absent token fails.

Grace path, `Owns` auto-grant, and the two `Console.WriteLine` sites are UNTOUCHED (census pin).

### 7.4 D4 -- the Weather mod pair (commit C5)

**`mods/DualFrontier.Mod.Weather.Contracts`** (shared vendor):

- `WeatherKind` enum: `Clear, Rain, Storm, Fog, Snow, EtherStorm` (mod-owned vocabulary).
- `WeatherChangedEvent : IEvent` -- `sealed record` with `required WeatherKind Kind`,
  `required WeatherKind PreviousKind`, `required float Intensity` ([0,1]). No `[EventTier]`
  (Normal default -- legacy alias tokens emit).
- csproj mirrors Mod.Example (Contracts `Private=false` reference, no copy-local, manifest deploy,
  `DeployToTestFixtures`); `mod.manifest.json`: `kind: "shared"`, EMPTY `entryAssembly` /
  `entryType` / `replaces` (Phase F), `apiVersion: "^2.0.0"`, empty capabilities (provided stays
  `[]` -- the ledger is the truth source), no dependencies.

**`mods/DualFrontier.Mod.Weather`** (regular):

- `WeatherStateComponent : IComponent` (unmanaged struct): `int Kind` (cast of `WeatherKind`),
  `float Intensity`, `long LastTransitionTick`. Lives in the REGULAR mod (Phase E binds
  events/contracts only). No `[ModAccessible]` (no cross-mod component access this wave).
- `WeatherMod : IMod` -- `Initialize(IModApi api)`: `RegisterComponent<WeatherStateComponent>()`,
  `RegisterSystem<WeatherSystem>()`, `RegisterSystem<WeatherPresentationSystem>()`. `Unload()` empty
  (engine chain releases subscriptions).
- `WeatherSystem : ISimulationSystem`, `[SystemAccess(reads: [], writes: [WeatherStateComponent])]`,
  `[TickRate(TickRates.NORMAL)]`:
  - Ensure-singleton (idempotent, adopt-existing): acquire span over `WeatherStateComponent`;
    if empty -> dispose the span FIRST (native rejects mutation under a live span), then
    `context.CreateEntity()` + batch `Add` of the initial state (`Clear`, intensity 0,
    `LastTransitionTick = CurrentTick`) -- this is the wave's "initial data" element; if
    non-empty -> adopt the existing entity (reload path).
  - Transition law (DETERMINISTIC -- no wall clock, no time-seeded RNG): every
    `TransitionPeriodTicks` (`public const long TransitionPeriodTicks = 300`) compute the next
    state as a pure function of `(CurrentTick, current Kind)` via a fixed xorshift/hash over a
    compile-time seed constant; intensity derives from the same hash. Same tick history => same
    weather history, always.
  - On transition: batch-update the component, then `context.Publish(new WeatherChangedEvent {...})`
    -- a CROSS-OWNER publish through the strict gate (the manifest declares it).
- `WeatherPresentationSystem : ISimulationSystem`, `[SystemAccess(reads: [], writes: [])]`,
  `[TickRate(TickRates.NORMAL)]`: `Initialize(ISystemContext ctx)` subscribes to
  `WeatherChangedEvent` (cross-owner subscribe, declared); the handler calls
  `ctx.SetAmbientTint(...)` from a per-kind tint table (mod data: e.g. Clear -> strength 0,
  Rain -> desaturated blue, Storm -> dark slate, Fog -> pale grey, Snow -> cold white,
  EtherStorm -> violet; exact values executor-chosen, table `internal static readonly`).
  NOTE the freshness law: the handler runs within the scheduler context (RestrictedModApi wraps
  subscribers in the captured `SystemExecutionContext`) -- confirm the wrapped-context path makes
  `SetAmbientTint` context-safe from the handler; if the sink route does not require world access
  this is trivially safe; state which in the code comment.
- `mod.manifest.json`: `kind: "regular"`, `apiVersion: "^2.0.0"`, `entryAssembly` / `entryType`
  set, `hotReload: true`, `dependencies: [{ "id": "dualfrontier.weather.contracts", "version": "^0.1.0" }]`,
  `capabilities.required` = exactly:
  `mod.dualfrontier.weather.contracts.publish:DualFrontier.Mod.Weather.Contracts.WeatherChangedEvent`
  and the matching `.subscribe:` token. FIRST on-disk manifests exiting the grace path -- the
  strict gate is live for them.
- Both projects enrolled in `DualFrontier.sln` (mirror the W0 vanilla-mod enrollment form).
- `Mod.Example/ExampleSystem.cs` doc-comment W1 events note updated: the shared-mod event story is
  now LIVE (point at the Weather pair as the reference). Mutable surface; rides C5.

### 7.5 D5 -- wave-gate and F-series tests (commit C6)

In `DualFrontier.Modding.Tests` (fixtures auto-deployed by C5's targets), through the REAL
`ModIntegrationPipeline.Apply` with the production validator + ledger:

1. **Load + tick**: Apply [contracts, weather] -> success; tick the scheduler past
   `TransitionPeriodTicks`; assert the singleton exists, transitions occur, and
   `WeatherChangedEvent` reaches a test subscriber (subscribe via a test-side api WITH the declared
   token, or capture through the weather mod's own presentation path via the recording sink).
2. **Determinism**: two fresh sessions ticked identically produce identical (Kind, Intensity,
   tick) transition sequences.
3. **Tint reaction**: recording `IPresentationSink` observes the per-kind tint after a transition;
   strength 0 for Clear.
4. **Strict gate negative**: a fixture regular mod with NON-empty capabilities that omits the
   publish token attempts `Publish<WeatherChangedEvent>` -> `CapabilityViolationException`; a
   fixture mod REQUIRING the token without listing the dependency -> Phase C `MissingCapability`.
5. **F-55 end-to-end**: assertion set 1 IS the proof (mod-authored event, shared-vended,
   pipeline-loaded, cross-owner-gated, round-tripped) -- name the test with an F-55 reference in
   its doc comment.
6. **F-56 re-entrant publish**: a subscriber handler publishes a second `WeatherChangedEvent`
   (depth-guarded to 1) from within delivery, through the mod path; assert both deliveries complete,
   ordering is coherent, no corruption/deadlock. Green => F-56 CLOSED at closure; red => record the
   defect as the F-56 resolution material and HALT H2-adjacent (surface, do not fix beyond scope
   without the operator).
7. **Unload gate**: `UnloadMod("dualfrontier.weather")` -> scheduler no longer ticks weather
   systems; no further events; subscriptions released; `RemoveOwner` ran (weather's regular-owner
   entry gone -- assert via a ledger probe test seam if needed); ALC WeakReference collected
   (established M7.3 pattern); engine stays healthy (subsequent ticks clean). Residual
   `WeatherStateComponent` data on the singleton entity is EXPECTED (G3 -- the F-row, not a W3
   defect); assert the residue explicitly so the gap is pinned by a test, with the F-row cited.
8. **Reload**: Apply again -> shared mod re-registration idempotent; weather adopts the existing
   singleton; transitions resume deterministically from the current tick.
9. **Ownership lifecycle**: shared owner tokens present after load; present after weather unload
   (shared persists); regular owner removed on unload and on validation-failure rollback.

### 7.6 D6 -- src stub deletion (commit C7; eradication mini-kind)

Delete `src/DualFrontier.Systems/World/WeatherSystem.cs` and
`src/DualFrontier.Events/World/WeatherChangedEvent.cs`; clean doc-comment mentions of them in other
files found by the Phase 0 grep (mutable surface). Build-green-as-inertness-proof: full sln build +
test run green after deletion proves inertness; a regression -> the stubs were NOT inert -> HALT H7
(do not force). Symmetric preservation: NOTHING under `historical/` or `docs/reports/` is touched;
the KEEP set is everything except the two named files and doc-comment mentions.

## 8. Kind-specific machinery [KIND: eradication/hygiene -- D6 only]

Covered in 7.6 (symmetric preservation halt + inertness gate). No other kind machinery.

## 9. S-LOCK invariants [CORE]

- **S-LOCK candidate (record in ROADMAP, do not implement an analyzer rule this wave):
  ownership-symmetry** -- every pipeline path that calls `RegisterOwner` for a REGULAR mod has a
  matching `RemoveOwner` on every exit (rollback, unload). Enforced this wave by the 7.5.9 tests;
  analyzer-rule candidate noted next to the K-L20 LOCK family.
- Existing locks preserved untouched: S-LOCK-4 tier tokens (Normal aliases emitted for the weather
  event), the section 9.5 unload chain order, K-L19 fail-fast (the unset-sink loud throw follows it).

## 10. Census discipline [CORE]

- HARD `Console.WriteLine` in `src/`: `rg -n "Console\.WriteLine" src/ --glob '!**/bin/**' --glob '!**/obj/**'`
  = **2 before, 2 after** (both `RestrictedModApi.cs`). New code (incl. Launcher dispatcher) adds
  none; mods use `IModApi.Log` or none.
- HARD `DFK-WAIVER`: count 2 -> 2 (untouched).
- HARD ratchet census: 4+1 engine->game edges UNCHANGED (D6 deletes files, not ProjectReference
  edges; `BoundaryRatchetTests` stays green as-is).
- SOFT deferred-marker counts (89/55): untouched; any doc-comment cleanup that brushes one is a
  census-delta record, not a finding.
- Plan section 2 stock figures move: Events 53 -> 52, Systems 30 -> 29 -- recorded in the D7 plan
  amendment as measured deltas.

## 11. Commit plan [CORE]

| #  | Subject | Content |
|----|---------|---------|
| C1 | `governance(w3): enroll W3_WEATHER_SLICE brief` | brief file under `docs/briefs/` (repo-conventional location), frontmatter D/3/Draft + sync + validate --armed |
| C2 | `feat(sdk): ISystemContext entity lifecycle (CreateEntity/DestroyEntity/IsEntityAlive); Contracts 2.1.0` | Contracts members + XML docs + SystemContextView impl + ContractsVersion bump + unit tests + `^2.0.0`-satisfaction test |
| C3 | `feat(presentation): SetAmbientTint SDK primitive + AmbientTintCommand + Launcher dispatch` | Contracts member + sink interface/slot in ModRegistry (loud-throw unset) + GameBootstrap wiring + dispatcher/renderer application + tests |
| C4 | `feat(modding): owner ledger live -- RegisterOwner wired both passes, RemoveOwner symmetry, Phase C shared/ledger satisfiability` | ledger removal surface + pipeline wiring + rollback/unload symmetry + Phase C extension + tests |
| C5 | `feat(mods): Weather mod pair -- shared contracts vendor + regular mechanic, first strict-gate manifests` | two projects + manifests + sln enrollment + fixture deploy + Mod.Example note update |
| C6 | `test(modding): W3 wave gate -- load/tick/event/tint/negative/re-entrant/unload/reload` | section 7.5 suite (F-55, F-56 proofs named) |
| C7 | `chore(scaffolding): delete src Weather stubs (WeatherSystem, WeatherChangedEvent)` | D6 deletion + doc-comment cleanup; build-green proof |
| C8 | `docs(w3): CONTRACTS/ECS/MOD_OS/MODDING amendments + migration-plan W3 row` | section 12 doc set, frontmatter MINOR bumps + sync + validate --armed |
| C9 | `governance(closure): W3_WEATHER_SLICE EVT + ROADMAP write-back` | AUDIT_TRAIL append + F-rows (14) + brief -> EXECUTED + sync + validate --armed |

Commit count is intended-form; a defect-iteration split is recorded in the closure report, never
compressed.

## 12. REGISTER cascade [CORE]

Schema-2.0 discipline; Phase 0 verbatim shapes only; `PENDING-*` outlawed; real hashes or omit.

- C1: this brief enrolled (D/3/Draft).
- C8 amendments (each = frontmatter edit + body change + sync + validate in ONE commit):
  - `CONTRACTS.md` MINOR: SDK 2.1.0 members (entity lifecycle + ambient tint), the absorption
    pointer (`Planned -- see ROADMAP.md` form), ContractsVersion history row.
  - `ECS.md` MINOR: SDK entity-lifecycle semantics (mint/destroy/liveness; deferred-destroy note;
    EntityId-persistable vs engine-reference distinction in section 8).
  - `MOD_OS_ARCHITECTURE.md` MINOR: sections 3.5-3.6 owner-aware wording is now LIVE (wired
    producer); Phase C shared/ledger satisfiability rule; section 9.5 owner-removal step; the
    shared-mod event vending pattern gains the Weather pair as the live reference (the W2 deferral
    of this doc update lands here).
  - `MODDING.md` MINOR: author guide -- shared-vendor pattern, declaring cross-owner capabilities,
    the strict gate vs grace, the tint primitive.
  - `VANILLA_SEPARATION_MIGRATION_PLAN.md` MINOR: W3 DONE row with commit hashes; section 2 stock
    deltas (Events 52, Systems 29); G1/G2/G4 recorded as the wave's surfaced-and-closed gaps, G3 as
    the surfaced-and-ledgered gap.
- C9 closure: single `AUDIT_TRAIL.yaml` EVT append (real hashes of C1..C8); brief frontmatter
  Draft -> EXECUTED; ROADMAP write-back per section 14. `validate --armed` exit 0 at C1, C8, C9
  (and any other frontmatter-touching commit).

Version bumps computed against the LIVE frontmatter versions read at Phase 0 -- never assumed from
this brief.

## 13. Halt conditions (H-series) [CORE]

- **H1** precondition/(RV) mismatch (section 3.1).
- **H2** build/test regression vs the Phase 0 baseline (modulo added tests), incl. the F-56 test
  revealing a real re-entrancy defect -- surface with evidence, do not widen scope to fix.
- **H3** `validate --armed` nonzero.
- **H4** a Phase 0 mandatory read materially contradicts section 2 -- stop, report the delta.
- **H5** a REGISTER field/enum/sentinel is needed beyond the FRAMEWORK 14.3/14.4 closed
  vocabularies -- escalate, never invent.
- **H6** truth-law unsatisfiable in a D7 doc without an architectural decision.
- **H7** Weather content grep hits compiled code outside the two D6 files, OR deletion breaks the
  green build (stubs not inert).
- **H8** the renderer cannot express the whole-scene tint within a small clean change (7.2
  guideline) -- report the option set.
- **H9** `ManifestCapabilities` grammar rejects the production owner tokens.
- **H10** Phase E/F fires on the Weather pair unexpectedly (shared-ALC type-identity fault) --
  this contradicts the section 2 facts; report verbatim.
- Standing rails: no pushes to origin; derived registers never hand-edited; `AUDIT_TRAIL.yaml`
  append-only; no history rewrite / force-push / squash; `historical/` and reference trees
  read-only; the auto-mode push-to-main block with in-session re-confirmation is expected behavior.

On halt: stop, report state verbatim, await the operator.

## 14. Closure protocol and report [CORE]

Execute the METHODOLOGY session closure protocol. ROADMAP write-back (C9):

- **F-55 -> CLOSED** (end-to-end mod-authored event through the real pipeline; cite the 7.5.1/7.5.5
  test names + C6 hash).
- **F-56 -> CLOSED** if 7.5.6 is green (cite the test); if red, F-56 stays OPEN with the measured
  defect attached.
- **NEW F-row (G3, severity Medium): mod-data reclamation at unload** -- a mod cannot clean its
  world state at dispose (`OnDispose` is parameterless; component data survives ALC unload as inert
  residue, pinned by the 7.5.7 residue assertion). Investigation pointer: `NativeWorld`
  `BeginModScope/EndModScope/ClearModScope` may already carry the native tracking for a future
  reclamation wave; relates to EQ-b identity/versions and W7 persistence. Owner: architect.
- **NEW ROADMAP note**: S-LOCK candidate ownership-symmetry (section 9) beside the K-L20 LOCK
  analyzer family; BD-9/W6 absorption trigger for `SetAmbientTint` + `AmbientTintCommand`.
- W3 forward state: wave DONE; W4 next.

Closure report (chat): commits table (hash | subject); versions table (ContractsVersion 2.0.0 ->
2.1.0; each doc before -> after; register derived state); gates table (baseline vs closure --
match-or-better, new-test delta named); census pins (section 10 HARD exact, SOFT deltas); F-ledger
final state; consolidated `Skeleton revisions` list (every deviation from intended forms, incl.
any naming/count drift per RESERVED_SURFACE_MUTABILITY); self-attestation (no pushes; sync in
every frontmatter-touching commit; single EVT append, prior entries byte-unchanged; no history
rewrites; historical/ untouched); operator manual checklist (review -> push -> live smoke: run the
Launcher, load the Weather pair via the mod menu, observe tint transitions; then unload and observe
the untinted healthy scene).

## 15. Out of scope [CORE]

- W4 composition-root work (GameBootstrap dissolution); W5 slice moves; BD-9 full presentation
  model (only the single tint primitive lands, with its absorption trigger).
- G3 resolution (F-row only); any `ClearModScope` wiring.
- The sovereign native switch family (F-57): native type IDs, tier-on-contract, native bus wiring.
- Grace-path removal or tightening; the two `Console.WriteLine` sites; F-51, F-10 skip family.
- Analyzer Phase beta; any new analyzer rule (S-LOCK stays a ROADMAP candidate).
- Vanilla.* mods content (they stay hollow); `historical/`; News Intelligence Hub reference tree.
- Pushes to origin (operator act, always).

---

**End of W3_WEATHER_SLICE_BRIEF.md v1.0**
