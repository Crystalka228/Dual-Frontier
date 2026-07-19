using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Analyzers.Tests;

/// <summary>
/// §3.8 repo-discipline meta-tests: read the src tree and assert census shape.
/// Materializes the TESTING_STRATEGY §4 census contracts as compiled tests — until
/// Phase β these were executed only by rg at closure (§7). File enumeration is
/// git-tracked (git ls-files) to mirror rg's .gitignore semantics.
/// </summary>
public sealed class CensusMetaTests
{
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "DualFrontier.sln")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("repo root (DualFrontier.sln) not found");
    }

    private static IReadOnlyList<string> TrackedSrcCsFiles()
    {
        string root = RepoRoot();
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "ls-files -- src",
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        }) ?? throw new InvalidOperationException("could not start git");

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var files = new List<string>();
        foreach (string line in output.Split('\n'))
        {
            string rel = line.Trim();
            if (rel.EndsWith(".cs", StringComparison.Ordinal))
            {
                files.Add(Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar)));
            }
        }

        files.Should().NotBeEmpty("git ls-files must find tracked src/*.cs");
        return files;
    }

    private static (int sites, int files) Census(Func<string, int> countInFile, Func<string, bool>? fileFilter = null)
    {
        int sites = 0;
        int files = 0;
        foreach (string file in TrackedSrcCsFiles())
        {
            if (fileFilter is not null && !fileFilter(file))
            {
                continue;
            }

            int n = countInFile(File.ReadAllText(file));
            if (n > 0)
            {
                sites += n;
                files++;
            }
        }

        return (sites, files);
    }

    private static int RegexCount(string text, string pattern, RegexOptions options)
        => Regex.Matches(text, pattern, options).Count;

    private static int LiteralCount(string text, string literal)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(literal, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += literal.Length;
        }

        return count;
    }

    [Fact]
    public void ReservedSurfaceCensus_MatchesExactPin()
    {
        // TESTING_STRATEGY §4.1 — EXACT pin (HARD): [ReservedStub application sites in
        // src, excluding the attribute definition file. 34 / 13. Phase β adds none;
        // any movement here is a defect.
        var (sites, files) = Census(
            text => LiteralCount(text, "[ReservedStub"),
            file => !file.EndsWith("ReservedStubAttribute.cs", StringComparison.Ordinal));

        sites.Should().Be(34, "reserved-surface exact pin (TESTING_STRATEGY §4.1)");
        files.Should().Be(13, "reserved-surface file pin (TESTING_STRATEGY §4.1)");
    }

    [Theory]
    [InlineData("stub", @"\bstub\b", true, 51, 20)]
    [InlineData("deferred", @"\bdeferred\b", true, 88, 54)]
    [InlineData("TODO", @"\bTODO\b", false, 136, 53)]
    [InlineData("not yet", "not yet", true, 10, 9)]
    public void MarkerFamilyCensus_MatchesPin(string name, string pattern, bool ignoreCase, int sitePin, int filePin)
    {
        // TESTING_STRATEGY §4.2 — marker-family census. Live pins as of Phase β. The
        // stub (48→51) / deferred (79→82) drift from the 2026-06-11 dated snapshot is
        // the F-25 owed refresh, materialized here. A future move updates the pin AND
        // records a census-delta (CODING_STANDARDS §8 / RESERVED_SURFACE §5).
        // EQ_A2 (Cascade B): deferred 82->86 / 51->52 -- the S3 shutdown-transaction
        // deferred-drop primitive (DomainEventBus.DropDeferred, GameServices.DropDeferred,
        // EngineSession.Dispose; EngineSession.cs is the +1 file).
        // W1 (SDK unlock): deferred 86->88 / 52->54 -- the C2 Contracts SDK docs
        // (Sdk/ISystemContext.cs deliberate-deferral note + Sdk/SpanScope.cs K7-deferred
        // caveat; both new files). Recorded here in C3 as the census-delta: C2 introduced
        // the two documentation sites and shipped before this census (Analyzers.Tests) ran.
        // W1-fix (Codex review): deferred 88->89 / 54->55 -- Sdk/ISystemServices.cs now
        // records that the mod-facing IModApi factory overload is deferred (N17).
        // W2/BD-3 (C3): deferred 89->88 / 55->54 -- EventBusAttribute.cs deleted with the
        // genre taxonomy; its /// [Deferred] doc-example line (line 15) was the file's only
        // deferred site. ModBusRouter.cs was also deleted this commit but carried no census
        // markers, so no other pin moves.
        var options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        var (sites, files) = Census(text => RegexCount(text, pattern, options));

        sites.Should().Be(sitePin, $"{name} marker-family census sites (TESTING_STRATEGY §4.2)");
        files.Should().Be(filePin, $"{name} marker-family census files (TESTING_STRATEGY §4.2)");
    }

    [Fact]
    public void PhaseSixMarkerCensus_MatchesPin()
    {
        var (sites, files) = Census(text => LiteralCount(text, "Phase 6"));

        sites.Should().Be(23, "Phase 6 marker-family census sites (TESTING_STRATEGY §4.2)");
        files.Should().Be(11, "Phase 6 marker-family census files (TESTING_STRATEGY §4.2)");
    }

    [Fact]
    public void DfkWaiverCensus_MatchesPin()
    {
        // TESTING_STRATEGY §4.3 — DFK-WAIVER census. Pin 2 as of A'.9.1 Phase β C9:
        // the two DFK001 waivers in ValidationLayer.cs (VK_EXT_debug_utils Vulkan
        // interop, К-L19), each carrying its CODING_STANDARDS §5.3 authority citation.
        // §4.4: every `// DFK-WAIVER(` marker pairs with a `#pragma warning disable`;
        // every future increase updates this pin + adds a citation.
        var (disables, _) = Census(text => RegexCount(text, @"#pragma warning disable (DFK|DFL|DF9)", RegexOptions.None));
        var (markers, _) = Census(text => LiteralCount(text, "// DFK-WAIVER("));

        disables.Should().Be(2, "DFK-WAIVER census pin (TESTING_STRATEGY §4.3) — 2 DFK001 Vulkan-interop waivers");
        markers.Should().Be(disables, "every '#pragma warning disable DFK/DFL/DF9' pairs with a '// DFK-WAIVER(' marker");
    }

    [Fact]
    public void SuppressMessageCensus_IsZero()
    {
        var (sites, _) = Census(text => LiteralCount(text, "SuppressMessage"));

        sites.Should().Be(0, "no [SuppressMessage] in src (DF999 baseline)");
    }
}
