using System;
using System.Collections.Generic;
using System.Diagnostics;
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
///
/// K8.3+K8.4 combined milestone (2026-05-14) — Spawn signature gained
/// a <see cref="NativeWorld"/> parameter and the body adopted the two-
/// phase pattern (per-entity K8.1-primitive allocation → bulk
/// <see cref="NativeWorld.AddComponents{T}"/> → dual-write to managed
/// <see cref="World"/> during transition). Phase A per-entity ids are
/// minted on BOTH worlds in lockstep (both start <c>_nextIndex = 1</c>
/// so indices align naturally; verified by Debug.Assert) — preserves
/// <c>world.IsAlive(id)</c> semantics for unmigrated systems while
/// <see cref="NativeWorld"/> sees the entity as alive for
/// <see cref="NativeWorld.AcquireSpan{T}"/> reads. Phase 5 commit 21
/// removes the managed-side mint + dual-write, leaving the factory
/// nativeWorld-only.
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

    public RandomPawnFactory(int seed, NavGrid navGrid, int mapWidth, int mapHeight)
    {
        _rng = new Random(seed);
        _navGrid = navGrid ?? throw new ArgumentNullException(nameof(navGrid));
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
    }

    /// <summary>
    /// Spawns <paramref name="count"/> pawns into both <paramref name="nativeWorld"/>
    /// and <paramref name="world"/> (dual-write transition pattern, Phase 2-4),
    /// each on a unique passable tile, with a randomly generated name and
    /// fully-populated SkillsComponent. Publishes one PawnSpawnedEvent per
    /// pawn on the supplied <paramref name="services"/>.Pawns bus.
    /// </summary>
    /// <returns>EntityIds of the created pawns, in spawn order.</returns>
    public IReadOnlyList<EntityId> Spawn(NativeWorld nativeWorld, World world,
                                         GameServices services, int count)
    {
        if (nativeWorld is null) throw new ArgumentNullException(nameof(nativeWorld));
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

        // ── Phase A: per-entity K8.1-primitive allocation + lockstep mint ─────
        // Mint on BOTH worlds. Both World.CreateEntity() and NativeWorld.CreateEntity()
        // assign monotonic indices starting at 1; called in lockstep they produce
        // matching ids — preserving managed-world IsAlive semantics for unmigrated
        // systems AND native-side is_alive semantics for AcquireSpan reads.
        // Phase 5 commit 21 removes the managed-side mint when the factory becomes
        // nativeWorld-only.
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
            EntityId managedId = world.CreateEntity();
            EntityId nativeId = nativeWorld.CreateEntity();
            // Both worlds start _nextIndex=1 and grow monotonically. Drift is a
            // bootstrap invariant violation (someone called CreateEntity outside
            // the factory between iterations). Debug-only assert; production runs
            // strip the check.
            Debug.Assert(managedId.Index == nativeId.Index,
                "Managed/native entity index drift during dual-write transition — " +
                "CreateEntity was called outside the factory between iterations.");
            entities[i] = managedId;

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

        // ── Dual-write to managed World during transition (Phase 2-4) ─────────
        // Removed in Phase 5 commit 21 once all 12 production systems migrate
        // to NativeWorld.AcquireSpan reads. Until then, unmigrated systems read
        // via World.GetEntitiesWith / GetComponentUnsafe and must see the same
        // entity-component records the native side carries.
        for (int i = 0; i < count; i++)
        {
            world.AddComponent(entities[i], positions[i]);
            world.AddComponent(entities[i], identities[i]);
            world.AddComponent(entities[i], needs[i]);
            world.AddComponent(entities[i], minds[i]);
            world.AddComponent(entities[i], jobs[i]);
            world.AddComponent(entities[i], skills[i]);
            world.AddComponent(entities[i], movements[i]);
        }

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
