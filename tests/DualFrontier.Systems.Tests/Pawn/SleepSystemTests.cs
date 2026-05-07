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

// Namespace mirrors ConsumeSystemTests so the unqualified names World, Pawn,
// etc. are not shadowed by the sub-namespaces DualFrontier.Systems.World and
// DualFrontier.Systems.Pawn during type lookup.
namespace DualFrontier.IntegrationTests.PawnDomain;

/// <summary>
/// Locks the autonomous sleep loop contract: pawns с JobKind.Sleep find the
/// nearest unoccupied bed, claim it on arrival, restore Sleep + Comfort
/// (hybrid 30%) each tick, и release the bed once Sleep ≥ 0.95.
/// </summary>
public sealed class SleepSystemTests
{
    [Fact]
    public void SleepingPawn_AtBedTile_ClaimsBed()
    {
        var (world, scheduler) = BuildSleepSystem();
        var bedPos = new GridVector(20, 20);
        EntityId pawn = SpawnSleepingPawn(world, sleep: 0.1f, position: bedPos);
        EntityId bed  = SpawnBed(world, position: bedPos);

        world.AddComponent(pawn, new JobComponent { Current = JobKind.Sleep, Target = bed });
        world.AddComponent(pawn, new MovementComponent { Target = bedPos });

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<BedComponent>(bed, out var bedComp).Should().BeTrue();
        bedComp.Occupant.Should().Be(pawn,
            "pawn standing on bed's tile claims it during the arrival phase");
    }

