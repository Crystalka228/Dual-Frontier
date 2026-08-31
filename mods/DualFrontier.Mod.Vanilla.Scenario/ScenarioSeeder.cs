using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Distribution;
using DualFrontier.Contracts.Math;
using DualFrontier.Contracts.Sdk;

namespace DualFrontier.Mod.Vanilla.Scenario;

/// <summary>
/// Seeds the starting colony the distribution manifest describes: colonists and items.
///
/// <para>
/// Terrain is NOT seeded here. It belongs to the grid and is scattered when the grid is built
/// (<see cref="ScenarioTerrain"/>), because a grid is constructed on every initialisation while
/// the colony is minted only once, and putting both behind the once-only guard cost the second
/// grid its walls.
/// </para>
///
/// <para>
/// This replaces two engine-side factories that took a concrete world and an engine bus. They
/// could not travel as written — <c>NativeWorld</c> and <c>GameServices</c> are engine types no
/// mod may name — so the placement rules were re-expressed against the SDK world surface. The
/// rules themselves are unchanged and are the ones the relocated factory tests pin: a pool of
/// passable tiles, shuffled, taken as a prefix; items excluded from pawn tiles; per-kind counts
/// and per-kind component values exactly as the scenario declares.
/// </para>
///
/// <para>
/// <b>This is not a system, and it cannot be one.</b> The scheduler enforces one writer per
/// component type across the whole graph — that global rule is what makes parallel phase dispatch
/// safe without locking — so a seeder that honestly declares the components it writes collides
/// with every gameplay system that owns one, and declaring fewer would undermine the guarantee
/// the rule exists to give. The engine-side factories never met this because they ran at
/// composition time, outside the graph entirely. The SDK grew a world-seeder hook so a mod can
/// stand where they stood.
/// </para>
///
/// <para>
/// <b>The span is released before anything is minted.</b> The world refuses mutation while a
/// read span is live, so the "have I already seeded" probe is a separate helper whose lease
/// closes before it returns. This is the same discipline the Weather singleton read carries, and
/// it is load-bearing rather than stylistic.
/// </para>
///
/// <para>
/// <b>No events are published.</b> The engine-side factories published spawn events so that a
/// bridge subscription elsewhere in the engine could turn them into render commands. Both ends
/// of that round trip now live in this mod, so the indirection is gone: the presentation system
/// reads component spans directly. That also keeps this mod free of any capability token, since
/// the SDK's publish and subscribe are gated on an owner-namespaced capability that no
/// engine-owned event type can satisfy — the kernel-provided set has been empty since the
/// capability ledger became owner-scoped.
/// </para>
/// </summary>
public sealed class ScenarioSeeder
{
    private static readonly string[] Forenames =
    {
        "Aldon", "Bryn", "Cass", "Dara", "Eryn", "Fen", "Gale", "Hale", "Ivo", "Jory",
        "Kest", "Lyra", "Mira", "Nell", "Orin", "Pell", "Quin", "Rhea", "Sable", "Tarn",
        "Umbra", "Vance", "Wren", "Xara", "Yorin", "Zeph",
    };

    private static readonly string[] Surnames =
    {
        "Ashcroft", "Blackwood", "Calder", "Dunmore", "Everly", "Fairbourne", "Grimsby",
        "Holloway", "Ironwood", "Jarrow", "Kingsley", "Lockhart", "Marsh", "Nightingale",
        "Oakhart", "Pemberton", "Quarles", "Ravenswood", "Stonebridge", "Thorne",
        "Underhill", "Vance", "Whitlock", "Yarrow",
    };

    private readonly ScenarioConfig _scenario;
    private readonly NavGrid _navGrid;
    private bool _seeded;

    /// <param name="scenario">What the distribution asked for.</param>
    /// <param name="navGrid">
    /// The grid this mod owns. It is passed in rather than built here because the pathfinding
    /// service the movement system receives must be built over the SAME grid, and the mod
    /// constructs both together at registration.
    /// </param>
    public ScenarioSeeder(ScenarioConfig scenario, NavGrid navGrid)
    {
        _scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
        _navGrid = navGrid ?? throw new ArgumentNullException(nameof(navGrid));
    }

    /// <summary>
    /// Seeds the colony. Invoked once by the host, outside the system graph, before the first
    /// tick.
    ///
    /// <para>
    /// It still reads before it mints. A reload re-registers the seeder against a world that
    /// already holds the colony, and re-seeding there would double the population rather than
    /// resume it — the same read-then-mint discipline the Weather singleton uses, for the same
    /// reason.
    /// </para>
    /// </summary>
    public void Seed(ISystemContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        if (_seeded || AlreadySeeded(context))
        {
            _seeded = true;
            return;
        }

        IReadOnlyList<GridVector> pawnTiles = SeedPawns(context);
        SeedItems(context, pawnTiles);
        _seeded = true;
    }

    /// <summary>
    /// True when the world already holds a colonist. The lease is released before returning,
    /// because the caller mints entities next and the world refuses mutation while a span is live.
    /// </summary>
    private static bool AlreadySeeded(ISystemContext context)
    {
        using SpanScope<IdentityComponent> span = context.AcquireSpan<IdentityComponent>();
        foreach ((EntityId _, IdentityComponent _) in span.Pairs)
            return true;
        return false;
    }

