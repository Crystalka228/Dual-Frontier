using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// Helper-level coverage for M5.1 dependency presence check
/// (<see cref="ModIntegrationPipeline.CheckDependencyPresence"/>):
/// required missing deps yield <see cref="ValidationErrorKind.MissingDependency"/>
/// errors, optional missing deps yield <see cref="ValidationWarning"/>s,
/// satisfied deps yield neither.
/// </summary>
public sealed class DependencyPresenceTests
{
    [Fact]
    public void Manifest_WithMissingRequiredDep_ProducesMissingDependencyError()
    {
        var modA = new ModManifest
        {
            Id = "tests.regular.a",
            Kind = ModKind.Regular,
            Dependencies = new[] { ModDependency.Required("tests.regular.b") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = modA,
        };

        (IReadOnlyList<ValidationError> errors, IReadOnlyList<ValidationWarning> warnings) =
            ModIntegrationPipeline.CheckDependencyPresence(manifestsById);

        warnings.Should().BeEmpty();
        ValidationError err = errors.Should().ContainSingle(e =>
            e.Kind == ValidationErrorKind.MissingDependency &&
            e.ModId == "tests.regular.a").Subject;
        err.Message.Should().Contain("tests.regular.b",
            "the diagnostic must name the missing dependency so authors can locate it");
    }

    [Fact]
    public void Manifest_WithMissingOptionalDep_ProducesWarning()
    {
        var modA = new ModManifest
        {
            Id = "tests.regular.a",
            Kind = ModKind.Regular,
            Dependencies = new[] { ModDependency.Optional("tests.regular.b") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = modA,
        };

        (IReadOnlyList<ValidationError> errors, IReadOnlyList<ValidationWarning> warnings) =
            ModIntegrationPipeline.CheckDependencyPresence(manifestsById);

        errors.Should().BeEmpty(
            "optional deps must NOT block loading when absent — the load can proceed " +
            "with degraded behavior, surfaced via warning");
        ValidationWarning w = warnings.Should().ContainSingle(x =>
            x.ModId == "tests.regular.a").Subject;
        w.Message.Should().Contain("tests.regular.b");
        w.Message.Should().Contain("Optional",
            "the diagnostic must distinguish optional from required at the message level");
    }

    [Fact]
    public void Manifest_WithBothMissingRequiredAndOptionalDeps_ReportsErrorAndWarning()
    {
        var modA = new ModManifest
        {
            Id = "tests.regular.a",
            Kind = ModKind.Regular,
            Dependencies = new[]
            {
                ModDependency.Required("tests.regular.requiredmissing"),
                ModDependency.Optional("tests.regular.optionalmissing"),
            },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = modA,
        };

        (IReadOnlyList<ValidationError> errors, IReadOnlyList<ValidationWarning> warnings) =
            ModIntegrationPipeline.CheckDependencyPresence(manifestsById);

        errors.Should().ContainSingle(e =>
            e.Kind == ValidationErrorKind.MissingDependency &&
            e.ModId == "tests.regular.a" &&
            e.Message.Contains("tests.regular.requiredmissing"));
        warnings.Should().ContainSingle(w =>
            w.ModId == "tests.regular.a" &&
            w.Message.Contains("tests.regular.optionalmissing"));
    }

    [Fact]
    public void Manifest_WithSatisfiedRequiredDep_NoErrorOrWarning()
    {
        // A requires B, B is in the batch. Presence-only check must report
        // nothing — version compatibility is M5.2's responsibility, not
        // CheckDependencyPresence.
        var modA = new ModManifest
        {
            Id = "tests.regular.a",
            Kind = ModKind.Regular,
            Dependencies = new[] { ModDependency.Required("tests.regular.b") },
        };
        var modB = new ModManifest
        {
            Id = "tests.regular.b",
            Kind = ModKind.Regular,
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = modA,
            ["tests.regular.b"] = modB,
        };

        (IReadOnlyList<ValidationError> errors, IReadOnlyList<ValidationWarning> warnings) =
            ModIntegrationPipeline.CheckDependencyPresence(manifestsById);

        errors.Should().BeEmpty();
        warnings.Should().BeEmpty();
    }
}
