---
register_id: DOC-A-MODDING
project: Dual Frontier
category: A
tier: 1
lifecycle: SUPERSEDED
owner: Crystalka
version: 1.1
first_authored: 2026-07-15
last_modified: 2026-07-15
content_language: en
next_review_due: null
title: Writing mods (historical; superseded by authored rework)
superseded_by: DOC-A-MODDING_V2
last_modified_commit: 6888246
review_cadence: on-change+annual
reviewer: Crystalka
risks_referenced:
- RISK-005
special_case_rationale: Superseded by DOC-A-MODDING_V2 per corpus rework EVT-2026-07-15-CORPUS_REWORK_R2_PLATFORM. Last-ratified reference preserved at docs/architecture/historical/MODDING.md; successor ratified LOCKED v1.0.0 2026-07-17 (EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION) — authority-gap window closed.
---

# Writing mods

In RimWorld a mod patches any private method via Harmony and at some point breaks it. Dual Frontier loads every mod into its own `AssemblyLoadContext`: the mod physically cannot see `DualFrontier.Core`, has no reference to `World` or to any concrete system. Mods interact with the core and with each other through contracts. This takes longer to write, but mods gain compatibility across versions and across each other.

## IMod

Every mod implements `IMod` — the single entry point.

```csharp
public interface IMod
{
    /// <summary>
    /// Called when the mod is loaded. Register components, systems,
    /// subscribe to events. The mod receives an IModApi — the only
    /// way to interact with the core.
    /// </summary>
    void Initialize(IModApi api);

    /// <summary>
    /// Called before the mod is unloaded. The mod MUST unsubscribe from
    /// every event and release resources. After this method returns,
    /// the AssemblyLoadContext is unloaded.
    /// </summary>
    void Unload();
}
```

The `ModLoader` loader in `DualFrontier.Application/Modding` creates a `ModLoadContext` (a subclass of `AssemblyLoadContext`) for each mod, loads the assembly, finds the class implementing `IMod`, and calls `Initialize`.

## IModApi — what is allowed

The mod receives an `IModApi` and may only do what the contract enumerates:

```csharp
// Canonical: src/DualFrontier.Contracts/Modding/IModApi.cs (v3 strict — K8.3+K8.4 cutover 2026-05-14)
public interface IModApi
{
    // Path α: NativeWorld-backed unmanaged struct storage (K-L3 default).
    void RegisterComponent<T>() where T : unmanaged, IComponent;

    // Path β: per-mod managed-class storage (K-L3.1 bridge).
    // T must be annotated with [ManagedStorage].
    void RegisterManagedComponent<T>() where T : class, IComponent;

    void RegisterSystem<T>() where T : class;

    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(Action<T> handler) where T : IEvent;
    // Note: no Unsubscribe — ModApi tracks every subscription and removes
    // them all on Unload via RestrictedModApi.UnsubscribeAll().

    // Publishes a contract for other mods
    void PublishContract<T>(T contract) where T : IModContract;

    // Retrieves another mod's contract (optional, graceful-degrade pattern)
    bool TryGetContract<T>(out T? contract) where T : class, IModContract;

    // Kernel capability set the mod may declare in its manifest
    IReadOnlySet<string> GetKernelCapabilities();

    // The mod's own manifest, as parsed by the loader
    ModManifest GetOwnManifest();

    // Single structured log entry; mod-id prefix added by RestrictedModApi.
    void Log(ModLogLevel level, string message);

    // Field-storage sub-API per MOD_OS_ARCHITECTURE §4.6 — null on builds
    // without K9 field storage support; mods null-check and degrade gracefully.
    IModFieldApi? Fields { get; }

    // Compute-pipeline sub-API per MOD_OS_ARCHITECTURE §4.6 — null on K9
    // (lands at G0). Mods null-check.
    IModComputePipelineApi? ComputePipelines { get; }
}
```

Anything not in `IModApi` is unreachable. `IModApi` is implemented inside the core (`RestrictedModApi`) and proxies calls into the core: system registration checks for the `[SystemAccess]` attribute, component registration is stored in `ComponentRegistry` tagged with the mod-id. **Strict v3 — manifestVersion must be `"3"`; v2 / v1 IModApi surfaces were deleted entirely in K8.3+K8.4 cutover (2026-05-14) per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §4.6.3.**

