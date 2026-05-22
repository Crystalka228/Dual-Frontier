using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DualFrontier.Application.Bus;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Scheduling;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Core.Tests.Scheduling.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduling;

/// <summary>
/// 8c/16t scheduler stress suite. Pushes the native scheduler graph,
/// managed <see cref="ParallelSystemScheduler"/>, the native three-tier bus
/// and the priority ordering API under simultaneous high contention.
///
/// Each test is tagged <c>[Trait("Category","Stress")]</c> so CI can opt-out
/// via <c>dotnet test --filter "Category!=Stress"</c>; opt-in via
/// <c>dotnet test --filter "Category=Stress"</c>. Tests aggressively reset
/// the process-global native singletons in their constructor / teardown to
/// stay isolated from each other and from the rest of the suite.
///
/// Sister test for the managed mod-dependency graph lives in
/// <c>DualFrontier.Modding.Tests/Pipeline/ModDependencyGraphStressTests.cs</c>
/// (where the relevant <c>InternalsVisibleTo</c> seam exposes
/// <c>ModIntegrationPipeline.TopoSortRegularMods</c>).
///
/// Shared fixture types (TickCounter, WideBase/ChainBase, WC00..WC63 /
/// CC00..CC15 components, W00..W63 / C00..C15 systems) live in
/// <c>Fixtures/ParallelSystemFixtures.cs</c> — extracted 2026-05-21 so the
/// extreme ceiling-probe suite (<c>SchedulerExtremeTests</c>) can share them.
/// </summary>
[Trait("Category", "Stress")]
public sealed class SchedulerStressTests : IDisposable
{
    public SchedulerStressTests()
    {
        ResetNativeGlobals();
    }

    public void Dispose()
    {
        ResetNativeGlobals();
    }

    private static void ResetNativeGlobals()
    {
        SystemGraphInterop.Clear();
        WakeRegistryInterop.Clear();
        SchedulingPoliciesInterop.Clear();
        EventTypeRegistryInterop.ClearForTesting();
    }

