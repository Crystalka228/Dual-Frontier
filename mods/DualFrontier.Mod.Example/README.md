---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-MODS-EXAMPLE
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-MODS-EXAMPLE
---
# DualFrontier.Mod.Example

## Purpose
Reference example mod: a minimal assembly demonstrating the correct
integration pattern. Shows how `IMod` + `IModApi` work and how
`mod.manifest.json` is described. Used as a living example in the
documentation and as the basis for new-mod templates.

## Dependencies
- `DualFrontier.Contracts` (and only that one).

## Contents
- `DualFrontier.Mod.Example.csproj` — assembly; the only `ProjectReference`
  is on `DualFrontier.Contracts`.
- `ExampleMod.cs` — `IMod` implementation: `Initialize(IModApi)` / `Unload()`.
- `mod.manifest.json` — mod manifest (id, version, entry-assembly, entry-type).

## Rules
- No dependencies on `Core`, `Systems`, `Components`, `Events`, `AI`,
  `Application`. Only `Contracts`. This is the mod-isolation rule (TechArch 11.8).
- Casting `IModApi` to a concrete type is forbidden — `ModLoader` detects the
  violation and unloads the mod.
- No blocking operations in `Initialize` — other mods' loading waits on it.

## Usage examples
```csharp
public sealed class ExampleMod : IMod
{
    public void Initialize(IModApi api) { /* registration */ }
    public void Unload() { /* unsubscription */ }
}
```

The mod assembly is built into `mods/DualFrontier.Mod.Example/bin/.../net8.0/`;
`mod.manifest.json` sits alongside it.

## TODO
- [ ] Phase 2 — add an example of registering a component and subscribing to an event.
- [ ] Phase 2 — example of `PublishContract<T>` / `TryGetContract<T>`.
