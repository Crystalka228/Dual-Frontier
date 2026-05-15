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

namespace DualFrontier.Application.Loop;

/// <summary>
/// Production entry point that assembles the simulation graph: a
/// <see cref="NativeWorld"/>, the <see cref="GameServices"/> aggregator,
/// the <see cref="TickScheduler"/>, and the dependency graph of registered
/// systems. Bridges domain events to render commands by subscribing on
/// the appropriate buses. Constructs the modding stack
/// (<see cref="ModIntegrationPipeline"/> + <see cref="ModMenuController"/>
/// + <see cref="DefaultModDiscoverer"/>) atop the same scheduler /
/// services and returns both via <see cref="GameContext"/>.
///
/// Tests bypass this factory and wire their own scheduler so they can
/// register only the systems under test. Smoke coverage of the production
/// wiring lives in <c>GameBootstrapIntegrationTests</c>.
///
/// K8.3+K8.4 cutover (2026-05-14, brief v2.0): native world is the sole
/// production component-storage backend; the prior managed <c>World</c>
/// is gone from production. Power subsystems (ElectricGrid + Converter)
/// are deleted; <c>coreSystems</c> shrinks from 12 to 10.
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

    public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")
    {
        // Bootstrap.Run constructs the ComponentTypeRegistry internally and
        // binds it to the returned NativeWorld — registry-based deterministic
        // type ids per K-L4. Vanilla components register through the single
        // source-of-truth helper.
        var nativeWorld = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!);

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
        IReadOnlyList<EntityId> pawnIds = pawnFactory.Spawn(nativeWorld, services, InitialPawnCount);

        // ItemFactory must not place items on pawn tiles — collect pawn
        // positions and pass as the excluded set. RandomPawnFactory always
        // adds PositionComponent, so TryGetComponent succeeds for every id.
        var excludedPositions = new List<GridVector>(pawnIds.Count);
        foreach (EntityId pid in pawnIds)
        {
            if (nativeWorld.TryGetComponent<PositionComponent>(pid, out PositionComponent pawnPos))
                excludedPositions.Add(pawnPos.Position);
        }

        var itemFactory = new ItemFactory(ItemFactorySeed, navGrid, MapWidth, MapHeight, nativeWorld);
        itemFactory.Spawn(
            nativeWorld,
            excludedPositions,
            InitialFoodCount,
            InitialWaterCount,
            InitialBedCount,
            InitialDecorationCount);

        // Emit ItemSpawnedEvent per item the factory just placed so the
        // bridge wiring above can hand each one to ItemLayer.
        PublishItemSpawnedEvents(nativeWorld, services);

        var coreSystems = new SystemBase[]
        {
            new NeedsSystem(),
            new MoodSystem(),
            new JobSystem(),
            new ConsumeSystem(),
            new SleepSystem(),
            new ComfortAuraSystem(),
            new MovementSystem(pathfinding),
            new PawnStateReporterSystem(),
            new InventorySystem(),
            new HaulSystem(),
        };

        var graph = new DependencyGraph();
        foreach (SystemBase s in coreSystems)
            graph.AddSystem(s);
        graph.Build();

        var modLoader = new ModLoader();
        var modRegistry = new ModRegistry();
        modRegistry.SetCoreSystems(coreSystems);
        var faultHandler = new ModFaultHandler();
        modLoader.SetFaultHandler(faultHandler);

        IReadOnlyDictionary<SystemBase, SystemMetadata> initialMetadata =
            SystemMetadataBuilder.Build(modRegistry);

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            ticks,
            initialMetadata,
            faultHandler,
            nativeWorld,
            services,
            modRegistry);

        var modValidator = new ContractValidator();
        var modContractStore = new ModContractStore();
        var pipeline = new ModIntegrationPipeline(
            modLoader, modRegistry, modValidator, modContractStore, services, scheduler, faultHandler);
        var discoverer = new DefaultModDiscoverer(modsRoot);
        var controller = new ModMenuController(pipeline, discoverer);

        var loop = new GameLoop(scheduler, ticks, bridge);

        controller.OnEditingBegan = () => loop.SetPaused(true);
        controller.OnEditingEnded = () => loop.SetPaused(false);

        return new GameContext(loop, controller);
    }

    /// <summary>
    /// Iterates every item ItemFactory just placed and publishes an
    /// <see cref="ItemSpawnedEvent"/> on the Pawns bus per item.
    /// </summary>
    private static void PublishItemSpawnedEvents(NativeWorld nativeWorld, GameServices services)
    {
        PublishKind<ConsumableComponent>(nativeWorld, services, ItemKind.Food);
        PublishKind<WaterSourceComponent>(nativeWorld, services, ItemKind.Water);
        PublishKind<BedComponent>(nativeWorld, services, ItemKind.Bed);
        PublishKind<DecorativeAuraComponent>(nativeWorld, services, ItemKind.Decoration);
    }

    private static void PublishKind<T>(NativeWorld nativeWorld, GameServices services, ItemKind kind)
        where T : unmanaged, IComponent
    {
        using SpanLease<T> lease = nativeWorld.AcquireSpan<T>();
        ReadOnlySpan<int> indices = lease.Indices;
        for (int i = 0; i < lease.Count; i++)
        {
            var id = new EntityId(indices[i], 0);
            if (!nativeWorld.TryGetComponent<PositionComponent>(id, out PositionComponent pos)) continue;
            services.Pawns.Publish(new ItemSpawnedEvent
            {
                ItemId = id,
                X      = pos.Position.X,
                Y      = pos.Position.Y,
                Kind   = kind,
            });
        }
    }
}
