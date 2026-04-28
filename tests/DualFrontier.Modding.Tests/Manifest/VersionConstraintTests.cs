using System;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Manifest;

/// <summary>
/// Full acceptance coverage for <see cref="VersionConstraint"/> and <see cref="VersionConstraintKind"/>.
/// </summary>
public sealed class VersionConstraintTests
{
    // --- Parse: valid inputs ------------------------------------------------

    [Fact]
    public void Parse_exact_basic_returns_exact_kind()
    {
        VersionConstraint result = VersionConstraint.Parse("1.2.3");

        result.Kind.Should().Be(VersionConstraintKind.Exact);
        result.Floor.Should().Be(new ContractsVersion(1, 2, 3));
    }

    [Fact]
    public void Parse_caret_prefix_returns_caret_kind()
    {
        VersionConstraint result = VersionConstraint.Parse("^1.2.3");

        result.Kind.Should().Be(VersionConstraintKind.Caret);
        result.Floor.Should().Be(new ContractsVersion(1, 2, 3));
    }

    [Fact]
    public void Parse_with_surrounding_whitespace_trims_correctly()
    {
        VersionConstraint result = VersionConstraint.Parse("  ^2.0.0 ");

        result.Kind.Should().Be(VersionConstraintKind.Caret);
        result.Floor.Should().Be(new ContractsVersion(2, 0, 0));
    }

    [Fact]
    public void Parse_zero_version_exact_returns_exact_kind()
    {
        VersionConstraint result = VersionConstraint.Parse("0.0.1");

        result.Kind.Should().Be(VersionConstraintKind.Exact);
        result.Floor.Should().Be(new ContractsVersion(0, 0, 1));
    }

    [Fact]
    public void Parse_zero_major_caret_returns_caret_kind()
    {
        VersionConstraint result = VersionConstraint.Parse("^0.2.3");

        result.Kind.Should().Be(VersionConstraintKind.Caret);
        result.Floor.Should().Be(new ContractsVersion(0, 2, 3));
    }

    // --- Parse: invalid inputs (FormatException) ----------------------------

    [Fact]
    public void Parse_empty_string_throws_format_exception()
    {
        Action act = () => VersionConstraint.Parse("");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_incomplete_version_throws_format_exception()
    {
        Action act = () => VersionConstraint.Parse("1.2");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_non_numeric_throws_format_exception()
    {
        Action act = () => VersionConstraint.Parse("abc");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_four_component_version_throws_format_exception()
    {
        Action act = () => VersionConstraint.Parse("1.2.3.4");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_negative_component_throws_format_exception()
    {
        Action act = () => VersionConstraint.Parse("1.-1.0");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_caret_only_throws_format_exception()
    {
        Action act = () => VersionConstraint.Parse("^");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_caret_with_space_before_version_throws_format_exception()
    {
        Action act = () => VersionConstraint.Parse("^ 1.2.3");

        act.Should().Throw<FormatException>();
    }

    // --- Parse: null input (ArgumentNullException) --------------------------

    [Fact]
    public void Parse_null_throws_argument_null_exception()
    {
        Action act = () => VersionConstraint.Parse(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    // --- Parse: tilde rejection with caret hint (§8.4) ----------------------

    [Fact]
    public void Parse_tilde_with_version_throws_with_caret_hint()
    {
        Action act = () => VersionConstraint.Parse("~1.2.3");

        act.Should().Throw<FormatException>()
            .Where(ex => ex.Message.Contains("caret", StringComparison.OrdinalIgnoreCase))
            .Where(ex => ex.Message.Contains("~1.2.3"));
    }

    [Fact]
    public void Parse_bare_tilde_throws_with_caret_hint()
    {
        Action act = () => VersionConstraint.Parse("~");

        act.Should().Throw<FormatException>()
            .Where(ex => ex.Message.Contains("caret", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Parse_tilde_with_whitespace_throws_with_caret_hint()
    {
        Action act = () => VersionConstraint.Parse(" ~1.2.3 ");

        act.Should().Throw<FormatException>()
            .Where(ex => ex.Message.Contains("caret", StringComparison.OrdinalIgnoreCase));
    }

    // --- IsSatisfiedBy: Exact -----------------------------------------------

    [Fact]
    public void IsSatisfiedBy_exact_same_version_is_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 2, 3)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_exact_different_patch_is_not_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 2, 4)).Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_exact_different_minor_is_not_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 3, 0)).Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_exact_different_major_is_not_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(2, 2, 3)).Should().BeFalse();
    }

    // --- IsSatisfiedBy: Caret -----------------------------------------------

    [Fact]
    public void IsSatisfiedBy_caret_same_version_is_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 2, 3)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caret_higher_patch_is_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 2, 4)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caret_higher_minor_is_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 3, 0)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caret_higher_minor_and_patch_is_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 9, 9)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caret_different_major_is_not_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(2, 0, 0)).Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_caret_lower_patch_is_not_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 2, 2)).Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_caret_lower_minor_is_not_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^1.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 1, 9)).Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_caret_zero_major_same_version_is_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^0.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(0, 2, 3)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caret_zero_major_higher_patch_is_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^0.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(0, 2, 4)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caret_zero_major_higher_minor_is_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^0.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(0, 3, 0)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caret_zero_major_different_major_is_not_satisfied()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^0.2.3");

        constraint.IsSatisfiedBy(new ContractsVersion(1, 0, 0)).Should().BeFalse();
    }

    // --- Equality and ToString ----------------------------------------------

    [Fact]
    public void Equality_same_kind_and_floor_are_equal()
    {
        VersionConstraint a = VersionConstraint.Parse("^1.2.3");
        VersionConstraint b = VersionConstraint.Parse("^1.2.3");

        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equality_different_kind_are_not_equal()
    {
        VersionConstraint exact = VersionConstraint.Parse("1.2.3");
        VersionConstraint caret = VersionConstraint.Parse("^1.2.3");

        (exact == caret).Should().BeFalse();
    }

    [Fact]
    public void ToString_caret_constraint_includes_caret_prefix()
    {
        VersionConstraint constraint = VersionConstraint.Parse("^1.2.3");

        constraint.ToString().Should().Be("^1.2.3");
    }

    [Fact]
    public void ToString_exact_constraint_has_no_prefix()
    {
        VersionConstraint constraint = VersionConstraint.Parse("1.2.3");

        constraint.ToString().Should().Be("1.2.3");
    }
}
