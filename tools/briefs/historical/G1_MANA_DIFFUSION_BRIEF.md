---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-G1
category: D
tier: 3
lifecycle: SUPERSEDED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-G1
---
> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.
# G1 — First field compute shader (mana diffusion)

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G1
**Prerequisites**: G0 closed

## Goal

First production-shaped use case. `Vanilla.Magic` mod registers `ManaField` (200×200, float32) и mana diffusion compute shader (isotropic, decay, source map). Mod startup wires field + pipeline + scheduled dispatch. CPU/GPU equivalence verified on reference scenarios.

## Time estimate

~1 week

## Deliverables (high-level)

- `Vanilla.Magic` mod registering `ManaField` через `IModApi.Fields.RegisterField<float>(...)`
- `mana_diffusion` compute shader (GLSL → SPIR-V)
- Mod startup code wiring field + pipeline + scheduled dispatch via `IModApi.RegisterSystem<ManaFieldUpdateSystem>`
- CPU reference implementation для shader equivalence test
- Integration tests: mana sources spread spatially per shader; CPU и GPU output match within tolerance

## Success criteria

- `ManaField` visibly diffuses across grid in-game
- Spell-casting systems can read local mana via point query
- CPU/GPU equivalence verified

## Status: NOT STARTED

Awaiting G0 closed.
