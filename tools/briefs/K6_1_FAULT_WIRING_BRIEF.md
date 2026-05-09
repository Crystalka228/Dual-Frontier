# K6.1 — Mod fault wiring end-to-end (Full Brief)

**Status**: AUTHORED — implementation brief, executes against K6 closure state to wire `ModFaultHandler` end-to-end through scheduler and `SystemExecutionContext`
**Reference**: `docs/MIGRATION_PROGRESS.md` K6 closure section "Out of K6 scope (deferred)" + K6 lessons learned
**Specification source**: `docs/MOD_OS_ARCHITECTURE.md` v1.6 §10.3 (architectural threats — caught), §3.6 (hybrid enforcement); `docs/KERNEL_ARCHITECTURE.md` v1.1 §K6 (deliverables superseded by K6.1 closure)
**Companion**: `docs/MIGRATION_PROGRESS.md` (live tracker — K6.1 row added on closure)
**Methodology lineage**: `tools/briefs/K6_MOD_REBUILD_BRIEF.md` (closure-shaped brief format), `tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md` (Anthropic `Edit` literal-mode semantics, atomic commit discipline)

---

## Executable contract notice

This brief is a deterministic instruction set for a Claude Code execution session. K6.1 closes the wiring gap explicitly flagged in K6 closure: `ModFaultHandler` exists as infrastructure but `SystemExecutionContext` still uses `NullModFaultSink`, and `ParallelSystemScheduler.BuildContext` hardcodes `SystemOrigin.Core` / `modId: null` for every system regardless of origin. K6.1 makes the fault path active end-to-end.

The executor reads this entire brief before any tool call. Anthropic `Edit` tool semantics assumed (literal string matching, not regex). When the executor encounters an underspecified situation, the stop condition is "halt and escalate", not "improvise".

Time estimate: **3-5 days at hobby pace (~1h/day)**. Auto-mode estimate: **3-5 hours wall time**.

Scope follows "no compromises" — the bootstrap order is restructured properly (ModFaultHandler created before scheduler), no setter-after-construction pattern, no lazy proxy indirection. The scheduler becomes immutable at the fault-sink position; ownership is inverted at the bootstrap layer where it belongs.

The brief introduces a small but architecturally meaningful change: `SystemRegistration` becomes the unit the scheduler consumes (not raw `SystemBase`), enabling per-system origin/modId propagation through `BuildContext`. This unlocks accurate `RouteAndThrow` behavior at runtime — the prerequisite for fault routing to actually disable misbehaving mods rather than crash the host.

---

## Phase 0 — Pre-flight verification

Before any edit, the executor verifies the working tree state, prerequisite milestones, and the assumptions this brief makes about the code state.

### 0.1 — Working tree clean

```
git status
```

**Expected output**: `nothing to commit, working tree clean` on branch `main` or `feat/k6-1-fault-wiring`.

**Halt condition**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K6-1-WIP"` and re-verify, or commit them on the current branch before starting K6.1 work.

### 0.2 — Prerequisite milestones closed

```
git log --oneline -30
```

**Expected**: K6 closure commits visible (`cb3d6cf`, `ab581cb`, `30b982b`, `a6664cf`, `208e9e7`, `4999926`, `af2b572`, `d438222`). The closure record `d438222` should be the most recent commit affecting MIGRATION_PROGRESS.md.

**Halt condition**: K6 not closed. K6.1 builds atop K6 deliverables; without K6, the `ModFaultHandler` class does not exist and this brief cannot run.

### 0.3 — Prerequisite documents at expected versions

```
head -10 docs/KERNEL_ARCHITECTURE.md
head -10 docs/MOD_OS_ARCHITECTURE.md
head -10 docs/MIGRATION_PROGRESS.md
```

**Expected**:

- `KERNEL_ARCHITECTURE.md` Status: AUTHORITATIVE LOCKED v1.1 (post-K6 amendment)
- `MOD_OS_ARCHITECTURE.md` Status: LOCKED v1.6
- `MIGRATION_PROGRESS.md` Last updated: 2026-05-09 (K6 closure)

**Halt condition**: any spec at unexpected version. K6.1 assumes K6 amendments landed; mismatch means either K6 was reverted or a different working tree.

### 0.4 — Code state inventory (gap detection)

The executor performs the following verification reads to confirm the gaps K6.1 closes are still present (i.e., not closed by some intervening change).

**Inventory checks**:

| K6.1 gap | Expected file | Expected state | Verify |
|---|---|---|---|
| Scheduler defaults to NullModFaultSink | `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` | ctor parameter `IModFaultSink? faultSink = null`, fallback `_faultSink = faultSink ?? new NullModFaultSink();` | `grep -n "faultSink ?? new NullModFaultSink" <file>` returns 1 match |
| Bootstrap passes faultSink: null | `src/DualFrontier.Application/Loop/GameBootstrap.cs` | `new ParallelSystemScheduler(... faultSink: null, ...)` | `grep -n "faultSink: null" <file>` returns 1 match |
| BuildContext hardcodes SystemOrigin.Core | `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` | `SystemOrigin.Core, modId: null` literal in BuildContext body | `grep -n "SystemOrigin.Core,.*modId: null" <file>` returns 1 match |
| ModFaultHandler exists from K6 | `src/DualFrontier.Application/Modding/ModFaultHandler.cs` | file exists with class definition | `ls <file>` succeeds |
| ModFaultHandler ctor takes pipeline | same | `public ModFaultHandler(ModIntegrationPipeline pipeline)` | `grep -n "public ModFaultHandler.ModIntegrationPipeline" <file>` returns 1 match |
| SystemRegistration carries Origin + ModId | `src/DualFrontier.Application/Modding/SystemRegistration.cs` | record with `SystemOrigin Origin, string? ModId` | `grep -n "SystemOrigin Origin\|string? ModId" <file>` returns 2 matches |
| ModRegistry exposes registrations | `src/DualFrontier.Application/Modding/ModRegistry.cs` | `GetAllSystems()` returns `IReadOnlyList<SystemRegistration>` | `grep -n "IReadOnlyList<SystemRegistration> GetAllSystems" <file>` returns 1 match |

**Halt condition**: any inventory check fails. K6.1 assumes the precise K6 closure state; if the state has shifted (e.g., someone fixed `BuildContext` already), the brief overlaps with that work and must be re-authored.

### 0.5 — Managed build clean

```
dotnet build
```

**Expected**: build succeeds without warnings or errors.

**Halt condition**: build failure on baseline. K6.1 starts from a known-good managed state.

