using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

namespace DualFrontier.Core.Benchmarks.TickLoop;

/// <summary>
/// K7 BenchmarkDotNet harness — measures per-tick wall-clock cost,
/// allocation rate, and gen0/1/2 collections per op for V2 and V3 of
/// the tick-loop scenario. V1 is intentionally absent from this class
/// because pre-K4 components don't exist on <c>main</c>; the V1 BDN
/// benchmark lives in the worktree-side overload added during Phase 3.3.
///
/// Job: <see cref="ShortRunJob"/> — 3 warmup × 5 measurement iterations.
/// Sufficient for K7 evidence base; full <see cref="LongRunJob"/> would
/// take an order of magnitude longer for negligible additional precision.
///
/// Diagnosers: <see cref="MemoryDiagnoser"/> for allocation +
/// gen0/1/2 collection counts per op.
/// <see cref="ThreadingDiagnoser"/> reports lock contentions if any —
/// useful sanity check that <c>Parallel.ForEach</c> isn't serializing.
/// </summary>
[ShortRunJob]
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class TickLoopBenchmark
{
    private const int PawnCount = 50;
    private const int SetupSeed = 42;
    private const float TickDelta = 1f / 30f;

    private V2ManagedStructsScenario _v2 = null!;
    private V3NativeBatchedScenario _v3 = null!;

    [GlobalSetup(Target = nameof(TickV2_ManagedStructs))]
    public void SetupV2()
    {
        _v2 = new V2ManagedStructsScenario();
        _v2.SetupWorld(PawnCount, SetupSeed);
    }

    [GlobalCleanup(Target = nameof(TickV2_ManagedStructs))]
    public void CleanupV2()
    {
        _v2.TeardownWorld();
    }

    [Benchmark]
    public void TickV2_ManagedStructs()
    {
        _v2.ExecuteTick(TickDelta);
    }

    [GlobalSetup(Target = nameof(TickV3_NativeBatched))]
    public void SetupV3()
    {
        _v3 = new V3NativeBatchedScenario();
        _v3.SetupWorld(PawnCount, SetupSeed);
    }

    [GlobalCleanup(Target = nameof(TickV3_NativeBatched))]
    public void CleanupV3()
    {
        _v3.TeardownWorld();
    }

    [Benchmark]
    public void TickV3_NativeBatched()
    {
        _v3.ExecuteTick(TickDelta);
    }
}
