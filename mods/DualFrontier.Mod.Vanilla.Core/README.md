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

---
register_id: DOC-F-MODS-VANILLA-CORE
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
title: Mod Vanilla Core
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
