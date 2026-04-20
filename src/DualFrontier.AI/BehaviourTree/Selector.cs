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
        throw new NotImplementedException(
            "TODO: Фаза 3 — реализация Selector.Tick с запоминанием текущего ребёнка"
        );
    }
}
