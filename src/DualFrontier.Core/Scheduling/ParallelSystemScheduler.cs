using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;

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
/// Exceptions from <c>SystemBase.Update</c> are caught per-system under the D2
/// origin-asymmetric policy (CONCURRENCY_AND_MEMORY_MODEL §7, via the single
/// <see cref="SystemExecutionContext.RouteFault"/> definition): a mod-origin
/// fault is reported to the <see cref="IModFaultSink"/> and its mod is committed
/// to the <see cref="ModQuarantine"/> skip-set, so that mod's systems are skipped
/// on every subsequent tick this session — the immediate half of the
/// ENGINE_LIFECYCLE_AND_TRANSACTIONS §2.3 split (the queued unload chain still
/// runs at the next menu open via <c>ModIntegrationPipeline.Apply</c>). The phase
/// completes, sibling systems run, and <c>FlushDeferred</c> still runs. A
/// core-origin fault is recorded and rethrown: it unwinds through
/// <c>Parallel.ForEach</c> as an <c>AggregateException</c> to fail fast — as
/// before, but now with a recorded cause. Fail-fast for core bugs holds in both
/// DEBUG and RELEASE (ELT §4, fault class 1).
///
/// The <c>MaxDegreeOfParallelism</c> follows the documented N-2 rule —
/// leaving headroom for the presentation main thread (Launcher render loop)
/// and for the OS and background work. See
/// <c>docs/architecture/THREADING.md</c> (managed dispatch facade).
/// </summary>
internal sealed class ParallelSystemScheduler
{
    private IReadOnlyList<SystemPhase> _phases;
    private readonly TickScheduler _ticks;
    private readonly IModFaultSink _faultSink;
    private readonly IGameServices? _services;
    private readonly NativeWorld _nativeWorld;
    // K8.3+K8.4 — Path β resolver passed by GameBootstrap (ModRegistry
    // implements IManagedStorageResolver). Null in tests + builds without
    // mod loading; system-side SystemBase.ManagedStore<T>() returns null
    // when this is null.
    private readonly IManagedStorageResolver? _managedStorageResolver;
    private readonly ParallelOptions _parallelOptions;
    private Dictionary<SystemBase, SystemExecutionContext> _contextCache;
    private IReadOnlyDictionary<SystemBase, SystemMetadata> _systemMetadata;
    // EQ_A1 / M1 — faulted-mod skip-set (ELT §2.3 immediate quarantine). A mod
    // system that faults commits its modId here; ExecutePhase then skips that
    // mod's systems on subsequent ticks. Session-lived and NOT reset by Rebuild,
    // so a mod that faulted before a menu rebuild stays quarantined until its
    // systems are actually evicted from the graph.
    private readonly ModQuarantine _quarantine = new();

    /// <summary>
    /// EQ_A2 / M7 — optional observer fired on the FIRST quarantine of a mod
    /// (modId, tickId). The Application wires this to <c>EngineSession.ReportDegraded</c>
    /// so a quarantined mod surfaces as a Degraded reason (ELT §4.1). Invoked from
    /// inside the parallel phase fan-out, so the handler must be thread-safe.
    /// </summary>
    internal Action<string, long>? OnModQuarantined { get; set; }

