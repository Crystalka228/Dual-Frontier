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
public interface IModApi
{
    void RegisterComponent<T>() where T : IComponent;
    void RegisterSystem<T>()    where T : SystemBase;

    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(Action<T> handler) where T : IEvent;
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;

    // Publishes a contract for other mods
    void PublishContract<T>(T contract) where T : IModContract;

    // Retrieves another mod's contract (optional)
    bool TryGetContract<T>(out T contract) where T : IModContract;

    // Logging to the shared log with the mod-id prefix
    void Log(string message);
    void LogWarning(string message);
    void LogError(string message);
}
```

Anything not in `IModApi` is unreachable. `IModApi` is implemented inside the core (`RestrictedModApi`) and proxies calls through the isolation guard: system registration checks for the `[SystemAccess]` attribute, component registration is stored in `ComponentRegistry` tagged with the mod-id.

| Action                                | Allowed | Reason                                            |
|---------------------------------------|---------|---------------------------------------------------|
| Publish events to a bus               | Yes     | Through `IModApi` — proxied                       |
| Subscribe to events                   | Yes     | Through `IModApi` — proxied                       |
| Register components                   | Yes     | Through `IModApi`                                 |
| Register systems                      | Yes     | Through `IModApi` + READ/WRITE declaration        |
| Publish a contract for mods           | Yes     | `IModContract` — a public interface               |
| Obtain a reference to `World`         | No      | `AssemblyLoadContext` blocks it                   |
| Obtain a reference to a system        | No      | Isolation guard — crash                           |
| Load `DualFrontier.Core`              | No      | `AssemblyLoadContext` blocks it                   |
| Bypass `EventBus` directly            | No      | Physically no reference                           |

## AssemblyLoadContext — what is physically unreachable

When asked to load an assembly, `ModLoadContext` inspects the name:

- `DualFrontier.Contracts` — passed through from the main context (shared).
- `DualFrontier.Core`, `DualFrontier.Systems`, `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Application` — **refused**. The mod receives `ModIsolationException`.
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

Every mod contains a manifest file at the package root.

```json
{
  "id": "com.example.voidmagic",
  "name": "Void Magic",
  "description": "Adds the Void school of magic.",
  "author": "Example Modder",
  "version": "1.2.0",
  "requiresContracts": "^1.0.0",
  "dependencies": [],
  "optionalDependencies": [
    { "id": "com.example.artifactmod", "version": "^0.5.0" }
  ],
  "entryAssembly": "VoidMagic.dll",
  "entryType": "VoidMagic.VoidMagicMod"
}
```

- `id` — a unique identifier in reverse-domain style.
- `version` — the mod's SemVer.
- `requiresContracts` — minimum `DualFrontier.Contracts` version. The loader refuses if the core has a different major version than the requirement.
- `dependencies` — required mods; their absence blocks loading.
- `optionalDependencies` — optional; signal that integration via a contract is possible, but the mod also works without them.
- `entryAssembly` / `entryType` — the assembly name and the FQN of the class implementing `IMod`.

## Hot reloading

`ModLoader` supports unloading through `ModLoadContext.Unload()`. The sequence:

1. The scheduler pauses between phases.
2. `OnDestroy` is called on every system registered by the mod.
3. `Unload()` is called on the mod.
4. The `IModApi` implementation removes any remaining subscriptions (a safety net).
5. `ModLoadContext.Unload()` releases the assembly (after the next GC).

Limitation: `AssemblyLoadContext.Unload` is collaborative. If a mod left a reference to its type in another mod's static field, unloading will not complete. That is why `OnDestroy` is critical.

Hot reload (updating a mod without exiting the game) = Unload + Load with the new version. No save is required — `World` does not change, only the set of systems and subscriptions.

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
            api.Log("MyFirstMod initialized");
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

| Method                   | What it does                                  | Constraint                                         |
|--------------------------|-----------------------------------------------|----------------------------------------------------|
| `RegisterComponent<T>()` | Adds a type to the ECS                        | `T : IComponent`, once per type                    |
| `RegisterSystem<T>()`    | Adds a system to the scheduler                | `T : class`, requires `[SystemAccess]` and `[TickRate]` |
| `Subscribe<T>(handler)`  | Subscribe to a bus event                      | Removed automatically on `Unload`                  |
| `Publish<T>(evt)`        | Publish an event                              | Only to the bus declared in the mod's `[SystemAccess]` |
| `PublishContract<T>(c)`  | Publish a contract for other mods             | `T : IModContract`                                 |
| `TryGetContract<T>(out)` | Retrieve another mod's contract               | Graceful degrade if the mod is not loaded          |

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
