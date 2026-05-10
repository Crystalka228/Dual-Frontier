# K8.2 — Component class → struct conversion + ModAccessible annotation completeness pass

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-09 (post-K-Lessons closure `9df2709..071ae11`, post-K8.2 pre-audit findings)
**Status**: DEPRECATED 2026-05-09 — superseded by K8_2_COMPONENT_CONVERSION_BRIEF_V2.md after Phase 2.7 §1 stop (K8.1 wrapper surface mismatch). v1 is preserved as audit trail of the design that hit the stop; the surface mismatch becomes Phase 2.A of v2.
**Reference docs**: `KERNEL_ARCHITECTURE.md` v1.3 LOCKED (target for Part 2 master plan amendment + K-L3 implication update + version bump to v1.4), `MOD_OS_ARCHITECTURE.md` v1.6 LOCKED (referenced for `[ModAccessible]` D-1 LOCKED semantics, no edits), `METHODOLOGY.md` v1.5 (cross-references for K-Lessons), `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.0 LOCKED (parent plan; brief implements §1.1 K8.2 scope reformulation)
**Predecessor**: K-Lessons (`9df2709..071ae11`) — 4 pipeline-execution lessons formalized in METHODOLOGY v1.5 + KERNEL v1.3
**Target**: fresh feature branch `feat/k82-component-conversion` from `main` after K-Lessons closure
**Estimated time**: 3-5 hours auto-mode (3-5 days hobby pace; matches K8.1.1 / K-Lessons sizing for focused single-milestone work)
**Estimated LOC delta**: ~+400 / -600 net (6 class → struct conversions, ~13 annotations added, consumer reconciliation patches; less deletion than originally estimated due to keeping 6 empty stubs and 19 already-struct components untouched)

---

## Goal

K8.2 closes K-L3 «без exception» state for `src/DualFrontier.Components/`. Per `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication: «After K8.2 closure, K-L3 holds without exception across vanilla and mod components alike.» This is the **kernel-side foundation completion** for components — production storage now consistently uses unmanaged structs, with K8.1 native-side reference primitives applied where component shape requires reference data.

K8.2 is **the K-side milestone**, not the M-side. M-series (M8.4 onward) handles the relocation of components and systems into vanilla mod assemblies; K8.2 makes them ready for that relocation by completing their K-shape and capability annotations.

K8.2 explicitly does **not** touch the 6 empty-stub components (TODO bodies, no fields). Per Crystalka's framing 2026-05-09: «там почти всё это заглушки кроме пары штук, данных там ноль только ивент события» — those stubs are honest test fixtures whose content lands in M-series content milestones (M9 Combat for Ammo/Shield/Weapon, M10.B Magic for School, M-series Pawn for Social, M-series World for Biome). K8.2 does not pre-empt those M-series decisions.

### Pre-audit findings (verified 2026-05-09 by directory_tree + per-file reads)

| Action category | Count | Components |
|---|---|---|
| Convert class → struct (real K-L3 work) | **6** | FactionComponent, StorageComponent, WorkbenchComponent, IdentityComponent, MovementComponent, SkillsComponent |
| Verify-only (already struct, real data) | **19** | All Items (Bed, Consumable, DecorativeAura, Reservation, WaterSource); Pawn{Job, Mind, Needs}; Magic{Ether, GolemBond, Mana}; World{EtherNode, Tile}; Building{PowerConsumer, PowerProducer}; Combat{Armor}; Shared{Health, Position, Race} |
| Empty-stub fixtures (TODO bodies, untouched in K8.2) | **6** | AmmoComponent, ShieldComponent, WeaponComponent, SchoolComponent, SocialComponent, BiomeComponent |
| **Total components in scope** | **31** | (6 convert + 19 verify + 6 stub-touch-only-for-annotation) |

| `[ModAccessible]` annotation state | Count | Notes |
|---|---|---|
| Already annotated correctly | ~6 | HealthComponent (`Read=true,Write=true`), AmmoComponent (`Read=true`), ArmorComponent (`Read=true`), ShieldComponent (`Read=true`), WeaponComponent (`Read=true`), Items{Bed, Consumable, DecorativeAura, Reservation, WaterSource} variations |
| Missing annotation (added in K8.2) | ~13 | Pawn{Job, Mind, Movement, Needs}; World{Biome, EtherNode, Tile}; Building{PowerConsumer, PowerProducer}; Magic{Ether, GolemBond, Mana, School}; verified at execution time |
| Added during conversion (the 6 converted) | 6 | FactionComponent, StorageComponent, WorkbenchComponent, IdentityComponent, MovementComponent, SkillsComponent — annotation applied as part of conversion edit |

The execution-time audit may surface small discrepancies with this pre-audit (some component might be already annotated and pre-audit missed it; or vice versa). Per `METHODOLOGY.md` Phase 0.4 inventory-as-hypothesis lesson (K-Lessons §2.2), the pre-audit is brief-authoring-time hypothesis, not authoritative; Cloud Code records divergences and proceeds.

---

## Phase 0 — Pre-flight verification

### 0.1 — Working tree clean

```
git status
```

**Expected**: `nothing to commit, working tree clean` on branch `main`.

**Halt condition (hard gate)**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K8.2-WIP"` and re-verify.

### 0.2 — Prerequisite milestone closed

```
git log --oneline -10
```

**Expected**: K-Lessons closure visible. Most recent K-Lessons commit per closure record: `da092d2`. K8.1.1 closure (`63777ef`) and K8.1 closure (`812df98`) precede.

**Halt condition (hard gate)**: K-Lessons not closed, or closure SHA does not match `da092d2`. K8.2 builds on the post-K-Lessons documentation state (METHODOLOGY v1.5, KERNEL_ARCHITECTURE v1.3) and uses K8.1 primitives (`InternedString`, `NativeMap<K,V>`, `NativeSet<T>`, `Composite`/`NativeComposite`) which closed at K8.1 + K8.1.1.

### 0.3 — Test baseline

```
dotnet test
```

**Expected**: 592 tests passing (post-K8.1.1 + K-Lessons baseline). Build and test must be clean before any conversion edit lands.

**Halt condition (hard gate)**: any test failing pre-K8.2. K8.2 is a substantive code-touching milestone; baseline must be clean.

### 0.4 — Component inventory hypothesis (per `METHODOLOGY.md` §K-Lessons / Phase 0.4 inventory-as-hypothesis lesson)

The pre-audit table above is the brief-authoring-time hypothesis. Cloud Code at execution time:

```
ls src/DualFrontier.Components/
ls src/DualFrontier.Components/{Building,Combat,Items,Magic,Pawn,Shared,World}/
```

Records actual filenames per slice. Compares to pre-audit table. **Divergences are recorded as deviations and proceed**, not halt — per Phase 0.4 inventory-as-hypothesis lesson formalized in K-Lessons.

