namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// "AND" composite: ticks children in order until all return Success.
/// If any child returns Failure — returns Failure. If a child returns
/// Running — returns Running.
///
/// Used for sequential plans: "walk to the warehouse, pick up an item,
/// deliver it to the construction site".
/// </summary>
public class Sequence : BTNode
{
    private readonly BTNode[] _children;
    private int _currentIndex;

    /// <summary>
    /// Creates a sequence from an ordered list of children.
    /// </summary>
    public Sequence(params BTNode[] children)
    {
        _children = children;
    }

    /// <inheritdoc />
    public override BTStatus Tick(BTContext ctx)
    {
        for (int i = _currentIndex; i < _children.Length; i++)
        {
            BTStatus status = _children[i].Tick(ctx);
            if (status == BTStatus.Failure)
            {
                _currentIndex = 0;
                return BTStatus.Failure;
            }
            if (status == BTStatus.Running)
            {
                _currentIndex = i;
                return BTStatus.Running;
            }
        }
        _currentIndex = 0;
        return BTStatus.Success;
    }
}
