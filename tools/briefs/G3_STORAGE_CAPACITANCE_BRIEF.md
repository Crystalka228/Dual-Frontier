---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-G3
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-G3
---
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
