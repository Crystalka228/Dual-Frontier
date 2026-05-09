using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// K6 Phase 3.5 — coverage for <see cref="ModFaultHandler"/> and the
/// pipeline drain path that closes the Phase 2 part 2 TODO. Verifies the
/// IModFaultSink contract surface (ReportFault / GetFaultedMods /
/// ClearFault), defensive null + idempotency behaviour, thread-safety
/// across concurrent ReportFault, and the integration path where the
/// pipeline drains the queued set on the next Apply.
///
/// Out of scope: provoking a real isolation violation through a worker
/// thread (covered indirectly via M72UnloadChainTests + M73Step7Tests'
/// coverage of the UnloadMod chain that the drain reuses), and the
/// full <see cref="ParallelSystemScheduler"/> rewiring (which would
/// replace the NullModFaultSink default in the per-system contexts —
/// noted as out-of-K6-scope in the K6 brief Phase 3.4 commentary).
/// </summary>
public sealed class ModFaultHandlerTests
{
    [Fact]
    public void ReportFault_AddsModToFaultedSet()
    {
        ModFaultHandler handler = BuildHandler();

        handler.ReportFault("tests.fault.mod-a", "isolation violation");

        IReadOnlyList<string> faulted = handler.GetFaultedMods();
        faulted.Should().ContainSingle().Which.Should().Be("tests.fault.mod-a");
    }

    [Fact]
    public void ReportFault_IsIdempotent()
    {
        // The set deduplicates via StringComparer.Ordinal — two calls for
        // the same id leave a single entry. Important: a tick can produce
        // multiple violations from the same mod before the next menu drain.
        ModFaultHandler handler = BuildHandler();

        handler.ReportFault("tests.fault.mod-a", "first");
        handler.ReportFault("tests.fault.mod-a", "second");

        IReadOnlyList<string> faulted = handler.GetFaultedMods();
        faulted.Should().ContainSingle().Which.Should().Be("tests.fault.mod-a");
    }

    [Fact]
    public void ReportFault_NullModId_NoThrow()
    {
        // Sink contract is non-throwing per IModFaultSink doc. A null id
        // can only arrive from a programming bug, not a real fault path,
        // but the handler treats it as a silent no-op so the bug surfaces
        // as missing-fault-record rather than a secondary exception that
        // masks the original IsolationViolationException.
        ModFaultHandler handler = BuildHandler();

        Action act = () => handler.ReportFault(null!, "anything");

        act.Should().NotThrow();
        handler.GetFaultedMods().Should().BeEmpty();
    }

    [Fact]
    public void GetFaultedMods_EmptyByDefault()
    {
        ModFaultHandler handler = BuildHandler();

        IReadOnlyList<string> faulted = handler.GetFaultedMods();

        faulted.Should().BeEmpty();
    }

    [Fact]
    public void ClearFault_RemovesFromSet()
    {
        ModFaultHandler handler = BuildHandler();
        handler.ReportFault("tests.fault.mod-a", "msg");

        handler.ClearFault("tests.fault.mod-a");

        handler.GetFaultedMods().Should().BeEmpty();
    }

    [Fact]
    public void ClearFault_UnknownMod_NoThrow()
    {
        // Defensive — Apply can call ClearFault for an id that was already
        // removed by a prior drain on a different code path. Must be a
        // silent no-op rather than an exception.
        ModFaultHandler handler = BuildHandler();

        Action act = () => handler.ClearFault("tests.fault.never-faulted");

        act.Should().NotThrow();
    }

    [Fact]
    public void ConcurrentReportFault_ThreadSafe()
    {
        // Faults arrive on simulation tick threads (multiple workers via
        // Parallel.ForEach). The handler must serialize the mutations so
        // the final set size equals the unique ids reported, with no
        // dropped entries from torn writes on the underlying HashSet.
        ModFaultHandler handler = BuildHandler();
        const int distinctMods = 64;
        const int duplicatesPerMod = 16;

        Parallel.For(0, distinctMods * duplicatesPerMod, i =>
        {
            int modIndex = i % distinctMods;
            handler.ReportFault($"tests.fault.mod-{modIndex:D3}", $"hit {i}");
        });

        IReadOnlyList<string> faulted = handler.GetFaultedMods();
        faulted.Should().HaveCount(distinctMods);
    }

