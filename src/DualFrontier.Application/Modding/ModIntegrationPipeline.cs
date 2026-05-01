using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Result of <see cref="ModIntegrationPipeline.Apply"/>.
/// </summary>
/// <param name="Success">True only when the scheduler was rebuilt with the new mods.</param>
/// <param name="Errors">Validation errors collected during the apply; empty on success.</param>
/// <param name="LoadedModIds">Ids of mods now active after the apply.</param>
/// <param name="FailedModIds">Ids (or paths) of mods that failed to load or initialize.</param>
public sealed record PipelineResult(
    bool Success,
    IReadOnlyList<ValidationError> Errors,
    IReadOnlyList<string> LoadedModIds,
    IReadOnlyList<string> FailedModIds);

/// <summary>
/// Orchestrator for mod integration. Wires <see cref="ModLoader"/>,
/// <see cref="ContractValidator"/>, <see cref="ModRegistry"/> and
/// <see cref="ParallelSystemScheduler"/> into a single atomic flow.
///
/// Executed from the mod menu when the user presses «Apply». The runtime
/// simulation must be stopped before the call — by contract the scheduler is
/// immutable while a session is alive.
///
/// Loading runs in two passes per MOD_OS_ARCHITECTURE §5.2/§5.3: shared
/// mods first (into the singleton <see cref="SharedModLoadContext"/> the
/// pipeline owns), then regular mods whose <see cref="ModLoadContext"/>
/// delegates to that shared ALC for cross-mod type identity. The shared
/// ALC is created once per pipeline instance and never unloaded (§5.1).
///
/// Atomicity: <see cref="DependencyGraph.Build"/> runs on a locally created
/// graph. The scheduler's phase list is replaced only after a successful
/// build. Any exception earlier in the chain rolls back regular-mod
/// registration and leaves the scheduler untouched. Shared mods, once
/// loaded, persist for the session.
/// </summary>
internal sealed class ModIntegrationPipeline
{
    private readonly ModLoader _loader;
    private readonly ModRegistry _registry;
    private readonly ContractValidator _validator;
    private readonly IModContractStore _contractStore;
    private readonly IGameServices _services;
    private readonly ParallelSystemScheduler _scheduler;
    private readonly KernelCapabilityRegistry _kernelCapabilities = KernelCapabilityRegistry.BuildFromKernelAssemblies();
    private readonly SharedModLoadContext _sharedAlc = new();
    private readonly List<LoadedMod> _activeMods = new();
    private readonly List<LoadedSharedMod> _activeShared = new();

    /// <summary>
    /// Creates a pipeline bound to the given collaborators. The scheduler is
    /// the one whose phase list the pipeline rebuilds on success. The
    /// pipeline also owns a singleton <see cref="SharedModLoadContext"/>
    /// reused across every <see cref="Apply"/> invocation per
    /// MOD_OS_ARCHITECTURE §5.1.
    /// </summary>
    public ModIntegrationPipeline(
        ModLoader loader,
        ModRegistry registry,
        ContractValidator validator,
        IModContractStore contractStore,
        IGameServices services,
        ParallelSystemScheduler scheduler)
    {
        _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _contractStore = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    }

