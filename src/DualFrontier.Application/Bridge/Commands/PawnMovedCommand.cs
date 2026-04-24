using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Команда: пешка <paramref name="PawnId"/> переместилась в клетку
/// (<paramref name="X"/>, <paramref name="Y"/>). Presentation-слой обновляет
/// позицию визуальной ноды.
/// </summary>
/// <param name="PawnId">Идентификатор пешки, которая переместилась.</param>
/// <param name="X">Новая координата X (в клетках сетки).</param>
/// <param name="Y">Новая координата Y (в клетках сетки).</param>
public sealed record PawnMovedCommand(EntityId PawnId, float X, float Y) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Фаза 5 — apply via active IRenderer backend. */
    }
}
