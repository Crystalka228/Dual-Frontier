#include "keyed_map.h"

#include <cstring>

namespace dualfrontier {

KeyedMap::KeyedMap(int32_t key_size, int32_t value_size)
    : key_size_(key_size), value_size_(value_size), count_(0) {}

int32_t KeyedMap::binary_search(const void* key) const noexcept {
    int32_t lo = 0;
    int32_t hi = count_;
    while (lo < hi) {
        const int32_t mid = lo + (hi - lo) / 2;
        const int cmp = std::memcmp(keys_.data() + mid * key_size_, key,
                                     static_cast<size_t>(key_size_));
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

int32_t KeyedMap::set(const void* key, const void* value) {
    const int32_t idx = binary_search(key);
    if (idx >= 0) {
        std::memcpy(values_.data() + idx * value_size_, value,
                    static_cast<size_t>(value_size_));
        return 0;
    }

    const int32_t insert_pos = -(idx + 1);
    keys_.resize(static_cast<size_t>((count_ + 1) * key_size_));
    values_.resize(static_cast<size_t>((count_ + 1) * value_size_));

    if (insert_pos < count_) {
        std::memmove(keys_.data() + (insert_pos + 1) * key_size_,
                     keys_.data() + insert_pos * key_size_,
                     static_cast<size_t>((count_ - insert_pos) * key_size_));
        std::memmove(values_.data() + (insert_pos + 1) * value_size_,
                     values_.data() + insert_pos * value_size_,
                     static_cast<size_t>((count_ - insert_pos) * value_size_));
    }

    std::memcpy(keys_.data() + insert_pos * key_size_, key,
                static_cast<size_t>(key_size_));
    std::memcpy(values_.data() + insert_pos * value_size_, value,
                static_cast<size_t>(value_size_));
    ++count_;
    return 1;
}

int32_t KeyedMap::get(const void* key, void* out_value) const noexcept {
    const int32_t idx = binary_search(key);
    if (idx < 0) {
        return 0;
    }
    std::memcpy(out_value, values_.data() + idx * value_size_,
                static_cast<size_t>(value_size_));
    return 1;
}

int32_t KeyedMap::remove(const void* key) {
    const int32_t idx = binary_search(key);
    if (idx < 0) {
        return 0;
    }
    if (idx < count_ - 1) {
        std::memmove(keys_.data() + idx * key_size_,
                     keys_.data() + (idx + 1) * key_size_,
                     static_cast<size_t>((count_ - idx - 1) * key_size_));
        std::memmove(values_.data() + idx * value_size_,
                     values_.data() + (idx + 1) * value_size_,
                     static_cast<size_t>((count_ - idx - 1) * value_size_));
    }
    --count_;
    keys_.resize(static_cast<size_t>(count_ * key_size_));
    values_.resize(static_cast<size_t>(count_ * value_size_));
    return 1;
}

int32_t KeyedMap::iterate(void* out_keys, void* out_values,
                          int32_t buffer_capacity) const noexcept {
    const int32_t to_write = (count_ < buffer_capacity) ? count_ : buffer_capacity;
    if (to_write > 0) {
        std::memcpy(out_keys, keys_.data(),
                    static_cast<size_t>(to_write * key_size_));
        std::memcpy(out_values, values_.data(),
                    static_cast<size_t>(to_write * value_size_));
    }
    return to_write;
}

int32_t KeyedMap::clear() noexcept {
    const int32_t prev = count_;
    count_ = 0;
    keys_.clear();
    values_.clear();
    return prev;
}

} // namespace dualfrontier
