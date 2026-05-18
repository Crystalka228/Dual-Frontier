---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K10_3
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K10_3
---
---
# Brief frontmatter (not REGISTER mirror — brief lives in tools/briefs/ as Tier 3 Category D)
brief_id: K10_3_EXECUTION_BRIEF
status: AUTHORED
authored: 2026-05-18
author: Claude Opus 4.7 (Crystalka deliberation session)
target_executor: Claude Code (auto-mode + Crystalka oversight)
estimated_duration: 20-32 hours auto-mode (К10.3 scope substantial — pipeline depth + display composition + hardware tier; first sub-milestone touching Vulkan/GPU code surface substantially)
brief_type: execution
authority_chain:
  - KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED (DOC-A-KERNEL_FULL_NATIVE_SCHEDULER) — primary spec для К10.3 scope (Items 33-44)
  - K10_DELIBERATION_STATE.md (Project file sister — S8 + S-TLA + К-L invariant rationale)
  - KERNEL_ARCHITECTURE.md v2.1 LOCKED (post-К10.2) — К-L1..L11 + К-L6 SUPERSEDED + К-L12/L13/L14/L15 baseline; К10.3 amends к v2.2 adding К-L7.1 sub-invariant + К-L16 + К-L17 + К-L18 + К-L19
  - VULKAN_SUBSTRATE.md v1.0 LOCKED — V0/V1/V2 substrate primitives + L1-L10 foundational decisions; К10.3 amends к v1.1 с substantial cross-document amendments (8 sub-sections per spec Part 7.5)
  - MOD_OS_ARCHITECTURE.md v1.9 LOCKED (post-К10.2) — §3.2 tier-prefixed capabilities + §9.5 8-step chain; К10.3 amends к v1.10 с К-L17 layer capabilities + К-L18 quiescent state enforcement note + Step 3.6 V resource cleanup
  - METHODOLOGY.md v1.8 LOCKED — Lessons #7/#8/#11/#20/#22 verbatim + Provisional Lessons + §12.7 canonical closure protocol
  - FRAMEWORK.md v1.1 LOCKED — Category D Tier 3 lifecycle transitions
  - K10_1_EXECUTION_BRIEF.md EXECUTED (DOC-D-K10_1, 2026-05-18) — К10.1 closure precedent: managed-facade-preserved pattern + selftest DF_CHECK runner convention + VS-bundled CMake operational reminder
  - K10_2_EXECUTION_BRIEF.md EXECUTED (DOC-D-K10_2, 2026-05-18) — К10.2 closure precedent: 14-commit cascade с load-bearing К-L15 amendment + managed-facade-preserved continued + 665 test baseline
  - README.md — К10.3 amends с new hardware requirements section per К-L19 (К-L19 user-facing hardware tier commitment)
---

# K10.3 — Pipeline depth + display composition + mod lifecycle quiescent + hardware tier

**Brief shape**: Execution-mode brief targeting Claude Code auto-mode с Crystalka oversight. Multi-commit atomic cascade implementing **12 architectural items** from KERNEL_FULL_NATIVE_SCHEDULER.md spec (Items 33-44) that ratify **5 К-L invariants** — К-L7.1 (sub-invariant), К-L16 (simulation pipeline depth), К-L17 (display composition multi-layer), К-L18 (mod lifecycle quiescent state), К-L19 (hardware tier commitment).

**Authority**: Direct execution against К10 deliberation arc closure (2026-05-17). К10.3 is the **third of four К10 sub-milestones** under Option III standalone-briefs structure ratified 2026-05-18. К10.1 closed 2026-05-18 (PR merged, К-L6 SUPERSEDED + К-L12/L13/L14 landed). К10.2 closed 2026-05-18 (PR merged, К-L15 landed + native bus three-tier + mod ALC unload primitive). К10.4 = TLA+ formal verification, future brief.