### 0.6 — Managed test baseline

```
dotnet test
```

**Expected**: 547 tests passing (post-K6; K6.1 expects this exact count as the verification baseline).

**Halt condition**: count differs by more than 5 (allows for minor adjustments since K6 closure). If 0 tests pass or count is wildly different, halt.

---

## Phase 1 — Architectural design (read-only, no edits)

Before any code change, the executor reads this section as the design contract for K6.1 implementation. The decisions here are LOCKED by the brief author; the executor implements per these decisions, no improvisation.

### 1.1 — Ownership inversion: ModFaultHandler created before scheduler

**Current state** (K6 closure):
```
GameBootstrap.CreateLoop:
  scheduler = new ParallelSystemScheduler(... faultSink: null ...)  // null sink
  modLoader, modRegistry, validator, contractStore = ...
  pipeline = new ModIntegrationPipeline(... scheduler ...)
    → pipeline ctor: _faultHandler = new ModFaultHandler(this)     // owned by pipeline
    → pipeline ctor: _loader.SetFaultHandler(_faultHandler)        // wired into loader only
```

**K6.1 target state**:
```
GameBootstrap.CreateLoop:
  modLoader, modRegistry = ...
  faultHandler = new ModFaultHandler(...)                          // created FIRST
  scheduler = new ParallelSystemScheduler(... faultSink: faultHandler ...)  // immutable real sink
  validator, contractStore = ...
  pipeline = new ModIntegrationPipeline(..., faultHandler, ...)    // pipeline RECEIVES handler
    → pipeline ctor: _loader.SetFaultHandler(_faultHandler)        // existing wiring preserved
```

**Decision rationale**:

1. **No setter-after-construction**: scheduler's `_faultSink` field stays `readonly`. Immutability at construction time is the correct shape for a long-horizon project; mutable setters invite race conditions and "is the sink wired yet?" questions across the lifecycle.

2. **No lazy proxy**: a `Func<IModFaultSink>`-based proxy resolved on first ReportFault would add indirection cost on every fault report and obscure the call graph. Faults are rare but their handling path must be debuggable; a direct reference is unambiguous.

3. **Ownership inversion is the structural fix**: `ModFaultHandler` does not need `ModIntegrationPipeline` in its ctor — it needs to *report to* the pipeline at drain time, but that's a one-way dependency. The K6 implementation captured the pipeline reference in `ModFaultHandler` ctor for symmetry; K6.1 changes this to "handler is independent; pipeline queries handler at Apply time."

### 1.2 — ModFaultHandler ctor change

**Current** (K6):
```csharp
internal sealed class ModFaultHandler : IModFaultSink
{
    private readonly ModIntegrationPipeline _pipeline;
    private readonly object _lock = new();
    private readonly HashSet<string> _faultedMods = new(StringComparer.Ordinal);

    public ModFaultHandler(ModIntegrationPipeline pipeline)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }
    // ...
}
```

**K6.1 target**:
```csharp
internal sealed class ModFaultHandler : IModFaultSink
{
    private readonly object _lock = new();
    private readonly HashSet<string> _faultedMods = new(StringComparer.Ordinal);

    public ModFaultHandler()
    {
        // No dependencies. Handler is a self-contained fault accumulator;
        // consumers (ModIntegrationPipeline.Apply, ModLoader.HandleModFault)
        // query GetFaultedMods / ClearFault on demand. Ownership lives in
        // GameBootstrap as a top-level singleton across the session.
    }
    // ... (rest unchanged: ReportFault, GetFaultedMods, ClearFault)
}
```

The `_pipeline` field is removed. Every consumer (pipeline at Apply time, loader at HandleModFault) holds a reference to the handler externally, not the handler holding a reference to them. This is the inversion.

### 1.3 — ParallelSystemScheduler ctor signature change

**Current** (K6):
```csharp
public ParallelSystemScheduler(
    IReadOnlyList<SystemPhase> phases,
    TickScheduler ticks,
    World world,
    IModFaultSink? faultSink = null,
    IGameServices? services = null)
```

**K6.1 target**:
```csharp
public ParallelSystemScheduler(
    IReadOnlyList<SystemPhase> phases,
    TickScheduler ticks,
    World world,
    IReadOnlyDictionary<SystemBase, SystemRegistration> registrationLookup,
    IModFaultSink faultSink,
    IGameServices? services = null)
```

Two changes:

1. **`faultSink` is now non-optional**. Callers must supply a sink. Tests that don't care about fault routing pass `new NullModFaultSink()` explicitly — the silent default is removed because it masks bugs (the K6 closure gap is exactly this kind of bug).

2. **New parameter `registrationLookup`**: a per-system map from `SystemBase` → `SystemRegistration`. The scheduler uses this in `BuildContext` to read each system's actual `Origin` and `ModId` instead of hardcoding `Core`/`null`.

The lookup is read-only; the scheduler does not mutate it. `Rebuild` accepts a fresh lookup parallel to the new phases (see §1.5).

Note: `SystemRegistration` lives in `DualFrontier.Application.Modding` (internal). To pass it to `DualFrontier.Core.Scheduling`, one of the following is needed:

- **Option a**: Move `SystemRegistration` to `DualFrontier.Core.Scheduling` (or a new `DualFrontier.Core.ECS.SystemMetadata` shared spot).
- **Option b**: Introduce a `Core`-side abstraction (e.g., `ISystemMetadata { SystemOrigin Origin; string? ModId; }`) that `SystemRegistration` implements, and the scheduler accepts `IReadOnlyDictionary<SystemBase, ISystemMetadata>`.
- **Option c**: Define a new `Core`-side record `SystemMetadataEntry(SystemOrigin Origin, string? ModId)` and have `Application.Modding` build the dictionary by projecting from `SystemRegistration`.

**LOCKED choice: Option c**. Rationale:
- Option a couples Application-only concept (mod registration) into Core, violating layer discipline.
- Option b adds an interface for two fields, overkill.
- Option c keeps `SystemRegistration` (with its richer Application-only context) where it lives and exposes a minimal Core-side projection. The projection happens at one site (the bootstrap or the pipeline before calling Rebuild) and is cheap.

**Core-side new file**: `src/DualFrontier.Core/Scheduling/SystemMetadata.cs`:

