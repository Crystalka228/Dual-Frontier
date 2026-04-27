namespace DualFrontier.Events.Magic;

/// <summary>
/// Reason for closing a mana lease (<c>ManaLeaseClosed</c>). Helps
/// subscribers distinguish a normal completion from an abnormal one and
/// correctly update state (animations, sounds, statistics counters).
/// </summary>
public enum CloseReason
{
    /// <summary>
    /// Lease completed normally — the spell/effect ran to its end.
    /// </summary>
    Completed,

    /// <summary>
    /// Cast was interrupted by external influence (damage, stun, disruption).
    /// </summary>
    SpellInterrupted,

    /// <summary>
    /// The consumer golem was deactivated (see <c>GolemBondComponent</c>,
    /// GDD 5.2).
    /// </summary>
    GolemDeactivated,

    /// <summary>
    /// The mage caster died — all of their leases are forcibly closed.
    /// </summary>
    PawnDied,

    /// <summary>
    /// The caster's mana is exhausted to zero, draining can no longer continue.
    /// </summary>
    ManaExhausted,
}
