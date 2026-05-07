# DualFrontier.Core.Native/src — Implementation Files

**Purpose**: C++ implementation files corresponding к headers в `../include/`.

**Reference**: `docs/KERNEL_ARCHITECTURE.md` §1.2

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
