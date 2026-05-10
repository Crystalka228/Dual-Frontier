using DualFrontier.Components.Shared;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K8.2 v2 Phase 2.B.3 — round-trip and sentinel-comparison semantics for
/// the post-conversion <see cref="FactionComponent"/> struct + InternedString
/// shape. Verifies the const-string-to-handle pattern: literals
/// <c>FactionComponent.PlayerFactionIdString</c> and
/// <c>FactionComponent.NeutralFactionIdString</c> intern through the world
/// to produce handles comparable against the field via
/// <see cref="InternedString.EqualsByContent"/>.
/// </summary>
public sealed class FactionComponentTests
{
    [Fact]
    public void Default_FactionIdIsEmpty_AndNeutral()
    {
        FactionComponent component = default;
        component.FactionId.IsEmpty.Should().BeTrue();
        component.IsHostile.Should().BeFalse();
        component.IsPlayer.Should().BeFalse();
        component.IsNeutral.Should().BeTrue();
    }

    [Fact]
    public void NewStruct_DefaultsToNeutral()
    {
        var component = new FactionComponent();
        component.IsNeutral.Should().BeTrue();
    }

    [Fact]
    public void Roundtrip_FactionIdInternedAndResolves()
    {
        using var world = new NativeWorld();
        var component = new FactionComponent
        {
            FactionId = world.InternString(FactionComponent.PlayerFactionIdString),
            IsPlayer = true,
        };

        component.FactionId.Resolve(world).Should().Be("colony");
        component.IsPlayer.Should().BeTrue();
        component.IsNeutral.Should().BeFalse();
    }

    [Fact]
    public void EqualsByContent_PlayerSentinel_MatchesInternedHandle()
    {
        using var world = new NativeWorld();
        var component = new FactionComponent
        {
            FactionId = world.InternString(FactionComponent.PlayerFactionIdString),
        };

        InternedString playerHandle = world.InternString(FactionComponent.PlayerFactionIdString);
        component.FactionId.EqualsByContent(playerHandle, world, world).Should().BeTrue();

        InternedString neutralHandle = world.InternString(FactionComponent.NeutralFactionIdString);
        component.FactionId.EqualsByContent(neutralHandle, world, world).Should().BeFalse();
    }

    [Fact]
    public void IsHostile_AndIsPlayer_AreMutuallyConsistentWithIsNeutral()
    {
        var hostile = new FactionComponent { IsHostile = true };
        hostile.IsNeutral.Should().BeFalse();

        var player = new FactionComponent { IsPlayer = true };
        player.IsNeutral.Should().BeFalse();

        var neutral = new FactionComponent();
        neutral.IsNeutral.Should().BeTrue();
    }

    [Fact]
    public void ModScopeReclaim_StaleFactionResolvesNull()
    {
        using var world = new NativeWorld();
        FactionComponent component;

        world.BeginModScope("ModFaction");
        component = new FactionComponent
        {
            FactionId = world.InternString("mod.faction.exclusive"),
        };
        component.FactionId.IsEmpty.Should().BeFalse();
        world.EndModScope("ModFaction");

        world.ClearModScope("ModFaction");

        component.FactionId.Resolve(world).Should().BeNull();
    }
}
