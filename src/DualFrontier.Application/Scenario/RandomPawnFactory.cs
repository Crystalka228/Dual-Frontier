using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
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
///
/// K8.3+K8.4 cutover (2026-05-14, brief v2.0): NativeWorld is the sole
/// component-storage backend; the prior dual-write to a managed
/// <c>World</c> is gone. Phase A allocates per-entity K8.1 primitives
/// (<c>InternString</c>/<c>CreateMap</c>/<c>CreateComposite</c>),
/// Phase B bulk-adds component data via <c>NativeWorld.AddComponents&lt;T&gt;</c>
/// — one P/Invoke per type.
/// </summary>
internal sealed class RandomPawnFactory
{
    private static readonly string[] Forenames =
    {
        "Aelin", "Bram", "Cara", "Dren", "Elin", "Fenn", "Greta", "Holt",
        "Iri", "Jona", "Kell", "Lira", "Marek", "Nev", "Olen", "Pem",
        "Quin", "Reva", "Soren", "Tav", "Una", "Vex", "Wren", "Xan",
        "Yara", "Zeph",
    };

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

    public RandomPawnFactory(int seed, NavGrid navGrid, int mapWidth, int mapHeight)
    {
        _rng = new Random(seed);
        _navGrid = navGrid ?? throw new ArgumentNullException(nameof(navGrid));
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
    }

    /// <summary>
    /// Spawns <paramref name="count"/> pawns into <paramref name="nativeWorld"/>,
    /// each on a unique passable tile, with a randomly generated name and
    /// fully-populated SkillsComponent. Publishes one PawnSpawnedEvent per
    /// pawn on the supplied <paramref name="services"/>.Pawns bus.
    /// </summary>
    /// <returns>EntityIds of the created pawns, in spawn order.</returns>
    public IReadOnlyList<EntityId> Spawn(NativeWorld nativeWorld, GameServices services, int count)
    {
        if (nativeWorld is null) throw new ArgumentNullException(nameof(nativeWorld));
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

        // Build the passable-tile pool, shuffle, take the first `count`.
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

        for (int i = passable.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(0, i + 1);
            (passable[i], passable[j]) = (passable[j], passable[i]);
        }

        // ── Phase A: per-entity K8.1-primitive allocation + native mint ────
        var entities = new EntityId[count];
        var positions = new PositionComponent[count];
        var identities = new IdentityComponent[count];
        var needs = new NeedsComponent[count];
        var minds = new MindComponent[count];
        var jobs = new JobComponent[count];
        var skills = new SkillsComponent[count];
        var movements = new MovementComponent[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = nativeWorld.CreateEntity();

            positions[i] = new PositionComponent { Position = passable[i] };

            string fullName = $"{Forenames[_rng.Next(Forenames.Length)]} " +
                              $"{Surnames[_rng.Next(Surnames.Length)]}";
            identities[i] = new IdentityComponent
            {
                Name = nativeWorld.InternString(fullName),
            };

            NativeMap<SkillKind, int> levels = nativeWorld.CreateMap<SkillKind, int>();
            NativeMap<SkillKind, float> experience = nativeWorld.CreateMap<SkillKind, float>();
            foreach (SkillKind kind in (SkillKind[])Enum.GetValues(typeof(SkillKind)))
            {
                levels.Set(kind, _rng.Next(0, SkillsComponent.MaxLevel + 1));
                experience.Set(kind, 0f);
            }
            skills[i] = new SkillsComponent { Levels = levels, Experience = experience };

            needs[i] = new NeedsComponent
            {
                Satiety = 0.9f,
                Hydration = 0.9f,
                Sleep = 0.9f,
                Comfort = 1.0f,
            };
            minds[i] = new MindComponent();
            jobs[i] = new JobComponent { Current = JobKind.Idle };
            movements[i] = new MovementComponent
            {
                Path = nativeWorld.CreateComposite<GridVector>(),
            };
        }

        // ── Phase B: bulk component add to NativeWorld (one P/Invoke per type) ─
        nativeWorld.AddComponents<PositionComponent>(entities, positions);
        nativeWorld.AddComponents<IdentityComponent>(entities, identities);
        nativeWorld.AddComponents<NeedsComponent>(entities, needs);
        nativeWorld.AddComponents<MindComponent>(entities, minds);
        nativeWorld.AddComponents<JobComponent>(entities, jobs);
        nativeWorld.AddComponents<SkillsComponent>(entities, skills);
        nativeWorld.AddComponents<MovementComponent>(entities, movements);

        // ── Event publication ─────────────────────────────────────────────────
        for (int i = 0; i < count; i++)
        {
            services.Pawns.Publish(new PawnSpawnedEvent
            {
                PawnId = entities[i],
                X = passable[i].X,
                Y = passable[i].Y,
            });
        }

        return entities;
    }
}