```csharp
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Per-system metadata the scheduler needs to construct an
/// <see cref="DualFrontier.Core.ECS.SystemExecutionContext"/> with the
/// correct <see cref="DualFrontier.Core.ECS.SystemOrigin"/> and modId.
/// Application-side <c>SystemRegistration</c> projects to this record at
/// bootstrap time; the scheduler stays in Core and does not depend on the
/// modding layer.
/// </summary>
/// <param name="Origin">Provenance of the system, drives fault routing.</param>
/// <param name="ModId">Owning mod id when <paramref name="Origin"/> is <see cref="SystemOrigin.Mod"/>; otherwise null.</param>
public sealed record SystemMetadata(SystemOrigin Origin, string? ModId);
```

The scheduler ctor becomes:

```csharp
public ParallelSystemScheduler(
    IReadOnlyList<SystemPhase> phases,
    TickScheduler ticks,
    World world,
    IReadOnlyDictionary<SystemBase, SystemMetadata> systemMetadata,
    IModFaultSink faultSink,
    IGameServices? services = null)
```

### 1.4 — BuildContext fix

**Current** (K6 closure):
```csharp
private SystemExecutionContext BuildContext(SystemBase system)
{
    Type systemType = system.GetType();
    SystemAccessAttribute attr = systemType.GetCustomAttribute<SystemAccessAttribute>(inherit: false)
        ?? throw new InvalidOperationException(...);

    return new SystemExecutionContext(
        _world,
        systemType.FullName ?? systemType.Name,
        attr.Reads,
        attr.Writes,
        attr.Buses,
        SystemOrigin.Core,        // ← always Core, even for mod systems
        modId: null,              // ← always null
        _faultSink,
        _services);
}
```

**K6.1 target**:
```csharp
private SystemExecutionContext BuildContext(SystemBase system)
{
    Type systemType = system.GetType();
    SystemAccessAttribute attr = systemType.GetCustomAttribute<SystemAccessAttribute>(inherit: false)
        ?? throw new InvalidOperationException(...);

    // K6.1 — read origin and modId from the metadata table provided at
    // construction. Systems not present in the table default to Core/null
    // (covers core systems registered via local arrays in tests, and any
    // future system path that doesn't go through ModRegistry).
    SystemOrigin origin = SystemOrigin.Core;
    string? modId = null;
    if (_systemMetadata.TryGetValue(system, out SystemMetadata? meta))
    {
        origin = meta.Origin;
        modId = meta.ModId;
    }

    return new SystemExecutionContext(
        _world,
        systemType.FullName ?? systemType.Name,
        attr.Reads,
        attr.Writes,
        attr.Buses,
        origin,
        modId,
        _faultSink,
        _services);
}
```

Field added: `private IReadOnlyDictionary<SystemBase, SystemMetadata> _systemMetadata;` (mutable so `Rebuild` can swap it).

Default fallback to `Core/null` is intentional: tests that construct the scheduler directly with raw `SystemBase[]` (without going through `ModRegistry`) continue to work. The fault routing path activates only for systems registered with explicit metadata — which means mod systems via `ModRegistry`, the only path that matters in production.

### 1.5 — Rebuild ctor signature change

**Current** (K6):
```csharp
internal void Rebuild(IReadOnlyList<SystemPhase> newPhases)
```

**K6.1 target**:
```csharp
internal void Rebuild(
    IReadOnlyList<SystemPhase> newPhases,
    IReadOnlyDictionary<SystemBase, SystemMetadata> newSystemMetadata)
```

Same atomicity contract: both fields swap in lockstep after the new context cache is built.

### 1.6 — Bootstrap order change in GameBootstrap

**Current** (K6 closure relevant section):
```csharp
var graph = new DependencyGraph();
foreach (SystemBase s in coreSystems)
    graph.AddSystem(s);
graph.Build();

var scheduler = new ParallelSystemScheduler(
    graph.GetPhases(),
    ticks,
    world,
    faultSink: null,
    services:  services);

var modLoader = new ModLoader();
var modRegistry = new ModRegistry();
modRegistry.SetCoreSystems(coreSystems);
var modValidator = new ContractValidator();
var modContractStore = new ModContractStore();
var pipeline = new ModIntegrationPipeline(
    modLoader, modRegistry, modValidator, modContractStore, services, scheduler);
```

**K6.1 target**:
```csharp
var graph = new DependencyGraph();
foreach (SystemBase s in coreSystems)
    graph.AddSystem(s);
graph.Build();

// K6.1 — modding stack is constructed before the scheduler so the
// fault handler can be wired into the scheduler ctor as an immutable
// reference. ModRegistry knows core systems already so the metadata
// lookup is correct from tick 0.
var modLoader = new ModLoader();
var modRegistry = new ModRegistry();
modRegistry.SetCoreSystems(coreSystems);
var faultHandler = new ModFaultHandler();
modLoader.SetFaultHandler(faultHandler);

// Build initial metadata lookup from ModRegistry's current registrations.
// At bootstrap time, only core systems are registered — mod systems
// arrive later through Apply, which calls scheduler.Rebuild with an
// updated lookup.
var initialMetadata = BuildSystemMetadata(modRegistry);

var scheduler = new ParallelSystemScheduler(
    graph.GetPhases(),
    ticks,
    world,
    initialMetadata,
    faultHandler,
    services);

var modValidator = new ContractValidator();
var modContractStore = new ModContractStore();
var pipeline = new ModIntegrationPipeline(
    modLoader, modRegistry, modValidator, modContractStore, services, scheduler, faultHandler);
```

New helper at the bottom of `GameBootstrap`:
```csharp
private static IReadOnlyDictionary<SystemBase, SystemMetadata> BuildSystemMetadata(ModRegistry registry)
{
    var lookup = new Dictionary<SystemBase, SystemMetadata>();
    foreach (SystemRegistration reg in registry.GetAllSystems())
    {
        lookup[reg.Instance] = new SystemMetadata(reg.Origin, reg.ModId);
    }
    return lookup;
}
```

This helper is called both at bootstrap (initial state, only core systems) and at every successful `ModIntegrationPipeline.Apply` (post-mod-load, includes mod systems). The pipeline needs access to the same helper; it lives in `Application.Modding` namespace, available from both call sites.

**LOCKED placement decision**: helper moves to a new static file `src/DualFrontier.Application/Modding/SystemMetadataBuilder.cs`. `GameBootstrap` imports it. `ModIntegrationPipeline` imports it. Single source of truth.

### 1.7 — ModIntegrationPipeline ctor signature change

**Current** (K6):
```csharp
public ModIntegrationPipeline(
    ModLoader loader,
    ModRegistry registry,
    ContractValidator validator,
    IModContractStore contractStore,
    IGameServices services,
    ParallelSystemScheduler scheduler)
{
    // ...
    _faultHandler = new ModFaultHandler(this);
    _loader.SetFaultHandler(_faultHandler);
}
```

