#pragma once

#include <cstdint>
#include <vector>

namespace dualfrontier {

// Type-erased keyed map with sorted-by-key iteration.
//
// Mirrors the K8.1 design contract recorded in
// tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md §1.3.
//
// Sorting is byte-wise (memcmp on key_size bytes). Works deterministically
// for blittable POD key types (uint32_t, EntityId, enum types). Composite
// key structs work as long as endianness is consistent — true within a
// single machine, which is the only assumption K8.1 needs to hold.
//
// Operations:
//   * set(key, value): O(log n) lookup + O(n) memmove on insert. Returns
//     1 if newly inserted, 0 if the key was already present (value updated).
//   * get(key, out_value): O(log n).
//   * remove(key): O(log n) + O(n) memmove.
//   * iterate: linear scan; encounters keys in sorted (memcmp) order.
//
// Determinism guarantee: save/load roundtrip writes entries in iteration
// order; reload reads them; result identical regardless of insertion
// sequence. Cross-mod systems iterating each other's maps see the same
// order on every machine.
class KeyedMap {
public:
    KeyedMap(int32_t key_size, int32_t value_size);

    int32_t set(const void* key, const void* value);
    int32_t get(const void* key, void* out_value) const noexcept;
    int32_t remove(const void* key);

    [[nodiscard]] int32_t count() const noexcept { return count_; }
    [[nodiscard]] int32_t key_size() const noexcept { return key_size_; }
    [[nodiscard]] int32_t value_size() const noexcept { return value_size_; }

    // Writes up to buffer_capacity entries into out_keys / out_values. Both
    // buffers must be sized for at least buffer_capacity * key_size_ /
    // value_size_ bytes. Returns count actually written (clipped).
    int32_t iterate(void* out_keys, void* out_values,
                    int32_t buffer_capacity) const noexcept;

    int32_t clear() noexcept;

private:
    int32_t key_size_;
    int32_t value_size_;
    int32_t count_;
    std::vector<uint8_t> keys_;
    std::vector<uint8_t> values_;

    // Returns the dense index where `key` is stored, or -(insert_pos + 1)
    // if absent (negative encoding mirrors std::lower_bound usage but keeps
    // a single int32_t return).
    int32_t binary_search(const void* key) const noexcept;
};

} // namespace dualfrontier
