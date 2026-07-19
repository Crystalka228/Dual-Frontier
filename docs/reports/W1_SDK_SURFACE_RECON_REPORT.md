---
register_id: DOC-E-W1_SDK_SURFACE_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-19'
last_modified: '2026-07-19'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'W1 SDK-SURFACE RECON REPORT — 2026-07-19 (R1–R7) — SDK usage-closure measurement at HEAD df1541d for BD-1/BD-2 chartering: 28 production SystemBase systems (10 REAL / 18 [BridgeImplementation] STUB), the 7-member SystemBase surface, the ~8-of-40 NativeWorld consumed subset (4 interop-leaking), 5 buses (only Pawns+Inventory live), construction bifurcation (27 parameterless + MovementSystem(IPathfindingService)), attribute census 28/28/0, zero field/compute usage, mods hollow, fixture→Core leak = SystemBase-naming; decision-inputs D1–D9 + anomalies A1–A10; extends DOC-E-GAME_ENGINE_BOUNDARY_AUDIT_REPORT, no kickoff↔law conflict'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-GAME_ENGINE_BOUNDARY_AUDIT_REPORT, DOC-E-EQ_A_SHUTDOWN_RECON_REPORT, DOC-E-F29_NATIVE_SCHEDULER_RECON_REPORT). Wave-1 pre-deliberation SDK usage-closure measurement — the empirical basis for the BD-1 (ISystemContext) + BD-2 (construction/DI) chartering deliberation. Produced read-only at HEAD df1541d (post-EQ_A4_RENDER_TAIL). UNTRACKED at authoring — enrolled at the next cascade C1.'
---

# W1 SDK-SURFACE RECON REPORT — 2026-07-19

SDK usage-closure measurement for Wave 1 of the `VANILLA_SEPARATION_MIGRATION_PLAN`. Read-only measurement session: **one report, zero repository mutations**. This document produces facts, anchors, and counts — **not designs, not API sketches, not recommendations**. Design belongs to the BD-1/BD-2 chartering deliberation that consumes this report.

**Mission.** Wave 1 unlocks the SDK by settling two ratification-grade decisions: **BD-1** — the Contracts-level system contract (`ISimulationSystem` + capability-scoped `ISystemContext`) that replaces `SystemBase`-inheritance, needed because `SystemBase` cannot relocate to `Contracts` (the `Contracts → Core.Interop → Contracts` cycle, audit anomaly A4); **BD-2** — the construction/DI model that replaces parameterless `Activator.CreateInstance`. Both must rest on *what systems actually consume today*. The minimal honest `ISystemContext` surface is whatever the measured usage closure says it is — classified by **capability**, not call-site fidelity. Per the scaffolding ruling (`VANILLA_SEPARATION_MIGRATION_PLAN.md` §1.1): the systems are a **sacrificial test harness**; the goal is to learn what capability classes a real system needs, not to port them faithfully.

**Relation to the boundary audit (extends, does not re-derive).** `DOC-E-GAME_ENGINE_BOUNDARY_AUDIT_REPORT` (HEAD `4c58942`) measured *where the game↔engine edges are* — the compile graph, type inventory, and coupling-claims C-1..C-10. Its C-1 (a mod system needs `SystemBase`, unnameable from Contracts), C-2 (`SystemBase` exposes `NativeWorld`+`IGameServices`), C-3 (parameterless `Activator.CreateInstance`) and C-4 (five genre buses baked into Contracts) are the *structural frame*. This recon measures **one level deeper**: the per-system usage closure that turns those structural facts into the empirical shape of the SDK surface. Where this report restates an audit fact it is to build on it; every new figure is re-measured at the current HEAD.

**Law in force (cited, not restated).** `GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md` v1.0.1 LOCKED (B-3 SDK sufficiency; L2 Contracts SDK) · `VANILLA_SEPARATION_MIGRATION_PLAN.md` v1.1.0 Live (BD-1/BD-2 rows, W1) · `ECS.md` v1.0.1 LOCKED (§7 `SystemBase` role) · `CONTRACTS.md` v1.0.1 LOCKED (§2 five-bus canon, §4 evolution, §6 enforcement reality) · `MOD_OS_ARCHITECTURE.md` (§4.1 registration, §4.3 Fields/Compute nullability, §3.7 cross-checks, §6 bridge replacement) · `EXECUTION_AUTHORITY_MATRIX.md` (R1 storage surface; `SystemBase`/`IGameServices` as sanctioned consumable surfaces).

---

## R1 — Base state

- **Branch / HEAD:** `main` @ `df1541dd687c11e74607569e25d9d21a5c9102e7` (`df1541d`) — commit `governance(closure): EQ_A4_RENDER_TAIL closure`. **Dispatch precondition satisfied** (`main` = `df1541d`).
- **`git status --porcelain`:** clean at session start. At session end the only change is this untracked report file (§Attestation).
- **Provenance vs the audit.** `df1541d` is a descendant of the audit's `4c58942` (`git merge-base --is-ancestor` → true; `git rev-list --count 4c58942..df1541d` → **27** commits: BOUNDARY_W0, EQ_A2 shutdown, EQ_A3 checked-destroy, EQ_A4 render). Audit anchors are re-measured here, not trusted.
- **Derived registers (read directly; `sync` NOT run):**
  - `docs/governance/REGISTER.yaml` → **361 documents** (`rg --count-matches '^- register_id:' docs/governance/REGISTER.yaml` → `361`).
  - `docs/governance/AUDIT_TRAIL.yaml` → **62 EVTs** (`rg --count-matches '^- id: EVT-'` → `62`).
  - (The audit recorded 349/58 at `4c58942`; the +12 docs / +4 EVTs are the EQ_A2/A3/A4 cascade enrollments.)
