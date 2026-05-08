using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Components.Shared;

/// <summary>
/// Position of an entity on the world's tile grid. Pure data.
/// Mutated only via MovementSystem / TeleportSystem.
/// </summary>
public struct PositionComponent : IComponent
{
    /// <summary>Tile-grid coordinate.</summary>
    public GridVector Position;
}
