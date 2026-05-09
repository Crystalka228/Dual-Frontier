#include "set_primitive.h"

#include <cstring>

namespace dualfrontier {

SetPrimitive::SetPrimitive(int32_t element_size)
    : element_size_(element_size), count_(0) {}

int32_t SetPrimitive::binary_search(const void* element) const noexcept {
    int32_t lo = 0;
    int32_t hi = count_;
    while (lo < hi) {
        const int32_t mid = lo + (hi - lo) / 2;
        const int cmp = std::memcmp(elements_.data() + mid * element_size_,
                                     element,
                                     static_cast<size_t>(element_size_));
        if (cmp == 0) {
            return mid;
        }
        if (cmp < 0) {
            lo = mid + 1;
        } else {
            hi = mid;
        }
    }
    return -(lo + 1);
}

int32_t SetPrimitive::add(const void* element) {
    const int32_t idx = binary_search(element);
    if (idx >= 0) {
        return 0;
    }

    const int32_t insert_pos = -(idx + 1);
    elements_.resize(static_cast<size_t>((count_ + 1) * element_size_));

    if (insert_pos < count_) {
        std::memmove(elements_.data() + (insert_pos + 1) * element_size_,
                     elements_.data() + insert_pos * element_size_,
                     static_cast<size_t>((count_ - insert_pos) * element_size_));
    }

    std::memcpy(elements_.data() + insert_pos * element_size_, element,
                static_cast<size_t>(element_size_));
    ++count_;
    return 1;
}

int32_t SetPrimitive::contains(const void* element) const noexcept {
    return binary_search(element) >= 0 ? 1 : 0;
}

int32_t SetPrimitive::remove(const void* element) {
    const int32_t idx = binary_search(element);
    if (idx < 0) {
        return 0;
    }
    if (idx < count_ - 1) {
        std::memmove(elements_.data() + idx * element_size_,
                     elements_.data() + (idx + 1) * element_size_,
                     static_cast<size_t>((count_ - idx - 1) * element_size_));
    }
    --count_;
    elements_.resize(static_cast<size_t>(count_ * element_size_));
    return 1;
}

int32_t SetPrimitive::iterate(void* out_buffer,
                               int32_t buffer_capacity) const noexcept {
    const int32_t to_write = (count_ < buffer_capacity) ? count_ : buffer_capacity;
    if (to_write > 0) {
        std::memcpy(out_buffer, elements_.data(),
                    static_cast<size_t>(to_write * element_size_));
    }
    return to_write;
}

} // namespace dualfrontier
