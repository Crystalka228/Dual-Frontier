---
register_id: DOC-A-IDENTITY_AND_ABI_CONTRACT
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.2.1
first_authored: 2026-07-15
last_modified: 2026-08-22
content_language: en
next_review_due: 2027-Q3
title: Identity & ABI Contract — identity registry table, version-0 resolution, C ABI protocol, error taxonomy (the A5+A6+A8 contract)
review_cadence: on-change+annual
last_review_date: 2026-08-22
last_review_event: 'PR #51 review fixes (2026-08-22, architect-triaged): the ID-B versions view TOMBSTONES a destroyed-but-unflushed slot (negative sentinel; is_alive rejects the slot; flush lifts it to the successor version before free-listing) so a span reconstructing an id for the surviving component row fails closed instead of handing back the not-yet-issued pair a recycle will mint -- an ID-B-introduced fail-open, caught in review. The create guard is NARROWED to the table-growth path, the only one that can reallocate a view''s buffer, and the refusal is now an exception rather than a silent EntityId.Invalid. The mutation + view family is serialized by one mutex (acquire published its pointer before raising its counter). New F-61 ledgers the wider World-mutation concurrency gap. Section 2 option 1 and the section 3.5 pointer-table row restated to the corrected mechanism; section 1 note 1 records the tombstone as what keeps the ABA law true across the pending-destroy window; v1.2.0 -> v1.2.1 PATCH. Prior review: ID_B_ENTITY_VERSIONS -- v1.1.2 -> v1.2.0 MINOR (section 2 transitions from PROPOSAL to SHIPPED LAW, with one factual correction inside it): the version-0 resolution shipped as option 1 -- the additive df_world_acquire_versions / df_world_release_versions pair, SpanLease.Versions, true generations in all three pair-iterators, every production fabrication site migrated or waived, EntityId.IsValid and entity_id.h::is_valid aligned on Index > 0, the law seated as K-L22 AUTHORED and enforced by DFK022 at Error. Option 1''s parenthetical claiming the component-span counter already prevents the versions table resizing is CORRECTED: create_entity has never consulted active_spans_, so the shipped view carries its own active_version_views_ counter -- a component span guards CONTENT, the view guards against REALLOCATION. Section 3.5 pointer-table row filled with the shipped window; section 1 row 1 and note 1 record that fabrication no longer voids the ABA law; section 7 items 2, 3 and 10 closed, item 4''s waiver named. F-59 CLOSED. EVT-2026-08-22-ID_B_ENTITY_VERSIONS. Prior review: ID_A_COMPONENT_IDENTITY -- v1.1.1 -> v1.1.2 PATCH (section 1 row 2 to code-truth + one stale cross-reference): component TypeId is now allocated against an owner-scoped identity (owner + type FullName; owner is kernel or mod.<modId>), so the collision-policy and generation cells state cross-owner distinctness and survival-across-mod-reload rather than bare sequentiality; note 2 (ids never cross the save boundary) is unchanged and still governs. Section 2''s DFK-entity-identity pointer said K-L20 is already reserved, which went stale at EQ_A2 when K-L20 was seated as shutdown-quiescence and the Mod-API reservation moved to K-L21 -- corrected to K-L22 as the next free row (identity recon anomaly A5). EVT-2026-08-20-ID_A_COMPONENT_IDENTITY. Prior review: EQ_A4_RENDER_TAIL Cascade D -- v1.1.0 -> v1.1.1 PATCH (sections 4 + 7 aligned to landed truth): the Vulkan result mapping (section 4) + section 7 item 7 device-lost/VkResult question RESOLVED by D1 -- VK_ERROR_DEVICE_LOST is now mapped to fail-fast v1 (managed DeviceLossBoundary -> Environment.FailFast, no recovery), owned by ELT section 4 class 6 / OQ-3 (CLOSED) and VULKAN section 6.3; a MANAGED mapping, not a native df_status code. EVT-2026-07-18-EQ_A4_RENDER_TAIL. Prior review: EQ_A3_CHECKED_DESTROY Cascade C (v1.0.0 -> v1.1.0 MINOR).'
reviewer: Crystalka
special_case_rationale: 'Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-DRAFTS_RATIFICATION (item [6]). The A5+A6+A8 identity/ABI contract — identity registry (12 id spaces with per-row law), the version-0 resolution (highest-value single fix; ECS §5 defers here), C ABI protocol (negotiation, type/ownership/no-exception laws, pointer windows, thread affinity), df_status error taxonomy; the identity-versions surface and DFK-entity-identity rule are the seeded engineering work orders.'
---

# Identity and ABI Contract (the A5+A6 contract)

> **Document class: RATIFIED — normative law in force (EVT-2026-07-17-DRAFTS_RATIFICATION).** The A5+A6+A8 identity/ABI contract, produced by the Architecture Decomposition & Contracts session 2026-07-15 ([docs/reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)). It closes gaps A5 (identity contract) and A6 (C ABI protocol) and supplies the error-taxonomy portion of A8; documents may now cite it as authority. Baseline anchors authored at HEAD `6f39903`; code re-verified unchanged at `48983c4`, doc anchors retargeted to the LOCKED v1.0.0 successors. **Carve-out:** the named LOCKED texts govern their specific points until the §7 forward amendments land; §7 items 1 & 2 were resolved in place by the corpus rework and are recorded closed.

**Forward amendment queue** (recorded in docs/ROADMAP.md; folds NOT executed at ratification):

| This document's section | Deferred destination |
|---|---|
| §1 identity registry table | [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — new part ("Part 9: Identity registry"), plus per-doc amendments ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) capability-grammar `layer.*` half; the VULKAN/ECS halves landed with the rework) |
| §2 version-0 resolution | [ECS](./ECS.md) + [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) §1.7 versions-view examples; `EntityId.cs` / `entity_id.h` code change; new DFK-entity-identity rule in [ANALYZER_RULES](./ANALYZER_RULES.md) — the seeded engineering work order |
| §3 C ABI protocol + §4 error contract + §5 compatibility | [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) §5 expansion (the successor already freezes 1/0 and forward-references §3–§4 here) |
| §6 enforcement | [ANALYZER_RULES](./ANALYZER_RULES.md) registry additions + `df_native_selftest` extension |

Baseline for every "today" claim: working tree at HEAD `6f39903`, 2026-07-15; code anchors re-verified unchanged at ratification HEAD `48983c4` (EVT-2026-07-17-DRAFTS_RATIFICATION); doc anchors retargeted to the LOCKED v1.0.0 successors per CODING_STANDARDS §6.1 form.

---

## 0. Why this document exists

Identity rules are scattered across at least six documents and two languages, with live drift between them. Three verified examples:

