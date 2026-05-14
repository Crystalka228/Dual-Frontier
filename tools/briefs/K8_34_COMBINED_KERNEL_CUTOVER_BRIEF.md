---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_34_COMBINED
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_34_COMBINED
supersedes:
  - DOC-D-K8_3                      # K8.3 v2.0 brief — system migration scope absorbed
  - DOC-D-K8_3_BRIEF_REFRESH_PATCH  # K8.3 patch — swap rationale resolved by combine
  - DOC-D-K8_4                      # K8.4 skeleton — storage migration scope absorbed
---
# K8.3+K8.4 — Combined Kernel Cutover Brief

**Status**: AUTHORED 2026-05-13 (combined milestone authoring per Crystalka direction same session as K8.3 v2.0 halt resolution).
**Scope**: Single atomic milestone executing storage migration (K8.4 original scope) + system migration to SpanLease/WriteBatch (K8.3 original scope) + Mod API v3 ship + managed `World` retirement + orphan `.uid` cleanup + manifestVersion field.
**Authority**: This brief is the **single execution artifact** for milestone A'.5 under combined approach. K8.3 v2.0 brief + K8.3 patch + K8.4 skeleton lifecycle transition AUTHORED → SUPERSEDED at this brief's execution closure; they remain on disk as historical artifacts (per K9 brief refresh patch precedent of preservation).
**Milestone**: A'.5 (combined; absorbs old A'.5 K8.3 + old A'.6 K8.4 + portions of old A'.7 K8.5 World retirement). Phase A' sequencing post-combined: A'.5 = this milestone, A'.6 = K8.5 ecosystem prep (unchanged from original Migration Plan §1.4 scope), A'.7 removed (no longer needed), A'.8 = K-closure report, A'.9 = analyzer milestone.
**Approach**: α hard cutover. No backward compatibility. No legacy API survival. No deprecation warnings. v1/v2 IModApi deleted entirely; only v3 surface ships. v1/v2 manifests rejected entirely; only `manifestVersion: "3"` accepted.

---

## ⚠ READ ORDER

This is a single self-contained brief. Read straight through. Three superseded artifacts (`K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` v2.0, `K8_3_BRIEF_REFRESH_PATCH.md`, `K8_4_MANAGED_WORLD_RETIRED_BRIEF.md`) preserved on disk for historical traceability — **do not execute against them**; their decisions are absorbed verbatim into this combined brief.

If at any point during execution this brief proves insufficient for mechanical execution, invoke METHODOLOGY §3 «stop, escalate, lock». Do not improvise. The expected outcome from combined approach is **zero halts during execution** — premise misses that triggered K8.3 v2.0 halt are structurally prevented by atomic execution.

---

## §0 — Q-COMBINED locks (ratified during deliberation 2026-05-13)

### §0.1 Q-COMBINED-1 — Scope consolidation

**Lock**: Combined K8.3+K8.4 milestone scope = Track A (storage migration) + Track B (system migration) + Track C (verification + governance), atomically executed.

**Track A** (storage layer):
- Component storage migration: managed `World` → `NativeWorld` for all production components used by `GameBootstrap.coreSystems`
- `ComponentTypeRegistry` activation on bootstrap path (replace FNV-1a fallback)
- `Bootstrap.Run(registry)` wiring in `GameBootstrap.CreateLoop` (replace `new NativeWorld()`)
- Production wiring switch: `RandomPawnFactory` and `ItemFactory` use `NativeWorld.AddComponents<T>` bulk API
- Managed `World` class deletion as production path (moved to test project as `ManagedTestWorld` per K-L11)

**Track B** (systems layer):
- 12 production systems in `GameBootstrap.coreSystems` migrated to `SpanLease<T>` reads + `WriteBatch<T>` writes
- Per-system `SystemExecutionContext` access pattern refactored
- `SystemBase.GetComponent<T>` / `SetComponent<T>` / `Query<T>` / `Query<T1,T2>` deleted entirely (no facade, no [Obsolete])
- Per-system test class (K8.2 precedent)

**Track C** (Mod API v3 surface + governance):
- `IModApi` v3 ships: `RegisterComponent<T> where T : unmanaged, IComponent` (Path α), `RegisterManagedComponent<T> where T : class, IComponent` (Path β plumbing), `Fields` + `ComputePipelines` (v1.6 additions preserved)
- v1/v2 IModApi deleted entirely
- `RestrictedModApi.ManagedStore<T>` per-mod storage implementation
- `[ManagedStorage]` attribute creation
- `ValidationErrorKind.MissingManagedStorageAttribute` enum addition
- `SystemBase.ManagedStore<T>()` accessor (parallel to existing `SystemBase.NativeWorld`)
- `ModManifest.ManifestVersion` field (O-1 audit inline)
- ManifestParser accepts only `manifestVersion: "3"` (no v1/v2 fallback)
- Orphan `.uid` cleanup (3 files: ShieldSystem, SocialSystem, BiomeSystem)
- CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT effectiveness verification (first post-register milestone)
- CAPA-2026-05-13-K8.3-PREMISE-MISS effectiveness verification (combined approach resolves premise structurally)
- Documentation amendments: MIGRATION_PLAN v1.1→v1.2, KERNEL v1.5→v1.6, MOD_OS v1.7→v1.8, PHASE_A_PRIME_SEQUENCING structural amendment, MIGRATION_PROGRESS closure entry