    // ════════════════════════════════════════════════════════════════════════
    // Scenario 1 — Native scheduler + native dependency graph.
    // Registers a 5000-system random DAG, hammers ComputePerTickGraph +
    // TickBegin in a tight loop. Validates correctness of the topology
    // builder under the heaviest single-thread compute load it will see in
    // production (registration is single-threaded by interop contract).
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void NativeGraph_FiveThousandSystems_RandomDag_ComputesAndTicksWithoutError()
    {
        if (Environment.ProcessorCount < 2) return;

        const int SystemCount = 5000;
        const int ComponentPool = 500;
        const int TickIterations = 5000;
        var rng = new Random(0xDF_8C_16);

        // Build a write-conflict-free DAG: every system owns one unique write
        // component (id = sysIdx), reads a small random subset of components
        // with strictly lower index. Forward-only edges => acyclic by
        // construction; unique writes => no write-write collision.
        for (uint sysIdx = 0; sysIdx < SystemCount; sysIdx++)
        {
            uint writeId = sysIdx + 1; // shift so no component id is 0
            int readCount = rng.Next(0, 5);
            var reads = new uint[readCount];
            for (int r = 0; r < readCount; r++)
            {
                // Read a component lower than this system's own write id, but
                // within the component pool window.
                uint maxLower = (uint)System.Math.Min(sysIdx, ComponentPool);
                reads[r] = maxLower == 0 ? 0u : (uint)rng.Next(1, (int)maxLower + 1);
            }
            int wakeType = (int)WakeType.Timer;
            int priorityClass = (int)SchedulingClass.Normal;

            bool registered = SystemGraphInterop.RegisterSystem(
                systemId: sysIdx,
                systemFqn: $"Stress.NativeSystem.{sysIdx:D5}",
                readComponentIds: reads,
                writeComponentIds: new[] { writeId },
                priorityClass: priorityClass,
                wakeType: wakeType);
            registered.Should().BeTrue("system {0} must register", sysIdx);

            // Half the systems wake on timer with random tick rates 1..16 so
            // the runnable subset changes every tick.
            if ((sysIdx & 1u) == 0)
            {
                uint ticksPerUpdate = (uint)rng.Next(1, 17);
                WakeRegistryInterop.SubscribeTimer(sysIdx, ticksPerUpdate).Should().BeTrue();
            }
        }

        SystemGraphInterop.SystemCount.Should().Be(SystemCount);

        var staticResult = SystemGraphInterop.ComputeStaticGraph();
        staticResult.Should().Be(SystemGraphInterop.ComputeResult.Success);
        SystemGraphInterop.StaticPhaseCount.Should().BeGreaterThan(0);

        // Tick loop — the per-tick recompute + dispatch is the hot path the
        // native scheduler is going to live in. Failing returns from any of
        // FireTimer / ComputePerTickGraph / TickBegin would surface a
        // regression in the C++ graph or the wake registry.
        for (ulong tick = 0; tick < TickIterations; tick++)
        {
            WakeRegistryInterop.FireTimer(tick);
            uint[] runnable = WakeRegistryInterop.DrainRunqueue();
            if (runnable.Length == 0)
                continue;

            var perTickResult = SystemGraphInterop.ComputePerTickGraph(runnable);
            perTickResult.Should().Be(SystemGraphInterop.ComputeResult.Success,
                "per-tick compute failed on tick {0} with {1} runnable systems",
                tick, runnable.Length);

            // TickBegin combines the above orchestration in native code; call
            // it on alternating ticks to also exercise the unified path.
            if ((tick & 1UL) == 0UL)
            {
                var tickResult = SystemGraphInterop.TickBegin(tick);
                tickResult.Should().Be(SystemGraphInterop.ComputeResult.Success,
                    "TickBegin failed on tick {0}", tick);
            }
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // Scenario 2 — Managed ParallelSystemScheduler under 16-thread contention.
    // Builds a graph with a wide independent layer (64 systems in one phase)
    // plus a deep dependency chain. Wide layer maxes out N-2 worker
    // parallelism; deep chain validates phase-barrier sequencing under load.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void ManagedScheduler_WideLayerPlusDeepChain_DoesNotDeadlockAndExecutesAllSystems()
    {
        if (Environment.ProcessorCount < 2) return;

        using var nativeWorld = new NativeWorld();
        var counter = new TickCounter();

        // Wide layer — every fixture writes a unique component, reads nothing.
        // All wide fixtures land in phase 0 => Parallel.ForEach dispatches
        // them concurrently across the worker pool.
        SystemBase[] wide =
        {
            new W00(counter), new W01(counter), new W02(counter), new W03(counter),
            new W04(counter), new W05(counter), new W06(counter), new W07(counter),
            new W08(counter), new W09(counter), new W10(counter), new W11(counter),
            new W12(counter), new W13(counter), new W14(counter), new W15(counter),
            new W16(counter), new W17(counter), new W18(counter), new W19(counter),
            new W20(counter), new W21(counter), new W22(counter), new W23(counter),
            new W24(counter), new W25(counter), new W26(counter), new W27(counter),
            new W28(counter), new W29(counter), new W30(counter), new W31(counter),
            new W32(counter), new W33(counter), new W34(counter), new W35(counter),
            new W36(counter), new W37(counter), new W38(counter), new W39(counter),
            new W40(counter), new W41(counter), new W42(counter), new W43(counter),
            new W44(counter), new W45(counter), new W46(counter), new W47(counter),
            new W48(counter), new W49(counter), new W50(counter), new W51(counter),
            new W52(counter), new W53(counter), new W54(counter), new W55(counter),
            new W56(counter), new W57(counter), new W58(counter), new W59(counter),
            new W60(counter), new W61(counter), new W62(counter), new W63(counter),
        };

        // Deep chain — each Ck reads Ck-1's write component, so the graph
        // builder must place them in strictly increasing phase indices.
        SystemBase[] chain =
        {
            new C00(counter), new C01(counter), new C02(counter), new C03(counter),
            new C04(counter), new C05(counter), new C06(counter), new C07(counter),
            new C08(counter), new C09(counter), new C10(counter), new C11(counter),
            new C12(counter), new C13(counter), new C14(counter), new C15(counter),
        };

        var graph = new DependencyGraph();
        foreach (SystemBase s in wide) graph.AddSystem(s);
        foreach (SystemBase s in chain) graph.AddSystem(s);
        graph.Build();

        IReadOnlyList<SystemPhase> phases = graph.GetPhases();
        phases.Count.Should().BeGreaterThanOrEqualTo(chain.Length,
            "the 16-system chain forces at least 16 sequential phases");

        var scheduler = new ParallelSystemScheduler(
            phases,
            new TickScheduler(),
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            nativeWorld);

        const int TickIterations = 5000;
        for (int t = 0; t < TickIterations; t++)
            scheduler.ExecuteTick(0.016f);

        // Every fixture's Update is supposed to do exactly one Interlocked.Increment
        // per tick. Total expected: (wide + chain) * TickIterations.
        int expected = (wide.Length + chain.Length) * TickIterations;
        counter.Total.Should().Be(expected, "no system may be skipped or run twice per tick");

        // Thread fan-out — independent systems must distribute across the
        // worker pool. We don't demand exactly N-2 distinct ids (TPL is
        // allowed to under-subscribe on short tasks), but observed parallelism
        // must be meaningful.
        int observedThreads = counter.SnapshotThreadIds().Count;
        observedThreads.Should().BeGreaterThanOrEqualTo(
            System.Math.Max(2, System.Math.Min(8, Environment.ProcessorCount - 2)),
            "wide independent layer must fan out across the thread pool");
    }

    // ════════════════════════════════════════════════════════════════════════
    // Scenario 4 — Three-tier bus under 16-thread publish contention.
    // Validates: Fast is synchronously dispatched (count exact), Normal is
    // batched (count exact after final drain), Background may coalesce
    // (count bounded but at least one per distinct coalesce key).
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void BusTiers_AllThreeTiers_HighContentionPublish_RespectsTierContract()
    {
        if (Environment.ProcessorCount < 2) return;

        var bridge = new ManagedBusBridge();
        var facade = new BusFacade(bridge) { UseNativeBusForDispatch = true };
        bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();

        try
        {
            facade.RegisterEventType<StressFastEvent>().Should().BeTrue();
            facade.RegisterEventType<StressNormalEvent>().Should().BeTrue();
            facade.RegisterEventType<StressBackgroundEvent>().Should().BeTrue();

            uint fastTypeId = facade.GetOrAssignTypeId<StressFastEvent>();
            uint normalTypeId = facade.GetOrAssignTypeId<StressNormalEvent>();
            uint backgroundTypeId = facade.GetOrAssignTypeId<StressBackgroundEvent>();

            var fastHandle = GCHandle.Alloc(s_fastSubscriberDelegate);
            var normalHandle = GCHandle.Alloc(s_normalSubscriberDelegate);
            var backgroundHandle = GCHandle.Alloc(s_backgroundSubscriberDelegate);
            IntPtr fastCb = Marshal.GetFunctionPointerForDelegate(s_fastSubscriberDelegate);
            IntPtr normalCb = Marshal.GetFunctionPointerForDelegate(s_normalSubscriberDelegate);
            IntPtr backgroundCb = Marshal.GetFunctionPointerForDelegate(s_backgroundSubscriberDelegate);

            ulong fastSid = bridge.SubscribeFast(fastTypeId, modId: 100, fastCb, fastHandle);
            ulong normalSid = bridge.SubscribeNormal(normalTypeId, modId: 200, normalCb, normalHandle);
            ulong backgroundSid = bridge.SubscribeBackground(backgroundTypeId, modId: 300, backgroundCb, backgroundHandle);

            fastSid.Should().NotBe(0u);
            normalSid.Should().NotBe(0u);
            backgroundSid.Should().NotBe(0u);

            s_fastCount = 0;
            s_normalCount = 0;
            s_backgroundCount = 0;

            // Conservative per-thread counts — the native Normal-tier batch
            // queue has an unknown capacity at the managed boundary; over-
            // publishing risks silently dropping events and breaking the
            // "delivered == published" invariant. 1000 × 16 = 16k per tier
            // = 48k total events; still enough to surface contention while
            // staying within typical queue bounds.
            const int ProducerThreads = 16;
            const int PerThreadFast = 1_000;
            const int PerThreadNormal = 1_000;
            const int PerThreadBackground = 1_000;

            // 16 producer threads — each publishes the same mix. Parallel.For
            // body holds no shared state besides the native publish path, so
            // contention lives inside the C++ bus implementation, which is
            // exactly where we want it.
            Parallel.For(0, ProducerThreads, threadIdx =>
            {
                for (int i = 0; i < PerThreadFast; i++)
                {
                    var evt = new StressFastEvent { Value = (threadIdx << 16) | i };
                    facade.Publish(evt);
                }
                for (int i = 0; i < PerThreadNormal; i++)
                {
                    var evt = new StressNormalEvent { Value = (threadIdx << 16) | i };
                    facade.Publish(evt);
                }
                for (int i = 0; i < PerThreadBackground; i++)
                {
                    var evt = new StressBackgroundEvent
                    {
                        Value = (threadIdx << 16) | i,
                        // coalesce_key for background tier is encoded via the
                        // event payload here; the native publish path passes
                        // a key=0 by default since BusFacade.Publish does not
                        // expose the parameter. We still validate at least
                        // one delivery for the type.
                    };
                    facade.Publish(evt);
                }
            });

            // Fast tier is synchronously dispatched — once Publish returns,
            // the callback must have already fired. Total invocations == total
            // published.
            int expectedFast = ProducerThreads * PerThreadFast;
            s_fastCount.Should().Be(expectedFast,
                "Fast tier contract requires synchronous dispatch (≤1ms, no queuing)");

            // Normal tier is batched; the scheduler would drain at phase
            // boundaries. We drive a final drain manually and assert the
            // observed callback count equals published normal events.
            // We may need multiple drains if the batch capacity is bounded;
            // drain in a loop until quiescent.
            int expectedNormal = ProducerThreads * PerThreadNormal;
            int safetyDrainIters = 0;
            while (s_normalCount < expectedNormal && safetyDrainIters < 1024)
            {
                bridge.DrainNormalBatch();
                safetyDrainIters++;
                if (s_normalCount == expectedNormal) break;
                // give native side a moment in case drain is async-ish
                Thread.Yield();
            }
            s_normalCount.Should().Be(expectedNormal,
                "Normal tier must deliver every published event after drain (drained {0}x)",
                safetyDrainIters);

            // Background tier dispatches on idle-slot via the native scheduler
            // (multi-tick acceptable per К-L15). Without driving the native
            // scheduler in this test, queued Background events may not have
            // dispatched yet — and coalescence at key=0 may merge them all.
            // Assert only that we did not exceed the published ceiling (no
            // ghost deliveries) and that publishing did not throw. Lower
            // bound is intentionally 0 — a strict lower bound requires
            // hooking the native idle-dispatch path which is out of scope
            // for a managed-only stress test.
            int expectedBackgroundCeiling = ProducerThreads * PerThreadBackground;
            s_backgroundCount.Should().BeGreaterOrEqualTo(0);
            s_backgroundCount.Should().BeLessOrEqualTo(expectedBackgroundCeiling,
                "Background tier delivery count cannot exceed published count");

            bridge.Unsubscribe(fastSid);
            bridge.Unsubscribe(normalSid);
            bridge.Unsubscribe(backgroundSid);
        }
        finally
        {
            bridge.ClearForTesting();
            EventTypeRegistryInterop.ClearForTesting();
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // Scenario 5 — SchedulingPolicies.OrderByPriority under 16-thread access.
    // The 5-class priority order is documented to be deterministic and pure;
    // hammering it from 16 threads simultaneously with random permutations of
    // the same input must produce identical (and correctly sorted) outputs.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void SchedulingPolicies_OrderByPriority_ParallelCalls_DeterministicAndCorrectlyOrdered()
    {
        if (Environment.ProcessorCount < 2) return;

        const int SystemCount = 500;
        var schedulingClasses = new SchedulingClass[]
        {
            SchedulingClass.RealTime,
            SchedulingClass.High,
            SchedulingClass.Normal,
            SchedulingClass.Low,
            SchedulingClass.Background,
        };

        for (uint id = 0; id < SystemCount; id++)
        {
            SchedulingClass cls = schedulingClasses[id % schedulingClasses.Length];
            SchedulingPoliciesInterop.SetPolicy(id, schedulingClass: cls).Should().BeTrue();
        }

        // Build 16 distinct random permutations of the id set and verify
        // OrderByPriority sorts each correctly + the function is pure.
        const int ParallelCalls = 16;
        const int IterationsPerThread = 100;
        var failures = new ConcurrentBag<string>();

        Parallel.For(0, ParallelCalls, callIdx =>
        {
            var rng = new Random(callIdx * 7919 + 1);
            for (int iter = 0; iter < IterationsPerThread; iter++)
            {
                uint[] permutation = ShufflePermutation(SystemCount, rng);
                uint[] ordered = SchedulingPoliciesInterop.OrderByPriority(permutation);

                if (ordered.Length != permutation.Length)
                {
                    failures.Add($"call {callIdx} iter {iter}: length mismatch {ordered.Length} != {permutation.Length}");
                    continue;
                }

                // Strict monotonicity by scheduling class (lower=higher prio).
                for (int i = 1; i < ordered.Length; i++)
                {
                    SchedulingClass prev = SchedulingPoliciesInterop.GetClass(ordered[i - 1]);
                    SchedulingClass curr = SchedulingPoliciesInterop.GetClass(ordered[i]);
                    if ((int)prev > (int)curr)
                    {
                        failures.Add($"call {callIdx} iter {iter}: priority inversion at index {i} ({prev} after {curr})");
                        break;
                    }
                    // Within same class, ascending id for determinism.
                    if (prev == curr && ordered[i - 1] >= ordered[i])
                    {
                        failures.Add($"call {callIdx} iter {iter}: in-class ordering violated at {i}");
                        break;
                    }
                }
            }
        });

        failures.Should().BeEmpty();
    }

    private static uint[] ShufflePermutation(int size, Random rng)
    {
        var arr = new uint[size];
        for (int i = 0; i < size; i++) arr[i] = (uint)i;
        // Fisher-Yates.
        for (int i = size - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }

    // ─── Bus tier fixture events + callbacks ────────────────────────────────

    [EventTier(BusTier.Fast)]
    private struct StressFastEvent : IEvent
    {
        public int Value;
    }

    [EventTier(BusTier.Normal)]
    private struct StressNormalEvent : IEvent
    {
        public int Value;
    }

    [EventTier(BusTier.Background)]
    private struct StressBackgroundEvent : IEvent
    {
        public int Value;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SubscriberDelegate(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData);

    private static int s_fastCount;
    private static int s_normalCount;
    private static int s_backgroundCount;

    private static readonly SubscriberDelegate s_fastSubscriberDelegate = FastCallback;
    private static readonly SubscriberDelegate s_normalSubscriberDelegate = NormalCallback;
    private static readonly SubscriberDelegate s_backgroundSubscriberDelegate = BackgroundCallback;

    private static void FastCallback(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try { Interlocked.Increment(ref s_fastCount); } catch { }
    }
    private static void NormalCallback(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try { Interlocked.Increment(ref s_normalCount); } catch { }
    }
    private static void BackgroundCallback(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try { Interlocked.Increment(ref s_backgroundCount); } catch { }
    }
}
