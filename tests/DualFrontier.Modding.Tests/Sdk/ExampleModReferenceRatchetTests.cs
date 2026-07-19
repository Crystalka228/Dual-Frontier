using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Sdk;

/// <summary>
/// W1 wave-gate ratchet: the reference example mod
/// (<c>DualFrontier.Mod.Example</c>) must be authored against
/// <c>DualFrontier.Contracts</c> ALONE — no engine assembly. Proves boundary law
/// B-3 (SDK sufficiency): everything a vanilla / third-party mod needs arrives
/// through the SDK contract surface. The referenced-project set is a frozen
/// constant; any growth here means the SDK leaked an engine type into the mod.
/// </summary>
public sealed class ExampleModReferenceRatchetTests
{
    [Fact]
    public void ExampleMod_ProjectReferences_AreContractsOnly()
    {
        string csproj = Path.Combine(
            RepoRoot(), "mods", "DualFrontier.Mod.Example", "DualFrontier.Mod.Example.csproj");
        File.Exists(csproj).Should().BeTrue($"the example mod project must exist at {csproj}");

        XDocument doc = XDocument.Load(csproj);
        System.Collections.Generic.HashSet<string> referenced = doc.Descendants()
            .Where(e => e.Name.LocalName == "ProjectReference")
            .Select(e => (string?)e.Attribute("Include"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => Path.GetFileNameWithoutExtension(v!.Replace('\\', '/')))
            .ToHashSet(StringComparer.Ordinal);

        referenced.Should().BeEquivalentTo(new[] { "DualFrontier.Contracts" },
            "the reference example mod is Contracts-only (boundary law B-3, SDK sufficiency); " +
            "any other project reference means the SDK could not express what the mod needs");
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "DualFrontier.sln")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("repo root (DualFrontier.sln) not found");
    }
}
