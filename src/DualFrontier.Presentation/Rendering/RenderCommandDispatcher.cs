using DualFrontier.Application.Bridge;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Presentation.Nodes;

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

    public RenderCommandDispatcher(PawnLayer pawnLayer)
    {
        _pawnLayer = pawnLayer;
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
        }
    }
}
