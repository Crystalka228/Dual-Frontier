# Real pawn data — IdentityComponent + populated SkillsComponent + 10 randomized colonists + UI wiring

## Operating principle (load-bearing)

> «Будем работать без заглушек которые обманывают состояние, оно либо есть, либо его нет вообще.»

UI displays only what real components carry. If a piece of data does not exist in any component, the UI does NOT show a fabricated value, a hash-derived value, or a hardcoded array indexed by EntityId. Either the data exists end-to-end (component → event → command → widget) or the corresponding UI element does not exist.

This brief enforces that principle by **adding the missing data layer** rather than removing the UI element. Specifically:
- Pawn names get a real `IdentityComponent` (data layer) instead of `Names[]` placeholder array
- Pawn skills get populated `SkillsComponent` (component already exists, currently empty) and flow through to UI
- Role label stays REMOVED — no component exists for role/class concept yet, UI has no business showing one
- 3 hardcoded pawns become 10 randomly generated colonists with deterministic-by-seed data

## Audit baseline

Comprehensive audit captured in `docs/audit/UI_REVIEW_PRE_M75B2.md` (created 2026-05-02). Findings reused here without re-derivation:

- Architecture (bridge, dispatcher, layer separation): ✅ correct, no change needed
- Display values that ARE real: position, mood (value+label), need bars (value flows correctly), job label, tick counter
- Display values that ARE fake: pawn names (Warhammer-flavored array), pawn role (no component exists), skill bars (SkillsComponent ignored, hash-derived display)
- Dead code: 5 stub UI files, 3 undispatched render commands
- Phase 5 territory deferred: NeedsSystem decay direction, job loop, MovementSystem job-aware wandering

## Out of scope

