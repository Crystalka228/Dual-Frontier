# System isolation

A silent isolation violation is worse than a crash: corrupted state surfaces an hour into play as an inexplicable bug. Dual Frontier installs an active isolation guard: every component access passes through `SystemExecutionContext`, and an undeclared access produces an immediate exception with diagnostics.

## Two response modes: crash vs soft unload

Behavior on an isolation violation depends on the system's origin.

| Origin                            | Behavior                                      | Reason                                            |
| --------------------------------- | --------------------------------------------- | ------------------------------------------------- |
| `DualFrontier.Systems` (your code) | `IsolationViolationException` → **crash**     | A developer bug; MUST be fixed                    |
| User mod                          | `ModIsolationException` → **mod is unloaded** | The game continues; the user sees a banner       |

Each system is tagged with its origin when registered with the scheduler:

```csharp
internal enum SystemOrigin
{
    Core,   // DualFrontier.Systems — crash on violation
    Mod     // Loaded via ModLoader — unload the mod on violation
}
```

## SystemExecutionContext

The execution context lives in `ThreadLocal<SystemExecutionContext>` and is active for the entire duration of the `system.Update(delta)` call. Before the call, the scheduler invokes `PushContext(system)` and places into the context:

- The system name (for messages).
- The set of permitted readable component types.
- The set of permitted writable component types.
- The active bus name.
- A reference to `World` (only inside the context — `internal` from the outside).
- `SystemOrigin` — the system's origin (Core or Mod).
- `ModId` — the mod's identifier (if `Origin == Mod`).

```csharp
internal T GetComponent<T>(EntityId id) where T : IComponent
{
    #if DEBUG
    if (!_allowedReads.Contains(typeof(T))
     && !_allowedWrites.Contains(typeof(T)))
    {
        var message =
            $"[ISOLATION VIOLATED]\n" +
            $"System '{_systemName}'\n" +
            $"accessed '{typeof(T).Name}'\n" +
            $"without an access declaration.\n" +
            $"Add: [SystemAccess(reads: " +
            $"new[]{{typeof({typeof(T).Name})}})]";

        if (_systemOrigin == SystemOrigin.Core)
        {
            throw new IsolationViolationException(message);
        }
        else
        {
            _modFaultSink.ReportFault(_modId!, message);
            throw new ModIsolationException(message);
        }
    }
    #endif
    return _world.GetComponentUnsafe<T>(id);
}
```

After `system.Update`, the scheduler calls `PopContext`, which clears the `ThreadLocal` value. The next system sees a clean context.

## ModFaultHandler — lifecycle on a mod violation

When a mod system catches `ModIsolationException` (or any other unhandled exception from the mod assembly), control passes to `ModFaultHandler`. It executes a strict sequence of steps:

1. **Log** — the full message and stack trace are written to `logs/mods/<mod-id>.log`, tagged with `ModId` and the system name.
2. **Unsubscribe systems from buses** — every `EventBus` subscription from the mod's systems is removed to prevent further calls into the now-dead mod.
3. **Removal from the scheduler** — the mod's systems are evicted from `ParallelSystemScheduler` so the next tick does not touch them.
4. **`IMod.Unload` with a timeout** — user code is given a bounded time to release resources cleanly; the call is aborted when the timeout elapses.
5. **`AssemblyLoadContext` unload** — `ModLoadContext.Unload()`, GC wait, physical release of the mod's assembly.
6. **Publish `ModDisabledEvent`** — the UI receives the event and shows the user a banner.

Any unhandled exception that originates in a mod assembly (including `ModIsolationException` from `SystemExecutionContext`) follows this same path. The core does not crash, the mod is unloaded, the game continues.

## What the user sees

After a mod author's mod is unloaded, the user gets a banner:

```
Mod "<name>" violated isolation
and was automatically disabled.
The game will continue without it.
Details in log: logs/mods/<mod-id>.log
```

On the next game start, this mod remains disabled and is marked in the mod list as "disabled due to error." The user can enable it manually once the author fixes the bug.

## DEBUG vs RELEASE

