# Roadmap

The Dual Frontier implementation is split across eight phases. Each phase has a clear output artifact, acceptance criteria, and a list of systems it unblocks. Phases do not overlap in code ownership: one layer first, then the next. This makes integration testing possible at every step and prevents risk accumulation.

## Phase 0 — Contracts

Goal: pin every public interface and attribute in the `DualFrontier.Contracts` assembly.

### What we implement

Files in `src/DualFrontier.Contracts/`:

- `Core/IEntity.cs`, `Core/EntityId.cs`, `Core/IComponent.cs`, `Core/IEvent.cs`, `Core/IQuery.cs`, `Core/IQueryResult.cs`, `Core/ICommand.cs`.
- `Bus/IEventBus.cs`, `Bus/IGameServices.cs`, `Bus/ICombatBus.cs`, `Bus/IInventoryBus.cs`, `Bus/IMagicBus.cs`, `Bus/IPawnBus.cs`, `Bus/IWorldBus.cs`.
- `Modding/IMod.cs`, `Modding/IModApi.cs`, `Modding/IModContract.cs`, `Modding/ModManifest.cs`.
- `Attributes/SystemAccessAttribute.cs`, `Attributes/DeferredAttribute.cs`, `Attributes/ImmediateAttribute.cs`, `Attributes/TickRateAttribute.cs`.

### Acceptance criteria

- `dotnet build src/DualFrontier.Contracts` passes with no errors or warnings.
- Every public type has English XML documentation.
- Dependencies: `System.*` only. No project references.

### Unblocks

Everything else — no other assembly builds without `Contracts`.

## Phase 1 — Core

Goal: a working ECS core with a multithreaded scheduler and domain buses.

> Extended in v0.3 (Phase 4 debt closure):
> `[Deferred]` and `[Immediate]` semantics are implemented in `DomainEventBus` —
> `[Deferred]` events accumulate in per-bus queues and are delivered by
> `ParallelSystemScheduler` on each phase boundary inside the subscriber's
> `SystemExecutionContext` captured at Subscribe;
> `[Immediate]` is guaranteed to deliver synchronously. See
> [EVENT_BUS](./EVENT_BUS.md).

### What we implement

Files in `src/DualFrontier.Core/`:

- `ECS/World.cs`, `ECS/ComponentStore.cs`, `ECS/SystemBase.cs`, `ECS/SystemExecutionContext.cs`, `ECS/IsolationViolationException.cs`.
- `Scheduling/DependencyGraph.cs`, `Scheduling/ParallelSystemScheduler.cs`, `Scheduling/SystemPhase.cs`, `Scheduling/TickScheduler.cs`, `Scheduling/TickRates.cs`.
- `Bus/DomainEventBus.cs`, `Bus/GameServices.cs`, `Bus/IntentBatcher.cs`.
- `Math/SpatialGrid.cs` (infrastructure; the `GridVector` primitive lives in `DualFrontier.Contracts.Math`).
- `Registry/ComponentRegistry.cs`, `Registry/SystemRegistry.cs`.

Main simulation loop (`src/DualFrontier.Application/Loop/`):

- [x] `FrameClock` — `delta` source backed by `Stopwatch`, with an `Update()` method.
- [x] `GameLoop` — accumulator-based tick (30 Hz), pause, speed x1/x2/x3.

### Acceptance criteria

- Unit tests for `ComponentStore`, `DependencyGraph`, `DomainEventBus` pass.
- Scheduler test on the "3 systems, 2 phases" scenario with real parallelism.
- `SystemExecutionContext` in DEBUG catches access to an undeclared component.
- `IntentBatcher` groups events for one phase and delivers the batch in the next.

### Unblocks

All game systems. Starting from this phase, `DualFrontier.Systems` builds.

## Phase 2 — Verification

Goal: prove that the architectural guarantees actually work, not just that they are declared.

### What we implement

Tests in `tests/`:

- `DualFrontier.Core.Tests/Isolation/` — full isolation-guard test suite: undeclared component, direct system access, `GetSystem` request, `async` in `Update`, write to a non-declared bus.
- `DualFrontier.Core.Tests/Scheduling/` — parallel systems without conflicts actually run on different threads (verified via `Thread.CurrentThread.ManagedThreadId`).
- `DualFrontier.Modding.Tests/` — a mod cannot load `DualFrontier.Core.dll` (`ModLoadContext` check).

