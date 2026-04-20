namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// Результат одного тика ноды behaviour tree.
/// </summary>
public enum BTStatus
{
    /// <summary>Нода ещё работает — вернуться на следующем тике.</summary>
    Running,

    /// <summary>Нода выполнилась успешно.</summary>
    Success,

    /// <summary>Нода не смогла выполниться.</summary>
    Failure
}

/// <summary>
/// Базовый класс ноды behaviour tree. Все композиты (Selector,
/// Sequence) и листья (Leaf) наследуются от него.
///
/// Тикается синхронно из AI-слоя (см. GDD раздел "AI пешек").
/// </summary>
public abstract class BTNode
{
    /// <summary>
    /// Прогон одного шага ноды. Чистая функция по контексту —
    /// никаких глобальных обращений к миру.
    /// </summary>
    public abstract BTStatus Tick(BTContext ctx);
}
