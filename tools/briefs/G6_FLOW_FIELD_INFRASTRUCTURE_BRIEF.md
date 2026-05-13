---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-G6
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-G6
---
# G6 — Flow field infrastructure

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G6
**Prerequisites**: G5 closed (or G4 closed if scheduling permits early start)

## Goal

Engine-side flow field primitives. No gameplay binding yet. Distance field compute shader (Option B simple diffusion variant initially); direction field compute shader (gradient extraction); `Vector2` field type support в kernel (K9 extension).

## Time estimate

~3-5 days

## Deliverables (high-level)

- Distance field compute shader (Option B simple diffusion)
- Direction field compute shader (gradient extraction)
- `Vector2` field type kernel support
- Tests: distance field converges; gradient points correctly toward target on representative grids

## Success criteria

- Distance field + direction field shaders run end-to-end on synthetic obstacle grid
- Engine exposes `Vector2`-typed `RawTileField` cleanly

## Status: NOT STARTED

Awaiting G5 closed.
