---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-MIGRATION_PLAN
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.1"
next_review_due: 2027-05-10
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-MIGRATION_PLAN
---
# MIGRATION PLAN — Kernel-to-Vanilla (K-series → M-series)

**Version**: 1.1 LOCKED (v1.1 = non-semantic correction post-K-L3.1 amendment 2026-05-10)
**Authored**: 2026-05-09 (post-K-Lessons closure `9df2709..071ae11`); amended 2026-05-10 (K-L3.1 bridge formalization, Decision #9 added, line 62 reframed, K8.3-K8.5 sub-section extensions)
**Locked**: 2026-05-09 (Crystalka acceptance during K8.2 v2 session)
**Status**: AUTHORITATIVE LOCKED — architectural roadmap for K-series and M-series
**Strategy**: Option (II) "Struct-first sequential" — kernel-track closes completely before mod-OS-track mass migration begins
**Sequencing parent**: β6 (kernel-first sequential per K2 closure 2026-05-07; Option c per K8.0 closure for K9 placement)

**Authoritative documents this plan integrates and reformulates**:
- `KERNEL_ARCHITECTURE.md` v1.3 LOCKED — K-series specification (Part 0, Part 2 master plan)
- `MOD_OS_ARCHITECTURE.md` v1.6 LOCKED — M-series specification (§1.3 strategic LOCKED #4, §11 migration phases)
- `METHODOLOGY.md` v1.5 — pipeline closure lessons reference
- `MIGRATION_PROGRESS.md` (live tracker) — current state snapshot
- `tools/briefs/*` — milestone brief precedents (K8.1, K8.1.1, K-Lessons)

**Document purpose** (per Crystalka request 2026-05-09): "правильно завершить миграцию K и подготовить документы к другим миграциям". This is the **architectural roadmap document**, not a brief. After this document is locked, individual milestone briefs (K8.2, K8.3, K8.4, K8.5, M8.4, M8.5-M8.7, M9, M10.x) are authored against its constraints.

**Output type**: architectural plan (input to brief authoring), not executable brief. No commit shape, no Phase 0-5 structure. Plan readers are: (a) Opus future sessions authoring the K8.2+/M8.4+ briefs, (b) Crystalka, as architectural reference.

---

## Section 0 — Executive summary and strategy lock

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
  G-series (Vulkan compute, per docs/architecture/GPU_COMPUTE.md v2.0)
```

(K9 moved from Phase C to Phase A'.4 per K8.0 closure Option c sequencing 2026-05-09; K9 is kernel-side independent of K8.3–K8.5 except in IModApi v3 surface which K8.4 ships.)

Phase A' is the **architectural bridge** between Phase A foundation (K8.2 v2) and Phase B mass migration (M8.4 onward): K8.5 closure (via A'.7) delivers a fully-K-completed `src/` codebase as the input substrate for M-series. M-series migrates that substrate from `src/` into `mods/Vanilla.*/` assemblies, **without re-doing K-series work**. Each component is converted class→struct (Path α) or annotated `[ManagedStorage]` (Path β) **once**, in `src/`. Each system is migrated to SpanLease/WriteBatch **once**, in `src/`. The migration into vanilla mods is a code-relocation operation, not a re-architecture.

### 0.2 — Why this strategy (decision rationale)

The competing strategies considered in the chat session of 2026-05-09:

- **Option (I) "Class-first move"**: M-series first (relocate legacy class components/systems into vanilla mods), K-series after (convert in vanilla mod assemblies).
- **Option (II) "Struct-first sequential"** (chosen): K-series first in `src/`, M-series after.
- **Option (III) "Combined vertical per slice"**: each slice (Pawn, Combat, ...) migrates atomically — class→struct conversion + relocation in one milestone per slice. K8.2/K8.3 dissolved into M-milestones.

Option (II) chosen by Crystalka 2026-05-09 with the architectural framing:

> «Чистый вариант сначала закрытие миграции K, а потом уже M, всё та архитектура позволила провести тесты и узнать что она имеет смысл»

The framing is structurally important: existing `src/DualFrontier.Components/*` and `src/DualFrontier.Systems/*` served their architectural purpose — they were the **test polygon** that validated K-series patterns end-to-end (K8.1 native primitives, K7 performance evidence, K6 hot-reload, K0-K5 native foundation). With K-Lessons formalized 2026-05-09, all four pipeline lessons came out of K8/K8.1 era closures. The legacy code is what made the foundation real.

After K-series closure on this codebase, the legacy content has accomplished its job. M-series then re-houses the closed content into vanilla mod assemblies. **No architectural work happens in two places** — kernel work in `src/`, mod-OS work in `mods/`, sequential.

Option (I) was rejected because it would put the mass migration work in front of the kernel work — vanilla mods would temporarily contain class components without `[ManagedStorage]` opt-in (K-L3 path-declaration ambiguity period of weeks-months), then K8.2 would convert/annotate them later. That's a second pass over the same code; under "без костылей" (Crystalka, repeated 2026-05-09), the second pass is structural cost without architectural benefit. Note (post-K-L3.1, 2026-05-10): post-amendment, class components carrying `[ManagedStorage]` are first-class Path β peers — the original «K-L3 violation» framing applied to undeclared class components only, not to managed-path components per se.

Option (III) was rejected because it would dissolve the K-series milestone structure mid-stream — KERNEL_ARCHITECTURE Part 2 K8.2/K8.3 rows would become subsumed into M-series, and the kernel completion gate would no longer have a clean closure milestone. Single architectural focus per period (Crystalka philosophy "cleanness > expediency", encoded in β6 sequencing decision 2026-05-07) is preserved by Option (II).

### 0.3 — LOCKED architectural decisions of this plan

The following 8 decisions are committed by this plan and cannot be revisited inside individual K8.x or M8.x/M9/M10 milestone briefs without re-opening this document:

1. **Sequence LOCKED**: K8.2 → K8.3 → K8.4 → K8.5 → M8.4 → M8.5 → M8.6 → M8.7 → M9 → M10.x. No interleaving. K-track closes before M-track begins beyond M8.1 (skeletons, already done).

2. **Existing-code purpose LOCKED**: `src/DualFrontier.Components/*` (31 components across 7 slices) and `src/DualFrontier.Systems/*` (34 systems across 7 slices) are **production code under K-series migration during Phase A**. They are not "legacy to delete" during Phase A. After Phase A closure they become **migration source** for Phase B; deletion happens **after** each slice's M-milestone closure (per slice, atomic with relocation) — Phased delete strategy in §5.

3. **K8.2 reformulated LOCKED**: scope is **all 31 production components in `src/DualFrontier.Components/`**, not "remaining 7" per old KERNEL_ARCHITECTURE Part 2 row. The original Part 2 wording was authored against an outdated assumption. K8.2 brief will audit the actual `src/Components/` inventory at brief-authoring time and convert all class components to `unmanaged` structs using K8.1 primitives where applicable.

4. **K8.3 reformulated LOCKED**: scope is **all 34 production systems in `src/DualFrontier.Systems/`**, not "12 vanilla systems" per old Part 2 row. Same correction principle. K8.3 brief will audit and migrate all systems to `SpanLease<T>` reads + `WriteBatch<T>` writes.

5. **K8.4 LOCKED**: managed `World` retired as production path within `src/`. Mod API v3 ships (per `MOD_OS_ARCHITECTURE.md` §4.6 v1.6 spec). No M-series prerequisites — purely kernel-side.

6. **K8.5 LOCKED**: bridge between Phase A and Phase B. Documentation, capability annotation pass on production components (`[ModAccessible]` per D-1 LOCKED), readiness gate. Does **not** start migration into vanilla mods.

7. **M-series unchanged structurally LOCKED**: M8.4, M8.5-M8.7, M9, M10.x sequence preserves `MOD_OS_ARCHITECTURE.md` §11. The vanilla mod doc-comments (in M8.1 skeleton commit `cafedcf`) authoritatively assign which slice goes to which M-milestone:
 - Vanilla.World → **M8.4**
 - Vanilla.Pawn → **M8.5-M8.7** (ConsumeSystem / SleepSystem / ComfortAuraSystem)
 - Vanilla.Combat → **M9** (Combat + Faction folded under Combat)
 - Vanilla.Inventory → **M10** (Inventory + Power folded under Inventory)
 - Vanilla.Magic → **M10.B**
 - Vanilla.Core → **M10 incremental** (shared types as cross-slice need surfaces)

8. **Phased delete LOCKED**: after each M-milestone closes, the corresponding `src/DualFrontier.Components/<Slice>/` and `src/DualFrontier.Systems/<Slice>/` directories are **deleted** in the same milestone's atomic commit set. No "delete legacy first, migrate later" — always "migrate then delete migrated source." Foundation never loses functionality between milestones (commits can be reverted; tests can be run at any commit). See §5 for full strategy.

9. **Bridge formalization LOCKED (K-L3.1, 2026-05-10)**: Path α (`unmanaged struct`, kernel-side `NativeWorld`) and Path β (managed `class` via `[ManagedStorage]`, mod-side per-mod `ManagedStore<T>`) are first-class peers per K-L3.1 amendment to `KERNEL_ARCHITECTURE.md` Part 0 K-L3. Decision criterion: per-component architectural fit; default Path α; Path β opt-in via attribute. K8.2 v2 closure framing «K-L3 «без exception»» reframed as «K-L3 selective per-component application» (closure delivered selective judgment, not universal mandate). M-series vanilla mod content milestones author per-component on appropriate path. K8.3 system migration extends to dual-path access (`SystemBase.NativeWorld` + `SystemBase.ManagedStore<T>`); per-system access pattern decision in K8.3 brief. K8.4 ships `IModApi.RegisterManagedComponent<T>` as part of v3 surface alongside `Fields` and `ComputePipelines`. K8.5 mod authoring guide documents per-component path choice criterion. Save system out of scope (Q4.b runtime-only managed-path). Authority: K-L3.1 amendment plan at `docs/architecture/K_L3_1_AMENDMENT_PLAN.md`; bridge brief at `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`.

### 0.4 — What this plan deliberately does not decide

To prevent scope creep into the K8.2/K8.3/etc. milestone briefs themselves:

- **K8.2 brief content**: this plan reformulates K8.2's scope (31 components instead of 7) and locks the strategy (struct-first), but does not author the brief. The K8.2 brief is a separate Opus session deliverable, written against this plan's constraints.
- **Per-component migration design**: which K8.1 primitive each component uses, ordering rules within K8.2, test fixture strategy — all in K8.2 brief.
- **Per-system migration design**: same for K8.3.
- **Save system implementation**: explicitly out of scope per Crystalka 2026-05-09 («системы сохранений нет, foundation first; будет внедряться от фундамента, не наоборот»). Save system follows post-G-series, against fully-completed K+M+G foundation.

---

## Section 1 — Phase A: K-series kernel completion

This section reformulates K8.2 through K8.5 in light of the actual `src/` inventory and the LOCKED Option (II) strategy. The reformulations supersede the rows in `KERNEL_ARCHITECTURE.md` Part 2 master plan and require an amendment to that document (see §6).

### 1.1 — K8.2: Component class→struct conversion (in `src/`)

**Old Part 2 wording**: "7 class components redesigned to structs (Movement, Identity, Skills, Social, Storage, Workbench, Faction). 1-2 weeks. -200/+300 LOC."

**New scope (LOCKED by this plan)**: **all 31 production components in `src/DualFrontier.Components/`**, distributed across 7 slices.

**Inventory** (verified by directory_tree read 2026-05-09):

| Slice | Path | Components | Count |
|---|---|---|---|
| Building | `src/DualFrontier.Components/Building/` | PowerConsumer, PowerProducer, Storage, Workbench | 4 |
| Combat | `src/DualFrontier.Components/Combat/` | Ammo, Armor, Shield, Weapon | 4 |
| Items | `src/DualFrontier.Components/Items/` | Bed, Consumable, DecorativeAura, Reservation, WaterSource | 5 |
| Magic | `src/DualFrontier.Components/Magic/` | Ether, GolemBond, Mana, School | 4 |
| Pawn | `src/DualFrontier.Components/Pawn/` | Identity, Job, Mind, Movement, Needs, Skills, Social | 7 (+ JobKind, NeedKind, SkillKind enums) |
| Shared | `src/DualFrontier.Components/Shared/` | Faction, Health, Position, Race | 4 |
| World | `src/DualFrontier.Components/World/` | Biome, EtherNode, Tile | 3 (+ TerrainKind enum) |
| **Total** | | | **31 components** |

The "remaining 7" wording in the old Part 2 row was authored against an out-of-date inventory model (probably from K4 era, when 24 of 31 were already converted as Hybrid Path). With Hybrid Path retired in K-L3 implication post-K8.1, **all 31 components are subject to K8.2 conversion**. Brief authoring will start with a fresh `directory_tree` audit to confirm the count at brief-authoring time.

**Architectural design constraints that K8.2 brief must inherit from this plan**:

- Conversion target: default Path α (`unmanaged struct`, K-L3 LOCKED). Per-component Path β (managed `class` via `[ManagedStorage]`) opt-in per K-L3.1 (2026-05-10) when conversion forces structural compromise; K8.2 v2 closure delivered selective per-component application (not universal mandate). See `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication post-K-L3.1 for the decision criterion.
- K8.1 primitives applied where component shape requires reference data:
 - `string` field → `InternedString` (e.g., `IdentityComponent.Name`, `FactionComponent.FactionId`)
 - `Dictionary<K,V>` field → `NativeMap<K,V>` (e.g., `SkillsComponent.Levels`)
 - `HashSet<T>` field → `NativeSet<T>` (e.g., `StorageComponent.AllowedItems`)
 - `List<T>` field → Composite or NativeComposite (e.g., `MovementComponent.Path`)
 - Empty stub component (e.g., `SocialComponent` with `// TODO: ...` body) — METHODOLOGY §7.1 "data exists or it doesn't" applies; the empty stub is a placeholder lie. **Resolution deferred to K8.2 brief authoring**, but this plan locks the principle: the empty SocialComponent does not survive K8.2 in its current shape (either deleted, converted to empty struct for K-L3 consistency, or waits for actual social-feature design).
- Test fixture strategy: per K8.2 brief decision; this plan does not lock between (A) per-component test class, (B) feature-axis, or (C) hybrid. Crystalka previously indicated Q3=(A) preference but final lock is K8.2 brief.
- Test count delta: per K8.2 brief, new baseline expected (Q-Tests=(iii) per Crystalka 2026-05-09). 592 → unknown. Test count may decrease if placeholder-lie tests removed; that is architecturally correct, not regression.

**Persistence layer**: out of K8.2 scope. `src/DualFrontier.Persistence/Compression/*` codec library is **untouched** by K8.2. Component snapshots in `src/DualFrontier.Persistence/Snapshots/*` are value-only DTO records and are **also untouched** — they don't depend on whether the underlying component is class or struct. Save system reconciliation (when save system implementing actually begins) is a separate future milestone, not part of K8.2.

**Estimated time**: 2-4 weeks at hobby pace, ~6-12 hours auto-mode (substantially larger than old "1-2 weeks" because actual scope is 4.4x larger, and component conversion against K8.1 primitives is non-trivial per shape). Brief will calibrate.

**Estimated LOC delta**: +800 / -1200 net (substantial deletion as Dictionary<K,V> field declarations replace by NativeMap<K,V> handles, plus K8.1 primitive call sites add per-component setup code).

### 1.2 — K8.3: System migration to SpanLease/WriteBatch (in `src/`)

**Old Part 2 wording**: "12 vanilla systems migrated to SpanLease/WriteBatch. 2-3 weeks. -400/+600 LOC."

**New scope (LOCKED by this plan)**: **all 34 production systems in `src/DualFrontier.Systems/`**, distributed across 7 slices.

**Inventory** (verified by directory_tree read 2026-05-09):

| Slice | Path | Systems | Count |
|---|---|---|---|
| Combat | `src/DualFrontier.Systems/Combat/` | CombatSystem, ComboResolutionSystem, CompositeResolutionSystem, DamageSystem, ProjectileSystem, ShieldSystem, StatusEffectSystem | 7 |
| Faction | `src/DualFrontier.Systems/Faction/` | RaidSystem, RelationSystem, TradeSystem | 3 |
| Inventory | `src/DualFrontier.Systems/Inventory/` | CraftSystem, HaulSystem, InventorySystem | 3 |
| Magic | `src/DualFrontier.Systems/Magic/` | EtherGrowthSystem, GolemSystem, ManaSystem, RitualSystem, SpellSystem (+ Internal/ManaLease helpers) | 5 |
| Pawn | `src/DualFrontier.Systems/Pawn/` | ComfortAuraSystem, ConsumeSystem, JobSystem, MoodSystem, MovementSystem, NeedsSystem, PawnStateReporterSystem, SkillSystem, SleepSystem, SocialSystem | 10 |
| Power | `src/DualFrontier.Systems/Power/` | ConverterSystem, ElectricGridSystem, EtherGridSystem | 3 |
| World | `src/DualFrontier.Systems/World/` | BiomeSystem, MapSystem, WeatherSystem | 3 |
| **Total** | | | **34 systems** |

The "12 vanilla systems" in the old wording was either a slice-count approximation (7 slices) or an early estimate from a different design era. Real count = 34.

**Architectural design constraints that K8.3 brief must inherit from this plan**:

- Read pattern: `using var lease = world.AcquireSpan<TComponent>();` followed by zero-P/Invoke iteration, per K5 protocol.
- Write pattern: `var batch = new WriteBatch<TComponent>(); batch.Add(entity, component);` then `world.FlushWrites(batch);` per K5 protocol.
- Mutation rejection: any system that holds a span and attempts to write the same component type without releasing first triggers `IsolationViolationException` per K5 invariant. K8.3 brief must verify each system's read/write pattern is consistent.
- System dependencies on K8.2 closure: every system reads at least one component, so K8.3 cannot start until K8.2 closes (all components are already struct).
- Tests: each system gets equivalence tests against pre-migration behavior. New baseline.
- **Dual access pattern (post-K-L3.1)**: each system accesses Path α components via existing `SystemBase.NativeWorld.AcquireSpan<T>()` + `WriteBatch<T>` (K8.2 v2 plumbing); Path β components (when present in same mod) via new `SystemBase.ManagedStore<T>()` (K8.4 plumbing). Per-system access-pattern audit (which components each system reads/writes and on which path) is K8.3 brief authoring concern; this plan locks only the dual-API mechanism per K-L3.1 Q3.i. Cross-mod managed-path direct access is structurally impossible (ALC isolation per K-L9); cross-mod data flow uses events/intents per `MOD_OS_ARCHITECTURE.md` §6.

**Estimated time**: 4-6 weeks at hobby pace, ~12-20 hours auto-mode. Brief will calibrate.

**Estimated LOC delta**: -1500 / +1200 net (system bodies become more idiomatic, per-system ceremony reduces — span/batch pattern eliminates `_world.Get<T>(entity)` per-call P/Invoke ceremony from K7 era).

### 1.3 — K8.4: Managed World retirement + Mod API v3 ship

**Old Part 2 wording**: "ManagedWorld retired; Mod API v3 ships. 1 week. -2000/+200 LOC."

**Reformulation**: scope unchanged from old wording; this plan does **not** reformulate K8.4 itself, only re-positions it. K8.4 is a kernel-internal milestone — no `src/Components/` or `src/Systems/` work; rather:

- **Managed `World` class deletion** as production path. `World` retained as test fixture and research reference per K-L11. All production code constructs `NativeWorld` via Bootstrap two-phase model (per K-L8 implication post-K8.4 closure: «Post-K8.4 closure, NativeWorld is the only production storage path»).

- **Mod API v3 ship** per `MOD_OS_ARCHITECTURE.md` §4.6 v1.6 LOCKED. Adds `Fields` and `ComputePipelines` sub-APIs to `IModApi` (gating K9 + G-series). Backward compatibility with v2 mods preserved (default-null sub-APIs).

- **Mod API v3 `RegisterManagedComponent<T>` ships** per K-L3.1 Q2.β-i lock. Surface added to `IModApi` v3 alongside existing `RegisterComponent<T>` (Path α), `Fields`, and `ComputePipelines` (v1.6 additions). `RestrictedModApi.RegisterManagedComponent<T>` creates per-mod `ManagedStore<T>` instance held in the RestrictedModApi instance; storage decentralized per-mod, reclaimed on `AssemblyLoadContext.Unload` per `MOD_OS_ARCHITECTURE.md` §9.5. `SystemBase.ManagedStore<T>()` accessor resolves via `SystemExecutionContext.Current.ModId` to owning mod's store. Loader registers Path α via existing `RegisterComponent<T>` (NativeWorld), Path β via new `RegisterManagedComponent<T>` (per-mod ManagedStore). Path β is runtime-only (Q4.b lock); save system reconstructs on load post-G-series.

- **`[ModAccessible]` annotation pass on `src/Components/*`** — every production component opts in to mod-readable/writable access per D-1 LOCKED. This is the foundation for M-series: vanilla mods must declare `kernel.read:DualFrontier.Components.X.YComponent` capabilities, and those capabilities only resolve if the component carries the annotation.

- **`KernelCapabilityRegistry`** finalized (per M3.5 deferred from MOD_OS_ARCHITECTURE §11.1, unblocked at K9 in-progress per v1.6) — but this plan **delays M3.5 to post-K9** since it depends on `[FieldAccessible]` annotation which doesn't exist until K9. K8.4 ships only the entity-component side of capability registry, sufficient for M8.4-M10.x migration.

**Estimated time**: 1-2 weeks at hobby pace, ~4-8 hours auto-mode.

**Estimated LOC delta**: -2000 / +400 (managed `World` deletion is large; replacement is small adapter shims + `[ModAccessible]` annotations + Mod API v3 surface).

### 1.4 — K8.5: Migration prep (Phase A→B handoff)

**Old Part 2 wording**: "Mod ecosystem migration prep. 3-5 days. +500 (docs)."

**Reformulation (this plan refines)**: K8.5 is the **gate document milestone** between Phase A and Phase B. It does not start vanilla mod content migration (that's M8.4 onward). K8.5 deliverables:

- **Migration guide for mod authors**: how to register components, systems, services in vanilla mod assemblies. Documents the IModApi v3 surface end-to-end with worked examples. Lives at `docs/MODDING_MIGRATION_GUIDE.md` (or similar).

- **Vanilla mod scaffolding readiness verification**: confirms every M8.1 skeleton (commit `cafedcf`) compiles against the post-K8.4 `IModApi` v3, manifests parse under v2 schema, capability strings validate. No skeleton gets new code; the verification is that the skeletons can be populated when M-series runs.

- **`MOD_OS_ARCHITECTURE.md` §11 reference verification**: confirm M8.4-M10.x rows still align with what K-series delivered (vanilla mod doc-comments cite `MOD_OS_ARCHITECTURE v1.5 §1.3 strategic LOCKED decision #4`; verify the citation still holds at v1.6 + K8.4 state).

- **Capability registry baseline**: `KernelCapabilityRegistry` (entity-component side from K8.4) snapshotted as the input for vanilla mod manifests. M8.4 brief authors against this snapshot.

- **Production component `[ModAccessible]` annotation completeness**: per-component review that every component Vanilla.* mods will read/write carries the annotation. Without this, M8.4 first vanilla mod content will fail with `MissingCapability` errors at load time.

- **`MIGRATION_PROGRESS.md` Phase B preparation section**: skeleton tracking entries for M8.4, M8.5-M8.7, M9, M10.x. Each entry has placeholders for SHA, date, brief reference. M-series sessions fill these in as they close.

**No code changes** outside docs and a small set of `[ModAccessible]` annotations confirmed in K8.4. K8.5 is primarily a documentation milestone, similar in shape to K-Lessons.

**Estimated time**: 3-5 days at hobby pace, ~1-2 hours auto-mode.

**Estimated LOC delta**: +600 / -50 (mostly docs).

### 1.5 — Phase A closure gate

After K8.5 closes:

- All 31 production components are `unmanaged` structs in `src/Components/*`
- All 34 production systems use SpanLease/WriteBatch in `src/Systems/*`
- Managed `World` retired as production path
- Mod API v3 ships
- `[ModAccessible]` annotation pass complete
- M-series migration guide published
- `MIGRATION_PROGRESS.md` carries closure entries for K8.2, K8.3, K8.4, K8.5
- Both Path α (`unmanaged struct` via `RegisterComponent<T>` → NativeWorld) and Path β (managed `class` via `[ManagedStorage]` + `RegisterManagedComponent<T>` → per-mod ManagedStore) registration paths active in `IModApi` v3 (K-L3.1 Q2.β-i)
- `SystemBase.ManagedStore<T>()` accessor available alongside existing `SystemBase.NativeWorld` (K-L3.1 Q3.i dual API)

The codebase is then in a **clean Phase B-ready state**: legacy (now correctly K-shaped) production code lives in `src/`, M-series will relocate it to `mods/Vanilla.*/` per slice.

**Test count expectation**: indeterminate — drops due to placeholder-lie test removal (Q-Tests=(iii)), grows due to per-component K8.2 tests, grows further due to per-system K8.3 tests. Brief authors estimate per milestone; aggregated across K8.2-K8.5, plausible end state is 600-900 tests.

---

## Section 2 — K → M architectural handoff

Section 1 closed Phase A. M-series begins from the resulting `src/` state. This section specifies the handoff: what M8.4 (first M-milestone in mass migration) inherits from K8.5 closure.

### 2.1 — Inputs to M8.4 from K8.5

**Codebase state at K8.5 close** (M8.4 brief authoring time):

- `src/DualFrontier.Components/{Building,Combat,Items,Magic,Pawn,Shared,World}/*.cs` — 31 unmanaged structs, all carrying `[ModAccessible]` annotations where mods will reference them.
- `src/DualFrontier.Systems/{Combat,Faction,Inventory,Magic,Pawn,Power,World}/*.cs` — 34 systems using SpanLease/WriteBatch patterns. Each system has explicit `[SystemAccess]` declarations.
- `src/DualFrontier.Application/Modding/` — Mod API v3 implementation including Fields and ComputePipelines (latter null-default if K9 not yet started).
- `src/DualFrontier.Contracts/Modding/IModApi.cs` — v3 interface.
- `mods/DualFrontier.Mod.Vanilla.{Pawn,Combat,Magic,Inventory,World,Core}/` — empty skeletons (M8.1 commit `cafedcf` state, untouched through Phase A).
- `docs/MODDING_MIGRATION_GUIDE.md` — K8.5 deliverable, authoritative reference for M-series brief authoring.
- `docs/architecture/KERNEL_ARCHITECTURE.md` v1.4+ (post-K8.5 amendment per §6 of this plan) — K8.x rows updated to reflect actual delivered state.

**Capability registry baseline**: snapshot of every capability string the vanilla mods will need, ready for inclusion in their manifests. Examples (drawn from K8.5 deliverable):

- `kernel.read:DualFrontier.Components.Pawn.IdentityComponent`
- `kernel.write:DualFrontier.Components.Pawn.NeedsComponent`
- `kernel.publish:DualFrontier.Events.Pawn.PawnHungryEvent` (if vanilla.pawn publishes such events)
- `kernel.subscribe:DualFrontier.Events.Pawn.JobAssignedEvent`

### 2.2 — What M-milestones do, structurally

Each M-milestone follows a common shape (M-template):

1. **Pre-flight**: verify K8.5 closure SHA, target slice's components/systems present in `src/` per-K8.5-state, target vanilla mod skeleton present and empty.
2. **Manifest population**: vanilla mod's `mod.manifest.json` updated with required capability strings for the slice's components/systems, dependencies updated (e.g., Vanilla.Pawn needs Vanilla.Core shared types when shared types eventually land).
3. **Component relocation**: `src/DualFrontier.Components/<Slice>/*.cs` files **moved** (git mv) to `mods/DualFrontier.Mod.Vanilla.<Slice>/Components/`. Namespace updated. `[ModAccessible]` retained — now self-asserted by the mod, not requested from kernel.
4. **System relocation**: `src/DualFrontier.Systems/<Slice>/*.cs` moved analogously to `mods/DualFrontier.Mod.Vanilla.<Slice>/Systems/`.
5. **IMod registration**: `<Slice>Mod.cs` `Initialize(IModApi api)` populated with `api.RegisterComponent<T>()` and `api.RegisterSystem<T>(...)` calls per the relocated content.
6. **Test relocation**: tests for the relocated components/systems moved to `tests/DualFrontier.Mod.Vanilla.<Slice>.Tests/` (new test project per slice).
7. **Source delete**: `src/DualFrontier.Components/<Slice>/` and `src/DualFrontier.Systems/<Slice>/` deleted (per §5 Phased delete).
8. **Closure**: tests pass under new mod-loaded shape; `MIGRATION_PROGRESS.md` updated.

The sequence is the same per M-milestone; only the slice changes. This is why §3 keeps M-milestone descriptions short — the per-milestone brief is mostly the slice-specific inventory and dependencies.

### 2.3 — What M-milestones explicitly do not do

- **No K-series work**: components are already struct, systems already use SpanLease/WriteBatch, managed World already retired. M-series only relocates.
- **No new architectural decisions**: any architectural surprise during relocation is escalated to chat session per «stop, escalate, lock» rule (METHODOLOGY §3) — not improvised.
- **No persistence layer changes**: `src/DualFrontier.Persistence/` untouched throughout M-series. Save system implementation comes after all M-milestones close (Crystalka Q5: «foundation first»).
- **No game-mechanics design**: M-series is structural relocation of K-completed code. Game mechanics design (e.g., the empty `SocialComponent` resolution, new gameplay features) is a separate post-M post-G phase.

---

## Section 3 — Phase B: M-series mod-OS migration sequence

This section specifies the M-milestone sequence. Detail is intentionally lower than Section 1 because each M-milestone follows the M-template (§2.2). Per-milestone briefs are authored against this plan's constraints in future sessions.

### 3.1 — M8.4: Vanilla.World

**Source slices**: `src/DualFrontier.Components/World/` (3 components: Biome, EtherNode, Tile + TerrainKind enum), `src/DualFrontier.Systems/World/` (3 systems: BiomeSystem, MapSystem, WeatherSystem).

**Target**: `mods/DualFrontier.Mod.Vanilla.World/`.

**Authority**: vanilla mod doc-comment (`WorldMod.cs`): «content lands in M8.4 (Item factory + 4 entity types)».

**Note**: doc-comment mentions "Item factory + 4 entity types" — this plan does not lock those specifics (factory pattern, entity types). M8.4 brief author resolves the specifics against K8.5 state. The phrasing suggests World mod ships not just slice components/systems but also some scaffolding for entity factory / item template patterns. K8.5 migration guide covers the IModApi for this; M8.4 brief specifies the World-specific application.

**Estimated time**: 1-2 weeks at hobby pace.

### 3.2 — M8.5-M8.7: Vanilla.Pawn (3 sub-milestones)

**Source slices**: `src/DualFrontier.Components/Pawn/` (7 components + 3 enums) + `src/DualFrontier.Components/Items/{Bed,Consumable,DecorativeAura,Reservation,WaterSource}.cs` (5 components), `src/DualFrontier.Systems/Pawn/` (10 systems).

**Target**: `mods/DualFrontier.Mod.Vanilla.Pawn/`.

**Authority**: vanilla mod doc-comment (`PawnMod.cs`): «content lands in M8.5–M8.7 (ConsumeSystem / SleepSystem / ComfortAuraSystem)».

**Sub-milestone split** (this plan locks the structure; M8.5 brief authors specifics):

- **M8.5**: ConsumeSystem migration (with NeedsComponent, ConsumableComponent, WaterSourceComponent — the eat/drink mechanic).
- **M8.6**: SleepSystem migration (with NeedsComponent (Sleep facet), BedComponent — the rest mechanic).
- **M8.7**: ComfortAuraSystem migration (with NeedsComponent (Comfort facet), DecorativeAuraComponent — the comfort aura mechanic).

The split is **mechanical-axis-based** — each sub-milestone is one wellness mechanic, not one component. This matches the userMemories wellness-system architecture (4 needs: Satiety, Hydration, Sleep, Comfort) and aligns with Crystalka's note that Comfort uses hybrid restoration (`ΔComfort = ΔSleep × 0.3` during bed sleep, plus passive ambient via `DecorativeAuraComponent`).

**Outstanding M8.5-M8.7 systems not in the doc-comment**: JobSystem, MoodSystem, MovementSystem, PawnStateReporterSystem, SkillSystem, SocialSystem, plus IdentityComponent, JobComponent, MindComponent, MovementComponent, SkillsComponent. These migrate in M8.7 or M10.x — this plan does not lock the sub-split; M8.5-M8.7 briefs do, with possible "M8.8" or "M8.9" extension if Pawn slice migration overflows the doc-comment's three sub-milestones.

**SocialComponent special case**: empty stub. Per METHODOLOGY §7.1 "data exists or it doesn't", the empty stub is a placeholder lie. K8.2 brief decides class→empty-struct or delete. **If K8.2 deletes it**, M8.x migration skips SocialSystem; SocialSystem itself becomes orphaned (no component to read/write) and **also** deletes during the same M-milestone. Tracked here so M8.x brief author doesn't try to migrate orphans.

**Estimated time**: 4-7 weeks at hobby pace across the three sub-milestones.

### 3.3 — M9: Vanilla.Combat (Combat + Faction folded)

**Source slices**: `src/DualFrontier.Components/Combat/` (4: Ammo, Armor, Shield, Weapon) + `src/DualFrontier.Components/Shared/{Faction,Health}.cs` (2 of 4 Shared components fold here), `src/DualFrontier.Systems/Combat/` (7 systems) + `src/DualFrontier.Systems/Faction/` (3 systems).

**Target**: `mods/DualFrontier.Mod.Vanilla.Combat/`.

**Authority**: vanilla mod doc-comment (`CombatMod.cs`): «content lands in M9 Combat (...mapping: Faction folds under Combat for the raid pipeline)».

**Faction-folded scope**: Faction systems (RaidSystem, RelationSystem, TradeSystem) are part of M9 Vanilla.Combat per the doc-comment. Faction component is in Shared/ but moves to Vanilla.Combat per this folding. (RelationSystem may overlap with future social-feature design; ledger noted, M9 brief decides.)

**Shared/Health.cs**: Health is read by Combat (DamageSystem writes; Shield reads), by Pawn (NeedsSystem may cross-reference), by Magic (RitualSystem may damage). Per `MOD_OS_ARCHITECTURE.md` §6.4, types that multiple regular mods reference must live in a **shared** mod. **Health migrates to Vanilla.Core (shared) as part of M9**, not to Vanilla.Combat. M9 brief addresses this.

**Shared/Position.cs and Shared/Race.cs**: similar shared-type analysis. Position is read by everything that has spatial location; Race by Pawn primarily but also potentially Magic for race-gated spells. Both **migrate to Vanilla.Core (shared) as part of M9 or M8.5**, brief decides which milestone.

**Estimated time**: 4-6 weeks at hobby pace.

### 3.4 — M10: Vanilla.Inventory (Inventory + Power folded)

**Source slices**: `src/DualFrontier.Components/Building/` (4: PowerConsumer, PowerProducer, Storage, Workbench), `src/DualFrontier.Systems/Inventory/` (3 systems) + `src/DualFrontier.Systems/Power/` (3 systems).

**Target**: `mods/DualFrontier.Mod.Vanilla.Inventory/`.

**Authority**: vanilla mod doc-comment (`InventoryMod.cs`): «content lands in M10 Inventory (...mapping: Power folds under Inventory for the economy pipeline)».

**Estimated time**: 3-5 weeks at hobby pace.

### 3.5 — M10.B: Vanilla.Magic

**Source slices**: `src/DualFrontier.Components/Magic/` (4: Ether, GolemBond, Mana, School), `src/DualFrontier.Systems/Magic/` (5 systems + Internal/ManaLease helpers).

**Target**: `mods/DualFrontier.Mod.Vanilla.Magic/`.

**Authority**: vanilla mod doc-comment (`MagicMod.cs`): «content lands in M10.B Magic».

**Note**: Magic slice is the most complex per system count (5 systems with internal helpers). May benefit from sub-milestone split during M10.B brief authoring (M10.B.1, M10.B.2, ...).

**Field-API integration**: Magic mod is a primary candidate for `IModApi.Fields` consumer (e.g., Mana field per `MOD_OS_ARCHITECTURE.md` §4.6.4 example). However, K9 (RawTileField) has not closed at M10.B authoring time per Phase A→B sequence. **Two scenarios**:

- **K9 closes after K8.5 but before M10.B brief authoring**: Magic mod uses Fields API directly, manifest declares `mod.dualfrontier.vanilla.magic.field.read:vanilla.magic.mana` and similar. M10.B brief includes Field registration code per `IModApi.Fields.RegisterField<float>("vanilla.magic.mana", w, h)`.

- **K9 not yet closed at M10.B brief authoring time**: Magic mod uses managed `Dictionary<GridVector, float>` interim storage with `IComponent`-keyed components. M10.B brief carries TODO marker for K9 follow-up: "Magic field-of-mana migrates to RawTileField when K9 closes, separate amendment milestone M10.B.field."

This plan does not lock between scenarios — depends on K9 timing relative to M10.B authoring. M10.B brief checks K9 status at brief-authoring time and routes accordingly.

**Estimated time**: 4-7 weeks at hobby pace (depending on K9 integration scenario).

### 3.6 — M10.Core: Vanilla.Core shared types

**Source**: cross-slice shared types identified during M8.4-M10.B migrations. The vanilla mod README (`mods/DualFrontier.Mod.Vanilla.Core/README.md`) states «Content (cross-slice shared definition records) lands when M10 incremental migration introduces shared types between mods.» The lock in this plan: M10.Core is **incremental and concurrent** with M8.4-M10.B — every time a slice migration discovers a type that needs sharing (e.g., Health, Position, Race during M9), that type lands in Vanilla.Core in the same milestone, not in a deferred standalone milestone.

This means **Vanilla.Core is not a single milestone** — it's an opportunistic addition stream during M8.4-M10.B. By M10.B closure, Vanilla.Core contains all the cross-slice types vanilla mods reference. No standalone "M10.Core" milestone is needed.

### 3.7 — Phase B closure gate

After M10.B closes (and M10.Core completed implicitly):

- All vanilla mod assemblies populated.
- `src/DualFrontier.Components/` empty (or contains only the directory listing for git, deleted entirely if last reference removes — see §5).
- `src/DualFrontier.Systems/` empty similarly.
- Tests run end-to-end: all 6 vanilla regular mods + 1 shared mod load via ModLoader, register components/systems, simulation runs.
- Test count: stable at whatever new baseline emerged from K8.2-M10 migrations.

---

## Section 4 — Per-slice migration mapping (consolidated)

Reference table for migration brief authors. Each row is one source slice; the destination is one vanilla mod. The K8.x column shows the kernel-side milestones that affect the slice; the M-x column shows the mod-OS milestone that relocates the slice.

| Source slice | Components | Systems | K8.x affecting | Destination mod | M-milestone |
|---|---|---|---|---|---|
| `Components/Building/` + `Systems/Inventory/` + `Systems/Power/` | 4 (PowerConsumer, PowerProducer, Storage, Workbench) | 6 (CraftSystem, HaulSystem, InventorySystem, ConverterSystem, ElectricGridSystem, EtherGridSystem) | K8.2 + K8.3 | Vanilla.Inventory | M10 |
| `Components/Combat/` + `Components/Shared/Faction.cs` + `Components/Shared/Health.cs` (partial — see §3.3) + `Systems/Combat/` + `Systems/Faction/` | 4 + 2 partial | 7 + 3 = 10 | K8.2 + K8.3 | Vanilla.Combat (+ Vanilla.Core for Health) | M9 |
| `Components/Items/` + `Components/Pawn/` + `Components/Shared/Race.cs` (partial) + `Systems/Pawn/` | 5 + 7 + 1 partial | 10 | K8.2 + K8.3 | Vanilla.Pawn (+ Vanilla.Core for Race) | M8.5-M8.7 |
| `Components/Magic/` + `Systems/Magic/` | 4 | 5 (+ helpers) | K8.2 + K8.3 (+ K9 if available) | Vanilla.Magic | M10.B |
| `Components/Shared/Position.cs` (partial — Position is universal) | 1 partial | 0 (no system specific to Position) | K8.2 only | Vanilla.Core (shared) | Incremental in earliest M-milestone needing it |
| `Components/World/` + `Systems/World/` | 3 (+ TerrainKind) | 3 | K8.2 + K8.3 | Vanilla.World | M8.4 |

Total mapping: 31 components + 34 systems → 6 regular vanilla mods + 1 shared (Core). Every component and system has exactly one destination, except the 4 Shared/ components which split between Vanilla.Core (Health, Position, Race) and Vanilla.Combat (Faction, the only Shared/ component that's domain-specific to one mod).

---

## Section 5 — Phased delete strategy

The principle: **never delete legacy code before its replacement is verified working**. Foundation never loses functionality between commits.

### 5.1 — Per-slice atomic delete

When M-milestone X closes (e.g., M8.4 closes Vanilla.World migration):

1. **Test-pass verification**: all tests for the slice pass under the new vanilla mod-loaded shape.
2. **End-to-end load verification**: the simulation boots with the new mod registered, the slice's components/systems function as before.
3. **Atomic commit (delete-after-verify)**: in the same milestone's atomic commit set, the now-orphaned `src/DualFrontier.Components/<Slice>/` and `src/DualFrontier.Systems/<Slice>/` directories are deleted.
4. **No `src/Components/<Slice>/` reference remains anywhere in the codebase** (verified by grep).

The delete is part of the M-milestone's commit shape, not a follow-up. M-milestone closure means the slice is **fully relocated**: present in `mods/Vanilla.<Slice>/`, absent from `src/`.

### 5.2 — Why this works

The two-step "migrate then delete" preserves three invariants:

- **Buildable at any commit**: at no commit between M-milestone start and close is the codebase in an unbuildable state. The migration commits land the new code first, run tests, then delete the old.
- **Revertible**: if M-milestone needs to revert, the relocation commits and delete commits revert as a unit. The legacy `src/<Slice>/` returns; the kernel still builds.
- **No orphan references**: by the close of each M-milestone, every reference to the moved code points to the new location. There's no period where a system in `src/Pawn/` reads a component that already moved to `Vanilla.Pawn/`.

### 5.3 — What about the `DualFrontier.Components.csproj` and `DualFrontier.Systems.csproj` themselves

After **all** slices migrate (post-M10.B):

- `src/DualFrontier.Components/DualFrontier.Components.csproj` — empty project, 0 source files. Delete the csproj and its directory.
- `src/DualFrontier.Systems/DualFrontier.Systems.csproj` — same.
- Solution file (`DualFrontier.sln`) — remove the entries.
- Any project that referenced `DualFrontier.Components` or `DualFrontier.Systems` — remove the reference (these projects already moved their content; references were updated in the per-slice migrations).

This terminal cleanup is an "M10.Cleanup" mini-milestone — small, mechanical, post-M10.B.

### 5.4 — What survives in `src/`

After M10.Cleanup:

- `src/DualFrontier.Core/` — managed wrappers (preserved per K-L11 World-as-test-fixture)
- `src/DualFrontier.Contracts/` — IComponent, EntityId, IModApi, etc. (kernel surface)
- `src/DualFrontier.Core.Interop/` — managed bridge layer
- `src/DualFrontier.Application/` — bootstrap, scheduler, mod loader
- `src/DualFrontier.Modding/` — mod loader implementation
- `src/DualFrontier.Persistence/` — codec library (untouched throughout K+M migration; addressed in future save-system milestone)
- `src/DualFrontier.Events/` — event types (might also see partial migration to vanilla mods if any events are slice-specific; brief decides)
- `src/DualFrontier.Runtime/`, `src/DualFrontier.Presentation/` — Vulkan-track, separate (per RUNTIME_ARCHITECTURE.md)

The `src/` directory shrinks by ~31+34 = 65 production files plus per-slice directories and per-slice tests. The codebase center of gravity moves to `mods/Vanilla.*/` — exactly what K-L9 «vanilla = mods» intends.

---

## Section 6 — Document maintenance requirements

This plan requires amendments to authoritative documents. Each amendment is a documentation milestone that this plan explicitly schedules.

### 6.1 — `KERNEL_ARCHITECTURE.md` Part 2 master plan amendment (during K8.2)

**Trigger**: K8.2 brief authoring time, or K8.2 closure time.
**Change**: K8.2 row reformulated from "7 class components" to "all 31 production components in `src/DualFrontier.Components/`". K8.3 row reformulated from "12 vanilla systems" to "all 34 production systems in `src/DualFrontier.Systems/`". K8.4 and K8.5 rows refined per §1.3 / §1.4 of this plan.
**Version bump**: KERNEL_ARCHITECTURE.md v1.3 → v1.4.
**Authority**: this migration plan is the change rationale; K8.2 brief includes the amendment as a phase.

### 6.2 — `MOD_OS_ARCHITECTURE.md` §11 consistency check (during K8.5)

**Trigger**: K8.5 brief authoring time.
**Change**: §11.1 migration phases table verified consistent with this plan. M8.4 row currently absent from the table (table goes M0...M3.4, M3.5, M4-M10); M8.4 (Vanilla.World) is implied via §11 M-series narrative but not row-level. K8.5 amendment adds M8.4-M10.B rows.
**Version bump**: MOD_OS_ARCHITECTURE.md v1.6 → v1.7 (non-semantic correction per existing v1.x convention).
**Authority**: this migration plan; K8.5 brief includes the amendment.

### 6.3 — `MIGRATION_PROGRESS.md` Phase B preparation (during K8.5)

**Trigger**: K8.5 brief execution.
**Change**: skeleton tracking entries added for M8.4 through M10.B (status NOT STARTED, brief reference TBD, SHA TBD). Live tracker top section updated to indicate Phase B begins after K8.5 closure.
**Authority**: this migration plan; K8.5 brief carries the edit.

### 6.4 — Per-M-milestone updates to `MIGRATION_PROGRESS.md`

**Trigger**: each M-milestone closure.
**Change**: the corresponding M-row updated with status DONE, SHA, date, brief reference, deviations (if any), test count delta (if any). Same shape as K-milestone closure entries (precedent: K8.1, K8.1.1, K-Lessons).
**Authority**: each M-milestone brief authors its own MIGRATION_PROGRESS update.

### 6.5 — Deletion of stale references (during K8.5 and per M-milestone)

**Trigger**: K8.5 brief execution (kernel-side stale refs), each M-milestone brief execution (slice-specific stale refs).
**Change**: any document referencing the old "7 components / 12 systems" wording, or referencing `src/DualFrontier.Components/<Slice>/` after that slice migrates, is updated.
**Authority**: each affected brief; this plan is the global change-rationale anchor.

### 6.6 — K-L3.1 amendment execution (deferred to follow-up brief)

**Trigger**: K-L3.1 amendment plan execution (separate Cloud Code amendment brief, post-K-L3.1 closure 2026-05-10).
**Change**: per-document edits enumerated in `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (this plan's authoring deliverable):
- KERNEL_ARCHITECTURE.md `v1.3 → v1.5` (Part 0 K-L3 row + implication paragraph + Part 4 Decisions log + Part 2 K8.2 row + status line + closing v1.0 sediment)
- MOD_OS_ARCHITECTURE.md `v1.6 → v1.7` (lines 1149–1150 + §3.5 D-1 path orthogonality + §4.6 IModApi v3 + §11.1 M3.5 + §11.2 new ValidationErrorKind)
- MIGRATION_PLAN_KERNEL_TO_VANILLA.md `v1.0 → v1.1` (this document — header + §0.1 Phase A' integration + §0.3 Decision #9 + §1.2/§1.3/§1.5 extensions + §6.6 self-reference)
- MIGRATION_PROGRESS.md content sync (lines 35/407/443/457 «без exception» reframing) + new K-L3.1 closure entry
- Forward-track brief dispositions per addendum §A5.6 (K9 surgical [full authored brief], K8.3 surgical-to-scope [skeleton], K8.4 in-place rewrite [skeleton], K8.5 surgical [skeleton])

**Authority**: K-L3.1 session lock 2026-05-10 (Crystalka + Opus deliberation; Q1–Q6 + synthesis form §4.A locked); Phase A' sequencing companion at `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`.
**Estimated execution**: 30–60 min auto-mode; per-document atomic commit shape; test count delta zero.

---

## Section 7 — Open decisions (need locking before K8.2 brief authoring)

This section enumerates the **minimum** set of decisions that must be locked before K8.2 brief authoring begins. Other decisions are deferred to per-milestone briefs. The list is intentionally short — this plan's strategy lock (Option II) closed the major axis; only tactical decisions remain.

### 7.1 — Q-K8.2-Ordering: component conversion order within K8.2

K8.2 converts 31 components. They have varying complexity:
- Trivial (no K8.1 primitives needed): pure value-only structs (numeric/enum fields). Examples: WeaponComponent, BiomeComponent (likely).
- Single InternedString: only string field. Examples: IdentityComponent, FactionComponent.
- NativeMap-using: Dictionary<K,V> field. Examples: SkillsComponent, StorageComponent.
- Composite-using: List<T> field. Examples: MovementComponent.
- Empty stub: SocialComponent (placeholder lie).

**Question**: K8.2 brief uses **rule-based** ordering (per Crystalka chat session 2026-05-09 Q1: "rule-based"), Cloud Code applies the rule on actual inventory at brief execution time. Default rule (preserved from chat session):

1. Tier 1: pure value-only (no K8.1 primitives) — first.
2. Tier 2: InternedString-only — second.
3. Tier 3: NativeMap or NativeSet — third.
4. Tier 4: Composite or NativeComposite — fourth.
5. Within tier: by domain slice for test fixture isolation.
6. Empty stubs: deferred to last; per-stub decision (delete vs empty-struct conversion).

**Status**: rule provisionally locked per chat session; K8.2 brief author confirms or refines.

### 7.2 — Q-K8.2-SocialComponent: empty stub disposition

**Question**: SocialComponent is `public sealed class SocialComponent : IComponent { // TODO: public Dictionary<EntityId, int> Relations = new(); }`. Per METHODOLOGY §7.1, empty stubs are placeholder lies. Three resolutions:

- **(α) Skip**: leave as class stub through K8.2; K-L3 violated for this one component until social-feature design milestone.
- **(β) Empty struct**: `public struct SocialComponent : IComponent { }` for K-L3 consistency, but content remains a placeholder lie.
- **(γ) Delete**: remove SocialComponent.cs and SocialSystem.cs (orphan dependent) entirely; restored when social-feature actually implementing.

**Recommendation**: **(γ) Delete**. Aligns with Crystalka's explicit Q5/Q6 framing («системы сохранений нет, foundation first; всё что существует это для тестов или TODO»). SocialComponent is the most TODO-shaped of all components.

**Status**: K8.2 brief author confirms Crystalka's call before execution. Default to **(γ)** if no contrary instruction.

### 7.3 — Q-K8.2-Tests: test fixture strategy

**Question**: K8.2 produces ~30-60 new tests. Strategy:

- **(A) Per-component test class**: 31 new test classes, one per component. Highest isolation, longest scope.
- **(B) Feature-axis**: ~3-5 test classes (round-trip semantics, mod-scope reclaim, save/load if applicable). All components covered in each. Lower scope.
- **(C) Hybrid**: per-component + cross-component invariant tests. Highest coverage, longest scope.

**Recommendation per chat session**: **(A) per-component**, optionally with **(C) invariant layer** as K8.2.x follow-up if needed.

**Status**: locked per chat session 2026-05-09 Q3=(A).

### 7.4 — Q-K8.2-PersistenceLayer: codec library treatment

**Question**: `src/DualFrontier.Persistence/Compression/{ComponentEncoder,EntityEncoder,StringPool,TileEncoder}.cs` reference specific component types (TileComponent, etc.). When K8.2 converts those components, codec layer references update mechanically (struct vs class is mostly transparent at codec level). Question: do codec fixes land in K8.2 or as separate milestone?

**Recommendation**: codec fixes are part of K8.2 atomic commits (same milestone, same per-component commit). They're not architectural change, they're consequence of struct conversion.

**Status**: locked by this plan. K8.2 brief includes codec consequence updates as part of per-component conversion.

### 7.5 — Q-K-Lessons-CrossRef: K-Lessons cross-reference style in K8.2 brief

**Question**: K-Lessons formalized 4 lessons in METHODOLOGY v1.5 (atomic commit, Phase 0.4 inventory, mod-scope test isolation) and KERNEL v1.3 (error semantics convention). K8.2 brief can either:

- **(A) Cross-reference by section heading**: stable refs to e.g. «Pipeline closure lessons (K-series, post-K8.1) → Atomic commit as compilable unit».
- **(B) Inline content**: duplicate lesson content into brief.

**Recommendation per chat session**: **(A) cross-reference**. K-Lessons closure verified `9df2709..071ae11`; section headings stable.

**Status**: locked per chat session 2026-05-09 Q4=(A).

### 7.6 — That's the complete open-decision list

No other decisions need locking before K8.2 brief authoring. The strategy is locked, the inventory is verified, the per-component K8.1 primitive mapping is mechanical, the M-series sequence is locked, the document amendment plan is locked.

---

## Section 8 — Out of scope (explicit)

To prevent scope creep:

### 8.1 — Save system implementation

Per Crystalka 2026-05-09 («системы сохранений нет, foundation first; будет внедряться от фундамента, не наоборот»):

- `src/DualFrontier.Application/Save/SaveSystem.cs` `throw new NotImplementedException` is **deliberate** state.
- `src/DualFrontier.Persistence/` codec library is **byte-level codecs**, not a save system. Functional, untouched throughout K+M migration.
- Save system implementation is a **future milestone** (post-K9, post-G-series). It will design against the fully-completed kernel + runtime + mod foundation, not against the migration-in-progress moving target.
- K8.2 codec consequences (per §7.4) are in scope; save system itself is not.

### 8.2 — Game mechanics design

The 31 components and 34 systems are **structural** code (the test polygon that validated K-series). Real game mechanics (gameplay design — what hunger does, how combat resolves stat checks, magic balance, etc.) are **future** post-foundation work.

Empty stubs (SocialComponent, anything similar) are placeholder lies per METHODOLOGY §7.1. They are not "game mechanics designed but not implemented yet" — they are scaffolding that should not exist until actual design.

When game mechanics design begins (post-M10.B, post-G-series), it authors **new** components and systems in vanilla mod assemblies (or third-party mods), shaped against the now-mature foundation. It does not "fill in" the placeholders; placeholders are deleted, real designs replace them.

### 8.3 — RUNTIME_ARCHITECTURE.md M9.x runtime track

Vulkan rendering layer (M9.0-M9.8 per RUNTIME_ARCHITECTURE.md) is a **separate track** from this plan's mod-OS migration. The naming overlap (RUNTIME M9 vs MOD-OS M9) is unfortunate but historical.

This plan's M9 (Vanilla.Combat migration) is **not** RUNTIME's M9.x (Vulkan device/swapchain/pipeline work). Sequence parents differ: this plan is β6 mod-OS sequencing; RUNTIME M9.x is β6+G-sequential per KERNEL_ARCHITECTURE Part 8.

K8.5 amendment (per §6.2) clarifies the namespace separation in MOD_OS_ARCHITECTURE.md and RUNTIME_ARCHITECTURE.md if needed.

### 8.4 — GPU compute (G-series) and K9 (RawTileField)

Per current MIGRATION_PROGRESS.md state, K9 is the **next** kernel milestone after K8.1.1/K-Lessons (Option c sequencing per K8.0). This means K9 may close **before** K8.2 begins — Option c places K9 between K8.1 and K8.2.

**Implication for this plan**: K9 is **kernel-side** work that does not interact with M-series. It produces RawTileField primitives but does not migrate any production component or system. K8.2-K8.5 sequence in this plan is unaffected by K9 placement (Option c puts K9 before K8.2; this plan's Phase A still K8.2-K8.5 sequential after K9).

**K9 status at K8.2 brief authoring time**: likely DONE per Option c sequencing. This plan does not require K9 closure as Phase A prerequisite — only K8.1.1 + K-Lessons (both DONE per chat session 2026-05-09).

G-series (G0-G9 GPU compute) is post-Phase-A, post-Phase-B work. Not addressed by this plan.

### 8.5 — Persistence codec layer architectural redesign

Per §7.4, codec layer **consequences** of K8.2 (file path updates, struct vs class consequence) are in K8.2 scope. **Architectural redesign** of the codec layer (e.g., to use K8.1 InternedString primitives natively, or to restructure ComponentEncoder for new component shapes) is **not** in K8.2 scope. That redesign happens when save system implementation begins.

---

## Closing — Plan summary

**Strategy**: Option (II) "Struct-first sequential" — K-track closes before M-track mass migration.

**Phase A** (4-8 weeks at hobby pace): K8.2 (31 components class→struct in `src/`) → K8.3 (34 systems → SpanLease/WriteBatch in `src/`) → K8.4 (managed World retire, Mod API v3) → K8.5 (Phase A→B handoff: docs + capability annotations + verification).

**Phase B** (5-10 weeks at hobby pace): M8.4 (Vanilla.World) → M8.5-M8.7 (Vanilla.Pawn 3 sub-milestones) → M9 (Vanilla.Combat + Faction) → M10 (Vanilla.Inventory + Power) → M10.B (Vanilla.Magic) → M10.Core (incremental, concurrent throughout) → M10.Cleanup (terminal `src/` directory cleanup).

**Phase C** (per existing roadmap): K9 if not yet closed (Option c places K9 between K8.1 and K8.2, so likely DONE before Phase A begins) → G-series GPU compute → save system implementation.

**Total Phase A+B duration**: 9-18 weeks at hobby pace.

**LOC impact**: Phase A ≈ -3000/+1500 net; Phase B ≈ neutral (file movements, not code rewrites). Aggregate ≈ -1500 / +1500 net (no compromise on architectural cleanness).

**Test count trajectory**: 592 (Phase A start) → unknown intermediate (placeholder-lie removal balanced against new K8.2/K8.3 tests) → stable Phase B baseline (file relocation preserves tests).

**Document amendments scheduled**: KERNEL_ARCHITECTURE.md v1.3→v1.4 (K8.2 era), MOD_OS_ARCHITECTURE.md v1.6→v1.7 (K8.5 era), MIGRATION_PROGRESS.md ongoing (per-milestone).

**Open decisions remaining**: 5 (per §7), all tactical, all resolvable within K8.2 brief authoring.

---

**Document end. This plan is the architectural reference for all K8.2-K8.5 + M8.4-M10.B brief authoring. Amendments require chat-session deliberation and explicit commit; «stop, escalate, lock» rule applies (METHODOLOGY §3).**
