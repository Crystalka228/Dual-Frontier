# Scene — Engine-neutral scene format

## Purpose
The `SceneDef` DTO hierarchy and the `ISceneLoader` contract for the `.dfscene`
format — a single scene description for both runtimes (Godot DevKit and Native).
The Godot plugin exports `.tscn` → `.dfscene`; both `ISceneLoader` implementations
read the same file.

## Dependencies
- `DualFrontier.Contracts.Math` — `GridVector` for coordinates.
- `System.Text.Json` — `JsonElement` in component overrides.

## Contents
- `SceneDef.cs` — root record + the `CurrentVersion` constant.
- `TilemapDef.cs` — tilemap: dimensions, layers, `TilemapLayerDef` with a `ushort[]`.
- `EntitySpawnDef.cs` — entity description: prefab + position + overrides.
- `MarkerDef.cs` — a named point on the map without an entity.
- `SceneMetadata.cs` — biome, ether density, author, export time.
- `ISceneLoader.cs` — loader contract.
- `SceneLoadException.cs` — parse/version errors.

## .dfscene format

Human-readable JSON with versioning. Tiles are stored per-layer as
`base64(ushort[width*height])`, row-major. Entities are prefab + optional
component-field overrides as `JsonElement`.

```json
{
  "version": 1,
  "name": "colony_start",
  "tilemap": { "width": 100, "height": 100, "tileSize": 32, "layers": [...] },
  "entities": [
    { "id": "pawn_01", "prefab": "core:pawn",
      "position": { "x": 10, "y": 5 },
      "components": { "HealthComponent": { "max": 150 } } }
  ],
  "markers": [...],
  "metadata": { "biome": "temperate", "etherDensity": 0.4, ... }
}
```

Versioning policy: any breaking change increments
`SceneDef.CurrentVersion`; old files require migration in
`ISceneLoader` implementations.

## Rules
- `SceneDef` contains no Godot types (no `Vector2`, `NodePath`, `Node`).
- Only primitives, strings, `GridVector`, and `JsonElement` for opaque overrides.
- `ushort[]` is the compact tile array; in JSON it serializes as base64.

## See also
- [../.docs/architecture/VISUAL_ENGINE.md](/docs/architecture/VISUAL_ENGINE.md) — full
  DevKit vs Native architecture.
- [../Rendering/README.md](../Rendering/README.md) — the `IRenderer` contract.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-APPLICATION-SCENE
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-APPLICATION-SCENE
---
