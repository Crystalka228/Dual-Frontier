# K8.3 — 12 vanilla systems migrated to SpanLease/WriteBatch

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K8, K-L7 (span protocol), K-L11 (NativeWorld backbone)
**Prerequisite**: K8.2 closure (all components are unmanaged structs)

## Goal

Migrate the 12 vanilla production systems from `World.GetComponent` / `World.SetComponent` access patterns to `NativeWorld` + `SpanLease<T>` reads + `WriteBatch<T>` writes. After K8.3 closure, all production system code runs against NativeWorld.

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
- [ ] Test fixture design — how does a system test construct a NativeWorld with the right components registered?
- [ ] Migrate test count (some tests currently use ManagedWorld directly; switching to NativeWorld may surface latent assumptions)
- [ ] Decide if K8.3 includes a benchmark commit confirming production-on-NativeWorld matches K7 V3 within 20% (recommended: yes)

**Brief authoring trigger**: after K8.2 closure.
