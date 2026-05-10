# K8.2 v2 — Component class→struct conversion + K8.1 wrapper value-type refactor + empty TODO stub deletions

**Brief version**: 2.0 (full rewrite of v1.0 after Cloud Code Phase 2.7 §1 stop)
**Authored**: 2026-05-09 (after K8.2 v1 halt; Crystalka Variant 3 lock)
**Status**: EXECUTED 2026-05-09 — branch `feat/k82-foundation-closure`, 3 main commits + 21 branch commits (`6ee1a85`..`7527d00`). See `docs/MIGRATION_PROGRESS.md` § "K8.2 v2 — K-L3 «без exception» closure (single milestone)" for closure record. K-L3 «без exception» state achieved.
**Supersedes**: `K8_2_COMPONENT_CONVERSION_BRIEF.md` v1.0 (deprecated; see Phase 0.8)
**Reference docs**:
- `KERNEL_ARCHITECTURE.md` v1.3 LOCKED (target for Part 2 + Part 0 + Part 7 amendment + bump to v1.4)
- `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (untracked, status `AUTHORED — pending`; Phase 0 locks it as v1.0)
- `MOD_OS_ARCHITECTURE.md` v1.6 LOCKED (referenced for `[ModAccessible]` D-1 LOCKED; no edits)
- `METHODOLOGY.md` v1.5 (K-Lessons cross-references)
- `K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` (precedent; line 293 LOCKED design — world-scoped storage, uint id keying)

**Predecessor**: K-Lessons (`9df2709..071ae11`) — closed
**Target branch**: fresh `feat/k82-foundation-closure` from `main`
**Estimated time**: 6-12 hours auto-mode (single session; if approaches limit, split per Phase 6.5 contingency)
**Estimated LOC delta**: ~+800 / -1500 net (wrapper refactor +400, 6 deletions -400 with consumer cleanup, 6 conversions +600/-200, annotations +50, docs +200)

---

## Goal

K8.2 v2 closes K-L3 «без exception» state for `src/DualFrontier.Components/` in a single milestone. Three concerns merged:

1. **K8.1 wrapper refactor** (was Cloud Code's recommended K8.1.2): `NativeMap<K,V>`, `NativeSet<T>`, `NativeComposite<T>` change from `sealed unsafe class` to `readonly struct`. `InternedString` gets `IComparable<InternedString>`. Per-instance id allocation strategy LOCKED. Without this, struct components cannot carry collection fields without violating K-L3 «unmanaged» constraint.

2. **Class→struct conversions** of 6 components with real data (Identity, Workbench, Faction, Skills, Storage, Movement). Per migration plan §1.1 reformulated K8.2 scope.

3. **Empty TODO stub deletions** of 6 components with no data (Ammo, Shield, Weapon, School, Social, Biome). Per Crystalka 2026-05-09 «снести гемпейные TODO ... очистив контракты под нормальный и чистый mod api». Per METHODOLOGY §7.1 «data exists or it doesn't» — these stubs are placeholder lies, not honest scaffolding.

**Why merged into single milestone**: Variant 3 chosen over Variant 1 (sequential K8.1.2 → K8.2 v2) and Variant 2 (partial K8.2 + K8.1.2 + K8.2.1). Per Crystalka 2026-05-09 «закончить миграцию K» + «без костылей». Sequential creates intermediate "K-L3 mostly closed" state for weeks; partial creates fragmented closure across 3 milestones. Combined achieves K-L3 «без exception» in one atomic milestone.

**Outcome**: after K8.2 v2 closes, `src/DualFrontier.Components/` contains 25 components, all unmanaged structs, all `[ModAccessible]`-annotated, ready for K8.3 system migration consumption.

---

## Pre-audit findings (verified 2026-05-09; subject to Phase 0.4 hypothesis-check at execution)

### Component inventory by action

| Action | Count | Components |
|---|---|---|
| Convert class→struct (K-L3 + K8.1 wrapper consumption) | **6** | IdentityComponent, WorkbenchComponent, FactionComponent (Tier 1: InternedString-only); SkillsComponent (Tier 2: 2× NativeMap); StorageComponent (Tier 3: NativeMap + NativeSet); MovementComponent (Tier 4: NativeComposite + bool HasTarget) |
| Delete (empty TODO stubs, placeholder lies) | **6** | AmmoComponent, ShieldComponent, WeaponComponent (Combat); SchoolComponent (Magic); SocialComponent (Pawn); BiomeComponent (World) |
| Verify-only (already struct + real data) | **19** | All Items (5); Pawn{Job, Mind, Needs} (3); Magic{Ether, GolemBond, Mana} (3); World{EtherNode, Tile} (2); Building{PowerConsumer, PowerProducer} (2); Combat{Armor} (1); Shared{Health, Position, Race} (3) |
| **Total touched** | **31** | (12 structurally edited: 6 convert + 6 delete; 19 annotation pass only) |
| **Survivors post-K8.2** | **25** | 31 - 6 deletions = 25 components, all struct |

### K8.1 wrapper refactor scope

| File | Change |
|---|---|
| `src/DualFrontier.Core.Interop/NativeMap.cs` | `sealed unsafe class` → `readonly struct`; carries `(IntPtr world_handle, uint map_id)` only; methods take `NativeWorld` at use-site OR resolve via `IntPtr` per existing K8.1 idiom |
| `src/DualFrontier.Core.Interop/NativeSet.cs` | same shape — `readonly struct (IntPtr, uint)` |
| `src/DualFrontier.Core.Interop/NativeComposite.cs` | same shape — `readonly struct (IntPtr, uint)` |
| `src/DualFrontier.Core.Interop/InternedString.cs` | implements `IComparable<InternedString>`; ordinal compare on `(Id, Generation)` tuple; required by `NativeMap<InternedString, V>` constraint |

### Per-instance id allocation strategy LOCKED (Phase 1.A.4)

**Pattern α**: global counter in `NativeWorld`. Each component holding a `NativeMap`/`NativeSet`/`NativeComposite` field receives a unique uint id from the world's counter at component construction time. Component carries `(world_handle, id)` as the value-type wrapper; world owns the actual collection storage per K8.1 LOCKED line 293 («world-scoped, uint-keyed»).

**API surface added** (managed bridge):
- `NativeWorld.AllocateMapId() : uint`
- `NativeWorld.AllocateSetId() : uint`
- `NativeWorld.AllocateCompositeId() : uint`

**Native side implementation** (Phase 2.A.4 below):
- `df_world_allocate_map_id(world) : uint32_t` — returns next monotonic uint
- Same for set and composite
- Counter starts at 1; 0 reserved as «invalid id» sentinel

**Lifecycle**: id is allocated at construction, never reused (no recycling in v1.0; if id space exhaustion becomes a concern, K-future amendment). Component destruction does **not** automatically release the underlying collection storage in native — that's a separate concern handled by mod-scope teardown (per K8.1 mod-scope reclaim semantics already in place).

### `[ModAccessible]` annotation pass

| Components needing annotation added | Count |
|---|---|
| Survivors of conversion (the 6 converted get annotation as part of conversion) | 6 |
| Verify-only that are missing annotation | ~13 |
| **Total annotation edits** | **~19** |

Per migration plan §1.5 annotation table — defaults: Read=true universal; Write=true for components that real systems mutate; Read-only for World terrain and Race (set at spawn, immutable thereafter).

---

## Phase 0 — Pre-flight verification

### 0.1 — Working tree clean

```
git status
```

**Expected**: `nothing to commit, working tree clean` on `main`.

**Note**: `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` and `K8_2_COMPONENT_CONVERSION_BRIEF.md` (v1, deprecated) and this v2 brief are expected to be **untracked**. They get committed in Phase 0.7-0.8.

**Halt condition (hard gate)**: any uncommitted modifications to **tracked** files. Resolution: stash via `git stash push -m "pre-K8.2v2-WIP"`.

### 0.2 — Prerequisite milestone closed

```
git log --oneline -5
```

**Expected**: HEAD = `da092d2` (K-Lessons closure). K8.1.1 (`63777ef`) and K8.1 (`812df98`) precede.

**Halt condition (hard gate)**: K-Lessons not closed at expected SHA.

### 0.3 — Test baseline

```
dotnet test
```

**Expected**: 592 tests passing.

**Halt condition (hard gate)**: any test failing pre-K8.2 v2.

### 0.4 — Component inventory hypothesis (per Phase 0.4 lesson, METHODOLOGY v1.5)

```
ls src/DualFrontier.Components/{Building,Combat,Items,Magic,Pawn,Shared,World}/
```

Compare against pre-audit table above. Record divergences as deviations; proceed unless 5+ component count mismatch (then halt for re-audit).

### 0.5 — K8.1 primitive current state confirmation

Read these files **fully** before Phase 2:
- `src/DualFrontier.Core.Interop/InternedString.cs`
- `src/DualFrontier.Core.Interop/NativeMap.cs`
- `src/DualFrontier.Core.Interop/NativeSet.cs`
- `src/DualFrontier.Core.Interop/NativeComposite.cs`
- `src/DualFrontier.Core.Interop/NativeWorld.cs`
- `native/DualFrontier.Core.Native/include/{keyed_map,set_primitive,composite}.h`
- `native/DualFrontier.Core.Native/src/capi.cpp` (id allocation primitive — does it exist?)
- `tests/DualFrontier.Core.Interop.Tests/InternedStringTests.cs` (test pattern reuse)
- `tests/DualFrontier.Core.Interop.Tests/VanillaComponentRoundTripTests.cs` (test fixture style)

**Findings recorded** for Phase 1 design refinement:
- Existing wrapper field layout (does class hold more than `IntPtr + uint`? if yes, what else?)
- Existing native side id allocation (does world already have counter?)
- Existing `world.InternString` exact signature (per Cloud Code Phase 2.7 §1: actual is `InternString` + scope state, not `InternStringInScope`)

### 0.6 — `[ModAccessible]` attribute confirmed

```
grep -r "class ModAccessibleAttribute" src/DualFrontier.Contracts/
```

**Expected**: present. Already verified by Cloud Code in K8.2 v1 Phase 0.

### 0.7 — Migration plan housekeeping

`docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` is untracked with status «AUTHORED — pending Crystalka acceptance and lock». Per Crystalka 2026-05-09 «Всё сохранил в главные документы как план» — accepted.

Edit the frontmatter:
- `**Status**: AUTHORED — pending Crystalka acceptance and lock` → `**Status**: AUTHORITATIVE LOCKED — architectural roadmap for K-series and M-series`
- `**Version**: 1.0 (initial authoring, ready for review)` → `**Version**: 1.0 LOCKED`
- Add line below `**Authored**:`: `**Locked**: 2026-05-09 (Crystalka acceptance during K8.2 v2 session)`

Commit on **main** before feature branch:

```
git add docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md
git commit -m "docs(plans): lock migration plan v1.0 (kernel→vanilla)"
```

Move plan to `docs/` if currently in working tree at non-`docs/` location. Verify via `ls docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` after move.

### 0.8 — K8.2 v1 brief deprecation

`tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF.md` (v1) is untracked. It describes the design that hit the Phase 2.7 §1 stop. v2 supersedes it.

Two options:
- **(α)** Commit v1 with EXECUTED-NEVER-DEPRECATED note, then commit v2: preserves audit trail of what was tried.
- **(β)** Delete v1, commit only v2: cleaner working tree.

**Lock (α)**: preserve v1 with frontmatter note added:

```markdown
**Status**: DEPRECATED 2026-05-09 — superseded by K8_2_COMPONENT_CONVERSION_BRIEF_V2.md after Phase 2.7 §1 stop (K8.1 wrapper surface mismatch). v1 is preserved as audit trail of the design that hit the stop; the surface mismatch becomes Phase 2.A of v2.
```

Then rename v1 file to `K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md` and commit:

```
git mv tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF.md tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md
git add tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md
git commit -m "docs(briefs): deprecate K8.2 v1 brief (superseded by v2 after Phase 2.7 §1 stop)"
```

### 0.9 — Commit v2 brief on main

```
git add tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V2.md
git commit -m "docs(briefs): author K8.2 v2 brief (wrapper refactor + 6 conversions + 6 deletions)"
```

### 0.10 — Create feature branch

```
git checkout -b feat/k82-foundation-closure
```

All subsequent commits land on this branch.

---

## Phase 1 — Architectural design (LOCKED — read-only, no edits)

This phase is the architectural foundation. All decisions LOCKED by Crystalka 2026-05-09.

### 1.A — K8.1 wrapper value-type refactor (LOCKED)

#### 1.A.1 — `NativeMap<K,V>` shape

**Pre-K8.2 v2**:
```csharp
public sealed unsafe class NativeMap<TKey, TValue> where TKey : unmanaged, IComparable<TKey> where TValue : unmanaged
{
    private readonly NativeWorld _world;
    private readonly IntPtr _handle;
    private readonly uint _id;
    // methods: TryGet, Add, Remove, Count, etc.
}
```

**Post-K8.2 v2**:
```csharp
public readonly struct NativeMap<TKey, TValue> where TKey : unmanaged, IComparable<TKey> where TValue : unmanaged
{
    private readonly IntPtr _world_handle;
    private readonly uint _id;

    internal NativeMap(IntPtr world_handle, uint id)
    {
        _world_handle = world_handle;
        _id = id;
    }

    public uint Id => _id;
    public bool IsValid => _id != 0;

    public bool TryGet(TKey key, out TValue value) { /* P/Invoke via _world_handle, _id, key */ }
    public void Add(TKey key, TValue value) { /* same */ }
    public bool Remove(TKey key) { /* same */ }
    public int Count { get { /* same */ } }
    // iteration methods unchanged in semantics
}
```

**Migration notes**:
- `sealed unsafe class` → `readonly struct`
- `NativeWorld _world` field → `IntPtr _world_handle` field (resolve world from handle if needed; or keep IntPtr-only and accept that some methods take explicit `NativeWorld world` parameter at use-site)
- All methods preserve their existing signature **except** any that returned wrapper-typed instances (those are now value types, returned by value)
- Constructor moves to `internal` — only `NativeWorld` constructs wrappers via `Allocate*Id` factory methods
- `IsValid` property added — `_id == 0` is the «invalid» sentinel

**Rationale**: blittable value type can be a field of an `unmanaged` struct without violating K-L3. Two machine words (`IntPtr` + `uint`) per wrapper instance — minimal overhead.

#### 1.A.2 — `NativeSet<T>` shape

Same transformation as `NativeMap`. `(IntPtr, uint)` value type.

#### 1.A.3 — `NativeComposite<T>` shape

Same transformation. **Note**: Cloud Code's K8.2 v1 stop report identified that `NativeComposite<T>` is currently keyed by `EntityId parent` (world-scoped collection, not per-component-instance container). Phase 1.A.3 design **must clarify**:

**Lock**: NativeComposite remains world-scoped at native side per K8.1 line 293 LOCKED. Managed wrapper changes shape to `readonly struct (IntPtr, uint)`. The wrapper's `id` IS the composite identity — multiple `EntityId parent` parents do not share an id. Each component-instance carrying a `NativeComposite` field gets its own unique id from `AllocateCompositeId()`.

If `NativeComposite` previously took `EntityId parent` as method argument: this remains valid, but the `parent` parameter is **per-method**, not per-construction. The struct wrapper itself is parameterized only on `T` value type.

If existing native side requires `EntityId parent` discrimination (per K8.1 implementation detail), Phase 1.A surfaces this and either:
- **(i)** Reuses existing native primitive but treats id allocation as parent-key generation
- **(ii)** Adds new native primitive `df_world_allocate_composite_id` that produces stable ids without parent coupling

Cloud Code at execution time chooses (i) or (ii) based on actual native code state; documents choice as deviation.

#### 1.A.4 — `InternedString` `IComparable<InternedString>` implementation

**Add**:
```csharp
public readonly struct InternedString : IEquatable<InternedString>, IComparable<InternedString>
{
    public readonly uint Id;
    public readonly uint Generation;

    // Existing members preserved.

    public int CompareTo(InternedString other)
    {
        // Ordinal compare on (Id, Generation) tuple
        int idCompare = Id.CompareTo(other.Id);
        if (idCompare != 0) return idCompare;
        return Generation.CompareTo(other.Generation);
    }
}
```

**Rationale**: `NativeMap<TKey, ...>` constraint requires `TKey : unmanaged, IComparable<TKey>`. Without `IComparable<InternedString>`, `NativeMap<InternedString, int>` (StorageComponent.Items) won't compile.

**Note on semantics**: this is **handle-comparison**, not **content-comparison**. Two `InternedString` handles with same content but different Generation will compare unequal — matching K8.1.1's `EqualsByContent` distinction. NativeMap/NativeSet membership is based on handle identity per K8.1 design; content-equality fallback would require world reference at compare time which `IComparable<T>` does not permit.

For maps where content-equality is desired (e.g., StorageComponent.Items keyed by item template id where two interns of same template should be the same key), the convention is: **canonicalize at insertion time** via single `world.InternString(content)` call. Caller responsibility, not NativeMap concern.

#### 1.A.5 — `NativeWorld` id allocation API

**Add**:
```csharp
public sealed class NativeWorld : IDisposable
{
    // Existing members preserved.

