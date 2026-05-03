# DualFrontier.Mod.Vanilla.World

## Purpose
Vanilla World mod — empty M8 skeleton. Establishes the assembly,
manifest, and IMod entry point so the mod is discoverable while the
DualFrontier.Systems.World kernel-level systems remain in place.
Content migration into this mod happens in M8.4 (Item factory + 4
entity types).

## Dependencies
- `DualFrontier.Contracts` (only).
- `dualfrontier.vanilla.core` (mod-level, shared).

## Contents
- `DualFrontier.Mod.Vanilla.World.csproj` — mod assembly project.
- `mod.manifest.json` — manifest with kind=regular and shared-Core dependency.
- `WorldMod.cs` — empty IMod implementation; content lands in M8.4.

## Status
M8.1 skeleton — content empty. Migration target: M8.4 Item factory + 4
entity types.
