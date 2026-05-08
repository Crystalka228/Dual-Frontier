# G9 — Eikonal upgrade (optional, evidence-gated)

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/GPU_COMPUTE.md` v2.0 Roadmap §G9
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
