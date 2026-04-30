using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Sharing;

/// <summary>
/// Coverage for ContractValidator Phase F (MOD_OS_ARCHITECTURE §5.2
/// shared-mod compliance — manifest fields and IMod-presence rules) and
/// ModIntegrationPipeline.TopoSortSharedMods (D-5 LOCKED shared-mod cycle
/// detection per §1.4).
/// </summary>
public sealed class SharedModComplianceTests
{
    [Fact]
    public void SharedMod_WithEntryAssemblyInManifest_ProducesSharedModWithEntryPointError()
    {
        LoadedSharedMod mod = MakeStubSharedMod(new ModManifest
        {
            Id = "tests.shared.x",
            Kind = ModKind.Shared,
            EntryAssembly = "WrongValue.dll",
        });
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(
            mods: Array.Empty<LoadedMod>(),
            coreSystems: Array.Empty<SystemBase>(),
            kernelCapabilities: null,
            sharedMods: new[] { mod });

        report.IsValid.Should().BeFalse();
        ValidationError err = report.Errors.Should()
            .ContainSingle(e =>
                e.Kind == ValidationErrorKind.SharedModWithEntryPoint &&
                e.ModId == "tests.shared.x" &&
                e.Message.Contains("entryAssembly"))
            .Subject;
        err.Message.Should().Contain("WrongValue.dll",
            "the diagnostic must name the offending value to help mod authors locate it");
    }

    [Fact]
    public void SharedMod_WithEntryTypeInManifest_ProducesSharedModWithEntryPointError()
    {
        LoadedSharedMod mod = MakeStubSharedMod(new ModManifest
        {
            Id = "tests.shared.y",
            Kind = ModKind.Shared,
            EntryType = "Some.Wrong.Type",
        });
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(
            mods: Array.Empty<LoadedMod>(),
            coreSystems: Array.Empty<SystemBase>(),
            kernelCapabilities: null,
            sharedMods: new[] { mod });

        report.IsValid.Should().BeFalse();
        ValidationError err = report.Errors.Should()
            .ContainSingle(e =>
                e.Kind == ValidationErrorKind.SharedModWithEntryPoint &&
                e.ModId == "tests.shared.y" &&
                e.Message.Contains("entryType"))
            .Subject;
        err.Message.Should().Contain("Some.Wrong.Type");
    }

    [Fact]
    public void SharedMod_WithReplacesInManifest_ProducesSharedModWithEntryPointError()
    {
        LoadedSharedMod mod = MakeStubSharedMod(new ModManifest
        {
            Id = "tests.shared.z",
            Kind = ModKind.Shared,
            Replaces = new[] { "Some.Kernel.System" },
        });
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(
            mods: Array.Empty<LoadedMod>(),
            coreSystems: Array.Empty<SystemBase>(),
            kernelCapabilities: null,
            sharedMods: new[] { mod });

        report.IsValid.Should().BeFalse();
        report.Errors.Should().ContainSingle(e =>
            e.Kind == ValidationErrorKind.SharedModWithEntryPoint &&
            e.ModId == "tests.shared.z" &&
            e.Message.Contains("replaces"));
    }

    [Fact]
    public void SharedMod_WithIModImplementation_ProducesSharedModWithEntryPointError()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();
        LoadedSharedMod loaded = loader.LoadSharedMod(
            TestModPaths.BadSharedModWithIMod, sharedAlc);
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(
            mods: Array.Empty<LoadedMod>(),
            coreSystems: Array.Empty<SystemBase>(),
            kernelCapabilities: null,
            sharedMods: new[] { loaded });

