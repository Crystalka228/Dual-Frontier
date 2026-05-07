using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by <c>SleepSystem</c> when it picks a sleep target for a pawn.
/// Carries the bed entity и tile so the component owners — <c>JobSystem</c>
/// for <c>JobComponent.Target</c> and <c>MovementSystem</c> for
/// <c>MovementComponent.Target</c> — can perform the writes inside their
/// captured contexts at the phase boundary.
/// <para>
/// Parallel to <see cref="PawnConsumeTargetEvent"/>. Marked
/// <see cref="DeferredAttribute"/>.
/// </para>
/// </summary>
[Deferred]
public sealed record PawnSleepTargetEvent : IEvent
{
    /// <summary>The pawn that selected a sleep target.</summary>
    public required EntityId PawnId { get; init; }

    /// <summary>The chosen bed entity.</summary>
    public required EntityId Target { get; init; }

    /// <summary>The tile the pawn must reach to claim the bed.</summary>
    public required GridVector TargetTile { get; init; }
}
