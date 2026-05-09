using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Systems.Pawn;
using FluentAssertions;
using Xunit;

// Namespace is deliberately outside the DualFrontier.Systems.* tree so the
// unqualified names World, Pawn, etc. are not shadowed by the sub-namespaces
// DualFrontier.Systems.World and DualFrontier.Systems.Pawn during lookup.
namespace DualFrontier.IntegrationTests.PawnDomain;

public sealed class NeedsJobIntegrationTests
{
    [Fact]
    public void Starving_pawn_receives_Eat_job_after_NeedsCritical_fires_on_the_Pawns_bus()
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        var graph = new DependencyGraph();
        graph.AddSystem(new NeedsSystem());
        graph.AddSystem(new JobSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            ticks,
            world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            services);

        EntityId pawn = world.CreateEntity();
        world.AddComponent(pawn, new NeedsComponent { Satiety = 0.1f });
        world.AddComponent(pawn, new JobComponent  { Current = JobKind.Idle });

        // One tick is enough: NeedsSystem (SLOW) fires on tick 0, publishes
        // NeedsCriticalEvent, JobSystem (NORMAL) runs later in the same tick
        // and drains the urgent set. Running a few more ticks protects
        // against flakes if tick ordering constants change.
        for (int i = 0; i < 5; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<JobComponent>(pawn, out JobComponent job)
             .Should().BeTrue();
        job.Current.Should().Be(JobKind.Eat);
    }
}
