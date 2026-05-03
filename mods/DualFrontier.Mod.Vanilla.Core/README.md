# DualFrontier.Mod.Vanilla.Core

## Purpose
Vanilla shared mod — empty M8 skeleton. Loaded into the shared
AssemblyLoadContext so dependent vanilla regular mods see identical
Type instances. No IMod entry point per §1.2 «pure type vendor».
Content (cross-slice shared definition records) lands when M10
incremental migration introduces shared types between mods.

## Dependencies
- `DualFrontier.Contracts` (only).

## Contents
- `DualFrontier.Mod.Vanilla.Core.csproj` — shared mod assembly project.
- `mod.manifest.json` — manifest with kind=shared, empty entryAssembly+entryType.

## Status
M8.1 skeleton — content empty. Migration target: M10 incremental shared types.
