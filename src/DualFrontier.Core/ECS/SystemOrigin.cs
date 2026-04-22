using System;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Origin of a registered system. Determines reaction to isolation violations:
/// Core systems crash with {IsolationViolationException}, mod systems route
/// through {IModFaultSink} so the upper layer can unload the mod without
/// taking the game down.
/// </summary>
internal enum SystemOrigin
{
    /// <summary>
    /// System from DualFrontier.Systems or any first-party assembly.
    /// A violation here is a developer bug — crash loudly.
    /// </summary>
    Core,

    /// <summary>
    /// System loaded from a user mod through ModLoader.
    /// A violation here unloads the mod and keeps the game running.
    /// </summary>
    Mod,
}
