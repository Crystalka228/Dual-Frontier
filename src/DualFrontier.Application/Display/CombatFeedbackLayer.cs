using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Display;

namespace DualFrontier.Application.Display;

/// <summary>
/// К-L17 combat feedback layer base (К10.3 v2 Item 40).
///
/// Renders damage numbers, hit sparks, weapon glints, и other К-L15 Fast
/// tier event consumers. Latency contract: ≤1ms К-L15 + ≤16ms display ≈
/// ≤17ms event-к-visible per К-L17 invariant (Prediction 15).
///
/// Per FastTierContractMonitor (К10.2 contract): subscribers must be bounded
/// exec, no blocking, no GC allocation в the hot path. This layer's
/// <see cref="EnqueueFeedback"/> is the bounded sink for К-L15 fast tier
/// subscribers; rendering happens at display tick time draining the queue.
///
/// К10.3 v2 boundary: layer slot established с the latency contract +
/// thread-safe feedback queue; concrete К-L15 Fast tier subscription wiring
/// (BusFacade.Subscribe&lt;CombatHitEvent&gt;) lives в game systems либо
/// presentation backends per К10.3 v2 §1.5 S-LOCK-11.
/// </summary>
public class CombatFeedbackLayer : Layer
{
    /// <summary>Default FQN для the vanilla combat feedback slot.</summary>
    public const string DefaultFqn = "DualFrontier.Application.Display.CombatFeedbackLayer";

    private readonly string _fqn;
    private readonly int _compositionOrder;
    private readonly Action<ILayerRenderContext, IReadOnlyList<object>>? _renderDelegate;
    private readonly Queue<object> _pendingFeedback = new();
    private readonly object _queueLock = new();

    /// <summary>Diagnostic — current queued feedback event count.</summary>
    public int PendingFeedbackCount
    {
        get { lock (_queueLock) return _pendingFeedback.Count; }
    }

    /// <summary>
    /// Constructs а combat feedback layer slot. Pass <paramref name="renderDelegate"/>
    /// = <see langword="null"/> для a no-op placeholder; pass а delegate
    /// drawing feedback effects for full operational integration.
    /// </summary>
    public CombatFeedbackLayer(
        string? fqn = null,
        int compositionOrder = 0,
        Action<ILayerRenderContext, IReadOnlyList<object>>? renderDelegate = null)
    {
        _fqn = fqn ?? DefaultFqn;
        _compositionOrder = compositionOrder;
        _renderDelegate = renderDelegate;
    }

    /// <inheritdoc/>
    public override LayerType Type => LayerType.CombatFeedback;

    /// <inheritdoc/>
    public override string Fqn => _fqn;

    /// <inheritdoc/>
    public override int CompositionOrder => _compositionOrder;

    /// <summary>
    /// К-L15 Fast tier subscriber sink. Thread-safe — invoked from Fast tier
    /// dispatch callback; bounded exec, no GC alloc beyond the queue node
    /// (per FastTierContractMonitor contract; queue uses reference-cell
    /// nodes — boxed for value-type events). Caller responsible для passing
    /// already-boxed payload если invoked от unmanaged callback.
    /// </summary>
    public void EnqueueFeedback(object feedbackEvent)
    {
        if (feedbackEvent is null) throw new ArgumentNullException(nameof(feedbackEvent));
        lock (_queueLock)
        {
            _pendingFeedback.Enqueue(feedbackEvent);
        }
    }

    /// <inheritdoc/>
    public override void Render(ILayerRenderContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        // Drain the queue into a snapshot per К-L17 atomic-from-observer semantics —
        // render delegate sees the events present at display tick start, не race
        // с in-flight Fast tier dispatch на the same frame.
        List<object>? snapshot = null;
        lock (_queueLock)
        {
            if (_pendingFeedback.Count > 0)
            {
                snapshot = new List<object>(_pendingFeedback);
                _pendingFeedback.Clear();
            }
        }

        if (snapshot is not null && _renderDelegate is not null)
        {
            _renderDelegate(context, snapshot);
        }
    }

    /// <inheritdoc/>
    public override void OnUnregistered()
    {
        lock (_queueLock)
        {
            _pendingFeedback.Clear();
        }
    }
}