- **The entire managed hot path fabricates entity versions.** Nine production systems (≈20 sites) plus `GameBootstrap.cs:241` and `src/DualFrontier.Persistence/Compression/EntityEncoder.cs:85` construct `new EntityId(indices[i], 0)`, and `SpanLease.Pairs` fabricates `Version = 1` (`src/DualFrontier.Core.Interop/SpanLease.cs:112`; sibling fabrication `WriteBatch.cs:221`) — while native `is_alive` demands `id.version == versions_[id.index]` (`native/DualFrontier.Core.Native/src/world.cpp:74-78`). The generation/ABA machinery exists natively and is never exercised from managed code. The pre-rework teaching texts *instructed* the defect; the successors now teach the corrected form and defer the fix here (§2) — the CODE defect persists.
- **Identity drift inside one LOCKED document — resolved by the rework.** The predecessor VULKAN §3.4 sketched `uint32_t field_id` while its own shipped-shape subsection and the real ABI use string ids (`df_capi.h:454-523`); the VULKAN successor §4.3 (field ABI — string-id corrected) records the correction, FIELDS §6 agrees. Retired sketch: historical/VULKAN_SUBSTRATE.md.
- **The C ABI has no protocol.** No version negotiation, no struct-layout/alignment law, no buffer-ownership rules, no error taxonomy (retryable vs fatal vs contract-violation), no thread-affinity declaration per entry point, no pointer validity windows. The only existing ABI law is the status-code + `catch (...)` convention (KERNEL_ARCHITECTURE §5 (C ABI conventions)) and the four-category managed rule (same §5) — real, but thin (session verdict A6: "CONFIRMED missing (thin)").

An engine whose kernel is a foreign-function boundary lives or dies by exactly two contracts: *what a value means* (identity) and *how it crosses* (ABI). This document is both, in one place, because they constrain each other — every identity in §1 has a wire form, and every ABI rule in §3 exists to keep those wire forms honest.

## 1. Identity registry

One table, every identity the engine mints. Column definitions: **allocation** — who mints the value and how; **collision policy** — what happens when two owners want the same value; **generation / reuse** — whether a value can be reissued and how staleness is detected; **registration lifetime** — how long a registered value stays meaningful. Terse cells; the numbered notes below the table carry the law.

| # | Identity | Canonical representation | Namespace owner | Allocation | Collision policy | Generation / reuse | Serialization form | Invalid / sentinel | Registration lifetime |
|---|---|---|---|---|---|---|---|---|---|
| 1 | EntityId | `record struct (int Index, int Version)` (`EntityId.cs:21`); packed `uint64` at ABI: hi 32 = Version, lo 32 = Index (`entity_id.h:19-23`, `df_capi.h:23-27`) | kernel world | native `create_entity`: free-list recycle or `next_index_++` (`world.cpp:57-72`) | impossible (single allocator) | Version bumps on destroy (`world.cpp`); slot reused with higher version; ABA law, note 1; the current generation of any slot is readable through the ID_B versions view (§2) | two `int32` fields, fixed — changing field types breaks Save/Load (`EntityId.cs:17-19`) | `Invalid = default` (0,0) (`EntityId.cs:28`); index 0 permanently dead (`world.cpp:75`) | until destroy + flush |
| 2 | Component TypeId | `uint32`, keyed on `ComponentIdentity` = `(owner, type FullName)`; owner is `kernel` or `mod.<modId>` (`ComponentTypeRegistry.cs:25`) | owner-scoped, per world | managed `ComponentTypeRegistry`, sequential from 1, idempotent per **identity** — vanilla at bootstrap, a mod's Path α types eagerly at Apply (KERNEL_ARCHITECTURE §3 (component type-id registry)) | none possible (sequential); two owners sharing a type FullName are two identities, so cross-mod collision is structurally impossible; re-register same id with different size rejected (`df_capi.h:139-141`) | identity rows SURVIVE mod unload, so a reloaded mod re-adopts its id and its store; stable within a run only; **not stable across load-order change**, note 2 | MUST NOT persist raw; save writes string→id map, note 2 | 0 reserved invalid (KERNEL_ARCHITECTURE §3, `df_capi.h:141`) | world lifetime |
| 3 | Event TypeId | `uint32` = FNV-1a-32 of CLR FQN (EVENT_BUS §4 (BusFacade FNV-1a mint); `BusFacade.cs:57-62,176-182`) | global (derived from FQN) | derived, not allocated | **TODAY ABSENT**, note 3 | none; stable across runs because FQN-derived | re-derivable; persist FQN, never the hash | 0 by convention (FNV output 0 treated as failure, cf. `ModUnloadInterop.cs:88`) | process-global registry until `clear` (test-only) |
| 4 | Field id | string `<mod-namespace>.<field-name>` (FIELDS §4 (identity and namespacing)) | registering mod's manifest id (MOD_OS reserved-namespace rule) | mod authors the name under its namespace | second registration → `FieldRegistrationConflict` (FIELDS §4) | stable across mod reload **because** string, note 4 | the string; save records `(field_id, owning_mod_id, dims, blob)` (FIELDS §12 (save/load)) | null/empty rejected | until `Unregister` / mod unload |
| 5 | ModId | reverse-domain string, globally unique (MOD_OS_ARCHITECTURE §2.2 (manifest fields)) | mod author | authored in manifest | duplicate ids rejected at batch validation | n/a | the string; save pairs it with modVersion (MOD_OS D-6) | empty rejected at parse (§2.3 rule 2) | mod load session |
| 6 | ModId, native bus form | `uint32` = FNV-1a-32 of ModId string (`ModUnloadInterop.cs:85-101`) | derived | derived | **ABSENT**, note 3 | n/a | never persisted | 0 = Core/vanilla (EVENT_BUS §3) or null input | mod load session |
| 7 | Subscription id | `uint64`: high 8 bits = tier, low 56 = per-tier sequence (EVENT_BUS §3 (subscription-id layout); К-L15.1, KERNEL_ARCHITECTURE Part 0) | native bus, per tier | per-tier `next_seq` counter | cross-tier collision structurally impossible (tier bits) | monotonic; never reused within a process | never persisted | none defined — propose 0, note 5 | until unsubscribe / per-mod bulk unsub / `df_bus_clear` |
| 8 | InternedString handle | `struct (uint Id, uint Generation)` (`InternedString.cs:58-72`) | world string pool | native intern within mod-scoped windows (`df_capi.h:244-266`) | same content interns to same id (co-ownership recorded) | generation bumps on `clear_mod_scope`; stale `{id, gen}` resolves not-found (`df_capi.h:262-266`) | **serialize CONTENT, never ids** — LOCKED (`df_capi.h:267-271`) | `Id == 0` empty sentinel (`InternedString.cs:60-64`) | until owning mod scope cleared |
| 9 | Capability token | string `<provider>.<verb>:<target>` (MOD_OS_ARCHITECTURE §2.3 (validation — the authoritative regex) / §3 (capability model); field grammar unified at FIELDS §8) | provider (`kernel` or `mod.<modId>`) | authored in manifest / emitted by `KernelCapabilityRegistry` | exact-match set; wildcards forbidden (MOD_OS_ARCHITECTURE §3) | n/a | the string | non-matching token rejected at parse | mod load session; revoked at unload step T4 (`mod_unload.h:12`) |
| 10 | System id | `uint32` at ABI (`df_capi.h:593`); string `SystemId` in `SystemRegistration` (KERNEL_ARCHITECTURE §1.10) | kernel scheduler | **TODAY: array index from 0** (`GameBootstrap.cs:162-176`) | none — caller-supplied, unchecked | none | string form (`"vanilla.pawn.needs_system"`) is the stable name; the uint32 is per-run | **none — 0 is a live system today**, note 6 | until unregister / `df_scheduler_clear` |
| 11 | Wake id (Explicit wake type) | `uint32` (`df_capi.h:652`) | undeclared | caller-chosen | **ABSENT** — two systems can claim one wake id | none | never persisted | none defined | until unsubscribe |
| 12 | Pipeline id | `uint32`, native-allocated monotonically from 1 (`df_capi.h:525-548,559-565`) | kernel compute registry | native returns id at registration | duplicate pipeline *name* rejected (`df_capi.h:539`) | none within a process | persist the pipeline name, never the id | 0 = failure/invalid | world lifetime (mod-unload cleanup is a placeholder, VULKAN §3.4.1) |