**K6.1 target**:
```csharp
public ModIntegrationPipeline(
    ModLoader loader,
    ModRegistry registry,
    ContractValidator validator,
    IModContractStore contractStore,
    IGameServices services,
    ParallelSystemScheduler scheduler,
    ModFaultHandler faultHandler)
{
    _loader = loader ?? throw new ArgumentNullException(nameof(loader));
    _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _contractStore = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
    _services = services ?? throw new ArgumentNullException(nameof(services));
    _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    _faultHandler = faultHandler ?? throw new ArgumentNullException(nameof(faultHandler));
    // SetFaultHandler call removed — bootstrap wires loader-side directly,
    // so the pipeline only needs to query the handler in Apply.
}
```

`_faultHandler` field stays `readonly`. The pipeline does not own the handler; it has a reference to query at Apply time.

### 1.8 — Apply rewrite: scheduler.Rebuild signature change propagates

**Current** (K6) Apply step [8]:
```csharp
_scheduler.Rebuild(localGraph.GetPhases());
_activeMods.AddRange(loaded);
_activeShared.AddRange(sharedLoaded);
```

**K6.1 target**:
```csharp
_activeMods.AddRange(loaded);
_activeShared.AddRange(sharedLoaded);
// K6.1 — metadata lookup must reflect the post-Apply registry state.
// _activeMods.AddRange must run BEFORE BuildSystemMetadata so the
// mod systems registered during step [4] are visible.
var newMetadata = SystemMetadataBuilder.Build(_registry);
_scheduler.Rebuild(localGraph.GetPhases(), newMetadata);
```

Order of `_activeMods.AddRange` and metadata build matters: if metadata is built before, the mod systems aren't yet in `_registry.GetAllSystems()` and the lookup misses them. Inverted order is the K6.1 fix.

The `UnloadMod` step 4 (graph rebuild after RemoveMod) gets the same treatment:

**Current** (K6 step 4 in `RunUnloadSteps1Through6AndCaptureAlc`):
```csharp
TryUnloadStep(4, modId, warnings, () =>
{
    var localGraph = new DependencyGraph();
    foreach (SystemRegistration reg in _registry.GetAllSystems())
        localGraph.AddSystem(reg.Instance);
    localGraph.Build();
    _scheduler.Rebuild(localGraph.GetPhases());
});
```

**K6.1 target**:
```csharp
TryUnloadStep(4, modId, warnings, () =>
{
    var localGraph = new DependencyGraph();
    foreach (SystemRegistration reg in _registry.GetAllSystems())
        localGraph.AddSystem(reg.Instance);
    localGraph.Build();
    var newMetadata = SystemMetadataBuilder.Build(_registry);
    _scheduler.Rebuild(localGraph.GetPhases(), newMetadata);
});
```

`UnloadAll` empty-active-set rebuild gets the same fix:

```csharp
if (modIds.Count == 0)
{
    var localGraph = new DependencyGraph();
    foreach (SystemRegistration reg in _registry.GetAllSystems())
        localGraph.AddSystem(reg.Instance);
    localGraph.Build();
    var newMetadata = SystemMetadataBuilder.Build(_registry);
    _scheduler.Rebuild(localGraph.GetPhases(), newMetadata);
}
```

### 1.9 — Test impact assessment

The scheduler ctor signature change touches **every test that constructs `ParallelSystemScheduler` directly**. Tests fall into two groups:

- **Group A**: tests that don't exercise faults — pass `new NullModFaultSink()` explicitly and an empty `Dictionary<SystemBase, SystemMetadata>()`. Behavior unchanged.
- **Group B**: tests that exercise faults — the K6 ModFaultHandler tests already construct a `ModFaultHandler` and call `ReportFault` directly. These tests stay valid for unit-level coverage. New end-to-end tests (Phase 4) wire a real handler through the scheduler.

The number of affected files is bounded; the executor enumerates them in Phase 4.1 before making the signature change.

---

## Phase 2 — Core-side changes

### 2.1 — Create `SystemMetadata` record

**File**: `src/DualFrontier.Core/Scheduling/SystemMetadata.cs` (NEW)

**Content**: per the §1.3 spec.

**Atomic commit**:
```
feat(core): add SystemMetadata record for scheduler context construction
```

### 2.2 — Update `ParallelSystemScheduler` ctor + Rebuild + BuildContext

**File**: `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs`

**Edits** (in order):

1. **Field addition**: add `private IReadOnlyDictionary<SystemBase, SystemMetadata> _systemMetadata;` near other fields.

2. **Ctor signature**: change from
   ```csharp
   public ParallelSystemScheduler(
       IReadOnlyList<SystemPhase> phases,
       TickScheduler ticks,
       World world,
       IModFaultSink? faultSink = null,
       IGameServices? services = null)
   ```
   to
   ```csharp
   public ParallelSystemScheduler(
       IReadOnlyList<SystemPhase> phases,
       TickScheduler ticks,
       World world,
       IReadOnlyDictionary<SystemBase, SystemMetadata> systemMetadata,
       IModFaultSink faultSink,
       IGameServices? services = null)
   ```

3. **Ctor body**: replace
   ```csharp
   _faultSink = faultSink ?? new NullModFaultSink();
   ```
   with
   ```csharp
   _faultSink = faultSink ?? throw new ArgumentNullException(nameof(faultSink));
   _systemMetadata = systemMetadata ?? throw new ArgumentNullException(nameof(systemMetadata));
   ```

4. **Ctor doc**: update parameter doc — `faultSink` is no longer optional, `systemMetadata` is new.

5. **Rebuild signature**: change from
   ```csharp
   internal void Rebuild(IReadOnlyList<SystemPhase> newPhases)
   ```
   to
   ```csharp
   internal void Rebuild(
       IReadOnlyList<SystemPhase> newPhases,
       IReadOnlyDictionary<SystemBase, SystemMetadata> newSystemMetadata)
   ```

6. **Rebuild body**: add null check for `newSystemMetadata`; add `_systemMetadata = newSystemMetadata;` between `_phases = newPhases;` and `_contextCache = newCache;`.

7. **BuildContext body**: replace per §1.4 spec.

**Atomic commit**:
```
feat(core): scheduler accepts system metadata for fault-routing origin propagation
```

### 2.3 — Verify `IModFaultSink` no longer has `NullModFaultSink` as silent default

The previous default was `_faultSink ?? new NullModFaultSink();`. K6.1 removes it. `NullModFaultSink` class itself stays for explicit use in tests; the silent default goes away.

