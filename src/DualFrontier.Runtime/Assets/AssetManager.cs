namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Centralized asset loading + caching. V0.C.1 scope: PNG loading via <see cref="PngDecoder"/>.
/// Future asset types (bitmap font, audio, JSON) extend through additional Load* methods or
/// generic IAssetLoader registration when consumer materializes per Lesson #25.
///
/// Path resolution rules:
/// - All paths are relative to <see cref="_rootDirectory"/> (configured at construction).
/// - Forward slashes (`/`) accepted as separators; translated to native separator.
/// - Path traversal protection: combined paths verified to stay inside root directory;
///   attempts к escape (e.g., `../secret.txt`) throw <see cref="InvalidOperationException"/>.
///
/// Caching:
/// - Decoded PngImage instances cached per AssetPath; subsequent LoadPng для same path returns
///   the same instance without re-decoding.
/// - Cache cleared on Dispose; can also be manually cleared via <see cref="ClearCache"/>.
/// </summary>
public sealed class AssetManager : IDisposable
{
    private readonly string _rootDirectory;
    private readonly Dictionary<AssetPath, PngImage> _pngCache = new();
    private bool _disposed;

    public AssetManager(string rootDirectory)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);
        if (!Directory.Exists(rootDirectory))
        {
            throw new DirectoryNotFoundException($"Asset root directory not found: {rootDirectory}");
        }
        // Normalize root path (trailing separator + canonical form) для secure traversal check.
        _rootDirectory = Path.GetFullPath(rootDirectory);
        if (!_rootDirectory.EndsWith(Path.DirectorySeparatorChar))
        {
            _rootDirectory += Path.DirectorySeparatorChar;
        }
    }

    /// <summary>Load PNG image, cached by path. Subsequent loads return cached instance.</summary>
    public PngImage LoadPng(AssetPath path)
    {
        ThrowIfDisposed();
        if (_pngCache.TryGetValue(path, out PngImage? cached))
        {
            return cached;
        }

        string fullPath = ResolvePath(path);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                $"Asset not found: {path.RelativePath} (resolved: {fullPath})", fullPath);
        }

        PngImage image = PngDecoder.DecodeFile(fullPath);
        _pngCache[path] = image;
        return image;
    }

    /// <summary>
    /// Resolve relative path к absolute filesystem path. Verifies result stays inside root
    /// directory (path traversal protection).
    /// </summary>
    public string ResolvePath(AssetPath path)
    {
        ThrowIfDisposed();
        string combined = Path.GetFullPath(Path.Combine(_rootDirectory, path.RelativePath));
        // Path traversal protection — combined must be inside root (case-insensitive on Windows).
        if (!combined.StartsWith(_rootDirectory, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Asset path escapes root directory: {path.RelativePath}");
        }
        return combined;
    }

    /// <summary>Clear all cached decoded assets. Subsequent loads re-decode from disk.</summary>
    public void ClearCache()
    {
        ThrowIfDisposed();
        _pngCache.Clear();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _pngCache.Clear();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(AssetManager));
        }
    }
}
