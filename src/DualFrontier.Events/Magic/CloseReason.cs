namespace DualFrontier.Events.Magic;

/// <summary>
/// Причина закрытия аренды маны (<c>ManaLeaseClosed</c>). Помогает
/// подписчикам отличать штатное завершение от аварийного и корректно
/// обновлять состояние (анимации, звуки, счётчики статистики).
/// </summary>
public enum CloseReason
{
    /// <summary>
    /// Аренда завершена штатно — заклинание/эффект отработали до конца.
    /// </summary>
    Completed,

    /// <summary>
    /// Каст прерван внешним воздействием (урон, оглушение, disruption).
    /// </summary>
    SpellInterrupted,

    /// <summary>
    /// Голем-потребитель был деактивирован (см. <c>GolemBondComponent</c>,
    /// GDD 5.2).
    /// </summary>
    GolemDeactivated,

    /// <summary>
    /// Маг-кастер погиб — все его аренды принудительно закрываются.
    /// </summary>
    PawnDied,

    /// <summary>
    /// Мана кастера исчерпана до нуля, продолжать дренирование невозможно.
    /// </summary>
    ManaExhausted,
}
