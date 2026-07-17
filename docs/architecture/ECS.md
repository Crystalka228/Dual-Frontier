---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ECS_V2
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0.0"
next_review_due: 2027-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ECS_V2
---
# Entity Component System

The entity/component storage model: `NativeWorld` as the single production backend, dense sparse-set storage, the span/batch access protocol, entity identity and lifecycle semantics, and `SystemBase`.

> **Document class: authored-rework (current-truth candidate).** Successor of `docs/architecture/historical/ECS.md` (DOC-A-ECS, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. Becomes the LOCKED authority upon Crystalka ratification per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7; until then the predecessor remains the last-ratified reference and prevails on conflict.
> **Ratification checklist:** [ ] content spot-audit at ratification HEAD · [ ] lifecycle AUTHORED → LOCKED, version → 1.0.0 · [ ] `next_review_due` set · [ ] predecessor register rationale updated.

## Status

| Field | Value |
|---|---|
| Role | normative-current-candidate |
| Successor of | `docs/architecture/historical/ECS.md` (DOC-A-ECS) |
| Scope | Entity/component storage as it exists in code: `NativeWorld` surface, dense-storage rationale, span/batch protocol, identity/lifecycle semantics (including the fabricated-version defect), `SystemBase`, anti-patterns |
| Non-goals | Field storage (FIELDS.md); scheduling ([THREADING.md](./THREADING.md)); Path β mod-API detail (MOD_OS_ARCHITECTURE.md); target identity/ABI law (IDENTITY_AND_ABI_CONTRACT.md, AUTHORED draft); persistence (PERSISTENCE_SNAPSHOT_CONTRACT.md, AUTHORED draft) |
| Authority domains | storage access-pattern teaching; entity lifecycle semantics (descriptive, code-anchored). Storage-path invariant text (К-L3/К-L3.1/К-L8/К-L11) stays [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md)'s |
| Defers to | [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) storage invariants · [THREADING.md](./THREADING.md) phase/dispatch law · [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) Path β / mod lifecycle · [IDENTITY_AND_ABI_CONTRACT.md](./IDENTITY_AND_ABI_CONTRACT.md) (AUTHORED draft) target identity law |

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

**EntityId** is `readonly record struct EntityId(int Index, int Version)` (`src/DualFrontier.Contracts/Core/EntityId.cs:21`), sentinel `Invalid = default` (0, 0) (`:28`). The version increments on destroy (§6), making cached references safely invalid: a stale id fails `TryGetComponent` and the system skips it — no crash. Verified drift: managed `IsValid => Index > 0 || Version > 0` (`EntityId.cs:38`) accepts ids like `(0, 5)` that native `is_alive` rejects unconditionally (`index <= 0` permanently dead, `world.cpp:75`); the alignment fix is [IDENTITY_AND_ABI_CONTRACT.md](./IDENTITY_AND_ABI_CONTRACT.md) (AUTHORED draft) §2's. `IsValid` is syntactic only — aliveness is answered exclusively by `NativeWorld.IsAlive`.

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
        // Bulk pass: dense span walk. No EntityId is needed — and none is
        // available: lease.Indices exposes entity INDICES only, without
        // versions. Do not manufacture an EntityId from an index (§5).
        using (SpanLease<HealthComponent> lease = NativeWorld.AcquireSpan<HealthComponent>())
        {
            ReadOnlySpan<HealthComponent> health = lease.Span;
            for (int i = 0; i < lease.Count; i++)
            { /* aggregate over health[i]; lease.Indices[i] names the slot, not an identity */ }
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

## §5 Known defect: fabricated entity versions (C10 / N-22)

The span ABI does not return versions, and the current codebase — including this document's predecessor and `KERNEL_ARCHITECTURE.md` §1.7, which taught the pattern — fabricates them: 13+ production sites across 9 systems plus `GameBootstrap.cs:241` construct `new EntityId(indices[i], 0)` (e.g. `NeedsSystem.cs:93`); `EntityEncoder.cs:85` decodes saved ranges to version 0; `SpanLease<T>.Pairs` and the `WriteBatch<T>` enumerator fabricate `Version = 1` with an in-code caveat (`SpanLease.cs:76-84,112`; `WriteBatch.cs:221`).

**What the kernel actually enforces (N-22).** The generation machinery is real and fails closed: `is_alive` demands exact version equality — `id.version == versions_[id.index]` (`world.cpp:74-78`) — `destroy_entity` bumps the version before the slot can recycle (`world.cpp:90`), and every accessor and mutation gates on it (e.g. `add_component`, `world.cpp:116`). But a fabricated version-0 id matches only a slot whose version is still 0 — never destroyed — so for these callers the ABA guarantee collapses to "this index was never recycled." The defect is latent only because production is creation-only today: no `src/` code calls `DestroyEntity` or `FlushDestroyedEntities` at HEAD (grep-verified; destruction is exercised by tests), so no recycled slot exists yet. The day destruction ships, every fabrication site becomes a stale-read hazard.

**Wrong version of a live entity: fails closed.** Because `is_alive` is exact equality, an id with the right index but wrong version — too low *or* too high — is indistinguishable from dead: `TryGetComponent` returns `false`, writes are silently dropped. No index-only or nearest-version fallback exists. (`Pairs`' fabricated version 1 therefore mismatches a fresh entity's true version 0 — the in-code caveat scopes it to snapshot-then-record flows where flush revalidates.)

This is a **known defect of the current ABI**, not a sanctioned pattern. Do not add fabrication sites; the fix is [IDENTITY_AND_ABI_CONTRACT.md](./IDENTITY_AND_ABI_CONTRACT.md) (AUTHORED draft) §2's.

> **FENCED (target / planned — not current truth):** per IDENTITY_AND_ABI_CONTRACT.md §2 — an additive `df_world_acquire_versions` entry point exposes a read-only view of the native `versions_` table under the same acquire/release discipline as component spans; `SpanLease<T>` acquires it alongside the span and loops reconstruct `new EntityId(idx, versions[idx])` with true generations; fabrication becomes analyzer-detectable; this document's §4 example gains the versions-view idiom when it ships.

## §6 Entity lifecycle

**Creation.** `NativeWorld.CreateEntity()` (`:120`), or the production bulk path — `RandomPawnFactory`/`ItemFactory` create entities and attach components via `AddComponents` in one P/Invoke (`ItemFactory.cs:144-150`). Natively the id comes from the free list (recycled index, current version) or `next_index_++` (`world.cpp:57-72`); index 0 is never live (`world.cpp:75`).

**Destruction.** `DestroyEntity(id)` marks: the version increments immediately (`world.cpp:90`) — `IsAlive`/`TryGetComponent` fail from that moment — and the id joins a pending queue. Component removal and index recycling are deferred to `flush_destroyed` (`world.cpp:95-108`) via `FlushDestroyedEntities` (`NativeWorld.cs:148`); both are rejected while any span or batch is active (§4). The predecessor tied removal to "the next scheduler phase boundary" — no such scheduler hook exists; flush runs when a caller invokes it, and today no production code does (§5).

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

## §8 Anti-patterns

- **Caching a `NativeWorld` reference in system state** (e.g. a constructor parameter stored in a field). Systems receive the world only through the execution context; a cached reference survives graph rebuilds and mod hot-reloads that invalidate it.
- **Fabricating an `EntityId` from a span index.** `new EntityId(indices[i], 0)` is the §5 defect. Existing sites are inventoried for the IDENTITY_AND_ABI_CONTRACT.md (AUTHORED draft) §2 fix; new code must not add more.
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
| [IDENTITY_AND_ABI_CONTRACT.md](./IDENTITY_AND_ABI_CONTRACT.md) (AUTHORED draft) | defers-to | Target identity law: §2 versions surface, `IsValid` alignment, analyzer rule (§5 here) |
| [PERSISTENCE_SNAPSHOT_CONTRACT.md](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (AUTHORED draft) | cites | Whether saves persist versions (`EntityEncoder` waiver) is decided there |

## Amendment protocol

Amendments surface to the owner (Crystalka) with rationale before landing — no default amendments to standing law. Semver: PATCH for correction, MINOR for additive sections, MAJOR for inverting described architecture; propagate to citing documents in the same change.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R1-15/16/17): §1 DFK011 pointer §4.1→§1.1; §5 fabrication census "10 systems"→"9" (InventorySystem constructs none); §4 rejection-site range :111-113→:112-115; §1 sketch parameter name `value`→`content` (signature-verified caption honored). |
| 0.1.0 (unreleased, AUTHORED) | 2026-07-15 | Successor of DOC-A-ECS v1.1.1: §4 canonical example rewritten without version fabrication and without the deleted-runtime-check comment (C10/N19); new §5 documents fabricated versions as a known defect with the N-22 collapse mechanism and the IDENTITY_AND_ABI_CONTRACT §2 fix fenced; §6 states wrong-version-of-live-entity fails-closed semantics and corrects the phase-boundary destruction claim; API sketches re-verified (constraints, `Span` vs `Components`, `batch.Set` removed). |