    /// <summary>
    /// Applies the given mod paths: classifies them by manifest kind, loads
    /// shared mods into the shared ALC, then loads regular mods (each into
    /// its own ALC delegating to the shared one), validates, registers,
    /// rebuilds the dependency graph and replaces the scheduler's phase
    /// list. See <see cref="ModIntegrationPipeline"/> for atomicity
    /// guarantees.
    /// </summary>
    public PipelineResult Apply(IReadOnlyList<string> modPaths)
    {
        if (modPaths is null) throw new ArgumentNullException(nameof(modPaths));

        // [0] Classify: pre-injected mods are taken as regular; on-disk
        // manifests are parsed and split by ModKind. Manifests are kept so the
        // shared-mod cycle detector (D-5) can run before any assembly load.
        var sharedEntries = new List<(string Path, ModManifest Manifest)>();
        var regularPaths = new List<string>();
        var preloadedRegulars = new List<LoadedMod>();
        var loadErrors = new List<ValidationError>();
        var failed = new List<string>();
        var manifestsById = new Dictionary<string, ModManifest>(StringComparer.Ordinal);

        foreach (string path in modPaths)
        {
            LoadedMod? preloaded = _loader.TryGetLoaded(path);
            if (preloaded is not null)
            {
                preloadedRegulars.Add(preloaded);
                manifestsById[preloaded.ModId] = preloaded.Manifest;
                continue;
            }

            try
            {
                ModManifest manifest = ModLoader.ReadManifestFromDirectory(path);
                manifestsById[manifest.Id] = manifest;
                if (manifest.Kind == ModKind.Shared)
                    sharedEntries.Add((path, manifest));
                else
                    regularPaths.Add(path);
            }
            catch (Exception ex)
            {
                failed.Add(path);
                loadErrors.Add(new ValidationError(
                    ModId: path,
                    Kind: ValidationErrorKind.MissingDependency,
                    Message: $"Failed to read manifest for '{path}': {ex.Message}"));
            }
        }

        // [0.5] D-5 LOCKED — shared-mod cycle detection. Runs after manifest
        // parsing and before any assembly load: cyclic shared mods do not
        // reach pass 1. Manifests not part of any cycle proceed in
        // topological order (per MOD_OS_ARCHITECTURE §1.4).
        var sharedManifestList = new List<ModManifest>(sharedEntries.Count);
        foreach ((string _, ModManifest m) in sharedEntries)
            sharedManifestList.Add(m);
        (IReadOnlyList<ModManifest> sortedShared, IReadOnlyList<ValidationError> cycleErrors) =
            TopoSortSharedMods(sharedManifestList, manifestsById);
        foreach (ValidationError e in cycleErrors)
        {
            loadErrors.Add(e);
            failed.Add(e.ModId);
        }

        var pathByModId = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach ((string p, ModManifest m) in sharedEntries)
            pathByModId[m.Id] = p;

        // [1] Pass 1 — shared mods in topological order. Each LoadSharedMod
        // call places its assembly in the singleton shared ALC and returns
        // the parsed mod. Cycle members from [0.5] are excluded.
        var sharedLoaded = new List<LoadedSharedMod>();
        foreach (ModManifest m in sortedShared)
        {
            string path = pathByModId[m.Id];
            try
            {
                sharedLoaded.Add(_loader.LoadSharedMod(path, _sharedAlc));
            }
            catch (Exception ex)
            {
                failed.Add(path);
                loadErrors.Add(new ValidationError(
                    ModId: path,
                    Kind: ValidationErrorKind.MissingDependency,
                    Message: $"Failed to load shared mod from '{path}': {ex.Message}"));
            }
        }

        // [2] Pass 2 — regular mods. Each ModLoadContext is wired to the
        // shared ALC so types defined by shared mods resolve to the same
        // Type instance across regular mods (MOD_OS_ARCHITECTURE §5.3).
        var loaded = new List<LoadedMod>();
        loaded.AddRange(preloadedRegulars);
        foreach (string path in regularPaths)
        {
            try
            {
                loaded.Add(_loader.LoadRegularMod(path, _sharedAlc));
            }
            catch (Exception ex)
            {
                failed.Add(path);
                loadErrors.Add(new ValidationError(
                    ModId: path,
                    Kind: ValidationErrorKind.MissingDependency,
                    Message: $"Failed to load mod from '{path}': {ex.Message}"));
            }
        }

        // [3] Validation: contract versions + write-write conflicts +
        // regular-mod contract scan + shared-mod compliance (Phase F).
        IReadOnlyList<SystemBase> coreSystems = GetCoreSystemInstances();
        ValidationReport report = _validator.Validate(
            loaded,
            coreSystems,
            kernelCapabilities: null,
            sharedMods: sharedLoaded);

        if (!report.IsValid || loadErrors.Count > 0)
        {
            RollbackLoaded(loaded);
            var errors = MergeErrors(loadErrors, report.Errors);
            return new PipelineResult(
                Success: false,
                Errors: errors,
                LoadedModIds: Array.Empty<string>(),
                FailedModIds: CollectFailedIds(loaded, failed));
        }

        // [4] IMod.Initialize — the mod registers components/systems through IModApi.
        var initFailed = new List<LoadedMod>();
        var initErrors = new List<ValidationError>();
        foreach (LoadedMod mod in loaded)
        {
            var api = new RestrictedModApi(mod.ModId, mod.Manifest, _registry, _contractStore, _services, _kernelCapabilities);
            try
            {
                mod.Instance.Initialize(api);
            }
            catch (Exception ex)
            {
                initFailed.Add(mod);
                initErrors.Add(new ValidationError(
                    mod.ModId,
                    ValidationErrorKind.MissingDependency,
                    $"Mod '{mod.ModId}' threw during Initialize: {ex.Message}"));
            }
        }

        if (initFailed.Count > 0)
        {
            // Rollback — undo every step that had succeeded: registry, contracts, loaded mods.
            _registry.ResetModSystems();
            foreach (LoadedMod mod in loaded)
                _contractStore.RevokeAll(mod.ModId);
            RollbackLoaded(loaded);
            return new PipelineResult(
                Success: false,
                Errors: initErrors,
                LoadedModIds: Array.Empty<string>(),
                FailedModIds: CollectModIds(initFailed));
        }

        // [5-7] Build the graph in a local variable — replace the scheduler only on success.
        var localGraph = new DependencyGraph();
        try
        {
            foreach (SystemRegistration reg in _registry.GetAllSystems())
                localGraph.AddSystem(reg.Instance);
            localGraph.Build();
        }
        catch (Exception ex)
        {
            // Build() failed — the current scheduler is left untouched.
            _registry.ResetModSystems();
            foreach (LoadedMod mod in loaded)
                _contractStore.RevokeAll(mod.ModId);
            RollbackLoaded(loaded);
            return new PipelineResult(
                Success: false,
                Errors: new[]
                {
                    new ValidationError(
                        ModId: "<graph>",
                        Kind: ValidationErrorKind.CyclicDependency,
                        Message: $"Dependency graph build failed: {ex.Message}"),
                },
                LoadedModIds: Array.Empty<string>(),
                FailedModIds: CollectModIds(loaded));
        }

        // [8] Atomically swap the scheduler's phases. The previous graph is no longer needed.
        _scheduler.Rebuild(localGraph.GetPhases());
        _activeMods.AddRange(loaded);
        _activeShared.AddRange(sharedLoaded);

        return new PipelineResult(
            Success: true,
            Errors: Array.Empty<ValidationError>(),
            LoadedModIds: CollectModIds(loaded),
            FailedModIds: failed);
    }

