---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH
companion_to: DOC-D-K8_34_COMBINED
---
# K8.3+K8.4 Combined Brief Refresh Patch — Companion to K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md v1.0

**Status**: AUTHORED 2026-05-14 — companion patch to `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` v1.0 (authored 2026-05-13).
**Scope**: Corrects five API-surface / helper-inventory / build-path premise misses discovered during Claude Code execution attempt 2026-05-14. Combined brief Phase 0 deep-reads surfaced them **before any source commit** — halt was structurally correct per brief §8.3 AS-C1 + METHODOLOGY §3.
**Authority**: This patch overrides specific §2.1, §2.2, §2.3, §4.1, §6.1, §6.3, §7.2 statements and all `dotnet build` invocations in the combined brief. The combined brief's **architectural intent is fully preserved** — all 8 Q-COMBINED locks hold unchanged. The corrections are at the API-mechanics layer, not the architecture layer.
**Milestone**: A'.5 (combined K8.3+K8.4 — combined brief + this patch as override pair).
**CAPA opened**: CAPA-2026-05-14-K8.34-API-SURFACE-MISS — combined brief authoring worked from architecture docs without verifying actual constructor surface of `ComponentTypeRegistry`, the existence of `VanillaComponentRegistration.RegisterAll`, the solution-file path, and the real factory K8.1-primitive structure. Same root-cause class as CAPA-2026-05-13-K8.3-PREMISE-MISS (brief from docs, not from API), different specific surface.

---

## ⚠ READ ORDER (critical — read this section first)

You are the executor of the Claude Code session opened to perform Milestone A'.5 (K8.3+K8.4 Combined Kernel Cutover). Two briefs are attached to this session:

1. **`tools/briefs/K8_34_COMBINED_BRIEF_REFRESH_PATCH.md`** — this document
2. **`tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md`** — the main combined brief v1.0 (authored 2026-05-13)

**Read THIS document FIRST.** Then read the combined brief with the five overrides below applied mentally.

Reason: the combined brief §2.1 / §2.2 / §2.3 / §4.1 / §6.1 / §6.3 / §7.2 are authored under premises about the `ComponentTypeRegistry` constructor surface, a (non-existent) `RegisterProductionComponentTypes` helper, the solution-file location, and the factory bulk-write shape. Those premises are false at HEAD. A prior Claude Code session correctly halted at Phase 0 — zero source commits, clean working tree — and authored a HALT_REPORT. This patch resolves all five findings so the combined brief executes cleanly.

The combined brief's claim of «zero halts during execution» was **partially falsified** — atomicity prevented the storage-location premise class (CAPA-2026-05-13), but not the API-surface class. The brief should have claimed «halts caught in Phase 0 before any commit», which is what the system delivered. This patch records that correction (§7).

---

## ⚠ DISPOSITION (the load-bearing decision this patch carries)

**Lock**: Q-COMBINED-PATCH ratified by Crystalka 2026-05-14 («да я сразу отправлю новый патч бриф в открытую сессию»).

**Resolution**: **Patch brief as override layer** — same pattern as `K8_3_BRIEF_REFRESH_PATCH.md` (2026-05-13) and `K9_BRIEF_REFRESH_PATCH.md` (2026-05-10). The combined brief v1.0 is preserved on disk as authored; this patch sits alongside as the override layer. The pair (combined brief + this patch) is the execution input for milestone A'.5.

The combined brief is **NOT amended in place**. All 8 Q-COMBINED locks, the 6-phase structure, the 24-commit shape, the test strategy, the stop conditions, and the closure protocol remain authoritative as written **except** where this patch explicitly overrides.

**No architectural lock changes.** Findings 1-5 are mechanics corrections. Q-COMBINED-1 through Q-COMBINED-8 hold verbatim.

---

## ⚠ HALT ARTIFACT HANDLING (current Claude Code session continuation)

The prior Claude Code session halted at Phase 0 with **zero source commits** and authored `docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT.md`. Working tree state at handoff: the combined brief itself (untracked), this patch (untracked once delivered), the HALT_REPORT (untracked), and an unrelated `.claude/settings.local.json` modification.

**Resolution sequence** for the session that executes this patch:

### Step 1 — Preserve the HALT_REPORT as a committed artifact

```powershell
git status --porcelain
# Expected: HALT_REPORT.md untracked, combined brief untracked, this patch untracked,
# .claude/settings.local.json modified

# Stage and commit the halt artifact + both briefs together as the milestone's
# Commit 1 (replaces the combined brief's original "Commit 1 = brief only" plan —
# the brief was correctly NOT committed as "ready" by the halting session).
git add docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT.md
git add tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md
git add tools/briefs/K8_34_COMBINED_BRIEF_REFRESH_PATCH.md
git commit -m "docs(briefs,scratch): K8.3+K8.4 combined brief + refresh patch + Phase 0 halt report"
```

Do **not** commit `.claude/settings.local.json` — it is a local-only permission change.

### Step 2 — Proceed to combined brief Phase 1

With Commit 1 landed (brief + patch + halt report), the milestone continues at **combined brief §6.2 Phase 1** (orphan `.uid` cleanup), exactly as the combined brief specifies. The 24-commit count is unchanged — Commit 1 is still "brief authoring commit", it just now also carries the patch + halt report.

All subsequent commits (2-24) proceed per combined brief §6, with the five overrides in this patch applied.

---

## Overview — what this patch changes

