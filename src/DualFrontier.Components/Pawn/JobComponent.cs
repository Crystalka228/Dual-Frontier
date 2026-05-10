using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Represents a job component defining the current assignment, target, and status of an entity's work.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct JobComponent : IComponent
{
    /// <inheritdoc />
    public JobKind Current;

    /// <inheritdoc />
    public EntityId? Target; // optional target entity (building to construct, item to haul, pawn to treat); null if job needs no target

    /// <summary>
    /// How many ticks the pawn has been working this job; reset to 0 on new assignment.
    /// </summary>
    public int TicksAtJob;

    /// <summary>
    /// True if the job was interrupted and needs reassignment next tick.
    /// </summary>
    public bool IsInterrupted;

    /// <inheritdoc />
    public bool IsIdle => Current == JobKind.Idle;

    /// <inheritdoc />
    public bool NeedsTarget => !Target.HasValue && Current != JobKind.Idle && Current != JobKind.Sleep && Current != JobKind.Meditate;
}
