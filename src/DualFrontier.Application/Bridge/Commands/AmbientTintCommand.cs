namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: modulate the whole rendered scene toward the colour
/// (<paramref name="R"/>, <paramref name="G"/>, <paramref name="B"/>) by
/// <paramref name="Strength"/>. Engine-generic by construction — it carries a
/// COLOUR, never a game concept. Nothing here knows about weather, time of day,
/// or damage flashes; a mod decides what a colour means and the engine only
/// applies it.
///
/// <para>
/// This is the W3 minimal presentation primitive, sibling to the six game
/// commands that the BD-9 presentation model absorbs at W6. It is the render-side
/// half of <c>ISystemContext.SetAmbientTint</c>: the SDK member reaches the
/// engine's presentation sink, the sink enqueues this command, and the renderer
/// applies it on its own thread.
/// </para>
/// </summary>
/// <param name="R">Red channel of the tint colour, 0..1.</param>
/// <param name="G">Green channel of the tint colour, 0..1.</param>
/// <param name="B">Blue channel of the tint colour, 0..1.</param>
/// <param name="Strength">
/// How far the scene is pulled toward the colour, 0..1. 0 restores the untinted
/// scene exactly; 1 is full modulation.
/// </param>
public sealed record AmbientTintCommand(float R, float G, float B, float Strength) : IRenderCommand;
