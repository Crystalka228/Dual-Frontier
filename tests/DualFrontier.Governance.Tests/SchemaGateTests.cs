using DualFrontier.Governance;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>G-SCHEMA -- always-Error. Every rule proven red and green.</summary>
public sealed class SchemaGateTests
{
    [Fact]
    public void Green_CompleteDoc_HasNoSchemaErrors()
    {
        Validators.ValidateSchema(Fixture.Doc()).Should().BeEmpty();
    }

    [Fact]
    public void Red_MissingRequiredField_IsErrorSeverity()
    {
        var findings = Validators.ValidateSchema(Fixture.Doc(("owner", null))).ToList();

        findings.Should().ContainSingle(f => f.Message.Contains("missing required field \"owner\""));
        findings.Should().OnlyContain(f => f.Gate == "G-SCHEMA" && f.Severity == Severity.Error);
    }

    [Fact]
    public void Red_InvalidEnumsAndId_FlagEachIndependently()
    {
        var findings = Validators.ValidateSchema(Fixture.Doc(
            ("category", "Z"),
            ("tier", 9),
            ("lifecycle", "nope"),
            ("content_language", "xx"),
            ("register_id", "BADID"))).ToList();

        findings.Should().HaveCount(5);
        findings.Should().OnlyContain(f => f.Gate == "G-SCHEMA" && f.Severity == Severity.Error);
    }

    [Fact]
    public void Red_PendingCommitPlaceholder_IsOutlawed()
    {
        Validators.ValidateSchema(Fixture.Doc(("last_modified_commit", "PENDING-COMMIT")))
            .Should().ContainSingle(f => f.Message.Contains("PENDING"));
    }

    [Fact]
    public void Green_RealCommitHash_IsAccepted()
    {
        Validators.ValidateSchema(Fixture.Doc(("last_modified_commit", "f0f76a8"))).Should().BeEmpty();
    }

    [Theory]
    [InlineData("DOC-F-SRC-AI-JOBS", "F")]        // path-flattened, hyphenated (FRAMEWORK 5)
    [InlineData("DOC-J-COMBAT-RESOLUTION", "J")]
    [InlineData("DOC-D-K_L3_1_BRIDGE", "D")]      // underscored
    public void Green_HyphenatedAndUnderscoredIds_AreValid(string id, string category)
    {
        Validators.ValidateSchema(Fixture.Doc(("register_id", id), ("category", category))).Should().BeEmpty();
    }
}
