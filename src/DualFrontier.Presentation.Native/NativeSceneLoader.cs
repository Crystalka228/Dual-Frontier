using System;
using DualFrontier.Application.Scene;

namespace DualFrontier.Presentation.Native;

/// <summary>
/// Native <see cref="ISceneLoader"/> stub. Full implementation parses
/// <c>.dfscene</c> via <c>System.IO.File.ReadAllText</c> and
/// <c>System.Text.Json</c>. Lands in Phase 5+.
/// </summary>
public sealed class NativeSceneLoader : ISceneLoader
{
    /// <inheritdoc />
    public SceneDef Load(string path)
        => throw new NotImplementedException("Phase 5+: JSON deserialisation via System.Text.Json.");

    /// <inheritdoc />
    public bool Exists(string path) => System.IO.File.Exists(path);
}
