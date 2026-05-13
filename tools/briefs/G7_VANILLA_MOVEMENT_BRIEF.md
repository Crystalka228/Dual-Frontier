---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-G7
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-G7
---
# G7 — `Vanilla.Movement` integration

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G7
**Prerequisites**: G6 closed

## Goal

Pathfinding ships through a vanilla mod, replacing per-pawn A* on common destinations. Flow field lifecycle management (creation, refresh, eviction); per-target field assignment logic; movement system reads direction field, applies к velocity; hybrid A* fallback для unique destinations (preserves `PathfindingService` для residual case).

## Time estimate

~1 week

## Deliverables (high-level)

- `Vanilla.Movement` mod
- Flow field lifecycle management (LRU eviction, max 32 active)
- Per-target field assignment
- Movement system reads direction field
- Hybrid A* fallback wiring
- Tests: pawn navigates к work zone via flow field; pawn navigates к specific entity via A*; correct handoff between modes

## Success criteria

- 50+ pawns navigating shared work zones consume O(K) field updates regardless of pawn count
- A* exercised only on unique-destination paths

## Status: NOT STARTED

Awaiting G6 closed.
