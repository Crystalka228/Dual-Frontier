using System.Collections.Generic;

namespace DualFrontier.Application.Scene;

/// <summary>
/// Engine-neutral representation of a game scene: tilemap, entity spawns,
/// named markers, and metadata. Authored by hand (the prior Godot DevKit
/// plugin producer was retired with К-extensions cascade #2); consumed by
/// any <see cref="ISceneLoader"/> implementation.
/// </summary>
public sealed record SceneDef(
    int                              Version,
    string                           Name,
    TilemapDef                       Tilemap,
    IReadOnlyList<EntitySpawnDef>    Entities,
    IReadOnlyList<MarkerDef>         Markers,
    SceneMetadata                    Metadata
)
{
    /// <summary>
    /// Current supported <c>.dfscene</c> format version. Incremented on any
    /// breaking change; older versions require explicit migration.
    /// </summary>
    public const int CurrentVersion = 1;
}