| Combined brief section | Status | Override per this patch |
|---|---|---|
| §0 (all 8 Q-COMBINED locks) | UNCHANGED | All architectural locks hold verbatim |
| §1 Goal | UNCHANGED | Falsifiable claims hold |
| §2.1 NativeWorld activation | **OVERRIDDEN** — Finding 1 + Finding 3 | See §1 of this patch |
| §2.2 Component storage migration mechanics | **OVERRIDDEN** — Finding 5 | See §4 of this patch |
| §2.3 Dual-write phase mechanics | **OVERRIDDEN** — Finding 5 | See §4 of this patch |
| §2.4 World class movement | UNCHANGED | Phase 5 commit 22 mechanics hold |
| §3 (all) System layer | UNCHANGED | SystemBase / SystemExecutionContext / per-system patterns hold |
| §4.1 IModApi.RegisterComponent constraint split | UNCHANGED conceptually — minor note | See §5 of this patch (the `unmanaged` constraint already matches reality — confirmation, not change) |
| §4.2–§4.6 Mod API v3 surface | UNCHANGED | ManagedStore / resolver / attribute design holds |
| §5 (all) Manifest schema | UNCHANGED | manifestVersion gate holds |
| §6.1 Phase 0 pre-flight | **OVERRIDDEN** — adds API-surface verification steps | See §2 of this patch |
| §6.3 Phase 2 storage foundation | **OVERRIDDEN** — Commits 3-4 reshaped | See §3 of this patch |
| §6.2, §6.4, §6.5, §6.6, §6.7 | UNCHANGED | Phases 1, 3, 4, 5, 6 hold |
| §7.2 NativeWorldTestFixture | **OVERRIDDEN** — Finding 3 | See §6 of this patch |
| §7.1, §7.3–§7.6 | UNCHANGED | Test strategy holds |
| §8 Stop conditions | UNCHANGED | The halt that produced this patch is §8.3 AS-C1 working as designed |
| §9 Closure protocol | UNCHANGED structurally — one addition | See §8 of this patch (CAPA-2026-05-14 closure added to Phase 6 commit 24) |
| §10 Estimated atomic commit log | **OVERRIDDEN** — Commit 3 absorbs Bootstrap.Run refactor | See §3 of this patch |
| §11 Provenance | UNCHANGED — this patch appends to lineage | See §9 of this patch |
| All `dotnet build src/DualFrontier.sln` invocations | **OVERRIDDEN** — Finding 4 | See §5 of this patch — sln is at repo root |

---

## §1 Finding 1 + Finding 3 — `ComponentTypeRegistry` construction + reuse of `VanillaComponentRegistration` — combined brief §2.1 OVERRIDDEN

### §1.1 — The two findings stated

**Finding 1 — `ComponentTypeRegistry` has no constructible public surface.**

Combined brief §2.1 prescribes:
```csharp
var registry = new ComponentTypeRegistry();          // ← does NOT compile
var nativeWorld = Bootstrap.Run(registry);
RegisterProductionComponentTypes(registry);
```

Actual `ComponentTypeRegistry` surface (verified `src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs`):
- Class is `public sealed`, but the **only** constructor is `internal ComponentTypeRegistry(IntPtr worldHandle)`.
- No parameterless constructor. No public constructor. No factory method.
- `DualFrontier.Core.Interop.csproj` grants `InternalsVisibleTo` only to `Core.Benchmarks` + `Core.Interop.Tests` — **not** to `DualFrontier.Application` (where `GameBootstrap.cs` lives). Even the internal ctor is unreachable from Application.
- Chicken-and-egg: the registry ctor needs a world handle; the world handle only exists after a native world is created.

**`Bootstrap.Run` signature is a stale K3 artifact.** `Bootstrap.Run(ComponentTypeRegistry? registry = null)` accepts a pre-built registry — but **no caller can build one**, because the ctor needs a handle that does not exist until `Bootstrap.Run` itself produces it. The registry-parameter path of `Bootstrap.Run` has never been reachable from production. `Bootstrap.Run(null)` → FNV-1a fallback is the only path that currently works.

The test pattern (`ComponentTypeRegistryTests.cs`) confirms this: tests do `var world = new NativeWorld(); var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);` — using a **test-only** internal accessor (`HandleForInternalUseTest`, named explicitly for cross-assembly test reach). And even there, the registry is **never bound back** to the world — `world._registry` stays null. The test pattern exercises the registry in isolation; it does not produce "a NativeWorld with an active registry".

**Conclusion**: there is currently **no working path** to activate a registry in production. K2 built the registry, K3 built `Bootstrap.Run`, K4 built `VanillaComponentRegistration.RegisterAll` — but the connecting path was never closed, because K8.4 (which this combined milestone absorbs) is the milestone that closes it. This is a genuine kernel-level gap, not merely a brief typo. The combined brief §2.1 described a non-existent API instead of **designing how to close the loop**.

**Finding 3 — `RegisterProductionComponentTypes` duplicates an existing K4-era helper.**

`VanillaComponentRegistration.RegisterAll` already exists at `src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs` (verified). Its doc-comment states verbatim: *"Called from the Application bootstrap chain after Bootstrap.Run() returns a ready NativeWorld."* It accepts an already-constructed `ComponentTypeRegistry` and registers the Vanilla production component set.

The combined brief's invented `RegisterProductionComponentTypes` helper duplicates this with inventory drift. Per Crystalka direction 2026-05-14 («Финдинг 3 тут можно переиспользовать») and the project anti-costyl principle, the invented helper is **deleted**; `RegisterAll` is the single source of truth, extended where needed.

### §1.2 — Resolution: `Bootstrap.Run` constructs the registry internally

**The clean fix — registry construction moves inside `Bootstrap.Run`.** The registry conceptually belongs to the world (its ctor needs the world handle; its doc-comment says "bound to this world"; `NativeWorld.Registry` is already a public getter). The only place a handle exists while the world is still being assembled — and not yet handed out — is `Bootstrap.Run`. `Bootstrap` and `ComponentTypeRegistry` are in the **same assembly** (`DualFrontier.Core.Interop`), so the `internal` ctor is legally reachable there with no `InternalsVisibleTo` hack and no public handle exposure.

**`Bootstrap.cs` — new signature** (replaces the stale `Run(ComponentTypeRegistry? registry = null)`):

```csharp
/// <summary>
/// Performs native engine bootstrap and returns a ready NativeWorld.
/// </summary>
/// <param name="useRegistry">
/// When true (default, K8.3+K8.4 production path), a ComponentTypeRegistry is
/// constructed against the bootstrapped world handle and bound to the returned
/// NativeWorld — deterministic sequential type ids per K-L4. When false, the
/// legacy FNV-1a fallback path is used (retained for the rare diagnostic case;
/// no production caller passes false post-K8.3+K8.4).
/// </param>
/// <returns>
/// Ready-to-use NativeWorld. When useRegistry is true, NativeWorld.Registry is
/// non-null and ready for VanillaComponentRegistration.RegisterAll.
/// </returns>
/// <exception cref="BootstrapFailedException">If native bootstrap fails.</exception>
public static NativeWorld Run(bool useRegistry = true)
{
    IntPtr handle = NativeMethods.df_engine_bootstrap();
    if (handle == IntPtr.Zero)
    {
        throw new BootstrapFailedException(
            "df_engine_bootstrap returned null. Native side performed " +
            "full rollback. See native logs (when implemented) for the " +
            "specific task that failed.");
    }

    // Registry construction lives here — the single point where the handle
    // exists but the world is not yet handed to callers. internal ctor is
    // legally reachable: Bootstrap and ComponentTypeRegistry share the
    // DualFrontier.Core.Interop assembly. K-L5 atomicity preserved — the
    // world is either fully ready (handle + bound registry) or an exception
    // propagated.
    ComponentTypeRegistry? registry =
        useRegistry ? new ComponentTypeRegistry(handle) : null;

    return NativeWorld.AdoptBootstrappedHandle(handle, registry);
}
```