**Verify**:
```
grep -n "NullModFaultSink" src/DualFrontier.Core/
```

**Expected**: 1-2 matches in `IModFaultSink.cs` (the class definition and possibly an XML doc reference). NO matches in `ParallelSystemScheduler.cs`.

If `ParallelSystemScheduler.cs` still references `NullModFaultSink` after Phase 2.2 edits, the executor missed an edit — halt.

---

## Phase 3 — Application-side changes

### 3.1 — Update `ModFaultHandler` ctor

**File**: `src/DualFrontier.Application/Modding/ModFaultHandler.cs`

**Edits**:

1. Remove `private readonly ModIntegrationPipeline _pipeline;` field.

2. Replace ctor body:
   ```csharp
   public ModFaultHandler(ModIntegrationPipeline pipeline)
   {
       _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
   }
   ```
   with
   ```csharp
   public ModFaultHandler()
   {
       // No dependencies. Handler is a self-contained fault accumulator;
       // consumers (ModIntegrationPipeline.Apply, ModLoader.HandleModFault)
       // query GetFaultedMods / ClearFault on demand. Owned by GameBootstrap
       // as a session-scoped singleton wired into the scheduler before
       // mods are loaded.
   }
   ```

3. Update class XML doc — remove the "Owned by ModIntegrationPipeline" line, replace with "Owned by GameBootstrap; passed to scheduler at construction and to pipeline as a query target."

**Atomic commit**:
```
refactor(application): invert ModFaultHandler ownership — no pipeline dependency
```

### 3.2 — Create `SystemMetadataBuilder`

**File**: `src/DualFrontier.Application/Modding/SystemMetadataBuilder.cs` (NEW)

**Content**:
```csharp
using System.Collections.Generic;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Projects <see cref="ModRegistry"/>'s current registration list into the
/// per-system metadata dictionary the scheduler needs for fault-routing
/// origin propagation. Called by <see cref="GameBootstrap"/> at startup
/// (initial core-only state) and by <see cref="ModIntegrationPipeline"/>
/// at every successful Apply / UnloadMod / UnloadAll boundary so the
/// scheduler's metadata stays in sync with the active mod set.
/// </summary>
internal static class SystemMetadataBuilder
{
    /// <summary>
    /// Builds an immutable snapshot of every registered system's metadata.
    /// The returned dictionary is keyed by <see cref="SystemBase"/> instance
    /// (reference equality) so the scheduler can look up the metadata for
    /// a system encountered during phase iteration without paying for
    /// reflection or string comparison.
    /// </summary>
    /// <param name="registry">Source of truth for registered systems.</param>
    /// <returns>
    /// Read-only dictionary mapping each system instance to its
    /// <see cref="SystemMetadata"/>. Systems registered as core have
    /// <c>Origin=Core, ModId=null</c>; mod systems carry their owning
    /// <c>modId</c>.
    /// </returns>
    public static IReadOnlyDictionary<SystemBase, SystemMetadata> Build(ModRegistry registry)
    {
        if (registry is null) throw new System.ArgumentNullException(nameof(registry));

        var lookup = new Dictionary<SystemBase, SystemMetadata>();
        foreach (SystemRegistration reg in registry.GetAllSystems())
        {
            lookup[reg.Instance] = new SystemMetadata(reg.Origin, reg.ModId);
        }
        return lookup;
    }
}
```

**Atomic commit**:
```
feat(application): add SystemMetadataBuilder for scheduler metadata projection
```

### 3.3 — Update `GameBootstrap.CreateLoop`

**File**: `src/DualFrontier.Application/Loop/GameBootstrap.cs`

**Edit 1**: bootstrap order rewrite. Replace the block from `var graph = new DependencyGraph();` through `var pipeline = new ModIntegrationPipeline(...)` with the K6.1 target per §1.6:

Construct `modLoader`, `modRegistry`, `faultHandler` BEFORE `scheduler`. Build initial metadata after `modRegistry.SetCoreSystems(coreSystems)`. Pass `initialMetadata` and `faultHandler` to scheduler ctor. Pass `faultHandler` to pipeline ctor.

**Edit 2**: remove `faultSink: null` literal from scheduler ctor call.

**Atomic commit**:
```
feat(application): bootstrap creates ModFaultHandler before scheduler — wires real fault sink
```

### 3.4 — Update `ModIntegrationPipeline` ctor

**File**: `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`

**Edits**:

1. **Field type unchanged**: `_faultHandler` stays `readonly`.

2. **Ctor signature**: add `ModFaultHandler faultHandler` parameter at the end (preserves positional compatibility with existing tests where possible).

3. **Ctor body**: replace
   ```csharp
   _faultHandler = new ModFaultHandler(this);
   _loader.SetFaultHandler(_faultHandler);
   ```
   with
   ```csharp
   _faultHandler = faultHandler ?? throw new ArgumentNullException(nameof(faultHandler));
   // Note: _loader.SetFaultHandler is now called by GameBootstrap before
   // scheduler construction, since the loader needs to know the handler
   // before mods can fault. The pipeline only queries the handler in
   // Apply / UnloadMod paths.
   ```

4. **Update XML doc**: mention that the handler is provided by the orchestrator (GameBootstrap) and the pipeline does not own it.

**Atomic commit**:
```
refactor(application): pipeline ctor receives ModFaultHandler from orchestrator
```

### 3.5 — Update Apply, UnloadMod step 4, UnloadAll empty-set path

**File**: `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`

**Edits** per §1.8 spec:

1. `Apply` step [8]: build new metadata after `_activeMods.AddRange(loaded)`, pass to `_scheduler.Rebuild`.

2. `RunUnloadSteps1Through6AndCaptureAlc` step 4: build new metadata after `RemoveMod` (already happened in step 3), pass to `_scheduler.Rebuild`.

3. `UnloadAll` empty-active-set rebuild: build new metadata, pass to `_scheduler.Rebuild`.

**Atomic commit**:
```
feat(application): scheduler.Rebuild calls propagate fresh system metadata
```

---

## Phase 4 — Test updates

### 4.1 — Enumerate affected test files

```
grep -l "new ParallelSystemScheduler" tests/
```

Each match needs the scheduler ctor signature update. The executor lists them in `tools/briefs/K6_1_AFFECTED_TESTS.md` (created in this phase) before making changes, with one line per file describing the kind of fix needed.

Expected result: 5-15 test files in `DualFrontier.Modding.Tests/Pipeline/`, `DualFrontier.Core.Tests/Scheduling/`, possibly `DualFrontier.Systems.Tests/`. The exact count depends on test density; the executor records the actual list.

