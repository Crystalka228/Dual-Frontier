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
