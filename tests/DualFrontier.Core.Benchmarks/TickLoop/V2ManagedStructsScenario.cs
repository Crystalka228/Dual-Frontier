using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Application.Scenario;
using DualFrontier.Components.Items;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Events.Pawn;
using DualFrontier.Systems.Inventory;
using DualFrontier.Systems.Pawn;
using DualFrontier.Systems.Power;

namespace DualFrontier.Core.Benchmarks.TickLoop;

/// <summary>
/// K7 V2 scenario — managed <see cref="World"/> + struct components
/// (post-K4 main). Mirrors <c>GameBootstrap.CreateLoop</c> but without
/// the modding stack, the presentation bridge, or the public game-loop
/// orchestration: the benchmark only needs a <see cref="World"/> and a
/// <see cref="ParallelSystemScheduler"/> wired around the production
/// system set so <c>ExecuteTick</c> drives a full simulation step.
///
/// The same <see cref="RandomPawnFactory"/> and <see cref="ItemFactory"/>
/// the production bootstrap uses are re-used here so the workload shape
/// matches the production scenario size byte-for-byte
/// (50 pawns + 150 food + 50 water + 30 beds + 25 decorations on a
/// 200×200 grid with 800 obstacles, identical RNG seeds).
/// </summary>
internal sealed class V2ManagedStructsScenario : TickLoopScenarioBase
{
    private const int MapWidth = 200;
    private const int MapHeight = 200;
    private const int ObstacleCount = 800;
    private const int ObstacleSeed = 42;
    private const int ItemFactorySeed = 43;
    private const int InitialFoodCount = 150;
    private const int InitialWaterCount = 50;
    private const int InitialBedCount = 30;
    private const int InitialDecorationCount = 25;

    private World _world = null!;
    private GameServices _services = null!;
    private TickScheduler _ticks = null!;
    private ParallelSystemScheduler _scheduler = null!;

    public override void SetupWorld(int pawnCount, int seed)
    {
        _world = new World();
        _services = new GameServices();
        _ticks = new TickScheduler();

        var navGrid = new NavGrid(MapWidth, MapHeight);
        var obstacleRng = new Random(ObstacleSeed);
        for (int i = 0; i < ObstacleCount; i++)
        {
            int ox = obstacleRng.Next(0, MapWidth);
            int oy = obstacleRng.Next(0, MapHeight);
            navGrid.SetTile(ox, oy, passable: false);
        }
        var pathfinding = new AStarPathfinding(navGrid);

        // Subscribe a no-op to PawnSpawnedEvent / ItemSpawnedEvent — the
        // factories publish during Spawn and an unsubscribed bus would
        // discard the events silently, which is fine for the benchmark.
        // The production wiring (PresentationBridge) is intentionally
        // omitted so the V2 scenario isolates simulation cost from
        // rendering cost.

        var nativeWorld = new NativeWorld();
        var pawnFactory = new RandomPawnFactory(seed, navGrid, MapWidth, MapHeight, nativeWorld);
        IReadOnlyList<EntityId> pawnIds = pawnFactory.Spawn(_world, _services, pawnCount);

        var excludedPositions = new List<GridVector>(pawnIds.Count);
        foreach (EntityId pid in pawnIds)
        {
            if (_world.TryGetComponent<PositionComponent>(pid, out var pawnPos))
                excludedPositions.Add(pawnPos.Position);
        }

        var itemFactory = new ItemFactory(ItemFactorySeed, navGrid, MapWidth, MapHeight);
        itemFactory.Spawn(
            _world,
            excludedPositions,
            InitialFoodCount,
            InitialWaterCount,
            InitialBedCount,
            InitialDecorationCount);

        // Hard-coded production system set, identical instance ordering to
        // GameBootstrap.CreateLoop. The dependency graph topologically
        // orders these by their declared [SystemAccess] reads/writes so
        // the scheduler can phase-parallelize without isolation hazards.
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
            new ElectricGridSystem(),
            new ConverterSystem(),
        };

        var graph = new DependencyGraph();
        foreach (SystemBase s in coreSystems)
            graph.AddSystem(s);
        graph.Build();

        // K7 V2 — empty metadata table is the right input here: every
        // benchmark system is core-origin, BuildContext defaults missing
        // entries to Core/null, no fault routing exercised. Explicit
        // NullModFaultSink because the post-K6.1 scheduler ctor requires
        // a non-null sink.
        _scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            _ticks,
            _world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            _services);
    }

    public override void ExecuteTick(float delta)
    {
        _scheduler.ExecuteTick(delta);
    }

    public override void TeardownWorld()
    {
        // Managed scenario owns no unmanaged resources; the World, scheduler,
        // and services become eligible for GC when this scenario goes out of
        // scope. Explicit null-out is cosmetic but communicates intent.
        _scheduler = null!;
        _ticks = null!;
        _services = null!;
        _world = null!;
    }
}
