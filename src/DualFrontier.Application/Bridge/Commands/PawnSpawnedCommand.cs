using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Команда: пешка <paramref name="PawnId"/> появилась в мире в позиции
/// (<paramref name="X"/>, <paramref name="Y"/>). Presentation-слой создаёт
/// визуальную ноду и размещает её на сцене.
/// </summary>
/// <param name="PawnId">Идентификатор новой пешки.</param>
/// <param name="X">Координата X (в клетках сетки).</param>
/// <param name="Y">Координата Y (в клетках сетки).</param>
public sealed record PawnSpawnedCommand(EntityId PawnId, float X, float Y) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Фаза 5 — apply via active IRenderer backend. */
    }
}
