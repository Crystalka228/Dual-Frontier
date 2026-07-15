---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-PERSISTENCE_SNAPSHOT_CONTRACT
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: "0.1.0"
next_review_due: post-ratification closure
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-PERSISTENCE_SNAPSHOT_CONTRACT
---
# Persistence and Snapshot Contract (the A7 contract)

> **Document class: authored-proposal (normative-target). NOT current truth, NOT enforceable law.** Produced by the Architecture Decomposition & Contracts session 2026-07-15 ([docs/reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)). Becomes normative only upon Crystalka ratification per FRAMEWORK.md §7. Until then no document may cite it as authority; conflicts resolve in favor of existing LOCKED docs.
>
> **Scope note.** This skeleton constrains; it does not schedule. Save-system implementation remains deferred to its milestone. The purpose is that ECS, Fields, Event Bus, Mod OS and GPU pipeline stop choosing mutually incompatible persistence semantics before that milestone. The deferral itself is ratified and correct: `SaveSystem.cs` `throw new NotImplementedException` is "**deliberate** state", and the save system "will design against the fully-completed kernel + runtime + mod foundation, not against the migration-in-progress moving target" ([MIGRATION_PLAN_KERNEL_TO_VANILLA.md §8.1](./MIGRATION_PLAN_KERNEL_TO_VANILLA.md), lines 652–659). This contract holds invariants and boundaries only — no file formats, no I/O design, no implementation.

**Ratification path.** Either of two destinations is acceptable; the invariants must not fork:

- remain a standalone Tier-1 contract (authored-proposal → AUTHORED → LOCKED per FRAMEWORK.md §7.2 amendment-milestone protocol), or
- fold as the invariants preamble into a future `PERSISTENCE.md` authored at the save-system milestone, which then owns implementation design on top of these boundaries.

**Non-goals of this document** (they belong to the save-system milestone, not here):

- save-file layout, container format, compression selection (the orphaned codec layer is inventory, not design);
- I/O strategy, autosave cadence, save-slot UX;
- cloud sync, replay files, network state transfer (out of scope for the project today — VULKAN §7.2.1: "single-player, no replays");
- performance engineering of save/load beyond the boundary and coordination constraints of §1/§6.

Normative statements below carry `PS-n` ids so later documents can cite a specific law instead of this file wholesale (same convention as the К-L series). The full index is at the end.

## Why this exists — the fragment inventory

Persistence is deferred everywhere, yet every subsystem has already chosen lifetime/consistency semantics that a future save system must compose with:

- [FIELDS.md](./FIELDS.md) §Save/load (lines 296–305): blob = primary buffer + conductivity + flags; back buffer not serialized; width/height mismatch = load error.
- K-L3.1: Path β managed components are "runtime-only (Q4.b lock) — not persisted by save system" ([K_L3_1_AMENDMENT_PLAN.md](./K_L3_1_AMENDMENT_PLAN.md) line 91); "save system reconstructs on load post-G-series" ([MIGRATION_PLAN_KERNEL_TO_VANILLA.md](./MIGRATION_PLAN_KERNEL_TO_VANILLA.md) line 245).
- KFNS Item 31 ([KERNEL_FULL_NATIVE_SCHEDULER.md](./KERNEL_FULL_NATIVE_SCHEDULER.md) lines 924–933): background tier event queue persists with the save file (`df_scheduler_serialize_background_queue`); [EVENT_BUS.md](./EVENT_BUS.md) line 59: "versioned wire format, schema v1".
- K-L16 ([KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) line 65): "Pipeline drain orderly at save/pause; pipeline refill orderly at load/resume" — versus [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) §7.2.0 (line 1281): save snapshots display tick state, "pipeline drain не required at save time". §1 reconciles these.
- VULKAN §7.2.1 (lines 1292–1295): "Save/load must produce reproducible state on load"; CPU reference kernels "produce canonical state for save snapshots" (design mitigation, explicitly not implemented).
- [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) §9.4 / §12 D-6: save records `(modId, modVersion)` per active mod; missing mod → strip its components, keep entities (LOCKED, line 1131).
- KERNEL Part 4 ([KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) line 667): "Save/load of Native World → Persistence integration milestone (M-Persistence?)".
- `src/DualFrontier.Persistence/` (Compression codecs + `PawnSnapshot` / `SaveMeta` / `StorageSnapshot` / `TileMapSnapshot` / `WorldSnapshot`) is orphaned from production — only tests reference it; `src/DualFrontier.Core.Interop/PipelineSlotInterop.cs` lines 202–203: "SaveSystem currently stub (NotImplementedException)".
- FHE D7 ([FHE_INTEGRATION_CONTRACT.md](./FHE_INTEGRATION_CONTRACT.md) lines 96–100): the Persistence layer "already pins `ContractsVersion.Current`"; the FHE library version is a reserved fourth version field in save metadata.