If a component is **missing** from `src/` that the pre-audit lists, K8.2 skips it (already removed in some intermediate work). If a component is **present** in `src/` that the pre-audit doesn't list, Cloud Code reads it and classifies (convert / verify / stub). New components since pre-audit go through normal classification.

### 0.5 — K8.1 primitives availability

```
ls src/DualFrontier.Core.Interop/
```

**Expected**: `InternedString.cs`, `NativeMap.cs`, `NativeComposite.cs`, `NativeSet.cs` present at top-level (not under `Marshalling/` — per K8.1 closure layout decision). Also `NativeWorld.cs`, `WriteBatch.cs`, `SpanLease.cs` present (K-series core).

**Halt condition**: K8.1 primitives missing or significantly out of expected location. K8.2 cannot proceed without InternedString and NativeMap at minimum (used by 4 of 6 converted components).

### 0.6 — `[ModAccessible]` attribute available

```
grep -r "class ModAccessibleAttribute" src/DualFrontier.Contracts/
```

**Expected**: `ModAccessibleAttribute` found in `DualFrontier.Contracts/Attributes/` (per `MOD_OS_ARCHITECTURE.md` D-1 LOCKED). Used by Items components currently — verified at K8.2 brief authoring time.

**Halt condition**: attribute not found. K8.2 annotation pass cannot proceed without the attribute existing. Resolution: stop and surface — attribute scaffolding is M3-era work and should be in place.

### 0.7 — Brief itself committed

If this brief is on disk in the working tree but uncommitted at the start of execution, commit it first on `main`:

```
git add tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF.md
git commit -m "docs(briefs): author K8.2 brief (component class→struct + ModAccessible pass)"
```

This is the first atomic commit of the milestone, performed on `main` before the feature branch is created (per K8.1 / K8.1.1 / K-Lessons precedent).

---

## Phase 1 — Architectural design (LOCKED — read-only, no edits)

This phase is the architectural foundation for K8.2. All decisions here are LOCKED by Crystalka's commitment («сложная архитектура без костылей») and the `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.0 strategy lock (Option II struct-first sequential).

### 1.1 — K-L3 closure shape (LOCKED)

After K8.2 closure, every concrete `IComponent` implementation in `src/DualFrontier.Components/` is one of:

- **Unmanaged struct with real fields** (the 19 verify-only + 6 newly-converted = 25 components)
- **Unmanaged struct with TODO body** (the 6 empty-stub fixtures, untouched structurally)

There are **no class components** in `src/DualFrontier.Components/` after K8.2 closes. K-L3 «без exception» state achieved per `KERNEL_ARCHITECTURE.md` Part 0 implication.

The 6 empty stubs remain `struct X : IComponent { /* TODO: ... */ }` — they were already struct pre-K8.2; they had TODO bodies pre-K8.2; they continue to have TODO bodies post-K8.2 (M-series content milestones add the bodies). K8.2's only touch on them is `[ModAccessible]` annotation if missing, **not** removing or modifying the TODO bodies.

### 1.2 — Per-component conversion specifications (LOCKED)

The 6 conversion targets are specified in dependency order (Tier 1 simplest, Tier 4 most complex). Cloud Code converts in this order to maximize commit independence — each conversion is its own atomic commit per the K-Lessons «atomic commit as compilable unit» lesson.

#### 1.2.1 — Tier 1: InternedString-only conversions (3 components)

These convert `string` field(s) to `InternedString`. K8.1 primitive surface used: `InternedString`, `NativeWorld.InternStringInScope`, `InternedString.Resolve(world)`, `InternedString.IsEmpty`.

##### **IdentityComponent** (`src/DualFrontier.Components/Pawn/IdentityComponent.cs`)

**Pre-K8.2 shape**:
```csharp
public sealed class IdentityComponent : IComponent
{
    public string Name { get; set; } = string.Empty;
}
```

**Post-K8.2 shape**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct IdentityComponent : IComponent
{
    /// <summary>
    /// Display name. Empty interned-string sentinel by default. Set at spawn time
    /// by the scenario loader; not mutated thereafter except by future rename
    /// systems. Resolve via NativeWorld.ResolveInternedString or InternedString.Resolve(world).
    /// </summary>
    public InternedString Name;
}
```

**Migration notes**:
- `string` → `InternedString` (handle, not value)
- `string.Empty` default → `InternedString` default (zero value = empty sentinel per K8.1 closure)
- Property → field (struct convention; no behavior beyond data carry)
- Doc-comment updated to clarify resolution semantics
- `[ModAccessible(Read = true, Write = true)]` added — name is mod-readable for UI display, mod-writable for renaming systems
- `Write = true` is correct because future rename systems (potentially mod-authored) will mutate

**Consumer reconciliation expected**:
- `RandomPawnFactory` or scenario loaders that set `.Name = someString` → must call `world.InternStringInScope(someString)` to obtain the `InternedString` handle, then assign
- `PawnStateReporterSystem` reads `.Name` → must call `name.Resolve(world)` or `world.ResolveInternedString(name)` to get the string
- Tests creating `new IdentityComponent { Name = "Alice" }` → adapt to `new IdentityComponent { Name = world.InternStringInScope("Alice") }`
- UI display layer → resolves InternedString to string at display time; empty sentinel renders as empty string verbatim per existing «no fabricated fallback» rule

**Tests added** (per K-Lessons «mod-scope test isolation» lesson §2.3):
- Round-trip: intern in mod scope → assign to component → read field → resolve → equality verified
- Mod-scope reclaim: intern under `BeginModScope("M")` → assign → `EndModScope("M")` + `ClearModScope("M")` → resolve returns null sentinel
- Empty-sentinel default: `new IdentityComponent()` → `.Name.IsEmpty` is true → resolution returns empty string
- All test setup/assertion references taken **inside** mod scope per K-Lessons §2.3

##### **WorkbenchComponent** (`src/DualFrontier.Components/Building/WorkbenchComponent.cs`)

**Pre-K8.2 shape**:
```csharp
public sealed class WorkbenchComponent : IComponent
{
    public string? ActiveRecipeId;
    public float Progress;
    public int TicksRemaining;
    public int WorkerEntityIndex;
    public bool IsOccupied => WorkerEntityIndex > 0;
    public bool IsIdle => ActiveRecipeId is null;
}
```

**Post-K8.2 shape**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct WorkbenchComponent : IComponent
{
    /// <summary>Recipe currently being crafted. Empty interned-string sentinel = idle.</summary>
    public InternedString ActiveRecipeId;

    public float Progress;
    public int TicksRemaining;
    public int WorkerEntityIndex;

    public bool IsOccupied => WorkerEntityIndex > 0;
    public bool IsIdle => ActiveRecipeId.IsEmpty;
}
```

