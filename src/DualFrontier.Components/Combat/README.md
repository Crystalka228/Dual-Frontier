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

---
register_id: DOC-F-SRC-COMPONENTS-COMBAT
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
title: Components Combat submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
