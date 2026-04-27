namespace DualFrontier.Contracts.Enums;

/// <summary>
/// Ownership mode of a golem from the bonded mage's side.
/// Defines how the bond system (<c>GolemBondComponent</c>) behaves on owner
/// change, control takeover, and bond breaking.
/// Used by both components (Components.Magic) and events (Events.Magic),
/// hence it lives in Contracts.
/// </summary>
public enum OwnershipMode
{
    /// <summary>
    /// Default mode: the golem is firmly bonded to a single mage.
    /// Full control, regular upkeep through mana.
    /// </summary>
    Bonded,

    /// <summary>
    /// The bond is contested: another mage is attempting takeover.
    /// <c>TicksSinceContested</c> grows until the timeout switches the owner.
    /// </summary>
    Contested,

    /// <summary>
    /// The golem is abandoned: the previous owner is dead/gone, no new one
    /// yet. The golem is inactive but the entity is preserved — it can be
    /// re-bonded.
    /// </summary>
    Abandoned,

    /// <summary>
    /// Ownership has just been transferred to a new mage (a one-tick
    /// transition state). It transitions to <see cref="Bonded"/> on the next
    /// tick.
    /// </summary>
    Transferred,
}
