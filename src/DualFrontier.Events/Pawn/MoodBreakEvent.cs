using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Настроение пешки упало ниже <c>MoodBreakThreshold</c>: психологический
/// срыв. Конкретная реакция (побег, депрессия, берсерк) определяется
/// BreakdownSystem в зависимости от черт характера и тяжести.
/// </summary>
public sealed record MoodBreakEvent : IEvent
{
    // TODO: public required EntityId PawnId { get; init; }
    // TODO: public required int Severity { get; init; }   // 1..3 — Фаза 3
    // TODO: public float MoodAtBreak { get; init; }
}
