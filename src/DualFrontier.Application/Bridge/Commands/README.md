---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-APPLICATION-BRIDGE-COMMANDS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-APPLICATION-BRIDGE-COMMANDS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-APPLICATION-BRIDGE-COMMANDS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-APPLICATION-BRIDGE-COMMANDS
---
# Commands — Render commands

## Purpose
Concrete `IRenderCommand` implementations. Each file is a single command:
event data from the domain plus an `Execute` method that, in Phase 5, applies
the effect to the Godot scene.

## Dependencies
- `DualFrontier.Contracts.Core` — `EntityId` and base types.
- `DualFrontier.Application.Bridge` — `IRenderCommand`.

## Contents
- `PawnDiedCommand.cs` — pawn death (effect, sound, UI update).
- `ProjectileSpawnedCommand.cs` — projectile spawn (trajectory visuals).
- `SpellCastCommand.cs` — spell casting (school-of-magic VFX).
- `UIUpdateCommand.cs` — UI element update (counter / banner).

## Rules
- Commands are **immutable** `record` types with simple fields
  (`EntityId`, `int`, `float`, `string`). No references to `IComponent` or
  systems.
- `Execute` works through `object renderContext`; the concrete cast is done by
  the caller from the active Presentation assembly (Godot → `GameRoot`,
  Native → `NativeRenderer`).

## Usage examples
```csharp
// Domain publishes the command after handling an event.
bridge.Enqueue(new PawnDiedCommand(pawnId, x: 42, y: 17));
```

## TODO
- [ ] Phase 5 — fill `Execute` with real Godot logic through Presentation
      helpers.
- [ ] Phase 5 — add commands for the rest of the domain events.
