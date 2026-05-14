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

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-MODS-VANILLA-WORLD
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-MODS-VANILLA-WORLD
---
