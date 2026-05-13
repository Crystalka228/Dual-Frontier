---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-CODING_STANDARDS
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-CODING_STANDARDS
---
# Coding standards

A single code style is not aesthetics — it is a navigation tool: when reading another file, a developer does not spend attention decoding someone else's habits. In Dual Frontier the standards are formalized and checked by the analyzer; deviations are caught by `dotnet build` with `TreatWarningsAsErrors`.

## Naming

### Public and protected members — PascalCase

```csharp
public sealed class HealthComponent : IComponent
{
    public float Current;
    public float Maximum;
    public void TakeDamage(float amount) { /* ... */ }
}
```

### Private fields — `_camelCase`

```csharp
private readonly Dictionary<Type, IComponentStore> _stores = new();
private int _nextEntityId;
```

The underscore tells a private field apart from a method parameter or a local variable at a glance. The IDE highlights them differently, but the code still must be readable in `git diff` and `github review`.

### Constants — PascalCase

```csharp
public const int MaxPawnsPerColony = 100;
```

### Interfaces — `I` prefix

```csharp
public interface IEventBus { /* ... */ }
public interface IComponent { }
```

### Generic parameters — `T` or `TContext`-style

```csharp
public interface IComponentStore<T> where T : IComponent { /* ... */ }
```

### Files and namespaces match the path

The file `src/DualFrontier.Core/ECS/World.cs` contains `namespace DualFrontier.Core.ECS;` and a single `World` type. The reverse holds too: a namespace always mirrors the physical directory structure.

## File-scoped namespaces

