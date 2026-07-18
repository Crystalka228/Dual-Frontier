using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Bus;

/// <summary>
/// EQ_A1 / M4 — deferred/sync fault-crossing parity under the D2 origin-asymmetric
/// policy (CONCURRENCY_AND_MEMORY_MODEL §7). Before this cascade the two delivery
/// modes were asymmetric: <c>DeliverSync</c> caught every subscriber fault and
/// swallowed it to <c>Console.WriteLine</c> (invisible to the mod-fault lifecycle),
/// while <c>InvokeDeferred</c> had no catch at all (a faulting deferred handler
/// killed the simulation thread). These tests pin the new law in BOTH modes and
/// assert the RECORDING surface (the <see cref="IModFaultSink"/>), not just the
/// propagation shape. No existing test exercised a throwing handler (recon
/// REC-R2.6), so this is first coverage, not a rewrite.
/// </summary>
public sealed class BusFaultIsolationTests : IDisposable
{
    private readonly NativeWorld _nw = new();

    public BusFaultIsolationTests()
    {
        // Defensive: clear any context a prior test leaked on this pool thread.
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
        _nw.Dispose();
    }

    // ── Sync delivery ────────────────────────────────────────────────────────

    [Fact]
    public void SyncModFault_ReportedToSink_RemainingSubscribersDeliver()
    {
        var bus = new DomainEventBus();
        var sink = new RecordingFaultSink();
        bool secondRan = false;

        SubscribeUnder<PlainEvent>(bus, ModContext(sink),
            _ => throw new InvalidOperationException("mod boom (sync)"));
        SubscribeUnder<PlainEvent>(bus, ModContext(sink), _ => secondRan = true);

        Action act = () => bus.Publish(new PlainEvent());

        act.Should().NotThrow("a mod-origin subscriber fault must be contained, not propagated");
        secondRan.Should().BeTrue("delivery must continue to the remaining subscribers after a contained mod fault");
        sink.Reports.Should().ContainSingle()
            .Which.ModId.Should().Be("test.mod.alpha", "the mod fault must be recorded at the sink, not swallowed to console");
    }

    [Fact]
    public void SyncCoreFault_RecordedAndPropagates()
    {
        // Behavior-change pin: the former swallow-everything sync path now records
        // and RETHROWS a core-origin fault (fail-fast). CMM §7 executed, not a regression.
        var bus = new DomainEventBus();
        var sink = new RecordingFaultSink();

        SubscribeUnder<PlainEvent>(bus, CoreContext(sink),
            _ => throw new InvalidOperationException("core boom (sync)"));

        Action act = () => bus.Publish(new PlainEvent());

        act.Should().Throw<InvalidOperationException>().WithMessage("core boom (sync)");
        sink.Reports.Should().BeEmpty("a core-origin fault is not a mod fault and must never reach the mod-fault sink");
    }

    [Fact]
    public void SyncContextlessFault_Propagates()
    {
        // A subscription that captured no system context has no mod identity, so it
        // is treated as core-origin: recorded and rethrown (fail-fast), never swallowed.
        var bus = new DomainEventBus();
        var sink = new RecordingFaultSink();

        SubscribeUnder<PlainEvent>(bus, null,
            _ => throw new InvalidOperationException("context-less boom"));

        Action act = () => bus.Publish(new PlainEvent());

        act.Should().Throw<InvalidOperationException>().WithMessage("context-less boom");
        sink.Reports.Should().BeEmpty();
    }

    [Fact]
    public void SyncModFault_DeliveryOrderPreserved_MiddleSubscriberFaults()
    {
        var bus = new DomainEventBus();
        var sink = new RecordingFaultSink();
        var observed = new List<string>();

        SubscribeUnder<PlainEvent>(bus, ModContext(sink), _ => observed.Add("first"));
        SubscribeUnder<PlainEvent>(bus, ModContext(sink),
            _ => throw new InvalidOperationException("middle boom"));
        SubscribeUnder<PlainEvent>(bus, ModContext(sink), _ => observed.Add("third"));

        bus.Publish(new PlainEvent());

        observed.Should().Equal(
            new[] { "first", "third" },
            "a contained mod fault in the middle must not skip later subscribers");
        sink.Reports.Should().ContainSingle();
    }

    // ── Deferred delivery ────────────────────────────────────────────────────

    [Fact]
    public void DeferredModFault_ReportedToSink_RemainingSubscribersDeliver()
    {
        var bus = new DomainEventBus();
        var sink = new RecordingFaultSink();
        bool secondRan = false;

        SubscribeUnder<DeferredEvent>(bus, ModContext(sink),
            _ => throw new InvalidOperationException("mod boom (deferred)"));
        SubscribeUnder<DeferredEvent>(bus, ModContext(sink), _ => secondRan = true);

        bus.Publish(new DeferredEvent());
        Action flush = () => bus.FlushDeferred();

        flush.Should().NotThrow("a mod-origin deferred handler fault must be contained (the pre-EQ_A1 crash)");
        secondRan.Should().BeTrue("the remaining deferred subscriber must still be dispatched");
        sink.Reports.Should().ContainSingle()
            .Which.ModId.Should().Be("test.mod.alpha");
    }

    [Fact]
    public void DeferredCoreFault_Propagates()
    {
        // InvokeDeferred previously had NO catch; a core-origin deferred fault now
        // records and propagates (fail-fast) rather than being a silent thread kill.
        var bus = new DomainEventBus();
        var sink = new RecordingFaultSink();

        SubscribeUnder<DeferredEvent>(bus, CoreContext(sink),
            _ => throw new InvalidOperationException("core boom (deferred)"));

        bus.Publish(new DeferredEvent());
        Action flush = () => bus.FlushDeferred();

        flush.Should().Throw<InvalidOperationException>().WithMessage("core boom (deferred)");
        sink.Reports.Should().BeEmpty();
    }

    // ── Fixtures ─────────────────────────────────────────────────────────────

    private SystemExecutionContext ModContext(IModFaultSink sink, string modId = "test.mod.alpha")
        => new("test.mod.system", new[] { "Pawns" }, SystemOrigin.Mod, modId, sink, _nw);

    private SystemExecutionContext CoreContext(IModFaultSink sink)
        => new("test.core.system", new[] { "Pawns" }, SystemOrigin.Core, null, sink, _nw);

    private static void SubscribeUnder<TEvent>(
        DomainEventBus bus, SystemExecutionContext? ctx, Action<TEvent> handler)
        where TEvent : IEvent
    {
        if (ctx is not null)
            SystemExecutionContext.PushContext(ctx);
        try
        {
            bus.Subscribe(handler);
        }
        finally
        {
            if (ctx is not null)
                SystemExecutionContext.PopContext();
        }
    }

    private sealed class RecordingFaultSink : IModFaultSink
    {
        public readonly List<(string ModId, string Message)> Reports = new();

        public void ReportFault(string modId, string message) => Reports.Add((modId, message));
    }

    internal sealed record PlainEvent : IEvent;

    [Deferred]
    internal sealed record DeferredEvent : IEvent;
}
