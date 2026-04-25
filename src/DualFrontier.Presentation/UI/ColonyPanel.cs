using System;
using DualFrontier.Contracts.Core;
using Godot;

namespace DualFrontier.Presentation.UI;

/// <summary>
/// Left-edge colony roster: list of pawns with mini status, click-to-select.
/// Skeletal in commit 1 — full Grimdark layout lands in commit 2.
/// </summary>
public partial class ColonyPanel : Panel
{
    public event Action<EntityId>? PawnSelected;

    public override void _Ready()
    {
        AnchorTop    = 0;
        AnchorBottom = 1;
        OffsetLeft   = 0;
        OffsetRight  = 180;
        OffsetTop    = 0;
        OffsetBottom = 0;
    }

    internal void RaisePawnSelected(EntityId id) => PawnSelected?.Invoke(id);
}