- **ROADMAP queue.** The row `| W1 | SDK surface unlock | BD-1, BD-2 (BD-6 test authored) | … | OPEN |` already exists (`docs/ROADMAP.md:1000`) under the Vanilla-separation track (`:993`). This recon is the pre-deliberation measurement feeding that row's chartering.
- **HEAD-drift check (BD-1/BD-2 crux files).** Re-read first-hand at `df1541d`, **zero drift** on the load-bearing surfaces: `SystemBase.cs` (7-member surface unchanged), `IGameServices.cs` (five buses unchanged), `ModRegistry.cs` (the `Activator.CreateInstance` gate unchanged). Non-crux drift worth recording: the composition root evolved `GameBootstrap.CreateLoop` → **`CreateSession` returning `EngineSession`** (EQ_A2 shutdown transaction); `NativeWorld` gained `DisposeChecked`/`ActiveSpanCount` (EQ_A3). Neither touches the SDK-consumption surface.

**Framing the report carries.** `GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md` B-3 states the target law directly: *"Everything vanilla needs must arrive through the same SDK surface available to any third-party mod. A capability vanilla requires that the SDK cannot express is an SDK gap to fix."* This recon measures exactly that surface — the usage closure of the current systems — so BD-1's `ISystemContext` and BD-2's construction model are bounded by data, not guesses. `EXECUTION_AUTHORITY_MATRIX.md` names `SystemBase`/`IGameServices` as the *current* sanctioned consumable surfaces; BD-1/BD-2 replace them. **The distance measured is current→target; it is not a corpus violation** (self-check §R7).

---

## R2 — T1: SystemBase consumer census (the core measurement)

### The `SystemBase` consumable surface — exactly 7 members (`src/DualFrontier.Core/ECS/SystemBase.cs`)

These are the capability columns; there is nothing else a subclass can reach on the base:

| # | Member | Line | Kind | Purpose |
|---|---|---|---|---|
| 1 | `Context` | `:29` | `public SystemExecutionContext { get; internal set; }` | **Reserved** — the scheduler does NOT assign it; systems reach the live context via `SystemExecutionContext.Current` (the `Services`/`NativeWorld` accessors do). |
| 2 | `OnInitialize()` | `:35` | `protected virtual void` | one-time setup / bus subscriptions |
| 3 | `Update(float delta)` | `:41` | `public abstract void` | the tick entry — **the only temporal input a system receives** |
| 4 | `OnDispose()` | `:48` | `protected virtual void` | cleanup / unsubscribe |
| 5 | `Services` | `:70` | `protected IGameServices` | domain-bus aggregator (publish/subscribe) |
| 6 | `NativeWorld` | `:93` | `protected NativeWorld` | sole production component-storage path |
| 7 | `ManagedStore<T>()` | `:126` | `protected ManagedStore<T>? where T:class,IComponent` | Path-β per-mod managed-class store |

There is **no dedicated deferred-event member**: deferral is a property of the *event type* (`[Deferred]`/`[Immediate]`, `CONTRACTS.md` §1), published through `Services.X`. `Initialize()`/`Dispose()` (`:53`/`:58`) are `internal`, called by the scheduler — not touchable by subclasses.

**The de-facto context object already exists.** `SystemExecutionContext` (`src/DualFrontier.Core/ECS/SystemExecutionContext.cs:33`) is pushed per-system into a `ThreadLocal` slot before each `Update` and holds exactly `{ _nativeWorld, _services, _allowedBuses, _origin, _modId, _faultSink, _managedStorageResolver }` (`:37-48`). `SystemBase.Services`/`.NativeWorld`/`.ManagedStore<T>()` are thin routers to it. It is `internal` to Core and wraps the concrete `NativeWorld`. **This is the empirical seed of BD-1's `ISystemContext`** — a Contracts-level, capability-scoped successor of a structure that already exists engine-internal.

### The census — 28 production `SystemBase` subclasses (all in `DualFrontier.Systems`)

`class \w+ : SystemBase` count-matches in `src` = **38**; the load-bearing figure is the **28 concrete production classes** below (the 38 is inflated by README code examples — the `--count-matches` metric drifts, per the EQ_A3 provenance lesson; `rg -l ': SystemBase' mods` → **0**, unchanged from the audit). Two Magic helper classes (`ManaLease`, `ManaLeaseRegistry`) are `internal`, non-`SystemBase`, excluded.

**`R/S` = REAL (has a live body) / STUB (empty `Update`).** Every ctor is implicit-parameterless **except `MovementSystem`**. `[Bridge]` = `[BridgeImplementation(Phase=N)]`. Per-cell line anchors are in the named file.

