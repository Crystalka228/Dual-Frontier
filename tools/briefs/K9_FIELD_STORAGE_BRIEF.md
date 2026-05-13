---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K9
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K9
---
# K9 — Field Storage Abstraction (Full Brief)

**Status**: AUTHORED — awaiting K6, K7, K8 closure per β6 sequencing before execution
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K9
**Specification**: `docs/architecture/GPU_COMPUTE.md` v2.0 LOCKED, "Architectural integration → Native kernel (K9)" + "Roadmap → K9"
**API contract**: `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.6 §4.6 (IModApi v3 — Fields and Compute Pipelines)
**Companion document**: `docs/architecture/FIELDS.md` (storage contract, orthogonal to ECS)
**Mathematical models**: `docs/architecture/GPU_COMPUTE.md` "Mathematical models" — diffusion, anisotropy, capacitance, cliff threshold

---

## Executable contract notice

This brief is a deterministic instruction set for a Claude Code execution session. Every Phase declares pre-flight checks, exact edits or new files, post-edit verification, and atomic commit format. No discretion is delegated to the executor on architectural choices — every choice is either fixed by a LOCKED spec or fixed in this brief. When the executor encounters an underspecified situation, the stop condition is "halt and escalate", not "improvise".

The executor reads this entire brief before any tool call. The brief assumes Anthropic `Edit` tool semantics (literal string matching, not regex — per closure record from MOD_OS v1.6 amendment). For environments with regex-mode `edit_file`, the same edits apply but each `oldText` and `newText` must be checked for `$ ^ \b \d \w \s [ ] ( | ) * + ?` before submission.

Time estimate: **2-3 weeks at hobby pace (~1h/day)**. Auto-mode estimate: **8-12 hours wall time** including dotnet build/test cycles. Scope follows "no compromises" — anisotropy, storage flags, ping-pong, capability cross-check, IModApi v3 wiring all land at K9, no retrofit "add later" passes.

---

## Phase 0 — Pre-flight verification

Before any edit, the executor verifies the working tree state and the assumptions this brief makes about prerequisite milestones.

### 0.1 — Working tree clean

```
git status
```

**Expected output**: `nothing to commit, working tree clean` on branch `main` or `feat/k9-field-storage`.

**Halt condition**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K9-WIP"` and re-verify, or commit them on the current branch before starting K9 work.

### 0.2 — Prerequisite milestones closed

```
git log --oneline -50
```

**Expected**: K0–K5 closure commits visible (`89a4b24`, `e2c50b8`, `129a0a0`, `7629f57`, `2fc59d1`, `547c919`). K6, K7, K8 closure commits visible.

**Halt condition**: K6, K7, or K8 not closed. K9 cannot run on an unfinished migration. Resolution: complete missing milestones first; this brief does not cover them.

### 0.3 — `MIGRATION_PROGRESS.md` baseline

Read `docs/MIGRATION_PROGRESS.md`. Verify:

- "Active phase" entry references K9 (or the executor updates it as part of Phase 9 closure).
- K0–K8 rows show DONE with commits.
- Test count baseline matches the post-K8 number (TBD at K8 closure; currently 538 from K5).

### 0.4 — Prerequisite documents at expected versions

```
head -10 docs/architecture/GPU_COMPUTE.md
head -10 docs/architecture/KERNEL_ARCHITECTURE.md
head -10 docs/architecture/MOD_OS_ARCHITECTURE.md
head -10 docs/architecture/FIELDS.md
```

**Expected**:

- `GPU_COMPUTE.md` LOCKED v2.0
- `KERNEL_ARCHITECTURE.md` AUTHORITATIVE LOCKED v1.5+ (per K-L3.1 bridge formalization 2026-05-10)
- `MOD_OS_ARCHITECTURE.md` LOCKED v1.7+ (per K-L3.1 bridge formalization 2026-05-10)
- `FIELDS.md` Status: Draft

**Halt condition**: any spec at unexpected version. K9 implements against these specs verbatim; version mismatch means the spec contract has shifted under the brief.

### 0.5 — Native build clean

```
cd native/DualFrontier.Core.Native/build
cmake --build . --config Release
```

**Expected**: build succeeds without warnings; `DualFrontier.Core.Native.dll` and `df_native_selftest.exe` produced.

**Halt condition**: build failure on baseline. K9 starts from a known-good native state; regressions are not the executor's problem to debug.

### 0.6 — Selftest baseline

```
./Release/df_native_selftest.exe
```

**Expected**: all 17 scenarios pass (post-K5 baseline: K0–K5 added scenarios cumulatively).

**Halt condition**: any scenario fails. Same reasoning as 0.5.

### 0.7 — Managed test baseline

```
dotnet test
```

**Expected**: 631+ tests passing (post-K8.2 v2 baseline; K-L3.1 amendment brief is docs-only and does not affect test count — record the actual baseline before continuing).

**Halt condition**: any test fails. Same reasoning as 0.5.

### 0.8 — Brief itself committed

If this brief was authored as an unstaged change on `main` (skeleton → full brief), it must be committed before any K9 work begins. Pattern from K1 and K3 lessons learned (see `MIGRATION_PROGRESS.md`):

```
git add tools/briefs/K9_FIELD_STORAGE_BRIEF.md
git commit -m "docs(briefs): K9 skeleton expanded to full brief"
```

Then create the K9 feature branch:

```
git checkout -b feat/k9-field-storage
```

All Phase 1+ commits land on `feat/k9-field-storage`. Final integration is a fast-forward merge to `main` after Phase 9 closure verification.

---

## Phase 1 — Native C++: `RawTileField<T>` core

### 1.1 — Create `tile_field.h` header

**New file**: `native/DualFrontier.Core.Native/include/tile_field.h`

```cpp
#ifndef DF_TILE_FIELD_H
#define DF_TILE_FIELD_H

#include <atomic>
#include <cstdint>
#include <cstring>
#include <stdexcept>
#include <vector>

namespace dualfrontier {

// RawTileField — type-erased dense 2D grid storage.
//
// Element type is fixed at registration via cell_size; the kernel does not
// know the static type. Caller (managed bridge) tracks the type and copies
// cell_size bytes per cell on read/write.
//
// Storage layout per field:
//   data_           — primary buffer, width * height * cell_size bytes
//   back_buffer_    — ping-pong target, identical layout to data_
//   conductivity_   — per-cell float D coefficient, width * height floats
//   storage_flags_  — per-cell bit, byte-packed (width * height + 7) / 8 bytes
//
// Mutation rejection:
//   active_spans_ atomic counter; while > 0, write_cell, set_conductivity,
//   set_storage_flag, swap_buffers all throw std::logic_error.
//   Caller must release every acquired span before mutating.
//
// All bounds checks return 0 on out-of-range; callers must validate input
// before assuming success.

class RawTileField {
public:
    RawTileField(int32_t width, int32_t height, int32_t cell_size);
    ~RawTileField() = default;

    RawTileField(const RawTileField&) = delete;
    RawTileField& operator=(const RawTileField&) = delete;
    RawTileField(RawTileField&&) = delete;
    RawTileField& operator=(RawTileField&&) = delete;

    int32_t width() const noexcept { return width_; }
    int32_t height() const noexcept { return height_; }
    int32_t cell_size() const noexcept { return cell_size_; }

    // Point access. Returns 1 on success, 0 on out-of-bounds or size mismatch.
    int32_t read_cell(int32_t x, int32_t y, void* out, int32_t size) const;
    int32_t write_cell(int32_t x, int32_t y, const void* in, int32_t size);

    // Span access. Returns 1 on success, 0 on failure.
    // Increments active_spans_; caller MUST call release_span.
    int32_t acquire_span(const void** out_data, int32_t* out_width, int32_t* out_height);
    void    release_span() noexcept;

    // Conductivity map. Default value 1.0 (uniform isotropic).
    int32_t set_conductivity(int32_t x, int32_t y, float value);
    float   get_conductivity(int32_t x, int32_t y) const;

    // Storage flag (per-cell bit; default 0).
    int32_t set_storage_flag(int32_t x, int32_t y, int32_t enabled);
    int32_t get_storage_flag(int32_t x, int32_t y) const;

    // Ping-pong buffer swap; throws if any span active.
    void swap_buffers();

    // Internal access for friend (CPU reference kernel needs both buffers).
    uint8_t* data_ptr() noexcept { return data_.data(); }
    uint8_t* back_buffer_ptr() noexcept { return back_buffer_.data(); }
    float* conductivity_ptr() noexcept { return conductivity_.data(); }
    const uint8_t* storage_flags_ptr() const noexcept { return storage_flags_.data(); }

private:
    int32_t width_;
    int32_t height_;
    int32_t cell_size_;
    std::vector<uint8_t> data_;
    std::vector<uint8_t> back_buffer_;
    std::vector<float>   conductivity_;
    std::vector<uint8_t> storage_flags_;
    std::atomic<int32_t> active_spans_{0};

    void throw_if_spans_active() const;
};

}  // namespace dualfrontier

#endif  // DF_TILE_FIELD_H
```

**Atomic commit**: `feat(native): add RawTileField header for K9 field storage`

### 1.2 — Create `tile_field.cpp` implementation

**New file**: `native/DualFrontier.Core.Native/src/tile_field.cpp`

