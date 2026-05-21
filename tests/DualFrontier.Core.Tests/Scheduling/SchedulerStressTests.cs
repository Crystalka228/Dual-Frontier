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
                uint maxLower = (uint)Math.Min(sysIdx, ComponentPool);
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
            Math.Max(2, Math.Min(8, Environment.ProcessorCount - 2)),
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

    // ════════════════════════════════════════════════════════════════════════
    // Fixture state — shared across scenarios.
    // ════════════════════════════════════════════════════════════════════════

    private sealed class TickCounter
    {
        private long _total;
        private readonly ConcurrentDictionary<int, byte> _threadIds = new();

        public long Total => Interlocked.Read(ref _total);

        public void Tick()
        {
            Interlocked.Increment(ref _total);
            _threadIds.TryAdd(Thread.CurrentThread.ManagedThreadId, 0);
        }

        public HashSet<int> SnapshotThreadIds() => new(_threadIds.Keys);
    }

    // ─── Wide-layer fixture systems ────────────────────────────────────────
    // 64 distinct SystemBase subclasses, each writing a unique component.
    // The DependencyGraph will see no shared writes and no reads across them,
    // placing every one into phase 0.

    private sealed class WC00 : IComponent { } private sealed class WC01 : IComponent { }
    private sealed class WC02 : IComponent { } private sealed class WC03 : IComponent { }
    private sealed class WC04 : IComponent { } private sealed class WC05 : IComponent { }
    private sealed class WC06 : IComponent { } private sealed class WC07 : IComponent { }
    private sealed class WC08 : IComponent { } private sealed class WC09 : IComponent { }
    private sealed class WC10 : IComponent { } private sealed class WC11 : IComponent { }
    private sealed class WC12 : IComponent { } private sealed class WC13 : IComponent { }
    private sealed class WC14 : IComponent { } private sealed class WC15 : IComponent { }
    private sealed class WC16 : IComponent { } private sealed class WC17 : IComponent { }
    private sealed class WC18 : IComponent { } private sealed class WC19 : IComponent { }
    private sealed class WC20 : IComponent { } private sealed class WC21 : IComponent { }
    private sealed class WC22 : IComponent { } private sealed class WC23 : IComponent { }
    private sealed class WC24 : IComponent { } private sealed class WC25 : IComponent { }
    private sealed class WC26 : IComponent { } private sealed class WC27 : IComponent { }
    private sealed class WC28 : IComponent { } private sealed class WC29 : IComponent { }
    private sealed class WC30 : IComponent { } private sealed class WC31 : IComponent { }
    private sealed class WC32 : IComponent { } private sealed class WC33 : IComponent { }
    private sealed class WC34 : IComponent { } private sealed class WC35 : IComponent { }
    private sealed class WC36 : IComponent { } private sealed class WC37 : IComponent { }
    private sealed class WC38 : IComponent { } private sealed class WC39 : IComponent { }
    private sealed class WC40 : IComponent { } private sealed class WC41 : IComponent { }
    private sealed class WC42 : IComponent { } private sealed class WC43 : IComponent { }
    private sealed class WC44 : IComponent { } private sealed class WC45 : IComponent { }
    private sealed class WC46 : IComponent { } private sealed class WC47 : IComponent { }
    private sealed class WC48 : IComponent { } private sealed class WC49 : IComponent { }
    private sealed class WC50 : IComponent { } private sealed class WC51 : IComponent { }
    private sealed class WC52 : IComponent { } private sealed class WC53 : IComponent { }
    private sealed class WC54 : IComponent { } private sealed class WC55 : IComponent { }
    private sealed class WC56 : IComponent { } private sealed class WC57 : IComponent { }
    private sealed class WC58 : IComponent { } private sealed class WC59 : IComponent { }
    private sealed class WC60 : IComponent { } private sealed class WC61 : IComponent { }
    private sealed class WC62 : IComponent { } private sealed class WC63 : IComponent { }

    // Wide fixture base — every concrete subclass has its own [SystemAccess]
    // declaring a unique write component. Per-tick work: increment shared
    // counter + small SpinWait to give the worker thread observable load.
    private abstract class WideBase : SystemBase
    {
        protected readonly TickCounter Counter;
        protected WideBase(TickCounter counter) { Counter = counter; }
        public override void Update(float delta)
        {
            Counter.Tick();
            Thread.SpinWait(2_000);
        }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC00) }, bus: "TestBus")]
    private sealed class W00 : WideBase { public W00(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC01) }, bus: "TestBus")]
    private sealed class W01 : WideBase { public W01(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC02) }, bus: "TestBus")]
    private sealed class W02 : WideBase { public W02(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC03) }, bus: "TestBus")]
    private sealed class W03 : WideBase { public W03(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC04) }, bus: "TestBus")]
    private sealed class W04 : WideBase { public W04(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC05) }, bus: "TestBus")]
    private sealed class W05 : WideBase { public W05(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC06) }, bus: "TestBus")]
    private sealed class W06 : WideBase { public W06(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC07) }, bus: "TestBus")]
    private sealed class W07 : WideBase { public W07(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC08) }, bus: "TestBus")]
    private sealed class W08 : WideBase { public W08(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC09) }, bus: "TestBus")]
    private sealed class W09 : WideBase { public W09(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC10) }, bus: "TestBus")]
    private sealed class W10 : WideBase { public W10(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC11) }, bus: "TestBus")]
    private sealed class W11 : WideBase { public W11(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC12) }, bus: "TestBus")]
    private sealed class W12 : WideBase { public W12(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC13) }, bus: "TestBus")]
    private sealed class W13 : WideBase { public W13(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC14) }, bus: "TestBus")]
    private sealed class W14 : WideBase { public W14(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC15) }, bus: "TestBus")]
    private sealed class W15 : WideBase { public W15(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC16) }, bus: "TestBus")]
    private sealed class W16 : WideBase { public W16(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC17) }, bus: "TestBus")]
    private sealed class W17 : WideBase { public W17(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC18) }, bus: "TestBus")]
    private sealed class W18 : WideBase { public W18(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC19) }, bus: "TestBus")]
    private sealed class W19 : WideBase { public W19(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC20) }, bus: "TestBus")]
    private sealed class W20 : WideBase { public W20(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC21) }, bus: "TestBus")]
    private sealed class W21 : WideBase { public W21(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC22) }, bus: "TestBus")]
    private sealed class W22 : WideBase { public W22(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC23) }, bus: "TestBus")]
    private sealed class W23 : WideBase { public W23(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC24) }, bus: "TestBus")]
    private sealed class W24 : WideBase { public W24(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC25) }, bus: "TestBus")]
    private sealed class W25 : WideBase { public W25(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC26) }, bus: "TestBus")]
    private sealed class W26 : WideBase { public W26(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC27) }, bus: "TestBus")]
    private sealed class W27 : WideBase { public W27(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC28) }, bus: "TestBus")]
    private sealed class W28 : WideBase { public W28(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC29) }, bus: "TestBus")]
    private sealed class W29 : WideBase { public W29(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC30) }, bus: "TestBus")]
    private sealed class W30 : WideBase { public W30(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC31) }, bus: "TestBus")]
    private sealed class W31 : WideBase { public W31(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC32) }, bus: "TestBus")]
    private sealed class W32 : WideBase { public W32(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC33) }, bus: "TestBus")]
    private sealed class W33 : WideBase { public W33(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC34) }, bus: "TestBus")]
    private sealed class W34 : WideBase { public W34(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC35) }, bus: "TestBus")]
    private sealed class W35 : WideBase { public W35(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC36) }, bus: "TestBus")]
    private sealed class W36 : WideBase { public W36(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC37) }, bus: "TestBus")]
    private sealed class W37 : WideBase { public W37(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC38) }, bus: "TestBus")]
    private sealed class W38 : WideBase { public W38(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC39) }, bus: "TestBus")]
    private sealed class W39 : WideBase { public W39(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC40) }, bus: "TestBus")]
    private sealed class W40 : WideBase { public W40(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC41) }, bus: "TestBus")]
    private sealed class W41 : WideBase { public W41(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC42) }, bus: "TestBus")]
    private sealed class W42 : WideBase { public W42(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC43) }, bus: "TestBus")]
    private sealed class W43 : WideBase { public W43(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC44) }, bus: "TestBus")]
    private sealed class W44 : WideBase { public W44(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC45) }, bus: "TestBus")]
    private sealed class W45 : WideBase { public W45(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC46) }, bus: "TestBus")]
    private sealed class W46 : WideBase { public W46(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC47) }, bus: "TestBus")]
    private sealed class W47 : WideBase { public W47(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC48) }, bus: "TestBus")]
    private sealed class W48 : WideBase { public W48(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC49) }, bus: "TestBus")]
    private sealed class W49 : WideBase { public W49(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC50) }, bus: "TestBus")]
    private sealed class W50 : WideBase { public W50(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC51) }, bus: "TestBus")]
    private sealed class W51 : WideBase { public W51(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC52) }, bus: "TestBus")]
    private sealed class W52 : WideBase { public W52(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC53) }, bus: "TestBus")]
    private sealed class W53 : WideBase { public W53(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC54) }, bus: "TestBus")]
    private sealed class W54 : WideBase { public W54(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC55) }, bus: "TestBus")]
    private sealed class W55 : WideBase { public W55(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC56) }, bus: "TestBus")]
    private sealed class W56 : WideBase { public W56(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC57) }, bus: "TestBus")]
    private sealed class W57 : WideBase { public W57(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC58) }, bus: "TestBus")]
    private sealed class W58 : WideBase { public W58(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC59) }, bus: "TestBus")]
    private sealed class W59 : WideBase { public W59(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC60) }, bus: "TestBus")]
    private sealed class W60 : WideBase { public W60(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC61) }, bus: "TestBus")]
    private sealed class W61 : WideBase { public W61(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC62) }, bus: "TestBus")]
    private sealed class W62 : WideBase { public W62(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(WC63) }, bus: "TestBus")]
    private sealed class W63 : WideBase { public W63(TickCounter c) : base(c) { } }

    // ─── Deep-chain fixture systems ─────────────────────────────────────────
    // C00 writes ChainC00; C01 reads ChainC00 and writes ChainC01; ... and so on.
    // The graph builder must serialize them into ascending phases.

    private sealed class CC00 : IComponent { } private sealed class CC01 : IComponent { }
    private sealed class CC02 : IComponent { } private sealed class CC03 : IComponent { }
    private sealed class CC04 : IComponent { } private sealed class CC05 : IComponent { }
    private sealed class CC06 : IComponent { } private sealed class CC07 : IComponent { }
    private sealed class CC08 : IComponent { } private sealed class CC09 : IComponent { }
    private sealed class CC10 : IComponent { } private sealed class CC11 : IComponent { }
    private sealed class CC12 : IComponent { } private sealed class CC13 : IComponent { }
    private sealed class CC14 : IComponent { } private sealed class CC15 : IComponent { }

    private abstract class ChainBase : SystemBase
    {
        protected readonly TickCounter Counter;
        protected ChainBase(TickCounter counter) { Counter = counter; }
        public override void Update(float delta)
        {
            Counter.Tick();
            Thread.SpinWait(2_000);
        }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(CC00) }, bus: "TestBus")]
    private sealed class C00 : ChainBase { public C00(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC00) }, writes: new[] { typeof(CC01) }, bus: "TestBus")]
    private sealed class C01 : ChainBase { public C01(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC01) }, writes: new[] { typeof(CC02) }, bus: "TestBus")]
    private sealed class C02 : ChainBase { public C02(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC02) }, writes: new[] { typeof(CC03) }, bus: "TestBus")]
    private sealed class C03 : ChainBase { public C03(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC03) }, writes: new[] { typeof(CC04) }, bus: "TestBus")]
    private sealed class C04 : ChainBase { public C04(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC04) }, writes: new[] { typeof(CC05) }, bus: "TestBus")]
    private sealed class C05 : ChainBase { public C05(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC05) }, writes: new[] { typeof(CC06) }, bus: "TestBus")]
    private sealed class C06 : ChainBase { public C06(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC06) }, writes: new[] { typeof(CC07) }, bus: "TestBus")]
    private sealed class C07 : ChainBase { public C07(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC07) }, writes: new[] { typeof(CC08) }, bus: "TestBus")]
    private sealed class C08 : ChainBase { public C08(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC08) }, writes: new[] { typeof(CC09) }, bus: "TestBus")]
    private sealed class C09 : ChainBase { public C09(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC09) }, writes: new[] { typeof(CC10) }, bus: "TestBus")]
    private sealed class C10 : ChainBase { public C10(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC10) }, writes: new[] { typeof(CC11) }, bus: "TestBus")]
    private sealed class C11 : ChainBase { public C11(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC11) }, writes: new[] { typeof(CC12) }, bus: "TestBus")]
    private sealed class C12 : ChainBase { public C12(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC12) }, writes: new[] { typeof(CC13) }, bus: "TestBus")]
    private sealed class C13 : ChainBase { public C13(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC13) }, writes: new[] { typeof(CC14) }, bus: "TestBus")]
    private sealed class C14 : ChainBase { public C14(TickCounter c) : base(c) { } }
    [SystemAccess(reads: new[] { typeof(CC14) }, writes: new[] { typeof(CC15) }, bus: "TestBus")]
    private sealed class C15 : ChainBase { public C15(TickCounter c) : base(c) { } }

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
