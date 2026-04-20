using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Терминальное событие жизненного цикла аренды маны. Помечено
/// <see cref="DeferredAttribute"/>: подписчики могут мутировать состояние
/// (снимать эффекты, удалять привязки) — это должно происходить на
/// следующем тике, чтобы не нарушить инварианты текущей фазы.
/// </summary>
/// <param name="Id">Идентификатор закрываемой аренды.</param>
/// <param name="Caster">Маг-кастер.</param>
/// <param name="Reason">Причина закрытия — штатная или аварийная.</param>
/// <param name="TotalManaDrained">Суммарное количество маны, списанное
/// за всё время жизни аренды (для статистики/баланса).</param>
[Deferred]
public sealed record ManaLeaseClosed(
    LeaseId Id,
    EntityId Caster,
    CloseReason Reason,
    float TotalManaDrained) : IEvent;
