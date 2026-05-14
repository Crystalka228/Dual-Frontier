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

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-PRESENTATION-SCENES
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-PRESENTATION-SCENES
---
