using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Power;

/// <summary>
/// Перегрузка сети: суммарное потребление превышает выработку. PowerSystem
/// отключает потребителей в порядке приоритета; UI/звук воспроизводят
/// предупреждение. Если перегрузка повторяется — предложение игроку
/// построить ещё производителей.
/// </summary>
public sealed record GridOverloadEvent : IEvent
{
    // TODO: public required PowerType Type { get; init; }
    // TODO: public required float Demand { get; init; }
    // TODO: public required float Supply { get; init; }
}
