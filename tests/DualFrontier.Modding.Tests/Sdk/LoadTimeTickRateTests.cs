using System;
using System.Collections.Generic;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using AwesomeAssertions;
using Xunit;

// TickRates is declared in both Contracts.Attributes and Core.Scheduling with
// mirrored values; pin the alias to the scheduler-side one these tests assert against.
using TickRates = DualFrontier.Core.Scheduling.TickRates;

namespace DualFrontier.Modding.Tests.Sdk;

/// <summary>
/// F60A_TICK_PATH — the per-behaviour pins for load-time tick-rate resolution.
///
/// <para>
/// The cadence used to be resolved lazily on the tick path and memoised into a
/// <c>ConcurrentDictionary&lt;Type,int&gt;</c> on <c>TickScheduler</c>, whose strong
/// <see cref="Type"/> keys had no eviction and rooted a ticking mod's collectible ALC for the
/// session (F-60(a)). It is now resolved once per system per rebuild by
/// <c>SystemMetadataBuilder</c> and carried in the scheduler's instance-keyed
/// <c>SystemMetadata</c> table, which is swapped wholesale at every load boundary.
/// </para>
///
/// <para>
/// These tests pin the SEMANTICS that moved, so the move cannot silently change behaviour:
/// the normalisation rules (missing / zero / negative -> REALTIME), the adapter forwarding a
/// wrapped SDK system's inner rate rather than its own bridge placeholder, and the
/// absent-from-table fall-through the scheduler documents for empty-metadata test paths.
/// The ALC-release proof itself lives in the Weather wave gate, which runs the production
/// composition end to end.
/// </para>
/// </summary>
public sealed class LoadTimeTickRateTests
{
    // ---- Normalisation: what the builder computes ----

    [Fact]
    public void SystemWithoutTickRate_ResolvesToRealtime()
    {
        SystemMetadata meta = BuildCoreMetadataFor(new NoRateSystem());

        meta.TicksPerUpdate.Should().Be(TickRates.REALTIME,
            "a system that declares no cadence runs every tick -- the same default the old " +
            "TickScheduler.ResolveTicksPerUpdate applied when TickRateDeclaration was null");
    }

    [Fact]
    public void SystemDeclaringZero_ResolvesToRealtime_NotADivideByZero()
    {
        SystemMetadata meta = BuildCoreMetadataFor(new ZeroRateSystem());

        meta.TicksPerUpdate.Should().Be(TickRates.REALTIME,
            "[TickRate] does not validate its argument, so 0 is constructible; normalising it " +
            "here is what keeps the scheduler's `_currentTick % ticksPerUpdate` total");
    }

    [Fact]
    public void SystemDeclaringNegative_ResolvesToRealtime()
    {
        SystemMetadata meta = BuildCoreMetadataFor(new NegativeRateSystem());

        meta.TicksPerUpdate.Should().Be(TickRates.REALTIME,
            "a non-positive rate is meaningless as a cadence and falls back to every tick");
    }

    [Fact]
    public void SystemDeclaringARate_CarriesItVerbatim()
    {
        SystemMetadata meta = BuildCoreMetadataFor(new SlowRateSystem());

        meta.TicksPerUpdate.Should().Be(TickRates.SLOW,
            "a positive declared rate passes through untouched");
    }

    // ---- Adapter forwarding: the case that used to leak ----

    [Fact]
    public void AdapterWrappedSdkSystem_ForwardsItsInnerRate_NotTheBridgePlaceholder()
    {
        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        registry.SetTickSource(() => 0L);
        registry.RegisterSystem("test.mod", typeof(NormalRateSdkSystem));

        IReadOnlyDictionary<SystemBase, SystemMetadata> metadata =
            SystemMetadataBuilder.Build(registry);
        SystemBase adapter = registry.GetAllSystems()
            .Single(r => r.Origin == SystemOrigin.Mod).Instance;

        adapter.GetType().Name.Should().StartWith("SystemAdapter",
            "precondition: an ISimulationSystem is registered through the generic adapter -- " +
            "this is the exact shape whose closed generic Type used to key the leaked cache");

        metadata[adapter].TicksPerUpdate.Should().Be(TickRates.NORMAL,
            "the adapter carries [TickRate(REALTIME)] of its own purely as a DFK013 bridge " +
            "placeholder; the effective cadence is the WRAPPED system's, forwarded through the " +
            "TickRateDeclaration hook. Resolving at load time must not quietly start reading the " +
            "adapter's placeholder instead");
    }

    // ---- Fall-through: the documented empty-table path ----

    [Fact]
    public void SystemAbsentFromTheMetadataTable_RunsEveryTick()
    {
        var system = new CountingSystem();
        var graph = new DependencyGraph();
        graph.AddSystem(system);
        graph.Build();

        using var world = new NativeWorld();
        var ticks = new TickScheduler();
        ParallelSystemScheduler scheduler = SchedulerTestFixture.BuildIsolated(
            graph.GetPhases(), ticks, world);

        for (int i = 0; i < 5; i++)
            scheduler.ExecuteTick(1f / 30f);

        system.Updates.Should().Be(5,
            "ParallelSystemScheduler documents that systems absent from the metadata table fall " +
            "through to Core/null defaults, which covers the scheduler tests that pass an empty " +
            "table. The cadence half of that fall-through is REALTIME, so such a system runs on " +
            "every tick -- exactly as it did when the rate was read from the instance");
    }

    // ---- helpers ----

    private static SystemMetadata BuildCoreMetadataFor(SystemBase system)
    {
        var registry = new ModRegistry();
        registry.SetCoreSystems(new[] { system });
        return SystemMetadataBuilder.Build(registry)[system];
    }

    // Core-origin systems: registered through SetCoreSystems, which applies no
    // [TickRate] validation gate (that gate guards the MOD registration path), so
    // the rate-less and malformed-rate cases are reachable here and nowhere else.

    [SystemAccess(reads: new Type[0], writes: new Type[0])]
    private sealed class NoRateSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new Type[0])]
    [TickRate(0)]
    private sealed class ZeroRateSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new Type[0])]
    [TickRate(-4)]
    private sealed class NegativeRateSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new Type[0])]
    [TickRate(TickRates.SLOW)]
    private sealed class SlowRateSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new Type[0])]
    [TickRate(TickRates.REALTIME)]
    private sealed class CountingSystem : SystemBase
    {
        public int Updates { get; private set; }

        public override void Update(float delta) => Updates++;
    }
}

/// <summary>
/// SDK system used by the adapter-forwarding pin. Public and top-level because
/// <c>ModRegistry.RegisterSystem</c> constructs it reflectively.
/// </summary>
[SystemAccess(reads: new Type[0], writes: new Type[0])]
[TickRate(TickRates.NORMAL)]
public sealed class NormalRateSdkSystem : ISimulationSystem
{
    public void Initialize(ISystemContext context) { }

    public void Tick(ISystemContext context) { }

    public void OnDispose() { }
}
