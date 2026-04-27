using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Enums;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Golem ownership has changed. Marked with <see cref="DeferredAttribute"/>:
/// subscribers (AI, UI, audit) update their state on the next tick,
/// to avoid forcing mutations during a phase where other magic systems are running.
/// </summary>
/// <param name="Golem">The golem entity.</param>
/// <param name="PreviousMage">Previous mage-owner (or <c>null</c> if the
/// golem was in <see cref="OwnershipMode.Abandoned"/>).</param>
/// <param name="NewMage">New mage-owner (or <c>null</c> if the golem
/// becomes abandoned).</param>
/// <param name="Mode">Resulting ownership mode.</param>
[Deferred]
public sealed record GolemOwnershipChanged(
    EntityId Golem,
    EntityId? PreviousMage,
    EntityId? NewMage,
    OwnershipMode Mode) : IEvent;
