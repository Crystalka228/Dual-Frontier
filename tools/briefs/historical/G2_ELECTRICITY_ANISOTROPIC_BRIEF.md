---
register_id: DOC-D-G2
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
title: G2 — Electricity Anisotropic (historical; demonstrated by M-V2 on V1)
superseded_by: DOC-A-VULKAN_SUBSTRATE
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: 'Q-G-2 LOCK: G2 reframed as M-V2 demonstration (Vanilla.Electricity power field) on V1 anisotropic diffusion variant. Brief content superseded by VULKAN_SUBSTRATE.md §1.2 + §5.1 + §6.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).'
---

> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
# G2 — Anisotropic diffusion (electricity)

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G2
**Prerequisites**: G1 closed

## Goal

Adds conductivity map handling. `Vanilla.Electricity` mod registers `PowerField` + conductivity map + cliff-threshold consumer effectiveness в managed code. Wire placement updates conductivity map (player build action). Shader implements `min(D_self, D_neighbor)` flow gating.

## Time estimate

~1 week

## Deliverables (high-level)

- `Vanilla.Electricity` mod registering `PowerField` + conductivity map
- Anisotropic diffusion shader с asymmetric flow rule
- Cliff threshold (60% rule) consumer effectiveness в managed code
- Wire placement updates conductivity map
- Tests: power propagates along wire paths; off-wire decay matches expectation; cliff threshold offlines underpowered consumers

## Success criteria

- Electricity behaves as designed (sources, wires, consumers, brownouts) in stress test scenarios

## Status: NOT STARTED

Awaiting G1 closed.