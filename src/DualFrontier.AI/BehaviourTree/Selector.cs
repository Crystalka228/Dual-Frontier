namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// Композит "ИЛИ": пробует детей по порядку, возвращает
/// первый Success / Running. Возвращает Failure, только если
/// все дети вернули Failure.
///
/// Используется для fallback-поведения: "попытайся поесть,
/// иначе поспи, иначе простаивай".
/// </summary>
public class Selector : BTNode
{
    private readonly BTNode[] _children;
    private int _currentIndex;

    /// <summary>
    /// Создаёт селектор из упорядоченного списка детей.
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
