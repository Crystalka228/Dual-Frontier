---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-PRESENTATION-INPUT
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-PRESENTATION-INPUT
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-PRESENTATION-INPUT
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-PRESENTATION-INPUT
---
# Input — Input routing

## Purpose
Single point for handling user input. `InputRouter` catches Godot events
(clicks, keys, drags) and translates them into domain commands through the
`EventBus` / `PresentationBridge`.

## Dependencies
- `DualFrontier.Application` — event bus / bridge.
- `GodotSharp` (Phase 3).

## Contents
- `InputRouter.cs` — input dispatcher.

## Rules
- Presentation NEVER mutates world state directly. Any user action becomes
  an event/command that flows into the domain.
- Input is processed only on Godot's main thread.

## Usage examples
```csharp
// Phase 3+:
public override void _UnhandledInput(InputEvent @event)
{
    _router.Route(@event);
}
```

## TODO
- [ ] Phase 3 — implement `InputRouter.Route` for mouse/keyboard.
- [ ] Phase 3 — configurable bindings.
