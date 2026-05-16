---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-G9
category: D
tier: 3
lifecycle: SUPERSEDED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-G9
---
> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
# G9 — Eikonal upgrade (optional, evidence-gated)

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G9
**Prerequisites**: G7 closed; measurement of Option B suboptimality

## Goal

Replace simple diffusion с Fast Sweeping Method (FSM) или Fast Marching Method (FMM) when measurement justifies it. Geodesic-accurate distances; reuse direction field extraction unchanged. Side-by-side comparison с Option B output on shipped scenarios.

## Time estimate

~1 week (only if evidence justifies)

## Deliverables (high-level, only if shipped)

- FSM or FMM compute shaders для geodesic-accurate distances
- Side-by-side comparison с Option B
- Archive measurement если closure без code

## Success criteria

- Only shipped if Option B measurement shows gameplay-relevant suboptimality
- Otherwise milestone closes без code, с measurement archived

## Status: NOT STARTED

Awaiting G7 closed + measurement evidence.
