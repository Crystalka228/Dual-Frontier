namespace DualFrontier.Events.Combat;

/// <summary>
/// Reason for refusing to perform a compound shot (<c>ShootRefused</c>).
/// Lets AI/UI distinguish refusal types: search for ammo, wait for mana regen,
/// switch target, etc.
/// </summary>
public enum ShotRefusalReason
{
    /// <summary>
    /// The shooter's inventory has no suitable ammo — <c>InventoryBus</c>
    /// responded with <c>AmmoRefused</c>.
    /// </summary>
    NoAmmo,

    /// <summary>
    /// The shooter does not have enough mana for the magical component of the shot —
    /// <c>MagicBus</c> responded with <c>ManaRefused</c>.
    /// </summary>
    NoMana,

    /// <summary>
    /// The weapon is on cooldown (the reload time has not yet elapsed).
    /// </summary>
    WeaponOnCooldown,

    /// <summary>
    /// Target is out of weapon range.
    /// </summary>
    OutOfRange,

    /// <summary>
    /// Target is invalid (destroyed, left the scene, friendly-fire forbidden).
    /// </summary>
    TargetInvalid,
}
