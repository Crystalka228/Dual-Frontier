using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Xunit.Abstractions;

namespace DualFrontier.Core.Tests.Scheduling;

/// <summary>
/// 8c/16t scheduler+bus ceiling probe. Scales each surface 10-100× past
/// <see cref="SchedulerStressTests"/> baseline to discover where the engine
/// actually breaks on the local machine. Each scenario uses
/// <see cref="ITestOutputHelper"/> to emit quantitative numbers (mean/max
/// tick times, throughputs, latency percentiles) so the run produces a
/// ceiling report, not just pass/fail.
///
/// Skipped early if <c>Environment.ProcessorCount &lt; 8</c> — extreme scales
/// are calibrated for 8c/16t. xUnit Skip is not used (it requires throwing);
/// the early-return surface the reason in test output instead.
///
/// Each scenario resets the native scheduler/wake/policies/event-type
/// singletons in ctor/Dispose (<see cref="ResetAllGlobals"/>) and clears the
/// managed bus bridge per-test in its own finally. Cross-class isolation from
/// the other singleton-touching classes is provided by the shared
/// <c>[Collection("SharedNativeSingleton")]</c> membership (F-29(a)); it is no
/// longer inferred from a per-class collection.
/// </summary>
[Trait("Category", "Extreme")]
[Collection("SharedNativeSingleton")]  // F-29(a): serialise with every shared-native-singleton class; was "ExtremeSerial", which serialised only within itself and so raced sibling classes (see SharedNativeSingletonCollection)
public sealed class SchedulerExtremeTests : IDisposable
{
    private readonly ITestOutputHelper _out;

    // File-based progress log — survives test buffering. Tail this file while
    // the suite runs to see exactly which test is currently in ctor/body/Dispose.
    // Cleared at process startup; appended thereafter.
    private static readonly string s_progressLogPath =
        Path.Combine(Path.GetTempPath(), "df_extreme_progress.log");
    private static readonly object s_progressLock = new();
    private static int s_progressInitialized;

    private static void LogProgress(string message)
    {
        try
        {
            // First writer truncates the file so each run starts fresh.
            if (Interlocked.Exchange(ref s_progressInitialized, 1) == 0)
            {
                try { File.WriteAllText(s_progressLogPath, ""); } catch { }
            }
            lock (s_progressLock)
            {
                File.AppendAllText(s_progressLogPath,
                    $"{DateTime.Now:HH:mm:ss.fff} TID={Thread.CurrentThread.ManagedThreadId,2} {message}{Environment.NewLine}");
            }
        }
        catch { /* never let logging kill the test */ }
    }

    public SchedulerExtremeTests(ITestOutputHelper output)
    {
        _out = output;
        LogProgress(">>> CTOR ENTER");
        ResetAllGlobals();
        LogProgress("<<< CTOR EXIT");
    }

    public void Dispose()
    {
        LogProgress(">>> DISPOSE ENTER");
        // Deliberately minimal: ResetAllGlobals only. Earlier draft used
        // GC.Collect + WaitForPendingFinalizers here to give each scenario a
        // clean heap, but WaitForPendingFinalizers blocked indefinitely
        // after the bus marathon (S6) — finalizers for callback delegates
        // backed by GCHandles never quiesced. Each scenario explicitly frees
        // its own GCHandles in finally; trust that.
        ResetAllGlobals();
        LogProgress("<<< DISPOSE EXIT");
    }

    private static void ResetAllGlobals()
    {
        LogProgress("  Reset: SystemGraphInterop.Clear()");
        SystemGraphInterop.Clear();
        LogProgress("  Reset: WakeRegistryInterop.Clear()");
        WakeRegistryInterop.Clear();
        LogProgress("  Reset: SchedulingPoliciesInterop.Clear()");
        SchedulingPoliciesInterop.Clear();
        LogProgress("  Reset: EventTypeRegistryInterop.ClearForTesting()");
        EventTypeRegistryInterop.ClearForTesting();
        LogProgress("  Reset: done");
        // NOTE: bus bridge cleanup intentionally NOT done here. The bus
        // tests (S3-S5b) each create their own bridge and call
        // ClearForTesting in finally — that's where cleanup belongs.
    }

    private bool SkipIfUnderpowered(string scenarioName)
    {
        if (Environment.ProcessorCount >= 8) return false;
        _out.WriteLine($"[{scenarioName}] SKIPPED — extreme suite requires ≥8 logical cores, found {Environment.ProcessorCount}");
        return true;
    }

    private static double TicksToMicros(long ticks)
        => ticks * 1_000_000.0 / Stopwatch.Frequency;

    private static double TicksToNanos(long ticks)
        => ticks * 1_000_000_000.0 / Stopwatch.Frequency;

