namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// Базовый класс листа behaviour tree: конкретное действие
/// (идти к точке, съесть еду) или условие (голоден ли пешка?).
///
/// Листья НЕ вызывают друг друга — композиция идёт через
/// <see cref="Selector"/> и <see cref="Sequence"/>.
/// </summary>
public class Leaf : BTNode
{
    /// <inheritdoc />
    public override BTStatus Tick(BTContext ctx)
    {
        throw new NotImplementedException(
            "TODO: Фаза 3 — конкретная логика листа переопределяется наследниками"
        );
    }
}