Plus scripted fixture mods in `tests/fixtures/` — both legitimate and "evil" (attempting to violate).

Sources in `src/`:

- `DualFrontier.Application/Modding/` — `ModIntegrationPipeline`,
  `ContractValidator`, `ModRegistry`, `ModContractStore`, `RestrictedModApi` (full implementation).

World components (`src/DualFrontier.Components/World/`):

- [x] `TerrainKind` enum (Grass, Rock, Sand, Water, Ice, Swamp, Arcane, Unknown).
- [x] `TileComponent` — `Terrain`, `Passable`, `Default`.

### Technical debt

- [ ] Modding tests for `AssemblyLoadContext` (WeakReference unload, attempt to
      load `DualFrontier.Core.dll`) — not yet implemented.

### Acceptance criteria

- 100% of isolation tests pass in both DEBUG and RELEASE (for the critical ones).
- Mod test: an attempt to load an assembly with `using DualFrontier.Core;` returns `ModIsolationException`.
- Documented isolation-guard coverage report — which violation types are tested.
- `ModIntegrationPipeline.Apply` is atomic: an error at any step does not break the current scheduler.
- `ContractValidator` returns a precise message on a write-write conflict between mods.
- WeakReference test: `ModLoadContext.Unload` physically releases the assembly.

### Unblocks

Safe development of everything that follows. From here on, any isolation regression is caught by CI.

## Phase 3 — Pawns

Goal: a living pawn that walks the map, eats, sleeps, and has tasks.

### ✅ Phase 3 — Living colony (closed)

Result: pawns walk the map via A* pathfinding,
needs decay, mood recomputes, jobs are assigned by priority through event buses.
61/61 tests. Godot visuals are wired in.

### What we implement

- `DualFrontier.Components/Shared/PositionComponent`, `FactionComponent`, `RaceComponent`.
- `DualFrontier.Components/Pawn/NeedsComponent`, `MindComponent`, `JobComponent`, `SkillsComponent`.
- [x] `DualFrontier.Systems/Pawn/NeedsSystem` (SLOW) — Hunger/Thirst/Rest/Comfort decay.
- [x] `DualFrontier.Systems/Pawn/MoodSystem` — formula `mood = f(needs)`,
      transition into MoodBreak.
- [x] `DualFrontier.Systems/Pawn/JobSystem` (NORMAL) — priorities driven by needs,
      `JobKind.Eat/Sleep/Idle`.
- [ ] `DualFrontier.Systems/Pawn/SocialSystem` — stub.
- [ ] `DualFrontier.Systems/Pawn/SkillSystem` — stub.
- [x] `DualFrontier.AI/Pathfinding/IPathfindingService`, `AStarPathfinding`
      (A* with a 2000-iteration cap, no cache), `NavGrid` (passability bitmap,
      cost map, SetTile).
- [x] `DualFrontier.Events/Pawn/DeathEvent`, `PawnSpawnedEvent`,
      `PawnMovedEvent`, `MoodBreakEvent`, `SkillGainEvent`.
- [x] `DualFrontier.Application/Scenario/ScenarioDef`, `ScenarioLoader`
      (JSON + `LoadDefault()`).
- [x] `MovementComponent` + `MovementSystem` — pawn movement along the A* route,
      publishes `PawnMovedEvent`.
- [x] `NavGrid` initialization from `GameBootstrap` (50×50, 50 obstacles).
- [x] Godot visuals — pawns move on the map, `PawnMovedEvent`
      is published and handled.
- `DualFrontier.AI/Jobs/IJob`, `JobHaul`, `JobMeditate`.
- `DualFrontier.AI/BehaviourTree/BTNode`, `Selector`, `Sequence`, `Leaf`.

### Acceptance criteria

- Simulation on 30 pawns, 100×100 tiles, 60 FPS without stutter.
- A pawn gets a task through `JobComponent`; `JobSystem` executes it via the behaviour tree.
- `NeedsSystem` decreases hunger/sleep; the pawn seeks food via `JobSystem`.
- Pathfinding works and caches routes.

### Open tasks

- [ ] `SocialSystem` — full implementation (currently a stub).
- [ ] `SkillSystem` — full implementation (currently a stub).
- [ ] System access to `IGameServices` for publishing events to buses
      (`MoodSystem` currently has a stub instead of publishing
      `MoodBreakEvent`).

### Unblocks