Each fragment is locally reasonable. Composed naively, they produce a save system that cannot exist. The sections below pin the composition.

## §1. Canonical snapshot boundary

**PS-1 — the one invariant everything hangs on: a snapshot is taken at a quiesced tick boundary, and the saved tick is the last fully-committed simulation tick.** Every inclusion/exclusion rule in §2 is downstream of this.

"Fully committed" for a simulation tick N means, precisely:

1. **All phases of tick N have completed.** Every `WriteBatch` flushed at its phase boundary ([ECS.md](./ECS.md) line 90); no span leases outstanding.
2. **Deferred destructions applied.** `DestroyEntity` defers component removal to the next phase boundary ([ECS.md](./ECS.md) line 100); at the snapshot boundary the pending-destruction set is empty and versions are already bumped.
3. **Deferred event queues at a known generation.** Managed `[Deferred]` queues drain at phase boundaries ([EVENT_BUS.md](./EVENT_BUS.md) lines 44, 49); a queue not drained by tick end already signals a defect (line 128). The native Normal tier pending queue is drained by `df_bus_drain_normal_batch` at the same cadence (line 57). The Background tier queue is frozen at its post-tick-N content.
4. **Pipeline slots quiescent per K-L18.** All fences completed, no in-flight compute dispatches ([KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) line 67; `df_pipeline_is_quiescent`). Save reuses the same quiescence machinery the mod lifecycle already mandates: `PauseAsync` → `WaitForQuiescenceAsync` → operation → `ResumeAsync`.

### Reconciling K-L16 "drain at save" with VULKAN §7.2.0 "no drain required"

K-L16 and KFNS Item 34 (lines 990–992) mandate "orderly pipeline drain at save/pause"; VULKAN §7.2.0 (S8-Q1.5, line 1281) says snapshot the display tick state (`CurrentSimTick − D`), "pipeline drain не required at save time". These are two mechanisms for reaching the same semantic point. This contract pins the semantics, not the mechanism:

- **PS-2 — Saved tick = the last fully-committed sim tick.**
  - Under drain-then-save, that is `CurrentSimTick` after the drain retires the pipeline tail.
  - Under snapshot-behind, that is `CurrentSimTick − D`, whose pipeline-managed results are all retired by construction of K-L16 (display reads exactly that tick).
- **PS-3 — The display tail — the D in-flight ticks ahead of the saved tick — is derived state, never saved.** On load the pipeline refills (Item 34) and the tail regenerates. No serialized bytes may originate from a tick newer than the saved tick.
- The mechanism choice (drain vs snapshot-behind, or a hybrid) is implementation freedom at the save milestone — subject to §6 coordination and to KFNS Prediction 17 (line 1324: pipeline drain on save coordinated with background-queue serialization, <100 ms save-initiation target).

Corollary: there is no "save mid-tick", no "save with dispatches in flight", and no snapshot that mixes state from two ticks. Any subsystem whose state cannot be expressed at a committed tick boundary is, by that fact, excluded from the snapshot (§2) or defective.

## §2. Inclusion/exclusion table

Normative-target composition of one snapshot. "Authority" names the fragment that already decided the row; rows marked *proposed* are decided here for the first time.

