namespace DualFrontier.Persistence.Snapshots;

/// <summary>
/// Per-pawn data carried in a save. Floats are unquantised here — quantisation
/// happens during encoding by <c>ComponentEncoder</c>.
/// </summary>
public sealed record PawnSnapshot
{
    /// <summary>Index portion of the original <c>EntityId</c>.</summary>
    public required int EntityIndex { get; init; }

    /// <summary>Tile-grid X coordinate.</summary>
    public required int X { get; init; }

    /// <summary>Tile-grid Y coordinate.</summary>
    public required int Y { get; init; }

    /// <summary>Satiety 0..1 (wellness semantics from <c>NeedsComponent</c>; 1 = full).</summary>
    public required float Satiety { get; init; }

    /// <summary>Hydration 0..1 (1 = full).</summary>
    public required float Hydration { get; init; }

    /// <summary>Sleep 0..1 (1 = fully rested).</summary>
    public required float Sleep { get; init; }

    /// <summary>Comfort 0..1 (1 = comfortable).</summary>
    public required float Comfort { get; init; }

    /// <summary>Mood 0..1 from <c>MindComponent</c> (1 = ecstatic).</summary>
    public required float Mood { get; init; }

    /// <summary>Current job kind, stored as the enum's string name for forward-compat.</summary>
    public required string JobKind { get; init; }
}
