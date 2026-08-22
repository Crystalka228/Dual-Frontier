---
register_id: DOC-F-NATIVE-CORE
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-05-11
last_modified: 2026-08-22
content_language: en
next_review_due: null
title: Native Core module
last_modified_commit: 80c9ba6
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---

# DualFrontier.Core.Native — Module Documentation

**Purpose**: ECS kernel storage + bootstrap orchestration + thread pool. C++23 implementation built independently от .NET solution via CMake.

**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §1.2, §1.3, §1.4

**Public API surface** (post-K8):
- `df_capi.h` — extern «C» functions (~20 total)
- `df_world_*` — entity/component lifecycle
- `df_engine_bootstrap` — startup entry point (K3)
- `df_world_acquire_span` / `df_world_release_span` — span lifetime (K5)
- `df_world_acquire_versions` / `df_world_release_versions` — read-only view over the per-slot `versions_` table, indexed by ENTITY INDEX, so managed pair-iterators reconstruct TRUE generations instead of fabricating `Version = 0` (ID-B; К-L22; `IDENTITY_AND_ABI_CONTRACT.md` §2). While a view is held, a `df_world_create_entity` that would GROW the table is REFUSED (the resize would dangle the view's pointer; creating from the free list or into spare capacity is permitted), and `df_world_destroy_entity` / `df_world_flush_destroyed` are refused for the whole window. A NEGATIVE table entry is a tombstone: the slot's entity is destroyed but its component row awaits the flush, and `df_world_is_alive` rejects such a slot so ids reconstructed from it fail closed
- `df_world_flush_write_batch` — mutation flush (K5)
- `df_world_register_component_type` — type registration (K2)

**`DF_API` export census**: 209 — `df_capi.h` 155 · `pipeline_slot.h` 18 · `bus_native.h` 15 · `background_queue.h` 8 · `event_type_registry.h` 5 · `phase_compute.h` 5 · `mod_unload.h` 3. Measured by `grep -c "^DF_API" native/DualFrontier.Core.Native/include/*.h` (sum). Was 207 before the ID-B versions-view pair.

**Dependencies**:
- C++23 stdlib only (`<vector>`, `<unordered_map>`, `<thread>`, `<atomic>`)
- No third-party libraries
- CMake 3.20+ build system

**Output artifact**: `DualFrontier.Core.Native.dll` (Windows) / `.so` (Linux) / `.dylib` (macOS)

**Layering**: lowest layer — knows nothing of game domain. Could be open-sourced separately as «sparse-set ECS in C++ с C ABI».

**TODO list**:
- K0: cherry-pick existing implementation from experimental branch
- K1: bulk operations + span access functions
- K2: component type registry function
- K3: bootstrap_graph.h/cpp + thread_pool.h/cpp
- K5: write_command_buffer.h/cpp

**Status**: scaffolding only. Implementation lives in cherry-picked branch contents (K0) и subsequent K-series milestones.