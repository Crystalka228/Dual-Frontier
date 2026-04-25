using DualFrontier.Application.Bridge.Commands;
using Godot;

namespace DualFrontier.Presentation.UI;

/// <summary>
/// Top-level HUD overlay (CanvasLayer 10). Hosts the colony roster on the
/// left and the per-pawn detail panel on the right. Forwards updates from
/// the render dispatcher to its child panels.
/// Phase 4 — Grimdark Warhammer style.
/// </summary>
public partial class GameHUD : CanvasLayer
{
    private ColonyPanel _colony = null!;
    private PawnDetail  _detail = null!;

    public override void _Ready()
    {
        Layer = 10;
        _colony = GetNode<ColonyPanel>("ColonyPanel");
        _detail = GetNode<PawnDetail>("PawnDetail");
        _colony.PawnSelected += id => _detail.ShowPawn(id);
    }

    public void UpdatePawn(PawnStateCommand cmd)
    {
        _colony.UpdatePawn(cmd.PawnId, cmd.Name, cmd.JobLabel, cmd.Mood);
        _detail.UpdatePawn(
            cmd.PawnId, cmd.Name, cmd.Hunger, cmd.Thirst, cmd.Rest,
            cmd.Comfort, cmd.Mood, cmd.JobLabel, cmd.JobUrgent);
    }

    public void SetTick(int tick) => _colony.SetTick(tick);
}
