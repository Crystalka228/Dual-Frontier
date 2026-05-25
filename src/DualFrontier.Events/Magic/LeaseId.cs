using DualFrontier.Contracts.Analyzer;

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
    [ReservedStub(
        ReservedStubPurpose.BuildComposition,
        "Magic Phase 5 roadmap stub (Lesson #N12 sub-pattern A) — LeaseId monotonic counter " +
        "via Interlocked.Increment on private static field. " +
        "Activation: Phase 5 continuous lease model.")]
    public static LeaseId New() => throw new NotImplementedException("TODO: Phase 5 — implement the monotonic LeaseId counter.");
}
