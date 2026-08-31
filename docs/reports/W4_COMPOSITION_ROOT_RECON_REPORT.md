---
register_id: DOC-E-W4_COMPOSITION_ROOT_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-08-30'
last_modified: '2026-08-30'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'W4 COMPOSITION ROOT + SCENARIO RECON REPORT — 2026-08-30 (A-H) — measurement at HEAD 34bc221 by a read-only Opus subagent for the W4 charter (BD-4 + BD-8): PRODUCTION LOADS ZERO MODS (CreateSession never calls Apply; the sole Apply site sits behind the mod menu the Launcher never opens — the boot-time mod-load path must be CREATED); the W4 gate as the plan words it is ALREADY GREEN at class level (EngineSession.cs has zero game-namespace references since EQ_A2/B-5) so the falsifiable gate is ASSEMBLY-scoped — the 4 ProjectReference edges at DualFrontier.Application.csproj:11,14,15,16; the forbidden-namespace footprint in Application is SURGICAL (6 files / 24 usings / 0 fully-qualified: GameBootstrap, VanillaComponentRegistration, RandomPawnFactory, ItemFactory, PawnStateCommand needing SkillKind only, ItemSpawnedCommand needing ItemKind only) so cutting the 4 edges is compiler-enforceable with NO new csproj and the W0 ratchet baseline shrinks 4->0 by its own ratified protocol; ISimulationSystem.Initialize is a live-world hook that fires PER GRAPH BUILD (ctor + every Apply-driven Rebuild) not once per session — the Weather lazy read-then-mint Tick seed is the re-entry-safe precedent; ModRegistry ALREADY supports mod registration of SystemBase types (the F60A-measured bridge path) so the 10 real systems can be mod-registered without moving files (W5 territory); IModApi exposes NO world access; the BD-8 orphan cluster is ~295 lines / 11 files (the whole Scene/ folder + ScenarioDef + the never-called ScenarioLoader) — 4x the named scope; 11 consts at GameBootstrap.cs:65-75 (the BD-8 :58-68 anchor is stale by 7); ScenarioDef.WorldSeed is the only modelled world seed (F-42 coordination); ManifestParser silently ignores unknown keys; DefaultModDiscoverer has NO ordering semantics; dead edge Systems->AI; GameServices genre nouns live in engine Core.Bus (invisible to a namespace gate — vocabulary clause is future scope); MOD_OS_ARCHITECTURE.md:570 stale (claims F-60 open, cites two renamed tests); EAM/plan anchor drift at 8+ sites; blast radius = 18 GameBootstrap integration tests + 8 EngineSession transaction tests; 9 discrepancies + 10 surprises enumerated'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-LINUX_PRESENT_1_RECON_REPORT, DOC-E-F60A_TICK_PATH_RECON_REPORT). Operator-requested subagent recon (2026-08-30) for the W4_COMPOSITION_ROOT charter, executed by a fresh read-only Opus subagent at HEAD 34bc221. Read-only: zero repository mutations besides this file, sync never run, zero builds/tests by the subagent. UNTRACKED at authoring — enrolls at the charter cascade C1.'
---

# W4 COMPOSITION ROOT + SCENARIO — READ-ONLY RECON

**HEAD**: `34bc221c569c95645b81a2cf136e04487752b9b2` — `34bc221 Merge pull request #53 from Crystalka228/claude/linux-present-1`. **MATCHES** the expected `34bc221`. Branch `main`, clean except `M .claude/settings.local.json`.
Command: `git rev-parse HEAD && git log --oneline -1 && git branch --show-current`

Discipline held: reads and non-mutating commands only. No edits, no builds, no `dotnet`/`cmake`, no git state changes.

---

## A. GAMEBOOTSTRAP ANATOMY (the dissolution target)

**File**: `/mnt/Work/Colony_Simulator/src/DualFrontier.Application/Loop/GameBootstrap.cs` — **284 lines**, `internal static class GameBootstrap` at `:63`.
Command: `wc -l src/DualFrontier.Application/Loop/GameBootstrap.cs`

### A.1 Members (complete — three methods, no fields but the consts)

| Member | Anchor | Visibility | Role |
|---|---|---|---|
| `CreateSession(PresentationBridge, string modsRoot = "mods", ShutdownTransactionHooks? = null) : EngineSession` | `:77-251` | `public static` | THE composition sequence |
| `PublishItemSpawnedEvents(NativeWorld, GameServices)` | `:257-263` | `private static` | seed-event emission (4 item kinds) |
| `PublishKind<T>(NativeWorld, GameServices, ItemKind) where T : unmanaged, IComponent` | `:265-283` | `private static` | per-kind span walk + `ItemSpawnedEvent` publish |

### A.2 Hardcoded consts — **11 consts at `:65-75`** (BD-8 claims `:58-68`; **stale by 7 lines**)

Command: `sed -n '65,75p' src/DualFrontier.Application/Loop/GameBootstrap.cs | grep -c "private const"` → `11`

```
:65  private const int MapWidth               = 200;
:66  private const int MapHeight              = 200;
:67  private const int InitialPawnCount       = 50;
:68  private const int FactorySeed            = 42;
:69  private const int ObstacleSeed           = 42;
:70  private const int ObstacleCount          = 800;
:71  private const int InitialFoodCount       = 150;
:72  private const int InitialWaterCount      = 50;
:73  private const int InitialBedCount        = 30;
:74  private const int InitialDecorationCount = 25;
:75  private const int ItemFactorySeed        = 43;
```

Not consts but equally hardcoded scenario inputs inside `CreateSession`: `priorityClass: 2` / `wakeType: 0` (`:197-198`), `SubscribeTimer(i, 1)` (`:201`), `modsRoot` default literal `"mods"` (`:79`).

### A.3 `CreateSession` sequence, in order, with anchors

| # | Step | Anchor | Touches |
|---|---|---|---|
| 1 | `Bootstrap.Run(useRegistry: true)` → `NativeWorld` | `:86` | kernel |
| 2 | `VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!)` | `:87` | **component registration (21 vanilla types)** |
| 3 | `new GameServices()` (Core.Bus, `internal`), `new TickScheduler()` | `:89-90` | engine |
| 4 | 5 bus→bridge subscriptions | `:92-103` | **game events → render commands** |
| 5 | `new NavGrid(200,200)` + 800 obstacles from `new Random(ObstacleSeed)` + `new AStarPathfinding(navGrid)` | `:105-113` | **AI + scenario** |
| 6 | `new RandomPawnFactory(42, navGrid, 200, 200).Spawn(world, services, 50)` | `:115-116` | **initial spawn** |
| 7 | collect pawn `PositionComponent`s into `excludedPositions` | `:121-126` | **components** |
| 8 | `new ItemFactory(43, …).Spawn(world, excluded, 150, 50, 30, 25)` | `:128-135` | **initial spawn** |
| 9 | `PublishItemSpawnedEvents(world, services)` | `:139` | **seed events** |
| 10 | `new ModRegistry()`; `SetSystemServices(new SystemServices(pathfinding))`; `SetTickSource(() => ticks.CurrentTick)` | `:144-146` | engine + AI service |
| 11 | `SetPresentationSink(new BridgePresentationSink(bridge))` | `:152` | engine |
| 12 | **10 × `modRegistry.RegisterSystem<T>(factory)`** | `:154-163` | **system construction** |
| 13 | `GetCoreSystemInstances()` → `DependencyGraph` add/Build | `:165-170` | engine |
| 14 | `SystemGraphInterop.Clear()` / `WakeRegistryInterop.Clear()`; per-system `RegisterSystem(...)` with EMPTY read/write sets + `SubscribeTimer(i,1)`; `ComputeStaticGraph()` | `:182-203` | engine (native graph) |
| 15 | `new ModLoader()`; `new ModFaultHandler()`; `SetFaultHandler` | `:205-207` | engine |
| 16 | `SystemMetadataBuilder.Build(modRegistry)` | `:209-210` | engine |
| 17 | `new ParallelSystemScheduler(phases, ticks, metadata, faultHandler, world, services, registry)` | `:212-219` | engine |
| 18 | `new ContractValidator()`, `new ModContractStore()`, `new ModIntegrationPipeline(…, nativeWorld.Registry)` | `:221-227` | engine |
| 19 | `new DefaultModDiscoverer(modsRoot)`; `new ModMenuController(pipeline, discoverer)` | `:228-229` | engine |
| 20 | `new ManagedBusBridge()` | `:235` | engine |
| 21 | `new GameLoop(scheduler, ticks, bridge, busBridge)` | `:237` | engine |
| 22 | `controller.OnEditingBegan/Ended` → `loop.SetPaused` | `:239-240` | engine |
| 23 | `new EngineSession(world, busBridge, pipeline, services, loop, controller, shutdownHooks)` | `:242` | engine |
| 24 | `scheduler.OnModQuarantined = (modId,tick) => session.ReportDegraded(...)` | `:247-248` | engine |
| 25 | `return session` | `:250` | — |

**Load-bearing negative**: `CreateSession` NEVER calls `pipeline.Apply(...)`. The only `Apply` call site in `src/` is `ModMenuController.cs:273`, reachable only through `BeginEditing()` → `Commit()`. The Launcher (`Program.cs`) never opens the mod menu. **No mod is loaded in production today.**
Command: `grep -rn "\.Apply(" --include=*.cs src/ | grep -v '/obj/'` → 1 hit (`ModMenuController.cs:273`).

### A.4 Forbidden-namespace census — GameBootstrap

**8 using directives** into the four game namespaces (`:3`, `:10`, `:11`, `:18`, `:19`, `:20`, `:21` — 7 lines, `:3` is AI):

| Namespace | using anchor | Types referenced (anchors) | ROLE |
|---|---|---|---|
| `DualFrontier.AI.Pathfinding` | `:3` | `NavGrid` `:105`, `AStarPathfinding` `:113` | **scenario/nav + service construction** |
| `DualFrontier.Components.Items` | `:10` | `ConsumableComponent` `:259`, `WaterSourceComponent` `:260`, `BedComponent` `:261`, `DecorativeAuraComponent` `:262` | **seed/spawn event emission** |
| `DualFrontier.Components.Shared` | `:11` | `PositionComponent` `:124`, `:125`, `:274`, `:277`, `:278` | **seed/spawn** |
| `DualFrontier.Events.Combat` | `:18` | `DeathEvent` `:98` | **bridge subscription (presentation)** |
| `DualFrontier.Events.Pawn` | `:19` | `PawnSpawnedEvent` `:92`, `ItemSpawnedEvent` `:94`/`:275`, `PawnMovedEvent` `:96`, `PawnStateChangedEvent` `:100`, `ItemKind` `:259-262`/`:265`/`:280` | **bridge subscription + seed events** |
| `DualFrontier.Systems.Inventory` | `:20` | `InventorySystem` `:162`, `HaulSystem` `:163` | **system construction** |
| `DualFrontier.Systems.Pawn` | `:21` | `NeedsSystem` `:154`, `MoodSystem` `:155`, `JobSystem` `:156`, `ConsumeSystem` `:157`, `SleepSystem` `:158`, `ComfortAuraSystem` `:159`, `MovementSystem` `:160`, `PawnStateReporterSystem` `:161` | **system construction** |

