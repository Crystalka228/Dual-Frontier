---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ISOLATION
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.1"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ISOLATION
---
# System isolation

A silent isolation violation is worse than a crash: corrupted state surfaces an hour into play as an inexplicable bug. Dual Frontier enforces system isolation at **compile time** via the `[SystemAccess]` attribute and the `DependencyGraph` that consumes it for parallel scheduling. The future A'.9 Roslyn analyzer extends this enforcement to call sites — flagging undeclared component access as a build error rather than a runtime exception.

## Enforcement model

Isolation is enforced by three mechanisms operating at different stages:

1. **Compile-time declarations** — every `SystemBase` subclass must carry a `[SystemAccess(reads, writes, bus)]` attribute. The attribute is the single source of truth for what components the system reads, writes, and which event bus it publishes to.
2. **DependencyGraph edge-building** — `ParallelSystemScheduler` reads `[SystemAccess]` declarations to compute the system dependency graph: writers-vs-readers edges, bus contention, phase ordering. Systems with no edge between them run on parallel scheduler threads; systems with read-write conflicts on the same component run sequentially.
3. **A'.9 Roslyn analyzer (planned)** — call-site enforcement: any `NativeWorld.AcquireSpan<T>()` or `BeginBatch<T>()` whose `T` is not declared in the enclosing system's `[SystemAccess]` raises a build error. The analyzer milestone lands in A'.9 per [PHASE_A_PRIME_SEQUENCING](./PHASE_A_PRIME_SEQUENCING.md).

The runtime guard methods that previously threw `IsolationViolationException` from `SystemExecutionContext.GetComponent` / `SetComponent` were **deleted in K8.3+K8.4 cutover (A'.5 closure 2026-05-14)**. Systems now read and write component storage exclusively through `NativeWorld`'s span/batch API (see [ECS](./ECS.md)). The compile-time + analyzer model replaces the runtime check; until A'.9 lands, undeclared access is caught by manual review and by integration tests that exercise the scheduler.

## SystemExecutionContext

`SystemExecutionContext` is still active per-tick — it carries scheduler context that systems reach implicitly:

- The active **scheduler thread** and system name (for diagnostics).
- A reference to **`NativeWorld`** (the sole production storage backend after K8.3+K8.4).
- The **`IGameServices` aggregator** for bus publication.
- The system's `SystemOrigin` (`Core` or `Mod`) and `ModId` (when origin is `Mod`).
- The fault sink for mod-origin systems (`IModFaultSink`).
- The Path β `IManagedStorageResolver` for per-mod managed-class storage (K-L3.1 bridge — see [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §9.5).

The context is pushed by `ParallelSystemScheduler` before each `SystemBase.Update` call and popped after (always, including on exceptions). It lives in a `ThreadLocal<SystemExecutionContext?>` slot — each scheduler thread has its own current context. Systems reach the context through `SystemBase.NativeWorld`, `SystemBase.Services`, and `SystemBase.ManagedStore<T>()`. Out-of-context calls (for example from the renderer main thread, or from a system that suspended via `async`/`await` and resumed on a different thread) fail loudly with `InvalidOperationException`.

`async`/`await` inside system code is forbidden by [THREADING §11.7](./THREADING.md): suspending would resume on a different scheduler thread where `Current` is null, breaking the per-thread context model.

## Two response modes: core crash vs mod soft-unload

When a system's `Update` throws an unhandled exception, behavior depends on the system's origin.

| Origin                            | Behavior                                                | Reason                                            |
| --------------------------------- | ------------------------------------------------------- | ------------------------------------------------- |
| `DualFrontier.Systems` (core)     | Exception propagates → **process crashes**              | A developer bug; MUST be fixed                    |
| User mod                          | Exception caught by `ModFaultHandler` → **mod unloaded** | The game continues; the user sees a banner        |

Each system is tagged with its origin when registered with the scheduler:

```csharp
internal enum SystemOrigin
{
    Core,   // DualFrontier.Systems — exception propagates, crashes the process
    Mod     // Loaded via ModLoader — exception triggers ModFaultHandler unload
}
```

This split was historically anchored on `IsolationViolationException` / `ModIsolationException` thrown by the deleted runtime guard. After K8.3+K8.4 the trigger is generalized to "any unhandled exception from system code" — the dispatch on origin is unchanged.

## ModFaultHandler — lifecycle on a mod fault

When a mod system raises any unhandled exception, control passes to `ModFaultHandler`. It executes a strict sequence of steps:

1. **Log** — the full message and stack trace are written to `logs/mods/<mod-id>.log`, tagged with `ModId` and the system name.
2. **Unsubscribe systems from buses** — every `EventBus` subscription from the mod's systems is removed to prevent further calls into the now-dead mod.
3. **Removal from the scheduler** — the mod's systems are evicted from `ParallelSystemScheduler` so the next tick does not touch them.
4. **`IMod.Unload` with a timeout** — user code is given a bounded time to release resources cleanly; the call is aborted when the timeout elapses.
5. **`AssemblyLoadContext` unload** — `ModLoadContext.Unload()`, GC wait, physical release of the mod's assembly per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §9.5.
6. **Publish `ModDisabledEvent`** — the UI receives the event and shows the user a banner.

The core does not crash, the mod is unloaded, the game continues.

## What the user sees

After a mod's exception triggers unload, the user gets a banner:

```
Mod "<name>" raised an unhandled exception
and was automatically disabled.
The game will continue without it.
Details in log: logs/mods/<mod-id>.log
```

On the next game start, this mod remains disabled and is marked in the mod list as "disabled due to error." The user can enable it manually once the author fixes the bug.

## System addition checklist

Before `git commit` for a new system, run through the checklist:

1. The class inherits from `SystemBase`.
2. The `[SystemAccess(reads: [...], writes: [...], bus: ...)]` attribute is present.
3. The `[TickRate(TickRates.XXX)]` attribute is set explicitly.
4. EVERY component the system uses in `Update` is listed in `reads` / `writes` — the future A'.9 analyzer will enforce this at build time; until then, manual review and integration tests catch gaps.
5. `Bus` matches the actual publication inside handlers.
6. `OnInitialize()` is overridden for subscriptions; `OnDispose()` unsubscribes on unload.
7. No `async` / `await` / `Task` inside system code — would break the per-thread context model.
8. No direct references to other systems — use `IGameServices` buses for cross-system communication.
9. Component access goes through `NativeWorld.AcquireSpan<T>()` or `BeginBatch<T>()` (Path α) or `SystemBase.ManagedStore<T>()` for Path β — never a managed `World` reference (none exists in production after K8.3+K8.4).
10. Integration test: the system runs as part of `ParallelSystemScheduler` without errors.

If any item is not satisfied, the PR is rejected.

## See also

- [ECS](./ECS.md) — `NativeWorld`, `EntityId`, `SpanLease<T>`, `WriteBatch<T>`, `SystemBase` accessors.
- [THREADING](./THREADING.md) — scheduler thread model, `async`/`await` ban, `DependencyGraph` edge-building.
- [PHASE_A_PRIME_SEQUENCING](./PHASE_A_PRIME_SEQUENCING.md) — A'.9 Roslyn analyzer milestone position.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §9.5 — mod ALC unload chain referenced from step 5.
- [TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md) — integration testing systems under scheduler.
