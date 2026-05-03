namespace DualFrontier.Components.Pawn;

/// <summary>
/// Identifies one of a pawn's wellness pools. Used by
/// <c>NeedsCriticalEvent</c> to tell subscribers which need crossed the
/// critical threshold without a string comparison.
/// </summary>
public enum NeedKind
{
    /// <summary>Satiety — depletes over time; restored by eating.</summary>
    Satiety,

    /// <summary>Hydration — depletes over time; restored by drinking.</summary>
    Hydration,

    /// <summary>Energy — depletes over time; restored by sleeping.</summary>
    Energy,

    /// <summary>Comfort — depletes over time; restored by environmental improvements or rest.</summary>
    Comfort
}
