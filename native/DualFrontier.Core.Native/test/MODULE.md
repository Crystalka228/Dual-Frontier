---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-NATIVE-CORE-TEST
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-NATIVE-CORE-TEST
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-NATIVE-CORE-TEST
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-NATIVE-CORE-TEST
---
# DualFrontier.Core.Native/test — Native Self-tests

**Purpose**: Standalone executable validating C ABI without requiring .NET runtime. Run after every native build.

**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` §1.11

**Current scenarios** (К0 от branch):
- `scenario_basic_crud` — entity/component CRUD round-trip
- `scenario_deferred_destroy` — slot recycling, version invalidation
- `scenario_sparse_set_swap_remove` — swap-with-last correctness
- `scenario_throughput` — 100k entity stress test

**К-series additions**:
- К1: `scenario_bulk_add` — bulk operations correctness
- К1: `scenario_span_access` — span acquisition/release с atomic counter
- К3: `scenario_bootstrap_graph` — topological sort + parallel execution
- К5: `scenario_write_batch` — write command buffer parsing

**Run command** (post-build):
```bash
./build/df_native_selftest
# Expected output: ALL PASSED
```

**Status**: К0 имports existing 4 scenarios; subsequent milestones add scenarios.