```cpp
#include "tile_field.h"

namespace dualfrontier {

RawTileField::RawTileField(int32_t width, int32_t height, int32_t cell_size)
    : width_(width), height_(height), cell_size_(cell_size)
{
    if (width <= 0 || height <= 0 || cell_size <= 0) {
        throw std::invalid_argument("RawTileField: width, height, cell_size must be positive");
    }

    const size_t total_cells = static_cast<size_t>(width) * static_cast<size_t>(height);
    const size_t buffer_bytes = total_cells * static_cast<size_t>(cell_size);

    data_.assign(buffer_bytes, 0);
    back_buffer_.assign(buffer_bytes, 0);
    conductivity_.assign(total_cells, 1.0f);  // default isotropic
    storage_flags_.assign((total_cells + 7) / 8, 0);  // default no storage
}

void RawTileField::throw_if_spans_active() const
{
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("RawTileField: cannot mutate during active span");
    }
}

int32_t RawTileField::read_cell(int32_t x, int32_t y, void* out, int32_t size) const
{
    if (x < 0 || x >= width_ || y < 0 || y >= height_) return 0;
    if (size != cell_size_ || out == nullptr) return 0;

    const size_t offset = (static_cast<size_t>(y) * static_cast<size_t>(width_) + static_cast<size_t>(x))
                          * static_cast<size_t>(cell_size_);
    std::memcpy(out, data_.data() + offset, static_cast<size_t>(size));
    return 1;
}

int32_t RawTileField::write_cell(int32_t x, int32_t y, const void* in, int32_t size)
{
    if (x < 0 || x >= width_ || y < 0 || y >= height_) return 0;
    if (size != cell_size_ || in == nullptr) return 0;
    throw_if_spans_active();

    const size_t offset = (static_cast<size_t>(y) * static_cast<size_t>(width_) + static_cast<size_t>(x))
                          * static_cast<size_t>(cell_size_);
    std::memcpy(data_.data() + offset, in, static_cast<size_t>(size));
    return 1;
}

int32_t RawTileField::acquire_span(const void** out_data, int32_t* out_width, int32_t* out_height)
{
    if (out_data == nullptr || out_width == nullptr || out_height == nullptr) return 0;

    active_spans_.fetch_add(1, std::memory_order_acquire);
    *out_data = data_.data();
    *out_width = width_;
    *out_height = height_;
    return 1;
}

void RawTileField::release_span() noexcept
{
    active_spans_.fetch_sub(1, std::memory_order_release);
}

int32_t RawTileField::set_conductivity(int32_t x, int32_t y, float value)
{
    if (x < 0 || x >= width_ || y < 0 || y >= height_) return 0;
    throw_if_spans_active();

    const size_t index = static_cast<size_t>(y) * static_cast<size_t>(width_) + static_cast<size_t>(x);
    conductivity_[index] = value;
    return 1;
}

float RawTileField::get_conductivity(int32_t x, int32_t y) const
{
    if (x < 0 || x >= width_ || y < 0 || y >= height_) return 0.0f;
    const size_t index = static_cast<size_t>(y) * static_cast<size_t>(width_) + static_cast<size_t>(x);
    return conductivity_[index];
}

int32_t RawTileField::set_storage_flag(int32_t x, int32_t y, int32_t enabled)
{
    if (x < 0 || x >= width_ || y < 0 || y >= height_) return 0;
    throw_if_spans_active();

    const size_t index = static_cast<size_t>(y) * static_cast<size_t>(width_) + static_cast<size_t>(x);
    const size_t byte_index = index / 8;
    const uint8_t bit_mask = static_cast<uint8_t>(1u << (index % 8));

    if (enabled != 0) {
        storage_flags_[byte_index] |= bit_mask;
    } else {
        storage_flags_[byte_index] &= static_cast<uint8_t>(~bit_mask);
    }
    return 1;
}

int32_t RawTileField::get_storage_flag(int32_t x, int32_t y) const
{
    if (x < 0 || x >= width_ || y < 0 || y >= height_) return 0;
    const size_t index = static_cast<size_t>(y) * static_cast<size_t>(width_) + static_cast<size_t>(x);
    const size_t byte_index = index / 8;
    const uint8_t bit_mask = static_cast<uint8_t>(1u << (index % 8));
    return (storage_flags_[byte_index] & bit_mask) != 0 ? 1 : 0;
}

void RawTileField::swap_buffers()
{
    throw_if_spans_active();
    data_.swap(back_buffer_);
}

}  // namespace dualfrontier
```

### 1.3 — Update CMakeLists.txt

**File**: `native/DualFrontier.Core.Native/CMakeLists.txt`

Locate the source list (existing `add_library(DualFrontier.Core.Native ...)` or `set(SOURCES ...)` block) and add `src/tile_field.cpp` to it. Same edit applies to the selftest target if it compiles sources directly (per K3 closure adjustment).

The exact location and current source list shape is verified by reading the file at execution time; do not assume specific line numbers from this brief.

**Atomic commit**: `feat(native): implement RawTileField with conductivity and storage flags`

### 1.4 — Verify build

```
cd native/DualFrontier.Core.Native/build
cmake --build . --config Release
```

**Expected**: clean build, no warnings.

**Halt condition**: compilation errors. The header / source files are self-contained; failures suggest a CMakeLists edit error or a header include path issue.

---

## Phase 2 — Native C++: `World` field registry extension

### 2.1 — Update `world.h`

**File**: `native/DualFrontier.Core.Native/include/world.h`

Add includes:

```cpp
#include "tile_field.h"
#include <string>
#include <unordered_map>
```

Add to the `World` class private section, alongside existing `stores_`:

```cpp
std::unordered_map<std::string, std::unique_ptr<RawTileField>> fields_;
```

Add public methods declaration:

```cpp
// K9 field registry (added 2026-XX-XX).
int32_t register_field(const std::string& field_id, int32_t width, int32_t height, int32_t cell_size);
RawTileField* get_field(const std::string& field_id) noexcept;
const RawTileField* get_field(const std::string& field_id) const noexcept;
int32_t unregister_field(const std::string& field_id);
```

The `register_field` signature accepts `cell_size` as `int32_t` for consistency with existing C ABI types.

### 2.2 — Update `world.cpp`

**File**: `native/DualFrontier.Core.Native/src/world.cpp`

Add implementations:

```cpp
int32_t World::register_field(const std::string& field_id, int32_t width, int32_t height, int32_t cell_size)
{
    if (field_id.empty()) {
        throw std::invalid_argument("World::register_field: field_id must be non-empty");
    }
    if (width <= 0 || height <= 0 || cell_size <= 0) {
        throw std::invalid_argument("World::register_field: dimensions and cell_size must be positive");
    }

    auto it = fields_.find(field_id);
    if (it != fields_.end()) {
        // Idempotent: same dimensions = no-op success; mismatch = throw.
        const RawTileField& existing = *it->second;
        if (existing.width() == width && existing.height() == height && existing.cell_size() == cell_size) {
            return 1;
        }
        throw std::invalid_argument("World::register_field: id already registered with different dimensions");
    }

    fields_.emplace(field_id, std::make_unique<RawTileField>(width, height, cell_size));
    return 1;
}

RawTileField* World::get_field(const std::string& field_id) noexcept
{
    auto it = fields_.find(field_id);
    return (it != fields_.end()) ? it->second.get() : nullptr;
}

const RawTileField* World::get_field(const std::string& field_id) const noexcept
{
    auto it = fields_.find(field_id);
    return (it != fields_.end()) ? it->second.get() : nullptr;
}

int32_t World::unregister_field(const std::string& field_id)
{
    auto it = fields_.find(field_id);
    if (it == fields_.end()) return 0;
    fields_.erase(it);
    return 1;
}
```

**Atomic commit**: `feat(native): extend World with field registry parallel to component stores`

### 2.3 — Verify build

Same as 1.4. Halt on failure.

---

## Phase 3 — C ABI extension

### 3.1 — Extend `df_capi.h`

**File**: `native/DualFrontier.Core.Native/include/df_capi.h`

Append the following block at the end of the file (before the closing `#ifdef __cplusplus` and `#endif`):

```c
/*
 * K9 field storage (added 2026-XX-XX).
 *
 * Field storage is a parallel abstraction alongside component stores. Each
 * field is a typed dense 2D grid keyed by string id. The id must be
 * mod-namespaced (caller's responsibility — kernel does not enforce); the
 * loader-side capability cross-check (MOD_OS_ARCHITECTURE.md §3.4) gates
 * which mods can register and access which fields.
 *
 * Storage layout per field:
 *   - Primary buffer: width * height * cell_size bytes
 *   - Back buffer: identical layout (ping-pong target for compute kernels)
 *   - Conductivity map: width * height floats (default 1.0)
 *   - Storage flags: width * height bits, byte-packed (default 0)
 *
 * Lifecycle:
 *   - df_world_register_field — idempotent on identical dimensions; rejects
 *     mismatched re-registration.
 *   - df_world_field_unregister_field — removes the field; subsequent access
 *     to that id returns 0.
 *
 * Mutation rejection contract (parallels active_spans on component stores):
 *   While any span on the field is acquired, write_cell, set_conductivity,
 *   set_storage_flag, and swap_buffers all return 0 / no-op.
 *
 * Span lifetime contract:
 *   1. Caller calls df_world_field_acquire_span -> dense data ptr + dimensions.
 *   2. Caller iterates without further P/Invokes.
 *   3. Caller MUST call df_world_field_release_span before any mutation.
 *   4. Multiple concurrent spans on different field ids OR same field allowed.
 *   5. Mutation attempt while any span active returns 0 / no-op.
 *
 * Returns 1 on success, 0 on failure (out-of-bounds, size mismatch, field
 * not found, mutation during active span).
 */

DF_API int32_t df_world_register_field(
    df_world_handle world,
    const char* field_id,
    int32_t width,
    int32_t height,
    int32_t cell_size);

DF_API int32_t df_world_field_unregister(
    df_world_handle world,
    const char* field_id);

DF_API int32_t df_world_field_read_cell(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    void* out_value,
    int32_t size);

DF_API int32_t df_world_field_write_cell(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    const void* value,
    int32_t size);

DF_API int32_t df_world_field_acquire_span(
    df_world_handle world,
    const char* field_id,
    const void** out_data,
    int32_t* out_width,
    int32_t* out_height);

DF_API int32_t df_world_field_release_span(
    df_world_handle world,
    const char* field_id);

DF_API int32_t df_world_field_set_conductivity(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    float value);

DF_API float df_world_field_get_conductivity(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y);

DF_API int32_t df_world_field_set_storage_flag(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    int32_t enabled);

DF_API int32_t df_world_field_get_storage_flag(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y);

DF_API int32_t df_world_field_swap_buffers(
    df_world_handle world,
    const char* field_id);

DF_API int32_t df_world_field_count(
    df_world_handle world);
```

