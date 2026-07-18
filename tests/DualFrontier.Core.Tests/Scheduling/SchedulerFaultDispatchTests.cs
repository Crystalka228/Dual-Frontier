using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduling;

/// <summary>
/// EQ_A1 / M1 — ExecutePhase per-system catch + origin dispatch + ELT §2.3
/// quarantine. Before this cascade ExecutePhase had no catch at all, so any system
/// throw became an AggregateException that unwound to GameLoop.RunLoop and killed
/// the simulation thread — mod and core alike — and stranded the deferred flush
/// (recon REC-R2.2). These tests pin the D2 law: a mod-origin fault is reported to
/// the sink, its mod is quarantined (skipped on later ticks — the immediate half of
/// the ELT §2.3 split), and the phase completes with siblings and the flush intact;
/// a core-origin fault fails fast with a recorded cause. No prior test exercised a
/// system throwing in ExecutePhase (REC-R2.6), so this is first coverage.
///
/// Managed-only: the scheduler and a per-instance NativeWorld are constructed
/// exactly as the existing DeferredEventDeliveryTests do — no process-global native
/// singleton is touched, so no [Collection("SharedNativeSingleton")] is needed
/// (TESTING_STRATEGY §2.8, verified against the DeferredEventDeliveryTests precedent).
/// </summary>
public sealed class SchedulerFaultDispatchTests : IDisposable
{
    private readonly NativeWorld _nw = new();

