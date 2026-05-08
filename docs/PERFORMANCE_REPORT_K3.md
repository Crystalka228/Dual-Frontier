# Performance Report — post-K3 snapshot

**Status**: LIVE document (early K7 partial preview)
**Created**: 2026-05-07 (post-K3 closure)
**Branch reference**: K3 closed at `7629f57`, this measurement taken on `feat/k3-bootstrap-graph` after merge to local main
**Companion documents**: `KERNEL_ARCHITECTURE.md` (LOCKED v1.0), `MIGRATION_PROGRESS.md`, `CPP_KERNEL_BRANCH_REPORT.md` (Discovery, ~3 hours earlier same day)

---

## Purpose

This document captures **quantitative measurements** of native vs managed paths after K0+K1+K2+K3 milestones, before K4 (component struct refactor) marathon begins.

Это **partial preview of K7** — full K7 milestone будет применять §8 metrics (p99, GC pause, drift, weak hardware) для tick-loop end-to-end measurement. Этот документ ограничен subset:

- **In scope**: NativeVsManagedBenchmark (Discovery baseline reproduction), planned K1 bulk path measurement
- **Out of scope** (deferred к K7): tick-loop measurement, p99/median ratio analysis, GC pause distribution, weak-hardware (CPU-limited container) numbers, post-K4 struct refactor comparison

**Decision support**: K7 будет authoritative for K8 cutover decision. Этот document — early signal: validate что K1 batching infrastructure delivers measurable gains, motivate (or re-evaluate) K4 marathon.

---

## Test environment