The full isolation guard costs ~2% CPU, which is too much for release. The modes differ.

### DEBUG (development, tests, CI)

- Read check: the component MUST be in `reads` or `writes`.
- Write check: the component MUST be in `writes`.
- Bus check: `Publish` is allowed only on the `bus` named in the attribute.
- Async check: the stack trace is analyzed for `AsyncStateMachine`.
- Direct-access check: `World`, `ComponentStore`, and `GetSystem` always crash.

### RELEASE (production build)

- Direct-access check: `GetSystem` **always crashes, even in Release**. This is a semantic violation, not an optimization concern.
- `ModFaultHandler` is active in Release — soft mod unload works in production.
- The remaining checks are disabled via `#if DEBUG`.

This way the developer writes code with the guarantee that every contract error is caught during development. Production retains only protection against the grossest violations — so that an experimental mod cannot corrupt a save.

## Types of violations

### Access to an undeclared component

The most common case: a system decided to "quickly" read a component without updating the attribute.

```csharp
[SystemAccess(reads: new[] { typeof(PositionComponent) })]
public sealed class WrongSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entity in Query<PositionComponent>())
        {
            var health = GetComponent<HealthComponent>(entity); // crash
        }
    }
}
```

Message:

```
[ISOLATION VIOLATED]
System 'WrongSystem'
accessed 'HealthComponent'
without an access declaration.
Add: [SystemAccess(reads: new[]{typeof(HealthComponent)})]
```

Core system → crash. Mod system → the mod is unloaded.

### Direct access to another system

A direct call breaks the decoupling and makes code unpredictable under parallel execution.

```csharp
// BAD
var inventory = GetSystem<InventorySystem>();
```

The isolation guard throws (even in Release):

```
[IsolationViolationException]
Direct access to systems is forbidden.
Use the EventBus instead of a direct system reference.
```

### Direct use of `World`

`World` is `internal`, but `InternalsVisibleTo` exposes it to `Systems`. The isolation guard nonetheless ensures that systems do not access `World` directly — any `world.CreateEntity()` call from `Update` goes through `SystemBase.CreateEntity`, which registers the operation in the context and checks permissions.

```
[IsolationViolationException]
System 'XxxSystem' called World.GetComponentUnsafe directly.
Use SystemBase.GetComponent / Query instead of direct World access.
```

### Publishing to a non-declared bus

`[SystemAccess(bus: nameof(IGameServices.Combat))]` means the system publishes only to the Combat bus.

```
[IsolationViolationException]
System 'CombatSystem' publishes 'ItemAddedEvent'
to bus 'Inventory' but declares only 'Combat'.
Either change the event's bus or add it to the system's declaration.
```

## Message format

All messages are built on a single template:

```
[HEADER]
System '<name>'
<what went wrong>
<hint to fix it>
```

Mandatory elements:

- The system name (including namespace).
- Exactly what went wrong, with type and field.
- A ready-to-paste hint — a line of code that can be copied directly into the attribute.

A message without a hint is considered insufficient: Claude, a mod author, or a junior developer MUST be able to fix the issue from the stack trace without consulting documentation.

## System addition checklist

Before `git commit` for a new system, run through the checklist:

1. The class inherits from `SystemBase`.
2. The `[SystemAccess(reads: [...], writes: [...], bus: ...)]` attribute is present.
3. The `[TickRate(TickRates.XXX)]` attribute is set explicitly.
4. EVERY component used in `Update` is listed in `reads` / `writes`.
5. `Bus` matches the actual publication inside handlers.
6. `OnInitialize()` is overridden for subscriptions; `OnDispose()` unsubscribes on unload.
7. No `async` / `await` / `Task` inside system code.
8. No direct references to other systems or to `World`.
9. Isolation-guard unit test: a test in which the system deliberately violates its declaration expects `IsolationViolationException`.
10. Integration test: the system runs as part of `ParallelSystemScheduler` without errors.

If any item is not satisfied, the PR is rejected.

## See also

- [ECS](./ECS.md)
- [THREADING](./THREADING.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
