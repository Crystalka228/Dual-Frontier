namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Typed asset path wrapper — relative path within <see cref="AssetManager"/>'s root directory.
/// Use forward slashes (`/`) as path separator for cross-platform compatibility; AssetManager
/// translates to platform-native separator at resolution time.
/// </summary>
public readonly record struct AssetPath(string RelativePath)
{
    public override string ToString() => RelativePath;
}
