using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using DualFrontier.Modding.Tests.Fixtures.GoodMod;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// End-to-end coverage for <see cref="ModIntegrationPipeline.Apply"/> and
/// <see cref="ModIntegrationPipeline.UnloadAll"/>: happy path, version
/// conflict, write conflict, build failure atomicity, and unload semantics.
/// </summary>
public sealed class ModIntegrationPipelineTests
{
    [Fact]
    public void Pipeline_with_valid_mod_loads_and_rebuilds_scheduler()
    {
        Harness h = Harness.WithCore(new CoreSystemA());
        IReadOnlyList<SystemPhase> before = h.Scheduler.Phases;

        LoadedMod mod = InjectMod(h, "com.example.good", new GoodMod(),
            new[] { typeof(GoodSystem) });
        PipelineResult result = h.Pipeline.Apply(new[] { mod.ModId });

        result.Success.Should().BeTrue();
        result.LoadedModIds.Should().ContainSingle().Which.Should().Be("com.example.good");
        // The scheduler has been rebuilt: the phase-list reference changed.
        h.Scheduler.Phases.Should().NotBeSameAs(before);
        // The mod's new system is now part of the scheduler's phases.
        FindSystem<GoodSystem>(h.Scheduler).Should().NotBeNull();
    }

    [Fact]
    public void Pipeline_with_version_conflict_rejects_mod_keeps_old_scheduler()
    {
        Harness h = Harness.WithCore(new CoreSystemA());
        IReadOnlyList<SystemPhase> before = h.Scheduler.Phases;

        LoadedMod mod = InjectMod(h, "com.example.future", new GoodMod(),
            new[] { typeof(GoodSystem) },
            requiresVersion: "2.0.0");
        PipelineResult result = h.Pipeline.Apply(new[] { mod.ModId });

        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.Kind == ValidationErrorKind.IncompatibleContractsVersion);
        // Atomicity: the scheduler's old phase list is left untouched.
        h.Scheduler.Phases.Should().BeSameAs(before);
    }

    [Fact]
    public void Pipeline_with_write_conflict_rejects_both_mods_with_precise_message()
    {
        Harness h = Harness.WithCore(new CoreSystemA());
        IReadOnlyList<SystemPhase> before = h.Scheduler.Phases;

        LoadedMod modA = InjectMod(h, "com.example.a", new WriteAMod1(),
            new[] { typeof(WriteAMod1System) });
        LoadedMod modB = InjectMod(h, "com.example.b", new WriteAMod2(),
            new[] { typeof(WriteAMod2System) });

        PipelineResult result = h.Pipeline.Apply(new[] { modA.ModId, modB.ModId });

        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.ModId == "com.example.a" &&
            e.ConflictingModId == "com.example.b" &&
            e.ConflictingComponent == typeof(ConflictComponent));
        result.Errors.Should().Contain(e =>
            e.ModId == "com.example.b" &&
            e.ConflictingModId == "com.example.a");
        // The scheduler has not changed.
        h.Scheduler.Phases.Should().BeSameAs(before);
    }

    [Fact]
    public void Pipeline_build_failure_leaves_old_scheduler_intact()
    {
        // Setup: the mod registers a system that introduces a cycle in the
        // graph via mutual read/write with a core system. Write-write
        // validation lets it through (no direct write-write), but Build()
        // detects the cycle.
        Harness h = Harness.WithCore(new CoreReadsBWritesA());
        IReadOnlyList<SystemPhase> before = h.Scheduler.Phases;

        LoadedMod mod = InjectMod(h, "com.example.cycle", new CycleMod(),
            new[] { typeof(CycleModSystem) });
        PipelineResult result = h.Pipeline.Apply(new[] { mod.ModId });

        result.Success.Should().BeFalse();
        // The old phase-list reference must persist — atomicity.
        h.Scheduler.Phases.Should().BeSameAs(before);
    }

    [Fact]
    public void Pipeline_unload_removes_mod_systems_from_scheduler()
    {
        Harness h = Harness.WithCore(new CoreSystemA());

        LoadedMod mod = InjectMod(h, "com.example.good", new GoodMod(),
            new[] { typeof(GoodSystem) });
        h.Pipeline.Apply(new[] { mod.ModId }).Success.Should().BeTrue();
        FindSystem<GoodSystem>(h.Scheduler).Should().NotBeNull();

        h.Pipeline.UnloadAll();

        // After UnloadAll the mod systems are gone; only the core systems remain.
        FindSystem<GoodSystem>(h.Scheduler).Should().BeNull();
        FindSystem<CoreSystemA>(h.Scheduler).Should().NotBeNull();
    }

    // --- Harness ------------------------------------------------------------

    private sealed class Harness
    {
        public ModLoader Loader { get; }
        public ModRegistry Registry { get; }
        public ContractValidator Validator { get; }
        public ModContractStore ContractStore { get; }
        public IGameServices Services { get; }
        public ParallelSystemScheduler Scheduler { get; }
        public ModIntegrationPipeline Pipeline { get; }

        private Harness(
            ModLoader loader,
            ModRegistry registry,
            ContractValidator validator,
            ModContractStore contractStore,
            IGameServices services,
            ParallelSystemScheduler scheduler,
            ModIntegrationPipeline pipeline)
        {
            Loader = loader;
            Registry = registry;
            Validator = validator;
            ContractStore = contractStore;
            Services = services;
            Scheduler = scheduler;
            Pipeline = pipeline;
        }

        public static Harness WithCore(params SystemBase[] coreSystems)
        {
            var loader = new ModLoader();
            var registry = new ModRegistry();
            registry.SetCoreSystems(coreSystems);
            var validator = new ContractValidator();
            var contractStore = new ModContractStore();
            var services = new GameServices();

            // Initial scheduler — core only. The pipeline will call Rebuild later.
            var world = new World();
            var ticks = new TickScheduler();
            var graph = new DependencyGraph();
            foreach (SystemBase s in coreSystems)
                graph.AddSystem(s);
            graph.Build();
            var scheduler = SchedulerTestFixture.BuildIsolated(graph.GetPhases(), ticks, world);

            var pipeline = new ModIntegrationPipeline(
                loader, registry, validator, contractStore, services, scheduler, new ModFaultHandler());

            return new Harness(loader, registry, validator, contractStore, services, scheduler, pipeline);
        }
    }

    private static LoadedMod InjectMod(
        Harness h,
        string modId,
        IMod instance,
        IReadOnlyList<Type> declaredSystemTypes,
        string requiresVersion = "1.0.0")
    {
        var manifest = new ModManifest
        {
            Id = modId,
            Name = modId,
            Version = "1.0.0",
            Author = "Test",
            RequiresContractsVersion = requiresVersion,
        };
        var context = new ModLoadContext(modId);
        var mod = new LoadedMod(modId, manifest, instance, context, declaredSystemTypes);
        h.Loader.RegisterLoaded(mod);
        return mod;
    }

    private static T? FindSystem<T>(ParallelSystemScheduler scheduler) where T : SystemBase
    {
        foreach (SystemPhase phase in scheduler.Phases)
        {
            foreach (SystemBase s in phase.Systems)
            {
                if (s is T match)
                    return match;
            }
        }
        return null;
    }

    // --- Core stub systems --------------------------------------------------

    public sealed class CoreComponentX : IComponent { public int X { get; init; } }
    public sealed class CoreComponentY : IComponent { public int Y { get; init; } }
    public sealed class ConflictComponent : IComponent { public int V { get; init; } }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(CoreComponentX) }, bus: nameof(IGameServices.World))]
    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    public sealed class CoreSystemA : SystemBase
    {
        public override void Update(float delta) { }
    }

    // For the build-failure test: Core reads ComponentY and writes ComponentX.
    // The mod system reads ComponentX and writes ComponentY. This creates a
    // Core → Mod → Core cycle in the graph that Build() catches.
    [SystemAccess(reads: new[] { typeof(CycleModComponent) }, writes: new[] { typeof(CoreComponentX) }, bus: nameof(IGameServices.World))]
    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    public sealed class CoreReadsBWritesA : SystemBase
    {
        public override void Update(float delta) { }
    }

    public struct CycleModComponent : IComponent { public int C; }

    [SystemAccess(reads: new[] { typeof(CoreComponentX) }, writes: new[] { typeof(CycleModComponent) }, bus: nameof(IGameServices.World))]
    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    public sealed class CycleModSystem : SystemBase
    {
        public override void Update(float delta) { }
    }

    private sealed class CycleMod : IMod
    {
        public void Initialize(IModApi api)
        {
            api.RegisterComponent<CycleModComponent>();
            api.RegisterSystem<CycleModSystem>();
        }
        public void Unload() { }
    }

    // --- Write-write conflict stubs -----------------------------------------

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ConflictComponent) }, bus: nameof(IGameServices.World))]
    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    public sealed class WriteAMod1System : SystemBase
    {
        public override void Update(float delta) { }
    }

    [SystemAccess(reads: new Type[0], writes: new[] { typeof(ConflictComponent) }, bus: nameof(IGameServices.World))]
    [TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
    public sealed class WriteAMod2System : SystemBase
    {
        public override void Update(float delta) { }
    }

    private sealed class WriteAMod1 : IMod
    {
        public void Initialize(IModApi api) => api.RegisterSystem<WriteAMod1System>();
        public void Unload() { }
    }

    private sealed class WriteAMod2 : IMod
    {
        public void Initialize(IModApi api) => api.RegisterSystem<WriteAMod2System>();
        public void Unload() { }
    }
}
