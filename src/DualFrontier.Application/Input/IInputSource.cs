namespace DualFrontier.Application.Input;

/// <summary>
/// The ingress point for user input events. Each presentation backend
/// implements this: Godot polls <c>Input</c> via its main thread, Native
/// polls via Silk.NET input. Regardless of source, the implementation
/// publishes normalised <c>InputEvent</c>s onto the appropriate domain bus.
/// </summary>
public interface IInputSource
{
    /// <summary>
    /// Called once per frame by the game loop. Reads any new input from the
    /// backend, translates it into domain events, and publishes them. Must
    /// not block.
    /// </summary>
    void Poll();
}
