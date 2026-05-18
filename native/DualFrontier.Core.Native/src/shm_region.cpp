#include "shm_region.h"

namespace dualfrontier {

ShmRegistry::ShmRegistry() = default;
ShmRegistry::~ShmRegistry() = default;

bool ShmRegistry::create(uint32_t region_id, int32_t size_bytes) {
    if (size_bytes <= 0) return false;
    auto it = regions_.find(region_id);
    if (it != regions_.end()) return false;  // duplicate
    Region r;
    r.data.resize(static_cast<std::size_t>(size_bytes), 0);
    regions_.emplace(region_id, std::move(r));
    return true;
}

void* ShmRegistry::map(uint32_t region_id) {
    auto it = regions_.find(region_id);
    if (it == regions_.end()) return nullptr;
    return it->second.data.data();
}

int32_t ShmRegistry::size(uint32_t region_id) const noexcept {
    auto it = regions_.find(region_id);
    if (it == regions_.end()) return 0;
    return static_cast<int32_t>(it->second.data.size());
}

bool ShmRegistry::unmap(uint32_t region_id) {
    // Single-process implementation: no-op. Just confirm existence.
    return regions_.find(region_id) != regions_.end();
}

bool ShmRegistry::destroy(uint32_t region_id) {
    return regions_.erase(region_id) > 0;
}

bool ShmRegistry::register_writer(uint32_t region_id, uint32_t writer_system_id) {
    auto it = regions_.find(region_id);
    if (it == regions_.end()) return false;
    it->second.writer_system_id = writer_system_id;
    return true;
}

uint32_t ShmRegistry::writer(uint32_t region_id) const noexcept {
    auto it = regions_.find(region_id);
    if (it == regions_.end()) return 0;
    return it->second.writer_system_id;
}

void ShmRegistry::clear() noexcept {
    regions_.clear();
}

ShmRegistry& default_shm_registry() {
    static ShmRegistry instance;
    return instance;
}

} // namespace dualfrontier