    public uint AllocateMapId() { /* P/Invoke df_world_allocate_map_id */ }
    public uint AllocateSetId() { /* P/Invoke df_world_allocate_set_id */ }
    public uint AllocateCompositeId() { /* P/Invoke df_world_allocate_composite_id */ }

    public NativeMap<TKey, TValue> CreateMap<TKey, TValue>() where TKey : unmanaged, IComparable<TKey> where TValue : unmanaged
    {
        uint id = AllocateMapId();
        return new NativeMap<TKey, TValue>(_handle, id);
    }

    public NativeSet<T> CreateSet<T>() where T : unmanaged, IComparable<T>
    {
        uint id = AllocateSetId();
        return new NativeSet<T>(_handle, id);
    }

    public NativeComposite<T> CreateComposite<T>() where T : unmanaged
    {
        uint id = AllocateCompositeId();
        return new NativeComposite<T>(_handle, id);
    }
}
```

**Native side**:
```cpp
extern "C" uint32_t df_world_allocate_map_id(df_world* world)
{
    return ++world->next_map_id;  // monotonic counter; starts at 1
}
// Same for set and composite.
```

If existing native side **already has** these counters (visible in `world.cpp` or `capi.cpp`), reuse. If not, add minimal field + 3 functions. C ABI extension follows K8.1 conventions (status sentinel: 0 = invalid id).

#### 1.A.6 — `InternedString.Empty` sentinel

```csharp
public readonly struct InternedString
{
    public static readonly InternedString Empty = default;  // Id = 0, Generation = 0

    public bool IsEmpty => Id == 0;
}
```

Already partially present per K8.1 closure. Phase 1.A.6 verifies and ensures explicit `Empty` constant accessible via `InternedString.Empty` (not just `default(InternedString)`).

### 1.B — Class→struct conversion specifications

Per migration plan §1.1 + post-wrapper-refactor wrapper consumption pattern.

#### 1.B.1 — IdentityComponent (Tier 1, InternedString-only)

**Pre-K8.2 v2**:
```csharp
public sealed class IdentityComponent : IComponent
{
    public string Name { get; set; } = string.Empty;
}
```

**Post-K8.2 v2**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct IdentityComponent : IComponent
{
    public InternedString Name;
}
```

