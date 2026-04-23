using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Команда: пешка <paramref name="PawnId"/> умерла в клетке
/// (<paramref name="X"/>, <paramref name="Y"/>). Presentation-слой показывает
/// эффект смерти и обновляет зависящие от пешки UI-плашки.
/// </summary>
/// <param name="PawnId">Идентификатор погибшей пешки.</param>
/// <param name="X">Координата X клетки.</param>
/// <param name="Y">Координата Y клетки.</param>
public sealed record PawnDiedCommand(EntityId PawnId, int X, int Y) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Фаза 5 — apply via active IRenderer backend. */
    }
}
