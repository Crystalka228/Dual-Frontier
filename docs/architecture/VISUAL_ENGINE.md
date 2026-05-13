---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-VISUAL_ENGINE
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-VISUAL_ENGINE
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-VISUAL_ENGINE
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-VISUAL_ENGINE
---
# Visual engine — DevKit and Native

Dual Frontier uses two parallel visual backends. Godot is the **DevKit**:
scene editor, visual test bench, rapid-iteration environment. Native is **Production**:
a custom runtime on Silk.NET + OpenGL that ships to players. Both backends
implement the same contract and consume the same scene format.

## Why two backends

Godot as production is lock-in. Its `.tscn` scenes are tied to a specific
renderer, its `SceneTree` is tied to the main thread, its UI requires `Control`
nodes. For a game with 500+ pawns and deep magic, that is a ceiling. But **as
an editor** Godot is irreplaceable: TileMap, inspector, plugin API, scene tree.

The solution: Godot lives as a development tool. The designer opens a `.tscn`,
places nodes with `DFEntityMeta`, exports to `.dfscene`. From there, everything
is identical for both backends.

## Contracts

Three contracts in `DualFrontier.Application`:

- `IRenderer` — the main render loop (`Initialize`, `RenderFrame`, `Shutdown`).
- `ISceneLoader` — parses `.dfscene`, returns `SceneDef`.
- `IInputSource` — input source, publishes events to the buses.

Every Presentation assembly implements all three. Application imports neither
Godot nor Silk.NET — it only uses the contracts.

## Tier split: production vs devkit

The contracts split across two tiers:

### Production-tier

The minimum set required for any backend. Everything the player sees in the
finished game. Implemented in both Godot DevKit and Native.

- `IRenderer` — main render loop.
- `ISceneLoader` — `.dfscene` loading.
- `IInputSource` — input into the buses.

### DevKit-tier

Extensions for development tooling. Implemented **only** in Godot DevKit.
Native does not know about them at all — no references, no `NotImplementedException`
stubs, nothing. All types are marked `[DevKitOnly]`.

- `IDevKitRenderer : IRenderer` — debug gizmos, system profiler, entity highlighting.
- *(future)* `ISceneEditor` — live-reload of scenes, editing directly at runtime.
- *(future)* `IDebugTimeControl` — pause, single-tick step, fast-forward for bug reproduction.

### Tier-selection rule

Ask yourself: **does the player need this in the finished game?**

- Yes, always → production-tier (`IRenderer`).
- Only for developers or modders → devkit-tier (`IDevKitRenderer`).
- Unsure → production-tier. Expanding a contract is easier than narrowing it.

### Why not just two assemblies with the same API

If `IRenderer` contained `DrawDebugGizmo`, Native would be forced either to
implement it (spending code and binary on something not needed) or to throw
`NotImplementedException` (cluttering the contract). The tier split resolves
this cleanly: Native simply does not know debug methods exist.

### Promotion policy

If a method from the devkit-tier turns out to be needed in production (for
example, `HighlightEntity` becomes required for an in-game tutorial), it can
be **promoted**:

1. Move the method from `IDevKitRenderer` to `IRenderer`.
2. Implement it in Native.
3. Update the docs.

The reverse direction — demotion — is forbidden. Removing a method from the
production tier breaks the contract for existing Native builds.

## .dfscene format

Human-readable JSON, versioned:

```json
{
  "version": 1,
  "name": "colony_start",
  "tilemap": { "width": 100, "height": 100, "tileSize": 32, "layers": [...] },
  "entities": [...],
  "markers": [...],
  "metadata": {...}
}
```

Tilemap stores tiles per-layer as a base64-encoded `ushort[]` of length `width * height`.
Entity describes a prefab + position + component overrides. Marker is a named
point without an entity.

Detailed schema: `src/DualFrontier.Application/Scene/README.md`.

## Godot DevKit plugin

Lives in `src/DualFrontier.Presentation/addons/df_devkit/`. The `#if TOOLS`
flag guarantees the plugin does not end up in a production Godot build.

Features:

- `DFEntityMeta` Resource — attaches to nodes, defines prefab and overrides.
- `SceneExporter` — walks the SceneTree, serializes to `.dfscene`.
- Menu "Tools → Export .dfscene" — developer-facing export button.

Full implementation: Phase 3.5.

## Native runtime

The `DualFrontier.Presentation.Native` assembly. Depends only on `Contracts` and
`Application`. Technology stack:

| Layer              | Library                                   |
|--------------------|-------------------------------------------|
| Window + GL context | Silk.NET.Windowing                       |
| Input              | Silk.NET.Input                            |
| 2D rendering       | Silk.NET.OpenGL + a custom SpriteBatch    |
| UI                 | ImGui.NET                                 |
| Audio              | OpenAL-CS                                 |

Currently these are stubs with `throw new NotImplementedException`. Implementation: Phase 5+.

## Rules

- Domain and Application never import `Godot;` and never import
  `Silk.NET;`. Contracts only.
- `[DevKitOnly]` marks every Godot-specific piece of code that must not
  migrate to Native.
- Both backends MUST produce the same result on the same `.dfscene`.
  If behavior diverges, the bug is in the bridge commands.
- `.dfscene` is the only scene format. `.tscn` is for Godot Editor only and
  is not read at runtime.

## Developer pipeline

```
Godot Editor (scene author)
    ↓  Tools → Export .dfscene
assets/scenes/colony.dfscene
    ↓  (identical file)
    ├─ GodotRenderer + GodotSceneLoader   ← F5 in Godot, rapid iteration
    └─ NativeRenderer + NativeSceneLoader ← dotnet run, production check
```

CI runs both implementations on the same `sample.dfscene` fixture — behavior
MUST be identical.

## See also

- [ARCHITECTURE](./ARCHITECTURE.md) — four layers, dependency rules.
- [GODOT_INTEGRATION](./GODOT_INTEGRATION.md) — `PresentationBridge`, main thread.
- [ROADMAP](./ROADMAP.md) — Phase 3.5 (DevKit plugin), Phase 5+ (Native).
