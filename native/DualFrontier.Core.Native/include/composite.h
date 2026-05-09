#pragma once

#include <cstdint>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

// Variable-length data attached to a parent entity. Each parent's data is
// stored in a separate byte vector keyed by the entity's packed uint64_t.
//
// Mirrors the K8.1 design contract recorded in
// tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md §1.4.
//
// Element size is fixed at construction. Operations:
//   * add: append element to parent's vector (amortised O(1) via
//     std::vector::resize + memcpy growth).
//   * get_at / iterate: O(1) random access / O(count) linear over parent.
//   * remove_at: swap-with-last + pop. O(1) but does NOT preserve order.
//   * clear_for: drops the parent's vector entirely.
//
// Insertion order is preserved for non-removed elements. After a
// remove_at(i), the element at index i is replaced with what was at the
// end. This is the swap-with-last pattern; appropriate for cases where
// order doesn't matter (movement waypoints consumed front-to-back,
// storage item lists iterated without a stable-order requirement).
class Composite {
public:
    explicit Composite(int32_t element_size);

    int32_t add(uint64_t parent, const void* element);
    [[nodiscard]] int32_t get_count(uint64_t parent) const noexcept;
    int32_t get_at(uint64_t parent, int32_t index, void* out_element) const noexcept;
    int32_t remove_at(uint64_t parent, int32_t index);
    int32_t clear_for(uint64_t parent);
    int32_t iterate(uint64_t parent, void* out_buffer,
                    int32_t buffer_capacity) const noexcept;

    [[nodiscard]] int32_t element_size() const noexcept { return element_size_; }

private:
    int32_t element_size_;
    std::unordered_map<uint64_t, std::vector<uint8_t>> data_per_parent_;
};

} // namespace dualfrontier