**Consumer reconciliation**:
- `RandomPawnFactory` setting `.Name = $"…"` → `.Name = world.InternString($"…")`
- `PawnStateReporterSystem` reading `.Name` → `var nameStr = component.Name.Resolve(world) ?? string.Empty;`
- Tests creating `new IdentityComponent { Name = "Alice" }` → `new IdentityComponent { Name = world.InternString("Alice") }`

**Tests added** (per K-Lessons §2.3 mod-scope test isolation):
- `Roundtrip_NameInternedAndResolves`
- `EmptyDefault_NameIsEmpty` (`new IdentityComponent().Name.IsEmpty == true`)
- `ModScopeReclaim_StaleAfterClear` (all references inside `BeginModScope`/`EndModScope`)

**Atomic commit**: `feat(components): convert IdentityComponent class→struct with InternedString`

#### 1.B.2 — WorkbenchComponent (Tier 1, InternedString-only)

**Pre-K8.2 v2**:
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

**Post-K8.2 v2**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct WorkbenchComponent : IComponent
{
    public InternedString ActiveRecipeId;  // Empty = idle sentinel
    public float Progress;
    public int TicksRemaining;
    public int WorkerEntityIndex;

    public bool IsOccupied => WorkerEntityIndex > 0;
    public bool IsIdle => ActiveRecipeId.IsEmpty;
}
```

**Consumer reconciliation**: `CraftSystem` (currently TODO body per migration plan) — minimal reconciliation needed; if any setter site exists it changes from `= "recipe_id"` to `= world.InternString("recipe_id")`. Idle clear `= null` becomes `= InternedString.Empty` (or `= default`).

**Tests**: round-trip + idle-via-empty-sentinel + mod-scope reclaim.

**Atomic commit**: `feat(components): convert WorkbenchComponent class→struct with InternedString`

#### 1.B.3 — FactionComponent (Tier 1, InternedString-only)

**Pre-K8.2 v2**:
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

**Post-K8.2 v2**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct FactionComponent : IComponent
{
    public const string PlayerFactionIdString = "colony";
    public const string NeutralFactionIdString = "neutral";

    public InternedString FactionId;
    public bool IsHostile;
    public bool IsPlayer;

    public bool IsNeutral => !IsHostile && !IsPlayer;
}
```

