namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Severity level for messages emitted by a mod through
/// <see cref="IModApi.Log(ModLogLevel, string)"/>.
/// </summary>
public enum ModLogLevel
{
    /// <summary>Verbose diagnostic detail; suppressed in release builds by default.</summary>
    Debug,

    /// <summary>Informational message describing normal operation.</summary>
    Info,

    /// <summary>Warning that does not prevent the operation from completing.</summary>
    Warning,

    /// <summary>Error indicating a failure the mod could not recover from.</summary>
    Error,
}
