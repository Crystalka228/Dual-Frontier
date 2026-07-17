# DualFrontier.Events

## Purpose
The assembly of all domain events, intents, and queries. Each event is an
immutable `record` that implements `IEvent` from `DualFrontier.Contracts.Core`.
They are split by domain (Combat / Magic / Inventory / Power / Pawn / World),
one folder per domain — one for each domain `IEventBus`.

The assembly implements two key architectural patterns (TechArch 11):
- **Intent vs Event**: two-step mechanics (AmmoIntent → AmmoGranted/Refused,
  ManaIntent → ManaGranted/Refused) instead of a blocking request/response.
- **Deferred**: events deferred to the next scheduler phase
  (`DeathEvent`, `EtherLevelUpEvent`) — marked with the `[Deferred]` attribute.

## Dependencies
- `DualFrontier.Contracts` — `IEvent`, `EntityId`, `[Deferred]`, `[Immediate]`.

Events do NOT depend on `Components` (shared types — `EntityId`, enums — must
live either in `Contracts` or in `Components`; field types in this scaffold are
left as TODO).

## Contents
- `Combat/` — ShootAttempt, Ammo Intent/Granted/Refused, Damage, Death, StatusApplied.
- `Magic/` — Mana Intent/Granted/Refused, SpellCast, EtherSurge, GolemActivated, EtherLevelUp.
- `Inventory/` — ItemAdded, ItemRemoved, ItemReserved, CraftRequest.
- `Power/` — PowerRequest, PowerGranted, GridOverload.
- `Pawn/` — MoodBreak, DeathReaction, SkillGain.
- `World/` — EtherNodeChanged, WeatherChanged, RaidIncoming.

## Rules
- `public sealed record XxxEvent : IEvent` only — no classes.
- All fields use `init` or `required init` — events are immutable after creation.
- `[Deferred]` for events that cannot be reacted to instantly (e.g., entity
  destruction — `DeathEvent`).
- `[Immediate]` only for critical phase preemptions (extremely rare).

## Usage examples
```csharp
// Step 1 of the two-step model — CombatSystem publishes the intent.
_bus.Publish(new AmmoIntent { /* RequesterId = pawn, AmmoType = ..., Position = ... */ });

// InventorySystem collects a batch of AmmoIntents and in the next phase publishes
// AmmoGranted / AmmoRefused per request.
```

## TODO
- [ ] Fill in event fields once the base types appear
      (`GridVector`, `AmmoType`, `DamageType`, `MagicSchool`, `PowerType`).
- [ ] Verify `[Deferred]` markup once EventBus implements deferred-delivery
      handling (Phase 1).
- [ ] Write a generator for "who publishes / who subscribes" diagrams from
      attributes and event names (Phase 3, tooling).

---
register_id: DOC-F-SRC-EVENTS
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: DualFrontier.Events module
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