    /// <summary>
    /// Unloads every regular mod currently managed by the pipeline and
    /// rebuilds the scheduler with only the core systems. Safe to call even
    /// when no mod is active — in that case only the scheduler is rebuilt.
    /// Shared mods are not unloaded: the shared ALC is non-collectible per
    /// MOD_OS_ARCHITECTURE §5.1.
    /// </summary>
    public void UnloadAll()
    {
        foreach (LoadedMod mod in _activeMods)
        {
            _contractStore.RevokeAll(mod.ModId);
            _loader.UnloadMod(mod.ModId);
        }
        _activeMods.Clear();
        _registry.ResetModSystems();

        var localGraph = new DependencyGraph();
        foreach (SystemRegistration reg in _registry.GetAllSystems())
            localGraph.AddSystem(reg.Instance);
        localGraph.Build();
        _scheduler.Rebuild(localGraph.GetPhases());
    }

    private IReadOnlyList<SystemBase> GetCoreSystemInstances()
    {
        var result = new List<SystemBase>();
        foreach (SystemRegistration reg in _registry.GetAllSystems())
        {
            if (reg.Origin == SystemOrigin.Core)
                result.Add(reg.Instance);
        }
        return result;
    }

    private void RollbackLoaded(List<LoadedMod> loaded)
    {
        // Physically unload any mod assemblies that already made it into memory.
        foreach (LoadedMod mod in loaded)
        {
            try { _loader.UnloadMod(mod.ModId); }
            catch { /* swallowed during rollback — what matters is the rollback itself, not further precision */ }
        }
    }

