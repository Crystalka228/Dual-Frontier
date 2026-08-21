using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Modding;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;

namespace DualFrontier.Modding.Tests.Weather;

/// <summary>
/// Records what a mod asked the engine to paint. Standing in for a renderer lets the wave gate
/// observe the mechanic end to end without a Vulkan device.
/// </summary>
internal sealed class RecordingSink : IPresentationSink
{
    public List<(float R, float G, float B, float Strength)> Calls { get; } = new();

    public void SetAmbientTint(float r, float g, float b, float strength)
        => Calls.Add((r, g, b, strength));
}

/// <summary>
/// A whole engine session, built from the production types, that loads the real on-disk mods
/// through the real <see cref="ModIntegrationPipeline"/>. Nothing here is a double except the
/// presentation sink: the loader, the validator, the capability ledger, the scheduler and the
/// native world are all production objects.
/// </summary>
internal sealed class WeatherHarness : IDisposable
{
    private static string FixturesRoot => Path.Combine(AppContext.BaseDirectory, "Fixtures");

    internal static string ContractsPath => Path.Combine(FixturesRoot, "dualfrontier.weather.contracts");
    internal static string WeatherPath => Path.Combine(FixturesRoot, "DualFrontier.Mod.Weather");
    internal static string GateNegativePath => Path.Combine(FixturesRoot, "tests.weather.gate.negative");

    internal NativeWorld World { get; }
    internal ModRegistry Registry { get; }
    internal TickScheduler Ticks { get; }
    internal ParallelSystemScheduler Scheduler { get; }
    internal ModIntegrationPipeline Pipeline { get; }
    internal GameServices Services { get; }
    internal RecordingSink Sink { get; } = new();

    internal WeatherHarness()
    {
        // PR #49 Codex review (P1). PRODUCTION-FAITHFUL: GameBootstrap.CreateSession builds its
        // world through Bootstrap.Run(useRegistry: true), so component type ids come from the
        // explicit ComponentTypeRegistry (K-L4), keyed on the Type OBJECT. A bare `new NativeWorld()`
        // falls back to the legacy FNV1a(AssemblyQualifiedName) path, whose ids are stable across
        // ALCs -- which silently masked the reload defect this harness exists to catch.
        // Fully qualified: the test assembly has its own DualFrontier.Modding.Tests.Bootstrap namespace.
        World = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        Registry = new ModRegistry();
        Registry.SetCoreSystems(Array.Empty<SystemBase>());

        // Without a real tick source the SDK context reads CurrentTick as 0 forever and weather
        // never transitions. Same wiring GameBootstrap performs in production.
        Ticks = new TickScheduler();
        Registry.SetTickSource(() => Ticks.CurrentTick);
        Registry.SetPresentationSink(Sink);

        var graph = new DependencyGraph();
        graph.Build();

        // ONE IGameServices for both the scheduler and the pipeline, exactly as
        // GameBootstrap.CreateSession wires it. This is load-bearing, not tidiness: an event a
        // system publishes is DEFERRED and drained by ExecutePhase after the phase barrier,
        // through the SCHEDULER's services. Give the scheduler a different instance (or null,
        // which is BuildIsolated's default) and the mod publishes into a bus nobody ever flushes,
        // so every delivery silently vanishes.
        Services = new GameServices();
        Scheduler = SchedulerTestFixture.BuildIsolated(
            graph.GetPhases(), Ticks, World, services: Services);

        // Mirrors GameBootstrap.CreateSession: the pipeline receives the world's component
        // registry, so the mod's component takes an owner-scoped id at Apply exactly as it
        // does in production. A harness that omitted it would be measuring a composition
        // the game never runs -- the fidelity gap that hid F-60 through W3.
        Pipeline = new ModIntegrationPipeline(
            new ModLoader(), Registry, new ContractValidator(), new ModContractStore(),
            Services, Scheduler, new ModFaultHandler(), World.Registry);
    }

    internal PipelineResult ApplyWeatherPair()
        => Pipeline.Apply(new[] { ContractsPath, WeatherPath });

    internal void Tick(int count)
    {
        for (int i = 0; i < count; i++)
            Scheduler.ExecuteTick(1f / 30f);
    }

    internal KernelCapabilityRegistry Ledger => Pipeline.GetKernelCapabilitiesForTests();

    public void Dispose() => World.Dispose();

    internal static string Describe(PipelineResult r)
        => string.Join("; ", r.Errors.Select(e => e.Kind + ":" + e.Message));
}
