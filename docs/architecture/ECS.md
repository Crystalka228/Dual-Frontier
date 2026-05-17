---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ECS
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.1"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ECS
---
# Entity Component System

Dual Frontier uses the classical ECS approach: an entity is an identifier, components are pure data, systems are logic. The ECS core lives in the `DualFrontier.Core` assembly. `IComponent` and `EntityId` are the public contracts; `NativeWorld` is the production component-storage backend exposed to systems via `SystemBase.NativeWorld`.

## NativeWorld, EntityId, Component

### NativeWorld

`NativeWorld` is the sole production component-storage backend after A'.5 K8.3+K8.4 (2026-05-14). Storage layout is span/batch-oriented for cache locality and parallelism. The prior managed `World` registry is retired from production and survives only as `ManagedTestWorld` — a test fixture for scenarios that don't yet need the native path. `NativeWorld` is constructed in `GameBootstrap.CreateLoop` (see [src/DualFrontier.Application/Loop/GameBootstrap.cs](../../src/DualFrontier.Application/Loop/GameBootstrap.cs)) and handed to `ParallelSystemScheduler`. Systems access it through `SystemBase.NativeWorld`, which routes through the active `SystemExecutionContext`.

```csharp
// Public surface visible to systems via SystemBase.NativeWorld.
public sealed class NativeWorld
{
    public SpanLease<T> AcquireSpan<T>() where T : unmanaged, IComponent;
    public WriteBatch<T> BeginBatch<T>() where T : unmanaged, IComponent;
    public bool HasComponent<T>(EntityId id) where T : unmanaged, IComponent;
    public bool TryGetComponent<T>(EntityId id, out T value)
        where T : unmanaged, IComponent;

    // K8.1 primitives for component-owned reference fields
    public InternedString InternString(string value);
    public NativeMap<TK, TV> CreateMap<TK, TV>()
        where TK : unmanaged where TV : unmanaged;
    public NativeSet<T> CreateSet<T>() where T : unmanaged;
    public NativeComposite<T> CreateComposite<T>() where T : unmanaged;
}
```