    public SchedulerFaultDispatchTests()
    {
        SystemExecutionContext.PopContext();
    }

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
        _nw.Dispose();
    }

    [Fact]
    public void ModSystemFault_Contained_SiblingRuns_FaultReported()
    {
        var faulting = new FaultingSystem();
        var counting = new CountingSystem();
        var sink = new RecordingFaultSink();
        var scheduler = BuildScheduler(
            new SystemBase[] { faulting, counting }, Mod(faulting, "test.mod.alpha"), sink, services: null);

        Action act = () => scheduler.ExecuteTick(0.016f);

        act.Should().NotThrow("a mod-origin system fault must be contained, not propagated");
        faulting.Runs.Should().Be(1);
        counting.Runs.Should().Be(1, "the sibling system in the same phase still runs after a contained mod fault");
        sink.Reports.Should().ContainSingle()
            .Which.ModId.Should().Be("test.mod.alpha", "the mod fault must be recorded at the sink");
    }

    [Fact]
    public void ModSystemFault_Quarantined_SkippedOnSubsequentTicks()
    {
        var faulting = new FaultingSystem();
        var counting = new CountingSystem();
        var sink = new RecordingFaultSink();
        var scheduler = BuildScheduler(
            new SystemBase[] { faulting, counting }, Mod(faulting, "test.mod.beta"), sink, services: null);

        scheduler.ExecuteTick(0.016f);
        scheduler.ExecuteTick(0.016f);
        scheduler.ExecuteTick(0.016f);

        faulting.Runs.Should().Be(1, "after faulting once the mod is quarantined and its systems are skipped thereafter");
        counting.Runs.Should().Be(3, "an unaffected sibling keeps running every tick");
        sink.Reports.Should().ContainSingle("the fault is reported once — the skipped ticks never re-enter the system");
    }

    [Fact]
    public void CoreSystemFault_Propagates_SinkEmpty()
    {
        var faulting = new FaultingSystem();
        var sink = new RecordingFaultSink();
        // No metadata entry -> the system defaults to Core origin.
        var scheduler = BuildScheduler(
            new SystemBase[] { faulting }, new Dictionary<SystemBase, SystemMetadata>(), sink, services: null);

        Action act = () => scheduler.ExecuteTick(0.016f);

        act.Should().Throw<AggregateException>("a core-origin system fault must fail fast, as before")
            .WithInnerException<InvalidOperationException>();
        sink.Reports.Should().BeEmpty("a core fault is never routed to the mod-fault sink");
    }

    [Fact]
    public void ModSystemFault_Contained_FlushDeferredStillRuns()
    {
        var faulting = new FaultingSystem();
        var emitter = new ProbeEmitterSystem();
        var listener = new ProbeListenerSystem();
        var sink = new RecordingFaultSink();
        var services = new GameServices();
        var scheduler = BuildScheduler(
            new SystemBase[] { faulting, emitter, listener }, Mod(faulting, "test.mod.gamma"), sink, services);

        scheduler.ExecuteTick(0.016f);

        listener.Received.Should().Be(1,
            "the per-phase flush must still run after a contained mod fault (the fault must not strand queued events)");
        sink.Reports.Should().ContainSingle().Which.ModId.Should().Be("test.mod.gamma");
    }

    [Fact]
    public void Quarantine_InertAfterModSystemLeavesGraph()
    {
        var faulting = new FaultingSystem();
        var counting = new CountingSystem();
        var sink = new RecordingFaultSink();
        var scheduler = BuildScheduler(
            new SystemBase[] { faulting, counting }, Mod(faulting, "test.mod.delta"), sink, services: null);

        scheduler.ExecuteTick(0.016f);
        faulting.Runs.Should().Be(1);

        // Simulate the queued reclamation: the graph is rebuilt without the faulted
        // system (as ModIntegrationPipeline.Apply would evict it at the next menu).
        var survivor = new CountingSystem();
        var g2 = new DependencyGraph();
        g2.AddSystem(survivor);
        g2.Build();
        scheduler.Rebuild(g2.GetPhases(), new Dictionary<SystemBase, SystemMetadata>());

        Action act = () => scheduler.ExecuteTick(0.016f);

        act.Should().NotThrow();
        survivor.Runs.Should().Be(1,
            "once the faulted mod's systems leave the graph the scheduler runs clean; the stale quarantine entry is inert");
    }

    // ── Fixtures ─────────────────────────────────────────────────────────────

    private ParallelSystemScheduler BuildScheduler(
        SystemBase[] systems,
        IReadOnlyDictionary<SystemBase, SystemMetadata> metadata,
        IModFaultSink sink,
        IGameServices? services)
    {
        var graph = new DependencyGraph();
        foreach (SystemBase s in systems)
            graph.AddSystem(s);
        graph.Build();

        return new ParallelSystemScheduler(
            graph.GetPhases(), new TickScheduler(), metadata, sink, _nw, services);
    }

    private static Dictionary<SystemBase, SystemMetadata> Mod(SystemBase system, string modId)
        => new() { [system] = new SystemMetadata(SystemOrigin.Mod, modId) };

    private sealed class RecordingFaultSink : IModFaultSink
    {
        public readonly List<(string ModId, string Message)> Reports = new();

        public void ReportFault(string modId, string message) => Reports.Add((modId, message));
    }

    // ── Test systems (disjoint writes -> a single concurrent phase) ───────────

    public struct FaultMarker : IComponent { }
    public struct NormalMarker : IComponent { }
    public struct EmitMarker : IComponent { }
    public struct ListenMarker : IComponent { }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(FaultMarker) }, bus: "Pawns")]
    internal sealed class FaultingSystem : SystemBase
    {
        public int Runs;

        public override void Update(float delta)
        {
            Runs++;
            throw new InvalidOperationException("system boom");
        }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(NormalMarker) }, bus: "Pawns")]
    internal sealed class CountingSystem : SystemBase
    {
        public int Runs;

        public override void Update(float delta) => Runs++;
    }

    [Deferred]
    internal sealed record ProbeEvent : IEvent;

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(EmitMarker) }, bus: "Pawns")]
    internal sealed class ProbeEmitterSystem : SystemBase
    {
        public override void Update(float delta) => Services.Pawns.Publish(new ProbeEvent());
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ListenMarker) }, bus: "Pawns")]
    internal sealed class ProbeListenerSystem : SystemBase
    {
        public int Received;

        protected override void OnInitialize() => Services.Pawns.Subscribe<ProbeEvent>(_ => Received++);

        public override void Update(float delta) { }
    }
}
