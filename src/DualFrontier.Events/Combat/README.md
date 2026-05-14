# Combat Events

## Purpose
Events of the combat subsystem: shot attempts, ammunition requests via the
two-step model, damage events, deaths, and status-effect application.

## Dependencies
- `DualFrontier.Contracts` — `IEvent`, `EntityId`, `[Deferred]`.

## Contents
- `ShootAttemptEvent.cs` — a pawn attempts a shot.
- `AmmoIntent.cs` — **step 1** of the two-step model: intent to acquire ammunition (TechArch 11.5).
- `AmmoGranted.cs` — **step 2**: ammunition granted, the shot can proceed.
- `AmmoRefused.cs` — **step 2**: no ammunition, the shot is canceled.
- `DamageEvent.cs` — damage application computed by DamageSystem.
- `DeathEvent.cs` — `[Deferred]`: the entity died; processing happens in the next phase.
- `StatusAppliedEvent.cs` — a status effect was applied to the entity.

## Rules
- `AmmoIntent` does NOT block — the response arrives as a separate event.
- `DeathEvent` is marked `[Deferred]`: entity destruction must not preempt the
  current phase (entity references are still in use by other systems).
- `ShootAttemptEvent` is published by AI/the player; CombatSystem decides
  whether the shot is possible.

## Usage examples
```csharp
_bus.Publish(new ShootAttemptEvent { /* AttackerId = pawn, TargetId = enemy */ });
// → CombatSystem publishes AmmoIntent
// → InventorySystem responds with AmmoGranted or AmmoRefused
// → CombatSystem, on AmmoGranted, publishes DamageEvent
// → DamageSystem publishes DeathEvent ([Deferred]) when HP ≤ 0
```

## TODO
- [ ] Fill the fields once `AmmoType`, `DamageType`, `GridVector`, `StatusKind` exist.
- [ ] Add `MissEvent` / `CritEvent` — optional, Phase 6.

## v02 Addendum additions
Combat-subsystem extension: two-phase commit for the "compound shot" (ammo + mana) and an explicit damage command.

- `TransactionId.cs` — composite shot transaction identifier (`readonly record struct`); `New()` factory — TODO Phase 4.
- `ShotRefusalReason.cs` — refusal reasons (`NoAmmo`, `NoMana`, `WeaponOnCooldown`, `OutOfRange`, `TargetInvalid`).
- `CompoundShotIntent.cs` — `IQuery`: intent to execute a compound shot (polls Inventory + Magic).
- `ShootGranted.cs` — `IEvent`: both buses confirmed; the shot is permitted.
- `ShootRefused.cs` — `IEvent`: at least one bus refused; the shot is canceled.
- `DamageIntent.cs` — `ICommand`: damage-application request for `DamageSystem` (before publishing `DamageEvent`).

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-EVENTS-COMBAT
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-EVENTS-COMBAT
---
