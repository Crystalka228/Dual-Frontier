# Bridge — Domain → Presentation bridge

## Purpose
A unidirectional command queue between the domain (multithreaded) and the
Godot layer (main thread only). Domain code, on any thread, places
`IRenderCommand` into `PresentationBridge`; Godot in its `_Process` reads the
queue and applies commands to the scene. See TechArch 11.9.

## Dependencies
- `DualFrontier.Contracts` — `EntityId` and base event interfaces.

## Contents
- `PresentationBridge.cs` — `ConcurrentQueue<IRenderCommand>` + push/drain API.
- `IRenderCommand.cs` — base interface for every render command.
- `Commands/` — concrete commands (pawn death, projectile, spell, UI).

## Rules
- No `using Godot;` in this folder. There are no references to Godot — commands
  accept `object renderContext`, the root object of the active `IRenderer`.
  In the Godot assembly this is `GameRoot`; in the Native assembly it is
  `NativeRenderer`.
- The domain thread **only writes**; the main thread **only reads and executes**.
- Commands MUST NOT hold references to ECS components — only simple values
  (`EntityId`, coordinates, identifiers).

## Usage examples
```csharp
// Domain side (any thread):
bridge.Enqueue(new PawnDiedCommand(entityId, x, y));

// Presentation side (main thread of the active IRenderer):
bridge.DrainCommands(cmd => cmd.Execute(renderContext));
```

## TODO
- [ ] Phase 3 — wire `DrainCommands` into `GameRoot._Process` in Presentation.
- [ ] Phase 5 — fill `Commands/` with real effects.

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
