# K1 — Batching primitive

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K1
**Prerequisite**: K0 complete

## Goal

Bulk Add/Get + Span<T> access via extended C ABI. Validates batching hypothesis quantitatively (target ≤200μs for bulk add 10k vs current 400μs unbatched).

## Time estimate

3-5 days

## Deliverables (high-level)

- C ABI extension (4 new functions: bulk add, bulk get, span acquire, span release)
- Managed bridge extension (`AddComponents<T>`, `AcquireSpan<T>`)
- Native span counter + mutation rejection
- Selftest scenarios (К1: bulk add, span access)
- Benchmark: `NativeBulkAddBenchmark`

## TODO

- [ ] Author full brief
- [ ] Include C++ implementation outlines
- [ ] Include C# bridge outlines
- [ ] Include benchmark target metrics
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K0 closure.