**Migration notes**:
- `string?` → `InternedString` (no nullable wrapper; empty sentinel = idle)
- `IsIdle` semantics shift: `ActiveRecipeId is null` → `ActiveRecipeId.IsEmpty`. Both express "no recipe active"; the latter aligns with K8.1 closure conventions where empty interned-string is a first-class sentinel value.
- `[ModAccessible(Read = true, Write = true)]` added — workbench state mod-writable for crafting systems

**Consumer reconciliation expected**:
- `CraftSystem` setting `.ActiveRecipeId = someString` → `.ActiveRecipeId = world.InternStringInScope(someString)`
- `CraftSystem` clearing `.ActiveRecipeId = null` → `.ActiveRecipeId = default(InternedString)` or `.ActiveRecipeId = InternedString.Empty` if such a sentinel constant exists
- Reads checking idle state → unchanged (`.IsIdle` still works, body just changed)

**Tests added**: round-trip + idle-state-via-empty-sentinel + mod-scope reclaim + `[ModAccessible]` capability check.

##### **FactionComponent** (`src/DualFrontier.Components/Shared/FactionComponent.cs`)

**Pre-K8.2 shape**:
```csharp
public sealed class FactionComponent : IComponent
{
    public const string PlayerFactionId = "colony";
    public const string NeutralFactionId = "neutral";
    public string FactionId { get; init; } = "";
    public bool IsHostile { get; init; }
    public bool IsPlayer { get; init; }
    public bool IsNeutral => !IsHostile && !IsPlayer;
}
```

**Post-K8.2 shape**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct FactionComponent : IComponent
{
    /// <summary>The unique string identifier used for the player's colony faction. Resolve to InternedString via world.</summary>
    public const string PlayerFactionIdString = "colony";

    /// <summary>The unique string identifier used for neutral factions. Resolve to InternedString via world.</summary>
    public const string NeutralFactionIdString = "neutral";

    public InternedString FactionId;
    public bool IsHostile;
    public bool IsPlayer;

    public bool IsNeutral => !IsHostile && !IsPlayer;
}
```

**Migration notes**:
- `string` → `InternedString` for FactionId
- The two const strings are **kept as `const string`** (not converted to InternedString — InternedString is a runtime handle, not a compile-time constant). Renamed to `PlayerFactionIdString` / `NeutralFactionIdString` to clarify they need interning at use site. Sites that read the constants must intern through the world before comparing to FactionId field.
- `init` → field (struct convention)
- `[ModAccessible(Read = true, Write = true)]` added

**Consumer reconciliation expected**:
- Code creating `new FactionComponent { FactionId = "colony", IsPlayer = true }` → `new FactionComponent { FactionId = world.InternStringInScope(FactionComponent.PlayerFactionIdString), IsPlayer = true }`
- Code comparing `faction.FactionId == "colony"` → `faction.FactionId.EqualsByContent(world.InternStringInScope(FactionComponent.PlayerFactionIdString), world, world)` or via dedicated helper
- The dual-world EqualsByContent signature from K8.1.1 closure applies — same world for both args is the common case
- Sentinel `PlayerFaction` / `NeutralFaction` interned-string handles can be cached in a static or service if frequently used (not in K8.2 scope; consumer reconciliation only)

**Tests added**: round-trip + sentinel-comparison-via-EqualsByContent + mod-scope reclaim.

#### 1.2.2 — Tier 2: NativeMap conversions (1 component)

##### **SkillsComponent** (`src/DualFrontier.Components/Pawn/SkillsComponent.cs`)

**Pre-K8.2 shape**:
```csharp
public sealed class SkillsComponent : IComponent
{
    public const int MaxLevel = 20;
    public const float XpPerLevel = 1000f;
    public Dictionary<SkillKind, int>? Levels = null!;
    public Dictionary<SkillKind, float>? Experience = null!;
    public bool IsInitialized => Levels?.Count > 0;
}
```

**Post-K8.2 shape**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct SkillsComponent : IComponent
{
    public const int MaxLevel = 20;
    public const float XpPerLevel = 1000f;

    /// <summary>
    /// Current skill levels per skill. NativeMap handle; empty after default
    /// construction. Populated on pawn creation by SkillSystem (or equivalent).
    /// </summary>
    public NativeMap<SkillKind, int> Levels;

    /// <summary>
    /// Accumulated XP toward next level per skill. NativeMap handle; empty after
    /// default construction. Populated on pawn creation by SkillSystem.
    /// </summary>
    public NativeMap<SkillKind, float> Experience;

    /// <summary>
    /// Whether the skill data has been populated. Implementation: queries Levels
    /// for non-empty state; default-constructed NativeMap reports empty.
    /// </summary>
    public bool IsInitialized => Levels.Count > 0;
}
```

**Migration notes**:
- `Dictionary<SkillKind, int>?` → `NativeMap<SkillKind, int>`
- `Dictionary<SkillKind, float>?` → `NativeMap<SkillKind, float>`
- `null!` initialization removed — NativeMap is a value-type handle; default-constructed reports empty (per K8.1 sparse-abstraction convention)
- `IsInitialized` body updated — `.Count > 0` semantics on NativeMap
- `[ModAccessible(Read = true, Write = true)]` added

**Consumer reconciliation expected**:
- `SkillSystem` populating `.Levels = new Dictionary<SkillKind, int>()` → `.Levels = world.GetKeyedMap<SkillKind, int>("dualfrontier.vanilla.pawn.skills.levels", ...)` or whatever the K8.1 idiom is for component-owned maps. **Open detail per K8.1 closure**: NativeMap ownership model — does each component instance own its NativeMap, or is it world-scoped with key indexing? Brief authors at execution time consult K8.1.1 closure for the canonical pattern.
- `Levels[SkillKind.Cooking] = 5` access → `Levels.Add(SkillKind.Cooking, 5)` or `Levels.TrySet` per K8.1 NativeMap surface
- Reads `Levels[SkillKind.Cooking]` → `Levels.TryGet(SkillKind.Cooking, out int level)` per K8.1 «sparse abstraction» error semantics convention from K-Lessons

**Tests added**: round-trip + IsInitialized state transitions + mod-scope reclaim of NativeMap ownership.

**Open detail noted in brief**: SkillsComponent uses TWO NativeMaps. K8.1 closure's NativeMap surface should clarify whether two co-owned NativeMaps with parallel keys (SkillKind enum) are an idiomatic pattern or whether a single NativeMap<SkillKind, (int Level, float Experience)> with tuple value is preferred. **Brief decision (LOCKED)**: keep two separate NativeMaps to preserve the existing component shape with minimum semantic change. Future optimization (single tuple-valued NativeMap) is a follow-up if profiling justifies.

#### 1.2.3 — Tier 3: NativeMap + NativeSet conversion (1 component)

##### **StorageComponent** (`src/DualFrontier.Components/Building/StorageComponent.cs`)

