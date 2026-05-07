using DualFrontier.Components.World;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Renders the terrain grid using a Kenney CC0 sprite atlas. M8.8 replaced
/// the prior DrawRect placeholder with DrawTextureRectRegion calls into the
/// pre-loaded atlas texture. Grass + Rock are mapped to specific 16x16
/// regions identified manually by image-editor inspection.
///
/// Atlas pack: Roguelike RPG Pack by Kenney (kenney.nl), CC0 1.0 Universal.
/// See assets/kenney/terrain/LICENSE.txt for full attribution. The pack uses
/// 16x16 tiles with a 1px margin between them (stride = 17px).
///
/// TerrainKind values not present in the atlas (Water/Sand/Ice/Swamp/Arcane)
/// fall back to KindToColor flat-fill. InitMap currently uses only Grass +
/// Rock, so the fallback is defensive — present for future biome expansion.
/// </summary>
public partial class TileMapRenderer : Node2D
{
	public const int TileSize = 16;
	private const int DefaultSeed = 42;

	// Atlas sub-regions verified by inspection of roguelikeSheet_transparent.png.
	// Pack stride = 17px (16 tile + 1 margin), so col*17, row*17 gives top-left.
	//   Grass = col 5,  row 0 → (85, 0)    — solid green floor tile.
	//   Rock  = col 6,  row 2 → (102, 34)  — grey stone-brick floor tile.
	private static readonly Rect2 GrassRegion = new Rect2(85, 0, TileSize, TileSize);
	private static readonly Rect2 RockRegion  = new Rect2(102, 34, TileSize, TileSize);

	private const string AtlasPath =
		"res://assets/kenney/terrain/roguelikeSheet_transparent.png";

	private int _width;
	private int _height;
	private TerrainKind[] _tiles = System.Array.Empty<TerrainKind>();
	private Texture2D? _atlas;

	public int Seed { get; set; } = DefaultSeed;

	public override void _Ready()
	{
		_atlas = GD.Load<Texture2D>(AtlasPath);
		if (_atlas is null)
		{
			GD.PrintErr($"TileMapRenderer: failed to load atlas at {AtlasPath}");
		}
	}

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
				TerrainKind kind = _tiles[y * _width + x];
				Rect2 dstRect = new Rect2(x * TileSize, y * TileSize, TileSize, TileSize);

				if (_atlas != null && TryGetAtlasRegion(kind, out Rect2 srcRegion))
				{
					DrawTextureRectRegion(_atlas, dstRect, srcRegion);
				}
				else
				{
					// Fallback for unmapped TerrainKind values or missing atlas
					// (defensive — InitMap uses only Grass + Rock, but other
					// biomes may join in future commits).
					DrawRect(dstRect, KindToColor(kind));
				}
			}
		}
	}

	private static bool TryGetAtlasRegion(TerrainKind kind, out Rect2 region)
	{
		switch (kind)
		{
			case TerrainKind.Grass:
				region = GrassRegion;
				return true;
			case TerrainKind.Rock:
				region = RockRegion;
				return true;
			default:
				region = default;
				return false;
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
