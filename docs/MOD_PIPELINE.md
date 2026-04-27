# Dual Frontier — MOD_PIPELINE

**Modular mod integration pipeline**
Architecture version: v0.2  |  Implementation phase: 2

## Concept

Mods register contracts (components, systems, events) through `IModApi`. The dependency graph (`DependencyGraph`) is rebuilt once when mods are applied — before the game session begins. At runtime the graph and the scheduler are immutable.

```
MOD MENU                         GAME RUNTIME
─────────────────────────        ────────────────────────────
ModIntegrationPipeline           ParallelSystemScheduler
├── ContractValidator            (immutable during the session)
├── ModRegistry
├── DependencyGraph.Reset()
├── DependencyGraph.Build()
└── Scheduler.Rebuild()
```

**Key property**: the graph is always consistent — either the old working one, or the new fully built one. A partially built graph never exists.

## Components

### 1. ModIntegrationPipeline

The orchestrator of the entire process. Invoked from the UI menu when "Apply mods" is clicked.

Responsibility: sequentially invoke the other components, roll back changes on error, return the final report.

```
ModIntegrationPipeline.Apply(IReadOnlyList<string> modPaths)
│
├── 1. ModLoader.LoadMod(path)                   → List<LoadedMod>
├── 2. ContractValidator.Validate(mods, core)    → ValidationReport
│       ├── ContractsVersion check
│       └── pre-registration write-write check
├── 3. IMod.Initialize(RestrictedModApi)         — registration in ModRegistry
├── 4. DependencyGraph.Reset() (local)
├── 5. DependencyGraph.AddSystems(core + mods)
├── 6. DependencyGraph.Build()                   → List<SystemPhase>
└── 7. ParallelSystemScheduler.Rebuild(phases)
```

### 2. ContractValidator

Two-phase validation before registration in the graph.

**Phase A — versioning**: every manifest declares `requiresContractsVersion`. The validator compares it to `ContractsVersion.Current` and rejects mods where `Major != Current.Major` or `Minor/Patch > Current`.

**Phase B — component conflicts**: before `DependencyGraph.Build()` is invoked, the validator checks write-write collisions across every declared system (Core + mods). It produces a precise message: "mod X conflicts with mod Y on component Z" rather than a bare "write conflict detected" from inside the graph.

```csharp
internal sealed class ContractValidator
{
    public ValidationReport Validate(
        IReadOnlyList<LoadedMod> mods,
        IReadOnlyList<SystemBase> coreSystems);
}

public sealed record ValidationError(
    string ModId,
    ValidationErrorKind Kind,
    string Message,
    string? ConflictingModId = null,
    Type? ConflictingComponent = null);

public enum ValidationErrorKind
{
    IncompatibleContractsVersion, // mod requires > the current contracts version
    WriteWriteConflict,           // two mods write the same component
    CyclicDependency,             // the mod graph contains a cycle
    MissingDependency,            // mod requires another mod that is missing
}
```

### 3. ModRegistry

Registry of every component and system registered by mods. The backing storage for `RestrictedModApi`.

```csharp
internal sealed class ModRegistry
{
    public void SetCoreSystems(IReadOnlyList<SystemBase> coreSystems);
    public void RegisterComponent(string modId, Type componentType);
    public void RegisterSystem(string modId, Type systemType);
    public IReadOnlyList<SystemRegistration> GetAllSystems();
    public void ResetModSystems();
    public void RemoveMod(string modId);
}

internal sealed record SystemRegistration(
    SystemBase Instance,
    SystemOrigin Origin,
    string? ModId);
```

Rules:

- `RegisterComponent` throws `InvalidOperationException` if the type is already registered by another mod (it names both owners).
- `RegisterSystem` checks for `[SystemAccess]` and `[TickRate]`; otherwise it throws `InvalidOperationException` with a hint.
- `GetAllSystems` returns Core systems first, then mod systems in registration order.
- `ResetModSystems` resets only the mod systems; Core remains.

### 4. RestrictedModApi

The `IModApi` implementation that `ModLoader` hands to every mod in `IMod.Initialize`. Proxies calls into the core through `ModRegistry` and `IModContractStore`.

