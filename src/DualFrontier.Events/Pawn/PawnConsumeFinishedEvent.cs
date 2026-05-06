using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by <c>ConsumeSystem</c> when a pawn finishes consuming
/// (Phase 2 arrival of the consume loop). Each owning system clears its
/// component on receipt: <c>JobSystem</c> resets <c>JobComponent</c> to
/// Idle with no Target, and <c>MovementSystem</c> clears
/// <c>MovementComponent.Target</c> + <c>Path</c>.
/// <para>
/// Marked <see cref="DeferredAttribute"/>: like
/// <see cref="PawnConsumeTargetEvent"/>, the event is dispatched at the
/// phase boundary so each subscriber's captured context allows the write.
/// Restoration travels separately via
/// <see cref="NeedsRestoredEvent"/> so NeedsComponent stays under
/// NeedsSystem's single-writer contract.
/// </para>
/// </summary>
[Deferred]
public sealed record PawnConsumeFinishedEvent : IEvent
{
    /// <summary>The pawn that just finished consuming.</summary>
    public required EntityId PawnId { get; init; }
}
