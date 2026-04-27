using System;
using System.Collections.Generic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// A set of systems that may execute in parallel (no shared WRITE access).
/// Immutable: built once by <see cref="DependencyGraph"/> and thereafter
/// only read by the scheduler.
/// </summary>
internal sealed class SystemPhase
{
    /// <summary>
    /// Systems in this phase. Order is unspecified — execution is parallel.
    /// </summary>
    public IReadOnlyList<SystemBase> Systems { get; }

    /// <summary>
    /// Creates a phase from a pre-built list of systems.
    /// </summary>
    public SystemPhase(IReadOnlyList<SystemBase> systems)
    {
        Systems = systems ?? throw new ArgumentNullException(nameof(systems));
    }
}
