#pragma once

#include <cstdint>
#include <cstring>
#include <memory>
#include <stdexcept>
#include <vector>

#include "sparse_set.h"

namespace dualfrontier {

// Type-erased component store used by the C ABI.
//
// Storage is a SparseSet<RawSlot> where RawSlot wraps `size` bytes of opaque
// component data. The component type id (uint32_t) is assigned by the C#
// caller; the native side only tracks the byte size declared on first Add
// for that type.
//
// For the PoC we assume blittable structs on the C# side. Reference-type
// components (class-based IComponent) are not supported through this store —
// they require GCHandle marshalling and stay on the managed side. See
// docs/NATIVE_CORE.md for the rationale.
class RawComponentStore {
public:
    explicit RawComponentStore(int32_t component_size)
        : component_size_(component_size) {
        if (component_size <= 0) {
            throw std::invalid_argument(
                "RawComponentStore: component_size must be > 0");
        }
    }

    [[nodiscard]] int32_t component_size() const noexcept {
        return component_size_;
    }

    void add(int32_t entity_index, const void* data, int32_t size) {
        if (size != component_size_) {
            throw std::invalid_argument(
                "RawComponentStore: size mismatch on add");
        }
        if (entity_index <= 0) {
            throw std::invalid_argument(
                "RawComponentStore: entity_index must be > 0");
        }

        ensure_sparse_capacity(entity_index);

        int32_t dense_index = sparse_[entity_index];
        if (dense_index != SparseSet<int>::kAbsent) {
            std::memcpy(slot_ptr(dense_index), data,
                        static_cast<std::size_t>(size));
            return;
        }

        const int32_t new_dense_index = count_;
        const std::size_t required_bytes =
            static_cast<std::size_t>((new_dense_index + 1) * component_size_);
        if (required_bytes > dense_bytes_.size()) {
            const std::size_t new_byte_capacity =
                std::max<std::size_t>(required_bytes, dense_bytes_.size() * 2);
            dense_bytes_.resize(new_byte_capacity);
        }
        if (static_cast<std::size_t>(new_dense_index + 1) >
            dense_to_index_.size()) {
            dense_to_index_.resize(dense_to_index_.empty()
                                       ? 16
                                       : dense_to_index_.size() * 2);
        }

        std::memcpy(slot_ptr(new_dense_index), data,
                    static_cast<std::size_t>(size));
        dense_to_index_[new_dense_index] = entity_index;
        sparse_[entity_index] = new_dense_index;
        ++count_;
    }

    [[nodiscard]] bool has(int32_t entity_index) const noexcept {
        if (entity_index <= 0 ||
            entity_index >= static_cast<int32_t>(sparse_.size())) {
            return false;
        }
        return sparse_[entity_index] != SparseSet<int>::kAbsent;
    }

    bool get(int32_t entity_index, void* out_data, int32_t size) const noexcept {
        if (size != component_size_) return false;
        if (!has(entity_index)) return false;
        const int32_t dense_index = sparse_[entity_index];
        std::memcpy(out_data, slot_ptr(dense_index),
                    static_cast<std::size_t>(size));
        return true;
    }

    void remove(int32_t entity_index) noexcept {
        if (entity_index <= 0 ||
            entity_index >= static_cast<int32_t>(sparse_.size())) {
            return;
        }
        const int32_t dense_index_to_remove = sparse_[entity_index];
        if (dense_index_to_remove == SparseSet<int>::kAbsent) return;

        const int32_t last_dense_index = count_ - 1;
        if (dense_index_to_remove != last_dense_index) {
            const int32_t last_entity_index = dense_to_index_[last_dense_index];
            std::memcpy(slot_ptr(dense_index_to_remove),
                        slot_ptr(last_dense_index),
                        static_cast<std::size_t>(component_size_));
            dense_to_index_[dense_index_to_remove] = last_entity_index;
            sparse_[last_entity_index] = dense_index_to_remove;
        }
        sparse_[entity_index] = SparseSet<int>::kAbsent;
        --count_;
    }

    [[nodiscard]] int32_t count() const noexcept { return count_; }

    [[nodiscard]] const std::vector<int32_t>& dense_indices() const noexcept {
        return dense_to_index_;
    }

private:
    uint8_t* slot_ptr(int32_t dense_index) noexcept {
        return dense_bytes_.data() +
               static_cast<std::size_t>(dense_index) *
                   static_cast<std::size_t>(component_size_);
    }
    const uint8_t* slot_ptr(int32_t dense_index) const noexcept {
        return dense_bytes_.data() +
               static_cast<std::size_t>(dense_index) *
                   static_cast<std::size_t>(component_size_);
    }

    void ensure_sparse_capacity(int32_t entity_index) {
        if (entity_index < static_cast<int32_t>(sparse_.size())) return;
        const std::size_t new_size =
            std::max<std::size_t>(
                static_cast<std::size_t>(entity_index) + 1,
                sparse_.empty() ? 256 : sparse_.size() * 2);
        sparse_.resize(new_size, SparseSet<int>::kAbsent);
    }

    int32_t component_size_;
    int32_t count_ = 0;
    std::vector<int32_t> sparse_;
    std::vector<int32_t> dense_to_index_;
    std::vector<uint8_t> dense_bytes_;
};

} // namespace dualfrontier