- ANY change to `src/DualFrontier.Core` or `src/DualFrontier.Contracts`. M-phase boundary preserved. Verified by `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returning empty.
- NeedsSystem decay direction fix — Phase 5 (requires job loop to land first; isolated change here would just make pawns starve to death silently).
- Job loop completion (food/water/bed entities, Eat/Drink/Sleep job assignment) — Phase 5.
- MovementSystem JobLabel honesty (currently shows "Idle" while pawns wander randomly) — Phase 5.
- HealthComponent / FactionComponent / RaceComponent display — Phase 5 (these components exist but are not added to spawned pawns; they would need both component-data and UI sections).
- Mana / Ether magic system display — Phase 5.
- Combat command dispatch (ProjectileSpawnedCommand etc.) — Phase 5; these 3 unused commands are deleted in this brief.
- Save format compatibility — there is no save system actively used in production yet. If `SaveSystem.cs` references a fixed pawn count or component shape, flag and STOP.
- Mod-menu UI scene (M7.5.B.2) — separate session immediately following this housekeeping closure.

## Approved architectural decisions

1. **New `IdentityComponent`.** Lives in `src/DualFrontier.Components/Pawn/IdentityComponent.cs`. Pure POCO, no logic, single field `Name` (string). Default value: empty string. Pawns spawned without explicit IdentityComponent assignment carry empty string — this is intentional, the contract is "if you want a name, attach the component". Future fields (rank, biography, portrait id) can be added later without breaking changes.

2. **`SkillsComponent` population at spawn time.** The component already exists with `Levels: Dictionary<SkillKind, int>` and `Experience: Dictionary<SkillKind, float>` fields. The factory populates `Levels` for every `SkillKind` enum value (all 13: Construction, Mining, Cooking, Crafting, Medicine, Combat, Shooting, Magic, Research, Social, Animals, Farming, Hauling). Experience starts at 0 for every skill. Levels are random integers in [0, MaxLevel=20].

3. **New `RandomPawnFactory` in `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs`.** Constructor: `(int seed, NavGrid navGrid, int mapWidth, int mapHeight)`. Method: `IReadOnlyList<EntityId> Spawn(World world, GameServices services, int count)`. For each requested pawn: picks a random unique passable tile coord, generates a random name from internal pools (forename + surname), assigns random skill levels per SkillKind, attaches all required components, publishes PawnSpawnedEvent. Deterministic given the same seed.

4. **Name pools embedded in `RandomPawnFactory`.** Two static readonly arrays of generic-fantasy/sci-fi-neutral forenames and surnames, ~25-30 each, yielding 600+ unique combinations (more than enough for 10-pawn requirement and any future expansion). NOT Warhammer-flavored. NOT placeholder-style ("Pawn 1"). Real-feeling colonist names that don't pin to any specific franchise. Examples: "Aelin Brand", "Cara Drake", "Jona Marsh".

5. **10 pawns spawned, replacing the 3-pawn hardcoded list.** `GameBootstrap.SpawnInitialPawns` is removed; its logic moves into `RandomPawnFactory.Spawn(world, services, 10)`. The `InitialPawnPositions` static array is removed. The seed for the factory is a constant 42 in production (deterministic across F5 runs) but tests can override.

6. **Spawn position selection: random unique passable tiles.** Factory builds list of all `(x, y)` tiles where `navGrid.IsPassable(x, y)` is true, shuffles via seeded RNG, takes the first N. Guarantees: (a) no two pawns at the same tile, (b) all spawn tiles are passable (not on rock obstacles), (c) deterministic given seed. If passable tile count is less than requested, throws `InvalidOperationException("Not enough passable tiles to spawn N pawns")` — defensive against edge cases.

7. **Initial component values for spawned pawns.** Per-pawn at spawn:
    - `PositionComponent { Position = tilePosition }` (passable random)
    - `IdentityComponent { Name = $"{forename} {surname}" }` (random from pools)
    - `SkillsComponent { Levels = new Dictionary<SkillKind, int>(), Experience = new Dictionary<SkillKind, float>() }` populated for every SkillKind: `Levels[k] = rng.Next(0, MaxLevel+1)`, `Experience[k] = 0f`
    - `NeedsComponent { Hunger = 0.1f, Thirst = 0.1f, Rest = 0.1f, Comfort = 0f }` (matches existing initial values; Phase 5 reviews these)
    - `MindComponent()` defaults (Mood = 0.5)
    - `JobComponent { Current = JobKind.Idle }`
    - `MovementComponent()` defaults

8. **Top-3 skills propagated to UI.** `PawnStateChangedEvent` extended with `IReadOnlyList<(SkillKind Kind, int Level)> TopSkills` field. `PawnStateReporterSystem` computes top 3 by descending level (ties broken by `SkillKind` enum order) and publishes them. `PawnStateCommand` mirrors the field. `PawnDetail` re-adds the SKILLS section using **real values** from the command, displaying skill name + level number + pip bar. The existing `SkillRow` partial class is repurposed to take real `(string name, int level)` and stays largely as-is.

9. **Display all 13 skills?** No — top 3 only, matches existing layout space in PawnDetail (260px-wide right panel). Phase 5 may add an "Inspector" mode that shows full skill grid.

10. **`PawnStateReporterSystem` reads `IdentityComponent.Name` and `SkillsComponent.Levels` directly.** The hardcoded `Names[]` array and `ResolveName` static method are deleted. If a pawn lacks `IdentityComponent`, the reporter publishes `Name = ""` (empty string, NOT a fabricated fallback). UI will display empty name. This is the load-bearing principle: data exists end-to-end or it doesn't show.

11. **Top-skills computation: explicit loop, no LINQ.** `Systems/Pawn/` has zero LINQ usage in existing code; explicit loop preserves convention. Build a temporary `(SkillKind, int)` array sized 13, sort in-place (insertion sort is fine for fixed-size 13), return top 3. Allocation: one short-lived array per pawn per SLOW tick — acceptable.

12. **Empty SKILLS section behaviour when pawn has no skills.** If `TopSkills` is empty (e.g. pawn missing SkillsComponent or Levels dictionary uninitialized), PawnDetail renders the SKILLS section header but no rows. Honest signal: "this pawn has no skill data". No fake data, no hidden section.

13. **Delete dead code in same housekeeping pass.** 5 stub UI files (`BuildMenu.cs`, `AlertPanel.cs`, `ManaBar.cs`, `PawnInspector.cs`, `ProjectileVisual.cs`) and 3 undispatched render commands (`ProjectileSpawnedCommand.cs`, `SpellCastCommand.cs`, `UIUpdateCommand.cs`) — all confirmed unused via grep, all Phase 3 leftover scaffolding. Phase 5 will recreate with real implementations.

14. **PawnDetail: remove fake role label permanently.** Field `_roleLabel`, helper `MakeRole`, and the role line in `BuildHeader` are removed. Header becomes avatar + name only. No role concept exists in any component; UI has nothing to display. Phase 5 may add it.

15. **Test coverage extension.** New tests for: `RandomPawnFactory` deterministic generation, unique names within a batch, position uniqueness, position passability, all 13 skills populated. Plus integration smoke that bootstrap actually produces 10 pawns and that PawnStateChangedEvent carries real Name + TopSkills. Target: +9 tests, 417 → 426.

16. **METHODOLOGY §2.4 atomic phase review** — implementation, tests, ROADMAP closure all in one session. Three commits per §7.3 (`feat → test → docs`).

17. **Boundary discipline**: Components, Events, Systems, Application, Presentation are all freely modifiable in this housekeeping pass. Only Core and Contracts are protected. Verified by post-commit boundary diff check.

## Required reading

1. **Audit context** (do NOT re-derive): `docs/audit/UI_REVIEW_PRE_M75B2.md` — full audit findings document.
2. **Components currently on pawns** (full files):
    - `src/DualFrontier.Components/Pawn/NeedsComponent.cs` — initial values reference
    - `src/DualFrontier.Components/Pawn/MindComponent.cs` — defaults reference
    - `src/DualFrontier.Components/Pawn/JobComponent.cs` — defaults reference
    - `src/DualFrontier.Components/Pawn/MovementComponent.cs` — defaults reference
    - `src/DualFrontier.Components/Pawn/SkillsComponent.cs` — already exists, will be populated
    - `src/DualFrontier.Components/Pawn/SkillKind.cs` — 13-value enum
    - `src/DualFrontier.Components/Shared/PositionComponent.cs` — used at spawn
3. **Events** (full file): `src/DualFrontier.Events/Pawn/PawnStateChangedEvent.cs` — record to extend with TopSkills.
4. **Bridge command** (full file): `src/DualFrontier.Application/Bridge/Commands/PawnStateCommand.cs` — record to extend with TopSkills.
5. **Bootstrap** (full file): `src/DualFrontier.Application/Loop/GameBootstrap.cs` — `SpawnInitialPawns` to be replaced by RandomPawnFactory call.
6. **Reporter** (full file): `src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs` — strip Names[] + ResolveName, read IdentityComponent.Name, compute TopSkills.
7. **UI** (full file): `src/DualFrontier.Presentation/UI/PawnDetail.cs` — strip role and DemoSkills/MakeRole, re-add SKILLS section using real data via SkillRow.
8. **Pathfinding** (head only): `src/DualFrontier.AI/Pathfinding/NavGrid.cs` — to confirm `IsPassable(x, y)` API or equivalent. If method differs, adapt to existing API; do NOT modify NavGrid.
9. **Existing tests** (head only): `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — confirm none assert pawn count == 3 or specific names. If any do, update them as part of this commit pass to expect count == 10 or assert presence of Name without specific value.
10. **Save system** (head only): `src/DualFrontier.Application/Save/SaveSystem.cs` — confirm it does not hardcode pawn count or component layout. If it does and would break, STOP.
11. **CODING_STANDARDS** (full): `docs/CODING_STANDARDS.md` — one class per file, English-only comments, `_camelCase` private fields, member order.
12. **METHODOLOGY** (relevant sections): `docs/METHODOLOGY.md` §2.4, §7.3.
13. **ROADMAP**: `docs/ROADMAP.md` — locate Backlog section.

Pre-flight grep checks before starting commit 2 (deletion):
- `grep -rn "BuildMenu" src/ tests/ src/DualFrontier.Presentation/Scenes/` — should return only the file being deleted
- Same for `AlertPanel`, `ManaBar`, `PawnInspector`, `ProjectileVisual`, `ProjectileSpawnedCommand`, `SpellCastCommand`, `UIUpdateCommand`
- Any external reference → STOP, do not delete that file, escalate.