**Pre-K8.2 shape**:
```csharp
public sealed class StorageComponent : IComponent
{
    public int Capacity = 20;
    public Dictionary<string, int> Items = new();
    public bool AcceptAll = true;
    public HashSet<string> AllowedItems = new();
    public bool IsFull => Items.Count >= Capacity;
    public int TotalQuantity { get { int total = 0; foreach (var v in Items.Values) total += v; return total; } }
}
```

**Post-K8.2 shape**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct StorageComponent : IComponent
{
    /// <summary>Maximum number of distinct item stacks this storage accepts. Default 20 set on construction by InventorySystem (factory).</summary>
    public int Capacity;

    /// <summary>Item stacks currently stored. Key = item template id (interned), Value = quantity.</summary>
    public NativeMap<InternedString, int> Items;

    /// <summary>Whether this storage accepts any item type (true) or only allowed types.</summary>
    public bool AcceptAll;

    /// <summary>Allowed item type ids when AcceptAll = false.</summary>
    public NativeSet<InternedString> AllowedItems;

    /// <summary>True if storage is full (Items.Count >= Capacity).</summary>
    public bool IsFull => Items.Count >= Capacity;

    /// <summary>Total quantity of all items across all stacks.</summary>
    public int TotalQuantity
    {
        get
        {
            int total = 0;
            foreach (var (_, v) in Items)
                total += v;
            return total;
        }
    }
}
```

**Migration notes**:
- `Dictionary<string, int>` → `NativeMap<InternedString, int>` — both key (string→InternedString) and storage type change
- `HashSet<string>` → `NativeSet<InternedString>`
- Default initialization `= 20` and `= true` removed from field declarations (struct field initializers have caveats; use factory pattern)
- `Capacity = 20` and `AcceptAll = true` defaults move to whatever creates the component (InventorySystem or scenario factory). Brief flags this as consumer-side responsibility.
- `IsFull` and `TotalQuantity` bodies updated to NativeMap iteration syntax
- `foreach (var v in Items.Values)` → `foreach (var (_, v) in Items)` per K8.1 NativeMap iteration convention (key-value tuple pattern)
- `[ModAccessible(Read = true, Write = true)]` added

**Consumer reconciliation expected**:
- `new StorageComponent()` → must explicitly set Capacity = 20 and AcceptAll = true at creation site
- `Items["wood"] = 50` → `Items.Add(world.InternStringInScope("wood"), 50)` (or appropriate K8.1 idiom)
- `Items.ContainsKey("wood")` → `Items.ContainsKey(world.InternStringInScope("wood"))`
- `AllowedItems.Add("steel")` → `AllowedItems.Add(world.InternStringInScope("steel"))`
- Tests creating storage with pre-populated items must adapt to interned-string keys

**Tests added**: round-trip + IsFull threshold + TotalQuantity sum + AcceptAll-vs-AllowedItems whitelist semantics + mod-scope reclaim of both NativeMap and NativeSet.

#### 1.2.4 — Tier 4: Composite conversion (1 component)

##### **MovementComponent** (`src/DualFrontier.Components/Pawn/MovementComponent.cs`)

**Pre-K8.2 shape**:
```csharp
public sealed class MovementComponent : IComponent
{
    public GridVector? Target;
    public List<GridVector> Path = new();
    public int StepCooldown;
}
```

**Post-K8.2 shape**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct MovementComponent : IComponent
{
    /// <summary>
    /// Current destination tile. <see cref="HasTarget"/> = false when no target;
    /// Target field carries default GridVector (0,0) which is a valid map location,
    /// so the explicit HasTarget flag is the authoritative «no-target» signal.
    /// </summary>
    public GridVector Target;

    /// <summary>True when Target is meaningful; false when no destination set.</summary>
    public bool HasTarget;

    /// <summary>
    /// Remaining path steps to destination. Composite handle; empty after default
    /// construction (no path planned). Populated by MovementSystem on pathfinding.
    /// </summary>
    public Composite<GridVector> Path;

    /// <summary>Ticks to wait before taking next step.</summary>
    public int StepCooldown;
}
```

**Migration notes**:
- `GridVector?` → `GridVector + bool HasTarget` — the nullable wrapper splits to value + flag because struct components avoid nullable value types where possible (K-L3 invariant: blittable layout; nullable<T> for value T can complicate marshalling)
- `List<GridVector>` → `Composite<GridVector>` — K8.1 closure's Composite primitive, designed for ordered sequences with insertion-order preservation per K8.1 LOCKED decision #6
- `[ModAccessible(Read = true, Write = true)]` added

**Consumer reconciliation expected**:
- `new MovementComponent { Target = someTile }` → `new MovementComponent { Target = someTile, HasTarget = true }`
- `MovementComponent { Target = null }` → `MovementComponent { HasTarget = false }` (Target field default-zeroes)
- `if (m.Target.HasValue)` → `if (m.HasTarget)`
- `if (m.Target == null)` → `if (!m.HasTarget)`
- `m.Target = null` → `m.HasTarget = false; m.Target = default;`
- `m.Target = newTile` → `m.HasTarget = true; m.Target = newTile;`
- `m.Path.Add(step)` → `m.Path.Add(step)` (Composite surface mirrors List for Add per K8.1)
- `m.Path.Clear()` → `m.Path.Clear()` (similar)
- `foreach (var step in m.Path)` → `foreach (var step in m.Path)` (Composite iterable per K8.1)

**Tests added**: round-trip + Target+HasTarget invariant (clearing target zeroes Target field) + Path insertion-order preservation + mod-scope reclaim of Composite.

**Open detail noted in brief**: Composite primitive at K8.1 closure may have subtle differences from List<T> semantics (e.g., default capacity, append-only vs random-access). Brief author at execution time consults `src/DualFrontier.Core.Interop/NativeComposite.cs` (or `Composite.cs`) for surface details. If Composite cannot exactly preserve List<GridVector> semantics for MovementComponent.Path use, brief surfaces the gap as a stop condition and proposes refinement (e.g., NativeComposite if it exposes different surface).

### 1.3 — Empty stub disposition (LOCKED)

The 6 empty-stub components are **structurally untouched** in K8.2. Per Crystalka 2026-05-09: «там почти всё это заглушки кроме пары штук, данных там ноль только ивент события» — these are honest test fixtures.

K8.2 only touches them for **annotation completeness**:

| Component | Slice | Pre-K8.2 annotation | Post-K8.2 annotation |
|---|---|---|---|
| AmmoComponent | Combat | `[ModAccessible(Read = true)]` | unchanged (already annotated) |
| ShieldComponent | Combat | `[ModAccessible(Read = true)]` | unchanged |
| WeaponComponent | Combat | `[ModAccessible(Read = true)]` | unchanged |
| SchoolComponent | Magic | (none — verify at exec time) | `[ModAccessible(Read = true)]` added if missing |
| SocialComponent | Pawn | (none — verify at exec time) | `[ModAccessible(Read = true, Write = true)]` added (social systems will mutate when content lands) |
| BiomeComponent | World | (none — verify at exec time) | `[ModAccessible(Read = true)]` added if missing |

