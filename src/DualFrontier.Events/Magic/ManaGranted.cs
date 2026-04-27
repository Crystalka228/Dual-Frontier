using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Step 2 of the two-step model: ManaSystem confirms the mana charge.
/// The <see cref="ManaIntent"/> recipient (e.g. SpellCastSystem) finishes
/// the action — publishes <c>SpellCastEvent</c>, activates the golem, etc.
/// </summary>
public sealed record ManaGranted : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public required float Amount { get; init; }
}
