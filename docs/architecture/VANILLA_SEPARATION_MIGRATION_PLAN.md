---
register_id: DOC-A-VANILLA_SEPARATION_MIGRATION_PLAN
project: Dual Frontier
category: A
tier: 2
lifecycle: Live
owner: Crystalka
version: 1.0.1
first_authored: 2026-07-18
last_modified: 2026-07-18
content_language: en
next_review_due: 2026-Q4
title: Vanilla Separation Migration Plan -- waves, gates, decision catalog, and the ownership map for dissolving the game-in-engine Domain layer (successor to historical/MIGRATION_PLAN_KERNEL_TO_VANILLA.md)
review_cadence: on-change
last_review_date: 2026-07-18
last_review_event: 'Ratified Live v1.0.0 2026-07-18 per EVT-2026-07-18-BOUNDARY_W0, BOUNDARY_W0 cascade C3 (companion to DOC-A-GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY, LOCKED the same commit). W0 row moves to DONE at closure (C6); the document ends SUPERSEDED into ROADMAP at W8.'
reviewer: Crystalka
---

# Vanilla Separation Migration Plan (waves + map)

> **Document class: Draft program document.** Companion to
> GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md (the law). This plan carries the measured
> distance from current to target and the wave program that closes it. Flips Live at
> ratification; rows move to DONE with commit hashes as waves close; the document ends
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
| BD-1 | SDK system-contract form | A4 cycle blocks relocation; ModRegistry gates on SystemBase + [SystemAccess] + [TickRate] | W1 | public `ISimulationSystem` + capability-scoped `ISystemContext` (world views, events, fields, services) in Contracts; engine-internal adapter wraps to the executor; concrete NativeWorld stays kernel-internal |
| BD-2 | Construction/DI model | `Activator.CreateInstance` (ModRegistry.cs:287) forbids ctor dependencies | W1 | registration-time factory delegate (`RegisterSystem<T>(Func<ISystemServices,T>)`); parameterless overload remains as convenience |
| BD-3 | Generic event routing | 5 genre buses + IGameServices baked into Contracts/Core; ModBusRouter reflects over IGameServices properties; [SystemAccess] binds `nameof(IGameServices.X)` | W2 | typed event hub with namespaced channel ownership (provider mod id); tier/latency declared on the event contract; native bus type IDs from a dynamic registry |
| BD-4 | Distribution manifest shape | no config file or loader exists (EAM R13); 6 mods off-solution | W4 | `game.manifest.json`: product id/version, root mod set + constraints, default scenario, asset roots, save namespace, min engine capabilities |
| BD-5 | Optional modules vs vanilla vs deletion | AI: only Pathfinding consumed (BehaviourTree/Jobs dormant); Persistence + Crypto.Future production-orphaned | W5 | Pathfinding -> L3 module; BehaviourTree/Jobs -> ride their first consumer; Crypto.Future -> deletion candidate; Persistence -> engine-generic core after DTO extraction |
| BD-6 | Position/Health boundary test | kernel scans HealthComponent as capability marker; GameBootstrap uses PositionComponent generically | W5 (test at W1) | law from the assessment: if the kernel only stores bytes, the type is game/shared-mod-owned; if the kernel interprets it, it is a versioned L3 contract. Expect Position -> L3 spatial contract, Health -> vanilla |
| BD-7 | Persistence ownership | EAM R12 vacant; 4 game DTO families engine-side; PSC held AUTHORED | W7 | engine snapshot = tick/time/RNG + identity + mod set + schema table + namespaced sections; mods own codecs/migrations; requires PSC ratification first |
| BD-8 | Config ownership | ScenarioDef.StartingPawnCount + SceneMetadata.EtherDensity ORPHAN (never read); GameBootstrap hardcodes consts :58-68 | W4 | scenario/config -> distribution manifest (L4) + owning mods (L5); orphans deleted, not migrated |
| BD-9 | Presentation slots | 6 game command records; LayerType.CombatFeedback in Contracts | W6 | generic ordered layer/slot ids owned by engine; game layers registered by mods; engine renders primitives without knowing pawn/item semantics |
| BD-10 | kernel.* reframing | KernelCapabilityRegistry (in Application -- A9) hard-scans Components/Events via markers, publishes vanilla types as kernel.* | W2 | registry becomes a registration ledger; engine capabilities only; mod types register dynamically as mod.<id>.*; registry relocates out of the composition layer |

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
**W2 -- Type/bus/capability ownership (BD-3, BD-10).** Genre buses and IGameServices leave
the engine contract (typed hub or channels per BD-3); capability registry -> ledger;
deterministic type IDs from (providerId, schemaId); manifest capability checked against
type owner. Gate: kernel capability surface contains zero Pawn/Combat/Magic/Inventory/
World types; [SystemAccess] no longer binds nameof(IGameServices.X).
**W3 -- Walking vertical slice.** ONE small mechanic end-to-end as a real mod (candidate:
Weather -- WeatherSystem + WeatherChangedEvent exist and are leaf-like): component, system,
event, initial data, presentation reaction, unload/reload. Purpose: surface every missing
world/service/asset/input/lifecycle SDK gap BEFORE mass migration. Gate: disabling the mod
removes the mechanic entirely; engine stays healthy.
**W4 -- Composition root + scenario (BD-4, BD-8).** GameBootstrap dissolves into an
EngineSession composition root knowing only kernel/SDK/manifest/pipeline (INTERSECTS EQ-a
Cascade B decision D3 -- EngineSession is designed ONCE, under this law); component
registration, factories, seeds, initial spawn move to vanilla lifecycle stages; distribution
manifest ships; orphan config deleted. Gate: EngineSession compiles with zero references to
Components/Events/Systems/AI.
**W5 -- Atomic slice moves (BD-5, BD-6).** Dependency-aware order: Vanilla.Core shared
contracts -> World -> Pawn -> Inventory -> Combat -> Magic. Per slice, one closure:
register schemas -> systems -> tests -> presentation -> unload proof -> DELETE old
sources/ProjectReferences/capabilities. Never two type identities of one component across
ALCs. Gate per slice: single mod owner; ratchet census shrinks by the slice's edge count.
**W6 -- Presentation, input, assets (BD-9).** Game render commands + HUD to vanilla
presentation mods; namespaced asset handles, input actions, layer registration,
deterministic cleanup. Gate: headless engine and a foreign pack load zero Dual Frontier
presentation types.
**W7 -- Generic persistence (BD-7; after PSC ratification).** Engine snapshot generic;
mod-owned sections/codecs/migrations; missing/updated-mod policy; game DTOs + RLE +
quantisation move to vanilla. Gate: engine saves a blank/foreign distribution with no game
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
| Contracts game surface | 5 genre buses, IGameServices, LayerType game members, OwnershipMode | dissolves at W2 (BD-3); OwnershipMode -> Vanilla.Magic shared contract; LayerType game members -> mod-registered slots (BD-9) |
| Application/Bridge commands | PawnSpawned/PawnState/PawnDied/ItemSpawned + 2 more (6 records) | Vanilla presentation mods (W6) |
| Persistence DTOs | PawnSnapshot, StorageSnapshot, TileMapSnapshot (TerrainKind RLE), WorldSnapshot | mod-owned sections/codecs (W7) |
| Scenario/config | ScenarioDef, SceneMetadata (orphan), GameBootstrap consts | manifest (L4) + owning mods; orphans deleted (BD-8, W4) |
| KernelCapabilityRegistry | in Application (A9) | engine-side registration ledger, engine capabilities only (BD-10, W2) |

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
