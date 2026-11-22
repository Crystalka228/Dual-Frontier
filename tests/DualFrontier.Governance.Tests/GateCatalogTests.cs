using System.Text.RegularExpressions;
using DualFrontier.Governance;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>
/// The gate registry equals the FRAMEWORK 14.5 table, and every registered gate is
/// one the tool emits. A gate on one side only is a red test, not a prose drift.
/// </summary>
public sealed class GateCatalogTests
{
    private static readonly Regex GateRow = new(@"^\|\s*\*\*(G-[A-Z0-9-]+)\*\*\s*\|", RegexOptions.Compiled);

    /// <summary>The gate ids of the first-column cells of the FRAMEWORK 14.5 table.</summary>
    internal static IReadOnlyList<string> FrameworkGateIds(string frameworkText)
    {
        string[] lines = frameworkText.Replace("\r\n", "\n").Split('\n');
        int start = Array.FindIndex(lines, l => l.StartsWith("### 14.5", StringComparison.Ordinal));
        start.Should().BeGreaterThanOrEqualTo(0, "the FRAMEWORK carries a 14.5 gate-catalog section");

        var ids = new List<string>();
        for (int i = start + 1; i < lines.Length && !lines[i].StartsWith("#", StringComparison.Ordinal); i++)
        {
            Match m = GateRow.Match(lines[i]);
            if (m.Success)
            {
                ids.Add(m.Groups[1].Value);
            }
        }

        return ids;
    }

    private const string FrameworkFixture =
        "### 14.4 The sentinel law\n\n" +
        "| **G-OTHER** | a row of an earlier table is not the catalog | x |\n\n" +
        "### 14.5 The gate catalog\n\n" +
        "| Gate | Statement | Cites |\n|---|---|---|\n" +
        "| **G-SCHEMA** | s | x |\n| **G-UNIQUE** | s | x |\n| **G-XREF** | s | x |\n" +
        "| **G-CATLIFE** | s | x |\n| **G-TERMINAL** | s | x |\n| **G-SENTINEL** | s | x |\n" +
        "| **G-PATH** | s | x |\n| **G-NAMESPACE** | s | x |\n| **G-GLOBALS** | s | x |\n" +
        "| **G-RATIO** | s | x |\n| **G-ROWFORM** | s | x |\n\n" +
        "### 14.6 The globals protocol\n\n" +
        "| **G-AFTER** | a row of a later table is not the catalog | x |\n";

    [Fact]
    public void Registry_HasNoDuplicates_AndIsNonEmpty()
    {
        GateCatalog.All.Should().NotBeEmpty();
        GateCatalog.All.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Parity_Green_FixtureTableEqualsRegistry()
    {
        IReadOnlyList<string> framework = FrameworkGateIds(FrameworkFixture);

        framework.Should().BeEquivalentTo(GateCatalog.All);
    }

    [Fact]
    public void Parity_Red_FixtureMissingAGateDiffersFromRegistry()
    {
        string missingRowForm = FrameworkFixture.Replace("| **G-ROWFORM** | s | x |\n", string.Empty);
        string extraGate = FrameworkFixture.Replace("| **G-RATIO** |", "| **G-INVENTED** | s | x |\n| **G-RATIO** |");

        FrameworkGateIds(missingRowForm).Should().NotBeEquivalentTo(GateCatalog.All);
        FrameworkGateIds(extraGate).Should().NotBeEquivalentTo(GateCatalog.All);
    }

    [Fact]
    public void Parity_RealFramework_TableEqualsRegistry()
    {
        string path = Path.Combine(RepoPaths.RepoRoot(), "docs", "governance", "FRAMEWORK.md");

        FrameworkGateIds(File.ReadAllText(path)).Should().BeEquivalentTo(GateCatalog.All,
            "FRAMEWORK 14.5 and the tool's registry name the same gates");
    }

    [Fact]
    public void EveryRegisteredGate_IsEmittedByTheTool_AndNoOtherGateIs()
    {
        using var root = new TempRoot();
        root.Write("docs/governance/ORPHAN.md", "# no frontmatter\n");
        root.Write("tools/governance/LEDGER_FORM.yaml", LedgerRowFormTests.ConfigYaml);
        root.Write("docs/ROADMAP.md", LedgerRowFormTests.Table("| F-2 | f | S9 | OPEN | r |"));
        root.Write("docs/governance/RISKS.yaml", "risks:\n  - id: RISK-1\n    impact: Catastrophic\n");

        var emitted = new HashSet<string>(StringComparer.Ordinal);
        RegisterSync.CorpusLoad load = RegisterSync.LoadCorpus(root.Path);
        emitted.UnionWith(load.WalkFindings.Select(f => f.Gate));

        var docs = new List<Frontmatter>
        {
            Fixture.Doc(("register_id", "DOC-D-DUP"), ("category", null)),
            Fixture.Doc(("register_id", "DOC-D-DUP"), ("tier", 1), ("lifecycle", "SUPERSEDED"), ("next_review_due", "someday")),
            Fixture.Doc(("register_id", "DOC-A-WRONG"), ("supersedes", new List<object?> { "DOC-A-MISSING" })),
        };
        emitted.UnionWith(Validators.RunAll(docs).Select(f => f.Gate));
        emitted.UnionWith(GlobalsCollections.Load(RepoPaths.GovernanceDir(root.Path)).Validate().Select(f => f.Gate));
        emitted.UnionWith(LedgerRowForm.Validate(root.Path).Select(f => f.Gate));

        emitted.Should().BeEquivalentTo(GateCatalog.All);
    }
}

/// <summary>A scratch repo root under the temp directory, removed on dispose.</summary>
internal sealed class TempRoot : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"df-gov-root-{Guid.NewGuid():N}");

    public TempRoot() => Directory.CreateDirectory(Path);

    public void Write(string relPath, string content)
    {
        string abs = System.IO.Path.Combine(Path, relPath.Replace('/', System.IO.Path.DirectorySeparatorChar));
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(abs)!);
        File.WriteAllText(abs, content);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(Path, recursive: true);
        }
        catch (IOException)
        {
            // best-effort temp cleanup
        }
    }
}