**Migration notes**:
- Const strings renamed to `*IdString` to clarify they require interning at use site (string is compile-time constant; `InternedString` is runtime handle, not interchangeable).
- `init` properties → fields (struct convention; mutable but discipline preserves immutability after initial set).

**Consumer reconciliation**: minimal (per migration plan: TradeSystem/RelationSystem/RaidSystem are TODO bodies). Any site comparing `faction.FactionId == "colony"` becomes `faction.FactionId.EqualsByContent(world.InternString(FactionComponent.PlayerFactionIdString), world, world)` — leveraging K8.1.1 dual-world EqualsByContent.

**Tests**: round-trip + sentinel-comparison via EqualsByContent + mod-scope reclaim.

**Atomic commit**: `feat(components): convert FactionComponent class→struct with InternedString`

#### 1.B.4 — SkillsComponent (Tier 2, NativeMap×2)

**Pre-K8.2 v2**:
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

**Post-K8.2 v2** (using post-wrapper-refactor `NativeMap` value-type):
```csharp
[ModAccessible(Read = true, Write = true)]
public struct SkillsComponent : IComponent
{
    public const int MaxLevel = 20;
    public const float XpPerLevel = 1000f;

    public NativeMap<SkillKind, int> Levels;
    public NativeMap<SkillKind, float> Experience;

    public bool IsInitialized => Levels.IsValid && Levels.Count > 0;
}
```

**Migration notes**:
- `Dictionary<SkillKind, int>?` → `NativeMap<SkillKind, int>` (value type, blittable; K-L3 satisfied)
- `null!` initialization removed — wrapper carries `(IntPtr, uint)` directly; default-constructed has `_id == 0` (`IsValid == false`)
- `IsInitialized` body updated — checks both validity and count

**Consumer reconciliation**:
- Factory creating component: `new SkillsComponent { Levels = world.CreateMap<SkillKind, int>(), Experience = world.CreateMap<SkillKind, float>() }`
- `Levels[SkillKind.Cooking] = 5` → `Levels.Add(SkillKind.Cooking, 5)` (requires `SkillKind` to be `IComparable<SkillKind>` — verify SkillKind is enum, which is auto-IComparable)
- `Levels[SkillKind.Cooking]` read → `Levels.TryGet(SkillKind.Cooking, out int level) ? level : 0`
- Tests creating populated component must use `world.CreateMap<...>()` factory, not direct field assignment

**Open detail (resolve at execution)**: SkillKind enum verification — confirm it's plain enum (auto-IComparable). If wrapper requires explicit IComparable, no action needed for enum types.

**Tests**: round-trip + IsInitialized state transitions + mod-scope reclaim of NativeMap-owned data + IComparable<SkillKind> usage in NativeMap context.

**Atomic commit**: `feat(components): convert SkillsComponent class→struct with NativeMap×2`

#### 1.B.5 — StorageComponent (Tier 3, NativeMap + NativeSet)

**Pre-K8.2 v2**:
```csharp
public sealed class StorageComponent : IComponent
{
    public int Capacity = 20;
    public Dictionary<string, int> Items = new();
    public bool AcceptAll = true;
    public HashSet<string> AllowedItems = new();
    public bool IsFull => Items.Count >= Capacity;
    public int TotalQuantity { get { /* sum */ } }
}
```

