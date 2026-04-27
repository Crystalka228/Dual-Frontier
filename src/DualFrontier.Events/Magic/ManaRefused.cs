using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Step 2 of the two-step model: ManaSystem refuses the charge — there is
/// not enough mana. The recipient cancels the action (cast, golem activation).
/// Prolonged exhaustion -> chain reaction on golems (see GDD 5.2).
/// </summary>
public sealed record ManaRefused : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public required float RequestedAmount { get; init; }
    // TODO: public required float AvailableAmount { get; init; }
}
