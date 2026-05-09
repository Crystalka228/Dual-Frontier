#pragma once

#include <cstdint>
#include <string>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

// String pool with generational mod-scoped interning.
//
// Mirrors the K8.1 design contract recorded in
// tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md §1.2.
//
// IDs are uint32_t. Generation tags are uint32_t. ID 0 is the "empty"
// sentinel; generation 0 the "uninitialized" sentinel. Returned IDs are
// always > 0 for non-empty content.
//
// Mod scope semantics:
//   * begin_mod_scope("ModX") before any intern() calls from ModX's path.
//   * end_mod_scope("ModX") closes the scope; intern() outside scope lands
//     in the implicit "core" scope (mod_id = "").
//   * clear_mod_scope("ModX") releases IDs uniquely owned by ModX. IDs
//     also referenced by other mods stay alive. Reclaimed IDs get their
//     generation bumped so any outstanding {id, gen} reference resolves
//     to nullptr instead of silently aliasing a reused slot.
//
// Save/load: the pool itself is not serialised. Components serialise
// string CONTENT, not IDs. Load re-interns content; a fresh {id, gen}
// pair is issued. The generation tag is the safety net for any save
// that did persist an ID (e.g., diagnostic dumps, in-memory snapshots).
class StringPool {
public:
    static constexpr uint32_t kEmptyId = 0;
    static constexpr uint32_t kUninitializedGeneration = 0;

    StringPool();

    // Intern a string. If already present, returns the existing ID; the
    // current mod scope is recorded as a co-owner (idempotent insert).
    // If new, allocates a fresh ID (reusing a freed slot if available).
    // Empty content always maps to kEmptyId without allocating.
    [[nodiscard]] uint32_t intern(const std::string& content);

    // Resolve {id, expected_generation} to the underlying content. Returns
    // nullptr if the id is out of range, kEmptyId, or the generation tag
    // does not match (stale reference after a clear-and-reuse cycle).
    [[nodiscard]] const std::string* resolve(uint32_t id,
                                              uint32_t expected_generation) const noexcept;

    // Returns the generation tag currently associated with `id`. Caller
    // can use this to refresh a stale {id, gen} pair when the underlying
    // content is known and re-intern is preferred over content lookup.
    [[nodiscard]] uint32_t generation_for(uint32_t id) const noexcept;

    // Mod scope orchestration. begin/end form a stack of size 1 — re-entry
    // (begin without matching end) replaces the current scope. end() with
    // a mismatching mod_id is a programmer error and throws.
    void begin_mod_scope(const std::string& mod_id);
    void end_mod_scope(const std::string& mod_id);

    // Reclaims all IDs uniquely owned by `mod_id`. IDs co-owned by other
    // mods are retained. Safe to call without a matching begin/end pair.
    void clear_mod_scope(const std::string& mod_id);

    [[nodiscard]] int32_t count() const noexcept;
    [[nodiscard]] uint32_t current_generation() const noexcept { return current_generation_; }
    [[nodiscard]] const std::string& current_mod_scope() const noexcept { return current_mod_scope_; }

private:
    // contents_[id] is the string for that ID. Index 0 is the empty
    // sentinel and never referenced as content.
    std::vector<std::string> contents_;
    // generation_per_id_[id] tracks the generation tag for that slot.
    std::vector<uint32_t> generation_per_id_;
    // lookup_[content] -> id, used for dedup on intern.
    std::unordered_map<std::string, uint32_t> lookup_;
    // ids_by_mod_[mod_id] -> IDs interned (or co-claimed) under that scope.
    std::unordered_map<std::string, std::vector<uint32_t>> ids_by_mod_;
    // free_ids_ — IDs whose content was reclaimed and may be reused.
    std::vector<uint32_t> free_ids_;

    std::string current_mod_scope_;
    uint32_t current_generation_;

    void reclaim_id(uint32_t id);
};

} // namespace dualfrontier
