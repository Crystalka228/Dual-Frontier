# DualFrontier.Mod.Vanilla.Magic

## Purpose
Vanilla Magic mod — empty M8 skeleton. Establishes the assembly,
manifest, and IMod entry point so the mod is discoverable while the
DualFrontier.Systems.Magic kernel-level systems remain in place.
Content migration into this mod happens in M10.B Magic.

## Dependencies
- `DualFrontier.Contracts` (only).
- `dualfrontier.vanilla.core` (mod-level, shared).

## Contents
- `DualFrontier.Mod.Vanilla.Magic.csproj` — mod assembly project.
- `mod.manifest.json` — manifest with kind=regular and shared-Core dependency.
- `MagicMod.cs` — empty IMod implementation; content lands in M10.B.

## Status
M8.1 skeleton — content empty. Migration target: M10.B Magic.
