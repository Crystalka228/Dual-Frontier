using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Application.Bootstrap;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Application.Modding;
using DualFrontier.Application.Scenario;
using DualFrontier.Components.Items;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Events.Combat;
using DualFrontier.Events.Pawn;
using DualFrontier.Systems.Inventory;
using DualFrontier.Systems.Pawn;
using DualFrontier.Systems.Power;

namespace DualFrontier.Application.Loop;

/// <summary>
/// Production entry point that assembles the simulation graph: a World,
/// the IGameServices aggregator, the TickScheduler, and the dependency
/// graph of registered systems. Bridges domain events to render commands
/// by subscribing on the appropriate buses. Constructs the modding stack
/// (<see cref="ModIntegrationPipeline"/> + <see cref="ModMenuController"/>
/// + <see cref="DefaultModDiscoverer"/>) atop the same scheduler /
/// services and returns both via <see cref="GameContext"/> — the loop the
/// caller starts plus the controller the mod menu (M7.5.B.2) drives.
///
/// Tests bypass this factory and wire their own scheduler so they can
/// register only the systems under test. Smoke coverage of the production
/// wiring lives in <c>GameBootstrapIntegrationTests</c> (M7.5.B.1).
/// </summary>
internal static class GameBootstrap
{
    private const int MapWidth = 200;
    private const int MapHeight = 200;
    private const int InitialPawnCount = 50;
    private const int FactorySeed = 42;
    private const int ObstacleSeed = 42;
    private const int ObstacleCount = 800;
    private const int InitialFoodCount       = 150;
    private const int InitialWaterCount      = 50;
    private const int InitialBedCount        = 30;
    private const int InitialDecorationCount = 25;
    private const int ItemFactorySeed        = 43;

    /// <summary>
    /// Builds the full production simulation context: domain world,
    /// kernel scheduler with the hard-coded ECS systems, modding
    /// pipeline owning the shared ALC, and the menu controller wired to
    /// that pipeline. The <c>TickScheduler</c> constructed here is also
    /// threaded into <see cref="GameLoop"/> so the loop publishes one
    /// <c>TickAdvancedCommand</c> per fixed-step tick onto the presentation
    /// bridge for HUD tick-label rendering. Returns both via
    /// <see cref="GameContext"/>.
    /// </summary>
    /// <param name="bridge">
    /// Presentation bridge the loop publishes render commands onto.
    /// </param>
    /// <param name="modsRoot">
    /// Directory the <see cref="DefaultModDiscoverer"/> scans for mods.
    /// Defaults to the literal string <c>"mods"</c>: production Godot
    /// launches with cwd = project root, so this resolves to
    /// <c>&lt;project&gt;/mods/</c>. Tests override with a temp path or
    /// fixture path. The discoverer handles a non-existent path by
    /// returning an empty list (first-launch safety, no exception).
    /// </param>
    /// <returns>
    /// A <see cref="GameContext"/> carrying the ready-to-start
    /// <see cref="GameLoop"/> and the <see cref="ModMenuController"/>
    /// the menu (M7.5.B.2) binds to its UI scene. The pipeline inside
    /// the controller is left in its default paused state — bootstrap
    /// does not call <c>Pause</c>/<c>Resume</c>; the menu drives the
    /// §9.2 Pause-Toggle-Apply-Resume sequence from
    /// <see cref="ModMenuController"/>.
    /// </returns>
    public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")
    {
        // K8.3+K8.4 combined milestone — Phase 2 storage foundation (dual-write
        // transition Phase 2-4; managed-side retired at Phase 5 commit 21).
        //
        // Bootstrap.Run constructs the ComponentTypeRegistry internally against
        // the bootstrapped handle and binds it to the returned NativeWorld —
        // registry-based deterministic type ids per K-L4. The managed World is
        // retained alongside during Phase 2-4 because the 12 production systems
        // still read via SystemBase.Query / GetComponent (managed). Factories
        // mint entities on BOTH worlds in lockstep (both start _nextIndex=1,
        // indices align naturally) and dual-write component data. Phase 4
        // commits migrate systems one-by-one to NativeWorld.AcquireSpan reads;
        // Phase 5 commit 21 removes the managed-side mint + dual-write entirely.
        var nativeWorld = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!);

        var world = new World();

        var services = new GameServices();
        var ticks    = new TickScheduler();

