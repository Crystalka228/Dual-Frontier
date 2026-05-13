---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-EVENTS-MAGIC
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-EVENTS-MAGIC
---
# Magic Events

## Purpose
Events of the magic subsystem: mana requests (the two-step model), spells,
ether breaks, golem activation, ether-level-up.

## Dependencies
- `DualFrontier.Contracts` — `IEvent`, `EntityId`, `[Deferred]`.

## Contents
- `ManaIntent.cs` — step 1: intent to spend mana.
- `ManaGranted.cs` — step 2: mana is debited; casting may proceed.
- `ManaRefused.cs` — step 2: insufficient mana; the cast is canceled.
- `SpellCastEvent.cs` — a spell was successfully cast.
- `EtherSurgeEvent.cs` — an "ether break" while working with a crystal / on overload (GDD 4.2).
- `GolemActivatedEvent.cs` — a mage activated their golem.
- `EtherLevelUpEvent.cs` — `[Deferred]`: ether perception level increased.

## Rules
- Spell cost and golem upkeep always go through `ManaIntent` — never debit
  mana directly from the component.
- `EtherLevelUpEvent` is `[Deferred]`: a level-up affects max mana and other
  derived values, so deferring to the next phase avoids races.
- `EtherSurgeEvent` may publish StatusApplied (burning, stun, etc.) — see GDD 4.2.

## Usage examples
```csharp
_bus.Publish(new ManaIntent { /* CasterId = mage, Amount = spell.Cost */ });
// → ManaSystem publishes ManaGranted → SpellCastSystem publishes SpellCastEvent
//   (or ManaRefused — the cast is canceled, the AI chooses another action)
```

## TODO
- [ ] Fill the fields once `MagicSchool`, `SpellId`, `GolemId` exist.
- [ ] Add `SpellInterruptedEvent` — if a cast is interrupted (damage, dispel, etc.).

## v02 Addendum additions
Magic-subsystem extension: continuous mana lease and golem ownership transfer.

- `LeaseId.cs` — mana-lease identifier (`readonly record struct`); `New()` factory — TODO Phase 5.
- `RefusalReason.cs` — refusal reasons for opening a lease (`InsufficientMana`, `LeaseCapExceeded`, `NoActiveBond`, `SchoolMismatch`).
- `CloseReason.cs` — lease-closure reasons (`Completed`, `SpellInterrupted`, `GolemDeactivated`, `PawnDied`, `ManaExhausted`).
- `ManaLeaseOpenRequest.cs` — `ICommand`: open a mana lease on the caster with the specified drain and duration window.
- `ManaLeaseOpened.cs` — `IEvent`: confirmation that the lease is open and the first tick has been debited.
- `ManaLeaseRefused.cs` — `IEvent`: lease opening refused with a reason and available mana.
- `ManaLeaseClosed.cs` — `[Deferred]` `IEvent`: terminal lease-lifecycle event.
- `GolemOwnershipTransferRequest.cs` — `ICommand`: transfer / contest / abandon golem ownership.
- `GolemOwnershipChanged.cs` — `[Deferred]` `IEvent`: golem ownership changed.

Note: `OwnershipMode` lives in `DualFrontier.Contracts.Enums`, since it is used both by components (`GolemBondComponent`) and by magic events.
