using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by <c>SleepSystem</c> when a pawn finishes sleeping (wakes
/// past <c>WakeThreshold</c>) или fails к claim a bed (race с another
/// pawn). Each owning system clears its component on receipt.
/// <para>
/// Parallel to <see cref="PawnConsumeFinishedEvent"/>. Marked
/// <see cref="DeferredAttribute"/>.
/// </para>
/// </summary>
[Deferred]
public sealed record PawnSleepFinishedEvent : IEvent
{
    /// <summary>The pawn that just finished sleeping (или aborted).</summary>
    public required EntityId PawnId { get; init; }
}
