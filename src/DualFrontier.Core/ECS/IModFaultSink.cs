using System;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Sink for mod-system fault reports. Implemented in the Application layer
/// by {ModFaultHandler} (realized — K6-era work); faults arrive through the
/// public <c>ModLoader.HandleModFault</c> entry point and the offending mod
/// is queued for deferred unload at the next menu open. Core never crashes a
/// mod's host process directly — it hands the fault off through this sink
/// and the upper layer decides what to do. The К8.3+К8.4 runtime isolation
/// guard (which called this sink and threw IsolationViolationException from
/// inside SystemExecutionContext) is deleted; isolation is now compile-time
/// via [SystemAccess].
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
