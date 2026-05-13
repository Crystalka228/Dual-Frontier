---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-APPLICATION-INPUT
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-APPLICATION-INPUT
---
# Input — User input source

## Purpose
`IInputSource` is the contract for polling input and publishing normalized
domain events. Application does not know where keystrokes physically come
from; each Presentation assembly implements its own polling.

## Dependencies
- `DualFrontier.Contracts` — `IGameServices` for publishing events to the buses.

## Contents
- `IInputSource.cs` — single `Poll()` method.

## Rules
- Neither `Godot.InputEvent` nor `Silk.NET.Input.*` leaks into Application.
- `Poll` does not block — it reads and dispatches to the buses, does not wait
  for a response.
- The "physical input → domain event" mapping lives in the concrete
  implementation.

## TODO
- [ ] Phase 3.5 — `GodotInputRouter` in `DualFrontier.Presentation`.
- [ ] Phase 5+ — `NativeInputHandler` in `DualFrontier.Presentation.Native`
      (Silk.NET input polling).

## See also
- [../.docs/architecture/VISUAL_ENGINE.md](/docs/architecture/VISUAL_ENGINE.md) — overall
  DevKit vs Native strategy and the three contracts (`IRenderer`,
  `ISceneLoader`, `IInputSource`).
