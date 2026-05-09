using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// M7.5.A — <see cref="ModIntegrationPipeline.GetActiveMods"/>
/// snapshot semantics. Verifies the new public read API the
/// <see cref="ModMenuController"/> uses to build the editing-session
/// active-set view.
///
/// Coverage (4 tests): empty pipeline, post-Apply contents,
/// post-UnloadMod removal, fresh-list-per-call (snapshot, not live
/// view).
/// </summary>
public sealed class PipelineGetActiveModsTests
{
    // 27
    [Fact]
    public void GetActiveMods_OnFreshPipeline_ReturnsEmpty()
    {
        // The default state — no mods applied yet. Returned list
        // must be an empty IReadOnlyList, not null.
        var pipeline = BuildPipeline();

        IReadOnlyList<ActiveModInfo> result = pipeline.GetActiveMods();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // 28
    [Fact]
    public void GetActiveMods_AfterApply_ContainsLoadedMod()
    {
        // After a successful Apply the active set surfaces the mod
        // through the new public read API. Manifest is preserved
        // verbatim — the controller will use it for the §9.6
        // hotReload check on Begin.
        var pipeline = BuildPipeline();
        var loader = new ModLoader();
        LoadedMod m = BuildEmptyMod("tests.pipread.alpha", hotReload: true);
        // Re-build pipeline with our loader so RegisterLoaded works.
        pipeline = BuildPipeline(loader);
        loader.RegisterLoaded(m);
        PipelineResult applyResult = pipeline.Apply(new[] { m.ModId });
        applyResult.Success.Should().BeTrue(
            $"setup must apply cleanly (errors: " +
            $"{string.Join("; ", System.Linq.Enumerable.Select(applyResult.Errors, e => e.Message))})");

        IReadOnlyList<ActiveModInfo> result = pipeline.GetActiveMods();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                ModId = "tests.pipread.alpha",
                Manifest = new { Id = "tests.pipread.alpha", HotReload = true },
            });
    }

    // 29
    [Fact]
    public void GetActiveMods_AfterUnloadMod_NoLongerContainsRemoved()
    {
        // UnloadMod removes the mod from _activeMods — subsequent
        // GetActiveMods reflects that removal. Locks the symmetry
        // between Apply (adds) and UnloadMod (removes) on the new
        // read surface.
        var loader = new ModLoader();
        var pipeline = BuildPipeline(loader);
        LoadedMod m = BuildEmptyMod("tests.pipread.beta", hotReload: true);
        loader.RegisterLoaded(m);
        pipeline.Apply(new[] { m.ModId }).Success.Should().BeTrue();

        pipeline.UnloadMod(m.ModId);

        pipeline.GetActiveMods().Should().BeEmpty(
            "UnloadMod must remove the mod from the active-set snapshot");
    }

    // 30
    [Fact]
    public void GetActiveMods_ReturnsFreshList_NotLiveView()
    {
        // Snapshot contract: callers may iterate the returned list
        // without concern for concurrent mutation. A subsequent
        // Apply must NOT show up in a previously captured reference.
        var loader = new ModLoader();
        var pipeline = BuildPipeline(loader);
        LoadedMod a = BuildEmptyMod("tests.pipread.snap.a", hotReload: true);
        LoadedMod b = BuildEmptyMod("tests.pipread.snap.b", hotReload: true);
        loader.RegisterLoaded(a);
        loader.RegisterLoaded(b);
        pipeline.Apply(new[] { a.ModId }).Success.Should().BeTrue();

        IReadOnlyList<ActiveModInfo> captured = pipeline.GetActiveMods();
        captured.Should().HaveCount(1);

        pipeline.Apply(new[] { b.ModId }).Success.Should().BeTrue();

        captured.Should().HaveCount(1,
            "captured list must be a fresh snapshot, not a live view of _activeMods");
        pipeline.GetActiveMods().Should().HaveCount(2,
            "a fresh GetActiveMods call after the second Apply must reflect both mods");
    }

    private static ModIntegrationPipeline BuildPipeline()
        => BuildPipeline(new ModLoader());

    private static ModIntegrationPipeline BuildPipeline(ModLoader loader)
    {
        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        var validator = new ContractValidator();
        var contractStore = new ModContractStore();
        IGameServices services = new GameServices();
        var world = new World();
        var ticks = new TickScheduler();
        var graph = new DependencyGraph();
        graph.Build();
        var scheduler = SchedulerTestFixture.BuildIsolated(graph.GetPhases(), ticks, world);
        return new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());
    }

    private static LoadedMod BuildEmptyMod(string id, bool hotReload)
    {
        var manifest = new ModManifest
        {
            Id = id,
            Name = id,
            Version = "1.0.0",
            Author = "Test",
            HotReload = hotReload,
        };
        var context = new ModLoadContext(id);
        return new LoadedMod(id, manifest, new EmptyMod(), context, Array.Empty<Type>());
    }

    private sealed class EmptyMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
