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
