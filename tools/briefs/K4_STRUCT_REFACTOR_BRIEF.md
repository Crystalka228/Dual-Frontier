# K4 — Component struct refactor (Hybrid path)

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-07
**Status**: READY FOR EXECUTION
**Reference docs**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K4, `docs/MIGRATION_PROGRESS.md` (K3 closure context), `docs/METHODOLOGY.md` v1.4, `docs/PERFORMANCE_REPORT_K3.md` (motivation evidence)
**Predecessor**: K3 (`7629f57`) — bootstrap graph + thread pool, merged to main
**Target**: fresh feature branch `feat/k4-struct-refactor` от `main`
**Estimated time**: 3-5 hours auto-mode (3-4 days at hobby pace, ~1h/day manual typing) — applying METHODOLOGY v1.4 calibration
**Estimated LOC delta**: ~+200/-150 (mostly converting `class` to `struct`, removing object initialization noise)

---

## Goal

Convert 23 trivial POCO components from `sealed class` to `struct`, register them в native ECS through `ComponentTypeRegistry`, и verify roundtrip through native bulk path. Implements Hybrid Path (Q1 architectural decision) — components с reference type fields (Dictionary, List, string, HashSet) **stay as classes** on managed path.

После K4: Категория A trivial components могут быть pushed через native bulk Add/Get achieving 2.7x throughput speedup measured in K1. Категория B/C components остаются на managed path с full functionality preserved.

**В scope**:
- 23 trivial component conversions (Категория A) — class → struct
- `where T : unmanaged` constraint enforcement compile-time через generic constraints
- `VanillaComponentRegistration.cs` helper для bootstrap registration
- Smoke test: `AllVanillaComponentsRoundTripTest` verifying registered structs roundtrip correctly through native
- Tricky-case tests for components с computed properties, EntityId? fields, init-only patterns
- MIGRATION_PROGRESS.md K4 closure update

**НЕ в scope** (explicit boundary):
- ❌ Категория B (collections): SkillsComponent, SocialComponent, StorageComponent, MovementComponent, WorkbenchComponent — **stay as classes**
- ❌ Категория C (strings): IdentityComponent, FactionComponent — **stay as classes** (note: WorkbenchComponent overlaps B/C, counted in B)
- ❌ ArrayPool fix для `AddComponents<T>` (K5 scope)
- ❌ Mod-driven component registration (K6 scope)
- ❌ Replacement of managed `World` с `NativeWorld` в Application (K8 cutover)
- ❌ TODO content в empty components (e.g. `WeaponComponent.Damage`) — these are GDD-tracked, separate work
- ❌ System code changes — existing get/modify/set pattern works с structs unchanged

---

## Architectural decisions (from 2026-05-07 K4 design discussion)

### Q1 — Reference type fields: **Hybrid (A)**

Trivial POCO components → `struct` (через native bulk path).
Components с `Dictionary`, `List`, `HashSet`, `string` fields → **stay as `class`** (managed path).

