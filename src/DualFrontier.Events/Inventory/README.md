---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-EVENTS-INVENTORY
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-EVENTS-INVENTORY
---
# Inventory Events

## Purpose
Inventory and crafting events: adding/removing/reserving items and crafting requests.

## Dependencies
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Contents
- `ItemAddedEvent.cs` — an item was placed into a storage / into a pawn's hands.
- `ItemRemovedEvent.cs` — an item was removed from a storage.
- `ItemReservedEvent.cs` — an item was reserved for a job (crafting, building, hauling).
- `CraftRequestEvent.cs` — a pawn requested a craft (AI or player).

## Rules
- Every change to `StorageComponent.Items` goes through these events so that
  the rest of the systems can react (UI, signals, log).
- `ItemReservedEvent` prevents double allocation: while an item is reserved,
  another haul request will not pick it up.
- `CraftRequestEvent` is **not** the start of crafting but a request: JobSystem
  prioritizes and assigns a pawn.

## Usage examples
```csharp
_bus.Publish(new CraftRequestEvent { /* RecipeId = "rifle", RequesterId = player */ });
// → JobSystem assigns a pawn → ItemReservedEvent for the components → WorkbenchSystem crafts
// → ItemRemovedEvent (components) + ItemAddedEvent (finished weapon)
```

## TODO
- [ ] Add `CraftCompletedEvent` / `CraftFailedEvent` — Phase 4.
- [ ] Decide whether to store `ItemTemplateId` in the event or pull it from the item's component.