| System (file) | R/S | `[SystemAccess]` bus | Tick | `[Bridge]` | Held state | Base surface touched (actual) |
|---|---|---|---|---|---|---|
| `Combat/CombatSystem.cs` | S | Combat,Magic (`:28`) | FAST | P5,Repl (`:31`) | none | OnInit(empty `:38`), Update(TODO `:40`); 1× `[ReservedStub]` throw |
| `Combat/ComboResolutionSystem.cs` | S | Combat,Magic (`:26`) | NORMAL | P5,Repl | none | none; reads/writes hold **event** types; 2× `[ReservedStub]` throw |
| `Combat/CompositeResolutionSystem.cs` | S | Combat,Inv,Magic (`:28`) | FAST | P5,Repl | none | none; event types; 6× `[ReservedStub]` throw |
| `Combat/DamageSystem.cs` | S | Combat (`:21`) | FAST | P5,Repl | none | none |
| `Combat/ProjectileSystem.cs` | S | Combat (`:20`) | REALTIME | P5,Repl | none | none |
| `Combat/StatusEffectSystem.cs` | S | Combat (`:20`) | FAST | P5,Repl | none | none |
| `Faction/RaidSystem.cs` | S | World (`:18`) | RARE | P7 | none | none |
| `Faction/RelationSystem.cs` | S | World (`:19`) | RARE | P7 | none | none |
| `Faction/TradeSystem.cs` | S | World (`:19`) | RARE | P7 | none | none |
| `Inventory/CraftSystem.cs` | S | Inventory (`:20`) | NORMAL | P6 | none | none |
| **`Inventory/HaulSystem.cs`** | **R** | Inventory (`:33`) | NORMAL | — | VALUE (`HashSet` `:38`) | AcquireSpan×3, Publish×3, InternedString.Resolve |
| **`Inventory/InventorySystem.cs`** | **R** | Inventory (`:22`) | FAST | — | VALUE (2 `Dict` `:27-28`) | Subscribe×3, TryGet/InternString/BeginBatch/AcquireSpan |
| `World/MapSystem.cs` | S | World (`:18`) | RARE | P7 | none | none |
| `World/WeatherSystem.cs` | S | World (`:19`) | RARE | P7 | none | none |
| `Magic/EtherGrowthSystem.cs` | S | Magic (`:19`) | SLOW | P6 | none | none |
| `Magic/GolemSystem.cs` | S | Magic (`:24`) | NORMAL | P6 | none | none; 2× `[ReservedStub]` throw |
| `Magic/ManaSystem.cs` | S | Magic (`:25`) | NORMAL | P6 | VALUE (dormant `_registry` `:33`) | none; event type in reads (`:23`); 5× `[ReservedStub]` throw |
| `Magic/RitualSystem.cs` | S | Magic (`:20`) | RARE | P6 | none | none |
| `Magic/SpellSystem.cs` | S | Magic (`:20`) | FAST | P6 | none | none |
| **`Pawn/ComfortAuraSystem.cs`** | **R** | Pawns (`:43`) | SLOW | — | none | AcquireSpan×2, Has/Get, Publish |
| **`Pawn/ConsumeSystem.cs`** | **R** | Pawns (`:37`) | NORMAL | — | none | AcquireSpan×4, TryGet×N, BeginBatch, Publish×4 |
| **`Pawn/JobSystem.cs`** | **R** | Pawns (`:18`) | NORMAL | — | VALUE (`HashSet` `:23`) | Subscribe×5, AcquireSpan, TryGet, BeginBatch×2 |
| **`Pawn/MoodSystem.cs`** | **R** | Pawns (`:16`) | SLOW | — | none | AcquireSpan, TryGet, BeginBatch, Publish |
| **`Pawn/MovementSystem.cs`** | **R** | Pawns (`:25`) | NORMAL | — | **ENGINE-REF** (`_pathfinding` `:34`) + `Random` `:35` | ctor(`IPathfindingService` `:37`); Subscribe×4, Publish, AcquireSpan, TryGet, **CreateComposite**, BeginBatch×2 |
| **`Pawn/NeedsSystem.cs`** | **R** | Pawns (`:27`) | SLOW | — | VALUE (`Dict` `:37`) | Subscribe, Publish, AcquireSpan, TryGet, BeginBatch×2 |
| **`Pawn/PawnStateReporterSystem.cs`** | **R** | Pawns (`:29`) | SLOW | — | none | AcquireSpan×3, Has/Get×4, Publish, InternedString.Resolve |
| `Pawn/SkillSystem.cs` | S | Pawns (`:18`) | NORMAL | P3 | none | none |
| **`Pawn/SleepSystem.cs`** | **R** | Pawns (`:31`) | NORMAL | — | none | AcquireSpan×3, TryGet×N, BeginBatch, Publish×3 |

**Split: 10 REAL / 18 STUB.** The 10 REAL systems are *exactly* the `GameBootstrap.coreSystems` array (`GameBootstrap.cs:141-153`) **and** *exactly* the 10 systems that touch `NativeWorld` (§R3) — a three-way triangulation. `[BridgeImplementation]` marks **all 18 stubs and no REAL system** (`rg -c '\[BridgeImplementation' src/DualFrontier.Systems` → 18; phases P3×1, P5×6, P6×6, P7×5): a ready-made migration-triage marker.

### Usage profiles — the empirical shape of `ISystemContext`

The distinct capability bundles that actually occur (the profile list *is* the measured shape of the context surface):

