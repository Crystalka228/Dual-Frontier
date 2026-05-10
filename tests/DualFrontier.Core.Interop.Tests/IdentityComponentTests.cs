using DualFrontier.Components.Pawn;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K8.2 v2 Phase 2.B.1 — round-trip and mod-scope semantics for the
/// post-conversion <see cref="IdentityComponent"/> struct + InternedString
/// shape. Mirrors the test pattern formalised by K-Lessons §2.3 (mod-scope
/// test isolation: hold all references inside the scope window).
/// </summary>
public sealed class IdentityComponentTests
{
    [Fact]
    public void Default_NameIsEmptySentinel()
    {
        IdentityComponent component = default;
        component.Name.IsEmpty.Should().BeTrue();
        component.Name.Should().Be(InternedString.Empty);
    }

    [Fact]
    public void NewStruct_NameIsEmptySentinel()
    {
        var component = new IdentityComponent();
        component.Name.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Roundtrip_InternedNameResolvesBackToContent()
    {
        using var world = new NativeWorld();
        var component = new IdentityComponent
        {
            Name = world.InternString("Aelin Ashford"),
        };

        component.Name.IsEmpty.Should().BeFalse();
        component.Name.Resolve(world).Should().Be("Aelin Ashford");
    }

    [Fact]
    public void EmptyContent_RoundsTripAsEmptySentinel()
    {
        using var world = new NativeWorld();
        var component = new IdentityComponent
        {
            Name = world.InternString(string.Empty),
        };

        component.Name.IsEmpty.Should().BeTrue();
        component.Name.Resolve(world).Should().BeNull();
    }

    [Fact]
    public void ModScopeReclaim_StaleHandleResolvesNull()
    {
        // Per METHODOLOGY v1.5 K-Lessons §2.3 — references taken inside the
        // scope-under-test only. The component handle is captured before the
        // ClearModScope call; after reclaim, the (id, generation) pair is stale
        // and Resolve returns null.
        using var world = new NativeWorld();
        IdentityComponent component;

        world.BeginModScope("ModX");
        component = new IdentityComponent { Name = world.InternString("Mod-only Pawn") };
        component.Name.IsEmpty.Should().BeFalse();
        world.EndModScope("ModX");

        world.ClearModScope("ModX");

        component.Name.Resolve(world).Should().BeNull(
            "post-clear re-intern advances the generation; the captured handle is stale");
    }
}
