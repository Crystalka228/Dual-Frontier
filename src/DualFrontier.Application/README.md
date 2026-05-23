# DualFrontier.Application

## Purpose
The glue layer between the domain (`Core`, `Systems`, `Components`, `Events`, `AI`)
and presentation (`DualFrontier.Launcher` via `IRenderer` contract). Home of the
main loop (`GameLoop`), saving (`SaveSystem`), scenario loading (`ScenarioLoader`),
the mod loader (`ModLoader`), and the bridge into the rendering layer
(`PresentationBridge`). Application is the only assembly that knows both the
domain and the existence of an "upstream" rendering layer. Godot DevKit + Silk.NET
Native paths retired per К-extensions cascade #2 (2026-05-23).

## Dependencies
- `DualFrontier.Contracts` — interfaces (`IMod`, `IModApi`, `IEvent`, `EntityId`, ...)
- `DualFrontier.Core` — `World`, `SystemBase`, `DomainEventBus`, `GameServices`
- `DualFrontier.Components` — standard components
- `DualFrontier.Events` — domain events
- `DualFrontier.Systems` — game systems
- `DualFrontier.AI` — behaviour trees / AI utilities

## Contents
- `Loop/` — main game loop (`GameLoop`, `FrameClock`).
- `Save/` — world serialization (`ISaveSystem`, `SaveSystem`, `SaveFormat`).
- `Scenario/` — start-scenario loading (`ScenarioLoader`, `ScenarioDef`).
- `Modding/` — mod loader and the isolated `IModApi`
  (`ModLoader`, `ModLoadContext`, `RestrictedModApi`, `ModIsolationException`).
- `Bridge/` — Domain → Presentation bridge through a command queue
  (`PresentationBridge`, `IRenderCommand`, `Commands/`).

## Rules
- Application **may** know about `Core` and `Systems` — gluing them is its job.
- Application **must not** know about specific renderer implementations or call
  presentation backends directly (Godot path retired per К-extensions cascade #2,
  2026-05-23). The link is strictly one-way: Domain/Application →
  `PresentationBridge` (command queue) → renderer reads in the main thread.
- A mod **always** loads through its own `AssemblyLoadContext` (`ModLoadContext`)
  with `isCollectible: true` to enable hot reload (TechArch 11.8).
- `SaveSystem.Save/Load` are synchronous. Async I/O is performed by the upstream
  layer that hands a ready path/stream. This rule comes from THREADING (see
  `docs/architecture/THREADING.md`).

## Usage examples
```csharp
// A normal game start: scenario + loop + bridge.
var services = new GameServices(); // from Core
var bridge   = new PresentationBridge();
var loop     = new GameLoop(services, bridge);

var scenario = new ScenarioLoader().Load("scenarios/default.json");
// TODO: build World from ScenarioDef.

loop.Start();
```

## TODO
- [x] Phase 1 — `GameLoop` with accumulator-based fixed step
      (30 Hz, pause, speed x1/x2/x3).
- [ ] Phase 1 — `SaveSystem.Save/Load` (binary + `SaveFormat` header).
- [ ] Phase 2 — `ModLoader` (`AssemblyLoadContext`, mod registry, hot reload).
- [ ] Phase 2 — `RestrictedModApi` proxies calls into `Core.GameServices`.
- [x] Phase 3 — wire `PresentationBridge.DrainCommands` into Launcher's
      `RenderCommandDispatcher` (К-extensions cascade #2 δ phase, 2026-05-23 —
      infrastructure-only with defensive throws per Lesson #N12; visual
      implementation lands cascade #3).
- [x] Phase 3 — `ScenarioLoader` parses JSON via `System.Text.Json`.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-APPLICATION
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-APPLICATION
---
