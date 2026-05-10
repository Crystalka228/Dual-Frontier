using System;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K8.2 v2 Phase 2.B.4 — round-trip and IsInitialized state semantics for
/// the post-conversion <see cref="SkillsComponent"/> struct + NativeMap×2
/// shape. Verifies the wrapper handles default to invalid sentinel and
/// must be explicitly created via <c>NativeWorld.CreateMap</c> before use.
/// </summary>
public sealed class SkillsComponentTests
{
    [Fact]
    public void Default_LevelsAreInvalidAndNotInitialized()
    {
        SkillsComponent component = default;
        component.Levels.IsValid.Should().BeFalse();
        component.Experience.IsValid.Should().BeFalse();
        component.IsInitialized.Should().BeFalse();
    }

    [Fact]
    public void NewStruct_NotInitialized()
    {
        var component = new SkillsComponent();
        component.IsInitialized.Should().BeFalse();
    }

    [Fact]
    public void CreatedButEmptyMaps_NotInitialized()
    {
        using var world = new NativeWorld();
        var component = new SkillsComponent
        {
            Levels = world.CreateMap<SkillKind, int>(),
            Experience = world.CreateMap<SkillKind, float>(),
        };

        component.Levels.IsValid.Should().BeTrue();
        component.IsInitialized.Should().BeFalse(
            "Levels is valid but empty; IsInitialized requires Count > 0");
    }

    [Fact]
    public void RoundTrip_PopulatedMapsAreInitialized()
    {
        using var world = new NativeWorld();
        var component = new SkillsComponent
        {
            Levels = world.CreateMap<SkillKind, int>(),
            Experience = world.CreateMap<SkillKind, float>(),
        };
        component.Levels.Set(SkillKind.Cooking, 5);
        component.Experience.Set(SkillKind.Cooking, 250f);

        component.IsInitialized.Should().BeTrue();
        component.Levels.TryGet(SkillKind.Cooking, out int level).Should().BeTrue();
        level.Should().Be(5);
        component.Experience.TryGet(SkillKind.Cooking, out float xp).Should().BeTrue();
        xp.Should().Be(250f);
    }

    [Fact]
    public void AllSkillKinds_PopulatedAndIterable()
    {
        using var world = new NativeWorld();
        var component = new SkillsComponent
        {
            Levels = world.CreateMap<SkillKind, int>(),
            Experience = world.CreateMap<SkillKind, float>(),
        };

        var allKinds = (SkillKind[])Enum.GetValues(typeof(SkillKind));
        for (int i = 0; i < allKinds.Length; i++)
        {
            component.Levels.Set(allKinds[i], i);
        }

        component.Levels.Count.Should().Be(allKinds.Length);

        foreach (SkillKind kind in allKinds)
        {
            component.Levels.TryGet(kind, out int found).Should().BeTrue();
            found.Should().Be((int)Array.IndexOf(allKinds, kind));
        }
    }

    [Fact]
    public void DistinctMaps_PerCreate_DoNotShareStorage()
    {
        using var world = new NativeWorld();
        var componentA = new SkillsComponent { Levels = world.CreateMap<SkillKind, int>() };
        var componentB = new SkillsComponent { Levels = world.CreateMap<SkillKind, int>() };

        componentA.Levels.Set(SkillKind.Cooking, 10);
        componentB.Levels.TryGet(SkillKind.Cooking, out _).Should().BeFalse(
            "each CreateMap returns a distinct backing storage");
    }
}
