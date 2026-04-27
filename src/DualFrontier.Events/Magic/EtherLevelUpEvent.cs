using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// A mage's ether perception level has increased (see GDD 4.1, 4.2).
/// Marked with <see cref="DeferredAttribute"/>: a level up changes
/// max mana, regeneration, available schools — those are derived values,
/// recalculation must happen in the next phase to avoid conflicting with
/// concurrent edits to <c>ManaComponent</c> (see DualFrontier.Components.Magic).
/// </summary>
[Deferred]
public sealed record EtherLevelUpEvent : IEvent
{
    // TODO: public required EntityId MageId { get; init; }
    // TODO: public required int OldLevel { get; init; }
    // TODO: public required int NewLevel { get; init; }  // 1..5 — see EtherComponent
}
