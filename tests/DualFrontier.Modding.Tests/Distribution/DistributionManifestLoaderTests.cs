using System;
using System.IO;
using DualFrontier.Application.Distribution;
using DualFrontier.Contracts.Distribution;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Distribution;

/// <summary>
/// W4 / D2 — the distribution manifest and its STRICT loader.
///
/// <para>
/// Before W4 the repository had no game-level configuration file and no loader for one; every
/// tunable was a constant in the composition root, which is what left the configuration row of
/// the execution-authority matrix without an owner. These facts pin both halves of what shipped:
/// the shape the manifest must have, and the refusals that make it strict.
/// </para>
///
/// <para>
/// The strictness is the point of contrast with the MOD manifest parser, which asks for the keys
/// it knows and silently ignores the rest — three shipped mod manifests carry a
/// <c>description</c> key nothing has ever read. That forgiveness suits third-party content the
/// host cannot fix. A distribution manifest ships WITH the product, so a typo in it is a defect
/// someone can correct before release, and swallowing it would turn a build error into a
/// mysterious runtime one.
/// </para>
/// </summary>
public sealed class DistributionManifestLoaderTests
{
    private const string Valid = """
    {
      "manifestVersion": "1",
      "product": { "id": "test.product", "name": "Test", "version": "1.2.3" },
      "rootMods": ["mod.a", "mod.b"],
      "scenario": {
        "id": "default",
        "worldSeed": 7,
        "mapWidth": 200,
        "mapHeight": 200,
        "obstacleCount": 800,
        "obstacleSeed": 42,
        "factorySeed": 42,
        "itemFactorySeed": 43,
        "counts": { "pawns": 50, "food": 150, "water": 50, "beds": 30, "decorations": 25 }
      },
      "assetRoots": ["assets"],
      "saveNamespace": "test.save",
      "minEngineCapabilities": []
    }
    """;

    [Fact]
    public void AWellFormedManifestParsesEveryField()
    {
        DistributionManifest m = DistributionManifestLoader.Parse(Valid, "test.json");

        m.ManifestVersion.Should().Be("1");
        m.Product.Id.Should().Be("test.product");
        m.Product.Name.Should().Be("Test");
        m.Product.Version.Should().Be("1.2.3");
        m.RootMods.Should().Equal("mod.a", "mod.b");
        m.AssetRoots.Should().Equal("assets");
        m.SaveNamespace.Should().Be("test.save");
        m.MinEngineCapabilities.Should().BeEmpty();

        ScenarioConfig s = m.Scenario;
        s.Id.Should().Be("default");
        s.WorldSeed.Should().Be(7);
        s.MapWidth.Should().Be(200);
        s.MapHeight.Should().Be(200);
        s.ObstacleCount.Should().Be(800);
        s.ObstacleSeed.Should().Be(42);
        s.FactorySeed.Should().Be(42);
        s.ItemFactorySeed.Should().Be(43);
        s.Counts.Pawns.Should().Be(50);
        s.Counts.Food.Should().Be(150);
        s.Counts.Water.Should().Be(50);
        s.Counts.Beds.Should().Be(30);
        s.Counts.Decorations.Should().Be(25);
    }

    [Fact]
    public void RootModOrderIsPreserved()
    {
        DistributionManifest m = DistributionManifestLoader.Parse(Valid, "test.json");

        m.RootMods.Should().Equal(
            new[] { "mod.a", "mod.b" },
            "the discoverer has no ordering semantics of its own — it returns filesystem order — " +
            "so the manifest's order is the only declared one and must survive the read");
    }

