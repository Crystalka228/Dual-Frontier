using DualFrontier.Application.Bridge;
using DualFrontier.Application.Loop;
using DualFrontier.Presentation.Rendering;
using DualFrontier.Presentation.UI;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Root game scene node. On <c>_Ready</c> it wires the presentation bridge
/// and starts the simulation loop; on every <c>_Process</c> it drains the
/// bridge onto the Godot main thread via <see cref="RenderCommandDispatcher"/>.
/// This is the only node that knows about <c>GameLoop</c> and
/// <see cref="PresentationBridge"/>.
/// </summary>
public partial class GameRoot : Node2D
{
    private const int MapWidth  = 50;
    private const int MapHeight = 50;

    private PresentationBridge _bridge = null!;
    private GameLoop _loop = null!;
    private RenderCommandDispatcher _dispatcher = null!;
    private GameHUD _hud = null!;

    public override void _Ready()
    {
        _bridge = new PresentationBridge();

        var tileMap   = GetNode<TileMapRenderer>("TileMapRenderer");
        var pawnLayer = GetNode<PawnLayer>("PawnLayer");
        _hud          = GetNode<GameHUD>("GameHUD");

        _dispatcher = new RenderCommandDispatcher(pawnLayer, _hud);

        tileMap.InitMap(MapWidth, MapHeight);

        _loop = GameBootstrap.CreateLoop(_bridge);
        _loop.Start();
    }

    public override void _Process(double delta)
    {
        _bridge.DrainCommands(_dispatcher.Dispatch);
    }

    public override void _ExitTree()
    {
        _loop.Stop();
    }
}