Indirect game reach (Application-internal types that themselves pull the forbidden namespaces): `VanillaComponentRegistration` `:87`, `RandomPawnFactory` `:115`, `ItemFactory` `:128`.

**Role tally for GameBootstrap's own references**: component registration 1 site (`:87`, delegated) · system construction 10 sites · seed/spawn 6 sites · factories 2 · presentation-bridge subscription 5 · scenario consts 11.

### A.5 `CreateSession` call sites — complete

Command: `grep -rn "CreateSession" --include=*.cs . | grep -v '/obj/' | grep -v '/bin/'`

- **Production: exactly ONE** — `src/DualFrontier.Launcher/Program.cs:55`: `using EngineSession session = GameBootstrap.CreateSession(bridge);`
- **Tests: 18 invocation sites across 2 files**
  - `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — `:64, :81, :99, :120, :148, :187, :209, :233, :274, :326, :373, :391, :409, :428, :447, :467` (16), plus a reflection pin at `:168` (`nameof(GameBootstrap.CreateSession)`)
  - `tests/DualFrontier.Modding.Tests/Loop/EngineSessionTransactionTests.cs` — `:29` (harness), `:148`, `:167`, `:179`
- **Doc-comment mentions only** (no call): `Bridge/IPresentationSink.cs:5`, `Modding/ModRegistry.cs:68`, `Modding/ModMenuController.cs:52`, `tests/.../WeatherHarness.cs:52,:72,:81`, `tests/DualFrontier.Core.Tests/Scheduling/SchedulerStressTests.cs:61`.

---

## B. ENGINESESSION TODAY

**File**: `/mnt/Work/Colony_Simulator/src/DualFrontier.Application/Loop/EngineSession.cs` — **357 lines**.

### B.1 Anatomy

| Element | Anchor |
|---|---|
| `internal enum ShutdownStep { FencePassed, DeferredDropped, ModsUnloaded, NativeSchedulerCleared, NativeBusCleared, WorldDisposed, Aborted }` | `:17-26` |
| `internal sealed record ShutdownAbortReport(...)` + `ForBusyWorld` + `Describe()` | `:40-62` |
| `internal sealed class ShutdownTransactionHooks` (FenceDeadline / SimFenceOverride / PipelineQuiescentOverride / OnAbort / OnStep / WorldTeardownOverride) | `:70-94` |
| `internal sealed class EngineSession : IDisposable` | `:118` |
| `DefaultFenceDeadline = TimeSpan.FromSeconds(5)` | `:121` |
| Fields: `_world`, `_busBridge`, `_pipeline`, `_services`, `_loop`, `_controller`, `_hooks`, `_quiescence`, `_disposed` | `:125-133` |
| ctor `(NativeWorld, ManagedBusBridge, ModIntegrationPipeline, GameServices, GameLoop, ModMenuController, ShutdownTransactionHooks?)` | `:135-154` |
| `public GameLoop Loop => _loop;` | `:157` |
| `public ModMenuController Controller => _controller;` | `:160` |
| `_healthLock`, `_degradedReasons` | `:168-169` |
| `event Action<EngineHealthChanged>? HealthChanged` | `:175` |
| `EngineHealth Health { get; }` | `:181-192` |
| `void ReportDegraded(DegradedReason)` | `:201-221` |
| `void ClearDegradedForMod(string)` | `:231-257` |
| `void Dispose()` — the S1→S7 transaction | `:263-322` |
| `bool WaitPipelineQuiescent(TimeSpan)` | `:328-339` |
| `void Abort(ShutdownAbortReport)` | `:341-354` |
| `void Step(ShutdownStep)` | `:356` |

`Dispose` order: fence (`:274-284`) → `_services.DropDeferred()` (`:290`) → `_pipeline.UnloadAll()` (`:294`) → `SystemGraphInterop.Clear()` / `WakeRegistryInterop.Clear()` / `SchedulerAdapter.ClearCallback()` (`:298-300`) → `_busBridge.Shutdown()` (`:303`) → `_world.DisposeChecked()` (`:314`).

### B.2 Namespace references — **the W4 gate is ALREADY MET at the class level**

`EngineSession.cs` usings (`:1-7`): `System`, `System.Threading`, `DualFrontier.Application.Bus`, `DualFrontier.Application.Modding`, `DualFrontier.Application.Scheduler`, `DualFrontier.Core.Bus`, `DualFrontier.Core.Interop`.
**ZERO references to `DualFrontier.Components` / `.Events` / `.Systems` / `.AI`** — no usings, no fully-qualified names.
Command: `grep -n "DualFrontier\.\(Components\|Events\|Systems\|AI\)\b" src/DualFrontier.Application/Loop/EngineSession.cs` → no output.

Its class doc states the discipline explicitly (`:113-116`): *"Game-vocabulary-free by discipline (B-5): this class names only engine types. The vanilla-content knowledge lives solely in `GameBootstrap`, the sacrificial harness…"*

**Consequence for the W4 gate wording.** "EngineSession compiles with zero references to Components/Events/Systems/AI" is *already true today, as a file*. What is NOT true is the assembly-level fact: `DualFrontier.Application.csproj` still carries all 4 engine→game `ProjectReference` edges. The gate therefore needs an explicit interpretation — see §G.4.

### B.3 Relationship: who constructs whom

`GameBootstrap.CreateSession` (`:242`) constructs `EngineSession`. `EngineSession` constructs nothing except `SimulationStateController` (`:151-153`). It is a *receiver* of an already-assembled graph, not a composer. Launcher consumes it (`Program.cs:55`, `:66` `session.Loop.Start()`, `:106` `session.Dispose()`).

### B.4 The ratified D3 constraint (verbatim)

`tools/briefs/EQ_A2_SHUTDOWN_TRANSACTION_BRIEF.md:28-34`:

> - **D3.** `EngineSession : IDisposable` lives in `DualFrontier.Application` as the ENGINE
>   composition root per boundary law B-5: owns kernel/world, scheduler, bus, mod pipeline
>   lifecycle end-to-end; ZERO knowledge of game types (the ratchet enforces the assembly
>   edge; keep the class itself game-vocabulary-free). `Launcher` consumes it. `GameContext`
>   DIES (delete). `GameBootstrap` survives only as a thin SACRIFICIAL harness caller (the
>   2026-07-18 scaffolding ruling: harness gameplay logic may be freely cut, no equivalence
>   obligation to it -- equivalence binds ENGINE behavior only).

Companion `D8` (`:51-52`):
> - **D8.** The bootstrap bridge lambdas are harness-side and sacrificial: EngineSession's
>   disposal contract governs; no preservation machinery for harness wiring.

**NOTE — `docs/architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md` contains NO string "D3".** Command: `grep -n "D3" docs/architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md` → no output. ELT carries the *realization* notes only: `:161` (the transaction is `EngineSession.Dispose`) and `:241` (`EngineHealth` owned by `EngineSession`). The ratified D3 text lives in the EQ_A2 brief. Boundary law B-5 is `GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md:75-77`:

> - **B-5 Distribution composition.** Launcher/Application know only: kernel, SDK,
>   distribution manifest, mod pipeline. Switching the game means switching the manifest,
>   not forking the bootstrap.

### B.5 GameLoop — who owns the tick

`src/DualFrontier.Application/Loop/GameLoop.cs` — **158 lines**, `internal sealed class GameLoop : IDisposable` at `:27`.
- `TargetTps = 30f` `:29`; `FixedDelta = 1/30` `:30`; `MaxAccumulator = 5×` `:31`; `IdleSafetyMargin = 10%` `:36`.
- `Start()` `:62-70` — spawns a background `Thread` named `"SimulationLoop"`, `IsBackground = true`.
- `TryStop(TimeSpan) : bool` `:81-86` — the CHECKED fence EngineSession consumes (`_cts.Cancel()` then `Join(deadline)`).
- `Stop()` `:93` — legacy 2 s convenience.
- `SetPaused` `:96`, `IsPaused` `:104`, `SetSpeed` `:107-108`.
- `RunLoop()` `:110-155` — fixed-step: `_scheduler.ExecuteTick(FixedDelta)` `:131`, `_bridge.Enqueue(new TickAdvancedCommand(...))` `:132`, Background drain `:136-144`.
- `Dispose() => Stop()` `:157`.

Flow: Launcher starts (`Program.cs:66`), Launcher's render loop runs on the main thread (`Program.cs:75-97`), Launcher disposes the session which fences the loop (`Program.cs:106` → `EngineSession.cs:275,277`).

---

## C. WHAT MUST MOVE — engine-side game content census

### C.1 `src/DualFrontier.Systems/` — **27 systems**, 29 `.cs` files (2 are `Magic/Internal/` helpers)

Command: `grep -rn "class [A-Za-z]*System : SystemBase" --include=*.cs src/DualFrontier.Systems/ | wc -l` → `27`
Command: `grep -rln "BridgeImplementation" --include=*.cs src/DualFrontier.Systems/ | wc -l` → `17`

**REAL = the 10 registered by `GameBootstrap.cs:154-163`; STUB = the 17 carrying `[BridgeImplementation]`. 10 + 17 = 27, exact partition.**

| File | Class decl | `[TickRate]` | Update body | Class | Role (one line) |
|---|---|---|---|---|---|
| `Pawn/NeedsSystem.cs` (136) | `:29` | SLOW `:28` | 66 | **REAL** | decays needs, publishes `NeedsCriticalEvent` |
| `Pawn/MoodSystem.cs` (78) | `:18` | SLOW `:17` | 57 | **REAL** | derives mood from needs |
| `Pawn/JobSystem.cs` (136) | `:20` | NORMAL `:19` | 56 | **REAL** | assigns jobs from need pressure |
| `Pawn/ConsumeSystem.cs` (227) | `:39` | NORMAL `:38` | 187 | **REAL** | eat/drink resolution |
| `Pawn/SleepSystem.cs` (205) | `:33` | NORMAL `:32` | 168 | **REAL** | bed claim + sleep restore |
| `Pawn/ComfortAuraSystem.cs` (93) | `:45` | SLOW `:44` | 47 | **REAL** | decorative-aura comfort field |
| `Pawn/MovementSystem.cs` (210) | `:27` | NORMAL `:26` | 121 | **REAL** | A* path stepping (`ISystemServices.Pathfinding`) |
| `Pawn/PawnStateReporterSystem.cs` (152) | `:31` | SLOW `:30` | 116 | **REAL** | publishes `PawnStateChangedEvent` for HUD |
| `Inventory/InventorySystem.cs` (119) | `:24` | FAST `:23` | 80 | **REAL** | storage counts + reservations |
| `Inventory/HaulSystem.cs` (164) | `:35` | NORMAL `:34` | 124 | **REAL** | haul job → `ItemReservedEvent` |
| `Combat/CombatSystem.cs` (63) | `:31` | FAST `:29` | 25 | STUB (`:30` Phase 5, Replaceable) | shot intent skeleton + `[ReservedStub]` `:54` |
| `Combat/ComboResolutionSystem.cs` (77) | `:29` | NORMAL `:27` | 37 | STUB (`:28` Phase 5, Replaceable) | damage-intent queue skeleton |
| `Combat/CompositeResolutionSystem.cs` (146) | `:31` | FAST `:29` | 103 | STUB (`:30` Phase 5, Replaceable) | compound-shot composition |
| `Combat/DamageSystem.cs` (35) | `:24` | FAST `:22` | 5 | STUB (`:23` Phase 5, Replaceable) | empty `Update` + TODO |
| `Combat/ProjectileSystem.cs` (34) | `:23` | REALTIME `:21` | 5 | STUB (`:22` Phase 5, Replaceable) | empty `Update` + TODO |
| `Combat/StatusEffectSystem.cs` (34) | `:23` | FAST `:21` | 5 | STUB (`:22` Phase 5, Replaceable) | empty `Update` + TODO |
| `Faction/RaidSystem.cs` (32) | `:21` | RARE `:19` | 5 | STUB (`:20` Phase 7) | empty `Update` + TODO |
| `Faction/RelationSystem.cs` (34) | `:22` | RARE `:20` | 5 | STUB (`:21` Phase 7) | empty `Update` + TODO |
| `Faction/TradeSystem.cs` (33) | `:22` | RARE `:20` | 5 | STUB (`:21` Phase 7) | empty `Update` + TODO |
| `Inventory/CraftSystem.cs` (34) | `:23` | NORMAL `:21` | 5 | STUB (`:22` Phase 6) | empty `Update` + TODO |
| `Magic/EtherGrowthSystem.cs` (33) | `:22` | SLOW `:20` | 5 | STUB (`:21` Phase 6) | empty `Update` + TODO |
| `Magic/GolemSystem.cs` (77) | `:27` | NORMAL `:25` | 43 | STUB (`:26` Phase 6) | ownership-transfer skeleton, `[ReservedStub]` `:48`,`:67` |
| `Magic/ManaSystem.cs` (131) | `:28` | NORMAL `:26` | 92 | STUB (`:27` Phase 6) | mana intent/lease skeleton, 5 `[ReservedStub]` |
| `Magic/RitualSystem.cs` (34) | `:23` | RARE `:21` | 5 | STUB (`:22` Phase 6) | empty `Update` + TODO |
| `Magic/SpellSystem.cs` (35) | `:23` | FAST `:21` | 5 | STUB (`:22` Phase 6) | empty `Update` + TODO |
| `Pawn/SkillSystem.cs` (32) | `:21` | NORMAL `:19` | 5 | STUB (`:20` Phase 3) | empty `Update` + TODO |
| `World/MapSystem.cs` (32) | `:21` | RARE `:19` | 5 | STUB (`:20` Phase 7) | empty `Update` + TODO |

Non-system files in the project: `Magic/Internal/ManaLease.cs` (93, `:14`), `Magic/Internal/ManaLeaseRegistry.cs` (113, `:18`, 5 `[ReservedStub]`).

Repo-wide `: SystemBase` count in `src/` = **30** — the 27 above + `Application/Modding/SystemAdapter.cs:48` (`SystemAdapter<TSystem>`) + 2 doc-comment examples (`Contracts/Attributes/ModCapabilitiesAttribute.cs:20`, `Contracts/Scheduling/WakeOnSlotTransitionAttribute.cs:25`).
Command: `grep -rn ": SystemBase" --include=*.cs src/ | grep -v '/obj/'`

### C.2 `src/DualFrontier.Components/` — 27 files, **28 types** (23 `IComponent` structs + 5 enums)

Commands: `grep -rn "struct [A-Za-z]*Component : IComponent" --include=*.cs src/DualFrontier.Components/ | wc -l` → `23`; `grep -rn "public enum " ... | wc -l` → `5`

**Registered by `VanillaComponentRegistration.RegisterAll` (`src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs:35-86`, 87 lines) — 21 types, ids 1..21:**

| id | Type | Anchor |
|---|---|---|
| 1 | `HealthComponent` | `:40` |
| 2 | `PositionComponent` | `:41` |
| 3 | `RaceComponent` | `:42` |
| 4 | `NeedsComponent` | `:45` |
| 5 | `MindComponent` | `:46` |
| 6 | `JobComponent` | `:47` |
| 7 | `BedComponent` | `:51` |
| 8 | `ConsumableComponent` | `:52` |
| 9 | `DecorativeAuraComponent` | `:53` |
| 10 | `ReservationComponent` | `:54` |
| 11 | `WaterSourceComponent` | `:55` |
| 12 | `TileComponent` | `:57` |
| 13 | `EtherNodeComponent` | `:58` |
| 14 | `EtherComponent` | `:61` |
| 15 | `GolemBondComponent` | `:62` |
| 16 | `ManaComponent` | `:63` |
| 17 | `ArmorComponent` | `:66` |
| 18 | `IdentityComponent` | `:82` |
| 19 | `SkillsComponent` | `:83` |
| 20 | `MovementComponent` | `:84` |
| 21 | `StorageComponent` | `:85` |

**NOT registered (2 structs)**: `FactionComponent` (`Shared/FactionComponent.cs:11`), `WorkbenchComponent` (`Building/WorkbenchComponent.cs:12`).
**5 enums** (never registered — plain value types): `JobKind` (`Pawn/JobKind.cs:6`), `SkillKind` (`Pawn/SkillKind.cs:6`), `NeedKind` (`Pawn/NeedKind.cs:8`), `RaceKind` (`Shared/RaceComponent.cs:9`), `TerrainKind` (`World/TerrainKind.cs:8`).

**Registered by mods: ZERO vanilla mods register anything.** Only `mods/DualFrontier.Mod.Weather/WeatherMod.cs:19` (`WeatherStateComponent`) and `mods/DualFrontier.Mod.Example/ExampleMod.cs:16` (`ExampleComponent`).
Command: `grep -rn "RegisterComponent\|RegisterSystem" --include=*.cs mods/ | grep -v '/obj/'` → 5 hits, all Weather/Example.

### C.3 `src/DualFrontier.Events/` — 51 files, **51 public type declarations**

Command: `grep -rn -E "public (readonly )?(sealed )?(record|struct|class|enum) " --include=*.cs src/DualFrontier.Events/ | wc -l` → `51`

Breakdown: **45 `IEvent` records** + 2 `readonly record struct` id types (`Combat/TransactionId.cs:11`, `Magic/LeaseId.cs:11`) + 4 enums (`Pawn/ItemKind.cs:16`, `Combat/ShotRefusalReason.cs:8`, `Magic/CloseReason.cs:8`, `Magic/RefusalReason.cs:8`).

By folder: Combat 13 files, Inventory 4, Magic 16, Pawn 16, World 2.

Events consumed by the engine composition today (`GameBootstrap.cs:92-103`): `PawnSpawnedEvent`, `ItemSpawnedEvent`, `PawnMovedEvent`, `DeathEvent`, `PawnStateChangedEvent` — 5 of 45.

### C.4 `src/DualFrontier.AI/` — 17 files, **19 public types**

Commands: `find src/DualFrontier.AI -name '*.cs' -not -path '*/obj/*' | wc -l` → `17`; public-type grep → `19`

| Sub-tree | Types | Consumed? |
|---|---|---|
| `Pathfinding/` | `NavGrid` (`NavGrid.cs:10`), `AStarPathfinding : IPathfindingService` (`AStarPathfinding.cs:13`) | **YES** — `GameBootstrap.cs:105,113`, `ItemFactory.cs:3`, `RandomPawnFactory.cs:3` |
| `BehaviourTree/` | `BTStatus` `:6`, `BTNode` `:24` (BTNode.cs), `BTBlackboard` `:7`, `BTContext` `:10`, `Leaf` `:10`, `Selector` `:10`, `Sequence` `:11`, `IdleLeaf` `:9`, `IsExhaustedLeaf` `:13`, `IsHungryLeaf` `:16` | **NO consumers** anywhere |
| `Jobs/` | `JobStatus` `:7`, `IJob` `:22` (IJob.cs), `JobCast` `:12`, `JobCraft` `:8`, `JobGolemCommand` `:8`, `JobHaul` `:8`, `JobMeditate` `:9` | **NO consumers** anywhere |

Command: `grep -rn "DualFrontier\.AI" --include=*.cs src/ tests/ mods/ | grep -v '/obj/' | grep -v '^src/DualFrontier.AI/'` → 8 hits: 3 real usings (all Pathfinding), 2 doc comments in `Contracts/Services/IPathfindingService.cs:14,19`, 3 in `BoundaryRatchetTests.cs:51,60`.

**BD-5 "only Pathfinding is consumed" — CONFIRMED, and stronger: BT and Jobs have zero consumers in src/, tests/, and mods/.**

### C.5 Seeds / factories / initial spawn — where world content comes from today

Production entity-minting sites in `src/` (command: `grep -rn "\.CreateEntity()\|AddComponents<" --include=*.cs src/ | grep -v '/obj/'`):

| Site | Anchor | What |
|---|---|---|
| `Scenario/RandomPawnFactory.cs` (169 lines) | `:112` `CreateEntity()`; `:148-154` seven `AddComponents<T>` (Position, Identity, Needs, Mind, Job, Skills, Movement); `:159` `PawnSpawnedEvent` publish | 50 pawns, seed 42, positions from `NavGrid` passable tiles |
| `Scenario/ItemFactory.cs` (152 lines) | `:144` `CreateEntity()`; `:149-150` `AddComponents<PositionComponent>` + `AddComponents<T>` | 150 food / 50 water / 30 beds / 25 decorations, seed 43, pawn tiles excluded |
| `Modding/SystemContextView.cs` | `:51` `World.CreateEntity()` | the SDK route (mods only) |
| `Core.Interop/NativeWorld.cs` | `:256` | the primitive itself |

Both factories are `internal sealed` in `DualFrontier.Application.Scenario` and IVT'd to `DualFrontier.Core.Benchmarks` (`DualFrontier.Application.csproj`, the K7 comment block).

`GameBootstrap.PublishItemSpawnedEvents` (`:257-283`) then walks four component spans and emits one `ItemSpawnedEvent` per item so `ItemLayer` can render it.

**RNG**: 4 fixed seeds — `ObstacleSeed = 42` (`GameBootstrap.cs:69`, used `:106`), `FactorySeed = 42` (`:68`, used `:115`), `ItemFactorySeed = 43` (`:75`, used `:128`), plus `MovementSystem.cs:35` target-typed `new(42)`. This is ROADMAP **F-42, OPEN** ("No RNG service"), whose ledger anchor `GameBootstrap.cs:96` is stale (actual `:106`).

### C.6 The mods/ inventory — 9 discoverable mods, 6 "vanilla" + Example + Weather pair

Command: `find mods -name 'mod.manifest.json' -not -path '*/obj/*'` → 8 manifests (the 9th discovered id is the Weather pair's second member; the discoverer test pins **9**, see below).

| Mod dir | `id` | `kind` | Entry | Owns today | Code files |
|---|---|---|---|---|---|
| `DualFrontier.Mod.Vanilla.Core` | `dualfrontier.vanilla.core` | **shared** | `""` / `""` | **NOTHING** — csproj + manifest + README only | 0 `.cs` |
| `DualFrontier.Mod.Vanilla.World` | `dualfrontier.vanilla.world` | regular | `WorldMod` | empty `Initialize`/`Unload` | `WorldMod.cs` (32) |
| `DualFrontier.Mod.Vanilla.Pawn` | `dualfrontier.vanilla.pawn` | regular | `PawnMod` | empty (`PawnMod.cs:22-25` TODO) | `PawnMod.cs` (33) |
| `DualFrontier.Mod.Vanilla.Inventory` | `dualfrontier.vanilla.inventory` | regular | `InventoryMod` | empty | `InventoryMod.cs` (33) |
| `DualFrontier.Mod.Vanilla.Combat` | `dualfrontier.vanilla.combat` | regular | `CombatMod` | empty | `CombatMod.cs` (33) |
| `DualFrontier.Mod.Vanilla.Magic` | `dualfrontier.vanilla.magic` | regular | `MagicMod` | empty | `MagicMod.cs` (32) |
| `DualFrontier.Mod.Example` | `dualfrontier.example` | regular (default) | `ExampleMod` | 1 component + 1 SDK system | 3 `.cs` (108) |
| `DualFrontier.Mod.Weather.Contracts` | `dualfrontier.weather.contracts` | **shared** | `""` / `""` | `WeatherKind`, `WeatherChangedEvent` | 2 `.cs` (63) |
| `DualFrontier.Mod.Weather` | `dualfrontier.weather` | regular | `WeatherMod` | 1 component + 2 SDK systems + 1 seeded singleton | 4 `.cs` (344) |

All five regular vanilla mods depend on `dualfrontier.vanilla.core ^0.1.0`; all declare `apiVersion: "^2.0.0"`, `hotReload: true`, empty `replaces` and empty `capabilities`.

**Does any vanilla mod do component registration / seeding? NO.** Only Weather (registration `WeatherMod.cs:19-21`; seeding `WeatherSystem.cs:119-130`) and Example (`ExampleMod.cs:16-17`).

Solution enrollment: all 8 mod projects are in `DualFrontier.sln` (`:40` Example, `:108` Vanilla.Core, `:113` World, `:118` Pawn, `:123` Inventory, `:128` Combat, `:133` Magic, `:138` Weather.Contracts, `:140` Weather).

---

## D. THE MOD LIFECYCLE SURFACE

### D.1 `IMod` — `src/DualFrontier.Contracts/Modding/IMod.cs` (28 lines)

```
:19  void Initialize(IModApi api);
:27  void Unload();
```
Two members. `Unload()` is **parameterless** — no world handle (this is ROADMAP **F-58 / G3**).

### D.2 `IModApi` — `src/DualFrontier.Contracts/Modding/IModApi.cs` (128 lines)

| Member | Anchor |
|---|---|
| `void RegisterComponent<T>() where T : unmanaged, IComponent` | `:38` |
| `void RegisterManagedComponent<T>() where T : class, IComponent` | `:59` |
| `void RegisterSystem<T>() where T : class` | `:66` |
| `void Publish<T>(T evt) where T : IEvent` | `:73` |
| `void Subscribe<T>(Action<T> handler) where T : IEvent` | `:79` |
| `void PublishContract<T>(T) where T : IModContract` | `:86` |
| `bool TryGetContract<T>(out T?) where T : class, IModContract` | `:94` |
| `IReadOnlySet<string> GetKernelCapabilities()` | `:101` |
| `ModManifest GetOwnManifest()` | `:106` |
| `void Log(ModLogLevel, string)` | `:111` |
| `IModFieldApi? Fields { get; }` | `:118` |
| `IModComputePipelineApi? ComputePipelines { get; }` | `:127` |

**`IModApi` exposes NO world access at all** — no `CreateEntity`, no component read/write. A mod therefore **cannot seed the world from `IMod.Initialize`**.

### D.3 `ISimulationSystem` — `src/DualFrontier.Contracts/Sdk/ISimulationSystem.cs` (59 lines)

```
:44  void Initialize(ISystemContext context);
:51  void Tick(ISystemContext context);
:58  void OnDispose();
```
`OnDispose()` parameterless (F-58 again). `Initialize` **does receive a live context**.

### D.4 `ISystemContext` — `src/DualFrontier.Contracts/Sdk/ISystemContext.cs` (210 lines)

| Group | Members (anchors) |
|---|---|
| Time | `long CurrentTick` `:44` |
| Entity lifecycle | `EntityId CreateEntity()` `:76`, `void DestroyEntity(EntityId)` `:102`, `bool IsEntityAlive(EntityId)` `:111` |
| Component per-id | `TryGetComponent<T>` `:116`, `HasComponent<T>` `:119`, `GetComponent<T>` `:122` |
| Component bulk | `SpanScope<T> AcquireSpan<T>()` `:131`, `WriteScope<T> BeginBatch<T>()` `:138` |
| Strings | `StringHandle InternString(string)` `:143`, `string? Resolve(StringHandle)` `:146` |
| Composites | `CreateComposite<T>` `:151`, `CompositeAdd<T>` `:154`, `CompositeTryGetAt<T>` `:157`, `CompositeCountFor<T>` `:160`, `CompositeClearFor<T>` `:163` |
| Presentation | `void SetAmbientTint(float,float,float,float)` `:193` |
| Events | `Publish<T>` `:202`, `Subscribe<T>` `:209` |

**18 members.** Explicit day-one omissions documented `:24-32`: no field/compute surface, no managed-store accessor, no services (services arrive at construction via `ISystemServices`).

`ISystemServices` (`src/DualFrontier.Contracts/Sdk/ISystemServices.cs`, 34 lines) has exactly **one** member: `IPathfindingService Pathfinding { get; }` `:33`.

### D.5 Pipeline `Apply` phases

Two distinct phase vocabularies exist and must not be conflated:

**(a) `ModIntegrationPipeline.Apply` numbered steps** (`src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`, 1509 lines; `Apply` at `:228-575`):

| Step | Anchor | What |
|---|---|---|
| `[-1]` | `:235-247` | drain `ModFaultHandler` queue → `UnloadMod` each faulted mod |
| `[0]` | `:249-287` | classify paths; parse manifests; split by `ModKind` |
| `[0.5]` | `:289-302` | shared-mod cycle detection (`TopoSortSharedMods`) |
| `[0.6]` | `:310-337` | regular-mod topo sort + dependency presence check |
| `[1]` | `:339-376` | pass 1 — shared mods into the shared ALC; `RegisterOwner` |
| `[2]` | `:378-401` | pass 2 — regular mods, each into its own ALC; `RegisterRegularOwner` |
| `[3]` | `:403-431` | `ContractValidator.Validate` (the lettered phases) → rollback on failure |
| `[4]` | `:433-501` | `new RestrictedModApi` per mod; `RegisterRestrictedModApi` `:449`; **`mod.Instance.Initialize(api)` `:452`**; `RegisterModComponents` `:472` (ID-A eager id allocation) |
| `[5-7]` | `:503-552` | build `DependencyGraph` locally, honouring `replaces` skips |
| `[8]` | `:554-574` | `_activeMods.AddRange`; `SystemMetadataBuilder.Build`; `_scheduler.Rebuild(...)` `:565` |

**(b) `ContractValidator` lettered phases A..H** (`src/DualFrontier.Application/Modding/ContractValidator.cs`, 837 lines; `Validate` at `:77`):

| Phase | Doc anchor | What |
|---|---|---|
| A | `:111` | kernel/contracts API version compatibility |
| B | `:16` (class doc) | inspects every mod system's declarations |
| C | `:395` | every `capabilities.required` token must be kernel-provided |
| D | `:483` | each mod system's `[SystemAccess]` token cross-check |
| E | `:321` | regular-mod assemblies must not export `IEvent`-implementing types (shared-only, MOD_OS §5/§6.5 D-4) |
| F | `:748` | shared-mod compliance (no `IMod`, no mutable state) |
| G | `:542` | inter-mod dependency version constraints |
| H | `:605` | `replaces` bridge-replacement validation |

`:42` records: *"Phases A, B, E, G and H run unconditionally"* (C/D are skipped when `kernelCapabilities` is null — the F15 fix passes it at `ModIntegrationPipeline.cs:413`).

### D.6 **Is there a once-at-session-start world-seeding hook? — Answer: PARTIALLY, and not by that name**

- `IMod.Initialize(IModApi)` — **no world access** (D.2). Not a seeding hook.
- `IMod.Unload()` / `ISimulationSystem.OnDispose()` — parameterless, **no world access** (F-58 OPEN).
- `ISimulationSystem.Initialize(ISystemContext)` — **DOES have full world access.** `SystemAdapter<TSystem>.OnInitialize()` (`src/DualFrontier.Application/Modding/SystemAdapter.cs:72`) forwards to `_system.Initialize(_view)`, and `ParallelSystemScheduler.InitializeAllSystems()` (`src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:137-155`) pushes a live `SystemExecutionContext` around every `system.Initialize()` call (`:144-152`), precisely so world/bus access works. `SystemContextView.World` (`SystemContextView.cs:41-45`) reads `SystemExecutionContext.Current`, so the seam is live.
- **BUT** `InitializeAllSystems` is called from BOTH the scheduler ctor (`:126`) and `Rebuild` (`:269`). Every `pipeline.Apply` triggers a `Rebuild` (`ModIntegrationPipeline.cs:565`), so `Initialize` re-runs on **every mod-set change**, not once per session. It is a "once per graph build" hook, not a "once per world" hook. Any W4 seeding design must handle re-entry.

### D.7 What the Weather pair actually used to seed its singleton

**It did NOT use `Initialize`.** `mods/DualFrontier.Mod.Weather/WeatherSystem.cs`:
- `Initialize(ISystemContext) { }` — empty, `:42-44`.
- `Tick` `:47-82`: `if (!TryReadSingleton(context, out entity, out state)) { SeedSingleton(context); return; }` (`:49-53`).
- `TryReadSingleton` `:94-111` — `AcquireSpan<WeatherStateComponent>()`, take the first pair, **release the span before returning** (span must be closed before minting).
- `SeedSingleton` `:119-130` — `context.CreateEntity()` `:121`, then `BeginBatch<WeatherStateComponent>()` + `batch.Add(...)` `:123-129`.

So the shipped pattern is **lazy self-seed on first tick, idempotent by read-then-mint**. `WeatherMod.Initialize` (`WeatherMod.cs:17-22`) does registration only. `WeatherMod.Unload()` (`:29-31`) is empty by necessity — F-58.

**F-58 is pinned positively as a measured residue**: `tests/DualFrontier.Modding.Tests/Weather/WeatherWaveGateTests.cs:131-133` asserts `EntityCount == 1` after unload, with the comment *"EXPECTED residue (G3): OnDispose is parameterless, so a mod has no way to reach the world and clean up"*.

---

## E. MANIFEST SURFACE TODAY

### E.1 `mod.manifest.json` schema

`ModManifest` — `src/DualFrontier.Contracts/Modding/ModManifest.cs` (149 lines):

| Property | Anchor | Default | Required in JSON |
|---|---|---|---|
| `ManifestVersion` | `:50` | `"3"` | **YES**, exact `"3"` |
| `Id` | `:56` | `""` | **YES** |
| `Name` | `:61` | `""` | **YES** |
| `Version` | `:67` | `"0.0.0"` | **YES** |
| `Author` | `:72` | `""` | no |
| `RequiresContractsVersion` | `:83` | `"2.0.0"` | no |
| `EntryAssembly` | `:90` | `""` | no |
| `EntryType` | `:97` | `""` | no |
| `Dependencies` | `:103` | empty | no |
| `Kind` (`ModKind.Regular\|Shared`, enum `:7-17`) | `:110` | `Regular` | no |
| `ApiVersion` (`VersionConstraint?`) | `:117` | `null` | no |
| `HotReload` | `:124` | `false` | no |
| `Replaces` | `:133` | empty | no |
| `Capabilities` (`ManifestCapabilities`) | `:140` | `Empty` | no |
| `EffectiveApiVersion` (derived) | `:147-148` | — | — (**F-35 OPEN: zero call sites in `src/`**) |

`ManifestParser` — `src/DualFrontier.Application/Modding/ManifestParser.cs` (413 lines), `Parse` `:26-95`:
- required: `manifestVersion` `:53` (strict `== "3"` gate `:54-59`), `id` `:61`, `name` `:62`, `version` `:63`
- optional: `author` `:65`, `requiresContractsVersion` `:66`, `entryAssembly` `:67`, `entryType` `:68`, `kind` `:70`/`:169`, `apiVersion` `:71`/`:194`, `hotReload` `:72`, `replaces` `:73`/`:224`, `dependencies` `:74`/`:254` (entry keys `id` `:307`, `version` `:321`, `optional` `:346`), `capabilities` `:75`/`:363`
- key lookup is case-insensitive (`TryGetPropertyCi` `:98-110`).

Live example — `mods/DualFrontier.Mod.Weather/mod.manifest.json` (27 lines) is the richest: id, name, version, author, description, kind, apiVersion, entryAssembly, entryType, hotReload, dependencies, replaces, capabilities.required (2 owner-namespaced tokens `mod.dualfrontier.weather.contracts.publish:…` / `.subscribe:…`).

Note: `description` is present in 3 manifests but is **not** a `ModManifest` property and is silently ignored by `ManifestParser`.

### E.2 `DefaultModDiscoverer` — `src/DualFrontier.Application/Modding/DefaultModDiscoverer.cs` (57 lines)

- ctor takes a single `rootPath` string, unvalidated (`:28-31`).
- `Discover()` `:33-56`: returns empty if the dir does not exist (`:35-36`); `Directory.EnumerateDirectories(_rootPath)` (`:39`) — **one level deep only**; requires `mod.manifest.json` in each subdir (`:41-43`); per-mod parse failures are **silently swallowed** (`:49-53`).
- **Ordering is `Directory.EnumerateDirectories` order — filesystem-defined, not sorted, not manifest-driven.** No priority, no load-order field anywhere.
- Discovery is decoupled from load order: `ModIntegrationPipeline` does its own topological sort at `[0.5]`/`[0.6]`.

Production root is the literal `"mods"` (`GameBootstrap.cs:79`), pinned by `GameBootstrapIntegrationTests.cs:158-171`.
Production discovery pin: `GameBootstrapIntegrationTests.cs:482-537` — **9 mods** (`:503`), ids enumerated `:508-516`.

### E.3 Distribution / game-level config surface — **NONE EXISTS**

Command: `grep -rn "game\.manifest\|gameManifest\|GameManifest\|DistributionManifest\|distribution manifest" --include=*.cs --include=*.json --include=*.csproj . | grep -v '/obj/' | grep -v '/bin/'` → **zero hits**.
Command: `find . -maxdepth 2 -name '*.json' -not -path './.git/*' -not -path './.vs/*'` → `global.json`, `CppProperties.json`, `.claude/settings*.json`, `.vscode/settings.json` — all toolchain, none game config.

EAM R13 verbatim (`docs/architecture/EXECUTION_AUTHORITY_MATRIX.md:90`):

> | R13 | Configuration constants | **TBD — ownerless today**; every tunable is hardcoded | Scattered consts (see note below) | None | Per-subsystem config files introduced piecemeal without a named owner |

EAM `:102` (row note):

> - **R13 (vacant).** The inventory: map 200×200 + seeds + item counts (`GameBootstrap.cs:58-68`), `TargetTps = 30f` (`GameLoop.cs:29`), MaxDoP = N−2 (`ParallelSystemScheduler.cs:90`), priorityClass 2 / wakeType 0 / Timer rate 1 (`GameBootstrap.cs:170-179`), window 1280×720 (`Program.cs:36-37`). No config file or loader exists anywhere in the repo.

EAM `:92` reading rule: *"A row marked TBD/ownerless is **vacant** and demands an ownership decision before any feature lands in it (R12, R13)."*

The **one loader-shaped thing that exists** is `ScenarioLoader` (`src/DualFrontier.Application/Scenario/ScenarioLoader.cs`, 50 lines) — `Load(string path)` `:24-35` (System.Text.Json) and `LoadDefault()` `:41-49`. It has **zero callers** (see §F). So "no config file or loader exists" is true of *live* config; a dead loader does exist.

### E.4 What the Launcher hardcodes (BD-4 candidates)

`src/DualFrontier.Launcher/Program.cs` (110 lines):

| Value | Anchor |
|---|---|
| `Title = "Dual Frontier"` | `:35` |
| `Width = 1280` | `:36` |
| `Height = 720` | `:37` |
| `AssetsDirectory = "assets"` | `:39` |
| `EnableValidationLayer` — omitted, uses the `#if DEBUG` default | `:40-41` |
| mods root — implicit `"mods"` via `CreateSession(bridge)` | `:55` |
| atlas — `LauncherProceduralAtlas.GenerateAtlas()` (procedural, no asset path) | `:48` |