    private IReadOnlyList<GridVector> SeedPawns(ISystemContext context)
    {
        var rng = new Random(_scenario.FactorySeed);
        List<GridVector> pool = PassableTiles(excluded: null);
        int count = _scenario.Counts.Pawns;
        if (pool.Count < count)
        {
            throw new InvalidOperationException(
                $"Scenario '{_scenario.Id}' asks for {count} colonists but only {pool.Count} " +
                "passable tiles are available. Reduce the count or the obstacle density.");
        }
        Shuffle(pool, rng);

        var entities = new EntityId[count];
        var positions = new PositionComponent[count];
        var identities = new IdentityComponent[count];
        var needs = new NeedsComponent[count];
        var minds = new MindComponent[count];
        var jobs = new JobComponent[count];
        var skills = new SkillsComponent[count];
        var movements = new MovementComponent[count];
        var taken = new List<GridVector>(count);

        for (int i = 0; i < count; i++)
        {
            entities[i] = context.CreateEntity();
            positions[i] = new PositionComponent { Position = pool[i] };
            taken.Add(pool[i]);

            string fullName = $"{Forenames[rng.Next(Forenames.Length)]} " +
                              $"{Surnames[rng.Next(Surnames.Length)]}";
            identities[i] = new IdentityComponent { Name = context.InternString(fullName) };

            var rolled = new SkillsComponent { Populated = true };
            foreach (SkillKind kind in Enum.GetValues<SkillKind>())
            {
                rolled.SetLevel(kind, rng.Next(0, SkillsComponent.MaxLevel + 1));
                rolled.SetExperience(kind, 0f);
            }
            skills[i] = rolled;

            needs[i] = new NeedsComponent { Satiety = 1f, Hydration = 1f, Sleep = 1f, Comfort = 0.5f };
            minds[i] = new MindComponent { Mood = 0.5f };
            jobs[i] = new JobComponent { Current = JobKind.Idle };
            movements[i] = new MovementComponent
            {
                Path = context.CreateComposite<GridVector>(),
                PathStepIndex = 0,
                StepCooldown = 0,
            };
        }

        Write(context, entities, positions);
        Write(context, entities, identities);
        Write(context, entities, needs);
        Write(context, entities, minds);
        Write(context, entities, jobs);
        Write(context, entities, skills);
        Write(context, entities, movements);

        return taken;
    }

    private void SeedItems(ISystemContext context, IReadOnlyList<GridVector> pawnTiles)
    {
        var rng = new Random(_scenario.ItemFactorySeed);
        List<GridVector> pool = PassableTiles(excluded: new HashSet<GridVector>(pawnTiles));

        ScenarioCounts c = _scenario.Counts;
        int total = c.Food + c.Water + c.Beds + c.Decorations;
        if (pool.Count < total)
        {
            throw new InvalidOperationException(
                $"Scenario '{_scenario.Id}' asks for {total} items but only {pool.Count} " +
                "passable tiles remain once colonist tiles are excluded.");
        }
        Shuffle(pool, rng);

        int cursor = 0;
        SpawnKind(context, pool, ref cursor, c.Food,
            _ => new ConsumableComponent
            {
                RestoresKind = NeedKind.Satiety,
                RestorationAmount = 0.4f,
                Charges = 1,
            });
        SpawnKind(context, pool, ref cursor, c.Water,
            _ => new WaterSourceComponent { RestorationAmount = 0.5f });
        SpawnKind(context, pool, ref cursor, c.Beds,
            _ => new BedComponent { Occupant = null, SleepRestorationPerTick = 0.005f });
        SpawnKind(context, pool, ref cursor, c.Decorations,
            _ => new DecorativeAuraComponent { Radius = 3, ComfortPerTick = 0.001f });
    }

    private static void SpawnKind<T>(
        ISystemContext context,
        List<GridVector> pool,
        ref int cursor,
        int count,
        Func<int, T> make) where T : unmanaged, IComponent
    {
        if (count == 0) return;

        var entities = new EntityId[count];
        var positions = new PositionComponent[count];
        var components = new T[count];
        for (int i = 0; i < count; i++)
        {
            entities[i] = context.CreateEntity();
            positions[i] = new PositionComponent { Position = pool[cursor++] };
            components[i] = make(i);
        }

        Write(context, entities, positions);
        Write(context, entities, components);
    }

    /// <summary>
    /// The SDK's batched write. The engine factories used a bulk add that took both arrays at
    /// once; the SDK batches per entity and flushes, which is the same work in a shape a mod can
    /// reach. The flush is explicit rather than left to disposal so a failure surfaces here.
    /// </summary>
    private static void Write<T>(ISystemContext context, EntityId[] entities, T[] values)
        where T : unmanaged, IComponent
    {
        using WriteScope<T> batch = context.BeginBatch<T>();
        for (int i = 0; i < entities.Length; i++)
            batch.Add(entities[i], values[i]);
        batch.Flush();
    }

    private List<GridVector> PassableTiles(HashSet<GridVector>? excluded)
    {
        var pool = new List<GridVector>(_scenario.MapWidth * _scenario.MapHeight);
        for (int y = 0; y < _scenario.MapHeight; y++)
        {
            for (int x = 0; x < _scenario.MapWidth; x++)
            {
                var tile = new GridVector(x, y);
                if (!_navGrid.IsPassable(x, y)) continue;
                if (excluded is not null && excluded.Contains(tile)) continue;
                pool.Add(tile);
            }
        }
        return pool;
    }

    private static void Shuffle(List<GridVector> tiles, Random rng)
    {
        for (int i = tiles.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (tiles[i], tiles[j]) = (tiles[j], tiles[i]);
        }
    }
}
