using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Container node for all pawn visuals.
/// Manages spawn, movement and removal of <see cref="PawnVisual"/> nodes.
/// Referenced by <c>RenderCommandDispatcher</c> on the main thread only.
/// </summary>
public partial class PawnLayer : Node2D
{
    public const int TileSize = 16;

    private readonly Dictionary<EntityId, PawnVisual> _visuals = new();

    public void SpawnPawn(EntityId id, float x, float y)
    {
        if (_visuals.ContainsKey(id)) return;
        var visual = new PawnVisual
        {
            EntityId = id,
            Position = new Vector2(x * TileSize, y * TileSize)
        };
        AddChild(visual);
        _visuals[id] = visual;
    }

    public void MovePawn(EntityId id, float x, float y)
    {
        if (_visuals.TryGetValue(id, out PawnVisual? visual))
            visual.Position = new Vector2(x * TileSize, y * TileSize);
    }

    public void RemovePawn(EntityId id)
    {
        if (_visuals.TryGetValue(id, out PawnVisual? visual))
        {
            visual.QueueFree();
            _visuals.Remove(id);
        }
    }
}