The production chain: no walking pawn means no crafting.

## Phase 3.5 — Godot DevKit (short interim phase)

Goal: Godot works fully as an editor and as a temporary runtime for testing.
The game can be launched through F5 in Godot and through `dotnet run` in Native — both
implementations consume the same `.dfscene`.
Architectural context: [VISUAL_ENGINE](./VISUAL_ENGINE.md).

### What we implement

- `src/DualFrontier.Presentation/addons/df_devkit/` — a full plugin:
  `DfDevKitPlugin`, `DFEntityMeta` with inspector UI, `SceneExporter` with a real
  SceneTree walk, `TilemapExporter`, `EntityExporter`.
- `src/DualFrontier.Presentation/GodotRenderer.cs` — `IRenderer` implementation
  using standard Godot `_Process`.
- `src/DualFrontier.Presentation/GodotSceneLoader.cs` — `ISceneLoader` implementation
  using `FileAccess.Open(res://...)` and `System.Text.Json`.
- `src/DualFrontier.Presentation/GodotInputRouter.cs` — `IInputSource` implementation.
- Editor menu: "Tools → Export .dfscene → choose path".

### Acceptance criteria

- Godot Editor opens `main.tscn`, edits, exports to `.dfscene`.
- `GodotSceneLoader.Load("sample.dfscene")` returns `SceneDef` with a tilemap
  and three entities (pawn + spawner + marker).
- F5 in Godot starts the game with the loaded scene; the pawn is alive.
- `dotnet test` passes all 43+ tests plus new `SceneDefSerializationTests`.

### Unblocks

The production pipeline Godot → `.dfscene` → Native. From here on, Phase 4
works with real scenes instead of hardcoded data.

## ✅ Phase 4 — Economy (closed)

Result: storages, crafting workbenches, the power grid, and the converter.
`InventorySystem` with a cache, `HaulSystem`, `ElectricGridSystem` with priority
distribution, `ConverterSystem` at 30% efficiency (registered in `GameBootstrap`,
the ElectricGrid cycle broken via a `[Deferred]` event). Grimdark HUD with
`ColonyPanel` and `PawnDetail`.
82/82 tests (including Phase 4 coverage).

### What is implemented

- [x] `StorageComponent`, `WorkbenchComponent`
- [x] `PowerConsumerComponent`, `PowerProducerComponent`
- [x] `InventorySystem` (cache + batching via `_freeSlotCache` / `_cacheDirty`,
      reservations via `[Deferred] ItemReservedEvent`)
- [x] `HaulSystem` — source→dest item lookup (Phase 4: teleport without
      pathfinding); per-Update reservation set prevents same-tick
      double-allocation
- [x] `ElectricGridSystem` — priority watt distribution, `IPowerBus` bus
- [x] `ConverterSystem` (30% efficiency) — registered, publishes
      `[Deferred] ConverterPowerOutputEvent` on `IPowerBus`, breaking
      the component cycle with ElectricGrid
- [x] `Events/Inventory/*` — all four events (`ItemAdded`, `ItemRemoved`,
      `ItemReserved`, `CraftRequest`); the first three are marked `[Deferred]`
- [x] `Events/Power/*` — `PowerRequest`, `PowerGranted`, `GridOverload`,
      `ConverterPowerOutputEvent` (`[Deferred]`)
- [x] Grimdark HUD — `GameHUD` (CanvasLayer), `ColonyPanel`, `PawnDetail`,
      `PawnStateReporterSystem`, `PawnStateCommand`
- [x] `IPowerBus` added to `IGameServices` (non-breaking, see
      [CONTRACTS](./CONTRACTS.md) §"Non-breaking → Adding a new domain bus")
- [x] `BridgeImplementationAttribute` introduced in `DualFrontier.Contracts.Attributes`
      and applied to every system with a stub `OnInitialize` — Phase 5+
      registration no longer fails

### Open tasks

Closed in v0.3 (see the commit history in the "v0.3 architectural fixes" section below).

### Acceptance criteria

- 100 pawns × 60 ticks × ammo request: ≤100 scans/sec (target from 11.11).
- Crafting chain: resource → workbench → product → storage. *(Phase 6 — `CraftSystem`)*
- Power grid: generator → wire → consumer; overload on overflow. ✅
  (see `tests/DualFrontier.Systems.Tests/Power/ElectricGridOverloadTests.cs`)
