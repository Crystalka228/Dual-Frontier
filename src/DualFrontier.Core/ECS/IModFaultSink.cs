using System;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Sink for mod-system faults reported from the isolation guard.
/// Implemented in the Application layer by {ModFaultHandler} (Phase 2 completion
/// is scheduled later — for now a no-op sink is enough). Core never crashes a
/// mod's host process directly; it hands the fault off through this sink and
/// throws {IsolationViolationException} with a mod-specific message, and the
/// scheduler's caller decides what to do.
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
/// scenarios. Production code in Application should replace this with the
/// real {ModFaultHandler}.
/// </summary>
internal sealed class NullModFaultSink : IModFaultSink
{
    /// <summary>
    /// No-op: silently discards the fault. The {IsolationViolationException}
    /// that the guard throws alongside this call still propagates, so faults
    /// are never truly silent — the exception carries the diagnostic message.
    /// </summary>
    /// <param name="modId">Ignored.</param>
    /// <param name="message">Ignored.</param>
    public void ReportFault(string modId, string message)
    {
    }
}
