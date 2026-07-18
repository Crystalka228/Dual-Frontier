using System;
using System.Threading;
using DualFrontier.Application.Bus;
using DualFrontier.Application.Modding;
using DualFrontier.Application.Scheduler;
using DualFrontier.Core.Bus;
using DualFrontier.Core.Interop;

namespace DualFrontier.Application.Loop;

/// <summary>
/// Ordered steps of the world-shutdown transaction, surfaced to the optional
/// <see cref="ShutdownTransactionHooks.OnStep"/> recorder so tests can assert the
/// sequence (EQ_A2 / К-L20 falsifiability (b): a teardown-order assertion must
/// fail if any step runs before the fence passes).
/// </summary>
internal enum ShutdownStep
{
    FencePassed,
    DeferredDropped,
    ModsUnloaded,
    NativeSchedulerCleared,
    NativeBusCleared,
    WorldDisposed,
    Aborted,
}

/// <summary>
/// Structured diagnostics recorded when the shutdown transaction aborts: either
/// the fence failed (EQ_A2 / D4 -- thread not stopped or pipeline not quiescent),
/// or the CHECKED world teardown refused because borrows were still outstanding
/// AFTER a passed fence (EQ_A3 / К-L20 -- a quiescence-invariant violation).
/// </summary>
/// <param name="ThreadStopped">Whether the sim thread observed cancellation and exited within the fence deadline.</param>
/// <param name="PipelineQuiescent">Whether the compute pipeline reached quiescence within the fence deadline.</param>
/// <param name="FenceDeadline">The bounded-join deadline that was applied.</param>
/// <param name="WorldBusy">True when the abort is an S7 checked-destroy refusal (spans/batches outstanding after a passed fence).</param>
/// <param name="ActiveSpans">Live component spans reported by the refusal (0 for a fence abort).</param>
/// <param name="ActiveBatches">Live write batches reported by the refusal (0 for a fence abort).</param>
internal sealed record ShutdownAbortReport(
    bool ThreadStopped,
    bool PipelineQuiescent,
    TimeSpan FenceDeadline,
    bool WorldBusy = false,
    int ActiveSpans = 0,
    int ActiveBatches = 0)
{
    /// <summary>Busy-world abort AFTER a passed fence (EQ_A3 / К-L20): live borrows despite proven quiescence.</summary>
    public static ShutdownAbortReport ForBusyWorld(int activeSpans, int activeBatches, TimeSpan fenceDeadline)
        => new(ThreadStopped: true, PipelineQuiescent: true, fenceDeadline,
               WorldBusy: true, ActiveSpans: activeSpans, ActiveBatches: activeBatches);

    public string Describe() =>
        WorldBusy
            ? $"EngineSession shutdown ABORTED at S7 (checked world-destroy REFUSED): {ActiveSpans} span(s) and " +
              $"{ActiveBatches} write-batch(es) still outstanding AFTER a passed fence -- a К-L20 quiescence-invariant " +
              "violation (live borrows despite proven quiescence). Failing fast WITHOUT forcing teardown; a forced destroy " +
              "here would be use-after-free (RESOURCE_OWNERSHIP_AND_LIFETIME section 6.2; К-L20)."
            : $"EngineSession shutdown fence FAILED (thread stopped={ThreadStopped}, pipeline quiescent={PipelineQuiescent}, " +
              $"deadline={FenceDeadline}); aborting WITHOUT native teardown -- dismantling native state under a live simulation " +
              "thread is worse than leaking on abort (CONCURRENCY_AND_MEMORY_MODEL section 6.2; К-L20).";
}

/// <summary>
/// Optional injection seams for the shutdown transaction. Production constructs
/// <see cref="EngineSession"/> with <see langword="null"/> hooks (all defaults);
/// tests supply an instance to drive the fence deterministically and observe the
/// transaction without terminating the host (EQ_A2 C5 seams -- H-SEAM resolution).
/// </summary>
internal sealed class ShutdownTransactionHooks
{
    /// <summary>Overrides the named fence deadline (default <see cref="EngineSession.DefaultFenceDeadline"/>).</summary>
    public TimeSpan? FenceDeadline { get; init; }

