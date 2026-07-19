using System;
using System.IO;
using System.Linq;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using AwesomeAssertions;
using Xunit;
using TickRates = DualFrontier.Contracts.Attributes.TickRates;

namespace DualFrontier.Modding.Tests.Sdk;

[SystemAccess(reads: new Type[0], writes: new Type[0], bus: nameof(IGameServices.World))]
[TickRate(TickRates.NORMAL)]
public sealed class DisposeRecordingSdkSystem : ISimulationSystem
{
    public static int DisposedCount;
    public static void Reset() => DisposedCount = 0;
    public void Initialize(ISystemContext context) { }
    public void Tick(ISystemContext context) { }
    public void OnDispose() => DisposedCount++;
}

/// <summary>
/// W1-fix (Codex review) — the END-TO-END proofs the original wave gate lacked: an SDK
/// ISimulationSystem exercised through the real production wiring, not just unit doubles.
/// Covers the pipeline gaps Codex flagged: the mod-API is registered so ISystemContext can
/// resolve it; OnDispose fires on unload through the sub-scheduler teardown; and the
/// Contracts-only reference mod actually LOADS through ModIntegrationPipeline.Apply.
/// </summary>
[Collection("GameLoopSerial")]
public sealed class SdkPipelineTests
{
    [Fact]
    public void SdkSystem_Unload_FiresOnDispose_ThroughSubSchedulerTeardown()
    {
        DisposeRecordingSdkSystem.Reset();
        var registry = new ModRegistry();

        registry.RegisterSystem("test.dispose.mod", typeof(DisposeRecordingSdkSystem));

        registry.RemoveSubScheduler("test.dispose.mod").Should().BeTrue(
            "registration must populate the mod's sub-scheduler (previously done only in tests)");
        DisposeRecordingSdkSystem.DisposedCount.Should().Be(1,
            "RemoveSubScheduler.Teardown must dispose the adapter, firing ISimulationSystem.OnDispose");
    }

    [Fact]
    public void ExampleMod_LoadsAndRegistersApi_ThroughPipeline()
    {
        var loader = new ModLoader();
        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        var validator = new ContractValidator();
        var contractStore = new ModContractStore();
        IGameServices services = new GameServices();
        using var world = new NativeWorld();
        var ticks = new TickScheduler();
        var graph = new DependencyGraph();
        graph.Build();
        ParallelSystemScheduler scheduler = SchedulerTestFixture.BuildIsolated(graph.GetPhases(), ticks, world);
        var pipeline = new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());

        string modPath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "DualFrontier.Mod.Example");
        PipelineResult result = pipeline.Apply(new[] { modPath });

        result.Success.Should().BeTrue(
            "the Contracts-only reference mod (component + ISimulationSystem, no mod-defined event) must load; " +
            $"errors: {string.Join("; ", result.Errors.Select(e => $"{e.Kind}:{e.Message}"))}");
        result.Errors.Should().NotContain(e => e.Kind == ValidationErrorKind.ContractTypeInRegularMod,
            "the reference mod no longer defines its own event, so Phase E must not reject it");
        registry.GetModApi("dualfrontier.example").Should().NotBeNull(
            "the pipeline must register the mod's API so an SDK ISimulationSystem's ISystemContext resolves it");
    }
}
