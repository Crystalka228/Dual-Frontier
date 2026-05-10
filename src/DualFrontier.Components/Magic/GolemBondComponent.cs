using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Enums;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Bond between a golem and its mage owner. Without an owner the golem does
/// not function (see GDD 5.2 "Vulnerabilities": mage exhaustion/death halts
/// the golems). <c>GolemTier</c> — golem level 1..5 (GDD 5.1 "Golem Tiers").
///
/// v0.2 Addendum (§12.5): added ownership-mode and contest-counter fields to
/// implement the golem takeover/abandonment mechanic via the
/// <c>GolemOwnershipTransferRequest</c> / <c>GolemOwnershipChanged</c> events.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct GolemBondComponent : IComponent
{
    // TODO: public EntityId? OwnerId;
    // TODO: public int GolemTier;  // 1..5 — see GDD 5.1

    /// <summary>
    /// Current mage owner. <c>null</c> when the golem is in
    /// <see cref="OwnershipMode.Abandoned"/>. TODO: Phase 6 — couple with the
    /// mage lifecycle (transition to Abandoned on owner death).
    /// </summary>
    public EntityId? BondedMage { get; init; }

    /// <summary>
    /// Golem ownership mode. Defaults to <see cref="OwnershipMode.Bonded"/>.
    /// TODO: Phase 6 — state machine for transitions via
    /// <c>GolemOwnershipTransferRequest</c>.
    /// </summary>
    public OwnershipMode Mode { get; init; }

    /// <summary>
    /// Counter of ticks elapsed since ownership contest began.
    /// Used by <c>GolemBondSystem</c> to time out the owner change in
    /// <see cref="OwnershipMode.Contested"/>.
    /// TODO: Phase 6 — define the timeout threshold.
    /// </summary>
    public int TicksSinceContested { get; init; }

    /// <summary>
    /// Strength of the bond with the owner: the higher it is, the harder it
    /// is to seize control of the golem. Participates in contest resolution.
    /// TODO: Phase 6 — formula based on mage level/school.
    /// </summary>
    public int BondStrength { get; init; }
}
