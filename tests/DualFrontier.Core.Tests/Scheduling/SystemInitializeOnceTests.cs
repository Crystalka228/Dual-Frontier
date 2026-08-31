using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Scheduling;

/// <summary>
/// W4 / D12 — <c>SystemBase.Initialize</c> is once-per-instance.
///
/// <para>
/// <c>ParallelSystemScheduler.InitializeAllSystems</c> runs from the constructor AND from every
/// <c>Rebuild</c>, and a rebuild happens on every mod-set change, so before this guard a system
/// that survived a rebuild had <c>OnInitialize</c> invoked again. The scheduler's own doc claimed
/// "exactly once", which the code never implemented.
/// </para>
///
/// <para>
/// The defect had ZERO production exposure at the time it was fixed, because the only paths to
/// <c>Rebuild</c> run through <c>ModMenuController.Commit</c> and production never opened the mod
/// menu. W4 creates the exposure by loading a root mod set at boot, so the wave that makes it
/// reachable is the wave that fixes it. It was survivable in tests only because
/// <c>DomainEventBus.Subscribe</c> de-duplicates by delegate value equality — an unrelated
/// invariant in another class, which does not cover a lambda handler and covers no
/// non-subscription work at all.
/// </para>
///
/// <para>
/// No test anywhere asserted an <c>Initialize</c>/<c>OnInitialize</c> invocation count before this
/// file, so the guard shipped with no oracle. These two facts are that oracle: the first pins the
/// guard, the second pins that the guard did not turn into a blanket "never initialize again" —
/// a system entering the graph for the first time at a rebuild must still be initialized.
/// </para>
///
/// Managed-only: a per-instance <c>NativeWorld</c>, exactly as
/// <c>SchedulerFaultDispatchTests</c> does — no process-global native singleton is touched, so no
/// <c>[Collection("SharedNativeSingleton")]</c> is needed (TESTING_STRATEGY §2.8).
/// </summary>
public sealed class SystemInitializeOnceTests : IDisposable
{
    private readonly NativeWorld _nw = new();

    public SystemInitializeOnceTests() => SystemExecutionContext.PopContext();

    public void Dispose()
    {
        SystemExecutionContext.PopContext();
        _nw.Dispose();
    }

    [Fact]
    public void Rebuild_DoesNotReInitialiseASystemThatSurvivesTheGraph()
    {
        var survivor = new CountingInitSystem();
        ParallelSystemScheduler scheduler = BuildScheduler(survivor);

        survivor.Initialised.Should().Be(1, "the scheduler constructor initialises the graph once");

        // The shape ModIntegrationPipeline.Apply produces: a fresh graph that still contains the
        // same instance, handed to Rebuild. Before the guard this drove Initialised to 2.
        scheduler.Rebuild(GraphOf(survivor).GetPhases(), new Dictionary<SystemBase, SystemMetadata>());

        survivor.Initialised.Should().Be(1,
            "SystemBase.Initialize latches per instance, so a system that survives a rebuild is " +
            "not initialised again — otherwise every mod-set change would re-run OnInitialize, " +
            "duplicating subscriptions and any other setup work");
    }

    [Fact]
    public void Rebuild_StillInitialisesASystemEnteringTheGraphForTheFirstTime()
    {
        var incumbent = new CountingInitSystem();
        ParallelSystemScheduler scheduler = BuildScheduler(incumbent);

        var newcomer = new OtherCountingInitSystem();
        var graph = new DependencyGraph();
        graph.AddSystem(incumbent);
        graph.AddSystem(newcomer);
        graph.Build();

        scheduler.Rebuild(graph.GetPhases(), new Dictionary<SystemBase, SystemMetadata>());

        newcomer.Initialised.Should().Be(1,
            "the guard is per-instance, not a blanket suppression: a system joining the graph at " +
            "a rebuild — which is exactly what loading a mod does — must still be initialised");
        incumbent.Initialised.Should().Be(1, "and the incumbent is still not re-initialised");
    }

    // ── Fixtures ─────────────────────────────────────────────────────────────

    private ParallelSystemScheduler BuildScheduler(SystemBase system)
        => new(
            GraphOf(system).GetPhases(),
            new TickScheduler(),
            new Dictionary<SystemBase, SystemMetadata>(),
            new NullFaultSink(),
            _nw,
            new GameServices());

    private static DependencyGraph GraphOf(SystemBase system)
    {
        var graph = new DependencyGraph();
        graph.AddSystem(system);
        graph.Build();
        return graph;
    }

    private sealed class NullFaultSink : IModFaultSink
    {
        public void ReportFault(string modId, string message) { }
    }

    public struct InitMarker : IComponent { }
    public struct OtherInitMarker : IComponent { }

    /// <summary>
    /// Counts invocations directly rather than subscribing to a bus. A subscription would be
    /// de-duplicated by <c>DomainEventBus</c> and would therefore pass even with the guard
    /// removed — the counter is what makes the deliberate break observable.
    /// </summary>
    [SystemAccess(reads: new Type[0], writes: new[] { typeof(InitMarker) })]
    internal sealed class CountingInitSystem : SystemBase
    {
        public int Initialised;

        protected override void OnInitialize() => Initialised++;

        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(OtherInitMarker) })]
    internal sealed class OtherCountingInitSystem : SystemBase
    {
        public int Initialised;

        protected override void OnInitialize() => Initialised++;

        public override void Update(float delta) { }
    }
}
