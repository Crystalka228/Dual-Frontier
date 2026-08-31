using System;
using System.Collections.Generic;
using System.Linq;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Application.Bootstrap;
using DualFrontier.Application.Scenario;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Interop;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Scenario;

/// <summary>
/// W4 / D15 — the behavioural floor <see cref="ItemFactory"/> never had.
///
/// <para>
/// <c>RandomPawnFactory</c> has had unit coverage since K7; <c>ItemFactory</c> had NONE — no
/// test anywhere named it, and its behaviour was pinned only indirectly through
/// <c>GameBootstrap</c>'s 50-pawn integration test, which asserts nothing about items. W4 moves
/// this file into a mod, so without a floor the move would be a relocation of code no test can
/// contradict. These facts are written against the CURRENT implementation deliberately: they are
/// the equivalence obligation the relocation must satisfy, not a description of where the code
/// should end up.
/// </para>
///
/// <para>
/// What is pinned: the per-kind counts, that every item is placed and placed exactly once, that
/// placement respects passability and the excluded set, determinism by seed, the component
/// values the scenario contract carries, and the two refusals. What is deliberately NOT pinned
/// is the shuffle's exact permutation — that is an implementation detail of the current RNG use,
/// and pinning it would forbid the very rework the relocation needs.
/// </para>
/// </summary>
public sealed class ItemFactoryTests
{
    private const int MapW = 40;
    private const int MapH = 40;

    [Fact]
    public void Spawn_PlacesExactlyTheRequestedCountOfEachKind()
    {
        using Fixture fx = Fixture.Build(seed: 43);

        fx.Factory.Spawn(fx.World, Array.Empty<GridVector>(), 7, 5, 3, 2);

        Ids<ConsumableComponent>(fx.World).Should().HaveCount(7, "food");
        Ids<WaterSourceComponent>(fx.World).Should().HaveCount(5, "water");
        Ids<BedComponent>(fx.World).Should().HaveCount(3, "beds");
        Ids<DecorativeAuraComponent>(fx.World).Should().HaveCount(2, "decorations");
    }

    [Fact]
    public void Spawn_EveryItemCarriesAPositionAndNoTwoItemsShareATile()
    {
        using Fixture fx = Fixture.Build(seed: 43);

        fx.Factory.Spawn(fx.World, Array.Empty<GridVector>(), 10, 10, 10, 10);

        List<GridVector> placed = AllItemPositions(fx.World);

        placed.Should().HaveCount(40, "every requested item must be placed and must carry a position");
        placed.Distinct().Should().HaveCount(40,
            "the factory takes a prefix of a shuffled tile pool, so a repeated tile means the " +
            "cursor was reused across kinds");
    }

    [Fact]
    public void Spawn_PlacesOnlyOnPassableTiles()
    {
        using Fixture fx = Fixture.Build(seed: 43);

        fx.Factory.Spawn(fx.World, Array.Empty<GridVector>(), 10, 10, 5, 5);

        foreach (GridVector p in AllItemPositions(fx.World))
            fx.Nav.IsPassable(p.X, p.Y).Should().BeTrue($"item placed on impassable tile {p.X},{p.Y}");
    }

    [Fact]
    public void Spawn_AvoidsTheExcludedTiles()
    {
        using Fixture fx = Fixture.Build(seed: 43);

        // The production caller passes the pawn tiles here so items never land under a pawn.
        var excluded = new List<GridVector>();
        for (int x = 0; x < MapW; x++)
            for (int y = 0; y < 5; y++)
                excluded.Add(new GridVector(x, y));

        fx.Factory.Spawn(fx.World, excluded, 20, 10, 5, 5);

        var excludedSet = new HashSet<GridVector>(excluded);
        foreach (GridVector p in AllItemPositions(fx.World))
            excludedSet.Should().NotContain(p, "the excluded set is what keeps items off pawn tiles");
    }

    [Fact]
    public void Spawn_IsDeterministicBySeed()
    {
        using Fixture a = Fixture.Build(seed: 43);
        using Fixture b = Fixture.Build(seed: 43);

        a.Factory.Spawn(a.World, Array.Empty<GridVector>(), 8, 6, 4, 2);
        b.Factory.Spawn(b.World, Array.Empty<GridVector>(), 8, 6, 4, 2);

        Sorted(AllItemPositions(a.World)).Should().Equal(Sorted(AllItemPositions(b.World)),
            "the same seed against the same grid must place the same tiles — this is what makes " +
            "a scenario reproducible from its manifest");
    }

    [Fact]
    public void Spawn_DifferentSeedsPlaceDifferently()
    {
        using Fixture a = Fixture.Build(seed: 43);
        using Fixture b = Fixture.Build(seed: 44);

        a.Factory.Spawn(a.World, Array.Empty<GridVector>(), 20, 0, 0, 0);
        b.Factory.Spawn(b.World, Array.Empty<GridVector>(), 20, 0, 0, 0);

        Sorted(AllItemPositions(a.World)).Should().NotEqual(Sorted(AllItemPositions(b.World)),
            "otherwise the seed is not reaching the shuffle and determinism above would be vacuous");
    }

