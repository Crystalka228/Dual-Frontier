---
register_id: DOC-D-G8
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
title: G8 — Local Avoidance (historical; demonstrated by M-V8, mod-level)
superseded_by: DOC-A-VULKAN_SUBSTRATE
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: 'Q-G-2 LOCK: G8 local avoidance reframed as M-V8 demonstration — mod-level concern, NOT V substrate primitive. Brief content superseded by VULKAN_SUBSTRATE.md §5.5 (local avoidance separate concern) + §6.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).'
---

> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
# G8 — Local avoidance layer

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G8
**Prerequisites**: G7 closed

## Goal

Local steering on top of flow field direction. RVO-like or simple boids approach; combines flow field global direction + local agent collision avoidance. Pure managed CPU code (per-pawn, но simple math, parallelizable).

## Time estimate

~3-5 days

## Deliverables (high-level)

- RVO or boids local steering layer
- Combines с flow field gradient
- Managed CPU implementation
- Per-pawn cost benchmark
- Tests: pawns following identical gradient do not cluster / jam / oscillate beyond tolerance

## Success criteria

- Crowd traversal through choke points без deadlock at target pawn counts

## Status: NOT STARTED

Awaiting G7 closed.