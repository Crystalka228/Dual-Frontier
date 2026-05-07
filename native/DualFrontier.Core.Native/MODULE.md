# DualFrontier.Core.Native — Module Documentation

**Purpose**: ECS kernel storage + bootstrap orchestration + thread pool. C++20 implementation built independently от .NET solution via CMake.

**Reference**: `docs/KERNEL_ARCHITECTURE.md` §1.2, §1.3, §1.4

**Public API surface** (post-K8):
- `df_capi.h` — extern «C» functions (~20 total)
- `df_world_*` — entity/component lifecycle
- `df_engine_bootstrap` — startup entry point (K3)
- `df_world_acquire_span` / `df_world_release_span` — span lifetime (K5)
- `df_world_flush_write_batch` — mutation flush (K5)
- `df_world_register_component_type` — type registration (K2)

**Dependencies**:
- C++20 stdlib only (`<vector>`, `<unordered_map>`, `<thread>`, `<atomic>`)
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
