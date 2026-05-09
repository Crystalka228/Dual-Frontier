#include "composite.h"

#include <cstring>

namespace dualfrontier {

Composite::Composite(int32_t element_size) : element_size_(element_size) {}

int32_t Composite::add(uint64_t parent, const void* element) {
    auto& vec = data_per_parent_[parent];
    const size_t old_size = vec.size();
    vec.resize(old_size + static_cast<size_t>(element_size_));
    std::memcpy(vec.data() + old_size, element, static_cast<size_t>(element_size_));
    return 1;
}

int32_t Composite::get_count(uint64_t parent) const noexcept {
    auto it = data_per_parent_.find(parent);
    if (it == data_per_parent_.end()) {
        return 0;
    }
    return static_cast<int32_t>(it->second.size() / static_cast<size_t>(element_size_));
}

int32_t Composite::get_at(uint64_t parent, int32_t index, void* out_element) const noexcept {
    auto it = data_per_parent_.find(parent);
    if (it == data_per_parent_.end()) {
        return 0;
    }
    const int32_t count = static_cast<int32_t>(
        it->second.size() / static_cast<size_t>(element_size_));
    if (index < 0 || index >= count) {
        return 0;
    }
    std::memcpy(out_element, it->second.data() + index * element_size_,
                static_cast<size_t>(element_size_));
    return 1;
}

int32_t Composite::remove_at(uint64_t parent, int32_t index) {
    auto it = data_per_parent_.find(parent);
    if (it == data_per_parent_.end()) {
        return 0;
    }
    const int32_t count = static_cast<int32_t>(
        it->second.size() / static_cast<size_t>(element_size_));
    if (index < 0 || index >= count) {
        return 0;
    }
    const int32_t last_index = count - 1;
    if (index != last_index) {
        std::memcpy(it->second.data() + index * element_size_,
                    it->second.data() + last_index * element_size_,
                    static_cast<size_t>(element_size_));
    }
    it->second.resize(static_cast<size_t>(last_index * element_size_));
    return 1;
}

int32_t Composite::clear_for(uint64_t parent) {
    return data_per_parent_.erase(parent) > 0 ? 1 : 0;
}

int32_t Composite::iterate(uint64_t parent, void* out_buffer,
                            int32_t buffer_capacity) const noexcept {
    auto it = data_per_parent_.find(parent);
    if (it == data_per_parent_.end()) {
        return 0;
    }
    const int32_t count = static_cast<int32_t>(
        it->second.size() / static_cast<size_t>(element_size_));
    const int32_t to_write = (count < buffer_capacity) ? count : buffer_capacity;
    if (to_write > 0) {
        std::memcpy(out_buffer, it->second.data(),
                    static_cast<size_t>(to_write * element_size_));
    }
    return to_write;
}

} // namespace dualfrontier
