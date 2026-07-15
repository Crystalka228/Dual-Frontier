using DualFrontier.Governance;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>G-XREF (authorizing-record semantic), G-TERMINAL resolution, G-UNIQUE.</summary>
public sealed class CrossReferenceTests
{
    private static List<object> Ids(params string[] ids) => ids.Cast<object>().ToList();

    [Fact]
    public void Xref_Green_BidirectionalSupersedePair()
    {
        var docs = new[]
        {
            Fixture.Doc(("register_id", "DOC-A-NEW"), ("category", "A"), ("supersedes", Ids("DOC-A-OLD"))),
            Fixture.Doc(("register_id", "DOC-A-OLD"), ("category", "A"), ("lifecycle", "SUPERSEDED"),
                ("superseded_by", "DOC-A-NEW")),
        };

        Validators.CrossReferenceFindings(docs).Should().BeEmpty();
    }

    [Fact]
    public void Xref_Green_AcceptsAuthorizingCascadeBrief()
    {
        // FRAMEWORK 3.3.2: superseded_by names the authorizing record (an enrolled
        // cascade brief), which need not equal the superseding doc's id.
        var docs = new[]
        {
            Fixture.Doc(("register_id", "DOC-A-SUCCESSOR"), ("category", "A"), ("supersedes", Ids("DOC-G-OLD"))),
            Fixture.Doc(("register_id", "DOC-G-OLD"), ("category", "G"), ("lifecycle", "SUPERSEDED"),
                ("superseded_by", "DOC-D-AUTHORIZING")),
            Fixture.Doc(("register_id", "DOC-D-AUTHORIZING"), ("category", "D")),
        };

        Validators.CrossReferenceFindings(docs).Should().BeEmpty();
    }

    [Fact]
    public void Xref_Red_DanglingPointerAndOneWaySupersede()
    {
        var docs = new[]
        {
            Fixture.Doc(("register_id", "DOC-A-NEW"), ("category", "A"), ("supersedes", Ids("DOC-A-OLD"))),
            Fixture.Doc(("register_id", "DOC-A-OLD"), ("category", "A")), // one-way: no superseded_by
            Fixture.Doc(("register_id", "DOC-A-X"), ("category", "A"), ("superseded_by", "DOC-A-GHOST")), // dangling
        };

        var findings = Validators.CrossReferenceFindings(docs).ToList();

        findings.Should().Contain(f => f.Message.Contains("no resolving superseded_by"));
        findings.Should().Contain(f => f.Message.Contains("which is not enrolled"));
        findings.Should().OnlyContain(f => f.Severity == Severity.Report);
    }

    [Fact]
    public void Terminal_Red_SupersededMissingResolvingPointer()
    {
        var docs = new[]
        {
            Fixture.Doc(("register_id", "DOC-G-OLD"), ("category", "G"), ("lifecycle", "SUPERSEDED"),
                ("next_review_due", "null")),
        };

        Validators.CrossReferenceFindings(docs)
            .Should().Contain(f => f.Gate == "G-TERMINAL" && f.Message.Contains("SUPERSEDED requires superseded_by"));
    }

    [Fact]
    public void Unique_Red_DuplicateRegisterId_IsError()
    {
        var docs = new[]
        {
            Fixture.Doc(("register_id", "DOC-A-DUP"), ("category", "A")),
            Fixture.Doc(("register_id", "DOC-A-DUP"), ("category", "A")),
        };

        Validators.DuplicateIdFindings(docs)
            .Should().ContainSingle(f => f.Severity == Severity.Error && f.Message.Contains("duplicate"));
    }

    [Fact]
    public void Unique_Green_DistinctIds()
    {
        var docs = new[]
        {
            Fixture.Doc(("register_id", "DOC-A-1"), ("category", "A")),
            Fixture.Doc(("register_id", "DOC-A-2"), ("category", "A")),
        };

        Validators.DuplicateIdFindings(docs).Should().BeEmpty();
    }
}
