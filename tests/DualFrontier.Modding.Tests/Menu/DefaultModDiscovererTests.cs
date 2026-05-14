using System;
using System.Collections.Generic;
using System.IO;
using DualFrontier.Application.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Menu;

/// <summary>
/// M7.5.A — <see cref="DefaultModDiscoverer"/> directory-scan
/// behavior. Each test owns its own temp directory tree so the
/// fixtures cannot leak between runs.
///
/// Coverage (4 tests): non-existent root → empty list (first-launch
/// safety), missing manifest skipped, valid manifest parsed,
/// per-manifest parse failure swallowed (best-effort enumeration
/// per the M7.5.A AD).
/// </summary>
public sealed class DefaultModDiscovererTests
{
    // 23
    [Fact]
    public void Discover_OnNonexistentRoot_ReturnsEmpty()
    {
        // First-launch safety: the mods/ directory may not exist on
        // a fresh install. The discoverer must not throw — it must
        // return an empty list so the menu opens cleanly with zero
        // discoverable mods.
        string nonexistent = Path.Combine(Path.GetTempPath(),
            $"dfm-disc-nonexistent-{Guid.NewGuid():N}");
        Directory.Exists(nonexistent).Should().BeFalse(
            "Guid.NewGuid path must not collide with an existing directory");
        var discoverer = new DefaultModDiscoverer(nonexistent);

        IReadOnlyList<DiscoveredModInfo> result = discoverer.Discover();

        result.Should().BeEmpty();
    }

    // 24
    [Fact]
    public void Discover_SkipsDirectoriesWithoutManifest()
    {
        // Build outputs, IDE artifacts, etc. live alongside real mod
        // directories under mods/. The discoverer recognizes a mod
        // by the presence of mod.manifest.json — directories without
        // it are skipped silently.
        using var temp = TempDir.New();
        Directory.CreateDirectory(Path.Combine(temp.Path, "no-manifest-here"));

        var discoverer = new DefaultModDiscoverer(temp.Path);
        IReadOnlyList<DiscoveredModInfo> result = discoverer.Discover();

        result.Should().BeEmpty();
    }

    // 25
    [Fact]
    public void Discover_OnValidFixtureRoot_ReturnsParsedManifests()
    {
        // Happy path — one valid manifest in a subdir. The returned
        // DiscoveredModInfo carries the parsed manifest so the menu
        // can render name/version/hotReload without re-parsing.
        using var temp = TempDir.New();
        const string id = "tests.disc.minimal";
        WriteValidManifest(Path.Combine(temp.Path, id), id);

        var discoverer = new DefaultModDiscoverer(temp.Path);
        IReadOnlyList<DiscoveredModInfo> result = discoverer.Discover();

        DiscoveredModInfo info = result.Should().ContainSingle().Subject;
        info.Manifest.Id.Should().Be(id);
        info.Manifest.Name.Should().Be("Minimal Disc");
        info.Manifest.Version.Should().Be("1.0.0");
    }

    // 26
    [Fact]
    public void Discover_HandlesInvalidManifestGracefully_SkipsBrokenButReturnsValid()
    {
        // Best-effort enumeration per the M7.5.A AD — one corrupt
        // manifest must not hide the rest of the discovered mods.
        // M7.5.B may surface skipped manifests through a separate
        // warning channel; for M7.5.A the discoverer is silent.
        using var temp = TempDir.New();
        WriteValidManifest(Path.Combine(temp.Path, "good.mod"), "tests.disc.good");
        string brokenDir = Path.Combine(temp.Path, "broken.mod");
        Directory.CreateDirectory(brokenDir);
        File.WriteAllText(Path.Combine(brokenDir, "mod.manifest.json"),
            "{ this is not valid JSON");

        var discoverer = new DefaultModDiscoverer(temp.Path);
        IReadOnlyList<DiscoveredModInfo> result = discoverer.Discover();

        result.Should().ContainSingle()
            .Which.Manifest.Id.Should().Be("tests.disc.good");
    }

    private static void WriteValidManifest(string dir, string id)
    {
        Directory.CreateDirectory(dir);
        string json = "{ \"manifestVersion\": \"3\", \"id\": \"" + id + "\", \"name\": \"Minimal Disc\", " +
            "\"version\": \"1.0.0\" }";
        File.WriteAllText(Path.Combine(dir, "mod.manifest.json"), json);
    }

    private sealed class TempDir : IDisposable
    {
        public string Path { get; }

        private TempDir(string path) { Path = path; }

        public static TempDir New()
        {
            string dir = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                $"dfm-disc-{Guid.NewGuid():N}");
            Directory.CreateDirectory(dir);
            return new TempDir(dir);
        }

        public void Dispose()
        {
            try { Directory.Delete(Path, recursive: true); } catch { /* best effort */ }
        }
    }
}
