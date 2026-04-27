# Power Events

## Purpose
Power-grid events (electricity and ether): power requests, grants, overload.
The same two-step Intent pattern used for mana / ammunition, applied at the
building level.

## Dependencies
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Contents
- `PowerRequestEvent.cs` — a building requests wattage for the tick.
- `PowerGrantedEvent.cs` — PowerSystem confirms the grant.
- `GridOverloadEvent.cs` — grid overload: consumption exceeds production.

## Rules
- Two independent networks: Electric and Ether. The events carry a `PowerType`
  so PowerSystem can route the flows.
- On `GridOverloadEvent`, low-priority consumers shut down; the shutdown order
  lives in PowerSystem.
- Absence of `PowerGrantedEvent` for the tick = the building did not run;
  derived systems (WorkbenchSystem) MUST account for that.

## Usage examples
```csharp
_bus.Publish(new PowerRequestEvent { /* ConsumerId = forge, Type = PowerType.Ether, Watts = 50 */ });
// → PowerSystem publishes PowerGrantedEvent on success, GridOverloadEvent on shortfall
```

## TODO
- [ ] Decide: separate `GridOverload` events for Electric and Ether, or a shared event with the kind in the body.
- [ ] Add `PowerShutdownEvent` for forced shutdown (Phase 4).