### 4.2 — Update non-fault tests (Group A)

For tests that don't exercise fault routing, the change is mechanical:

```csharp
// Before
var scheduler = new ParallelSystemScheduler(phases, ticks, world);
// or
var scheduler = new ParallelSystemScheduler(phases, ticks, world, faultSink: null);

// After
var scheduler = new ParallelSystemScheduler(
    phases, ticks, world,
    new Dictionary<SystemBase, SystemMetadata>(),
    new NullModFaultSink());
```

Empty metadata dictionary + explicit `NullModFaultSink`. Tests that exercise faults override one or both.

A small helper test fixture may be appropriate to reduce boilerplate:

**File**: `tests/DualFrontier.Modding.Tests/Fixtures/SchedulerTestFixture.cs` (NEW or extend existing)

```csharp
internal static class SchedulerTestFixture
{
    public static ParallelSystemScheduler BuildIsolated(
        IReadOnlyList<SystemPhase> phases,
        TickScheduler ticks,
        World world,
        IModFaultSink? faultSink = null,
        IGameServices? services = null,
        IReadOnlyDictionary<SystemBase, SystemMetadata>? systemMetadata = null)
    {
        return new ParallelSystemScheduler(
            phases, ticks, world,
            systemMetadata ?? new Dictionary<SystemBase, SystemMetadata>(),
            faultSink ?? new NullModFaultSink(),
            services);
    }
}
```

Tests can then use `SchedulerTestFixture.BuildIsolated(...)` for Group A scenarios. The fixture is internal-only; production code uses the explicit ctor.

**Atomic commit**:
```
test(modding): add SchedulerTestFixture for K6.1 ctor signature changes
```

(This commit lands BEFORE the per-test edits so the fixture is available when tests reference it.)

### 4.3 — Update affected tests

Each affected test file: switch to `SchedulerTestFixture.BuildIsolated` or pass explicit empty metadata + `NullModFaultSink`. The executor groups updates by directory and commits per directory:

```
test(modding): update Pipeline tests for K6.1 scheduler ctor signature
test(core): update Scheduling tests for K6.1 scheduler ctor signature
test(systems): update Systems tests for K6.1 scheduler ctor signature  // if applicable
```

Each commit must build and tests must pass before the next commit lands.

### 4.4 — Add end-to-end fault routing tests

**File**: `tests/DualFrontier.Modding.Tests/Pipeline/K6_1_FaultRoutingEndToEndTests.cs` (NEW)

The K6 ModFaultHandler tests cover the handler in isolation (manual `ReportFault` calls). K6.1 needs **end-to-end** coverage: a real mod system commits an isolation violation, the violation routes through `SystemExecutionContext.RouteAndThrow` → real `ModFaultHandler.ReportFault`, the next pipeline `Apply` drains the faulted mod from the active set.

**Test fixtures**: a minimal mod with a system that intentionally violates isolation (writes to a component not declared in `[SystemAccess]`). The brief uses an existing fixture if one is available; otherwise a new one is added under `tests/Fixture.RegularMod_FaultsOnTick/`.

**Test cases** (≥6):

1. `ModSystemViolatesIsolation_ReportFaultCalled` — load mod, run one tick, mod system writes undeclared component, assert `faultHandler.GetFaultedMods()` contains the mod id.

2. `ModSystemViolatesIsolation_NextApplyDrainsFault` — same setup, then call `pipeline.Apply([])` (no new mods), assert mod is no longer in `pipeline.GetActiveMods()`.

3. `CoreSystemViolatesIsolation_ThrowsButFaultNotRecorded` — register a core system that violates isolation, assert `IsolationViolationException` thrown but `faultHandler.GetFaultedMods()` is empty (core faults are not routed to the handler).

4. `MultipleModSystemsFault_AllRecorded` — two mods, each with a faulting system, run a tick, assert both mod ids in `GetFaultedMods()`.

5. `FaultedModRemoved_ApplySucceedsForRemainingMods` — fault mod A, apply with mods [A, B], assert A unloaded, B loaded.

6. `FaultDuringInitialize_NotIgnoredByPipeline` — mod whose system faults during `SystemBase.Initialize` (which runs under PushContext per scheduler.InitializeAllSystems). Assert fault recorded; next Apply drains.

**Atomic commit**:
```
test(modding): add K6.1 end-to-end fault routing tests (mod fault → handler → drain)
```

### 4.5 — Update existing K6 ModFaultHandler tests for ctor change

`ModFaultHandlerTests.cs` (K6) constructs `ModFaultHandler` with a pipeline argument:

```csharp
var handler = new ModFaultHandler(pipeline);
```

After K6.1, the ctor takes no arguments:

```csharp
var handler = new ModFaultHandler();
```

The 9 existing tests need this single-line change each. The fault-recording behavior they verify is unchanged.

**Atomic commit**:
```
test(modding): update ModFaultHandlerTests for K6.1 parameterless ctor
```

### 4.6 — Build + test gate

After Phase 4 commits:

```
dotnet build
dotnet test
```

**Expected**: 0 errors, 0 warnings, all tests pass.

**Test count delta**: 547 baseline → 547 + 6 new K6.1 end-to-end tests = **553 minimum**. Some Phase 4 fixture restructuring may add 1-2 helper tests. The executor records the exact count.

**Stop condition**: any test failure. The new tests are deterministic; failures indicate either a test bug or a wiring bug — both halt for review.

---

## Phase 5 — Closure documentation

### 5.1 — MIGRATION_PROGRESS.md update

**File**: `docs/MIGRATION_PROGRESS.md`

**Edit 1**: K-series Overview table, add K6.1 row immediately after K6:

```
| K6.1 | Mod fault wiring end-to-end | DONE | 1 day hobby pace (~3-5h auto-mode) | <commit SHA range> | <date> |
```

**Edit 2**: Add K6.1 closure section after K6:

