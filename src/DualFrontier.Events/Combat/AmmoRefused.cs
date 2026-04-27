using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Step 2 of the two-step model (TechArch 11.5): InventorySystem refuses
/// to issue ammo — the stockpile is empty or the reservation is taken by another request.
/// CombatSystem must cancel the shot and possibly switch tactics.
/// </summary>
public sealed record AmmoRefused : IEvent
{
    // TODO: public required EntityId RequesterId { get; init; }
    // TODO: public required AmmoType AmmoType { get; init; }
    // TODO: public string Reason { get; init; } = string.Empty;  // for debugging/log
}
