using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
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
/// Exceptions from <c>SystemBase.Update</c> are not caught here — they
/// propagate through <c>Parallel.ForEach</c> as an <c>AggregateException</c>.
/// This is deliberate: during development we want system faults to surface
/// immediately. Future phases introduce <c>ModFaultHandler</c>, which wraps
/// mod-origin systems; Core systems continue to propagate.
///
/// The <c>MaxDegreeOfParallelism</c> follows the documented N-2 rule —
/// reserving one core for Godot's main thread and one for the OS and
/// background work. See <c>docs/THREADING.md</c>.
/// </summary>
internal sealed class ParallelSystemScheduler
{
    private IReadOnlyList<SystemPhase> _phases;
    private readonly TickScheduler _ticks;
    private readonly World _world;
    private readonly IModFaultSink _faultSink;
    private readonly IGameServices? _services;
    private readonly ParallelOptions _parallelOptions;
    private Dictionary<SystemBase, SystemExecutionContext> _contextCache;

    /// <summary>
    /// Creates a scheduler bound to the given phase list, tick clock, and
    /// target world. The <c>MaxDegreeOfParallelism</c> is fixed at
    /// construction time to <c>max(1, ProcessorCount - 2)</c>. Execution
    /// contexts are pre-built for every registered system so the per-tick
    /// hot path does no reflection or allocation.
    /// </summary>
    /// <param name="phases">Phases in execution order as produced by <see cref="DependencyGraph"/>.</param>
    /// <param name="ticks">Tick clock used to filter systems by <c>[TickRate]</c>.</param>
    /// <param name="world">Target world the systems act upon.</param>
    /// <param name="faultSink">Optional sink for mod-origin faults; defaults to a no-op sink.</param>
    /// <param name="services">Optional domain-bus aggregator surfaced to systems via <c>SystemBase.Services</c>; null for tests that never publish.</param>
    public ParallelSystemScheduler(
        IReadOnlyList<SystemPhase> phases,
        TickScheduler ticks,
        World world,
        IModFaultSink? faultSink = null,
        IGameServices? services = null)
    {
        _phases = phases ?? throw new ArgumentNullException(nameof(phases));
        _ticks = ticks ?? throw new ArgumentNullException(nameof(ticks));
        _world = world ?? throw new ArgumentNullException(nameof(world));
        _faultSink = faultSink ?? new NullModFaultSink();
        _services = services;
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = System.Math.Max(1, Environment.ProcessorCount - 2),
        };

        _contextCache = new Dictionary<SystemBase, SystemExecutionContext>();
        foreach (SystemPhase phase in _phases)
        {
            foreach (SystemBase system in phase.Systems)
            {
                if (_contextCache.ContainsKey(system))
                    continue;
                _contextCache[system] = BuildContext(system);
            }
        }

        InitializeAllSystems();
    }

    /// <summary>
    /// Invokes <see cref="SystemBase.Initialize"/> on every registered system
    /// exactly once, with the isolation guard active. This is where systems
    /// subscribe to domain buses via <c>Services</c>, so the execution context
    /// must be pushed for each call — otherwise <c>SystemBase.Services</c>
    /// would throw. Called at the end of the constructor and
    /// <see cref="Rebuild"/>.
    /// </summary>
    private void InitializeAllSystems()
    {
        foreach (SystemPhase phase in _phases)
        {
            foreach (SystemBase system in phase.Systems)
            {
                SystemExecutionContext ctx = _contextCache[system];
                SystemExecutionContext.PushContext(ctx);
                try
                {
                    system.Initialize();
                }
                finally
                {
                    SystemExecutionContext.PopContext();
                }
            }
        }
    }

    /// <summary>
    /// Executes a single phase: every system whose <c>[TickRate]</c> is due
    /// on the current tick runs on a pool thread. <c>Parallel.ForEach</c>
    /// blocks until all systems in the phase have completed, which forms the
    /// phase barrier. After the barrier, deferred events queued during this
    /// phase are dispatched via <see cref="IDeferredFlush.FlushDeferred"/> —
    /// each handler runs with its captured <c>SystemExecutionContext</c>
    /// re-pushed so isolation guarantees hold for cross-system mutations.
    /// </summary>
    public void ExecutePhase(SystemPhase phase, float delta)
    {
        if (phase is null)
            throw new ArgumentNullException(nameof(phase));

        Parallel.ForEach(phase.Systems, _parallelOptions, system =>
        {
            if (!_ticks.ShouldRun(system))
                return;

            SystemExecutionContext ctx = _contextCache[system];
            SystemExecutionContext.PushContext(ctx);
            try
            {
                system.Update(delta);
            }
            finally
            {
                SystemExecutionContext.PopContext();
            }
        });

        if (_services is IDeferredFlush flusher)
            flusher.FlushDeferred();
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

    /// <summary>
    /// Replaces the active phase list with a new one produced after mod
    /// integration. Must be called only from the menu (not during a game
    /// session). The per-system execution-context cache is rebuilt for the
    /// new system set using the same construction rules as the constructor.
    /// </summary>
    /// <param name="newPhases">Phases in execution order as produced by <see cref="DependencyGraph"/>.</param>
    internal void Rebuild(IReadOnlyList<SystemPhase> newPhases)
    {
        if (newPhases is null)
            throw new ArgumentNullException(nameof(newPhases));

        // Сбор новой таблицы контекстов. Алгоритм тот же, что в конструкторе.
        var newCache = new Dictionary<SystemBase, SystemExecutionContext>();
        foreach (SystemPhase phase in newPhases)
        {
            foreach (SystemBase system in phase.Systems)
            {
                if (newCache.ContainsKey(system))
                    continue;
                newCache[system] = BuildContext(system);
            }
        }

        _phases = newPhases;
        _contextCache = newCache;

        InitializeAllSystems();
    }

    /// <summary>
    /// Returns the currently installed phase list. Exposed for tests that
    /// observe the atomic-rebuild contract: on failure the old list must
    /// remain referentially identical.
    /// </summary>
    internal IReadOnlyList<SystemPhase> Phases => _phases;

    private SystemExecutionContext BuildContext(SystemBase system)
    {
        Type systemType = system.GetType();
        SystemAccessAttribute attr =
            systemType.GetCustomAttribute<SystemAccessAttribute>(inherit: false)
            ?? throw new InvalidOperationException(
                $"System '{systemType.FullName}' is missing [SystemAccess]. " +
                "The scheduler cannot build an execution context without a declaration.");

        return new SystemExecutionContext(
            _world,
            systemType.FullName ?? systemType.Name,
            attr.Reads,
            attr.Writes,
            attr.Buses,
            SystemOrigin.Core,
            modId: null,
            _faultSink,
            _services);
    }
}
