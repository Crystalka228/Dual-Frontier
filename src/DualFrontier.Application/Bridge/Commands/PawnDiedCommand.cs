using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Команда: пешка <paramref name="PawnId"/> умерла. Presentation-слой
/// находит её визуальную ноду, проигрывает анимацию смерти и удаляет
/// ноду по её завершении.
/// </summary>
/// <param name="PawnId">Идентификатор погибшей пешки.</param>
public sealed record PawnDiedCommand(EntityId PawnId) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Фаза 5 — apply via active IRenderer backend. */
    }
}
