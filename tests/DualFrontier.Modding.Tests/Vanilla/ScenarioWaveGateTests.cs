using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Distribution;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using DualFrontier.Modding.Tests.Sdk;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Vanilla;

/// <summary>
/// W4 wave gate — the vanilla colony exists because a MOD put it there.
///
/// <para>
/// Everything asserted here used to be done by the engine's composition root before it handed
/// anything to the session: 21 component registrations, two spawn factories, a walkability grid,
/// a pathfinding service and ten system registrations. The engine referenced the game's four
/// assemblies for exactly that. These facts prove the same colony now arrives through the
/// ordinary mod pipeline instead, which is what makes cutting those references possible rather
/// than merely desirable.
/// </para>
///
/// <para>
/// Nothing here is a double except the presentation sink. The loader, the validator, the
/// capability ledger, the scheduler and a registry-backed world are all production objects, and
/// the mod is loaded from its deployed output exactly as an install would load it.
/// </para>
/// </summary>
public sealed class ScenarioWaveGateTests : IDisposable
{
    private static string FixturesRoot => Path.Combine(AppContext.BaseDirectory, "Fixtures");
    private static string ScenarioPath => Path.Combine(FixturesRoot, "DualFrontier.Mod.Vanilla.Scenario");

    private static readonly ScenarioConfig Scenario = new(
        Id: "gate",
        WorldSeed: 0,
        MapWidth: 60,
        MapHeight: 60,
        ObstacleCount: 80,
        ObstacleSeed: 42,
        FactorySeed: 42,
        ItemFactorySeed: 43,
        Counts: new ScenarioCounts(Pawns: 12, Food: 9, Water: 5, Beds: 4, Decorations: 3));

    private readonly NativeWorld _world;
    private readonly ModRegistry _registry;
    private readonly TickScheduler _ticks;
    private readonly ParallelSystemScheduler _scheduler;
    private readonly ModIntegrationPipeline _pipeline;
    private readonly RecordingPresentationSink _sink = new();

    public ScenarioWaveGateTests()
    {
        _world = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        _registry = new ModRegistry();
        _registry.SetCoreSystems(Array.Empty<SystemBase>());
        _ticks = new TickScheduler();
        _registry.SetTickSource(() => _ticks.CurrentTick);
        _registry.SetPresentationSink(_sink);
        _registry.SetScenario(Scenario);

        var graph = new DependencyGraph();
        graph.Build();

        var services = new GameServices();
        _scheduler = SchedulerTestFixture.BuildIsolated(
            graph.GetPhases(), _ticks, _world, services: services);

        _pipeline = new ModIntegrationPipeline(
            new ModLoader(), _registry, new ContractValidator(), new ModContractStore(),
            services, _scheduler, new ModFaultHandler(), _world.Registry);
    }

    public void Dispose() => _world.Dispose();

    private PipelineResult Apply() => _pipeline.Apply(new[] { ScenarioPath });

    [Fact]
    public void TheScenarioModLoadsAndSeedsTheColonyTheDistributionAskedFor()
    {
        PipelineResult result = Apply();

        result.Success.Should().BeTrue(
            "the scenario mod is the product's content. Failures: " +
            string.Join("; ", result.Errors.Select(e => e.Kind + ":" + e.Message)));

        // Colonists carry an identity; items do not. Counting identities counts the colony.
        Ids<IdentityComponent>(_world).Should().HaveCount(Scenario.Counts.Pawns,
            "the count comes from the distribution manifest, not from a constant in the engine");

        Ids<ConsumableComponent>(_world).Should().HaveCount(Scenario.Counts.Food);
        Ids<WaterSourceComponent>(_world).Should().HaveCount(Scenario.Counts.Water);
        Ids<BedComponent>(_world).Should().HaveCount(Scenario.Counts.Beds);
        Ids<DecorativeAuraComponent>(_world).Should().HaveCount(Scenario.Counts.Decorations);
    }

    [Fact]
    public void EveryColonistIsFullyFormed()
    {
        Apply().Success.Should().BeTrue();

        List<EntityId> pawns = Ids<IdentityComponent>(_world);
        foreach (EntityId pawn in pawns)
        {
            _world.TryGetComponent(pawn, out IdentityComponent identity).Should().BeTrue();
            string? name = _world.Resolve(identity.Name);
            name.Should().NotBeNullOrWhiteSpace("a colonist has a name");
            name!.Should().Contain(" ", "forename and surname");

            _world.TryGetComponent(pawn, out PositionComponent _).Should().BeTrue("placed on a tile");
            _world.TryGetComponent(pawn, out SkillsComponent skills).Should().BeTrue();
            skills.IsInitialized.Should().BeTrue("skills were rolled, not left at the default");
            _world.TryGetComponent(pawn, out MovementComponent movement).Should().BeTrue();
            movement.Path.IsValid.Should().BeTrue("the path composite is minted at spawn");
        }
    }

