---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-EVENT_BUS
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "2.0.0"
next_review_due: 2027-06-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-EVENT_BUS
---
# Event buses

Events are the only horizontal link between systems — direct system-to-system calls are forbidden by the architecture (see [ISOLATION](./ISOLATION.md)). Two bus layers exist in code, with distinct authority:

- the **managed domain buses** (`DomainEventBus` behind `IGameServices`) — the bus surface systems and mods program against, and the dispatch path production events actually travel today;
- the **native three-tier kernel bus** (Fast / Normal / Background, К-L15) — the kernel's sovereign event-routing substrate, fully implemented behind a C ABI, production-integrated today only at the Background-drain and mod-unload touchpoints (see "Division of labor" below).

Invariant wording for К-L15/К-L15.1 is owned by [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 (К-L invariants).

## Why domain buses, not a global one

A single bus for everything is the natural solution at a dozen systems and becomes a single lock point at a hundred: one subscriber table holding every event type concentrates contention in one structure. Dual Frontier splits the managed bus by game domain — each bus is a self-contained `DomainEventBus` with its own subscriber table and its own deferred queue.

```csharp
public interface IGameServices
{
    ICombatBus    Combat    { get; }
    IInventoryBus Inventory { get; }
    IMagicBus     Magic     { get; }
    IPawnBus      Pawns     { get; }
    IWorldBus     World     { get; }
}
```

The five-bus list is canonical per [src/DualFrontier.Contracts/Bus/IGameServices.cs](../../src/DualFrontier.Contracts/Bus/IGameServices.cs) (the historical sixth `IPowerBus` was deleted with the Power subsystems at К8.3+К8.4). Benefits: locks of unrelated domains never overlap, per-bus logging, and the `[SystemAccess(bus: nameof(IGameServices.Combat))]` declaration states where a system may publish at all.

## Managed delivery: synchronous, [Deferred], [Immediate]

`DomainEventBus` (`src/DualFrontier.Core/Bus/DomainEventBus.cs`) resolves a delivery mode per event type from attributes, cached per type:

- **Synchronous (default).** Handlers run inside `Publish`, before control returns, within the current scheduler phase. A handler must not block — it stalls the phase. Handler exceptions are caught and logged; publication continues to the remaining subscribers.
- **`[Deferred]`.** The event is enqueued (a `ConcurrentQueue`, safe for parallel publication within a phase) and delivered at the next phase boundary. Use it when ordering matters: `DeathEvent` is `[Deferred]` so that by delivery time every parallel system has finished reading `HealthComponent`. The Inventory mutation flow works the same way — `ItemAddedEvent` / `ItemRemovedEvent` / `ItemReservedEvent` are all `[Deferred]` on disk.
- **`[Immediate]`.** Synchronous delivery that is structurally prevented from ever entering the deferred queue. The attribute exists and is honored by the mode resolver (`src/DualFrontier.Contracts/Attributes/ImmediateAttribute.cs`); no production event type currently carries it — it is the declared escape hatch for phase-preempting signals, functionally equal to default synchronous delivery today.

Delivery mechanics: `Subscribe<T>` captures the current `SystemExecutionContext` (usually the subscribing system's `OnInitialize` context) alongside the handler. After each phase's `Parallel.ForEach` barrier, `ParallelSystemScheduler` invokes `IDeferredFlush.FlushDeferred()` on the `GameServices` aggregator, which drains every domain bus: for each queued event, each subscriber's **captured** context is re-pushed for the duration of the handler and popped in `finally`. The handler therefore runs under its own system's identity — its own `NativeWorld` access and declared `[SystemAccess]` rights — not the publisher's. That is why a publisher with `writes: []` (e.g. `HaulSystem`) can publish `[Deferred] ItemRemovedEvent` and the storage mutation happens in `InventorySystem`'s own context one phase later.

Re-entrancy: the drain operates on a snapshot — a `[Deferred]` event published from inside a deferred handler lands in the queue and is delivered on the **next** flush, keeping each phase boundary bounded.

## The native three-tier bus (К-L15)

The kernel owns sovereign event routing for kernel-space and cross-layer events: type registry, subscriber registry, payload dispatch, and tier-based delivery semantics are native authority (К-L15). The implementation is the four-TU split landed at А'.7.5 — `native/DualFrontier.Core.Native/src/bus_fast.cpp`, `bus_normal.cpp`, `bus_background.cpp`, `bus_common.cpp`, sharing the internal header `bus_native_internal.h`; the public C ABI is `include/bus_native.h`. An event type declares its tier once, managed-side, via `[EventTier(BusTier.…)]` (`src/DualFrontier.Contracts/Bus/EventTierAttribute.cs`); undeclared types default to Normal.

**Fast tier — synchronous bypass.** `df_bus_publish_fast` snapshots the subscriber list under the Fast mutex, releases it, then invokes each callback **on the publisher's thread without holding any bus mutex**. Subscriber contract: bounded execution (≤1 ms target), no blocking, no GC allocation — the bus does not time-box callbacks; the managed `FastTierContractMonitor` (`src/DualFrontier.Application/Bus/FastTierContractMonitor.cs`) measures invocation latency and raises a threshold event on repeated budget violations per (modId, event) pair. No pending queue exists for this tier.

**Normal tier — batched dispatch at the phase boundary.** `df_bus_publish_normal` copies the payload into the tier's pending queue under the Normal mutex (dropped with return 0 if the type has no subscribers). Delivery happens only when the driver calls `df_bus_drain_normal_batch`: pending events and a subscriber snapshot are taken under the lock, then callbacks fire outside it using the same batched-callback ABI shape as scheduler dispatch (`df_managed_system_batch`), preserving К-L7 atomic-from-observer within the batch boundary.

**Background tier — coalesce + idle-slot dispatch.** `df_bus_publish_background` enqueues the payload with a caller-supplied `coalesce_key`. The dispatch policy lives in a separate module, `background_queue.cpp`: events sharing `(type_id, coalesce_key)` coalesce through the event type's registered coalesce function; `df_background_queue_dispatch_idle_slot(budget_micros)` drains only as many events as fit the CPU budget the caller has left in the tick; saturation applies drop-oldest with a warning counter at a configurable size cap (default 10 MB, warn at 80%). The pending queue carries save-integrated serialization at the C ABI level (versioned wire format, schema v1).

**К-L15.1 — three-tier independence**, materialized at three structural layers:

1. *State layer* — each tier owns its own `std::mutex`, its own `next_seq` counter, its own subscriber map, and its own pending queue where applicable (`FastTierState` / `NormalTierState` / `BackgroundTierState` in `bus_native_internal.h`); no shared mutable state between tiers.
2. *Runtime layer* — subscription ids encode the tier in the high 8 bits and a per-tier sequence in the low 56 bits, so cross-tier id collisions are structurally impossible; `df_bus_unsubscribe` decodes the tier bit to dispatch; `df_bus_clear` acquires the three tier mutexes in fixed fast → normal → background order.
3. *Compile-time layer* — the per-tier source split itself (the four TUs above, А'.7.5).

Cross-tier re-entrancy is safe by construction: a Fast subscriber may publish to **any** tier from inside its callback, because Fast callbacks fire outside the (per-tier) mutex — the pre-split single shared mutex made this a deadlock hazard, closed at А'.7.x and held by the S10 re-entrancy probe in `tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs`.

Subscriber identity carries a `mod_id` (0 for Core/vanilla), enabling per-mod bulk unsubscribe (`df_bus_unsubscribe_*_by_mod`) — consumed by the native mod-unload primitive in the unload chain (see [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §9.5). Fast/Background publish and subscribe additionally require per-FQN per-tier capability declarations in the mod manifest (К-L15; token construction in `KernelCapabilityRegistry`).

## Division of labor today

What actually routes where, per the wiring on disk:

- **Managed events take the managed path.** Systems publish through `SystemBase.Services` → domain bus; mods publish/subscribe through `IModApi`, which routes via `ModBusRouter` to the correct domain bus after a capability check (`src/DualFrontier.Application/Modding/RestrictedModApi.cs`). All delivery semantics in the managed section above are live production behavior.
- **The managed→native routing facade exists but is dormant in production.** `BusFacade` (`src/DualFrontier.Application/Bus/BusFacade.cs`) maps event types to stable FNV-1a type ids, reads the tier, registers types in the native event-type registry, and publishes through `ManagedBusBridge`'s P/Invoke surface — but only when its `UseNativeBusForDispatch` flag is set, and the flag defaults to `false`. No production code constructs a `BusFacade`; the native dispatch path is exercised by the scheduler stress/extreme test suites.
- **Two native-bus touchpoints are live in production:** (1) `GameLoop` drains the Background tier after every fixed step with the tick budget remaining (`ManagedBusBridge.DrainBackgroundBatch`); (2) the mod-unload chain invokes the native unload primitive, which clears per-mod native bus/wake state (vacuously today, since production registers no native subscribers).
- The sovereign authority switch — native tiers becoming the dispatch path for managed events — is `Planned — see docs/ROADMAP.md §Native foundation tracks`.

## Two-step Intent → Granted/Refused model

Synchronous request/response in a multithreaded phase is a trap: either a blocking call (kills parallelism) or `TaskCompletionSource` machinery (breaks the declaration model). The architecture uses intents and batch resolution instead:

1. **Intent phase** — the requester publishes an intent and does not block (`ManaIntent`, `AmmoIntent` — event types on disk in `src/DualFrontier.Events/`).
2. **Resolution phase** — the owning system collects the phase's intents and answers with granted/refused events (`ManaGranted`/`ManaRefused`, `AmmoGranted`/`AmmoRefused`). The requester reacts one phase later.

`IntentBatcher` (`src/DualFrontier.Core/Bus/IntentBatcher.cs`) carries the batching: intents enqueue during a phase and flush as a typed batch snapshot, so a resolver walks its cache once per phase instead of once per request. The event vocabulary and batcher are on disk; several resolution handlers in the Magic domain are explicit roadmap stubs (marked as such in source), and multi-bus requests compose through two-phase commit — see [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md).

## Lease model

Intents target discrete requests: one bullet, one mana charge. When a resource drains **every tick** (a spell channel, shield upkeep), an intent chain per tick is redundant and survives aborts poorly. The Lease model covers these:

```
Open → Active (drain per tick) → Closed
```

- **Open.** The initiator publishes `ManaLeaseOpenRequest` with `DrainPerTick`, `MinDurationTicks`, `MaxDurationTicks`. `ManaLeaseRegistry` (`src/DualFrontier.Systems/Magic/Internal/`) checks the reserve, reserves `MinDurationTicks × DrainPerTick` up front (reserve-then-consume), and answers `ManaLeaseOpened` with a `LeaseId` — or `ManaLeaseRefused` with a `RefusalReason`.
- **Active.** Each tick drains `DrainPerTick` from the reserved pool; on exhaustion the lease extends or closes.
- **Closed.** `ManaLeaseClosed` (`[Deferred]`) with a `CloseReason`; the reserve remainder returns to the mana component.

Reserve-then-consume guarantees a lease cannot abort on its first tick because a parallel intent consumed the resource between Open and the first drain. All four lease event types are on disk in `src/DualFrontier.Events/Magic/`. Intent-vs-Lease selection rules: [RESOURCE_MODELS](./RESOURCE_MODELS.md).

## Subscription lifecycle

A subscription requires an unsubscription — the bus holds a reference to the handler, and a missing `Unsubscribe` is a leak. Systems subscribe in `OnInitialize()` — the scheduler pushes the execution context for every `Initialize` call, which is why `Services` is reachable there — and unsubscribe in `OnDispose()` (invoked through the mod teardown path for mod-origin systems).

```csharp
protected override void OnInitialize()
{
    Services.Combat.Subscribe<AmmoGranted>(OnAmmoGranted);
}

protected override void OnDispose()
{
    Services.Combat.Unsubscribe<AmmoGranted>(OnAmmoGranted);
}
```

Mods do not manage this manually: `RestrictedModApi.Subscribe` wraps the handler (re-pushing the captured execution context around each invocation) and records the subscription; the unload chain removes every recorded subscription when the mod's `AssemblyLoadContext` unloads — managed-side from the recorded list, native-side via the per-mod bulk unsubscribe in the same chain.

## Diagnostics

The managed `DomainEventBus` keeps no counters. The observable surface that exists:

- **Native diagnostic ABI** — per-tier subscriber counts (`df_bus_subscriber_count_fast/normal/background`), Background queue size and bytes used (`df_background_queue_size`), and the saturation (drop-oldest) event counter (`df_background_queue_saturation_events`); managed mirrors in `src/DualFrontier.Core.Interop/` (`BackgroundQueueInterop`, `EventTypeRegistryInterop`).
- **Fast-tier latency monitoring** — `FastTierContractMonitor` measures managed Fast subscriber invocations against the ≤1 ms budget and raises `ViolationsExceededThreshold` per (modId, event) for a fault-handler consumer.

A deferred queue that has not drained by the end of a tick means a handler is blocking — look for `lock` or `await` in handlers (`async` is forbidden in system code; see [THREADING](./THREADING.md)).

## See also

- [THREADING](./THREADING.md) — phase barriers, deferred flush timing, scheduler model.
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — Part 0 (К-L invariants): К-L15, К-L15.1, К-L9.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — capability model, unload chain consuming per-mod unsubscribe.
- [CONTRACTS](./CONTRACTS.md) — event/record conventions.
- [RESOURCE_MODELS](./RESOURCE_MODELS.md) — Intent vs Lease selection rules.
- [PERFORMANCE](./PERFORMANCE.md) — target metrics the bus design serves.

## Amendment protocol

This document is Tier 1 LOCKED. An amendment proceeds: (1) surface the change to the owner (Crystalka) — no default amendments to standing law; (2) record rationale; (3) semver — PATCH for correction, MINOR for additive sections, MAJOR for inverting described architecture; (4) bump `version` and `next_review_due` in the register mirror via a governance commit with a validate run folded in; (5) propagate to documents citing this one ([THREADING](./THREADING.md), [ARCHITECTURE](./ARCHITECTURE.md), [ISOLATION](./ISOLATION.md)).

## Change history

| Version | Date | Change |
|---|---|---|
| **2.0.0** | 2026-06-12 | Native-truth rewrite (Architecture Truth Cascade): authored the previously missing native three-tier bus spec from `bus_fast/normal/background/common.cpp` (К-L15/К-L15.1 — per-tier semantics, id encoding, cross-tier re-entrancy, coalesce + idle-slot, save integration); managed `DomainEventBus` delivery mechanics rewritten without the deleted `SetComponent`/`_allowedWrites` path; honest managed/native division-of-labor section added (`UseNativeBusForDispatch` default false; Background drain + unload primitive are the live native touchpoints); phantom profiling surface (`BenchmarkRegistry`, F3 overlay, dotTrace) replaced with the real diagnostic ABI. **MAJOR.** |
| 1.1.1 | 2026-06-02 (era) | DD-1 code-truth notice banner fencing the managed-only delivery story; minor pin fixes. Superseded by 2.0.0 (banner removed — the body is now code-truth). |
| 1.0 | 2026-04 (era) | Initial managed bus spec: five domain buses, three delivery modes, Intent→Granted, lease model, subscription lifecycle. |
