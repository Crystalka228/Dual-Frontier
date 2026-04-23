using DualFrontier.Application.Bridge;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Systems.Pawn;

namespace DualFrontier.Application.Loop;

/// <summary>
/// Production entry point that assembles the simulation graph: a World,
/// the IGameServices aggregator, the TickScheduler, and the dependency
/// graph of registered systems. Returns a ready-to-start
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
