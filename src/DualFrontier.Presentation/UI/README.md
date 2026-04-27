# UI — Interface controls

## Purpose
User-interface elements layered on top of the game scene. They react to
`UIUpdateCommand` from `PresentationBridge` and to user input.

## Dependencies
- `DualFrontier.Application` — commands and data.
- `GodotSharp` (Phase 3).

## Contents
- `PawnInspector.cs` — panel with the selected pawn's stats.
- `ManaBar.cs` — mana / ether bar.
- `BuildMenu.cs` — building menu.
- `AlertPanel.cs` — alert banners.

## Rules
- Phase 0: classes are plain `public sealed class` without inheritance.
  Phase 3 will add inheritance from `Godot.Control` / `CanvasLayer`.
- The UI does not mutate the simulation directly: user actions become domain
  commands through `InputRouter`.

## Usage examples
```csharp
// Phase 3+: react to UIUpdateCommand.
public void ApplyUpdate(string payload) { /* update text/value */ }
```

## TODO
- [ ] Phase 3 — inherit from `Godot.Control`.
- [ ] Phase 3 — wire up to UI scenes (`.tscn`).
