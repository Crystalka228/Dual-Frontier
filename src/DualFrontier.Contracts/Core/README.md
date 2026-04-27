# Core — ECS marker interfaces

## Purpose
Defines the base ECS types on which the entire domain model is built: the
`EntityId` identifier and marker interfaces for components, events, queries,
and commands. This is the foundation — every other assembly uses these types
as the vocabulary for communication.

## Dependencies
- `System` (BCL)

## Contents
- `IEntity.cs` — entity marker (used rarely, mostly for ADT completeness).
- `EntityId.cs` — `readonly record struct` identifier with a version field for
  detecting dead references after an entity is destroyed.
- `IComponent.cs` — component marker (pure data).
- `IEvent.cs` — domain-bus event marker.
- `IQuery.cs` — synchronous query marker (a question to a bus).
- `IQueryResult.cs` — query-response marker.
- `ICommand.cs` — command marker (an imperative action).

## Rules
- All types here are markers or immutable value types.
- Never add methods to a marker interface: it breaks every existing implementation.
- A change to `EntityId` is a breaking change for the Save/Load format.

## Usage examples
```csharp
public sealed record DamageEvent(EntityId Target, float Amount) : IEvent;

public sealed class HealthComponent : IComponent
{
    public float Current;
    public float Maximum;
}

EntityId id = EntityId.Invalid; // "no entity" check
```

## TODO
- [ ] Phase 0 — settle the `EntityId` version-field format (int vs ushort).
- [ ] Phase 2 — add `IQuery<TResult>` for type-safe queries.
- [ ] Phase 2 — cover `EntityId` with equality and hash-code tests.
