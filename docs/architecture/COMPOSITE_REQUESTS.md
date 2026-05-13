---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-COMPOSITE_REQUESTS
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-COMPOSITE_REQUESTS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-COMPOSITE_REQUESTS
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-COMPOSITE_REQUESTS
---
# Composite requests

A one-step Intent solves the problem when exactly one resource on one bus is required. But even a "mana arrow" shot needs two confirmations at once: a bullet on `IInventoryBus` and mana on `IMagicBus`. A direct "get both responses and act" approach produces race conditions inside a multithreaded phase. v0.2 introduces two-phase commit through a single mediator.

## Problem

The initiator (e.g., `CombatSystem`) publishes two independent intents: `AmmoIntent` and `ManaIntent`. Both are processed by batch systems within one phase in parallel. Four outcomes are possible: (granted, granted), (granted, refused), (refused, granted), (refused, refused). In three of four cases the action is impossible, but the partially debited resources have already been spent — the bullet is reserved, mana is gone, and the rollback has to be done manually on each bus's side. Any error is a permanent leak.

On top of that: the initiator must collect both responses and figure out that they are about the same action. In batches with hundreds of intents the `RequesterId` correlation breaks down — one pawn within a tick may receive ammo and mana from completely unrelated events.

## Solution

A request that touches N buses is published **once** as `CompoundShotIntent` (or another composite type) with a shared `TransactionId`. A dedicated mediator system, `CompositeResolutionSystem`, subscribed to every involved bus, collects partial responses by `TransactionId` and publishes the final `ShootGranted` or `ShootRefused` (with `ShotRefusalReason`).

### Sequence

```
 Shooter                 Combat              Inventory           Magic           CompositeResolution
    │                       │                    │                 │                      │
    │── CompoundShotIntent ─┤                    │                 │                      │
    │    (TransactionId)    │                    │                 │                      │
    │                       ├───── AmmoIntent ──►│                 │                      │
    │                       ├────────────── ManaIntent ───────────►│                      │
    │                       │                    │                 │                      │
    │                       │                 (batch phase)                                │
    │                       │                    │                 │                      │
    │                       │◄── AmmoGranted ────┤                 │                      │
    │                       │                    │                 │                      │
    │                       │◄──────────────── ManaGranted ────────┤                      │
    │                       │                    │                 │                      │
    │                       │                                                              │
    │                       ├── forwards partials by TransactionId ────────────────────►  │
    │                       │                                                              │
    │                       │                                   (collects pair)            │
    │                       │                                                              │
    │                       │◄──────────── ShootGranted ──────────────────────────────────┤
    │◄── ShootGranted ──────┤                                                              │
    │                       │                                                              │
```

`CompositeResolutionSystem` lives in a separate phase between intent collection and application — see [THREADING](./THREADING.md), Phase 2 (Intent Collection).

### Determinism rules

- `TransactionId` is monotonic within one tick: the generator is an atomic counter held by `CompositeResolutionSystem`.
- Every partial response carries its `TransactionId`. Without one, a response is treated as part of an ordinary Intent/Granted exchange and does not enter composite resolution.
- `CompositeResolutionSystem` keeps a `TransactionId → PartialState` dictionary with the expected response count. As soon as every response is collected, the final result is published and the entry is removed.
- If fewer responses than expected arrive by the end of the phase, the transaction is marked as stranded and `ShootRefused` is published with reason `PartialTimeout`.

### Rollback on partial refusal

If one bus answered Granted and the other Refused, the already debited resource is returned via a Refund event on the corresponding bus. For example, `AmmoGranted` means the bullet is already reserved in `InventorySystem`. On final `ShootRefused`, `CompositeResolutionSystem` publishes `AmmoRefunded` (TODO Phase 4), and `InventorySystem` returns the bullet to storage.

Refund events are marked `[Deferred]` — their delivery must not compete with new intents in the same phase. The bus receiving a Refund processes it in the next phase alongside new Intents.

## Generalization

Any request that touches more than one bus goes through `CompositeResolutionSystem`. The list of supported composite intents:

- `CompoundShotIntent` — bullet + mana (enchanted shot).
- `CompoundCraftIntent` — materials + power + workbench (TODO Phase 4).
- `CompoundRitualIntent` — ether + mana + anchor item (TODO Phase 6).

Each is described by its own pair of `...Granted` / `...Refused` with a corresponding Refusal enum.

## Related task: damage merging

The same pattern of "collect many intents and emit one deterministic result" is used for damage — but with a different mediator: `ComboResolutionSystem`. See [COMBO_RESOLUTION](./COMBO_RESOLUTION.md). The difference: composite waits for responses from different buses for one transaction; combo collects homogeneous `DamageIntent`s on a single target and orders them.

## See also

- [EVENT_BUS](./EVENT_BUS.md) — the two-step Intent → Granted model.
- [RESOURCE_MODELS](./RESOURCE_MODELS.md) — choosing between Intent and Lease.
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md) — `ComboResolutionSystem` for damage.
- [THREADING](./THREADING.md) — the Intent Collection phase.
