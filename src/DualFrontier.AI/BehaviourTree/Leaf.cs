namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// Base class for behaviour-tree leaves: a concrete action
/// (walk to a point, eat food) or a condition (is the pawn hungry?).
///
/// Leaves do NOT call each other — composition goes through
/// <see cref="Selector"/> and <see cref="Sequence"/>.
/// </summary>
public abstract class Leaf : BTNode { }
