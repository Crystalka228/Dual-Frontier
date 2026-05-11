#ifndef DF_TILE_FIELD_H
#define DF_TILE_FIELD_H

#include <atomic>
#include <cstdint>
#include <cstring>
#include <stdexcept>
#include <vector>

namespace dualfrontier {

// RawTileField — type-erased dense 2D grid storage.
//
// Element type is fixed at registration via cell_size; the kernel does not
// know the static type. Caller (managed bridge) tracks the type and copies
// cell_size bytes per cell on read/write.
//
// Storage layout per field:
//   data_           — primary buffer, width * height * cell_size bytes
//   back_buffer_    — ping-pong target, identical layout to data_
//   conductivity_   — per-cell float D coefficient, width * height floats
//   storage_flags_  — per-cell bit, byte-packed (width * height + 7) / 8 bytes
//
// Mutation rejection:
//   active_spans_ atomic counter; while > 0, write_cell, set_conductivity,
//   set_storage_flag, swap_buffers all throw std::logic_error.
//   Caller must release every acquired span before mutating.
//
// All bounds checks return 0 on out-of-range; callers must validate input
// before assuming success.

class RawTileField {
public:
    RawTileField(int32_t width, int32_t height, int32_t cell_size);
    ~RawTileField() = default;

    RawTileField(const RawTileField&) = delete;
    RawTileField& operator=(const RawTileField&) = delete;
    RawTileField(RawTileField&&) = delete;
    RawTileField& operator=(RawTileField&&) = delete;

    int32_t width() const noexcept { return width_; }
    int32_t height() const noexcept { return height_; }
    int32_t cell_size() const noexcept { return cell_size_; }

    // Point access. Returns 1 on success, 0 on out-of-bounds or size mismatch.
    int32_t read_cell(int32_t x, int32_t y, void* out, int32_t size) const;
    int32_t write_cell(int32_t x, int32_t y, const void* in, int32_t size);

    // Span access. Returns 1 on success, 0 on failure.
    // Increments active_spans_; caller MUST call release_span.
    int32_t acquire_span(const void** out_data, int32_t* out_width, int32_t* out_height);
    void    release_span() noexcept;

    // Conductivity map. Default value 1.0 (uniform isotropic).
    int32_t set_conductivity(int32_t x, int32_t y, float value);
    float   get_conductivity(int32_t x, int32_t y) const;

    // Storage flag (per-cell bit; default 0).
    int32_t set_storage_flag(int32_t x, int32_t y, int32_t enabled);
    int32_t get_storage_flag(int32_t x, int32_t y) const;

    // Ping-pong buffer swap; throws if any span active.
    void swap_buffers();

    // Internal access for friend (CPU reference kernel needs both buffers).
    uint8_t* data_ptr() noexcept { return data_.data(); }
    uint8_t* back_buffer_ptr() noexcept { return back_buffer_.data(); }
    float* conductivity_ptr() noexcept { return conductivity_.data(); }
    const uint8_t* storage_flags_ptr() const noexcept { return storage_flags_.data(); }

private:
    int32_t width_;
    int32_t height_;
    int32_t cell_size_;
    std::vector<uint8_t> data_;
    std::vector<uint8_t> back_buffer_;
    std::vector<float>   conductivity_;
    std::vector<uint8_t> storage_flags_;
    std::atomic<int32_t> active_spans_{0};

    void throw_if_spans_active() const;
};

}  // namespace dualfrontier

#endif  // DF_TILE_FIELD_H