`RuntimeOptions` (`src/DualFrontier.Runtime/RuntimeOptions.cs`, 31 lines): `Window` (default `new()`), `EnableValidationLayer` (`#if DEBUG` true / else false), `AssetsDirectory = "assets"`.
`WindowOptions` (`src/DualFrontier.Runtime/Window/WindowOptions.cs`, 13 lines): `Title = "Dual Frontier"` `:9`, `Width = 1280` `:10`, `Height = 720` `:11`, `Resizable = true` `:12` (**F-68 OPEN — dead property, no backend reads it**).

There is **no `LauncherOptions` type** anywhere. Command: `find . -name 'LauncherOptions*'` → none.

`assets/` top-level: `cinzel`, `kenney*` (8 packs), `scenes`, `shaders`, `sprites`. `assets/scenes/` holds `README.md` + `sample.dfscene`.

---

## F. ORPHAN VERIFICATION (BD-8)

### F.1 `ScenarioDef` — **ORPHAN, and its loader is orphan too**

`src/DualFrontier.Application/Scenario/ScenarioDef.cs` — **31 lines**, `public sealed class ScenarioDef` `:9`. Full property set:

```
:12  string  Id                = "default"
:15  string  Name              = "Default Colony"
:18  int     StartingPawnCount = 3
:21  int     WorldSeed         = 0
:24  int     MapWidth          = 100
:27  int     MapHeight         = 100
:30  Dictionary<string,int> StartingItems = new()
```

