namespace DualFrontier.Application.Bridge;

/// <summary>
/// The engine-internal seam a mod's presentation call lands on. The composition
/// root (<c>GameBootstrap.CreateSession</c>) installs an implementation that
/// enqueues the matching <see cref="IRenderCommand"/> onto the
/// <see cref="PresentationBridge"/>; tests install a recording double and assert
/// what a mod asked for without standing up a renderer.
///
/// <para>
/// Deliberately NOT part of <c>DualFrontier.Contracts</c>: a mod names
/// <c>ISystemContext.SetAmbientTint</c>, never this interface. Keeping the sink
/// Application-internal means the render path can be reshaped (BD-9 / W6) without
/// touching the mod-facing contract.
/// </para>
///
/// <para>
/// <b>Fail-open is forbidden here.</b> When no sink is installed, a presentation
/// call throws (see <c>ModRegistry.RequirePresentationSink</c>) rather than
/// silently doing nothing — a mod whose visuals vanish with no diagnostic is the
/// exact shape K-L19 fail-fast exists to prevent.
/// </para>
/// </summary>
internal interface IPresentationSink
{
    /// <summary>
    /// Applies a whole-scene colour modulation. Channels and strength are 0..1;
    /// strength 0 restores the untinted scene.
    /// </summary>
    void SetAmbientTint(float r, float g, float b, float strength);
}
