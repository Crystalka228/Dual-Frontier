using DualFrontier.Contracts.Core;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Visual representation of a pawn. M8.9 replaces the prior coloured square
/// (DrawRect) with a sprite from the shared Kenney atlas (M8.8). Stores
/// <see cref="EntityId"/> so the presentation layer can correlate with the
/// Domain entity for picks, tooltips and selection.
///
/// Pack: Roguelike RPG Pack (CC0). Coordinates identified by Pillow
/// programmatic inspection (see tools/m8.9-atlas-inspect/) per the M8.8
/// methodology. A single pawn sprite is shared across all 50 pawns; per-pawn
/// variation is future work.
/// </summary>
public partial class PawnVisual : Node2D
{
	private const int TileSize = 16;

	// Pawn sprite atlas region (col 52, row 16 in the atlas).
	// Stride 17, so offset = (52*17, 16*17) = (884, 272). Renders a hooded
	// humanoid figure — the closest character sprite in the pack.
	private static readonly Rect2 PawnRegion = new Rect2(884, 272, TileSize, TileSize);

	private const string AtlasPath =
		"res://assets/kenney/terrain/roguelikeSheet_transparent.png";

	private static readonly Color FallbackColor = new(0.2f, 0.5f, 1.0f);

	private Texture2D? _atlas;

	public EntityId EntityId { get; set; }

	public override void _Ready()
	{
		_atlas = GD.Load<Texture2D>(AtlasPath);
		if (_atlas is null)
			GD.PrintErr($"PawnVisual: failed to load atlas at {AtlasPath}");
		QueueRedraw();
	}

	public override void _Draw()
	{
		Rect2 dstRect = new Rect2(-TileSize / 2f, -TileSize / 2f, TileSize, TileSize);
		if (_atlas is null)
		{
			// Defensive fallback to the prior coloured rectangle if atlas load failed.
			DrawRect(dstRect, FallbackColor);
			return;
		}
		DrawTextureRectRegion(_atlas, dstRect, PawnRegion);
	}
}
