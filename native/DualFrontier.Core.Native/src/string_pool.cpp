#include "string_pool.h"

#include <algorithm>
#include <stdexcept>

namespace dualfrontier {

StringPool::StringPool()
    : contents_(1, std::string{}),
      generation_per_id_(1, kUninitializedGeneration),
      current_generation_(1) {}

uint32_t StringPool::intern(const std::string& content) {
    if (content.empty()) {
        return kEmptyId;
    }

    auto it = lookup_.find(content);
    if (it != lookup_.end()) {
        const uint32_t id = it->second;
        auto& mod_ids = ids_by_mod_[current_mod_scope_];
        if (std::find(mod_ids.begin(), mod_ids.end(), id) == mod_ids.end()) {
            mod_ids.push_back(id);
        }
        return id;
    }

    uint32_t id;
    if (!free_ids_.empty()) {
        id = free_ids_.back();
        free_ids_.pop_back();
        contents_[id] = content;
    } else {
        id = static_cast<uint32_t>(contents_.size());
        contents_.push_back(content);
        generation_per_id_.push_back(current_generation_);
    }
    generation_per_id_[id] = current_generation_;
    lookup_[content] = id;
    ids_by_mod_[current_mod_scope_].push_back(id);
    return id;
}

const std::string* StringPool::resolve(uint32_t id,
                                        uint32_t expected_generation) const noexcept {
    if (id == kEmptyId || id >= contents_.size()) {
        return nullptr;
    }
    if (generation_per_id_[id] != expected_generation) {
        return nullptr;
    }
    return &contents_[id];
}

uint32_t StringPool::generation_for(uint32_t id) const noexcept {
    if (id == kEmptyId || id >= generation_per_id_.size()) {
        return kUninitializedGeneration;
    }
    return generation_per_id_[id];
}

void StringPool::begin_mod_scope(const std::string& mod_id) {
    current_mod_scope_ = mod_id;
    ids_by_mod_.try_emplace(mod_id);
}

void StringPool::end_mod_scope(const std::string& mod_id) {
    if (current_mod_scope_ != mod_id) {
        throw std::logic_error("StringPool::end_mod_scope: mod_id != current scope");
    }
    current_mod_scope_.clear();
}

void StringPool::clear_mod_scope(const std::string& mod_id) {
    auto it = ids_by_mod_.find(mod_id);
    if (it == ids_by_mod_.end()) {
        return;
    }

    // Snapshot the IDs to reclaim — we mutate ids_by_mod_ while iterating
    // (the entry for mod_id is erased at the end), so we cannot rely on
    // `it` after the loop body if it triggers any rehash. Take a copy.
    const std::vector<uint32_t> ids_to_check = it->second;

    for (const uint32_t id : ids_to_check) {
        bool referenced_elsewhere = false;
        for (const auto& [other_mod, other_ids] : ids_by_mod_) {
            if (other_mod == mod_id) {
                continue;
            }
            if (std::find(other_ids.begin(), other_ids.end(), id) != other_ids.end()) {
                referenced_elsewhere = true;
                break;
            }
        }
        if (!referenced_elsewhere) {
            reclaim_id(id);
        }
    }
    ids_by_mod_.erase(mod_id);
}

void StringPool::reclaim_id(uint32_t id) {
    if (id == kEmptyId || id >= contents_.size()) {
        return;
    }
    lookup_.erase(contents_[id]);
    contents_[id].clear();
    ++current_generation_;
    generation_per_id_[id] = current_generation_;
    free_ids_.push_back(id);
}

int32_t StringPool::count() const noexcept {
    return static_cast<int32_t>(contents_.size() - 1 - free_ids_.size());
}

} // namespace dualfrontier
