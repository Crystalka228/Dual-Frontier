namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// Result of a single behaviour-tree node tick.
/// </summary>
public enum BTStatus
{
    /// <summary>Node is still working — call again next tick.</summary>
    Running,

    /// <summary>Node completed successfully.</summary>
    Success,

    /// <summary>Node failed to complete.</summary>
    Failure
}

/// <summary>
/// Base class for behaviour-tree nodes. All composites (Selector, Sequence)
/// and leaves (Leaf) inherit from it.
///
/// Ticked synchronously from the AI layer (see GDD section "Pawn AI").
/// </summary>
public abstract class BTNode
{
    /// <summary>
    /// Runs a single step of the node. Pure function over the context —
    /// no global access to the world.
    /// </summary>
    public abstract BTStatus Tick(BTContext ctx);
}
