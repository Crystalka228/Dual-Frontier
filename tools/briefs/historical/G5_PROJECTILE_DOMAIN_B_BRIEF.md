---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-G5
category: D
tier: 3
lifecycle: SUPERSEDED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-G5
---
> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
# G5 — Domain B integration (`ProjectileSystem` reactivation)

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G5
**Prerequisites**: G4 closed

## Goal

Re-implements original Phase 3 `ProjectileSystem` GPU path on the new architecture. `ProjectileSystem` registers a compute pipeline для projectile update + collision; native kernel exposes projectile component span as SSBO directly; asynchronous readback с one-tick lag (Phase 3 pattern preserved). Threshold detection: managed code chooses CPU vs GPU based on entity count.

## Time estimate

~1 week

## Deliverables (high-level)

- `ProjectileSystem` compute pipeline через `IModApi.ComputePipelines`
- Native kernel exposes component span as SSBO
- Asynchronous readback с one-tick lag
- Threshold detection (CPU vs GPU swap)
- Performance comparison vs CPU baseline; threshold pinned per measurement

## Success criteria

- `ProjectileSystem` on GPU validates Domain B pattern
- Threshold для swap measured; falls back к CPU below threshold

## Status: NOT STARTED

Awaiting G4 closed.
