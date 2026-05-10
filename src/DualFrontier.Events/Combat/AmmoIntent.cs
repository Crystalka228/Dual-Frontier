using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Step 1 of the two-step model (TechArch 11.5): CombatSystem publishes
/// the intent to obtain ammo. InventorySystem collects all AmmoIntents
/// in the gather phase and resolves them in batch in the next phase.
///
/// Do NOT confuse with a blocking Request — this event does not wait for a reply.
/// The reply will arrive later as <see cref="AmmoGranted"/> or <see cref="AmmoRefused"/>.
/// </summary>
public sealed record AmmoIntent : IEvent
{
    // TODO: public required EntityId RequesterId { get; init; }
    // TODO: public required GridVector Position { get; init; } // see Components/Shared/PositionComponent
}
