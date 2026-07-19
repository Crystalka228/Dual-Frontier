using DualFrontier.Core.Modding;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

/// <summary>
/// W2/BD-10 wave-gate proof, inverting the retired kernel-scan acceptance. The genre taxonomy
/// left the engine contract, so <see cref="KernelCapabilityRegistry"/> is a registration
/// ledger, not a kernel-assembly scanner: a fresh ledger provides ZERO gameplay FQN under
/// <c>kernel.*</c>. The MOD_OS §2.1 example manifest's <c>kernel.*</c> gameplay tokens are no
/// longer kernel-provided -- those types ride the v1 grace path this wave and become
/// <c>mod.&lt;id&gt;</c>-owned once the vanilla mods own them. (The pre-BD-10 suite asserted the
/// exact opposite -- that BuildFromKernelAssemblies resolved these tokens and the set was
/// non-empty; that behavior is the thing this wave removes.)
/// </summary>
public sealed class ProductionComponentCapabilityTests
{
    [Theory]
    [InlineData("kernel.publish:DualFrontier.Events.Combat.DamageEvent")]
    [InlineData("kernel.publish:DualFrontier.Events.Combat.DeathEvent")]
    [InlineData("kernel.subscribe:DualFrontier.Events.Combat.ShootGranted")]
    [InlineData("kernel.read:DualFrontier.Components.Combat.ArmorComponent")]
    [InlineData("kernel.write:DualFrontier.Components.Shared.HealthComponent")]
    public void GenreTypes_AreNotProvidedUnderKernel_AfterBD10(string token)
    {
        var registry = new KernelCapabilityRegistry();
        registry.Provides(token).Should().BeFalse(
            "W2/BD-10 removed the kernel-assembly scan — the engine owns no gameplay types");
    }

    [Fact]
    public void FreshLedger_HasEmptyKernelSurface()
    {
        // The BD-10 inverse of the retired "BuildFromKernelAssemblies_ProducesNonEmptyCapabilitySet".
        new KernelCapabilityRegistry().Capabilities.Should().BeEmpty();
    }
}