    [Fact]
    public void Spawn_WritesTheScenarioComponentValues()
    {
        using Fixture fx = Fixture.Build(seed: 43);

        fx.Factory.Spawn(fx.World, Array.Empty<GridVector>(), 1, 1, 1, 1);

        ConsumableComponent food = One<ConsumableComponent>(fx.World);
        food.RestoresKind.Should().Be(NeedKind.Satiety);
        food.RestorationAmount.Should().BeApproximately(0.4f, 1e-6f);
        food.Charges.Should().Be(1);

        One<WaterSourceComponent>(fx.World).RestorationAmount.Should().BeApproximately(0.5f, 1e-6f);

        BedComponent bed = One<BedComponent>(fx.World);
        bed.Occupant.Should().BeNull("a freshly spawned bed is unclaimed");
        bed.SleepRestorationPerTick.Should().BeApproximately(0.005f, 1e-6f);

        DecorativeAuraComponent decor = One<DecorativeAuraComponent>(fx.World);
        decor.Radius.Should().Be(3);
        decor.ComfortPerTick.Should().BeApproximately(0.001f, 1e-6f);
    }

    [Fact]
    public void Spawn_WithAllCountsZero_CreatesNothing()
    {
        using Fixture fx = Fixture.Build(seed: 43);

        fx.Factory.Spawn(fx.World, Array.Empty<GridVector>(), 0, 0, 0, 0);

        AllItemPositions(fx.World).Should().BeEmpty(
            "SpawnTyped returns early on a zero count, so a zero-item scenario mints no entities");
    }

    [Fact]
    public void Spawn_WhenThePoolIsTooSmall_RefusesLoudlyBeforeMintingAnything()
    {
        var nav = new NavGrid(MapW, MapH);
        for (int y = 0; y < MapH; y++)
            for (int x = 0; x < MapW; x++)
                if (!(x < 2 && y < 2))
                    nav.SetTile(x, y, passable: false);

        using Fixture fx = Fixture.Build(seed: 43, nav);

        Action act = () => fx.Factory.Spawn(fx.World, Array.Empty<GridVector>(), 10, 0, 0, 0);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Not enough passable tiles*",
                "a scenario that cannot be placed must fail loudly rather than silently placing fewer items");
        AllItemPositions(fx.World).Should().BeEmpty("the refusal happens before any entity is minted");
    }

    [Fact]
    public void Spawn_RejectsNegativeCounts()
    {
        using Fixture fx = Fixture.Build(seed: 43);

        Action act = () => fx.Factory.Spawn(fx.World, Array.Empty<GridVector>(), -1, 0, 0, 0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    // ── Fixture + readback helpers ───────────────────────────────────────────

    private sealed class Fixture : IDisposable
    {
        public required NativeWorld World { get; init; }
        public required NavGrid Nav { get; init; }
        public required ItemFactory Factory { get; init; }

        public static Fixture Build(int seed, NavGrid? nav = null)
        {
            NavGrid grid = nav ?? BuildNavGrid();
            NativeWorld world = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
            VanillaComponentRegistration.RegisterAll(world.Registry!);
            return new Fixture
            {
                World = world,
                Nav = grid,
                Factory = new ItemFactory(seed, grid, MapW, MapH),
            };
        }

        public void Dispose() => World.Dispose();
    }

    private static NavGrid BuildNavGrid()
    {
        var nav = new NavGrid(MapW, MapH);
        var rng = new Random(99);
        for (int i = 0; i < 40; i++)
            nav.SetTile(rng.Next(MapW), rng.Next(MapH), passable: false);
        return nav;
    }

    /// <summary>
    /// The entity ids carrying <typeparamref name="T"/>. Mirrors the span walk
    /// <c>GameBootstrap.PublishKind</c> uses, and releases the lease before the caller reads
    /// components back — the world refuses mutation while a span is live.
    /// </summary>
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

    private static T One<T>(NativeWorld world) where T : unmanaged, IComponent
    {
        List<EntityId> ids = Ids<T>(world);
        ids.Should().ContainSingle($"exactly one {typeof(T).Name} was requested");
        world.TryGetComponent(ids[0], out T value).Should().BeTrue();
        return value;
    }

    private static List<GridVector> AllItemPositions(NativeWorld world)
    {
        var ids = new List<EntityId>();
        ids.AddRange(Ids<ConsumableComponent>(world));
        ids.AddRange(Ids<WaterSourceComponent>(world));
        ids.AddRange(Ids<BedComponent>(world));
        ids.AddRange(Ids<DecorativeAuraComponent>(world));

        var positions = new List<GridVector>(ids.Count);
        foreach (EntityId id in ids)
        {
            world.TryGetComponent(id, out PositionComponent p).Should().BeTrue(
                "every item the factory mints receives a PositionComponent");
            positions.Add(p.Position);
        }
        return positions;
    }

    private static List<GridVector> Sorted(List<GridVector> source)
        => source.OrderBy(p => p.Y).ThenBy(p => p.X).ToList();
}
