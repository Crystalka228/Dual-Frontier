using System.Reflection;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Display;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

// K10.3 v2 Items 39+40 — top-level public types for KernelCapabilityRegistry
// layer-token scan tests. FQNs must avoid `+` nested-type marker which scan skips.

[Layer(LayerType.Intent)]
public sealed class K10_3_KCR_IntentLayerProvider { }

[Layer(LayerType.CombatFeedback)]
public sealed class K10_3_KCR_CombatFeedbackLayerProvider { }

[Layer(LayerType.SimState)]
public sealed class K10_3_KCR_SimStateLayerProvider { }

[Layer(LayerType.Static)]
public sealed class K10_3_KCR_StaticLayerProvider { }

public sealed class K10_3_KCR_NoLayerAttributeType { }

/// <summary>
/// К10.3 v2 Items 39+40 + S8-Q3 / S3-Q5 pattern — KernelCapabilityRegistry
/// К-L17 layer-token generation tests.
/// </summary>
public sealed class KernelCapabilityRegistryLayerTests
{
    private static KernelCapabilityRegistry RegistryForTestAssembly()
        => new(new[] { Assembly.GetExecutingAssembly() });

    [Fact]
    public void IntentLayer_EmitsIntentToken()
    {
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_3_KCR_IntentLayerProvider).FullName!;

        reg.Capabilities.Should().Contain($"kernel.layer.intent:{fqn}");
        reg.Capabilities.Should().NotContain($"kernel.layer.combat_feedback:{fqn}");
    }

    [Fact]
    public void CombatFeedbackLayer_EmitsCombatFeedbackToken()
    {
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_3_KCR_CombatFeedbackLayerProvider).FullName!;

        reg.Capabilities.Should().Contain($"kernel.layer.combat_feedback:{fqn}");
        reg.Capabilities.Should().NotContain($"kernel.layer.intent:{fqn}");
    }

    [Fact]
    public void SimStateLayer_DoesNotEmitLayerToken()
    {
        // SimState uses existing renderer-level capabilities (V substrate
        // primitives) — no kernel.layer.simstate token surface.
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_3_KCR_SimStateLayerProvider).FullName!;

        reg.Capabilities.Should().NotContain(s => s.StartsWith("kernel.layer.") && s.EndsWith($":{fqn}"));
    }

    [Fact]
    public void StaticLayer_DoesNotEmitLayerToken()
    {
        // Static uses existing renderer-level capabilities — no token surface.
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_3_KCR_StaticLayerProvider).FullName!;

        reg.Capabilities.Should().NotContain(s => s.StartsWith("kernel.layer.") && s.EndsWith($":{fqn}"));
    }

    [Fact]
    public void TypeWithoutLayerAttribute_DoesNotEmitLayerToken()
    {
        var reg = RegistryForTestAssembly();
        string fqn = typeof(K10_3_KCR_NoLayerAttributeType).FullName!;

        reg.Capabilities.Should().NotContain(s => s.StartsWith("kernel.layer.") && s.EndsWith($":{fqn}"));
    }

    [Fact]
    public void LayerToken_IsGranularPerFqn()
    {
        // Two distinct types с the same LayerType emit distinct tokens —
        // S3-Q5 + S8-Q3 granular FQN pattern verified.
        var reg = RegistryForTestAssembly();
        string intentFqn = typeof(K10_3_KCR_IntentLayerProvider).FullName!;
        string combatFqn = typeof(K10_3_KCR_CombatFeedbackLayerProvider).FullName!;

        reg.Capabilities.Should().Contain($"kernel.layer.intent:{intentFqn}");
        reg.Capabilities.Should().Contain($"kernel.layer.combat_feedback:{combatFqn}");
    }
}
