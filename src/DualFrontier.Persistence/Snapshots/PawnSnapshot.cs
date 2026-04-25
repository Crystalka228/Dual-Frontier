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

    /// <summary>Hunger 0..1 (deficit semantics from <c>NeedsComponent</c>).</summary>
    public required float Hunger { get; init; }

    /// <summary>Thirst 0..1.</summary>
    public required float Thirst { get; init; }

    /// <summary>Rest deficit 0..1.</summary>
    public required float Rest { get; init; }

    /// <summary>Comfort deficit 0..1.</summary>
    public required float Comfort { get; init; }

    /// <summary>Mood 0..1 from <c>MindComponent</c> (1 = ecstatic).</summary>
    public required float Mood { get; init; }

    /// <summary>Current job kind, stored as the enum's string name for forward-compat.</summary>
    public required string JobKind { get; init; }
}
