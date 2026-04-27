using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Step 2 of the two-step model (TechArch 11.5): InventorySystem confirms
/// that ammo has been issued in response to the prior <see cref="AmmoIntent"/>.
/// CombatSystem, upon receiving this event, completes the shot.
/// </summary>
public sealed record AmmoGranted : IEvent
{
    // TODO: public required EntityId RequesterId { get; init; }
    // TODO: public required AmmoType AmmoType { get; init; }
    // TODO: public required int Count { get; init; }
}
