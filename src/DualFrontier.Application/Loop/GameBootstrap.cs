using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Events.Combat;
using DualFrontier.Events.Pawn;
using DualFrontier.Systems.Pawn;

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

        var graph = new DependencyGraph();
        graph.AddSystem(new NeedsSystem());
        graph.AddSystem(new MoodSystem());
        graph.AddSystem(new JobSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            ticks,
            world,
            faultSink: null,
            services:  services);

        return new GameLoop(scheduler, bridge);
    }
}
