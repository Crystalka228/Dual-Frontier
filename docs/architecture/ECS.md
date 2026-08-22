---
register_id: DOC-A-ECS_V2
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.3.1
first_authored: 2026-07-15
last_modified: '2026-08-22'
content_language: en
next_review_due: 2027-Q3
title: Entity Component System (authored rework; version-0 identity defect documented)
supersedes:
- DOC-A-ECS
review_cadence: on-change+annual
last_review_date: 2026-07-17
last_review_event: 'PATCH 1.3.0 -> 1.3.1 2026-08-22 (ID_B_ENTITY_VERSIONS, PR #51 review fixes): section 6 records the pending-destroy TOMBSTONE and that the create refusal is an exception rather than a silent EntityId.Invalid; section 5 gains the hazard the fix nearly created -- reading TRUE versions is not automatically safe, because between DestroyEntity and the flush the component row survives and the naive reconstruction named the pair a recycle would mint, which is_alive accepted. Caught in review of the cascade that introduced it. Prior review: MINOR 1.2.0 -> 1.3.0 2026-08-22 (ID_B_ENTITY_VERSIONS C7): section 5 flips from KNOWN DEFECT to RESOLVED -- the span ABI now carries versions via SpanLease.Versions over the native df_world_acquire_versions view, all three pair-iterators reconstruct true generations, every production fabrication site is migrated, and DFK022 (Error) makes the prohibition structural; the two-stage history (Version=1, then Version=0) is retained because the shape of the mistake is the lesson, and one silent trap is documented in its place -- Versions is ENTITY-INDEX-keyed while Span/Indices are dense-keyed, so versions[i] compiles and is wrong. Section 4''s canonical example gains the versions-view idiom with that trap called out inline. Section 6 records that creation is refused while a versions view is held and that EntityId.IsValid is now Index > 0. Section 8''s anti-pattern points at the analyzer rather than at an inventory. F-59 CLOSED. No lifecycle transition (LOCKED). Prior review: MINOR 1.1.0 -> 1.2.0 2026-08-20 (W3_WEATHER_SLICE C8): section 5 records the C5b correction -- SpanLease.Pairs, the WriteBatch enumerator and Sdk/SpanScope.Pairs fabricated Version=1, a version no entity carries, so batched writes keyed on a span id were dropped at flush silently; all three now fabricate 0 like every other site, narrowing but NOT resolving the C10/N-22 defect. Section 5 also records that ISystemContext.DestroyEntity puts destruction in MOD hands, so the creation-only latency argument no longer rests on engine call sites. Section 6 gains the SDK entity-lifecycle surface (liveness immediate, storage reclamation deferred, no flush member promoted). Section 8 adds the id-is-not-an-engine-reference clarification. No lifecycle transition (LOCKED).'
reviewer: Crystalka
special_case_rationale: Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION (checklist item [1]). Successor of DOC-A-ECS per EVT-2026-07-15-CORPUS_REWORK_R1_KERNEL_CORE; session C10 teaching defect fixed (no EntityId(index,0) fabrication in examples).
---

# Entity Component System

The entity/component storage model: `NativeWorld` as the single production backend, dense sparse-set storage, the span/batch access protocol, entity identity and lifecycle semantics, and `SystemBase`.

