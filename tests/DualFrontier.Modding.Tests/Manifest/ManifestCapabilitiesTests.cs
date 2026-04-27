using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Manifest;

/// <summary>
/// Full acceptance coverage for <see cref="ManifestCapabilities"/>.
/// </summary>
public sealed class ManifestCapabilitiesTests
{
    // --- Parse (valid) -------------------------------------------------------

    [Fact]
    public void Parse_two_distinct_tokens_count_is_two()
    {
        var caps = ManifestCapabilities.Parse(["ecs.systems", "contracts.publish"]);

        caps.Tokens.Count.Should().Be(2);
    }

    [Fact]
    public void Parse_duplicate_token_deduplicates_to_one()
    {
        var caps = ManifestCapabilities.Parse(["ecs.systems", "ecs.systems"]);

        caps.Tokens.Count.Should().Be(1);
    }

    [Fact]
    public void Parse_token_with_surrounding_whitespace_trimmed_and_accepted()
    {
        var act = () => ManifestCapabilities.Parse([" ecs.systems "]);

        act.Should().NotThrow();
    }

    [Fact]
    public void Parse_empty_enumerable_is_empty()
    {
        var caps = ManifestCapabilities.Parse([]);

        caps.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Parse_null_enumerable_is_empty()
    {
        var caps = ManifestCapabilities.Parse(null);

        caps.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Empty_IsEmpty_is_true()
    {
        ManifestCapabilities.Empty.IsEmpty.Should().BeTrue();
    }

    // --- Parse (invalid) -----------------------------------------------------

    [Fact]
    public void Parse_single_segment_throws_ArgumentException()
    {
        Action act = () => ManifestCapabilities.Parse(["ecs"]);

        act.Should().Throw<ArgumentException>().WithMessage("*ecs*");
    }

    [Fact]
    public void Parse_uppercase_token_throws_ArgumentException()
    {
        Action act = () => ManifestCapabilities.Parse(["Ecs.Systems"]);

        act.Should().Throw<ArgumentException>().WithMessage("*Ecs.Systems*");
    }

    [Fact]
    public void Parse_trailing_dot_throws_ArgumentException()
    {
        Action act = () => ManifestCapabilities.Parse(["ecs."]);

        act.Should().Throw<ArgumentException>().WithMessage("*ecs.*");
    }

    [Fact]
    public void Parse_leading_dot_throws_ArgumentException()
    {
        Action act = () => ManifestCapabilities.Parse([".ecs.systems"]);

        act.Should().Throw<ArgumentException>().WithMessage("*.ecs.systems*");
    }

    [Fact]
    public void Parse_empty_segment_in_middle_throws_ArgumentException()
    {
        Action act = () => ManifestCapabilities.Parse(["ecs..systems"]);

        act.Should().Throw<ArgumentException>().WithMessage("*ecs..systems*");
    }

    [Fact]
    public void Parse_empty_string_throws_ArgumentException()
    {
        Action act = () => ManifestCapabilities.Parse([""]);

        act.Should().Throw<ArgumentException>();
    }

    // --- Contains ------------------------------------------------------------

    [Fact]
    public void Contains_present_token_returns_true()
    {
        var caps = ManifestCapabilities.Parse(["ecs.systems"]);

        caps.Contains("ecs.systems").Should().BeTrue();
    }

    [Fact]
    public void Contains_absent_token_returns_false()
    {
        var caps = ManifestCapabilities.Parse(["ecs.systems"]);

        caps.Contains("ecs.components").Should().BeFalse();
    }

    [Fact]
    public void Contains_null_returns_false()
    {
        var caps = ManifestCapabilities.Parse(["ecs.systems"]);

        caps.Contains(null!).Should().BeFalse();
    }

    [Fact]
    public void Contains_empty_string_returns_false()
    {
        var caps = ManifestCapabilities.Parse(["ecs.systems"]);

        caps.Contains("").Should().BeFalse();
    }

    // --- ToString ------------------------------------------------------------

    [Fact]
    public void ToString_empty_capabilities_returns_none()
    {
        ManifestCapabilities.Parse([]).ToString().Should().Be("(none)");
    }

    [Fact]
    public void ToString_multiple_tokens_returns_sorted_comma_joined()
    {
        var caps = ManifestCapabilities.Parse(["ecs.systems", "contracts.publish"]);

        caps.ToString().Should().Be("contracts.publish, ecs.systems");
    }

    // --- Equality ------------------------------------------------------------

    [Fact]
    public void Equality_different_insertion_order_are_equal()
    {
        var a = ManifestCapabilities.Parse(["b.b", "a.a"]);
        var b = ManifestCapabilities.Parse(["a.a", "b.b"]);

        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equality_different_tokens_are_not_equal()
    {
        var a = ManifestCapabilities.Parse(["a.a"]);
        var b = ManifestCapabilities.Parse(["b.b"]);

        (a == b).Should().BeFalse();
    }

    [Fact]
    public void Equality_empty_and_empty_instance_are_equal()
    {
        var empty = ManifestCapabilities.Parse([]);

        (empty == ManifestCapabilities.Empty).Should().BeTrue();
    }
}