    /// <summary>Replaces the sim-thread fence (<c>GameLoop.TryStop</c>). Inject <c>_ =&gt; false</c> to simulate a join timeout.</summary>
    public Func<TimeSpan, bool>? SimFenceOverride { get; init; }

    /// <summary>Replaces the pipeline-quiescence predicate (default: the real К-L18 check via <see cref="SimulationStateController"/>).</summary>
    public Func<bool>? PipelineQuiescentOverride { get; init; }

    /// <summary>Replaces the abort action (default: record diagnostics + <see cref="Environment.FailFast(string)"/>). Inject a recorder so abort-not-teardown is observable without killing the host.</summary>
    public Action<ShutdownAbortReport>? OnAbort { get; init; }

    /// <summary>Optional step recorder for order assertions.</summary>
    public Action<ShutdownStep>? OnStep { get; init; }

    /// <summary>
    /// Replaces the S7 checked world-teardown (default:
    /// <see cref="NativeWorld.DisposeChecked"/>). Inject a result with
    /// <c>Destroyed == false</c> to drive the busy-refusal -> Abort route without
    /// a real busy native world (EQ_A3 H-SEAM resolution).
    /// </summary>
    public Func<WorldTeardownResult>? WorldTeardownOverride { get; init; }
}

/// <summary>
/// The engine composition root (GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md B-5).
/// Owns the engine's runtime resources end-to-end -- the native world/kernel, the
/// managed&lt;-&gt;native bus bridge, the domain-bus aggregator, and the
/// mod-integration pipeline -- and releases them as one ordered transaction on
/// <see cref="Dispose"/>.
///
/// <see cref="Dispose"/> runs the world-shutdown TRANSACTION per
/// RESOURCE_OWNERSHIP_AND_LIFETIME.md section 4.4 and CONCURRENCY_AND_MEMORY_MODEL.md
/// section 6.2: <b>quiesce -&gt; fence -&gt; teardown</b>. The fence is a bounded,
/// checked join of the simulation thread plus a pipeline-quiescence gate; on a
/// missed fence the transaction ABORTS -- structured diagnostics then fail-fast --
/// WITHOUT native teardown, because dismantling native state under a live sim
/// thread is worse than leaking on abort (EQ_A2 / D4; seats К-L20). Because the
/// fence is self-contained (it proves quiescence before any teardown), world
/// disposal is safe irrespective of renderer-teardown timing.
///
/// Game-vocabulary-free by discipline (B-5): this class names only engine types.
/// The vanilla-content knowledge lives solely in <see cref="GameBootstrap"/>, the
/// sacrificial harness that constructs a session; the engine-to-game assembly edge
/// stays frozen by the boundary ratchet.
/// </summary>
internal sealed class EngineSession : IDisposable
{
    /// <summary>The named shutdown-fence deadline (EQ_A2 / D4): the bounded budget for the sim-thread join plus pipeline quiescence.</summary>
    public static readonly TimeSpan DefaultFenceDeadline = TimeSpan.FromSeconds(5);

    private static readonly TimeSpan QuiescencePollInterval = TimeSpan.FromMilliseconds(10);

    private readonly NativeWorld _world;
    private readonly ManagedBusBridge _busBridge;
    private readonly ModIntegrationPipeline _pipeline;
    private readonly GameServices _services;
    private readonly GameLoop _loop;
    private readonly ModMenuController _controller;
    private readonly ShutdownTransactionHooks? _hooks;
    private readonly SimulationStateController _quiescence;
    private bool _disposed;

    public EngineSession(
        NativeWorld world,
        ManagedBusBridge busBridge,
        ModIntegrationPipeline pipeline,
        GameServices services,
        GameLoop loop,
        ModMenuController controller,
        ShutdownTransactionHooks? hooks = null)
    {
        _world = world ?? throw new ArgumentNullException(nameof(world));
        _busBridge = busBridge ?? throw new ArgumentNullException(nameof(busBridge));
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loop = loop ?? throw new ArgumentNullException(nameof(loop));
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _hooks = hooks;
        _quiescence = new SimulationStateController(
            onPausedChanged: null,
            isPipelineQuiescentOverride: hooks?.PipelineQuiescentOverride);
    }

