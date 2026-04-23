using System;

namespace DualFrontier.Application.Scene;

/// <summary>
/// Free-form scene metadata: biome, ether density, authorship info.
/// Not used by the simulation — consumed by the UI and debugging tools.
/// </summary>
public sealed record SceneMetadata(
    string         Biome,
    float          EtherDensity,
    string         CreatedBy,
    DateTimeOffset ExportedAt
);
