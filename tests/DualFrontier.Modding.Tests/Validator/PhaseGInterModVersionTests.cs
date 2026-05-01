using System;
using System.Collections.Generic;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Validator;

/// <summary>
/// Coverage for the M5.2 Phase G inter-mod dependency version check.
/// Phase G iterates each mod's <see cref="ModDependency"/> entries and
/// — when the dependency carries a non-null <see cref="VersionConstraint"/>
/// — verifies the constraint is satisfied by the provider's
/// <see cref="ModManifest.Version"/>. Missing providers are skipped
/// silently per separation of concerns with M5.1's
/// <see cref="ModIntegrationPipeline.CheckDependencyPresence"/>.
/// Cascade-failure semantics: errors accumulate; the validator reports
/// every detected mismatch and never silently drops mods whose providers
/// fail their own validation phases.
/// </summary>
public sealed class PhaseGInterModVersionTests
{
    [Fact]
    public void Mod_WithSatisfiedDepVersion_NoError()
    {
        // A depends on B with constraint ^1.0.0; B is at 1.5.0.
        // Caret pins major and accepts higher minor/patch — satisfied.
        LoadedMod a = MakeMod(
            "com.example.a",
            "1.0.0",
            ModDependency.Required("com.example.b", VersionConstraint.Parse("^1.0.0")));
        LoadedMod b = MakeMod("com.example.b", "1.5.0");

        ValidationReport report = new ContractValidator().Validate(
            new[] { a, b }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Mod_WithUnsatisfiedDepVersion_ProducesIncompatibleVersionError()
    {
        // A depends on B with ^2.0.0; B is at 1.5.0 — major mismatch.
        // Phase G must surface IncompatibleVersion attributed to A
        // (the dependent — A's constraint is what failed).
        LoadedMod a = MakeMod(
            "com.example.a",
            "1.0.0",
            ModDependency.Required("com.example.b", VersionConstraint.Parse("^2.0.0")));
        LoadedMod b = MakeMod("com.example.b", "1.5.0");

        ValidationReport report = new ContractValidator().Validate(
            new[] { a, b }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError error = report.Errors.Should().ContainSingle().Subject;
        error.Kind.Should().Be(ValidationErrorKind.IncompatibleVersion);
        error.ModId.Should().Be("com.example.a",
            "the constraint that failed is on the dependent A, not the provider B");
        error.Message.Should().Contain("com.example.b",
            "the diagnostic must name the provider mod");
        error.Message.Should().Contain("§8.7",
            "Phase G diagnostic must cite MOD_OS_ARCHITECTURE §8.7");
    }

    [Fact]
    public void Mod_WithDepNullVersion_NoError()
    {
        // A depends on B with no version constraint (presence-only dep).
        // Phase G must skip this dep entirely — Version=null means
        // the manifest accepts any provider version.
        LoadedMod a = MakeMod(
            "com.example.a",
            "1.0.0",
            ModDependency.Required("com.example.b"));
        LoadedMod b = MakeMod("com.example.b", "0.0.1");

        ValidationReport report = new ContractValidator().Validate(
            new[] { a, b }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Mod_WithMissingProvider_PhaseGSkipsSilently()
    {
        // A depends on B with ^1.0.0; B is NOT in the batch. Phase G
        // must produce zero errors — missing-provider detection belongs
        // to M5.1's CheckDependencyPresence at the pipeline level, not
        // Phase G. Validator-level test isolates Phase G alone, so we
        // assert on report.Errors == empty (no IncompatibleVersion or
        // MissingDependency from this validator).
        LoadedMod a = MakeMod(
            "com.example.a",
            "1.0.0",
            ModDependency.Required("com.example.absent", VersionConstraint.Parse("^1.0.0")));

        ValidationReport report = new ContractValidator().Validate(
            new[] { a }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue(
            "Phase G must NOT surface any error when the provider is absent " +
            "from the batch — that case is M5.1's CheckDependencyPresence " +
            "responsibility, not Phase G");
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Mod_WithProviderInvalidVersionString_ProducesIncompatibleVersionError()
    {
        // A depends on B with ^1.0.0; B's manifest.Version is malformed
        // ("not.a.version"). Phase G must surface an IncompatibleVersion
        // error attributed to B (the provider — B's mistake, not A's),
        // citing §2.2 manifest schema.
        LoadedMod a = MakeMod(
            "com.example.a",
            "1.0.0",
            ModDependency.Required("com.example.b", VersionConstraint.Parse("^1.0.0")));
        LoadedMod b = MakeMod("com.example.b", "not.a.version");

        ValidationReport report = new ContractValidator().Validate(
            new[] { a, b }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError error = report.Errors.Should().ContainSingle().Subject;
        error.Kind.Should().Be(ValidationErrorKind.IncompatibleVersion);
        error.ModId.Should().Be("com.example.b",
            "the malformed version string is the provider's mistake");
        error.Message.Should().Contain("§2.2",
            "the manifest-schema diagnostic must cite MOD_OS_ARCHITECTURE §2.2");
        error.Message.Should().Contain("not.a.version",
            "the diagnostic must echo the offending version string");
    }

    [Fact]
    public void Mod_WithExactDepVersion_RequiresExactMatch()
    {
        // Exact constraint = 1.0.0 against provider version 1.0.0 → satisfied.
        LoadedMod aOk = MakeMod(
            "com.example.aok",
            "1.0.0",
            ModDependency.Required("com.example.bok", VersionConstraint.Parse("1.0.0")));
        LoadedMod bOk = MakeMod("com.example.bok", "1.0.0");

        ValidationReport okReport = new ContractValidator().Validate(
            new[] { aOk, bOk }, Array.Empty<SystemBase>());

        okReport.IsValid.Should().BeTrue("exact 1.0.0 matches provider 1.0.0");

        // Exact constraint = 1.0.0 against provider version 1.0.1 →
        // NOT satisfied (exact requires equality, even at patch level).
        LoadedMod aBad = MakeMod(
            "com.example.abad",
            "1.0.0",
            ModDependency.Required("com.example.bbad", VersionConstraint.Parse("1.0.0")));
        LoadedMod bBad = MakeMod("com.example.bbad", "1.0.1");

        ValidationReport badReport = new ContractValidator().Validate(
            new[] { aBad, bBad }, Array.Empty<SystemBase>());

        badReport.IsValid.Should().BeFalse();
        badReport.Errors.Should().ContainSingle()
            .Which.Kind.Should().Be(ValidationErrorKind.IncompatibleVersion);
        badReport.Errors[0].ModId.Should().Be("com.example.abad",
            "exact-mismatch attribution belongs to the dependent");
    }

    [Fact]
    public void Mod_WithCascadeFailure_BothErrorsReportedNotSkipped()
    {
        // A depends on B with ^1.0.0; B IS at 1.5.0 (so A's Phase G
        // constraint is satisfied). However, B itself fails Phase A
        // because its v2 ApiVersion=^99.0.0 cannot be satisfied by the
        // current ContractsVersion. Both errors must surface
        // independently — A passes Phase G (its dep IS at the right
        // version), B fails Phase A. This documents the cascade-failure
        // semantics: no silent skipping when a provider has its own
        // problems.
        LoadedMod a = MakeMod(
            "com.example.a",
            "1.0.0",
            ModDependency.Required("com.example.b", VersionConstraint.Parse("^1.0.0")));
        var bManifest = new ModManifest
        {
            Id = "com.example.b",
            Name = "com.example.b",
            Version = "1.5.0",
            Author = "Test",
            ApiVersion = VersionConstraint.Parse("^99.0.0"),
        };
        var bContext = new ModLoadContext("com.example.b");
        var b = new LoadedMod(
            "com.example.b", bManifest, new StubMod(), bContext, Array.Empty<Type>());

        ValidationReport report = new ContractValidator().Validate(
            new[] { a, b }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        // Exactly one error: B's Phase A failure. A passes Phase G
        // because B IS at 1.5.0 which satisfies A's ^1.0.0 constraint.
        ValidationError error = report.Errors.Should().ContainSingle().Subject;
        error.Kind.Should().Be(ValidationErrorKind.IncompatibleVersion);
        error.ModId.Should().Be("com.example.b",
            "B's Phase A failure is attributed to B, not propagated to A");
        report.Errors.Should().NotContain(e => e.ModId == "com.example.a",
            "A is NOT silently dropped — it passes Phase G because its dep " +
            "constraint IS satisfied by the provider's declared version. " +
            "Cascade-failure semantics surface both errors independently.");
    }

    private static LoadedMod MakeMod(string modId, string version, params ModDependency[] dependencies)
    {
        var manifest = new ModManifest
        {
            Id = modId,
            Name = modId,
            Version = version,
            Author = "Test",
            Dependencies = dependencies,
        };
        var context = new ModLoadContext(modId);
        return new LoadedMod(modId, manifest, new StubMod(), context, Array.Empty<Type>());
    }

    private sealed class StubMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
