# G3 — Storage cells / capacitance (batteries, tanks)

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/GPU_COMPUTE.md` v2.0 Roadmap §G3
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
