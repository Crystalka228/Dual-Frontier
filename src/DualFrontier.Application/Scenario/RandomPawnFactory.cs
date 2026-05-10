using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Application.Scenario;

/// <summary>
/// Deterministic random pawn factory. Generates colonists with real
/// IdentityComponent names (from internal name pools), SkillsComponent
/// populated with random levels for every SkillKind, and standard
/// per-pawn components (Position, Needs, Mind, Job, Movement). Spawn
/// positions are chosen from passable tiles in the supplied NavGrid;
/// each pawn gets a unique tile.
///
/// Determinism: given the same constructor seed, the same NavGrid
/// (same passable-tile set), and the same Spawn count, the factory
/// produces byte-identical pawns (names, skills, positions, order).
/// Used by GameBootstrap in production with seed 42; tests override
/// the seed to produce known fixtures.
///
/// Operating principle: "data exists or it doesn't". The factory does
/// NOT generate role/class data because no component exists for it.
/// The factory does NOT generate health/faction/race/mana/etc. because
/// the corresponding UI does not display them yet — adding the data
/// without the display would create asymmetry. Phase 5 expands scope.
/// </summary>
internal sealed class RandomPawnFactory
{
    /// <summary>Generic-fantasy/sci-fi-neutral forename pool.</summary>
    private static readonly string[] Forenames =
    {
        "Aelin", "Bram", "Cara", "Dren", "Elin", "Fenn", "Greta", "Holt",
        "Iri", "Jona", "Kell", "Lira", "Marek", "Nev", "Olen", "Pem",
        "Quin", "Reva", "Soren", "Tav", "Una", "Vex", "Wren", "Xan",
        "Yara", "Zeph",
    };

    /// <summary>Generic-fantasy/sci-fi-neutral surname pool.</summary>
    private static readonly string[] Surnames =
    {
        "Ashford", "Brand", "Cole", "Drake", "Esten", "Forge", "Gale",
        "Hale", "Ivor", "Jaxe", "Korr", "Lome", "Marsh", "Nash", "Orem",
        "Pyre", "Quill", "Ridge", "Stone", "Thorn", "Ulf", "Vale",
        "Welk", "Yew",
    };

    private readonly Random _rng;
    private readonly NavGrid _navGrid;
    private readonly int _mapWidth;
    private readonly int _mapHeight;
    private readonly NativeWorld _nativeWorld;

    public RandomPawnFactory(int seed, NavGrid navGrid, int mapWidth, int mapHeight, NativeWorld nativeWorld)
    {
        _rng = new Random(seed);
        _navGrid = navGrid ?? throw new ArgumentNullException(nameof(navGrid));
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
        _nativeWorld = nativeWorld ?? throw new ArgumentNullException(nameof(nativeWorld));
    }

    /// <summary>
    /// Spawns <paramref name="count"/> pawns into <paramref name="world"/>,
    /// each on a unique passable tile, with a randomly generated name and
    /// fully-populated SkillsComponent. Publishes one PawnSpawnedEvent per
    /// pawn on the supplied <paramref name="services"/>.Pawns bus.
    /// </summary>
    /// <returns>EntityIds of the created pawns, in spawn order.</returns>
    public IReadOnlyList<EntityId> Spawn(World world, GameServices services, int count)
    {
        if (world is null) throw new ArgumentNullException(nameof(world));
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

        // Build the passable-tile pool, shuffle, take the first `count`.
        // Guarantees uniqueness and passability in one allocation.
        var passable = new List<GridVector>(_mapWidth * _mapHeight);
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                if (_navGrid.IsPassable(x, y))
                    passable.Add(new GridVector(x, y));
            }
        }

        if (passable.Count < count)
        {
            throw new InvalidOperationException(
                $"Not enough passable tiles to spawn {count} pawns: only {passable.Count} available");
        }

        // Fisher-Yates shuffle, then take head.
        for (int i = passable.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(0, i + 1);
            (passable[i], passable[j]) = (passable[j], passable[i]);
        }

        var ids = new List<EntityId>(count);
        for (int i = 0; i < count; i++)
        {
            EntityId id = SpawnOne(world, services, passable[i]);
            ids.Add(id);
        }
        return ids;
    }

    private EntityId SpawnOne(World world, GameServices services, GridVector pos)
    {
        EntityId id = world.CreateEntity();

        world.AddComponent(id, new PositionComponent { Position = pos });

        string fullName = $"{Forenames[_rng.Next(Forenames.Length)]} {Surnames[_rng.Next(Surnames.Length)]}";
        world.AddComponent(id, new IdentityComponent
        {
            Name = _nativeWorld.InternString(fullName)
        });

        NativeMap<SkillKind, int> levels = _nativeWorld.CreateMap<SkillKind, int>();
        NativeMap<SkillKind, float> experience = _nativeWorld.CreateMap<SkillKind, float>();
        foreach (SkillKind kind in (SkillKind[])Enum.GetValues(typeof(SkillKind)))
        {
            levels.Set(kind, _rng.Next(0, SkillsComponent.MaxLevel + 1));
            experience.Set(kind, 0f);
        }
        world.AddComponent(id, new SkillsComponent
        {
            Levels = levels,
            Experience = experience,
        });

        world.AddComponent(id, new NeedsComponent
        {
            Satiety   = 0.9f,
            Hydration = 0.9f,
            Sleep     = 0.9f,
            Comfort   = 1.0f
        });
        world.AddComponent(id, new MindComponent());
        world.AddComponent(id, new JobComponent { Current = JobKind.Idle });
        world.AddComponent(id, new MovementComponent
        {
            Path = _nativeWorld.CreateComposite<GridVector>(),
        });

        services.Pawns.Publish(new PawnSpawnedEvent
        {
            PawnId = id,
            X = pos.X,
            Y = pos.Y
        });

        return id;
    }
}