Only the file-scoped syntax (C# 10+) is used:

```csharp
// CORRECT
namespace DualFrontier.Core.ECS;

public sealed class World { /* ... */ }

// WRONG — an extra nesting level
namespace DualFrontier.Core.ECS
{
    public sealed class World { /* ... */ }
}
```

Less indentation → more useful code on screen. Checked by analyzer IDE0161.

## Nullable enabled

Every project is built with `<Nullable>enable</Nullable>` in `Directory.Build.props`. No `#nullable disable`; no `!` without justification.

```csharp
// CORRECT
public string? OptionalName { get; init; }
public string RequiredName { get; init; } = "";

// BAD
public string SomeName { get; init; } = null!;  // only in a DTO with required and init-only.
```

If an API must return `null`, it returns `T?`. If it must not, it returns `T`. A `null` in an unmarked field is grounds for a bug report.

## Code comments are English

All code comments — inline, block, and XML doc — are in English. This applies uniformly to public API, internal implementation, domain logic, and game-design notes. The i18n campaign that closed in v0.3 made this a hard rule: no Russian in source files except inside `SESSION_PHASE_4_CLOSURE_REVIEW.md`, which is preserved verbatim as audit trail.

Domain terminology that has a Russian origin (pawn, golem, ether node, ritual, ammo crystal) is referenced by its English name everywhere in code; the canonical Russian↔English mapping lives in `TRANSLATION_GLOSSARY.md`.

```csharp
// Ether breakdown: a mage works a crystal above their tier.
// Formula from GDD 4.2: fail chance = (crystalTier - mageLevel) × 0.25.
if (crystal.Tier > mage.EtherLevel)
{
    var failChance = (crystal.Tier - mage.EtherLevel) * 0.25f;
    // ...
}
```

## English XML docs — public API

The public API is everything visible outside the assembly. Its XML documentation is written in English: docs are generated into `bin/` and shipped with the NuGet package, where any third-party developer may read them.

```csharp
/// <summary>
/// Publishes an event to the domain bus. The event is delivered synchronously
/// to all subscribers within the current scheduler phase. Use [Deferred] for
/// cross-phase delivery, [Immediate] to interrupt the phase.
/// </summary>
/// <typeparam name="T">Event type; must implement IEvent.</typeparam>
/// <param name="evt">The event instance to publish.</param>
public void Publish<T>(T evt) where T : IEvent;
```

XML docs use the `<summary>` / `<remarks>` / `<param>` / `<returns>` / `<exception>` set; inline `//` comments cover formulas, game-design notes, and reasoning that does not belong in the public contract. Both stay English.

## One class per file

Navigation principle: open the IDE → type the class name → see the file. The exceptions are very narrow:

- An enum or `readonly struct` up to 10 lines, logically paired with the main class (for example, `HealthComponent` and its related `HealthStatus` enum).
- A private nested class logically inseparable from its owner (state-machine sub-classes).

No files with 5+ classes. No "utility" dumping grounds like a 500-line `Helpers.cs`.

## Class member order

A single order, top-down by what the reader needs first:

1. `const` fields.
2. `static readonly` fields.
3. `private readonly` fields.
4. `private` mutable fields.
5. Constructors.
6. `public` properties.
7. `public` methods.
8. `protected` methods.
9. `private` methods.
10. Nested types.

```csharp
public sealed class DomainEventBus : IEventBus
{
    private const int InitialCapacity = 16;

    private static readonly object SubscribeLock = new();

    private readonly ConcurrentDictionary<Type, Delegate> _subscribers = new();
    private int _publishCount;

    public DomainEventBus() { }

    public int PublishCount => _publishCount;

    public void Publish<T>(T evt) where T : IEvent { /* ... */ }
    public void Subscribe<T>(Action<T> handler) where T : IEvent { /* ... */ }

    private void RecordMetric() { /* ... */ }
}
```

The reviewer sees at once — which fields, which constructor, which public API. No need to scroll.

## Additional rules

- `using` — sorted: `System.*` first, then `DualFrontier.*`, then the rest. IDE auto-format.
- `var` — when the type is obvious from the RHS (`new`, cast, factory). Otherwise an explicit type.
- `async` is forbidden in Domain (see [THREADING](/docs/architecture/THREADING.md)). Allowed in Application and Presentation.
- Magic numbers go into a `const` with a name that describes the meaning. `4.2f` in code without a comment is a mortal sin.
- Returning `null` from a public API only when it is an explicit part of the contract (`TryGet` and `T? FindBy(...)`).

## Stack-frame retention for collected resources

Some code paths must give the GC an opportunity to collect a resource — `AssemblyLoadContext` unload, finalizer-based cleanup, weak-reference-based cache eviction. These paths run after every strong reference to the resource has been dropped from the executing thread's stack frames. Two C# constructs silently retain strong refs in ways that defeat such code paths if not handled with discipline: the iteration variable of a `foreach` loop (in DEBUG builds, until the enclosing method returns) and **lambda closure display classes** (always, in any build).

A lambda capturing a local variable causes the C# compiler to synthesize a heap `<>c__DisplayClass` object containing that variable as a field. The display class is allocated on entry to the enclosing method and rooted from that method's stack frame for the entire method scope. Any local captured by even one lambda is strongly reachable until the enclosing method returns, regardless of how out-of-scope the local appears textually.

```csharp
// BAD — the display class for the step lambdas is allocated in this
// method's frame and keeps `mod` (and thereby mod.Context) alive
// through the spin. WR.IsAlive never flips to false on real mods;
// the spin times out on every invocation and emits ModUnloadTimeout.
public IReadOnlyList<ValidationWarning> UnloadMod(string modId)
{
    if (!_activeMods.TryGetValue(modId, out LoadedMod? mod)) return [];

    var warnings = new List<ValidationWarning>();
    TryUnloadStep(1, modId, warnings, () => mod.Api?.UnsubscribeAll());
    // ... more lambdas capturing mod ...

    var alcRef = new WeakReference(mod.Context);
    _activeMods.Remove(mod);

    // Display class still rooted by this frame; mod.Context still strongly reachable.
    for (int i = 0; i < 100; i++)
    {
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        if (!alcRef.IsAlive) return warnings;
        Thread.Sleep(100);
    }
    warnings.Add(new ValidationWarning(modId, "ModUnloadTimeout: ..."));
    return warnings;
}
```

```csharp
// CORRECT — the work-with-refs phase is isolated in a non-inlined
// helper that returns only the WeakReference. The caller's frame
// holds nothing strong, so the spin can observe the release.
public IReadOnlyList<ValidationWarning> UnloadMod(string modId)
{
    if (!_activeMods.TryGetValue(modId, out LoadedMod? mod)) return [];

    var warnings = new List<ValidationWarning>();
    WeakReference alcRef = RunUnloadSteps1Through6AndCaptureAlc(modId, mod, warnings);

    // mod is no longer referenced here; the display class lives only
    // in the helper's frame, which has returned. Frame is clean.
    TryStep7AlcVerification(modId, alcRef, warnings);
    return warnings;
}

[MethodImpl(MethodImplOptions.NoInlining)]
private WeakReference RunUnloadSteps1Through6AndCaptureAlc(
    string modId, LoadedMod mod, List<ValidationWarning> warnings)
{
    TryUnloadStep(1, modId, warnings, () => mod.Api?.UnsubscribeAll());
    // Lambdas captured here are confined to THIS frame's display class.
    var alcRef = new WeakReference(mod.Context);
    _activeMods.Remove(mod);
    return alcRef;
}

[MethodImpl(MethodImplOptions.NoInlining)]
private static void TryStep7AlcVerification(
    string modId, WeakReference alcRef, List<ValidationWarning> warnings)
{ /* spin with the GC pump bracket; signature carries no resource ref */ }
```

The pattern surfaced empirically in M7.3 (commit `9bed1a4`): an initial implementation kept `mod` as a local in `UnloadMod`, and three of the M7.2 regression tests started emitting `ModUnloadTimeout` warnings the assertions were not expecting. The root cause was not the JIT retaining the explicit local — it was the C# compiler's display-class hoisting for the lambdas passed to `TryUnloadStep`. Step 1's lambda captures `mod` (`mod.Api?.UnsubscribeAll()`), so the compiler synthesized a heap `<>c__DisplayClass` holding `mod` as a field, allocated on entry to `UnloadMod` and rooted from `UnloadMod`'s stack frame for the entire method scope. The same pathology surfaced in `UnloadAll`'s `foreach (LoadedMod mod in _activeMods)` snapshot loop: the iteration variable's last value persisted in the DEBUG stack slot through the rest of the method (fixed by extracting `SnapshotActiveModIds`).

Rule of thumb: if a method needs to give the GC a chance to collect a resource it referenced, that method's body must split into **(a)** a non-inlined helper that captures, works with, and releases its strong refs to the resource, returning only a `WeakReference` (or nothing at all), and **(b)** the caller, which invokes the wait/spin phase with no resource-holding locals or lambdas in its frame.

The `[MethodImpl(MethodImplOptions.NoInlining)]` attribute is non-negotiable on both helpers — without it the JIT may inline the helper back into the caller, recreating the display class in the caller's frame and defeating the discipline. The attribute MUST sit on every helper that exists for the sole purpose of containing strong refs to a resource that subsequently must be released.

Reuse this pattern for any future code path with similar shape: ALC unload (M7.x), finalizer-driven cleanup, weak-reference-based caches, or any GC-dependent test assertion. Test-side helpers (e.g. `ModUnloadAssertions.AssertAlcReleasedWithin`) follow the same discipline as production-side helpers (`TryStep7AlcVerification`).

## See also

- [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
- [ISOLATION](/docs/architecture/ISOLATION.md)
- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md)
