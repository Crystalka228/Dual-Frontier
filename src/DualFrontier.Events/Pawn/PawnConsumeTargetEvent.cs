using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by <c>ConsumeSystem</c> when it picks a consume target for a
/// pawn (Phase 1 of the consume loop). Carries the entity id and tile so
/// the relevant component owners — <c>JobSystem</c> for
/// <c>JobComponent.Target</c> and <c>MovementSystem</c> for
/// <c>MovementComponent.Target</c> — can perform the writes inside their
/// own captured contexts at the phase boundary.
/// <para>
/// Marked <see cref="DeferredAttribute"/>: needed so each subscriber's
/// declared <c>[SystemAccess]</c> writes are honoured by the isolation
/// guard. Without deferral the handler would run inside ConsumeSystem's
/// publishing context and the writes would be rejected.
/// </para>
/// </summary>
[Deferred]
public sealed record PawnConsumeTargetEvent : IEvent
{
    /// <summary>The pawn that selected a consume target.</summary>
    public required EntityId PawnId { get; init; }

    /// <summary>The chosen consumable / water source entity.</summary>
    public required EntityId Target { get; init; }

    /// <summary>The tile the pawn must reach to consume the target.</summary>
    public required GridVector TargetTile { get; init; }
}