Command: `grep -rn "ScenarioDef\|ScenarioLoader" --include=*.cs . | grep -v '/obj/' | grep -v '/bin/'` → **7 hits, ALL inside the two files themselves**:
`ScenarioLoader.cs:8` (doc), `:10` (class decl), `:24` (return type), `:31` (`Deserialize<ScenarioDef>`), `:41` (`LoadDefault`), `ScenarioDef.cs:7` (doc), `:9` (class decl).

**Zero production readers. Zero test readers. `ScenarioLoader` (50 lines) is itself never constructed anywhere.** Both directions proven: nothing reads `ScenarioDef`, and nothing calls `ScenarioLoader`.

Values disagree with the live path, confirming it is not the source: `ScenarioDef` says 3 pawns / 100×100; `GameBootstrap` says 50 pawns / 200×200.

Stale doc rider: `src/DualFrontier.Application/Scenario/README.md:33` still marks `ScenarioDef` "Phase 3 [x]" as delivered work.

### F.2 `SceneMetadata` — **ORPHAN in production; one test-only reader via `SceneDef`**

`src/DualFrontier.Application/Scene/SceneMetadata.cs` — **14 lines**:

```
:9   public sealed record SceneMetadata(
:10      string         Biome,
:11      float          EtherDensity,
:12      string         CreatedBy,
:13      DateTimeOffset ExportedAt
:14   );
```

