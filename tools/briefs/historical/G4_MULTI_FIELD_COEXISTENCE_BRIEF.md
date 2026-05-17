---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-G4
category: D
tier: 3
lifecycle: SUPERSEDED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-G4
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
