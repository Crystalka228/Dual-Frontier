---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-MODS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-MODS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-MODS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-MODS
---
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
