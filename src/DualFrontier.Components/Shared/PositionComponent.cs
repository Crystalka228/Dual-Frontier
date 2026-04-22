using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Components.Shared;

/// <summary>
/// Позиция entity в тайловой сетке мира. Pure data.
/// Модификация — только через MovementSystem / TeleportSystem.
/// </summary>
public sealed class PositionComponent : IComponent
{
    /// <summary>Координата в тайловой сетке.</summary>
    public GridVector Position;
}
