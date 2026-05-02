using System;
using System.Collections.Concurrent;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Tracks the monotonically increasing game tick counter and decides whether
/// a given system is due to run on the current tick according to its
/// <c>[TickRate]</c> attribute. Consumed by <c>ParallelSystemScheduler</c>
/// to filter systems before each phase.
///
/// The reflection lookup of <see cref="TickRateAttribute"/> is memoised per
/// concrete system type so the hot path of <see cref="ShouldRun"/> does no
/// reflection or allocation after the first call for each type.
///
/// Thread-safety: the type-rate cache uses <see cref="ConcurrentDictionary{TKey,TValue}"/>
/// so parallel system dispatch (which queries <see cref="ShouldRun"/> from
/// inside <c>Parallel.ForEach</c> over a phase's systems) does not race on
/// cache population. The mutable scalar field <c>_currentTick</c> is written
/// only by the scheduler's driver thread between phases via <see cref="Advance"/>
/// and read by <see cref="ShouldRun"/> callers within a phase; that field's
/// single-writer pattern is preserved.
/// </summary>
internal sealed class TickScheduler
{
    private long _currentTick;
    private readonly ConcurrentDictionary<Type, int> _tickRateCache = new();

    /// <summary>
    /// The current tick number. Starts at 0 and is incremented by
    /// <see cref="Advance"/> once per full <c>ExecuteTick</c> call.
    /// </summary>
    public long CurrentTick => _currentTick;

    /// <summary>
    /// Increments the tick counter by 1. Called by
    /// <c>ParallelSystemScheduler.ExecuteTick</c> after every phase in the
    /// current tick has completed, so <see cref="ShouldRun"/> queries during
    /// the tick always see the same value.
    /// </summary>
    public void Advance()
    {
        _currentTick++;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="system"/> should run on the
    /// current tick according to its <c>[TickRate]</c>. Systems without the
    /// attribute default to <see cref="TickRates.REALTIME"/> (every tick).
    /// </summary>
    public bool ShouldRun(SystemBase system)
    {
        if (system is null)
            throw new ArgumentNullException(nameof(system));

        Type systemType = system.GetType();
        int ticksPerUpdate = _tickRateCache.GetOrAdd(systemType, ResolveTicksPerUpdate);

        return _currentTick % ticksPerUpdate == 0;
    }

    /// <summary>
    /// Resets the tick counter to 0 and clears the tick-rate cache. Intended
    /// for test isolation and future mod hot-reload.
    /// </summary>
    public void Reset()
    {
        _currentTick = 0;
        _tickRateCache.Clear();
    }

    private static int ResolveTicksPerUpdate(Type systemType)
    {
        TickRateAttribute? attribute =
            systemType.GetCustomAttribute<TickRateAttribute>(inherit: false);
        if (attribute is null)
            return TickRates.REALTIME;

        int value = attribute.TicksPerUpdate;
        return value > 0 ? value : TickRates.REALTIME;
    }
}
