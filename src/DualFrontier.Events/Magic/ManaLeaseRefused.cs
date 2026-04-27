using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Refusal to open a mana lease. Published by <c>ManaSystem</c> in response to
/// <c>ManaLeaseOpenRequest</c> when one of the conditions is not met
/// (see <see cref="RefusalReason"/>). The initiator (SpellCastSystem, AI)
/// must choose an alternative action.
/// </summary>
/// <param name="Caster">The mage caster that was refused.</param>
/// <param name="Reason">Refusal reason.</param>
/// <param name="AvailableMana">Actually available mana value at the moment of
/// refusal (for diagnostics and UI).</param>
public sealed record ManaLeaseRefused(
    EntityId Caster,
    RefusalReason Reason,
    float AvailableMana) : IEvent;