Command: `grep -rn "SceneMetadata\|EtherDensity" --include=*.cs . | grep -v '/obj/' | grep -v '/bin/'` → 4 hits:
- `src/DualFrontier.Application/Scene/SceneDef.cs:17` — a positional field of `SceneDef` (25 lines)
- `src/DualFrontier.Application/Scene/SceneMetadata.cs:9`, `:11` — the declaration
- `tests/DualFrontier.Core.Tests/Scene/SceneDefSerializationTests.cs:46` — the only construction site, a test

`SceneDef` itself: command `grep -rn "SceneDef\|ISceneLoader\|TilemapDef\|EntitySpawnDef\|MarkerDef\|SceneLoadException\|UshortArrayBase64" --include=*.cs . | grep -v Scene/ folder` → hits ONLY in `tests/DualFrontier.Core.Tests/Scene/SceneDefSerializationTests.cs` (`:9,:18,:24,:27,:38,:40,:41,:43,:44,:45,:51`).

**So the whole `src/DualFrontier.Application/Scene/` folder — 9 files, 214 lines** (`SceneDef.cs` 25, `SceneMetadata.cs` 14, `TilemapDef.cs` 31, `EntitySpawnDef.cs` 22, `MarkerDef.cs` 9, `ISceneLoader.cs` 25, `SceneLoadException.cs` 13, `UshortArrayBase64JsonConverter.cs` 51, plus data `assets/scenes/sample.dfscene`) **— has ZERO production consumers.** Its only consumer is one serialization round-trip test. `SceneMetadata`'s own doc says so (`:7`): *"Not used by the simulation — consumed by the UI and debugging tools"* — but no UI or debugging tool reads it either.

