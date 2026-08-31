using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using DualFrontier.Application.Modding;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Distribution;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;
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
/// ordinary mod pipeline instead, which is what made cutting those references possible rather
/// than merely desirable.
/// </para>
///
/// <para>
/// The placement RULES the seeding obeys are pinned next door, in ScenarioSeedingTests. This
/// class is about the colony existing at all, and about the ten gameplay systems the mod
/// registered actually running.
/// </para>
/// </summary>
[Collection("GameLoopSerial")]
public sealed class ScenarioWaveGateTests : IDisposable
{
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

    private readonly ScenarioHarness _h = new(Scenario);

    public void Dispose() => _h.Dispose();

    private PipelineResult Apply() => _h.Apply();

    [Fact]
    public void TheScenarioModLoadsAndSeedsTheColonyTheDistributionAskedFor()
    {
        PipelineResult result = Apply();

        result.Success.Should().BeTrue(
            "the scenario mod is the product's content. Failures: " +
            string.Join("; ", result.Errors.Select(e => e.Kind + ":" + e.Message)));

        // Colonists carry an identity; items do not. Counting identities counts the colony.
        ScenarioHarness.Ids<IdentityComponent>(_h.World).Should().HaveCount(Scenario.Counts.Pawns,
            "the count comes from the distribution manifest, not from a constant in the engine");

        ScenarioHarness.Ids<ConsumableComponent>(_h.World).Should().HaveCount(Scenario.Counts.Food);
        ScenarioHarness.Ids<WaterSourceComponent>(_h.World).Should().HaveCount(Scenario.Counts.Water);
        ScenarioHarness.Ids<BedComponent>(_h.World).Should().HaveCount(Scenario.Counts.Beds);
        ScenarioHarness.Ids<DecorativeAuraComponent>(_h.World).Should().HaveCount(Scenario.Counts.Decorations);
    }

    [Fact]
    public void TheModRegistersTheWholeVanillaComponentSet()
    {
        // Relocated from the engine's own round-trip suite, which asserted this against a
        // registration helper the engine owned. The set is content, so the claim belongs to the
        // mod that ships it, registered through IModApi during the mod's initialisation, into a
        // registry the world hands over EMPTY.
        _h.World.Registry!.Count.Should().Be(0, "the engine registers no component type of its own");

        Apply().Success.Should().BeTrue();

        // 22 = the 21 vanilla content types + ScenarioSeededComponent. The mark is registered
        // apart from VanillaComponents and counted apart from it here, because it is not content:
        // it is how the seeder recognises a world it has already written. Rolling it into the
        // vanilla figure would make "the vanilla component set" a number that no longer means
        // what it says.
        _h.World.Registry!.Count.Should().Be(22,
            "the vanilla component set is 21 types -- FactionComponent and WorkbenchComponent are " +
            "deliberately absent because nothing constructs or reads either -- plus the scenario " +
            "mod's own seeding mark");
    }

    [Fact]
    public void EveryColonistIsFullyFormed()
    {
        Apply().Success.Should().BeTrue();

        foreach (EntityId pawn in ScenarioHarness.Ids<IdentityComponent>(_h.World))
        {
            _h.World.TryGetComponent(pawn, out IdentityComponent identity).Should().BeTrue();
            string? name = _h.World.Resolve(identity.Name);
            name.Should().NotBeNullOrWhiteSpace("a colonist has a name");
            name!.Should().Contain(" ", "forename and surname");

            _h.World.TryGetComponent(pawn, out PositionComponent _).Should().BeTrue("placed on a tile");
            _h.World.TryGetComponent(pawn, out SkillsComponent skills).Should().BeTrue();
            skills.IsInitialized.Should().BeTrue("skills were rolled, not left at the default");
            _h.World.TryGetComponent(pawn, out MovementComponent movement).Should().BeTrue();
            movement.Path.IsValid.Should().BeTrue("the path composite is minted at spawn");
        }
    }

