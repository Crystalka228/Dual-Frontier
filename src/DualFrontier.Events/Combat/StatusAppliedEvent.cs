using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// На сущность наложен статус-эффект (горение, оглушение, заморозка,
/// порча — см. GDD 6.1). StatusSystem добавляет/обновляет соответствующий
/// компонент-модификатор и планирует его истечение.
/// </summary>
public sealed record StatusAppliedEvent : IEvent
{
    // TODO: public required EntityId TargetId { get; init; }
    // TODO: public required StatusKind Kind { get; init; }       // enum — Фаза 6
    // TODO: public required float Duration { get; init; }
    // TODO: public EntityId? SourceId { get; init; }
}