### F.3 GameBootstrap consts — see §A.2 (11 consts, `:65-75`)

### F.4 Other scenario/config-smelling constants in `src/`

| Constant | Anchor | Note |
|---|---|---|
| `GameLoop.TargetTps = 30f` | `GameLoop.cs:29` | EAM R13 inventory item |
| `GameLoop.MaxAccumulator = FixedDelta * 5f` | `GameLoop.cs:31` | |
| `GameLoop.IdleSafetyMargin = FixedDeltaMicros / 10` | `GameLoop.cs:36` | |
| `MaxDegreeOfParallelism = max(1, ProcessorCount - 2)` | `ParallelSystemScheduler.cs:112` | **EAM cites `:90` — stale, actual `:112`** |
| `EngineSession.DefaultFenceDeadline = 5 s` | `EngineSession.cs:121` | |
| `QuiescencePollInterval = 10 ms` | `EngineSession.cs:123` | |
| `Step7TimeoutMs = 10_000` / `Step7PollIntervalMs = 100` | `ModIntegrationPipeline.cs:135-136` | |
| `MovementSystem` RNG `new(42)` (target-typed) | `MovementSystem.cs:35` | F-42; grep-resistant |
| `WeatherSystem.TransitionPeriodTicks = 300`, `Seed`, `KindCount`, `MinimumIntensity` | `WeatherSystem.cs:27,30,32,39` | mod-side, correctly owned |
| Window 1280×720 / title / assets dir | `Program.cs:35-39`; `WindowOptions.cs:9-12`; `RuntimeOptions.cs:29` | BD-4 candidates |

Command: `grep -rn "StartingPawn\|EtherDensity" --include=*.cs . | grep -v '/obj/'` → only the two orphan declarations plus the one test.

---

## G. THE RATCHET + GATE MECHANICS

### G.1 `BoundaryRatchetTests` — the W0 ratchet

`tests/DualFrontier.Governance.Tests/BoundaryRatchetTests.cs` — **128 lines**.

| Element | Anchor | Value pinned |
|---|---|---|
| `EngineAssemblies[]` | `:34-42` | Contracts, Core, Core.Interop, Runtime, Application, Launcher (**6**) |
| `GameAssemblies` set | `:46-52` | Components, Events, Systems, AI (**4**), ordinal exact-name |
| `ProjectReferenceBaseline` | `:55-61` | **4** edges, all `DualFrontier.Application -> {Components, Events, Systems, AI}` |
| `InternalsVisibleToBaseline` | `:63-66` | **1**: `DualFrontier.Core -> DualFrontier.Systems` |
| Test 1 `EngineToGame_ProjectReferenceEdges_EqualFrozenBaseline` | `:68-79` | `BeEquivalentTo` — equality, so removal reddens too |
| Test 2 `EngineToGame_InternalsVisibleTo_EqualFrozenBaseline` | `:81-92` | same |
| Mechanism `MeasureEngineToGameEdges` | `:94-115` | parses the **six engine csprojs as XML**, filesystem read, no compilation |

Deletion trigger recorded `:23-25`: *"superseded by the B-6 boundary analyzer rule when one ships. This class is DELETED in the same cascade that lands that analyzer."*
Shrink protocol `:27-29`: *"the baseline constants below may only SHRINK. Shrinkage = update the constant + a census-delta note in the removing commit's body. Growth is forbidden."*

**Critical mechanical property**: the ratchet measures **csproj `ProjectReference`/`InternalsVisibleTo` XML**, not source-level `using` directives and not compilation. It cannot see whether `EngineSession.cs` names a game type; it can only see whether `DualFrontier.Application.csproj` names a game project.

### G.2 The second ratchet — `ExampleModReferenceRatchetTests`

`tests/DualFrontier.Modding.Tests/Sdk/ExampleModReferenceRatchetTests.cs` — **50 lines**. `ExampleMod_ProjectReferences_AreContractsOnly` `:21-38` pins `mods/DualFrontier.Mod.Example/DualFrontier.Mod.Example.csproj` reference set to exactly `{ DualFrontier.Contracts }` (`:35`). Same XML-parse mechanism. This is the shape a "mod is Contracts-only" gate takes.

### G.3 `CensusMetaTests` — `tests/DualFrontier.Analyzers.Tests/CensusMetaTests.cs` (214 lines)

Pins that W4 deletions will move (each requires a same-commit census-delta per RESERVED_SURFACE_MUTABILITY §5):

| Pin | Anchor | Current value |
|---|---|---|
| reserved-surface sites / files | `:107-108` | 34 / 13 |
| `stub` | `:112` | 51 / 20 |
| `deferred` | `:113` | 87 / 54 |
| `TODO` | `:114` | 132 / 51 |
| `not yet` | `:115` | 10 / 9 |
| `Phase 6` | `:182-183` | 23 / 11 |
| DFK-WAIVER | `:203-204` | 3 |
| `[SuppressMessage]` in src | `:212` | 0 |

**Census trap (carried from EQ_A4 memory, re-confirmed):** `CensusMetaTests` scans **git-TRACKED** `src/*.cs` — `:55` asserts `git ls-files` is non-empty. `git ls-files 'src/**/*.cs' | wc -l` → **421**. Deleting orphan files changes both site and file pins.

`Console.WriteLine` census in `src/` = **2** (`RestrictedModApi.cs:251`, `:289`) — the HARD pin cited by the EQ_A2 brief `:79` and by `EngineSession.cs:351-352`.
Command: `grep -rn "Console.Write" --include=*.cs src/ | grep -v '/obj/'`

### G.4 The project graph, and what shape the W4 gate can take

Command: per-csproj `ProjectReference`/`InternalsVisibleTo` extraction over `src/*.csproj`.

```
Contracts          -> (none)                       IVT: Application
Core.Interop       -> Contracts                    IVT: Core.Benchmarks, Core.Interop.Tests, Application, Core.Tests
Core               -> Contracts, Core.Interop      IVT: Systems, Application, Core.Tests, Modding.Tests, Systems.Tests, Core.Benchmarks
Runtime            -> Core.Interop                 IVT: Runtime.Tests
Components         -> Contracts, Core.Interop
Events             -> Contracts, Components
AI                 -> Contracts, Components
Systems            -> Contracts, Core, Components, Events, AI
Application        -> Contracts, Core, Components, Core.Interop, Events, Systems, AI     IVT: Modding.Tests, Launcher, Core.Benchmarks
Launcher           -> Application, Runtime         IVT: Runtime.Tests
Persistence        -> Contracts, Components        (no production consumer)
Crypto.Future      -> (none)                       (no production consumer)
```

**The forbidden-namespace footprint inside `DualFrontier.Application` is exactly 6 FILES / 24 using-sites / 0 fully-qualified references.**
Command: `grep -rn "DualFrontier\.\(Components\|Events\|Systems\|AI\)\b" --include=*.cs src/DualFrontier.Application/ | grep -v '/obj/'` → 24 lines, 6 files:

| File | Sites | Anchors | What it does |
|---|---|---|---|
| `Loop/GameBootstrap.cs` | 7 | `:3,:10,:11,:18,:19,:20,:21` | the whole composition |
| `Bootstrap/VanillaComponentRegistration.cs` | 7 | `:1-7` | 21 component registrations |
| `Scenario/RandomPawnFactory.cs` | 4 | `:3,:4,:5,:10` | pawn seeding |
| `Scenario/ItemFactory.cs` | 4 | `:3,:4,:5,:6` | item seeding |
| `Bridge/Commands/PawnStateCommand.cs` | 1 | `:2` (`Components.Pawn` for `SkillKind`) | render-command record |
| `Bridge/Commands/ItemSpawnedCommand.cs` | 1 | `:2` (`Events.Pawn` for `ItemKind`) | render-command record |

**Gate-shape analysis (three options, all measurable today):**