**Out of scope** (deferred to K8.5 / A'.6 or later):
- `[ModAccessible]` annotation completeness pass — K8.5 audit
- Mod ecosystem migration guide — K8.5 deliverable
- Analyzer preparation work — A'.9
- Vanilla mod skeletons population — Phase B M-series
- Save system integration — post-G-series
- Native code changes — none required, C ABI complete at K2-K5 era

### §0.2 Q-COMBINED-2 — α hard cutover decision

**Lock**: α hard cutover applied. Mechanics:
- `World` class moved to `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs`
- `SystemBase.GetComponent/SetComponent/Query/Query<T1,T2>` deleted (no facade, no [Obsolete])
- `SystemExecutionContext` constructor refactored: `World world` parameter removed, `NativeWorld nativeWorld` required, `ComponentTypeRegistry registry` new required, `IManagedStorageResolver? managedStorageResolver` new optional
- Isolation guard preserves `HashSet<Type>` semantics (type-id migration deferred to A'.9 analyzer milestone if profile demands)
- Production wiring (`GameBootstrap.CreateLoop`, factories) uses `Bootstrap.Run(registry)` instead of `new NativeWorld()`
- **Dual-write scaffolding allowed inside milestone execution** (Phase 2 commit 2 through Phase 5 commit 20) — execution-time architectural device, not cross-milestone debt; eliminated atomically Phase 5 commit 20

**Rationale**: No backward compatibility (Crystalka direction 2026-05-13 «мы не будем поддерживать старый api так что удаление и новый»). Combined milestone is the architectural break point. Mods written against v2 API will not compile post-K8.3+K8.4. Tests either migrate to `ManagedTestWorld` (legacy `Core.Tests` fixture) or to `NativeWorld.AcquireSpan` (new `Systems.Tests` pattern). No deprecation period; no `[Obsolete]` annotations; no compile warnings about legacy usage. Clean break.

### §0.3 Q-COMBINED-3 — IModApi v3 surface specification

**Lock**: IModApi v3 surface ships as **sole** IModApi interface. v1/v2 surface deleted entirely.

```csharp
public interface IModApi
{
    // ── Path α component registration (default per K-L3) ────────────────
    void RegisterComponent<T>() where T : unmanaged, IComponent;
    
    // ── Path β component registration (K-L3.1 bridge) ───────────────────
    void RegisterManagedComponent<T>() where T : class, IComponent;
    
    // ── System registration (constraint preserved) ──────────────────────
    void RegisterSystem<T>() where T : class;
    
    // ── Bus operations (v2 semantics preserved) ─────────────────────────
    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(Action<T> handler) where T : IEvent;
    
    // ── Inter-mod contracts (v2 semantics preserved) ────────────────────
    void PublishContract<T>(T contract) where T : IModContract;
    bool TryGetContract<T>(out T? contract) where T : class, IModContract;
    
    // ── Self-introspection (v2 surface preserved) ───────────────────────
    IReadOnlySet<string> GetKernelCapabilities();
    ModManifest GetOwnManifest();
    void Log(ModLogLevel level, string message);
    
    // ── v1.6 additions preserved (Fields, ComputePipelines) ─────────────
    IModFieldApi? Fields { get; }
    IModComputePipelineApi? ComputePipelines { get; }
}
```

Supporting types:
- `[ManagedStorage]` attribute on Path β class component types
- `ManagedStore<T>` data structure: `Dictionary<EntityId, T>` BCL (single-threaded, runtime-only per Q4.b)
- `SystemBase.ManagedStore<T>()` accessor via `IManagedStorageResolver` indirection
- `ModRegistry` implements `IManagedStorageResolver`
- `ValidationErrorKind.MissingManagedStorageAttribute` new enum member

### §0.4 Q-COMBINED-4 — Atomic commit shape

**Lock**: 24 atomic commits in 6 phases. Rule-based system ordering (Tier 1-5 by interleave risk). Dual-write bridge in Phase 2 (execution-time scaffolding, eliminated Phase 5).

Commit log:
- **Phase 0**: Commit 1 — brief authoring commit (this brief on disk before any execution)
- **Phase 1**: Commit 2 — orphan `.uid` cleanup (3 files)
- **Phase 2** (storage foundation): Commits 3-4
  - Commit 3: `ComponentTypeRegistry` activation + `Bootstrap.Run` wiring in `GameBootstrap` + factory dual-write switch + storage migration tests
  - Commit 4: Mass component type registration (atomic compilable unit per K8.1 lesson)
- **Phase 3** (IModApi v3 surface): Commits 5-8
  - Commit 5: `IModApi` v3 interface (delete v2, ship v3 with constraint split)
  - Commit 6: `[ManagedStorage]` attribute + `ManagedStore<T>` + `ValidationErrorKind` enum addition
  - Commit 7: `RestrictedModApi` v3 implementation + `IManagedStorageResolver` wiring + `ModRegistry` resolver implementation
  - Commit 8: `ModManifest.ManifestVersion` field + `ManifestParser` v3-only acceptance + vanilla mod manifest updates
- **Phase 4** (per-system migration): Commits 9-20 (12 systems, one per commit, Tier 1→5 order)
  - Commit 9:  ConsumeSystem → SpanLease/WriteBatch (Tier 1)
  - Commit 10: SleepSystem → SpanLease/WriteBatch (Tier 1)
  - Commit 11: ComfortAuraSystem → SpanLease/WriteBatch (Tier 1)
  - Commit 12: MoodSystem → SpanLease/WriteBatch (Tier 2)
  - Commit 13: PawnStateReporterSystem → SpanLease/WriteBatch (Tier 2)
  - Commit 14: JobSystem → SpanLease/WriteBatch (Tier 3)
  - Commit 15: HaulSystem → SpanLease/WriteBatch (Tier 3)
  - Commit 16: NeedsSystem → SpanLease/WriteBatch (Tier 3)
  - Commit 17: ConverterSystem → SpanLease/WriteBatch (Tier 4)
  - Commit 18: ElectricGridSystem → SpanLease/WriteBatch (Tier 4)
  - Commit 19: InventorySystem → SpanLease/WriteBatch (Tier 4)
  - Commit 20: MovementSystem → SpanLease/WriteBatch (Tier 5)
- **Phase 5** (World retirement): Commits 21-22
  - Commit 21: `SystemBase.GetComponent/SetComponent/Query/Query<T1,T2>` deleted + `SystemExecutionContext` refactored + `ParallelSystemScheduler` signature updated + factories' managed-side dual-write removed
  - Commit 22: `World.cs` moved to `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs` + `ComponentStore.cs` moved + `InternalsVisibleTo` grants updated + 472+ legacy `Core.Tests` rename
- **Phase 6** (closure): Commits 23-24
  - Commit 23: Documentation amendments (MIGRATION_PLAN v1.2 + KERNEL v1.6 + MOD_OS v1.8 + PHASE_A_PRIME_SEQUENCING structural amendment)
  - Commit 24: MIGRATION_PROGRESS closure entry + REGISTER.yaml updates (lifecycle transitions + version bumps + REQ promotions + CAPA closures + audit trail entry) + optional bootstrap backfill per FRAMEWORK §8.3

**Total**: 24 atomic commits

### §0.5 Q-COMBINED-5 — Manifest version strict gating

**Lock**: `ManifestParser` accepts only `manifestVersion: "3"`. Any other value or missing field → hard reject with `ValidationErrorKind.IncompatibleContractsVersion`. No deprecation warnings; no v1/v2 acceptance. Vanilla mod manifests (7 skeletons) gain `"manifestVersion": "3"` mandatory in Phase 3 Commit 8.

### §0.6 Q-COMBINED-6 — Test strategy

**Lock**:
- Per-system test class (K8.2 precedent)
- `NativeWorldTestFixture` shared fixture in `DualFrontier.Systems.Tests`
- `ManagedTestWorld` migration of `World.cs` to `DualFrontier.Core.Tests/Fixtures/`
- IModApi v3 surface test class new (`DualFrontier.Modding.Tests/IModApiV3Tests.cs`)
- Manifest version test class new (`DualFrontier.Modding.Tests/ManifestVersionTests.cs`)
- Test count delta: +50-90 (post-combined baseline ~680-790)

### §0.7 Q-COMBINED-7 — Closure protocol

**Lock**: Full METHODOLOGY §12.7 closure protocol exercised. Specific updates enumerated in §9 of this brief. CAPA-2026-05-12 and CAPA-2026-05-13 both close at this milestone. REQ-K-L3, REQ-K-L4, REQ-K-L8, REQ-K-L11, REQ-Q-A45-X5 promote PENDING → VERIFIED.

### §0.8 Q-COMBINED-8 — Brief structure

**Lock**: Brief filename `K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md`. Concern-axis organization (§2 storage, §3 systems, §4 Mod API, §5 manifest, §6 phases) complements Phase 0-N execution scaffold. ~1800-2200 lines estimated. Supersedes K8_3 + K8_3_PATCH + K8_4 briefs (preserved as SUPERSEDED artifacts at execution closure).

---

## §1 — Goal and scope summary

**Goal**: Complete the Path α (kernel-side NativeWorld) production storage backbone migration and the 12-system access pattern migration to `SpanLease<T>` / `WriteBatch<T>` in a single atomic milestone. Ship `IModApi` v3 surface as the sole IModApi. Retire managed `World` from production code paths. Establish first post-A'.4.5 closure protocol exercise.

**Architectural outcome**:
- `NativeWorld` is the single production storage path (K-L11 fully realized)
- `ComponentTypeRegistry` is active on bootstrap path (K-L4 fully realized)
- All 12 `coreSystems` use `SpanLease<T>` reads + `WriteBatch<T>` writes (K-L7 fully realized)
- `IModApi` v3 is the sole modding surface (K-L3 + K-L3.1 fully realized via Path α default + Path β opt-in)
- Managed `World` exists only as test fixture (`ManagedTestWorld` in `DualFrontier.Core.Tests/Fixtures/`)
- Combined milestone is first post-register milestone closure (Q-A45-X5 verified)

**Falsifiable claim**: post-combined-closure, no production code path constructs `World` directly. Grep `src/` for `new World()` returns zero results outside test directories. Grep `src/` for `SystemBase.GetComponent` / `SetComponent` / `Query` returns zero results. The architectural goal is structurally verifiable, not procedurally asserted.

---

## §2 — Architectural concern: Storage layer

### §2.1 — NativeWorld activation (Bootstrap.Run + registry)

**Current state (HEAD = `efd67df`)** — verified via `src/DualFrontier.Application/Loop/GameBootstrap.cs` lines 85-96:

```csharp
public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")
{
    var world        = new World();
    // K8.2 v2 — NativeWorld owned alongside the managed World until K8.4
    // retires the latter. Production uses NativeWorld only for K8.1
    // primitives (string interning, native maps/sets/composites bound to
    // struct components); component storage stays on the managed World
    // through K8.3. Bootstrap path uses FNV-1a fallback type ids; the
    // registry-based path lights up at K8.4 when component storage moves.
    var nativeWorld  = new NativeWorld();
    // ... (existing wiring) ...
}
```

**Target state (post-combined)**:

```csharp
public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")
{
    // Registry-based component type ids active per K-L4.
    var registry = new ComponentTypeRegistry();
    
    // NativeWorld constructed via K3 bootstrap graph (parallel allocation +
    // thread pool init). The registry is passed at construction so type ids
    // are deterministic from first AddComponent call. Replaces both
    // `new World()` and `new NativeWorld()` previously constructed separately.
    var nativeWorld = Bootstrap.Run(registry);
    
    // Pre-register all production component types so factory bulk-add calls
    // (next phase) don't perform per-call type registration in the hot loop.
    // Idempotent — if already registered (e.g. by Bootstrap.Run itself),
    // Register<T>() returns existing id without re-calling native.
    RegisterProductionComponentTypes(registry);
    
    // ... (rest of wiring; factories now consume nativeWorld instead of world) ...
}

private static void RegisterProductionComponentTypes(ComponentTypeRegistry registry)
{
    // Tier 1 (factory-touched, single-system consumed)
    registry.Register<ConsumableComponent>();
    registry.Register<WaterSourceComponent>();
    registry.Register<BedComponent>();
    registry.Register<DecorativeAuraComponent>();
    
    // Tier 2-3 (multi-system, no K8.1 primitives)
    registry.Register<NeedsComponent>();
    registry.Register<JobComponent>();
    registry.Register<ReservationComponent>();
    registry.Register<MindComponent>();
    registry.Register<PowerConsumerComponent>();
    registry.Register<PowerProducerComponent>();
    
    // Tier 4 (K8.1 primitive heavy)
    registry.Register<StorageComponent>();      // NativeMap + NativeSet
    registry.Register<MovementComponent>();     // NativeComposite (path)
    
    // Tier 5 (cross-slice spatial)
    registry.Register<PositionComponent>();
    
    // Factory-touched, not system-touched (informational identity carriers)
    registry.Register<IdentityComponent>();
    registry.Register<SkillsComponent>();       // NativeMap×2
    
    // Phase 0 verification step: confirm count matches expected inventory
    // before proceeding to Phase 1.
}
```

**Migration constraints**:
- Registry construction precedes any component-type usage.
- `Bootstrap.Run(registry)` replaces both `new World()` and `new NativeWorld()`. There is no managed `World` in production after Phase 5 commit 21.
- Component-type registration in `RegisterProductionComponentTypes` is **explicit and auditable** (K-L4 fully realized). No reflection-based scan. No implicit registration on first add.
- The list is verified against `RandomPawnFactory` + `ItemFactory` + 12 system `[SystemAccess]` declarations at Phase 0 of execution. Count drift between this brief authoring and execution time surfaces as either added components (new entries to add to list) or removed components (entries to delete from list) — record divergence, do not stop.

**Implementation notes**:
- `ComponentTypeRegistry.Register<T>` already exists (K2 era); idempotent re-registration verified.
- `Bootstrap.Run(registry)` already exists (K3 era); registry parameter optional but used here as required.
- `NativeWorld.AdoptBootstrappedHandle` already wired to support registry-bound construction.

### §2.2 — Component storage migration mechanics

**Per-component migration pattern**:

For each component type `T`:
1. Register `T` via `registry.Register<T>()` (pre-flight phase)
2. Factories switch from `world.AddComponent<T>(entity, value)` (managed) to dual-write: both `world.AddComponent<T>(entity, value)` AND `nativeWorld.AddComponent<T>(entity, value)` during Phase 2-4
3. Per-system migration in Phase 4: system reads via `NativeWorld.AcquireSpan<T>()` and writes via `NativeWorld.BeginBatch<T>()`
4. Phase 5 commit 21: factories' managed-side write removed (now native-only)
5. Phase 5 commit 22: managed `World` physically moved to test project

**Dual-write rationale**:
- Phase 2 commit 3 introduces dual-write: factories write to both managed `World` (so unmigrated systems still read it) and `NativeWorld` (so migrated systems read it)
- Each Phase 4 per-system commit (9-20) migrates one system to read from `NativeWorld`; managed `World` no longer read by that system after its commit
- Between commit 9 (first system migrated) and commit 21 (last system migration + managed write removal), managed `World` has progressively fewer readers
- Phase 5 commit 21 removes managed-side write (factories now native-only); no system reads managed `World` anymore; class is dead but still loaded
- Phase 5 commit 22 physically moves the class file

This sequencing satisfies the K8.1 lesson «atomic commit as compilable unit»: every commit leaves the project in a working state. No commit produces broken builds or temporary stubs.

**Factory bulk-write pattern**:

Current `RandomPawnFactory.Spawn` writes per-entity:
```csharp
for (int i = 0; i < count; i++)
{
    EntityId entity = world.CreateEntity();
    world.AddComponent<PositionComponent>(entity, new PositionComponent { Position = ... });
    world.AddComponent<IdentityComponent>(entity, new IdentityComponent { ... });
    world.AddComponent<NeedsComponent>(entity, new NeedsComponent { ... });
    // ... per-component per-entity ...
}
```

Post-combined `RandomPawnFactory.Spawn` uses bulk operations:
```csharp
public IReadOnlyList<EntityId> Spawn(NativeWorld nativeWorld, World world, IGameServices services, int count)
{
    // Allocate entity ids first (cheap, native counter)
    var entities = new EntityId[count];
    for (int i = 0; i < count; i++)
        entities[i] = nativeWorld.CreateEntity();
    
    // Generate per-component arrays
    var positions = new PositionComponent[count];
    var identities = new IdentityComponent[count];
    var needs = new NeedsComponent[count];
    var skills = new SkillsComponent[count];
    // ... per type ...
    
    for (int i = 0; i < count; i++)
    {
        positions[i] = new PositionComponent { Position = GeneratePosition(i) };
        identities[i] = new IdentityComponent { Name = nativeWorld.InternString(GenerateName(i)) };
        needs[i] = new NeedsComponent { Satiety = 1f, Hydration = 1f, Sleep = 1f, Comfort = 1f };
        skills[i] = GenerateSkills(nativeWorld, i);
    }
    
    // Bulk add — single P/Invoke per type, regardless of count
    nativeWorld.AddComponents<PositionComponent>(entities, positions);
    nativeWorld.AddComponents<IdentityComponent>(entities, identities);
    nativeWorld.AddComponents<NeedsComponent>(entities, needs);
    nativeWorld.AddComponents<SkillsComponent>(entities, skills);
    
    // Dual-write to managed World during transition (Phase 2-4)
    // Removed in Phase 5 commit 21
    for (int i = 0; i < count; i++)
    {
        world.AddComponent<PositionComponent>(entities[i], positions[i]);
        world.AddComponent<IdentityComponent>(entities[i], identities[i]);
        world.AddComponent<NeedsComponent>(entities[i], needs[i]);
        world.AddComponent<SkillsComponent>(entities[i], skills[i]);
    }
    
    return entities;
}
```

After Phase 5 commit 21, the dual-write block is deleted. Factory becomes native-only.

**Performance note**: bulk operations from K1 era (P/Invoke per type, not per entity) finally light up in production. Expected performance improvement on factory hot path: factor of count (50 pawns → 50× fewer P/Invokes for that component type). Empirical verification belongs to A'.8 K-closure report.

### §2.3 — Dual-write phase mechanics

**Phase 2 commit 3 introduces dual-write**:

`RandomPawnFactory.Spawn` and `ItemFactory.Spawn` write to both stores. The `GameBootstrap.CreateLoop` passes both `world` and `nativeWorld` to factories during transition:

```csharp
// GameBootstrap.CreateLoop, Phase 2 commit 3 transitional state
var registry = new ComponentTypeRegistry();
var nativeWorld = Bootstrap.Run(registry);
RegisterProductionComponentTypes(registry);

var world = new World();  // managed; retained during transition

// Factories receive both stores
var pawnFactory = new RandomPawnFactory(FactorySeed, navGrid, MapWidth, MapHeight, nativeWorld);
IReadOnlyList<EntityId> pawnIds = pawnFactory.Spawn(nativeWorld, world, services, InitialPawnCount);
```

Factories' `Spawn` method signature gains `World world` parameter (alongside existing `NativeWorld nativeWorld`). Dual-write loop writes to both.

**Phase 4 per-system migrations**: each system commit removes its dependency on managed `World` (it now reads `NativeWorld.AcquireSpan<T>`). Managed `World` is **never deleted** during Phase 4 — only readers progressively switch.

**Phase 5 commit 21 removes dual-write**:

```csharp
// GameBootstrap.CreateLoop, Phase 5 commit 21 final state
var registry = new ComponentTypeRegistry();
var nativeWorld = Bootstrap.Run(registry);
RegisterProductionComponentTypes(registry);

// No managed World construction. world variable deleted.

var pawnFactory = new RandomPawnFactory(FactorySeed, navGrid, MapWidth, MapHeight, nativeWorld);
IReadOnlyList<EntityId> pawnIds = pawnFactory.Spawn(nativeWorld, services, InitialPawnCount);
// ↑ factory signature reverts to nativeWorld-only (the World parameter deleted)
```

Factories' `Spawn` method signature reverts to `NativeWorld nativeWorld` only. Dual-write loop deleted. Factories become native-only.

**Critical invariant**: every commit between Phase 2 commit 3 and Phase 5 commit 21 must build cleanly AND pass all tests. The compilable-unit rule from K8.1 lesson applies at milestone scope: dual-write is not a temporary stub (which would violate the lesson) — it is **legitimate intermediate state** because both writes serve real consumers (managed `World` serves unmigrated systems; `NativeWorld` serves migrated systems).

After Phase 5 commit 21, managed `World` has no readers (legacy `SystemBase` API deleted) and no writers (dual-write removed). It exists as dead code on disk. Phase 5 commit 22 physically removes the dead code.

### §2.4 — World class movement to test project

**Phase 5 commit 22 mechanics**:

1. Create `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs`. Content is verbatim copy of `src/DualFrontier.Core/ECS/World.cs` with:
   - Visibility: `internal sealed class World` → `public sealed class ManagedTestWorld`
   - Namespace: `DualFrontier.Core.ECS` → `DualFrontier.Core.Tests.Fixtures`
   - XML doc: updated to reflect K-L11 «test fixture and research reference» role
2. Create `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestComponentStore.cs`. Content is verbatim copy of `src/DualFrontier.Core/ECS/ComponentStore.cs` with same visibility/namespace adjustments.
3. Delete `src/DualFrontier.Core/ECS/World.cs` and `.uid` companion.
4. Delete `src/DualFrontier.Core/ECS/ComponentStore.cs` and `.uid` companion.
5. Update `InternalsVisibleTo` grants:
   - `DualFrontier.Core` no longer needs `InternalsVisibleTo("DualFrontier.Core.Tests")` for `World` access (the class is now in the test project itself).
   - Verify no other `InternalsVisibleTo` grants depended on `World` exposure — if any production assembly still has such grant for `World`, the grant is dead.
6. Update 472+ legacy test files in `DualFrontier.Core.Tests` to reference `ManagedTestWorld` instead of `World`:
   - Global find-replace: `new World()` → `new ManagedTestWorld()`
   - Type references: `World` (when unqualified in test code) → `ManagedTestWorld`
   - Where tests use `World` as parameter type for SUT, update to `ManagedTestWorld`
   - This rename is mechanical and large-scale (~50-100 test files affected). Execution agent performs via `Get-ChildItem -Recurse | Select-String -Pattern '\bWorld\b'` then targeted replacements.
7. Verify production build is clean: `dotnet build src/DualFrontier.sln` succeeds without referencing `World` anywhere.
8. Verify test build is clean: `dotnet test tests/DualFrontier.Core.Tests` succeeds with `ManagedTestWorld` references resolved.

**Edge case**: any test that exercises the **interaction** between `SystemBase` and managed `World` is broken by combined milestone (because `SystemBase` no longer has managed-`World` access methods). Such tests either:
- (a) Rewrite to use `NativeWorld` access patterns (preferred — exercises post-cutover code path)
- (b) Delete the test if it tested behavior that no longer exists (legitimate — the behavior was wrong architecturally)

Execution agent identifies such tests during Phase 5 commit 22 and chooses per-test, recording rationale in commit message. If volume exceeds ~10 such tests, escalate per METHODOLOGY §3 — the threshold suggests architectural concern not anticipated by this brief.

---

## §3 — Architectural concern: System layer

### §3.1 — SystemBase API surface refactor

**Deletions** (Phase 5 commit 21):
- `SystemBase.GetComponent<T>(EntityId id) where T : IComponent`
- `SystemBase.SetComponent<T>(EntityId id, T value) where T : IComponent`
- `SystemBase.Query<T>() where T : IComponent`
- `SystemBase.Query<T1,T2>() where T1 : IComponent where T2 : IComponent`
- `SystemBase.GetSystem<TSystem>() where TSystem : SystemBase` (deleted — was always-throws stub per K-L9 isolation enforcement; no real consumer in production after combined milestone since systems communicate via events)

**Preservations**:
- `SystemBase.Context` property (set by scheduler)
- `SystemBase.OnInitialize()` / `OnDispose()` virtual hooks
- `SystemBase.Update(float delta)` abstract
- `SystemBase.Services` accessor (event bus aggregator)
- `SystemBase.NativeWorld` accessor (K8.2 v2 era; now sole storage access)

**Additions**:
- `SystemBase.ManagedStore<T>() where T : class, IComponent` accessor (Q-COMBINED-3 lock)

**Final `SystemBase` shape post-combined**:

```csharp
public abstract class SystemBase
{
    public SystemExecutionContext Context { get; internal set; } = null!;
    
    protected virtual void OnInitialize() { }
    public abstract void Update(float delta);
    protected virtual void OnDispose() { }
    
    internal void Initialize() => OnInitialize();
    internal void Dispose() => OnDispose();
    
    /// <summary>
    /// Domain-bus aggregator supplied by the scheduler. Use for publishing
    /// events (Services.Pawns.Publish(...)) and subscribing in OnInitialize.
    /// </summary>
    protected IGameServices Services
    {
        get
        {
            var ctx = SystemExecutionContext.Current
                ?? throw new InvalidOperationException(
                    "SystemBase.Services accessed outside an active scheduler context.");
            return ctx.Services
                ?? throw new InvalidOperationException(
                    "SystemBase.Services requested but the scheduler did not supply an IGameServices instance.");
        }
    }
    
    /// <summary>
    /// Native world handle for Path α component access (default per K-L3).
    /// Use AcquireSpan&lt;T&gt;() for read iteration, BeginBatch&lt;T&gt;() for writes.
    /// </summary>
    protected NativeWorld NativeWorld
    {
        get
        {
            var ctx = SystemExecutionContext.Current
                ?? throw new InvalidOperationException(
                    "SystemBase.NativeWorld accessed outside an active scheduler context.");
            return ctx.NativeWorld
                ?? throw new InvalidOperationException(
                    "SystemBase.NativeWorld requested but the scheduler did not supply a NativeWorld instance.");
        }
    }
    
    /// <summary>
    /// Path β access (K-L3.1 bridge). Returns per-mod ManagedStore&lt;T&gt; for the
    /// currently executing system's owning mod. Returns null if T not registered
    /// by this mod, or if system is Core origin.
    /// </summary>
    protected ManagedStore<T>? ManagedStore<T>() where T : class, IComponent
    {
        var ctx = SystemExecutionContext.Current
            ?? throw new InvalidOperationException(
                "SystemBase.ManagedStore called outside an active scheduler context.");
        return ctx.ResolveManagedStore<T>();
    }
}
```

Note absent: no `GetComponent`, `SetComponent`, `Query`, `Query<T1,T2>`, `GetSystem`. These methods do not exist post-combined. System bodies that previously called them must rewrite (Phase 4 per-system commits 9-20).

### §3.2 — SystemExecutionContext refactor

**Current ctor signature**:
```csharp
internal SystemExecutionContext(
    World world,
    string systemName,
    IEnumerable<Type> allowedReads,
    IEnumerable<Type> allowedWrites,
    IEnumerable<string> allowedBuses,
    SystemOrigin origin,
    string? modId,
    IModFaultSink faultSink,
    IGameServices? services = null,
    NativeWorld? nativeWorld = null)
```

**Post-combined ctor signature**:
```csharp
internal SystemExecutionContext(
    NativeWorld nativeWorld,                              // ← was optional, now required
    ComponentTypeRegistry registry,                       // ← NEW: required for type-id lookups in isolation guard
    string systemName,
    IEnumerable<Type> allowedReads,
    IEnumerable<Type> allowedWrites,
    IEnumerable<string> allowedBuses,
    SystemOrigin origin,
    string? modId,
    IModFaultSink faultSink,
    IGameServices? services = null,
    IManagedStorageResolver? managedStorageResolver = null) // ← NEW: optional, null for Core systems
{
    _nativeWorld = nativeWorld ?? throw new ArgumentNullException(nameof(nativeWorld));
    _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    // ... existing initialization preserved (allowedReads/Writes/Buses, origin, modId, faultSink, services) ...
    _managedStorageResolver = managedStorageResolver;
    
    // World parameter deleted — no _world field
}
```

**Method deletions** (parallel to SystemBase deletions):
- `GetComponent<T>` — deleted; systems now use `NativeWorld.AcquireSpan<T>` or `NativeWorld.GetComponent<T>` (single-entity read still available but discouraged for hot path)
- `SetComponent<T>` — deleted; systems now use `NativeWorld.BeginBatch<T>().Update(entity, value)` or `NativeWorld.AddComponent<T>` (single-entity write)
- `Query<T>` — deleted; systems iterate via `AcquireSpan<T>`
- `Query<T1,T2>` — deleted; systems acquire two spans and join via entity id intersection (pattern documented in §3.3)
- `GetSystem<TSystem>` — deleted; was always-throws, no legitimate use case remains

**Method additions**:
- `ResolveManagedStore<T>() where T : class, IComponent` — returns `ManagedStore<T>?` via `_managedStorageResolver?.Resolve<T>(_modId)`; returns null if `_modId` is null (Core system) or resolver is null

**Method preservations**:
- `Current` (ThreadLocal accessor) — preserved
- `PushContext` / `PopContext` (scheduler-side) — preserved
- `SystemName` accessor — preserved
- Isolation guard internals (`_allowedReads` / `_allowedWrites` / `_allowedBuses` HashSets) — preserved structurally; their consumers (the deleted methods) gone, but the data preserved for future analyzer milestone integration

**Isolation guard fate**: existing `_allowedReads` / `_allowedWrites` HashSets keyed by `Type`. Post-combined, the deletion of `GetComponent` / `SetComponent` methods removes the only call sites that check these. The HashSets become unused.

**Option A**: delete the HashSets entirely. Isolation guard effectively gone — systems can read/write any component type via `NativeWorld` directly. Trust restored to compile-time + analyzer.

**Option B**: preserve the HashSets but add new check points in `NativeWorld.AcquireSpan<T>` / `BeginBatch<T>` wrappers that consult the current `SystemExecutionContext.AllowedReads` / `AllowedWrites`. Isolation guard preserved at native-access surface.

**Decision**: **Option A — delete isolation guard fields**. Rationale:
- Compile-time enforcement via `[SystemAccess]` attribute + future analyzer (A'.9) provides static safety
- Runtime check overhead (HashSet lookup per `AcquireSpan` / `BeginBatch` call) is non-trivial in hot path
- Existing K7 performance measurements were taken without this overhead; preserving runtime check changes performance contract
- Option B can be reintroduced at A'.9 analyzer milestone if profile demands

**Side effect**: tests that exercised isolation violations (`IsolationViolationException`) need rewriting. They tested behavior that no longer exists. Mark as deleted in Phase 5 commit 21 with rationale: «isolation guard moved to compile-time + analyzer per A'.9; runtime violation no longer enforceable without performance cost».

Approximate test count affected: ~5-15 in `DualFrontier.Core.Tests/ECS/IsolationViolationTests.cs` or similar. Verify count at Phase 0 inventory.

### §3.3 — Per-system migration patterns

**Generic system migration template**:

Pre-combined system body pattern:
```csharp
public sealed class NeedsSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (EntityId entity in Query<NeedsComponent>())
        {
            NeedsComponent needs = GetComponent<NeedsComponent>(entity);
            needs.Satiety = MathF.Max(0f, needs.Satiety - delta * 0.001f);
            needs.Hydration = MathF.Max(0f, needs.Hydration - delta * 0.002f);
            SetComponent<NeedsComponent>(entity, needs);
        }
    }
}
```

Post-combined system body pattern:
```csharp
public sealed class NeedsSystem : SystemBase
{
    public override void Update(float delta)
    {
        using var lease = NativeWorld.AcquireSpan<NeedsComponent>();
        using var batch = NativeWorld.BeginBatch<NeedsComponent>();
        
        for (int i = 0; i < lease.Count; i++)
        {
            EntityId entity = new EntityId(lease.Indices[i], 0); // version not tracked in span
            ref readonly NeedsComponent needs = ref lease.Span[i];
            
            NeedsComponent updated = new NeedsComponent
            {
                Satiety = MathF.Max(0f, needs.Satiety - delta * 0.001f),
                Hydration = MathF.Max(0f, needs.Hydration - delta * 0.002f),
                Sleep = needs.Sleep,
                Comfort = needs.Comfort,
            };
            
            batch.Update(entity, updated);
        }
        
        // batch.Flush() implicit on Dispose at end of using scope
        // lease.Dispose() releases span; mutations now visible
    }
}
```

**Key transformations**:
- `Query<T>()` → `NativeWorld.AcquireSpan<T>()` (single P/Invoke, zero-allocation iteration)
- `GetComponent<T>(entity)` → `ref readonly span[i]` (no per-entity P/Invoke; direct memory access)
- `SetComponent<T>(entity, value)` → `batch.Update(entity, updated)` (queued; flushed atomically)
- Reading and writing are now **explicitly separated**: read via lease, write via batch. The K-L7 invariant «no mutation while span active» is enforced by `NativeWorld` — writes via batch are deferred until lease disposal.

**Query<T1,T2> migration pattern**:

Pre-combined:
```csharp
foreach (EntityId entity in Query<NeedsComponent, PositionComponent>())
{
    NeedsComponent needs = GetComponent<NeedsComponent>(entity);
    PositionComponent pos = GetComponent<PositionComponent>(entity);
    // ... logic ...
}
```

Post-combined (smaller-store-iteration pattern):
```csharp
// Acquire both spans
using var needsLease = NativeWorld.AcquireSpan<NeedsComponent>();
using var posLease = NativeWorld.AcquireSpan<PositionComponent>();

// Iterate the smaller store
ReadOnlySpan<int> smallerIndices = needsLease.Count <= posLease.Count 
    ? needsLease.Indices 
    : posLease.Indices;

for (int i = 0; i < smallerIndices.Length; i++)
{
    EntityId entity = new EntityId(smallerIndices[i], 0);
    
    // Check the other store via HasComponent
    if (!NativeWorld.HasComponent<PositionComponent>(entity)) continue;
    
    NeedsComponent needs = NativeWorld.GetComponent<NeedsComponent>(entity);
    PositionComponent pos = NativeWorld.GetComponent<PositionComponent>(entity);
    // ... logic ...
}
```

**Performance note**: per-entity `HasComponent` + `GetComponent` calls cross P/Invoke boundary; for hot paths consider redesigning to use single-component iteration. Per-system migration commits assess whether the legacy `Query<T1,T2>` pattern translates directly or whether the system needs restructuring. Restructuring decisions recorded in per-commit messages.

**Write-only pattern (e.g., MoodSystem on read-only span over MindComponent)**:

```csharp
using var mindLease = NativeWorld.AcquireSpan<MindComponent>();
using var batch = NativeWorld.BeginBatch<MoodComponent>();

for (int i = 0; i < mindLease.Count; i++)
{
    EntityId entity = new EntityId(mindLease.Indices[i], 0);
    ref readonly MindComponent mind = ref mindLease.Span[i];
    
    MoodComponent newMood = CalculateMood(mind);
    batch.Update(entity, newMood);
}
```

### §3.4 — Rule-based ordering (Tier 1-5)

**Tier 1 systems** (touch only Tier 1-2 components, single-system writes, lowest interleave risk):

- **ConsumeSystem** (Phase 4 commit 9): reads NeedsComponent (Satiety/Hydration facets) + ConsumableComponent + WaterSourceComponent + PositionComponent; writes NeedsComponent (Satiety/Hydration facets) + destroys ConsumableComponent/WaterSourceComponent entities on consumption.
- **SleepSystem** (Phase 4 commit 10): reads NeedsComponent (Sleep + Comfort facets) + BedComponent + PositionComponent + ReservationComponent; writes NeedsComponent (Sleep + Comfort facets) + ReservationComponent.
- **ComfortAuraSystem** (Phase 4 commit 11): reads NeedsComponent (Comfort facet) + DecorativeAuraComponent + PositionComponent; writes NeedsComponent (Comfort facet).

**Tier 2 systems** (touch Tier 3 components, no cross-system writes):

- **MoodSystem** (Phase 4 commit 12): reads NeedsComponent + MindComponent; writes MindComponent.
- **PawnStateReporterSystem** (Phase 4 commit 13): reads NeedsComponent + IdentityComponent + SkillsComponent + JobComponent + MindComponent + PositionComponent; writes nothing (pure read pipeline, emits PawnStateChangedEvent).

**Tier 3 systems** (cross-system writes, no K8.1 primitives):

- **JobSystem** (Phase 4 commit 14): reads JobComponent + ReservationComponent + NeedsComponent + PositionComponent; writes JobComponent + ReservationComponent.
- **HaulSystem** (Phase 4 commit 15): reads ReservationComponent + StorageComponent + PositionComponent; writes ReservationComponent + StorageComponent (read-modify-write).
- **NeedsSystem** (Phase 4 commit 16): reads NeedsComponent only; writes NeedsComponent (all facets — passive decay). High-frequency; downstream of every other Need-touching system per tick scheduler phase ordering.

**Tier 4 systems** (touch Tier 4 components or write Tier 5):

- **ConverterSystem** (Phase 4 commit 17): reads PowerConsumerComponent + PowerProducerComponent; writes PowerConsumerComponent + PowerProducerComponent.
- **ElectricGridSystem** (Phase 4 commit 18): reads PowerConsumerComponent + PowerProducerComponent + PositionComponent; writes PowerConsumerComponent + PowerProducerComponent (grid balancing).
- **InventorySystem** (Phase 4 commit 19): reads StorageComponent + ReservationComponent; writes StorageComponent (heavy NativeMap+NativeSet operations via K8.1 primitives).

**Tier 5 systems** (touch Tier 5 spatial components):

- **MovementSystem** (Phase 4 commit 20): reads MovementComponent + PositionComponent + NavGrid; writes MovementComponent + PositionComponent (NativeComposite path consumption + position updates).

**Ordering rationale**: Tier 1 systems migrate first because each touches a narrow component set with simple read/write patterns. Issues surfaced during Tier 1 commits indicate fundamental brief assumptions wrong → halt and reassess. Issues surfaced during Tier 5 commits are typically pattern-specific to that system (e.g., NativeComposite path consumption mechanics in MovementSystem). Late-tier issues localized; early-tier issues structural.

---

## §4 — Architectural concern: Mod API v3 surface

### §4.1 — IModApi.RegisterComponent constraint split

**Current v2 signature** (at HEAD `efd67df`, `src/DualFrontier.Contracts/Modding/IModApi.cs`):
```csharp
void RegisterComponent<T>() where T : IComponent;
```

No distinction between value-type (struct, Path α) and reference-type (class, Path β) components. K-L3.1 lock requires distinction.

**Post-combined v3 signature**:
```csharp
void RegisterComponent<T>() where T : unmanaged, IComponent;
void RegisterManagedComponent<T>() where T : class, IComponent;
```

**Breaking change semantics**:
- Mods that called `RegisterComponent<T>` with class types (Path β before K-L3.1) **will not compile** post-combined. Correct — those registrations were pre-K-L3.1 ambiguous and pre-combined wiring would have silently failed at runtime.
- Mods that called `RegisterComponent<T>` with unmanaged struct types continue to compile (constraint strengthens but still satisfied).
- New Path β consumers explicitly call `RegisterManagedComponent<T>` and the type must be annotated with `[ManagedStorage]`.

**No backward compatibility** per Crystalka direction. No grace period. No `[Obsolete]` warnings. No deprecation layer. Mods that don't compile post-combined are correctly rejected — combined milestone is the architectural break point.

**RestrictedModApi implementation** (Phase 3 commit 7):
```csharp
internal sealed class RestrictedModApi : IModApi
{
    // ...existing fields preserved (modId, manifest, registry, contractStore, services, kernelCapabilities, fieldsApi, subscriptions)...
    private readonly Dictionary<Type, IManagedStore> _managedStores = new();
    
    public void RegisterComponent<T>() where T : unmanaged, IComponent
    {
        _registry.RegisterComponent(_modId, typeof(T));
        // ComponentTypeRegistry on NativeWorld side registers the type id; via ModRegistry pathway
    }
    
    public void RegisterManagedComponent<T>() where T : class, IComponent
    {
        // 1. Verify T has [ManagedStorage] attribute (Q-COMBINED-3 lock)
        if (!typeof(T).IsDefined(typeof(ManagedStorageAttribute), inherit: false))
        {
            throw new ValidationException(
                ValidationErrorKind.MissingManagedStorageAttribute,
                $"Type {typeof(T).FullName} registered via RegisterManagedComponent must have [ManagedStorage] attribute.");
        }
        
        // 2. Verify manifest declares manifestVersion: "3" (Q-COMBINED-5 lock)
        // Combined milestone guarantees this — ManifestParser rejects non-v3 manifests at load time.
        // Defense-in-depth runtime check elided here; load-time gate is sufficient.
        
        // 3. Create per-mod store
        _managedStores[typeof(T)] = new ManagedStore<T>(_modId);
        
        // 4. Register with ModRegistry for resolver dispatch
        _registry.RegisterManagedComponent(_modId, typeof(T));
    }
    
    /// <summary>
    /// Called by SystemBase.ManagedStore&lt;T&gt;() via IManagedStorageResolver.
    /// Returns null if T not registered by this mod.
    /// </summary>
    internal ManagedStore<T>? GetManagedStore<T>() where T : class, IComponent
    {
        return _managedStores.TryGetValue(typeof(T), out var store) ? store as ManagedStore<T> : null;
    }
    
    /// <summary>
    /// Called from ModLoader.UnloadMod (existing M0-M7 unload chain step 5).
    /// Clears all per-mod managed stores; storage is reclaimed deterministically
    /// per MOD_OS_ARCHITECTURE §9.5.
    /// </summary>
    internal void ClearManagedStores()
    {
        foreach (var store in _managedStores.Values)
            store.Clear();
        _managedStores.Clear();
    }
    
    // ...rest of RestrictedModApi v2 surface preserved (Publish, Subscribe, PublishContract, etc.)...
}
```

### §4.2 — IModApi.RegisterManagedComponent design

Full signature post-combined:

```csharp
/// <summary>
/// Register a Path β managed-class component (K-L3.1 bridge per
/// KERNEL_ARCHITECTURE.md Part 0 K-L3 implication post-K-L3.1).
/// 
/// Type T must:
/// - Be a class implementing IComponent (constraint: where T : class, IComponent)
/// - Be annotated with [ManagedStorage] attribute; absence raises
///   ValidationErrorKind.MissingManagedStorageAttribute at registration time
/// 
/// Storage lives in the per-mod RestrictedModApi instance (mod-side ownership
/// per K-L3.1 Q2.β-i); reclaimed deterministically on AssemblyLoadContext.Unload
/// per MOD_OS_ARCHITECTURE §9.5 unload chain.
/// 
/// Path β components are runtime-only (Q4.b lock) — not persisted by save system;
/// reconstructed on load post-G-series.
/// 
/// Mods that need Path β registration must declare manifestVersion: "3" — the
/// manifest parser rejects non-v3 manifests at load time.
/// </summary>
void RegisterManagedComponent<T>() where T : class, IComponent;
```

### §4.3 — RestrictedModApi.ManagedStore<T> implementation

File creation (Phase 3 commit 6): `src/DualFrontier.Application/Modding/ManagedStore.cs`

```csharp
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Marker interface for type-erased ManagedStore&lt;T&gt; instances. Allows
/// RestrictedModApi to hold a Dictionary&lt;Type, IManagedStore&gt; without
/// open generics. Concrete operations require downcast to ManagedStore&lt;T&gt;.
/// </summary>
internal interface IManagedStore
{
    /// <summary>Number of components currently stored.</summary>
    int Count { get; }
    
    /// <summary>Clear all stored components. Called from RestrictedModApi.ClearManagedStores on ALC.Unload.</summary>
    void Clear();
}

/// <summary>
/// Per-mod Path β component storage per K-L3.1 bridge formalization.
/// 
/// Implementation: BCL Dictionary&lt;EntityId, T&gt; (single-threaded — Path β
/// access serialized through scheduler phase ordering per existing single-
/// threaded contract; ConcurrentDictionary not required).
/// 
/// Lifetime: scoped to owning mod's RestrictedModApi instance. Cleared on
/// ALC.Unload via ModLoader.UnloadMod step 5 (chain to RestrictedModApi.ClearManagedStores).
/// </summary>
internal sealed class ManagedStore<T> : IManagedStore where T : class, IComponent
{
    private readonly Dictionary<EntityId, T> _components = new();
    private readonly string _modId;
    
    /// <summary>Constructs an empty store bound to a specific mod's lifetime.</summary>
    internal ManagedStore(string modId)
    {
        _modId = modId ?? throw new System.ArgumentNullException(nameof(modId));
    }
    
    /// <summary>Adds or overwrites the component for the given entity.</summary>
    public void Add(EntityId entity, T component) => _components[entity] = component;
    
    /// <summary>Attempts to retrieve the component for the given entity. Returns false if absent.</summary>
    public bool TryGet(EntityId entity, out T? component) => _components.TryGetValue(entity, out component);
    
    /// <summary>Returns true if a component exists for the given entity.</summary>
    public bool Has(EntityId entity) => _components.ContainsKey(entity);
    
    /// <summary>Removes the component for the given entity. No-op if absent.</summary>
    public bool Remove(EntityId entity) => _components.Remove(entity);
    
    /// <summary>Enumerates entities that have a component in this store. Lazy.</summary>
    public IEnumerable<EntityId> Entities => _components.Keys;
    
    /// <summary>Count of stored components.</summary>
    public int Count => _components.Count;
    
    /// <summary>Clears the store. Called from RestrictedModApi.ClearManagedStores on unload.</summary>
    public void Clear() => _components.Clear();
}
```

### §4.4 — SystemBase.ManagedStore<T> accessor

Already specified in §3.1 (final SystemBase shape). Accessor body:

```csharp
protected ManagedStore<T>? ManagedStore<T>() where T : class, IComponent
{
    var ctx = SystemExecutionContext.Current
        ?? throw new InvalidOperationException(
            "SystemBase.ManagedStore called outside an active scheduler context.");
    return ctx.ResolveManagedStore<T>();
}
```

`SystemExecutionContext.ResolveManagedStore<T>` implementation:

```csharp
internal ManagedStore<T>? ResolveManagedStore<T>() where T : class, IComponent
{
    if (_managedStorageResolver is null) return null;  // Core system, no per-mod resolver
    if (_modId is null) return null;                    // Core system origin
    return _managedStorageResolver.Resolve<T>(_modId);
}
```

### §4.5 — [ManagedStorage] attribute

File creation (Phase 3 commit 6): `src/DualFrontier.Contracts/Modding/ManagedStorageAttribute.cs`

```csharp
using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Marks a class IComponent type as Path β (managed-class storage) per K-L3.1.
/// Mods register such components via IModApi.RegisterManagedComponent&lt;T&gt;.
/// 
/// Without this attribute, RegisterManagedComponent&lt;T&gt; raises
/// ValidationErrorKind.MissingManagedStorageAttribute at registration time
/// (per MOD_OS_ARCHITECTURE.md v1.8 §11.2 enum extension).
/// 
/// Path β components are runtime-only (Q4.b K-L3.1 lock) — not persisted
/// by save system; reconstructed on load post-G-series. Storage lives in
/// per-mod RestrictedModApi.ManagedStore&lt;T&gt; instance; reclaimed on
/// AssemblyLoadContext.Unload per MOD_OS_ARCHITECTURE §9.5 unload chain.
/// 
/// Authority: KERNEL_ARCHITECTURE.md v1.5+ Part 0 K-L3 implication post-K-L3.1;
/// K-L3.1 amendment plan at docs/architecture/K_L3_1_AMENDMENT_PLAN.md.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ManagedStorageAttribute : Attribute
{
}
```

Location rationale: lives in `DualFrontier.Contracts.Modding` namespace because mod assemblies reference it directly (they annotate their own component classes); cannot live in `DualFrontier.Application.Modding` (which mods do not reference).

### §4.6 — IManagedStorageResolver contract

File creation (Phase 3 commit 7): `src/DualFrontier.Application/Modding/IManagedStorageResolver.cs`

```csharp
using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Resolves a mod-id to its RestrictedModApi.ManagedStore&lt;T&gt; instance.
/// Implementation lives in DualFrontier.Application.Modding.ModRegistry
/// (knows mod-id → RestrictedModApi mapping). Surfaced to
/// SystemExecutionContext via the ParallelSystemScheduler constructor.
/// 
/// Indirection rationale: SystemExecutionContext lives in DualFrontier.Core
/// namespace and cannot reference DualFrontier.Application.Modding directly
/// without inverting dependency direction. The interface provides a seam.
/// </summary>
internal interface IManagedStorageResolver
{
    /// <summary>
    /// Returns the ManagedStore&lt;T&gt; for the mod identified by modId, or null
    /// if the mod has not registered T via RegisterManagedComponent&lt;T&gt;.
    /// Returns null for unknown modId.
    /// </summary>
    ManagedStore<T>? Resolve<T>(string modId) where T : class, IComponent;
}
```

`ModRegistry` implementation (Phase 3 commit 7 modifies existing `ModRegistry.cs`):

```csharp
internal sealed class ModRegistry : IManagedStorageResolver
{
    // ...existing M0-M7 fields preserved...
    private readonly Dictionary<string, RestrictedModApi> _restrictedModApis = new();
    
    /// <summary>
    /// Called from ModLoader at mod load time when constructing the
    /// RestrictedModApi for a regular mod. Allows ModRegistry to dispatch
    /// IManagedStorageResolver.Resolve queries to the right RestrictedModApi.
    /// </summary>
    internal void RegisterRestrictedModApi(string modId, RestrictedModApi api)
    {
        _restrictedModApis[modId] = api;
    }
    
    /// <summary>
    /// Called from ModLoader.UnloadMod step 5 (existing M0-M7 chain), after
    /// RestrictedModApi.UnsubscribeAll and RestrictedModApi.ClearManagedStores.
    /// </summary>
    internal void UnregisterRestrictedModApi(string modId)
    {
        _restrictedModApis.Remove(modId);
    }
    
    public ManagedStore<T>? Resolve<T>(string modId) where T : class, IComponent
    {
        return _restrictedModApis.TryGetValue(modId, out var api) 
            ? api.GetManagedStore<T>() 
            : null;
    }
    
    // ...rest of ModRegistry preserved...
}
```

`ParallelSystemScheduler` consumes the resolver:

```csharp
internal sealed class ParallelSystemScheduler
{
    // ...existing fields preserved, World replaced with NativeWorld + registry + resolver...
    private readonly NativeWorld _nativeWorld;
    private readonly ComponentTypeRegistry _registry;
    private readonly IManagedStorageResolver? _managedStorageResolver;
    
    public ParallelSystemScheduler(
        IReadOnlyList<SystemPhase> phases,
        TickScheduler ticks,
        NativeWorld nativeWorld,                                  // ← required (was managed World)
        ComponentTypeRegistry registry,                           // ← NEW required
        IReadOnlyDictionary<SystemBase, SystemMetadata> systemMetadata,
        IModFaultSink faultSink,
        IGameServices? services = null,
        IManagedStorageResolver? managedStorageResolver = null)   // ← NEW optional
    {
        _nativeWorld = nativeWorld ?? throw new ArgumentNullException(nameof(nativeWorld));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _managedStorageResolver = managedStorageResolver;
        // ...rest preserved...
    }
}
```

In `GameBootstrap.CreateLoop`, the resolver is supplied via `ModRegistry`:

```csharp
var modRegistry = new ModRegistry();
// ...rest of mod stack construction preserved...

var scheduler = new ParallelSystemScheduler(
    graph.GetPhases(),
    ticks,
    nativeWorld,                  // ← required
    registry,                     // ← required
    initialMetadata,
    faultHandler,
    services,
    modRegistry);                 // ← ModRegistry implements IManagedStorageResolver
```

---

## §5 — Architectural concern: Manifest schema

### §5.1 — ManifestVersion field

File modification (Phase 3 commit 8): `src/DualFrontier.Contracts/Modding/ModManifest.cs`

Add field:

```csharp
/// <summary>
/// Manifest schema version per O-1 audit (2026-05-13). Distinguishes the
/// IModApi surface available to the mod:
/// - "3" = full v3 surface (RegisterManagedComponent, Fields, ComputePipelines)
/// 
/// Post-K8.3+K8.4 combined milestone (2026-05-XX), only "3" is accepted by
/// ManifestParser. v1/v2 manifests rejected with
/// ValidationErrorKind.IncompatibleContractsVersion at load time. No grace
/// period; no deprecation warnings; no backward compatibility.
/// 
/// Mods authored before combined milestone must update their manifest:
///   {
///     "manifestVersion": "3",
///     ...
///   }
/// 
/// Default value "3" applies when constructed in-memory (e.g. tests); on-disk
/// manifests without manifestVersion field are rejected (default is for
/// programmatic construction, not on-disk parsing).
/// </summary>
public string ManifestVersion { get; init; } = "3";
```

**Default rationale**: in-memory `new ModManifest { Id = "x", ... }` constructions (in tests and bootstrap code) get `"3"` automatically — they don't need to specify the field. On-disk JSON manifests must explicitly include `"manifestVersion": "3"` because the JSON deserializer creates a fresh instance and the default applies only if field is absent from JSON. Combined with `ManifestParser.Parse` validation, missing field on disk → rejection (the parser enforces explicit declaration even though the C# default exists).

### §5.2 — Parser branching

File modification (Phase 3 commit 8): `src/DualFrontier.Application/Modding/ManifestParser.cs`

Add validation step:

```csharp
internal sealed class ManifestParser
{
    public static ParseResult Parse(string json)
    {
        // ...existing parse...
        var manifest = JsonSerializer.Deserialize<ModManifest>(json, options);
        var errors = new List<ValidationError>();
        var warnings = new List<string>();
        
        // NEW — Manifest version gate (Q-COMBINED-5 lock)
        if (string.IsNullOrEmpty(manifest.ManifestVersion))
        {
            errors.Add(new ValidationError(
                ValidationErrorKind.IncompatibleContractsVersion,
                "Manifest must declare manifestVersion field. Only \"3\" is accepted."));
            return new ParseResult(null, errors, warnings);
        }
        
        if (manifest.ManifestVersion != "3")
        {
            errors.Add(new ValidationError(
                ValidationErrorKind.IncompatibleContractsVersion,
                $"Manifest declares manifestVersion: \"{manifest.ManifestVersion}\" — only \"3\" is accepted. " +
                $"v1/v2 manifests must be updated to v3 format."));
            return new ParseResult(null, errors, warnings);
        }
        
        // ...rest of existing validation steps (dependencies, capabilities, etc.)...
    }
}
```

No `IsV3Verb` helper needed — combined milestone accepts only v3 manifests, so `field.*` and `pipeline.*` verbs are always allowed.

### §5.3 — Vanilla mod manifest updates

File modifications (Phase 3 commit 8): 7 vanilla mod `mod.manifest.json` files.

Locations (verified during Phase 0 inventory):
- `mods/DualFrontier.Mod.Vanilla.Combat/mod.manifest.json`
- `mods/DualFrontier.Mod.Vanilla.Core/mod.manifest.json`
- `mods/DualFrontier.Mod.Vanilla.Inventory/mod.manifest.json`
- `mods/DualFrontier.Mod.Vanilla.Magic/mod.manifest.json`
- `mods/DualFrontier.Mod.Vanilla.Pawn/mod.manifest.json`
- `mods/DualFrontier.Mod.Vanilla.World/mod.manifest.json`
- (possible 7th shared mod — verify count at Phase 0)

Each manifest gains one line:
```json
{
  "manifestVersion": "3",
  ...existing fields preserved...
}
```

Atomic with parser change (Phase 3 commit 8) — if parser update lands without manifest updates, vanilla mod loading breaks. If manifest updates land without parser, no error but no enforcement. Single commit covers both.

### §5.4 — Fixture manifest updates

Test fixtures with `mod.manifest.json` (verified during Phase 0 inventory; expected locations):
- `tests/Fixture.BadRegularMod/`
- `tests/Fixture.BadSharedMod_WithIMod/`
- `tests/Fixture.PublisherMod/`
- `tests/Fixture.RegularMod_BadApiVersion/`
- `tests/Fixture.RegularMod_CyclicA/`
- `tests/Fixture.RegularMod_CyclicB/`
- `tests/Fixture.RegularMod_DependedOn/`
- `tests/Fixture.RegularMod_DependsOnAnother/`
- `tests/Fixture.RegularMod_DependsOnBadApi/`
- `tests/Fixture.RegularMod_DepsBadVersion/`
- `tests/Fixture.RegularMod_MissingOptional/`
- `tests/Fixture.RegularMod_MissingRequired/`
- `tests/Fixture.RegularMod_ReplacesCombat/`
- `tests/Fixture.RegularMod_ReplacesCombat_Alt/`
- `tests/Fixture.RegularMod_ReplacesProtected/`
- `tests/Fixture.RegularMod_ReplacesUnknown/`
- `tests/Fixture.SharedEvents/`
- `tests/Fixture.SubscriberMod/`
- `tests/Fixture.VanillaMod_HotReloadOverride/`

~19 fixture manifests gain `"manifestVersion": "3"` line. Same commit as vanilla manifests (Phase 3 commit 8).

**Exception**: `tests/Fixture.BadRegularMod` may need to retain bad manifest for negative testing (it intentionally has malformed schema). Verify at Phase 0 — if it tests dependency cycle or other non-version-related failure mode, add v3 line; if it tests v1/v2 rejection (post-combined), preserve as-is.

### §5.5 — Manifest version test additions

File creation (Phase 6 commit 23, optional — may bundle into Phase 3 commit 8): `tests/DualFrontier.Modding.Tests/ManifestVersionTests.cs`

~6-8 tests covering:
- `Parse_ManifestVersion3_Succeeds`
- `Parse_ManifestVersion1_RejectsWithIncompatibleContractsVersion`
- `Parse_ManifestVersion2_RejectsWithIncompatibleContractsVersion`
- `Parse_MissingManifestVersion_RejectsWithIncompatibleContractsVersion`
- `Parse_UnknownManifestVersion_RejectsWithIncompatibleContractsVersion` (e.g., `"4"`, `"abc"`)
- `Parse_ManifestVersion3_WithFieldCapabilities_Succeeds`
- `Parse_ManifestVersion3_WithPipelineCapabilities_Succeeds`

---

## §6 — Phase-by-phase execution plan

### §6.1 — Phase 0: pre-flight verification (mandatory deep-reads)

**Step 0 — brief authoring commit** (Commit 1):

This brief itself was authored on main during Opus deliberation session 2026-05-13. The brief file `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` is committed as a separate prerequisite step before execution begins, per METHODOLOGY K1-era lesson «brief authoring as prerequisite step».

```powershell
# Already done at Opus session 2026-05-13 (this brief committed on main).
# Cloud Code session opens with HEAD pointing to commit containing this brief.
git log --oneline -1 tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md
# Expected: "docs(briefs): K8.3+K8.4 combined kernel cutover brief authored"
```

**Step 1 — hard gates** (workspace state):

```powershell
# HG-1: Working tree clean
git status --porcelain
# Expected: no output (clean)
# If output: HALT — commit or stash before proceeding

# HG-2: Baseline tests passing
dotnet test --no-build src/DualFrontier.sln 2>&1 | Select-String -Pattern "Passed:\s+\d+"
# Expected: "Passed: 631+" (post-A'.4.5 + K9 closure baseline)
# If failing: HALT — identify regression source before any K8.3+K8.4 work

# HG-3: Native build clean
cmake --build native/DualFrontier.Core.Native/out --target df_core_native_selftest
./native/DualFrontier.Core.Native/out/df_core_native_selftest
# Expected: all scenarios pass
# If failing: HALT — K8.1 era invariant broken, not K8.3+K8.4 scope

# HG-4: HEAD includes A'.4.5 closure state (register operational)
Test-Path docs/governance/REGISTER.yaml
Test-Path tools/governance/sync_register.ps1
./tools/governance/sync_register.ps1 --validate
# Expected: exit 0, no errors, no warnings
# If failing: HALT — register state corrupted, must repair before K8.3+K8.4 closure

# HG-5: K8.3 v2.0 halt artifacts on disk
Test-Path docs/scratch/A_PRIME_5/HALT_REPORT.md
Test-Path tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md
# Expected: both present
# These are historical artifacts; combined milestone supersedes them but they
# remain on disk.
```

**Step 2 — informational checks** (record + continue):

```powershell
# IC-1: HEAD SHA — record for closure entry provenance
git rev-parse HEAD
# Record: <SHA>

# IC-2: Commit count ahead of origin
git log --oneline origin/main..HEAD | Measure-Object -Line
# Record: <N> commits ahead

# IC-3: Verify K8.3 v2.0 + K8.3 patch + K8.4 skeleton supersession targets exist
Test-Path tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md
Test-Path tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md
Test-Path tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md
# Expected: all three present
# Record any missing — affects Phase 6 REGISTER lifecycle transition list
```

**Step 3 — deep-read mandatory files** (Lesson #1 from K8.3 halt resolution):

Mandatory reads in full (not just for specific scope verification):
- `src/DualFrontier.Application/Loop/GameBootstrap.cs` (entry-point file; embedded transitional-state comments)
- `src/DualFrontier.Core.Interop/Bootstrap.cs` (managed bootstrap orchestration)
- `src/DualFrontier.Core.Interop/NativeWorld.cs` (NativeWorld API surface; verify `AcquireSpan`, `BeginBatch`, bulk operations, registry binding all present)
- `src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs` (registry implementation)
- `src/DualFrontier.Core/ECS/World.cs` (managed World; about to be moved)
- `src/DualFrontier.Core/ECS/SystemBase.cs` (legacy API about to be deleted)
- `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` (about to be refactored)
- `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` (about to receive new ctor signature)
- `src/DualFrontier.Contracts/Modding/IModApi.cs` (about to be v3'd)
- `src/DualFrontier.Contracts/Modding/ModManifest.cs` (about to gain ManifestVersion field)
- `src/DualFrontier.Application/Modding/RestrictedModApi.cs` (about to receive ManagedStore plumbing)
- `src/DualFrontier.Application/Modding/ModRegistry.cs` (about to receive IManagedStorageResolver implementation)
- `src/DualFrontier.Application/Modding/ManifestParser.cs` (about to receive v3-only gate)

Embedded-comment harvest (Lesson #3 from K8.3 halt resolution — cross-reference K-Lxx implication paragraphs):

```powershell
Get-ChildItem -Path src/ -Recurse -Filter *.cs | 
  Select-String -Pattern '(until|post-|lights up at|retires the latter)\s*K\d+\.\d+' -SimpleMatch |
  ForEach-Object { $_.Line }
# Expected matches: GameBootstrap.cs lines 91-96 (the comment that triggered K8.3 halt);
# Any other matches indicate transitional-state assumptions to verify before proceeding.
```

**Step 4 — production component inventory** (Phase 0.4 inventory as hypothesis, not authority — K8.1 lesson):

```powershell
# Inventory components used by GameBootstrap.coreSystems
Select-String -Path src/DualFrontier.Application/Loop/GameBootstrap.cs -Pattern 'new \w+System\(' -SimpleMatch |
  ForEach-Object { $_.Matches.Value }
# Expected: 12 system constructors (NeedsSystem, MoodSystem, JobSystem, ConsumeSystem, 
# SleepSystem, ComfortAuraSystem, MovementSystem, PawnStateReporterSystem, InventorySystem, 
# HaulSystem, ElectricGridSystem, ConverterSystem)
# Record: actual list

# Inventory factory writes
Select-String -Path src/DualFrontier.Application/Loop/GameBootstrap.cs -Pattern 'world\.AddComponent' -SimpleMatch
Select-String -Path src/DualFrontier.Application/Scenario/RandomPawnFactory.cs -Pattern 'world\.AddComponent|nativeWorld\.AddComponent' -SimpleMatch
Select-String -Path src/DualFrontier.Application/Scenario/ItemFactory.cs -Pattern 'world\.AddComponent|nativeWorld\.AddComponent' -SimpleMatch
# Record: per-component-type AddComponent call sites

# Inventory orphan .uid files
Get-ChildItem -Path src/DualFrontier.Systems/ -Recurse -Filter *.uid |
  Where-Object { -not (Test-Path ($_.FullName -replace '\.uid, '')) }
# Expected: 3 files (ShieldSystem.cs.uid, SocialSystem.cs.uid, BiomeSystem.cs.uid)
# Record: actual list with paths
```

**Step 5 — storage premise verification** (Lesson #2 from K8.3 halt resolution):

```powershell
# Verify HEAD is post-A'.4.5 closure + halt resolution
git log --oneline -10 origin/main
# Expected: recent commits include K8.3 halt resolution (b46a6c6..efd67df range)

# Confirm GameBootstrap embedded comment still says «K8.4 lights up storage»
Select-String -Path src/DualFrontier.Application/Loop/GameBootstrap.cs -Pattern 'lights up at K8.4' -SimpleMatch
# Expected: present (this is the comment K8.3 v2.0 brief missed; this brief addresses it)
# This is informational — confirms HEAD state matches brief authoring assumption.
```

**Step 6 — confirm clean working tree before Phase 1 begins**:

```powershell
git status --porcelain
# Expected: clean. Phase 0 has been pure inventory + reads; no edits.
```

### §6.2 — Phase 1: orphan .uid cleanup (Commit 2)

**Scope**: Delete 3 orphan `.uid` files left over from K8.2 v2 closure (deleted `.cs` files; `.uid` companions survived).

Godot's `.uid` files are auto-generated per `.cs` file in the editor. When a `.cs` file is deleted, the `.uid` should be deleted too. K8.2 v2 deletion of ShieldSystem/SocialSystem/BiomeSystem `.cs` files missed the companion `.uid` deletion. This is METHODOLOGY §7.1 «data exists or it doesn't» application — the `.uid` files reference data that no longer exists.

**Files to delete** (verified at Phase 0 step 4):
- `src/DualFrontier.Systems/Combat/ShieldSystem.cs.uid`
- `src/DualFrontier.Systems/Pawn/SocialSystem.cs.uid`
- `src/DualFrontier.Systems/World/BiomeSystem.cs.uid`

**Action**:
```powershell
Remove-Item src/DualFrontier.Systems/Combat/ShieldSystem.cs.uid
Remove-Item src/DualFrontier.Systems/Pawn/SocialSystem.cs.uid
Remove-Item src/DualFrontier.Systems/World/BiomeSystem.cs.uid
```

**Verification**:
```powershell
dotnet build src/DualFrontier.sln
# Expected: succeeds (UID files don't affect build)

dotnet test src/DualFrontier.sln
# Expected: 631+ tests pass (UID files don't affect runtime)
```

**Commit shape**:
```
chore(scratch): delete 3 orphan .uid files (METHODOLOGY §7.1)

ShieldSystem, SocialSystem, BiomeSystem .cs files deleted at K8.2 v2 closure;
.uid companions survived. This is the 12th formal application of METHODOLOGY 
§7.1 «data exists or it doesn't» — .uid files reference data (their .cs 
companions) that no longer exists.

Part of K8.3+K8.4 combined milestone Phase 1.
```

### §6.3 — Phase 2: storage migration foundation (Commits 3-4)

**Commit 3: ComponentTypeRegistry activation + Bootstrap.Run wiring + factory dual-write switch**

**Files modified**:
- `src/DualFrontier.Application/Loop/GameBootstrap.cs`: 
  - Add `RegisterProductionComponentTypes(registry)` helper method body (specified in §2.1)
  - Replace `var nativeWorld = new NativeWorld();` with `var registry = new ComponentTypeRegistry(); var nativeWorld = Bootstrap.Run(registry); RegisterProductionComponentTypes(registry);`
  - Update embedded comment block: remove «K8.2 v2 — NativeWorld owned alongside the managed World until K8.4 retires» (the post-K-L3.1 comment); replace with new comment explaining current dual-write phase: «Phase 2-4 dual-write: factories write to both managed World and NativeWorld during system migration. Managed-side write removed in Phase 5 commit 21 after last system migrates.»
  - Factories receive both `nativeWorld` and `world` parameters
  - `RandomPawnFactory.Spawn(nativeWorld, world, services, count)` signature change (factory file modified)
  - `ItemFactory.Spawn(nativeWorld, world, ...)` signature change
- `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs`: dual-write loop pattern (specified in §2.2)
- `src/DualFrontier.Application/Scenario/ItemFactory.cs`: dual-write loop pattern
- `tests/DualFrontier.Systems.Tests/Fixtures/NativeWorldTestFixture.cs`: NEW file (specified in §7.2)

**Commit shape**:
```
feat(bootstrap): activate ComponentTypeRegistry + Bootstrap.Run wiring (K8.4 storage Phase 2/2)

- GameBootstrap.CreateLoop constructs ComponentTypeRegistry, calls Bootstrap.Run
  with registry-bound NativeWorld (replaces new NativeWorld() FNV-1a fallback)
- RegisterProductionComponentTypes helper enrolls 14-17 production component types
  at bootstrap (deterministic, auditable, K-L4 fully realized)
- Factories receive both nativeWorld and world; dual-write loop writes to both
  (transition Phase 2-4; managed-side write removed Phase 5 commit 21)
- NativeWorldTestFixture shared fixture for Systems.Tests (per Q-COMBINED-6 lock)

Production code still reads via managed World API (legacy SystemBase.GetComponent)
until per-system migration begins (Phase 4 commit 9). Build green; tests pass.

Part of K8.3+K8.4 combined milestone Phase 2 commit 3 of 24.
```

**Commit 4: Mass production component type registration verification**

**Files modified**:
- Tests added: `tests/DualFrontier.Core.Interop.Tests/ProductionComponentRegistryTests.cs` (NEW)
  - Verify all 14-17 production component types are registered
  - Verify registry idempotency (re-registration returns existing id)
  - Verify type id stability across multiple bootstrap calls
  - Verify each component is `unmanaged` constraint compatible

**Commit shape**:
```
test(interop): production component registry verification tests

Verify Bootstrap.Run(registry) + RegisterProductionComponentTypes flow:
- All production component types successfully registered
- Type ids deterministic (re-registration idempotent)
- All component types satisfy unmanaged constraint (compile-time guarantee
  + runtime sanity check via Unsafe.SizeOf<T>)

This commit verifies Phase 2 commit 3 wiring works correctly at the unit-test
level before Phase 3 IModApi v3 surface lands.

Part of K8.3+K8.4 combined milestone Phase 2 commit 4 of 24.
```

### §6.4 — Phase 3: IModApi v3 surface (Commits 5-8)

**Commit 5: IModApi v3 interface (delete v2, ship v3 with constraint split)**

**Files modified**:
- `src/DualFrontier.Contracts/Modding/IModApi.cs`: replace v2 surface with v3 surface (specified in §0.3 lock body)
  - `RegisterComponent<T>` constraint changed from `where T : IComponent` to `where T : unmanaged, IComponent`
  - `RegisterManagedComponent<T> where T : class, IComponent` added
  - Rest preserved

**Breaking change**: any mod (vanilla or test fixture) that called `RegisterComponent<T>` with class type fails to compile. Vanilla mods are empty skeletons (M8.1 state); test fixtures need audit at this commit.

```powershell
# Identify call sites that may break
Get-ChildItem -Path mods/,tests/Fixture.* -Recurse -Filter *.cs | 
  Select-String -Pattern 'RegisterComponent<' -SimpleMatch
# For each match: verify the component type is `unmanaged`. If not, the mod 
# must rewrite to `RegisterManagedComponent` (and the type must gain [ManagedStorage])
```

Expected breakage volume at this commit: **near-zero** because vanilla mods are empty skeletons and test fixtures register either no components or known-unmanaged ones. Any genuine breakage indicates a previously-undetected Path β attempt; escalate per METHODOLOGY §3 if surfaced.

**Commit shape**:
```
feat(modding): IModApi v3 surface — RegisterComponent constraint split

Replace v2 IModApi with v3:
- RegisterComponent<T> where T : unmanaged, IComponent (Path α default, K-L3)
- RegisterManagedComponent<T> where T : class, IComponent (Path β bridge, K-L3.1)
- Fields, ComputePipelines (v1.6 additions) preserved
- Publish, Subscribe, Contract, GetKernelCapabilities, GetOwnManifest, Log preserved

No backward compatibility. v2 IModApi deleted entirely; mods that called 
RegisterComponent<T> with non-unmanaged type fail to compile. This is intentional — 
combined milestone is the architectural break point per K-L3.1 + Crystalka direction 
2026-05-13 «мы не будем поддерживать старый api так что удаление и новый».

Part of K8.3+K8.4 combined milestone Phase 3 commit 5 of 24.
```

**Commit 6: [ManagedStorage] attribute + ManagedStore<T> + ValidationErrorKind enum addition**

**Files created**:
- `src/DualFrontier.Contracts/Modding/ManagedStorageAttribute.cs` (specified in §4.5)
- `src/DualFrontier.Application/Modding/ManagedStore.cs` (specified in §4.3, with IManagedStore marker interface)

**Files modified**:
- `src/DualFrontier.Application/Modding/ValidationError.cs`: enum addition
  ```csharp
  public enum ValidationErrorKind
  {
      // ...existing 11 members preserved...
      MissingManagedStorageAttribute,   // NEW — raised by RegisterManagedComponent<T> when T lacks [ManagedStorage]
  }
  ```
  Expected enum members post-commit: 12 (11 existing + 1 new).

**Commit shape**:
```
feat(modding): [ManagedStorage] attribute + ManagedStore<T> + enum extension

- [ManagedStorage] attribute marks Path β class IComponent types (K-L3.1 bridge)
- ManagedStore<T> per-mod storage (Dictionary<EntityId, T>, single-threaded)
  + IManagedStore marker interface for type-erased Dictionary<Type, IManagedStore>
- ValidationErrorKind.MissingManagedStorageAttribute added (raised by
  RegisterManagedComponent<T> when T lacks [ManagedStorage] annotation)

Part of K8.3+K8.4 combined milestone Phase 3 commit 6 of 24.
```

**Commit 7: RestrictedModApi v3 implementation + IManagedStorageResolver wiring + ModRegistry resolver**

**Files modified**:
- `src/DualFrontier.Application/Modding/RestrictedModApi.cs`: 
  - Constraint update on `RegisterComponent<T>` (now requires `unmanaged, IComponent`)
  - `RegisterManagedComponent<T>` method body (specified in §4.1)
  - `_managedStores` field, `GetManagedStore<T>` accessor, `ClearManagedStores` method
- `src/DualFrontier.Application/Modding/IManagedStorageResolver.cs`: NEW (specified in §4.6)
- `src/DualFrontier.Application/Modding/ModRegistry.cs`: 
  - Implements `IManagedStorageResolver`
  - `RegisterRestrictedModApi(modId, api)` method
  - `UnregisterRestrictedModApi(modId)` method
  - `Resolve<T>(modId)` method
- `src/DualFrontier.Application/Modding/ModLoader.cs`: 
  - Add `modRegistry.RegisterRestrictedModApi(modId, api)` call after RestrictedModApi construction
  - Add `modRegistry.UnregisterRestrictedModApi(modId)` + `api.ClearManagedStores()` calls in UnloadMod step 5 (per MOD_OS §9.5 chain)
- `src/DualFrontier.Core/ECS/SystemExecutionContext.cs`: 
  - Constructor signature update (specified in §3.2): `World` parameter removed, `NativeWorld` required, `ComponentTypeRegistry` new required, `IManagedStorageResolver?` new optional
  - `ResolveManagedStore<T>` method body
  - Delete `GetComponent<T>`, `SetComponent<T>`, `Query<T>`, `Query<T1,T2>`, `GetSystem<TSystem>` methods
  - Delete isolation-guard internals if not needed (`_allowedReads`, `_allowedWrites` HashSets) per Option A in §3.2
- `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs`: 
  - Constructor signature update (specified in §4.6): `World` parameter removed, `NativeWorld` required, `ComponentTypeRegistry` new required, `IManagedStorageResolver?` new optional
  - `BuildContext` method updated to pass new ctor args to `SystemExecutionContext`
- `src/DualFrontier.Core/ECS/SystemBase.cs`: 
  - Delete `GetComponent`, `SetComponent`, `Query`, `Query<T1,T2>`, `GetSystem` methods
  - Add `ManagedStore<T>` accessor method (specified in §4.4)

**Breaking changes at this commit**: massive. All 12 production systems currently use `SystemBase.GetComponent` / `Query` etc. — they will not compile after this commit. **This commit deliberately breaks production systems** so Phase 4 per-system migration commits 9-20 can rewrite them one at a time.

**Critical**: this commit alone does NOT leave project in a green state. It is the **transition commit** between Phase 3 (Mod API v3) and Phase 4 (per-system migration). Build will fail after this commit until Phase 4 commit 20 (last system migrated) completes.

**Wait** — this violates the K8.1 lesson «atomic commit as compilable unit». Need to reconsider.

**Resolution**: split this commit. **Commit 7 ships only the IModApi v3 surface implementation** (RestrictedModApi, IManagedStorageResolver, ModRegistry, ModLoader changes). It does **not** delete SystemBase legacy API yet. Build remains green because legacy API still in place.

**SystemBase API deletion + SystemExecutionContext + scheduler refactor moved to Phase 5 commit 21** — the «World retirement» commit. Phase 4 per-system migrations rewrite system bodies one-by-one but **legacy SystemBase API remains available** (each migrated system simply stops using it; unmigrated systems continue to use it). Phase 5 commit 21 deletes legacy API once all 12 systems migrated.

**Revised Commit 7 scope**:
- IModApi v3 surface implementation in RestrictedModApi only
- IManagedStorageResolver + ModRegistry plumbing
- ModLoader integration (chain points for RegisterRestrictedModApi + UnregisterRestrictedModApi + ClearManagedStores)
- SystemBase.ManagedStore<T> accessor added (legacy GetComponent/Query/etc still present)
- SystemExecutionContext gains ResolveManagedStore<T> method (legacy methods still present)
- ParallelSystemScheduler ctor preserved (still accepts managed World)

Build green. Legacy SystemBase API still works. Mods using v2 IModApi `RegisterComponent<T>` without unmanaged constraint **do** fail to compile (per Commit 5 already), but production systems continue to work.

**Commit shape (revised)**:
```
feat(modding): RestrictedModApi v3 implementation + IManagedStorageResolver

- RestrictedModApi.RegisterManagedComponent<T> implementation with [ManagedStorage]
  attribute validation + ValidationErrorKind.MissingManagedStorageAttribute path
- IManagedStorageResolver interface for mod-id → ManagedStore<T> dispatch
- ModRegistry implements IManagedStorageResolver (per-mod RestrictedModApi map)
- ModLoader integration in UnloadMod step 5 (MOD_OS §9.5 chain extension):
  RegisterRestrictedModApi at load, UnregisterRestrictedModApi + ClearManagedStores
  at unload
- SystemBase.ManagedStore<T> accessor added (legacy SystemBase API preserved until
  Phase 5 commit 21)
- SystemExecutionContext.ResolveManagedStore<T> resolution path (legacy methods
  preserved until Phase 5 commit 21)

This commit adds new Path β surface without touching Path α access patterns. 
Production systems continue to use SystemBase.GetComponent legacy API until 
Phase 4 per-system migrations begin. Build green; tests pass.

Part of K8.3+K8.4 combined milestone Phase 3 commit 7 of 24.
```

**Commit 8: ModManifest.ManifestVersion field + ManifestParser v3-only acceptance + vanilla & fixture manifest updates**

**Files modified**:
- `src/DualFrontier.Contracts/Modding/ModManifest.cs`: add `ManifestVersion` field (specified in §5.1)
- `src/DualFrontier.Application/Modding/ManifestParser.cs`: add v3-only validation gate (specified in §5.2)
- 7 vanilla mod manifests: add `"manifestVersion": "3"` (specified in §5.3)
- ~19 test fixture manifests: add `"manifestVersion": "3"` (specified in §5.4)
- `tests/DualFrontier.Modding.Tests/ManifestVersionTests.cs`: NEW (specified in §5.5)

**Commit shape**:
```
feat(modding): ManifestVersion field + strict v3-only acceptance

- ModManifest.ManifestVersion field added (O-1 audit inline absorption)
- ManifestParser rejects any value other than "3" with
  ValidationErrorKind.IncompatibleContractsVersion at load time
- Vanilla mod manifests updated to "manifestVersion": "3" (7 files)
- Test fixture manifests updated (~19 files)
- ManifestVersionTests test class with 6-8 negative + positive coverage

No backward compatibility. v1/v2 manifests rejected at load time. No grace
period; no deprecation warnings. Per Crystalka direction 2026-05-13 «мы не
будем поддерживать старый api так что удаление и новый».

Part of K8.3+K8.4 combined milestone Phase 3 commit 8 of 24.
```

### §6.5 — Phase 4: per-system migration (Commits 9-20)

Each commit migrates one system from legacy `SystemBase.GetComponent` / `Query` API to direct `NativeWorld.AcquireSpan` / `BeginBatch` API. Pattern documented in §3.3.

**Commit template for each system migration**:

```
feat(systems): migrate <SystemName> to SpanLease/WriteBatch (K8.3 N/12)

- <SystemName>.Update body rewritten to use NativeWorld.AcquireSpan<T> for reads
  and NativeWorld.BeginBatch<T> for writes (K-L7 pattern fully realized)
- Legacy SystemBase.GetComponent/SetComponent/Query references removed from
  this system; legacy API itself preserved (deleted Phase 5 commit 21)
- Per-system test class <SystemName>Tests added/updated:
  - <test_count_added> new tests covering pre-migration behavior parity
  - NativeWorldTestFixture used for arrange/act/assert pattern
- Test count delta: +<N>

Verification:
- dotnet build green
- dotnet test green (existing N + N new = M tests pass)
- Native selftest green
- Manual smoke: F5 simulation runs without crash for 100 ticks

Part of K8.3+K8.4 combined milestone Phase 4 commit <X> of 24.
```

**Per-system commit specifics**:

#### Commit 9: ConsumeSystem (Tier 1)

**Files modified**:
- `src/DualFrontier.Systems/Pawn/ConsumeSystem.cs`: body rewrite per pattern in §3.3
- `tests/DualFrontier.Systems.Tests/Pawn/ConsumeSystemTests.cs`: test class created if absent, updated if present

**Components touched**: NeedsComponent (Satiety, Hydration facets) + ConsumableComponent + WaterSourceComponent + PositionComponent + ReservationComponent

**Patterns expected**:
- Read span over NeedsComponent (filter to those with low Satiety or Hydration)
- Per-low-need-pawn: find nearest ConsumableComponent or WaterSourceComponent entity (single-entity HasComponent check OK at low frequency)
- Write batch: update NeedsComponent (raise Satiety/Hydration) + destroy consumed entity

**Test count delta**: +3-5 tests

#### Commit 10: SleepSystem (Tier 1)

**Files modified**:
- `src/DualFrontier.Systems/Pawn/SleepSystem.cs`
- `tests/DualFrontier.Systems.Tests/Pawn/SleepSystemTests.cs`

**Components touched**: NeedsComponent (Sleep, Comfort facets) + BedComponent + PositionComponent + ReservationComponent

**Patterns expected**: similar to ConsumeSystem; hybrid Comfort restoration per userMemories spec (`ΔComfort = ΔSleep × 0.3` during bed sleep)

**Test count delta**: +3-5 tests

#### Commit 11: ComfortAuraSystem (Tier 1)

**Files modified**:
- `src/DualFrontier.Systems/Pawn/ComfortAuraSystem.cs`
- `tests/DualFrontier.Systems.Tests/Pawn/ComfortAuraSystemTests.cs`

**Components touched**: NeedsComponent (Comfort facet) + DecorativeAuraComponent + PositionComponent

**Patterns expected**: spatial scan from each pawn position for nearby aura sources; passive ambient Comfort restoration. May use `SpatialGrid` query infrastructure (verify existence at Phase 0).

**Test count delta**: +3-5 tests

#### Commit 12: MoodSystem (Tier 2)

**Files modified**:
- `src/DualFrontier.Systems/Pawn/MoodSystem.cs`
- `tests/DualFrontier.Systems.Tests/Pawn/MoodSystemTests.cs`

**Components touched**: NeedsComponent + MindComponent (read+write)

**Patterns expected**: read both NeedsComponent and MindComponent spans (Query<T1,T2> pattern per §3.3); compute new MoodValue; batch write MindComponent.

**Test count delta**: +3-5 tests

#### Commit 13: PawnStateReporterSystem (Tier 2)

**Files modified**:
- `src/DualFrontier.Systems/Pawn/PawnStateReporterSystem.cs`
- `tests/DualFrontier.Systems.Tests/Pawn/PawnStateReporterSystemTests.cs`

**Components touched**: read-only iteration over NeedsComponent + IdentityComponent + SkillsComponent + JobComponent + MindComponent + PositionComponent; publishes PawnStateChangedEvent

**Patterns expected**: multi-span acquisition; joining via entity id intersection per §3.3 Query<T1,T2> pattern (here 6 spans). Consider whether 6-span join is efficient or whether iteration shape needs restructuring — record decision in commit message.

**Test count delta**: +3-5 tests

#### Commit 14: JobSystem (Tier 3)

**Files modified**:
- `src/DualFrontier.Systems/Pawn/JobSystem.cs`
- `tests/DualFrontier.Systems.Tests/Pawn/JobSystemTests.cs`

**Components touched**: JobComponent (read+write) + ReservationComponent (read+write) + NeedsComponent (read) + PositionComponent (read)

**Patterns expected**: scan idle pawns (no JobComponent or stale JobComponent); assign new job based on need priorities + nearby reservation availability; write back JobComponent + ReservationComponent

**Test count delta**: +5-8 tests (JobSystem is the most complex Tier 3 system)

#### Commit 15: HaulSystem (Tier 3)

**Files modified**:
- `src/DualFrontier.Systems/Inventory/HaulSystem.cs`
- `tests/DualFrontier.Systems.Tests/Inventory/HaulSystemTests.cs`

**Components touched**: ReservationComponent (read+write) + StorageComponent (read+write) + PositionComponent (read)

**Patterns expected**: pawn-job-coordinated haul tasks; StorageComponent updates via NativeMap+NativeSet K8.1 primitive operations.

**Test count delta**: +3-5 tests

#### Commit 16: NeedsSystem (Tier 3)

**Files modified**:
- `src/DualFrontier.Systems/Pawn/NeedsSystem.cs`
- `tests/DualFrontier.Systems.Tests/Pawn/NeedsSystemTests.cs`

**Components touched**: NeedsComponent (read+write only)

**Patterns expected**: simplest hot path — acquire span over NeedsComponent, batch write decay updates for every pawn. Direct port of pattern shown in §3.3 «Generic system migration template».

**Test count delta**: +3-5 tests

#### Commit 17: ConverterSystem (Tier 4)

**Files modified**:
- `src/DualFrontier.Systems/Power/ConverterSystem.cs`
- `tests/DualFrontier.Systems.Tests/Power/ConverterSystemTests.cs`

**Components touched**: PowerConsumerComponent (read+write) + PowerProducerComponent (read+write)

**Patterns expected**: dual-span read + dual-batch write; power conversion math.

**Test count delta**: +3-5 tests

#### Commit 18: ElectricGridSystem (Tier 4)

**Files modified**:
- `src/DualFrontier.Systems/Power/ElectricGridSystem.cs`
- `tests/DualFrontier.Systems.Tests/Power/ElectricGridSystemTests.cs`

**Components touched**: PowerConsumerComponent (read+write) + PowerProducerComponent (read+write) + PositionComponent (read)

**Patterns expected**: spatial grid balancing; pull production toward consumers within range.

**Test count delta**: +3-5 tests

#### Commit 19: InventorySystem (Tier 4)

**Files modified**:
- `src/DualFrontier.Systems/Inventory/InventorySystem.cs`
- `tests/DualFrontier.Systems.Tests/Inventory/InventorySystemTests.cs`

**Components touched**: StorageComponent (heavy NativeMap+NativeSet operations) + ReservationComponent (read)

**Patterns expected**: StorageComponent has NativeMap (item type → count) and NativeSet (allowed item types). System operations on these are K8.1 primitive calls. Migration here is largely **swap span/batch wrapping** around existing K8.1-primitive operations rather than algorithmic rewrite.

**Test count delta**: +3-5 tests

#### Commit 20: MovementSystem (Tier 5)

**Files modified**:
- `src/DualFrontier.Systems/Pawn/MovementSystem.cs`
- `tests/DualFrontier.Systems.Tests/Pawn/MovementSystemTests.cs`

**Components touched**: MovementComponent (read+write NativeComposite path) + PositionComponent (write)

**Patterns expected**: consume NativeComposite path step-by-step; write PositionComponent updates. This is the most complex Tier 5 system because of NativeComposite mechanics + NavGrid path coordination.

**Test count delta**: +5-8 tests

**Phase 4 closure check after commit 20**:
```powershell
# Verify no system body still references legacy SystemBase API
Select-String -Path src/DualFrontier.Systems/ -Pattern 'GetComponent<|SetComponent<|Query<' -SimpleMatch -Recurse
# Expected: zero matches — all 12 systems migrated to NativeWorld API

# Verify dual-write factories still active (Phase 5 commit 21 removes)
Select-String -Path src/DualFrontier.Application/Scenario/ -Pattern 'world\.AddComponent' -SimpleMatch -Recurse
# Expected: matches present (factories still dual-writing)
```

### §6.6 — Phase 5: World retirement (Commits 21-22)

**Commit 21: SystemBase legacy API deleted + SystemExecutionContext refactored + scheduler signature updated + factory dual-write removed**

This is the **load-bearing cutover commit**. After this commit, managed `World` has no readers and no writers. Class becomes dead code.

**Files modified**:
- `src/DualFrontier.Core/ECS/SystemBase.cs`: delete legacy `GetComponent`, `SetComponent`, `Query`, `Query<T1,T2>`, `GetSystem` methods
- `src/DualFrontier.Core/ECS/SystemExecutionContext.cs`: 
  - Constructor signature update (specified in §3.2): `World` parameter removed, `NativeWorld` required, `ComponentTypeRegistry` required, `IManagedStorageResolver?` optional
  - Delete legacy method bodies (`GetComponent`, `SetComponent`, `Query`, etc.)
  - Delete `_world` field
  - Preserve `_nativeWorld` field as required
  - Optionally delete `_allowedReads` / `_allowedWrites` HashSets per Option A in §3.2 (lock chose Option A)
- `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs`: 
  - Constructor signature update (specified in §4.6 end)
  - Delete `_world` field; preserve `_nativeWorld`, `_registry`, `_managedStorageResolver`
  - `BuildContext` method updated
- `src/DualFrontier.Application/Loop/GameBootstrap.cs`: 
  - Remove `var world = new World();` line entirely
  - Update factory calls: `pawnFactory.Spawn(nativeWorld, services, count)` (drop `world` parameter)
  - Update factory calls: `itemFactory.Spawn(nativeWorld, excludedPositions, ...)` (drop `world` parameter; `excludedPositions` collection-builder pre-loop iterates via NativeWorld instead)
  - Update `PublishItemSpawnedEvents` helper: replace `world.GetEntitiesWith<T>()` with NativeWorld span iteration
  - Update embedded comment block: remove «K8.2 v2» comment; remove «K8.4 lights up» comment; new comment describes post-cutover state: «NativeWorld is the sole production storage path per K-L11. ComponentTypeRegistry is active; type ids are deterministic.»
  - Update scheduler construction: pass `nativeWorld`, `registry`, `modRegistry` instead of `world`, `nativeWorld`
- `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs`: 
  - Constructor signature: drop `nativeWorld` parameter (now passed to Spawn)
  - `Spawn` signature: `Spawn(NativeWorld nativeWorld, IGameServices services, int count)` — `World world` parameter deleted
  - Body: delete dual-write loop; native-only writes
- `src/DualFrontier.Application/Scenario/ItemFactory.cs`: similar dual-write removal
- Tests in `tests/DualFrontier.Core.Tests/` that exercised `SystemExecutionContext`-via-`World`: rewrite to use `ManagedTestWorld` (anticipating Phase 5 commit 22) OR rewrite to use `NativeWorld` test fixture (preferred for exercising post-cutover paths)

**Breaking changes at this commit**: substantial. Every consumer of legacy `SystemBase.GetComponent` / `Query` etc. (none remaining in `src/DualFrontier.Systems/` per Phase 4 closure check, but possibly some in tests) breaks. Test files that referenced these methods need rewriting before this commit can build green.

**Critical execution discipline**: this commit must be **atomic and complete**. Partial completion leaves project in broken state. Cloud Code agent should:
1. Identify all consumer call sites via grep at start of commit work
2. Plan the full edit set before applying any individual edit
3. Apply edits in dependency order (delete consumers first, then producers)
4. Run `dotnet build` after every ~3-5 file edits to catch issues early
5. Final `dotnet build` clean + `dotnet test` 681+ pass before committing

**Commit shape**:
```
refactor(core): retire managed World as production storage path (K-L11 fully realized)

This is the load-bearing cutover commit of K8.3+K8.4 combined milestone.

Deletions:
- SystemBase.GetComponent<T>, SetComponent<T>, Query<T>, Query<T1,T2>, GetSystem
- SystemExecutionContext legacy method bodies + _world field
- ParallelSystemScheduler _world field
- Factory dual-write loops (managed-side write removed; native-only writes)
- Isolation guard runtime checks (_allowedReads/_allowedWrites HashSets); compile-
  time enforcement via [SystemAccess] + future A'.9 analyzer milestone

Signature changes:
- SystemExecutionContext ctor: World param removed, NativeWorld required,
  ComponentTypeRegistry required, IManagedStorageResolver? optional
- ParallelSystemScheduler ctor: same updates
- RandomPawnFactory.Spawn: drop World param, drop dual-write loop
- ItemFactory.Spawn: same
- GameBootstrap.CreateLoop: drop `new World()`, drop world parameter from factory
  calls and scheduler call; comment block updated

Post-commit: managed World class has no readers and no writers. Class exists
on disk as dead code; Phase 5 commit 22 physically moves it to test project.

Verification: dotnet build green, dotnet test 681+ pass, native selftest green,
F5 manual smoke: simulation runs without crash for 100 ticks.

Part of K8.3+K8.4 combined milestone Phase 5 commit 21 of 24.
```

**Commit 22: World class moved to test project as ManagedTestWorld**

**Files moved**:
- `src/DualFrontier.Core/ECS/World.cs` → `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs`
- `src/DualFrontier.Core/ECS/ComponentStore.cs` → `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestComponentStore.cs`
- `src/DualFrontier.Core/ECS/World.cs.uid` → deleted (orphan after move)
- `src/DualFrontier.Core/ECS/ComponentStore.cs.uid` → deleted

**Content modifications during move**:
- Visibility: `internal sealed class World` → `public sealed class ManagedTestWorld`
- Namespace: `DualFrontier.Core.ECS` → `DualFrontier.Core.Tests.Fixtures`
- XML doc updated per §2.4
- `internal` methods on `World` become `public` or `internal` to `ManagedTestWorld` per test-fixture appropriateness
- Same modifications to `ComponentStore` → `ManagedTestComponentStore`

**Test file mass rename**:
```powershell
# Identify all test files referencing World
Get-ChildItem -Path tests/DualFrontier.Core.Tests/ -Recurse -Filter *.cs |
  Select-String -Pattern '\bWorld\b' -SimpleMatch |
  Group-Object Filename |
  ForEach-Object { $_.Name }
# Expected: ~50-100 files

# Apply replacement per file (verify each before commit)
# Pattern: `new World(` → `new ManagedTestWorld(`
# Pattern: `World ` (as type) → `ManagedTestWorld `
# Pattern: `, World w` (param) → `, ManagedTestWorld w`
# Pattern: `World w =` (local var) → `ManagedTestWorld w =`
```

**`InternalsVisibleTo` cleanup**:
```powershell
# DualFrontier.Core no longer needs to grant InternalsVisibleTo to Core.Tests for World access
Select-String -Path src/DualFrontier.Core/DualFrontier.Core.csproj -Pattern 'InternalsVisibleTo'
# Verify grants; remove if World-specific; preserve if other internal access needed
```

**Verification**:
```powershell
dotnet build src/DualFrontier.sln
# Expected: green (production code doesn't reference World class anywhere)

dotnet test src/DualFrontier.sln
# Expected: 681+ pass (ManagedTestWorld replaces World in legacy Core.Tests; 
# no test behavior changes — fixture renaming only)

# Grep verification: no production reference to World
Select-String -Path src/ -Pattern '\bnew World\(' -SimpleMatch -Recurse
# Expected: zero matches

Select-String -Path src/ -Pattern ': World\b' -SimpleMatch -Recurse
# Expected: zero matches
```

**Commit shape**:
```
refactor(core,tests): move World class to test project as ManagedTestWorld (K-L11)

Managed World deleted from production assemblies; moved to test project as
ManagedTestWorld per KERNEL_ARCHITECTURE.md K-L11 «test fixture and research
reference».

File moves:
- src/DualFrontier.Core/ECS/World.cs → tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs
- src/DualFrontier.Core/ECS/ComponentStore.cs → tests/DualFrontier.Core.Tests/Fixtures/ManagedTestComponentStore.cs
- .uid companions deleted

Content modifications:
- Visibility: internal sealed → public sealed (test fixture accessibility)
- Namespace: DualFrontier.Core.ECS → DualFrontier.Core.Tests.Fixtures
- XML docs updated to reflect K-L11 role

Test file mass rename: ~50-100 files in DualFrontier.Core.Tests update
`new World(` → `new ManagedTestWorld(`, `World w` → `ManagedTestWorld w`,
and similar patterns. Mechanical edits; no test behavior changes.

InternalsVisibleTo grants cleaned up (DualFrontier.Core no longer needs to
expose World internals).

Verification: dotnet build green, dotnet test 681+ pass, grep src/ for
`new World(` returns zero matches.

Part of K8.3+K8.4 combined milestone Phase 5 commit 22 of 24.
```

### §6.7 — Phase 6: closure (Commits 23-24)

**Commit 23: Documentation amendments (MIGRATION_PLAN v1.2 + KERNEL v1.6 + MOD_OS v1.8 + PHASE_A_PRIME_SEQUENCING structural amendment)**

Details per §9.2 (Version bumps + amendments).

**Commit 24: MIGRATION_PROGRESS closure entry + REGISTER.yaml updates**

Details per §9.1-§9.5 (lifecycle transitions, audit trail, CAPA closures, REQ promotions, final sync gate).

Optional bootstrap backfill commit for self-referential `last_modified_commit` per FRAMEWORK §8.3 (Option B preferred per A'.4.5 lesson — separate commit cleaner than amend which orphans register self-reference).

---

## §7 — Test strategy

### §7.1 — Per-system test classes

Q-COMBINED-6 lock: Option A from K8.2 v2 / K8.3 v2.0 precedent. Per-system / per-component test class. Existing test classes preserved if present; missing ones authored per-system migration commit.

**Expected test class inventory post-combined**:

```
tests/DualFrontier.Systems.Tests/
├── Fixtures/
│   └── NativeWorldTestFixture.cs                    (NEW — Phase 2 commit 3)
├── Pawn/
│   ├── NeedsSystemTests.cs                          (existing or NEW Phase 4 commit 16)
│   ├── ConsumeSystemTests.cs                        (existing or NEW Phase 4 commit 9)
│   ├── SleepSystemTests.cs                          (existing or NEW Phase 4 commit 10)
│   ├── ComfortAuraSystemTests.cs                    (existing or NEW Phase 4 commit 11)
│   ├── MoodSystemTests.cs                           (existing or NEW Phase 4 commit 12)
│   ├── PawnStateReporterSystemTests.cs              (existing or NEW Phase 4 commit 13)
│   ├── MovementSystemTests.cs                       (existing or NEW Phase 4 commit 20)
│   └── JobSystemTests.cs                            (existing or NEW Phase 4 commit 14)
├── Inventory/
│   ├── InventorySystemTests.cs                      (existing or NEW Phase 4 commit 19)
│   └── HaulSystemTests.cs                           (existing or NEW Phase 4 commit 15)
├── Power/
│   ├── ConverterSystemTests.cs                      (existing or NEW Phase 4 commit 17)
│   └── ElectricGridSystemTests.cs                   (existing or NEW Phase 4 commit 18)
└── Modding/
    ├── IModApiV3Tests.cs                            (NEW — Phase 3 commits 5-7 cumulative)
    └── ManifestVersionTests.cs                      (NEW — Phase 3 commit 8)
```

At Phase 0 inventory, execution agent verifies which test classes already exist. For each missing one, the per-system migration commit authors it. For each existing one, the migration commit updates the arrange/act/assert to use `NativeWorldTestFixture` instead of `new World()`.

### §7.2 — NativeWorldTestFixture

File creation (Phase 2 commit 3): `tests/DualFrontier.Systems.Tests/Fixtures/NativeWorldTestFixture.cs`

```csharp
using System;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using DualFrontier.Application.Services;

namespace DualFrontier.Systems.Tests.Fixtures;

/// <summary>
/// xUnit class fixture providing a fresh ComponentTypeRegistry + NativeWorld
/// + GameServices stack for per-system tests. All production component types
/// pre-registered to match GameBootstrap.RegisterProductionComponentTypes
/// behavior.
///
/// Usage:
/// <code>
/// public sealed class MySystemTests : IClassFixture&lt;NativeWorldTestFixture&gt;
/// {
///     private readonly NativeWorldTestFixture _fixture;
///     public MySystemTests(NativeWorldTestFixture fixture) { _fixture = fixture; }
///
///     [Fact]
///     public void MyTest()
///     {
///         var entity = _fixture.NativeWorld.CreateEntity();
///         _fixture.NativeWorld.AddComponent(entity, new MyComponent { Value = 42 });
///         var system = new MySystem();
///         system.Update(deltaTime: 1f);
///         Assert.Equal(43, _fixture.NativeWorld.GetComponent&lt;MyComponent&gt;(entity).Value);
///     }
/// }
/// </code>
///
/// xUnit class fixture lifetime: one instance per test class; shared across
/// all test methods in that class. Per-method test isolation requires fixture
/// reset between methods — use [Fact] with arrange-act-assert pattern that
/// creates fresh entities and disposes them via NativeWorld.DestroyEntity
/// (assertion-side state cleanup) rather than fixture reconstruction.
/// </summary>
public sealed class NativeWorldTestFixture : IDisposable
{
    public NativeWorld NativeWorld { get; }
    public ComponentTypeRegistry Registry { get; }
    public GameServices Services { get; }
    
    public NativeWorldTestFixture()
    {
        Registry = new ComponentTypeRegistry();
        NativeWorld = Bootstrap.Run(Registry);
        Services = new GameServices();
        
        // Match production registration list per GameBootstrap.RegisterProductionComponentTypes
        // (verified at Phase 0 inventory; list refreshed if drift surfaces)
        Registry.Register<ConsumableComponent>();
        Registry.Register<WaterSourceComponent>();
        Registry.Register<BedComponent>();
        Registry.Register<DecorativeAuraComponent>();
        Registry.Register<NeedsComponent>();
        Registry.Register<JobComponent>();
        Registry.Register<ReservationComponent>();
        Registry.Register<MindComponent>();
        Registry.Register<PowerConsumerComponent>();
        Registry.Register<PowerProducerComponent>();
        Registry.Register<StorageComponent>();
        Registry.Register<MovementComponent>();
        Registry.Register<PositionComponent>();
        Registry.Register<IdentityComponent>();
        Registry.Register<SkillsComponent>();
    }
    
    public void Dispose()
    {
        NativeWorld.Dispose();
    }
}
```

Note: this fixture is **separate** from any existing test fixture in `DualFrontier.Core.Tests`. The Core.Tests project tests low-level ECS primitives (managed `World` via `ManagedTestWorld` post-Phase 5 commit 22); the Systems.Tests project tests system behavior end-to-end on production-equivalent `NativeWorld` stack.

### §7.3 — ManagedTestWorld for legacy Core.Tests

Phase 5 commit 22 movements (specified in §2.4 + §6.6 commit 22).

`tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs` becomes the test-fixture replacement for the deleted `src/DualFrontier.Core/ECS/World.cs`. Content is verbatim copy with visibility/namespace adjustments.

File content (full):
```csharp
using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Tests.Fixtures;

/// <summary>
/// Managed in-memory ECS used as a test fixture and research reference per
/// KERNEL_ARCHITECTURE.md K-L11 «managed-side reference implementation
/// retained as test fixture and research reference».
/// 
/// History:
/// - Pre-K0: was the production ECS implementation
/// - K0…K8.2 v2: continued as production storage backbone while NativeWorld
///   capabilities were built up (K0→K8.1 primitives, K8.2 struct conversions)
/// - K8.3+K8.4 combined milestone: moved to test project as ManagedTestWorld
///   when NativeWorld + ComponentTypeRegistry + Bootstrap.Run path became the
///   sole production storage path per K-L11
/// 
/// Surface preserves managed World public API (CreateEntity, AddComponent,
/// GetComponent, RemoveComponent, GetComponentUnsafe, Query, GetEntitiesWith,
/// etc.) for legacy test reuse without rewriting ~472 test files.
/// 
/// Production code does NOT reference this class. Grep verification: 
/// `Select-String -Path src/ -Pattern \"ManagedTestWorld\" -Recurse` returns zero matches.
/// </summary>
public sealed class ManagedTestWorld
{
    // ...verbatim content of pre-combined World.cs, with namespace + visibility adjustments...
}
```

`ManagedTestComponentStore` follows same pattern.

### §7.4 — IModApi v3 surface tests

File creation (Phase 3 commits 5-7 cumulative; specific commit per test): `tests/DualFrontier.Modding.Tests/IModApiV3Tests.cs`

```csharp
namespace DualFrontier.Modding.Tests;

public sealed class IModApiV3Tests : IClassFixture<NativeWorldTestFixture>
{
    private readonly NativeWorldTestFixture _fixture;
    
    public IModApiV3Tests(NativeWorldTestFixture fixture) { _fixture = fixture; }
    
    // Test 1: Path α registration succeeds for unmanaged struct
    [Fact]
    public void RegisterComponent_UnmanagedStruct_Succeeds() { /* ... */ }
    
    // Test 2: Path α registration via reflection-based fixture (compile-time
    //         test would require source generator; verify runtime check works)
    [Fact]
    public void RegisterComponent_StrengthenedConstraint_RejectsClassReflectively()
    {
        // Cannot directly express compile error in xUnit; verify via reflection
        // that the constraint IS unmanaged on the method's generic parameter.
        var method = typeof(IModApi).GetMethod("RegisterComponent")!;
        var typeParam = method.GetGenericArguments()[0];
        var constraints = typeParam.GetGenericParameterConstraints();
        Assert.Contains(constraints, c => c == typeof(ValueType));  // unmanaged implies struct
    }
    
    // Test 3: Path β registration succeeds for [ManagedStorage] class
    [Fact]
    public void RegisterManagedComponent_WithManagedStorageAttribute_Succeeds() { /* ... */ }
    
    // Test 4: Path β registration without [ManagedStorage] raises validation error
    [Fact]
    public void RegisterManagedComponent_WithoutManagedStorageAttribute_RaisesValidationException() { /* ... */ }
    
    // Test 5: ManagedStore<T> persists across system invocations within mod lifetime
    [Fact]
    public void ManagedStore_PersistsAcrossSystemInvocations() { /* ... */ }
    
    // Test 6: ManagedStore<T> cleared on ALC.Unload (via UnloadMod chain)
    [Fact]
    public void ManagedStore_ClearedOnModUnload() { /* ... */ }
    
    // Test 7: SystemBase.ManagedStore<T> returns owning mod's store
    [Fact]
    public void SystemBase_ManagedStore_ReturnsOwningModsStore() { /* ... */ }
    
    // Test 8: SystemBase.ManagedStore<T> returns null for unregistered type
    [Fact]
    public void SystemBase_ManagedStore_ReturnsNullForUnregisteredType() { /* ... */ }
    
    // Test 9: Core systems (no modId) get null from SystemBase.ManagedStore<T>
    [Fact]
    public void SystemBase_ManagedStore_ReturnsNullForCoreSystemOrigin() { /* ... */ }
    
    // Test 10: Multiple mods get isolated ManagedStore<T> instances
    [Fact]
    public void ManagedStore_IsolatedAcrossMods() { /* ... */ }
    
    // Test 11: Fields v1.6 capability preserved through v3 surface
    [Fact]
    public void IModApiV3_FieldsAccessor_Preserved() { /* ... */ }
    
    // Test 12: ComputePipelines v1.6 capability preserved through v3 surface
    [Fact]
    public void IModApiV3_ComputePipelinesAccessor_Preserved() { /* ... */ }
}
```

~12 tests covering IModApi v3 surface contract.

### §7.5 — Manifest version tests

Already specified in §5.5 (~6-8 tests in `tests/DualFrontier.Modding.Tests/ManifestVersionTests.cs`).

### §7.6 — Test count expectation

**Baseline** at HEAD `efd67df` (post-A'.4.5 + K9 closure + halt resolution): **631+ tests**.

**Combined milestone deltas**:
- 12 per-system migration commits each add 3-8 tests → +40-80 new system-behavior tests
- IModApi v3 surface tests: +12
- Manifest version tests: +6-8
- Storage migration foundation tests (Phase 2 commit 4): +3-5 (ProductionComponentRegistryTests)
- Isolation guard tests deleted at Phase 5 commit 21: −5 to −15 (per §3.2 Option A decision)
- Net: **+50-90 tests**

**Post-combined target**: 680-790 tests.

Execution agent records actual count at closure entry; deviation from expected range >20% triggers METHODOLOGY §3 escalation (unexpected test breakage or unexpected new test surface).

---

## §8 — Stop conditions

### §8.1 — Hard gates (workspace corruption signals)

At **any point** during execution, if any of these conditions surface:

- **HG-A1**: `dotnet build` fails AND build error message references a file the current commit did not modify
  - Indicates: workspace state corrupted (rebase artifact, merge conflict, untracked file)
  - Action: HALT. Run `git status`, identify divergence, restore via `git stash` or `git checkout -- <file>` as appropriate. Do not commit forward until clean.

- **HG-A2**: `dotnet test` fails for tests not modified by the current commit and not in scope of any system migrated to date in this milestone
  - Indicates: unintended regression from earlier commits in this milestone
  - Action: HALT. Bisect among milestone commits to identify root cause. Apply fix as new commit (not amend; preserves milestone audit trail).

- **HG-A3**: Native build fails OR `df_core_native_selftest` fails
  - Indicates: K8.1 era invariant broken, scope outside combined milestone
  - Action: HALT. This is a K8.1-tier issue and should not surface during K8.3+K8.4 — if it does, the workspace is fundamentally broken.

- **HG-A4**: `sync_register.ps1 --validate` fails at any closure-protocol step (Phase 6 commits 23-24)
  - Indicates: register state inconsistent with documents on disk
  - Action: HALT. Identify divergence (FRAMEWORK §8.3 bypass invocation procedure). Repair register before continuing closure.

- **HG-A5**: A K-Lxx implication paragraph in KERNEL_ARCHITECTURE.md proves materially wrong during execution (e.g., K-L11 «retained as test fixture» turns out to depend on grants the cutover removes)
  - Indicates: architectural authority document mis-anticipates implementation reality
  - Action: HALT. Author HALT_REPORT.md per K8.3 v2.0 halt precedent. Escalate to deliberation mode for architectural reassessment.

### §8.2 — Informational checks (record + continue)

- **IC-B1**: Test count delta exceeds +90 or falls below +50
  - Record actual count in closure entry; deviation explained in commit message

- **IC-B2**: LOC delta substantially different from estimate (~ −2000 / +1500 net expected)
  - Record in closure entry

- **IC-B3**: A per-system migration commit (Phase 4) surfaces a behavior regression in its system that requires algorithmic change rather than mechanical port
  - Record decision in commit message; preserve original algorithm where feasible; note any intentional behavior delta

- **IC-B4**: Dual-write phase reveals a component type that needs additional registration not anticipated
  - Add to `RegisterProductionComponentTypes` helper; no scope creep concern (component-list growth is expected)

- **IC-B5**: Fixture manifest update at Phase 3 commit 8 reveals fixture that intentionally tests v1/v2 rejection
  - Preserve fixture as-is; document in commit message that fixture is **expected to reject** under new parser

### §8.3 — Architectural surprises (escalate per METHODOLOGY §3)

Situations that warrant escalation to deliberation mode mid-execution:

- **AS-C1**: A K-Lxx implication proves to depend on assumptions the brief did not surface (analogous to K8.3 v2.0 halt rationale)
  - Examples: K-L11 grants insufficient for ManagedTestWorld test reuse; K-L3.1 bridge implementation differs from brief's specified resolver pattern; K-L7 SpanLease/WriteBatch contract incompatible with a specific system's read-modify-write pattern
  - Action: HALT. Author HALT_REPORT in `docs/scratch/A_PRIME_5_CONTINUED/`. Crystalka authors brief refresh patch in deliberation mode. Resume execution against patch.

- **AS-C2**: A production component type discovered at Phase 0 inventory that is not in this brief's list (§2.1 RegisterProductionComponentTypes)
  - Record discovery in Phase 0 audit; add to list if compatible with `unmanaged` constraint; escalate if class type (Path β candidate — needs explicit `[ManagedStorage]` decision)

- **AS-C3**: A production system discovered at Phase 0 inventory that is not in this brief's 12-system list
  - Inventory count drift; record exact count discovered; if discrepancy >2, escalate per METHODOLOGY §3 (suggests systemic count error in brief authoring)

- **AS-C4**: A test fixture or test pattern reveals legacy `SystemBase.GetComponent` usage that this brief assumed didn't exist outside production systems
  - Record exact count and locations in commit message; Phase 5 commit 21 rewrites them per per-system migration patterns

- **AS-C5**: An MOD_OS architectural section discovered to be ambiguous regarding Path β unload chain ordering (when does `ClearManagedStores` run relative to `UnsubscribeAll`?)
  - Action: HALT and reference MOD_OS_ARCHITECTURE.md §9.5 for canonical chain order. If section ambiguous or contradictory, escalate.

---

## §9 — Closure protocol (REGISTER updates + docs amendments)

Full METHODOLOGY §12.7 closure protocol exercised. First post-A'.4.5 milestone closure (per CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT effectiveness verification). Each step below maps to a Phase 6 commit (23 or 24) action.

### §9.1 — Document lifecycle transitions (Phase 6 commit 24)

Update `docs/governance/REGISTER.yaml` entries:

```yaml
# 1. SUPERSEDED transitions (3 entries)
- id: DOC-D-K8_3
  category: D
  tier: 3
  lifecycle: SUPERSEDED                     # ← was AUTHORED
  owner: Crystalka
  version: "2.0"
  next_review_due: null
  superseded_by: DOC-D-K8_34_COMBINED       # ← NEW field
  superseded_at: "2026-XX-XX"               # ← NEW field (closure date)
  superseded_commit: "<commit-24-sha>"      # ← NEW field
  register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_3

- id: DOC-D-K8_3_BRIEF_REFRESH_PATCH
  category: D
  tier: 3
  lifecycle: SUPERSEDED                     # ← was AUTHORED
  owner: Crystalka
  version: "1.0"
  next_review_due: null
  superseded_by: DOC-D-K8_34_COMBINED
  superseded_at: "2026-XX-XX"
  superseded_commit: "<commit-24-sha>"
  register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_3_BRIEF_REFRESH_PATCH

- id: DOC-D-K8_4
  category: D
  tier: 3
  lifecycle: SUPERSEDED                     # ← was AUTHORED (skeleton)
  owner: Crystalka
  version: "0.1"
  next_review_due: null
  superseded_by: DOC-D-K8_34_COMBINED
  superseded_at: "2026-XX-XX"
  superseded_commit: "<commit-24-sha>"
  register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_4

# 2. EXECUTED transition (1 entry — this brief)
- id: DOC-D-K8_34_COMBINED
  category: D
  tier: 3
  lifecycle: EXECUTED                       # ← was AUTHORED (set at brief commit; promoted at closure)
  owner: Crystalka
  version: "1.0"
  authored_at: "2026-05-13"
  executed_at: "2026-XX-XX"                 # ← NEW field (closure date)
  execution_commit_range: "<commit-1-sha>..<commit-24-sha>"  # ← NEW field
  supersedes:
    - DOC-D-K8_3
    - DOC-D-K8_3_BRIEF_REFRESH_PATCH
    - DOC-D-K8_4
  next_review_due: null
  register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_34_COMBINED
```

Verification:
```powershell
./tools/governance/sync_register.ps1 --validate
# Expected: exit 0; superseded_by + supersedes cross-references resolve;
# no orphan lifecycle transitions
```

### §9.2 — Version bumps + amendments

#### §9.2.1 — MIGRATION_PLAN_KERNEL_TO_VANILLA.md v1.1 → v1.2 (Phase 6 commit 23)

Header update:
```markdown
# MIGRATION_PLAN_KERNEL_TO_VANILLA.md

**Version**: 1.2  
**Status**: LOCKED  
**Last revised**: 2026-XX-XX (K8.3+K8.4 combined milestone closure)  
**Previous version**: 1.1 (2026-05-08, K-L3.1 bridge formalization)  
```

**§0.1 sequence diagram amendment**:
```markdown
Phase A' (architectural) timeline:

  A'.0  K-L3.1 bridge formalization (2026-05-10 — commit 45d831c)
  A'.0.5  Reorganization + refresh (2026-05-11 — commit XXX)
  A'.0.7  METHODOLOGY restructure (2026-05-11 — commit XXX)
  A'.1   Amendment execution (2026-05-11 — commit XXX)
  A'.2   K-L3.1 amendment plan (2026-05-11 — commit XXX)
  A'.3   K9 field storage (2026-05-11 — commit XXX)
  A'.4   K-Lessons batch (2026-05-12 — commit XXX)
  A'.4.5 Document Control Register (2026-05-12 — commits 411c284..056579f)
  A'.5   K8.3+K8.4 combined kernel cutover (2026-XX-XX — this milestone) [REVISED]
  A'.6   K8.5 mod ecosystem migration prep (downstream)
  A'.7   REMOVED (combined into A'.5)
  A'.8   K-closure report (downstream)
  A'.9   Roslyn analyzer (downstream)
```

**§0.3 Decision #1 amendment**:
```markdown
## Decision #1: K-track and M-track non-interleave preserved (refined)

Original Decision #1 (2026-05-08): K-track milestones execute sequentially; 
M-track milestones do not interleave with K-track.

Refinement (2026-XX-XX, K8.3+K8.4 closure): Decision #1's «no interleaving» 
refers to K-track vs. M-track sequencing. **Internal K-series milestone 
bundling is permissible when architectural coupling demands it** — the 
K8.3+K8.4 combined milestone is the precedent example. Splitting K8.3 and 
K8.4 created a facade pattern (intermediate state with dual-write factory + 
dual-storage-path systems) that bundling eliminates. Atomic execution 
prevents premise-miss errors of the kind that triggered K8.3 v2.0 halt.

Refinement does not affect M-track interleaving prohibition. K-track and 
M-track remain non-interleaved.
```

**§1.2 + §1.3 merged into new §1.2**:
```markdown
## §1.2 K8.3+K8.4 combined: storage migration + system migration + Mod API v3

**Status**: Combined milestone (2026-XX-XX closure). Original K8.3 and K8.4 
entries below preserved as historical reference; their substantive content is 
absorbed into this section.

**Combined scope**:
- (Original K8.4) Storage migration: managed `World` → `NativeWorld` via 
  `ComponentTypeRegistry` + `Bootstrap.Run` activation
- (Original K8.3) System migration: 12 production systems rewritten to 
  `SpanLease<T>` / `WriteBatch<T>` access pattern
- (Cross-cutting) Mod API v3 surface: `RegisterComponent<T> where T : unmanaged, 
  IComponent` + `RegisterManagedComponent<T> where T : class, IComponent` + 
  `Fields` + `ComputePipelines` (v1.6 additions preserved)
- (Cross-cutting) `[ManagedStorage]` attribute + `ManagedStore<T>` data 
  structure + `IManagedStorageResolver` + `ValidationErrorKind.MissingManagedStorageAttribute`
- (Cross-cutting) `ModManifest.ManifestVersion` field (O-1 audit absorption); 
  ManifestParser accepts only `manifestVersion: "3"`
- (Cross-cutting) Managed `World` class moved to `tests/DualFrontier.Core.Tests/
  Fixtures/ManagedTestWorld.cs` per K-L11
- (Cross-cutting) Orphan `.uid` cleanup (3 files)

**No backward compatibility**. v1/v2 IModApi deleted; v1/v2 manifests rejected. 
Clean break per Crystalka direction 2026-05-13 + Q-COMBINED locks.

**Bundling rationale**: storage migration and system migration architecturally 
couple via shared dependency on legacy `SystemBase.GetComponent`/`Query` API. 
Splitting requires facade pattern; combining eliminates it. K8.3 v2.0 halt 
resolution (2026-05-13) revealed the premise-miss class of errors that bundling 
structurally prevents — atomic execution makes «storage stays through K8.3» 
obsolete because K8.3 doesn't exist as separate milestone.

**Original K8.3 reference** (now historical, executed at closure of this combined 
milestone): production system migration to SpanLease/WriteBatch. K8.3 v2.0 brief 
preserved on disk as SUPERSEDED artifact for historical traceability. 
Migration plan v1.1's K8.3 section is now reference history; this v1.2 section 
is authoritative.

**Original K8.4 reference** (now historical, executed at closure of this combined 
milestone): managed World retirement. K8.4 skeleton brief preserved on disk as 
SUPERSEDED artifact. Migration plan v1.1's K8.4 section is now reference 
history; this v1.2 section is authoritative.
```

**§1.4 K8.5 section**: minor wording cleanup («K8.5 absorbs World retirement from old K8.4» → «K8.5 mod ecosystem migration prep — original Migration Plan v1.0 scope restored after combined milestone»).

**§6 amendment requirements**: append:
```markdown
### §6.7 Combined K8.3+K8.4 amendment execution

Applied at K8.3+K8.4 combined milestone closure (2026-XX-XX):
- This document version 1.1 → 1.2
- KERNEL_ARCHITECTURE.md v1.5 → v1.6
- MOD_OS_ARCHITECTURE.md v1.7 → v1.8
- PHASE_A_PRIME_SEQUENCING.md structural amendment (Phase A' sequence diagram 
  + duration estimate table + amendment cascade list)
- MIGRATION_PROGRESS.md closure entry
- 6 REQ verification status promotions PENDING → VERIFIED (REQ-K-L3, REQ-K-L4, 
  REQ-K-L8, REQ-K-L11, REQ-Q-A45-X5, REQ-K-L3.1)
- 2 CAPA closures (CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT, CAPA-2026-05-13-K8.3-PREMISE-MISS)

Amendment scope absorbs work previously planned for §6.5 (K8.4) + §6.6 (K8.5 
World retirement absorbed). Now unified in single closure event.
```

#### §9.2.2 — KERNEL_ARCHITECTURE.md v1.5 → v1.6 (Phase 6 commit 23)

Header update:
```markdown
# KERNEL_ARCHITECTURE.md

**Version**: 1.6  
**Status**: LOCKED  
**Last revised**: 2026-XX-XX (K8.3+K8.4 combined milestone closure)  
**Previous version**: 1.5 (2026-05-10, K-L3.1 bridge formalization)
```

**Part 0 K-L11 Implication paragraph** — rewrite:
```markdown
**K-L11 Implication post-combined milestone (2026-XX-XX)**:

NativeWorld is the **only** production storage path. Managed World class has 
been moved from `src/DualFrontier.Core/ECS/World.cs` to `tests/DualFrontier.Core.
Tests/Fixtures/ManagedTestWorld.cs` per K-L11's «retained as test fixture and 
research reference» provision. Production code constructs NativeWorld via 
`Bootstrap.Run(registry)`; grep verification of `src/` for `new World(` returns 
zero matches.

ManagedTestWorld preserves the managed ECS surface (CreateEntity, AddComponent, 
GetComponent, GetComponentUnsafe, Query, GetEntitiesWith, etc.) for legacy 
DualFrontier.Core.Tests reuse (~472 tests) without algorithmic rewrite. Test 
behavior is identical to pre-combined World behavior; only namespace + 
visibility + lifecycle home changed.

This fully realizes K-L11.
```

**Part 0 K-L3 Implication post-K-L3.1** — refinement:
```markdown
**K-L3 Implication post-K-L3.1 (refined 2026-XX-XX)**:

Path α (unmanaged struct components) is the default. Path β (managed class 
components annotated with `[ManagedStorage]`) is an opt-in bridge for cases 
where a class type cannot be expressed as unmanaged struct (Q1.β K-L3.1 lock).

Mod API v3 ships at K8.3+K8.4 combined closure (2026-XX-XX). Surface:
- `IModApi.RegisterComponent<T> where T : unmanaged, IComponent` — Path α
- `IModApi.RegisterManagedComponent<T> where T : class, IComponent` — Path β
- v2 IModApi deleted; no backward compatibility, no deprecation period
- v1/v2 manifests rejected by ManifestParser; only `manifestVersion: "3"` accepted

Path β storage:
- Per-mod ownership via RestrictedModApi.ManagedStore<T> (Dictionary<EntityId, T>)
- Lifetime scoped to mod ALC; cleared on UnloadMod step 5 (MOD_OS §9.5)
- Runtime-only per Q4.b K-L3.1 lock (no save persistence; reconstructed on load)

SystemBase.ManagedStore<T> accessor exposes Path β from system bodies via 
IManagedStorageResolver indirection.
```

**Part 2 master plan table** — rows K8.3 + K8.4 merged:
```markdown
| Milestone | Scope | Status | Estimated time |
|-----------|-------|--------|----------------|
| ... | ... | ... | ... |
| K8.2 | Class component redesign (Path α → unmanaged conversion) | CLOSED 2026-05-XX | ... |
| K8.3+K8.4 combined | Storage migration + system migration + Mod API v3 + World retirement | CLOSED 2026-XX-XX (this closure) | ~10-14 hours auto-mode |
| K8.5 | Mod ecosystem migration prep ([ModAccessible] audit, migration guide) | PLANNED | ~4-6 hours |
| K9 | Field storage | CLOSED 2026-05-11 | ... |
| ... | ... | ... | ... |
```

**Part 4 Decisions log** — new entry:
```markdown
### Decision: Combined K8.3+K8.4 (resolved 2026-XX-XX)

Context: K8.3 v2.0 halt resolution (2026-05-13) revealed that K8.3 (system 
migration) and K8.4 (storage migration) are architecturally coupled — systems 
can't migrate to `SpanLease`/`WriteBatch` without storage migration completing 
first, and storage migration leaves systems in broken state without immediate 
system-side migration. K8.3 v2.0 brief presupposed storage migration done; 
reality at HEAD `efd67df` was storage still on managed World. Patch brief 
resolved by swapping K8.3/K8.4 execution order with override patch layer.

Decision (Crystalka, 2026-05-13): combine K8.3+K8.4 into single atomic milestone. 
Claude Code's 1M token context permits this scope in single execution session. 
Atomic execution eliminates the facade pattern (dual-write factory + 
dual-storage-path systems) that splitting would require, and structurally 
prevents premise-miss errors of the K8.3 v2.0 halt class.

No backward compatibility for IModApi or manifests. v1/v2 deleted entirely. 
Clean break point.

Lessons:
- Architectural coupling forces milestone bundling (METHODOLOGY §K-Lessons 
  Lesson #6, encoded at A'.6 K8.5 closure batch revision)
- Brief authoring time vs execution time: brief assumptions can be invalidated 
  by HEAD state drift; Phase 0 deep-reads + storage premise verification 
  (Lessons #1 + #2 from K8.3 halt resolution) are operational, not exceptional
```

#### §9.2.3 — MOD_OS_ARCHITECTURE.md v1.7 → v1.8 (Phase 6 commit 23)

Header update:
```markdown
# MOD_OS_ARCHITECTURE.md

**Version**: 1.8  
**Status**: LOCKED  
**Last revised**: 2026-XX-XX (K8.3+K8.4 combined milestone closure)  
**Previous version**: 1.7 (2026-05-10, K9 field storage closure)
```

**Version history**:
```markdown
- v1.8 (2026-XX-XX, K8.3+K8.4 combined): IModApi v3 surface (Register Component
  constraint split + RegisterManagedComponent + ManagedStore<T>); ManifestVersion
  field strict v3-only acceptance; [ManagedStorage] attribute; 
  ValidationErrorKind.MissingManagedStorageAttribute enum addition
- v1.7 (2026-05-10, K9): Field storage surface (IModFieldApi, IModComputePipelineApi)
- v1.6 (2026-05-10, K9): Capability verbs field.* and pipeline.* added
- v1.5 (2026-05-XX): Earlier baseline
```

**§2.1 Manifest schema example** — augmented:
```json
{
  "manifestVersion": "3",
  "id": "com.example.mymod",
  "name": "My Mod",
  "version": "1.0.0",
  "requiresContractsVersion": "3",
  "capabilities": {
    "required": ["kernel.publish:PawnSpawnedEvent", "kernel.field.read:ManaField"],
    "provided": ["mod.example.publish:CustomEvent"]
  },
  ...
}
```

**§2.2 Field reference table** — row added:
```markdown
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `manifestVersion` | string | **YES** (v1.8+) | Schema version. Only `"3"` accepted post-K8.3+K8.4 closure. v1/v2 manifests rejected by ManifestParser with ValidationErrorKind.IncompatibleContractsVersion. |
| ... | ... | ... | ... |
```

**§2.3 Validation steps** — step added:
```markdown
7. **manifestVersion gate** (post-K8.3+K8.4): if `manifestVersion` is absent 
   or != `"3"`, reject with `ValidationErrorKind.IncompatibleContractsVersion`. 
   No grace period; no deprecation warnings.
```

**§4.6 IModApi v3 surface section** — wording tightened to align with shipped surface (verbatim review against `src/DualFrontier.Contracts/Modding/IModApi.cs` at closure HEAD).

**§9.5 UnloadMod chain** — step 5 extended:
```markdown
5. **Per-mod state reclamation** (extended at K8.3+K8.4):
   - RestrictedModApi.UnsubscribeAll (preserved from v1.7)
   - RestrictedModApi.ClearManagedStores (NEW at v1.8 — clears all
     ManagedStore<T> instances for the unloading mod's Path β components)
   - ModRegistry.UnregisterRestrictedModApi (NEW at v1.8 — removes mod's
     RestrictedModApi from resolver dispatch table)
```

**§11.2 ValidationErrorKind** — enum entries:
```markdown
| Kind | Description | Introduced |
|------|-------------|------------|
| ... | ... | ... |
| `IncompatibleContractsVersion` | Manifest's `requiresContractsVersion` or `manifestVersion` does not match supported version | v1.5 (extended semantic at v1.8 to cover manifestVersion gate) |
| `MissingManagedStorageAttribute` | Class type registered via `RegisterManagedComponent<T>` lacks `[ManagedStorage]` attribute | v1.8 (K8.3+K8.4 closure) |
```

#### §9.2.4 — PHASE_A_PRIME_SEQUENCING.md structural amendment (Phase 6 commit 23)

This is a Live document (no semantic version; tracked via `last_modified` + `last_modified_commit`).

**§2 Full Phase A' sequence** — ASCII diagram rewritten:
```
Phase A' sequence (post-K8.3+K8.4 combined closure 2026-XX-XX):

  A'.0   K-L3.1 bridge formalization                      COMPLETED 2026-05-10
  A'.0.5 Reorganization + refresh                         COMPLETED 2026-05-11
  A'.0.7 METHODOLOGY restructure                          COMPLETED 2026-05-11
  A'.1   Amendment execution                              COMPLETED 2026-05-11
  A'.2   K-L3.1 amendment plan                            COMPLETED 2026-05-11
  A'.3   K9 field storage                                 COMPLETED 2026-05-11
  A'.4   K-Lessons batch                                  COMPLETED 2026-05-12
  A'.4.5 Document Control Register                        COMPLETED 2026-05-12
  A'.5   K8.3+K8.4 combined kernel cutover                COMPLETED 2026-XX-XX  ← this milestone
  A'.6   K8.5 mod ecosystem migration prep                PLANNED
  A'.7   REMOVED (combined into A'.5)
  A'.8   K-closure report                                 PLANNED
  A'.9   Roslyn analyzer                                  PLANNED
```

**§3 Cumulative duration estimate** — table update:
```markdown
| Milestone | Estimated time | Actual time | Status |
|-----------|----------------|-------------|--------|
| ... | ... | ... | ... |
| A'.4.5    | ~6 hours auto-mode  | ~6 hours    | COMPLETED |
| A'.5      | ~10-14 hours auto-mode | TBD     | COMPLETED 2026-XX-XX (combined K8.3+K8.4) |
| A'.6      | ~4-6 hours auto-mode   | TBD     | PLANNED (K8.5) |
| A'.7      | (removed)              | n/a     | n/a |
| A'.8      | ~3-5 hours auto-mode   | TBD     | PLANNED |
| A'.9      | ~12-20 hours auto-mode | TBD     | PLANNED |
```

**§5 Document amendments triggered by Phase A'** — sub-sections updated:
```markdown
### §5.5 Amendments at A'.5 closure (K8.3+K8.4 combined, 2026-XX-XX)

Documents amended:
- MIGRATION_PLAN_KERNEL_TO_VANILLA.md v1.1 → v1.2 (combined section reformulation)
- KERNEL_ARCHITECTURE.md v1.5 → v1.6 (Part 2 row merge; K-L11 + K-L3 Implication updates)
- MOD_OS_ARCHITECTURE.md v1.7 → v1.8 (IModApi v3 surface; manifestVersion gate; unload chain extension)
- PHASE_A_PRIME_SEQUENCING.md structural amendment (this document)
- MIGRATION_PROGRESS.md closure entry (Tier 2 live tracker)

Document lifecycle transitions:
- DOC-D-K8_3 AUTHORED → SUPERSEDED
- DOC-D-K8_3_BRIEF_REFRESH_PATCH AUTHORED → SUPERSEDED
- DOC-D-K8_4 AUTHORED → SUPERSEDED
- DOC-D-K8_34_COMBINED AUTHORED → EXECUTED

REQ verification status promotions:
- REQ-K-L3 PENDING → VERIFIED
- REQ-K-L3.1 PENDING → VERIFIED
- REQ-K-L4 PENDING → VERIFIED
- REQ-K-L8 PENDING → VERIFIED
- REQ-K-L11 PENDING → VERIFIED
- REQ-Q-A45-X5 PENDING → VERIFIED

CAPA closures:
- CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT OPEN → CLOSED
- CAPA-2026-05-13-K8.3-PREMISE-MISS OPEN → CLOSED
```

#### §9.2.5 — MIGRATION_PROGRESS.md closure entry (Phase 6 commit 24)

Append new section at top of document:

```markdown
## A'.5 K8.3+K8.4 combined closure (2026-XX-XX)

**Brief**: `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md`  
**Commit range**: `<commit-1-sha>..<commit-24-sha>`  
**Atomic commits**: 24  
**Test count delta**: +<N> (post-closure baseline: <M>)  
**LOC delta**: ~−2000 / +1500 net (subject to actual measurement)  

### Architectural outcomes

- NativeWorld is the single production storage path (K-L11 fully realized)
- ComponentTypeRegistry is active on bootstrap path (K-L4 fully realized)
- All 12 coreSystems use SpanLease<T> reads + WriteBatch<T> writes (K-L7 fully realized)
- IModApi v3 is the sole modding surface (K-L3 + K-L3.1 fully realized)
- Managed World moved to tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs
- ManifestParser accepts only manifestVersion: "3" (no v1/v2 fallback)
- 3 orphan .uid files deleted (METHODOLOGY §7.1 11th application)

### Lessons

1. Production wiring file deep-read in Phase 0 (Lesson #1, K8.3 halt resolution origin, reapplied at this milestone Phase 0)
2. Explicit storage/lifecycle premise verification in Phase 0 (Lesson #2, K8.3 halt resolution origin, reapplied)
3. Cross-reference K-Lxx implication paragraphs (Lesson #3, K8.3 halt resolution origin, reapplied)
4. «Stop, escalate, lock» is operational, not exceptional (Lesson #4, K8.3 halt resolution origin)
5. Patch briefs are first-class artifacts (Lesson #5, K9 + K8.3 precedent)
6. **Architectural coupling forces milestone bundling** (Lesson #6, this milestone's primary precedent). When two scoped milestones share architectural dependency such that one's premise is satisfied only by the other's completion, bundle them. Splitting creates handoff cost (brief lockstep maintenance) without architectural benefit. Combined K8.3+K8.4 is the canonical example. Lesson #6 added to METHODOLOGY §K-Lessons at A'.6 K8.5 closure batch revision.

### Deviations from brief

[Recorded by execution agent at closure; expected to be near-empty if brief proves accurate]

### CAPA closures verified

- CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT (effectiveness verification: combined milestone closure exercised full §12.7 protocol; sync_register.ps1 --validate exit 0; ~23+ documents tracked; register doesn't degrade post-bootstrap)
- CAPA-2026-05-13-K8.3-PREMISE-MISS (effectiveness verification: combined milestone atomicity removes swap-introduced complexity; premise miss class structurally prevented by atomic execution)

### Related artifacts

- HALT_REPORT (K8.3 v2.0 halt origin): `docs/scratch/A_PRIME_5/HALT_REPORT.md`
- K8.3 v2.0 brief (SUPERSEDED): `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md`
- K8.3 patch brief (SUPERSEDED): `tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md`
- K8.4 skeleton brief (SUPERSEDED): `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md`
- This brief (EXECUTED): `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md`
```

### §9.3 — REQ verification status promotions (Phase 6 commit 24)

Update `docs/governance/REGISTER.yaml`:

```yaml
- id: REQ-K-L3
  verification_status: VERIFIED                  # ← was PENDING
  verified_at: "2026-XX-XX"                      # ← NEW field
  verified_commit: "<commit-24-sha>"             # ← NEW field
  verification_evidence: "K8.3+K8.4 combined milestone shipped IModApi v3 with RegisterComponent<T> where T : unmanaged constraint; grep src/ for non-unmanaged RegisterComponent usage returns zero"

- id: REQ-K-L3.1
  verification_status: VERIFIED
  verified_at: "2026-XX-XX"
  verified_commit: "<commit-24-sha>"
  verification_evidence: "K8.3+K8.4 combined milestone shipped IModApi.RegisterManagedComponent<T> + ManagedStore<T> per-mod storage + [ManagedStorage] attribute + IManagedStorageResolver dispatch; Path β bridge operational"

- id: REQ-K-L4
  verification_status: VERIFIED
  verified_at: "2026-XX-XX"
  verified_commit: "<commit-24-sha>"
  verification_evidence: "ComponentTypeRegistry active on bootstrap path via Bootstrap.Run(registry); all production component types explicitly registered via RegisterProductionComponentTypes; no FNV-1a fallback in production"

- id: REQ-K-L8
  verification_status: VERIFIED
  verified_at: "2026-XX-XX"
  verified_commit: "<commit-24-sha>"
  verification_evidence: "Factories use NativeWorld.AddComponents<T> bulk API (single P/Invoke per type, regardless of count); per-entity P/Invoke pattern eliminated from factory hot path"

- id: REQ-K-L11
  verification_status: VERIFIED
  verified_at: "2026-XX-XX"
  verified_commit: "<commit-24-sha>"
  verification_evidence: "Managed World moved to tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs; production assemblies don't reference World; grep src/ for `new World(` returns zero matches; K-L11 «retained as test fixture» provision applied verbatim"

- id: REQ-Q-A45-X5
  verification_status: VERIFIED
  verified_at: "2026-XX-XX"
  verified_commit: "<commit-24-sha>"
  verification_evidence: "First post-A'.4.5 milestone closure exercised full METHODOLOGY §12.7 closure protocol: 4 lifecycle transitions, 4 version bumps + 1 structural amendment, 6 REQ verifications, 2 CAPA closures, 1 audit trail entry, ~23 documents tracked. sync_register.ps1 --validate exit 0. Register governance operational under realistic load."
```

### §9.4 — CAPA closures (Phase 6 commit 24)

Update `docs/governance/REGISTER.yaml`:

```yaml
- id: CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT
  status: CLOSED                                 # ← was OPEN
  closed_at: "2026-XX-XX"                        # ← NEW field
  closed_commit: "<commit-24-sha>"               # ← NEW field
  effectiveness_verification:
    method: "First post-register milestone closure exercises full METHODOLOGY §12.7 protocol with sync_register.ps1 --validate clean"
    date_verified: "2026-XX-XX"
    verification_commit: "<commit-24-sha>"
    verification_outcome: "Combined K8.3+K8.4 closure exercised full §12.7 protocol; sync validate exit 0; 23+ documents tracked; register doesn't degrade post-bootstrap; document count drift observed during A'.4.5 enrollment (pass 5 underestimate of ~195 vs actual 229) did not recur. Future briefs incorporate Phase 0 grep-count step per A'.4.5 lesson."

- id: CAPA-2026-05-13-K8.3-PREMISE-MISS
  status: CLOSED                                 # ← was OPEN
  closed_at: "2026-XX-XX"
  closed_commit: "<commit-24-sha>"
  effectiveness_verification:
    method: "Combined milestone atomicity removes swap-introduced complexity; no further premise miss surfaced during execution"
    date_verified: "2026-XX-XX"
    verification_commit: "<commit-24-sha>"
    verification_outcome: "Crystalka decision to combine K8.3+K8.4 (2026-05-13) resolves premise miss structurally — atomic execution makes 'storage stays through K8.3' obsolete because K8.3 doesn't exist as separate milestone. Combined milestone executed without halt; Phase 0 deep-read protocol from K8.3 halt resolution applied throughout. Lesson #6 (architectural coupling forces milestone bundling) encoded for METHODOLOGY revision at A'.6 K8.5 closure."
```

### §9.5 — Audit trail entry (Phase 6 commit 24)

Append to `docs/governance/REGISTER.yaml` audit trail section:

```yaml
- id: EVT-2026-XX-XX-K8.3-K8.4-COMBINED-CLOSURE
  date: "2026-XX-XX"
  event: "K8.3+K8.4 combined kernel cutover milestone closure"
  event_type: milestone_closure
  documents_affected:
    - DOC-D-K8_34_COMBINED                       # AUTHORED → EXECUTED
    - DOC-D-K8_3                                 # AUTHORED → SUPERSEDED
    - DOC-D-K8_3_BRIEF_REFRESH_PATCH             # AUTHORED → SUPERSEDED
    - DOC-D-K8_4                                 # AUTHORED → SUPERSEDED
    - DOC-A-MIGRATION_PLAN                       # v1.1 → v1.2
    - DOC-A-KERNEL                               # v1.5 → v1.6
    - DOC-A-MOD_OS                               # v1.7 → v1.8
    - DOC-A-PHASE_A_PRIME_SEQUENCING             # structural amendment
    - DOC-C-MIGRATION_PROGRESS                   # closure entry appended
  commits:
    range: "<commit-1-sha>..<commit-24-sha>"
    key_commits:
      - hash: "<commit-21-sha>"
        summary: "World retirement load-bearing cutover commit (Phase 5 commit 21)"
      - hash: "<commit-22-sha>"
        summary: "World class moved to test project (Phase 5 commit 22)"
      - hash: "<commit-23-sha>"
        summary: "Documentation amendments (MIGRATION_PLAN v1.2 + KERNEL v1.6 + MOD_OS v1.8 + PHASE_A_PRIME_SEQUENCING)"
      - hash: "<commit-24-sha>"
        summary: "MIGRATION_PROGRESS closure + REGISTER.yaml updates (this commit)"
  governance_impact: |
    First post-A'.4.5 closure under register governance.
    
    CAPA closures:
    - CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT (verification gate exercised; passed)
    - CAPA-2026-05-13-K8.3-PREMISE-MISS (verification gate exercised; passed via atomicity)
    
    REQ verification promotions:
    - REQ-K-L3, REQ-K-L3.1, REQ-K-L4, REQ-K-L8, REQ-K-L11 — K-Lxx invariants empirically materialized by combined milestone closure
    - REQ-Q-A45-X5 — post-session protocol exercised realistically (4 lifecycle transitions, 4 version bumps + 1 structural amendment, 6 REQ verifications, 2 CAPA closures, 1 audit trail entry, ~23 documents tracked)
    
    RISK status:
    - RISK-008 (amendment plan completeness gap): mitigation status APPLIED confirmed by structural prevention of premise-miss class via combined milestone approach
    
    METHODOLOGY §K-Lessons updates:
    - Lesson #6 (architectural coupling forces milestone bundling) encoded for batch revision at A'.6 K8.5 closure
  cross_references:
    capa_entries:
      - CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT (CLOSED via this closure)
      - CAPA-2026-05-13-K8.3-PREMISE-MISS (CLOSED via this closure)
    req_entries:
      - REQ-K-L3 (PENDING → VERIFIED)
      - REQ-K-L3.1 (PENDING → VERIFIED)
      - REQ-K-L4 (PENDING → VERIFIED)
      - REQ-K-L8 (PENDING → VERIFIED)
      - REQ-K-L11 (PENDING → VERIFIED)
      - REQ-Q-A45-X5 (PENDING → VERIFIED)
    risk_entries:
      - RISK-008 (mitigation APPLIED confirmed)
```

### §9.6 — Final sync_register.ps1 --validate gate (Phase 6 commit 24 final check)

Last operation before commit 24 ships:

```powershell
./tools/governance/sync_register.ps1 --validate
# Expected exit 0
# Expected no errors
# Expected no warnings
#
# If any output:
#   - Errors: HALT, repair register, retry
#   - Warnings: assess; in most cases continue with warning recorded in commit msg
```

Optional bootstrap backfill (per FRAMEWORK §8.3, Option B preferred per A'.4.5 lesson):

If REGISTER.yaml entries reference their own commit hash (e.g., `superseded_commit: <commit-24-sha>` for an entry committed in commit 24), then commit 24's own hash is not yet known when commit 24's content is authored. Two approaches:

- **Option A (amend route)**: stage commit 24 with placeholder `<TBD>` for self-reference; run `git commit`; capture hash; amend the commit replacing `<TBD>` with actual hash. **Drawback**: changes hash, orphans the register's self-reference (A'.4.5 lesson).
- **Option B (separate backfill commit, preferred)**: stage commit 24 with placeholder; commit; immediately follow with commit 24b backfilling actual hashes into REGISTER.yaml. Both commits ship together to origin.

Decision per A'.4.5 lesson: **Option B**. Commit 24 ships with placeholders; commit 25 (informally numbered — not counted in the 24-commit total) backfills.

---

## §10 — Estimated atomic commit log

Consolidated reference (full details in §6 phase plan):

```
Phase 0 — brief authoring
  Commit  1: docs(briefs): K8.3+K8.4 combined kernel cutover brief authored

Phase 1 — orphan .uid cleanup
  Commit  2: chore(scratch): delete 3 orphan .uid files (METHODOLOGY §7.1)

Phase 2 — storage migration foundation
  Commit  3: feat(bootstrap): activate ComponentTypeRegistry + Bootstrap.Run wiring (K8.4 storage Phase 2/2)
  Commit  4: test(interop): production component registry verification tests

Phase 3 — IModApi v3 surface
  Commit  5: feat(modding): IModApi v3 surface — RegisterComponent constraint split
  Commit  6: feat(modding): [ManagedStorage] attribute + ManagedStore<T> + enum extension
  Commit  7: feat(modding): RestrictedModApi v3 implementation + IManagedStorageResolver
  Commit  8: feat(modding): ManifestVersion field + strict v3-only acceptance

Phase 4 — per-system migration (Tier 1 → Tier 5)
  Commit  9: feat(systems): migrate ConsumeSystem to SpanLease/WriteBatch (K8.3 1/12)
  Commit 10: feat(systems): migrate SleepSystem to SpanLease/WriteBatch (K8.3 2/12)
  Commit 11: feat(systems): migrate ComfortAuraSystem to SpanLease/WriteBatch (K8.3 3/12)
  Commit 12: feat(systems): migrate MoodSystem to SpanLease/WriteBatch (K8.3 4/12)
  Commit 13: feat(systems): migrate PawnStateReporterSystem to SpanLease/WriteBatch (K8.3 5/12)
  Commit 14: feat(systems): migrate JobSystem to SpanLease/WriteBatch (K8.3 6/12)
  Commit 15: feat(systems): migrate HaulSystem to SpanLease/WriteBatch (K8.3 7/12)
  Commit 16: feat(systems): migrate NeedsSystem to SpanLease/WriteBatch (K8.3 8/12)
  Commit 17: feat(systems): migrate ConverterSystem to SpanLease/WriteBatch (K8.3 9/12)
  Commit 18: feat(systems): migrate ElectricGridSystem to SpanLease/WriteBatch (K8.3 10/12)
  Commit 19: feat(systems): migrate InventorySystem to SpanLease/WriteBatch (K8.3 11/12)
  Commit 20: feat(systems): migrate MovementSystem to SpanLease/WriteBatch (K8.3 12/12)

Phase 5 — World retirement
  Commit 21: refactor(core): retire managed World as production storage path (K-L11 fully realized)
  Commit 22: refactor(core,tests): move World class to test project as ManagedTestWorld (K-L11)

Phase 6 — closure
  Commit 23: docs(architecture): K8.3+K8.4 amendments (MIGRATION_PLAN v1.2 + KERNEL v1.6 + MOD_OS v1.8 + PHASE_A_PRIME_SEQUENCING)
  Commit 24: docs(governance,closure): K8.3+K8.4 combined milestone closure (REGISTER updates + MIGRATION_PROGRESS entry)
  Commit 25 (optional backfill): docs(governance): backfill self-referential commit hashes per FRAMEWORK §8.3 Option B
```

**Total atomic commits**: 24 (+ optional commit 25 backfill).

**Estimated wall-clock execution time**: 10-14 hours auto-mode Claude Code session.

Pushed to `origin/main` after closure verification passes (manual gate; per Claude Code auto-mode classifier behavior re: push-to-main).

---

## §11 — Provenance + relationship to superseded artifacts

### §11.1 — Authorship lineage

**This brief**: authored 2026-05-13 by Claude (Opus 4.7) under Crystalka direction in deliberation mode session continuing the K8.3 v2.0 halt resolution session of the same day.

**Authoring session structure**:
- Initial context transfer from prior session via uploaded session log
- Phase 0 deep-reads of production wiring files (Tier 1 + Tier 2)
- 8-pass deliberation ratifying Q-COMBINED locks 1-8
- Crystalka direction at Pass 8 closure: «Так как окно контекста в сессии Claude Code 1 миллион токенов, то правильно будет совместить K8.3+K8.4 и написать подробный бриф чтобы не было конфликтов в дальнейшей работе»
- Crystalka direction post-Pass 8: «мы не будем поддерживать старый api так что удаление и новый» (no backward compatibility, clean break)
- Brief authoring (Pass 9 — this artifact)

### §11.2 — Superseded artifacts

Three briefs on disk preserved as historical artifacts:

#### §11.2.1 — `K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` (K8.3 v2.0)

**Status at this brief's authoring**: AUTHORED. Lifecycle transition AUTHORED → SUPERSEDED at this brief's execution closure (Phase 6 commit 24).

**Original purpose**: Migrate 12 production systems from `SystemBase.GetComponent`/`Query` legacy API to `NativeWorld.AcquireSpan`/`BeginBatch` SpanLease/WriteBatch pattern.

**Why superseded**: K8.3 v2.0 brief presupposed component storage already on NativeWorld (K8.4 done first). Reality at HEAD `efd67df`: storage still on managed World (K8.4 pending). K8.3 v2.0 brief authoring missed the embedded comment in GameBootstrap.cs lines 91-96 declaring transition state. Halt rationale documented in `docs/scratch/A_PRIME_5/HALT_REPORT.md`.

Resolution path (pre-combined decision):
- K8.3 patch brief authored (next entry below)
- A'.5 ↔ A'.6 swap proposed (K8.4 first, K8.3 second)

Combined milestone supersession path (decided 2026-05-13):
- K8.3 scope (system migration) absorbed into Track B of combined brief
- K8.3 v2.0 brief preserved on disk for historical traceability

**Content overlap with combined brief**: Track B (per-system migration patterns + ordering + commit shape templates) §3 + §6.5 of this brief absorb K8.3 v2.0 brief's principal content.

#### §11.2.2 — `K8_3_BRIEF_REFRESH_PATCH.md`

**Status at this brief's authoring**: AUTHORED. Lifecycle transition AUTHORED → SUPERSEDED at this brief's execution closure.

**Original purpose**: Override layer for K8.3 v2.0 brief swapping K8.3/K8.4 execution order. A'.5 becomes K8.4, A'.6 becomes K8.3, A'.7 becomes K8.5 (absorbing World retirement scope).

**Why superseded**: Combined milestone approach eliminates the swap need. K8.3+K8.4 are bundled into single atomic milestone; swap rationale becomes obsolete because K8.3 doesn't exist as separate milestone.

**Content overlap with combined brief**: Track A (storage migration mechanics + factory dual-write pattern) §2 + §6.3 of this brief absorb K8.3 patch's principal content.

#### §11.2.3 — `K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` (skeleton)

**Status at this brief's authoring**: AUTHORED (skeleton state only — ~30% content, awaiting detailed authoring). Lifecycle transition AUTHORED → SUPERSEDED at this brief's execution closure.

**Original purpose**: Migrate component storage from managed `World` to `NativeWorld` + activate `ComponentTypeRegistry` + retire managed `World` as production code path.

**Why superseded**: K8.4 scope absorbed into combined milestone Track A. K8.4 skeleton was never fully authored; combined brief authoring takes its place.

**Content overlap with combined brief**: §2.1-§2.4 (storage layer architectural concern) + §6.3 + §6.6 (Phase 2 + Phase 5 World retirement) of this brief absorb K8.4 skeleton's intended content.

### §11.3 — Relationship to other K-track briefs

**Downstream**: K8.5 (Mod ecosystem migration prep) at `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` — remains AUTHORED at this brief's closure. Original Migration Plan v1.0 scope restored (no longer absorbing World retirement since combined milestone handles it). K8.5 execution depends on this milestone's closure.

**Upstream**: K9 field storage (closed 2026-05-11 — commit XXX). K9 IModFieldApi + IModComputePipelineApi surface preserved through v3 IModApi surface (Track C in this brief). K9 brief refresh patch (`K9_BRIEF_REFRESH_PATCH.md`) was the precedent for first-class patch-brief artifact pattern that informed K8.3 patch authoring.

**Lateral**: K-Lessons batch (closed 2026-05-12 — commit XXX). Lessons #1-#5 originated from K8.3 halt resolution; Lesson #6 (architectural coupling forces milestone bundling) added by this milestone's closure deferred to A'.6 K8.5 closure batch revision per METHODOLOGY §K-Lessons amendment pattern.

### §11.4 — Brief authoring lineage

```
2026-05-08  Migration Plan v1.0 (LOCKED) authored — separate K8.3 + K8.4 + K8.5 milestones
2026-05-10  K-L3.1 bridge formalization (commit 45d831c) — dual-path Path α / Path β contract
2026-05-10  Migration Plan v1.1 (LOCKED) — K-L3.1 bridge absorbed; K8.3/K8.4 scope refined
2026-05-11  K9 field storage closure — IModFieldApi shipped
2026-05-12  A'.4.5 Document Control Register operational — governance baseline established
2026-05-13  K8.3 v2.0 brief execution halt — storage premise miss identified
2026-05-13  K8.3 brief refresh patch authored — A'.5/A'.6 swap rationale
2026-05-13  Crystalka direction: combine K8.3+K8.4 into single milestone
2026-05-13  This combined brief authored (this artifact)
2026-XX-XX  This combined brief execution closure (planned, ~10-14 hours auto-mode)
```

### §11.5 — Brief-on-disk reading order at HEAD post-closure

For future readers reconstructing the K8.3+K8.4 history:

1. **Authoritative**: this brief (`K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` — EXECUTED). Single source of execution truth.
2. **Historical** (in reverse chronological order, all SUPERSEDED):
   - `K8_3_BRIEF_REFRESH_PATCH.md` — the swap proposal that was ultimately superseded by combination
   - `K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` (v2.0) — the system migration brief whose halt triggered the patch
   - `K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` (skeleton) — the storage migration skeleton that was never independently executed
3. **Halt artifact**: `docs/scratch/A_PRIME_5/HALT_REPORT.md` — the halt rationale that drove the patch + combination decisions
4. **Closure ledger entry**: `docs/architecture/MIGRATION_PROGRESS.md` §«A'.5 K8.3+K8.4 combined closure» (Phase 6 commit 24 appended)
5. **Audit trail entry**: `docs/governance/REGISTER.yaml` §audit trail «EVT-2026-XX-XX-K8.3-K8.4-COMBINED-CLOSURE»

---

## §12 — Reading completion sign-off

This brief is self-contained for K8.3+K8.4 combined milestone execution. Execution agent (Claude Code in auto-mode) reads this brief in full, performs Phase 0 verification (§6.1), then executes Phase 1-6 sequentially.

If at any point during execution the brief proves insufficient, ambiguous, or contradicted by HEAD reality, **halt per METHODOLOGY §3** and author a HALT_REPORT in `docs/scratch/A_PRIME_5_CONTINUED/`. Do not improvise; do not patch in-flight; escalate to deliberation mode.

The brief expects ~0 halts during execution because the K8.3 v2.0 halt resolution lessons are encoded in Phase 0 protocol. If halt occurs, the halt itself is a high-value architectural signal warranting investigation, not an execution defect.

**End of brief.**
