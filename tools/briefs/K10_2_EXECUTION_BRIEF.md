---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K10_2
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K10_2
---
---
# Brief frontmatter (not REGISTER mirror — brief lives in tools/briefs/ as Tier 3 Category D)
brief_id: K10_2_EXECUTION_BRIEF
status: AUTHORED
authored: 2026-05-18
author: Claude Opus 4.7 (Crystalka deliberation session)
target_executor: Claude Code (auto-mode + Crystalka oversight)
estimated_duration: 14-22 hours auto-mode (К10.2 scope substantial — native bus three-tier + mod ALC lifecycle native primitives)
brief_type: execution
authority_chain:
  - KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED (DOC-A-KERNEL_FULL_NATIVE_SCHEDULER) — primary spec для К10.2 scope (Items 21, 26-32)
  - K10_DELIBERATION_STATE.md (Project file sister — 9 S-locks rationale, particularly S1 native bus three-tier + S3 mod ALC lifecycle)
  - KERNEL_ARCHITECTURE.md v2.0 LOCKED (post-К10.1) — К-L1..L11 + К-L6 SUPERSEDED + К-L12/L13/L14 baseline; К10.2 amends к v2.1 adding К-L15
  - MOD_OS_ARCHITECTURE.md v1.8 LOCKED — §9.5 unload chain 7-step verbatim + §3.2 capability syntax + §4.6 IModApi v3 strict; К10.2 amends к v1.9
  - METHODOLOGY.md v1.8 LOCKED — Lessons #7/#8/#11/#20/#22 verbatim + Provisional Lessons + §12.7 canonical closure protocol
  - FRAMEWORK.md v1.1 LOCKED — Category D Tier 3 lifecycle transitions
  - K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md EXECUTED (DOC-D-K8_34_COMBINED_V2, 2026-05-14) — atomic-cutover precedent + halt taxonomy
  - K10_1_EXECUTION_BRIEF.md EXECUTED (DOC-D-K10_1, 2026-05-18) — К10.1 closure precedent + managed-facade-preserved pattern + selftest infrastructure pattern + DF_CHECK runner convention
---

# K10.2 — Native bus three-tier dispatch + mod ALC lifecycle native primitives

**Brief shape**: Execution-mode brief targeting Claude Code auto-mode с Crystalka oversight. Multi-commit atomic cascade implementing **8 architectural items** from KERNEL_FULL_NATIVE_SCHEDULER.md spec (Items 21, 26-32) that ratify **К-L15 native bus authority + three-tier event dispatch** + integrate per-mod sub-scheduler teardown native primitive.

**Authority**: Direct execution against К10 deliberation arc closure (2026-05-17). К10.2 is the **second of four К10 sub-milestones** under Option III standalone-briefs structure ratified 2026-05-18 (К10.1 closed 2026-05-18 PR merged; К10.2 = native bus + mod ALC; К10.3 = pipeline depth + display + hardware tier, future brief; К10.4 = TLA+ formal verification, future brief).

