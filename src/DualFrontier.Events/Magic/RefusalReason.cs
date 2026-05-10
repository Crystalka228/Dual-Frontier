namespace DualFrontier.Events.Magic;

/// <summary>
/// Reason for refusing to open a mana lease (<c>ManaLeaseRefused</c>).
/// Lets subscribers distinguish refusal types and react differently
/// (for example, try another school, wait for regen, pick a different mage).
/// </summary>
public enum RefusalReason
{
    /// <summary>
    /// The caster does not have enough mana for the requested <c>DrainPerTick</c>
    /// over the minimum lease duration.
    /// </summary>
    InsufficientMana,

    /// <summary>
    /// The cap on simultaneously open leases for this caster has been exceeded
    /// (a guard against "infinitely" sustaining many effects).
    /// </summary>
    LeaseCapExceeded,

    /// <summary>
    /// The caster has no active bond with a golem/target (<c>GolemBondComponent</c>
    /// is missing or in the <c>Abandoned</c> state).
    /// </summary>
    NoActiveBond,

    /// <summary>
    /// The school of the requesting spell does not match the school of the caster/target.
    /// School data lives in vanilla magic mod content per migration plan §3.5; the
    /// kernel-side SchoolComponent stub was removed in K8.2 v2 alongside the other
    /// empty TODO stubs. GDD 4.3.
    /// </summary>
    SchoolMismatch,
}