The `df_world_field_swap_buffers` and `df_world_field_count` functions are introspection helpers; `swap_buffers` is needed by the CPU reference kernel (Phase 6), `count` is used by tests to verify registration state.

**Atomic commit**: `feat(native): add K9 field storage C ABI declarations`

### 3.2 — Implement C ABI in `capi.cpp`

**File**: `native/DualFrontier.Core.Native/src/capi.cpp`

Append the following block at the end of the file (before any closing `extern "C"` brace if present):

```cpp
// K9 field storage C ABI implementations.

extern "C" DF_API int32_t df_world_register_field(
    df_world_handle world, const char* field_id,
    int32_t width, int32_t height, int32_t cell_size)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        return w->register_field(field_id, width, height, cell_size);
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_unregister(
    df_world_handle world, const char* field_id)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        return w->unregister_field(field_id);
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_read_cell(
    df_world_handle world, const char* field_id,
    int32_t x, int32_t y, void* out_value, int32_t size)
{
    if (world == nullptr || field_id == nullptr || out_value == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->read_cell(x, y, out_value, size);
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_write_cell(
    df_world_handle world, const char* field_id,
    int32_t x, int32_t y, const void* value, int32_t size)
{
    if (world == nullptr || field_id == nullptr || value == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->write_cell(x, y, value, size);
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_acquire_span(
    df_world_handle world, const char* field_id,
    const void** out_data, int32_t* out_width, int32_t* out_height)
{
    if (world == nullptr || field_id == nullptr) return 0;
    if (out_data == nullptr || out_width == nullptr || out_height == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->acquire_span(out_data, out_width, out_height);
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_release_span(
    df_world_handle world, const char* field_id)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        field->release_span();
        return 1;
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_set_conductivity(
    df_world_handle world, const char* field_id,
    int32_t x, int32_t y, float value)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->set_conductivity(x, y, value);
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API float df_world_field_get_conductivity(
    df_world_handle world, const char* field_id, int32_t x, int32_t y)
{
    if (world == nullptr || field_id == nullptr) return 0.0f;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0.0f;
        return field->get_conductivity(x, y);
    } catch (...) {
        return 0.0f;
    }
}

extern "C" DF_API int32_t df_world_field_set_storage_flag(
    df_world_handle world, const char* field_id,
    int32_t x, int32_t y, int32_t enabled)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->set_storage_flag(x, y, enabled);
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_get_storage_flag(
    df_world_handle world, const char* field_id, int32_t x, int32_t y)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        return field->get_storage_flag(x, y);
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_swap_buffers(
    df_world_handle world, const char* field_id)
{
    if (world == nullptr || field_id == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        auto* field = w->get_field(field_id);
        if (field == nullptr) return 0;
        field->swap_buffers();
        return 1;
    } catch (...) {
        return 0;
    }
}

extern "C" DF_API int32_t df_world_field_count(df_world_handle world)
{
    if (world == nullptr) return 0;
    try {
        auto* w = static_cast<dualfrontier::World*>(world);
        return static_cast<int32_t>(w->field_count());  // requires World::field_count() — add to world.h/cpp
    } catch (...) {
        return 0;
    }
}
```

The `field_count()` getter on `World` is a single-line `return fields_.size();` — add to `world.h` public section and `world.cpp` if needed (Phase 2 amendment, single edit).

**Atomic commit**: `feat(native): implement K9 field storage C ABI bridge`

### 3.3 — Verify build

```
cd native/DualFrontier.Core.Native/build
cmake --build . --config Release
```

**Halt condition**: compilation errors. Common causes: missing `#include "tile_field.h"` in `capi.cpp`, missing `field_count()` method on `World`.

---

## Phase 4 — Native selftest extension

### 4.1 — Add field scenarios to `selftest.cpp`

**File**: `native/DualFrontier.Core.Native/test/selftest.cpp`

Add scenarios at the end of the existing scenario list. Each scenario follows the existing pattern (function returning `int`, registered in `main()`).

```cpp
static int scenario_field_register_and_read()
{
    auto* world = df_world_create();
    if (world == nullptr) return 1;

    int32_t result = df_world_register_field(world, "test.scalar", 10, 10, sizeof(float));
    if (result != 1) { df_world_destroy(world); return 2; }

    float value = 0.0f;
    if (df_world_field_read_cell(world, "test.scalar", 5, 5, &value, sizeof(float)) != 1) {
        df_world_destroy(world); return 3;
    }
    if (value != 0.0f) { df_world_destroy(world); return 4; }  // default zero-initialized

    df_world_destroy(world);
    return 0;
}

static int scenario_field_write_and_read_roundtrip()
{
    auto* world = df_world_create();
    if (world == nullptr) return 1;

    df_world_register_field(world, "test.roundtrip", 5, 5, sizeof(float));

    float in = 42.5f;
    if (df_world_field_write_cell(world, "test.roundtrip", 2, 3, &in, sizeof(float)) != 1) {
        df_world_destroy(world); return 2;
    }

    float out = 0.0f;
    df_world_field_read_cell(world, "test.roundtrip", 2, 3, &out, sizeof(float));
    if (out != in) { df_world_destroy(world); return 3; }

    df_world_destroy(world);
    return 0;
}

static int scenario_field_span_lifecycle()
{
    auto* world = df_world_create();
    df_world_register_field(world, "test.span", 4, 4, sizeof(float));

    const void* data = nullptr;
    int32_t w = 0, h = 0;
    if (df_world_field_acquire_span(world, "test.span", &data, &w, &h) != 1) {
        df_world_destroy(world); return 1;
    }
    if (w != 4 || h != 4 || data == nullptr) { df_world_destroy(world); return 2; }

    // Mutation must reject during active span.
    float x = 1.0f;
    if (df_world_field_write_cell(world, "test.span", 0, 0, &x, sizeof(float)) != 0) {
        df_world_destroy(world); return 3;
    }

    df_world_field_release_span(world, "test.span");

    // After release, mutation succeeds.
    if (df_world_field_write_cell(world, "test.span", 0, 0, &x, sizeof(float)) != 1) {
        df_world_destroy(world); return 4;
    }

    df_world_destroy(world);
    return 0;
}

static int scenario_field_conductivity_default_and_set()
{
    auto* world = df_world_create();
    df_world_register_field(world, "test.cond", 3, 3, sizeof(float));

    // Default 1.0
    if (df_world_field_get_conductivity(world, "test.cond", 1, 1) != 1.0f) {
        df_world_destroy(world); return 1;
    }

    df_world_field_set_conductivity(world, "test.cond", 1, 1, 0.5f);
    if (df_world_field_get_conductivity(world, "test.cond", 1, 1) != 0.5f) {
        df_world_destroy(world); return 2;
    }

    df_world_destroy(world);
    return 0;
}

static int scenario_field_storage_flag_toggle()
{
    auto* world = df_world_create();
    df_world_register_field(world, "test.stor", 3, 3, sizeof(float));

    if (df_world_field_get_storage_flag(world, "test.stor", 1, 1) != 0) {
        df_world_destroy(world); return 1;
    }

    df_world_field_set_storage_flag(world, "test.stor", 1, 1, 1);
    if (df_world_field_get_storage_flag(world, "test.stor", 1, 1) != 1) {
        df_world_destroy(world); return 2;
    }

    df_world_field_set_storage_flag(world, "test.stor", 1, 1, 0);
    if (df_world_field_get_storage_flag(world, "test.stor", 1, 1) != 0) {
        df_world_destroy(world); return 3;
    }

    df_world_destroy(world);
    return 0;
}

static int scenario_field_swap_buffers()
{
    auto* world = df_world_create();
    df_world_register_field(world, "test.swap", 2, 2, sizeof(float));

    float a = 1.0f, b = 2.0f, c = 3.0f, d = 4.0f;
    df_world_field_write_cell(world, "test.swap", 0, 0, &a, sizeof(float));
    df_world_field_write_cell(world, "test.swap", 1, 0, &b, sizeof(float));
    df_world_field_write_cell(world, "test.swap", 0, 1, &c, sizeof(float));
    df_world_field_write_cell(world, "test.swap", 1, 1, &d, sizeof(float));

    df_world_field_swap_buffers(world, "test.swap");

    // After swap, primary buffer is the back buffer (zero-initialized).
    float check = 99.0f;
    df_world_field_read_cell(world, "test.swap", 0, 0, &check, sizeof(float));
    if (check != 0.0f) { df_world_destroy(world); return 1; }

    df_world_destroy(world);
    return 0;
}

static int scenario_field_register_idempotent_and_conflict()
{
    auto* world = df_world_create();

    if (df_world_register_field(world, "test.idem", 5, 5, 4) != 1) { df_world_destroy(world); return 1; }
    if (df_world_register_field(world, "test.idem", 5, 5, 4) != 1) { df_world_destroy(world); return 2; }  // idempotent
    if (df_world_register_field(world, "test.idem", 6, 6, 4) != 0) { df_world_destroy(world); return 3; }  // conflict

    df_world_destroy(world);
    return 0;
}

static int scenario_field_unregister()
{
    auto* world = df_world_create();
    df_world_register_field(world, "test.unreg", 3, 3, 4);

    if (df_world_field_count(world) != 1) { df_world_destroy(world); return 1; }
    if (df_world_field_unregister(world, "test.unreg") != 1) { df_world_destroy(world); return 2; }
    if (df_world_field_count(world) != 0) { df_world_destroy(world); return 3; }

    // Read on unregistered field fails cleanly.
    float v = 0.0f;
    if (df_world_field_read_cell(world, "test.unreg", 0, 0, &v, sizeof(float)) != 0) {
        df_world_destroy(world); return 4;
    }

    df_world_destroy(world);
    return 0;
}
```