## Implementation

### Layer 1 — Component (1 new file)

`src/DualFrontier.Components/Pawn/IdentityComponent.cs`:

```csharp
namespace DualFrontier.Components.Pawn;

using DualFrontier.Contracts.Core;

/// <summary>
/// Pure POCO carrying a pawn's identity data. Currently a single field —
/// <see cref="Name"/> — populated by <c>RandomPawnFactory</c> (or any
/// future scenario loader). Pawns lacking this component carry no name;
/// <c>PawnStateReporterSystem</c> publishes empty <c>Name</c> in that
/// case. The UI displays empty name verbatim — no fabricated fallback.
/// Future fields (rank, biography, portrait id) can be added later.
/// </summary>
public sealed class IdentityComponent : IComponent
{
    /// <summary>
    /// Display name. Empty string by default. Set at spawn time by the
    /// scenario loader; not mutated thereafter except by future rename
    /// systems.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
```

### Layer 2 — Factory (1 new file)

`src/DualFrontier.Application/Scenario/RandomPawnFactory.cs`:

```csharp
using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
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
public sealed class RandomPawnFactory
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

        world.AddComponent(id, new IdentityComponent
        {
            Name = $"{Forenames[_rng.Next(Forenames.Length)]} {Surnames[_rng.Next(Surnames.Length)]}"
        });

        var levels = new Dictionary<SkillKind, int>();
        var experience = new Dictionary<SkillKind, float>();
        foreach (SkillKind kind in Enum.GetValues<SkillKind>())
        {
            levels[kind] = _rng.Next(0, SkillsComponent.MaxLevel + 1);
            experience[kind] = 0f;
        }
        world.AddComponent(id, new SkillsComponent
        {
            Levels = levels,
            Experience = experience
        });

        world.AddComponent(id, new NeedsComponent
        {
            Hunger = 0.1f,
            Thirst = 0.1f,
            Rest = 0.1f,
            Comfort = 0f
        });
        world.AddComponent(id, new MindComponent());
        world.AddComponent(id, new JobComponent { Current = JobKind.Idle });
        world.AddComponent(id, new MovementComponent());

        services.Pawns.Publish(new PawnSpawnedEvent
        {
            PawnId = id,
            X = pos.X,
            Y = pos.Y
        });

        return id;
    }
}
```

**Implementation notes for the agent:**
- If `NavGrid.IsPassable(x, y)` doesn't exist, find the equivalent (`navGrid.GetTile(x,y)` returning a passable bool, or `navGrid.IsBlocked` — invert). Adapt the code, do NOT modify NavGrid.
- If `Enum.GetValues<SkillKind>()` (generic form) is unavailable due to .NET version, use `(SkillKind[])Enum.GetValues(typeof(SkillKind))`.
- `PawnSpawnedEvent` is the existing event in `DualFrontier.Events.Pawn`. Confirm its shape matches `{ EntityId PawnId, int X, int Y }` from current GameBootstrap usage.
- `GameServices.Pawns` is the existing bus aggregator.

### Layer 3 — Event extension

`src/DualFrontier.Events/Pawn/PawnStateChangedEvent.cs`:

Add to existing record (replace whole record):

```csharp
using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

public sealed record PawnStateChangedEvent : IEvent
{
    public required EntityId PawnId    { get; init; }
    public required string   Name      { get; init; }
    public required float    Hunger    { get; init; }
    public required float    Thirst    { get; init; }
    public required float    Rest      { get; init; }
    public required float    Comfort   { get; init; }
    public required float    Mood      { get; init; }
    public required string   JobLabel  { get; init; }
    public required bool     JobUrgent { get; init; }

    /// <summary>
    /// Top 3 skills by descending level. Each entry is the SkillKind and
    /// the integer level (0..SkillsComponent.MaxLevel). May be empty if
    /// the pawn has no SkillsComponent or its Levels dictionary is null.
    /// Order: highest level first; ties broken by SkillKind enum order.
    /// </summary>
    public required IReadOnlyList<(SkillKind Kind, int Level)> TopSkills { get; init; }
}
```

This adds a project reference: `DualFrontier.Events.csproj` must reference `DualFrontier.Components`. Add the reference if not present:

```xml
<ProjectReference Include="..\DualFrontier.Components\DualFrontier.Components.csproj" />
```

