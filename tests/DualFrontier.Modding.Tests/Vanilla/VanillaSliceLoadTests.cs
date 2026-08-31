using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Vanilla;

/// <summary>
/// W4 / D14 — the six vanilla slice mods are REAL loadable mods.
///
/// <para>
/// Until W4 they were inert source directories: an assembly, a manifest, and an empty
/// <c>Initialize</c>. Nothing ever loaded them, and because nothing loaded them, two defects
/// that would each have blocked a real load sat unnoticed — the shared vendor's assembly name
/// missed the name <c>ModLoader</c> derives from its manifest id by the <c>.Mod</c> segment (so
/// it could not resolve on ANY filesystem, not merely a case-sensitive one), and no slice had
/// any route at all from its build output to a mods root.
/// </para>
///
/// <para>
/// A third suspected defect was investigated and DISMISSED with a measurement: the missing
/// copy-local suppression, which puts a private <c>DualFrontier.Contracts.dll</c> in each
/// slice's own output. It cannot cause a double identity here, for two independent reasons —
/// the deploy carries only the mod assembly and its manifest, and <c>ModLoadContext.Load</c>
/// returns the shared assembly or null without ever probing the mod directory (there is no
/// <c>AssemblyDependencyResolver</c> anywhere in <c>src/</c>). The suppression was still added
/// for hygiene and to match <c>DualFrontier.Mod.Weather</c>, but this file does not claim it
/// fixes anything reachable. See the theory below for what replaced the vacuous assertion.
/// </para>
///
/// <para>
/// They still carry no mechanics — those arrive at W5. What they do is announce what they will
/// own, which is enough to make them exercise the real pipeline.
/// </para>
///
/// <para>
/// Everything here is a production object — <c>ModLoader</c>, <c>ContractValidator</c>, the
/// capability ledger, the scheduler and a registry-backed <c>NativeWorld</c>. The mods are
/// loaded from their deployed output, exactly as an install would, per the standing lesson that
/// a wave gate must load the real artifact through the real pipeline.
/// </para>
/// </summary>
public sealed class VanillaSliceLoadTests : IDisposable
{
    private static string FixturesRoot => Path.Combine(AppContext.BaseDirectory, "Fixtures");

    /// <summary>The shared vendor. Its directory name is its ASSEMBLY name, which is its mod id.</summary>
    private static string CorePath => Path.Combine(FixturesRoot, "dualfrontier.vanilla.core");

    private static readonly string[] SliceDirs =
    {
        "DualFrontier.Mod.Vanilla.World",
        "DualFrontier.Mod.Vanilla.Pawn",
        "DualFrontier.Mod.Vanilla.Inventory",
        "DualFrontier.Mod.Vanilla.Combat",
        "DualFrontier.Mod.Vanilla.Magic",
    };

    private static readonly string[] SliceIds =
    {
        "dualfrontier.vanilla.world",
        "dualfrontier.vanilla.pawn",
        "dualfrontier.vanilla.inventory",
        "dualfrontier.vanilla.combat",
        "dualfrontier.vanilla.magic",
    };

    private readonly NativeWorld _world;
    private readonly ModIntegrationPipeline _pipeline;

    public VanillaSliceLoadTests()
    {
        // Production-faithful: Bootstrap.Run(useRegistry: true) is what the composition root uses,
        // so component type ids come from the explicit ComponentTypeRegistry (K-L4). A bare
        // `new NativeWorld()` takes the legacy path, whose ids are stable across ALCs — the
        // fidelity gap that hid F-60 through W3.
        _world = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);

        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        var ticks = new TickScheduler();
        registry.SetTickSource(() => ticks.CurrentTick);

        var graph = new DependencyGraph();
        graph.Build();

        var services = new GameServices();
        ParallelSystemScheduler scheduler = SchedulerTestFixture.BuildIsolated(
            graph.GetPhases(), ticks, _world, services: services);

