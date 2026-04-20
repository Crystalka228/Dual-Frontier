using DualFrontier.Contracts.Core;

namespace DualFrontier.AI.BehaviourTree;

/// <summary>
/// Контекст одного тика BT: за какую пешку работаем, какие
/// сервисы доступны. Передаётся в <see cref="BTNode.Tick"/>.
///
/// Система из <c>DualFrontier.Systems</c> собирает этот контекст
/// на каждом тике пешки и прокидывает в корневой BT.
///
/// TODO: заменить <see cref="Services"/> с <c>object?</c> на
/// реальный интерфейс сервисов AI, когда он появится
/// (например, <c>IAIServices</c> в <c>Contracts</c>).
/// </summary>
/// <param name="Entity">Пешка, для которой тикаем BT.</param>
/// <param name="DeltaSeconds">Прошло реального времени с прошлого тика.</param>
/// <param name="Services">Набор сервисов, доступных AI (pathfinding, blackboard).</param>
public readonly record struct BTContext(
    EntityId Entity,
    float DeltaSeconds,
    object? Services
);
