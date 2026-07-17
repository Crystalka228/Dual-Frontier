---
register_id: DOC-D-G3
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
title: G3 — Storage Capacitance (historical; reduced to gameplay node config)
superseded_by: DOC-A-VULKAN_SUBSTRATE
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: 'Q-G-2 LOCK: G3 storage cells / capacitance reduced to gameplay-level node config (not shader feature, not substrate primitive). Brief content superseded by VULKAN_SUBSTRATE.md §5.1 + Lesson #12 candidate. Retained as historical record of reduction rationale. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).'
---

> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
# G3 — Storage cells / capacitance (batteries, tanks)

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G3
**Prerequisites**: G2 closed

## Goal

Adds temporal capacitance к field shaders. Storage flag handling в shader; battery/tank placement updates storage flags + retention parameters. `storage[t+1] = α · storage[t] + (1-α) · field_local[t]`; emit-when-demanded `β · storage[t]`.

## Time estimate

~3-5 days

## Deliverables (high-level)

- Storage flag handling в shader
- Battery/tank placement updates storage flags
- α/β retention parameters per storage cell type
- Tests: battery accumulates excess during low demand; battery discharges during droop; load-smoothing visible в benchmark scenarios

## Success criteria

- Power и water systems demonstrate storage cell behavior

## Status: NOT STARTED

Awaiting G2 closed.