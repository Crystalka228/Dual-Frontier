using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Команда на нанесение урона — в отличие от фактического <c>DamageEvent</c>,
/// это запрос «нанеси урон», который <c>DamageSystem</c> валидирует,
/// применяет резисты/уязвимости и публикует итоговый <c>DamageEvent</c>.
/// </summary>
/// <param name="Source">Источник урона (атакующая сущность, снаряд, ловушка).</param>
/// <param name="Target">Цель урона.</param>
/// <param name="Amount">Сырое количество урона до резистов/модификаторов.</param>
/// <param name="DamageKind">Строковый идентификатор типа урона
/// (TODO: Фаза 4 — заменить на <c>DamageType</c> enum).</param>
public sealed record DamageIntent(
    EntityId Source,
    EntityId Target,
    float Amount,
    string DamageKind) : ICommand;
