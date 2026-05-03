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

/// <summary>
/// Locks the post-honesty-pass decay-direction contract: needs grow
/// over time, representing accumulating deficit. The previous (incorrect)
/// direction was decay-toward-0, which falsely implied automatic
/// recovery. See HOUSEKEEPING_NEEDS_DECAY_DIRECTION.md for context.
/// </summary>
public sealed class NeedsAccumulationTests
{
    [Fact]
    public void Hunger_GrowsOverTime_WhenNoRecoveryExists()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(world, hunger: 0.5f);

        // Run several SLOW ticks. Each tick, NeedsSystem should
        // increment Hunger by HungerDecayPerTick (0.002).
        for (int i = 0; i < 10; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Hunger.Should().BeGreaterThan(0.5f,
            "deficit must accumulate without recovery — no module closes needs yet");
    }

    [Fact]
    public void AllFourNeeds_GrowOverTime_WhenNoRecoveryExists()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(
            world, hunger: 0.3f, thirst: 0.3f, rest: 0.3f, comfort: 0.3f);

        for (int i = 0; i < 10; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Hunger.Should().BeGreaterThan(0.3f);
        needs.Thirst.Should().BeGreaterThan(0.3f);
        needs.Rest.Should().BeGreaterThan(0.3f);
        needs.Comfort.Should().BeGreaterThan(0.3f);
    }

    [Fact]
    public void Hunger_ClampsAt1_WhenAlreadyAtCeiling()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(world, hunger: 1.0f);

        for (int i = 0; i < 5; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Hunger.Should().Be(1.0f, "ceiling clamp must hold at 1.0");
    }

    private static (World world, ParallelSystemScheduler scheduler) BuildOneNeedsSystem()
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        var graph = new DependencyGraph();
        graph.AddSystem(new NeedsSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world, faultSink: null, services: services);

        return (world, scheduler);
    }

    private static EntityId SpawnIdleNeedsPawn(
        World world,
        float hunger  = 0f,
        float thirst  = 0f,
        float rest    = 0f,
        float comfort = 0f)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new NeedsComponent
        {
            Hunger = hunger, Thirst = thirst, Rest = rest, Comfort = comfort,
        });
        return id;
    }
}
