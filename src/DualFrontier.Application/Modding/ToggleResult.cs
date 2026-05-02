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
    /// per MOD_OS_ARCHITECTURE v1.5 §9.6 it cannot be toggled
    /// mid-session. UI should grey out the button and show the
    /// restart-required tooltip.
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
