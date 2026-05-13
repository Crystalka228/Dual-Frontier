---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-EVENTS-PAWN
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-EVENTS-PAWN
---
# Pawn Events

## Purpose
Events relating to pawns as personalities: psychological breaks, reactions to
the death of others, skill gains.

## Dependencies
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Contents
- `MoodBreakEvent.cs` — a pawn's mood dropped below threshold
  (`PawnId`, `MoodValue`).
- `DeathReactionEvent.cs` — a pawn witnessed the death of another.
- `SkillGainEvent.cs` — a skill leveled up (`PawnId`, `Skill`, `NewLevel`,
  `Delta`).
- `PawnSpawnedEvent.cs` — a pawn appeared in the world (`PawnId`, `X`, `Y`).
- `PawnMovedEvent.cs` — a pawn changed tile (`PawnId`, `X`, `Y`).
- `JobAssignedEvent.cs`, `JobCompletedEvent.cs` — job lifecycle.
- `NeedsCriticalEvent.cs` — one of a pawn's needs is in the critical zone.

## Rules
- `MoodBreakEvent` is published by MoodSystem once per transition — a repeat
  break without recovery is not counted as a new event.
- `DeathReactionEvent` subscribes to the deferred `DeathEvent` from Combat:
  the reaction happens in the next phase, by which time the body is already
  removed from the scene.
- `SkillGainEvent` is for UI/statistics; the level itself is stored in
  `SkillsComponent` — the event carries the delta.

## Usage examples
```csharp
// MoodSystem (SLOW tick):
if (mind.Mood < mind.MoodBreakThreshold && !already)
    _bus.Publish(new MoodBreakEvent { /* PawnId = pawn, Severity = ... */ });
```

## TODO
- [ ] Settle the break severity scale (minor / major / berserk) — psychology GDD.