        services.Pawns.Subscribe<PawnSpawnedEvent>(e =>
            bridge.Enqueue(new PawnSpawnedCommand(e.PawnId, e.X, e.Y)));
        services.Pawns.Subscribe<ItemSpawnedEvent>(e =>
            bridge.Enqueue(new ItemSpawnedCommand(e.ItemId, e.X, e.Y, e.Kind)));
        services.Pawns.Subscribe<PawnMovedEvent>(e =>
            bridge.Enqueue(new PawnMovedCommand(e.PawnId, e.X, e.Y)));
        services.Combat.Subscribe<DeathEvent>(e =>
            bridge.Enqueue(new PawnDiedCommand(e.Who)));
        services.Pawns.Subscribe<PawnStateChangedEvent>(e =>
            bridge.Enqueue(new PawnStateCommand(
                e.PawnId, e.Name, e.Satiety, e.Hydration, e.Sleep, e.Comfort,
                e.Mood, e.JobLabel, e.JobUrgent, e.TopSkills)));

        var navGrid = new NavGrid(MapWidth, MapHeight);
        var obstacleRng = new Random(ObstacleSeed);
        for (int i = 0; i < ObstacleCount; i++)
        {
            int ox = obstacleRng.Next(0, MapWidth);
            int oy = obstacleRng.Next(0, MapHeight);
            navGrid.SetTile(ox, oy, passable: false);
        }
        var pathfinding = new AStarPathfinding(navGrid);

        var pawnFactory = new RandomPawnFactory(FactorySeed, navGrid, MapWidth, MapHeight);
        IReadOnlyList<EntityId> pawnIds = pawnFactory.Spawn(nativeWorld, world, services, InitialPawnCount);

        // ItemFactory must not place items on pawn tiles — collect pawn
        // positions and pass as the excluded set. RandomPawnFactory always
        // adds PositionComponent, so TryGetComponent succeeds for every id.
        var excludedPositions = new List<GridVector>(pawnIds.Count);
        foreach (EntityId pid in pawnIds)
        {
            if (world.TryGetComponent<PositionComponent>(pid, out var pawnPos))
                excludedPositions.Add(pawnPos.Position);
        }

        var itemFactory = new ItemFactory(ItemFactorySeed, navGrid, MapWidth, MapHeight, nativeWorld);
        itemFactory.Spawn(
            nativeWorld,
            world,
            excludedPositions,
            InitialFoodCount,
            InitialWaterCount,
            InitialBedCount,
            InitialDecorationCount);

        // M8.9 — emit ItemSpawnedEvent per item the factory just placed so
        // the bridge wiring above can hand each one to ItemLayer. Iteration
        // happens here (not inside ItemFactory) so the factory stays a pure
        // data creator; GameBootstrap is the only place that owns both the
        // World and the IGameServices aggregator. Subscriptions are wired
        // before this call (synchronous pub-sub on the same thread), so the
        // events are queued onto the presentation bridge in deterministic
        // order.
        PublishItemSpawnedEvents(world, services);

        // Hard-coded kernel systems live in a local array so the same
        // instances flow into both the dependency graph (kernel
        // scheduling) and ModRegistry.SetCoreSystems (visible to the
        // mod-validation/replacement logic in the pipeline). Per AD #5
        // of the M7.5.B.1 prompt, the mod side must see the same
        // SystemBase instances the kernel scheduler is ticking.
        var coreSystems = new SystemBase[]
        {
            new NeedsSystem(),
            new MoodSystem(),
            new JobSystem(),
            new ConsumeSystem(),       // M8.5 — eats food, drinks water on arrival
            new SleepSystem(),         // M8.6 — sleeps on a bed, restores Sleep + Comfort
            new ComfortAuraSystem(),   // M8.7 — passive ambient Comfort from decorations
            new MovementSystem(pathfinding),
            new PawnStateReporterSystem(),
            new InventorySystem(),
            new HaulSystem(),
            new ElectricGridSystem(),
            new ConverterSystem(),
        };

        var graph = new DependencyGraph();
        foreach (SystemBase s in coreSystems)
            graph.AddSystem(s);
        graph.Build();

