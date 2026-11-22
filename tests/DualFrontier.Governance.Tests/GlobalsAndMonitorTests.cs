using DualFrontier.Governance;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>G-GLOBALS (the four collection enums) and the G-RATIO gate -- red and green.</summary>
public sealed class GlobalsAndMonitorTests
{
    [Fact]
    public void Globals_Green_ValidCollections()
    {
        var globals = new GlobalsCollections
        {
            Requirements = new() { new() { ["id"] = "REQ-1", ["verification_status"] = "VERIFIED" } },
            Risks = new() { new() { ["id"] = "RISK-1", ["likelihood"] = "Medium", ["impact"] = "High", ["risk_type"] = "Technical", ["status"] = "ACTIVE" } },
            Capa = new() { new() { ["id"] = "CAPA-1", ["closure_status"] = "CLOSED" } },
            AuditTrail = new() { new() { ["id"] = "EVT-1", ["event_type"] = "execution_milestone" } },
        };

        globals.Validate().Should().BeEmpty();
    }

    [Fact]
    public void Globals_Red_InvalidVerificationStatus()
    {
        var globals = new GlobalsCollections
        {
            Requirements = new() { new() { ["id"] = "REQ-1", ["verification_status"] = "MAYBE" } },
        };

        globals.Validate().Should().ContainSingle(f =>
            f.Gate == "G-GLOBALS" && f.Message.Contains("verification_status"));
    }

    [Fact]
    public void Globals_Red_InvalidRiskImpact()
    {
        var globals = new GlobalsCollections
        {
            Risks = new() { new() { ["id"] = "RISK-1", ["impact"] = "Catastrophic" } },
        };

        globals.Validate().Should().Contain(f => f.Message.Contains("impact"));
    }

    [Fact]
    public void Globals_Red_DuplicateId()
    {
        var globals = new GlobalsCollections
        {
            Capa = new()
            {
                new() { ["id"] = "CAPA-1", ["closure_status"] = "OPEN" },
                new() { ["id"] = "CAPA-1", ["closure_status"] = "CLOSED" },
            },
        };

        globals.Validate().Should().Contain(f => f.Message.Contains("duplicate capa id"));
    }

    private static IReadOnlyList<Frontmatter> RationaleCorpus(int total, int withRationale)
    {
        var docs = new List<Frontmatter>();
        for (int i = 0; i < total; i++)
        {
            docs.Add(i < withRationale
                ? Fixture.Doc(("register_id", $"DOC-D-N{i}"), ("special_case_rationale", "sanctioned"))
                : Fixture.Doc(("register_id", $"DOC-D-N{i}")));
        }

        return docs;
    }

    [Theory]
    [InlineData(0, 0, Severity.Monitor)]
    [InlineData(100, 0, Severity.Monitor)]
    [InlineData(100, 19, Severity.Monitor)]
    [InlineData(6, 1, Severity.Monitor)]
    [InlineData(100, 20, Severity.Report)]
    [InlineData(5, 1, Severity.Report)]
    [InlineData(100, 21, Severity.Report)]
    [InlineData(1, 1, Severity.Report)]
    public void Ratio_ReportsAtTheThreshold_AndMonitorsBelowIt(int total, int overrides, Severity expected)
    {
        Finding finding = Validators.RationaleRatio(RationaleCorpus(total, overrides));

        finding.Gate.Should().Be(GateCatalog.Ratio);
        finding.Severity.Should().Be(expected);
        finding.Message.Should().Contain($"{overrides}/{total}");
    }

    [Fact]
    public void Ratio_RunAll_CarriesTheThresholdVerdict()
    {
        Validators.RunAll(RationaleCorpus(5, 1))
            .Should().ContainSingle(f => f.Gate == GateCatalog.Ratio && f.Severity == Severity.Report);
        Validators.RunAll(RationaleCorpus(6, 1))
            .Should().ContainSingle(f => f.Gate == GateCatalog.Ratio && f.Severity == Severity.Monitor);
    }

    [Fact]
    public void Ratio_CountsOnlySpecialCaseRationale_NotProvenanceNote()
    {
        var docs = new[]
        {
            Fixture.Doc(("register_id", "DOC-D-A1"), ("provenance_note", "carried from an earlier register")),
            Fixture.Doc(("register_id", "DOC-D-A2"), ("provenance_note", "carried from an earlier register")),
        };

        Finding finding = Validators.RationaleRatio(docs);

        finding.Severity.Should().Be(Severity.Monitor);
        finding.Message.Should().Contain("0/2");
    }

    [Fact]
    public void ProvenanceNote_IsParsed_ButNeverExemptsAForbiddenCombination()
    {
        Frontmatter doc = Fixture.Doc(
            ("category", "D"), ("tier", 1), ("lifecycle", "EXECUTED"),
            ("provenance_note", "classification carried over"));

        doc.ProvenanceNote.Should().Be("classification carried over");
        doc.HasSanctionedDeviation.Should().BeFalse();
        Validators.Gate1CategoryLifecycle(doc).Should().ContainSingle(f => f.Gate == GateCatalog.CatLife);
    }

    [Fact]
    public void ProvenanceNote_IsAccepted_ByTheSchemaGate_AndCarriedIntoTheArchive()
    {
        Frontmatter doc = Fixture.Doc(
            ("register_id", "DOC-A-NOTED"), ("category", "A"),
            ("provenance_note", "classification carried over"));

        Validators.ValidateSchema(doc).Should().BeEmpty();
        Validators.RenderArchive(new[] { doc }, new GlobalsCollections(), "2.1")
            .Should().Contain("provenance_note: classification carried over");
    }
}
