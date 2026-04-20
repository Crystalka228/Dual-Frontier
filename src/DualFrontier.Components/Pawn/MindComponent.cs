using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Настроение пешки и порог психологического срыва.
/// Когда <c>Mood &lt; MoodBreakThreshold</c> — MoodSystem публикует MoodBreakEvent.
/// См. GDD раздел психологии колонии.
/// </summary>
public sealed class MindComponent : IComponent
{
    // TODO: public float Mood;
    // TODO: public int MoodBreakThreshold;
}