| State | In snapshot? | Authority | Notes |
|---|---|---|---|
| Path α component data (`NativeWorld`) | **Yes** | KERNEL Part 4 (line 667); K-L11 | Via native world serialization; implementation deferred to M-Persistence. Identity rules per §3. |
| Path β managed components | **No** | K-L3.1 Q4.b (K_L3_1 line 91; MIGRATION_PLAN line 245) | Runtime-only; "save system reconstructs on load post-G-series". Reconstruction contract is an open question (§7.3). |
| Fields: primary buffer + conductivity map + storage flags | **Yes** | FIELDS §Save/load (lines 300–301) | One blob per field: `(field_id, owning_mod_id, width, height, cell_size, blob)`. CPU-canonical content per §6. |
| Fields: back buffers | **No** | FIELDS §Save/load (line 300) | "Restored to zero on load (any in-flight ping-pong state is lost)." |
| Managed `[Deferred]` event queues | **Empty at boundary** (*proposed*) | EVENT_BUS lines 44, 128 | Today undecided. Proposal: flush before snapshot rather than serialize — the tick-end drain invariant already makes a non-empty queue a defect, and flushing is strictly cheaper than a managed-payload wire format. |
| Native Normal tier pending queue | **Empty at boundary** (*proposed*) | EVENT_BUS line 57 | Same rationale; drained at the tick-N phase boundary. |
| Background tier event queue | **Yes** | KFNS Item 31 (lines 924–933); EVENT_BUS line 59 | Versioned wire format (schema v1); untargeted persistence per S3-Q4 (queue persists for future subscribers). Event type ids per §3.4. |
| Pipeline slots / GPU state (slot ring, fences, `VkBuffer` field mirrors) | **No** | K-L16 (line 65); KFNS Item 34; VULKAN §7.2.0 | Refill on load/resume; GPU-side field mirrors lazily re-allocate on first dispatch. |
| Pending entity destructions | **Flushed pre-snapshot** | ECS line 100; §1 item 2 | The snapshot never contains a destroy-in-flight. |
| Bus subscriptions (managed + native, incl. wake registrations) | **No** | MOD_OS §9.5 (inverse); EVENT_BUS line 69 | Re-established by systems/mods during load-time registration, exactly as at boot/mod-load. |
| Active mod set | **Yes (metadata)** | MOD_OS §8.2 (line 792), §9.4 (line 916) | `(modId, modVersion)` for every active mod. §4 governs mismatches. |
| RNG streams | **Yes, once the RNG law lands** | [TIME_AND_CONSISTENCY_MODEL.md](./TIME_AND_CONSISTENCY_MODEL.md) (draft, this session) | Today no RNG service exists — only scattered seeded `Random` instances (e.g. `GameBootstrap.cs:96`). Serialized RNG state is a precondition for the §6 reproducibility class. |
| Config / map dimensions | **Yes** | FIELDS §Save/load (line 303); `SaveMeta.cs` (MapWidth/MapHeight) | "Width / height mismatch between save and current registration is a load error — fields cannot resize across sessions." |
| `ContractsVersion` + FHE fourth version field | **Yes (metadata)** | FHE D7 (lines 96–100, 161) | Persistence layer pins `ContractsVersion.Current`; FHE library version appended as fourth field, ignored when the library is absent. |

**PS-4 — Anything not listed is excluded by default.** Adding a row is an amendment to this contract, not a save-milestone implementation detail.

## §3. Identity serialization law

Persistent identity is where today's code actively contradicts the invariants the rest of the architecture relies on.

1. **PS-5 — `EntityId` generations MUST be serialized.** Today they are not:
   - `EntityEncoder.DecodeRanges` "Returns entities with version 0; callers reassign versions when re-creating live entities through their world" (`src/DualFrontier.Persistence/Compression/EntityEncoder.cs:68–85`);
   - the package README states "`Version` is lost — saves freeze a moment, and on load every restored entity gets a fresh version" (`src/DualFrontier.Persistence/README.md:75–77`).
   - This silently kills the ABA guarantee across save/load. The versioning law ([ECS.md](./ECS.md) §Versioning, line 104) and the native `is_alive` generation check make stale references safely invalid *within* a session — but any serialized state that carries an `EntityId` (background-queue event payloads per §2, component-embedded references) would revalidate against freshly-zeroed generations after load.
   - The law: entity identity in a snapshot is `(Index, Version)`, and load restores generations verbatim. The version-0 idiom is a codec-scaffold convention, not a contract.
2. **PS-6 — Component TypeIds MUST NOT be serialized as raw integers.**
   - TypeIds are "Sequential IDs (1, 2, 3, ...) deterministic per mod load order", and "Mod load order matters для type ID stability across runs" ([KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) §1.10, lines 547–549). A load-order change between save and load renumbers every type.
   - The law: the snapshot carries a stable-key table `(modId, typeName) → snapshot-local id`; load builds a remap from snapshot-local ids to the current runtime's TypeIds. Load-order independence is a correctness requirement, not an optimization.
3. **PS-7 — Field ids are already correct — follow that model.** Field registration is keyed by string id precisely because "Numeric field ids are not stable across mod reload (mod A reloaded twice could see its field assigned id 7 then id 9); string ids are" ([FIELDS.md](./FIELDS.md) line 140). This is the identity discipline rows 1–2 must converge to: stable namespaced names on the wire, runtime integers only in memory.
4. **PS-8 — Event TypeIds appear on the wire only inside the persisted background queue** (§2).
   - They are FNV-1a hashes of the event FQN ([EVENT_BUS.md](./EVENT_BUS.md) line 76; KFNS Item 31 "type_id table", Q-N-43/Q-N-44 open).
   - The law: the persisted queue carries its own FQN → type_id table; on load each id is remapped (or validated against the recomputed FNV of the FQN), and events whose type no longer resolves are dropped with a diagnostic — mirroring D-6 strip semantics rather than failing the whole load.

