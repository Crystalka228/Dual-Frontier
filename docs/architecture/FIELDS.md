---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-FIELDS_V2
category: A
tier: 1
lifecycle: AUTHORED
owner: Crystalka
version: "0.1.0"
next_review_due: post-ratification closure
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-FIELDS_V2
---
# Field Storage

The storage contract for spatial scalar/vector fields: native `RawTileField` layout, the `df_world_field_*` C ABI, the managed `FieldRegistry`/`FieldHandle<T>` bridge, field identity, capability verbs, and lifecycle — the substrate the Vulkan compute layer sits on top of.

> **Document class: authored-rework (current-truth candidate).** Successor of `docs/architecture/historical/FIELDS.md` (DOC-A-FIELDS, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. Becomes the LOCKED authority upon Crystalka ratification per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7; until then the predecessor remains the last-ratified reference and prevails on conflict.
> **Ratification checklist:** [ ] content spot-audit at ratification HEAD · [ ] lifecycle AUTHORED → LOCKED, version → 1.0.0 · [ ] `next_review_due` set · [ ] predecessor register rationale updated · [ ] VULKAN_SUBSTRATE.md (successor) ratified — §9/§10 cite its exclusion-policy and К-L7.1 sections as pointers, not restatements.

## Status

| Field | Value |
|---|---|
| Role | normative-current-candidate |
| Successor of | `docs/architecture/historical/FIELDS.md` (DOC-A-FIELDS, Live v0.1.1, now SUPERSEDED) |
| Scope | Native `RawTileField` layout/mutation rules; the `df_world_field_*` C ABI; the managed `FieldRegistry`/`FieldHandle<T>`/`FieldSpanLease<T>` bridge; field identity/namespacing; the `field.*` capability grammar and its enforcement points; field lifecycle as actually wired. |
| Non-goals | Field mathematics, compute pipeline registration/dispatch, hardware exclusion policy detail (VULKAN_SUBSTRATE.md). Save-file format/I/O (PERSISTENCE_SNAPSHOT_CONTRACT.md, AUTHORED draft). Mod-specific gameplay decisions. |
| Authority domains | **storage/memory**, field-slice (layout, ping-pong, conductivity, storage flags, mutation-rejection). **entity-identity**, field-slice (string field-id namespacing, distinct from `EntityId`). **mod-lifecycle**, capability-slice (`field.*` grammar and enforcement). |
| Defers to | VULKAN_SUBSTRATE.md · KERNEL_ARCHITECTURE.md Part 0 · MOD_OS_ARCHITECTURE.md · PERSISTENCE_SNAPSHOT_CONTRACT.md (AUTHORED draft) · ECS.md — full table in Cross-references. |

---

## §1 Why two systems, not one

Two orthogonal data systems live under the managed Application layer. ECS ([ECS.md](./ECS.md)) stores per-entity state as sparse component arrays; Field Storage stores spatial state as dense 2D grids. A pawn is an entity with components; a mana density is a field with cells — code reading one never touches the other. Both share the native kernel (KERNEL_ARCHITECTURE.md Part 0) as storage owner and `IModApi` (MOD_OS_ARCHITECTURE.md §4.6) as registration surface, but access, lifecycle, and capability verbs are disjoint. A single "World" model absorbing both loses either way: "one entity per cell" bloats the entity table by `width×height` per field (a 200×200 field is 40,000 entities), while "an entity as a grid row" loses the per-entity model where a pawn carries unrelated, ungridded components.

| Aspect | ECS | Field Storage |
|---|---|---|
| Identity | `EntityId` (index + version) | `(field_id, x, y)` cell coordinate |
| Storage layout | SparseSet | Dense 2D array, every cell present |
| Mutation | `WriteBatch<T>` stages, flushed at phase boundary | Point write (rejected during active spans) or compute dispatch |
| Capability verbs | `read`, `write` | `field.read/write/acquire/conductivity/storage/dispatch` |

(`WriteBatch<T>` — `src/DualFrontier.Core.Interop/WriteBatch.cs:49` — is current; the predecessor's `WriteCommandBuffer` name does not exist on disk.)

## §2 Scope

This is the **storage contract**: the substrate VULKAN_SUBSTRATE.md's compute primitives (V0/V1/V2) sit on. The storage path has no GPU dependency itself — a field is a plain native allocation whether or not a shader touches it. Out of band: **field mathematics** (VULKAN_SUBSTRATE.md is the spec layer); **compute pipeline registration/dispatch** (`IModApi.ComputePipelines`, unwired for mods — §10–§11); **mod-specific gameplay decisions**.

## §3 Field — the unit of storage

A field is a typed dense 2D array bound to a registered string id, fixed structural shape regardless of element type (`tile_field.h:1-30`, allocation `tile_field.cpp:5-19`):

- **Primary buffer** — `width×height×cell_size` bytes, zero-filled at construction.
- **Back buffer** — identical layout, ping-pong target for compute kernels.
- **Conductivity map** — `width×height` floats, default `1.0` (uniform isotropic).
- **Storage-flag bitmap** — `(width×height+7)/8` bytes, byte-packed, default clear; marks capacitance-retaining cells.

Element type `T` is `unmanaged` (K-L3's blittable-layout requirement, KERNEL_ARCHITECTURE.md Part 0): `float`, `Vector2`, `int32` are common; modder types are accepted if blittable. Dimensions/cell-size must be positive or the constructor throws `std::invalid_argument`.

**Why ping-pong from day one.** Reading and writing the same buffer during a diffusion update produces order-dependent Gauss-Seidel semantics; GPU compute parallelises across cells with no order guarantee (Jacobi), and the CPU reference kernel serving as the shader equivalence oracle (§10) must match it. Ping-pong is the only model that survives that requirement — the back buffer is allocated at registration, never lazily.

**Why conductivity map and storage flags from day one.** Retrofitting anisotropy onto an isotropic-only field forces a storage migration (buffer growth, new C ABI entry, new managed method, every field-aware test updated). The fixed cost of carrying both from the start (roughly +25% footprint over a bare `float` field — VULKAN_SUBSTRATE.md §7.4 has current budget figures) beats a flag-day migration later.

## §4 Identity and namespacing

A field id is `<mod-namespace>.<field-name>`, e.g. `vanilla.magic.mana`; `<mod-namespace>` must equal the registering mod's manifest id (MOD_OS_ARCHITECTURE.md §3.3; enforced at `RestrictedFieldApi.cs:41-46` — `RegisterField<T>` throws `CapabilityViolationException` if the id doesn't start with `_modId + "."`).

Identity is string-, not numeric-, keyed on purpose: component type ids are kernel-local sequential integers assigned by mod-load-order and are **not** stable across a mod reload (a mod reloaded twice could see its field assigned id 7, then 9); string ids are (`world.cpp:576-597`, `FieldRegistry.cs`). PERSISTENCE_SNAPSHOT_CONTRACT.md (AUTHORED draft) PS-7 holds this up as the identity model the rest of the corpus should converge to.

**Registration conflict — corrected.** Two *different* mods cannot collide on an id, but not via a dedicated exception: `RegisterField<T>` rejects any id outside the caller's own namespace before the registry is reached, and manifest `id` is asserted globally unique (MOD_OS_ARCHITECTURE.md §2.2), so no two mods can share a prefix. The only real collision is the *same* mod re-registering its own id with different type/dimensions: `FieldRegistry.Register<T>` (`:42-51`) treats an identical `(id, width, height, type)` re-registration as an idempotent no-op returning the cached handle, and throws `InvalidOperationException` on mismatch — before native is called. Native `World::register_field` enforces the same rule independently (`std::invalid_argument`, `world.cpp:585-593`), caught at the ABI boundary and reported as `0` (`capi.cpp:742-747`).

MOD_OS_ARCHITECTURE.md §11.2 lists `FieldRegistrationConflict` only among "documented-but-reserved" `ValidationErrorKind` names (`:1044`) — **not a member of the shipped enum.** The conflict is real and structurally prevented as above; the named exception type does not exist.

Other mods reach a field via `IModApi.Fields.GetField<T>(id)` (§7): the accessing mod must declare the applicable `field.read` capability and list the owning mod in `dependencies` (MOD_OS_ARCHITECTURE.md §3.4).

## §5 Native layer — `RawTileField`

Shipped (`tile_field.h`/`.cpp`). Surface: `width()`/`height()`/`cell_size()`; `read_cell`/`write_cell` (bounds-checked point access, `0` on failure); `acquire_span`/`release_span` (atomic-counted); `set/get_conductivity`, `set/get_storage_flag`; `swap_buffers` (throws if any span active). Private state: `data_`, `back_buffer_`, `conductivity_`, `storage_flags_` (`std::vector`), plus `std::atomic<int32_t> active_spans_{0}` (`tile_field.h:32-83`). No `gpu_buffer_`/`gpu_descriptor_` members exist — GPU-side buffer binding is external orchestration owned by the Runtime/Compute layer (§10), not a field of this class.

`World` (`world.h`) carries the field registry alongside its component-store map — `stores_` (`:166`) and `fields_: unordered_map<string, unique_ptr<RawTileField>>` (`:180`, K9) — with `register_field`/`get_field`/`unregister_field`/`field_count` as `World` methods (`:119-125`, bodies `world.cpp:576-617`).

**Mutation rejection contract.** While any span is acquired (`active_spans_ > 0`), `write_cell`, `set_conductivity`, `set_storage_flag`, and `swap_buffers` throw `std::logic_error` (`throw_if_spans_active`, `tile_field.cpp:21-26`, checked at `:43,70,87,112`). All ten `df_world_field_*` C ABI entry points wrap their native call in `catch (...) { return 0; }` (`capi.cpp:737-899`) — the exception never crosses the DLL boundary. Same span/mutation-exclusion discipline K-L7 establishes for components: a span is a read view, mutations are explicit, races are structurally impossible. Compute dispatches are **not** gated by `active_spans_` — a dispatch has its own fence-based sync (§10).

**Span lifetime.** `acquire_span` returns a pointer to the primary buffer and increments `active_spans_`; valid until `release_span` decrements it or a mutating call throws (fails before any state change, `tile_field.cpp:51-65`). The managed wrapper exposes it read-only (`FieldSpanLease<T>.Span : ReadOnlySpan<T>`, §7). Bulk writes go through compute dispatch, not span mutation.

## §6 C ABI extension — `df_world_field_*`

Shipped, `df_capi.h:454-523`: `df_world_register_field`, `_field_unregister`, `_read_cell`, `_write_cell`, `_acquire_span`, `_release_span`, `_set_conductivity`, `_get_conductivity`, `_set_storage_flag`, `_get_storage_flag`, `_swap_buffers`, `_count` — twelve entry points, all taking `df_world_handle world, const char* field_id` plus operation-specific arguments, all returning `int32_t` (`float` for the conductivity getter).

**Field id is `const char*`, verified.** The predecessor's own listing already matched this. A numeric `uint32_t field_id` sketch survives only inside VULKAN_SUBSTRATE.md §3.4's illustrative C ABI block (an internal drift in *that* document); it does not change the shipped signature, independently confirmed against `df_capi.h` and the managed P/Invoke layer (`NativeMethods.Fields.cs:19-83`, marshalling every field id as a stackalloc'd UTF-8 `byte*`).

All bounds/size/not-found/active-span failures return `0` (`0.0f` for the float getter); every function bounds-checks before touching memory (`tile_field.cpp:28-108`) and is exception-wrapped at the ABI (§5). Naming (`df_world_field_*`) distinguishes field-bound calls from `df_world_*_component` calls.

Compute-dispatch/pipeline-registration entry points (`df_world_field_dispatch_compute`, `df_world_register_compute_pipeline`, `df_world_attach_vulkan`, same header, `:551-577`) are V0.B/V1 substrate primitives, not field-storage primitives — see §10.

## §7 Managed bridge — `FieldRegistry` and `FieldHandle<T>`

Shipped (`FieldRegistry.cs`, `FieldHandle.cs`). `FieldRegistry` (per-`NativeWorld`, constructed at `NativeWorld.cs:88,109`, guarded by a plain `lock` since the kernel is single-threaded per K-L7): `Register<T>`, `Get<T>`, `TryGet<T>`, `IsRegistered`, `Unregister` (§9 — no production caller). `FieldHandle<T> : IFieldHandle`: `ReadCell`/`WriteCell`, `AcquireSpan()` → `FieldSpanLease<T>`, `SetConductivity`/`GetConductivity`, `SetStorageFlag`/`GetStorageFlag`, `SwapBuffers()`. **No `DispatchCompute` method exists on this type** — see §10.

Every field id crosses the ABI as a stackalloc'd or heap UTF-8 buffer (`FieldRegistry.cs:55-64`, every `FieldHandle.cs` method). A non-success native return raises `FieldOperationFailedException` uniformly for out-of-bounds, size mismatch, not-found, and active-span mutation (`FieldHandle.cs:246-249`).

`FieldSpanLease<T>` (`:195-239`) is a stack-only `ref struct`: `ReadOnlySpan<T> Span`, an indexer, manual `Dispose()` (a `ref struct` cannot implement `IDisposable`) calling `df_world_field_release_span`.

Mods never see `FieldHandle<T>` directly — the contract type is `IFieldHandle` (`Id`, `Width`, `Height`, `ElementType`), since the concrete generic lives in `Core.Interop`, which `Contracts` cannot reference without inverting the dependency direction. Mods downcast at the call site.

## §8 IModApi wiring — Fields and Compute Pipelines

The capability grammar carries **one** shape: `<provider>.field.<verb>:<field-id>`, `provider` = `kernel` or `mod.<modId>`, `verb` ∈ `{read, write, acquire, conductivity, storage, dispatch}` — verbatim from the validated pattern (`ManifestCapabilities.cs:25`, MOD_OS_ARCHITECTURE.md §3.2/§2.3 step 6). The predecessor carried two incompatible shapes across its own sections (a bare `field.*:<id>` with no provider, and a `mod.<id>.field.read:<id>` form); the form above is the sole one the parser accepts and the sole one `RestrictedFieldApi` ever constructs.

`kernel.field.*` is grammar-legal but semantically dead: MOD_OS_ARCHITECTURE.md's changelog frames it as an "infrastructure verb" for a hypothetical kernel-owned field, but `KernelCapabilityRegistry.BuildFromKernelAssemblies()` (§3.5) never emits a field token, and `RestrictedFieldApi` never checks one — it always resolves `mod.<owning-modId>.field.<verb>:<field-id>` (`RestrictedFieldApi.cs:41-46,58-84`), consistent with §9's "no engine-special field."

**Enforcement is two-point, not three.** The predecessor described a third, load-time `[FieldAccess]` attribute paralleling `[SystemAccess]`; no such attribute exists on disk. Real points: **(1) manifest parse** — every token validated against the regex above (`ManifestCapabilities.Parse`, MOD_OS_ARCHITECTURE.md §2.3 step 6); **(2) runtime, at acquisition** — `RegisterField<T>` requires the caller's own-namespace `field.write` token, `GetField<T>` requires `field.read` against the owning namespace (`RestrictedFieldApi.cs:36-84`), a miss throws `CapabilityViolationException`. Per-cell traffic through an already-acquired handle is **not** re-checked — gated once, at acquisition (MOD_OS_ARCHITECTURE.md §4.6.2: "no `[FieldAccess]`-style load-time attribute cross-check exists").

**Nullability, verified.** `IModApi.Fields` is non-null only when the loader supplies a `FieldRegistry`; production `ModIntegrationPipeline` constructs `RestrictedModApi` without one today, so mods loaded through the live pipeline observe `Fields == null` (§4.6.1) — the field stack is exercised by tests, not the production mod-load path. `IModApi.ComputePipelines` is unconditionally `null`: hardwired at `RestrictedModApi.cs:216`; `IModComputePipelineApi` has zero implementing types in the solution.

> **FENCED (target/planned):** a load-time `[FieldAccess]`-equivalent cross-check is not scheduled by name in any reviewed document — unscoped design space, not a near-term commitment.

## §9 Lifecycle

**Designed contract.** A mod registers fields during `IMod.Initialize(api)`; fields persist while loaded; on mod unload the registry deregisters every field the mod owned and native releases the buffers; on shutdown, `World::fields_` releases every field through ordinary C++ destruction. The kernel owns no field itself — "vanilla = mods" means even mana/electricity/water fields belong to vanilla mods.

**Verified wiring — the unload half is not built.** `FieldRegistry.Unregister(string)` (`:104-120`) is exercised only by `FieldRegistryTests.cs:91,102` — no call site exists anywhere in production `src/`. The real §9.5 unload chain, `ModIntegrationPipeline.UnloadMod` (`:561-592`), unsubscribes buses, clears managed component stores, invokes the native per-mod bus/wake cleanup primitive (EVENT_BUS.md), and unloads the ALC — it never touches `FieldRegistry`. A mod's fields remain registered, content intact, straight through an ALC unload.

**Corollary for reload.** Since nothing clears a field on unload, and `Register<T>` treats an identical re-registration as an idempotent no-op returning the existing handle, a mod reloading with the same field shape does not get a fresh field — `Initialize` re-runs, calls `RegisterField`, and receives the *same* handle over the *same*, still-populated buffer. This is the reverse of "content does not survive reload": content survives *by omission*, not by design — the same shape of gap the corpus documents for mod/engine shutdown generally, not fields-specific.

A field is destroyed today only by explicit `FieldRegistry.Unregister` (design-available, production-unused) or whole-process/native-`World` teardown via RAII (not deterministically triggered in production — an engine-lifecycle concern, out of scope here).

## §10 CPU and GPU paths — exclusion, not fallback

The predecessor described a transparent runtime choice ("the runtime decides whether `DispatchCompute` issues a Vulkan compute dispatch or runs the CPU reference kernel. The mod doesn't care."). **That is not the shipped policy** — VULKAN_SUBSTRATE.md §7.1: "The shipped policy resolves this by **exclusion, not fallback**."

- **K-L19 fail-fast (shipped).** `Runtime.Create` runs `HardwareCapabilityCheck.Verify` at startup and throws `HardwareCapabilityException` if the Vulkan 1.3 + async-compute-queue tier is absent (`HardwareCapabilityCheck.cs`, `HardwareCapabilityException.cs`). The game does not start on excluded hardware — no per-dispatch runtime choice exists.
- **`CpuKernels/*` are an equivalence oracle, not a runtime path.** `IsotropicDiffusionKernel`/`AnisotropicDiffusionKernel` (`src/DualFrontier.Core.Interop/CpuKernels/`) drive the V1 equivalence test suites, runnable without a GPU: "this kernel exists as the GPU equivalence oracle, not as a performance target" (`IsotropicDiffusionKernel.cs:20-23`). Also the intended source of CPU-canonical field state for future saves (§12; PS-15) — never a config-selected alternate dispatch path, which is design-only and not on disk.

**Sync dispatch blocks; it does not return early.** The predecessor's "field dispatches are non-blocking" claim inverts the shipped fence semantics: the К-L7 default sync path "returns after the fence signals, so a subsequent `FieldHandle<T>.ReadCell` sees the dispatched result" (VULKAN_SUBSTRATE.md §2.3/§2.3.1) — the calling thread blocks until the GPU signals completion. An opt-in К-L7.1 pipeline-managed path trades that per-call fence wait for a bounded one-tick slot-tail lag; that mechanism belongs entirely to К-L7.1 (KERNEL_ARCHITECTURE.md Part 0) and VULKAN_SUBSTRATE.md §2.3.1/§7.3.0 — not restated here.

A field remains a single native allocation regardless of GPU involvement — `RawTileField` carries no GPU handles (§5). GPU-side buffer binding is external orchestration owned by the Runtime/Compute layer (`FieldStorageBinding.cs`, `ComputePipelineRegistry`) — mechanism in VULKAN_SUBSTRATE.md §3.4, not restated here.

## §11 Compute dispatch — mod-facing surface

> **FENCED (target/planned — not current truth):** design intent is a mod-owned field calling a mod-registered pipeline (`field.DispatchCompute(pipeline, pushConstants, iterations)`), symmetric across excluded-at-startup and present hardware.
>
> **Verified current state:** `FieldHandle<T>` carries no `DispatchCompute` method (§7). `IModApi.ComputePipelines` is hardwired `null` (§8). `IModComputePipelineApi` has zero implementing types. `docs/ROADMAP.md`'s "V substrate — M-V demonstrations" row names the same gap: "mod-facing compute surface unwired (`RestrictedModApi.ComputePipelines => null`)". The native C ABI and the engine-internal V1 diffusion pipeline exist and are exercised by engine code/tests — the gap is specifically the *mod-facing* surface. Full detail: VULKAN_SUBSTRATE.md §3.3/§3.4.

## §12 Save / load

> **FENCED (target/planned — not current truth).** No field save/load path exists on disk. PERSISTENCE_SNAPSHOT_CONTRACT.md (AUTHORED draft) is the normative-target composition; this section carries forward field-specific intents as fenced candidates, superseded on conflict:
>
> - One blob per field: `(field_id, owning_mod_id, width, height, cell_size, blob)` — primary buffer + conductivity + storage flags. Back buffer is **not** serialized; restores to zero on load — PERSISTENCE_SNAPSHOT_CONTRACT.md §2 rows "Fields: primary buffer…"/"…back buffers", both citing this document's predecessor as originating authority.
> - Width/height mismatch between save and current registration is a load error (§2 "Config / map dimensions" row).
> - Serialized bytes are the §10 equivalence-oracle CPU-canonical state, not raw GPU output — PS-15 (§6), since GPU compute is not bit-exact across hardware/drivers (VULKAN_SUBSTRATE.md §7.2.1). Explicitly **not implemented** today.
> - Field ids are cited at PS-7 (§3) as the identity model the rest of the corpus's serialized-identity law should converge to.
>
> No independent authority; PERSISTENCE_SNAPSHOT_CONTRACT.md §2/§3/§6 is normative once ratified.

## §13 Anti-patterns

**A field per entity.** A naive "one mana value per pawn" needs a component plus an ambient field plus linking arithmetic. The field model dissolves this: one mana field; a caster reads `mana.ReadCell(pawn.x, pawn.y)`. Same for electricity, water, heat, sound, scent.

**A component holding a `FieldHandle<T>`.** Registry-bound; caching it ties component memory to mod-managed lifetime, and — per §9's verified gap — an unloaded-then-reloaded mod's cached handle may point at stale data rather than fail cleanly. Fetch through `IModApi.Fields` per call instead.

**Direct point-write loop instead of compute dispatch.** A per-cell `WriteCell` loop over a 200×200 grid is 40,000 P/Invoke crossings. Bulk updates belong on the compute path (§10–§11); point writes are for genuinely sparse mutations.

**A mod that registers a field without providing it.** Registering without declaring `mod.<id>.field.read:<id>` in `capabilities.provided` makes the field invisible to every other mod — the declaration is the only path another mod's `GetField` can resolve through (§4, §8). Enforced by the loader's static check (MOD_OS_ARCHITECTURE.md §3.4).

---

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) | defers-to | Field math; compute pipeline mechanics; hardware exclusion (К-L19); К-L7/К-L7.1 sync and slot-tail semantics. |
| [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) | defers-to | Part 0: К-L3 (unmanaged constraint), К-L7/К-L7.1, К-L19 (hardware tier). |
| [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) | defers-to | Capability grammar (§3.2), manifest validation (§2.3), namespaces (§3.3), `IModApi` v3 (§4.6), `ValidationErrorKind` (§11.2), unload chain (§9.5). |
| [PERSISTENCE_SNAPSHOT_CONTRACT](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (AUTHORED draft) | defers-to | Normative-target save/load composition; supersedes §12 on conflict. |
| [ECS](./ECS.md) | cites | The orthogonal storage system — entities, components, `WriteBatch<T>`. |
| [EVENT_BUS](./EVENT_BUS.md) | cites | The native per-mod unload primitive in §9 also clears bus/wake state. |
| [PERFORMANCE](./PERFORMANCE.md) | cites | Field memory budget, dispatch timing targets. |
| [EXECUTION_AUTHORITY_MATRIX](./EXECUTION_AUTHORITY_MATRIX.md) (AUTHORED draft) | cites | Any future §11 mod-facing compute cutover gate belongs there. |

## Amendment protocol

A correction to a verified code claim (file:line, exception type, wiring state) may be spot-fixed against current HEAD without a version bump beyond PATCH. A change to the storage contract itself (buffer layout, C ABI shape, capability grammar) is MINOR/MAJOR and requires re-verifying every citing document plus owner sign-off per FRAMEWORK.md §7.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.0 (this doc) | 2026-07-15 | Corpus rework: killed the transparent CPU/GPU fallback framing (exclusion-not-fallback); fixed "non-blocking dispatch" to the verified sync-blocks-until-fence truth; corrected the capability grammar to the single code-verified shape; corrected `FieldRegistrationConflict` to its reserved-not-implemented status; corrected the mod-unload sweep and reload-survival claims against a verified zero-call-site finding; corrected the C ABI field-id type and `WriteCommandBuffer`→`WriteBatch<T>` naming; fenced mod-facing compute-dispatch and save/load as target-only. |
| 0.1.1 | pre-rework | Last state of predecessor `DOC-A-FIELDS` (see historical/) — Live, K9 storage contract HOLDS, GPU-facing sections stale pre-Q-G-2. |
