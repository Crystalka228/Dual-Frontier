namespace DualFrontier.Events.Magic;

/// <summary>
/// Причина отказа в открытии аренды маны (<c>ManaLeaseRefused</c>).
/// Позволяет подписчикам различать типы отказа и реагировать по-разному
/// (например, попробовать другую школу, дождаться регена, выбрать другого мага).
/// </summary>
public enum RefusalReason
{
    /// <summary>
    /// У кастера недостаточно маны для запрошенного <c>DrainPerTick</c>
    /// на минимальной длительности аренды.
    /// </summary>
    InsufficientMana,

    /// <summary>
    /// Превышен лимит одновременно открытых аренд для данного кастера
    /// (защита от «бесконечного» удержания множества эффектов).
    /// </summary>
    LeaseCapExceeded,

    /// <summary>
    /// У кастера нет активной связи с големом/целью (<c>GolemBondComponent</c>
    /// отсутствует или в состоянии <c>Abandoned</c>).
    /// </summary>
    NoActiveBond,

    /// <summary>
    /// Школа запрашивающего заклинания не соответствует школе кастера/цели
    /// — см. <c>SchoolComponent</c>, GDD 4.3.
    /// </summary>
    SchoolMismatch,
}
