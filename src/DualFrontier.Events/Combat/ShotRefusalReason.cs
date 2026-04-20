namespace DualFrontier.Events.Combat;

/// <summary>
/// Причина отказа в проведении составного выстрела (<c>ShootRefused</c>).
/// Позволяет AI/UI различать типы отказа: искать патрон, ждать регена маны,
/// сменить цель и т. п.
/// </summary>
public enum ShotRefusalReason
{
    /// <summary>
    /// В инвентаре стрелка нет подходящего патрона — <c>InventoryBus</c>
    /// ответил <c>AmmoRefused</c>.
    /// </summary>
    NoAmmo,

    /// <summary>
    /// У стрелка недостаточно маны для магической составляющей выстрела —
    /// <c>MagicBus</c> ответил <c>ManaRefused</c>.
    /// </summary>
    NoMana,

    /// <summary>
    /// Оружие на кулдауне (время перезарядки ещё не истекло).
    /// </summary>
    WeaponOnCooldown,

    /// <summary>
    /// Цель вне дальности оружия.
    /// </summary>
    OutOfRange,

    /// <summary>
    /// Цель невалидна (уничтожена, покинула сцену, friendly-fire запрещён).
    /// </summary>
    TargetInvalid,
}
