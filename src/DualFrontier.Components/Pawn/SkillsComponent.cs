using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Defines the skill levels and experience points for an entity's pawn.
/// </summary>
public sealed class SkillsComponent : IComponent
{
    /// <constants>
    /// The maximum attainable level for any skill.
    /// </constants>
    public const int MaxLevel = 20;

    /// <constants>
    /// The amount of experience points required to advance one skill level.
    /// </constants>
    public const float XpPerLevel = 1000f;

    /// <summary>
    /// Current skill levels per skill. Populated by systems upon pawn creation.
    /// Null-for-null initialization is used due to pooling rules.
    /// </summary>
    // Note: Dictionary fields are NOT initialized in the class body (null by default). Systems initialize them on pawn creation — this is the project rule for pooling. So declare as:
    public Dictionary<SkillKind, int>? Levels = null!;

    /// <summary>
    /// Accumulated XP toward next level per skill. Populated by systems upon pawn creation.
    /// </summary>
    // Note: Dictionary fields are NOT initialized in the class body (null by default). Systems initialize them on pawn creation — this is the project rule for pooling. So declare as:
    public Dictionary<SkillKind, float>? Experience = null!;

    /// <summary>
    /// Checks if the skill levels component has been populated with data.
    /// </summary>
    public bool IsInitialized => Levels != null && Levels.Count > 0;
}