        // K6.1 — modding stack is constructed BEFORE the scheduler so the
        // fault handler can be wired into the scheduler ctor as an
        // immutable reference. The handler is owned by the bootstrap layer
        // (session-scoped singleton); the scheduler holds it as a read-only
        // sink, the pipeline receives it for query at Apply time, and the
        // loader is wired to it directly via SetFaultHandler. Construction
        // order matters: regressing to "scheduler first, handler later"
        // would silently undo K6.1 and reintroduce the silent NullModFaultSink
        // default. ModRegistry knows core systems already (via
        // SetCoreSystems below) so the metadata lookup is correct from
        // tick 0.
        var modLoader = new ModLoader();
        var modRegistry = new ModRegistry();
        modRegistry.SetCoreSystems(coreSystems);
        var faultHandler = new ModFaultHandler();
        modLoader.SetFaultHandler(faultHandler);

        // Initial metadata lookup reflects the core-only registry state at
        // bootstrap. Mod systems arrive later through ModIntegrationPipeline.Apply,
        // which calls scheduler.Rebuild with a fresh metadata snapshot
        // built from the post-Apply ModRegistry.
        IReadOnlyDictionary<SystemBase, SystemMetadata> initialMetadata =
            SystemMetadataBuilder.Build(modRegistry);

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            ticks,
            world,
            initialMetadata,
            faultHandler,
            services,
            nativeWorld,
            modRegistry);  // K8.3+K8.4 — IManagedStorageResolver for Path β bridge

        // M7.5.B.1 — modding stack. Pipeline starts in its default
        // paused state (M7.1 load-bearing default); bootstrap does not
        // call Pause/Resume. The controller (returned via GameContext)
        // is what M7.5.B.2's menu scene drives through Begin/Toggle/
        // Cancel/Commit. The same `services` aggregator is threaded
        // through both the kernel scheduler and the pipeline so mod-
        // published events route through the existing M2 wiring.
        var modValidator = new ContractValidator();
        var modContractStore = new ModContractStore();
        var pipeline = new ModIntegrationPipeline(
            modLoader, modRegistry, modValidator, modContractStore, services, scheduler, faultHandler);
        var discoverer = new DefaultModDiscoverer(modsRoot);
        var controller = new ModMenuController(pipeline, discoverer);

        // Pass the TickScheduler into GameLoop so its accumulator can publish
        // a TickAdvancedCommand carrying CurrentTick after every ExecuteTick;
        // RenderCommandDispatcher routes the command to the HUD tick label.
        var loop = new GameLoop(scheduler, ticks, bridge);

        // MOD_OS_ARCHITECTURE §9.2 step 1 — menu opens => simulation pauses.
        // Pipeline.Pause() (already called by controller.BeginEditing) only
        // gates the Apply-mutation safety flag; the tick-advance gate is
        // GameLoop._paused. Wire both surfaces here at the orchestration
        // layer that owns both the loop and the controller.
        controller.OnEditingBegan = () => loop.SetPaused(true);
        controller.OnEditingEnded = () => loop.SetPaused(false);

        return new GameContext(loop, controller);
    }

    /// <summary>
    /// Iterates every item ItemFactory just placed and publishes an
    /// <see cref="ItemSpawnedEvent"/> on the Pawns bus per item. ItemKind
    /// is determined by component family (presentation hint, not domain
    /// truth — domain still classifies items by component presence).
    /// Reads <c>World.GetEntitiesWith&lt;T&gt;</c> via the InternalsVisibleTo
    /// grant from DualFrontier.Core to DualFrontier.Application.
    /// </summary>
    private static void PublishItemSpawnedEvents(World world, GameServices services)
    {
        foreach (EntityId id in world.GetEntitiesWith<ConsumableComponent>())
            PublishOne(world, services, id, ItemKind.Food);
        foreach (EntityId id in world.GetEntitiesWith<WaterSourceComponent>())
            PublishOne(world, services, id, ItemKind.Water);
        foreach (EntityId id in world.GetEntitiesWith<BedComponent>())
            PublishOne(world, services, id, ItemKind.Bed);
        foreach (EntityId id in world.GetEntitiesWith<DecorativeAuraComponent>())
            PublishOne(world, services, id, ItemKind.Decoration);
    }

    private static void PublishOne(World world, GameServices services, EntityId id, ItemKind kind)
    {
        if (!world.TryGetComponent<PositionComponent>(id, out var pos)) return;
        services.Pawns.Publish(new ItemSpawnedEvent
        {
            ItemId = id,
            X      = pos.Position.X,
            Y      = pos.Position.Y,
            Kind   = kind,
        });
    }
}
