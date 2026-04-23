namespace DualFrontier.Application.Rendering;

/// <summary>
/// The production-tier rendering contract implemented by every presentation
/// backend. Exactly one <see cref="IRenderer"/> is active per game session.
/// This interface carries only methods that the shipped Native runtime must
/// support. Debug visualisations, inspectors, and authoring helpers live in
/// the devkit-tier extension <see cref="IDevKitRenderer"/>, which only Godot
/// DevKit implements.
/// </summary>
/// <remarks>
/// Rule of thumb: if players see it in the final game, it belongs here. If
/// only developers or modders ever see it, it belongs in
/// <see cref="IDevKitRenderer"/>.
/// </remarks>
public interface IRenderer
{
    /// <summary>
    /// Prepares the backend for the first frame: creates the window, loads
    /// initial assets, initialises GPU resources. Called once before the
    /// game loop starts.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Draws a single frame. Called by the game loop every tick. The backend
    /// is responsible for draining the <see cref="Bridge.PresentationBridge"/>
    /// command queue and applying commands before rendering.
    /// </summary>
    /// <param name="deltaSeconds">Wall-clock time since the previous frame.</param>
    void RenderFrame(double deltaSeconds);

    /// <summary>
    /// Releases GPU resources, closes the window, and detaches from any
    /// event subscriptions. Called once at game shutdown.
    /// </summary>
    void Shutdown();

    /// <summary>
    /// True while the renderer is alive and the window is open. Flips to
    /// false after <see cref="Shutdown"/> or when the user closes the window.
    /// </summary>
    bool IsRunning { get; }
}