**Post-K8.2 v2**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct StorageComponent : IComponent
{
    public int Capacity;
    public NativeMap<InternedString, int> Items;
    public bool AcceptAll;
    public NativeSet<InternedString> AllowedItems;

    public bool IsFull => Items.IsValid && Items.Count >= Capacity;
    public int TotalQuantity
    {
        get
        {
            if (!Items.IsValid) return 0;
            int total = 0;
            foreach (var (_, v) in Items) total += v;
            return total;
        }
    }
}
```

**Migration notes**:
- Default `= 20` and `= true` in field initializers removed (struct field initializer caveats — caller responsibility)
- `Dictionary<string, int>` → `NativeMap<InternedString, int>` (uses `IComparable<InternedString>` from Phase 1.A.4)
- `HashSet<string>` → `NativeSet<InternedString>`
- `IsFull` and `TotalQuantity` updated to validate wrapper

**Consumer reconciliation** (significant — InventorySystem/HaulSystem/CraftSystem touch this):
- Factory: `new StorageComponent { Capacity = 20, Items = world.CreateMap<InternedString, int>(), AcceptAll = true, AllowedItems = world.CreateSet<InternedString>() }`
- `storage.Items["wood"] = 50` → `storage.Items.Add(world.InternString("wood"), 50)` (or `Add` with TryAdd semantics)
- `storage.Items.ContainsKey("wood")` → `storage.Items.ContainsKey(world.InternString("wood"))` (per K8.1 `NativeMap.ContainsKey` if present, else compose from `TryGet`)
- `storage.AllowedItems.Add("steel")` → `storage.AllowedItems.Add(world.InternString("steel"))`

**Open detail (resolve at execution)**: NativeMap iteration syntax. If existing K8.1 surface exposes `foreach (var (k, v) in map)` per `IEnumerable<KeyValuePair<TKey, TValue>>` or similar, use that. If only enumerator-based, adapt. Document discovered surface as deviation.

**Tests**: round-trip + IsFull threshold + TotalQuantity + AcceptAll/AllowedItems whitelist + mod-scope reclaim of both wrappers.

**Atomic commit**: `feat(components): convert StorageComponent class→struct with NativeMap+NativeSet`

#### 1.B.6 — MovementComponent (Tier 4, NativeComposite + bool HasTarget)

**Pre-K8.2 v2**:
```csharp
public sealed class MovementComponent : IComponent
{
    public GridVector? Target;
    public List<GridVector> Path = new();
    public int StepCooldown;
}
```

**Post-K8.2 v2**:
```csharp
[ModAccessible(Read = true, Write = true)]
public struct MovementComponent : IComponent
{
    public GridVector Target;
    public bool HasTarget;
    public NativeComposite<GridVector> Path;
    public int StepCooldown;
}
```

**Migration notes**:
- `GridVector?` (nullable struct) → `GridVector + bool HasTarget` — nullable<T> for value T can complicate marshalling and isn't universally blittable
- `List<GridVector>` → `NativeComposite<GridVector>` (post-refactor value-type wrapper)
- `Path = new()` initializer removed (caller calls `world.CreateComposite<GridVector>()` at construction)

**Consumer reconciliation** (MovementSystem touches this heavily):
- Factory: `new MovementComponent { Target = default, HasTarget = false, Path = world.CreateComposite<GridVector>(), StepCooldown = 0 }`
- `m.Target = newTile` (where newTile is non-null) → `m.HasTarget = true; m.Target = newTile;`
- `m.Target = null` → `m.HasTarget = false; m.Target = default;`
- `if (m.Target.HasValue)` → `if (m.HasTarget)`
- `m.Path.Add(step)` → `m.Path.Add(step)` (assuming `NativeComposite` exposes Add per K8.1; verify at execution)
- `m.Path.Clear()` → `m.Path.Clear()` (verify presence)
- `foreach (var step in m.Path)` → `foreach (var step in m.Path)` (verify iteration support)

**Open detail (resolve at execution)**: NativeComposite surface for List<T>-equivalent operations (Add, Clear, foreach). If gaps exist (e.g., Composite is append-only without Clear), Phase 1.B.6 surfaces as deviation; resolution may require K-future amendment to K8.1 native side.

**Tests**: round-trip + Target+HasTarget invariant (clearing zeroes Target) + Path insertion-order preservation + mod-scope reclaim of NativeComposite.

**Atomic commit**: `feat(components): convert MovementComponent class→struct with NativeComposite+HasTarget`

### 1.C — Empty TODO stub deletions (LOCKED)

Per Crystalka 2026-05-09 «снести гемпейные TODO». 6 deletions:

| File | Slice | Body shape |
|---|---|---|
| `src/DualFrontier.Components/Combat/AmmoComponent.cs` | Combat | TODO comments only, zero fields |
| `src/DualFrontier.Components/Combat/ShieldComponent.cs` | Combat | TODO comments only, zero fields |
| `src/DualFrontier.Components/Combat/WeaponComponent.cs` | Combat | TODO comments only, zero fields |
| `src/DualFrontier.Components/Magic/SchoolComponent.cs` | Magic | TODO comments only, zero fields |
| `src/DualFrontier.Components/Pawn/SocialComponent.cs` | Pawn | TODO comments only, zero fields |
| `src/DualFrontier.Components/World/BiomeComponent.cs` | World | TODO comments only, zero fields |

**Per-deletion cleanup**:
- Delete `.cs` file
- Delete companion `.cs.uid` file if present
- Find and remove all references via `grep -r "<ComponentName>" src/ tests/`:
  - **Systems** that mention deleted component (likely TODO Update bodies referencing the type via `[SystemAccess]` reads/writes attributes) — remove the reference from attribute list, leave system in place if it references other components, delete system if its only purpose was the deleted component
  - **Tests** that use deleted component — delete test or rewrite if test scope is broader
  - **Factories / scenario loaders** — remove instantiation
  - **Vanilla mod skeletons** in `mods/Vanilla.*/mod.manifest.json` — verify no `kernel.read:DualFrontier.Components.X.<DeletedComponent>` capability is declared (if yes, remove from manifest; this is M8.1 skeleton scope, expected empty)

**Atomic commit per slice** (4 commits — Combat folds 3 deletions into one commit since they're same slice):
- `refactor(components): delete empty TODO stubs (Ammo/Shield/Weapon) — content deferred to M9 vanilla content`
- `refactor(components): delete empty TODO stub (School) — content deferred to M10.B vanilla magic content`
- `refactor(components): delete empty TODO stub (Social) — content deferred to M-series pawn social feature`
- `refactor(components): delete empty TODO stub (Biome) — content deferred to M-series world biome feature`

**METHODOLOGY §7.1 invocation**: empty TODO stubs are placeholder lies («data exists or it doesn't»). Real game-mechanics components, when designed in M9/M10/M-series, will be authored fresh as struct in vanilla mod assemblies, not "filled into" the deleted placeholders. The TODO comments documenting the deferred features serve their archival purpose in the deletion commit messages.

### 1.D — Verify-only pass

19 already-struct components: shape unchanged. Phase 4 annotation pass touches them but no code changes.

### 1.E — `[ModAccessible]` annotation pass

Per migration plan §1.5 table:

| Component | File | Annotation |
|---|---|---|
| PowerConsumerComponent | `src/DualFrontier.Components/Building/PowerConsumerComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| PowerProducerComponent | `src/DualFrontier.Components/Building/PowerProducerComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| EtherComponent | `src/DualFrontier.Components/Magic/EtherComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| GolemBondComponent | `src/DualFrontier.Components/Magic/GolemBondComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| ManaComponent | `src/DualFrontier.Components/Magic/ManaComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| JobComponent | `src/DualFrontier.Components/Pawn/JobComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| MindComponent | `src/DualFrontier.Components/Pawn/MindComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| NeedsComponent | `src/DualFrontier.Components/Pawn/NeedsComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| PositionComponent | `src/DualFrontier.Components/Shared/PositionComponent.cs` | `[ModAccessible(Read=true, Write=true)]` |
| RaceComponent | `src/DualFrontier.Components/Shared/RaceComponent.cs` | `[ModAccessible(Read=true)]` |
| EtherNodeComponent | `src/DualFrontier.Components/World/EtherNodeComponent.cs` | `[ModAccessible(Read=true)]` |
| TileComponent | `src/DualFrontier.Components/World/TileComponent.cs` | `[ModAccessible(Read=true)]` |

12 verify-only annotations. (The 6 converted components received their annotations as part of Phase 2.B conversion commits. The 6 deleted components don't need annotation — they're gone. Items components and pre-annotated Combat ArmorComponent + Shared/Health are already correctly annotated, no edits.)

### 1.F — Atomic commit shape (LOCKED)

**Approximately 14-18 atomic commits** in this order:

1. (main) `docs(plans): lock migration plan v1.0 (kernel→vanilla)` — Phase 0.7
2. (main) `docs(briefs): deprecate K8.2 v1 brief (superseded by v2 after Phase 2.7 §1 stop)` — Phase 0.8
3. (main) `docs(briefs): author K8.2 v2 brief (wrapper refactor + 6 conversions + 6 deletions)` — Phase 0.9
4. (branch) `refactor(interop): add IComparable<InternedString> + InternedString.Empty constant` — Phase 2.A.1
5. (branch) `feat(native): add df_world_allocate_{map,set,composite}_id C ABI primitives` — Phase 2.A.2 (if needed; skip if native already has them)
6. (branch) `feat(interop): add NativeWorld.Allocate*Id and Create* factory methods` — Phase 2.A.3
7. (branch) `refactor(interop): NativeMap<K,V> from sealed unsafe class to readonly struct` — Phase 2.A.4
8. (branch) `refactor(interop): NativeSet<T> from sealed unsafe class to readonly struct` — Phase 2.A.5
9. (branch) `refactor(interop): NativeComposite<T> from sealed unsafe class to readonly struct` — Phase 2.A.6
10. (branch) `refactor(components): delete empty TODO stubs (Ammo/Shield/Weapon) — content deferred to M9` — Phase 2.C.1
11. (branch) `refactor(components): delete empty TODO stub (School) — content deferred to M10.B` — Phase 2.C.2
12. (branch) `refactor(components): delete empty TODO stub (Social) — content deferred to M-series` — Phase 2.C.3
13. (branch) `refactor(components): delete empty TODO stub (Biome) — content deferred to M-series` — Phase 2.C.4
14. (branch) `feat(components): convert IdentityComponent class→struct with InternedString` — Phase 2.B.1
15. (branch) `feat(components): convert WorkbenchComponent class→struct with InternedString` — Phase 2.B.2
16. (branch) `feat(components): convert FactionComponent class→struct with InternedString` — Phase 2.B.3
17. (branch) `feat(components): convert SkillsComponent class→struct with NativeMap×2` — Phase 2.B.4
18. (branch) `feat(components): convert StorageComponent class→struct with NativeMap+NativeSet` — Phase 2.B.5
19. (branch) `feat(components): convert MovementComponent class→struct with NativeComposite+HasTarget` — Phase 2.B.6
20. (branch) `feat(components): ModAccessible annotation pass on verify-only struct components (12)` — Phase 3
21. (branch) `docs(kernel): K-L3 «без exception» state achieved at K8.2; K8.1 wrapper refactor recorded; bump KERNEL_ARCHITECTURE to v1.4` — Phase 4
22. (branch) `docs(progress): record K8.2 closure (wrapper refactor + 6 conversions + 6 deletions + 12 annotations)` — Phase 5

Total: ~18-22 atomic commits depending on bundling decisions for cross-cutting consumer reconciliation.

**Per K-Lessons §2.1 atomic-commit-as-compilable-unit**: each commit leaves codebase in a buildable + testable state. If wrapper refactor (Phase 2.A) and existing K8.1 tests have dependency cycle, bundle into single «refactor(interop): wrapper value-type pass» commit.

### 1.G — Scope limits (LOCKED)

K8.2 v2 does **not**:
- Touch `src/DualFrontier.Persistence/` architecturally (codec consequence updates only if forced).
- Touch `src/DualFrontier.Application/Save/SaveSystem.cs` (`NotImplementedException` preserved).
- Move components or systems to `mods/Vanilla.*/` (M-series concern).
- Modify `IModApi` interface (M-series + K9 concern).
- Touch `mods/DualFrontier.Mod.Vanilla.*/` skeletons.
- Add new tests for verify-only components (only annotation, no behavioral change).
- Refactor existing K8.1 native side beyond adding id allocation primitives (if needed).

K8.2 v2 **does**:
- Refactor 3 K8.1 wrappers from class to readonly struct + add IComparable<InternedString>.
- Convert 6 class components to unmanaged structs using post-refactor wrappers.
- Delete 6 empty TODO stubs + their references.
- Apply `[ModAccessible]` annotations to ~12 verify-only components.
- Update `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication and Part 2 K8.2 row to reflect achieved closure.
- Update `MIGRATION_PROGRESS.md` with K8.2 closure record.
- Lock migration plan v1.0 status.
- Deprecate v1 brief.

