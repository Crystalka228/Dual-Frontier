using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Подтверждение открытия аренды маны. Публикуется <c>ManaSystem</c>
/// немедленно после того, как первый тик дренажа был успешно списан с кастера.
/// Подписчики (VFX/SFX, UI) включают визуализацию непрерывного эффекта.
/// </summary>
/// <param name="Id">Идентификатор аренды — используется для привязки
/// последующего <c>ManaLeaseClosed</c>.</param>
/// <param name="Caster">Маг-кастер.</param>
/// <param name="DrainPerTick">Фактический расход маны за тик.</param>
public sealed record ManaLeaseOpened(
    LeaseId Id,
    EntityId Caster,
    float DrainPerTick) : IEvent;
