using System;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Contracts.Services;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Concrete <see cref="ISystemServices"/> — the construction-time dependency
/// surface handed to system factories (W1 BD-2). Every member is a measured injection, never a
/// speculative one: pathfinding, which <c>MovementSystem</c> takes at construction, and the
/// walkability grid, which the vanilla placement routines consult and which only game-side code
/// can build once the engine sheds its game references (W4).
/// </summary>
internal sealed class SystemServices : ISystemServices
{
    public SystemServices(IPathfindingService pathfinding, INavGridService navGrid)
    {
        Pathfinding = pathfinding ?? throw new ArgumentNullException(nameof(pathfinding));
        NavGrid = navGrid ?? throw new ArgumentNullException(nameof(navGrid));
    }

    public IPathfindingService Pathfinding { get; }

    public INavGridService NavGrid { get; }
}
