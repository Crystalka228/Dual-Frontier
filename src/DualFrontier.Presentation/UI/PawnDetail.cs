using DualFrontier.Contracts.Core;
using Godot;

namespace DualFrontier.Presentation.UI;

/// <summary>
/// Right-edge per-pawn detail card: header, mood, needs, current job, skills.
/// Skeletal in commit 1 — full Grimdark layout lands in commit 2.
/// </summary>
public partial class PawnDetail : Panel
{
    public override void _Ready()
    {
        AnchorLeft   = 1;
        AnchorRight  = 1;
        AnchorTop    = 0;
        AnchorBottom = 1;
        OffsetLeft   = -260;
        OffsetRight  = 0;
        OffsetTop    = 0;
        OffsetBottom = 0;
    }

    public void ShowPawn(EntityId id)
    {
        // Phase 4 commit 2: render full detail card for the selected pawn.
    }
}
