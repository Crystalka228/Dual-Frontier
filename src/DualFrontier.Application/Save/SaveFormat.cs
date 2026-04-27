namespace DualFrontier.Application.Save;

/// <summary>
/// Save format version and header. When <see cref="Version"/> is incompatible,
/// the save is considered old and must go through migration (TODO: Phase 3).
/// </summary>
public sealed class SaveFormat
{
    /// <summary>
    /// TODO: Phase 1 — current format version. Incremented on any
    /// incompatible serialisation change.
    /// </summary>
    public int Version { get; init; }

    /// <summary>
    /// TODO: Phase 1 — magic file header (e.g. "DFSAVE\0") so a save can be
    /// quickly distinguished from an arbitrary binary.
    /// </summary>
    public string Header { get; init; } = "DFSAVE";
}