The `[ModAccessible]` annotation values (Read=true vs Write=true) chosen per Crystalka's principle that K-side prep should not pre-empt M-side decisions. **Default**: Read=true unless the future content is obviously mutable by mod systems. The annotation can be amended in M-series if M brief author needs different semantics; this K8.2 sets reasonable defaults.

K8.2 does **not**:
- Remove TODO comments from empty-stub bodies
- Add fields to empty-stub bodies
- Convert struct to anything else
- Delete the empty-stub files

These are M-series concerns or specific feature design moments.

### 1.4 — Verify-only pass (LOCKED)

The 19 already-struct components with real fields receive **no shape changes** in K8.2. Each gets:

1. **Annotation review**: if `[ModAccessible]` is missing or has wrong Read/Write flags for its expected mod usage, add or correct.
2. **Doc-comment review**: if doc-comments reference `class` keyword or Dictionary/List/HashSet types that no longer apply (none expected for these 19, but verify), correct.
3. **K-L3 compliance verification**: confirm struct, confirm no reference fields beyond K8.1-compatible (InternedString, NativeMap, NativeSet, Composite, EntityId, GridVector — all blittable).

**No code changes** for the 19 verify-only components beyond potential annotation additions.

The 19 components: BedComponent, ConsumableComponent, DecorativeAuraComponent, ReservationComponent, WaterSourceComponent, JobComponent, MindComponent, NeedsComponent, EtherComponent, GolemBondComponent, ManaComponent, EtherNodeComponent, TileComponent, PowerConsumerComponent, PowerProducerComponent, ArmorComponent, HealthComponent, PositionComponent, RaceComponent.

### 1.5 — `[ModAccessible]` annotation completeness pass (LOCKED)

Per `MOD_OS_ARCHITECTURE.md` D-1 LOCKED: «`read` and `write` capabilities apply only to a curated, opt-in subset of public components. A component is reachable from a mod only when annotated with `[ModAccessible(Read = true, Write = false)]`.»

K8.2 brings annotation coverage to completeness so M-series can declare capabilities without `MissingCapability` errors at vanilla mod load time.

**Annotation defaults applied per pre-audit hypothesis** (verified at execution time):

| Component | Annotation choice | Rationale |
|---|---|---|
| Pawn{Job, Mind, Movement, Needs} | `Read=true, Write=true` | Pawn state is mutable by multiple systems (job assignment, mood progression, movement, needs decay) |
| World{Biome, EtherNode, Tile} | `Read=true` only | World data is read-mostly; mutation paths are MapSystem (kernel-internal pre-K8.3) or future mod systems requesting Write explicitly via amendment |
| Building{PowerConsumer, PowerProducer} | `Read=true, Write=true` | Power state mutated by ElectricGridSystem and similar |
| Magic{Ether, GolemBond, Mana, School} | `Read=true, Write=true` | Magic state is mutable (mana regeneration, ether tier change, golem bond updates, school level changes) |
| Shared/Position | `Read=true, Write=true` | Position is universally mutated; clearly write-side capability needed |
| Shared/Race | `Read=true` only | Race is set at spawn and immutable thereafter (per existing scenario semantics) |

Cloud Code at execution time confirms the annotation table by reading current `[ModAccessible]` state on each component, applying the table, and recording any divergence as a deviation per Phase 0.4 lesson.

### 1.6 — Atomic commit shape (LOCKED)

K8.2 lands as approximately **9-12 atomic commits**, structured as:

1. `docs(briefs): author K8.2 brief (component class→struct + ModAccessible pass)` — Phase 0.7, on main before feature branch.
2. `feat(components): convert IdentityComponent class→struct with InternedString` — Phase 2.1.
3. `feat(components): convert WorkbenchComponent class→struct with InternedString` — Phase 2.2.
4. `feat(components): convert FactionComponent class→struct with InternedString` — Phase 2.3.
5. `feat(components): convert SkillsComponent class→struct with NativeMap` — Phase 2.4.
6. `feat(components): convert StorageComponent class→struct with NativeMap+NativeSet` — Phase 2.5.
7. `feat(components): convert MovementComponent class→struct with Composite` — Phase 2.6.
8. `feat(components): ModAccessible annotation completeness pass` — Phase 3 (single commit covering all annotation-only edits).
9. `docs(kernel): K-L3 implication closure; bump KERNEL_ARCHITECTURE to v1.4` — Phase 4.
10. `docs(progress): record K8.2 closure (6 conversions + annotation pass + verify-only)` — Phase 5.

**Per-commit invariant** (per K-Lessons «atomic commit as compilable unit» lesson): each commit leaves the codebase in a buildable + testable state. If a particular component conversion has consumer reconciliation that must land in the same commit (e.g., system that immediately reads the converted field), Cloud Code bundles the consumer patch into the conversion commit per the dependency-cycle pattern formalized in K-Lessons §2.1.

If consumer reconciliation creates wider cross-cutting changes that don't fit into per-component commits, an interstitial commit may be needed:
- `refactor(systems): adapt consumers to InternedString-based components` (between commits 4 and 5, for example)
- `refactor(tests): adapt fixtures to struct components` (between commits 7 and 8, for example)

Cloud Code judges per execution; document split decisions in closure.

### 1.7 — Scope limits (LOCKED)

K8.2 does **not**:

- Touch the 6 empty-stub components structurally (only annotation if missing).
- Touch `src/DualFrontier.Persistence/` codec layer architecturally. Codec consequence updates (file path references, etc.) included as part of converted-component edits if absolutely necessary.
- Touch `src/DualFrontier.Application/Save/SaveSystem.cs` (`NotImplementedException` preserved per Crystalka «foundation first»).
- Move any component or system from `src/` to `mods/Vanilla.*/` (M-series concern).
- Modify K8.1 native primitives or their managed bridges.
- Add new tests for verify-only components.
- Modify `IModApi` surface.
- Touch `mods/DualFrontier.Mod.Vanilla.*/` skeletons.

K8.2 **does**:

- Convert the 6 specified class components to unmanaged structs.
- Apply K8.1 primitives (InternedString, NativeMap, NativeSet, Composite) to the converted components per Section 1.2.
- Reconcile consumers (systems, factories, tests, UI sites, persistence sites) of the converted components.
- Apply `[ModAccessible]` annotations to the ~13 components missing them.
- Update doc-comments for converted components.
- Amend `KERNEL_ARCHITECTURE.md` Part 2 master plan K8.2 row to reflect actual scope (31 components touched, 6 converted) and bump to v1.4.
- Update K-L3 implication in KERNEL_ARCHITECTURE.md Part 0 to reflect achieved closure state.
- Update `MIGRATION_PROGRESS.md` with K8.2 closure record.

