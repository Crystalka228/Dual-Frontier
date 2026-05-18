#include "managed_callback.h"

namespace dualfrontier {

ManagedCallbackRegistry::ManagedCallbackRegistry() = default;
ManagedCallbackRegistry::~ManagedCallbackRegistry() = default;

bool ManagedCallbackRegistry::register_callback(df_managed_batch_fn cb, void* user_data) {
    callback_ = cb;
    user_data_ = user_data;
    return true;
}

int32_t ManagedCallbackRegistry::dispatch_batch(const df_managed_system_batch* batch) {
    if (callback_ == nullptr || batch == nullptr) {
        return 0;
    }
    df_managed_system_batch local = *batch;
    if (local.user_data == nullptr) {
        // Use the user_data captured at registration time when caller did not
        // supply one in the batch (allows registration-time binding of the
        // managed context).
        local.user_data = user_data_;
    }
    callback_(&local);
    return 1;
}

int32_t ManagedCallbackRegistry::has_callback() const noexcept {
    return callback_ != nullptr ? 1 : 0;
}

void ManagedCallbackRegistry::clear() noexcept {
    callback_ = nullptr;
    user_data_ = nullptr;
}

ManagedCallbackRegistry& default_managed_callback_registry() {
    static ManagedCallbackRegistry instance;
    return instance;
}

} // namespace dualfrontier
