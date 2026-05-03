# UI — Interface controls

## Purpose
User-interface widgets that react to render commands from
`PresentationBridge` and to user input.

## Dependencies
- `DualFrontier.Application` — commands and data.
- `GodotSharp`.

## Contents
- `GameHUD.cs` — top-level HUD overlay (CanvasLayer 10).
- `ColonyPanel.cs` — left-edge colony roster + tick label.
- `PawnDetail.cs` — right-edge per-pawn detail (header, mood, needs, job, top skills).
- `Palette.cs` — shared color tokens.

Phase 5 stubs (BuildMenu, AlertPanel, ManaBar, PawnInspector) were deleted as
unused scaffolding; they will be re-created with real implementations when the
corresponding gameplay systems land (build mode, alert/notification system,
magic, expanded pawn inspector).

## Rules
- The UI does not mutate the simulation directly: user actions become domain
  commands through `InputRouter`.
