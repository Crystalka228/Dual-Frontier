using DualFrontier.Contracts.Core;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Visual representation of a pawn. Phase 3: coloured square.
/// Phase 5+: sprite with animations. Stores <see cref="EntityId"/> so the
/// presentation layer can correlate with the Domain entity for picks,
/// tooltips and selection.
/// </summary>
public partial class PawnVisual : Node2D
{
	private const int Size = 16;
	private static readonly Color FillColor = new(0.2f, 0.5f, 1.0f);

	public EntityId EntityId { get; set; }

	public override void _Draw()
	{
		DrawRect(new Rect2(-Size / 2f, -Size / 2f, Size, Size), FillColor);
	}
}