```markdown
### K6.1 — Mod fault wiring end-to-end

- **Status**: DONE (`<commit SHA range>`, <date>)
- **Brief**: `tools/briefs/K6_1_FAULT_WIRING_BRIEF.md` (FULL EXECUTED)
- **Goal**: close the wiring gap explicitly flagged in K6 closure ("Out of K6 scope (deferred)"). After K6, `ModFaultHandler` existed as infrastructure but `ParallelSystemScheduler` defaulted to `NullModFaultSink` and `BuildContext` hardcoded `SystemOrigin.Core`/`modId: null` for every system. K6.1 makes the fault path active end-to-end.
- **Architectural change**: ownership of `ModFaultHandler` inverted from `ModIntegrationPipeline` to `GameBootstrap`. Handler created before scheduler; passed as immutable reference into scheduler ctor; pipeline receives reference for query at Apply time. This eliminates the circular construction dependency that K6 worked around with a setter-after-construction pattern.
- **Per-system origin propagation**: introduced `SystemMetadata` record (Core-side) and `SystemMetadataBuilder` (Application-side projection from `ModRegistry.GetAllSystems()`). Scheduler accepts metadata dictionary at construction and Rebuild; `BuildContext` reads each system's actual origin/modId from the table instead of hardcoding `Core`/`null`.
- **Deliverables**:
  - `SystemMetadata.cs` new (Core)
  - `ParallelSystemScheduler` ctor + Rebuild + BuildContext rewritten
  - `ModFaultHandler` ctor parameter removed (no more pipeline dependency)
  - `SystemMetadataBuilder.cs` new (Application)
  - `GameBootstrap.CreateLoop` bootstrap order restructured (handler before scheduler)
  - `ModIntegrationPipeline` ctor + Apply + UnloadMod step 4 + UnloadAll updated to propagate metadata
  - `K6_1_AFFECTED_TESTS.md` (operational artifact, lists test files touched)
  - `SchedulerTestFixture.cs` helper for non-fault test scenarios
  - `K6_1_FaultRoutingEndToEndTests.cs` 6 new end-to-end fault tests
  - 9 K6 `ModFaultHandlerTests` updated for parameterless ctor
  - Affected non-fault tests updated for new scheduler ctor signature
- **Test count**: 547 baseline → **<final count>** (+6 new end-to-end tests, no test reduction).
- **Lessons learned**:
  - Setter-after-construction was the K6 workaround; K6.1 confirms restructuring ownership at the orchestrator layer is the right shape. The cost (touching `GameBootstrap` + ctor signatures) is bounded; the benefit (immutable scheduler, no temporal coupling around "is the sink wired yet?") is structural.
  - `SystemRegistration` already carried `Origin` + `ModId` since K4-era work, but the data was unused at the scheduler boundary because the scheduler is in Core (no access to `SystemRegistration` which is in Application). The Core-side `SystemMetadata` projection bridges this without the layer violation that putting `SystemRegistration` itself in Core would cause.
  - The K6 closure-shaped brief format successfully flagged the wiring gap in MIGRATION_PROGRESS.md. K6.1 is the validation: an explicitly-flagged deferred gap turns into a focused follow-up milestone with bounded scope. Future deferred-scope notes should follow the same pattern (specific files, specific line items, specific blocker rationale).
```

**Edit 3**: Update `Current state snapshot` table:

- `Last completed milestone`: K6 → K6.1
- `Tests passing`: 547 → <new count>
- `Last updated`: 2026-05-09 (K6 closure) → <new date> (K6.1 closure)

**Edit 4**: Update K6 closure section's "Out of K6 scope (deferred)" subsection to reference K6.1 as the resolution:

```markdown
- **Out of K6 scope (deferred → resolved by K6.1)**: Full `ParallelSystemScheduler`/`SystemExecutionContext` rewiring [...existing text...]. **Resolved by K6.1 (`<commit SHA range>`, <date>) — see K6.1 closure section below for details.**
```

**Atomic commit**:
```
docs(migration): K6.1 closure recorded — fault wiring end-to-end
```

### 5.2 — Final verification

```
dotnet build
dotnet test
```

**Expected**: 0 errors, 0 warnings, all tests pass.

