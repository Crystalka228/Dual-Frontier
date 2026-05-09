#pragma once

#include <cstdint>
#include <vector>

namespace dualfrontier {

// Type-erased set with sorted-by-element iteration.
//
// Mirrors the K8.1 design contract recorded in
// tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md §1.5.
//
// SetPrimitive is the value-less mirror of KeyedMap. Sorting is byte-wise
// (memcmp on element_size bytes) for deterministic save/load and cross-mod
// iteration ordering.
//
// Operations:
//   * add: O(log n) lookup + O(n) memmove on insert. Returns 1 if newly
//     inserted, 0 if already present.
//   * contains: O(log n).
//   * remove: O(log n) + O(n) memmove.
//   * iterate: linear scan; encounters elements in sorted (memcmp) order.
//
// Use case (per K8.2 plan): replaces HashSet<EntityId> reservation
// tracking in StorageComponent with the same membership semantics but
// deterministic iteration order.
class SetPrimitive {
public:
    explicit SetPrimitive(int32_t element_size);

    int32_t add(const void* element);
    [[nodiscard]] int32_t contains(const void* element) const noexcept;
    int32_t remove(const void* element);

    [[nodiscard]] int32_t count() const noexcept { return count_; }
    [[nodiscard]] int32_t element_size() const noexcept { return element_size_; }

    int32_t iterate(void* out_buffer, int32_t buffer_capacity) const noexcept;

private:
    int32_t element_size_;
    int32_t count_;
    std::vector<uint8_t> elements_;

    int32_t binary_search(const void* element) const noexcept;
};

} // namespace dualfrontier
