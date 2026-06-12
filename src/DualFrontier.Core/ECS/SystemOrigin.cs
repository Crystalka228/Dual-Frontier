using System;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Origin of a registered system. Determines reaction to faults: a Core
/// system fault is a developer bug and propagates out of the scheduler
/// unhandled; mod faults route through {IModFaultSink} (production:
/// ModLoader.HandleModFault → ModFaultHandler) so the upper layer can
/// queue the mod for deferred unload at the next menu open without
/// taking the game down. The К8.3+К8.4 runtime isolation guard (which
/// threw IsolationViolationException) is deleted; isolation is now
/// compile-time via [SystemAccess].
/// </summary>
internal enum SystemOrigin
{
    /// <summary>
    /// System from DualFrontier.Systems or any first-party assembly.
    /// A fault here is a developer bug — crash loudly.
    /// </summary>
    Core,

    /// <summary>
    /// System loaded from a user mod through ModLoader.
    /// A fault here queues the mod for deferred unload and keeps the
    /// game running.
    /// </summary>
    Mod,
}
