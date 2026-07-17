using DualFrontier.Governance;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>The per-document semantic gates -- Report severity; each proven red and green.</summary>
public sealed class SemanticGateTests
{
    // --- G-CATLIFE (FRAMEWORK 3.4.1) ---

    [Fact]
    public void CatLife_Green_LegalCombination()
    {
        Validators.Gate1CategoryLifecycle(Fixture.Doc(("category", "D"), ("tier", 3), ("lifecycle", "EXECUTED")))
            .Should().BeEmpty();
    }

    [Fact]
    public void CatLife_Red_ForbiddenCategoryTier()
    {
        var findings = Validators.Gate1CategoryLifecycle(
            Fixture.Doc(("category", "D"), ("tier", 1), ("lifecycle", "EXECUTED"))).ToList();

        findings.Should().ContainSingle(f => f.Message.Contains("category=D + tier=1"));
        findings.Should().OnlyContain(f => f.Severity == Severity.Report);
    }

    [Fact]
    public void CatLife_Red_ForbiddenTierLifecycle()
    {
        Validators.Gate1CategoryLifecycle(Fixture.Doc(("category", "A"), ("tier", 3), ("lifecycle", "LOCKED")))
            .Should().ContainSingle(f => f.Message.Contains("tier=3 + lifecycle=LOCKED"));
    }

    [Fact]
    public void CatLife_Exempt_WhenSanctionedRationalePresent()
    {
        Validators.Gate1CategoryLifecycle(Fixture.Doc(
            ("category", "D"), ("tier", 1), ("lifecycle", "EXECUTED"),
            ("special_case_rationale", "sanctioned deviation, ratified"))).Should().BeEmpty();
    }

    // --- G-NAMESPACE ---

    [Fact]
    public void Namespace_Green_PrefixMatchesCategory()
    {
        Validators.Gate3Namespace(Fixture.Doc(("register_id", "DOC-A-THING"), ("category", "A"))).Should().BeEmpty();
    }

    [Fact]
    public void Namespace_Red_PrefixMismatchesCategory()
    {
        Validators.Gate3Namespace(Fixture.Doc(("register_id", "DOC-B-THING"), ("category", "A")))
            .Should().ContainSingle(f => f.Message.Contains("DOC-A-"));
    }

    // --- G-TERMINAL (per-doc null rule) ---

    [Fact]
    public void Terminal_Green_TerminalCarriesNull()
    {
        Validators.Gate4TerminalNull(Fixture.Doc(("lifecycle", "EXECUTED"), ("next_review_due", "null")))
            .Should().BeEmpty();
    }

    [Fact]
    public void Terminal_Red_TerminalCarriesDate()
    {
        Validators.Gate4TerminalNull(Fixture.Doc(("lifecycle", "EXECUTED"), ("next_review_due", "2027-01-01")))
            .Should().ContainSingle(f => f.Message.Contains("must carry next_review_due 'null'"));
    }

    [Fact]
    public void Terminal_Green_NonTerminalMayCarryDate()
    {
        Validators.Gate4TerminalNull(Fixture.Doc(("lifecycle", "Live"), ("next_review_due", "2027-01-01")))
            .Should().BeEmpty();
    }

    // --- G-SENTINEL (FRAMEWORK 14.4) ---

    [Theory]
    [InlineData("null")]
    [InlineData("2027-05-25")]
    [InlineData("2026-Q3")]
    [InlineData("post-Phase 0 closure")]
    public void Sentinel_Green_SanctionedForms(string form)
    {
        Validators.Gate5Sentinel(Fixture.Doc(("next_review_due", form))).Should().BeEmpty();
    }

    [Fact]
    public void Sentinel_Red_UnrecognizedForm()
    {
        Validators.Gate5Sentinel(Fixture.Doc(("next_review_due", "someday maybe")))
            .Should().ContainSingle(f => f.Message.Contains("not in the sanctioned set"));
    }
}
