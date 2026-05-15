using System;
using System.Collections.Generic;
using System.Linq;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Application.Bootstrap;
using DualFrontier.Application.Scenario;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Pawn;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Scenario;

/// <summary>
/// Unit coverage of <see cref="RandomPawnFactory"/>: deterministic
/// generation by seed, name uniqueness, position passability, full
/// SkillKind population, event publication, and bounds checking.
///
/// K8.3+K8.4 cutover: factory writes only into <see cref="NativeWorld"/>;
/// tests read components back via <c>NativeWorld.TryGetComponent</c>. The
/// fixture bootstraps a registry-bound NativeWorld through
/// <see cref="DualFrontier.Core.Interop.Bootstrap.Run"/> +
/// <see cref="VanillaComponentRegistration.RegisterAll"/> so the factory's
/// component registrations succeed.
/// </summary>
public sealed class RandomPawnFactoryTests
{
    private const int MapW = 50;
    private const int MapH = 50;

    [Fact]
    public void Spawn_RequestedCount_ReturnsExactCount()
    {
        var fx = BuildFixture(seed: 1);
        var ids = fx.factory.Spawn(fx.nativeWorld, fx.services, 10);
        ids.Count.Should().Be(10);
        fx.nativeWorld.Dispose();
    }

    [Fact]
    public void Spawn_DeterministicBySeed_ProducesIdenticalOutput()
    {
        var fxA = BuildFixture(seed: 42);
        var fxB = BuildFixture(seed: 42);

        var idsA = fxA.factory.Spawn(fxA.nativeWorld, fxA.services, 10);
        var idsB = fxB.factory.Spawn(fxB.nativeWorld, fxB.services, 10);

        var namesA = idsA.Select(id =>
            GetComponent<IdentityComponent>(fxA.nativeWorld, id).Name.Resolve(fxA.nativeWorld)).ToList();
        var namesB = idsB.Select(id =>
            GetComponent<IdentityComponent>(fxB.nativeWorld, id).Name.Resolve(fxB.nativeWorld)).ToList();
        namesA.Should().Equal(namesB);
        fxA.nativeWorld.Dispose();
        fxB.nativeWorld.Dispose();
    }

    [Fact]
    public void Spawn_NamesAreNonEmpty()
    {
        var fx = BuildFixture(seed: 7);
        var ids = fx.factory.Spawn(fx.nativeWorld, fx.services, 10);

        foreach (var id in ids)
        {
            var ident = GetComponent<IdentityComponent>(fx.nativeWorld, id);
            ident.Name.IsEmpty.Should().BeFalse();
            string? resolved = ident.Name.Resolve(fx.nativeWorld);
            resolved.Should().NotBeNullOrWhiteSpace();
            resolved!.Should().Contain(" ", "names follow forename + surname pattern");
        }
        fx.nativeWorld.Dispose();
    }

    [Fact]
    public void Spawn_PositionsAreUnique()
    {
        var fx = BuildFixture(seed: 7);
        var ids = fx.factory.Spawn(fx.nativeWorld, fx.services, 10);

        var positions = ids
            .Select(id => GetComponent<PositionComponent>(fx.nativeWorld, id).Position)
            .ToList();
        positions.Distinct().Count().Should().Be(positions.Count);
        fx.nativeWorld.Dispose();
    }

    [Fact]
    public void Spawn_PositionsArePassable()
    {
        var navGrid = BuildNavGrid();
        using var nativeWorld = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!);
        var factory = new RandomPawnFactory(seed: 7, navGrid, MapW, MapH);
        var services = new GameServices();

        var ids = factory.Spawn(nativeWorld, services, 10);

        foreach (var id in ids)
        {
            var pos = GetComponent<PositionComponent>(nativeWorld, id).Position;
            navGrid.IsPassable(pos.X, pos.Y).Should().BeTrue();
        }
    }

    [Fact]
    public void Spawn_EveryPawnHasAllSkillKindsPopulated()
    {
        var fx = BuildFixture(seed: 7);
        var ids = fx.factory.Spawn(fx.nativeWorld, fx.services, 10);

        var allKinds = (SkillKind[])Enum.GetValues(typeof(SkillKind));

        foreach (var id in ids)
        {
            var skills = GetComponent<SkillsComponent>(fx.nativeWorld, id);
            skills.Levels.IsValid.Should().BeTrue();
            foreach (var kind in allKinds)
            {
                skills.Levels.TryGet(kind, out int level).Should().BeTrue($"skill {kind} must be populated");
                level.Should().BeInRange(0, SkillsComponent.MaxLevel);
            }
        }
        fx.nativeWorld.Dispose();
    }

    [Fact]
    public void Spawn_PublishesPawnSpawnedEventPerPawn()
    {
        var fx = BuildFixture(seed: 7);

        var observed = new List<PawnSpawnedEvent>();
        fx.services.Pawns.Subscribe<PawnSpawnedEvent>(e => observed.Add(e));

        fx.factory.Spawn(fx.nativeWorld, fx.services, 10);

        observed.Count.Should().Be(10);
        fx.nativeWorld.Dispose();
    }

    [Fact]
    public void Spawn_RequestingMoreThanAvailableTiles_Throws()
    {
        var navGrid = new NavGrid(MapW, MapH);
        for (int y = 0; y < MapH; y++)
            for (int x = 0; x < MapW; x++)
                if (!(x < 3 && y < 3))
                    navGrid.SetTile(x, y, passable: false);

        using var nativeWorld = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!);
        var factory = new RandomPawnFactory(seed: 7, navGrid, MapW, MapH);
        var services = new GameServices();

        Action act = () => factory.Spawn(nativeWorld, services, 100);
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

    private static (RandomPawnFactory factory, GameServices services, NativeWorld nativeWorld)
        BuildFixture(int seed)
    {
        var navGrid = BuildNavGrid();
        var nativeWorld = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!);
        var factory = new RandomPawnFactory(seed, navGrid, MapW, MapH);
        var services = new GameServices();
        return (factory, services, nativeWorld);
    }

    private static T GetComponent<T>(NativeWorld nativeWorld, EntityId id) where T : unmanaged, IComponent
    {
        if (!nativeWorld.TryGetComponent<T>(id, out var component))
            throw new InvalidOperationException(
                $"Component {typeof(T).Name} not found for entity {id} in test fixture");
        return component;
    }
}
