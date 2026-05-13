---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ECS
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ECS
---
# Entity Component System

Dual Frontier uses the classical ECS approach: an entity is an identifier, components are pure data, systems are logic. The ECS core lives in the `DualFrontier.Core` assembly and is marked `internal` — only the `IComponent` and `EntityId` contracts plus the `SystemBase` base class are visible from outside.

## World, EntityId, Component

### World

`World` is the registry of all entities. A separate `ComponentStore<T>` is kept for each component type. There is one world per game; it is created in `GameLoop` and handed to `ParallelSystemScheduler`. Systems do not touch `World` directly — access goes through `SystemExecutionContext`.

```csharp
internal sealed class World
{
    private readonly ConcurrentDictionary<Type, IComponentStore> _stores = new();
    private int _nextEntityId;

    public EntityId CreateEntity() { /* ... */ }
    public void DestroyEntity(EntityId id) { /* ... */ }

    // Unsafe access — only for SystemExecutionContext.
    internal T GetComponentUnsafe<T>(EntityId id) where T : IComponent { /* ... */ }
}
```

### EntityId

`EntityId` is a `readonly struct` with two fields: `int Index` and `int Version`. The version is incremented when an entity is destroyed, which makes old references safely invalid. The comparison `id.Version == world.GetVersion(id.Index)` returns `false` for a dead reference — the system simply skips it rather than crashing.

### Component

A component is a class (or `struct`) that implements `IComponent`. No logic, only data. Validation, arithmetic, and checks all live in systems. This yields two wins: components serialize freely as POCOs, and they can be safely read by several systems simultaneously as long as no one writes.

## SparseSet — why not an array

A naive `T[]` array for components wastes memory: most entities do not have a given component. A dense `List<T>` requires binary search by `EntityId` — O(log n). SparseSet solves both problems.

SparseSet keeps two structures:

- `sparse[index]` — an array of length `maxEntityCount` that stores the component's position in the dense array, or `-1`.
- `dense[]` — the dense array of components and a parallel array of `EntityId`s.

Insertion is O(1), removal is O(1) via swap-with-last, iteration is O(N) over the dense array with no gaps. This pattern is adopted by every modern ECS engine (EnTT, bevy_ecs, flecs).

Iteration is what matters for Dual Frontier: systems walk every entity with the required component set inside `Update`. The dense array gives good cache locality — a substantial win at 10–20 thousand entities.

## Query<T1, T2, ...>

The `Query<HealthComponent, PositionComponent>()` query returns an iterator over entities that have both components. The algorithm is a sparse-set intersection: take the smallest `ComponentStore` and check each of its entities for the remaining components.

```csharp
[SystemAccess(reads: new[] { typeof(HealthComponent), typeof(PositionComponent) })]
public class HealthReporterSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entity in Query<HealthComponent, PositionComponent>())
        {
            var health = GetComponent<HealthComponent>(entity);
            var pos = GetComponent<PositionComponent>(entity);
            // Access is permitted — both components are declared in reads.
        }
    }
}
```

`Query` does not materialize a list — it yields entities lazily. This matters because the list can change during `Update` (another system in parallel may destroy an entity), and the generator simply skips a stale version.

## Entity lifecycle

### Creation

`world.CreateEntity()` returns a new `EntityId` with an index and a version. Components are attached separately: `world.AddComponent(id, new HealthComponent { Maximum = 100 })`. Only `Application` or a system can create entities; a system does so through the dedicated `SystemBase.CreateEntity` method (registered in `SystemExecutionContext` as write-all).

### Destruction

`world.DestroyEntity(id)` does not free the index immediately — component removal happens at the end of the current scheduler phase so that parallel systems finish their walk without a `NullReferenceException`. The entity version is incremented. All subsequent `GetComponent<T>(oldId)` calls return `null`, or throw `IsolationViolationException` on an attempted write.

### Versioning

An entity with an old version is the indicator of a dead reference. Systems check validity through `world.IsAlive(id)`. No `null` panics: immutable event records carry an `EntityId`, and if the entity has already been destroyed by the time the handler runs, the handler simply returns. This makes the event bus resilient to races.

## SystemBase

`SystemBase` is the base class for every game system. It defines three extension points:

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

    // Safe read/write via the isolation guard.
    protected T GetComponent<T>(EntityId id) where T : IComponent;
    protected void SetComponent<T>(EntityId id, T value) where T : IComponent;
    protected IEnumerable<EntityId> Query<T>() where T : IComponent;
    protected IEnumerable<EntityId> Query<T1, T2>()
        where T1 : IComponent where T2 : IComponent;
    protected TSystem GetSystem<TSystem>() where TSystem : SystemBase; // always crashes
}
```

An access declaration is mandatory: `[SystemAccess(reads: [...], writes: [...], bus: nameof(IGameServices.Combat))]`. The scheduler reads the attribute once at startup and builds the dependency graph.

## Anti-patterns

### Keeping a `World` reference in a system

```csharp
// BAD — bypasses the isolation guard, breaks parallelism.
public class BadSystem : SystemBase
{
    private World _world;
    public BadSystem(World w) { _world = w; }
}
```

Systems receive `World` only through `SystemExecutionContext` — and do not store it. A graph rebuild or a mod hot-reload breaks any cached reference.

### Direct call to another system

```csharp
// BAD — bypasses the bus, creates an implicit dependency.
var inventory = GetSystem<InventorySystem>();
inventory.ReserveAmmo(...);
```

The isolation guard always (even in Release) throws `IsolationViolationException` on `GetSystem`. Use `bus.Inventory.Publish(new AmmoIntent { ... })` instead.

### Logic in a component

```csharp
// BAD — the component is not pure data.
public sealed class HealthComponent : IComponent
{
    public float Current;
    public void TakeDamage(float amount) { Current -= amount; }  // no.
}
```

Damage logic lives in `DamageSystem`. The component stays a POCO.

### Creating an entity in `Update` with an immediate `GetComponent`

```csharp
// BAD — another system in the same phase will not see the new entity.
var id = CreateEntity();
var health = GetComponent<HealthComponent>(id);  // race condition.
```

New entities become visible in the next phase. Split the logic: create in this phase, work with components in the next.

## See also

- [ARCHITECTURE](./ARCHITECTURE.md)
- [FIELDS](./FIELDS.md) — the orthogonal storage system; spatial scalar/vector grids alongside per-entity components. Same kernel, separate access model.
- [ISOLATION](./ISOLATION.md)
- [THREADING](./THREADING.md)
