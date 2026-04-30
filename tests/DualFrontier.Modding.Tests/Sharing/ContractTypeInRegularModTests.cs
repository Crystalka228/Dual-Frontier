using System;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Sharing;

/// <summary>
/// Acceptance tests for MOD_OS_ARCHITECTURE §5/§6.5 D-4 — Phase E of
/// <see cref="ContractValidator"/> rejects regular mods whose assemblies
/// export <c>IEvent</c> or <c>IModContract</c> types and lets shared mods
/// (the legitimate vendors of those types) pass.
/// </summary>
public sealed class ContractTypeInRegularModTests
{
    [Fact]
    public void RegularMod_WithIEventType_ProducesValidationError()
    {
        var loader = new ModLoader();
        LoadedMod bad = loader.LoadRegularMod(TestModPaths.BadRegularMod, sharedAlc: null);
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(new[] { bad }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError eventError = report.Errors.Should()
            .Contain(e =>
                e.Kind == ValidationErrorKind.ContractTypeInRegularMod &&
                e.ModId == "tests.bad-regular" &&
                e.Message.Contains("BadEvent") &&
                e.Message.Contains(nameof(IEvent)))
            .Subject;
        eventError.Message.Should().Contain("Fixture.BadRegularMod",
            "the diagnostic must name the offending assembly");
    }

    [Fact]
    public void RegularMod_WithIModContractType_ProducesValidationError()
    {
        var loader = new ModLoader();
        LoadedMod bad = loader.LoadRegularMod(TestModPaths.BadRegularMod, sharedAlc: null);
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(new[] { bad }, Array.Empty<SystemBase>());

        report.IsValid.Should().BeFalse();
        ValidationError contractError = report.Errors.Should()
            .Contain(e =>
                e.Kind == ValidationErrorKind.ContractTypeInRegularMod &&
                e.ModId == "tests.bad-regular" &&
                e.Message.Contains("BadContract") &&
                e.Message.Contains(nameof(IModContract)))
            .Subject;
        contractError.Message.Should().Contain("kind=\"shared\"",
            "the diagnostic must hint at the remediation — move the type to a shared mod");
    }

    [Fact]
    public void RegularMod_WithMultipleBadTypes_AccumulatesErrors()
    {
        var loader = new ModLoader();
        LoadedMod bad = loader.LoadRegularMod(TestModPaths.BadRegularMod, sharedAlc: null);
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(new[] { bad }, Array.Empty<SystemBase>());

        int phaseECount = report.Errors
            .Count(e => e.Kind == ValidationErrorKind.ContractTypeInRegularMod);
        phaseECount.Should().Be(2,
            "Phase E reports one error per offending type — BadEvent and BadContract.");
    }

    [Fact]
    public void SharedMod_WithIEventType_NoError()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();
        // Fixture.SharedEvents exports SharedTestEvent : IEvent — the
        // legitimate §5 use case. LoadSharedMod returns LoadedSharedMod, which
        // never reaches the validator's regular-mods list, so Phase E does
        // not fire.
        _ = loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(
            mods: Array.Empty<LoadedMod>(),
            coreSystems: Array.Empty<SystemBase>());

        report.Errors.Should().NotContain(e =>
            e.Kind == ValidationErrorKind.ContractTypeInRegularMod);
    }

    [Fact]
    public void GoodRegularMod_NoContractTypes_NoError()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();
        // Publisher references SharedTestEvent but does not define an
        // IEvent/IModContract type; Phase E must therefore find nothing.
        _ = loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);
        LoadedMod publisher = loader.LoadRegularMod(TestModPaths.PublisherMod, sharedAlc);
        var validator = new ContractValidator();

        ValidationReport report = validator.Validate(
            new[] { publisher },
            Array.Empty<SystemBase>());

        report.Errors.Should().NotContain(e =>
            e.Kind == ValidationErrorKind.ContractTypeInRegularMod);
    }

    [Fact]
    public void Phase_E_RunsBeforeInitialize()
    {
        // Full pipeline: Apply runs Validate → IMod.Initialize. BadMod's
        // Initialize throws on call, which the pipeline would record as a
        // "threw during Initialize" error. Asserting that no such error
        // appears — only ContractTypeInRegularMod ones — proves Phase E
        // rejects the mod before Initialize is invoked (the architectural
        // ordering invariant).
        var loader = new ModLoader();
        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        var validator = new ContractValidator();
        var contractStore = new ModContractStore();
        var services = new GameServices();
        var world = new World();
        var ticks = new TickScheduler();
        var graph = new DependencyGraph();
        graph.Build();
        var scheduler = new ParallelSystemScheduler(graph.GetPhases(), ticks, world);
        var pipeline = new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler);

        PipelineResult result = pipeline.Apply(new[] { TestModPaths.BadRegularMod });

        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.ContractTypeInRegularMod);
        result.Errors.Should().NotContain(
            e => e.Message.Contains("threw during Initialize"),
            "Phase E must reject the mod before IMod.Initialize is invoked — " +
            "BadMod.Initialize throws on call, so its absence here proves the " +
            "validation-precedes-initialization invariant.");
    }
}
