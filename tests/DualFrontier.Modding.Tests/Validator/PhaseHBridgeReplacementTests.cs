using System;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Validator;

// Top-level fixture classes — Phase H sweeps every loaded assembly via
// Type.GetType, so any FQN resolvable from the test assembly works as a
// replacement target. They are POCOs because Phase H only inspects the
// presence and Replaceable flag of [BridgeImplementation], not the SystemBase
// shape.
[BridgeImplementation(Phase = 99, Replaceable = true)]
public sealed class PhaseH_ReplaceableBridgeFixture { }

[BridgeImplementation(Phase = 99, Replaceable = true)]
public sealed class PhaseH_AlternateReplaceableFixture { }

[BridgeImplementation(Phase = 99, Replaceable = false)]
public sealed class PhaseH_ProtectedBridgeFixture { }

public sealed class PhaseH_NoBridgeAttributeFixture { }

/// <summary>
/// Coverage for the M6.1 Phase H bridge replacement validation. Phase H runs
/// unconditionally but early-outs when no mod in the batch declares any
/// <see cref="ModManifest.Replaces"/> entries. For each FQN it reports
/// <see cref="ValidationErrorKind.BridgeReplacementConflict"/> (two mods
/// replacing the same FQN), <see cref="ValidationErrorKind.ProtectedSystemReplacement"/>
/// (target type lacks <c>[BridgeImplementation(Replaceable = true)]</c>), and
/// <see cref="ValidationErrorKind.UnknownSystemReplacement"/> (FQN resolves
/// to no type in any loaded assembly). Per MOD_OS_ARCHITECTURE §7 LOCKED.
/// </summary>
public sealed class PhaseHBridgeReplacementTests
{
    [Fact]
    public void Mod_WithReplaceableBridge_NoError()
    {
        // FQN of PhaseH_ReplaceableBridgeFixture resolves to a type carrying
        // [BridgeImplementation(Replaceable = true)] — Phase H must accept.
        LoadedMod mod = MakeMod(
            "com.example.replaceok",
            typeof(PhaseH_ReplaceableBridgeFixture).FullName!);

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Mod_WithProtectedSystemReplacement_ProducesError()
    {
        // PhaseH_ProtectedBridgeFixture is annotated Replaceable=false. Phase H
        // must surface ProtectedSystemReplacement, and the diagnostic must
        // distinguish "Replaceable=false" from "attribute missing".
        string targetFqn = typeof(PhaseH_ProtectedBridgeFixture).FullName!;
        LoadedMod mod = MakeMod("com.example.protected", targetFqn);

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError error = report.Errors.Should().ContainSingle().Subject;
        error.Kind.Should().Be(ValidationErrorKind.ProtectedSystemReplacement);
        error.ModId.Should().Be("com.example.protected");
        error.Message.Should().Contain("Replaceable=false",
            "the diagnostic must explain why the target is protected");
        error.Message.Should().Contain("Phase = 99",
            "the diagnostic must echo the phase value when the attribute is present");
        error.Message.Should().Contain("§7.4",
            "Phase H protected diagnostic must cite MOD_OS_ARCHITECTURE §7.4");
    }

    [Fact]
    public void Mod_WithSystemMissingBridgeAttribute_ProducesProtectedError()
    {
        // PhaseH_NoBridgeAttributeFixture has no [BridgeImplementation] at all.
        // Phase H still surfaces ProtectedSystemReplacement (non-bridge kernel
        // systems are authoritative per LOCKED §7) but the diagnostic must
        // mention the missing attribute, not a phase value.
        string targetFqn = typeof(PhaseH_NoBridgeAttributeFixture).FullName!;
        LoadedMod mod = MakeMod("com.example.noattr", targetFqn);

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError error = report.Errors.Should().ContainSingle().Subject;
        error.Kind.Should().Be(ValidationErrorKind.ProtectedSystemReplacement);
        error.Message.Should().Contain("attribute missing",
            "the diagnostic must explain that no [BridgeImplementation] is present");
        error.Message.Should().NotContain("Phase = ",
            "no phase value should appear when the attribute is missing");
    }

    [Fact]
    public void Mod_WithUnknownFqn_ProducesUnknownSystemReplacementError()
    {
        // No type with this FQN exists in any loaded assembly. Phase H must
        // surface UnknownSystemReplacement and cite §7.2.
        const string ghostFqn = "DualFrontier.Phase.H.Ghost.DoesNotExistAnywhere";
        LoadedMod mod = MakeMod("com.example.ghost", ghostFqn);

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError error = report.Errors.Should().ContainSingle().Subject;
        error.Kind.Should().Be(ValidationErrorKind.UnknownSystemReplacement);
        error.ModId.Should().Be("com.example.ghost");
        error.Message.Should().Contain(ghostFqn,
            "the diagnostic must echo the offending FQN");
        error.Message.Should().Contain("§7.2",
            "Phase H unknown-FQN diagnostic must cite MOD_OS_ARCHITECTURE §7.2");
    }

    [Fact]
    public void TwoMods_ReplacingSameFqn_ProducesBridgeReplacementConflictBothSides()
    {
        // Two mods both list the same Replaceable target. Phase H must report
        // both with BridgeReplacementConflict, each pointing at the other via
        // ConflictingModId — symmetric attribution so the UI flags both cards.
        // Secondary validation (Replaceable check) is suppressed for conflicted
        // FQNs so the diagnostic stays focused on the conflict.
        string targetFqn = typeof(PhaseH_ReplaceableBridgeFixture).FullName!;
        LoadedMod a = MakeMod("com.example.a", targetFqn);
        LoadedMod b = MakeMod("com.example.b", targetFqn);

        ValidationReport report = new ContractValidator().Validate(
            new[] { a, b }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        report.Errors.Should().HaveCount(2);
        report.Errors.Should().AllSatisfy(e =>
            e.Kind.Should().Be(ValidationErrorKind.BridgeReplacementConflict));

        ValidationError errA = report.Errors.Single(e => e.ModId == "com.example.a");
        errA.ConflictingModId.Should().Be("com.example.b",
            "A's error must reference B as the conflict partner");
        errA.Message.Should().Contain("§7.2");

        ValidationError errB = report.Errors.Single(e => e.ModId == "com.example.b");
        errB.ConflictingModId.Should().Be("com.example.a",
            "B's error must reference A as the conflict partner");
        errB.Message.Should().Contain("§7.2");
    }

    [Fact]
    public void Mod_WithEmptyReplaces_PhaseHEarlyOuts()
    {
        // No mod in the batch declares any replacements. Phase H's early-out
        // must yield a clean report — exercised here by having a single mod
        // whose every other phase passes.
        LoadedMod mod = MakeMod("com.example.empty", replaces: null);

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty(
            "Phase H must produce zero errors when no mod declares any replacements");
    }

    [Fact]
    public void Mod_WithMultipleReplaces_AllValidatedIndependently()
    {
        // Single mod replaces three FQNs: one valid, one protected, one
        // unknown. Phase H reports two errors and lets the valid replacement
        // through silently — no cross-contamination between entries.
        var manifest = new ModManifest
        {
            Id = "com.example.multi",
            Name = "com.example.multi",
            Version = "1.0.0",
            Author = "Test",
            Replaces = new[]
            {
                typeof(PhaseH_ReplaceableBridgeFixture).FullName!,
                typeof(PhaseH_ProtectedBridgeFixture).FullName!,
                "DualFrontier.Phase.H.Ghost.AlsoMissing",
            },
        };
        var context = new ModLoadContext("com.example.multi");
        var mod = new LoadedMod(
            "com.example.multi", manifest, new StubMod(), context, Array.Empty<Type>());

        ValidationReport report = new ContractValidator().Validate(
            new[] { mod }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        report.Errors.Should().HaveCount(2);
        report.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.ProtectedSystemReplacement,
            "the protected target must surface ProtectedSystemReplacement");
        report.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.UnknownSystemReplacement,
            "the missing FQN must surface UnknownSystemReplacement");
        report.Errors.Should().NotContain(e =>
            e.Message.Contains(typeof(PhaseH_ReplaceableBridgeFixture).FullName!),
            "the valid replacement must not produce any error");
    }

    [Fact]
    public void TwoMods_ReplacingDifferentFqns_NoConflict()
    {
        // A replaces one Replaceable fixture; B replaces a different one.
        // Phase H must accept both — only same-FQN duplication triggers a
        // conflict, never disjoint targets.
        LoadedMod a = MakeMod(
            "com.example.a",
            typeof(PhaseH_ReplaceableBridgeFixture).FullName!);
        LoadedMod b = MakeMod(
            "com.example.b",
            typeof(PhaseH_AlternateReplaceableFixture).FullName!);

        ValidationReport report = new ContractValidator().Validate(
            new[] { a, b }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeTrue();
        report.Errors.Should().BeEmpty();
    }

    private static LoadedMod MakeMod(string modId, string? replaces)
    {
        var manifest = new ModManifest
        {
            Id = modId,
            Name = modId,
            Version = "1.0.0",
            Author = "Test",
            Replaces = replaces is null
                ? Array.Empty<string>()
                : new[] { replaces },
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
