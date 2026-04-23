using System;

namespace DualFrontier.Application.Scene;

/// <summary>
/// Thrown by <see cref="ISceneLoader.Load"/> when the file is missing,
/// malformed, or authored against an unsupported <c>.dfscene</c> version.
/// </summary>
public sealed class SceneLoadException : Exception
{
    public SceneLoadException(string message) : base(message) { }
    public SceneLoadException(string message, Exception inner) : base(message, inner) { }
}
