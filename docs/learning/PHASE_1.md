---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-LEARNING_PHASE_1
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-LEARNING_PHASE_1
---
# Learning artifact: C# and multithreading for Dual Frontier (Phase 1)

> **Note:** This document was originally written in Russian as a self-teaching
> artifact after Phase 1. Translated to English on 2026-04-27 as part of the
> i18n campaign. The original Russian version is preserved in git history at
> commit `cf8ef86`.

*Self-teaching ritual artifact produced after Phase 1 closure (Core ECS).
Full course tied to real project files. Version 1.0, 2026-04-25.*

*Each topic is a real `src/` file, not an abstract example.
Format: 20–30 min of theory + 30–60 min of work with the code + a practical check.*

*This document is referenced from [METHODOLOGY.md §4.5](/docs/methodology/METHODOLOGY.md) as
empirical evidence for the between-phase self-teaching ritual.*

---

# 1. Project architectural foundation

Before learning the language — you need to understand how the project is laid out. Dual Frontier consists of strictly separated layers. Breaking a dependency between them is an architectural error.

## 1.1 Four layers (docs/architecture/ARCHITECTURE.md)

| Layer | Assembly | What it does | Rule |
|---|---|---|---|
| Contracts | DualFrontier.Contracts | Interfaces, attributes, EntityId, GridVector. No implementation. | BCL dependencies only |
| Core (Infrastructure) | DualFrontier.Core | World, ComponentStore, DependencyGraph, ParallelSystemScheduler, DomainEventBus. Everything internal. | internal-first; opened to Systems via InternalsVisibleTo |
| Domain | DualFrontier.Systems, .Components, .Events, .AI | All game logic. Systems, components, events. | Knows nothing about Godot; multithreaded |
| Presentation / Application | DualFrontier.Presentation, .Application | Godot SceneTree, UI, GameLoop, SaveSystem, PresentationBridge. | Presentation runs only on the main thread |

> **📌 Rule** — Each layer knows only the layers below it. AI → Contracts + Components (not Core). Presentation → Application → Core. A violation is an arch-review error.

## 1.2 Dependency direction

```csharp
Contracts ← Components ← Systems ← Application ← Presentation
Contracts ← Events     ↗
Contracts ← Core       ↗  (Core is opened to Systems via InternalsVisibleTo)
Contracts ← AI (depends only on Contracts + Components)
```

> **⚠ Forbidden** — A mod sees ONLY DualFrontier.Contracts — through AssemblyLoadContext. A direct mod reference to Core is physically blocked. This is protection, not a convention.

# 2. C# as a language of contracts

## 2.1 class vs struct — where each is used

In Dual Frontier this is not an academic question — it is an architectural decision.

| Type | Where in the project | Why | Key rule |
|---|---|---|---|
| struct | EntityId, LeaseId, TransactionId, GridVector | Small, immutable identifier. Copied by value — no risk of accidental sharing. | readonly record struct — at most two int fields |
| class | World, SystemBase, DomainEventBus, ComponentStore<T> | Stateful object or service. Passed by reference — one instance for everyone. | sealed wherever there is no inheritance |
| interface | IComponent, IEvent, IModApi, IGameServices, ICombatBus | Contract for an interaction's shape, no implementation. The whole modding API consists of interfaces. | No methods on marker interfaces |

### EntityId — the canonical struct example

```csharp
public readonly record struct EntityId(int Index, int Version)
{
    public static readonly EntityId Invalid = default;
    public bool IsValid => Index > 0;
}
```

> **💡 Understand** — EntityId.Version is incremented on entity destruction. An old reference with version=1 to an entity with version=2 is a dead reference. The system simply skips it; it does not crash.

### Reference vs copy — the struct trap

This is the most common error when working with structs. Remember two cases:

```csharp
// ✅ CORRECT — work with the original through GetComponent/SetComponent
var hp = GetComponent<HealthComponent>(id);
hp.Current -= damage;
SetComponent(id, hp);   // wrote the change back
```

```csharp
// ❌ TRAP — if HealthComponent is a struct
ref var hp2 = ref store.GetRef(id);  // need ref semantics
hp2.Current -= damage;  // otherwise we modify a COPY; the original is unchanged
```

