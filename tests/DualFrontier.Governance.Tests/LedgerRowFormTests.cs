using DualFrontier.Governance;
using System.Text.RegularExpressions;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>
/// G-ROWFORM over fixtures that copy the real ledger row shapes: a missing closing
/// pipe, a stray unescaped pipe, an escaped pipe, an out-of-order id, glyph and
/// bold state cells, and out-of-legend severity and state values; and over the
/// two-file ledger (live rows, closed rows): ids unique across both files and
/// ascending within each, the vocabulary severity set per file, the structure armed
/// in both.
/// </summary>
public sealed class LedgerRowFormTests
{
    private const string LivePath = "docs/LEDGER.md";
    private const string ArchivePath = "docs/LEDGER_ARCHIVE.md";

    private const string Sets =
        "id_header: \"F-#\"\n" +
        "severity_header: \"Severity\"\n" +
        "state_header: \"State\"\n" +
        "severity_set: [\"S0\", \"S1\", \"S2\", \"S3\", \"S4\", \"N\"]\n" +
        "state_set: [\"OPEN\", \"CLOSED\", \"ACCEPTED-NO-ACTION\"]\n";

    /// <summary>The two-file config: the live file held to the legend, the archive watched.</summary>
    internal const string ConfigYaml =
        "schema_version: \"2.0\"\n" +
        "ledgers:\n" +
        "  - path: \"docs/LEDGER.md\"\n" +
        "    vocabulary: report\n" +
        "  - path: \"docs/LEDGER_ARCHIVE.md\"\n" +
        "    vocabulary: monitor\n" +
        Sets;

    private static readonly LedgerFileConfig Live = new(LivePath, Severity.Report);
    private static readonly LedgerFileConfig Archive = new(ArchivePath, Severity.Monitor);

    private static readonly LedgerFormConfig Config = new(
        new[] { Live, Archive }, "F-#", "Severity", "State",
        new[] { "S0", "S1", "S2", "S3", "S4", "N" },
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
        "> Severity: S0 critical / S1 high / S2 medium / S3 low / S4 latent / N note. State:\n" +
        "> OPEN / CLOSED / ACCEPTED-NO-ACTION.\n\n" +
        "| F-# | Finding | Severity | State | Resolution / owner |\n" +
        "|---|---|---|---|---|\n" +
        string.Join("\n", rows) +
        "\n\nA paragraph after the table | with a pipe | is not a row.\n";

    private static string RowAt(int number) => $"| F-{number} | a finding | S2 | OPEN | r |";

    /// <summary>The rows as the live file (vocabulary findings are Reports).</summary>
    private static IReadOnlyList<Finding> Check(params string[] rows) =>
        LedgerRowForm.ValidateTable(Table(rows).Split('\n'), Config, Live);

    /// <summary>The rows as the archive file (vocabulary findings are Monitors).</summary>
    private static IReadOnlyList<Finding> CheckArchive(params string[] rows) =>
        LedgerRowForm.ValidateTable(Table(rows).Split('\n'), Config, Archive);

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
        root.Write(LivePath, crlf);

        LedgerRowForm.Validate(root.Path).Should().BeEmpty();
    }

    // --- structural: red (Report) ---

    [Fact]
    public void RowForm_Red_MissingSeparator_ReadsTheFirstRowInsteadOfSkippingIt()
    {
        // Without its separator, the line after the header is the first data row; a duplicate
        // id there must still be caught, and the missing separator is itself a finding.
        string ledger = Table(RowAt(1), RowAt(1)).Replace("|---|---|---|---|---|\n", string.Empty);

        IReadOnlyList<Finding> findings = LedgerRowForm.ValidateTable(ledger.Split('\n'), Config, Live);

        Structural(findings).Select(f => f.Message).Should()
            .Contain(m => m.Contains("no separator row"))
            .And.Contain(m => m.Contains("F-1: duplicate ledger id"));
    }

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

        IReadOnlyList<Finding> findings = LedgerRowForm.ValidateTable(lines, Config, Live);

        // the header, then the separator, then the first row; the faulty row is the second data row
        Structural(findings).Should().ContainSingle().Which.Subject.Should().Be($"{LivePath}:{headerLine + 3}");
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
        var findings = LedgerRowForm.ValidateTable(new[] { "# a ledger with no table", "| a | b |" }, Config, Live);