### Notes (the actual law per row)

**Note 1 — EntityId ABA law (normative).** A given `(Index, Version)` pair is issued at most once per world lifetime: `destroy_entity` increments `versions_[index]` before the slot returns to the free list (`world.cpp:84-93`), and `flush_destroyed` recycles only the index (`world.cpp:95-108`), so a recycled slot always carries a strictly higher version. Any cached id therefore resolves dead forever after its entity is destroyed — across ticks, saves, deferred events, and mod boundaries. This is the engine's only structural defense against stale-reference corruption. Note that the law binds OBSERVABILITY, not just issuance: an id that can be READ before its entity exists breaks it exactly as badly as one reissued after the entity dies — which is why a destroyed-but-unflushed slot is TOMBSTONED rather than pre-incremented (§2, PR #51 review R1). It was nullified managed-side by version fabrication until ID_B; the pack/unpack functions always preserved Version honestly (`entity_id.h`), and the loss happened purely in managed construction, which §2's versions view and DFK022 now prevent.

**Note 2 — Component TypeId save implication (proposal).** Sequential ids are "deterministic per mod load order" and "mod load order matters" (KERNEL_ARCHITECTURE §1.10). Consequence: adding, removing, or renaming a mod between save and load silently reassigns every subsequent numeric id, and any persisted raw id decodes as the wrong component type — a corruption class with no error signal. Proposed law: **numeric component ids never cross the save boundary.** The save header carries a `FQN string → uint32` map captured at save time; the loader translates old ids to current ids at load, and a missing FQN is a typed load error, not a guess. Precedent: the string pool's serialize-content-not-ids rule, already LOCKED (`df_capi.h:267-271`). Owner for the save-format half: the A7 draft ([PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md)).

**Note 3 — Event TypeId / native ModId collision policy (proposal).** FNV-1a-32 collisions over FQN space are improbable but not impossible, and today they are *silent*: `df_event_type_registry_register` returns 0 for "already registered" (`event_type_registry.h:64-66`), so a colliding second type simply fails registration and its events dispatch under the first type's tier, payload size, and subscribers. Proposed law: (a) registration-time collision detection — the registry compares the stored FQN against the incoming FQN whenever the id already exists; same FQN = idempotent success, different FQN = hard contract-violation status (`DF_ERR_ID_COLLISION`, §4) that aborts the registering mod's load; (b) id 0 stays reserved; (c) a reserved range (proposal: `0x00000001–0x000000FF`) is carved out for kernel-synthetic event types so a mod FQN hash can never shadow them. The same FQN-comparison rule applies to the native `mod_id` hash (`ModUnloadInterop.cs:85-101`), where a collision today would merge two mods' bulk-unsubscribe identity — one mod's unload would strip another mod's subscriptions.

**Note 4 — Field id (the uint32 drift, killed).** String identity is the *rationale-carrying* choice: "Numeric field ids are not stable across mod reload… string ids are" (FIELDS §4 (identity and namespacing)). The shipped ABI agrees (`const char* field_id` throughout, `df_capi.h:454-523`). The predecessor VULKAN §3.4 `uint32_t field_id` sketch was corrected by the successor (VULKAN §4.3, string-id ABI; §7 item 1, closed). No numeric alias for fields is ever introduced; if profiling someday demands one, it must be a per-run lookup handle acquired from the string, never a persisted identity.

**Note 5 — Subscription id sentinel.** The bit layout leaves value 0 (tier 0, sequence 0) reachable in principle for the Fast tier's first subscription. Proposed: per-tier sequences start at 1, making the all-zero `uint64` a true never-issued sentinel. Cheap now, impossible after subscriptions are ever persisted or exposed to mods.

**Note 6 — System id (proposal).** Align with the component-id convention: 0 reserved invalid, ids allocated sequentially from 1 by a registry keyed on the stable string `SystemId`/FQN (the ABI already transports the FQN at registration, `df_capi.h:593-595`). Today's allocation is the `coreSystems` array index starting at 0 (`GameBootstrap.cs:162-179`) — it uses the proposed sentinel value for a live system, and reordering a source-file array silently renumbers every system id the native graph, wake registry, policies, and trace events refer to.

## 2. The version-0 resolution — SHIPPED (ID_B_ENTITY_VERSIONS, 2026-08-22)

**Status.** Option 1 below shipped as specified. The additive native pair `df_world_acquire_versions` / `df_world_release_versions` exposes the per-slot `versions_` table read-only; `SpanLease<T>.Versions` carries it managed-side; the three pair-iterators reconstruct true generations; every production fabrication site is migrated or waived; `EntityId.IsValid` and `entity_id.h::is_valid` are aligned; the law is seated as К-L22 (AUTHORED, [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) Part 0) and enforced by DFK022 at Error. ROADMAP F-59 is CLOSED. The option history below is retained deliberately — the rejected alternatives are why the shipped shape is the shape it is.

