# Event buses

The event bus is the central mechanism for system interaction. Direct calls between systems are forbidden by the isolation guard, so every horizontal link runs through a bus. The right choice of delivery model and bus type directly determines the performance and correctness of multithreaded code.

## Why domain buses, not a global one

A single bus for everything is the natural solution while there are about a dozen systems. At a hundred systems and ten thousand entities it becomes a bottleneck: a `ConcurrentDictionary` holding subscribers of every type at once turns into a single lock point. Profiling shows the same pattern: 70% of bus time is spent on contention in one dictionary.

Dual Frontier splits the bus by game domain. Each bus is a self-contained `DomainEventBus` with its own subscriber table and its own buffer pool for deferred delivery.

```csharp
public interface IGameServices
{
    ICombatBus    Combat    { get; }
    IInventoryBus Inventory { get; }
    IMagicBus     Magic     { get; }
    IWorldBus     World     { get; }
    IPawnBus      Pawns     { get; }
    IPowerBus     Power     { get; } // Introduced in v0.3 §13.1
}
```

Advantages:

- Less contention: `CombatSystem` writes only to `Combat` and `InventorySystem` only to `Inventory`; their locks do not overlap.
- Easier to debug: logging is per-bus.
- Easier to profile: `dotTrace` shows each bus's time separately.
- Explicit declaration: the `[SystemAccess(bus: nameof(IGameServices.Combat))]` attribute specifies where the system is permitted to write at all.

## Synchronous, Deferred, Immediate

The bus supports three delivery modes. The mode is set by an attribute on the event class.

### Synchronous (default)

Handlers are called directly inside `Publish`, before control returns. All subscribers process the event within the current scheduler phase. The model is simple, but a handler MUST NOT block — it stalls the entire phase.

### [Deferred]

The event is placed in a per-`DomainEventBus` queue and delivered on the boundary between scheduler phases. Use it when ordering matters: `DeathEvent` is always marked `[Deferred]` so that by delivery time every parallel system has finished reading `HealthComponent`.

```csharp
[Deferred]
public sealed record DeathEvent : IEvent
{
    public required EntityId EntityId { get; init; }
    public required EntityId? KillerId { get; init; }
}
```

#### Delivery mechanics (v0.3)

`Subscribe<T>(Action<T>)` captures the current `SystemExecutionContext.Current` —
usually the context of the system whose `OnInitialize` is currently running. The
capture is stored alongside the handler.

`Publish<T>(T)` for `[Deferred]` enqueues `(eventType, evt)` onto the bus queue.
The queue is a `ConcurrentQueue`, safe for parallel publication from systems
within the same phase.

After the `Parallel.ForEach` barrier, `ParallelSystemScheduler.ExecutePhase`
invokes the internal `IDeferredFlush.FlushDeferred()` on the bus aggregator
(`GameServices`). It takes a snapshot of each bus's current queue, clears it,
and for each subscriber:

1. Pushes the subscriber context **captured at `Subscribe`**.
2. Invokes the handler.
3. Pops the context inside `finally`.

This delivers isolation-correct component mutation from a handler:
`SetComponent<T>` checks the subscriber context's `_allowedWrites`, not the
publishing system's. That is why a Phase 4 `HaulSystem` (writes=`[]`) can
safely publish `[Deferred] ItemRemovedEvent`, and `InventorySystem`
(writes=`[StorageComponent]`) applies the mutation from its own context in
the next phase.

Re-entrancy: a `[Deferred]` event published **inside** another `[Deferred]`
handler lands in the queue and is delivered on the next `FlushDeferred` call
(i.e., the next phase) — the current drain operates on a snapshot and does
not pick up new arrivals.

### [Immediate]

An extremely rare mode: the event is guaranteed to be delivered synchronously
from `Publish` and **never** lands in the `[Deferred]` queue. Used for critical
breaks — for example, a high-level `EtherSurgeCriticalEvent` when further
parallel work is meaningless.

```csharp
[Immediate]
public sealed record EtherSurgeCriticalEvent : IEvent
{
    public required EntityId CasterId { get; init; }
}
```

In the current implementation `[Immediate]` is functionally equivalent to default
synchronous delivery (since phases already serialize publication and handling),
but the attribute carries the declarative intent and explicitly prevents the
event from entering the deferred queue — important for UI-critical signals and
for future bus optimizations.

## Two-step Intent → Granted/Refused model

Synchronous `Request/Response` in a multithreaded environment is a trap: either a blocking call (kills parallelism) or complex synchronization with `TaskCompletionSource` (breaks the access declaration and the isolation guard). Dual Frontier uses intents and batch resolution.

```csharp
// DANGEROUS — blocks the thread inside a multithreaded phase.
var result = bus.Request<AmmoRequest, AmmoResult>(...);

// SAFE — two-step model.

// STEP 1 (intent collection phase):
// CombatSystem publishes the intent — does not block.
bus.Combat.Publish(new AmmoIntent {
    RequesterId = entityId,
    AmmoType    = weapon.RequiredAmmo,
    Position    = position
});
```

