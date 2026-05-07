using DualFrontier.Contracts.Core;
using DualFrontier.Events.Pawn;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Visual representation of an item entity. Renders a sprite from the shared
/// Kenney atlas (M8.8) using DrawTextureRectRegion with a per-Kind region.
///
/// Pack: Roguelike RPG Pack (CC0). Atlas stride 17px (16 tile + 1 margin), so
/// region offsets are <c>col*17, row*17</c>. Coordinates were identified via
/// Pillow programmatic inspection (see tools/m8.9-atlas-inspect/) per the
/// M8.8 methodology — sample center RGB of every tile, generate zoomed
/// contact sheets, verify visual identity manually.
/// </summary>
public partial class ItemVisual : Node2D
{
	private const int TileSize = 16;

	// Atlas regions identified by Pillow inspection of the Roguelike RPG Pack.
	// All offsets follow stride=17 (col*17, row*17, 16, 16):
	//   Food       = col 54, row 15 → roast meat on plate.
	//   Water      = col  0, row  0 → solid water tile (top-left of atlas).
	//   Bed        = col 25, row  2 → bedroll/sleeping mat with visible pillow.
	//   Decoration = col  0, row  6 → orange flowers on grass.
	private static readonly Rect2 FoodRegion       = new Rect2(918, 255, TileSize, TileSize);
	private static readonly Rect2 WaterRegion      = new Rect2(  0,   0, TileSize, TileSize);
	private static readonly Rect2 BedRegion        = new Rect2(425,  34, TileSize, TileSize);
	private static readonly Rect2 DecorationRegion = new Rect2(  0, 102, TileSize, TileSize);

	private const string AtlasPath =
		"res://assets/kenney/terrain/roguelikeSheet_transparent.png";

	private Texture2D? _atlas;

	public EntityId EntityId { get; set; }
	public ItemKind Kind     { get; set; }

	public override void _Ready()
	{
		_atlas = GD.Load<Texture2D>(AtlasPath);
		if (_atlas is null)
			GD.PrintErr($"ItemVisual: failed to load atlas at {AtlasPath}");
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (_atlas is null) return;
		Rect2 srcRegion = KindToRegion(Kind);
		Rect2 dstRect   = new Rect2(-TileSize / 2f, -TileSize / 2f, TileSize, TileSize);
		DrawTextureRectRegion(_atlas, dstRect, srcRegion);
	}

	private static Rect2 KindToRegion(ItemKind kind) => kind switch
	{
		ItemKind.Food       => FoodRegion,
		ItemKind.Water      => WaterRegion,
		ItemKind.Bed        => BedRegion,
		ItemKind.Decoration => DecorationRegion,
		_                   => FoodRegion,
	};
}