---

## Phase 2 — Component conversions

Six conversions in tier order. Each is its own atomic commit per Phase 1.6.

### 2.1 — IdentityComponent (Tier 1, simplest InternedString conversion)

**Per Section 1.2.1 IdentityComponent specification.**

**File touched**: `src/DualFrontier.Components/Pawn/IdentityComponent.cs`

**Steps**:

1. Read the current file via `view`.
2. Apply the post-K8.2 shape per Section 1.2.1.
3. Identify consumers via `grep -r "IdentityComponent" src/ tests/`. Likely sites:
 - `src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs` (reads .Name)
 - Tests creating `new IdentityComponent { Name = ... }`
 - Possibly `RandomPawnFactory` or scenario loaders
 - Possibly UI display layer
4. Reconcile each consumer per Section 1.2.1 «Consumer reconciliation expected».
5. Add new tests per Section 1.2.1 «Tests added».
6. `dotnet build` clean.
7. `dotnet test` passes (count grows by ~3-5 new tests, existing tests adapted).
8. Atomic commit.

**Atomic commit**: `feat(components): convert IdentityComponent class→struct with InternedString`

### 2.2 — WorkbenchComponent

**Per Section 1.2.1 WorkbenchComponent specification.**

**File touched**: `src/DualFrontier.Components/Building/WorkbenchComponent.cs`

**Consumers expected**: `src/DualFrontier.Systems/Inventory/CraftSystem.cs`, related tests.

**Atomic commit**: `feat(components): convert WorkbenchComponent class→struct with InternedString`

### 2.3 — FactionComponent

**Per Section 1.2.1 FactionComponent specification.**

**File touched**: `src/DualFrontier.Components/Shared/FactionComponent.cs`

**Consumers expected**: `src/DualFrontier.Systems/Faction/{RaidSystem,RelationSystem,TradeSystem}.cs`, possibly Combat systems that check faction hostility, tests, scenario loaders.

**Atomic commit**: `feat(components): convert FactionComponent class→struct with InternedString`

### 2.4 — SkillsComponent (Tier 2, NativeMap)

**Per Section 1.2.2 SkillsComponent specification.**

**File touched**: `src/DualFrontier.Components/Pawn/SkillsComponent.cs`

**Consumers expected**: `src/DualFrontier.Systems/Pawn/SkillSystem.cs` (heaviest consumer), JobSystem (may read skill levels for job suitability), tests.

**Open details from Section 1.2.2**: NativeMap ownership model (component-instance-owned vs world-scoped key-indexed). Brief author confirms idiom at execution time per K8.1 closure surface.

**Atomic commit**: `feat(components): convert SkillsComponent class→struct with NativeMap`

### 2.5 — StorageComponent (Tier 3, NativeMap + NativeSet)

**Per Section 1.2.3 StorageComponent specification.**

**File touched**: `src/DualFrontier.Components/Building/StorageComponent.cs`

