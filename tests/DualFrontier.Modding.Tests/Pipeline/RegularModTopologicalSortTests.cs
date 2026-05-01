using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// Helper-level coverage for M5.1 regular-mod topological sort
/// (<see cref="ModIntegrationPipeline.TopoSortRegularMods"/>) and a
/// regression guard that <see cref="ModIntegrationPipeline.TopoSortSharedMods"/>
/// still detects shared-mod cycles after the M5.1 refactor extracted
/// <see cref="ModIntegrationPipeline.TopoSortByPredicate"/>.
/// </summary>
public sealed class RegularModTopologicalSortTests
{
    [Fact]
    public void RegularMods_Acyclic_LoadsInTopologicalOrder()
    {
        // A depends on B, B depends on C — a linear chain. Sorted output
        // must place dependencies before their dependents regardless of
        // input order, so [C, B, A].
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
            Dependencies = new[] { ModDependency.Required("tests.regular.c") },
        };
        var modC = new ModManifest
        {
            Id = "tests.regular.c",
            Kind = ModKind.Regular,
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = modA,
            ["tests.regular.b"] = modB,
            ["tests.regular.c"] = modC,
        };

        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortRegularMods(
                new[] { modA, modB, modC },  // input order intentionally NOT topological
                manifestsById);

        cycleErrors.Should().BeEmpty();
        sorted.Should().HaveCount(3);
        sorted[0].Id.Should().Be("tests.regular.c",
            "c has no dependencies and must be loaded first");
        sorted[1].Id.Should().Be("tests.regular.b",
            "b depends only on c and must be loaded second");
        sorted[2].Id.Should().Be("tests.regular.a",
            "a depends on b and must be loaded last");
    }

    [Fact]
    public void RegularModCycle_TwoMods_ProducesCyclicDependencyErrors()
    {
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
            Dependencies = new[] { ModDependency.Required("tests.regular.a") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = modA,
            ["tests.regular.b"] = modB,
        };

        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortRegularMods(new[] { modA, modB }, manifestsById);

        sorted.Should().BeEmpty(
            "every manifest is part of the cycle and none can be processed");
        cycleErrors.Should().HaveCount(2);
        cycleErrors.Should().OnlyContain(e => e.Kind == ValidationErrorKind.CyclicDependency);
        cycleErrors.Should().Contain(e =>
            e.ModId == "tests.regular.a" && e.Message.Contains("tests.regular.b"));
        cycleErrors.Should().Contain(e =>
            e.ModId == "tests.regular.b" && e.Message.Contains("tests.regular.a"));
        cycleErrors.Should().OnlyContain(e => e.Message.Contains("§8.7"),
            "the regular-mod cycle diagnostic must cite MOD_OS_ARCHITECTURE §8.7");
    }

    [Fact]
    public void RegularModCycle_ThreeMods_ProducesCyclicDependencyErrors()
    {
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
            Dependencies = new[] { ModDependency.Required("tests.regular.c") },
        };
        var modC = new ModManifest
        {
            Id = "tests.regular.c",
            Kind = ModKind.Regular,
            Dependencies = new[] { ModDependency.Required("tests.regular.a") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = modA,
            ["tests.regular.b"] = modB,
            ["tests.regular.c"] = modC,
        };

        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortRegularMods(new[] { modA, modB, modC }, manifestsById);

        sorted.Should().BeEmpty();
        cycleErrors.Should().HaveCount(3);
        cycleErrors.Should().OnlyContain(e => e.Kind == ValidationErrorKind.CyclicDependency);
        cycleErrors.Should().Contain(e => e.ModId == "tests.regular.a");
        cycleErrors.Should().Contain(e => e.ModId == "tests.regular.b");
        cycleErrors.Should().Contain(e => e.ModId == "tests.regular.c");
    }

    [Fact]
    public void RegularMod_DependsOnSharedMod_NotInRegularCycleGraph()
    {
        // Regular A depends on shared B. The predicate filters edges whose
        // target is not regular, so the shared dep does NOT participate in
        // regular-cycle detection and the sort completes successfully.
        var regularA = new ModManifest
        {
            Id = "tests.regular.a",
            Kind = ModKind.Regular,
            Dependencies = new[] { ModDependency.Required("tests.shared.b") },
        };
        var sharedB = new ModManifest
        {
            Id = "tests.shared.b",
            Kind = ModKind.Shared,
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = regularA,
            ["tests.shared.b"] = sharedB,
        };

        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortRegularMods(new[] { regularA }, manifestsById);

        cycleErrors.Should().BeEmpty(
            "regular cycle detection ignores edges to shared mods");
        sorted.Should().ContainSingle().Which.Id.Should().Be("tests.regular.a");
    }

    [Fact]
    public void RegularMod_DependsOnMissingMod_NotInRegularCycleGraph()
    {
        // Regular A depends on missing C. The TryGetValue lookup filters
        // out missing targets, so no edge is added and the sort completes
        // for A. (Presence-checking is the job of CheckDependencyPresence,
        // not the toposort.)
        var regularA = new ModManifest
        {
            Id = "tests.regular.a",
            Kind = ModKind.Regular,
            Dependencies = new[] { ModDependency.Required("tests.regular.missing") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.regular.a"] = regularA,
        };

        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortRegularMods(new[] { regularA }, manifestsById);

        cycleErrors.Should().BeEmpty(
            "missing dependencies are not toposort concerns — they surface in CheckDependencyPresence");
        sorted.Should().ContainSingle().Which.Id.Should().Be("tests.regular.a");
    }

    [Fact]
    public void SharedMods_DependCycleStill_DetectedSeparately()
    {
        // Regression guard: after the M5.1 refactor extracted
        // TopoSortByPredicate, TopoSortSharedMods must still detect cycles
        // in the shared-mod dependency graph and cite D-5 LOCKED.
        var sharedA = new ModManifest
        {
            Id = "tests.shared.a",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.b") },
        };
        var sharedB = new ModManifest
        {
            Id = "tests.shared.b",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.a") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.shared.a"] = sharedA,
            ["tests.shared.b"] = sharedB,
        };

        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortSharedMods(new[] { sharedA, sharedB }, manifestsById);

        sorted.Should().BeEmpty();
        cycleErrors.Should().HaveCount(2);
        cycleErrors.Should().OnlyContain(e => e.Kind == ValidationErrorKind.CyclicDependency);
        cycleErrors.Should().OnlyContain(e => e.Message.Contains("D-5 LOCKED"),
            "the shared-mod cycle diagnostic must still cite D-5 LOCKED after the refactor");
    }
}
