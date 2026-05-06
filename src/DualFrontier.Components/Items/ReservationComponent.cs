namespace DualFrontier.Components.Items;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

/// <summary>
/// Marks an entity as reserved by a specific pawn. Generic primitive used
/// by multiple systems to prevent contention: pawn walking к food entity
/// reserves it, second pawn skipping that entity in target selection;
/// pawn approaching bed reserves it before sitting down.
///
/// Reservation lifetime: presence of this component = reservation active.
/// System removes the component когда reservation released (consume done,
/// bed claimed, target reached, pawn died).
///
/// Multiple systems may set/clear reservations as primitive operation;
/// no central reservation manager. <see cref="ReservedAtTick"/> supports
/// timeout cleanup (e.g. orphan reservations from dead pawns).
/// </summary>
[ModAccessible(Read = true, Write = true)]
public sealed class ReservationComponent : IComponent
{
    /// <summary>
    /// Pawn entity holding the reservation. Component existence implies
    /// the reservation is active — null is not a valid state (component
    /// is removed on release).
    /// </summary>
    public EntityId ReservedBy { get; set; }

    /// <summary>
    /// Simulation tick when reservation was created. Used by timeout
    /// cleanup if the reserving pawn dies, gets stuck, or otherwise
    /// fails to follow through.
    /// </summary>
    public long ReservedAtTick { get; set; }
}
