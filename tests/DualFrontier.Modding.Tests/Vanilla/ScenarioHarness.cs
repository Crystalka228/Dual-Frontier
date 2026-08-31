using System;
using System.Collections.Generic;
using System.IO;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Distribution;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using DualFrontier.Modding.Tests.Sdk;

namespace DualFrontier.Modding.Tests.Vanilla;

/// <summary>
/// Loads the vanilla scenario mod the way an installation would, over a scenario the test
/// chooses.
///
/// <para>
/// Nothing here is a double except the presentation sink. The loader, the validator, the
/// capability ledger, the scheduler and a registry-backed native world are all production
/// objects, and the mod is loaded from its deployed output. The one thing the harness supplies
/// that a distribution would is the <see cref="ScenarioConfig"/> — which is the point: the
/// starting state is data now, so a test can ask for a different colony without touching a line
/// of the mod.
/// </para>
///
/// <para>
/// The core system set is EMPTY by construction, which mirrors the composer exactly. Every system
/// that runs here was registered by the mod.
/// </para>
/// </summary>
internal sealed class ScenarioHarness : IDisposable
{
    /// <summary>The deployed scenario mod, as the pipeline takes it: a directory path.</summary>
    internal static string ScenarioPath =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "DualFrontier.Mod.Vanilla.Scenario");

    internal NativeWorld World { get; }
    internal ModRegistry Registry { get; }
    internal TickScheduler Ticks { get; }
    internal ParallelSystemScheduler Scheduler { get; }
    internal ModIntegrationPipeline Pipeline { get; }
    internal GameServices Services { get; }
    internal RecordingPresentationSink Sink { get; } = new();

    internal ScenarioHarness(ScenarioConfig scenario)
    {
        World = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);
        Registry = new ModRegistry();
        Registry.SetCoreSystems(Array.Empty<SystemBase>());
        Ticks = new TickScheduler();
        Registry.SetTickSource(() => Ticks.CurrentTick);
        Registry.SetPresentationSink(Sink);
        Registry.SetScenario(scenario);

        var graph = new DependencyGraph();
        graph.Build();

        Services = new GameServices();
        Scheduler = SchedulerTestFixture.BuildIsolated(
            graph.GetPhases(), Ticks, World, services: Services);

        Pipeline = new ModIntegrationPipeline(
            new ModLoader(), Registry, new ContractValidator(), new ModContractStore(),
            Services, Scheduler, new ModFaultHandler(), World.Registry);
    }

    internal PipelineResult Apply() => Pipeline.Apply(new[] { ScenarioPath });

    // ── world queries the scenario suites share ───────────────────────────────

    /// <summary>Every entity carrying <typeparamref name="T"/>.</summary>
    internal static List<EntityId> Ids<T>(NativeWorld world) where T : unmanaged, IComponent
    {
        var ids = new List<EntityId>();
        using SpanLease<T> lease = world.AcquireSpan<T>();
        ReadOnlySpan<int> indices = lease.Indices;
        ReadOnlySpan<int> versions = lease.Versions;
        for (int i = 0; i < lease.Count; i++)
            ids.Add(new EntityId(indices[i], versions[indices[i]]));
        return ids;
    }

    /// <summary>The tiles occupied by every entity carrying <typeparamref name="T"/>.</summary>
    internal static List<GridVector> TilesOf<T>(NativeWorld world) where T : unmanaged, IComponent
    {
        var tiles = new List<GridVector>();
        foreach (EntityId id in Ids<T>(world))
        {
            if (world.TryGetComponent(id, out PositionComponent p))
                tiles.Add(p.Position);
        }
        return tiles;
    }

    /// <summary>Every starting entity: colonists first, then each item kind in seeding order.</summary>
    internal static List<EntityId> AllStartingEntities(NativeWorld world)
    {
        var ids = new List<EntityId>();
        ids.AddRange(Ids<IdentityComponent>(world));
        ids.AddRange(Ids<ConsumableComponent>(world));
        ids.AddRange(Ids<WaterSourceComponent>(world));
        ids.AddRange(Ids<BedComponent>(world));
        ids.AddRange(Ids<DecorativeAuraComponent>(world));
        return ids;
    }

    /// <summary>
    /// The tiles the scenario's obstacle scatter marks impassable, recomputed from the same seed
    /// the mod uses. This reproduces an INPUT the scenario declares, not a placement rule: the
    /// grid itself belongs to the mod and a test cannot reach it, but the seed and the count are
    /// data on the manifest and the sequence they generate is deterministic.
    /// </summary>
    internal static HashSet<GridVector> ObstacleTiles(ScenarioConfig scenario)
    {
        var rng = new Random(scenario.ObstacleSeed);
        var blocked = new HashSet<GridVector>();
        for (int i = 0; i < scenario.ObstacleCount; i++)
        {
            int x = rng.Next(0, scenario.MapWidth);
            int y = rng.Next(0, scenario.MapHeight);
            blocked.Add(new GridVector(x, y));
        }
        return blocked;
    }

    /// <summary>A stable ordering, so two runs can be compared element by element.</summary>
    internal static List<GridVector> Sorted(IEnumerable<GridVector> tiles)
    {
        var copy = new List<GridVector>(tiles);
        copy.Sort((a, b) => a.Y != b.Y ? a.Y - b.Y : a.X - b.X);
        return copy;
    }

    public void Dispose() => World.Dispose();
}
