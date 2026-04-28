using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

// Top-level public test systems used by Phase D acceptance tests. Defined at
// namespace scope (not nested) so reflection over the test assembly picks
// them up uniformly.

[ModCapabilities(CapabilityValidationTests.TestPublishEventToken)]
public sealed class CapAttrSystem : SystemBase
{
    public override void Update(float delta) { }
}

public sealed class NoCapAttrSystem : SystemBase
{
    public override void Update(float delta) { }
}

/// <summary>
/// Coverage for <see cref="ContractValidator"/> Phase C (capability
/// satisfiability) and Phase D (<see cref="ModCapabilitiesAttribute"/>
/// × manifest cross-check), strictly following the M3.3 acceptance criteria.
/// </summary>
public sealed class CapabilityValidationTests
{
    // Hard-coded because attribute arguments must be compile-time constants.
    // AttributeTokenConstant_matches_runtime_fqn guards the literal against a
    // future rename of TestPublishEvent (defined in KernelCapabilityRegistryTests.cs).
    internal const string TestPublishEventToken =
        "kernel.publish:DualFrontier.Modding.Tests.Capability.TestPublishEvent";

    private const string ProviderProvidedToken =
        "mod.com.example.provider.publish:Foo.Bar";

    private const string UnsatisfiableToken =
        "mod.com.example.unknown.publish:Foo.Bar";

    private static KernelCapabilityRegistry BuildKernelRegistry()
        => new(new[] { typeof(CapabilityValidationTests).Assembly });

    // --- Phase C ------------------------------------------------------------

