# Magic

## Purpose
Components of the magic subsystem: mana, magic-school levels, ether perception,
and the golem-mage bond. Implements the key mechanics of GDD 4 ("Magical colony")
and GDD 5 ("Golem system").

## Dependencies
- `DualFrontier.Contracts` — `IComponent`, `EntityId`.

## Contents
- `ManaComponent.cs` — current/maximum mana + regeneration.
- `EtherComponent.cs` — ether perception level (1–5, GDD 4.1).
- `GolemBondComponent.cs` — bond between a golem and its owning mage, golem level (1–5).

## Rules
- A mage without `ManaComponent` cannot cast spells — checked in SpellCastSystem.
- `EtherComponent.Level` is hard-clamped to 1..5. Increases happen through
  `EtherLevelUpEvent` (deferred).
- A golem MUST have `GolemBondComponent.OwnerId`. Death/exhaustion of the owner
  → the golem stops (GDD 5.2).

## Usage examples
```csharp
[SystemAccess(reads: new[] { typeof(ManaComponent) })]
public class SpellCastSystem : SystemBase { /* ... */ }
```

## TODO
- [ ] Add `ManaRegenModifierComponent` for environmental effects (nodes,
      potions, rituals) — Phase 6.
- [ ] `GolemTier` 1..5 — validator in Phase 5.

## v02 Addendum additions
Extension to `GolemBondComponent` (v02 §12.5): ownership-mode fields and contest/handover mechanics.

- `GolemBondComponent.BondedMage` — `EntityId?`, reference to the current owning mage (`null` when `Abandoned`).
- `GolemBondComponent.Mode` — `OwnershipMode`, ownership mode, defaults to `Bonded`.
- `GolemBondComponent.TicksSinceContested` — `int`, tick counter in the `Contested` state for owner-change timeout.
- `GolemBondComponent.BondStrength` — `int`, bond strength, used in dispute resolution.

Note: the `OwnershipMode` enum is defined in `DualFrontier.Contracts.Enums` — Components do not depend on Events, so the shared type is lifted into Contracts.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-MAGIC
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-MAGIC
---