    /// <summary>
    /// Creates a scheduler bound to the given phase list, tick clock, and
    /// native world. The <c>MaxDegreeOfParallelism</c> is fixed at
    /// construction time to <c>max(1, ProcessorCount - 2)</c>. Execution
    /// contexts are pre-built for every registered system so the per-tick
    /// hot path does no reflection or allocation.
    /// </summary>
    /// <param name="phases">Phases in execution order as produced by <see cref="DependencyGraph"/>.</param>
    /// <param name="ticks">Tick clock used to filter systems by <c>[TickRate]</c>.</param>
    /// <param name="systemMetadata">Per-system <see cref="SystemMetadata"/> table the scheduler reads in <c>BuildContext</c> for origin/modId propagation. Systems absent from the table fall through to <c>Core/null</c> defaults — covers core systems registered via local arrays in tests where the table is empty.</param>
    /// <param name="faultSink">Sink for mod-origin faults; required (no silent default). Tests that never produce faults pass <c>new NullModFaultSink()</c> explicitly.</param>
    /// <param name="nativeWorld">Native world handle the systems act upon. Required post-K8.3+K8.4 cutover — sole production component-storage backend.</param>
    /// <param name="services">Optional domain-bus aggregator surfaced to systems via <c>SystemBase.Services</c>; null for tests that never publish.</param>
    /// <param name="managedStorageResolver">K8.3+K8.4 — optional Path β resolver. Production passes the ModRegistry (which implements <see cref="IManagedStorageResolver"/>); tests and builds without mod loading pass null and <c>SystemBase.ManagedStore&lt;T&gt;()</c> returns null.</param>
    public ParallelSystemScheduler(
        IReadOnlyList<SystemPhase> phases,
        TickScheduler ticks,
        IReadOnlyDictionary<SystemBase, SystemMetadata> systemMetadata,
        IModFaultSink faultSink,
        NativeWorld nativeWorld,
        IGameServices? services = null,
        IManagedStorageResolver? managedStorageResolver = null)
    {
        _phases = phases ?? throw new ArgumentNullException(nameof(phases));
        _ticks = ticks ?? throw new ArgumentNullException(nameof(ticks));
        _systemMetadata = systemMetadata ?? throw new ArgumentNullException(nameof(systemMetadata));
        _faultSink = faultSink ?? throw new ArgumentNullException(nameof(faultSink));
        _nativeWorld = nativeWorld ?? throw new ArgumentNullException(nameof(nativeWorld));
        _services = services;
        _managedStorageResolver = managedStorageResolver;
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

            // EQ_A1 / M1 — ELT §2.3 immediate quarantine: a mod whose system
            // already faulted this session is skipped (its unload is queued for the
            // next menu open). Consulted here, before the context push.
            if (IsQuarantined(system))
                return;

            SystemExecutionContext ctx = _contextCache[system];
            SystemExecutionContext.PushContext(ctx);
            try
            {
                system.Update(delta);
            }
            catch (Exception ex)
            {
                // D2 origin dispatch (CONCURRENCY_AND_MEMORY_MODEL §7, via the single
                // RouteFault definition): mod-origin -> reported to the sink and
                // committed to the quarantine skip-set, then this iteration ends and
                // the phase carries on (siblings run, the flush runs); core-origin ->
                // recorded and rethrown, unwinding as an AggregateException to fail
                // fast (as before, now with a cause). The finally still pops.
                if (ctx.RouteFault(ex, out string? modId) == FaultDisposition.RethrowCore)
                    throw;
                if (_quarantine.Quarantine(modId!))
                    OnModQuarantined?.Invoke(modId!, (long)_ticks.CurrentTick);
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
    /// <param name="newSystemMetadata">Per-system metadata snapshot reflecting the post-mod-load registry state. Swaps in lockstep with the new phase list so context construction reads the correct origin/modId from tick 0 of the new graph.</param>
    internal void Rebuild(
        IReadOnlyList<SystemPhase> newPhases,
        IReadOnlyDictionary<SystemBase, SystemMetadata> newSystemMetadata)
    {
        if (newPhases is null)
            throw new ArgumentNullException(nameof(newPhases));
        if (newSystemMetadata is null)
            throw new ArgumentNullException(nameof(newSystemMetadata));

        // K6.1 — install the new metadata before BuildContext runs so the
        // freshly constructed contexts read each mod system's actual origin
        // and modId. The phase list and context cache swap at the bottom of
        // this method, atomically with metadata.
        _systemMetadata = newSystemMetadata;

        // Build a fresh context table. Same algorithm as in the constructor.
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

    // EQ_A1 / M1 — quarantine consult for ExecutePhase: a system is skipped when
    // its owning mod (per the metadata table BuildContext reads) is in the
    // ModQuarantine set. Core systems (no modId) are never quarantined.
    private bool IsQuarantined(SystemBase system)
        => _systemMetadata.TryGetValue(system, out SystemMetadata? meta)
           && meta.ModId is not null
           && _quarantine.IsQuarantined(meta.ModId);

    private SystemExecutionContext BuildContext(SystemBase system)
    {
        Type systemType = system.GetType();
        // Read via the SystemBase hook (not systemType directly) so a wrapped
        // SDK system (SystemAdapter<T>) forwards its inner [SystemAccess] (W1 BD-1).
        // F-54 (W2): [SystemAccess] presence is still required -- the scheduler cannot build a
        // context without it, and DependencyGraph consumes its Reads/Writes for edge-building.
        // The bus leg formerly read here (attr.Buses -> the retired _allowedBuses) is gone, so the
        // declaration is validated for presence but no longer bound into the execution context.
        SystemAccessAttribute? attr = system.AccessDeclaration;
        if (attr is null)
            throw new InvalidOperationException(
                $"System '{systemType.FullName}' is missing [SystemAccess]. " +
                "The scheduler cannot build an execution context without a declaration.");

        // K6.1 — read origin and modId from the metadata table provided at
        // construction. Systems not present in the table default to Core/null
        // (covers core systems registered via local arrays in tests, and any
        // future system path that doesn't go through ModRegistry). Mod systems
        // registered through ModRegistry carry their owning modId so the
        // context can resolve their Path β managed stores
        // (SystemExecutionContext.ResolveManagedStore<T>) and attribute the
        // system to the right mod.
        SystemOrigin origin = SystemOrigin.Core;
        string? modId = null;
        if (_systemMetadata.TryGetValue(system, out SystemMetadata? meta))
        {
            origin = meta.Origin;
            modId = meta.ModId;
        }

        return new SystemExecutionContext(
            systemType.FullName ?? systemType.Name,
            origin,
            modId,
            _faultSink,
            _nativeWorld,
            _services,
            _managedStorageResolver);
    }
}
