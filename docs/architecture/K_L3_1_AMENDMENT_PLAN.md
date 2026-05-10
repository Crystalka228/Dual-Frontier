# K-L3.1 Amendment Plan — old/new text pairs

**Status**: AUTHORED 2026-05-10 (Phase 4 deliverable of K-L3.1 session)
**Authoring**: Crystalka + Opus chat session, single deliberation 2026-05-10
**Authority**: K-L3.1 session locks Q1–Q6 + synthesis form §4.A per `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` + `K_L3_1_BRIEF_ADDENDUM_1.md`
**Execution target**: separate amendment brief, executed by Cloud Code post-K-L3.1 closure
**Estimated execution**: 30–60 min auto-mode (docs-only; per-document atomic commit)
**Test count delta**: zero (no source changes)

---

## §0 Locks summary (recap from session)

| # | Lock | Mechanism |
|---|---|---|
| Q1 | (a) Attribute on type | `[ManagedStorage]` annotation on `class : IComponent` types; absence implies Path α (`unmanaged struct`); analyzer enforcement of shape↔attribute consistency deferred to post-migration analyzer milestone (Q5.b/M3.4-extension) |
| Q2 | (β-i) Mod-side + IModApi extension | Storage owned by mod assembly; `IModApi.RegisterManagedComponent<T> where T : class, IComponent` added to v3 surface; per-mod ManagedStore<T> instance lives in RestrictedModApi, reclaimed on `AssemblyLoadContext.Unload` |
| Q3 | (i) Explicit dual API | `SystemBase.NativeWorld.AcquireSpan<T>()` for Path α (existing K8.2 v2 plumbing); `SystemBase.ManagedStore<T>()` for Path β (new K8.4 plumbing); cross-mod managed-path direct access structurally impossible by ALC isolation, cross-mod data flow via events per `MOD_OS_ARCHITECTURE.md` §6 three-level contracts |
| Q4 | (b) Runtime-only managed-path | Save system out of scope per migration plan §0.4 + §8.1; managed-path components not persisted; codec layer untouched; persistence opt-in revisitable as future amendment milestone if/when game-mechanic demands |
| Q5 | (a) Passive metrics | `KernelCapabilityRegistry` tracks per-component path; performance reporting splits native/managed; active Roslyn analyzer enforcement (Q5.b) deferred to post-migration analyzer milestone per Crystalka 2026-05-10 «после миграции нужен будет анализатор... но это потом» |
| Q6 | (a) Path-blind capability | `[ModAccessible]` already targets `Class \| Struct` (K4 prerequisite); capability strings (`kernel.read:`, `mod.<id>.read:`) carry verb + FQN, path-orthogonal; resolver dispatches internally per-T |
| Synthesis | §4.A | Amend K-L3 wording (single principle, peer paths); KERNEL `v1.3 → v1.5`; MOD_OS `v1.6 → v1.7`; MIGRATION_PLAN `v1.0 → v1.1`; MIGRATION_PROGRESS content sync (no version) |

---

## §1 KERNEL_ARCHITECTURE.md amendments

**Target version**: `v1.3 → v1.5` (header drift correction: K8.2 v2 closure intended v1.4 per commit `7527d00` message but header was not bumped; K-L3.1 amendment lands header at v1.5 capturing both v1.4 implicit + v1.5 explicit substantive changes).

### §1.1 Header version line

**Old text** (line 3):
```
**Version**: 1.3
```

**New text**:
```
**Version**: 1.5
```

### §1.2 Status line

**Old text** (line 5):
```
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3)
```

**New text**:
```
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3); K8.2 v2 closure of K-L3 selective per-component application via K8.1 primitives (v1.4, header bump deferred to v1.5); K-L3.1 bridge formalization — Path β (managed-class, mod-side storage) as first-class peer to Path α (`unmanaged struct`, kernel-side NativeWorld) per session 2026-05-10 (v1.5)
```

### §1.3 Part 0 K-L3 row (decisions table)

**Old text** (line 40):
```
| K-L3 | Component constraint | Unmanaged structs only (Path α) | Storage requires blittable layout; class-based prohibited |
```

**New text**:
```
| K-L3 | Component storage paths | Path α (`unmanaged struct`, NativeWorld) default; Path β (managed `class` via `[ManagedStorage]`, mod-side store) per opt-in | Two first-class peer paths; per-component author choice based on architectural fit; native-path retains blittable-layout invariant, managed-path is mod-private and runtime-only |
```

### §1.4 K-L3 implication paragraph

This is the largest single edit. The current paragraph carries the K8.2 v2 closure framing «K-L3 «без exception» state achieved» which K-L3.1 supersedes.

**Old text** (line 50, single paragraph):
```
**Implication of K-L3**: All managed components must be unmanaged structs. **K8.2 v2 (`MIGRATION_PROGRESS.md` K8.2 closure entry, 2026-05-09) achieved K-L3 «без exception» state** for `src/DualFrontier.Components/`. Three deliverables in one milestone: (1) K8.1 wrapper value-type refactor — `NativeMap<K,V>`, `NativeSet<T>`, `NativeComposite<T>` from `sealed unsafe class` to `readonly unsafe struct` so component structs can carry these as fields without violating the `unmanaged` constraint; `InternedString` gained `IComparable<InternedString>`; `NativeWorld` gained `Allocate*Id` counters and `Create*` factory methods. (2) 6 class→struct conversions using K8.1 primitives — Identity/Workbench/Faction (InternedString), Skills (NativeMap×2), Storage (NativeMap+NativeSet), Movement (NativeComposite + bool HasTarget + PathStepIndex). (3) 6 empty TODO stub deletions per METHODOLOGY §7.1 «data exists or it doesn't» — Combat (Ammo/Shield/Weapon), Magic (School), Pawn (Social), World (Biome). Real game-mechanics components for these slices are authored fresh as `unmanaged struct` in the M-series vanilla mod content milestones (M9 Combat, M10.B Magic, M-series Pawn social, M-series World biome). Mod components subject to same constraint. **K4's "Hybrid Path" softening retired** — after K8.2 v2 closure, K-L3 holds without exception across vanilla and mod components alike.
```

**New text**:
```
**Implication of K-L3 (post-K-L3.1, 2026-05-10)**: Components are first-class via either Path α (`unmanaged struct`, kernel-side `NativeWorld` storage) or Path β (managed `class`, mod-side per-mod ManagedStore). Path α is the default — author silence + struct shape implies native registration via existing `IModApi.RegisterComponent<T> where T : IComponent`. Path β is per-component opt-in via `[ManagedStorage]` attribute on a `class : IComponent` type, registered through `IModApi.RegisterManagedComponent<T> where T : class, IComponent` (Mod API v3 surface, ships at K8.4 closure). Decision criterion is per-component architectural fit: Path α applies when conversion to `unmanaged struct` is justified (performance, locality, blittable layout, K8.1 primitive coverage); Path β applies when conversion forces structural compromise (managed-only references not expressible as K8.1 primitives, lazy state graphs, runtime-only computed handles, complex object graphs not blittable).

Path β components are runtime-only (Q4.b lock) — not persisted by save system; managed-storage lives per-mod (mod assembly's `RestrictedModApi` instance), reclaimed deterministically on `AssemblyLoadContext.Unload` per `MOD_OS_ARCHITECTURE.md` §9.5 unload chain. Cross-mod managed-path direct access is structurally impossible by ALC isolation; cross-mod data flow uses event/intent contracts per `MOD_OS_ARCHITECTURE.md` §6 three-level contracts. Within-mod cross-path access (one system reads native + managed components on same entity) is supported via dual `SystemBase` API (Q3.i lock): `SystemBase.NativeWorld.AcquireSpan<T>()` for Path α, `SystemBase.ManagedStore<T>()` for Path β; performance characteristics are visible per-call (no opaque dispatch).

**K8.2 v2 closure (`MIGRATION_PROGRESS.md` K8.2 v2 entry, 2026-05-09)** delivered the Path α kernel-side completion: K8.1 wrapper value-type refactor (`NativeMap<K,V>`, `NativeSet<T>`, `NativeComposite<T>` from `sealed unsafe class` to `readonly unsafe struct`; `InternedString.IComparable`; `NativeWorld.Allocate*Id` counters + `Create*` factory methods); 6 class→struct conversions using K8.1 primitives (Identity/Workbench/Faction via InternedString, Skills via NativeMap×2, Storage via NativeMap+NativeSet, Movement via NativeComposite+`HasTarget`+`PathStepIndex`); 6 empty TODO stub deletions per METHODOLOGY §7.1 «data exists or it doesn't» (Combat: Ammo/Shield/Weapon; Magic: School; Pawn: Social; World: Biome). The 6 deletions reflect §7.1 application — empty placeholder components removed because their data did not exist; this is selective per-component judgment (delete per §7.1, convert per K8.1 primitives, leave verify-only struct annotations on already-struct), not universal Path α mandate.

**K4's "Hybrid Path" framing superseded by K-L3.1**: post-amendment, both paths are first-class peers, not «α default plus β tolerated as exception». Author choice is recorded explicitly via `[ManagedStorage]` opt-in; absence implies Path α. The K8.2 v2 closure framing «K-L3 «без exception» state achieved» is corrected — closure delivered selective per-component application of K-L3 to `src/DualFrontier.Components/`, not universal mandate. Capability model is path-orthogonal (Q6.a lock) — `[ModAccessible]` attribute and capability strings (`kernel.read:` / `mod.<id>.read:`) function uniformly across paths; the resolver dispatches internally to NativeWorld span access or ManagedStore lookup per-T.

**Performance contract (Q5.a lock)**: native-path publishes specific guarantees (zero-allocation reads via `SpanLease<T>`, structure-of-arrays layout, batched writes via `WriteBatch<T>`). Managed-path provides Dictionary-shaped lookup with no zero-allocation guarantee. The contract difference is visible per-call via dual `SystemBase` API (Q3.i). Performance metrics are tagged per-path in `KernelCapabilityRegistry` (Q5.a passive); active analyzer enforcement (Q5.b) deferred to post-migration analyzer milestone per Crystalka 2026-05-10 «после миграции нужен будет анализатор... но это потом».
```

