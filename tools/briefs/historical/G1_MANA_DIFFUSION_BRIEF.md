---
register_id: DOC-D-G1
project: Dual Frontier
category: D
tier: 3
lifecycle: SUPERSEDED
owner: Crystalka
version: 1.0
first_authored: 2026-05-16
last_modified: 2026-05-16
content_language: en
next_review_due: null
title: G1 — Mana Diffusion (historical; demonstrated by M-V1 on V1)
superseded_by: DOC-A-VULKAN_SUBSTRATE
review_cadence: on-status-transition
reviewer: Crystalka
special_case_rationale: 'Q-G-2 LOCK: G1 reframed as M-V1 demonstration (Vanilla.Magic mana field) on V1 substrate primitive (isotropic diffusion). Brief content superseded by VULKAN_SUBSTRATE.md §1.2 + §6.3. Retained as historical record. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).'
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