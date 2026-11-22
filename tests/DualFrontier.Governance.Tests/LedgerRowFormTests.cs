using DualFrontier.Governance;
using System.Text.RegularExpressions;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>
/// G-ROWFORM over fixtures that copy the real ledger row shapes: a missing closing
/// pipe, a stray unescaped pipe, an escaped pipe, an out-of-order id, glyph and
/// bold state cells, and out-of-legend severity and state values.
/// </summary>
public sealed class LedgerRowFormTests
{
    internal const string ConfigYaml =
        "schema_version: \"1.0\"\n" +
        "ledger_path: \"docs/ROADMAP.md\"\n" +
        "id_header: \"F-#\"\n" +
        "severity_header: \"Severity\"\n" +
        "state_header: \"State\"\n" +
        "severity_set: [\"S0\", \"S1\", \"S2\", \"S3\", \"N\"]\n" +
        "state_set: [\"OPEN\", \"CLOSED\", \"ACCEPTED-NO-ACTION\"]\n";

    private static readonly LedgerFormConfig Config = new(
        "docs/ROADMAP.md", "F-#", "Severity", "State",
        new[] { "S0", "S1", "S2", "S3", "N" },
        new[] { "OPEN", "CLOSED", "ACCEPTED-NO-ACTION" });

    // Real shapes, bodies shortened; every pipe, glyph and emphasis marker is placed as in the ledger.
    private const string RowPlain = "| F-1 | `REGISTER_RENDER.md` self-declares register v2.0 | S2 | CLOSED | Render regenerated at the final commit |";
    private const string RowBoldClosed = "| F-2 | `PENDING-COMMIT` placeholders across REGISTER.yaml | S2 | **CLOSED** | placeholders dropped at the flip |";
    private const string RowEscapedPipe = "| F-9 | KERNEL counting-convention divergence risk (table composition vs doc-stated \u00ab21 final\u00bb) | S3 | **CLOSED** (2026-06-22) | closed; the dual form `a \\| b` stays |";
    private const string RowLargerId = "| F-13 | `render_register.ps1` variable-expansion defect | S2 | CLOSED | the script is retired |";
    private const string RowDatedClosed = "| F-39 | a finding closed with a date | S3 | CLOSED (2026-07-17) | closed at the flip |";
    private const string RowGlyphClosed = "| F-51 | **CLOSED at the guard fix (2026-08-26) \u2014 the guard was scoped to the wrong boundary.** As ledgered | S2 | \u2705 CLOSED 2026-08-26 | the guard now covers the boundary | ";
    private const string RowNoClosingPipeA = "| F-55 | W1 SDK event story is incomplete for REGULAR mods | S2 | OPEN | Owner: architect";
    private const string RowNoClosingPipeB = "| F-56 | the context-capturing wrapper nests | S3 | OPEN | Owner: architect";
    private const string RowStrayPipe =
        "| F-59 | **CLOSED at the span fix (2026-08-22) \u2014 span pair-iterators no longer fabricate versions** | S2 | \u2705 CLOSED 2026-08-22 | " +
        "fixed with a mutex; the red test is watched failing then restored | **review addendum (2026-08-22).** the cascade verified it";
    private const string RowStrayPipeEscaped =
        "| F-59 | **CLOSED at the span fix (2026-08-22) \u2014 span pair-iterators no longer fabricate versions** | S2 | \u2705 CLOSED 2026-08-22 | " +
        "fixed with a mutex; the red test is watched failing then restored \\| **review addendum (2026-08-22).** the cascade verified it |";
    private const string RowGlyphlessClosed = "| F-60 | an ALC leak | S2 | CLOSED | closed on the tick path |";
    private const string RowPlannedState = "| F-65 | **No structural rule forbids a strong Type-keyed container** | S3 | Planned | Owner: architect |";
    private const string RowSeverityS4 = "| F-49 | a severity beyond the legend | S4 | CLOSED | closed |";
    private const string RowSeverityW2 = "| F-54 | a severity of another scheme | W2 | OPEN | open |";

    /// <summary>A ledger file fixture: a legend paragraph, the header, the separator, the rows, then trailing prose.</summary>
    internal static string Table(params string[] rows) =>
        "## Findings ledger (F-series)\n\n" +
        "> Severity: S0 critical / S1 high / S2 medium / S3 low / N note. State:\n" +
        "> OPEN / CLOSED / ACCEPTED-NO-ACTION.\n\n" +
        "| F-# | Finding | Severity | State | Resolution / owner |\n" +
        "|---|---|---|---|---|\n" +
        string.Join("\n", rows) +
        "\n\nA paragraph after the table | with a pipe | is not a row.\n";

