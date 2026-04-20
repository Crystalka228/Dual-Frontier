using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Шаг 2 двухшаговой модели: ManaSystem отказывает в списании — маны
/// недостаточно. Получатель отменяет действие (каст, активацию голема).
/// Долгое истощение → цепная реакция на големов (см. GDD 5.2).
/// </summary>
public sealed record ManaRefused : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public required float RequestedAmount { get; init; }
    // TODO: public required float AvailableAmount { get; init; }
}
