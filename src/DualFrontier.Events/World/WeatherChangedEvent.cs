using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.World;

/// <summary>
/// Weather has changed. Subscribers: MoodSystem (mood modifiers),
/// CombatSystem (accuracy / damage modifiers), VisualSystem (effects),
/// FarmSystem (plant growth).
/// </summary>
public sealed record WeatherChangedEvent : IEvent
{
    // TODO: create the DualFrontier.Events.World.WeatherKind enum (Clear, Rain, Storm, EtherStorm, Fog ...) — Phase 4.
    // TODO: public required WeatherKind Kind { get; init; }
    // TODO: public float Intensity { get; init; }
}
