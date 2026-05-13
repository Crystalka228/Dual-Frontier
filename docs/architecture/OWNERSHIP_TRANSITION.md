---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-OWNERSHIP_TRANSITION
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-OWNERSHIP_TRANSITION
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-OWNERSHIP_TRANSITION
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-OWNERSHIP_TRANSITION
---
# Golem ownership transitions

A golem belongs to a mage through `GolemBondComponent`. The bond is not a binary "exists / does not exist" — it passes through several states: active, contested, abandoned, transferred. v0.2 pins the full list of states, the permitted transitions, and the rules for executing them.

## States

- **Bonded** — normal state. The golem has a living mage owner, mana drips from their pool, and the golem follows orders.
- **Contested** — the bond is being contested: another mage is running an interception ritual. The current owner remains, but the `TicksSinceContested` timer ticks.
- **Abandoned** — the owner is dead or has released the bond intentionally. The golem stands inactive and is available for a new bond.
- **Transferred** — an intermediate state spanning a single tick: the bond has already been broken from the previous owner, but the new one has not formally accepted it. Used only for the correct ordering of `[Deferred]` events.

## Transition table

| From          | To            | Trigger                                                          |
|---------------|---------------|------------------------------------------------------------------|
| `Bonded`      | `Contested`   | Another mage started an interception ritual — attacks the bond.  |
| `Contested`   | `Bonded`      | The `TicksSinceContested` timer expired; the original owner held. |
| `Contested`   | `Transferred` | The attacking mage finished the ritual; the bond passes to them. |
| `Transferred` | `Bonded`      | The new owner accepts the bond (one tick to formalize).          |
| `Bonded`      | `Abandoned`   | Owner dies (`DeathEvent`) or voluntarily breaks the bond.        |
| `Contested`   | `Abandoned`   | Owner dies during contestation — the bond is released.           |
| `Abandoned`   | `Bonded`      | A new mage performs the bond ritual.                             |

Every other transition is forbidden — attempting one throws `InvalidOwnershipTransitionException` in DEBUG and is silently ignored in RELEASE (with an error counter increment for diagnostics).

## State diagram

```
                  ritual succeeds
   ┌──────────────────────────────────────────────┐
   │                                              │
   ▼                                              │
┌──────────┐   contender starts ritual   ┌─────────────┐
│ Bonded   │ ───────────────────────────►│ Contested   │
│          │                             │             │
│          │◄────── timer expires ───────│             │
└────┬─────┘   (owner held)              └──────┬──────┘
     │                                          │
     │ owner dies            contender wins     │
     │ or releases           ritual             │
     ▼                                          ▼
┌────────────┐                          ┌──────────────┐
│ Abandoned  │                          │ Transferred  │
│            │                          │ (one tick)   │
└────┬───────┘                          └──────┬───────┘
     │                                         │
     │ new mage runs bond ritual               │ new owner accepts
     │                                         │
     └───────────────► Bonded ◄────────────────┘
```

## Events

Every transition publishes `GolemOwnershipChanged` (`[Deferred]`). Event fields:

- `GolemId` — `EntityId` of the golem.
- `PreviousOwnerId` — `EntityId?` of the previous owner (null when coming from `Abandoned`).
- `NewOwnerId` — `EntityId?` of the new owner (null when going to `Abandoned`).
- `PreviousMode` — state before the transition (`OwnershipMode`).
- `NewMode` — state after the transition (`OwnershipMode`).

A separate inbound event `GolemOwnershipTransferRequest` requests the start of an interception ritual. Handling: `GolemSystem` checks admissibility (the golem is not in `Transferred`, the attacker is actually a mage, the target is in range) and either moves it to `Contested` or replies with `GolemOwnershipRefused` (TODO Phase 6).

## The `GolemBondComponent` component

```csharp
public sealed class GolemBondComponent : IComponent
{
    public EntityId? BondedMage;       // null when Abandoned
    public OwnershipMode Mode;         // Bonded | Contested | Abandoned | Transferred
    public int TicksSinceContested;    // timer counter; 0 when not Contested
    public float BondStrength;         // 0..1, used in the interception ritual
}
```

`BondStrength` grows with bond duration and shared successful battles; it shrinks with long idleness and after every contestation. The formula is in `DualFrontier.Systems/Magic/GolemBondStrength.cs` (TODO Phase 6).

## Mutation rule

**Only `GolemSystem` may write `GolemBondComponent`.** Other systems observe `GolemOwnershipChanged` events but do not mutate the component directly. The declaration:

```csharp
[SystemAccess(
    reads:  new[] { typeof(ManaSnapshotComponent), typeof(PositionComponent) },
    writes: new[] { typeof(GolemBondComponent) },
    bus:    nameof(IGameServices.Magic)
)]
public sealed class GolemSystem : SystemBase { /* ... */ }
```

An attempt by another system to declare `writes: GolemBondComponent` crashes the scheduler at startup — write conflict, see [THREADING](./THREADING.md).

The mage's mana is read inside `GolemSystem` through a previous-tick snapshot — otherwise a cycle with `ManaSystem` arises. Details in [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md).

## See also

- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md) — why `GolemSystem` reads `ManaSnapshot`.
- [EVENT_BUS](./EVENT_BUS.md) — the `[Deferred]` mode for `GolemOwnershipChanged`.
- [THREADING](./THREADING.md) — write conflicts and the access declaration.
- [ROADMAP](./ROADMAP.md) — Phase 6 (Magic) as the implementation slot.
