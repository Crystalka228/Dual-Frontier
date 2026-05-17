# Modding — Mod API

## Purpose
The public mod API. Every mod implements `IMod`, receives an `IModApi`, and
interacts with the core only through it. `AssemblyLoadContext` physically blocks
the mod's access to the internals of `DualFrontier.Core` — the only reference a
mod has is this assembly (Contracts).

## Dependencies
- `DualFrontier.Contracts.Core` (the `IEvent` and `IComponent` markers).

## Contents
- `IMod.cs` — the mod's entry point: `Initialize(api)` and `Unload()`.
- `IModApi.cs` — Mod API v3 strict (post-K8.3+K8.4 cutover 2026-05-14). Methods the mod can call: register Path α components (`RegisterComponent<T> where T : unmanaged, IComponent`), register Path β components (`RegisterManagedComponent<T> where T : class, IComponent` with `[ManagedStorage]`), register systems, publish/subscribe to events, publish and retrieve inter-mod contracts, structured log, get kernel capabilities + own manifest. Includes `Fields` (`IModFieldApi?`) and `ComputePipelines` (`IModComputePipelineApi?`) sub-APIs (default-null on builds without K9/V-substrate support).
- `IModContract.cs` — marker interface for a public contract between mods.
- `ModManifest.cs` — mod metadata: id, name, version, author, dependencies, capabilities. `manifestVersion` must be strict literal `"3"` per K8.3+K8.4.
- `IManagedStore.cs` + `ManagedStore.cs` — Path β per-mod managed-class storage (K-L3.1 bridge). Reachable from systems via `SystemBase.ManagedStore<T>()`.

## Rules
- A mod MUST NOT cast `IModApi` to a concrete type — that is an attempt to bypass
  isolation.
- A mod interacts with other mods only through `IModContract`.
- Hard inter-mod dependencies are forbidden: use `TryGetContract<T>` and gracefully
  degrade if the contract is not found.
- Manifest must declare `manifestVersion: "3"` — parser rejects any other value.

## Usage examples
```csharp
public sealed class ExampleMod : IMod
{
    public void Initialize(IModApi api)
    {
        api.RegisterComponent<MyUnmanagedComponent>();        // Path α
        api.RegisterManagedComponent<MyManagedComponent>();   // Path β (requires [ManagedStorage])
        api.Subscribe<SpellCastEvent>(OnSpellCast);
        api.Log(ModLogLevel.Info, "ExampleMod initialized");
    }

    public void Unload() { /* subscriptions removed automatically */ }

    private void OnSpellCast(SpellCastEvent e) { /* ... */ }
}
```

## TODO
- [x] `RestrictedModApi` implemented in `DualFrontier.Application/Modding/RestrictedModApi.cs` (v3 strict — closed K8.3+K8.4 2026-05-14).
- [x] `mod.manifest.json` schema documented in [MODDING](../../../docs/architecture/MODDING.md) §mod.manifest.json.
- [ ] Phase 2 — settle the SemVer policy for `ModManifest.Version`.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CONTRACTS-MODDING
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CONTRACTS-MODDING
---
