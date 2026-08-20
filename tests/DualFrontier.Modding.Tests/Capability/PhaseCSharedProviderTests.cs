using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Modding;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Capability;

/// <summary>
/// W3/D3 — Phase C's third satisfiability arm (G4). Before W3 a regular mod requiring an event
/// vended by its own SHARED contracts mod was rejected outright: the arm-(2) search only ever
/// looked at the REGULAR mods list, which a shared mod never appears in. These tests pin the new
/// arm and, just as importantly, pin what it did NOT loosen — a dependency must still be declared,
/// and the ledger must actually hold the token.
/// </summary>
public sealed class PhaseCSharedProviderTests
{
    private const string SharedId = "df.weather.contracts";
    private static readonly Assembly TestAssembly = typeof(TestPublishEvent).Assembly;
    private static readonly string EventFqn = typeof(TestPublishEvent).FullName!;
    private static string OwnedToken => $"mod.{SharedId}.publish:{EventFqn}";

    [Fact]
    public void OwnerNamespacedToken_WithDeclaredSharedDependency_AndLedgerRegistration_IsSatisfied()
    {
        var ledger = new KernelCapabilityRegistry();
        ledger.RegisterOwner($"mod.{SharedId}", TestAssembly);

        LoadedMod consumer = MakeConsumer(dependsOn: SharedId);

        ValidationReport report = new ContractValidator().Validate(
            new[] { consumer },
            Array.Empty<SystemBase>(),
            kernelCapabilities: ledger,
            sharedMods: new[] { MakeShared(SharedId) });

        report.IsValid.Should().BeTrue(
            "the token's owner is a declared dependency present in the batch and the ledger holds it");
    }

    [Fact]
    public void OwnerNamespacedToken_WithoutTheDependencyDeclared_IsRejected_EvenWhenTheLedgerHasIt()
    {
        var ledger = new KernelCapabilityRegistry();
        ledger.RegisterOwner($"mod.{SharedId}", TestAssembly);

        LoadedMod consumer = MakeConsumer(dependsOn: null);   // ledger has the token; manifest does not ask

        ValidationReport report = new ContractValidator().Validate(
            new[] { consumer },
            Array.Empty<SystemBase>(),
            kernelCapabilities: ledger,
            sharedMods: new[] { MakeShared(SharedId) });

        report.IsValid.Should().BeFalse();
        report.Errors.Should().ContainSingle()
            .Which.Kind.Should().Be(ValidationErrorKind.MissingCapability,
                "implicit satisfaction stays rejected (MOD_OS §3.4) — the new arm relaxes the " +
                "PROVIDER's manifest bookkeeping, never the CONSUMER's duty to declare its dependency");
    }

    [Fact]
    public void OwnerNamespacedToken_WithDeclaredDependency_ButLedgerAbsent_IsRejected()
    {
        var ledger = new KernelCapabilityRegistry();   // nothing registered: no assembly exports it

        LoadedMod consumer = MakeConsumer(dependsOn: SharedId);

        ValidationReport report = new ContractValidator().Validate(
            new[] { consumer },
            Array.Empty<SystemBase>(),
            kernelCapabilities: ledger,
            sharedMods: new[] { MakeShared(SharedId) });

        report.IsValid.Should().BeFalse();
        report.Errors.Should().ContainSingle()
            .Which.Kind.Should().Be(ValidationErrorKind.MissingCapability,
                "the ledger is the source of truth for owner-scanned types; a declared dependency " +
                "that does not actually export the type satisfies nothing");
    }

    [Fact]
    public void ANestedDependencyId_DoesNotSatisfyAnotherOwnersToken()
    {
        // Consumer depends on "df.weather" but requires a token owned by "df.weather.contracts".
        // "mod.df.weather." IS a string prefix of that token, so any prefix-based ownership test
        // accepts it wrongly — this test is what caught exactly that bug during C4.
        var ledger = new KernelCapabilityRegistry();
        ledger.RegisterOwner($"mod.{SharedId}", TestAssembly);

        LoadedMod consumer = MakeConsumer(dependsOn: "df.weather");

        ValidationReport report = new ContractValidator().Validate(
            new[] { consumer },
            Array.Empty<SystemBase>(),
            kernelCapabilities: ledger,
            sharedMods: new[] { MakeShared("df.weather") });

        report.IsValid.Should().BeFalse();
        report.Errors.Should().ContainSingle()
            .Which.Kind.Should().Be(ValidationErrorKind.MissingCapability,
                "'df.weather' is a different owner from 'df.weather.contracts' despite the shared prefix");
    }

    // --- Helpers ------------------------------------------------------------

    private static LoadedMod MakeConsumer(string? dependsOn)
    {
        var manifest = new ModManifest
        {
            Id = "df.weathermod",
            Name = "df.weathermod",
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = "2.0.0",
            Capabilities = ManifestCapabilities.Parse(new[] { OwnedToken }, null),
            Dependencies = dependsOn is null
                ? Array.Empty<ModDependency>()
                : new[] { new ModDependency(dependsOn, null, false) },
        };
        return new LoadedMod("df.weathermod", manifest, new StubMod(),
                             new ModLoadContext("df.weathermod"), Array.Empty<Type>());
    }

    private static LoadedSharedMod MakeShared(string id)
    {
        var manifest = new ModManifest
        {
            Id = id,
            Name = id,
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = "2.0.0",
            Kind = ModKind.Shared,
        };
        return new LoadedSharedMod(id, manifest, new SharedModLoadContext(), TestAssembly,
                                   Array.Empty<Type>());
    }

    private sealed class StubMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