In the next phase another system collects every `AmmoIntent` at once and answers with a batch of `AmmoGranted` / `AmmoRefused`. The requester receives the answer one phase later and decides what to do there.

## Batch processing on the AmmoIntent example

`InventorySystem` receives the list of all `AmmoIntent`s for the phase and walks the ammo cache once — instead of a separate scan per intent.

```csharp
void OnAmmoIntentBatch(IReadOnlyList<AmmoIntent> intents)
{
    foreach (var intent in intents)
    {
        var granted = TryReserveFromCache(intent);
        bus.Inventory.Publish(granted
            ? new AmmoGranted { RequesterId = intent.RequesterId }
            : new AmmoRefused { RequesterId = intent.RequesterId });
    }
}
```

Batching equals one cache pass instead of N separate requests. For 100 pawns the gain is two-digit: instead of 100 scans 60 times per second, one pass every three frames. Numbers from the target metrics: `≤100 scans/sec` versus `6000 scans/sec` in RimWorld.

`IntentBatcher` in `DualFrontier.Core/Bus` is responsible for intent collection: handler systems subscribe to a batch, not to individual events.

## Lease model (v0.2 §12.1)

The two-step `Intent → Granted/Refused` model targets discrete requests: one bullet, one mana charge. When a resource is consumed not once but every tick (a spell channel, shield maintenance, an active ritual), a chain of Intents per tick is redundant and survives aborts poorly. For these scenarios v0.2 introduces the **Lease** model.

### Lifecycle

```
Open → Active (drain per tick) → Closed
```

- **Open.** The initiator publishes `ManaLeaseOpenRequest` with `DrainPerTick`, `MinDurationTicks`, and `MaxDurationTicks`. `ManaLeaseRegistry` checks the reserve, reserves `MinDurationTicks * DrainPerTick` (reserve-then-consume), and answers `ManaLeaseOpened` with a `LeaseId`. On shortfall: `ManaLeaseRefused` with a `RefusalReason`.
- **Active.** Each tick `ManaSystem` drains `DrainPerTick` from the reserved pool. On reserve exhaustion the lease either extends (if mana is available) or closes.
- **Closed.** Explicit closure through `ManaLeaseClosed` (`[Deferred]`) with `CloseReason` (`OwnerRequested`, `MaxDurationReached`, `Starvation`, `OwnerDied`). The reserve remainder returns to `ManaComponent`.

Reserve-then-consume guarantees that a lease will not abort on the first tick because of a parallel intent that consumed mana between `Open` and the first drain.

### New events

- `ManaLeaseOpenRequest` — opening request.
- `ManaLeaseOpened` — confirmation with a `LeaseId`.
- `ManaLeaseRefused` — refusal with a `RefusalReason`.
- `ManaLeaseClosed` (`[Deferred]`) — closure with a `CloseReason`.

Equivalent events exist for shields, rituals, and any other Lease-like resources — names follow the same `{Resource}Lease{Stage}` template.

The choice between Intent and Lease and the application rules are detailed in [RESOURCE_MODELS](./RESOURCE_MODELS.md).

## Subscription lifecycle

A bus subscription requires an unsubscription: the handler holds a reference to the subscriber, and without `Unsubscribe` in `Dispose` a memory leak follows. A system calls `Subscribe` in its `OnInitialize()` and `Unsubscribe` in its `OnDispose()`. The scheduler guarantees both methods are called.

```csharp
protected override void OnInitialize()
{
    Bus.Combat.Subscribe<AmmoGranted>(OnAmmoGranted);
    Bus.Combat.Subscribe<AmmoRefused>(OnAmmoRefused);
}

protected override void OnDispose()
{
    Bus.Combat.Unsubscribe<AmmoGranted>(OnAmmoGranted);
    Bus.Combat.Unsubscribe<AmmoRefused>(OnAmmoRefused);
}
```

A mod subscribes and unsubscribes through `IModApi` — the implementation itself tracks subscriptions and removes them when the `AssemblyLoadContext` is unloaded. The mod does not need to remember exactly what it subscribed to.

## Profiling

Every bus keeps counters: publication count, delivery count, average per-event handling time, peak batch size. The counters are exported to `BenchmarkRegistry` and displayed in the F3 diagnostic overlay.

Typical symptoms:

- **High `PublishCount`, low `SubscriberCount`** — the event is published but nobody listens. Likely a naming error or stale code.
- **Peak batch > 1000** — too many intents per phase; either tick the initiator less often or filter earlier.
- **The deferred queue is not draining by frame end** — a subscriber is blocking. Look for `lock` or `await` in handlers (`async` is forbidden — see [THREADING](./THREADING.md)).

For bottleneck analysis, the dotTrace timeline is plugged in — each bus is tagged with its own markers, which lets you see load distribution across domains over time.

## See also

- [THREADING](./THREADING.md)
- [CONTRACTS](./CONTRACTS.md)
- [PERFORMANCE](./PERFORMANCE.md)