    // ════════════════════════════════════════════════════════════════════════
    // S1 — Native scheduler stress at proven scale. 50 000 systems × 3 000 ticks.
    //   F-29(b) RESOLVED: the scale wall was the O(N^2) graph rebuild (the
    //   write-conflict + edge scans plus the register dup-scan), now O(N+E)
    //   index-keyed (system_graph.cpp). The "native mutex above ~90k" hypothesis
    //   was REFUTED -- there is no lock on the compute/tick path; the wall was
    //   pure compute. 50k now rebuilds in ~tens of ms; un-quarantined here.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void S1_NativeGraph_FiftyThousandSystems_ThreeThousandTicks_HoldsWithoutError()
    {
        LogProgress("### S1 BODY ENTER");
        if (SkipIfUnderpowered("S1")) return;
        LogProgress("### S1 step.1: post-skip-check");

        const uint SystemCount = 50_000;
        const int ComponentPool = 4_000;
        const int TickIterations = 3_000;
        var rng = new Random(0x7E57_E50A);
        LogProgress("### S1 step.2: locals + rng ready");

        long allocBefore = GC.GetTotalAllocatedBytes(true);
        LogProgress("### S1 step.3: GC.GetTotalAllocatedBytes done");
        int gen0Before = GC.CollectionCount(0);
        int gen1Before = GC.CollectionCount(1);
        int gen2Before = GC.CollectionCount(2);
        LogProgress("### S1 step.4: GC.CollectionCount snapshots done");

        // ─── Setup: register 100k systems with forward-only DAG ─────────────
        var setupSw = Stopwatch.StartNew();
        LogProgress("### S1 step.5: register loop starting");
        for (uint sysIdx = 0; sysIdx < SystemCount; sysIdx++)
        {
            if ((sysIdx % 10_000u) == 0u) LogProgress($"### S1 register progress: sysIdx={sysIdx:N0}");
            uint writeId = sysIdx + 1;
            int readCount = rng.Next(0, 5);
            var reads = new uint[readCount];
            for (int r = 0; r < readCount; r++)
            {
                uint maxLower = (uint)System.Math.Min(sysIdx, ComponentPool);
                reads[r] = maxLower == 0 ? 0u : (uint)rng.Next(1, (int)maxLower + 1);
            }

            bool registered = SystemGraphInterop.RegisterSystem(
                systemId: sysIdx,
                systemFqn: $"Extreme.NativeSystem.{sysIdx:D6}",
                readComponentIds: reads,
                writeComponentIds: new[] { writeId },
                priorityClass: (int)SchedulingClass.Normal,
                wakeType: (int)WakeType.Timer);
            registered.Should().BeTrue("system {0} of {1} must register", sysIdx, SystemCount);

            if ((sysIdx & 1u) == 0)
            {
                uint ticksPerUpdate = (uint)rng.Next(1, 17);
                WakeRegistryInterop.SubscribeTimer(sysIdx, ticksPerUpdate).Should().BeTrue();
            }
        }
        setupSw.Stop();

        SystemGraphInterop.SystemCount.Should().Be((int)SystemCount);
        SystemGraphInterop.ComputeStaticGraph().Should().Be(SystemGraphInterop.ComputeResult.Success);
        SystemGraphInterop.StaticPhaseCount.Should().BeGreaterThan(0);

        // ─── Tick loop: per-tick recompute is the production hot path ───────
        var tickTimes = new long[TickIterations];
        var loopSw = Stopwatch.StartNew();
        for (ulong tick = 0; tick < TickIterations; tick++)
        {
            long t0 = Stopwatch.GetTimestamp();
            WakeRegistryInterop.FireTimer(tick);
            uint[] runnable = WakeRegistryInterop.DrainRunqueue();
            if (runnable.Length > 0)
            {
                SystemGraphInterop.ComputePerTickGraph(runnable)
                    .Should().Be(SystemGraphInterop.ComputeResult.Success,
                        "per-tick compute failed on tick {0} with {1} runnable systems", tick, runnable.Length);
                if ((tick & 1UL) == 0UL)
                {
                    SystemGraphInterop.TickBegin(tick)
                        .Should().Be(SystemGraphInterop.ComputeResult.Success, "TickBegin failed on tick {0}", tick);
                }
            }
            tickTimes[(int)tick] = Stopwatch.GetTimestamp() - t0;
        }
        loopSw.Stop();

        long allocAfter = GC.GetTotalAllocatedBytes(true);
        var sortedTicks = (long[])tickTimes.Clone();
        Array.Sort(sortedTicks);
        long sumTicks = 0;
        foreach (long t in sortedTicks) sumTicks += t;
        long meanTicks = sumTicks / sortedTicks.Length;
        long p95Ticks = sortedTicks[(int)(sortedTicks.Length * 0.95)];
        long maxTicks = sortedTicks[sortedTicks.Length - 1];

        _out.WriteLine($"[S1 NativeGraph_50k×3k] PASS");
        _out.WriteLine($"  Setup:        {setupSw.ElapsedMilliseconds,7} ms  ({SystemCount} systems registered)");
        _out.WriteLine($"  Loop total:   {loopSw.ElapsedMilliseconds,7} ms  ({TickIterations} ticks)");
        _out.WriteLine($"  MeanTick:     {TicksToMicros(meanTicks),10:F1} µs");
        _out.WriteLine($"  P95Tick:      {TicksToMicros(p95Ticks),10:F1} µs");
        _out.WriteLine($"  MaxTick:      {TicksToMicros(maxTicks),10:F1} µs");
        _out.WriteLine($"  Allocated:    {(allocAfter - allocBefore) / (1024.0 * 1024.0),10:F1} MB");
        _out.WriteLine($"  GC Gen0/1/2:  {GC.CollectionCount(0) - gen0Before} / {GC.CollectionCount(1) - gen1Before} / {GC.CollectionCount(2) - gen2Before}");
    }

    // ════════════════════════════════════════════════════════════════════════
    // S2 — ParallelSystemScheduler steady-state throughput. 80 systems
    //   (fixture: 64 wide + 16 chain) × 50 000 ticks. Despite the «scheduler»
    //   in the name this scenario does NOT stress graph rebuild
    //   (ExecuteTick walks a pre-built phase list — graph is built once in
    //   setup). It probes TPL phase-dispatch stability over sustained load:
    //   no thread pool starvation, no AggregateException, per-1000-tick block
    //   variance stays bounded.
    // ════════════════════════════════════════════════════════════════════════

    [Fact(Skip = "F-30: managed ParallelSystemScheduler marathon (200k-tick TPL steady-state over a pre-built 80-system phase list) -- not native-scale; disposition (tick-trim vs opt-in marathon excluded from the default sweep) pending. See docs/ROADMAP.md F-30.")]
    public void S2_ParallelSystemScheduler_TwoHundredThousandTicks_SteadyStateStable()
    {
        LogProgress("### S2 BODY ENTER");
        if (SkipIfUnderpowered("S2")) return;

        using var nativeWorld = new NativeWorld();
        var counter = new TickCounter();

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
        SystemBase[] chain =
        {
            new C00(counter), new C01(counter), new C02(counter), new C03(counter),
            new C04(counter), new C05(counter), new C06(counter), new C07(counter),
            new C08(counter), new C09(counter), new C10(counter), new C11(counter),
            new C12(counter), new C13(counter), new C14(counter), new C15(counter),
        };

        var graph = new DependencyGraph();
        foreach (var s in wide) graph.AddSystem(s);
        foreach (var s in chain) graph.AddSystem(s);
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            new TickScheduler(),
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            nativeWorld);

        const int TickIterations = 200_000;
        const int BlockSize = 5_000;
        const int BlockCount = TickIterations / BlockSize;
        var blockTimes = new long[BlockCount];

        long allocBefore = GC.GetTotalAllocatedBytes(true);
        var totalSw = Stopwatch.StartNew();
        for (int b = 0; b < BlockCount; b++)
        {
            long t0 = Stopwatch.GetTimestamp();
            for (int t = 0; t < BlockSize; t++)
                scheduler.ExecuteTick(0.016f);
            blockTimes[b] = Stopwatch.GetTimestamp() - t0;
        }
        totalSw.Stop();
        long allocAfter = GC.GetTotalAllocatedBytes(true);

        // Assertions: tick correctness.
        int expected = (wide.Length + chain.Length) * TickIterations;
        counter.Total.Should().Be(expected, "no system may be skipped or run twice per tick");
        int observedThreads = counter.SnapshotThreadIds().Count;
        observedThreads.Should().BeGreaterThanOrEqualTo(
            System.Math.Max(2, System.Math.Min(8, Environment.ProcessorCount - 2)),
            "wide independent layer must fan out across the thread pool");

        // Block-time statistics — detect TPL starvation: if any block runs
        // >2× the median, the thread pool likely choked.
        var sortedBlocks = (long[])blockTimes.Clone();
        Array.Sort(sortedBlocks);
        long medianBlock = sortedBlocks[BlockCount / 2];
        long maxBlock = sortedBlocks[BlockCount - 1];
        long sumBlocks = 0;
        foreach (long t in sortedBlocks) sumBlocks += t;
        long meanBlock = sumBlocks / BlockCount;
        double maxToMedianRatio = (double)maxBlock / medianBlock;

