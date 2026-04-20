using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Уровни навыков пешки. Прокачиваются SkillSystem при выполнении работ.
/// Уровни используются NeedsSystem/JobSystem для скорости и качества работ.
/// </summary>
public sealed class SkillsComponent : IComponent
{
    // TODO: создать DualFrontier.Components.Pawn.SkillKind enum (Construction, Mining, Cooking, Combat, Magic, Social …) — Фаза 2
    // TODO: public Dictionary<SkillKind, int> Levels = new();
}
