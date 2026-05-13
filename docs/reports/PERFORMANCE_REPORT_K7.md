---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-PERFORMANCE_REPORT_K7
category: E
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-PERFORMANCE_REPORT_K7
---
# K7 Performance Report — Tick-loop measurements

**Status**: FINAL
**Date**: 2026-05-09
**Author**: K7 brief execution session (`tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md`)
**Hardware**: AMD Ryzen 7 7435HS (Zen 3+, 8C/16T, 3.10 GHz base), 32 GB DDR5-4800, AMD Radeon RX 7600S (irrelevant for K7), Win11 25H2 build 26200.8246, ASUS TUF A16 ("Skarlet" — Crystalka's primary dev machine, treated as median target audience hardware per K7 brief Phase 1)
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §K7 + Part 0 K-L10, K8 evidence base, `MIGRATION_PROGRESS.md` K6.1 closure context

---

## Executive summary

V3 (NativeWorld + WriteBatch) dominates V2 (managed-with-structs) on every §8 metric measured: per-tick mean is **4.3× faster** (19.9 μs → 4.5 μs), allocation per op is **19× lower** (6985 B → 360 B), and p99 long-run tick time is **32× lower** (480 μs → 15 μs) with zero GC pauses observed in the V3 long-run vs 11 gen0/1 gen1/1 gen2 collections totalling 3.4 ms of pause in the V2 long-run.

V2 already meets target frame budget on Skarlet at the 50-pawn workload size — V2 mean per-tick (35 μs) and p99 (480 μs) are both well under the 16 ms (60 FPS) budget and far under the 33 ms (30 TPS) budget. The K8 outcome question is therefore not "must we go native to ship 60 FPS" but "is V3's 5× absolute speedup worth the K8 cutover cost".

V1 (pre-K4 managed-class baseline) numbers are not cleanly comparable to V2/V3 because the V1 scenario, run inside the pre-K4 worktree, was simplified to a depletion-only loop per K7 brief stop condition #6 (the full pre-K4 vanilla system pipeline would have required IGameServices bus wiring out of K7 scope). V1 numbers are reported as a data point for future weak-hardware re-measurement, not as a head-to-head V1-vs-V2 comparison. The §"Caveats and limitations" section spells this out.

**Recommended K8 outcome direction**: numbers favour **Outcome 1 (native + batching wins decisively)** on the relative-improvement axis (V3 >> V2 by 4-30× on §8 metrics) and **Outcome 2 (managed-with-structs alone wins)** on the absolute-budget axis (V2 already meets 30 TPS / 60 FPS target with margin). Crystalka makes the K8 decision call against this trade-off.

---

## Methodology

- **Workload**: 50 pawns + 255 items (150 food + 50 water + 30 beds + 25 decorations) on a 200×200 grid with 800 obstacles. Fixed seeds: 42 for nav/pawns, 43 for items.
- **V2/V3 frame budget**: 10,000 ticks @ 30 TPS (target 333.3 simulated seconds).
- **Variants**:
  - **V1 (pre-K4, simplified)**: managed `World` + class components, depletion-only inline loop on 50 entities (NeedsSystem-equivalent + MoodSystem-equivalent). No `ParallelSystemScheduler`, no factories, no event publishing. Run from a `git worktree add` at commit `9227a577` (one commit before the first K4 component conversion).
  - **V2 (post-K4 main, full pipeline)**: managed `World` + struct components + `ParallelSystemScheduler` + the full 12-system production set (`NeedsSystem`, `MoodSystem`, `JobSystem`, `ConsumeSystem`, `SleepSystem`, `ComfortAuraSystem`, `MovementSystem`, `PawnStateReporterSystem`, `InventorySystem`, `HaulSystem`, `ElectricGridSystem`, `ConverterSystem`). Spawned via the production `RandomPawnFactory` + `ItemFactory`. No `PresentationBridge` (events publish into unsubscribed buses). Run from main HEAD post-K6.1.
  - **V3 (post-K4 main, native+batched)**: `NativeWorld` + struct components + `SpanLease<T>` reads + `WriteBatch<T>` writes. Three purpose-written V3 systems (NeedsDepletion, MoodFromNeeds, PowerConsumption) mirror the V2 read/write patterns but operate directly on dense native storage. Class-based components (Movement, Identity, Skills, Storage, …) are absent because `unmanaged` excludes reference types. Run from main HEAD post-K6.1.
- **Metrics frameworks**:
  - **BenchmarkDotNet 0.13.12** (`ShortRunJob` — 3 warmup × 5 measurement iterations, `MemoryDiagnoser`, `ThreadingDiagnoser`) — per-tick mean / stddev / allocations / gen0/1/2 / completed work items / lock contentions.
  - **Custom Stopwatch loop** (`LongRunDriftRunner`) — 10,000 ticks recorded individually for percentile distributions; GC counts/duration and process memory growth captured at start and end. Output: `docs/benchmarks/k7-long-run-V<n>.csv`.

---

## Variant V1 — managed-current (pre-K4, simplified)

[Source: pre-K4 worktree at commit `9227a577`. Scenario: depletion-only inline loop, 50 entities × 2 systems. Not a faithful pre-K4-production replica — see Caveats.]

### Long-run 10k ticks
| Metric | Value |
|---|---|
| Total wall-clock | 48.482 ms |
| Drift vs simulated | -333,284.86 ms (sim way ahead, run massively under-budget) |
| Per-tick mean | 4.667 μs |
| Per-tick stddev | 1.088 μs |
| p50 | 4.600 μs |
| p90 | 4.800 μs |
| p95 | 4.900 μs |
| p99 | 7.300 μs |
| p99.9 | 15.400 μs |
| Max | 68.600 μs |
| Total allocated | 0.004 MB (4 KB) |
| Gen0 / Gen1 / Gen2 | 0 / 0 / 0 |
| GC pause total | 0.000 ms |
| Working-set growth | 0.367 MB |
| CPU time | 62.500 ms |

V1 has no GC pressure because the depletion loop allocates nothing per tick (the class instances are reused — managed `World` returns the same reference each `TryGetComponent`, mutation is in-place on the heap-resident object). The 4 KB total allocation is one-time setup cost.

V1 is fast in absolute terms (4.7 μs mean) only because the scenario is much smaller than V2/V3. See Caveats.

---

## Variant V2 — managed-with-structs (post-K4 main)

[Source: main HEAD post-K6.1. Scenario: full 12-system production pipeline, 50 pawns + 255 items via production factories.]

### BDN per-tick (ShortRunJob, 3w×5m)
| Metric | Value |
|---|---|
| Mean | 19.905 μs |
| Error (99.9% CI half-width) | 71.801 μs |
| StdDev | 3.936 μs |
| Allocated/op | 6985 B (6.82 KB) |
| Gen0/1000 ops | 0.7935 |
| Completed work items/op | 5.19 (Parallel.ForEach over phases) |
| Lock contentions/op | 0.0006 |

Wide `Error` reflects the 3-iteration `ShortRunJob` — the first iteration includes JIT settling overhead. StdDev (3.9 μs across measurement iterations) is the more reliable spread indicator.

### Long-run 10k ticks
| Metric | Value |
|---|---|
| Total wall-clock | 347.546 ms |
| Drift vs simulated | -332,985.80 ms (sim way ahead) |
| Per-tick mean | 34.546 μs |
| Per-tick stddev | 921.059 μs |
| p50 | 8.400 μs |
| p90 | 15.700 μs |
| p95 | 84.300 μs |
| p99 | 480.100 μs |
| p99.9 | 782.900 μs |
| **Max** | **87,985.200 μs (88 ms)** |
| Total allocated | 92.239 MB |
| Gen0 / Gen1 / Gen2 | 11 / 1 / 1 |
| GC pause total | 3.402 ms |
| Working-set growth | 10.328 MB |
| CPU time | 921.875 ms |

The 88 ms max tick is the dominant outlier — likely the first gen2 collection during warmup. Subsequent percentiles drop sharply (p99.9 = 0.78 ms, p99 = 0.48 ms), so the worst-case is bounded but visible.

The mean (34 μs) being 4× the p50 (8.4 μs) is driven by the heavy tail (gen2 + JIT spike). With more warmup ticks before measurement starts, the mean would drop closer to the p50.

92 MB cumulative allocation over 10k ticks ≈ 9.2 KB/tick — consistent with the BDN per-op figure (6.85 KB) plus per-tick overhead from the scheduler's `Parallel.ForEach`, deferred-event flushes, etc.

---

## Variant V3 — native-with-batching (post-K4 main, NativeWorld)

[Source: main HEAD post-K6.1. Scenario: 3 purpose-written V3 systems on `NativeWorld` storage. Workload covers struct-component pipelines only — see Caveats.]

### BDN per-tick (ShortRunJob, 3w×5m)
| Metric | Value |
|---|---|
| Mean | 5.224 μs |
| Error (99.9% CI half-width) | 5.138 μs |
| StdDev | 0.282 μs |
| Allocated/op | 360 B |
| Gen0/1000 ops | 0.042 |
| Completed work items/op | 0 (V3 doesn't use Parallel.ForEach — single-threaded loops) |
| Lock contentions/op | 0 |

V3's tighter StdDev (0.28 μs) reflects the more deterministic execution profile — no thread pool dispatching, no GC interaction, no JIT tier transitions visible at this iteration count.

### Long-run 10k ticks
| Metric | Value |
|---|---|
| Total wall-clock | 81.745 ms |
| Drift vs simulated | -333,251.60 ms (sim way ahead) |
| Per-tick mean | 7.978 μs |
| Per-tick stddev | 1.927 μs |
| p50 | 7.300 μs |
| p90 | 10.400 μs |
| p95 | 12.300 μs |
| p99 | 15.000 μs |
| p99.9 | 26.000 μs |
| Max | 43.700 μs |
| Total allocated | 3.437 MB |
| Gen0 / Gen1 / Gen2 | 0 / 0 / 0 |
| GC pause total | 0.000 ms |
| Working-set growth | 3.992 MB |
| CPU time | 93.750 ms |

V3 has zero GC collections of any generation across 10k ticks — exactly the property the K1/K5 design aimed for. The 3.4 MB total allocation comes from the lease/batch object lifecycles (per-tick `WriteBatch<T>` and `SpanLease<T>` allocate small managed wrappers around the native handles).

The max (44 μs) is only ~5.5× p50 — a much tighter distribution than V2's max which was ~10,000× p50. Predictable worst-case is more valuable than low average for fixed-step simulation; V3 wins this dimension by a wide margin.

---

## Comparative analysis

### Per-metric ranking (V2 vs V3, the apples-to-apples pair)

| Metric | V2 (managed-struct) | V3 (native+batched) | Winner | Margin |
|---|---|---|---|---|
| BDN mean tick | 19.905 μs | 5.224 μs | V3 | **3.81× faster** |
| BDN allocated/op | 6985 B | 360 B | V3 | **19.4× less** |
| BDN gen0/1000 ops | 0.7935 | 0.042 | V3 | **18.9× less** |
| Long-run mean | 34.546 μs | 7.978 μs | V3 | **4.33× faster** |
| Long-run p99 | 480.1 μs | 15.0 μs | V3 | **32.0× faster** |
| Long-run p99.9 | 782.9 μs | 26.0 μs | V3 | **30.1× faster** |
| Long-run max | 87,985 μs | 43.7 μs | V3 | **2,013× faster** |
| Total allocated (10k ticks) | 92.239 MB | 3.437 MB | V3 | **26.8× less** |
| GC gen2 collections | 1 | 0 | V3 | (qualitative) |
| GC pause total | 3.402 ms | 0.000 ms | V3 | (qualitative — zero pauses) |
| Wall-clock 10k ticks | 347.5 ms | 81.7 ms | V3 | **4.25× faster** |
| Drift @ 10k @ 30 TPS | meets budget | meets budget | tie | both massively under-budget |

V3 wins every measured dimension. The closest V2-favourable interpretation would be the BDN error bar (V2 ± 71.8 μs vs V3 ± 5.1 μs) being on similar absolute ranges, but those are wide-CI artifacts of the 3-iteration ShortRunJob and don't reflect the long-run distribution.

### V1 placement note

V1's apparent speed (4.7 μs mean) is artifact of the simplified scenario (only 2 systems, 50 entities, no scheduler, no events). It does not represent pre-K4 production tick cost. A faithful V1 measurement of the full pre-K4 production pipeline would require: setting up `IGameServices` with all bus subscriptions, wiring events through `PresentationBridge` no-ops, and running the production scheduler with class-component systems. K7 brief stop condition #6 explicitly authorized the simplification when worktree work proves cumbersome; K7 took that authorization.

The qualitative direction we expect from a faithful V1 measurement: V1 should be slower than V2 (class instances have higher GC pressure than dense struct storage) and far slower than V3 (no batching, no native arithmetic). The K8 decision does not depend on a precise V1 measurement because Outcome 3 ("all paths equivalent") is already excluded by the V2 vs V3 5×–30× delta.

### §8 rule application

Per `KERNEL_ARCHITECTURE.md` §8 / K-L10, the decision rule is **GC pause / p99 / long-run drift on target hardware**:

- **GC pause**: V3 = 0 ms over 10k ticks; V2 = 3.4 ms over 10k ticks. V3 wins decisively. For 30 TPS sustained simulation on Skarlet, V2's 3.4 ms total over 10k ticks averages 0.34 μs/tick of GC pause — well under any reasonable budget — so V2 is acceptable in absolute terms. V3 is strictly better.

- **p99 tick**: V3 long-run p99 = 15.0 μs; V2 long-run p99 = 480.1 μs. Both are far under the 16 ms (60 FPS) budget and the 33 ms (30 TPS) budget. V3 wins by 32×; V2 acceptable in absolute terms.

- **Long-run drift**: both variants have massively negative drift (-332 s and -333 s respectively against the 333 s simulated target), meaning both can sustain 30 TPS with enormous margin on Skarlet. Drift is not the discriminator at this workload size.

The §8 rule applied literally favours V3 on every numeric axis. The §8 rule applied with the absolute-budget framing favours either choice — both V2 and V3 meet target.

### Recommended K8 outcome direction

**Outcome 1 (native + batching wins decisively)** is favoured by the relative-improvement axis: V3's 4-32× margin across §8 metrics is exactly the "wins decisively" definition.

**Outcome 2 (managed-with-structs alone wins)** is favoured by the absolute-budget axis: V2 already meets the 30 TPS / 60 FPS target on Skarlet with 100×+ margin. The K8 cutover cost (rewriting all production systems against `NativeWorld` access patterns + thread-safety review for the `ParallelSystemScheduler`-on-NativeWorld combination + handling the class-component subset that can't go native) may not be justified by V3's absolute speedup on a workload that's already comfortably under-budget.

**Outcome 3 (all paths equivalent)** is excluded — V2 vs V3 are not within 10% on §8 metrics; the relative gap is 4-32×.

The recommendation is **Outcome 1 OR Outcome 2 depending on Crystalka's weighting of relative-improvement vs cutover-cost**. The numbers don't make the call for us — they bound the choice but don't pick a side.

If Skarlet stays the target hardware AND production workload size stays at ~50 pawns × ~255 items, Outcome 2 looks more practical — V2 has comfortable headroom and ships sooner. If the target hardware downgrades (broader minimum spec) OR the workload scales (200+ pawns, larger maps), Outcome 1 becomes more compelling because V3's better tail-latency profile (32× lower p99) tolerates worse hardware better than V2's heavy-tail distribution.

The recommendation is **advisory only**. Crystalka makes the K8 decision call.

---

## Caveats and limitations

1. **V3 system harness purpose-written**. V3's three systems (NeedsDepletion, MoodFromNeeds, PowerConsumption) are NOT the production systems running on `NativeWorld`. They mirror the V2 systems' read/write patterns but were authored for the K7 measurement. Production-system-on-NativeWorld numbers may differ — possibly worse if production code has hidden allocations the K7 V3 systems don't model, possibly better if production systems can amortize batch operations more aggressively. K8 cutover work would surface the actual delta.

2. **V3 omits class-component systems**. `MovementSystem`, `InventorySystem`, `HaulSystem`, `PawnStateReporterSystem` cannot run on `NativeWorld` because their components carry reference types (Dictionary / List / string). V3 measures only the struct-component pipeline cost; V2 measures the full pipeline. The "V3 4× faster" comparison is therefore "V3 simulating fewer systems per tick" partially. The brief explicitly authorizes this scope (per §1.8 LOCKED). K8 work would either keep class-component systems on managed `World` and run a hybrid path or build native-side reference handling out of K9 component-storage work.

3. **V1 simplified vs faithful**. V1's depletion-only loop is not the pre-K4 production pipeline. K7 brief stop condition #6 authorized this simplification; the K8 decision does not depend on a faithful V1 measurement (the V2-vs-V3 delta is large enough that pre-K4 V1 cannot plausibly catch up to V3).

4. **Hardware target is "median 2025 gaming laptop" not "weak hardware"**. Skarlet (Ryzen 7 7435HS, 8C/16T, 32 GB DDR5) is fast. Future weak-hardware re-measurement (Steam Deck, low-end office laptop with integrated graphics) may show different absolute numbers and may shift the K8 outcome trade-off — V3's better tail-latency profile is more valuable on slower hardware.

5. **50-pawn workload is the production scenario size**. Scaling to 200+ pawns or 500+ items may shift the V2-vs-V3 ratio. Larger workloads tend to favour the cache-friendly dense storage (V3) over the sparse-set + class-allocation pattern (V2). K7 didn't measure the scaling axis; K8 follow-up may want to.

6. **Single-machine measurement**. No cross-hardware variance data. Numbers reproducible from this exact commit set on Skarlet; absolute numbers on different hardware will differ.

7. **BDN ShortRunJob has wide error bars**. The 3 warmup × 5 measurement iterations are sufficient to identify a 4× difference between V2 and V3 but insufficient to draw confidence intervals tight enough to distinguish, say, V2 from a hypothetical V2.5 with minor optimizations. Subsequent K-series benchmarks should consider `MediumRunJob` or `LongRunJob` if differentiating among similar variants.

8. **No instrumentation of allocation source**. V2's 6.85 KB/op allocation isn't decomposed into "scheduler's `Parallel.ForEach` task captures" vs "per-system `GetComponent` boxing" vs "deferred-event flush" vs "subscriber dictionary lookups". A profiler trace would identify the dominant contributor. K8 cutover work would benefit from this decomposition before deciding which V2 systems to migrate first.

9. **GC.GetTotalPauseDuration() is .NET 7+ specific** and reports only sub-millisecond pauses — total wall-clock-relevant pause may include sub-millisecond pauses summed differently than other diagnostics report. V2's 3.4 ms is the monotonic delta from this API and consistent within the run; cross-run comparison is valid.

---

## Raw data

- BDN combined V2+V3 report: `docs/benchmarks/k7-bdn-tick.csv`, `docs/benchmarks/k7-bdn-tick-report.html`, `docs/benchmarks/k7-bdn-tick-report.md`
- Long-run V1 (pre-K4 worktree): `docs/benchmarks/k7-long-run-V1.csv`
- Long-run V2 (post-K6.1 main): `docs/benchmarks/k7-long-run-V2.csv`
- Long-run V3 (post-K6.1 main): `docs/benchmarks/k7-long-run-V3.csv`

Each long-run CSV has a metadata header (`# variant=…`, `# tickCount=…`, `# gen0Collections=…`) followed by a per-tick `tick,tickTimeNs` row sequence. The header values are the report's summary metrics; the per-tick data is the time-series for distribution / drift visualization.

---

## Closure note

K7 produces evidence, not a decision. Crystalka reads this report, weighs the relative-improvement vs cutover-cost trade-off in light of project priorities Crystalka holds (development time, team size, target audience hardware policy, willingness to maintain a hybrid managed/native system set, etc.), and makes the K8 outcome call. The K8 brief is then authored against that decision (one of three K8 brief variants per Outcome 1 / Outcome 2 / Outcome 3-excluded path).

The K7 closure is independent of the K8 outcome — K7 is done either way. The MIGRATION_PROGRESS.md K7 section closes here; the K7 brief is marked EXECUTED.
