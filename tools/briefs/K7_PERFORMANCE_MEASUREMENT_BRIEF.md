# K7 — Performance measurement (tick-loop)

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-09
**Status**: EXECUTED 2026-05-09 — closed at `72ea8b5..e917220`. See `docs/PERFORMANCE_REPORT_K7.md` for measurements + analysis and `docs/MIGRATION_PROGRESS.md` K7 closure section for closure record.
**Reference docs**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K7, `docs/MIGRATION_PROGRESS.md` (K6.1 closure context), `docs/METHODOLOGY.md`, `docs/CODING_STANDARDS.md`
**Companion**: `docs/MIGRATION_PROGRESS.md` (live tracker — K7 row promotes from NOT STARTED → DONE on closure)
**Methodology lineage**: `tools/briefs/K6_1_FAULT_WIRING_BRIEF.md` (read-first/brief-second/execute-third pivot, Anthropic `Edit` literal-mode semantics)
**Predecessor**: K6.1 (`fe03ed3..ab5a717`) — mod fault wiring end-to-end
**Target**: fresh feature branch `feat/k7-tick-loop-benchmark` from `main` after K6.1 closure
**Estimated time**: 4-6 hours auto-mode (3-5 days at hobby pace ~1h/day)
**Estimated LOC delta**: ~+700/-20 (new benchmark project files + report markdown; minimal source changes)

---

## Goal

Apply §8 metrics rule (GC pause / p99 / long-run drift on target hardware) to three TickLoopBenchmark variants — managed-current (pre-K4 baseline), managed-with-structs (post-K4 main), native-with-batching (NativeWorld + K1 batching + K5 WriteBatch) — and write `docs/PERFORMANCE_REPORT_K7.md` with raw CSV + per-variant analysis. Report informs K8 decision call (which Crystalka makes on report basis, not K7 executor).

K7 is **purely instrumentation + measurement + reporting**. It does not change production behavior. No Phase B (managed bootstrap) modifications; no scheduler changes; no system contract changes. The benchmark project is a leaf tests-adjacent project.

---

## Hardware target (LOCKED)

K7 measurements are run on **Crystalka's primary development machine** ("Skarlet"), the same hardware the game targets:

- **CPU**: AMD Ryzen 7 7435HS (Zen 3+, 8 cores / 16 threads, 3.10 GHz base)
- **RAM**: 32.0 GB DDR5-4800
- **GPU**: AMD Radeon RX 7600S (RDNA 3 mobile, 8 GB) — irrelevant for K7 (CPU-only), recorded for future G-series reference
- **Storage**: NVMe SSD (2.29 TB), measurements run from local disk
- **OS**: Windows 11 Home 25H2, build 26200.8246
- **Chassis**: ASUS TUF Gaming A16 (FA617NSR)

This is **not "weak hardware"** in the Docker-cpu-limit sense referenced in `KERNEL_ARCHITECTURE.md` §K7 design notes. K7 explicitly redefines the target: "the hardware Crystalka develops on, which represents median target audience hardware for a 2025-era release". §8 decision rule still applies — GC pause count, p99 tick time, long-run drift — but on actual target rather than synthetic weak target.

**Implication for K8**: outcomes evaluated against target-hardware numbers, not weak-hardware numbers. If native shows large absolute speedup but managed-with-structs already meets target frame budget on Skarlet, K8 Outcome 2 ("managed-with-structs alone wins") becomes plausible. Report wording reflects this.

**Future weak-hardware re-measurement**: deferred concern, recorded in MIGRATION_PROGRESS.md Open Questions if K8 doesn't conclusively settle the path. Not in K7 scope.

---

## Phase 0 — Pre-flight verification

Before any tool call, the executor verifies the working tree state, prerequisite milestone closures, and the assumptions this brief makes about code state.

### 0.1 — Working tree clean

```
git status
```

**Expected**: `nothing to commit, working tree clean` on branch `main`.

**Halt condition**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K7-WIP"` and re-verify, or commit them on the current branch before starting K7 work.

### 0.2 — Prerequisite milestones closed

```
git log --oneline -25
```

