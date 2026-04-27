namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// "OR" composite: tries children in order, returns the first Success or
/// Running. Returns Failure only if every child returned Failure.
///
/// Used for fallback behaviour: "try to eat, otherwise sleep, otherwise
/// idle".
/// </summary>
public class Selector : BTNode
{
    private readonly BTNode[] _children;
    private int _currentIndex;

    /// <summary>
    /// Creates a selector from an ordered list of children.
    /// </summary>
    public Selector(params BTNode[] children)
    {
        _children = children;
    }

    /// <inheritdoc />
    public override BTStatus Tick(BTContext ctx)
    {
        for (int i = _currentIndex; i < _children.Length; i++)
        {
            BTStatus status = _children[i].Tick(ctx);
            if (status == BTStatus.Success)
            {
                _currentIndex = 0;
                return BTStatus.Success;
            }
            if (status == BTStatus.Running)
            {
                _currentIndex = i;
                return BTStatus.Running;
            }
        }
        _currentIndex = 0;
        return BTStatus.Failure;
    }
}
