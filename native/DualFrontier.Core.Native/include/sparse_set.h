#pragma once

#include <cstddef>
#include <cstdint>
#include <cstring>
#include <stdexcept>
#include <vector>

namespace dualfrontier {

// Header-only SparseSet template.
//
// Mirrors the algorithm in src/DualFrontier.Core/ECS/ComponentStore.cs:
//   * _sparse[entity_index] -> dense slot, or -1 if absent.
//   * _dense packs live values contiguously for cache-friendly iteration.
//   * _dense_to_index maps dense slot back to entity index, needed for the
//     swap-with-last erase.
//
// Intentionally trivial: no allocator customisation, no exception safety
// beyond strong guarantee on std::vector growth, no thread safety — the
// calling layer (World) serialises access. This keeps the hot path aligned
// with the managed version being benchmarked against.
template <typename T>
class SparseSet {
public:
    static constexpr int32_t kAbsent = -1;

    SparseSet() = default;

    void add(int32_t entity_index, const T& value) {
        if (entity_index <= 0) {
            throw std::invalid_argument("SparseSet: entity_index must be > 0");
        }
        ensure_sparse_capacity(entity_index);

        int32_t dense_index = sparse_[entity_index];
        if (dense_index != kAbsent) {
            dense_[dense_index] = value;
            return;
        }

        const int32_t new_dense_index = static_cast<int32_t>(dense_.size());
        dense_.push_back(value);
        dense_to_index_.push_back(entity_index);
        sparse_[entity_index] = new_dense_index;
    }

    void remove(int32_t entity_index) noexcept {
        if (entity_index <= 0 ||
            entity_index >= static_cast<int32_t>(sparse_.size())) {
            return;
        }
        const int32_t dense_index_to_remove = sparse_[entity_index];
        if (dense_index_to_remove == kAbsent) {
            return;
        }

        const int32_t last_dense_index =
            static_cast<int32_t>(dense_.size()) - 1;

        if (dense_index_to_remove != last_dense_index) {
            const int32_t last_entity_index = dense_to_index_[last_dense_index];
            dense_[dense_index_to_remove] = dense_[last_dense_index];
            dense_to_index_[dense_index_to_remove] = last_entity_index;
            sparse_[last_entity_index] = dense_index_to_remove;
        }

        dense_.pop_back();
        dense_to_index_.pop_back();
        sparse_[entity_index] = kAbsent;
    }

    [[nodiscard]] bool has(int32_t entity_index) const noexcept {
        if (entity_index <= 0 ||
            entity_index >= static_cast<int32_t>(sparse_.size())) {
            return false;
        }
        return sparse_[entity_index] != kAbsent;
    }

    // Returns pointer into the dense array — valid until the next mutation.
    [[nodiscard]] T* get_ptr(int32_t entity_index) noexcept {
        if (!has(entity_index)) return nullptr;
        return &dense_[sparse_[entity_index]];
    }

    [[nodiscard]] const T* get_ptr(int32_t entity_index) const noexcept {
        if (!has(entity_index)) return nullptr;
        return &dense_[sparse_[entity_index]];
    }

    [[nodiscard]] int32_t count() const noexcept {
        return static_cast<int32_t>(dense_.size());
    }

    [[nodiscard]] const std::vector<int32_t>& dense_indices() const noexcept {
        return dense_to_index_;
    }

    [[nodiscard]] const std::vector<T>& dense() const noexcept {
        return dense_;
    }

private:
    void ensure_sparse_capacity(int32_t entity_index) {
        if (entity_index < static_cast<int32_t>(sparse_.size())) return;
        const std::size_t new_size =
            std::max<std::size_t>(
                static_cast<std::size_t>(entity_index) + 1,
                sparse_.empty() ? 256 : sparse_.size() * 2);
        sparse_.resize(new_size, kAbsent);
    }

    std::vector<int32_t> sparse_;
    std::vector<T> dense_;
    std::vector<int32_t> dense_to_index_;
};

} // namespace dualfrontier