```csharp
internal sealed class RestrictedModApi : IModApi
{
    internal RestrictedModApi(
        string modId,
        ModRegistry registry,
        IModContractStore contractStore,
        IGameServices services);

    public void RegisterComponent<T>() where T : IComponent;
    public void RegisterSystem<T>() where T : class;
    public void PublishContract<T>(T c) where T : IModContract;
    public bool TryGetContract<T>(out T? c) where T : class, IModContract;
    public void Subscribe<T>(Action<T> handler) where T : IEvent;
    public void Publish<T>(T evt) where T : IEvent;

    internal void UnsubscribeAll();
}
```

Subscriptions (`Subscribe<T>`) are tracked in a private `List<(Type eventType, Delegate handler)> _subscriptions` for removal during `Unload`. Routing events to specific buses is handled in the next sub-phase.

### 5. IModContractStore

Registry of inter-mod contracts (`IModContract`). A component separate from `ModRegistry`, because contracts live at runtime (not only during loading).

```csharp
internal interface IModContractStore
{
    void Publish<T>(string modId, T contract) where T : IModContract;
    bool TryGet<T>(out T? contract) where T : class, IModContract;
    void RevokeAll(string modId);
}
```

When the provider mod is unloaded: `RevokeAll(modId)` revokes all of its contracts. Subsequent `TryGetContract` calls return `false`.

## Lifecycle

### Mod loading (in the menu)

```
The user clicks "Apply"
      │
      ▼
ModIntegrationPipeline.Apply()
      │
      ├── [1] ModLoader: reads mod.manifest.json
      │         creates ModLoadContext (AssemblyLoadContext, isCollectible: true)
      │         finds IMod via reflection
      │
      ├── [2] ContractValidator.Validate()
      │         on errors, the pipeline stops,
      │         returns ValidationReport to the UI
      │
      ├── [3] IMod.Initialize(RestrictedModApi)
      │         the mod registers components/systems
      │         the mod subscribes to events
      │
      ├── [4] DependencyGraph.Reset() + AddSystems() + Build()
      │         the graph is rebuilt with Core + mod systems
      │
      └── [5] ParallelSystemScheduler.Rebuild(phases)
                the new scheduler is ready for the next session
```

### Unloading a mod

```
ModLoader.UnloadMod(modId)
      │
      ├── [1] ModRegistry.ResetModSystems()       — remove the mod's systems
      ├── [2] IModContractStore.RevokeAll(modId)  — revoke inter-mod contracts
      ├── [3] EventBus: remove every subscription of the mod
      ├── [4] IMod.Unload() with a 500ms timeout
      ├── [5] ModLoadContext.Unload()             — unload the AssemblyLoadContext
      ├── [6] DependencyGraph.Reset() + Build()   — rebuild without the mod
      └── [7] Publish ModDisabledEvent            — the UI shows a banner
```

### ModFaultHandler (runtime violation)

If a mod system violates isolation at runtime (not from the menu), `ModFaultHandler` runs the same chain, but step **[6] is skipped** (the graph cannot be rebuilt at runtime). Instead: the mod's systems are marked `Disabled` in the scheduler, and the graph is rebuilt the next time the mod menu is opened.

## File layout

```
src/DualFrontier.Application/Modding/
├── ModIntegrationPipeline.cs ← orchestrator
├── ContractValidator.cs      ← validation
├── ModRegistry.cs            ← system / component registry
├── IModContractStore.cs      ← inter-mod contracts interface
├── ModContractStore.cs       ← implementation
├── ModLoader.cs              ← loads .manifest.json + AssemblyLoadContext
├── ModLoadContext.cs         ← per-mod AssemblyLoadContext
├── RestrictedModApi.cs       ← IModApi implementation
├── ValidationError.cs        ← ValidationError / ValidationWarning / Kind
├── ValidationReport.cs       ← ValidationReport + Ok()
├── LoadedMod.cs              ← result of loading a single mod
├── SystemRegistration.cs     ← registration record in the system registry
└── ModIsolationException.cs

src/DualFrontier.Contracts/Modding/
├── IModApi.cs                ← stable
├── IMod.cs                   ← stable
├── IModContract.cs           ← stable
├── ModManifest.cs            ← + RequiresContractsVersion
└── ContractsVersion.cs       ← Parse/IsCompatible/Current

tests/DualFrontier.Modding.Tests/
├── Pipeline/
│   ├── ModIntegrationPipelineTests.cs
│   └── ContractValidatorTests.cs
└── Fixtures/
    └── GoodMod/
        └── GoodMod.cs        ← a legal test mod
```

