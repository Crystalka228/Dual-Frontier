using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
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

// Namespace is deliberately outside the DualFrontier.Systems.* tree so the
// unqualified names World, Pawn, etc. are not shadowed by the sub-namespaces
// DualFrontier.Systems.World and DualFrontier.Systems.Pawn during lookup.
namespace DualFrontier.IntegrationTests.PawnDomain;

/// <summary>
/// Locks the autonomous consume loop contract: pawns with JobKind.Eat AND no
/// Job.Target select the nearest matching consumable; pawns at the target's
/// tile apply restoration + decrement Charges + reset Job/Movement state.
/// </summary>
public sealed class ConsumeSystemTests
{
    [Fact]
    public void EatingPawn_WithoutTarget_SelectsNearestFood()
    {
        var (world, scheduler) = BuildConsumeSystem();
        EntityId pawn = SpawnEatingPawn(world, satiety: 0.1f, position: new GridVector(10, 10));
        EntityId nearFood = SpawnFood(world, position: new GridVector(11, 10));
        SpawnFood(world, position: new GridVector(50, 50));

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<JobComponent>(pawn, out var job).Should().BeTrue();
        job.Target.Should().Be(nearFood,
            "pawn must select nearest food, not the further one");
    }

    [Fact]
    public void EatingPawn_AtFoodTile_AppliesRestorationAndDecrementsCharges()
    {
        var (world, scheduler) = BuildConsumeSystem();
        var foodPos = new GridVector(20, 20);
        EntityId pawn = SpawnEatingPawn(world, satiety: 0.1f, position: foodPos);
        EntityId food = SpawnFood(world, position: foodPos, charges: 3);

        // Pre-set the pawn's job and movement target so we skip target selection
        // and exercise the arrival branch directly.
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Eat, Target = food });
        world.AddComponent(pawn, new MovementComponent { Target = foodPos });

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        // Tolerance accommodates one tick of NeedsSystem depletion
        // (~0.002 Satiety) which happens in the same tick as restoration.
        needs.Satiety.Should().BeApproximately(0.1f + 0.4f, 0.005f,
            "Satiety must increase by food's RestorationAmount=0.4 (minus one tick of depletion)");

        world.TryGetComponent<ConsumableComponent>(food, out var consumable).Should().BeTrue();
        consumable.Charges.Should().Be(2, "Charges must decrement by 1");
    }

    [Fact]
    public void EatingPawn_AfterConsume_ReturnsToIdle()
    {
        var (world, scheduler) = BuildConsumeSystem();
        var foodPos = new GridVector(15, 15);
        EntityId pawn = SpawnEatingPawn(world, satiety: 0.1f, position: foodPos);
        EntityId food = SpawnFood(world, position: foodPos, charges: 1);
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Eat, Target = food });
        world.AddComponent(pawn, new MovementComponent { Target = foodPos });

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<JobComponent>(pawn, out var job).Should().BeTrue();
        job.Current.Should().Be(JobKind.Idle, "Job clears to Idle after consume");
        job.Target.Should().BeNull("Job.Target clears after consume");

        world.TryGetComponent<MovementComponent>(pawn, out var move).Should().BeTrue();
        move.Target.Should().BeNull("Movement.Target clears after consume");
    }

    [Fact]
    public void ThirstyPawn_PrioritizesWaterSourceOverPackagedDrink()
    {
        var (world, scheduler) = BuildConsumeSystem();
        EntityId pawn = SpawnEatingPawn(world, hydration: 0.1f, position: new GridVector(10, 10));
        EntityId waterSource = SpawnWater(world, position: new GridVector(15, 10));
        SpawnFood(world, position: new GridVector(11, 10), restores: NeedKind.Hydration);

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<JobComponent>(pawn, out var job).Should().BeTrue();
        job.Target.Should().Be(waterSource,
            "WaterSourceComponent prioritised over ConsumableComponent with " +
            "RestoresKind=Hydration, even when packaged drink is closer");
    }

    [Fact]
    public void EatingPawn_AtWaterTile_RestoresHydration_NoChargesChange()
    {
        var (world, scheduler) = BuildConsumeSystem();
        var waterPos = new GridVector(25, 25);
        EntityId pawn = SpawnEatingPawn(world, hydration: 0.1f, position: waterPos);
        EntityId water = SpawnWater(world, position: waterPos);
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Eat, Target = water });
        world.AddComponent(pawn, new MovementComponent { Target = waterPos });

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        // Tolerance accommodates one tick of NeedsSystem depletion
        // (~0.0015 Hydration) which happens in the same tick as restoration.
        needs.Hydration.Should().BeApproximately(0.1f + 0.5f, 0.005f,
            "Hydration must increase by water's RestorationAmount=0.5 (minus one tick of depletion)");

        // Water source has no Charges field — entity unchanged.
        world.TryGetComponent<WaterSourceComponent>(water, out var ws).Should().BeTrue();
        ws.RestorationAmount.Should().Be(0.5f);
    }

    [Fact]
    public void ConsumeSystem_IgnoresExhaustedFoodEntities()
    {
        var (world, scheduler) = BuildConsumeSystem();
        EntityId pawn = SpawnEatingPawn(world, satiety: 0.1f, position: new GridVector(10, 10));
        SpawnFood(world, position: new GridVector(11, 10), charges: 0); // closer but exhausted
        EntityId aliveFood = SpawnFood(world, position: new GridVector(15, 10), charges: 1);

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<JobComponent>(pawn, out var job).Should().BeTrue();
        job.Target.Should().Be(aliveFood,
            "Charges <= 0 entities filtered out even when closer");
    }

    private static (World world, ParallelSystemScheduler scheduler) BuildConsumeSystem()
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        var graph = new DependencyGraph();
        // ConsumeSystem publishes PawnConsumeTarget / PawnConsumeFinished /
        // NeedsRestored events; the writes land in the captured contexts of
        // the component owners (NeedsSystem / JobSystem / MovementSystem) at
        // each phase boundary, so all four must be registered for the loop
        // to close inside the test.
        graph.AddSystem(new NeedsSystem());
        graph.AddSystem(new JobSystem());
        graph.AddSystem(new MovementSystem(new NoOpPathfinding()));
        graph.AddSystem(new ConsumeSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world, faultSink: null, services: services);

        return (world, scheduler);
    }

    /// <summary>
    /// Test stub — the 6 facts here either skip pathfinding entirely
    /// (pawn already at target) or terminate before the second tick where
    /// MovementSystem would actually invoke this. Returning false with an
    /// empty path keeps MovementSystem's "external target unreachable"
    /// branch from rewriting any state we care about.
    /// </summary>
    private sealed class NoOpPathfinding : IPathfindingService
    {
        public bool TryFindPath(GridVector from, GridVector to, out IReadOnlyList<GridVector> path)
        {
            path = System.Array.Empty<GridVector>();
            return false;
        }
    }

    private static EntityId SpawnEatingPawn(
        World world,
        float satiety   = 0.9f,
        float hydration = 0.9f,
        GridVector? position = null)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new PositionComponent { Position = position ?? new GridVector(0, 0) });
        world.AddComponent(id, new NeedsComponent
        {
            Satiety = satiety, Hydration = hydration, Sleep = 0.9f, Comfort = 1.0f
        });
        world.AddComponent(id, new JobComponent { Current = JobKind.Eat });
        world.AddComponent(id, new MovementComponent());
        return id;
    }

    private static EntityId SpawnFood(
        World world,
        GridVector position,
        int charges = 1,
        NeedKind restores = NeedKind.Satiety)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new PositionComponent { Position = position });
        world.AddComponent(id, new ConsumableComponent
        {
            RestoresKind      = restores,
            RestorationAmount = 0.4f,
            Charges           = charges,
        });
        return id;
    }

    private static EntityId SpawnWater(World world, GridVector position)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new PositionComponent { Position = position });
        world.AddComponent(id, new WaterSourceComponent { RestorationAmount = 0.5f });
        return id;
    }
}
