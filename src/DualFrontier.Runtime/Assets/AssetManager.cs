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

    /// <summary>Absolute path к resolved assets root directory (with trailing separator).</summary>
    public string RootDirectory => _rootDirectory;

    public AssetManager(string rootDirectory)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);
        string resolved = ResolveAssetsDirectory(rootDirectory);
        // Normalize root path (trailing separator + canonical form) для secure traversal check.
        _rootDirectory = Path.GetFullPath(resolved);
        if (!_rootDirectory.EndsWith(Path.DirectorySeparatorChar))
        {
            _rootDirectory += Path.DirectorySeparatorChar;
        }
    }

    /// <summary>
    /// Resolve assets directory: if absolute path, use as-is; if relative, first check cwd,
    /// then walk up from AppContext.BaseDirectory looking for an ancestor directory containing
    /// the named subdirectory. To avoid false positives на case-insensitive filesystems
    /// (e.g., test project "Assets" folders), candidates must also contain a "shaders"
    /// subdirectory (V0.B+ baseline asset layout). Throws DirectoryNotFoundException if not found.
    /// </summary>
    private static string ResolveAssetsDirectory(string rootDirectory)
    {
        if (Path.IsPathRooted(rootDirectory))
        {
            if (!Directory.Exists(rootDirectory))
            {
                throw new DirectoryNotFoundException($"Asset root directory not found: {rootDirectory}");
            }
            return rootDirectory;
        }

        // 1. Relative к current working directory — verify с shaders subdir too.
        string cwdCandidate = Path.GetFullPath(rootDirectory);
        if (IsValidAssetsRoot(cwdCandidate))
        {
            return cwdCandidate;
        }

        // 2. Walk up from AppContext.BaseDirectory looking for ancestor containing the directory.
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            string candidate = Path.Combine(dir.FullName, rootDirectory);
            if (IsValidAssetsRoot(candidate))
            {
                return candidate;
            }
            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Asset root directory not found: {rootDirectory} (checked cwd + ancestors of AppContext.BaseDirectory). " +
            "Candidate must exist and contain a 'shaders' subdirectory.");
    }

    private static bool IsValidAssetsRoot(string path) =>
        Directory.Exists(path) && Directory.Exists(Path.Combine(path, "shaders"));

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
        // Path traversal protection — combined must stay inside root. The comparison's case
        // sensitivity must track the host filesystem: Windows paths are case-insensitive, but on
        // case-sensitive volumes (Linux, case-sensitive macOS) an unconditional OrdinalIgnoreCase
        // check treats a case-variant path as inside the root when it actually resolves elsewhere,
        // letting it escape (F03). Ordinal on non-Windows can only reject more, never admit an escape.
        StringComparison pathComparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        if (!combined.StartsWith(_rootDirectory, pathComparison))
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
