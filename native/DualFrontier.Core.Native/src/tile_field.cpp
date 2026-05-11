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