- Converter: 100W → 30W (the magic branch will receive mana in Phase 6); works
  point-wise, unprofitable at scale. ✅ (see
  `tests/DualFrontier.Systems.Tests/Power/ConverterEfficiencyTests.cs`)

### v0.3 architectural fixes (technical-debt closure)

- `[Deferred]` / `[Immediate]` delivery is implemented in `DomainEventBus` (Q1).
- `IPowerBus` is added to `IGameServices`; `ElectricGridSystem` and
  `ConverterSystem` are migrated to this bus (Q2).
- `ItemAddedEvent` / `ItemRemovedEvent` / `ItemReservedEvent` are marked
  `[Deferred]`; `InventorySystem` subscribes to all three, `StorageComponent`
  mutations happen in the next phase through the captured context —
  `HaulSystem.writes=[]` isolation is preserved (Q3, Q4).
- `ConverterSystem.writes` is narrowed to `[]`; output is published via
  `[Deferred] ConverterPowerOutputEvent`, breaking the DAG cycle (Q6).
- All stubs with `throw new NotImplementedException` in `OnInitialize` got
  an empty body + `[BridgeImplementation(Phase = N)]` (Q5) — unblocks
  Phase 5/6/7 registration.
- `HaulSystem`'s `return` after a failed `TryFindHaul` is replaced with `continue`;
  the `DomainEventBus.Subscribe` TOCTOU is fixed via `GetOrAdd`.

### Unblocks

Combat and magic — both systems need inventory (ammo, crystals).
Phase 5 (combat) now has a working `[Deferred] DeathEvent` and bridge stubs
for the Magic systems that no longer crash on registration.

## 🔨 Phase 5 — Combat (current)

Goal: Combat Extended built on top of the bus.

### What we implement

- `DualFrontier.Components/Combat/WeaponComponent`, `ArmorComponent`, `AmmoComponent`, `ShieldComponent`, `HealthComponent`.
- `DualFrontier.Systems/Combat/CombatSystem` (FAST, phase 1), `ProjectileSystem` (REALTIME), `DamageSystem` (FAST, phase 2), `StatusEffectSystem`, `ShieldSystem`.
- `DualFrontier.Events/Combat/ShootAttemptEvent`, `AmmoIntent`, `AmmoGranted`, `AmmoRefused`, `DamageEvent`, `DeathEvent`, `StatusAppliedEvent`.

### Acceptance criteria

- The two-step `AmmoIntent → AmmoGranted` model works through `IntentBatcher`.
- Damage model: damage types (Heat, Sharp, Blunt, EMP, Toxic, Psychic, Stagger) apply with armor accounting.
- Shields: HP pool, regeneration, weaknesses.
- `DeathEvent` is marked `[Deferred]` — `MoodSystem` and `SocialSystem` receive it in the next phase.

### In parallel: Native Runtime Bootstrap

Development of `DualFrontier.Presentation.Native` begins:
- `PackageReference` on Silk.NET.
- `NativeRenderer.Initialize` creates the window and GL context.
- `SpriteBatch`, `TilemapRenderer` — basic rendering.
- `NativeSceneLoader` parses the same `.dfscene`.

Does not block Phase 5's main work — Native lives in a separate assembly.

### Unblocks

Magical combat: the unified damage physics is already in place; magic will simply consume it.

## Phase 6 — Magic

Goal: a magical colony with golems, mage progression, and ether breaks.

### What we implement

- `DualFrontier.Components/Magic/ManaComponent`, `SchoolComponent`, `EtherComponent`, `GolemBondComponent`.
- `DualFrontier.Systems/Magic/ManaSystem` (NORMAL, phase 1), `SpellSystem` (FAST), `GolemSystem` (NORMAL), `EtherGrowthSystem` (SLOW, phase 2), `RitualSystem` (RARE).
- `DualFrontier.Events/Magic/ManaIntent`, `ManaGranted`, `ManaRefused`, `SpellCastEvent`, `EtherSurgeEvent`, `GolemActivatedEvent`, `EtherLevelUpEvent`.
- `DualFrontier.AI/Jobs/JobCast`, `JobGolemCommand`, `JobMeditate`.

### Acceptance criteria

