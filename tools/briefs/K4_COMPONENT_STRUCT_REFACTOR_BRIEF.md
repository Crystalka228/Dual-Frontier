---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K4
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K4
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K4
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K4
---
# K4 — Component struct refactor (Path α)

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K4
**Prerequisite**: K3 complete

## Goal

Convert all class-based components к `unmanaged` structs. ~50-80 components в `DualFrontier.Components/`. Eliminates GC pressure structurally.

## Time estimate

2-3 weeks (substantial scope, mostly mechanical)

## Deliverables (high-level)

- All components converted от `class X : IComponent` к `struct X : IComponent`
- Systems updated to use `ref` semantics для mutation
- Tests updated (existing 472 still passing)
- Components с complex behavior split: pure-data struct + behavior class

## TODO

- [ ] Author full brief
- [ ] Inventory all 50-80 components
- [ ] Identify components needing redesign (behavior, reference fields)
- [ ] Include conversion sequence (5-10 components per commit)
- [ ] Include allocation profile target (zero managed allocs in component access)
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K3 closure.
