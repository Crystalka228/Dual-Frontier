using System;
using System.Runtime.CompilerServices;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Thirteen <c>int</c>s, one per <see cref="SkillKind"/>, laid out inline.
/// </summary>
[InlineArray(SkillsComponent.SkillCount)]
public struct SkillLevelArray
{
    private int _element0;
}

/// <summary>
/// Thirteen <c>float</c>s, one per <see cref="SkillKind"/>, laid out inline.
/// </summary>
[InlineArray(SkillsComponent.SkillCount)]
public struct SkillExperienceArray
{
    private float _element0;
}

/// <summary>
/// Defines the skill levels and experience points for an entity's pawn.
///
/// <para>
/// <b>W4 — storage changed from native maps to inline arrays.</b> The two fields were
/// <c>NativeMap&lt;SkillKind, int&gt;</c> / <c>&lt;SkillKind, float&gt;</c>, minted through
/// <c>NativeWorld.CreateMap</c>. That is a <c>Core.Interop</c> call, and once the vanilla
/// scenario is authored from a mod there is no route to it: the SDK exposes composites and
/// string interning but no keyed-map surface, and <c>ISystemContext</c>'s standing rule is that
/// no <c>Core.Interop</c> type ever crosses it.
/// </para>
///
/// <para>
/// The alternative was to grow Contracts a whole <c>MapHandle&lt;K,V&gt;</c> primitive family.
/// It was rejected on the project's own precedent: the mod-facing factory registration overload
/// was held back at W1 explicitly because it had no consumer yet, and a map primitive has
/// exactly one consumer here, with its second (<c>StorageComponent</c>) belonging to W5. A
/// primitive family should ride its second consumer. The skill roster is a compile-time constant
/// — thirteen values — so an inline array expresses it without any SDK surface at all, and
/// removes two native allocations per pawn (a hundred at colony start) as a side effect.
/// </para>
///
/// <para>
/// Indexing is by <see cref="SkillKind"/> cast to <c>int</c>, which is safe because the enum is
/// contiguous from zero; <see cref="LevelOf"/> and <see cref="ExperienceOf"/> are the intended
/// accessors and range-check on the caller's behalf.
/// </para>
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct SkillsComponent : IComponent
{
    /// <summary>The number of declared <see cref="SkillKind"/> values.</summary>
    public const int SkillCount = 13;

    /// <summary>The maximum attainable level for any skill.</summary>
    public const int MaxLevel = 20;

    /// <summary>The amount of experience points required to advance one skill level.</summary>
    public const float XpPerLevel = 1000f;

    /// <summary>Current level per skill, indexed by <see cref="SkillKind"/>.</summary>
    public SkillLevelArray Levels;

    /// <summary>Accumulated XP toward the next level per skill, indexed by <see cref="SkillKind"/>.</summary>
    public SkillExperienceArray Experience;

    /// <summary>
    /// Set by whoever populates the component. Inline storage is always PRESENT, so presence can
    /// no longer stand in for population the way a map handle's validity did: a pawn whose skills
    /// were never rolled and a pawn whose skills are all zero would otherwise be indistinguishable.
    /// This flag keeps that distinction explicit rather than inferring it from the data.
    /// </summary>
    public bool Populated;

    /// <summary>True once the skills have been populated.</summary>
    public readonly bool IsInitialized => Populated;

    /// <summary>The level for <paramref name="kind"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a declared skill.</exception>
    public readonly int LevelOf(SkillKind kind) => Levels[Index(kind)];

    /// <summary>The accumulated experience for <paramref name="kind"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a declared skill.</exception>
    public readonly float ExperienceOf(SkillKind kind) => Experience[Index(kind)];

    /// <summary>Sets the level for <paramref name="kind"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a declared skill.</exception>
    public void SetLevel(SkillKind kind, int level) => Levels[Index(kind)] = level;

    /// <summary>Sets the accumulated experience for <paramref name="kind"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is not a declared skill.</exception>
    public void SetExperience(SkillKind kind, float experience)
        => Experience[Index(kind)] = experience;

    private static int Index(SkillKind kind)
    {
        int i = (int)kind;
        if (i < 0 || i >= SkillCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(kind), kind,
                $"SkillKind must be one of the {SkillCount} declared values.");
        }
        return i;
    }
}
