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
- `IModApi.cs` — methods the mod can call: register components and systems,
  publish/subscribe to events, publish and retrieve inter-mod contracts.
- `IModContract.cs` — marker interface for a public contract between mods.
- `ModManifest.cs` — mod metadata: id, name, version, author, dependencies.

## Rules
- A mod MUST NOT cast `IModApi` to a concrete type — that is an attempt to bypass
  isolation.
- A mod interacts with other mods only through `IModContract`.
- Hard inter-mod dependencies are forbidden: use `TryGetContract<T>` and gracefully
  degrade if the contract is not found.

## Usage examples
```csharp
public sealed class ExampleMod : IMod
{
    public void Initialize(IModApi api)
    {
        api.RegisterComponent<MyComponent>();
        api.Subscribe<SpellCastEvent>(OnSpellCast);
    }

    public void Unload() { /* TODO: cleanup */ }

    private void OnSpellCast(SpellCastEvent e) { /* ... */ }
}
```

## TODO
- [ ] Phase 2 — describe the `mod.manifest.json` structure and the mapping to `ModManifest`.
- [ ] Phase 2 — settle the SemVer policy for `ModManifest.Version`.
- [ ] Phase 2 — implement `RestrictedModApi` in `DualFrontier.Application`.
