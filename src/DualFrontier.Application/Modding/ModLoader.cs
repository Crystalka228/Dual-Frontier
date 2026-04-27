using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Loads and unloads mods. Each mod lives in its own
/// <see cref="System.Runtime.Loader.AssemblyLoadContext"/> (via
/// <see cref="ModLoadContext"/>) — per TechArch 11.8. The context is created
/// with <c>isCollectible: true</c>, which enables hot unload and physically
/// isolates the mod assembly from the core.
/// </summary>
public sealed class ModLoader
{
    private readonly Dictionary<string, LoadedMod> _loaded = new();

    /// <summary>
    /// Loads the mod at the given directory. Reads <c>mod.manifest.json</c>,
    /// creates a <see cref="ModLoadContext"/>, loads the entry assembly,
    /// resolves the <see cref="IMod"/> type via reflection and returns the
    /// resulting <see cref="LoadedMod"/>. The mod's <c>Initialize</c> is NOT
    /// called here — the pipeline runs it after validation.
    /// </summary>
    /// <param name="path">Mod directory containing manifest and assembly.</param>
    internal LoadedMod LoadMod(string path)
    {
        if (path is null) throw new ArgumentNullException(nameof(path));
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Mod directory not found: {path}");

        ModManifest manifest = ReadManifest(path);
        if (string.IsNullOrWhiteSpace(manifest.Id))
            throw new InvalidOperationException(
                $"Mod manifest at '{path}' has empty id.");

        if (_loaded.ContainsKey(manifest.Id))
            throw new InvalidOperationException(
                $"Mod '{manifest.Id}' is already loaded.");

        var context = new ModLoadContext(manifest.Id);

        string assemblyName = string.IsNullOrWhiteSpace(manifest.EntryAssembly)
            ? manifest.Id + ".dll"
            : manifest.EntryAssembly;
        string assemblyPath = Path.Combine(path, assemblyName);
        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException(
                $"Mod '{manifest.Id}' entry assembly not found: {assemblyPath}");

        Assembly asm = context.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));

        Type? modType = ResolveModType(asm, manifest);
        if (modType is null)
            throw new InvalidOperationException(
                $"Mod '{manifest.Id}' assembly '{assemblyName}' does not contain an IMod implementation.");

        IMod instance = (IMod)Activator.CreateInstance(modType)!;
        IReadOnlyList<Type> declared = DiscoverSystemTypes(asm);

        var loaded = new LoadedMod(manifest.Id, manifest, instance, context, declared);
        _loaded[manifest.Id] = loaded;
        return loaded;
    }

    private static IReadOnlyList<Type> DiscoverSystemTypes(Assembly asm)
    {
        Type[] types;
        try
        {
            types = asm.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Only the types that managed to load are returned —
            // a mod with broken classes cannot register them.
            types = ex.Types as Type[] ?? Array.Empty<Type>();
        }

        var result = new List<Type>();
        foreach (Type t in types)
        {
            if (t is null) continue;
            if (t.IsAbstract) continue;
            if (!typeof(SystemBase).IsAssignableFrom(t)) continue;
            result.Add(t);
        }
        return result;
    }

    /// <summary>
    /// Unloads the mod with the given id. Chain per MOD_PIPELINE: mod
    /// <c>Unload</c> is invoked, the assembly load context is released and
    /// the registry entry is dropped. The caller (pipeline) is responsible
    /// for cleaning the <see cref="ModRegistry"/> and contract store before
    /// invoking this method so the mod cannot resurrect its state after
    /// <c>Unload</c> returns.
    /// </summary>
    /// <param name="id">Identifier from <see cref="ModManifest.Id"/>.</param>
    public void UnloadMod(string id)
    {
        if (id is null) throw new ArgumentNullException(nameof(id));

        if (!_loaded.TryGetValue(id, out LoadedMod? mod))
            return;

        try
        {
            mod.Instance.Unload();
        }
        catch
        {
            // Mod errors during Unload must not break the whole pipeline —
            // the context still needs to be released below.
        }

        mod.Context.Unload();
        _loaded.Remove(id);
    }

    /// <summary>
    /// Returns the identifiers of mods currently held by the loader.
    /// </summary>
    public IReadOnlyList<string> GetLoaded()
    {
        var ids = new List<string>(_loaded.Count);
        foreach (string id in _loaded.Keys)
            ids.Add(id);
        return ids;
    }

    /// <summary>
    /// Returns the <see cref="LoadedMod"/> entry for the given id or null if
    /// the mod is not currently loaded. Used by the pipeline on the unload
    /// path.
    /// </summary>
    internal LoadedMod? TryGetLoaded(string id)
    {
        _loaded.TryGetValue(id, out LoadedMod? mod);
        return mod;
    }

    /// <summary>
    /// Registers an already-built <see cref="LoadedMod"/> with the loader —
    /// used by tests that construct fixtures in-memory without going through
    /// the on-disk load path.
    /// </summary>
    internal void RegisterLoaded(LoadedMod mod)
    {
        if (mod is null) throw new ArgumentNullException(nameof(mod));
        if (_loaded.ContainsKey(mod.ModId))
            throw new InvalidOperationException(
                $"Mod '{mod.ModId}' is already registered.");
        _loaded[mod.ModId] = mod;
    }

    /// <summary>
    /// Handles an isolation violation surfaced by the runtime guard.
    /// Implemented in the follow-up of Phase 2 — the pipeline hands the
    /// exception to <c>ModFaultHandler</c>, which unloads the offending mod
    /// without reshaping the graph (graph rebuild is deferred to the next
    /// menu open). See <see cref="ModIsolationException"/> and
    /// <c>docs/MOD_PIPELINE.md</c>.
    /// </summary>
    public void HandleModFault(string modId, ModIsolationException exception)
    {
        // TODO: Phase 2 (part 2) — extracted into a dedicated ModFaultHandler.
        throw new NotImplementedException("TODO: Phase 2 (part 2) — ModFaultHandler");
    }

    private static ModManifest ReadManifest(string path)
    {
        string manifestPath = Path.Combine(path, "mod.manifest.json");
        if (!File.Exists(manifestPath))
            throw new FileNotFoundException(
                $"mod.manifest.json not found in '{path}'.", manifestPath);

        string json = File.ReadAllText(manifestPath);
        ModManifest? manifest = JsonSerializer.Deserialize<ModManifest>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            });

        if (manifest is null)
            throw new InvalidOperationException(
                $"Failed to parse mod manifest at '{manifestPath}'.");

        return manifest;
    }

    private static Type? ResolveModType(Assembly asm, ModManifest manifest)
    {
        if (!string.IsNullOrWhiteSpace(manifest.EntryType))
            return asm.GetType(manifest.EntryType, throwOnError: false, ignoreCase: false);

        Type? found = null;
        foreach (Type t in asm.GetTypes())
        {
            if (t.IsAbstract) continue;
            if (!typeof(IMod).IsAssignableFrom(t)) continue;
            // Multiple IMod implementations in a single assembly are a manifest
            // error; an explicit EntryType is required to avoid guessing.
            if (found is not null)
                throw new InvalidOperationException(
                    $"Mod assembly '{asm.FullName}' contains multiple IMod implementations. " +
                    "Specify entryType in the manifest.");
            found = t;
        }
        return found;
    }
}
