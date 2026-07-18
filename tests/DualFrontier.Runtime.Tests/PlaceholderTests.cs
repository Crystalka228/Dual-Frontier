using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests;

public sealed class PlaceholderTests
{
    [Fact]
    public void Runtime_scaffold_compiles_and_references_runtime_project()
    {
        // V0.A Commit 2 scaffold smoke test — confirms test project + Runtime project reference +
        // xunit + AwesomeAssertions wiring. Subsequent V0.A commits add per-module tests
        // (Window, VulkanInstance, ValidationLayer, VulkanDevice).
        var runtimeType = typeof(Runtime);
        runtimeType.FullName.Should().Be("DualFrontier.Runtime.Runtime");
    }
}
