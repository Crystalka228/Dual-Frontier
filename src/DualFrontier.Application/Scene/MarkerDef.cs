using DualFrontier.Contracts.Math;

namespace DualFrontier.Application.Scene;

/// <summary>
/// A named position on the map with no entity attached. Used for spawn
/// points, caravan exits, ritual circles, camera anchors.
/// </summary>
public sealed record MarkerDef(string Id, GridVector Position);
