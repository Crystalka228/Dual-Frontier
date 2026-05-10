namespace DualFrontier.Components.Pawn;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

/// <summary>
/// Represents a component managing the pawn's mental state, including mood and breakdown status.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct MindComponent : IComponent
{
    // --- Constants ---

    /// <summary>The default threshold for triggering a mental breakdown (0.3f).</summary>
    public const float DefaultBreakThreshold = 0.3f;

    /// <summary>Mood level considered ecstatic (0.8f).</summary>
    public const float EcstaticThreshold = 0.8f;

    /// <summary>Mood level considered content enough to avoid major stress (0.6f).</summary>
    public const float ContentThreshold = 0.6f;

    // --- Public Fields ---

    /// <summary>The current mood value of the pawn, ranging from 0f (miserable) to 1f (ecstatic). Defaults to 0.5f (neutral).</summary>
    public float Mood = 0.5f;

    /// <summary>Mood level at which a breakdown triggers. Defaults to 0.3f.</summary>
    public float MoodBreakThreshold = DefaultBreakThreshold;

    /// <summary>True when the pawn is currently experiencing a mental break.</summary>
    public bool IsInBreakdown;

    /// <summary>Ticks remaining in current breakdown. 0 when not breaking.</summary>
    public int BreakdownTicksRemaining;

    /// <summary>Parameterless ctor required so field initializers run on `new MindComponent()`.</summary>
    public MindComponent() { }


    // --- Expression-bodied Properties ---

    /// <summary>Gets true if the pawn's mood is above the content threshold.</summary>
    public bool IsHappy => Mood >= ContentThreshold;

    /// <summary>Gets true if the pawn's mood is ecstatic enough to be considered such.</summary>
    public bool IsEcstatic => Mood >= EcstaticThreshold;

    /// <summary>Gets true if the pawn's mood indicates it is at risk of a breakdown.</summary>
    public bool IsAtRisk => Mood <= MoodBreakThreshold + 0.15f;
}
