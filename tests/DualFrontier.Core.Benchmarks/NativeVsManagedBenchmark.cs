using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Benchmarks;

/// <summary>
/// Apples-to-apples comparison of the managed <see cref="World"/> against
/// the experimental native <see cref="NativeWorld"/>.
///
/// Three scenarios are measured:
///   * <see cref="Add"/>: 10k AddComponent calls on a fresh world.
///   * <see cref="Get"/>: 10k GetComponent calls (the hot read path).
///   * <see cref="SumCurrent"/>: 10k GetComponent + integer reduction — a
///     shape closer to real system updates where data is read and folded.
///
/// The managed side is marked <see cref="BenchmarkAttribute.Baseline"/> so
/// BenchmarkDotNet prints relative ratios. See
/// <c>docs/NATIVE_CORE.md</c> for the success threshold and the decision
/// rule that gates further investment.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class NativeVsManagedBenchmark
{
    private const int EntityCount = 10_000;

    private World _managedWorld = null!;
    private NativeWorld _nativeWorld = null!;
    private EntityId[] _managedEntities = null!;
    private EntityId[] _nativeEntities = null!;

    [GlobalSetup]
    public void Setup()
    {
        _managedWorld = new World();
        _nativeWorld = new NativeWorld();
        _managedEntities = new EntityId[EntityCount];
        _nativeEntities = new EntityId[EntityCount];

        for (int i = 0; i < EntityCount; i++)
        {
            _managedEntities[i] = _managedWorld.CreateEntity();
            _nativeEntities[i] = _nativeWorld.CreateEntity();

            var component = new BenchHealthComponent(i, 100);
            _managedWorld.AddComponent(_managedEntities[i], component);
            _nativeWorld.AddComponent(_nativeEntities[i], component);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _nativeWorld.Dispose();
    }

    [Benchmark(Baseline = true)]
    public int ManagedSumCurrent()
    {
        int sum = 0;
        for (int i = 0; i < EntityCount; i++)
        {
            _managedWorld.TryGetComponent(_managedEntities[i], out BenchHealthComponent c);
            sum += c.Current;
        }
        return sum;
    }

    [Benchmark]
    public int NativeSumCurrent()
    {
        int sum = 0;
        for (int i = 0; i < EntityCount; i++)
        {
            _nativeWorld.TryGetComponent(_nativeEntities[i], out BenchHealthComponent c);
            sum += c.Current;
        }
        return sum;
    }

    [Benchmark]
    public void ManagedAdd10k()
    {
        // Rebuild a fresh managed world each invocation so only AddComponent
        // is being timed. CreateEntity is unavoidable setup.
        var world = new World();
        for (int i = 0; i < EntityCount; i++)
        {
            var e = world.CreateEntity();
            world.AddComponent(e, new BenchHealthComponent(i, 100));
        }
    }

    [Benchmark]
    public void NativeAdd10k()
    {
        using var world = new NativeWorld();
        for (int i = 0; i < EntityCount; i++)
        {
            var e = world.CreateEntity();
            world.AddComponent(e, new BenchHealthComponent(i, 100));
        }
    }
}
