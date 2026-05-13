---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-COMBAT
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-COMBAT
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-COMBAT
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-COMBAT
---
# Combat

## Purpose
Components of the combat subsystem in the spirit of Combat Extended: weapon
characteristics, armor, magic shields, and ammunition. See GDD 6 "Combat System".

## Dependencies
- `DualFrontier.Contracts` — `IComponent`.

## Contents
- `ArmorComponent.cs` — resistances (sharp / blunt / heat).

## Rules
- `ArmorComponent.*Resist` use a single scale (to be documented in
  `/docs/COMBAT.md`). The check happens in DamageSystem.

## TODO
- [ ] Define the `DamageType` enum (Sharp, Blunt, Heat, Frost, Arcane …) per GDD 6.1.
