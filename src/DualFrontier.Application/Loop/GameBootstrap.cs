using System;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
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
/// by subscribing on the appropriate buses. Returns a ready-to-start
/// <see cref="GameLoop"/> bound to the supplied
/// <see cref="PresentationBridge"/>.
///
/// Tests bypass this factory and wire their own scheduler so they can
/// register only the systems under test.
/// </summary>
internal static class GameBootstrap
{
    private static readonly GridVector[] InitialPawnPositions =
    {
        new(25, 25),
        new(27, 25),
        new(26, 27)
    };

    public static GameLoop CreateLoop(PresentationBridge bridge)
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        services.Pawns.Subscribe<PawnSpawnedEvent>(e =>
            bridge.Enqueue(new PawnSpawnedCommand(e.PawnId, e.X, e.Y)));
        services.Pawns.Subscribe<PawnMovedEvent>(e =>
            bridge.Enqueue(new PawnMovedCommand(e.PawnId, e.X, e.Y)));
        services.Combat.Subscribe<DeathEvent>(e =>
            bridge.Enqueue(new PawnDiedCommand(e.Who)));
        services.Pawns.Subscribe<PawnStateChangedEvent>(e =>
            bridge.Enqueue(new PawnStateCommand(
                e.PawnId, e.Name, e.Hunger, e.Thirst, e.Rest, e.Comfort,
                e.Mood, e.JobLabel, e.JobUrgent)));

        SpawnInitialPawns(world, services);

        var navGrid = new NavGrid(50, 50);
        var obstacleRng = new Random(42);
        for (int i = 0; i < 50; i++)
        {
            int ox = obstacleRng.Next(0, 50);
            int oy = obstacleRng.Next(0, 50);
            navGrid.SetTile(ox, oy, passable: false);
        }
        var pathfinding = new AStarPathfinding(navGrid);

        var graph = new DependencyGraph();
        graph.AddSystem(new NeedsSystem());
        graph.AddSystem(new MoodSystem());
        graph.AddSystem(new JobSystem());
        graph.AddSystem(new MovementSystem(pathfinding));
        graph.AddSystem(new PawnStateReporterSystem());
        graph.AddSystem(new InventorySystem());
        graph.AddSystem(new HaulSystem());
        graph.AddSystem(new ElectricGridSystem());
        // ConverterSystem registration deferred — see TODO in ROADMAP.md
        // (cycle ElectricGrid ↔ Converter requires [Deferred] event coupling).
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            ticks,
            world,
            faultSink: null,
            services:  services);

        return new GameLoop(scheduler, bridge);
    }

    private static void SpawnInitialPawns(World world, GameServices services)
    {
        foreach (GridVector pos in InitialPawnPositions)
        {
            EntityId id = world.CreateEntity();
            world.AddComponent(id, new PositionComponent { Position = pos });
            world.AddComponent(id, new NeedsComponent { Hunger = 0.1f, Thirst = 0.1f, Rest = 0.1f });
            world.AddComponent(id, new MindComponent());
            world.AddComponent(id, new JobComponent { Current = JobKind.Idle });
            world.AddComponent(id, new MovementComponent());

            services.Pawns.Publish(new PawnSpawnedEvent
            {
                PawnId = id,
                X      = pos.X,
                Y      = pos.Y
            });
        }
    }
}
