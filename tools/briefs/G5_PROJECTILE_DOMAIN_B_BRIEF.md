# G5 — Domain B integration (`ProjectileSystem` reactivation)

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/GPU_COMPUTE.md` v2.0 Roadmap §G5
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
