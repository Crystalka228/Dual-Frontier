---
register_id: DOC-A-GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.0.1
first_authored: 2026-07-18
last_modified: 2026-07-18
content_language: en
next_review_due: 2027-Q3
title: Game Distribution & Vanilla Boundary -- the composition law (engine = simulation OS; the game = its first distribution; game code exists only as mods)
review_cadence: on-change+annual
last_review_date: 2026-07-18
last_review_event: 'Banner PATCH 1.0.0 -> 1.0.1 2026-07-18 (BOUNDARY_BANNER_PATCH, DOC-D-BOUNDARY_BANNER_PATCH_BRIEF; operator chat ratification 2026-07-18): the stale body banner (AUTHORED target-axis law, pending ratification) updated to the ratified-successor LOCKED form; no lifecycle transition. Prior: ratified LOCKED v1.0.0 2026-07-18 per EVT-2026-07-18-BOUNDARY_W0 (BOUNDARY_W0 C3), opening the game-vs-engine composition axis grounded in DOC-E-GAME_ENGINE_BOUNDARY_AUDIT_REPORT (HEAD 4c58942); AUTHORED 0.1.0 enrolled at C1.'
reviewer: Crystalka
---

# Game Distribution & Vanilla Boundary (the composition law)

> **Document class: LOCKED law of the game-vs-engine composition axis.** Ratified 2026-07-18
> (EVT-2026-07-18-BOUNDARY_W0). This document states the composition model the operator ratified and
> the boundary rules that make it enforceable. It opens an authority axis (game-vs-engine composition)
> that no existing corpus document governs -- the standing LOCKED corpus documents the CURRENT
> game-in-engine couplings as intended design (ARCHITECTURE.md section 1-2 Domain layer;
> MOD_OS_ARCHITECTURE.md section 3.4 kernel.* scan; EXECUTION_AUTHORITY_MATRIX.md section
> 3.3), and those documents remain current-truth until the migration waves amend them.
> There is NO conflict: this law governs the target; the migration plan
> (VANILLA_SEPARATION_MIGRATION_PLAN.md) carries the distance. Evidence base:
> docs/reports/GAME_ENGINE_BOUNDARY_AUDIT_REPORT.md, all anchors at HEAD 4c58942.

## 1. The composition model (the law statement)

**The engine is a simulation OS. The game Dual Frontier is its first distribution. Game
code exists only as mods consuming the engine.** This extends K-L9 ("vanilla = mods",
LOCKED) by one increment: K-L9 moved vanilla SYSTEMS into mod projects; this law also
removes vanilla DATA TYPES, presentation semantics, persistence DTOs, and scenario
configuration from every engine assembly. Completion is defined by section 6, not by file
location.

Five levels, strictly ordered. Every type, project, and capability has exactly one home:

| Level | Belongs here | Must NOT be here |
|---|---|---|
| L1 Engine kernel (native + Core/Core.Interop/Runtime) | entity storage, scheduling, event transport, time, lifecycle, resource ownership, C ABI, low-level render/compute | any gameplay noun: Pawn, Combat, Magic, Needs, items, scenario data |
| L2 Engine SDK (Contracts) | the stable types and interfaces through which ANY mod declares systems, data, events, resources | fixed genre-domain lists; references to game assemblies; game enums |
| L3 Optional engine modules | reusable genre-agnostic services (spatial 2D, navigation, tilemap, generic field solvers) | mandatory-for-every-genre coupling; Dual-Frontier-specific balance or schema |
| L4 Game distribution (manifest) | product id + version, root mod set + constraints, default scenario, asset roots, save-compatibility namespace, minimum engine capabilities | compile-time changes to Launcher/Application per game |
| L5 Vanilla mods (game userland) | components, events, systems, AI jobs, worldgen, UI/HUD, balance, recipes, factions -- the entire game | privileged kernel paths unavailable to third-party mods |

The genuinely reusable is NOT automatically kernel: A*, tilemaps, Transform-class types are
L3 libraries, not L1 syscalls.

## 2. Boundary rules (B-1 .. B-6)

- **B-1 Zero engine-to-game references.** No engine assembly (Contracts, Core, Core.Interop,
  Runtime, Application, Launcher) may reference a game assembly or game type, by
  ProjectReference, InternalsVisibleTo, or reflection-by-name. Measured baseline at HEAD
  4c58942: exactly 4 ProjectReference edges (all `DualFrontier.Application.csproj` ->
  Components/Events/Systems/AI), 1 IVT (`Core.csproj` -> Systems, already documented
  removable), 1 test-fixture leak (Fixture.RegularMod_ReplacesCombat -> Core). The
  migration drives these to zero; this rule then holds them there.
- **B-2 No new gameplay nouns in src/ (effective at ratification).** New game mechanics,
  components, events, systems, and configuration are born in `mods/`, never in engine
  assemblies. The existing inventory (28 component types, 53 events, 30 systems, 20 AI
  types, 8 game-binding Contracts types) is frozen as migration stock -- it may only shrink.