The old `Run(ComponentTypeRegistry? registry = null)` overload is **deleted** — it is a dead K3 artifact no caller could use. This is a clean break consistent with the combined milestone's no-backward-compatibility stance (Q-COMBINED-2). Any test or call site that passed a registry to `Bootstrap.Run` (verify at Phase 0 — likely zero, since the path was unreachable) migrates to `Run(useRegistry: true)` then reads `world.Registry`.

`AdoptBootstrappedHandle(handle, registry)` is **unchanged** — it already accepts `(IntPtr, ComponentTypeRegistry?)` and binds the registry into the new `NativeWorld`. `NativeWorld.Registry` getter is **unchanged** — already public.

### §1.3 — Combined brief §2.1 replacement text

The combined brief §2.1 "Target state (post-combined)" block is **replaced** by:

```csharp
public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")
{
    // K8.3+K8.4 — NativeWorld is the sole production storage path (K-L11).
    // Bootstrap.Run constructs the ComponentTypeRegistry internally against
    // the bootstrapped handle and binds it to the world (registry-based
    // deterministic type ids per K-L4). No managed World in production.
    var nativeWorld = Bootstrap.Run(useRegistry: true);

    // K4-era helper — registers the Vanilla production component set with the
    // world's bound registry. Single source of truth for production component
    // type-id assignment; extended in this milestone to cover the full
    // coreSystems + factory component set (see patch §1.4).
    VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!);

    // ... (rest of wiring; factories consume nativeWorld — see patch §4) ...
}
```

The invented `RegisterProductionComponentTypes` helper and its body in combined brief §2.1 are **deleted entirely**. There is no parallel registration helper.

### §1.4 — `VanillaComponentRegistration.RegisterAll` extension

`RegisterAll` as it exists registers 20 component types across 7 categories (verified — the "World category (3)" comment is stale; only `TileComponent` + `EtherNodeComponent` are registered, so 20 total, not 21):

```
Shared:   HealthComponent, PositionComponent, RaceComponent
Pawn:     NeedsComponent, MindComponent, JobComponent
Items:    BedComponent, ConsumableComponent, DecorativeAuraComponent,
          ReservationComponent, WaterSourceComponent
World:    TileComponent, EtherNodeComponent
Magic:    EtherComponent, GolemBondComponent, ManaComponent
Combat:   ArmorComponent
Building: PowerConsumerComponent, PowerProducerComponent
```