| Action                                | Allowed | Reason                                            |
|---------------------------------------|---------|---------------------------------------------------|
| Publish events to a bus               | Yes     | Through `IModApi` — proxied                       |
| Subscribe to events                   | Yes     | Through `IModApi` — proxied                       |
| Register Path α components            | Yes     | `RegisterComponent<T>` — `where T : unmanaged, IComponent` |
| Register Path β components            | Yes     | `RegisterManagedComponent<T>` — class + `[ManagedStorage]` |
| Register systems                      | Yes     | Through `IModApi` + `[SystemAccess]` declaration  |
| Publish a contract for mods           | Yes     | `IModContract` — a public interface               |
| Register a compute pipeline           | Yes     | `IModApi.ComputePipelines` (null pre-G0)          |
| Register a field type                 | Yes     | `IModApi.Fields` (null pre-K9 field support)      |
| Obtain a reference to `NativeWorld`   | No      | `AssemblyLoadContext` blocks `DualFrontier.Core`  |
| Obtain a reference to a system        | No      | No `GetSystem` accessor exists (deleted K8.3+K8.4) |
| Load `DualFrontier.Core`              | No      | `AssemblyLoadContext` blocks it                   |
| Bypass `EventBus` directly            | No      | Physically no reference                           |

## AssemblyLoadContext — what is physically unreachable

When asked to load an assembly, `ModLoadContext` inspects the name:

