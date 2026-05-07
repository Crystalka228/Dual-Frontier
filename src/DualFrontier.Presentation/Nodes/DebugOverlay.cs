using DualFrontier.Application.Bridge;
using Godot;

namespace DualFrontier.Presentation.Nodes;

/// <summary>
/// Debug overlay showing FPS, current simulation tick, and
/// PresentationBridge queue depth. Top-right corner of the screen.
/// Toggle visibility with F3 key.
///
/// Surfaces diagnostic data exposing performance characteristics:
/// - FPS drops indicate render-bound condition.
/// - BridgeDepth growth indicates simulation/render desync
///   (production exceeds drainage).
/// - Tick continues even during visual pause indicates pause coupling
///   broken (M8.10 fix should prevent this).
/// </summary>
public partial class DebugOverlay : Label
{
    private PresentationBridge? _bridge;
    private int _currentTick;

    /// <summary>
    /// Wires the bridge reference. Called from GameRoot._Ready after bridge
    /// construction. Stored as nullable for defensive initialization order.
    /// </summary>
    public void Setup(PresentationBridge bridge)
    {
        _bridge = bridge;
    }

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight);
        OffsetLeft  = -250;
        OffsetTop   = 8;
        OffsetRight = -8;
        AddThemeColorOverride("font_color", new Color(1f, 1f, 0f, 0.85f));
        AddThemeFontSizeOverride("font_size", 14);
        Text = "FPS: -- | Tick: -- | Queue: --";
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;

        int fps   = (int)Engine.GetFramesPerSecond();
        int queue = _bridge?.QueueDepth ?? 0;

        Text = $"FPS: {fps} | Tick: {_currentTick} | Queue: {queue}";
    }

    /// <summary>
    /// Updates current tick for display. Called from GameHUD.SetTick (parallel
    /// to existing ColonyPanel wiring).
    /// </summary>
    public void SetTick(int tick) => _currentTick = tick;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true, Keycode: Key.F3 })
        {
            Visible = !Visible;
            GetViewport().SetInputAsHandled();
        }
    }
}
