using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Movement state for a pawn: current path and destination.
/// Written exclusively by MovementSystem.
/// </summary>
public sealed class MovementComponent : IComponent
{
    /// <summary>Current destination tile. Null = no target.</summary>
    public GridVector? Target;

    /// <summary>Remaining path steps to destination.</summary>
    public List<GridVector> Path = new();

    /// <summary>Ticks to wait before taking next step.</summary>
    public int StepCooldown;
}
