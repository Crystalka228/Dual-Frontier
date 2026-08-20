# World Events

## Purpose
World-level events: ether-node changes and incoming raids. Broadcast events
with many subscribers (AI, MoodSystem, UI, audio).

Weather events are NOT here. `WeatherChangedEvent` is MOD-OWNED as of W3 and is
vended by the shared mod `mods/DualFrontier.Mod.Weather.Contracts`; the empty
`src/` stub was deleted. A cross-mod event type must live in a shared mod so
every consumer resolves the same `Type` (`ContractValidator` Phase E).

## Dependencies
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Contents
- `EtherNodeChangedEvent.cs` — an ether node's parameters changed (level/radius).
- `RaidIncomingEvent.cs` — a raid is approaching the colony.

## Rules
- `EtherNodeChangedEvent` is published by EtherFieldSystem after a field
  recompute; ManaSystem uses it to recompute mana regeneration.
- `RaidIncomingEvent` arrives with lead time (the preparation phase); the
  actual engagement is then driven by `ShootAttemptEvent` and the like.

## TODO
- [ ] Add `SeasonChangedEvent` — if seasons exist, Phase 6.

---
register_id: DOC-F-SRC-EVENTS-WORLD
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: '2026-08-20'
content_language: en
next_review_due: null
title: Events World submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
