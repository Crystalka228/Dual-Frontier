using System.Collections.Generic;

namespace DualFrontier.Persistence.Snapshots;

/// <summary>
/// Per-storage data carried in a save. Item ids are kept as strings here;
/// <c>StringPool</c> dedupes them at encode time.
/// </summary>
public sealed record StorageSnapshot
{
    /// <summary>Index portion of the storage entity's <c>EntityId</c>.</summary>
    public required int EntityIndex { get; init; }

    /// <summary>Stack capacity copied from <c>StorageComponent.Capacity</c>.</summary>
    public required int Capacity { get; init; }

    /// <summary>Stored item stacks: item template id → quantity.</summary>
    public required IReadOnlyDictionary<string, int> Items { get; init; }
}
