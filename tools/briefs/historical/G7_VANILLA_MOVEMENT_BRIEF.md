---
register_id: DOC-D-G7
project: Dual Frontier
category: D
tier: 3
lifecycle: SUPERSEDED
owner: Crystalka
version: 1.0
first_authored: 2026-05-16
last_modified: 2026-05-16
content_language: en
next_review_due: null
title: G7 — Vanilla Movement (historical; demonstrated by M-V7 on V2)
superseded_by: DOC-A-VULKAN_SUBSTRATE
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: 'Q-G-2 LOCK: G7 reframed as M-V7 demonstration (Vanilla.Movement routed flow field pathfinding) on V2 substrate primitive (wave shader). Brief content superseded by VULKAN_SUBSTRATE.md §1.3 + §5.3 + §5.5 + §6.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).'
---

> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
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