**К10.1 closure context inherited** (per session log):
- 624 baseline preserved (620 pre-К10.1 + 4 BatchedCallback tests new)
- К-L6 SUPERSEDED landed (KERNEL_ARCHITECTURE.md v1.6 → v2.0)
- К-L12/L13/L14 AUTHORED в same amendment
- К-L6 supersession executed как **managed-facade-preserved strategy** (Lesson #8 atomic intermediate stability): native scheduler infrastructure landed parallel к managed dispatch; native graph registers Core systems for parallel verification, managed dispatch remains hot-path authority
- Native test convention: **K3-era custom DF_CHECK runner** (`selftest.cpp` pattern), not catch2/gtest — Lesson #22 «match existing convention» applied
- Operational reminder: VS-bundled CMake at `D:\Visual Studio\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe` (system cmake не в PATH)
- К10.1 selftest scenarios: 58 (28 ECS baseline + 30 К10.1 scheduler) ALL PASSED

**К10.2 ratifies architecturally** (per К10 deliberation closure):
- **К-L15 AUTHORED** (Items 26-30): Native bus authority + three-tier event dispatch (fast / normal / background)
- Item 21: Mod scheduler authority (per-mod sub-schedulers) — К10.2 lands teardown native primitive shape
- Items 31-32: Background queue save-integrated storage + native unload primitive (`df_scheduler_unload_mod_native_state` + `ModUnloadResult`)

**К10.2 scope discipline (Lesson #20 + Lesson #14 candidate application)**:
- In-scope: 8 К10 items establishing native bus + mod ALC lifecycle native primitives (per spec §3.8-3.9)
- Out-of-scope: К10.3 (pipeline depth + display + hardware) — separate brief
- Out-of-scope: К10.4 (TLA+ formal verification) — separate brief
- Out-of-scope: Item 14 (К11+ Core migration) — explicitly deferred per spec §3.4
- Out-of-scope: Item 25 (К-closure report) — cross-cutting к А'.8

**Strategic pattern inherited from К10.1**: К10.2 native bus implementation lands as **managed-facade-preserved** (per К10.1 Commit 14 strategy + Lesson #8). Managed bus continues to operate authoritatively; native bus parallel infrastructure landed; К-L15 invariant text lands и amendments propagate. Actual sovereign authority switch для bus events deferred к К10.4 closure или К-closure report (А'.8) when full К10 infrastructure complete + integration tests verified.

---

## §1 — Crystalka ratified scope locks (К10 deliberation 2026-05-16..2026-05-17, applicable subset для К10.2)

### §1.1 — S-LOCK-1: К10.2 scope items (8 items)

**LOCK**: К10.2 implements exactly these 8 items from KERNEL_FULL_NATIVE_SCHEDULER.md spec, in dependency order:

| Item | Title | Spec § | Group |
|---|---|---|---|
| 26 | Native bus implementation (three-tier dispatcher) | §3.8 | Native bus |
| 27 | Managed bus facade + C ABI bridge | §3.8 | Native bus |
| 28 | Event type registry (tier-annotated) | §3.8 | Native bus |
| 29 | Subscriber contract enforcement (per-tier validation) | §3.8 | Native bus |
| 30 | Background work queue + idle-slot scheduling | §3.8 | Native bus |
| 31 | Background queue save-integrated storage (S3-Q3) | §3.9 | Mod ALC lifecycle |
| 32 | Native unload primitive + ModUnloadResult (S3-Q1/Q6) | §3.9 | Mod ALC lifecycle |
| 21 | Mod scheduler authority (per-mod sub-schedulers) | §3.6 | Mod integration |

Item 21 located в §3.6 spec но belongs logically к К10.2 because S3 lock pattern combines mod scheduler authority с per-mod native teardown — Item 32 native unload primitive encapsulates Item 21 teardown logic per S3-Q1 L3 layering. Item 21 placed last в cascade because it consumes Items 26-32 infrastructure.

### §1.2 — S-LOCK-2: К-L15 invariant landing

**LOCK**: К10.2 lands **К-L15** in KERNEL_ARCHITECTURE.md v2.0 → v2.1. К-L15 text (verbatim from spec Part 2):

> «Native kernel owns sovereign event routing for kernel-space and cross-layer events. Bus implementation native: type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics all native authority. Bus supports three-tier dispatch (fast / normal / background) with tier declared per event type. Managed bus facade preserved as IModApi-exposed surface (К-L9 uniformity); implementation routes through C ABI bridge к native bus. Fast/background event publish/subscribe requires per-FQN per-tier capability declarations. Cross-layer event delivery uses batched callback ABI (normal/background tiers) or immediate synchronous callback (fast tier).»

К-L15 row inserted в KERNEL_ARCHITECTURE.md Part 0 К-L invariants table after К-L14 (added в К10.1).

К-L16, К-L17, К-L18, К-L19, К-L7.1 (sub-invariant) are **К10.3 scope**, not К10.2.

### §1.3 — S-LOCK-3: Three-tier dispatch semantics

**LOCK** (per К-L15 + spec Part 2.4 / S1 lock):

| Tier | Latency | Dispatch path | Subscriber contract | Use cases |
|---|---|---|---|---|
| **Fast** | Immediate (≤1ms target) | Synchronous bypass, preemption-aware | Bounded exec, no blocking, no GC alloc, RT class | Combat hits, player input, emergency |
| **Normal** | Within tick | Batched callback per-phase | Standard | AI signals, lifecycle, bulk |
| **Background** | Multi-tick acceptable (idle slots) | Coalesce + idle-slot dispatch | Long-running ok, interruptible | Off-screen simulation, climate, quest gen |

Tier declared per event type at registration time.

### §1.4 — S-LOCK-4: Per-FQN per-tier capability tokens (S3-Q5)

**LOCK** (per S3-Q5 ratified 2026-05-17): Capability tokens granular per FQN per tier per action:

```
kernel.fast.publish:{FQN}
kernel.fast.subscribe:{FQN}
kernel.normal.publish:{FQN}     ← preserves current kernel.publish:{FQN} semantics
kernel.normal.subscribe:{FQN}   ← preserves current kernel.subscribe:{FQN} semantics
kernel.background.publish:{FQN}
kernel.background.subscribe:{FQN}
```

Backward compatibility per spec §3 + MOD_OS §3.2: existing `kernel.publish:{FQN}` / `kernel.subscribe:{FQN}` tokens **continue to work** and map к Normal tier dispatch automatically (default tier when not specified). New tier-explicit tokens activate Fast/Background tier dispatch.

ContractValidator 8-phase pipeline (existing) extended к recognize tier-prefixed tokens. KernelCapabilityRegistry scanning extension consumed at static cross-check (§3.7 MOD_OS).

### §1.5 — S-LOCK-5: Atomic cascade (multi-commit ordered) + managed-facade-preserved

**LOCK**: К10.2 executes as multi-commit cascade analogous к К10.1 pattern. Per Lesson #8: К10.2 items 26-32 have **clean intermediate states** because:
1. Native bus infrastructure (Items 26, 28, 30) lands additive — managed bus continues to operate
2. Managed bus facade + C ABI bridge (Item 27) lands як integration layer — managed bus still authoritative для actual dispatch
3. Subscriber contract enforcement (Item 29) lands at load-time + runtime gates parallel к existing capability checks
4. Background queue save integration (Item 31) extends DualFrontier.Persistence
5. Native unload primitive (Item 32) wired into ModIntegrationPipeline.UnloadMod Step 3.5
6. Mod scheduler authority (Item 21) lands as teardown primitive consumption pattern
7. К-L15 invariant text lands в KERNEL_ARCHITECTURE.md v2.1 + MOD_OS amendments at closure

Tests pass at every commit (existing 624 К10.1 baseline preserved; new К10.2 tests additive).

Contrast с К10.1: К10.1 had **one load-bearing commit** (Commit 14 К-L6 supersession + Core dispatch switch). К10.2 has **distributed К-L15 landing** — invariant text + amendment commit late в cascade, but no single "binary switch" commit because three-tier dispatch implementation is composable (each tier independent path).

### §1.6 — S-LOCK-6: К10.2 closes К10.2 only

**LOCK**: К10.2 closure is sub-milestone closure (per FRAMEWORK §3.3 Lifecycle EXECUTED для К10.2 brief). К-closure report (А'.8) waits для all four К10 sub-milestones closed.

**Implications**:
- К10.2 commit message scope: `feat(kernel): K10.2 — native bus three-tier + mod ALC lifecycle (К-L15)`
- К10.2 closure REGISTER entry: separate audit_trail event (`EVT-{date}-K10_2-CLOSURE`)
- К-L invariant landings happen incrementally: К10.1 landed К-L6 SUPERSEDED + К-L12/13/14; К10.2 lands К-L15; К10.3 lands К-L7.1/L16/L17/L18/L19; К10.4 has no new К-L

---

## §2 — Phase 0 — Pre-flight reads (mandatory before any edit)

Per Lesson #7 (transcribe API verbatim) + Lesson #22 (read existing code before mechanism design), executor MUST complete every read listed below **before writing a single line of К10.2 code**. К10.2 brief authored 2026-05-18 from point-in-time read; drift between brief authoring and execution time is **expected**, surfaces as halt triggers (SC-3) — never silent improvisation per Lesson #20.

### §2.1 — Verify post-К10.1 closure state (hard gates)

Read and verify (these are **hard gates**, blocking commits если failed):

1. `git log --oneline -20` on `main` — confirm:
   - К10.1 PR merged (16 commits `f439b74..21a732d` per К10.1 closure report)
   - HEAD references К10.1 closure commit
   - Halt если К10.1 closure не reached — К10.2 starts from post-К10.1 main

2. `git status` — working tree clean before execution starts. **Hard gate** per К8.34 v2.0 + К10.1 precedent SC-1.

3. `docs/governance/REGISTER.yaml` head check — confirm `register_version` ≥ "1.9" (post-К10.1 closure baseline). К10.1 closure set 1.9; lower means К10.2 not based on correct baseline.

4. `tools/governance/sync_register.ps1 --validate` — exit 0 required as baseline. Если validation fails before execution, halt и surface per К10.1 precedent SC-7.

5. `dotnet build DualFrontier.sln` — clean baseline. 0 warnings, 0 errors.

6. `dotnet test DualFrontier.sln` — baseline pass count: **624 tests green** (per К10.1 closure metrics: 620 pre-К10.1 + 4 BatchedCallback tests new в К10.1). Если suite fails or count diverges (excluding intentional К10.2 additions), halt.

7. `cmake --build native/DualFrontier.Core.Native` через VS-bundled cmake (`D:\Visual Studio\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe` per К10.1 operational finding) — clean baseline. Native selftest passes 58 scenarios (28 ECS + 30 К10.1 scheduler).

### §2.2 — Read KERNEL_FULL_NATIVE_SCHEDULER.md spec as ground truth

Read in full, identify exact line numbers для К10.2 items:
- `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v2.0 LOCKED:
  - Part 2.4 К-L15 verbatim text (line ~225-235)
  - Part 3.6 Item 21 (mod scheduler authority + teardown primitive S3 lock)
  - Part 3.8 Items 26-30 (native bus three-tier)
  - Part 3.9 Items 31-32 (mod ALC lifecycle native primitives)
  - Part 5.1.A Predictions 6-10 (К10.2 measurable scope)
  - Part 8 risks R-K10-6 (background queue save compatibility versioning)

**Per Lesson #7**: spec verbatim is **authoritative для К-L15 invariant text**, three-tier dispatch semantics, capability token format, ModUnloadResult struct shape.

### §2.3 — Read code anchors verbatim (К10.2 specific)

Read these files для verbatim content к understand the migration shape (Lesson #22):

**Managed bus + capability layer (current state)**:
- `src/DualFrontier.Core/Bus/` — current managed bus implementation (publishers, subscribers, dispatch logic)
- `src/DualFrontier.Application/Modding/RestrictedModApi.cs` — `Publish<T>` / `Subscribe<T>` implementation (capability enforcement per §4.2 + §4.3 MOD_OS)
- `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs` — capability registry; К10.2 extension for per-FQN per-tier token generation
- `src/DualFrontier.Application/Modding/ContractValidator.cs` — 8-phase validation pipeline (MissingCapability check, Path A et al.); К10.2 extension для tier-aware validation

**Mod integration pipeline + unload chain**:
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` — `UnloadMod` 7-step chain per MOD_OS §9.5; К10.2 inserts Step 3.5 calling `df_scheduler_unload_mod_native_state`
- `src/DualFrontier.Application/Modding/ModLoader.cs` — ALC unload + WeakReference spin loop
- `src/DualFrontier.Application/Modding/ModRegistry.cs` — RemoveSystems implementation
- `src/DualFrontier.Application/Modding/RestrictedModApi.cs` — UnsubscribeAll implementation (Step 1)

**Native side (post-К10.1)**:
- `native/DualFrontier.Core.Native/include/df_capi.h` — C ABI surface (post-К10.1 extended с scheduler primitives); К10.2 adds bus + unload C ABI
- `native/DualFrontier.Core.Native/include/wake_registry.h` — К10.1 wake registry; К10.2 extends с EventWake dispatch when К-L15 bus publishes
- `native/DualFrontier.Core.Native/include/managed_callback.h` — К10.1 batched callback ABI (Item 15); К10.2 consumes для bus callbacks
- `native/DualFrontier.Core.Native/test/selftest.cpp` — К3-era DF_CHECK runner pattern (Lesson #22 match existing convention)

**Persistence (К10.2 background queue save integration)**:
- `src/DualFrontier.Persistence/` — current save/load infrastructure; К10.2 extends с background queue serialization (Item 31)
- Save file format documentation если exists

**Test fixtures (post-К10.1)**:
- `tests/DualFrontier.Core.Tests/` — existing scheduler tests (including new К10.1 BatchedCallback tests)
- `tests/DualFrontier.Modding.Tests/` — mod lifecycle tests, capability tests, unload tests; К10.2 extends с tier-prefixed token tests + Step 3.5 integration tests
- `tests/DualFrontier.Application.Tests/Modding/` — ModIntegrationPipeline integration tests

**Если any read surfaces a contradiction с this brief** — halt per SC-3.

### §2.4 — Read MOD_OS_ARCHITECTURE.md §9.5 + §3.2 + §11.2

Read in full:
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.8 LOCKED:
  - §3.2 capability syntax (verb extension surface)
  - §3.5 D-1 LOCKED + path orthogonality (capability pattern symmetry)
  - §3.7 Cross-check c [SystemAccess] (К10.2 extends к include tier-aware tokens)
  - §4.6 IModApi v3 strict + sub-API patterns (Fields + ComputePipelines precedent для bus-tier extension)
  - §9.5 unload chain 7 steps verbatim (Step 3.5 insertion point — between Step 3 RemoveSystems и Step 4 graph rebuild)
  - §9.5.1 Failure semantics (best-effort sequential discipline preserved)
  - §11.2 ValidationErrorKind enum (К10.2 adds new kinds)

**Per Lesson #7**: MOD_OS §9.5 7-step chain text is **authoritative**. Step 3.5 insertion text must match precisely existing chain wording style.

### §2.5 — Read REGISTER.yaml К10.2 enrollment area

Identify exact line ranges:
- DOC-A-KERNEL (КERNEL_ARCHITECTURE.md v2.0 post-К10.1) — К10.2 amends к v2.1
- DOC-A-MOD_OS (MOD_OS_ARCHITECTURE.md v1.8) — К10.2 amends к v1.9
- DOC-A-KERNEL_FULL_NATIVE_SCHEDULER (К10 spec v2.0 LOCKED) — governance_events append for К10.2
- DOC-D-K10_1 (К10.1 brief EXECUTED) — К10.2 brief enrollment after this entry
- Audit_trail events list (К10.2 adds EVT-{date}-K10_2-CLOSURE)

### §2.6 — Halt category clarity

**Hard gates (STOP-eligible)** per §2.1 + К10.1 precedent:
- Working tree dirty
- Baseline tests failing (excluding intentional changes)
- `sync_register.ps1 --validate` non-zero baseline
- Build failure baseline
- К10.1 closure не reached

**Informational checks (record-only)**:
- HEAD SHAs, branch topology
- Document version states (К-L invariants count, etc.)
- File layout details (Phase 0.4 inventory as hypothesis, not authority)

Если informational check diverges from brief expectation — **record divergence в Commit message, continue**. Hard gate failure → halt per SC-N (§5).

---

## §3 — Atomic commit cascade (target ~13-15 commits)

Each commit: individually compilable + register-valid intermediate state per Lesson #8. `sync_register.ps1 --validate` exit 0 at every governance-touching commit; `dotnet build` clean at every code-touching commit; `dotnet test` 624+ passing at every commit (К10.1 baseline preserved; new К10.2 tests additive).

**Cascade strategy** per S-LOCK-5 + К10.1 precedent: К10.2 native bus infrastructure lands as **additive parallel** к existing managed bus. Managed bus continues to operate как dispatch authority during К10.2 cascade. К-L15 invariant text lands late в cascade (Commit 13) along with KERNEL_ARCHITECTURE.md v2.0 → v2.1 + MOD_OS_ARCHITECTURE.md v1.8 → v1.9 amendments. Actual sovereign authority switch для bus events deferred к К10.4 closure (analogous к К10.1 managed-facade-preserved pattern).

### Commit 1 — Brief authoring commit (К10.2 brief enrollment)

**Files**:
- `tools/briefs/K10_2_EXECUTION_BRIEF.md` (this brief)
- `docs/governance/REGISTER.yaml` (DOC-D-K10_2 entry с `lifecycle: AUTHORED`, `category: D`, `tier: 3`)

**Rationale** per «brief authoring as prerequisite step» Lesson (K10.1 precedent): brief lives on `main` BEFORE К10.2 feature branch creates.

**Validation**:
- `sync_register.ps1 --validate` exit 0
- No code changes; no test impact

**Commit message**: `docs(briefs): K10.2 brief authored — native bus three-tier dispatch + mod ALC lifecycle (К-L15)`

### Commit 2 — Phase 0 verification + native test infrastructure preparation

**Files**:
- `native/DualFrontier.Core.Native/test/selftest.cpp` (extended с К10.2 placeholder section — empty section ready для new test scenarios added in subsequent commits)
- Build cmake-bundled toolchain verified per К10.1 operational finding

**Rationale per Lesson #22**: Phase 0 confirms К3-era DF_CHECK runner pattern preserved from К10.1. New native test sections added incrementally per Item-commit pairing rather than new test framework adoption.

**Validation**:
- `cmake --build` clean
- `cmake --build --target run_native_tests` — 58 К10.1 scenarios pass (К10.2 placeholder section returns success vacuously)
- `dotnet build` clean
- `dotnet test` 624+ green (К10.1 baseline preserved)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(native-test): K10.2 — native test scaffold preserved (DF_CHECK runner extension)`

### Commit 3 — Item 28: Event type registry (tier-annotated)

**Files**:
- `native/DualFrontier.Core.Native/include/event_type_registry.h` (new)
- `native/DualFrontier.Core.Native/src/event_type_registry.cpp` (new)
- `native/DualFrontier.Core.Native/test/event_type_registry_test.cpp` (new selftest section)
- `src/DualFrontier.Core.Interop/EventTypeRegistryInterop.cs` (new managed binding)
- `src/DualFrontier.Contracts/Bus/EventTierAttribute.cs` (new — `[EventTier(BusTier)]` attribute)

**Drift surface**: New native data structure for per-event-type metadata (tier + payload size constraint + coalesce function + capability token format). Managed bus continues to dispatch existing events; native registry is read-only via interop initially.

**Implementation surface (per spec §3.8 Item 28)**:

```c
// event_type_registry.h
typedef enum {
    BusTier_Fast = 0,
    BusTier_Normal = 1,
    BusTier_Background = 2
} BusTier;

typedef struct {
    uint32_t type_id;
    BusTier tier;
    uint32_t payload_size_bytes;
    const char* fqn;  // for capability token construction
    // Coalesce function pointer для Background tier (NULL для Fast/Normal)
    void (*coalesce_fn)(void* existing, const void* new_event);
} EventTypeMetadata;

int32_t df_event_type_registry_register(
    uint32_t type_id,
    BusTier tier,
    uint32_t payload_size_bytes,
    const char* fqn,
    void (*coalesce_fn)(void*, const void*));

int32_t df_event_type_registry_lookup(uint32_t type_id, EventTypeMetadata* out_metadata);
int32_t df_event_type_registry_get_tier(uint32_t type_id, BusTier* out_tier);
```

```csharp
// EventTierAttribute.cs
public enum BusTier { Fast = 0, Normal = 1, Background = 2 }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class EventTierAttribute : Attribute
{
    public BusTier Tier { get; }
    public int PayloadSizeBytesHint { get; }
    public EventTierAttribute(BusTier tier, int payloadSizeBytesHint = 0)
    {
        Tier = tier;
        PayloadSizeBytesHint = payloadSizeBytesHint;
    }
}
```

**Default tier semantics** (per S-LOCK-4 backward compatibility): events без `[EventTier]` annotation default к `BusTier.Normal`. Existing events continue к work с Normal tier dispatch — no migration required для third-party mods.

**Per Lesson #7**: spec §3.8 Item 28 verbatim authoritative. EventTypeMetadata struct shape preserved literally.

**Validation**:
- `cmake --build` clean
- `event_type_registry_test`: register/lookup correctness, tier assignment, payload size handling, coalesce function pointer round-trip
- `dotnet build` clean
- `dotnet test` 624+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.2 Item 28 — event type registry (tier-annotated metadata)`

### Commit 4 — Item 26: Native bus implementation (three-tier dispatcher)

**Files**:
- `native/DualFrontier.Core.Native/include/bus_native.h` (new)
- `native/DualFrontier.Core.Native/src/bus_native.cpp` (new — three subscriber registries + per-tier dispatch paths)
- `native/DualFrontier.Core.Native/test/bus_native_test.cpp` (new selftest section)
- `src/DualFrontier.Core.Interop/BusNativeInterop.cs` (new managed binding)

**Drift surface**: New native bus infrastructure. Managed bus continues к dispatch — native bus runs parallel infrastructure during К10.2.

**Implementation surface (per spec §3.8 Item 26 verbatim)**:

```c
// bus_native.h
// Per-tier subscriber registries
typedef void (*FastSubscriberCallback)(uint32_t type_id, const void* payload, uint32_t payload_size);
typedef void (*BatchedSubscriberCallback)(const ManagedSystemBatch* batch /* defined in managed_callback.h */);

// Three dispatch paths:
// Fast — synchronous bypass, immediate dispatch
int32_t df_bus_publish_fast(uint32_t type_id, const void* payload, uint32_t payload_size);
// Normal — batched, dispatched per-phase via batched callback ABI
int32_t df_bus_publish_normal(uint32_t type_id, const void* payload, uint32_t payload_size);
// Background — coalesce + idle-slot dispatch
int32_t df_bus_publish_background(uint32_t type_id, const void* payload, uint32_t payload_size, uint32_t coalesce_key);

// Subscription
int32_t df_bus_subscribe_fast(uint32_t type_id, FastSubscriberCallback callback, void* context);
int32_t df_bus_subscribe_normal(uint32_t type_id, BatchedSubscriberCallback callback, void* context);
int32_t df_bus_subscribe_background(uint32_t type_id, BatchedSubscriberCallback callback, void* context);

// Unsubscribe (по mod_id для bulk operations during unload)
int32_t df_bus_unsubscribe_fast_by_mod(uint32_t mod_id);
int32_t df_bus_unsubscribe_normal_by_mod(uint32_t mod_id);
int32_t df_bus_unsubscribe_background_by_mod(uint32_t mod_id);
```

**Dispatch path semantics** (per К-L15 verbatim):
- **Fast tier**: synchronous bypass, preemption-aware. Publisher's thread invokes subscriber callback directly. Bounded execution required (≤1ms per К-L15 target). No batching overhead.
- **Normal tier**: batched callback per-phase. Events queued; dispatched at phase boundary via Item 15 batched callback ABI (К10.1). К-L7 atomic-from-observer preserved within batch.
- **Background tier**: coalesce + idle-slot. Per-type coalesce function applied at publish time (per Item 30 idle-slot scheduling). Multi-tick spread acceptable.

**Native authority** (per К-L15): type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics all native. Managed bus facade (Item 27, next commit) routes через C ABI к native.

**Validation**:
- `cmake --build` clean
- `bus_native_test`: 
  - Fast tier publish→subscribe round-trip с timing assertion (≤1ms)
  - Normal tier publish→batch→subscribe round-trip
  - Background tier publish→coalesce→idle-slot dispatch
  - Per-mod unsubscribe bulk
- `dotnet build` clean
- `dotnet test` 624+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.2 Item 26 — native bus three-tier dispatcher (fast/normal/background)`

### Commit 5 — Item 30: Background work queue + idle-slot scheduling

**Files**:
- `native/DualFrontier.Core.Native/include/background_queue.h` (new)
- `native/DualFrontier.Core.Native/src/background_queue.cpp` (new — coalesce-on-publish + idle-slot detection + saturation handling)
- `native/DualFrontier.Core.Native/test/background_queue_test.cpp` (new selftest section)

**Drift surface**: Background tier implementation detail. Coalesce'инг reduces work; idle-slot scheduling integrates с К10.1 scheduler.

**Implementation surface (per spec §3.8 Item 30 + Q-N-34/35/36)**:

```c
// background_queue.h
typedef struct {
    uint32_t type_id;
    uint32_t coalesce_key;
    void* payload;
    uint32_t payload_size;
} BackgroundEvent;

// Coalesce function declared at event type registration (Item 28)
// На publish: existing event с same (type_id, coalesce_key) coalesced via coalesce_fn
int32_t df_background_queue_publish(uint32_t type_id, const void* payload, uint32_t payload_size, uint32_t coalesce_key);

// Idle-slot detection: scheduler queries available CPU budget (Q-N-35)
// Default budget: scheduler CPU time slot at phase end before tick boundary
// Configurable per-mod budget shares (Q-N-35 deferred к brief authoring — К10.2 default: shared budget pool)
int32_t df_background_queue_dispatch_idle_slot(uint64_t available_budget_micros);

// Saturation handling (Q-N-36 deferred к brief authoring — К10.2 default: drop oldest с warning)
int32_t df_background_queue_get_size(uint32_t* out_event_count, uint32_t* out_bytes_used);
int32_t df_background_queue_set_saturation_strategy(int strategy_type /* 0=drop_oldest, 1=backpressure, 2=expand */);
```

**К10.2 defaults** (per К-L14 default-inclusion + scope discipline):
- **Coalesce strategy**: per-type coalesce function declared by event author (`EventTierAttribute` с custom coalesce_fn) — explicit author control per Q-N-34
- **Idle-slot detection**: phase end before tick boundary (after dispatch barrier completes, before next tick wake firing) — Q-N-35 К10.2 simplest viable default; refinement deferred к К-extensions
- **Saturation strategy**: drop oldest с warning logged via observability hook (Item 19 К10.1) — Q-N-36 К10.2 default; backpressure/expand deferred к К-extensions
- **Size cap**: configurable per-mod (default 10MB shared pool); Q-N-45 deferred к brief authoring — К10.2 hard cap 10MB total, emit warning at 80% threshold

**Per К-L14 + scope discipline**: К10.2 lands functional Background tier с simplest defaults; refinement (per-mod budgets, NUMA-aware queuing) defers к К-extensions, not omitted.

**Validation**:
- `cmake --build` clean
- `background_queue_test`: coalesce correctness, idle-slot dispatch only при available budget, saturation drop-oldest behavior, multi-tick spread
- `dotnet test` 624+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.2 Item 30 — background queue с coalesce + idle-slot scheduling`

### Commit 6 — Item 27: Managed bus facade + C ABI bridge

**Files**:
- `src/DualFrontier.Application/Bus/BusFacade.cs` (new — IModApi-exposed surface; routes к native bus via C ABI)
- `src/DualFrontier.Application/Bus/ManagedBusBridge.cs` (new — Forward P/Invoke per tier + reverse callback per tier)
- `src/DualFrontier.Application/Modding/RestrictedModApi.cs` (modified — `Publish<T>` / `Subscribe<T>` route through BusFacade с tier-aware dispatch)
- `tests/DualFrontier.Core.Tests/Bus/ManagedBusBridgeTests.cs` (new)

**Drift surface**: Managed-side facade preserves IModApi (К-L9 uniformity) while routing к native bus. Existing managed bus continues к operate; BusFacade is **additive layer** — К-L15 sovereign authority switch deferred per managed-facade-preserved strategy.

**Implementation surface (per spec §3.8 Item 27)**:

```csharp
// BusFacade.cs
public sealed class BusFacade
{
    private readonly ManagedBusBridge _bridge;
    private readonly EventTypeRegistry _typeRegistry;
    private readonly bool _useNativeBus;  // К10.2 default: false (managed-facade-preserved)

    public void Publish<T>(T evt) where T : IEvent
    {
        var typeId = _typeRegistry.GetOrAssignId<T>();
        var tier = _typeRegistry.GetTier<T>();

        if (_useNativeBus)
        {
            // К-L15 native dispatch path (sovereign authority deferred к К10.4)
            switch (tier)
            {
                case BusTier.Fast:
                    _bridge.PublishFast(typeId, evt);
                    break;
                case BusTier.Normal:
                    _bridge.PublishNormal(typeId, evt);
                    break;
                case BusTier.Background:
                    _bridge.PublishBackground(typeId, evt, GetCoalesceKey(evt));
                    break;
            }
        }
        else
        {
            // Managed bus continues authoritative (К10.2 default)
            _managedBus.Publish(evt);

            // Parallel: register с native registry for verification (no actual dispatch)
            _bridge.RegisterForVerification(typeId, evt);
        }
    }

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        // Mirror semantics
    }
}
```

```csharp
// ManagedBusBridge.cs
public sealed class ManagedBusBridge
{
    [LibraryImport("DualFrontier.Core.Native", EntryPoint = "df_bus_publish_fast")]
    private static partial int PublishFastNative(uint typeId, IntPtr payload, uint payloadSize);

    [LibraryImport("DualFrontier.Core.Native", EntryPoint = "df_bus_publish_normal")]
    private static partial int PublishNormalNative(uint typeId, IntPtr payload, uint payloadSize);

    [LibraryImport("DualFrontier.Core.Native", EntryPoint = "df_bus_publish_background")]
    private static partial int PublishBackgroundNative(uint typeId, IntPtr payload, uint payloadSize, uint coalesceKey);

    // Reverse callbacks (per К10.1 Item 15 batched callback ABI pattern)
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void OnFastEventCallback(uint typeId, void* payload, uint payloadSize)
    {
        // Synchronous Fast tier subscriber dispatch
        // К-L15 contract: bounded exec, no blocking, no GC alloc
        try
        {
            // Marshal payload, invoke subscriber, return immediately
        }
        catch (Exception ex)
        {
            FaultLog.RecordFastTierFault(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void OnBatchedEventCallback(ManagedSystemBatch* batch)
    {
        // Batched Normal/Background tier subscriber dispatch
        // Reuses К10.1 Item 15 ManagedSystemDispatcher pattern
    }
}
```

**К-L9 uniformity preserved**: vanilla and mod systems use same managed bus API (BusFacade.Publish/Subscribe routed через IModApi). Mod authors don't see native bus directly; tier dispatch transparent based on event's `[EventTier]` attribute.

**Per Lesson #7**: spec §3.8 Item 27 + К10.1 Item 15 batched callback ABI patterns preserved verbatim — reverse-P/Invoke constraints (static method, blittable args, no generics, no managed exceptions across boundary, SuppressGCTransition forbidden) apply.

**Validation**:
- `cmake --build` clean
- `dotnet build` clean
- `dotnet test` 624+ green — existing scheduler tests not affected
- `ManagedBusBridgeTests`: round-trip per tier (Fast sync, Normal batched, Background coalesce); К-L9 uniformity preserved (IModApi surface unchanged)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel): K10.2 Item 27 — managed bus facade + C ABI bridge (3 tiers, IModApi uniformity preserved)`

### Commit 7 — Item 29: Subscriber contract enforcement (per-tier validation)

**Files**:
- `src/DualFrontier.Application/Modding/SubscriberContractValidator.cs` (new — per-tier load-time validation)
- `src/DualFrontier.Application/Modding/ContractValidator.cs` (modified — extended 8-phase pipeline с tier-aware capability validation)
- `src/DualFrontier.Application/Bus/FastTierContractMonitor.cs` (new — runtime instrumentation per К-L15)
- `tests/DualFrontier.Modding.Tests/SubscriberContractTests.cs` (new)

**Drift surface**: Capability validation extended к tier-aware. Existing `kernel.publish:{FQN}` / `kernel.subscribe:{FQN}` tokens continue к work и map к Normal tier (per S-LOCK-4 backward compatibility); new tier-explicit tokens (`kernel.fast.subscribe:{FQN}`, etc.) activate Fast/Background tier validation.

**Implementation surface (per spec §3.8 Item 29 + Q-N-37 resolution)**:

Load-time validation (extends ContractValidator Phase B):
- **Fast tier subscribers**: verified bounded exec contract — system method body cannot contain blocking calls (Roslyn analyzer deferred к A'.9; К10.2 lands runtime detection via Item 7 quota mechanism); no GC allocation hint via `[NoAllocation]` attribute (optional; К10.2 default off)
- **Normal tier subscribers**: standard subscriber contract (existing semantics)
- **Background tier subscribers**: verified interruptible (system method body must support multi-tick spread — accepts repeated invocations с partial state per Item 30 dispatch model)

Runtime instrumentation (per К-L15 fast tier latency invariant):
- Fast tier subscriber latency measured per call (deadline ≤1ms)
- Exceedance triggers fault handler (similar pattern к Item 7 К10.1 quota mechanism): logged via observability hook + counter incremented; on threshold breach (configurable, default 3 violations per N ticks) — mod fault handler invoked для mod subscribers, kernel fault log для Core subscribers
- Per-tier diagnostic API extension (consumes Item 4 scheduler diagnostics К10.1 + Item 19 trace ring buffer)

Capability token enforcement (per S-LOCK-4):
- `kernel.fast.subscribe:{FQN}` capability required для Fast tier subscribe call
- `kernel.fast.publish:{FQN}` capability required для Fast tier publish call
- Same pattern для Background tier
- Normal tier preserves existing `kernel.publish:{FQN}` / `kernel.subscribe:{FQN}` semantics (backward compatible)

**New ValidationErrorKind entries** (Commit 13 lands в MOD_OS §11.2):
- `FastTierContractViolation` — Fast tier subscriber has blocking method body или missing `kernel.fast.subscribe` capability
- `BusTierMismatch` — manifest declares tier-specific capability но event type has different tier annotation
- `BackgroundCoalesceMissing` — Background tier event missing coalesce function in `[EventTier]` attribute

**Validation**:
- `cmake --build` clean
- `dotnet build` clean
- `dotnet test` 624+ green
- `SubscriberContractTests`: per-tier capability validation, Fast tier latency monitoring, backward-compatible existing tokens work
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel): K10.2 Item 29 — subscriber contract enforcement (per-tier validation + runtime latency monitoring)`

### Commit 8 — Item 31: Background queue save-integrated storage (S3-Q3)

**Files**:
- `src/DualFrontier.Persistence/Bus/BackgroundQueueSerializer.cs` (new)
- `src/DualFrontier.Persistence/SaveFileFormat.cs` (extended — `background_queue` section added)
- `native/DualFrontier.Core.Native/src/background_queue.cpp` (extended — serialize/deserialize C ABI)
- `native/DualFrontier.Core.Native/include/background_queue.h` (extended)
- `tests/DualFrontier.Persistence.Tests/BackgroundQueueSerializerTests.cs` (new)

**Drift surface**: DualFrontier.Persistence extended к include background queue serialization (per S3-Q3 save-integrated semantics). Save-game format gains new section; loader gracefully handles missing section (older saves load с empty background queue).

**Implementation surface (per spec §3.9 Item 31 + S3-Q3 + Q-N-43/44)**:

C ABI surface:
```c
// background_queue.h extension
int32_t df_background_queue_serialize(
    void* out_buffer,
    uint32_t buffer_size,
    uint32_t* out_bytes_written);

int32_t df_background_queue_deserialize(
    const void* buffer,
    uint32_t buffer_size,
    uint32_t* out_events_loaded);
```

Serialization format (per S3-Q3 untargeted persistence):
- Header: schema version (uint32) + event count (uint32) + total payload bytes (uint32) + type_id table size (uint32)
- Type ID table: per К-L4 type registry pattern (verbatim type_id → FQN mapping для cross-version compatibility per Q-N-44)
- Event records: each as `{ type_id (uint32), coalesce_key (uint32), payload_size (uint32), payload_bytes (variable) }`
- Per S3-Q4 untargeted: events available для any future subscribers (not specifically queued для unloading mod's subscribers); allows mod replacement pattern

```csharp
// BackgroundQueueSerializer.cs
public sealed class BackgroundQueueSerializer
{
    public void Serialize(Stream output)
    {
        // Reserve sufficient buffer; call df_background_queue_serialize
        // Write к Stream
    }

    public void Deserialize(Stream input)
    {
        // Read schema version
        // If newer version than supported: log warning, skip section (graceful degradation)
        // If older version: apply migration per type_id table
        // Call df_background_queue_deserialize
    }
}
```

**Save-game compatibility versioning** (per Q-N-44 + R-K10-6 risk):
- Schema version field в serialized format
- Type ID table preserves FQN per type_id (cross-version mapping); если type missing в new game version → event dropped with warning logged; если type present but FQN changed → migration via type_id table lookup
- Migration mechanism deferred к К-extensions для complex schema changes; К10.2 lands forward-compatible serialization format

**Per К-L14 + R-K10-6 mitigation**: schema versioning is architectural primitive, not feature creep. К10.2 establishes migration foundation; full migration spec deferred per R-K10-6 mitigation note.

**Persistence integration**:
- DualFrontier.Persistence save flow extended: after existing world state serialization, BackgroundQueueSerializer.Serialize() appends `background_queue` section
- Load flow: after existing world state deserialization, BackgroundQueueSerializer.Deserialize() reads `background_queue` section; gracefully handles missing section (older saves)

**Size cap enforcement** (per Q-N-45):
- Default 10MB hard cap on serialized background queue
- При size > cap при save attempt: warning logged, oldest events dropped per saturation strategy (Item 30)

**Validation**:
- `cmake --build` clean
- `dotnet build` clean
- `BackgroundQueueSerializerTests`: round-trip serialization, version handling, type_id migration, size cap enforcement, missing-section graceful load
- `dotnet test` 624+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(persistence): K10.2 Item 31 — background queue save-integrated storage (S3-Q3 untargeted persistence)`

### Commit 9 — Item 32: Native unload primitive + ModUnloadResult (S3-Q1/Q6)

**Files**:
- `native/DualFrontier.Core.Native/include/mod_unload.h` (new)
- `native/DualFrontier.Core.Native/src/mod_unload.cpp` (new — T0-T7 internal sequence)
- `native/DualFrontier.Core.Native/test/mod_unload_test.cpp` (new selftest section)
- `src/DualFrontier.Core.Interop/ModUnloadInterop.cs` (new managed binding + ModUnloadResult marshalling)

**Drift surface**: Single native C ABI primitive encapsulates per-mod scheduler teardown (per S3-Q1 L3 layering + S3-Q6 single primitive). Не replaces ModIntegrationPipeline 7-step chain; inserts at Step 3.5 в Commit 11.

**Implementation surface (per spec §3.9 Item 32 + S3-Q1 L3 + S3-Q6 single primitive verbatim)**:

```c
// mod_unload.h verbatim from spec §3.9 Item 32

typedef struct {
    int success;  // bool

    // Fast tier
    int fast_subscriptions_cleared;
    int fast_in_flight_dropped;

    // Normal tier
    int normal_subscriptions_cleared;
    int normal_events_drained;
    int normal_batch_commit_completed;

    // Background tier
    int background_subscriptions_cleared;
    int background_events_preserved;
    int background_subscriber_count_remaining;

    // Capabilities
    int capabilities_revoked;

    // Wake registry
    int wake_subscriptions_cleared;

    // Errors (fixed-size array для C ABI compatibility)
    char error_messages[8][256];
    int error_count;
} ModUnloadResult;

int32_t df_scheduler_unload_mod_native_state(
    const char* mod_id,
    ModUnloadResult* out_result);
```

**Internal T0-T7 sequence** (per spec §3.9 Item 32 verbatim):
- **T0**: Lock subscriber registries (acquire scheduler critical section)
- **T1**: Fast tier — clear subscriptions + drop in-flight events (per S3-Q2 «fast drop» policy)
- **T2**: Normal tier — drain current batch к commit boundary (per К-L7 atomic-from-observer), then clear subscriptions
- **T3**: Background tier — clear subscriber registry, queue contents untouched (per S3-Q3/Q4 untargeted persistence — events available для future subscribers)
- **T4**: Revoke fast/background tier capabilities per FQN
- **T5**: Clear ShmWriter/ShmReader registrations (per К10.1 Item 9 shared memory) + CPU affinity (per К10.1 Item 11)
- **T6**: Clear wake registry subscriptions (5 wake types per К10.1 Item 3 — TimerWake, EventWake, StateChangeWake, InitWake, ExplicitWake — orderly teardown per Q-N-48 — order: ExplicitWake → InitWake (no-op for already-fired) → StateChangeWake (with filter primitive update per К10.1 Item 17 S2 hybrid) → EventWake → TimerWake)
- **T7**: Unregister system access declarations (per К10.1 Item 1 native dependency graph rebuild trigger)
- **Unlock**: Release scheduler critical section, populate ModUnloadResult, return

**К-L18 quiescent state precondition** (per S8-Q4 lock — К10.3 scope для full enforcement; К10.2 lands precondition check, full UI enforcement deferred к К10.3):
- К10.2: `df_scheduler_unload_mod_native_state` returns error code если `sim_state != Paused` (no partial teardown); К10.3 lands settings menu / mod management UI integration enforcing precondition
- К10.2 default behavior: error returned, ModUnloadResult.success=0, error_messages contains «quiescent state precondition violated»

**ModUnloadResult marshalling**:
```csharp
// ModUnloadInterop.cs
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ModUnloadResult
{
    public int Success;
    public int FastSubscriptionsCleared;
    public int FastInFlightDropped;
    public int NormalSubscriptionsCleared;
    public int NormalEventsDrained;
    public int NormalBatchCommitCompleted;
    public int BackgroundSubscriptionsCleared;
    public int BackgroundEventsPreserved;
    public int BackgroundSubscriberCountRemaining;
    public int CapabilitiesRevoked;
    public int WakeSubscriptionsCleared;
    public fixed byte ErrorMessages[8 * 256];
    public int ErrorCount;
}

public static partial class ModUnloadInterop
{
    [LibraryImport("DualFrontier.Core.Native", EntryPoint = "df_scheduler_unload_mod_native_state",
                    StringMarshalling = StringMarshalling.Utf8)]
    public static partial int UnloadModNativeState(string modId, out ModUnloadResult result);
}
```

**Per Lesson #7**: ModUnloadResult struct shape preserved literally from spec; T0-T7 internal sequence transcribed verbatim per S3-Q6 single primitive contract.

**Validation**:
- `cmake --build` clean
- `mod_unload_test` selftest section: 
  - T0-T7 sequence orderly execution
  - Quiescent state precondition enforced
  - Per-tier in-flight policy applied (Fast drop, Normal drain, Background persist)
  - ModUnloadResult populated correctly
  - Error path returns proper error messages
- `dotnet test` 624+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.2 Item 32 — native unload primitive + ModUnloadResult (S3-Q1 L3 + S3-Q6 single primitive)`

### Commit 10 — Item 21: Mod scheduler authority (per-mod sub-schedulers)

**Files**:
- `src/DualFrontier.Application/Modding/ModSubScheduler.cs` (new — per-mod managed sub-scheduler instance)
- `src/DualFrontier.Application/Modding/ModRegistry.cs` (modified — extended с per-mod scheduler ownership tracking)
- `src/DualFrontier.Application/Loop/SchedulerAdapter.cs` (modified — К10.1 Commit 11 SchedulerAdapter extended с per-mod sub-scheduler dispatch; mod system batches dispatched к owning mod's sub-scheduler instead of monolithic ManagedSystemDispatcher)
- `tests/DualFrontier.Modding.Tests/ModSubSchedulerTests.cs` (new)

**Drift surface**: Each mod ALC owns sub-scheduler instance per К-L12 separation. Kernel scheduler dispatches mod-system batch as **opaque dispatch к mod-side scheduler** instead of dispatching individual mod systems directly.

**Implementation surface (per spec §3.6 Item 21 + S3-Q1 L3 + К-L12)**:

```csharp
// ModSubScheduler.cs
public sealed class ModSubScheduler
{
    private readonly string _modId;
    private readonly List<SystemBase> _systems = new();
    private readonly TaskScheduler _taskScheduler;  // ALC-isolated TaskScheduler instance

    public void Dispatch(ReadOnlySpan<uint> systemIds, float delta)
    {
        // Mod-internal: mod owns scheduling decisions (priority, ordering, parallelism)
        // К-L12 cleanness: kernel doesn't arbitrate inside mod processes
        Parallel.ForEach(GetSystemsFor(systemIds), system =>
        {
            SystemExecutionContext.PushContext(/* mod-scoped */);
            try { system.Update(delta); }
            finally { SystemExecutionContext.PopContext(); }
        });
    }

    public void Teardown()
    {
        // Called by Step 3.5 native primitive consumer
        // K10.2: native primitive (Item 32) handles native state; this method handles managed state
        foreach (var system in _systems) (system as IDisposable)?.Dispose();
        _systems.Clear();
    }
}
```

```csharp
// ModRegistry.cs extension
public sealed class ModRegistry
{
    private readonly Dictionary<string, ModSubScheduler> _modSchedulers = new();

    public ModSubScheduler GetOrCreateSubScheduler(string modId)
    {
        if (!_modSchedulers.TryGetValue(modId, out var sub))
        {
            sub = new ModSubScheduler(modId);
            _modSchedulers[modId] = sub;
        }
        return sub;
    }

    public bool TryGetSubScheduler(string modId, out ModSubScheduler? sub) =>
        _modSchedulers.TryGetValue(modId, out sub);

    public void RemoveSubScheduler(string modId)
    {
        if (_modSchedulers.TryGetValue(modId, out var sub))
        {
            sub.Teardown();
            _modSchedulers.Remove(modId);
        }
    }
}
```

**Per S3-Q1 L3 layering**: per-mod sub-scheduler teardown encapsulated в native primitive (Item 32 Commit 9); managed-side Teardown() method handles ALC-isolated managed state в parallel.

**К-L9 «Vanilla = mods» preserved**: vanilla mods get sub-scheduler same as third-party mods. Registration uniformity preserved через IModApi.RegisterSystem<T> — kernel ModRegistry routes к owning mod's sub-scheduler.

**К10.2 boundary**: К10.2 establishes infrastructure (per-mod sub-scheduler instance ownership + teardown primitive consumption); detailed mod-system priority arbitration within mod sub-scheduler stays К8.5 scope (mod authoring practice documentation).

**Validation**:
- `dotnet build` clean
- `dotnet test` 624+ green — existing mod tests preserved
- `ModSubSchedulerTests`: per-mod sub-scheduler instance ownership, dispatch routing к owning mod, teardown via native primitive integration
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel): K10.2 Item 21 — mod scheduler authority (per-mod sub-schedulers + ALC-isolated dispatch)`

### Commit 11 — ModIntegrationPipeline.UnloadMod Step 3.5 insertion

**Files**:
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` (modified — Step 3.5 inserted between existing Step 3 RemoveSystems и Step 4 graph rebuild)
- `tests/DualFrontier.Modding.Tests/ModUnloadIntegrationTests.cs` (new — full 8-step chain validation, including new Step 3.5)

**Drift surface**: Existing 7-step unload chain (per MOD_OS §9.5) becomes 8-step (Step 3.5 inserted). Existing chain semantics preserved; new step calls `df_scheduler_unload_mod_native_state` and processes ModUnloadResult per S3-Q6.

**Implementation surface (per S3-Q1 L3 + S3-Q6 + MOD_OS §9.5 verbatim)**:

```csharp
// ModIntegrationPipeline.cs UnloadMod extension
public void UnloadMod(string modId)
{
    var mod = _registry.GetMod(modId);
    if (mod == null) return;

    // Step 1: RestrictedModApi.UnsubscribeAll() — drops bus subscriptions (existing)
    try { mod.Api?.UnsubscribeAll(); }
    catch (Exception ex) { LogStep(modId, 1, ex); }  // per §9.5.1 failure semantics

    // Step 2: IModContractStore.RevokeAll(modId) — drops contract registrations (existing)
    try { _contractStore.RevokeAll(modId); }
    catch (Exception ex) { LogStep(modId, 2, ex); }

    // Step 3: ModRegistry.RemoveSystems(modId) — drops system instances (existing)
    try { _registry.RemoveSystems(modId); }
    catch (Exception ex) { LogStep(modId, 3, ex); }

    // NEW Step 3.5 (К10.2): native primitive — encapsulates T0-T7 native teardown sequence
    try
    {
        var rc = ModUnloadInterop.UnloadModNativeState(modId, out var result);
        if (rc != 0 || result.Success == 0)
        {
            // Native teardown reported failures
            for (int i = 0; i < result.ErrorCount; i++)
            {
                LogStep(modId, 35, new InvalidOperationException(
                    $"Native unload error: {GetErrorMessage(result, i)}"));
            }
        }

        // Audit metrics from ModUnloadResult logged via observability hook (К10.1 Item 19)
        _trace?.RecordModUnload(modId, result);

        // Per-mod sub-scheduler managed-side teardown (Item 21 consumption)
        _registry.RemoveSubScheduler(modId);
    }
    catch (Exception ex) { LogStep(modId, 35, ex); }

    // Step 4: Dependency graph rebuilt without this mod's systems (existing)
    try { _dependencyGraph.Rebuild(_registry.AllSystems); }
    catch (Exception ex) { LogStep(modId, 4, ex); }

    // Step 5: Scheduler swaps to new phase list (existing)
    try { _scheduler.SwapPhases(_dependencyGraph.GetPhases()); }
    catch (Exception ex) { LogStep(modId, 5, ex); }

    // Step 6: ALC.Unload() (existing)
    try { mod.LoadContext.Unload(); }
    catch (Exception ex) { LogStep(modId, 6, ex); }

    // Step 7: WeakReference spin loop (existing, per §9.5 step 7)
    SpinUntilWeakReferenceDead(mod.LoadContextWeakRef);
}
```

**Per MOD_OS §9.5.1 failure semantics**: Step 3.5 follows same best-effort sequential discipline. Если native primitive call fails, error logged как non-blocking `ValidationWarning`; chain continues к Step 4. Native primitive itself uses T0-T7 sequence with internal critical section atomicity — Step 3.5 от ModIntegrationPipeline perspective is single atomic call.

**Backward compatibility**:
- If mod was loaded before К10.2 (no native registration), `df_scheduler_unload_mod_native_state` returns success vacuously (no native state к clean up)
- Existing tests preserved: full 7-step chain semantics unchanged from observer perspective; Step 3.5 transparent для mod authors

**Per Lesson #7**: MOD_OS §9.5 verbatim 7-step chain preserved literally; Step 3.5 insertion text matches existing wording style.

**Validation**:
- `dotnet build` clean
- `dotnet test` 624+ green — existing WeakReference unload tests, capability violation tests, all preserved
- `ModUnloadIntegrationTests`: full 8-step chain executes orderly; ModUnloadResult metrics captured; native error path logged correctly; sub-scheduler teardown integrated
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(modding): K10.2 — ModIntegrationPipeline Step 3.5 insertion (df_scheduler_unload_mod_native_state)`

### Commit 12 — KernelCapabilityRegistry tier-prefixed tokens

**Files**:
- `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs` (modified — scanning logic extended к include tier attribute reading; tier-prefixed token generation per S3-Q5)
- `src/DualFrontier.Application/Modding/ContractValidator.cs` (modified — Phase B capability validation extended с tier-aware matching)
- `tests/DualFrontier.Modding.Tests/KernelCapabilityRegistryTierTests.cs` (new)

**Drift surface**: KernelCapabilityRegistry scanning extension. Для каждого `IEvent` type, scanner checks `[EventTier]` attribute → generates appropriate tier-prefixed tokens. Existing tokens (`kernel.publish:{FQN}`, `kernel.subscribe:{FQN}`) preserved для backward compatibility и map к Normal tier.

**Implementation surface (per S3-Q5 + spec §3.8 Item 28 + MOD_OS §3.2 extension)**:

```csharp
// KernelCapabilityRegistry.cs scanning extension
public sealed class KernelCapabilityRegistry
{
    public IReadOnlySet<string> GetKernelCapabilities()
    {
        var capabilities = new HashSet<string>();
        var eventTypes = ScanForTypes<IEvent>();

        foreach (var eventType in eventTypes)
        {
            var fqn = eventType.FullName!;
            var tier = GetTier(eventType);  // reads [EventTier] attribute, defaults к Normal

            // К10.2: tier-prefixed tokens per S3-Q5
            switch (tier)
            {
                case BusTier.Fast:
                    capabilities.Add($"kernel.fast.publish:{fqn}");
                    capabilities.Add($"kernel.fast.subscribe:{fqn}");
                    break;
                case BusTier.Normal:
                    capabilities.Add($"kernel.normal.publish:{fqn}");
                    capabilities.Add($"kernel.normal.subscribe:{fqn}");
                    // Backward-compatible aliases (per S-LOCK-4)
                    capabilities.Add($"kernel.publish:{fqn}");
                    capabilities.Add($"kernel.subscribe:{fqn}");
                    break;
                case BusTier.Background:
                    capabilities.Add($"kernel.background.publish:{fqn}");
                    capabilities.Add($"kernel.background.subscribe:{fqn}");
                    break;
            }
        }

        // Existing component, field, pipeline capabilities preserved
        // ... existing scanning logic
        return capabilities;
    }

    private static BusTier GetTier(Type eventType)
    {
        var attr = eventType.GetCustomAttribute<EventTierAttribute>();
        return attr?.Tier ?? BusTier.Normal;  // default per S-LOCK-4 backward compat
    }
}
```

```csharp
// ContractValidator.cs Phase B extension
private ValidationResult ValidateCapabilities(ModManifest manifest, IReadOnlySet<string> kernelCaps)
{
    foreach (var required in manifest.Capabilities.Required)
    {
        // Match strict (token must exist in kernel-provided OR dependency-provided set)
        // К10.2: tier-aware matching — Normal tier tokens have backward-compat aliases
        if (kernelCaps.Contains(required))
            continue;

        // Cross-mod resolution (existing semantics)
        if (TryResolveCrossModCapability(required, manifest.Dependencies))
            continue;

        // Per К10.2: surface tier-aware diagnostic
        if (IsTierPrefixedToken(required))
        {
            var inferredTier = InferTierFromToken(required);
            if (HasMatchingTierMismatch(required, kernelCaps))
                return ValidationResult.Failure(ValidationErrorKind.BusTierMismatch,
                    $"Capability '{required}' requests {inferredTier} tier but event type is annotated с different tier");
        }

        return ValidationResult.Failure(ValidationErrorKind.MissingCapability,
            $"Mod '{manifest.Id}' requires capability '{required}' that is not provided by kernel or any listed dependency");
    }
    return ValidationResult.Success();
}
```

**К10.2 default**: existing third-party mods using `kernel.publish:{FQN}` / `kernel.subscribe:{FQN}` continue к work без manifest changes. Mod authors opt-into tier-explicit tokens только when they want Fast tier (latency contract) or Background tier (off-screen simulation pattern) semantics.

**Validation**:
- `dotnet build` clean
- `dotnet test` 624+ green
- `KernelCapabilityRegistryTierTests`: tier-prefixed token generation, backward-compat aliases for Normal tier, BusTierMismatch detection
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(modding): K10.2 — KernelCapabilityRegistry tier-prefixed tokens (S3-Q5 per-FQN per-tier)`

### Commit 13 — К-L15 invariant landing + cross-document amendments (load-bearing)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.0 → v2.1 (К-L invariant table updated — К-L15 row added; Part 2 K10 row updated с К-L15 ratified)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.8 → v1.9 (§3.2 tier-prefixed capabilities documented; §9.5 amended к 8 steps describing Step 3.5 insertion; §11.2 new ValidationErrorKind entries added)
- `docs/governance/REGISTER.yaml` (DOC-A-KERNEL version bump 2.0 → 2.1; DOC-A-MOD_OS version bump 1.8 → 1.9; governance_events append references к К10.2 closure)

**This is the load-bearing commit of К10.2** analogous к К10.1 Commit 14 в scope/risk. All prior К10.2 commits land **additive** infrastructure parallel к managed bus + capability layer; Commit 13 lands К-L15 invariant text + amendments propagation.

**Drift surface (large)**:
- К-L15 added к KERNEL_ARCHITECTURE.md Part 0 К-L table с verbatim text from KERNEL_FULL_NATIVE_SCHEDULER.md Part 2
- KERNEL_ARCHITECTURE.md Part 2 master plan: К10 row updated с К-L15 ratified annotation
- MOD_OS_ARCHITECTURE.md §3.2: capability syntax extended documentation с tier prefixes
- MOD_OS_ARCHITECTURE.md §9.5: 7-step chain documented as 8-step с Step 3.5 description (matching К10.2 Commit 11 implementation)
- MOD_OS_ARCHITECTURE.md §11.2: new ValidationErrorKind entries added (`FastTierContractViolation`, `BusTierMismatch`, `BackgroundCoalesceMissing`)
- К-L9 «Vanilla = mods» note preserved (К-L15 explicitly states managed bus facade preserves IModApi uniformity)

**К-L15 invariant text (verbatim from spec Part 2 — Lesson #7 transcription mandatory)**:

> «Native kernel owns sovereign event routing for kernel-space and cross-layer events. Bus implementation native: type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics all native authority. Bus supports three-tier dispatch (fast / normal / background) with tier declared per event type. Managed bus facade preserved as IModApi-exposed surface (К-L9 uniformity); implementation routes through C ABI bridge к native bus. Fast/background event publish/subscribe requires per-FQN per-tier capability declarations. Cross-layer event delivery uses batched callback ABI (normal/background tiers) or immediate synchronous callback (fast tier).»

**К-L invariant table amendment** (KERNEL_ARCHITECTURE.md Part 0):

К-L15 row inserted after К-L14:
```
| K-L15 | Native bus authority + three-tier event dispatch | Native kernel owns sovereign event routing; three-tier dispatch (fast / normal / background); managed bus facade preserves IModApi uniformity (К-L9) | OS-faithful event routing; cross-layer delivery via batched callback ABI; К10.2 amendment |
```

**MOD_OS §9.5 amendment** (existing 7-step chain becomes 8-step with Step 3.5 inserted):

```markdown
### 9.5 ALC unload protocol

`AssemblyLoadContext.Unload()` is asynchronous; the runtime waits для all references к the assembly к be released. The unload chain:

1. `RestrictedModApi.UnsubscribeAll()` — drops bus subscriptions.
2. `IModContractStore.RevokeAll(modId)` — drops contract registrations.
3. `ModRegistry.RemoveSystems(modId)` — drops system instances.
3.5. **(К10.2)** `df_scheduler_unload_mod_native_state(modId)` — native primitive encapsulating T0-T7 internal sequence: clears native scheduler state (subscriber registries per tier, capability registrations, wake registry subscriptions, shared memory registrations); returns `ModUnloadResult` с per-tier metrics. Best-effort sequential per §9.5.1; native primitive internal critical section atomicity (per S3-Q1 L3 layering, single primitive contract per S3-Q6).
4. The dependency graph is rebuilt without this mod's systems.
... [existing steps 5-7 preserved verbatim]
```

**MOD_OS §3.2 amendment** (tier-prefixed capability documentation):

Document tier-prefixed token format added к existing capability syntax section:
```
- `kernel.fast.publish:<FQN>` / `kernel.fast.subscribe:<FQN>` — Fast tier (≤1ms latency target)
- `kernel.normal.publish:<FQN>` / `kernel.normal.subscribe:<FQN>` — Normal tier (per-phase batched)
  - **Backward-compatible aliases**: `kernel.publish:<FQN>` / `kernel.subscribe:<FQN>` continue к function and map к Normal tier
- `kernel.background.publish:<FQN>` / `kernel.background.subscribe:<FQN>` — Background tier (multi-tick coalesce)
```

**MOD_OS §11.2 amendment** (ValidationErrorKind entries):
- `FastTierContractViolation` (К10.2) — Fast tier subscriber violates bounded exec contract OR Fast tier capability declared без matching event type tier annotation
- `BusTierMismatch` (К10.2) — manifest declares tier-specific capability but event type has different tier annotation
- `BackgroundCoalesceMissing` (К10.2) — Background tier event missing coalesce function declaration

**REGISTER.yaml amendments**:
- DOC-A-KERNEL: version 1.6 → 2.0 (post-К10.1) → 2.1 (К10.2 К-L15 amendment); last_modified_commit к К10.2 Commit 13; governance_events append с EVT-K10_2-CLOSURE reference (added в Commit 14 closure)
- DOC-A-MOD_OS: version 1.8 → 1.9; last_modified_commit к К10.2 Commit 13; governance_events append с EVT-K10_2-CLOSURE reference
- DOC-A-KERNEL_FULL_NATIVE_SCHEDULER: governance_events append с EVT-K10_2-CLOSURE (К10.2 closure references this spec as authority for Items 21, 26-32)
- DOC-D-K10_2: lifecycle preserved AUTHORED until Commit 14 (closure flips к EXECUTED)

**Per Lesson #8 (atomic compilable unit)**: К-L15 invariant text + KERNEL_ARCHITECTURE amendment + MOD_OS amendments + REGISTER updates land в **same commit**. Splitting these into separate commits leaves intermediate state where К-L table claims «К-L15 in effect» but MOD_OS §3.2 capability syntax doesn't reflect tier extensions (inconsistent doc state).

**Per К-L14 default-inclusion**: cross-document amendments are architectural primitive (К-L15 invariant landing), not optional refinement. К10.2 cannot close без MOD_OS §9.5 reflecting Step 3.5 reality.

**Validation**:
- `cmake --build` clean
- `dotnet build` clean
- `dotnet test` 624+ green — К-L15 amendment doesn't introduce code changes, тестовая база preserved through К10.2 cascade
- К-L invariant cross-reference integrity verified: KERNEL_ARCHITECTURE.md v2.1 К-L15 references KERNEL_FULL_NATIVE_SCHEDULER.md Part 2 verbatim
- MOD_OS §9.5 Step 3.5 description matches К10.2 Commit 11 implementation literally
- `sync_register.ps1 --validate` exit 0

**Commit message**:
```
feat(architecture): K10.2 — К-L15 amendment landing + cross-document propagation

The load-bearing commit of К10.2. К-L15 «Native bus authority + three-tier
event dispatch» added к KERNEL_ARCHITECTURE.md v2.0 → v2.1. К-L invariant
table extended с К-L15 row preserving К-L9 «Vanilla = mods» uniformity
through managed bus facade.

MOD_OS_ARCHITECTURE.md v1.8 → v1.9 cross-document amendments:
- §3.2 capability syntax extended с tier-prefixed token format
- §9.5 unload chain extended 7-step → 8-step с Step 3.5 inserted
- §11.2 ValidationErrorKind entries added: FastTierContractViolation,
  BusTierMismatch, BackgroundCoalesceMissing

К-L15 text: «Native kernel owns sovereign event routing for kernel-space and
cross-layer events. Bus implementation native: type registry, subscriber
registry, payload dispatch, wake firing, tier-based delivery semantics all
native authority...»

Build clean; <N> tests green (target 624+).

Phase 13 of K10.2 cascade. Commit 13 of <total>.
```

### Commit 14 — К10.2 closure: REGISTER amendments + audit_trail EVT + CAPA closures + brief lifecycle EXECUTED

**Files**:
- `docs/governance/REGISTER.yaml` (DOC-D-K10_2 lifecycle AUTHORED → EXECUTED; audit_trail EVT entry; CAPA entries если any opened during cascade)
- `tools/briefs/K10_2_EXECUTION_BRIEF.md` (frontmatter status AUTHORED → EXECUTED; new §8 closure section added)
- `docs/MIGRATION_PROGRESS.md` (К10.2 closure entry per METHODOLOGY §12.7 step 3)
- `docs/governance/REGISTER_RENDER.md` (regenerated via render_register.ps1)
- `docs/governance/VALIDATION_REPORT.md` (regenerated via sync_register.ps1)

**REGISTER amendments**:

1. **DOC-D-K10_2**: lifecycle AUTHORED → EXECUTED; last_modified_commit к К10.2 closure commit; brief lifecycle transition complete
2. **DOC-A-KERNEL** (KERNEL_ARCHITECTURE.md): version 2.0 → 2.1 (К-L15 amendment landed in Commit 13); last_modified_commit к К10.2 Commit 13; governance_events extended с EVT-K10_2-CLOSURE reference
3. **DOC-A-MOD_OS** (MOD_OS_ARCHITECTURE.md): version 1.8 → 1.9 (Step 3.5 + tier capabilities + new ValidationErrorKinds); last_modified_commit к К10.2 Commit 13; governance_events extended
4. **DOC-A-KERNEL_FULL_NATIVE_SCHEDULER**: governance_events append с EVT-K10_2-CLOSURE (К10.2 closure references this spec as authority)
5. **audit_trail entry**: `EVT-{date}-K10_2-CLOSURE` — type: execution_milestone; documents_affected: KERNEL_ARCHITECTURE + MOD_OS_ARCHITECTURE + KERNEL_FULL_NATIVE_SCHEDULER + DOC-D-K10_2 lifecycle transition; commits: range Commit 1..13; governance_impact: «К10.2 closure — native bus three-tier dispatch landed (К-L15 AUTHORED); per-mod sub-scheduler authority + mod ALC native unload primitive integrated»
6. **Requirements added** (REQ collection): REQ-K-L15 — verified_by linking к native bus tests + tier capability tests + Step 3.5 integration tests
7. **Risks status update** (R-K10-6, R-K10-7 from spec Part 8): 
   - R-K10-6 (background queue save compatibility versioning) status ACTIVE → RESIDUAL (К10.2 lands schema versioning foundation; full migration spec deferred к К-extensions per mitigation note)
   - R-K10-7 (filter consistency races) status preserved ACTIVE; TLA+ verification К10.4 closes per К-L15 + S2 filter integration

**Per METHODOLOGY §12.7 canonical closure protocol**:
1. ✅ Final verification (build + tests + native + sync_register)
2. ✅ Atomic commit с scope prefix
3. ✅ Update MIGRATION_PROGRESS.md closure entry
4. ✅ Update brief Status field (К10.2 → EXECUTED)
5. ✅ Update REGISTER.yaml entries для all documents touched
6. ✅ Append audit_trail entry (EVT-{date}-K10_2-CLOSURE)
7. ✅ Run sync_register.ps1 --validate (exit 0 required)
8. ✅ Final commit incorporates REGISTER.yaml updates

**Validation**:
- `sync_register.ps1 --validate` exit 0 — **mandatory gate** per METHODOLOGY §12.7
- `dotnet build` clean
- `dotnet test` 624+ green (К10.2 baseline preserved + new tests additive; final count documented в closure)
- `cmake --build` clean
- Native selftest scenarios: ~80-90 (58 pre-К10.2 + ~25-30 new К10.2 sections)

**Commit message**:
```
governance: K10.2 closure — REGISTER amendments + REQ-K-L15 + EVT-K10_2-CLOSURE

К10.2 sub-milestone closure per METHODOLOGY §12.7 canonical protocol.

REGISTER updates:
- DOC-D-K10_2 lifecycle AUTHORED → EXECUTED
- DOC-A-KERNEL version 2.0 → 2.1 (К-L15 amendment)
- DOC-A-MOD_OS version 1.8 → 1.9 (§9.5 Step 3.5 + §3.2 tier capabilities + §11.2 new ValidationErrorKinds)

Requirements added:
- REQ-K-L15 (native bus authority + three-tier event dispatch) — verified_by
  native bus tests, tier capability validation tests, Step 3.5 integration tests

audit_trail entry: EVT-{date}-K10_2-CLOSURE

К10.2 closure leaves К10.3 (pipeline depth + display composition + hardware
tier, 12 items) and К10.4 (TLA+ formal verification, 3 items) as separate
future sub-milestone briefs per Option III standalone-briefs structure.

К-L15 architecturally landed; К-L15 measurable sovereign authority switch
(actual bus dispatch ownership к native) deferred к К10.4 closure or
К-closure report (А'.8) per managed-facade-preserved strategy (К10.1
precedent).

Phase 14 of K10.2 cascade. Commit 14 of 14 — К10.2 closure.
```

---

## §4 — К-L invariant amendment (Commit 13 detailed scope)

### §4.1 — Verbatim К-L15 row amendment

`docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L invariants table receives К-L15 row insertion after К-L14 (which landed в К10.1):

```
| K-L15 | Native bus authority + three-tier event dispatch | Native kernel owns sovereign event routing для kernel-space and cross-layer events. Bus implementation native (type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery). Three-tier dispatch (fast / normal / background) с tier declared per event type. Managed bus facade preserved as IModApi-exposed surface (К-L9 uniformity); routes through C ABI bridge к native bus. Fast/background event publish/subscribe requires per-FQN per-tier capability declarations. Cross-layer event delivery uses batched callback ABI (normal/background) или immediate synchronous callback (fast). | OS-faithful event routing; К-L9 «Vanilla = mods» preserved through facade; К10.2 amendment |
```

### §4.2 — KERNEL_ARCHITECTURE.md Part 2 master plan amendment

Part 2 master plan table receives К10 row update (existing К10 row from К10.1 closure updated с К-L15 ratified annotation):

```
| К10 | Native kernel scheduler + bus + mod ALC native primitives | AUTHORED-IN-PROGRESS (К10.1 closed YYYY-MM-DD; К10.2 closed YYYY-MM-DD) | К-L6 SUPERSEDED + К-L12/L13/L14 (К10.1) + К-L15 (К10.2); К-L7.1 + К-L16/L17/L18/L19 К10.3 pending; TLA+ К10.4 pending |
```

К-closure row remains unchanged.

### §4.3 — MOD_OS_ARCHITECTURE.md cross-document amendments

К10.2 amendment surface к MOD_OS_ARCHITECTURE.md v1.8 → v1.9:

**§3.2 capability syntax** — tier-prefixed token format added:
- Existing verb enumeration preserved (`publish`, `subscribe`, `read`, `write`, `field.*`, `pipeline.register`)
- Tier prefix syntax documented: `kernel.<tier>.<verb>:<FQN>` where `<tier> ∈ {fast, normal, background}`
- Backward-compat alias rule explicit: `kernel.publish:<FQN>` ≡ `kernel.normal.publish:<FQN>` (per S-LOCK-4)

**§3.7 cross-check c [SystemAccess]** — tier-aware diagnostic:
- Drift between manifest tier-prefixed capability и `[EventTier]` attribute on event type surfaces as `ValidationErrorKind.BusTierMismatch` (load-time diagnostic)

**§9.5 unload chain** — Step 3.5 insertion (8 steps total):
```
1. RestrictedModApi.UnsubscribeAll() — drops bus subscriptions.
2. IModContractStore.RevokeAll(modId) — drops contract registrations.
3. ModRegistry.RemoveSystems(modId) — drops system instances.
3.5. (К10.2) df_scheduler_unload_mod_native_state(modId) — native primitive encapsulating T0-T7 internal sequence: clears native scheduler state (subscriber registries per tier, capability registrations, wake registry subscriptions, shared memory registrations); returns ModUnloadResult с per-tier metrics. Best-effort sequential per §9.5.1; native primitive internal critical section atomicity (per S3-Q1 L3 layering, single primitive contract per S3-Q6).
4. The dependency graph is rebuilt without this mod's systems.
5. The scheduler swaps к new phase list.
6. ALC.Unload() is called.
7. The loader spins on WeakReference.IsAlive (existing protocol).
```

**§9.5.1 failure semantics** — preserved verbatim; Step 3.5 follows best-effort sequential discipline same as existing steps.

**§11.2 ValidationErrorKind** — three new entries:
- `FastTierContractViolation` (К10.2) — Fast tier subscriber violates bounded exec contract OR Fast tier capability missing matching tier annotation
- `BusTierMismatch` (К10.2) — manifest declares tier-specific capability but event type has different tier annotation
- `BackgroundCoalesceMissing` (К10.2) — Background tier event missing coalesce function declaration in `[EventTier]` attribute

### §4.4 — К10.2 amendment scope bounded — К10.3 carries rest

Per Crystalka ratification 2026-05-18: К10.2 amendment scope **точечно версии поменять** (KERNEL_ARCHITECTURE v2.0 → v2.1; MOD_OS v1.8 → v1.9). Other documents (VULKAN_SUBSTRATE, MIGRATION_PLAN, PHASE_A_PRIME_SEQUENCING) **not touched** by К10.2 — К10.3 carries pipeline + display + hardware tier amendments to VULKAN_SUBSTRATE; full Q-K-1 retroactive lock + А' sequencing reconciliation happens at К10.4 closure when К-series fully landed.

К10.2 keeps amendment surface bounded к focal К-L invariant (К-L15) + mod-OS integration touchpoint (§9.5 Step 3.5) + capability layer extension (§3.2 + §11.2).

---

## §5 — Halt triggers (К10.2-specific SC-N taxonomy)

Если execution agent encounters any of these conditions, **halt и surface к Crystalka**. Per Lesson #8 corollary: brief promises «halts before damage», not «zero halts». Halts на К10.2 are **success indicators** — К10.1 had zero halts, К10.2 may surface different shapes given different scope characteristics.

### SC-1 — Code anchor doesn't match spec evidence

Если any code anchor (RestrictedModApi.cs, ModIntegrationPipeline.cs, KernelCapabilityRegistry.cs, ContractValidator.cs, ModRegistry.cs, native df_capi.h, managed_callback.h) doesn't match spec's described shape after К10.1 closure (2026-05-18), halt. Brief authored 2026-05-18 from point-in-time read; drift between brief authoring и execution time surfaces here.

### SC-2 — Native bus thread synchronization bug

Если native bus tests reveal race conditions, deadlocks, или data corruption в hot path (Fast tier subscribers, batch dispatch boundaries, Background queue coalesce'инг), halt.

Recovery: stop-the-world debugging via Item 20 scheduler intrinsics (suspend/resume/snapshot from К10.1). Surface к Crystalka before continuing.

### SC-3 — Deep-read contradiction

Any §2.3 mandatory re-read surfaces a file shape that contradicts this brief. Halt и surface contradiction. К10.2 brief was authored from a point-in-time read 2026-05-18; drift since then is exactly what this trigger catches per К10.1 SC-3 precedent + Lesson #7.

### SC-4 — К-L15 amendment cross-reference integrity broken

Commit 13 amends KERNEL_ARCHITECTURE.md v2.0 → v2.1 + MOD_OS_ARCHITECTURE.md v1.8 → v1.9. Если after Commit 13 `sync_register.ps1 --validate` flags broken cross-references (e.g., К-L15 references KERNEL_FULL_NATIVE_SCHEDULER.md Part 2 but path lookup fails, OR MOD_OS §9.5 Step 3.5 description references non-existent native primitive symbol), halt.

Recovery: verify amendment text against spec verbatim; fix cross-reference; re-validate.

### SC-5 — Fast tier latency budget violation in tests

Commit 4 + Commit 7 land Fast tier infrastructure. Если К10.2 native bus tests show Fast tier subscriber dispatch latency exceeding 1ms target consistently (per К-L15 latency invariant + Prediction 7 §5.1.A), halt.

Recovery: profile dispatch path; identify GC interaction, lock contention, или callback overhead causing latency; surface к Crystalka — К-L15 fast tier semantic refinement may be required.

### SC-6 — ModUnloadResult struct shape mismatch managed/native

Commit 9 lands ModUnloadResult struct. Если after Commit 9 marshalling test fails (managed view of ModUnloadResult doesn't match native layout — padding, alignment, или field ordering bug), halt.

Recovery: verify `[StructLayout(LayoutKind.Sequential)]` attribute matches C struct layout; check fixed-size array marshalling; surface если non-trivial.

### SC-7 — Background queue save format incompatibility

Commit 8 lands background queue save integration. Если saved background queue loads с error на subsequent game session OR schema versioning produces silent data loss, halt.

Recovery: investigate schema migration logic; verify type_id table preservation; if structural format change needed, surface к Crystalka — R-K10-6 risk activation.

### SC-8 — Step 3.5 ModIntegrationPipeline regression

Commit 11 inserts Step 3.5 в ModIntegrationPipeline.UnloadMod. Если after Commit 11 `dotnet test` shows mod lifecycle test regression (existing WeakReference unload tests, capability violation tests, или ALC unload tests failing), halt.

Recovery: bisect к find which specific change caused regression; verify §9.5.1 failure semantics preserved (best-effort sequential); surface к Crystalka before continuing. **Do not commit a partial Commit 11** — atomic per Lesson #8.

### SC-9 — Tier-prefixed token backward compatibility broken

Commit 12 extends KernelCapabilityRegistry. Если existing mods using `kernel.publish:{FQN}` / `kernel.subscribe:{FQN}` tokens start failing capability validation (backward-compat aliasing к Normal tier broken), halt.

Recovery: verify alias generation logic preserves both tier-explicit и tier-implicit tokens for Normal tier events; surface если semantics ambiguity.

### SC-10 — К-L15 invariant text drift

Commit 13 lands К-L15 verbatim from spec Part 2. Если verbatim transcription differs from spec text (per Lesson #7 strict transcription requirement), halt.

Recovery: re-read spec Part 2.4 verbatim, restore exact wording, re-validate cross-references.

### SC-11 — Validation regression post-commit

Если `sync_register.ps1 --validate` exits non-zero after any К10.2 commit, halt immediately. К10.2 cascade must not introduce new validation errors per К8.34 v2.0 + CLEANUP_CASCADE + К10.1 precedents.

### SC-12 — Scope creep

Если execution encounters drift не в К10.2 scope (e.g., К10.3 pipeline issues, VULKAN_SUBSTRATE amendments, К-L7.1/L16/L17/L18/L19), halt и surface. Do not «fix while we're here» — К10.2 scope discipline per S-LOCK-1.

Per Lesson #14 candidate (provisional): pre-existing drift cleanup как separate cascade. К10.2 surface drift triages к К10.3/К10.4 sub-milestone briefs.

### SC-13 — Push-to-main classifier block (operational reminder, не halt)

Known behavior per К10.1 closure: Claude Code auto-mode classifier blocks push-to-main even с explicit instruction. Not a halt — expected. Re-confirm in-session after the work is done, then push.

При halting (SC-1..SC-12): author a HALT_REPORT в `docs/scratch/A_PRIME_7_K10_2/` (or similar scratch path), state trigger, state what was/wasn't committed, stop. **Do not commit a partial atomic commit** — atomicity protects the milestone per Lesson #8.

---

## §6 — Closure protocol (per METHODOLOGY §12.7 canonical)

After Commit 14 lands clean:

### §6.1 — Verify final state

1. `git log --oneline` shows ~14 commits added by К10.2 на feature branch `claude/k10_2-native-bus-mod-alc`
2. `git status` clean working tree
3. `sync_register.ps1 --validate` exit 0
4. `cmake --build` clean, native selftest passes (~80-90 scenarios — 58 К10.1 baseline + ~25-30 К10.2 new sections)
5. `dotnet build` clean, `dotnet test` 624+ green (К10.2 new tests additive — final count documented в closure entry)
6. К10.2 benchmarks per §5.1.A Predictions 6-10 measured + documented (results-as-measured)

### §6.2 — Update brief status + closure section

Set `status: EXECUTED` в this brief's frontmatter; add §8 closure section с commit range + date + commit ledger table + verification metrics + halt protocol activations + lesson candidates + pattern established (per К10.1 + CLEANUP_CASCADE_BRIEF §8 precedent).

Closure section template:
```markdown
## §8 — Closure (added at brief EXECUTED transition YYYY-MM-DD)

Execution closed YYYY-MM-DD by Claude Code auto-mode на branch `claude/k10_2-native-bus-mod-alc` from `main` HEAD <starting-sha>. Final commit <final-sha>.

### Commit ledger (commits <first>..<last>)

| # | Hash | Commit summary | Items closed |
|---|---|---|---|
| 1 | ... | brief authored | DOC-D-K10_2 enrollment |
| ... | ... | ... | ... |
| 14 | ... | governance closure | EVT-K10_2-CLOSURE |

### Verification metrics (final state)

- `git status`: clean working tree on branch `claude/k10_2-native-bus-mod-alc`
- `sync_register.ps1 --validate`: exit 0
- `cmake --build`: 0 warnings, 0 errors
- `dotnet build`: 0 warnings, 0 errors
- `dotnet test`: <N> passed, 0 failed (target 624+ baseline preserved + К10.2 tests additive)
- Native selftest: <N> scenarios passed (58 К10.1 baseline + К10.2 new)
- К10.2 benchmarks Predictions 6-10: <measurement results>

### Halt protocol activations
[Any SC-N halts that fired during execution + their resolution]

### Out-of-scope items deferred
- К10.3 scope: pipeline depth + display composition + hardware tier (Items 33-44)
- К10.4 scope: TLA+ formal verification (Items 18, 45, 46)
- Item 14: К11+ Core migration к native code
- Item 25: К-closure report (А'.8)

### Pattern established
[Patterns from К10.2 execution worth noting для К10.3/К10.4 briefs]

### Lesson candidates surfaced
[Anything worth bringing к К10.3 brief authoring deliberation]
```

### §6.3 — PR opening (NOT auto-push, per К10.1 precedent)

- Push branch `claude/k10_2-native-bus-mod-alc` к remote (NOT к `main`)
- Open PR titled «К10.2 — Native bus three-tier dispatch + mod ALC lifecycle (К-L15)»
- Body summarizes per-commit per-item mapping + verification metrics + halt activations (если any) + closure section
- **DO NOT auto-push к main**. Crystalka reviews + merges per established protocol

### §6.4 — Surface к Crystalka

PR ready for review. Crystalka:
1. Reviews К10.2 closure report content
2. Merges PR к `main`
3. Provides closure report к next Opus deliberation session для К10.3 brief authoring discussion

К10.3 brief authoring informed by:
- К10.2 closure report findings (halt activations, lesson candidates, patterns established)
- К10.2 architectural reality post-landing (К-L15 verbatim в KERNEL_ARCHITECTURE.md v2.1; native bus parallel infrastructure operational с managed facade preserved)
- Updated REGISTER state (К10.2 EXECUTED; К10.3 будет AUTHORED при brief authoring)

К10.3 scope context: pipeline depth (Items 33-37) + display composition (Items 38-40) + mod lifecycle quiescent state (Items 41-42) + hardware tier commitment (Items 43-44). К-L7.1 sub-invariant + К-L16/L17/L18/L19 invariants land в К10.3. Cross-document amendment surface для К10.3: VULKAN_SUBSTRATE.md (substantial), README.md (hardware requirements).

---

## §7 — Brief authority + lifecycle

**Brief authority**: К10 deliberation arc 2026-05-16 → 2026-05-17 (Crystalka + Claude Opus 4.7). 9 S-locks ratified в KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED. К10.2 standalone brief per Option III ratified 2026-05-18 (К10.1 closure precedent extended).

**Brief lifecycle (per FRAMEWORK §3.3 + §3.3.1)**:
- AUTHORED at this commit (Commit 1 of cascade)
- EXECUTED post-Commit 14 closure
- Registered в `tools/briefs/` as Tier 3 Category D per A'.4.5 governance
- AUTHORED-SKELETON → AUTHORED transition: not applicable here — К10.2 was authored fully from К10 deliberation arc closure + К10.1 closure context, не skeleton intermediate

**Brief enrollment**: К10.2 brief added к REGISTER.yaml в Commit 1 atomic с brief authoring per CLEANUP_CASCADE_BRIEF + К10.1 precedents (brief enrollment at first commit of cascade).

**Brief location**: `tools/briefs/K10_2_EXECUTION_BRIEF.md` after Crystalka copies from `/mnt/user-data/outputs/` per Filesystem MCP workaround pattern.

---

**End of brief. ~14 atomic commits across 8 К10 items + К-L15 invariant landing + cross-document amendments. Expected 14-22 hours auto-mode execution.**

К10.2 closes 8 of 46 К10 items (cumulative с К10.1: 25 of 46). Remaining 21 items distributed across К10.3 (12 items: 33-44), К10.4 (3 items: 18, 45, 46) + Item 14 deferred к К11+ + Item 25 cross-cutting к А'.8.

К10 as a whole remains AUTHORED-IN-PROGRESS until К10.4 closure. К-series formally closes only after all four К10 sub-milestones + К-closure report (А'.8).

«Halt is success, not failure» per Lesson #8 corollary. The brief's honest guarantee: bad premises surface at Phase 0 / at deep-read / at the compile gate, before they reach `main`.