> **⚠ Rule** — If the type is a class, mutating a field through a reference changes the original. If it is a struct, GetComponent returns a COPY. Changes must be written back via SetComponent.

## 2.2 Generics — how to read signatures

Generics are everywhere in the project. You need to read them without fear.

| Signature | What it means | Where in the project |
|---|---|---|
| ComponentStore<T> where T : IComponent | T — any type implementing IComponent. One Store implementation for every component. | DualFrontier.Core/ECS/ComponentStore.cs |
| void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent | Subscribe to a specific event type. The compiler guarantees type safety. | DomainEventBus, IEventBus |
| IEnumerable<EntityId> Query<T1, T2>() where T1, T2 : IComponent | Lazy iterator over entities that have both components. | SystemExecutionContext, SystemBase |
| GetComponent<T>(EntityId id) where T : IComponent | Get a component in a strictly typed way. Type errors caught at compile time. | SystemBase (through the context) |

### How ComponentStore<T> works inside

```csharp
internal sealed class ComponentStore<T> : IComponentStore where T : IComponent
{
    private int[]  _sparse;       // indexed by EntityId.Index → position in dense
    private T[]    _dense;         // dense array of components
    private int[]  _denseToIndex;  // dense[i] belongs to the entity with index denseToIndex[i]
}
```

IComponentStore is a marker interface with no methods. It exists only to store different ComponentStore<T> instances in a single Dictionary<Type, IComponentStore>.

## 2.3 Interfaces — project contracts

An interface in Dual Frontier is not just a C# concept. It is an architectural promise. A mod knows only the interfaces, not the implementation.

| Interface | Meaning | Implemented by |
|---|---|---|
| IComponent | Marker: this is pure ECS data. No logic. | HealthComponent, PositionComponent, WeaponComponent... |
| IEvent | Marker: this is a domain-bus event. Immutable record. | DamageEvent, AmmoIntent, ManaGranted... |
| IModApi | Mod-API contract: RegisterComponent, RegisterSystem, Subscribe, Publish. | RestrictedModApi in Application |
| IGameServices | Aggregator of 5 buses: Combat, Inventory, Magic, Pawns, World. | GameServices in Core |
| ICombatBus, IMagicBus... | Contract of a specific domain bus. | CombatBus, MagicBus inside Core |

> **💡 Principle** — A mod registers a SystemBase descendant via IModApi.RegisterSystem<T>(). It does not know about World, does not know about Scheduler. It only knows that its system will be called with delta.

## 2.4 Attributes — the system's declarative language

Attributes in Dual Frontier are not metadata for code readers. They are declarations that DependencyGraph reads via reflection at startup.

```csharp
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)   // not a string — nameof
)]
[TickRate(TickRates.FAST)]
public sealed class CombatSystem : SystemBase { ... }
```

| Attribute | Read by | Why |
|---|---|---|
| [SystemAccess] | DependencyGraph.AddSystem() | Builds the READ/WRITE conflict graph. Determines phase order. |
| [TickRate] | TickScheduler.ShouldRun() | REALTIME=60Hz, FAST=30Hz, NORMAL=10Hz, SLOW=2Hz, RARE=0.2Hz |
| [Deferred] | DomainEventBus.Publish() | The event is delivered in the next phase, not instantly. |
| [Immediate] | DomainEventBus.Publish() | A critical preempt — delivery ahead of others. |

### How to read attributes through reflection

```csharp
SystemAccessAttribute? access =
    systemType.GetCustomAttribute<SystemAccessAttribute>(inherit: false);
if (access is null)
    throw new InvalidOperationException($"System '{systemType.Name}' has no [SystemAccess]");
```

inherit: false is an important parameter. The attribute is searched only on the concrete type, not on parents. This guarantees that every system explicitly declares its access.

## 2.5 Nullable and exceptions

The nullable context MUST be enabled. It is an early contract guard — null on input = a violation.

```csharp
// SystemExecutionContext — nullable used correctly throughout
public static SystemExecutionContext? Current => _current.Value;
// null = the thread does not belong to the scheduler (Godot main thread, test without context)
```