**Phase 0 inventory task** (verify against `RandomPawnFactory` + `ItemFactory` + the 12 coreSystems' `[SystemAccess]` declarations): the combined brief §2.1 component list named several types **not** currently in `RegisterAll` — at minimum `IdentityComponent`, `SkillsComponent`, `MovementComponent`, `StorageComponent`. Possibly `FactionComponent`, `WorkbenchComponent` (combined brief §4.1 inventory table listed them, though it noted they are not coreSystems-touched).

**Resolution**: the milestone **extends `RegisterAll`** with the missing production component types — it does **not** create a parallel helper. The extension is part of Phase 2 Commit 3 (see patch §3). The exact additions are determined at Phase 0 by the empirical inventory:

```powershell
# Phase 0 — enumerate every component type the factories instantiate
Select-String -Path src/DualFrontier.Application/Scenario/RandomPawnFactory.cs `
  -Pattern 'new (\w+Component)' | ForEach-Object { $_.Matches.Groups[1].Value } | Sort-Object -Unique
Select-String -Path src/DualFrontier.Application/Scenario/ItemFactory.cs `
  -Pattern 'new (\w+Component)' | ForEach-Object { $_.Matches.Groups[1].Value } | Sort-Object -Unique

# Phase 0 — enumerate every component type the 12 coreSystems read/write
# (via [SystemAccess] attributes or AcquireSpan<T>/BeginBatch<T> once migrated)
Select-String -Path src/DualFrontier.Systems/ -Pattern '\[SystemAccess' -Recurse -Context 0,3

# Cross-reference against RegisterAll's current list; the delta is the
# RegisterAll extension set.
```

Every component in the union of (factory-instantiated) ∪ (coreSystems-accessed) must appear in `RegisterAll`. `RegisterAll`'s K-L4 contract — stable order, sequential ids — is preserved: new entries append to their category blocks; existing entries do not reorder (id stability across runs).

**`RegisterAll` is also the single source of truth for the test fixture** — see patch §6.

---

## §2 Finding 1-5 — combined brief §6.1 Phase 0 pre-flight — OVERRIDDEN (additions)

The combined brief §6.1 Phase 0 already mandates deep-reads of `ComponentTypeRegistry.cs`, `Bootstrap.cs`, `NativeWorld.cs`, the factories. The prior halt proves those deep-reads were insufficient at brief-authoring time. Phase 0 gains **explicit API-surface verification steps** — checks that would have caught all five findings.

**§6.1 Step 3.5 (NEW) — API-surface premise verification**:

```powershell
# APV-1 — ComponentTypeRegistry has no parameterless ctor (Finding 1)
# Confirm the only ctor is internal(IntPtr). If a parameterless ctor has
# appeared since this patch was authored, the Bootstrap.Run refactor in
# patch §1.2 may be unnecessary — escalate to confirm.
Select-String -Path src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs `
  -Pattern 'ComponentTypeRegistry\(' -SimpleMatch
# Expected: exactly one match — "internal ComponentTypeRegistry(IntPtr worldHandle)"

# APV-2 — Bootstrap.Run current signature (Finding 1)
Select-String -Path src/DualFrontier.Core.Interop/Bootstrap.cs `
  -Pattern 'public static NativeWorld Run' -SimpleMatch
# Expected: "Run(ComponentTypeRegistry? registry = null)" — the stale K3 signature
# this patch §1.2 replaces with "Run(bool useRegistry = true)".

# APV-3 — VanillaComponentRegistration.RegisterAll exists (Finding 3)
Test-Path src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs
# Expected: True. This is the helper the milestone reuses + extends.

# APV-4 — solution file location (Finding 4)
Test-Path DualFrontier.sln
Test-Path src/DualFrontier.sln
# Expected: True, False. The sln is at repo root. ALL `dotnet build` /
# `dotnet test` invocations in the combined brief that say
# `src/DualFrontier.sln` must use `DualFrontier.sln` (repo root) instead.

# APV-5 — factory K8.1-primitive structure (Finding 5)
Select-String -Path src/DualFrontier.Application/Scenario/RandomPawnFactory.cs `
  -Pattern 'CreateMap|CreateComposite|InternString' -SimpleMatch
# Expected: matches present — per-pawn K8.1-primitive allocation. The bulk-add
# pattern in combined brief §2.2 must account for this (see patch §4).

# APV-6 — ItemFactory does NOT currently take NativeWorld (Finding 5)
Select-String -Path src/DualFrontier.Application/Scenario/ItemFactory.cs `
  -Pattern 'NativeWorld' -SimpleMatch
# Expected: zero matches. ItemFactory's ctor must GAIN a NativeWorld parameter
# in Phase 2 (see patch §4) — combined brief §2.3 assumed it already had one.
```

If any APV check returns an unexpected result, the codebase has drifted since this patch was authored — **escalate per METHODOLOGY §3** before proceeding. Do not improvise around drift.

**§6.1 Step 4 amendment** — the component inventory step now explicitly cross-references `RegisterAll`'s current contents (patch §1.4 procedure) and produces the **`RegisterAll` extension set** as a named Phase 0 output, consumed by Phase 2 Commit 3.

---

## §3 Finding 1 + Finding 3 — combined brief §6.3 Phase 2 + §10 commit log — OVERRIDDEN

Combined brief §6.3 Phase 2 Commit 3 is reshaped to carry the `Bootstrap.Run` refactor and the `RegisterAll` extension. The 24-commit total is **unchanged** — the work fits within the existing Commit 3 + Commit 4 envelope.

### §3.1 — Commit 3 (revised) — `Bootstrap.Run` refactor + `RegisterAll` extension + factory wiring

**Files modified**:
- `src/DualFrontier.Core.Interop/Bootstrap.cs` — `Run` signature `Run(ComponentTypeRegistry? registry = null)` → `Run(bool useRegistry = true)`; registry constructed internally via the `internal` ctor; old overload deleted (patch §1.2).
- `src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs` — `RegisterAll` extended with the Phase 0 inventory delta (patch §1.4). New entries append to category blocks; existing order preserved (K-L4 id stability).
- `src/DualFrontier.Application/Loop/GameBootstrap.cs` — `CreateLoop` uses `Bootstrap.Run(useRegistry: true)` + `VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!)` (patch §1.3); the invented `RegisterProductionComponentTypes` helper is **not** added; embedded comment block updated to describe the dual-write transition phase (combined brief §6.3 wording, minus the registry-helper reference).
- `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs` — dual-write phase wiring (patch §4).
- `src/DualFrontier.Application/Scenario/ItemFactory.cs` — gains `NativeWorld` ctor parameter + dual-write phase wiring (patch §4).
- `tests/DualFrontier.Systems.Tests/Fixtures/NativeWorldTestFixture.cs` — NEW (patch §6).
- Any call site that passed a registry to `Bootstrap.Run` — migrate to `Run(useRegistry: true)` + `world.Registry` (Phase 0 APV-2 enumerates these; likely zero).

**Commit shape**:
```
feat(bootstrap): Bootstrap.Run constructs registry internally + reuse VanillaComponentRegistration (K8.4 storage Phase 2/2)

- Bootstrap.Run signature: Run(ComponentTypeRegistry?) → Run(bool useRegistry = true).
  Registry constructed inside Run against the bootstrapped handle via the
  internal ctor (same-assembly, legal) and bound to the returned NativeWorld.
  Resolves the chicken-and-egg gap: the registry ctor needs a world handle that
  only exists after bootstrap. Old overload deleted (dead K3 artifact, no
  reachable caller). K-L5 atomicity preserved.
- VanillaComponentRegistration.RegisterAll (K4-era helper) extended with the
  production component types not previously covered (Phase 0 inventory delta:
  Identity/Skills/Movement/Storage + any others surfaced). Single source of
  truth for production component type-id assignment — no parallel helper.
- GameBootstrap.CreateLoop: Bootstrap.Run(useRegistry: true) +
  RegisterAll(nativeWorld.Registry!). No managed World construction in the
  registry path; factories receive nativeWorld for dual-write transition.
- ItemFactory gains a NativeWorld ctor parameter (previously managed-World-only).
- NativeWorldTestFixture shared fixture for Systems.Tests, delegating to
  RegisterAll for component registration (no hand-maintained list).

Resolves combined-brief-refresh-patch Findings 1, 3, 4 (partial), 5 (partial).
Build green; tests pass. Production still reads via managed World legacy API
until per-system migration (Phase 4 commit 9).

Part of K8.3+K8.4 combined milestone Phase 2 commit 3 of 24.
```

### §3.2 — Commit 4 (revised) — registry verification tests

Combined brief §6.3 Commit 4 (`ProductionComponentRegistryTests`) is **unchanged in intent** but its assertions target `RegisterAll`, not the deleted `RegisterProductionComponentTypes`:

- Verify `Bootstrap.Run(useRegistry: true)` returns a `NativeWorld` with non-null `Registry`.
- Verify `Bootstrap.Run(useRegistry: false)` returns a `NativeWorld` with null `Registry` (FNV-1a fallback path still works).
- Verify `VanillaComponentRegistration.RegisterAll(world.Registry!)` registers the full expected production set (count assertion against the Phase 0 inventory).
- Verify registry idempotency (re-`RegisterAll` returns identical ids — `RegisterAll` is composed of idempotent `Register<T>` calls).
- Verify type-id determinism across two independent `Bootstrap.Run` + `RegisterAll` sequences (same order → same ids, K-L4).
- Verify every registered type satisfies the `unmanaged` constraint (compile-time guaranteed by `Register<T> where T : unmanaged`; runtime `Unsafe.SizeOf<T>` sanity check).

Commit message: `test(interop): registry activation + RegisterAll verification tests` — otherwise per combined brief §6.3 Commit 4.

### §3.3 — §10 commit log — no renumbering

The combined brief §10 commit log is **unchanged in numbering**. Commits 3 and 4 carry revised content per patch §3.1 + §3.2; all other commits (1-2, 5-24) are exactly as the combined brief specifies. Total remains 24 (+ optional 25 backfill).

---

## §4 Finding 5 — combined brief §2.2 + §2.3 factory mechanics — OVERRIDDEN

### §4.1 — The finding stated

Combined brief §2.2 shows a clean bulk-write `RandomPawnFactory.Spawn` that calls `nativeWorld.AddComponents<T>(entities, positions)` etc. — as if components were plain values. **Reality** (verified `RandomPawnFactory.cs` + `ItemFactory.cs`):

- `RandomPawnFactory` already holds a `NativeWorld _nativeWorld` field — but uses it **only** for K8.1 primitives, not as a storage backbone. Components are written to the managed `World world` via per-entity `world.AddComponent(...)`.
- Several production components **embed K8.1-primitive handles inside the struct**:
  - `IdentityComponent` carries `InternedString Name` — produced by `_nativeWorld.InternString(fullName)` **per pawn**.
  - `SkillsComponent` carries `NativeMap<SkillKind,int> Levels` + `NativeMap<SkillKind,float> Experience` — produced by `_nativeWorld.CreateMap<,>()` **per pawn**, then populated per-`SkillKind`.
  - `MovementComponent` carries `NativeComposite<GridVector> Path` — produced by `_nativeWorld.CreateComposite<GridVector>()` **per pawn**.
- `CreateMap` / `CreateComposite` allocate a fresh monotonic native id **per call** (`NativeWorld.AllocateMapId` etc.). They are intrinsically per-entity — each pawn's `SkillsComponent` owns distinct backing storage.
- `ItemFactory` currently has **no `NativeWorld`** in its constructor at all — pure managed `World`, four entity types, none of whose components embed K8.1 primitives.

So the combined brief's bulk-write shape is not merely under-specified — it is **incompatible with the real component structure** as written. Bulk `AddComponents<T>` requires a pre-built `T[]`; but the `T` values for `SkillsComponent` / `MovementComponent` / `IdentityComponent` cannot exist until their per-pawn K8.1 primitives are allocated and their ids written into each struct.

### §4.2 — Resolution: explicit two-phase factory pattern

The bulk-write optimization is **still valid** — it just has a mandatory ordering. The combined brief §2.2 "Factory bulk-write pattern" block is **replaced** by:

**`RandomPawnFactory.Spawn` — two-phase shape (dual-write transition, Phase 2–4)**:

```csharp
public IReadOnlyList<EntityId> Spawn(NativeWorld nativeWorld, World world,
                                     GameServices services, int count)
{
    // ... passable-tile pool + Fisher-Yates shuffle unchanged ...

    var entities = new EntityId[count];
    for (int i = 0; i < count; i++)
        entities[i] = nativeWorld.CreateEntity();

    // ── Phase A: per-entity K8.1-primitive allocation ──────────────────────
    // CreateMap / CreateComposite / InternString allocate fresh monotonic
    // native ids per call — intrinsically per-entity. Each component struct
    // captures the ids of the primitives it owns. This phase CANNOT be
    // bulked; the primitives must exist before the component array is built.
    var positions  = new PositionComponent[count];
    var identities = new IdentityComponent[count];
    var needs      = new NeedsComponent[count];
    var minds      = new MindComponent[count];
    var jobs       = new JobComponent[count];
    var skills     = new SkillsComponent[count];
    var movements  = new MovementComponent[count];

    for (int i = 0; i < count; i++)
    {
        positions[i] = new PositionComponent { Position = passable[i] };

        string fullName = $"{Forenames[_rng.Next(Forenames.Length)]} " +
                          $"{Surnames[_rng.Next(Surnames.Length)]}";
        identities[i] = new IdentityComponent
        {
            Name = nativeWorld.InternString(fullName)   // per-entity primitive
        };

        NativeMap<SkillKind, int>   levels     = nativeWorld.CreateMap<SkillKind, int>();
        NativeMap<SkillKind, float> experience = nativeWorld.CreateMap<SkillKind, float>();
        foreach (SkillKind kind in (SkillKind[])Enum.GetValues(typeof(SkillKind)))
        {
            levels.Set(kind, _rng.Next(0, SkillsComponent.MaxLevel + 1));
            experience.Set(kind, 0f);
        }
        skills[i] = new SkillsComponent { Levels = levels, Experience = experience };

        needs[i]     = new NeedsComponent { Satiety = 0.9f, Hydration = 0.9f,
                                            Sleep = 0.9f, Comfort = 1.0f };
        minds[i]     = new MindComponent();
        jobs[i]      = new JobComponent { Current = JobKind.Idle };
        movements[i] = new MovementComponent
        {
            Path = nativeWorld.CreateComposite<GridVector>()   // per-entity primitive
        };
    }

    // ── Phase B: bulk component add (one P/Invoke per type) ────────────────
    // The component arrays are now fully built — every embedded primitive id
    // is already populated. Bulk add is a pure value copy across the boundary.
    nativeWorld.AddComponents<PositionComponent>(entities, positions);
    nativeWorld.AddComponents<IdentityComponent>(entities, identities);
    nativeWorld.AddComponents<NeedsComponent>(entities, needs);
    nativeWorld.AddComponents<MindComponent>(entities, minds);
    nativeWorld.AddComponents<JobComponent>(entities, jobs);
    nativeWorld.AddComponents<SkillsComponent>(entities, skills);
    nativeWorld.AddComponents<MovementComponent>(entities, movements);

    // ── Dual-write to managed World during transition (Phase 2–4) ──────────
    // Removed in Phase 5 commit 21 (combined brief §2.3 + §6.6).
    for (int i = 0; i < count; i++)
    {
        world.AddComponent(entities[i], positions[i]);
        world.AddComponent(entities[i], identities[i]);
        world.AddComponent(entities[i], needs[i]);
        world.AddComponent(entities[i], minds[i]);
        world.AddComponent(entities[i], jobs[i]);
        world.AddComponent(entities[i], skills[i]);
        world.AddComponent(entities[i], movements[i]);
    }

    for (int i = 0; i < count; i++)
        services.Pawns.Publish(new PawnSpawnedEvent
            { PawnId = entities[i], X = passable[i].X, Y = passable[i].Y });

    return new List<EntityId>(entities);
}
```

**Key correction**: Phase A (per-entity primitive allocation) and Phase B (bulk add) are **distinct, ordered phases**. The combined brief showed only Phase B. The K8.1-primitive allocation is *not* a defect to "fix away" — it is intrinsic to how `SkillsComponent` / `MovementComponent` / `IdentityComponent` are structured, and it stays per-entity.

**Important — entity-id source**: in the two-phase shape, entities are created on `nativeWorld` (`nativeWorld.CreateEntity()`), and the **same** `EntityId` values are used for the managed-`World` dual-write (`world.AddComponent(entities[i], ...)`). This requires that `World.AddComponent` accepts an externally-minted `EntityId` rather than only ids from `World.CreateEntity()`. **Phase 0 verification task**: confirm `World.AddComponent(EntityId, T)` does not assert the id came from `World.CreateEntity()`. If it does assert, the dual-write loop instead calls `world.CreateEntity()` per entity and the factory keeps a `nativeId → managedId` map for the transition — record which path applies in the Commit 3 message. (This is a transition-only concern; Phase 5 commit 21 deletes the managed-side write entirely.)

### §4.3 — `ItemFactory` — gains `NativeWorld`, same two-phase shape

Combined brief §2.3 assumed `ItemFactory.Spawn(nativeWorld, world, ...)`. Reality: `ItemFactory`'s constructor has no `NativeWorld`. Resolution:

- `ItemFactory` constructor gains a `NativeWorld nativeWorld` parameter (Commit 3, patch §3.1).
- `ItemFactory.Spawn` gains a `NativeWorld` path. None of `ItemFactory`'s four component types (`ConsumableComponent`, `WaterSourceComponent`, `BedComponent`, `DecorativeAuraComponent`) embed K8.1 primitives — verified — so Phase A is trivial (plain value structs) and Phase B is a straight bulk add per type. The dual-write loop mirrors `RandomPawnFactory`.
- The four entity types are spawned in distinct count-loops today; the two-phase shape builds four small typed arrays (one per item type) then bulk-adds each. Entity-id-source concern from patch §4.2 applies identically.

### §4.4 — Phase 5 commit 21 — dual-write removal (combined brief §2.3 + §6.6 — clarification)

Combined brief §6.6 Commit 21 already specifies removing the managed-side dual-write. The clarification this patch adds: the **two-phase structure stays** at commit 21 — only the "Dual-write to managed World" loop is deleted. Phase A (per-entity primitive allocation) and Phase B (bulk add to `nativeWorld`) are the permanent post-cutover shape. `RandomPawnFactory.Spawn` and `ItemFactory.Spawn` signatures drop the `World world` parameter; they keep `NativeWorld nativeWorld`.

---

## §5 Finding 4 + Finding 1 confirmation — mechanical + no-op corrections

### §5.1 — Finding 4 — solution-file path (mechanical, repo-wide)

The combined brief uses `dotnet build src/DualFrontier.sln` and `dotnet test src/DualFrontier.sln` throughout §6 and §8. **The solution file is at the repository root: `DualFrontier.sln`.** There is no `src/DualFrontier.sln`.

**Override**: every `dotnet build src/DualFrontier.sln` → `dotnet build DualFrontier.sln`; every `dotnet test src/DualFrontier.sln` → `dotnet test DualFrontier.sln`. This applies to every Phase verification block in combined brief §6, every gate in §8, and the closure checks in §9. Purely mechanical; no semantic change.

Per-project `dotnet test tests/DualFrontier.Core.Tests` style invocations in the combined brief are **unaffected** — those paths are correct.

### §5.2 — Finding 1 confirmation — IModApi `unmanaged` constraint (combined brief §4.1 — no change, confirmation only)

Combined brief §4.1 prescribes `RegisterComponent<T> where T : unmanaged, IComponent`. Verified: `ComponentTypeRegistry.Register<T>` **already** has `where T : unmanaged`, and `NativeWorld`'s component methods already require `where T : unmanaged`. The combined brief's v3 constraint is **consistent with the existing kernel surface** — §4.1 needs no change. This is recorded only to close the loop: Finding 1's blast radius is the *constructor*, not the *generic constraints*, which were correct.

---

## §6 Finding 3 — combined brief §7.2 `NativeWorldTestFixture` — OVERRIDDEN

Combined brief §7.2 specifies a `NativeWorldTestFixture` with a **hand-maintained** `Registry.Register<T>()` list duplicating the production component set. That hand-maintained list is a second copy of `RegisterAll` — the same duplication Finding 3 rejects.

**Override**: the fixture delegates to `VanillaComponentRegistration.RegisterAll` — single source of truth.

```csharp
public sealed class NativeWorldTestFixture : IDisposable
{
    public NativeWorld NativeWorld { get; }
    public ComponentTypeRegistry Registry { get; }
    public GameServices Services { get; }

    public NativeWorldTestFixture()
    {
        // Same path production uses — Bootstrap.Run builds + binds the registry.
        NativeWorld = Bootstrap.Run(useRegistry: true);
        Registry = NativeWorld.Registry!;

        // Single source of truth — the SAME helper production calls. No
        // hand-maintained component list to drift against production.
        VanillaComponentRegistration.RegisterAll(Registry);

        Services = new GameServices();
    }

    public void Dispose() => NativeWorld.Dispose();
}
```

Rationale: a hand-maintained fixture list silently drifts from `RegisterAll` the first time a component is added to production. Delegating means the fixture is correct by construction. The combined brief §7.2's explicit `Register<T>()` enumeration is **deleted**; the delegation above replaces it. Combined brief §7.1, §7.3–§7.6 are unaffected.

---

## §7 Meta-correction — the combined brief's "zero halts" claim

Combined brief §12 and §6.1 assert the brief "expects ~0 halts during execution because the K8.3 v2.0 halt resolution lessons are encoded in Phase 0 protocol."

**This claim was partially falsified.** The combined brief encoded the lessons from CAPA-2026-05-13 (storage-location premise) — and that class of miss did not recur. But it introduced a *different* class — API-surface premise (constructor shape, helper existence, sln path, factory structure) — and that class was not prevented, because the brief was authored from architecture documents and prior-session notes rather than from a fresh read of the actual API surface. The prior session's Phase 0 notes even recorded "K2 era registry ready" without transcribing the constructor signature.

**The correct claim** — and the one this patch substitutes — is: *"the brief expects ~0 **commits before a halt** if a premise miss exists; Phase 0 deep-reads + API-surface verification (patch §2) catch premise misses before any source commit lands."* That is exactly what happened: the prior session halted at Phase 0 with a clean working tree and zero source commits. The system worked as designed. A brief cannot promise the absence of authoring misses; it can promise they are caught early. The combined brief over-claimed; this patch right-sizes the claim.

This correction is recorded in CAPA-2026-05-14 (patch §8) and is a METHODOLOGY §K-Lessons candidate (patch §10).

---

## §8 Finding 1-5 — combined brief §9 closure protocol — OVERRIDDEN (one addition)

Combined brief §9 closure protocol is **structurally unchanged** — all lifecycle transitions, version bumps, REQ promotions, the two existing CAPA closures, and the audit-trail entry hold. This patch adds **one CAPA** to the closure set.

### §8.1 — New entry: this patch's REGISTER enrollment

`DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH` enrolls in REGISTER.yaml at Commit 1 (alongside the combined brief and HALT_REPORT). At Phase 6 closure its lifecycle transitions `AUTHORED → EXECUTED` — it is consumed as an override layer during execution, same pattern as `DOC-D-K8_3_BRIEF_REFRESH_PATCH`.

Combined brief §9.1 lifecycle-transition block gains one EXECUTED transition:
```yaml
- id: DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH
  category: D
  tier: 3
  lifecycle: EXECUTED                       # ← was AUTHORED
  owner: Crystalka
  version: "1.0"
  companion_to: DOC-D-K8_34_COMBINED
  authored_at: "2026-05-14"
  executed_at: "2026-XX-XX"
  register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH
```

### §8.2 — New CAPA: CAPA-2026-05-14-K8.34-API-SURFACE-MISS

Added to combined brief §9.4 CAPA-closure set (Phase 6 commit 24). Opened at this patch's authoring; closes at the **same** combined-milestone closure (it is resolved by this patch, and the closure verifies the resolution executed cleanly):

```yaml
- id: CAPA-2026-05-14-K8.34-API-SURFACE-MISS
  opened_date: "2026-05-14"
  status: CLOSED                                  # at combined milestone closure
  closed_at: "2026-XX-XX"
  closed_commit: "<commit-24-sha>"
  trigger: |
    Combined K8.3+K8.4 brief v1.0 authoring (2026-05-13) prescribed
    `new ComponentTypeRegistry()` (no such ctor — only internal(IntPtr)),
    invented a `RegisterProductionComponentTypes` helper duplicating the
    existing K4-era `VanillaComponentRegistration.RegisterAll`, used a
    non-existent `src/DualFrontier.sln` path, and showed a factory bulk-write
    shape incompatible with the real per-entity K8.1-primitive structure of
    SkillsComponent / MovementComponent / IdentityComponent. Claude Code
    execution attempt 2026-05-14 surfaced all five at Phase 0 — zero source
    commits, clean working tree.
  affected_documents:
    - DOC-D-K8_34_COMBINED
    - DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH
  root_cause: |
    Same class as CAPA-2026-05-13-K8.3-PREMISE-MISS: brief authored from
    architecture documents + prior-session notes rather than from a fresh
    read of the actual API surface. The prior session's Phase 0 notes
    recorded "K2 era registry ready" without transcribing the constructor
    signature; §2.1 was then written against an assumed `new Registry()`
    surface. The combined brief's own claim that atomicity "structurally
    prevents premise misses" was over-scoped — atomicity prevents the
    storage-LOCATION class, not the API-SURFACE class.
  immediate_action: |
    Prior Claude Code session halted at Phase 0 per combined brief §8.3 AS-C1
    + METHODOLOGY §3. HALT_REPORT authored at
    docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT.md. Zero source commits.
  corrective_action: |
    (1) This patch authored as override layer (combined brief preserved as-is).
    (2) Finding 1 — Bootstrap.Run refactored to construct the registry
        internally (Run(bool useRegistry); internal ctor reachable same-assembly);
        stale K3 overload deleted. Combined brief §2.1 replacement text supplied.
    (3) Finding 3 — invented RegisterProductionComponentTypes deleted;
        VanillaComponentRegistration.RegisterAll reused + extended with the
        Phase 0 inventory delta. Test fixture delegates to RegisterAll.
    (4) Finding 4 — all `dotnet build src/DualFrontier.sln` → `DualFrontier.sln`
        (repo root).
    (5) Finding 5 — factory mechanics rewritten as explicit two-phase
        (per-entity K8.1-primitive allocation, then bulk add); ItemFactory
        gains a NativeWorld ctor parameter.
    (6) Phase 0 gains explicit API-surface verification steps (patch §2,
        APV-1..APV-6) that would have caught all five findings.
    (7) Brief-authoring protocol: a brief that prescribes a constructor /
        helper / file-path MUST transcribe the actual signature verbatim into
        the brief at authoring time, not paraphrase from memory. Lesson added
        to METHODOLOGY §K-Lessons at A'.6 K8.5 closure batch revision (joins
        Lesson #6).
  effectiveness_verification:
    method: "Combined milestone executes to closure with the five overrides applied; no further API-surface premise miss surfaces"
    date_verified: "2026-XX-XX"
    verification_commit: "<commit-24-sha>"
    verification_outcome: "Populated at closure — combined milestone reached Phase 6 with patch §1-§6 overrides applied; Phase 0 APV checks passed; no further halt"
  lessons_learned_reference: DOC-B-METHODOLOGY
```

### §8.3 — Combined brief §9.5 audit-trail entry — one cross-reference added

The `EVT-...-K8.3-K8.4-COMBINED-CLOSURE` audit entry's `cross_references.capa_entries` gains:
```yaml
      - CAPA-2026-05-14-K8.34-API-SURFACE-MISS (CLOSED via this closure)
```
and `documents_affected` gains `DOC-D-K8_34_COMBINED_BRIEF_REFRESH_PATCH`.

No other §9 change. The two pre-existing CAPA closures (2026-05-12 count-drift, 2026-05-13 premise-miss) and all six REQ promotions are **unaffected**.

---

## §9 What this patch does NOT do

- **Does not amend the combined brief on disk.** The combined brief v1.0 is preserved as authored. This patch is the override layer; the pair is the execution input. Same precedent as `K8_3_BRIEF_REFRESH_PATCH.md` and `K9_BRIEF_REFRESH_PATCH.md`.
- **Does not change any Q-COMBINED lock.** All 8 architectural locks hold verbatim. Findings 1-5 are API-mechanics corrections beneath the architecture layer.
- **Does not change the 6-phase structure or the 24-commit count.** Commit 3 absorbs the `Bootstrap.Run` refactor + `RegisterAll` extension; Commit 4's assertions retarget; all other commits are exactly as the combined brief specifies. Optional Commit 25 (backfill) unchanged.
- **Does not change the system-layer design (combined brief §3).** SystemBase deletions, SystemExecutionContext refactor, per-system migration patterns, Tier 1-5 ordering — all hold.
- **Does not change the Mod API v3 surface (combined brief §4.2-§4.6).** ManagedStore / IManagedStorageResolver / [ManagedStorage] / ValidationErrorKind — all hold. Only §4.1 gets a no-op confirmation note (patch §5.2).
- **Does not change the manifest schema work (combined brief §5).** manifestVersion strict-v3 gate holds.
- **Does not re-author or decompose the milestone.** Disposition is patch-as-override (Crystalka 2026-05-14). The combined milestone stays a single atomic A'.5.
- **Does not predict whether further premise misses exist.** If Phase 0 APV checks (patch §2) or any later phase surfaces a sixth finding, halt per METHODOLOGY §3 and author a HALT_REPORT — do not improvise. The expectation is that APV-1..APV-6 plus the combined brief's existing Phase 0 deep-reads now cover the API surface this milestone touches.

---

## §10 Lesson encoded for future brief authoring

Extends the CAPA-2026-05-13 lesson set (combined brief §11.3 references Lessons #1-#5; Lesson #6 was added by the combined milestone). This patch adds **Lesson #7**, deferred to METHODOLOGY §K-Lessons batch revision at A'.6 K8.5 closure alongside Lesson #6:

**Lesson #7 — A brief that prescribes an API must transcribe the API, not paraphrase it.** When a brief tells the executor to call a constructor, a helper, or a file path, the brief author must **open the actual source file and copy the real signature into the brief** at authoring time. "K2 era registry ready" is a note, not a signature. The CAPA-2026-05-13 lesson ("read entry-point files in full") addressed *transitional-state comments*; Lesson #7 addresses *API surface* — the constructor shape, the helper's existence and parameters, the solution-file location, the real structure of the data being manipulated. The two lessons share a root: a brief is a contract for mechanical execution, and a contract cannot reference an interface it has not read.

Corollary — **a brief cannot promise "zero halts"; it can promise "halts before commits."** The combined brief over-claimed structural prevention of premise misses. The honest, achievable guarantee is that Phase 0 deep-reads + API-surface verification catch premise misses before any source commit. The halt that produced this patch *is the guarantee working* — clean tree, zero commits, escalation to deliberation. That is success, not failure.

---

## §11 Brief authoring lineage (this patch appends to combined brief §11.4)

```
2026-05-13  Combined K8.3+K8.4 brief v1.0 authored (Opus deliberation session);
            8 Q-COMBINED locks ratified; ~3100 lines; clean-break direction encoded
2026-05-13  Combined brief committed-intent: "Commit 1 = brief authoring commit"
2026-05-14  Claude Code session opened against combined brief; Phase 0 deep-reads
            surfaced five API-surface / helper-inventory / build-path findings;
            halted at Phase 0 per combined brief §8.3 AS-C1 + METHODOLOGY §3;
            zero source commits; clean working tree; HALT_REPORT authored at
            docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT.md
2026-05-14  Crystalka direction: patch brief as override layer, send to the
            open execution session («да я сразу отправлю новый патч бриф»);
            Finding 3 → reuse RegisterAll; Finding 1 → Opus reads kernel locally
            for a clean solution
2026-05-14  Opus local deep-reads: ComponentTypeRegistry.cs, NativeWorld.cs,
            Bootstrap.cs, VanillaComponentRegistration.cs, ComponentTypeRegistryTests.cs,
            RandomPawnFactory.cs, ItemFactory.cs — Finding 1 clean solution found
            (Bootstrap.Run constructs registry internally; same-assembly internal
            ctor; stale K3 overload deleted)
2026-05-14  This patch authored as resolution artifact
2026-XX-XX  Claude Code session: Commit 1 = combined brief + this patch +
            HALT_REPORT; milestone continues at combined brief §6.2 Phase 1 with
            patch §1-§6 overrides applied
2026-XX-XX  Combined milestone execution closure (planned, ~10-14 hours auto-mode);
            CAPA-2026-05-14-K8.34-API-SURFACE-MISS closes alongside the two
            pre-existing CAPAs
```

---

## §12 Quick-reference — the five findings and their resolutions

| # | Finding | Combined brief location | Resolution | Patch § |
|---|---|---|---|---|
| 1 | `new ComponentTypeRegistry()` — no such ctor (only `internal(IntPtr)`); `Application` can't reach it; chicken-and-egg with the world handle | §2.1 | `Bootstrap.Run(bool useRegistry = true)` constructs the registry internally against the bootstrapped handle (same-assembly `internal` ctor); stale K3 overload deleted | §1, §3 |
| 2 | (subsumed by Finding 1 — the chicken-and-egg is the same root) | §2.1 | Resolved by Finding 1's `Bootstrap.Run` refactor | §1 |
| 3 | Invented `RegisterProductionComponentTypes` duplicates the existing K4-era `VanillaComponentRegistration.RegisterAll` | §2.1, §7.2 | Delete the invented helper; reuse + extend `RegisterAll`; test fixture delegates to it | §1, §6 |
| 4 | `dotnet build src/DualFrontier.sln` — the sln is at the repo root | §6, §8, §9 (all build invocations) | `dotnet build DualFrontier.sln` (mechanical, repo-wide) | §5.1 |
| 5 | Factory bulk-write shape incompatible with real per-entity K8.1-primitive structure; `ItemFactory` has no `NativeWorld` ctor param | §2.2, §2.3 | Explicit two-phase factory pattern (per-entity primitive allocation, then bulk add); `ItemFactory` gains a `NativeWorld` ctor parameter | §4 |

Architectural note: Findings 1-5 are **all** beneath the architecture layer. Every Q-COMBINED lock, the 6-phase structure, the 24-commit shape, the system-layer design, the Mod API v3 surface, the manifest schema work, and the closure protocol are **unchanged**. This patch corrects how the kernel is *called*, not what the milestone *is*.

---

**Patch end. Read the combined brief next, with §1-§8 overrides applied. Execution begins at combined brief §6.1 Phase 0, augmented by patch §2 APV checks.**
