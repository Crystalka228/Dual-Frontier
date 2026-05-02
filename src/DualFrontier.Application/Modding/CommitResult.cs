using System;
using System.Collections.Generic;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Outcome of <see cref="ModMenuController.Commit"/>. Aggregates results
/// across the per-removed-mod <c>UnloadMod</c> calls and the single
/// <c>Apply</c> call for added mods. On success the simulation has been
/// resumed; on failure it stays paused so the user can fix the pending
/// state and retry (per MOD_OS_ARCHITECTURE v1.5 §9.2 + the M7.5.A AD #4
/// deliberate interpretation registered in ROADMAP).
/// </summary>
/// <param name="Success">
/// True iff <c>Apply</c> returned <c>Success: true</c> — or, when no
/// add-paths were involved, true unconditionally (only-removals path).
/// Per §9.5.1 best-effort, an <c>UnloadMod</c> warning does not by
/// itself fail the commit; only an <c>Apply</c> validation error flips
/// this to false.
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
/// <c>Apply</c>'s <c>LoadedModIds</c>). Empty on the failure path and
/// when only removals were committed.
/// </param>
/// <param name="NewlyInactiveModIds">
/// Mods that became inactive as a result of this commit. Records each
/// mod the controller passed to <c>UnloadMod</c>; the list is unchanged
/// regardless of per-mod warnings since the mod is removed from the
/// active set regardless per §9.5.1.
/// </param>
public sealed record CommitResult(
    bool Success,
    IReadOnlyList<ValidationError> Errors,
    IReadOnlyList<ValidationWarning> Warnings,
    IReadOnlyList<string> NewlyActiveModIds,
    IReadOnlyList<string> NewlyInactiveModIds);
