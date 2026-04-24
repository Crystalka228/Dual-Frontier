using Godot;

namespace DualFrontier.Presentation.Input;

/// <summary>
/// Routes Godot input events to the simulation layer. Phase 3 implements
/// only ESC-to-quit; full routing of clicks, build hotkeys and camera
/// controls lands in Phase 5+.
/// </summary>
public partial class InputRouter : Node
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true, Keycode: Key.Escape })
            GetTree().Quit();
    }
}
