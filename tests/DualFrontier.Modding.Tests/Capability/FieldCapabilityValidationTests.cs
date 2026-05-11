using System;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

/// <summary>
/// Coverage for the K9 capability-pattern extension (MOD_OS v1.7 §2.3 / §4.6).
/// The regex in <see cref="ManifestCapabilities"/> must now accept
/// <c>field.(read|write|acquire|conductivity|storage|dispatch)</c> and
/// <c>pipeline.register</c> as verbs alongside the existing
/// publish/subscribe/read/write set.
/// </summary>
public sealed class FieldCapabilityValidationTests
{
    [Theory]
    [InlineData("kernel.field.read:vanilla.magic.mana")]
    [InlineData("mod.dualfrontier.vanilla.magic.field.write:vanilla.magic.mana")]
    [InlineData("mod.dualfrontier.vanilla.electricity.field.conductivity:vanilla.electricity.power")]
    [InlineData("mod.dualfrontier.vanilla.water.field.dispatch:vanilla.water.pressure")]
    [InlineData("mod.dualfrontier.vanilla.magic.pipeline.register:vanilla.magic.diffusion")]
    [InlineData("kernel.field.acquire:vanilla.magic.mana")]
    [InlineData("mod.vanilla.magic.field.storage:vanilla.magic.mana")]
    public void ValidCapabilityStrings_Parse(string capability)
    {
        var result = ManifestCapabilities.Parse(new[] { capability }, null);
        result.Required.Should().Contain(capability);
    }

    [Theory]
    [InlineData("field.read:vanilla.magic.mana")]                        // missing provider
    [InlineData("kernel.field.unknown:vanilla.magic.mana")]              // invalid verb
    [InlineData("mod.foo.field.read:")]                                  // empty target
    [InlineData("mod.foo.field.read:bad chars in id!")]                  // invalid chars
    [InlineData("mod.foo.pipeline.unknown:vanilla.magic.diffusion")]     // invalid pipeline verb
    public void InvalidCapabilityStrings_FailValidation(string capability)
    {
        Action act = () => ManifestCapabilities.Parse(new[] { capability }, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void PreviousVerbs_StillAccepted()
    {
        // K9 regex extension must not regress the publish/subscribe/read/write
        // verbs the rest of the kernel depends on.
        var result = ManifestCapabilities.Parse(
            new[]
            {
                "kernel.publish:DualFrontier.Events.SomeEvent",
                "kernel.subscribe:DualFrontier.Events.SomeEvent",
                "kernel.read:HealthComponent",
                "kernel.write:HealthComponent"
            },
            null);
        result.Required.Should().HaveCount(4);
    }
}