**Hardware**:
- Windows 11 (build 10.0.26200.8246)
- CPU: «Unknown processor» per BenchmarkDotNet self-detection (Crystalka's main development machine)
- BenchmarkDotNet v0.13.12

**Software**:
- .NET SDK 10.0.202
- Runtime: .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX2
- GC: Concurrent Workstation
- Build configuration: Release

**Pre-run hygiene**:
- Visual Studio closed
- Godot game not running
- No background dotnet test processes

**State of project at measurement**:
- All K0-K3 commits merged to local `main` (also visible as `feat/k3-bootstrap-graph` PR)
- 472 managed tests passing (76 Core + 4 Persistence + 45 Interop + 347 Modding)
- 12/12 native selftest scenarios passing
- 18 functions in C ABI (post-K3)

---

## Measurement 1 — NativeVsManagedBenchmark (full BenchmarkDotNet)

### Run command
```powershell
cd D:\Colony_Simulator\Colony_Simulator\tests\DualFrontier.Core.Benchmarks
dotnet run -c Release -- --full
```

### Results

| Method            | Mean     | Error   | StdDev   | Ratio | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
|------------------ |---------:|--------:|---------:|------:|--------:|--------:|--------:|--------:|----------:|------------:|
| ManagedSumCurrent | 102.6 µs | 0.97 µs |  0.81 µs |  1.00 |    0.00 |       - |       - |       - |         - |          NA |
| NativeSumCurrent  | 256.6 µs | 3.83 µs |  4.10 µs |  2.51 |    0.05 |       - |       - |       - |         - |          NA |
| ManagedAdd10k     | 205.1 µs | 1.97 µs |  1.64 µs |  2.00 |    0.02 | 83.0078 | 41.5039 | 41.5039 | 655,606 B |          NA |
| NativeAdd10k      | 404.1 µs | 8.08 µs | 23.43 µs |  4.08 |    0.16 |       - |       - |       - |      32 B |          NA |

**Run time**: 2 min 32 sec
**Outliers removed**: 9 total (multimodal distribution warning on NativeAdd10k, mValue=2.92)

### Comparison vs Discovery report

Discovery report (`docs/CPP_KERNEL_BRANCH_REPORT.md` §10.4, measured ~3 hours earlier same day on experimental branch):

| Method | Discovery | Post-K3 | Δ |
|---|---:|---:|---|
| ManagedAdd10k | 218 µs | 205.1 µs | -6% |
| NativeAdd10k  | 399 µs | 404.1 µs | +1% |

**Numbers are stable**: K0 cherry-pick + K1 batching infra + K2 registry + K3 bootstrap graph **did not regress and did not improve** the per-entity native path. K1 bulk path is **separate** (different ABI functions), measured next milestone.

### Key observations

#### 1. Per-entity overhead — quantified

NativeAdd10k = 404 µs, ManagedAdd10k = 205 µs. **Native medlee managed by 2x на per-entity Add**.

Per-entity P/Invoke crossing cost ≈ (404-205)/10000 = **~20ns per crossing**. На реальной игровой нагрузке (50 pawns × 30 TPS × ~100 components × ~7 phases) это ≈ 1M crossings/sec ≈ 20 ms/sec overhead. **Это эквивалент 2 frames при 60 FPS, потерянных только на marshalling.**

Это и есть мотивация K1 bulk path: single P/Invoke per batch eliminates this overhead.

#### 2. GC pressure — структурное преимущество native

ManagedAdd10k allocates **655,606 B** (≈64 KB) per Add cycle, with collections в Gen0/Gen1/Gen2. NativeAdd10k allocates **32 B** total — 20,000x reduction.

На long-run workload это означает:
- Managed: continuous GC pressure → unpredictable pauses → p99 latency spikes
- Native: near-zero GC pressure → stable latency profile

Это applies §8 metrics rule: **«mean throughput is half the story; p99 latency и tail behavior matter for game tick consistency»**. Native path **structurally** addresses tail behavior через elimination of allocation churn.

#### 3. Sum path — managed wins на dense iteration

NativeSumCurrent = 256.6 µs vs ManagedSumCurrent = 102.6 µs. **Native в 2.5x medlee на iterate-and-sum 1000 components.**

Причина: текущий native Sum path использует `df_world_get_component` per-call = 1000 P/Invoke crossings. Managed direct array iteration с JIT vectorization (AVX2) на cached data = much faster.

**Это prime candidate для span access** (K1 `df_world_acquire_span`). С span — single acquire call returns dense pointer, iteration становится zero-P/Invoke. Это будет следующий major test (планируется в K1 bulk follow-up или K7 full).

#### 4. NativeAdd10k multimodal distribution

BenchmarkDotNet zafiksiroval warning:
> NativeAdd10k: It seems that the distribution can have several modes (mValue = 2.92)

Histogram peaks: 376-389 µs, 403-420 µs, 434-457 µs.

Возможные причины: memory paging (cold vs warm cache), occasional GC (даже при 32B allocated), CPU thermal throttling. Не критично для basic interpretation, но указывает что **native path more sensitive к runtime conditions** чем managed. K7 на CPU-limited hardware покажет real impact.

---

## Measurement 2 — NativeBulkAddBenchmark (PLANNED, not yet executed)

### Hypothesis (K1 brief, restated)

Per-entity native Add = ~400 µs (10,000 P/Invoke crossings).
Bulk native Add = ? µs (1 P/Invoke crossing + native batch loop).

**Target**: bulk path < 100 µs (≥4x speedup over per-entity), restoring throughput parity (or advantage) over managed.

**Stretch target**: bulk path < 50 µs (≥8x speedup), establishing dominant performance.

### Setup required

`Program.cs` currently runs only `NativeVsManagedBenchmark` when `--full` flag is passed. To include `NativeBulkAddBenchmark`:

```csharp
// In Program.cs, replace existing --full handling block:
if (Array.Exists(args, a => a == "--full"))
{
    BenchmarkRunner.Run(new[]
    {
        typeof(NativeVsManagedBenchmark),
        typeof(NativeBulkAddBenchmark),
    });
    return 0;
}
```

5-line change. Imports already include `BenchmarkRunner`.

### Run command (after patch)

```powershell
cd D:\Colony_Simulator\Colony_Simulator\tests\DualFrontier.Core.Benchmarks
dotnet run -c Release -- --full
```

Expected run time: ~10-15 minutes (4 existing + 2 new = 6 benchmarks, each ~2 min warmup+iterations).

### Results placeholder

To be filled when bulk benchmark executes. Expected table format:

| Method                          | Mean     | StdDev | Ratio | Allocated |
|---------------------------------|---------:|-------:|------:|----------:|
| NativeAdd10k_PerEntity          | ~400 µs  | -      | 1.00  | ~32 B     |
| NativeBulkAdd10k_SinglePInvoke  | ?        | ?      | ?     | ?         |

Key questions to answer post-execution:
- **Bulk gain ratio**: NativeAdd10k_PerEntity / NativeBulkAdd10k_SinglePInvoke?
- **Bulk vs Managed**: bulk path faster than ManagedAdd10k (205 µs)?
- **Allocations**: bulk path remains near-zero (~32 B baseline)?

---

## Implications for K-series migration

### Confirmed by Measurement 1

1. **GC pressure elimination structurally works** — 32 B vs 655,606 B = 20,000x reduction. This is the most reliable native-path win, validated quantitatively на main development machine.

2. **Per-entity P/Invoke overhead is real** — ~20ns per crossing × game-realistic crossing rate = significant frame budget cost. K1 batching infrastructure (already shipped) is the structural fix.

3. **Discovery numbers are reproducible** — K0-K3 milestones did not regress baseline. Migration work is additive, not destructive to existing measurement.

### Pending validation

1. **K1 bulk path delivers measurable gain** — Measurement 2 (planned).
2. **K1 span access closes Sum gap** — requires separate benchmark (K7).
3. **Tick-loop end-to-end native vs managed** — K7 representative load.
4. **K4 struct refactor compounds gains** — after K4 closure.
5. **Weak-hardware sensitivity** — K7 CPU-limited container measurements.

### Decision support

Текущие numbers **не достаточны для K8 cutover decision** (это K7's job). Но они достаточны для:

- **Confirming K-series migration delivers structural wins** (GC elimination)
- **Motivating K4 marathon** (per-entity overhead is real, struct refactor will compound)
- **Validating K1 batching infrastructure** (ABI surface ready, only execution measurement остаётся)

If Measurement 2 shows bulk gain ≥4x: **strong validation** of K1 hypothesis, motivation для full K-series completion.
If Measurement 2 shows bulk gain <2x: **rethink** K4 scope, possibly reconsider sequencing.

---

## Document end

Performance snapshot at end of K3. K7 will produce comprehensive performance report applying full §8 metrics rule. K8 will reference both reports для cutover decision.

**Update protocol**: when Measurement 2 executes, append results к section «Measurement 2». No new sections needed — this document will be **superseded**, not extended, when K7 produces its full report.