```csharp
// SystemBase uses a null-check as a guard
var ctx = SystemExecutionContext.Current
    ?? throw new InvalidOperationException(
        "GetComponent called outside an active scheduler context.");
```

> **📌 The finally rule** — PopContext MUST be in finally. If Update() throws, the thread still releases the context. Otherwise the next system on the same thread will get a nested push and fail with a diagnostic about a scheduler bug.

```csharp
SystemExecutionContext.PushContext(ctx);
try {
    system.Update(delta);
}
finally {
    SystemExecutionContext.PopContext();  // always, even on exception
}
```

# 3. Collections — part of the performance model

In the project, a collection is not just a container. The collection-type choice affects simulator performance with 10–20 thousand entities.

| Collection | Complexity | Where in the project | When to use |
|---|---|---|---|
| List<T> | O(1) append, O(n) search | SystemPhase.Systems, system sets, temporary batches | Need order, fast traversal, no lookup needed |
| Dictionary<TKey,TValue> | O(1) lookup | _stores in World (Type→IComponentStore), _contextCache in Scheduler | Fast lookup by key. Order does not matter. |
| HashSet<T> | O(1) contains | _allowedReads, _allowedWrites in SystemExecutionContext | Membership check, uniqueness, intersections |
| ConcurrentDictionary<K,V> | O(1) thread-safe | _stores in World, _handlers in DomainEventBus | Access from multiple threads. Does not replace architecture. |
| int[] (SparseSet) | O(1) all operations | _sparse, _dense in ComponentStore<T> | Maximum ECS performance, cache-friendly |

### SparseSet — implementation details

ComponentStore<T> uses a SparseSet. Understanding its structure matters for debugging.

```csharp
private int[] _sparse;       // length = maxEntityCount. sparse[EntityId.Index] → position in dense or -1
private T[]   _dense;         // dense array: dense[0], dense[1]... no holes
private int[] _denseToIndex;  // parallel to dense: which EntityId.Index lives in this slot
```

| Operation | How it works | Complexity |
|---|---|---|
| Add(id, component) | sparse[id.Index] = Count; dense[Count] = component; Count++ | O(1) |
| Remove(id) | Swap last element into removed slot. sparse[last.Index] = denseIdx. Count--. | O(1) via swap-with-last |
| Get(id) | return dense[sparse[id.Index]] | O(1) |
| Iterate all | Walk dense[0..Count-1] — dense, cache-friendly | O(Count), no gaps |
| Query<T1,T2> | Take the smaller store, check T2 presence for each entity | O(min(N1,N2)) |

### Collection-choice rule (three questions)

- 1. Is order needed? → If yes: List or a sorted structure
- 2. Is fast key lookup needed? → Dictionary or HashSet
- 3. Will access come from multiple threads? → ConcurrentDictionary or separate synchronization

> **⚡ Important** — ConcurrentDictionary makes individual operations atomic, but does NOT make a sequence of operations (read-modify-write) atomic. It is not a replacement for the proper phase model.

# 4. The ECS core: World, Entity, Component, System

## 4.1 Entity lifecycle

```csharp
// 1. Creation
EntityId id = world.CreateEntity();
// id.Index = unique index, id.Version = 1
```

```csharp
// 2. Adding a component
world.AddComponent(id, new HealthComponent { Current = 100, Maximum = 100 });
```

```csharp
// 3. Access through a system (not directly to World!)
// Inside the system's Update():
var hp = GetComponent<HealthComponent>(id);   // through SystemExecutionContext
```

```csharp
// 4. Destruction — increments Version
world.DestroyEntity(id);
// id.Version = 1, world.GetVersion(id.Index) = 2
// The old id is now a dead reference
```

> **✅ Dead references** — After DestroyEntity the old EntityId is safely invalid — its Version no longer matches the current. Systems check IsValid and skip dead entities instead of crashing.

## 4.2 System isolation — the core invariant

A system in Dual Frontier does NOT touch World directly. Only through SystemExecutionContext, which checks the [SystemAccess] declaration.

