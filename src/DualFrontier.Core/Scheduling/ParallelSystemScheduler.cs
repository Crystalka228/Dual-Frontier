using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Parallel system scheduler. Consumes the ordered phase list produced by
/// <see cref="DependencyGraph"/> and executes them sequentially: within a
/// phase, systems run concurrently via <c>Parallel.ForEach</c>; between
/// phases, the blocking semantics of <c>Parallel.ForEach</c> provide an
/// implicit barrier.
///
/// Per-system lifecycle inside a phase:
///   1. <see cref="TickScheduler.ShouldRun"/> filters out systems not due on
///      this tick according to their <c>[TickRate]</c>.
///   2. <c>SystemExecutionContext.PushContext</c> establishes the isolation
///      guard on the current thread.
///   3. <c>SystemBase.Update</c> runs.
///   4. <c>SystemExecutionContext.PopContext</c> clears the guard — always,
///      even if <c>Update</c> throws.
///
/// Exceptions from <c>SystemBase.Update</c> are not caught here — in Phase 1
/// they propagate through <c>Parallel.ForEach</c> as an
/// <c>AggregateException</c>. This is deliberate: during development we want
/// system faults to surface immediately. Phase 2 introduces
/// <c>ModFaultHandler</c>, which wraps mod-origin systems; Core systems
/// continue to propagate.
///
/// The <c>MaxDegreeOfParallelism</c> follows the documented N-2 rule —
/// reserving one core for Godot's main thread and one for the OS and
/// background work. See <c>docs/THREADING.md</c>.
/// </summary>
internal sealed class ParallelSystemScheduler
{
    private readonly IReadOnlyList<SystemPhase> _phases;
    private readonly TickScheduler _ticks;
    private readonly ParallelOptions _parallelOptions;

    /// <summary>
    /// Creates a scheduler bound to the given phase list and tick clock.
    /// The <c>MaxDegreeOfParallelism</c> is fixed at construction time to
    /// <c>max(1, ProcessorCount - 2)</c>.
    /// </summary>
    public ParallelSystemScheduler(
        IReadOnlyList<SystemPhase> phases,
        TickScheduler ticks)
    {
        _phases = phases ?? throw new ArgumentNullException(nameof(phases));
        _ticks = ticks ?? throw new ArgumentNullException(nameof(ticks));
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = System.Math.Max(1, Environment.ProcessorCount - 2),
        };
    }

    /// <summary>
    /// Executes a single phase: every system whose <c>[TickRate]</c> is due
    /// on the current tick runs on a pool thread. <c>Parallel.ForEach</c>
    /// blocks until all systems in the phase have completed, which forms the
    /// phase barrier.
    /// </summary>
    public void ExecutePhase(SystemPhase phase, float delta)
    {
        if (phase is null)
            throw new ArgumentNullException(nameof(phase));

        Parallel.ForEach(phase.Systems, _parallelOptions, system =>
        {
            if (!_ticks.ShouldRun(system))
                return;

            SystemExecutionContext.PushContext(system);
            try
            {
                system.Update(delta);
            }
            finally
            {
                SystemExecutionContext.PopContext();
            }
        });
    }

    /// <summary>
    /// Executes one full game tick: every phase in order with the current
    /// tick value, then advances the tick counter. The first call executes
    /// on tick 0; subsequent calls execute on tick 1, 2, and so on.
    /// </summary>
    public void ExecuteTick(float delta)
    {
        foreach (SystemPhase phase in _phases)
            ExecutePhase(phase, delta);

        _ticks.Advance();
    }
}
