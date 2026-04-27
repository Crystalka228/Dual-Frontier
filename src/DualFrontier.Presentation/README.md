# DualFrontier.Presentation

## Purpose
The Godot layer: nodes, UI controls, input. The only assembly allowed to use
`using Godot;`. Runs ONLY in the Godot main thread (the `SceneTree` / `Node`
API constraint), reads commands from `PresentationBridge`, and applies them
to the scene. See TechArch 11.9.

> **TODO — the GodotSharp NuGet package will be wired in during Phase 3**, when
> the actual Godot project is integrated. For now every class is a **stub
> without `using Godot;`**: classes do not inherit from `Node` / `Control` and
> carry a header comment "after wiring up GodotSharp, inherit from
> Godot.Node2D / Control". Phase 0 only creates the structure and the contract.

## Tier

DevKit tier. Implements both `IRenderer` (production minimum) and
`IDevKitRenderer` (debug surface: gizmos, profiler, entity highlighting).
Everything debug-specific is marked `[DevKitOnly]` — production analytics
(Phase 5+ Roslyn analyzer) guarantees that such code does not leak into Native.

## Dependencies
- `DualFrontier.Application` — `PresentationBridge`, render commands.
- `GodotSharp` (will be added in Phase 3).

## Contents
- `Nodes/` — root scene nodes (`GameRoot`, `PawnVisual`, `TileMapRenderer`, `ProjectileVisual`).
- `UI/` — interface controls (`PawnInspector`, `ManaBar`, `BuildMenu`, `AlertPanel`).
- `Input/` — input routing (`InputRouter`).
- `Scenes/` — `.tscn` files, added through the Godot editor later.
- `project.godot` — placeholder; will be overwritten by Godot when the project is opened.

## Rules
- Any `using Godot;` MUST live ONLY in this assembly.
- Presentation NEVER calls `DualFrontier.Core` or `Systems` directly —
  communication only through `PresentationBridge`.
- All Presentation code runs on the main thread. No tasks, no `async void` in
  nodes.

## Usage examples
```csharp
// Inside the Godot layer (Phase 3+):
public override void _Process(double delta)
{
    _bridge.DrainCommands(cmd => cmd.Execute(this));
}
```

## TODO
- [ ] Phase 3 — add `PackageReference Include="GodotSharp"` in the `.csproj`.
- [ ] Phase 3 — make nodes inherit from the corresponding base Godot classes.
- [ ] Phase 3 — create the real Godot project and replace `project.godot`.
- [ ] Phase 3.5 — implement GodotRenderer through IDevKitRenderer (inherits IRenderer + debug surface).
