namespace DualFrontier.Events.Combat;

/// <summary>
/// Identifier of a compound shot transaction (two-phase commit between
/// <c>InventoryBus</c> and <c>MagicBus</c>). Used to correlate
/// <c>CompoundShotIntent</c> with the <c>ShootGranted</c> / <c>ShootRefused</c> responses.
/// Monotonically increasing value, unique within the process.
/// </summary>
public readonly record struct TransactionId(ulong Value)
{
    /// <summary>
    /// Factory method that issues a new monotonically increasing identifier.
    /// TODO: Phase 4 — implement the counter (Interlocked.Increment on a private
    /// field); for now <see cref="NotImplementedException"/> is thrown.
    /// </summary>
    public static TransactionId New() => throw new NotImplementedException("TODO: Phase 4 — implement the monotonic TransactionId counter.");
}
