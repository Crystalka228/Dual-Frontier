using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DualFrontier.Application.Scene;

/// <summary>
/// Dimensions and layers of a tilemap. Tiles are stored per-layer as a flat
/// <c>ushort[]</c> of length <c>Width * Height</c>, row-major order.
/// </summary>
public sealed record TilemapDef(
    int                                 Width,
    int                                 Height,
    int                                 TileSize,
    IReadOnlyList<TilemapLayerDef>      Layers
)
{
    /// <summary>
    /// Singleton empty tilemap used when a scene has no map authored.
    /// </summary>
    public static readonly TilemapDef Empty =
        new(0, 0, 32, System.Array.Empty<TilemapLayerDef>());
}

/// <summary>
/// A single named tilemap layer. Tile values are opaque <c>ushort</c> IDs
/// resolved against a tileset registered at runtime. Serialised as base64
/// via <see cref="UshortArrayBase64JsonConverter"/>.
/// </summary>
public sealed record TilemapLayerDef(
    string Id,
    [property: JsonConverter(typeof(UshortArrayBase64JsonConverter))] ushort[] Tiles);