## Component APIs

### ModIntegrationPipeline

```csharp
internal sealed class ModIntegrationPipeline
{
    /// <summary>
    /// Applies the list of mods: load, validate, register,
    /// rebuild the graph and the scheduler. Called only from the menu.
    /// </summary>
    public PipelineResult Apply(IReadOnlyList<string> modPaths);

    /// <summary>
    /// Unloads every active mod and returns the scheduler
    /// to a Core-systems-only state.
    /// </summary>
    public void UnloadAll();
}

public sealed record PipelineResult(
    bool Success,
    IReadOnlyList<ValidationError> Errors,
    IReadOnlyList<string> LoadedModIds,
    IReadOnlyList<string> FailedModIds);
```

### ContractsVersion

```csharp
public readonly struct ContractsVersion
{
    public static readonly ContractsVersion Current = new(1, 0, 0);
    public static ContractsVersion Parse(string text);
    public static bool IsCompatible(ContractsVersion required, ContractsVersion available);
}
```

## Error handling

Strategy: fail-fast in the menu, graceful at runtime.

| Where               | Error                           | Behavior                                          |
|---------------------|---------------------------------|---------------------------------------------------|
| Menu — validation   | `IncompatibleContractsVersion`  | Mod is not loaded; show the version               |
| Menu — validation   | `WriteWriteConflict`            | Both mods fail to load; name the component        |
| Menu — validation   | `MissingDependency`             | The dependent mod fails to load                   |
| Menu — `Build()`    | Cycle in the graph              | Full rollback; the old scheduler is preserved     |
| Runtime             | `ModIsolationException`         | `ModFaultHandler`: the mod is unloaded; the game continues |
| Runtime             | Any exception from a mod assembly | The same `ModFaultHandler`                      |

## Graph-rebuild atomicity

`DependencyGraph.Reset()` + `Build()` are performed on a **local variable**. The scheduler is replaced only after `Build()` succeeds. On error, the old scheduler stays active.

```csharp
// Pseudocode inside Pipeline
var newGraph = new DependencyGraph();
foreach (var system in _registry.GetAllSystems())
    newGraph.AddSystem(system.Instance);
newGraph.Build();                     // if it throws — the scheduler is untouched

// only here do we replace the existing scheduler's phases
_scheduler.Rebuild(newGraph.GetPhases());
```

## Tests

### Pipeline tests

- `Pipeline_with_valid_mod_loads_and_rebuilds_scheduler`
- `Pipeline_with_version_conflict_rejects_mod_keeps_old_scheduler`
- `Pipeline_with_write_conflict_rejects_both_mods_with_precise_message`
- `Pipeline_build_failure_leaves_old_scheduler_intact`
- `Pipeline_unload_removes_mod_systems_from_scheduler`

### Validator tests

- `Validator_rejects_mod_requiring_newer_contracts_version`
- `Validator_detects_write_conflict_between_two_mods`
- `Validator_detects_write_conflict_between_mod_and_core`
- `Validator_valid_mods_return_empty_errors`
- `Validator_reports_precise_component_in_conflict_message`
- `Validator_ok_for_compatible_older_patch_version`

## Acceptance criteria (Phase 2)

- `ModLoader.LoadMod` reads the manifest, creates an `AssemblyLoadContext`, and calls `IMod.Initialize`.
- `ContractValidator` returns a precise message on a write-write conflict between mods.
- `ModIntegrationPipeline.Apply` is atomic: an error at any step does not break the current scheduler.
- Test: a mod with `RequiresContractsVersion: "2.0.0"` does not load against current version 1.x.
- Test: two mods writing the same component — both rejected with the conflict component named.
- Test: `ModLoadContext.Unload` physically releases the assembly (`WeakReference` test — part 2).
- Every test in the "Tests" section is green.

## See also

- [MODDING](./MODDING.md) — guide for mod authors
- [ISOLATION](./ISOLATION.md) — the isolation guard and ModFaultHandler
- [CONTRACTS](./CONTRACTS.md) — contract versioning
- [THREADING](./THREADING.md) — DependencyGraph and the scheduler
- [ROADMAP](./ROADMAP.md) — Phase 2 (modding acceptance criteria)
