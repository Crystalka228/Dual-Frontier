---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-IDENTITY_AND_ABI_CONTRACT
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: "0.1.0"
next_review_due: post-ratification closure
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-IDENTITY_AND_ABI_CONTRACT
---
# Identity and ABI Contract (the A5+A6 contract)

> **Document class: authored-proposal (normative-target). NOT current truth, NOT enforceable law.** Produced by the Architecture Decomposition & Contracts session 2026-07-15 ([docs/reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)). It closes gaps A5 (identity contract) and A6 (C ABI protocol) and supplies the error-taxonomy portion of A8. Becomes normative only upon Crystalka ratification per [FRAMEWORK](../governance/FRAMEWORK.md) §7 (amendment milestone protocol, §7.2). Until then no document may cite it as authority; **conflicts resolve in favor of existing LOCKED docs** (enumerated honestly in §7).

**Ratification path** (per FRAMEWORK.md §7.2 Tier 1 amendment protocol):

| This document's section | Folds into, on ratification |
|---|---|
| §1 identity registry table | [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — new part ("Part 9: Identity registry"), plus per-doc amendments: [ECS](./ECS.md) span example, [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) §1.7 example, [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) §3.4 `uint32` fix, [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §2.3/§3.2 capability grammar unification |
| §2 version-0 resolution | [ECS](./ECS.md) + [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) §1.7 amendments; `EntityId.cs` / `entity_id.h` code change; new DFK rule in [ANALYZER_RULES](./ANALYZER_RULES.md) |
| §3 C ABI protocol + §4 error contract + §5 compatibility | [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 7 expansion (the existing error-semantics convention grows into a full ABI protocol) |
| §6 enforcement | [ANALYZER_RULES](./ANALYZER_RULES.md) registry additions + `df_native_selftest` extension |

Baseline for every "today" claim: working tree at HEAD `6f39903`, 2026-07-15.

---

## 0. Why this document exists

Identity rules are scattered across at least six documents and two languages, with live drift between them. Three verified examples:

- **The entire managed hot path fabricates entity versions.** Ten production systems plus `GameBootstrap.cs:241` and `src/DualFrontier.Persistence/Compression/EntityEncoder.cs:85` construct `new EntityId(indices[i], 0)`, and `SpanLease.Pairs` fabricates `Version = 1` (`src/DualFrontier.Core.Interop/SpanLease.cs:112`) — while native `is_alive` demands `id.version == versions_[id.index]` (`native/DualFrontier.Core.Native/src/world.cpp:74-78`). The generation/ABA machinery exists natively and is never exercised from managed code. Worse, the teaching texts *instruct* the defect (§2).
- **Identity drift inside one LOCKED document.** [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) §3.4 sketches `uint32_t field_id` (lines 831-854) while its own shipped-shape subsection (lines 861-886) and the real ABI use string ids (`df_capi.h:454-523`).
- **The C ABI has no protocol.** No version negotiation, no struct-layout/alignment law, no buffer-ownership rules, no error taxonomy (retryable vs fatal vs contract-violation), no thread-affinity declaration per entry point, no pointer validity windows. The only existing ABI law is the status-code + `catch (...)` convention (`KERNEL_ARCHITECTURE.md:759`) and the four-category managed rule (Part 7) — real, but thin (session verdict A6: "CONFIRMED missing (thin)").

An engine whose kernel is a foreign-function boundary lives or dies by exactly two contracts: *what a value means* (identity) and *how it crosses* (ABI). This document is both, in one place, because they constrain each other — every identity in §1 has a wire form, and every ABI rule in §3 exists to keep those wire forms honest.

## 1. Identity registry

One table, every identity the engine mints. Column definitions: **allocation** — who mints the value and how; **collision policy** — what happens when two owners want the same value; **generation / reuse** — whether a value can be reissued and how staleness is detected; **registration lifetime** — how long a registered value stays meaningful. Terse cells; the numbered notes below the table carry the law.

| # | Identity | Canonical representation | Namespace owner | Allocation | Collision policy | Generation / reuse | Serialization form | Invalid / sentinel | Registration lifetime |
|---|---|---|---|---|---|---|---|---|---|
| 1 | EntityId | `record struct (int Index, int Version)` (`EntityId.cs:21`); packed `uint64` at ABI: hi 32 = Version, lo 32 = Index (`entity_id.h:19-23`, `df_capi.h:23-27`) | kernel world | native `create_entity`: free-list recycle or `next_index_++` (`world.cpp:57-72`) | impossible (single allocator) | Version bumps on destroy (`world.cpp:90`); slot reused with higher version; ABA law, note 1 | two `int32` fields, fixed — changing field types breaks Save/Load (`EntityId.cs:17-19`) | `Invalid = default` (0,0) (`EntityId.cs:28`); index 0 permanently dead (`world.cpp:75`) | until destroy + flush |
| 2 | Component TypeId | `uint32` | kernel-local, per process | managed `ComponentTypeRegistry`, sequential from 1, idempotent per type (`KERNEL_ARCHITECTURE.md:513-547`) | none possible (sequential); re-register same id with different size rejected (`df_capi.h:139-141`) | stable within a run only; **not stable across load-order change**, note 2 | MUST NOT persist raw; save writes string→id map, note 2 | 0 reserved invalid (`KERNEL_ARCHITECTURE.md:519`, `df_capi.h:141`) | world lifetime |
| 3 | Event TypeId | `uint32` = FNV-1a-32 of CLR FQN (`EVENT_BUS.md:76`; `BusFacade.cs:57-62,176-182`) | global (derived from FQN) | derived, not allocated | **TODAY ABSENT**, note 3 | none; stable across runs because FQN-derived | re-derivable; persist FQN, never the hash | 0 by convention (FNV output 0 treated as failure, cf. `ModUnloadInterop.cs:88`) | process-global registry until `clear` (test-only) |
| 4 | Field id | string `<mod-namespace>.<field-name>` (`FIELDS.md:76-78`) | registering mod's manifest id (MOD_OS §3.3 reserved-namespace rule) | mod authors the name under its namespace | second registration → `FieldRegistrationConflict` (`FIELDS.md:78`, MOD_OS §11.2) | stable across mod reload **because** string, note 4 | the string; save records `(field_id, owning_mod_id, dims, blob)` (`FIELDS.md:301`) | null/empty rejected | until `Unregister` / mod unload |
| 5 | ModId | reverse-domain string, globally unique (`MOD_OS_ARCHITECTURE.md:265`) | mod author | authored in manifest | duplicate ids rejected at batch validation | n/a | the string; save pairs it with modVersion (MOD_OS D-6) | empty rejected at parse (§2.3 rule 2) | mod load session |
| 6 | ModId, native bus form | `uint32` = FNV-1a-32 of ModId string (`ModUnloadInterop.cs:85-101`) | derived | derived | **ABSENT**, note 3 | n/a | never persisted | 0 = Core/vanilla (`EVENT_BUS.md:69`) or null input | mod load session |
| 7 | Subscription id | `uint64`: high 8 bits = tier, low 56 = per-tier sequence (`EVENT_BUS.md:64`; K-L15.1, `KERNEL_ARCHITECTURE.md:64`) | native bus, per tier | per-tier `next_seq` counter | cross-tier collision structurally impossible (tier bits) | monotonic; never reused within a process | never persisted | none defined — propose 0, note 5 | until unsubscribe / per-mod bulk unsub / `df_bus_clear` |
| 8 | InternedString handle | `struct (uint Id, uint Generation)` (`InternedString.cs:58-72`) | world string pool | native intern within mod-scoped windows (`df_capi.h:244-266`) | same content interns to same id (co-ownership recorded) | generation bumps on `clear_mod_scope`; stale `{id, gen}` resolves not-found (`df_capi.h:262-266`) | **serialize CONTENT, never ids** — LOCKED (`df_capi.h:267-271`) | `Id == 0` empty sentinel (`InternedString.cs:60-64`) | until owning mod scope cleared |
| 9 | Capability token | string `<provider>.<verb>:<target>` (`MOD_OS_ARCHITECTURE.md:319`; regex §2.3 step 6, `:292`) | provider (`kernel` or `mod.<modId>`) | authored in manifest / emitted by `KernelCapabilityRegistry` | exact-match set; wildcards forbidden (§3.1, `:307-312`) | n/a | the string | non-matching token rejected at parse | mod load session; revoked at unload step T4 (`mod_unload.h:12`) |
| 10 | System id | `uint32` at ABI (`df_capi.h:593`); string `SystemId` in `SystemRegistration` (`KERNEL_ARCHITECTURE.md:476-481`) | kernel scheduler | **TODAY: array index from 0** (`GameBootstrap.cs:162-176`) | none — caller-supplied, unchecked | none | string form (`"vanilla.pawn.needs_system"`) is the stable name; the uint32 is per-run | **none — 0 is a live system today**, note 6 | until unregister / `df_scheduler_clear` |
| 11 | Wake id (Explicit wake type) | `uint32` (`df_capi.h:652`) | undeclared | caller-chosen | **ABSENT** — two systems can claim one wake id | none | never persisted | none defined | until unsubscribe |
| 12 | Pipeline id | `uint32`, native-allocated monotonically from 1 (`df_capi.h:525-548,559-565`) | kernel compute registry | native returns id at registration | duplicate pipeline *name* rejected (`df_capi.h:539`) | none within a process | persist the pipeline name, never the id | 0 = failure/invalid | world lifetime (mod-unload cleanup is a placeholder, VULKAN §3.4.1) |

### Notes (the actual law per row)

**Note 1 — EntityId ABA law (normative).** A given `(Index, Version)` pair is issued at most once per world lifetime: `destroy_entity` increments `versions_[index]` before the slot returns to the free list (`world.cpp:84-93`), and `flush_destroyed` recycles only the index (`world.cpp:95-108`), so a recycled slot always carries a strictly higher version. Any cached id therefore resolves dead forever after its entity is destroyed — across ticks, saves, deferred events, and mod boundaries. This is the engine's only structural defense against stale-reference corruption, and it is currently nullified managed-side by version fabrication (§2). The pack/unpack functions preserve Version honestly (`entity_id.h:19-30`); the loss happens purely in managed construction.

**Note 2 — Component TypeId save implication (proposal).** Sequential ids are "deterministic per mod load order" and "mod load order matters" (`KERNEL_ARCHITECTURE.md:547-549`). Consequence: adding, removing, or renaming a mod between save and load silently reassigns every subsequent numeric id, and any persisted raw id decodes as the wrong component type — a corruption class with no error signal. Proposed law: **numeric component ids never cross the save boundary.** The save header carries a `FQN string → uint32` map captured at save time; the loader translates old ids to current ids at load, and a missing FQN is a typed load error, not a guess. Precedent: the string pool's serialize-content-not-ids rule, already LOCKED (`df_capi.h:267-271`). Owner for the save-format half: the A7 draft ([PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md)).

**Note 3 — Event TypeId / native ModId collision policy (proposal).** FNV-1a-32 collisions over FQN space are improbable but not impossible, and today they are *silent*: `df_event_type_registry_register` returns 0 for "already registered" (`event_type_registry.h:64-66`), so a colliding second type simply fails registration and its events dispatch under the first type's tier, payload size, and subscribers. Proposed law: (a) registration-time collision detection — the registry compares the stored FQN against the incoming FQN whenever the id already exists; same FQN = idempotent success, different FQN = hard contract-violation status (`DF_ERR_ID_COLLISION`, §4) that aborts the registering mod's load; (b) id 0 stays reserved; (c) a reserved range (proposal: `0x00000001–0x000000FF`) is carved out for kernel-synthetic event types so a mod FQN hash can never shadow them. The same FQN-comparison rule applies to the native `mod_id` hash (`ModUnloadInterop.cs:85-101`), where a collision today would merge two mods' bulk-unsubscribe identity — one mod's unload would strip another mod's subscriptions.

**Note 4 — Field id (kill the uint32 drift).** String identity is the *rationale-carrying* choice: "Numeric field ids are not stable across mod reload… string ids are" (`FIELDS.md:140`). The shipped ABI agrees (`const char* field_id` throughout, `df_capi.h:454-523`). The `uint32_t field_id` sketch in VULKAN §3.4 (`:831-854`) is drift inside a LOCKED doc and is amended out (§7 item 1). No numeric alias for fields is ever introduced; if profiling someday demands one, it must be a per-run lookup handle acquired from the string, never a persisted identity.

**Note 5 — Subscription id sentinel.** The bit layout leaves value 0 (tier 0, sequence 0) reachable in principle for the Fast tier's first subscription. Proposed: per-tier sequences start at 1, making the all-zero `uint64` a true never-issued sentinel. Cheap now, impossible after subscriptions are ever persisted or exposed to mods.

**Note 6 — System id (proposal).** Align with the component-id convention: 0 reserved invalid, ids allocated sequentially from 1 by a registry keyed on the stable string `SystemId`/FQN (the ABI already transports the FQN at registration, `df_capi.h:593-595`). Today's allocation is the `coreSystems` array index starting at 0 (`GameBootstrap.cs:162-179`) — it uses the proposed sentinel value for a live system, and reordering a source-file array silently renumbers every system id the native graph, wake registry, policies, and trace events refer to.

## 2. The version-0 resolution (highest-value single fix)

**The defect, precisely.** `KERNEL_ARCHITECTURE.md` §1.7 line 391 *teaches* the pattern — `EntityId entity = new EntityId(lease.Indices[i], 0); // version not exposed via span` — and `ECS.md:81` repeats it in the document's canonical system example, in the same file whose §Versioning declares the version "the indicator of a dead reference" (`ECS.md:102-104`). Production followed the teaching: 13+ sites across 10 systems fabricate version 0 (`HaulSystem.cs:54`, `NeedsSystem.cs:93`, `JobSystem.cs:100`, `ConsumeSystem.cs:57,132,152,177`, `MovementSystem.cs:108`, `MoodSystem.cs:40`, `SleepSystem.cs:57,101,185`, `PawnStateReporterSystem.cs:63`, `GameBootstrap.cs:241`), `EntityEncoder.cs:85` decodes saved index ranges to version 0, and `SpanLease.Pairs` fabricates `Version = 1` with an honest caveat comment (`SpanLease.cs:76-84,112`). Because these fabricated ids flow into `TryGetComponent` lookups and batch records, generation validation collapses to "the index was never recycled" (session verdicts C10/N29). The window is real: `DestroyEntity` → `flush_destroyed` → free-list recycle can complete within the same tick sequence a stale fabricated id is still circulating in.

**The law.** Span and batch ABI surfaces MUST surface true versions; managed code MUST NOT construct an `EntityId` whose Version it did not receive from the world. Three candidate mechanisms:

1. **Parallel versions view (RECOMMENDED).** One additive entry point: `df_world_acquire_versions(world, const int32_t** out_versions_ptr, int32_t* out_count)` exposing a read-only view of the native `versions_` table, valid under the same acquire/release discipline as component spans (`df_capi.h:88-99`; the mutation-rejection counter already guarantees the table cannot resize while any span is active). `SpanLease<T>` acquires it alongside the component span; `Pairs` and every hot loop reconstruct `new EntityId(idx, versions[idx])`. No shipped signature changes, zero copies, one extra P/Invoke per lease. `SpanLease.cs:76-84` itself already names this option ("extending the span ABI to return parallel version arrays — deferred to K7"); the deferral ends here.
2. **Extended span acquire.** Add a versions out-parameter to `df_world_acquire_span`. Rejected: mutates a shipped signature (ABI-breaking per §5, so it would have to ship as `df_world_acquire_span_v2`), and a dense-parallel version array requires a per-acquire copy that option 1 avoids.
3. **Index-only `EntityRef`.** A distinct `readonly record struct EntityRef(int Index)` for span-scoped iteration, convertible to `EntityId` only through the world (which fills in the true version). Fabrication becomes *unrepresentable* — the strongest shape — but it ripples through every `EntityId`-accepting API and all 13+ call sites simultaneously. Retained as the structural endgame if the analyzer rule below proves leaky; not the first step.

**Sketch of the recommended mechanism** (signatures normative-target, names bikesheddable at the amendment milestone):

```c
/* df_capi.h addition — same acquire/release discipline as component spans.
 * Window: pointer valid until df_world_release_versions; mutations rejected
 * while held (shares the active_spans_ counter). Read-only view over the
 * per-slot versions_ table, indexed by ENTITY INDEX, not dense position. */
DF_API int32_t df_world_acquire_versions(
    df_world_handle world,
    const int32_t** out_versions_ptr,
    int32_t*        out_count);

DF_API void     df_world_release_versions(df_world_handle world);
```

```csharp
// SpanLease<T>.Pairs — corrected Current (replaces SpanLease.cs:112):
public (EntityId Entity, T Component) Current
{
    get
    {
        int entityIndex = _lease.Indices[_index];
        return (new EntityId(entityIndex, _lease.Versions[entityIndex]),
                _lease.Span[_index]);
    }
}

// EntityId.IsValid — corrected (replaces EntityId.cs:38; see below):
public bool IsValid => Index > 0;
```

**Consequential amendments (the ratification cascade):**

- `ECS.md` span example and `KERNEL_ARCHITECTURE.md` §1.7 example are rewritten to the versions-view idiom. The adjacent `ECS.md:84` comment "Access is permitted — both components are declared in reads" is corrected in the same edit — it implies a runtime permission check that was deleted (session verdict N19).
- `EntityId(index, 0)` construction becomes analyzer-detectable: new rule **DFK-entity-identity** (working id; the final id lands as the DFK number matching the К-L row this law ratifies as — К-L20 is already reserved post-Mod-API lock, `KERNEL_ARCHITECTURE.md:70`, and DFK numbering is 1:1 with К-L numbers per `ANALYZER_RULES.md:95`). The rule flags any `new EntityId(<expr>, <integer literal>)` outside `DualFrontier.Core.Interop` internals and test fixtures, Error severity, NativeBoundary category — same class as DFK001/DFK002.
- `EntityEncoder` (persistence) receives a census-pinned waiver only until the A7 contract decides how versions are persisted (§7 item 4); the waiver mechanism and its meta-test census already exist (`ANALYZER_RULES.md:298`).

**`IsValid` alignment.** `EntityId.IsValid => Index > 0 || Version > 0` (`EntityId.cs:38`) disagrees with native `is_alive`, which rejects `index <= 0` unconditionally (`world.cpp:75`): the id `(0, 5)` is "valid" managed-side and permanently dead native-side (verdict N38). Current-generation knowledge is unknowable managed-side without a world call, so `IsValid` stays a *syntactic* check by design — but it must be the syntactic projection of the native rule: **`IsValid => Index > 0`**. Aliveness remains answerable only by the world (`World.IsAlive`, comparing versions), exactly as the existing doc comment already warns (`EntityId.cs:34-37`). The native mirror `entity_id.h:14-16` carries the same flawed disjunction (`index > 0 || version > 0`) and is fixed in the same change. No shipped behavior depends on the `(0, v>0)` corner being "valid" — the change is safe by inspection of all `IsValid` call sites, to be verified during the amendment milestone.

## 3. C ABI protocol

Scope: every `extern "C"` entry point exported by `DualFrontier.Core.Native` (`df_capi.h`, `bus_native.h`, `event_type_registry.h`, `mod_unload.h`, and successors). Shipped conventions cited below stay LOCKED; everything marked *proposed* is normative-target only.

### 3.1 Versioning and negotiation (proposed — absent today)

- New entry point `int32_t df_abi_version(void)` returning `(MAJOR << 16) | MINOR`. The managed bootstrap MUST call it before any other ABI call (before `df_engine_bootstrap`) and refuse to run on MAJOR mismatch — fail-fast with a user-facing diagnostic, mirroring the K-L19 hardware-gate precedent.
- Evolution rules (semver-ish): **adding** an entry point bumps MINOR. **Changing** the signature or observable semantics of a shipped entry point is forbidden — ship a `_v2` sibling and deprecate the old one in the header comment. **Removing** an entry point, changing a struct layout, or renumbering an enum bumps MAJOR.
- Why negotiation matters here specifically: the native binary is not committed and there is no CI (session verdict N13), so managed/native skew is a live deployment risk today. The existing P/Invoke marshalling pre-flight check (`KERNEL_ARCHITECTURE.md:743`) catches skew at review time; `df_abi_version` catches it at run time.

### 3.2 Type law (partially shipped, made explicit)

- **Blittable only.** Pointers + fixed-width primitives. Already the batched-callback constraint ("All args blittable (pointer + primitives only)", `KERNEL_FULL_NATIVE_SCHEDULER.md:616`) and the component-payload assumption (`df_capi.h:29-35`). No generics across the boundary (`KERNEL_FULL_NATIVE_SCHEDULER.md:617`).
- **Layout.** Every ABI struct uses `stdint.h` fixed-width fields, `LayoutKind.Sequential` on the managed mirror, natural alignment, no packing pragmas, no bitfields. Existing structs already comply (`df_trace_event` `df_capi.h:757-763`; `df_managed_system_batch` `:814-819`; `ModUnloadResult` `mod_unload.h:48+`); the law converts custom into mandatory.
- **Endianness.** Little-endian assumed and now stated. The entity packing (`entity_id.h:19-23`) is defined in value space (shifts and masks), not byte space, so it is safe wherever both sides agree on integer representation — which the LE statement pins.
- **No `bool` at the boundary.** Truthiness travels as `int32_t` 0/1 — already universal practice (`df_world_is_alive`, every `*_enabled` accessor). Marshaling C# `bool` is ambiguous (1 vs 4 bytes depending on context) and is forbidden.

### 3.3 Ownership law (per-call rules; the ownership *tree* lives in the A2 draft)

- **Allocation symmetry.** Native never frees managed memory; managed never frees native memory. Handle ownership follows the documented owner per handle class: world handles — caller, via `df_world_destroy`; batch handles — caller, via `df_batch_destroy` (destroy auto-flushes, `df_capi.h:196-198`); K8.1 primitive handles — the World, "Caller never frees" (`df_capi.h:254-258`). Cross-cutting dispose order is [RESOURCE_OWNERSHIP_AND_LIFETIME](./RESOURCE_OWNERSHIP_AND_LIFETIME.md)'s jurisdiction.
- **Out-buffers are caller-allocated.** The iterate/snapshot family takes caller buffers plus capacity and returns count written, clipped (`df_keyed_map_iterate` `df_capi.h:272-276`; `df_world_resolve_string` `:290-295`; `df_scheduler_trace_dump`, `df_scheduler_snapshot` `:778`). New entry points MUST follow this pattern; a native-allocated return requires a paired `df_*_free` and explicit rationale in the header comment.
- **String lifetime = call duration unless documented.** Default: `const char*` parameters are valid only for the duration of the call; native copies what it keeps. One grandfathered documented exception exists: `df_event_type_registry_register` stores the FQN pointer without copying — "owned by caller; lifetime ≥ registry" (`event_type_registry.h:42`, "stored by pointer" `:77`) — and the managed side satisfies it with a deliberately never-freed `Marshal.StringToHGlobalAnsi` allocation (`EventTypeRegistryInterop.cs:48-62`). Proposed: migrate this entry point to copy-on-register at the next MAJOR bump; store-by-pointer is a standing footgun for any caller that has not read that one comment.

### 3.4 No-exception law, both directions (shipped — cited)

- **Native → managed:** every `extern "C"` function returns a status code or sentinel and swallows all exceptions via `catch (...)` at the boundary; "non-negotiable for cross-DLL safety" (`KERNEL_ARCHITECTURE.md:759`; 72 `catch (...)` sites in `capi.cpp` at HEAD).
- **Managed → native (reverse P/Invoke):** callbacks are `static`, `[UnmanagedCallersOnly]`, blittable-args-only, and absorb every managed exception before returning (`KERNEL_FULL_NATIVE_SCHEDULER.md:572-590` — the `FaultLog.RecordKernelBatchFault` absorb pattern; constraints list `:614-620`).
- **GC transitions:** `SuppressGCTransition` is forbidden for reverse P/Invoke (`KERNEL_FULL_NATIVE_SCHEDULER.md:619`). For forward P/Invoke it is currently used nowhere (`NativeMethods.cs` is plain `[DllImport(..., CallingConvention.Cdecl)]`, `:23-25`); any future use requires per-entry-point proof of non-blocking, non-allocating, sub-microsecond behavior, recorded in the header comment.

### 3.5 Pointer validity windows (proposed law: declared per entry point)

Every entry point that returns or accepts a pointer MUST document its validity window in the header. Normative table for shipped surfaces:

| Pointer | Valid from | Valid until | Anchor |
|---|---|---|---|
| component span (`out_dense_ptr`, `out_indices_ptr`) | `df_world_acquire_span` returns | `df_world_release_span` for that type; mutations rejected meanwhile | `df_capi.h:88-99` |
| field span (`out_data`) | `df_world_field_acquire_span` | `df_world_field_release_span` | `df_capi.h:443-448` |
| versions view (§2, proposed) | `df_world_acquire_versions` | paired release, same discipline | — |
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
| bus publish/subscribe/drain | internally-synchronized | per-tier mutexes with fixed fast→normal→background acquisition order (`EVENT_BUS.md:64`) |
| `df_world_attach_vulkan`, compute pipeline registration | main-thread-only | pending the render-thread contract; Vulkan handle ownership per VULKAN §3.4 |
| `df_engine_bootstrap`, `df_world_create/destroy` | any-thread + external-sync | caller serializes creation/teardown; concurrent engines are isolated (`df_capi.h:174-177`) |

The declaration lands as a tag line in each `df_capi.h` block comment and a mirrored attribute on `NativeMethods` members so the analyzer (§6) can consume it.

## 4. Error contract (the A8 taxonomy portion)

**Today.** Three regimes coexist: boolean `1/0` with sentinel returns ("0 — failure / not found; 1 — success / present. Out-of-range inputs return 0 rather than crashing", `df_capi.h:37-40`); ad-hoc negative codes (`df_batch_flush` returns −1 on "logic error", `df_capi.h:212`; scheduler graph returns −1 write-write conflict / −2 cycle, `:588-591`); and fixed-size out-struct error payloads (`error_messages[8][256]` + `error_count` in `ModUnloadResult`, `mod_unload.h:41-49`, mirrored in `VulkanModUnloadResult`, VULKAN §3.4.1). There is no `last_error`, no retryability classification, and no device-lost story anywhere in the corpus (session verdict A8).

**Proposed status space (`df_status`) — new and `_v2` entry points only.** The `KERNEL_ARCHITECTURE.md:759` convention is explicitly labeled immutable, so shipped `1/0` entry points are grandfathered unchanged; the ratified Part 7 must state both regimes side by side (§7 item 6). New entry points return `int32_t df_status`:

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
/* positive 100-199 — fatal-subsystem; 200-299 — fatal-process */
#define DF_FAIL_SUBSYSTEM_BASE     100
#define DF_FAIL_PROCESS_BASE       200
```

**Diagnostic retrieval (proposed).** `int32_t df_last_error(char* out_buffer, int32_t capacity)` — thread-local storage, records the human-readable message of the calling thread's most recent non-OK status, caller-allocated buffer per §3.3, never load-bearing for control flow. This supersedes the fixed `error_messages[8][256]` pattern for new surfaces (which silently drops messages beyond 8, `mod_unload.h:43-47`); the shipped unload structs keep their shape — they are frozen ABI.

**Mapping to managed exceptions.** The four-category rule (`KERNEL_ARCHITECTURE.md:761-769`) already decides throw-vs-return per abstraction shape; the status classes bind onto it:

| Status class | Managed projection |
|---|---|
| retryable (1–99) | `bool` / `TryX` return — the sparse category; never an exception |
| contract violation (negative) | throw — `ObjectDisposedException` / `InvalidOperationException` per the lifecycle and construction categories; dense category throws range exceptions |
| fatal-subsystem (100–199) | typed exception (`HardwareCapabilityException` precedent, VULKAN §7.1) routed to the mod/system fault handler |
| fatal-process (200–299) | fault-log write, then fail-fast |

**Vulkan result mapping — OPEN.** Nothing maps `VkResult` into any error class today; "device-lost" has zero corpus hits. Placeholder classification pending a V-cycle decision: `VK_ERROR_DEVICE_LOST` → fatal-subsystem (with recreate-or-die unresolved); `VK_ERROR_OUT_OF_DATE_KHR` / `VK_SUBOPTIMAL_KHR` → retryable (swapchain recreation is shipped behavior). Tracked in §7 item 7.

**Downstream propagation** — which faults quarantine a mod, restart a subsystem, or end the process, and how per-mod fault budgets work — is the jurisdiction of the `ENGINE_LIFECYCLE_AND_TRANSACTIONS.md` session draft (A3/A8 proper). This section defines only the code space and its managed projection, so that draft has a stable vocabulary to consume.

## 5. Compatibility policy

**ABI-breaking (MAJOR bump; forbidden without a `_v2` sibling or a coordinated release):**

- changing a shipped signature, parameter meaning, or return-code semantics;
- changing any ABI struct's layout, field widths, order, or alignment;
- renumbering shipped enum values (`BusTier`, wake types 0–4, barrier types, scheduling classes, trace event types);
- changing an id space's wire semantics: entity `uint64` packing, subscription-id tier/sequence bit split, id-0 sentinels;
- narrowing a pointer validity window or reversing a buffer-ownership direction.

**Non-breaking (MINOR):** new entry points; new enum values appended at the tail; new reserved id ranges; widening a documented capacity; adding fields at the *end* of caller-allocated out-structs only when a size/version field governs them (none do today — so in practice: new struct = new entry point).

**DLL identity and the single-DLL law.** One library, unversioned name `DualFrontier.Core.Native` (`NativeMethods.cs:23`), platform prefix/extension per loader rules (`NativeMethods.cs:10-15`). K-L2 — pure P/Invoke to this single DLL, zero third-party C# (`KERNEL_ARCHITECTURE.md:51`) — implies there will never be side-by-side native versions: compatibility is carried entirely by §3.1 negotiation plus the `_v2` sibling rule, not by DLL renaming. A MAJOR bump therefore requires a coordinated managed+native release. That is acceptable — both halves live in one repository and ship as one product — but it makes §3.1's runtime check the *only* guard against a stale binary, because the loader will happily load any file with the right name (and no CI builds or pins the native artifact today, verdict N13).

## 6. Enforcement

- **Existing analyzers (cited).** DFK001 (К-L1) and DFK002 (К-L2) already police the native boundary as Error-severity, enforcing, NativeBoundary-category rules (`ANALYZER_RULES.md:135-136`; on-disk `:118`), with exactly 2 census-pinned DFK001 waivers (`ValidationLayer.cs` debug-messenger interop, `:298`). The identity/ABI laws extend this family; they do not invent a parallel regime (the Draft DF_TS program is a separate, unreconciled analyzer lineage — session verdict N39 — and is not the vehicle).
- **Proposed rules:**
  - **DFK-entity-identity** (§2): flags `new EntityId(<expr>, <integer literal>)` outside Interop internals and tests. Error severity.
  - **DFK-abi-struct**: every type crossing a `NativeMethods` signature is blittable, `LayoutKind.Sequential`, fixed-width fields, no `bool`, no reference-type fields.
  - **DFK-abi-thread**: every `NativeMethods` member carries the §3.6 affinity attribute; missing attribute is an Error; provably-wrong-thread call sites are flagged where the analyzer can see the executor.
  - **DFK-abi-version**: no `NativeMethods` call reachable before the `df_abi_version` negotiation in bootstrap order.
- **Tests.**
  - ABA regression: create → destroy → flush → recycle the slot → the stale id MUST miss on `TryGetComponent` and MUST be rejected at batch flush. This is §1 note 1 — currently true natively and untested end-to-end because managed code never supplies real versions.
  - Round-trip: entity pack/unpack symmetry; save/load of the component-id map (§1 note 2, jointly with A7).
  - Collision: a harness registry forcing two FQNs onto one id asserts the hard failure of §1 note 3.
  - Negotiation: managed refusal on manufactured MAJOR mismatch.
- **Selftest.** `df_native_selftest` (the custom `DF_CHECK` harness, verdict N13) gains an identity section: version monotonicity under recycle stress, span-window enforcement, status-space conformance of every new entry point. Selftest remains the only native-side gate until CI exists; closing that gap belongs to the session report's tooling track, not to this contract.

## 7. Open questions and LOCKED-doc conflicts

**Conflicts — LOCKED text wins until the ratification cascade lands:**

1. **VULKAN §3.4 C ABI sketch.** `uint32_t field_id` (`VULKAN_SUBSTRATE.md:831-854`) and caller-supplied `pipeline_id` (`:856-858`) contradict the shipped string-id / native-allocated-id ABI (`df_capi.h:454-523,559-565`) and FIELDS' own stability rationale (`FIELDS.md:140`). The doc's shipped-shape subsection (`:861-886`) is already correct; the amendment deletes or period-marks the sketch. Drift inside one LOCKED doc (verdict N14).
2. **ECS.md:81 and KERNEL_ARCHITECTURE.md:391** teach version-0 construction against `ECS.md:102-104`'s own versioning law — amended per §2, including the stale "Access is permitted" comment at `ECS.md:84` (verdict N19).
3. **`EntityId.IsValid` vs native `is_alive`** (`EntityId.cs:38` and `entity_id.h:14-16` vs `world.cpp:75`) — both sides amended to `Index > 0` (§2; verdict N38).
4. **`EntityEncoder` version loss** (`EntityEncoder.cs:85`): whether saves persist versions or re-derive them at load is the A7 contract's call ([PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md)); the DFK-entity-identity waiver stands until it decides.
5. **Capability token — one shape.** MOD_OS §3.2 lists `layer.intent` / `layer.combat_feedback` verbs (`MOD_OS_ARCHITECTURE.md:325`) that §2.3's authoritative regex cannot parse ("a manifest cannot currently declare layer tokens", `:295`); MOD_OS `:46-47` cites bare infrastructure verbs (`kernel.field.acquire`) without the `:<target>` tail the regex requires; `FIELDS.md:364` uses the full three-part shape. Proposed resolution: exactly one grammar — `<provider>.<verb>:<target>`, no bare-verb tokens — with `layer.*` folded into the manifest regex; MOD_OS §2.3/§3.2 and FIELDS amend together (verdict N20).
6. **Status-space coexistence.** `KERNEL_ARCHITECTURE.md:759` declares the `1/0` convention immutable. §4 does not overturn it; the Part 7 expansion must define both regimes explicitly — boolean regime frozen for shipped entry points, `df_status` for new ones — or the ratification milestone must formally re-open the immutability wording. This document takes no authority over that choice.

**Open — no current owner:**

7. Device-lost / `VkResult` mapping (§4) — needs a V-cycle decision; today unspecified corpus-wide.
8. Event-id reserved-range boundaries, and whether the event registry stores FQN *copies* (prerequisite for both collision detection, §1 note 3, and retiring the store-by-pointer exception, §3.3).
9. Wake-id namespace owner for Explicit wakes (§1 row 11) — allocation registry vs capability-token gating.
10. Whether `SpanLease.Pairs` keeps its synthetic-version escape hatch during the §2 migration window, or the versions view ships atomically with the teaching-doc amendments (recommended: atomic — a migration window here recreates the defect under a smaller flag).
11. System-id renumbering (§1 note 6) interacts with the scheduler cutover (A0 matrix): renumbering while two schedulers reference the ids is riskier than after cutover. Sequencing call belongs to the ratification milestone.

## Cross-references

- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0 (K-L2/K-L4/K-L8), §1.7, §1.10, Part 7 — primary ratification target.
- [ECS](./ECS.md), [FIELDS](./FIELDS.md), [EVENT_BUS](./EVENT_BUS.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) — per-doc amendment targets enumerated in §7.
- [ANALYZER_RULES](./ANALYZER_RULES.md) — DFK registry receiving the §6 additions.
- [RESOURCE_OWNERSHIP_AND_LIFETIME](./RESOURCE_OWNERSHIP_AND_LIFETIME.md) (A2 draft) — ownership tree and dispose order; §3.3 here states only per-call ABI rules.
- [PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (A7 draft) — save-side halves of §1 notes 2 and §7 item 4.
- `ENGINE_LIFECYCLE_AND_TRANSACTIONS.md` (session draft) — fault taxonomy consuming §4's code space.
- [docs/reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md) — session report; verdict ids cited throughout (C10, N13, N14, N19, N20, N29, N38, N39, A5, A6, A8) resolve there.
