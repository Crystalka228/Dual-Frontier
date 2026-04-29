using DualFrontier.Application.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

/// <summary>
/// Acceptance proof for MOD_OS_ARCHITECTURE.md §2.1 example manifest:
/// every token listed in the example's <c>capabilities.required</c> array
/// must resolve through <see cref="KernelCapabilityRegistry.BuildFromKernelAssemblies"/>.
/// Without this proof the spec example is end-to-end unloadable (Phase C
/// MissingCapability). Per METHODOLOGY §7.3, the test encodes the spec
/// invariant as a falsifiable claim.
/// </summary>
public sealed class ProductionComponentCapabilityTests
{
    [Theory]
    [InlineData("kernel.publish:DualFrontier.Events.Combat.DamageEvent")]
    [InlineData("kernel.publish:DualFrontier.Events.Combat.DeathEvent")]
    [InlineData("kernel.subscribe:DualFrontier.Events.Combat.ShootGranted")]
    [InlineData("kernel.read:DualFrontier.Components.Combat.WeaponComponent")]
    [InlineData("kernel.read:DualFrontier.Components.Combat.ArmorComponent")]
    [InlineData("kernel.read:DualFrontier.Components.Combat.AmmoComponent")]
    [InlineData("kernel.read:DualFrontier.Components.Combat.ShieldComponent")]
    [InlineData("kernel.write:DualFrontier.Components.Shared.HealthComponent")]
    public void Section21ExampleManifest_TokensResolveInKernelRegistry(string token)
    {
        KernelCapabilityRegistry registry = KernelCapabilityRegistry.BuildFromKernelAssemblies();
        registry.Provides(token).Should().BeTrue();
    }

    [Theory]
    [InlineData("kernel.write:DualFrontier.Components.Combat.WeaponComponent")]
    [InlineData("kernel.write:DualFrontier.Components.Combat.ArmorComponent")]
    [InlineData("kernel.write:DualFrontier.Components.Combat.AmmoComponent")]
    [InlineData("kernel.write:DualFrontier.Components.Combat.ShieldComponent")]
    public void ReadOnlyCombatComponents_DoNotProvideWriteTokens(string token)
    {
        // Regression guard: Read = true alone must NOT yield a write token.
        // Defends §3.5 + D-1 against a refactor that conflates the two flags.
        KernelCapabilityRegistry registry = KernelCapabilityRegistry.BuildFromKernelAssemblies();
        registry.Provides(token).Should().BeFalse();
    }

    [Fact]
    public void BuildFromKernelAssemblies_ProducesNonEmptyCapabilitySet()
    {
        // Regression guard for the BuildFromKernelAssemblies bug:
        // pre-fix the method scanned only DualFrontier.Contracts, which
        // holds marker interfaces but no concrete types, returning an
        // empty set. Any future regression that drops Components or
        // Events from the scan list would re-empty the registry and
        // fail this assertion before the per-token tests.
        KernelCapabilityRegistry registry = KernelCapabilityRegistry.BuildFromKernelAssemblies();
        registry.Capabilities.Should().NotBeEmpty();
    }
}