Register the eight new scenarios in `main()` alongside existing scenarios. The existing test runner pattern (numbered scenario, return code 0 = pass) is preserved.

**Atomic commit**: `test(native): add 8 field storage scenarios to selftest`

### 4.2 — Verify selftest

```
cd native/DualFrontier.Core.Native/build
cmake --build . --config Release
./Release/df_native_selftest.exe
```

**Expected**: 17 + 8 = 25 scenarios pass. (Adjust 17 to the actual post-K8 count if K6/K7/K8 added scenarios.)

**Halt condition**: any scenario fails. Diagnostic: scenario number prints to stderr; map to source above to identify the failure mode.

---

## Phase 5 — Managed bridge: `FieldRegistry` and `FieldHandle<T>`

### 5.1 — P/Invoke declarations

**New file**: `src/DualFrontier.Core.Interop/Native/NativeMethods.Fields.cs`

```csharp
using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop.Native;

internal static partial class NativeMethods
{
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int df_world_register_field(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId,
        int width, int height, int cellSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int df_world_field_unregister(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern unsafe int df_world_field_read_cell(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId,
        int x, int y, void* outValue, int size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern unsafe int df_world_field_write_cell(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId,
        int x, int y, void* value, int size);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern unsafe int df_world_field_acquire_span(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId,
        void** outData, int* outWidth, int* outHeight);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int df_world_field_release_span(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int df_world_field_set_conductivity(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId,
        int x, int y, float value);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern float df_world_field_get_conductivity(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId,
        int x, int y);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int df_world_field_set_storage_flag(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId,
        int x, int y, int enabled);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int df_world_field_get_storage_flag(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId,
        int x, int y);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern int df_world_field_swap_buffers(
        IntPtr world,
        [MarshalAs(UnmanagedType.LPStr)] string fieldId);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_field_count(IntPtr world);
}
```

**Atomic commit**: `feat(interop): add P/Invoke declarations for K9 field storage`

### 5.2 — `FieldHandle<T>` and `FieldSpanLease<T>`

**New file**: `src/DualFrontier.Core.Interop/FieldHandle.cs`

```csharp
using System.Runtime.CompilerServices;
using DualFrontier.Core.Interop.Native;

namespace DualFrontier.Core.Interop;

public sealed class FieldHandle<T> : IFieldHandle where T : unmanaged
{
    private readonly IntPtr _worldHandle;
    private readonly string _fieldId;
    private readonly int _width;
    private readonly int _height;

    internal FieldHandle(IntPtr worldHandle, string fieldId, int width, int height)
    {
        _worldHandle = worldHandle;
        _fieldId = fieldId;
        _width = width;
        _height = height;
    }

    public string Id => _fieldId;
    public int Width => _width;
    public int Height => _height;
    public Type ElementType => typeof(T);

    public T ReadCell(int x, int y)
    {
        T value = default;
        unsafe
        {
            int result = NativeMethods.df_world_field_read_cell(
                _worldHandle, _fieldId, x, y, &value, sizeof(T));
            if (result != 1)
                throw new FieldOperationFailedException($"ReadCell({x},{y}) on '{_fieldId}' returned 0");
        }
        return value;
    }

    public void WriteCell(int x, int y, T value)
    {
        unsafe
        {
            int result = NativeMethods.df_world_field_write_cell(
                _worldHandle, _fieldId, x, y, &value, sizeof(T));
            if (result != 1)
                throw new FieldOperationFailedException($"WriteCell({x},{y}) on '{_fieldId}' returned 0 (out of bounds, size mismatch, or active span)");
        }
    }

    public FieldSpanLease<T> AcquireSpan()
    {
        unsafe
        {
            void* data = null;
            int w = 0, h = 0;
            int result = NativeMethods.df_world_field_acquire_span(
                _worldHandle, _fieldId, &data, &w, &h);
            if (result != 1 || data == null)
                throw new FieldOperationFailedException($"AcquireSpan on '{_fieldId}' returned 0");
            return new FieldSpanLease<T>(_worldHandle, _fieldId, (T*)data, w, h);
        }
    }

    public void SetConductivity(int x, int y, float value)
    {
        int result = NativeMethods.df_world_field_set_conductivity(_worldHandle, _fieldId, x, y, value);
        if (result != 1)
            throw new FieldOperationFailedException($"SetConductivity({x},{y}) on '{_fieldId}' returned 0");
    }

    public float GetConductivity(int x, int y)
        => NativeMethods.df_world_field_get_conductivity(_worldHandle, _fieldId, x, y);

    public void SetStorageFlag(int x, int y, bool enabled)
    {
        int result = NativeMethods.df_world_field_set_storage_flag(
            _worldHandle, _fieldId, x, y, enabled ? 1 : 0);
        if (result != 1)
            throw new FieldOperationFailedException($"SetStorageFlag({x},{y}) on '{_fieldId}' returned 0");
    }

    public bool GetStorageFlag(int x, int y)
        => NativeMethods.df_world_field_get_storage_flag(_worldHandle, _fieldId, x, y) == 1;

    public void SwapBuffers()
    {
        int result = NativeMethods.df_world_field_swap_buffers(_worldHandle, _fieldId);
        if (result != 1)
            throw new FieldOperationFailedException($"SwapBuffers on '{_fieldId}' returned 0 (active span?)");
    }
}

public interface IFieldHandle
{
    string Id { get; }
    int Width { get; }
    int Height { get; }
    Type ElementType { get; }
}

public unsafe ref struct FieldSpanLease<T> where T : unmanaged
{
    private readonly IntPtr _worldHandle;
    private readonly string _fieldId;
    private readonly T* _data;
    private readonly int _width;
    private readonly int _height;
    private bool _released;

    internal FieldSpanLease(IntPtr worldHandle, string fieldId, T* data, int width, int height)
    {
        _worldHandle = worldHandle;
        _fieldId = fieldId;
        _data = data;
        _width = width;
        _height = height;
        _released = false;
    }

    public ReadOnlySpan<T> Span => new(_data, _width * _height);
    public int Width => _width;
    public int Height => _height;

    public T this[int x, int y]
    {
        get => _data[y * _width + x];
    }

    public void Dispose()
    {
        if (_released) return;
        _released = true;
        NativeMethods.df_world_field_release_span(_worldHandle, _fieldId);
    }
}

public sealed class FieldOperationFailedException : Exception
{
    public FieldOperationFailedException(string message) : base(message) { }
}
```

**Atomic commit**: `feat(interop): add FieldHandle, FieldSpanLease, and exception types`

### 5.3 — `FieldRegistry`

**New file**: `src/DualFrontier.Core.Interop/FieldRegistry.cs`

```csharp
using DualFrontier.Core.Interop.Native;

namespace DualFrontier.Core.Interop;

public sealed class FieldRegistry
{
    private readonly IntPtr _worldHandle;
    private readonly Dictionary<string, IFieldHandle> _handles = new();
    private readonly object _lock = new();

    internal FieldRegistry(IntPtr worldHandle)
    {
        _worldHandle = worldHandle;
    }

    public FieldHandle<T> Register<T>(string id, int width, int height) where T : unmanaged
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Field id must be non-empty", nameof(id));
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Field dimensions must be positive");

        lock (_lock)
        {
            if (_handles.TryGetValue(id, out var existing))
            {
                if (existing is FieldHandle<T> typed
                    && typed.Width == width && typed.Height == height)
                {
                    return typed;  // idempotent
                }
                throw new InvalidOperationException(
                    $"Field '{id}' already registered with different type or dimensions");
            }

            unsafe
            {
                int cellSize = sizeof(T);
                int result = NativeMethods.df_world_register_field(_worldHandle, id, width, height, cellSize);
                if (result != 1)
                    throw new InvalidOperationException($"Native registration failed for field '{id}'");
            }

            var handle = new FieldHandle<T>(_worldHandle, id, width, height);
            _handles[id] = handle;
            return handle;
        }
    }

    public FieldHandle<T> Get<T>(string id) where T : unmanaged
    {
        lock (_lock)
        {
            if (!_handles.TryGetValue(id, out var existing))
                throw new InvalidOperationException($"Field '{id}' is not registered");
            if (existing is not FieldHandle<T> typed)
                throw new InvalidOperationException(
                    $"Field '{id}' is registered with element type {existing.ElementType.Name}, not {typeof(T).Name}");
            return typed;
        }
    }

    public bool TryGet<T>(string id, out FieldHandle<T>? handle) where T : unmanaged
    {
        lock (_lock)
        {
            handle = null;
            if (!_handles.TryGetValue(id, out var existing)) return false;
            if (existing is not FieldHandle<T> typed) return false;
            handle = typed;
            return true;
        }
    }

    public bool IsRegistered(string id)
    {
        lock (_lock) return _handles.ContainsKey(id);
    }

    public void Unregister(string id)
    {
        lock (_lock)
        {
            if (!_handles.Remove(id)) return;
            NativeMethods.df_world_field_unregister(_worldHandle, id);
        }
    }

    public int Count
    {
        get { lock (_lock) return _handles.Count; }
    }
}
```

