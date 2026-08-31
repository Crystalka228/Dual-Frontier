using System;
using System.Collections.Generic;
using AwesomeAssertions;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Distribution;
using DualFrontier.Contracts.Math;
using Xunit;

namespace DualFrontier.Modding.Tests.Vanilla;

/// <summary>
/// The placement rules the starting colony obeys.
///
/// <para>
/// <b>Where this came from.</b> Two engine-side suites covered these rules until W4:
/// RandomPawnFactoryTests, which had existed since K7, and ItemFactoryTests, which this cascade
/// wrote as the behavioural floor ItemFactory had never had. Both took a concrete NativeWorld and
/// a GameServices bus, which is exactly why the factories could not travel as written and why
/// their tests could not either. The rules travelled instead, into
/// <c>ScenarioSeeder</c>, and this is where they are pinned now.
/// </para>
///
/// <para>
/// <b>Why through the pipeline rather than against the class.</b> The seeder is a mod type. A
/// test project that referenced the mod assembly directly would compile against a copy in the
/// default load context while the wave gate loads another copy into a collectible one — two
/// identities for one type, for no benefit. Driving the real pipeline avoids the question
/// entirely and is the stronger oracle anyway: it pins what an installation does, including the
/// world-seeder hook and the scheduler rebuild, not just what one class computes.
/// </para>
///
/// <para>
/// Every test declares its own <see cref="ScenarioConfig"/>. That is the shape of the wave: the
/// starting state is data a distribution supplies, so asking for a different colony is a
/// different manifest, not a different build.
/// </para>
/// </summary>
[Collection("GameLoopSerial")]
public sealed class ScenarioSeedingTests
{
    private static ScenarioConfig Scenario(
        int pawns = 8,
        int food = 5,
        int water = 3,
        int beds = 2,
        int decorations = 1,
        int width = 30,
        int height = 30,
        int obstacles = 40,
        int factorySeed = 42,
        int itemSeed = 43,
        int obstacleSeed = 42) => new(
            Id: "seeding-tests",
            WorldSeed: 0,
            MapWidth: width,
            MapHeight: height,
            ObstacleCount: obstacles,
            ObstacleSeed: obstacleSeed,
            FactorySeed: factorySeed,
            ItemFactorySeed: itemSeed,
            Counts: new ScenarioCounts(pawns, food, water, beds, decorations));

    // ── the colonists ─────────────────────────────────────────────────────────

    [Fact]
    public void EveryColonistHasEverySkillKindRolledWithinRange()
    {
        ScenarioConfig scenario = Scenario();
        using var h = new ScenarioHarness(scenario);
        h.Apply().Success.Should().BeTrue();

        List<EntityId> colonists = ScenarioHarness.Ids<IdentityComponent>(h.World);
        colonists.Should().HaveCount(scenario.Counts.Pawns);

        foreach (EntityId colonist in colonists)
        {
            h.World.TryGetComponent(colonist, out SkillsComponent skills).Should().BeTrue();
            skills.IsInitialized.Should().BeTrue("skills were rolled, not left at the default");
            foreach (SkillKind kind in Enum.GetValues<SkillKind>())
            {
                skills.LevelOf(kind).Should().BeInRange(0, SkillsComponent.MaxLevel,
                    $"skill {kind} must be rolled into the legal band");
            }
        }
    }

    [Fact]
    public void ColonistsAreOnlyPlacedOnPassableTiles()
    {
        // A dense scatter, so the assertion has something to catch: with few obstacles a placement
        // bug would pass by luck.
        ScenarioConfig scenario = Scenario(pawns: 8, obstacles: 300, width: 30, height: 30);
        using var h = new ScenarioHarness(scenario);
        h.Apply().Success.Should().BeTrue();

        HashSet<GridVector> blocked = ScenarioHarness.ObstacleTiles(scenario);
        blocked.Should().NotBeEmpty("the premise of this test is that some tiles are impassable");

        foreach (GridVector tile in ScenarioHarness.TilesOf<IdentityComponent>(h.World))
            blocked.Should().NotContain(tile, $"a colonist stands on impassable tile {tile.X},{tile.Y}");
    }

    // ── the items ─────────────────────────────────────────────────────────────

    [Fact]
    public void EachItemKindGetsExactlyTheRequestedCount()
    {
        ScenarioConfig scenario = Scenario(pawns: 4, food: 7, water: 5, beds: 3, decorations: 2);
        using var h = new ScenarioHarness(scenario);
        h.Apply().Success.Should().BeTrue();

        ScenarioHarness.Ids<ConsumableComponent>(h.World).Should().HaveCount(7, "food");
        ScenarioHarness.Ids<WaterSourceComponent>(h.World).Should().HaveCount(5, "water");
        ScenarioHarness.Ids<BedComponent>(h.World).Should().HaveCount(3, "beds");
        ScenarioHarness.Ids<DecorativeAuraComponent>(h.World).Should().HaveCount(2, "decorations");
    }