Rationale: cleanness > expediency. Native batching `where T : unmanaged` constraint enforcement требует pure value types. Forcing decomposition of collections (`SkillsComponent.Levels` Dictionary → separate native table) would require ~20-30 callsite changes per Dictionary с no proportionate performance benefit (collections aren't hot path for batching).

K8 Outcome 2 (mixed managed+native) becomes principled architecture, not compromise.

### Q2 — Conversion strategy: **Per-component atomic commits**

23 components = ~23-25 commits (some may need 2 commits if computed properties require careful conversion).

Rationale: bisect-friendly если K7 показывает regression — найти точно какой component виноват. Simple revert per item. Auto-mode pace makes per-component commits trivial (~5-10 min execution each).

### Q3 — Native registration: **Explicit at Application bootstrap**

`VanillaComponentRegistration.RegisterAll(ComponentTypeRegistry registry)` — static method called в Application bootstrap chain (after `Bootstrap.Run()` returns NativeWorld, before any system access).

Rationale: matches K-L4 deterministic registry principle. Visible registration order = deterministic type IDs across runs. No reflection overhead. Mod-aware registration is K6 scope.

### Q4 — Test strategy: **Smoke + tricky-case tests**

One smoke test iterates всех registered structs verifying basic roundtrip (write → read returns same data). Tricky-case tests для:
- `JobComponent` (EntityId? nullable field)
- `NeedsComponent` (computed properties: IsHungry, IsThirsty, IsExhausted)
- `RaceComponent` (multiple computed properties: IsOrganic, NeedsFood, NeedsSleep + init-only fields)
- `GolemBondComponent` (init-only fields с EntityId?)

Rationale: existing 472 tests **уже** cover behavior через consumer systems (DamageSystem uses Health, NeedsSystem uses Needs, etc.). If roundtrip breaks, downstream behavior breaks — existing tests catch it. Plus dedicated smoke + 4 tricky-case tests = ~5 new tests total.

---

## Component inventory — final classification

### Категория A (convert to struct) — 23 components

| # | Component | Path | Fields summary | Notes |
|---|---|---|---|---|
| 1 | HealthComponent | Shared/HealthComponent.cs | TODO empty | Stub fields per TODO comments |
| 2 | PositionComponent | Shared/PositionComponent.cs | `GridVector Position` | GridVector is struct (verify) |
| 3 | RaceComponent | Shared/RaceComponent.cs | `RaceKind, bool×2, computed bool×3` | Has init-only — tricky case test |
| 4 | NeedsComponent | Pawn/NeedsComponent.cs | `float×4, computed bool×3` | Has computed properties — tricky case test |
| 5 | MindComponent | Pawn/MindComponent.cs | `float×2, bool, int, computed bool×3` | |
| 6 | JobComponent | Pawn/JobComponent.cs | `JobKind, EntityId?, int, bool, computed bool×2` | EntityId? nullable — tricky case test |
| 7 | BedComponent | Items/BedComponent.cs | `EntityId?, float` | |
| 8 | ConsumableComponent | Items/ConsumableComponent.cs | `NeedKind, float, int` | |
| 9 | WaterSourceComponent | Items/WaterSourceComponent.cs | `float` | |
| 10 | DecorativeAuraComponent | Items/DecorativeAuraComponent.cs | `int, float` | |
| 11 | ReservationComponent | Items/ReservationComponent.cs | `EntityId, long` | |
| 12 | TileComponent | World/TileComponent.cs | `TerrainKind, bool` | Has static `Default` factory — keep |
| 13 | BiomeComponent | World/BiomeComponent.cs | TODO empty | Stub fields per TODO |
| 14 | EtherNodeComponent | World/EtherNodeComponent.cs | TODO empty | Stub fields per TODO |
| 15 | EtherComponent | Magic/EtherComponent.cs | TODO empty | Stub fields per TODO |
| 16 | ManaComponent | Magic/ManaComponent.cs | TODO empty | Stub fields per TODO |
| 17 | GolemBondComponent | Magic/GolemBondComponent.cs | `EntityId?, OwnershipMode, int×2` | All init-only — tricky case test |
| 18 | SchoolComponent | Magic/SchoolComponent.cs | TODO empty | Stub fields per TODO |
| 19 | AmmoComponent | Combat/AmmoComponent.cs | TODO empty | Stub fields per TODO |
| 20 | ArmorComponent | Combat/ArmorComponent.cs | TODO empty | Stub fields per TODO |
| 21 | ShieldComponent | Combat/ShieldComponent.cs | TODO empty | Stub fields per TODO |
| 22 | WeaponComponent | Combat/WeaponComponent.cs | TODO empty | Stub fields per TODO |
| 23 | PowerConsumerComponent | Building/PowerConsumerComponent.cs | `float, bool, int` | |
| 24 | PowerProducerComponent | Building/PowerProducerComponent.cs | `float×2, bool, float` | |

Wait — that's 24, not 23. Let me recount: 4 Shared + 4 Pawn + 5 Items + 3 World + 4 Magic + 4 Combat + 2 Building (PowerProducer, PowerConsumer; не Storage/Workbench which are в Категории B).

4+4+5+3+4+4+2 = **26**. Adjust counting:

Actually:
- Shared (4): Health, Position, Race + Faction (Faction is Категория C, exclude). = **3**
- Pawn (7 total): Identity (C), Job (A), Mind (A), Movement (B - List), Needs (A), Skills (B), Social (B). = **3 in A** (Job, Mind, Needs)
- Items (5): Bed, Consumable, DecorativeAura, Reservation, WaterSource. **All A** = **5**
- World (3): Biome, EtherNode, Tile. **All A** = **3**
- Magic (4): Ether, GolemBond, Mana, School. **All A** = **4**
- Combat (4): Ammo, Armor, Shield, Weapon. **All A** = **4**
- Building (4): PowerConsumer, PowerProducer, Storage (B), Workbench (B). **2 in A** (PowerConsumer, PowerProducer)

Total Category A: 3 + 3 + 5 + 3 + 4 + 4 + 2 = **24 components**.

Updating inventory count: **К4 converts 24 components**, not 23.

### Категория B (stay class — collections) — 5 components

| Component | Path | Reason kept managed |
|---|---|---|
| MovementComponent | Pawn/MovementComponent.cs | `List<GridVector> Path` |
| SkillsComponent | Pawn/SkillsComponent.cs | `Dictionary<SkillKind, int>, Dictionary<SkillKind, float>` |
| SocialComponent | Pawn/SocialComponent.cs | TODO `Dictionary<EntityId, int>` |
| StorageComponent | Building/StorageComponent.cs | `Dictionary<string, int>, HashSet<string>` |
| WorkbenchComponent | Building/WorkbenchComponent.cs | `string? ActiveRecipeId` |

### Категория C (stay class — strings) — 2 components

| Component | Path | Reason kept managed |
|---|---|---|
| IdentityComponent | Pawn/IdentityComponent.cs | `string Name` |
| FactionComponent | Shared/FactionComponent.cs | `string FactionId` |

**Total: 24 + 5 + 2 = 31 components.** All accounted for.

---

## Step 0 — Brief authoring commit (METHODOLOGY v1.3 prerequisite)

```powershell
cd D:\Colony_Simulator\Colony_Simulator
git status
# Expected: K4_STRUCT_REFACTOR_BRIEF.md modified (skeleton → full brief)

git add tools/briefs/K4_STRUCT_REFACTOR_BRIEF.md
git commit -m "docs(briefs): K4 brief authored — full executable struct refactor"
```

After this — working tree clean, HG-1 will pass.

---

## Throw inventory (METHODOLOGY v1.3)

K4 introduces **zero new throws в native code**. К4 это purely managed-side refactor + native registration. Existing native throw inventory (from K0-K3) unchanged.

**Managed-side throw points** (existing, не new):
- `ComponentTypeRegistry.Register<T>` — `InvalidOperationException` if native registration fails (already existed К2)
- `NativeWorld.AddComponent<T>` — generic `where T : unmanaged` constraint enforced compile-time (no runtime throw)

No new try/catch blocks needed. No ABI boundary changes.

---

## Pre-flight checks (METHODOLOGY v1.2 descriptive style)

### Hard gates (STOP-eligible)

#### HG-1: Working tree clean
```powershell
git status                                # должен быть clean (после Step 0)
```

#### HG-2: K3 successfully merged to main
```powershell
git log main --oneline | Select-String "K3|bootstrap" | Select-Object -First 5
# Expected: visible commits for K3 closure
```

If K3 commits not visible — STOP, K3 not merged.

#### HG-3: Component source intact
```powershell
Test-Path src\DualFrontier.Components\Shared\HealthComponent.cs
Test-Path src\DualFrontier.Components\Pawn\NeedsComponent.cs
Test-Path src\DualFrontier.Components\Items\BedComponent.cs
# ... all 24 Категория A components present
```

#### HG-4: Native infrastructure intact (K0-K3 baseline)
```powershell
Test-Path src\DualFrontier.Core.Interop\Marshalling\ComponentTypeRegistry.cs
Test-Path src\DualFrontier.Core.Interop\Bootstrap.cs

# Verify K3 ABI present:
Select-String -Path native\DualFrontier.Core.Native\include\df_capi.h -Pattern "df_engine_bootstrap"
# Expected: match
```

#### HG-5: Baseline tests passing
```powershell
dotnet test
# Expected: 472 passing, 0 failed (post-K3 baseline)
```

#### HG-6: Native build clean
```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
# Expected: 0 errors, 0 warnings
```

#### HG-7: Native selftest passing
```powershell
.\build\Release\df_native_selftest.exe
# Expected: 12 scenarios ALL PASSED (post-K3 baseline)
```

#### HG-8: ALL Категория A components currently classes
```powershell
# Verify none accidentally already converted к struct
foreach ($file in @(
    "src\DualFrontier.Components\Shared\HealthComponent.cs",
    "src\DualFrontier.Components\Shared\PositionComponent.cs",
    "src\DualFrontier.Components\Shared\RaceComponent.cs",
    "src\DualFrontier.Components\Pawn\NeedsComponent.cs",
    "src\DualFrontier.Components\Pawn\MindComponent.cs",
    "src\DualFrontier.Components\Pawn\JobComponent.cs",
    "src\DualFrontier.Components\Items\BedComponent.cs"
    # ... abbreviated; check first ~10 как sample
)) {
    $content = Get-Content $file -Raw
    if ($content -match 'public\s+(sealed\s+)?struct') {
        Write-Host "STOP: $file already a struct"
    }
}
```

If any already struct — STOP, K4 partially executed or unexpected state.

### Informational checks (record-only)

#### INF-1: Total component count
```powershell
(Get-ChildItem src\DualFrontier.Components -Recurse -Filter "*Component.cs" | Measure-Object).Count
# Expected: 31 component files (excluding *Kind.cs enums)
```

#### INF-2: Test counts by project
```powershell
dotnet test --list-tests --no-build | Select-String "^    " | Measure-Object
# Expected: 472 total
```

#### INF-3: Recent commit history
```powershell
git log main --oneline -5
# Record HEAD SHA для K4 closure reference
```

---

## Step 1 — Branch setup

```powershell
git checkout main
git pull origin main
git checkout -b feat/k4-struct-refactor main
```

Branch name: **`feat/k4-struct-refactor`** (точно).

---

## Step 2 — Component conversions (24 atomic commits, batched by category)

For each component, the conversion pattern is:

**Before** (class):
```csharp
public sealed class FooComponent : IComponent
{
    public int FieldA { get; set; }
    public float FieldB;
    public bool ComputedFlag => FieldA > 0;
}
```

**After** (struct):
```csharp
public struct FooComponent : IComponent
{
    public int FieldA;       // properties с auto-{ get; set; } можно конвертировать в fields
    public float FieldB;     // (matches existing "pure data" semantic)
    public bool ComputedFlag => FieldA > 0;  // computed properties работают со struct
}
```

**Conversion rules**:
1. Replace `sealed class` → `struct` (drop `sealed` modifier — structs are implicitly sealed)
2. Replace `IComponent` interface implementation — keep (works со struct, no boxing с `where T` constraints)
3. Convert `{ get; set; }` auto-properties to public fields (matches IComponent "data only" semantic, simpler memory layout)
4. Keep `{ get; init; }` properties as-is (works со struct, valid construction pattern)
5. Keep computed expression-bodied properties (`=> ...`) — work со struct
6. Keep `const` declarations — work со struct
7. Keep static `Default` factory методы (TileComponent has one) — adjust к return struct
8. Drop default field initializers if they would prevent struct's implicit `default` constructor (rare)

**Special handling**:

#### TileComponent — static factory method
**Before**:
```csharp
public sealed class TileComponent : IComponent
{
    public TerrainKind Terrain;
    public bool Passable;
    public static TileComponent Default =>
        new TileComponent { Terrain = TerrainKind.Grass, Passable = true };
}
```

**After**:
```csharp
public struct TileComponent : IComponent
{
    public TerrainKind Terrain;
    public bool Passable;
    public static TileComponent Default =>
        new TileComponent { Terrain = TerrainKind.Grass, Passable = true };
}
```

(Object initializer syntax работает со struct идентично.)

#### Components с TODO empty bodies

Several components currently have only TODO comments:
- HealthComponent: `// TODO: public float Current; // TODO: public float Maximum; // TODO: public bool IsDead => Current <= 0;`
- BiomeComponent, EtherNodeComponent, EtherComponent, ManaComponent, SchoolComponent, AmmoComponent, ArmorComponent, ShieldComponent, WeaponComponent

**For these**: implement the TODO fields per their comments, drop TODO markers. Specifically:

| Component | Implement |
|---|---|
| HealthComponent | `public float Current; public float Maximum; public bool IsDead => Current <= 0;` |
| BiomeComponent | Skip — no TODO field specified, leave empty struct |
| EtherNodeComponent | `public int Tier; public float Radius;` |
| EtherComponent | `public int Level;  // 1..5` |
| ManaComponent | `public float Current; public float Maximum; public float RegenerationRate;` |
| SchoolComponent | Skip — depends on undefined `MagicSchool` enum, leave empty struct |
| AmmoComponent | Skip — depends on undefined `AmmoType` enum, leave empty struct |
| ArmorComponent | `public float SharpResist; public float BluntResist; public float HeatResist;` |
| ShieldComponent | Skip — depends on undefined `ShieldKind` enum, leave empty struct |
| WeaponComponent | Skip — depends on undefined `DamageType` enum, leave empty struct |

For empty struct — the struct can have **no fields**, just be `public struct FooComponent : IComponent { }`. Это valid и unmanaged-compatible.

#### Components с init-only properties

`RaceComponent`, `GolemBondComponent` use `{ get; init; }` extensively. Convert by:
1. Keep `init` property syntax (works со struct)
2. Computed properties remain expression-bodied
3. Default values via property initializers stay (`public RaceKind Kind { get; init; } = RaceKind.Human;`)

### Step 2.1 — Batch 1: Shared category (3 components)

Convert in single batch, one commit per component:

```powershell
# 2.1.a — HealthComponent
# Edit src/DualFrontier.Components/Shared/HealthComponent.cs:
#   - Replace `sealed class` с `struct`
#   - Implement TODO fields per Special Handling table
git add src/DualFrontier.Components/Shared/HealthComponent.cs
git commit -m "refactor(components): convert HealthComponent to struct"

# 2.1.b — PositionComponent
# Edit src/DualFrontier.Components/Shared/PositionComponent.cs
git add src/DualFrontier.Components/Shared/PositionComponent.cs
git commit -m "refactor(components): convert PositionComponent to struct"

# 2.1.c — RaceComponent (has init-only — preserve syntax)
# Edit src/DualFrontier.Components/Shared/RaceComponent.cs
git add src/DualFrontier.Components/Shared/RaceComponent.cs
git commit -m "refactor(components): convert RaceComponent to struct"

# Build verification после batch
dotnet build
# Expected: 0 errors, possibly warnings about IsAssignableFrom checks
```

### Step 2.2 — Batch 2: Pawn category trivial (3 components)

```powershell
# 2.2.a — NeedsComponent (has computed properties)
git add src/DualFrontier.Components/Pawn/NeedsComponent.cs
git commit -m "refactor(components): convert NeedsComponent to struct"

# 2.2.b — MindComponent
git add src/DualFrontier.Components/Pawn/MindComponent.cs
git commit -m "refactor(components): convert MindComponent to struct"

# 2.2.c — JobComponent (has EntityId? nullable)
git add src/DualFrontier.Components/Pawn/JobComponent.cs
git commit -m "refactor(components): convert JobComponent to struct"

dotnet build
```

### Step 2.3 — Batch 3: Items category (5 components)

```powershell
# 2.3.a — BedComponent (has EntityId? nullable)
git add src/DualFrontier.Components/Items/BedComponent.cs
git commit -m "refactor(components): convert BedComponent to struct"

# 2.3.b — ConsumableComponent
git add src/DualFrontier.Components/Items/ConsumableComponent.cs
git commit -m "refactor(components): convert ConsumableComponent to struct"

# 2.3.c — DecorativeAuraComponent
git add src/DualFrontier.Components/Items/DecorativeAuraComponent.cs
git commit -m "refactor(components): convert DecorativeAuraComponent to struct"

# 2.3.d — ReservationComponent
git add src/DualFrontier.Components/Items/ReservationComponent.cs
git commit -m "refactor(components): convert ReservationComponent to struct"

# 2.3.e — WaterSourceComponent
git add src/DualFrontier.Components/Items/WaterSourceComponent.cs
git commit -m "refactor(components): convert WaterSourceComponent to struct"

dotnet build
```

### Step 2.4 — Batch 4: World category (3 components)

```powershell
# 2.4.a — TileComponent (has static Default factory)
git add src/DualFrontier.Components/World/TileComponent.cs
git commit -m "refactor(components): convert TileComponent to struct"

# 2.4.b — BiomeComponent (empty struct)
git add src/DualFrontier.Components/World/BiomeComponent.cs
git commit -m "refactor(components): convert BiomeComponent to struct"

# 2.4.c — EtherNodeComponent (implement TODO fields)
git add src/DualFrontier.Components/World/EtherNodeComponent.cs
git commit -m "refactor(components): convert EtherNodeComponent to struct"

dotnet build
```

### Step 2.5 — Batch 5: Magic category (4 components)

```powershell
# 2.5.a — EtherComponent (implement TODO field)
git add src/DualFrontier.Components/Magic/EtherComponent.cs
git commit -m "refactor(components): convert EtherComponent to struct"

# 2.5.b — GolemBondComponent (init-only fields)
git add src/DualFrontier.Components/Magic/GolemBondComponent.cs
git commit -m "refactor(components): convert GolemBondComponent to struct"

# 2.5.c — ManaComponent (implement TODO fields)
git add src/DualFrontier.Components/Magic/ManaComponent.cs
git commit -m "refactor(components): convert ManaComponent to struct"

# 2.5.d — SchoolComponent (empty struct, depends on undefined enum)
git add src/DualFrontier.Components/Magic/SchoolComponent.cs
git commit -m "refactor(components): convert SchoolComponent to struct"

dotnet build
```

### Step 2.6 — Batch 6: Combat category (4 components)

```powershell
# 2.6.a — AmmoComponent (empty struct)
git add src/DualFrontier.Components/Combat/AmmoComponent.cs
git commit -m "refactor(components): convert AmmoComponent to struct"

# 2.6.b — ArmorComponent (implement TODO fields)
git add src/DualFrontier.Components/Combat/ArmorComponent.cs
git commit -m "refactor(components): convert ArmorComponent to struct"

# 2.6.c — ShieldComponent (empty struct)
git add src/DualFrontier.Components/Combat/ShieldComponent.cs
git commit -m "refactor(components): convert ShieldComponent to struct"

# 2.6.d — WeaponComponent (empty struct)
git add src/DualFrontier.Components/Combat/WeaponComponent.cs
git commit -m "refactor(components): convert WeaponComponent to struct"

dotnet build
```

### Step 2.7 — Batch 7: Building category (2 components)

```powershell
# 2.7.a — PowerConsumerComponent
git add src/DualFrontier.Components/Building/PowerConsumerComponent.cs
git commit -m "refactor(components): convert PowerConsumerComponent to struct"

# 2.7.b — PowerProducerComponent
git add src/DualFrontier.Components/Building/PowerProducerComponent.cs
git commit -m "refactor(components): convert PowerProducerComponent to struct"

dotnet build

# Verify all 472 tests still passing после full conversion
dotnet test
# Expected: 472 passing — system code unchanged, struct copy semantics preserve behavior
```

If `dotnet test` shows regressions — INVESTIGATE before continuing. Common causes:
- Component has reference field we missed (move к Категория B)
- Test uses identity check (`==` reference comparison) on component — change к value comparison
- Component default value differs (struct field default vs class field initializer)

---

## Step 3 — VanillaComponentRegistration helper

### 3.1 — Create `src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs`

```csharp
using DualFrontier.Components.Building;
using DualFrontier.Components.Combat;
using DualFrontier.Components.Items;
using DualFrontier.Components.Magic;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Components.World;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Application.Bootstrap;

/// <summary>
/// Registers all 24 Vanilla trivial-POCO components с the native
/// ComponentTypeRegistry. Called по Application bootstrap chain после
/// Bootstrap.Run() returns ready NativeWorld.
///
/// Per K4 Hybrid Path: only Категория A trivial structs registered. 
/// Категория B (collections) и Категория C (strings) components stay 
/// on managed path — not registered с native registry.
///
/// Per K-L4 deterministic registry principle: registration order is 
/// stable, type IDs are sequential (1, 2, 3, ... 24). Mod-driven 
/// registration extends this list at K6 milestone.
/// </summary>
public static class VanillaComponentRegistration
{
    /// <summary>
    /// Register all 24 Vanilla trivial-POCO components.
    /// </summary>
    /// <param name="registry">The component type registry to register с.</param>
    public static void RegisterAll(ComponentTypeRegistry registry)
    {
        // Shared category (3)
        registry.Register<HealthComponent>();
        registry.Register<PositionComponent>();
        registry.Register<RaceComponent>();

        // Pawn category — trivial (3)
        registry.Register<NeedsComponent>();
        registry.Register<MindComponent>();
        registry.Register<JobComponent>();

        // Items category (5)
        registry.Register<BedComponent>();
        registry.Register<ConsumableComponent>();
        registry.Register<DecorativeAuraComponent>();
        registry.Register<ReservationComponent>();
        registry.Register<WaterSourceComponent>();

        // World category (3)
        registry.Register<TileComponent>();
        registry.Register<BiomeComponent>();
        registry.Register<EtherNodeComponent>();

        // Magic category (4)
        registry.Register<EtherComponent>();
        registry.Register<GolemBondComponent>();
        registry.Register<ManaComponent>();
        registry.Register<SchoolComponent>();

        // Combat category (4)
        registry.Register<AmmoComponent>();
        registry.Register<ArmorComponent>();
        registry.Register<ShieldComponent>();
        registry.Register<WeaponComponent>();

        // Building category — trivial (2)
        registry.Register<PowerConsumerComponent>();
        registry.Register<PowerProducerComponent>();
    }
}
```

### 3.2 — Verify project references

`DualFrontier.Application.csproj` already references `DualFrontier.Components` and `DualFrontier.Core.Interop`. No changes needed unless Bootstrap subdirectory is new — verify:

```powershell
Test-Path src\DualFrontier.Application\Bootstrap
# If False, mkdir created it via create_file
```

### 3.3 — Atomic commit

```powershell
git add src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs
git commit -m "refactor(components): VanillaComponentRegistration bootstrap helper

Registers all 24 trivial-POCO components с ComponentTypeRegistry.
Called по Application bootstrap chain после Bootstrap.Run() returns 
ready NativeWorld.

Per K4 Q3 architectural decision (cleanness): explicit deterministic 
registration. Type IDs are sequential 1..24 across runs. Mod-driven 
registration extends this list at K6.

Categories registered:
- Shared (3): Health, Position, Race
- Pawn trivial (3): Needs, Mind, Job
- Items (5): Bed, Consumable, DecorativeAura, Reservation, WaterSource
- World (3): Tile, Biome, EtherNode
- Magic (4): Ether, GolemBond, Mana, School
- Combat (4): Ammo, Armor, Shield, Weapon
- Building trivial (2): PowerConsumer, PowerProducer

NOT registered (Категория B/C, stay managed):
- Collections: Movement, Skills, Social, Storage, Workbench
- Strings: Identity, Faction"
```

---

## Step 4 — Smoke test + tricky-case tests

### 4.1 — Create `tests/DualFrontier.Core.Interop.Tests/VanillaComponentRoundTripTests.cs`

```csharp
using DualFrontier.Application.Bootstrap;
using DualFrontier.Components.Building;
using DualFrontier.Components.Combat;
using DualFrontier.Components.Items;
using DualFrontier.Components.Magic;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Components.World;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Enums;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K4 verification: Категория A components survive native bulk roundtrip.
///
/// Smoke test verifies all 24 registered components support add → get
/// roundtrip without data corruption. Tricky-case tests verify specific
/// patterns that posed risk during conversion:
///   - EntityId? nullable fields (JobComponent, BedComponent)
///   - Computed properties (NeedsComponent, RaceComponent)
///   - init-only fields (RaceComponent, GolemBondComponent)
/// </summary>
public class VanillaComponentRoundTripTests
{
    [Fact]
    public void Smoke_AllVanillaComponents_RegisterSuccessfully()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        // Should not throw — all 24 components must register
        var act = () => VanillaComponentRegistration.RegisterAll(registry);
        act.Should().NotThrow();

        // Verify count
        registry.Count.Should().Be(24);
    }

    [Fact]
    public void HealthComponent_RoundTrip_PreservesData()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId entity = world.CreateEntity();
        var original = new HealthComponent { Current = 75f, Maximum = 100f };

        world.AddComponent(entity, original);
        HealthComponent retrieved = world.GetComponent<HealthComponent>(entity);

        retrieved.Current.Should().Be(75f);
        retrieved.Maximum.Should().Be(100f);
        retrieved.IsDead.Should().BeFalse();
    }

    [Fact]
    public void NeedsComponent_RoundTrip_PreservesComputedProperties()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId entity = world.CreateEntity();
        var original = new NeedsComponent
        {
            Satiety = 0.15f,    // below CriticalThreshold (0.2)
            Hydration = 0.5f,
            Sleep = 0.18f,      // below CriticalThreshold
            Comfort = 0.7f,
        };

        world.AddComponent(entity, original);
        NeedsComponent retrieved = world.GetComponent<NeedsComponent>(entity);

        // Data preservation
        retrieved.Satiety.Should().Be(0.15f);
        retrieved.Hydration.Should().Be(0.5f);
        retrieved.Sleep.Should().Be(0.18f);
        retrieved.Comfort.Should().Be(0.7f);

        // Computed properties recompute correctly on retrieved struct
        retrieved.IsHungry.Should().BeTrue();
        retrieved.IsThirsty.Should().BeFalse();
        retrieved.IsExhausted.Should().BeTrue();
    }

    [Fact]
    public void JobComponent_RoundTrip_PreservesNullableEntityId()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId pawn = world.CreateEntity();
        EntityId targetEntity = world.CreateEntity();

        // Test 1: with target
        var jobWithTarget = new JobComponent
        {
            Current = JobKind.Haul,
            Target = targetEntity,
            TicksAtJob = 42,
            IsInterrupted = false,
        };
        world.AddComponent(pawn, jobWithTarget);
        JobComponent retrieved = world.GetComponent<JobComponent>(pawn);

        retrieved.Current.Should().Be(JobKind.Haul);
        retrieved.Target.Should().Be(targetEntity);
        retrieved.TicksAtJob.Should().Be(42);
        retrieved.IsIdle.Should().BeFalse();

        // Test 2: with null target (idle)
        var jobIdle = new JobComponent
        {
            Current = JobKind.Idle,
            Target = null,
            TicksAtJob = 0,
            IsInterrupted = false,
        };
        world.AddComponent(pawn, jobIdle);
        JobComponent idle = world.GetComponent<JobComponent>(pawn);

        idle.Target.Should().BeNull();
        idle.IsIdle.Should().BeTrue();
        idle.NeedsTarget.Should().BeFalse();
    }

    [Fact]
    public void RaceComponent_RoundTrip_PreservesInitOnlyFields()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId entity = world.CreateEntity();
        var original = new RaceComponent
        {
            Kind = RaceKind.Elf,
            HasEtherChannels = true,
            HasIndustrialAffinity = false,
        };

        world.AddComponent(entity, original);
        RaceComponent retrieved = world.GetComponent<RaceComponent>(entity);

        retrieved.Kind.Should().Be(RaceKind.Elf);
        retrieved.HasEtherChannels.Should().BeTrue();
        retrieved.HasIndustrialAffinity.Should().BeFalse();
        retrieved.IsOrganic.Should().BeTrue();      // computed
        retrieved.NeedsFood.Should().BeTrue();      // computed
        retrieved.NeedsSleep.Should().BeTrue();     // computed

        // Verify Golem race (non-organic)
        EntityId golem = world.CreateEntity();
        world.AddComponent(golem, new RaceComponent { Kind = RaceKind.Golem });
        RaceComponent golemRace = world.GetComponent<RaceComponent>(golem);

        golemRace.IsOrganic.Should().BeFalse();
        golemRace.NeedsFood.Should().BeFalse();
        golemRace.NeedsSleep.Should().BeFalse();
    }

    [Fact]
    public void GolemBondComponent_RoundTrip_PreservesInitOnlyEntityId()
    {
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId golem = world.CreateEntity();
        EntityId mage = world.CreateEntity();

        var bond = new GolemBondComponent
        {
            BondedMage = mage,
            Mode = OwnershipMode.Bonded,
            TicksSinceContested = 0,
            BondStrength = 5,
        };

        world.AddComponent(golem, bond);
        GolemBondComponent retrieved = world.GetComponent<GolemBondComponent>(golem);

        retrieved.BondedMage.Should().Be(mage);
        retrieved.Mode.Should().Be(OwnershipMode.Bonded);
        retrieved.BondStrength.Should().Be(5);
    }

    [Fact]
    public void EmptyComponents_RoundTrip_DoNotCorruptStorage()
    {
        // Components с no fields (BiomeComponent, SchoolComponent, AmmoComponent,
        // ShieldComponent, WeaponComponent) must roundtrip без issue
        using var world = Bootstrap.Run();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);
        VanillaComponentRegistration.RegisterAll(registry);

        EntityId entity = world.CreateEntity();

        var act1 = () => world.AddComponent(entity, new BiomeComponent());
        var act2 = () => world.AddComponent(entity, new SchoolComponent());
        var act3 = () => world.AddComponent(entity, new AmmoComponent());

        act1.Should().NotThrow();
        act2.Should().NotThrow();
        act3.Should().NotThrow();

        world.HasComponent<BiomeComponent>(entity).Should().BeTrue();
        world.HasComponent<SchoolComponent>(entity).Should().BeTrue();
        world.HasComponent<AmmoComponent>(entity).Should().BeTrue();
    }
}
```

### 4.2 — Verify test project references

`tests/DualFrontier.Core.Interop.Tests/DualFrontier.Core.Interop.Tests.csproj` needs ProjectReference к `DualFrontier.Components`, `DualFrontier.Application` (для VanillaComponentRegistration). Update if missing:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\DualFrontier.Components\DualFrontier.Components.csproj" />
  <ProjectReference Include="..\..\src\DualFrontier.Application\DualFrontier.Application.csproj" />
</ItemGroup>
```

### 4.3 — Atomic commit

```powershell
git add tests/DualFrontier.Core.Interop.Tests/VanillaComponentRoundTripTests.cs
git add tests/DualFrontier.Core.Interop.Tests/DualFrontier.Core.Interop.Tests.csproj
git commit -m "test(interop): K4 vanilla component roundtrip tests

Adds 7 tests verifying Категория A components survive native bulk roundtrip:

Smoke (1):
- AllVanillaComponents_RegisterSuccessfully: всех 24 components register

Tricky cases (6):
- HealthComponent: basic roundtrip с computed property (IsDead)
- NeedsComponent: 4 floats + 3 computed properties (IsHungry, IsThirsty, IsExhausted)
- JobComponent: nullable EntityId? (с target / без target)
- RaceComponent: init-only fields + 3 computed properties (IsOrganic, NeedsFood, NeedsSleep)
- GolemBondComponent: init-only fields с EntityId? + enum
- EmptyComponents: BiomeComponent / SchoolComponent / AmmoComponent — no fields

Test count: 472 → 479 (+7 K4 tests)."
```

---

## Step 5 — Full verification

```powershell
cd D:\Colony_Simulator\Colony_Simulator

# Native — should be unchanged from K3 baseline
cd native\DualFrontier.Core.Native
cmake --build build --config Release
.\build\Release\df_native_selftest.exe
# Expected: 12 scenarios, ALL PASSED (unchanged)

cd D:\Colony_Simulator\Colony_Simulator
dotnet build
# Expected: 0 errors, 0 warnings

dotnet test
# Expected: 479 passing, 0 failed (472 baseline + 7 K4)
```

If counts differ — investigate. Common issues:
- Component conversion broke a behavior expectation in existing test
- Tricky-case test failure due to subtle struct vs class semantic difference
- Missing project reference in test project

---

## Step 6 — Update MIGRATION_PROGRESS.md

Update `Current state snapshot`:
```markdown
| **Active phase** | K5 (planned) — Span<T> protocol + write command batching |
| **Last completed milestone** | K4 (struct refactor Hybrid path) — `<sha>` 2026-MM-DD |
| **Next milestone (recommended)** | K5 (Span<T> protocol + ArrayPool fix) |
| **Tests passing** | 479 (76 Core + 4 Persistence + 52 Interop + 347 Modding) |
```

Update K4 row in K-series progress table:
```markdown
| K4 | Component struct refactor (Hybrid Path) | DONE | 3-5 hours auto-mode (3-4 days hobby pace) | `<sha>` | 2026-MM-DD |
```

Add detailed K4 entry (after K3):
```markdown
### K4 — Component struct refactor (Hybrid Path)

- **Status**: DONE (`<sha>`, 2026-MM-DD)
- **Brief**: `tools/briefs/K4_STRUCT_REFACTOR_BRIEF.md` (FULL EXECUTED)
- **Architectural decisions implemented** (per 2026-05-07 K4 design discussion):
  - Q1 — **Hybrid Path**: Trivial POCO components → struct (native batching path), components с reference types (Dictionary/List/HashSet/string) stay as class (managed path). Cleanness > expediency.
  - Q2 — **Per-component atomic commits**: 24 components = 24 commits across 7 batches. Bisect-friendly.
  - Q3 — **Explicit registration at Application bootstrap**: VanillaComponentRegistration.RegisterAll(registry) called once. Sequential type IDs 1..24.
  - Q4 — **Smoke + tricky-case tests**: 7 new tests. Existing 472 tests cover behavior через consumer systems.
- **Components converted** (24 in Категория A):
  - Shared (3): Health, Position, Race
  - Pawn trivial (3): Needs, Mind, Job
  - Items (5): Bed, Consumable, DecorativeAura, Reservation, WaterSource
  - World (3): Tile, Biome, EtherNode
  - Magic (4): Ether, GolemBond, Mana, School
  - Combat (4): Ammo, Armor, Shield, Weapon
  - Building trivial (2): PowerConsumer, PowerProducer
- **Components staying as class** (7 in Категории B+C):
  - Collections (5): Movement (List), Skills/Social (Dictionary), Storage (Dictionary+HashSet), Workbench (string?)
  - Strings (2): Identity, Faction
- **VanillaComponentRegistration.cs**: Application/Bootstrap helper, registers all 24 на Application init
- **Test count**: 472 → 479 (+7 K4 roundtrip tests)
- **System code changes**: NONE — existing get/modify/set pattern works со struct copy semantics unchanged
- **Lessons learned**: <fill if anything non-trivial>
```

Атомарный commit:
```powershell
git add docs/MIGRATION_PROGRESS.md
git commit -m "docs(migration): K4 closure recorded"
```

---

## Step 7 — Update K4 brief skeleton

Open `tools/briefs/K4_STRUCT_REFACTOR_BRIEF.md`. Replace TODO list с:

```markdown
## Status: EXECUTED

**Date**: 2026-MM-DD
**Branch**: feat/k4-struct-refactor
**Final commit**: <sha>

Full executable brief authored and executed. See git log on
feat/k4-struct-refactor branch for atomic commit sequence.

See `MIGRATION_PROGRESS.md` for closure record.
```

```powershell
git add tools/briefs/K4_STRUCT_REFACTOR_BRIEF.md
git commit -m "docs(briefs): K4 brief skeleton marked EXECUTED"
```

---

## Step 8 — Final verification & merge prep

```powershell
git log --oneline main..HEAD
# Expected sequence (~30 commits):
#   <sha> docs(briefs): K4 brief skeleton marked EXECUTED
#   <sha> docs(migration): K4 closure recorded
#   <sha> test(interop): K4 vanilla component roundtrip tests
#   <sha> refactor(components): VanillaComponentRegistration bootstrap helper
#   ... 24 component conversion commits ...
#   <sha> docs(briefs): K4 brief authored — full executable struct refactor   ← Step 0

git status                                  # clean

# Final builds
cmake --build native\DualFrontier.Core.Native\build --config Release
.\native\DualFrontier.Core.Native\build\Release\df_native_selftest.exe
# Expected: 12 scenarios ALL PASSED (unchanged)

dotnet build
dotnet test
# Expected: 479 passing
```

### Push

```powershell
git push -u origin feat/k4-struct-refactor
```

---

## Acceptance criteria

K4 закрыт когда ВСЕ выполнено:

- [ ] Step 0 brief authoring commit на main выполнен
- [ ] Branch `feat/k4-struct-refactor` создан от `main`
- [ ] 24 component conversion commits (per Категория A inventory)
- [ ] VanillaComponentRegistration.cs commit
- [ ] VanillaComponentRoundTripTests.cs commit (7 tests)
- [ ] `dotnet build` clean — 0 errors
- [ ] Native selftest: **12 scenarios ALL PASSED** (unchanged from K3)
- [ ] `dotnet test`: **479 passing** (472 baseline + 7 K4)
- [ ] MIGRATION_PROGRESS.md K4 row DONE с commit SHA
- [ ] tools/briefs/K4_STRUCT_REFACTOR_BRIEF.md marked EXECUTED
- [ ] Branch pushed to origin
- [ ] No build artifacts committed
- [ ] Q1-Q4 architectural decisions implemented exactly as specified

---

## Rollback procedure

K4 не делает destructive changes:

```powershell
git checkout main
git branch -D feat/k4-struct-refactor
# Step 0 brief authoring commit остаётся на main (durable artifact)
```

If partial conversion completed and must rollback selectively:
```powershell
# Revert specific component conversion commit:
git revert <commit-sha>
```

---

## Open issues / lessons learned (заполнить при closure)

<empty — заполнить если что-то нетривиальное всплыло>

---

## Pipeline metadata

- **Brief authored by**: Opus (architect)
- **Brief executed by**: Claude Code agent or human
- **Final review**: Crystalka (architectural judgment + commit author)
- **Methodology compliance**:
  - METHODOLOGY.md v1.4 «calibrated time estimates» applied (3-5 hours auto-mode / 3-4 days hobby pace)
  - METHODOLOGY.md v1.3 «Step 0 brief authoring» applied
  - METHODOLOGY.md v1.3 «throw inventory» — zero new throws (purely managed refactor)
  - METHODOLOGY.md v1.2 «descriptive pre-flight» applied (8 hard gates + 3 informational)
- **Architectural decisions traceability**: Q1-Q4 explicit, derived от 2026-05-07 K4 design discussion, recorded в MIGRATION_PROGRESS K4 entry
- **Performance evidence**: PERFORMANCE_REPORT_K3.md Measurement 2 validated K1 bulk gain (3.6x speedup), motivating K4 marathon

**Brief end. Companion docs: KERNEL_ARCHITECTURE.md (§K4), MIGRATION_PROGRESS.md, METHODOLOGY.md v1.4, PERFORMANCE_REPORT_K3.md.**
