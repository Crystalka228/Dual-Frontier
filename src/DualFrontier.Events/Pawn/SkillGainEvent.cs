using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Навык пешки получил прогресс. SkillSystem увеличивает уровень в
/// <c>SkillsComponent</c> и публикует это событие для UI / истории.
/// </summary>
public sealed record SkillGainEvent : IEvent
{
    // TODO: public required EntityId PawnId { get; init; }
    // TODO: public required SkillKind Skill { get; init; }   // enum — см. Components/Pawn/SkillsComponent
    // TODO: public required int NewLevel { get; init; }
    // TODO: public int Delta { get; init; } = 1;
}