**Expected** most recent commits visible: K6.1 closure record `ab5a717` (or whichever SHA Cloud Code's last K6.1 commit had — the exact value is recorded in MIGRATION_PROGRESS.md K6.1 closure section). K6 closure SHAs (`cb3d6cf`..`d438222`) and K5 closure (`547c919`) also expected upstream of K6.1.

**Halt condition**: K6.1 not closed. K7 builds atop K6.1 baseline (553 tests, scheduler with real fault sink, BuildContext propagating origin/modId). Without K6.1, the benchmarks may differ in subtle ways from the post-K6.1 main reality and K7 numbers won't be representative.

### 0.3 — Prerequisite documents at expected versions

```
head -10 docs/KERNEL_ARCHITECTURE.md
head -10 docs/MIGRATION_PROGRESS.md
```

**Expected**:

- `KERNEL_ARCHITECTURE.md` Status: AUTHORITATIVE LOCKED v1.1
- `MIGRATION_PROGRESS.md` Last updated: 2026-05-09 (K6.1 closure)

**Halt condition**: any spec at unexpected version. K7 implements against §K7 of the locked KERNEL spec; mismatch means the contract has shifted.

### 0.4 — Code state inventory

The executor performs the following verification reads to confirm K7 prerequisites are present.

**Inventory checks**:

| Prerequisite | File | Verify |
|---|---|---|
| `NativeWorld` exists with full K1/K2/K5 surface | `src/DualFrontier.Core.Interop/NativeWorld.cs` | `grep -n "AddComponents\|GetComponents\|AcquireSpan\|BeginBatch" <file>` returns ≥4 matches |
| `WriteBatch<T>` exists per K5 | `src/DualFrontier.Core.Interop/Marshalling/WriteBatch.cs` | `ls <file>` succeeds |
| `SpanLease<T>` exists per K1 | `src/DualFrontier.Core.Interop/Marshalling/SpanLease.cs` | `ls <file>` succeeds |
| `ComponentTypeRegistry` exists per K2 | `src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs` | `ls <file>` succeeds |
| Vanilla components are structs (post-K4) | `src/DualFrontier.Components/Shared/HealthComponent.cs` | `grep -c "public struct HealthComponent\|public class HealthComponent" <file>` — `struct` should match, `class` should not |
| Existing benchmark project exists per K1/K3 | `tests/DualFrontier.Core.Benchmarks/` (or `benchmarks/...`) | `ls <directory>` succeeds; project file present |
| ParallelSystemScheduler post-K6.1 ctor | `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` | `grep -n "IReadOnlyDictionary<SystemBase, SystemMetadata>" <file>` returns 1+ matches |
| K7 brief skeleton exists | `tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md` | `ls <file>` succeeds, status currently SKELETON |

**Halt condition**: any inventory check fails. K7 assumes K0–K6.1 deliverables present.

### 0.5 — Managed build clean

```
dotnet build
```

**Expected**: build succeeds without warnings or errors.

**Halt condition**: build failure. K7 starts from a known-good baseline.

### 0.6 — Managed test baseline

```
dotnet test
```

**Expected**: 553 tests passing (post-K6.1 baseline).

**Halt condition**: test failures or count differs by more than 5. K7 should not regress test count; the only allowed test-count change is +N if Phase 4 adds benchmark validation tests.

### 0.7 — BenchmarkDotNet dependency check

```
grep -rn "BenchmarkDotNet" tests/ benchmarks/ 2>/dev/null
```

**Expected**: matches in existing benchmark `.csproj` (per K1 NativeBulkAddBenchmark, K3 BootstrapTimeBenchmark). The package version recorded — K7 uses the same to avoid version drift.

**Halt condition**: BenchmarkDotNet absent. K7 expects K1/K3 benchmarks landed; absence indicates regression or different working tree.

### 0.8 — Pre-K4 commit identified for variant V1

K7 needs a separate solution copy at the pre-K4 state to measure the "managed-current" variant (classes, not structs). The pre-K4 closure commit per MIGRATION_PROGRESS.md K4 section is `2fc59d1`; the parent commit is the appropriate pre-K4 baseline.

```
git show 2fc59d1 --pretty=format:"%H %ad %s" --no-patch
git show 2fc59d1^ --pretty=format:"%H %ad %s" --no-patch
```

**Expected**: K4 closure commit at `2fc59d1`; parent commit is the immediate predecessor.

**Halt condition**: K4 closure SHA not at expected location. Recheck MIGRATION_PROGRESS.md K4 section for the actual closure SHA before proceeding.

---

## Phase 1 — Architectural design (read-only, no edits)

Before any code change, the executor reads this section as the design contract for K7. Decisions here are LOCKED by the brief author.

### 1.1 — The three variants

K7 measures three variants of the same logical workload — 50 pawns × full vanilla component set × 10,000 ticks at fixed 30 TPS — under three different storage/system implementations:

**Variant V1: managed-current** (pre-K4 state)
- Components: classes (managed reference types)
- Storage: managed `World` with class-based components
- Systems: managed, no batching
- Scheduler: `ParallelSystemScheduler` (managed)
- **Source**: separate working copy at pre-K4 commit (per Phase 0.8)
- **Purpose**: baseline for K8 Outcome 3 (all paths equivalent) — establishes "what does the code look like before any of K4/K5/K1/K2 work?"

**Variant V2: managed-with-structs** (post-K4, current `main`)
- Components: structs (per K-L3 Path α)
- Storage: managed `World` with struct components
- Systems: managed, no native batching, no native bridge
- Scheduler: `ParallelSystemScheduler` (managed)
- **Source**: current `main` HEAD at K7 brief execution time
- **Purpose**: validates K4 conversion value standalone; informs K8 Outcome 2 (managed-with-structs alone wins)

**Variant V3: native-with-batching** (post-K5, NativeWorld path)
- Components: structs (same as V2, K-L3)
- Storage: `NativeWorld` (C++ kernel via Interop)
- Systems: managed (per K-L6 — all systems are mods, scheduler stays managed); use `SpanLease<T>` for reads, `WriteBatch<T>` for writes
- Scheduler: `ParallelSystemScheduler` (managed) — unchanged from V2
- **Source**: current `main` HEAD; benchmark project switches storage backend via DI
- **Purpose**: informs K8 Outcome 1 (native + batching wins decisively)

The three variants share **the same workload**: same scenario (50 pawns spawned with same component set), same tick count (10,000), same delta (1/30 s), same RNG seed. Only the storage/access path differs. This is essential for §8 comparability.

### 1.2 — Workload definition (LOCKED scenario)

The benchmark scenario, identical across V1/V2/V3:

**Initial state at tick 0**:
- 50 pawns spawned at random positions on a 200×200 grid (RNG seed = 42)
- Each pawn carries the full vanilla component set (post-K4 list — see §1.5 for the exact list)
- 800 obstacles placed on the navgrid (RNG seed = 42)
- 150 food items, 50 water sources, 30 beds, 25 decorations placed (RNG seed = 43)

**Per-tick work**:
- Full vanilla system pipeline runs each tick: NeedsSystem, MoodSystem, JobSystem, ConsumeSystem, SleepSystem, ComfortAuraSystem, MovementSystem, PawnStateReporterSystem, InventorySystem, HaulSystem, ElectricGridSystem, ConverterSystem
- `delta = 1f/30f` (fixed-step, 30 TPS target)
- No external events injected; no mod loading/unloading
- No presentation bridge writes (benchmark uses a no-op `IPresentationBridge`)

**Total runtime**:
- 10,000 ticks
- At 30 TPS this is 333.3 simulated seconds
- Wall-clock duration depends on variant; expected range 5–60 seconds total per variant on Skarlet hardware

**Why these numbers**:
- 50 pawns: matches `GameBootstrap.InitialPawnCount` (the production scenario size)
- 10,000 ticks: enough for GC patterns, drift, and p99 to stabilize; not so long that runs are tedious
- Vanilla component set: real workload, not synthetic micro-benchmarks
- Same RNG seeds across variants: deterministic, reproducible, comparable

### 1.3 — Metrics collected per variant

For each variant, the benchmark collects:

**Per-tick wall-clock time** (10,000 samples):
- Stored as `long[] tickTimesNs` of size 10,000
- Recorded via `Stopwatch.GetTimestamp()` deltas (ticks → nanoseconds)
- Used to compute p50, p90, p95, p99, p99.9, max, mean, stddev

**GC metrics**:
- `GC.CollectionCount(0)`, `GC.CollectionCount(1)`, `GC.CollectionCount(2)` sampled at start and end
- Per-tick GC count delta: `long[] gcCountsPerTick` (sampled every 100 ticks for sanity, full per-tick at gen0/1/2 boundaries)
- `GC.GetTotalAllocatedBytes(precise: true)` at start and end → total bytes allocated over 10k ticks
- `GC.GetTotalPauseDuration()` at start and end (.NET 7+ API, sub-millisecond pauses) — total pause time over 10k ticks

**Drift metric**:
- Cumulative wall-clock time vs simulated time
- At fixed 30 TPS, ideal cumulative wall-clock ≤ simulated time × 1.0 = 333.3 s
- Drift = (actual cumulative wall-clock) - (simulated time) at end
- A run that drifts means simulation cannot keep up; positive drift = bad

**Process-level metrics** (snapshot at start and end of run):
- `Process.GetCurrentProcess().WorkingSet64` (memory footprint)
- `Process.GetCurrentProcess().PrivateMemorySize64`
- `Process.GetCurrentProcess().TotalProcessorTime`

### 1.4 — Mixed framework approach

K7 uses two complementary measurement frameworks:

**BenchmarkDotNet**: micro-benchmark for tick time only. Runs `ExecuteTick(delta)` in a tight loop with statistical rigor (warmup, multiple iterations, outlier detection, JIT settling). Configuration:
- Job: `ShortRunJob` (3 warmup × 5 measurement iterations) — full LongRunJob is overkill for K7 evidence base
- Diagnoser: `MemoryDiagnoser` (allocations, gen0/1/2 collections per op)
- Diagnoser: `ThreadingDiagnoser` (lock contentions if any)
- Diagnoser: `EventPipeProfiler` with `Profile.GcVerbose` (GC events captured)

**Custom Stopwatch loop** for the 10,000-tick scenario. BenchmarkDotNet is not designed for "run this exact 10,000-step deterministic scenario once and report drift" — it's for "run this small operation thousands of times to establish a statistical mean". The K7 long-run scenario is the latter; the K7 micro-tick scenario is the former.

The two frameworks are complementary, not redundant:
- BDN tells us "what does a single tick cost on average, with statistical confidence"
- Custom loop tells us "does a 10,000-tick run drift, how does GC behave over time, what's the cumulative allocation"

Both are needed for §8 evidence base.

### 1.5 — Vanilla component set inventory (for benchmark scenario)

The 50 pawns in V1/V2/V3 carry the same component set. For deterministic comparison, the set is enumerated here and matched in code:

**Per-pawn (always present)**:
- `HealthComponent`
- `PositionComponent`
- `RaceComponent`
- `NeedsComponent`
- `MindComponent`
- `JobComponent`
- `MovementComponent` (collection — post-K4 stays as class per Hybrid Path)
- `IdentityComponent` (string — post-K4 stays as class)

**Per-pawn (probabilistic)**:
- `SkillsComponent` (Dictionary — class, post-K4)
- `SocialComponent` (Dictionary — class, post-K4)
- `WeaponComponent` (struct, ~30% of pawns)
- `ArmorComponent` (struct, ~50% of pawns)
- `AmmoComponent` (struct, when has weapon)

**World items (factory output, separate entities)**:
- 150 entities with `ConsumableComponent` + `PositionComponent`
- 50 entities with `WaterSourceComponent` + `PositionComponent`
- 30 entities with `BedComponent` + `PositionComponent`
- 25 entities with `DecorativeAuraComponent` + `PositionComponent`

This list mirrors `GameBootstrap.cs` factory output. The benchmark scenario reuses `RandomPawnFactory` and `ItemFactory` directly to ensure parity.

### 1.6 — Variant V1 implementation strategy

V1 (pre-K4) is the awkward variant because the source code form for V1 doesn't exist on `main`. Three options were considered:

**Option a (rejected)**: Revert main to pre-K4 state, run V1, reapply K4. Touches main, pollutes history, risks breaking K5/K6/K6.1 work that built on K4.

**Option b (rejected)**: Maintain a long-lived branch at pre-K4. Branch must be kept up to date with non-K4 changes for fairness; over time this gets tangled.

**Option c (LOCKED)**: At K7 execution time, create a fresh worktree of the repo at the pre-K4 commit, copy the K7 benchmark project files into that worktree, build and run V1 measurements there, archive the CSV output. Worktree is disposable post-measurement.

**Option c implementation**:

```bash
# In K7 brief Phase 3.2 — V1 measurement
git worktree add -d ../df-prek4 2fc59d1^
cp -r tests/DualFrontier.Core.Benchmarks ../df-prek4/tests/
cd ../df-prek4
# adjust Benchmarks.csproj references for pre-K4 state if needed (no NativeWorld variant in V1, only V1 scenario class)
dotnet build -c Release
dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks -- --filter "*V1*"
# CSV output captured to bin/Release/.../BenchmarkDotNet.Artifacts/
cp -r BenchmarkDotNet.Artifacts/results /path/to/main/repo/docs/benchmarks/k7-v1-raw/
cd /path/to/main/repo
git worktree remove ../df-prek4
```

The **K7 benchmark project** designed in Phase 2 carries V1 / V2 / V3 scenario classes. V2 and V3 run from `main`. V1 runs from the worktree at pre-K4 — but only the V1 scenario class is exercised (V2/V3 reference K4+ APIs that don't exist at pre-K4).

For this to work, the V1 scenario class must:
- Avoid `unsafe` blocks against `unmanaged` constraint that would fail to compile pre-K4
- Reference only types that existed at pre-K4 commit
- Use the managed `World` API surface that worked pre-K4

Because the K7 brief is written **post-K4**, the K7 author may not remember every API delta. The Phase 3.2 instructions handle this defensively: if V1 fails to compile in the worktree, halt and escalate; do not improvise pre-K4 fixes.

### 1.7 — Project structure (LOCKED)

K7 adds files to one new benchmark project:

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/TickLoopBenchmark.cs` (NEW)

Contains BDN benchmark class with three benchmark methods (one per variant). Uses `[GlobalSetup]` to spawn pawns/items, `[Benchmark]` for per-tick measurement.

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/V1ManagedCurrentScenario.cs` (NEW)

V1 scenario harness — managed `World` + class components. Used by V1 worktree run (Phase 3.2).

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/V2ManagedStructsScenario.cs` (NEW)

V2 scenario harness — managed `World` + struct components. Used directly from main.

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/V3NativeBatchedScenario.cs` (NEW)

V3 scenario harness — `NativeWorld` + struct components + `SpanLease<T>` reads + `WriteBatch<T>` writes. Used directly from main.

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/LongRunDriftRunner.cs` (NEW)

Custom loop runner for the 10k-tick scenario. Stand-alone executable mode (CLI argument `--long-run V<n>` selects variant). Writes CSV to `docs/benchmarks/k7-long-run-<variant>-<timestamp>.csv`.

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/MetricsCollector.cs` (NEW)

Helper class — wraps GC/process metrics sampling. Used by both BDN benchmarks and long-run runner.

**File**: `tests/DualFrontier.Core.Benchmarks/Program.cs` (MODIFIED — currently has K1/K3 BDN entry; extend to dispatch K7 modes)

Adds CLI flags: `--bdn-tick V<n>` (run BDN tick-cost on variant n), `--long-run V<n>` (run 10k-tick custom loop).

**File**: `docs/PERFORMANCE_REPORT_K7.md` (NEW)

The actual K7 deliverable — written after measurements complete. See §1.9 for structure.

**File**: `docs/benchmarks/k7-bdn-V1.csv`, `k7-bdn-V2.csv`, `k7-bdn-V3.csv` (NEW)

BDN-output raw CSVs, copied from `BenchmarkDotNet.Artifacts/results/` per variant.

**File**: `docs/benchmarks/k7-long-run-V1.csv`, `k7-long-run-V2.csv`, `k7-long-run-V3.csv` (NEW)

Custom long-run CSVs (10,000 rows each, columns per §1.3).

**File**: `tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md` (MODIFIED — this brief; status SKELETON → AUTHORED → EXECUTED)

### 1.8 — Variant V3 system simplification

V3 (NativeWorld) is the most architecturally complex variant because the production system code is written against managed `World` API. K7 does NOT migrate the production systems to `NativeWorld` — that's K8 Outcome 1 work.

Instead, V3 uses **a small set of simulated systems** that mirror the production systems' read/write patterns but operate via `NativeWorld` directly. These are scenario-specific harnesses, not production code.

For example, `V3NeedsSystem`:
```csharp
internal sealed class V3NeedsSystem
{
    private readonly NativeWorld _world;
    private readonly uint _needsTypeId;

    public void Update(float delta)
    {
        using var batch = _world.BeginBatch<NeedsComponent>();
        using var lease = _world.AcquireSpan<NeedsComponent>(_needsTypeId);
        for (int i = 0; i < lease.Count; i++)
        {
            ref readonly NeedsComponent old = ref lease.Span[i];
            EntityId id = lease.Pairs[i];
            var updated = old; // struct copy
            updated.Hunger -= delta * 0.001f;
            updated.Thirst -= delta * 0.0015f;
            // ...
            batch.Update(id, updated);
        }
    }
}
```

The V3 system set mirrors the V2/main system set (NeedsSystem, MoodSystem, MovementSystem, etc.) function-for-function but uses `NativeWorld` access patterns.

**Why simulated systems**: V3 is measuring "what does the tick cost when storage is native and access is via SpanLease+WriteBatch?" — not "does the production system code run unchanged on NativeWorld?". The latter is K8 work. The former is what K8 needs evidence for.

**Trade-off acknowledged**: V3 numbers are slightly optimistic vs production-on-NativeWorld because the V3 system code is purpose-written for the access pattern. K8 cutover work may show some friction. The report explicitly notes this caveat.

### 1.9 — PERFORMANCE_REPORT_K7.md structure (LOCKED template)

The report is written **after measurements** as the final K7 deliverable. Structure:

```markdown
# K7 Performance Report — Tick-loop measurements

**Status**: FINAL
**Date**: 2026-MM-DD
**Author**: K7 brief execution session
**Hardware**: AMD Ryzen 7 7435HS, 32 GB DDR5-4800, Win11 25H2 (Skarlet)
**Reference**: `docs/KERNEL_ARCHITECTURE.md` §K7, K8 evidence base

## Executive summary

[3-5 sentence summary: which variant won on which metric; whether §8 rule decisively favors one variant; recommended K8 outcome direction.]

## Methodology

- Workload: 50 pawns + 255 items, 10k ticks @ 30 TPS, fixed seeds (42 for nav/pawns, 43 for items)
- Three variants per §1.1 of K7 brief
- Metrics: per-tick time (BDN), GC count/duration/allocations (BDN + long-run), drift (long-run)
- Frameworks: BenchmarkDotNet 0.13.x for tick cost; custom Stopwatch loop for long-run

## Variant V1 — managed-current (pre-K4)

[Source: pre-K4 worktree at commit `2fc59d1^`]

### BDN tick cost
| Metric | Value |
|---|---|
| Mean | XX.X μs |
| StdDev | XX.X μs |
| p50 | XX.X μs |
| p99 | XX.X μs |
| Allocations/op | XX.X B |
| Gen0/op | X.XX |

### Long-run 10k ticks
| Metric | Value |
|---|---|
| Total wall-clock | XX.X s |
| Drift vs simulated | +X.X s (or -X.X s) |
| Total allocated | XX MB |
| Gen0 collections | XX |
| Gen1 collections | XX |
| Gen2 collections | XX |
| GC pause total | XX ms |

[Per-tick time series chart description — referenced from CSV]

## Variant V2 — managed-with-structs (post-K4)

[Same metric tables]

## Variant V3 — native-with-batching

[Same metric tables, with footnote on V3 system harnesses being purpose-written]

## Comparative analysis

### Per-metric ranking

| Metric | V1 | V2 | V3 | Winner | Margin |
|---|---|---|---|---|---|
| Mean tick time | ... | ... | ... | ... | ... |
| p99 tick time | ... | ... | ... | ... | ... |
| Total allocations | ... | ... | ... | ... | ... |
| Gen0 collections | ... | ... | ... | ... | ... |
| GC pause total | ... | ... | ... | ... | ... |
| Drift @ 10k | ... | ... | ... | ... | ... |

### §8 rule application

Per `KERNEL_ARCHITECTURE.md` §8 / K-L10: decision rule is **GC pause / p99 / long-run drift on target hardware**.

- GC pause: [V1 vs V2 vs V3 comparison; which decisively wins]
- p99 tick: [comparison; if V2 already meets 16ms / 33ms / target frame budget, K8 Outcome 2 plausible]
- Drift: [comparison; if any variant drifts positive, that's a 30 TPS sustainability concern]

### Recommended K8 outcome direction

[One of:]
- **Outcome 1 (native + batching wins decisively)**: V3 dominates on all three §8 metrics by margin > 30%. Production cutover to `NativeWorld` is justified.
- **Outcome 2 (managed-with-structs alone wins)**: V2 meets target frame budget with margin; V3 advantage on absolute terms is < 30% or comes with K8 cutover cost not justified by margin. Park native, keep managed-with-structs.
- **Outcome 3 (all paths equivalent)**: V1, V2, V3 differ by < 10% on §8 metrics. K4 conversion delivered the structural improvement; native is not adding measurable value at this workload.

The recommendation is **advisory only**. Crystalka makes the K8 decision call.

## Caveats and limitations

- V3 system harnesses are purpose-written for NativeWorld access patterns; production system code on NativeWorld may show different numbers (K8 work).
- Hardware target is "median 2025 gaming laptop" not "weak hardware" — future weak-hardware re-measurement may shift conclusions if game ships with broader minimum spec.
- 50-pawn workload is the production scenario size; scaling to 200+ pawns may stress the variants differently.
- Single-machine measurement; no cross-hardware variance data.

## Raw data

- BDN CSV exports: `docs/benchmarks/k7-bdn-V{1,2,3}.csv`
- Long-run CSV exports: `docs/benchmarks/k7-long-run-V{1,2,3}.csv`
- BDN HTML reports (full statistical detail): `docs/benchmarks/k7-bdn-V{1,2,3}-report.html`
```

The template is filled in by the K7 executor after measurements complete. Numbers and analysis are honest reads of the data; the recommendation in §"Recommended K8 outcome direction" is a single sentence pointing at the most-likely outcome based on numbers, not advocacy.

---

## Phase 2 — Benchmark project scaffolding

### 2.1 — Verify benchmark project location and namespace

```
ls tests/DualFrontier.Core.Benchmarks/
cat tests/DualFrontier.Core.Benchmarks/DualFrontier.Core.Benchmarks.csproj
```

**Expected**: project exists with K1/K3 benchmarks already present.

**Edit decision**: K7 adds files under `tests/DualFrontier.Core.Benchmarks/TickLoop/` subfolder. No new csproj file; existing one references the new files via glob.

### 2.2 — Create `MetricsCollector.cs`

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/MetricsCollector.cs` (NEW)

**Content**: helper class with static methods:
- `static GcSnapshot CaptureGc()` — captures `GC.CollectionCount(0/1/2)`, `GC.GetTotalAllocatedBytes(true)`, `GC.GetTotalPauseDuration()`
- `static GcDelta Diff(GcSnapshot start, GcSnapshot end)` — returns deltas
- `static ProcessSnapshot CaptureProcess()` — captures WorkingSet64, PrivateMemorySize64, TotalProcessorTime
- `record GcSnapshot(int Gen0, int Gen1, int Gen2, long AllocatedBytes, TimeSpan PauseDuration)`
- `record GcDelta(int Gen0Collections, int Gen1Collections, int Gen2Collections, long AllocatedBytes, TimeSpan PauseDuration)`
- `record ProcessSnapshot(long WorkingSet, long PrivateMemory, TimeSpan TotalProcessorTime)`

**Atomic commit**:
```
feat(benchmarks): add MetricsCollector helper for K7 GC/process sampling
```

### 2.3 — Create scenario base class

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/TickLoopScenarioBase.cs` (NEW)

**Content**: abstract base class with:
- `abstract void SetupWorld(int pawnCount, int seed)` — spawns the workload
- `abstract void ExecuteTick(float delta)` — runs one tick
- `abstract void TeardownWorld()` — disposes resources (NativeWorld, etc.)

Each variant (V1/V2/V3) inherits and implements per its storage backend.

**Atomic commit**:
```
feat(benchmarks): add TickLoopScenarioBase abstract for V1/V2/V3 variants
```

### 2.4 — Create V2 scenario (post-K4 main, simplest case first)

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/V2ManagedStructsScenario.cs` (NEW)

V2 is implemented first because it can be tested against current main without worktree gymnastics. Once V2 works, the scaffold is reused for V1 and V3.

**Content**: concrete `TickLoopScenarioBase` impl using `World` (managed) + `ParallelSystemScheduler` + the production system set (NeedsSystem, MoodSystem, etc.). The benchmark spawns pawns/items via `RandomPawnFactory` / `ItemFactory` (reused from production). Tick = `_scheduler.ExecuteTick(delta)`.

Note: the scheduler ctor post-K6.1 takes non-optional `IModFaultSink` and a `IReadOnlyDictionary<SystemBase, SystemMetadata>`. V2 uses `new NullModFaultSink()` and an empty metadata dict (all benchmark systems are origin Core).

**Atomic commit**:
```
feat(benchmarks): add V2ManagedStructsScenario for K7 post-K4 baseline
```

### 2.5 — Create V3 scenario (NativeWorld + batching)

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/V3NativeBatchedScenario.cs` (NEW)

V3 uses `NativeWorld` directly. Per §1.8, the system set is purpose-written: `V3NeedsSystem`, `V3MovementSystem`, etc. These mirror the production systems' read/write patterns but use `SpanLease` + `WriteBatch`.

The V3 scenario file is larger than V2 because it contains the full simulated system set. Estimated: ~400 lines.

**Atomic commit**:
```
feat(benchmarks): add V3NativeBatchedScenario for K7 native+batching variant
```

### 2.6 — Create V1 scenario (pre-K4 worktree-runnable)

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/V1ManagedCurrentScenario.cs` (NEW)

V1 is written **last** because it must be careful about API surfaces. The V1 scenario class:
- References only types that exist at pre-K4 commit (verified by trying to compile in worktree later)
- Uses managed `World` + class-based components
- Uses the same factories — but note: factories pre-K4 may have different signatures. The scenario reads pre-K4 factory APIs and adapts.

**Defensive design**: V1 scenario class uses **conditional compilation** to gate against missing APIs. If `#if !NET_K4_LANDED` (a custom symbol set by the V1 worktree's csproj), the V1 scenario uses pre-K4 factory signatures; otherwise, it's a no-op.

This is fragile. The clean alternative: V1 scenario class lives **only in the worktree**, copied there manually during Phase 3.2. The K7 brief takes this approach: V1 file exists in the main repo as a stub, gets copied to worktree and replaced with a real implementation written there.

**LOCKED choice**: V1 scenario lives in main as documented stub; real V1 implementation written in worktree at Phase 3.2. The stub explains how to use it.

**Atomic commit**:
```
feat(benchmarks): add V1ManagedCurrentScenario stub for pre-K4 worktree run
```

### 2.7 — Create BDN benchmark class

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/TickLoopBenchmark.cs` (NEW)

**Content**: BDN benchmark class with `[Benchmark]` methods:
- `[Benchmark] public void TickV2_ManagedStructs()` — runs V2 single tick
- `[Benchmark] public void TickV3_NativeBatched()` — runs V3 single tick
- `[GlobalSetup]` — spawns workload via the scenario class for the variant under test
- `[GlobalCleanup]` — tears down

BDN config:
- `[ShortRunJob]` (3 warmup, 5 iterations)
- `[MemoryDiagnoser]`
- `[ThreadingDiagnoser]`
- `[EventPipeProfiler(Profile.GcVerbose)]`

V1 is **not** in this BDN class because BDN needs to be run from the worktree where V1 is implemented. The worktree gets a parallel `TickV1_ManagedCurrent` BDN method added during Phase 3.2.

**Atomic commit**:
```
feat(benchmarks): add TickLoopBenchmark BDN harness for V2 + V3
```

### 2.8 — Create LongRunDriftRunner

**File**: `tests/DualFrontier.Core.Benchmarks/TickLoop/LongRunDriftRunner.cs` (NEW)

**Content**: stand-alone runner method:
```csharp
public static void Run(string variant, int tickCount, string outputPath)
{
    TickLoopScenarioBase scenario = variant switch
    {
        "V2" => new V2ManagedStructsScenario(),
        "V3" => new V3NativeBatchedScenario(),
        _    => throw new ArgumentException(...)
    };
    scenario.SetupWorld(pawnCount: 50, seed: 42);

    var tickTimes = new long[tickCount];
    var gcStart = MetricsCollector.CaptureGc();
    var procStart = MetricsCollector.CaptureProcess();
    var swTotal = Stopwatch.StartNew();

    for (int i = 0; i < tickCount; i++)
    {
        long t0 = Stopwatch.GetTimestamp();
        scenario.ExecuteTick(1f / 30f);
        tickTimes[i] = Stopwatch.GetTimestamp() - t0;

        if (i % 100 == 0)
        {
            // sample GC count for time-series
            // (optional — could write to CSV)
        }
    }

    swTotal.Stop();
    var gcEnd = MetricsCollector.CaptureGc();
    var procEnd = MetricsCollector.CaptureProcess();

    WriteCsv(outputPath, tickTimes, gcStart, gcEnd, procStart, procEnd, swTotal.ElapsedMilliseconds);
    scenario.TeardownWorld();
}
```

V1 long-run is not in this runner for the same reason as BDN; runs from worktree.

**Atomic commit**:
```
feat(benchmarks): add LongRunDriftRunner for K7 10k-tick scenario (V2/V3)
```

### 2.9 — Update Program.cs CLI

**File**: `tests/DualFrontier.Core.Benchmarks/Program.cs` (MODIFIED)

Add CLI flag dispatcher:
- `--bdn-tick V<n>` → BDN tick benchmark
- `--long-run V<n>` → custom long-run loop
- existing flags preserved

**Atomic commit**:
```
feat(benchmarks): extend Program.cs with K7 BDN + long-run dispatch
```

### 2.10 — Build + sanity test

```
dotnet build tests/DualFrontier.Core.Benchmarks/
```

**Expected**: build clean, 0 warnings.

**Halt condition**: build failure. Diagnose against the new files; do not weaken the existing benchmark project.

---

## Phase 3 — Run measurements

This phase produces CSV output. No code edits to source projects.

### 3.1 — V2 measurements (post-K4 main)

Run BDN tick benchmark for V2:
```
dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks -- --bdn-tick V2
```

**Expected output**: BenchmarkDotNet artifacts directory populated. Copy CSV + HTML reports to `docs/benchmarks/`:
```
mkdir -p docs/benchmarks
cp -v BenchmarkDotNet.Artifacts/results/*V2*.csv docs/benchmarks/k7-bdn-V2.csv
cp -v BenchmarkDotNet.Artifacts/results/*V2*.html docs/benchmarks/k7-bdn-V2-report.html
```

Run long-run for V2:
```
dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks -- --long-run V2
```

**Expected output**: `docs/benchmarks/k7-long-run-V2.csv` (10,000 rows + header).

**Halt condition**: any run fails or output file empty. Diagnose before continuing.

### 3.2 — V3 measurements

Same as V2 but with `V3` flag:
```
dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks -- --bdn-tick V3
dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks -- --long-run V3
```

Copy outputs:
```
cp -v BenchmarkDotNet.Artifacts/results/*V3*.csv docs/benchmarks/k7-bdn-V3.csv
cp -v BenchmarkDotNet.Artifacts/results/*V3*.html docs/benchmarks/k7-bdn-V3-report.html
```

(Long-run V3 CSV already at `docs/benchmarks/k7-long-run-V3.csv` per LongRunDriftRunner.)

**Halt condition**: NativeWorld initialization or P/Invoke errors. Diagnose against K1/K5 deliverables; do not patch around.

### 3.3 — V1 measurements (pre-K4 worktree)

This is the most fragile phase. Execute carefully.

**Step 1**: create worktree at pre-K4:
```
cd D:\Colony_Simulator\Colony_Simulator
git worktree add -d ../df-prek4 2fc59d1^
```

**Step 2**: copy K7 benchmark project files to worktree:
```
robocopy tests\DualFrontier.Core.Benchmarks ..\df-prek4\tests\DualFrontier.Core.Benchmarks /E /XO
```

**Step 3**: in the worktree, V1 scenario stub needs to be replaced with real V1 impl. The executor reads pre-K4 codebase to understand:
- What does `HealthComponent` look like (class, with what API)?
- What does `RandomPawnFactory` signature look like?
- Does `World.AddComponent<T>` work the same way?

The executor writes the real V1 scenario class **inside the worktree**, referencing only pre-K4 APIs. Estimated time: 30-60 min (this is the brief's longest single task).

**Step 4**: build worktree:
```
cd ..\df-prek4
dotnet build -c Release tests\DualFrontier.Core.Benchmarks
```

**Halt condition**: build failures in worktree that aren't trivially fixable (renamed type, removed API). Resolution: stash V1 work, **change V1 scenario to a simpler proxy** that exercises the pre-K4 hot path on managed `World` directly without the full vanilla system set. The simpler V1 still answers "what's the per-tick cost of class-based components in managed World" even if it skips some systems. Document the divergence in the report.

**Step 5**: run V1 benchmarks in worktree:
```
dotnet run -c Release --project tests\DualFrontier.Core.Benchmarks -- --bdn-tick V1
dotnet run -c Release --project tests\DualFrontier.Core.Benchmarks -- --long-run V1
```

**Step 6**: copy outputs from worktree to main repo:
```
xcopy ..\df-prek4\BenchmarkDotNet.Artifacts\results\*V1*.csv D:\Colony_Simulator\Colony_Simulator\docs\benchmarks\k7-bdn-V1.csv /Y
xcopy ..\df-prek4\BenchmarkDotNet.Artifacts\results\*V1*.html D:\Colony_Simulator\Colony_Simulator\docs\benchmarks\k7-bdn-V1-report.html /Y
xcopy ..\df-prek4\docs\benchmarks\k7-long-run-V1.csv D:\Colony_Simulator\Colony_Simulator\docs\benchmarks\k7-long-run-V1.csv /Y
```

**Step 7**: clean up worktree:
```
cd D:\Colony_Simulator\Colony_Simulator
git worktree remove ..\df-prek4 --force
```

**Atomic commit at end of Phase 3** (single commit for all 3 variants' raw data):
```
docs(benchmarks): K7 raw measurements — V1, V2, V3 BDN + long-run CSVs
```

---

## Phase 4 — Analysis and report

### 4.1 — Read CSVs into analysis

The executor reads the 6 CSVs (3 BDN + 3 long-run) and computes per-variant metrics:
- p50, p90, p95, p99, p99.9, max, mean, stddev of per-tick time
- Total wall-clock for 10k ticks
- Drift = total wall-clock - 333.3 s
- GC counts gen0/1/2 (delta over run)
- Total allocated bytes (delta)
- Total GC pause duration (delta)

Computation can be done with a Python script, an Excel pivot, or by hand from the BDN HTML report. Recommended: small `tools/analyze_k7.py` script (or `.cs` console runner) reading CSVs and emitting a markdown table.

### 4.2 — Write PERFORMANCE_REPORT_K7.md

**File**: `docs/PERFORMANCE_REPORT_K7.md` (NEW)

Fill the template from §1.9 with actual numbers. Be honest:
- If V3 doesn't dominate, say so
- If V1 is faster than expected (unlikely), investigate before reporting
- If a variant's drift is positive, explicitly call out 30 TPS sustainability concern

**Atomic commit**:
```
docs(performance): K7 report — tick-loop measurements V1/V2/V3 with §8 analysis
```

### 4.3 — Update MIGRATION_PROGRESS.md

**File**: `docs/MIGRATION_PROGRESS.md`

**Edit 1**: K-series Overview table, K7 row:

`| K7 | Performance measurement (tick-loop) | NOT STARTED | 3-5 days | — | — |`

→

`| K7 | Performance measurement (tick-loop) | DONE | <commit SHA range> | <date> |`

**Edit 2**: Add K7 closure section after K6.1:

```markdown
### K7 — Performance measurement (tick-loop)

- **Status**: DONE (`<commit SHA range>`, <date>)
- **Brief**: `tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md` (FULL EXECUTED)
- **Hardware**: AMD Ryzen 7 7435HS, 32 GB DDR5-4800, Win11 25H2 ("Skarlet" — Crystalka's primary dev machine, treated as median target audience hardware per K7 brief Phase 1)
- **Variants measured**: V1 managed-current (pre-K4 worktree), V2 managed-with-structs (main), V3 native-with-batching (main + NativeWorld)
- **Workload**: 50 pawns × full vanilla component set × 10,000 ticks @ 30 TPS, fixed seeds
- **Metrics**: BDN tick cost (mean / p50 / p99 / allocations / gen0-2) + long-run cumulative (drift / GC count / GC pause / allocations)
- **Frameworks**: BenchmarkDotNet 0.13.x for tick cost; custom Stopwatch loop for 10k-tick scenario
- **Report**: `docs/PERFORMANCE_REPORT_K7.md`
- **Raw data**: `docs/benchmarks/k7-bdn-V{1,2,3}.csv`, `docs/benchmarks/k7-long-run-V{1,2,3}.csv`
- **Recommended K8 outcome direction (per report §"Recommended K8 outcome direction")**: <fill in based on actual numbers; one of Outcome 1 / 2 / 3>. Crystalka makes the K8 decision call on report basis.
- **Lessons learned**:
  - <fill in based on what was actually surprising during measurement>
  - V1 worktree approach worked; pre-K4 reconstruction took <X> minutes per Phase 3.3
  - V3 system harness divergence from production system code: <noted in report; may need K8 follow-up to confirm production-system numbers match>
```

**Edit 3**: Update `Current state snapshot`:
- `Active phase`: K7 → K8 (recommendation pending Crystalka decision per report)
- `Last completed milestone`: K6.1 → K7
- `Tests passing`: 553 → <new count if any benchmarks-as-tests added; likely unchanged>
- `Last updated`: 2026-05-09 (K6.1 closure) → <new date> (K7 closure)

**Atomic commit**:
```
docs(migration): K7 closure recorded — performance measurement complete
```

### 4.4 — Mark brief EXECUTED

**File**: `tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md`

Status line: `AUTHORED` → `EXECUTED (<date>, branch <branch>, closure <commit SHA range>)`. Add link to MIGRATION_PROGRESS.md K7 closure section.

**Atomic commit** (bundled with §4.3):
```
docs(briefs): mark K7 brief as EXECUTED with closure refs
```

---

## Phase 5 — Final verification

### 5.1 — Build + test gate

```
dotnet build
dotnet test
```

**Expected**: 0 errors, 0 warnings, 553 tests passing (or 553+ if any benchmark-validation tests added).

**Halt condition**: any regression. K7 must not break test count; benchmarks are separate from `dotnet test`.

### 5.2 — Pre-commit grep

```
grep -rn "TODO\|FIXME\|XXX" tests/DualFrontier.Core.Benchmarks/TickLoop/
```

**Expected**: 0 matches in K7-introduced files.

### 5.3 — Verify all 6 CSVs and report present

```
ls docs/benchmarks/k7-bdn-V*.csv
ls docs/benchmarks/k7-long-run-V*.csv
ls docs/PERFORMANCE_REPORT_K7.md
```

**Expected**: 3 BDN CSVs + 3 long-run CSVs + report present.

**Halt condition**: any file missing. Phase 3 / Phase 4 incomplete.

---

## Atomic commit log expected

Approximate commit count: **12-14**:

**Phase 2 (8 commits)**:
1. `feat(benchmarks): add MetricsCollector helper for K7 GC/process sampling`
2. `feat(benchmarks): add TickLoopScenarioBase abstract for V1/V2/V3 variants`
3. `feat(benchmarks): add V2ManagedStructsScenario for K7 post-K4 baseline`
4. `feat(benchmarks): add V3NativeBatchedScenario for K7 native+batching variant`
5. `feat(benchmarks): add V1ManagedCurrentScenario stub for pre-K4 worktree run`
6. `feat(benchmarks): add TickLoopBenchmark BDN harness for V2 + V3`
7. `feat(benchmarks): add LongRunDriftRunner for K7 10k-tick scenario (V2/V3)`
8. `feat(benchmarks): extend Program.cs with K7 BDN + long-run dispatch`

**Phase 3 (1 commit, bundled)**:
9. `docs(benchmarks): K7 raw measurements — V1, V2, V3 BDN + long-run CSVs`

**Phase 4 (3 commits)**:
10. `docs(performance): K7 report — tick-loop measurements V1/V2/V3 with §8 analysis`
11. `docs(migration): K7 closure recorded — performance measurement complete`
12. `docs(briefs): mark K7 brief as EXECUTED with closure refs`

**Phase 5**: no commits (verification only).

---

## Cross-cutting design constraints

1. **No production-source changes**. K7 is measurement-only; the benchmark project is the only target for new files. Any edit to `src/` (other than maybe a benchmark-only `internal` accessor exposure) is a brief violation — halt.

2. **Same workload across variants**. V1, V2, V3 must run the same scenario (50 pawns, same seeds, 10k ticks, same delta, same component set). Any divergence required by API drift between pre-K4 and post-K4 must be explicitly documented in the report's caveats section.

3. **Same hardware across variants**. Run all measurements in a single session, on the same machine, without machine restarts in between. Background processes minimized (close Discord, browser, etc.). Run with laptop plugged in (no battery throttling).

4. **Atomic commits per logical change**. One commit per Phase sub-step. Each commit must build cleanly; benchmark runs are not gated by `dotnet test` (no test count change).

5. **Do not auto-decide K8**. The report's "Recommended K8 outcome direction" section points at one outcome based on numbers; the K8 decision call is Crystalka's. The K7 executor does NOT promote any variant to production, does NOT delete deprecated managed paths, does NOT change `GameBootstrap.CreateLoop`. K8 work, not K7.

6. **Variant V1 is allowed to use a simpler workload** if pre-K4 API drift makes the full vanilla system set non-portable. The simpler workload must still exercise managed `World` + class-based components on the same scenario size (50 pawns). Document the divergence explicitly.

7. **Pre-flight grep discipline (AD #4)**. Phase 0.4 inventory is structured grep verification. Phase 5.2 final pre-commit grep verifies no TODO leaks in benchmark code.

8. **No regex metacharacters in `Edit` tool boundaries** (per `MOD_OS_V16_AMENDMENT_CLOSURE.md`). All `oldText` / `newText` payloads are plain prose / code without regex metacharacters at boundary positions.

9. **«Data exists or it doesn't»**. CSVs either have all expected rows (10,000 + header) or the run failed and is re-run. No partial CSV gets analyzed.

10. **§8 rule application is honest**. Recommended outcome direction in report follows from numbers, not from prior expectation. If V3 disappoints, report says so. If V2 already meets target with margin, report says so.

---

## Stop conditions

The executor halts and escalates the brief authoring session if any of the following:

1. Phase 0 pre-flight check fails — working tree dirty, prerequisites missing, specs at unexpected version, baseline build/test fails.

2. Phase 0.4 inventory diverges — NativeWorld surface incomplete, K4 components still classes, K6.1 scheduler signature absent.

3. Phase 0.8 K4 closure SHA at unexpected location.

4. Phase 2 build fails after any commit. Each commit must build clean.

5. Phase 3 V2 or V3 measurement run fails (BDN crashes, NativeWorld P/Invoke errors, output empty).

6. Phase 3 V1 worktree creation or pre-K4 build fails irrecoverably (substantive API drift). Resolution per Phase 3.3 step 4: simplify V1 workload, document divergence. If even simplified V1 won't compile, halt.

7. Phase 4 analysis reveals one variant is suspiciously fast or slow (>3x outside expected range). Re-measure once; if confirmed, document; if cannot confirm, halt.

8. Phase 4 report wording would advocate for a K8 outcome the numbers don't support. Halt — wording must be honest.

9. Native kernel files modified during execution. K7 is managed-only.

10. Production source files modified. K7 is benchmarks-only.

The fallback in every halt case is `git stash push -m "k7-WIP-halt-$(date +%s)"` and report to the brief author.

---

## Brief authoring lineage

- **2026-05-09** — K6.1 closed (`fe03ed3..ab5a717`), K7 brief authoring triggered per skeleton's "Brief authoring trigger: after K6 closure" (post-K6.1 since K6.1 is logical extension of K6). Author: Opus architect session per «доки сначала, миграция потом» pivot continuation. K7 designed as pure measurement milestone — no production behavior changes, no scheduler edits, no system contract changes.
- **(date TBD)** — Executed and closed at K7 milestone closure.

The brief was authored read-first / brief-second per the methodology pivot recorded in `MOD_OS_V16_AMENDMENT_CLOSURE.md`. Source documents read during authoring: `KERNEL_ARCHITECTURE.md` v1.1 LOCKED §K7 + Part 0 K-L10 + Part 4 §6 vs §8 reconciliation, `MIGRATION_PROGRESS.md` (K6.1 closure section, K4/K5 closure history), existing K5 brief (format reference), hardware screenshot from Crystalka (target hardware spec).

---

## Methodology note

K7 is the **first measurement-only milestone** in the K-series. It produces evidence rather than code. The brief format adapts:

- Phase 1 (architectural design) is shorter than implementation briefs because there's no class hierarchy to design — only scenario shapes and metric definitions.
- Phase 3 (measurement runs) is unique to K7: the deliverable is CSV data, not code; the work is "run benchmarks correctly" not "edit files correctly".
- Phase 4 (analysis + report) is the K7-distinctive phase. The report's "Recommended K8 outcome direction" is the closest K7 comes to a code-like decision, and even that is advisory.

The K6 → K6.1 pattern (closure-shaped brief surfaces deferred gap → focused follow-up) does not apply to K7. K7 is a single coherent milestone; if measurements reveal an unexpected concern (e.g., V3 has a hidden allocation pathway), that's K8 cutover work or a future K-series amendment, not a K7.1 follow-up.

The K7 brief's "Recommended K8 outcome direction" advisory output is the bridge to K8. Crystalka reads the K7 report, makes the K8 outcome call, and the K8 brief is authored against that decision (one of three K8 brief variants per Outcome 1 / 2 / 3).

---

**Brief end.** Awaits Crystalka's review and feed to Claude Code session for execution.