### §1.5 Part 4 Decisions log — Path α vs Path β entry

**Old text** (lines 805–809):
```
**Path α vs Path β для components**:
- Path α: convert components к structs (this document, K-L3)
- Path β: GCHandle marshalling for class components (rejected — defeats GC pressure reduction goal)
- **Path α chosen** — aligns с modern ECS conventions, eliminates GC pressure structurally
```

**New text**:
```
**Path α vs Path β для components (resolved 2026-05-10 per K-L3.1)**:
- Path α: `unmanaged struct` components in kernel-side `NativeWorld` (existing K-L3 default; K8.2 v2 closure delivered for `src/DualFrontier.Components/`)
- Path β (original rejection): GCHandle marshalling on kernel-side managed component store — rejected, defeats GC pressure reduction goal
- Path β (K-L3.1 reformulation): managed `class` components annotated with `[ManagedStorage]`, stored mod-side in per-mod `RestrictedModApi.ManagedStore<T>` instance (Q2.β-i lock); kernel-side has no managed component store; ALC isolation provides ownership boundary; reclaim is deterministic on `AssemblyLoadContext.Unload`
- **Both paths chosen as first-class peers per K-L3.1 (2026-05-10)**: kernel-side native storage (Path α) preserves K-L11 «NativeWorld single source of truth» for native data; managed-storage decentralization-by-mod is consistent with K-L9 «vanilla = mods» + ALC isolation. Performance characteristics visible per-call via dual `SystemBase` API (Q3.i). Capability model path-orthogonal (Q6.a). Save system out of scope (Q4.b runtime-only managed-path). Amendment authority: K-L3.1 amendment plan at `docs/K_L3_1_AMENDMENT_PLAN.md` + bridge formalization brief at `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`.
```

### §1.6 Part 2 K8.2 row (master plan table)

The K8.2 row currently uses «без exception» framing matching the K8.2 v2 closure intent. K-L3.1 amendment corrects to «selective per-component closure».

**Old text** (line 582):
```
| K8.2 | K-L3 «без exception» closure: K8.1 wrapper value-type refactor (NativeMap/NativeSet/NativeComposite to readonly struct + IComparable<InternedString> + per-instance id allocation) + 6 class→struct conversions (Identity/Workbench/Faction/Skills/Storage/Movement) + 6 empty TODO stub deletions (Ammo/Shield/Weapon/School/Social/Biome — content deferred to M-series) + 12 ModAccessible annotation completeness pass | 6-12 hours auto-mode (3-5 days hobby) | +800/-1500 |
```

**New text**:
```
| K8.2 | K-L3 selective per-component closure (post-K-L3.1 reframing): K8.1 wrapper value-type refactor (NativeMap/NativeSet/NativeComposite to readonly struct + IComparable<InternedString> + per-instance id allocation) + 6 class→struct conversions on Path α via K8.1 primitives (Identity/Workbench/Faction/Skills/Storage/Movement) + 6 empty TODO stub deletions per METHODOLOGY §7.1 (Ammo/Shield/Weapon/School/Social/Biome — content deferred to M-series, authored on appropriate path per K-L3.1) + 12 ModAccessible annotation completeness pass on already-struct components | 6-12 hours auto-mode (3-5 days hobby) | +800/-1500 |
```

### §1.7 Closing line (document v1.0 sediment)

The closing notes line «This document is **v1.0**, authoritative until amended» is sediment from initial authoring.

**Old text** (line 1085):
```
This document is **v1.0**, authoritative until amended via explicit decision. Amendments require commit с rationale (similar к how MOD_OS_ARCHITECTURE.md evolved).
```

**New text**:
```
This document is **v1.5** (current), authoritative until amended via explicit decision. Amendments require commit с rationale (similar к how MOD_OS_ARCHITECTURE.md evolved). Version history: v1.0 initial; v1.1 K6 reconciliation; v1.2 K-L11 + Solution A; v1.3 Interop error semantics convention; v1.4 K8.2 v2 closure (header bump deferred); v1.5 K-L3.1 bridge formalization.
```

---

## §2 MOD_OS_ARCHITECTURE.md amendments