---

## Phase 2 — Execution

### 2.A — K8.1 wrapper refactor

Sequential commits per Phase 1.F #4-9. After each commit: `dotnet build` + `dotnet test` clean. Existing K8.1 tests (`InternedStringTests`, etc.) must continue to pass — wrapper refactor is internal to bridge layer, not surface-breaking from caller perspective beyond the field shape change.

If existing K8.1 tests reference wrappers as **reference types** (e.g., null checks, identity comparison), update tests accordingly — these are **legitimate updates** because the wrappers are now value types; a reference-type assumption was implicit.

**Stop condition Phase 2.A.X**: any failure where wrapper refactor breaks an invariant K8.1 closure documented (per K8.1 closure report). Halt, document, surface to Crystalka.

### 2.B — Class→struct conversions

Per Phase 1.F #14-19. Tier ordering: InternedString-only first (Phase 2.B.1-3), then NativeMap (2.B.4), then NativeMap+NativeSet (2.B.5), then NativeComposite (2.B.6).

Each conversion's consumer reconciliation lands in same atomic commit per K-Lessons §2.1. If a system has TODO Update body and references no actual fields (per migration plan: many systems are TODO bodies), reconciliation may be empty — still verify by `grep`.

### 2.C — Empty stub deletions

Per Phase 1.F #10-13. Each deletion finds and removes all references. Sanity grep after each commit:

```
grep -rn "<DeletedComponentName>" src/ tests/ mods/
```

**Expected**: zero matches after deletion commit (or only references in `git log`/`git blame` historical contexts, which are fine).

### 2.7 — Stop conditions

Halt and surface to Crystalka if:

1. **K8.1 native side missing id allocation primitives** AND adding them is non-trivial (e.g., requires native data structure redesign). Resolution: surface, propose minimal native edit, await guidance.

