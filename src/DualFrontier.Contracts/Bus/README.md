# Bus — Domain event buses

## Purpose
Defines the base event-bus contract `IEventBus` and the six domain buses
(Combat, Inventory, Magic, Pawn, Power, World), aggregated under `IGameServices`.
Splitting buses by domain reduces lock contention, simplifies debugging, and
makes per-domain load profiling possible.

## Dependencies
- `DualFrontier.Contracts.Core` (the `IEvent` marker).

## Contents
- `IEventBus.cs` — the base contract for a single-domain bus (Publish/Subscribe/Unsubscribe).
- `IGameServices.cs` — aggregator of every domain bus, the access point for systems.
- `ICombatBus.cs` — combat bus: ShootAttempt, DamageEvent, DeathEvent.
- `IInventoryBus.cs` — inventory bus: AmmoRequest/Result, ItemAdded/Removed.
- `IMagicBus.cs` — magic bus: ManaRequest/Result, SpellCast, EtherSurge.
- `IPawnBus.cs` — pawn bus: MoodBreak, DeathReaction, SkillGain.
- `IPowerBus.cs` — power-grid bus: ConverterPowerOutput.
- `IWorldBus.cs` — world bus: EtherNodeChanged, WeatherChanged, RaidIncoming.

## Rules
- Each domain bus is a separate instance with its own subscription set.
- A system writes only to "its" bus (declared via `[SystemAccess(bus: ...)]`).
- A subscription MUST be removed on disposal — otherwise it leaks memory.
- Handlers are invoked synchronously. A handler MUST NOT block — that stalls the
  entire scheduler phase.

## Usage examples
```csharp
public sealed class CombatSystem
{
    private readonly IGameServices _bus;

    public CombatSystem(IGameServices bus) => _bus = bus;

    public void OnShot(EntityId shooter)
        => _bus.Combat.Publish(new ShootAttemptEvent(shooter));
}
```

## TODO
- [x] Phase 1 — `DomainEventBus` implemented with a `ConcurrentDictionary` of subscriptions.
- [x] Phase 1 — `GameServices` implemented as the composition of the six buses.
- [ ] Phase 2 — add metrics (events/sec per bus) for the profiler.
