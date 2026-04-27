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

## Russian-language domain comments

Internal domain logic is commented in Russian. That is the project's working language: GDD, tech architecture, issues, reviews. Mixing Russian and English in one file's comments is unnecessary.

```csharp
// Эфирный срыв: маг работает с кристаллом выше своего уровня.
// По формуле из GDD 4.2: шанс провала = (кристаллТир - магУровень) × 0.25.
if (crystal.Tier > mage.EtherLevel)
{
    var failChance = (crystal.Tier - mage.EtherLevel) * 0.25f;
    // ...
}
```

Domain terms stay in Russian inside comments: "пешка" (pawn), "голем" (golem), "эфирный узел" (ether node), "ритуал" (ritual), "кристалл-боеприпас" (ammo crystal).

## English XML docs — public API

The public API is everything visible outside the assembly. Its XML documentation is written in English: docs are generated into `bin/` and shipped with the NuGet package, where any third-party developer (including non-Russian speakers) may read them.

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

Split rule: `/// <summary>` + `<remarks>` are in English; `//` inline comments about a specific formula or game-design point are in Russian.

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
- `async` is forbidden in Domain (see [THREADING](./THREADING.md)). Allowed in Application and Presentation.
- Magic numbers go into a `const` with a name that describes the meaning. `4.2f` in code without a comment is a mortal sin.
- Returning `null` from a public API only when it is an explicit part of the contract (`TryGet` and `T? FindBy(...)`).

## See also

- [ARCHITECTURE](./ARCHITECTURE.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
- [ISOLATION](./ISOLATION.md)
