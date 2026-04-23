#if TOOLS
using Godot;

namespace DualFrontier.Presentation.DevKit.Meta;

/// <summary>
/// Attached to any Node in a Godot scene that should become an ECS entity
/// on export. <see cref="Prefab"/> selects a registered component bundle
/// (e.g. <c>"core:pawn"</c>); <see cref="ComponentOverrides"/> allows
/// per-instance tuning of component fields without touching the prefab.
/// </summary>
[Tool]
[GlobalClass]
public partial class DFEntityMeta : Resource
{
    [Export] public string Prefab { get; set; } = "core:pawn";

    [Export]
    public Godot.Collections.Dictionary ComponentOverrides { get; set; } = new();
}
#endif
