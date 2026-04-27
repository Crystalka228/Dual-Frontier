namespace DualFrontier.Events.Magic;

/// <summary>
/// Identifier of a continuous mana lease — used to
/// correlate lease lifecycle events: <c>ManaLeaseOpened</c>,
/// <c>ManaLeaseClosed</c>. Monotonically increasing value, unique within
/// the process.
/// </summary>
public readonly record struct LeaseId(ulong Value)
{
    /// <summary>
    /// Factory method that issues a new monotonically increasing identifier.
    /// TODO: Phase 5 — implement the counter (Interlocked.Increment on a private
    /// field); for now <see cref="NotImplementedException"/> is thrown.
    /// </summary>
    public static LeaseId New() => throw new NotImplementedException("TODO: Phase 5 — implement the monotonic LeaseId counter.");
}
