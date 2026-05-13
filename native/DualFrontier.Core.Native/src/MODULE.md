---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-NATIVE-CORE-SRC
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-NATIVE-CORE-SRC
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-NATIVE-CORE-SRC
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-NATIVE-CORE-SRC
---
# DualFrontier.Core.Native/src — Implementation Files

**Purpose**: C++ implementation files corresponding к headers в `../include/`.

**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §1.2

**Files** (post-K8):
- `world.cpp` — World methods
- `capi.cpp` — extern «C» wrappers
- `bootstrap_graph.cpp` — startup graph + Kahn's algorithm (K3 NEW)
- `thread_pool.cpp` — std::thread pool implementation (K3 NEW)
- `write_command_buffer.cpp` — mutation batch parser (K5 NEW)

**Compilation**:
- All files compiled into single shared library `DualFrontier.Core.Native.dll`
- CMakeLists.txt в parent directory orchestrates build

**Status**: K0 cherry-picks existing implementations; K3, K5 add new implementations.
