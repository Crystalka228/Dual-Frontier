# assets/scenes — Scene fixtures and test data

Scenes in the `.dfscene` format are engine-neutral JSON — the project's own
`SceneDef` format, read through the `ISceneLoader` seam (no engine dependency).

## Files
- `sample.dfscene` — the minimal valid scene used by unit tests and smoke checks.

## Creating a scene
No export tool ships today (a tooling-assisted exporter is `Planned — see
docs/ROADMAP.md`); fixtures are hand-authored for now — brittle, since base64
tile data is easy to get wrong and hard to debug.

## Format version
Current version: `1` (see `SceneDef.CurrentVersion`).

When raising the version:
1. Update `SceneDef.CurrentVersion`.
2. Update every existing fixture, OR add a migration in `ISceneLoader`
   implementations.
3. Document the breaking change in the live rendering spec (`docs/architecture/VULKAN_SUBSTRATE.md`).

---
register_id: DOC-F-ASSETS-SCENES
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-06-29
last_modified: 2026-06-29
content_language: en
next_review_due: null
title: Assets scenes
last_modified_commit: 192a4e3
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
