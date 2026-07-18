using System;
using System.Threading;
using DualFrontier.Application.Bus;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

// К10.2 Item 29 — top-level fixture event types (FQN must avoid `+`
// nested-type marker which existing regex/registry skip per К10.2 Phase 0 inventory).
[EventTier(BusTier.Fast)]
public sealed class K10_2_FastDamageEvent : IEvent { }

[EventTier(BusTier.Background)]
public sealed class K10_2_BackgroundClimateEvent : IEvent { }

[EventTier(BusTier.Background, CoalesceFunctionTypeName = "Test.Coalesce")]
public sealed class K10_2_BackgroundWithCoalesce : IEvent { }

public sealed class K10_2_DefaultTierEvent : IEvent { }

// K10.2 Item 29 — Subscriber contract enforcement tests.
public sealed class SubscriberContractTests
{
    private static ModManifest ManifestWithCaps(string id, params string[] required)
        => new ModManifest
        {
            Id = id,
            Version = "1.0.0",
            Capabilities = ManifestCapabilities.Parse(required, null),
        };

    [Fact]
    public void TryParseTierToken_LegacyToken_ReturnsFalse()
    {
        bool ok = SubscriberContractValidator.TryParseTierToken(
            "kernel.publish:Foo.Bar", out _, out _);
        ok.Should().BeFalse("legacy tokens map к Normal tier per S-LOCK-4 backward compat");
    }

    [Theory]
    [InlineData("kernel.fast.publish:Foo.Bar",       BusTier.Fast)]
    [InlineData("kernel.fast.subscribe:Foo.Bar",     BusTier.Fast)]
    [InlineData("kernel.normal.publish:Foo.Bar",     BusTier.Normal)]
    [InlineData("kernel.background.subscribe:X.Y",   BusTier.Background)]
    public void TryParseTierToken_TierPrefixed_ReturnsTier(string token, BusTier expected)
    {
        bool ok = SubscriberContractValidator.TryParseTierToken(token, out BusTier tier, out string fqn);
        ok.Should().BeTrue();
        tier.Should().Be(expected);
        fqn.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_BackgroundEventMissingCoalesce_Emits_BackgroundCoalesceMissing()
    {
        var validator = new SubscriberContractValidator();
        var manifest = ManifestWithCaps("test.mod",
            $"kernel.background.subscribe:{typeof(K10_2_BackgroundClimateEvent).FullName}");

        var errors = validator.Validate("test.mod", typeof(K10_2_BackgroundClimateEvent), manifest);

        errors.Should().HaveCount(1);
        errors[0].Kind.Should().Be(ValidationErrorKind.BackgroundCoalesceMissing);
    }

    [Fact]
    public void Validate_BackgroundEventWithCoalesce_NoError()
    {
        var validator = new SubscriberContractValidator();
        var manifest = ManifestWithCaps("test.mod",
            $"kernel.background.subscribe:{typeof(K10_2_BackgroundWithCoalesce).FullName}");

        var errors = validator.Validate("test.mod", typeof(K10_2_BackgroundWithCoalesce), manifest);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_TierMismatch_Emits_BusTierMismatch()
    {
        // Event is annotated Fast, but manifest declares the Normal tier token
        var validator = new SubscriberContractValidator();
        var manifest = ManifestWithCaps("test.mod",
            $"kernel.normal.subscribe:{typeof(K10_2_FastDamageEvent).FullName}");

        var errors = validator.Validate("test.mod", typeof(K10_2_FastDamageEvent), manifest);

        errors.Should().HaveCount(1);
        errors[0].Kind.Should().Be(ValidationErrorKind.BusTierMismatch);
    }

    [Fact]
    public void Validate_NoTierTokenDeclared_NoError_DefaultsToNormal()
    {
        // S-LOCK-4 backward compat: legacy kernel.subscribe token works для Normal-tier events
        var validator = new SubscriberContractValidator();
        var manifest = ManifestWithCaps("test.mod",
            $"kernel.subscribe:{typeof(K10_2_DefaultTierEvent).FullName}");

        var errors = validator.Validate("test.mod", typeof(K10_2_DefaultTierEvent), manifest);

        errors.Should().BeEmpty();
    }
}

public sealed class FastTierContractMonitorTests
{
    [Fact]
    public void MeasureInvocation_UnderBudget_NoViolation()
    {
        var monitor = new FastTierContractMonitor
        {
            LatencyBudgetMicros = 1_000_000  // 1 second budget to avoid flake
        };
        monitor.MeasureInvocation("mod.a", "Test.Event", () => { });
        monitor.GetViolationCount("mod.a", "Test.Event").Should().Be(0);
    }

    [Fact]
    public void MeasureInvocation_OverBudget_IncrementsViolation()
    {
        var monitor = new FastTierContractMonitor
        {
            LatencyBudgetMicros = 0  // any work exceeds 0µs budget
        };
        monitor.MeasureInvocation("mod.a", "Test.Event", () => Thread.Sleep(1));
        monitor.GetViolationCount("mod.a", "Test.Event").Should().BeGreaterThan(0);
    }

    [Fact]
    public void MeasureInvocation_BeyondThreshold_RaisesEvent()
    {
        var monitor = new FastTierContractMonitor
        {
            LatencyBudgetMicros = 0,
            ViolationThreshold = 2,
        };
        int eventFireCount = 0;
        monitor.ViolationsExceededThreshold += (_, _, _) => Interlocked.Increment(ref eventFireCount);

        monitor.MeasureInvocation("mod.a", "Test.Event", () => Thread.Sleep(1));
        eventFireCount.Should().Be(0, "first violation below threshold");
        monitor.MeasureInvocation("mod.a", "Test.Event", () => Thread.Sleep(1));
        eventFireCount.Should().BeGreaterThan(0, "second violation reaches threshold");
    }

    [Fact]
    public void ResetViolationCounts_Clears()
    {
        var monitor = new FastTierContractMonitor { LatencyBudgetMicros = 0 };
        monitor.MeasureInvocation("mod.a", "Test.Event", () => Thread.Sleep(1));
        monitor.GetViolationCount("mod.a", "Test.Event").Should().BeGreaterThan(0);

        monitor.ResetViolationCounts();
        monitor.GetViolationCount("mod.a", "Test.Event").Should().Be(0);
    }
}