    [Fact]
    public void ItemsCarryTheComponentValuesTheScenarioImplies()
    {
        ScenarioConfig scenario = Scenario(pawns: 2, food: 1, water: 1, beds: 1, decorations: 1);
        using var h = new ScenarioHarness(scenario);
        h.Apply().Success.Should().BeTrue();

        ConsumableComponent food = One<ConsumableComponent>(h);
        food.RestoresKind.Should().Be(NeedKind.Satiety);
        food.RestorationAmount.Should().BeApproximately(0.4f, 1e-6f);
        food.Charges.Should().Be(1);

        One<WaterSourceComponent>(h).RestorationAmount.Should().BeApproximately(0.5f, 1e-6f);

        BedComponent bed = One<BedComponent>(h);
        bed.Occupant.Should().BeNull("a freshly seeded bed is unclaimed");
        bed.SleepRestorationPerTick.Should().BeApproximately(0.005f, 1e-6f);

        DecorativeAuraComponent decor = One<DecorativeAuraComponent>(h);
        decor.Radius.Should().Be(3);
        decor.ComfortPerTick.Should().BeApproximately(0.001f, 1e-6f);
    }

    [Fact]
    public void ItemsAreNeverPlacedOnAColonistTile()
    {
        // The colonist tiles are excluded from the item pool. Without that exclusion the two
        // shuffles are independent and collisions are near-certain at this density.
        ScenarioConfig scenario = Scenario(pawns: 20, food: 20, water: 10, beds: 10, decorations: 5,
                                           width: 12, height: 12, obstacles: 0);
        using var h = new ScenarioHarness(scenario);
        h.Apply().Success.Should().BeTrue();

        var colonistTiles = new HashSet<GridVector>(ScenarioHarness.TilesOf<IdentityComponent>(h.World));
        colonistTiles.Should().HaveCount(scenario.Counts.Pawns);

        foreach (GridVector tile in ItemTiles(h))
        {
            colonistTiles.Should().NotContain(tile,
                "the excluded set is what keeps items off colonist tiles");
        }
    }

    [Fact]
    public void ItemsAreOnlyPlacedOnPassableTiles()
    {
        ScenarioConfig scenario = Scenario(pawns: 2, food: 6, water: 4, beds: 3, decorations: 2,
                                           obstacles: 300, width: 30, height: 30);
        using var h = new ScenarioHarness(scenario);
        h.Apply().Success.Should().BeTrue();

        HashSet<GridVector> blocked = ScenarioHarness.ObstacleTiles(scenario);
        foreach (GridVector tile in ItemTiles(h))
            blocked.Should().NotContain(tile, $"an item sits on impassable tile {tile.X},{tile.Y}");
    }

    // ── determinism ───────────────────────────────────────────────────────────

    [Fact]
    public void TheSameSeedsProduceTheSameColony()
    {
        ScenarioConfig scenario = Scenario();

        (List<GridVector> tiles, List<string> names) a = SeedAndDescribe(scenario);
        (List<GridVector> tiles, List<string> names) b = SeedAndDescribe(scenario);

        a.tiles.Should().Equal(b.tiles, "placement is a pure function of the declared seeds");
        a.names.Should().Equal(b.names, "so is name generation");
    }

    [Fact]
    public void DifferentSeedsProduceADifferentColony()
    {
        // The falsifying companion to the determinism test: without it, a seeder that ignored its
        // seeds entirely would satisfy the pair above.
        (List<GridVector> tiles, List<string> _) a = SeedAndDescribe(Scenario(factorySeed: 42, itemSeed: 43));
        (List<GridVector> tiles, List<string> _) b = SeedAndDescribe(Scenario(factorySeed: 7, itemSeed: 8));

        a.tiles.Should().NotEqual(b.tiles, "the seeds are the only source of variation, so they must vary it");
    }

    // ── the degenerate and the impossible ─────────────────────────────────────

    [Fact]
    public void AScenarioAskingForNothingSeedsNothing()
    {
        ScenarioConfig scenario = Scenario(pawns: 0, food: 0, water: 0, beds: 0, decorations: 0);
        using var h = new ScenarioHarness(scenario);

        h.Apply().Success.Should().BeTrue("an empty colony is a legal colony");

        ScenarioHarness.AllStartingEntities(h.World).Should().BeEmpty();
    }

