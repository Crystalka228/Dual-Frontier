---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K10_3
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "2.0"
next_review_due: null
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K10_3
---
---
# Brief frontmatter (not REGISTER mirror — brief lives in tools/briefs/ as Tier 3 Category D)
brief_id: K10_3_EXECUTION_BRIEF_v2
status: EXECUTED
authored: 2026-05-19
ratified: 2026-05-19 (Crystalka ratified all 5 PENDING-RATIFICATION S-LOCKs via Q1+Q5/Q2/Q3/Q4 recommended-option lock)
executed: 2026-05-20 (15-commit atomic cascade 1982351..PENDING-COMMIT-K10_3-V2-CLOSURE; 4 К-L invariants AUTHORED; cumulative К-Lxx 20 invariants; К-L14 thesis seventh verification — zero hard-gate halts during execution)
author: Claude Opus 4.7 (Crystalka auto-mode session, Option D execution)
supersedes: K10_3_EXECUTION_BRIEF.md (v1, AUTHORED 2026-05-18, never enrolled, two Phase 0 halts)
target_executor: Claude Code (auto-mode + Crystalka oversight)
estimated_duration: 14-22 hours auto-mode (К10.3 scope reduced vs v1: К-L19 + Items 43/44 already landed в V0.B; revised cascade ~14 commits, 4 К-L additions)
brief_type: execution
amendments_chain:
  - K10_3_EXECUTION_BRIEF.md v1 (2026-05-18, never enrolled, supersession candidate)
  - docs/scratch/A_PRIME_7_K10_3/HALT_REPORT.md (2026-05-18 first halt SC-14 V substrate absent)
  - docs/scratch/A_PRIME_7_K10_3/HALT_REPORT_ADDENDUM_2026_05_19.md (2026-05-19 second halt SC-1+SC-3+SC-14 post-V substrate cascade)
