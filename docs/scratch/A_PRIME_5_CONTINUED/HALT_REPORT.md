# A'.5 K8.3+K8.4 combined execution halt report

**Status**: HALT 2026-05-14 — K8.3+K8.4 combined brief execution halted at Phase 0 per METHODOLOGY §3 «stop, escalate, lock» and brief §8.3 AS-C1. No code commits. Working tree clean (the combined brief itself remains untracked; not committed as the prescribed «Commit 1» pending refresh).

**Authored**: 2026-05-14 by Claude Opus 4.7 (1M context) Cloud Code session, auto mode.

**Halt authority**: brief §8.3 AS-C1 — «A K-Lxx implication proves to depend on assumptions the brief did not surface. Action: HALT. Author HALT_REPORT in `docs/scratch/A_PRIME_5_CONTINUED/`. Crystalka authors brief refresh patch in deliberation mode.»

**Brief under execution**: `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` v1.0 AUTHORED 2026-05-13. Remains AUTHORED — not transitioned. Brief file still untracked on disk; not yet committed.

**Prior halt this resolves**: `docs/scratch/A_PRIME_5/HALT_REPORT.md` (K8.3 v2.0 execution halt 2026-05-13) — combined approach selected to structurally prevent the K8.3 v2.0 premise-miss class. The combined brief explicitly claims (§READ_ORDER + §0.1 + §12) that «the expected outcome from combined approach is **zero halts during execution** — premise misses that triggered K8.3 v2.0 halt are structurally prevented by atomic execution». This halt falsifies that claim. The mechanism preventing K8.3 v2.0's specific storage-location premise miss does not prevent the broader class of «architecture-document target state diverges silently from production-API surface».

---

## §1 Halt rationale

K8.3+K8.4 combined brief §2.1 «NativeWorld activation (Bootstrap.Run + registry)» prescribes the bootstrap path replacement as:

```csharp
public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")
{
    // Registry-based component type ids active per K-L4.
    var registry = new ComponentTypeRegistry();

    // NativeWorld constructed via K3 bootstrap graph (parallel allocation +
    // thread pool init). The registry is passed at construction so type ids
    // are deterministic from first AddComponent call. Replaces both
    // `new World()` and `new NativeWorld()` previously constructed separately.
    var nativeWorld = Bootstrap.Run(registry);

    // ...
    RegisterProductionComponentTypes(registry);
    // ...
}
```

§2.1 «Implementation notes» asserts:
> - `ComponentTypeRegistry.Register<T>` already exists (K2 era); idempotent re-registration verified.
> - `Bootstrap.Run(registry)` already exists (K3 era); registry parameter optional but used here as required.
> - `NativeWorld.AdoptBootstrappedHandle` already wired to support registry-bound construction.

Phase 0 §6.1 Step 3 deep-reads of [`src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs`](src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs:35) falsify the brief's assumed `new ComponentTypeRegistry()` callability. The premise is structurally wrong.

---

## §2 Empirical evidence

### §2.1 — Evidence A: `ComponentTypeRegistry` has no parameterless ctor and the only ctor is `internal`

[`src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs:35-44`](src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs:35):

```csharp
internal ComponentTypeRegistry(IntPtr worldHandle)
{
    if (worldHandle == IntPtr.Zero)
    {
        throw new ArgumentException(
            "Cannot bind ComponentTypeRegistry to a null world handle.",
            nameof(worldHandle));
    }
    _worldHandle = worldHandle;
}
```

The registry is **constructor-bound to a world handle**. There is:
- No `public ComponentTypeRegistry()` parameterless ctor.
- No `public ComponentTypeRegistry(IntPtr worldHandle)` ctor (visibility = `internal`).
- No `internal static` factory method.
- No `Bind(IntPtr)` or `Attach(IntPtr)` method that could late-bind a registry constructed without a handle.

The `_worldHandle` field is `private readonly` — captured at construction and used inside [`Register<T>`](src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs:64) to call `df_world_register_component_type(_worldHandle, id, size)`. The handle is operationally required for registry function, not just decorative.

### §2.2 — Evidence B: No `InternalsVisibleTo` grant from Core.Interop to Application

