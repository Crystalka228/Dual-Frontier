---
register_id: DOC-A-VANILLA_SEPARATION_MIGRATION_PLAN
project: Dual Frontier
category: A
tier: 2
lifecycle: Live
owner: Crystalka
version: 1.3.0
first_authored: 2026-07-18
last_modified: 2026-07-19
content_language: en
next_review_due: 2026-Q4
title: Vanilla Separation Migration Plan -- waves, gates, decision catalog, and the ownership map for dissolving the game-in-engine Domain layer (successor to historical/MIGRATION_PLAN_KERNEL_TO_VANILLA.md)
review_cadence: on-change
last_review_date: 2026-07-19
last_review_event: 'MINOR 1.2.0 -> 1.3.0 2026-07-19 (W2_BUS_CAPABILITY, DOC-D-W2_BUS_CAPABILITY_BRIEF; operator chat ratification 2026-07-19): W2 marked DONE with commit hashes; BD-3 + BD-10 rows RESOLVED (managed scope) -- the five genre buses + IGameServices collapsed to one DomainEventBus and left Contracts for Core.Bus (ContractsVersion 1 -> 2 MAJOR), KernelCapabilityRegistry became an owner-namespaced ledger relocated to Core/Modding (kernel surface empty, self-access via Owns mechanism-only); §5 map rows marked done; native (providerId,schemaId) type IDs + tier-on-contract deferred to F-57; no lifecycle transition (Live). Prior: MINOR 1.1.0 -> 1.2.0 2026-07-19 (W1_SDK_UNLOCK, DOC-D-W1_SDK_UNLOCK_BRIEF; operator chat ratification 2026-07-19): W1 marked DONE with commit hashes; BD-1 + BD-2 rows RESOLVED with the ratified text; BD-6 operational criterion recorded; no lifecycle transition (Live). Prior: MINOR 1.0.1 -> 1.1.0 2026-07-18 (BOUNDARY_BANNER_PATCH, DOC-D-BOUNDARY_BANNER_PATCH_BRIEF; operator chat ratification 2026-07-18): banner updated to the Live-program form + NEW section 1.1 records the operator scaffolding ruling (delete-and-reimplement over migrate-preserve; equivalence binds engine behavior only) with W3/W5/W7 consequences; no lifecycle transition. Prior: ratified Live v1.0.0 2026-07-18 per EVT-2026-07-18-BOUNDARY_W0 (C3), W0 DONE + PATCH 1.0.1 at C6; ends SUPERSEDED into ROADMAP at W8.'
reviewer: Crystalka
---

# Vanilla Separation Migration Plan (waves + map)

> **Document class: Live program document.** Live since 2026-07-18 (EVT-2026-07-18-BOUNDARY_W0).
> Companion to GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md (the law). This plan carries the measured
> distance from current to target and the wave program that closes it. Rows move to DONE with commit
> hashes as waves close; the document ends
> SUPERSEDED into ROADMAP when Wave 8's proof lands. Evidence base:
> docs/reports/GAME_ENGINE_BOUNDARY_AUDIT_REPORT.md (HEAD 4c58942) -- every number below
> is measured there, none is estimated.

## 1. Relation to the predecessor

historical/MIGRATION_PLAN_KERNEL_TO_VANILLA.md migrated WITHIN a game-in-engine
architecture: K-series prepared the kernel, M-series moved vanilla SYSTEMS into mod
projects (K-L9), and `Components/Events/Systems/AI/Persistence` remained a legitimate
engine "Domain layer". This plan's endpoint is different: the Domain layer DISSOLVES.
The old plan's remaining M-series assumptions (mass `git mv` of prepared systems) are
retired -- the audit shows a mass move today would force vanilla mods to reference Core
(C-1) or spawn privileged facades. The SDK is unlocked first; slices move only after.

## 1.1 The scaffolding ruling (2026-07-18)

Operator ruling, ratified in chat 2026-07-18 (recorded via DOC-D-BOUNDARY_BANNER_PATCH_BRIEF): the
current gameplay logic is NOT a product to preserve -- it is a minimal conditional test harness. The
separation program may therefore DELETE and REIMPLEMENT rather than migrate-preserve. Consequences:
equivalence obligations bind ENGINE behavior only, never harness gameplay behavior; there is no
save-compatibility obligation toward pre-separation saves of the harness; vanilla content is grown
CLEAN inside mods, not rescued out of src/. The law (Definition of Done section 6, B-1..B-6) is
unchanged -- only the migration MECHANICS get cheaper.

