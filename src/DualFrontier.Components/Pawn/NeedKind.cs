namespace DualFrontier.Components.Pawn;

/// <summary>
/// Identifies one of a pawn's core biological needs. Used by
/// <c>NeedsCriticalEvent</c> to tell subscribers which need crossed the
/// critical threshold without a string comparison.
/// </summary>
public enum NeedKind
{
    /// <summary>Hunger — resolved by eating.</summary>
    Hunger,

    /// <summary>Thirst — resolved by drinking.</summary>
    Thirst,

    /// <summary>Rest/energy — resolved by sleeping.</summary>
    Rest,

    /// <summary>Comfort — resolved by environmental improvements or rest.</summary>
    Comfort
}
