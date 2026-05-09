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
/// Owned by <see cref="ModIntegrationPipeline"/>; constructed during pipeline
/// startup. Faults arrive on simulation tick threads (via the isolation
/// guard or the public <see cref="ModLoader.HandleModFault"/> entry point);
/// reads happen on the menu thread when <see cref="ModIntegrationPipeline.Apply"/>
/// drains the faulted set. All access is gated through <c>lock (_lock)</c>
/// to keep the handler thread-safe across that boundary.
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
    // Owner reference per the K6 brief Phase 3.1 design. Currently unused
    // by handler methods (the deferred-drain model means the pipeline
    // pulls from the handler, not the reverse), retained for the
    // documented future hook where a fault might trigger a synchronous
    // pipeline notification before the next menu open.
    private readonly ModIntegrationPipeline _pipeline;
    private readonly object _lock = new();
    private readonly HashSet<string> _faultedMods = new(StringComparer.Ordinal);

    public ModFaultHandler(ModIntegrationPipeline pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    /// <summary>
    /// Called by <see cref="SystemExecutionContext"/> (via the
    /// <see cref="IModFaultSink"/> contract) and by
    /// <see cref="ModLoader.HandleModFault"/> when a mod system breaks
    /// isolation or otherwise misbehaves. Records the fault in an internal
    /// set for deferred unload at the next menu open. Idempotent: faulting
    /// the same mod twice during a single tick is harmless. Defensive on
    /// null modId (silent no-op) per the sink contract — the
    /// <see cref="DualFrontier.Core.ECS.IsolationViolationException"/>
    /// thrown alongside the report carries the diagnostic message, so
    /// the sink itself is intentionally silent.
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