    private static IReadOnlyList<string> CollectModIds(IReadOnlyList<LoadedMod> mods)
    {
        var ids = new List<string>(mods.Count);
        foreach (LoadedMod mod in mods)
            ids.Add(mod.ModId);
        return ids;
    }

    private static IReadOnlyList<string> CollectFailedIds(
        IReadOnlyList<LoadedMod> loaded,
        IReadOnlyList<string> failedPaths)
    {
        var ids = new List<string>(loaded.Count + failedPaths.Count);
        foreach (LoadedMod mod in loaded)
            ids.Add(mod.ModId);
        foreach (string path in failedPaths)
            ids.Add(path);
        return ids;
    }

    private static IReadOnlyList<ValidationError> MergeErrors(
        IReadOnlyList<ValidationError> a,
        IReadOnlyList<ValidationError> b)
    {
        var merged = new List<ValidationError>(a.Count + b.Count);
        foreach (ValidationError e in a) merged.Add(e);
        foreach (ValidationError e in b) merged.Add(e);
        return merged;
    }

    /// <summary>
    /// Topologically orders the given shared mod manifests by their inter-shared
    /// dependency graph. Thin wrapper over <see cref="TopoSortByPredicate"/>:
    /// only edges whose target is itself a shared mod participate in the cycle
    /// graph, since D-5 LOCKED applies specifically to the shared-mod
    /// dependency graph (MOD_OS_ARCHITECTURE §1.4).
    /// </summary>
    internal static (IReadOnlyList<ModManifest> SortedShared, IReadOnlyList<ValidationError> CycleErrors)
        TopoSortSharedMods(
            IReadOnlyList<ModManifest> sharedManifests,
            IReadOnlyDictionary<string, ModManifest> manifestsById)
        => TopoSortByPredicate(
            sharedManifests,
            manifestsById,
            (_, dep) => dep.Kind == ModKind.Shared,
            "MOD_OS_ARCHITECTURE §1.4 / D-5 LOCKED");