**The defect, as it stood.** The pre-rework `KERNEL_ARCHITECTURE.md` §1.7 *taught* the pattern — `EntityId entity = new EntityId(lease.Indices[i], 0); // version not exposed via span` — and the pre-rework ECS canonical example repeated it, in the same file whose versioning law declares the version "the indicator of a dead reference" (now ECS §6 (versioning law)); both teaching sites are retired (historical/KERNEL_ARCHITECTURE.md, historical/ECS.md) and the successors carry the corrected non-fabricating form (ECS §4/§5 (fabricated-version defect), deferring the identity fix to this §2). Production followed the old teaching: ≈20 sites across nine systems fabricate version 0 (`HaulSystem.cs:54,110,144`, `NeedsSystem.cs:93`, `JobSystem.cs:100`, `ConsumeSystem.cs:57,132,152,177`, `MovementSystem.cs:108`, `MoodSystem.cs:40`, `SleepSystem.cs:57,101,185`, `PawnStateReporterSystem.cs:63`, `ComfortAuraSystem.cs:61,69`) plus `GameBootstrap.cs:241`; `EntityEncoder.cs:85` decodes saved index ranges to version 0, and `SpanLease.Pairs` fabricates `Version = 1` with an honest caveat comment (`SpanLease.cs:76-84,112`; sibling `WriteBatch.cs:221`). Because these fabricated ids flow into `TryGetComponent` lookups and batch records, generation validation collapses to "the index was never recycled" (session verdicts C10/N29). The window is real: `DestroyEntity` → `flush_destroyed` → free-list recycle can complete within the same tick sequence a stale fabricated id is still circulating in.

**The law.** Span and batch ABI surfaces MUST surface true versions; managed code MUST NOT construct an `EntityId` whose Version it did not receive from the world. Three candidate mechanisms:

1. **Parallel versions view (RECOMMENDED — SHIPPED).** One additive entry point pair: `df_world_acquire_versions(world, const int32_t** out_versions_ptr, int32_t* out_count)` / `df_world_release_versions(world)`, exposing a read-only view of the native `versions_` table under the same acquire/release discipline as component spans (`df_capi.h`, the span family). `SpanLease<T>` acquires it alongside the component span; `Pairs` and every hot loop reconstruct `new EntityId(idx, versions[idx])`. No shipped signature changes, zero copies, one extra P/Invoke per lease. `SpanLease.cs` itself already named this option ("extending the span ABI to return parallel version arrays — deferred to K7"); the deferral ended here.

   **Correction, made at implementation (ID_B fact 2).** This entry previously claimed the view could ride the component-span counter, "the mutation-rejection counter already guarantees the table cannot resize while any span is active". Measured against `world.cpp`, that is **false**: `create_entity` has never consulted `active_spans_` — creation under a live component span is legal and selftest-asserted — and it is precisely `create_entity` that grows `versions_`, reallocating the buffer the view points into. Reusing the span counter would therefore have required changing shipped span semantics, which §3.1 forbids. The shipped view carries its **own** counter, `active_version_views_`, which refuses a table-GROWING `create_entity` (reallocation — creating from the free list or into spare capacity writes nothing to the table and is permitted), `destroy_entity` (it rewrites a generation the reader is looking at) and `flush_destroyed` (it makes slots recyclable underneath the reader) for the view's lifetime, REFUSE-NOT-FORCE per the EQ_A3 precedent, with the managed wrapper raising the refusal as an exception rather than returning a silent `EntityId.Invalid`. Creation under a plain component span remains legal, unchanged. The distinction is the load-bearing one: a component span guards table CONTENT, the versions view guards against REALLOCATION. The whole family — create, destroy, flush, acquire, release — is serialized by one mutex, because publishing the view pointer and raising its counter must not be a check-then-act pair against a concurrent create (PR #51 review R3).
2. **Extended span acquire.** Add a versions out-parameter to `df_world_acquire_span`. Rejected: mutates a shipped signature (ABI-breaking per §5, so it would have to ship as `df_world_acquire_span_v2`), and a dense-parallel version array requires a per-acquire copy that option 1 avoids.
3. **Index-only `EntityRef`.** A distinct `readonly record struct EntityRef(int Index)` for span-scoped iteration, convertible to `EntityId` only through the world (which fills in the true version). Fabrication becomes *unrepresentable* — the strongest shape — but it ripples through every `EntityId`-accepting API and all 13+ call sites simultaneously. Retained as the structural endgame if the analyzer rule below proves leaky; not the first step.

**The landed mechanism** (signatures as shipped; the normative targets were adopted verbatim):

```c
/* df_capi.h — same acquire/release discipline as component spans.
 * Window: pointer valid until df_world_release_versions. Create/destroy/flush
 * are refused while held, via the view's OWN active_version_views_ counter —
 * NOT the span counter (see the correction under option 1). Read-only view
 * over the per-slot versions_ table, indexed by ENTITY INDEX, not dense
 * position; out_count is the table size, not the entity count. */
DF_API int32_t df_world_acquire_versions(
    df_world_handle world,
    const int32_t** out_versions_ptr,
    int32_t*        out_count);

DF_API void     df_world_release_versions(df_world_handle world);
```

```csharp
// SpanLease<T>.Pairs — the shipped Current (SpanLease.cs). The WriteBatch<T>
// enumerator and Sdk/SpanScope<T>.PairsEnumerator carry the same reconstruction.
public (EntityId Entity, T Component) Current
{
    get
    {
        int entityIndex = _lease.Indices[_index];
        return (new EntityId(entityIndex, _lease.Versions[entityIndex]),
                _lease.Span[_index]);
    }
}

// EntityId.IsValid — as shipped (EntityId.cs); entity_id.h::is_valid mirrors it:
public bool IsValid => Index > 0;
```

**Consequential amendments (the ratification cascade — ALL LANDED at ID_B):**

- The ECS span example (ECS.md §4) and the KERNEL_ARCHITECTURE identity example (§2) are rewritten to the versions-view idiom, with the entity-index-vs-dense-position trap called out inline. The earlier half of this item — removing the version-0 teaching and the stale "Access is permitted" comment (session verdicts C10/N19) — had already landed at the rework.
- `EntityId(index, 0)` construction is analyzer-detectable: rule **DFK022** (the working name was DFK-entity-identity; the shipped id is 1:1 with К-L22 per ANALYZER_RULES §2). It flags any `new EntityId(<expr>, <integer literal>)` — any literal, not only 0 — outside `DualFrontier.Core.Interop` internals and test namespaces, Error severity, NativeBoundary category, same class as DFK001/DFK002. Detection is by parameter NAME, so a named out-of-order `Version:` argument is caught and a literal `Index` with a real version is not. `EntityId.Invalid` and `default` are unreachable by the rule, being no constructor call.
- `EntityEncoder` (persistence) carries the census-pinned waiver, `DFK-WAIVER(DFK022)` around its `DecodeRanges` loop, until the A7 contract decides how versions are persisted (§7 item 4) — A7 is named in the waiver comment as its retirement trigger. The waiver census moved 2 → 3 (ANALYZER_RULES §1.4; TESTING_STRATEGY §4.3), the single HARD-pin movement the cascade ratified.