    [Fact]
    public void PhaseC_kernel_satisfies_required_capability_is_valid()
    {
        var validator = new ContractValidator();
        ManifestCapabilities caps = ManifestCapabilities.Parse(
            required: new[] { TestPublishEventToken },
            provided: null);
        LoadedMod mod = MakeMod("com.example.consumer", caps);

        ValidationReport report = validator.Validate(
            new[] { mod }, Array.Empty<SystemBase>(), BuildKernelRegistry());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void PhaseC_unsatisfied_required_capability_reports_missing_capability()
    {
        var validator = new ContractValidator();
        ManifestCapabilities caps = ManifestCapabilities.Parse(
            required: new[] { UnsatisfiableToken },
            provided: null);
        LoadedMod mod = MakeMod("com.example.consumer", caps);

        ValidationReport report = validator.Validate(
            new[] { mod }, Array.Empty<SystemBase>(), BuildKernelRegistry());

        report.IsValid.Should().BeFalse();
        ValidationError err = report.Errors.Should().ContainSingle().Subject;
        err.Kind.Should().Be(ValidationErrorKind.MissingCapability);
        err.ModId.Should().Be("com.example.consumer");
        err.Message.Should().Contain(UnsatisfiableToken);
        err.Message.Should().Contain("com.example.consumer");
    }

    [Fact]
    public void PhaseC_dependency_provider_listed_in_dependencies_is_valid()
    {
        var validator = new ContractValidator();

        ManifestCapabilities providerCaps = ManifestCapabilities.Parse(
            required: null,
            provided: new[] { ProviderProvidedToken });
        LoadedMod provider = MakeMod("com.example.provider", providerCaps);

        ManifestCapabilities consumerCaps = ManifestCapabilities.Parse(
            required: new[] { ProviderProvidedToken },
            provided: null);
        var deps = new[] { ModDependency.Required("com.example.provider") };
        LoadedMod consumer = MakeMod("com.example.consumer", consumerCaps,
                                     dependencies: deps);

        ValidationReport report = validator.Validate(
            new[] { provider, consumer }, Array.Empty<SystemBase>(),
            BuildKernelRegistry());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void PhaseC_implicit_provider_not_listed_in_dependencies_reports_missing_capability()
    {
        var validator = new ContractValidator();

        ManifestCapabilities providerCaps = ManifestCapabilities.Parse(
            required: null,
            provided: new[] { ProviderProvidedToken });
        LoadedMod provider = MakeMod("com.example.provider", providerCaps);

        ManifestCapabilities consumerCaps = ManifestCapabilities.Parse(
            required: new[] { ProviderProvidedToken },
            provided: null);
        // Provider is loaded but NOT listed in the consumer's dependencies.
        LoadedMod consumer = MakeMod("com.example.consumer", consumerCaps);

        ValidationReport report = validator.Validate(
            new[] { provider, consumer }, Array.Empty<SystemBase>(),
            BuildKernelRegistry());

        report.IsValid.Should().BeFalse();
        ValidationError err = report.Errors.Should().ContainSingle().Subject;
        err.Kind.Should().Be(ValidationErrorKind.MissingCapability);
        err.ModId.Should().Be("com.example.consumer");
        err.Message.Should().Contain(ProviderProvidedToken);
    }

    [Fact]
    public void PhaseC_skipped_when_kernel_registry_is_null()
    {
        var validator = new ContractValidator();
        ManifestCapabilities caps = ManifestCapabilities.Parse(
            required: new[] { UnsatisfiableToken },
            provided: null);
        LoadedMod mod = MakeMod("com.example.consumer", caps);

        // No third argument — Phases C and D are skipped entirely.
        ValidationReport report = validator.Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void PhaseC_empty_manifest_capabilities_produces_no_errors()
    {
        var validator = new ContractValidator();
        LoadedMod mod = MakeMod("com.example.empty", ManifestCapabilities.Empty);

        ValidationReport report = validator.Validate(
            new[] { mod }, Array.Empty<SystemBase>(), BuildKernelRegistry());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    // --- Phase D ------------------------------------------------------------

    [Fact]
    public void PhaseD_attribute_token_present_in_manifest_is_valid()
    {
        var validator = new ContractValidator();
        ManifestCapabilities caps = ManifestCapabilities.Parse(
            required: new[] { TestPublishEventToken },
            provided: null);
        LoadedMod mod = MakeMod("com.example.consumer", caps,
                                systemTypes: new[] { typeof(CapAttrSystem) });

        ValidationReport report = validator.Validate(
            new[] { mod }, Array.Empty<SystemBase>(), BuildKernelRegistry());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void PhaseD_attribute_token_missing_from_manifest_reports_error()
    {
        var validator = new ContractValidator();
        // Manifest declares no required capabilities — but the system does.
        LoadedMod mod = MakeMod("com.example.consumer", ManifestCapabilities.Empty,
                                systemTypes: new[] { typeof(CapAttrSystem) });

        ValidationReport report = validator.Validate(
            new[] { mod }, Array.Empty<SystemBase>(), BuildKernelRegistry());

        report.IsValid.Should().BeFalse();
        ValidationError err = report.Errors.Should().ContainSingle().Subject;
        err.Kind.Should().Be(ValidationErrorKind.MissingCapability);
        err.ModId.Should().Be("com.example.consumer");
        err.Message.Should().Contain(typeof(CapAttrSystem).FullName!);
        err.Message.Should().Contain(TestPublishEventToken);
    }

    [Fact]
    public void PhaseD_system_without_attribute_produces_no_error()
    {
        var validator = new ContractValidator();
        LoadedMod mod = MakeMod("com.example.consumer", ManifestCapabilities.Empty,
                                systemTypes: new[] { typeof(NoCapAttrSystem) });

        ValidationReport report = validator.Validate(
            new[] { mod }, Array.Empty<SystemBase>(), BuildKernelRegistry());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void PhaseD_skipped_when_kernel_registry_is_null()
    {
        var validator = new ContractValidator();
        // Same arrangement as the failing case — but no registry passed, so
        // Phase D must not run.
        LoadedMod mod = MakeMod("com.example.consumer", ManifestCapabilities.Empty,
                                systemTypes: new[] { typeof(CapAttrSystem) });

        ValidationReport report = validator.Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    // --- Self-check ---------------------------------------------------------

    [Fact]
    public void AttributeTokenConstant_matches_runtime_fqn()
    {
        // Guards the hard-coded const against a future rename of TestPublishEvent.
        TestPublishEventToken.Should().Be(
            $"kernel.publish:{typeof(TestPublishEvent).FullName}");
    }

    // --- Helpers ------------------------------------------------------------

    private static LoadedMod MakeMod(
        string modId,
        ManifestCapabilities capabilities,
        IReadOnlyList<ModDependency>? dependencies = null,
        Type[]? systemTypes = null)
    {
        var manifest = new ModManifest
        {
            Id = modId,
            Name = modId,
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = "1.0.0",
            Capabilities = capabilities,
            Dependencies = dependencies ?? Array.Empty<ModDependency>(),
        };
        var context = new ModLoadContext(modId);
        IMod instance = new StubMod();
        return new LoadedMod(modId, manifest, instance, context,
                             systemTypes ?? Array.Empty<Type>());
    }

    private sealed class StubMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