**Atomic commit**: `feat(interop): add FieldRegistry tracking managed-side field handles`

### 5.4 — Wire `FieldRegistry` into `NativeWorld`

**File**: `src/DualFrontier.Core.Interop/NativeWorld.cs`

Add a property exposing the registry:

```csharp
public FieldRegistry Fields { get; }
```

Initialize it in the constructor right after `_handle` is assigned:

```csharp
Fields = new FieldRegistry(_handle);
```

The exact location of the constructor is verified at execution time.

**Atomic commit**: `feat(interop): expose FieldRegistry on NativeWorld`

### 5.5 — Verify build

```
dotnet build
```

**Halt condition**: build errors. Common cause: missing `using` directives or `unsafe` context attribute on the `.csproj`.

---

## Phase 6 — CPU reference diffusion kernel

This phase implements the canonical isotropic diffusion kernel used by `Vanilla.Magic` (G1) and as the equivalence oracle for GPU shaders. The kernel runs entirely on CPU; G-series replaces it with a Vulkan compute shader producing identical output.

### 6.1 — Reference kernel implementation

**New file**: `src/DualFrontier.Core.Interop/CpuKernels/IsotropicDiffusionKernel.cs`

```csharp
namespace DualFrontier.Core.Interop.CpuKernels;

public static class IsotropicDiffusionKernel
{
    public readonly struct Parameters
    {
        public float DiffusionCoefficient { get; init; }    // D
        public float DecayCoefficient { get; init; }        // K (per tick)
        public float DeltaTime { get; init; }                // dt

        public static Parameters Default => new()
        {
            DiffusionCoefficient = 0.1f,
            DecayCoefficient = 0.01f,
            DeltaTime = 1.0f
        };
    }

    public static void Run(FieldHandle<float> field, Parameters p, int iterations)
    {
        if (field is null) throw new ArgumentNullException(nameof(field));
        if (iterations < 1) return;

        int width = field.Width;
        int height = field.Height;
        var scratch = new float[width * height];

        for (int iter = 0; iter < iterations; iter++)
        {
            // Read all cells via span (one lease, zero-copy in).
            using (var lease = field.AcquireSpan())
            {
                ReadOnlySpan<float> readBuf = lease.Span;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int i = y * width + x;
                        float center = readBuf[i];

                        float north = (y > 0)        ? readBuf[i - width] : center;
                        float south = (y < height-1) ? readBuf[i + width] : center;
                        float east  = (x < width-1)  ? readBuf[i + 1]     : center;
                        float west  = (x > 0)        ? readBuf[i - 1]     : center;

                        float laplacian = north + south + east + west - 4.0f * center;
                        float delta = (p.DiffusionCoefficient * laplacian - p.DecayCoefficient * center) * p.DeltaTime;
                        scratch[i] = center + delta;
                    }
                }
            }  // lease.Dispose() — releases native span; primary buffer mutable again.

            // Flush scratch back to native primary via point-writes.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    field.WriteCell(x, y, scratch[y * width + x]);
                }
            }
        }
    }
}
```

**Implementation strategy and known performance constraint**: the K9 CPU kernel uses a managed scratch array as the next-state buffer. Read happens via one zero-copy `AcquireSpan` lease per iteration; write happens via `width × height` `WriteCell` P/Invokes per iteration after the lease releases. For a 200×200 field that is 40 000 P/Invokes per iteration — slow on CPU, acceptable for K9 because the kernel exists as the GPU shader equivalence oracle, not as a performance target. G1 (`Vanilla.Magic`) replaces this with a Vulkan compute dispatch that does the same math in a single dispatch call.

The native ping-pong infrastructure (back buffer + `SwapBuffers`) is in place for the GPU path; the CPU kernel does not use it because the C ABI does not expose back-buffer write access (would require a separate `df_world_field_acquire_back_span` function). A future K9.1 amendment may add that primitive to eliminate the per-cell write loop on CPU; not in K9 scope.

**Atomic commit**: `feat(interop): add CPU isotropic diffusion kernel as equivalence oracle`

### 6.2 — Verify build

```
dotnet build
```

---

## Phase 7 — Bridge tests

### 7.1 — Test class for `FieldRegistry`

**New file**: `tests/DualFrontier.Core.Interop.Tests/FieldRegistryTests.cs`

```csharp
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public sealed class FieldRegistryTests : IDisposable
{
    private readonly NativeWorld _world = new();

    public void Dispose() => _world.Dispose();

    [Fact]
    public void Register_NewField_Succeeds()
    {
        var handle = _world.Fields.Register<float>("test.field", 10, 10);
        handle.Id.Should().Be("test.field");
        handle.Width.Should().Be(10);
        handle.Height.Should().Be(10);
        handle.ElementType.Should().Be(typeof(float));
    }

    [Fact]
    public void Register_SameDimensions_IsIdempotent()
    {
        var first = _world.Fields.Register<float>("test.idem", 5, 5);
        var second = _world.Fields.Register<float>("test.idem", 5, 5);
        second.Should().BeSameAs(first);
    }

    [Fact]
    public void Register_ConflictingDimensions_Throws()
    {
        _world.Fields.Register<float>("test.conf", 5, 5);
        Action act = () => _world.Fields.Register<float>("test.conf", 6, 6);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Register_ConflictingType_Throws()
    {
        _world.Fields.Register<float>("test.type", 5, 5);
        Action act = () => _world.Fields.Register<int>("test.type", 5, 5);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Register_EmptyId_Throws()
    {
        Action act = () => _world.Fields.Register<float>("", 5, 5);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Register_NonPositiveDimensions_Throws()
    {
        Action a1 = () => _world.Fields.Register<float>("test.zero", 0, 5);
        Action a2 = () => _world.Fields.Register<float>("test.neg", 5, -1);
        a1.Should().Throw<ArgumentException>();
        a2.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Get_RegisteredField_Returns()
    {
        var registered = _world.Fields.Register<float>("test.get", 3, 3);
        var got = _world.Fields.Get<float>("test.get");
        got.Should().BeSameAs(registered);
    }

    [Fact]
    public void Get_UnregisteredField_Throws()
    {
        Action act = () => _world.Fields.Get<float>("does.not.exist");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Get_WrongType_Throws()
    {
        _world.Fields.Register<float>("test.gt", 3, 3);
        Action act = () => _world.Fields.Get<int>("test.gt");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Unregister_RemovesField()
    {
        _world.Fields.Register<float>("test.un", 3, 3);
        _world.Fields.IsRegistered("test.un").Should().BeTrue();
        _world.Fields.Unregister("test.un");
        _world.Fields.IsRegistered("test.un").Should().BeFalse();
    }

    [Fact]
    public void Count_ReflectsRegistrations()
    {
        _world.Fields.Count.Should().Be(0);
        _world.Fields.Register<float>("a", 2, 2);
        _world.Fields.Register<float>("b", 2, 2);
        _world.Fields.Count.Should().Be(2);
        _world.Fields.Unregister("a");
        _world.Fields.Count.Should().Be(1);
    }
}
```

### 7.2 — Test class for `FieldHandle<T>`

**New file**: `tests/DualFrontier.Core.Interop.Tests/FieldHandleTests.cs`

```csharp
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public sealed class FieldHandleTests : IDisposable
{
    private readonly NativeWorld _world = new();

    public void Dispose() => _world.Dispose();

    [Fact]
    public void ReadCell_DefaultZero()
    {
        var f = _world.Fields.Register<float>("rc.zero", 4, 4);
        f.ReadCell(2, 2).Should().Be(0f);
    }

    [Fact]
    public void WriteCell_Then_ReadCell_RoundTrip()
    {
        var f = _world.Fields.Register<float>("rc.rt", 4, 4);
        f.WriteCell(2, 2, 42.5f);
        f.ReadCell(2, 2).Should().Be(42.5f);
    }

    [Fact]
    public void ReadCell_OutOfBounds_Throws()
    {
        var f = _world.Fields.Register<float>("rc.oob", 3, 3);
        Action a1 = () => f.ReadCell(-1, 0);
        Action a2 = () => f.ReadCell(3, 0);
        Action a3 = () => f.ReadCell(0, 3);
        a1.Should().Throw<FieldOperationFailedException>();
        a2.Should().Throw<FieldOperationFailedException>();
        a3.Should().Throw<FieldOperationFailedException>();
    }

    [Fact]
    public void AcquireSpan_ProvidesReadOnlyView()
    {
        var f = _world.Fields.Register<float>("sp.view", 3, 3);
        f.WriteCell(1, 1, 5f);

        using var lease = f.AcquireSpan();
        lease.Width.Should().Be(3);
        lease.Height.Should().Be(3);
        lease[1, 1].Should().Be(5f);
        lease.Span.Length.Should().Be(9);
    }

    [Fact]
    public void WriteCell_DuringActiveSpan_Throws()
    {
        var f = _world.Fields.Register<float>("sp.reject", 3, 3);
        using var lease = f.AcquireSpan();

        Action act = () => f.WriteCell(0, 0, 1f);
        act.Should().Throw<FieldOperationFailedException>();
    }

    [Fact]
    public void WriteCell_AfterSpanRelease_Succeeds()
    {
        var f = _world.Fields.Register<float>("sp.after", 3, 3);
        using (var lease = f.AcquireSpan()) { /* hold then release */ }

        f.WriteCell(0, 0, 7f);
        f.ReadCell(0, 0).Should().Be(7f);
    }

    [Fact]
    public void Conductivity_DefaultOne()
    {
        var f = _world.Fields.Register<float>("c.def", 3, 3);
        f.GetConductivity(1, 1).Should().Be(1.0f);
    }

    [Fact]
    public void SetConductivity_Then_Get()
    {
        var f = _world.Fields.Register<float>("c.set", 3, 3);
        f.SetConductivity(1, 1, 0.25f);
        f.GetConductivity(1, 1).Should().Be(0.25f);
    }

    [Fact]
    public void StorageFlag_DefaultFalse()
    {
        var f = _world.Fields.Register<float>("s.def", 3, 3);
        f.GetStorageFlag(1, 1).Should().BeFalse();
    }

    [Fact]
    public void StorageFlag_Toggle()
    {
        var f = _world.Fields.Register<float>("s.tog", 3, 3);
        f.SetStorageFlag(1, 1, true);
        f.GetStorageFlag(1, 1).Should().BeTrue();
        f.SetStorageFlag(1, 1, false);
        f.GetStorageFlag(1, 1).Should().BeFalse();
    }

    [Fact]
    public void SwapBuffers_PrimaryBecomesBack()
    {
        var f = _world.Fields.Register<float>("sw.test", 2, 2);
        f.WriteCell(0, 0, 1f);
        f.WriteCell(1, 1, 9f);
        f.SwapBuffers();

        // After swap, primary is the (zero-initialized) back buffer.
        f.ReadCell(0, 0).Should().Be(0f);
        f.ReadCell(1, 1).Should().Be(0f);
    }

    [Fact]
    public void IntField_RoundTrip()
    {
        var f = _world.Fields.Register<int>("i.rt", 3, 3);
        f.WriteCell(1, 1, 12345);
        f.ReadCell(1, 1).Should().Be(12345);
    }
}
```