- **Profile 0 — Dormant / bridge stub (18).** Touches none of the 7 members at runtime: empty `OnInitialize` + inert `Update`. (Sub-variant: 5 carry `[ReservedStub]` throwing roadmap methods — Combat×2, Composite, Golem, Mana.)
- **Profile 1 — Pure publisher (3): `ComfortAura`, `PawnStateReporter`, `Haul`.** `NativeWorld` read (`AcquireSpan`/`Has`/`Get`) → `Services.X.Publish`. No `BeginBatch`, no `Subscribe`.
- **Profile 2 — Tick writer (3): `Mood`, `Sleep`, `Consume`.** `AcquireSpan` snapshot → `BeginBatch` write of one component → `Publish`. No `Subscribe`.
- **Profile 3 — Subscriber + writer (4): `Needs`, `Job`, `Movement`, `Inventory`.** `Services.X.Subscribe` in a real `OnInitialize`; captured-context handlers write via `BeginBatch`; `Update` also span+batch.

The union of Profiles 1–3 is the entire capability demand of every live system: **`{ span-read, batch-write, per-id Try/Has/Get, InternString+Resolve, CreateComposite, Publish, Subscribe, real OnInitialize }`**.

**Universal negatives across all 28** (each independently confirmed): `Update`'s `delta` parameter is used by **0**; `ManagedStore<T>()` by **0**; `Context` by **0**; `OnDispose()` overridden by **0**; the Field/compute surface by **0** (§R6). `OnInitialize()` is overridden by 11, but only 3 have non-empty bodies (`Needs`, `Job`, `Movement`).

---

## R3 — T2: NativeWorld usage closure

### Full public surface — 40 members (`src/DualFrontier.Core.Interop/NativeWorld.cs`)

`public sealed class NativeWorld : IDisposable` (`:28`). 5 properties + 35 methods (2 public ctors + finalizer excluded). `[LEAK]` = returns/consumes a `Core.Interop` type that cannot cross to `Contracts`; `[val]` = value-semantics.

| Group | Members (line) |
|---|---|
| Properties | `Registry` (`:62`), `Fields`→`FieldRegistry` **[LEAK]** (`:118`), `EntityCount` (`:139`) `[val]`, `StringPoolCount` (`:832`), `StringPoolCurrentGeneration` (`:835`) |
| Entity lifecycle | `CreateEntity`→`EntityId` (`:120`), `DestroyEntity` (`:127`), `IsAlive` (`:133`) `[val]`, `FlushDestroyedEntities` (`:148`) |
| Per-entity component | `AddComponent<T>` (`:154`), `TryGetComponent<T>` (`:166`) `[val]`, `GetComponent<T>` (`:180`) `[val]`, `HasComponent<T>` (`:190`) `[val]`, `RemoveComponent<T>` (`:199`), `GetComponentCount<T>` (`:208`) |
| Bulk component | `AddComponents<T>(ROSpan…)` (`:225`), `GetComponents<T>(…Span)` (`:283`) |
| Span / batch | `AcquireSpan<T>`→`SpanLease<T>` **[LEAK]** (`:345`), `BeginBatch<T>`→`WriteBatch<T>` **[LEAK]** (`:376`) |
| Vulkan / compute | `AttachVulkan` (`:398`), `RegisterComputePipeline` (`:418`), `DispatchFieldCompute` (`:449`), `ComputePipelineCount` (`:477`) |
| df_status / teardown | `ActiveSpanCount` (`:496`), `DisposeChecked`→`WorldTeardownResult` (`:523`), `Dispose` (`:551`) |
| K8.1 interning | `InternString`→`InternedString` **[LEAK]** (`:616`), `ResolveInternedString(InternedString)`→`string?` (`:649`) |
| K8.2 map/set/composite | `GetKeyedMap`**[LEAK]** (`:701`), `GetComposite`**[LEAK]** (`:720`), `GetSet`**[LEAK]** (`:736`), `AllocateMapId/SetId/CompositeId` (`:758/:767/:776`), `CreateMap`**[LEAK]** (`:787`), `CreateSet`**[LEAK]** (`:797`), `CreateComposite`→`NativeComposite<T>`**[LEAK]** (`:805`) |
| Mod scope | `BeginModScope` (`:810`), `EndModScope` (`:817`), `ClearModScope` (`:824`) |

### The system-consumed subset — 8 of 40

Systems call **only** these NativeWorld members (`rg 'NativeWorld\.\w+' src/DualFrontier.Systems` + the per-system profiles cross-check):

| Member | Return | Boundary | System callers |
|---|---|---|---|
| `AcquireSpan<T>` | `SpanLease<T>` | **[LEAK]** | all 10 REAL |
| `BeginBatch<T>` | `WriteBatch<T>` | **[LEAK]** | Needs, Mood, Job, Movement, Consume, Sleep, Inventory |
| `TryGetComponent<T>` | `bool` + `out T` | `[val]` | Needs, Job, Mood, Movement, Consume, Sleep, Inventory |
| `HasComponent<T>` | `bool` | `[val]` | ComfortAura, PawnStateReporter |
| `GetComponent<T>` | `T` | `[val]` | ComfortAura, PawnStateReporter |
| `InternString` | `InternedString` | **[LEAK]** | Inventory |
| `CreateComposite<T>` | `NativeComposite<T>` | **[LEAK]** | **MovementSystem only** (`MovementSystem.cs:157`, `move.Path` storage) |
| `ResolveInternedString` | `string?` | indirect | Haul, PawnStateReporter (via `InternedString.Resolve(NativeWorld)`) |

