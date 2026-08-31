using System;
using DualFrontier.Components.Pawn;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// State semantics of <see cref="SkillsComponent"/> after W4 changed its storage from two
/// native maps to two inline arrays.
///
/// <para>
/// The predecessor file (K8.2 v2 Phase 2.B.4) pinned the map-handle shape: default is the
/// invalid sentinel, a handle must be minted through <c>NativeWorld.CreateMap</c> before use,
/// and two mints do not share storage. None of that survives the change, and none of it should:
/// inline storage has no handle, no mint and no sharing. What DOES survive is the behaviour
/// those tests existed to protect — a pawn whose skills were never rolled must be
/// distinguishable from one whose skills are all zero, and every declared skill must be
/// addressable. Those are re-expressed here against the new shape.
/// </para>
///
/// <para>
/// The distinction is now carried by an explicit flag rather than inferred from handle validity,
/// because inline storage is always present: without the flag, "never populated" and "populated
/// to all zeroes" would be the same bytes.
/// </para>
/// </summary>
public sealed class SkillsComponentTests
{
    [Fact]
    public void Default_IsNotInitialised()
    {
        SkillsComponent component = default;

        component.IsInitialized.Should().BeFalse(
            "a default component has never been populated, and inline storage cannot say so on " +
            "its own — the flag is what carries it");
    }

    [Fact]
    public void PopulatedToAllZeroes_IsStillInitialised()
    {
        var component = new SkillsComponent { Populated = true };
        foreach (SkillKind kind in Enum.GetValues<SkillKind>())
            component.SetLevel(kind, 0);

        component.IsInitialized.Should().BeTrue(
            "this is the case the flag exists for: all-zero skills are populated skills, and a " +
            "handle-validity check could not have told them apart from an unrolled pawn");
    }

    [Fact]
    public void EverySkillKindIsAddressableAndRoundTrips()
    {
        var component = new SkillsComponent { Populated = true };

        foreach (SkillKind kind in Enum.GetValues<SkillKind>())
        {
            component.SetLevel(kind, (int)kind + 1);
            component.SetExperience(kind, (int)kind * 10f);
        }

        foreach (SkillKind kind in Enum.GetValues<SkillKind>())
        {
            component.LevelOf(kind).Should().Be((int)kind + 1);
            component.ExperienceOf(kind).Should().Be((int)kind * 10f);
        }
    }

    [Fact]
    public void TheDeclaredSkillCountMatchesTheEnum()
    {
        Enum.GetValues<SkillKind>().Length.Should().Be(SkillsComponent.SkillCount,
            "the inline arrays are sized by SkillCount, so adding a SkillKind without widening " +
            "the constant would silently make the new skill unaddressable");
    }

    [Fact]
    public void CopiesDoNotShareStorage()
    {
        var original = new SkillsComponent { Populated = true };
        original.SetLevel(SkillKind.Mining, 7);

        SkillsComponent copy = original;
        copy.SetLevel(SkillKind.Mining, 19);

        original.LevelOf(SkillKind.Mining).Should().Be(7,
            "inline storage travels BY VALUE with the struct. The map shape this replaced shared " +
            "storage through a handle, so a copy aliased the original — the opposite behaviour, " +
            "and worth pinning rather than assuming");
        copy.LevelOf(SkillKind.Mining).Should().Be(19);
    }

    [Fact]
    public void AnUndeclaredSkillIsRefusedRatherThanReadingAdjacentStorage()
    {
        var component = new SkillsComponent { Populated = true };
        var undeclared = (SkillKind)SkillsComponent.SkillCount;

        ((Action)(() => component.LevelOf(undeclared))).Should().Throw<ArgumentOutOfRangeException>(
            "an inline array indexed by a cast enum would otherwise read whatever sits after it");
        ((Action)(() => component.SetLevel(undeclared, 1))).Should().Throw<ArgumentOutOfRangeException>();
    }
}