**`IsValid` alignment — SHIPPED.** `EntityId.IsValid => Index > 0 || Version > 0` disagreed with native `is_alive`, which rejects `index <= 0` unconditionally (`world.cpp`): the id `(0, 5)` was "valid" managed-side and permanently dead native-side (verdict N38). Current-generation knowledge is unknowable managed-side without a world call, so `IsValid` stays a *syntactic* check by design — but it must be the syntactic projection of the native rule, and now is: **`IsValid => Index > 0`**. Aliveness remains answerable only by the world (`World.IsAlive`, comparing versions), exactly as the doc comment on the member says. The native mirror `entity_id.h` carried the same flawed disjunction (`index > 0 || version > 0`) and was fixed in the same cascade. The "safe by inspection" claim was **verified rather than assumed** at ID_B Phase 0: `src/` and `mods/` contain ZERO `EntityId.IsValid` consumers (every other `IsValid` in the tree belongs to `NativeMap` / `NativeSet` / `NativeComposite` / `CompositeHandle` / `ValidationReport`), and native `is_valid` had zero call sites — the definition only. The one place that depended on the corner was a TEST that asserted it, `EntityIdTests.Zero_index_nonzero_version_is_valid`, whose comment claimed "Index=0 is allowed for real entities"; the world has never allowed it (`next_index_ = 1`), so the test encoded the defect and was inverted.

## 3. C ABI protocol

Scope: every `extern "C"` entry point exported by `DualFrontier.Core.Native` (`df_capi.h`, `bus_native.h`, `event_type_registry.h`, `mod_unload.h`, and successors). Shipped conventions cited below stay LOCKED; everything marked *proposed* is normative-target only.

### 3.1 Versioning and negotiation (proposed — absent today)

- New entry point `int32_t df_abi_version(void)` returning `(MAJOR << 16) | MINOR`. The managed bootstrap MUST call it before any other ABI call (before `df_engine_bootstrap`) and refuse to run on MAJOR mismatch — fail-fast with a user-facing diagnostic, mirroring the K-L19 hardware-gate precedent.
- Evolution rules (semver-ish): **adding** an entry point bumps MINOR. **Changing** the signature or observable semantics of a shipped entry point is forbidden — ship a `_v2` sibling and deprecate the old one in the header comment. **Removing** an entry point, changing a struct layout, or renumbering an enum bumps MAJOR.
- Why negotiation matters here specifically: the native binary is not committed and there is no CI (session verdict N13), so managed/native skew is a live deployment risk today. The existing P/Invoke marshalling pre-flight check (KERNEL_ARCHITECTURE §(methodology adjustments — P/Invoke pre-flight)) catches skew at review time; `df_abi_version` catches it at run time.

### 3.2 Type law (partially shipped, made explicit)

