---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-MODS-VANILLA-PAWN
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-MODS-VANILLA-PAWN
---
# DualFrontier.Mod.Vanilla.Pawn

## Purpose
Vanilla Pawn mod — empty M8 skeleton. Establishes the assembly,
manifest, and IMod entry point so the mod is discoverable while the
DualFrontier.Systems.Pawn kernel-level systems remain in place.
Content migration into this mod happens across M8.5–M8.7
(ConsumeSystem / SleepSystem / ComfortAuraSystem).

## Dependencies
- `DualFrontier.Contracts` (only).
- `dualfrontier.vanilla.core` (mod-level, shared).

## Contents
- `DualFrontier.Mod.Vanilla.Pawn.csproj` — mod assembly project.
- `mod.manifest.json` — manifest with kind=regular and shared-Core dependency.
- `PawnMod.cs` — empty IMod implementation; content lands in M8.5–M8.7.

## Status
M8.1 skeleton — content empty. Migration target: M8.5–M8.7
ConsumeSystem / SleepSystem / ComfortAuraSystem.