**The structural finding (BD-1 method-level crux).** Of the 8 members systems touch, **4 leak `Core.Interop` types** (`SpanLease<T>`, `WriteBatch<T>`, `InternedString`, `NativeComposite<T>`) and 3 are value-safe (`Try/Has/Get`). So even the *minimal* world-view systems demand cannot be re-exposed by handing out `NativeWorld` — the span/batch/intern/composite primitives are the entangled residue. `ISystemContext` needs Contracts-level abstractions over these four, not a relocation (consistent with audit A4: `Contracts → Core.Interop → Contracts` cycle blocks relocation; this report locates the cycle at the *method* level).

### Engine-internal-only members and dead surface

- **The other 32 members are engine-internal only.** Callers (`rg` per member, classified by file): `RandomPawnFactory` (`Scenario/RandomPawnFactory.cs`, 13 NativeWorld calls — `CreateEntity`/`AddComponents` bulk path), `ItemFactory`, `GameBootstrap`, `EngineSession`, `Bootstrap`; the Vulkan/compute quartet is Runtime/render-side; `DisposeChecked`/`ActiveSpanCount` are `EngineSession` S7 shutdown (EQ_A3).
- **`DestroyEntity` / `FlushDestroyedEntities`: zero production callers** at `df1541d` (`rg -n 'DestroyEntity|FlushDestroyedEntities' src` → only the two definitions + doc-comments in `WriteBatch.cs`/`EntityId.cs`/`Components/Items/README.md`). Confirms `ECS.md` §5 "production is creation-only" still holds — the SDK world-view need not expose destruction on day one.
- **`Fields` (the FieldRegistry property): zero system callers** (§R6).

---

## R4 — T3: Services / bus / event consumption

### Five buses, two live (`src/DualFrontier.Contracts/Bus/IGameServices.cs:13-49`)

`IGameServices` aggregates `Combat` (`:20`), `Inventory` (`:27`), `Magic` (`:34`), `Pawns` (`:41`), `World` (`:48`), each a marker `: IEventBus`. The declared bus per system, and its *actual* traffic:

| Bus | Systems declaring it | Systems with ACTUAL traffic |
|---|---|---|
| Combat | CombatSystem, Combo, Composite, Damage, Projectile, StatusEffect (6) | **0** (all stubs; one engine-side subscriber: `GameBootstrap`→`DeathEvent`) |
| Inventory | Craft, **Haul**, **Inventory** (3) | 2 (Haul publishes, Inventory subscribes) |
| Magic | Ether, Golem, Mana, Ritual, Spell (5) | **0** (all stubs) |
| Pawns | all 9 Pawn systems | 8 (all but SkillSystem) |
| World | Raid, Relation, Trade (Faction), Map, Weather (5) | **0** (all stubs) |

**Only the Pawns bus (plus Inventory) carries live system traffic.** The Combat / Magic / World buses have **zero live system producers or consumers** — every system declaring them is a stub. Note also that the three **Faction** systems declare the **`World`** bus (there is no Faction bus) — system domain ≠ genre bus.

### The actual event surface (publish / subscribe)

Every publication/subscription routes through `Services.<Bus>.Publish<T>` / `.Subscribe<T>`; **no system uses `DomainEventBus` (the Core bus implementation) directly.** All events are default-synchronous within-phase; none carries `[Immediate]`, and `[Deferred]` is declared on the event *type*, not at the call site (`CONTRACTS.md` §1).

| System | Bus | Publishes | Subscribes |
|---|---|---|---|
| NeedsSystem | Pawns | `NeedsCriticalEvent` (`:125`) | `NeedsRestoredEvent` (`:46`) |
| MoodSystem | Pawns | `MoodBreakEvent` (`:70`) | — |
| ComfortAuraSystem | Pawns | `NeedsRestoredEvent` (`:76`) | — |
| ConsumeSystem | Pawns | `PawnConsumeFinishedEvent` (`:73`), `PawnConsumeTargetEvent` (`:84`), `NeedsRestoredEvent` (`:193,:207`) | — |
| SleepSystem | Pawns | `NeedsRestoredEvent` (`:165`), `PawnSleepFinishedEvent` (`:167`), `PawnSleepTargetEvent` (`:169`) | — |
| MovementSystem | Pawns | `PawnMovedEvent` (`:202`) | `PawnConsume{Target,Finished}Event`, `PawnSleep{Target,Finished}Event` (`:42-46`) |
| JobSystem | Pawns | — | `NeedsCriticalEvent`, `PawnConsume{Target,Finished}`, `PawnSleep{Target,Finished}` (`:27-38`) |
| PawnStateReporterSystem | Pawns | `PawnStateChangedEvent` (`:79`) | — |
| HaulSystem | Inventory | `ItemReservedEvent` (`:61`), `ItemRemovedEvent` (`:69`), `ItemAddedEvent` (`:76`) | — |
| InventorySystem | Inventory | — | `ItemAddedEvent`, `ItemRemovedEvent`, `ItemReservedEvent` (`:36-38`) |

Engine-side (not a system): `GameBootstrap.CreateSession` subscribes to `PawnSpawned`/`ItemSpawned`/`PawnMoved`/`PawnStateChanged` (Pawns) and `DeathEvent` (Combat) to feed the render bridge (`GameBootstrap.cs:92-103`).

### Declared-vs-actual `[SystemAccess]` (matters for both directions)