## §4. Missing/updated mod policy (D-6 extension)

- **Missing mod — decided; this contract keeps it.** D-6 is LOCKED: "(a) Default: strip components owned by the missing mod, keep entities; load with a clear warning naming each missing mod. (c) Available as a user-toggle 'strict mod compatibility' setting; when enabled, the save refuses to load" ([MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) §12 D-6, line 1131). The §3.2 stable-key table is what makes stripping implementable: a missing mod's components are identifiable by `modId` prefix without loading the mod.
- **Updated mod — open, but bounded here.** MOD_OS §9.4 (line 919) says only "higher major version → user warned, save may behave incorrectly"; that is a placeholder, not a policy. The open question (§7.2) is per-mod migration hooks (the mod migrates its own component/event data across its versions) versus strip-as-if-missing for incompatible majors. **PS-9 —** whatever lands MUST:
  - (a) never partially apply a mod's data — per-mod atomicity: migrate all of it or strip all of it;
  - (b) respect the strict-compatibility toggle uniformly — strict mode refuses on *any* unresolved mismatch, missing or incompatible;
  - (c) record every strip/migrate decision in a load report the user sees.
- **Lower or equal version → load proceeds** (MOD_OS §9.4, line 920). Unchanged.

## §5. Schema/version/migration policy

- **The snapshot format itself is semver-versioned.** The orphaned scaffold's single `int` (`SaveMeta.Version`, currently 2 — `src/DualFrontier.Persistence/Snapshots/SaveMeta.cs:17`) is not sufficient as a contract; it does demonstrate the need — its own doc comment records a breaking v1→v2 byte shift.
- **PS-10 — Every serialized sub-blob carries `(schema id, version)`:** the native world blob, each field blob, the background queue (already specified — "versioned wire format, schema v1", [EVENT_BUS.md](./EVENT_BUS.md) line 59), and the metadata block. Sub-blobs version independently; the top-level format version pins the set of sub-schema versions it may contain.
- **The metadata block is the version vector:** snapshot format semver + `ContractsVersion` + per-mod `(modId, modVersion)` list + the FHE fourth field (D7). Nothing else may claim version authority over a save file.
- **PS-11 — Forward-incompatible load = clean refusal with a diagnostic naming the incompatibility. Never a partial load.** Partial application is reserved exclusively for the per-mod strip path of §4 — a *policy* outcome, not a parse failure.
- **PS-12 — Migration is an explicit, tested transform chain** — vN → vN+1 steps composed in order, mirroring the governance precedent for schema amendments ([FRAMEWORK.md](../governance/FRAMEWORK.md) §7.2: explicit migration step per entry, migration runs before validation). Silent in-place reinterpretation of old bytes is forbidden. Event-schema migration across game versions is the acknowledged hard case (KFNS R-K10-6; Q-N-43/44).

## §6. Coordination — who orchestrates, and what "reproducible" means

- **PS-13 — Save is a quiesce + read transaction.** The orchestrator is the engine lifecycle owner, not the persistence layer: pause simulation, reach the §1 boundary via the K-L18 machinery, read/serialize, resume. See [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](./ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) (draft, this session) — save is the read-only instance of the same prepare/commit discipline mod-apply already exhibits. The persistence layer receives a quiesced world; it never negotiates quiescence itself.
- **Hard sync is legitimate here and only here:** "Hard sync (`waitIdle`) is available but only used for save snapshots and shutdown" ([VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) §7.3, line 1317).
- **PS-14 — Load lands in reproducibility class D1** per [TIME_AND_CONSISTENCY_MODEL.md](./TIME_AND_CONSISTENCY_MODEL.md) (draft, this session): "Save/load must produce reproducible state on load" (VULKAN §7.2.1, line 1292) — same snapshot + same mod set ⇒ identical committed state, independent of GPU/driver.
- **PS-15 — CPU kernels produce the canonical field state for saves**, because GPU compute is not bit-exact across hardware/driver combinations: "the CPU reference kernels (§7.1) produce canonical state for save snapshots — save pauses GPU dispatch, runs one CPU iteration to produce canonical field state, serializes that" (VULKAN §7.2.1, line 1295 — design mitigation, explicitly not implemented). This contract adopts it as the normative target: serialized field bytes are CPU-canonical.

