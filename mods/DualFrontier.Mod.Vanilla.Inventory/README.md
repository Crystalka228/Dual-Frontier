# DualFrontier.Mod.Vanilla.Inventory

## Purpose
Vanilla Inventory mod — empty M8 skeleton. Establishes the assembly,
manifest, and IMod entry point so the mod is discoverable while the
DualFrontier.Systems.Inventory / .Power kernel-level systems remain in
place. Content migration into this mod happens in M10 Inventory (Power
folds under Inventory for the economy pipeline per §1.3).

## Dependencies
- `DualFrontier.Contracts` (only).
- `dualfrontier.vanilla.core` (mod-level, shared).

## Contents
- `DualFrontier.Mod.Vanilla.Inventory.csproj` — mod assembly project.
- `mod.manifest.json` — manifest with kind=regular and shared-Core dependency.
- `InventoryMod.cs` — empty IMod implementation; content lands in M10.

## Status
M8.1 skeleton — content empty. Migration target: M10 Inventory.