Wave consequences:
- **W3 (vertical slice)** is WRITTEN FRESH in the mod: the src/ Weather code is reference material,
  not a migration source.
- **W5 (slice replacement)**: implement each slice clean in its owning mod, then DELETE the src/
  originals in the same closure -- no equivalence proof against harness gameplay behavior; the ratchet
  census still shrinks by the slice's edge count.
- **W7 (persistence)** carries NO backward-compatibility obligation toward harness-era saves: the PSC
  schema starts clean.

## 2. Measured baseline (audit digest -- re-verify at each wave's Phase 0)

- Compile boundary: 4 engine->game ProjectReference edges (all in
  `DualFrontier.Application.csproj`), 1 IVT (`Core.csproj` -> Systems), 1 test-fixture
  leak (Fixture.RegularMod_ReplacesCombat -> Core). Native tree + df_capi.h: CLEAN (0
  game-semantic exports of 154).
- Stock to migrate: Components 28 types / Events 53 / Systems 30 / AI 20; Contracts
  game-binding surface ~8 types (ICombatBus, IInventoryBus, IMagicBus, IPawnBus,
  IWorldBus, IGameServices, LayerType game members, OwnershipMode); 6 presentation
  command records; 4 persistence DTO families; ScenarioDef/SceneMetadata (orphan) +
  hardcoded GameBootstrap consts.
- Vanilla-as-mods reality: 6 vanilla mods are hollow IMod skeletons, absent from
  DualFrontier.sln (only Mod.Example enrolled); 0 SystemBase subclasses under mods/ vs
  34 declarations under src/.
- The structural blocker (A4): SystemBase cannot relocate to Contracts --
  Contracts -> Core.Interop -> Contracts cycle. A NEW SDK abstraction is required (BD-1).
