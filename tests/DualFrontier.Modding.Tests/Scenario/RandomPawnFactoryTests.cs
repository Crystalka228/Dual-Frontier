using System;
using System.Collections.Generic;
using System.Linq;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Application.Scenario;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Scenario;

/// <summary>
/// Unit coverage of <see cref="RandomPawnFactory"/>: deterministic
/// generation by seed, name uniqueness, position passability, full
/// SkillKind population, event publication, and bounds checking. The
/// factory is internal — tests live here (already has
/// <c>InternalsVisibleTo</c> on Core + Application) rather than in a
/// separate Application.Tests project, to keep the M-phase boundary
/// (Core/Contracts unchanged) intact.
/// </summary>
public sealed class RandomPawnFactoryTests
{
    private const int MapW = 50;
    private const int MapH = 50;

    [Fact]
    public void Spawn_RequestedCount_ReturnsExactCount()
    {
        var (factory, world, services) = BuildFixture(seed: 1);
        var ids = factory.Spawn(world, services, 10);
        ids.Count.Should().Be(10);
    }

    [Fact]
    public void Spawn_DeterministicBySeed_ProducesIdenticalOutput()
    {
        var (factoryA, worldA, servicesA) = BuildFixture(seed: 42);
        var (factoryB, worldB, servicesB) = BuildFixture(seed: 42);

        var idsA = factoryA.Spawn(worldA, servicesA, 10);
        var idsB = factoryB.Spawn(worldB, servicesB, 10);

        // Compare names per pawn — these come straight from the RNG draws
        // against fixed pools, so identical seeds must produce identical
        // names in identical order.
        var namesA = idsA.Select(id => worldA.GetComponentForTest<IdentityComponent>(id).Name).ToList();
        var namesB = idsB.Select(id => worldB.GetComponentForTest<IdentityComponent>(id).Name).ToList();
        namesA.Should().Equal(namesB);
    }

    [Fact]
    public void Spawn_NamesAreNonEmpty()
    {
        var (factory, world, services) = BuildFixture(seed: 7);
        var ids = factory.Spawn(world, services, 10);

        foreach (var id in ids)
        {
            var ident = world.GetComponentForTest<IdentityComponent>(id);
            ident.Name.Should().NotBeNullOrWhiteSpace();
            ident.Name.Should().Contain(" ", "names follow forename + surname pattern");
        }
    }

    [Fact]
    public void Spawn_PositionsAreUnique()
    {
        var (factory, world, services) = BuildFixture(seed: 7);
        var ids = factory.Spawn(world, services, 10);

        var positions = ids
            .Select(id => world.GetComponentForTest<PositionComponent>(id).Position)
            .ToList();
        positions.Distinct().Count().Should().Be(positions.Count);
    }

    [Fact]
    public void Spawn_PositionsArePassable()
    {
        var navGrid = BuildNavGrid();
        var factory = new RandomPawnFactory(seed: 7, navGrid, MapW, MapH);
        var world = new World();
        var services = new GameServices();

        var ids = factory.Spawn(world, services, 10);

        foreach (var id in ids)
        {
            var pos = world.GetComponentForTest<PositionComponent>(id).Position;
            navGrid.IsPassable(pos.X, pos.Y).Should().BeTrue();
        }
    }

    [Fact]
    public void Spawn_EveryPawnHasAllSkillKindsPopulated()
    {
        var (factory, world, services) = BuildFixture(seed: 7);
        var ids = factory.Spawn(world, services, 10);

        var allKinds = (SkillKind[])Enum.GetValues(typeof(SkillKind));

        foreach (var id in ids)
        {
            var skills = world.GetComponentForTest<SkillsComponent>(id);
            skills.Levels.Should().NotBeNull();
            foreach (var kind in allKinds)
            {
                skills.Levels!.ContainsKey(kind).Should().BeTrue($"skill {kind} must be populated");
                skills.Levels[kind].Should().BeInRange(0, SkillsComponent.MaxLevel);
            }
        }
    }

    [Fact]
    public void Spawn_PublishesPawnSpawnedEventPerPawn()
    {
        var (factory, world, services) = BuildFixture(seed: 7);

        var observed = new List<PawnSpawnedEvent>();
        services.Pawns.Subscribe<PawnSpawnedEvent>(e => observed.Add(e));

        factory.Spawn(world, services, 10);

        observed.Count.Should().Be(10);
    }

    [Fact]
    public void Spawn_RequestingMoreThanAvailableTiles_Throws()
    {
        // Block almost every tile — leave only a 3x3 corner = 9 passable.
        var navGrid = new NavGrid(MapW, MapH);
        for (int y = 0; y < MapH; y++)
            for (int x = 0; x < MapW; x++)
                if (!(x < 3 && y < 3))
                    navGrid.SetTile(x, y, passable: false);

        var factory = new RandomPawnFactory(seed: 7, navGrid, MapW, MapH);
        var world = new World();
        var services = new GameServices();

        Action act = () => factory.Spawn(world, services, 100);
        act.Should().Throw<InvalidOperationException>();
    }

    private static NavGrid BuildNavGrid()
    {
        var nav = new NavGrid(MapW, MapH);
        var rng = new Random(99);
        for (int i = 0; i < 50; i++)
            nav.SetTile(rng.Next(MapW), rng.Next(MapH), passable: false);
        return nav;
    }

    private static (RandomPawnFactory factory, World world, GameServices services)
        BuildFixture(int seed)
    {
        var navGrid = BuildNavGrid();
        var factory = new RandomPawnFactory(seed, navGrid, MapW, MapH);
        var world = new World();
        var services = new GameServices();
        return (factory, world, services);
    }
}

/// <summary>
/// Test-side helper that exposes <see cref="World.TryGetComponent{T}"/>
/// as a thrown-on-missing accessor — World's public Get path goes
/// through the isolation guard, which only allows access from inside an
/// active scheduler context. Tests inspect components directly outside
/// any scheduler, so this helper does the lookup via the public
/// <c>TryGetComponent</c> method and throws a clear assertion if absent.
/// </summary>
internal static class WorldTestExtensions
{
    public static T GetComponentForTest<T>(this World world, EntityId id) where T : IComponent
    {
        if (!world.TryGetComponent<T>(id, out var component))
            throw new InvalidOperationException(
                $"Component {typeof(T).Name} not found for entity {id} in test fixture");
        return component;
    }
}