- **Declared-but-unused, even in REAL systems:** `JobSystem` declares reads `{Needs, Skills, Position}` but reads only `Needs` (Skills, Position unused). `HaulSystem` declares reads `{Storage, Position, Job}` but never reads `Position`. All 18 stubs: their entire declared reads/writes/bus is aspirational (0 actual usage).
- **Used-but-undeclared:** `InventorySystem` declares `reads: []` yet reads `StorageComponent` (its declared *write*-target, so read-under-write-ownership); `MovementSystem` declares `reads: []` yet reads `PositionComponent` (also its write-target). No foreign undeclared read exists.
- **Unenforced.** `CONTRACTS.md` §6 confirms the shipped A'.9 analyzer (17 rules) contains **no `[SystemAccess]` call-site scoping check** — the declaration is consumed only for dependency-graph edge-building, so declared/actual drift is not caught. `ECS.md` §4 states the per-access runtime guard was deleted at K8.3+K8.4.

### `nameof(IGameServices.X)` binding + resolution sites

- **Binding (production):** the 28 system `[SystemAccess(bus:/buses:)]` declarations (enumerated in R2). Plus test fixtures (`ContractValidatorTests`, `ModIntegrationPipelineTests`, `M72UnloadChainTests`, `GoodMod`, `Fixture.RegularMod_ReplacesCombat`).
- **Resolution / consumption:** `ModBusRouter.Resolve` reflects `typeof(IGameServices).GetProperties()` (`ModBusRouter.cs:42`) and matches an event's `[EventBus].BusName` against the property name (`:35`) — the genre list baked into a reflection lookup; `DependencyGraph` / `SystemGraphInterop` build edges from `[SystemAccess]`; the registration gate `ModRegistry.cs:123` requires it. (The string `nameof(IGameServices.X)` also appears in error-hint literals at `ModRegistry.cs:123` and `DependencyGraph.cs:58` — not bindings.)

---

## R5 — T4: Construction reality (BD-2 input)

### Constructor signatures — 27 parameterless, 1 injected

- **27 of 28 systems declare no constructor** (implicit public parameterless) — verified across both fan-out passes.
- **The sole exception:** `MovementSystem` (`src/DualFrontier.Systems/Pawn/MovementSystem.cs:37`):
  ```csharp
  public MovementSystem(IPathfindingService pathfinding)
      => _pathfinding = pathfinding ?? throw new ArgumentNullException(nameof(pathfinding));
  ```
  It stores `_pathfinding` (`:34`, the **only ENGINE-REF field in the entire set**) and a seeded `Random(42)` (`:35`).

### The bifurcated construction model — the BD-2 gap, measured

- **Core path (can inject):** `GameBootstrap.CreateSession` hand-instantiates the 10 live systems with `new` (`GameBootstrap.cs:141-153`), passing the dependency directly: `new MovementSystem(pathfinding)` (`:149`), where `pathfinding = new AStarPathfinding(navGrid)` (`:113`). The other 9 use parameterless `new`.
- **Mod path (cannot inject):** `ModRegistry.CreateSystemInstance` (`:283-302`) calls `Activator.CreateInstance(systemType)` (`:287`); a `MissingMethodException` is rethrown as *"requires a public parameterless constructor"* (`:297-300`). A mod system therefore **cannot** take `IPathfindingService` (or any dependency) through its constructor.
- **Consequence:** `MovementSystem` — a *core* system today — could not be authored as a *mod* system under the current SDK. It is the living proof of the BD-2 gap: core construction already uses constructor injection that the mod path forbids. (`ECS.md` §8 separately flags "caching a `NativeWorld` reference in system state / a constructor parameter stored in a field" as an anti-pattern; `MovementSystem._pathfinding` is the sole instance, and it is a service, not the world.)
- **`Activator.CreateInstance` sites in `src` = 2:** `ModRegistry.cs:287` (systems) and `ModLoader.cs:96` (`IMod` entry types) — both parameterless. (`ModRegistry.cs:292` is an error-message literal, not a call.) Other instantiation paths: the `GameBootstrap` array (10 systems), and tests.

### Held-state census (stateless-worker vs stateful-object)

| Class | Systems |
|---|---|
| **ENGINE-REF** (holds a reference to an engine object across ticks) | 1 — `MovementSystem` (`_pathfinding : IPathfindingService`) |
| **VALUE-STATE** (primitives / structs / collections of value data) | 6 — `Haul` (`HashSet`), `Inventory` (2× `Dict` + dirty flag), `Needs` (`Dict`), `Job` (`HashSet`), `Movement` (`Random`), `Mana` (dormant `ManaLeaseRegistry`, never used) |
| **NONE** | ~21 (all 18 stubs bar Mana + `ComfortAura`, `Mood`, `Sleep`, `PawnStateReporter`, `Consume`) |

**27 of 28 systems hold no engine reference across ticks** → a context-per-tick model is empirically viable; `MovementSystem`'s single held service is the one construction dependency the DI decision must accommodate.

---

## R6 — T5: Attribute, field/compute, and mod surface

### Attribute census (production systems)

| Attribute | Count | Where the type lives |
|---|---|---|
| `[SystemAccess]` | **28** (1/system) | `Contracts/Attributes/SystemAccessAttribute.cs` |
| `[TickRate]` | **28** (1/system) | `Contracts/Attributes/TickRateAttribute.cs` (values: REALTIME×1, FAST×6, NORMAL×10, SLOW×5, RARE×6) |
| `[ModCapabilities]` | **0** | `Contracts/Attributes/ModCapabilitiesAttribute.cs` — used by no system |
| `[BridgeImplementation]` | **18** | `Contracts/Attributes/BridgeImplementationAttribute.cs` (P3×1, P5×6 `Replaceable=true`, P6×6, P7×5) — marks all 18 stubs |
| `[ReservedStub]` (methods) | 16 on 5 systems | `Contracts/Analyzer/ReservedStubAttribute.cs` (+6 on the internal `ManaLease`/`ManaLeaseRegistry` helpers) |

