using DualFrontier.Components.World;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Renders the terrain grid as coloured rectangles. Phase 3 placeholder
/// that paints a deterministic grass/rock pattern seeded by <see cref="Seed"/>.
/// Phase 7+ replaces this with a Godot TileSet driven by real terrain data
/// from <c>TileComponent</c>.
/// </summary>
public partial class TileMapRenderer : Node2D
{
    public const int TileSize = 16;
    private const int DefaultSeed = 42;

    private int _width;
    private int _height;
    private TerrainKind[] _tiles = System.Array.Empty<TerrainKind>();

    public int Seed { get; set; } = DefaultSeed;

    /// <summary>
    /// Initialises the tile map with a procedural grass/rock pattern.
    /// Called once from <c>GameRoot._Ready</c> before the simulation starts.
    /// </summary>
    public void InitMap(int width, int height)
    {
        _width  = width;
        _height = height;
        _tiles  = new TerrainKind[width * height];

        var rng = new RandomNumberGenerator { Seed = (ulong)Seed };
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i] = rng.Randf() < 0.85f ? TerrainKind.Grass : TerrainKind.Rock;

        QueueRedraw();
    }

    public override void _Draw()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Color color = KindToColor(_tiles[y * _width + x]);
                DrawRect(new Rect2(x * TileSize, y * TileSize, TileSize, TileSize), color);
            }
        }
    }

    private static Color KindToColor(TerrainKind kind) => kind switch
    {
        TerrainKind.Grass  => new Color(0.30f, 0.60f, 0.20f),
        TerrainKind.Rock   => new Color(0.50f, 0.50f, 0.50f),
        TerrainKind.Water  => new Color(0.20f, 0.40f, 0.80f),
        TerrainKind.Sand   => new Color(0.80f, 0.70f, 0.40f),
        TerrainKind.Ice    => new Color(0.75f, 0.90f, 0.95f),
        TerrainKind.Swamp  => new Color(0.35f, 0.40f, 0.25f),
        TerrainKind.Arcane => new Color(0.55f, 0.25f, 0.75f),
        _                  => new Color(0.40f, 0.40f, 0.40f)
    };
}
