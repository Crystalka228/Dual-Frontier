using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using DualFrontier.Events.Pawn;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Container node for item visuals. Parallel to <see cref="PawnLayer"/>.
/// Items are static after spawn — no MoveItem method. <see cref="RemoveItem"/>
/// is prepared for M9+ destruction mechanism (currently not invoked in the
/// M8 batch per the Charges=0 cosmetic limitation).
/// </summary>
public partial class ItemLayer : Node2D
{
	public const int TileSize = 16;

	private readonly Dictionary<EntityId, ItemVisual> _visuals = new();

	public void SpawnItem(EntityId id, float x, float y, ItemKind kind)
	{
		if (_visuals.ContainsKey(id)) return;
		var visual = new ItemVisual
		{
			EntityId = id,
			Kind     = kind,
			Position = new Vector2(x * TileSize, y * TileSize)
		};
		AddChild(visual);
		_visuals[id] = visual;
	}

	public void RemoveItem(EntityId id)
	{
		if (_visuals.TryGetValue(id, out ItemVisual? visual))
		{
			visual.QueueFree();
			_visuals.Remove(id);
		}
	}
}
