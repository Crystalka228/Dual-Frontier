using System.Linq;
using System.Reflection;
using DualFrontier.Application.Attributes;
using DualFrontier.Application.Rendering;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Rendering;

/// <summary>
/// Verifies the structural invariants of the DevKit-tier renderer contract.
/// These are not runtime tests — they are architecture tests, checking that
/// the type system enforces the tier split as documented.
/// </summary>
public sealed class DevKitRendererContractTests
{
    [Fact]
    public void IDevKitRenderer_Extends_IRenderer()
    {
        typeof(IRenderer)
            .IsAssignableFrom(typeof(IDevKitRenderer))
            .Should().BeTrue(
                "devkit-tier must be a superset of production-tier so callers " +
                "accepting IRenderer can also receive IDevKitRenderer");
    }

    [Fact]
    public void IDevKitRenderer_Has_DevKitOnly_Attribute()
    {
        DevKitOnlyAttribute? attr = typeof(IDevKitRenderer)
            .GetCustomAttributes<DevKitOnlyAttribute>(inherit: false)
            .FirstOrDefault();

        attr.Should().NotBeNull(
            "[DevKitOnly] on the interface is the marker the future Roslyn " +
            "analyser scans for — without it, the tier split is invisible");
        attr!.Reason.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void IRenderer_Does_Not_Have_DevKitOnly_Attribute()
    {
        typeof(IRenderer)
            .GetCustomAttributes<DevKitOnlyAttribute>(inherit: false)
            .Should().BeEmpty(
                "production-tier must remain unrestricted — every backend " +
                "implements IRenderer including Native");
    }

    [Fact]
    public void IDevKitRenderer_Declares_Expected_DebugMethods()
    {
        string[] expected =
        {
            nameof(IDevKitRenderer.DrawDebugGizmo),
            nameof(IDevKitRenderer.ShowSystemProfiler),
            nameof(IDevKitRenderer.HighlightEntity)
        };

        string[] actual = typeof(IDevKitRenderer)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance |
                        BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .ToArray();

        actual.Should().BeEquivalentTo(expected,
            "changing the devkit-tier method list is a doc-worthy event; " +
            "update docs/VISUAL_ENGINE.md before editing this test");
    }
}
