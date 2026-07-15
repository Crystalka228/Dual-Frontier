---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-EVENT_BUS_V2
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: "0.1.0"
next_review_due: post-ratification closure
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-EVENT_BUS_V2
---
# Event Buses

The dual bus architecture: five managed domain buses carrying every production event today, and the sovereign native three-tier kernel bus (К-L15) that exists fully behind a C ABI but carries almost none of that traffic yet.

> **Document class: authored-rework (current-truth candidate).** Successor of `docs/architecture/historical/EVENT_BUS.md` (DOC-A-EVENT_BUS, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. Becomes the LOCKED authority upon Crystalka ratification per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7; until then the predecessor remains the last-ratified reference and prevails on conflict.
> **Ratification checklist:** [ ] content spot-audit at ratification HEAD · [ ] lifecycle AUTHORED → LOCKED, version → 1.0.0 · [ ] `next_review_due` set · [ ] predecessor register rationale updated · [ ] the CONCURRENCY/TIME draft sections cited in §5–§7 still stand at ratification.

## Status

| Field | Value |
|---|---|
| Role | normative-current-candidate |
| Successor of | `docs/architecture/historical/EVENT_BUS.md` (DOC-A-EVENT_BUS, LOCKED v2.0.0, now SUPERSEDED) |
| Scope | Managed `DomainEventBus` delivery (sync/`[Deferred]`/`[Immediate]`); the native three-tier bus as implemented; today's managed/native division of labor; fault-isolation and capacity truths as coded; Intent and Lease models; subscription lifecycle. |
| Non-goals | К-L15/К-L15.1 canonical text (KERNEL_ARCHITECTURE.md Part 0); backpressure/fault-crossing law (CONCURRENCY_AND_MEMORY_MODEL.md, AUTHORED draft); visibility/determinism law (TIME_AND_CONSISTENCY_MODEL.md, AUTHORED draft); scheduler phase mechanics (THREADING.md); background-queue persistence design (PERSISTENCE_SNAPSHOT_CONTRACT.md, AUTHORED draft). |
| Authority domains | **event-routing** — current managed delivery mechanics and current native-bus wiring. Native-bus *sovereignty* itself remains К-L15's domain: this document reports the wiring under that law, it does not set the law. |
| Defers to | KERNEL_ARCHITECTURE.md Part 0 · CONCURRENCY_AND_MEMORY_MODEL.md (AUTHORED draft) · TIME_AND_CONSISTENCY_MODEL.md (AUTHORED draft) · THREADING.md · MOD_OS_ARCHITECTURE.md · PERSISTENCE_SNAPSHOT_CONTRACT.md (AUTHORED draft) — full table in Cross-references. |

---

## §1 Why domain buses, not a global one

Events are the only horizontal link between systems — direct system-to-system calls are forbidden by the architecture (ISOLATION.md). A single global bus becomes a single lock point at scale: one subscriber table for every event type concentrates contention in one structure. Dual Frontier splits the managed bus by game domain — each is a self-contained `DomainEventBus` with its own subscriber table and deferred queue:

```csharp
public interface IGameServices {
    ICombatBus Combat { get; }       IInventoryBus Inventory { get; }
    IMagicBus Magic { get; }         IPawnBus Pawns { get; }
    IWorldBus World { get; }
}
```

The five-bus list is canonical per `src/DualFrontier.Contracts/Bus/IGameServices.cs` (the historical sixth `IPowerBus` was deleted with the Power subsystems at К8.3+К8.4). Benefits: locks of unrelated domains never overlap, per-bus logging, and `[SystemAccess(bus: nameof(IGameServices.Combat))]` states where a system may publish at all.

## §2 Managed delivery: synchronous, `[Deferred]`, `[Immediate]`

`DomainEventBus` (`src/DualFrontier.Core/Bus/DomainEventBus.cs`) resolves a delivery mode per event type from attributes, cached per type (`ResolveDeliveryMode`, `:192-199`):

- **Synchronous (default).** `Publish` invokes every subscriber inline before returning (`DeliverSync`, `:147-167`): subscriber list snapshotted under `lock`, callbacks invoked outside it. A handler must not block — it stalls the phase.
- **`[Deferred]`.** `Publish` enqueues onto a `ConcurrentQueue<DeferredItem>` (`:96-99`) instead. Delivery happens at the next `FlushDeferred()` call — in production, the sole call site is `ParallelSystemScheduler.cs:166-167`, immediately after the phase's `Parallel.ForEach` barrier. `DeathEvent` is `[Deferred]` so every parallel system has finished reading `HealthComponent` first; the Inventory mutation flow (`ItemAddedEvent`/`ItemRemovedEvent`/`ItemReservedEvent`) works the same way.
- **`[Immediate]`.** The attribute and its resolver branch exist (`ImmediateAttribute.cs`, `DeliveryMode.Immediate`), but `Publish` routes `Sync` and `Immediate` through the identical `DeliverSync` call (`:91-103`) — no behavioral difference exists in code today, and no production event type carries the attribute. It is a declared, unused escape hatch, functionally equal to default synchronous delivery.

Delivery mechanics: `Subscribe<T>` captures the calling `SystemExecutionContext` alongside the handler (`:36-58`; delegate-value-equality dedupe, since a method-group handler allocates a fresh delegate per conversion). `FlushDeferred` (`:115-135`) drains a snapshot of the queue; for each event, each subscriber's *captured* context is re-pushed for the handler's duration and popped in `finally` (`InvokeDeferred`, `:169-187`) — the handler runs under its own system's identity, not the publisher's. That is why a publisher with `writes: []` can publish `[Deferred] ItemRemovedEvent` and the mutation happens in `InventorySystem`'s own context one phase later. Re-entrancy is bounded: a `[Deferred]` event published from inside a deferred handler delivers on the *next* flush (`:111-113`), not the current one.

## §3 The native three-tier bus (К-L15)

The kernel owns sovereign event routing for kernel-space and cross-layer events per К-L15 (canonical text: KERNEL_ARCHITECTURE.md Part 0). Implementation is the А'.7.5 four-TU split — `bus_fast.cpp`, `bus_normal.cpp`, `bus_background.cpp`, `bus_common.cpp` over `bus_native_internal.h`; public C ABI `include/bus_native.h`. An event type declares its tier once via `[EventTier(BusTier.…)]` (`src/DualFrontier.Contracts/Bus/EventTierAttribute.cs`); undeclared types default to `Normal`.

**Fast — synchronous bypass.** `df_bus_publish_fast` snapshots the subscriber list under the Fast mutex, releases it, then invokes each callback on the publisher's thread holding no bus mutex at all (`bus_fast.cpp:30-52`). Contract: bounded execution (≤1 ms target), no blocking, no GC allocation — the bus does not time-box callbacks; `FastTierContractMonitor` (`src/DualFrontier.Application/Bus/FastTierContractMonitor.cs`) measures latency and raises `ViolationsExceededThreshold` per `(modId, event)` on repeated breach. No pending queue exists for this tier.

**Normal — batched dispatch at the phase boundary.** `df_bus_publish_normal` copies the payload into the tier's pending queue under the Normal mutex, dropped with `0` if the type has no subscribers (`bus_normal.cpp:31-44`). Delivery happens only when the driver calls `df_bus_drain_normal_batch` (`:46-72`): pending events and a subscriber snapshot taken under the lock, callbacks fired outside it — К-L7 atomic-from-observer preserved within the batch boundary.

**Background — coalesce + idle-slot dispatch.** `df_bus_publish_background` enqueues the payload with a caller-supplied `coalesce_key`, unconditionally (`bus_background.cpp:31-44`). Dispatch policy lives in `background_queue.cpp`: events sharing `(type_id, coalesce_key)` coalesce through the type's registered coalesce function; `df_background_queue_dispatch_idle_slot(budget_micros)` drains within the caller's remaining tick budget (`:139-189`). Capacity and save-format specifics: §6.

**К-L15.1 — three-tier independence**, at three structural layers: **(1) state** — each tier owns its own `std::mutex`, `next_seq`, subscriber map, and pending queue where applicable (`FastTierState`/`NormalTierState`/`BackgroundTierState`, `bus_native_internal.h:57-78`); no shared mutable state. **(2) runtime** — subscription ids encode tier in the high 8 bits, per-tier sequence in the low 56 (`TIER_SHIFT = 56`, `:96-112`; `bus_native.h:67`) — cross-tier id collisions structurally impossible; `df_bus_unsubscribe` decodes the tier bit (`bus_common.cpp:44-64`); `df_bus_clear` locks the three tier mutexes in fixed **fast → normal → background** order (`:68-85`). **(3) compile-time** — the four-TU split itself.

Cross-tier re-entrancy is safe by construction: a Fast subscriber may publish to any tier from inside its callback because Fast callbacks fire outside the per-tier mutex — the pre-split single shared mutex made this a deadlock hazard, closed at А'.7.x and held by the S10 probe (`tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs:1009-1010`).

Every subscriber record carries a `mod_id` (`0` for Core/vanilla), enabling per-mod bulk unsubscribe (`df_bus_unsubscribe_{fast,normal,background}_by_mod`) — consumed by the native mod-unload primitive (§10). Fast/Background publish/subscribe additionally require per-FQN per-tier capability tokens in the manifest (MOD_OS_ARCHITECTURE.md §3.2; token construction in `KernelCapabilityRegistry`).

## §4 Division of labor today

What actually routes where, verified on disk:

- **Managed events take the managed path.** Systems publish through `SystemBase.Services` → domain bus; mods publish/subscribe through `IModApi`, routed by `ModBusRouter` (reflects `IGameServices` properties against each event's `[EventBus]` attribute, `ModBusRouter.cs`) to the correct bus after a capability check in `RestrictedModApi.Publish`/`Subscribe` (`RestrictedModApi.cs:157-190`). Every delivery mechanic in §2 is live production behavior.
- **The managed→native routing facade exists but is dormant.** `BusFacade` (`src/DualFrontier.Application/Bus/BusFacade.cs`) maps event types to FNV-1a type ids (`Fnv1a32`, `:176-187`), reads the tier, and publishes through `ManagedBusBridge` — but only when `UseNativeBusForDispatch` is set, and it **defaults `false`** (`:49`). No production code constructs a `BusFacade`; the native dispatch path is exercised only by the scheduler stress/extreme test suites (e.g. S10, §3).
- **Two native-bus touchpoints are live in production.** (1) `GameLoop` drains the Background tier after every fixed step with whatever tick budget remains (`ManagedBusBridge.DrainBackgroundBatch`, called at `GameLoop.cs:118-128`). (2) The mod-unload chain invokes the native unload primitive: `ModIntegrationPipeline.cs:668` calls `ModUnloadInterop.UnloadModNativeState`, which wraps native `df_scheduler_unload_mod_native_state` (`mod_unload.cpp:45-121`) — its T1/T2/T3 steps call the three per-tier `_by_mod` unsubscribes (`:83,87-89,93`), vacuously today since production registers no native subscribers.

> **FENCED (target / planned — not current truth):** the sovereign-authority switch — native tiers becoming the dispatch path for managed events per К-L15 — is Planned; scheduling in `docs/ROADMAP.md`, cutover gates per EXECUTION_AUTHORITY_MATRIX.md §3 (AUTHORED draft). `BusFacade` and the bridge are the cutover scaffolding for that switch, not cruft — they carry their deletion trigger with the gates, not before.

## §5 Fault isolation is asymmetric across delivery modes

Sync delivery isolates a faulting subscriber; deferred delivery does not. `DeliverSync` wraps each subscriber invocation in `try { sub.Invoker(evt); } catch (Exception ex) { Console.WriteLine(...); }` and continues to the remaining subscribers (`DomainEventBus.cs:147-167`; try/catch `:158-166`). `InvokeDeferred` (`:169-187`) wraps the same call in `try { ... } finally { PopContext(); }` — **no `catch` at all.** A throwing `[Deferred]` handler unwinds `InvokeDeferred` → the `foreach` in `FlushDeferred` → `GameServices.FlushDeferred` → the scheduler's post-barrier flush (`ParallelSystemScheduler.cs:166-167`) — propagating into the scheduler rather than being contained per-subscriber.

This is not hypothetical: `DeathEvent` and the Inventory trio are all `[Deferred]` (§2) — the exact events chosen for cross-system ordering are the ones with no per-handler fault isolation. Even the "good" sync path under-reports: its catch logs to `Console.WriteLine` (`:164`), not to any fault sink, so a caught fault is invisible to diagnostics too (§7).

The normative fix — symmetric per-subscriber isolation in both modes, mod-origin faults routed to a fault sink — is CONCURRENCY_AND_MEMORY_MODEL.md §7 (AUTHORED draft), which independently confirmed the same file:line ranges. This document states the fact; that document states the law.

## §6 Capacity truths

**Managed queues are unbounded.** `DomainEventBus` backs subscribers with a plain `ConcurrentDictionary<Type, List<Subscription>>` and deferred items with a plain `ConcurrentQueue<DeferredItem>` (`DomainEventBus.cs:27-28`) — no depth cap, no byte cap, anywhere in the managed path.

**The native Background tier's 10 MB cap is enforced in exactly one function.** `background_queue.cpp` defines `DEFAULT_MAX_BYTES = 10u*1024u*1024u` with an 80% warn threshold (`:15-16`) and an `apply_drop_oldest_locked` helper (`:100-109`) — called **only** inside `df_background_queue_force_coalesce` (`:312-332`). It is **not** on the publish path (`df_bus_publish_background` appends unconditionally, `bus_background.cpp:31-44`) and **not** on the dispatch path (`df_background_queue_dispatch_idle_slot`, `:139-189`, coalesces but checks only a per-event time budget, never the byte cap). Nothing in the tick loop calls `force_coalesce` automatically — §4's live drain touchpoint is `dispatch_idle_slot`. Absent an explicit `force_coalesce` call, the Background queue grows without a size check.

**BACKPRESSURE and EXPAND are accepted parameters, not implemented behavior.** `df_background_queue_configure` validates and stores the `strategy` enum (`:118-137`), but per the in-source comment (`:124-130`, verbatim): "К10.2 default: only DROP_OLDEST is implemented. BACKPRESSURE / EXPAND deferred к К-extensions… Configuration accepted (records intent) но behavior remains drop-oldest." Requesting either strategy silently gets drop-oldest semantics whenever `force_coalesce` happens to run.

Full resource × operation matrix and the normative backpressure law: CONCURRENCY_AND_MEMORY_MODEL.md §3.3 (AUTHORED draft) — facts here, law there.

## §7 Observability is inverted

The path carrying production traffic has no instrumentation; the path carrying almost none has a full diagnostic ABI. `DomainEventBus` keeps no counters of any kind — no publish count, no subscriber count, no queue-depth gauge. The native side exposes per-tier subscriber counts (`df_bus_subscriber_count_fast/normal/background`), Background queue size and bytes used (`df_background_queue_size`), and the saturation (drop-oldest) counter (`df_background_queue_saturation_events`) — mirrored managed-side in `src/DualFrontier.Core.Interop/BackgroundQueueInterop.cs` and `EventTypeRegistryInterop.cs`. `FastTierContractMonitor` (§3) is the sole managed-side instrument, and it watches the native Fast tier, not the managed buses.

Net effect: the 100%-of-production-traffic managed path is a runtime black box; the near-zero-traffic native path (§4) is fully observable. A deferred queue that hasn't drained by tick end signals a blocking handler — the only managed-side diagnostic is inference from a stalled tick, not a counter.

## §8 Two-step Intent → Granted/Refused model

Synchronous request/response inside a multithreaded phase is a trap: either a blocking call (kills parallelism) or `TaskCompletionSource` machinery (breaks the declaration model). The architecture uses intents and batch resolution: **(1) Intent phase** — the requester publishes an intent and does not block (`ManaIntent`, `AmmoIntent`, on disk in `src/DualFrontier.Events/`). **(2) Resolution phase** — the owning system collects the phase's intents and answers `ManaGranted`/`ManaRefused`, `AmmoGranted`/`AmmoRefused`; the requester reacts one phase later.

`IntentBatcher` (`src/DualFrontier.Core/Bus/IntentBatcher.cs`) carries the batching: intents enqueue during a phase and flush as a typed snapshot, so a resolver walks its cache once per phase, not once per request. Vocabulary and batcher are on disk; several Magic-domain resolution handlers are explicit roadmap stubs marked as such in source. Multi-bus requests compose through two-phase commit — COMPOSITE_REQUESTS.md.

## §9 Lease model

Intents target discrete requests (one bullet, one mana charge); a resource draining *every tick* (a spell channel, shield upkeep) needs a lease instead of an intent chain per tick:

```
Open → Active (drain per tick) → Closed
```

**Open.** The initiator publishes `ManaLeaseOpenRequest` (`DrainPerTick`, `MinDurationTicks`, `MaxDurationTicks`); `ManaLeaseRegistry` (`src/DualFrontier.Systems/Magic/Internal/`) reserves `MinDurationTicks × DrainPerTick` up front (reserve-then-consume) and answers `ManaLeaseOpened` with a `LeaseId` or `ManaLeaseRefused` with a `RefusalReason`. **Active.** Each tick drains `DrainPerTick`; on exhaustion the lease extends or closes. **Closed.** `ManaLeaseClosed` (`[Deferred]`) with a `CloseReason`; the reserve remainder returns to the mana component.

Reserve-then-consume guarantees a lease cannot abort on its first tick because a parallel intent consumed the resource between Open and first drain. All four lease event types are on disk in `src/DualFrontier.Events/Magic/`. Intent-vs-Lease selection rules: RESOURCE_MODELS.md.

## §10 Subscription lifecycle

A subscription requires an unsubscription — the bus holds a reference to the handler, and a missing `Unsubscribe` is a leak. Systems subscribe in `OnInitialize()` (the scheduler pushes the execution context for every `Initialize` call, which is why `Services` is reachable there) and unsubscribe in `OnDispose()`.

Mods do not manage this manually: `RestrictedModApi.Subscribe` wraps the handler (re-pushing the captured execution context around each invocation) and records the subscription; the unload chain removes every recorded subscription when the mod's ALC unloads — managed-side from the recorded list, native-side via the per-mod bulk unsubscribe reached through `df_scheduler_unload_mod_native_state` (§4, `mod_unload.cpp:81-93`).

---

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) | defers-to | Part 0 canonical texts: К-L15 (native bus authority), К-L15.1 (tier independence), К-L9 (vanilla=mods facade uniformity). |
| [CONCURRENCY_AND_MEMORY_MODEL](./CONCURRENCY_AND_MEMORY_MODEL.md) (AUTHORED draft) | defers-to | §3.3 resource×operation matrix, §5 lock-order law, §7 fault-crossing law — normative resolution of §5–§6's facts. |
| [TIME_AND_CONSISTENCY_MODEL](./TIME_AND_CONSISTENCY_MODEL.md) (AUTHORED draft) | defers-to | Visibility table, deferred-flush happens-before edges, determinism classes. |
| [THREADING](./THREADING.md) | defers-to | Phase barrier mechanics that `FlushDeferred`'s sole call site depends on. |
| [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) | defers-to | Capability grammar, unload chain consuming the native per-mod unsubscribe. |
| [PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (AUTHORED draft) | defers-to | Background-queue wire-format inclusion and versioning at the save boundary. |
| [RESOURCE_MODELS](../mechanics/RESOURCE_MODELS.md) | cites | Intent vs. Lease selection rules. |
| [COMPOSITE_REQUESTS](../mechanics/COMPOSITE_REQUESTS.md) | cites | Multi-bus two-phase commit built on §8's intent model. |
| [FIELDS](./FIELDS.md) | cites | Shares the native per-mod unload primitive described in §4/§10. |

## Amendment protocol

A correction to a verified code claim (file:line, default value, call-site count) may be spot-fixed against current HEAD without a version bump beyond PATCH. A change to delivery semantics, tier wiring, or the managed/native division of labor is MINOR/MAJOR and requires re-verifying THREADING.md, ARCHITECTURE.md, and MOD_OS_ARCHITECTURE.md plus owner sign-off per FRAMEWORK.md §7.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.0 (this doc) | 2026-07-15 | Corpus rework, light-touch restyle (predecessor HOLDS against code): added the verified fault-isolation asymmetry (sync try/catch vs. deferred uncaught) with pointer to CONCURRENCY_AND_MEMORY_MODEL.md's law; added capacity truths (unbounded managed queues; 10 MB cap enforced only in `force_coalesce`; BACKPRESSURE/EXPAND accepted-but-unimplemented); reframed Diagnostics as observability inversion; anchors refreshed at HEAD `35364c2`. |
| 2.0.0 | pre-rework | Last state of predecessor `DOC-A-EVENT_BUS` (see historical/) — Native-truth rewrite; LOCKED; code-truth verified HOLDS at this session's audit. |
