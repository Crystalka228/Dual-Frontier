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
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-EVENTS-WORLD
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-EVENTS-WORLD
---
