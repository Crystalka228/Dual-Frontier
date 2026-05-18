using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using DualFrontier.Contracts.Bus;

namespace DualFrontier.Application.Bus;

/// <summary>
/// K10.2 Item 29 — Runtime instrumentation for Fast tier subscriber latency
/// (К-L15 invariant: fast tier subscriber response ≤1ms target).
///
/// Wraps a managed Fast tier subscriber action with a stopwatch + violation
/// counter. Exceedance of <see cref="LatencyBudgetMicros"/> increments the
/// per-(modId, eventType) violation counter; on <see cref="ViolationThreshold"/>
/// breach within a tick window, the monitor signals the consumer (typically
/// a fault handler) к surface
/// <c>ValidationErrorKind.FastTierContractViolation</c>.
///
/// К10.2 scope discipline: monitor lands the measurement + counter primitive
/// + violation event. Fault handler integration deferred к К-extensions
/// per Q-N-N tracking; consumer code calls <see cref="ResetViolationCounts"/>
/// at tick boundaries.
/// </summary>
public sealed class FastTierContractMonitor
{
    /// <summary>Default latency budget per К-L15 (1 ms target).</summary>
    public const long DefaultLatencyBudgetMicros = 1_000;

    /// <summary>Default violation threshold per tick window (К10.2 default).</summary>
    public const int DefaultViolationThreshold = 3;

    private readonly ConcurrentDictionary<(string ModId, string EventFqn), int> _violationCounts = new();

    public long LatencyBudgetMicros { get; set; } = DefaultLatencyBudgetMicros;
    public int ViolationThreshold { get; set; } = DefaultViolationThreshold;

    /// <summary>
    /// Raised when a (modId, eventFqn) pair exceeds <see cref="ViolationThreshold"/>
    /// since the last reset. Consumer (typically <c>ModFaultHandler</c>)
    /// surfaces <c>ValidationErrorKind.FastTierContractViolation</c>.
    /// </summary>
    public event Action<string, string, int>? ViolationsExceededThreshold;

    /// <summary>
    /// Measures the elapsed time of <paramref name="subscriber"/> against the
    /// latency budget. Increments per-(modId, eventFqn) violation count if
    /// exceeded; raises <see cref="ViolationsExceededThreshold"/> when count
    /// crosses <see cref="ViolationThreshold"/>.
    /// </summary>
    /// <returns>The elapsed microseconds.</returns>
    public long MeasureInvocation(string modId, string eventFqn, Action subscriber)
    {
        if (subscriber is null) throw new ArgumentNullException(nameof(subscriber));
        var sw = Stopwatch.StartNew();
        try
        {
            subscriber();
        }
        finally
        {
            sw.Stop();
        }
        long elapsedMicros = sw.ElapsedTicks * 1_000_000L / Stopwatch.Frequency;
        if (elapsedMicros > LatencyBudgetMicros)
        {
            int newCount = _violationCounts.AddOrUpdate(
                (modId, eventFqn),
                _ => 1,
                (_, prev) => prev + 1);
            if (newCount >= ViolationThreshold)
            {
                ViolationsExceededThreshold?.Invoke(modId, eventFqn, newCount);
            }
        }
        return elapsedMicros;
    }

    /// <summary>Diagnostic: current violation count для a (modId, eventFqn).</summary>
    public int GetViolationCount(string modId, string eventFqn)
        => _violationCounts.TryGetValue((modId, eventFqn), out int count) ? count : 0;

    /// <summary>
    /// Resets all violation counters. Called at tick boundary (or per-mod
    /// at unload time, как сторонний side-effect of Step 3.5).
    /// </summary>
    public void ResetViolationCounts() => _violationCounts.Clear();
}
