namespace DualFrontier.Components.Pawn;

using DualFrontier.Contracts.Core;

/// <summary>
/// Represents the core biological needs of a pawn, including hunger, thirst, rest, and comfort levels.
/// This component is designed as a Pure POCO to hold state data without internal logic.
/// </summary>
public sealed class NeedsComponent : IComponent
{
    /// <summary>
    /// The current hunger level (0 = full, 1 = starving).
    /// </summary>
    public float Hunger { get; set; }

    /// <summary>
    /// The current thirst level (0 = full, 1 = dehydrated).
    /// </summary>
    public float Thirst { get; set; }

    /// <summary>
    /// The current rest/energy level (0 = fully rested, 1 = exhausted).
    /// </summary>
    public float Rest { get; set; }

    /// <summary>
    /// The current comfort level (0 = comfortable, 1 = miserable).
    /// </summary>
    public float Comfort { get; set; }

    // Constants defining critical thresholds for needs warnings.
    
    /// <summary>
    /// Value above which the NeedsSystem publishes a critical warning (e.g., 0.8f).
    /// </summary>
    public const float CriticalThreshold = 0.8f;

    /// <summary>
    /// Value above which a mood break may trigger (e.g., 0.95f).
    /// </summary>
    public const float BreakThreshold = 0.95f;

    /// <inheritdoc/>
    /// <summary>Checks if the pawn is considered hungry based on critical thresholds.</summary>
    public bool IsHungry => Hunger >= CriticalThreshold;

    /// <inheritdoc/>
    /// <summary>Checks if the pawn is considered thirsty based on critical thresholds.</summary>
    public bool IsThirsty => Thirst >= CriticalThreshold;

    /// <inheritdoc/>
    /// <summary>Checks if the pawn is considered exhausted based on critical thresholds.</summary>
    public bool IsExhausted => Rest >= CriticalThreshold;
}