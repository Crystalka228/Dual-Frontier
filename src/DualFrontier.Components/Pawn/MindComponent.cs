namespace DualFrontier.Components.Pawn;

using DualFrontier.Contracts.Core;

/// <summary>
/// Represents a component managing the pawn's mental state, including mood and breakdown status.
/// </summary>
public sealed class MindComponent : IComponent
{
    // --- Constants ---

    /// <summary>The default threshold for triggering a mental breakdown (-0.7f).</summary>
    public const float DefaultBreakThreshold = -0.7f;

    /// <summary>Mood level considered ecstatic (0.8f).</summary>
    public const float EcstaticThreshold = 0.8f;

    /// <summary>Mood level considered content enough to avoid major stress (0.2f).</summary>
    public const float ContentThreshold = 0.2f;

    // --- Public Fields ---

    /// <summary>The current mood value of the pawn, ranging from -1f (miserable) to 1f (ecstatic).</summary>
    public float Mood { get; set; } = 0f;

    /// <summary>Mood level at which a breakdown triggers. Defaults to -0.7f.</summary>
    public float MoodBreakThreshold { get; set; } = DefaultBreakThreshold;

    /// <summary>True when the pawn is currently experiencing a mental break.</summary>
    public bool IsInBreakdown { get; set; } = false;

    /// <summary>Ticks remaining in current breakdown. 0 when not breaking.</summary>
    public int BreakdownTicksRemaining { get; set; } = 0;


    // --- Expression-bodied Properties ---

    /// <summary>Gets true if the pawn's mood is above the content threshold.</summary>
    public bool IsHappy => Mood >= ContentThreshold;

    /// <summary>Gets true if the pawn's mood is ecstatic enough to be considered such.</summary>
    public bool IsEcstatic => Mood >= EcstaticThreshold;

    /// <summary>Gets true if the pawn's mood indicates it is at risk of a breakdown.</summary>
    public bool IsAtRisk => Mood <= MoodBreakThreshold + 0.15f;
}