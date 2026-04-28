using System;
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
    public void Parse_required_one_provided_null_counts_correct()
    {
        var caps = ManifestCapabilities.Parse(["kernel.publish:A.B.C"], null);

        caps.Required.Count.Should().Be(1);
        caps.Provided.Count.Should().Be(0);
    }

    [Fact]
    public void Parse_required_null_provided_one_counts_correct()
    {
        var caps = ManifestCapabilities.Parse(null, ["mod.x.y.publish:E.F.G"]);

        caps.Required.Count.Should().Be(0);
        caps.Provided.Count.Should().Be(1);
    }

    [Fact]
    public void Parse_both_sides_one_each_counts_correct()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B"], ["mod.x.y.write:C.D"]);

        caps.Required.Count.Should().Be(1);
        caps.Provided.Count.Should().Be(1);
    }

    [Fact]
    public void Parse_duplicate_required_token_deduplicates_to_one()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B", "kernel.read:A.B"], null);

        caps.Required.Count.Should().Be(1);
    }

    [Fact]
    public void Parse_token_with_surrounding_whitespace_trimmed_and_accepted()
    {
        Action act = () => ManifestCapabilities.Parse([" kernel.read:A.B "], null);

        act.Should().NotThrow();
    }

    [Fact]
    public void Parse_both_null_is_empty()
    {
        var caps = ManifestCapabilities.Parse(null, null);

        caps.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Parse_both_empty_enumerables_is_empty()
    {
        var caps = ManifestCapabilities.Parse([], []);

        caps.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Empty_IsEmpty_is_true()
    {
        ManifestCapabilities.Empty.IsEmpty.Should().BeTrue();
    }

    // --- Parse (invalid) -----------------------------------------------------

    [Fact]
    public void Parse_wrong_format_entirely_throws_with_token_in_message()
    {
        Action act = () => ManifestCapabilities.Parse(["ecs.systems"], null);

        act.Should().Throw<ArgumentException>().WithMessage("*ecs.systems*");
    }

    [Fact]
    public void Parse_missing_colon_and_fqn_throws_with_token_in_message()
    {
        Action act = () => ManifestCapabilities.Parse(["kernel.publish"], null);

        act.Should().Throw<ArgumentException>().WithMessage("*kernel.publish*");
    }

    [Fact]
    public void Parse_empty_fqn_after_colon_throws_with_token_in_message()
    {
        Action act = () => ManifestCapabilities.Parse(["kernel.read:"], null);

        act.Should().Throw<ArgumentException>().WithMessage("*kernel.read:*");
    }

    [Fact]
    public void Parse_uppercase_provider_throws_with_token_in_message()
    {
        Action act = () => ManifestCapabilities.Parse(["Kernel.publish:A.B"], null);

        act.Should().Throw<ArgumentException>().WithMessage("*Kernel.publish:A.B*");
    }

    [Fact]
    public void Parse_unknown_verb_throws_with_token_in_message()
    {
        Action act = () => ManifestCapabilities.Parse(["kernel.delete:A.B"], null);

        act.Should().Throw<ArgumentException>().WithMessage("*kernel.delete:A.B*");
    }

    [Fact]
    public void Parse_mod_without_modId_segment_throws_with_token_in_message()
    {
        Action act = () => ManifestCapabilities.Parse(["mod.publish:A.B"], null);

        act.Should().Throw<ArgumentException>().WithMessage("*mod.publish:A.B*");
    }

    [Fact]
    public void Parse_fqn_starting_with_digit_throws_with_token_in_message()
    {
        Action act = () => ManifestCapabilities.Parse(["kernel.publish:1Bad.Type"], null);

        act.Should().Throw<ArgumentException>().WithMessage("*kernel.publish:1Bad.Type*");
    }

    [Fact]
    public void Parse_empty_string_throws_ArgumentException()
    {
        Action act = () => ManifestCapabilities.Parse([""], null);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Parse_invalid_token_in_provided_side_throws_ArgumentException()
    {
        Action act = () => ManifestCapabilities.Parse(null, ["bad"]);

        act.Should().Throw<ArgumentException>().WithMessage("*bad*");
    }

    // --- RequiresCapability / ProvidesCapability -----------------------------

    [Fact]
    public void RequiresCapability_present_token_returns_true()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B"], ["mod.x.y.publish:C.D"]);

        caps.RequiresCapability("kernel.read:A.B").Should().BeTrue();
    }

    [Fact]
    public void RequiresCapability_absent_token_returns_false()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B"], ["mod.x.y.publish:C.D"]);

        caps.RequiresCapability("kernel.write:A.B").Should().BeFalse();
    }

    [Fact]
    public void ProvidesCapability_present_token_returns_true()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B"], ["mod.x.y.publish:C.D"]);

        caps.ProvidesCapability("mod.x.y.publish:C.D").Should().BeTrue();
    }

    [Fact]
    public void ProvidesCapability_token_from_required_returns_false()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B"], ["mod.x.y.publish:C.D"]);

        caps.ProvidesCapability("kernel.read:A.B").Should().BeFalse();
    }

    [Fact]
    public void RequiresCapability_null_returns_false()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B"], null);

        caps.RequiresCapability(null!).Should().BeFalse();
    }

    [Fact]
    public void RequiresCapability_empty_string_returns_false()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B"], null);

        caps.RequiresCapability("").Should().BeFalse();
    }

    // --- ToString ------------------------------------------------------------

    [Fact]
    public void ToString_empty_returns_none()
    {
        ManifestCapabilities.Parse(null, null).ToString().Should().Be("(none)");
    }

    [Fact]
    public void ToString_required_only_omits_provided_label()
    {
        var caps = ManifestCapabilities.Parse(["kernel.publish:A.B"], null);

        caps.ToString().Should().Be("required: kernel.publish:A.B");
    }

    [Fact]
    public void ToString_provided_only_omits_required_label()
    {
        var caps = ManifestCapabilities.Parse(null, ["mod.x.y.publish:C.D"]);

        caps.ToString().Should().Be("provided: mod.x.y.publish:C.D");
    }

    [Fact]
    public void ToString_both_sides_separated_by_semicolon()
    {
        var caps = ManifestCapabilities.Parse(["kernel.read:A.B"], ["mod.x.y.write:C.D"]);

        caps.ToString().Should().Be("required: kernel.read:A.B; provided: mod.x.y.write:C.D");
    }

    [Fact]
    public void ToString_multiple_required_tokens_are_sorted()
    {
        var caps = ManifestCapabilities.Parse(
            ["kernel.write:B.B", "kernel.publish:A.A"], null);

        caps.ToString().Should().Be("required: kernel.publish:A.A, kernel.write:B.B");
    }

    // --- Equality ------------------------------------------------------------

    [Fact]
    public void Equality_different_insertion_order_required_are_equal()
    {
        var a = ManifestCapabilities.Parse(["kernel.read:A.B", "kernel.write:C.D"], null);
        var b = ManifestCapabilities.Parse(["kernel.write:C.D", "kernel.read:A.B"], null);

        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equality_different_tokens_are_not_equal()
    {
        var a = ManifestCapabilities.Parse(["kernel.read:A.B"], null);
        var b = ManifestCapabilities.Parse(["kernel.write:C.D"], null);

        (a == b).Should().BeFalse();
    }

    [Fact]
    public void Equality_two_empty_instances_are_equal()
    {
        var empty = ManifestCapabilities.Parse(null, null);

        (empty == ManifestCapabilities.Empty).Should().BeTrue();
    }
}
