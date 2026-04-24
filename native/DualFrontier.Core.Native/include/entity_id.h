#pragma once

#include <cstdint>

namespace dualfrontier {

// Mirror of the C# EntityId(Index, Version) record struct.
// Stored on the native side as two 32-bit values; packed to uint64_t at the
// C ABI boundary.
struct EntityId {
    int32_t index = 0;
    int32_t version = 0;

    [[nodiscard]] bool is_valid() const noexcept {
        return index > 0 || version > 0;
    }
};

inline uint64_t pack_entity(EntityId id) noexcept {
    const uint64_t lo = static_cast<uint32_t>(id.index);
    const uint64_t hi = static_cast<uint32_t>(id.version);
    return (hi << 32) | lo;
}

inline EntityId unpack_entity(uint64_t packed) noexcept {
    EntityId id;
    id.index = static_cast<int32_t>(packed & 0xFFFFFFFFu);
    id.version = static_cast<int32_t>((packed >> 32) & 0xFFFFFFFFu);
    return id;
}

} // namespace dualfrontier