**Consumers expected**: `src/DualFrontier.Systems/Inventory/InventorySystem.cs`, `HaulSystem.cs`, possibly `CraftSystem.cs` (reads workbench's adjacent storage), tests.

**Atomic commit**: `feat(components): convert StorageComponent class→struct with NativeMap+NativeSet`

### 2.6 — MovementComponent (Tier 4, Composite)

**Per Section 1.2.4 MovementComponent specification.**

**File touched**: `src/DualFrontier.Components/Pawn/MovementComponent.cs`

**Consumers expected**: `src/DualFrontier.Systems/Pawn/MovementSystem.cs` (primary), JobSystem (may read MovementComponent.HasTarget), pathfinding service, presentation layer for movement preview, tests.

**Open detail from Section 1.2.4**: Composite primitive surface vs List<T>. Brief author confirms surface at execution time. If Composite cannot preserve List semantics, surfaces stop condition.

**Atomic commit**: `feat(components): convert MovementComponent class→struct with Composite`

### 2.7 — Stop conditions specific to Phase 2

Halt and surface to Crystalka via chat session if any of:

1. **K8.1 primitive surface mismatch**: a component's reconciliation requires a K8.1 primitive method that doesn't exist on the actual `InternedString`/`NativeMap`/`NativeSet`/`Composite` API. Possibilities: NativeMap iteration syntax differs from `foreach (var (k, v) in ...)`; Composite doesn't expose Add/Clear/iterate as anticipated. **Resolution**: stop, document the actual K8.1 surface, propose adjustment to the conversion specification, await Crystalka confirmation. Do not invent K8.1 extensions.

2. **Consumer count surprise**: a converted component has consumers in unexpected projects (e.g., DualFrontier.Runtime touches IdentityComponent for some reason). **Resolution**: stop, name the consumer, surface as deviation, await guidance on whether consumer should adapt or whether conversion needs different shape.

3. **Persistence layer requires architectural changes**: codec layer (`src/DualFrontier.Persistence/Compression/`) requires more than mechanical updates — e.g., needs new K8.1-aware encoders. **Resolution**: stop. Per Section 1.7, persistence architectural redesign is out of K8.2 scope.

4. **Test substrate breaks**: existing test that uses converted-component-as-class is no longer adaptable — e.g., test depends on reference identity that struct semantics cannot provide. **Resolution**: stop, record test as deletion candidate (per METHODOLOGY §7.1 if test was placeholder lie), or escalate if test seems legitimate.

---

## Phase 3 — `[ModAccessible]` annotation completeness pass

**Per Section 1.5 annotation table.**

**Goal**: every component in `src/DualFrontier.Components/` carries an appropriate `[ModAccessible]` annotation, ready for M-series capability declaration in vanilla mod manifests.

**Steps**:

1. Read all 31 component files (Phase 0.4 already produced a list; reuse).
2. For each component, verify current annotation state.
3. Apply Section 1.5 annotation table:
 - Add missing annotations per the defaults
 - Confirm existing annotations match expected per the table; correct if mismatch
 - The 6 newly-converted components in Phase 2 already received annotations as part of their conversion commits — they're not re-touched here
4. Per K-Lessons «atomic commit as compilable unit», the annotation pass is a single atomic commit (annotations are independent additions; each one compiles standalone with the others; bundling reduces commit log noise).
5. `dotnet build` clean.
6. `dotnet test` passes (no test count change; annotations don't affect runtime behavior).

**Atomic commit**: `feat(components): ModAccessible annotation completeness pass`

---

## Phase 4 — KERNEL_ARCHITECTURE.md amendment

### 4.1 — Part 2 master plan K8.2 row update

Locate the K8.2 row in `KERNEL_ARCHITECTURE.md` Part 2 «Master plan» table. Currently reads:

```markdown
| K8.2 | 7 class components redesigned to structs | 1-2 weeks | -200/+300 |
```

Replace with:

```markdown
| K8.2 | 31 production components closed under K-L3: 6 class→struct conversions (using K8.1 primitives), 19 already-struct verify-only, 6 empty-stub fixtures untouched (M-series content concern); ModAccessible annotation completeness pass | 3-5 days | +400/-600 |
```

### 4.2 — Part 0 K-L3 implication update

Locate the K-L3 implication paragraph in Part 0 («Implication of K-L3»). Currently reads:

```markdown
**Implication of K-L3**: All managed components must convert от class к struct (K4 closed 24 of 31; K8.2 closes the remaining 7 via native-side reference primitives from K8.1). Mod components subject к same constraint. **K4's "Hybrid Path" softening retired in K8.2** — after K8.2 closure, K-L3 holds without exception across vanilla and mod components alike.
```

Replace with:

```markdown
**Implication of K-L3**: All managed components must be unmanaged structs. K8.2 (`MIGRATION_PROGRESS.md` K8.2 closure entry) closed K-L3 «без exception» state for `src/DualFrontier.Components/`: 6 class→struct conversions using K8.1 native-side reference primitives (InternedString, NativeMap, NativeSet, Composite); 19 already-struct components verified K-L3 compliant; 6 empty-stub fixtures (TODO bodies, no fields) preserved as honest test scaffolding pending M-series content milestones (M9 Combat for Ammo/Shield/Weapon, M10.B Magic for School, M-series Pawn for Social, M-series World for Biome). Mod components subject to same constraint. **K4's "Hybrid Path" softening retired** — after K8.2 closure, K-L3 holds without exception across vanilla and mod components alike. M-series migration of `src/DualFrontier.Components/` content into vanilla mod assemblies preserves K-L3; M-series content commits authoring new components do so as `unmanaged struct` from inception.
```

### 4.3 — Document version bump

Update version block at top of document.

Currently:
```markdown
**Version**: 1.3
**Date**: 2026-05-09
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3)
```

Replace with:
```markdown
**Version**: 1.4
**Date**: 2026-05-09
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3); K-L3 «без exception» state achieved at K8.2 closure (v1.4)
```

### 4.4 — Atomic commit

The Part 2 row update + K-L3 implication update + version bump are bundled into a single commit (per Phase 1.6 atomic commit shape; matches K-Lessons precedent of bundling lesson content + version bump into one commit).

**Atomic commit**: `docs(kernel): K-L3 implication closure; K8.2 row reformulation; bump KERNEL_ARCHITECTURE to v1.4`

---

## Phase 5 — Closure verification

### 5.1 — Update `MIGRATION_PROGRESS.md`

Locate the K8.2 row in the K-series Overview table. Currently:

```markdown
| K8.2 | 7 class components redesigned to structs | NOT STARTED | 1-2 weeks | — | — |
```

Replace with:

```markdown
| K8.2 | 31 components closed under K-L3 (6 conversions + 19 verify + 6 stubs preserved + annotation pass) | DONE | 3-5 days | `<phase-2-1-sha>..<phase-4-sha>` | 2026-MM-DD |
```

Add the K8.2 closure section detail block (matching K8.1 / K8.1.1 / K-Lessons closure block shape):

```markdown
### K8.2 — Component class→struct conversions + ModAccessible completeness

**Status**: DONE
**Closure**: `<phase-2-1-sha>..<phase-4-sha>` on `feat/k82-component-conversion` (fast-forward merged to main)
**Brief**: `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF.md`
**Test count**: 592 → <new baseline> (per Q-Tests=(iii); placeholder-lie tests removed if any, conversion tests added)

**Deliverables**:

- 6 class→struct conversions:
 - IdentityComponent (InternedString.Name)
 - WorkbenchComponent (InternedString.ActiveRecipeId — empty=idle sentinel)
 - FactionComponent (InternedString.FactionId)
 - SkillsComponent (2× NativeMap<SkillKind, T>)
 - StorageComponent (NativeMap<InternedString, int> + NativeSet<InternedString>)
 - MovementComponent (Composite<GridVector>.Path + bool HasTarget)
- 19 verify-only confirmations (already struct + real data, K-L3 compliant pre-K8.2)
- 6 empty-stub annotations (no shape change; M-series content concern)
- ~13 missing `[ModAccessible]` annotations applied per Section 1.5 defaults
- KERNEL_ARCHITECTURE.md v1.3 → v1.4: Part 2 K8.2 row reformulated; Part 0 K-L3 implication updated to «без exception» achieved state.

**Brief deviations**: record per execution. Expected zero structural deviations; possible per-component consumer-reconciliation surprises (e.g., unexpected consumer in DualFrontier.Runtime).

**Architectural decisions LOCKED in this milestone**: none new. K-L3 «без exception» state is the implication of existing K-L3 + K8.1 + K8.2 work; no new K-Lxx decision is added or modified. Empty-stub preservation is per-Crystalka decision 2026-05-09 (not a new K-decision; an application of existing METHODOLOGY §7.1 «data exists or it doesn't» reading: TODO-marked stubs are honest scaffolding, not placeholder lies).

**Cross-cutting impact**:

- K8.3 brief authoring (next K-milestone): systems consuming the 6 converted components now use struct semantics + K8.1 primitive reads/writes. K8.3 scope is system-side conversion to SpanLease/WriteBatch.
- M-series brief authoring: vanilla mod manifests can declare capabilities like `kernel.read:DualFrontier.Components.Pawn.IdentityComponent` and they'll resolve against the now-annotated production components.
```

Replace `<phase-2-1-sha>..<phase-4-sha>` and `2026-MM-DD` with actual closure values.

### 5.2 — Update live tracker section

Update the «Current state snapshot» section at the top of `MIGRATION_PROGRESS.md`:

- **Active phase**: K9 (or K8.3 if K9 already closed per Option c sequencing — verify at execution time)
- **Last completed milestone**: K8.2 (6 component conversions + ModAccessible completeness; 31 components closed under K-L3)
- **Next milestone (recommended)**: K8.3 if K9 already closed; otherwise K9 still pending. Verify at execution.
- **Tests passing**: <new baseline> (was 592 pre-K8.2)
- **Last updated**: 2026-05-MM (K8.2 closure)

### 5.3 — Mark brief EXECUTED

Edit the front matter of this brief (`tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF.md`):

```markdown
**Status**: EXECUTED (2026-MM-DD, branch `feat/k82-component-conversion`, closure `<phase-2-1-sha>..<phase-4-sha>`). See `docs/MIGRATION_PROGRESS.md` § "K8.2 — Component class→struct conversions + ModAccessible completeness" for closure record.
```

The brief EXECUTED edit is part of the same commit as the MIGRATION_PROGRESS.md closure entry.

**Atomic commit**: `docs(progress): record K8.2 closure (6 conversions + annotation pass + verify-only)`

### 5.4 — Final verification

```
git status
git log --oneline -15
dotnet build
dotnet test
```

**Expected**:
- Working tree clean post-merge
- Commit log shows ~9-12 K8.2 commits (1 brief on main + 6 conversion commits + possible interstitial reconciliation commits + 1 annotation commit + 1 KERNEL doc commit + 1 closure commit)
- `dotnet build` clean across all projects
- `dotnet test` passes; new test count documented in closure entry

**Sanity grep on touched components**:

```
grep -rnE "public sealed class.*Component.*IComponent" src/DualFrontier.Components/
```

**Expected**: zero matches. After K8.2, no `class` IComponent implementations remain in `src/DualFrontier.Components/`.

Halt condition: any match. Surfaces a missed conversion.

```
grep -rn "ModAccessible" src/DualFrontier.Components/ | wc -l
```

**Expected**: ≥ 31 lines (one annotation per component, possibly more for components with both `[ModAccessible(Read=true, Write=true)]` and additional content).

### 5.5 — Merge to main

Branch is N commits ahead of `origin/main` (and 28+ commits ahead per K-Lessons closure baseline). Fast-forward merge:

```
git checkout main
git merge --ff-only feat/k82-component-conversion
```

If non-fast-forward, halt and report. K8.2 expects to land cleanly on top of K-Lessons closure SHA `da092d2` with no intervening commits.

Do **not** push to origin. Per established auto-mode convention, `git push` is a Crystalka decision after closure report.

---

## Stop conditions

Halt and surface to Crystalka if:

1. **Phase 0 baseline failure**: K-Lessons closure SHA does not match `da092d2`, baseline test count is not 592, KERNEL_ARCHITECTURE.md is not at v1.3, METHODOLOGY.md is not at v1.5. K8.2 cannot proceed against an unknown predecessor state.

2. **K8.1 primitive surface mismatch** (per Phase 2.7 §1): a component's reconciliation hits a K8.1 surface limitation. Stop, document, await guidance.

3. **Consumer count surprise** (per Phase 2.7 §2): a converted component has consumers in unexpected locations.

4. **Persistence architectural redesign required** (per Phase 2.7 §3): codec layer needs more than mechanical updates. Out of K8.2 scope.

5. **Test substrate inadequate** (per Phase 2.7 §4): test depends on class semantics struct cannot replicate.

6. **Component inventory surprise**: pre-audit count of 31 components diverges substantially from disk reality at execution time. Phase 0.4 lesson allows recording divergence and proceeding for small differences (1-3 components added/removed since pre-audit); 5+ component divergence warrants stop and re-audit.

7. **Annotation surprise**: pre-audit estimate of ~13 missing annotations diverges substantially. Same threshold as inventory: small diff proceeds with deviation record; large diff warrants stop.

8. **Empty-stub purity violation**: an empty-stub component (Ammo/Shield/Weapon/School/Social/Biome) turns out to have non-TODO content discovered at execution-time read. Stop, document, await guidance — Crystalka 2026-05-09 confirmed «keep all 6 empty stubs» based on pre-audit reading; if reading was wrong, decision needs revisiting.

---

## Atomic commit log expected

| # | Phase | Message |
|---|---|---|
| 1 | 0.7 | `docs(briefs): author K8.2 brief (component class→struct + ModAccessible pass)` |
| 2 | 2.1 | `feat(components): convert IdentityComponent class→struct with InternedString` |
| 3 | 2.2 | `feat(components): convert WorkbenchComponent class→struct with InternedString` |
| 4 | 2.3 | `feat(components): convert FactionComponent class→struct with InternedString` |
| 5 | 2.4 | `feat(components): convert SkillsComponent class→struct with NativeMap` |
| 6 | 2.5 | `feat(components): convert StorageComponent class→struct with NativeMap+NativeSet` |
| 7 | 2.6 | `feat(components): convert MovementComponent class→struct with Composite` |
| 8 | 3 | `feat(components): ModAccessible annotation completeness pass` |
| 9 | 4 | `docs(kernel): K-L3 implication closure; K8.2 row reformulation; bump KERNEL_ARCHITECTURE to v1.4` |
| 10 | 5 | `docs(progress): record K8.2 closure (6 conversions + annotation pass + verify-only)` |

Total: **10 atomic commits expected**. Interstitial reconciliation commits may add 1-2 more if consumer reconciliation crosses cleanly per-component boundaries; document any insertion in closure.

---

## Cross-cutting design constraints

- **K-Lessons cross-references**: this brief inherits 4 lessons from K-Lessons (METHODOLOGY v1.5 + KERNEL v1.3):
 - Atomic commit as compilable unit (METHODOLOGY) — applied in Phase 1.6 commit shape.
 - Phase 0.4 inventory as hypothesis (METHODOLOGY) — applied in Phase 0.4 with explicit divergence-record-and-proceed.
 - Mod-scope test isolation (METHODOLOGY) — applied in Section 1.2 component conversion tests; all reclaim assertions take references inside the scope under test.
 - Error semantics convention (KERNEL Part 7) — K8.1 primitives used in this brief comply with the convention; consumer reconciliation of TryGet/etc patterns matches sparse-abstraction category.

- **Migration plan compliance**: K8.2 implements `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.0 §1.1 reformulated scope (31 components instead of 7). Plan is the authoritative parent; brief implements specific milestones.

- **No M-series scope creep**: K8.2 makes no changes to `mods/Vanilla.*/` skeletons, no manifest edits, no IModApi changes. M-series concerns are explicitly deferred to M-milestones per migration plan.

- **No save-system scope creep**: K8.2 makes no changes to `src/DualFrontier.Application/Save/SaveSystem.cs` or to `src/DualFrontier.Persistence/` architecturally. Per Crystalka «foundation first» 2026-05-09.

- **No game mechanics design**: K8.2 closes structural foundation; gameplay design (filling in TODO stubs with real mechanics) is post-foundation work.

- **Documentation atomicity**: Part 2 row update + K-L3 implication update + version bump bundled into one commit. Avoids tax commits.

---

## Execution closure

When all phases complete and all gates pass, K8.2 is **DONE**.

Closure report to Crystalka should include:

- Commit range merged to main
- New test baseline (down/up from 592 with documented rationale)
- KERNEL_ARCHITECTURE.md version bump confirmed (v1.3 → v1.4)
- K-L3 «без exception» state confirmed achieved
- Brief deviations recorded
- Architectural surprises (if any) that should inform K8.3 brief authoring or M-series planning

After closure report, Crystalka decides:

- Whether to push `main` to `origin`
- Whether next K-milestone is K8.3 (system migration) or K9 (RawTileField, if not yet closed)
- Whether any deviations require migration plan amendment

End of K8.2 component class→struct conversion brief.
