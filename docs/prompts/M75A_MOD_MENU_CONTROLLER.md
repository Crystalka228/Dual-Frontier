# M7.5.A — `ModMenuController` + pipeline read API

## Context

M3, M4, M5, M6 closed cleanly with closure verification reports. M7.1 + M7.2 + M7.3 + M7.4 closed (commits `a2ab761`, `c964475`, `2531ed7`, `d68ba93`, `c3f5251`, `9bed1a4`, `46b4f33`, `1d43858`, `5385fe5`, `8d51a9d`, `4ff04f8`). 378/378 tests passing. Working tree clean. Standalone CODING_STANDARDS update closed (commit `f4b2cb8`) — M7.3's display-class hoisting finding persisted.

`MOD_OS_ARCHITECTURE.md` LOCKED v1.5. §9.2 specifies the menu-driven hot-reload flow: user opens menu → menu calls `Pipeline.Pause()` → user toggles mods, edits versions, clicks Apply → menu invokes `Pipeline.Apply(newModSet)` → on success menu calls `Pipeline.Resume()`. §9.6 specifies that mods with `hotReload: false` cannot be reloaded mid-session — the menu disables the reload button and presents a tooltip.

Approved decomposition per METHODOLOGY §2.4 and ratified during pre-flight: M7.5 split into **M7.5.A** ← this session (controller logic, pure C#, fully unit-tested) and **M7.5.B** (Godot UI binding, manual verification). M7-closure remains a separate post-M7.5.B session, parallel pattern to M5/M6 closure precedent.

M7.5.A scope: ship `ModMenuController` in `DualFrontier.Application/Modding/` encapsulating the editing-session model (Begin → Toggle → Commit/Cancel) defined by §9.2, plus the supporting `IModDiscoverer` abstraction for available-mod discovery, plus the new public `Pipeline.GetActiveMods()` query method. Full unit-test coverage. M7.5.B will bind a Godot scene to this controller; bootstrap wiring (exposing controller from `GameBootstrap` to `GameRoot`) is M7.5.B territory.

## Out of scope (M7.5.B / M7-closure / M8 will do — NOT in this session)

- Godot UI scene + bindings (`ModMenuPanel`, button widgets, hot-reload disabled tooltip rendering) — M7.5.B.
- Bootstrap wiring of controller into `GameBootstrap` / `GameRoot` — M7.5.B.
- M7-closure session — separate post-M7.5.B session with full M7 verification report.
- Vanilla mod skeletons that the menu would actually toggle in production — M8.
- Modifications to `DualFrontier.Core` or `DualFrontier.Contracts` — M-phase boundary discipline preserved through M3–M7.4. Verified via `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returning empty.
- Save-game compatibility policy when a mod is missing (D-6 LOCKED) — separate concern targeting the persistence layer, out of M0–M10 scope per ROADMAP.

## Approved architectural decisions

1. **Discovery scope: controller composes `IModDiscoverer`.** The pipeline stays storage-agnostic. `IModDiscoverer.Discover() : IReadOnlyList<DiscoveredModInfo>` is the abstraction; `DefaultModDiscoverer` is the production implementation that walks a configured root directory (defaults to `mods/`). Tests inject a `FakeModDiscoverer` returning a hand-rolled list. M8+ may swap for embedded vanilla list, Steam Workshop adapter, etc., without touching the pipeline.

2. **Pipeline read-API: new public `GetActiveMods()` returning `IReadOnlyList<ActiveModInfo>`.** `ActiveModInfo` is a public record with `ModId` (string) and `Manifest` (existing public `ModManifest`). Other pipeline surface unchanged. The pipeline's `internal sealed class` declaration stays — `GetActiveMods` is the only new public method; the controller composes it via `internal` access (both live in `DualFrontier.Application`).

3. **Edit session model: stateful controller with explicit lifecycle.** `BeginEditing()` snapshots the current active set into a pending mutable set and calls `Pipeline.Pause()`. `Toggle(modId)` mutates only the pending set, never the pipeline. `Commit()` computes the diff (added paths, removed ids), calls `Pipeline.UnloadMod(id)` per removed, calls `Pipeline.Apply(addedPaths)` if any added, calls `Pipeline.Resume()` only on full success, returns aggregated `CommitResult`. `Cancel()` discards pending state and calls `Pipeline.Resume()`. This matches §9.2's wording — multiple mutations between Pause and Apply, single explicit commit.

4. **Failure-path semantics on `Commit()`: simulation stays paused on validation failure.** When `Pipeline.Apply` returns `Success: false`, controller does NOT call `Resume()`. The menu (M7.5.B) keeps the editing session open with errors displayed; user fixes pending state and re-invokes `Commit()`. Only successful `Commit()` and any `Cancel()` call `Resume()`. This is a deliberate interpretation of §9.2 (which only specifies the success path) registered as a deliberate-interpretation paragraph in the ROADMAP M7.5.A closure entry, parallel pattern to M7.1's §9.2/§9.3 footer and M7.3's §9.5 step 7 ordering footer.

5. **Hot-reload disabled (`hotReload: false`) semantics per §9.6: applies only to currently-active mods.** A mod with `hotReload: false` that is currently active CANNOT be removed during the session — `CanToggle(modId)` returns `false`, `Toggle(modId)` returns `RejectedHotReloadDisabled`. A mod with `hotReload: false` that is NOT currently active CAN be added (the restriction is on reload, not initial load — first-load doesn't count as reload). On first launch the user adds it; subsequent sessions it stays until restart. This is consistent with §9.6 ("cannot be reloaded mid-session") and §2.2 ("loads only at session start; menu refuses to reload it").

6. **Idempotency.** `BeginEditing()` while already editing — no-op (returns silently). `Cancel()` while not editing — no-op. `Commit()` while not editing — throws `InvalidOperationException("No active editing session")`. `Toggle()` while not editing — returns `ToggleResult.NoSession`, no mutation. These match the menu-flow assumption that the UI is the sole driver and does not double-call.

7. **No public access to `LoadedMod`.** `LoadedMod` is `internal sealed record` per existing isolation. `ActiveModInfo` is the public surface — narrow DTO carrying just the modId and manifest. Controller methods that need richer access stay internal-friendly; public API exposes only the narrow DTO.

8. **One class per file (per CODING_STANDARDS).** `ModMenuController`, `IModDiscoverer`, `DefaultModDiscoverer`, `EditableModInfo`, `CommitResult`, `ToggleResult`, `DiscoveredModInfo`, `ActiveModInfo` each live in their own file under `src/DualFrontier.Application/Modding/`.

9. **Atomic phase review per METHODOLOGY §2.4** — implementation, tests, ROADMAP closure all in one session. Three-commit invariant per §7.3.

## Required reading

1. `docs/MOD_OS_ARCHITECTURE.md` LOCKED v1.5 — §9.1 (lifecycle states), §9.2 (menu-driven hot-reload flow, the canonical pause-toggle-apply-resume sequence), §9.3 (no live-tick reload, run-flag guard wording), §9.5 / §9.5.1 (unload chain — controller calls `UnloadMod` per removed mod, accumulates warnings), §9.6 (hot-reload disabled semantics — applies to currently-active mods), §2.2 (`hotReload` field default and effect), §11.2 (no new error kinds for M7.5.A).
2. `docs/ROADMAP.md` — M7 sub-phase status block, M7.5 entry "Mod-menu UI integration" (will be replaced with M7.5.A + M7.5.B sub-entries by commit 3 of this session), M5/M6 closure entry pattern as decomposition reference.
3. `docs/METHODOLOGY.md` — §2.4 atomic phase review, §7.3 three-commit invariant.
4. `docs/CODING_STANDARDS.md` — full document. Especially: one class per file, English-only comments, member order, `_camelCase` private fields, **Stack-frame retention** section (added in commit `f4b2cb8` — not directly relevant to M7.5.A since no GC-collection code paths, but locks the broader discipline for any future helper extraction).
5. Code (full files):
   - `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` — controller composes `Pause`/`Resume`/`Apply`/`UnloadMod` directly. Read the public surface and the `_activeMods` field. The new `GetActiveMods` method extends this surface.
   - `src/DualFrontier.Application/Modding/ModLoader.cs` — `ReadManifestFromDirectory(path)` is the manifest-parsing primitive `DefaultModDiscoverer` uses.
   - `src/DualFrontier.Application/Modding/LoadedMod.cs` — `internal sealed record` shape; controller never returns it directly, only the public `ActiveModInfo` DTO.
   - `src/DualFrontier.Contracts/Modding/ModManifest.cs` — public type referenced by both `ActiveModInfo` and `DiscoveredModInfo`.
   - `src/DualFrontier.Contracts/Modding/ValidationError.cs` and `ValidationWarning` — surfaces returned by `CommitResult`.
6. Existing test patterns:
   - `tests/DualFrontier.Modding.Tests/Pipeline/M71PauseResumeTests.cs` — minimal `BuildPipeline` harness; controller tests can reuse this shape.
   - `tests/DualFrontier.Modding.Tests/Pipeline/M72UnloadChainTests.cs` — `Harness` with `RegisterLoaded` test seam pattern; ModMenuControllerTests can reuse it for in-memory mod injection without filesystem dependency.
   - `tests/DualFrontier.Modding.Tests/Pipeline/M73Phase2DebtTests.cs` — real-mod fixture pattern via `pipeline.Apply([fixturePath])`; reference for any test that needs disk-based mod loading.
7. Existing test fixture mods — `tests/Fixture.RegularMod_DependedOn/`, `tests/Fixture.RegularMod_ReplacesCombat/`, etc. M7.5.A tests use in-memory injected mods (faster); the fixtures are referenced by the discoverer test only.

## Implementation

### 1. Pipeline `GetActiveMods()` extension

Add to `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`. Place near `IsRunning` getter (other public read-only state):

```csharp
/// <summary>
/// Snapshot of every regular mod currently in the active set. Returned
/// as <see cref="ActiveModInfo"/> records carrying only public fields
/// (modId + manifest) — <see cref="LoadedMod"/> stays internal so callers
/// outside <c>DualFrontier.Application</c> cannot reach the
/// <see cref="ModLoadContext"/> or other implementation surfaces.
/// Returned list is a fresh allocation; callers may iterate without
/// concern for concurrent mutation, but the list itself is not live —
/// a subsequent <see cref="Apply"/> or <see cref="UnloadMod"/> won't be
/// reflected. Used by <see cref="ModMenuController"/> to build the
/// editing-session snapshot.
/// </summary>
public IReadOnlyList<ActiveModInfo> GetActiveMods()
{
    var result = new List<ActiveModInfo>(_activeMods.Count);
    foreach (LoadedMod mod in _activeMods)
        result.Add(new ActiveModInfo(mod.ModId, mod.Manifest));
    return result;
}
```

### 2. Public DTOs

`src/DualFrontier.Application/Modding/ActiveModInfo.cs`:

```csharp
namespace DualFrontier.Application.Modding;

/// <summary>
/// Public snapshot of an active mod. Carries only fields safe for
/// non-Application callers (the controller surface returned by
/// <see cref="ModIntegrationPipeline.GetActiveMods"/>).
/// </summary>
public sealed record ActiveModInfo(string ModId, ModManifest Manifest);
```

`src/DualFrontier.Application/Modding/DiscoveredModInfo.cs`:

```csharp
namespace DualFrontier.Application.Modding;

/// <summary>
/// Public record returned by <see cref="IModDiscoverer.Discover"/>.
/// Carries the on-disk path so the controller can pass it to
/// <see cref="ModIntegrationPipeline.Apply"/>, plus the parsed manifest
/// so the menu can display name / version / hotReload flag without
/// re-parsing.
/// </summary>
public sealed record DiscoveredModInfo(string Path, ModManifest Manifest);
```

`src/DualFrontier.Application/Modding/EditableModInfo.cs`:

```csharp
namespace DualFrontier.Application.Modding;

/// <summary>
/// Per-mod row in the editing session, returned by
/// <see cref="ModMenuController.GetEditableState"/>. Combines the
/// session-start active-set snapshot with the current pending state
/// and the §9.6 hot-reload restriction so the UI can render rows
/// without computing flags itself.
/// </summary>
/// <param name="ModId">Manifest id.</param>
/// <param name="Manifest">Parsed manifest.</param>
/// <param name="IsCurrentlyActive">
/// True iff the mod was active at <see cref="ModMenuController.BeginEditing"/>
/// time. Stable across the editing session.
/// </param>
/// <param name="IsPendingActive">
/// True iff the mod is in the pending active set (will be active after
/// a successful <see cref="ModMenuController.Commit"/>). Mutates as the
/// user toggles.
/// </param>
/// <param name="CanToggle">
/// False when §9.6 forbids the toggle: a currently-active mod with
/// <see cref="ModManifest.HotReload"/> = false. The UI greys out the
/// toggle button and shows the "restart required" tooltip when this is
/// false. <see cref="ModMenuController.Toggle"/> additionally rejects
/// the call defensively.
/// </param>
public sealed record EditableModInfo(
    string ModId,
    ModManifest Manifest,
    bool IsCurrentlyActive,
    bool IsPendingActive,
    bool CanToggle);
```

`src/DualFrontier.Application/Modding/CommitResult.cs`:

```csharp
namespace DualFrontier.Application.Modding;

/// <summary>
/// Outcome of <see cref="ModMenuController.Commit"/>. Aggregates results
/// across the per-removed-mod <c>UnloadMod</c> calls and the single
/// <c>Apply</c> call for added mods. On success the simulation has been
/// resumed; on failure it stays paused so the user can fix and retry
/// (per the M7.5.A AD #4 deliberate interpretation of §9.2).
/// </summary>
/// <param name="Success">
/// True iff every <c>UnloadMod</c> step succeeded with zero warnings AND
/// <c>Apply</c> returned <c>Success: true</c>. Note: per §9.5.1 best-
/// effort, an <c>UnloadMod</c> warning does not by itself fail the
/// commit — only an <c>Apply</c> validation error (or no Apply at all
/// when only removals occurred) flips this to false.
/// </param>
/// <param name="Errors">
/// Validation errors from <c>Apply</c> — empty on success. Empty when
/// commit only had removals (no Apply call).
/// </param>
/// <param name="Warnings">
/// Aggregated <see cref="ValidationWarning"/> entries from every per-mod
/// <c>UnloadMod</c> plus <c>Apply</c>'s own warnings list. Best-effort
/// signals that should surface to the user but do not block.
/// </param>
/// <param name="NewlyActiveModIds">
/// Mods that became active as a result of this commit (from
/// <c>Apply</c>'s <c>LoadedModIds</c>).
/// </param>
/// <param name="NewlyInactiveModIds">
/// Mods that became inactive as a result of this commit (the controller
/// records each <c>UnloadMod</c> id; the list is unchanged regardless of
/// per-mod warnings since the mod is removed from the active set
/// regardless per §9.5.1).
/// </param>
public sealed record CommitResult(
    bool Success,
    IReadOnlyList<ValidationError> Errors,
    IReadOnlyList<ValidationWarning> Warnings,
    IReadOnlyList<string> NewlyActiveModIds,
    IReadOnlyList<string> NewlyInactiveModIds);
```

`src/DualFrontier.Application/Modding/ToggleResult.cs`:

```csharp
namespace DualFrontier.Application.Modding;

/// <summary>
/// Outcome of <see cref="ModMenuController.Toggle"/>.
/// </summary>
public enum ToggleResult
{
    /// <summary>Pending set was mutated.</summary>
    Toggled,

    /// <summary>
    /// The mod is currently active and has <c>hotReload: false</c>;
    /// per §9.6 it cannot be toggled mid-session. UI should grey out
    /// the button and show the restart-required tooltip.
    /// </summary>
    RejectedHotReloadDisabled,

    /// <summary>
    /// <see cref="ModMenuController.BeginEditing"/> has not been called
    /// (or <see cref="ModMenuController.Cancel"/> /
    /// <see cref="ModMenuController.Commit"/> has closed the session).
    /// </summary>
    NoSession,

    /// <summary>
    /// Unknown modId — neither in the active set nor in the discovered
    /// set. Defensive guard for misrouted UI calls.
    /// </summary>
    UnknownMod,
}
```

### 3. `IModDiscoverer` abstraction

`src/DualFrontier.Application/Modding/IModDiscoverer.cs`:

```csharp
namespace DualFrontier.Application.Modding;

/// <summary>
/// Locates available mods for the editing session. Implementations may
/// scan a directory tree (<see cref="DefaultModDiscoverer"/>), enumerate
/// embedded vanilla manifests, query a Steam Workshop adapter, etc.
/// The contract is purely about producing a list — composition,
/// dependency resolution, and load-order all happen later inside
/// <see cref="ModIntegrationPipeline.Apply"/>.
/// </summary>
public interface IModDiscoverer
{
    /// <summary>
    /// Returns every mod the discoverer can locate, with the absolute
    /// or pipeline-acceptable path (the same string passed to
    /// <see cref="ModIntegrationPipeline.Apply"/>) and the parsed
    /// manifest. Order is implementation-defined; the controller
    /// re-orders as needed for display.
    /// </summary>
    IReadOnlyList<DiscoveredModInfo> Discover();
}
```

### 4. `DefaultModDiscoverer` implementation

`src/DualFrontier.Application/Modding/DefaultModDiscoverer.cs`:

```csharp
namespace DualFrontier.Application.Modding;

/// <summary>
/// Scans a configured root directory for subdirectories containing a
/// <c>mod.manifest.json</c>. Skips subdirectories without the manifest
/// file (build outputs, IDE artifacts, etc.). Catches per-mod parse
/// errors so a single broken manifest does not prevent the menu from
/// listing the rest.
/// </summary>
public sealed class DefaultModDiscoverer : IModDiscoverer
{
    private readonly string _rootPath;

    /// <summary>
    /// Creates a discoverer scanning <paramref name="rootPath"/>. The
    /// path is not validated at construction; <see cref="Discover"/>
    /// returns an empty list if the path does not exist (rather than
    /// throwing — first-launch with no <c>mods/</c> directory must be
    /// a clean empty-list case, not an exception).
    /// </summary>
    public DefaultModDiscoverer(string rootPath)
    {
        _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
    }

    public IReadOnlyList<DiscoveredModInfo> Discover()
    {
        if (!Directory.Exists(_rootPath))
            return Array.Empty<DiscoveredModInfo>();

        var result = new List<DiscoveredModInfo>();
        foreach (string dir in Directory.EnumerateDirectories(_rootPath))
        {
            string manifestPath = Path.Combine(dir, "mod.manifest.json");
            if (!File.Exists(manifestPath))
                continue;
            try
            {
                ModManifest manifest = ModLoader.ReadManifestFromDirectory(dir);
                result.Add(new DiscoveredModInfo(dir, manifest));
            }
            catch
            {
                // Per-mod parse failure — skip silently. M7.5.B may
                // surface this through a separate warning channel; for
                // M7.5.A the discoverer is best-effort enumeration.
            }
        }
        return result;
    }
}
```

`ModLoader.ReadManifestFromDirectory` is currently `internal static`. Verify it stays internal — `DefaultModDiscoverer` is in the same assembly, so internal access works without surface change.

### 5. `ModMenuController`

`src/DualFrontier.Application/Modding/ModMenuController.cs`:

```csharp
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
/// failed commit (validation error from <see cref="ModIntegrationPipeline.Apply"/>)
/// leaves the session open and the simulation paused so the user can
/// fix the pending state and retry (M7.5.A AD #4).
/// </summary>
public sealed class ModMenuController
{
    private readonly ModIntegrationPipeline _pipeline;
    private readonly IModDiscoverer _discoverer;

    private bool _isEditing;
    private HashSet<string>? _pendingActiveIds;
    private Dictionary<string, ActiveModInfo>? _sessionStartActive;
    private Dictionary<string, DiscoveredModInfo>? _sessionDiscovered;

    public ModMenuController(
        ModIntegrationPipeline pipeline,
        IModDiscoverer discoverer)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        _discoverer = discoverer ?? throw new ArgumentNullException(nameof(discoverer));
    }

    public bool IsEditing => _isEditing;

    public void BeginEditing()
    {
        if (_isEditing) return;  // idempotent per AD #6
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
    }

    public void Cancel()
    {
        if (!_isEditing) return;  // idempotent per AD #6
        _pendingActiveIds = null;
        _sessionStartActive = null;
        _sessionDiscovered = null;
        _isEditing = false;
        _pipeline.Resume();
    }

    public ToggleResult Toggle(string modId)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (!_isEditing) return ToggleResult.NoSession;

        // Defensive §9.6 check — UI should already grey out via CanToggle,
        // this rejects misrouted calls.
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

    public IReadOnlyList<EditableModInfo> GetEditableState()
    {
        if (!_isEditing)
            throw new InvalidOperationException(
                "GetEditableState requires an active editing session — call BeginEditing first");

        var result = new List<EditableModInfo>();
        var emitted = new HashSet<string>(StringComparer.Ordinal);

        // Active mods first (so the "active" rows render at the top of
        // the menu by default; UI can re-sort).
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

    public CommitResult Commit()
    {
        if (!_isEditing)
            throw new InvalidOperationException(
                "Commit requires an active editing session — call BeginEditing first");

        // Compute diff.
        var current = _sessionStartActive!.Keys.ToHashSet(StringComparer.Ordinal);
        var pending = _pendingActiveIds!;
        var removed = new List<string>();
        foreach (string id in current)
            if (!pending.Contains(id)) removed.Add(id);
        var addedPaths = new List<string>();
        foreach (string id in pending)
        {
            if (current.Contains(id)) continue;
            if (_sessionDiscovered!.TryGetValue(id, out DiscoveredModInfo? info))
                addedPaths.Add(info.Path);
            // (id not in discovered should be impossible — Toggle gated)
        }

        var aggregatedWarnings = new List<ValidationWarning>();

        // Step 1 — unload removed mods (best-effort per §9.5.1).
        foreach (string id in removed)
        {
            IReadOnlyList<ValidationWarning> w = _pipeline.UnloadMod(id);
            foreach (ValidationWarning entry in w) aggregatedWarnings.Add(entry);
        }

        // Step 2 — apply added mods. If empty, skip Apply entirely
        // (no-op apply with empty list also works but is wasteful).
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
            // Success path — close session, resume simulation.
            _pendingActiveIds = null;
            _sessionStartActive = null;
            _sessionDiscovered = null;
            _isEditing = false;
            _pipeline.Resume();
            return new CommitResult(
                Success: true,
                Errors: Array.Empty<ValidationError>(),
                Warnings: aggregatedWarnings,
                NewlyActiveModIds: newlyActive,
                NewlyInactiveModIds: removed);
        }

        // Failure path (AD #4) — leave session open, simulation paused,
        // user fixes and retries.
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
}
```

**Implementation note**: `current.Keys.ToHashSet(...)` requires `using System.Linq;` — verify present. The `_sessionStartActive`, `_sessionDiscovered`, `_pendingActiveIds` are nullable to enforce "only valid between Begin and Commit/Cancel"; the `!` operators are safe because every method that touches them gates on `_isEditing`.

## Tests

### `tests/DualFrontier.Modding.Tests/Menu/ModMenuControllerTests.cs`

Reuse the harness pattern from `M72UnloadChainTests` for an in-memory pipeline with `_loader.RegisterLoaded(...)` injection. Use a `FakeModDiscoverer` returning a hand-rolled list. Each test isolates its fixture.

1. **`BeginEditing_OnIdleController_PausesPipelineAndSetsIsEditing`** — fresh pipeline (default paused). `BeginEditing()` keeps it paused, sets `IsEditing = true`. Verify via `pipeline.IsRunning == false` (was already false; verify Pause was called via fact that controller is now in editing state).

2. **`BeginEditing_OnRunningPipeline_PausesPipeline`** — `pipeline.Resume()` first. `BeginEditing()`. `pipeline.IsRunning` should be false.

3. **`BeginEditing_Twice_Idempotent`** — two calls in a row, no exception, `IsEditing == true` at end. Pipeline state unchanged after second call.

4. **`Cancel_AfterBeginEditing_DiscardsPendingAndResumes`** — Begin, Toggle some mod, Cancel. `IsEditing == false`, `pipeline.IsRunning == true`. Subsequent `Toggle` returns `NoSession`.

5. **`Cancel_WithoutBeginEditing_NoOp`** — fresh controller, `Cancel()`. No exception. `IsEditing == false`, pipeline still paused.

6. **`Toggle_OnHotReloadFalseCurrentlyActiveMod_ReturnsRejected`** — fixture mod with `HotReload = false`, registered as active. Begin. `Toggle(modId)` returns `RejectedHotReloadDisabled`. `_pendingActiveIds` unchanged.

7. **`Toggle_OnHotReloadFalseNotActiveMod_AllowedSinceFirstLoadIsNotReload`** — fixture mod with `HotReload = false`, in discovered set but not active. Begin. `Toggle(modId)` returns `Toggled`. `_pendingActiveIds` now contains it. (Implements AD #5 — first-load doesn't count as reload.)

8. **`Toggle_OnHotReloadTrueMod_TogglesPendingSet`** — toggle adds when absent, removes when present. Two consecutive `Toggle` calls revert to original.

9. **`Toggle_WithoutBeginEditing_ReturnsNoSession`** — fresh controller. `Toggle("anything")` returns `NoSession`.

10. **`Toggle_OnUnknownModId_ReturnsUnknownMod`** — Begin with empty fixtures. `Toggle("nonexistent.mod")` returns `UnknownMod`.

11. **`CanToggle_ForHotReloadFalseCurrentlyActive_ReturnsFalse`** — same fixture as test 6. Begin. `CanToggle(modId)` returns false.

12. **`CanToggle_ForHotReloadFalseDiscoveredOnly_ReturnsTrue`** — same as test 7. `CanToggle(modId)` returns true.

13. **`CanToggle_WithoutSession_ReturnsFalse`** — fresh controller. `CanToggle("anything")` returns false (defensive).

14. **`GetEditableState_ReturnsCombinedActiveAndDiscovered_WithFlags`** — fixture: 1 active mod (hotReload=true), 1 discovered-only mod (hotReload=true). Begin. `GetEditableState()` returns 2 rows: active row has `IsCurrentlyActive=true`, `IsPendingActive=true`, `CanToggle=true`; discovered row has `IsCurrentlyActive=false`, `IsPendingActive=false`, `CanToggle=true`.

15. **`GetEditableState_WithoutSession_Throws`** — fresh controller. `GetEditableState()` throws `InvalidOperationException`.

16. **`Commit_NoChanges_NoOpSuccess_Resumes`** — Begin, no Toggle, Commit. Returns `Success=true`, empty errors/warnings, empty NewlyActive/Inactive. `pipeline.IsRunning == true`. `IsEditing == false`.

17. **`Commit_AddOnly_CallsApply_NoUnload_Resumes`** — Begin, Toggle one discovered mod (adding it to pending), Commit. `pipeline.Apply` called with that mod's path; no `UnloadMod` calls. Result `Success=true`, `NewlyActiveModIds` contains the mod, `NewlyInactiveModIds` empty. `pipeline.IsRunning == true`.

18. **`Commit_RemoveOnly_CallsUnloadMod_NoApply_Resumes`** — Begin with 1 active mod. Toggle it (removing from pending). Commit. `UnloadMod` called once; `Apply` not called. Result `Success=true`, `NewlyActiveModIds` empty, `NewlyInactiveModIds` contains the mod.

19. **`Commit_AddAndRemove_BothCalled_WarningsAccumulated`** — Begin with 1 active mod (id=A) + 1 discovered-only mod (id=B). Toggle A (removing). Toggle B (adding). Commit. `UnloadMod(A)` called → `Apply([Bpath])` called. Both surface in result. Pipeline.IsRunning == true on success.

20. **`Commit_ApplyValidationFailure_StaysPaused_SessionOpen_ReturnsErrors`** — fixture with discovered mod that will trigger Apply validation error (e.g. missing required dependency). Begin, Toggle to add it, Commit. Returns `Success=false`, `Errors` populated. `pipeline.IsRunning == false` (still paused). `IsEditing == true` (session still open). User can Toggle again and re-Commit.

21. **`Commit_WithoutBeginEditing_Throws`** — fresh controller. `Commit()` throws `InvalidOperationException`.

22. **`Commit_PostFailureRetry_AfterFixingPending_Succeeds`** — same setup as test 20. After Commit returns failure, Toggle to remove the offending mod from pending. Commit again. `Success=true`. (Locks the recovery flow.)

### `tests/DualFrontier.Modding.Tests/Menu/DefaultModDiscovererTests.cs`

23. **`Discover_OnNonexistentRoot_ReturnsEmpty`** — discoverer with path that doesn't exist. `Discover()` returns empty list, no exception.

24. **`Discover_SkipsDirectoriesWithoutManifest`** — temp dir with subdir containing no `mod.manifest.json`. `Discover()` returns empty list.

25. **`Discover_OnValidFixtureRoot_ReturnsParsedManifests`** — point at a temp dir containing one valid mod (manifest + minimal contents). `Discover()` returns 1 entry with the parsed manifest.

26. **`Discover_HandlesInvalidManifestGracefully_SkipsBrokenButReturnsValid`** — temp dir with one valid mod and one broken (corrupt JSON) mod. `Discover()` returns 1 entry (the valid one); broken silently skipped per AD's best-effort enumeration.

### `tests/DualFrontier.Modding.Tests/Pipeline/PipelineGetActiveModsTests.cs`

27. **`GetActiveMods_OnFreshPipeline_ReturnsEmpty`** — new pipeline, `GetActiveMods()` returns empty list.

28. **`GetActiveMods_AfterApply_ContainsLoadedMod`** — pipeline.Apply with a fixture, `GetActiveMods()` returns 1 entry with the modId and manifest.

29. **`GetActiveMods_AfterUnloadMod_NoLongerContainsRemoved`** — Apply, then UnloadMod, then GetActiveMods returns empty.

30. **`GetActiveMods_ReturnsFreshList_NotLiveView`** — capture list reference, then Apply another mod, original captured list unchanged.

### Out-of-scope tests (M7.5.B / M8 / future will add)

- Godot UI scene smoke tests — M7.5.B.
- Real disk discovery against `mods/DualFrontier.Mod.Vanilla.*` directories — M8 (when real vanilla mods exist).
- Save-game policy tests — D-6 implementation (out of M0–M10).

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors. New types in `DualFrontier.Application/Modding/`.
2. `dotnet test` — 378 existing pass; 30 new pass (22 controller + 4 discoverer + 4 pipeline). Expected total: **408/408**.
3. New types exist with one-class-per-file: `ModMenuController`, `IModDiscoverer`, `DefaultModDiscoverer`, `EditableModInfo`, `CommitResult`, `ToggleResult`, `DiscoveredModInfo`, `ActiveModInfo`. All XML-doc'd in English per CODING_STANDARDS.
4. `ModIntegrationPipeline.GetActiveMods()` exists as new public method, returns fresh `IReadOnlyList<ActiveModInfo>` per call. Other pipeline surface unchanged (verified by M7.1–M7.4 regression).
5. `ModMenuController` lifecycle: BeginEditing → 0..N Toggle → Commit/Cancel; idempotent BeginEditing/Cancel; Commit/GetEditableState throw without active session.
6. §9.6 enforcement on hot-reload-disabled currently-active mods: both `Toggle` (defensive) and `CanToggle` (UI hint) return the right values per tests 6, 7, 11, 12.
7. AD #4 deliberate interpretation: failed Commit leaves simulation paused and session open (test 20); successful retry after fix resumes (test 22).
8. M7.1 + M7.2 + M7.3 + M7.4 regression guards still pass: `M71PauseResumeTests` (11) + `M72UnloadChainTests` (13) + `M73Step7Tests` (5) + `M73Phase2DebtTests` (2) + `ManifestRewriterTests` (7) + `M74BuildPipelineTests` (2) all green.
9. `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty (M-phase boundary discipline preserved through M7.5.A).
10. `dotnet sln list` unchanged — no new projects added (M7.5.A is pure additions to existing `DualFrontier.Application` + `DualFrontier.Modding.Tests`).

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `feat(modding): ModMenuController + IModDiscoverer + Pipeline.GetActiveMods`

- New files in `src/DualFrontier.Application/Modding/`: `ActiveModInfo.cs`, `DiscoveredModInfo.cs`, `EditableModInfo.cs`, `CommitResult.cs`, `ToggleResult.cs`, `IModDiscoverer.cs`, `DefaultModDiscoverer.cs`, `ModMenuController.cs`.
- Modify `ModIntegrationPipeline.cs`: add `GetActiveMods()` public method + XML-doc.
- No test changes — verify behaviour through existing M7.1–M7.4 suites which must still pass (`dotnet test --filter "FullyQualifiedName~M71|FullyQualifiedName~M72|FullyQualifiedName~M73|FullyQualifiedName~M74|FullyQualifiedName~ManifestRewriter"`).

**2.** `test(modding): ModMenuController + DefaultModDiscoverer + Pipeline.GetActiveMods coverage`

- New files in `tests/DualFrontier.Modding.Tests/Menu/`: `ModMenuControllerTests.cs` (22 tests), `DefaultModDiscovererTests.cs` (4 tests).
- New file in `tests/DualFrontier.Modding.Tests/Pipeline/`: `PipelineGetActiveModsTests.cs` (4 tests).
- Reuse `M72UnloadChainTests.Harness` pattern for in-memory pipeline injection. If extracting the harness to a shared seam file (e.g. `tests/DualFrontier.Modding.Tests/Pipeline/PipelineTestHarness.cs`) is cleaner than duplicating, do so as part of this commit; otherwise inline copy with a comment noting future consolidation.
- Ensure all 30 new tests are green; full suite at 408/408.

**3.** `docs(roadmap): close M7.5.A — ModMenuController + pipeline read API`

- `ROADMAP.md` M7 sub-phase status block: split former M7.5 entry into M7.5.A and M7.5.B.
   - M7.5.A entry: ✅ Closed, with commits 1+2 SHA + acceptance summary in M7.1/M7.2/M7.3/M7.4 entry pattern. Mention controller surface, IModDiscoverer abstraction, GetActiveMods extension, AD #4 deliberate interpretation of §9.2 failure path.
   - M7.5.B entry: ⏭ Pending, "Godot UI binding for ModMenuController; manual verification".
- Header status line: `*Updated: YYYY-MM-DD (M7.5.A closed — ModMenuController + IModDiscoverer + Pipeline.GetActiveMods on DualFrontier.Application; M7.5.B + M7-closure pending).*`
- Engine snapshot: 378 → 408 tests. List the 30 new tests by class (`ModMenuControllerTests`, `DefaultModDiscovererTests`, `PipelineGetActiveModsTests`).
- Status overview table M7 row tests column extended with `M7.5.A added` line.
- Register AD #4 deliberate-interpretation paragraph (failed-commit-stays-paused) parallel pattern to the M7.1 §9.2/§9.3 footer and the M7.3 §9.5 step 7 ordering footer.

**Special verification preamble for commits 1 + 2:**

- After commit 1: `dotnet build && dotnet test --filter "FullyQualifiedName~M71|FullyQualifiedName~M72|FullyQualifiedName~M73|FullyQualifiedName~M74|FullyQualifiedName~ManifestRewriter"` — no regression. Plus a smoke `dotnet build` confirming the new types compile cleanly.
- After commit 2: `dotnet test --filter "FullyQualifiedName~ModMenuController|FullyQualifiedName~DefaultModDiscoverer|FullyQualifiedName~PipelineGetActiveMods"` — 30 new tests green. Full suite at 408/408 expected.
- After commit 3: ROADMAP renders cleanly; cross-references resolve; M7.5.A and M7.5.B entries both present and correctly statused.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice. Per spec preamble "stop, escalate, lock — never guess".

**Hypothesis-falsification clause:**

Datapoints (per [M6 closure review §10](./M6_CLOSURE_REVIEW.md)): M3=1, M4=1, M5=0, M6=0, M7.1=0, M7.2=0, M7.3=0, M7.4=0. M7.5.A closure pending = potentially **ninth consecutive zero** post-M4.

M7.5.A exercises §9.2 (menu flow) + §9.6 (hot-reload disabled semantics) + §2.2 (`hotReload` field default). The implementation surface — controller class, abstraction interface, DTO records, list-snapshot pipeline method — sits on standard C# composition patterns, not exotic runtime contracts. **If implementation surfaces a §9 contradiction requiring v1.6 ratification → hypothesis falsified. Report immediately.**

Plausible v1.6 candidates worth flagging if encountered:

(a) **§9.2 failure-path semantics underspec.** AD #4 (failed commit stays paused, session open) is a deliberate interpretation. If empirical reality demands different semantics (e.g. §9.2 wording forbids paused-after-failed-Apply), ratification candidate.

(b) **§9.6 first-load semantics underspec.** AD #5 (hotReload=false applies only to currently-active mods, first-load OK) is interpreted from the wording "cannot be reloaded mid-session" + §2.2 "loads only at session start". If reality demands a stricter reading (hotReload=false mods cannot be added mid-session either), ratification.

(c) **§9.5.1 best-effort warnings affecting commit Success.** Currently CommitResult.Success is gated only on Apply.Success, not on UnloadMod warnings. If a per-mod UnloadMod warning should fail the commit, this is a different §9 reading.

(d) **GetActiveMods race semantics.** The fresh-list-per-call contract (test 30) is the simplest contract; if pipeline reality requires a different surface (e.g. immutable snapshot, or live view), ratification candidate.

(e) **DefaultModDiscoverer error handling.** AD's silently-skip-broken-manifest contract (test 26). If §9 actually wants discoverer errors to propagate to the menu UI, ratification candidate.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (378 + 30 = 408 expected, or actual with discrepancy noted).
- Per-test confirmation: 30 new tests all green by name (22 `ModMenuControllerTests` + 4 `DefaultModDiscovererTests` + 4 `PipelineGetActiveModsTests`).
- Regression confirmation: M7.1 (11) + M7.2 (13) + M7.3 (7) + M7.4 (9) + remaining M0–M6 + Persistence + Systems + Core all still green.
- Working tree state: clean / dirty.
- **§9 contradiction status**: zero (or REPORT IMMEDIATELY with section reference + which of the 5 plausible categories above + proposed ratification candidate).
- **AD #4 deliberate interpretation**: registered in ROADMAP M7.5.A closure entry, parallel to M7.1 / M7.3 footer pattern.
- **M-phase boundary discipline**: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` = empty (verify and report).
- **Solution file**: `dotnet sln list` count unchanged from M7.4 baseline (no new projects in M7.5.A).
- Any unexpected findings, especially around `LoadedMod` access patterns or `_activeMods` mutation semantics during Apply that affect controller-side reasoning.
- **Special**: any §9 spec contradiction discovered (would be ratification candidate for v1.6 — flag immediately with category from list above).
