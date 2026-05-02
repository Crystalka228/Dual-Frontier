using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Presentation.Nodes;
using DualFrontier.Presentation.UI;

namespace DualFrontier.Presentation.Rendering;

/// <summary>
/// Translates <see cref="IRenderCommand"/> instances into Godot node operations.
/// Called from <c>GameRoot._Process</c> via
/// <see cref="PresentationBridge.DrainCommands"/>. Lives in Presentation — the
/// only layer allowed to know about Godot nodes.
/// </summary>
public sealed class RenderCommandDispatcher
{
    private readonly PawnLayer _pawnLayer;
    private readonly GameHUD?  _hud;

    public RenderCommandDispatcher(PawnLayer pawnLayer, GameHUD? hud = null)
    {
        _pawnLayer = pawnLayer;
        _hud       = hud;
    }

    public void Dispatch(IRenderCommand command)
    {
        switch (command)
        {
            case PawnSpawnedCommand c:
                _pawnLayer.SpawnPawn(c.PawnId, c.X, c.Y);
                break;
            case PawnMovedCommand c:
                _pawnLayer.MovePawn(c.PawnId, c.X, c.Y);
                break;
            case PawnDiedCommand c:
                _pawnLayer.RemovePawn(c.PawnId);
                break;
            case PawnStateCommand c:
                _hud?.UpdatePawn(c);
                break;
            case TickAdvancedCommand c:
                _hud?.SetTick(c.Tick);
                break;
        }
    }
}
