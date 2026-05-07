using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Presentation.Nodes;
using Godot;

namespace DualFrontier.Presentation.UI;

/// <summary>
/// Top-level HUD overlay (CanvasLayer 10). Hosts the colony roster on the
/// left, the per-pawn detail panel on the right, and the M8.10 DebugOverlay
/// in the top-right corner. Forwards updates from the render dispatcher to
/// its child panels.
/// Phase 4 — Grimdark Warhammer style.
/// </summary>
public partial class GameHUD : CanvasLayer
{
    private ColonyPanel  _colony = null!;
    private PawnDetail   _detail = null!;
    private DebugOverlay _debug  = null!;

    public override void _Ready()
    {
        Layer = 10;
        _colony = GetNode<ColonyPanel>("ColonyPanel");
        _detail = GetNode<PawnDetail>("PawnDetail");
        _debug  = GetNode<DebugOverlay>("DebugOverlay");
        _colony.PawnSelected += id => _detail.ShowPawn(id);
    }

    /// <summary>
    /// Wires PresentationBridge reference to DebugOverlay for queue depth
    /// observation. Called from GameRoot._Ready after bridge creation.
    /// </summary>
    public void SetupDebugOverlay(PresentationBridge bridge)
    {
        _debug.Setup(bridge);
    }

    public void UpdatePawn(PawnStateCommand cmd)
    {
        _colony.UpdatePawn(cmd.PawnId, cmd.Name, cmd.JobLabel, cmd.Mood);
        _detail.UpdatePawn(
            cmd.PawnId, cmd.Name, cmd.Satiety, cmd.Hydration, cmd.Sleep,
            cmd.Comfort, cmd.Mood, cmd.JobLabel, cmd.JobUrgent,
            cmd.TopSkills);
    }

    public void SetTick(int tick)
    {
        _colony.SetTick(tick);
        _debug.SetTick(tick);
    }
}