**Target version**: `v1.6 → v1.7` (non-semantic correction; K-L3.1 amendment doesn't change locked decisions D-1 through D-7).

### §2.1 Status line

**Old text** (line 8):
```
**Status:** LOCKED v1.6 — Phase 0 closed; non-semantic corrections from M1–M3.1 audit (v1.1), M3 closure review (v1.2), M4.3 implementation review (v1.3), M7 pre-flight readiness review (v1.4), Audit Campaign Pass 2 (v1.5), and GPU compute integration ratification (v1.6) applied. Every architectural decision in this document is final input to all subsequent migration phases (M1–M10, K9, G0–G9, see §11). Items marked **✓ LOCKED** reflect decisions taken during Phase 0 deliberation; deviation in implementation requires reopening this document, not improvisation in code.
```

**New text**:
```
**Status:** LOCKED v1.7 — Phase 0 closed; non-semantic corrections from M1–M3.1 audit (v1.1), M3 closure review (v1.2), M4.3 implementation review (v1.3), M7 pre-flight readiness review (v1.4), Audit Campaign Pass 2 (v1.5), GPU compute integration ratification (v1.6), and K-L3.1 bridge formalization (v1.7) applied. Every architectural decision in this document is final input to all subsequent migration phases (M1–M10, K9, G0–G9, see §11). Items marked **✓ LOCKED** reflect decisions taken during Phase 0 deliberation; deviation in implementation requires reopening this document, not improvisation in code.
```

### §2.2 Add v1.7 entry to version history

Insert before `---` separator following v1.6 entry:

**New text** (insertion at end of version-history list, before the «---» on line 41):
```
- v1.7 — K-L3.1 bridge formalization applied (session 2026-05-10):
  - §«Modding с native ECS kernel» (lines 1149–1150 v1.6): rewritten to reflect K-L3.1 — Path α (`unmanaged struct` via `RegisterComponent<T>`) and Path β (managed `class` via `[ManagedStorage]` + `RegisterManagedComponent<T>`) as first-class peers; cross-mod direct managed-path access structurally impossible by ALC isolation; within-mod cross-path access via dual `SystemBase` API.
  - §3.5 D-1 LOCKED note: path orthogonality clarified — `[ModAccessible]` (already `Class | Struct` per K4 prerequisite) and capability strings function uniformly across paths.
  - §4.6 IModApi v3 surface: `RegisterManagedComponent<T> where T : class, IComponent` added alongside existing `RegisterComponent<T>` (Path α) and `Fields`/`ComputePipelines` v1.6 additions.
  - §11.1: M3.5 deferred milestone description extended (analyzer covers Path α/β consistency in addition to `[FieldAccessible]` scan extension); analyzer ships post-migration per Q5.b deferral.
  - No semantic changes to v1.6 decisions. No locked decision (D-1 through D-7) is altered. Authority: K-L3.1 amendment plan at `docs/K_L3_1_AMENDMENT_PLAN.md`.
```

### §2.3 Lines 1149–1150 (the precise stale wording)

This is the critical edit identified by brief §1.2.2.

**Old text** (lines 1148–1153):
```
When kernel migration к native completes (K-series, see `KERNEL_ARCHITECTURE.md`):
- Mod component types must be `unmanaged` structs (Path α)
- Class-based component storage prohibited (через ECS — mod state classes acceptable outside ECS)
- Vanilla mods register components и systems through same IModApi as third-party (vanilla = mods principle preserved)
- Mod replacement triggers second-graph rebuild (managed) — native side untouched
- Mod fields (`RawTileField<T>`) и compute pipelines registered through same `IModApi` extension (`api.Fields` / `api.ComputePipelines`); see [GPU_COMPUTE](./GPU_COMPUTE.md) "Architectural integration → Mod-driven shader registration"
```

**New text**:
```
When kernel migration к native completes (K-series, see `KERNEL_ARCHITECTURE.md`):
- Mod component types are either Path α (`unmanaged struct`, kernel-side `NativeWorld` storage) or Path β (managed `class` annotated with `[ManagedStorage]`, mod-side `ManagedStore<T>` storage). Path α is default; Path β is per-component opt-in for shapes where unmanaged conversion forces structural compromise (managed-only references, lazy state graphs, complex object graphs not blittable). See `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication post-K-L3.1 for the decision criterion.
- Path α registers via existing `IModApi.RegisterComponent<T> where T : IComponent`. Path β registers via `IModApi.RegisterManagedComponent<T> where T : class, IComponent` (Mod API v3 surface, ships at K8.4 closure). Both registration entries are uniform across vanilla and third-party mods (K-L9 «vanilla = mods» preserved).
- Cross-mod managed-path direct access is structurally impossible by `AssemblyLoadContext` isolation: a regular mod's ALC cannot resolve types from another regular mod's ALC, so `Vanilla.Combat` cannot reference `Vanilla.Pawn.JobQueueComponent` at compile time. Cross-mod data flow uses event/intent contracts per §6 three-level contracts (publish/subscribe via shared protocol mods).
- Within-mod cross-path access (one system reads native + managed components on same entity) is supported via dual `SystemBase` API: `SystemBase.NativeWorld.AcquireSpan<T>()` for Path α; `SystemBase.ManagedStore<T>()` for Path β. The accessor resolves to the system's owning mod's per-mod store via `SystemMetadata.ModId` (K6.1 plumbing). Performance characteristics visible per-call: native-path is zero-allocation span iteration; managed-path is Dictionary-shaped lookup.
- Mod replacement triggers second-graph rebuild (managed) — native side untouched. Per-mod `ManagedStore<T>` instances reclaim deterministically with the mod's `AssemblyLoadContext.Unload` per §9.5 unload chain.
- Vanilla mods register components, systems, fields, and compute pipelines through the same IModApi as third-party (vanilla = mods principle preserved per K-L9).
- Mod fields (`RawTileField<T>`) и compute pipelines registered through `IModApi` extension (`api.Fields` / `api.ComputePipelines`); see [GPU_COMPUTE](./GPU_COMPUTE.md) "Architectural integration → Mod-driven shader registration".
- `[ModAccessible]` annotation and capability strings (`kernel.read:`, `mod.<id>.read:`) function uniformly across Path α and Path β (Q6.a path-blind capability lock per K-L3.1). The capability resolver dispatches internally to `NativeWorld` span access or `ManagedStore` lookup per-T.
```

### §2.4 §3.5 D-1 LOCKED — path-orthogonality clarification

**Old text** (block following «✓ LOCKED (D-1)» around line 308):
```
> **✓ LOCKED (D-1).** `read` and `write` capabilities apply only to a **curated, opt-in subset** of public components. A component is reachable from a mod only when annotated with `[ModAccessible(Read = true, Write = false)]`. The component author actively decides what mods can touch; everything else is invisible to the capability resolver and produces a `MissingCapability` error if requested. Aligns with the project's structural-isolation philosophy: tighter blast radius, falsifiable surface.
```

**New text**:
```
> **✓ LOCKED (D-1).** `read` and `write` capabilities apply only to a **curated, opt-in subset** of public components. A component is reachable from a mod only when annotated with `[ModAccessible(Read = true, Write = false)]`. The component author actively decides what mods can touch; everything else is invisible to the capability resolver and produces a `MissingCapability` error if requested. Aligns with the project's structural-isolation philosophy: tighter blast radius, falsifiable surface.
>
> **Path orthogonality (K-L3.1, 2026-05-10).** `[ModAccessible]` annotation applies uniformly across Path α (`unmanaged struct`) and Path β (managed `class` via `[ManagedStorage]`). The attribute's `AttributeUsage` already targets `Class | Struct` (widened in K4 prerequisite commit). Capability strings carry verb + FQN (e.g. `kernel.read:Vanilla.Pawn.JobQueueComponent` or `mod.dualfrontier.vanilla.pawn.read:JobQueueComponent`) and are path-orthogonal — provider namespace prefix differs by ownership (kernel vs mod), not by storage path. The capability resolver dispatches internally to `NativeWorld` span access (Path α) or per-mod `ManagedStore<T>` lookup (Path β) per-T. See `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication post-K-L3.1 for the storage-path decision criterion.
```

### §2.5 §4.6 IModApi v3 surface — add `RegisterManagedComponent<T>`

**Insertion target**: §4.6.1 surface code block (around line 430). The current v3 surface already documents `Fields` and `ComputePipelines` additions. K-L3.1 adds `RegisterManagedComponent<T>` as another v3 addition.

**Old text** (the surface code block in §4.6.1):
```csharp
public interface IModApi // v3 — extends v2
{
    // ── v1 + v2 surface (preserved verbatim) ──────────────────────────────
    // RegisterComponent, RegisterSystem, Publish, Subscribe,
    // PublishContract, TryGetContract, GetKernelCapabilities,
    // GetOwnManifest, Log

    // ── New in v3 ─────────────────────────────────────────────────────────

    /// <summary>
    /// Field-storage sub-API. Null when the kernel does not support fields
    /// (e.g. K9 not yet landed, or the build runs in CPU-only fallback mode).
    /// Mods checking this for null degrade gracefully.
    /// </summary>
    IModFieldApi? Fields { get; }

    /// <summary>
    /// Compute-pipeline sub-API. Null when the kernel does not support GPU
    /// compute (e.g. G0 not yet landed, or Vulkan 1.3 compute unavailable on
    /// the host machine). Mods checking this for null fall back to CPU
    /// reference implementations.
    /// </summary>
    IModComputePipelineApi? ComputePipelines { get; }
}
```

**New text**:
```csharp
public interface IModApi // v3 — extends v2
{
    // ── v1 + v2 surface (preserved verbatim) ──────────────────────────────
    // RegisterComponent<T> where T : IComponent (Path α; existing v2)
    // RegisterSystem, Publish, Subscribe,
    // PublishContract, TryGetContract, GetKernelCapabilities,
    // GetOwnManifest, Log

    // ── New in v3 ─────────────────────────────────────────────────────────

    /// <summary>
    /// Register a Path β managed-class component (K-L3.1 bridge per
    /// `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication post-K-L3.1).
    /// Type T must be a class implementing IComponent and annotated with
    /// [ManagedStorage]; absence of attribute on a class component is a
    /// load-time `ValidationErrorKind.MissingManagedStorageAttribute`
    /// error (added in v1.7 §11.2). Storage lives in the per-mod
    /// RestrictedModApi instance (mod-side ownership per Q2.β-i);
    /// reclaimed deterministically on AssemblyLoadContext.Unload.
    /// </summary>
    void RegisterManagedComponent<T>() where T : class, IComponent;

    /// <summary>
    /// Field-storage sub-API. Null when the kernel does not support fields
    /// (e.g. K9 not yet landed, or the build runs in CPU-only fallback mode).
    /// Mods checking this for null degrade gracefully.
    /// </summary>
    IModFieldApi? Fields { get; }

    /// <summary>
    /// Compute-pipeline sub-API. Null when the kernel does not support GPU
    /// compute (e.g. G0 not yet landed, or Vulkan 1.3 compute unavailable on
    /// the host machine). Mods checking this for null fall back to CPU
    /// reference implementations.
    /// </summary>
    IModComputePipelineApi? ComputePipelines { get; }
}
```

### §2.6 §11.1 M3.5 deferred milestone — extend description

The current §11.1 M3.5 row covers field-type capability scan extension. K-L3.1 extends scope to Path α/β consistency analyzer (Q5.b deferred enforcement).

**Old text** (line 999, M3.5 row):
```
| **M3.5** *(deferred)* | Capability registry refresh for field types via `[FieldAccessible]` scan extension to `KernelCapabilityRegistry` (per v1.6 §3.5) | Extended `KernelCapabilityRegistry` recognising `[FieldAccessible]` annotation | `FieldCapabilityRegistryTests`; unblocked at K9 in-progress |
```

**New text**:
```
| **M3.5** *(deferred)* | Capability registry refresh for field types via `[FieldAccessible]` scan extension (per v1.6 §3.5) + Path α/β consistency analyzer covering `[ManagedStorage]` shape↔attribute consistency, `RegisterComponent`/`RegisterManagedComponent` constraint match, and dual-API access pattern enforcement (per K-L3.1 Q5.b deferred enforcement) | Extended `KernelCapabilityRegistry` recognising `[FieldAccessible]` annotation; Roslyn analyzer covering Path α/β consistency rules | `FieldCapabilityRegistryTests` + `PathConsistencyAnalyzerTests`; unblocked at K9 in-progress (field side) and post-migration (path-consistency side per K-L3.1 Q5.b) |
```

### §2.7 §11.2 ValidationErrorKind enumeration — add new entry

**Insertion target**: §11.2 list of v1.6 added kinds. Add new K-L3.1-derived entry.

**Insertion location**: in the bullet list at §11.2, after `ComputeUnsupportedWarning (G0)`.

**New text** (new bullet, append to §11.2 list):
```
- `MissingManagedStorageAttribute` (K-L3.1) — mod calls `RegisterManagedComponent<T>` where T is a class but not annotated with `[ManagedStorage]`, OR mod calls `RegisterComponent<T>` where T is a class (Path α expects `unmanaged struct` shape; class type without `[ManagedStorage]` is shape-attribute mismatch). Active enforcement is load-time runtime check until M3.5 analyzer ships (per K-L3.1 Q5.b deferral); analyzer adds compile-time enforcement.
```

---

## §3 MIGRATION_PLAN_KERNEL_TO_VANILLA.md amendments

**Target version**: `v1.0 → v1.1` (non-semantic correction; K-L3.1 doesn't reverse Option (II) or any Decision #1–8).

### §3.1 Header version + lock-date line

**Old text** (lines 3–4):
```
**Version**: 1.0 LOCKED
**Authored**: 2026-05-09 (post-K-Lessons closure `9df2709..071ae11`)
```

**New text**:
```
**Version**: 1.1 LOCKED (v1.1 = non-semantic correction post-K-L3.1 amendment 2026-05-10)
**Authored**: 2026-05-09 (post-K-Lessons closure `9df2709..071ae11`); amended 2026-05-10 (K-L3.1 bridge formalization, Decision #9 added, line 62 reframed, K8.3-K8.5 sub-section extensions)
```

### §3.2 §0.3 — add Decision #9

**Insertion target**: end of §0.3 LOCKED architectural decisions list (after Decision #8).

**New text** (new bullet, append to §0.3 numbered list):
```
9. **Bridge formalization LOCKED (K-L3.1, 2026-05-10)**: Path α (`unmanaged struct`, kernel-side `NativeWorld`) and Path β (managed `class` via `[ManagedStorage]`, mod-side per-mod `ManagedStore<T>`) are first-class peers per K-L3.1 amendment to `KERNEL_ARCHITECTURE.md` Part 0 K-L3. Decision criterion: per-component architectural fit; default Path α; Path β opt-in via attribute. K8.2 v2 closure framing «K-L3 «без exception»» reframed as «K-L3 selective per-component application» (closure delivered selective judgment, not universal mandate). M-series vanilla mod content milestones author per-component on appropriate path. K8.3 system migration extends to dual-path access (`SystemBase.NativeWorld` + `SystemBase.ManagedStore<T>`); per-system access pattern decision in K8.3 brief. K8.4 ships `IModApi.RegisterManagedComponent<T>` as part of v3 surface alongside `Fields` and `ComputePipelines`. K8.5 mod authoring guide documents per-component path choice criterion. Save system out of scope (Q4.b runtime-only managed-path). Authority: K-L3.1 amendment plan at `docs/K_L3_1_AMENDMENT_PLAN.md`; bridge brief at `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`.
```

### §3.3 Line 62 — Option (I) rejection paragraph reframing

**Old text** (line 62):
```
Option (I) was rejected because it would put the mass migration work in front of the kernel work — vanilla mods would temporarily contain class components (K-L3 violation period of weeks-months), then K8.2 would convert them later. That's a second pass over the same code; under "без костылей" (Crystalka, repeated 2026-05-09), the second pass is structural cost without architectural benefit.
```

**New text**:
```
Option (I) was rejected because it would put the mass migration work in front of the kernel work — vanilla mods would temporarily contain class components without `[ManagedStorage]` opt-in (K-L3 path-declaration ambiguity period of weeks-months), then K8.2 would convert/annotate them later. That's a second pass over the same code; under "без костылей" (Crystalka, repeated 2026-05-09), the second pass is structural cost without architectural benefit. Note (post-K-L3.1, 2026-05-10): post-amendment, class components carrying `[ManagedStorage]` are first-class Path β peers — the original «K-L3 violation» framing applied to undeclared class components only, not to managed-path components per se.
```

### §3.4 §1.2 K8.3 scope — extend with dual access pattern

**Insertion target**: §1.2 «Architectural design constraints that K8.3 brief must inherit from this plan» list.

**New text** (new bullet, append to §1.2 constraints list):
```
- **Dual access pattern (post-K-L3.1)**: each system accesses Path α components via existing `SystemBase.NativeWorld.AcquireSpan<T>()` + `WriteBatch<T>` (K8.2 v2 plumbing); Path β components (when present in same mod) via new `SystemBase.ManagedStore<T>()` (K8.4 plumbing). Per-system access-pattern audit (which components each system reads/writes and on which path) is K8.3 brief authoring concern; this plan locks only the dual-API mechanism per K-L3.1 Q3.i. Cross-mod managed-path direct access is structurally impossible (ALC isolation per K-L9); cross-mod data flow uses events/intents per `MOD_OS_ARCHITECTURE.md` §6.
```

### §3.5 §1.3 K8.4 scope — add `RegisterManagedComponent` to v3 deliverables

**Insertion target**: §1.3 K8.4 deliverables paragraphs (currently three bullet groups around lines 184–192).

**New text** (insert as new bullet group, after the «Mod API v3 ship» bullet):
```
- **Mod API v3 `RegisterManagedComponent<T>` ships** per K-L3.1 Q2.β-i lock. Surface added to `IModApi` v3 alongside existing `RegisterComponent<T>` (Path α), `Fields`, and `ComputePipelines` (v1.6 additions). `RestrictedModApi.RegisterManagedComponent<T>` creates per-mod `ManagedStore<T>` instance held in the RestrictedModApi instance; storage decentralized per-mod, reclaimed on `AssemblyLoadContext.Unload` per `MOD_OS_ARCHITECTURE.md` §9.5. `SystemBase.ManagedStore<T>()` accessor resolves via `SystemExecutionContext.Current.ModId` to owning mod's store. Loader registers Path α via existing `RegisterComponent<T>` (NativeWorld), Path β via new `RegisterManagedComponent<T>` (per-mod ManagedStore). Path β is runtime-only (Q4.b lock); save system reconstructs on load post-G-series.
```

### §3.6 §1.5 Phase A closure gate — extend bullet list

**Insertion target**: §1.5 closure gate bullet list.

**New text** (insert new bullet near the existing K8.4-related items):
```
- Both Path α (`unmanaged struct` via `RegisterComponent<T>` → NativeWorld) and Path β (managed `class` via `[ManagedStorage]` + `RegisterManagedComponent<T>` → per-mod ManagedStore) registration paths active in `IModApi` v3 (K-L3.1 Q2.β-i)
- `SystemBase.ManagedStore<T>()` accessor available alongside existing `SystemBase.NativeWorld` (K-L3.1 Q3.i dual API)
```

### §3.7 §6 — add §6.6 K-L3.1 amendment milestone

**Insertion target**: end of §6 «Document maintenance requirements».

**New text** (new sub-section appended to §6):
```
### 6.6 — K-L3.1 amendment execution (deferred to follow-up brief)

**Trigger**: K-L3.1 amendment plan execution (separate Cloud Code amendment brief, post-K-L3.1 closure 2026-05-10).
**Change**: per-document edits enumerated in `docs/K_L3_1_AMENDMENT_PLAN.md` (this plan's authoring deliverable):
- KERNEL_ARCHITECTURE.md `v1.3 → v1.5` (Part 0 K-L3 row + implication paragraph + Part 4 Decisions log + Part 2 K8.2 row + status line + closing v1.0 sediment)
- MOD_OS_ARCHITECTURE.md `v1.6 → v1.7` (lines 1149–1150 + §3.5 D-1 path orthogonality + §4.6 IModApi v3 + §11.1 M3.5 + §11.2 new ValidationErrorKind)
- MIGRATION_PLAN_KERNEL_TO_VANILLA.md `v1.0 → v1.1` (this document — header + §0.1 Phase A' integration + §0.3 Decision #9 + §1.2/§1.3/§1.5 extensions + §6.6 self-reference)
- MIGRATION_PROGRESS.md content sync (lines 35/407/443/457 «без exception» reframing) + new K-L3.1 closure entry
- Forward-track brief dispositions per addendum §A5.6 (K9 surgical [full authored brief], K8.3 surgical-to-scope [skeleton], K8.4 in-place rewrite [skeleton], K8.5 surgical [skeleton])

**Authority**: K-L3.1 session lock 2026-05-10 (Crystalka + Opus deliberation; Q1–Q6 + synthesis form §4.A locked); Phase A' sequencing companion at `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`.
**Estimated execution**: 30–60 min auto-mode; per-document atomic commit shape; test count delta zero.
```

### §3.8 §0.1 — Phase A' integration (sequence diagram amendment)

**Rationale**: Phase A' sequencing document at `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (authored 2026-05-10, companion to K-L3.1 brief) introduces a new structural unit between Phase A closure (K8.5) and Phase B start (M8.4). Phase A' contains: K-L3.1 deliberation (A'.0), amendment brief (A'.1), README cleanup (A'.2), push (A'.3), K9/K8.3/K8.4/K8.5 execution (A'.4–A'.7), K-closure report (A'.8 NEW), Architectural analyzer milestone (A'.9 NEW). Migration Plan v1.0 §0.1 currently shows single arrow Phase A → Phase B; v1.1 amendment integrates Phase A'.

**Old text** (§0.1 sequence diagram, lines ~29–42):
```
### 0.1 — Strategy: Option (II) "Struct-first sequential"

K-series (kernel migration: components class→struct, systems → SpanLease/WriteBatch, managed retire, ecosystem prep) executes **completely** before M-series (mod-OS migration: vanilla mod content population) begins mass migration.

```
Phase A: K-series kernel completion       (4-8 weeks at hobby pace)
  K8.2 → K8.3 → K8.4 → K8.5
       └──────────────────────┘
                    │
                    ▼
Phase B: M-series mod-OS migration         (5-10 weeks at hobby pace)
  M8.4 → M8.5 → M8.6 → M8.7 → M9 → M10.x
       └──────────────────────────────┘
                    │
                    ▼
Phase C: Post-migration kernel work        (per existing roadmap)
  K9 (field abstraction) → G-series (GPU compute)
```
```

**New text**:
```
### 0.1 — Strategy: Option (II) "Struct-first sequential" + Phase A' bridge

K-series (kernel migration: components class→struct, systems → SpanLease/WriteBatch, managed retire, ecosystem prep) executes **completely** before M-series (mod-OS migration: vanilla mod content population) begins mass migration. Phase A' (added per K-L3.1 amendment 2026-05-10) inserts a structural bridge between K-series kernel completion and M-series mass migration: bridge formalization + remaining K-series execution + K-closure report + architectural analyzer. Companion sequencing reference: `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`.

```
Phase A: K-series kernel foundation       (post-K8.2 v2, partial — K8.3-K8.5 deferred to Phase A')
  K8.2 v2 [DONE 2026-05-09]
                    │
                    ▼
Phase A': Bridge formalization + closure + analyzer  (10-16 weeks at hobby pace)
  A'.0 K-L3.1 deliberation [DONE 2026-05-10]
  A'.1 Amendment brief execution
  A'.2 README cleanup
  A'.3 Push to origin
  A'.4 K9 execution (full authored brief, awaiting)
  A'.5 K8.3 execution (skeleton → full brief → execute)
  A'.6 K8.4 execution (skeleton → full brief → execute)
  A'.7 K8.5 execution (skeleton → full brief → execute)
  A'.8 K-closure report (NEW — historical record + analyzer rule specification surface)
  A'.9 Architectural analyzer milestone (NEW — Roslyn analyzer encoding K-Lxx invariants;
       dual purpose: M-series migration verifier + architectural debugger)
       └─────────────────────────────────────────────────────────────────────────────────┘
                    │
                    ▼
Phase B: M-series mod-OS migration         (5-10 weeks at hobby pace; runs under analyzer protection)
  M8.4 → M8.5 → M8.6 → M8.7 → M9 → M10.x
       └──────────────────────────────┘
                    │
                    ▼
Phase C: Post-migration GPU compute        (per existing roadmap)
  G-series (Vulkan compute, per docs/GPU_COMPUTE.md v2.0)
```

(K9 moved from Phase C to Phase A'.4 per K8.0 closure Option c sequencing 2026-05-09; K9 is kernel-side independent of K8.3–K8.5 except in IModApi v3 surface which K8.4 ships.)
```

**Note on §0.1 prose paragraph**: the existing prose «The single arrow between phases A and B is the **architectural handoff»» wording becomes inaccurate post-amendment. Replace with «Phase A' is the **architectural bridge** between Phase A foundation (K8.2 v2) and Phase B mass migration (M8.4 onward)» — this prose-level edit folds into the §0.1 amendment alongside the diagram replacement.

---

## §4 MIGRATION_PROGRESS.md amendments

Live tracker — content sync, no version bump.

### §4.1 Last updated line

**Old text** (line 5):
```
**Last updated**: 2026-05-09 (K-Lessons closure — 4 pipeline-execution lessons formalized in METHODOLOGY.md v1.5 and KERNEL_ARCHITECTURE.md v1.3)
```

**New text**:
```
**Last updated**: 2026-05-10 (K-L3.1 bridge formalization closure — amendment plan authored at `docs/K_L3_1_AMENDMENT_PLAN.md`)
```

### §4.2 Current state snapshot — line 35 (K8.2 v2 framing correction)

**Old text** (line 35):
```
| **Last completed milestone** | K8.2 v2 (K-L3 «без exception» state achieved: K8.1 wrapper value-type refactor + 6 class→struct conversions + 6 empty TODO stub deletions + 12 ModAccessible annotation pass) — 2026-05-09 |
```

**New text**:
```
| **Last completed milestone** | K-L3.1 (bridge formalization — Path α/β peer paths locked; amendment plan authored at `docs/K_L3_1_AMENDMENT_PLAN.md`) — 2026-05-10. Previous: K8.2 v2 (K-L3 selective per-component closure: K8.1 wrapper value-type refactor + 6 class→struct conversions + 6 empty TODO stub deletions + 12 ModAccessible annotation pass) — 2026-05-09 |
```

### §4.3 Current state snapshot — Active phase line

**Old text** (line 34):
```
| **Active phase** | K9 / K8.3 (per Option c sequencing K9 runs before K8.3; K-Lessons + K8.2 v2 closed as the K-side foundation completion) |
```

**New text**:
```
| **Active phase** | Amendment brief (K-L3.1 amendment plan execution; docs-only; per-document atomic commits) → K9 / K8.3 (per Option c sequencing K9 runs before K8.3; K-Lessons + K8.2 v2 + K-L3.1 closed as the K-side foundation completion) |
```

### §4.4 K8.2 v2 entry header — line 407

**Old text** (line 407):
```
### K8.2 v2 — K-L3 «без exception» closure (single milestone)
```

**New text**:
```
### K8.2 v2 — K-L3 selective per-component closure (single milestone; framing reformulated by K-L3.1 amendment 2026-05-10)
```

### §4.5 K8.2 v2 KERNEL doc bump entry — line 443

**Old text** (line 443):
```
**KERNEL_ARCHITECTURE.md** v1.3 → v1.4: Part 2 K8.2 row reformulated; Part 0 K-L3 implication rewritten («без exception» state achieved); status line updated.
```

**New text**:
```
**KERNEL_ARCHITECTURE.md** v1.3 → v1.4 (header bump deferred; landed at v1.5 in K-L3.1 amendment): Part 2 K8.2 row reformulated; Part 0 K-L3 implication rewritten (post-K-L3.1: K8.2 v2 selective per-component application; «без exception» framing superseded); status line updated.
```

### §4.6 K8.2 v2 architectural decisions — line 457

**Old text** (line 457, last bullet of «Architectural decisions LOCKED»):
```
  - K-L3 «без exception» state is now an architectural fact for `src/DualFrontier.Components/`. KERNEL_ARCHITECTURE.md Part 0 implication updated.
```

**New text**:
```
  - K-L3 selective per-component application is the architectural fact for `src/DualFrontier.Components/` (25 surviving components: 6 conversions via K8.1 primitives + 19 verify-only annotations on already-struct + 6 deletions per METHODOLOGY §7.1). KERNEL_ARCHITECTURE.md Part 0 implication updated. Post-K-L3.1 (2026-05-10): «без exception» framing reformulated as «selective per-component»; bridge formalization adds Path β as first-class peer.
```

### §4.7 New K-L3.1 closure entry

**Insertion target**: after K8.2 v2 entry (line ~467) and before K8.3-K8.5 sub-milestones row (line 469).

**New text** (full new sub-section):
```
### K-L3.1 — Bridge formalization (architectural decision session)

- **Status**: DONE (2026-05-10)
- **Brief**: `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` + `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md` (FULL EXECUTED — Phase 0 reads + Phase 1 deliberation Q1–Q6 + Phase 3 synthesis + Phase 4 amendment plan)
- **Brief type**: Architectural decision brief (fourth brief type, K8.0 precedent)
- **Trigger**: post-K8.2 v2 cleanup session 2026-05-10 — Crystalka clarification «там был частичный перенос то что можно легко преобразовывать в struct было преобразовано, а что нет то managed, так как не все имеет смысл тащить в натив для скорости» revealed K8.2 v2 closure framing «K-L3 «без exception»» as misframing of selective per-component judgment actually applied; bridge formalization escalated per METHODOLOGY §3 «stop, escalate, lock»
- **Session length**: deliberation 2026-05-10 (single session, Crystalka + Opus, no Cloud Code execution)
- **Test count**: 631 (unchanged — deliberation session, no source edits; Phase 4 deliverable is documentation plan)

**Locks** (Phase 1 deliberation closures):

- **Q1 = (a)**: `[ManagedStorage]` attribute on type. Class `: IComponent` types annotated with this attribute are Path β; absent attribute + struct shape implies Path α. Analyzer enforcement of shape↔attribute consistency deferred to post-migration analyzer milestone (Q5.b/M3.5-extension).
- **Q2 = (β-i)**: mod-side managed-storage ownership; `IModApi` v3 extension `RegisterManagedComponent<T> where T : class, IComponent`. Storage lives in per-mod `RestrictedModApi` instance; reclaimed deterministically on `AssemblyLoadContext.Unload`.
- **Q3 = (i)**: explicit dual API. `SystemBase.NativeWorld.AcquireSpan<T>()` for Path α (existing K8.2 v2 plumbing); `SystemBase.ManagedStore<T>()` for Path β (new K8.4 plumbing). Cross-mod managed-path direct access structurally impossible by ALC isolation; cross-mod data flow via events per §6 three-level contracts.
- **Q4 = (b)**: managed-path forbidden for persisted components. Save system out of scope per migration plan §0.4 + §8.1; managed-path = runtime-only state; codec layer untouched.
- **Q5 = (a)**: passive performance metrics; `KernelCapabilityRegistry` tagged per-path; PERFORMANCE_REPORT splits native/managed. Active Roslyn analyzer enforcement (Q5.b) deferred per Crystalka 2026-05-10 «после миграции нужен будет анализатор... но это потом».
- **Q6 = (a)**: capability surface path-blind. `[ModAccessible]` already targets `Class | Struct` (K4 prerequisite); capability strings (`kernel.read:`, `mod.<id>.read:`) carry verb + FQN, path-orthogonal — confirms existing structural reality.
- **Synthesis = §4.A**: amend K-L3 wording (single principle, peer paths) rather than add new K-L12. KERNEL `v1.3 → v1.5`; MOD_OS `v1.6 → v1.7`; MIGRATION_PLAN `v1.0 → v1.1`.

**Architectural decisions LOCKED in this milestone**:

- Path α (`unmanaged struct` / native) and Path β (managed `class` via `[ManagedStorage]` / mod-side) are first-class peers, not principle/exception
- K-L3 implication post-K-L3.1: components are either path; default is α (silent + struct shape); β requires `[ManagedStorage]` opt-in
- Mod-side managed-storage ownership preserves K-L11 «NativeWorld single source of truth» for native data; managed-storage decentralization-by-mod is consistent with K-L9 «vanilla = mods» + ALC isolation
- Path β components are runtime-only at K-L3.1; persistence opt-in (if/when needed) is a future amendment milestone
- Capability model uniform across paths (Q6.a confirmed structural truth)
- K8.2 v2 closure framing «K-L3 «без exception» state achieved» reformulated as «K-L3 selective per-component application» — closure outcome was selective judgment per METHODOLOGY §7.1 (delete) + per K8.1 primitive coverage (convert) + verify-only annotations (already struct), not universal mandate

**Output artifacts**:

1. `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (Status: AUTHORED → EXECUTED 2026-05-10)
2. `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md` (skeleton brief disposition extension — APPLIED)
3. `docs/K_L3_1_AMENDMENT_PLAN.md` (NEW — Phase 4 deliverable; old/new text pairs for KERNEL + MOD_OS + MIGRATION_PLAN + MIGRATION_PROGRESS + skeleton brief dispositions)
4. This MIGRATION_PROGRESS entry (added by amendment brief atomic with line corrections)

**Cross-cutting impact**:

- **Amendment brief = Phase A'.1**: docs-only execution per `docs/K_L3_1_AMENDMENT_PLAN.md`; KERNEL `v1.3 → v1.5`, MOD_OS `v1.6 → v1.7`, MIGRATION_PLAN `v1.0 → v1.1` (includes §0.1 Phase A' integration) + this MIGRATION_PROGRESS sync. Estimated 30–60 min auto-mode. Test count delta zero (docs-only). Atomic commit shape: per-document.
- **Phase A' sequencing**: companion document at `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (authored 2026-05-10) anchors structural unit between K8.2 v2 closure (DONE) and M8.4 begin (Phase B). Phase A' contains 10 phases: A'.0 K-L3.1 (DONE), A'.1 amendment brief, A'.2 README cleanup, A'.3 push, A'.4 K9, A'.5 K8.3, A'.6 K8.4, A'.7 K8.5, A'.8 K-closure report (NEW), A'.9 Architectural analyzer (NEW). Cumulative duration ~10–16 weeks hobby pace; analyzer's dual purpose = M-series migration verifier + architectural debugger per Crystalka 2026-05-10 «анализатор будет верификатором миграции и будет нашим дебагером на баги которые не ловят тесты».
- **K9 brief = Phase A'.4** (full authored, ~1200 lines, AUTHORED awaiting execution per β6 sequencing — NOT a skeleton): Disposition B (surgical) — version refs (KERNEL v1.4+ instead of v1.0) + test baseline (631 instead of 538) update; scope unchanged (fields architecturally orthogonal to entity-component bridge per addendum §A2.5)
- **K8.3 skeleton brief = Phase A'.5** (~36 lines true skeleton): Disposition B-to-C — scope undercount correction (12 named systems → 34 actual per migration plan §1.2 reformulated scope) + dual-path access pattern wording per Q3.i; full brief authoring against post-K-L3.1 amended state
- **K8.4 skeleton brief = Phase A'.6** (~33 lines true skeleton): Disposition C.1 in-place rewrite — title «ManagedWorld retired» framing preserved (kernel-side managed `World` still retires per K-L11; managed-storage moves mod-side per K-L3.1, not stays kernel-side); add `RegisterManagedComponent<T>` to v3 surface deliverables; add `SystemBase.ManagedStore<T>()` plumbing
- **K8.5 skeleton brief = Phase A'.7** (~30 lines true skeleton): Disposition B (surgical) — content list expands to include bridge mechanism documentation (per-component path choice criterion, `[ManagedStorage]` usage, `ManagedStore<T>` access pattern, dual-API examples)
- **K-closure report = Phase A'.8** (NEW per Phase A' sequencing): structured document enumerating final K-Lxx invariants; dual purpose = (1) historical record of K-series, (2) formal analyzer rule specification surface; format = each invariant has formal statement + violation example + compliance example. Out of K-L3.1 amendment plan scope; tracked as future Phase A' milestone.
- **Architectural analyzer = Phase A'.9** (NEW per Phase A' sequencing): Roslyn analyzer encoding K-Lxx invariants per K-closure report; dual purpose = M-series migration verifier + architectural debugger; track B activation candidate per ROADMAP «Maximum Engineering Refactor»; M3.4 capability analyzer merge decision at analyzer brief authoring time. Out of K-L3.1 amendment plan scope; tracked as future Phase A' milestone.

**Lessons learned**:

- Architectural decision briefs (fourth brief type, K8.0 precedent) extended to K-L3.1 with «closure clarification triggers retroactive principle reformulation» case. Phase 0 reads → Phase 1 deliberation Q1–Q6 → Phase 3 synthesis → Phase 4 amendment plan format works for this case (K8.2 v2 closure framing was misread by closure report; K-L3.1 reformulates without invalidating closure outcome).
- B.2 finding (live `IModApi.RegisterComponent<T> where T : IComponent` is path-agnostic at code level despite K-L3 doc-level «universal mandate» framing) was the structural anchor enabling Reading γ. Bridge was empirically already compatible with existing surface; K-L3.1 formalized convention into LOCKED architectural decision rather than creating new mechanism.
- B.4 finding (K8.2 v2 closure outcome empirically embodies Reading γ — 6 §7.1 deletions are «not converted to struct, removed because data didn't exist», not «universally constrained») confirmed §4.A synthesis correctness. The closure outcome was selective per-component judgment, not universal mandate; §4.A captures this in K-L3 amendment without inventing new principle.
- Crystalka clarification («что можно легко преобразовывать в struct было преобразовано, а что нет — managed») as session trigger validated «stop, escalate, lock» rule (METHODOLOGY §3): closure framing misalignment was surfaced by user observation, not auto-detected by tooling; deliberation session is the structurally correct response (vs. inline closure-report patch which would have been a kostyl).
- Q6 (capability path-orthogonality) was already structurally true per K4 prerequisite (`[ModAccessible]` widened to `Class | Struct`). This is empirical evidence that some «open» architectural questions in deliberation briefs may already be answered by accumulated code state — Phase 0 inventory lessons (K-Lessons «inventory as hypothesis, not authority») extended: hypothesis can resolve question without deliberation when disk truth is decisive.
- Crystalka direction «без костылей, у меня нет давления временем» as session frame enabled rigorous Q1–Q6 walkthrough with each lock + rationale, rather than batched/forced answers under time pressure. Long-horizon framing aligns with brief §1.3 «architectural cleanness... десятилетиями».
```

---

## §5 Forward-track brief dispositions (per addendum §A5.6)

Each forward-track brief receives one of three dispositions per addendum §A5.6: untouched (A), surgical amendment (B), or scope reformulation (C — split into C.1 in-place rewrite and C.2 deprecate + author replacement).

**Brief state classification correction (per Crystalka 2026-05-10)**: addendum §A1 uses collective phrasing «K9 + K8.3 + K8.4 + K8.5 briefs all AUTHORED as skeletons» which is imprecise. **K9 is a full authored brief (~1200 lines)** awaiting execution per β6 sequencing post-K6/K7/K8 closure — `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` carries Phase 0–9 with detailed C++/managed/test specifications. **K8.3, K8.4, K8.5 are true skeletons** (~30–36 lines each) requiring full brief authoring before execution. The disposition assignments below preserve this distinction.

### §5.1 K9 — `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` (full authored brief, awaiting execution)

**State**: AUTHORED (full brief, ~1200 lines, Phase 0–9 specified). Awaits execution per β6 sequencing (Option c places K9 between K8.1 and K8.2; current state is post-K8.2 v2 + post-K-L3.1, so K9 is unblocked for execution after Phase A'.1 amendment lands).

**Disposition**: B (surgical amendment — version + baseline references only)

**Rationale**: K9 is architecturally independent of entity-component bridge per addendum §A2.5 — fields are spatial grid storage, separate `IModApi.Fields` v3 surface; K-L3.1 doesn't change K9 scope or deliverables. K9 brief Phase 0.4 carries stale version refs (expects KERNEL v1.0; current is v1.5 post-amendment) and stale test baseline (538 from K5; actual 631 post-K8.2 v2). Surgical edits update these informational checks; full Phase 0–9 specification holds.

**Edits** (executed by amendment brief or K9 brief authoring time, whichever earlier):

- **Phase 0.4 expected versions block** (around line 64–69):
  - Old: `KERNEL_ARCHITECTURE.md` AUTHORITATIVE LOCKED v1.0
  - New: `KERNEL_ARCHITECTURE.md` AUTHORITATIVE LOCKED v1.5+ (per K-L3.1 bridge formalization 2026-05-10)
  - Old: `MOD_OS_ARCHITECTURE.md` LOCKED v1.6
  - New: `MOD_OS_ARCHITECTURE.md` LOCKED v1.7+ (per K-L3.1 bridge formalization 2026-05-10)

- **Phase 0.7 expected baseline** (around line 99):
  - Old: `Expected: 538 tests passing (post-K5; K6–K8 closure may have added more — record the actual baseline before continuing).`
  - New: `Expected: 631+ tests passing (post-K8.2 v2 baseline; K-L3.1 amendment brief is docs-only and does not affect test count — record the actual baseline before continuing).`

- **Architectural reference clarification** (early sections that mention K8 prerequisite): no scope change; K9's RawTileField is orthogonal to entity-component bridge (Path α/β are component-scoped; fields are field-scoped). Add a single paragraph noting orthogonality if convenient during K9 execution; not required for closure.

**Scope changes**: none. K9 deliverables list unchanged.

### §5.2 K8.3 — `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md`

**Disposition**: B-to-C (surgical scope correction with possible authorship-time reformulation)

**Rationale**: K8.3 skeleton states scope as «12 vanilla systems» but migration plan v1.0 §1.2 reformulated to 34 systems (already a known correction). K-L3.1 adds dual-path access pattern requirement (Q3.i lock). Both corrections compose into K8.3 full-brief authoring (post-K-L3.1 amendment + post-K8.2 v2 closure).

**Edits** (executed by amendment brief; full brief authoring is separate post-K-L3.1):

- **Header «Systems in scope»** (around line 12):
  - Old: `Migrate the 12 vanilla production systems from `World.GetComponent` / `World.SetComponent` access patterns to `NativeWorld` + `SpanLease<T>` reads + `WriteBatch<T>` writes. After K8.3 closure, all production system code runs against NativeWorld.`
  - New: `Migrate all 34 production systems in `src/DualFrontier.Systems/` (per `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.1 §1.2 reformulated scope) from `World.GetComponent` / `World.SetComponent` access patterns to dual-path access per K-L3.1 Q3.i: `SystemBase.NativeWorld` + `SpanLease<T>` reads + `WriteBatch<T>` writes for Path α components; `SystemBase.ManagedStore<T>()` for Path β components (when present in same mod). After K8.3 closure, all production system code runs against `NativeWorld` (Path α) and per-mod `ManagedStore<T>` (Path β if any).`

- **Skeleton TODO list** (around line 27):
  - Insert new TODO bullet: «- [ ] Per-system Path α/β access audit — for each of 34 systems, identify Path α reads/writes and Path β reads/writes (Path β requires K8.4 plumbing; if K8.4 ships before K8.3 brief authoring, dual-path access enabled; if K8.3 authored before K8.4, Path β is empty and brief covers only Path α — defer Path β audit to amendment after K8.4)»

**Scope changes**: scope grows from 12 to 34 systems (already known per migration plan), plus dual-path access added (K-L3.1 new). Skeleton TODOs adjusted accordingly. Full brief authoring is post-K-L3.1 amendment closure + post-K8.4 closure (since dual-API plumbing requires K8.4).

### §5.3 K8.4 — `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md`

**Disposition**: C.1 (in-place rewrite at full brief authoring time; skeleton itself receives surgical edits)

**Rationale**: K-L3.1 is most consequential for K8.4. The skeleton's title «ManagedWorld retired as production» remains correct (kernel-side `World` class does retire — managed-storage moves mod-side per K-L3.1, not kernel-side peer). But K8.4 deliverables expand significantly: ships `RegisterManagedComponent<T>` v3 surface, ships `SystemBase.ManagedStore<T>()` plumbing, ships per-mod `ManagedStore<T>` storage in `RestrictedModApi`. The skeleton's «Mod API v3 ships» line covers this implicitly but needs explicit enumeration.

**Edits** (executed by amendment brief; full brief authoring is C.1 in-place rewrite at K8.4 time):

- **Header goal paragraph** (around line 8):
  - Old: `Remove `World` class from production code paths. `World` retained only as test fixture (renamed to `ManagedTestWorld` for clarity) and research reference. Bootstrap two-phase model becomes the only entry to production. Mod API v3 ships with NativeWorld-only access.`
  - New: `Remove kernel-side `World` class from production code paths. `World` retained only as test fixture (renamed to `ManagedTestWorld` for clarity per per `KERNEL_ARCHITECTURE.md` K-L11) and research reference. Bootstrap two-phase model becomes the only entry to production. Mod API v3 ships with two component-registration paths per K-L3.1: Path α via existing `RegisterComponent<T>` (NativeWorld), Path β via new `RegisterManagedComponent<T>` (per-mod `ManagedStore<T>` in `RestrictedModApi` instance). `SystemBase.ManagedStore<T>()` accessor ships alongside existing `SystemBase.NativeWorld`.`

- **Deliverables list** (around lines 16–22):
  - Insert new bullet after «`IModApi` v3 ships»: «- `RegisterManagedComponent<T> where T : class, IComponent` added to `IModApi` v3 (per K-L3.1 Q2.β-i lock). `RestrictedModApi` implementation creates per-mod `ManagedStore<T>` instance held in the mod's `RestrictedModApi` instance; reclaimed on `AssemblyLoadContext.Unload`.»
  - Insert new bullet: «- `SystemBase.ManagedStore<T>()` accessor ships (parallel to `SystemBase.NativeWorld` K8.2 v2 plumbing). Resolves via `SystemExecutionContext.Current.ModId` to owning mod's per-mod store. Type `T` must be a class annotated with `[ManagedStorage]`; absence triggers load-time `MissingManagedStorageAttribute` error.»
  - Insert new bullet: «- `MOD_OS_ARCHITECTURE.md` v1.7+ references (already amended at K-L3.1 amendment time per `docs/K_L3_1_AMENDMENT_PLAN.md` §2); K8.4 brief verifies the v1.7 wording against shipped Mod API v3 surface and adjusts further if needed (per migration plan §6 sequence).»

- **TODO list** (lines 25–30):
  - Insert TODO: «- [ ] Per-mod `ManagedStore<T>` implementation in `RestrictedModApi` — concrete data structure choice (Dictionary<EntityId, T>, custom hashmap, etc.); per-store lifecycle parallel to subscription cleanup (UnsubscribeAll precedent)»
  - Insert TODO: «- [ ] `MissingManagedStorageAttribute` error kind addition to `ValidationErrorKind` enum (per K-L3.1 Q5.b deferred enforcement — runtime check until M3.5 analyzer ships)»

**Scope changes**: K8.4 deliverables grow to include K-L3.1 bridge implementation. Estimated time goes from «1 week hobby» to potentially 1.5–2 weeks (pending K8.4 full brief authoring), still bounded.

### §5.4 K8.5 — `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md`

**Disposition**: B (surgical amendment)

**Rationale**: K8.5 is documentation milestone (mod authoring guide, migration guide, sample mod). K-L3.1 adds bridge mechanism documentation as new content scope.

**Edits** (executed by amendment brief; full brief authoring is post-K8.4 closure):

- **Deliverables list** (around lines 16–21):
  - Insert new bullet: «- Bridge mechanism documentation in `docs/MOD_AUTHORING_GUIDE.md`: per-component path choice criterion (Path α default, Path β opt-in via `[ManagedStorage]`); decision tree (when to use struct vs class); K8.1 primitive coverage table (which managed types convert to which K8.1 primitive); concrete examples (Identity-style InternedString, Skills-style NativeMap, Storage-style NativeMap+NativeSet, Movement-style NativeComposite); managed-path examples (job queue, AI working memory, animation state — runtime-only patterns).»
  - Insert new bullet: «- Dual-API access pattern documentation: `SystemBase.NativeWorld.AcquireSpan<T>()` + `WriteBatch<T>` for Path α; `SystemBase.ManagedStore<T>()` for Path β; mixed-path system examples (system reads native PositionComponent + writes managed JobQueueComponent on same entity); cross-mod managed-path NOT-allowed pattern (compile-time barrier via ALC) + event-mediated alternative.»
  - Insert new bullet: «- v2 → v3 migration guide includes: how to add `[ManagedStorage]` to existing class components, how to call `RegisterManagedComponent<T>` instead of `RegisterComponent<T>` for class types, how to access per-mod `ManagedStore<T>` from systems via `SystemBase.ManagedStore<T>()`.»

- **TODO list** (lines 25–28):
  - Insert TODO: «- [ ] Bridge documentation completeness audit — every K-L3.1 lock (Q1–Q6) documented with concrete example»

**Scope changes**: K8.5 documentation scope grows to cover bridge mechanism. Estimated time adjustment minor (3–5 days hobby → 4–6 days).

---

## §6 Atomic commit shape recommendation

Per K-Lessons «atomic commit as compilable unit» (METHODOLOGY v1.5): docs-only milestones use per-document atomic commits where each commit leaves the document in a coherent reviewable state. The K-L3.1 work splits into two milestones: **session output bundle** (lands now, K-L3.1 closure) and **Phase A'.1 amendment brief** (lands later, follow-up Cloud Code execution).

### §6.1 K-L3.1 session output bundle (commit now)

| # | Commit | Files | Scope |
|---|---|---|---|
| S1 | `docs(architecture): K-L3.1 bridge formalization session — brief, addendum, amendment plan, Phase A' sequencing` | `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (new tracked, EXECUTED), `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md` (new tracked, APPLIED), `docs/K_L3_1_AMENDMENT_PLAN.md` (new), `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (new) | Single bundled commit for all K-L3.1 session deliverables. Brief + addendum start tracked at EXECUTED/APPLIED status (already AUTHORED on disk pre-session). Amendment plan + Phase A' sequencing are Phase 4 outputs of the same session — bundled per K-Lessons «atomic = compilable unit» since these four files are co-dependent (amendment plan references brief/addendum/Phase A'; Phase A' references brief/amendment plan). |

**Single commit** for K-L3.1 session output. Test count delta zero. Repo on commit: K-L3.1 fully closed, amendment brief and downstream Phase A' phases unblocked.

### §6.2 Phase A'.1 amendment brief execution (commit later, separate Cloud Code session)

| # | Commit | Files | Scope |
|---|---|---|---|
| A1 | `docs(kernel): K-L3.1 bridge formalization; bump KERNEL v1.3→v1.5` | `docs/KERNEL_ARCHITECTURE.md` | All edits per §1 of this plan |
| A2 | `docs(modos): K-L3.1 bridge formalization; bump MOD_OS v1.6→v1.7` | `docs/MOD_OS_ARCHITECTURE.md` | All edits per §2 of this plan |
| A3 | `docs(migration): K-L3.1 bridge formalization + Phase A' integration; bump MIGRATION_PLAN v1.0→v1.1` | `docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | All edits per §3 of this plan (incl. §3.8 Phase A' integration into §0.1) |
| A4 | `docs(progress): record K-L3.1 closure; reframe K8.2 v2 «без exception»` | `docs/MIGRATION_PROGRESS.md` | All edits per §4 of this plan (line corrections + new K-L3.1 entry, atomic) |
| A5 | `docs(briefs): K9 surgical edits per K-L3.1 disposition B` | `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` | Per §5.1 (full authored brief, version refs only) |
| A6 | `docs(briefs): K8.3 surgical edits per K-L3.1 disposition B-to-C` | `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` | Per §5.2 (skeleton, scope undercount + dual-path note) |
| A7 | `docs(briefs): K8.4 surgical edits per K-L3.1 disposition C.1` | `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` | Per §5.3 (skeleton, RegisterManagedComponent + ManagedStore) |
| A8 | `docs(briefs): K8.5 surgical edits per K-L3.1 disposition B` | `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` | Per §5.4 (skeleton, bridge documentation expansion) |

**Total Phase A'.1**: 8 commits. **Test count delta**: zero per commit. **Each commit leaves repo in compilable + tests-passing state** (docs-only). Estimated 30–60 min auto-mode.

### §6.3 Combined commit count

K-L3.1 session output (§6.1: 1 commit) + Phase A'.1 amendment brief (§6.2: 8 commits) = **9 commits total** for K-L3.1 + Phase A'.1 deliverables. Per K-Lessons single-file-per-doc rule, this preserves bisect-friendliness for downstream debugging.

---

## §7 Cross-document drift audit (Phase 5 of amendment brief)

After commits 1–10 land, the amendment brief executes a cross-document grep verification per K-L3.1 brief §5.5:

**Audit greps** (each must return zero results in the 4 LOCKED docs after amendment):

```
grep -rn "без exception" docs/KERNEL_ARCHITECTURE.md docs/MOD_OS_ARCHITECTURE.md docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md docs/MIGRATION_PROGRESS.md
grep -rn "no exception" docs/KERNEL_ARCHITECTURE.md docs/MOD_OS_ARCHITECTURE.md docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md docs/MIGRATION_PROGRESS.md
grep -rn "K-L3 violation" docs/KERNEL_ARCHITECTURE.md docs/MOD_OS_ARCHITECTURE.md docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md docs/MIGRATION_PROGRESS.md
grep -rn "must be unmanaged struct" docs/KERNEL_ARCHITECTURE.md docs/MOD_OS_ARCHITECTURE.md docs/MIGRATION_PLAN_KERNEL_TO_VANILLA.md docs/MIGRATION_PROGRESS.md
grep -rn "Class-based component storage prohibited" docs/MOD_OS_ARCHITECTURE.md
```

**Acceptable matches**: only in version-history entries that quote prior wording for traceability (e.g. v1.1 entry recording the v1.0 framing being corrected). All matches must be in version-history quote-context, never in active spec wording.

**Stop condition**: any match in active spec wording — amendment brief halts, escalates to chat session for Phase 5.x correction.

---

## §8 What this plan deliberately does not do

- **No source code changes**. Plan covers documentation only.
- **No test additions**. Plan is docs-only; test count delta zero per commit.
- **No K-Lxx renumbering**. Per synthesis form §4.A, K-L3 grows in scope; no new K-L12 (which would be §4.B); existing K-L1..K-L11 unchanged.
- **No re-deliberation**. Q1–Q6 + synthesis are LOCKED per session 2026-05-10; plan executes on those locks.
- **No K8.4 implementation specification**. Plan documents what K8.4 brief must include; full K8.4 brief authoring is separate.
- **No analyzer authoring**. Q5.b deferred to post-migration analyzer milestone (M3.5 extension); plan only references the deferral.
- **No save-system design**. Q4.b runtime-only managed-path; save system out of scope per migration plan §0.4 + §8.1.

---

## §9 Document end

**Plan authored**: 2026-05-10, Opus 4.7, K-L3.1 session Phase 4
**Authority**: K-L3.1 session locks (Q1=a, Q2=β-i, Q3=i, Q4=b, Q5=a, Q6=a, Synthesis=§4.A)
**Execution target**: amendment brief, 10 atomic commits, 30–60 min auto-mode, test count delta zero
**Companion**: `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` + `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md`

End of K-L3.1 amendment plan. Amendment brief authoring is a separate session deliverable; this plan is the input contract.