### Field / compute surface — zero system usage

`rg '\.Fields|IModFieldApi|RegisterField|GetField|ComputePipeline|RestrictedFieldApi' src/DualFrontier.Systems` → **no matches**. The entire K9-field / Vulkan-compute capability class — present on `NativeWorld` (`Fields`, `AttachVulkan`, `RegisterComputePipeline`, `DispatchFieldCompute`) and on the mod API (`RestrictedFieldApi : IModFieldApi`, `IModApi.Fields`/`ComputePipelines`, production-null per `MOD_OS_ARCHITECTURE.md` §4.3) — is **empirically absent from system consumption**. **The day-one `ISystemContext` needs no field or compute surface.**

### Mod inventory — all hollow

- `ExampleMod` (`mods/DualFrontier.Mod.Example/ExampleMod.cs`): `Initialize` is TODO-only (`api.RegisterComponent<…>`, `api.Subscribe<…>` commented); imports `DualFrontier.Contracts.Modding` only. Registers **nothing**.
- `CombatMod`/`Magic`/`Inventory`/`Pawn`/`World` (`mods/DualFrontier.Mod.Vanilla.*`): empty `Initialize`/`Unload`, "content lands in M9"; Contracts-only. `Vanilla.Core` has **no `.cs`** (shared type-vendor, empty entryType). `rg -l ': SystemBase' mods` → **0**.

### The `Fixture.RegularMod_ReplacesCombat` → Core leak — exact mechanism

This is the **only place in the repo a real mod system is authored**, and the exact BD-1 datum:

- `ReplacesCombatMod.cs:20` — `api.RegisterSystem<ReplacementCombatSystem>();` — the **only live `IModApi.RegisterSystem<T>()` call in the tree**. The mod class imports `DualFrontier.Contracts.Modding` only (Contracts-clean).
- `ReplacementCombatSystem.cs:27` — `public sealed class ReplacementCombatSystem : SystemBase` — forces `using DualFrontier.Core.ECS;` (`:4`), and `SystemBase` lives in `DualFrontier.Core`.
- `Fixture.RegularMod_ReplacesCombat.csproj:13` — `<ProjectReference Include="..\..\src\DualFrontier.Core\…" />`. **The Core reference exists solely to name `SystemBase`.** The system itself is minimal (`[SystemAccess(reads:[], writes:[], bus:Combat)]`, `[TickRate(NORMAL)]`, empty `Update`) — it tests registration/skip mechanics, not gameplay. This is the empirical proof of the BD-1 gap: a Contracts-only mod cannot author a system without Core, because the base type is Core-resident.

---

## R7 — T6: Scale, decision inputs, anomalies

### Scale (per BD-1/BD-2 consequence area, grep-counted at `df1541d`)

| Quantity | Value |
|---|---|
| Production `SystemBase` systems | **28** concrete (of which **10 wired in production**; 18 compiled-but-unwired) |
| `[SystemAccess]` / `[TickRate]` sites | 28 / 28 |
| `[BridgeImplementation]` stub markers | 18 |
| `NativeWorld` members consumed by systems | 8 of 40 (4 interop-leaking) |
| Genre buses (declared / live) | 5 / 2 (Pawns + Inventory) |
| Constructor-injected systems | 1 (`MovementSystem`) |
| `Activator.CreateInstance` system sites | 1 (`ModRegistry.cs:287`; +`ModLoader.cs:96` for `IMod`) |
| Real `IModApi.RegisterSystem<T>()` callers | 1 (a test fixture, leaking Core) |
| `nameof(IGameServices.X)` production bindings | 28 (+ test fixtures) |
| Test-fixture `SystemBase` subclasses | 14 files / 75 count-matches (dominated by `TickSchedulerThreadSafetyTests.cs`=32; test scaffolding, not production) |

### Decision-input list (measured context only, NO leans)

- **D1 — `ISystemContext` world-view (BD-1).** Systems consume 8 of `NativeWorld`'s 40 members; **4 leak `Core.Interop` types** (`SpanLease`/`WriteBatch`/`InternedString`/`NativeComposite`). The context cannot re-expose `NativeWorld`; it must carry Contracts-level abstractions over span-read, batch-write, per-id Try/Has/Get, string interning, and composite storage.
- **D2 — no field/compute surface needed day-one.** 0 systems touch `Fields`/compute; the context can omit them (or defer to a later capability tier).
- **D3 — `delta` is dead.** 0 of 28 systems use `Update`'s `delta`; cadence comes from `[TickRate]` + tick-count fields. The `Update(float delta)` shape is empirically unused temporal API.
- **D4 — context lifetime (BD-2).** 27/28 systems hold no engine reference across ticks → context-per-tick is viable; `MovementSystem`'s `IPathfindingService` is the single held dependency the DI model must place (factory-delegate vs service-locator vs registration-time injection is open).
- **D5 — construction bifurcation (BD-2).** Core systems already receive ctor dependencies (`new MovementSystem(pathfinding)`); the mod path (`Activator.CreateInstance`) forbids it. Unifying the two paths is the BD-2 core.
- **D6 — bus surface (BD-3-adjacent).** Only Pawns + Inventory carry live traffic; Combat/Magic/World are declared-only. Any generic-routing decision touches a surface with almost no live producers to preserve.
- **D7 — lifecycle hooks.** `OnInitialize` (subscriptions) is the only lifecycle hook with live bodies (3 systems); `OnDispose` is used by none. The context/contract must expose subscribe-at-init; teardown is currently unexercised.
- **D8 — managed store.** `ManagedStore<T>()` (Path β) is used by 0 systems — it is a Core-origin no-op today (`SystemBase.cs:110-113`). Whether `ISystemContext` surfaces it is unforced by current usage.
- **D9 — `SystemExecutionContext` is the seed.** BD-1 need not invent the context — it already exists engine-internal with the exact `{world, services, buses, origin, modId, faultSink, managed-resolver}` shape; BD-1 is its Contracts-level, capability-scoped promotion.

