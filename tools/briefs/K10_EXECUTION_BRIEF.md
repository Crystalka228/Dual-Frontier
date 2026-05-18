---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K10_EXECUTION
category: D
tier: 3
lifecycle: AUTHORED-SKELETON
owner: Crystalka
version: "0.1"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K10_EXECUTION
---
# К10 Native Kernel Scheduler — Execution Brief (SKELETON)

**Brief authored**: 2026-05-17 (skeleton; Claude Opus 4.7 К10 deliberation arc closure)
**Lifecycle**: AUTHORED-SKELETON — awaits full brief authoring at proper milestone timing (post-К8.5 closure per Phase A' sequencing)
**Target session**: Future Opus deliberation session (К10 execution brief authoring)
**Subsequent execution**: Claude Code execution mode for implementation
**Estimated full brief size**: 1500-2500 lines (substantial; К10 spec is 46 items + 12 К-L invariants + TLA+ scope)
**Parent**: К10 specification `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v2.0 LOCKED

---

## 0. Executive summary (skeleton)

Implement К10 specification: full native kernel scheduler, bus, control plane migration, V substrate interlock, TLA+ formal verification.

**Reference specification**: `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v2.0 (1875 lines), particularly:
- Part 3: 46-item К10 scope (Items 1-46 grouped by S lock origin)
- Part 4: К10 deliverables
- Part 5.1.A: 18 К10 architecture realization predictions (closure gate)
- Part 8: Risk register R-K10-1..R-K10-9

**К-L invariants targeted**: К-L7.1 (sub), К-L12 (full native kernel scheduling), К-L13 (on-demand activation), К-L14 (performance from architecture — *architecturally established* at this milestone), К-L15 (native bus three-tier), К-L16 (simulation tick pipeline depth), К-L17 (display composition multi-layer), К-L18 (mod lifecycle quiescent state), К-L19 (hardware tier commitment).

**К-L6 SUPERSEDED** rationale realized в this implementation.

---

## 1. Phase 0 — Preflight reads (skeleton)

Full brief к specify:
- К10 specification full read (Parts 0-11)
- KERNEL_ARCHITECTURE.md v1.6 (post-amendment) К-L table
- VULKAN_SUBSTRATE.md v1.0 LOCKED + V interlock sections (post-amendment)
- MOD_OS_ARCHITECTURE.md v1.8 (post-amendment) capability + lifecycle sections
- GameBootstrap.cs (production wiring ground truth per Lesson #22)
- ThreadPool.h, scheduler.cpp (native subsystem entry points)
- IGameServices.cs + SystemExecutionContext.cs (managed API boundary)
- TLA+ tooling availability (apalache, TLC) — capability check

---

## 2. Expected sub-milestones (skeleton)

К10 split into sub-milestones at brief authoring time. Skeleton ordering:

**Sub-milestone K10.A — Native scheduler primitives** (Items 1, 2, 3, 4, 5):
- Native dependency graph (replacing managed)
- Native thread pool extension
- Wake-up registry (5 wake types)
- Wake registry diagnostic API
- Dynamic per-tick graph computation

**Sub-milestone K10.B — Scheduling policies** (Items 6, 7, 8):
- Priority-based dispatch
- Resource quotas
- Preemption semantics

**Sub-milestone K10.C — Memory and IPC** (Items 9, 10, 11, 12, 13):
- Shared memory regions
- CPU affinity hints
- Work stealing
- Phase barrier semantics

**Sub-milestone K10.D — Native execution layer** (Items 14, 15, 16):
- Core systems migration к native (К11+ scope flag)
- Batched callback ABI
- К-L6 supersession + К-L12/13/14 amendment landing

**Sub-milestone K10.E — State and observability** (Items 17, 18, 19, 20):
- Write-through hook for NativeWorld (StateChange wake trigger)
- Formal verification — TLA+ spec authoring
- Observability hooks
- Scheduler intrinsics

**Sub-milestone K10.F — Mod integration** (Items 21, 22, 23):
- Mod scheduler authority (per-mod sub-schedulers)
- Hot-patching primitives
- Real-time scheduling guarantees

**Sub-milestone K10.G — Test infrastructure and migration** (Items 24, 25):
- Test infrastructure migration (catch2/gtest + ManagedTestScheduler fixture)
- К-closure report contributions

**Sub-milestone K10.H — S1 lock items (native bus)** (Items 26, 27, 28, 29, 30):
- Native bus implementation (three-tier dispatcher)
- Managed bus facade + C ABI bridge
- Event type registry (tier-annotated)
- Subscriber contract enforcement
- Background work queue + idle-slot scheduling

**Sub-milestone K10.I — S2 + S3 items** (Items 17 amended + 18 extended + 19 extended + 21 amended + 31, 32):
- Hybrid filter primitive + commit-time hook
- TLA+ spec extensions
- Filter metrics
- Per-mod sub-scheduler teardown encapsulated
- Background queue save-integrated storage
- Native unload primitive + ModUnloadResult

**Sub-milestone K10.J — S8 + S-TLA items** (Items 33-46):
- Pipeline depth mechanism
- Pipeline drain/refill protocols
- Phase.Compute scheduler integration
- Pipeline slot read API
- Filter primitive pipeline integration
- Display composition framework
- Intent overlay layer infrastructure
- Combat feedback layer infrastructure
- К-L18 quiescent state enforcement
- Settings menu / mod management UI integration
- Async compute queue mandate
- Hardware capability check
- TLA+ specification authoring (12 invariants)
- Safety verification CI gate
- Liveness verification targeted

Full brief к decide sub-milestone consolidation (precedent: К8.3+К8.4 combined into A'.5).

---

## 3. Halt conditions (skeleton)

Full brief к specify halt protocol. Anticipated halt classes:

- **HG-1**: Working tree dirty before execution
- **HG-2**: К10 specification version mismatch (post-amendment cascade may shift v2.0 → v2.x)
- **HG-3**: GameBootstrap.cs / production wiring divergence от К10 spec assumptions (Lesson #22 risk class)
- **SC-1**: Hardware capability check fails (RDNA 1+/Turing+/Arc+ requirement per К-L19)
- **SC-2**: TLA+ tooling unavailable (apalache/TLC) — К10 deliverable incomplete
- **SC-3**: Batched callback ABI overhead measurement violates Prediction 6 (80-95% reduction expected) — К-L14 falsification candidate
- **SC-4**: Pipeline depth D=2 produces simulation thread fence wait — К-L16 violation

---

## 4. Q-N seeds (skeleton)

From К10 spec Part 6, 49+ open Q-N candidates. Full brief к surface execution-time questions. Anticipated:

- Q-N-EXEC-1: К10.A native scheduler primitive ordering — replace managed scheduler atomically, или incremental с feature flag?
- Q-N-EXEC-2: TLA+ spec authoring parallelization с implementation — TLA+ authored по mere completion of corresponding К-L invariant code, или batched at end?
- Q-N-EXEC-3: К11+ Core systems migration flag — мark items K11-1..K11-5 as deferred-execution vs deferred-authoring?
- Q-N-EXEC-4: Hardware capability check failure UX — fail-fast at startup с diagnostic, or graceful degradation path?
- Q-N-EXEC-5: Background queue save format versioning — schema migration approach для save-game backward compatibility?

---

## 5. Closure protocol stub (skeleton)

Full brief к specify. Expected:
- sync_register.ps1 --validate exit 0 gate
- All 12 К-L safety verification CI gate runs clean
- 18 К10 architecture realization predictions measured + recorded
- К-closure report contributions authored (A'.8 prerequisite)
- New EVT entry — EVT-2026-XX-XX-K10-EXECUTION-CLOSURE
- К10 specification version v2.0 → v3.0 (post-execution version reflecting realized state)

---

## 6. Reference appendices (skeleton)

Full brief к include:
- A: К10 specification verbatim sections relevant к each sub-milestone
- B: TLA+ tooling reference + apalache/TLC usage patterns
- C: Async compute queue Vulkan API surface (vkGetPhysicalDeviceQueueFamilyProperties etc) — verbatim per Lesson #7
- D: К-Lxx invariant verbatim text для each invariant targeted
- E: Performance measurement protocol matching Predictions 1-18

---

**End of skeleton.**

**Promotion к AUTHORED** triggers: full brief authoring at Opus deliberation session post-К8.5 closure. Estimated session duration: ~4-8 hours deliberation (К10 substantial; multiple Q rounds expected). Skeleton к patch-brief mutation pattern if К10 spec amended pre-execution (precedent: K8_3_BRIEF_REFRESH_PATCH).
