---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CORE-BUS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CORE-BUS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CORE-BUS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CORE-BUS
---
# Bus — Domain bus implementations

## Purpose
Implements the `IEventBus` and `IGameServices` contracts from
`DualFrontier.Contracts.Bus`. Each domain bus is a separate `DomainEventBus`
instance with its own subscription set. `IntentBatcher` is used by systems to
implement the two-step Intent → Granted/Refused model.

## Dependencies
- `DualFrontier.Contracts.Bus` — bus contracts.
- `DualFrontier.Contracts.Core` — the `IEvent` marker.

## Contents
- `DomainEventBus.cs` — single-domain bus implementation with a
  `ConcurrentDictionary` of subscriptions for thread-safe operation.
- `GameServices.cs` — composition of the six domain buses (Combat, Inventory, Magic, Pawn, Power, World); implements `IGameServices`.
- `IntentBatcher.cs` — collects intents within a phase and hands a batch to the
  handler in the next phase.

## Rules
- The bus does not hold references to the systems themselves — only to
  `Action<T>` delegates.
- Subscribe and unsubscribe are thread-safe operations (`ConcurrentDictionary`).
- Handlers are invoked synchronously. For long-running work, the handler MUST
  hand off the task to `IntentBatcher`.

## Usage examples
```csharp
var services = new GameServices();
services.Combat.Publish(new ShootAttemptEvent(shooterId));
services.Inventory.Subscribe<AmmoIntent>(batcher.Collect);
```

## TODO
- [x] Phase 1 — implement `DomainEventBus` with `[Deferred]` / `[Immediate]` support.
- [x] Phase 1 — implement `IntentBatcher` with two-phase collection and drain.
- [ ] Phase 2 — add telemetry (events/sec counters per bus).
