using System;
using System.IO;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// End-to-end coverage for M5.2 validator-level rejection paths driven
/// through <see cref="ModIntegrationPipeline.Apply"/>. Phase A
/// modernization (v2 manifest <c>apiVersion</c>) and Phase G inter-mod
/// dependency version check both surface
/// <see cref="ValidationErrorKind.IncompatibleVersion"/> errors at the
/// pipeline boundary, and a cascade-failure scenario demonstrates that
/// both errors surface independently when a dependent's constraint
/// targets a provider that itself has a Phase A failure.
/// </summary>
public sealed class M52IntegrationTests
{
    [Fact]
    public void Apply_WithIncompatibleApiVersion_RejectsBatchWithIncompatibleVersion()
    {
        // Fixture.RegularMod_BadApiVersion declares apiVersion: "^99.0.0".
        // The current Contracts is 1.0.0, so Phase A's v2 path must
        // reject the load with IncompatibleVersion attributed to the mod.
        var pipeline = BuildPipeline();
        PipelineResult result = pipeline.Apply(new[]
        {
            M52FixturePaths.BadApiVersion,
        });

        result.Success.Should().BeFalse();
        ValidationError err = result.Errors.Should().ContainSingle(e =>
            e.Kind == ValidationErrorKind.IncompatibleVersion &&
            e.ModId == "tests.regular.badapi").Subject;
        err.Message.Should().Contain("§8.1",
            "the v2 Phase A diagnostic must cite MOD_OS_ARCHITECTURE §8.1");
        result.LoadedModIds.Should().BeEmpty();
    }

    [Fact]
    public void Apply_WithIncompatibleDepVersion_RejectsBatchWithIncompatibleVersion()
    {
        // Fixture.RegularMod_DepsBadVersion declares a dependency on
        // tests.regular.dependedon with constraint "^99.0.0", while
        // tests.regular.dependedon's manifest.Version is "1.0.0". Phase
        // G must reject the load with IncompatibleVersion attributed to
        // the dependent — the constraint that failed is on the
        // dependent, not the provider.
        var pipeline = BuildPipeline();
        PipelineResult result = pipeline.Apply(new[]
        {
            M52FixturePaths.DepsBadVersion,
            M51FixturePaths.DependedOn,
        });

        result.Success.Should().BeFalse();
        ValidationError err = result.Errors.Should().ContainSingle(e =>
            e.Kind == ValidationErrorKind.IncompatibleVersion &&
            e.ModId == "tests.regular.depsbadver").Subject;
        err.Message.Should().Contain("tests.regular.dependedon",
            "the diagnostic must name the provider mod");
        err.Message.Should().Contain("§8.7",
            "the Phase G diagnostic must cite MOD_OS_ARCHITECTURE §8.7");
        result.LoadedModIds.Should().BeEmpty();
    }

    [Fact]
    public void Apply_WithCascadeFailure_SurfacesBothErrors()
    {
        // Fixture.RegularMod_BadApiVersion (id=tests.regular.badapi)
        // fails Phase A because its v2 apiVersion=^99.0.0 is unsatisfied
        // by the current Contracts. Fixture.RegularMod_DependsOnBadApi
        // (id=tests.regular.donbadapi) declares a dep on
        // tests.regular.badapi with constraint ^99.0.0; the provider's
        // manifest.Version=1.0.0 does NOT satisfy ^99.0.0, so Phase G
        // also fails for the dependent. Both errors must surface
        // independently — no silent dropping per the cascade-failure
        // semantics documented at the helper level in test 13.
        var pipeline = BuildPipeline();
        PipelineResult result = pipeline.Apply(new[]
        {
            M52FixturePaths.BadApiVersion,
            M52FixturePaths.DependsOnBadApi,
        });

        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.IncompatibleVersion &&
            e.ModId == "tests.regular.badapi" &&
            e.Message.Contains("§8.1"),
            "the provider's Phase A failure surfaces with §8.1 citation");
        result.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.IncompatibleVersion &&
            e.ModId == "tests.regular.donbadapi" &&
            e.Message.Contains("§8.7"),
            "the dependent's Phase G failure surfaces with §8.7 citation");
        result.LoadedModIds.Should().BeEmpty();
    }

    private static ModIntegrationPipeline BuildPipeline()
    {
        var loader = new ModLoader();
        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        var validator = new ContractValidator();
        var contractStore = new ModContractStore();
        IGameServices services = new GameServices();
        var world = new World();
        var ticks = new TickScheduler();
        var graph = new DependencyGraph();
        graph.Build();
        var scheduler = new ParallelSystemScheduler(graph.GetPhases(), ticks, world);
        return new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler);
    }
}

internal static class M52FixturePaths
{
    private static readonly string FixturesRoot =
        Path.Combine(AppContext.BaseDirectory, "Fixtures");

    public static string BadApiVersion => Path.Combine(FixturesRoot, "Fixture.RegularMod_BadApiVersion");
    public static string DepsBadVersion => Path.Combine(FixturesRoot, "Fixture.RegularMod_DepsBadVersion");
    public static string DependsOnBadApi => Path.Combine(FixturesRoot, "Fixture.RegularMod_DependsOnBadApi");
}