    /// <summary>The simulation loop. The Launcher starts it; the session fences it in <see cref="Dispose"/>.</summary>
    public GameLoop Loop => _loop;

    /// <summary>The mod-menu controller (edit-session hooks for the mod UI).</summary>
    public ModMenuController Controller => _controller;

    // --- M7 EngineHealth (ELT section 4.1): the session-scoped Degraded annotation.
    // NOT a sixth session/mod state; the LOCKED five-state diagrams are untouched.
    // Transition-refusal ("a Degraded session refuses transitions whose safety
    // assumptions the reason invalidates") depends on the section 3.2 session state
    // machine, which is not built -- this cascade lands the queryable annotation +
    // the quarantine link + the lifecycle event; enforcement wiring is later scope.
    private readonly object _healthLock = new();
    private readonly List<DegradedReason> _degradedReasons = new();

    /// <summary>
    /// Raised on every Degraded entry/exit (ELT section 4.1 lifecycle event). No UI
    /// obligation this cascade -- the event is the queryable/observable seam.
    /// </summary>
    public event Action<EngineHealthChanged>? HealthChanged;

    /// <summary>
    /// The current session-health snapshot (ELT section 4.1): Normal, or Degraded
    /// with the accumulated structured reasons. Queryable by UI and diagnostics.
    /// </summary>
    public EngineHealth Health
    {
        get
        {
            lock (_healthLock)
            {
                if (_degradedReasons.Count == 0)
                    return EngineHealth.Normal;
                return new EngineHealth(EngineHealthKind.Degraded, _degradedReasons.ToArray());
            }
        }
    }

    /// <summary>
    /// Records a Degraded reason (ELT section 4.1 escalation entry). Mod-origin
    /// reasons dedupe by <see cref="DegradedReason.ModId"/> -- a mod contributes one
    /// reason however many of its systems fault. Fires <see cref="HealthChanged"/>
    /// with Entered = true on the Normal -&gt; Degraded transition. Thread-safe: the
    /// scheduler calls this from inside its parallel phase fan-out.
    /// </summary>
    public void ReportDegraded(DegradedReason reason)
    {
        if (reason is null)
            throw new ArgumentNullException(nameof(reason));

        bool entered;
        lock (_healthLock)
        {
            if (reason.ModId is not null)
            {
                foreach (DegradedReason existing in _degradedReasons)
                {
                    if (string.Equals(existing.ModId, reason.ModId, StringComparison.Ordinal))
                        return; // already recorded for this mod
                }
            }
            entered = _degradedReasons.Count == 0;
            _degradedReasons.Add(reason);
        }
        HealthChanged?.Invoke(new EngineHealthChanged(entered, reason, EngineHealthKind.Degraded));
    }

    /// <summary>
    /// Clears every Degraded reason contributed by <paramref name="modId"/> (ELT
    /// section 4.1 exit path: "exited only by the action named in the advice; a
    /// successful retry removes its own reason"). Fires <see cref="HealthChanged"/>
    /// with Entered = false, ResultingKind = Normal when this clears the last reason.
    /// The caller that resolves a quarantined mod is later-cascade scope; this is the
    /// exit mechanism the C5 tests drive.
    /// </summary>
    public void ClearDegradedForMod(string modId)
    {
        if (modId is null)
            return;

        DegradedReason? removed = null;
        bool exited = false;
        lock (_healthLock)
        {
            for (int i = _degradedReasons.Count - 1; i >= 0; i--)
            {
                if (string.Equals(_degradedReasons[i].ModId, modId, StringComparison.Ordinal))
                {
                    removed = _degradedReasons[i];
                    _degradedReasons.RemoveAt(i);
                }
            }
            exited = removed is not null && _degradedReasons.Count == 0;
        }
        if (removed is not null)
        {
            HealthChanged?.Invoke(new EngineHealthChanged(
                Entered: false,
                removed,
                exited ? EngineHealthKind.Normal : EngineHealthKind.Degraded));
        }
    }

