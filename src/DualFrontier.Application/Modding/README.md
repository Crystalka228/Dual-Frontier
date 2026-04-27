# Modding — Mod loader

## Purpose
Infrastructure for loading, unloading, and isolating mods. Each mod lives in
its own `AssemblyLoadContext` (`ModLoadContext`) with `isCollectible: true`
— this physically prevents the mod from reaching into the core's internals
and enables hot reload (see TechArch 11.8).

## Dependencies
- `DualFrontier.Contracts` — `IMod`, `IModApi`, `IModContract`, `ModManifest`
- `DualFrontier.Core` — `GameServices` (through a proxy)

## Contents
- `ModLoader.cs` — load/unload API and the active-mod registry.
- `ModLoadContext.cs` — per-mod `AssemblyLoadContext`.
- `RestrictedModApi.cs` — `IModApi` implementation; proxies calls into the core
  with extra rights and quota checks.
- `ModIsolationException.cs` — thrown when a mod tries to reach into core
  internals (bypassing `IModApi`).

## Rules
- A mod **sees** only the `DualFrontier.Contracts` assembly. Everything else —
  through `IModApi`.
- Casting `IModApi` to `RestrictedModApi` is forbidden and detected.
- Not a single reference to `DualFrontier.Core` from a mod assembly — the
  `AssemblyLoadContext` guarantees this physically.
- `ModLoader.Unload` MUST wait for every mod callback to complete before
  unloading its context.

## Usage examples
```csharp
var loader = new ModLoader(services);
loader.LoadMod("mods/DualFrontier.Mod.Example/bin/Debug/net8.0/");
foreach (var id in loader.GetLoaded())
{
    Console.WriteLine($"Loaded mod: {id}");
}
loader.UnloadMod("dualfrontier.example");
```

## TODO
- [ ] Phase 2 — `ModLoader.LoadMod` (Manifest → Assembly → reflection `IMod`).
- [ ] Phase 2 — `RestrictedModApi` proxies into `GameServices`.
- [ ] Phase 2 — isolation tests (`tests/DualFrontier.Modding.Tests`).
- [ ] Phase 3 — hot reload (Unload + repeat LoadMod).
