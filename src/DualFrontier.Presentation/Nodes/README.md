# Nodes — Godot game-scene nodes

## Purpose
Visual scene nodes: the root (`GameRoot`) and its children
(`PawnVisual`, `TileMapRenderer`, `ProjectileVisual`). They read commands
from `PresentationBridge` and apply them to the scene.

## Dependencies
- `DualFrontier.Application` — `PresentationBridge`, render commands.
- `GodotSharp` (Phase 3).

## Contents
- `GameRoot.cs` — root scene node (future `Node2D`). Ticks `PresentationBridge`.
- `PawnVisual.cs` — visual for a single pawn (future `Node2D`).
- `TileMapRenderer.cs` — tile map (future `TileMap`).
- `ProjectileVisual.cs` — visual for a flying projectile (future `Node2D`).

## Rules
- Phase 0: classes are declared as `public sealed class X` **without**
  inheriting from `Godot.Node*` and without `using Godot;`. Phase 3 will
  change the inheritance.
- Nodes do not reach into Domain/Application for data — they only read commands.

## Usage examples
```csharp
// Phase 3+ (after wiring up GodotSharp):
public override void _Process(double delta)
{
    _bridge.DrainCommands(cmd => cmd.Execute(this));
}
```

## TODO
- [ ] Phase 3 — inherit from the corresponding Godot types.
- [ ] Phase 3 — wire up to `.tscn` scenes from `Scenes/`.