- **Blittable only.** Pointers + fixed-width primitives. Already the batched-callback constraint ("All args blittable (pointer + primitives only)", `historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 15 constraints) and the component-payload assumption (`df_capi.h:29-35`). No generics across the boundary (same constraints list).
- **Layout.** Every ABI struct uses `stdint.h` fixed-width fields, `LayoutKind.Sequential` on the managed mirror, natural alignment, no packing pragmas, no bitfields. Existing structs already comply (`df_trace_event` `df_capi.h:757-763`; `df_managed_system_batch` `:814-819`; `ModUnloadResult` `mod_unload.h:48+`); the law converts custom into mandatory.
- **Endianness.** Little-endian assumed and now stated. The entity packing (`entity_id.h:19-23`) is defined in value space (shifts and masks), not byte space, so it is safe wherever both sides agree on integer representation — which the LE statement pins.
- **No `bool` at the boundary.** Truthiness travels as `int32_t` 0/1 — already universal practice (`df_world_is_alive`, every `*_enabled` accessor). Marshaling C# `bool` is ambiguous (1 vs 4 bytes depending on context) and is forbidden.

### 3.3 Ownership law (per-call rules; the ownership *tree* lives in RESOURCE_OWNERSHIP_AND_LIFETIME.md, the A2 contract)

- **Allocation symmetry.** Native never frees managed memory; managed never frees native memory. Handle ownership follows the documented owner per handle class: world handles — caller, via `df_world_destroy`; batch handles — caller, via `df_batch_destroy` (destroy auto-flushes, `df_capi.h:196-198`); K8.1 primitive handles — the World, "Caller never frees" (`df_capi.h:254-258`). Cross-cutting dispose order is [RESOURCE_OWNERSHIP_AND_LIFETIME](./RESOURCE_OWNERSHIP_AND_LIFETIME.md)'s jurisdiction.
- **Out-buffers are caller-allocated.** The iterate/snapshot family takes caller buffers plus capacity and returns count written, clipped (`df_keyed_map_iterate` `df_capi.h:272-276`; `df_world_resolve_string` `:290-295`; `df_scheduler_trace_dump`, `df_scheduler_snapshot` `:778`). New entry points MUST follow this pattern; a native-allocated return requires a paired `df_*_free` and explicit rationale in the header comment.
- **String lifetime = call duration unless documented.** Default: `const char*` parameters are valid only for the duration of the call; native copies what it keeps. One grandfathered documented exception exists: `df_event_type_registry_register` stores the FQN pointer without copying — "owned by caller; lifetime ≥ registry" (`event_type_registry.h:42`, "stored by pointer" `:77`) — and the managed side satisfies it with a deliberately never-freed `Marshal.StringToHGlobalAnsi` allocation (`EventTypeRegistryInterop.cs:48-62`). Proposed: migrate this entry point to copy-on-register at the next MAJOR bump; store-by-pointer is a standing footgun for any caller that has not read that one comment.

### 3.4 No-exception law, both directions (shipped — cited)

- **Native → managed:** every `extern "C"` function returns a status code or sentinel and swallows all exceptions via `catch (...)` at the boundary; "non-negotiable for cross-DLL safety" (KERNEL_ARCHITECTURE §5 (C ABI conventions) — the successor itself records 72 `catch (...)` sites in `capi.cpp`; recount at `48983c4` = 72, exact).
- **Managed → native (reverse P/Invoke):** callbacks are `static`, `[UnmanagedCallersOnly]`, blittable-args-only, and absorb every managed exception before returning (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 15 — the `FaultLog.RecordKernelBatchFault` absorb pattern + constraints list).
- **GC transitions:** `SuppressGCTransition` is forbidden for reverse P/Invoke (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 15 constraints). For forward P/Invoke it is currently used nowhere (`NativeMethods.cs` is plain `[DllImport(..., CallingConvention.Cdecl)]`, `:23-25`); any future use requires per-entry-point proof of non-blocking, non-allocating, sub-microsecond behavior, recorded in the header comment.

### 3.5 Pointer validity windows (proposed law: declared per entry point)

Every entry point that returns or accepts a pointer MUST document its validity window in the header. Normative table for shipped surfaces:

| Pointer | Valid from | Valid until | Anchor |
|---|---|---|---|
| component span (`out_dense_ptr`, `out_indices_ptr`) | `df_world_acquire_span` returns | `df_world_release_span` for that type; mutations rejected meanwhile | `df_capi.h:88-99` |
| field span (`out_data`) | `df_world_field_acquire_span` | `df_world_field_release_span` | `df_capi.h:443-448` |
| versions view (§2, shipped ID_B) | `df_world_acquire_versions` returns | `df_world_release_versions`; table-growing create, plus destroy/flush, refused meanwhile via `active_version_views_` under the mutation mutex | `df_capi.h` (versions-view block, after the span family) |
| batch handle | `df_world_begin_batch` | `df_batch_destroy`; recorded pointers are copied at record time, not at flush | `df_capi.h:186-214` |
| K8.1 primitive handles (map/composite/set) | `df_world_get_*` | owning `df_world_destroy` | `df_capi.h:254-258` |
| `df_shm_map` pointer | map returns | `df_shm_unmap` / `df_shm_destroy` | `df_capi.h:836-840` |
| `out_fqn` from event-registry lookup | lookup returns | next registry mutation (points into stored registration data) | `event_type_registry.h:80-89` |
| caller out-buffers | owned by caller | filled during the call only; native keeps no reference | §3.3 |

The general rule the table instantiates: **a returned pointer is valid until the entry point's documented release call, and never survives the owner it points into.** "Returned strings valid until the next call on the same handle" applies to any future accessor that returns native-owned string data.

### 3.6 Thread affinity (proposed law: declared per entry point)

Every entry point declares exactly one of: **main-thread-only** | **sim-thread-only** | **any-thread + external-sync** | **internally-synchronized**. No such declaration exists anywhere today (session verdict A6). Starter classification, to be audited entry-by-entry during the ratification milestone:

| Surface group | Declaration | Basis |
|---|---|---|
| world entity/component mutation, span acquire/release, batches, fields | sim-thread-only | the world's only internal synchronization is the atomic span/batch counters (`world.cpp:85-87`); everything else is unguarded |
| scheduler graph / wake registry / policies / trace / intrinsics / shm / state filter (process-global singletons) | internally-synchronized | per-singleton mutexes; the unload primitive takes the scheduler critical section (T0, `mod_unload.h:7`) |
| bus publish/subscribe/drain | internally-synchronized | per-tier mutexes with fixed fast→normal→background acquisition order (EVENT_BUS §3 (К-L15.1 lock order)) |
| `df_world_attach_vulkan`, compute pipeline registration | main-thread-only | pending the render-thread contract; Vulkan handle ownership per VULKAN §3.4 |
| `df_engine_bootstrap`, `df_world_create/destroy` | any-thread + external-sync | caller serializes creation/teardown; concurrent engines are isolated (`df_capi.h:174-177`) |

The declaration lands as a tag line in each `df_capi.h` block comment and a mirrored attribute on `NativeMethods` members so the analyzer (§6) can consume it.

## 4. Error contract (the A8 taxonomy portion)

**Today.** Three regimes coexist: boolean `1/0` with sentinel returns ("0 — failure / not found; 1 — success / present. Out-of-range inputs return 0 rather than crashing", `df_capi.h:37-40`); ad-hoc negative codes (`df_batch_flush` returns −1 on "logic error", `df_capi.h:212`; scheduler graph returns −1 write-write conflict / −2 cycle, `:588-591`); and fixed-size out-struct error payloads (`error_messages[8][256]` + `error_count` in `ModUnloadResult`, `mod_unload.h:41-49`, mirrored in `VulkanModUnloadResult`, VULKAN §3.4.1). There is no `last_error`, no retryability classification, and no device-lost story anywhere in the corpus (session verdict A8).

**Proposed status space (`df_status`) — new and `_v2` entry points only.** The KERNEL_ARCHITECTURE §5 convention is explicitly labeled immutable, so shipped `1/0` entry points are grandfathered unchanged; the ratified §5 expansion must state both regimes side by side (§7 item 6 — the successor §5 already freezes 1/0 and forward-references this contract's §3–§4). New entry points return `int32_t df_status`:

- **`0` — success** (`DF_OK`). Counts and sizes travel through out-parameters, never overloaded onto the status.
- **Negative — contract violation (caller bug):** null/invalid handle, id 0 where forbidden, size mismatch, mutation during active span/batch, quiescence precondition violated, id collision (§1 note 3). DEBUG builds assert/crash at the violation site (fail-fast, K-L19 spirit); RELEASE builds return the structured code. A negative status is never retried — it means the program is wrong, not that the world is busy.
- **Positive — runtime condition**, classed by range:
  - `1–99` **retryable**: not found, already present, buffer too small (count needed reported via out-param), queue saturated, quiescence pending. The caller may branch, resize, or retry.
  - `100–199` **fatal-subsystem**: the subsystem is unusable but the process continues — compute pipeline creation failure, device-lost class. Managed side surfaces a typed exception and the fault handler decides quarantine.
  - `200–299` **fatal-process**: an invariant is torn — scheduler panic (`df_scheduler_panic_halt`, `df_capi.h:776`), allocation exhaustion. Managed side must fail fast.

**Constant sketch** (names normative-target; exact values assigned at the amendment milestone):

```c
typedef int32_t df_status;

#define DF_OK                        0
/* negative — contract violations (caller bug; DEBUG asserts, RELEASE returns) */
#define DF_ERR_INVALID_HANDLE       -1
#define DF_ERR_INVALID_ID           -2   /* id 0 / unregistered where forbidden */
#define DF_ERR_SIZE_MISMATCH        -3
#define DF_ERR_MUTATION_LOCKED      -4   /* active span or batch */
#define DF_ERR_NOT_QUIESCENT        -5   /* К-L18 precondition violated */
#define DF_ERR_ID_COLLISION         -6   /* §1 note 3: FQN mismatch on existing id */
/* positive 1-99 — retryable runtime conditions */
#define DF_COND_NOT_FOUND            1
#define DF_COND_ALREADY_PRESENT      2
#define DF_COND_BUFFER_TOO_SMALL     3   /* required size via out-param */
#define DF_COND_SATURATED            4
#define DF_COND_PENDING              5   /* quiescence / fence not yet reached */
#define DF_COND_WORLD_BUSY           6   /* EQ_A3: live spans/batches on checked destroy */
/* positive 100-199 — fatal-subsystem; 200-299 — fatal-process */
#define DF_FAIL_SUBSYSTEM_BASE     100
#define DF_FAIL_PROCESS_BASE       200
```

**Realized (EQ_A3_CHECKED_DESTROY, 2026-07-18).** The checked-destroy pair `df_world_active_span_count` / `df_world_destroy_checked` (`native/DualFrontier.Core.Native/src/capi.cpp`, `include/df_capi.h`) are the first `df_status` entry points on disk. EQ_A3 materialized exactly three constants of the sketch above — `DF_OK`, `DF_ERR_INVALID_HANDLE`, and the new `DF_COND_WORLD_BUSY` (6) — as a conforming subset; the remaining constants stay proposed (F-43 owns the full space + `df_last_error` + affinity/version negotiation). The pair follows the section 3.3 count-via-out-param rule and the section 3.4 no-exception law (the boundary `catch(...)` returns a NEGATIVE status, never 0 = DF_OK). Two new entry points = MINOR (section 5). Commits: C2 `7bc4e07`, C5 `31dfb26`.

**Diagnostic retrieval (proposed).** `int32_t df_last_error(char* out_buffer, int32_t capacity)` — thread-local storage, records the human-readable message of the calling thread's most recent non-OK status, caller-allocated buffer per §3.3, never load-bearing for control flow. This supersedes the fixed `error_messages[8][256]` pattern for new surfaces (which silently drops messages beyond 8, `mod_unload.h:43-47`); the shipped unload structs keep their shape — they are frozen ABI.

**Mapping to managed exceptions.** The four-category rule (KERNEL_ARCHITECTURE §5 (managed projection categories)) already decides throw-vs-return per abstraction shape; the status classes bind onto it:

| Status class | Managed projection |
|---|---|
| retryable (1–99) | `bool` / `TryX` return — the sparse category; never an exception |
| contract violation (negative) | throw — `ObjectDisposedException` / `InvalidOperationException` per the lifecycle and construction categories; dense category throws range exceptions |
| fatal-subsystem (100–199) | typed exception (`HardwareCapabilityException` precedent, VULKAN §7.1) routed to the mod/system fault handler |
| fatal-process (200–299) | fault-log write, then fail-fast |

**Vulkan result mapping — device-lost RESOLVED by D1 (EQ_A4).** `VK_ERROR_DEVICE_LOST` is now mapped: the managed render stack classifies it at the 5 wrapper sites (`DeviceLost.ThrowIfLost` → `DeviceLostException`) and fail-fasts with a structured diagnostic at the render-loop boundary (`DeviceLossBoundary` → `Environment.FailFast`) — v1 policy is crash-with-diagnostic, NO recovery (device re-creation is a separate epic). This supersedes the earlier placeholder "`VK_ERROR_DEVICE_LOST` → fatal-subsystem, recreate-or-die unresolved". Note it stays a MANAGED mapping — device-lost does not flow through the native `df_status` taxonomy. `VK_ERROR_OUT_OF_DATE_KHR` / `VK_SUBOPTIMAL_KHR` → retryable (swapchain recreation, now the prepare-before-reclaim transaction of ELT §2.5). Owned by ELT §4 class 6 / OQ-3 (CLOSED) and VULKAN_SUBSTRATE §6.3; §7 item 7 resolved.

**Downstream propagation** — which faults quarantine a mod, restart a subsystem, or end the process, and how per-mod fault budgets work — is the jurisdiction of `ENGINE_LIFECYCLE_AND_TRANSACTIONS.md` (A3/A8 proper). This section defines only the code space and its managed projection, so that draft has a stable vocabulary to consume.

## 5. Compatibility policy

**ABI-breaking (MAJOR bump; forbidden without a `_v2` sibling or a coordinated release):**

- changing a shipped signature, parameter meaning, or return-code semantics;
- changing any ABI struct's layout, field widths, order, or alignment;
- renumbering shipped enum values (`BusTier`, wake types 0–4, barrier types, scheduling classes, trace event types);
- changing an id space's wire semantics: entity `uint64` packing, subscription-id tier/sequence bit split, id-0 sentinels;
- narrowing a pointer validity window or reversing a buffer-ownership direction.

**Non-breaking (MINOR):** new entry points; new enum values appended at the tail; new reserved id ranges; widening a documented capacity; adding fields at the *end* of caller-allocated out-structs only when a size/version field governs them (none do today — so in practice: new struct = new entry point).

**DLL identity and the single-DLL law.** One library, unversioned name `DualFrontier.Core.Native` (`NativeMethods.cs:23`), platform prefix/extension per loader rules (`NativeMethods.cs:10-15`). K-L2 — pure P/Invoke to this single DLL, zero third-party C# (К-L2, KERNEL_ARCHITECTURE Part 0) — implies there will never be side-by-side native versions: compatibility is carried entirely by §3.1 negotiation plus the `_v2` sibling rule, not by DLL renaming. A MAJOR bump therefore requires a coordinated managed+native release. That is acceptable — both halves live in one repository and ship as one product — but it makes §3.1's runtime check the *only* guard against a stale binary, because the loader will happily load any file with the right name (and no CI builds or pins the native artifact today, verdict N13).

## 6. Enforcement

- **Existing analyzers (cited).** DFK001 (К-L1) and DFK002 (К-L2) already police the native boundary as Error-severity, enforcing, NativeBoundary-category rules (ANALYZER_RULES §1.1 (shipped rule registry)), with exactly 2 census-pinned DFK001 waivers (`ValidationLayer.cs` debug-messenger interop, ANALYZER_RULES §1.4 (waiver census)). The identity/ABI laws extend this family; they do not invent a parallel regime (the Draft DF_TS program is a separate, unreconciled analyzer lineage — session verdict N39 — and is not the vehicle).
- **Proposed rules:**
  - **DFK-entity-identity** (§2): flags `new EntityId(<expr>, <integer literal>)` outside Interop internals and tests. Error severity.
  - **DFK-abi-struct**: every type crossing a `NativeMethods` signature is blittable, `LayoutKind.Sequential`, fixed-width fields, no `bool`, no reference-type fields.
  - **DFK-abi-thread**: every `NativeMethods` member carries the §3.6 affinity attribute; missing attribute is an Error; provably-wrong-thread call sites are flagged where the analyzer can see the executor.
  - **DFK-abi-version**: no `NativeMethods` call reachable before the `df_abi_version` negotiation in bootstrap order.
- **Tests.**
  - ABA regression: create → destroy → flush → recycle the slot → the stale id MUST miss on `TryGetComponent` and MUST be rejected at batch flush. This is §1 note 1. **LANDED at ID_B**: `SpanWriteRoundTripTests.RecycledIndex_StaleKeyIsDropped_WhileTheSpanKeyLands` records two updates in one batch — one keyed on the id the span handed back, one on the stale id sharing its index — and asserts `Flush()` returns exactly 1, with the span-keyed value persisted. Expressible only once managed code supplies real versions, which is why it could not be written before.
  - Round-trip: entity pack/unpack symmetry; save/load of the component-id map (§1 note 2, jointly with A7).
  - Collision: a harness registry forcing two FQNs onto one id asserts the hard failure of §1 note 3.
  - Negotiation: managed refusal on manufactured MAJOR mismatch.
- **Selftest.** `df_native_selftest` (the custom `DF_CHECK` harness, verdict N13) gains an identity section: version monotonicity under recycle stress, span-window enforcement, status-space conformance of every new entry point. **PARTIALLY LANDED at ID_B**: five scenarios cover the versions view (`scenario_versions_view_round_trip`, `_refuses_mutation`, `_release_is_tolerant`, `_recycled_index_true_generation`) and the `is_valid` corner (`scenario_entity_id_is_valid_is_index_only`); 104 → 109 scenarios. Recycle STRESS and status-space conformance for the rest of the ABI remain owed. Selftest remains the only native-side gate until CI exists; closing that gap belongs to the session report's tooling track, not to this contract.

## 7. Open questions and LOCKED-doc conflicts

**Conflicts — LOCKED text wins until the ratification cascade lands:**

1. **VULKAN §3.4 C ABI sketch — RESOLVED by the rework.** The predecessor's `uint32_t field_id` sketch and caller-supplied `pipeline_id` contradicted the shipped string-id / native-allocated-id ABI (`df_capi.h:454-523,559-565`) and FIELDS' own stability rationale; the VULKAN successor §4.3 (field ABI — string-id corrected) records the correction and FIELDS §6 agrees (verdict N14, closed). Retired sketch: historical/VULKAN_SUBSTRATE.md.
2. **Version-0 teaching texts — RESOLVED by the rework.** The pre-rework ECS canonical example and KERNEL §1.7 taught version-0 construction against ECS's own versioning law; the successors (ECS §4/§5, KERNEL §1.7) now carry the corrected non-fabricating form, removed the stale "Access is permitted" comment, and defer the identity fix to this contract's §2 (verdicts C10/N19, closed). The CODE defect (§2) is RESOLVED at ID_B (2026-08-22): the versions view shipped, the three pair-iterators and every production fabrication site were migrated, and DFK022 prevents new ones. ROADMAP F-59 CLOSED.
3. **`EntityId.IsValid` vs native `is_alive`** — RESOLVED at ID_B: both sides now read `Index > 0` (§2; verdict N38). The Phase 0 sweep found zero production consumers of the `(0, v>0)` corner on either side.
4. **`EntityEncoder` version loss** (`EntityEncoder.cs:85`): whether saves persist versions or re-derive them at load is the A7 contract's call ([PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md)); the `DFK-WAIVER(DFK022)` on `EntityEncoder.DecodeRanges` stands until it decides, and names A7 as its retirement trigger.
5. **Capability token — one shape (PARTIALLY RESOLVED).** The field-grammar half is unified by the rework: FIELDS §8 (IModApi wiring) now carries the one shape `<provider>.field.<verb>:<field-id>`. The `layer.*`-verb half (MOD_OS §3 lists layer verbs the §2.3 authoritative regex cannot parse; bare infrastructure verbs without the `:<target>` tail) remains open. Proposed resolution unchanged: exactly one grammar — `<provider>.<verb>:<target>`, no bare-verb tokens — with `layer.*` folded into the manifest regex; MOD_OS §2.3/§3 amend together (verdict N20).
6. **Status-space coexistence — ACCOMMODATED.** KERNEL_ARCHITECTURE §5 declares the `1/0` convention immutable, freezes it for shipped entry points, and forward-references this contract's §3–§4 for the new-surface regime. §4 does not overturn the immutability; the §5 expansion states both regimes side by side. This document takes no authority over re-opening the immutability wording.

**Open — no current owner:**

7. Device-lost / `VkResult` mapping (§4) — **RESOLVED by D1 (EQ_A4, 2026-07-18): `VK_ERROR_DEVICE_LOST` → fail-fast v1 (managed `DeviceLossBoundary` → `Environment.FailFast`, no recovery), owned by ELT §4 class 6 / OQ-3 (CLOSED) and VULKAN §6.3 / OQ-V1.**
8. Event-id reserved-range boundaries, and whether the event registry stores FQN *copies* (prerequisite for both collision detection, §1 note 3, and retiring the store-by-pointer exception, §3.3).
9. Wake-id namespace owner for Explicit wakes (§1 row 11) — allocation registry vs capability-token gating.
10. ~~Whether `SpanLease.Pairs` keeps its synthetic-version escape hatch during the §2 migration window~~ — **CLOSED at ID_B (2026-08-22)**: the recommendation was taken. No escape hatch shipped; the mechanism (C3/C4), the site migration (C5), the analyzer (C6) and the teaching-doc amendments (C7) landed in one cascade, and DFK022 was enforcing at Error from the moment it appeared rather than after a grace period.
11. System-id renumbering (§1 note 6) interacts with the scheduler cutover (A0 matrix): renumbering while two schedulers reference the ids is riskier than after cutover. Sequencing call belongs to the ratification milestone.

## Cross-references

- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 (K-L2/K-L4/K-L8), §1.7, §1.10, Part 7 — primary ratification target.
- [ECS](./ECS.md), [FIELDS](./FIELDS.md), [EVENT_BUS](./EVENT_BUS.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) — per-doc amendment targets enumerated in §7.
- [ANALYZER_RULES](./ANALYZER_RULES.md) — DFK registry receiving the §6 additions.
- [RESOURCE_OWNERSHIP_AND_LIFETIME](./RESOURCE_OWNERSHIP_AND_LIFETIME.md) (the A2 contract) — ownership tree and dispose order; §3.3 here states only per-call ABI rules.
- [PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (A7 draft) — save-side halves of §1 notes 2 and §7 item 4.
- `ENGINE_LIFECYCLE_AND_TRANSACTIONS.md` — fault taxonomy consuming §4's code space.
- [docs/reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md) — session report; verdict ids cited throughout (C10, N13, N14, N19, N20, N29, N38, N39, A5, A6, A8) resolve there.