        findings.Should().ContainSingle(f => f.Severity == Severity.Report && f.Message.Contains("cannot read the ledger"));
    }

    // --- vocabulary: the legend sets, per-file severity ---

    [Fact]
    public void RowForm_Vocabulary_SeverityOutsideTheLegend_IsAMonitorInTheArchive()
    {
        IReadOnlyList<Finding> findings = CheckArchive(RowSeverityS4, RowSeverityW2);

        Structural(findings).Should().BeEmpty();
        Vocabulary(findings).Should().ContainSingle().Which.Message.Should().Contain("\"W2\"");
    }

    [Fact]
    public void RowForm_Vocabulary_SeverityS4_IsInTheLegend()
    {
        Check(RowSeverityS4).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Vocabulary_StateReadByItsLeadingMember()
    {
        // bold, dated, glyph-prefixed and plain states all lead with a legend member
        Check(RowBoldClosed, RowEscapedPipe, RowDatedClosed, RowGlyphClosed, RowGlyphlessClosed).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Vocabulary_StateOutsideTheLegend_IsAMonitorInTheArchive()
    {
        IReadOnlyList<Finding> findings = CheckArchive(RowPlannedState);

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

        LedgerRowForm.ValidateTable(reordered.Split('\n'), Config, Live).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Red_LiveVocabulary_IsAReport_ArchiveVocabulary_IsAMonitor_ForTheSameRows()
    {
        IReadOnlyList<Finding> live = Check(RowSeverityW2, RowPlannedState.Replace("F-65", "F-66"));
        IReadOnlyList<Finding> archive = CheckArchive(RowSeverityW2, RowPlannedState.Replace("F-65", "F-66"));

        live.Should().HaveCount(2).And.OnlyContain(f => f.Severity == Severity.Report);
        archive.Should().HaveCount(2).And.OnlyContain(f => f.Severity == Severity.Monitor);
    }

    // --- the two files: ids across them, structure in both ---

    [Fact]
    public void RowForm_Red_IdHeldByBothFiles_IsReportedOnTheSecondFile_NamingTheFirst()
    {
        using var root = new TempRoot();
        WriteTwoFileRoot(root, Table(RowAt(1), RowAt(5)), Table(RowAt(2), RowAt(5)));

        IReadOnlyList<Finding> findings = LedgerRowForm.Validate(root.Path);

        findings.Should().ContainSingle().Which.Should().Match<Finding>(f =>
            f.Severity == Severity.Report &&
            f.Subject.StartsWith(ArchivePath + ":", StringComparison.Ordinal) &&
            f.Message == "F-5: duplicate ledger id (also in docs/LEDGER.md)");
    }

    [Fact]
    public void RowForm_Red_IdRepeatedInsideOneFile_NamesNoOtherFile()
    {
        IReadOnlyList<Finding> findings = Check(RowAt(1), RowAt(1));

        findings.Should().ContainSingle().Which.Message.Should().Be("F-1: duplicate ledger id");
    }

    [Fact]
    public void RowForm_Green_IdsAscendInEachFile_NotAcrossThem()
    {
        // the archive may open below the live file's last id and above its first: the order is per file
        using var root = new TempRoot();
        WriteTwoFileRoot(root, Table(RowAt(2), RowAt(7)), Table(RowAt(1), RowAt(3), RowAt(4), RowAt(9)));

        LedgerRowForm.Validate(root.Path).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Red_ArchiveIdsOutOfOrder_AreReportedAtTheArchiveLine()
    {
        using var root = new TempRoot();
        string archive = Table(RowAt(3), RowAt(1));
        WriteTwoFileRoot(root, Table(RowAt(2)), archive);
        int line = Array.FindIndex(archive.Split('\n'), l => l.StartsWith("| F-1 |", StringComparison.Ordinal)) + 1;

        IReadOnlyList<Finding> findings = LedgerRowForm.Validate(root.Path);

        findings.Should().ContainSingle().Which.Should().Match<Finding>(f =>
            f.Severity == Severity.Report &&
            f.Subject == $"{ArchivePath}:{line}" &&
            f.Message.StartsWith("F-1: ids are not strictly increasing", StringComparison.Ordinal));
    }

    [Fact]
    public void RowForm_Red_ArchiveStructure_IsArmed_LikeTheLiveFile()
    {
        using var root = new TempRoot();
        WriteTwoFileRoot(root, Table(RowPlain), Table(RowNoClosingPipeA));

        IReadOnlyList<Finding> findings = LedgerRowForm.Validate(root.Path);

        findings.Should().ContainSingle().Which.Should().Match<Finding>(f =>
            f.Severity == Severity.Report &&
            f.Subject.StartsWith(ArchivePath + ":", StringComparison.Ordinal) &&
            f.Message.Contains("closing pipe"));
        RegisterSync.Validate(root.Path, armed: true, TextWriter.Null).Should().Be(1);
    }

    [Fact]
    public void RowForm_Red_VocabularySeverityIsPerFile_ThroughTheCommand()
    {
        using var liveOutside = new TempRoot();
        WriteTwoFileRoot(liveOutside, Table(RowSeverityW2), Table(RowPlain));

        using var archiveOutside = new TempRoot();
        WriteTwoFileRoot(archiveOutside, Table(RowPlain), Table(RowSeverityW2, RowPlannedState));

        LedgerRowForm.Validate(liveOutside.Path).Should().ContainSingle(f =>
            f.Severity == Severity.Report && f.Subject.StartsWith(LivePath + ":", StringComparison.Ordinal));
        RegisterSync.Validate(liveOutside.Path, armed: true, TextWriter.Null).Should().Be(1, "an out-of-legend value in the live file reddens the armed run");

        LedgerRowForm.Validate(archiveOutside.Path).Should().HaveCount(2).And.OnlyContain(f => f.Severity == Severity.Monitor);
        RegisterSync.Validate(archiveOutside.Path, armed: true, TextWriter.Null).Should().Be(0, "legacy values in the archive are watched, not armed");
    }

    // --- the detached row after a table (a row-shaped line the table no longer reads) ---

    [Fact]
    public void RowForm_Red_DetachedRow_IsLocatedAtItsOwnLine_InEachFile()
    {
        string detached = "| F-8 | a row after the table | S2 | OPEN | r |";
        string live = Table(RowAt(1), "", detached);
        string archive = Table(RowAt(2), "", "", detached.Replace("F-8", "F-9"));
        using var root = new TempRoot();
        WriteTwoFileRoot(root, live, archive);
        int liveLine = Array.FindIndex(live.Split('\n'), l => l.StartsWith("| F-8 |", StringComparison.Ordinal)) + 1;
        int archiveLine = Array.FindIndex(archive.Split('\n'), l => l.StartsWith("| F-9 |", StringComparison.Ordinal)) + 1;

        IReadOnlyList<Finding> findings = LedgerRowForm.Validate(root.Path);

        findings.Select(f => f.Subject).Should().BeEquivalentTo(new[] { $"{LivePath}:{liveLine}", $"{ArchivePath}:{archiveLine}" });
        findings.Should().OnlyContain(f => f.Severity == Severity.Report && f.Message.Contains("follows the end of the table"));
    }

    [Theory]
    [InlineData("F-12")]
    [InlineData("F-103")]
    public void RowForm_Red_DetachedRow_CapturesTheWholeMultiDigitId(string id)
    {
        IReadOnlyList<Finding> findings = Check(RowAt(1), "", $"| {id} | a row after the table | S2 | OPEN | r |");

        findings.Should().ContainSingle().Which.Message.Should().StartWith(id + ":");
    }

    // --- the I/O boundary, the config, and the wiring ---

    [Fact]
    public void RowForm_Inert_WhenTheConfigIsAbsent()
    {
        using var root = new TempRoot();
        root.Write(LivePath, Table(RowNoClosingPipeA));

        LedgerRowForm.Validate(root.Path).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Inert_WhenNoConfiguredLedgerFileIsPresent()
    {
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml);

        LedgerRowForm.Validate(root.Path).Should().BeEmpty();
    }

    [Fact]
    public void RowForm_Red_AConfiguredFileThatIsAbsent_IsSkipped_TheOtherIsStillRead()
    {
        using var onlyLive = new TempRoot();
        onlyLive.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml);
        onlyLive.Write(LivePath, Table(RowNoClosingPipeA));

        using var onlyArchive = new TempRoot();
        onlyArchive.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml);
        onlyArchive.Write(ArchivePath, Table(RowNoClosingPipeA));

        LedgerRowForm.Validate(onlyLive.Path).Should().ContainSingle(f => f.Subject.StartsWith(LivePath + ":", StringComparison.Ordinal));
        LedgerRowForm.Validate(onlyArchive.Path).Should().ContainSingle(f => f.Subject.StartsWith(ArchivePath + ":", StringComparison.Ordinal));
    }

    [Fact]
    public void RowForm_Red_ReadsTheFilesTheConfigNames()
    {
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml",
            ConfigYaml.Replace("docs/LEDGER.md", "docs/elsewhere/LIVE.md").Replace("docs/LEDGER_ARCHIVE.md", "docs/elsewhere/OLD.md"));
        root.Write(LivePath, Table(RowNoClosingPipeA));
        root.Write("docs/elsewhere/LIVE.md", Table(RowPlain));
        root.Write("docs/elsewhere/OLD.md", Table(RowNoClosingPipeB));

        LedgerRowForm.Validate(root.Path).Should().ContainSingle(f => f.Subject.StartsWith("docs/elsewhere/OLD.md:", StringComparison.Ordinal));
    }

    [Fact]
    public void LedgerFormConfig_Load_ReadsBothFilesInOrder_WithTheirVocabularySeverity()
    {
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml);

        LedgerFormConfig config = LedgerFormConfig.Load(RepoPaths.LedgerFormPath(root.Path))!;

        config.Ledgers.Should().Equal(new LedgerFileConfig(LivePath, Severity.Report), new LedgerFileConfig(ArchivePath, Severity.Monitor));
        config.SeveritySet.Should().Equal("S0", "S1", "S2", "S3", "S4", "N");
        config.StateSet.Should().Equal("OPEN", "CLOSED", "ACCEPTED-NO-ACTION");
    }

    [Fact]
    public void RowForm_Red_UnreadableConfig_IsAReportNotACrash()
    {
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml", "schema_version: \"2.0\"\n");

        LedgerRowForm.Validate(root.Path).Should().ContainSingle(f =>
            f.Gate == GateCatalog.RowForm && f.Severity == Severity.Report && f.Message.Contains("unreadable"));
    }

    [Theory]
    [InlineData("ledger_path: \"docs/ROADMAP.md\"\n", "a single ledger_path and no ledgers list")]
    [InlineData("ledgers: []\n", "an empty list")]
    [InlineData("ledgers:\n  - vocabulary: report\n", "an entry without its path")]
    [InlineData("ledgers:\n  - path: \"docs/LEDGER.md\"\n    vocabulary: report\n  - path: \"docs/LEDGER.md\"\n    vocabulary: monitor\n", "the same path listed twice")]
    [InlineData("ledgers:\n  - path: \"docs/LEDGER.md\"\n    vocabulary: armed\n", "an unknown vocabulary value")]
    [InlineData("ledgers:\n  - path: \"docs/LEDGER.md\"\n", "an entry without its vocabulary")]
    public void RowForm_Red_ConfigThatCannotName_TheLedgerFiles_IsUnreadable(string ledgersBlock, string because)
    {
        using var root = new TempRoot();
        root.Write("tools/governance/LEDGER_FORM.yaml", "schema_version: \"2.0\"\n" + ledgersBlock + Sets);
        root.Write(LivePath, Table(RowNoClosingPipeA));

        LedgerRowForm.Validate(root.Path).Should().ContainSingle(f =>
            f.Gate == GateCatalog.RowForm && f.Severity == Severity.Report && f.Message.Contains("unreadable"), because);
    }

    [Fact]
    public void RowForm_IsWiredIntoValidate_StructuralReddensTheArmedRun_VocabularyOfTheArchiveDoesNot()
    {
        using var broken = new TempRoot();
        WriteTwoFileRoot(broken, Table(RowPlain, RowNoClosingPipeA), Table(RowAt(2)));

        using var vocabularyOnly = new TempRoot();
        WriteTwoFileRoot(vocabularyOnly, Table(RowPlain, RowSeverityS4), Table(RowAt(2), RowSeverityW2, RowPlannedState));

        RegisterSync.Validate(broken.Path, armed: false, TextWriter.Null).Should().Be(0, "a report finding is exit-neutral unarmed");
        RegisterSync.Validate(broken.Path, armed: true, TextWriter.Null).Should().Be(1, "a structural finding reddens the armed run");
        RegisterSync.Validate(vocabularyOnly.Path, armed: true, TextWriter.Null).Should().Be(0, "vocabulary findings of the archive are monitors");
    }

    private static void WriteTwoFileRoot(TempRoot root, string live, string archive)
    {
        root.Write("tools/governance/LEDGER_FORM.yaml", ConfigYaml);
        root.Write("tools/governance/SCOPE_EXCLUSIONS.yaml",
            "excluded_paths:\n" +
            "  - pattern: \"docs/LEDGER.md\"\n    rationale: \"ledger fixture without frontmatter\"\n" +
            "  - pattern: \"docs/LEDGER_ARCHIVE.md\"\n    rationale: \"ledger fixture without frontmatter\"\n");
        root.Write(LivePath, live);
        root.Write(ArchivePath, archive);
    }

    // --- the real ledger: read from the repository, both files ---

    private static readonly Regex StateLead = new(@"^[^A-Za-z0-9]*([A-Za-z][A-Za-z0-9-]*)", RegexOptions.CultureInvariant);

    /// <summary>The table rows of a real ledger file as (id, state-cell leading member), read independently of the gate.</summary>
    private static List<(string Id, string Lead)> RealRows(string root, string relPath)
    {
        string[] lines = File.ReadAllText(Path.Combine(root, relPath)).Replace("\r\n", "\n").Split('\n');
        var rows = new List<(string, string)>();
        foreach (string line in lines)
        {
            Match m = Regex.Match(line, @"^\|\s*(F-\d+)\s*\|");
            if (!m.Success)
            {
                continue;
            }

            string[] cells = Regex.Split(line.TrimEnd().TrimStart('|'), @"(?<!\\)\|");
            rows.Add((m.Groups[1].Value, StateLead.Match(cells[3].Trim()).Groups[1].Value));
        }

        return rows;
    }

    [Fact]
    public void RowForm_Green_RealLedgers_BothFilesAreRead_NoReportFinding_LiveVocabularyArmed()
    {
        string root = RepoPaths.RepoRoot();
        LedgerFormConfig config = LedgerFormConfig.Load(RepoPaths.LedgerFormPath(root))!;
        config.Should().NotBeNull("the gate is armed on this repository");

        config.Ledgers.Should().Equal(
            new LedgerFileConfig("docs/LEDGER.md", Severity.Report),
            new LedgerFileConfig("docs/LEDGER_ARCHIVE.md", Severity.Monitor));
        foreach (LedgerFileConfig ledger in config.Ledgers)
        {
            File.Exists(Path.Combine(root, ledger.Path)).Should().BeTrue($"{ledger.Path} is a configured ledger file");
        }

        IReadOnlyList<Finding> findings = LedgerRowForm.Validate(root);

        findings.Where(f => f.Severity == Severity.Report).Should().BeEmpty("structure is armed in both files and the live vocabulary is armed");
        findings.Where(f => f.Subject.StartsWith("docs/LEDGER.md:", StringComparison.Ordinal)).Should().BeEmpty("the live file keeps the legend");
    }

    [Fact]
    public void RowForm_Green_RealLedgers_EveryIdLivesInExactlyOneFile_ClosedRowsInTheArchive()
    {
        string root = RepoPaths.RepoRoot();
        List<(string Id, string Lead)> live = RealRows(root, "docs/LEDGER.md");
        List<(string Id, string Lead)> archive = RealRows(root, "docs/LEDGER_ARCHIVE.md");

        live.Should().NotBeEmpty();
        archive.Should().NotBeEmpty();
        live.Select(r => r.Id).Concat(archive.Select(r => r.Id)).Should().OnlyHaveUniqueItems("an id is held by one file");
        live.Should().OnlyContain(r => r.Lead != "CLOSED", "the live file holds the rows whose state does not begin CLOSED");
        archive.Should().OnlyContain(r => r.Lead == "CLOSED", "the archive holds the CLOSED rows");
    }

    [Fact]
    public void LedgerFormConfig_RealFile_SetsMatchTheLedgerLegend()
    {
        string root = RepoPaths.RepoRoot();
        LedgerFormConfig config = LedgerFormConfig.Load(RepoPaths.LedgerFormPath(root))!;
        config.Should().NotBeNull("the repository carries the ledger-form config");

        // the legend is declared once, above the table of the file that carries it
        var legendTokens = new HashSet<string>(StringComparer.Ordinal);
        foreach (LedgerFileConfig ledger in config.Ledgers)
        {
            string[] lines = File.ReadAllText(Path.Combine(root, ledger.Path)).Replace("\r\n", "\n").Split('\n');
            int header = Array.FindIndex(lines, l => l.StartsWith("| " + config.IdHeader + " |", StringComparison.Ordinal));
            header.Should().BeGreaterThan(0, $"{ledger.Path} carries its table header");

            int bodyStart = lines[0] == "---" ? Array.FindIndex(lines, 1, l => l == "---") + 1 : 0;
            legendTokens.UnionWith(Regex.Split(string.Join(" ", lines.Skip(bodyStart).Take(header - bodyStart)), "[^A-Za-z0-9-]+"));
        }

        foreach (string member in config.SeveritySet.Concat(config.StateSet))
        {
            legendTokens.Should().Contain(member, "the legend above a ledger table names every configured member as a whole word");
        }
    }
}