[`src/DualFrontier.Core.Interop/DualFrontier.Core.Interop.csproj:18-19`](src/DualFrontier.Core.Interop/DualFrontier.Core.Interop.csproj:18):

```xml
<InternalsVisibleTo Include="DualFrontier.Core.Benchmarks" />
<InternalsVisibleTo Include="DualFrontier.Core.Interop.Tests" />
```

`DualFrontier.Application` (containing `GameBootstrap.cs`) is not on the list. Even if the brief's author intended a caller in Application to pass `nativeWorld.HandleForInternalUseTest` to a (currently `internal`) ctor, the access would not compile across the assembly boundary.

### §2.3 — Evidence C: All existing call sites construct the registry with a world handle

Grep `new ComponentTypeRegistry(` across the codebase returns:
- 9 sites in `tests/DualFrontier.Core.Interop.Tests/ComponentTypeRegistryTests.cs` — all pass `world.HandleForInternalUseTest`
- 2 sites in `tests/DualFrontier.Core.Interop.Tests/BootstrapTests.cs` — same pattern
- 6 sites in `tests/DualFrontier.Core.Interop.Tests/VanillaComponentRoundTripTests.cs` — same pattern
- 0 sites in `src/` (production)
- The K8.3+K8.4 combined brief is the only artifact prescribing `new ComponentTypeRegistry()` (no args)

The K2/K3/K4 era briefs (precursors) all show the handle-bound construction. The brief's no-arg construction is novel and unsupported by the current API.

### §2.4 — Evidence D: `Bootstrap.Run` signature does support `registry` param, but cannot satisfy the chicken-and-egg

[`src/DualFrontier.Core.Interop/Bootstrap.cs:43-54`](src/DualFrontier.Core.Interop/Bootstrap.cs:43):

```csharp
public static NativeWorld Run(ComponentTypeRegistry? registry = null)
{
    IntPtr handle = NativeMethods.df_engine_bootstrap();
    if (handle == IntPtr.Zero) { throw ... }
    return NativeWorld.AdoptBootstrappedHandle(handle, registry);
}
```

Brief's correct that the param exists. But the registry has to already be constructed before this call — and **a constructed registry needs a world handle the caller can only get from this exact call**. Chicken-and-egg.

The only existing callers of `Bootstrap.Run` pass no args (FNV-1a path):
- [`tests/DualFrontier.Core.Benchmarks/BootstrapTimeBenchmark.cs:28`](tests/DualFrontier.Core.Benchmarks/BootstrapTimeBenchmark.cs:28)
- 6 sites in `tests/DualFrontier.Core.Interop.Tests/BootstrapTests.cs`
- 6 sites in `tests/DualFrontier.Core.Interop.Tests/VanillaComponentRoundTripTests.cs`

No production wiring uses `Bootstrap.Run` at all today (`GameBootstrap.cs:91` uses `new NativeWorld()` directly).

### §2.5 — Evidence E: `VanillaComponentRegistration.RegisterAll` already exists; brief's `RegisterProductionComponentTypes` overlaps with inventory drift

[`src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs:21-62`](src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs:21) — the K4-era helper:

```csharp
public static class VanillaComponentRegistration
{
    public static void RegisterAll(ComponentTypeRegistry registry)
    {
        // Shared (3): HealthComponent, PositionComponent, RaceComponent
        // Pawn (3): NeedsComponent, MindComponent, JobComponent
        // Items (5): BedComponent, ConsumableComponent, DecorativeAuraComponent,
        //            ReservationComponent, WaterSourceComponent
        // World (2): TileComponent, EtherNodeComponent
        // Magic (3): EtherComponent, GolemBondComponent, ManaComponent
        // Combat (1): ArmorComponent
        // Building (2): PowerConsumerComponent, PowerProducerComponent
        // Total: 19 components
        ...
    }
}
```

The brief's §2.1 `RegisterProductionComponentTypes` helper lists 16 components and has divergence from `VanillaComponentRegistration`:

