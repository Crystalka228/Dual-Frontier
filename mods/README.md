# mods/

Example mods. Each is a separate assembly that sees ONLY `DualFrontier.Contracts`.
See [docs/architecture/MODDING.md](/docs/architecture/MODDING.md).

## Contents

- `DualFrontier.Mod.Example/` — reference minimal mod.
  Demonstrates the correct pattern: `IMod` + `mod.manifest.json`,
  with a dependency only on `DualFrontier.Contracts`.

## Rules

- A mod assembly MUST NOT reference `Core`, `Systems`, `Components`,
  `Events`, `AI`, or `Application`. Only `Contracts`.
- The core's `AssemblyLoadContext` enforces this physically — any
  additional reference produces a load error.
- Every mod MUST ship with `mod.manifest.json` next to the dll.

---
register_id: DOC-F-MODS
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
title: Mods directory index
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