    [Fact]
    public void OccupiedBed_RestoresSleepEachTick()
    {
        var (world, scheduler) = BuildSleepAndNeedsSystem();
        EntityId pawn = SpawnSleepingPawn(world, sleep: 0.1f, position: new GridVector(10, 10));
        EntityId bed  = SpawnBed(world, position: new GridVector(10, 10), restorationRate: 0.005f);
        world.TryGetComponent<BedComponent>(bed, out var bedComp);
        bedComp.Occupant = pawn;
        world.AddComponent(bed, bedComp);
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Sleep, Target = bed });
        world.AddComponent(pawn, new MovementComponent { Target = new GridVector(10, 10) });

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        // Tolerance 0.002 covers one tick of NeedsSystem depletion (~0.001
        // Sleep) plus float-precision noise around the +0.005 restoration.
        needs.Sleep.Should().BeApproximately(0.1f + 0.005f, 0.002f,
            "Sleep must increase by bed.SleepRestorationPerTick (minus one tick of depletion)");
    }

    [Fact]
    public void OccupiedBed_RestoresComfortAt30PercentRate()
    {
        var (world, scheduler) = BuildSleepAndNeedsSystem();
        EntityId pawn = SpawnSleepingPawn(world, sleep: 0.1f, comfort: 0.5f,
            position: new GridVector(10, 10));
        EntityId bed = SpawnBed(world, position: new GridVector(10, 10), restorationRate: 0.005f);
        world.TryGetComponent<BedComponent>(bed, out var bedComp);
        bedComp.Occupant = pawn;
        world.AddComponent(bed, bedComp);
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Sleep, Target = bed });
        world.AddComponent(pawn, new MovementComponent { Target = new GridVector(10, 10) });

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out var needs).Should().BeTrue();
        // ΔComfort = ΔSleep × 0.3 = 0.005 × 0.3 = 0.0015 (master plan AD-3 hybrid).
        // Tolerance 0.002 covers same-tick Comfort depletion (~0.0005) plus
        // float noise.
        needs.Comfort.Should().BeApproximately(0.5f + 0.005f * 0.3f, 0.002f,
            "Comfort must increase at 30% of Sleep restoration rate");
    }

    [Fact]
    public void SleepingPawn_AtFullSleep_WakesAndReleasesBed()
    {
        var (world, scheduler) = BuildSleepAndNeedsSystem();
        EntityId pawn = SpawnSleepingPawn(world, sleep: 0.96f, position: new GridVector(10, 10));
        EntityId bed  = SpawnBed(world, position: new GridVector(10, 10));
        world.TryGetComponent<BedComponent>(bed, out var bedComp);
        bedComp.Occupant = pawn;
        world.AddComponent(bed, bedComp);
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Sleep, Target = bed });
        world.AddComponent(pawn, new MovementComponent { Target = new GridVector(10, 10) });

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<BedComponent>(bed, out var afterBed).Should().BeTrue();
        afterBed.Occupant.Should().BeNull("bed releases occupant once Sleep ≥ 0.95");

        world.TryGetComponent<JobComponent>(pawn, out var afterJob).Should().BeTrue();
        afterJob.Current.Should().Be(JobKind.Idle, "JobSystem clears Sleep job after wake");
        afterJob.Target.Should().BeNull("JobSystem clears Job.Target after wake");
    }

    [Fact]
    public void SleepingPawn_NoBedsInWorld_StaysWithoutTarget()
    {
        var (world, scheduler) = BuildSleepSystem();
        EntityId pawn = SpawnSleepingPawn(world, sleep: 0.1f, position: new GridVector(10, 10));
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Sleep });
        world.AddComponent(pawn, new MovementComponent());

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<JobComponent>(pawn, out var job).Should().BeTrue();
        job.Target.Should().BeNull("no bed exists, so SleepSystem cannot publish a target");
    }

    [Fact]
    public void SleepingPawn_PrefersNearestUnoccupiedBed()
    {
        var (world, scheduler) = BuildSleepAndNeedsSystem();
        EntityId pawn    = SpawnSleepingPawn(world, sleep: 0.1f, position: new GridVector(10, 10));
        EntityId nearBed = SpawnBed(world, position: new GridVector(11, 10));
        SpawnBed(world, position: new GridVector(50, 50));
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Sleep });
        world.AddComponent(pawn, new MovementComponent());

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<JobComponent>(pawn, out var job).Should().BeTrue();
        job.Target.Should().Be(nearBed, "pawn must select nearest bed, not the further one");
    }

    [Fact]
    public void OccupiedBeds_FilteredOut_OfTargetSelection()
    {
        var (world, scheduler) = BuildSleepAndNeedsSystem();
        EntityId pawn         = SpawnSleepingPawn(world, sleep: 0.1f, position: new GridVector(10, 10));
        EntityId occupiedNear = SpawnBed(world, position: new GridVector(11, 10));
        // Dummy occupant must carry NeedsComponent — Phase 1 of SleepSystem
        // reads it for every pawn occupying a bed (restoration / wake check).
        EntityId dummyOccupant = SpawnSleepingPawn(world, sleep: 0.5f,
            position: new GridVector(11, 10));
        world.TryGetComponent<BedComponent>(occupiedNear, out var occBed);
        occBed.Occupant = dummyOccupant;
        world.AddComponent(occupiedNear, occBed);
        EntityId emptyFar = SpawnBed(world, position: new GridVector(20, 20));
        world.AddComponent(pawn, new JobComponent { Current = JobKind.Sleep });
        world.AddComponent(pawn, new MovementComponent());

        scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<JobComponent>(pawn, out var job).Should().BeTrue();
        job.Target.Should().Be(emptyFar,
            "occupied beds must be excluded even when closer to the pawn");
    }

    // --- Helpers -----------------------------------------------------------

    private static (World world, ParallelSystemScheduler scheduler) BuildSleepSystem()
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        var graph = new DependencyGraph();
        graph.AddSystem(new SleepSystem());
        graph.AddSystem(new JobSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world, faultSink: null, services: services);
        return (world, scheduler);
    }

    private static (World world, ParallelSystemScheduler scheduler) BuildSleepAndNeedsSystem()
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        var graph = new DependencyGraph();
        graph.AddSystem(new NeedsSystem());
        graph.AddSystem(new JobSystem());
        graph.AddSystem(new SleepSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world, faultSink: null, services: services);
        return (world, scheduler);
    }

    private static EntityId SpawnSleepingPawn(
        World world,
        float sleep    = 0.1f,
        float comfort  = 1f,
        GridVector? position = null)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new PositionComponent { Position = position ?? new GridVector(0, 0) });
        world.AddComponent(id, new NeedsComponent
        {
            Satiety   = 0.9f,
            Hydration = 0.9f,
            Sleep     = sleep,
            Comfort   = comfort,
        });
        return id;
    }

    private static EntityId SpawnBed(
        World world,
        GridVector position,
        float restorationRate = 0.005f)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new PositionComponent { Position = position });
        world.AddComponent(id, new BedComponent
        {
            Occupant                = null,
            SleepRestorationPerTick = restorationRate,
        });
        return id;
    }
}
