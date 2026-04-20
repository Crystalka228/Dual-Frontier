using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Отказ в открытии аренды маны. Публикуется <c>ManaSystem</c> в ответ на
/// <c>ManaLeaseOpenRequest</c>, если одно из условий не выполнено
/// (см. <see cref="RefusalReason"/>). Инициатор (SpellCastSystem, AI)
/// должен выбрать альтернативное действие.
/// </summary>
/// <param name="Caster">Маг-кастер, которому отказано.</param>
/// <param name="Reason">Причина отказа.</param>
/// <param name="AvailableMana">Фактически доступное значение маны на момент
/// отказа (для диагностики и UI).</param>
public sealed record ManaLeaseRefused(
    EntityId Caster,
    RefusalReason Reason,
    float AvailableMana) : IEvent;