- **B-3 SDK sufficiency, no privileged paths.** Everything vanilla needs must arrive through
  the same SDK surface available to any third-party mod. A capability vanilla requires that
  the SDK cannot express is an SDK gap to fix, never a special-case loader branch.
- **B-4 Single type ownership.** Every gameplay component/event/system has exactly one
  owning mod; ownership is namespaced (`mod.<id>.*`). The kernel capability registry
  publishes ENGINE capabilities only -- it stops republishing vanilla types as `kernel.*`
  (today's MOD_OS section 3.4 scan behavior is current-truth until Wave 2 amends it).
- **B-5 Distribution composition.** Launcher/Application know only: kernel, SDK,
  distribution manifest, mod pipeline. Switching the game means switching the manifest,
  not forking the bootstrap.
- **B-6 Enforcement is mechanical, not conventional.** The boundary is held by an analyzer
  or build-time dependency test, not by directory convention. Enforcement instrument:
  Planned -- see ROADMAP.md (the Wave-0 ratchet test, then the analyzer rule). Until it
  ships, this rule is an obligation on the migration plan, not a claim of present
  enforcement.

## 3. Ownership matrix (family level; per-type map lives in the migration plan)

| Family (measured) | Target owner | Note |
|---|---|---|
| Native kernel + df_capi.h (154 exports) | L1 | ALREADY CLEAN -- 0 game-semantic exports (audit R6) |
| Core, Core.Interop, Runtime | L1 | Interop/Runtime already clean (3/9 word-bounded hits, doc examples); Core sheds the domain-bus taxonomy at Wave 2 |
| Contracts minus game types | L2 | sheds: 5 genre bus interfaces, IGameServices, LayerType game members, OwnershipMode |
| DualFrontier.AI: Pathfinding | L3 candidate | only consumed member today; BD-5 decides |
| DualFrontier.AI: BehaviourTree/Jobs, Persistence, Crypto.Future | BD-5 gated | dormant/orphaned -- L3, L5, or deletion |
| DualFrontier.Components (28), Events (53), Systems (30) | L5 | per-slice owners: Vanilla.Core/World/Pawn/Inventory/Combat/Magic; PositionComponent + HealthComponent pass the BD-6 boundary test first |
| Presentation commands (6 records) + game render subscriptions | L5 | engine keeps primitives/layers/slots; BD-9 |
| Persistence DTOs (PawnSnapshot, StorageSnapshot, TerrainKind RLE, WorldSnapshot) | L5 | mod-owned sections/codecs; gated on PSC ratification; BD-7 |
| ScenarioDef / SceneMetadata (orphan today) + GameBootstrap consts | L4/L5 | scenario is distribution+mod configuration; BD-8 |

## 4. Cutover discipline (inherited, cited)

Any transitional duplication created by the migration (a bridge, a facade, a `replaces`
shim, a kept-alive old path) follows EXECUTION_AUTHORITY_MATRIX.md section 3.0 verbatim:
named gate conditions + an equivalence-proof obligation + a DELETION TRIGGER recorded at
creation time. A transitional structure without a recorded deletion trigger is a boundary
violation of this law, not a convenience.

## 5. Falsifiability tie-in

This law is part of the research framework's falsifiable surface: the terminal proof
(Wave 8) is that switching the distribution manifest changes the game while engine binaries
and source stay identical, including one foreign-genre probe distribution. Failure to reach
that proof within the migration program falsifies the composition claim -- record it, do not
soften it.

## 6. Definition of Done (all simultaneously true)

1. The engine solution builds with zero game projects; Components/Events/Systems/AI no
   longer exist as engine assemblies.
2. Launcher/Application have no compile-time knowledge of any game pack (B-5).
3. Every vanilla system uses exactly the SDK available to third-party mods (B-3).
4. Vanilla holds no privileged capabilities, loader branches, or NativeWorld escape hatches.
5. Every gameplay component/event/system has exactly one mod owner (B-4).
6. The kernel capability registry advertises no vanilla types under `kernel.*`.
7. Mod disable/fault/hot-reload removes the mod's scheduler, bus, storage, fields, GPU,
   asset, and presentation resources completely.
8. The save format records mod/schema ownership; no game DTO remains in the engine layer.
9. Three integration profiles pass separately: blank engine, full Vanilla distribution,
   foreign-genre probe.
10. B-1 is held by a shipped mechanical enforcer (analyzer/CI), not convention.

## 7. Amendment protocol

Tier 1. Upon ratification: LOCKED; amendments via FRAMEWORK section 7 (deliberation + EVT +
version bump). The migration plan is the companion program document; wave closures amend the
CURRENT-truth corpus documents (ARCHITECTURE, MOD_OS_ARCHITECTURE, CONTRACTS, and peers) as
each wave lands -- this law itself should rarely change.
