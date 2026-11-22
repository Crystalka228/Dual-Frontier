using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Analyzers.Tests;

/// <summary>
/// Repo-discipline meta-tests (TESTING_STRATEGY section 3.8): they read the src tree
/// and assert the section 4 census pins from inside the suite at every run; the
/// registered rg expressions stay the closure-audit cross-check (section 7). Files
/// are enumerated with git ls-files: the census counts tracked files only, so an
/// untracked file under src/ never moves a pin even where rg would count it.
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
        // TESTING_STRATEGY section 4.1: [ReservedStub application sites in src, the
        // attribute definition file excluded. The pin is EXACT, not a bound, and no
        // direction is asserted: reserved surface may close or grow, but a commit that
        // moves the count updates this pin in the same commit and records the delta
        // (Skeleton revisions, RESERVED_SURFACE_MUTABILITY; CODING_STANDARDS section 8).
        var (sites, files) = Census(
            text => LiteralCount(text, "[ReservedStub"),
            file => !file.EndsWith("ReservedStubAttribute.cs", StringComparison.Ordinal));

        sites.Should().Be(34, "reserved-surface exact pin (TESTING_STRATEGY §4.1)");
        files.Should().Be(13, "reserved-surface file pin (TESTING_STRATEGY §4.1)");
    }

    [Theory]
    [InlineData("stub", @"\bstub\b", true, 51, 20)]
    [InlineData("deferred", @"\bdeferred\b", true, 86, 53)]
    [InlineData("TODO", @"\bTODO\b", false, 132, 51)]
    [InlineData("not yet", "not yet", true, 10, 9)]
    public void MarkerFamilyCensus_MatchesPin(string name, string pattern, bool ignoreCase, int sitePin, int filePin)
    {
        // TESTING_STRATEGY section 4.2: one census per doc-tag family registered in
        // CODING_STANDARDS section 5.2, pattern and case-sensitivity as registered. Both
        // pins are EXACT. The regex counts words, not intent: a mechanism or model name
        // (a deferred-dispatch path, a deferred-destroy flush) moves the deferred pin
        // exactly as a deferral note does, so a moved pin is updated here in the same
        // commit and its census-delta recorded in the commit body (CODING_STANDARDS
        // section 8.2), never reworded away. The pins impose no retroactive cleanup.
        var options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        var (sites, files) = Census(text => RegexCount(text, pattern, options));

        sites.Should().Be(sitePin, $"{name} marker-family census sites (TESTING_STRATEGY §4.2)");
        files.Should().Be(filePin, $"{name} marker-family census files (TESTING_STRATEGY §4.2)");
    }

    [Fact]
    public void PhaseSixMarkerCensus_MatchesPin()
    {
        // TESTING_STRATEGY section 4.2: the literal family, matched as an ordinal
        // substring rather than a word regex; same exact-pin contract as the theory above.
        var (sites, files) = Census(text => LiteralCount(text, "Phase 6"));

        sites.Should().Be(23, "Phase 6 marker-family census sites (TESTING_STRATEGY §4.2)");
        files.Should().Be(11, "Phase 6 marker-family census files (TESTING_STRATEGY §4.2)");
    }

    [Fact]
    public void DfkWaiverCensus_MatchesPin()
    {
        // TESTING_STRATEGY section 4.3: the DFK-WAIVER census. The pin is EXACT; the
        // waivers it counts are enumerated in that section. Every
        // '#pragma warning disable DFK/DFL/DF9' pairs with a '// DFK-WAIVER(' marker
        // carrying its CODING_STANDARDS section 5.3 authority citation, so an increase
        // updates this pin and lands with its citation in the same commit.
        var (disables, _) = Census(text => RegexCount(text, @"#pragma warning disable (DFK|DFL|DF9)", RegexOptions.None));
        var (markers, _) = Census(text => LiteralCount(text, "// DFK-WAIVER("));

        disables.Should().Be(3, "DFK-WAIVER census pin (TESTING_STRATEGY §4.3) — 2 DFK001 Vulkan-interop waivers + 1 DFK022 persistence-decode waiver");
        markers.Should().Be(disables, "every '#pragma warning disable DFK/DFL/DF9' pairs with a '// DFK-WAIVER(' marker");
    }

    [Fact]
    public void SuppressMessageCensus_IsZero()
    {
        // TESTING_STRATEGY section 4.3: [SuppressMessage] in src stays at zero; the
        // paired DFK-WAIVER form above is the only lawful suppression.
        var (sites, _) = Census(text => LiteralCount(text, "SuppressMessage"));

        sites.Should().Be(0, "no [SuppressMessage] in src (DF999 baseline)");
    }
}
