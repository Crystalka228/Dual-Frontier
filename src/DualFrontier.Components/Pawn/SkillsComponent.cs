using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Defines the skill levels and experience points for an entity's pawn.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct SkillsComponent : IComponent
{
    /// <summary>The maximum attainable level for any skill.</summary>
    public const int MaxLevel = 20;

    /// <summary>The amount of experience points required to advance one skill level.</summary>
    public const float XpPerLevel = 1000f;

    /// <summary>
    /// Current skill levels per skill. Native-backed map handle; default
    /// (<see cref="NativeMap{TKey,TValue}"/>.IsValid == false) for un-initialised
    /// pawns. Populated by the scenario factory or SkillSystem at pawn creation
    /// via <c>NativeWorld.CreateMap&lt;SkillKind, int&gt;()</c>.
    /// </summary>
    public NativeMap<SkillKind, int> Levels;

    /// <summary>
    /// Accumulated XP toward next level per skill. Same NativeMap shape as
    /// <see cref="Levels"/>; populated alongside it.
    /// </summary>
    public NativeMap<SkillKind, float> Experience;

    /// <summary>
    /// Checks if the skill levels component has been populated with data.
    /// True when the underlying NativeMap is bound (Levels.IsValid) and
    /// holds at least one entry.
    /// </summary>
    public bool IsInitialized => Levels.IsValid && Levels.Count > 0;
}
