using System;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Contracts.Services;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Concrete <see cref="ISystemServices"/> — the construction-time dependency
/// surface handed to system factories (W1 BD-2). Every member is a measured injection, never a
/// speculative one: pathfinding, which <c>MovementSystem</c> takes at construction.
/// </summary>
internal sealed class SystemServices : ISystemServices
{
    public SystemServices(IPathfindingService pathfinding)
        => Pathfinding = pathfinding ?? throw new ArgumentNullException(nameof(pathfinding));

    public IPathfindingService Pathfinding { get; }
}
