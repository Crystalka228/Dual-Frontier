using BenchmarkDotNet.Attributes;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Benchmarks;

/// <summary>
/// K3 benchmark: validates the parallel bootstrap speedup hypothesis.
///
/// The bootstrap graph contains 4 tasks. InitWorldStructure and InitThreadPool
/// can run in parallel (both depend only on AllocateMemoryPools). On a
/// multi-core machine, parallel dispatch should show a measurable improvement
/// over hypothetical sequential execution.
///
/// Note: this benchmark measures end-to-end bootstrap time, NOT a managed-vs-
/// native comparison. The point is "bootstrap is fast enough" (target
/// 5-15 ms on typical hardware per KERNEL_ARCHITECTURE §K3 success criteria).
///
/// K7 will add the comparative tick-loop benchmark applying §8 metrics.
/// Benchmark execution is deferred to that milestone — this commit ships
/// the benchmark code only.
/// </summary>
[MemoryDiagnoser]
public class BootstrapTimeBenchmark
{
    [Benchmark]
    public NativeWorld BootstrapAndDispose()
    {
        // useRegistry: false — preserves the original K3-era benchmark baseline
        // (5-15ms target measured without registry construction overhead). The
        // K8.3+K8.4 milestone (2026-05-14) made the registry-bound path the
        // Bootstrap.Run default; benchmark stays on the no-registry path so
        // historical baselines remain comparable. A separate benchmark variant
        // for the registry path can be added if K-L4 wiring overhead matters
        // for production wall-clock targets.
        var world = Bootstrap.Run(useRegistry: false);
        world.Dispose();
        return world;
    }
}
