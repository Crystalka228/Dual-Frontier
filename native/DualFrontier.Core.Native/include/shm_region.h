#pragma once

#include <cstdint>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

// K10.1 Item 9 — Shared memory regions (К-L14 architectural completeness;
// real OS provides this — Linux shmget/shmat, mmap shared, perf_event ring
// buffers).
//
// Use case: high-frequency cross-system data flow that doesn't fit ECS
// component model (positions, velocities, animation state). Bus events
// serialize; SHM regions are raw memory access.
//
// K10.1 implementation: single-process, just a buffer per region. Multi-
// process shared memory deferred until cross-process scope arises (post-К-series).
//
// Single-writer / multi-reader pattern enforced (writer system declared via
// register_writer). Lock-free reads: callers responsible for atomic access
// semantics on хот data structures within the region.
//
// Per К-L18 quiescent state (К10.3 lock): region destruction requires
// quiescent state в К10.2 mod lifecycle; К10.1 lands creation/map/destroy
// primitives only.
class ShmRegistry {
public:
    ShmRegistry();
    ~ShmRegistry();

    ShmRegistry(const ShmRegistry&) = delete;
    ShmRegistry& operator=(const ShmRegistry&) = delete;

    // Create a region of the given size. Returns true on success, false if
    // already exists (use destroy first к replace) or size_bytes <= 0.
    bool create(uint32_t region_id, int32_t size_bytes);

    // Get a writable pointer к the region data. Returns nullptr if not
    // created. Caller responsible for synchronization (single-writer pattern
    // enforced through register_writer + read-only access from non-writers).
    void* map(uint32_t region_id);

    // Get the size of the region (bytes). Returns 0 if not created.
    [[nodiscard]] int32_t size(uint32_t region_id) const noexcept;

    // Unmap is a no-op в the single-process implementation. Returns true if
    // the region exists.
    bool unmap(uint32_t region_id);

    // Destroy the region. Returns true if removed, false if not found.
    bool destroy(uint32_t region_id);

    // Declare the writer system. Returns true on success, false if region not
    // created. Multiple register_writer calls overwrite the writer; the
    // intent is one-writer-at-a-time architectural convention, not enforced
    // panic в К10.1.
    bool register_writer(uint32_t region_id, uint32_t writer_system_id);

    // Get the registered writer system id. Returns 0 if unset or region absent.
    [[nodiscard]] uint32_t writer(uint32_t region_id) const noexcept;

    [[nodiscard]] int32_t region_count() const noexcept {
        return static_cast<int32_t>(regions_.size());
    }

    void clear() noexcept;

private:
    struct Region {
        std::vector<uint8_t> data;
        uint32_t writer_system_id = 0;
    };

    std::unordered_map<uint32_t, Region> regions_;
};

ShmRegistry& default_shm_registry();

} // namespace dualfrontier
