# Bridge — Domain → Presentation bridge

## Purpose
A unidirectional command queue between the domain (multithreaded) and the
rendering layer (main thread only). Domain code, on any thread, places
`IRenderCommand` into `PresentationBridge`; Launcher's main loop iteration
reads the queue and dispatches commands via `RenderCommandDispatcher` (per
К-extensions cascade #2 Q-G-7 hybrid orchestration). See TechArch 11.9.

## Dependencies
- `DualFrontier.Contracts` — `EntityId` and base event interfaces.

## Contents
- `PresentationBridge.cs` — `ConcurrentQueue<IRenderCommand>` + push/drain API.
- `IRenderCommand.cs` — marker interface for every render command (К-extensions
  cascade #2 Q-G-3: stripped к pure marker; dispatch handled by renderer's
  dispatcher, not per-command `Execute()` — Lesson #25 refined).
- `Commands/` — concrete command records (pawn spawn/move/death/state, item
  spawn, tick advanced).

## Rules
- No `using Godot;` in this folder (Godot path retired per К-extensions cascade
  #2). Commands are pure data records — the active renderer's dispatcher
  (Launcher's `RenderCommandDispatcher`) pattern-matches on concrete type.
  Future DevKit renderer (dormant `IDevKitRenderer` consumer) will implement
  similar pattern над Vulkan substrate.
- The domain thread **only writes**; the main thread **only reads and dispatches**.
- Commands MUST NOT hold references to ECS components — only simple values
  (`EntityId`, coordinates, identifiers).

## Usage examples
```csharp
// Domain side (any thread):
bridge.Enqueue(new PawnDiedCommand(entityId));

// Presentation side (main thread of the active IRenderer):
bridge.DrainCommands(dispatcher.Dispatch);
```

## TODO
- [x] Phase 3 — `DrainCommands` wired into Launcher's per-frame iteration
      (К-extensions cascade #2 δ phase, 2026-05-23).
- [ ] К-extensions cascade #3 — replace Launcher's defensive throws (Lesson #N12)
      с real visual dispatching (SpriteCatalog + scene state + Vulkan recording).

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-APPLICATION-BRIDGE
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-APPLICATION-BRIDGE
---
