using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Drives the menu-side editing session per MOD_OS_ARCHITECTURE v1.5
/// §9.2 / §9.6. Encapsulates the Pause-Toggle-Apply-Resume sequence so
/// the UI layer (M7.5.B) is purely presentational and the orchestration
/// logic stays unit-testable inside <c>DualFrontier.Application</c>.
///
/// Lifecycle: <see cref="BeginEditing"/> → 0..N <see cref="Toggle"/>
/// calls → either <see cref="Commit"/> or <see cref="Cancel"/>.
/// <see cref="Commit"/> on a clean session resumes the simulation; a
/// failed commit (validation error from
/// <see cref="ModIntegrationPipeline.Apply"/>) leaves the session open
/// and the simulation paused so the user can fix the pending state and
/// retry. This failure-path semantics is a deliberate interpretation of
/// §9.2 (which only specifies the success path) registered as M7.5.A
/// AD #4 — see the ROADMAP M7.5.A closure entry footer parallel to
/// M7.1's §9.2/§9.3 footer and M7.3's §9.5 step 7 ordering footer.
///
/// §9.6 enforcement — a mod with <c>hotReload: false</c> that is
/// currently active CANNOT be removed during the session
/// (<see cref="CanToggle"/> returns <see langword="false"/>;
/// <see cref="Toggle"/> returns
/// <see cref="ToggleResult.RejectedHotReloadDisabled"/>). A mod with
/// <c>hotReload: false</c> that is NOT currently active CAN be added —
/// per AD #5, the §9.6 wording "cannot be reloaded mid-session" plus
/// §2.2 "loads only at session start" treats first-load as not a
/// reload. On first launch the user adds it; subsequent sessions it
/// stays until restart.
/// </summary>
internal sealed class ModMenuController
{
    private readonly ModIntegrationPipeline _pipeline;
    private readonly IModDiscoverer _discoverer;

    private bool _isEditing;
    private HashSet<string>? _pendingActiveIds;
    private Dictionary<string, ActiveModInfo>? _sessionStartActive;
    private Dictionary<string, DiscoveredModInfo>? _sessionDiscovered;

    /// <summary>
    /// Optional hook fired by <see cref="BeginEditing"/> AFTER the pipeline
    /// is paused, on the transition from "not editing" to "editing".
    /// Idempotent re-entry (a second BeginEditing call while already
    /// editing) does NOT re-fire the hook — the no-op early return path
    /// skips both the pipeline call and the hook.
    ///
    /// Wired by <see cref="Loop.GameBootstrap.CreateLoop"/> to also pause
    /// the background simulation thread (<c>GameLoop.SetPaused(true)</c>)
    /// per MOD_OS_ARCHITECTURE §9.2 step 1, since
    /// <see cref="ModIntegrationPipeline.Pause"/> only gates the
    /// Apply-mutation safety flag and does not affect tick advance.
    ///
    /// Hook exceptions are caught and swallowed inside the controller —
    /// hook failures must not prevent the menu lifecycle from completing.
    /// Tests not exercising the simulation pause leave the field
    /// <see langword="null"/>; the null path is a no-op.
    /// </summary>
    internal Action? OnEditingBegan { get; set; }

    /// <summary>
    /// Optional hook fired by <see cref="Cancel"/> and by the
    /// success-path branch of <see cref="Commit"/>, AFTER the pipeline is
    /// resumed, on the transition from "editing" to "not editing". The
    /// failure-path branch of <see cref="Commit"/> does NOT fire this
    /// hook — failed commit leaves the session open per AD #4 of M7.5.A,
    /// and the simulation must stay paused so the user can fix the
    /// pending state and retry.
    ///
    /// Symmetric counterpart to <see cref="OnEditingBegan"/>; same
    /// swallow-exceptions discipline.
    /// </summary>
    internal Action? OnEditingEnded { get; set; }