1. **Ratchet-test on using-directives.** Cheapest, and there is no precedent for it — both existing ratchets parse csproj XML, not source. A new source-scanning test would be a *new instrument class* (it would need the `git ls-files`-style scan `CensusMetaTests` already uses). It could be scoped to `Loop/EngineSession.cs` alone, but that assertion is **already green today** (§B.2) — it would pin, not prove.
2. **Analyzer rule (B-6).** `GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md:78-82` names this the endpoint, and `BoundaryRatchetTests.cs:23-25` records that the ratchet is DELETED in the cascade that lands it. There is an analyzer project (`tests/DualFrontier.Analyzers.Tests`, DFK/DFL rule family) but no boundary rule exists yet.
3. **Compile-time project split.** Moving `EngineSession` + `GameLoop` + the pipeline into a project that does not reference the four is the only *mechanical, compiler-enforced* form. Given the footprint above, this requires relocating or deleting all 6 files, since the split project cannot carry them.

**Does the gate need a new csproj? Measured answer: NO, if the 6 files leave `Application`.** The 4 `ProjectReference` edges live in `DualFrontier.Application.csproj:11,14,15,16` and can be deleted outright once those 6 files stop naming game types. `EngineSession`, `GameLoop`, `ModIntegrationPipeline`, `ModRegistry`, `ContractValidator`, `SystemContextView`, `SystemAdapter`, the Bus and Scheduler folders — none of them reference the four namespaces. Removing the 4 edges shrinks `ProjectReferenceBaseline` (`BoundaryRatchetTests.cs:55-61`) from 4 to 0, which the ratchet already permits ("may only SHRINK") and which makes the gate compiler-enforced without a new project.

