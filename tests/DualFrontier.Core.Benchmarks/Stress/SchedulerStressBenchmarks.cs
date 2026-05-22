using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using DualFrontier.Contracts.Scheduling;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Benchmarks.Stress;

/// <summary>
/// BDN companions to the xUnit <c>SchedulerStressTests</c>. xUnit tests give
/// pass/fail invariants; these benchmarks give numbers — per-op latency,
/// Gen0/1/2 collections, lock contentions, completed work items.
///
/// Run with:
/// <code>
/// dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks \
///     -- --bdn-stress
/// </code>
/// (Wire-up in Program.cs picks the right filter; see report
/// docs/reports/SCHEDULER_STRESS_TEST_SUITE.md.)
/// </summary>
[ShortRunJob]
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class SchedulerStressBenchmarks
{
    [Params(500, 2_000, 5_000)]
    public int SystemCount { get; set; }

    private uint[] _allIds = Array.Empty<uint>();
    private uint[] _shuffledIds = Array.Empty<uint>();

    [GlobalSetup]
    public void GlobalSetup()
    {
        // Reset process-global native singletons so prior benchmark runs do
        // not bleed state into this one.
        SystemGraphInterop.Clear();
        WakeRegistryInterop.Clear();
        SchedulingPoliciesInterop.Clear();

        var rng = new Random(0xDF_BD_BD);
        _allIds = new uint[SystemCount];
        for (uint i = 0; i < SystemCount; i++)
        {
            _allIds[i] = i;

            // Random scheduling class for the priority benchmark.
            SchedulingClass cls = (SchedulingClass)(i % 5);
            SchedulingPoliciesInterop.SetPolicy(i, schedulingClass: cls);

            // Register native graph node with a unique write component and 0-3
            // forward-only reads. Mirrors the xUnit Scenario 1 graph topology
            // so benchmark numbers correspond to a representative production
            // workload.
            uint writeId = i + 1;
            int readCount = rng.Next(0, 4);
            var reads = new uint[readCount];
            for (int r = 0; r < readCount; r++)
            {
                int maxLower = (int)i;
                reads[r] = maxLower == 0 ? 0u : (uint)rng.Next(1, maxLower + 1);
            }
            SystemGraphInterop.RegisterSystem(
                systemId: i,
                systemFqn: $"BenchStress.{i:D6}",
                readComponentIds: reads,
                writeComponentIds: new[] { writeId },
                priorityClass: (int)cls,
                wakeType: (int)WakeType.Timer);

            // Subscribe every system on a timer with tick rate 1..8 so the
            // runqueue churns.
            WakeRegistryInterop.SubscribeTimer(i, (uint)rng.Next(1, 9));
        }

        SystemGraphInterop.ComputeStaticGraph();

        _shuffledIds = (uint[])_allIds.Clone();
        ShuffleInPlace(_shuffledIds, rng);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        SystemGraphInterop.Clear();
        WakeRegistryInterop.Clear();
        SchedulingPoliciesInterop.Clear();
    }

    /// <summary>
    /// One tick on the native scheduler — FireTimer, drain runqueue, recompute
    /// per-tick graph, TickBegin. The unified per-tick orchestration is the
    /// scheduler's hot path; this benchmark measures it end-to-end.
    /// </summary>
    [Benchmark]
    public int NativeScheduler_OneTick()
    {
        // Use a different tick value per call so the wake registry can fire
        // again — BDN replays the same OperationsPerInvoke many times.
        ulong tick = (ulong)Environment.TickCount;
        WakeRegistryInterop.FireTimer(tick);
        uint[] runnable = WakeRegistryInterop.DrainRunqueue();
        if (runnable.Length > 0)
        {
            SystemGraphInterop.ComputePerTickGraph(runnable);
        }
        return runnable.Length;
    }

    /// <summary>
    /// Static graph rebuild — exercised whenever a mod is added/removed or the
    /// game session starts. Measures Kahn's-algorithm throughput on the C++
    /// side for a large registered system set.
    /// </summary>
    [Benchmark]
    public int NativeScheduler_RebuildStaticGraph()
    {
        SystemGraphInterop.ComputeStaticGraph();
        return SystemGraphInterop.StaticPhaseCount;
    }

    /// <summary>
    /// Priority ordering throughput — the policy engine sorts a system set by
    /// SchedulingClass on every tick where order matters. Hot path for
    /// per-phase dispatch.
    /// </summary>
    [Benchmark]
    public uint OrderByPriority_FullSet()
    {
        uint[] ordered = SchedulingPoliciesInterop.OrderByPriority(_shuffledIds);
        return ordered.Length == 0 ? 0u : ordered[0];
    }

    private static void ShuffleInPlace(uint[] arr, Random rng)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
}

/// <summary>
/// Managed mod-load dependency graph throughput. Lives in its own class so
/// the per-instance <see cref="Params"/> sweep is independent of the native
/// scheduler benchmark (different scale curves, different setup cost).
/// </summary>
[ShortRunJob]
[MemoryDiagnoser]
public class ModDependencyGraphBenchmarks
{
    [Params(500, 2_000, 5_000)]
    public int ModCount { get; set; }

    private List<ModManifest> _mods = new();
    private Dictionary<string, ModManifest> _byId = new(StringComparer.Ordinal);

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rng = new Random(0xBA7C_BD);
        _mods = new List<ModManifest>(ModCount);
        _byId = new Dictionary<string, ModManifest>(ModCount, StringComparer.Ordinal);

        for (int i = 0; i < ModCount; i++)
        {
            string id = $"bench.mod.{i:D5}";
            int depCount = i == 0 ? 0 : rng.Next(0, System.Math.Min(4, i + 1));
            var deps = new ModDependency[depCount];
            for (int d = 0; d < depCount; d++)
            {
                int target = rng.Next(0, i);
                deps[d] = ModDependency.Required($"bench.mod.{target:D5}");
            }
            var manifest = new ModManifest
            {
                Id = id,
                Kind = ModKind.Regular,
                Dependencies = deps,
            };
            _mods.Add(manifest);
            _byId[id] = manifest;
        }
    }

    /// <summary>
    /// Throughput of TopoSortRegularMods on a single large batch. Reports
    /// allocations + Gen0 pressure — relevant because mod-apply runs this
    /// every time the user applies a mod-list change.
    /// </summary>
    [Benchmark]
    public int TopoSort_FullBatch()
    {
        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> errors) =
            ModIntegrationPipeline.TopoSortRegularMods(_mods, _byId);
        return sorted.Count + errors.Count;
    }

    [Benchmark]
    public int CheckDependencyPresence_FullBatch()
    {
        (IReadOnlyList<ValidationError> errors, IReadOnlyList<ValidationWarning> warnings) =
            ModIntegrationPipeline.CheckDependencyPresence(_byId);
        return errors.Count + warnings.Count;
    }
}
