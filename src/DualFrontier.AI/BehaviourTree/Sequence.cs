namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// Композит "И": выполняет детей по порядку, пока все не
/// вернут Success. Если любой вернул Failure — возвращает
/// Failure. Если ребёнок Running — возвращает Running.
///
/// Используется для последовательных планов: "подойти к
/// складу, взять предмет, отнести на стройку".
/// </summary>
public class Sequence : BTNode
{
    private readonly BTNode[] _children;
    private int _currentIndex;

    /// <summary>
    /// Создаёт последовательность из упорядоченного списка детей.
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