        report.IsValid.Should().BeFalse();
        ValidationError err = report.Errors.Should()
            .ContainSingle(e =>
                e.Kind == ValidationErrorKind.SharedModWithEntryPoint &&
                e.ModId == "tests.bad-shared-imod" &&
                e.Message.Contains("WronglyHere"))
            .Subject;
        err.Message.Should().Contain("IMod",
            "the diagnostic must explain that the offending type implements IMod");
        err.Message.Should().Contain("\"regular\"",
            "the diagnostic must hint at the remediation — move IMod to a regular mod");
    }

    [Fact]
    public void SharedModCycle_TwoMods_ProducesCyclicDependencyErrors()
    {
        var modA = new ModManifest
        {
            Id = "tests.shared.a",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.b") },
        };
        var modB = new ModManifest
        {
            Id = "tests.shared.b",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.a") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.shared.a"] = modA,
            ["tests.shared.b"] = modB,
        };

        (IReadOnlyList<ModManifest> sortedShared, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortSharedMods(new[] { modA, modB }, manifestsById);

        sortedShared.Should().BeEmpty(
            "every manifest is part of the cycle and none can be processed");
        cycleErrors.Should().HaveCount(2);
        cycleErrors.Should().OnlyContain(e => e.Kind == ValidationErrorKind.CyclicDependency);
        cycleErrors.Should().Contain(e =>
            e.ModId == "tests.shared.a" && e.Message.Contains("tests.shared.b"));
        cycleErrors.Should().Contain(e =>
            e.ModId == "tests.shared.b" && e.Message.Contains("tests.shared.a"));
    }

    [Fact]
    public void SharedModCycle_ThreeMods_ProducesCyclicDependencyErrors()
    {
        var modA = new ModManifest
        {
            Id = "tests.shared.a",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.b") },
        };
        var modB = new ModManifest
        {
            Id = "tests.shared.b",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.c") },
        };
        var modC = new ModManifest
        {
            Id = "tests.shared.c",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.a") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.shared.a"] = modA,
            ["tests.shared.b"] = modB,
            ["tests.shared.c"] = modC,
        };

        (IReadOnlyList<ModManifest> sortedShared, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortSharedMods(new[] { modA, modB, modC }, manifestsById);

        sortedShared.Should().BeEmpty();
        cycleErrors.Should().HaveCount(3);
        cycleErrors.Should().OnlyContain(e => e.Kind == ValidationErrorKind.CyclicDependency);
        cycleErrors.Should().Contain(e => e.ModId == "tests.shared.a");
        cycleErrors.Should().Contain(e => e.ModId == "tests.shared.b");
        cycleErrors.Should().Contain(e => e.ModId == "tests.shared.c");
    }

    [Fact]
    public void SharedMods_Acyclic_LoadsInTopologicalOrder()
    {
        var core = new ModManifest
        {
            Id = "tests.shared.core",
            Kind = ModKind.Shared,
        };
        var modA = new ModManifest
        {
            Id = "tests.shared.a",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.core") },
        };
        var modB = new ModManifest
        {
            Id = "tests.shared.b",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.shared.a") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.shared.core"] = core,
            ["tests.shared.a"] = modA,
            ["tests.shared.b"] = modB,
        };

        (IReadOnlyList<ModManifest> sortedShared, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortSharedMods(
                new[] { modB, modA, core },  // input order intentionally reversed
                manifestsById);

        cycleErrors.Should().BeEmpty();
        sortedShared.Should().HaveCount(3);
        sortedShared[0].Id.Should().Be("tests.shared.core",
            "core has no dependencies and must be loaded first");
        sortedShared[1].Id.Should().Be("tests.shared.a",
            "a depends only on core and must be loaded second");
        sortedShared[2].Id.Should().Be("tests.shared.b",
            "b depends on a and must be loaded last");
    }

    [Fact]
    public void SharedMod_WithRegularModDependency_NotConsideredForCycleDetection()
    {
        // Shared mod A declares a dependency on regular mod R; R declares a
        // dependency on A. D-5 LOCKED applies to the shared-mod dependency
        // graph specifically (MOD_OS_ARCHITECTURE §1.4) — a shared/regular
        // edge does not contribute to that graph, so the helper must accept
        // A without a cycle error. Regular-mod cycles surface separately in
        // M5's general dependency resolver.
        var sharedA = new ModManifest
        {
            Id = "tests.shared.a",
            Kind = ModKind.Shared,
            Dependencies = new[] { ModDependency.Required("tests.regular.r") },
        };
        var regularR = new ModManifest
        {
            Id = "tests.regular.r",
            Kind = ModKind.Regular,
            Dependencies = new[] { ModDependency.Required("tests.shared.a") },
        };
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal)
        {
            ["tests.shared.a"] = sharedA,
            ["tests.regular.r"] = regularR,
        };

        (IReadOnlyList<ModManifest> sortedShared, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortSharedMods(new[] { sharedA }, manifestsById);

        cycleErrors.Should().BeEmpty(
            "D-5 only governs the shared-mod dependency graph; cross-kind " +
            "edges to regular mods are out of scope at this layer");
        sortedShared.Should().ContainSingle().Which.Id.Should().Be("tests.shared.a");
    }

    /// <summary>
    /// Builds a <see cref="LoadedSharedMod"/> minimally for tests that only
    /// exercise Phase F's manifest-level validations. The shared ALC owns no
    /// assemblies, so the IMod scan inside Phase F runs zero iterations and
    /// only manifest-field violations fire.
    /// </summary>
    private static LoadedSharedMod MakeStubSharedMod(ModManifest manifest)
    {
        var ctx = new SharedModLoadContext();
        return new LoadedSharedMod(
            manifest.Id,
            manifest,
            ctx,
            typeof(SharedModComplianceTests).Assembly,
            Array.Empty<Type>());
    }
}
