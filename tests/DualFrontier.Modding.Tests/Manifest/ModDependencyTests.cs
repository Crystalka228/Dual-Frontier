using System;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Manifest;

/// <summary>
/// Full acceptance coverage for <see cref="ModDependency"/>.
/// </summary>
public sealed class ModDependencyTests
{
    // --- Construction: valid ------------------------------------------------

    [Fact]
    public void Constructor_valid_modId_constructs_without_exception()
    {
        var dep = new ModDependency("com.example.mod", null, false);

        dep.ModId.Should().Be("com.example.mod");
        dep.Version.Should().BeNull();
        dep.IsOptional.Should().BeFalse();
    }

    [Fact]
    public void Required_factory_sets_IsOptional_false_and_Version_null()
    {
        var dep = ModDependency.Required("com.example.mod");

        dep.IsOptional.Should().BeFalse();
        dep.Version.Should().BeNull();
        dep.ModId.Should().Be("com.example.mod");
    }

    [Fact]
    public void Optional_factory_with_version_sets_IsOptional_true()
    {
        var constraint = VersionConstraint.Parse("^1.0.0");
        var dep = ModDependency.Optional("com.example.mod", constraint);

        dep.IsOptional.Should().BeTrue();
        dep.Version.Should().Be(constraint);
    }

    // --- Construction: invalid ModId ----------------------------------------

    [Fact]
    public void Constructor_empty_modId_throws()
    {
        Action act = () => new ModDependency("", null, false);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_whitespace_modId_throws()
    {
        Action act = () => new ModDependency("  ", null, false);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_uppercase_modId_throws()
    {
        Action act = () => new ModDependency("BadId", null, false);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_single_segment_modId_throws()
    {
        Action act = () => new ModDependency("com", null, false);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_trailing_dot_modId_throws()
    {
        Action act = () => new ModDependency("com.example.", null, false);

        act.Should().Throw<ArgumentException>();
    }

    // --- IsSatisfiedBy -------------------------------------------------------

    [Fact]
    public void IsSatisfiedBy_noConstraint_anyVersion_returns_true()
    {
        var dep = ModDependency.Required("com.example.mod");

        dep.IsSatisfiedBy("com.example.mod", new ContractsVersion(5, 0, 0)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caretConstraint_satisfiedVersion_returns_true()
    {
        var dep = ModDependency.Required("com.example.mod", VersionConstraint.Parse("^1.0.0"));

        dep.IsSatisfiedBy("com.example.mod", new ContractsVersion(1, 2, 0)).Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_caretConstraint_majorMismatch_returns_false()
    {
        var dep = ModDependency.Required("com.example.mod", VersionConstraint.Parse("^1.0.0"));

        dep.IsSatisfiedBy("com.example.mod", new ContractsVersion(2, 0, 0)).Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_constraint_set_nullAvailableVersion_returns_false()
    {
        var dep = ModDependency.Required("com.example.mod", VersionConstraint.Parse("^1.0.0"));

        dep.IsSatisfiedBy("com.example.mod", null).Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_modId_mismatch_returns_false()
    {
        var dep = ModDependency.Required("com.example.mod");

        dep.IsSatisfiedBy("com.other.mod", null).Should().BeFalse();
    }

    // --- ToString ------------------------------------------------------------

    [Fact]
    public void ToString_required_no_version_returns_modId()
    {
        ModDependency.Required("com.example.mod").ToString()
            .Should().Be("com.example.mod");
    }

    [Fact]
    public void ToString_required_with_version_returns_modId_at_version()
    {
        ModDependency.Required("com.example.mod", VersionConstraint.Parse("^1.2.0")).ToString()
            .Should().Be("com.example.mod@^1.2.0");
    }

    [Fact]
    public void ToString_optional_no_version_returns_modId_optional()
    {
        ModDependency.Optional("com.example.mod").ToString()
            .Should().Be("com.example.mod (optional)");
    }

    [Fact]
    public void ToString_optional_with_version_returns_full_string()
    {
        ModDependency.Optional("com.example.mod", VersionConstraint.Parse("^1.2.0")).ToString()
            .Should().Be("com.example.mod@^1.2.0 (optional)");
    }

    // --- Record equality -----------------------------------------------------

    [Fact]
    public void Equality_identical_fields_are_equal()
    {
        var a = ModDependency.Required("com.example.mod", VersionConstraint.Parse("^1.0.0"));
        var b = ModDependency.Required("com.example.mod", VersionConstraint.Parse("^1.0.0"));

        (a == b).Should().BeTrue();
    }
}
