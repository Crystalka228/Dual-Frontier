using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Systems.Combat;
using DualFrontier.Systems.Pawn;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Systems.Tests.Combat;

/// <summary>
/// Guards for the M6.1 bridge-annotation contract: every Phase 5 combat stub
/// must carry <c>[BridgeImplementation(Phase = 5, Replaceable = true)]</c>
/// so mods may supersede them once M9 Vanilla.Combat lands. Phase 3 stubs
/// (SocialSystem, SkillSystem) must remain <c>Replaceable = false</c> until
/// M10.C — that boundary is enforced by tests at the bottom of this file.
/// Per MOD_OS_ARCHITECTURE §7 LOCKED.
/// </summary>
public sealed class Phase5BridgeAnnotationsTests
{
    [Fact]
    public void CombatSystem_HasReplaceableBridgeAttribute()
    {
        AssertReplaceablePhase5(typeof(CombatSystem));
    }

    [Fact]
    public void DamageSystem_HasReplaceableBridgeAttribute()
    {
        AssertReplaceablePhase5(typeof(DamageSystem));
    }

    [Fact]
    public void ProjectileSystem_HasReplaceableBridgeAttribute()
    {
        AssertReplaceablePhase5(typeof(ProjectileSystem));
    }

    [Fact]
    public void StatusEffectSystem_HasReplaceableBridgeAttribute()
    {
        AssertReplaceablePhase5(typeof(StatusEffectSystem));
    }

    [Fact]
    public void ComboResolutionSystem_HasReplaceableBridgeAttribute()
    {
        AssertReplaceablePhase5(typeof(ComboResolutionSystem));
    }

    [Fact]
    public void CompositeResolutionSystem_HasReplaceableBridgeAttribute()
    {
        AssertReplaceablePhase5(typeof(CompositeResolutionSystem));
    }

    [Fact]
    public void SkillSystem_RemainsProtected()
    {
        // Phase 3 stub — annotation must NOT yet flip to Replaceable=true.
        // M10.C will reopen this when the Vanilla.Pawn replacement lands.
        BridgeImplementationAttribute attr = RequireBridge(typeof(SkillSystem));
        attr.Phase.Should().Be(3);
        attr.Replaceable.Should().BeFalse(
            "SkillSystem belongs to the M10.C boundary, not M6.1; flipping " +
            "this without coordinating with the Pawn replacement plan would " +
            "let mods supersede a system whose contract is not yet stable");
    }

    private static void AssertReplaceablePhase5(System.Type systemType)
    {
        BridgeImplementationAttribute attr = RequireBridge(systemType);
        attr.Phase.Should().Be(5,
            $"{systemType.Name} is a Phase 5 bridge stub");
        attr.Replaceable.Should().BeTrue(
            $"{systemType.Name} must be flagged Replaceable so mods may " +
            "supersede it via mod.manifest.json's `replaces` field once M9 " +
            "Vanilla.Combat lands");
    }

    private static BridgeImplementationAttribute RequireBridge(System.Type systemType)
    {
        BridgeImplementationAttribute? attr =
            systemType.GetCustomAttribute<BridgeImplementationAttribute>(inherit: false);
        attr.Should().NotBeNull(
            $"{systemType.Name} must carry [BridgeImplementation] until its " +
            "phase replacement lands");
        return attr!;
    }
}