Path β managed-class storage (K-L3.1 bridge) lives in per-mod `ManagedStore<T>` reached via `SystemBase.ManagedStore<T>()` — see [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §9.5.

### EntityId

`EntityId` is a `readonly struct` with two fields: `int Index` and `int Version`. The version is incremented when an entity is destroyed, which makes old references safely invalid. A stale reference simply fails the `TryGetComponent` check and the system skips it rather than crashing.

### Component

A component is a `struct` (Path α — preferred, `where T : unmanaged, IComponent`) or a class (Path β — for components with reference-typed fields living in per-mod managed storage, `where T : class, IComponent` with `[ManagedStorage]` attribute). No logic, only data. Validation, arithmetic, and checks all live in systems. Path α components serialize freely as POCOs and can be safely batch-read by several systems simultaneously as long as no one writes; Path β components live in per-mod managed storage and are runtime-only (not persisted by the save system — see K-L3.1 lock).

## SparseSet — why not an array

A naive `T[]` array for components wastes memory: most entities do not have a given component. A dense `List<T>` requires binary search by `EntityId` — O(log n). SparseSet solves both problems.

SparseSet keeps two structures:

- `sparse[index]` — an array of length `maxEntityCount` that stores the component's position in the dense array, or `-1`.
- `dense[]` — the dense array of components and a parallel array of `EntityId`s.

Insertion is O(1), removal is O(1) via swap-with-last, iteration is O(N) over the dense array with no gaps. This pattern is adopted by every modern ECS engine (EnTT, bevy_ecs, flecs).

Iteration is what matters for Dual Frontier: systems walk every entity with the required component set inside `Update`. The dense array gives good cache locality — a substantial win at 10–20 thousand entities. `NativeWorld.AcquireSpan<T>()` returns a `SpanLease<T>` that exposes the dense storage directly (read-only span + parallel `EntityId` index array).

## Span/Batch access pattern

`NativeWorld.AcquireSpan<T>()` returns a `SpanLease<T>` — a read-only span into the dense storage. `BeginBatch<T>()` returns a `WriteBatch<T>` that buffers mutations for application at phase boundaries.

```csharp
[SystemAccess(reads: new[] { typeof(HealthComponent), typeof(PositionComponent) })]
public class HealthReporterSystem : SystemBase
{
    public override void Update(float delta)
    {
        using SpanLease<HealthComponent> healthLease = NativeWorld.AcquireSpan<HealthComponent>();
        ReadOnlySpan<HealthComponent> health = healthLease.Components;
        ReadOnlySpan<int> indices = healthLease.Indices;

        for (int i = 0; i < healthLease.Count; i++)
        {
            var id = new EntityId(indices[i], 0);
            if (!NativeWorld.TryGetComponent<PositionComponent>(id, out PositionComponent pos))
                continue;
            // Access is permitted — both components are declared in reads.
        }
    }
}
```

For writes, `BeginBatch<T>` returns a `WriteBatch<T>` that stages writes; the scheduler flushes the batch at the next phase boundary so parallel systems observe a consistent snapshot during their span walks.

## Entity lifecycle

### Creation

Entities are created through `NativeWorld.CreateEntity()` and equivalent factory paths used by `RandomPawnFactory`, `ItemFactory`, etc. in production. Components are attached separately via `WriteBatch<T>` or at bootstrap through the registry handoff in `Bootstrap.Run`.

### Destruction

`NativeWorld.DestroyEntity(id)` does not free the index immediately — component removal happens at the next scheduler phase boundary so that parallel systems finish their span walk without observing partial state. The entity version is incremented. Subsequent `TryGetComponent<T>(oldId)` calls return `false`; writes against a dead `EntityId` are silently dropped at flush time.

### Versioning

An entity with an old version is the indicator of a dead reference. Systems check validity through `NativeWorld.IsAlive(id)` (or implicitly via `TryGetComponent`). No `null` panics: immutable event records carry an `EntityId`, and if the entity has already been destroyed by the time the handler runs, the handler simply returns. This makes the event bus resilient to races.

## SystemBase

`SystemBase` is the base class for every game system. It defines three lifecycle hooks plus accessors for the production storage path.

```csharp
public abstract class SystemBase
{
    // Called once when the system is registered.
    // Use for bus subscriptions or other one-time setup.
    protected virtual void OnInitialize() { }

    // Called by the scheduler at the rate set by [TickRate].
    public abstract void Update(float delta);

    // Called when the system is unloaded. Unsubscribe from buses, release resources.
    protected virtual void OnDispose() { }

    // Production storage access — sole path post-K8.3+K8.4.
    protected NativeWorld NativeWorld { get; }

    // Domain-bus aggregator for cross-system communication.
    protected IGameServices Services { get; }

    // Path β bridge accessor — returns null when no managed storage registered
    // for this mod (Core origin systems always get null; mods without
    // RegisterManagedComponent<T> get null).
    protected ManagedStore<T>? ManagedStore<T>() where T : class, IComponent;
}
```

`SystemBase.NativeWorld` and `SystemBase.Services` throw `InvalidOperationException` when accessed outside an active scheduler context (for example from the Godot main thread, or after an illegal `async`/`await` resumed on a different thread). The K8.3+K8.4 cutover removed the managed-World access surface (`GetComponent` / `SetComponent` / `Query` / `GetSystem`) — see [src/DualFrontier.Core/ECS/SystemBase.cs](../../src/DualFrontier.Core/ECS/SystemBase.cs) lines 9-17 for the canonical statement.

An access declaration is mandatory: `[SystemAccess(reads: [...], writes: [...], bus: nameof(IGameServices.Combat))]`. The scheduler reads the attribute once at startup and builds the dependency graph. Compile-time isolation enforcement is described in [ISOLATION](./ISOLATION.md); the future A'.9 Roslyn analyzer extends enforcement to call sites.

## Anti-patterns

### Caching a `NativeWorld` reference in system state

```csharp
// BAD — bypasses the per-tick context, breaks parallelism guarantees.
public class BadSystem : SystemBase
{
    private NativeWorld _world;
    public BadSystem(NativeWorld w) { _world = w; }
}
```

Systems receive `NativeWorld` only through `SystemExecutionContext` (via `SystemBase.NativeWorld`) and must not cache the reference. A graph rebuild or a mod hot-reload may invalidate cached state.

### Direct call to another system

```csharp
// BAD — bypasses the bus, creates an implicit dependency.
var inventory = GetSystem<InventorySystem>();  // method does not exist after K8.3+K8.4
```

There is no `GetSystem` accessor on `SystemBase` post-K8.3+K8.4 — the managed-World access surface was deleted entirely. Cross-system communication routes through `Services.Combat.Publish(...)` / `Services.Inventory.Publish(...)` / etc. — see [CONTRACTS](./CONTRACTS.md).

### Logic in a component

```csharp
// BAD — the component is not pure data.
public sealed class HealthComponent : IComponent
{
    public float Current;
    public void TakeDamage(float amount) { Current -= amount; }  // no.
}
```

Damage logic lives in `DamageSystem`. The component stays a POCO. Additional motivation post-K8.3+K8.4: Path α components must be `unmanaged` structs to cross the native storage boundary; methods on the struct are fine as long as they don't capture references.

### Creating an entity in `Update` with an immediate read

```csharp
// BAD — parallel readers will not see the new entity until the WriteBatch is
// flushed at the phase boundary.
using WriteBatch<HealthComponent> batch = NativeWorld.BeginBatch<HealthComponent>();
var id = NativeWorld.CreateEntity();
batch.Set(id, new HealthComponent { Maximum = 100 });
if (NativeWorld.TryGetComponent<HealthComponent>(id, out _)) { /* race */ }
```

New entities become visible to parallel readers only at the next phase boundary (after `WriteBatch` flush). Split the logic: create + write in this phase, read in the next.

## See also

- [ARCHITECTURE](./ARCHITECTURE.md) — the four layers and dependency rules.
- [FIELDS](./FIELDS.md) — the orthogonal storage system; spatial scalar/vector grids alongside per-entity components. Same kernel, separate access model.
- [ISOLATION](./ISOLATION.md) — compile-time enforcement model + ModFaultHandler lifecycle.
- [THREADING](./THREADING.md) — scheduler phases, dependency graph, `async` ban.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — Mod API v3, `RegisterManagedComponent`, Path α/β semantics.
