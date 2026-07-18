using System;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Sink for mod-system fault reports. Implemented in the Application layer by
/// {ModFaultHandler}. Faults arrive through <c>SystemExecutionContext.RouteFault</c>
/// — the single EQ_A1 D2 origin-dispatch definition: a mod-origin fault caught in
/// <c>ParallelSystemScheduler.ExecutePhase</c> (or in <c>DomainEventBus</c> delivery,
/// either mode) is reported here via <c>ReportFault</c>, the offending mod is
/// committed to the session quarantine skip-set, and its deferred unload is drained by
/// <c>ModIntegrationPipeline.Apply</c> at the next menu open. Core never crashes a
/// mod's host process directly — it hands the fault off through this sink and the
/// upper layer decides what to do. (The prior "faults arrive through
/// <c>ModLoader.HandleModFault</c>" description was stale — corrected at EQ_A2, REC-A1.)
/// The К8.3+К8.4 runtime isolation guard that once called this sink is deleted;
/// isolation is now compile-time via [SystemAccess].
/// </summary>
internal interface IModFaultSink
{
    /// <summary>
    /// Reports a fault originating from the mod with the given identifier.
    /// Implementations typically log the fault and schedule the mod for unload.
    /// </summary>
    /// <param name="modId">Unique identifier of the offending mod.</param>
    /// <param name="message">Diagnostic message with the violation details.</param>
    void ReportFault(string modId, string message);
}

/// <summary>
/// Default no-op {IModFaultSink} used when no Application-layer handler is
/// wired in — keeps {SystemExecutionContext} functional in tests and Core-only
/// scenarios. Production wiring (GameBootstrap) passes the real
/// {ModFaultHandler} instead.
/// </summary>
internal sealed class NullModFaultSink : IModFaultSink
{
    /// <summary>
    /// No-op: silently discards the fault. Post-К8.3+К8.4 no Core call site
    /// invokes the sink (the runtime guard that did is deleted); this null
    /// implementation exists to satisfy the scheduler/context constructor
    /// contract in tests and Core-only scenarios.
    /// </summary>
    /// <param name="modId">Ignored.</param>
    /// <param name="message">Ignored.</param>
    public void ReportFault(string modId, string message)
    {
    }
}
