namespace DualFrontier.Application.Save;

/// <summary>
/// Contract for the world save/load system. Both methods are synchronous —
/// asynchrony (progress, cancellation) is the responsibility of the caller.
/// </summary>
public interface ISaveSystem
{
    /// <summary>
    /// TODO: Phase 1 — saves the current world state to a file at the given path.
    /// </summary>
    /// <param name="path">Full path to the save file.</param>
    void Save(string path);

    /// <summary>
    /// TODO: Phase 1 — loads the world state from a file. The current world
    /// state is overwritten.
    /// </summary>
    /// <param name="path">Full path to the save file.</param>
    void Load(string path);
}
