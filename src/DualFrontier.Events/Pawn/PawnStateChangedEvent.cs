using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Periodic snapshot of one pawn's HUD-relevant state. Emitted by
/// PawnStateReporterSystem on its SLOW tick; GameBootstrap subscribes and
/// forwards as PawnStateCommand to the presentation bridge.
/// All need values are wellness 0..1 (1 = best), passed through directly
/// from NeedsComponent.
/// </summary>
public sealed record PawnStateChangedEvent : IEvent
{
    public required EntityId PawnId    { get; init; }
    public required string   Name      { get; init; }
    public required float    Satiety   { get; init; }
    public required float    Hydration { get; init; }
    public required float    Energy    { get; init; }
    public required float    Comfort   { get; init; }
    public required float    Mood      { get; init; }
    public required string   JobLabel  { get; init; }
    public required bool     JobUrgent { get; init; }

    /// <summary>
    /// Top 3 skills by descending level. Each entry is the SkillKind and
    /// the integer level (0..SkillsComponent.MaxLevel). May be empty if
    /// the pawn has no SkillsComponent or its Levels dictionary is null.
    /// Order: highest level first; ties broken by SkillKind enum order.
    /// </summary>
    public required IReadOnlyList<(SkillKind Kind, int Level)> TopSkills { get; init; }
}
