# Scenes — Godot scenes

## Purpose
Godot scene `.tscn` files (main menu, game scene, UI overlays).
The folder is currently empty: scenes will be added through the Godot editor
in Phase 3.

## Dependencies
- Godot 4.3+ (the editor).

## Contents
- For now only `.gitkeep`, so the folder exists in git.
- In Phase 3 `main.tscn`, `ui_root.tscn`, etc. will appear.

## Rules
- `.tscn` files are stored as plain text — reviewable in diffs.
- Scenes MUST NOT contain absolute paths to resources outside `res://`.

## Usage examples
Editing through the Godot editor; programmatic access via
`GD.Load<PackedScene>("res://Scenes/main.tscn")` in Phase 3.

## TODO
- [ ] Phase 3 — create the base scenes (`main.tscn`, `ui_root.tscn`).
