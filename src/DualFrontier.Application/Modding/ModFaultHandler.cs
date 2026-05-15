using System;
using System.Collections.Generic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Application-side handler for mod faults reported through the Core
/// <see cref="IModFaultSink"/> interface. Implements the "fault → deferred
/// unload" pipeline per MOD_OS_ARCHITECTURE §10.3 (architectural threats —
/// caught) and TechArch 11.8 (the documented "core does not crash; the
/// offending mod is unloaded" behaviour).
///
/// Owned by <see cref="DualFrontier.Application.Loop.GameBootstrap"/>; constructed before the scheduler
/// so the scheduler ctor can take it as an immutable reference (K6.1
/// ownership inversion). Consumers
/// (<see cref="ModIntegrationPipeline.Apply"/>,
/// <see cref="ModLoader.HandleModFault"/>) hold a reference to the handler
/// and query <see cref="GetFaultedMods"/> / <see cref="ClearFault"/> on
/// demand — the handler itself depends on nothing. Faults arrive via the
/// public <see cref="ModLoader.HandleModFault"/> entry point (post-K8.3+K8.4
/// cutover, the runtime isolation-guard route is removed; isolation is
/// enforced at compile time by <c>[SystemAccess]</c> + the future A'.9
/// analyzer). Reads happen on the menu thread when
/// <see cref="ModIntegrationPipeline.Apply"/> drains the faulted set.
/// All access is gated through <c>lock (_lock)</c> to keep the handler
/// thread-safe across that boundary.
///
/// The handler does NOT rebuild the dependency graph synchronously. Per
/// TechArch 11.8 and the comment retained from the original
/// <see cref="ModLoader.HandleModFault"/> Phase 2 plan, graph rebuild is
/// deferred to the next menu open. Rationale: a fault arrives mid-tick on
/// a worker thread; rebuilding the graph synchronously would race with
/// other workers. The mod is queued; the next time the user opens the mod
/// menu, the pipeline observes the queued mod and the rebuild happens
/// through the normal <see cref="ModIntegrationPipeline.Apply"/> flow,
/// which already pauses the scheduler.
///
/// Reentrance: <see cref="ReportFault"/> from inside another fault handler
/// is harmless — the set deduplicates, and the handler does not call back
/// into <see cref="ModIntegrationPipeline"/> at fault time.
/// </summary>
internal sealed class ModFaultHandler : IModFaultSink
{
    private readonly object _lock = new();
    private readonly HashSet<string> _faultedMods = new(StringComparer.Ordinal);

    public ModFaultHandler()
    {
        // No dependencies. Handler is a self-contained fault accumulator;
        // consumers (ModIntegrationPipeline.Apply, ModLoader.HandleModFault)
        // query GetFaultedMods / ClearFault on demand. Owned by GameBootstrap
        // as a session-scoped singleton wired into the scheduler before
        // mods are loaded.
    }

    /// <summary>
    /// Called by <see cref="ModLoader.HandleModFault"/> when a mod system
    /// misbehaves. Records the fault in an internal set for deferred unload
    /// at the next menu open. Idempotent: faulting the same mod twice during
    /// a single tick is harmless. Defensive on null modId (silent no-op) per
    /// the sink contract — the diagnostic message is already on the
    /// underlying exception that triggered the report.
    /// </summary>
    public void ReportFault(string modId, string message)
    {
        if (modId is null) return;
        lock (_lock)
        {
            _faultedMods.Add(modId);
        }
    }

    /// <summary>
    /// Snapshot of mods marked for unload due to faults. Read by
    /// <see cref="ModIntegrationPipeline.Apply"/> at menu-open time to
    /// determine which mods to unload via the standard §9.5 chain before
    /// processing the new mod set. Returns a fresh allocation so callers
    /// may iterate without holding the lock.
    /// </summary>
    public IReadOnlyList<string> GetFaultedMods()
    {
        lock (_lock)
        {
            if (_faultedMods.Count == 0)
                return Array.Empty<string>();
            return new List<string>(_faultedMods);
        }
    }

    /// <summary>
    /// Clears one mod from the faulted-mods set. Called by
    /// <see cref="ModIntegrationPipeline.Apply"/> after the deferred unload
    /// for that mod completes. Defensive on null and unknown modId — both
    /// are silent no-ops.
    /// </summary>
    public void ClearFault(string modId)
    {
        if (modId is null) return;
        lock (_lock)
        {
            _faultedMods.Remove(modId);
        }
    }
}
