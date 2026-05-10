using DualFrontier.Components.Building;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K8.2 v2 Phase 2.B.2 — round-trip and idle-state semantics for the
/// post-conversion <see cref="WorkbenchComponent"/> struct + InternedString
/// shape. The struct's <see cref="WorkbenchComponent.IsIdle"/> property
/// re-routes from null-check on the legacy string to <c>IsEmpty</c> on
/// the new InternedString sentinel.
/// </summary>
public sealed class WorkbenchComponentTests
{
    [Fact]
    public void Default_IsIdle_True()
    {
        WorkbenchComponent component = default;
        component.IsIdle.Should().BeTrue();
        component.ActiveRecipeId.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void NewStruct_IsIdleAndUnoccupied()
    {
        var component = new WorkbenchComponent();
        component.IsIdle.Should().BeTrue();
        component.IsOccupied.Should().BeFalse();
    }

    [Fact]
    public void SettingActiveRecipe_FlipsIsIdleFalse()
    {
        using var world = new NativeWorld();
        var component = new WorkbenchComponent
        {
            ActiveRecipeId = world.InternString("recipe.bread"),
        };

        component.IsIdle.Should().BeFalse();
        component.ActiveRecipeId.Resolve(world).Should().Be("recipe.bread");
    }

    [Fact]
    public void ClearingActiveRecipe_FlipsIsIdleTrue()
    {
        using var world = new NativeWorld();
        var component = new WorkbenchComponent
        {
            ActiveRecipeId = world.InternString("recipe.steel-bar"),
        };

        component.ActiveRecipeId = InternedString.Empty;

        component.IsIdle.Should().BeTrue();
    }

    [Fact]
    public void IsOccupied_TracksWorkerEntityIndex()
    {
        var component = new WorkbenchComponent { WorkerEntityIndex = 42 };
        component.IsOccupied.Should().BeTrue();

        component.WorkerEntityIndex = 0;
        component.IsOccupied.Should().BeFalse();
    }

    [Fact]
    public void ModScopeReclaim_StaleRecipeResolvesNull()
    {
        using var world = new NativeWorld();
        WorkbenchComponent component;

        world.BeginModScope("ModRecipes");
        component = new WorkbenchComponent
        {
            ActiveRecipeId = world.InternString("mod.recipe.unique"),
        };
        component.IsIdle.Should().BeFalse();
        world.EndModScope("ModRecipes");

        world.ClearModScope("ModRecipes");

        component.ActiveRecipeId.Resolve(world).Should().BeNull();
    }
}
