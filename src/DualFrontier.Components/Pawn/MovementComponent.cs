using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Interop;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Movement state for a pawn: current path and destination.
/// Written exclusively by MovementSystem.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct MovementComponent : IComponent
{
    /// <summary>
    /// Current destination tile. Meaningful only when <see cref="HasTarget"/>
    /// is true; otherwise carries the default GridVector(0,0) which is also
    /// a valid map location, hence the explicit flag.
    /// </summary>
    public GridVector Target;

    /// <summary>True when <see cref="Target"/> is meaningful; false = no destination.</summary>
    public bool HasTarget;

    /// <summary>
    /// Pathfinding waypoints to <see cref="Target"/>. Walked sequentially via
    /// <see cref="PathStepIndex"/> rather than via RemoveAt because
    /// <see cref="NativeComposite{T}.RemoveAt"/> uses swap-with-last semantics
    /// that would break the FIFO walk order. Path is fully populated at
    /// pathfinding time; <see cref="PathStepIndex"/> advances from 0 toward
    /// CountFor(entity); when index == count the path is done.
    ///
    /// Default is the invalid sentinel (<c>IsValid == false</c>) — must be
    /// constructed via <c>NativeWorld.CreateComposite&lt;GridVector&gt;()</c>
    /// at factory time before MovementSystem can populate steps.
    /// </summary>
    public NativeComposite<GridVector> Path;

    /// <summary>0-based index into <see cref="Path"/>: the next waypoint to walk.</summary>
    public int PathStepIndex;

    /// <summary>Ticks to wait before taking next step.</summary>
    public int StepCooldown;
}