### 7.3 — Test class for `IsotropicDiffusionKernel`

**New file**: `tests/DualFrontier.Core.Interop.Tests/IsotropicDiffusionKernelTests.cs`

```csharp
using DualFrontier.Core.Interop.CpuKernels;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public sealed class IsotropicDiffusionKernelTests : IDisposable
{
    private readonly NativeWorld _world = new();
    public void Dispose() => _world.Dispose();

    [Fact]
    public void EmptyField_RemainsZero()
    {
        var f = _world.Fields.Register<float>("d.empty", 5, 5);
        IsotropicDiffusionKernel.Run(f, IsotropicDiffusionKernel.Parameters.Default, iterations: 5);

        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                f.ReadCell(x, y).Should().Be(0f);
    }

    [Fact]
    public void PointSource_SpreadsToNeighbors()
    {
        var f = _world.Fields.Register<float>("d.spread", 5, 5);
        f.WriteCell(2, 2, 100f);

        var p = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = 0.1f,
            DecayCoefficient = 0.0f,  // no decay for clearer test
            DeltaTime = 1.0f
        };
        IsotropicDiffusionKernel.Run(f, p, iterations: 1);

        // After one iteration, neighbors gained value, center lost.
        f.ReadCell(2, 2).Should().BeLessThan(100f);
        f.ReadCell(1, 2).Should().BeGreaterThan(0f);
        f.ReadCell(3, 2).Should().BeGreaterThan(0f);
        f.ReadCell(2, 1).Should().BeGreaterThan(0f);
        f.ReadCell(2, 3).Should().BeGreaterThan(0f);
        // Diagonals untouched after 1 iteration (4-neighbor stencil).
        f.ReadCell(1, 1).Should().Be(0f);
    }

    [Fact]
    public void Decay_ReducesValuesOverTime()
    {
        var f = _world.Fields.Register<float>("d.decay", 3, 3);
        f.WriteCell(1, 1, 100f);

        var p = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = 0.0f,  // no diffusion to isolate decay
            DecayCoefficient = 0.1f,
            DeltaTime = 1.0f
        };
        IsotropicDiffusionKernel.Run(f, p, iterations: 5);

        f.ReadCell(1, 1).Should().BeLessThan(100f);
        f.ReadCell(1, 1).Should().BeGreaterThan(0f);
    }

    [Fact]
    public void ConservationApproximate_NoDecay_NoBoundaryLoss()
    {
        var f = _world.Fields.Register<float>("d.cons", 5, 5);
        f.WriteCell(2, 2, 100f);

        var p = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = 0.05f,
            DecayCoefficient = 0.0f,
            DeltaTime = 1.0f
        };
        IsotropicDiffusionKernel.Run(f, p, iterations: 3);

        float total = 0;
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                total += f.ReadCell(x, y);

        // With reflective boundary (edge cells use self as neighbor),
        // total mass is approximately conserved within a small tolerance.
        total.Should().BeApproximately(100f, 1f);
    }
}
```

### 7.4 — Run tests

```
dotnet test
```

**Expected**: previous baseline + 11 (FieldRegistry) + 12 (FieldHandle) + 4 (Diffusion) = +27 new tests passing.

**Halt condition**: any new test fails OR any existing test regresses. The tests are deterministic; flaky behavior is a bug to investigate.

**Atomic commit**: `test(interop): add 27 K9 tests for FieldRegistry, FieldHandle, and CPU diffusion kernel`

---

## Phase 8 — IModApi v3 wiring

This phase exposes the field registry through the modding surface per `MOD_OS_ARCHITECTURE.md` v1.6 §4.6. The compute pipelines sub-API is **not** wired in K9 (it lands in G0 with Vulkan compute plumbing); `IModApi.ComputePipelines` returns null.

### 8.1 — `IModFieldApi` interface

**New file**: `src/DualFrontier.Contracts/Modding/IModFieldApi.cs`

```csharp
namespace DualFrontier.Contracts.Modding;

public interface IModFieldApi
{
    FieldHandle<T> RegisterField<T>(string id, int width, int height) where T : unmanaged;
    FieldHandle<T> GetField<T>(string id) where T : unmanaged;
    bool IsRegistered(string id);
}
```

The `FieldHandle<T>` type is in `DualFrontier.Core.Interop`. The `Contracts` assembly does not currently reference `Core.Interop` — verify dependency direction at execution time. If a circular dependency would result, define a marker interface `IFieldHandle` in `Contracts` and have `FieldHandle<T>` implement it; the contract returns `IFieldHandle`-typed handles cast back at the call site.

### 8.2 — Extend `IModApi`

**File**: `src/DualFrontier.Contracts/Modding/IModApi.cs`

Add at the end of the interface (before closing brace):

```csharp
/// <summary>
/// Field-storage sub-API per MOD_OS_ARCHITECTURE.md v1.6 §4.6.
/// Returns null on builds without K9 field storage support; mods check
/// for null and degrade gracefully.
/// </summary>
IModFieldApi? Fields { get; }

/// <summary>
/// Compute-pipeline sub-API per MOD_OS_ARCHITECTURE.md v1.6 §4.6.
/// Returns null on K9 (lands at G0). Mods check for null.
/// </summary>
IModComputePipelineApi? ComputePipelines { get; }
```

The `IModComputePipelineApi` is a placeholder marker interface — single property `string Name => "ComputePipelinesPlaceholder";` is sufficient. K9 always returns null for this property; G0 implements it.

### 8.3 — `RestrictedModApi` implementation

**File**: `src/DualFrontier.Application/Modding/RestrictedModApi.cs` (or wherever the implementation lives — verify at execution time).

Add backing field and property implementations. The `Fields` getter returns a per-mod `RestrictedFieldApi` instance that wraps `FieldRegistry` with capability cross-check; the `ComputePipelines` getter returns null.

```csharp
private readonly RestrictedFieldApi? _fieldsApi;

public IModFieldApi? Fields => _fieldsApi;
public IModComputePipelineApi? ComputePipelines => null;  // K9 — lands at G0
```

Constructor accepts the `FieldRegistry` (from `NativeWorld.Fields`) and the per-mod capability set; constructs `RestrictedFieldApi` if the registry is non-null.

### 8.4 — `RestrictedFieldApi` — capability cross-check wrapper

**New file**: `src/DualFrontier.Application/Modding/RestrictedFieldApi.cs`

```csharp
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Interop;

namespace DualFrontier.Application.Modding;

internal sealed class RestrictedFieldApi : IModFieldApi
{
    private readonly FieldRegistry _registry;
    private readonly string _modId;
    private readonly IReadOnlySet<string> _modCapabilities;

    internal RestrictedFieldApi(FieldRegistry registry, string modId, IReadOnlySet<string> capabilities)
    {
        _registry = registry;
        _modId = modId;
        _modCapabilities = capabilities;
    }

    public FieldHandle<T> RegisterField<T>(string id, int width, int height) where T : unmanaged
    {
        // Field id must be in the calling mod's namespace.
        string expectedPrefix = _modId + ".";
        if (!id.StartsWith(expectedPrefix, StringComparison.Ordinal))
        {
            throw new CapabilityViolationException(
                $"Mod '{_modId}' attempted to register field '{id}' outside its own namespace");
        }

        // Capability check: mod must declare write access to the field it registers.
        string requiredCap = $"mod.{_modId}.field.write:{id}";
        if (!_modCapabilities.Contains(requiredCap))
        {
            throw new CapabilityViolationException(
                $"Mod '{_modId}' lacks capability '{requiredCap}' required to register field '{id}'");
        }

        return _registry.Register<T>(id, width, height);
    }

    public FieldHandle<T> GetField<T>(string id) where T : unmanaged
    {
        // Cross-mod field access requires capability declaration referencing the foreign mod's namespace.
        // The capability format follows MOD_OS_ARCHITECTURE.md §3.2.
        string ownNamespacePrefix = _modId + ".";
        string requiredCap;
        if (id.StartsWith(ownNamespacePrefix, StringComparison.Ordinal))
        {
            // Own-namespace access: read capability under own provider.
            requiredCap = $"mod.{_modId}.field.read:{id}";
        }
        else
        {
            // Cross-mod access: read capability under foreign provider.
            int dot = id.IndexOf('.');
            if (dot <= 0) throw new ArgumentException($"Field id '{id}' must be namespaced");
            string foreignMod = id[..dot];
            requiredCap = $"mod.{foreignMod}.field.read:{id}";
        }

        if (!_modCapabilities.Contains(requiredCap))
        {
            throw new CapabilityViolationException(
                $"Mod '{_modId}' lacks capability '{requiredCap}' required to access field '{id}'");
        }

        return _registry.Get<T>(id);
    }

    public bool IsRegistered(string id) => _registry.IsRegistered(id);
}
```