    [Fact]
    public void NoTwoStartingEntitiesShareATile()
    {
        Apply().Success.Should().BeTrue();

        var occupied = new List<(int X, int Y)>();
        foreach (EntityId id in AllStartingEntities(_world))
        {
            _world.TryGetComponent(id, out PositionComponent p).Should().BeTrue();
            occupied.Add((p.Position.X, p.Position.Y));
        }

        occupied.Should().OnlyHaveUniqueItems(
            "colonists are excluded from the item pool, and each pool is a prefix of one shuffle — " +
            "a repeat means the exclusion or the cursor broke");
    }

    [Fact]
    public void SeedingIsIdempotent_ASecondApplyDoesNotDoubleTheColony()
    {
        Apply().Success.Should().BeTrue();
        int afterFirst = Ids<IdentityComponent>(_world).Count;

        // Unload and re-apply: the seeder is registered again and must find the colony already
        // there. Without the read-then-mint check this doubles the population rather than
        // resuming it, which is the failure mode a lazily-seeding system exists to avoid.
        _pipeline.UnloadMod("dualfrontier.vanilla.scenario");
        Apply().Success.Should().BeTrue();

        Ids<IdentityComponent>(_world).Should().HaveCount(afterFirst,
            "a reload resumes the world, it does not re-seed it");
    }

    [Fact]
    public void TheColonyRendersItselfThroughThePresentationSurface()
    {
        Apply().Success.Should().BeTrue();
        _sink.Shown.Should().BeEmpty("nothing is drawn until a tick runs");

        _scheduler.ExecuteTick(1f / 30f);

        _sink.Shown.Should().HaveCount(Scenario.Counts.Pawns,
            "the presentation system reports every colonist on its first tick. It reads the " +
            "identity span rather than subscribing to spawn events, so items are deliberately " +
            "not drawn — which is exactly what shipped before this wave, where the renderer's " +
            "item handler was an empty stub");
    }

    [Fact]
    public void MovingColonistsAreReportedOnceEach()
    {
        Apply().Success.Should().BeTrue();
        _scheduler.ExecuteTick(1f / 30f);
        _sink.Moved.Clear();

        for (int i = 0; i < 30; i++)
            _scheduler.ExecuteTick(1f / 30f);

        _sink.Shown.Should().HaveCount(Scenario.Counts.Pawns,
            "no colonist is announced twice — the diff reports a change, not a state");
        _sink.Moved.Should().NotBeEmpty(
            "the colony is alive: movement runs, and the presentation diff notices");
    }

    [Fact]
    public void TheTenGameplaySystemsAreRegisteredByTheModNotTheEngine()
    {
        Apply().Success.Should().BeTrue();

        IReadOnlyList<SystemRegistration> all = _registry.GetAllSystems();

        all.Should().OnlyContain(r => r.Origin == SystemOrigin.Mod,
            "the engine registers nothing — the core set is empty by construction");
        all.Select(r => r.Instance.GetType().Name).Should().Contain("MovementSystem",
            "movement takes a pathfinding service at construction, so it is the one system the " +
            "parameterless registration path could never have expressed");
        _registry.GetCoreSystemInstances().Should().BeEmpty();
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static List<EntityId> Ids<T>(NativeWorld world) where T : unmanaged, IComponent
    {
        var ids = new List<EntityId>();
        using SpanLease<T> lease = world.AcquireSpan<T>();
        ReadOnlySpan<int> indices = lease.Indices;
        ReadOnlySpan<int> versions = lease.Versions;
        for (int i = 0; i < lease.Count; i++)
            ids.Add(new EntityId(indices[i], versions[indices[i]]));
        return ids;
    }

    private static List<EntityId> AllStartingEntities(NativeWorld world)
    {
        var ids = new List<EntityId>();
        ids.AddRange(Ids<IdentityComponent>(world));
        ids.AddRange(Ids<ConsumableComponent>(world));
        ids.AddRange(Ids<WaterSourceComponent>(world));
        ids.AddRange(Ids<BedComponent>(world));
        ids.AddRange(Ids<DecorativeAuraComponent>(world));
        return ids;
    }
}
