# World Systems

## Purpose
Global world systems: map (tiles, decorations) and biomes. They tick rarely
and change rarely — but their events (`BiomeShiftEvent`) are read by almost
everyone.

Weather is NOT here. It shipped at W3 as a mod pair —
`mods/DualFrontier.Mod.Weather.Contracts` (shared vendor) plus
`mods/DualFrontier.Mod.Weather` (the mechanic) — and the `src/` stubs that
stood in for it were deleted. It is the reference for how a mechanic is
authored outside the engine.

## Dependencies
- `DualFrontier.Contracts` — attributes, `IWorldBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.World` — `TileComponent`.

## Contents
- `MapSystem.cs` — RARE: load/unload of map regions.

## Rules
- Domain bus — `nameof(IGameServices.World)`.
- `MapSystem` is RARE (3600 frames ≈ once per real-time minute), so it does
  not weigh on the main loop.

## TODO
- [ ] Implement `MapSystem`: region streaming based on the camera center.

---
register_id: DOC-F-SRC-SYSTEMS-WORLD
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: '2026-08-20'
content_language: en
next_review_due: null
title: Systems World submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
