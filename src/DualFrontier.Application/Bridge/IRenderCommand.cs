namespace DualFrontier.Application.Bridge;

/// <summary>
/// The base contract for all render commands pushed from Domain to
/// Presentation. Implementations are immutable value records carrying the
/// data needed to apply a visual effect. Each active <see cref="Rendering.IRenderer"/>
/// drains the bridge queue and invokes <see cref="Execute"/> on the main
/// thread of its backend, passing the backend-specific root object:
/// Godot passes its <c>GameRoot</c> node; Native passes its <c>NativeRenderer</c>.
/// The command casts as needed.
/// </summary>
public interface IRenderCommand
{
    /// <summary>
    /// Applies the command to the active rendering context. Called on the
    /// main/render thread of whichever backend is live. Must not capture
    /// references to ECS components or query the <c>World</c>.
    /// </summary>
    /// <param name="renderContext">
    /// Backend-specific root object. Cast to the expected concrete type
    /// inside the implementing Presentation assembly.
    /// </param>
    void Execute(object renderContext);
}