- The structural force: shared game types get hoisted into Contracts because two game
  assemblies must share them (OwnershipMode's own doc states this). Wave 1's shared-mod
  contract assembly removes the force.

## 3. Decision catalog (BD-1 .. BD-10)

Each wave charters only when its listed decisions are ratified. Leans are architect leans,
NOT ratified; each decision gets its own recon-grounded deliberation at chartering.

| # | Fork | Measured context | Resolves at | Architect lean |
|---|---|---|---|---|
| BD-1 | SDK system-contract form | A4 cycle blocks relocation; ModRegistry gates on SystemBase + [SystemAccess] + [TickRate] | W1 ✅ | **RESOLVED W1**: public `ISimulationSystem` (Initialize/Tick/OnDispose, no `float delta` -- SimTick via the context) + capability-scoped `ISystemContext` in Contracts (component access over the measured union, events via the live capability gate, SimTick; NO fields/managed-store/services -- services arrive at construction via `ISystemServices`, fields deferred per N17). Engine-internal `SystemAdapter<T>` (in Application, not Core -- the view needs RestrictedModApi + Core.Interop IVT) wraps to the executor; concrete NativeWorld stays kernel-internal behind opaque-object Contracts handles |
| BD-2 | Construction/DI model | `Activator.CreateInstance` (ModRegistry.cs:287) forbids ctor dependencies | W1 ✅ | **RESOLVED W1**: `RegisterSystem<T>(Func<ISystemServices,T>)` + parameterless convenience; core + mod construction unified through ModRegistry (GameBootstrap stops hand-instantiating -- the bifurcation dies). Mod-facing IModApi factory overload DEFERRED (N17, no consumer yet) so ContractsVersion stays unchanged |
| BD-3 | Generic event routing | 5 genre buses + IGameServices baked into Contracts/Core; ModBusRouter reflects over IGameServices properties; [SystemAccess] binds `nameof(IGameServices.X)` | W2 ✅ | **RESOLVED W2** (managed scope): the five genre buses + `IGameServices` collapsed to ONE generic `DomainEventBus` (BD-3b), the five getters now cosmetic bridges over it (`UnifiedBusParityTests`); the interfaces relocated out of Contracts to `Core.Bus` (BD-3a — ContractsVersion 1→2 MAJOR, a breaking interface removal); mods route by event type to the one dispatch (`ModBusRouter` + the `[EventBus]` attribute deleted; `[SystemAccess]` loses its `bus:` member, F-54); namespaced channel ownership via `KernelCapabilityRegistry.RegisterOwner` (mechanism, see BD-10). The native-facing parts — tier/latency on the event contract, native bus type IDs from a dynamic registry — stay FENCED behind the sovereign native switch, tracked as F-57 |
| BD-4 | Distribution manifest shape | no config file or loader exists (EAM R13); 6 mods off-solution | W4 | `game.manifest.json`: product id/version, root mod set + constraints, default scenario, asset roots, save namespace, min engine capabilities |
| BD-5 | Optional modules vs vanilla vs deletion | AI: only Pathfinding consumed (BehaviourTree/Jobs dormant); Persistence + Crypto.Future production-orphaned | W5 | Pathfinding -> L3 module; BehaviourTree/Jobs -> ride their first consumer; Crypto.Future -> deletion candidate; Persistence -> engine-generic core after DTO extraction |
| BD-6 | Position/Health boundary test | kernel scans HealthComponent as capability marker; GameBootstrap uses PositionComponent generically | W5 (criterion recorded W1) | law from the assessment: if the kernel only stores bytes, the type is game/shared-mod-owned; if the kernel interprets it, it is a versioned L3 contract. Expect Position -> L3 spatial contract, Health -> vanilla. **Operational criterion (recorded W1)**: per component type, the boundary test asserts whether any engine (`src/`) code reads a specific field of the type as a capability marker or control input (INTERPRETS -> versioned L3 contract) versus only storing/relaying its bytes (game/shared-mod-owned); the executable Position/Health test lands at W5 when the slice moves |
| BD-7 | Persistence ownership | EAM R12 vacant; 4 game DTO families engine-side; PSC held AUTHORED | W7 | engine snapshot = tick/time/RNG + identity + mod set + schema table + namespaced sections; mods own codecs/migrations; requires PSC ratification first |
| BD-8 | Config ownership | ScenarioDef.StartingPawnCount + SceneMetadata.EtherDensity ORPHAN (never read); GameBootstrap hardcodes consts :58-68 | W4 | scenario/config -> distribution manifest (L4) + owning mods (L5); orphans deleted, not migrated |
| BD-9 | Presentation slots | 6 game command records; LayerType.CombatFeedback in Contracts | W6 | generic ordered layer/slot ids owned by engine; game layers registered by mods; engine renders primitives without knowing pawn/item semantics |
| BD-10 | kernel.* reframing | KernelCapabilityRegistry (in Application -- A9) hard-scans Components/Events via markers, publishes vanilla types as kernel.* | W2 ✅ | **RESOLVED W2**: `KernelCapabilityRegistry` became an owner-namespaced **registration ledger**, relocated out of the composition layer (Application → `Core/Modding`); `BuildFromKernelAssemblies` retired → the kernel-provided FQN set is **empty** (the engine owns no gameplay types); `RegisterOwner(owner, assembly)` emits `<owner>.{verb}:{FQN}` and records ownership for the self-access predicate `Owns`. Live per-mod registration is mechanism-only — no producer this wave (vanilla mods define no types yet); wiring deferred to the slice-move wave |

## 4. Waves (each = its own cascade family with recon, brief, gates, deletion triggers)

**W0 -- Law + freeze + ratchet.** Ratify the boundary law; B-2 freeze active (no new
gameplay nouns in src/); enroll the 6 vanilla mods into DualFrontier.sln (A1 -- they must
be real build participants before anything migrates into them); ship the RATCHET: a
build-time dependency test asserting the exact engine->game edge census (4+1+1) so it can
only shrink. The ratchet carries its deletion trigger: replaced by the analyzer rule the
moment one ships (B-6). ROADMAP row seeded. Gate: law LOCKED; ratchet red-once-then-green
proven; sln builds with the 6 mods.
> **W0 DONE 2026-07-18** (BOUNDARY_W0; EVT-2026-07-18-BOUNDARY_W0): law LOCKED 1.0.0, this plan
> Live 1.0.1, BoundaryRatchetTests red-once-then-green, the 6 vanilla mods enrolled + building.
> Commits da97308 (C1 enroll) / 4aa1fa0 (C2 sln) / b973192 (C3 flips) / a26e8ac (C4 ratchet) /
> c9387c1 (C5 comment) / closure C6.
**W1 -- SDK surface unlock (BD-1, BD-2; BD-6 test authored).** New Contracts-level system
abstraction + ISystemContext; ModRegistry accepts it alongside SystemBase (bridge with
deletion trigger: SystemBase path deleted when the last src/ system migrates, W5); ONE
reference example mod with a REAL component + system + event, Contracts-only. Gate: the
example mod compiles against Contracts alone, registers -> ticks -> faults (D2 route) ->
disposes -> ALC collects, and never names Core.
> **W1 DONE 2026-07-19** (W1_SDK_UNLOCK; EVT-2026-07-19-W1_SDK_UNLOCK): BD-1 (`ISimulationSystem` +
> `ISystemContext`) + BD-2 (factory registration) LANDED; Mod.Example authored Contracts-only (real
> component + system + event); the `Fixture.RegularMod_ReplacesCombat` retargeted off Core (the
> empirical BD-1 gap-proof leak is dead); full SDK lifecycle proven (register -> tick -> D2
> fault/quarantine -> dispose -> ALC unload). CONTRACTS 1.0.1->1.1.0, ECS 1.0.1->1.1.0, MOD_OS
> 1.0.2->1.1.0 (all MINOR); ContractsVersion unchanged. Code-truth correction recorded: the F5
> `allowedBuses` router/validator was deleted at K8.3+K8.4 -- events route through the live
> `RestrictedModApi` manifest-capability gate instead (ROADMAP F-row). Commits d3edbdb (C1 enroll) /
> 6449306 (C2 contract) / 2dea4fa (C3 adapter+registration) / 4d86d2d (C4 proof mod+fixture) /
> 4264d6f (C5 tests) / C6 governance / C7 closure.
**W2 -- Type/bus/capability ownership (BD-3, BD-10).** Genre buses and IGameServices leave
the engine contract (typed hub or channels per BD-3); capability registry -> ledger;
deterministic type IDs from (providerId, schemaId); manifest capability checked against
type owner. Gate: kernel capability surface contains zero Pawn/Combat/Magic/Inventory/
World types; [SystemAccess] no longer binds nameof(IGameServices.X).
> **W2 DONE 2026-07-19** (W2_BUS_CAPABILITY; EVT-2026-07-19-W2_BUS_CAPABILITY): BD-3 + BD-10 RESOLVED
> (managed scope). The five genre buses + `IGameServices` collapsed to ONE generic `DomainEventBus` (getters
> now cosmetic bridges, `UnifiedBusParityTests`) and left Contracts for `Core.Bus` (ContractsVersion 1.0.0 ->
> 2.0.0 MAJOR); `ModBusRouter` + the `[EventBus]` attribute + the `[SystemAccess(bus:)]` form deleted (F-54
> CLOSED); mods route by event type to the one dispatch. `KernelCapabilityRegistry` -> owner-namespaced
> ledger, relocated Application -> `Core/Modding`; `BuildFromKernelAssemblies` retired -> kernel surface
> EMPTY; `RegisterOwner`/`Owns` self-access is mechanism-only (no producer this wave). Gate MET: kernel
> capability surface holds zero Pawn/Combat/Magic/Inventory/World types; `[SystemAccess]` no longer binds
> `nameof(IGameServices.X)`. Deferred to the sovereign native switch (F-57): deterministic type IDs from
> (providerId, schemaId), tier/latency declared on the event contract, native bus type IDs from a dynamic
> registry. CONTRACTS 1.1.0->2.0.0 (MAJOR), EVENT_BUS 1.1.0->1.2.0, MOD_OS 1.1.0->1.2.0 (MINOR), this plan
> 1.2.0->1.3.0. Commits 61127cc (C1 enroll) / 691aeb2 (C2 retire _allowedBuses) / 9f7107d (C3 collapse) /
> 6b0b7d6 (C4 contracts MAJOR) / 95365af (C5 ledger) / eb7bca8 (C6 tests) / C7 governance / C8 closure.
**W3 -- Walking vertical slice (written fresh).** ONE small mechanic end-to-end as a real mod, WRITTEN
FRESH (candidate: Weather -- the src/ WeatherSystem + WeatherChangedEvent are reference material, not a
migration source; section 1.1): component, system,
event, initial data, presentation reaction, unload/reload. Purpose: surface every missing
world/service/asset/input/lifecycle SDK gap BEFORE mass migration. Gate: disabling the mod
removes the mechanic entirely; engine stays healthy.
**W4 -- Composition root + scenario (BD-4, BD-8).** GameBootstrap dissolves into an
EngineSession composition root knowing only kernel/SDK/manifest/pipeline (INTERSECTS EQ-a
Cascade B decision D3 -- EngineSession is designed ONCE, under this law); component
registration, factories, seeds, initial spawn move to vanilla lifecycle stages; distribution
manifest ships; orphan config deleted. Gate: EngineSession compiles with zero references to
Components/Events/Systems/AI.
**W5 -- Slice replacement (clean rebuild + delete) (BD-5, BD-6).** Dependency-aware order: Vanilla.Core
shared contracts -> World -> Pawn -> Inventory -> Combat -> Magic. Per slice, one closure: implement the
slice CLEAN in its owning mod (register schemas -> systems -> tests -> presentation -> unload proof),
then DELETE the src/ originals (sources/ProjectReferences/capabilities) in the same closure -- no
equivalence proof against harness gameplay behavior (section 1.1). Never two type identities of one
component across ALCs. Gate per slice: single mod owner; ratchet census shrinks by the slice's edge count.
**W6 -- Presentation, input, assets (BD-9).** Game render commands + HUD to vanilla
presentation mods; namespaced asset handles, input actions, layer registration,
deterministic cleanup. Gate: headless engine and a foreign pack load zero Dual Frontier
presentation types.
**W7 -- Generic persistence (BD-7; after PSC ratification).** Engine snapshot generic;
mod-owned sections/codecs/migrations; missing/updated-mod policy; game DTOs + RLE +
quantisation are re-authored CLEAN in vanilla (NO backward-compatibility obligation toward harness-era
saves; the PSC schema starts clean -- section 1.1). Gate: engine saves a blank/foreign distribution with no game
DTO; vanilla save round-trips only under a compatible mod set or explicit migrations.
**W8 -- Reuse proof (the falsifiability capstone).** Engine-only boot; full Vanilla
distribution through the ordinary Mod OS path; ONE foreign-genre probe distribution with
unchanged kernel/Application; last transitional bridges + game-specific analyzer
exceptions deleted. Gate: switching the manifest switches the game; engine binaries and
source identical across all three profiles.

## 5. The map (family -> target owner; the law's section-3 matrix, applied)

| Current location | Members (measured) | Target owner |
|---|---|---|
| DualFrontier.Components | 28 types: Pawn/Needs cluster, Health, Position, items/storage/workbench, combat (weapon/armor/faction), magic (mana/ether/golem bond), terrain/weather | Vanilla.Pawn / Vanilla.Inventory / Vanilla.Combat / Vanilla.Magic / Vanilla.World; genuinely cross-slice types -> Vanilla.Core shared mod; Position per BD-6 (expected L3 spatial contract); Health per BD-6 (expected vanilla) |
| DualFrontier.Events | 53 event records | owning slice mods; engine keeps only IEvent + generic routing (BD-3) |
| DualFrontier.Systems | 30 systems | owning slice mods after W1 SDK unlock; zero gameplay entries remain in core registration |
| DualFrontier.AI | 20 types; Pathfinding consumed, BT/Jobs dormant | Pathfinding -> L3 candidate; BT/Jobs + behaviour leaves -> Vanilla.Pawn/Combat/Magic (BD-5) |
| Contracts game surface | 5 genre buses, IGameServices, LayerType game members, OwnershipMode | buses + IGameServices **dissolved to `Core.Bus` ✅ W2 (BD-3a/b)**; OwnershipMode -> Vanilla.Magic shared contract; LayerType game members -> mod-registered slots (BD-9, later) |
| Application/Bridge commands | PawnSpawned/PawnState/PawnDied/ItemSpawned + 2 more (6 records) | Vanilla presentation mods (W6) |
| Persistence DTOs | PawnSnapshot, StorageSnapshot, TileMapSnapshot (TerrainKind RLE), WorldSnapshot | mod-owned sections/codecs (W7) |
| Scenario/config | ScenarioDef, SceneMetadata (orphan), GameBootstrap consts | manifest (L4) + owning mods; orphans deleted (BD-8, W4) |
| KernelCapabilityRegistry | in Application (A9) | engine-side registration ledger, engine capabilities only (BD-10, W2) — **✅ DONE W2: relocated to `Core/Modding`, owner-namespaced, kernel surface empty** |

## 6. Interaction with the standing queues

- **EQ-a Cascade B (shutdown transaction):** proceeds NEXT, engine-side; its D3
  (EngineSession owner) is decided citing this law's B-5 so the session root is built once.
  W4 later consumes that EngineSession rather than re-creating it.
- **EQ-b (identity versions) and PSC:** prerequisites for W7; unchanged order.
- **Doc amendments:** each wave's closure amends the CURRENT-truth corpus docs it obsoletes
  (ARCHITECTURE Domain layer, MOD_OS 3.4, CONTRACTS bus canon, MODDING author guide) --
  per-wave, never as one big-bang rewrite.
- **A6 rider:** the stale net8.0 comment in src/Directory.Build.props rides the next
  convenient cascade.

## 7. Standing rails

Read-only recon before every wave brief (Lesson #N18); every transitional bridge carries a
deletion trigger at creation (law section 4); wave closures append EVTs and update this
plan's wave table with hashes; a wave that cannot meet its gate HALTS and reports -- gates
are never weakened to pass. Bez kostylei.