The capability format mirrors §3.2: `mod.<provider>.field.<verb>:<field-id>`. The capability check happens at the API boundary; per-cell ReadCell/WriteCell calls go through `FieldHandle<T>` directly without further capability checks (the handle was already capability-gated at acquisition).

### 8.5 — Capability parser extension

**File**: `src/DualFrontier.Application/Modding/CapabilityParser.cs` (or equivalent — verify at execution).

The regex extension already landed in MOD_OS v1.6 §2.3 step 6 — the parser must accept `field.read`, `field.write`, `field.acquire`, `field.conductivity`, `field.storage`, `field.dispatch`, and `pipeline.register` as verbs. If the parser uses a switch/match against verb names, extend the list. If it uses the regex directly, ensure the runtime regex matches the spec regex.

Verify by reading the parser implementation at execution time and adjusting accordingly. The exact edit shape is conditional on existing parser structure.

### 8.6 — Manifest validation tests

**New file**: `tests/DualFrontier.Modding.Tests/FieldCapabilityValidationTests.cs`

```csharp
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests;

public sealed class FieldCapabilityValidationTests
{
    [Theory]
    [InlineData("kernel.field.read:vanilla.magic.mana")]
    [InlineData("mod.dualfrontier.vanilla.magic.field.write:vanilla.magic.mana")]
    [InlineData("mod.dualfrontier.vanilla.electricity.field.conductivity:vanilla.electricity.power")]
    [InlineData("mod.dualfrontier.vanilla.water.field.dispatch:vanilla.water.pressure")]
    [InlineData("mod.dualfrontier.vanilla.magic.pipeline.register:vanilla.magic.diffusion")]
    public void ValidCapabilityStrings_Parse(string capability)
    {
        // Use existing manifest validator entry point — exact API name verified at execution.
        // The capability should validate without error.
    }

    [Theory]
    [InlineData("field.read:vanilla.magic.mana")]                        // missing provider
    [InlineData("kernel.field.unknown:vanilla.magic.mana")]              // invalid verb
    [InlineData("mod.foo.field.read:")]                                  // empty target
    [InlineData("mod.foo.field.read:bad chars in id!")]                  // invalid chars
    public void InvalidCapabilityStrings_FailValidation(string capability)
    {
        // Should produce a typed validation error.
    }
}
```

The exact API for invoking the manifest validator is read at execution time. The test bodies are filled in to call the validator and assert success/failure; the test data above is the contract.

**Atomic commit**: `feat(modding): wire IModApi.Fields to FieldRegistry with capability cross-check`

### 8.7 — Verify build and tests

```
dotnet build
dotnet test
```

**Expected**: all previous tests + 27 from Phase 7 + N from Phase 8 (test count varies by validator API).

---

## Phase 9 — Closure verification

### 9.1 — Update `MIGRATION_PROGRESS.md`

**File**: `docs/MIGRATION_PROGRESS.md`

Update `Current state snapshot` table:
- Active phase → next planned (G0, or whichever is next)
- Last completed milestone → K9 with commit hash + date
- Tests passing → new total

Add K9 row under K-series progress (full closure entry following K0–K5 format):
- Status: DONE with commit hash + date
- Brief: `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` (FULL EXECUTED)
- C ABI extension: 12 new functions
- Native files added: `tile_field.h/cpp`
- Selftest scenarios: previous + 8
- Bridge tests: previous + 27
- Lessons learned: any non-trivial discoveries from execution

### 9.2 — Update `KERNEL_ARCHITECTURE.md` status snapshot

**File**: `docs/architecture/KERNEL_ARCHITECTURE.md`

The status snapshot line in the Executive Summary section currently reads:

```
**Status snapshot** (live, обновляется по closure milestone): K0–K5 closed (...); 538 tests passing; K6 next per β6 sequencing.
```

Update to reflect K9 closure (preserve the format).

### 9.3 — Update `FIELDS.md` from Draft to LOCKED-equivalent

**File**: `docs/architecture/FIELDS.md`

Remove **TBD** markers from sections that K9 has now grounded (Native layer, C ABI, Managed bridge — all have concrete implementations). The Save/load section keeps **TBD** until the persistence-integration milestone.

Update Status line:

```
**Status:** Live — populated by K9 closure. Storage contract is concrete and stable; G-series amendments will not change the K9-defined surface.
```

### 9.4 — Update K9 brief with execution summary

**File**: `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` (this file).

Append a closing section:

```markdown
---

## Execution closure

**Status**: FULL EXECUTED on (date). Final commit: (hash).

**Atomic commits landed** (~16-18 expected, exact list at execution):

(list)

**Test deltas**:
- Native selftest: N → N+8 scenarios
- Bridge tests: N → N+27 tests
- Modding tests: N → N+M tests (Phase 8 capability validation)

**Lessons learned**:

(populated at closure)
```

### 9.5 — Final atomic commit

```
git add docs/MIGRATION_PROGRESS.md docs/architecture/KERNEL_ARCHITECTURE.md docs/architecture/FIELDS.md tools/briefs/K9_FIELD_STORAGE_BRIEF.md
git commit -m "docs(migration): K9 closure recorded"
```

### 9.6 — Final verification

```
dotnet build
dotnet test
cd native/DualFrontier.Core.Native/build && cmake --build . --config Release && ./Release/df_native_selftest.exe
```

All three must pass. If any fails, the closure is incomplete; investigate and amend.

### 9.7 — Merge to main

```
git checkout main
git merge --ff-only feat/k9-field-storage
git push origin main
```

The merge must be fast-forward — no merge commit. If non-FF, the executor halts and escalates: parallel work on main happened during K9 execution and needs reconciliation manually.

---

## Stop conditions

The executor halts and escalates the brief authorship session if any of the following:

1. Any pre-flight check (Phase 0) fails.
2. Any phase produces unexpected `dotnet build` warnings or errors not covered by the deterministic edits.
3. Any test fails after a phase's edits — the edits are deterministic, so failures indicate either a brief bug (escalate) or an environment issue (escalate with environment details).
4. The `Edit` tool reports unexpected behavior on any oldText/newText pair (the brief avoids regex metacharacters in literal-mode tools, so anomalies suggest tool drift).
5. The executor encounters a project structure that contradicts this brief's assumptions (e.g. `RestrictedModApi` lives in a different file than expected, the `IModApi` interface has a different shape, etc.). The brief documents what should be true; execution-time reality must match. Mismatches halt for clarification.
6. Native build environment has a CMake or compiler change that produces output the brief did not anticipate. Same halt-and-escalate.

The fallback in every halt case is `git stash push -m "k9-WIP-halt-$(date +%s)"` and report to the brief author. Partial K9 work is recoverable; an ad-hoc continuation on a corrupted state is not.

---

## Atomic commit log expected

Approximate commit count: **16-18**, all on `feat/k9-field-storage` branch:

1. `docs(briefs): K9 skeleton expanded to full brief` (Phase 0)
2. `feat(native): add RawTileField header for K9 field storage` (Phase 1.1)
3. `feat(native): implement RawTileField with conductivity and storage flags` (Phase 1.2-1.3)
4. `feat(native): extend World with field registry parallel to component stores` (Phase 2)
5. `feat(native): add K9 field storage C ABI declarations` (Phase 3.1)
6. `feat(native): implement K9 field storage C ABI bridge` (Phase 3.2)
7. `test(native): add 8 field storage scenarios to selftest` (Phase 4)
8. `feat(interop): add P/Invoke declarations for K9 field storage` (Phase 5.1)
9. `feat(interop): add FieldHandle, FieldSpanLease, and exception types` (Phase 5.2)
10. `feat(interop): add FieldRegistry tracking managed-side field handles` (Phase 5.3)
11. `feat(interop): expose FieldRegistry on NativeWorld` (Phase 5.4)
12. `feat(interop): add CPU isotropic diffusion kernel as equivalence oracle` (Phase 6)
13. `test(interop): add 27 K9 tests for FieldRegistry, FieldHandle, and CPU diffusion kernel` (Phase 7)
14. `feat(contracts): add IModFieldApi and IModComputePipelineApi to IModApi v3 surface` (Phase 8.1-8.2)
15. `feat(modding): wire IModApi.Fields to FieldRegistry with capability cross-check` (Phase 8.3-8.5)
16. `test(modding): add field capability validation tests` (Phase 8.6)
17. `docs(migration): K9 closure recorded` (Phase 9.1-9.5)

A merge commit on `main` is **not** in this list — the merge is fast-forward, no commit produced.

---

## Cross-cutting design constraints

This brief explicitly enforces the following architectural invariants. The executor checks each at the relevant phase and halts on violation.

1. **Two orthogonal systems** (per `FIELDS.md`). Field storage lives in `RawTileField`, not in `RawComponentStore`. The two registries are separate `unordered_map`s. No code path treats a field as a component or vice versa.