    [Fact]
    public void ApplyAfterFault_DrainsFaultedMods()
    {
        // K6 Phase 3.3 integration — pipeline.Apply must call
        // _faultHandler.GetFaultedMods at step [-1] and unload each via
        // the §9.5 chain, then ClearFault each entry. Verified by
        // observing the handler set is empty after Apply returns.
        // The drain calls UnloadMod on a non-active id, which is a
        // no-op per UnloadMod_OnNonActiveMod_ReturnsEmptyWarnings_NoThrow
        // (M72UnloadChainTests), so the drain produces no warnings here —
        // the assertion is purely on the handler state.
        ModIntegrationPipeline pipeline = BuildPipeline();
        ModFaultHandler handler = pipeline.GetFaultHandlerForTests();
        handler.ReportFault("tests.fault.queued", "queued for drain");
        handler.GetFaultedMods().Should().ContainSingle();

        PipelineResult result = pipeline.Apply(Array.Empty<string>());

        result.Success.Should().BeTrue("empty mod batch is a valid Apply");
        handler.GetFaultedMods().Should().BeEmpty(
            "the drain at Apply step [-1] must clear every queued fault");
    }

    [Fact]
    public void ModLoader_HandleModFault_RoutesToHandler()
    {
        // K6 Phase 3.2 wiring — the public ModLoader.HandleModFault
        // surface must route the fault into the installed handler instead
        // of throwing NotImplementedException as it did pre-K6. The
        // pipeline ctor calls SetFaultHandler at construction time, so
        // the loader is wired to the same handler the pipeline owns.
        ModIntegrationPipeline pipeline = BuildPipeline();
        ModFaultHandler handler = pipeline.GetFaultHandlerForTests();
        ModLoader loader = GetLoaderForTests(pipeline);

        var ex = new ModIsolationException("simulated isolation violation");
        Action act = () => loader.HandleModFault("tests.fault.via-loader", ex);

        act.Should().NotThrow(
            "post-K6 HandleModFault must route, not throw NotImplementedException");
        handler.GetFaultedMods()
            .Should().ContainSingle()
            .Which.Should().Be("tests.fault.via-loader");
    }

    /// <summary>
    /// Builds a bare <see cref="ModFaultHandler"/> bound to a freshly
    /// constructed minimal pipeline. Tests 1–7 exercise the handler
    /// surface directly without touching the pipeline; the pipeline
    /// instance exists only to satisfy the handler's constructor
    /// non-null check.
    /// </summary>
    private static ModFaultHandler BuildHandler()
    {
        ModIntegrationPipeline pipeline = BuildPipeline();
        return pipeline.GetFaultHandlerForTests();
    }

    /// <summary>
    /// Minimal pipeline construction mirroring
    /// <see cref="M71PauseResumeTests"/>. Empty kernel-system set, default
    /// <see cref="DependencyGraph"/>, freshly constructed
    /// <see cref="ParallelSystemScheduler"/>. The pipeline's ctor wires
    /// the new <see cref="ModFaultHandler"/> automatically per K6 Phase 3.3.
    /// </summary>
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

    /// <summary>
    /// Reflective access to the pipeline's loader for the
    /// <see cref="ModLoader_HandleModFault_RoutesToHandler"/> test. The
    /// pipeline ctor stores the loader as a private readonly field;
    /// rather than introduce a new public/internal accessor for one test,
    /// reflection keeps the production surface unchanged.
    /// </summary>
    private static ModLoader GetLoaderForTests(ModIntegrationPipeline pipeline)
    {
        System.Reflection.FieldInfo? field = typeof(ModIntegrationPipeline)
            .GetField("_loader",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);
        field.Should().NotBeNull("_loader is the pipeline's stored ModLoader field");
        var loader = (ModLoader?)field!.GetValue(pipeline);
        loader.Should().NotBeNull();
        return loader!;
    }
}
