using System;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Contracts.Services;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Concrete <see cref="ISystemServices"/> — the construction-time dependency
/// surface handed to system factories (W1 BD-2). Day-one surface is exactly the
/// measured injection: pathfinding (the sole service the harness injects, into
/// <c>MovementSystem</c>). No speculative members.
/// </summary>
internal sealed class SystemServices : ISystemServices
{
    public SystemServices(IPathfindingService pathfinding)
        => Pathfinding = pathfinding ?? throw new ArgumentNullException(nameof(pathfinding));

    public IPathfindingService Pathfinding { get; }
}
