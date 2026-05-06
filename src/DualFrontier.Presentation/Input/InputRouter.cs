using Godot;

namespace DualFrontier.Presentation.Input;

/// <summary>
/// Routes Godot input events to the simulation layer. Currently:
/// — ESC quits the game.
/// — Middle mouse button drag pans the active Camera2D (M8.2.A).
/// Phase 5+ adds clicks, build hotkeys, and additional camera controls.
/// </summary>
public partial class InputRouter : Node
{
    private Camera2D? _camera;
    private bool _isPanning;
    private Vector2 _panStartMousePos;
    private Vector2 _panStartCameraPos;

    /// <summary>
    /// Plumbs the current Camera2D so middle-mouse drag can pan it.
    /// Called from <c>GameRoot._Ready</c> after the camera is constructed.
    /// </summary>
    public void SetCamera(Camera2D camera) => _camera = camera;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true, Keycode: Key.Escape })
        {
            GetTree().Quit();
            return;
        }

        if (_camera is null) return;

        if (@event is InputEventMouseButton mb &&
            mb.ButtonIndex == MouseButton.Middle)
        {
            if (mb.Pressed)
            {
                _isPanning = true;
                _panStartMousePos  = mb.Position;
                _panStartCameraPos = _camera.Position;
            }
            else
            {
                _isPanning = false;
            }
            GetViewport().SetInputAsHandled();
            return;
        }

        if (_isPanning && @event is InputEventMouseMotion mm)
        {
            // Drag the world: camera moves in opposite direction of cursor.
            // Mouse moves right by D → world appears to move left → camera
            // moves left to keep cursor over the same world point.
            var delta = mm.Position - _panStartMousePos;
            _camera.Position = _panStartCameraPos - delta;
            GetViewport().SetInputAsHandled();
        }
    }
}