**Final pre-commit grep** (AD #4 discipline):

```
grep -rn "faultSink: null" src/ tests/
grep -rn "SystemOrigin.Core,.*modId: null" src/
grep -rn "new NullModFaultSink" src/
```

**Expected**:
- First grep returns 0 matches (the explicit null pass-through is gone)
- Second grep returns 0 matches in `src/` (BuildContext fix removed the hardcode)
- Third grep returns 0 matches in `src/`; matches in `tests/` are acceptable (tests construct null sinks explicitly for fault-irrelevant scenarios)

---

## Atomic commit log expected

Approximate commit count: **10-14**, depending on number of test files touched in Phase 4.3:

**Always present** (10):
1. `feat(core): add SystemMetadata record for scheduler context construction` (Phase 2.1)
2. `feat(core): scheduler accepts system metadata for fault-routing origin propagation` (Phase 2.2)
3. `refactor(application): invert ModFaultHandler ownership — no pipeline dependency` (Phase 3.1)
4. `feat(application): add SystemMetadataBuilder for scheduler metadata projection` (Phase 3.2)
5. `feat(application): bootstrap creates ModFaultHandler before scheduler — wires real fault sink` (Phase 3.3)
6. `refactor(application): pipeline ctor receives ModFaultHandler from orchestrator` (Phase 3.4)
7. `feat(application): scheduler.Rebuild calls propagate fresh system metadata` (Phase 3.5)
8. `test(modding): add SchedulerTestFixture for K6.1 ctor signature changes` (Phase 4.2)
9. `test(modding): add K6.1 end-to-end fault routing tests (mod fault → handler → drain)` (Phase 4.4)
10. `test(modding): update ModFaultHandlerTests for K6.1 parameterless ctor` (Phase 4.5)
11. `docs(migration): K6.1 closure recorded — fault wiring end-to-end` (Phase 5.1)

**Phase 4.3 conditional** (1-3 commits, one per affected test directory):
- `test(modding): update Pipeline tests for K6.1 scheduler ctor signature`
- `test(core): update Scheduling tests for K6.1 scheduler ctor signature`
- `test(systems): update Systems tests for K6.1 scheduler ctor signature` (if applicable)

A merge commit on `main` is **not** in this list — fast-forward merge.

---

## Cross-cutting design constraints

This brief explicitly enforces the following architectural invariants. The executor checks each at the relevant phase and halts on violation.

1. **No setter-after-construction** (per §1.1 ownership inversion decision). `ParallelSystemScheduler._faultSink` and `_systemMetadata` are immutable post-construction (well, `_systemMetadata` is mutable for Rebuild, but only via the explicit Rebuild method; no public/internal setter for fault sink). Adding a `SetFaultSink` method anywhere in the scheduler is a brief violation — halt.

2. **Layer discipline preserved** (per §1.3 LOCKED Option c choice). `SystemRegistration` stays in Application; Core gets a minimal projection (`SystemMetadata`). Moving `SystemRegistration` into Core or adding a Modding-namespace import in Core code is a brief violation — halt.

3. **Non-optional faultSink** (per §1.3 ctor signature change). Tests that need a no-op sink construct `new NullModFaultSink()` explicitly. Reverting the ctor parameter to `IModFaultSink? = null` would re-introduce the silent-default bug K6.1 fixes — halt.

4. **Atomic commits per logical change** (project standing rule). One commit per Phase sub-step. Each commit must build and test cleanly.

5. **No regex metacharacters in `Edit` tool boundaries** (per `MOD_OS_V16_AMENDMENT_CLOSURE.md` lessons learned). All `oldText` / `newText` payloads are plain prose / code without regex metacharacters at boundary positions.

6. **Pre-flight grep discipline** (AD #4). Phase 0.4 inventory is structured grep verification. Phase 5.2 final pre-commit grep verifies no regression. Every Phase that touches existing identifiers checks for collisions via grep before introducing changes.

7. **Triple binding awareness** (Russian error message strings caveat). K6.1 work touches `IsolationDiagnostics` strings used in `BuildReadViolationMessage` / `BuildWriteViolationMessage`. These strings appear in source code, tests, and possibly spec wording. K6.1 must NOT modify any of them. Edits are limited to plumbing (ctor signatures, field types, lookup logic); message bodies are out of scope.

8. **«Data exists or it doesn't»** (METHODOLOGY §7.1). `BuildContext` for a system not in `_systemMetadata` falls through to `Core`/`null` defaults (per §1.4). This is the explicit "missing data" branch — not a silent assumption that everything is mod-origin or core-origin. The default is documented in the BuildContext body comment.

9. **Single ownership boundary**. `ModFaultHandler._faultedMods` HashSet is locked by `_lock`. No external mutation. `GameBootstrap` constructs the handler; `ParallelSystemScheduler` holds it as read-only sink; `ModIntegrationPipeline` queries via `GetFaultedMods` and `ClearFault` (single-id write); `ModLoader.HandleModFault` reports via `ReportFault`. Four consumers, each with explicit access pattern; no shared mutable state outside the lock.

10. **Construction order documented in code**. The new bootstrap order in `GameBootstrap.CreateLoop` includes a comment block explaining why ModFaultHandler is created before the scheduler. Future readers (or future Crystalka 6 months from now) need to understand why the order matters; a regression to "construct scheduler first, wire handler later" would silently undo K6.1.

---

## Stop conditions

The executor halts and escalates the brief authoring session if any of the following:

1. Any pre-flight check (Phase 0) fails — working tree dirty, prerequisites missing, specs at unexpected version, baseline build/test fails.

2. Phase 0.4 inventory diverges substantially from the table — particularly if `BuildContext` already reads from a metadata dictionary (someone fixed the gap) or if `ModFaultHandler` ctor is already parameterless. Indicates intervening work; brief overlaps.

3. Phase 1 design contract is unclear in any spot — e.g., `SystemMetadata` location decision (LOCKED Option c) is questioned. Halt and re-read this brief; if still unclear, escalate to brief authoring session.

4. Phase 2 or Phase 3 edit lands but build fails. Each commit must build clean before the next. Failure means an earlier edit was incomplete or wrong; halt and review the diff.

5. Phase 4 affected-tests list reveals tests that exercise scenarios K6.1 spec didn't anticipate — e.g., a test that relies on the silent `NullModFaultSink` default for a fault-relevant scenario. Halt and surface the test for review.

6. Phase 4.4 end-to-end tests fail in a way that suggests a deeper wiring bug — e.g., `ReportFault` is called but the fault doesn't propagate to `_faultedMods`, or the metadata table doesn't reflect the post-Apply state. Halt; the bug is in the K6.1 implementation, not the test.

7. Phase 5 closure record includes inaccurate commit SHAs or test counts. The executor records actual values, not estimates. If the verification log shows a gap between brief expectation and reality, halt.

8. The `Edit` tool reports unexpected behavior on any oldText/newText pair.

9. Any phase modifies native kernel files. K6.1 is managed-only.

10. The new bootstrap order in `GameBootstrap.CreateLoop` does not preserve the M7.5.B.1 wiring (controller's `OnEditingBegan`/`OnEditingEnded` hooks to `loop.SetPaused`). The scheduler reorder must NOT disturb this.

The fallback in every halt case is `git stash push -m "k6-1-WIP-halt-$(date +%s)"` and report to the brief author. Partial K6.1 work is recoverable; an ad-hoc continuation on a corrupted state is not.

---

## Brief authoring lineage

- **2026-05-09** — K6 closed with explicit "Out of K6 scope (deferred)" flag for fault wiring. K6.1 brief authored same day in response: read-first investigation of `ParallelSystemScheduler` ctor signature, `SystemExecutionContext.BuildContext` hardcode, `GameBootstrap` construction order, `SystemRegistration` data shape; design decisions LOCKED (ownership inversion, Core-side `SystemMetadata` projection, immutable scheduler); brief written as pure implementation contract.
- **(date TBD)** — Executed and closed at K6.1 milestone closure.

The brief was authored read-first / brief-second per the methodology pivot recorded in `MOD_OS_V16_AMENDMENT_CLOSURE.md`. Source documents read during authoring: `KERNEL_ARCHITECTURE.md` v1.1 LOCKED §K6 (post-K6 amendment), `MOD_OS_ARCHITECTURE.md` v1.6 LOCKED §10.3, `MIGRATION_PROGRESS.md` (live, K6 closure section), existing managed code (`ParallelSystemScheduler.cs`, `SystemExecutionContext.cs`, `GameBootstrap.cs`, `ModFaultHandler.cs`, `ModIntegrationPipeline.cs`, `ModRegistry.cs`, `SystemRegistration.cs`, `IModFaultSink.cs`). The "no compromises" rule applied: ownership inversion via bootstrap restructure, not setter-after-construction; per-system metadata propagation via Core-side projection record, not Application-namespace leak into Core.

---

## Methodology note

K6.1 is the first follow-up milestone in the project. K6 closure-shaped brief format successfully flagged a deferred scope item; K6.1 implementation brief picks it up with bounded scope (no drift reconciliation, single coherent implementation goal, well-defined entry/exit criteria). The pattern — closure-shaped brief surfaces a deferred gap → focused follow-up milestone closes it — is a methodology validation: deferred scope is honest, not abandoned, and gets a real milestone rather than a backlog ticket. Future K-series milestones may produce similar follow-ups; the pattern is now established.

---

**Brief end.** Awaits Crystalka's review and feed to Claude Code session for execution.
