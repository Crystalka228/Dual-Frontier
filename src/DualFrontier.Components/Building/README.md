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
register_id: DOC-F-SRC-COMPONENTS-BUILDING
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
title: Components Building submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
