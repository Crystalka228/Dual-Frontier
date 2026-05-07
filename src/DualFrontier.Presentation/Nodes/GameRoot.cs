using DualFrontier.Application.Bridge;
using DualFrontier.Application.Loop;
using DualFrontier.Application.Modding;
using DualFrontier.Presentation.Input;
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
    private const int MapWidth  = 200;
    private const int MapHeight = 200;

    private PresentationBridge _bridge = null!;
    private GameLoop _loop = null!;
    private RenderCommandDispatcher _dispatcher = null!;
    private GameHUD _hud = null!;

    /// <summary>
    /// Mod-menu controller obtained from <see cref="GameContext"/>. M7.5.B.1
    /// surfaces the field via the bootstrap return shape; M7.5.B.2 binds it
    /// to the Godot mod-menu scene so the user can drive the §9.2
    /// Pause-Toggle-Apply-Resume sequence. The field is held here rather
    /// than reconstructed inside the menu scene so the menu cannot
    /// accidentally instantiate a second pipeline.
    /// </summary>
    private ModMenuController _modMenuController = null!;

    // M7.5.B.2 — modal mod menu overlay; constructed and wired here in
    // _Ready, toggled by F10 via _UnhandledInput.
    private ModMenuPanel _modMenuPanel = null!;

    public override void _Ready()
    {
        _bridge = new PresentationBridge();

        var tileMap   = GetNode<TileMapRenderer>("TileMapRenderer");
        var itemLayer = GetNode<ItemLayer>("ItemLayer");
        var pawnLayer = GetNode<PawnLayer>("PawnLayer");
        _hud          = GetNode<GameHUD>("GameHUD");

        _dispatcher = new RenderCommandDispatcher(pawnLayer, itemLayer, _hud);

        _hud.SetupDebugOverlay(_bridge);

        tileMap.InitMap(MapWidth, MapHeight);

        // M8.2.A — add Camera2D centered on map. Without a Camera2D, the
        // 2D viewport defaults to world coordinates (0,0)..(viewport_size),
        // so a 200×200×16 = 3200px map is only ~36% visible. Centering the
        // camera on (mapPixelW/2, mapPixelH/2) gives the user a sensible
        // initial viewpoint; InputRouter handles middle-mouse drag to pan.
        var camera = new Camera2D
        {
            Position = new Vector2(MapWidth * TileMapRenderer.TileSize / 2f,
                                   MapHeight * TileMapRenderer.TileSize / 2f),
            Enabled = true,
        };
        AddChild(camera);
        camera.MakeCurrent();

        var inputRouter = GetNode<InputRouter>("InputRouter");
        inputRouter.SetCamera(camera);

        GameContext context = GameBootstrap.CreateLoop(_bridge);
        _loop = context.Loop;
        _modMenuController = context.Controller;

        _modMenuPanel = new ModMenuPanel();
        AddChild(_modMenuPanel);
        _modMenuPanel.Setup(_modMenuController);

        _loop.Start();
    }

    public override void _Process(double delta)
    {
        _bridge.DrainCommands(_dispatcher.Dispatch);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true, Keycode: Key.F10 })
        {
            if (_modMenuPanel.Visible)
                _modMenuPanel.CloseAndCancel();
            else
                _modMenuPanel.OpenAndBegin();
            GetViewport().SetInputAsHandled();
        }
    }

    /// <summary>
    /// M8.10 — couples Godot focus loss to <see cref="GameLoop.SetPaused"/>.
    /// GameLoop runs on a dedicated background thread decoupled from
    /// Godot's pause semantics; without this hook, alt-tab / window focus
    /// loss stops the main thread <c>_Process</c> (which drains the bridge)
    /// while the simulation thread keeps producing events at 30 TPS,
    /// causing <see cref="PresentationBridge"/> queue accumulation and
    /// frame stutter on resume. Null-conditional guards against
    /// pre-_Ready notifications during scene initialization.
    /// </summary>
    public override void _Notification(int what)
    {
        // Godot 4.6.1 inconsistency: parameter is int, but constants like
        // NotificationApplicationFocusOut are typed long. Use if-else with
        // == (int implicitly promotes to long) instead of switch (which
        // requires narrowing the long constants to int and emits CS0266).
        if (what == NotificationApplicationFocusOut ||
            what == NotificationWMWindowFocusOut)
        {
            _loop?.SetPaused(true);
        }
        else if (what == NotificationApplicationFocusIn ||
                 what == NotificationWMWindowFocusIn)
        {
            _loop?.SetPaused(false);
        }
    }

    public override void _ExitTree()
    {
        _loop.Stop();
    }
}
