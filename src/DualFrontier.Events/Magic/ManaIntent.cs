using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Step 1 of the two-step model (TechArch 11.5): a system publishes the intent
/// to spend mana (cast a spell, sustain a golem, perform a ritual).
/// ManaSystem replies with <see cref="ManaGranted"/> or <see cref="ManaRefused"/>.
/// </summary>
public sealed record ManaIntent : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public required float Amount { get; init; }
    // TODO: public string Purpose { get; init; } = string.Empty;  // for debugging/analytics
}
