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
