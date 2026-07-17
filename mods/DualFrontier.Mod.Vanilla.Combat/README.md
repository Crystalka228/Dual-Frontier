# DualFrontier.Mod.Vanilla.Combat

## Purpose
Vanilla Combat mod — empty M8 skeleton. Establishes the assembly,
manifest, and IMod entry point so the mod is discoverable while the
DualFrontier.Systems.Combat / .Faction kernel-level systems remain in
place. Content migration into this mod happens in M9 Combat (Faction
folds under Combat for the raid pipeline per §1.3).

## Dependencies
- `DualFrontier.Contracts` (only).
- `dualfrontier.vanilla.core` (mod-level, shared).

## Contents
- `DualFrontier.Mod.Vanilla.Combat.csproj` — mod assembly project.
- `mod.manifest.json` — manifest with kind=regular and shared-Core dependency.
- `CombatMod.cs` — empty IMod implementation; content lands in M9.

## Status
M8.1 skeleton — content empty. Migration target: M9 Combat.

---
register_id: DOC-F-MODS-VANILLA-COMBAT
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: Mod Vanilla Combat
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
