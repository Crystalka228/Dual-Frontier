#if TOOLS
using Godot;

namespace DualFrontier.Presentation.DevKit.Export;

/// <summary>
/// Walks a Godot SceneTree and produces a <c>.dfscene</c> JSON file. The
/// full implementation is scheduled for Phase 3.5; this stub documents the
/// expected algorithm and signature.
/// </summary>
[Tool]
public static class SceneExporter
{
    /// <summary>
    /// Exports the given root node into a <c>.dfscene</c> file at
    /// <paramref name="outputPath"/>. Walks the tree, collecting TileMap
    /// nodes as tilemap layers, nodes with <see cref="Meta.DFEntityMeta"/>
    /// as entity spawns, and nodes whose name begins with "Marker_" as
    /// named markers.
    /// </summary>
    public static void Export(Node root, string outputPath)
    {
        // TODO Phase 3.5: tree walk, TilemapExporter, EntityExporter.
        // TODO Phase 3.5: serialise SceneDef into JSON with camelCase.
        GD.PushWarning("[DF DevKit] SceneExporter.Export not yet implemented.");
    }
}
#endif
