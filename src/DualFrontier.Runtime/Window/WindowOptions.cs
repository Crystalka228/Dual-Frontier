namespace DualFrontier.Runtime.Window;

/// <summary>
/// Creation parameters для <see cref="Window"/>. Immutable record passed at Window construction;
/// modifying parameters after construction requires creating new Window instance.
/// </summary>
public sealed record WindowOptions
{
    public string Title { get; init; } = "Dual Frontier";
    public int Width { get; init; } = 1280;
    public int Height { get; init; } = 720;
    public bool Resizable { get; init; } = true;
}
