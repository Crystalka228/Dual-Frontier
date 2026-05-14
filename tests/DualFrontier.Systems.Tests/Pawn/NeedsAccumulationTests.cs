using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Systems.Pawn;
using FluentAssertions;
using Xunit;

// Namespace is deliberately outside the DualFrontier.Systems.* tree so the
// unqualified names World, Pawn, etc. are not shadowed by the sub-namespaces
// DualFrontier.Systems.World and DualFrontier.Systems.Pawn during lookup.
namespace DualFrontier.IntegrationTests.PawnDomain;

/// <summary>
/// Locks the wellness-pool depletion contract: needs deplete over time,
/// no recovery exists yet. The earlier (incorrect) direction was decay-
/// toward-0 of a deficit accumulator, which falsely implied automatic
/// recovery; HOUSEKEEPING_NEEDS_DECAY_DIRECTION.md flipped the sign,
/// and TD-3.1 then flipped the storage semantics so the value's name
/// matches its meaning (Satiety = wellness, depletes downward).
/// </summary>
public sealed class NeedsAccumulationTests
{
    [Fact]
    public void Satiety_DepletesOverTime_WhenNoRecoveryExists()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(world, satiety: 0.5f);

        // Run several SLOW ticks. Each tick, NeedsSystem should
        // decrement Satiety by SatietyDepletionPerTick (0.002).
        for (int i = 0; i < 10; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Satiety.Should().BeLessThan(0.5f,
            "wellness must deplete without recovery — no module restores needs yet");
    }

    [Fact]
    public void AllFourNeeds_DepleteOverTime_WhenNoRecoveryExists()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(
            world, satiety: 0.3f, hydration: 0.3f, sleep: 0.3f, comfort: 0.3f);

        for (int i = 0; i < 10; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Satiety.Should().BeLessThan(0.3f);
        needs.Hydration.Should().BeLessThan(0.3f);
        needs.Sleep.Should().BeLessThan(0.3f);
        needs.Comfort.Should().BeLessThan(0.3f);
    }

    [Fact]
    public void Satiety_ClampsAt0_WhenAlreadyAtFloor()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(world, satiety: 0.0f);

        for (int i = 0; i < 5; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Satiety.Should().Be(0.0f, "floor clamp must hold at 0.0");
    }

    private static (World world, ParallelSystemScheduler scheduler) BuildOneNeedsSystem()
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();
        // K8.3+K8.4 Phase 4 — NeedsSystem invokes NativeWorld.BeginBatch for dual-write.
        var nativeWorld = new NativeWorld();

        var graph = new DependencyGraph();
        graph.AddSystem(new NeedsSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world,
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            services,
            nativeWorld);

        return (world, scheduler);
    }

    private static EntityId SpawnIdleNeedsPawn(
        World world,
        float satiety   = 1f,
        float hydration = 1f,
        float sleep     = 1f,
        float comfort   = 1f)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new NeedsComponent
        {
            Satiety = satiety, Hydration = hydration, Sleep = sleep, Comfort = comfort,
        });
        return id;
    }
}
