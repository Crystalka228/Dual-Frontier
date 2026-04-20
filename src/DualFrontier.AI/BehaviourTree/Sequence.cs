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
        throw new NotImplementedException(
            "TODO: Фаза 3 — реализация Sequence.Tick с запоминанием текущего ребёнка"
        );
    }
}
