---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_3
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_3
---
# K8.3 — 12 vanilla systems migrated to SpanLease/WriteBatch

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K8, K-L7 (span protocol), K-L11 (NativeWorld backbone)
**Prerequisite**: K8.2 closure (all components are unmanaged structs)

## Goal

Migrate all 34 production systems in `src/DualFrontier.Systems/` (per `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.1 §1.2 reformulated scope) from `World.GetComponent` / `World.SetComponent` access patterns to dual-path access per K-L3.1 Q3.i: `SystemBase.NativeWorld` + `SpanLease<T>` reads + `WriteBatch<T>` writes for Path α components; `SystemBase.ManagedStore<T>()` for Path β components (when present in same mod). After K8.3 closure, all production system code runs against `NativeWorld` (Path α) and per-mod `ManagedStore<T>` (Path β if any).

## Systems in scope (per `GameBootstrap.coreSystems`)

NeedsSystem, MoodSystem, JobSystem, ConsumeSystem, SleepSystem, ComfortAuraSystem, MovementSystem, PawnStateReporterSystem, InventorySystem, HaulSystem, ElectricGridSystem, ConverterSystem.

## Time estimate

2-3 weeks at hobby pace.

## Deliverables (high-level)

- Each system rewritten to use SpanLease/WriteBatch access patterns
- Per-system atomic commits (12 atomic commits)
- Each system's tests updated to construct NativeWorld via test fixture
- New shared test fixture `NativeWorldTestFixture` for system tests
- Performance comparison commit: K8.3 vs K7 V2 baseline (sanity check that production-system-on-NativeWorld matches K7 V3 numbers within 20%)

## TODO

- [ ] Author full brief
- [ ] Per-system access pattern analysis (read-only vs read-write; phase placement; deferred-event publish ordering)
- [ ] Per-system Path α/β access audit — for each of 34 systems, identify Path α reads/writes and Path β reads/writes (Path β requires K8.4 plumbing; if K8.4 ships before K8.3 brief authoring, dual-path access enabled; if K8.3 authored before K8.4, Path β is empty and brief covers only Path α — defer Path β audit to amendment after K8.4)
- [ ] Test fixture design — how does a system test construct a NativeWorld with the right components registered?
- [ ] Migrate test count (some tests currently use ManagedWorld directly; switching to NativeWorld may surface latent assumptions)
- [ ] Decide if K8.3 includes a benchmark commit confirming production-on-NativeWorld matches K7 V3 within 20% (recommended: yes)

**Brief authoring trigger**: after K8.2 closure.