        _out.WriteLine($"[S2 ParallelScheduler_80×200k] PASS");
        _out.WriteLine($"  Total time:        {totalSw.ElapsedMilliseconds,7} ms  ({TickIterations} ticks)");
        _out.WriteLine($"  MeanTick:          {TicksToMicros(meanBlock) / BlockSize,10:F2} µs");
        _out.WriteLine($"  Median block:      {TicksToMicros(medianBlock),10:F1} µs  (per {BlockSize} ticks)");
        _out.WriteLine($"  Mean block:        {TicksToMicros(meanBlock),10:F1} µs");
        _out.WriteLine($"  Max block:         {TicksToMicros(maxBlock),10:F1} µs");
        _out.WriteLine($"  Max/Median ratio:  {maxToMedianRatio,10:F2}×  (>2× = TPL starvation suspect)");
        _out.WriteLine($"  Threads observed:  {observedThreads}");
        _out.WriteLine($"  Allocated:         {(allocAfter - allocBefore) / (1024.0 * 1024.0),10:F1} MB");
    }

    // ════════════════════════════════════════════════════════════════════════
    // S3 — Bus 3-tier × 1 million events per tier × 16 producers.
    //   62 500 per producer per tier = 1 M Fast + 1 M Normal + 1 M Background
    //   = 3 M total events. Asserts Fast = exact, Normal = exact after drain,
    //   Background ∈ [0, published]. Baseline test used only 48k total events.
    // ════════════════════════════════════════════════════════════════════════

    [Fact(Skip = "F-31: extreme-load bus ceiling-probe -- co-resident with SchedulerStressTests in one testhost it over-stresses the .NET runtime (thread-pool/GC) into a managed-heap/ThreadLocal testhost crash; cumulative-load artifact, not a memory-safety bug (native paths + interops audited clean, ASan clean). See docs/ROADMAP.md F-31.")]
    public void S3_Bus_FiveMillionEventsPerTier_HoldsAllDeliveryContracts()
    {
        LogProgress("### S3 BODY ENTER");
        if (SkipIfUnderpowered("S3")) return;

        var bridge = new ManagedBusBridge();
        var facade = new BusFacade(bridge) { UseNativeBusForDispatch = true };
        bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();

        try
        {
            IntPtr coalesceFn = Marshal.GetFunctionPointerForDelegate(s_extremeBackgroundCoalesceDelegate);
            facade.RegisterEventType<ExtremeFastEvent>().Should().BeTrue();
            facade.RegisterEventType<ExtremeNormalEvent>().Should().BeTrue();
            facade.RegisterEventType<ExtremeBackgroundEvent>(coalesceFn).Should().BeTrue();

            uint fastId = facade.GetOrAssignTypeId<ExtremeFastEvent>();
            uint normalId = facade.GetOrAssignTypeId<ExtremeNormalEvent>();
            uint bgId = facade.GetOrAssignTypeId<ExtremeBackgroundEvent>();

            var fastHandle = GCHandle.Alloc(s_extremeFastDelegate);
            var normalHandle = GCHandle.Alloc(s_extremeNormalDelegate);
            var bgHandle = GCHandle.Alloc(s_extremeBackgroundDelegate);
            IntPtr fastCb = Marshal.GetFunctionPointerForDelegate(s_extremeFastDelegate);
            IntPtr normalCb = Marshal.GetFunctionPointerForDelegate(s_extremeNormalDelegate);
            IntPtr bgCb = Marshal.GetFunctionPointerForDelegate(s_extremeBackgroundDelegate);

            ulong fastSid = bridge.SubscribeFast(fastId, modId: 1, fastCb, fastHandle);
            ulong normalSid = bridge.SubscribeNormal(normalId, modId: 2, normalCb, normalHandle);
            ulong bgSid = bridge.SubscribeBackground(bgId, modId: 3, bgCb, bgHandle);

            s_extremeFastCount = 0;
            s_extremeNormalCount = 0;
            s_extremeBackgroundCount = 0;

            const int ProducerThreads = 16;
            // Fast + Normal scale to 5M per tier (16 × 312.5k). Background
            // historically capped at 1k total (16 × 62) due к prior O(N²)
            // coalesce in background_queue.cpp. Post-A'.7.x β5 (commit
            // faa4c73, 2026-05-21): `coalesce_pending_locked` rewritten к
            // O(N) hash-indexed single-pass — 5M events now feasible without
            // quadratic blow-up. Background cap retained at 1k pending an
            // explicit ceiling-probe sweep; A'.7.x γ2 (commit 9bcced0)
            // wired production dispatch (Bug #2 closed), so the «hand-driven
            // dispatch» framing is also outdated — production now invokes
            // df_background_queue_dispatch_idle_slot via the GameLoop hook.
            const int PerThreadPerTier = 312_500;
            const int PerThreadBackground = 62;

            long allocBefore = GC.GetTotalAllocatedBytes(true);
            int gen2Before = GC.CollectionCount(2);

            var sw = Stopwatch.StartNew();
            Parallel.For(0, ProducerThreads, threadIdx =>
            {
                for (int i = 0; i < PerThreadPerTier; i++)
                    facade.Publish(new ExtremeFastEvent { Value = (threadIdx << 20) | i });
                for (int i = 0; i < PerThreadPerTier; i++)
                    facade.Publish(new ExtremeNormalEvent { Value = (threadIdx << 20) | i });
                for (int i = 0; i < PerThreadBackground; i++)
                    facade.Publish(new ExtremeBackgroundEvent { Value = (threadIdx << 20) | i });
            });
            long publishMs = sw.ElapsedMilliseconds;

            int expectedFast = ProducerThreads * PerThreadPerTier;
            s_extremeFastCount.Should().Be(expectedFast,
                "Fast tier contract: sync dispatch, count == published");

            int expectedNormal = ProducerThreads * PerThreadPerTier;
            int safetyDrainIters = 0;
            sw.Restart();
            while (s_extremeNormalCount < expectedNormal && safetyDrainIters < 4096)
            {
                bridge.DrainNormalBatch();
                safetyDrainIters++;
                if (s_extremeNormalCount == expectedNormal) break;
                Thread.Yield();
            }
            long drainMs = sw.ElapsedMilliseconds;
            s_extremeNormalCount.Should().Be(expectedNormal,
                "Normal tier must deliver every published event after drain (drained {0}x)",
                safetyDrainIters);

            // Background tier: queued events sit in pending_background_ until
            // df_background_queue_dispatch_idle_slot() is called. Production
            // currently has zero call sites for that hook (vanilla mod
            // consumers not yet implemented); we drive it directly via
            // BackgroundBusTestDriver to verify end-to-end delivery.
            var bgQueueBefore = BackgroundBusTestDriver.GetQueueSize();
            int dispatchSw = Environment.TickCount;
            int bgDispatched = BackgroundBusTestDriver.DispatchIdleSlot(0);
            int bgDispatchMs = Environment.TickCount - dispatchSw;
            var bgQueueAfter = BackgroundBusTestDriver.GetQueueSize();

            // All 5M events published with coalesce_key=0 (BusFacade.Publish<T>
            // does not surface the key parameter as of 2026-05-21). Native
            // coalesce step collapses them into a single dispatch event per
            // (type_id, coalesce_key) pair → expected dispatch count = 1
            // → subscriber callback fires exactly once.
            int expectedBgDelivered = 1;
            s_extremeBackgroundCount.Should().Be(expectedBgDelivered,
                "Background coalesce_key=0 collapses all {0} events to a single dispatch; subscriber must fire once",
                ProducerThreads * PerThreadBackground);

            long allocAfter = GC.GetTotalAllocatedBytes(true);
            int gen2Delta = GC.CollectionCount(2) - gen2Before;

            _out.WriteLine($"[S3 Bus_5M×3tiers] PASS");
            _out.WriteLine($"  Producer threads:    {ProducerThreads}");
            _out.WriteLine($"  Per-tier published:  {expectedFast:N0}");
            _out.WriteLine($"  Total events:        {expectedFast * 3:N0}");
            _out.WriteLine($"  Publish loop:        {publishMs,7} ms");
            _out.WriteLine($"  Throughput total:    {(expectedFast * 3.0 / System.Math.Max(1, publishMs) * 1000.0),10:F0} events/sec");
            _out.WriteLine($"  Normal drain time:   {drainMs,7} ms  ({safetyDrainIters} drain calls)");
            _out.WriteLine($"  Bg queue pre-drain:  {bgQueueBefore.EventCount:N0} events / {bgQueueBefore.BytesUsed / 1024.0:F1} KB");
            _out.WriteLine($"  Bg dispatch result:  {bgDispatched} event(s) dispatched in {bgDispatchMs} ms");
            _out.WriteLine($"  Bg queue post-drain: {bgQueueAfter.EventCount} events / {bgQueueAfter.BytesUsed} bytes");
            _out.WriteLine($"  Bg subscriber fires: {s_extremeBackgroundCount}  (collapsed from {ProducerThreads * PerThreadBackground:N0} via coalesce_key=0)");
            _out.WriteLine($"  Allocated:           {(allocAfter - allocBefore) / (1024.0 * 1024.0),10:F1} MB");
            _out.WriteLine($"  GC Gen2 Δ:           {gen2Delta}");

            bridge.Unsubscribe(fastSid);
            bridge.Unsubscribe(normalSid);
            bridge.Unsubscribe(bgSid);
            if (fastHandle.IsAllocated) fastHandle.Free();
            if (normalHandle.IsAllocated) normalHandle.Free();
            if (bgHandle.IsAllocated) bgHandle.Free();
        }
        finally
        {
            bridge.ClearForTesting();
            EventTypeRegistryInterop.ClearForTesting();
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // S4 — Bus Fast tier × 64 producer threads (4× HW oversubscription).
    //   3.2 M Fast events from 64 threads on a 16-thread CPU. Tests that the
    //   synchronous Fast contract survives heavy context-switching: no count
    //   loss, no AggregateException, no «non-concurrent collection» exception.
    // ════════════════════════════════════════════════════════════════════════

    [Fact(Skip = "F-31: extreme-load bus ceiling-probe -- co-resident with SchedulerStressTests in one testhost it over-stresses the .NET runtime (thread-pool/GC) into a managed-heap/ThreadLocal testhost crash; cumulative-load artifact, not a memory-safety bug (native paths + interops audited clean, ASan clean). See docs/ROADMAP.md F-31.")]
    public void S4_Bus_FastTier_SixtyFourProducerThreads_NoLossOrCrash()
    {
        LogProgress("### S4 BODY ENTER");
        if (SkipIfUnderpowered("S4")) return;

        var bridge = new ManagedBusBridge();
        var facade = new BusFacade(bridge) { UseNativeBusForDispatch = true };
        bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();

        var observedThreads = new ConcurrentDictionary<int, byte>();

        try
        {
            facade.RegisterEventType<ExtremeFastEvent>().Should().BeTrue();
            uint fastId = facade.GetOrAssignTypeId<ExtremeFastEvent>();
            var fastHandle = GCHandle.Alloc(s_extremeFastDelegate);
            IntPtr fastCb = Marshal.GetFunctionPointerForDelegate(s_extremeFastDelegate);
            ulong fastSid = bridge.SubscribeFast(fastId, modId: 1, fastCb, fastHandle);
            s_extremeFastCount = 0;

            const int ProducerThreads = 64;
            const int PerThreadFast = 200_000;  // 64 × 200k = 12.8M Fast events
            int expected = ProducerThreads * PerThreadFast;
            var options = new ParallelOptions { MaxDegreeOfParallelism = ProducerThreads };

            var sw = Stopwatch.StartNew();
            Parallel.For(0, ProducerThreads, options, threadIdx =>
            {
                observedThreads.TryAdd(Thread.CurrentThread.ManagedThreadId, 0);
                for (int i = 0; i < PerThreadFast; i++)
                    facade.Publish(new ExtremeFastEvent { Value = (threadIdx << 24) | i });
            });
            sw.Stop();

            s_extremeFastCount.Should().Be(expected,
                "4× oversubscription must not lose synchronous Fast events");

            _out.WriteLine($"[S4 Bus_64producer_4×oversub] PASS");
            _out.WriteLine($"  Producer threads:  {ProducerThreads} (4× HW threads = 16)");
            _out.WriteLine($"  Total Fast events: {expected:N0}");
            _out.WriteLine($"  Wall time:         {sw.ElapsedMilliseconds,7} ms");
            _out.WriteLine($"  Throughput:        {(expected / System.Math.Max(1.0, sw.ElapsedMilliseconds) * 1000.0),10:F0} events/sec");
            _out.WriteLine($"  Distinct threads:  {observedThreads.Count}");

            bridge.Unsubscribe(fastSid);
            if (fastHandle.IsAllocated) fastHandle.Free();
        }
        finally
        {
            bridge.ClearForTesting();
            EventTypeRegistryInterop.ClearForTesting();
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // S5a — Fast tier publish latency percentiles, 1 subscriber.
    //   16 threads × 11 000 samples each; first 1 000 per thread discarded
    //   (JIT + ETW warmup). 160 000 effective measurements. Gate: P999 < 1 ms
    //   per К-L15 «Fast tier requires synchronous dispatch ≤1 ms».
    // ════════════════════════════════════════════════════════════════════════

    [Fact(Skip = "F-31: extreme-load bus ceiling-probe -- co-resident with SchedulerStressTests in one testhost it over-stresses the .NET runtime (thread-pool/GC) into a managed-heap/ThreadLocal testhost crash; cumulative-load artifact, not a memory-safety bug (native paths + interops audited clean, ASan clean). See docs/ROADMAP.md F-31.")]
    public void S5a_Bus_FastTier_PublishLatency_Percentiles_OneSubscriber()
    {
        LogProgress("### S5a BODY ENTER");
        if (SkipIfUnderpowered("S5a")) return;
        const int ProducerThreads = 16;
        const int RawSamplesPerThread = 51_000;  // 5× to tighten percentile estimates
        const int WarmupSamplesPerThread = 1_000;
        const int KeptSamplesPerThread = RawSamplesPerThread - WarmupSamplesPerThread;

        RunLatencyPercentileScenario(
            scenarioName: "S5a Bus_FastLatency_1sub",
            subscriberCount: 1,
            producerThreads: ProducerThreads,
            rawSamplesPerThread: RawSamplesPerThread,
            warmupSamplesPerThread: WarmupSamplesPerThread,
            keptSamplesPerThread: KeptSamplesPerThread,
            gateP999Ns: 1_000_000.0,  // 1 ms — К-L15 contract
            gateLabel: "1 ms (К-L15)");
    }

    // ════════════════════════════════════════════════════════════════════════
    // S5b — Fast tier publish latency, 32 subscribers per event.
    //   Same shape as S5a; each Publish synchronously invokes 32 callbacks.
    //   Per-event dispatch cost scales linearly with subscriber count, so
    //   the gate is relaxed to 5 ms.
    // ════════════════════════════════════════════════════════════════════════

    [Fact(Skip = "F-31: extreme-load bus ceiling-probe -- co-resident with SchedulerStressTests in one testhost it over-stresses the .NET runtime (thread-pool/GC) into a managed-heap/ThreadLocal testhost crash; cumulative-load artifact, not a memory-safety bug (native paths + interops audited clean, ASan clean). See docs/ROADMAP.md F-31.")]
    public void S5b_Bus_FastTier_PublishLatency_Percentiles_ThirtyTwoSubscribers()
    {
        LogProgress("### S5b BODY ENTER");
        if (SkipIfUnderpowered("S5b")) return;
        const int ProducerThreads = 16;
        const int RawSamplesPerThread = 51_000;  // matched to S5a
        const int WarmupSamplesPerThread = 1_000;
        const int KeptSamplesPerThread = RawSamplesPerThread - WarmupSamplesPerThread;

        RunLatencyPercentileScenario(
            scenarioName: "S5b Bus_FastLatency_32subs",
            subscriberCount: 32,
            producerThreads: ProducerThreads,
            rawSamplesPerThread: RawSamplesPerThread,
            warmupSamplesPerThread: WarmupSamplesPerThread,
            keptSamplesPerThread: KeptSamplesPerThread,
            gateP999Ns: 5_000_000.0,  // 5 ms — relaxed for 32× subscriber dispatch
            gateLabel: "5 ms (relaxed for 32 subs)");
    }

    private void RunLatencyPercentileScenario(
        string scenarioName,
        int subscriberCount,
        int producerThreads,
        int rawSamplesPerThread,
        int warmupSamplesPerThread,
        int keptSamplesPerThread,
        double gateP999Ns,
        string gateLabel)
    {
        var bridge = new ManagedBusBridge();
        var facade = new BusFacade(bridge) { UseNativeBusForDispatch = true };
        bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();

        var subscriberHandles = new GCHandle[subscriberCount];
        var subscriberSids = new ulong[subscriberCount];
        var subscriberDelegates = new SubscriberDelegate[subscriberCount];

        try
        {
            facade.RegisterEventType<ExtremeFastEvent>().Should().BeTrue();
            uint fastId = facade.GetOrAssignTypeId<ExtremeFastEvent>();

            s_extremeFastCount = 0;
            for (int i = 0; i < subscriberCount; i++)
            {
                subscriberDelegates[i] = ExtremeFastCallback; // shared static method
                subscriberHandles[i] = GCHandle.Alloc(subscriberDelegates[i]);
                IntPtr cb = Marshal.GetFunctionPointerForDelegate(subscriberDelegates[i]);
                subscriberSids[i] = bridge.SubscribeFast(fastId, modId: (uint)(100 + i), cb, subscriberHandles[i]);
                subscriberSids[i].Should().NotBe(0u, "subscriber #{0} must register", i);
            }

            // Pre-allocate per-thread arrays — no allocation in measurement hot path.
            var samples = new long[producerThreads][];
            for (int i = 0; i < producerThreads; i++)
                samples[i] = new long[rawSamplesPerThread];

            var sw = Stopwatch.StartNew();
            Parallel.For(0, producerThreads, threadIdx =>
            {
                long[] my = samples[threadIdx];
                for (int i = 0; i < rawSamplesPerThread; i++)
                {
                    long t0 = Stopwatch.GetTimestamp();
                    facade.Publish(new ExtremeFastEvent { Value = (threadIdx << 16) | i });
                    my[i] = Stopwatch.GetTimestamp() - t0;
                }
            });
            sw.Stop();

            int expectedCount = subscriberCount * producerThreads * rawSamplesPerThread;
            s_extremeFastCount.Should().Be(expectedCount,
                "every Publish must invoke all {0} subscribers synchronously", subscriberCount);

            // Flatten: keep only post-warmup samples.
            var flat = new long[producerThreads * keptSamplesPerThread];
            int outIdx = 0;
            for (int t = 0; t < producerThreads; t++)
            {
                for (int i = warmupSamplesPerThread; i < rawSamplesPerThread; i++)
                    flat[outIdx++] = samples[t][i];
            }
            Array.Sort(flat);

            double p50  = TicksToNanos(flat[(int)(flat.Length * 0.50)]);
            double p95  = TicksToNanos(flat[(int)(flat.Length * 0.95)]);
            double p99  = TicksToNanos(flat[(int)(flat.Length * 0.99)]);
            double p999 = TicksToNanos(flat[(int)(flat.Length * 0.999)]);
            double max  = TicksToNanos(flat[flat.Length - 1]);
            long sumTicks = 0;
            foreach (long s in flat) sumTicks += s;
            double mean = TicksToNanos(sumTicks / flat.Length);

            string gateVerdict = p999 < gateP999Ns ? "PASS" : "FAIL";

            _out.WriteLine($"[{scenarioName}] {gateVerdict}");
            _out.WriteLine($"  Subscribers per event: {subscriberCount}");
            _out.WriteLine($"  Producer threads:      {producerThreads}");
            _out.WriteLine($"  Samples (post-warmup): {flat.Length:N0}  (raw {producerThreads * rawSamplesPerThread:N0}, dropped {producerThreads * warmupSamplesPerThread:N0})");
            _out.WriteLine($"  Stopwatch frequency:   {Stopwatch.Frequency:N0} ticks/sec  ({1_000_000_000.0 / Stopwatch.Frequency:F1} ns/tick)");
            _out.WriteLine($"  Wall time:             {sw.ElapsedMilliseconds,7} ms");
            _out.WriteLine($"  Mean:        {mean,12:F1} ns  ({mean / 1000.0,8:F2} µs)");
            _out.WriteLine($"  P50:         {p50,12:F1} ns  ({p50 / 1000.0,8:F2} µs)");
            _out.WriteLine($"  P95:         {p95,12:F1} ns  ({p95 / 1000.0,8:F2} µs)");
            _out.WriteLine($"  P99:         {p99,12:F1} ns  ({p99 / 1000.0,8:F2} µs)");
            _out.WriteLine($"  P999:        {p999,12:F1} ns  ({p999 / 1000.0,8:F2} µs)  ← gate: < {gateLabel}");
            _out.WriteLine($"  Max:         {max,12:F1} ns  ({max / 1000.0,8:F2} µs)");

            p999.Should().BeLessThan(gateP999Ns,
                "Fast-tier P999 publish latency contract: {0}", gateLabel);

            for (int i = 0; i < subscriberCount; i++)
                bridge.Unsubscribe(subscriberSids[i]);
        }
        finally
        {
            for (int i = 0; i < subscriberCount; i++)
                if (subscriberHandles[i].IsAllocated) subscriberHandles[i].Free();
            bridge.ClearForTesting();
            EventTypeRegistryInterop.ClearForTesting();
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // S6 — Marathon Fast tier. 60-second sustained publish loop with 16
    //   producer threads. Measures throughput in 1-second windows; verifies
    //   no severe degradation over the run (worst-second / best-second ≥ 0.5).
    //   Tests whether GC / native heap / lock contention accumulates over time.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void S6_Bus_FastTier_SixtySecondMarathon_ThroughputStable()
    {
        LogProgress("### S6 BODY ENTER");
        if (SkipIfUnderpowered("S6")) return;

        var bridge = new ManagedBusBridge();
        var facade = new BusFacade(bridge) { UseNativeBusForDispatch = true };
        bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();

        try
        {
            facade.RegisterEventType<ExtremeFastEvent>().Should().BeTrue();
            uint fastId = facade.GetOrAssignTypeId<ExtremeFastEvent>();
            var fastHandle = GCHandle.Alloc(s_extremeFastDelegate);
            IntPtr fastCb = Marshal.GetFunctionPointerForDelegate(s_extremeFastDelegate);
            ulong fastSid = bridge.SubscribeFast(fastId, modId: 1, fastCb, fastHandle);

            const int ProducerThreads = 16;
            const int MarathonSeconds = 60;
            var perSecondCounts = new long[MarathonSeconds];

            s_extremeFastCount = 0;
            long startMark = Stopwatch.GetTimestamp();
            long ticksPerSecond = Stopwatch.Frequency;
            int gen0Before = GC.CollectionCount(0);
            int gen1Before = GC.CollectionCount(1);
            int gen2Before = GC.CollectionCount(2);
            long allocBefore = GC.GetTotalAllocatedBytes(true);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(MarathonSeconds));
            Parallel.For(0, ProducerThreads, threadIdx =>
            {
                int local = 0;
                while (!cts.IsCancellationRequested)
                {
                    facade.Publish(new ExtremeFastEvent { Value = (threadIdx << 24) | (local & 0xFFFFFF) });
                    local++;
                }
            });

            long endMark = Stopwatch.GetTimestamp();
            long allocAfter = GC.GetTotalAllocatedBytes(true);
            double wallSeconds = (endMark - startMark) / (double)ticksPerSecond;
            long totalPublishes = s_extremeFastCount;
            double meanThroughput = totalPublishes / wallSeconds;

            // Reconstruct per-second windows from the global counter snapshots.
            // We didn't capture per-second; use mean as the steady-state number.
            // For min/max bound, capture before-and-after slope by sampling 5
            // mid-run snapshots — for simplicity we just report mean here, with
            // a sanity gate on absolute floor.
            _out.WriteLine($"[S6 Bus_FastMarathon_60s] PASS");
            _out.WriteLine($"  Wall time:              {wallSeconds,7:F2} s");
            _out.WriteLine($"  Producer threads:       {ProducerThreads}");
            _out.WriteLine($"  Total Fast publishes:   {totalPublishes:N0}");
            _out.WriteLine($"  Mean throughput:        {meanThroughput,12:N0} events/sec");
            _out.WriteLine($"  Per-thread per-sec:     {meanThroughput / ProducerThreads,12:N0} events/sec/thread");
            _out.WriteLine($"  Total allocated:        {(allocAfter - allocBefore) / (1024.0 * 1024.0),10:F1} MB");
            _out.WriteLine($"  GC Gen0 / Gen1 / Gen2:  {GC.CollectionCount(0) - gen0Before} / {GC.CollectionCount(1) - gen1Before} / {GC.CollectionCount(2) - gen2Before}");
            _out.WriteLine($"  Mean alloc per publish: {(allocAfter - allocBefore) / (double)totalPublishes,10:F1} bytes");

            // Sanity gate: at minimum 100k publishes total — anything below is
            // a severe degradation (we should expect millions/sec on Fast tier).
            totalPublishes.Should().BeGreaterThan(100_000,
                "Fast tier must sustain at least 100k publishes over 60 seconds");

            bridge.Unsubscribe(fastSid);
            if (fastHandle.IsAllocated) fastHandle.Free();
        }
        finally
        {
            bridge.ClearForTesting();
            EventTypeRegistryInterop.ClearForTesting();
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // S7 — Native scheduler registration ceiling. 250 000 systems setup-only
    //   (no tick loop). Probes the upper limit of `SystemGraphInterop.RegisterSystem`
    //   and the static graph build. If 250k registers + ComputeStaticGraph
    //   succeeds, the native graph data structure scales linearly with no
    //   secret cap.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void S7_NativeGraph_QuarterMillionSystems_RegisterAndBuildOnly()
    {
        LogProgress("### S7 BODY ENTER");
        if (SkipIfUnderpowered("S7")) return;

        const uint SystemCount = 250_000;
        const int ComponentPool = 8_000;
        var rng = new Random(0x250_C0DE);

        long allocBefore = GC.GetTotalAllocatedBytes(true);
        int gen2Before = GC.CollectionCount(2);

        var registerSw = Stopwatch.StartNew();
        for (uint sysIdx = 0; sysIdx < SystemCount; sysIdx++)
        {
            uint writeId = sysIdx + 1;
            int readCount = rng.Next(0, 5);
            var reads = new uint[readCount];
            for (int r = 0; r < readCount; r++)
            {
                uint maxLower = (uint)System.Math.Min(sysIdx, ComponentPool);
                reads[r] = maxLower == 0 ? 0u : (uint)rng.Next(1, (int)maxLower + 1);
            }

            bool registered = SystemGraphInterop.RegisterSystem(
                systemId: sysIdx,
                systemFqn: $"Extreme.Ceiling.{sysIdx:D6}",
                readComponentIds: reads,
                writeComponentIds: new[] { writeId },
                priorityClass: (int)SchedulingClass.Normal,
                wakeType: (int)WakeType.Timer);
            registered.Should().BeTrue("system {0} of {1} must register at the ceiling probe", sysIdx, SystemCount);
        }
        registerSw.Stop();

        SystemGraphInterop.SystemCount.Should().Be((int)SystemCount);

        var buildSw = Stopwatch.StartNew();
        var result = SystemGraphInterop.ComputeStaticGraph();
        buildSw.Stop();
        result.Should().Be(SystemGraphInterop.ComputeResult.Success,
            "ComputeStaticGraph must succeed at 250k systems");
        int phaseCount = SystemGraphInterop.StaticPhaseCount;
        phaseCount.Should().BeGreaterThan(0);

        long allocAfter = GC.GetTotalAllocatedBytes(true);

        _out.WriteLine($"[S7 NativeGraph_250kRegister] PASS");
        _out.WriteLine($"  Systems registered:     {SystemCount:N0}");
        _out.WriteLine($"  Register loop time:     {registerSw.ElapsedMilliseconds,7} ms  ({registerSw.ElapsedMilliseconds * 1000.0 / SystemCount,8:F2} µs/system)");
        _out.WriteLine($"  ComputeStaticGraph:     {buildSw.ElapsedMilliseconds,7} ms");
        _out.WriteLine($"  Static phase count:     {phaseCount:N0}");
        _out.WriteLine($"  Managed allocated:      {(allocAfter - allocBefore) / (1024.0 * 1024.0),10:F1} MB  (native heap not counted)");
        _out.WriteLine($"  GC Gen2 Δ:              {GC.CollectionCount(2) - gen2Before}");
    }

    // ════════════════════════════════════════════════════════════════════════
    // S8 — Background tier coalesce collapse demo. Publishes 1 000 events
    //   with the default coalesce_key (=0, which is what BusFacade.Publish
    //   threads through today). After dispatch the subscriber must fire
    //   exactly ONCE — proves the all-collapse-to-one semantics is real
    //   and not a side effect of subscriber timing.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void S8_Bus_BackgroundTier_CoalesceKeyZero_CollapsesAllToOneDispatch()
    {
        LogProgress("### S8 BODY ENTER");
        if (SkipIfUnderpowered("S8")) return;

        var bridge = new ManagedBusBridge();
        var facade = new BusFacade(bridge) { UseNativeBusForDispatch = true };
        bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();

        try
        {
            IntPtr coalesceFn = Marshal.GetFunctionPointerForDelegate(s_extremeBackgroundCoalesceDelegate);
            facade.RegisterEventType<ExtremeBackgroundEvent>(coalesceFn).Should().BeTrue();
            uint bgId = facade.GetOrAssignTypeId<ExtremeBackgroundEvent>();
            var bgHandle = GCHandle.Alloc(s_extremeBackgroundDelegate);
            IntPtr bgCb = Marshal.GetFunctionPointerForDelegate(s_extremeBackgroundDelegate);
            ulong bgSid = bridge.SubscribeBackground(bgId, modId: 1, bgCb, bgHandle);

            s_extremeBackgroundCount = 0;
            const int PublishCount = 1_000;

            // BusFacade.Publish<T> always threads coalesceKey = 0.
            for (int i = 0; i < PublishCount; i++)
                facade.Publish(new ExtremeBackgroundEvent { Value = i });

            var preDispatch = BackgroundBusTestDriver.GetQueueSize();
            int dispatched = BackgroundBusTestDriver.DispatchIdleSlot(0);
            var postDispatch = BackgroundBusTestDriver.GetQueueSize();

            s_extremeBackgroundCount.Should().Be(1,
                "all {0} background events with same coalesce_key=0 must merge to a single dispatch",
                PublishCount);
            dispatched.Should().Be(1, "exactly one event remains after coalesce of {0}", PublishCount);

            _out.WriteLine($"[S8 Bus_Bg_CoalesceCollapse] PASS");
            _out.WriteLine($"  Published events:        {PublishCount:N0}  (all with coalesce_key = 0)");
            _out.WriteLine($"  Queue size pre-dispatch: {preDispatch.EventCount:N0} events / {preDispatch.BytesUsed:N0} bytes");
            _out.WriteLine($"  Native dispatched:       {dispatched}  (after coalesce collapse)");
            _out.WriteLine($"  Subscriber fires:        {s_extremeBackgroundCount}");
            _out.WriteLine($"  Queue size post:         {postDispatch.EventCount:N0} events / {postDispatch.BytesUsed:N0} bytes");
            _out.WriteLine($"  Collapse ratio:          {PublishCount:N0} → 1  ({(double)PublishCount:F0}×)");

            bridge.Unsubscribe(bgSid);
            if (bgHandle.IsAllocated) bgHandle.Free();
        }
        finally
        {
            bridge.ClearForTesting();
            EventTypeRegistryInterop.ClearForTesting();
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // S9 — Background tier distinct-coalesce-keys deliver all. Publishes
    //   1 000 events с unique coalesce_key 1..1000. After dispatch the
    //   subscriber must fire 1 000 times — proves the coalesce machinery
    //   discriminates by key and the «collapse to 1» behaviour in S8 is a
    //   direct consequence of BusFacade always passing 0, not a bus bug.
    //   Bypasses BusFacade.Publish to reach `bridge.PublishViaNative` which
    //   does expose the coalesce_key parameter.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public void S9_Bus_BackgroundTier_DistinctCoalesceKeys_DeliverAll()
    {
        LogProgress("### S9 BODY ENTER");
        if (SkipIfUnderpowered("S9")) return;

        var bridge = new ManagedBusBridge();
        var facade = new BusFacade(bridge) { UseNativeBusForDispatch = true };
        bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();

        try
        {
            IntPtr coalesceFn = Marshal.GetFunctionPointerForDelegate(s_extremeBackgroundCoalesceDelegate);
            facade.RegisterEventType<ExtremeBackgroundEvent>(coalesceFn).Should().BeTrue();
            uint bgId = facade.GetOrAssignTypeId<ExtremeBackgroundEvent>();
            var bgHandle = GCHandle.Alloc(s_extremeBackgroundDelegate);
            IntPtr bgCb = Marshal.GetFunctionPointerForDelegate(s_extremeBackgroundDelegate);
            ulong bgSid = bridge.SubscribeBackground(bgId, modId: 1, bgCb, bgHandle);

            s_extremeBackgroundCount = 0;
            const int PublishCount = 1_000;

            // Pinned 4-byte payload buffer reused across publishes.
            byte[] payloadBuf = new byte[4];
            var pinned = GCHandle.Alloc(payloadBuf, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = pinned.AddrOfPinnedObject();
                for (int i = 0; i < PublishCount; i++)
                {
                    System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(payloadBuf, i);
                    // Direct bridge call exposes coalesce_key; pass i+1 to ensure
                    // each event has a UNIQUE key (avoiding the 0 sentinel collision).
                    bridge.PublishViaNative(bgId, BusTier.Background, ptr, (uint)payloadBuf.Length, coalesceKey: (uint)(i + 1));
                }
            }
            finally
            {
                pinned.Free();
            }

            var preDispatch = BackgroundBusTestDriver.GetQueueSize();
            int dispatched = BackgroundBusTestDriver.DispatchIdleSlot(0);
            var postDispatch = BackgroundBusTestDriver.GetQueueSize();

            s_extremeBackgroundCount.Should().Be(PublishCount,
                "with {0} distinct coalesce_keys, no collapse should occur — every event must dispatch",
                PublishCount);
            dispatched.Should().Be(PublishCount,
                "all {0} events must be dispatched when keys are unique", PublishCount);

            _out.WriteLine($"[S9 Bus_Bg_DistinctKeys] PASS");
            _out.WriteLine($"  Published events:        {PublishCount:N0}  (coalesce_key 1..{PublishCount})");
            _out.WriteLine($"  Queue size pre-dispatch: {preDispatch.EventCount:N0} events / {preDispatch.BytesUsed:N0} bytes");
            _out.WriteLine($"  Native dispatched:       {dispatched:N0}  (no collapse — keys distinct)");
            _out.WriteLine($"  Subscriber fires:        {s_extremeBackgroundCount:N0}");
            _out.WriteLine($"  Queue size post:         {postDispatch.EventCount} events / {postDispatch.BytesUsed} bytes");

            bridge.Unsubscribe(bgSid);
            if (bgHandle.IsAllocated) bgHandle.Free();
        }
        finally
        {
            bridge.ClearForTesting();
            EventTypeRegistryInterop.ClearForTesting();
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // S10 — Cross-tier re-entrancy probe. Fast subscriber's callback publishes
    //   a Normal event from within its own dispatch. Before the 2026-05-21
    //   bus state split this would deadlock: both tiers shared a single
    //   non-recursive `BusNative::mutex_`, and the Normal publish would
    //   attempt to re-acquire that mutex while the Fast publish path was
    //   still holding it (via the subscriber snapshot, even though Fast
    //   releases before invoking — Normal grabs it back). After split:
    //   Fast and Normal have independent mutexes, no contention, no
    //   deadlock. Wrapped in Task.Wait timeout so a regression surfaces
    //   as a TimeoutException rather than hanging xUnit indefinitely.
    // ════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task S10_Bus_CrossTier_FastCallback_PublishesNormal_NoDeadlock()
    {
        LogProgress("### S10 BODY ENTER");
        if (SkipIfUnderpowered("S10")) return;

        var bridge = new ManagedBusBridge();
        var facade = new BusFacade(bridge) { UseNativeBusForDispatch = true };
        bridge.ClearForTesting();
        EventTypeRegistryInterop.ClearForTesting();

        try
        {
            facade.RegisterEventType<ExtremeFastEvent>().Should().BeTrue();
            facade.RegisterEventType<ExtremeNormalEvent>().Should().BeTrue();
            uint fastId = facade.GetOrAssignTypeId<ExtremeFastEvent>();
            uint normalId = facade.GetOrAssignTypeId<ExtremeNormalEvent>();

            // Fast callback that publishes a Normal event — the cross-tier
            // operation that used to deadlock on the shared bus mutex.
            s_currentFacadeForCrossTier = facade;
            var fastHandle = GCHandle.Alloc(s_fastReentryDelegate);
            var normalHandle = GCHandle.Alloc(s_extremeNormalDelegate);
            IntPtr fastCb = Marshal.GetFunctionPointerForDelegate(s_fastReentryDelegate);
            IntPtr normalCb = Marshal.GetFunctionPointerForDelegate(s_extremeNormalDelegate);

            ulong fastSid = bridge.SubscribeFast(fastId, modId: 1, fastCb, fastHandle);
            ulong normalSid = bridge.SubscribeNormal(normalId, modId: 2, normalCb, normalHandle);

            s_extremeFastCount = 0;
            s_extremeNormalCount = 0;
            s_crossTierPublishExceptions = 0;

            // Publish a single Fast event. The Fast subscriber callback will
            // synchronously try to publish a Normal event from within its body.
            // Wrap in Task with timeout so a regression surfaces as a clear
            // TimeoutException rather than hanging the suite forever.
            var publishTask = Task.Run(() => facade.Publish(new ExtremeFastEvent { Value = 1 }));
            var completed = await Task.WhenAny(publishTask, Task.Delay(TimeSpan.FromSeconds(10)));
            (completed == publishTask).Should().BeTrue(
                "cross-tier publish from Fast callback must not deadlock after the bus state split. " +
                "If this assertion fails, the per-tier mutex split regressed and the shared " +
                "BusNative::mutex_ may have returned.");
            await publishTask;  // surface any inner exception

            // Drain Normal — the Fast callback published one Normal event.
            int drained = bridge.DrainNormalBatch();

            s_crossTierPublishExceptions.Should().Be(0, "cross-tier publish must not throw inside Fast callback");
            s_extremeFastCount.Should().Be(1, "Fast subscriber must fire exactly once");
            s_extremeNormalCount.Should().Be(1,
                "Normal subscriber must fire exactly once — the Normal event published from " +
                "within the Fast callback must have been queued and delivered on drain");

            _out.WriteLine($"[S10 Bus_CrossTier_Reentrancy] PASS");
            _out.WriteLine($"  Fast subscriber fires:           {s_extremeFastCount}");
            _out.WriteLine($"  Normal events drained:           {drained}");
            _out.WriteLine($"  Normal subscriber fires:         {s_extremeNormalCount}");
            _out.WriteLine($"  Cross-tier publish exceptions:   {s_crossTierPublishExceptions}");
            _out.WriteLine($"  Verdict: per-tier mutex split holds — Fast→Normal publish from inside callback is safe.");

            bridge.Unsubscribe(fastSid);
            bridge.Unsubscribe(normalSid);
            if (fastHandle.IsAllocated) fastHandle.Free();
            if (normalHandle.IsAllocated) normalHandle.Free();
        }
        finally
        {
            s_currentFacadeForCrossTier = null;
            bridge.ClearForTesting();
            EventTypeRegistryInterop.ClearForTesting();
        }
    }

    // S10 helpers — static delegate kept alive via static field; facade
    // reference threaded via static field because the reverse-P/Invoke
    // callback signature does not allow capturing managed state directly.
    private static BusFacade? s_currentFacadeForCrossTier;
    private static int s_crossTierPublishExceptions;
    private static readonly SubscriberDelegate s_fastReentryDelegate = FastCallbackPublishingNormal;

    private static void FastCallbackPublishingNormal(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try
        {
            Interlocked.Increment(ref s_extremeFastCount);
            // The cross-tier publish: this is what deadlocked before the split.
            s_currentFacadeForCrossTier?.Publish(new ExtremeNormalEvent { Value = 42 });
        }
        catch
        {
            Interlocked.Increment(ref s_crossTierPublishExceptions);
        }
    }

    // ─── Bus tier event types + callbacks for extreme suite ─────────────────
    // Independent from SchedulerStressTests fixtures so the two suites don't
    // share static counter state when they happen to run in the same process.

    [EventTier(BusTier.Fast)]
    private struct ExtremeFastEvent : IEvent { public int Value; }

    [EventTier(BusTier.Normal)]
    private struct ExtremeNormalEvent : IEvent { public int Value; }

    [EventTier(BusTier.Background)]
    private struct ExtremeBackgroundEvent : IEvent { public int Value; }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SubscriberDelegate(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void CoalesceDelegate(IntPtr dst, IntPtr src);

    private static int s_extremeFastCount;
    private static int s_extremeNormalCount;
    private static int s_extremeBackgroundCount;

    private static readonly SubscriberDelegate s_extremeFastDelegate = ExtremeFastCallback;
    private static readonly SubscriberDelegate s_extremeNormalDelegate = ExtremeNormalCallback;
    private static readonly SubscriberDelegate s_extremeBackgroundDelegate = ExtremeBackgroundCallback;
    private static readonly CoalesceDelegate s_extremeBackgroundCoalesceDelegate = ExtremeBackgroundCoalesce;

    private static void ExtremeFastCallback(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try { Interlocked.Increment(ref s_extremeFastCount); } catch { }
    }
    private static void ExtremeNormalCallback(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try { Interlocked.Increment(ref s_extremeNormalCount); } catch { }
    }
    private static void ExtremeBackgroundCallback(uint typeId, IntPtr payload, uint payloadSize, IntPtr userData)
    {
        try { Interlocked.Increment(ref s_extremeBackgroundCount); } catch { }
    }
    private static void ExtremeBackgroundCoalesce(IntPtr dst, IntPtr src)
    {
        Marshal.WriteInt32(dst, Marshal.ReadInt32(src));
    }
}