    /// <summary>
    /// Runs the world-shutdown transaction and releases every owned engine
    /// resource. Idempotent -- a second call is a no-op.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        // --- QUIESCE + FENCE (S1/S2) ---------------------------------------
        // Stop admitting sim work, then a bounded, CHECKED join of the sim
        // thread, then a pipeline-quiescence gate. The whole fence shares one
        // deadline. On failure the transaction aborts -- never teardown under a
        // live mutator (CMM section 6.2 abandoned-thread prohibition).
        TimeSpan deadline = _hooks?.FenceDeadline ?? DefaultFenceDeadline;
        Func<TimeSpan, bool> fence = _hooks?.SimFenceOverride ?? _loop.TryStop;

        bool threadStopped = fence(deadline);
        bool quiescent = threadStopped && WaitPipelineQuiescent(deadline);
        if (!threadStopped || !quiescent)
        {
            Abort(new ShutdownAbortReport(threadStopped, quiescent, deadline));
            return;
        }
        Step(ShutdownStep.FencePassed);

        // --- TEARDOWN (S3 -> S7), reverse acquisition order ----------------
        // S3: drop deferred domain events -- no handler may run post-quiesce --
        // counted for the exit diagnostics (the S8 exit-log consumer is the
        // Launcher's, out of this cascade's managed scope).
        _services.DropDeferred();
        Step(ShutdownStep.DeferredDropped);

        // S4: unload mods -- UnloadAll's first production caller (EQ_A2 / M2).
        _pipeline.UnloadAll();
        Step(ShutdownStep.ModsUnloaded);

        // S5: native scheduler/graph/wake teardown, then native bus clear.
        SystemGraphInterop.Clear();
        WakeRegistryInterop.Clear();
        SchedulerAdapter.ClearCallback();
        Step(ShutdownStep.NativeSchedulerCleared);

        _busBridge.Shutdown();
        Step(ShutdownStep.NativeBusCleared);

        // S7: dispose the native world deterministically on the thread that
        // observed quiescence, via the CHECKED path (EQ_A3 / ROL section 6.2
        // wait-for-zero; К-L20). A WORLD_BUSY refusal AFTER a passed fence means
        // live borrows despite proven quiescence -- a К-L20 invariant violation:
        // record structured diagnostics (incl. the counts) and route through
        // Abort (fail-fast). NEVER loop-retry -- a spin here would be a kostyl
        // hiding the breach. The finalizer stays a leak-reporter backstop only.
        WorldTeardownResult teardown =
            _hooks?.WorldTeardownOverride?.Invoke() ?? _world.DisposeChecked();
        if (!teardown.Destroyed)
        {
            Abort(ShutdownAbortReport.ForBusyWorld(
                teardown.ActiveSpans, teardown.ActiveBatches, deadline));
            return;
        }
        Step(ShutdownStep.WorldDisposed);
    }

    /// <summary>
    /// Polls the К-L18 pipeline-quiescence predicate until quiescent or the
    /// deadline elapses. Depth-0 (uninitialised pipeline) is vacuously quiescent.
    /// </summary>
    private bool WaitPipelineQuiescent(TimeSpan deadline)
    {
        DateTime end = DateTime.UtcNow + deadline;
        while (true)
        {
            if (_quiescence.IsPipelineQuiescent())
                return true;
            if (DateTime.UtcNow >= end)
                return false;
            Thread.Sleep(QuiescencePollInterval);
        }
    }

    private void Abort(ShutdownAbortReport report)
    {
        Step(ShutdownStep.Aborted);
        if (_hooks?.OnAbort is { } onAbort)
        {
            onAbort(report);
            return;
        }

        // Default: fail-fast WITHOUT native teardown (D4). Environment.FailFast
        // routes the message to WER / the event log and terminates the process --
        // never console output; keeps the src console-write census pinned at 2.
        Environment.FailFast(report.Describe());
    }

    private void Step(ShutdownStep step) => _hooks?.OnStep?.Invoke(step);
}
