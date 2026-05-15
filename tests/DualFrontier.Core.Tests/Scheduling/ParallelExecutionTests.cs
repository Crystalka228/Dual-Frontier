using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduling;

public sealed class ParallelExecutionTests
{
    [Fact]
    public void IndependentSystems_ExecuteOnMultipleThreads()
    {
        if (Environment.ProcessorCount < 2)
            return; // Test requires multi-core; single-core CI cannot observe parallelism.

        using var nativeWorld = new NativeWorld();
        var a = new ThreadRecorderASystem();
        var b = new ThreadRecorderBSystem();
        var c = new ThreadRecorderCSystem();
        var phases = new List<SystemPhase>
        {
            new SystemPhase(new SystemBase[] { a, b, c }),
        };
        var scheduler = new ParallelSystemScheduler(
            phases, new TickScheduler(),
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            nativeWorld);

        for (int i = 0; i < 50; i++)
            scheduler.ExecuteTick(0.016f);

        var allThreads = new HashSet<int>();
        foreach (int tid in a.ObservedThreads) allThreads.Add(tid);
        foreach (int tid in b.ObservedThreads) allThreads.Add(tid);
        foreach (int tid in c.ObservedThreads) allThreads.Add(tid);

        allThreads.Count.Should().BeGreaterThanOrEqualTo(
            2, "independent systems in the same phase must fan out to multiple threads");
    }

    [Fact]
    public void DependentSystems_RunSequentially()
    {
        using var nativeWorld = new NativeWorld();
        var timeline = new TimelineRecorder();
        var producer = new TimestampProducerSystem(timeline, "producer");
        var consumer = new TimestampConsumerSystem(timeline, "consumer");
        var phases = new List<SystemPhase>
        {
            new SystemPhase(new SystemBase[] { producer }),
            new SystemPhase(new SystemBase[] { consumer }),
        };
        var scheduler = new ParallelSystemScheduler(
            phases, new TickScheduler(),
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            nativeWorld);

        scheduler.ExecuteTick(0.016f);

        long producerTs = timeline.GetEndFor("producer");
        long consumerTs = timeline.GetStartFor("consumer");
        consumerTs.Should().BeGreaterThan(
            producerTs,
            "the consumer phase must only start after the producer phase has finished");
    }

    [Fact]
    public void PhaseBarrier_WaitsForAllSystems()
    {
        using var nativeWorld = new NativeWorld();
        var fast = new CompletionCounterSystem(spinMs: 0);
        var slow = new CompletionCounterSystem(spinMs: 50);
        var phases = new List<SystemPhase>
        {
            new SystemPhase(new SystemBase[] { fast, slow }),
        };
        var scheduler = new ParallelSystemScheduler(
            phases, new TickScheduler(),
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullModFaultSink(),
            nativeWorld);

        scheduler.ExecutePhase(phases[0], 0.016f);

        fast.CompletedCount.Should().Be(1);
        slow.CompletedCount.Should().Be(1);
    }

    // ── Test fixture components ─────────────────────────────────────────────

    internal sealed class ThreadRecordingComponentA : IComponent { }
    internal sealed class ThreadRecordingComponentB : IComponent { }
    internal sealed class ThreadRecordingComponentC : IComponent { }
    internal sealed class ProducerComponent : IComponent { }
    internal sealed class ConsumerComponent : IComponent { }
    internal sealed class BarrierComponentFast : IComponent { }

    // ── Test fixture systems ────────────────────────────────────────────────

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ThreadRecordingComponentA) }, bus: "TestBus")]
    internal sealed class ThreadRecorderASystem : SystemBase
    {
        public ConcurrentBag<int> ObservedThreads { get; } = new();
        public override void Update(float delta)
        {
            ObservedThreads.Add(Thread.CurrentThread.ManagedThreadId);
            Thread.SpinWait(100_000);
        }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ThreadRecordingComponentB) }, bus: "TestBus")]
    internal sealed class ThreadRecorderBSystem : SystemBase
    {
        public ConcurrentBag<int> ObservedThreads { get; } = new();
        public override void Update(float delta)
        {
            ObservedThreads.Add(Thread.CurrentThread.ManagedThreadId);
            Thread.SpinWait(100_000);
        }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ThreadRecordingComponentC) }, bus: "TestBus")]
    internal sealed class ThreadRecorderCSystem : SystemBase
    {
        public ConcurrentBag<int> ObservedThreads { get; } = new();
        public override void Update(float delta)
        {
            ObservedThreads.Add(Thread.CurrentThread.ManagedThreadId);
            Thread.SpinWait(100_000);
        }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ProducerComponent) }, bus: "TestBus")]
    internal sealed class TimestampProducerSystem : SystemBase
    {
        private readonly TimelineRecorder _timeline;
        private readonly string _label;
        public TimestampProducerSystem(TimelineRecorder timeline, string label)
        {
            _timeline = timeline;
            _label = label;
        }
        public override void Update(float delta)
        {
            _timeline.RecordStart(_label);
            Thread.SpinWait(500_000);
            _timeline.RecordEnd(_label);
        }
    }

    [SystemAccess(reads: new[] { typeof(ProducerComponent) }, writes: new[] { typeof(ConsumerComponent) }, bus: "TestBus")]
    internal sealed class TimestampConsumerSystem : SystemBase
    {
        private readonly TimelineRecorder _timeline;
        private readonly string _label;
        public TimestampConsumerSystem(TimelineRecorder timeline, string label)
        {
            _timeline = timeline;
            _label = label;
        }
        public override void Update(float delta)
        {
            _timeline.RecordStart(_label);
            Thread.SpinWait(500_000);
            _timeline.RecordEnd(_label);
        }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(BarrierComponentFast) }, bus: "TestBus")]
    internal sealed class CompletionCounterSystem : SystemBase
    {
        private readonly int _spinMs;
        private int _completed;
        public int CompletedCount => _completed;

        public CompletionCounterSystem(int spinMs)
        {
            _spinMs = spinMs;
        }

        public override void Update(float delta)
        {
            if (_spinMs > 0)
                Thread.Sleep(_spinMs);
            Interlocked.Increment(ref _completed);
        }
    }

    // ── Helper ──────────────────────────────────────────────────────────────

    internal sealed class TimelineRecorder
    {
        private readonly ConcurrentDictionary<string, (long start, long end)> _entries = new();

        public void RecordStart(string label)
        {
            long ts = Stopwatch.GetTimestamp();
            _entries.AddOrUpdate(label, (ts, 0), (_, existing) => (ts, existing.end));
        }

        public void RecordEnd(string label)
        {
            long ts = Stopwatch.GetTimestamp();
            _entries.AddOrUpdate(label, (0, ts), (_, existing) => (existing.start, ts));
        }

        public long GetStartFor(string label) => _entries[label].start;
        public long GetEndFor(string label) => _entries[label].end;
    }
}