2. **NativeComposite world-scoping fundamentally incompatible with per-component-instance use**. Resolution: surface; may require K-future amendment beyond K8.2 scope.

3. **Consumer count surprise**: a converted component has consumers in unexpected projects (e.g., DualFrontier.Runtime). Surface, await guidance.

4. **Persistence architectural redesign required**. Out of K8.2 scope.

5. **Test substrate inadequate**: existing test depends on class semantics struct cannot replicate.

6. **Component inventory surprise**: pre-audit count diverges substantially. Phase 0.4 lesson allows small divergence; 5+ component diff warrants halt.

7. **Empty-stub purity violation**: stub turns out to have non-TODO content. Surface, await guidance — Crystalka 2026-05-09 «снести гемпейные TODO» applies to TODO-only bodies; if reading was wrong, decision needs revisiting.

8. **Wrapper refactor reveals deeper issue**: e.g., existing K8.1 native side has implicit assumptions about wrapper class identity that don't survive value-type transition. Surface, document.

---

## Phase 3 — `[ModAccessible]` annotation completeness pass

Per Phase 1.E table. 12 verify-only components annotated. Single atomic commit. No test changes (annotations don't affect runtime behavior beyond capability resolution at mod load time, which has no tests touching it pre-K8.2).

**Atomic commit**: `feat(components): ModAccessible annotation pass on verify-only struct components (12)`

---

## Phase 4 — KERNEL_ARCHITECTURE.md amendment

### 4.1 — Part 2 master plan K8.2 row

Replace existing K8.2 row:
```markdown
| K8.2 | 7 class components redesigned to structs | 1-2 weeks | -200/+300 |
```

With:
```markdown
| K8.2 | K-L3 «без exception» closure: K8.1 wrapper value-type refactor (NativeMap/NativeSet/NativeComposite to readonly struct + IComparable<InternedString> + per-instance id allocation strategy); 6 class→struct conversions (Identity/Workbench/Faction/Skills/Storage/Movement); 6 empty TODO stub deletions (Ammo/Shield/Weapon/School/Social/Biome — content deferred to M-series); 12 ModAccessible annotation completeness pass | 6-12 hours auto-mode (3-5 days hobby) | +800/-1500 |
```

### 4.2 — Part 0 K-L3 implication

Replace existing K-L3 implication paragraph with:

```markdown
**Implication of K-L3**: All managed components must be unmanaged structs. **K8.2 (`MIGRATION_PROGRESS.md` K8.2 closure entry, 2026-05-09) achieved K-L3 «без exception» state**: K8.1 managed bridge wrappers (NativeMap/NativeSet/NativeComposite) refactored to readonly struct value types; InternedString gained IComparable<InternedString>; per-instance id allocation strategy locked (NativeWorld global counter). 6 class components converted to structs using the value-type wrapper surface (Identity/Workbench/Faction/Skills/Storage/Movement). 6 empty TODO-stub components deleted (Ammo/Shield/Weapon/School/Social/Biome) per METHODOLOGY §7.1 «data exists or it doesn't» — placeholder lies removed; real content authored fresh in M-series vanilla mod content milestones (M9 Combat, M10.B Magic, M-series Pawn social, M-series World biome). Mod components subject to same constraint; M-series content commits author components as `unmanaged struct` from inception. **K4's "Hybrid Path" softening retired**.
```

### 4.3 — Part 7 amendment (optional, if relevant)

If wrapper refactor surfaces architectural lesson worth recording (e.g., «value-type wrappers as first-class K-L3 enforcement mechanism»), add as new sub-section in Part 7 «Methodology adjustments для K-series». Lock at execution time per Cloud Code judgment.

### 4.4 — Version bump

```markdown
**Version**: 1.4
**Date**: 2026-05-09
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3); K-L3 «без exception» state achieved at K8.2 closure with K8.1 wrapper value-type refactor (v1.4)
```

**Atomic commit**: `docs(kernel): K-L3 «без exception» state achieved at K8.2; K8.1 wrapper refactor recorded; bump KERNEL_ARCHITECTURE to v1.4`

---

## Phase 5 — Closure verification

### 5.1 — `MIGRATION_PROGRESS.md` update

Locate K-series Overview table K8.2 row:
```markdown
| K8.2 | 7 class components redesigned to structs | NOT STARTED | 1-2 weeks | — | — |
```

Replace:
```markdown
| K8.2 | K-L3 «без exception» closure: wrapper refactor + 6 conversions + 6 deletions + 12 annotations | DONE | 6-12 hours auto-mode | `<sha-2A1>..<sha-5>` | 2026-05-09 |
```

Add closure section block (matching K8.1/K8.1.1/K-Lessons shape):

```markdown
### K8.2 — K-L3 «без exception» closure

**Status**: DONE
**Closure**: `<sha-2A1>..<sha-5>` on `feat/k82-foundation-closure` (fast-forward merged to main)
**Brief**: `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V2.md` (v1 deprecated to `..._V1_DEPRECATED.md`)
**Test count**: 592 → <new>

**Deliverables**:
- K8.1 wrapper refactor: NativeMap<K,V>, NativeSet<T>, NativeComposite<T> from `sealed unsafe class` to `readonly struct (IntPtr, uint)`; InternedString gained IComparable; NativeWorld gained AllocateMapId/AllocateSetId/AllocateCompositeId + CreateMap/CreateSet/CreateComposite factory methods; native side gained corresponding C ABI primitives (if not pre-existing).
- 6 class→struct conversions: IdentityComponent (InternedString), WorkbenchComponent (InternedString, Empty=idle), FactionComponent (InternedString), SkillsComponent (2× NativeMap), StorageComponent (NativeMap+NativeSet), MovementComponent (NativeComposite + bool HasTarget).
- 6 empty TODO stub deletions: AmmoComponent, ShieldComponent, WeaponComponent (Combat); SchoolComponent (Magic); SocialComponent (Pawn); BiomeComponent (World). All consumer references removed.
- 12 verify-only ModAccessible annotations applied per migration plan §1.5 table.
- KERNEL_ARCHITECTURE.md v1.3 → v1.4: Part 2 K8.2 row, Part 0 K-L3 implication, Status line updated.
- MIGRATION_PLAN_KERNEL_TO_VANILLA.md status: AUTHORED → LOCKED v1.0 (committed 2026-05-09).
- K8.2 v1 brief deprecated and renamed.

**Brief deviations**: per execution. Possible: NativeComposite per-instance vs world-scoped resolution at execution time; NativeMap iteration syntax discovery.

**Architectural decisions LOCKED in this milestone**:
- K8.1 wrapper class→struct refactor (was Cloud Code's recommended K8.1.2; folded into K8.2 v2 per Crystalka Variant 3 lock 2026-05-09).
- Per-instance id allocation strategy: NativeWorld global counter, monotonic uint, 0 as invalid sentinel.
- Empty TODO stub deletions per METHODOLOGY §7.1.

**Cross-cutting impact**:
- K8.3 (system migration) unblocked: all 6 surviving consumer-bearing components are now struct + use post-refactor wrapper surface.
- M-series migrations (M8.4 World, M8.5-M8.7 Pawn, M9 Combat, M10/M10.B Magic/Inventory) now consume K-L3-clean components without wrapper-class artifacts.
- K-L3 «без exception» state achieved; KERNEL_ARCHITECTURE Part 0 implication updated.
```

### 5.2 — Live tracker update

«Current state snapshot» section:
- **Active phase**: K8.3 (next per migration plan §1.2 and KERNEL_ARCHITECTURE Part 2)
- **Last completed milestone**: K8.2 (K-L3 «без exception» closure: wrapper refactor + 6 conversions + 6 deletions + 12 annotations)
- **Next milestone (recommended)**: K8.3 (34 systems → SpanLease/WriteBatch)
- **Tests passing**: <new baseline> (was 592)
- **Last updated**: 2026-05-09 (K8.2 closure)

### 5.3 — Brief mark EXECUTED

```markdown
**Status**: EXECUTED (2026-05-09, branch `feat/k82-foundation-closure`, closure `<sha-2A1>..<sha-5>`). See `docs/MIGRATION_PROGRESS.md` § "K8.2 — K-L3 «без exception» closure" for closure record.
```

### 5.4 — Final verification

```
git status
git log --oneline -25
dotnet build
dotnet test
```

Expected: clean tree, ~18-22 K8.2 commits visible, build clean, tests pass at new baseline.

**Sanity grep — no class IComponent left**:
```
grep -rE "public sealed class.*IComponent" src/DualFrontier.Components/
```
Expected: zero matches.

**Sanity grep — deleted stubs gone**:
```
grep -rn "AmmoComponent\|ShieldComponent\|WeaponComponent\|SchoolComponent\|SocialComponent\|BiomeComponent" src/ tests/ mods/
```
Expected: zero matches (or only doc/changelog historical references).

**Sanity grep — annotation coverage**:
```
grep -rcE "ModAccessible" src/DualFrontier.Components/
```
Expected: ≥ 25 lines (25 surviving components, all annotated).

### 5.5 — Merge to main

```
git checkout main
git merge --ff-only feat/k82-foundation-closure
```

Halt if non-fast-forward. K8.2 v2 expects to land cleanly atop K-Lessons closure SHA `da092d2`.

**Do not push to origin**. Crystalka decision after closure report.

---

## Phase 6 — Execution contingencies

### 6.1 — Single-session feasibility

K8.2 v2 estimated 6-12 hours auto-mode. If session approaches limit (Cloud Code judges based on remaining work + complexity):

**Split point** (LOCKED): after Phase 2.A (wrapper refactor) + 3 InternedString-only conversions (Phase 2.B.1-3) + 6 deletions (Phase 2.C). Land partial closure as «K8.2-part-1»; Skills/Storage/Movement (NativeMap/NativeSet/NativeComposite-using) defer to «K8.2-part-2» next session.

In that case:
- Brief at MIGRATION_PROGRESS records partial closure with explicit deferral
- KERNEL_ARCHITECTURE K-L3 implication says «K-L3 partial closure at K8.2-part-1; «без exception» pending K8.2-part-2»
- K8.2-part-2 brief authored next session, references this v2 brief and the part-1 closure SHA

### 6.2 — Native side gap on id allocation

If K8.1 native side doesn't have `df_world_allocate_*_id` primitives and adding them touches structs/headers in non-trivial way: surface, propose minimal addition, await Crystalka guidance. Do not invent native side architectural changes mid-K8.2.

### 6.3 — NativeComposite world-scoping irreconcilable

If NativeComposite cannot be value-type wrapped without breaking K8.1 LOCKED line 293 invariants: surface, propose K8.2 scope reduction to wrapper-refactor + 5 conversions + Movement deferred, OR propose K-future amendment.

### 6.4 — Persistence layer breaks

If persistence codec layer (`src/DualFrontier.Persistence/Compression/*.cs`) requires architectural redesign to handle struct components with NativeMap fields: this is **out of K8.2 v2 scope**. Surface, propose:
- (a) Partial K8.2: convert components, leave persistence tests failing temporarily; record as known-debt with K-future cleanup milestone
- (b) Halt K8.2: persistence too tightly coupled, requires wider redesign

Crystalka decision per surface report.

---

## Cross-cutting design constraints

- **K-Lessons compliance**: 4 lessons applied:
  - Atomic commit as compilable unit (METHODOLOGY §K-Lessons): per-conversion + per-deletion + wrapper-per-file commits
  - Phase 0.4 inventory as hypothesis: pre-audit table verified at execution time, divergences recorded
  - Mod-scope test isolation: all reclaim tests take references inside scope under test
  - Error semantics convention (KERNEL Part 7): post-refactor wrappers comply (sparse abstraction → bool returns; lifecycle → throw)

- **Migration plan compliance**: implements §1.1 reformulated K8.2 scope (31 components touched: 6 convert + 6 delete + 19 verify-only), achieves K-L3 «без exception» per §0.3 LOCKED #3.

- **No M-series scope creep**: zero touches to `mods/Vanilla.*/`, no manifest edits, no IModApi changes.

- **No save-system scope creep**: SaveSystem.cs preserved as `NotImplementedException`.

- **No game mechanics design**: deletions remove placeholder lies; real content is M-series scope.

- **Documentation atomicity**: Part 2 row + K-L3 implication + version bump bundled into one Phase 4 commit.

- **Migration plan housekeeping bundled into Phase 0**: avoid leaving plan in untracked + AUTHORED state across milestone boundaries.

---

## Execution closure

When all phases complete, K8.2 v2 is **DONE**.

Closure report to Crystalka:
- Commit range merged to main
- New test baseline (vs 592 pre-K8.2)
- KERNEL_ARCHITECTURE v1.3 → v1.4 confirmed
- Migration plan LOCKED v1.0 confirmed
- v1 brief deprecated
- K-L3 «без exception» state confirmed achieved
- Brief deviations recorded
- Architectural surprises (if any) for K8.3 brief authoring or K-future planning

Crystalka decides post-closure:
- Push `main` to `origin` (currently 28 ahead pre-K8.2; will be ~50 ahead post-K8.2)
- K8.3 brief authoring next session
- Whether any wrapper refactor surface lesson warrants K-Lessons amendment

End of K8.2 v2 brief.
