#include "component_store.h"

// All functionality is defined inline in component_store.h for the PoC.
// This translation unit exists so the CMake target has a dedicated object
// file for the type-erased store, making future non-inline additions (e.g.
// diagnostic dumps, stable ABI wrappers) straightforward to drop in without
// reshaping the build graph.

namespace dualfrontier {
} // namespace dualfrontier
