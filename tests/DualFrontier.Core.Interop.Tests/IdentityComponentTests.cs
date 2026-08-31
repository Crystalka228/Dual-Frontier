using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Core.Interop;
using AwesomeAssertions;
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
        component.Name.Should().Be(StringHandle.Empty,
            "W4 re-typed the field onto the Contracts-side handle; the payload is the same " +
            "(Id, Generation) pair, so default is still the empty sentinel");
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
            Name = world.InternHandle("Aelin Ashford"),
        };

        component.Name.IsEmpty.Should().BeFalse();
        world.Resolve(component.Name).Should().Be("Aelin Ashford");
    }

    [Fact]
    public void EmptyContent_RoundsTripAsEmptySentinel()
    {
        using var world = new NativeWorld();
        var component = new IdentityComponent
        {
            Name = world.InternHandle(string.Empty),
        };

        component.Name.IsEmpty.Should().BeTrue();
        world.Resolve(component.Name).Should().BeNull();
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
        component = new IdentityComponent { Name = world.InternHandle("Mod-only Pawn") };
        component.Name.IsEmpty.Should().BeFalse();
        world.EndModScope("ModX");

        world.ClearModScope("ModX");

        world.Resolve(component.Name).Should().BeNull(
            "post-clear re-intern advances the generation; the captured handle is stale");
    }
}
