using System.Collections.Generic;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;

namespace DualFrontier.Modding.Tests.Fixtures;

/// <summary>
/// K6.1 + K8.3+K8.4 — helper for tests that construct
/// <see cref="ParallelSystemScheduler"/> directly. Defaults the
/// <c>faultSink</c> parameter to <see cref="NullModFaultSink"/>, the
/// <c>systemMetadata</c> table to an empty dictionary, and (post-cutover)
/// constructs a throwaway <see cref="NativeWorld"/> when none is supplied
/// so existing tests that only need the scheduler shape can keep their
/// minimal call sites. Tests that exercise fault routing override
/// <c>faultSink</c> with a real <see cref="ModFaultHandler"/> and
/// (when relevant) populate the metadata table.
/// </summary>
internal static class SchedulerTestFixture
{
    public static ParallelSystemScheduler BuildIsolated(
        IReadOnlyList<SystemPhase> phases,
        TickScheduler ticks,
        NativeWorld? nativeWorld = null,
        IModFaultSink? faultSink = null,
        IGameServices? services = null,
        IReadOnlyDictionary<SystemBase, SystemMetadata>? systemMetadata = null)
    {
        return new ParallelSystemScheduler(
            phases,
            ticks,
            systemMetadata ?? new Dictionary<SystemBase, SystemMetadata>(),
            faultSink ?? new NullModFaultSink(),
            nativeWorld ?? new NativeWorld(),
            services);
    }
}
