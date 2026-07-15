using DualFrontier.Governance;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Governance.Tests;

/// <summary>G-GLOBALS (the four collection enums) and the G-RATIO monitor -- red and green.</summary>
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

    [Fact]
    public void Ratio_IsMonitorSeverity_AndComputesTheOverrideShare()
    {
        var docs = new[]
        {
            Fixture.Doc(("special_case_rationale", "sanctioned")),
            Fixture.Doc(("register_id", "DOC-A-2"), ("category", "A")),
        };

        Finding finding = Validators.RationaleRatio(docs);

        finding.Severity.Should().Be(Severity.Monitor);
        finding.Gate.Should().Be("G-RATIO");
        finding.Message.Should().Contain("1/2");
    }
}
