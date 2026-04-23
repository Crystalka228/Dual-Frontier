using System.Collections.Generic;

namespace DualFrontier.Application.Scene;

/// <summary>
/// Engine-neutral representation of a game scene: tilemap, entity spawns,
/// named markers, and metadata. Produced by the Godot DevKit plugin or
/// authored by hand; consumed by any <see cref="ISceneLoader"/>
/// implementation.
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