### Anomalies (A-numbered — divergences neither the audit nor the kickoff anticipated)

- **A1** — Only **10 of 28** systems are wired into production (`GameBootstrap.coreSystems`); 18 are compiled-but-unwired classes. The migration's "systems to re-home" surface is 28 classes but 10 live behaviors.
- **A2** — `[BridgeImplementation]` **perfectly partitions** stub(18)/real(10) — an in-tree triage marker the migration can drive off directly.
- **A3** — **3 of 5 buses** (Combat, Magic, World) carry zero live system traffic; the genre-bus surface is mostly aspirational.
- **A4** — `MovementSystem` is the **sole** ctor-injected + engine-ref-holding system (and an `ECS.md` §8 anti-pattern instance) — the entire BD-2 tension concentrated in one class.
- **A5** — `[SystemAccess]` `reads:`/`writes:` slots hold **event types, not components**, in `ComboResolutionSystem`, `CompositeResolutionSystem`, and `ManaSystem` — the access model is used inconsistently by stubs.
- **A6** — Declared-vs-actual **read drift in REAL systems** (`JobSystem` over-declares Skills+Position; `HaulSystem` over-declares Position) — unenforced (`CONTRACTS.md` §6), so declarations are not a reliable usage oracle.
- **A7** — `Update`'s `delta` is **universally ignored** (0/28) — the one temporal parameter the base offers is dead.
- **A8** — `ManaSystem` holds a **dormant self-instantiated `ManaLeaseRegistry`** field never touched by any live path (`Update` is TODO) — dead state.
- **A9** — The composition root has **already partly dissolved** into `EngineSession` (EQ_A2 `GameBootstrap.CreateSession`), which W4/BD-2 intersects — the DI decision lands on a moving root.
- **A10** — The three **Faction** systems declare the **`World`** bus (no Faction bus exists); `CombatMod.cs` documents "Faction folds under Combat" — system taxonomy ≠ bus taxonomy ≠ mod taxonomy.

### Self-check (kickoff vs standing law)

**No kickoff↔standing-law conflict found.** The kickoff's mission (measure the SDK usage closure for BD-1/BD-2) is pure read-only measurement extending a DOC-E precedent; it asserts no enforcement and mutates nothing. `GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md` B-3 (SDK sufficiency) is the target law this recon feeds; `EXECUTION_AUTHORITY_MATRIX.md` names `SystemBase`/`IGameServices` as *current* sanctioned surfaces, which BD-1/BD-2 will supersede. The report measures current→target distance and records it; it decides nothing. The scaffolding ruling (`VANILLA_SEPARATION_MIGRATION_PLAN.md` §1.1) is honored: findings are classified by **capability class**, not by the fidelity of the sacrificial harness systems.

---

## Attestation

- **Read-only law honored.** Zero writes beyond this single report file; zero git mutations (no branch, commit, or stage — the report is UNTRACKED); zero builds; zero test runs; `dotnet run … Governance -- sync` **never run**. Derived registers and frontmatter were read directly only.
- **HEAD pinned:** `main` @ `df1541dd687c11e74607569e25d9d21a5c9102e7` (`df1541d`). All anchors reference this tree; the BD-1/BD-2 crux surfaces (`SystemBase`, `IGameServices`, `ModRegistry` Activator gate) were re-verified first-hand at this HEAD with zero drift.
- **Method.** Two parallel read-only per-system fan-out passes over all 28 systems (measured durations ~208 s and ~285 s), each extracting the same schema (attributes, ctor, fields/state, base-surface touch, Services/NativeWorld usage, stub/real), followed by first-hand reads of every load-bearing definition (`SystemBase`, `SystemExecutionContext`, `NativeWorld`, `IGameServices`, `RestrictedFieldApi`, `ModRegistry`, `ModBusRouter`, `GameBootstrap`) and the kickoff input docs, plus canonical `rg --count-matches` censuses. Every census names its rg expression; every claim is anchored `file:line`; load-bearing code is quoted verbatim.
- **Wall-clock:** single continuous session on 2026-07-19. The two fan-out passes are tool-timestamped (~208 s / ~285 s, parallel); the total session (governance-doc reads + first-hand verification + authoring) is **not** independently timestamped (no wall-clock instrument available).
- **Completeness:** T1–T6 all completed; no section left partial. The one soft figure (test-fixture count, audit 38 → 75) was re-measured per-file and explained (`TickSchedulerThreadSafetyTests.cs`=32); the `class \w+ : SystemBase` count-matches caveat (38 in src, inflated by README examples vs 28 concrete classes) is carried explicitly per the EQ_A3 provenance lesson.

---

*End of report.*
