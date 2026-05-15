using System;
using System.IO;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// End-to-end coverage for M5.1 pipeline behavior — pass [0.6] of
/// <see cref="ModIntegrationPipeline.Apply"/>: regular-mod cycle
/// detection, missing-dependency reporting, and topological-order
/// loading. All scenarios drive the full pipeline through the public
/// <see cref="ModIntegrationPipeline.Apply"/> entry point and inspect
/// the returned <see cref="PipelineResult"/>.
/// </summary>
public sealed class M51PipelineIntegrationTests
{
    [Fact]
    public void Apply_WithRegularModCycle_RejectsBatchWithCyclicDependency()
    {
        // CyclicA → CyclicB → CyclicA. Pass [0.6] must surface a
        // CyclicDependency error per cycle member and exclude both from
        // pass 2 — neither mod's IMod.Initialize must run. (CyclicAMod
        // and CyclicBMod throw on Initialize precisely so this can be
        // observed: if either gets called, the pipeline records a
        // "threw during Initialize" error and the test catches it.)
        var pipeline = BuildPipeline();
        PipelineResult result = pipeline.Apply(new[]
        {
            M51FixturePaths.CyclicA,
            M51FixturePaths.CyclicB,
        });

        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.CyclicDependency &&
            e.ModId == "tests.regular.cyca");
        result.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.CyclicDependency &&
            e.ModId == "tests.regular.cycb");
        result.Errors.Should().NotContain(
            e => e.Message.Contains("threw during Initialize"),
            "pass [0.6] must reject the batch before pass 2 reaches IMod.Initialize");
        result.LoadedModIds.Should().BeEmpty();
        result.FailedModIds.Should().Contain("tests.regular.cyca")
            .And.Contain("tests.regular.cycb");
    }

    [Fact]
    public void Apply_WithMissingRequiredDep_RejectsBatchWithMissingDependency()
    {
        // MissingRequiredMod declares a required dependency on
        // tests.regular.absent which is never in the batch. Pass [0.6]
        // surfaces a MissingDependency error and the pipeline rolls
        // back the load.
        var pipeline = BuildPipeline();
        PipelineResult result = pipeline.Apply(new[]
        {
            M51FixturePaths.MissingRequired,
        });

        result.Success.Should().BeFalse();
        ValidationError err = result.Errors.Should().ContainSingle(e =>
            e.Kind == ValidationErrorKind.MissingDependency &&
            e.ModId == "tests.regular.missreq").Subject;
        err.Message.Should().Contain("tests.regular.absent",
            "the diagnostic must name the missing dependency");
        result.LoadedModIds.Should().BeEmpty();
    }

    [Fact]
    public void Apply_WithMissingOptionalDep_LoadsSuccessfullyWithWarning()
    {
        // MissingOptionalMod declares an OPTIONAL dependency on
        // tests.regular.absent. Optional missing deps must NOT block
        // loading; pass [0.6] emits a non-blocking ValidationWarning,
        // the mod loads, and the warning surfaces through
        // PipelineResult.Warnings.
        var pipeline = BuildPipeline();
        PipelineResult result = pipeline.Apply(new[]
        {
            M51FixturePaths.MissingOptional,
        });

        result.Success.Should().BeTrue(
            "an optional missing dependency must not block the apply");
        result.LoadedModIds.Should().ContainSingle()
            .Which.Should().Be("tests.regular.missopt");
        ValidationWarning w = result.Warnings.Should().ContainSingle(x =>
            x.ModId == "tests.regular.missopt").Subject;
        w.Message.Should().Contain("tests.regular.absent");
        w.Message.Should().Contain("Optional");
    }

    [Fact]
    public void Apply_WithSatisfiedDeps_LoadsInTopologicalOrder()
    {
        // tests.regular.dependson depends on tests.regular.dependedon.
        // Even when the input order places the dependent first, pass
        // [0.6] must reorder so the dependency loads before its
        // dependent — observable through result.LoadedModIds, which
        // mirrors the pass-2 iteration order.
        var pipeline = BuildPipeline();
        PipelineResult result = pipeline.Apply(new[]
        {
            M51FixturePaths.DependsOnAnother,  // input order intentionally NOT topological
            M51FixturePaths.DependedOn,
        });

        result.Success.Should().BeTrue();
        result.LoadedModIds.Should().HaveCount(2);
        result.LoadedModIds[0].Should().Be("tests.regular.dependedon",
            "the dependency target must load first");
        result.LoadedModIds[1].Should().Be("tests.regular.dependson",
            "the dependent must load after its dependency");
        result.Warnings.Should().BeEmpty(
            "no advisories expected when every required dep is present");
    }

    private static ModIntegrationPipeline BuildPipeline()
    {
        var loader = new ModLoader();
        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        var validator = new ContractValidator();
        var contractStore = new ModContractStore();
        IGameServices services = new GameServices();
        using var nativeWorld = new NativeWorld();
        var ticks = new TickScheduler();
        var graph = new DependencyGraph();
        graph.Build();
        var scheduler = SchedulerTestFixture.BuildIsolated(graph.GetPhases(), ticks, nativeWorld);
        return new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());
    }
}

internal static class M51FixturePaths
{
    private static readonly string FixturesRoot =
        Path.Combine(AppContext.BaseDirectory, "Fixtures");

    public static string DependsOnAnother => Path.Combine(FixturesRoot, "Fixture.RegularMod_DependsOnAnother");
    public static string DependedOn => Path.Combine(FixturesRoot, "Fixture.RegularMod_DependedOn");
    public static string CyclicA => Path.Combine(FixturesRoot, "Fixture.RegularMod_CyclicA");
    public static string CyclicB => Path.Combine(FixturesRoot, "Fixture.RegularMod_CyclicB");
    public static string MissingRequired => Path.Combine(FixturesRoot, "Fixture.RegularMod_MissingRequired");
    public static string MissingOptional => Path.Combine(FixturesRoot, "Fixture.RegularMod_MissingOptional");
}
