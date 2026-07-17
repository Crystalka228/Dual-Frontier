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

The mod assembly is built into `mods/DualFrontier.Mod.Example/bin/.../net10.0/`;
`mod.manifest.json` sits alongside it.

## TODO
- [ ] Phase 2 — add an example of registering a component and subscribing to an event.
- [ ] Phase 2 — example of `PublishContract<T>` / `TryGetContract<T>`.

---
register_id: DOC-F-MODS-EXAMPLE
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-07-17
content_language: en
next_review_due: null
title: Mod Example
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
