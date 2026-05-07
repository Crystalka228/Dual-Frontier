# K3 — Native bootstrap graph + thread pool

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K3
**Prerequisite**: K2 complete

## Goal

Declarative startup task graph executed parallel где deps allow. Native scheduler used ONLY для bootstrap (not game tick).

## Time estimate

5-7 days

## Deliverables (high-level)

- `bootstrap_graph.h/cpp` — declarative `kStartupTasks` array, Kahn topological sort
- `thread_pool.h/cpp` — std::thread pool (N cores)
- `df_engine_bootstrap()` C ABI entry point
- Selftest scenario: `scenario_bootstrap_graph`
- Benchmark: `BootstrapTimeBenchmark` (parallel vs sequential)

## TODO

- [ ] Author full brief
- [ ] Enumerate startup tasks с dependencies
- [ ] Include thread pool design (work-stealing vs fixed-partitioned)
- [ ] Include benchmark target metrics (≤15ms typical hardware)
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K2 closure.