    [Fact]
    public void NoTwoStartingEntitiesShareATile()
    {
        Apply().Success.Should().BeTrue();

        var occupied = new List<(int X, int Y)>();
        foreach (EntityId id in ScenarioHarness.AllStartingEntities(_h.World))
        {
            _h.World.TryGetComponent(id, out PositionComponent p).Should().BeTrue();
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
        int afterFirst = ScenarioHarness.Ids<IdentityComponent>(_h.World).Count;

        // Unload and re-apply: the seeder is registered again and must find the colony already
        // there. Without the read-then-mint check this doubles the population rather than
        // resuming it, which is the failure mode a lazily-seeding system exists to avoid.
        _h.Pipeline.UnloadMod("dualfrontier.vanilla.scenario");
        Apply().Success.Should().BeTrue();

        ScenarioHarness.Ids<IdentityComponent>(_h.World).Should().HaveCount(afterFirst,
            "a reload resumes the world, it does not re-seed it");
    }

    [Fact]
    public void TheColonyRendersItselfThroughThePresentationSurface()
    {
        Apply().Success.Should().BeTrue();
        _h.Sink.Shown.Should().BeEmpty("nothing is drawn until a tick runs");

        _h.Scheduler.ExecuteTick(1f / 30f);

        _h.Sink.Shown.Should().HaveCount(Scenario.Counts.Pawns,
            "the presentation system reports every colonist on its first tick. It reads the " +
            "identity span rather than subscribing to spawn events, so items are deliberately " +
            "not drawn — which is exactly what shipped before this wave, where the renderer's " +
            "item handler was empty");
    }

    [Fact]
    public void UnloadingTheScenarioRetractsEverySpriteItDrew()
    {
        // The renderer holds its own registration per entity, so forgetting the ids mod-side
        // retracts nothing. Unloading the scenario used to leave the whole colony drawn on a scene
        // the simulation had stopped maintaining, with nothing left running that could ever notice
        // those entities gone -- "unload removes the mechanic" has to include what the mechanic
        // drew. The entities themselves deliberately survive an unload; the SPRITES must not.
        Apply().Success.Should().BeTrue();
        _h.Scheduler.ExecuteTick(1f / 30f);

        _h.Sink.Shown.Should().HaveCount(Scenario.Counts.Pawns, "precondition: the colony is drawn");
        _h.Sink.Hidden.Should().BeEmpty("nothing has been retracted yet");

        _h.Pipeline.UnloadMod("dualfrontier.vanilla.scenario");

        _h.Sink.Hidden.Should().BeEquivalentTo(
            _h.Sink.Shown.ConvertAll(s => s.Entity),
            "every sprite this mod announced must be retracted when the mod goes away");
    }

    [Fact]
    public void MovingColonistsAreReportedOnceEach()
    {
        Apply().Success.Should().BeTrue();
        _h.Scheduler.ExecuteTick(1f / 30f);
        _h.Sink.Moved.Clear();

        for (int i = 0; i < 30; i++)
            _h.Scheduler.ExecuteTick(1f / 30f);

        _h.Sink.Shown.Should().HaveCount(Scenario.Counts.Pawns,
            "no colonist is announced twice — the diff reports a change, not a state");
        _h.Sink.Moved.Should().NotBeEmpty(
            "the colony is alive: movement runs, and the presentation diff notices");
    }

    [Fact]
    public void TheTenGameplaySystemsAreRegisteredByTheModNotTheEngine()
    {
        Apply().Success.Should().BeTrue();

        IReadOnlyList<SystemRegistration> all = _h.Registry.GetAllSystems();

        all.Should().OnlyContain(r => r.Origin == SystemOrigin.Mod,
            "the engine registers nothing — the core set is empty by construction");
        all.Select(r => r.Instance.GetType().Name).Should().Contain("MovementSystem",
            "movement takes a pathfinding service at construction, so it is the one system the " +
            "parameterless registration path could never have expressed");
        _h.Registry.GetCoreSystemInstances().Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void TheReporterSystemDescribesEveryColonistOnItsSlowTick()
    {
        // Relocated, and moved DOWN a level. Two engine-side tests used to assert this on the
        // PawnStateCommand the composition root produced by subscribing to this very event; the
        // command's handler read none of its fields, so the assertion travelled through a
        // translation that existed only to be asserted on. The event is where the data is, and
        // the mod is what registers the system that publishes it.
        Apply().Success.Should().BeTrue();

        var reported = new List<PawnStateChangedEvent>();
        _h.Services.Pawns.Subscribe<PawnStateChangedEvent>(reported.Add);

        // The reporter is a SLOW system: one wake every 60 ticks.
        for (int i = 0; i < 65; i++)
            _h.Scheduler.ExecuteTick(1f / 30f);

        reported.Should().NotBeEmpty("the reporter must have woken at least once in 65 ticks");
        reported.Should().HaveCountGreaterThanOrEqualTo(Scenario.Counts.Pawns,
            "every colonist is described on each wake");

        foreach (PawnStateChangedEvent e in reported)
        {
            e.Name.Should().NotBeNullOrWhiteSpace(
                "the reporter carries IdentityComponent.Name through; an empty one means the " +
                "identity was not wired");
            e.Name.Should().Contain(" ", "the seeder gives forename and surname");

            e.TopSkills.Should().HaveCount(3, "the reporter reduces the roll to a top three");
            for (int i = 0; i < e.TopSkills.Count - 1; i++)
            {
                e.TopSkills[i].Level.Should().BeGreaterThanOrEqualTo(e.TopSkills[i + 1].Level,
                    "top skills are sorted descending by level");
            }
        }
    }
}
