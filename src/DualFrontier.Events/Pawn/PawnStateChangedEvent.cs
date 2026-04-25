using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Periodic snapshot of one pawn's HUD-relevant state. Emitted by
/// PawnStateReporterSystem on its SLOW tick; GameBootstrap subscribes and
/// forwards as PawnStateCommand to the presentation bridge.
/// All need values are wellness 0..1 (1 = best), already inverted from
/// NeedsComponent deficit semantics.
/// </summary>
public sealed record PawnStateChangedEvent : IEvent
{
    public required EntityId PawnId    { get; init; }
    public required string   Name      { get; init; }
    public required float    Hunger    { get; init; }
    public required float    Thirst    { get; init; }
    public required float    Rest      { get; init; }
    public required float    Comfort   { get; init; }
    public required float    Mood      { get; init; }
    public required string   JobLabel  { get; init; }
    public required bool     JobUrgent { get; init; }
}
