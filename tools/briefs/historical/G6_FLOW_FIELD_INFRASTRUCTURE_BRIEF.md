---
register_id: DOC-D-G6
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
title: G6 — Flow Field Infrastructure (historical; folded into V2)
superseded_by: DOC-A-VULKAN_SUBSTRATE
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: 'Q-G-2 LOCK: G6 flow field infrastructure folded into V2 wave shader side products (distance/direction fields). Brief content superseded by VULKAN_SUBSTRATE.md §1.3 + §5.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).'
---

> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
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