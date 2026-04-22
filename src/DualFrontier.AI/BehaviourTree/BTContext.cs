using DualFrontier.AI.BehaviourTree;
using DualFrontier.Contracts.Core;

/// <summary>
/// Passed to every BTNode.Tick call. Contains the entity being evaluated
/// and its blackboard. Nodes must not store a reference to BTContext
/// between ticks — it may be recreated each tick.
/// </summary>
public sealed class BTContext
{
    /// <summary>
    /// The unique identifier of the entity this Behaviour Tree is currently evaluating.
    /// </summary>
    public EntityId PawnId { get; }

    /// <summary>
    /// Shared state for this pawn's Behaviour Tree across ticks.
    /// </summary>
    public BTBlackboard Blackboard { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BTContext"/> class.
    /// </summary>
    /// <param name="pawnId">The unique ID of the pawn.</param>
    /// <param name="blackboard">The shared blackboard for this pawn's state.</param>
    public BTContext(EntityId pawnId, BTBlackboard blackboard)
    {
        PawnId = pawnId;

        // Null-check blackboard and store both.
        if (blackboard == null)
        {
            throw new ArgumentNullException(nameof(blackboard), "The blackboard cannot be null.");
        }
        Blackboard = blackboard;
    }
}