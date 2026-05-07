using BenchmarkDotNet.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Benchmarks;

/// <summary>
/// K1 benchmark: validates the bulk-add hypothesis quantitatively.
///
/// Discovery report (CPP_KERNEL_BRANCH_REPORT.md §10.4) recorded:
///   ManagedAdd10k = 218 μs (with 655 KB allocations)
///   NativeAdd10k  = 399 μs (with 24 B allocations) — per-entity P/Invoke
///                                                    overhead dominant
///
/// K1 hypothesis: a single P/Invoke crossing for the entire batch eliminates
/// the per-entity overhead and pushes the native bulk path under 200 μs,
/// restoring throughput parity with managed AND keeping GC pressure near zero.
///
/// Benchmark execution + analysis is deferred to K7 (representative-load
/// measurement). This file ships the benchmark code only.
/// </summary>
[MemoryDiagnoser]
public class NativeBulkAddBenchmark
{
    private const int EntityCount = 10_000;
    private NativeWorld _world = null!;
    private EntityId[] _entities = null!;
    private BenchHealthComponent[] _components = null!;

    [GlobalSetup]
    public void Setup()
    {
        _world = new NativeWorld();
        _entities = new EntityId[EntityCount];
        _components = new BenchHealthComponent[EntityCount];

        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i] = _world.CreateEntity();
            _components[i] = new BenchHealthComponent(i, 100);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _world.Dispose();
    }

    [Benchmark(Baseline = true)]
    public void NativeAdd10k_PerEntity()
    {
        // Existing path: 10,000 P/Invoke crossings.
        for (int i = 0; i < EntityCount; i++)
        {
            _world.AddComponent(_entities[i], _components[i]);
        }
    }

    [Benchmark]
    public void NativeBulkAdd10k_SinglePInvoke()
    {
        // K1 path: a single P/Invoke crossing for the entire batch.
        _world.AddComponents<BenchHealthComponent>(_entities, _components);
    }
}
