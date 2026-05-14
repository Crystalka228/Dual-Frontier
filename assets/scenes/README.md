# assets/scenes — Scene fixtures and test data

Scenes in the `.dfscene` format are engine-neutral JSON, readable both by the
Godot build and by the Native build through `ISceneLoader`.

## Files
- `sample.dfscene` — the minimal valid scene used by unit tests and smoke checks.

## Creating a scene
Preferred path: the Godot DevKit plugin (Tools → Export .dfscene).
Hand-authoring is possible for fixtures but brittle — base64 tile data is
easy to get wrong and hard to debug.

## Format version
Current version: `1` (see `SceneDef.CurrentVersion`).

When raising the version:
1. Update `SceneDef.CurrentVersion`.
2. Update every existing fixture, OR add a migration in `ISceneLoader`
   implementations.
3. Document the breaking change in `docs/architecture/VISUAL_ENGINE.md`.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-ASSETS-SCENES
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-ASSETS-SCENES
---
