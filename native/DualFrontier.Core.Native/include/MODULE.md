---
register_id: DOC-F-NATIVE-CORE-INCLUDE
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-05-11
last_modified: 2026-05-11
content_language: en
next_review_due: null
title: Native Core include
last_modified_commit: 80c9ba6
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---

# DualFrontier.Core.Native/include — Public Headers

**Purpose**: Public C ABI и C++ implementation headers.

**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §1.2

**Files** (post-K8):
- `df_capi.h` — public C ABI (extern «C» functions, DllExport/DllImport macros)
- `entity_id.h` — EntityId POD + pack/unpack helpers
- `sparse_set.h` — header-only template (or removed K0.2 if unused)
- `component_store.h` — type-erased RawComponentStore
- `world.h` — World class declaration
- `bootstrap_graph.h` — startup task graph (K3 NEW)
- `thread_pool.h` — std::thread pool (K3 NEW)
- `write_command_buffer.h` — mutation batch parser (K5 NEW)

**Visibility rules**:
- C ABI in `df_capi.h` — exported via `DF_API` macro
- C++ classes — `internal` к library, not exported
- Implementation в corresponding .cpp файлах в `../src/`

**Status**: K0 cherry-picks existing headers; K3, K5 add new headers.