---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ARCHITECTURE
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "0.4.1"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ARCHITECTURE
---
# Dual Frontier architecture

The project is built around one rigid principle: a system MUST NOT access another system's data directly. Interaction happens only through contracts. Any deviation is caught by the isolation guard and produces an immediate crash with diagnostics, not silent state corruption.

> **⚠ Code-truth notice — DD-1 spec-truth restoration (2026-06-02, Documentation Dual-Load Drift Reconnaissance).**
> This document describes the **pre-kernel managed-ECS architecture**. It predates the К8.3+К8.4 managed-world
> retirement (2026-05-14) and the К10.1–К10.3 native-kernel migration (2026-05); it does **not** describe the native
> C++ kernel (`NativeWorld` SSoT per К-L11, native `system_graph` scheduler, native tiered bus), the 21 К-L
> invariants, or the current native/managed split. **Authoritative current architecture:**
> [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 (К-L invariants) + Part 1, and the native source
> `native/DualFrontier.Core.Native/`. A full body rewrite is tracked as remaining DD-1 work
> (see [DD-2/DD-1 refactor progress report](/docs/reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REFACTOR_PROGRESS.md)).
> Where this document disagrees with KERNEL_ARCHITECTURE or the native source, **the latter governs**.

## Change history

- **v0.3 (2026-04):** Phase 4 architectural debt closed. `[Deferred]` / `[Immediate]` delivery is implemented in `DomainEventBus` (per-bus queue, drain between phases, subscriber `SystemExecutionContext` capture); a sixth domain bus, `IPowerBus`, is added for `ElectricGridSystem` + `ConverterSystem`; the ElectricGrid↔Converter component cycle is broken via `[Deferred] ConverterPowerOutputEvent`; `ItemAddedEvent` / `ItemRemovedEvent` / `ItemReservedEvent` are marked `[Deferred]` — `StorageComponent` mutation runs in the `InventorySystem` context, preserving `HaulSystem.writes=[]` isolation; `BridgeImplementationAttribute(Phase = N)` is introduced and applied to every system with a stub `OnInitialize`.
- **v0.2 (2026-04):** Added Lease models ([RESOURCE_MODELS](./RESOURCE_MODELS.md), [EVENT_BUS](./EVENT_BUS.md)), two-phase commit for multi-bus requests ([COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md)), feedback-loop resolution through tick lag ([FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md)), deterministic damage resolution ([COMBO_RESOLUTION](./COMBO_RESOLUTION.md)), golem ownership states ([OWNERSHIP_TRANSITION](./OWNERSHIP_TRANSITION.md)), and the bridge pattern between Phases 5 and 6 ([ROADMAP](./ROADMAP.md)).
- **v0.1 (2026-03):** Initial scaffolding: four layers, five domain buses, declarative isolation, parallel scheduler.

## RimWorld problems being solved

RimWorld is the popular starting point for colony simulators, but its architecture carries three chronic problems, and Dual Frontier fixes each of them.

### Performance

RimWorld's combat system reaches into storage itself to check for ammo. A hundred pawns, sixty ticks per second, an O(n) item walk — the result is tens of thousands of scans per second even on a small colony. Dual Frontier replaces direct scans with a domain bus and invalidation caches: the combat system publishes an `AmmoIntent`, the storage system answers from cache in O(1), and a batch of a hundred intents is processed in one pass.

### Multithreading

RimWorld is single-threaded: every system runs in sequence, even when no conflicts exist between them. Dual Frontier forces every system to declare its readable and writable components through `[SystemAccess]`. The scheduler builds the dependency graph once at startup, topologically sorts it into phases, and runs unrelated systems in parallel — on 8 cores, phases run up to 6–7 threads simultaneously.

### Moddability

In RimWorld a mod patches any private method via Harmony and breaks it. Dual Frontier loads every mod into its own `AssemblyLoadContext`: the mod physically cannot see `DualFrontier.Core`, has no reference to `World` or to any concrete system. Mods interact with each other through `IModContract` — a public-API declaration — not through reflection.

## Four layers

The architecture is split into four layers. Each layer knows only the layers below it.

```
┌─────────────────────────────────────────────────────┐
│                   PRESENTATION                      │
│      Vulkan substrate, sprite render, UI, Input     │
│      Main thread only. Visuals only.                │
├─────────────────────────────────────────────────────┤
│                  APPLICATION                        │
│      GameLoop, SaveSystem, ScenarioManager          │
│      Command queue Domain → Presentation            │
├─────────────────────────────────────────────────────┤
│                    DOMAIN                           │
│   Systems, Entities, Components, Contracts          │
│   Multithreaded. Renderer-agnostic.                 │
├─────────────────────────────────────────────────────┤
│                 INFRASTRUCTURE                      │
│   EventBus (domain buses), Pathfinding,             │
│   SpatialGrid, ParallelScheduler                    │
└─────────────────────────────────────────────────────┘
```

### Presentation

The `DualFrontier.Launcher` assembly (Vulkan substrate via `DualFrontier.Runtime`). Implements the `IRenderer` contract from Application. Reserved DevKit-tier extension `IDevKitRenderer` remains dormant per К-extensions cascade #2 (2026-05-23) — reserved для future first-party DevKit work над Vulkan substrate. Works only in the renderer main thread. Does not call Domain directly — reads commands from `PresentationBridge` and dispatches via `RenderCommandDispatcher`. Current authority: [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md). Historical (superseded): [VISUAL_ENGINE](./historical/VISUAL_ENGINE.md), [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md) — pre-V-substrate dual-backend Godot DevKit + Silk.NET Native state, retired в К-extensions cascade #2.

### Application

The `DualFrontier.Application` assembly. A glue layer: the main game loop (`GameLoop`), the save system, the scenario loader, and `ModLoader`. Contains `PresentationBridge` — a unidirectional command queue from Domain to Presentation.

### Domain

The `DualFrontier.Systems`, `DualFrontier.Components`, `DualFrontier.Events`, and `DualFrontier.AI` assemblies. All game rules. Multithreaded: systems execute in parallel. Never imports renderer-specific code (Vulkan, Win32, et al.) — renderer-agnostic per layer contract.

### Infrastructure

The `DualFrontier.Core` assembly. ECS infrastructure: `World`, `ComponentStore`, `DomainEventBus`, `ParallelSystemScheduler`, `DependencyGraph`, `SpatialGrid`. Everything is `internal` — only contracts are visible from outside. `DualFrontier.Systems` is granted access via `InternalsVisibleTo`.

## Dependency rules

The dependency-arrow direction is strictly top-to-bottom. A violation is an architectural-review error.

- `Contracts` depends on nothing except `System.*`.
- `Components` and `Events` depend only on `Contracts`.
- `Core` depends on `Contracts`.
- `Systems` depends on `Contracts`, `Components`, `Events`, and `Core` through `InternalsVisibleTo`.
- `AI` depends on `Contracts` and `Components`.
- `Application` depends on `Core` and `Systems`.
- `Launcher` depends on `Application` and on the Vulkan substrate ([VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) — `vulkan-1.dll` via pure P/Invoke + Win32 P/Invoke). К-extensions cascade #2 (2026-05-23) retired the historical Godot DevKit + Silk.NET + OpenGL dual-backend; `DualFrontier.Launcher` is the single production renderer.
- Mods depend **only** on `Contracts`. A reference to `Core` from a mod is blocked by `AssemblyLoadContext`.

## Why this way: scenarios

### Scenario 1 — a mage spends mana

`SpellSystem` publishes a `ManaIntent` to `IMagicBus`. `ManaSystem` collects all intents in the next phase, checks the reserve via `ManaComponent`, and answers `ManaGranted` or `ManaRefused`. Between the two steps, the scheduler runs `WeatherSystem` and `NeedsSystem` in parallel. This is exactly what is impossible in single-threaded RimWorld.

### Scenario 2 — a mod adds the School of the Void

The `VoidMagic` mod loads into its own `AssemblyLoadContext`. It registers the `VoidAffinityComponent` component through `IModApi`, registers `VoidSpellSystem` with the `[SystemAccess(reads: [VoidAffinityComponent])]` declaration, and publishes the `IVoidMagicContract` contract. Another mod discovers the contract via `api.TryGetContract<IVoidMagicContract>` and builds its own logic on top — when `VoidMagic` is unloaded, the other mod simply does not subscribe, no crashes.

### Scenario 3 — a pawn dies

`DamageSystem` drops `HealthComponent.Current` to zero and publishes a `DeathEvent` (marked `[Deferred]`) to `IPawnBus`. In the next phase, `MoodSystem` reacts to the death, `SocialSystem` adjusts relationships, and `Application` places a `PawnDiedCommand` on the `PresentationBridge` queue — Launcher picks up the command in its per-frame iteration and dispatches via `RenderCommandDispatcher` (К-extensions cascade #2 2026-05-23 architecture). Domain and Presentation never meet.

## Assembly dependency diagram

```
                     ┌────────────────────────────┐
                     │   DualFrontier.Contracts   │
                     │  (interfaces, attributes)  │
                     └──────────────┬─────────────┘
                                    │
           ┌────────────────────────┼────────────────────────┐
           │                        │                        │
           ▼                        ▼                        ▼
 ┌─────────────────┐   ┌─────────────────────┐   ┌────────────────────┐
 │   Components    │   │       Events        │   │       Core         │
 │   (POCO only)   │   │    (records only)   │   │   (ECS internal)   │
 └────────┬────────┘   └──────────┬──────────┘   └──────────┬─────────┘
          │                       │                         │
          │                       │                         │ InternalsVisibleTo
          ▼                       ▼                         ▼
       ┌────────────────────────────────────────────────────────────┐
       │                   DualFrontier.Systems                     │
       │   Combat / Magic / Pawn / Inventory / Power / World / ...  │
       └──────────────────────────────┬─────────────────────────────┘
                                      │
                                      ▼
                         ┌────────────────────────────┐
                         │  DualFrontier.Application  │
                         │   GameLoop / Save / Mods   │
                         └──────────────┬─────────────┘
                                        │
                                        ▼
                         ┌────────────────────────────┐
                         │   DualFrontier.Launcher    │
                         │ Vulkan substrate / Renderer│
                         └────────────────────────────┘

    ┌──────────────────────┐
    │   Mods/AnyMod.dll    │ ──► depends ONLY on Contracts
    └──────────────────────┘
```

## See also

- [CONTRACTS](./CONTRACTS.md)
- [ECS](./ECS.md)
- [THREADING](./THREADING.md)
- [RESOURCE_MODELS](./RESOURCE_MODELS.md)
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md)
- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md)
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md)
- [OWNERSHIP_TRANSITION](./OWNERSHIP_TRANSITION.md)
