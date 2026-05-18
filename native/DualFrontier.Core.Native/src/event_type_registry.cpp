#include "event_type_registry.h"

#include <mutex>
#include <unordered_map>

namespace dualfrontier {
namespace {

struct RegistryEntry {
    BusTier    tier;
    uint32_t   payload_size_bytes;
    const char* fqn;
    CoalesceFn coalesce_fn;
};

// Process-global registry. Single shared instance per К-L15 native authority —
// bus dispatch (Item 26) and capability registry (Item 27) consult the same
// tier metadata.
//
// Concurrency: registration occurs at startup / mod load (single-threaded
// caller под scheduler critical section per Step 3.5 native primitive surface);
// lookups happen on hot path (per publish call). Read-write mutex avoids
// reader contention on the typical case.
std::unordered_map<uint32_t, RegistryEntry>& registry() {
    static std::unordered_map<uint32_t, RegistryEntry> g_registry;
    return g_registry;
}

std::mutex& registry_mutex() {
    static std::mutex g_mutex;
    return g_mutex;
}

} // namespace
} // namespace dualfrontier

using namespace dualfrontier;

extern "C" {

DF_API int32_t df_event_type_registry_register(
    uint32_t type_id,
    int32_t  tier_value,
    uint32_t payload_size_bytes,
    const char* fqn,
    void (*coalesce_fn)(void*, const void*))
{
    if (tier_value < 0 || tier_value > 2) return 0;
    if (fqn == nullptr) return 0;
    BusTier tier = static_cast<BusTier>(tier_value);
    // Background tier requires coalesce function declaration (per К10.2 default
    // per Q-N-34 + spec §3.8 Item 28). Brief notes Items 29 surfaces
    // BackgroundCoalesceMissing diagnostic к ValidationErrorKind; here the
    // native registry returns failure при missing coalesce_fn for Background
    // tier, allowing managed-side к surface validation error.
    if (tier == BusTier::Background && coalesce_fn == nullptr) return 0;

    std::lock_guard<std::mutex> lock(registry_mutex());
    auto& reg = registry();
    auto [it, inserted] = reg.try_emplace(type_id);
    if (!inserted) {
        // Re-registration с identical metadata is idempotent; differing
        // metadata returns failure (managed side may surface as conflict).
        const auto& existing = it->second;
        if (existing.tier != tier ||
            existing.payload_size_bytes != payload_size_bytes ||
            existing.coalesce_fn != coalesce_fn) {
            return 0;
        }
        return 1;
    }
    it->second = RegistryEntry{tier, payload_size_bytes, fqn, coalesce_fn};
    return 1;
}

DF_API int32_t df_event_type_registry_lookup(
    uint32_t type_id,
    int32_t* out_tier,
    uint32_t* out_payload_size_bytes,
    const char** out_fqn,
    void (**out_coalesce_fn)(void*, const void*))
{
    std::lock_guard<std::mutex> lock(registry_mutex());
    auto& reg = registry();
    auto it = reg.find(type_id);
    if (it == reg.end()) return 0;
    if (out_tier)               *out_tier               = static_cast<int32_t>(it->second.tier);
    if (out_payload_size_bytes) *out_payload_size_bytes = it->second.payload_size_bytes;
    if (out_fqn)                *out_fqn                = it->second.fqn;
    if (out_coalesce_fn)        *out_coalesce_fn        = it->second.coalesce_fn;
    return 1;
}

DF_API int32_t df_event_type_registry_get_tier(
    uint32_t type_id,
    int32_t* out_tier)
{
    if (out_tier == nullptr) return 0;
    std::lock_guard<std::mutex> lock(registry_mutex());
    auto& reg = registry();
    auto it = reg.find(type_id);
    if (it == reg.end()) {
        // Default tier per S-LOCK-4: unregistered events map к Normal tier
        // (backward compatibility for legacy events без [EventTier] annotation).
        *out_tier = static_cast<int32_t>(BusTier::Normal);
        return 1;
    }
    *out_tier = static_cast<int32_t>(it->second.tier);
    return 1;
}

DF_API int32_t df_event_type_registry_count(void) {
    std::lock_guard<std::mutex> lock(registry_mutex());
    return static_cast<int32_t>(registry().size());
}

DF_API void df_event_type_registry_clear(void) {
    std::lock_guard<std::mutex> lock(registry_mutex());
    registry().clear();
}

} // extern "C"
