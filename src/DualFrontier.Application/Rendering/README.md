# Rendering — Render-backend contract

## Purpose
`IRenderer` is the single entry point for any visual backend. Application holds
only the contract; the concrete implementations live in Presentation assemblies:
Godot DevKit and Native (Silk.NET). At runtime exactly one is active.

## Dependencies
- `DualFrontier.Contracts` — base types (through `PresentationBridge`).

## Contents
- `IRenderer.cs` — `Initialize` / `RenderFrame` / `Shutdown` / `IsRunning`.

## Rules
- No `using Godot;` or `using Silk.NET;` — only the abstract contract.
- The implementation MUST drain `PresentationBridge` inside its `RenderFrame`.
- Every GPU resource is released in `Shutdown` — leaks are unacceptable.

## TODO
- [ ] Phase 3.5 — `GodotRenderer` in `DualFrontier.Presentation`.
- [ ] Phase 5+ — `NativeRenderer` in `DualFrontier.Presentation.Native`
      (Silk.NET + OpenGL, SpriteBatch, TilemapRenderer).

## See also
- [../.docs/architecture/VISUAL_ENGINE.md](/docs/architecture/VISUAL_ENGINE.md) — overall
  DevKit vs Native strategy, the `.dfscene` format, and the developer pipeline.