    [Fact]
    public void AnUnknownTopLevelKeyIsRefusedAndNamed()
    {
        string withStray = Valid.Replace("\"saveNamespace\"", "\"saveNamesapce\"");

        Action act = () => DistributionManifestLoader.Parse(withStray, "test.json");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*unknown key*saveNamesapce*",
                "a misspelt key must name itself. The mod parser would have ignored this and " +
                "then failed on the MISSING saveNamespace instead, pointing at the wrong end of " +
                "the same typo");
    }

    [Fact]
    public void AnUnknownNestedKeyIsRefusedWithItsPath()
    {
        string withStray = Valid.Replace("\"pawns\": 50", "\"pawns\": 50, \"colonists\": 50");

        Action act = () => DistributionManifestLoader.Parse(withStray, "test.json");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*scenario.counts.colonists*",
                "the path matters: 'unknown key colonists' would leave the author hunting for " +
                "which of four objects it was in");
    }

    [Fact]
    public void KeysAreCaseSensitive()
    {
        string wrongCase = Valid.Replace("\"rootMods\"", "\"RootMods\"");

        Action act = () => DistributionManifestLoader.Parse(wrongCase, "test.json");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*RootMods*",
                "the mod parser matches case-insensitively because it must tolerate hand-authored " +
                "third-party files; a distribution manifest ships with the product, so a casing " +
                "slip is a defect to name rather than a spelling to absorb");
    }

    [Theory]
    [InlineData("manifestVersion")]
    [InlineData("product")]
    [InlineData("rootMods")]
    [InlineData("scenario")]
    [InlineData("assetRoots")]
    [InlineData("saveNamespace")]
    [InlineData("minEngineCapabilities")]
    public void EveryTopLevelFieldIsRequired(string field)
    {
        string without = RemoveKey(Valid, field);

        Action act = () => DistributionManifestLoader.Parse(without, "test.json");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*{field}*",
                "no field defaults. The mod parser defaults kind, apiVersion, hotReload, " +
                "replaces, dependencies and capabilities when absent; a distribution declares " +
                "all of itself or none of it");
    }

    [Fact]
    public void AWrongManifestVersionIsRefusedWithBothVersionsNamed()
    {
        string v2 = Valid.Replace("\"manifestVersion\": \"1\"", "\"manifestVersion\": \"2\"");

        Action act = () => DistributionManifestLoader.Parse(v2, "test.json");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*'2'*")
            .WithMessage("*'1'*", "the reader must be told what it got AND what this build accepts");
    }

    [Fact]
    public void AFieldOfTheWrongTypeIsRefused()
    {
        string stringSeed = Valid.Replace("\"worldSeed\": 7", "\"worldSeed\": \"7\"");

        Action act = () => DistributionManifestLoader.Parse(stringSeed, "test.json");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*worldSeed*integer*");
    }

    [Fact]
    public void MalformedJsonIsRefusedWithTheSourcePathNamed()
    {
        Action act = () => DistributionManifestLoader.Parse("{ not json", "somewhere/game.manifest.json");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*somewhere/game.manifest.json*",
                "one loader may serve several distributions; the failure must say which file");
    }

    [Fact]
    public void TheShippedManifestIsValidAndDescribesTheVanillaColony()
    {
        // The real artifact, resolved the way production resolves it: an upward walk from the
        // directory holding this test binary. That is the same mechanism the Launcher uses, so
        // this fact also proves the resolution works from a bin directory several levels deep.
        string path = DistributionManifestLoader.Locate();
        DistributionManifest m = DistributionManifestLoader.Load(path);

        m.Product.Id.Should().Be("dualfrontier.vanilla");
        m.Scenario.Counts.Pawns.Should().Be(50,
            "the colony the integration suite asserts on is 50 pawns; the manifest now carries " +
            "that number, which used to be a constant in the composition root");
        m.Scenario.MapWidth.Should().Be(200);
        m.Scenario.MapHeight.Should().Be(200);
        m.RootMods.Should().Contain("dualfrontier.vanilla.core");
    }

    [Fact]
    public void AnExplicitPathThatDoesNotExistFailsNamingThePath()
    {
        // Locate()'s own not-found arm cannot be reached from inside the repository, where the
        // upward walk always succeeds; what IS reachable is the explicit-path arm, and the thing
        // worth pinning about both is that the failure names the file. A loader that says only
        // "not found" is the least actionable failure a launcher can produce.
        string absent = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "game.manifest.json");

        Action act = () => DistributionManifestLoader.Load(absent);

        act.Should().Throw<IOException>()
            .WithMessage($"*{Path.GetFileName(absent)}*");
    }

    private static string RemoveKey(string json, string key)
    {
        int start = json.IndexOf($"\"{key}\"", StringComparison.Ordinal);
        start.Should().BeGreaterThan(-1, $"the fixture must contain '{key}'");

        // Walk to the end of this member: past its value, stopping at the comma or the closing
        // brace that ends it at THIS nesting depth.
        int i = json.IndexOf(':', start) + 1;
        int depth = 0;
        while (i < json.Length)
        {
            char c = json[i];
            if (c is '{' or '[') depth++;
            else if (c is '}' or ']')
            {
                if (depth == 0) break;
                depth--;
            }
            else if (c == ',' && depth == 0) { i++; break; }
            i++;
        }
        return json.Remove(start, i - start);
    }
}
