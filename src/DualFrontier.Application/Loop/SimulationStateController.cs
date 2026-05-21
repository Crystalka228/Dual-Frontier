using System;
using System.Threading;
using System.Threading.Tasks;
using DualFrontier.Core.Interop;

namespace DualFrontier.Application.Loop;

/// <summary>
/// К10.3 v2 Item 42 — К-L18 quiescent state helper layer (per S-LOCK-12
/// helpers-only scope).
///
/// Programmatic API для pausing the simulation thread, waiting for pipeline
/// slots к reach quiescent state, и resuming. Wraps the lower-level
/// primitives:
/// <list type="bullet">
///   <item><see cref="ModUnloadInterop.SetSimPaused"/> — К10.2 К-L18 sim
///       pause flag.</item>
///   <item><see cref="PipelineSlotInterop.IsQuiescent"/> — К10.3 v2 Item 33
///       pipeline slot quiescence check.</item>
/// </list>
///
/// Consumers (mod management UI, hot reload tooling) compose
/// <see cref="PauseAsync"/> → <see cref="WaitForQuiescenceAsync"/> →
/// (perform mod operation) → <see cref="ResumeAsync"/>. The native
/// <c>df_scheduler_unload_mod_native_state</c> primitive enforces К-L18
/// precondition independently (Item 41); this controller exists к make
/// the precondition reachable from managed callers без forcing every site
/// к poll the pipeline directly.
///
/// К10.3 v2 boundary per S-LOCK-12: full settings menu / preferences UI =
/// V-cycle or К-extensions scope. К10.3 v2 lands this helper plus the
/// existing <see cref="DualFrontier.Application.Modding.ModMenuController"/>
/// pause hook integration; settings menu deferred.
/// </summary>
public sealed class SimulationStateController
{
    /// <summary>Default quiescence wait timeout (5 seconds).</summary>
    public static readonly TimeSpan DefaultQuiescenceTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Polling interval used by <see cref="WaitForQuiescenceAsync"/>. Set
    /// short enough that quiescence is detected promptly without burning
    /// CPU; defaults к 10ms (≈1 native pipeline tick at 100Hz sim).
    /// </summary>
    public static readonly TimeSpan DefaultQuiescencePollInterval = TimeSpan.FromMilliseconds(10);

    private readonly Action<bool>? _onPausedChanged;
    private readonly Func<bool>? _isPipelineQuiescent;

    /// <summary>
    /// Constructs the controller. Pass <paramref name="onPausedChanged"/>
    /// к receive pause/resume notifications (e.g. GameBootstrap wires this
    /// к <see cref="GameLoop.SetPaused"/>); pass
    /// <paramref name="isPipelineQuiescentOverride"/> = <see langword="null"/>
    /// к use <see cref="PipelineSlotInterop.IsQuiescent"/> (default; production
    /// path). Tests inject а deterministic delegate.
    /// </summary>
    public SimulationStateController(
        Action<bool>? onPausedChanged = null,
        Func<bool>? isPipelineQuiescentOverride = null)
    {
        _onPausedChanged = onPausedChanged;
        _isPipelineQuiescent = isPipelineQuiescentOverride;
    }

    /// <summary>
    /// Sets the К-L18 sim-paused flag к <see langword="true"/> и invokes the
    /// optional pause notification delegate. Idempotent. К10.2 native bus
    /// + К10.3 v2 native pipeline check use this flag.
    /// </summary>
    public Task PauseAsync()
    {
        ModUnloadInterop.SetSimPaused(true);
        _onPausedChanged?.Invoke(true);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the К-L18 sim-paused flag к <see langword="false"/> и invokes
    /// the optional resume notification delegate. Idempotent.
    /// </summary>
    public Task ResumeAsync()
    {
        ModUnloadInterop.SetSimPaused(false);
        _onPausedChanged?.Invoke(false);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the pipeline is quiescent — either
    /// pipeline не initialized (depth=0) или all slots Empty / ReadableAsTail
    /// per К-L18 spec verbatim. Reads from the injected override delegate
    /// если provided, otherwise calls <see cref="PipelineSlotInterop.IsQuiescent"/>.
    /// </summary>
    public bool IsPipelineQuiescent()
    {
        if (_isPipelineQuiescent is not null) return _isPipelineQuiescent();
        // Pre-init (depth=0) → vacuously quiescent (matches native mod unload
        // primitive treatment per Item 41 — no pipeline means no in-flight
        // compute by definition). Post-init → all slots Empty/ReadableAsTail.
        if (PipelineSlotInterop.GetDepth() == 0) return true;
        return PipelineSlotInterop.IsQuiescent();
    }

    /// <summary>
    /// Polls <see cref="IsPipelineQuiescent"/> until <see langword="true"/>
    /// или <paramref name="timeout"/> elapses. Returns <see langword="true"/>
    /// on quiescence; <see langword="false"/> on timeout.
    /// </summary>
    public async Task<bool> WaitForQuiescenceAsync(
        TimeSpan? timeout = null,
        TimeSpan? pollInterval = null,
        CancellationToken cancellationToken = default)
    {
        TimeSpan effectiveTimeout = timeout ?? DefaultQuiescenceTimeout;
        TimeSpan effectivePoll = pollInterval ?? DefaultQuiescencePollInterval;

        if (effectiveTimeout < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeout), "Must be non-negative.");
        if (effectivePoll <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(pollInterval), "Must be positive.");

        DateTime deadline = DateTime.UtcNow + effectiveTimeout;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (IsPipelineQuiescent()) return true;
            if (DateTime.UtcNow >= deadline) return false;
            await Task.Delay(effectivePoll, cancellationToken).ConfigureAwait(false);
        }
    }
}

/// <summary>
/// К10.3 v2 Item 42 — exception thrown by callers wrapping а timeout failure
/// of <see cref="SimulationStateController.WaitForQuiescenceAsync"/>. Кernel
/// itself returns the boolean result; consumers may choose к raise this
/// instead. Per MOD_OS §11.2 К-L18 amendment, the equivalent
/// <c>PipelineQuiescenceTimeout</c> validation error kind covers the
/// load-time path; this exception covers the runtime path.
/// </summary>
public sealed class PipelineQuiescenceTimeoutException : Exception
{
    public PipelineQuiescenceTimeoutException(string message) : base(message) { }
}