**К10.1 + К10.2 closure context inherited** (per session logs):
- 665 test baseline preserved (620 pre-К10 + 4 К10.1 BatchedCallback + 41 К10.2 additive)
- 77 native scenarios (59 К10.1 baseline + 18 К10.2 new)
- К-L invariant series state at К10.3 entry: 16 invariants (К-L1..L11 + К-L6 SUPERSEDED + К-L12/L13/L14/L15)
- Managed-facade-preserved strategy inherited dual times (К10.1 + К10.2): native infrastructure landed parallel к managed authority; sovereign authority switch deferred к К10.4 closure or К-closure report (А'.8)
- Operational reminders: VS-bundled CMake at `D:\Visual Studio\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe`; K3-era custom DF_CHECK runner pattern (Lesson #22 «match existing convention»); Filesystem MCP `create_file` workaround pattern

**К10.3 ratifies architecturally** (per К10 deliberation closure + spec Part 2.4):
- **К-L7.1 AUTHORED** (sub-invariant к К-L7): GPU compute pipeline slot binding extends К-L7 span protocol to GPU memory
- **К-L16 AUTHORED** (Items 33-37): Simulation tick pipeline depth (D=2 default)
- **К-L17 AUTHORED** (Items 38-40): Display composition multi-layer (world + intent + combat feedback)
- **К-L18 AUTHORED** (Items 41-42): Mod lifecycle quiescent state precondition
- **К-L19 AUTHORED** (Items 43-44): Hardware tier commitment (Vulkan 1.3 + async compute queue mandate)

**К10.3 scope discipline (Lesson #20 + Lesson #14 candidate application)**:
- In-scope: 12 К10 items establishing pipeline depth + display + quiescent state + hardware tier (per spec §3.10-3.13)
- Out-of-scope: К10.4 (TLA+ formal verification) — separate brief
- Out-of-scope: Item 14 (К11+ Core migration) — explicitly deferred per spec §3.4
- Out-of-scope: Item 25 (К-closure report) — cross-cutting к А'.8

**Strategic pattern inherited from К10.1 + К10.2**: К10.3 implementations land **managed-facade-preserved** где applicable (К-L18 mod lifecycle quiescent state ratified architecturally + enforcement в `df_scheduler_unload_mod_native_state` Item 32 К10.2 precondition check; UI/settings menu integration deferred к К-closure). К-L16 pipeline depth substantial implementation — pipeline slots actually allocated и operational; К-L17 display composition substantial — layer registry operational; К-L19 hardware capability check substantial — startup fail-fast logic operational.

**Key architectural difference от К10.1/К10.2**: К10.3 imposes **first substantial Vulkan/GPU code surface** на К10 cascade. К10.1 был чистый kernel/managed work. К10.2 был чистый kernel/bus work. К10.3 включает Vulkan instance lifecycle modification (К-L19 queue family selection), async compute queue submission paths (Item 35 Phase.Compute), pipeline slot fence orchestration (Item 33), display composition multi-layer rendering paths (Items 38-40). Phase 0 reads должны specifically cover VULKAN_SUBSTRATE.md sections + native Vulkan code anchors.

**Brief size note**: К10.3 brief substantially larger than К10.1/К10.2 (estimated 1500-1800 lines vs Profile B 900-1200 target) per К-L14 default-inclusion bias — К10.3 cross-document amendment scope substantial (VULKAN_SUBSTRATE.md 8 sub-section amendments + MOD_OS amendments + README.md new section) + 5 К-L invariants vs К10.2's single К-L15. Brief size growth proportional к architectural complexity per К-L14.

---

## §1 — Crystalka ratified scope locks (К10 deliberation 2026-05-16..2026-05-17, applicable subset для К10.3)

### §1.1 — S-LOCK-1: К10.3 scope items (12 items)

**LOCK**: К10.3 implements exactly these 12 items from KERNEL_FULL_NATIVE_SCHEDULER.md spec, in dependency order:

| Item | Title | Spec § | Group |
|---|---|---|---|
| 43 | Async compute queue mandate | §3.13 | Hardware tier |
| 44 | Hardware capability check at startup | §3.13 | Hardware tier |
| 33 | Tick pipeline depth mechanism (S8-Q1 + S8-Q2) | §3.10 | Pipeline depth |
| 34 | Pipeline drain/refill protocols | §3.10 | Pipeline depth |
| 35 | Phase.Compute scheduler integration | §3.10 | Pipeline depth |
| 36 | Pipeline slot read API (S8-Q2 Pattern C) | §3.10 | Pipeline depth |
| 37 | Filter primitive integration с pipeline slot transitions | §3.10 | Pipeline depth |
| 38 | Display composition framework | §3.11 | Display composition |
| 39 | Intent overlay layer infrastructure | §3.11 | Display composition |
| 40 | Combat feedback layer infrastructure | §3.11 | Display composition |
| 41 | К-L18 quiescent state enforcement | §3.12 | Mod lifecycle quiescent |
| 42 | Settings menu / mod management UI integration с К10 | §3.12 | Mod lifecycle quiescent |

**Item ordering rationale**:
- **Items 43-44 first** (hardware tier) — async compute queue capability detection foundation; pipeline depth (Items 33+) requires async compute queue к exist; build cascade на fundamental hardware capability
- **Items 33-37 (pipeline depth)** middle — К-L16/L7.1 cascade builds после hardware capability; Items 33→34→35→36→37 ordered by dependency (slot mechanism → drain/refill → Phase.Compute integration → slot read API → filter integration)
- **Items 38-40 (display composition)** — К-L17 cascade builds после pipeline depth (Items 38-40 depend на pipeline slot tail reads)
- **Items 41-42 (mod lifecycle)** last — К-L18 quiescent state precondition consumes Items 33 (pipeline slot state) + Items 41 reads pipeline state to verify quiescence

### §1.2 — S-LOCK-2: К-L invariants landing strategy (Approach C grouped)

**LOCK** (per Crystalka ratification 2026-05-18): **Approach C grouped landing**. 4 load-bearing commits для 5 К-L invariants:

1. **К-L19 commit** (after Items 43-44) — hardware tier commitment landed first per dependency order
2. **К-L7.1 + К-L16 combined commit** (after Items 33-37) — К-L7.1 sub-invariant и К-L16 pipeline depth tied к одной physical reality (GPU pipeline slots); landing them вместе preserves К-L7 invariant integrity per Lesson #11 (architectural reduction methodology)
3. **К-L17 commit** (after Items 38-40) — display composition multi-layer
4. **К-L18 commit** (after Items 41-42) — mod lifecycle quiescent state

**Rationale**:
- Approach C balances atomicity (per Lesson #8) с attribution clarity (per Lesson #11 reduction methodology)
- К-L7.1 + К-L16 combined: same physical reality (GPU pipeline slots); structural tie justifies grouped landing
- К-L17 separate: display composition concern independent от pipeline mechanism
- К-L18 separate: mod lifecycle concern independent от GPU pipeline mechanism
- К-L19 separate: hardware tier mandate foundational, lands first per dependency order

### §1.3 — S-LOCK-3: К-L7.1 sub-invariant landing pattern (mirror К-L3.1 precedent)

**LOCK**: К-L7.1 lands as **sub-invariant к К-L7** (mirror К-L3.1 sub-invariant к К-L3 precedent established 2026-05-10). К-L invariant table treatment:

```
| К-L7 (+К-L7.1) | LOCKED (К-L7.1 added К10.3) | Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1) |
```

К-L7.1 text (verbatim from spec Part 2.4):

> «К-L7.1: GPU compute writes to RawTileField storage are bound к pipeline slot (К-L16). Sim-thread reads of compute-managed fields see slot-tail state — sim tick T+D reads dispatched-at-(T+D-1) state. One-tick lag from sim-perspective bounded и deterministic. К-L7 atomic-from-observer invariant preserved within pipeline slot boundary; cross-slot reads see different snapshots.»

**К-L7.1 preserves К-L7** — К-L7 atomic-from-observer invariant remains authoritative; К-L7.1 specifies extension к GPU memory domain. Mirror К-L3.1 pattern (К-L3 preserved, К-L3.1 sub specifies Path α + Path β coexistence).

### §1.4 — S-LOCK-4: К-L16 pipeline depth (D=2 default)

**LOCK** (per S8-Q1 + S8-Q2): Configurable depth D = 1-3 (default 2). Simulation thread runs D ticks ahead of display thread. Cross-layer async operations (GPU compute, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread.

К-L16 text (verbatim from spec Part 2.4):

> «К-L16: Simulation tick pipeline depth D ≥ 1 (configurable 1-3, default 2). Simulation thread runs D ticks ahead of display thread. Cross-layer async operations (GPU compute, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread. Display thread reads results from simulation tick (CurrentSimTick - D). Pipeline drain orderly at save/pause; pipeline refill orderly at load/resume. К-L16 establishes display latency invariant (D × tick_period); К-L15 fast tier latency invariant (subscriber response ≤1ms) preserved independently — К-L15 governs publish-to-subscribe response time, К-L16 governs sim-to-display visibility latency.»

Pipeline slot data model (per S8-Q2 amendment + spec §3.10 Item 33):

```cpp
struct PipelineSlot {
    uint64_t sim_tick;
    NativeWorld snapshot;
    FieldStorageSnapshot fields;        // S8-Q2: K-L7.1 binding
    FenceHandle compute_fence;
    enum { Dispatched, FenceCompleted, ReadableAsTail } state;
};
```

### §1.5 — S-LOCK-5: К-L17 display composition multi-layer

**LOCK** (per S8-Q3): Three-layer composition framework.

Layer taxonomy (per К-L17 + spec §3.11):

| Layer | Source | Latency | Mutating? |
|---|---|---|---|
| Sim state | Pipeline slot tail (К-L16) | D × tick_period | Yes (sim writes) |
| Intent overlay | Current input state | ≤16ms (60 FPS) | No (read-only) |
| Combat feedback | Fast tier consumers (К-L15) | ≤1ms + ≤16ms | No (read-only) |
| Static | Loaded assets | N/A | No |

К-L17 text (verbatim from spec Part 2.4):

> «К-L17: Display tick T composites multi-layer state where layers carry independent latency contracts. Sim state layers read from pipeline slot tail (К-L16 governed, D-tick lag). Intent overlay layer reads from current input state (sub-tick latency, не sim-mutating). Combat feedback layer reads from Fast tier event consumers (К-L15 latency). Composition order: sim state layers rendered first, intent and combat overlays composited on top. Layer separation maintains К-L16 invariant (sim state pipeline-bound) while enabling sub-pipeline-latency feedback channels для player intent surfaces. К-L15 fast tier subscriber latency, К-L16 sim-to-display latency preserved as independent invariants; К-L17 establishes display-composition latency invariant (intent layer ≤1 display frame ≈16ms at 60 FPS).»

### §1.6 — S-LOCK-6: К-L18 mod lifecycle quiescent state

**LOCK** (per S8-Q4): Pattern (b) delegate — mod lifecycle transitions require sim paused; scheduler delegate signals quiescent achieved; transition proceeds only post-signal.

К-L18 text (verbatim from spec Part 2.4):

> «К-L18: Mod load/unload operations require simulation paused state. Simulation thread не active during mod lifecycle changes; pipeline slots quiescent (all fences completed); no concurrent compute dispatches in-flight. Mod management UI (settings menu, hot reload tooling) enforces pause precondition. Hot reload preserves game state through managed dependency graph swap mechanism, но simulation pause required для consistent mod transition. К-L16 pipeline depth invariant naturally drains under pause; К-L17 multi-layer composition pauses with sim state. Mod lifecycle coordination protocol simplified by quiescent state precondition.»

**К10.2 forward dependency consumed**: К10.2 Item 32 `df_scheduler_unload_mod_native_state` already includes quiescent state precondition check (returns error если `sim_state != Paused`). К10.3 Item 41 extends this к include pipeline slot quiescence verification (all D slots в `ReadableAsTail` state or empty); Item 42 lands UI integration enforcing precondition.

### §1.7 — S-LOCK-7: К-L19 hardware tier commitment

**LOCK** (per S8-Q5): Vulkan 1.3 + async compute queue family mandate. Hardware tier explicit commitment.

К-L19 text (verbatim from spec Part 2.4):

> «К-L19: Dual Frontier requires Vulkan 1.3 hardware с async compute queue family support. Target hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20xx и newer) / AMD RDNA 1+ (RX 5000 и newer) / Intel Arc Alchemist+. Async compute queue used для К-L16 pipeline depth dispatches. Graphics queue used для display rendering. Copy/transfer queue used для asset transfers. К10 mandates queue family availability at startup; failure to detect async compute queue is fail-fast condition с clear hardware requirement diagnostic message. Hardware exclusion of pre-Turing NVIDIA, pre-RDNA AMD, pre-Arc Intel, и most integrated GPUs accepted as architectural choice supporting clean implementation. Project release timeline accounts for hardware proliferation — by Dual Frontier release, target hardware tier represents majority of gaming hardware.»

### §1.8 — S-LOCK-8: Atomic cascade (multi-commit ordered) + managed-facade-preserved

**LOCK**: К10.3 executes as multi-commit cascade analogous к К10.1 + К10.2 pattern. Per Lesson #8: К10.3 items 33-44 have **clean intermediate states** because:
1. Hardware tier (Items 43-44) lands first — capability detection foundation; existing initialization paths unchanged when async compute queue available
2. Pipeline depth infrastructure (Items 33-37) lands additive — pipeline slots allocated и operational, но display thread continues к read most-recent state (slot tail = current при D=1 backward-compat mode for transition)
3. Display composition (Items 38-40) lands additive — layer registry operational; existing rendering path preserved as Sim state layer; intent/combat feedback layers opt-in для new use cases
4. Mod lifecycle quiescent state (Items 41-42) lands incremental — К10.2 Item 32 already includes quiescent precondition; К10.3 Item 41 extends с pipeline state verification; Item 42 lands UI integration
5. К-L invariant landings happen incrementally per Approach C grouped (4 load-bearing commits)

Tests pass at every commit (existing 665 К10.2 baseline preserved; new К10.3 tests additive).

**Strategic note**: К10.3 introduces **first substantial Vulkan code surface** в К10 cascade. Phase 0 reads должны verify VULKAN_SUBSTRATE.md state + native Vulkan code anchors. Drift between brief authoring (2026-05-18) и execution time может surface новые halt classes specific к Vulkan integration (SC-N additions per §5).

### §1.9 — S-LOCK-9: К10.3 closes К10.3 only

**LOCK**: К10.3 closure is sub-milestone closure (per FRAMEWORK §3.3 Lifecycle EXECUTED для К10.3 brief). К-closure report (А'.8) waits для all four К10 sub-milestones closed (К10.1 ✅ + К10.2 ✅ + К10.3 pending + К10.4 pending).

**Implications**:
- К10.3 commit message scope: `feat(kernel): K10.3 — pipeline depth + display composition + quiescent + hardware tier (К-L7.1/L16/L17/L18/L19)`
- К10.3 closure REGISTER entry: separate audit_trail event (`EVT-{date}-K10_3-CLOSURE`)
- К-L invariant landings: К10.1 landed К-L6 SUPERSEDED + К-L12/13/14; К10.2 landed К-L15; К10.3 lands К-L7.1 + К-L16 + К-L17 + К-L18 + К-L19; К10.4 has no new К-L (TLA+ verification spec only)

---
## §2 — Phase 0 — Pre-flight reads (mandatory before any edit)

Per Lesson #7 (transcribe API verbatim) + Lesson #22 (read existing code before mechanism design), executor MUST complete every read listed below **before writing a single line of К10.3 code**. К10.3 brief authored 2026-05-18 from point-in-time read; drift between brief authoring и execution time is **expected**, surfaces as halt triggers (SC-3) — never silent improvisation per Lesson #20.

К10.3 Phase 0 reads expanded vs К10.1/К10.2 — К10.3 imposes **first substantial Vulkan code surface** на К10 cascade; Vulkan-specific reads required.

### §2.1 — Verify post-К10.2 closure state (hard gates)

Read и verify (hard gates, blocking commits если failed):

1. `git log --oneline -20` on `main` — confirm:
   - К10.2 PR merged (14 commits per К10.2 closure report)
   - HEAD references К10.2 closure commit
   - Halt если К10.2 closure не reached — К10.3 starts от post-К10.2 main

2. `git status` — working tree clean before execution starts. **Hard gate** per К8.34 v2.0 + К10.1 + К10.2 precedents.

3. `docs/governance/REGISTER.yaml` head check — confirm `register_version` ≥ post-К10.2 closure baseline (~2.0+ based on К10.2 closure cadence). Lower means К10.3 не based on correct baseline.

4. `tools/governance/sync_register.ps1 --validate` — exit 0 required as baseline.

5. `dotnet build DualFrontier.sln` — clean baseline.

6. `dotnet test DualFrontier.sln` — baseline pass count: **665 tests green** (per К10.2 closure metrics: 624 К10.1 baseline + 41 К10.2 additive). Если suite fails или count diverges (excluding intentional К10.3 additions), halt.

7. `cmake --build native/DualFrontier.Core.Native` через VS-bundled cmake — clean baseline. Native selftest passes ~77 scenarios (59 К10.1 + 18 К10.2). К10.3 brief authored expecting 77; informational drift recording если actual differs (per К10.2 Phase 0 precedent — 59 actual vs 58 expected recorded informational, не halt).

### §2.2 — Read KERNEL_FULL_NATIVE_SCHEDULER.md spec (К10.3 sections)

Read в полном объёме, identify exact line numbers для К10.3 items:
- Part 2.4 К-L7.1 + К-L16 + К-L17 + К-L18 + К-L19 verbatim text
- Part 3.10 Items 33-37 (pipeline depth)
- Part 3.11 Items 38-40 (display composition)
- Part 3.12 Items 41-42 (mod lifecycle quiescent state)
- Part 3.13 Items 43-44 (hardware tier)
- Part 5.1.A Predictions 11-17 (К10.3 measurable scope)
- Part 7.5 VULKAN_SUBSTRATE.md amendment list (8 sub-section amendments)
- Part 7.9 README.md amendment scope (hardware requirements section)
- Part 8 risks R-K10-8 (hardware tier exclusion), R-K10-9 (TLA+ state space — informational, К10.4 scope)

**Per Lesson #7**: spec verbatim authoritative для К-L invariant text, PipelineSlot struct shape, three-layer composition framework taxonomy, queue family selection requirements.

### §2.3 — Read VULKAN_SUBSTRATE.md (К10.3 amendment surface)

Read в полном объёме (или targeted sections per spec Part 7.5 amendment list):
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.0 LOCKED — 97 KB substantial:
  - **§0 L1-L10** foundational decisions (К10.3 extends L1 с async compute + L7 hardware tier baseline confirmation)
  - **§2 architecture** (К10.3 amends с display composition section)
  - **§2.3 threading model** (К10.3 substantial amendment — pipeline depth + async compute queue + sim-thread read pattern)
  - **§3.4 native compute dispatch** (К10.3 amendment — `df_vulkan_unload_mod_resources` C ABI + К-L19 mandate)
  - **§4 rendering use case** (К10.3 amendment — display composition section)
  - **§5.5 Mode C navigation** (К10.3 amendment — visibility latency через К-L17 composition)
  - **§7.2 determinism** (К10.3 amendment — pipeline drain semantics К-L16)
  - **§7.3 async sync hazards** (К10.3 amendment — pipeline slot tail read pattern К-L7.1)

**Per Lesson #7**: VULKAN_SUBSTRATE.md verbatim sections preserved literally; К10.3 amendments inserted preserving existing wording style.

### §2.4 — Read code anchors verbatim (К10.3 Vulkan-specific)

Read эти файлы для verbatim content к understand Vulkan surface (Lesson #22):

**Vulkan instance + device + queue setup**:
- `src/DualFrontier.Vulkan/` (or wherever Vulkan substrate code lives — verify path при Phase 0) — Vulkan instance creation, physical device enumeration, queue family selection
- `src/DualFrontier.Vulkan/VulkanInstance.cs` (или native equivalent) — `vkCreateInstance` setup
- `src/DualFrontier.Vulkan/PhysicalDeviceSelection.cs` (или equivalent) — `vkEnumeratePhysicalDevices` + queue family enumeration
- `src/DualFrontier.Vulkan/QueueFamilySelection.cs` (или equivalent) — `vkGetPhysicalDeviceQueueFamilyProperties` consumer
- `native/DualFrontier.Vulkan.Native/` (если exists) — native Vulkan code

**Note (per Lesson #22)**: К10.3 brief assumes Vulkan code layout — actual layout verified at Phase 0; if substantially different (e.g., Silk.NET wrapper used vs pure P/Invoke; or Vulkan code lives in different project), brief amendments via SC-3 halt path.

**К10 scheduler integration points**:
- `native/DualFrontier.Core.Native/include/scheduler.h` (post-К10.2) — extended с Phase.Compute integration (Item 35)
- `native/DualFrontier.Core.Native/include/bus_native.h` (К10.2) — extended consumer of К-L18 quiescent state check
- `native/DualFrontier.Core.Native/include/mod_unload.h` (К10.2 Item 32) — extended с pipeline state verification (Item 41)

**Application-side wiring**:
- `src/DualFrontier.Application/Loop/GameBootstrap.cs` — sim/display thread separation; К10.3 amends с pipeline slot allocation
- `src/DualFrontier.Application/Loop/GameLoop.cs` — sim tick / display tick coordination
- Display rendering path (где is it?) — К10.3 amends с layer composition

**FieldStorage + persistence** (К-L7.1 implications):
- `src/DualFrontier.Core/Fields/RawTileField.cs` (или equivalent) — К-L7 span protocol extension к К-L7.1
- `src/DualFrontier.Persistence/` — pipeline slot snapshot serialization (К-L16, save-integrated per S8-Q1.5)

**Test fixtures (post-К10.2)**:
- `tests/DualFrontier.Core.Tests/` — К10.1/К10.2 scheduler tests
- `tests/DualFrontier.Vulkan.Tests/` (или equivalent) — К10.3 Vulkan tests would extend
- `tests/DualFrontier.Application.Tests/` — integration tests

**Если any read surfaces a contradiction с this brief** — halt per SC-3. К10.3 Vulkan-specific анchors имеют **наибольший drift risk** since К10.1/К10.2 не trogали Vulkan code; К10.3 brief authored from spec assumption, не from verified-recent code reads.

### §2.5 — Read MOD_OS_ARCHITECTURE.md §9.5 + §3.2 (К10.2 state)

Read post-К10.2 state:
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.9 LOCKED (post-К10.2):
  - §3.2 tier-prefixed capability syntax (К10.2 amendment) — К10.3 extends с К-L17 layer capability tokens
  - §4 IModApi v3 strict (extends с layer registration capabilities К10.3)
  - §9.5 8-step unload chain (К10.2 amendment с Step 3.5) — К10.3 adds Step 3.6 V resource cleanup
  - §11.2 ValidationErrorKind enum — К10.3 adds new kinds (К-L17 + К-L18 specific)

**Per Lesson #7**: MOD_OS §9.5 K10.2 amended state preserved verbatim; К10.3 Step 3.6 insertion text matches existing wording style.

### §2.6 — Read README.md (К-L19 amendment surface)

Read:
- `README.md` — К10.3 lands NEW «Hardware Requirements» section (К-L19). Identify appropriate placement (likely between «Overview» и «Installation» sections or analogous).

К-L19 hardware tier publication к README.md is **user-facing** documentation surface — К10.3 introduces hardware exclusion choice к public-visible documentation.

### §2.7 — Read REGISTER.yaml К10.3 enrollment area

Identify exact line ranges:
- DOC-A-KERNEL (KERNEL_ARCHITECTURE.md v2.1 post-К10.2) — К10.3 amends к v2.2 (5 К-L invariants added — К-L7.1 sub + К-L16/L17/L18/L19)
- DOC-A-VULKAN_SUBSTRATE (VULKAN_SUBSTRATE.md v1.0 LOCKED) — К10.3 amends к v1.1 (8 sub-section amendments per spec Part 7.5)
- DOC-A-MOD_OS (MOD_OS_ARCHITECTURE.md v1.9 post-К10.2) — К10.3 amends к v1.10 (К-L17 layer caps + К-L18 quiescent note + Step 3.6)
- DOC-A-KERNEL_FULL_NATIVE_SCHEDULER (К10 spec v2.0 LOCKED) — governance_events append для К10.3
- DOC-G-README (README.md) — К10.3 amends с hardware requirements section
- DOC-D-K10_2 (К10.2 brief EXECUTED) — К10.3 brief enrollment after this entry
- Audit_trail events list (К10.3 adds EVT-{date}-K10_3-CLOSURE)

### §2.8 — Halt category clarity

**Hard gates (STOP-eligible)** per §2.1 + К10.1 + К10.2 precedents:
- Working tree dirty
- Baseline tests failing (excluding intentional changes)
- `sync_register.ps1 --validate` non-zero baseline
- Build failure baseline
- К10.2 closure не reached

**Vulkan-specific hard gates** (К10.3 first introduction):
- `vulkan-1.dll` отсутствует at expected location (hardware capability detection prerequisite)
- `glslangValidator.exe` отсутствует (shader compilation prerequisite per VULKAN_SUBSTRATE §0 L6)
- Vulkan project code anchors substantially differ from brief assumptions (Lesson #22 application — match existing convention)

**Informational checks (record-only)**:
- HEAD SHAs, branch topology
- Native test scenario count (К10.2 baseline ~77, actual recorded в Commit messages)
- Vulkan code surface layout details (Phase 0.4 inventory as hypothesis)

Если informational check diverges from brief expectation — **record divergence в Commit message, continue**. Hard gate failure → halt per SC-N (§5).

---

## §3 — Atomic commit cascade (target ~16-18 commits)

Each commit: individually compilable + register-valid intermediate state per Lesson #8. `sync_register.ps1 --validate` exit 0 at every governance-touching commit; `dotnet build` clean + `cmake --build` clean at every code-touching commit; `dotnet test` 665+ passing at every commit (К10.2 baseline preserved; new К10.3 tests additive).

**Cascade strategy** per S-LOCK-8 + К10.1/К10.2 precedent: hardware tier foundation first (Items 43-44), pipeline depth infrastructure middle (Items 33-37), display composition (Items 38-40), mod lifecycle (Items 41-42). 4 К-L invariant landing commits distributed throughout (К-L19 after Items 43-44; К-L7.1 + К-L16 после Items 33-37; К-L17 после Items 38-40; К-L18 после Items 41-42) per Approach C grouped landing.

### Commit 1 — Brief authoring commit (К10.3 brief enrollment)

**Files**:
- `tools/briefs/K10_3_EXECUTION_BRIEF.md` (this brief)
- `docs/governance/REGISTER.yaml` (DOC-D-K10_3 entry с `lifecycle: AUTHORED`, `category: D`, `tier: 3`)

**Validation**:
- `sync_register.ps1 --validate` exit 0
- No code changes

**Commit message**: `docs(briefs): K10.3 brief authored — pipeline depth + display composition + quiescent + hardware tier (К-L7.1/L16/L17/L18/L19)`

### Commit 2 — Phase 0 verification + native test scaffold

**Files**:
- `native/DualFrontier.Core.Native/test/selftest.cpp` (extended с К10.3 placeholder section — empty section ready)

**Rationale per Lesson #22**: К10.3 native test sections added incrementally; existing DF_CHECK runner pattern preserved.

**Validation**:
- `cmake --build` clean
- `cmake --build --target run_native_tests` — 77+ К10.2 scenarios pass (К10.3 placeholder vacuous)
- `dotnet build` clean
- `dotnet test` 665+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(native-test): K10.3 — native test scaffold preserved (DF_CHECK runner extension)`

### Commit 3 — Item 43: Async compute queue mandate (foundation)

**Files**:
- `src/DualFrontier.Vulkan/QueueFamilySelection.cs` (modified — async compute queue family selection logic added)
- `src/DualFrontier.Vulkan/VulkanInstance.cs` (modified — async compute queue handle stored)
- `tests/DualFrontier.Vulkan.Tests/QueueFamilySelectionTests.cs` (new или extended — async compute queue selection coverage)

**Drift surface**: Vulkan substrate gains async compute queue selection requirement. Existing graphics queue selection preserved; new async compute queue selection runs parallel. Failure handled at Item 44 (Commit 4).

**Implementation surface (per spec §3.13 Item 43)**:

```csharp
// QueueFamilySelection.cs amendment
public sealed class QueueFamilySelection
{
    // Existing graphics queue selection preserved
    public uint GraphicsQueueFamilyIndex { get; private set; }
    
    // К10.3 Item 43: async compute queue mandate
    public uint? AsyncComputeQueueFamilyIndex { get; private set; }
    
    // Copy/transfer queue (К-L19 part of three-queue architecture)
    public uint? TransferQueueFamilyIndex { get; private set; }
    
    public void EnumerateQueueFamilies(VkPhysicalDevice physicalDevice)
    {
        // Existing vkGetPhysicalDeviceQueueFamilyProperties call preserved
        var queueFamilies = GetQueueFamilyProperties(physicalDevice);
        
        // Existing graphics queue family selection preserved
        for (uint i = 0; i < queueFamilies.Length; i++)
        {
            if ((queueFamilies[i].queueFlags & VkQueueFlags.GraphicsBit) != 0)
            {
                GraphicsQueueFamilyIndex = i;
                break;
            }
        }
        
        // К10.3 Item 43: async compute queue selection (NEW)
        // Prefer dedicated async compute queue family (compute-only, no graphics)
        // Fallback к compute-capable graphics queue family
        for (uint i = 0; i < queueFamilies.Length; i++)
        {
            var flags = queueFamilies[i].queueFlags;
            if ((flags & VkQueueFlags.ComputeBit) != 0 
                && (flags & VkQueueFlags.GraphicsBit) == 0)
            {
                // Dedicated async compute queue family — К-L19 ideal
                AsyncComputeQueueFamilyIndex = i;
                break;
            }
        }
        
        // К10.3 Item 43: transfer queue selection (NEW per К-L19 three-queue architecture)
        for (uint i = 0; i < queueFamilies.Length; i++)
        {
            var flags = queueFamilies[i].queueFlags;
            if ((flags & VkQueueFlags.TransferBit) != 0 
                && (flags & VkQueueFlags.GraphicsBit) == 0
                && (flags & VkQueueFlags.ComputeBit) == 0)
            {
                TransferQueueFamilyIndex = i;
                break;
            }
        }
    }
}
```

**Per Lesson #22**: actual implementation matches existing convention в DualFrontier.Vulkan project. Если pure P/Invoke к vulkan-1.dll: вышеуказанная shape applies. Если Silk.NET wrapper used: adapt к Silk.NET API. Phase 0 read determines.

**К10.3 boundary**: К-L19 mandate (Item 44 next commit) enforces presence; Item 43 lands selection logic.

**Validation**:
- `dotnet build` clean
- `dotnet test` 665+ green
- `QueueFamilySelectionTests` extended: async compute queue selection on supported hardware; fallback behavior if absent (test mocks GPU enumeration)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(vulkan): K10.3 Item 43 — async compute queue selection (К-L19 foundation)`

### Commit 4 — Item 44: Hardware capability check at startup

**Files**:
- `src/DualFrontier.Vulkan/HardwareCapabilityCheck.cs` (new — startup fail-fast logic)
- `src/DualFrontier.Application/Loop/GameBootstrap.cs` (modified — invokes HardwareCapabilityCheck.Verify at startup)
- `tests/DualFrontier.Vulkan.Tests/HardwareCapabilityCheckTests.cs` (new)

**Drift surface**: Vulkan substrate gains startup-time hardware capability gate. Игра отказывается стартовать если К-L19 hardware tier не satisfied; fail-fast диагностическое сообщение clearly states requirement.

**Implementation surface (per spec §3.13 Item 44 + R-K10-8 risk mitigation)**:

```csharp
// HardwareCapabilityCheck.cs
public static class HardwareCapabilityCheck
{
    public sealed class HardwareCapabilityException : Exception
    {
        public HardwareCapabilityException(string message) : base(message) { }
    }
    
    public static void Verify(VulkanInstance vulkan, QueueFamilySelection queues)
    {
        // К-L19 mandate: async compute queue family availability
        if (queues.AsyncComputeQueueFamilyIndex == null)
        {
            throw new HardwareCapabilityException(
                "Dual Frontier requires Vulkan 1.3 hardware с dedicated async compute queue " +
                "family support. Detected hardware does not expose async compute queue family. " +
                "Required hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20xx или newer), " +
                "AMD RDNA 1+ (RX 5500 или newer), Intel Arc Alchemist+ (A380 или newer). " +
                "See README.md hardware requirements section."
            );
        }
        
        // Vulkan 1.3 API version check (К-L19 mandate)
        if (vulkan.InstanceApiVersion < new VkVersion(1, 3, 0))
        {
            throw new HardwareCapabilityException(
                "Dual Frontier requires Vulkan 1.3 API version. Detected version: " +
                $"{vulkan.InstanceApiVersion}. Upgrade GPU driver или install Vulkan 1.3 capable hardware."
            );
        }
    }
}
```

**Per К-L19 + spec §3.13 Item 44**: fail-fast at startup, не graceful degradation. К-L19 hardware tier exclusion is architectural commitment supporting clean implementation. R-K10-8 risk (hardware tier exclusion) accepted as architectural choice.

**GameBootstrap.cs integration**:
```csharp
// GameBootstrap.cs CreateLoop excerpt — К10.3 form
public GameLoop CreateLoop(/* ... */)
{
    // Existing Vulkan instance + device creation preserved
    var vulkan = new VulkanInstance(/* ... */);
    var queues = new QueueFamilySelection();
    queues.EnumerateQueueFamilies(vulkan.PhysicalDevice);
    
    // К10.3 Item 44: hardware capability check (NEW)
    HardwareCapabilityCheck.Verify(vulkan, queues);
    
    // Existing scheduler + system registration preserved
    // ...
}
```

**Validation**:
- `dotnet build` clean
- `dotnet test` 665+ green
- `HardwareCapabilityCheckTests`: positive case (mock с async compute queue → success); negative case (mock без async compute queue → HardwareCapabilityException with correct message); Vulkan 1.3 version check
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(vulkan): K10.3 Item 44 — hardware capability check at startup (К-L19 fail-fast)`

### Commit 5 — К-L19 invariant landing + README.md hardware section (load-bearing 1 of 4)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.1 → v2.2 (К-L invariant table updated — К-L19 row added; Part 2 K10 row updated с К-L19 ratified)
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.0 → v1.1 partial (§0 L1 decision extends с async compute requirement; §0 L7 hardware tier baseline note; §3.4 native compute dispatch mandate documentation)
- `README.md` (NEW «Hardware Requirements» section added per К-L19 user-facing publication)
- `docs/governance/REGISTER.yaml` (DOC-A-KERNEL version 2.1 → 2.2 partial — К-L19 row only; DOC-A-VULKAN_SUBSTRATE version 1.0 → 1.1 partial; DOC-G-README amended; governance_events append references к К10.3 closure)

**This is the first load-bearing commit of К10.3** (К-L19 amendment landing). К-L19 architectural commitment landed; subsequent commits (Items 33+) build on hardware tier foundation.

**Drift surface (substantial — first К10.3 К-L landing)**:
- К-L19 added к KERNEL_ARCHITECTURE.md Part 0 К-L table verbatim from spec Part 2.4
- KERNEL_ARCHITECTURE.md Part 2 master plan: К10 row updated с К-L19 ratified annotation (К-L7.1/L16/L17/L18 still pending)
- VULKAN_SUBSTRATE.md §0 L1 amended: «Vulkan 1.3 + async compute queue family requirement (К-L19 К10.3 amendment)»
- VULKAN_SUBSTRATE.md §0 L7 amended: «Windows-only initial platform matches К-L19 hardware tier baseline»
- VULKAN_SUBSTRATE.md §3.4 amended: native compute dispatch к dedicated async compute queue mandate (К-L19 К10.3 amendment)
- README.md NEW section: «Hardware Requirements» с target hardware tier published к user-visible documentation

**К-L19 invariant text (verbatim per Lesson #7)**: see §1.7 above.

**README.md amendment text** (new section):

```markdown
## Hardware Requirements

Dual Frontier requires Vulkan 1.3 hardware с dedicated async compute queue family support.

**Minimum supported hardware tier**:
- NVIDIA Turing+ (GTX 16xx series или RTX 20xx series и newer)
- AMD RDNA 1+ (RX 5500 series и newer)
- Intel Arc Alchemist+ (A380 series и newer)

**Why this tier**: Dual Frontier uses pipeline depth GPU compute for simulation parallelism per К-L16/L19. Pre-Turing NVIDIA, pre-RDNA AMD, и most integrated GPUs do не expose async compute queue family; они cannot run Dual Frontier с required performance characteristics.

**Diagnostic**: Game fails fast at startup с clear hardware requirement message if your hardware tier не satisfied. See `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` К-L19 for architectural rationale.
```

**Per Lesson #8 (atomic compilable unit)**: К-L19 invariant text + KERNEL_ARCHITECTURE amendment + VULKAN_SUBSTRATE.md partial amendment + README.md new section + REGISTER updates land в **same commit**. Splitting leaves intermediate state where К-L table claims «К-L19 in effect» но README.md doesn't document hardware tier (inconsistent doc state).

**Validation**:
- `dotnet build` clean
- `dotnet test` 665+ green
- К-L invariant cross-reference integrity: KERNEL_ARCHITECTURE.md К-L19 references KERNEL_FULL_NATIVE_SCHEDULER.md Part 2.4 verbatim
- README.md amendment matches К-L19 wording
- `sync_register.ps1 --validate` exit 0

**Commit message**:
```
feat(architecture): K10.3 — К-L19 hardware tier amendment + README hardware requirements

The first load-bearing commit of К10.3 (К-L19 amendment landing).
К-L19 «Hardware tier commitment» added к KERNEL_ARCHITECTURE.md v2.1 → v2.2.
К-L invariant table extended с К-L19 row.

VULKAN_SUBSTRATE.md v1.0 → v1.1 partial amendments:
- §0 L1 extended с async compute queue requirement (К-L19)
- §0 L7 hardware tier baseline note
- §3.4 native compute dispatch mandate documentation

README.md NEW «Hardware Requirements» section publishes К-L19 user-facing.

К-L19 text: «Dual Frontier requires Vulkan 1.3 hardware с async compute queue family
support. Target hardware tier: NVIDIA Turing+ / AMD RDNA 1+ / Intel Arc Alchemist+.»

Build clean; 665 tests green.

Phase 5 of K10.3 cascade. Commit 5 of <total>.
```

### Commit 6 — Item 33: Tick pipeline depth mechanism (S8-Q1 + S8-Q2)

**Files**:
- `native/DualFrontier.Core.Native/include/pipeline_slot.h` (new)
- `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (new — PipelineSlot allocation, state machine, fence orchestration)
- `native/DualFrontier.Core.Native/test/pipeline_slot_test.cpp` (new selftest section — D=2 default coverage)
- `src/DualFrontier.Core.Interop/PipelineSlotInterop.cs` (new managed binding)

**Drift surface**: К-L16 pipeline depth infrastructure landed. К-L7.1 sub-invariant landed parallel в same commit grouping (per Approach C — К-L7.1 + К-L16 share physical reality «GPU pipeline slots»). Sim thread continues к dispatch normally; pipeline slot allocation operational но fence orchestration runs in parallel mode (display still reads most-recent state when D=1 backward-compat, transitions к D=2 при К-L16 amendment commit).

**Implementation surface (per spec §3.10 Item 33 + S8-Q1 + S8-Q2 verbatim)**:

```cpp
// pipeline_slot.h verbatim from spec §3.10 Item 33

typedef enum {
    SlotState_Dispatched = 0,
    SlotState_FenceCompleted = 1,
    SlotState_ReadableAsTail = 2,
    SlotState_Empty = 3  // initial state, before sim_tick assigned
} SlotState;

typedef struct {
    uint64_t sim_tick;
    void* world_snapshot_ptr;       // NativeWorld snapshot pointer
    void* fields_snapshot_ptr;      // FieldStorageSnapshot (К-L7.1 binding)
    void* compute_fence_handle;     // VkFence opaque handle
    SlotState state;
} PipelineSlot;

// Pipeline depth D = 1-3 (configurable per К-L16), default 2
int32_t df_pipeline_init(int depth /* 1-3 */);
int32_t df_pipeline_get_depth(int* out_depth);

// Slot management
int32_t df_pipeline_allocate_slot(uint64_t sim_tick, /* out */ PipelineSlot** out_slot);
int32_t df_pipeline_get_slot(int slot_index /* 0=current, -1=tail */, /* out */ PipelineSlot** out_slot);

// Fence orchestration
int32_t df_pipeline_set_fence(PipelineSlot* slot, void* vk_fence_handle);
int32_t df_pipeline_check_fences(/* updates Dispatched → FenceCompleted via vkGetFenceStatus */);
int32_t df_pipeline_transition_to_tail(PipelineSlot* slot /* FenceCompleted → ReadableAsTail */);

// Quiescent state check (К-L18 prerequisite)
int32_t df_pipeline_is_quiescent(int* out_is_quiescent);
```

**Per Lesson #7 + S8-Q2**: PipelineSlot struct shape preserved literally from spec. FieldStorageSnapshot (К-L7.1) bound к slot — К-L7 atomic-from-observer invariant preserved within slot boundary.

**Per S8-Q1 + К-L16**: Default D=2 configurable 1-3. Settings exposed через configuration (settings file? command-line flag? compile-time #define?). К10.3 default: compile-time #define `DF_PIPELINE_DEPTH=2` с runtime override via configuration — final policy deferred к Phase 0 review of existing configuration patterns.

**Slot state machine** (per spec §3.10 Item 33):

```
SlotState_Empty
    ↓ allocate_slot (sim_tick assigned)
SlotState_Dispatched (sim thread dispatched compute work to GPU)
    ↓ check_fences detects fence signaled
SlotState_FenceCompleted (GPU finished, results available)
    ↓ transition_to_tail (advance pipeline)
SlotState_ReadableAsTail (display thread reads from here)
    ↓ recycle (sim thread reallocates with new sim_tick)
SlotState_Empty (cycle completes)
```

**Validation**:
- `cmake --build` clean
- `pipeline_slot_test` selftest section:
  - Allocate slot, verify state machine transitions
  - Fence orchestration: set fence, check fences updates state
  - Quiescent state detection: empty pipeline reports quiescent; in-flight reports not quiescent
  - D=2 default operational
- `dotnet build` clean
- `dotnet test` 665+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 Item 33 — tick pipeline depth mechanism (К-L16 + К-L7.1 foundation; D=2 default)`

### Commit 7 — Item 35: Phase.Compute scheduler integration

**Files**:
- `native/DualFrontier.Core.Native/include/scheduler.h` (modified — Phase.Compute added к phase enumeration)
- `native/DualFrontier.Core.Native/src/scheduler.cpp` (modified — Phase.Compute dispatch path + VkQueueSubmit batching)
- `native/DualFrontier.Core.Native/test/phase_compute_test.cpp` (new selftest section)

**Drift surface**: К10 scheduler gains Phase.Compute phase between Phase.Update (sim writes) и Phase.Display (display reads slot tail). VkQueueSubmit batching: all compute dispatches from active dispatch systems coalesced into single submit per tick (per Prediction 12 ~50-100μs savings).

**Implementation surface (per spec §3.10 Item 35)**:

```cpp
// scheduler.h amendment
typedef enum {
    Phase_Update = 0,      // sim writes (existing)
    Phase_Compute = 1,     // К10.3 NEW: GPU compute dispatches per pipeline slot
    Phase_Display = 2,     // display reads slot tail (existing, renamed if needed)
    // ... other existing phases
} Phase;

// Phase.Compute dispatch
int32_t df_scheduler_dispatch_phase_compute(PipelineSlot* current_slot);

// Compute dispatch registration (called by V substrate consumers during system update)
int32_t df_scheduler_register_compute_dispatch(
    PipelineSlot* slot,
    void* pipeline_handle,      // VkPipeline opaque
    void* descriptor_set,        // VkDescriptorSet opaque
    int dispatch_x, int dispatch_y, int dispatch_z);

// VkQueueSubmit batching
int32_t df_scheduler_submit_compute_batch(PipelineSlot* slot, void* async_compute_queue);
```

```cpp
// scheduler.cpp implementation
int32_t df_scheduler_dispatch_phase_compute(PipelineSlot* current_slot)
{
    // Collect all compute dispatches registered for this slot
    // Single VkQueueSubmit с all dispatches coalesced
    // Submit к async compute queue (К-L19)
    // Set slot fence (via df_pipeline_set_fence per Item 33)
    
    df_scheduler_submit_compute_batch(current_slot, g_async_compute_queue);
    df_pipeline_set_fence(current_slot, vk_fence_handle);
    
    // Slot state: Dispatched (per Item 33 state machine)
    return DF_SUCCESS;
}
```

**Async compute queue consumed (К-L19 + Item 43)**: Phase.Compute submits к dedicated async compute queue obtained at Item 43; graphics queue не used для compute.

**К10.3 boundary**: К10.3 establishes Phase.Compute infrastructure + VkQueueSubmit batching mechanism; actual compute pipelines registered by V substrate consumers (mods, vanilla systems using IModApi.ComputePipelines) outside К10.3 scope.

**Validation**:
- `cmake --build` clean
- `phase_compute_test`: Phase.Compute phase enumerated; dispatch registration accumulates; submit batching coalesces multiple dispatches into single VkQueueSubmit (verified via Vulkan validation layer logs in DEBUG)
- `dotnet test` 665+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 Item 35 — Phase.Compute scheduler integration (VkQueueSubmit batching к async compute queue)`

### Commit 8 — Item 36: Pipeline slot read API (S8-Q2 Pattern C)

**Files**:
- `native/DualFrontier.Core.Native/include/pipeline_slot.h` (extended — slot tail read API)
- `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (extended)
- `native/DualFrontier.Core.Native/test/pipeline_slot_test.cpp` (extended)
- `src/DualFrontier.Core/Fields/RawTileField.cs` (modified — К-L7.1 read pattern; sim-thread reads see slot tail state)

**Drift surface**: Sim-thread reads from slot tail (`SimTick - 1` reads dispatched-at-`(SimTick - 1) - 1` results); display thread reads from slot tail (`SimTick - D`). К-L7 atomic-from-observer preserved within pipeline slot boundary.

**Implementation surface (per spec §3.10 Item 36 + S8-Q2 Pattern C)**:

```c
// pipeline_slot.h extension
// slot_offset: 0 = current, -1 = previous (tail для sim thread reads), -D = display tail
int32_t df_pipeline_read_slot_tail(
    int slot_offset,
    /* out */ void** out_field_snapshot,
    /* out */ uint64_t* out_sim_tick);

// Validation: slot must be in ReadableAsTail or FenceCompleted state
// Returns error если slot still Dispatched (fence не signaled)
```

```csharp
// RawTileField.cs К-L7.1 read pattern
public ReadOnlySpan<T> AcquireSpan()
{
    // К-L7.1 sub-invariant: sim-thread reads see slot tail state
    // (one-tick lag bounded и deterministic per К-L16 D=2 → reads from sim_tick - 1)
    
    if (_isComputeManaged)
    {
        // GPU compute-managed field: read from pipeline slot tail
        var slotTail = PipelineSlotInterop.GetSlotTail(-1);  // sim_tick - 1
        return new ReadOnlySpan<T>(slotTail.FieldsSnapshotPtr, _width * _height);
    }
    else
    {
        // CPU-managed field: existing К-L7 semantics preserved
        return _backingStore.AsSpan();
    }
}
```

**Per К-L7.1 verbatim (spec Part 2.4)**: «sim tick T+D reads dispatched-at-(T+D-1) state. One-tick lag from sim-perspective bounded и deterministic.»

Cross-slot reads see different snapshots — К-L7.1 establishes this explicitly. Within slot, К-L7 atomic-from-observer preserved (slot snapshot atomically captured, не torn).

**К10 forward integration (Item 37 next)**: Slot transition Dispatched → ReadableAsTail fires StateChangeWake via filter primitive integration; Item 37 wires this.

**Validation**:
- `cmake --build` clean
- `pipeline_slot_test` extended: read API correct slot_offset interpretation; error handling для Dispatched-state slots
- `dotnet build` clean
- `dotnet test` 665+ green (К-L7.1 semantics validated)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 Item 36 — pipeline slot read API (К-L7.1 sim-thread slot tail read pattern)`

### Commit 9 — Item 34: Pipeline drain/refill protocols

**Files**:
- `native/DualFrontier.Core.Native/include/pipeline_slot.h` (extended — drain/refill C ABI)
- `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (extended)
- `src/DualFrontier.Persistence/Pipeline/PipelineSlotSerializer.cs` (new — save/load integration per S8-Q1.5)
- `src/DualFrontier.Persistence/SaveFileFormat.cs` (extended — `pipeline_slot_snapshot` section added)
- `tests/DualFrontier.Persistence.Tests/PipelineSlotSerializerTests.cs` (new)

**Drift surface**: Orderly pipeline drain at save/pause; orderly pipeline refill at load/resume. К10.2 precedent (background queue save-integrated storage Item 31) extended pattern к pipeline slot snapshots.

**Implementation surface (per spec §3.10 Item 34)**:

```c
// pipeline_slot.h extension
// Save protocol per S8-Q1.5: snapshot display tick state (CurrentSimTick - D)
int32_t df_pipeline_serialize_display_state(
    /* out */ void* buffer,
    uint32_t buffer_size,
    uint32_t* out_bytes_written);

// Load protocol: orderly refill from saved state
int32_t df_pipeline_deserialize_display_state(
    const void* buffer,
    uint32_t buffer_size);

// Pause protocol: natural convergence — sim thread completes current tick, no new dispatch
int32_t df_pipeline_pause(/* sim thread eventually quiesces */);

// Resume protocol: refill from pause point
int32_t df_pipeline_resume(/* sim thread resumes от last saved sim_tick */);
```

**Save protocol semantics** (per S8-Q1.5):
- Snapshot display tick state (CurrentSimTick - D) — display already sees coherent world
- Pipeline drain NOT required at save time — display tick state captures coherent world
- Faster save (no waiting для in-flight compute completion)

**Pause protocol semantics**:
- Natural convergence: sim thread completes current tick, no new dispatch
- Pipeline depth naturally absorbs already-dispatched work
- К-L18 quiescent state precondition (Item 41) verifies pipeline quiesced before mod operations

**Load protocol semantics**:
- Sim thread starts at saved sim_tick; refills pipeline incrementally
- Display unblocks once D slots populated
- К-L16 invariant established post-refill

**Persistence integration**: DualFrontier.Persistence extended per К10.2 Item 31 pattern (background queue save-integrated precedent).

**Validation**:
- `cmake --build` clean
- `dotnet build` clean
- `PipelineSlotSerializerTests`: round-trip serialization; save protocol snapshots display tick state; load protocol refills correctly
- Pipeline drain/refill scenarios tested via pipeline_slot_test selftest extension
- `dotnet test` 665+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(persistence): K10.3 Item 34 — pipeline drain/refill protocols + save-integrated pipeline slot storage`

### Commit 10 — Item 37: Filter primitive integration с pipeline slot transitions

**Files**:
- `native/DualFrontier.Core.Native/src/state_change_filter.cpp` (modified — pipeline slot transition triggers filter check)
- `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (modified — `transition_to_tail` fires StateChangeWake)
- `native/DualFrontier.Core.Native/test/pipeline_slot_wake_test.cpp` (new selftest section)
- `src/DualFrontier.Contracts/Scheduling/WakeOnSlotTransitionAttribute.cs` (new — `[WakeOnSlotTransition]` для systems wanting wake-on-fence-completion)

**Drift surface**: К-L13 on-demand activation (К10.1 Item 3 + Item 17 filter primitive) extended с pipeline slot transitions. Downstream read systems can wake when fence completes на slot tail (drives sim-side reactivity post-GPU dispatch).

**Implementation surface (per spec §3.10 Item 37)**:

```cpp
// pipeline_slot.cpp — transition_to_tail extension
int32_t df_pipeline_transition_to_tail(PipelineSlot* slot)
{
    // Atomic state transition: FenceCompleted → ReadableAsTail
    SlotState expected = SlotState_FenceCompleted;
    if (!atomic_compare_exchange(&slot->state, expected, SlotState_ReadableAsTail))
    {
        return DF_ERR_INVALID_SLOT_STATE;
    }
    
    // К10.3 Item 37 NEW: trigger StateChangeWake для slot-subscribed systems
    // Reuses К10.1 Item 17 filter primitive (Level 1 + Level 2 hybrid)
    df_filter_check_pipeline_slot_subscribers(slot->sim_tick);
    
    return DF_SUCCESS;
}
```

```csharp
// WakeOnSlotTransitionAttribute.cs
[AttributeUsage(AttributeTargets.Class)]
public sealed class WakeOnSlotTransitionAttribute : Attribute
{
    public uint SlotOffset { get; }  // 0 = current (just transitioned), -1 = previous, etc.
    public WakeOnSlotTransitionAttribute(uint slotOffset = 0) => SlotOffset = slotOffset;
}

// System usage example:
[WakeOnSlotTransition]  // wakes when slot transitions to ReadableAsTail
public class MovementReactSystem : SystemBase
{
    public override void Update(float delta)
    {
        // Read RawTileField data from slot tail (К-L7.1)
        // Apply movement decisions based on freshly-computed fields
    }
}
```

**Atomic slot transition + filter check (К-L7 batch-commit semantics)**: no observer sees torn slot state; filter check happens after atomic state transition completes.

**Validation**:
- `cmake --build` clean
- `pipeline_slot_wake_test`: 
  - Slot transition fires StateChangeWake к subscribed systems
  - Filter primitive Level 1 cold-path bypass measurable
  - Atomic transition correctness (no torn state observed)
- `dotnet test` 665+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 Item 37 — filter primitive + pipeline slot transition wake integration`

### Commit 11 — К-L7.1 + К-L16 invariants landing + VULKAN_SUBSTRATE.md amendments (load-bearing 2 of 4)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.2 partial → v2.2 (К-L7.1 sub-invariant row + К-L16 row added)
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 partial → v1.1 partial (§2 architecture extended с pipeline depth; §2.3 threading model substantial amendment; §7.2 determinism extended с pipeline drain; §7.3 async sync hazards reworded для pipeline slot tail read pattern)
- `docs/governance/REGISTER.yaml` (DOC-A-KERNEL version partial bump completed; DOC-A-VULKAN_SUBSTRATE partial bump continued)

**This is the second load-bearing commit of К10.3** (К-L7.1 + К-L16 amendment landing per Approach C grouped). К-L7.1 sub-invariant and К-L16 share physical reality «GPU pipeline slots» — landing them вместе preserves К-L7 invariant integrity per Lesson #11 (architectural reduction methodology).

**К-L invariant table amendments**:

К-L7 row amended (extending с К-L7.1 sub-invariant per К-L3.1 precedent):
```
| К-L7 (+К-L7.1) | LOCKED (К-L7.1 added К10.3) | Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1) | К-L7 atomic-from-observer preserved within pipeline slot boundary |
```

К-L16 row inserted:
```
| К-L16 | LOCKED (К10.3) | Simulation tick pipeline depth | D ≥ 1 (configurable 1-3, default 2); К-L16 establishes display latency invariant (D × tick_period); pipeline drain orderly at save/pause |
```

**К-L7.1 invariant text (verbatim per §1.3)**.

**К-L16 invariant text (verbatim per §1.4)**.

**VULKAN_SUBSTRATE.md substantial amendments**:

**§2 architecture amendment** (add pipeline depth section после existing display architecture):

```markdown
### Pipeline depth architecture (К-L16, К10.3 amendment)

The V substrate supports simulation tick pipeline depth D=2 (default, configurable 1-3) per К-L16. Simulation thread runs D ticks ahead of display thread; cross-layer async operations (GPU compute, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread.

Pipeline slot data model (verbatim from KERNEL_FULL_NATIVE_SCHEDULER.md §3.10 Item 33):
[PipelineSlot struct]

Sim-thread reads see slot tail state (К-L7.1 sub-invariant): sim tick T reads dispatched-at-(T-1) state. One-tick lag bounded и deterministic.

Display thread reads from CurrentSimTick - D — display latency invariant established по К-L16.
```

**§2.3 threading model amendment** (substantial — extends с pipeline depth + async compute queue + sim-thread read pattern):

Existing threading model preserved verbatim; К10.3 amendment adds:

```markdown
### Pipeline depth and queue family roles (К-L16/L19, К10.3 amendment)

Sim thread coordinates с three Vulkan queues:
- **Graphics queue** — display rendering (existing — preserved verbatim)
- **Async compute queue** (К-L19 К10.3 amendment) — К-L16 pipeline depth dispatches per Phase.Compute (К10.3 Item 35)
- **Copy/transfer queue** (К-L19 К10.3 amendment) — asset transfers (existing semantics)

Pipeline depth D=2 default (К-L16): sim thread allocates new slot at start of tick; Phase.Compute dispatches к async compute queue; fence orchestration tracks slot transitions Dispatched → FenceCompleted → ReadableAsTail.

Sim-thread reads from slot tail (К-L7.1) — `df_pipeline_read_slot_tail(slot_offset = -1)` returns sim_tick - 1 results. К-L7 atomic-from-observer preserved within slot boundary; cross-slot reads see different snapshots.
```

**§7.2 determinism amendment** (extends с pipeline drain semantics):

```markdown
### Pipeline drain semantics (К-L16, К10.3 amendment)

Save protocol per S8-Q1.5: snapshot display tick state (CurrentSimTick - D). Display already sees coherent world; pipeline drain не required at save time. Faster save (no waiting для in-flight compute completion).

Pause protocol: natural convergence — sim thread completes current tick, no new dispatch. Pipeline depth naturally absorbs already-dispatched work. К-L18 quiescent state precondition (Item 41) verifies pipeline quiesced before mod operations.
```

**§7.3 async sync hazards amendment** (reworded для pipeline slot tail read):

Previous hazard description (per existing v1.0 text) reworded:
```markdown
### Pipeline slot tail read pattern (К-L7.1, К10.3 amendment)

Previously async sync hazards section described per-read fence polling (fence-async query на each FieldHandle.ReadCell). К10.3 К-L7.1 replaces this с slot tail read pattern: sim-thread reads see slot tail state (sim_tick - 1) without per-read fence query. Predicted savings ~30-50% reduction в FieldHandle.ReadCell latency (Prediction 13).

К-L7 atomic-from-observer invariant preserved within pipeline slot boundary; cross-slot reads see different snapshots (К-L7.1).
```

**Per Lesson #8 (atomic compilable unit)**: К-L7.1 + К-L16 invariant text + KERNEL_ARCHITECTURE amendment + VULKAN_SUBSTRATE.md substantial amendments + REGISTER updates land в **same commit**. Per Lesson #11 (architectural reduction): К-L7.1 sub-invariant к К-L7 preserves К-L7 integrity, mirrors К-L3.1 pattern.

**Validation**:
- `dotnet build` clean
- `cmake --build` clean
- `dotnet test` 665+ green
- К-L cross-reference integrity verified
- VULKAN_SUBSTRATE.md amendments preserve existing wording style
- `sync_register.ps1 --validate` exit 0

**Commit message**:
```
feat(architecture): K10.3 — К-L7.1 + К-L16 amendments + VULKAN_SUBSTRATE.md substantial amendments

The second load-bearing commit of К10.3 (К-L7.1 + К-L16 grouped landing per
Approach C). К-L7.1 sub-invariant к К-L7 (mirrors К-L3.1 precedent); К-L16
simulation pipeline depth.

KERNEL_ARCHITECTURE.md v2.2 (К-L invariant table extended с К-L7.1 sub +
К-L16 rows).

VULKAN_SUBSTRATE.md v1.0 → v1.1 substantial amendments:
- §2 architecture: pipeline depth section added (К-L16)
- §2.3 threading: pipeline depth + async compute queue + sim-thread read pattern (К-L16/L19/L7.1)
- §7.2 determinism: pipeline drain semantics (К-L16)
- §7.3 async sync hazards: pipeline slot tail read pattern (К-L7.1)

К-L7.1 text: «GPU compute writes к RawTileField storage are bound к pipeline
slot (К-L16). Sim-thread reads of compute-managed fields see slot-tail state...»

К-L16 text: «Simulation tick pipeline depth D ≥ 1 (configurable 1-3, default
2). Simulation thread runs D ticks ahead of display thread...»

Build clean; 665 tests green.

Phase 11 of K10.3 cascade. Commit 11 of <total>.
```

### Commit 12 — Item 38: Display composition framework

**Files**:
- `src/DualFrontier.Application/Display/CompositionFramework.cs` (new — layer registry + latency contracts + composition order)
- `src/DualFrontier.Application/Display/Layer.cs` (new — layer abstraction)
- `src/DualFrontier.Application/Display/LayerType.cs` (new — enum SimState / Intent / CombatFeedback / Static)
- `tests/DualFrontier.Application.Tests/Display/CompositionFrameworkTests.cs` (new)

**Drift surface**: Display composition framework operational. Existing rendering path continues к work as «Sim state» layer; intent/combat feedback layers opt-in для new use cases. Layer registration + composition order infrastructure landed.

**Implementation surface (per spec §3.11 Item 38 + К-L17)**:

```csharp
// LayerType.cs
public enum LayerType
{
    SimState = 0,       // Pipeline slot tail (К-L16 governed)
    Intent = 1,         // Current input state (≤16ms latency)
    CombatFeedback = 2, // Fast tier consumers (К-L15 latency)
    Static = 3          // Loaded assets
}

// Layer.cs
public abstract class Layer
{
    public LayerType Type { get; }
    public string Fqn { get; }  // FQN для capability tokens
    public int CompositionOrder { get; }  // 0 = first, lower number = bottom layer
    
    public abstract void Render(/* render context */);
}

// CompositionFramework.cs
public sealed class CompositionFramework
{
    private readonly Dictionary<string, Layer> _layers = new();
    
    public void RegisterLayer(Layer layer)
    {
        // К-L17 composition order: sim state layers rendered first, intent/combat overlays composited on top
        _layers[layer.Fqn] = layer;
    }
    
    public void RenderFrame(/* render context */)
    {
        // Composition order per К-L17:
        // 1. Sim state layers (lowest CompositionOrder)
        // 2. Intent overlays
        // 3. Combat feedback overlays
        // 4. Static layers
        
        var orderedLayers = _layers.Values
            .OrderBy(l => (int)l.Type)
            .ThenBy(l => l.CompositionOrder);
        
        foreach (var layer in orderedLayers)
        {
            layer.Render(/* render context */);
        }
    }
}
```

**Layer taxonomy (per К-L17 + §1.5 table)**:
- SimState — reads from pipeline slot tail (К-L16)
- Intent — reads from current input state (sub-tick latency)
- CombatFeedback — reads from Fast tier event consumers (К-L15)
- Static — loaded assets

**К-L17 latency invariants preserved**:
- SimState layer latency = D × tick_period (К-L16 governed)
- Intent layer latency ≤ 16ms (К-L17 mandate)
- CombatFeedback latency ≤ 1ms К-L15 + ≤ 16ms display ≈ ≤ 17ms event-to-visible (Prediction 15)

**Existing rendering path preserved**: Current Vulkan rendering path registered as SimState layer. К10.3 doesn't replace existing rendering; adds layer registry on top.

**Validation**:
- `dotnet build` clean
- `dotnet test` 665+ green
- `CompositionFrameworkTests`: layer registration; composition order correctness; rendering invokes layers в correct order
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(application): K10.3 Item 38 — display composition framework (К-L17 layer registry + composition order)`

### Commit 13 — Items 39+40: Intent overlay + combat feedback layer infrastructure

**Files**:
- `src/DualFrontier.Application/Display/IntentOverlayLayer.cs` (new — current input state surface + sub-pipeline-latency rendering)
- `src/DualFrontier.Application/Display/CombatFeedbackLayer.cs` (new — Fast tier event consumers rendering)
- `src/DualFrontier.Contracts/Display/LayerAttribute.cs` (new — `[Layer(LayerType.Intent)]` или `[Layer(LayerType.CombatFeedback)]` для mod-registered layer providers)
- `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs` (modified — К-L17 layer capabilities added per S8-Q3 + S3-Q5 pattern: `kernel.layer.intent:{FQN}` + `kernel.layer.combat_feedback:{FQN}`)
- `tests/DualFrontier.Application.Tests/Display/IntentOverlayTests.cs` (new)
- `tests/DualFrontier.Application.Tests/Display/CombatFeedbackTests.cs` (new)

**Drift surface**: Intent overlay + combat feedback layer infrastructure landed. Mods can register layers via capability declaration + `[Layer]` attribute pattern. К-L17 composition order enforced via CompositionFramework (Commit 12).

**Implementation surface (per spec §3.11 Items 39+40)**:

```csharp
// IntentOverlayLayer.cs
public class IntentOverlayLayer : Layer
{
    public override LayerType Type => LayerType.Intent;
    
    public override void Render(/* render context */)
    {
        // К-L17 contract: ≤16ms latency (1 display frame at 60 FPS)
        // Read directly from current input state, не from pipeline slot
        var inputState = InputState.Current;
        
        // Render cursor / hover / drag-and-drop / construction-placement preview
        // Sub-pipeline-latency: input state read at display tick time, не sim tick time
    }
}

// CombatFeedbackLayer.cs
public class CombatFeedbackLayer : Layer
{
    public override LayerType Type => LayerType.CombatFeedback;
    
    private readonly Queue<CombatFeedbackEvent> _pendingFeedback = new();
    
    public CombatFeedbackLayer(IBusFacade bus)
    {
        // К-L15 fast tier subscription: combat hit events
        bus.Subscribe<CombatHitEvent>(OnCombatHit);
        bus.Subscribe<DamageNumberEvent>(OnDamageNumber);
    }
    
    private void OnCombatHit(CombatHitEvent evt)
    {
        // Fast tier subscriber: bounded exec, no blocking, no GC alloc
        // Queue для next display frame (≤16ms К-L17 latency)
        _pendingFeedback.Enqueue(new CombatFeedbackEvent(evt));
    }
    
    public override void Render(/* render context */)
    {
        while (_pendingFeedback.TryDequeue(out var feedback))
        {
            // Render damage numbers, hit sparks, weapon glints
            // К-L15 + К-L17 latency: ≤1ms К-L15 + ≤16ms display ≈ ≤17ms event-to-visible
        }
    }
}
```

**Capability tokens per S8-Q3 + S3-Q5 pattern**:
- `kernel.layer.intent:{FQN}` — mod registers intent overlay layer
- `kernel.layer.combat_feedback:{FQN}` — mod registers combat feedback layer
- Granular per FQN per layer type per action (consistent с К10.2 capability tier pattern)

**KernelCapabilityRegistry extension**:

```csharp
public sealed class KernelCapabilityRegistry
{
    public IReadOnlySet<string> GetKernelCapabilities()
    {
        // ... existing tier-prefixed event tokens (К10.2)
        
        // К10.3 layer capabilities (NEW)
        var layerTypes = ScanForTypes<Layer>();
        foreach (var layerType in layerTypes)
        {
            var fqn = layerType.FullName!;
            var typeAttr = layerType.GetCustomAttribute<LayerAttribute>();
            
            switch (typeAttr.Type)
            {
                case LayerType.Intent:
                    capabilities.Add($"kernel.layer.intent:{fqn}");
                    break;
                case LayerType.CombatFeedback:
                    capabilities.Add($"kernel.layer.combat_feedback:{fqn}");
                    break;
            }
        }
        return capabilities;
    }
}
```

**Per К-L9 «Vanilla = mods»**: vanilla layers (built-in intent overlay для core gameplay, built-in combat feedback) register through same `[Layer]` attribute + capability declaration pattern as third-party mods.

**Validation**:
- `dotnet build` clean
- `dotnet test` 665+ green
- `IntentOverlayTests`: input state read at display tick; sub-pipeline-latency rendering measurable
- `CombatFeedbackTests`: Fast tier subscriber pattern; event-to-visible latency tracking
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(application): K10.3 Items 39+40 — intent overlay + combat feedback layer infrastructure (К-L17)`

### Commit 14 — К-L17 invariant landing + VULKAN_SUBSTRATE.md amendments (load-bearing 3 of 4)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.2 partial → v2.2 (К-L17 row added)
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 partial → v1.1 partial (§4 rendering use case extended с display composition section; §5.5 Mode C navigation extended с visibility latency through К-L17 composition)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.9 partial → v1.10 partial (§3.2 capability syntax extended с layer tokens per S3-Q5 + S8-Q3 pattern)
- `docs/governance/REGISTER.yaml` (version bumps continued)

**This is the third load-bearing commit of К10.3** (К-L17 amendment landing).

**К-L17 invariant text (verbatim per §1.5).**

К-L17 row inserted:
```
| К-L17 | LOCKED (К10.3) | Display composition multi-layer | Sim state + intent overlay + combat feedback layers с independent latency contracts; composition order: sim state first, overlays composited on top |
```

**VULKAN_SUBSTRATE.md §4 rendering amendment** (add display composition section после existing rendering pipeline):

```markdown
### Display composition (К-L17, К10.3 amendment)

V substrate rendering use case extends с display composition framework per К-L17. Three-layer composition:

1. **Sim state layer** — V substrate render path (existing — preserved verbatim). Reads from pipeline slot tail (К-L16). Latency D × tick_period.
2. **Intent overlay layer** (К10.3 amendment) — current input state surface. Reads from `InputState.Current` at display tick time. Latency ≤16ms (60 FPS).
3. **Combat feedback layer** (К10.3 amendment) — Fast tier event consumers. Subscribes к К-L15 fast tier events; renders damage numbers, hit sparks, weapon glints. Latency ≤1ms К-L15 + ≤16ms display ≈ ≤17ms event-to-visible.

Composition order (К-L17 mandate): sim state layers rendered first, intent and combat overlays composited on top.

Mod-registered layers use `[Layer(LayerType.Intent | CombatFeedback)]` attribute + capability declaration (`kernel.layer.intent:{FQN}` или `kernel.layer.combat_feedback:{FQN}` per S3-Q5 + S8-Q3 pattern).
```

**VULKAN_SUBSTRATE.md §5.5 Mode C navigation amendment**:

```markdown
### Mode C visibility latency (К-L17, К10.3 amendment)

Mode C navigation (existing — preserved verbatim) visibility latency now governed по К-L17 composition framework. Player commands ⟶ Intent overlay layer (≤16ms render); pawn responses ⟶ Sim state layer (К-L16 D=2 lag); combat feedback ⟶ CombatFeedback layer (К-L15 + display ≈ ≤17ms event-to-visible).

No special-case visibility mechanism — К-L17 composition framework handles latency separation uniformly.
```

**MOD_OS_ARCHITECTURE.md §3.2 capability syntax extension** (К-L17 layer tokens added к existing tier-prefixed token format documentation):

```markdown
### К-L17 layer capabilities (К10.3 amendment)

In addition к bus tier-prefixed tokens (К-L15 К10.2 amendment), К10.3 extends capability syntax с layer registration tokens:

- `kernel.layer.intent:<FQN>` — mod registers intent overlay layer (sub-pipeline-latency)
- `kernel.layer.combat_feedback:<FQN>` — mod registers combat feedback layer (Fast tier consumer)

Granular per FQN per layer type — consistent с К10.2 tier-prefixed token pattern.
```

**Per Lesson #8**: К-L17 invariant text + KERNEL_ARCHITECTURE amendment + VULKAN_SUBSTRATE.md §4/§5.5 amendments + MOD_OS §3.2 amendment + REGISTER updates land в **same commit**.

**Validation**:
- `dotnet build` clean
- `dotnet test` 665+ green
- К-L cross-reference integrity verified
- VULKAN_SUBSTRATE.md + MOD_OS_ARCHITECTURE.md amendments preserve existing wording style
- `sync_register.ps1 --validate` exit 0

**Commit message**:
```
feat(architecture): K10.3 — К-L17 display composition amendment + cross-document propagation

The third load-bearing commit of К10.3 (К-L17 amendment landing).
К-L17 «Display composition multi-layer» added к KERNEL_ARCHITECTURE.md.

VULKAN_SUBSTRATE.md v1.1 partial amendments:
- §4 rendering: display composition section (К-L17 three-layer model)
- §5.5 Mode C: visibility latency through К-L17 composition (no special-case)

MOD_OS_ARCHITECTURE.md v1.9 partial amendments:
- §3.2 capability syntax: layer tokens (kernel.layer.intent + kernel.layer.combat_feedback)

К-L17 text: «Display tick T composites multi-layer state where layers carry
independent latency contracts...»

Build clean; 665 tests green.

Phase 14 of K10.3 cascade. Commit 14 of <total>.
```

### Commit 15 — Item 41: К-L18 quiescent state enforcement

**Files**:
- `native/DualFrontier.Core.Native/src/mod_unload.cpp` (modified — К-L18 quiescent state precondition extended с pipeline slot verification)
- `native/DualFrontier.Core.Native/include/mod_unload.h` (extended)
- `native/DualFrontier.Core.Native/test/quiescent_state_test.cpp` (new selftest section)
- `src/DualFrontier.Core.Interop/ModUnloadInterop.cs` (modified — quiescent state error code handling extended)

**Drift surface**: К10.2 Item 32 `df_scheduler_unload_mod_native_state` already includes quiescent state precondition check (returns error if `sim_state != Paused`). К10.3 Item 41 extends this к include pipeline slot quiescence verification (all D slots в `ReadableAsTail` state or empty per Item 33 state machine).

**Implementation surface (per spec §3.12 Item 41)**:

```cpp
// mod_unload.cpp К-L18 quiescent state precondition extension
ModUnloadResult df_scheduler_unload_mod_native_state(const char* mod_id)
{
    ModUnloadResult result = { 0 };
    
    // К-L18 precondition: sim must be paused (К10.2 К-L18 check, preserved)
    if (g_sim_state != SimState_Paused)
    {
        result.success = 0;
        result.error_count = 1;
        strncpy(result.error_messages[0], "К-L18 violation: simulation must be paused", 255);
        return result;
    }
    
    // К-L18 precondition extended (К10.3 Item 41): pipeline slots quiescent
    int is_quiescent = 0;
    df_pipeline_is_quiescent(&is_quiescent);  // Item 33 API
    if (!is_quiescent)
    {
        result.success = 0;
        result.error_count = 1;
        strncpy(result.error_messages[0], 
                "К-L18 violation: pipeline slots not quiescent (in-flight compute dispatches)",
                255);
        return result;
    }
    
    // Existing К10.2 T0-T7 sequence proceeds...
    return result;
}
```

**Per К-L18 verbatim (spec Part 2.4)**: «pipeline slots quiescent (all fences completed); no concurrent compute dispatches in-flight».

**Mod load primitive mirror**: К-L18 quiescent state precondition symmetric для mod load operations. Item 42 (next commit) lands UI integration enforcing precondition.

**Validation**:
- `cmake --build` clean
- `quiescent_state_test`: 
  - Mod unload succeeds when sim paused + pipeline empty
  - Mod unload fails с clear error when sim не paused
  - Mod unload fails с clear error when pipeline has in-flight slots
- `dotnet test` 665+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 Item 41 — К-L18 quiescent state enforcement extension (pipeline slot quiescence)`

### Commit 16 — Item 42: Settings menu / mod management UI integration

**Files**:
- `src/DualFrontier.Application/UI/ModManagementUI.cs` (new или extended если existing) — К-L18 quiescent state UI workflow
- `src/DualFrontier.Application/Loop/SimulationStateController.cs` (new — pause/resume operations wired к К-L18 precondition)
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` (modified — Step 3.6 V resource cleanup added к unload chain per S8-Q4 V substrate scope)
- `tests/DualFrontier.Application.Tests/UI/ModManagementUITests.cs` (new)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.10 partial (§9.5 amended с Step 3.6 description; §11 hot reload section noted с К-L18 compliance)

**Drift surface**: UI workflow enforces К-L18 pause precondition. User triggers «Disable Mod» → UI pauses sim → wait for pipeline quiescence → invoke unload primitive → on success resume. Hot reload tooling similarly pauses sim. V resource cleanup primitive (Step 3.6) added к MOD_OS §9.5 chain.

**Implementation surface (per spec §3.12 Item 42)**:

```csharp
// ModManagementUI.cs
public sealed class ModManagementUI
{
    private readonly SimulationStateController _simController;
    private readonly ModIntegrationPipeline _modPipeline;
    
    public async Task<ModOperationResult> DisableModAsync(string modId)
    {
        // К-L18 precondition: pause simulation before mod operation
        await _simController.PauseAsync();
        
        // Wait for pipeline quiescence (К-L16 natural drain under pause)
        await _simController.WaitForQuiescenceAsync(TimeSpan.FromSeconds(5));
        
        try
        {
            // Existing 8-step unload chain (К10.2 amended)
            var result = await _modPipeline.UnloadModAsync(modId);
            return result;
        }
        finally
        {
            // Resume simulation after mod operation
            await _simController.ResumeAsync();
        }
    }
}
```

**MOD_OS §9.5 amendment** (Step 3.6 V resource cleanup added к existing 8-step chain — becomes 9-step):

```markdown
### 9.5 ALC unload protocol (К10.2 + К10.3 amended)

The unload chain (9 steps total post-К10.3):

1. RestrictedModApi.UnsubscribeAll()
2. IModContractStore.RevokeAll(modId)
3. ModRegistry.RemoveSystems(modId)
3.5. **(К10.2)** df_scheduler_unload_mod_native_state(modId) — native scheduler primitive
3.6. **(К10.3)** df_vulkan_unload_mod_resources(modId) — V substrate primitive (per S8-Q4). Releases mod-registered Vulkan resources (compute pipelines, descriptor sets, buffers) after К10 scheduler unload completes. Best-effort sequential per §9.5.1.
4. The dependency graph is rebuilt without this mod's systems.
5. The scheduler swaps to new phase list.
6. ALC.Unload() is called.
7. The loader spins on WeakReference.IsAlive (existing protocol).
```

**§11 hot reload section amendment**:

```markdown
### Hot reload К-L18 compliance (К10.3 amendment)

Hot reload preserves game state through managed dependency graph swap mechanism (existing). К-L18 pause precondition required for consistent mod transition. Hot reload tooling pauses simulation, waits для pipeline quiescence, invokes ALC swap, resumes simulation. Mod management UI и hot reload tooling share К-L18 enforcement pattern.
```

**Validation**:
- `dotnet build` clean
- `dotnet test` 665+ green
- `ModManagementUITests`: pause→quiescence→unload→resume orderly flow; failure handling при quiescence timeout
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(application): K10.3 Item 42 — settings menu / mod management UI integration с К-L18 (Step 3.6 V resource cleanup)`

### Commit 17 — К-L18 invariant landing + MOD_OS_ARCHITECTURE.md amendments (load-bearing 4 of 4)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.2 → v2.2 final (К-L18 row added — completes К10.3 К-L additions)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.9 → v1.10 final (Step 3.6 in §9.5 + К-L18 compliance §11 + ValidationErrorKind additions §11.2)
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.0 → v1.1 final (§3.4 native compute dispatch extended с `df_vulkan_unload_mod_resources` C ABI — final VULKAN_SUBSTRATE amendment)
- `docs/governance/REGISTER.yaml` (final version bumps: DOC-A-KERNEL v2.1 → v2.2; DOC-A-VULKAN_SUBSTRATE v1.0 → v1.1; DOC-A-MOD_OS v1.9 → v1.10; governance_events append references к К10.3 closure)

**This is the fourth load-bearing commit of К10.3** (К-L18 amendment landing + final cross-document propagation).

**К-L18 invariant text (verbatim per §1.6)**.

К-L18 row inserted:
```
| К-L18 | LOCKED (К10.3) | Mod lifecycle quiescent state | Sim paused + pipeline slots quiescent precondition для mod load/unload; UI enforces precondition |
```

**MOD_OS §11.2 ValidationErrorKind additions** (К10.3 new entries):
- `QuiescentStatePreconditionViolated` (К10.3) — mod operation attempted while sim active or pipeline in-flight
- `PipelineQuiescenceTimeout` (К10.3) — quiescence wait timeout exceeded
- `LayerCapabilityMismatch` (К10.3) — layer attribute declares one type but capability declares another
- `VulkanModResourceCleanupFailed` (К10.3) — V substrate unload primitive failed

**VULKAN_SUBSTRATE.md §3.4 final amendment**:

```markdown
### `df_vulkan_unload_mod_resources` C ABI primitive (К-L18, К10.3 amendment per S8-Q4)

V substrate native primitive releases mod-registered Vulkan resources after К10 scheduler unload completes (`ModIntegrationPipeline.UnloadMod` Step 3.6).

```c
typedef struct {
    int success;
    int pipelines_destroyed;
    int descriptor_sets_destroyed;
    int buffers_destroyed;
    int images_destroyed;
    char error_messages[8][256];
    int error_count;
} VulkanModUnloadResult;

int32_t df_vulkan_unload_mod_resources(
    const char* mod_id,
    VulkanModUnloadResult* out_result);
```

К-L18 precondition: sim paused + pipeline quiescent (verified before call). VkDestroyPipeline / VkFreeDescriptorSets / vkDestroyBuffer / vkDestroyImage operations for mod-registered resources. Best-effort sequential per MOD_OS §9.5.1.
```

**Per Lesson #8 (atomic compilable unit)**: К-L18 invariant text + KERNEL_ARCHITECTURE amendment + MOD_OS § amendments + VULKAN_SUBSTRATE.md §3.4 final amendment + REGISTER updates land в **same commit**.

**Validation**:
- `dotnet build` clean
- `cmake --build` clean
- `dotnet test` 665+ green
- К-L cross-reference integrity verified (final state — 5 К-L invariants added в К10.3, total К-Lxx series 20)
- VULKAN_SUBSTRATE.md amendments preserve existing wording style
- `sync_register.ps1 --validate` exit 0

**Commit message**:
```
feat(architecture): K10.3 — К-L18 quiescent state amendment + final cross-document propagation

The fourth load-bearing commit of К10.3 (К-L18 amendment landing).
К-L18 «Mod lifecycle quiescent state» added к KERNEL_ARCHITECTURE.md
v2.1 → v2.2 (К10.3 complete К-L additions).

MOD_OS_ARCHITECTURE.md v1.9 → v1.10:
- §9.5: Step 3.6 V resource cleanup added (9-step chain post-К10.3)
- §11 hot reload: К-L18 compliance note
- §11.2: new ValidationErrorKinds (QuiescentStatePreconditionViolated,
  PipelineQuiescenceTimeout, LayerCapabilityMismatch, VulkanModResourceCleanupFailed)

VULKAN_SUBSTRATE.md v1.0 → v1.1:
- §3.4: df_vulkan_unload_mod_resources C ABI primitive (К-L18 К10.3)

К-L18 text: «Mod load/unload operations require simulation paused state...
pipeline slots quiescent (all fences completed)...»

К-Lxx series total: 20 invariants post-К10.3 (К-L1..L11 + К-L6 SUPERSEDED +
К-L12..L19 + К-L3.1/L7.1 subs).

Build clean; 665 tests green.

Phase 17 of K10.3 cascade. Commit 17 of 18 — К10.3 К-L invariant landings complete.
```

### Commit 18 — К10.3 closure: REGISTER amendments + audit_trail EVT + brief lifecycle EXECUTED

**Files**:
- `docs/governance/REGISTER.yaml` (DOC-D-K10_3 lifecycle AUTHORED → EXECUTED; audit_trail EVT entry; CAPA entries если any opened)
- `tools/briefs/K10_3_EXECUTION_BRIEF.md` (frontmatter status AUTHORED → EXECUTED; new §8 closure section added)
- `docs/MIGRATION_PROGRESS.md` (К10.3 closure entry per METHODOLOGY §12.7 step 3)
- `docs/governance/REGISTER_RENDER.md` (regenerated via render_register.ps1)
- `docs/governance/VALIDATION_REPORT.md` (regenerated via sync_register.ps1)

**REGISTER amendments** (per METHODOLOGY §12.7 canonical):

1. **DOC-D-K10_3**: lifecycle AUTHORED → EXECUTED
2. **DOC-A-KERNEL**: version 2.1 → 2.2 (5 К-L invariants added — К-L7.1 sub + К-L16/L17/L18/L19)
3. **DOC-A-VULKAN_SUBSTRATE**: version 1.0 → 1.1 (8 sub-section amendments per spec Part 7.5)
4. **DOC-A-MOD_OS**: version 1.9 → 1.10 (К-L17 layer caps + К-L18 compliance + Step 3.6 + new ValidationErrorKinds)
5. **DOC-A-KERNEL_FULL_NATIVE_SCHEDULER**: governance_events append с EVT-K10_3-CLOSURE
6. **DOC-G-README**: governance_events append (hardware requirements section landed)
7. **audit_trail entry**: `EVT-{date}-K10_3-CLOSURE`
8. **Requirements added**: REQ-K-L7_1, REQ-K-L16, REQ-K-L17, REQ-K-L18, REQ-K-L19 (5 new REQs)
9. **Risks status update**: R-K10-8 (hardware tier exclusion) status ACTIVE → ACCEPTED (К-L19 architectural commitment)

**Validation**:
- `sync_register.ps1 --validate` exit 0 — **mandatory gate** per METHODOLOGY §12.7
- `dotnet build` clean
- `dotnet test` 665+ green (К10.3 final count documented в closure)
- `cmake --build` clean
- Native selftest scenarios: 77 К10.2 baseline + ~25-35 К10.3 new sections = ~102-112 total

**Commit message**:
```
governance: K10.3 closure — REGISTER amendments + 5 REQs + EVT-K10_3-CLOSURE

К10.3 sub-milestone closure per METHODOLOGY §12.7 canonical protocol.

REGISTER updates:
- DOC-D-K10_3 lifecycle AUTHORED → EXECUTED
- DOC-A-KERNEL version 2.1 → 2.2 (5 К-L invariants added)
- DOC-A-VULKAN_SUBSTRATE version 1.0 → 1.1 (8 sub-section amendments)
- DOC-A-MOD_OS version 1.9 → 1.10 (К-L17 + К-L18 + Step 3.6)

Requirements added: REQ-K-L7_1, REQ-K-L16, REQ-K-L17, REQ-K-L18, REQ-K-L19

audit_trail entry: EVT-{date}-K10_3-CLOSURE

К10.3 closure leaves К10.4 (TLA+ formal verification, 3 items: 18, 45, 46)
as final К10 sub-milestone brief per Option III standalone-briefs structure.

К-L7.1 + К-L16/L17/L18/L19 architecturally established (5 invariants).
Actual sovereign authority switch (display composition rendering authority,
quiescent state UI enforcement) preserved managed-facade strategy; full
hardening deferred к К10.4 closure or К-closure report (А'.8).

К-Lxx series total: 20 invariants post-К10.3.

Phase 18 of K10.3 cascade. Commit 18 of 18 — К10.3 closure.
```

---
## §4 — К-L invariant amendments (К10.3 detailed scope)

К10.3 lands **5 К-L invariants** в KERNEL_ARCHITECTURE.md v2.1 → v2.2 — largest К-L surface among К10 sub-milestones (К10.1 landed 3; К10.2 landed 1; К10.3 lands 5). Per S-LOCK-2 Approach C grouped landing: 4 load-bearing commits distribute К-L additions across cascade.

### §4.1 — Verbatim К-L invariant table amendments

`docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L invariants table receives 5 row insertions/extensions:

**К-L7 row extended** (К-L7.1 sub-invariant added):
```
| К-L7 (+К-L7.1) | LOCKED (К-L7.1 added К10.3) | Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1) | К-L7 atomic-from-observer preserved within pipeline slot boundary; К-L3.1 sub-invariant precedent pattern |
```

**К-L16 row inserted** (после К-L15 К10.2):
```
| К-L16 | LOCKED (К10.3) | Simulation tick pipeline depth | D ≥ 1 (configurable 1-3, default 2); display latency invariant (D × tick_period); pipeline drain orderly at save/pause |
```

**К-L17 row inserted**:
```
| К-L17 | LOCKED (К10.3) | Display composition multi-layer | Sim state + intent overlay + combat feedback layers; independent latency contracts; composition order: sim state first, overlays on top |
```

**К-L18 row inserted**:
```
| К-L18 | LOCKED (К10.3) | Mod lifecycle quiescent state | Sim paused + pipeline slots quiescent precondition для mod load/unload; UI enforces precondition |
```

**К-L19 row inserted**:
```
| К-L19 | LOCKED (К10.3) | Hardware tier commitment | Vulkan 1.3 + async compute queue family mandate; NVIDIA Turing+ / AMD RDNA 1+ / Intel Arc Alchemist+ baseline |
```

### §4.2 — KERNEL_ARCHITECTURE.md Part 2 master plan amendment

Part 2 master plan table К10 row updated (existing post-К10.2 row updated с К-L7.1/L16/L17/L18/L19 ratified annotation):

```
| К10 | Native kernel scheduler + bus + pipeline + display + hardware | AUTHORED-IN-PROGRESS (К10.1 ✅ + К10.2 ✅ + К10.3 ✅; К10.4 pending) | К-L6 SUPERSEDED + К-L12/L13/L14 (К10.1) + К-L15 (К10.2) + К-L7.1 + К-L16/L17/L18/L19 (К10.3); TLA+ verification К10.4 pending |
```

К-closure row remains unchanged (А'.8 milestone).

### §4.3 — VULKAN_SUBSTRATE.md substantial amendments (v1.0 → v1.1)

8 sub-section amendments per spec Part 7.5. Distributed across load-bearing commits:

**К-L19 commit (Commit 5)**:
- §0 L1 — Vulkan 1.3 + async compute queue requirement
- §0 L7 — Windows-only platform matches К-L19 hardware tier baseline
- §3.4 — native compute dispatch к dedicated async compute queue mandate

**К-L7.1 + К-L16 commit (Commit 11)**:
- §2 architecture — pipeline depth section added
- §2.3 threading — substantial amendment (pipeline depth + async compute queue + sim-thread read pattern)
- §7.2 determinism — pipeline drain semantics (К-L16)
- §7.3 async sync hazards — pipeline slot tail read pattern (К-L7.1)

**К-L17 commit (Commit 14)**:
- §4 rendering — display composition section
- §5.5 Mode C — visibility latency through К-L17 composition

**К-L18 commit (Commit 17)**:
- §3.4 — `df_vulkan_unload_mod_resources` C ABI primitive (К-L18 К10.3 per S8-Q4)

### §4.4 — MOD_OS_ARCHITECTURE.md amendments (v1.9 → v1.10)

**К-L17 commit (Commit 14)**:
- §3.2 capability syntax — layer tokens (`kernel.layer.intent:{FQN}`, `kernel.layer.combat_feedback:{FQN}`)

**К-L18 commit (Commit 17)**:
- §9.5 — Step 3.6 V resource cleanup (chain extended 8 → 9 steps)
- §11 hot reload — К-L18 compliance note
- §11.2 — new ValidationErrorKind entries (QuiescentStatePreconditionViolated, PipelineQuiescenceTimeout, LayerCapabilityMismatch, VulkanModResourceCleanupFailed)

### §4.5 — README.md amendment (К-L19 commit)

NEW «Hardware Requirements» section added к README.md (Commit 5). Publishes К-L19 hardware tier к user-facing documentation:

```markdown
## Hardware Requirements

Dual Frontier requires Vulkan 1.3 hardware с dedicated async compute queue family support.

**Minimum supported hardware tier**:
- NVIDIA Turing+ (GTX 16xx или RTX 20xx и newer)
- AMD RDNA 1+ (RX 5500 и newer)
- Intel Arc Alchemist+ (A380 и newer)

[full text per Commit 5]
```

### §4.6 — К10.3 amendment scope summary

Total К10.3 cross-document amendment scope:
- KERNEL_ARCHITECTURE.md v2.1 → v2.2 (5 К-L additions)
- VULKAN_SUBSTRATE.md v1.0 → v1.1 (8 sub-section amendments)
- MOD_OS_ARCHITECTURE.md v1.9 → v1.10 (5 amendment points)
- README.md (1 new section)
- REGISTER.yaml (incremental version bumps + 5 new REQs + audit_trail event)

Substantial cross-document propagation distributed across 4 load-bearing commits per Approach C grouped landing — each load-bearing commit lands its К-L invariant(s) + immediately related cross-document amendments atomically per Lesson #8.

---

## §5 — Halt triggers (К10.3-specific SC-N taxonomy)

К10.3 SC-N taxonomy expanded vs К10.1 (10 SC-N) + К10.2 (13 SC-N) — К10.3 imposes Vulkan code surface introducing new halt classes specific к GPU/hardware integration.

If execution agent encounters any of these conditions, **halt и surface к Crystalka**. Per Lesson #8 corollary: brief promises «halts before damage», not «zero halts». К10.1 + К10.2 both closed zero halts; К10.3 expanded scope may surface new shapes.

### SC-1 — Code anchor doesn't match spec evidence

Если any code anchor (Vulkan instance, queue family selection, RawTileField, GameBootstrap, scheduler.h, mod_unload.h) doesn't match spec's described shape after К10.2 closure (2026-05-18), halt. Brief authored 2026-05-18 from point-in-time read; subsequent drift surfaces here. К10.3 has **highest drift risk** since К10.1/К10.2 не trogали Vulkan code anchors; К10.3 brief authored from spec assumption, не from verified-recent Vulkan code reads.

### SC-2 — Vulkan integration race conditions

Если К10.3 native tests reveal race conditions, deadlocks, или data corruption в hot path (pipeline slot fence orchestration, Phase.Compute VkQueueSubmit batching, async compute queue submission), halt.

Recovery: stop-the-world debugging via Item 20 scheduler intrinsics (К10.1). Vulkan validation layer logs (in DEBUG builds) consulted. Surface к Crystalka before continuing.

### SC-3 — Deep-read contradiction

Any §2.3/§2.4 mandatory re-read surfaces a file shape that contradicts this brief. Halt и surface contradiction. К10.3 Vulkan-specific anchors **highest drift surface** per §2.4 note.

### SC-4 — Async compute queue absent on test runner hardware

Commit 4 (Item 44) hardware capability check fails fast if async compute queue absent. Если test runner / CI machine lacks async compute queue, К10.3 development workflow blocked.

Recovery: confirm Crystalka's «Skarlet» (ASUS TUF Gaming A16) hardware exposes async compute queue (likely yes — AMD GPU); document hardware tier requirement в session log. Если CI machine lacks: extend test mocks к simulate hardware capability detection без actual GPU enumeration. **Не halt К10.3 implementation** — hardware tier exclusion is architectural commitment, не development blocker.

### SC-5 — К-L invariant cross-reference integrity broken

Commits 5/11/14/17 (load-bearing) amend KERNEL_ARCHITECTURE.md, VULKAN_SUBSTRATE.md, MOD_OS_ARCHITECTURE.md. Если after любого load-bearing commit `sync_register.ps1 --validate` flags broken cross-references (e.g., К-L17 references К-L15 fast tier semantic that doesn't exist post-amendment, OR К-L7.1 references К-L7 row that has been incorrectly merged), halt.

Recovery: verify amendment text against spec verbatim; fix cross-reference; re-validate.

### SC-6 — Pipeline slot fence handling bug

Commit 6 (Item 33) lands PipelineSlot state machine. Если pipeline_slot_test surfaces incorrect fence handling (slot transitions Dispatched → ReadableAsTail without fence completion, OR fence completion не triggers transition), halt.

Recovery: verify state machine implementation; check atomic transition logic; surface если non-trivial Vulkan fence API misuse.

### SC-7 — К-L17 layer composition latency violation в tests

Commit 13 (Items 39+40) lands intent overlay + combat feedback layers. Если К-L17 tests show layer latency exceeding contracted bounds (intent overlay > 16ms, OR combat feedback total > 17ms event-to-visible), halt.

Recovery: profile rendering path; identify GC interaction, lock contention, или layer composition order overhead causing latency; surface к Crystalka — К-L17 semantic refinement may be required.

### SC-8 — К-L18 quiescent state false negative

Commit 15 (Item 41) extends mod_unload primitive с pipeline quiescence check. Если quiescent_state_test surfaces false negative (mod unload succeeds while pipeline has in-flight slots) или false positive (mod unload fails while pipeline empty), halt.

Recovery: verify quiescent state detection logic; check `df_pipeline_is_quiescent` API correctness; surface если К-L18 semantic ambiguity.

### SC-9 — К-L19 hardware capability check regression

Commit 4 (Item 44) lands HardwareCapabilityCheck. Если existing user-installed builds (pre-К10.3) start failing на hardware that previously worked (regression in capability detection logic), halt.

Recovery: verify capability detection respects existing hardware support; surface если hardware tier exclusion broader than intended.

### SC-10 — Cross-document amendment integrity broken

Commits 5/11/14/17 (load-bearing) amend multiple documents atomically. Если after commit one document references К-L invariant text differently than another (e.g., VULKAN_SUBSTRATE.md cites К-L16 verbatim differently than KERNEL_ARCHITECTURE.md), halt.

Recovery: cross-check verbatim text across all amended documents; restore exact consistency per Lesson #7.

### SC-11 — Validation regression post-commit

Если `sync_register.ps1 --validate` exits non-zero after any К10.3 commit, halt immediately. К10.3 cascade must не introduce new validation errors per К8.34 v2.0 + К10.1 + К10.2 precedents.

### SC-12 — Scope creep

Если execution encounters drift не в К10.3 scope (e.g., К10.4 TLA+ verification, К11+ Core migration к native, А'.8 К-closure report preparation), halt и surface. Do не «fix while we're here» — К10.3 scope discipline per S-LOCK-1.

### SC-13 — Push-to-main classifier reminder (operational, не halt)

Known behavior per К10.1 + К10.2 closures: Claude Code auto-mode classifier blocks push-to-main even с explicit instruction. Not a halt — expected. Re-confirm in-session after work done, then push.

### SC-14 — Vulkan code surface drift (Lesson #22 application)

К10.3-specific halt class. Если Phase 0 reads (§2.4) reveal Vulkan code surface substantially differs from brief assumptions (e.g., Silk.NET wrapper used vs pure P/Invoke; or Vulkan code lives в different project), halt and apply Lesson #22 «match existing convention». Surface к Crystalka для approach realignment before continuing.

Recovery options:
- Adapt К10.3 brief к existing Vulkan code shape (preferred per Lesson #22)
- Refactor Vulkan code surface к pattern brief assumes (only if Crystalka ratifies)
- Defer К10.3 Vulkan-specific items к later (К10.4 brief amendment or К-extensions)

При halting (SC-1..SC-12, SC-14): author HALT_REPORT в `docs/scratch/A_PRIME_7_K10_3/`, state trigger, state what was/wasn't committed, stop. **Do не commit partial atomic commit** — atomicity protects milestone per Lesson #8.

---

## §6 — Closure protocol (per METHODOLOGY §12.7 canonical)

After Commit 18 lands clean:

### §6.1 — Verify final state

1. `git log --oneline` shows ~18 commits added by К10.3 на feature branch `claude/k10_3-pipeline-display-hardware`
2. `git status` clean working tree
3. `sync_register.ps1 --validate` exit 0
4. `cmake --build` clean, native selftest passes (~102-112 scenarios — 77 К10.2 baseline + ~25-35 К10.3 new sections)
5. `dotnet build` clean, `dotnet test` 665+ green (К10.3 new tests additive — final count documented в closure entry)
6. К10.3 benchmarks per §5.1.A Predictions 11-17 measured + documented (results-as-measured)
7. Vulkan validation layer logs (in DEBUG builds) confirm no validation errors during pipeline slot operations или Phase.Compute submissions

### §6.2 — Update brief status + closure section

Set `status: EXECUTED` в this brief's frontmatter; add §8 closure section с commit range + date + commit ledger table + verification metrics + halt protocol activations + lesson candidates + pattern established (per К10.1 + К10.2 precedent).

Closure section template:
```markdown
## §8 — Closure (added at brief EXECUTED transition YYYY-MM-DD)

Execution closed YYYY-MM-DD by Claude Code auto-mode на branch `claude/k10_3-pipeline-display-hardware` from `main` HEAD <starting-sha>. Final commit <final-sha>.

### Commit ledger (commits <first>..<last>)

| # | Hash | Commit summary | Items closed |
|---|---|---|---|
| 1 | ... | brief authored | DOC-D-K10_3 enrollment |
| ... | ... | ... | ... |
| 18 | ... | governance closure | EVT-K10_3-CLOSURE |

### Verification metrics (final state)

- `git status`: clean working tree on branch `claude/k10_3-pipeline-display-hardware`
- `sync_register.ps1 --validate`: exit 0
- `cmake --build`: 0 warnings, 0 errors
- `dotnet build`: 0 warnings, 0 errors
- `dotnet test`: <N> passed, 0 failed (target 665+ baseline preserved + К10.3 tests additive)
- Native selftest: <N> scenarios passed (77 К10.2 baseline + К10.3 new)
- К10.3 benchmarks Predictions 11-17: <measurement results>
- Vulkan validation layer (DEBUG): 0 validation errors during К10.3 operations

### Halt protocol activations
[Any SC-N halts that fired during execution + their resolution]

### Out-of-scope items deferred
- К10.4 scope: TLA+ formal verification (Items 18, 45, 46)
- Item 14: К11+ Core migration к native code
- Item 25: К-closure report (А'.8)

### Pattern established
[Patterns from К10.3 execution worth noting для К10.4 brief]

### Lesson candidates surfaced
[Anything worth bringing к К10.4 brief authoring deliberation]

### К-L invariant landings final state
К10.1: К-L6 SUPERSEDED + К-L12/L13/L14 (3 invariants)
К10.2: К-L15 (1 invariant)
К10.3: К-L7.1 sub + К-L16/L17/L18/L19 (4 invariants + 1 sub = 5 К-L additions)
Cumulative К-Lxx series post-К10.3: 20 invariants
К10.4 expected: 0 new К-L (TLA+ verification spec only)
```

### §6.3 — PR opening (NOT auto-push, per К10.1 + К10.2 precedent)

- Push branch `claude/k10_3-pipeline-display-hardware` к remote (NOT к `main`)
- Open PR titled «К10.3 — Pipeline depth + display composition + quiescent + hardware tier (К-L7.1/L16/L17/L18/L19)»
- Body summarizes per-commit per-item mapping + verification metrics + halt activations (если any) + closure section
- **DO NOT auto-push к main**. Crystalka reviews + merges per established protocol

### §6.4 — Surface к Crystalka

PR ready для review. Crystalka:
1. Reviews К10.3 closure report content
2. Merges PR к `main`
3. Provides closure report к next Opus deliberation session для К10.4 brief authoring discussion

К10.4 brief authoring informed by:
- К10.3 closure report findings (halt activations, lesson candidates, patterns established)
- К10.3 architectural reality post-landing (5 new К-L invariants в KERNEL_ARCHITECTURE.md v2.2; pipeline depth operational; display composition operational; hardware tier mandated)
- Updated REGISTER state (К10.3 EXECUTED; К10.4 будет AUTHORED при brief authoring)
- К10.4 = **final К10 sub-milestone** — TLA+ formal verification (3 items: 18, 45, 46); likely **single comprehensive switching point** where managed facades retire и native sovereign authority activates across ALL К-L12..L19 invariants per managed-facade-preserved strategy от К10.1/2/3 inheritance

К10.4 scope context per spec §3.14:
- Item 18: TLA+ specification authoring covering 12 invariants (К-L3, К-L4, К-L5, К-L7, К-L7.1, К-L8, К-L11, К-L12, К-L13, К-L15, К-L16, К-L18)
- Item 45: Safety verification CI gate (~1-2 hour runtime)
- Item 46: Liveness verification targeted (К-L7.1 fence completion, К-L12 scheduler progress, К-L16 pipeline drain)

К10.4 no new К-L invariants — pure verification spec landing.

---

## §7 — Brief authority + lifecycle

**Brief authority**: К10 deliberation arc 2026-05-16 → 2026-05-17 (Crystalka + Claude Opus 4.7). 9 S-locks ratified в KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED. К10.3 standalone brief per Option III ratified 2026-05-18 (К10.1 + К10.2 closure precedents extended).

**Brief lifecycle (per FRAMEWORK §3.3 + §3.3.1)**:
- AUTHORED at this commit (Commit 1 of cascade)
- EXECUTED post-Commit 18 closure
- Registered в `tools/briefs/` as Tier 3 Category D per A'.4.5 governance
- AUTHORED-SKELETON → AUTHORED transition: not applicable here — К10.3 was authored fully from К10 deliberation arc closure + К10.1 + К10.2 closure context, не skeleton intermediate

**Brief enrollment**: К10.3 brief added к REGISTER.yaml в Commit 1 atomic с brief authoring per К10.1 + К10.2 precedents.

**Brief location**: `tools/briefs/K10_3_EXECUTION_BRIEF.md` after Crystalka copies from `/mnt/user-data/outputs/` per Filesystem MCP workaround pattern.

---

**End of brief. ~18 atomic commits across 12 К10 items + 5 К-L invariant landings + cross-document amendments (КERNEL_ARCHITECTURE + VULKAN_SUBSTRATE + MOD_OS + README). Expected 20-32 hours auto-mode execution.**

К10.3 closes 12 of 46 К10 items (cumulative с К10.1 + К10.2: 37 of 46). Remaining 9 items distributed across К10.4 (3 items: 18, 45, 46) + Item 14 deferred к К11+ + Item 25 cross-cutting к А'.8 + Item 10 NUMA deferred к К-extensions + Items 22/23 (hot-patching + RT guarantees) К-extensions scope.

К10 as a whole remains AUTHORED-IN-PROGRESS until К10.4 closure. К-series formally closes only после all four К10 sub-milestones + К-closure report (А'.8).

**К10.4 outlook**: К10.4 = TLA+ formal verification finalization. Architecturally significant beyond TLA+ alone — К10.4 likely **single comprehensive switching point** где managed-facade-preserved strategy (inherited К10.1 + К10.2 + К10.3) retires и native sovereign authority activates across all 8 К10-landed invariants (К-L12/L13/L14/L15/L7.1/L16/L17/L18/L19; К-L6 superseded). К10.4 brief authoring должен capture this architectural transition explicitly.

«Halt is success, не failure» per Lesson #8 corollary. Brief's honest guarantee: bad premises surface at Phase 0 / at deep-read / at the compile gate, before they reach `main`.
