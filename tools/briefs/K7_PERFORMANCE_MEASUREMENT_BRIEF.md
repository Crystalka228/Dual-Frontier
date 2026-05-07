# K7 — Performance measurement (tick-loop)

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K7
**Prerequisite**: K6 complete

## Goal

Representative-load benchmark applying §8 metrics rule (GC pause / p99 / long-run drift on weak hardware). Evidence base для K8 decision step.

## Time estimate

3-5 days

## Deliverables (high-level)

- `TickLoopBenchmark` — 50 pawns × full component set × 10k ticks
- Three variants: managed-current, managed-with-structs, native-with-batching
- Metrics: p50/p95/p99, GC count + duration, total allocations, drift
- Run on weak hardware (Docker cpu-limit OR secondary machine)
- Report: `docs/PERFORMANCE_REPORT_K7.md`

## TODO

- [ ] Author full brief
- [ ] Include benchmark scenario definition
- [ ] Include weak-hardware target spec
- [ ] Include report template
- [ ] Include acceptance criteria (metrics threshold для §8 rule)

**Brief authoring trigger**: after K6 closure.