authority_chain:
  - KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED (DOC-A-KERNEL_FULL_NATIVE_SCHEDULER) — primary spec для К10.3 scope (Items 33-42; Items 43-44 already landed V0.B)
  - K10_DELIBERATION_STATE.md (Project file sister) — S8 + S-TLA + К-L invariant rationale
  - KERNEL_ARCHITECTURE.md v2.2 LOCKED (post-V0.B) — К-L1..L15 + К-L19 already landed; К10.3 amends к v2.3 adding К-L7.1 sub + К-L16 + К-L17 + К-L18
  - VULKAN_SUBSTRATE.md v1.0 LOCKED (frontmatter) — content accumulated через V0.A/B/C.1/C.2/V1 cascades without version bump; К10.3 amends к v1.1 LOCKED с (a) K-L19 amendments deferred from V0.B and (b) К-L7.1/L16/L17/L18 amendments
  - MOD_OS_ARCHITECTURE.md v1.7 (last per content frontmatter; post-К10.2 capabilities tier prefix landed; needs version reconciliation) — К10.3 amends adding К-L17 layer caps + К-L18 quiescent note + §9.5 Step 3.6 + new ValidationErrorKinds
  - METHODOLOGY.md v1.8 LOCKED — Lessons #7/#8/#11/#20/#22 verbatim + Provisional Lessons + §12.7 canonical closure protocol
  - FRAMEWORK.md v1.1 LOCKED — Category D Tier 3 lifecycle transitions
  - K10_1_EXECUTION_BRIEF.md EXECUTED (DOC-D-K10_1, 2026-05-18) — К10.1 closure precedent
  - K10_2_EXECUTION_BRIEF.md EXECUTED (DOC-D-K10_2, 2026-05-18) — К10.2 closure precedent + Step 3.5 unload chain
  - V0_B_EXECUTION_BRIEF EXECUTED (V0.B PR #37, 2026-05-18) — К-L19 + Items 43/44 implementation backing; HardwareCapabilityCheck + QueueFamilyInfo + async compute queue infrastructure
  - V0_C_2_EXECUTION_BRIEF EXECUTED (V0.C.2 PR #39, 2026-05-19) — Runtime composition с Camera2D + SpriteRenderer batched
  - V1_V2_EXECUTION_BRIEF EXECUTED V1 portion (V1 PR #40, 2026-05-19) — V1 diffusion pipeline + dispatch_compute_field synchronous pattern (К-L7 baseline для К-L7.1 reasoning)
  - README.md — К-L19 «Hardware requirements» section landed V0.B cascade
---

# K10.3 v2 — Pipeline depth + display composition + quiescent state (К-L7.1/L16/L17/L18)

**Brief shape**: Execution-mode brief targeting Claude Code auto-mode с Crystalka oversight. Multi-commit atomic cascade implementing **10 architectural items** from KERNEL_FULL_NATIVE_SCHEDULER.md spec (Items 33-42; Items 43-44 already landed V0.B) that ratify **4 К-L invariants** — К-L7.1 (sub-invariant), К-L16 (pipeline depth), К-L17 (display composition), К-L18 (mod lifecycle quiescent).

**Status: AUTHORED (ratified 2026-05-19)** — §8 Q-decisions ratified by Crystalka 2026-05-19 (all 5 PENDING-RATIFICATION S-LOCKs locked per recommended options). Brief ready for Commit 1 enrollment.

**Authority**: Direct execution against К10 deliberation arc closure (2026-05-17) + Option D ratification 2026-05-19 (revised brief authored from current state per HALT_REPORT_ADDENDUM_2026_05_19 §6). К10.3 is the **third of four К10 sub-milestones** under Option III standalone-briefs structure ratified 2026-05-18. К10.1 closed 2026-05-18. К10.2 closed 2026-05-18. К10.4 = TLA+ formal verification, future brief.

**К10.3 v1 → v2 supersession context**:
- v1 brief authored 2026-05-18 expecting V substrate would be built к К10.3 execution time OR К10.3 lands architecturally-only
- v1 halted Phase 0 SC-14 2026-05-18 (V substrate absent → Crystalka Option B)
- V0.A → V0.B → V0.C.1 → V0.C.2 → V1 cascades landed 2026-05-18..2026-05-19 (5 closure PRs, main `070be85` → `88aebf2`)
- V0.B landed К-L19 + Items 43+44 as cross-stream prerequisite resolution per KERNEL_ARCHITECTURE.md line 67 «К-L19 lands V0.B per cross-stream prerequisite resolution»
- v1 brief re-loaded 2026-05-19, Phase 0 SC-1+SC-3+SC-14 fired (HALT_REPORT_ADDENDUM_2026_05_19) — v1 Commits 3/4/5 duplicate landed work
- Crystalka chose Option D (revised brief from current state) per addendum §6 recommendation
- **v2 brief = Option D execution artifact**

**К10.3 v2 ratifies architecturally** (per К10 deliberation closure + spec Part 2.4, minus К-L19 done):
- **К-L7.1 AUTHORED** (sub-invariant к К-L7): GPU compute pipeline slot binding extends К-L7 span protocol to pipeline-managed GPU memory (Items 33, 36)
- **К-L16 AUTHORED** (Items 33-37): Simulation tick pipeline depth (D=2 default)
- **К-L17 AUTHORED** (Items 38-40): Display composition multi-layer (world + intent + combat feedback)
- **К-L18 AUTHORED** (Items 41-42): Mod lifecycle quiescent state precondition
- ~~К-L19 (Items 43-44)~~ **LANDED V0.B 2026-05-18** — inherited from V0.B closure, не К10.3 scope

**К10.3 v2 scope discipline (Lesson #20 + Lesson #22 application)**:
- In-scope: 10 К10 items (33, 34, 35, 36, 37, 38, 39, 40, 41, 42) establishing pipeline depth + display + quiescent state (per spec §3.10-3.12)
- Out-of-scope: К10.4 (TLA+ formal verification) — separate brief
- Out-of-scope: Item 14 (К11+ Core migration) — explicitly deferred per spec §3.4
- Out-of-scope: Item 25 (К-closure report) — cross-cutting к А'.8
- ~~Out-of-scope previously~~: Items 43-44 (hardware tier) — **LANDED V0.B**, brief authority inherited not re-implemented

**Strategic pattern inherited from К10.1 + К10.2 + V0.B**: К10.3 implementations land **managed-facade-preserved** where applicable. К-L16 pipeline depth substantial — pipeline slots actually allocated и operational with new opt-in pattern (Q1 coexistence model per §8.1); К-L7 sync dispatch path (V1 ExecuteIteration) preserved unchanged for backward compat. К-L17 display composition framework substantial — layer registry + IntentOverlayLayer + CombatFeedbackLayer operational; existing sprite rendering preserved as default SimState layer. К-L18 quiescent state enforcement substantial — pause precondition enforced at native + UI helper layer; full settings menu deferred к К-extensions.

**Key architectural fact**: K10.3 v2 is **first К10 sub-milestone touching V substrate as consumer** (V0.A/B/C.1/C.2/V1 built V substrate; К10.3 v2 uses it). Phase 0 reads cover V substrate code anchors verified against current state.

**Brief size note**: К10.3 v2 brief ~1500-1700 lines vs v1's 1923 lines — scope reduction (К-L19 done) + clarity (HALT_REPORT addendum captured prior context) reduces lengths; ~14 commits vs v1's 18.

---

## §0 — What changed since v1 brief authoring

| Aspect | v1 (2026-05-18) | v2 (2026-05-19) |
|---|---|---|
| `main` HEAD | `070be85` (K10.2 closure) | `88aebf2` (V1 PR #40 merge) |
| Test count (full sln) | 665 | 936 post-V0.C.2 + V1 additive (re-measure at Phase 0) |
| Native scenarios | 77 (59 K10.1 + 18 K10.2) | Larger (V0.B/V1 additions; re-measure at Phase 0) |
| V substrate state | Absent | V0.A/B/C.1/C.2/V1 closed |
| Vulkan code anchors | Absent | Exist at `src/DualFrontier.Runtime/Graphics/` + `src/DualFrontier.Runtime/Compute/` |
| К-L19 invariant | Pending К10.3 landing | LANDED V0.B (KERNEL_ARCHITECTURE.md v2.2 row 65) |
| Items 43+44 | Pending К10.3 implementation | LANDED V0.B (`HardwareCapabilityCheck.cs` + `QueueFamilyInfo.cs` + `Runtime.Create` integration) |
| README hardware section | Pending К10.3 landing | LANDED V0.B cascade |
| V1 dispatch path | Not yet authored | Live: `V1DiffusionPipeline.ExecuteIteration` synchronous К-L7 pattern (`compute_dispatch.h` line 9 `Sync model: К-L7 atomic-from-observer`) |
| KERNEL_ARCHITECTURE.md version | v2.1 expected | v2.2 actual (post-V0.B) → v2.3 target post-К10.3 (К-L7.1/L16/L17/L18) |
| VULKAN_SUBSTRATE.md version | v1.0 expected → v1.1 target | v1.0 frontmatter, content advanced без version bump — К10.3 reconciles + amends к v1.1 |
| Cascade size | 18 commits | ~14 commits (К-L19 cluster removed) |
| К-L additions | 5 (К-L7.1+L16+L17+L18+L19) | 4 (К-L7.1+L16+L17+L18; К-L19 inherited) |

---

## §1 — Crystalka ratified scope locks (К10 deliberation 2026-05-16..2026-05-17, applicable subset для К10.3 v2)

### §1.1 — S-LOCK-1: К10.3 v2 scope items (10 items)

**LOCK**: К10.3 v2 implements exactly these 10 items from KERNEL_FULL_NATIVE_SCHEDULER.md spec, in dependency order:

| Item | Title | Spec § | Group |
|---|---|---|---|
| 33 | Tick pipeline depth mechanism (S8-Q1 + S8-Q2) | §3.10 | Pipeline depth foundation |
| 35 | Phase.Compute scheduler integration | §3.10 | Pipeline depth |
| 36 | Pipeline slot read API (S8-Q2 Pattern C) | §3.10 | Pipeline depth |
| 34 | Pipeline drain/refill protocols | §3.10 | Pipeline depth |
| 37 | Filter primitive integration с pipeline slot transitions | §3.10 | Pipeline depth |
| 38 | Display composition framework | §3.11 | Display composition |
| 39 | Intent overlay layer infrastructure | §3.11 | Display composition |
| 40 | Combat feedback layer infrastructure | §3.11 | Display composition |
| 41 | К-L18 quiescent state enforcement | §3.12 | Mod lifecycle quiescent |
| 42 | Settings menu / mod management UI integration с К10 | §3.12 | Mod lifecycle quiescent |

~~| 43 | Async compute queue mandate | §3.13 | LANDED V0.B |~~
~~| 44 | Hardware capability check at startup | §3.13 | LANDED V0.B |~~

**Item ordering rationale (revised)**:
- **Items 33+35+36 first** — pipeline slot mechanism + Phase.Compute + slot read API form the К-L7.1/L16 architectural cluster; co-design natural per Lesson #11 (architectural reduction)
- **Item 34** — drain/refill follows once slot mechanism + read API exist; persistence integration last in pipeline depth group
- **Item 37** — filter primitive integration consumes Item 33 slot transitions + reuses К10.1 Item 17 filter primitive
- **Items 38-40 (display composition)** — К-L17 cluster builds on pipeline depth (Item 38 consumes slot tail; Items 39/40 are layer-side additions independent of pipeline)
- **Items 41-42 (mod lifecycle)** last — К-L18 quiescent state precondition consumes Item 33 (pipeline state) + extends К10.2 Item 32 unload primitive

### §1.2 — S-LOCK-2: К-L invariants landing strategy (Approach C grouped, revised)

**LOCK** (inherited from v1 S-LOCK-2 Crystalka ratification 2026-05-18, adjusted for v2 scope): **Approach C grouped landing**. 3 load-bearing commits для 4 К-L invariants:

1. **К-L7.1 + К-L16 combined commit** (after Items 33+35+36+34+37) — К-L7.1 sub-invariant и К-L16 pipeline depth tied к одной physical reality (GPU pipeline slots); grouped landing preserves К-L7 invariant integrity per Lesson #11
2. **К-L17 commit** (after Items 38-40) — display composition multi-layer
3. **К-L18 commit** (after Items 41-42) — mod lifecycle quiescent state

(К-L19 commit removed — landed V0.B Commit 13 per V0.B brief.)

**Rationale**:
- Approach C balances atomicity (per Lesson #8) с attribution clarity (per Lesson #11)
- К-L7.1 + К-L16 combined: same physical reality (pipeline slots); structural tie justifies grouped landing
- К-L17 separate: display composition concern independent от pipeline mechanism architecturally
- К-L18 separate: mod lifecycle concern independent от GPU pipeline mechanism architecturally
- 1 fewer load-bearing commit vs v1 (К-L19 removed) → tighter cascade

### §1.3 — S-LOCK-3: К-L7.1 sub-invariant landing pattern (mirror К-L3.1 precedent) ✓ LOCKED

**LOCK**: К-L7.1 lands as **sub-invariant к К-L7** (mirror К-L3.1 sub-invariant к К-L3 precedent 2026-05-10). К-L invariant table treatment:

```
| К-L7 (+К-L7.1) | LOCKED (К-L7.1 added К10.3) | Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1) |
```

К-L7.1 text (verbatim from spec Part 2.4 line 238):

> «К-L7.1: GPU compute writes to RawTileField storage are bound к pipeline slot (К-L16). Sim-thread reads of compute-managed fields see slot-tail state — sim tick T+D reads dispatched-at-(T+D-1) state. One-tick lag from sim-perspective bounded и deterministic. К-L7 atomic-from-observer invariant preserved within pipeline slot boundary; cross-slot reads see different snapshots.»

**Critical interpretation (resolves Q1 §8.1)**: К-L7.1 governs **pipeline-managed fields** opted into pipeline depth machinery. V1's existing synchronous `V1DiffusionPipeline.ExecuteIteration` pattern (К-L7 atomic-from-observer per `compute_dispatch.h` line 9 explicit comment) remains operational for V1 consumers; К-L7.1 introduces opt-in pipeline-managed alternative для new consumers. Per К-L9 «Vanilla = mods», mods choose per-field whether to use К-L7 sync pattern (current V1) or К-L7.1 async pipeline-managed pattern (new К10.3).

### §1.4 — S-LOCK-4: К-L16 pipeline depth (D=2 default) ✓ LOCKED

**LOCK** (per S8-Q1 + S8-Q2 К10 ratification): Configurable depth D = 1-3 (default 2). Simulation thread runs D ticks ahead of display thread for pipeline-managed dispatches.

К-L16 text (verbatim from spec Part 2.4 line 246).

Pipeline slot data model (per spec §3.10 Item 33 verbatim line 965-973):

```c++
struct PipelineSlot {
    uint64_t sim_tick;
    NativeWorld snapshot;
    FieldStorageSnapshot fields;        // S8-Q2: К-L7.1 binding
    FenceHandle compute_fence;
    enum { Dispatched, FenceCompleted, ReadableAsTail } state;
};
```

**Critical scoping (resolves Q5 §8.5)**: К-L16 pipeline depth operates over **Phase.Compute pipeline-managed dispatches** specifically. V1's existing `dispatch_compute_field` synchronous path remains operational и orthogonal к pipeline depth; V1 consumers experience no behavior change. Pipeline depth applies к **new pipeline-managed dispatches** registered through Phase.Compute machinery (Item 35).

### §1.5 — S-LOCK-5: К-L17 display composition multi-layer ✓ LOCKED

**LOCK** (per S8-Q3): Three-layer composition framework.

Layer taxonomy (per К-L17 + spec §3.11 lines 1049-1054):

| Layer | Source | Latency | Mutating? |
|---|---|---|---|
| Sim state | Pipeline slot tail (К-L16) for pipeline-managed; current state for К-L7 sync | D × tick_period or sub-tick | Yes (sim writes) |
| Intent overlay | Current input state | ≤16ms (60 FPS) | No (read-only) |
| Combat feedback | Fast tier consumers (К-L15) | ≤1ms + ≤16ms | No (read-only) |
| Static | Loaded assets | N/A | No |

К-L17 text (verbatim from spec Part 2.4 line 250).

**Critical interpretation (resolves Q3 §8.3)**: К-L17 layer abstractions land в `src/DualFrontier.Application/Display/` (NEW directory). Existing `src/DualFrontier.Application/Rendering/IRenderer.cs` interfaces preserved unchanged; `Display/` layer composition operates above the renderer abstraction. К-L17 is composition over substrate, not extension of renderer interfaces.

### §1.6 — S-LOCK-6: К-L18 mod lifecycle quiescent state ✓ LOCKED

**LOCK** (per S8-Q4): Pattern (b) delegate — mod lifecycle transitions require sim paused; scheduler delegate signals quiescent achieved; transition proceeds only post-signal.

К-L18 text (verbatim from spec Part 2.4 line 254).

**К10.2 forward dependency consumed**: К10.2 Item 32 `df_scheduler_unload_mod_native_state` already includes К-L18 precondition stub (per `mod_unload.h` lines 71-76: `df_scheduler_set_sim_paused` / `df_scheduler_is_sim_paused`). К10.3 v2 Item 41 extends this к include pipeline slot quiescence verification (all D slots в `ReadableAsTail` state or empty); Item 42 lands UI helper integration enforcing precondition.

**Critical interpretation (resolves Q4 §8.4)**: К-L18 UI integration in К10.3 v2 = **helper layer wiring** (SimulationStateController + ModMenuController pause hook), NOT full settings menu / preferences UI. Full UI framework (button/checkbox/textfield/scroll) is V-cycle or К-extensions scope. К10.3 v2 lands К-L18 architectural commitment + minimal helpers; settings menu integration deferred per managed-facade-preserved pattern.

### §1.7 — S-LOCK-7: ~~К-L19 hardware tier commitment~~ INHERITED FROM V0.B

К-L19 landed V0.B 2026-05-18 per `KERNEL_ARCHITECTURE.md` line 65 + V0.B brief Commit 13. К10.3 v2 inherits К-L19 as authority chain entry, no implementation work.

### §1.8 — S-LOCK-8: Atomic cascade (multi-commit ordered) + managed-facade-preserved ✓ LOCKED

**LOCK**: К10.3 v2 executes as multi-commit cascade. Per Lesson #8: К10.3 v2 items 33-42 have **clean intermediate states** because:
1. Pipeline depth infrastructure (Items 33+35+36+34+37) lands additive — pipeline slots allocated и operational, но V1 sync dispatch path continues к work unchanged (К-L16 only governs pipeline-managed dispatches per S-LOCK-4)
2. Display composition (Items 38-40) lands additive — layer registry operational; existing rendering path preserved as default SimState layer (via wrapper); intent/combat feedback layers opt-in для new use cases
3. Mod lifecycle quiescent state (Items 41-42) lands incremental — К10.2 Item 32 already includes quiescent precondition stub; К10.3 v2 Item 41 extends с pipeline state verification; Item 42 lands UI helper integration
4. К-L invariant landings happen incrementally per Approach C grouped (3 load-bearing commits)

Tests pass at every commit (existing 936+ baseline preserved at Phase 0 measurement; new К10.3 v2 tests additive).

### §1.9 — S-LOCK-9: К10.3 v2 closes К10.3 only ✓ LOCKED

**LOCK** (inherited from v1 S-LOCK-9): К10.3 v2 closure is sub-milestone closure (per FRAMEWORK §3.3 Lifecycle EXECUTED для К10.3 v2 brief). К-closure report (А'.8) waits для all four К10 sub-milestones closed (К10.1 ✅ + К10.2 ✅ + К10.3 v2 pending + К10.4 pending).

**Implications**:
- К10.3 v2 commit message scope: `feat(kernel): K10.3 v2 — pipeline depth + display composition + quiescent (К-L7.1/L16/L17/L18)`
- К10.3 v2 closure REGISTER entry: separate audit_trail event (`EVT-{date}-K10_3-CLOSURE`)
- К-L invariant landings: К10.1 landed К-L6 SUPERSEDED + К-L12/13/14; К10.2 landed К-L15; **V0.B landed К-L19**; К10.3 v2 lands К-L7.1 + К-L16 + К-L17 + К-L18; К10.4 has no new К-L (TLA+ verification spec only)

### §1.10 — S-LOCK-10 ✓ LOCKED (Q1 Crystalka 2026-05-19): K-L7.1 application policy (coexistence opt-in)

**LOCK**: К-L7.1 applies к pipeline-managed fields opted into Phase.Compute machinery. V1 `dispatch_compute_field` synchronous path (К-L7 atomic-from-observer) remains operational; К-L7.1 introduces opt-in pipeline-managed alternative. Per К-L9 «Vanilla = mods» uniformity — author choice per field.

### §1.11 — S-LOCK-11 ✓ LOCKED (Q3 Crystalka 2026-05-19): Display composition project location

**LOCK**: `src/DualFrontier.Application/Display/` (NEW directory). Existing `Application/Rendering/IRenderer.cs` preserved unchanged. Display composition operates above renderer abstraction. К-L17 cross-substrate concerns belong в Application layer.

### §1.12 — S-LOCK-12 ✓ LOCKED (Q4 Crystalka 2026-05-19): К-L18 UI integration scope

**LOCK**: К10.3 v2 К-L18 UI = helper layer wiring (SimulationStateController + ModMenuController pause hook). Full settings menu / preferences UI = V-cycle or К-extensions scope, deferred per managed-facade-preserved pattern.

### §1.13 — S-LOCK-13 ✓ LOCKED (Q5 Crystalka 2026-05-19): V1 sync dispatch coexistence с Phase.Compute

**LOCK**: V1's `dispatch_compute_field` (synchronous VkQueueSubmit + vkWaitForFences) remains unchanged. Phase.Compute (Item 35) introduces parallel async dispatch path для pipeline-managed dispatches. V1 consumers experience no behavior change. Future К-extensions могут deprecate sync path если pipeline-managed becomes universal.

### §1.14 — S-LOCK-14 ✓ LOCKED (Q2 Crystalka 2026-05-19): VULKAN_SUBSTRATE.md version reconciliation

**LOCK**: VULKAN_SUBSTRATE.md frontmatter v1.0 reconciled via combined version bump v1.0 → v1.1 incorporating: (a) deferred K-L19 amendments from V0.B (§0 L1, §0 L7, §3.4 K-L19 mandate documentation); (b) К-L7.1/L16/L17/L18 К10.3 v2 amendments per spec Part 7.5. Reconciliation distributed across К10.3 v2 load-bearing commits per Lesson #8 atomic compilable units.

---

## §2 — Phase 0 — Pre-flight reads (mandatory before any edit)

Per Lesson #7 (transcribe API verbatim) + Lesson #22 (read existing code before mechanism design), executor MUST complete every read listed below **before writing a single line of К10.3 v2 code**. К10.3 v2 brief authored 2026-05-19 from comprehensive Phase 0 ground-truth reads (Glob inventory + Read of code anchors + Spec verbatim reads) — drift between brief authoring и execution time expected to be lower than v1 (re-authored from current state) but still surfaces as halt triggers (SC-3) — never silent improvisation per Lesson #20.

### §2.1 — Verify post-V1 closure state (hard gates)

Read и verify (hard gates, blocking commits если failed):

1. `git log --oneline -20` on `main` — confirm:
   - V1 PR #40 merged (current HEAD `88aebf2` at brief authoring)
   - HEAD references V1 closure or later
   - Halt если post-V1 closure не reached — К10.3 v2 starts от post-V1 main

2. `git status` — working tree clean before execution starts. **Hard gate** per К8.34 v2.0 + К10.1 + К10.2 precedents.

3. `docs/governance/REGISTER.yaml` head check — confirm `register_version` is post-V1 closure baseline. Lower means К10.3 v2 не based on correct baseline.

4. `tools/governance/sync_register.ps1 --validate` — exit 0 required as baseline (advisory orphan warnings acceptable; K10_3 v1 brief in tools/briefs/ as orphan, K10_3 v2 brief enrollment adds 1 more transient orphan until Commit 1 enrollment).

5. `dotnet build DualFrontier.sln` — clean baseline.

6. `dotnet test DualFrontier.sln` — record baseline pass count при Phase 0 (target ≥936 per memory snapshot). Если suite fails или count diverges (excluding intentional К10.3 v2 additions), halt.

7. `cmake --build native/DualFrontier.Core.Native` через VS-bundled cmake at `D:\Visual Studio\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe` — clean baseline. Native selftest passes baseline scenarios.

### §2.2 — Read KERNEL_FULL_NATIVE_SCHEDULER.md spec (К10.3 v2 sections)

Read в полном объёме, identify exact line numbers для К10.3 v2 items:
- Part 2.4 К-L7.1 + К-L16 + К-L17 + К-L18 verbatim text (lines 236-254)
- Part 3.10 Items 33-37 (pipeline depth, lines 957-1039)
- Part 3.11 Items 38-40 (display composition, lines 1041-1091)
- Part 3.12 Items 41-42 (mod lifecycle quiescent, lines 1093-1121)
- ~~Part 3.13 Items 43-44~~ — landed V0.B, skip
- Part 5.1.A Predictions 11-15, 17 measurable К10.3 v2 scope (lines 1308-1320; Prediction 16 К-L19 done)
- Part 7.5 VULKAN_SUBSTRATE.md amendment list (lines 1611-1621)
- ~~Part 7.9 README.md~~ — landed V0.B, skip

**Per Lesson #7**: spec verbatim authoritative для К-L invariant text, PipelineSlot struct shape, three-layer composition framework taxonomy.

### §2.3 — Read VULKAN_SUBSTRATE.md (К10.3 v2 amendment surface + version reconciliation)

Read в полном объёме (1539 lines), identify amendment scope per spec Part 7.5:
- Frontmatter v1.0 (stale relative к V0.A/B/C.1/C.2/V1 content cumulative additions)
- §0 L1 — needs K-L19 async compute requirement annotation (deferred from V0.B)
- §0 L7 — Windows-only platform matches K-L19 hardware tier baseline annotation (deferred V0.B)
- §2 architecture — К10.3 v2 amendment: display composition section (К-L17)
- §2.3 threading model — К10.3 v2 substantial amendment: pipeline depth + async compute queue (К-L19 cleanup от V0.B) + sim-thread read pattern (К-L7.1)
- §3.4 native compute dispatch — К10.3 v2 amendment: `df_vulkan_unload_mod_resources` C ABI (Item 42 V scope) + К-L19 mandate documentation cleanup
- §4 rendering use case — К10.3 v2 amendment: display composition section (К-L17)
- §5.5 Mode C navigation — К10.3 v2 amendment: visibility latency через К-L17 composition
- §7.2 determinism — К10.3 v2 amendment: pipeline drain semantics (К-L16)
- §7.3 async sync hazards — К10.3 v2 amendment: pipeline slot tail read pattern (К-L7.1) + V1 sync coexistence (S-LOCK-13)

**Per Lesson #7**: VULKAN_SUBSTRATE.md verbatim sections preserved literally; К10.3 v2 amendments inserted preserving existing wording style.

### §2.4 — Read code anchors verbatim (К10.3 v2 V substrate + kernel surface)

Verified at brief authoring 2026-05-19; re-confirm at Phase 0:

**V substrate composition (Runtime facade)**:
- `src/DualFrontier.Runtime/Runtime.cs` — Runtime.Create composition с К-L19 HardwareCapabilityCheck.Verify integration line 83; V0.A+B+C.1+C.2 primitives; V1 factory methods `CreateFieldStorageBinding` line 355 + `CreateV1DiffusionPipeline` line 379
- `src/DualFrontier.Runtime/Compute/V1DiffusionPipeline.cs` — `ExecuteIteration` line 67 sync semantics («call returns after the compute fence signals» line 65-66)
- `src/DualFrontier.Runtime/Compute/FieldStorageBinding.cs` — managed wrapper для native field binding
- `src/DualFrontier.Runtime/Compute/ComputeDispatch.cs` — managed-side compute dispatch entry
- `src/DualFrontier.Runtime/Compute/ComputePipelineRegistry.cs` — managed-side pipeline registry

**V substrate display surface (К-L17 consumers)**:
- `src/DualFrontier.Runtime/Sprite/SpriteRenderer.cs` (V0.C.2 batched) — exists, K-L17 SimState layer wraps this
- `src/DualFrontier.Runtime/Sprite/Camera2D.cs` (V0.C.2) — exists, К-L17 composition consumes
- `src/DualFrontier.Runtime/Sprite/TileMap.cs` (V0.C.2) — exists, К-L17 SimState consumer
- `src/DualFrontier.Runtime/Input/*` (V0.C.1) — exists, К-L17 Intent overlay reads from InputEventQueue

**Native kernel surface (К10.1 + К10.2)**:
- `native/DualFrontier.Core.Native/include/compute_dispatch.h` — V1 sync dispatch (К-L7 baseline для К-L7.1 reasoning)
- `native/DualFrontier.Core.Native/include/compute_pipeline.h` — `VulkanAttachment` struct + `ComputePipelineRegistry` — K10.3 v2 Item 33 extends с pipeline slot fields
- `native/DualFrontier.Core.Native/include/state_change_filter.h` — К10.1 Item 17 filter primitive; К10.3 v2 Item 37 extends с pipeline slot transition triggers
- `native/DualFrontier.Core.Native/include/mod_unload.h` — К10.2 Item 32 unload primitive с `df_scheduler_set_sim_paused` К-L18 stub; К10.3 v2 Item 41 extends с pipeline quiescence check
- `native/DualFrontier.Core.Native/include/scheduling_policies.h` + `system_graph.h` + `wake_registry.h` + `phase_barrier.h` + `bus_native.h` — К10.1/К10.2 split layout (brief assumes split, no monolithic `scheduler.h`)

**Application composition (К-L18 + К-L17 integration anchors)**:
- `src/DualFrontier.Application/Loop/GameBootstrap.cs` — exists, may need integration с pipeline slot init (Item 33)
- `src/DualFrontier.Application/Loop/GameLoop.cs` — exists, sim/display tick coordination potential touch point
- `src/DualFrontier.Application/Loop/FrameClock.cs` — exists, display frame timing
- `src/DualFrontier.Application/Modding/ModMenuController.cs` — exists, К-L18 UI integration anchor (Item 42)
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` — exists, К10.2 unload chain owner; К10.3 v2 Item 42 extends с Step 3.6 V resource cleanup
- `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs` — exists, К10.3 v2 Items 39+40 extend с layer capability tokens
- `src/DualFrontier.Application/Rendering/IRenderer.cs` + `IDevKitRenderer.cs` — exists, preserved unchanged per S-LOCK-11

**FieldStorage + RawTileField (К-L7 → К-L7.1 extension)**:
- `src/DualFrontier.Core/Fields/*` (assumed exists post-К9 — verify Phase 0) — RawTileField К-L7 span protocol extension к К-L7.1

**Test fixtures (post-V1)**:
- `tests/DualFrontier.Runtime.Tests/Graphics/*` — К-L19 + V0.A/B coverage
- `tests/DualFrontier.Runtime.Tests/Compute/*` — V1 dispatch coverage (K-L7 baseline)
- `tests/DualFrontier.Core.Tests/` — К10.1/К10.2 scheduler tests
- `tests/DualFrontier.Application.Tests/` — does this exist? Phase 0 verifies; if absent, К10.3 v2 creates per Items 38+42 test requirements

**Если any read surfaces a contradiction с this brief** — halt per SC-3.

### §2.5 — Read MOD_OS_ARCHITECTURE.md (К10.3 v2 amendment surface)

Read post-К10.2 state:
- `docs/architecture/MOD_OS_ARCHITECTURE.md`:
  - Frontmatter version (verify current; v2 brief expects post-К10.2 with К-L15 + К10.2 tier-prefixed token additions)
  - §3.2 tier-prefixed capability syntax (К10.2 amendment line 287+) — К10.3 v2 extends с К-L17 layer capability tokens (`kernel.layer.intent:{FQN}` + `kernel.layer.combat_feedback:{FQN}`)
  - §4.6 IModApi v3 strict (Fields + Compute Pipelines) — К10.3 v2 may need layer registration capability surface mention
  - §9.5 8-step unload chain (К10.2 amendment с Step 3.5 line 965) — К10.3 v2 Item 42 adds Step 3.6 V resource cleanup → 9-step chain post-К10.3 v2
  - §9.5.1 failure semantics (К10.2 line 975) — extend с Step 3.6 inclusion
  - §11.2 ValidationErrorKind enum (line 1058) — К10.3 v2 adds new kinds:
    - `QuiescentStatePreconditionViolated` (К-L18)
    - `PipelineQuiescenceTimeout` (К-L18)
    - `LayerCapabilityMismatch` (К-L17)
    - `VulkanModResourceCleanupFailed` (К-L18 V scope)

**Per Lesson #7**: MOD_OS §9.5 К10.2 amended state preserved verbatim; К10.3 v2 Step 3.6 insertion text matches existing wording style.

### §2.6 — Read REGISTER.yaml К10.3 v2 enrollment area

Identify exact line ranges:
- DOC-A-KERNEL (KERNEL_ARCHITECTURE.md v2.2 post-V0.B) — К10.3 v2 amends к v2.3 (4 К-L invariants added — К-L7.1 sub + К-L16 + К-L17 + К-L18; К-L19 already enrolled V0.B)
- DOC-A-VULKAN_SUBSTRATE (VULKAN_SUBSTRATE.md v1.0 frontmatter) — К10.3 v2 amends к v1.1 (8 sub-section amendments per spec Part 7.5, including deferred K-L19 amendments)
- DOC-A-MOD_OS — К10.3 v2 amends (К-L17 layer caps + К-L18 quiescent note + Step 3.6 + new ValidationErrorKinds)
- DOC-A-KERNEL_FULL_NATIVE_SCHEDULER (К10 spec v2.0 LOCKED) — governance_events append для К10.3 v2 closure
- DOC-D-K10_3 brief enrollment (NEW: K10_3_EXECUTION_BRIEF_v2.md as primary; K10_3_EXECUTION_BRIEF.md v1 superseded annotation)
- Audit_trail events list (К10.3 v2 adds EVT-{date}-K10_3-CLOSURE; HALT_REPORT + ADDENDUM events may be recorded retroactively for record)
- Requirements: K10.3 v2 enrolls REQ-K-L7_1 + REQ-K-L16 + REQ-K-L17 + REQ-K-L18 (4 REQs; REQ-K-L19 already enrolled V0.B per line 4336)

### §2.7 — Halt category clarity

**Hard gates (STOP-eligible)** per §2.1 + К10.1 + К10.2 precedents:
- Working tree dirty
- Baseline tests failing (excluding intentional changes)
- `sync_register.ps1 --validate` non-zero baseline
- Build failure baseline
- Post-V1 closure не reached

**Informational checks (record-only)**:
- HEAD SHAs, branch topology
- Test count + native scenario count actuals at Phase 0
- VULKAN_SUBSTRATE.md content state vs frontmatter version (expected stale; Q2 §8.2 resolves в S-LOCK-14)

Если informational check diverges from brief expectation — **record divergence в Commit message, continue**. Hard gate failure → halt per SC-N (§5).

---

## §3 — Atomic commit cascade (target ~14 commits)

Each commit: individually compilable + register-valid intermediate state per Lesson #8. `sync_register.ps1 --validate` exit 0 at every governance-touching commit; `dotnet build` clean + `cmake --build` clean at every code-touching commit; `dotnet test` baseline+ passing at every commit (936+ baseline preserved; new К10.3 v2 tests additive).

**Cascade strategy** per S-LOCK-8 + К10.1/К10.2 precedent: pipeline depth foundation first (Items 33+35+36+34+37), display composition (Items 38-40), mod lifecycle (Items 41-42). 3 К-L invariant landing commits distributed throughout per Approach C grouped landing.

### Commit 1 — Brief authoring + v1 supersession enrollment

**Files**:
- `tools/briefs/K10_3_EXECUTION_BRIEF_v2.md` (this brief)
- `tools/briefs/K10_3_EXECUTION_BRIEF.md` (v1; add frontmatter `superseded_by: K10_3_EXECUTION_BRIEF_v2.md` annotation, retain on disk for historical record)
- `docs/governance/REGISTER.yaml` (DOC-D-K10_3 entry: lifecycle AUTHORED, references K10_3_EXECUTION_BRIEF_v2.md; v1 brief retained as historical orphan per acceptable advisory)

**Validation**:
- `sync_register.ps1 --validate` exit 0 (advisory orphan от v1 acceptable)
- No code changes

**Commit message**: `docs(briefs): K10.3 v2 brief authored — pipeline depth + display composition + quiescent (К-L7.1/L16/L17/L18)`

### Commit 2 — Phase 0 verification + native test scaffold

**Files**:
- `native/DualFrontier.Core.Native/test/selftest.cpp` (extended с К10.3 v2 placeholder section — empty section ready, follows К10.1/К10.2 DF_CHECK runner pattern per Lesson #22)

**Validation**:
- `cmake --build` clean
- `cmake --build --target run_native_tests` — baseline scenarios pass (К10.3 v2 placeholder vacuous)
- `dotnet build` clean
- `dotnet test` baseline+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(native-test): K10.3 v2 — native test scaffold (DF_CHECK runner extension)`

### Commit 3 — Item 33: Tick pipeline depth mechanism (S8-Q1 + S8-Q2)

**Files**:
- `native/DualFrontier.Core.Native/include/pipeline_slot.h` (NEW)
- `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (NEW — PipelineSlot allocation, state machine, fence orchestration)
- `native/DualFrontier.Core.Native/test/pipeline_slot_test.cpp` (NEW selftest section — D=2 default coverage)
- `src/DualFrontier.Core.Interop/PipelineSlotInterop.cs` (NEW managed binding)

**Drift surface**: К-L16 pipeline depth infrastructure landed greenfield. К-L7.1 sub-invariant landed parallel в same commit grouping (Approach C). V1 sync dispatch path unchanged per S-LOCK-13. Pipeline slot infrastructure operational; **no V1 consumer behavior change** (К-L16 governs pipeline-managed dispatches only per S-LOCK-4).

**Implementation surface (per spec §3.10 Item 33 verbatim)**:

```c
// pipeline_slot.h verbatim from spec §3.10 Item 33

typedef enum {
    SlotState_Dispatched = 0,
    SlotState_FenceCompleted = 1,
    SlotState_ReadableAsTail = 2,
    SlotState_Empty = 3  // initial state, before sim_tick assigned
} SlotState;

typedef struct {
    uint64_t sim_tick;
    void* world_snapshot_ptr;       // NativeWorld snapshot pointer (К-L7.1 binding subject)
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

**Per Lesson #7 + S8-Q2**: PipelineSlot struct shape preserved literally from spec.

**Per S8-Q1 + К-L16**: Default D=2, configurable 1-3. Configuration policy: compile-time `#define DF_PIPELINE_DEPTH=2` with runtime override via `df_pipeline_init` parameter (final policy verified at Phase 0 against existing configuration patterns).

**Slot state machine**:

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
- `dotnet test` baseline+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 v2 Item 33 — tick pipeline depth mechanism (К-L7.1 + К-L16 foundation; D=2 default; V1 sync coexists)`

### Commit 4 — Item 35: Phase.Compute scheduler integration

**Files**:
- `native/DualFrontier.Core.Native/include/phase_compute.h` (NEW per split layout convention; не monolithic scheduler.h)
- `native/DualFrontier.Core.Native/src/phase_compute.cpp` (NEW — Phase.Compute dispatch path + VkQueueSubmit batching)
- `native/DualFrontier.Core.Native/test/phase_compute_test.cpp` (NEW selftest section)
- `native/DualFrontier.Core.Native/src/scheduling_policies.cpp` (possible modification — Phase.Compute placement in tick lifecycle)

**Drift surface**: К10 scheduler gains Phase.Compute phase between Phase.Update (sim writes) и Phase.Display (display reads slot tail) for **pipeline-managed dispatches**. VkQueueSubmit batching: all compute dispatches from active pipeline-managed dispatch systems coalesced into single submit per tick (per Prediction 12 ~50-100μs savings). **V1's `dispatch_compute_field` synchronous path unchanged per S-LOCK-13** — Phase.Compute is alternative dispatch path, not replacement.

**Implementation surface (per spec §3.10 Item 35)**:

```c
// phase_compute.h
typedef enum {
    Phase_Update = 0,      // sim writes (existing)
    Phase_Compute = 1,     // К10.3 v2 NEW: GPU compute dispatches per pipeline slot (pipeline-managed only)
    Phase_Display = 2,     // display reads slot tail (existing, may be renamed if needed)
    // ... other existing phases
} Phase;

// Phase.Compute dispatch
int32_t df_scheduler_dispatch_phase_compute(PipelineSlot* current_slot);

// Compute dispatch registration (called by pipeline-managed dispatch consumers during system update)
int32_t df_scheduler_register_compute_dispatch(
    PipelineSlot* slot,
    void* pipeline_handle,      // VkPipeline opaque (from existing ComputePipelineRegistry)
    void* descriptor_set,        // VkDescriptorSet opaque
    int dispatch_x, int dispatch_y, int dispatch_z);

// VkQueueSubmit batching
int32_t df_scheduler_submit_compute_batch(PipelineSlot* slot, void* async_compute_queue);
```

**Async compute queue consumed (К-L19 + Item 43 V0.B)**: Phase.Compute submits к dedicated async compute queue obtained at V0.B `Runtime.Create` (per `VulkanDevice.AsyncComputeQueueFamilyIndex`); graphics queue не used для compute. Reuses existing `VulkanAttachment.async_compute_queue` field (per compute_pipeline.h line 57).

**К10.3 v2 boundary**: К10.3 v2 establishes Phase.Compute infrastructure + VkQueueSubmit batching mechanism; actual pipeline-managed compute consumers (mods, vanilla systems opting in) outside К10.3 v2 scope. V1 sync path remains operational в parallel.

**Validation**:
- `cmake --build` clean
- `phase_compute_test`: Phase.Compute phase enumerated; dispatch registration accumulates; submit batching coalesces multiple dispatches into single VkQueueSubmit (verified via Vulkan validation layer logs в DEBUG)
- `dotnet test` baseline+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 v2 Item 35 — Phase.Compute scheduler integration (VkQueueSubmit batching к async compute queue; V1 sync coexists)`

### Commit 5 — Item 36: Pipeline slot read API (S8-Q2 Pattern C)

**Files**:
- `native/DualFrontier.Core.Native/include/pipeline_slot.h` (extended — slot tail read API)
- `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (extended)
- `native/DualFrontier.Core.Native/test/pipeline_slot_test.cpp` (extended)
- `src/DualFrontier.Core/Fields/RawTileField.cs` (modified — К-L7.1 read pattern opt-in for pipeline-managed fields)

**Drift surface**: Sim-thread reads from slot tail для **pipeline-managed fields** (`SimTick - 1` reads dispatched-at-`(SimTick - 1) - 1` results); display thread reads from slot tail (`SimTick - D`). К-L7 atomic-from-observer preserved within pipeline slot boundary. V1 `dispatch_compute_field` sync path reads remain unchanged (К-L7 baseline preserved).

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
// RawTileField.cs К-L7.1 opt-in read pattern (S-LOCK-10 coexistence)
public ReadOnlySpan<T> AcquireSpan()
{
    if (_isPipelineManaged)  // К-L7.1 opt-in — set at field registration
    {
        // К-L7.1 sub-invariant: sim-thread reads see slot tail state
        // (one-tick lag bounded и deterministic per К-L16 D=2 → reads from sim_tick - 1)
        var slotTail = PipelineSlotInterop.GetSlotTail(-1);  // sim_tick - 1
        return new ReadOnlySpan<T>(slotTail.FieldsSnapshotPtr, _width * _height);
    }
    else
    {
        // К-L7 sync semantics (V1 default; preserved)
        return _backingStore.AsSpan();
    }
}
```

**Per К-L7.1 verbatim**: «sim tick T+D reads dispatched-at-(T+D-1) state. One-tick lag from sim-perspective bounded и deterministic.» Cross-slot reads see different snapshots — К-L7.1 establishes this explicitly. Within slot, К-L7 atomic-from-observer preserved.

**К10 forward integration (Item 37 next)**: Slot transition Dispatched → ReadableAsTail fires StateChangeWake via filter primitive integration; Item 37 wires this.

**Validation**:
- `cmake --build` clean
- `pipeline_slot_test` extended: read API correct slot_offset interpretation; error handling для Dispatched-state slots; К-L7.1 opt-in vs К-L7 default verified
- `dotnet build` clean
- `dotnet test` baseline+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 v2 Item 36 — pipeline slot read API (К-L7.1 opt-in coexists с К-L7 sync default)`

### Commit 6 — Item 34: Pipeline drain/refill protocols

**Files**:
- `native/DualFrontier.Core.Native/include/pipeline_slot.h` (extended — drain/refill C ABI)
- `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (extended)
- `src/DualFrontier.Persistence/Pipeline/PipelineSlotSerializer.cs` (NEW — save/load integration per S8-Q1.5)
- `src/DualFrontier.Persistence/SaveFileFormat.cs` (extended — `pipeline_slot_snapshot` section added) — verify exact file path at Phase 0
- `tests/DualFrontier.Persistence.Tests/PipelineSlotSerializerTests.cs` (NEW)

**Drift surface**: Orderly pipeline drain at save/pause; orderly pipeline refill at load/resume. К10.2 precedent (background queue save-integrated storage Item 31) extended pattern к pipeline slot snapshots.

**Implementation surface (per spec §3.10 Item 34 + S8-Q1.5)**:

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
int32_t df_pipeline_pause(void);

// Resume protocol: refill from pause point
int32_t df_pipeline_resume(void);
```

**Save protocol semantics** (per S8-Q1.5):
- Snapshot display tick state (CurrentSimTick - D) — display already sees coherent world
- Pipeline drain NOT required at save time — display tick state captures coherent world
- Faster save (no waiting для in-flight compute completion)

**Pause protocol semantics**:
- Natural convergence: sim thread completes current tick, no new dispatch
- Pipeline depth naturally absorbs already-dispatched work
- К-L18 quiescent state precondition (Item 41) verifies pipeline quiesced before mod operations

**Validation**:
- `cmake --build` clean
- `dotnet build` clean
- `PipelineSlotSerializerTests`: round-trip serialization; save protocol snapshots display tick state; load protocol refills correctly
- Pipeline drain/refill scenarios tested via pipeline_slot_test selftest extension
- `dotnet test` baseline+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(persistence): K10.3 v2 Item 34 — pipeline drain/refill protocols + save-integrated pipeline slot storage`

### Commit 7 — Item 37: Filter primitive integration с pipeline slot transitions

**Files**:
- `native/DualFrontier.Core.Native/src/state_change_filter.cpp` (modified — pipeline slot transition triggers filter check)
- `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (modified — `transition_to_tail` fires StateChangeWake)
- `native/DualFrontier.Core.Native/test/pipeline_slot_wake_test.cpp` (NEW selftest section)
- `src/DualFrontier.Contracts/Scheduling/WakeOnSlotTransitionAttribute.cs` (NEW — `[WakeOnSlotTransition]` для systems wanting wake-on-fence-completion)

**Drift surface**: К-L13 on-demand activation (К10.1 Item 3 + Item 17 filter primitive) extended с pipeline slot transitions. Downstream pipeline-managed read systems wake when fence completes на slot tail.

**Implementation surface (per spec §3.10 Item 37)**:

```c
// pipeline_slot.cpp — transition_to_tail extension
int32_t df_pipeline_transition_to_tail(PipelineSlot* slot)
{
    // Atomic state transition: FenceCompleted → ReadableAsTail
    SlotState expected = SlotState_FenceCompleted;
    if (!atomic_compare_exchange(&slot->state, expected, SlotState_ReadableAsTail))
    {
        return DF_ERR_INVALID_SLOT_STATE;
    }
    
    // К10.3 v2 Item 37 NEW: trigger StateChangeWake для slot-subscribed systems
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
```

**Atomic slot transition + filter check (К-L7 batch-commit semantics)**: no observer sees torn slot state; filter check happens after atomic state transition completes.

**Validation**:
- `cmake --build` clean
- `pipeline_slot_wake_test`:
  - Slot transition fires StateChangeWake к subscribed systems
  - Filter primitive Level 1 cold-path bypass measurable
  - Atomic transition correctness (no torn state observed)
- `dotnet test` baseline+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 v2 Item 37 — filter primitive + pipeline slot transition wake integration`

### Commit 8 — К-L7.1 + К-L16 invariants landing + VULKAN_SUBSTRATE.md amendments (LOAD-BEARING 1 of 3)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.2 partial → v2.3 (К-L7 row extended с К-L7.1 sub; К-L16 row added; Part 2 К10 row updated с К-L7.1/L16 ratified annotation)
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.0 → v1.1 substantial (consolidates: deferred K-L19 amendments §0 L1/§0 L7/§3.4 from V0.B + К10.3 v2 K-L7.1/L16 amendments §2/§2.3/§7.2/§7.3)
- `docs/governance/REGISTER.yaml` (DOC-A-KERNEL version 2.2 → 2.3 partial — К-L7.1/L16 only; DOC-A-VULKAN_SUBSTRATE version 1.0 → 1.1; governance_events append references)

**This is the first load-bearing commit of К10.3 v2** (К-L7.1 + К-L16 grouped landing per Approach C). Also performs VULKAN_SUBSTRATE.md version reconciliation per S-LOCK-14 (consolidates V0.B-deferred amendments + К10.3 v2 amendments).

**К-L invariant table amendments**:

К-L7 row amended (extending с К-L7.1 sub-invariant per К-L3.1 precedent):
```
| К-L7 (+К-L7.1) | LOCKED (К-L7.1 added К10.3 v2) | Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1 opt-in coexistence) | К-L7 atomic-from-observer preserved within pipeline slot boundary; V1 К-L7 sync default remains operational |
```

К-L16 row inserted:
```
| К-L16 | LOCKED (К10.3 v2) | Simulation tick pipeline depth | D ≥ 1 (configurable 1-3, default 2); К-L16 establishes display latency invariant (D × tick_period); pipeline drain orderly at save/pause; governs pipeline-managed dispatches |
```

**К-L7.1 invariant text (verbatim per §1.3)**.

**К-L16 invariant text (verbatim per §1.4)**.

**VULKAN_SUBSTRATE.md substantial amendments** (consolidated K-L19 deferred + K-L7.1/L16):

**§0 L1 amended** (K-L19 К10.3 v2 cleanup):
```markdown
| L1 | GPU API | Vulkan 1.3 + async compute queue family mandate (К-L19 V0.B landing) | Future-proof, total control, modern GPU pipeline (rendering + compute + async dispatch per К-L16 К10.3 v2) |
```

**§0 L7 amended** (К-L19 baseline note):
```markdown
| L7 | Initial platform | Windows-only (matches К-L19 hardware tier baseline — Vulkan 1.3 + async compute) | Cross-platform deferred |
```

**§2 architecture amendment** (add pipeline depth section after existing architecture):
```markdown
### Pipeline depth architecture (К-L16, К10.3 v2 amendment)

The V substrate supports simulation tick pipeline depth D=2 (default, configurable 1-3) per К-L16 для pipeline-managed dispatches. Simulation thread runs D ticks ahead of display thread for these dispatches; cross-layer async operations (GPU compute pipeline-managed, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread.

Pipeline slot data model (verbatim from KERNEL_FULL_NATIVE_SCHEDULER.md §3.10 Item 33):
[PipelineSlot struct]

Sim-thread reads see slot tail state for pipeline-managed fields (К-L7.1 sub-invariant): sim tick T reads dispatched-at-(T-1) state. One-tick lag bounded и deterministic.

Display thread reads from CurrentSimTick - D для pipeline-managed display state — display latency invariant established по К-L16.

**К-L7 sync coexistence**: V1's existing `V1DiffusionPipeline.ExecuteIteration` synchronous dispatch path (К-L7 atomic-from-observer per `compute_dispatch.h`) remains operational orthogonal к К-L16. К-L7.1 is opt-in для new pipeline-managed consumers; К-L7 is default для existing V1 consumers. К-L9 «Vanilla = mods» preserved — author choice per field.
```

**§2.3 threading model amendment** (substantial — pipeline depth + async compute queue + sim-thread read pattern):
```markdown
### Pipeline depth and queue family roles (К-L16/L19, К10.3 v2 amendment)

Sim thread coordinates с three Vulkan queues:
- **Graphics queue** — display rendering (existing — preserved verbatim)
- **Async compute queue** (К-L19 V0.B amendment) — К-L16 pipeline depth dispatches per Phase.Compute (К10.3 v2 Item 35); V1 sync dispatch also uses this queue
- **Copy/transfer queue** (К-L19 V0.B amendment) — asset transfers (existing semantics)

Pipeline depth D=2 default (К-L16): sim thread allocates new slot at start of pipeline-managed tick; Phase.Compute dispatches к async compute queue; fence orchestration tracks slot transitions Dispatched → FenceCompleted → ReadableAsTail.

Sim-thread reads from slot tail (К-L7.1) для pipeline-managed fields — `df_pipeline_read_slot_tail(slot_offset = -1)` returns sim_tick - 1 results. К-L7 atomic-from-observer preserved within slot boundary; cross-slot reads see different snapshots.

V1's `dispatch_compute_field` sync path (К-L7 baseline): preserved unchanged; consumer call returns after fence signals. Orthogonal к pipeline depth.
```

**§3.4 native compute dispatch amendment** (К-L19 К10.3 v2 cleanup):
```markdown
### К-L19 async compute queue mandate (К10.3 v2 documentation cleanup от V0.B landing)

Native compute dispatch к dedicated async compute queue family mandated per К-L19 (V0.B implementation backing): `df_world_attach_vulkan` populates `VulkanAttachment.async_compute_queue` from `VulkanDevice.AsyncComputeQueueFamilyIndex`. V1's `dispatch_compute_field` and К10.3 v2's Phase.Compute (Item 35) both submit к this queue. Graphics queue не used for compute. К-L19 hardware tier exclusion accepted as architectural choice per `KERNEL_ARCHITECTURE.md` К-L19 row.
```

**§7.2 determinism amendment** (extends с pipeline drain semantics):
```markdown
### Pipeline drain semantics (К-L16, К10.3 v2 amendment)

Save protocol per S8-Q1.5: snapshot display tick state (CurrentSimTick - D). Display already sees coherent world; pipeline drain не required at save time. Faster save (no waiting для in-flight compute completion).

Pause protocol: natural convergence — sim thread completes current tick, no new dispatch. Pipeline depth naturally absorbs already-dispatched work. К-L18 quiescent state precondition (Item 41) verifies pipeline quiesced before mod operations.

V1 sync dispatch path: not affected by pipeline drain semantics (no slot machinery involvement).
```

**§7.3 async sync hazards amendment** (К-L7.1 + coexistence):
```markdown
### Pipeline slot tail read pattern (К-L7.1, К10.3 v2 amendment)

Previously async sync hazards section described per-read fence polling. К10.3 v2 К-L7.1 introduces opt-in slot tail read pattern: sim-thread reads see slot tail state (sim_tick - 1) without per-read fence query для pipeline-managed fields. Predicted savings ~30-50% reduction в FieldHandle.ReadCell latency for pipeline-managed paths (Prediction 13).

К-L7 atomic-from-observer invariant preserved within pipeline slot boundary; cross-slot reads see different snapshots (К-L7.1).

**Coexistence (S-LOCK-13)**: V1's K-L7 sync semantics remain default; pipeline-managed K-L7.1 is opt-in per field. Mod authors choose per-field based on consumer pattern requirements.
```

**Per Lesson #8 (atomic compilable unit)**: К-L7.1 + К-L16 invariant text + KERNEL_ARCHITECTURE amendment + VULKAN_SUBSTRATE.md substantial amendments (consolidating K-L19 deferred + K-L7.1/L16) + REGISTER updates land в **same commit**. Per Lesson #11 (architectural reduction): К-L7.1 sub-invariant к К-L7 preserves К-L7 integrity, mirrors К-L3.1 pattern.

**Validation**:
- `dotnet build` clean
- `cmake --build` clean
- `dotnet test` baseline+ green
- К-L cross-reference integrity verified
- VULKAN_SUBSTRATE.md amendments preserve existing wording style
- `sync_register.ps1 --validate` exit 0

**Commit message**:
```
feat(architecture): K10.3 v2 — К-L7.1 + К-L16 amendments + VULKAN_SUBSTRATE.md v1.0→v1.1 reconciliation

The first load-bearing commit of К10.3 v2 (К-L7.1 + К-L16 grouped landing per
Approach C). К-L7.1 sub-invariant к К-L7 (mirrors К-L3.1 precedent, opt-in
coexistence per S-LOCK-10); К-L16 simulation pipeline depth (governs pipeline-
managed dispatches per S-LOCK-13).

KERNEL_ARCHITECTURE.md v2.2 → v2.3 partial (К-L invariant table extended).

VULKAN_SUBSTRATE.md v1.0 → v1.1 substantial amendments (S-LOCK-14
reconciliation: consolidates K-L19 deferred V0.B amendments + К10.3 v2
К-L7.1/L16 amendments):
- §0 L1: Vulkan 1.3 + async compute mandate (K-L19 V0.B cleanup)
- §0 L7: К-L19 baseline note (K-L19 V0.B cleanup)
- §2 architecture: pipeline depth section + К-L7.1 coexistence (К-L16)
- §2.3 threading: pipeline depth + queue family roles (К-L16/L19/L7.1)
- §3.4 native compute: К-L19 mandate documentation cleanup
- §7.2 determinism: pipeline drain semantics (К-L16)
- §7.3 async sync hazards: pipeline slot tail read pattern (К-L7.1)

К-L7.1 text: «GPU compute writes к RawTileField storage are bound к pipeline
slot (К-L16). Sim-thread reads of compute-managed fields see slot-tail state...»

К-L16 text: «Simulation tick pipeline depth D ≥ 1 (configurable 1-3, default
2). Simulation thread runs D ticks ahead of display thread...»

V1 К-L7 sync coexistence preserved; К-L7.1 opt-in per field.

Build clean; baseline+ tests green.

Phase 8 of K10.3 v2 cascade. Commit 8 of ~14.
```

### Commit 9 — Item 38: Display composition framework

**Files** (per S-LOCK-11 Application/Display location):
- `src/DualFrontier.Application/Display/CompositionFramework.cs` (NEW — layer registry + latency contracts + composition order)
- `src/DualFrontier.Application/Display/Layer.cs` (NEW — layer abstraction)
- `src/DualFrontier.Application/Display/LayerType.cs` (NEW — enum SimState / Intent / CombatFeedback / Static)
- `src/DualFrontier.Application/Display/SimStateLayer.cs` (NEW — wraps existing SpriteRenderer/TileMap/Camera2D rendering path as default SimState layer)
- `tests/DualFrontier.Application.Tests/Display/CompositionFrameworkTests.cs` (NEW; create test project if absent per Phase 0 finding)

**Drift surface**: Display composition framework operational в Application layer above Runtime renderer. Existing rendering path continues к work, wrapped as «SimState» layer; intent/combat feedback layers opt-in для new use cases. Layer registration + composition order infrastructure landed. Existing `Application/Rendering/IRenderer.cs` interfaces preserved unchanged (S-LOCK-11).

**Implementation surface (per spec §3.11 Item 38 + К-L17 + S-LOCK-11)**:

```csharp
// LayerType.cs
public enum LayerType
{
    SimState = 0,       // Pipeline slot tail (К-L16 governed) for pipeline-managed; current state for К-L7 sync default
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
    
    public abstract void Render(/* render context — to be designed against Runtime SpriteRenderer/Camera surfaces */);
}

// CompositionFramework.cs
public sealed class CompositionFramework
{
    private readonly Dictionary<string, Layer> _layers = new();
    
    public void RegisterLayer(Layer layer)
    {
        _layers[layer.Fqn] = layer;
    }
    
    public void RenderFrame(/* render context */)
    {
        // К-L17 composition order: sim state first, intent/combat overlays composited on top, static last
        var orderedLayers = _layers.Values
            .OrderBy(l => (int)l.Type)
            .ThenBy(l => l.CompositionOrder);
        
        foreach (var layer in orderedLayers)
        {
            layer.Render(/* render context */);
        }
    }
}

// SimStateLayer.cs
public class SimStateLayer : Layer
{
    private readonly SpriteRenderer _spriteRenderer;
    private readonly Camera2D _camera;
    // ... wraps existing V0.C.2 rendering pipeline
    public override LayerType Type => LayerType.SimState;
    public override void Render(/* render context */) { /* existing batched sprite path */ }
}
```

**К-L17 latency invariants preserved**:
- SimState layer latency = D × tick_period for pipeline-managed; sub-tick for К-L7 sync default (V1 path)
- Intent layer latency ≤ 16ms (К-L17 mandate)
- CombatFeedback latency ≤ 1ms К-L15 + ≤ 16ms display ≈ ≤ 17ms event-to-visible (Prediction 15)

**Existing rendering path preserved**: V0.C.2 batched sprite rendering wrapped as default SimState layer. К10.3 v2 doesn't replace existing rendering; adds layer registry on top.

**Validation**:
- `dotnet build` clean
- `dotnet test` baseline+ green
- `CompositionFrameworkTests`: layer registration; composition order correctness; rendering invokes layers в correct order; SimStateLayer wraps SpriteRenderer correctly
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(application): K10.3 v2 Item 38 — display composition framework (К-L17 layer registry + composition order; SimStateLayer wraps V0.C.2 path)`

### Commit 10 — Items 39+40: Intent overlay + combat feedback layer infrastructure

**Files**:
- `src/DualFrontier.Application/Display/IntentOverlayLayer.cs` (NEW — current input state surface + sub-pipeline-latency rendering)
- `src/DualFrontier.Application/Display/CombatFeedbackLayer.cs` (NEW — Fast tier event consumers rendering)
- `src/DualFrontier.Contracts/Display/LayerAttribute.cs` (NEW — `[Layer(LayerType.Intent)]` или `[Layer(LayerType.CombatFeedback)]` для mod-registered layer providers)
- `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs` (modified — К-L17 layer capabilities added per S8-Q3 + S3-Q5 pattern: `kernel.layer.intent:{FQN}` + `kernel.layer.combat_feedback:{FQN}`)
- `tests/DualFrontier.Application.Tests/Display/IntentOverlayTests.cs` (NEW)
- `tests/DualFrontier.Application.Tests/Display/CombatFeedbackTests.cs` (NEW)

**Drift surface**: Intent overlay + combat feedback layer infrastructure landed. Mods register layers via capability declaration + `[Layer]` attribute pattern. К-L17 composition order enforced via CompositionFramework (Commit 9).

**Implementation surface (per spec §3.11 Items 39+40)**:

```csharp
// IntentOverlayLayer.cs
public class IntentOverlayLayer : Layer
{
    private readonly InputEventQueue _inputQueue;  // V0.C.1 surface
    public override LayerType Type => LayerType.Intent;
    
    public override void Render(/* render context */)
    {
        // К-L17 contract: ≤16ms latency (1 display frame at 60 FPS)
        // Read directly from current input state, не from pipeline slot
        var inputState = _inputQueue.Peek();
        
        // Render cursor / hover / drag-and-drop / construction-placement preview
        // Sub-pipeline-latency: input state read at display tick time
    }
}

// CombatFeedbackLayer.cs
public class CombatFeedbackLayer : Layer
{
    public override LayerType Type => LayerType.CombatFeedback;
    
    private readonly Queue<CombatFeedbackEvent> _pendingFeedback = new();
    
    public CombatFeedbackLayer(IBusFacade bus)  // BusFacade per К10.2
    {
        // К-L15 fast tier subscription: combat hit events
        bus.SubscribeFast<CombatHitEvent>(OnCombatHit);
    }
    
    private void OnCombatHit(CombatHitEvent evt)
    {
        // Fast tier subscriber: bounded exec, no blocking, no GC alloc per FastTierContractMonitor (К10.2)
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

**Capability tokens per S8-Q3 + S3-Q5 pattern** (extends MOD_OS §3.2):
- `kernel.layer.intent:{FQN}` — mod registers intent overlay layer
- `kernel.layer.combat_feedback:{FQN}` — mod registers combat feedback layer
- Granular per FQN per layer type per action (consistent с К10.2 capability tier pattern)

**KernelCapabilityRegistry extension**:

```csharp
// Application/Modding/KernelCapabilityRegistry.cs extension
public IReadOnlySet<string> GetKernelCapabilities()
{
    // ... existing tier-prefixed event tokens (К10.2) preserved
    
    // К10.3 v2 layer capabilities (NEW)
    var layerTypes = ScanForTypes<Layer>();
    foreach (var layerType in layerTypes)
    {
        var fqn = layerType.FullName!;
        var typeAttr = layerType.GetCustomAttribute<LayerAttribute>();
        if (typeAttr is null) continue;
        
        switch (typeAttr.Type)
        {
            case LayerType.Intent:
                capabilities.Add($"kernel.layer.intent:{fqn}");
                break;
            case LayerType.CombatFeedback:
                capabilities.Add($"kernel.layer.combat_feedback:{fqn}");
                break;
            // SimState + Static use existing renderer-level capabilities (V substrate primitives)
        }
    }
    return capabilities;
}
```

**Per К-L9 «Vanilla = mods»**: vanilla layers (built-in intent overlay для core gameplay, built-in combat feedback) register through same `[Layer]` attribute + capability declaration pattern as third-party mods.

**Validation**:
- `dotnet build` clean
- `dotnet test` baseline+ green
- `IntentOverlayTests`: input state read at display tick; sub-pipeline-latency rendering measurable
- `CombatFeedbackTests`: Fast tier subscriber pattern; event-to-visible latency tracking
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(application): K10.3 v2 Items 39+40 — intent overlay + combat feedback layer infrastructure (К-L17)`

### Commit 11 — К-L17 invariant landing + VULKAN_SUBSTRATE.md + MOD_OS_ARCHITECTURE.md amendments (LOAD-BEARING 2 of 3)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.3 partial (К-L17 row added)
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 partial (§4 rendering use case extended с display composition section; §5.5 Mode C navigation extended с visibility latency through К-L17 composition)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` partial (§3.2 capability syntax extended с layer tokens per S3-Q5 + S8-Q3 pattern)
- `docs/governance/REGISTER.yaml` (version bumps continued)

**This is the second load-bearing commit of К10.3 v2** (К-L17 amendment landing).

**К-L17 invariant text (verbatim per §1.5)**.

К-L17 row inserted:
```
| К-L17 | LOCKED (К10.3 v2) | Display composition multi-layer | Sim state + intent overlay + combat feedback layers с independent latency contracts; composition order: sim state first, overlays composited on top; lives в Application/Display/ per S-LOCK-11 |
```

**VULKAN_SUBSTRATE.md §4 rendering amendment** (display composition section after existing rendering pipeline):

```markdown
### Display composition (К-L17, К10.3 v2 amendment)

V substrate rendering use case is consumed by display composition framework per К-L17 (lives в `src/DualFrontier.Application/Display/`, not в V substrate). Three-layer composition:

1. **SimStateLayer** — V substrate render path (existing V0.C.2 batched sprite + Camera2D — preserved verbatim, wrapped as default layer). Reads from pipeline slot tail (К-L16) for pipeline-managed display state; reads current state for К-L7 sync default. Latency D × tick_period or sub-tick.
2. **IntentOverlayLayer** (К10.3 v2 amendment) — current input state surface. Reads from InputEventQueue at display tick time. Latency ≤16ms (60 FPS).
3. **CombatFeedbackLayer** (К10.3 v2 amendment) — Fast tier event consumers. Subscribes к К-L15 fast tier events; renders damage numbers, hit sparks, weapon glints. Latency ≤1ms К-L15 + ≤16ms display ≈ ≤17ms event-to-visible.

Composition order (К-L17 mandate): sim state layers rendered first, intent and combat overlays composited on top.

Mod-registered layers use `[Layer(LayerType.Intent | CombatFeedback)]` attribute + capability declaration (`kernel.layer.intent:{FQN}` или `kernel.layer.combat_feedback:{FQN}` per S3-Q5 + S8-Q3 pattern). Layer registration capabilities live в MOD_OS_ARCHITECTURE.md §3.2 (К10.3 v2 amendment).

V substrate exposes rendering primitives (SpriteRenderer, Camera2D, TileMap); layer composition lives one architectural layer above per S-LOCK-11.
```

**VULKAN_SUBSTRATE.md §5.5 Mode C navigation amendment**:

```markdown
### Mode C visibility latency (К-L17, К10.3 v2 amendment)

Mode C navigation (existing — preserved verbatim) visibility latency now governed по К-L17 composition framework. Player commands ⟶ IntentOverlayLayer (≤16ms render); pawn responses ⟶ SimStateLayer (pipeline-managed К-L16 D=2 lag, или sync К-L7); combat feedback ⟶ CombatFeedbackLayer (К-L15 + display ≈ ≤17ms event-to-visible).

No special-case visibility mechanism — К-L17 composition framework handles latency separation uniformly.
```

**MOD_OS_ARCHITECTURE.md §3.2 capability syntax extension** (К-L17 layer tokens added к existing tier-prefixed token format documentation):

```markdown
### К-L17 layer capabilities (К10.3 v2 amendment)

In addition к bus tier-prefixed tokens (К-L15 К10.2 amendment), К10.3 v2 extends capability syntax с layer registration tokens:

- `kernel.layer.intent:<FQN>` — mod registers intent overlay layer (sub-pipeline-latency)
- `kernel.layer.combat_feedback:<FQN>` — mod registers combat feedback layer (Fast tier consumer)

Granular per FQN per layer type — consistent с К10.2 tier-prefixed token pattern.

Layer mismatch detection: if mod's `[Layer]` attribute declares one type but capability declares another, validation emits `LayerCapabilityMismatch` (§11.2 К10.3 v2 amendment).
```

**Per Lesson #8**: К-L17 invariant text + KERNEL_ARCHITECTURE amendment + VULKAN_SUBSTRATE.md §4/§5.5 amendments + MOD_OS §3.2 amendment + REGISTER updates land в **same commit**.

**Validation**:
- `dotnet build` clean
- `dotnet test` baseline+ green
- К-L cross-reference integrity verified
- VULKAN_SUBSTRATE.md + MOD_OS_ARCHITECTURE.md amendments preserve existing wording style
- `sync_register.ps1 --validate` exit 0

**Commit message**:
```
feat(architecture): K10.3 v2 — К-L17 display composition amendment + cross-document propagation

The second load-bearing commit of К10.3 v2 (К-L17 amendment landing).
К-L17 «Display composition multi-layer» added к KERNEL_ARCHITECTURE.md v2.3
(lives в Application/Display/ per S-LOCK-11).

VULKAN_SUBSTRATE.md v1.1 partial amendments:
- §4 rendering: display composition section (К-L17 three-layer model)
- §5.5 Mode C: visibility latency через К-L17 composition (no special-case)

MOD_OS_ARCHITECTURE.md partial amendments:
- §3.2 capability syntax: layer tokens (kernel.layer.intent + kernel.layer.combat_feedback)

К-L17 text: «Display tick T composites multi-layer state where layers carry
independent latency contracts...»

Build clean; baseline+ tests green.

Phase 11 of K10.3 v2 cascade. Commit 11 of ~14.
```

### Commit 12 — Item 41: К-L18 quiescent state enforcement

**Files**:
- `native/DualFrontier.Core.Native/src/mod_unload.cpp` (modified — К-L18 quiescent state precondition extended с pipeline slot verification)
- `native/DualFrontier.Core.Native/include/mod_unload.h` (extended)
- `native/DualFrontier.Core.Native/test/quiescent_state_test.cpp` (NEW selftest section)
- `src/DualFrontier.Core.Interop/ModUnloadInterop.cs` (modified — quiescent state error code handling extended)

**Drift surface**: К10.2 Item 32 `df_scheduler_unload_mod_native_state` already includes quiescent state precondition stub (`df_scheduler_set_sim_paused` / `df_scheduler_is_sim_paused` per mod_unload.h lines 71-76). К10.3 v2 Item 41 extends this к include pipeline slot quiescence verification (all D slots в `ReadableAsTail` state or empty per Item 33 state machine).

**Implementation surface (per spec §3.12 Item 41)**:

```cpp
// mod_unload.cpp К-L18 quiescent state precondition extension
ModUnloadResult df_scheduler_unload_mod_native_state(uint32_t mod_id, ModUnloadResult* out_result)
{
    if (!out_result) return DF_ERR_NULL;
    memset(out_result, 0, sizeof(*out_result));
    
    // К-L18 precondition: sim must be paused (К10.2 stub, now enforced)
    if (!df_scheduler_is_sim_paused())
    {
        out_result->success = 0;
        out_result->error_count = 1;
        strncpy(out_result->error_messages[0],
                "К-L18 violation: simulation must be paused before mod unload",
                255);
        return 0;
    }
    
    // К-L18 precondition extended (К10.3 v2 Item 41): pipeline slots quiescent
    int is_quiescent = 0;
    df_pipeline_is_quiescent(&is_quiescent);  // Item 33 API
    if (!is_quiescent)
    {
        out_result->success = 0;
        out_result->error_count = 1;
        strncpy(out_result->error_messages[0],
                "К-L18 violation: pipeline slots not quiescent (in-flight compute dispatches)",
                255);
        return 0;
    }
    
    // Existing К10.2 T0-T7 sequence proceeds...
    return 1;
}
```

**Per К-L18 verbatim (spec Part 2.4)**: «pipeline slots quiescent (all fences completed); no concurrent compute dispatches in-flight».

**Mod load primitive mirror**: К-L18 quiescent state precondition symmetric для mod load operations. Item 42 (next commit) lands UI helper integration enforcing precondition (per S-LOCK-12: helpers only, not full settings menu).

**Validation**:
- `cmake --build` clean
- `quiescent_state_test`:
  - Mod unload succeeds when sim paused + pipeline empty
  - Mod unload fails с clear error when sim не paused
  - Mod unload fails с clear error when pipeline has in-flight slots
- `dotnet test` baseline+ green
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(kernel-native): K10.3 v2 Item 41 — К-L18 quiescent state enforcement (pipeline slot quiescence verification + sim pause enforcement)`

### Commit 13 — Item 42: Mod management UI helper integration

**Files** (per S-LOCK-12 helpers-only scope):
- `src/DualFrontier.Application/Loop/SimulationStateController.cs` (NEW — pause/resume operations wired к К-L18 precondition + quiescence wait helper)
- `src/DualFrontier.Application/Modding/ModMenuController.cs` (modified — existing controller extended с pause-then-unload-then-resume workflow per К-L18)
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` (modified — Step 3.6 V resource cleanup added к unload chain per S8-Q4 V substrate scope)
- `src/DualFrontier.Application/Bridge/VResourceCleanup.cs` (NEW or analog — managed wrapper around `df_vulkan_unload_mod_resources` C ABI primitive when V scope implementation lands)
- `tests/DualFrontier.Application.Tests/UI/ModManagementWorkflowTests.cs` (NEW)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` partial (§9.5 amended с Step 3.6 description; §9.5.1 failure semantics extended; §11 hot reload section noted с К-L18 compliance — landing finalized в Commit 14)

**Drift surface**: Helper layer enforces К-L18 pause precondition. ModMenuController workflow: user triggers «Disable Mod» → SimulationStateController pauses sim → wait for pipeline quiescence → invoke unload primitive → on success resume. Hot reload tooling similarly pauses sim. V resource cleanup primitive (Step 3.6) added к MOD_OS §9.5 chain (landing v1.10).

**Implementation surface (per spec §3.12 Item 42 + S-LOCK-12 helpers-only)**:

```csharp
// SimulationStateController.cs
public sealed class SimulationStateController
{
    private readonly INativeSchedulerInterop _scheduler;
    private readonly IPipelineSlotInterop _pipeline;
    
    public async Task PauseAsync()
    {
        _scheduler.SetSimPaused(true);
        // К-L16 pipeline depth naturally drains under pause
    }
    
    public async Task WaitForQuiescenceAsync(TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (_pipeline.IsQuiescent()) return;
            await Task.Delay(10);
        }
        throw new PipelineQuiescenceTimeoutException(
            $"Pipeline did не reach quiescent state within {timeout.TotalSeconds}s");
    }
    
    public async Task ResumeAsync() => _scheduler.SetSimPaused(false);
}

// ModMenuController.cs extension (existing controller extended)
public async Task<ModOperationResult> DisableModAsync(string modId)
{
    await _simController.PauseAsync();
    await _simController.WaitForQuiescenceAsync(TimeSpan.FromSeconds(5));
    
    try
    {
        var result = await _modPipeline.UnloadModAsync(modId);
        return result;
    }
    finally
    {
        await _simController.ResumeAsync();
    }
}
```

**MOD_OS §9.5 amendment** (Step 3.6 V resource cleanup added к existing 8-step chain — becomes 9-step):

```markdown
### 9.5 ALC unload protocol (К10.2 + К10.3 v2 amended)

The unload chain (9 steps total post-К10.3 v2):

1. RestrictedModApi.UnsubscribeAll()
2. IModContractStore.RevokeAll(modId)
3. ModRegistry.RemoveSystems(modId)
3.5. **(К10.2)** df_scheduler_unload_mod_native_state(modId) — native scheduler primitive
3.6. **(К10.3 v2)** V resource cleanup — wraps `df_vulkan_unload_mod_resources(mod_id)` C ABI per S8-Q4 spec scope. К10.3 v2 lands managed wrapper + С ABI placeholder (returning success when no pipeline-managed resources registered); native implementation lands as V-cycle work or К-extensions. К-L18 quiescent state precondition naturally satisfied (Step 3.5 already verified). Best-effort sequential per §9.5.1.
4. The dependency graph is rebuilt without this mod's systems.
5. The scheduler swaps to the new phase list.
6. ALC.Unload() is called.
7. The loader spins on WeakReference.IsAlive (existing protocol).
```

**§11 hot reload section amendment** (К-L18 compliance):
```markdown
### Hot reload К-L18 compliance (К10.3 v2 amendment)

Hot reload preserves game state through managed dependency graph swap mechanism (existing). К-L18 pause precondition required для consistent mod transition. Hot reload tooling uses SimulationStateController.PauseAsync + WaitForQuiescenceAsync before invoking ALC swap, ResumeAsync after. Mod management UI и hot reload tooling share К-L18 enforcement pattern.
```

**Validation**:
- `dotnet build` clean
- `dotnet test` baseline+ green
- `ModManagementWorkflowTests`: pause→quiescence→unload→resume orderly flow; failure handling при quiescence timeout
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(application): K10.3 v2 Item 42 — mod management UI helper integration с К-L18 (SimulationStateController + ModMenuController pause hook; Step 3.6 V resource cleanup placeholder)`

### Commit 14 — К-L18 invariant landing + MOD_OS_ARCHITECTURE.md amendments (LOAD-BEARING 3 of 3)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.3 final (К-L18 row added — completes К10.3 v2 К-L additions)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` final (Step 3.6 in §9.5 + К-L18 compliance §11 + ValidationErrorKind additions §11.2)
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 final (§3.4 native compute dispatch extended с `df_vulkan_unload_mod_resources` C ABI primitive placeholder — final VULKAN_SUBSTRATE amendment)
- `docs/governance/REGISTER.yaml` (final version bumps: DOC-A-KERNEL v2.2 → v2.3; DOC-A-VULKAN_SUBSTRATE v1.0 → v1.1; DOC-A-MOD_OS version bump per current content; governance_events append references к К10.3 v2 closure)

**This is the third load-bearing commit of К10.3 v2** (К-L18 amendment landing + final cross-document propagation).

**К-L18 invariant text (verbatim per §1.6)**.

К-L18 row inserted:
```
| К-L18 | LOCKED (К10.3 v2) | Mod lifecycle quiescent state | Sim paused + pipeline slots quiescent precondition для mod load/unload; UI helper layer (SimulationStateController + ModMenuController) enforces precondition; full settings menu deferred (S-LOCK-12) |
```

**MOD_OS §11.2 ValidationErrorKind additions** (К10.3 v2 new entries):
- `QuiescentStatePreconditionViolated` (К-L18) — mod operation attempted while sim active or pipeline in-flight
- `PipelineQuiescenceTimeout` (К-L18) — quiescence wait timeout exceeded
- `LayerCapabilityMismatch` (К-L17) — layer attribute declares one type but capability declares another
- `VulkanModResourceCleanupFailed` (К-L18 V scope) — V resource cleanup primitive failed (placeholder until V implementation lands)

**VULKAN_SUBSTRATE.md §3.4 final amendment**:

```markdown
### `df_vulkan_unload_mod_resources` C ABI primitive (К-L18, К10.3 v2 placeholder per S8-Q4 spec scope)

К10.3 v2 lands C ABI signature + managed wrapper placeholder; native implementation lands as V-cycle work or К-extensions. К-L18 quiescent state precondition (Step 3.5 К10.2 native primitive completed) satisfied before invocation.

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

K10.3 v2 placeholder returns `success=1` + zero counts (no pipeline-managed mod resources yet registered). Full implementation: VkDestroyPipeline / VkFreeDescriptorSets / vkDestroyBuffer / vkDestroyImage operations для mod-registered resources. Best-effort sequential per MOD_OS §9.5.1.
```

**Per Lesson #8 (atomic compilable unit)**: К-L18 invariant text + KERNEL_ARCHITECTURE amendment + MOD_OS § amendments + VULKAN_SUBSTRATE.md §3.4 final amendment + REGISTER updates land в **same commit**.

**Validation**:
- `dotnet build` clean
- `cmake --build` clean
- `dotnet test` baseline+ green
- К-L cross-reference integrity verified (final state — 4 К-L invariants added в К10.3 v2; cumulative К-Lxx series 20: К-L1..L11 + К-L6 SUPERSEDED + К-L3.1/L7.1 subs + К-L12..L19)
- VULKAN_SUBSTRATE.md amendments preserve existing wording style
- `sync_register.ps1 --validate` exit 0

**Commit message**:
```
feat(architecture): K10.3 v2 — К-L18 quiescent state amendment + final cross-document propagation

The third load-bearing commit of К10.3 v2 (К-L18 amendment landing).
К-L18 «Mod lifecycle quiescent state» added к KERNEL_ARCHITECTURE.md v2.3
(К10.3 v2 complete К-L additions; cumulative 20 invariants).

MOD_OS_ARCHITECTURE.md:
- §9.5: Step 3.6 V resource cleanup placeholder (9-step chain post-К10.3 v2)
- §11 hot reload: К-L18 compliance note
- §11.2: new ValidationErrorKinds (QuiescentStatePreconditionViolated,
  PipelineQuiescenceTimeout, LayerCapabilityMismatch, VulkanModResourceCleanupFailed)

VULKAN_SUBSTRATE.md v1.1 final:
- §3.4: df_vulkan_unload_mod_resources C ABI placeholder (К-L18 К10.3 v2;
  full implementation V-cycle/К-extensions)

К-L18 text: «Mod load/unload operations require simulation paused state...
pipeline slots quiescent (all fences completed)...»

К-Lxx series total: 20 invariants post-К10.3 v2 (К-L1..L11 + К-L6 SUPERSEDED +
К-L3.1/L7.1 subs + К-L12..L19).

Build clean; baseline+ tests green.

Phase 14 of K10.3 v2 cascade. Commit 14 of ~14 — К10.3 v2 К-L invariant landings complete.
```

### Commit 15 (optional, depending on need) — К10.3 v2 closure: REGISTER amendments + audit_trail EVT + brief lifecycle EXECUTED

**Files**:
- `docs/governance/REGISTER.yaml` (DOC-D-K10_3 lifecycle AUTHORED → EXECUTED; audit_trail EVT entry; CAPA entries если any opened)
- `tools/briefs/K10_3_EXECUTION_BRIEF_v2.md` (frontmatter status DRAFT-PENDING-RATIFICATION → EXECUTED; new §9 closure section added)
- `docs/MIGRATION_PROGRESS.md` (К10.3 v2 closure entry per METHODOLOGY §12.7 step 3)
- `docs/governance/REGISTER_RENDER.md` (regenerated via render_register.ps1)
- `docs/governance/VALIDATION_REPORT.md` (regenerated via sync_register.ps1)

**REGISTER amendments** (per METHODOLOGY §12.7 canonical):

1. **DOC-D-K10_3**: lifecycle AUTHORED → EXECUTED
2. **DOC-A-KERNEL**: version 2.2 → 2.3 (4 К-L invariants added — К-L7.1 sub + К-L16/L17/L18; К-L19 already enrolled V0.B)
3. **DOC-A-VULKAN_SUBSTRATE**: version 1.0 → 1.1 (8 sub-section amendments — consolidated K-L19 deferred + К10.3 v2)
4. **DOC-A-MOD_OS**: version bump per current content (К-L17 layer caps + К-L18 compliance + Step 3.6 + new ValidationErrorKinds)
5. **DOC-A-KERNEL_FULL_NATIVE_SCHEDULER**: governance_events append с EVT-K10_3-CLOSURE
6. **audit_trail entry**: `EVT-{date}-K10_3-CLOSURE`
7. **Requirements added**: REQ-K-L7_1, REQ-K-L16, REQ-K-L17, REQ-K-L18 (4 new REQs; REQ-K-L19 already enrolled V0.B)
8. **Risks status update**: any К10.3 v2-related risks moved к accepted/closed status

**Validation**:
- `sync_register.ps1 --validate` exit 0 — **mandatory gate** per METHODOLOGY §12.7
- `dotnet build` clean
- `dotnet test` baseline+ green (К10.3 v2 final count documented в closure)
- `cmake --build` clean

**Commit message**:
```
governance: K10.3 v2 closure — REGISTER amendments + 4 REQs + EVT-K10_3-CLOSURE

К10.3 v2 sub-milestone closure per METHODOLOGY §12.7 canonical protocol.

REGISTER updates:
- DOC-D-K10_3 lifecycle AUTHORED → EXECUTED
- DOC-A-KERNEL version 2.2 → 2.3 (4 К-L invariants added; К-L19 inherited V0.B)
- DOC-A-VULKAN_SUBSTRATE version 1.0 → 1.1 (consolidated V0.B deferred + К10.3 v2 amendments)
- DOC-A-MOD_OS version bump (К-L17 + К-L18 + Step 3.6)

Requirements added: REQ-K-L7_1, REQ-K-L16, REQ-K-L17, REQ-K-L18

audit_trail entry: EVT-{date}-K10_3-CLOSURE

К10.3 v2 closure leaves К10.4 (TLA+ formal verification, 3 items: 18, 45, 46)
as final К10 sub-milestone brief per Option III standalone-briefs structure.

К-L7.1 + К-L16/L17/L18 architecturally established; К-L7 sync coexistence
preserved (S-LOCK-10/13); К-L17 lives в Application/Display/ (S-LOCK-11);
К-L18 UI = helpers only (S-LOCK-12). Managed-facade-preserved strategy continues.

К-Lxx series total: 20 invariants post-К10.3 v2.

Phase 15 of K10.3 v2 cascade. Commit 15 of ~15 — К10.3 v2 closure.
```

---

## §4 — К-L invariant amendments (К10.3 v2 detailed scope)

К10.3 v2 lands **4 К-L invariants** в KERNEL_ARCHITECTURE.md v2.2 → v2.3. Per S-LOCK-2 Approach C grouped landing: 3 load-bearing commits distribute К-L additions across cascade.

### §4.1 — Verbatim К-L invariant table amendments

`docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 К-L invariants table receives 4 row insertions/extensions:

**К-L7 row extended** (К-L7.1 sub-invariant added; coexistence noted):
```
| К-L7 (+К-L7.1) | LOCKED (К-L7.1 added К10.3 v2) | Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1 opt-in coexistence) | К-L7 atomic-from-observer preserved within pipeline slot boundary; V1 К-L7 sync default remains operational |
```

**К-L16 row inserted** (после К-L15 К10.2):
```
| К-L16 | LOCKED (К10.3 v2) | Simulation tick pipeline depth | D ≥ 1 (configurable 1-3, default 2); display latency invariant (D × tick_period); pipeline drain orderly at save/pause; governs pipeline-managed dispatches |
```

**К-L17 row inserted**:
```
| К-L17 | LOCKED (К10.3 v2) | Display composition multi-layer | Sim state + intent overlay + combat feedback layers с independent latency contracts; composition order: sim state first, overlays on top; lives в Application/Display/ |
```

**К-L18 row inserted**:
```
| К-L18 | LOCKED (К10.3 v2) | Mod lifecycle quiescent state | Sim paused + pipeline slots quiescent precondition для mod load/unload; UI helper layer (SimulationStateController + ModMenuController) enforces; full settings menu deferred |
```

(К-L19 already exists at line 65, inherited V0.B.)

### §4.2 — KERNEL_ARCHITECTURE.md Part 2 master plan amendment

Part 2 master plan table К10 row updated:

```
| К10 | Native kernel scheduler + bus + pipeline + display + hardware | AUTHORED-IN-PROGRESS (К10.1 ✅ + К10.2 ✅ + V0.B К-L19 ✅ + К10.3 v2 ✅; К10.4 pending) | К-L6 SUPERSEDED + К-L12/L13/L14 (К10.1) + К-L15 (К10.2) + К-L19 (V0.B inheritance) + К-L7.1 + К-L16/L17/L18 (К10.3 v2); TLA+ verification К10.4 pending |
```

К-closure row remains unchanged (А'.8 milestone).

### §4.3 — VULKAN_SUBSTRATE.md substantial amendments (v1.0 → v1.1) — reconciled

S-LOCK-14 reconciliation: consolidates V0.B-deferred amendments + К10.3 v2 amendments в single v1.0 → v1.1 bump. Distributed across К10.3 v2 load-bearing commits:

**К-L7.1 + К-L16 commit (Commit 8)** — primary VULKAN_SUBSTRATE amendments:
- §0 L1 — Vulkan 1.3 + async compute mandate (K-L19 V0.B cleanup)
- §0 L7 — К-L19 baseline note (K-L19 V0.B cleanup)
- §2 architecture — pipeline depth section + К-L7.1 coexistence (К-L16)
- §2.3 threading — pipeline depth + queue family roles (К-L16/L19/L7.1)
- §3.4 native compute — К-L19 mandate documentation cleanup
- §7.2 determinism — pipeline drain semantics (К-L16)
- §7.3 async sync hazards — pipeline slot tail read pattern (К-L7.1)

**К-L17 commit (Commit 11)**:
- §4 rendering — display composition section
- §5.5 Mode C — visibility latency через К-L17 composition

**К-L18 commit (Commit 14)**:
- §3.4 — `df_vulkan_unload_mod_resources` C ABI primitive placeholder (К-L18 К10.3 v2)

### §4.4 — MOD_OS_ARCHITECTURE.md amendments

**К-L17 commit (Commit 11)**:
- §3.2 capability syntax — layer tokens (`kernel.layer.intent:{FQN}`, `kernel.layer.combat_feedback:{FQN}`)

**К-L18 commit (Commit 14)**:
- §9.5 — Step 3.6 V resource cleanup placeholder (chain extended 8 → 9 steps)
- §9.5.1 — failure semantics extended с Step 3.6 inclusion
- §11 hot reload — К-L18 compliance note
- §11.2 — new ValidationErrorKind entries (QuiescentStatePreconditionViolated, PipelineQuiescenceTimeout, LayerCapabilityMismatch, VulkanModResourceCleanupFailed)

### §4.5 — README.md — INHERITED V0.B (no К10.3 v2 amendments)

«Hardware Requirements» section already landed V0.B cascade (README.md lines 96-123). No К10.3 v2 amendments.

### §4.6 — К10.3 v2 amendment scope summary

Total К10.3 v2 cross-document amendment scope:
- KERNEL_ARCHITECTURE.md v2.2 → v2.3 (4 К-L additions; К-L19 inherited V0.B)
- VULKAN_SUBSTRATE.md v1.0 → v1.1 (8 sub-section amendments — consolidated K-L19 deferred + К10.3 v2)
- MOD_OS_ARCHITECTURE.md (5 amendment points)
- README.md (0 — inherited V0.B)
- REGISTER.yaml (incremental version bumps + 4 new REQs + audit_trail event)

Substantial cross-document propagation distributed across 3 load-bearing commits per Approach C grouped landing — each load-bearing commit lands its К-L invariant(s) + immediately related cross-document amendments atomically per Lesson #8.

---

## §5 — Halt triggers (К10.3 v2-specific SC-N taxonomy)

К10.3 v2 SC-N taxonomy refined vs v1 — К10.3 v2 imposes less Vulkan code surface risk (V substrate exists) but introduces new coexistence risks (К-L7 sync + К-L7.1 opt-in coexistence per S-LOCK-10/13).

If execution agent encounters any of these conditions, **halt и surface к Crystalka**. Per Lesson #8 corollary: brief promises «halts before damage», not «zero halts».

### SC-1 — Code anchor doesn't match brief evidence

Если any code anchor (Runtime composition, V1 dispatch path, compute_pipeline.h structures, mod_unload.h К10.2 stubs, KernelCapabilityRegistry, ModMenuController) doesn't match brief's described shape after V1 closure (2026-05-19), halt. Brief authored 2026-05-19 from current Phase 0 reads; subsequent drift surfaces here.

### SC-2 — Pipeline slot fence handling bug

Commit 3 (Item 33) lands PipelineSlot state machine. Если pipeline_slot_test surfaces incorrect fence handling (slot transitions Dispatched → ReadableAsTail без fence completion, OR fence completion не triggers transition), halt.

Recovery: verify state machine implementation; check atomic transition logic; surface если non-trivial Vulkan fence API misuse.

### SC-3 — Deep-read contradiction

Any §2.2/§2.3/§2.4/§2.5 mandatory re-read surfaces a file shape that contradicts this brief. Halt и surface contradiction.

### SC-4 — К-L7/К-L7.1 coexistence violation в tests

Commits 3+5 (Items 33+36) land К-L7.1 opt-in pattern. Если V1 sync ExecuteIteration behavior changes (consumer reads stale data, sync semantics broken), OR new pipeline-managed reads see К-L7 sync data unexpectedly, halt.

Recovery: verify coexistence boundary; check `_isPipelineManaged` flag dispatch; surface if K-L7/L7.1 boundary unclear in spec.

### SC-5 — К-L invariant cross-reference integrity broken

Commits 8/11/14 (load-bearing) amend KERNEL_ARCHITECTURE.md + VULKAN_SUBSTRATE.md + MOD_OS_ARCHITECTURE.md. Если after любого load-bearing commit `sync_register.ps1 --validate` flags broken cross-references, halt.

Recovery: verify amendment text against spec verbatim; fix cross-reference; re-validate.

### SC-6 — К-L17 layer composition latency violation в tests

Commit 10 (Items 39+40) lands intent overlay + combat feedback layers. Если К-L17 tests show layer latency exceeding contracted bounds (intent overlay > 16ms, OR combat feedback total > 17ms event-to-visible), halt.

Recovery: profile rendering path; identify GC interaction, lock contention, или layer composition order overhead causing latency; surface к Crystalka — К-L17 semantic refinement may be required.

### SC-7 — К-L18 quiescent state false negative

Commit 12 (Item 41) extends mod_unload primitive с pipeline quiescence check. Если quiescent_state_test surfaces false negative (mod unload succeeds while pipeline has in-flight slots) или false positive (mod unload fails while pipeline empty), halt.

Recovery: verify quiescent state detection logic; check `df_pipeline_is_quiescent` API correctness; surface если К-L18 semantic ambiguity.

### SC-8 — Cross-document amendment integrity broken

Commits 8/11/14 (load-bearing) amend multiple documents atomically. Если after commit one document references К-L invariant text differently than another (e.g., VULKAN_SUBSTRATE.md cites К-L16 verbatim differently than KERNEL_ARCHITECTURE.md), halt.

Recovery: cross-check verbatim text across all amended documents; restore exact consistency per Lesson #7.

### SC-9 — Validation regression post-commit

Если `sync_register.ps1 --validate` exits non-zero after any К10.3 v2 commit, halt immediately.

### SC-10 — Scope creep

Если execution encounters drift не в К10.3 v2 scope (e.g., К10.4 TLA+ verification, К11+ Core migration к native, А'.8 К-closure report preparation, full settings menu implementation, V resource cleanup full native impl), halt и surface. Do не «fix while we're here» — К10.3 v2 scope discipline per S-LOCK-1.

### SC-11 — Push-to-main classifier reminder (operational, не halt)

Known behavior per К10.1 + К10.2 closures: Claude Code auto-mode classifier blocks push-to-main even с explicit instruction. Not a halt — expected. Re-confirm in-session after work done, then push.

### SC-12 — Display composition project drift

Commit 9 (Item 38) lands display composition в `src/DualFrontier.Application/Display/` per S-LOCK-11. Если existing `Application/Rendering/IRenderer.cs` interfaces are inadvertently modified or display composition tries to extend renderer interfaces instead of operating above them, halt.

Recovery: verify S-LOCK-11 boundary — `Display/` operates above `Rendering/`, not extending it.

При halting (SC-1..SC-10, SC-12): author HALT_REPORT в `docs/scratch/A_PRIME_7_K10_3/`, state trigger, state what was/wasn't committed, stop. **Do не commit partial atomic commit** — atomicity protects milestone per Lesson #8.

---

## §6 — Closure protocol (per METHODOLOGY §12.7 canonical)

After Commit 14 lands clean (или Commit 15 if separate closure commit):

### §6.1 — Verify final state

1. `git log --oneline` shows ~14-15 commits added by К10.3 v2 на feature branch `claude/k10_3-v2-pipeline-display-quiescent`
2. `git status` clean working tree
3. `sync_register.ps1 --validate` exit 0
4. `cmake --build` clean, native selftest passes baseline+ scenarios
5. `dotnet build` clean, `dotnet test` baseline+ green (К10.3 v2 new tests additive — final count documented в closure entry)
6. К10.3 v2 benchmarks per §5.1.A Predictions 11-15, 17 measured + documented (results-as-measured; Prediction 16 К-L19 already V0.B-measured)
7. Vulkan validation layer logs (in DEBUG builds) confirm no validation errors during pipeline slot operations или Phase.Compute submissions

### §6.2 — Update brief status + closure section

Set `status: EXECUTED` в this brief's frontmatter; add §9 closure section с commit range + date + commit ledger table + verification metrics + halt protocol activations + lesson candidates + pattern established.

### §6.3 — PR opening (NOT auto-push, per К10.1 + К10.2 precedent)

- Push branch `claude/k10_3-v2-pipeline-display-quiescent` к remote (NOT к `main`)
- Open PR titled «К10.3 v2 — Pipeline depth + display composition + quiescent (К-L7.1/L16/L17/L18)»
- Body summarizes per-commit per-item mapping + verification metrics + halt activations (если any) + closure section
- **DO NOT auto-push к main**. Crystalka reviews + merges per established protocol

### §6.4 — Surface к Crystalka

PR ready для review. Crystalka:
1. Reviews К10.3 v2 closure report content
2. Merges PR к `main`
3. Provides closure report к next Opus deliberation session для К10.4 brief authoring discussion

К10.4 brief authoring informed by:
- К10.3 v2 closure report findings (halt activations, lesson candidates, patterns established)
- К10.3 v2 architectural reality post-landing (4 new К-L invariants в KERNEL_ARCHITECTURE.md v2.3; pipeline depth operational; display composition operational; quiescent state enforced)
- Updated REGISTER state (К10.3 v2 EXECUTED; К10.4 will be AUTHORED при brief authoring)
- К10.4 = **final К10 sub-milestone** — TLA+ formal verification (3 items: 18, 45, 46); likely **single comprehensive switching point** where managed facades retire и native sovereign authority activates across ALL К-L12..L19 invariants per managed-facade-preserved strategy от К10.1/2/3 v2 inheritance

К10.4 scope context per spec §3.14:
- Item 18: TLA+ specification authoring covering 12 invariants (К-L3, К-L4, К-L5, К-L7, К-L7.1, К-L8, К-L11, К-L12, К-L13, К-L15, К-L16, К-L18)
- Item 45: Safety verification CI gate (~1-2 hour runtime)
- Item 46: Liveness verification targeted (К-L7.1 fence completion, К-L12 scheduler progress, К-L16 pipeline drain)

К10.4 no new К-L invariants — pure verification spec landing.

---

## §7 — Brief authority + lifecycle

**Brief authority**: К10 deliberation arc 2026-05-16 → 2026-05-17 (Crystalka + Claude Opus 4.7). 9 S-locks ratified в KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED. К10.3 standalone brief per Option III ratified 2026-05-18 (К10.1 + К10.2 closure precedents extended). **К10.3 v2 brief authored 2026-05-19 per Option D ratification** (Crystalka «принимаю рекомендацию» response к HALT_REPORT_ADDENDUM_2026_05_19 §6 Option D).

**Brief lifecycle (per FRAMEWORK §3.3 + §3.3.1)**:
- DRAFT-PENDING-RATIFICATION at brief authoring (this commit, before Commit 1 enrollment)
- AUTHORED at Commit 1 (post-Crystalka §8 Q-decision ratification — 5 PENDING-RATIFICATION S-LOCKs locked)
- EXECUTED post-Commit 14 (или Commit 15 если separate closure)
- Registered в `tools/briefs/` as Tier 3 Category D per A'.4.5 governance
- v1 brief retained on disk for historical record с `superseded_by: K10_3_EXECUTION_BRIEF_v2.md` annotation

**Brief enrollment**: К10.3 v2 brief added к REGISTER.yaml в Commit 1 atomic с brief authoring per К10.1 + К10.2 precedents. v1 brief enrollment intentionally never happened (halted Phase 0 twice); v2 brief takes its DOC-D-K10_3 slot.

**Brief location**: `tools/briefs/K10_3_EXECUTION_BRIEF_v2.md` (this file).

---

## §8 — Open Q-decisions для Crystalka ratification

Before Commit 1 enrollment, the 5 PENDING-RATIFICATION S-LOCKs require Crystalka decision lock. Each Q-decision below: proposed answer + rationale + alternatives + tradeoff per «без костылей» feedback memory.

### §8.1 — Q1: К-L7.1 application policy — coexistence (S-LOCK-10)

**Question**: How does К-L7.1 («sim tick T+D reads dispatched-at-(T+D-1) state» — one-tick lag) relate к V1's current К-L7 sync semantics (`V1DiffusionPipeline.ExecuteIteration` returns after fence signals per `compute_dispatch.h` line 9 comment)?

**Proposed answer (Option (b) coexistence)**: К-L7.1 applies к **pipeline-managed fields** opted into Phase.Compute machinery. V1's existing sync path (К-L7 atomic-from-observer) remains operational. Per К-L9 «Vanilla = mods» uniformity, mods choose per-field whether to use К-L7 sync pattern (current V1) or К-L7.1 async pipeline-managed pattern (new К10.3 v2).

**Alternatives**:
- **Option (a) Replace**: All V1 consumers migrate to slot-tail read pattern. К10.3 v2 includes V1 consumer migration.
  - **Без костылей tradeoff**: forces V1 ExecuteIteration semantic change; consumers currently calling and reading same-tick must restructure; net invasive across V substrate consumers. Costly migration without immediate benefit.
- **Option (c) Wrap**: ExecuteIteration unchanged but internally goes through pipeline slots; sync semantics preserved by "block on slot N+1 ReadableAsTail before returning". Sim thread never gets ahead of display.
  - **Без костылей tradeoff**: incompatible с К-L16 «Simulation thread runs D ticks ahead» — wraps async machinery in sync facade defeating pipeline depth purpose. Architectural inconsistency.

**Rationale for (b)**: К-L9 «Vanilla = mods» uniformity preserved (author choice); К-L7 sync path remains operational без regression risk for V1 consumers; К-L7.1 opt-in available для new consumers seeking pipeline depth benefits. Aligns с managed-facade-preserved strategy.

**Decision needed**: Confirm (b) or specify alternative.

### §8.2 — Q2: VULKAN_SUBSTRATE.md version reconciliation policy (S-LOCK-14)

**Question**: VULKAN_SUBSTRATE.md frontmatter is v1.0 but content has accumulated V0.A/B/C.1/C.2/V1 additions without version bump. К10.3 v2 needs to amend (К-L7.1/L16/L17/L18) + may need к land К-L19 amendments deferred from V0.B (§0 L1, §0 L7, §3.4 documentation cleanup). How к reconcile version?

**Proposed answer**: Combined v1.0 → v1.1 bump в К10.3 v2 cascade, incorporating: (a) deferred K-L19 amendments from V0.B (§0 L1, §0 L7, §3.4); (b) К-L7.1/L16/L17/L18 К10.3 v2 amendments per spec Part 7.5. Reconciliation distributed across К10.3 v2 load-bearing commits per Lesson #8 atomic compilable units (Commit 8 substantial: K-L19 cleanup + K-L7.1/L16; Commit 11: K-L17; Commit 14: K-L18 placeholder).

**Alternatives**:
- **Option (a) Separate V0.B follow-up commit before К10.3 v2 starts** — V0.B owes a cleanup commit to land deferred VULKAN_SUBSTRATE.md amendments; К10.3 v2 starts от cleaner v1.0a baseline.
  - **Без костылей tradeoff**: cleaner attribution (V0.B owns K-L19 amendments fully), но extra cascade step. К10.3 v2 brief authoring already done; replanning incurs cost.
- **Option (b) Skip V0.B-deferred amendments; К10.3 v2 lands only its own** — К-L19 documentation gap accepted (К-L19 invariant landed; VULKAN_SUBSTRATE.md just doesn't reflect it). Future cleanup опционал.
  - **Без костылей tradeoff**: leaves architectural documentation gap; VULKAN_SUBSTRATE.md doesn't reflect К-L19 user-facing commitment. Inconsistent doc state.

**Rationale for proposed (combined)**: К10.3 v2 already touches VULKAN_SUBSTRATE.md substantially (8 sub-section amendments per spec Part 7.5); folding V0.B-deferred amendments into same v1.1 bump adds minor work but eliminates outstanding doc debt. Aligns с Lesson #8 atomic compilable units (single v1.1 bump preferred over fragmented bumps).

**Decision needed**: Confirm combined or specify alternative.

### §8.3 — Q3: Display composition project location (S-LOCK-11)

**Question**: К-L17 layer composition framework: which project hosts it?

**Proposed answer**: `src/DualFrontier.Application/Display/` (NEW directory). Existing `Application/Rendering/IRenderer.cs` interfaces preserved unchanged; `Display/` operates above renderer abstraction.

**Alternatives**:
- **Option (a) `src/DualFrontier.Runtime/Display/`** — V substrate side hosts composition.
  - **Без костылей tradeoff**: K-L17 mixes substrate concerns (rendering primitives) с cross-substrate concerns (input + bus + composition). Couples Runtime к input event types + Fast tier subscriptions. Violates V substrate scope per VULKAN_SUBSTRATE.md Preamble «V substrate exposes presentation + compute primitives only; mods cannot reach the substrate layer directly except through IModApi v3».
- **Option (c) `src/DualFrontier.Application/Rendering/` — extend existing rendering interfaces** — К-L17 layers extend renderer.
  - **Без костылей tradeoff**: `IRenderer` semantics shift от «one renderer» к «one renderer per layer + composer» — interface change. К10.3 v2 brief plans layer abstraction, not renderer extension. Risk of interface churn.

**Rationale for proposed (b) `Application/Display/`**: К-L17 composition is cross-substrate (consumes V renderer + input + bus). Application layer is the natural home (above substrate, below domain). New `Display/` directory keeps existing `Rendering/` interfaces intact (S-LOCK-11 hard rule).

**Decision needed**: Confirm `Application/Display/` or specify alternative.

### §8.4 — Q4: К-L18 UI integration scope (S-LOCK-12)

**Question**: К-L18 mandates «Mod management UI (settings menu, hot reload tooling) enforces pause precondition». What scope for К10.3 v2?

**Proposed answer**: К10.3 v2 К-L18 UI = **helper layer wiring** (SimulationStateController + ModMenuController pause hook + Step 3.6 V resource cleanup placeholder). Full settings menu / preferences UI = V-cycle or К-extensions scope, deferred per managed-facade-preserved pattern.

**Alternatives**:
- **Option (a) Full settings menu implementation в К10.3 v2** — text rendering, button widget, scroll, full UI framework + settings menu + mod management screen + pause toggle.
  - **Без костылей tradeoff**: massive scope expansion (UI framework = multi-week effort itself). Breaks К10.3 v2 scope discipline per Lesson #20. V substrate text rendering deferred к later V cycle; UI framework deferred similarly.
- **Option (c) К-L18 architectural only — no UI wiring** — invariant landed, enforcement deferred к later UI work.
  - **Без костылей tradeoff**: К-L18 invariant landed but no enforcement path; К10.2 stub remains stub. Architectural commitment without operational backing. Inconsistent с К10.3 v2 «substantial implementation» framing in v1 brief.

**Rationale for proposed (b) helpers only**: SimulationStateController provides programmatic API для К-L18 enforcement; ModMenuController existing class extended с pause hook usage; tests verify workflow. UI framework deferred orthogonally. Aligns с К10.1/К10.2 managed-facade-preserved precedent (architecture + minimal enforcement; full UI later).

**Decision needed**: Confirm helpers-only or specify alternative.

### §8.5 — Q5: V1 sync dispatch coexistence с Phase.Compute (S-LOCK-13)

**Question**: Item 35 introduces Phase.Compute с VkQueueSubmit batching. Does V1's `dispatch_compute_field` migrate к Phase.Compute, or stay synchronous?

**Proposed answer**: V1's `dispatch_compute_field` (synchronous VkQueueSubmit + vkWaitForFences per `compute_dispatch.h`) **remains unchanged**. Phase.Compute (Item 35) introduces parallel async dispatch path для pipeline-managed dispatches. V1 consumers experience no behavior change. Future К-extensions могут deprecate sync path если pipeline-managed becomes universal.

**Alternatives**:
- **Option (a) Migrate V1 к Phase.Compute** — V1 ExecuteIteration becomes pipeline-managed; sync semantics break.
  - **Без костылей tradeoff**: V1 consumers' ExecuteIteration calls change behavior. M-V demonstration mods (M-V1 mana, M-V2 electricity per V1 brief) currently expect sync result. Migration costs without immediate benefit. Couples Q1 + Q5.
- **Option (c) Wrap V1 to Phase.Compute с sync facade** — V1 ExecuteIteration goes через Phase.Compute slot machinery but blocks on completion.
  - **Без костылей tradeoff**: same problem as Q1 Option (c) — wrapping async in sync defeats pipeline depth purpose. Architectural inconsistency.

**Rationale for proposed (b) parallel paths**: V1 sync path is operational and tested (V1 closure 2026-05-19, PR #40 merged today); К10.3 v2 should не break it. Phase.Compute is **new substrate primitive** for pipeline-managed dispatches; V1 consumers can opt in later if desired. Per К-L9 «Vanilla = mods», V1 future could migrate per-pipeline.

**Decision needed**: Confirm parallel paths or specify alternative.

### §8.6 — Q-decision ratification flow ✓ COMPLETED 2026-05-19

Crystalka ratified all 5 PENDING-RATIFICATION S-LOCKs 2026-05-19 via Q1+Q5 / Q2 / Q3 / Q4 recommended-option lock:
- Q1 (S-LOCK-10): Coexistence opt-in ✓
- Q5 (S-LOCK-13): V1 sync parallel paths ✓
- Q2 (S-LOCK-14): Combined v1.0→v1.1 reconciliation ✓
- Q3 (S-LOCK-11): src/DualFrontier.Application/Display/ NEW ✓
- Q4 (S-LOCK-12): Helpers + ModMenuController hook ✓

Brief frontmatter updated DRAFT-PENDING-RATIFICATION → AUTHORED. S-LOCK-10/11/12/13/14 PENDING markers replaced с ✓ LOCKED annotations. Ready for Commit 1 enrollment + cascade execution.

---

**End of K10.3 v2 brief draft. Estimated 14-22 hours auto-mode execution post-ratification.**

К10.3 v2 closes 10 of 46 К10 items (cumulative с К10.1 + К10.2 + V0.B К-L19 partial: 37+2=39 of 46). Remaining 7 items distributed across К10.4 (3 items: 18, 45, 46) + Item 14 deferred к К11+ + Item 25 cross-cutting к А'.8 + Item 10 NUMA deferred к К-extensions + Items 22/23 (hot-patching + RT guarantees) К-extensions scope.

К10 as a whole remains AUTHORED-IN-PROGRESS until К10.4 closure. К-series formally closes only после all four К10 sub-milestones + К-closure report (А'.8).

**К10.4 outlook unchanged from v1**: К10.4 = TLA+ formal verification finalization. Architecturally significant beyond TLA+ alone — К10.4 likely single comprehensive switching point где managed-facade-preserved strategy retires и native sovereign authority activates across all 8 К10-landed invariants (К-L12/L13/L14/L15/L7.1/L16/L17/L18/L19; К-L6 superseded).

«Halt is success, не failure» per Lesson #8 corollary. Brief's honest guarantee: bad premises surface at Phase 0 / at deep-read / at the compile gate, before they reach `main`.

---

## §9 Closure section (К10.3 v2 EXECUTED 2026-05-20)

### §9.1 Commit ledger (15-commit atomic cascade)

| # | Hash | Title | Purpose |
|---|---|---|---|
| 1 | `1982351` | docs(briefs): K10.3 v2 brief authored | Brief authoring + v1 supersession enrollment |
| 2 | `2e441d9` | feat(native-test): K10.3 v2 — native test scaffold | DF_CHECK runner extension |
| 3 | `15f8aca` | feat(kernel-native): Item 33 tick pipeline depth | К-L7.1 + К-L16 foundation; D=2 default |
| 4 | `3e0fc84` | feat(kernel-native): Item 35 Phase.Compute | VkQueueSubmit batching scaffold; V1 sync coexists |
| 5 | `859e6dc` | feat(kernel-native): Item 36 pipeline slot read API | К-L7.1 opt-in coexists с К-L7 sync default |
| 6 | `971cb24` | feat(kernel-native): Item 34 drain/refill protocols | Pipeline pause/resume + save/load serialization |
| 7 | `26cce91` | feat(kernel-native): Item 37 slot transition wake | WakeOnSlotTransitionAttribute + counter scaffold |
| 8 | `fd5c7e2` | **LOAD-BEARING 1/3** К-L7.1 + К-L16 | KERNEL_ARCH v2.3 partial + VULKAN_SUBSTRATE v1.0→v1.1 reconciliation |
| 9 | `fc2a908` | feat(application): Item 38 display composition framework | К-L17 layer registry + composition order; Application/Display/ |
| 10 | `28a2029` | feat(application): Items 39+40 intent overlay + combat feedback | К-L17 layer infrastructure + capability tokens |
| 11 | `9c660d9` | **LOAD-BEARING 2/3** К-L17 amendment | KERNEL_ARCH v2.3 partial + VULKAN_SUBSTRATE v1.1 partial + MOD_OS §3.2 |
| 12 | `90c1dd0` | feat(kernel-native): Item 41 К-L18 quiescent enforcement | mod_unload pipeline quiescence verification |
| 13 | `d29a765` | feat(application): Item 42 К-L18 helper layer wiring | SimulationStateController + Step 3.6 V cleanup placeholder |
| 14 | `f09055a` | **LOAD-BEARING 3/3** К-L18 amendment | KERNEL_ARCH v2.3 final + MOD_OS v1.11 + VULKAN_SUBSTRATE v1.1 final |
| 15 | `PENDING` | governance: К10.3 v2 closure | DOC-D-K10_3 EXECUTED + audit_trail EVT + MIGRATION_PROGRESS + brief §9 |

### §9.2 Verification metrics (final state)

| Gate | Baseline (pre-К10.3 v2) | К10.3 v2 final | Delta |
|---|---|---|---|
| Full sln tests | 936 | 1022 | +86 |
| - Application.Tests (NEW) | 0 | 45 | +45 |
| - Modding.Tests | 389 | 395 | +6 (layer tokens) |
| - Runtime.Tests | 292 | 292 | 0 |
| - Core.Interop.Tests | 167 | 202 | +35 (pipeline slot interop) |
| - Core.Tests | 75 | 75 | 0 |
| - Persistence.Tests | 4 | 4 | 0 |
| - Mod.ManifestRewriter | 7 | 7 | 0 |
| - Systems.Tests | 2 | 2 | 0 |
| Native selftest | 29 (К10.2 baseline) | 33 (+4 К-L18 integration) + 14 К10.3 v2 pipeline scenarios | +18 net |
| REGISTER documents | 253 | 253 | 0 |
| REGISTER requirements | 38 | 40 | +2 (REQ-K-L17, REQ-K-L18; REQ-K-L7_1 + REQ-K-L16 already enrolled Commit 8 / pre-existing) |
| REGISTER audit_trail | 22 | 23 | +1 (EVT-2026-05-20-K10_3-V2-CLOSURE) |
| `sync_register.ps1 -Validate` | exit 0 | exit 0 | unchanged (advisory orphans baseline) |
| `dotnet build` | clean | clean | unchanged |
| `cmake --build` | clean | clean | unchanged |

### §9.3 Halt protocol activations (К10.3 v2 cascade)

**Hard gate halts during cascade**: 0 (zero halts during 15-commit execution — К-L14 thesis seventh verification).

**Pre-cascade halts on К10.3 series**: 2 (v1 brief halted Phase 0 twice 2026-05-18 + 2026-05-19 per HALT_REPORT + HALT_REPORT_ADDENDUM; supersession к v2 chose Option D per Crystalka 2026-05-19 lock). Halts surfaced V substrate prerequisite (V0.A/V0.B/V0.C.1/V0.C.2/V1 landed 2026-05-18..2026-05-19) + duplicate-landed-work detection (v1 Commits 3/4/5 duplicate; К-L19 + Items 43/44 landed V0.B per cross-stream prerequisite resolution).

«Halt is success, не failure» empirically validated: v1 halts produced clean v2 brief reflecting current ground truth; v2 cascade executed zero-halt across 15 commits.

### §9.4 Lesson candidates (К10.3 v2 patterns)

1. **Sub-invariant pattern repeats** (К-L7.1 mirrors К-L3.1) — К-L invariant numerical extension when parent integrity must be preserved across opt-in/coexistence boundary. К-L9 «Vanilla = mods» author choice surface enables natural per-field opt-in. Pattern validated across two cases (К-L3.1 component storage path + К-L7.1 GPU compute pipeline slot binding) — promote к METHODOLOGY Lesson после third validation case surfaces.

2. **Capability-mismatch placeholder reservation** — К-L17 LayerCapabilityMismatch + К-L18 QuiescentStatePreconditionViolated/PipelineQuiescenceTimeout/VulkanModResourceCleanupFailed reserved в §11.2 amendments в advance of full integration (placeholder ValidationErrorKind entries lock the error semantic so future implementations consume the same error path). Pattern preserves architectural integrity across multi-phase implementation.

3. **Application/Display/ new directory location** (S-LOCK-11) — К-L17 lives above renderer abstraction, не extending IRenderer interfaces. New directory establishes clean separation: V substrate exposes rendering primitives, Application/Display composes them. Future-proof against renderer-implementation churn (Vulkan vs Godot DevKit) — same composition framework consumed by both backend implementations.

4. **Helpers-only scope discipline** (S-LOCK-12 К-L18) — Architecturally complete invariant landing с minimum operational glue, full UI deferred per managed-facade-preserved precedent. К10.3 v2 third application of managed-facade-preserved pattern (К10.1 native scheduler authoritative + managed adapter facade preserved; К10.2 native bus authoritative + managed bus facade preserved; К10.3 v2 К-L17/L18 architecture landed + concrete UI integration deferred). Pattern validated for К + V architectural commitments across 3 milestones.

### §9.5 Pattern established (К-L14 thesis seventh verification)

К-L14 thesis «performance derives from clean complex architecture без compromise» now empirically validated across **seven consecutive zero-hard-gate-halt cascades**:

| # | Milestone | Closure date | Halts |
|---|---|---|---|
| 1 | К0..К8 lineage | through 2026-05-09 | 0 (8 milestones combined) |
| 2 | V0.A | 2026-05-18 | 0 |
| 3 | V0.B | 2026-05-18 | 0 |
| 4 | V0.C.1 | 2026-05-19 | 0 |
| 5 | V0.C.2 | 2026-05-19 | 0 |
| 6 | V1 | 2026-05-19 | 0 |
| 7 | **К10.3 v2** | **2026-05-20** | **0** |

Decade-horizon planning thesis evidence accumulates further. К-series + V substrate authoring streams both demonstrate that careful brief authoring + Phase 0 ground-truth reads + atomic compilable unit cascade + multi-layer code anchor verification + ratified Q-decisions per S-LOCK pattern combine into reliable zero-halt execution.

### §9.6 Post-К10.3 v2 К-Lxx series state

**Cumulative К-Lxx series total: 20 invariants**:

| # | Invariant | Status | Landed at |
|---|---|---|---|
| К-L1 | Native language (C++20) | LOCKED | К0 |
| К-L2 | Bindings (pure P/Invoke) | LOCKED | К0 |
| К-L3 | Component storage paths | LOCKED | К1-К8 |
| К-L3.1 | Bridge sub-invariant (Path β managed) | LOCKED | 2026-05-10 |
| К-L4 | Type IDs (explicit registry) | LOCKED | К2 |
| К-L5 | Bootstrap orchestration | LOCKED | К3 |
| К-L6 | **SUPERSEDED by К-L12** | SUPERSEDED 2026-05-18 | К10.1 closure |
| К-L7 | Span protocol | LOCKED | К5 |
| К-L7.1 | GPU compute pipeline slot binding sub-invariant | LOCKED 2026-05-20 | К10.3 v2 commit 8 (LOAD-BEARING 1/3) |
| К-L8 | Component lifetime | LOCKED | К4 |
| К-L9 | Mod parity («Vanilla = mods») | LOCKED | К3 |
| К-L10 | Decision rule (§8 metrics) | LOCKED | К7 |
| К-L11 | Production storage backbone (NativeWorld) | LOCKED | К7 |
| К-L12 | Native kernel scheduling | LOCKED | К10.1 closure 2026-05-18 |
| К-L13 | On-demand system activation | LOCKED | К10.1 closure 2026-05-18 |
| К-L14 | Performance derives from cleanness | LOCKED | К10.1 closure 2026-05-18 |
| К-L15 | Native bus authority + three-tier dispatch | LOCKED | К10.2 closure 2026-05-18 |
| К-L16 | Simulation tick pipeline depth | LOCKED 2026-05-20 | К10.3 v2 commit 8 (LOAD-BEARING 1/3) |
| К-L17 | Display composition multi-layer | LOCKED 2026-05-20 | К10.3 v2 commit 11 (LOAD-BEARING 2/3) |
| К-L18 | Mod lifecycle quiescent state | LOCKED 2026-05-20 | К10.3 v2 commit 14 (LOAD-BEARING 3/3) |
| К-L19 | Hardware tier commitment | LOCKED 2026-05-18 | V0.B cross-stream prerequisite resolution |

Next К-L invariant landing: К10.4 brief authoring (no new К-L; TLA+ formal verification only — Items 18/45/46). К-series formal closure waits для all 4 К10 sub-milestones + А'.8 К-closure report.

### §9.7 Next-session options (Crystalka prerogative)

- **Option α**: V2 brief portion execution (PR #41) — V substrate full close per VULKAN_SUBSTRATE.md §1.4 (WaveKernel + DirectionExtractKernel + multi-field coexistence)
- **Option β**: К10.4 brief authoring — TLA+ formal verification finalization (3 items: 18, 45, 46); likely single comprehensive switching point где managed-facade-preserved strategy retires и native sovereign authority activates across 8 К10-landed invariants
- **Option γ**: A'.8 К-closure report preparation — formal К-Lxx enumeration + Lessons promotion
- **Option δ**: A'.9 Roslyn analyzer milestone — architectural analyzer для M-cycle migration verification

Independent streams unchanged.