    public ModMenuController(
        ModIntegrationPipeline pipeline,
        IModDiscoverer discoverer)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        _discoverer = discoverer ?? throw new ArgumentNullException(nameof(discoverer));
    }

    /// <summary>
    /// True iff <see cref="BeginEditing"/> has been called and neither
    /// <see cref="Commit"/> (on its success path) nor <see cref="Cancel"/>
    /// has closed the session. Surfaced for the menu layer (M7.5.B) to
    /// gate UI affordances on session presence.
    /// </summary>
    public bool IsEditing => _isEditing;

    /// <summary>
    /// Snapshots the pipeline's active set into the pending mutable set
    /// and pauses the simulation per §9.2 step 1. Idempotent per AD #6
    /// — calling while already editing is a silent no-op so the menu
    /// can re-enter Begin from either Cancel or fresh construction
    /// without special casing.
    /// </summary>
    public void BeginEditing()
    {
        if (_isEditing) return;
        _pipeline.Pause();

        IReadOnlyList<ActiveModInfo> active = _pipeline.GetActiveMods();
        _sessionStartActive = new Dictionary<string, ActiveModInfo>(StringComparer.Ordinal);
        foreach (ActiveModInfo info in active)
            _sessionStartActive[info.ModId] = info;

        IReadOnlyList<DiscoveredModInfo> discovered = _discoverer.Discover();
        _sessionDiscovered = new Dictionary<string, DiscoveredModInfo>(StringComparer.Ordinal);
        foreach (DiscoveredModInfo info in discovered)
            _sessionDiscovered[info.Manifest.Id] = info;

        _pendingActiveIds = new HashSet<string>(_sessionStartActive.Keys, StringComparer.Ordinal);
        _isEditing = true;
        RaiseHook(OnEditingBegan);
    }

    /// <summary>
    /// Discards the pending state and resumes the simulation per §9.2.
    /// Idempotent per AD #6 — calling while not editing is a silent
    /// no-op (and importantly does NOT call
    /// <see cref="ModIntegrationPipeline.Resume"/>, so a stray Cancel
    /// from the UI on a fresh controller cannot accidentally start the
    /// scheduler).
    /// </summary>
    public void Cancel()
    {
        if (!_isEditing) return;
        _pendingActiveIds = null;
        _sessionStartActive = null;
        _sessionDiscovered = null;
        _isEditing = false;
        _pipeline.Resume();
        RaiseHook(OnEditingEnded);
    }

    /// <summary>
    /// Mutates the pending set: adds the mod if absent, removes it if
    /// present. Defensive §9.6 check — a currently-active mod with
    /// <c>hotReload: false</c> is rejected with
    /// <see cref="ToggleResult.RejectedHotReloadDisabled"/> regardless
    /// of whether the UI greyed out the button via
    /// <see cref="CanToggle"/>. Unknown modIds (neither active nor
    /// discovered) return <see cref="ToggleResult.UnknownMod"/>; calls
    /// without an active session return
    /// <see cref="ToggleResult.NoSession"/>.
    /// </summary>
    public ToggleResult Toggle(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (!_isEditing) return ToggleResult.NoSession;

        if (_sessionStartActive!.TryGetValue(modId, out ActiveModInfo? active)
            && !active.Manifest.HotReload)
        {
            return ToggleResult.RejectedHotReloadDisabled;
        }

        bool knownActive = _sessionStartActive.ContainsKey(modId);
        bool knownDiscovered = _sessionDiscovered!.ContainsKey(modId);
        if (!knownActive && !knownDiscovered)
            return ToggleResult.UnknownMod;

        if (_pendingActiveIds!.Contains(modId))
            _pendingActiveIds.Remove(modId);
        else
            _pendingActiveIds.Add(modId);

        return ToggleResult.Toggled;
    }

    /// <summary>
    /// UI hint mirroring <see cref="Toggle"/>'s §9.6 rejection rule.
    /// Returns <see langword="false"/> for currently-active mods with
    /// <c>hotReload: false</c>, and for any unknown modId when a
    /// session is open. Returns <see langword="false"/> outside an
    /// active session as a defensive default.
    /// </summary>
    public bool CanToggle(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (!_isEditing) return false;
        if (_sessionStartActive!.TryGetValue(modId, out ActiveModInfo? active)
            && !active.Manifest.HotReload)
        {
            return false;
        }
        return _sessionStartActive.ContainsKey(modId)
            || _sessionDiscovered!.ContainsKey(modId);
    }

    /// <summary>
    /// Returns one <see cref="EditableModInfo"/> row per mod the menu
    /// should render. Active mods come first (so the "active" rows
    /// render at the top by default — UI may re-sort), then the
    /// discovered-only mods. Throws when called without an active
    /// session — rendering rows without a Begin would be meaningless.
    /// </summary>
    public IReadOnlyList<EditableModInfo> GetEditableState()
    {
        if (!_isEditing)
            throw new InvalidOperationException(
                "GetEditableState requires an active editing session — call BeginEditing first");

        var result = new List<EditableModInfo>();
        var emitted = new HashSet<string>(StringComparer.Ordinal);

        foreach (KeyValuePair<string, ActiveModInfo> kv in _sessionStartActive!)
        {
            result.Add(BuildRow(kv.Key, kv.Value.Manifest));
            emitted.Add(kv.Key);
        }
        foreach (KeyValuePair<string, DiscoveredModInfo> kv in _sessionDiscovered!)
        {
            if (emitted.Contains(kv.Key)) continue;
            result.Add(BuildRow(kv.Key, kv.Value.Manifest));
        }
        return result;
    }

    /// <summary>
    /// Computes the diff between the session-start active set and the
    /// pending set, then drives the §9.2 commit sequence: per-removed
    /// mod <see cref="ModIntegrationPipeline.UnloadMod"/>, then a
    /// single <see cref="ModIntegrationPipeline.Apply"/> for added
    /// paths. On success the simulation is resumed and the session is
    /// closed; on failure (validation error from <c>Apply</c>) the
    /// session stays open and the simulation stays paused so the user
    /// can fix the pending state and retry — see AD #4 in the class
    /// XML-doc and the ROADMAP M7.5.A closure footer.
    /// </summary>
    public CommitResult Commit()
    {
        if (!_isEditing)
            throw new InvalidOperationException(
                "Commit requires an active editing session — call BeginEditing first");

        var pending = _pendingActiveIds!;
        var removed = new List<string>();
        foreach (string id in _sessionStartActive!.Keys)
            if (!pending.Contains(id)) removed.Add(id);

        var addedPaths = new List<string>();
        foreach (string id in pending)
        {
            if (_sessionStartActive.ContainsKey(id)) continue;
            if (_sessionDiscovered!.TryGetValue(id, out DiscoveredModInfo? info))
                addedPaths.Add(info.Path);
            // (id not in discovered would mean Toggle bypassed its
            //  UnknownMod gate — defensively skip rather than throw.)
        }

        var aggregatedWarnings = new List<ValidationWarning>();

        // Step 1 — unload removed mods (best-effort per §9.5.1).
        foreach (string id in removed)
        {
            IReadOnlyList<ValidationWarning> w = _pipeline.UnloadMod(id);
            foreach (ValidationWarning entry in w) aggregatedWarnings.Add(entry);
        }

        // Step 2 — apply added mods. If empty, skip Apply entirely so
        // the no-op path doesn't churn the scheduler unnecessarily.
        IReadOnlyList<ValidationError> errors = Array.Empty<ValidationError>();
        IReadOnlyList<string> newlyActive = Array.Empty<string>();
        bool applySucceeded = true;
        if (addedPaths.Count > 0)
        {
            PipelineResult result = _pipeline.Apply(addedPaths);
            errors = result.Errors;
            newlyActive = result.LoadedModIds;
            foreach (ValidationWarning w in result.Warnings) aggregatedWarnings.Add(w);
            applySucceeded = result.Success;
        }

        if (applySucceeded)
        {
            _pendingActiveIds = null;
            _sessionStartActive = null;
            _sessionDiscovered = null;
            _isEditing = false;
            _pipeline.Resume();
            RaiseHook(OnEditingEnded);
            return new CommitResult(
                Success: true,
                Errors: Array.Empty<ValidationError>(),
                Warnings: aggregatedWarnings,
                NewlyActiveModIds: newlyActive,
                NewlyInactiveModIds: removed);
        }

        // Failure path (AD #4) — leave session open, simulation paused.
        return new CommitResult(
            Success: false,
            Errors: errors,
            Warnings: aggregatedWarnings,
            NewlyActiveModIds: Array.Empty<string>(),
            NewlyInactiveModIds: removed);
    }

    private EditableModInfo BuildRow(string modId, ModManifest manifest)
    {
        bool currentlyActive = _sessionStartActive!.ContainsKey(modId);
        bool pendingActive = _pendingActiveIds!.Contains(modId);
        bool canToggle = !(currentlyActive && !manifest.HotReload);
        return new EditableModInfo(
            modId, manifest, currentlyActive, pendingActive, canToggle);
    }

    private static void RaiseHook(Action? hook)
    {
        if (hook is null) return;
        try { hook(); }
        catch { /* §9.5.1-style swallow: lifecycle cannot be derailed by hook callbacks. */ }
    }
}