    /// <summary>
    /// Generalized topological sort over a subset of mod manifests using
    /// Kahn's algorithm. Self-dependencies and dependencies on manifests
    /// outside <paramref name="manifestsToSort"/> are skipped;
    /// <paramref name="shouldIncludeEdge"/> further filters which edges feed
    /// the cycle graph (e.g. only shared→shared edges for D-5).
    /// </summary>
    /// <param name="manifestsToSort">
    /// Subset of manifests to topologically order. Edges are formed from each
    /// manifest's <c>Dependencies</c> to other manifests in this set.
    /// </param>
    /// <param name="allManifestsById">
    /// Full manifest dictionary used to look up <c>ModDependency.ModId</c>
    /// targets. Manifests outside <paramref name="manifestsToSort"/> are
    /// queryable here only so the predicate can inspect them; they do not
    /// become nodes in the graph.
    /// </param>
    /// <param name="shouldIncludeEdge">
    /// Predicate <c>(from, to)</c> deciding whether the edge participates in
    /// cycle detection. The shared-mod sort gates on
    /// <c>to.Kind == ModKind.Shared</c>; the regular-mod sort gates on
    /// <c>to.Kind == ModKind.Regular</c>.
    /// </param>
    /// <param name="cycleMessageContext">
    /// Spec section cited in cycle error messages — e.g.
    /// <c>"MOD_OS_ARCHITECTURE §1.4 / D-5 LOCKED"</c> for shared,
    /// <c>"MOD_OS_ARCHITECTURE §1.4 / §8.7"</c> for regular.
    /// </param>
    /// <returns>
    /// A tuple of (sorted, cycleErrors). When the graph is acyclic,
    /// <c>sorted</c> is a topological ordering of every input manifest and
    /// <c>cycleErrors</c> is empty. When a cycle exists, the unprocessable
    /// manifests are excluded from <c>sorted</c> and surfaced in
    /// <c>cycleErrors</c> as one <see cref="ValidationErrorKind.CyclicDependency"/>
    /// per affected mod.
    /// </returns>
    internal static (IReadOnlyList<ModManifest> Sorted, IReadOnlyList<ValidationError> CycleErrors)
        TopoSortByPredicate(
            IReadOnlyList<ModManifest> manifestsToSort,
            IReadOnlyDictionary<string, ModManifest> allManifestsById,
            Func<ModManifest, ModManifest, bool> shouldIncludeEdge,
            string cycleMessageContext)
    {
        if (manifestsToSort is null) throw new ArgumentNullException(nameof(manifestsToSort));
        if (allManifestsById is null) throw new ArgumentNullException(nameof(allManifestsById));
        if (shouldIncludeEdge is null) throw new ArgumentNullException(nameof(shouldIncludeEdge));
        if (cycleMessageContext is null) throw new ArgumentNullException(nameof(cycleMessageContext));

        var inDegree = new Dictionary<string, int>(StringComparer.Ordinal);
        var dependents = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var byId = new Dictionary<string, ModManifest>(StringComparer.Ordinal);

        foreach (ModManifest m in manifestsToSort)
        {
            inDegree[m.Id] = 0;
            dependents[m.Id] = new List<string>();
            byId[m.Id] = m;
        }

        foreach (ModManifest m in manifestsToSort)
        {
            foreach (ModDependency dep in m.Dependencies)
            {
                if (!allManifestsById.TryGetValue(dep.ModId, out ModManifest? depManifest))
                    continue;
                if (!shouldIncludeEdge(m, depManifest))
                    continue;
                if (!inDegree.ContainsKey(dep.ModId))
                    continue;
                if (StringComparer.Ordinal.Equals(dep.ModId, m.Id))
                    continue;
                dependents[dep.ModId].Add(m.Id);
                inDegree[m.Id]++;
            }
        }

        var queue = new Queue<string>();
        foreach (KeyValuePair<string, int> kvp in inDegree)
        {
            if (kvp.Value == 0) queue.Enqueue(kvp.Key);
        }

        var sortedIds = new List<string>(manifestsToSort.Count);
        while (queue.Count > 0)
        {
            string id = queue.Dequeue();
            sortedIds.Add(id);
            foreach (string dependent in dependents[id])
            {
                inDegree[dependent]--;
                if (inDegree[dependent] == 0)
                    queue.Enqueue(dependent);
            }
        }

        var sortedManifests = new List<ModManifest>(sortedIds.Count);
        foreach (string id in sortedIds)
            sortedManifests.Add(byId[id]);

        if (sortedIds.Count == manifestsToSort.Count)
            return (sortedManifests, Array.Empty<ValidationError>());

        var sortedSet = new HashSet<string>(sortedIds, StringComparer.Ordinal);
        var unprocessed = new List<string>();
        foreach (ModManifest m in manifestsToSort)
        {
            if (!sortedSet.Contains(m.Id)) unprocessed.Add(m.Id);
        }

        var errors = new List<ValidationError>(unprocessed.Count);
        foreach (string memberId in unprocessed)
        {
            var others = new List<string>(unprocessed.Count - 1);
            foreach (string otherId in unprocessed)
            {
                if (!StringComparer.Ordinal.Equals(otherId, memberId))
                    others.Add(otherId);
            }
            string otherList = others.Count == 0 ? "<none>" : string.Join(", ", others);
            errors.Add(new ValidationError(
                memberId,
                ValidationErrorKind.CyclicDependency,
                $"Mod '{memberId}' participates in a dependency cycle with: " +
                $"{otherList}. Cycles are forbidden per {cycleMessageContext}. " +
                $"Resolve by removing one of the dependency edges."));
        }

        return (sortedManifests, errors);
    }
}
