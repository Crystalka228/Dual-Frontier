using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Per-mod row in the editing session, returned by
/// <see cref="ModMenuController.GetEditableState"/>. Combines the
/// session-start active-set snapshot with the current pending state
/// and the §9.6 hot-reload restriction so the UI (M7.5.B) can render
/// rows without re-computing flags itself.
/// </summary>
/// <param name="ModId">Manifest id.</param>
/// <param name="Manifest">Parsed manifest.</param>
/// <param name="IsCurrentlyActive">
/// True iff the mod was active at <see cref="ModMenuController.BeginEditing"/>
/// time. Stable across the editing session — the snapshot is taken at
/// Begin and never updated until Commit/Cancel close the session.
/// </param>
/// <param name="IsPendingActive">
/// True iff the mod is in the pending active set (will be active after
/// a successful <see cref="ModMenuController.Commit"/>). Mutates as the
/// user toggles.
/// </param>
/// <param name="CanToggle">
/// False when §9.6 forbids the toggle: a currently-active mod with
/// <see cref="ModManifest.HotReload"/> = false. The UI greys out the
/// toggle button and shows the "restart required" tooltip when this
/// flag is false. <see cref="ModMenuController.Toggle"/> additionally
/// rejects the call defensively for misrouted UI inputs.
/// </param>
public sealed record EditableModInfo(
    string ModId,
    ModManifest Manifest,
    bool IsCurrentlyActive,
    bool IsPendingActive,
    bool CanToggle);
