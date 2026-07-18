using System;
using System.Collections.Generic;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Session-local quarantine set for faulted mods (EQ_A1 / M1 — the immediate half
/// of the ENGINE_LIFECYCLE_AND_TRANSACTIONS §2.3 two-stage split). When a mod
/// system faults in <see cref="ParallelSystemScheduler.ExecutePhase"/>, its owning
/// mod id is committed here so the scheduler skips that mod's systems on every
/// subsequent tick this session. This is cheap (a set consulted by dispatch, no
/// graph rebuild mid-tick) and atomic — the "quarantine = commit" stage ELT §2.3
/// names.
///
/// The queued-reclamation half is unchanged and owned by the Application layer: the
/// §9.5 unload chain drained by <c>ModIntegrationPipeline.Apply</c> at the next menu
/// open. Release from this set follows implicitly — a reclaimed mod's systems leave
/// the graph, so their ids are never consulted again. No scheduler-side release API
/// is wired (that Application-to-scheduler call is later-cascade scope).
///
/// This is the quarantine MECHANISM. As of EQ_A2 (Cascade B) it is LINKED to the
/// session-scoped EngineHealth/Degraded surface (ELT §4.1): on the FIRST quarantine
/// of a mod the scheduler fires <c>ParallelSystemScheduler.OnModQuarantined</c>, which
/// the Application wires to <c>EngineSession.ReportDegraded</c> — the quarantined mod
/// becomes a structured Degraded reason. The reason-removal (exit) path and any UI
/// surfacing remain later-cascade scope.
///
/// Thread-safe: <see cref="ParallelSystemScheduler.ExecutePhase"/> reads and mutates
/// this concurrently across the <c>Parallel.ForEach</c> fan-out, so every access is
/// lock-guarded (the <c>ModFaultHandler</c> pattern).
/// </summary>
internal sealed class ModQuarantine
{
    private readonly object _lock = new();
    private readonly HashSet<string> _quarantined = new(StringComparer.Ordinal);

    /// <summary>
    /// Commits a mod to quarantine. Idempotent — a mod that faults on several
    /// systems (or on several ticks before the first skip takes effect) is added
    /// once.
    /// </summary>
    /// <param name="modId">Owning mod id of the faulted system.</param>
    /// <returns><see langword="true"/> iff this call NEWLY quarantined the mod (the first commit); <see langword="false"/> if it was already quarantined or <paramref name="modId"/> is null.</returns>
    internal bool Quarantine(string modId)
    {
        if (modId is null) return false;
        lock (_lock)
        {
            return _quarantined.Add(modId);
        }
    }

    /// <summary>
    /// True if the mod is currently quarantined and its systems must be skipped.
    /// </summary>
    /// <param name="modId">Owning mod id to test.</param>
    internal bool IsQuarantined(string modId)
    {
        if (modId is null) return false;
        lock (_lock)
        {
            return _quarantined.Contains(modId);
        }
    }
}
