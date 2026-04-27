# DualFrontier.Presentation.Native — Production Runtime Stub

## Purpose
A production runtime on Silk.NET + OpenGL. Implements the `IRenderer`,
`ISceneLoader`, and `IInputSource` contracts from Application without a single
reference to Godot. This is the assembly that ships to players.

For now — only stubs with the right class structure and XML documentation.
The real implementation begins in Phase 5.

## Dependencies
- `DualFrontier.Contracts`
- `DualFrontier.Application`

Neither `Core`, nor `Systems`, nor `Godot`.

## Contents
- `NativeRenderer.cs` — stub `IRenderer` implementation.
- `NativeSceneLoader.cs` — stub `ISceneLoader` (System.IO + System.Text.Json).
- `NativeInputHandler.cs` — stub `IInputSource`.

## Rules
- Native **never** references `IDevKitRenderer` or any other type marked
  `[DevKitOnly]`. The CI check (`grep` today, a Roslyn analyzer later) fails
  if such a reference appears.
- Not a single line of `using Godot;` in this assembly — CI grep enforces it.
- No `DualFrontier.Presentation` reference — that is a peer assembly, not a parent.
- Types are marked `[DevKitOnly]` only when they will **NOT** survive the
  transition to production. Nothing here should be marked.

## TODO
- [ ] Phase 5 — add a `PackageReference` to Silk.NET.
- [ ] Phase 5 — implement `Initialize` / `RenderFrame` / `Shutdown` in NativeRenderer.
- [ ] Phase 5 — implement JSON parsing in NativeSceneLoader.
- [ ] Phase 5 — implement Silk.NET input polling in NativeInputHandler.
- [ ] Phase 6 — add SpriteBatch + TilemapRenderer + ShaderProgram.
- [ ] Phase 6 — add ImGui.NET for the HUD.
