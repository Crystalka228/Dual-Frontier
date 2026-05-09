using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Loads and unloads mods. Regular mods (with an <see cref="IMod"/> entry
/// point) are loaded through <see cref="LoadRegularMod"/>; each lives in its
/// own collectible <see cref="ModLoadContext"/> per TechArch 11.8 — the
/// context is created with <c>isCollectible: true</c> so the mod can be
/// unloaded without restarting the game. Shared mods (pure type vendors per
/// MOD_OS_ARCHITECTURE §1.2/§5) are loaded through <see cref="LoadSharedMod"/>
/// into a single non-collectible <see cref="SharedModLoadContext"/>.
/// </summary>
public sealed class ModLoader
{
    private readonly Dictionary<string, LoadedMod> _loaded = new();
    private readonly Dictionary<string, LoadedSharedMod> _sharedLoaded = new();
    private ModFaultHandler? _faultHandler;

    /// <summary>
    /// Installs the Application-side fault handler. Called once during
    /// <see cref="ModIntegrationPipeline"/> startup; subsequent calls
    /// overwrite the reference so test fixtures can swap a stub handler
    /// in. Null is allowed and disables fault routing —
    /// <see cref="HandleModFault"/> becomes a no-op except for the
    /// argument null-checks.
    /// </summary>
    /// <param name="handler">Handler to receive fault reports, or null to disable routing.</param>
    internal void SetFaultHandler(ModFaultHandler? handler)
    {
        _faultHandler = handler;
    }

    /// <summary>
    /// Backward-compatible alias for <see cref="LoadRegularMod"/>. Equivalent
    /// to calling <c>LoadRegularMod(path, sharedAlc: null)</c>: no shared ALC
    /// is wired in, so the resulting <see cref="ModLoadContext"/> resolves
    /// only its own assemblies and the default ALC. Retained because tests
    /// and ad-hoc callers still target the original signature.
    /// </summary>
    /// <param name="path">Mod directory containing manifest and assembly.</param>
    internal LoadedMod LoadMod(string path) => LoadRegularMod(path, sharedAlc: null);

    /// <summary>
    /// Loads a regular mod from the given directory. Reads
    /// <c>mod.manifest.json</c>, creates a <see cref="ModLoadContext"/>,
    /// loads the entry assembly, resolves the <see cref="IMod"/> type via
    /// reflection and returns the resulting <see cref="LoadedMod"/>. The
    /// mod's <c>Initialize</c> is NOT called here — the pipeline runs it
    /// after validation.
    /// </summary>
    /// <param name="path">Mod directory containing manifest and assembly.</param>
    /// <param name="sharedAlc">
    /// Singleton shared ALC the regular mod's <see cref="ModLoadContext"/>
    /// should delegate to for cross-mod type references. Pass
    /// <see langword="null"/> when no shared mods participate (single-mod
    /// tests, ad-hoc usage).
    /// </param>
    internal LoadedMod LoadRegularMod(string path, SharedModLoadContext? sharedAlc)
    {
        if (path is null) throw new ArgumentNullException(nameof(path));
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Mod directory not found: {path}");

        ModManifest manifest = ReadManifestFromDirectory(path);
        if (string.IsNullOrWhiteSpace(manifest.Id))
            throw new InvalidOperationException(
                $"Mod manifest at '{path}' has empty id.");

        if (_loaded.ContainsKey(manifest.Id))
            throw new InvalidOperationException(
                $"Mod '{manifest.Id}' is already loaded.");

        var context = new ModLoadContext(manifest.Id, sharedAlc);

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

    /// <summary>
    /// Loads a shared mod from the given directory into the shared ALC.
    /// Per MOD_OS_ARCHITECTURE §5.2: defensively asserts <c>kind=shared</c>
    /// and loads the assembly through
    /// <see cref="SharedModLoadContext.LoadSharedAssembly"/>. Architectural
    /// compliance — that the assembly does not contain an <see cref="IMod"/>
    /// implementation and that the manifest's <c>entryAssembly</c>,
    /// <c>entryType</c> and <c>replaces</c> fields are empty — is enforced
    /// post-load by <c>ContractValidator</c> Phase F, which surfaces typed
    /// <see cref="ValidationErrorKind.SharedModWithEntryPoint"/> errors. The
    /// loader's kind check remains because a mismatched kind reflects a
    /// pipeline programming error, not a mod-author mistake.
    /// </summary>
    /// <param name="path">Shared mod directory containing manifest and assembly.</param>
    /// <param name="sharedAlc">
    /// Singleton shared ALC owned by the pipeline. Same instance is reused
    /// across every <c>LoadSharedMod</c> call in the session (§5.1).
    /// </param>
    internal LoadedSharedMod LoadSharedMod(string path, SharedModLoadContext sharedAlc)
    {
        if (path is null) throw new ArgumentNullException(nameof(path));
        if (sharedAlc is null) throw new ArgumentNullException(nameof(sharedAlc));
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Mod directory not found: {path}");

        ModManifest manifest = ReadManifestFromDirectory(path);
        if (string.IsNullOrWhiteSpace(manifest.Id))
            throw new InvalidOperationException(
                $"Mod manifest at '{path}' has empty id.");

        // Caller (the pipeline) is expected to branch by ModKind; the
        // loader still rejects regular manifests defensively because a
        // mismatched kind here means the pipeline routed the wrong mod
        // path to LoadSharedMod — a programming error, not a mod-author
        // mistake. ValidationError surfacing of architectural rules lives
        // in ContractValidator Phase F (M4.3).
        if (manifest.Kind != ModKind.Shared)
            throw new InvalidOperationException(
                $"Mod '{manifest.Id}' at '{path}' is not declared as 'shared' " +
                $"(kind={manifest.Kind}); shared mods are required for LoadSharedMod.");

        if (_sharedLoaded.ContainsKey(manifest.Id))
            throw new InvalidOperationException(
                $"Shared mod '{manifest.Id}' is already loaded.");

        string assemblyName = string.IsNullOrWhiteSpace(manifest.EntryAssembly)
            ? manifest.Id + ".dll"
            : manifest.EntryAssembly;
        string assemblyPath = Path.Combine(path, assemblyName);
        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException(
                $"Shared mod '{manifest.Id}' assembly not found: {assemblyPath}");

        Assembly asm = sharedAlc.LoadSharedAssembly(Path.GetFullPath(assemblyPath));

        Type[] exported;
        try
        {
            exported = asm.GetExportedTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Any unloadable type prevents reasoning about the assembly's
            // public surface; surface as a load failure (§5.2 step 4).
            throw new InvalidOperationException(
                $"Shared mod '{manifest.Id}' assembly '{assemblyName}' " +
                $"failed to enumerate exported types: {ex.Message}", ex);
        }

        var loaded = new LoadedSharedMod(manifest.Id, manifest, sharedAlc, asm, exported);
        _sharedLoaded[manifest.Id] = loaded;
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
    /// Returns the <see cref="LoadedSharedMod"/> entry for the given id or
    /// null if no shared mod with this id is currently loaded.
    /// </summary>
    internal LoadedSharedMod? TryGetLoadedShared(string id)
    {
        _sharedLoaded.TryGetValue(id, out LoadedSharedMod? mod);
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
    /// Per MOD_OS_ARCHITECTURE §10.3 + TechArch 11.8: the core does not
    /// crash on a mod isolation violation. The fault is reported through
    /// <see cref="ModFaultHandler"/> (an <see cref="IModFaultSink"/>
    /// implementation owned by <see cref="ModIntegrationPipeline"/>); the
    /// offending mod is queued for deferred unload at the next menu open
    /// per the design comment retained from the original Phase 2 (part 2)
    /// plan.
    ///
    /// This method is the public-surface entry point for callers that hold
    /// a <see cref="ModLoader"/> reference but not a
    /// <see cref="ModFaultHandler"/> reference. New code routes faults
    /// through <see cref="IModFaultSink"/> directly via
    /// <see cref="DualFrontier.Core.ECS.SystemExecutionContext"/>'s
    /// installed sink. See <see cref="ModIsolationException"/> and
    /// <c>docs/MOD_PIPELINE.md</c>.
    ///
    /// Idempotent: handling the same fault twice is harmless; the handler
    /// deduplicates internally. When no handler is installed
    /// (<see cref="SetFaultHandler"/> was not called or was called with
    /// null), the routing is a silent no-op after the argument checks.
    /// </summary>
    public void HandleModFault(string modId, ModIsolationException exception)
    {
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        if (exception is null) throw new ArgumentNullException(nameof(exception));

        _faultHandler?.ReportFault(modId, exception.Message);
    }

    /// <summary>
    /// Reads <c>mod.manifest.json</c> from the given mod directory and parses
    /// it. No assembly is loaded — used by the pipeline to classify mods by
    /// <see cref="ModManifest.Kind"/> before deciding which load path to take.
    /// </summary>
    /// <param name="path">Mod directory containing <c>mod.manifest.json</c>.</param>
    internal static ModManifest ReadManifestFromDirectory(string path)
    {
        string manifestPath = Path.Combine(path, "mod.manifest.json");
        if (!File.Exists(manifestPath))
            throw new FileNotFoundException(
                $"mod.manifest.json not found in '{path}'.", manifestPath);

        string json = File.ReadAllText(manifestPath);
        return ManifestParser.Parse(json, manifestPath);
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
