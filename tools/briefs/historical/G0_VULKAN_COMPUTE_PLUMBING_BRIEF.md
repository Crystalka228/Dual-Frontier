---
register_id: DOC-D-G0
project: Dual Frontier
category: D
tier: 3
lifecycle: SUPERSEDED
owner: Crystalka
version: 1.0
first_authored: 2026-05-16
last_modified: 2026-05-16
content_language: en
next_review_due: null
title: G0 — Vulkan Compute Plumbing (historical; consolidated into V0)
superseded_by: DOC-A-VULKAN_SUBSTRATE
review_cadence: on-status-transition
reviewer: Crystalka
risks_referenced:
- RISK-013
special_case_rationale: 'Q-G-2 LOCK: G0 consolidated into V0 (Vulkan substrate foundation, covers rendering + compute use cases). Brief content superseded by VULKAN_SUBSTRATE.md §1.1 + §3.4. Retained as historical record of pre-Q-G-1/Q-G-2 architectural intent. Moved to tools/briefs/historical/ per CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16).'
---

> **HISTORICAL DOCUMENT**: Superseded as of 2026-05-16. See `docs/architecture/VULKAN_SUBSTRATE.md` (current authority).
> Original lifecycle: AUTHORED. Disposition per Q-G-2 LOCK (composite namespace deliberation 2026-05-15).
> Preserved for historical record. Do not execute as current brief.

# G0 — Vulkan compute pipeline plumbing

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap §G0
**Prerequisites**: K9 closed; M9.0–M9.4 closed (Vulkan instance/device live)

## Goal

Establish compute path; no game-visible behavior yet. Native kernel gains linkage to `vulkan-1.dll` for compute dispatch (separate from rendering layer linkage); `VkBuffer`/storage image creation, descriptor sets, pipeline registration C ABI, fence-based sync, build-time SPIR-V compilation extending `RUNTIME_ARCHITECTURE.md` §1.7.

## Time estimate

~1 week

## Deliverables (high-level)

- `VkBuffer` / storage image creation для fields marked compute-managed
- `VkDescriptorSetLayout`, `VkDescriptorPool`, `VkPipelineLayout`
- `df_world_register_compute_pipeline` C ABI
- `df_world_field_dispatch_compute` C ABI
- Fence-based sync between CPU writes (conductivity updates) и GPU dispatch
- Build-time compute shader compilation in mod build process
- Tests: empty dispatch (no-op pipeline) executes без error; pipeline registration round-trip

## Success criteria

- Managed code can register a compute pipeline и trigger empty dispatch
- Framework operational без specific gameplay use yet

## Status: NOT STARTED

Awaiting K9 closed; M9.0–M9.4 closed.

См. также `MOD_OS_ARCHITECTURE.md` v1.6 §4.6 IModApi v3 для compute pipeline registration surface.