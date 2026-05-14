# DF DevKit — Godot editor plugin

## Purpose
Editor plugin for authoring game scenes and exporting to the `.dfscene`
format. The plugin runs ONLY inside the Godot Editor (the `#if TOOLS` flag)
and never lands in a production build.

## Architecture
- `DfDevKitPlugin.cs` — EditorPlugin entry point, "Tools → Export .dfscene" menu.
- `meta/DFEntityMeta.cs` — `[GlobalClass] Resource` with `Prefab` and
  `ComponentOverrides` fields, attached to nodes in the inspector.
- `export/SceneExporter.cs` — SceneTree walk → `SceneDef` → JSON.

## Developer pipeline
1. Open a scene in the Godot Editor.
2. Place a Node2D, attach a DFEntityMeta to it (Prefab = "core:pawn").
3. In the inspector, set component overrides (HealthComponent.max = 150).
4. Tools → Export .dfscene → choose path → the plugin writes the file.
5. Run the game via `dotnet run` (or F5 in Godot) — the loader picks it up.

## Status
- [ ] Phase 3.5 — `DFEntityMeta`: registration in the editor, inspector UI.
- [ ] Phase 3.5 — `SceneExporter`: tree walk, TilemapExporter, EntityExporter.
- [ ] Phase 3.5 — "Export .dfscene" menu item under Tools.
- [ ] Phase 4 — selective export (only the selected nodes).
- [ ] Phase 4 — live preview: Godot displays ECS state in a separate panel.

## Rules
- Plugin code is always under `#if TOOLS`. Never compiled into production.
- No game logic here. Only authoring and export.
- The JSON output is validated against the `SceneDef` DTO in
  `DualFrontier.Application.Scene`.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-PRESENTATION-ADDONS-DEVKIT
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-PRESENTATION-ADDONS-DEVKIT
---