- `DualFrontier.Contracts` — passed through from the main context (shared).
- `DualFrontier.Core`, `DualFrontier.Systems`, `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Application` — **refused**. The load attempt fails when the mod runtime cannot resolve the assembly; the exception propagates up to `ModFaultHandler`, which runs the §9.5 unload chain (see [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §9.5).
- `System.*`, `Microsoft.*`, third-party libraries under the mod's `BasePath` — passed through.

This means: even if a mod tries to find the `World` type through reflection, it will not find the assembly. Even compiling a mod with `using DualFrontier.Core;` will fail, because `ModLoadContext` will not supply the assembly when the package is loaded.

## IModContract — API between mods

Mods do not reference each other directly: that creates loading cycles and hard dependencies. Instead, a mod publishes an interface that implements `IModContract`, and another mod requests it by type.

```csharp
// In the shared VoidMagic.Contracts assembly:
public interface IVoidMagicContract : IModContract
{
    bool CanCastVoid(EntityId caster);
    void EmitVoidSurge(EntityId source);
}

// Mod A (VoidMagic) publishes the implementation:
public class VoidMagicMod : IMod
{
    public void Initialize(IModApi api)
    {
        api.PublishContract<IVoidMagicContract>(new VoidMagicImpl());
    }
    public void Unload() { }
}

// Mod B (ArtifactMod) picks it up if available:
public class ArtifactMod : IMod
{
    public void Initialize(IModApi api)
    {
        if (api.TryGetContract<IVoidMagicContract>(out var voidMagic))
        {
            api.Subscribe<VoidSpellCastEvent>(e => voidMagic.EmitVoidSurge(e.CasterId));
        }
        // Mod A is not loaded — simply do not subscribe.
        // No crash, no hard dependency.
    }
    public void Unload() { }
}
```

A contract is a regular public interface housed in a separate assembly that both mods know. Typically this is `ModName.Contracts.dll`, distributed by the author of the original mod.

## mod.manifest.json

Every mod contains a manifest file at the package root. **`manifestVersion` must be exactly `"3"`** — the parser rejects any other value (strict v3, K8.3+K8.4 cutover).

```json
{
  "manifestVersion": "3",
  "id": "com.example.voidmagic",
  "name": "Void Magic",
  "description": "Adds the Void school of magic.",
  "author": "Example Modder",
  "version": "1.2.0",
  "requiresContracts": "^1.0.0",
  "capabilities": {
    "required": ["kernel.bus.subscribe:Magic"]
  },
  "dependencies": [],
  "optionalDependencies": [
    { "id": "com.example.artifactmod", "version": "^0.5.0" }
  ],
  "entryAssembly": "VoidMagic.dll",
  "entryType": "VoidMagic.VoidMagicMod"
}
```

- `manifestVersion` — strict literal `"3"` per IModApi v3 (anything else is a load-time error).
- `id` — a unique identifier in reverse-domain style.
- `version` — the mod's SemVer.
- `requiresContracts` — minimum `DualFrontier.Contracts` version. The loader refuses if the core has a different major version than the requirement.
- `capabilities.required` — capability strings the mod needs (see [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §3.2).
- `dependencies` — required mods; their absence blocks loading.
- `optionalDependencies` — optional; signal that integration via a contract is possible, but the mod also works without them.
- `entryAssembly` / `entryType` — the assembly name and the FQN of the class implementing `IMod`.

## Hot reloading

`ModLoader` supports unloading through `ModLoadContext.Unload()`. The sequence follows the canonical §9.5 unload chain in [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) (best-effort per §9.5.1):

1. `ModRegistry.ResetModSystems()` — remove the mod's systems from the scheduler.
2. `IModContractStore.RevokeAll(modId)` — revoke this mod's inter-mod contracts.
3. `EventBus`: remove every subscription registered by the mod (`RestrictedModApi.UnsubscribeAll`).
4. `IMod.Unload()` is invoked with a bounded timeout; user code releases resources.
5. `ModLoadContext.Unload()` releases the `AssemblyLoadContext` (after the next GC).
6. `DependencyGraph.Reset()` + `Build()` rebuilds without the mod (deferred to the mod menu for runtime faults — see [MOD_PIPELINE](./MOD_PIPELINE.md) §ModFaultHandler).
7. `ModDisabledEvent` is published; the UI shows a banner.

Limitation: `AssemblyLoadContext.Unload` is collaborative. If a mod leaves a reference to its type in another mod's static field, unloading will not complete. That is why subscription tracking + `IMod.Unload` discipline matter.

Hot reload (updating a mod without exiting the game) = Unload + Load with the new version. No save is required — `NativeWorld` does not change, only the set of systems and subscriptions. Path β managed-class component data does not survive a reload — it lives in per-mod `ManagedStore<T>` that is collected when the ALC is unloaded (K-L3.1 lock: Path β is runtime-only, not persisted).

## Step-by-step guide

To create your first mod:

1. **Create the project.** `dotnet new classlib -n MyFirstMod -f net8.0`.
2. **Add a reference to the contracts.** `<Reference Include="DualFrontier.Contracts.dll" Private="false" />`. The `Private=false` flag matters: the mod must not bundle contracts alongside itself.
3. **Write the `IMod` implementation.**

    ```csharp
    using DualFrontier.Contracts.Modding;

    namespace MyFirstMod;

    public sealed class MyFirstMod : IMod
    {
        public void Initialize(IModApi api)
        {
            api.Log(ModLogLevel.Info, "MyFirstMod initialized");
            api.Subscribe<DeathEvent>(OnDeath);
        }

        public void Unload()
        {
            // The subscription will be removed automatically.
        }

        private void OnDeath(DeathEvent e) { /* ... */ }
    }
    ```

4. **Write the manifest.** `mod.manifest.json` — as in the example above.
5. **Build and place in `mods/`.**
    ```
    mods/com.example.myfirstmod/
        MyFirstMod.dll
        mod.manifest.json
    ```
6. **Launch the game.** The log will show `[com.example.myfirstmod] MyFirstMod initialized`.

## Mod Integration Pipeline

### When it runs

The pipeline activates only in the mod settings menu when "Apply" is clicked. During an active game session, the dependency graph and the scheduler do not change.

```
Mod menu                       Game session
──────────────────────         ────────────────────
[Apply mods]                   The scheduler runs
      │                        (unchanged)
      ▼
ModIntegrationPipeline
1. Load manifests
2. Validate contracts    ←──── the only place
3. Register systems            where the graph changes
4. Rebuild the graph
5. New scheduler
```

### Validation at load time

Before registering systems, `ContractValidator` checks:

**Contracts version.** The mod manifest declares the minimum version:

```json
{
  "id": "com.example.mymod",
  "requiresContractsVersion": "1.0.0"
}
```

If the current `DualFrontier.Contracts` version is incompatible, the mod is not loaded and a precise version-incompatibility message is shown.

**Write-write conflicts.** If two mods (or a mod and Core) declare `[SystemAccess(writes: new[]{typeof(SomeComponent)})]` on the same component, both are rejected before the graph is built. The message names the specific component and both conflict participants.

**Inter-mod dependencies.** If mod A requires mod B (via the manifest) and B is not loaded, A does not load.

### What happens on a validation error

The pipeline is atomic. On any error:

- The old scheduler stays active.
- The UI shows a precise error description naming the mods involved.
- Already-loaded successful mods are not rolled back (only the failed ones).

### What a mod can register

Through `IModApi` in `IMod.Initialize`:

| Method                          | What it does                                      | Constraint                                                  |
|---------------------------------|---------------------------------------------------|-------------------------------------------------------------|
| `RegisterComponent<T>()`        | Adds a Path α component type (NativeWorld struct) | `T : unmanaged, IComponent`, once per type                  |
| `RegisterManagedComponent<T>()` | Adds a Path β component type (per-mod managed)    | `T : class, IComponent`, annotated `[ManagedStorage]`       |
| `RegisterSystem<T>()`           | Adds a system to the scheduler                    | `T : class`, requires `[SystemAccess]` and `[TickRate]`     |
| `Subscribe<T>(handler)`         | Subscribe to a bus event                          | Removed automatically on `Unload` (no separate `Unsubscribe`) |
| `Publish<T>(evt)`               | Publish an event                                  | Only to the bus declared in the mod's `[SystemAccess]`      |
| `PublishContract<T>(c)`         | Publish a contract for other mods                 | `T : IModContract`                                          |
| `TryGetContract<T>(out)`        | Retrieve another mod's contract                   | Graceful degrade if the mod is not loaded                   |
| `Log(level, message)`           | Structured log with mod-id prefix                 | `ModLogLevel` (Info / Warning / Error)                      |
| `Fields`                        | Field-storage sub-API (§4.6 of MOD_OS)            | `null` on builds without K9 field storage                   |
| `ComputePipelines`              | Compute-pipeline sub-API (§4.6 of MOD_OS)         | `null` until G0 lands                                        |
| `GetKernelCapabilities()`       | Capability strings the kernel exposes             | Used by mod to know what it may declare in manifest         |
| `GetOwnManifest()`              | The parsed manifest of the calling mod            | Read-only snapshot                                          |

### Hot reload

Available only from the menu. Sequence:

```
[Reload mod X]
      │
      ├── UnloadMod(X)    — unload the AssemblyLoadContext
      ├── LoadMod(X)      — reload from disk
      └── Pipeline.Apply() — rebuild the scheduler
```

Limitation: the mod's component data written to `World` does not survive a hot reload. This is fine for development iteration but unsuitable for swapping mods mid-session.

### Inter-mod contracts

Mods communicate only through `IModContract` — no direct references to another mod's assembly.

```csharp
// Provider mod publishes the contract
public interface IMyModApi : IModContract
{
    void DoSomething();
}

public void Initialize(IModApi api)
{
    api.PublishContract<IMyModApi>(new MyModApiImpl());
}

// Consumer mod retrieves the contract
public void Initialize(IModApi api)
{
    if (api.TryGetContract<IMyModApi>(out var myMod))
        myMod.DoSomething();
    // otherwise gracefully degrade — provider mod is not loaded
}
```

When the provider mod is unloaded, all of its contracts are automatically revoked. Subsequent `TryGetContract` calls return `false`.

## See also

- [MOD_PIPELINE](./MOD_PIPELINE.md) — integration pipeline, validation, atomic graph rebuild
- [CONTRACTS](./CONTRACTS.md)
- [ISOLATION](./ISOLATION.md)
- [ARCHITECTURE](./ARCHITECTURE.md)