using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.World;

/// <summary>
/// Сменилась погода. Подписчики: MoodSystem (мудовые модификаторы),
/// CombatSystem (модификаторы точности / урона), VisualSystem (эффекты),
/// FarmSystem (рост растений).
/// </summary>
public sealed record WeatherChangedEvent : IEvent
{
    // TODO: создать DualFrontier.Events.World.WeatherKind enum (Clear, Rain, Storm, EtherStorm, Fog …) — Фаза 4.
    // TODO: public required WeatherKind Kind { get; init; }
    // TODO: public float Intensity { get; init; }
}
