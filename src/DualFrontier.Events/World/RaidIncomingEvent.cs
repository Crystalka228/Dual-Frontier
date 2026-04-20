using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.World;

/// <summary>
/// К колонии приближается рейд. Публикуется RaidDirectorSystem с запасом
/// по игровому времени, чтобы колония успела подготовиться. Конкретные
/// стычки триггерят уже события Combat-домена.
/// </summary>
public sealed record RaidIncomingEvent : IEvent
{
    // TODO: public required string FactionId { get; init; } = string.Empty;
    // TODO: public required int EnemyCount { get; init; }
    // TODO: public required float ThreatPoints { get; init; }
    // TODO: public required float EtaSeconds { get; init; }   // сколько игрового времени до нападения
}
