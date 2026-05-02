using System;
using System.Collections.Generic;
using System.IO;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Scans a configured root directory for subdirectories containing a
/// <c>mod.manifest.json</c>. Skips subdirectories without the manifest
/// file (build outputs, IDE artifacts, etc.). Catches per-mod parse
/// errors so a single broken manifest does not prevent the menu from
/// listing the rest of the discovered mods (best-effort enumeration —
/// M7.5.B may surface skipped manifests through a separate warning
/// channel later).
/// </summary>
public sealed class DefaultModDiscoverer : IModDiscoverer
{
    private readonly string _rootPath;

    /// <summary>
    /// Creates a discoverer scanning <paramref name="rootPath"/>. The
    /// path is not validated at construction; <see cref="Discover"/>
    /// returns an empty list if the path does not exist (rather than
    /// throwing — first-launch with no <c>mods/</c> directory must be
    /// a clean empty-list case, not an exception).
    /// </summary>
    public DefaultModDiscoverer(string rootPath)
    {
        _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
    }

    public IReadOnlyList<DiscoveredModInfo> Discover()
    {
        if (!Directory.Exists(_rootPath))
            return Array.Empty<DiscoveredModInfo>();

        var result = new List<DiscoveredModInfo>();
        foreach (string dir in Directory.EnumerateDirectories(_rootPath))
        {
            string manifestPath = Path.Combine(dir, "mod.manifest.json");
            if (!File.Exists(manifestPath))
                continue;
            try
            {
                ModManifest manifest = ModLoader.ReadManifestFromDirectory(dir);
                result.Add(new DiscoveredModInfo(dir, manifest));
            }
            catch
            {
                // Per-mod parse failure — skip silently so a single
                // broken manifest does not hide the rest from the menu.
            }
        }
        return result;
    }
}
