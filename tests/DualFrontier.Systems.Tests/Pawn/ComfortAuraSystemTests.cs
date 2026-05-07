using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Systems.Pawn;
using FluentAssertions;
using Xunit;

namespace DualFrontier.IntegrationTests.PawnDomain;

/// <summary>
/// Locks the passive ambient Comfort contract: pawns within Chebyshev
/// radius of DecorativeAuraComponent receive ComfortPerTick restoration
/// per SLOW tick. Edge cases: edge-of-radius inclusive, multi-decoration
/// stacking, no-decoration safety.
/// </summary>
public sealed class ComfortAuraSystemTests
{
    [Fact]
    public void PawnInRadius_ReceivesComfort()
    {
        var (world, scheduler) = BuildAuraAndNeedsSystem();
        EntityId pawn = SpawnPawnAt(world, comfort: 0.5f, position: new GridVector(2, 2));
        SpawnDecoration(world,
            position: new GridVector(0, 0), radius: 3, comfortPerTick: 0.001f);

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        // Pawn started at 0.5, ambient adds 0.001, decay subtracts ComfortDepletion.
        // Net delta per tick = +0.001 - 0.0005 = +0.0005. Verify Comfort > 0.5.
        needs.Comfort.Should().BeGreaterThan(0.5f,
            "pawn within Chebyshev radius receives ComfortPerTick — net positive after decay");
    }

    [Fact]
    public void PawnOutsideRadius_DoesNotReceiveComfort()
    {
        var (world, scheduler) = BuildAuraAndNeedsSystem();
        EntityId pawn = SpawnPawnAt(world, comfort: 0.5f, position: new GridVector(10, 10));
        SpawnDecoration(world,
            position: new GridVector(0, 0), radius: 3, comfortPerTick: 0.001f);

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        // Pawn outside radius — only decay applies. Comfort should drop slightly.
        needs.Comfort.Should().BeLessThan(0.5f,
            "pawn outside radius — no restoration, only decay");
    }

    [Fact]
    public void MultipleDecorations_StackComfort()
    {
        var (world, scheduler) = BuildAuraAndNeedsSystem();
        EntityId pawn = SpawnPawnAt(world, comfort: 0.5f, position: new GridVector(0, 0));
        SpawnDecoration(world, position: new GridVector(1, 0), radius: 3, comfortPerTick: 0.001f);
        SpawnDecoration(world, position: new GridVector(0, 1), radius: 3, comfortPerTick: 0.001f);
        SpawnDecoration(world, position: new GridVector(2, 2), radius: 3, comfortPerTick: 0.001f);

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        // 3 decorations × 0.001 each = 0.003 restoration, 0.0005 decay.
        // Net delta = +0.0025. Pawn comfort >= 0.502.
        needs.Comfort.Should().BeGreaterThan(0.502f,
            "3 decorations stack — each publishes its own NeedsRestoredEvent");
    }

    [Fact]
    public void DecorationAtEdgeOfRadius_StillRestores()
    {
        var (world, scheduler) = BuildAuraAndNeedsSystem();
        // Pawn at exactly Chebyshev distance = Radius → inclusive bound
        EntityId pawn = SpawnPawnAt(world, comfort: 0.5f, position: new GridVector(3, 0));
        SpawnDecoration(world, position: new GridVector(0, 0), radius: 3, comfortPerTick: 0.001f);

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        needs.Comfort.Should().BeGreaterThan(0.5f,
            "Chebyshev distance == Radius is inclusive — pawn receives restoration");
    }

    [Fact]
    public void NoDecorations_ComfortDecaysOnly()
    {
        var (world, scheduler) = BuildAuraAndNeedsSystem();
        EntityId pawn = SpawnPawnAt(world, comfort: 0.5f, position: new GridVector(0, 0));
        // No decorations spawned

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        // Only decay applies (no restoration source)
        needs.Comfort.Should().BeLessThan(0.5f,
            "no decorations, no restoration — Comfort decays per NeedsSystem");
    }

    [Fact]
    public void MultiplePawns_AllInRadiusReceiveComfort()
    {
        var (world, scheduler) = BuildAuraAndNeedsSystem();
        EntityId pawnA = SpawnPawnAt(world, comfort: 0.5f, position: new GridVector(1, 0));
        EntityId pawnB = SpawnPawnAt(world, comfort: 0.5f, position: new GridVector(0, 1));
        EntityId pawnC = SpawnPawnAt(world, comfort: 0.5f, position: new GridVector(2, 2));
        SpawnDecoration(world, position: new GridVector(0, 0), radius: 3, comfortPerTick: 0.001f);

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawnA, out var needsA).Should().BeTrue();
        world.TryGetComponent<NeedsComponent>(pawnB, out var needsB).Should().BeTrue();
        world.TryGetComponent<NeedsComponent>(pawnC, out var needsC).Should().BeTrue();

        needsA.Comfort.Should().BeGreaterThan(0.5f, "pawnA within radius");
        needsB.Comfort.Should().BeGreaterThan(0.5f, "pawnB within radius");
        needsC.Comfort.Should().BeGreaterThan(0.5f, "pawnC within radius (Chebyshev=2 ≤ 3)");
    }

    // --- Helpers -----------------------------------------------------------

    private static (World world, ParallelSystemScheduler scheduler) BuildAuraAndNeedsSystem()
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        var graph = new DependencyGraph();
        graph.AddSystem(new NeedsSystem());
        graph.AddSystem(new ComfortAuraSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world, faultSink: null, services: services);
        return (world, scheduler);
    }

    private static EntityId SpawnPawnAt(
        World world,
        float comfort = 1f,
        GridVector? position = null)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new PositionComponent { Position = position ?? new GridVector(0, 0) });
        world.AddComponent(id, new NeedsComponent
        {
            Satiety = 0.9f, Hydration = 0.9f, Sleep = 0.9f, Comfort = comfort
        });
        return id;
    }

    private static EntityId SpawnDecoration(
        World world,
        GridVector position,
        int radius,
        float comfortPerTick)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new PositionComponent { Position = position });
        world.AddComponent(id, new DecorativeAuraComponent
        {
            Radius         = radius,
            ComfortPerTick = comfortPerTick,
        });
        return id;
    }
}