If the reference creates a cycle (Components currently doesn't reference Events, but verify), abort and use a duplicate `SkillKind` enum in Events — but only if cycle exists. Most likely it doesn't.

### Layer 4 — Bridge command extension

`src/DualFrontier.Application/Bridge/Commands/PawnStateCommand.cs`:

Replace whole file:

```csharp
using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Periodic snapshot of one pawn's HUD-relevant state. Published by
/// GameBootstrap in response to a domain PawnStateChangedEvent.
/// All need values are normalised "wellness" 0..1 (1 = best), already
/// inverted from <c>NeedsComponent</c> deficit semantics by the reporter
/// system, so the HUD can render bars without further translation.
/// <see cref="TopSkills"/> carries up to 3 highest skill levels for
/// PawnDetail's SKILLS section; empty if the pawn has no skill data.
/// </summary>
public sealed record PawnStateCommand(
    EntityId PawnId,
    string Name,
    float Hunger,
    float Thirst,
    float Rest,
    float Comfort,
    float Mood,
    string JobLabel,
    bool JobUrgent,
    IReadOnlyList<(SkillKind Kind, int Level)> TopSkills
) : IRenderCommand
{
    public void Execute(object renderContext)
    {
        /* Dispatched by RenderCommandDispatcher, not via Execute. */
    }
}
```

### Layer 5 — Reporter system

`src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs` — full rewrite:

```csharp
using System;
using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// HUD bridge: each SLOW tick, emits a PawnStateChangedEvent per pawn so
/// GameBootstrap can forward the data as a PawnStateCommand to the
/// presentation HUD. Read-only on pawn components; only publishes on
/// the Pawns bus.
///
/// Operating principle "data exists or it doesn't": Name comes from
/// IdentityComponent (empty string if absent), TopSkills from
/// SkillsComponent.Levels (empty list if absent or uninitialized). No
/// hardcoded placeholder names, no hash-derived fallback values.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(NeedsComponent), typeof(MindComponent), typeof(JobComponent),
                    typeof(IdentityComponent), typeof(SkillsComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.SLOW)]
public sealed class PawnStateReporterSystem : SystemBase
{
    private const int TopSkillCount = 3;

    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        foreach (var pawn in Query<NeedsComponent, JobComponent>())
        {
            var needs = GetComponent<NeedsComponent>(pawn);
            var mind  = GetComponent<MindComponent>(pawn);
            var job   = GetComponent<JobComponent>(pawn);

            string name = TryGetComponent<IdentityComponent>(pawn, out var identity)
                ? identity.Name
                : string.Empty;

            IReadOnlyList<(SkillKind Kind, int Level)> topSkills =
                TryGetComponent<SkillsComponent>(pawn, out var skills)
                    ? ComputeTopSkills(skills)
                    : Array.Empty<(SkillKind, int)>();

            Services.Pawns.Publish(new PawnStateChangedEvent
            {
                PawnId    = pawn,
                Name      = name,
                Hunger    = 1f - needs.Hunger,
                Thirst    = 1f - needs.Thirst,
                Rest      = 1f - needs.Rest,
                Comfort   = 1f - needs.Comfort,
                Mood      = mind.Mood,
                JobLabel  = TranslateJob(job.Current),
                JobUrgent = job.Current == JobKind.Eat || job.Current == JobKind.Sleep,
                TopSkills = topSkills,
            });
        }
    }

    private static IReadOnlyList<(SkillKind Kind, int Level)> ComputeTopSkills(SkillsComponent skills)
    {
        if (skills.Levels is null || skills.Levels.Count == 0)
            return Array.Empty<(SkillKind, int)>();

        // Build a working array of all (kind, level) pairs, sort by level
        // descending (ties broken by enum order — already the iteration
        // order from the dictionary if populated in enum order, but we
        // explicitly sort to be safe), take top N.
        var pairs = new (SkillKind Kind, int Level)[skills.Levels.Count];
        int i = 0;
        foreach (var kv in skills.Levels)
            pairs[i++] = (kv.Key, kv.Value);

        // Insertion sort — fixed size 13, allocation-free, O(n^2) trivial.
        for (int a = 1; a < pairs.Length; a++)
        {
            var cur = pairs[a];
            int b = a - 1;
            while (b >= 0 && CompareDesc(pairs[b], cur) > 0)
            {
                pairs[b + 1] = pairs[b];
                b--;
            }
            pairs[b + 1] = cur;
        }

        int take = Math.Min(TopSkillCount, pairs.Length);
        var result = new (SkillKind, int)[take];
        Array.Copy(pairs, result, take);
        return result;
    }

    /// <summary>
    /// Comparer: descending by level, ascending by SkillKind enum value
    /// for ties. Returns negative if `a` should come BEFORE `b` in the
    /// sorted (top-first) order; positive otherwise.
    /// </summary>
    private static int CompareDesc((SkillKind Kind, int Level) a, (SkillKind Kind, int Level) b)
    {
        if (a.Level != b.Level) return b.Level - a.Level;
        return (int)a.Kind - (int)b.Kind;
    }

    private static string TranslateJob(JobKind kind) => kind switch
    {
        JobKind.Idle         => "Idle",
        JobKind.Haul         => "Hauling",
        JobKind.Build        => "Building",
        JobKind.Mine         => "Mining",
        JobKind.Cook         => "Cooking",
        JobKind.Craft        => "Crafting",
        JobKind.Research     => "Researching",
        JobKind.Medicate     => "Medicating",
        JobKind.Fight        => "Fighting",
        JobKind.Flee         => "Fleeing",
        JobKind.Meditate     => "Meditating",
        JobKind.GolemCommand => "Commanding golem",
        JobKind.Sleep        => "Sleeping",
        JobKind.Eat          => "Foraging",
        JobKind.Social       => "Socialising",
        _                    => "—"
    };
}
```

**Implementation notes:**
- `TryGetComponent<T>(entity, out T comp)` may not be the exact API name on `SystemBase`. Check `GetComponent<T>` vs `TryGetComponent` patterns. If only `GetComponent<T>` exists and throws when absent, wrap in try/catch OR use `world.HasComponent<T>` if available. Adapt to existing API — do NOT modify `SystemBase` or `World`.
- The `[SystemAccess]` reads list now includes `IdentityComponent` and `SkillsComponent`. This may change the dependency graph — verify build succeeds without ordering complaints.
- The `Query<NeedsComponent, JobComponent>` doesn't include the new components in the query predicate — that's intentional, we want pawns that have at least Needs+Job to be reported, even if Identity/Skills are missing (graceful degradation per "data exists or it doesn't" — UI shows empty Name).

### Layer 6 — Bootstrap update

`src/DualFrontier.Application/Loop/GameBootstrap.cs` — modify in two places:

**Remove the `InitialPawnPositions` static array** (top of class).

**Replace the `SpawnInitialPawns` private static method** call. The current call:

```csharp
SpawnInitialPawns(world, services);
```

becomes:

```csharp
const int InitialPawnCount = 10;
const int FactorySeed = 42;
var pawnFactory = new RandomPawnFactory(FactorySeed, navGrid, MapWidth, MapHeight);
pawnFactory.Spawn(world, services, InitialPawnCount);
```

where `MapWidth = 50, MapHeight = 50` are added as `private const int` fields on `GameBootstrap` (currently those values are inline in the `new NavGrid(50, 50)` and obstacle loop — extract them to constants for the factory).

Note: this means the factory's spawn is called BEFORE the `coreSystems` array is built (the factory only needs `world`, `services`, `navGrid` — not the scheduler). Place the factory call after `navGrid` is built and obstacles are stamped, but before scheduler/pipeline construction.

**Delete the `SpawnInitialPawns` method body** (entire private static method) since logic moved into factory.

**Add `using DualFrontier.Application.Scenario;`** at top.

### Layer 7 — UI re-add SKILLS section + remove role label

`src/DualFrontier.Presentation/UI/PawnDetail.cs` — multiple edits:

**Remove role label** (per audit Decision A):
- Remove `_roleLabel` field declaration
- Remove `_roleLabel` setup block in `BuildHeader`
- Remove `_roleLabel.Text = MakeRole(id);` line in `Render`
- Delete the `MakeRole` static method entirely

**Re-add SKILLS section, but driven by real data:**
- Keep `_skillsBox` field declaration
- Keep `BuildSkills()` method (creates the VBoxContainer placeholder)
- Keep `BuildSkills()` call in `_Ready`
- **Replace `RenderSkills(EntityId id)`** with `RenderSkills(IReadOnlyList<(SkillKind Kind, int Level)> topSkills)`:

```csharp
private void RenderSkills(IReadOnlyList<(SkillKind Kind, int Level)> topSkills)
{
    foreach (var child in _skillsBox.GetChildren())
        child.QueueFree();

    foreach (var (kind, level) in topSkills)
        _skillsBox.AddChild(new SkillRow(kind.ToString(), level));
}
```

- **Delete `DemoSkills` static method entirely.**

**Update `PawnState` record** to include `IReadOnlyList<(SkillKind Kind, int Level)> TopSkills`:

```csharp
private readonly record struct PawnState(
    string Name, float Hunger, float Thirst, float Rest, float Comfort,
    float Mood, string JobLabel, bool JobUrgent,
    IReadOnlyList<(SkillKind Kind, int Level)> TopSkills);
```

**Update `UpdatePawn` signature** to accept TopSkills:

```csharp
public void UpdatePawn(
    EntityId id, string name, float hunger, float thirst, float rest,
    float comfort, float mood, string jobLabel, bool jobUrgent,
    IReadOnlyList<(SkillKind Kind, int Level)> topSkills)
{
    _states[id] = new PawnState(name, hunger, thirst, rest, comfort, mood,
        jobLabel, jobUrgent, topSkills);
    if (_shown is null) _shown = id;
    if (_shown.HasValue && _shown.Value.Equals(id))
        Render(_states[id], id);
}
```

**Update `Render`** to call new `RenderSkills`:

```csharp
private void Render(PawnState s, EntityId id)
{
    _initials.Text  = MakeInitials(s.Name);
    _nameLabel.Text = s.Name.ToUpperInvariant();
    // _roleLabel removed

    int moodPct = (int)MathF.Round(s.Mood * 100f);
    _moodValue.Text = moodPct.ToString();
    _moodMood.Text  = MoodLabel(s.Mood);
    _moodBar.Color  = StatusColor(s.Mood);
    _moodBar.CustomMinimumSize = new Vector2(BarWidth(s.Mood), 4);

    _hunger.Set(s.Hunger);
    _thirst.Set(s.Thirst);
    _rest.Set(s.Rest);
    _comfort.Set(s.Comfort);

    _jobDot.Color  = JobDotColor(s.JobLabel, s.JobUrgent);
    _jobLabel.Text = s.JobLabel;

    RenderSkills(s.TopSkills);
}
```

**Add `using` directives** at top of PawnDetail.cs:

```csharp
using System.Collections.Generic;
using DualFrontier.Components.Pawn;
```

(`System.Collections.Generic` may already be present.)

**SkillRow stays largely unchanged** but ensure it's not deleted accidentally — it's used by RenderSkills now.

### Layer 8 — GameHUD passthrough update

`src/DualFrontier.Presentation/UI/GameHUD.cs`:

`UpdatePawn` method needs to pass TopSkills to PawnDetail:

```csharp
public void UpdatePawn(PawnStateCommand cmd)
{
    _colony.UpdatePawn(cmd.PawnId, cmd.Name, cmd.JobLabel, cmd.Mood);
    _detail.UpdatePawn(
        cmd.PawnId, cmd.Name, cmd.Hunger, cmd.Thirst, cmd.Rest,
        cmd.Comfort, cmd.Mood, cmd.JobLabel, cmd.JobUrgent,
        cmd.TopSkills);
}
```

### Layer 9 — GameBootstrap subscription update

`src/DualFrontier.Application/Loop/GameBootstrap.cs` — the `services.Pawns.Subscribe<PawnStateChangedEvent>` lambda needs to forward TopSkills:

```csharp
services.Pawns.Subscribe<PawnStateChangedEvent>(e =>
    bridge.Enqueue(new PawnStateCommand(
        e.PawnId, e.Name, e.Hunger, e.Thirst, e.Rest, e.Comfort,
        e.Mood, e.JobLabel, e.JobUrgent, e.TopSkills)));
```

### Layer 10 — Delete dead code (8 files)

`git rm` each:

- `src/DualFrontier.Presentation/UI/BuildMenu.cs`
- `src/DualFrontier.Presentation/UI/AlertPanel.cs`
- `src/DualFrontier.Presentation/UI/ManaBar.cs`
- `src/DualFrontier.Presentation/UI/PawnInspector.cs`
- `src/DualFrontier.Presentation/Nodes/ProjectileVisual.cs`
- `src/DualFrontier.Application/Bridge/Commands/ProjectileSpawnedCommand.cs`
- `src/DualFrontier.Application/Bridge/Commands/SpellCastCommand.cs`
- `src/DualFrontier.Application/Bridge/Commands/UIUpdateCommand.cs`

Confirm zero references via `grep -r` before deletion. If any reference exists in a `.tscn` or `.cs` file, STOP and escalate.

## Tests

### `tests/DualFrontier.Application.Tests/Scenario/RandomPawnFactoryTests.cs`

If the test project does not exist yet (`DualFrontier.Application.Tests`), create it with:
- xUnit reference (match existing project versions)
- Project references: `DualFrontier.Application`, `DualFrontier.AI`, `DualFrontier.Components`, `DualFrontier.Core`, `DualFrontier.Events`, `DualFrontier.Contracts`
- Add to `.sln` via `dotnet sln add`

If it exists, add the new file under `Scenario/` subdirectory.

```csharp
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
using Xunit;

namespace DualFrontier.Application.Tests.Scenario;

public sealed class RandomPawnFactoryTests
{
    private const int MapW = 50;
    private const int MapH = 50;

    [Fact]
    public void Spawn_RequestedCount_ReturnsExactCount()
    {
        var (factory, world, services) = BuildFixture(seed: 1);
        var ids = factory.Spawn(world, services, 10);
        Assert.Equal(10, ids.Count);
    }

    [Fact]
    public void Spawn_DeterministicBySeed_ProducesIdenticalOutput()
    {
        var (factoryA, worldA, servicesA) = BuildFixture(seed: 42);
        var (factoryB, worldB, servicesB) = BuildFixture(seed: 42);

        var idsA = factoryA.Spawn(worldA, servicesA, 10);
        var idsB = factoryB.Spawn(worldB, servicesB, 10);

        // Compare the names for each pawn — these come straight from the
        // RNG draws against fixed pools, so identical seeds must produce
        // identical names in identical order.
        var namesA = idsA.Select(id => worldA.GetComponent<IdentityComponent>(id).Name).ToList();
        var namesB = idsB.Select(id => worldB.GetComponent<IdentityComponent>(id).Name).ToList();
        Assert.Equal(namesA, namesB);
    }

    [Fact]
    public void Spawn_NamesAreNonEmpty()
    {
        var (factory, world, services) = BuildFixture(seed: 7);
        var ids = factory.Spawn(world, services, 10);

        foreach (var id in ids)
        {
            var ident = world.GetComponent<IdentityComponent>(id);
            Assert.False(string.IsNullOrWhiteSpace(ident.Name));
            Assert.Contains(' ', ident.Name);  // forename surname
        }
    }

    [Fact]
    public void Spawn_PositionsAreUnique()
    {
        var (factory, world, services) = BuildFixture(seed: 7);
        var ids = factory.Spawn(world, services, 10);

        var positions = ids
            .Select(id => world.GetComponent<PositionComponent>(id).Position)
            .ToList();
        Assert.Equal(positions.Count, positions.Distinct().Count());
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
            var pos = world.GetComponent<PositionComponent>(id).Position;
            Assert.True(navGrid.IsPassable(pos.X, pos.Y));
        }
    }

    [Fact]
    public void Spawn_EveryPawnHasAllSkillKindsPopulated()
    {
        var (factory, world, services) = BuildFixture(seed: 7);
        var ids = factory.Spawn(world, services, 10);

        var allKinds = System.Enum.GetValues<SkillKind>();

        foreach (var id in ids)
        {
            var skills = world.GetComponent<SkillsComponent>(id);
            Assert.NotNull(skills.Levels);
            foreach (var kind in allKinds)
            {
                Assert.True(skills.Levels.ContainsKey(kind), $"Skill {kind} missing");
                Assert.InRange(skills.Levels[kind], 0, SkillsComponent.MaxLevel);
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

        Assert.Equal(10, observed.Count);
    }

    [Fact]
    public void Spawn_RequestingMoreThanAvailableTiles_Throws()
    {
        // Block almost every tile.
        var navGrid = new NavGrid(MapW, MapH);
        for (int y = 0; y < MapH; y++)
            for (int x = 0; x < MapW; x++)
                if (!(x < 3 && y < 3))
                    navGrid.SetTile(x, y, passable: false);

        var factory = new RandomPawnFactory(seed: 7, navGrid, MapW, MapH);
        var world = new World();
        var services = new GameServices();

        Assert.Throws<System.InvalidOperationException>(
            () => factory.Spawn(world, services, 100));
    }

    private static NavGrid BuildNavGrid()
    {
        var nav = new NavGrid(MapW, MapH);
        var rng = new System.Random(99);
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
```

### `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — additional facts

Add to existing test class (keep all existing tests intact). Three new tests:

```csharp
[Fact]
public void CreateLoop_Spawns10PawnsByDefault()
{
    var bridge = new PresentationBridge();
    var observedSpawns = new List<PawnSpawnedCommand>();

    GameContext context = GameBootstrap.CreateLoop(bridge);
    bridge.DrainCommands(c =>
    {
        if (c is PawnSpawnedCommand sp) observedSpawns.Add(sp);
    });

    Assert.Equal(10, observedSpawns.Count);
}

[Fact]
[Trait("Category", "Integration")]
public void CreateLoop_RunningLoop_PawnStateCommandCarriesRealName()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);

    try
    {
        context.Loop.Start();
        Thread.Sleep(500);  // allow at least one SLOW tick (TickRates.SLOW)
    }
    finally
    {
        context.Loop.Stop();
    }

    var stateCommands = new List<PawnStateCommand>();
    bridge.DrainCommands(c =>
    {
        if (c is PawnStateCommand ps) stateCommands.Add(ps);
    });

    Assert.NotEmpty(stateCommands);
    foreach (var ps in stateCommands)
    {
        Assert.False(string.IsNullOrWhiteSpace(ps.Name),
            "PawnStateCommand carries empty Name — IdentityComponent not wired correctly");
        Assert.Contains(' ', ps.Name);  // forename + surname format
    }
}

[Fact]
[Trait("Category", "Integration")]
public void CreateLoop_RunningLoop_PawnStateCommandCarriesTopSkills()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);

    try
    {
        context.Loop.Start();
        Thread.Sleep(500);
    }
    finally
    {
        context.Loop.Stop();
    }

    var stateCommands = new List<PawnStateCommand>();
    bridge.DrainCommands(c =>
    {
        if (c is PawnStateCommand ps) stateCommands.Add(ps);
    });

    Assert.NotEmpty(stateCommands);
    foreach (var ps in stateCommands)
    {
        Assert.NotNull(ps.TopSkills);
        Assert.Equal(3, ps.TopSkills.Count);
        // Verify top-3 ordering: each entry's Level >= the next entry's Level
        for (int i = 0; i < ps.TopSkills.Count - 1; i++)
            Assert.True(ps.TopSkills[i].Level >= ps.TopSkills[i + 1].Level);
    }
}
```

### Existing tests — survey and update

Run `grep -rn "InitialPawnPositions\|SpawnInitialPawns" tests/` and `grep -rn "Sister Maria\|Magus Ferro\|Brother Cassian" tests/`. Any test that references these strings or assumes 3 pawns must be updated to the new behaviour:
- If it asserts pawn count == 3: change to `>= 1` or `== 10` depending on intent
- If it asserts specific names: replace with assertion that name is non-empty and contains a space

If existing tests don't reference these, no test changes outside the new file.

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors.
2. `dotnet test` count: **417 + 11 new = 428/428** (8 RandomPawnFactory tests + 3 GameBootstrapIntegration tests). If existing tests had to be updated due to pawn-count or name assumptions, the total may differ — report exact count.
3. `IdentityComponent` exists at `src/DualFrontier.Components/Pawn/IdentityComponent.cs` with single `Name` field.
4. `RandomPawnFactory` exists at `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs` with documented constructor and `Spawn` method.
5. `GameBootstrap.CreateLoop` spawns 10 pawns via `RandomPawnFactory` (seed 42, no `SpawnInitialPawns` method, no `InitialPawnPositions` array).
6. `PawnStateChangedEvent` and `PawnStateCommand` both carry `TopSkills` field.
7. `PawnStateReporterSystem` reads `IdentityComponent.Name` and `SkillsComponent.Levels`. No hardcoded `Names[]` array, no `ResolveName`, no `MakeRole`, no `DemoSkills`.
8. `PawnDetail` displays SKILLS section using real `TopSkills` data; no role label; no `MakeRole` or `DemoSkills` helpers.
9. 8 deleted files (5 stub UI + 3 undispatched commands) confirmed via `grep -rn` returning zero references across `src/`, `tests/`, `*.tscn`.
10. M-phase boundary preserved: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty.
11. All M7.x regression suites still pass: `M71PauseResumeTests`, `M72UnloadChainTests`, `M73Step7Tests`, `M73Phase2DebtTests`, `M74BuildPipelineTests`, `ManifestRewriterTests`, `ModMenuControllerTests`, `DefaultModDiscovererTests`, `PipelineGetActiveModsTests`, `GameBootstrapIntegrationTests` (existing 7 tests), `TickSchedulerThreadSafetyTests`. All M0–M6 + Persistence + Systems + Core suites still pass.
12. `dotnet sln list` count: unchanged if `DualFrontier.Application.Tests` already exists; +1 if it had to be created (report which case).
13. **Manual F5 verification deferred to user.** Agent cannot launch Godot from terminal. Commit 1 message documents the visual checklist:
    - 10 pawn sprites visible (vs 3 previously)
    - Each pawn row in ColonyPanel shows real name like "Aelin Brand", "Cara Drake" — not "Sister Maria" / "Magus Ferro"
    - Selecting a pawn shows PawnDetail with avatar + name only in header (no role label like "sergeant")
    - SKILLS section in PawnDetail shows 3 real skill names (e.g. "Magic 14", "Hauling 11", "Combat 7"), each pawn has different top skills
    - All 10 pawns have distinct names and skill profiles (deterministic by seed 42, but distinct from each other)

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `feat(scenario): real pawn data — IdentityComponent + RandomPawnFactory + 10 colonists + UI wiring`

This commit is large by design but coherent in purpose: "make pawn data real, end-to-end". Includes:
- `IdentityComponent.cs` (new, in Components)
- `RandomPawnFactory.cs` (new, in Application/Scenario)
- `PawnStateChangedEvent.cs` extended with TopSkills
- `PawnStateCommand.cs` extended with TopSkills
- `PawnStateReporterSystem.cs` rewritten to read IdentityComponent + SkillsComponent
- `GameBootstrap.cs` updated: factory call, MapWidth/Height constants, subscription forwards TopSkills, SpawnInitialPawns method removed, InitialPawnPositions array removed
- `GameHUD.cs` UpdatePawn passes TopSkills through
- `PawnDetail.cs` removes role + MakeRole + DemoSkills, RenderSkills now takes IReadOnlyList of real (Kind, Level), PawnState record extended, UpdatePawn signature extended
- 8 dead files deleted: BuildMenu, AlertPanel, ManaBar, PawnInspector, ProjectileVisual, ProjectileSpawnedCommand, SpellCastCommand, UIUpdateCommand
- Project reference: `DualFrontier.Events` → `DualFrontier.Components` added if not present
- Verify existing M7.x + housekeeping suites still pass via `dotnet test --filter "FullyQualifiedName~M71|FullyQualifiedName~M72|FullyQualifiedName~M73|FullyQualifiedName~M74|FullyQualifiedName~ManifestRewriter|FullyQualifiedName~ModMenuController|FullyQualifiedName~DefaultModDiscoverer|FullyQualifiedName~PipelineGetActiveMods|FullyQualifiedName~GameBootstrapIntegration|FullyQualifiedName~TickScheduler"` — all green

**Pre-commit-1 grep checks** (8 deleted files):
For each filename without extension: `BuildMenu`, `AlertPanel`, `ManaBar`, `PawnInspector`, `ProjectileVisual`, `ProjectileSpawnedCommand`, `SpellCastCommand`, `UIUpdateCommand` — run `grep -rn "<name>" src/ tests/ src/DualFrontier.Presentation/Scenes/`. Each must return zero matches outside the file being deleted itself. Any external reference → STOP, do not delete that file, escalate.

**2.** `test(scenario,bootstrap): RandomPawnFactory unit tests + bootstrap integration coverage`

- `tests/DualFrontier.Application.Tests/Scenario/RandomPawnFactoryTests.cs` (new) with 8 tests
- `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` extended with 3 new facts
- If `DualFrontier.Application.Tests` project doesn't exist, create it with proper csproj + sln entry
- Run full suite, confirm 428/428 (or report actual count if existing tests had to be updated)

**3.** `docs(roadmap): real pawn data closure + categorized Phase 5 backlog`

- `ROADMAP.md`:
    - Header status line: `*Updated: YYYY-MM-DD (housekeeping — real pawn data via IdentityComponent + RandomPawnFactory + 10 colonists; M7.5.B.2 + M7-closure pending; Phase 5 backlog categorized).*`
    - Engine snapshot: 417 → 428 (or actual).
    - Backlog section restructured into the categorized subsections specified in the prior `HOUSEKEEPING_UI_HONESTY_PASS.md` brief, now updated to reflect:
        - **REMOVED from Backlog** (no longer pending): "BuildMenu.cs stub", "fake pawn names", "fake role label", "fake skill bars", "AlertPanel.cs stub", "ManaBar.cs stub", "PawnInspector.cs stub", "ProjectileVisual.cs stub", "ProjectileSpawnedCommand undispatched", "SpellCastCommand undispatched", "UIUpdateCommand undispatched", "IdentityComponent for real names", "SkillsComponent display wiring"
        - **STILL PENDING in Backlog** (Phase 5):
            - NeedsSystem decay direction
            - Job loop (food/water/bed entities, Eat/Drink/Sleep job assignment)
            - MoodBreakEvent handlers
            - HealthComponent display
            - FactionComponent display
            - RaceComponent display
            - SocialComponent display (relationships)
            - ManaComponent / EtherComponent display (magic system)
            - Combat command dispatch (re-create ProjectileSpawnedCommand etc. when systems land)
            - Stub UI re-creation (re-create BuildMenu, AlertPanel, etc. when needed for Phase 5)
            - MovementSystem job-aware wandering / JobLabel honesty
            - Role / class concept (if introduced, add component + UI)
            - Full skills display (currently top-3; expand to all 13 in inspector mode)
        - **Resolved subsection** retained:
            - TickScheduler.ShouldRun race (commit `e0b0ecf`)
            - UI lies removed via real pawn data (this commit's SHA)

**Special verification preamble:**

After commit 1: `dotnet build && dotnet test` — must pass. M7.x regression filter confirmed. F5 verification deferred to user.

After commit 2: `dotnet test --filter "FullyQualifiedName~RandomPawnFactory|FullyQualifiedName~CreateLoop_Spawns10|FullyQualifiedName~CreateLoop_RunningLoop_PawnStateCommandCarries"` — 11 new tests green. Full suite 428/428.

After commit 3: ROADMAP renders cleanly; categorized subsections visible; resolved entries marked.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice. Per spec preamble "stop, escalate, lock — never guess".

**Hypothesis-falsification clause:**

This is housekeeping, not an M-cycle phase. The M-cycle datapoint sequence (10 consecutive zeros post-M4) is unaffected. No `MOD_OS_ARCHITECTURE` ratification candidate is plausible — this brief is gameplay-data-flow work, below the spec layer.

Plausible non-spec frictions worth flagging if encountered:

(a) **Cyclic project reference**: `DualFrontier.Events` → `DualFrontier.Components` may already be reverse-direction. If adding the reference creates a cycle, abort that approach and use a duplicate `SkillKind` enum locally in Events namespace (or a new shared enum project — but only if cycle is real).

(b) **`SystemBase.TryGetComponent<T>`** may not exist as a method. Adapt to whatever existing API is (HasComponent + GetComponent, or guard inside GetComponent). Do NOT modify SystemBase.

(c) **Existing tests asserting 3 pawns or specific names** — if found via grep, update them within commit 1 to match new behaviour. Do NOT skip them.

(d) **`Enum.GetValues<T>()` generic form** — requires .NET 5+. Check target framework. If unavailable, use `(SkillKind[])Enum.GetValues(typeof(SkillKind))`.

(e) **`NavGrid.IsPassable(int, int)` API** — verify signature. If named differently (`GetTile(x,y).IsPassable` or similar), adapt the factory.

(f) **`SaveSystem` referencing pawn count or component layout** — verified in required reading. If referenced and would break, STOP and escalate.

(g) **`DualFrontier.Application.Tests` project** — verify existence. If absent, creating it requires csproj + sln entry; agent must do this in commit 2.

(h) **TopSkills `IReadOnlyList<(SkillKind, int)>` allocation per SLOW tick per pawn** — at 10 pawns and SLOW tick rate, this is ~10 allocations / SLOW. Negligible. Flag only if benchmarks show otherwise.

(i) **Existing PawnRow uses ColonyPanel-internal Palette and PawnRow uses MakeInitials** — those stay. The `MakeInitials` method in PawnDetail can move from PawnDetail into a shared helper if needed, but probably stays inline duplicate. No deduplication required.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (417 + 11 = 428 expected, or actual).
- Per-test confirmation:
    - 8 `RandomPawnFactoryTests` by name (`Spawn_RequestedCount_ReturnsExactCount`, `Spawn_DeterministicBySeed_ProducesIdenticalOutput`, `Spawn_NamesAreNonEmpty`, `Spawn_PositionsAreUnique`, `Spawn_PositionsArePassable`, `Spawn_EveryPawnHasAllSkillKindsPopulated`, `Spawn_PublishesPawnSpawnedEventPerPawn`, `Spawn_RequestingMoreThanAvailableTiles_Throws`)
    - 3 `GameBootstrapIntegrationTests` new facts (`CreateLoop_Spawns10PawnsByDefault`, `CreateLoop_RunningLoop_PawnStateCommandCarriesRealName`, `CreateLoop_RunningLoop_PawnStateCommandCarriesTopSkills`)
- Regression confirmation: M7.1–M7.5.B.1 + housekeeping (TickScheduler race, UI honesty pass) + remaining suites all green.
- Working tree state: clean.
- M-phase boundary preserved: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.
- 8 deleted files: list each + grep zero-reference confirmation.
- Solution file: `dotnet sln list` count (note +1 if created `DualFrontier.Application.Tests`).
- ROADMAP backlog: confirm categorized subsections present, resolved entries listed.
- Manual F5 verification: deferred to user with the specific visual checklist reproduced above.
- **Project reference modifications**: list any added (e.g. `DualFrontier.Events` → `DualFrontier.Components`).
- **API adaptations**: any deviation from brief code skeletons due to actual API signatures (NavGrid, SystemBase, Enum.GetValues, etc.).
- **Existing tests updated** due to pawn-count or name assumptions — if any.
- Any unexpected findings.