## §7. Open questions

1. **Device-lost during save.** No device-lost story exists anywhere in the corpus (zero hits; not covered by VULKAN failure modes). If `waitIdle` / CPU-canonicalization races a device loss, does the save abort cleanly? Belongs jointly to this contract and the failure-taxonomy work (A8).
2. **Updated-mod migration hooks** (§4): per-mod data migration vs strip-as-if-missing; where the hook lives (manifest-declared? `IMod` surface?).
3. **Path β reconstruction contract** (§2): "save system reconstructs on load post-G-series" (MIGRATION_PLAN line 245) names no mechanism — mod-driven rebuild during load-time registration, event replay, or lazy fault-in. Must be pinned before any gameplay state of consequence lands in Path β.
4. **Background queue at saturation:** interaction of the persisted queue with the size cap and drop-oldest policy (KFNS Q-N-45; EVENT_BUS line 59 — 10 MB default cap) at the snapshot boundary.
5. **Incremental/delta saves — out of scope for this contract.** The codec README plans delta encoding (`src/DualFrontier.Persistence/README.md:60–62`); nothing here forbids it, but the §1 boundary invariant applies to every delta base and every delta alike.
6. **Orphaned codec layer disposition.** `DualFrontier.Persistence` (RLE, quantization, range encoding, string pool + snapshot records) is functional but production-orphaned — only tests reference it; its "architectural redesign… happens when save system implementation begins" ([MIGRATION_PLAN_KERNEL_TO_VANILLA.md](./MIGRATION_PLAN_KERNEL_TO_VANILLA.md) §8.5, lines 687–689). Note the §3.1 tension: `EntityEncoder`'s version-0 range encoding is exactly the compression trick the generation law breaks — the redesign must resolve that (e.g., ranges + parallel version array), not inherit it.
7. **Async/copy-on-write saves.** Baseline assumption is a blocking save inside the quiesce window (bounded by KFNS Prediction 17's <100 ms initiation target); saving off a copied snapshot while simulation resumes is a later optimization that must not weaken §1.

## Invariant index

One line per law, for citation by later documents and by the save-milestone brief.

| Id | Law (abbreviated) | Section |
|---|---|---|
| PS-1 | Snapshot at a quiesced tick boundary; saved tick = last fully-committed sim tick | §1 |
| PS-2 | Both drain-then-save and snapshot-behind must yield a fully-committed saved tick | §1 |
| PS-3 | Display tail is derived state; never serialized | §1 |
| PS-4 | Inclusion table is closed; new rows require amendment | §2 |
| PS-5 | `EntityId` generations are serialized; version-0 idiom is not a contract | §3 |
| PS-6 | No raw component TypeIds on the wire; stable `(modId, typeName)` remap table | §3 |
| PS-7 | String field ids are the identity model to converge to | §3 |
| PS-8 | Persisted background queue carries its own event-type table; unresolvable types strip with diagnostic | §3 |
| PS-9 | Mod-mismatch handling is per-mod atomic, strict-toggle-uniform, and reported | §4 |
| PS-10 | Every sub-blob carries `(schema id, version)` | §5 |
| PS-11 | Forward-incompatible load refuses cleanly; never partial load | §5 |
| PS-12 | Migration is an explicit, tested transform chain | §5 |
| PS-13 | Save is a quiesce + read transaction owned by the engine lifecycle | §6 |
| PS-14 | Load is reproducibility class D1 | §6 |
| PS-15 | Serialized field state is CPU-canonical | §6 |

## See also

- [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](./ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) (draft, this session) — the quiesce/transaction machinery PS-13 delegates to.
- [TIME_AND_CONSISTENCY_MODEL.md](./TIME_AND_CONSISTENCY_MODEL.md) (draft, this session) — tick commitment, visibility, reproducibility classes, RNG law.
- [FIELDS.md](./FIELDS.md) §Save/load, [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) §9.4/§12, [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) §7.2, [KERNEL_FULL_NATIVE_SCHEDULER.md](./KERNEL_FULL_NATIVE_SCHEDULER.md) Items 31/34 — the owning fragments this contract composes.
- `src/DualFrontier.Persistence/` — the orphaned codec scaffold (inventory for the milestone, not a design commitment).

---

*End of skeleton. Implementation design — file layout, I/O, compression selection, autosave cadence — is deliberately absent and stays with the save-system milestone.*
