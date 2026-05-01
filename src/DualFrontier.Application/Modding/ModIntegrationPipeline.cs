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
    IReadOnlyList<string> FailedModIds)
{
    /// <summary>
    /// Non-blocking advisories produced during the apply — e.g. an optional
    /// dependency missing from the load batch (M5.1). Populated on both the
    /// success and failure paths so the UI can surface advisories regardless
    /// of outcome. Always non-null; defaults to empty for backward
    /// compatibility with callers that only consume the four positional
    /// fields.
    /// </summary>
    public IReadOnlyList<ValidationWarning> Warnings { get; init; } = Array.Empty<ValidationWarning>();
}

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
///
/// Per-mod unload discipline (M7.2, per MOD_OS_ARCHITECTURE v1.4 §9.5):
/// <see cref="UnloadMod"/> implements steps 1–6 of the unload chain
/// (UnsubscribeAll → RevokeAll → RemoveMod → graph rebuild → scheduler
/// swap → ALC.Unload). Each step is wrapped in best-effort try/catch per
/// §9.5.1; failures surface as <see cref="ValidationWarning"/> entries in
/// the returned list and the chain continues. Step 7 (WeakReference
/// verification with GC pump per §9.5 step 7) lands in M7.3.
/// <see cref="UnloadAll"/> now delegates to <see cref="UnloadMod"/> per
/// mod and returns the accumulated warnings.
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
    /// Pipeline-mediated proxy for the run flag described by
    /// MOD_OS_ARCHITECTURE §9.2 / §9.3. Per the spec, hot reload requires the
    /// scheduler to be paused before <see cref="Apply"/>; per §9.3 the guard
    /// "is enforced by ModIntegrationPipeline checking the scheduler's run
    /// flag." The flag itself lives here rather than on
    /// <see cref="ParallelSystemScheduler"/> as a deliberate interpretation
    /// registered in ROADMAP M7.1: putting it on the pipeline keeps every
    /// M-phase change inside <c>DualFrontier.Application</c> /
    /// <c>DualFrontier.Modding.Tests</c> and preserves the M3–M6 boundary
    /// discipline of leaving <c>DualFrontier.Core</c> untouched. If a future
    /// closure review finds the wording is materially incompatible with
    /// pipeline-mediated state, the resolution is a v1.5 ratification — not
    /// a silent move into the scheduler.
    ///
    /// Default <c>false</c> ("paused") is load-bearing: every M0–M6 test
    /// constructs a fresh pipeline and calls <see cref="Apply"/> without ever
    /// touching <see cref="Pause"/> or <see cref="Resume"/>, so the default
    /// must let those existing flows through unchanged.
    /// </summary>
    private bool _isRunning;

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
    /// True iff the pipeline is currently in the running state surfaced by
    /// MOD_OS_ARCHITECTURE §9.3. While running, <see cref="Apply"/> and
    /// <see cref="UnloadAll"/> reject mutation attempts: the spec forbids
    /// reloading a mod during a tick, so callers must <see cref="Pause"/>
    /// the simulation first.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Drops the run flag to <c>false</c> (the §9.2 step 1 "menu pauses the
    /// scheduler" entry point). Idempotent: calling twice is a no-op and
    /// must not throw, since the menu can re-enter the paused state from
    /// either Resume or fresh construction.
    /// </summary>
    public void Pause() => _isRunning = false;

    /// <summary>
    /// Raises the run flag to <c>true</c> (the §9.2 step 4 "menu resumes the
    /// scheduler" entry point). Idempotent: the simulation loop may call
    /// <c>Resume</c> on an already-running pipeline as a defensive no-op,
    /// e.g. on a reentry from a UI dialog that did not actually pause.
    /// </summary>
    public void Resume() => _isRunning = true;

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
        if (_isRunning)
            throw new InvalidOperationException(
                "Pause the scheduler before applying mods");

        // [0] Classify: pre-injected mods are taken as regular; on-disk
        // manifests are parsed and split by ModKind. Manifests are kept so the
        // shared-mod cycle detector (D-5) can run before any assembly load.
        var sharedEntries = new List<(string Path, ModManifest Manifest)>();
        var regularEntries = new List<(string Path, ModManifest Manifest)>();
        var preloadedRegulars = new List<LoadedMod>();
        var loadErrors = new List<ValidationError>();
        var loadWarnings = new List<ValidationWarning>();
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
                    regularEntries.Add((path, manifest));
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
        (IReadOnlyList<ModManifest> sortedShared, IReadOnlyList<ValidationError> sharedCycleErrors) =
            TopoSortSharedMods(sharedManifestList, manifestsById);
        foreach (ValidationError e in sharedCycleErrors)
        {
            loadErrors.Add(e);
            failed.Add(e.ModId);
        }

        var pathByModId = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach ((string p, ModManifest m) in sharedEntries)
            pathByModId[m.Id] = p;
        foreach ((string p, ModManifest m) in regularEntries)
            pathByModId[m.Id] = p;

        // [0.6] M5.1 — Regular-mod topological sort + dependency presence
        // check. Cyclic regular mods produce CyclicDependency errors per
        // MOD_OS_ARCHITECTURE §1.4 / §8.7 and are excluded from pass 2.
        // Missing required deps produce MissingDependency errors; missing
        // optional deps produce non-blocking ValidationWarnings. Pre-injected
        // mods are not topologically sorted here (their dependencies were
        // resolved on a prior Apply), but their dependencies do participate
        // in the presence check so other mods' deps on them are recognized.
        // Cascade-failure semantics: errors accumulate; no mod is silently
        // skipped, the whole batch rolls back if any error fires.
        var regularManifestList = new List<ModManifest>(regularEntries.Count);
        foreach ((string _, ModManifest m) in regularEntries)
            regularManifestList.Add(m);
        (IReadOnlyList<ModManifest> sortedRegular, IReadOnlyList<ValidationError> regularCycleErrors) =
            TopoSortRegularMods(regularManifestList, manifestsById);
        foreach (ValidationError e in regularCycleErrors)
        {
            loadErrors.Add(e);
            failed.Add(e.ModId);
        }

        (IReadOnlyList<ValidationError> missingErrors,
         IReadOnlyList<ValidationWarning> missingOptionalWarnings) =
            CheckDependencyPresence(manifestsById);
        foreach (ValidationError e in missingErrors)
            loadErrors.Add(e);
        foreach (ValidationWarning w in missingOptionalWarnings)
            loadWarnings.Add(w);

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

        // [2] Pass 2 — regular mods in topological order from [0.6]. Cyclic
        // regulars are excluded (not in sortedRegular). Each ModLoadContext
        // is wired to the shared ALC so types defined by shared mods resolve
        // to the same Type instance across regular mods (§5.3).
        var loaded = new List<LoadedMod>();
        loaded.AddRange(preloadedRegulars);
        foreach (ModManifest m in sortedRegular)
        {
            string path = pathByModId[m.Id];
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

        IReadOnlyList<ValidationWarning> mergedWarnings =
            MergeWarnings(loadWarnings, report.Warnings);

        if (!report.IsValid || loadErrors.Count > 0)
        {
            RollbackLoaded(loaded);
            var errors = MergeErrors(loadErrors, report.Errors);
            return new PipelineResult(
                Success: false,
                Errors: errors,
                LoadedModIds: Array.Empty<string>(),
                FailedModIds: CollectFailedIds(loaded, failed))
            {
                Warnings = mergedWarnings,
            };
        }

        // [4] IMod.Initialize — the mod registers components/systems through IModApi.
        var initFailed = new List<LoadedMod>();
        var initErrors = new List<ValidationError>();
        foreach (LoadedMod mod in loaded)
        {
            var api = new RestrictedModApi(mod.ModId, mod.Manifest, _registry, _contractStore, _services, _kernelCapabilities);
            mod.Api = api;  // M7.2 — retain for unload chain step 1 per §9.5.
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
                FailedModIds: CollectModIds(initFailed))
            {
                Warnings = mergedWarnings,
            };
        }

        // [5-7] Build the graph in a local variable — replace the scheduler only on success.
        // Per MOD_OS_ARCHITECTURE §7.1 step 3, every kernel system whose FQN
        // appears in any mod's manifest.Replaces list is skipped here: the
        // bridge stays compiled but is never registered with the scheduler.
        // The mod's replacement system was registered through
        // IModApi.RegisterSystem during step [4] above and lives in
        // _registry as SystemOrigin.Mod, so it is added by the loop below
        // along with all other mod systems. Phase H (M6.1) has already
        // verified each FQN points at a [BridgeImplementation(Replaceable=true)]
        // type and that no two mods replace the same FQN — by the time we
        // reach this step, replacedFqns contains only valid skip targets.
        HashSet<string> replacedFqns = CollectReplacedFqns(loaded);
        var localGraph = new DependencyGraph();
        try
        {
            foreach (SystemRegistration reg in _registry.GetAllSystems())
            {
                if (reg.Origin == SystemOrigin.Core)
                {
                    string? fqn = reg.Instance.GetType().FullName;
                    if (fqn is not null && replacedFqns.Contains(fqn))
                        continue;
                }
                localGraph.AddSystem(reg.Instance);
            }
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
                FailedModIds: CollectModIds(loaded))
            {
                Warnings = mergedWarnings,
            };
        }

        // [8] Atomically swap the scheduler's phases. The previous graph is no longer needed.
        _scheduler.Rebuild(localGraph.GetPhases());
        _activeMods.AddRange(loaded);
        _activeShared.AddRange(sharedLoaded);

        return new PipelineResult(
            Success: true,
            Errors: Array.Empty<ValidationError>(),
            LoadedModIds: CollectModIds(loaded),
            FailedModIds: failed)
        {
            Warnings = mergedWarnings,
        };
    }

    /// <summary>
    /// Unloads a single mod by id per MOD_OS_ARCHITECTURE v1.4 §9.5 steps
    /// 1–6 + §9.5.1 best-effort failure discipline. Each step is wrapped
    /// in a try/catch; on exception a non-blocking
    /// <see cref="ValidationWarning"/> is recorded with
    /// <c>(modId, stepNumber)</c> and the chain continues to the next
    /// step. There is no atomic-unload guarantee — <c>Unload</c> is
    /// conceptually irreversible (subscriptions removed in step 1 cannot
    /// be re-attached without re-running <see cref="IModApi.Subscribe{T}"/>),
    /// and the chain is structured so each step is a no-op if its
    /// predecessor failed (e.g. <see cref="ModRegistry.RemoveMod"/> on a
    /// mod with no registered systems is harmless).
    ///
    /// Step 7 (<c>WeakReference</c> spin loop with GC pump per v1.4 §9.5
    /// step 7) is NOT performed here — that lands in M7.3 alongside the
    /// Phase 2 carried debt unload tests. M7.2's chain stops at step 6
    /// (<c>ALC.Unload</c>) and leaves verification of actual assembly
    /// release to M7.3.
    ///
    /// Idempotent: calling <see cref="UnloadMod"/> for a
    /// <paramref name="modId"/> not currently in <c>_activeMods</c>
    /// returns an empty warning list and does not throw.
    /// </summary>
    /// <param name="modId">Identifier from <see cref="ModManifest.Id"/>.</param>
    /// <returns>
    /// Warnings collected during best-effort step execution. Empty list
    /// when every step succeeded or when the mod was not active.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// If <see cref="IsRunning"/> is true. Per v1.4 §9.3, mods cannot be
    /// unloaded while the scheduler is ticking.
    /// </exception>
    public IReadOnlyList<ValidationWarning> UnloadMod(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (_isRunning)
            throw new InvalidOperationException(
                "Pause the scheduler before unloading mods");

        LoadedMod? mod = null;
        foreach (LoadedMod m in _activeMods)
        {
            if (StringComparer.Ordinal.Equals(m.ModId, modId))
            {
                mod = m;
                break;
            }
        }
        if (mod is null)
            return Array.Empty<ValidationWarning>();

        var warnings = new List<ValidationWarning>();

        // Step 1 — drop bus subscriptions. RestrictedModApi.UnsubscribeAll
        // iterates _subscriptions and calls each captured Unsubscribe action.
        TryUnloadStep(1, modId, warnings, () =>
        {
            mod.Api?.UnsubscribeAll();
        });

        // Step 2 — drop contract registrations. ModContractStore.RevokeAll
        // is the existing primitive used by validation-failure rollback (M2).
        TryUnloadStep(2, modId, warnings, () =>
        {
            _contractStore.RevokeAll(modId);
        });

        // Step 3 — drop system instances. ModRegistry.RemoveMod is the
        // per-mod surface (bulk variant ResetModSystems is what UnloadAll
        // historically used; M7.2 introduces per-mod cleanup precisely so
        // hot-reload of a single mod doesn't disturb the others).
        TryUnloadStep(3, modId, warnings, () =>
        {
            _registry.RemoveMod(modId);
        });

        // Steps 4 + 5 — rebuild graph without the mod's systems and swap
        // the scheduler. Coupled atomically: the rebuilt graph must reach
        // the scheduler before step 6 (ALC.Unload) drops the assembly,
        // otherwise the scheduler would briefly reference systems whose
        // types are being collected.
        TryUnloadStep(4, modId, warnings, () =>
        {
            var localGraph = new DependencyGraph();
            foreach (SystemRegistration reg in _registry.GetAllSystems())
                localGraph.AddSystem(reg.Instance);
            localGraph.Build();
            _scheduler.Rebuild(localGraph.GetPhases());
        });

        // Step 6 — ALC.Unload. ModLoader.UnloadMod also calls
        // mod.Instance.Unload (IMod cleanup hook) and removes from
        // ModLoader._loaded. The IMod.Unload call is consistent with
        // §9.5.1 best-effort: ModLoader's existing swallowed try/catch
        // around it is the canonical example of the discipline.
        TryUnloadStep(6, modId, warnings, () =>
        {
            _loader.UnloadMod(modId);
        });

        // Remove from _activeMods regardless of any step's success — per
        // §9.5.1, the mod is removed from the active set even if the
        // assembly didn't actually unload.
        _activeMods.Remove(mod);

        return warnings;
    }

    /// <summary>
    /// Unloads every regular mod currently managed by the pipeline by
    /// calling <see cref="UnloadMod"/> for each in turn, then forces a
    /// final scheduler rebuild to ensure the kernel-only graph is
    /// installed even when the active list was empty. Safe to call when
    /// no mod is active — only the scheduler rebuild runs in that case.
    /// Shared mods are not unloaded: the shared ALC is non-collectible
    /// per MOD_OS_ARCHITECTURE §5.1.
    ///
    /// Per v1.4 §9.5.1, best-effort: warnings from individual
    /// <see cref="UnloadMod"/> calls are accumulated and returned. The
    /// bulk-unload semantics are preserved (every active mod is removed
    /// from <c>_activeMods</c> regardless of step-level failures).
    /// </summary>
    /// <returns>
    /// Warnings accumulated across every per-mod unload chain. Empty
    /// when every mod unloaded cleanly or no mods were active.
    /// </returns>
    public IReadOnlyList<ValidationWarning> UnloadAll()
    {
        if (_isRunning)
            throw new InvalidOperationException(
                "Pause the scheduler before unloading mods");

        var warnings = new List<ValidationWarning>();

        // Snapshot _activeMods before iterating — UnloadMod mutates the list.
        var modIds = new List<string>(_activeMods.Count);
        foreach (LoadedMod mod in _activeMods)
            modIds.Add(mod.ModId);

        foreach (string modId in modIds)
        {
            IReadOnlyList<ValidationWarning> perModWarnings = UnloadMod(modId);
            foreach (ValidationWarning w in perModWarnings)
                warnings.Add(w);
        }

        // Final scheduler rebuild for the empty-active-set case. UnloadMod
        // has already rebuilt the scheduler per mod above; for the
        // no-mods-active path we still want the kernel-only graph
        // reinstalled — same semantics as the v1 UnloadAll.
        if (modIds.Count == 0)
        {
            var localGraph = new DependencyGraph();
            foreach (SystemRegistration reg in _registry.GetAllSystems())
                localGraph.AddSystem(reg.Instance);
            localGraph.Build();
            _scheduler.Rebuild(localGraph.GetPhases());
        }

        return warnings;
    }

    /// <summary>
    /// Wraps a single unload-chain step in best-effort try/catch per
    /// MOD_OS_ARCHITECTURE v1.4 §9.5.1. On exception, records a
    /// <see cref="ValidationWarning"/> attributed to <paramref name="modId"/>
    /// with the failing step number and message, then returns so the next
    /// step in <see cref="UnloadMod"/> can run. The mod is removed from
    /// the active set regardless of any step's outcome.
    /// </summary>
    private static void TryUnloadStep(
        int stepNumber,
        string modId,
        List<ValidationWarning> warnings,
        Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            warnings.Add(new ValidationWarning(
                modId,
                $"Unload step {stepNumber} failed for mod '{modId}': {ex.Message}. " +
                "Per MOD_OS_ARCHITECTURE v1.4 §9.5.1, unload is best-effort; " +
                "subsequent steps continue. The mod has been removed from the " +
                "active set regardless."));
        }
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

    private static IReadOnlyList<ValidationWarning> MergeWarnings(
        IReadOnlyList<ValidationWarning> a,
        IReadOnlyList<ValidationWarning> b)
    {
        if (a.Count + b.Count == 0)
            return Array.Empty<ValidationWarning>();
        var merged = new List<ValidationWarning>(a.Count + b.Count);
        foreach (ValidationWarning w in a) merged.Add(w);
        foreach (ValidationWarning w in b) merged.Add(w);
        return merged;
    }

    /// <summary>
    /// Collects every fully-qualified system type name declared in any mod's
    /// <see cref="ModManifest.Replaces"/> across the load batch. The pipeline
    /// uses this set in step [5-7] to skip kernel bridge systems superseded
    /// by mod replacements during dependency-graph construction
    /// (MOD_OS_ARCHITECTURE §7.1 step 3).
    ///
    /// Pre-conditions: Phase H validation has already been run by
    /// <see cref="ContractValidator"/> in step [3] and produced no errors.
    /// That means duplicates across mods (BridgeReplacementConflict),
    /// non-Replaceable targets (ProtectedSystemReplacement) and unknown
    /// FQNs (UnknownSystemReplacement) have all been rejected before this
    /// helper is reached. The helper itself is dedup-safe: an
    /// <see cref="StringComparer.Ordinal"/>-keyed
    /// <see cref="HashSet{T}"/> swallows accidental duplicates.
    ///
    /// Iterates regular mods only — shared mods cannot meaningfully populate
    /// <see cref="ModManifest.Replaces"/>; <see cref="ContractValidator"/>
    /// Phase F (M4.3) already rejects shared mods that try, so the field is
    /// effectively empty for them and the iteration cost is dominated by the
    /// regular-mod set.
    /// </summary>
    /// <param name="loaded">Regular mods successfully loaded by step [2].</param>
    /// <returns>
    /// Set of FQN strings the graph build must skip. Empty when no mod in
    /// the batch declared any replacement.
    /// </returns>
    private static HashSet<string> CollectReplacedFqns(IReadOnlyList<LoadedMod> loaded)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (LoadedMod mod in loaded)
        {
            foreach (string fqn in mod.Manifest.Replaces)
                result.Add(fqn);
        }
        return result;
    }

    /// <summary>
    /// Test seam over <see cref="CollectReplacedFqns"/> — the production
    /// helper is private static, this exposes it through
    /// <c>InternalsVisibleTo</c> for helper-level coverage of the skip-set
    /// construction logic without forcing tests through the full
    /// <see cref="Apply"/> orchestration.
    /// </summary>
    internal static HashSet<string> CollectReplacedFqnsForTests(IReadOnlyList<LoadedMod> loaded)
        => CollectReplacedFqns(loaded);

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
    /// Topologically orders the given regular mod manifests by their
    /// inter-regular dependency graph. Thin wrapper over
    /// <see cref="TopoSortByPredicate"/>: only edges whose target is itself a
    /// regular mod participate in the cycle graph (a regular mod's dependency
    /// on a shared mod, or on a mod outside the load batch, does not feed
    /// regular-mod cycle detection — those flow through other layers per
    /// MOD_OS_ARCHITECTURE §1.4 / §8.7).
    /// </summary>
    internal static (IReadOnlyList<ModManifest> Sorted, IReadOnlyList<ValidationError> CycleErrors)
        TopoSortRegularMods(
            IReadOnlyList<ModManifest> regularManifests,
            IReadOnlyDictionary<string, ModManifest> allManifestsById)
        => TopoSortByPredicate(
            regularManifests,
            allManifestsById,
            (_, dep) => dep.Kind == ModKind.Regular,
            "MOD_OS_ARCHITECTURE §1.4 / §8.7");

    /// <summary>
    /// Walks every manifest in the batch and verifies that each declared
    /// dependency is present. A missing required dependency produces a
    /// <see cref="ValidationErrorKind.MissingDependency"/> error attributed to
    /// the dependent mod; a missing optional dependency produces a
    /// non-blocking <see cref="ValidationWarning"/>. Presence is a structural
    /// check only — version compatibility is verified separately by
    /// <c>ContractValidator</c> per MOD_OS_ARCHITECTURE §8.7 / §11.2.
    /// </summary>
    /// <param name="allManifestsById">
    /// Full manifest dictionary representing the load batch. A dependency is
    /// "present" iff its <c>ModId</c> appears as a key here.
    /// </param>
    /// <returns>
    /// Tuple of (errors, warnings). Errors are empty when every required
    /// dependency is present; warnings are empty when every optional
    /// dependency is present.
    /// </returns>
    internal static (IReadOnlyList<ValidationError> MissingErrors, IReadOnlyList<ValidationWarning> MissingOptionalWarnings)
        CheckDependencyPresence(IReadOnlyDictionary<string, ModManifest> allManifestsById)
    {
        if (allManifestsById is null) throw new ArgumentNullException(nameof(allManifestsById));

        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        foreach (KeyValuePair<string, ModManifest> kvp in allManifestsById)
        {
            ModManifest manifest = kvp.Value;
            foreach (ModDependency dep in manifest.Dependencies)
            {
                if (allManifestsById.ContainsKey(dep.ModId))
                    continue;

                if (dep.IsOptional)
                {
                    warnings.Add(new ValidationWarning(
                        manifest.Id,
                        $"Optional dependency '{dep.ModId}' for mod '{manifest.Id}' " +
                        "is not present in the load batch; behavior may degrade."));
                }
                else
                {
                    errors.Add(new ValidationError(
                        manifest.Id,
                        ValidationErrorKind.MissingDependency,
                        $"Mod '{manifest.Id}' requires dependency '{dep.ModId}' " +
                        "which is not present in the load batch."));
                }
            }
        }

        return (errors, warnings);
    }

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
