---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-RESOURCE_MODELS
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-RESOURCE_MODELS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-RESOURCE_MODELS
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-RESOURCE_MODELS
---
# Resource models

A resource in Dual Frontier is any quantity that one system spends and another tracks: ammunition in inventory, a mage's mana, shield durability, ritual slots. Previously every system picked a request style by feel, and the same scenario ended up written differently in different places. v0.2 pins two models and a rule for choosing between them.

## Two models

The model is chosen by a single question: is the resource consumed once or continuously?

### Intent → Granted/Refused — discrete request

Used for one-shot consumption. The step is atomic: either the resource is granted and debited, or refused. Examples:

- One bullet for a shot (`AmmoIntent` → `AmmoGranted` / `AmmoRefused`).
- A single mana charge for an instant spell (`ManaIntent` → `ManaGranted` / `ManaRefused`).
- A one-shot target for a job (`TargetIntent` → `TargetGranted` / `TargetRefused`).

The two-step handshake is described in [EVENT_BUS](./EVENT_BUS.md): publish the intent in one phase, batch response in the next.

### Lease — continuous consumption

Used when the resource drains every tick while the action lasts. Examples:

- Spell concentration (channel): mana drains every tick while the mage holds the channel.
- Shield maintenance: shield durability or mana decreases while the shield is up.
- Active ritual: ether drains while the ritual runs.

Lease is reserve-then-consume. On open, `MinDurationTicks * DrainPerTick` is reserved immediately: this guarantees that the lease will not abort on the very first tick due to a parallel intent's spend. After the reservation, drain proceeds as the action plays out.

```
Open → Active (drain per tick) → Closed
```

The lifecycle and events are described in the "Lease model" section of [EVENT_BUS](./EVENT_BUS.md).

## Choice table

| Scenario                              | Model            | Example event                                    |
|---------------------------------------|------------------|--------------------------------------------------|
| Shot, single bullet                   | Intent           | `AmmoIntent` → `AmmoGranted`                     |
| Instant spell, fixed mana             | Intent           | `ManaIntent` → `ManaGranted`                     |
| Target request for a job              | Intent           | `TargetIntent` → `TargetGranted`                 |
| Spell channel, mana drain per tick    | Lease            | `ManaLeaseOpenRequest` → `ManaLeaseOpened`       |
| Shield maintenance                    | Lease            | `ManaLeaseOpenRequest` → `ManaLeaseOpened`       |
| Active ritual                         | Lease            | `ManaLeaseOpenRequest` → `ManaLeaseOpened`       |

## Choice rule

- If the resource is spent **once** per action — Intent.
- If the resource is spent **every tick** — Lease.

Edge cases resolve in favor of Lease when the duration is not bounded to a single tick: it is simpler to close a lease with an explicit event than to emulate continuity through a chain of per-tick Intents.

## Reserve-then-consume in detail

On `ManaLeaseOpenRequest` the lease registry (`ManaLeaseRegistry`) checks for `MinDurationTicks * DrainPerTick` of mana. If insufficient — `ManaLeaseRefused` with `RefusalReason`. If sufficient — the reserve is debited from `ManaComponent` immediately, the lease receives a `LeaseId`, and `ManaLeaseOpened` is published. From there each tick `ManaSystem` drains `DrainPerTick` from the already reserved pool; on reserve exhaustion the lease either extends or closes.

This differs from the naive scheme of "check at start, debit per tick": under the naive scheme a parallel intent can eat the mana before the first drain and the lease aborts even though the player has already seen the start animation.

Lease closure:

- Explicit `ManaLeaseClosed` (`[Deferred]`) event by the owner's choice.
- `MaxDurationTicks` expiration.
- Abort — mana shortage even after extension.
- Owner death (`DeathEvent`).

In every case, `CloseReason` in the event names the cause — needed by the UI and the log.

## See also

- [EVENT_BUS](./EVENT_BUS.md) — the "Lease model" section.
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md) — Intent that spans multiple buses.
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md) — combining multiple damage intents.
- [THREADING](./THREADING.md) — phases in which Intent and Lease are processed.
