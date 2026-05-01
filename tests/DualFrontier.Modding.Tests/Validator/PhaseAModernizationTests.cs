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
/// Coverage for the M5.2 Phase A modernization: <see cref="ContractValidator"/>
/// must use the legacy <see cref="ValidationErrorKind.IncompatibleContractsVersion"/>
/// path for v1 manifests (where <see cref="ModManifest.ApiVersion"/> is
/// <see langword="null"/>) and the new <see cref="ValidationErrorKind.IncompatibleVersion"/>
/// path through the <see cref="VersionConstraint"/> pipeline for v2 manifests.
/// </summary>
public sealed class PhaseAModernizationTests
{
    [Fact]
    public void V1Manifest_WithCompatibleRequiresContractsVersion_NoError()
    {
        // ApiVersion = null marks the manifest as v1; RequiresContractsVersion
        // matches the current Contracts (1.0.0) so the legacy compat check
        // passes and Phase A produces no error.
        LoadedMod mod = MakeV1Mod("com.example.v1ok", "1.0.0");

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void V1Manifest_WithIncompatibleRequiresContractsVersion_ProducesIncompatibleContractsVersionError()
    {
        // ApiVersion = null → legacy path. Required 2.0.0 vs current 1.0.0
        // — major mismatch; legacy path must surface the historical
        // IncompatibleContractsVersion kind so v1 callers and tests
        // continue to observe the same behavior.
        LoadedMod mod = MakeV1Mod("com.example.v1bad", "2.0.0");

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError error = report.Errors.Should().ContainSingle().Subject;
        error.Kind.Should().Be(ValidationErrorKind.IncompatibleContractsVersion);
        error.ModId.Should().Be("com.example.v1bad");
    }

    [Fact]
    public void V2Manifest_WithCompatibleApiVersion_NoError()
    {
        // ApiVersion = ^1.0.0 — caret accepts 1.0.0 → no error.
        LoadedMod mod = MakeV2Mod(
            "com.example.v2ok",
            VersionConstraint.Parse("^1.0.0"));

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void V2Manifest_WithIncompatibleApiVersion_ProducesIncompatibleVersionError()
    {
        // ApiVersion = ^99.0.0 — major mismatch. v2 path must surface
        // the new IncompatibleVersion kind (NOT the legacy
        // IncompatibleContractsVersion), per §11.2 M5 spec.
        LoadedMod mod = MakeV2Mod(
            "com.example.v2bad",
            VersionConstraint.Parse("^99.0.0"));

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError error = report.Errors.Should().ContainSingle().Subject;
        error.Kind.Should().Be(ValidationErrorKind.IncompatibleVersion);
        error.ModId.Should().Be("com.example.v2bad");
        error.Message.Should().Contain("§8.1",
            "the v2 path diagnostic must cite MOD_OS_ARCHITECTURE §8.1");
    }

    [Fact]
    public void V2Manifest_WithCaretAcceptsCompatibleMinorBump_NoError()
    {
        // Caret accepts higher minor/patch within same major. Current is
        // pinned at 1.0.0 so we cannot test against 1.5.3 directly here,
        // but we can test the satisfaction at the constraint level:
        // ^1.0.0 must be satisfied by 1.5.3. The validator-level test
        // mirrors §8.1 caret semantics on the available current.
        VersionConstraint constraint = VersionConstraint.Parse("^1.0.0");
        constraint.IsSatisfiedBy(new ContractsVersion(1, 5, 3))
            .Should().BeTrue("caret pins major and accepts higher minor/patch");

        // Validator-level: ApiVersion=^1.0.0, current=1.0.0 → satisfied.
        LoadedMod mod = MakeV2Mod(
            "com.example.caret",
            VersionConstraint.Parse("^1.0.0"));

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
    }

    [Fact]
    public void V2Manifest_WithExactConstraintRequiresExactMatch()
    {
        // Exact constraint = 1.0.0 against current = 1.0.0 → satisfied.
        LoadedMod ok = MakeV2Mod(
            "com.example.exactok",
            VersionConstraint.Parse("1.0.0"));

        ValidationReport okReport = new ContractValidator().Validate(
            new[] { ok }, Array.Empty<SystemBase>());

        okReport.IsValid.Should().BeTrue("exact 1.0.0 == current 1.0.0");

        // Exact constraint = 1.0.1 against current = 1.0.0 → NOT
        // satisfied (exact requires equality, not >=).
        LoadedMod bad = MakeV2Mod(
            "com.example.exactbad",
            VersionConstraint.Parse("1.0.1"));

        ValidationReport badReport = new ContractValidator().Validate(
            new[] { bad }, Array.Empty<SystemBase>());

        badReport.IsValid.Should().BeFalse();
        badReport.Errors.Should().ContainSingle()
            .Which.Kind.Should().Be(ValidationErrorKind.IncompatibleVersion);
    }

    private static LoadedMod MakeV1Mod(string modId, string requiresVersion)
    {
        var manifest = new ModManifest
        {
            Id = modId,
            Name = modId,
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = requiresVersion,
            // ApiVersion intentionally left null — this is a v1 manifest.
        };
        var context = new ModLoadContext(modId);
        IMod instance = new StubMod();
        return new LoadedMod(modId, manifest, instance, context, Array.Empty<Type>());
    }

    private static LoadedMod MakeV2Mod(string modId, VersionConstraint apiVersion)
    {
        var manifest = new ModManifest
        {
            Id = modId,
            Name = modId,
            Version = "1.0.0",
            Author = "Test",
            ApiVersion = apiVersion,
        };
        var context = new ModLoadContext(modId);
        IMod instance = new StubMod();
        return new LoadedMod(modId, manifest, instance, context, Array.Empty<Type>());
    }

    private sealed class StubMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
