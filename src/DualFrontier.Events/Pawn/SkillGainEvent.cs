using DualFrontier.Contracts.Core;
using DualFrontier.Components.Pawn;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by SkillSystem when a pawn skill level increases.
/// Used by UI and statistics — actual level is stored in SkillsComponent.
/// </summary>
public sealed record SkillGainEvent : IEvent
{
    /// <summary>Pawn whose skill increased.</summary>
    public required EntityId PawnId { get; init; }

    /// <summary>Skill kind that improved.</summary>
    public required SkillKind Skill { get; init; }

    /// <summary>New skill level after gain.</summary>
    public required int NewLevel { get; init; }

    /// <summary>Amount gained (usually 1).</summary>
    public int Delta { get; init; } = 1;
}