| Brief list | In Vanilla helper? | Notes |
|---|---|---|
| ConsumableComponent | YES | |
| WaterSourceComponent | YES | |
| BedComponent | YES | |
| DecorativeAuraComponent | YES | |
| NeedsComponent | YES | |
| JobComponent | YES | |
| ReservationComponent | YES | |
| MindComponent | YES | |
| PowerConsumerComponent | YES | |
| PowerProducerComponent | YES | |
| StorageComponent | **NO** | not in Vanilla helper |
| MovementComponent | **NO** | not in Vanilla helper |
| PositionComponent | YES | |
| IdentityComponent | **NO** | not in Vanilla helper |
| SkillsComponent | **NO** | not in Vanilla helper |

Conversely, `VanillaComponentRegistration` registers HealthComponent, RaceComponent, TileComponent, EtherNodeComponent, EtherComponent, GolemBondComponent, ManaComponent, ArmorComponent — none of which appear in the brief's list.

The brief introduces an overlapping but inventory-divergent helper without referencing the existing one. Per brief §8.3 AS-C2 («A production component type discovered at Phase 0 inventory that is not in this brief's list»), this is a recordable drift. Combined with the overlap, the right model isn't obvious: extend `VanillaComponentRegistration` (single helper) vs. introduce a parallel `RegisterProductionComponentTypes` (two helpers, divergent inventories, redundant `Register<T>` calls for the 10 overlapping components). The brief doesn't surface this design choice.

### §2.6 — Evidence F: Solution file path in brief is wrong

Brief §6.1 Step 1 `dotnet test --no-build src/DualFrontier.sln` and §6.6 commit 22 `dotnet build src/DualFrontier.sln` reference `src/DualFrontier.sln`. The actual solution file is at the repo root: `DualFrontier.sln`. There is **no** `src/DualFrontier.sln` (only `src/DualFrontier.Presentation/DualFrontier.Presentation.sln` for the presentation subproject).

This is a minor brief authoring slip — easily corrected — but symptomatic of the same authoring path that produced the deeper premise misses: the brief was authored from architecture documents rather than verified against the working tree.

### §2.7 — Evidence G: Factory current shape, not as brief assumes

[`src/DualFrontier.Application/Scenario/RandomPawnFactory.cs:77-115`](src/DualFrontier.Application/Scenario/RandomPawnFactory.cs:77):

```csharp
public IReadOnlyList<EntityId> Spawn(World world, GameServices services, int count)
{
    if (world is null) throw new ArgumentNullException(nameof(world));
    // ...
    for (int i = 0; i < count; i++)
        ids.Add(SpawnOne(world, services, passable[i]));
    // ...
}

private EntityId SpawnOne(World world, GameServices services, GridVector pos)
{
    EntityId id = world.CreateEntity();
    world.AddComponent(id, new PositionComponent { Position = pos });
    // ... per-entity per-component AddComponent calls into managed World ...
    // K8.1 primitives use _nativeWorld via InternString / CreateMap / CreateComposite
    // ... finally world.AddComponent for the component records themselves ...
}
```

Brief §2.2 prescribes a bulk dual-write shape:

```csharp
public IReadOnlyList<EntityId> Spawn(NativeWorld nativeWorld, World world, IGameServices services, int count)
{
    // Allocate ids on nativeWorld; build arrays; bulk AddComponents per type;
    // then dual-write per-entity into managed World.
}
```

This is a substantial signature + body rewrite per factory. The brief lists it as a single commit (Phase 2 commit 3). For each per-pawn component (PositionComponent, IdentityComponent, NeedsComponent, MindComponent, JobComponent, MovementComponent, SkillsComponent — 7 components), the rewrite requires:
1. Allocating a `T[]` of `count` entries
2. Populating each entry
3. Calling `nativeWorld.AddComponents<T>(entities, populated)` (which requires T to be `unmanaged`)
4. Iterating again to dual-write into managed `World`

Two of these components hold K8.1 primitive handles bound to `nativeWorld` (`SkillsComponent.Levels` = `NativeMap`, `SkillsComponent.Experience` = `NativeMap`, `MovementComponent.Path` = `NativeComposite`). Each `Create*` call has side-effect on `nativeWorld` (allocates a fresh id from its monotonic counter). The bulk-write rewrite has to preserve the per-pawn handle allocation order to keep determinism (factory contract is byte-identical output given identical seed). The brief's bulk pattern doesn't address how per-pawn `Create*` calls interleave with the bulk `AddComponents` write.

This isn't a halt-eligible obstacle on its own — but it's an additional layer of unsurfaced complexity that, combined with §2.1-§2.6, multiplies the brief's premise misses.

---

## §3 The architectural diagnosis

### §3.1 Why brief authoring missed this (again)

The K8.3 v2.0 HALT_REPORT §3.1 lists four candidate root causes:
1. Closure-framing inheritance
2. Bootstrap comment not in mandatory reads
3. «Path α default» ambiguity
4. K-L3.1 amendment surface gap

This halt adds a fifth:

5. **Combined-brief authoring optimism**. The combined brief explicitly claims structural prevention of the K8.3 v2.0 halt class via atomic execution (§READ_ORDER, §0.1 §12). The author may have over-extended this claim — combining the milestones prevents intra-milestone sequence-dependency premise misses (one milestone's done-state being another's pre-condition), but doesn't prevent independent API-surface premise misses. The author wrote §2.1 «Implementation notes» as an assertion about API existence rather than a verification of API surface against actual files. The prior halt's lesson candidate (§7 of `A_PRIME_5/HALT_REPORT.md`) — «production wiring says what is true; architecture docs say what should be true; Phase 0 reads both; discrepancy is data for halt-escalate-lock» — was not applied to the combined brief authoring.

The combined brief's Phase 0 mandatory reads (§6.1 Step 3) **do** include `ComponentTypeRegistry.cs` (line 1319). The author listed the file as required reading for the executor, but didn't apply that requirement to the authoring step itself.

### §3.2 Scope of the gap across the brief

| Brief section | Issue | Severity |
|---|---|---|
| §0.4 (atomic commit shape) | Commit 3 «activation» assumes API change is a wiring switch | High — Commit 3 cannot mechanically execute |
| §2.1 (NativeWorld activation) | `new ComponentTypeRegistry()` does not compile | High — direct compile failure |
| §2.1 (implementation notes) | Asserts API exists that does not exist | High — false premise |
| §2.1 (RegisterProductionComponentTypes) | Overlaps existing `VanillaComponentRegistration.RegisterAll` with divergent inventory | Medium — design decision not surfaced |
| §2.2 (factory bulk pattern) | Conflates bulk `AddComponents` shape with per-pawn K8.1 primitive ordering | Medium — additional rewrite complexity |
| §6.1 (sln path) | `src/DualFrontier.sln` does not exist | Low — easily corrected |
| §6.6 (sln path, repeated) | Same | Low |
| §7.2 (NativeWorldTestFixture) | Same `new ComponentTypeRegistry()` premise | High — same root miss |

### §3.3 Combined-milestone claim vs. observed halt

The combined brief asserts:
> «The expected outcome from combined approach is **zero halts during execution** — premise misses that triggered K8.3 v2.0 halt are structurally prevented by atomic execution.»

This claim, examined empirically:
- ✅ **Structurally prevents the K8.3 v2.0 storage-location premise miss.** Combined milestone makes the «storage stays through K8.3» framing obsolete because K8.3 doesn't exist as a separate milestone.
- ❌ **Does not prevent other premise-miss classes.** API-surface, ctor-visibility, helper-inventory, sln-path are all independent of the original storage-location framing. They reproduce because the brief authoring methodology (architecture-document target state without runtime-file verification) was not changed.

The combined brief is itself an instance of the failure mode it claims to prevent. Halt-escalate-lock catches it structurally — the protocol is doing its job — but the brief authoring methodology improvement needed by the K8.3 v2.0 lesson candidate (§7 of prior HALT_REPORT) has not been operationalized.

---

## §4 What was NOT done (clean halt)

Per brief §8.3 AS-C1 + METHODOLOGY §3:

- ❌ No Commit 1 (brief authoring commit) — brief file `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` remains **untracked**. Not committed because the brief itself contains the premise misses; committing it as «ready for execution» would mis-tag it.
- ❌ No Phase 1 (orphan .uid cleanup) execution
- ❌ No Phase 2 (ComponentTypeRegistry activation + factory dual-write) execution
- ❌ No Phase 3.x per-system migrations (zero of 12)
- ❌ No Phase 4 verification
- ❌ No Phase 5 World retirement
- ❌ No Phase 6 closure documentation amendments
- ❌ No REGISTER.yaml mutations
- ❌ No audit_trail entry append
- ❌ No CAPA closure (CAPA-2026-05-12 and CAPA-2026-05-13 remain OPEN)
- ❌ No git commits
- ❌ No edits to any tracked file in `src/`, `tests/`, `mods/`, `native/`
- ❌ No edits to LOCKED documents

**Working tree state at halt** (relative to HEAD `ced2842`):
- `M .claude/settings.local.json` (pre-existing modification, unrelated; harmless permission-list addition for `git push *`)
- `?? tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` (untracked — the brief itself)
- `?? docs/scratch/A_PRIME_5_CONTINUED/` (untracked — this halt artifact)

HEAD is `ced2842` (one commit ahead of brief's claimed authoring HEAD `efd67df`). The intervening commit `ced2842 fix(governance): relocate auto-generated frontmatter to end-of-file for README.md` is a benign cosmetic change; it does not affect the K8.3+K8.4 premise.

---

## §5 Resolution path (deliberation session input)

Per brief §8.3 AS-C1: «Crystalka authors brief refresh patch in deliberation mode. Resume execution against patch.»

### §5.1 Candidate resolutions (deliberation surface)

**Disposition A — Brief refresh patch authored as patch artifact** (parallels K8.3 v2.0 → K8.3 patch precedent):
- New artifact: `tools/briefs/K8_34_COMBINED_BRIEF_REFRESH_PATCH.md` AUTHORED
- Patch covers: §2.1 ComponentTypeRegistry construction flow (one of: parameterless ctor + Bind method; OR Bootstrap.Run returns (world, registry) tuple; OR `new NativeWorld(...)` constructs internal registry); §2.1 inventory reconciliation with `VanillaComponentRegistration.RegisterAll`; §6.1/§6.6 sln path correction; §2.2 factory bulk pattern interleave specification.
- Combined brief + patch executed together (similar to K9 brief + K9 brief patch precedent).

**Disposition B — Brief v1.1 in-place revision** (alternative to patch — supersedes the v1.0 directly):
- Brief file modified in place; version bumped 1.0 → 1.1; «AUTHORED 2026-05-13 / revised 2026-05-14» history added.
- Single authoritative artifact remains; no patch overlay to mentally compose.
- Loses the K9/K8.3 patch precedent of «patch as first-class historical artifact».

**Disposition C — Brief v2.0 full re-author**:
- Brief v1.0 transitions AUTHORED → DEPRECATED.
- New brief authored from scratch with verified API surface, Phase 0 reads applied to authoring, and explicit «inventory drift vs `VanillaComponentRegistration` recorded as design decision».
- Heaviest option; warranted only if the premise misses are wider than §3.2 enumerates.

**Disposition D — Decompose into smaller execution units**:
- Split combined milestone into:
  - A'.5a — ComponentTypeRegistry API refactor (parameterless ctor + Bind, OR Bootstrap.Run returning tuple, OR similar; chosen via deliberation)
  - A'.5b — `VanillaComponentRegistration` inventory reconciliation (add missing 4, decide on overlap)
  - A'.5c — GameBootstrap rewiring (the original brief Phase 2)
  - A'.5d — Factories' bulk + dual-write (the original brief Phase 2 commit 3 second half)
  - A'.5e — Mod API v3 surface (the original brief Phase 3)
  - A'.5f — Per-system migration (original Phase 4)
  - A'.5g — World retirement (original Phase 5)
  - A'.5h — Closure (original Phase 6)
- Restores incremental-commit safety; loses the «atomic milestone» property the combined brief was designed for.

### §5.2 Recommended deliberation questions

- **Q-K8.34-Premise-1**: which Disposition (A/B/C/D) best preserves the combined-brief atomicity property AND the «без компромиссов» commitment?
- **Q-K8.34-Premise-2**: design choice — should `ComponentTypeRegistry` gain a parameterless ctor + `Bind` method, OR should `Bootstrap.Run` return `(NativeWorld, ComponentTypeRegistry)` tuple, OR should `NativeWorld` construct its own registry internally on null pass-through? (Each has different consumer impact and API surface implications; combined brief silently presumed the first.)
- **Q-K8.34-Premise-3**: should `RegisterProductionComponentTypes` (brief §2.1) be merged into `VanillaComponentRegistration.RegisterAll`, or should the two remain as separate helpers with explicit role separation? If separate: what is the inventory boundary?
- **Q-K8.34-Premise-4**: factory bulk-write — how do per-pawn `_nativeWorld.CreateMap` / `CreateComposite` calls (which mutate `nativeWorld` per pawn) interleave with bulk `nativeWorld.AddComponents<T>` calls (which require pre-built arrays)? Brief §2.2 shows the bulk shape without resolving the interleave.
- **Q-K8.34-Premise-5**: methodology — should METHODOLOGY v1.8 add a lesson «brief authoring methodology amendment: any brief asserting API existence must include a grep verification of the asserted API surface against `src/` at brief authoring time, captured in the brief's provenance section»? (The K8.3 v2.0 HALT_REPORT §7 lesson candidate addresses one half; this halt's miss suggests a second half: not just runtime-state, but API-surface verification.)
- **Q-K8.34-Premise-6**: combined-milestone meta — does the «atomic execution structurally prevents premise miss» claim survive after this halt, restricted to its actual scope (storage-location class), with explicit acknowledgement that other premise-miss classes are not addressed by atomicity?

### §5.3 CAPA reproduction

This halt reproduces a brief-authoring premise miss. Two CAPA candidates:

- **CAPA-2026-05-14-K8.34-API-SURFACE-PREMISE-MISS** (new):
  - Trigger: «K8.3+K8.4 combined brief asserted `ComponentTypeRegistry` API surface that does not exist; the brief assumed a parameterless ctor and ignored `internal` visibility»
  - Corrective action: TBD per deliberation — likely brief-authoring methodology amendment (API surface grep verification at authoring time)
  - Effectiveness verification: next brief that asserts API existence passes the grep check at authoring time; halt-escalate-lock at execution time does not surface API-surface mismatches.

- **CAPA-2026-05-13-K8.3-PREMISE-MISS** (existing, brief §9.4 plans to close at combined milestone closure):
  - Status: REMAINS OPEN — combined milestone has not closed and would not close at this brief execution; the halt blocks closure.
  - Effectiveness verification: combined milestone claim of structural prevention is partially falsified (storage-location class addressed; API-surface class not addressed). Effectiveness verification result: **PARTIAL**; needs deliberation-time amendment.

### §5.4 RISK-008 reproduction

The K8.3 v2.0 HALT_REPORT §5.3 noted RISK-008 covers brief-authoring failure modes. This halt reproduces that risk. The stop-escalate-lock protocol caught the miss structurally before harmful commits — protocol operating as designed per METHODOLOGY §3. Same logic as prior halt; same outcome.

---

## §6 Phase 0 read inventory at halt time

Reads performed before halt declaration (operational ground truth captured):

1. ✅ `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` (3,117 lines) — full read
2. ✅ `src/DualFrontier.Application/Loop/GameBootstrap.cs` — full read; embedded comment block lines 85-90 (K8.2 v2 transitional state) confirmed present
3. ✅ `src/DualFrontier.Core/ECS/SystemBase.cs` — full read; legacy `GetComponent`/`SetComponent`/`Query`/`Query<T1,T2>`/`GetSystem` methods present
4. ✅ `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` — full read; ctor signature confirmed: `World world` + `NativeWorld? nativeWorld`
5. ✅ `src/DualFrontier.Contracts/Modding/IModApi.cs` — full read; v2 surface confirmed (no constraint split, no `RegisterManagedComponent`)
6. ✅ `src/DualFrontier.Core.Interop/Bootstrap.cs` — full read; signature `Run(ComponentTypeRegistry? registry = null)` confirmed
7. ✅ `src/DualFrontier.Core.Interop/NativeWorld.cs` — full read; `AcquireSpan<T>`, `BeginBatch<T>`, `AddComponents<T>` bulk all present and matching brief expectations
8. ✅ `src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs` — full read; **internal-only ctor with required IntPtr handle — halt trigger**
9. ✅ `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` — full read; current ctor takes both `World` + `NativeWorld?`
10. ✅ `src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs` — full read; the K4-era helper present with 19-component list
11. ✅ `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs` — full read; signature `Spawn(World, GameServices, int)` confirmed; K8.1 primitive per-pawn pattern confirmed
12. ✅ `docs/scratch/A_PRIME_5/HALT_REPORT.md` — full read; prior halt rationale + lesson candidate confirmed

Reads grepped but not full-read:
- `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` (presence verified per brief §6.1 step 5 IC-3)
- `tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md` (presence verified)
- `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` (presence verified)
- 17 other files with `ComponentTypeRegistry` matches (call-site sample only)

### §6.1 Hard gates state at halt declaration

- HG-1 (working tree clean): **partial** — `.claude/settings.local.json` modified (benign; permission-list `git push *` addition) + brief file untracked (expected per §6.1 Step 0). No `src/`/`tests/` modifications.
- HG-2 (baseline tests passing): not run — initial `dotnet test` command failed due to incorrect sln path (brief's `src/DualFrontier.sln`); root sln exists; baseline test run preempted by premise-miss discovery before retest.
- HG-3 (native build clean): not run — preempted by premise-miss discovery.
- HG-4 (register operational, `sync_register.ps1 --validate`): not run — preempted.
- HG-5 (K8.3 halt artifacts on disk): **PASS** — all three artifacts present at expected paths.
- IC-1 (HEAD SHA): `ced28420511ad23320c8297155dbe2f0c902adb1`
- IC-2 (commits ahead of origin): not measured.
- IC-3 (supersession targets exist): **PASS** — three superseded-target briefs present on disk.

---

## §7 Methodology lesson candidate (for METHODOLOGY v1.8 if Crystalka chooses to add)

This extends the K8.3 v2.0 HALT_REPORT §7 lesson candidate. That lesson focused on production runtime ground-truth verification. This halt surfaces a related but distinct need: API surface verification at brief authoring time.

**Title**: API surface verification as brief authoring discipline

**Body draft** (subject to Crystalka edit + deliberation):

> Briefs that assert «API X already exists» as a foundation for prescribed execution must verify the API surface against the actual files at brief authoring time, not just the architecture documents. The K8.3 v2.0 halt (2026-05-13) surfaced the need for Phase 0 *runtime-state* verification (architecture says what should be true; runtime files say what is true). The K8.3+K8.4 combined halt (2026-05-14) surfaces a parallel need for brief-authoring-time *API surface* verification: architecture says what API should exist; source files say what API does exist.
>
> **Brief authoring requirement**: any brief that prescribes a specific construction syntax (`new ComponentTypeRegistry()`), method call (`Bootstrap.Run(registry)` with specific parameter shape), or signature change (`RegisterComponent<T> where T : unmanaged, IComponent`) must capture in its provenance section a grep query and result snippet demonstrating that the asserted surface exists in `src/` at brief authoring HEAD. The provenance section is structured evidence, not narrative.
>
> **Failure mode (observed at K8.3+K8.4 combined execution halt, 2026-05-14)**: Combined brief §2.1 asserted `new ComponentTypeRegistry()` callability based on §2.1 «Implementation notes» — narrative claims of API existence. Phase 0 execution-time reads falsified the claim. `ComponentTypeRegistry`'s only ctor is `internal ComponentTypeRegistry(IntPtr worldHandle)`. The brief author treated architecture-document target state («registry-based component type ids active per K-L4») as if it implied API existence in callable form; the implementation requires API surface modification not enumerated in the brief.
>
> **Principle**: architecture docs say what target state should be; source files say what is callable now. Brief authoring reads both. Discrepancy is brief authoring data, not execution data — fixed at authoring time, not deferred to executor.
>
> **Falsifiable claim**: future briefs that include API-surface grep snippets in provenance will encounter fewer API-mismatch halts at execution time. Counter-example would force re-examination of the verification protocol.
>
> **Relationship to K8.3 v2.0 lesson**: K8.3 v2.0 added a runtime-state check (read `GameBootstrap.cs` and equivalents). This adds an API-surface check (grep for asserted API existence). Both checks are Phase 0 sub-steps for brief authoring, not Phase 0 sub-steps for executor. They prevent different premise-miss classes; both are necessary.
>
> **Combined-milestone meta-lesson**: the K8.3+K8.4 combined-milestone claim of «atomic execution structurally prevents premise miss» is true for one premise-miss class (cross-milestone sequence-dependency) and false for others (API surface, helper inventory, sln path). The atomicity property does not subsume brief-authoring discipline. Combined milestones still need both Phase 0 verification protocols at brief authoring time.

---

## §8 Provenance

- **Authored**: 2026-05-14, Claude Opus 4.7 (1M context) Cloud Code session, auto mode
- **Triggering event**: K8.3+K8.4 combined brief execution session, Phase 0 sub-step §6.1 Step 3 (mandatory deep-reads)
- **Brief under execution**: `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` v1.0 AUTHORED 2026-05-13; lifecycle remains AUTHORED
- **Halt direction**: Brief §8.3 AS-C1 (self-prescribed escalation) + METHODOLOGY §3
- **Working tree state at halt**: clean except this new file + brief file (untracked) + benign `.claude/settings.local.json` modification
- **HEAD at halt**: `ced28420511ad23320c8297155dbe2f0c902adb1` (one commit beyond brief's claimed authoring HEAD `efd67df`; intervening commit is benign README frontmatter cosmetic relocation)

### §8.1 Related governance artifacts (to author post-deliberation)

- EVT-2026-05-14-K8.34-EXECUTION-HALT (audit_trail) — to be appended at deliberation closure
- CAPA-2026-05-14-K8.34-API-SURFACE-PREMISE-MISS (capa_entries) — to be opened at deliberation
- REGISTER.yaml entry for this artifact: DOC-E-A_PRIME_5_CONTINUED_K8_34_HALT_INVESTIGATION (category E scratch, tier 3, lifecycle EXECUTED at this artifact's authoring)
- METHODOLOGY v1.8 lesson if Crystalka adopts §7 above
- CAPA-2026-05-13-K8.3-PREMISE-MISS effectiveness verification: **PARTIAL** — combined approach prevents one premise-miss class but not all

### §8.2 Comparison with K8.3 v2.0 halt

| Dimension | K8.3 v2.0 halt (2026-05-13) | K8.3+K8.4 combined halt (2026-05-14) |
|---|---|---|
| Halt phase | Phase 0.2 (production state verification) | Phase 0 Step 3 (deep-reads) |
| Premise miss class | Storage location (managed vs native at K8.3 time) | API surface (ctor visibility + signature) + helper inventory drift + sln path |
| Specific root | K-L3.1 amendment didn't surface K8.3-vs-K8.4 split | Brief §2.1 «Implementation notes» asserted API without verification |
| Brief artifact disposition | Stayed AUTHORED; patch brief authored same day | Stays AUTHORED; patch or v1.1 revision pending |
| Code commits | 0 | 0 |
| Working tree edits | 0 (`src/`, `tests/`, `mods/`, `native/`) | 0 (`src/`, `tests/`, `mods/`, `native/`) |
| Time to halt | ~30 minutes (deep-reads to halt declaration) | ~30 minutes (deep-reads to halt declaration) |
| Resolution path | Patch brief 2026-05-13 → combined brief 2026-05-13 | Brief refresh patch OR v1.1 revision (Crystalka chooses) |
| Methodology lesson | «Production runtime ground-truth verification» | «API surface verification at brief authoring» (this) |

Both halts demonstrate the «stop, escalate, lock» protocol catching brief-authoring premise misses structurally before destructive execution. Both halt at Phase 0. Both produce ~30-line scratch artifacts plus deliberation surface. The pattern is converging — the protocol is reliable; the brief authoring discipline is the remaining gap.

---

**Artifact end. Deliberation session input ready. Brief v1.0 lifecycle remains AUTHORED pending Crystalka resolution.**