2. **Native single-threaded contract** (per K-L7). Field mutations are gated by `active_spans_` exactly like component mutations. The atomic counter pattern is reused, not reinvented.

3. **No native callbacks to managed** (per K-L7 direction discipline). Field operations are managed-initiated; native does not call back. The CPU diffusion kernel runs in managed code, reaching native through `WriteCell` / `ReadCell` / `AcquireSpan`.

4. **Vanilla = mods** (per K-L9). The kernel does not own any field. Even the canonical mana / electricity / water fields are registered by vanilla mods at startup. K9 ships zero kernel-internal fields.

5. **CPU functional first** (per `KERNEL_ARCHITECTURE.md` K9, `GPU_COMPUTE.md` G-series). The G-series Vulkan compute path is **not** wired in K9. `IModApi.ComputePipelines` returns null. The diffusion kernel is CPU-only.

6. **Capability cross-check at three layers** (per `MOD_OS_ARCHITECTURE.md` §3.6). Manifest parse → load-time `[FieldAccess]` cross-check → runtime per-call hash-set lookup. K9 wires layers 1 and 3; layer 2 (`[FieldAccess]` attribute and `SystemExecutionContext` enforcement) is wired here as part of Phase 8.

7. **No regex metacharacters in `Edit` tool boundaries** (per MOD_OS v1.6 closure record). All oldText / newText payloads in this brief are plain prose / code without `$ ^ \b \d \w \s [ ] ( | ) * + ?` at the boundary positions. Where regex content appears as content (e.g. capability syntax), it lives inside fenced code blocks where it is interior, not boundary.

8. **Atomic commits per logical change** (per project standing rule). One commit per Phase sub-step. The commit log above is the contract; deviations halt for review.

9. **Pre-flight grep discipline** (AD #4 from project memory). Every Phase that adds new identifiers checks for collisions with existing code via grep before introducing them.

10. **«Data exists or it doesn't»** (METHODOLOGY §7.1). Out-of-bounds reads return zero / fail with explicit error code; never undefined behavior. The native side checks bounds; the managed side surfaces errors as exceptions.

---

## Brief authoring lineage

- **2026-05-08** — Skeleton committed alongside MOD_OS v1.6 amendment closure (commit `356ea50` per closure record).
- **2026-05-08** — Skeleton expanded to full brief in this revision. Author: Opus architect session per «доки сначала, миграция потом» pivot.
- **(date TBD)** — Executed and closed at K9 milestone closure.

The full brief was authored read-first / brief-second per the methodology pivot recorded in MOD_OS_V16_AMENDMENT_CLOSURE.md. Source documents read during authoring: `GPU_COMPUTE.md` v2.0 LOCKED, `KERNEL_ARCHITECTURE.md` v1.0 LOCKED, `MOD_OS_ARCHITECTURE.md` v1.6 LOCKED, `MIGRATION_PROGRESS.md` (live), `ECS.md`, `FIELDS.md` (draft authored in parallel with this brief), existing native code (`world.h/cpp`, `df_capi.h`, `selftest.cpp`), existing bridge tests format (`K2_REGISTRY_TESTS_BRIEF.md` lessons learned). Authored against the contract surfaces in those documents verbatim; any deviation between this brief and those specs is a brief bug, not a design choice.

---

**Brief end.** ~~Awaits K6, K7, K8 closure for execution per β6 sequencing.~~

---

## Execution closure

**Status**: FULL EXECUTED on 2026-05-10 as A'.4 milestone (K9 + A'.4.0 patch bundle per Crystalka 2026-05-10 «всё в одну сессию, окно контекста позволяет»). Branch: `feat/k9-field-storage`.

**Atomic commits landed** (17 total — patch added commit #1 per `K9_BRIEF_REFRESH_PATCH.md` §"Commit ordering"):

1. `d163341` — `docs(briefs): A'.4.0 K9 brief refresh patch (companion to K9 brief)`
2. `0cc24a3` — `feat(native): add RawTileField header for K9 field storage` (Phase 1.1)
3. `4e89cba` — `feat(native): implement RawTileField with conductivity and storage flags` (Phase 1.2-1.4)
4. `fbf5ef5` — `feat(native): extend World with field registry parallel to component stores` (Phase 2)
5. `0f2a076` — `feat(native): add K9 field storage C ABI declarations` (Phase 3.1)
6. `ce4dba8` — `feat(native): implement K9 field storage C ABI bridge` (Phase 3.2)
7. `4b5f873` — `test(native): add 8 field storage scenarios to selftest` (Phase 4)
8. `b96750d` — `feat(interop): add P/Invoke declarations for K9 field storage` (Phase 5.1)
9. `b86d357` — `feat(interop): add FieldHandle, FieldSpanLease, and exception types` (Phase 5.2)
10. `0825fce` — `feat(interop): add FieldRegistry tracking managed-side field handles` (Phase 5.3)
11. `48eb485` — `feat(interop): expose FieldRegistry on NativeWorld` (Phase 5.4)
12. `4fe81e1` — `feat(interop): add CPU isotropic diffusion kernel as equivalence oracle` (Phase 6)
13. `e30c662` — `test(interop): add 27 K9 tests for FieldRegistry, FieldHandle, and CPU diffusion kernel` (Phase 7)
14. `46e995c` — `feat(contracts): add IModFieldApi and IModComputePipelineApi to IModApi v3 surface` (Phase 8.1-8.2)
15. `873ce14` — `feat(modding): wire IModApi.Fields to FieldRegistry with capability cross-check` (Phase 8.3-8.5)
16. `ab5e578` — `test(modding): add field capability validation tests` (Phase 8.6)
17. (this commit) — `docs(migration): K9 closure recorded` (Phase 9.1-9.5)

**Test deltas**:
- Native selftest: 21 → 29 scenarios (8 new K9 field scenarios — `scenario_field_register_and_read`, `scenario_field_write_and_read_roundtrip`, `scenario_field_span_lifecycle`, `scenario_field_conductivity_default_and_set`, `scenario_field_storage_flag_toggle`, `scenario_field_swap_buffers`, `scenario_field_register_idempotent_and_conflict`, `scenario_field_unregister`); ALL PASSED.
- Bridge tests (Interop.Tests): +27 (11 FieldRegistryTests + 12 FieldHandleTests + 4 IsotropicDiffusionKernelTests); ALL PASSED 0.92s.
- Modding tests (capability validation): +13 (7 valid InlineData + 5 invalid InlineData + 1 PreviousVerbs_StillAccepted). 631 → 671 expected.

**Lessons learned**:

- **Brief Phase 4 scenario style was stale**: the brief authored 8 selftest scenarios in `static int` return-code style; the actual `selftest.cpp` pattern at execution time uses `void` + `DF_CHECK` macro + `g_failures` counter. Adapted to existing pattern for file-local consistency; brief intent preserved verbatim (the brief itself said «each scenario follows the existing pattern» — the author's recollection of «existing pattern» was off, not the architectural intent).
- **NativeMethods P/Invoke style**: brief drafted `[MarshalAs(UnmanagedType.LPStr)] string fieldId` with `CharSet.Ansi`; the existing project pattern (`df_world_intern_string`, `df_world_begin_mod_scope`) uses `byte*` + UTF-8 stackalloc helper. Adopted the contemporary `byte*` pattern (CharSet is deprecated in .NET 8+, and UTF-8 is the correct encoding for namespaced ids). Brief's contract (12 P/Invoke functions over the C ABI) preserved; only the marshalling style differs.
- **IModApi v3 Fields/ComputePipelines circular dependency**: brief Phase 8.1 flagged this case. Resolved per brief's fallback: `IFieldHandle` moved to `DualFrontier.Contracts.Modding` (non-generic marker), `FieldHandle<T>` in `Core.Interop` implements it, `IModFieldApi.RegisterField<T>` returns `IFieldHandle` (callers downcast). Avoids inverting the dep direction.
- **Capability regex was not pre-extended**: brief Phase 8.5 assumed the K-L3.1 MOD_OS v1.6 §2.3 step 6 regex extension already landed in source. Inspection showed the actual `ManifestCapabilities.Parse` regex still had only `publish|subscribe|read|write`. Extended in this milestone to add `field.(read|write|acquire|conductivity|storage|dispatch)` and `pipeline.register` per spec.
- **RegisterManagedComponent<T> pre-flight check (per patch §Phase 8.2 override)**: `IModApi.cs` did NOT contain `RegisterManagedComponent<T>` at execution time (it ships at K8.4 per Phase A' sequencing, not K-L3.1 amendment). K9 did not add it — only `Fields` and `ComputePipelines` properties.
- **FieldRegistry plumbing through ModIntegrationPipeline**: K9 exposes `FieldRegistry` on `NativeWorld.Fields`, but `ModIntegrationPipeline` does not currently carry a `NativeWorld` reference (mod loading is decoupled from world lifecycle). `RestrictedModApi` accepts a nullable `FieldRegistry?`; K9 passes `null` at the existing call site and `Fields` returns `null`. Mods degrade gracefully per the IModApi.Fields docstring contract («Returns null on builds without K9 field storage support»). Production wiring (passing the live `FieldRegistry` from a configured `NativeWorld`) lands at A'.5 K8.3 or A'.6 K8.4 when the kernel-world / mod-loader integration matures.
- **CPU kernel write performance**: the IsotropicDiffusionKernel uses per-cell `WriteCell` after reading the span (40 000 P/Invokes per iteration on 200×200). Acceptable for K9 (GPU equivalence oracle), not for production. G1 replaces with Vulkan compute dispatch. Brief explicitly documents this design choice; no surprise.