- 8 schools of magic (Fire / Ice / Thunder / Earth / Wind / Water / Dark / Light) with distinct damage profiles per GDD 6.1.
- 5 levels of ether perception per GDD 4.1, damage scaling per 6.2.
- 5 golem types per 5.1, constant mana drain, mage exhaustion halts the golem.
- Ether break when working with a crystal above the mage's level.
- Combo mechanics: Ice→Earth, Lightning→Metal, Water→Lightning (per 6.3).

### Unblocks

Final content — world, factions, events.

## Phase 7 — World

Goal: a living world around the colony.

### What we implement

- `DualFrontier.Components/World/TileComponent`, `BiomeComponent`, `EtherNodeComponent`.
- `DualFrontier.Systems/World/BiomeSystem`, `WeatherSystem`, `MapSystem`.
- `DualFrontier.Systems/Faction/RelationSystem` (RARE), `TradeSystem`, `RaidSystem` (RARE).
- `DualFrontier.Events/World/EtherNodeChangedEvent`, `WeatherChangedEvent`, `RaidIncomingEvent`.

### Acceptance criteria

- Biomes affect resources, weather, and passability.
- Ether nodes form coverage radii for the magical power grid.
- Inter-faction relations per the GDD 3.3 matrix; dynamic changes through events.
- Raids arrive at different intervals and compositions based on colony level.
- Trade: caravans, resource exchange, seasonality.

### Unblocks

The full game. Further iterations are content expansions through mods.

## Phase 6 (Magic) and Phase 7 (World): bridge between phases (v0.2 §12.6)

### Problem

Phase 5 (Combat) already references magic components: `CombatSystem` knows about `ManaComponent` to process enchanted shots through a composite request (see [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md)). But the magic itself is implemented only in Phase 6. If we wait for the full magic implementation before testing magic shots, Phase 5 is blocked. If we partly wedge magic into Phase 5, we break the "phases do not overlap in code ownership" principle.

### Solution: bridge implementation

Phase 5 wires up a **bridge implementation** of the magic systems — a stub granter that returns a successful answer without real logic. `CombatSystem` sees a full `ManaSystem` through the bus, successfully receives `ManaGranted`, and runs its magic-shot tests. In Phase 6 the stub is replaced with the full implementation without changes to `CombatSystem`.

- **`ManaSystem` (bridge).** Always returns `ManaGranted` (or `ManaLeaseOpened` with a fake `LeaseId`), treating mana level as 100%. `ManaComponent` is not updated.
- **`GolemSystem` (bridge).** Returns an infinite bond: any `GolemOwnershipTransferRequest` is accepted, `GolemBondComponent` is not mutated. Enough for `CombatSystem` to correctly recognize a golem as ally or enemy.
- **`EtherSystem` (bridge).** Static ether level of 50% — a stable constant that lets `EtherSurgeEvent` form correctly at the tolerance boundary in tests.

Bridge systems live in `DualFrontier.Systems/Magic/Bridge/` and are marked `[BridgeImplementation(Phase = 6)]` — the analyzer warns if a bridge implementation remains in a release build after Phase 6.

### Bridge-closure criteria

In Phase 6 every bridge system is replaced by the full implementation. Criteria:

- Every test passing on the bridge version continues to pass on the full implementation.
- The `[BridgeImplementation]` mark is removed; the analyzer stops warning.
- No public contract (`ManaGranted`, `GolemOwnershipChanged`, `EtherSurgeEvent`) changes signature — `CombatSystem` and other consumers require no edits.

## Phase 9 — Native Runtime

### What we implement

- `DualFrontier.Runtime` — own entry point,
  launches `GameLoop` directly without Godot in the chain.
- `IRenderer` — abstract render interface.
- `IInputProvider` — abstract input interface.
- `GodotBackend` — implementation via Godot GDExtension
  (rendering + input without the Godot runtime in execution).
- Godot plugin — reads `.tscn` files and translates
  them into native-runtime calls without the Godot SceneTree.

### Why this is possible

The architecture is already ready for this:
- Domain is fully decoupled from Godot (no `using Godot`)
- `GameLoop` runs on a pure .NET thread
- `PresentationBridge` abstracts rendering behind `IRenderCommand`
- Simulation lives without Godot — the tests prove it

### Unblocks

- Full control over runtime and performance
- Ability to port to any render backend
- Godot as a content tool, not as an engine

### When

After Phase 7 closure and Steam launch.
This is a separate large project; it does not block the release.

## See also

- [ARCHITECTURE](./ARCHITECTURE.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
- [PERFORMANCE](./PERFORMANCE.md)
