---
register_id: DOC-D-G4
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
title: G4 — Multi-Field Coexistence (historical; reframed as V substrate close criterion)
superseded_by: DOC-A-VULKAN_SUBSTRATE
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: 'Q-G-2 LOCK: G4 multi-field coexistence reframed as V substrate close acceptance criterion (not separate primitive). Brief content superseded by VULKAN_SUBSTRATE.md §1.4 (V substrate close acceptance criteria). Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).'
---

> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
# G4 — Multi-field coexistence

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G4
**Prerequisites**: G3 closed

## Goal

Validates architecture с multiple simultaneous fields. `Vanilla.Magic` + `Vanilla.Electricity` + `Vanilla.Water` all active simultaneously; independent dispatch scheduling per field (different update intervals). GPU memory budget verification (worst-case all-fields-active scenario). Cross-field interaction tests (none expected; verify isolation).

## Time estimate

~3-5 days

## Deliverables (high-level)

- 3 vanilla mods active concurrently
- Independent dispatch scheduling per field
- GPU memory budget worst-case verification
- Cross-field isolation tests

## Success criteria

- Three fields run concurrently без interference, within performance budget

## Status: NOT STARTED

Awaiting G3 closed.