> **Ratified successor (LOCKED v1.0.0 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION, 2026-07-17).** Successor of `docs/architecture/historical/ECS.md` (DOC-A-ECS, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`.

## Status

| Field | Value |
|---|---|
| Role | normative (ratified successor) |
| Successor of | `docs/architecture/historical/ECS.md` (DOC-A-ECS) |
| Scope | Entity/component storage as it exists in code: `NativeWorld` surface, dense-storage rationale, span/batch protocol, identity/lifecycle semantics (including the fabricated-version defect), `SystemBase`, anti-patterns |
| Non-goals | Field storage (FIELDS.md); scheduling ([THREADING.md](./THREADING.md)); Path β mod-API detail (MOD_OS_ARCHITECTURE.md); target identity/ABI law (IDENTITY_AND_ABI_CONTRACT.md); persistence (PERSISTENCE_SNAPSHOT_CONTRACT.md, AUTHORED draft) |
| Authority domains | storage access-pattern teaching; entity lifecycle semantics (descriptive, code-anchored). Storage-path invariant text (К-L3/К-L3.1/К-L8/К-L11) stays [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md)'s |
| Defers to | [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) storage invariants · [THREADING.md](./THREADING.md) phase/dispatch law · [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) Path β / mod lifecycle · [IDENTITY_AND_ABI_CONTRACT.md](./IDENTITY_AND_ABI_CONTRACT.md) target identity law |

## §1 NativeWorld — the single production backend

Dual Frontier uses classical ECS: an entity is an identifier, components are pure data, systems are logic. `NativeWorld` (`src/DualFrontier.Core.Interop/NativeWorld.cs:28`) is the **sole production component-storage backend** after the К8.3+К8.4 cutover (A'.5 closure 2026-05-14) — the К-L11 single-source-of-truth invariant, analyzer-policed by DFK011 ([ANALYZER_RULES.md](./ANALYZER_RULES.md) §1.1). The prior managed `World` is retired and survives only as a test fixture (`tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs`). Production constructs the world via `Bootstrap.Run(useRegistry: true)` (`GameBootstrap.cs:76`) and hands it to `ParallelSystemScheduler` (`:192-199`); systems reach it through `SystemBase.NativeWorld` (§6).

The surface systems use — signatures verified; the constraint is `unmanaged`, not `unmanaged, IComponent` (the `IComponent` marker sits on component types, not on this API):

```csharp
// Verified against src/DualFrontier.Core.Interop/NativeWorld.cs at HEAD.
public SpanLease<T> AcquireSpan<T>() where T : unmanaged;              // :345
public WriteBatch<T> BeginBatch<T>() where T : unmanaged;              // :376
public bool TryGetComponent<T>(EntityId id, out T v) where T : unmanaged; // :166
public bool HasComponent<T>(EntityId id) where T : unmanaged;          // :190
public EntityId CreateEntity();                                        // :120
public void DestroyEntity(EntityId id);      // :127   IsAlive :133   FlushDestroyedEntities :148
public void AddComponents<T>(…entities, …components);                  // :225 (bulk, one P/Invoke)
public InternedString InternString(string content);                    // :549 — K8.1 primitives:
public NativeMap<TK,TV> CreateMap<TK,TV>();  // :720   CreateSet :730  CreateComposite :738
```

Path β managed-class storage (К-L3.1 bridge) lives in per-mod `ManagedStore<T>` via `SystemBase.ManagedStore<T>()` — see [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md), Path β section.

## §2 EntityId and components

**EntityId** is `readonly record struct EntityId(int Index, int Version)` (`src/DualFrontier.Contracts/Core/EntityId.cs:21`), sentinel `Invalid = default` (0, 0) (`:28`). The version increments on destroy (§6), making cached references safely invalid: a stale id fails `TryGetComponent` and the system skips it — no crash. Verified drift: managed `IsValid => Index > 0 || Version > 0` (`EntityId.cs:38`) accepts ids like `(0, 5)` that native `is_alive` rejects unconditionally (`index <= 0` permanently dead, `world.cpp:75`); the alignment fix is [IDENTITY_AND_ABI_CONTRACT.md](./IDENTITY_AND_ABI_CONTRACT.md) §2's. `IsValid` is syntactic only — aliveness is answered exclusively by `NativeWorld.IsAlive`.

**Components** carry no logic, only data. Path α (preferred): an `unmanaged` struct implementing `IComponent` (`src/DualFrontier.Contracts/Core/IComponent.cs:10`) — POCO-serializable, batch-readable concurrently while no one writes. Path β: a class with `[ManagedStorage]` (`ManagedStorageAttribute.cs`) in per-mod managed storage — runtime-only, never persisted (К-L3.1 lock). Validation and arithmetic live in systems.

## §3 Dense storage — why not an array

A naive `T[]` per component wastes memory (most entities lack a given component); a sorted list costs O(log n). The kernel stores components in a **sparse set** — the EnTT/bevy_ecs/flecs pattern: `sparse_[entity_index]` → dense slot or −1; `dense_` packs live values contiguously; `dense_to_index_` maps back for swap-with-last erase (`native/DualFrontier.Core.Native/include/sparse_set.h:20-23`). Insert O(1), remove O(1), iteration O(N) with no gaps. The C ABI store is the type-erased `RawComponentStore` over this pattern (`component_store.h:13-25`). Iteration is what matters at 10-20k entities: `acquire_span` hands systems `dense_data()` plus the parallel entity-index array directly (`world.cpp:231-248`) — zero copies.

## §4 Span/batch access pattern

`AcquireSpan<T>()` returns a `SpanLease<T>`: `Span` (read-only dense component view, `SpanLease.cs:50`), `Indices` (parallel entity-**index** array — indices only, no versions, `:63`), `Count` (`:44`). `BeginBatch<T>()` returns a `WriteBatch<T>` recording commands (`Update`/`Add`/`Remove`, `WriteBatch.cs:79,93,106`) applied atomically at `Flush` (`:122`) or auto-flushed on `Dispose` (`:163-170`); `Cancel` discards (`:139`).

**Mutation-rejection contract.** While any span or batch is active, direct mutations (`AddComponent`/`RemoveComponent`/`DestroyEntity`/`FlushDestroyedEntities`/`AddComponents`) are silently no-op'd — the native throw is caught at the C ABI boundary (`NativeWorld.cs:337`; `WriteBatch.cs:39-47`; rejection sites `world.cpp:85-88,96-98,112-115`). Dispose leases before mutating. Recorded batch commands are invisible until `Flush` (`WriteBatch.cs:196`).

The canonical read pattern — **bulk work walks the span without entity identity; per-entity identity operations use ids the world actually issued** (factory returns, event payloads), validated via `TryGetComponent`/`IsAlive`:

```csharp
[SystemAccess(reads: new[] { typeof(HealthComponent), typeof(PositionComponent) },
              writes: Array.Empty<Type>(), bus: nameof(IGameServices.Combat))]
public sealed class HealthReporterSystem : SystemBase
{
    private readonly List<EntityId> _wounded = new(); // ids from DamageEvent payloads

    public override void Update(float delta)
    {
        // Bulk pass: dense span walk. When no EntityId is needed, don't build one —
        // health[i] is the whole point of a span.
        using (SpanLease<HealthComponent> lease = NativeWorld.AcquireSpan<HealthComponent>())
        {
            ReadOnlySpan<HealthComponent> health = lease.Span;
            for (int i = 0; i < lease.Count; i++)
            { /* aggregate over health[i] */ }
        }

        // When an identity IS needed, take it from the world's version table —
        // never invent one (§5, К-L22, DFK022 Error).
        using (SpanLease<HealthComponent> lease = NativeWorld.AcquireSpan<HealthComponent>())
        {
            ReadOnlySpan<int> indices  = lease.Indices;   // dense-keyed
            ReadOnlySpan<int> versions = lease.Versions;  // ENTITY-INDEX-keyed
            for (int i = 0; i < lease.Count; i++)
            {
                int index = indices[i];
                var id = new EntityId(index, versions[index]);
                // `versions[i]` would compile and read an unrelated slot — the two
                // spans are not parallel. `lease.Pairs` says the same thing shorter.
            }
        }

        // Per-entity pass: identity came from the world (event payload).
        foreach (EntityId id in _wounded)
        {
            if (!NativeWorld.TryGetComponent(id, out PositionComponent pos))
                continue; // destroyed since the event — stale id fails closed
            // report pos …
        }
        _wounded.Clear();
    }
}
```

Enforcement note: there is **no runtime permission check** relating this code to the `[SystemAccess]` declaration — the per-access runtime guard was deleted at К8.3+К8.4. The declaration is consumed at registration for graph edge-building; call-site conformance is convention plus the analyzer program, which does not yet police `[SystemAccess]` completeness ([THREADING.md](./THREADING.md), execution-contexts section). The predecessor's example comment "Access is permitted — both components are declared in reads" implied a runtime check and is removed; its `Components` property and reads-only `[SystemAccess]` overload never existed either (`Span`, `SpanLease.cs:50`; ctors require `reads`/`writes`/`bus`, `SystemAccessAttribute.cs:55-74`).

## §5 Fabricated entity versions — the C10 / N-22 defect, RESOLVED (ID_B, 2026-08-22)

**Current truth.** The span ABI now carries versions. `SpanLease<T>.Versions` is a read-only view over the world's per-slot version table (native `df_world_acquire_versions`, acquired with the component span and released with it), and `SpanLease<T>.Pairs`, the `WriteBatch<T>` enumerator and the SDK-facing `Sdk/SpanScope<T>.Pairs` all reconstruct `new EntityId(idx, versions[idx])` — the generation the slot actually holds. Every production fabrication site was migrated in the same cascade; the law is seated as К-L22 ([KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) Part 0) and enforced by **DFK022** at Error, so a new fabrication fails the build. Exactly one waived site remains: `EntityEncoder.DecodeRanges`, which decodes persisted INDEX ranges with no world to ask — version truth across the save boundary is the A7 contract's call, named in the waiver as its retirement trigger. ROADMAP F-59 is CLOSED.

**The history below is retained deliberately.** The defect took two forms, each of which looked reasonable when written, and the shape of the mistake is worth more than a one-line "fixed" note.

**What it was.** The span ABI returned entity INDICES without versions, and the codebase — including this document's predecessor and the KERNEL_ARCHITECTURE example that taught the pattern — fabricated the missing half: ~18 production sites across 9 systems plus `GameBootstrap` constructed `new EntityId(indices[i], 0)`; `EntityEncoder` decoded saved ranges to version 0; the three pair-iterators fabricated too.

**W3 correction (2026-08-20), the first narrowing.** Those three pair-iterators previously fabricated `Version = 1`, which is the version NO entity ever carries at creation — versions start at 0 and only grow. That is strictly worse than fabricating 0: a version-0 id at least matches a never-recycled slot, whereas a version-1 id matched NOTHING, so the canonical read-span-then-write-batch loop recorded commands that the flush-time version check then dropped in silence. The W3 wave gate hit it end to end (a mod's per-tick component write persisted nothing while `Update` returned `true`), and W3_WEATHER_SLICE C5b aligned all three on 0 — the same fabrication every other site performs. `SpanWriteRoundTripTests` now pins the round trip: an id a span hands back must be usable as a write key. This narrows the defect to the shared one below; it does not resolve it.

**What the kernel actually enforces (N-22).** The generation machinery is real and fails closed: `is_alive` demands exact version equality — `id.version == versions_[id.index]` — `destroy_entity` bumps the version before the slot can recycle, and every accessor and mutation gates on it. But a fabricated version-0 id matched only a slot whose version was still 0 — never destroyed — so for those callers the ABA guarantee collapsed to "this index was never recycled." The defect was latent only while production was creation-only, and `ISystemContext.DestroyEntity` (W3, CONTRACTS.md §4.2) had already put destruction in the hands of MOD code, which `src/`-only greps do not see. The latency was therefore no longer guaranteed by anything the engine controlled — which is what made this the identity family's urgent half rather than a tidy-up.

**Wrong version of a live entity: fails closed.** Because `is_alive` is exact equality, an id with the right index but wrong version — too low *or* too high — is indistinguishable from dead: `TryGetComponent` returns `false`, writes are silently dropped. No index-only or nearest-version fallback exists. (Before the W3 correction above, `Pairs`' fabricated version 1 mismatched a fresh entity's true version 0 in exactly this way — every write keyed on it was dropped at flush, silently.)

**Do not add fabrication sites.** The prohibition is now structural rather than advisory: DFK022 is Error-enforcing, and a literal in the `Version` position fails the build. The mechanism and its rationale live at [IDENTITY_AND_ABI_CONTRACT.md](./IDENTITY_AND_ABI_CONTRACT.md) §2, which also records why the versions view carries its own guard counter rather than riding the component-span one.

**One trap survives the fix, and it is silent.** `Versions` is keyed by ENTITY INDEX; `Span` and `Indices` are keyed by dense position. `versions[indices[i]]` is correct and `versions[i]` compiles, reads an unrelated slot's generation, and produces an id that fails closed everywhere — exactly the symptom the fabricated version had. §4's example calls this out inline for that reason.

**And one hazard the fix nearly created.** Reading true versions is not automatically safe: between `DestroyEntity` and the flush the component row survives, so a span still contains it, and the naive reconstruction handed back the very pair `CreateEntity` mints on recycle — an id that `IsAlive` ACCEPTED, for an entity the caller had already destroyed. That is a fail-OPEN where the old fabricated version had been fail-closed by accident. It was caught in review of the cascade that introduced it (PR #51) and fixed by the tombstone described in §6. Recorded here because the lesson generalises: making a value truthful can widen what the value reaches, and the new reach needs its own audit.

## §6 Entity lifecycle

**Creation.** `NativeWorld.CreateEntity()` (`:120`), or the production bulk path — `RandomPawnFactory`/`ItemFactory` create entities and attach components via `AddComponents` in one P/Invoke (`ItemFactory.cs:144-150`). Natively the id comes from the free list (recycled index, current version) or `next_index_++`; index 0 is never live, which is also why `EntityId.IsValid` is `Index > 0` (ID_B alignment, IDENTITY_AND_ABI_CONTRACT §2). While a versions view is held — a `SpanLease` holds one for its lifetime — a creation that would GROW the entity table is REFUSED, because the resize would invalidate the view's pointer; creating from the free list or into spare capacity is permitted. The refusal is an **exception**, not a silent `EntityId.Invalid`: a sentinel returned quietly would let a caller attach components to a nonexistent entity and lose the spawn without a word.

**Destruction.** `DestroyEntity(id)` marks: the version increments immediately (`world.cpp:90`) — `IsAlive`/`TryGetComponent` fail from that moment — and the id joins a pending queue. Component removal and index recycling are deferred to `flush_destroyed` (`world.cpp:95-108`) via `FlushDestroyedEntities` (`NativeWorld.cs:148`); both are rejected while any span or batch is active (§4), and since ID_B also while any versions view is held — which a `SpanLease` now takes for its lifetime. **Between `DestroyEntity` and the flush the slot is TOMBSTONED**: its version-table entry goes negative, `IsAlive` rejects the slot outright, and an id a span reconstructs for the surviving row therefore fails closed. The flush lifts the tombstone to the destroyed entity's successor version immediately before the index becomes recyclable, so the pair a later recycle mints was never observable during the window — which is what keeps the ABA law (IDENTITY_AND_ABI_CONTRACT §1 note 1) true rather than merely intended. The predecessor tied removal to "the next scheduler phase boundary" — no such scheduler hook exists; flush runs when a caller invokes it.

**The SDK surface (W3).** A mod reaches all three through `ISystemContext` — `CreateEntity()` / `DestroyEntity(EntityId)` / `IsEntityAlive(EntityId)` — implemented by `SystemContextView` as straight delegations to the `NativeWorld` members above, so mod-side semantics are the engine's semantics, not a parallel model. Two consequences worth stating plainly, because they are what a mod author gets wrong:

- **Liveness ends at once; STORAGE reclamation is what waits.** `IsEntityAlive` reads `false` on the very next call after `DestroyEntity`, while the component row survives until someone flushes. A bulk read taken in between still sees the dead entity's row — gate on `IsEntityAlive` when that matters.
- **No flush member is promoted to the SDK.** `FlushDestroyedEntities` stays engine-side deliberately: flushing has whole-world ordering consequences and is rejected while any span or batch is active, so a mod able to force one could reclaim storage out from under a concurrently running system. A mod states the intent; the engine schedules the effect.

**Versioning law.** A given `(Index, Version)` pair is issued at most once per world lifetime. Stale references fail closed everywhere: reads return `false`, batch commands are dropped at flush ("entities still alive at flush time", `WriteBatch.cs:114-117`), event handlers holding a dead `EntityId` simply return. The law is only as strong as the versions callers present (§5).

## §7 SystemBase

`SystemBase` (`src/DualFrontier.Core/ECS/SystemBase.cs`) defines three lifecycle hooks plus the storage/bus accessors. The К8.3+К8.4 cutover statement is canonical in its class doc comment (`:12-17`): the managed-`World` surface (`GetComponent`/`SetComponent`/`Query`/`GetSystem`) is removed; systems use span/batch exclusively.

```csharp
// Abridged from src/DualFrontier.Core/ECS/SystemBase.cs (verified at HEAD).
protected virtual void OnInitialize() { }    // :35 — bus subscriptions, one-time setup
public abstract void Update(float delta);    // :41 — called per [TickRate]
protected virtual void OnDispose() { }       // :48 — unsubscribe, release
protected IGameServices Services { get; }    // :70 — domain-bus aggregator
protected NativeWorld NativeWorld { get; }   // :93 — sole production storage path
protected ManagedStore<T>? ManagedStore<T>() // :126 — Path β; null for Core origin,
    where T : class, IComponent;             //   missing resolver, or unregistered T
```

`Services` and `NativeWorld` route through the active `SystemExecutionContext` and throw `InvalidOperationException` outside a scheduler context (`:74-76`, `:97-99`) — e.g. from the renderer main thread, or after an illegal `async` resumption ([THREADING.md](./THREADING.md), async-ban section). An access declaration is mandatory: `[SystemAccess(reads: […], writes: […], bus: nameof(IGameServices.X))]`, read once at registration for graph building ([THREADING.md](./THREADING.md)).

### §7.1 The SDK system contract (W1)

W1 (VANILLA_SEPARATION_MIGRATION_PLAN BD-1) added `DualFrontier.Contracts.Sdk.ISimulationSystem` — the durable, Contracts-only system-authoring surface (`Initialize(ISystemContext)` / `Tick(ISystemContext)` / `OnDispose()`; no `float delta` — SimTick arrives via `ISystemContext.CurrentTick`). A mod (and, after the W5 slice move, vanilla) implements it instead of deriving `SystemBase`, which cannot relocate to Contracts (audit A4 `Contracts → Core.Interop → Contracts` cycle). The engine wraps an implementation onto the executor through the internal `SystemAdapter<T>` (`DualFrontier.Application`), reading the wrapped system's `[SystemAccess]`/`[TickRate]` via the `SystemBase.AccessDeclaration`/`TickRateDeclaration` hooks so the adapter is transparent to the executor's reflection. `SystemBase` and the adapter are a BRIDGE: both retire at W5 when the last `src/` harness system migrates (GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY §4 deletion trigger). The §8 "no cached world reference" anti-pattern binds `ISystemContext` identically — the context is per-tick, and neither it nor any value obtained from it may be held across ticks.

## §8 Anti-patterns

- **Caching a `NativeWorld` reference in system state** (e.g. a constructor parameter stored in a field). Systems receive the world only through the execution context; a cached reference survives graph rebuilds and mod hot-reloads that invalidate it.
- **Fabricating an `EntityId` from a span index.** `new EntityId(indices[i], 0)` was the §5 defect and is now a build error (**DFK022**, К-L22). Read the generation from the world instead: `new EntityId(indices[i], versions[indices[i]])`, or just iterate `lease.Pairs`. Note that `versions[i]` is the same mistake wearing a fix — `Versions` is entity-index-keyed, not dense-keyed.
- **NOT an anti-pattern: holding an `EntityId` across ticks.** The first bullet binds engine OBJECT references — a `NativeWorld`, an `ISystemContext`, anything reached through one. An `EntityId` is WORLD IDENTITY, a value the world resolves, and a mod may legitimately keep one (the W3 Weather mod holds its singleton across ticks). What a held id cannot promise is that the entity is still there, so probe it with `IsEntityAlive` before use. Conflating the two reads this section as forbidding stable identity, which would make persistent mod state impossible.
- **Calling another system directly.** `GetSystem<T>()` does not exist post-К8.3+К8.4. Cross-system communication routes through the domain buses — `Services.Combat.Publish(…)` etc. ([CONTRACTS.md](./CONTRACTS.md)).
- **Logic in a component.** Damage math lives in `DamageSystem`; the component stays data. Post-cutover motivation: Path α components must remain `unmanaged` structs to cross the native boundary.
- **Recording a write and reading it back in the same scope.** Batch commands are invisible until `Flush` (`WriteBatch.cs:196`) — `TryGetComponent` immediately after `batch.Add(id, …)` reads pre-flush state. Record in this pass, read next pass (or `Flush` first). The predecessor's version of this example called `batch.Set(…)`, which does not exist — the recording surface is `Update`/`Add`/`Remove` (§4).

## Cross-references

| Document | Relation | Note |
|---|---|---|
| [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) | defers-to | К-L3/К-L3.1/К-L8/К-L11 storage invariants; §1.7 span protocol (its version-0 example is amended by the same C10 fix) |
| [THREADING.md](./THREADING.md) | cites | Phases, dispatch, execution contexts, `[SystemAccess]` enforcement state, async ban |
| [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) | defers-to | Path β / `RegisterManagedComponent`, mod fault lifecycle, enforcement model |
| [FIELDS.md](./FIELDS.md) · [ARCHITECTURE.md](./ARCHITECTURE.md) · [CONTRACTS.md](./CONTRACTS.md) | cites | Orthogonal spatial storage (identity `(field_id, x, y)`); layer map; domain buses |
| [IDENTITY_AND_ABI_CONTRACT.md](./IDENTITY_AND_ABI_CONTRACT.md) | defers-to | Target identity law: §2 versions surface, `IsValid` alignment, analyzer rule (§5 here) |
| [PERSISTENCE_SNAPSHOT_CONTRACT.md](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (AUTHORED draft) | cites | Whether saves persist versions (`EntityEncoder` waiver) is decided there |

## Amendment protocol

Amendments surface to the owner (Crystalka) with rationale before landing — no default amendments to standing law. Semver: PATCH for correction, MINOR for additive sections, MAJOR for inverting described architecture; propagate to citing documents in the same change.

## Change history

| Version | Date | Change |
|---|---|---|
| **1.3.1** | 2026-08-22 | **PATCH — ID_B_ENTITY_VERSIONS, PR #51 review fixes.** §6 records the pending-destroy **tombstone** (negative version-table entry; `IsAlive` rejects the slot; the flush lifts it to the successor version before the index becomes recyclable) and that the create refusal is an exception, not a silent `EntityId.Invalid`. §5 gains the hazard the fix nearly created: reading TRUE versions is not automatically safe, because the component row outlives `DestroyEntity` and the naive reconstruction named the pair a recycle would mint — a fail-OPEN where fabrication had been fail-closed by accident. Recorded rather than quietly fixed, because the lesson generalises: making a value truthful widens what it reaches, and the new reach needs its own audit. |
| **1.3.0** | 2026-08-22 | **MINOR — ID_B_ENTITY_VERSIONS C7.** §5 flips from *Known defect: fabricated entity versions* to **RESOLVED**: `SpanLease<T>.Versions` exposes the world's per-slot version table (native `df_world_acquire_versions`), all three pair-iterators reconstruct `new EntityId(idx, versions[idx])`, ~18 production sites migrated, DFK022 Error-enforcing, К-L22 seated. The fabrication history is retained rather than deleted — the two-stage mistake (`Version = 1`, then `Version = 0`) is the instructive part — and one surviving silent trap is documented in its place: `Versions` is ENTITY-INDEX-keyed while `Span`/`Indices` are dense-keyed. §4's canonical example gains the versions-view idiom; §6 records the create-under-view refusal and the `IsValid => Index > 0` alignment; §8's anti-pattern now points at the analyzer instead of an inventory. Closes F-59. EVT-2026-08-22-ID_B_ENTITY_VERSIONS. |
| **1.2.0** | 2026-08-20 | **MINOR — W3_WEATHER_SLICE C8.** §5 records the W3 C5b correction: `SpanLease<T>.Pairs`, the `WriteBatch<T>` enumerator and `Sdk/SpanScope<T>.Pairs` fabricated `Version = 1` — a version no entity ever carries — so every batched write keyed on a span id was dropped at flush in silence; all three now fabricate 0 like every other site, narrowing but NOT resolving the C10/N-22 defect. §5 also records that `ISystemContext.DestroyEntity` (W3) puts destruction in MOD hands, so the "production is creation-only" latency argument no longer rests on engine call sites alone. §6 gains the SDK entity-lifecycle surface (liveness ends at once, storage reclamation waits; no flush member promoted, and why). §8 adds the id-is-not-an-engine-reference clarification — holding an `EntityId` across ticks is legitimate. EVT-2026-08-20-W3_WEATHER_SLICE. |
| 0.1.1 | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R1-15/16/17): §1 DFK011 pointer §4.1→§1.1; §5 fabrication census "10 systems"→"9" (InventorySystem constructs none); §4 rejection-site range :111-113→:112-115; §1 sketch parameter name `value`→`content` (signature-verified caption honored). |
| 0.1.0 (unreleased, AUTHORED) | 2026-07-15 | Successor of DOC-A-ECS v1.1.1: §4 canonical example rewritten without version fabrication and without the deleted-runtime-check comment (C10/N19); new §5 documents fabricated versions as a known defect with the N-22 collapse mechanism and the IDENTITY_AND_ABI_CONTRACT §2 fix fenced; §6 states wrong-version-of-live-entity fails-closed semantics and corrects the phase-boundary destruction claim; API sketches re-verified (constraints, `Span` vs `Components`, `batch.Set` removed). |