# K5 — Span<T> protocol + write command batching

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K5
**Prerequisite**: K4 complete

## Goal

Production-grade span lifetime guard + write command batching infrastructure. Mutation rejection during active spans. Single P/Invoke per phase для writes.

## Time estimate

1 week

## Deliverables (high-level)

- `SpanLease<T>` (`IDisposable`) wrapping native span pointer
- `WriteCommandBuffer` full implementation (Add/Remove/Destroy/Spawn commands)
- C++ command buffer parser
- `df_world_flush_write_batch` C ABI
- Native atomic counter mutation rejection
- Tests: span lifetime, write batch round-trip, mutation rejection

## TODO

- [ ] Author full brief
- [ ] Include byte-stream serialization format spec
- [ ] Include native parser implementation outline
- [ ] Include test scenarios (lifetime, rejection, round-trip)
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K4 closure.