| Method | What the context checks (DEBUG) | Error on violation |
|---|---|---|
| GetComponent<T>(id) | T MUST be in the attribute's reads OR writes | IsolationViolationException: UndeclaredRead |
| SetComponent<T>(id, v) | T MUST be in the attribute's writes | IsolationViolationException: UndeclaredWrite |
| Query<T>() | T MUST be in reads or writes | IsolationViolationException: UndeclaredRead |
| GetSystem<TSystem>() | Always forbidden. Use the bus. | IsolationViolationException: DirectSystemAccess |
| Publish<TEvent>(evt) | The bus name MUST be in allowedBuses | IsolationViolationException: UnauthorizedBus |

```csharp
// ✅ Correct system
[SystemAccess(reads: new[]{ typeof(HealthComponent) }, writes: new Type[0], bus: nameof(IGameServices.Combat))]
[TickRate(TickRates.FAST)]
public sealed class DamageReporterSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entity in Query<HealthComponent>())
        {
            var hp = GetComponent<HealthComponent>(entity);
            if (hp.Current <= 0) Services.Combat.Publish(new DeathEvent(entity));
        }
    }
}
```

> **⚠ Core-system isolation violation** — Crashes immediately — IsolationViolationException. The violation is a developer bug. In a RELEASE build the checks are disabled (#if DEBUG); there is no overhead.

> **📌 Mod-system isolation violation** — Not a crash, but a route through IModFaultSink → ModFaultHandler in Application. The mod is unloaded; the game continues.

# 5. Simulator multithreading

## 5.1 DependencyGraph — how phases are built

DependencyGraph reads every system's [SystemAccess] and builds a conflict graph. A W/W conflict on one component is an error during Build(). A W/R conflict is a dependency edge.

```csharp
// Conflict examples:
// WriterASystem    writes: CompA
// ReadAWriteB      reads: CompA, writes: CompB   → depends on WriterA
// ReaderBSystem    reads: CompB                  → depends on ReadAWriteB
```

```csharp
// Result of topological sort (Kahn's algorithm):
// Phase 0: [WriterASystem]          — no dependencies
// Phase 1: [ReadAWriteB]             — waits on Phase 0
// Phase 2: [ReaderBSystem]           — waits on Phase 1
```

| Conflict type | What happens | Result |
|---|---|---|
| W/W (both write CompA) | Two threads write into one ComponentStore → race → corrupted world | Build() throws InvalidOperationException: Write conflict |
| W/R (one writes, the other reads) | The reader MUST start AFTER the writer | Edge in the graph → different phases |
| R/R (both read) | Safe — they may share a phase | In one phase, in parallel |
| Cycle in the graph | A depends on B, B depends on A — impossible to order | Build() throws: Cyclic dependency |

## 5.2 Parallel.ForEach — how phases execute

```csharp
// ParallelSystemScheduler.ExecutePhase()
Parallel.ForEach(phase.Systems, _parallelOptions, system =>
{
    if (!_ticks.ShouldRun(system)) return;
```

```csharp
    SystemExecutionContext ctx = _contextCache[system];
    SystemExecutionContext.PushContext(ctx);
    try { system.Update(delta); }
    finally { SystemExecutionContext.PopContext(); }
});
// Parallel.ForEach blocks until every system in the phase finishes
// That IS the BARRIER between phases
```

| Detail | Value |
|---|---|
| MaxDegreeOfParallelism | Environment.ProcessorCount - 2. Reserves a core for the Godot main thread and the OS. |
| Order within a phase | NOT guaranteed. Systems within one phase are parallel — make no assumptions. |
| Barrier between phases | Parallel.ForEach blocks ExecutePhase until completion. The next phase starts only afterward. |
| Context cache | _contextCache is built once in the constructor. The hot path runs without reflection and without allocations. |

## 5.3 ThreadLocal — why the guard is bound to the thread

```csharp
private static readonly ThreadLocal<SystemExecutionContext?> _current = new();
```

ThreadLocal<T> gives every thread its own independent storage. A pool thread from Parallel.ForEach has its own _current. The Godot main thread has its own (null, not in a scheduler context).

| Thread | _current value | Meaning |
|---|---|---|
| Scheduler thread inside Update() | The SystemExecutionContext of the specific system | GetComponent/SetComponent allowed per the declaration |
| Scheduler thread outside Update() | null (after PopContext) | Component access forbidden |
| Godot main thread | null | Domain code does not run here |
| Test without PushContext | null | The test MUST PushContext explicitly to exercise the guard |

> **🚫 async/await inside systems is forbidden** — await schedules the continuation on a different thread. _current.Value on the new thread is null. GetComponent() throws. Determinism is broken. Phase semantics are broken. async/await in Domain is strictly forbidden.

## 5.4 Race conditions — how to spot them

A data race = the result depends on the simultaneous access order of threads. In a simulator this is fatal: the bug surfaces hours into play.

| Scenario | Why dangerous | Project's defense |
|---|---|---|
| Two threads write into one ComponentStore | Data is overwritten unpredictably | DependencyGraph forbids W/W in one phase |
| One writes, another reads in the same phase | The reader sees a partially updated state | W/R is also split across phases |
| A mod registers a system during a tick | Breaks the _phases invariant when read inside Parallel.ForEach | Rebuild() only from the menu, never during a session |
| A bus handler subscribes during Publish | ConcurrentModificationException | DomainEventBus copies the list before iterating |

> **✅ Critical principle** — For a simulator's multithreading, an early crash is better than a silent bug. If an invariant is broken and the world cannot be trusted, the game MUST stop before the corrupted state lands in a save.

# 6. Event Bus — the two-step model

## 6.1 Bus architecture

Dual Frontier has 5 domain buses: Combat, Inventory, Magic, Pawns, World. Each is a separate DomainEventBus instance. This reduces lock contention and simplifies profiling.

```csharp
// GameServices — bus aggregator (docs/architecture/CONTRACTS.md)
public interface IGameServices
{
    ICombatBus    Combat    { get; }
    IInventoryBus Inventory { get; }
    IMagicBus     Magic     { get; }
    IPawnBus      Pawns     { get; }
    IWorldBus     World     { get; }
}
```

### How DomainEventBus.Publish() works

```csharp
// 1. Get the handler list for the event type
if (!_handlers.TryGetValue(eventType, out var handlersList)) return;
```

```csharp
// 2. Copy the list under lock — protection against changes during iteration
List<Delegate> handlersCopy;
lock (handlersList) { handlersCopy = new List<Delegate>(handlersList); }
```

```csharp
// 3. Invoke handlers outside the lock — no deadlock if a handler subscribes
foreach (var handler in handlersCopy)
    ((Action<TEvent>)handler)?.Invoke(evt);
```

## 6.2 Intent → Granted/Refused (two-step model)

The key pattern for resource mechanics. The system does not get the resource directly — it publishes an intent. Another system answers in the next phase.

| Step | Who | What is published | Phase |
|---|---|---|---|
| 1 | CombatSystem | AmmoIntent { RequesterId, AmmoType, Position } | Phase N |
| 2 | IntentBatcher | Collects every AmmoIntent for phase N | Phase N |
| 3 | InventorySystem | Flush AmmoIntent → AmmoGranted or AmmoRefused | Phase N+1 |
| 4 | CombatSystem | Subscribed to AmmoGranted — fires the shot | Phase N+1 |

```csharp
// CombatSystem — publishes the intent (does not block)
Services.Combat.Publish(new AmmoIntent { RequesterId = pawnId, AmmoType = AmmoType.Rifle });
```

```csharp
// InventorySystem — handles the batch of Intents in the next phase
var intents = _batcher.Flush<AmmoIntent>();
foreach (var intent in intents) {
    if (HasAmmo(intent.RequesterId, intent.AmmoType))
        Services.Combat.Publish(new AmmoGranted(intent.RequesterId));
    else
        Services.Combat.Publish(new AmmoRefused(intent.RequesterId));
}
```

> **💡 Principle** — The two-step model eliminates blocking request/response between systems. Each system stays independent. The scheduler arranges them via the dependency graph automatically.

# 7. Debugging — skills for the project

## 7.1 Reading stack traces

Read a stack trace from bottom to top: first locate the original throw (the source), not the spot where the exception bubbled out.

```csharp
System.InvalidOperationException: SystemExecutionContext is already set on this thread
   at DualFrontier.Core.ECS.SystemExecutionContext.PushContext(...)  ← SOURCE
   at DualFrontier.Core.Scheduling.ParallelSystemScheduler.ExecutePhase(...)
   at DualFrontier.Core.Scheduling.ParallelSystemScheduler.ExecuteTick(...)
```

This crash means: an attempt at a nested PushContext — the previously set context was not popped. Cause: PopContext was not in finally, and the previous Update() threw.

## 7.2 Types of isolation violations

| Exception / message | What was violated | How to fix |
|---|---|---|
| UndeclaredRead: T not in reads | The system reads a component not declared in [SystemAccess] | Add T to the reads: array of the attribute |
| UndeclaredWrite: T not in writes | The system writes a component without a declaration | Add T to the writes: array of the attribute |
| DirectSystemAccess | The system called GetSystem<T>() — a direct reference to another system | Replace with Publish through the bus |
| UnauthorizedBus | The system publishes to a bus not declared in bus: | Fix bus: nameof(IGameServices.X) |
| nested push detected | PushContext without a preceding PopContext | Make sure PopContext is in finally |
| Write conflict in Build() | Two systems write the same component — W/W conflict | Split the logic or merge the systems |
| Cyclic dependency in Build() | Cyclic dependency A→B→A | Reconsider reads/writes; break via events |

## 7.3 Logic vs architectural error

| Error type | Symptom | Where to look |
|---|---|---|
| Logic | Wrong damage calc, wrong formula, error in an if-condition | Inside the system's Update(), in components |
| Architectural | Isolation exception, W/W conflict, scheduler crash, contract violation | In the [SystemAccess] attributes, in the phase model, in ownership |
| Concurrency | Unstable crash — does not reproduce reliably, depends on order | In DependencyGraph: check that there is no W/R in one phase |

## 7.4 Tests as invariant proofs

In Dual Frontier a test is not a checkbox. It is a proof that a specific invariant holds. xUnit + FluentAssertions.

```csharp
// Test: the guard catches an undeclared read
[Fact]
public void GetComponent_UndeclaredRead_ThrowsIsolationViolation()
{
    var world = new World();
    var ctx = new SystemExecutionContext(world, "TestSystem",
        reads: Array.Empty<Type>(), writes: Array.Empty<Type>(),
        buses: Array.Empty<string>(), origin: SystemOrigin.Core,
        modId: null, faultSink: new NullModFaultSink());
```

```csharp
    SystemExecutionContext.PushContext(ctx);
    try {
        var id = world.CreateEntity();
        world.AddComponent(id, new TestComponent());
```

```csharp
        Action act = () => ctx.GetComponent<TestComponent>(id);
        act.Should().Throw<IsolationViolationException>();
    }
    finally { SystemExecutionContext.PopContext(); }
}
```

# 8. Study path — 14 days

Each day: 20–30 min of theory on one topic → 30–60 min of work with real project files → one practical check → a short note in the project journal.

## Days 1–7: Language and contracts

| Day | Topic | Project file | Practical check |
|---|---|---|---|
| 1 | class vs struct, readonly, nullable | EntityId.cs, ComponentStore.cs | Explain: why is EntityId a readonly struct, not a class? Where does nullable guard the contract? |
| 2 | Reference vs value copy | ComponentStore<T>.Get(), SetComponent() | Write code: change a HealthComponent through GetComponent/SetComponent. Confirm the change persists. |
| 3 | Interfaces as a contract | IComponent, IEvent, IModApi, IGameServices | Answer: what happens if you add a method to IComponent? Why is IGameServices not a class? |
| 4 | Generics: reading signatures | ComponentStore<T>, Query<T1,T2>(), Subscribe<TEvent>() | Read the ComponentStore<T> signature aloud. Explain where T : IComponent. |
| 5 | Attributes and reflection | [SystemAccess], DependencyGraph.AddSystem() | Find in code where GetCustomAttribute reads the attribute. Add a test system without the attribute — verify the crash. |
| 6 | Exceptions and finally | ParallelSystemScheduler, SystemExecutionContext | Remove the finally around PopContext in test code. Run the test. Understand what broke. |
| 7 | Project collections | World._stores, _contextCache, _allowedReads | For each field, explain: why this collection (order? lookup? thread safety?). |

## Days 8–14: Multithreading and debugging

| Day | Topic | Project file | Practical check |
|---|---|---|---|
| 8 | Race conditions: W/W and W/R | DependencyGraph.cs, DependencyGraphTests.cs | Find the Build_WriteWriteConflict_Throws test. Run it. Explain why it exists. |
| 9 | ThreadLocal — mechanics | SystemExecutionContext._current, PushContext/PopContext | Explain: why is _current ThreadLocal and not a static field? What would happen without ThreadLocal? |
| 10 | Parallel.ForEach and the barrier | ParallelSystemScheduler.ExecutePhase(), ExecuteTick() | Sketch the diagram: phases 0→1→2 and where the barrier sits. Why is system order within a phase not guaranteed? |
| 11 | TickScheduler — frequencies | TickScheduler.cs, TickRates.cs | Answer: a system with [TickRate(SLOW)] runs how many times in 10 seconds? And REALTIME? |
| 12 | Reading stack traces | IsolationViolationException in tests | Run an isolation test for a deliberate violation. Find the first throw line in the stack trace. |
| 13 | Tests as proof | DependencyGraphTests.cs, ComponentStoreTests.cs | Read the Build_LinearChain_OrderedPhases test. Explain exactly what it proves about the architecture. |
| 14 | Review and documentation | All module READMEs, ARCHITECTURE.md | Update your notes. Mark what became canonical. Write one new test for a favorite invariant. |

> **✅ Daily rule** — If you read code and cannot explain why it is written that way, that is a signal to study. Not "what does the code do" but "what invariant does it protect".

# 9. Quick reference — project rules

## Architectural prohibitions

| Prohibition | Why | Alternative |
|---|---|---|
| async/await in Domain | The continuation is on another thread; ThreadLocal is null; determinism is broken | Synchronous code, buses for communication |
| Direct access to World from a system | Bypasses the isolation guard, undeclared access | GetComponent<T> through SystemBase (through the context) |
| GetSystem<TSystem>() inside a system | Direct system coupling — an isolation violation | Publish an event to a bus |
| Mod reference to Core | AssemblyLoadContext blocks it physically | Only DualFrontier.Contracts |
| W/W on one component | Data race — silent bug in the save | DependencyGraph forbids it during Build() |
| Rebuild() of the scheduler during a tick | Breaks _phases under Parallel.ForEach | Only from the menu between sessions |

## Key ECS guarantees

| Guarantee | Where it is enforced |
|---|---|
| No W/W or W/R on one component within a phase | DependencyGraph.Build() + topological sort |
| A system sees only declared components (DEBUG) | SystemExecutionContext._allowedReads/_allowedWrites |
| Each thread has its own independent context | ThreadLocal<SystemExecutionContext?> _current |
| PopContext is always called, even on exceptions | try/finally in ParallelSystemScheduler.ExecutePhase() |
| A mod cannot crash the game with an isolation violation | IModFaultSink → ModFaultHandler unloads the mod |
| The next phase starts only after the current finishes | Parallel.ForEach blocks until every system in the phase completes |

## Quick choice

| Situation | Solution |
|---|---|
| Need a small immutable identifier | readonly record struct (EntityId, GridVector) |
| Need a stateful object or a service | class, sealed |
| Need a type-safe collection of components | ComponentStore<T> where T : IComponent |
| Need a link between systems | Publish an event to a bus, not GetSystem<T>() |
| Need a resource (bullet, mana, inventory slot) | AmmoIntent / ManaIntent → two-step model |
| Need to do something after entity destruction | [Deferred] on DeathEvent |
| Need to preempt a phase immediately | [Immediate] — use extremely sparingly |
| Need component access from a test without a system | Explicit PushContext in the test, explicit PopContext in finally |

Contracts give shape. Systems give behavior. The scheduler protects the rules. Tests prove that the rules hold.

Dual Frontier · C# and multithreading · Project learning artifact