    [Fact]
    public void AScenarioThatCannotFitRefusesLoudlyAndSeedsNothing()
    {
        // A 4x4 map with every tile blocked leaves no room for a single colonist. The refusal
        // happens before any entity is minted, and it surfaces out of Apply rather than as a
        // failed PipelineResult: a distribution whose scenario cannot be realised is broken in a
        // way no amount of continuing helps.
        ScenarioConfig scenario = Scenario(pawns: 20, food: 0, water: 0, beds: 0, decorations: 0,
                                           width: 4, height: 4, obstacles: 400);
        using var h = new ScenarioHarness(scenario);

        Action apply = () => h.Apply();

        apply.Should().Throw<InvalidOperationException>()
            .WithMessage("*seeding-tests*", "the refusal names the scenario that asked")
            .WithMessage("*20 colonists*", "and how many it asked for");

        ScenarioHarness.AllStartingEntities(h.World).Should().BeEmpty(
            "the pool is checked before the first entity is created");
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    [Fact]
    public void AReloadKeepsTheTerrain()
    {
        // The grid is rebuilt from scratch on every initialisation and a fresh one is wholly
        // passable, so terrain has to be scattered on every registration. It used to be scattered
        // inside the seeding path, behind the read-then-mint guard that exists to stop a reload
        // re-populating the colony -- so a reload took the early return, and the movement system
        // it had just been handed pathfound over a world with no walls in it.
        //
        // The colony cannot reveal this on its own: it is seeded on the FIRST apply, when the
        // obstacles were still correct, so its starting tiles are legal either way. Movement is
        // what reveals it, which is why this ticks. Obstacle density is deliberately high so a
        // pathfinder let loose on an empty grid crosses a blocked tile almost immediately; with
        // the terrain intact the count is exactly zero, always, because A* never routes onto an
        // impassable tile.
        ScenarioConfig scenario = Scenario(
            pawns: 12, food: 4, water: 3, beds: 2, decorations: 1,
            width: 40, height: 40, obstacles: 900);

        using var h = new ScenarioHarness(scenario);
        h.Apply().Success.Should().BeTrue();

        h.Pipeline.UnloadMod("dualfrontier.vanilla.scenario");
        h.Apply().Success.Should().BeTrue();

        for (int i = 0; i < 60; i++)
            h.Scheduler.ExecuteTick(1f / 30f);

        HashSet<GridVector> blocked = ScenarioHarness.ObstacleTiles(scenario);
        List<GridVector> occupied = ScenarioHarness.TilesOf<IdentityComponent>(h.World);

        occupied.Should().NotBeEmpty("the colony survives the reload");
        occupied.Should().OnlyContain(t => !blocked.Contains(t),
            "a reload rebuilds the grid, so it must re-scatter the terrain too -- a colonist " +
            "standing on an obstacle means the second grid was left wholly passable");
    }

    private static (List<GridVector> Tiles, List<string> Names) SeedAndDescribe(ScenarioConfig scenario)
    {
        using var h = new ScenarioHarness(scenario);
        h.Apply().Success.Should().BeTrue();

        var names = new List<string>();
        foreach (EntityId colonist in ScenarioHarness.Ids<IdentityComponent>(h.World))
        {
            h.World.TryGetComponent(colonist, out IdentityComponent identity).Should().BeTrue();
            names.Add(h.World.Resolve(identity.Name) ?? string.Empty);
        }

        var tiles = new List<GridVector>();
        foreach (EntityId id in ScenarioHarness.AllStartingEntities(h.World))
        {
            h.World.TryGetComponent(id, out DualFrontier.Components.Shared.PositionComponent p)
                .Should().BeTrue();
            tiles.Add(p.Position);
        }

        return (tiles, names);
    }

    private static List<GridVector> ItemTiles(ScenarioHarness h)
    {
        var tiles = new List<GridVector>();
        tiles.AddRange(ScenarioHarness.TilesOf<ConsumableComponent>(h.World));
        tiles.AddRange(ScenarioHarness.TilesOf<WaterSourceComponent>(h.World));
        tiles.AddRange(ScenarioHarness.TilesOf<BedComponent>(h.World));
        tiles.AddRange(ScenarioHarness.TilesOf<DecorativeAuraComponent>(h.World));
        return tiles;
    }

    private static T One<T>(ScenarioHarness h) where T : unmanaged, IComponent
    {
        List<EntityId> ids = ScenarioHarness.Ids<T>(h.World);
        ids.Should().ContainSingle($"exactly one {typeof(T).Name} was requested");
        h.World.TryGetComponent(ids[0], out T value).Should().BeTrue();
        return value;
    }
}