Residual after that: the `Core -> Systems` IVT (`DualFrontier.Core.csproj`, pinned `BoundaryRatchetTests.cs:65`), and `Systems -> AI` / `Systems -> Components` / `Systems -> Events` (game→game, outside the ratchet's scope).

---

## H. SIZE / SHAPE

### H.1 `wc -l` for every file read (48 files, 9654 lines)

```
 284  src/DualFrontier.Application/Loop/GameBootstrap.cs
 357  src/DualFrontier.Application/Loop/EngineSession.cs
 158  src/DualFrontier.Application/Loop/GameLoop.cs
  63  src/DualFrontier.Application/Loop/EngineHealth.cs
 145  src/DualFrontier.Application/Loop/SimulationStateController.cs
  87  src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs
  31  src/DualFrontier.Application/Scenario/ScenarioDef.cs
  50  src/DualFrontier.Application/Scenario/ScenarioLoader.cs
 169  src/DualFrontier.Application/Scenario/RandomPawnFactory.cs
 152  src/DualFrontier.Application/Scenario/ItemFactory.cs
  14  src/DualFrontier.Application/Scene/SceneMetadata.cs
  25  src/DualFrontier.Application/Scene/SceneDef.cs
1509  src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs
 837  src/DualFrontier.Application/Modding/ContractValidator.cs
 413  src/DualFrontier.Application/Modding/ManifestParser.cs
  57  src/DualFrontier.Application/Modding/DefaultModDiscoverer.cs
 480  src/DualFrontier.Application/Modding/ModRegistry.cs
  77  src/DualFrontier.Application/Modding/SystemAdapter.cs
 163  src/DualFrontier.Application/Modding/SystemContextView.cs
 320  src/DualFrontier.Application/Modding/ModMenuController.cs
  22  src/DualFrontier.Application/Bridge/Commands/ItemSpawnedCommand.cs
  27  src/DualFrontier.Application/Bridge/Commands/PawnStateCommand.cs
  28  src/DualFrontier.Contracts/Modding/IMod.cs
 128  src/DualFrontier.Contracts/Modding/IModApi.cs
 149  src/DualFrontier.Contracts/Modding/ModManifest.cs
  59  src/DualFrontier.Contracts/Sdk/ISimulationSystem.cs
 210  src/DualFrontier.Contracts/Sdk/ISystemContext.cs
  34  src/DualFrontier.Contracts/Sdk/ISystemServices.cs
 319  src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs
  79  src/DualFrontier.Core/Bus/GameServices.cs
 110  src/DualFrontier.Launcher/Program.cs
  31  src/DualFrontier.Runtime/RuntimeOptions.cs
  13  src/DualFrontier.Runtime/Window/WindowOptions.cs
  32  mods/DualFrontier.Mod.Weather/WeatherMod.cs
 164  mods/DualFrontier.Mod.Weather/WeatherSystem.cs
  33  mods/DualFrontier.Mod.Vanilla.Pawn/PawnMod.cs
 128  tests/DualFrontier.Governance.Tests/BoundaryRatchetTests.cs
  50  tests/DualFrontier.Modding.Tests/Sdk/ExampleModReferenceRatchetTests.cs
 214  tests/DualFrontier.Analyzers.Tests/CensusMetaTests.cs
 601  tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs
 191  tests/DualFrontier.Modding.Tests/Loop/EngineSessionTransactionTests.cs
 470  tests/DualFrontier.Modding.Tests/Weather/WeatherWaveGateTests.cs
 105  tests/DualFrontier.Modding.Tests/Weather/WeatherHarness.cs
 231  docs/architecture/VANILLA_SEPARATION_MIGRATION_PLAN.md
 135  docs/architecture/GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md
 228  docs/architecture/EXECUTION_AUTHORITY_MATRIX.md
 292  docs/architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md
 180  tools/briefs/EQ_A2_SHUTDOWN_TRANSACTION_BRIEF.md
```

Plus the whole-project scans (`src/DualFrontier.Systems/` 2622 lines across 29 files; `src/DualFrontier.AI/` 683 across 17; `src/DualFrontier.Application/` 9212 across 96 incl. obj).

### H.2 Composition-root test coverage today

`tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — 601 lines, **18 test methods**:
`:57` ReturnsContextWithLoopAndController · `:72` ReturnedController_BeginEditingSucceedsAndPauses · `:90` WithEmptyModsRoot · `:108` WithModsRootContainingFixture · `:134` WithNonExistentModsRoot_NoThrow · `:158` **DefaultModsRoot_IsLiteralStringMods** (reflection pin on the default arg + `CreateSession` name) · `:180` ReturnedLoop_StartStopRoundTripsCleanly · `:199` **Spawns50PawnsByDefault** · `:220` RunningLoop_PawnStateCommandCarriesRealName · `:265` RunningLoop_PawnStateCommandCarriesTopSkills · `:310` RunningLoop_PublishesTickAdvancedCommandsThroughBridge · `:366`/`:384`/`:401`/`:420`/`:441`/`:459` six MenuFlow tests · `:482` **DefaultModDiscoverer_FindsAll6VanillaSkeletonsInProductionModsRoot** (pins 9 mods).

`tests/DualFrontier.Modding.Tests/Loop/EngineSessionTransactionTests.cs` — 191 lines, **8 tests**: `:32` reverse-order teardown · `:53` UnloadAll+bus-clear exactly once · `:67` fence-timeout abort · `:92` idempotent Dispose · `:107` busy-world abort · `:146`/`:165`/`:177` EngineHealth triple. Harness at `:29` calls `CreateSession(new PresentationBridge(), modsRoot: "mods", shutdownHooks: hooks)`.

`tests/DualFrontier.Modding.Tests/Weather/WeatherWaveGateTests.cs` — 470 lines, **14 tests**, including the F-58 residue pin `:112-137` and the three F60A ALC-release pins `:236`, `:263`, `:290`.

**W4 blast radius on tests**: deleting `InitialPawnCount = 50` reddens `Spawns50PawnsByDefault` (`:199`); moving component registration reddens the RunningLoop trio; changing the default mods root reddens `:158`; changing `CreateSession`'s existence reddens `:168`.

### H.3 EAM rows W4 touches — quoted

**R13 (the BD-4/BD-8 row)** — quoted in §E.3 above (`:90`, `:102`, reading rule `:92`).

**R9 — Engine lifecycle & composition** (`docs/architecture/EXECUTION_AUTHORITY_MATRIX.md:86`):

> | R9 | Engine lifecycle & composition | Proposed: the Application composition root (`GameBootstrap.CreateLoop`, `GameBootstrap.cs:70`) plus a to-be-named shutdown owner | De-facto `Program.Main` + `GameBootstrap.CreateLoop`; **doc-space owner is LOCKED law** — ENGINE_LIFECYCLE_AND_TRANSACTIONS.md + RESOURCE_OWNERSHIP_AND_LIFETIME.md (ratified 2026-07-17); the CODE owner (`EngineSession` + the shutdown transaction) is unbuilt — ROADMAP EQ-a | `GameContext` (carries loop + mod controller out of the factory) | A second composition root; subsystems self-composing at static-init time |

**R2 — scheduling decisions** (`:79`, cited because W4 moves the `GameBootstrap.cs:182-203` native-graph registration block):

> | R2 | System hazards, execution order, wakes (scheduling *decisions*) | Native `SystemGraph` + wake registry (K-L12/K-L13, KERNEL_ARCHITECTURE.md Part 0) | **Managed `DependencyGraph`** builds the production phases (`GameBootstrap.cs:145-148`); the native graph holds the same systems with empty access sets (`GameBootstrap.cs:170-176`) and Timer-1 wakes for all (`GameBootstrap.cs:179`) | `SchedulerAdapter` + `ManagedSystemDispatcher.OnBatch` … | Managed `DependencyGraph` in production planning after the §3.1 cutover |

**R11 note** (`:100`, cited because it names a `GameBootstrap` anchor that ID-B already fixed):

> The version-0 idiom (`new EntityId(indices[i], 0)`, `GameBootstrap.cs:241`; `EntityEncoder.cs:85`) collapses generation validation to "hope the index was never reused".

**R5 — event routing** (`:82`) is W4-adjacent because `EngineSession` holds a `GameServices` whose genre getters (`src/DualFrontier.Core/Bus/GameServices.cs:29,32,35,38,41` — `Combat`, `Inventory`, `Magic`, `Pawns`, `World`) are cosmetic bridges over one `DomainEventBus` (`:26`) yet still carry **game vocabulary inside an engine assembly**.

---

# DISCREPANCIES (vs the CONTEXT given)

1. **BD-8 const anchors are stale.** `VANILLA_SEPARATION_MIGRATION_PLAN.md:93` and `EXECUTION_AUTHORITY_MATRIX.md:102` both say `GameBootstrap.cs:58-68`. Actual: **`:65-75`**, and there are **11** consts, not 11 lines' worth of a `:58-68` range. Off by 7 lines.

2. **The W4 gate is already satisfied at the class level.** The CONTEXT states the GATE as "EngineSession compiles with zero references to Components/Events/Systems/AI". `src/DualFrontier.Application/Loop/EngineSession.cs` **already has zero such references** at HEAD (§B.2). The gate as phrased is green before the wave begins. The falsifiable version has to be assembly-scoped (the 4 `ProjectReference` edges in `DualFrontier.Application.csproj:11,14,15,16`), which is what the existing `BoundaryRatchetTests` actually measures.

3. **W1 recon's "28 systems, 10 real / 18 stub" is now 27 systems, 10 real / 17 stub.** Command `grep -rn "class [A-Za-z]*System : SystemBase" --include=*.cs src/DualFrontier.Systems/ | wc -l` → 27; `grep -rln "BridgeImplementation" ... | wc -l` → 17. W3 deleted the Weather stub (plan `:148`, commit `351d623`), which accounts for the −1.

4. **Migration plan §2 stock deltas disagree with measurement on three of four families.**
   - Plan `:65`/`:206`: "Systems 29". Measured: **27 system classes** (29 is the `.cs` FILE count including `Magic/Internal/ManaLease.cs` + `ManaLeaseRegistry.cs`, which are not systems).
   - Plan `:65`/`:205`: "Events 52". Measured: **51 files, 51 public type declarations** (45 `IEvent` records + 2 record structs + 4 enums). Command: `grep -rn -E "public (readonly )?(sealed )?(record|struct|class|enum) " --include=*.cs src/DualFrontier.Events/ | wc -l` → 51.
   - Plan `:65`/`:207`: "AI: 20 types". Measured: **19 public type declarations across 17 files**.
   - Plan `:65`/`:204`: "Components 28 types". Measured: **28** (23 `IComponent` structs + 5 enums). ✅ EXACT.

5. **"6 vanilla mods … absent from DualFrontier.sln (only Mod.Example enrolled)"** (plan `:70-71`) is stale prose retained from the pre-W0 baseline; W0 enrolled them (plan `:106-109`). All 8 mod projects are in the sln today (`DualFrontier.sln:40,108,113,118,123,128,133,138,140`). The plan's §2 header does say "re-verify at each wave's Phase 0", so this is expected drift, but it must not be quoted as current truth.

6. **ELT contains no "D3".** The CONTEXT directs "grep docs/architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md for D3". `grep -n "D3"` on that file returns **nothing**. The ratified D3 text lives in `tools/briefs/EQ_A2_SHUTDOWN_TRANSACTION_BRIEF.md:28-34` (quoted verbatim in §B.4); ELT carries only realization notes at `:161` and `:241`.

7. **EAM R13's "No config file or loader exists anywhere in the repo"** is true for live config but not literally true: `ScenarioLoader` (`src/DualFrontier.Application/Scenario/ScenarioLoader.cs`, 50 lines) is a JSON config loader — it simply has zero callers.

8. **EAM anchor drift on three cited sites.** `ParallelSystemScheduler.cs:90` for MaxDoP → actual `:112`. `GameBootstrap.cs:145-148` (DependencyGraph) → actual `:167-170`. `GameBootstrap.cs:170-179` (native graph + Timer-1) → actual `:192-201`. `GameBootstrap.cs:241` (`EntityId(…, 0)`) → the site is now `:273` and reads `new EntityId(indices[i], versions[indices[i]])` — **ID-B already fixed it**, so the R11 note describes a defect that no longer exists.

9. **F-42's `GameBootstrap.cs:96` anchor** (ObstacleSeed) → actual `:106`.

---

# SURPRISES (load-bearing, unanticipated)

1. **Production never loads a single mod.** `GameBootstrap.CreateSession` builds `ModIntegrationPipeline`, `DefaultModDiscoverer` and `ModMenuController` (`:225-229`) but **never calls `pipeline.Apply(...)`**. The only `Apply` site in `src/` is `ModMenuController.cs:273`, gated behind `BeginEditing()` → `Commit()`, and `Program.cs` never opens the menu. So the mod pipeline is fully built and fully idle at runtime. **W4's "move to vanilla lifecycle stages" therefore has no boot-time mod-load path to hang stages off — one must be created.** This also means the 9 discoverable mods, Weather included, do not run in the shipping Launcher; Weather is exercised only by `WeatherHarness`.

2. **There is a once-per-graph-build world-seeding hook and nobody uses it.** `ISimulationSystem.Initialize(ISystemContext)` runs with a LIVE `SystemExecutionContext` pushed (`ParallelSystemScheduler.cs:137-155`, `SystemAdapter.cs:72`, `SystemContextView.cs:41-45`), so full world access — `CreateEntity`, `BeginBatch` — is legal there. Weather nonetheless seeds lazily inside `Tick` (`WeatherSystem.cs:49-53`, `:119-130`). The catch: `InitializeAllSystems` is called from BOTH the ctor (`:126`) and `Rebuild` (`:269`), and every `Apply` rebuilds (`ModIntegrationPipeline.cs:565`) — so `Initialize` re-fires on every mod-set change. It is a "once per graph" hook masquerading as "once per session", which is probably why W3 avoided it. **This is the single most important design input for BD-8's "seeds move to vanilla lifecycle stages".**

3. **The forbidden-namespace footprint in Application is only 6 files.** Not a diffuse coupling — a surgical one: `GameBootstrap.cs`, `VanillaComponentRegistration.cs`, `RandomPawnFactory.cs`, `ItemFactory.cs`, `PawnStateCommand.cs`, `ItemSpawnedCommand.cs`. 24 using-sites, **zero** fully-qualified references outside usings. Delete/relocate those six and the four `ProjectReference` edges can be cut outright, making the gate compiler-enforced with **no new csproj**. The two render-command records (`PawnStateCommand` needs only `SkillKind`; `ItemSpawnedCommand` needs only `ItemKind`) are the cheapest two of the six — both need one enum, not a component or system.

4. **`src/DualFrontier.Systems/DualFrontier.Systems.csproj:11` references `DualFrontier.AI` and nothing in `src/DualFrontier.Systems/` uses it.** Command: `grep -rn "DualFrontier\.AI\|AStarPathfinding\|NavGrid" --include=*.cs src/DualFrontier.Systems/` → **no output**. `MovementSystem` takes `IPathfindingService` from `Contracts` instead. A dead ProjectReference the boundary ratchet cannot see (game→game edges are outside its `EngineAssemblies` set at `BoundaryRatchetTests.cs:34-42`).

5. **The `Scene/` orphan cluster is 4× bigger than BD-8 says.** BD-8 names `SceneMetadata`. In fact the entire `src/DualFrontier.Application/Scene/` folder — **9 files, 214 lines** (`SceneDef`, `SceneMetadata`, `TilemapDef`, `EntitySpawnDef`, `MarkerDef`, `ISceneLoader`, `SceneLoadException`, `UshortArrayBase64JsonConverter`) plus `assets/scenes/sample.dfscene` — has zero production consumers; its only reader is `tests/DualFrontier.Core.Tests/Scene/SceneDefSerializationTests.cs`. Likewise `ScenarioLoader.cs` (50 lines) is orphan alongside `ScenarioDef.cs`. **BD-8's deletion scope is ~295 lines across 11 files, not the 45 lines of the two named types.**

6. **`MOD_OS_ARCHITECTURE.md:570` is stale and cites two test names that no longer exist.** It still states *"A reloaded mod does NOT yet resume its own component state (F-60)"* and pins it to `WeatherWaveGateTests.Reload_DoesNotYetAdoptTheSurvivingSingleton_BecauseComponentIdentityIsPerAlc` and `…Unload_LeaksTheModAlc_BecauseTheTypeRegistryStillHoldsItsComponentType`. F-60 was **CLOSED 2026-08-26 at F60A_TICK_PATH** (`docs/ROADMAP.md:1120`), and the tests were renamed to `Reload_AdoptsTheSurvivingSingleton_ComponentIdentitySurvivesAlcReload` (`WeatherWaveGateTests.cs:178`) and `Unload_AfterTicking_ReleasesTheModAlc_TickPathHoldsNoTypeKey` (`:236`). MOD_OS is at `version: 1.4.0`, `last_modified: '2026-08-20'`, last touched by `60052b6` (2026-08-20) — six days before F60A landed. A W4 doc-amendment rider.

7. **`GameServices` genre vocabulary lives in the ENGINE, not in the four migration-stock assemblies.** `src/DualFrontier.Core/Bus/GameServices.cs:29,32,35,38,41` exposes `Combat`, `Inventory`, `Magic`, `Pawns`, `World` as cosmetic bridges over one `DomainEventBus` (`:26`), and `IGameServices` sits in `DualFrontier.Core.Bus`. `EngineSession` holds a `GameServices` field (`EngineSession.cs:128`) and `ModIntegrationPipeline`'s ctor takes `IGameServices` (`ModIntegrationPipeline.cs:160`). **The W4 gate as written would not catch this** — these are engine namespaces, so a "zero references to Components/Events/Systems/AI" gate passes while five genre nouns remain in `DualFrontier.Core`. If the architect wants B-5 in spirit, the gate needs a vocabulary clause, not only a namespace clause.

8. **`ManifestParser` silently ignores unknown fields.** `description` appears in 3 shipped manifests (`Mod.Example`, `Mod.Weather`, `Mod.Weather.Contracts`) and is not a `ModManifest` property — `Parse` (`ManifestParser.cs:26-95`) reads a fixed key list and never rejects extras. Good news for BD-4 (a `game.manifest.json` can be introduced without breaking anything), but it also means a typo'd key in any manifest is silent today.

9. **`DefaultModDiscoverer` has no ordering semantics at all.** `Directory.EnumerateDirectories` (`:39`) returns filesystem order; parse failures are silently swallowed (`:49-53`). BD-4's "root mod set + constraints" has nothing to override — there is no existing order to respect, which makes a manifest-declared root set additive rather than a replacement.

10. **The RNG story blocks a clean scenario manifest.** F-42 (OPEN) names four fixed-seed sites; three are the W4 consts (`GameBootstrap.cs:68,69,75`) and the fourth is target-typed (`MovementSystem.cs:35` `new(42)`) so a textual grep misses it. `ScenarioDef` already declares `WorldSeed` (`ScenarioDef.cs:21`) — the orphan being deleted is the only place a world seed is *modelled*. If BD-4's manifest carries a seed, W4 either re-invents that field or coordinates with F-42.
