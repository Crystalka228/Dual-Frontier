# Commands — Render commands

## Purpose
Concrete `IRenderCommand` records. Each file is a single command record
carrying event data from the domain. Per К-extensions cascade #2 Q-G-3
(2026-05-23): commands are pure data records без per-command `Execute()`
method — dispatch handled centrally by the active renderer's pattern-matching
dispatcher (Launcher's `RenderCommandDispatcher`). Lesson #25 refined applied:
empty `Execute()` bodies were lying-test surface, structurally eliminated.

## Dependencies
- `DualFrontier.Contracts.Core` — `EntityId` and base types.
- `DualFrontier.Application.Bridge` — `IRenderCommand` marker interface.

## Contents
- `PawnSpawnedCommand.cs` — pawn appears at (X, Y).
- `PawnMovedCommand.cs` — pawn moves к (X, Y).
- `PawnDiedCommand.cs` — pawn dies.
- `PawnStateCommand.cs` — periodic HUD snapshot (needs / mood / job / skills).
- `ItemSpawnedCommand.cs` — item appears at (X, Y) of given kind.
- `TickAdvancedCommand.cs` — simulation tick counter advanced.

## Rules
- Commands are **immutable** `record` types с simple fields
  (`EntityId`, `int`, `float`, `string`, `IReadOnlyList<...>`). No references
  к `IComponent` or systems.
- No `Execute()` method on command records (К-extensions cascade #2 Q-G-3).
  The active renderer's dispatcher (Launcher's `RenderCommandDispatcher`)
  pattern-matches on concrete command type.

## Usage examples
```csharp
// Domain publishes the command after handling an event.
bridge.Enqueue(new PawnSpawnedCommand(pawnId, x: 42f, y: 17f));

// Renderer drains + dispatches centrally.
bridge.DrainCommands(dispatcher.Dispatch);
```

## TODO
- [x] К-extensions cascade #2 — Launcher's `RenderCommandDispatcher` ships
      infrastructure (defensive throws per Lesson #N12) for all 6 command types
      (δ phase, 2026-05-23).
- [ ] К-extensions cascade #3 — real visual dispatching (SpriteCatalog +
      scene state + Vulkan recording).

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
