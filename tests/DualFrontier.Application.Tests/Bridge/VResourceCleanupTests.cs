using System;
using DualFrontier.Application.Bridge;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Application.Tests.Bridge;

/// <summary>
/// К10.3 v2 Item 42 + S-LOCK-12 — Step 3.6 V resource cleanup placeholder
/// tests. Verifies the К10.3 v2 wrapper surface returns vacuous success;
/// full implementation deferred к V-cycle / К-extensions.
/// </summary>
public sealed class VResourceCleanupTests
{
    [Fact]
    public void UnloadModResources_returns_success_with_zero_counts()
    {
        var cleanup = new VResourceCleanup();

        VResourceCleanup.Result result = cleanup.UnloadModResources("test.mod.id");

        result.Success.Should().BeTrue();
        result.PipelinesDestroyed.Should().Be(0);
        result.DescriptorSetsDestroyed.Should().Be(0);
        result.BuffersDestroyed.Should().Be(0);
        result.ImagesDestroyed.Should().Be(0);
        result.ErrorMessages.Should().BeEmpty();
    }

    [Fact]
    public void UnloadModResources_with_null_modId_throws()
    {
        var cleanup = new VResourceCleanup();

        cleanup.Invoking(c => c.UnloadModResources(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UnloadModResources_handles_empty_modId()
    {
        var cleanup = new VResourceCleanup();

        VResourceCleanup.Result result = cleanup.UnloadModResources(string.Empty);

        result.Success.Should().BeTrue();
    }
}
