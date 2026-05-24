# DualFrontier.Launcher — Production Launch Entry Point

## Purpose
Production launcher для Dual Frontier. Composes:
- **Vulkan substrate** (`DualFrontier.Runtime`) — window, Vulkan device, sprite
  pipeline, compute primitives.
- **Domain layer** (`DualFrontier.Application`) — GameLoop, ModMenuController,
  PresentationBridge, GameBootstrap.
- **Bridge** (`LauncherRenderer` + `RenderCommandDispatcher`) — drains
  PresentationBridge commands per frame, dispatches к Vulkan primitives.

Implements `IRenderer` contract from `DualFrontier.Application.Rendering` via
`LauncherRenderer` class.

## Dependencies
- `DualFrontier.Application` — contracts + GameBootstrap + bridge + commands.
- `DualFrontier.Runtime` — Vulkan substrate primitives.

Inherits `Directory.Build.props` defaults: net8.0 + Nullable enabled +
LangVersion 12.0 + ImplicitUsings + TreatWarningsAsErrors +
GenerateDocumentationFile.

## Contents
- `Program.cs` — `Main()` entry point, composition root, (d) hybrid main loop
  per Q-G-7 LOCKED orchestration.
- `LauncherRenderer.cs` — `IRenderer` implementation wrapping Runtime + bridge.
- `RenderCommandDispatcher.cs` — pattern-matching dispatcher для IRenderCommand
  instances; defensive throws per Lesson #N12 first application.

## Cascade scope (К-extensions cascade #2)
К-extensions cascade #2 (2026-05-23) ships **infrastructure-only** per Q-G-6
(b1) functional bar:
- Window opens, Vulkan initializes, GameLoop ticks (on its own background
  thread via `Loop.Start()`).
- PresentationBridge connects к dispatcher.
- Dispatcher receives commands (defensive throws fire if visual paths invoked).

**Defensive Reserved Stub Pattern** (Lesson #N12 first application): all 6
dispatch handler methods (`HandlePawnSpawned`, `HandlePawnMoved`,
`HandlePawnDied`, `HandlePawnState`, `HandleItemSpawned`, `HandleTickAdvanced`)
throw `NotImplementedException` с descriptive message. This is **intentional**
— prevents lying tests per Lesson #25 refined.

## Architecture notes
**Brief amendment** (Crystalka Option A ratification mid-cascade 2026-05-23):
brief assumed `gameContext.GameLoop.Tick()` callable externally — empirically
`GameLoop` runs on its own background thread via `Loop.Start()`/`Loop.Stop()`
(accumulator-based fixed step at 30 TPS). Main loop in `Program.cs` drives
window message pump + input drain + per-frame render only; simulation ticks
autonomously on background thread. Cross-thread communication через
`PresentationBridge` command queue (commands enqueued от sim thread,
drained от main thread per frame).

Q-G-7 (d) hybrid orchestration intent preserved — `Program.cs` still explicitly
drives lifecycle (Start/Stop, message pump, render), just не sim tick (which
is background thread concern).

`GameContext` + `GameLoop` + `GameBootstrap` are `internal` types в
Application; Launcher accesses them via `InternalsVisibleTo("DualFrontier.Launcher")`
declared в `DualFrontier.Application.csproj`.

## Forward roadmap
**К-extensions cascade #3** (next session, separate brief):
- Replace defensive throws с real visual implementations.
- Add SpriteCatalog (`PawnId → Sprite` mapping).
- Add scene state management.
- Wire К-L17 CompositionFramework integration в `LauncherRenderer`
  (deferred к cascade #3 per Crystalka ratification mid-cascade-#2).
- (b2) functional bar — pawns appear as sprites, move, despawn on death.

## Rules
- No `using Godot;` (Godot path retired per К-extensions cascade #2).
- No `using Silk.NET;` (Silk.NET path retired — superseded by Vulkan substrate).
- Implements `DualFrontier.Application.Rendering.IRenderer` contract.
- Domain knowledge limited к `PresentationBridge` + `IRenderCommand` types.

---
<!-- Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY -->
<!-- Manual edits overwritten by sync_register.ps1 on next sync. -->
<!-- register_id: DOC-F-SRC-LAUNCHER -->
<!-- category: F | tier: 4 | lifecycle: Live | owner: Crystalka -->

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-LAUNCHER
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: TBD — after Vanilla mods consumer materialization OR substrate palette decoder extension cascade
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-LAUNCHER
---
