# K2 — Type-id registry + bridge tests

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K2
**Prerequisite**: K1 complete

## Goal

Replace FNV-1a hash с explicit deterministic registry. Comprehensive bridge test coverage (~30-40 tests).

## Time estimate

2-3 days

## Deliverables (high-level)

- `ComponentTypeRegistry` class (sequential IDs)
- C ABI: `df_world_register_component_type`
- New project: `DualFrontier.Core.Interop.Tests`
- Tests: 30-40 covering NativeWorld, packing, registry, spans, write buffer

## TODO

- [ ] Author full brief
- [ ] Include test scaffolding template
- [ ] Include test category breakdown
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K1 closure.
