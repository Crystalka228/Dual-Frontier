using System.Collections.Generic;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;

namespace DualFrontier.Modding.Tests.Fixtures;

/// <summary>
/// K6.1 — helper for tests that construct
/// <see cref="ParallelSystemScheduler"/> directly. Defaults the new
/// non-optional <c>faultSink</c> parameter to <see cref="NullModFaultSink"/>
/// and the <c>systemMetadata</c> table to an empty dictionary so existing
/// non-fault tests can keep their original positional ctor call shape with
/// minimal boilerplate. Tests that exercise fault routing override
/// <c>faultSink</c> with a real <see cref="ModFaultHandler"/> and
/// (when relevant) populate the metadata table.
/// </summary>
internal static class SchedulerTestFixture
{
    public static ParallelSystemScheduler BuildIsolated(
        IReadOnlyList<SystemPhase> phases,
        TickScheduler ticks,
        World world,
        IModFaultSink? faultSink = null,
        IGameServices? services = null,
        IReadOnlyDictionary<SystemBase, SystemMetadata>? systemMetadata = null)
    {
        return new ParallelSystemScheduler(
            phases,
            ticks,
            world,
            systemMetadata ?? new Dictionary<SystemBase, SystemMetadata>(),
            faultSink ?? new NullModFaultSink(),
            services);
    }
}