    private static string RowAt(int number) => $"| F-{number} | a finding | S2 | OPEN | r |";

    private static IReadOnlyList<Finding> Check(params string[] rows) =>
        LedgerRowForm.ValidateTable(Table(rows).Split('\n'), Config);

    private static IEnumerable<Finding> Structural(IReadOnlyList<Finding> f) => f.Where(x => x.Severity == Severity.Report);

    private static IEnumerable<Finding> Vocabulary(IReadOnlyList<Finding> f) => f.Where(x => x.Severity == Severity.Monitor);

    // --- green ---

    [Fact]
    public void RowForm_Green_WellFormedRows_AcrossTheRealShapes()
    {
        IReadOnlyList<Finding> findings = Check(RowPlain, RowBoldClosed, RowEscapedPipe, RowLargerId, RowDatedClosed, RowGlyphClosed, RowStrayPipeEscaped, RowGlyphlessClosed);

        findings.Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Green_EscapedPipeIsNotACellBoundary()
    {
        Check(RowEscapedPipe).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Green_TextAfterTheTable_IsNotRead()
    {
        // the closing paragraph carries pipes and no id; reading it as a row would add findings
        Check(RowPlain).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Green_CrlfLedger()
    {
        string crlf = Table(RowPlain, RowBoldClosed).Replace("\n", "\r\n");
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml);
        root.Write("docs/ROADMAP.md", crlf);

        LedgerRowForm.Validate(root.Path).Should().BeEmpty();
    }

    // --- structural: red (Report) ---

    [Fact]
    public void RowForm_Red_MissingClosingPipe()
    {
        IReadOnlyList<Finding> findings = Check(RowPlain, RowNoClosingPipeA, RowNoClosingPipeB);

        Structural(findings).Should().HaveCount(2);
        Structural(findings).Should().OnlyContain(f => f.Gate == GateCatalog.RowForm && f.Message.Contains("closing pipe"));
        Structural(findings).Select(f => f.Message).Should().Contain(m => m.StartsWith("F-55")).And.Contain(m => m.StartsWith("F-56"));
        Vocabulary(findings).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Red_StrayUnescapedPipe_IsAWrongCellCountAndAMissingClosingPipe()
    {
        IReadOnlyList<Finding> findings = Check(RowStrayPipe);

        Structural(findings).Should().HaveCount(2);
        Structural(findings).Should().Contain(f => f.Message.Contains("6 cells") && f.Message.Contains("declares 5"));
        Structural(findings).Should().Contain(f => f.Message.Contains("closing pipe"));
    }

    [Fact]
    public void RowForm_Red_EscapingTheStrayPipe_RestoresTheRow()
    {
        Structural(Check(RowStrayPipe)).Should().NotBeEmpty();
        Structural(Check(RowStrayPipeEscaped)).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Red_RowEndingInAnEscapedPipe_HasNoClosingPipe()
    {
        // the last cell ends in an escaped pipe: five cells are counted, yet no unescaped pipe closes the row
        IReadOnlyList<Finding> findings = Check("| F-2 | a finding | S2 | OPEN | the dual form `a \\|");

        Structural(findings).Should().ContainSingle();
        Structural(findings).Should().ContainSingle(f => f.Message.StartsWith("F-2") && f.Message.Contains("closing pipe"));
    }

    [Fact]
    public void RowForm_Red_FindingSubject_NamesThePathAndTheOneBasedLineOfTheRow()
    {
        string[] lines = Table(RowPlain, RowNoClosingPipeA).Split('\n');
        int headerLine = Array.FindIndex(lines, l => l.StartsWith("| F-# |", StringComparison.Ordinal)) + 1;

        IReadOnlyList<Finding> findings = LedgerRowForm.ValidateTable(lines, Config);

        // the header, then the separator, then the first row; the faulty row is the second data row
        Structural(findings).Should().ContainSingle().Which.Subject.Should().Be($"docs/ROADMAP.md:{headerLine + 3}");
    }

    [Fact]
    public void RowForm_Red_RowsAfterABlankLine_AreFlaggedNotSilentlyUnread()
    {
        // a blank line ends the table; the rows behind it (one without a closing pipe, one a duplicate) are read by nothing
        IReadOnlyList<Finding> findings = Check(RowAt(1), RowAt(2), "", "| F-3 | no closing pipe | S2 | OPEN | r", RowAt(3));

        Structural(findings).Should().HaveCount(2);
        Structural(findings).Should().OnlyContain(f => f.Message.Contains("follows the end of the table"));
        Structural(findings).Select(f => f.Message).Should().OnlyContain(m => m.StartsWith("F-3"));
    }

    [Fact]
    public void RowForm_Red_AnIndentedRow_EndsTheTableAndIsFlagged()
    {
        IReadOnlyList<Finding> findings = Check(RowAt(1), " " + RowAt(2));

        Structural(findings).Should().ContainSingle(f => f.Message.StartsWith("F-2") && f.Message.Contains("follows the end of the table"));
    }

    [Fact]
    public void RowForm_Green_RealLedger_HasNoStructuralFinding()
    {
        string root = RepoPaths.RepoRoot();

        Structural(LedgerRowForm.Validate(root)).Should().BeEmpty("the ledger the repository names keeps its row form");
        File.Exists(Path.Combine(root, "tools", "governance", "LEDGER_FORM.yaml")).Should().BeTrue("the gate is armed on this repository");
    }

    [Fact]
    public void RowForm_Red_TooFewCells()
    {
        IReadOnlyList<Finding> findings = Check("| F-3 | a finding | S2 | OPEN |");

        Structural(findings).Should().ContainSingle(f => f.Message.Contains("4 cells"));
    }

    [Fact]
    public void RowForm_Red_IdBehindALargerId()
    {
        IReadOnlyList<Finding> findings = Check(RowPlain, RowLargerId, RowBoldClosed, RowGlyphlessClosed);

        Structural(findings).Should().ContainSingle(f => f.Message.StartsWith("F-2") && f.Message.Contains("F-13 comes before"));
    }

    [Fact]
    public void RowForm_Red_OneDisplacedLargeId_IsOneFinding_NotOneForEachRowBehindIt()
    {
        // F-13 sits ahead of three rows that follow each other in order: only the first of them is out of order
        IReadOnlyList<Finding> findings = Check(RowPlain, RowLargerId, RowAt(2), RowAt(3), RowAt(4));

        Structural(findings).Should().ContainSingle();
        Structural(findings).Single().Message.Should().StartWith("F-2").And.Contain("F-13 comes before");
    }

    [Fact]
    public void RowForm_Green_IdsInNumericOrderEvenWhenLexicallyUnordered()
    {
        Check(RowBoldClosed, RowEscapedPipe, RowLargerId).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Red_DuplicateId()
    {
        IReadOnlyList<Finding> findings = Check(RowBoldClosed, RowBoldClosed);

        Structural(findings).Should().ContainSingle(f => f.Message.Contains("duplicate ledger id"));
    }

    [Theory]
    [InlineData("| G-7 | wrong prefix | S2 | OPEN | r |")]
    [InlineData("| F-x | non-numeric | S2 | OPEN | r |")]
    [InlineData("| F-7a | trailing letter | S2 | OPEN | r |")]
    [InlineData("|  | empty id | S2 | OPEN | r |")]
    public void RowForm_Red_IdIsNotOfTheFormFDashNumber(string row)
    {
        Structural(Check(row)).Should().Contain(f => f.Message.Contains("not of the form F-<n>"));
    }

    [Fact]
    public void RowForm_Red_MissingTableHeader_IsNotSilentlyGreen()
    {
        var findings = LedgerRowForm.ValidateTable(new[] { "# a ledger with no table", "| a | b |" }, Config);

        findings.Should().ContainSingle(f => f.Severity == Severity.Report && f.Message.Contains("cannot read the ledger"));
    }

    // --- vocabulary: Monitor ---

    [Fact]
    public void RowForm_Vocabulary_SeverityOutsideTheLegend_IsAMonitor()
    {
        IReadOnlyList<Finding> findings = Check(RowSeverityS4, RowSeverityW2);

        Structural(findings).Should().BeEmpty();
        Vocabulary(findings).Should().HaveCount(2);
        Vocabulary(findings).Should().Contain(f => f.Message.Contains("\"S4\"")).And.Contain(f => f.Message.Contains("\"W2\""));
    }

    [Fact]
    public void RowForm_Vocabulary_StateReadByItsLeadingMember()
    {
        // bold, dated, glyph-prefixed and plain states all lead with a legend member
        Check(RowBoldClosed, RowEscapedPipe, RowDatedClosed, RowGlyphClosed, RowGlyphlessClosed).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Vocabulary_StateOutsideTheLegend_IsAMonitor()
    {
        IReadOnlyList<Finding> findings = Check(RowPlannedState);

        Structural(findings).Should().BeEmpty();
        Vocabulary(findings).Should().ContainSingle(f => f.Message.Contains("\"Planned\""));
    }

    [Fact]
    public void RowForm_Vocabulary_AcceptedNoActionIsOneMember()
    {
        Check("| F-3 | a finding | N | ACCEPTED-NO-ACTION | r |").Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Vocabulary_IsReadOnTheHeaderColumns_NotByPosition()
    {
        string reordered =
            "| F-# | Finding | State | Severity | Resolution / owner |\n|---|---|---|---|---|\n" +
            "| F-1 | a finding | OPEN | S2 | r |\n";

        LedgerRowForm.ValidateTable(reordered.Split('\n'), Config).Should().BeEmpty();
    }

    // --- the I/O boundary and the wiring ---

    [Fact]
    public void RowForm_Inert_WhenTheConfigIsAbsent()
    {
        using var root = new TempRoot();
        root.Write("docs/ROADMAP.md", Table(RowNoClosingPipeA));

        LedgerRowForm.Validate(root.Path).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Inert_WhenTheLedgerFileIsAbsent()
    {
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml);

        LedgerRowForm.Validate(root.Path).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Red_ReadsTheFileTheConfigNames()
    {
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml.Replace("docs/ROADMAP.md", "docs/LEDGER.md"));
        root.Write("docs/ROADMAP.md", Table(RowPlain));
        root.Write("docs/LEDGER.md", Table(RowNoClosingPipeA));

        LedgerRowForm.Validate(root.Path).Should().ContainSingle(f => f.Subject.StartsWith("docs/LEDGER.md:"));
    }

    [Fact]
    public void RowForm_Red_UnreadableConfig_IsAReportNotACrash()
    {
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml", "schema_version: \"1.0\"\n");

        LedgerRowForm.Validate(root.Path).Should().ContainSingle(f =>
            f.Gate == GateCatalog.RowForm && f.Severity == Severity.Report && f.Message.Contains("unreadable"));
    }

    [Fact]
    public void RowForm_IsWiredIntoValidate_StructuralReddensTheArmedRun_VocabularyDoesNot()
    {
        using var broken = new TempRoot();
        WriteLedgerRoot(broken, Table(RowPlain, RowNoClosingPipeA));

        using var vocabularyOnly = new TempRoot();
        WriteLedgerRoot(vocabularyOnly, Table(RowPlain, RowSeverityS4, RowPlannedState));

        RegisterSync.Validate(broken.Path, armed: false, TextWriter.Null).Should().Be(0, "a report finding is exit-neutral unarmed");
        RegisterSync.Validate(broken.Path, armed: true, TextWriter.Null).Should().Be(1, "a structural finding reddens the armed run");
        RegisterSync.Validate(vocabularyOnly.Path, armed: true, TextWriter.Null).Should().Be(0, "vocabulary findings are monitors");
    }

    private static void WriteLedgerRoot(TempRoot root, string ledger)
    {
        root.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml);
        root.Write("tools/governance/SCOPE_EXCLUSIONS.yaml",
            "excluded_paths:\n  - pattern: \"docs/ROADMAP.md\"\n    rationale: \"ledger fixture without frontmatter\"\n");
        root.Write("docs/ROADMAP.md", ledger);
    }

    [Fact]
    public void LedgerFormConfig_RealFile_SetsMatchTheLedgerLegend()
    {
        string root = RepoPaths.RepoRoot();
        LedgerFormConfig config = LedgerFormConfig.Load(RepoPaths.LedgerFormPath(root))!;
        config.Should().NotBeNull("the repository carries the ledger-form config");

        string[] lines = File.ReadAllText(Path.Combine(root, config.LedgerPath)).Replace("\r\n", "\n").Split('\n');
        int header = Array.FindIndex(lines, l => l.StartsWith("| " + config.IdHeader + " |", StringComparison.Ordinal));
        header.Should().BeGreaterThan(0, "the configured ledger carries its table header");

        string legend = string.Join(" ", lines.Skip(Math.Max(0, header - 12)).Take(Math.Min(12, header)));
        var legendTokens = new HashSet<string>(Regex.Split(legend, "[^A-Za-z0-9-]+"), StringComparer.Ordinal);
        foreach (string member in config.SeveritySet.Concat(config.StateSet))
        {
            legendTokens.Should().Contain(member, "the legend above the table names every configured member as a whole word");
        }
    }
}
