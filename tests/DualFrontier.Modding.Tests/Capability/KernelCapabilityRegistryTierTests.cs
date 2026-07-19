using System.Linq;
using System.Reflection;
using DualFrontier.Core.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

// K10.2 — top-level public event types for KernelCapabilityRegistry scan tests.
// FQN must avoid `+` nested-type marker which scan skips.
[EventTier(BusTier.Fast)]
public sealed class K10_2_KCR_FastTokenEvent : IEvent { }

[EventTier(BusTier.Normal)]
public sealed class K10_2_KCR_NormalTokenEvent : IEvent { }

[EventTier(BusTier.Background, CoalesceFunctionTypeName = "T")]
public sealed class K10_2_KCR_BgTokenEvent : IEvent { }

public sealed class K10_2_KCR_DefaultTokenEvent : IEvent { }

/// <summary>
/// K10.2 Item 28 + S-LOCK-4 — KernelCapabilityRegistry tier-prefixed token
/// generation tests.
/// </summary>
public sealed class KernelCapabilityRegistryTierTests
{
    private static KernelCapabilityRegistry RegistryForTestAssembly()
    {
        var reg = new KernelCapabilityRegistry();
        reg.RegisterOwner("kernel", Assembly.GetExecutingAssembly());
        return reg;
    }

    [Fact]
    public void FastEvent_EmitsFastPrefixedTokens()
    {
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_2_KCR_FastTokenEvent).FullName!;

        reg.Capabilities.Should().Contain($"kernel.fast.publish:{fqn}");
        reg.Capabilities.Should().Contain($"kernel.fast.subscribe:{fqn}");
        // Fast tier events не get legacy aliases (legacy is Normal-only)
        reg.Capabilities.Should().NotContain($"kernel.publish:{fqn}");
        reg.Capabilities.Should().NotContain($"kernel.subscribe:{fqn}");
    }

    [Fact]
    public void NormalEvent_EmitsNormalTokens_AndLegacyAliases()
    {
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_2_KCR_NormalTokenEvent).FullName!;

        reg.Capabilities.Should().Contain($"kernel.normal.publish:{fqn}");
        reg.Capabilities.Should().Contain($"kernel.normal.subscribe:{fqn}");
        // Backward-compat aliases per S-LOCK-4
        reg.Capabilities.Should().Contain($"kernel.publish:{fqn}");
        reg.Capabilities.Should().Contain($"kernel.subscribe:{fqn}");
    }

    [Fact]
    public void BackgroundEvent_EmitsBackgroundPrefixedTokens()
    {
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_2_KCR_BgTokenEvent).FullName!;

        reg.Capabilities.Should().Contain($"kernel.background.publish:{fqn}");
        reg.Capabilities.Should().Contain($"kernel.background.subscribe:{fqn}");
        // Background tier events не get Normal aliases
        reg.Capabilities.Should().NotContain($"kernel.publish:{fqn}");
    }

    [Fact]
    public void EventWithoutTierAttribute_DefaultsToNormal_WithLegacyAliases()
    {
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_2_KCR_DefaultTokenEvent).FullName!;

        // S-LOCK-4: default tier is Normal — both tier-explicit и legacy tokens emitted
        reg.Capabilities.Should().Contain($"kernel.normal.publish:{fqn}");
        reg.Capabilities.Should().Contain($"kernel.normal.subscribe:{fqn}");
        reg.Capabilities.Should().Contain($"kernel.publish:{fqn}");
        reg.Capabilities.Should().Contain($"kernel.subscribe:{fqn}");
    }
}
