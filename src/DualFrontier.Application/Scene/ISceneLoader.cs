namespace DualFrontier.Application.Scene;

/// <summary>
/// Loads <c>.dfscene</c> files from disk or virtual file systems. Each
/// presentation backend provides its own implementation: Godot reads via
/// <c>GD.Load</c> (res://), Native reads via <c>System.IO</c>.
/// </summary>
public interface ISceneLoader
{
    /// <summary>
    /// Parses a <c>.dfscene</c> file at the given path and returns a
    /// fully-populated <see cref="SceneDef"/>. Throws
    /// <see cref="SceneLoadException"/> on parse errors or version mismatch.
    /// </summary>
    /// <param name="path">Relative or absolute path to the scene file.</param>
    SceneDef Load(string path);

    /// <summary>
    /// Checks whether the file exists and is readable without attempting to
    /// parse it. Use this before calling <see cref="Load"/> to fail fast.
    /// </summary>
    /// <param name="path">Relative or absolute path to the scene file.</param>
    bool Exists(string path);
}
