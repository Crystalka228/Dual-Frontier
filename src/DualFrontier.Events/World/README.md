# World Events

## Purpose
World-level events: ether-node changes, weather changes, incoming raids.
Broadcast events with many subscribers (AI, MoodSystem, UI, audio).

## Dependencies
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Contents
- `EtherNodeChangedEvent.cs` — an ether node's parameters changed (level/radius).
- `WeatherChangedEvent.cs` — the weather changed.
- `RaidIncomingEvent.cs` — a raid is approaching the colony.

## Rules
- `EtherNodeChangedEvent` is published by EtherFieldSystem after a field
  recompute; ManaSystem uses it to recompute mana regeneration.
- `RaidIncomingEvent` arrives with lead time (the preparation phase); the
  actual engagement is then driven by `ShootAttemptEvent` and the like.

## Usage examples
```csharp
// MoodSystem applies a penalty on bad weather:
_bus.Subscribe<WeatherChangedEvent>(evt =>
{
    // if (evt.Kind == WeatherKind.Storm) { ... }
});
```

## TODO
- [ ] Define the `WeatherKind` enum (Clear, Rain, Storm, EtherStorm …) — Phase 4.
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
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: Events World submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
