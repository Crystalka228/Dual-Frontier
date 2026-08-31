using System;
using System.Collections.Generic;
using System.IO;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bus;
using DualFrontier.Application.Distribution;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Contracts.Services;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Scheduling;

namespace DualFrontier.Application.Loop;

/// <summary>
/// Builds an <see cref="EngineSession"/> out of ENGINE parts only, then loads the distribution's
/// root mod set. It names no component, no event, no system and no AI type — the whole game
/// arrives through the mod pipeline.
///
/// <para>
/// <b>Why this class is not called *Bootstrap.</b> DFK005 (severity Error) permits exactly one
/// managed type whose name ends in "Bootstrap" outside <c>DualFrontier.Core.Interop</c>, and that
/// name is the one this class replaces. Kernel invariant К-L5 forbids managed bootstrap
/// FRAGMENTS duplicating the native graph; one composer is one entry, so the invariant holds and
/// the analyzer needs no change.
/// </para>
///
/// <para>
/// <b>The empty core set is the point.</b> The predecessor registered ten gameplay systems, 21
/// component types, two factories and five bus-to-render subscriptions before handing anything
/// to the session. This registers none of them: the dependency graph is built empty, the native
/// graph is computed over an empty set, and the pipeline's own rebuild at <c>Apply</c> is what
/// installs the real phases. That shape is not new — the Weather wave gate has run exactly this
/// way since W3, with an empty core set and a rebuild.
/// </para>
///
/// <para>
/// <b>Boot load, and why it fails fast.</b> Production had never loaded a mod: the pipeline was
/// fully constructed and never asked to apply anything, because the only call site sat behind a
/// menu the Launcher never opened. This calls <c>Apply</c> once, at composition, with the root
/// set the manifest declares. It throws on any refusal. A distribution missing part of its root
/// set is not a degraded distribution, it is a broken one, and the menu's warn-and-continue
/// semantics are wrong for it — that path exists for a player toggling optional content, not for
/// the product's own definition.
/// </para>
///
/// <para>
/// <b>Exactly one Apply.</b> This is required, not tidiness. A failing <c>Apply</c> rolls back
/// through <c>ModRegistry.ResetModSystems</c>, which clears EVERY mod system and component owner
/// rather than only the failing batch's — so a second boot-time apply that failed would strip
/// the first one's systems while the active set still listed them.
/// </para>
/// </summary>
internal static class EngineComposer
{
    /// <summary>
    /// Composes the session and loads the manifest's root mods.
    /// </summary>
    /// <param name="bridge">The one-way presentation queue the renderer drains.</param>
    /// <param name="manifest">The distribution definition.</param>
    /// <param name="distributionRoot">
    /// The directory the manifest was found in. The mods root is derived from it, so the manifest
    /// and the content it names cannot disagree about where they live, and no code path has to
    /// know the process working directory.
    /// </param>
    /// <param name="shutdownHooks">Test seam for the shutdown transaction.</param>
    internal static EngineSession CreateSession(
        PresentationBridge bridge,
        DistributionManifest manifest,
        string distributionRoot,
        ShutdownTransactionHooks? shutdownHooks = null)
    {
        if (bridge is null) throw new ArgumentNullException(nameof(bridge));
        if (manifest is null) throw new ArgumentNullException(nameof(manifest));
        if (distributionRoot is null) throw new ArgumentNullException(nameof(distributionRoot));

        // Registry-bound world: component type ids come from the explicit ComponentTypeRegistry
        // (K-L4) rather than the legacy implicit path.
        var nativeWorld = DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true);

        var services = new GameServices();
        var ticks = new TickScheduler();

        var modRegistry = new ModRegistry();
        modRegistry.SetTickSource(() => ticks.CurrentTick);
        modRegistry.SetPresentationSink(new BridgePresentationSink(bridge));
        modRegistry.SetScenario(manifest.Scenario);
        modRegistry.SetSystemServices(new UnprovidedSystemServices());
        modRegistry.SetCoreSystems(Array.Empty<SystemBase>());

        IReadOnlyList<SystemBase> coreSystems = modRegistry.GetCoreSystemInstances();

        var graph = new DependencyGraph();
        foreach (SystemBase s in coreSystems)
            graph.AddSystem(s);
        graph.Build();