        _pipeline = new ModIntegrationPipeline(
            new ModLoader(), registry, new ContractValidator(), new ModContractStore(),
            services, scheduler, new ModFaultHandler(), _world.Registry);
    }

    public void Dispose() => _world.Dispose();

    [Fact]
    public void TheSixVanillaSlices_LoadThroughTheRealPipeline()
    {
        var paths = new List<string> { CorePath };
        foreach (string dir in SliceDirs)
            paths.Add(Path.Combine(FixturesRoot, dir));

        PipelineResult result = _pipeline.Apply(paths);

        result.Success.Should().BeTrue(
            "the whole vanilla slice set must load as a batch. Failures: " +
            string.Join("; ", result.Errors.Select(e => e.Kind + ":" + e.Message)));

        // A successful Apply is itself the type-identity proof: Initialize is invoked through the
        // engine's own IMod, and each slice's body calls into the SHARED vendor with a
        // shared-owned parameter type. Were either assembly loaded twice with distinct
        // identities, the cast or the call would fail and land in result.Errors above.
        foreach (string id in SliceIds)
            result.LoadedModIds.Should().Contain(id, "every vanilla slice must be reported loaded");
    }

    [Fact]
    public void TheSharedVendor_IsNotReportedAsLoaded_BecauseLoadedModIdsCoversRegularModsOnly()
    {
        var paths = new List<string> { CorePath };
        foreach (string dir in SliceDirs)
            paths.Add(Path.Combine(FixturesRoot, dir));

        PipelineResult result = _pipeline.Apply(paths);

        result.Success.Should().BeTrue();
        result.LoadedModIds.Should().NotContain("dualfrontier.vanilla.core",
            "PipelineResult.LoadedModIds is built from the REGULAR mod list only — shared mods " +
            "are tracked separately and have no accessor. This is pinned deliberately: a boot " +
            "loader that verifies 'every requested root mod appears in LoadedModIds' would " +
            "falsely fail on any shared root mod, so the property must be visible, not folded " +
            "into a passing assertion elsewhere");
    }

    [Fact]
    public void TheSharedVendorAssembly_IsNamedForItsModId_SoModLoaderCanResolveIt()
    {
        // ModLoader derives "<id>.dll" when a manifest leaves entryAssembly empty, which a
        // shared mod's manifest MUST do (Phase F). Before W4 the project built
        // DualFrontier.Mod.Vanilla.Core.dll against a derived name of
        // dualfrontier.vanilla.core.dll — a mismatch by the ".Mod" segment, which fails on
        // case-insensitive filesystems too. This pins the name, not merely the load.
        string expected = Path.Combine(CorePath, "dualfrontier.vanilla.core.dll");

        File.Exists(expected).Should().BeTrue(
            $"the shared vendor's assembly must be named for its mod id so ModLoader resolves it: {expected}");
    }

    [Theory]
    [InlineData("dualfrontier.vanilla.core")]
    [InlineData("DualFrontier.Mod.Vanilla.World")]
    [InlineData("DualFrontier.Mod.Vanilla.Pawn")]
    [InlineData("DualFrontier.Mod.Vanilla.Inventory")]
    [InlineData("DualFrontier.Mod.Vanilla.Combat")]
    [InlineData("DualFrontier.Mod.Vanilla.Magic")]
    public void EveryDeployedSliceCarriesItsAssemblyAndManifestAndNothingElse(string deployedDir)
    {
        // What this pins, and what it deliberately does NOT.
        //
        // The obvious assertion here would be "no private DualFrontier.Contracts.dll beside the
        // mod", to pin the copy-local suppression the csprojs now carry. That assertion was
        // written first and MEASURED VACUOUS: the deploy copies only $(TargetPath) and the
        // output manifest, so the deployed directory never receives an engine assembly whatever
        // the csproj says. Removing the suppression from a slice and rebuilding left it green.
        //
        // Two independent mechanisms already make a double identity unreachable: that file list,
        // and ModLoadContext.Load, which returns the shared assembly or null and never probes the
        // mod directory (there is no AssemblyDependencyResolver anywhere in src/). So the honest
        // thing to pin is the FIRST of those two -- the deploy contract itself. Widen the deploy
        // to copy the whole output tree and this reddens, which is exactly when the private-copy
        // question stops being moot and the csproj hygiene starts carrying weight on its own.
        string dir = Path.Combine(FixturesRoot, deployedDir);
        Directory.Exists(dir).Should().BeTrue($"the slice must be deployed: {dir}");

        string[] files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(n => n is not null)
            .Select(n => n!)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        files.Should().BeEquivalentTo(
            new[] { deployedDir + ".dll", "mod.manifest.json" },
            "a deployed mod is its assembly and its manifest -- nothing else. Widening this set " +
            "would put engine assemblies beside the mod, which is inert only for as long as " +
            "ModLoadContext refuses to probe the mod directory");
    }
}
