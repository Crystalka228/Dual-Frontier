# Building

## Purpose
Components for buildings and their infrastructure: storages, workbenches.

(The v0.3 power components — `PowerConsumerComponent`, `PowerProducerComponent` — and the related events were deleted in A'.5 K8.3+K8.4 cutover 2026-05-14. Future electricity-like mechanics are routed to V substrate field/compute work per [VULKAN_SUBSTRATE](../../../docs/architecture/VULKAN_SUBSTRATE.md) §1.2 + §5.1.)

## Dependencies
- `DualFrontier.Contracts` — `IComponent`, `EntityId`.

## Contents
- `StorageComponent.cs` — storage (capacity + item list).
- `WorkbenchComponent.cs` — workbench (kind + work speed).

## Rules
- `StorageComponent.Items` is a list of entity IDs of items in the world (items
  are separate entities with their own components).

## Usage examples
```csharp
var crate = nativeWorld.CreateEntity();
using var batch = nativeWorld.BeginBatch<StorageComponent>();
batch.Set(crate, new StorageComponent { /* Capacity = 50 */ });
```

## TODO
- [ ] Define the `WorkbenchKind` enum (Cooking, Smithing, Research, GolemForge …).
- [ ] Plan equipment degradation/breakage — a separate `DurabilityComponent`.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS-BUILDING
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS-BUILDING
---
