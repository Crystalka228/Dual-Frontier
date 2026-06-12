namespace DualFrontier.Application.Input;

/// <summary>
/// The ingress point for user input events. Reserved presentation-backend
/// seam — no implementation is currently wired: the Launcher pumps raw
/// Win32 input into its own <c>InputQueue</c> and Application-side
/// forwarding is pending (the prior Godot <c>Input</c> / Silk.NET poll
/// backends were retired with К-extensions cascade #2). Per the seam
/// contract, an implementation publishes normalised <c>InputEvent</c>s
/// onto the appropriate domain bus regardless of source.
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