        // К-L12 — the native scheduler graph is cleared and recomputed here. With no engine-side
        // systems the loop body does not execute, which is correct and worth stating: the native
        // graph does not mirror mod-registered systems, and nothing in production reads it after
        // boot (ExecuteTick is entirely managed, and the per-tick native entry points have no
        // production callers). Mirroring it is ledgered, not done here.
        SystemGraphInterop.Clear();
        WakeRegistryInterop.Clear();
        for (uint i = 0; i < coreSystems.Count; i++)
        {
            SystemBase s = coreSystems[(int)i];
            SystemGraphInterop.RegisterSystem(
                systemId: i,
                systemFqn: s.GetType().FullName ?? s.GetType().Name,
                readComponentIds: ReadOnlySpan<uint>.Empty,
                writeComponentIds: ReadOnlySpan<uint>.Empty,
                priorityClass: 2,
                wakeType: 0);
            WakeRegistryInterop.SubscribeTimer(i, 1);
        }
        SystemGraphInterop.ComputeStaticGraph();

        var modLoader = new ModLoader();
        var faultHandler = new ModFaultHandler();
        modLoader.SetFaultHandler(faultHandler);

        IReadOnlyDictionary<SystemBase, SystemMetadata> initialMetadata =
            SystemMetadataBuilder.Build(modRegistry);

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(),
            ticks,
            initialMetadata,
            faultHandler,
            nativeWorld,
            services,
            modRegistry);

        var modValidator = new ContractValidator();
        var modContractStore = new ModContractStore();
        // The pipeline receives the world's component registry so a mod's components take
        // owner-scoped native ids at Apply (ID-A).
        var pipeline = new ModIntegrationPipeline(
            modLoader, modRegistry, modValidator, modContractStore, services, scheduler,
            faultHandler, nativeWorld.Registry);

        string modsRoot = Path.Combine(distributionRoot, ModsDirectoryName);
        var discoverer = new DefaultModDiscoverer(modsRoot);
        var controller = new ModMenuController(pipeline, discoverer);

        // К-L15 §3.8 — the Background tier drains through a live managed bridge each tick.
        var busBridge = new ManagedBusBridge();

        var loop = new GameLoop(scheduler, ticks, bridge, busBridge);

        controller.OnEditingBegan = () => loop.SetPaused(true);
        controller.OnEditingEnded = () => loop.SetPaused(false);

        var session = new EngineSession(
            nativeWorld, busBridge, pipeline, services, loop, controller, shutdownHooks);

        // A quarantined mod surfaces as a session Degraded reason (ELT §4.1).
        scheduler.OnModQuarantined = (modId, tickId) =>
            session.ReportDegraded(DegradedReason.ForQuarantinedMod(modId, tickId));

        try
        {
            LoadRootMods(pipeline, discoverer, manifest.RootMods, modsRoot);
        }
        catch
        {
            // The session owns the world, the bus and the pipeline; if the root set will not
            // load there is no viable game, and leaking a live native world out of a failed
            // composition would be worse than the failure itself.
            session.Dispose();
            throw;
        }

        return session;
    }

    /// <summary>The mods directory, relative to the distribution root.</summary>
    internal const string ModsDirectoryName = "mods";

    /// <summary>
    /// Resolves each declared root mod id to the path the pipeline accepts, then applies the
    /// whole set once, in the manifest's order.
    ///
    /// <para>
    /// Ids are resolved THROUGH the discoverer rather than by building paths from strings,
    /// because a discovered mod's own record carries the path the pipeline expects. Resolving
    /// here rather than letting <c>Apply</c> discover the problem also puts the blame in the
    /// right place: an id that is absent, malformed, or missing its assembly all reach the
    /// pipeline as the same "failed to load mod from path" error, which names the path and not
    /// the id the distribution actually declared.
    /// </para>
    /// </summary>
    private static void LoadRootMods(
        ModIntegrationPipeline pipeline,
        IModDiscoverer discoverer,
        IReadOnlyList<string> rootModIds,
        string modsRoot)
    {
        if (rootModIds.Count == 0) return;

        IReadOnlyList<DiscoveredModInfo> discovered = discoverer.Discover();
        var byId = new Dictionary<string, DiscoveredModInfo>(StringComparer.Ordinal);
        foreach (DiscoveredModInfo info in discovered)
        {
            // Two directories may declare the same id -- a stale copy left by a rename is the
            // ordinary way it happens -- and the discoverer has no opinion about that: it returns
            // filesystem order, so last-one-wins would let directory ordering silently pick which
            // build of a mod the product runs. Refuse instead, naming both paths.
            if (byId.TryGetValue(info.Manifest.Id, out DiscoveredModInfo? existing))
            {
                throw new InvalidOperationException(
                    $"Two mods in '{modsRoot}' both declare the id '{info.Manifest.Id}': " +
                    $"'{existing.Path}' and '{info.Path}'. Discovery returns filesystem order, so " +
                    "resolving this by taking one of them would make the product depend on " +
                    "directory ordering. Remove the stale one.");
            }
            byId[info.Manifest.Id] = info;
        }

        var paths = new List<string>(rootModIds.Count);
        var kinds = new Dictionary<string, ModKind>(StringComparer.Ordinal);
        foreach (string id in rootModIds)
        {
            if (!byId.TryGetValue(id, out DiscoveredModInfo? info))
            {
                throw new InvalidOperationException(
                    $"Distribution root mod '{id}' was not found in '{modsRoot}'. " +
                    $"Discovered ids: {(discovered.Count == 0 ? "(none)" : string.Join(", ", Ids(discovered)))}. " +
                    "A root mod is part of the product's definition, so a missing one is fatal " +
                    "rather than a warning. Note the discoverer swallows a malformed manifest " +
                    "silently, so an id listed here and absent above may be present on disk but " +
                    "unreadable.");
            }
            paths.Add(info.Path);
            kinds[id] = info.Manifest.Kind;
        }

        PipelineResult result = pipeline.Apply(paths);
        if (!result.Success)
        {
            throw new InvalidOperationException(
                "The distribution's root mod set failed to load: " +
                string.Join("; ", Describe(result)) +
                ". The composer refuses a partial root set; the mod menu's warn-and-continue " +
                "path exists for optional content, not for the product's own definition.");
        }

        // Completeness is checked per REGULAR id only. LoadedModIds is built from the regular
        // mod list; shared mods are tracked separately and have no accessor, so asserting that
        // every requested id appears would fail on every shared root mod -- of which the vanilla
        // distribution has one. A shared mod that silently failed to load cannot be detected
        // through PipelineResult at all; that gap is ledgered rather than papered over here.
        foreach (string id in rootModIds)
        {
            if (kinds[id] == ModKind.Shared) continue;
            if (!Contains(result.LoadedModIds, id))
            {
                throw new InvalidOperationException(
                    $"Distribution root mod '{id}' reported no load failure but is absent from " +
                    "the loaded set. This should be unreachable and indicates the pipeline " +
                    "reported success for a batch it did not fully apply.");
            }
        }
    }

    private static IEnumerable<string> Ids(IReadOnlyList<DiscoveredModInfo> discovered)
    {
        foreach (DiscoveredModInfo info in discovered)
            yield return info.Manifest.Id;
    }

    private static IEnumerable<string> Describe(PipelineResult result)
    {
        foreach (ValidationError error in result.Errors)
            yield return $"{error.ModId}: {error.Kind} {error.Message}";
    }

    private static bool Contains(IReadOnlyList<string> values, string value)
    {
        for (int i = 0; i < values.Count; i++)
        {
            if (string.Equals(values[i], value, StringComparison.Ordinal)) return true;
        }
        return false;
    }

    /// <summary>
    /// The construction-time service surface a NEUTRAL engine can offer: none.
    ///
    /// <para>
    /// <c>ISystemServices</c> exists so the engine can inject services into systems it
    /// constructs. After the cut the engine constructs no gameplay system, and every service it
    /// used to supply — pathfinding over a walkability grid — is game content a mod builds and
    /// closes over itself. Rather than hand a mod null, or invent a provision API nothing asked
    /// for, the composer installs a surface that refuses loudly and says what to do instead.
    /// </para>
    ///
    /// <para>
    /// A mod using the factory overload receives this object and is free to ignore it, which is
    /// what the vanilla scenario does. Only a mod that actually reads a member sees the refusal,
    /// and then it sees a sentence rather than a null-reference.
    /// </para>
    /// </summary>
    private sealed class UnprovidedSystemServices : ISystemServices
    {
        public IPathfindingService Pathfinding
            => throw new InvalidOperationException(
                "The engine provides no pathfinding service. Pathfinding is game content: build " +
                "it in your mod and close over it in the RegisterSystem<T>(Func<ISystemServices, T>) " +
                "factory, as the vanilla scenario mod does.");
    }
}
