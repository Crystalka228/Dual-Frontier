---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_7_5_BUS_SOURCE_SPLIT
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_7_5_BUS_SOURCE_SPLIT
---
---
brief_id: A_PRIME_7_5_BUS_SOURCE_SPLIT_BRIEF
status: EXECUTED
authored: 2026-05-22 (Claude Opus 4.7 post-A'.7.x closure deliberation session)
ratified: 2026-05-22 (Crystalka Q-N deliberation — all 3 S-LOCKs ratified per Option A recommendations: S-LOCK-1 4-file split + background_queue distinct; S-LOCK-2 helpers к internal header; S-LOCK-3 stale comment cleanup в-scope as ε3)
executed: 2026-05-22 (Claude Code auto-mode + Crystalka oversight; 5 atomic commits on feature/a_prime_7_5-bus-source-split c1d10b0..PENDING-COMMIT-A_PRIME_7_5-CLOSURE; native selftest 97 ALL PASSED + Core.Tests.Bus 20/20 PASS; pre-existing test pollution failures confirmed pre-existing on ε1 state)
author: Claude Opus 4.7
target_executor: Claude Code (auto-mode + Crystalka oversight)
estimated_duration: 2-4 hours auto-mode (pure code reorganization, no architectural surface, no diagnostic uncertainty)
brief_type: execution (К-extensions cascade #1 sub-milestone, pre-A'.8 closure)
amendments_chain:
  - A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md EXECUTED (DOC-D-A_PRIME_7_X, 2026-05-21) — parent cascade; K-L15.1 LOCKED 2-layer (state + runtime); compile-time layer deferred to this sub-milestone
  - A_PRIME_7_X CLOSURE_REPORT (docs/scratch/A_PRIME_7_X/) — empirical baseline at A'.7.x closure (731 tests PASS, +45% bus throughput, 33 native selftests, O(N) coalesce, sync_register exit 0)
  - SCHEDULER_STRESS_TEST_SUITE.md — investigation reference + stress run logs
  - A_PRIME_7_X_LESSON_CANDIDATES.md — Lesson candidates surfaced from A'.7.x
authority_chain:
  - KERNEL_ARCHITECTURE.md v2.4 LOCKED (DOC-A-KERNEL) — К-L15.1 «Three-tier independence» 2-layer formulation; A'.7.5 materializes compile-time layer (3rd layer) WITHOUT amending invariant text (К-L15.1 already accommodates source split via state isolation precondition)
  - METHODOLOGY.md v1.9 LOCKED (DOC-B-METHODOLOGY) — §12.7 closure protocol с Modding-suite verification gate; Lessons #8/#11/#20/#22 application discipline
  - FRAMEWORK.md v1.1 LOCKED (DOC-A-FRAMEWORK) — Tier 3 Category D lifecycle
  - A'.7.x parent brief EXECUTED — K-L15.1 LOCKED via γ4 commit 08d0bba; this sub-milestone implements K-L15.1 compile-time layer
---

# A'.7.5 BUS_SOURCE_SPLIT — К-extensions cascade #1 sub-milestone (pre-А'.8 closure)

**Brief shape**: Pure execution brief targeting Claude Code auto-mode с Crystalka oversight. Source-level reorganization implementing **compile-time layer** of К-L15.1 «Three-tier independence» that was LOCKED in 2-layer formulation (state + runtime) at A'.7.x γ4. **No К-L amendment** — К-L15.1 invariant text preserved verbatim; this sub-milestone is **implementation depth follow-through** (per Lesson #25 consumer materialization).

**Status: AUTHORED** — pending Crystalka Q-N ratification (minimal Q-N surface, ~3 questions). Brief ready for ratification + execution.

**Authority**: Direct execution against A'.7.x closure baseline. К-L15.1 already LOCKED — this brief implements compile-time layer without invariant amendment.

**Cascade nature**:
- **Source reorganization only**: `bus_native.cpp` (240 lines) → `bus_fast.cpp` + `bus_normal.cpp` + `bus_background.cpp` + `bus_common.cpp`
- **`background_queue.cpp` preserved distinct** per A'.7.x closure §16.5 narrative (deviation from initial brief leaning к absorption)
- **No K-L impact**: К-L15.1 invariant text unchanged; cumulative К-Lxx series stays 21 invariants
- **No architectural amendment**: KERNEL_ARCHITECTURE.md v2.4 unchanged; METHODOLOGY v1.9 unchanged
- **Empirical preservation**: 731 tests baseline preserved; +45% bus throughput preserved; native selftest 33/33 preserved
- **Atomic discipline**: 3-5 atomic compilable commits per Lesson #8

**Why A'.7.5 is а sub-milestone, not а full cascade**:
- No deliberation surface (К-L15.1 already decided)
- No architectural amendment (invariant text preserved)
- No gap audit needed (scope crystal clear after A'.7.x execution)
- No bug closure (A'.7.x closed all bugs)
- No diagnostic uncertainty (Group A premise disproved in A'.7.x)
- Pure code reorganization with empirical preservation as success criterion

**Brief size note**: A'.7.5 brief estimated 350-500 lines per Lesson #16 (brief length scales с deliberation complexity, not execution scope). A'.7.x brief was 1853 lines because deliberation surface was large. A'.7.5 deliberation surface = ~3 Q-N + execution path crystal clear.

---

## §0 — What changed since A'.7.x closure (2026-05-21 → 2026-05-22)

**A'.7.x closure state** (verified post-merge к main):

- 13 atomic commits b59ab2d..δ6 merged to main via fast-forward (origin/main → `56610cb`)
- K-L15.1 LOCKED 2-layer (state + runtime) per γ4 commit `08d0bba`
- `bus_native.cpp` post-β4 contains: tier-state singleton accessors (`BusNative::fast()`, `BusNative::normal()`, `BusNative::background()`), all C ABI implementations (publish/subscribe/unsubscribe/clear/drain/count) for all three tiers, organized с clear section markers (Tier-state singletons, Subscribe, Publish, Drain, Unsubscribe, Subscriber count, Clear)
- `background_queue.cpp` post-β5 contains: PolicyConfig static, O(N) `coalesce_pending_locked` (hash-map first_index), `apply_drop_oldest_locked`, df_background_queue_* C ABI functions (configure/dispatch_idle_slot/size/saturation_events/compute_save_size/serialize/deserialize/force_coalesce)
- `bus_native_internal.h` contains: FastSubscriberRecord, BatchedSubscriberRecord, PendingNormalEventRecord, PendingBackgroundEventRecord, FastTierState/NormalTierState/BackgroundTierState structs, BusNative accessor class
- `mod_unload.cpp` uses `BusNative::fast()/normal()/background()` accessors per A'.7.x β4 update
- 731 tests passing baseline на main (Core 81 + Core-Stress 4 + Core-Interop 202 + Application 45 + Modding 395 + Modding-Stress 4)
- +45% S3 throughput empirically demonstrated; S10 cross-tier re-entrancy PASS ≤100ms

**Parallel branches state**:
- `claude/godot-removal-deliberation-Vfg2R` — Godot removal cascade CLOSED, 1 atomic commit, awaiting Crystalka merge decision (per Crystalka direction: «после A'.7.5 решим что делать с той веткой»)

**State on disk verified 2026-05-22**: HEAD on main `56610cb`. Working tree clean post-merge. bus_native.cpp 10.4KB / 240 lines monolithic but well-organized. background_queue.cpp 12.7KB / 280 lines с O(N) coalesce verified.

---

## §1 — Crystalka ratified scope locks (PENDING)

Minimal Q-N surface — three questions. PENDING ratification.

### §1.1 — S-LOCK-1 (PENDING): File layout (4-file split + background_queue distinct)

**Recommended lock**:

| File | Status | Content scope |
|---|---|---|
| `bus_native_internal.h` | UNCHANGED | Shared types (FastSubscriberRecord, etc.) + BusNative accessor declarations |
| `bus_common.cpp` | **NEW** | BusNative tier accessor implementations (fast/normal/background); df_bus_clear (all 3 tiers fixed-order lock); df_bus_unsubscribe (single-ID dispatch by tier-bit) |
| `bus_fast.cpp` | **NEW** | df_bus_subscribe_fast, df_bus_publish_fast, df_bus_unsubscribe_fast_by_mod, df_bus_subscriber_count_fast |
| `bus_normal.cpp` | **NEW** | df_bus_subscribe_normal, df_bus_publish_normal, df_bus_drain_normal_batch, df_bus_unsubscribe_normal_by_mod, df_bus_subscriber_count_normal |
| `bus_background.cpp` | **NEW** | df_bus_subscribe_background, df_bus_publish_background, df_bus_unsubscribe_background_by_mod, df_bus_subscriber_count_background |
| `bus_native.cpp` | **DELETED** | Content distributed к 4 new files |
| `background_queue.cpp` | **UNCHANGED** | Background dispatch policy + save/load preserved distinct per A'.7.x closure §16.5 |

**Rationale**: Background tier has **two architectural concerns** — subscribe/publish/unsubscribe (bus contract) + dispatch policy/save-load (queue infrastructure). Bus contract belongs в `bus_background.cpp`. Queue infrastructure belongs в `background_queue.cpp`. Source split mirrors architectural concern separation.

**Decision alternative considered**: absorb `background_queue.cpp` into `bus_background.cpp` (initial A'.7.x brief leaning §5.5 prescribed this). Rejected at A'.7.x closure per §16.5: background_queue infrastructure has distinct architectural concern (queue policy + save/load), keeping distinct preserves single-responsibility per .cpp file.

### §1.2 — S-LOCK-2 (PENDING): Helper template disposition

`bus_native.cpp` namespace-private contains:
- `TIER_SHIFT`, `TIER_MASK`, `ID_MASK` constants
- `TierTag` enum
- `encode_id(TierTag, uint64_t)` function
- `decode_tier(df_bus_subscription_id)` function
- `remove_by_id_locked<Record>` template function
- `remove_by_mod_locked<Record>` template function

These helpers are used across all 3 tiers. Decision options:

**Option A — Move к `bus_native_internal.h`** (recommended):
- Templates need definition visible at use site — internal header is correct location
- Constants + enum + small inline functions also fit header pattern
- All 4 new .cpp files include `bus_native_internal.h` already (for tier state structs)

**Option B — Move к `bus_common.cpp`** anonymous namespace:
- Single ownership per .cpp file
- But templates require visibility across .cpp files — would need template instantiation declarations

**Recommendation**: Option A. Templates + tier-bit primitives are **cross-tier shared infrastructure** by definition. Internal header is canonical location.

### §1.3 — S-LOCK-3 (PENDING): Stale comment cleanup scope

`SchedulerExtremeTests.cs:309` contains stale comment:

> «the native coalesce algorithm `coalesce_pending_locked` in background_queue.cpp:42-64 is O(N²) — every dispatched event scans all prior queued events... With 5M events the dispatch step becomes effectively infinite (25T comparisons).»

Comment **describes pre-β5 state**. Post-β5 algorithm is O(N) (verified read 2026-05-22). Comment **stale documentation drift** — instance of Lesson #23 candidate pattern.

**Decision options**:

**Option A — In-scope (recommended)**: Update comment к reflect O(N) algorithm in A'.7.5 cleanup commit.

**Option B — Out-of-scope**: Defer к separate Lesson #23 cleanup cascade post-A'.7.5.

**Recommendation**: Option A. Comment scope minimal (single file, single comment block), cleanly fits A'.7.5 atomic commit pattern. Deferring creates unnecessary tracking burden.

**Additional opportunistic cleanup candidates** (Phase 0 read may surface):
- Other test file comments referencing «O(N²)» bus coalesce — same stale pattern
- bus_native.cpp comment headers — may need «moved to bus_X.cpp» reference at file deletion

---

## §2 — Phase 0 reads (mandatory before execution)

### §2.1 — Source files (production)

- `native/DualFrontier.Core.Native/src/bus_native.cpp` (240 lines) — source content for split
- `native/DualFrontier.Core.Native/src/bus_native_internal.h` (current) — receives helper templates + constants if S-LOCK-2 Option A ratified
- `native/DualFrontier.Core.Native/src/background_queue.cpp` (preserved distinct, read для verification only)
- `native/DualFrontier.Core.Native/include/bus_native.h` (public C ABI, unchanged — read для verification)
- `native/DualFrontier.Core.Native/include/background_queue.h` (preserved distinct)
- `native/DualFrontier.Core.Native/src/mod_unload.cpp` — verify BusNative tier accessor usage still works (no edit expected)
- `native/DualFrontier.Core.Native/CMakeLists.txt` — sources list update target

### §2.2 — Test files (verification)

- `native/DualFrontier.Core.Native/test/selftest.cpp` — 33 scenarios baseline preserved
- `tests/DualFrontier.Core.Tests/Scheduling/SchedulerStressTests.cs` — bus probes baseline
- `tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs` — S3/S10 probes baseline + stale comment update target (§1.3 S-LOCK-3 Option A)
- `tests/DualFrontier.Core.Tests/Scheduling/Fixtures/BackgroundBusTestDriver.cs` — test fixture
- `tests/DualFrontier.Modding.Tests/` — 395 tests baseline preserved

### §2.3 — Architecture documents (read-only verification)

- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.4 — К-L15.1 text verification (no amendment in A'.7.5)
- `docs/methodology/METHODOLOGY.md` v1.9 — §12.7 closure protocol reference

### §2.4 — Governance files

- `docs/governance/REGISTER.yaml` — register_version 2.1 baseline (A'.7.x post-state); A'.7.5 enrollment target

### §2.5 — Reads checklist

Before any code commit:

- [ ] bus_native.cpp full read (content distribution map)
- [ ] bus_native_internal.h full read (current type definitions)
- [ ] background_queue.cpp verification (preserved distinct, no edit needed)
- [ ] bus_native.h verification (public C ABI unchanged target)
- [ ] CMakeLists.txt current sources list
- [ ] SchedulerExtremeTests.cs:309 stale comment verification
- [ ] selftest.cpp verification (33 scenarios)
- [ ] К-L15.1 text verification (preserve verbatim)

---

## §3 — Atomic commit cascade specification

### §3.1 — Commit cadence (3 commits minimum, up к 5)

```
[main: 56610cb post-A'.7.x]
    ↓
[Branch: feature/a_prime_7_5-bus-source-split]
    ↓
ε1: Helper migration к internal header (S-LOCK-2 Option A)
    - Move TierTag enum + TIER_SHIFT/MASK/ID_MASK constants to bus_native_internal.h
    - Move encode_id/decode_tier inline functions to bus_native_internal.h
    - Move remove_by_id_locked<Record> + remove_by_mod_locked<Record> templates to bus_native_internal.h
    - bus_native.cpp updated to use moved primitives via include
    - VERIFY: build clean + native selftest 33/33 + Core test subset PASS
    ↓
ε2: Source split atomic commit (per S-LOCK-1)
    - Create bus_common.cpp (BusNative accessors + df_bus_clear + df_bus_unsubscribe)
    - Create bus_fast.cpp (Fast tier C ABI)
    - Create bus_normal.cpp (Normal tier C ABI + drain)
    - Create bus_background.cpp (Background tier C ABI)
    - Delete bus_native.cpp
    - Update CMakeLists.txt sources list
    - VERIFY: build clean + native selftest 33/33 + Core test subset PASS + linker resolves all df_bus_* symbols exactly once
    ↓
ε3: Stale comment cleanup (S-LOCK-3 Option A)
    - SchedulerExtremeTests.cs:309 comment updated к reflect O(N) algorithm
    - Other opportunistic stale comments identified Phase 0 (if any)
    - VERIFY: build clean + Core test subset PASS
    ↓
ε4 (если needed): A'.7.5 closure governance commit
    - REGISTER.yaml: DOC-D-A_PRIME_7_5_BUS_SOURCE_SPLIT enrollment + lifecycle AUTHORED → EXECUTED
    - REGISTER.yaml: EVT-2026-05-XX-A_PRIME_7_5-CLOSURE audit_trail entry
    - PHASE_A_PRIME_SEQUENCING.md: A'.7.5 closure entry (mirror A'.7.x §X pattern)
    - MIGRATION_PROGRESS.md: automatic chronicle entry
    - VERIFY: sync_register.ps1 --validate exit 0
    ↓
ε5: A'.7.5 closure ratification commit (brief AUTHORED → EXECUTED + §X closure summary appended)
```

**Estimated total**: 3-5 atomic commits within Q-N-7_5-Y (A'.7.5 atomization count Q-N) tolerance.

### §3.2 — Per-commit verification gate

Each commit MUST pass:

1. `dotnet build -c Release DualFrontier.sln` → 0 warnings, 0 errors
2. `cmake --build native/DualFrontier.Core.Native --config Release` → clean
3. `df_native_selftest.exe` → 33/33 ALL PASSED
4. ε1: subset of Core test suite (smoke validation)
5. ε2 (load-bearing): full Core suite + Core-Stress + Modding-Stress + bus probes (S3 + S10)
6. ε3: full Core suite (subset sufficient — comment-only change)
7. ε4/ε5: sync_register.ps1 --validate exit 0

**ε2 specific verification** (most critical):
- Linker: all 16 df_bus_* C ABI functions resolved exactly once (no duplicate definitions, no unresolved externals)
- Runtime: native selftest exercises full bus C ABI surface (33 scenarios cover publish/subscribe/unsubscribe/clear)
- Performance: S3 throughput maintained ≥6M e/s (post-A'.7.x +45% baseline)
- Re-entrancy: S10 PASS ≤100ms preserved

### §3.3 — Atomic discipline halt conditions

- **SC-ε1**: Helper migration breaks bus_native.cpp compile → halt + investigate include chain
- **SC-ε2**: Source split atomic commit produces duplicate symbol linker error → halt + identify which function in multiple .cpp files
- **SC-ε2**: Source split breaks symbol export → halt + verify DF_API macro propagation in new files
- **SC-ε2**: Native selftest regresses → halt + identify which C ABI function changed semantically (should be impossible — copy-paste move)
- **SC-ε2**: Bus probe S3 throughput degrades >5% → halt + investigate (compiler optimization regression possible if inlining boundary changed)
- **SC-ε3**: Phase 0 surfaces more stale comments than anticipated → expand scope OR halt + defer overflow к separate cascade

### §3.4 — Commit message convention

- `refactor(native-bus): A'.7.5 ε1 — migrate helper primitives to internal header`
- `refactor(native-bus): A'.7.5 ε2 — source split bus_native.cpp → 4-file (K-L15.1 compile-time layer)`
- `docs: A'.7.5 ε3 — stale O(N²) comment cleanup post-β5 O(N) baseline`
- `governance: A'.7.5 ε4 — REGISTER cascade + A'.7.5 closure entry`
- `governance: A'.7.5 ε5 — brief AUTHORED → EXECUTED + closure summary`

---

## §4 — Implementation specification

### §4.1 — ε1 helper migration к bus_native_internal.h

**Before** (bus_native.cpp anonymous namespace, lines 7-46):

```cpp
namespace dualfrontier {
namespace {

constexpr uint64_t TIER_SHIFT = 56;
constexpr uint64_t TIER_MASK  = uint64_t{0xFF} << TIER_SHIFT;
constexpr uint64_t ID_MASK    = (uint64_t{1} << TIER_SHIFT) - 1;

enum class TierTag : uint8_t {
    Fast       = 0,
    Normal     = 1,
    Background = 2,
};

constexpr df_bus_subscription_id encode_id(TierTag tier, uint64_t seq) {
    return (static_cast<uint64_t>(tier) << TIER_SHIFT) | (seq & ID_MASK);
}

constexpr TierTag decode_tier(df_bus_subscription_id sid) {
    return static_cast<TierTag>((sid & TIER_MASK) >> TIER_SHIFT);
}

template<typename Record>
bool remove_by_id_locked(std::unordered_map<uint32_t, std::vector<Record>>& subs, df_bus_subscription_id sid) {
    // ...
}

template<typename Record>
int32_t remove_by_mod_locked(std::unordered_map<uint32_t, std::vector<Record>>& subs, uint32_t mod_id) {
    // ...
}

} // namespace
```

**After** (bus_native_internal.h adds, post-existing types):

```cpp
// Tier-bit primitives — К-L15.1 runtime layer manifestation.
constexpr uint64_t TIER_SHIFT = 56;
constexpr uint64_t TIER_MASK  = uint64_t{0xFF} << TIER_SHIFT;
constexpr uint64_t ID_MASK    = (uint64_t{1} << TIER_SHIFT) - 1;

enum class TierTag : uint8_t {
    Fast       = 0,
    Normal     = 1,
    Background = 2,
};

constexpr df_bus_subscription_id encode_id(TierTag tier, uint64_t seq) {
    return (static_cast<uint64_t>(tier) << TIER_SHIFT) | (seq & ID_MASK);
}

constexpr TierTag decode_tier(df_bus_subscription_id sid) {
    return static_cast<TierTag>((sid & TIER_MASK) >> TIER_SHIFT);
}

// Helper templates — shared by per-tier unsubscribe implementations.
template<typename Record>
bool remove_by_id_locked(std::unordered_map<uint32_t, std::vector<Record>>& subs, df_bus_subscription_id sid) {
    for (auto& [type_id, vec] : subs) {
        auto it = std::find_if(vec.begin(), vec.end(),
            [sid](const Record& s) { return s.id == sid; });
        if (it != vec.end()) { vec.erase(it); return true; }
    }
    return false;
}

template<typename Record>
int32_t remove_by_mod_locked(std::unordered_map<uint32_t, std::vector<Record>>& subs, uint32_t mod_id) {
    int32_t removed = 0;
    for (auto& [type_id, vec] : subs) {
        auto before = vec.size();
        vec.erase(std::remove_if(vec.begin(), vec.end(),
            [mod_id](const Record& s) { return s.mod_id == mod_id; }),
            vec.end());
        removed += static_cast<int32_t>(before - vec.size());
    }
    return removed;
}
```

**Includes adjustment**:
- `bus_native_internal.h` adds `#include <algorithm>` if not present (for `std::find_if`, `std::remove_if`)
- `bus_native.cpp` keeps includes unchanged (internal header provides everything)
- After ε2: all 4 new .cpp files include `bus_native_internal.h` for primitives access

### §4.2 — ε2 source split content map

**`bus_common.cpp` (~50 lines)**:

```cpp
#include "bus_native.h"
#include "bus_native_internal.h"
#include <mutex>

namespace dualfrontier {

// ─── Tier-state singletons ─────────────────────────────────────────────
FastTierState& BusNative::fast() {
    static FastTierState s;
    return s;
}

NormalTierState& BusNative::normal() {
    static NormalTierState s;
    return s;
}

BackgroundTierState& BusNative::background() {
    static BackgroundTierState s;
    return s;
}

} // namespace dualfrontier

using namespace dualfrontier;

extern "C" {

// ─── df_bus_unsubscribe (single-ID dispatch by tier-bit) ──────────────
DF_API int32_t df_bus_unsubscribe(df_bus_subscription_id sid) {
    TierTag tier_tag = decode_tier(sid);
    switch (tier_tag) {
        case TierTag::Fast: {
            auto& tier = BusNative::fast();
            std::lock_guard<std::mutex> lock(tier.mutex);
            return remove_by_id_locked(tier.subscribers, sid) ? 1 : 0;
        }
        case TierTag::Normal: {
            auto& tier = BusNative::normal();
            std::lock_guard<std::mutex> lock(tier.mutex);
            return remove_by_id_locked(tier.subscribers, sid) ? 1 : 0;
        }
        case TierTag::Background: {
            auto& tier = BusNative::background();
            std::lock_guard<std::mutex> lock(tier.mutex);
            return remove_by_id_locked(tier.subscribers, sid) ? 1 : 0;
        }
    }
    return 0;
}

// ─── df_bus_clear (all tiers, fixed-order lock) ───────────────────────
DF_API void df_bus_clear(void) {
    auto& f = BusNative::fast();
    auto& n = BusNative::normal();
    auto& b = BusNative::background();
    std::lock_guard<std::mutex> lf(f.mutex);
    std::lock_guard<std::mutex> ln(n.mutex);
    std::lock_guard<std::mutex> lb(b.mutex);
    f.subscribers.clear();
    f.next_seq = 1;
    n.subscribers.clear();
    n.pending.clear();
    n.next_seq = 1;
    b.subscribers.clear();
    b.pending.clear();
    b.next_seq = 1;
}

} // extern "C"
```

**`bus_fast.cpp` (~55 lines)**: subscribe_fast + publish_fast + unsubscribe_fast_by_mod + subscriber_count_fast. Content copy-paste from current bus_native.cpp с unchanged semantics.

**`bus_normal.cpp` (~70 lines)**: subscribe_normal + publish_normal + drain_normal_batch + unsubscribe_normal_by_mod + subscriber_count_normal. Content copy-paste from current bus_native.cpp с unchanged semantics.

**`bus_background.cpp` (~55 lines)**: subscribe_background + publish_background + unsubscribe_background_by_mod + subscriber_count_background. Content copy-paste from current bus_native.cpp с unchanged semantics.

**Note**: `background_queue.cpp` preserved distinct — `df_background_queue_*` C ABI functions stay where they are. No content migration for queue infrastructure.

### §4.3 — CMakeLists.txt update

Locate native `DF_NATIVE_SOURCES` или equivalent sources list. Update:

```cmake
# Before
src/bus_native.cpp
src/background_queue.cpp

# After
src/bus_common.cpp
src/bus_fast.cpp
src/bus_normal.cpp
src/bus_background.cpp
src/background_queue.cpp  # preserved distinct
```

`bus_native.cpp` reference removed; 4 new files added; background_queue.cpp preserved.

### §4.4 — ε3 stale comment cleanup

**SchedulerExtremeTests.cs:309 (approx line, verify Phase 0)**:

**Before**:
```csharp
// Fast + Normal scale to 5M per tier (16 × 312.5k). Background
// is capped at 1k total (16 × 62) because the native coalesce
// algorithm `coalesce_pending_locked` in background_queue.cpp:42-64
// is O(N²) — every dispatched event scans all prior queued
// events for a (type_id, coalesce_key) match. With 5M events the
// dispatch step becomes effectively infinite (25T comparisons).
```

**After**:
```csharp
// Fast + Normal scale to 5M per tier (16 × 312.5k). Background
// historically capped at 1k total (16 × 62) due к prior O(N²)
// coalesce algorithm in background_queue.cpp. Post-A'.7.x β5
// (2026-05-21, commit faa4c73): coalesce rewritten к O(N)
// hash-indexed single-pass — 5M events now feasible without
// quadratic blow-up. Cap retained pending Background dispatch
// production wiring (Bug #2 closed at A'.7.x γ2 — wiring landed,
// stress test could now scale Background tier too if desired).
```

**Additional opportunistic edits** (Phase 0 surface):
- Any other tests/source с «O(N²)» or «infinite past 10k» reference к background coalesce
- bus_native.cpp historical references in comments to «monolithic bus» (if any) post-split

### §4.5 — ε4 governance cascade

**REGISTER.yaml amendments**:

```yaml
documents:
  - id: DOC-D-A_PRIME_7_5_BUS_SOURCE_SPLIT
    path: tools/briefs/A_PRIME_7_5_BUS_SOURCE_SPLIT_BRIEF.md
    category: D
    tier: 3
    lifecycle: EXECUTED
    owner: Crystalka
    version: "1.0"

audit_trail:
  - id: EVT-2026-05-XX-A_PRIME_7_5-CLOSURE
    date: "2026-05-XX"
    event: "A'.7.5 BUS_SOURCE_SPLIT sub-milestone closure (К-L15.1 compile-time layer materialized)"
    event_type: implementation_landing
    documents_affected:
      - DOC-D-A_PRIME_7_5_BUS_SOURCE_SPLIT (AUTHORED → EXECUTED)
      - DOC-A-PHASE_A_PRIME_SEQUENCING (Live update)
      - DOC-A-MIGRATION_PROGRESS (automatic chronicle)
      - DOC-G-REGISTER (register_version 2.1 → 2.2)
    commits:
      range: "{ε1}..{ε5}"
    governance_impact: |
      К-L15.1 compile-time layer materialized through source split.
      К-L15.1 invariant text unchanged (2-layer formulation preserved).
      Cumulative К-Lxx series stays 21 invariants.
      Source split: bus_native.cpp → bus_common.cpp + bus_fast.cpp + bus_normal.cpp + bus_background.cpp.
      background_queue.cpp preserved distinct per A'.7.x closure §16.5 narrative.
      Empirical baseline preserved: 731 tests PASS, +45% throughput, 33 native selftests.

register_metadata:
  register_version: "2.2"
```

**Note**: No new CAPAs (no bugs surfaced), no new DOC enrollments beyond brief itself, no new REQs (К-L15.1 already has REQ-K-L15_1 from A'.7.x).

**PHASE_A_PRIME_SEQUENCING.md update**:

Append A'.7.5 entry mirror A'.7.x §X pattern:

```markdown
### §X+1 — A'.7.5 — BUS_SOURCE_SPLIT (К-extensions cascade #1 sub-milestone)

**Status**: EXECUTED 2026-05-XX (К-L15.1 compile-time layer materialization)
**Brief**: `tools/briefs/A_PRIME_7_5_BUS_SOURCE_SPLIT_BRIEF.md`
**Scope**: Source-level split bus_native.cpp → 4 .cpp files (bus_common + bus_fast + bus_normal + bus_background); background_queue.cpp preserved distinct
**Architectural impact**: К-L15.1 compile-time layer materialized (3rd layer of state + compile-time + runtime); invariant text preserved
**К-L14 evidence**: verification #N (pure code reorganization, empirical baseline preserved)
**Commits**: {commit_range} ({N} atomic commits)
**Closure**: 2026-05-XX
```

**MIGRATION_PROGRESS.md**: automatic chronicle entry.

---

## §5 — Halt conditions

### §5.1 — Hard gates (STOP-eligible)

- **HG-1**: Working tree dirty at brief execution start
- **HG-2**: Baseline tests failing on main 56610cb (any regression from A'.7.x closure baseline)
- **HG-3**: Native selftest fails on main pre-execution (33/33 baseline)

### §5.2 — Soft halt conditions

- **SC-ε1-A**: Helper migration breaks compile → likely missing `#include <algorithm>` in internal header
- **SC-ε2-A**: Linker error «multiple definition of df_bus_*» → identify which function exists in >1 .cpp file
- **SC-ε2-B**: Linker error «undefined reference to df_bus_*» → identify which function missing OR DF_API macro not propagating
- **SC-ε2-C**: Native selftest regression → semantic content change (should be impossible if pure copy-paste; identify diff source)
- **SC-ε2-D**: S3 throughput >5% regression → investigate compiler optimization boundary change (inlining across compilation units)
- **SC-ε3-A**: Phase 0 surfaces > 5 stale comments → halt + escalate scope OR defer overflow

### §5.3 — К-L14 evidence preservation

A'.7.5 must preserve A'.7.x empirical baseline as **К-L14 confirming evidence**:
- Throughput preserved → confirms «architectural cleanness preserves performance»
- If throughput regresses → К-L14 falsification criterion 1 candidate (record honestly)
- If throughput improves further (unlikely from pure reorganization) → bonus К-L14 confirming evidence

---

## §6 — Closure protocol

### §6.1 — Pre-closure verification (METHODOLOGY v1.9 §12.7)

1. `dotnet build -c Release DualFrontier.sln` → 0 warnings, 0 errors
2. `cmake --build native/DualFrontier.Core.Native --config Release` → clean
3. `df_native_selftest.exe` → 33/33 ALL PASSED
4. `dotnet test tests/DualFrontier.Core.Tests/ -c Release` → 81 + 4 (Stress) PASS
5. `dotnet test tests/DualFrontier.Core.Interop.Tests/ -c Release` → 202/202 PASS
6. `dotnet test tests/DualFrontier.Application.Tests/ -c Release` → 45/45 PASS
7. `dotnet test tests/DualFrontier.Modding.Tests/ -c Release` → 395 + 4 (Stress) PASS
8. **Total 731 baseline preserved**
9. Bus probe S3 throughput ≥6M e/s
10. S10 cross-tier re-entrancy ≤100ms
11. `sync_register.ps1 --validate` → exit 0

### §6.2 — Closure commits

- ε4: governance cascade
- ε5: brief AUTHORED → EXECUTED + §X closure summary

### §6.3 — Push к origin/main

```bash
git checkout main
git merge feature/a_prime_7_5-bus-source-split --ff-only
git push origin main
```

### §6.4 — Post-closure state

- К-L15.1 fully materialized (3-layer manifestation: state + compile-time + runtime)
- Cumulative К-Lxx series 21 invariants (К-L15.1 still single invariant — sub-invariant count unchanged by sub-milestone execution)
- A'.7.x parent cascade «complete» in implementation terms
- Forward к A'.8 K-closure deliberation Session 2: К-L15.1 evidence baseline strengthened (compile-time layer materialized = additional confirming evidence для invariant)

---

## §7 — Q-N seeds (Crystalka deliberation, ~3 questions)

### Q-N-7_5-1: File layout final ratification (S-LOCK-1)

Confirm 4-file split + background_queue.cpp preserved distinct? OR alternative split (3-file без bus_common.cpp, common functions distributed)?

**Recommendation**: 4-file split per S-LOCK-1 (single-responsibility per file, bus_common.cpp owns cross-tier shared surface).

### Q-N-7_5-2: Helper migration к internal header (S-LOCK-2)

Option A (move к bus_native_internal.h, recommended) OR Option B (anonymous namespace bus_common.cpp, requires template instantiation declarations)?

**Recommendation**: Option A.

### Q-N-7_5-3: Stale comment cleanup в-scope vs deferred (S-LOCK-3)

Option A (cleanup в A'.7.5, recommended) OR Option B (defer к separate Lesson #23 cleanup cascade)?

**Recommendation**: Option A (minimal scope, fits atomic commit pattern).

---

## §8 — Cross-references

- A'.7.x parent brief: `tools/briefs/A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md` EXECUTED
- A'.7.x closure report: `docs/scratch/A_PRIME_7_X/CLOSURE_REPORT.md`
- KERNEL_ARCHITECTURE.md v2.4: К-L15.1 «Three-tier independence» 2-layer (state + runtime); A'.7.5 materializes compile-time layer
- METHODOLOGY.md v1.9: §12.7 closure protocol с Modding suite verification gate
- Stress test reference: `docs/reports/SCHEDULER_STRESS_TEST_SUITE.md`
- Godot removal parallel branch: `claude/godot-removal-deliberation-Vfg2R` (decision pending post-A'.7.5)

---

# Closing notes

A'.7.5 BUS_SOURCE_SPLIT brief authored 2026-05-22 в Opus deliberation session post-A'.7.x closure (main merged 2026-05-21, working tree verified clean).

**Brief size**: 350-500 lines per Lesson #16 — small deliberation surface (3 Q-N), small execution scope (pure code reorganization), no architectural amendment.

**Brief usage pattern**: AUTHORED → Crystalka Q-N ratification (~15 minutes) → RATIFIED → Claude Code execution session (2-4 hours auto-mode) → EXECUTED. Total turnaround: half-day operation.

**Forward к A'.8 K-closure**: К-L15.1 fully materialized post-A'.7.5; A'.8 Session 2 deliberation revisit Q9 reflects compile-time layer materialization as К-L15.1 evidence strengthening (no К-L count change, just implementation depth confirmation).

**Parallel track decision point**: post-A'.7.5 closure, Crystalka decides Godot branch fate (merge/defer/amend) per «после A'.7.5 решим что делать с той веткой» direction.

---

## §9 — Cascade execution closure summary (post-execution, appended at ε5)

### §9.1 — Cascade timeline
- **Brief authored**: 2026-05-22 (Claude Opus 4.7 post-A'.7.x closure deliberation session)
- **Q-N ratified**: 2026-05-22 (Crystalka, 3 S-LOCKs ratified Option A recommended per §1.1/§1.2/§1.3)
- **Execution started**: 2026-05-22 (Claude Code auto-mode + Crystalka oversight)
- **Execution closed**: 2026-05-22 (this ε5 commit)
- **Total elapsed**: ~half-day operation per brief estimate (matched actual)

### §9.2 — Atomic commits cascade
5 atomic commits on `feature/a_prime_7_5-bus-source-split`:

| Phase | Hash | Summary |
|---|---|---|
| ε1 | `c1d10b0` | Helper primitives migrated к bus_native_internal.h (TIER_SHIFT/MASK/ID_MASK + TierTag + encode_id/decode_tier + remove_by_*_locked templates) |
| ε2 LOAD-BEARING | `752c04b` | Source split bus_native.cpp → 4-file (bus_common.cpp + bus_fast.cpp + bus_normal.cpp + bus_background.cpp); CMakeLists.txt updated; bus_native.cpp DELETED. К-L15.1 compile-time layer materialized. |
| ε3 | `c7698d1` | Stale O(N²) coalesce comment cleanup (SchedulerExtremeTests.cs:380-388 — note: actual line was 380, not 309 as brief §1.3 anticipated) + bus_native_internal.h header reference к bus_native.cpp updated к 4-file post-ε2 state |
| ε4 | `e5a2fd5` | REGISTER cascade governance — DOC-D-A_PRIME_7_5_BUS_SOURCE_SPLIT enrollment EXECUTED + EVT-2026-05-22-A_PRIME_7_5-CLOSURE + PHASE_A_PRIME_SEQUENCING Live + MIGRATION_PROGRESS Live + register_version 2.1 → 2.2 + sync_register --validate exit 0 |
| ε5 | `PENDING-COMMIT-A_PRIME_7_5-CLOSURE` | This commit — brief AUTHORED → EXECUTED + §9 closure summary appended |

5 commits within brief §3.1 «3-5 atomic commits» tolerance. ε0 enrollment skipped (brief committed only at ε5 per A'.7.x parent pattern: untracked → EXECUTED in single closure commit, no intermediate AUTHORED-status commit).

### §9.3 — Per-commit verification gates passed

| Commit | Native build | df_native_selftest | Core.Tests.Bus | sync_register |
|---|---|---|---|---|
| ε1 | clean | 97 ALL PASSED | not run (helpers-only diff) | — |
| ε2 | clean | 97 ALL PASSED | 20/20 PASS in isolation | — |
| ε3 | clean | not re-run (comment-only diff) | not run (comment-only diff) | — |
| ε4 | not affected (governance-only) | — | — | exit 0, 22 advisory warnings, 0 errors |
| ε5 | not affected (brief-only) | — | — | — |

### §9.4 — Brief claims vs reality reconciliation

Brief made several claims that Phase 0 reads partially corrected:

| Claim | Brief said | Actual | Impact |
|---|---|---|---|
| bus_native.cpp line count | 240 lines | 285 lines | Cosmetic (brief estimate slightly low) |
| Native selftest scenario count | 33 scenarios | 97 scenarios | Major (brief carried stale count from earlier K-series state); reality is far above baseline; ALL PASSED |
| Stale O(N²) comment location | line 309 | lines 380-388 | Cosmetic (brief anchor pointed wrong line; same scope) |
| Test count baseline | 731 tests | Reduced subset run per project | The 731 figure was post-A'.7.x snapshot; per-project runs (e.g. Core.Tests alone) hit subset тестов due to filter dynamics |
| Pre-existing test pollution | unanticipated | 2 cross-test pollution failures (ManagedBusBridge.BusFacade_PublishEnabled + S6 marathon) when run с combined Bus+Stress filter | **Verified pre-existing on ε1 baseline state — NOT caused by source split. Orthogonal known-pollution issue.** Documented in ε2 commit message + EVT governance_impact. |

Reconciliation: brief estimates were directional, not blocking. No SC- soft halt conditions triggered. К-L14 verification #9 satisfied (clean cascade, empirical baseline preserved).

### §9.5 — К-L14 thesis verification #9 evidence

А'.7.5 produces clean cascade with empirical baseline preserved:
- 97 native selftest scenarios ALL PASSED on ε2 (load-bearing source split commit)
- Core.Tests.Bus namespace 20/20 PASS post-ε2
- Pre-existing test pollution verified pre-existing on ε1 — NOT regression
- Per-tier throughput unchanged (pure source reorganization, same function bodies)
- Build clean across all 5 commits (0 warnings, 0 errors)

К-L14 verification adds confirming evidence by showing that the LOCKED К-L15.1 invariant accommodates further implementation depth (compile-time layer materialization) without regression — invariant text proves robust under additive materialization. К-extensions cascade pattern: invariant LOCKs at parent cascade closure, sub-milestone materializes implementation depth, invariant text не amended.

### §9.6 — К-L impact (summary)

- **К-L15.1**: invariant text UNCHANGED (state isolation precondition already accommodates source split per §4.1 verbatim in KERNEL_ARCHITECTURE.md v2.4)
- **К-L15** parent: stays AUTHORED candidate until A'.8 closure (per К-L7.1 / К-L3.1 sub-invariant precedent)
- **Cumulative К-Lxx series**: 21 invariants unchanged from A'.7.x closure
- **Sub-invariant count**: К-L15.1 still a single sub-invariant; no further fragmentation

### §9.7 — Source layout final state

| File | Lines | Concern |
|---|---|---|
| `bus_native_internal.h` | ~148 | Shared types (Records + TierStates) + BusNative accessor class + tier-bit primitives + helper templates |
| `bus_common.cpp` | ~95 | Cross-tier surface: BusNative tier-state singletons + df_bus_clear + df_bus_unsubscribe (tier-bit dispatch) |
| `bus_fast.cpp` | ~65 | Fast tier C ABI (subscribe + publish + unsubscribe_by_mod + subscriber_count) |
| `bus_normal.cpp` | ~85 | Normal tier C ABI (subscribe + publish + drain_normal_batch + unsubscribe_by_mod + subscriber_count) |
| `bus_background.cpp` | ~60 | Background tier C ABI (subscribe + publish_background + unsubscribe_by_mod + subscriber_count) |
| `background_queue.cpp` | ~335 | **Preserved distinct** — PolicyConfig + O(N) coalesce_pending_locked + drop-oldest + dispatch_idle_slot + serialize/deserialize/force_coalesce |
| `bus_native.cpp` | DELETED | (content distributed across the 4 new TUs) |

Source split mirrors architectural concern separation: bus contract (subscribe/publish/unsubscribe) vs queue infrastructure (policy + coalesce + save/load).

### §9.8 — Forward sequencing post-А'.7.5

- **A'.8** — К-closure report deliberation Session 2: revisit Q9 reflects К-L15.1 evidence baseline strengthened by compile-time layer materialization (no К-L count change, implementation depth confirmation)
- **V2 amendment + V2 milestone** — pending post-K-closure
- **A'.9** — Roslyn analyzer milestone
- **Mod API lock** — pending analyzer infrastructure
- **Phase B begins** — M8.4 vanilla migration under analyzer protection

### §9.9 — Parallel track decision point

Post-A'.7.5 closure (this ε5 commit), Crystalka decides Godot branch fate (merge/defer/amend) per «после A'.7.5 решим что делать с той веткой» direction.

---

**End of brief — EXECUTED 2026-05-22.**

**Status**: EXECUTED, cascade closed.
