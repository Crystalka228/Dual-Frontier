#pragma once

#include <cstdint>

namespace dualfrontier {

// K10.1 Item 13 — Phase barrier semantics formalization per spec §3.3.
//
// Three barrier types per К-L12. Default Full preserves correctness;
// Partial/None opt-in for optimization. Per-phase barrier type set on the
// system graph phase descriptor; scheduler honors during dispatch.
enum class BarrierType : int32_t {
    // All systems in phase N complete before any system in phase N+1 starts.
    // Default.
    Full    = 0,
    // Phase N+1 systems depending only on subset of phase N can start as soon
    // as that subset completes (data-flow-driven, not phase-boundary).
    Partial = 1,
    // Phases independent enough that overlap permitted; primarily для
    // diagnostic / observability phases that don't write component state.
    None    = 2,
};

} // namespace dualfrontier
