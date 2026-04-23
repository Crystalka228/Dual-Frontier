#if TOOLS
using Godot;

namespace DualFrontier.Presentation.DevKit;

/// <summary>
/// Entry point for the Dual Frontier DevKit editor plugin. Registered via
/// <c>addons/df_devkit/plugin.cfg</c>. Adds a Tools menu entry for exporting
/// the currently edited scene into a <c>.dfscene</c> file.
/// </summary>
[Tool]
public partial class DfDevKitPlugin : EditorPlugin
{
    private const string MenuLabel = "Export .dfscene...";

    public override void _EnterTree()
    {
        // TODO Phase 3.5: register DFEntityMeta custom Resource type.
        // TODO Phase 3.5: add editor menu item calling SceneExporter.Export.
        GD.Print("[DF DevKit] Plugin loaded. Export menu: ", MenuLabel);
    }

    public override void _ExitTree()
    {
        // TODO Phase 3.5: unregister custom types and menu entries.
        GD.Print("[DF DevKit] Plugin unloaded.");
    }
}
#endif
