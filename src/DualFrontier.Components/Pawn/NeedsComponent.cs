namespace DualFrontier.Components.Pawn;

using DualFrontier.Contracts.Core;

/// <summary>
/// Wellness pool for a pawn: satiety, hydration, sleep, and comfort. Each
/// value is normalised to [0..1] where 1 = best (fully fed / hydrated /
/// rested / comfortable) and 0 = worst (starving / dehydrated / exhausted /
/// miserable). Pure POCO — no internal logic beyond the threshold queries.
/// </summary>
public struct NeedsComponent : IComponent
{
    /// <summary>
    /// The current satiety level (0 = starving, 1 = full).
    /// </summary>
    public float Satiety;

    /// <summary>
    /// The current hydration level (0 = dehydrated, 1 = full).
    /// </summary>
    public float Hydration;

    /// <summary>
    /// The current sleep level (0 = sleep-deprived, 1 = fully rested).
    /// </summary>
    public float Sleep;

    /// <summary>
    /// The current comfort level (0 = miserable, 1 = comfortable).
    /// </summary>
    public float Comfort;

    // Constants defining critical thresholds for needs warnings.

    /// <summary>
    /// Value at or below which the NeedsSystem publishes a critical warning (e.g., 0.2f).
    /// </summary>
    public const float CriticalThreshold = 0.2f;

    /// <summary>
    /// Value at or below which a mood break may trigger (e.g., 0.05f).
    /// </summary>
    public const float BreakThreshold = 0.05f;

    /// <inheritdoc/>
    /// <summary>Checks if the pawn is considered hungry based on critical thresholds.</summary>
    public bool IsHungry => Satiety <= CriticalThreshold;

    /// <inheritdoc/>
    /// <summary>Checks if the pawn is considered thirsty based on critical thresholds.</summary>
    public bool IsThirsty => Hydration <= CriticalThreshold;

    /// <inheritdoc/>
    /// <summary>Checks if the pawn is considered exhausted based on critical thresholds.</summary>
    public bool IsExhausted => Sleep <= CriticalThreshold;
}
